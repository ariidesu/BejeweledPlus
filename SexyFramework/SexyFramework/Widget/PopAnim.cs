using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace SexyFramework.Widget
{
	public class PopAnim : Widget
	{
		public static ulong PAM_MAGIC = 3136297300uL;

		public static int PAM_VERSION = 5;

		public static int PAM_STATE_VERSION = 1;

		public static int FRAMEFLAGS_HAS_REMOVES = 1;

		public static int FRAMEFLAGS_HAS_ADDS = 2;

		public static int FRAMEFLAGS_HAS_MOVES = 4;

		public static int FRAMEFLAGS_HAS_FRAME_NAME = 8;

		public static int FRAMEFLAGS_HAS_STOP = 16;

		public static int FRAMEFLAGS_HAS_COMMANDS = 32;

		public static int MOVEFLAGS_HAS_SRCRECT = 32768;

		public static int MOVEFLAGS_HAS_ROTATE = 16384;

		public static int MOVEFLAGS_HAS_COLOR = 8192;

		public static int MOVEFLAGS_HAS_MATRIX = 4096;

		public static int MOVEFLAGS_HAS_LONGCOORDS = 2048;

		public static int MOVEFLAGS_HAS_ANIMFRAMENUM = 1024;

		public int mId;

		public PopAnimListener mListener;

		public List<string> mImageSearchPathVector = new List<string>();

		public int mVersion;

		public SexyFramework.Misc.Buffer mCRCBuffer = new SexyFramework.Misc.Buffer();

		public float mDrawScale;

		public float mImgScale;

		public bool mLoaded;

		public int mMotionFilePos;

		public string mModPamFile;

		public string mLoadedPamFile;

		public LinkedList<KeyValuePair<string, string>> mRemapList = new LinkedList<KeyValuePair<string, string>>();

		public int mAnimRate;

		public Rect mAnimRect = default(Rect);

		public string mError;

		public string mLastPlayedFrameLabel;

		public List<PAImage> mImageVector = new List<PAImage>();

		public PASpriteInst mMainSpriteInst;

		public PopAnimDef mMainAnimDef;

		public float mBlendTicksTotal;

		public float mBlendTicksCur;

		public float mBlendDelay;

		public MTRand mRand = new MTRand();

		public bool mRandUsed;

		public FPoint mParticleAttachOffset = default(FPoint);

		public SexyTransform2D mTransform = new SexyTransform2D(false);

		public Color mColor = default(Color);

		public bool mAdditive;

		public bool mTransDirty;

		public bool mAnimRunning;

		public bool mMirror;

		public bool mInterpolate;

		public bool mLoadedImageIsNew;

		public bool mPaused;

		public bool mColorizeType;

		private string Remap_aString;

		private PAObjectPos[] COP_aNextObjectPos = new PAObjectPos[3];

		private int[] COP_anOfsTab = new int[3] { 0, 1, 2 };

		public static int ClrR(int theColor)
		{
			return (theColor >> 16) & 0xFF;
		}

		public static int ClrG(int theColor)
		{
			return (theColor >> 8) & 0xFF;
		}

		public static int ClrB(int theColor)
		{
			return theColor & 0xFF;
		}

		public static int ClrA(int theColor)
		{
			return (theColor >> 24) & 0xFF;
		}

		internal static string WildcardExpand(string theValue, int theMatchStart, int theMatchEnd, string theReplacement)
		{
			string text;
			if (theReplacement.Length == 0)
			{
				return text = "";
			}
			if (theReplacement[0] == '*')
			{
				if (theReplacement.Length == 1)
				{
					return theValue.Substring(0, theMatchStart) + theValue.Substring(theMatchEnd);
				}
				if (theReplacement[theReplacement.Length - 1] == '*')
				{
					return theValue.Substring(0, theMatchStart) + theReplacement.Substring(1, theReplacement.Length - 2) + theValue.Substring(theMatchEnd);
				}
				return theValue.Substring(0, theMatchStart) + theReplacement.Substring(1, theReplacement.Length - 1);
			}
			if (theReplacement[theReplacement.Length - 1] == '*')
			{
				return theReplacement.Substring(0, theReplacement.Length - 1) + theValue.Substring(theMatchEnd);
			}
			return theReplacement;
		}

		internal static bool WildcardReplace(string theValue, string theWildcard, string theReplacement, ref string theResult)
		{
			if (theWildcard.Length == 0)
			{
				return false;
			}
			if (theWildcard[0] == '*')
			{
				if (theWildcard.Length == 1)
				{
					theResult = WildcardExpand(theValue, 0, theValue.Length, theReplacement);
					return true;
				}
				if (theWildcard[theWildcard.Length - 1] != '*')
				{
					if (theValue.Length < theWildcard.Length - 1)
					{
						return false;
					}
					if (theWildcard.Substring(1) == theValue.Substring(theValue.Length - theWildcard.Length + 1))
					{
						return false;
					}
					theResult = WildcardExpand(theValue, theValue.Length - theWildcard.Length + 1, theValue.Length, theReplacement);
					return true;
				}
				int num = theWildcard.Length - 2;
				int num2 = theValue.Length - num;
				for (int i = 0; i <= num2; i++)
				{
					bool flag = true;
					for (int j = 0; j < num; j++)
					{
						if (char.ToUpper(theWildcard[j + 1]) != char.ToUpper(theValue[i + j]))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						theResult = WildcardExpand(theValue, i, i + num, theReplacement);
						return true;
					}
				}
			}
			else
			{
				if (theWildcard[theWildcard.Length - 1] == '*')
				{
					if (theValue.Length < theWildcard.Length - 1)
					{
						return false;
					}
					if (theWildcard == theValue.Substring(0, theWildcard.Length - 1))
					{
						return false;
					}
					theResult = WildcardExpand(theValue, 0, theWildcard.Length - 1, theReplacement);
					return true;
				}
				if (theWildcard == theValue)
				{
					if (theReplacement.Length > 0)
					{
						if (theReplacement[0] == '*')
						{
							theResult = theValue + theReplacement.Substring(1);
						}
						else if (theReplacement[theReplacement.Length - 1] == '*')
						{
							theResult = theReplacement.Substring(0, theReplacement.Length - 1) + theValue;
						}
						else
						{
							theResult = theReplacement;
						}
					}
					else
					{
						theResult = theReplacement;
					}
					return true;
				}
			}
			return false;
		}

		internal static bool WildcardMatches(string theValue, string theWildcard)
		{
			if (theWildcard.Length == 0)
			{
				return false;
			}
			if (theWildcard[0] == '*')
			{
				if (theWildcard.Length == 1)
				{
					return true;
				}
				if (theWildcard[theWildcard.Length - 1] == '*')
				{
					int num = theWildcard.Length - 2;
					int num2 = theValue.Length - num;
					for (int i = 0; i <= num2; i++)
					{
						bool flag = true;
						for (int j = 0; j < num; j++)
						{
							if (char.ToUpper(theWildcard[j + 1]) != char.ToUpper(theValue[i + j]))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							return true;
						}
					}
					return false;
				}
				if (theValue.Length < theWildcard.Length - 1)
				{
					return false;
				}
				return theWildcard.Substring(1) == theValue.Substring(theValue.Length - theWildcard.Length + 1);
			}
			if (theWildcard[theWildcard.Length - 1] == '*')
			{
				if (theValue.Length < theWildcard.Length - 1)
				{
					return false;
				}
				return theWildcard == theValue.Substring(theWildcard.Length - 1);
			}
			if (theWildcard == theValue)
			{
				return true;
			}
			return false;
		}

		public bool Fail(string theError)
		{
			mError = theError;
			return false;
		}

		public void SetTransform(SexyTransform2D tran)
		{
			mTransform.CopyFrom(tran);
			mTransDirty = true;
		}

		public string Remap(string theString)
		{
			LinkedList<KeyValuePair<string, string>>.Enumerator enumerator = mRemapList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (WildcardReplace(theString, enumerator.Current.Value, enumerator.Current.Key, ref Remap_aString))
				{
					return Remap_aString;
				}
			}
			return theString;
		}

		public bool LoadSpriteDef(SexyFramework.Misc.Buffer theBuffer, PASpriteDef theSpriteDef)
		{
			Dictionary<int, PAObjectPos> dictionary = new Dictionary<int, PAObjectPos>();
			if (mVersion >= 4)
			{
				mMainAnimDef.mObjectNamePool.AddLast(theBuffer.ReadString());
				theSpriteDef.mName = mMainAnimDef.mObjectNamePool.Last.Value;
				theSpriteDef.mAnimRate = (float)theBuffer.ReadLong() / 65536f;
				mCRCBuffer.WriteString(theSpriteDef.mName);
			}
			else
			{
				theSpriteDef.mName = null;
				theSpriteDef.mAnimRate = mAnimRate;
			}
			int num = theBuffer.ReadShort();
			if (mVersion >= 5)
			{
				theSpriteDef.mWorkAreaStart = theBuffer.ReadShort();
				theSpriteDef.mWorkAreaDuration = theBuffer.ReadShort();
			}
			else
			{
				theSpriteDef.mWorkAreaStart = 0;
				theSpriteDef.mWorkAreaDuration = num - 1;
			}
			theSpriteDef.mWorkAreaDuration = Math.Min(theSpriteDef.mWorkAreaStart + theSpriteDef.mWorkAreaDuration, num - 1) - theSpriteDef.mWorkAreaStart;
			mCRCBuffer.WriteShort((short)num);
			theSpriteDef.mFrames = new PAFrame[num];
			for (int i = 0; i < num; i++)
			{
				PAFrame pAFrame = new PAFrame();
				theSpriteDef.mFrames[i] = pAFrame;
				byte b = theBuffer.ReadByte();
				if ((b & FRAMEFLAGS_HAS_REMOVES) != 0)
				{
					int num2 = theBuffer.ReadByte();
					if (num2 == 255)
					{
						num2 = theBuffer.ReadShort();
					}
					for (int j = 0; j < num2; j++)
					{
						int num3 = theBuffer.ReadShort();
						if (num3 >= 2047)
						{
							num3 = (int)theBuffer.ReadLong();
						}
						dictionary.Remove(num3);
					}
				}
				if ((b & FRAMEFLAGS_HAS_ADDS) != 0)
				{
					int num4 = theBuffer.ReadByte();
					if (num4 == 255)
					{
						num4 = theBuffer.ReadShort();
					}
					for (int k = 0; k < num4; k++)
					{
						PAObjectPos pAObjectPos = new PAObjectPos();
						ushort num5 = (ushort)theBuffer.ReadShort();
						pAObjectPos.mObjectNum = num5 & 0x7FF;
						if (pAObjectPos.mObjectNum == 2047)
						{
							pAObjectPos.mObjectNum = (int)theBuffer.ReadLong();
						}
						pAObjectPos.mIsSprite = (num5 & 0x8000) != 0;
						pAObjectPos.mIsAdditive = (num5 & 0x4000) != 0;
						pAObjectPos.mResNum = theBuffer.ReadByte();
						pAObjectPos.mHasSrcRect = false;
						pAObjectPos.mColorInt = -1;
						pAObjectPos.mAnimFrameNum = 0;
						pAObjectPos.mTimeScale = 1f;
						pAObjectPos.mName = null;
						if ((num5 & 0x2000) != 0)
						{
							pAObjectPos.mPreloadFrames = theBuffer.ReadShort();
						}
						else
						{
							pAObjectPos.mPreloadFrames = 0;
						}
						if ((num5 & 0x1000) != 0)
						{
							mMainAnimDef.mObjectNamePool.AddLast(theBuffer.ReadString());
							pAObjectPos.mName = mMainAnimDef.mObjectNamePool.Last.Value;
						}
						if ((num5 & 0x800) != 0)
						{
							pAObjectPos.mTimeScale = (float)theBuffer.ReadLong() / 65536f;
						}
						if (theSpriteDef.mObjectDefVector.Count < pAObjectPos.mObjectNum + 1)
						{
							Common.Resize(theSpriteDef.mObjectDefVector, pAObjectPos.mObjectNum + 1);
						}
						theSpriteDef.mObjectDefVector[pAObjectPos.mObjectNum].mName = pAObjectPos.mName;
						if (pAObjectPos.mIsSprite)
						{
							theSpriteDef.mObjectDefVector[pAObjectPos.mObjectNum].mSpriteDef = mMainAnimDef.mSpriteDefVector[pAObjectPos.mResNum];
						}
						dictionary[pAObjectPos.mObjectNum] = pAObjectPos;
					}
				}
				if ((b & FRAMEFLAGS_HAS_MOVES) != 0)
				{
					int num6 = theBuffer.ReadByte();
					if (num6 == 255)
					{
						num6 = theBuffer.ReadShort();
					}
					for (int l = 0; l < num6; l++)
					{
						ushort num7 = (ushort)theBuffer.ReadShort();
						int num8 = num7 & 0x3FF;
						if (num8 == 1023)
						{
							num8 = (int)theBuffer.ReadLong();
						}
						dictionary[num8] = new PAObjectPos(dictionary[num8]);
						PAObjectPos pAObjectPos2 = dictionary[num8];
						pAObjectPos2.mTransform.mMatrix.LoadIdentity();
						if ((num7 & MOVEFLAGS_HAS_MATRIX) != 0)
						{
							pAObjectPos2.mTransform.mMatrix.m00 = (float)theBuffer.ReadLong() / 65536f;
							pAObjectPos2.mTransform.mMatrix.m01 = (float)theBuffer.ReadLong() / 65536f;
							pAObjectPos2.mTransform.mMatrix.m10 = (float)theBuffer.ReadLong() / 65536f;
							pAObjectPos2.mTransform.mMatrix.m11 = (float)theBuffer.ReadLong() / 65536f;
						}
						else if ((num7 & MOVEFLAGS_HAS_ROTATE) != 0)
						{
							float num9 = (float)theBuffer.ReadShort() / 1000f;
							float num10 = (float)Math.Sin(num9);
							float num11 = (float)Math.Cos(num9);
							if (mVersion == 2)
							{
								num10 = 0f - num10;
							}
							pAObjectPos2.mTransform.mMatrix.m00 = num11;
							pAObjectPos2.mTransform.mMatrix.m01 = 0f - num10;
							pAObjectPos2.mTransform.mMatrix.m10 = num10;
							pAObjectPos2.mTransform.mMatrix.m11 = num11;
						}
						SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
						sexyTransform2D.LoadIdentity();
						if ((num7 & MOVEFLAGS_HAS_LONGCOORDS) != 0)
						{
							sexyTransform2D.m02 = (float)theBuffer.ReadLong() / 20f;
							sexyTransform2D.m12 = (float)theBuffer.ReadLong() / 20f;
						}
						else
						{
							sexyTransform2D.m02 = (float)theBuffer.ReadShort() / 20f;
							sexyTransform2D.m12 = (float)theBuffer.ReadShort() / 20f;
						}
						pAObjectPos2.mTransform.mMatrix = sexyTransform2D * pAObjectPos2.mTransform.mMatrix;
						pAObjectPos2.mHasSrcRect = (num7 & MOVEFLAGS_HAS_SRCRECT) != 0;
						if ((num7 & MOVEFLAGS_HAS_SRCRECT) != 0)
						{
							pAObjectPos2.mSrcRect.mX = theBuffer.ReadShort() / 20;
							pAObjectPos2.mSrcRect.mY = theBuffer.ReadShort() / 20;
							pAObjectPos2.mSrcRect.mWidth = theBuffer.ReadShort() / 20;
							pAObjectPos2.mSrcRect.mHeight = theBuffer.ReadShort() / 20;
						}
						if ((num7 & MOVEFLAGS_HAS_COLOR) != 0)
						{
							pAObjectPos2.mColorInt = (theBuffer.ReadByte() << 16) | (theBuffer.ReadByte() << 8) | theBuffer.ReadByte() | (theBuffer.ReadByte() << 24);
						}
						if ((num7 & MOVEFLAGS_HAS_ANIMFRAMENUM) != 0)
						{
							pAObjectPos2.mAnimFrameNum = theBuffer.ReadShort();
						}
					}
				}
				if ((b & FRAMEFLAGS_HAS_FRAME_NAME) != 0)
				{
					string theString = theBuffer.ReadString();
					theString = Remap(theString).ToUpper();
					theSpriteDef.mLabels.Add(theString, i);
				}
				if ((b & FRAMEFLAGS_HAS_STOP) != 0)
				{
					pAFrame.mHasStop = true;
				}
				if ((b & FRAMEFLAGS_HAS_COMMANDS) != 0)
				{
					int num12 = theBuffer.ReadByte();
					pAFrame.mCommandVector = new PACommand[num12];
					for (int m = 0; m < num12; m++)
					{
						pAFrame.mCommandVector[m] = new PACommand();
						pAFrame.mCommandVector[m].mCommand = Remap(theBuffer.ReadString());
						pAFrame.mCommandVector[m].mParam = Remap(theBuffer.ReadString());
					}
				}
				pAFrame.mFrameObjectPosVector = new PAObjectPos[dictionary.Count];
				for (int n = 0; n < pAFrame.mFrameObjectPosVector.Length; n++)
				{
					pAFrame.mFrameObjectPosVector[n] = new PAObjectPos();
				}
				int num13 = 0;
				int[] array = dictionary.Keys.ToArray();
				Array.Sort(array);
				for (int num14 = 0; num14 < array.Length; num14++)
				{
					PAObjectPos pAObjectPos3 = dictionary[array[num14]];
					pAFrame.mFrameObjectPosVector[num13] = pAObjectPos3;
					pAObjectPos3.mPreloadFrames = 0;
					num13++;
				}
			}
			if (num == 0)
			{
				theSpriteDef.mFrames = new PAFrame[1];
				theSpriteDef.mFrames[0] = new PAFrame();
			}
			for (int num15 = 0; num15 < theSpriteDef.mObjectDefVector.Count; num15++)
			{
				PAObjectDef pAObjectDef = theSpriteDef.mObjectDefVector[num15];
				mCRCBuffer.WriteBoolean(pAObjectDef.mSpriteDef != null);
			}
			return true;
		}

		public void InitSpriteInst(PASpriteInst theSpriteInst, PASpriteDef theSpriteDef)
		{
			theSpriteInst.mFrameRepeats = 0;
			theSpriteInst.mDelayFrames = 0;
			theSpriteInst.mDef = theSpriteDef;
			theSpriteInst.mLastUpdated = -1;
			theSpriteInst.mOnNewFrame = true;
			theSpriteInst.mFrameNum = 0f;
			Common.Resize(theSpriteInst.mChildren, theSpriteDef.mObjectDefVector.Count);
			for (int i = 0; i < theSpriteDef.mObjectDefVector.Count; i++)
			{
				PAObjectDef pAObjectDef = theSpriteDef.mObjectDefVector[i];
				PAObjectInst pAObjectInst = theSpriteInst.mChildren[i];
				pAObjectInst.mColorMult = new Color(Color.White);
				pAObjectInst.mName = pAObjectDef.mName;
				pAObjectInst.mIsBlending = false;
				PASpriteDef mSpriteDef = pAObjectDef.mSpriteDef;
				if (mSpriteDef != null)
				{
					PASpriteInst pASpriteInst = new PASpriteInst();
					pASpriteInst.mParent = theSpriteInst;
					InitSpriteInst(pASpriteInst, mSpriteDef);
					pAObjectInst.mSpriteInst = pASpriteInst;
				}
			}
			if (theSpriteInst == mMainSpriteInst)
			{
				GetToFirstFrame();
			}
		}

		public void GetToFirstFrame()
		{
			while (mMainSpriteInst.mDef != null && mMainSpriteInst.mFrameNum < (float)mMainSpriteInst.mDef.mWorkAreaStart)
			{
				bool flag = mAnimRunning;
				bool flag2 = mPaused;
				mAnimRunning = true;
				mPaused = false;
				Update();
				if (GlobalMembers.gSexyAppBase.mVSyncUpdates)
				{
					UpdateF(1f);
				}
				mAnimRunning = flag;
				mPaused = flag2;
			}
		}

		public void FrameHit(PASpriteInst theSpriteInst, PAFrame theFrame, PAObjectPos theObjectPos)
		{
			theSpriteInst.mOnNewFrame = false;
			for (int i = 0; i < theFrame.mFrameObjectPosVector.Length; i++)
			{
				PAObjectPos pAObjectPos = theFrame.mFrameObjectPosVector[i];
				if (!pAObjectPos.mIsSprite)
				{
					continue;
				}
				PASpriteInst mSpriteInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
				if (mSpriteInst != null && theSpriteInst.mDef.mAnimRate != 0f)
				{
					for (int j = 0; j < pAObjectPos.mPreloadFrames; j++)
					{
						IncSpriteInstFrame(mSpriteInst, pAObjectPos, 1000f / GlobalMembers.gSexyAppBase.mFrameTime / theSpriteInst.mDef.mAnimRate);
					}
				}
			}
			for (int k = 0; k < theFrame.mCommandVector.Length; k++)
			{
				PACommand pACommand = theFrame.mCommandVector[k];
				if (mListener != null && mListener.PopAnimCommand(mId, theSpriteInst, pACommand.mCommand, pACommand.mParam))
				{
					continue;
				}
				if (string.Compare(pACommand.mCommand, "delay") == 0)
				{
					int num = pACommand.mParam.IndexOf(',');
					if (num != -1)
					{
						int num2 = Convert.ToInt32(pACommand.mParam.Substring(0, num).Trim());
						int num3 = Convert.ToInt32(pACommand.mParam.Substring(num + 1).Trim());
						if (num3 <= num2)
						{
							num3 = num2 + 1;
						}
						Random random = new Random(100);
						theSpriteInst.mDelayFrames = num2 + random.Next(num3 - num2);
					}
					else
					{
						int mDelayFrames = Convert.ToInt32(pACommand.mParam.Trim());
						theSpriteInst.mDelayFrames = mDelayFrames;
					}
				}
				else if (string.Compare(pACommand.mCommand, "playsample", StringComparison.OrdinalIgnoreCase) == 0)
				{
					string text = pACommand.mParam;
					int thePan = 0;
					double theVolume = 1.0;
					double theNumSteps = 0.0;
					string theSampleName = "";
					bool flag = true;
					while (text.Length > 0)
					{
						int num4 = text.IndexOf(',');
						string text2 = ((num4 != -1) ? text.Substring(0, num4) : text);
						if (flag)
						{
							theSampleName = text2;
							flag = false;
						}
						else
						{
							int startIndex;
							while ((startIndex = text2.IndexOf(' ')) != -1)
							{
								text2 = text2.Remove(startIndex);
							}
							if (text2.Substring(0, 7) == "volume=")
							{
								theVolume = double.Parse(text2.Substring(7), NumberStyles.Float, CultureInfo.InvariantCulture);
							}
							else if (text2.Substring(0, 4) == "pan=")
							{
								thePan = int.Parse(text2.Substring(4));
							}
							else if (text2.Substring(0, 6) == "steps=")
							{
								theNumSteps = double.Parse(text2.Substring(6), NumberStyles.Float, CultureInfo.InvariantCulture);
							}
						}
						if (num4 == -1)
						{
							break;
						}
						text = text.Substring(num4 + 1);
					}
					if (mListener != null)
					{
						mListener.PopAnimPlaySample(theSampleName, thePan, theVolume, theNumSteps);
					}
				}
				else
				{
					if (string.Compare(pACommand.mCommand, "addparticleeffect", StringComparison.OrdinalIgnoreCase) != 0)
					{
						continue;
					}
					string text3 = pACommand.mParam;
					PAParticleEffect pAParticleEffect = new PAParticleEffect();
					pAParticleEffect.mXOfs = 0.0;
					pAParticleEffect.mYOfs = 0.0;
					pAParticleEffect.mBehind = false;
					pAParticleEffect.mEffect = null;
					pAParticleEffect.mAttachEmitter = false;
					pAParticleEffect.mTransform = false;
					pAParticleEffect.mLastUpdated = mUpdateCnt;
					bool flag2 = false;
					string text4 = "";
					bool flag3 = true;
					while (text3.Length > 0)
					{
						int num5 = text3.IndexOf(',');
						string text5 = ((num5 != -1) ? text3.Substring(0, num5) : text3);
						text5 = text5.Trim();
						if (flag3)
						{
							pAParticleEffect.mName = text5;
							text4 = text5;
							flag3 = false;
						}
						else
						{
							int startIndex2;
							while ((startIndex2 = text5.IndexOf(' ')) != -1)
							{
								text5 = text5.Remove(startIndex2);
							}
							if (text5.StartsWith("x="))
							{
								pAParticleEffect.mXOfs = double.Parse(text5.Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture);
							}
							else if (text5.StartsWith("y="))
							{
								pAParticleEffect.mYOfs = double.Parse(text5.Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture);
							}
							else if (string.Compare(text5, "attachemitter", StringComparison.OrdinalIgnoreCase) == 0)
							{
								pAParticleEffect.mAttachEmitter = true;
							}
							else if (string.Compare(text5, "once", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = true;
							}
							else if (string.Compare(text5, "behind", StringComparison.OrdinalIgnoreCase) == 0)
							{
								pAParticleEffect.mBehind = true;
							}
							else if (string.Compare(text5, "transform", StringComparison.OrdinalIgnoreCase) == 0)
							{
								pAParticleEffect.mTransform = true;
							}
						}
						if (num5 == -1)
						{
							break;
						}
						text3 = text3.Substring(num5 + 1);
					}
					if (flag2)
					{
						for (int l = 0; l < theSpriteInst.mParticleEffectVector.Count; l++)
						{
							PAParticleEffect pAParticleEffect2 = theSpriteInst.mParticleEffectVector[l];
							if (pAParticleEffect2.mName == text4)
							{
								return;
							}
						}
					}
					string pathFrom = Common.GetPathFrom("..\\" + text4 + "\\" + text4, Common.GetFileDir(mLoadedPamFile, false));
					string pathFrom2 = Common.GetPathFrom(text4 + "\\" + text4, Common.GetFileDir(mLoadedPamFile, false));
					pathFrom2 = "images\\960\\help\\" + text4 + "\\" + text4;
					if (mListener != null)
					{
						pAParticleEffect.mEffect = mListener.PopAnimLoadParticleEffect(text4);
					}
					if (pAParticleEffect.mEffect == null)
					{
						ResourceRef resourceRef = GlobalMembers.gSexyAppBase.mResourceManager.GetResourceRefFromPath(pathFrom2 + ".ppf");
						if (resourceRef == null)
						{
							resourceRef = GlobalMembers.gSexyAppBase.mResourceManager.GetResourceRefFromPath(pathFrom + ".ppf");
						}
						if (resourceRef == null)
						{
							resourceRef = GlobalMembers.gSexyAppBase.mResourceManager.GetResourceRef(4, "PIEFFECT_" + text4.ToUpper());
						}
						if (resourceRef != null)
						{
							pAParticleEffect.mEffect = resourceRef.GetPIEffect().Duplicate();
							pAParticleEffect.mResourceRef = resourceRef;
						}
					}
					if (pAParticleEffect.mEffect == null)
					{
						pAParticleEffect.mEffect = new PIEffect();
						if (!pAParticleEffect.mEffect.LoadEffect(pathFrom2 + ".ppf") && !pAParticleEffect.mEffect.LoadEffect(pathFrom + ".ppf") && !pAParticleEffect.mEffect.LoadEffect(pathFrom2 + ".ip3"))
						{
							bool flag4 = false;
							for (int m = 0; m < mImageSearchPathVector.Count; m++)
							{
								flag4 |= pAParticleEffect.mEffect.LoadEffect(mImageSearchPathVector[m] + text4 + ".ip3");
								if (flag4)
								{
									break;
								}
							}
							if (!flag4)
							{
								if (pAParticleEffect.mEffect != null)
								{
									pAParticleEffect.mEffect.Dispose();
								}
								pAParticleEffect.mEffect = null;
							}
						}
					}
					if (pAParticleEffect.mEffect == null)
					{
						continue;
					}
					if (!mRandUsed)
					{
						mRandUsed = true;
						mRand.SRand(mRand.Next());
					}
					if (pAParticleEffect.mEffect.mRandSeeds.Count > 0)
					{
						int seed = pAParticleEffect.mEffect.mRandSeeds[(int)(mRand.Next() % pAParticleEffect.mEffect.mRandSeeds.Count)];
						pAParticleEffect.mEffect.mRand.SRand((uint)seed);
					}
					else
					{
						pAParticleEffect.mEffect.mRand.SRand(mRand.Next());
					}
					pAParticleEffect.mEffect.mWantsSRand = false;
					if (theObjectPos != null && theSpriteInst.mDef.mAnimRate != 0f)
					{
						int num6 = (int)(100.0 * (double)((float)theObjectPos.mPreloadFrames / theObjectPos.mTimeScale / theSpriteInst.mDef.mAnimRate) + 0.5);
						for (int n = 0; n < num6; n++)
						{
							pAParticleEffect.mEffect.Update();
						}
					}
					theSpriteInst.mParticleEffectVector.Add(pAParticleEffect);
				}
			}
		}

		public void DoFramesHit(PASpriteInst theSpriteInst, PAObjectPos theObjectPos)
		{
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[(int)theSpriteInst.mFrameNum];
			FrameHit(theSpriteInst, pAFrame, theObjectPos);
			for (int i = 0; i < pAFrame.mFrameObjectPosVector.Length; i++)
			{
				PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[i];
				if (pAObjectPos.mIsSprite)
				{
					PASpriteInst mSpriteInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
					if (mSpriteInst != null)
					{
						DoFramesHit(mSpriteInst, pAObjectPos);
					}
				}
			}
		}

		public void CalcObjectPos(PASpriteInst theSpriteInst, int theObjectPosIdx, bool frozen, ref PATransform theTransform, out Color theColor)
		{
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[(int)theSpriteInst.mFrameNum];
			PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[theObjectPosIdx];
			PAObjectInst pAObjectInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum];
			for (int i = 0; i < 3; i++)
			{
				COP_aNextObjectPos[i] = null;
			}
			COP_anOfsTab[0] = theSpriteInst.mDef.mFrames.Length - 1;
			COP_anOfsTab[1] = 1;
			COP_anOfsTab[2] = 2;
			if (theSpriteInst == mMainSpriteInst && theSpriteInst.mFrameNum >= (float)theSpriteInst.mDef.mWorkAreaStart)
			{
				COP_anOfsTab[0] = theSpriteInst.mDef.mWorkAreaDuration - 1;
			}
			PATransform outTran = new PATransform(true);
			Color color;
			if (mInterpolate && !frozen)
			{
				for (int j = 0; j < 3; j++)
				{
					PAFrame pAFrame2 = theSpriteInst.mDef.mFrames[((int)theSpriteInst.mFrameNum + COP_anOfsTab[j]) % theSpriteInst.mDef.mFrames.Length];
					pAFrame2 = ((theSpriteInst != mMainSpriteInst || !(theSpriteInst.mFrameNum >= (float)theSpriteInst.mDef.mWorkAreaStart)) ? theSpriteInst.mDef.mFrames[((int)theSpriteInst.mFrameNum + COP_anOfsTab[j]) % theSpriteInst.mDef.mFrames.Length] : theSpriteInst.mDef.mFrames[((int)theSpriteInst.mFrameNum + COP_anOfsTab[j] - theSpriteInst.mDef.mWorkAreaStart) % (theSpriteInst.mDef.mWorkAreaDuration + 1) + theSpriteInst.mDef.mWorkAreaStart]);
					if (pAFrame.mHasStop)
					{
						pAFrame2 = pAFrame;
					}
					if (pAFrame2.mFrameObjectPosVector.Length > theObjectPosIdx)
					{
						COP_aNextObjectPos[j] = pAFrame2.mFrameObjectPosVector[theObjectPosIdx];
						if (COP_aNextObjectPos[j].mObjectNum != pAObjectPos.mObjectNum)
						{
							COP_aNextObjectPos[j] = null;
						}
					}
					if (COP_aNextObjectPos[j] != null)
					{
						continue;
					}
					for (int k = 0; k < pAFrame2.mFrameObjectPosVector.Length; k++)
					{
						if (pAFrame2.mFrameObjectPosVector[k].mObjectNum == pAObjectPos.mObjectNum)
						{
							COP_aNextObjectPos[j] = pAFrame2.mFrameObjectPosVector[k];
							break;
						}
					}
				}
				if (COP_aNextObjectPos[1] != null)
				{
					float num = theSpriteInst.mFrameNum - (float)(int)theSpriteInst.mFrameNum;
					bool flag = false;
					SexyVector2 sexyVector = pAObjectPos.mTransform.mMatrix * new SexyVector2(0f, 0f);
					SexyVector2 sexyVector2 = COP_aNextObjectPos[1].mTransform.mMatrix * new SexyVector2(0f, 0f);
					if (COP_aNextObjectPos[0] != null && COP_aNextObjectPos[2] != null)
					{
						SexyVector2 sexyVector3 = COP_aNextObjectPos[0].mTransform.mMatrix * new SexyVector2(0f, 0f);
						SexyVector2 sexyVector4 = COP_aNextObjectPos[2].mTransform.mMatrix * new SexyVector2(0f, 0f);
						SexyVector2 sexyVector5 = sexyVector - sexyVector3;
						SexyVector2 sexyVector6 = sexyVector2 - sexyVector;
						SexyVector2 sexyVector7 = sexyVector4 - sexyVector2;
						SexyVector2 sexyVector9 = sexyVector6 - sexyVector5;
						SexyVector2 sexyVector8 = sexyVector7 - sexyVector6;
						float num2 = Math.Max(sexyVector5.Magnitude(), sexyVector7.Magnitude()) * 0.5f + sexyVector5.Magnitude() * 0.25f + sexyVector7.Magnitude() * 0.25f;
						if (sexyVector8.Magnitude() > num2 * 4f)
						{
							flag = true;
						}
					}
					if (flag)
					{
						num = ((num < 0.5f) ? 0f : 1f);
					}
					pAObjectPos.mTransform.InterpolateTo(COP_aNextObjectPos[1].mTransform, num, ref outTran);
					color = new Color((int)((float)ClrR(pAObjectPos.mColorInt) * (1f - num) + (float)ClrR(COP_aNextObjectPos[1].mColorInt) * num + 0.5f), (int)((float)ClrG(pAObjectPos.mColorInt) * (1f - num) + (float)ClrG(COP_aNextObjectPos[1].mColorInt) * num + 0.5f), (int)((float)ClrB(pAObjectPos.mColorInt) * (1f - num) + (float)ClrB(COP_aNextObjectPos[1].mColorInt) * num + 0.5f), (int)((float)ClrA(pAObjectPos.mColorInt) * (1f - num) + (float)ClrA(COP_aNextObjectPos[1].mColorInt) * num + 0.5f));
				}
				else
				{
					outTran.CopyFrom(pAObjectPos.mTransform);
					color = new Color(ClrR(pAObjectPos.mColorInt), ClrG(pAObjectPos.mColorInt), ClrB(pAObjectPos.mColorInt), ClrA(pAObjectPos.mColorInt));
				}
			}
			else
			{
				outTran.CopyFrom(pAObjectPos.mTransform);
				color = new Color(ClrR(pAObjectPos.mColorInt), ClrG(pAObjectPos.mColorInt), ClrB(pAObjectPos.mColorInt), ClrA(pAObjectPos.mColorInt));
			}
			outTran.mMatrix.CopyFrom(pAObjectInst.mTransform * outTran.mMatrix);
			if (pAObjectInst.mIsBlending && mBlendTicksTotal != 0f && theSpriteInst == mMainSpriteInst)
			{
				float num3 = mBlendTicksCur / mBlendTicksTotal;
				pAObjectInst.mBlendSrcTransform.InterpolateTo(outTran, num3, ref outTran);
				color = new Color((int)((float)pAObjectInst.mBlendSrcColor.mRed * (1f - num3) + (float)color.mRed * num3 + 0.5f), (int)((float)pAObjectInst.mBlendSrcColor.mGreen * (1f - num3) + (float)color.mGreen * num3 + 0.5f), (int)((float)pAObjectInst.mBlendSrcColor.mBlue * (1f - num3) + (float)color.mBlue * num3 + 0.5f), (int)((float)pAObjectInst.mBlendSrcColor.mAlpha * (1f - num3) + (float)color.mAlpha * num3 + 0.5f));
			}
			theTransform = outTran;
			theColor = color;
		}

		public void UpdateTransforms(PASpriteInst theSpriteInst, PATransform? theTransform, Color theColor, bool parentFrozen)
		{
			if (theTransform.HasValue)
			{
				theSpriteInst.mCurTransform.CopyFrom(theTransform.Value);
			}
			else
			{
				theSpriteInst.mCurTransform.mMatrix.CopyFrom(mTransform);
			}
			theSpriteInst.mCurColor = theColor;
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[(int)theSpriteInst.mFrameNum];
			PATransform theTransform2 = new PATransform(true);
			bool flag = parentFrozen || theSpriteInst.mDelayFrames > 0 || pAFrame.mHasStop;
			int num = 0;
			PAObjectPos[] mFrameObjectPosVector = pAFrame.mFrameObjectPosVector;
			foreach (PAObjectPos pAObjectPos in mFrameObjectPosVector)
			{
				if (pAObjectPos.mIsSprite)
				{
					Color theColor2;
					CalcObjectPos(theSpriteInst, num, flag, ref theTransform2, out theColor2);
					if (theTransform.HasValue)
					{
						theTransform.Value.TransformSrc(theTransform2, ref theTransform2);
					}
					UpdateTransforms(theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst, theTransform2, theColor2, flag);
				}
				num++;
			}
			foreach (PAParticleEffect item in theSpriteInst.mParticleEffectVector)
			{
				if (item.mAttachEmitter)
				{
					if (item.mTransform)
					{
						SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
						sexyTransform2D.Translate((float)item.mEffect.mWidth / 2f, (float)item.mEffect.mHeight / 2f);
						SexyTransform2D theMatrix = new SexyTransform2D(false);
						theMatrix.CopyFrom(theSpriteInst.mCurTransform.mMatrix * sexyTransform2D);
						item.mEffect.mEmitterTransform.CopyFrom(theMatrix);
					}
					else
					{
						SexyVector2 sexyVector = theSpriteInst.mCurTransform.mMatrix * new SexyVector2((float)item.mXOfs, (float)item.mYOfs);
						SexyTransform2D theMatrix2 = new SexyTransform2D(false);
						theMatrix2.Translate(sexyVector.x, sexyVector.y);
						item.mEffect.mEmitterTransform.CopyFrom(theMatrix2);
					}
					item.mEffect.mEmitterTransform.Translate(mParticleAttachOffset.mX, mParticleAttachOffset.mY);
				}
			}
		}

		public void UpdateParticles(PASpriteInst theSpriteInst, PAObjectPos theObjectPos)
		{
			if (theSpriteInst == null)
			{
				return;
			}
			for (int i = 0; i < theSpriteInst.mParticleEffectVector.Count; i++)
			{
				PAParticleEffect pAParticleEffect = theSpriteInst.mParticleEffectVector[i];
				SexyVector2 sexyVector = default(SexyVector2);
				if (!pAParticleEffect.mAttachEmitter)
				{
					sexyVector = theSpriteInst.mCurTransform.mMatrix * new SexyVector2((float)pAParticleEffect.mXOfs, (float)pAParticleEffect.mYOfs);
				}
				pAParticleEffect.mEffect.mDrawTransform.LoadIdentity();
				pAParticleEffect.mEffect.mDrawTransform.Translate(sexyVector.x, sexyVector.y);
				if (mMirror)
				{
					sexyVector.x = (float)mAnimRect.mWidth - sexyVector.x;
					pAParticleEffect.mEffect.mDrawTransform.Translate(-(mAnimRect.mWidth / 2), 0f);
					pAParticleEffect.mEffect.mDrawTransform.Scale(-1f, 1f);
					pAParticleEffect.mEffect.mDrawTransform.Translate(mAnimRect.mWidth / 2, 0f);
				}
				pAParticleEffect.mEffect.mDrawTransform.Scale(mDrawScale, mDrawScale);
				if (pAParticleEffect.mTransform && theObjectPos != null)
				{
					pAParticleEffect.mEffect.mAnimSpeed = 1f / theObjectPos.mTimeScale;
				}
				pAParticleEffect.mEffect.Update();
				pAParticleEffect.mLastUpdated = mUpdateCnt;
				if (!pAParticleEffect.mEffect.IsActive())
				{
					if (pAParticleEffect.mEffect != null)
					{
						pAParticleEffect.mEffect.Dispose();
					}
					theSpriteInst.mParticleEffectVector.RemoveAt(i);
					i--;
				}
			}
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[(int)theSpriteInst.mFrameNum];
			for (int j = 0; j < pAFrame.mFrameObjectPosVector.Length; j++)
			{
				PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[j];
				if (pAObjectPos.mIsSprite)
				{
					PASpriteInst mSpriteInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
					UpdateParticles(mSpriteInst, pAObjectPos);
				}
			}
		}

		public void CleanParticles(PASpriteInst theSpriteInst)
		{
			CleanParticles(theSpriteInst, false);
		}

		public void CleanParticles(PASpriteInst theSpriteInst, bool force)
		{
			if (theSpriteInst == null)
			{
				return;
			}
			for (int i = 0; i < theSpriteInst.mParticleEffectVector.Count; i++)
			{
				PAParticleEffect pAParticleEffect = theSpriteInst.mParticleEffectVector[i];
				if (pAParticleEffect.mLastUpdated != mUpdateCnt || force)
				{
					if (pAParticleEffect.mEffect != null)
					{
						pAParticleEffect.mEffect.Dispose();
					}
					theSpriteInst.mParticleEffectVector.RemoveAt(i);
					i--;
				}
			}
			foreach (PAObjectInst mChild in theSpriteInst.mChildren)
			{
				PASpriteInst mSpriteInst = mChild.mSpriteInst;
				if (mSpriteInst != null)
				{
					CleanParticles(mSpriteInst, force);
				}
			}
		}

		public bool HasParticles(PASpriteInst theSpriteInst)
		{
			if (theSpriteInst == null)
			{
				return false;
			}
			if (theSpriteInst.mParticleEffectVector.Count != 0)
			{
				return true;
			}
			for (int i = 0; i < theSpriteInst.mChildren.Count; i++)
			{
				PASpriteInst mSpriteInst = theSpriteInst.mChildren[i].mSpriteInst;
				if (mSpriteInst != null && HasParticles(mSpriteInst))
				{
					return true;
				}
			}
			return false;
		}

		public void IncSpriteInstFrame(PASpriteInst theSpriteInst, PAObjectPos theObjectPos, float theFrac)
		{
			int num = (int)theSpriteInst.mFrameNum;
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[num];
			if (pAFrame.mHasStop)
			{
				return;
			}
			float num2 = theObjectPos?.mTimeScale ?? 1f;
			theSpriteInst.mFrameNum += theFrac * (theSpriteInst.mDef.mAnimRate / (1000f / GlobalMembers.gSexyAppBase.mFrameTime)) / num2;
			if (theSpriteInst == mMainSpriteInst)
			{
				if (!theSpriteInst.mDef.mFrames[theSpriteInst.mDef.mFrames.Length - 1].mHasStop)
				{
					if ((int)theSpriteInst.mFrameNum >= theSpriteInst.mDef.mWorkAreaStart + theSpriteInst.mDef.mWorkAreaDuration + 1)
					{
						theSpriteInst.mFrameRepeats++;
						theSpriteInst.mFrameNum -= theSpriteInst.mDef.mWorkAreaDuration + 1;
					}
				}
				else if ((int)theSpriteInst.mFrameNum >= theSpriteInst.mDef.mWorkAreaStart + theSpriteInst.mDef.mWorkAreaDuration)
				{
					theSpriteInst.mOnNewFrame = true;
					theSpriteInst.mFrameNum = theSpriteInst.mDef.mWorkAreaStart + theSpriteInst.mDef.mWorkAreaDuration;
					if (theSpriteInst.mDef.mWorkAreaDuration != 0)
					{
						mAnimRunning = false;
						if (mListener != null)
						{
							mListener.PopAnimStopped(mId);
						}
						return;
					}
					theSpriteInst.mFrameRepeats++;
				}
			}
			else if ((int)theSpriteInst.mFrameNum >= theSpriteInst.mDef.mFrames.Length)
			{
				theSpriteInst.mFrameRepeats++;
				theSpriteInst.mFrameNum -= theSpriteInst.mDef.mFrames.Length;
			}
			theSpriteInst.mOnNewFrame = (int)theSpriteInst.mFrameNum != num;
			if (theSpriteInst.mOnNewFrame && theSpriteInst.mDelayFrames > 0)
			{
				theSpriteInst.mOnNewFrame = false;
				theSpriteInst.mFrameNum = num;
				theSpriteInst.mDelayFrames--;
				return;
			}
			for (int i = 0; i < pAFrame.mFrameObjectPosVector.Length; i++)
			{
				PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[i];
				if (pAObjectPos.mIsSprite)
				{
					PASpriteInst mSpriteInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
					IncSpriteInstFrame(mSpriteInst, pAObjectPos, theFrac / num2);
				}
			}
		}

		public void PrepSpriteInstFrame(PASpriteInst theSpriteInst, PAObjectPos theObjectPos)
		{
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[(int)theSpriteInst.mFrameNum];
			if (theSpriteInst.mOnNewFrame)
			{
				FrameHit(theSpriteInst, pAFrame, theObjectPos);
			}
			if (pAFrame.mHasStop)
			{
				if (theSpriteInst == mMainSpriteInst)
				{
					mAnimRunning = false;
					if (mListener != null)
					{
						mListener.PopAnimStopped(mId);
					}
				}
				return;
			}
			for (int i = 0; i < pAFrame.mFrameObjectPosVector.Length; i++)
			{
				PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[i];
				if (!pAObjectPos.mIsSprite)
				{
					continue;
				}
				PASpriteInst mSpriteInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
				if (mSpriteInst != null)
				{
					int num = (int)theSpriteInst.mFrameNum + theSpriteInst.mFrameRepeats * theSpriteInst.mDef.mFrames.Length;
					int num2 = num - 1;
					if (mSpriteInst.mLastUpdated != num2 && mSpriteInst.mLastUpdated != num)
					{
						mSpriteInst.mFrameNum = 0f;
						mSpriteInst.mFrameRepeats = 0;
						mSpriteInst.mDelayFrames = 0;
						mSpriteInst.mOnNewFrame = true;
					}
					PrepSpriteInstFrame(mSpriteInst, pAObjectPos);
					mSpriteInst.mLastUpdated = num;
				}
			}
		}

		public void AnimUpdate(float theFrac)
		{
			if (!mAnimRunning)
			{
				return;
			}
			if (mBlendTicksTotal > 0f)
			{
				mBlendTicksCur += theFrac;
				if (mBlendTicksCur >= mBlendTicksTotal)
				{
					mBlendTicksTotal = 0f;
				}
			}
			mTransDirty = true;
			if (mBlendDelay > 0f)
			{
				mBlendDelay -= theFrac;
				if (mBlendDelay <= 0f)
				{
					mBlendDelay = 0f;
					DoFramesHit(mMainSpriteInst, null);
				}
			}
			else
			{
				IncSpriteInstFrame(mMainSpriteInst, null, theFrac);
				PrepSpriteInstFrame(mMainSpriteInst, null);
				MarkDirty();
			}
		}

		public void ResetAnimHelper(PASpriteInst theSpriteInst)
		{
			theSpriteInst.mFrameNum = 0f;
			theSpriteInst.mFrameRepeats = 0;
			theSpriteInst.mDelayFrames = 0;
			theSpriteInst.mLastUpdated = -1;
			theSpriteInst.mOnNewFrame = true;
			for (int i = 0; i < theSpriteInst.mParticleEffectVector.Count; i++)
			{
				PAParticleEffect pAParticleEffect = theSpriteInst.mParticleEffectVector[i];
				pAParticleEffect.mEffect.ResetAnim();
			}
			for (int j = 0; j < theSpriteInst.mChildren.Count; j++)
			{
				PASpriteInst mSpriteInst = theSpriteInst.mChildren[j].mSpriteInst;
				if (mSpriteInst != null)
				{
					ResetAnimHelper(mSpriteInst);
				}
			}
			mTransDirty = true;
		}

		public void SaveStateSpriteInst(ref SexyFramework.Misc.Buffer theBuffer, PASpriteInst theSpriteInst)
		{
			theBuffer.WriteLong((long)(theSpriteInst.mFrameNum * 65536f));
			theBuffer.WriteLong(theSpriteInst.mDelayFrames);
			theBuffer.WriteLong(theSpriteInst.mLastUpdated);
			theBuffer.WriteShort((short)theSpriteInst.mParticleEffectVector.Count);
			for (int i = 0; i < theSpriteInst.mParticleEffectVector.Count; i++)
			{
				PAParticleEffect pAParticleEffect = theSpriteInst.mParticleEffectVector[i];
				pAParticleEffect.mEffect.SaveState(theBuffer);
				theBuffer.WriteString(pAParticleEffect.mName);
				theBuffer.WriteBoolean(pAParticleEffect.mBehind);
				theBuffer.WriteBoolean(pAParticleEffect.mAttachEmitter);
				theBuffer.WriteBoolean(pAParticleEffect.mTransform);
				theBuffer.WriteLong((int)(pAParticleEffect.mXOfs * 65536.0));
				theBuffer.WriteLong((int)(pAParticleEffect.mYOfs * 65536.0));
			}
			for (int j = 0; j < theSpriteInst.mChildren.Count; j++)
			{
				PAObjectInst pAObjectInst = theSpriteInst.mChildren[j];
				if (pAObjectInst.mSpriteInst != null)
				{
					SaveStateSpriteInst(ref theBuffer, pAObjectInst.mSpriteInst);
				}
			}
		}

		public void LoadStateSpriteInst(SexyFramework.Misc.Buffer theBuffer, PASpriteInst theSpriteInst)
		{
			theSpriteInst.mFrameNum = (float)theBuffer.ReadLong() / 65536f;
			theSpriteInst.mFrameRepeats = 0;
			theSpriteInst.mDelayFrames = (int)theBuffer.ReadLong();
			theSpriteInst.mLastUpdated = (int)theBuffer.ReadLong();
			theSpriteInst.mOnNewFrame = false;
			int num = theBuffer.ReadShort();
			for (int i = 0; i < num; i++)
			{
				PAParticleEffect pAParticleEffect = new PAParticleEffect();
				pAParticleEffect.mEffect.LoadState(theBuffer);
				pAParticleEffect.mName = theBuffer.ReadString();
				pAParticleEffect.mBehind = theBuffer.ReadBoolean();
				pAParticleEffect.mAttachEmitter = theBuffer.ReadBoolean();
				pAParticleEffect.mTransform = theBuffer.ReadBoolean();
				pAParticleEffect.mXOfs = (float)theBuffer.ReadLong() / 65536f;
				pAParticleEffect.mYOfs = (float)theBuffer.ReadLong() / 65536f;
				theSpriteInst.mParticleEffectVector.Add(pAParticleEffect);
			}
			for (int j = 0; j < theSpriteInst.mChildren.Count; j++)
			{
				PAObjectInst pAObjectInst = theSpriteInst.mChildren[j];
				if (pAObjectInst.mSpriteInst != null)
				{
					LoadStateSpriteInst(theBuffer, pAObjectInst.mSpriteInst);
				}
			}
		}

		public void DrawParticleEffects(SexyFramework.Graphics.Graphics g, PASpriteInst theSpriteInst, PATransform? theTransform, Color theColor, bool front)
		{
			if (theSpriteInst.mParticleEffectVector.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < theSpriteInst.mParticleEffectVector.Count; i++)
			{
				PAParticleEffect pAParticleEffect = theSpriteInst.mParticleEffectVector[i];
				if (pAParticleEffect.mTransform)
				{
					if (!pAParticleEffect.mAttachEmitter)
					{
						SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
						sexyTransform2D.Translate((float)pAParticleEffect.mEffect.mWidth / 2f, (float)pAParticleEffect.mEffect.mHeight / 2f);
						SexyTransform2D theMatrix = new SexyTransform2D(false);
						theMatrix.CopyFrom(theTransform.Value.mMatrix * sexyTransform2D);
						pAParticleEffect.mEffect.mDrawTransform.CopyFrom(theMatrix);
					}
					else
					{
						pAParticleEffect.mEffect.mDrawTransform.LoadIdentity();
						pAParticleEffect.mEffect.mDrawTransform.Translate(0f - mParticleAttachOffset.mX, 0f - mParticleAttachOffset.mY);
					}
					pAParticleEffect.mEffect.mColor = theColor;
				}
				if (pAParticleEffect.mBehind == !front)
				{
					pAParticleEffect.mEffect.Draw(g);
				}
			}
		}

		public virtual void DrawSprite(SexyFramework.Graphics.Graphics g, PASpriteInst theSpriteInst, PATransform? theTransform, Color theColor, bool additive, bool parentFrozen)
		{
			DrawParticleEffects(g, theSpriteInst, theTransform, theColor, false);
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[(int)theSpriteInst.mFrameNum];
			PATransform theTransform2 = default(PATransform);
			bool flag = parentFrozen || theSpriteInst.mDelayFrames > 0 || pAFrame.mHasStop;
			for (int i = 0; i < pAFrame.mFrameObjectPosVector.Length; i++)
			{
				PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[i];
				PAObjectInst pAObjectInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum];
				if (mListener != null && pAObjectInst.mPredrawCallback)
				{
					pAObjectInst.mPredrawCallback = mListener.PopAnimObjectPredraw(mId, g, theSpriteInst, pAObjectInst, theTransform.Value, theColor);
				}
				Color theColor2;
				if (pAObjectPos.mIsSprite)
				{
					PASpriteInst mSpriteInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
					theColor2 = mSpriteInst.mCurColor;
					theTransform2 = new PATransform(true);
					theTransform2.CopyFrom(mSpriteInst.mCurTransform);
				}
				else
				{
					CalcObjectPos(theSpriteInst, i, flag, ref theTransform2, out theColor2);
				}
				PATransform outTran = new PATransform(true);
				if (!theTransform.HasValue && mDrawScale != 1f)
				{
					PATransform pATransform = new PATransform(true);
					pATransform.mMatrix.m00 = mDrawScale;
					pATransform.mMatrix.m11 = mDrawScale;
					pATransform.mMatrix.CopyFrom(mTransform * pATransform.mMatrix);
					pATransform.TransformSrc(theTransform2, ref outTran);
				}
				else if (!theTransform.HasValue || pAObjectPos.mIsSprite)
				{
					outTran.CopyFrom(theTransform2);
					if (mDrawScale != 1f)
					{
						PATransform pATransform2 = new PATransform(true);
						pATransform2.mMatrix.m00 = mDrawScale;
						pATransform2.mMatrix.m11 = mDrawScale;
						outTran.mMatrix.CopyFrom(pATransform2.mMatrix * outTran.mMatrix);
					}
					outTran.mMatrix.CopyFrom(mTransform * outTran.mMatrix);
				}
				else
				{
					theTransform.Value.TransformSrc(theTransform2, ref outTran);
				}
				Color color = new Color(theColor2.mRed * theColor.mRed * pAObjectInst.mColorMult.mRed / 65025, theColor2.mGreen * theColor.mGreen * pAObjectInst.mColorMult.mGreen / 65025, theColor2.mBlue * theColor.mBlue * pAObjectInst.mColorMult.mBlue / 65025, theColor2.mAlpha * theColor.mAlpha * pAObjectInst.mColorMult.mAlpha / 65025);
				if (color.mAlpha == 0)
				{
					continue;
				}
				if (pAObjectPos.mIsSprite)
				{
					PASpriteInst mSpriteInst2 = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
					DrawSprite(g, mSpriteInst2, outTran, color, pAObjectPos.mIsAdditive || additive, flag);
				}
				else
				{
					int num = 0;
					while (true)
					{
						PAImage pAImage = mImageVector[pAObjectPos.mResNum];
						PATransform outTran2 = new PATransform(true);
						outTran.TransformSrc(pAImage.mTransform, ref outTran2);
						g.SetColorizeImages(true);
						g.SetColor(color);
						if (additive || pAObjectPos.mIsAdditive)
						{
							g.SetDrawMode(1);
						}
						else
						{
							g.SetDrawMode(pAImage.mDrawMode);
						}
						Rect rect = default(Rect);
						DeviceImage deviceImage;
						if (pAObjectPos.mAnimFrameNum == 0 || pAImage.mImages.Count() == 1)
						{
							deviceImage = (DeviceImage)pAImage.mImages[0].GetImage();
							rect = deviceImage.GetCelRect(pAObjectPos.mAnimFrameNum);
						}
						else
						{
							deviceImage = (DeviceImage)pAImage.mImages[pAObjectPos.mAnimFrameNum].GetImage();
							rect = deviceImage.GetCelRect(0);
						}
						if (pAObjectPos.mHasSrcRect)
						{
							rect = pAObjectPos.mSrcRect;
						}
						if (mImgScale != 1f)
						{
							float m = outTran2.mMatrix.m02;
							float m2 = outTran2.mMatrix.m12;
							PATransform pATransform3 = new PATransform(true);
							pATransform3.mMatrix.m00 = 1f / mImgScale;
							pATransform3.mMatrix.m11 = 1f / mImgScale;
							pATransform3.TransformSrc(outTran2, ref outTran2);
							outTran2.mMatrix.m02 = m;
							outTran2.mMatrix.m12 = m2;
						}
						ImagePredrawResult imagePredrawResult = ImagePredrawResult.ImagePredraw_DontAsk;
						if (mListener != null && pAObjectInst.mImagePredrawCallback)
						{
							imagePredrawResult = mListener.PopAnimImagePredraw(mId, theSpriteInst, pAObjectInst, outTran2, deviceImage, g, num);
							if (imagePredrawResult == ImagePredrawResult.ImagePredraw_DontAsk)
							{
								pAObjectInst.mImagePredrawCallback = false;
							}
							if (imagePredrawResult == ImagePredrawResult.ImagePredraw_Skip)
							{
								break;
							}
						}
						SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
						sexyTransform2D.LoadIdentity();
						sexyTransform2D.m02 = (float)rect.mWidth / 2f;
						sexyTransform2D.m12 = (float)rect.mHeight / 2f;
						outTran2.mMatrix.CopyFrom(outTran2.mMatrix * sexyTransform2D);
						if (mColorizeType)
						{
							g.SetColor(255, 0, 0);
						}
						g.DrawImageMatrix(deviceImage, outTran2.mMatrix, rect);
						if (imagePredrawResult != ImagePredrawResult.ImagePredraw_Repeat)
						{
							break;
						}
						num++;
					}
				}
				if (mListener != null && pAObjectInst.mPostdrawCallback)
				{
					pAObjectInst.mPostdrawCallback = mListener.PopAnimObjectPostdraw(mId, g, theSpriteInst, pAObjectInst, theTransform.Value, theColor);
				}
			}
			DrawParticleEffects(g, theSpriteInst, theTransform, theColor, true);
			g.SetColorizeImages(false);
			g.SetDrawMode(0);
		}

		public virtual void DrawSpriteMirrored(SexyFramework.Graphics.Graphics g, PASpriteInst theSpriteInst, PATransform? theTransform, Color theColor, bool additive, bool parentFrozen)
		{
			DrawParticleEffects(g, theSpriteInst, theTransform, new Color(theColor), false);
			PAFrame pAFrame = theSpriteInst.mDef.mFrames[(int)theSpriteInst.mFrameNum];
			PATransform theTransform2 = default(PATransform);
			bool flag = parentFrozen || theSpriteInst.mDelayFrames > 0 || pAFrame.mHasStop;
			for (int i = 0; i < pAFrame.mFrameObjectPosVector.Length; i++)
			{
				PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[i];
				Color theColor2;
				if (pAObjectPos.mIsSprite)
				{
					PASpriteInst mSpriteInst = theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
					theColor2 = mSpriteInst.mCurColor;
					theTransform2 = mSpriteInst.mCurTransform;
				}
				else
				{
					CalcObjectPos(theSpriteInst, i, flag, ref theTransform2, out theColor2);
				}
				PATransform outTran = new PATransform(true);
				if (!theTransform.HasValue && mDrawScale != 1f)
				{
					PATransform pATransform = new PATransform(true);
					pATransform.mMatrix.m00 = mDrawScale;
					pATransform.mMatrix.m11 = mDrawScale;
					pATransform.TransformSrc(theTransform2, ref outTran);
				}
				else if (!theTransform.HasValue || pAObjectPos.mIsSprite)
				{
					outTran.CopyFrom(theTransform2);
				}
				else
				{
					theTransform.Value.TransformSrc(theTransform2, ref outTran);
				}
				Color color = new Color(theColor2.mRed * theColor.mRed / 255, theColor2.mGreen * theColor.mGreen / 255, theColor2.mBlue * theColor.mBlue / 255, theColor2.mAlpha * theColor.mAlpha / 255);
				if (color.mAlpha == 0)
				{
					continue;
				}
				if (pAObjectPos.mIsSprite)
				{
					DrawSpriteMirrored(g, theSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst, outTran, new Color(color), pAObjectPos.mIsAdditive || additive, flag);
					continue;
				}
				PAImage pAImage = mImageVector[pAObjectPos.mResNum];
				PATransform outTran2 = new PATransform(true);
				outTran.TransformSrc(pAImage.mTransform, ref outTran2);
				g.SetColorizeImages(true);
				g.SetColor(color);
				if (additive || pAObjectPos.mIsAdditive)
				{
					g.SetDrawMode(1);
				}
				else
				{
					g.SetDrawMode(pAImage.mDrawMode);
				}
				Image image = null;
				Rect rect = default(Rect);
				if (pAObjectPos.mAnimFrameNum == 0 || pAImage.mImages.Count == 1)
				{
					image = pAImage.mImages[0].GetImage();
					rect = image.GetCelRect(pAObjectPos.mAnimFrameNum);
				}
				else
				{
					image = pAImage.mImages[pAObjectPos.mAnimFrameNum].GetImage();
					rect = image.GetCelRect(0);
				}
				if (pAObjectPos.mHasSrcRect)
				{
					rect = pAObjectPos.mSrcRect;
				}
				if (mImgScale != 1f)
				{
					float m = outTran2.mMatrix.m02;
					float m2 = outTran2.mMatrix.m12;
					PATransform pATransform2 = new PATransform(true);
					pATransform2.mMatrix.m00 = 1f / mImgScale;
					pATransform2.mMatrix.m11 = 1f / mImgScale;
					pATransform2.TransformSrc(outTran2, ref outTran2);
					outTran2.mMatrix.m02 = m;
					outTran2.mMatrix.m12 = m2;
				}
				if (mDrawScale != 1f)
				{
					outTran2.mMatrix.m02 += (float)mAnimRect.mWidth * (1f - mDrawScale);
				}
				if ((double)outTran2.mMatrix.m00 == 1.0 && outTran2.mMatrix.m01 == 0f && outTran2.mMatrix.m10 == 0f && (double)outTran2.mMatrix.m11 == 1.0)
				{
					float theX = (float)(mAnimRect.mWidth - rect.mWidth) / 2f - (outTran2.mMatrix.m02 + (float)(rect.mWidth - mAnimRect.mWidth) / 2f);
					float m3 = outTran2.mMatrix.m12;
					g.DrawImageF(image, theX, m3, rect);
				}
				else if (mVersion == 1 || (outTran2.mMatrix.m00 == outTran2.mMatrix.m11 && outTran2.mMatrix.m01 == 0f - outTran2.mMatrix.m10 && Math.Abs((double)(outTran2.mMatrix.m00 * outTran2.mMatrix.m00 + outTran2.mMatrix.m01 * outTran2.mMatrix.m01) - 1.0) < 0.01))
				{
					float num = (float)Math.Atan2(outTran2.mMatrix.m01, outTran2.mMatrix.m00);
					float num2 = 0f - num;
					float num3 = outTran2.mMatrix.m02 + (float)Math.Cos(num2) * (float)rect.mWidth / 2f - (float)Math.Sin(num2) * (float)rect.mHeight / 2f;
					float num4 = outTran2.mMatrix.m12 + (float)Math.Sin(num2) * (float)rect.mWidth / 2f + (float)Math.Cos(num2) * (float)rect.mHeight / 2f;
					float num5 = num3 - (float)rect.mWidth / 2f;
					float theY = num4 - (float)rect.mHeight / 2f;
					num5 = (float)(mAnimRect.mWidth - rect.mWidth) / 2f - (num5 + (float)(rect.mWidth - mAnimRect.mWidth) / 2f);
					g.DrawImageRotatedF(image, num5, theY, 0f - num, rect);
				}
				else
				{
					SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
					sexyTransform2D.m02 = (float)rect.mWidth / 2f;
					sexyTransform2D.m12 = (float)rect.mHeight / 2f;
					outTran2.mMatrix.CopyFrom(outTran2.mMatrix * sexyTransform2D);
					outTran2.mMatrix.m02 = (float)mAnimRect.mWidth - outTran2.mMatrix.m02;
					outTran2.mMatrix.m01 = 0f - outTran2.mMatrix.m01;
					outTran2.mMatrix.m10 = 0f - outTran2.mMatrix.m10;
					g.DrawImageMatrix(image, outTran2.mMatrix, rect);
				}
			}
			DrawParticleEffects(g, theSpriteInst, theTransform, theColor, true);
			g.SetColorizeImages(false);
			g.SetDrawMode(0);
		}

		public virtual void Load_Init()
		{
			Clear();
		}

		public virtual void Load_SetModPamFile(string theFileName)
		{
			mModPamFile = theFileName;
		}

		public virtual void Load_AddRemap(string theWildcard, string theReplacement)
		{
			mRemapList.AddLast(new KeyValuePair<string, string>(theWildcard, theReplacement));
		}

		public virtual bool Load_LoadPam(string theFileName)
		{
			mLoadedPamFile = theFileName;
			if (mMainAnimDef != null)
			{
				return false;
			}
			mMainSpriteInst = new PASpriteInst();
			mMainAnimDef = new PopAnimDef();
			mMainSpriteInst.mParent = null;
			mMainSpriteInst.mDef = null;
			SexyFramework.Misc.Buffer theBuffer = new SexyFramework.Misc.Buffer();
			string fileDir = Common.GetFileDir(theFileName, false);
			if (!GlobalMembers.gSexyAppBase.ReadBufferFromStream(theFileName, ref theBuffer))
			{
				return Fail("Unable to load file: " + theFileName);
			}
			uint num = (uint)theBuffer.ReadLong();
			if (num != PAM_MAGIC)
			{
				return Fail("Invalid header");
			}
			mVersion = (int)theBuffer.ReadLong();
			if ((uint)mVersion > PAM_VERSION)
			{
				return Fail("Invalid version");
			}
			mAnimRate = theBuffer.ReadByte();
			mAnimRect.mX = theBuffer.ReadShort() / 20;
			mAnimRect.mY = theBuffer.ReadShort() / 20;
			mAnimRect.mWidth = (ushort)theBuffer.ReadShort() / 20;
			mAnimRect.mHeight = (ushort)theBuffer.ReadShort() / 20;
			int num2 = theBuffer.ReadShort();
			Common.Resize(mImageVector, num2);
			for (int i = 0; i < num2; i++)
			{
				PAImage pAImage = mImageVector[i];
				pAImage.mDrawMode = 0;
				string theString = theBuffer.ReadString();
				string text = Remap(theString);
				string text2 = "";
				int num3 = text.IndexOf('(');
				int num4 = text.IndexOf(')');
				if (num3 != -1 && num4 != -1 && num3 < num4)
				{
					text2 = text.Substring(num3 + 1, num4 - num3 - 1).ToLower();
					text = (text.Substring(0, num3) + text.Substring(num4 + 1)).Trim();
				}
				else
				{
					num4 = text.IndexOf('$');
					if (num4 != -1)
					{
						text2 = text.Substring(0, num4).ToLower();
						text = text.Substring(num4 + 1).Trim();
					}
				}
				pAImage.mCols = 1;
				pAImage.mRows = 1;
				num3 = text.IndexOf('[');
				num4 = text.IndexOf(']');
				if (num3 != -1 && num4 != -1 && num3 < num4)
				{
					string text3 = text.Substring(num3 + 1, num4 - num3 - 1).ToLower();
					text = (text.Substring(0, num3) + text.Substring(num4 + 1)).Trim();
					int num5 = text3.IndexOf(',');
					if (num5 != -1)
					{
						pAImage.mCols = Convert.ToInt32(text3.Substring(0, num5));
						pAImage.mRows = Convert.ToInt32(text3.Substring(num5 + 1));
					}
				}
				if (text2.IndexOf("add") != -1)
				{
					pAImage.mDrawMode = 1;
				}
				if (mVersion >= 4)
				{
					pAImage.mOrigWidth = theBuffer.ReadShort();
					pAImage.mOrigHeight = theBuffer.ReadShort();
				}
				else
				{
					pAImage.mOrigWidth = -1;
					pAImage.mOrigHeight = -1;
				}
				if (mVersion == 1)
				{
					float num6 = (float)theBuffer.ReadShort() / 1000f;
					float num7 = (float)Math.Sin(num6);
					float num8 = (float)Math.Cos(num6);
					pAImage.mTransform.mMatrix.m00 = num8;
					pAImage.mTransform.mMatrix.m01 = 0f - num7;
					pAImage.mTransform.mMatrix.m10 = num7;
					pAImage.mTransform.mMatrix.m11 = num8;
					pAImage.mTransform.mMatrix.m02 = (float)theBuffer.ReadShort() / 20f;
					pAImage.mTransform.mMatrix.m12 = (float)theBuffer.ReadShort() / 20f;
				}
				else
				{
					pAImage.mTransform.mMatrix.m00 = (float)theBuffer.ReadLong() / 1310720f;
					pAImage.mTransform.mMatrix.m01 = (float)theBuffer.ReadLong() / 1310720f;
					pAImage.mTransform.mMatrix.m10 = (float)theBuffer.ReadLong() / 1310720f;
					pAImage.mTransform.mMatrix.m11 = (float)theBuffer.ReadLong() / 1310720f;
					pAImage.mTransform.mMatrix.m02 = (float)theBuffer.ReadShort() / 20f;
					pAImage.mTransform.mMatrix.m12 = (float)theBuffer.ReadShort() / 20f;
				}
				pAImage.mImageName = text;
				if (pAImage.mImageName.Length == 0)
				{
					bool isNew = false;
					SharedImageRef sharedImageRef = new SharedImageRef();
					sharedImageRef = GlobalMembers.gSexyAppBase.GetSharedImage("!whitepixel", "", ref isNew, true, false);
					pAImage.mImages.Add(sharedImageRef);
				}
				else
				{
					int num9 = 0;
					while (num9 < text.Length)
					{
						int num10 = text.IndexOf(',', num9);
						string text4 = ((num10 == -1) ? text.Substring(num9) : text.Substring(num9, num10 - num9));
						Load_GetImage(pAImage, fileDir, text4, text4);
						if (num10 == -1)
						{
							break;
						}
						num9 = num10 + 1;
					}
				}
				if (mError.Length > 0)
				{
					return false;
				}
				if (mMirror && mLoadedImageIsNew)
				{
					for (int j = 0; j < pAImage.mImages.Count; j++)
					{
						GlobalMembers.gSexyAppBase.MirrorImage(pAImage.mImages[j].GetImage());
					}
				}
				Load_PostImageLoadHook(pAImage);
			}
			mMotionFilePos = theBuffer.mReadBitPos / 8;
			int num11 = theBuffer.ReadShort();
			Common.Resize(mMainAnimDef.mSpriteDefVector, num11);
			for (int k = 0; k < num11; k++)
			{
				if (!LoadSpriteDef(theBuffer, mMainAnimDef.mSpriteDefVector[k]))
				{
					return false;
				}
			}
			if (mVersion <= 3 || theBuffer.ReadBoolean())
			{
				mMainAnimDef.mMainSpriteDef = new PASpriteDef();
				if (!LoadSpriteDef(theBuffer, mMainAnimDef.mMainSpriteDef))
				{
					return false;
				}
			}
			mLoaded = true;
			mRandUsed = false;
			return true;
		}

		public virtual bool Load_LoadMod(string theFileName)
		{
			PopAnimModParser popAnimModParser = new PopAnimModParser();
			popAnimModParser.mErrorHeader = "PopAnim Mod File Error in " + theFileName + "\r\n";
			popAnimModParser.mPopAnim = this;
			popAnimModParser.mPassNum = 1;
			if (!popAnimModParser.LoadDescriptor(theFileName))
			{
				return false;
			}
			if (mModPamFile.Length == 0)
			{
				return Fail("No Pam file specified");
			}
			string pathFrom = Common.GetPathFrom(mModPamFile, Common.GetFileDir(theFileName, false));
			if (!Load_LoadPam(pathFrom))
			{
				return popAnimModParser.Error("Failed to load Pam: " + mModPamFile + "\r\n\r\n" + mError);
			}
			popAnimModParser.mPassNum = 2;
			if (!popAnimModParser.LoadDescriptor(theFileName))
			{
				return false;
			}
			return true;
		}

		public virtual SharedImageRef Load_GetImageHook(string theFileDir, string theOrigName, string theRemappedName)
		{
			if (theRemappedName.Length == 0)
			{
				Fail("No image file name specified");
				return null;
			}
			for (int i = 0; i < mImageSearchPathVector.Count; i++)
			{
				string text = Common.GetPathFrom(mImageSearchPathVector[i], theFileDir);
				if (text.Length > 0 && text[text.Length - 1] != '\\' && text[text.Length - 1] != '/')
				{
					text += "\\";
				}
				text += theRemappedName;
				SharedImageRef sharedImage = GlobalMembers.gSexyAppBase.GetSharedImage(text, mMirror ? "MIRRORED" : "", ref mLoadedImageIsNew, true, false);
				if (sharedImage.GetImage() != null)
				{
					return sharedImage;
				}
			}
			Fail("Unable to load image: " + theRemappedName + " (" + theOrigName + ")");
			return null;
		}

		public virtual bool Load_GetImage(PAImage theImage, string theFileDir, string theOrigName, string theRemappedName)
		{
			SharedImageRef sharedImageRef = Load_GetImageHook(theFileDir, theOrigName, theRemappedName);
			if (sharedImageRef.GetDeviceImage() == null)
			{
				return false;
			}
			sharedImageRef.GetImage().mNumCols = theImage.mCols;
			sharedImageRef.GetImage().mNumRows = theImage.mRows;
			if (theImage.mImages.Count == 0 && theImage.mOrigWidth != -1 && theImage.mOrigHeight != -1)
			{
				theImage.mTransform.mMatrix.m20 += (0f - ((float)sharedImageRef.mWidth - (float)theImage.mOrigWidth * mImgScale)) / (float)(sharedImageRef.GetImage().mNumCols + 1);
				theImage.mTransform.mMatrix.m21 += (0f - ((float)sharedImageRef.mHeight - (float)theImage.mOrigHeight * mImgScale)) / (float)(sharedImageRef.GetImage().mNumRows + 1);
			}
			theImage.mImages.Add(sharedImageRef);
			return true;
		}

		public virtual void Load_PostImageLoadHook(PAImage theImage)
		{
		}

		public PopAnim(int theId, PopAnimListener theListener)
		{
			GlobalMembers.gSexyAppBase.mPopAnimSet.Add(this);
			mId = theId;
			mListener = theListener;
			mMirror = false;
			mAdditive = false;
			mColor = new Color(Color.White);
			mAnimRate = 0;
			mLoaded = false;
			mAnimRunning = false;
			mMainSpriteInst = null;
			mMainAnimDef = null;
			mInterpolate = true;
			mLoadedImageIsNew = false;
			mRandUsed = false;
			Clear();
			mVersion = 0;
			mPaused = false;
			mColorizeType = false;
			mImgScale = 1f;
			mDrawScale = 1f;
			mTransDirty = true;
			mBlendTicksTotal = 0f;
			mBlendTicksCur = 0f;
			mBlendDelay = 0f;
			mImageSearchPathVector.Add("images\\");
			mImageSearchPathVector.Add("");
		}

		public PopAnim(PopAnim rhs)
		{
			GlobalMembers.gSexyAppBase.mPopAnimSet.Add(this);
			CopyForm(rhs);
			mMainSpriteInst = new PASpriteInst();
			mMainSpriteInst.mDef = null;
			mMainSpriteInst.mParent = null;
			mMainAnimDef.mRefCount++;
		}

		public override void Dispose()
		{
			GlobalMembers.gSexyAppBase.mPopAnimSet.Remove(this);
			base.Dispose();
		}

		public void CopyForm(PopAnim rhs)
		{
			CopyFrom(rhs);
			mId = rhs.mId;
			mListener = rhs.mListener;
			mVersion = rhs.mVersion;
			mCRCBuffer = rhs.mCRCBuffer;
			mDrawScale = rhs.mDrawScale;
			mImgScale = rhs.mImgScale;
			mLoaded = rhs.mLoaded;
			mMotionFilePos = rhs.mMotionFilePos;
			mModPamFile = rhs.mModPamFile;
			mLoadedPamFile = rhs.mLoadedPamFile;
			mAnimRate = rhs.mAnimRate;
			mError = rhs.mError;
			mLastPlayedFrameLabel = rhs.mLastPlayedFrameLabel;
			mMainAnimDef = rhs.mMainAnimDef;
			mBlendTicksTotal = rhs.mBlendTicksTotal;
			mBlendTicksCur = rhs.mBlendTicksCur;
			mBlendDelay = rhs.mBlendDelay;
			mRandUsed = rhs.mRandUsed;
			mAdditive = rhs.mAdditive;
			mTransDirty = rhs.mTransDirty;
			mAnimRunning = rhs.mAnimRunning;
			mMirror = rhs.mMirror;
			mInterpolate = rhs.mInterpolate;
			mLoadedImageIsNew = rhs.mLoadedImageIsNew;
			mPaused = rhs.mPaused;
			mColorizeType = rhs.mColorizeType;
			Remap_aString = rhs.Remap_aString;
			mParticleAttachOffset.mX = rhs.mParticleAttachOffset.mX;
			mParticleAttachOffset.mY = rhs.mParticleAttachOffset.mY;
			mTransform.CopyFrom(rhs.mTransform);
			mColor = new Color(rhs.mColor);
			mAnimRect.SetValue(rhs.mAnimRect.mX, rhs.mAnimRect.mY, rhs.mAnimRect.mWidth, rhs.mAnimRect.mHeight);
			mImageSearchPathVector = rhs.mImageSearchPathVector;
			mRemapList = rhs.mRemapList;
			mImageVector = rhs.mImageVector;
		}

		public void Clear()
		{
			mMirror = false;
			mColor = new Color(Color.White);
			mLoaded = false;
			mRandUsed = false;
			mAnimRunning = false;
			mAnimRate = 0;
			mError = "";
			mImageVector.Clear();
			mModPamFile = "";
			mRemapList.Clear();
			mCRCBuffer.Clear();
			if (mMainAnimDef != null)
			{
				if (mMainAnimDef.mRefCount == 0)
				{
					mMainAnimDef.mSpriteDefVector.Clear();
					if (mMainAnimDef != null)
					{
						mMainAnimDef.Dispose();
					}
				}
				else
				{
					mMainAnimDef.mRefCount--;
				}
			}
			mMainAnimDef = null;
			mTransDirty = true;
			if (mMainSpriteInst != null)
			{
				mMainSpriteInst.Dispose();
			}
			mMainSpriteInst = null;
		}

		public PopAnim Duplicate()
		{
			return new PopAnim(this);
		}

		public bool LoadFile(string theFileName)
		{
			return LoadFile(theFileName, false);
		}

		public bool LoadFile(string theFileName, bool doMirror)
		{
			Load_Init();
			mMirror = doMirror;
			string text = "";
			int num = theFileName.LastIndexOf('.');
			if (num != -1)
			{
				text = theFileName.Substring(num).ToLower();
			}
			if (text == ".pam")
			{
				return Load_LoadPam(theFileName);
			}
			if (text == ".txt")
			{
				if (Load_LoadMod(theFileName))
				{
					return true;
				}
				if (mError.Length == 0)
				{
					mError = "Mod file loading error";
				}
				return false;
			}
			if (text.Length == 0)
			{
				if (Load_LoadPam(theFileName + ".pam"))
				{
					return true;
				}
				if (Load_LoadMod(theFileName + ".txt"))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public bool LoadState(SexyFramework.Misc.Buffer theBuffer)
		{
			theBuffer.mReadBitPos = (theBuffer.mReadBitPos + 7) & -8;
			int num = (int)theBuffer.ReadLong();
			int num2 = theBuffer.mReadBitPos / 8 + num;
			int num3 = theBuffer.ReadShort();
			if (theBuffer.ReadBoolean())
			{
				string theFileName = theBuffer.ReadString();
				int num4 = (int)theBuffer.ReadLong();
				mMirror = theBuffer.ReadBoolean();
				if (!mLoaded)
				{
					Load_LoadPam(theFileName);
				}
				else if (mMainSpriteInst != null)
				{
					ResetAnimHelper(mMainSpriteInst);
					CleanParticles(mMainSpriteInst, true);
					mAnimRunning = false;
				}
				mAnimRunning = theBuffer.ReadBoolean();
				mPaused = theBuffer.ReadBoolean();
				if (num4 != (int)mCRCBuffer.GetCRC32(0uL))
				{
					theBuffer.mReadBitPos = num2 * 8;
					return false;
				}
				string theName = theBuffer.ReadString();
				SetupSpriteInst(theName);
				LoadStateSpriteInst(theBuffer, mMainSpriteInst);
				if (num3 >= 1)
				{
					mRandUsed = theBuffer.ReadBoolean();
					if (mRandUsed)
					{
						mRand.SRand(theBuffer.ReadString());
					}
				}
			}
			return true;
		}

		public bool SaveState(ref SexyFramework.Misc.Buffer theBuffer)
		{
			theBuffer.mWriteBitPos = (theBuffer.mWriteBitPos + 7) & -8;
			int num = theBuffer.mWriteBitPos / 8;
			theBuffer.WriteLong(0L);
			theBuffer.WriteShort((short)PAM_STATE_VERSION);
			theBuffer.WriteBoolean(mLoaded);
			if (mLoaded)
			{
				theBuffer.WriteString(mLoadedPamFile);
				theBuffer.WriteLong((long)mCRCBuffer.GetCRC32(0uL));
				theBuffer.WriteBoolean(mMirror);
				theBuffer.WriteBoolean(mAnimRunning);
				theBuffer.WriteBoolean(mPaused);
				SetupSpriteInst();
				theBuffer.WriteString((mMainSpriteInst.mDef.mName != null) ? mMainSpriteInst.mDef.mName : "");
				SaveStateSpriteInst(ref theBuffer, mMainSpriteInst);
				theBuffer.WriteBoolean(mRandUsed);
				if (mRandUsed)
				{
					theBuffer.WriteString(mRand.Serialize());
				}
			}
			int num2 = theBuffer.mWriteBitPos / 8 - num - 4;
			theBuffer.mData[num] = (byte)num2;
			return true;
		}

		public void ResetAnim()
		{
			ResetAnimHelper(mMainSpriteInst);
			CleanParticles(mMainSpriteInst, true);
			mAnimRunning = false;
			GetToFirstFrame();
			mBlendTicksTotal = 0f;
			mBlendTicksCur = 0f;
			mBlendDelay = 0f;
		}

		public bool SetupSpriteInst()
		{
			return SetupSpriteInst("");
		}

		public bool SetupSpriteInst(string theName)
		{
			if (mMainSpriteInst == null)
			{
				return false;
			}
			if (mMainSpriteInst.mDef != null && theName.Length == 0)
			{
				return true;
			}
			if (mMainAnimDef.mMainSpriteDef != null)
			{
				InitSpriteInst(mMainSpriteInst, mMainAnimDef.mMainSpriteDef);
				return true;
			}
			if (mMainAnimDef.mSpriteDefVector.Count == 0)
			{
				return false;
			}
			string text = theName;
			if (text.Length == 0)
			{
				text = "main";
			}
			PASpriteDef pASpriteDef = null;
			for (int i = 0; i < mMainAnimDef.mSpriteDefVector.Count; i++)
			{
				if (mMainAnimDef.mSpriteDefVector[i].mName != null && mMainAnimDef.mSpriteDefVector[i].mName == text)
				{
					pASpriteDef = mMainAnimDef.mSpriteDefVector[i];
				}
			}
			if (pASpriteDef == null)
			{
				pASpriteDef = mMainAnimDef.mSpriteDefVector[0];
			}
			if (pASpriteDef != mMainSpriteInst.mDef)
			{
				if (mMainSpriteInst.mDef != null)
				{
					if (mMainSpriteInst != null)
					{
						mMainSpriteInst.Dispose();
					}
					mMainSpriteInst.mParent = null;
				}
				InitSpriteInst(mMainSpriteInst, pASpriteDef);
				mTransDirty = true;
			}
			return true;
		}

		public bool Play(int theFrameNum)
		{
			return Play(theFrameNum, true);
		}

		public bool Play()
		{
			return Play(0, true);
		}

		public bool Play(int theFrameNum, bool resetAnim)
		{
			if (!SetupSpriteInst())
			{
				return false;
			}
			if (theFrameNum >= mMainSpriteInst.mDef.mFrames.Length)
			{
				mAnimRunning = false;
				return false;
			}
			if (mMainSpriteInst.mFrameNum != (float)theFrameNum && resetAnim)
			{
				ResetAnim();
			}
			mPaused = false;
			mAnimRunning = true;
			mMainSpriteInst.mDelayFrames = 0;
			mMainSpriteInst.mFrameNum = theFrameNum;
			mMainSpriteInst.mFrameRepeats = 0;
			if (resetAnim)
			{
				CleanParticles(mMainSpriteInst, true);
			}
			if (mBlendDelay == 0f)
			{
				DoFramesHit(mMainSpriteInst, null);
			}
			MarkDirty();
			return true;
		}

		public bool Play(string theFrameLabel)
		{
			return Play(theFrameLabel, true);
		}

		public bool Play(string theFrameLabel, bool resetAnim)
		{
			mAnimRunning = false;
			if (mMainAnimDef.mMainSpriteDef != null)
			{
				if (!SetupSpriteInst())
				{
					return false;
				}
				int labelFrame = mMainAnimDef.mMainSpriteDef.GetLabelFrame(theFrameLabel);
				if (labelFrame == -1)
				{
					return false;
				}
				mLastPlayedFrameLabel = theFrameLabel;
				return Play(labelFrame, resetAnim);
			}
			SetupSpriteInst(theFrameLabel);
			return Play(mMainSpriteInst.mDef.mWorkAreaStart, resetAnim);
		}

		public bool BlendTo(string theFrameLabel, int theBlendTicks)
		{
			return BlendTo(theFrameLabel, theBlendTicks, 0);
		}

		public bool BlendTo(string theFrameLabel, int theBlendTicks, int theAnimStartDelay)
		{
			if (!SetupSpriteInst())
			{
				return false;
			}
			if (!mTransDirty)
			{
				UpdateTransforms(mMainSpriteInst, null, new Color(mColor), false);
				mTransDirty = false;
			}
			Dictionary<string, BlendSrcData> dictionary = new Dictionary<string, BlendSrcData>();
			PAFrame pAFrame = mMainSpriteInst.mDef.mFrames[(int)mMainSpriteInst.mFrameNum];
			PATransform theTransform = new PATransform(true);
			for (int i = 0; i < pAFrame.mFrameObjectPosVector.Length; i++)
			{
				PAObjectPos pAObjectPos = pAFrame.mFrameObjectPosVector[i];
				PAObjectInst pAObjectInst = mMainSpriteInst.mChildren[pAObjectPos.mObjectNum];
				if (pAObjectInst.mName != null && pAObjectInst.mName[0] != 0)
				{
					Color theColor;
					if (pAObjectPos.mIsSprite)
					{
						PASpriteInst mSpriteInst = mMainSpriteInst.mChildren[pAObjectPos.mObjectNum].mSpriteInst;
						theColor = mSpriteInst.mCurColor.Clone();
						theTransform = mSpriteInst.mCurTransform.Clone();
					}
					else
					{
						CalcObjectPos(mMainSpriteInst, i, false, ref theTransform, out theColor);
					}
					BlendSrcData blendSrcData = new BlendSrcData();
					blendSrcData.mTransform = theTransform;
					blendSrcData.mColor = theColor;
					if (pAObjectInst.mSpriteInst != null)
					{
						blendSrcData.mParticleEffectVector = pAObjectInst.mSpriteInst.mParticleEffectVector;
						pAObjectInst.mSpriteInst.mParticleEffectVector.Clear();
					}
					dictionary.Add(pAObjectPos.mName, blendSrcData);
				}
			}
			List<PAParticleEffect> mParticleEffectVector = mMainSpriteInst.mParticleEffectVector;
			mMainSpriteInst.mParticleEffectVector.Clear();
			mBlendTicksTotal = theBlendTicks;
			mBlendTicksCur = 0f;
			mBlendDelay = theAnimStartDelay;
			if (mMainAnimDef.mMainSpriteDef != null)
			{
				if (!SetupSpriteInst())
				{
					return false;
				}
				int labelFrame = mMainAnimDef.mMainSpriteDef.GetLabelFrame(theFrameLabel);
				if (labelFrame == -1)
				{
					return false;
				}
				mLastPlayedFrameLabel = theFrameLabel;
				Play(labelFrame, false);
				mTransDirty = true;
			}
			else
			{
				SetupSpriteInst(theFrameLabel);
				Play(mMainSpriteInst.mDef.mWorkAreaStart, false);
			}
			mMainSpriteInst.mParticleEffectVector = mParticleEffectVector;
			mParticleEffectVector.Clear();
			for (int j = 0; j < mMainSpriteInst.mDef.mObjectDefVector.Count; j++)
			{
				PAObjectInst pAObjectInst2 = mMainSpriteInst.mChildren[j];
				if (pAObjectInst2.mName == null || pAObjectInst2.mName[0] == '\0')
				{
					continue;
				}
				BlendSrcData value = null;
				if (dictionary.TryGetValue(pAObjectInst2.mName, out value))
				{
					pAObjectInst2.mIsBlending = true;
					pAObjectInst2.mBlendSrcColor = value.mColor;
					pAObjectInst2.mBlendSrcTransform = value.mTransform;
					if (pAObjectInst2.mSpriteInst != null)
					{
						if (value.mParticleEffectVector.Count() > 0)
						{
							pAObjectInst2.mSpriteInst.mParticleEffectVector = value.mParticleEffectVector;
							value.mParticleEffectVector.Clear();
						}
					}
					else
					{
						List<PAParticleEffect> mParticleEffectVector2 = value.mParticleEffectVector;
						while (mParticleEffectVector2.Count > 0)
						{
							mParticleEffectVector2[mParticleEffectVector2.Count - 1].mEffect = null;
							mParticleEffectVector2.RemoveAt(mParticleEffectVector2.Count - 1);
						}
					}
					dictionary.Remove(pAObjectInst2.mName);
				}
				else
				{
					pAObjectInst2.mIsBlending = false;
				}
			}
			while (dictionary.Count > 0)
			{
				List<PAParticleEffect> mParticleEffectVector3 = dictionary.First().Value.mParticleEffectVector;
				while (mParticleEffectVector3.Count > 0)
				{
					mParticleEffectVector3[mParticleEffectVector3.Count - 1].mEffect = null;
					mParticleEffectVector3.RemoveAt(mParticleEffectVector3.Count - 1);
				}
				dictionary.Remove(dictionary.First().Key);
			}
			return true;
		}

		public bool IsActive()
		{
			if (mAnimRunning)
			{
				return true;
			}
			if (HasParticles(mMainSpriteInst))
			{
				return true;
			}
			return false;
		}

		public void SetColor(Color theColor)
		{
			mColor = theColor.Clone();
			MarkDirty();
		}

		public PAObjectInst GetObjectInst(string theName)
		{
			SetupSpriteInst();
			return mMainSpriteInst.GetObjectInst(theName);
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			if (mLoaded && SetupSpriteInst())
			{
				if (mTransDirty)
				{
					UpdateTransforms(mMainSpriteInst, null, mColor, false);
					mTransDirty = false;
				}
				if (mMirror)
				{
					DrawSpriteMirrored(g, mMainSpriteInst, null, mColor, mAdditive, false);
				}
				else
				{
					DrawSprite(g, mMainSpriteInst, null, mColor, mAdditive, false);
				}
			}
		}

		public override void Update()
		{
			if (!mLoaded)
			{
				return;
			}
			base.Update();
			if (SetupSpriteInst())
			{
				if (!GlobalMembers.gSexyAppBase.mVSyncUpdates)
				{
					UpdateF(1f);
				}
				UpdateTransforms(mMainSpriteInst, null, mColor, false);
				mTransDirty = false;
				if (!mPaused)
				{
					UpdateParticles(mMainSpriteInst, null);
					CleanParticles(mMainSpriteInst);
				}
			}
		}

		public override void UpdateF(float theFrac)
		{
			if (!mPaused)
			{
				AnimUpdate(theFrac);
			}
		}

		public int GetLabelFrame(string theFrameLabel)
		{
			return mMainAnimDef.mMainSpriteDef.GetLabelFrame(theFrameLabel);
		}

		public PASpriteDef FindSpriteDef(string theAnimName)
		{
			if (mMainAnimDef != null)
			{
				for (int i = 0; i < mMainAnimDef.mSpriteDefVector.Count; i++)
				{
					if (mMainAnimDef.mSpriteDefVector[i].mName == theAnimName)
					{
						return mMainAnimDef.mSpriteDefVector[i];
					}
				}
			}
			return null;
		}
	}
}
