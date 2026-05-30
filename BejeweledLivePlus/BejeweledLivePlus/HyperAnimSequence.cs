using System;
using System.IO;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace BejeweledLivePlus
{
	public class HyperAnimSequence
	{
		public int mFrameCount;

		public int mLightCount;

		public HyperAnimKey[] mCamera;

		public HyperAnimKey[] mBoard;

		public HyperAnimKey[,,] mGems;

		public HyperAnimKey[,] mLights;

		public int[,] mGemHitFrame = new int[GlobalMembers.NUM_ROWS, GlobalMembers.NUM_COLS];

		public float mTickF;

		public int GetCurFrame()
		{
			return (int)mTickF;
		}

		public bool IsComplete()
		{
			return mTickF >= mFrameCount - 1.01f;
		}

		public int GetGemHitFrame(int row, int col)
		{
			return mGemHitFrame[row, col];
		}

		public void Reset()
		{
			mTickF = 0f;
		}

		public void Tick()
		{
			mTickF += 0.5f;
			float aMaxTickF = mFrameCount - 1.01f;
			if (mTickF >= aMaxTickF) mTickF = aMaxTickF;
		}

		private static float EulerInterpolate(float from, float to, float u)
		{
			float twoPi = (float)(Math.PI * 2.0);
			float diff = (to - from) % twoPi;
			if (diff < 0f) diff += twoPi;
			if (diff > Math.PI) diff = -1.0f * (twoPi - diff);
			float angle = (from + diff * u) % twoPi;
			if (angle < 0f) angle += twoPi;
			return angle;
		}

		private static SexyVector3 LerpV3(SexyVector3 a, SexyVector3 b, float t)
		{
			float oma = 1f - t;
			return new SexyVector3(a.x * oma + b.x * t, a.y * oma + b.y * t, a.z * oma + b.z * t);
		}

		private static SexyVector3 EulerV3(SexyVector3 a, SexyVector3 b, float t)
		{
			return new SexyVector3(EulerInterpolate(a.x, b.x, t), EulerInterpolate(a.y, b.y, t), EulerInterpolate(a.z, b.z, t));
		}

		public SexyVector3 GetGemPos(int row, int col)
		{
			int f = (int)mTickF; float a = mTickF - f;
			return LerpV3(mGems[row, col, f].mPos, mGems[row, col, f + 1].mPos, a);
		}
		public SexyVector3 GetGemRot(int row, int col)
		{
			int f = (int)mTickF; float a = mTickF - f;
			return EulerV3(mGems[row, col, f].mRot, mGems[row, col, f + 1].mRot, a);
		}
		public SexyVector3 GetGemScale(int row, int col)
		{
			int f = (int)mTickF; float a = mTickF - f;
			return LerpV3(mGems[row, col, f].mScale, mGems[row, col, f + 1].mScale, a);
		}
		public SexyVector3 GetBoardPos()
		{
			int f = (int)mTickF; float a = mTickF - f;
			return LerpV3(mBoard[f].mPos, mBoard[f + 1].mPos, a);
		}
		public SexyVector3 GetBoardRot()
		{
			int f = (int)mTickF; float a = mTickF - f;
			return EulerV3(mBoard[f].mRot, mBoard[f + 1].mRot, a);
		}
		public SexyVector3 GetBoardScale()
		{
			int f = (int)mTickF; float a = mTickF - f;
			return LerpV3(mBoard[f].mScale, mBoard[f + 1].mScale, a);
		}
		public SexyVector3 GetCameraPos()
		{
			int f = (int)mTickF; float a = mTickF - f;
			return LerpV3(mCamera[f].mPos, mCamera[f + 1].mPos, a);
		}
		public SexyVector3 GetCameraRot()
		{
			int f = (int)mTickF; float a = mTickF - f;
			return EulerV3(mCamera[f].mRot, mCamera[f + 1].mRot, a);
		}
		public SexyVector3 GetCameraScale()
		{
			int f = (int)mTickF; float a = mTickF - f;
			return LerpV3(mCamera[f].mScale, mCamera[f + 1].mScale, a);
		}

		public static HyperAnimSequence LoadFromFile(string thePath)
		{
			byte[] bytes;
			try
			{
				using (Stream s = TitleContainer.OpenStream("Content/" + thePath.Replace("\\", "/")))
				{
					if (s == null) return null;
					using (var ms = new MemoryStream())
					{
						s.CopyTo(ms);
						bytes = ms.ToArray();
					}
				}
			}
			catch
			{
				return null;
			}

			using (var br = new BinaryReader(new MemoryStream(bytes)))
			{
				int formatVer = br.ReadInt32();
				if (formatVer != 0x4) return null;

				HyperAnimSequence seq = new HyperAnimSequence();
				int frameCount = br.ReadInt32();
				int lightCount = br.ReadInt32();

				seq.mFrameCount = frameCount;
				seq.mLightCount = lightCount;
				seq.mCamera = new HyperAnimKey[frameCount];
				seq.mBoard = new HyperAnimKey[frameCount];
				seq.mGems = new HyperAnimKey[GlobalMembers.NUM_ROWS, GlobalMembers.NUM_COLS, frameCount];
				seq.mLights = new HyperAnimKey[lightCount, frameCount];

				for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
					for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
						seq.mGemHitFrame[row, col] = frameCount;

				int numHiddenObjects = br.ReadInt32();
				for (int i = 0; i < numHiddenObjects; i++)
				{
					int trackId = br.ReadInt32();
					int shouldHideAtFrameNum = br.ReadInt32();
					if (trackId < 2 || trackId > 65) continue;
					int gemCol = (trackId - 2) % 8;
					int gemRow = ((trackId - 2) - gemCol) / 8;
					seq.mGemHitFrame[gemRow, gemCol] = shouldHideAtFrameNum;
				}

				for (int frame = 0; frame < frameCount; frame++)
				{
					seq.mCamera[frame] = ReadFullKey(br);
					seq.mBoard[frame] = ReadFullKey(br);

					float aXDeltaScale = br.ReadSingle();
					float aYDeltaScale = br.ReadSingle();
					float aZDeltaScale = br.ReadSingle();
					float aXRotDeltaScale = br.ReadSingle();
					float aYRotDeltaScale = br.ReadSingle();
					float aZRotDeltaScale = br.ReadSingle();

					for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
					{
						for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
						{
							HyperAnimKey k = new HyperAnimKey();
							seq.mGems[row, col, frame] = k;
							if (frame == 0)
							{
								k.mPos.x = br.ReadSingle();
								k.mPos.y = br.ReadSingle();
								k.mPos.z = br.ReadSingle();
								k.mRot.x = br.ReadSingle();
								k.mRot.y = br.ReadSingle();
								k.mRot.z = br.ReadSingle();
								k.mScale.x = br.ReadSingle();
								k.mScale.y = br.ReadSingle();
								k.mScale.z = br.ReadSingle();
							}
							else
							{
								HyperAnimKey prev = seq.mGems[row, col, frame - 1];
								ushort flags = br.ReadUInt16();

								if ((flags & 1) != 0) k.mRot.x = prev.mRot.x + ByteToFloat(br.ReadByte(), aXRotDeltaScale);
								else if ((flags & 1024) != 0) k.mRot.x = ShortToRot(br.ReadInt16());
								else k.mRot.x = prev.mRot.x;
								if ((flags & 2) != 0) k.mRot.y = prev.mRot.y + ByteToFloat(br.ReadByte(), aYRotDeltaScale);
								else if ((flags & 2048) != 0) k.mRot.y = ShortToRot(br.ReadInt16());
								else k.mRot.y = prev.mRot.y;
								if ((flags & 4) != 0) k.mRot.z = prev.mRot.z + ByteToFloat(br.ReadByte(), aZRotDeltaScale);
								else if ((flags & 4096) != 0) k.mRot.z = ShortToRot(br.ReadInt16());
								else k.mRot.z = prev.mRot.z;
								if ((flags & 8) != 0) k.mPos.x = prev.mPos.x + ByteToFloat(br.ReadByte(), aXDeltaScale);
								else if ((flags & 16) != 0) k.mPos.x = br.ReadSingle();
								else k.mPos.x = prev.mPos.x;
								if ((flags & 32) != 0) k.mPos.y = prev.mPos.y + ByteToFloat(br.ReadByte(), aYDeltaScale);
								else if ((flags & 64) != 0) k.mPos.y = br.ReadSingle();
								else k.mPos.y = prev.mPos.y;
								if ((flags & 128) != 0) k.mPos.z = prev.mPos.z + ByteToFloat(br.ReadByte(), aZDeltaScale);
								else if ((flags & 256) != 0) k.mPos.z = br.ReadSingle();
								else k.mPos.z = prev.mPos.z;
								if ((flags & 512) != 0)
								{
									k.mScale.x = br.ReadSingle();
									k.mScale.y = br.ReadSingle();
									k.mScale.z = br.ReadSingle();
								}
								else
								{
									k.mScale.x = prev.mScale.x;
									k.mScale.y = prev.mScale.y;
									k.mScale.z = prev.mScale.z;
								}
							}
						}
					}

					for (int light = 0; light < lightCount; light++)
					{
						HyperAnimKey lk = new HyperAnimKey();
						lk.mPos.x = br.ReadSingle();
						lk.mPos.y = br.ReadSingle();
						lk.mPos.z = br.ReadSingle();
						seq.mLights[light, frame] = lk;
					}
				}

				return seq;
			}
		}
		private static HyperAnimKey ReadFullKey(BinaryReader br)
		{
			HyperAnimKey k = new HyperAnimKey();
			k.mPos.x = br.ReadSingle();
			k.mPos.y = br.ReadSingle();
			k.mPos.z = br.ReadSingle();
			k.mRot.x = br.ReadSingle();
			k.mRot.y = br.ReadSingle();
			k.mRot.z = br.ReadSingle();
			k.mScale.x = br.ReadSingle();
			k.mScale.y = br.ReadSingle();
			k.mScale.z = br.ReadSingle();
			return k;
		}
		private static float ByteToFloat(byte b, float maxScale)
		{
			sbyte sb = (sbyte)b;
			return sb * maxScale / 127f;
		}
		private static float ShortToRot(short s)
		{
			return s * 3.14159f / 0x7fff;
		}
	}
}
