using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class PIEffect : IDisposable
	{
		public SexyFramework.Misc.Buffer mReadBuffer = new SexyFramework.Misc.Buffer();

		public int mFileChecksum;

		public bool mIsPPF;

		public bool mAutoPadImages;

		public bool mInUse = true;

		public int mVersion;

		public string mSrcFileName;

		public string mDestFileName;

		public MTRand mRand = new MTRand();

		public SexyFramework.Misc.Buffer mStartupState = new SexyFramework.Misc.Buffer();

		public static int mNeedUpdate;

		public int mOptimizeValue = 1;

		public float mLastLifePct = -1f;

		public int mBufTemp;

		public int mBufPos;

		public int mChecksumPos;

		public string mNotes;

		public int mFileIdx;

		public List<string> mStringVector = new List<string>();

		public int mWidth;

		public int mHeight;

		public Color mBkgColor = default(Color);

		public int mFramerate;

		public int mFirstFrameNum;

		public int mLastFrameNum;

		public DeviceImage mThumbnail;

		public Dictionary<string, string> mNotesParams = new Dictionary<string, string>();

		public PIEffectDef mDef;

		public List<PILayer> mLayerVector = new List<PILayer>();

		public List<PIParticleInstance> mNormalList;

		public List<PIParticleInstance> mAdditiveList;

		public bool mDarken;

		public List<float> mTimes = new List<float>();

		public List<Vector2> mPoints = new List<Vector2>();

		public List<Vector2> mControlPoints = new List<Vector2>();

		public string mError = "";

		public bool mLoaded;

		public int mUpdateCnt;

		public float mFrameNum;

		public bool mIsNewFrame;

		public ObjectPool<PIParticleInstance> mParticlePool;

		public ObjectPool<PIFreeEmitterInstance> mFreeEmitterPool;

		public int mPoolSize;

		public bool mHasEmitterTransform;

		public bool mHasDrawTransform;

		public bool mDrawTransformSimple;

		public int mCurNumParticles;

		public int mCurNumEmitters;

		public int mLastDrawnPixelCount;

		public float mAnimSpeed;

		public Color mColor = default(Color);

		public bool mDebug;

		public bool mDrawBlockers;

		public bool mEmitAfterTimeline;

		public List<int> mRandSeeds = new List<int>();

		public bool mWantsSRand;

		private SpriteBatch mSpriteBatch;

		public SexyTransform2D mDrawTransform = new SexyTransform2D(false);

		public SexyTransform2D mEmitterTransform = new SexyTransform2D(false);

		public bool Fail(string theError)
		{
			if (mError.Length == 0)
			{
				mError = theError;
			}
			return false;
		}

		public void Deref()
		{
			mDef.mRefCount--;
			if (mDef.mRefCount <= 0)
			{
				if (mDef != null)
				{
					mDef.Dispose();
				}
				mDef = null;
			}
			if (mParticlePool != null)
			{
				mParticlePool.Dispose();
				mParticlePool = null;
			}
			if (mFreeEmitterPool != null)
			{
				mFreeEmitterPool.Dispose();
				mFreeEmitterPool = null;
			}
		}

		public float GetRandFloat()
		{
			return (float)(mRand.Next() % 20000000) / 10000000f - 1f;
		}

		public float GetRandFloatU()
		{
			return (float)(mRand.Next() % 10000000) / 10000000f;
		}

		public float GetRandSign()
		{
			if (mRand.Next() % 2 == 0)
			{
				return 1f;
			}
			return -1f;
		}

		public float GetVariationScalar()
		{
			return GetRandFloat() * GetRandFloat();
		}

		public float GetVariationScalarU()
		{
			return GetRandFloatU() * GetRandFloatU();
		}

		public string ReadString()
		{
			int num = mReadBuffer.ReadByte();
			string text = "";
			for (int i = 0; i < num; i++)
			{
				text += (char)mReadBuffer.ReadByte();
			}
			return text;
		}

		public string ReadStringS()
		{
			int num = mReadBuffer.ReadShort();
			if (num == -1)
			{
				mReadBuffer.ReadShort();
				num = mReadBuffer.ReadShort();
				return "";
			}
			if ((num & 0x8000) != 0)
			{
				string text = mStringVector[num & 0x7FFF];
				mStringVector.Add(text);
				return text;
			}
			string text2 = "";
			for (int i = 0; i < num; i++)
			{
				text2 += (char)mReadBuffer.ReadByte();
			}
			mStringVector.Add(text2);
			mStringVector.Add(text2);
			return text2;
		}

		public bool ExpectCmd(string theCmdExpected)
		{
			if (mIsPPF)
			{
				return true;
			}
			string text = ReadStringS();
			if (text != theCmdExpected)
			{
				return Fail("Expected '" + theCmdExpected + "'");
			}
			return true;
		}

		public void ReadValue2D(PIValue2D theValue2D)
		{
			int num = mReadBuffer.ReadShort();
			List<float> list = mTimes;
			List<Vector2> list2 = mPoints;
			List<Vector2> list3 = mControlPoints;
			bool flag = false;
			if (mIsPPF && num > 1)
			{
				flag = mReadBuffer.ReadBoolean();
			}
			for (int i = 0; i < num; i++)
			{
				ExpectCmd("CKey");
				float num2 = mReadBuffer.ReadInt32();
				list.Add(num2);
				Vector2 vector = default(Vector2);
				vector.X = mReadBuffer.ReadFloat();
				vector.Y = mReadBuffer.ReadFloat();
				list2.Add(vector);
				if (!mIsPPF || flag)
				{
					Vector2 vector2 = default(Vector2);
					vector2.X = mReadBuffer.ReadFloat();
					vector2.Y = mReadBuffer.ReadFloat();
					if (i > 0)
					{
						list3.Add(vector + vector2);
					}
					Vector2 vector3 = default(Vector2);
					vector3.X = mReadBuffer.ReadFloat();
					vector3.Y = mReadBuffer.ReadFloat();
					list3.Add(vector + vector3);
				}
				if (!mIsPPF)
				{
					mReadBuffer.ReadInt32();
					int num3 = mReadBuffer.ReadInt32();
					flag = flag || (num3 & 1) == 0;
				}
				PIValuePoint2D pIValuePoint2D = new PIValuePoint2D();
				pIValuePoint2D.mValue = vector;
				pIValuePoint2D.mTime = num2;
				theValue2D.mValuePoint2DVector.Add(pIValuePoint2D);
			}
			if (num > 1 && flag)
			{
				theValue2D.mBezier.Init(list2.ToArray(), list3.ToArray(), list.ToArray(), num);
			}
			list2.Clear();
			list3.Clear();
			list.Clear();
		}

		public void ReadEPoint(PIValue2D theValue2D)
		{
			int num = mReadBuffer.ReadShort();
			for (int i = 0; i < num; i++)
			{
				ExpectCmd("CPointKey");
				PIValuePoint2D pIValuePoint2D = new PIValuePoint2D();
				pIValuePoint2D.mTime = mReadBuffer.ReadInt32();
				pIValuePoint2D.mValue.X = mReadBuffer.ReadFloat();
				pIValuePoint2D.mValue.Y = mReadBuffer.ReadFloat();
				theValue2D.mValuePoint2DVector.Add(pIValuePoint2D);
			}
		}

		public void ReadValue(ref PIValue theValue)
		{
			List<float> list = mTimes;
			List<Vector2> list2 = mPoints;
			List<Vector2> list3 = mControlPoints;
			int num = (mIsPPF ? mReadBuffer.ReadByte() : 0);
			int num2 = num & 7;
			if (!mIsPPF || num2 == 7)
			{
				num2 = mReadBuffer.ReadShort();
			}
			bool flag = false;
			if (num2 > 1)
			{
				flag = flag || (num & 8) != 0;
			}
			Common.Resize(theValue.mValuePointVector, num2);
			for (int i = 0; i < num2; i++)
			{
				bool flag2 = true;
				string text = "";
				if (!mIsPPF)
				{
					text = ReadStringS();
					flag2 = text == "CDataKey" || text == "CDataOverLifeKey";
				}
				if (flag2)
				{
					float num3 = (((num & 0x10) != 0 && i == 0) ? 0f : ((!(text == "CDataKey")) ? mReadBuffer.ReadFloat() : ((float)mReadBuffer.ReadInt32())));
					list.Add(num3);
					float y = ((i != 0 || (num & 0x60) == 0) ? mReadBuffer.ReadFloat() : (((num & 0x60) == 32) ? 0f : (((num & 0x60) != 64) ? 2f : 1f)));
					Vector2 vector = default(Vector2);
					vector.X = num3;
					vector.Y = y;
					list2.Add(vector);
					if (!mIsPPF || flag)
					{
						Vector2 vector2 = default(Vector2);
						vector2.X = mReadBuffer.ReadFloat();
						vector2.Y = mReadBuffer.ReadFloat();
						if (i > 0)
						{
							list3.Add(vector + vector2);
						}
						Vector2 vector3 = default(Vector2);
						vector3.X = mReadBuffer.ReadFloat();
						vector3.Y = mReadBuffer.ReadFloat();
						list3.Add(vector + vector3);
					}
					if (!mIsPPF)
					{
						mReadBuffer.ReadInt32();
						int num4 = mReadBuffer.ReadInt32();
						flag = flag || (num4 & 1) == 0;
					}
					PIValuePoint pIValuePoint = theValue.mValuePointVector[i];
					pIValuePoint.mValue = vector.Y;
					pIValuePoint.mTime = num3;
				}
				else
				{
					Fail("CDataKey or CDataOverLifeKey expected");
				}
			}
			if (!flag && theValue.mValuePointVector.Count == 2 && theValue.mValuePointVector[0].mValue == theValue.mValuePointVector[1].mValue)
			{
				theValue.mValuePointVector.RemoveAt(theValue.mValuePointVector.Count - 1);
			}
			if (num2 > 1 && flag)
			{
				theValue.mBezier.Init(list2.ToArray(), list3.ToArray(), list.ToArray(), num2);
			}
			list.Clear();
			list2.Clear();
			list3.Clear();
		}

		public void ReadEmitterType(PIEmitter theEmitter)
		{
			mReadBuffer.ReadInt32();
			theEmitter.mName = ReadString();
			theEmitter.mKeepInOrder = mReadBuffer.ReadBoolean();
			mReadBuffer.ReadInt32();
			theEmitter.mOldestInFront = mReadBuffer.ReadBoolean();
			short num = mReadBuffer.ReadShort();
			for (int i = 0; i < num; i++)
			{
				PIParticleDef pIParticleDef = new PIParticleDef();
				ExpectCmd("CEmParticleType");
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadFloat();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				pIParticleDef.mIntense = mReadBuffer.ReadBoolean();
				pIParticleDef.mSingleParticle = mReadBuffer.ReadBoolean();
				pIParticleDef.mPreserveColor = mReadBuffer.ReadBoolean();
				pIParticleDef.mAttachToEmitter = mReadBuffer.ReadBoolean();
				pIParticleDef.mAttachVal = mReadBuffer.ReadFloat();
				pIParticleDef.mFlipHorz = mReadBuffer.ReadBoolean();
				pIParticleDef.mFlipVert = mReadBuffer.ReadBoolean();
				pIParticleDef.mAnimStartOnRandomFrame = mReadBuffer.ReadBoolean();
				pIParticleDef.mRepeatColor = mReadBuffer.ReadInt32();
				pIParticleDef.mRepeatAlpha = mReadBuffer.ReadInt32();
				pIParticleDef.mLinkTransparencyToColor = mReadBuffer.ReadBoolean();
				pIParticleDef.mName = ReadString();
				pIParticleDef.mAngleAlignToMotion = mReadBuffer.ReadBoolean();
				pIParticleDef.mAngleRandomAlign = mReadBuffer.ReadBoolean();
				pIParticleDef.mAngleKeepAlignedToMotion = mReadBuffer.ReadBoolean();
				pIParticleDef.mAngleValue = mReadBuffer.ReadInt32();
				pIParticleDef.mAngleAlignOffset = mReadBuffer.ReadInt32();
				pIParticleDef.mAnimSpeed = mReadBuffer.ReadInt32();
				pIParticleDef.mRandomGradientColor = mReadBuffer.ReadBoolean();
				mReadBuffer.ReadInt32();
				pIParticleDef.mTextureIdx = mReadBuffer.ReadInt32();
				int num2 = mReadBuffer.ReadShort();
				for (int j = 0; j < num2; j++)
				{
					ExpectCmd("CColorPoint");
					byte b = mReadBuffer.ReadByte();
					byte b2 = mReadBuffer.ReadByte();
					byte b3 = mReadBuffer.ReadByte();
					ulong num3 = 0xFF000000u | ((ulong)b << 16) | ((ulong)b2 << 8) | b3;
					float mTime = mReadBuffer.ReadFloat();
					PIInterpolatorPoint pIInterpolatorPoint = new PIInterpolatorPoint();
					pIInterpolatorPoint.mValue = (int)num3;
					pIInterpolatorPoint.mTime = mTime;
					pIParticleDef.mColor.mInterpolatorPointVector.Add(pIInterpolatorPoint);
				}
				int num4 = mReadBuffer.ReadShort();
				for (int k = 0; k < num4; k++)
				{
					ExpectCmd("CAlphaPoint");
					byte mValue = mReadBuffer.ReadByte();
					float mTime2 = mReadBuffer.ReadFloat();
					PIInterpolatorPoint pIInterpolatorPoint2 = new PIInterpolatorPoint();
					pIInterpolatorPoint2.mValue = mValue;
					pIInterpolatorPoint2.mTime = mTime2;
					pIParticleDef.mAlpha.mInterpolatorPointVector.Add(pIInterpolatorPoint2);
				}
				for (int l = 0; l < 23; l++)
				{
					ReadValue(ref pIParticleDef.mValues[l]);
				}
				pIParticleDef.mRefPointOfs.X = mReadBuffer.ReadFloat();
				pIParticleDef.mRefPointOfs.Y = mReadBuffer.ReadFloat();
				if (!mIsPPF)
				{
					Image image = mDef.mTextureVector[pIParticleDef.mTextureIdx].mImageVector[0].GetImage();
					pIParticleDef.mRefPointOfs.X /= image.mWidth;
					pIParticleDef.mRefPointOfs.Y /= image.mHeight;
				}
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				pIParticleDef.mLockAspect = mReadBuffer.ReadBoolean();
				ReadValue(ref pIParticleDef.mValues[25]);
				ReadValue(ref pIParticleDef.mValues[26]);
				ReadValue(ref pIParticleDef.mValues[27]);
				pIParticleDef.mAngleRange = mReadBuffer.ReadInt32();
				pIParticleDef.mAngleOffset = mReadBuffer.ReadInt32();
				pIParticleDef.mGetColorFromLayer = mReadBuffer.ReadBoolean();
				pIParticleDef.mUpdateColorFromLayer = mReadBuffer.ReadBoolean();
				pIParticleDef.mUseEmitterAngleAndRange = mReadBuffer.ReadBoolean();
				ReadValue(ref pIParticleDef.mValues[23]);
				ReadValue(ref pIParticleDef.mValues[24]);
				mReadBuffer.ReadInt32();
				PIValue theValue = new PIValue();
				ReadValue(ref theValue);
				pIParticleDef.mUseKeyColorsOnly = mReadBuffer.ReadBoolean();
				pIParticleDef.mUpdateTransparencyFromLayer = mReadBuffer.ReadBoolean();
				pIParticleDef.mUseNextColorKey = mReadBuffer.ReadBoolean();
				pIParticleDef.mNumberOfEachColor = mReadBuffer.ReadInt32();
				pIParticleDef.mGetTransparencyFromLayer = mReadBuffer.ReadBoolean();
				if (theEmitter.mOldestInFront)
				{
					theEmitter.mParticleDefVector.Insert(0, pIParticleDef);
				}
				else
				{
					theEmitter.mParticleDefVector.Add(pIParticleDef);
				}
			}
			mReadBuffer.ReadInt32();
			for (int m = 0; m < 42; m++)
			{
				ReadValue(ref theEmitter.mValues[m]);
			}
			theEmitter.mIsSuperEmitter = theEmitter.mValues[0].mValuePointVector.Count != 0;
			mReadBuffer.ReadInt32();
			mReadBuffer.ReadInt32();
		}

		public void WriteByte(sbyte theByte)
		{
			throw new NotImplementedException();
		}

		public void WriteInt(int theInt)
		{
			throw new NotImplementedException();
		}

		public void WriteShort(short theShort)
		{
			throw new NotImplementedException();
		}

		public void WriteFloat(float theFloat)
		{
			throw new NotImplementedException();
		}

		public void WriteBool(int theValue)
		{
			throw new NotImplementedException();
		}

		public void WriteString(string theString)
		{
			throw new NotImplementedException();
		}

		public void WriteValue2D(PIValue2D theValue2D)
		{
			throw new NotImplementedException();
		}

		public void WriteValue(PIValue theValue)
		{
			throw new NotImplementedException();
		}

		public void WriteEmitterType(PIEmitter theEmitter)
		{
			throw new NotImplementedException();
		}

		public void SaveParticleDefInstance(SexyFramework.Misc.Buffer theBuffer, PIParticleDefInstance theParticleDefInstance)
		{
			theBuffer.WriteFloat(theParticleDefInstance.mNumberAcc);
			theBuffer.WriteFloat(theParticleDefInstance.mCurNumberVariation);
			theBuffer.WriteLong(theParticleDefInstance.mParticlesEmitted);
			theBuffer.WriteLong(theParticleDefInstance.mTicks);
		}

		public void SaveParticle(SexyFramework.Misc.Buffer theBuffer, PILayer theLayer, PIParticleInstance theParticle)
		{
			theBuffer.WriteFloat(theParticle.mTicks);
			theBuffer.WriteFloat(theParticle.mLife);
			theBuffer.WriteFloat(theParticle.mLifePct);
			theBuffer.WriteFloat(theParticle.mZoom);
			theBuffer.WriteFPoint(theParticle.mPos);
			theBuffer.WriteFPoint(theParticle.mVel);
			theBuffer.WriteFPoint(theParticle.mEmittedPos);
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mAttachToEmitter)
			{
				theBuffer.WriteFPoint(theParticle.mOrigPos);
				theBuffer.WriteFloat(theParticle.mOrigEmitterAng);
			}
			theBuffer.WriteFloat(theParticle.mImgAngle);
			int num = 0;
			for (int i = 0; i < 9; i++)
			{
				if (Math.Abs(theParticle.mVariationValues[i]) >= 1E-05f)
				{
					num |= 1 << i;
				}
			}
			theBuffer.WriteShort((short)num);
			for (int j = 0; j < 9; j++)
			{
				if ((num & (1 << j)) != 0)
				{
					theBuffer.WriteFloat(theParticle.mVariationValues[j]);
				}
			}
			theBuffer.WriteFloat(theParticle.mSrcSizeXMult);
			theBuffer.WriteFloat(theParticle.mSrcSizeYMult);
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mRandomGradientColor)
			{
				theBuffer.WriteFloat(theParticle.mGradientRand);
			}
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mAnimStartOnRandomFrame)
			{
				theBuffer.WriteShort((short)theParticle.mAnimFrameRand);
			}
			if (theLayer.mLayerDef.mDeflectorVector.Count > 0)
			{
				theBuffer.WriteFloat(theParticle.mThicknessHitVariation);
			}
		}

		public void LoadParticleDefInstance(SexyFramework.Misc.Buffer theBuffer, PIParticleDefInstance theParticleDefInstance)
		{
			theParticleDefInstance.mNumberAcc = theBuffer.ReadFloat();
			theParticleDefInstance.mCurNumberVariation = theBuffer.ReadFloat();
			theParticleDefInstance.mParticlesEmitted = (int)theBuffer.ReadLong();
			theParticleDefInstance.mTicks = (int)theBuffer.ReadLong();
		}

		public void LoadParticle(SexyFramework.Misc.Buffer theBuffer, PILayer theLayer, PIParticleInstance theParticle)
		{
			theParticle.mTicks = theBuffer.ReadFloat();
			theParticle.mLife = theBuffer.ReadFloat();
			theParticle.mLifePct = theBuffer.ReadFloat();
			theParticle.mZoom = theBuffer.ReadFloat();
			theParticle.mPos = theBuffer.ReadVector2();
			theParticle.mVel = theBuffer.ReadVector2();
			theParticle.mEmittedPos = theBuffer.ReadVector2();
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mAttachToEmitter)
			{
				theParticle.mOrigPos = theBuffer.ReadVector2();
				theParticle.mOrigEmitterAng = theBuffer.ReadFloat();
			}
			theParticle.mImgAngle = theBuffer.ReadFloat();
			int num = theBuffer.ReadShort();
			for (int i = 0; i < 9; i++)
			{
				if ((num & (1 << i)) != 0)
				{
					theParticle.mVariationValues[i] = theBuffer.ReadFloat();
				}
				else
				{
					theParticle.mVariationValues[i] = 0f;
				}
			}
			theParticle.mSrcSizeXMult = theBuffer.ReadFloat();
			theParticle.mSrcSizeYMult = theBuffer.ReadFloat();
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mRandomGradientColor)
			{
				theParticle.mGradientRand = theBuffer.ReadFloat();
			}
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mAnimStartOnRandomFrame)
			{
				theParticle.mAnimFrameRand = theBuffer.ReadShort();
			}
			if (theLayer.mLayerDef.mDeflectorVector.Count > 0)
			{
				theParticle.mThicknessHitVariation = theBuffer.ReadFloat();
			}
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mAnimStartOnRandomFrame)
			{
				theParticle.mAnimFrameRand = (int)(mRand.Next() & 0x7FFF);
			}
			else
			{
				theParticle.mAnimFrameRand = 0;
			}
		}

		public Vector2 GetGeomPos(PIEmitterInstance theEmitterInstance, PIParticleInstance theParticleInstance, ref float theTravelAngle)
		{
			bool temp = false;
			return GetGeomPos(theEmitterInstance, theParticleInstance, ref theTravelAngle, ref temp);
		}

		public Vector2 GetGeomPos(PIEmitterInstance theEmitterInstance, PIParticleInstance theParticleInstance)
		{
			float temp = 0f;
			bool temp2 = false;
			return GetGeomPos(theEmitterInstance, theParticleInstance, ref temp, ref temp2);
		}

		public Vector2 GetGeomPos(PIEmitterInstance theEmitterInstance, PIParticleInstance theParticleInstance, ref float theTravelAngle, ref bool isMaskedOut)
		{
			Vector2 thePoint = default(Vector2);
			PIEmitterInstanceDef mEmitterInstanceDef = theEmitterInstance.mEmitterInstanceDef;
			switch ((PIEmitterInstanceDef.PIEmitterGEOM)mEmitterInstanceDef.mEmitterGeom)
			{
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_LINE:
			{
				if (mEmitterInstanceDef.mPoints.Count < 2)
				{
					break;
				}
				int num10 = 0;
				float num11 = 0f;
				int num12 = 0;
				for (int i = 0; i < mEmitterInstanceDef.mPoints.Count - 1; i++)
				{
					Vector2 valueAt3 = mEmitterInstanceDef.mPoints[i].GetValueAt(mFrameNum);
					Vector2 valueAt4 = mEmitterInstanceDef.mPoints[i + 1].GetValueAt(mFrameNum);
					Vector2 vector = valueAt4 - valueAt3;
					float num13 = vector.X * vector.X + vector.Y * vector.Y;
					num12 += (int)num13;
				}
				float num15;
				if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					int num14 = theParticleInstance.mNum % mEmitterInstanceDef.mEmitAtPointsNum;
					num15 = (float)(num14 * num12) / (float)(mEmitterInstanceDef.mEmitAtPointsNum - 1);
				}
				else
				{
					num15 = GetRandFloatU() * (float)num12;
				}
				num12 = 0;
				for (int j = 0; j < mEmitterInstanceDef.mPoints.Count - 1; j++)
				{
					Vector2 valueAt5 = mEmitterInstanceDef.mPoints[j].GetValueAt(mFrameNum);
					Vector2 valueAt6 = mEmitterInstanceDef.mPoints[j + 1].GetValueAt(mFrameNum);
					Vector2 vector2 = valueAt6 - valueAt5;
					float num16 = vector2.X * vector2.X + vector2.Y * vector2.Y;
					if (num15 >= (float)num12 && num15 <= (float)num12 + num16)
					{
						num11 = (num15 - (float)num12) / num16;
						num10 = j;
						break;
					}
					num12 += (int)num16;
				}
				Vector2 valueAt7 = mEmitterInstanceDef.mPoints[num10].GetValueAt(mFrameNum);
				Vector2 valueAt8 = mEmitterInstanceDef.mPoints[num10 + 1].GetValueAt(mFrameNum);
				Vector2 vector3 = valueAt8 - valueAt7;
				thePoint = valueAt7 * (1f - num11) + valueAt8 * num11;
				float num17 = ((!mEmitterInstanceDef.mEmitIn) ? 1f : (mEmitterInstanceDef.mEmitOut ? GetRandSign() : (-1f)));
				if (theTravelAngle != 0f)
				{
					float num18 = (float)Math.Atan2(vector3.Y, vector3.X) + GlobalPIEffect.M_PI / 2f + num17 * GlobalPIEffect.M_PI / 2f;
					theTravelAngle += num18;
				}
				break;
			}
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_ECLIPSE:
			{
				float valueAt9 = theEmitterInstance.mEmitterInstanceDef.mValues[15].GetValueAt(mFrameNum);
				float valueAt10 = theEmitterInstance.mEmitterInstanceDef.mValues[16].GetValueAt(mFrameNum);
				float num20;
				if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					int num19 = theParticleInstance.mNum % mEmitterInstanceDef.mEmitAtPointsNum;
					num20 = (float)num19 * GlobalPIEffect.M_PI * 2f / (float)mEmitterInstanceDef.mEmitAtPointsNum;
					if (num20 > GlobalPIEffect.M_PI)
					{
						num20 -= GlobalPIEffect.M_PI * 2f;
					}
				}
				else
				{
					num20 = GetRandFloat() * GlobalPIEffect.M_PI;
				}
				if (valueAt9 > valueAt10)
				{
					float num21 = 1f + (valueAt9 / valueAt10 - 1f) * 0.3f;
					num20 = ((num20 < (0f - GlobalPIEffect.M_PI) / 2f) ? ((float)((double)GlobalPIEffect.M_PI + Math.Pow((num20 + GlobalPIEffect.M_PI) / (GlobalPIEffect.M_PI / 2f), num21) * (double)GlobalPIEffect.M_PI / 2.0)) : ((num20 < 0f) ? ((float)((0.0 - Math.Pow((0f - num20) / (GlobalPIEffect.M_PI / 2f), num21)) * (double)GlobalPIEffect.M_PI / 2.0)) : ((!(num20 < GlobalPIEffect.M_PI / 2f)) ? ((float)((double)GlobalPIEffect.M_PI - Math.Pow((GlobalPIEffect.M_PI - num20) / (GlobalPIEffect.M_PI / 2f), num21) * (double)GlobalPIEffect.M_PI / 2.0)) : ((float)(Math.Pow(num20 / (GlobalPIEffect.M_PI / 2f), num21) * (double)GlobalPIEffect.M_PI / 2.0)))));
				}
				else if (valueAt10 > valueAt9)
				{
					float num22 = 1f + (valueAt10 / valueAt9 - 1f) * 0.3f;
					num20 = ((num20 < (0f - GlobalPIEffect.M_PI) / 2f) ? ((float)((double)((0f - GlobalPIEffect.M_PI) / 2f) - Math.Pow(((0f - GlobalPIEffect.M_PI) / 2f - num20) / (GlobalPIEffect.M_PI / 2f), num22) * (double)GlobalPIEffect.M_PI / 2.0)) : ((num20 < 0f) ? ((float)((double)((0f - GlobalPIEffect.M_PI) / 2f) + Math.Pow((num20 + GlobalPIEffect.M_PI / 2f) / (GlobalPIEffect.M_PI / 2f), num22) * (double)GlobalPIEffect.M_PI / 2.0)) : ((!(num20 < GlobalPIEffect.M_PI / 2f)) ? ((float)((double)(GlobalPIEffect.M_PI / 2f) + Math.Pow((num20 - GlobalPIEffect.M_PI / 2f) / (GlobalPIEffect.M_PI / 2f), num22) * (double)GlobalPIEffect.M_PI / 2.0)) : ((float)((double)(GlobalPIEffect.M_PI / 2f) - Math.Pow((GlobalPIEffect.M_PI / 2f - num20) / (GlobalPIEffect.M_PI / 2f), num22) * (double)GlobalPIEffect.M_PI / 2.0)))));
				}
				thePoint = new Vector2((float)(Math.Cos(num20) * (double)valueAt9), (float)(Math.Sin(num20) * (double)valueAt10));
				if (theTravelAngle != 0f)
				{
					float num23 = ((!mEmitterInstanceDef.mEmitIn) ? 1f : (mEmitterInstanceDef.mEmitOut ? GetRandSign() : (-1f)));
					float num24 = num20 + num23 * GlobalPIEffect.M_PI / 2f;
					theTravelAngle += num24;
				}
				break;
			}
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_CIRCLE:
			{
				float valueAt11 = theEmitterInstance.mEmitterInstanceDef.mValues[15].GetValueAt(mFrameNum);
				float num26;
				if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					int num25 = theParticleInstance.mNum % mEmitterInstanceDef.mEmitAtPointsNum;
					num26 = (float)num25 * GlobalPIEffect.M_PI * 2f / (float)mEmitterInstanceDef.mEmitAtPointsNum;
				}
				else
				{
					num26 = GetRandFloat() * GlobalPIEffect.M_PI;
				}
				thePoint = new Vector2((float)Math.Cos(num26) * valueAt11, (float)Math.Sin(num26) * valueAt11);
				if (theTravelAngle != 0f)
				{
					float num27 = ((!mEmitterInstanceDef.mEmitIn) ? 1f : (mEmitterInstanceDef.mEmitOut ? GetRandSign() : (-1f)));
					float num28 = num26 + num27 * GlobalPIEffect.M_PI / 2f;
					theTravelAngle += num28;
				}
				break;
			}
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_AREA:
			{
				float valueAt = theEmitterInstance.mEmitterInstanceDef.mValues[15].GetValueAt(mFrameNum);
				float valueAt2 = theEmitterInstance.mEmitterInstanceDef.mValues[16].GetValueAt(mFrameNum);
				if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					float num = theParticleInstance.mNum % mEmitterInstanceDef.mEmitAtPointsNum;
					float num2 = theParticleInstance.mNum / mEmitterInstanceDef.mEmitAtPointsNum % mEmitterInstanceDef.mEmitAtPointsNum2;
					if (mEmitterInstanceDef.mEmitAtPointsNum > 1)
					{
						thePoint.X = (float)((double)(num / (float)(mEmitterInstanceDef.mEmitAtPointsNum - 1)) - 0.5) * valueAt;
					}
					if (mEmitterInstanceDef.mEmitAtPointsNum2 > 1)
					{
						thePoint.Y = (float)((double)(num2 / (float)(mEmitterInstanceDef.mEmitAtPointsNum2 - 1)) - 0.5) * valueAt2;
					}
				}
				else
				{
					thePoint = new Vector2(GetRandFloat() * valueAt / 2f, GetRandFloat() * valueAt2 / 2f);
				}
				if (theEmitterInstance.mMaskImage.GetDeviceImage() != null && isMaskedOut)
				{
					float num3 = thePoint.X / valueAt + 0.5f;
					float num4 = thePoint.Y / valueAt2 + 0.5f;
					int num5 = theEmitterInstance.mMaskImage.mWidth;
					int num6 = theEmitterInstance.mMaskImage.mHeight;
					int num7 = Math.Min((int)(num3 * (float)num5), num5 - 1);
					int num8 = Math.Min((int)(num4 * (float)num6), num6 - 1);
					uint[] bits = theEmitterInstance.mMaskImage.GetDeviceImage().GetBits();
					uint num9 = bits[num7 + num8 * num5];
					if (((num9 & 0x80000000u) == 0) ^ mEmitterInstanceDef.mInvertMask)
					{
						isMaskedOut = true;
					}
				}
				break;
			}
			}

			thePoint += GetEmitterPos(theEmitterInstance, false);
			thePoint += theEmitterInstance.mOffset;
			thePoint = GlobalPIEffect.TransformFPoint(theEmitterInstance.mTransform, thePoint);
			return GlobalPIEffect.TransformFPoint(mEmitterTransform, thePoint);
		}

		public Vector2 GetEmitterPos(PIEmitterInstance theEmitterInstance, bool doTransform)
		{
			Vector2 vector = theEmitterInstance.mEmitterInstanceDef.mPosition.GetValueAt(mFrameNum);
			if (doTransform)
			{
				vector = GlobalPIEffect.TransformFPoint(theEmitterInstance.mTransform, vector);
				vector = GlobalPIEffect.TransformFPoint(mEmitterTransform, vector);
				vector += theEmitterInstance.mOffset;
			}
			return vector;
		}

		public int CountParticles(PIParticleInstance theStart)
		{
			int num = 0;
			while (theStart != null)
			{
				num++;
				theStart = theStart.mNext;
			}
			return num;
		}

		public void CalcParticleTransform(PILayer theLayer, PIEmitterInstance theEmitterInstance, PIEmitter theEmitter, PIParticleDef theParticleDef, PIParticleGroup theParticleGroup, PIParticleInstance theParticleInstance)
		{
			float mLifePct = theParticleInstance.mLifePct;
			float num = 1f;
			float num2 = 1f;
			float num3 = 1f;
			float num4 = 1f;
			Rect rect = Rect.ZERO_RECT;
			if (theParticleDef != null)
			{
				PITexture pITexture = mDef.mTextureVector[theParticleDef.mTextureIdx];
				if (pITexture.mImageVector.Count != 0)
				{
					DeviceImage deviceImage = pITexture.mImageVector[theParticleInstance.mImgIdx].GetDeviceImage();
					rect = new Rect(0, 0, deviceImage.mWidth, deviceImage.mHeight);
				}
				else
				{
					DeviceImage deviceImage = pITexture.mImageStrip.GetDeviceImage();
					if (deviceImage == null)
					{
						pITexture.mImageStrip = GetImage(pITexture.mName, pITexture.mFileName);
						deviceImage = pITexture.mImageStrip.GetDeviceImage();
					}
					rect = deviceImage.GetCelRect(theParticleInstance.mImgIdx);
					if (pITexture.mPadded)
					{
						rect.mX++;
						rect.mWidth -= 2;
						rect.mY++;
						rect.mHeight -= 2;
					}
				}
				if (theParticleDef.mSingleParticle)
				{
					theParticleInstance.mSrcSizeXMult = (theParticleGroup.mWasEmitted ? theEmitter.mValues[10].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[2].GetValueAt(mFrameNum)) * (theParticleDef.mValues[2].GetValueAt(mFrameNum) + theParticleInstance.mVariationValues[1]);
					theParticleInstance.mSrcSizeYMult = (theParticleGroup.mWasEmitted ? theEmitter.mValues[11].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[2].GetValueAt(mFrameNum)) * (theParticleDef.mValues[2].GetValueAt(mFrameNum) + theParticleInstance.mVariationValues[1]);
				}
				float num5 = Math.Max(theParticleDef.mValues[16].GetValueAt(mLifePct) * theParticleInstance.mSrcSizeXMult, 0.1f);
				float num6 = Math.Max(theParticleDef.mValues[27].GetValueAt(mLifePct) * theParticleInstance.mSrcSizeYMult, 0.1f);
				int num7 = Math.Max(rect.mWidth, rect.mHeight);
				num3 = (float)num7 / (float)rect.mWidth;
				num4 = (float)num7 / (float)rect.mHeight;
				num = num5 / (float)num7 * 2f;
				num2 = num6 / (float)num7 * 2f;
			}
			else
			{
				num = 1f;
				num2 = 1f;
			}
			SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
			float valueAt = theEmitterInstance.mEmitterInstanceDef.mValues[14].GetValueAt(mFrameNum);
			if (valueAt != 0f)
			{
				sexyTransform2D.RotateDeg(valueAt);
			}
			if (theParticleInstance.mParentFreeEmitter != null && theParticleInstance.mParentFreeEmitter.mImgAngle != 0f)
			{
				sexyTransform2D.RotateRad(0f - theParticleInstance.mParentFreeEmitter.mImgAngle);
			}
			SexyTransform2D mTransform = new SexyTransform2D(false);
			float num8 = 1f;
			if (theParticleDef != null)
			{
				mTransform.Translate((0f - theParticleDef.mRefPointOfs.X) * num3 * (float)rect.mWidth, (0f - theParticleDef.mRefPointOfs.Y) * num4 * (float)rect.mHeight);
				if (theParticleDef.mFlipHorz)
				{
					mTransform.Scale(-1f, 1f);
				}
				if (theParticleDef.mFlipVert)
				{
					mTransform.Scale(1f, -1f);
				}
			}
			float num9 = 0f;
			num8 *= num * num2;
			if (num != 1f || num2 != 1f)
			{
				mTransform.Scale(num, num2);
			}
			if (theParticleInstance.mImgAngle != 0f)
			{
				num9 += theParticleInstance.mImgAngle;
			}
			if (theParticleDef != null && theParticleDef.mAttachToEmitter)
			{
				float num10 = 0f;
				num10 = ((theParticleInstance.mParentFreeEmitter == null) ? (MathHelper.ToRadians(theEmitterInstance.mEmitterInstanceDef.mValues[14].GetValueAt(mFrameNum)) * theParticleDef.mAttachVal) : ((theParticleInstance.mParentFreeEmitter.mImgAngle - theParticleInstance.mOrigEmitterAng) * theParticleDef.mAttachVal));
				if (num10 != 0f)
				{
					num9 += num10;
				}
			}
			if (theParticleDef != null && theParticleDef.mSingleParticle && (!theParticleDef.mAngleKeepAlignedToMotion || theParticleDef.mAttachToEmitter))
			{
				num9 += MathHelper.ToRadians(theEmitterInstance.mEmitterInstanceDef.mValues[14].GetValueAt(mFrameNum));
			}
			mTransform.RotateRad(num9);
			Vector2 vector = theParticleInstance.mPos;
			if (theParticleDef != null && theParticleDef.mAttachToEmitter)
			{
				SexyTransform2D sexyTransform2D2 = new SexyTransform2D(false);
				sexyTransform2D2.RotateRad(theParticleInstance.mOrigEmitterAng);
				Vector2 vector2 = sexyTransform2D2 * vector;
				Vector2 vector3 = sexyTransform2D * vector2;
				vector = vector * (1f - theParticleDef.mAttachVal) + vector3 * theParticleDef.mAttachVal;
			}
			mTransform.Translate(vector.X, vector.Y);
			if (theParticleDef != null && theParticleDef.mSingleParticle)
			{
				theParticleInstance.mZoom = (theParticleGroup.mWasEmitted ? theEmitter.mValues[17].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[8].GetValueAt(mFrameNum)) * theEmitter.mValues[17].GetValueAt(mFrameNum, 1f);
			}
			num8 *= theParticleInstance.mZoom * theParticleInstance.mZoom;
			if (theParticleInstance.mZoom != 1f)
			{
				mTransform.Scale(theParticleInstance.mZoom, theParticleInstance.mZoom);
			}
			Vector2 vector4 = theParticleInstance.mEmittedPos;
			if (theParticleDef != null && theParticleDef.mSingleParticle)
			{
				Vector2 vector5 = sexyTransform2D * theParticleInstance.mOrigPos;
				vector5 += GetEmitterPos(theEmitterInstance, !theParticleGroup.mWasEmitted);
				vector4 = vector5;
			}
			else if (theParticleDef != null && theParticleDef.mAttachToEmitter && !theParticleGroup.mIsSuperEmitter)
			{
				Vector2 vector6;
				if (theParticleInstance.mParentFreeEmitter != null)
				{
					vector6 = theParticleInstance.mParentFreeEmitter.mLastEmitterPos + theParticleInstance.mParentFreeEmitter.mOrigPos + theParticleInstance.mParentFreeEmitter.mPos;
				}
				else
				{
					vector6 = GlobalPIEffect.TransformFPoint(sexyTransform2D, theParticleInstance.mOrigPos);
					vector6 += GetEmitterPos(theEmitterInstance, !theParticleGroup.mWasEmitted);
				}
				vector4 = vector4 * (1f - theParticleDef.mAttachVal) + vector6 * theParticleDef.mAttachVal;
			}
			theParticleInstance.mLastEmitterPos = vector4;
			mTransform.Translate(vector4.X, vector4.Y);
			Vector2 vector7 = theLayer.mLayerDef.mOffset.GetValueAt(mFrameNum) - theLayer.mLayerDef.mOrigOffset;
			mTransform.Translate(vector7.X, vector7.Y);
			float valueAt2 = theLayer.mLayerDef.mAngle.GetValueAt(mFrameNum);
			if (valueAt2 != 0f)
			{
				mTransform.RotateDeg(valueAt2);
			}
			theParticleInstance.mTransform = mTransform;
			theParticleInstance.mTransformScaleFactor = num8;
		}

		public void UpdateParticleDef(PILayer theLayer, PIEmitter theEmitter, PIEmitterInstance theEmitterInstance, PIParticleDef theParticleDef, PIParticleDefInstance theParticleDefInstance, PIParticleGroup theParticleGroup, PIFreeEmitterInstance theFreeEmitter)
		{
			PIEmitterInstanceDef mEmitterInstanceDef = theEmitterInstance.mEmitterInstanceDef;
			float num = 100f / mAnimSpeed;
			float num2 = 0f;
			if (theFreeEmitter != null)
			{
				num2 = theFreeEmitter.mLifePct;
			}
			if (theParticleDefInstance.mTicks % 25 == 0 && !theParticleGroup.mIsSuperEmitter)
			{
				if (theParticleDefInstance.mTicks == 0)
				{
					theParticleDefInstance.mCurNumberVariation = GetRandFloat() * 0.5f * theParticleDef.mValues[9].GetValueAt(mFrameNum) / 2f;
				}
				else
				{
					theParticleDefInstance.mCurNumberVariation = GetRandFloat() * 0.75f * theParticleDef.mValues[9].GetValueAt(mFrameNum) / 2f;
				}
			}
			theParticleDefInstance.mTicks++;
			float num3 = 0f;
			if (theParticleGroup.mIsSuperEmitter)
			{
				num3 = theEmitter.mValues[1].GetValueAt(mFrameNum) * (theParticleGroup.mWasEmitted ? theEmitter.mValues[9].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[1].GetValueAt(mFrameNum));
			}
			else
			{
				num3 = (theParticleGroup.mWasEmitted ? theEmitter.mValues[9].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[1].GetValueAt(mFrameNum)) * (theParticleDef.mValues[1].GetValueAt(mFrameNum) + theParticleDefInstance.mCurNumberVariation) * theEmitter.mValues[33].GetValueAt(num2, 1f);
				num3 = Math.Max(0f, num3);
				if (theParticleGroup.mWasEmitted && num2 >= 1f)
				{
					num3 = 0f;
				}
			}
			num3 *= theEmitterInstance.mNumberScale;
			if (theParticleGroup.mIsSuperEmitter)
			{
				num3 *= 30f;
			}
			else if (!theParticleGroup.mWasEmitted)
			{
				switch ((PIEmitterInstanceDef.PIEmitterGEOM)mEmitterInstanceDef.mEmitterGeom)
				{
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_LINE:
				{
					if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						num3 *= (float)mEmitterInstanceDef.mEmitAtPointsNum;
						break;
					}
					int num5 = 0;
					for (int i = 0; i < mEmitterInstanceDef.mPoints.Count - 1; i++)
					{
						Vector2 valueAt5 = mEmitterInstanceDef.mPoints[i].GetValueAt(mFrameNum);
						Vector2 valueAt6 = mEmitterInstanceDef.mPoints[i + 1].GetValueAt(mFrameNum);
						Vector2 vector = valueAt6 - valueAt5;
						float num6 = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
						num5 += (int)num6;
					}
					num3 *= (float)num5 / 35f;
					break;
				}
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_ECLIPSE:
				{
					float valueAt3 = theEmitterInstance.mEmitterInstanceDef.mValues[15].GetValueAt(mFrameNum);
					float valueAt4 = theEmitterInstance.mEmitterInstanceDef.mValues[16].GetValueAt(mFrameNum);
					if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						num3 *= (float)mEmitterInstanceDef.mEmitAtPointsNum;
						break;
					}
					float num4 = 6.28318f * (float)Math.Sqrt((valueAt3 * valueAt3 + valueAt4 * valueAt4) / 2f);
					num3 *= num4 / 35f;
					break;
				}
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_CIRCLE:
				{
					float valueAt7 = theEmitterInstance.mEmitterInstanceDef.mValues[15].GetValueAt(mFrameNum);
					if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						num3 *= (float)mEmitterInstanceDef.mEmitAtPointsNum;
						break;
					}
					float num7 = 6.28318f * (float)Math.Sqrt(valueAt7 * valueAt7);
					num3 *= num7 / 35f;
					break;
				}
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_AREA:
				{
					if (mEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						num3 *= (float)(mEmitterInstanceDef.mEmitAtPointsNum * mEmitterInstanceDef.mEmitAtPointsNum2);
						break;
					}
					float valueAt = theEmitterInstance.mEmitterInstanceDef.mValues[15].GetValueAt(mFrameNum);
					float valueAt2 = theEmitterInstance.mEmitterInstanceDef.mValues[16].GetValueAt(mFrameNum);
					num3 *= 1f + valueAt * valueAt2 / 900f / 4f;
					break;
				}
				}
			}

			theParticleDefInstance.mNumberAcc += num3 / num;// * 0.16f * 5f;
			if ((!mEmitterInstanceDef.mIsSuperEmitter && !theEmitterInstance.mWasActive) || !theEmitterInstance.mWithinLifeFrame)
			{
				theParticleDefInstance.mNumberAcc = 0f;
			}
			bool flag = true;
			if (!theParticleGroup.mIsSuperEmitter && theParticleDef.mSingleParticle)
			{
				int num8 = ((mEmitterInstanceDef.mEmitterGeom == 1 || mEmitterInstanceDef.mEmitterGeom == 4) ? mEmitterInstanceDef.mEmitAtPointsNum : ((mEmitterInstanceDef.mEmitterGeom != 3) ? 1 : (mEmitterInstanceDef.mEmitAtPointsNum * mEmitterInstanceDef.mEmitAtPointsNum2)));
				if (num8 == 0)
				{
					flag = false;
					num8 = 1;
				}
				int num9 = 0;
				for (PIParticleInstance pIParticleInstance = theParticleGroup.mHead; pIParticleInstance != null; pIParticleInstance = pIParticleInstance.mNext)
				{
					if (pIParticleInstance.mParticleDef == theParticleDef)
					{
						num9++;
					}
				}
				theParticleDefInstance.mNumberAcc = num8 - num9;
			}
			while (theParticleDefInstance.mNumberAcc >= 1f)
			{
				theParticleDefInstance.mNumberAcc -= 1f;
				PIParticleInstance pIParticleInstance2 = null;
				if (theParticleGroup.mIsSuperEmitter)
				{
					PIFreeEmitterInstance pIFreeEmitterInstance = mFreeEmitterPool.Alloc();
					pIFreeEmitterInstance.Reset();
					Common.Resize(pIFreeEmitterInstance.mEmitter.mParticleDefInstanceVector, theEmitter.mParticleDefVector.Count);
					pIParticleInstance2 = pIFreeEmitterInstance;
				}
				else
				{
					pIParticleInstance2 = mParticlePool.Alloc();
					pIParticleInstance2.Reset();
				}
				pIParticleInstance2.mParticleDef = theParticleDef;
				pIParticleInstance2.mEmitterSrc = theEmitter;
				pIParticleInstance2.mParentFreeEmitter = theFreeEmitter;
				pIParticleInstance2.mNum = theParticleDefInstance.mParticlesEmitted++;
				float num10 = 0f;
				num10 = (theParticleGroup.mIsSuperEmitter ? ((theParticleGroup.mWasEmitted ? theEmitter.mValues[21].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[11].GetValueAt(mFrameNum)) + (theParticleGroup.mWasEmitted ? theEmitter.mValues[22].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[12].GetValueAt(mFrameNum)) * GetRandFloat() / 2f) : ((!theParticleDef.mUseEmitterAngleAndRange) ? (theParticleDef.mValues[23].GetValueAt(mFrameNum) + theParticleDef.mValues[24].GetValueAt(mFrameNum) * GetRandFloat() / 2f) : ((!theParticleGroup.mWasEmitted) ? ((theParticleGroup.mWasEmitted ? theEmitter.mValues[21].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[11].GetValueAt(mFrameNum)) + (theParticleGroup.mWasEmitted ? theEmitter.mValues[22].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[12].GetValueAt(mFrameNum)) * GetRandFloat() / 2f) : (theEmitter.mValues[21].GetValueAt(mFrameNum) + theEmitter.mValues[22].GetValueAt(mFrameNum) * GetRandFloat() / 2f))));
				num10 = MathHelper.ToRadians(0f - num10);
				float num11 = 0f;
				num11 = theFreeEmitter?.mImgAngle ?? MathHelper.ToRadians(0f - theEmitterInstance.mEmitterInstanceDef.mValues[14].GetValueAt(mFrameNum));
				num10 += num11;
				pIParticleInstance2.mOrigEmitterAng = num11;
				if (theParticleDef != null && theParticleDef.mAnimStartOnRandomFrame)
				{
					pIParticleInstance2.mAnimFrameRand = (int)(mRand.Next() & 0x7FFF);
				}
				else
				{
					pIParticleInstance2.mAnimFrameRand = 0;
				}
				pIParticleInstance2.mZoom = (theParticleGroup.mWasEmitted ? theEmitter.mValues[17].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[8].GetValueAt(mFrameNum)) * theEmitter.mValues[17].GetValueAt(mFrameNum, 1f);
				if (!theParticleGroup.mIsSuperEmitter)
				{
					pIParticleInstance2.mVariationValues[0] = GetVariationScalar() * theParticleDef.mValues[8].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[1] = GetVariationScalar() * theParticleDef.mValues[10].GetValueAt(mFrameNum);
					if (theParticleDef == null || theParticleDef.mLockAspect)
					{
						pIParticleInstance2.mVariationValues[2] = pIParticleInstance2.mVariationValues[1];
					}
					else
					{
						pIParticleInstance2.mVariationValues[2] = GetVariationScalar() * theParticleDef.mValues[26].GetValueAt(mFrameNum);
					}
					pIParticleInstance2.mVariationValues[3] = GetVariationScalar() * theParticleDef.mValues[11].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[4] = GetVariationScalar() * theParticleDef.mValues[12].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[5] = GetVariationScalar() * theParticleDef.mValues[13].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[6] = GetVariationScalar() * theParticleDef.mValues[14].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[7] = GetVariationScalar() * theParticleDef.mValues[15].GetValueAt(mFrameNum);
					pIParticleInstance2.mSrcSizeXMult = (theParticleGroup.mWasEmitted ? theEmitter.mValues[10].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[2].GetValueAt(mFrameNum)) * (theParticleDef.mValues[2].GetValueAt(mFrameNum) + pIParticleInstance2.mVariationValues[1]);
					pIParticleInstance2.mSrcSizeYMult = (theParticleGroup.mWasEmitted ? theEmitter.mValues[11].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[17].GetValueAt(mFrameNum)) * (theParticleDef.mValues[25].GetValueAt(mFrameNum) + pIParticleInstance2.mVariationValues[2]);
					if (theParticleGroup.mWasEmitted)
					{
						pIParticleInstance2.mSrcSizeXMult *= (1f + theFreeEmitter.mVariationValues[1]) * theEmitter.mValues[34].GetValueAt(num2, 1f);
						pIParticleInstance2.mSrcSizeYMult *= (1f + theFreeEmitter.mVariationValues[2]) * theEmitter.mValues[34].GetValueAt(num2, 1f);
						pIParticleInstance2.mZoom *= (1f + theFreeEmitter.mVariationValues[8]) * theEmitter.mValues[41].GetValueAt(num2, 1f);
					}
				}
				else
				{
					pIParticleInstance2.mVariationValues[0] = GetVariationScalar() * theEmitter.mValues[23].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[1] = GetRandFloat() * theEmitter.mValues[25].GetValueAt(mFrameNum);
					if (theParticleDef == null || theParticleDef.mLockAspect)
					{
						pIParticleInstance2.mVariationValues[2] = pIParticleInstance2.mVariationValues[1];
					}
					else
					{
						pIParticleInstance2.mVariationValues[2] = GetRandFloat() * theEmitter.mValues[26].GetValueAt(mFrameNum);
					}
					pIParticleInstance2.mVariationValues[3] = GetVariationScalar() * theEmitter.mValues[27].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[4] = GetVariationScalar() * theEmitter.mValues[28].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[5] = GetVariationScalar() * theEmitter.mValues[29].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[6] = GetVariationScalar() * theEmitter.mValues[30].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[7] = GetVariationScalar() * theEmitter.mValues[31].GetValueAt(mFrameNum);
					pIParticleInstance2.mVariationValues[8] = GetVariationScalar() * theEmitter.mValues[32].GetValueAt(mFrameNum);
				}
				float num12 = num10;
				pIParticleInstance2.mGradientRand = GetRandFloatU();
				pIParticleInstance2.mTicks = 0f;
				pIParticleInstance2.mThicknessHitVariation = GetRandFloat();
				pIParticleInstance2.mImgAngle = 0f;
				if (theParticleGroup.mIsSuperEmitter)
				{
					pIParticleInstance2.mLife = (theEmitter.mValues[0].GetValueAt(mFrameNum) + pIParticleInstance2.mVariationValues[0]) * 5f * (theParticleGroup.mWasEmitted ? theEmitter.mValues[8].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[0].GetValueAt(mFrameNum));
				}
				else
				{
					pIParticleInstance2.mLife = (theParticleGroup.mWasEmitted ? theEmitter.mValues[8].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[0].GetValueAt(mFrameNum)) * (theParticleDef.mValues[0].GetValueAt(mFrameNum) + pIParticleInstance2.mVariationValues[0]);
				}
				Vector2 vector2 = default(Vector2);
				if (theParticleGroup.mWasEmitted)
				{
					pIParticleInstance2.mEmittedPos = theFreeEmitter.mLastEmitterPos + theFreeEmitter.mPos;
					pIParticleInstance2.mLastEmitterPos = pIParticleInstance2.mEmittedPos;
				}
				else
				{
					pIParticleInstance2.mEmittedPos = GetEmitterPos(theEmitterInstance, true);
					pIParticleInstance2.mLastEmitterPos = pIParticleInstance2.mEmittedPos;
					bool flag2 = false;
					if (flag)
					{
						vector2 = GetGeomPos(theEmitterInstance, pIParticleInstance2, ref num12, ref flag2) - pIParticleInstance2.mEmittedPos;
					}
					if (flag2)
					{
						continue;
					}
				}
				pIParticleInstance2.mVel = new Vector2((float)Math.Cos(num12), (float)Math.Sin(num12));
				if (theParticleGroup.mIsSuperEmitter)
				{
					pIParticleInstance2.mVel = pIParticleInstance2.mVel * ((theParticleGroup.mWasEmitted ? theEmitter.mValues[12].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[3].GetValueAt(mFrameNum)) * (theEmitter.mValues[2].GetValueAt(mFrameNum) + pIParticleInstance2.mVariationValues[3])) * 160f;
				}
				else
				{
					pIParticleInstance2.mVel *= (theParticleGroup.mWasEmitted ? theEmitter.mValues[12].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[3].GetValueAt(mFrameNum)) * (theParticleDef.mValues[3].GetValueAt(mFrameNum) + pIParticleInstance2.mVariationValues[3]);
				}
				if (!theParticleGroup.mIsSuperEmitter)
				{
					if (theParticleDef.mAngleAlignToMotion)
					{
						if (pIParticleInstance2.mVel.Length() == 0f)
						{
							num12 = 0f;
							if (Math.Cos(num12) > 0.0)
							{
								pIParticleInstance2.mImgAngle = 0f;
							}
							else
							{
								pIParticleInstance2.mImgAngle = GlobalPIEffect.M_PI;
							}
							if (theParticleDef.mSingleParticle && theParticleDef.mAngleKeepAlignedToMotion && !theParticleDef.mAttachToEmitter)
							{
								pIParticleInstance2.mImgAngle += MathHelper.ToRadians(theEmitterInstance.mEmitterInstanceDef.mValues[14].GetValueAt(mFrameNum));
							}
						}
						else
						{
							pIParticleInstance2.mImgAngle = 0f - num12;
						}
						pIParticleInstance2.mImgAngle += MathHelper.ToRadians(-theParticleDef.mAngleAlignOffset);
					}
					else if (theParticleDef.mAngleRandomAlign)
					{
						pIParticleInstance2.mImgAngle = MathHelper.ToRadians(0f - ((float)theParticleDef.mAngleOffset + GetRandFloat() * (float)theParticleDef.mAngleRange / 2f));
					}
					else
					{
						pIParticleInstance2.mImgAngle = MathHelper.ToRadians(-theParticleDef.mAngleValue);
					}
				}
				pIParticleInstance2.mOrigPos = vector2;
				SexyTransform2D theMatrix = new SexyTransform2D(false);
				theMatrix.RotateDeg(theEmitterInstance.mEmitterInstanceDef.mValues[14].GetValueAt(mFrameNum));
				pIParticleInstance2.mEmittedPos += GlobalPIEffect.TransformFPoint(theMatrix, vector2);
				if (theEmitter.mOldestInFront)
				{
					if (theParticleGroup.mHead != null)
					{
						theParticleGroup.mHead.mPrev = pIParticleInstance2;
					}
					pIParticleInstance2.mNext = theParticleGroup.mHead;
					if (theParticleGroup.mTail == null)
					{
						theParticleGroup.mTail = pIParticleInstance2;
					}
					theParticleGroup.mHead = pIParticleInstance2;
				}
				else
				{
					if (theParticleGroup.mTail != null)
					{
						theParticleGroup.mTail.mNext = pIParticleInstance2;
					}
					pIParticleInstance2.mPrev = theParticleGroup.mTail;
					if (theParticleGroup.mHead == null)
					{
						theParticleGroup.mHead = pIParticleInstance2;
					}
					theParticleGroup.mTail = pIParticleInstance2;
				}
				theParticleGroup.mCount++;
			}
		}

		public void UpdateParticleGroup(PILayer theLayer, PIEmitterInstance theEmitterInstance, PIParticleGroup theParticleGroup)
		{
			float num = 100f / mAnimSpeed;
			PIParticleInstance pIParticleInstance = theParticleGroup.mHead;
			PILayerDef mLayerDef = theLayer.mLayerDef;
			PIEmitterInstanceDef mEmitterInstanceDef = theEmitterInstance.mEmitterInstanceDef;
			while (pIParticleInstance != null)
			{
				PIParticleInstance mNext = pIParticleInstance.mNext;
				PIEmitter mEmitterSrc = pIParticleInstance.mEmitterSrc;
				PIParticleDef mParticleDef = pIParticleInstance.mParticleDef;
				if (pIParticleInstance.mParentFreeEmitter != null)
				{
					float mLifePct = pIParticleInstance.mParentFreeEmitter.mLifePct;
				}
				bool flag = pIParticleInstance.mTicks == 0f;
				pIParticleInstance.mTicks += 1f / num;
				float num2 = 0f;
				if (mParticleDef != null && mParticleDef.mSingleParticle)
				{
					float nextKeyframeTime = theEmitterInstance.mEmitterInstanceDef.mValues[13].GetNextKeyframeTime(mFrameNum);
					int nextKeyframeIdx = theEmitterInstance.mEmitterInstanceDef.mValues[13].GetNextKeyframeIdx(mFrameNum);
					num2 = ((!(nextKeyframeTime >= mFrameNum) || nextKeyframeIdx != 1) ? 0.02f : Math.Min(1f, (mFrameNum + (float)mEmitterInstanceDef.mFramesToPreload) / Math.Max(1f, nextKeyframeTime)));
				}
				else
				{
					num2 = pIParticleInstance.mTicks / pIParticleInstance.mLife;
				}
				pIParticleInstance.mLifePct = num2;
				if (pIParticleInstance.mLifePct >= 0.9999999f || pIParticleInstance.mLife <= 1E-08f || (!theEmitterInstance.mWasActive && !mEmitterInstanceDef.mIsSuperEmitter))
				{
					if (theParticleGroup.mIsSuperEmitter && ((PIFreeEmitterInstance)pIParticleInstance).mEmitter.mParticleGroup.mHead != null)
					{
						pIParticleInstance = mNext;
						continue;
					}
					if (theParticleGroup.mIsSuperEmitter || !mParticleDef.mSingleParticle || !theEmitterInstance.mWasActive)
					{
						if (theParticleGroup.mIsSuperEmitter)
						{
							mFreeEmitterPool.Free((PIFreeEmitterInstance)pIParticleInstance);
						}
						else
						{
							mParticlePool.Free(pIParticleInstance);
						}
						if (pIParticleInstance.mPrev != null)
						{
							pIParticleInstance.mPrev.mNext = pIParticleInstance.mNext;
						}
						if (pIParticleInstance.mNext != null)
						{
							pIParticleInstance.mNext.mPrev = pIParticleInstance.mPrev;
						}
						if (theParticleGroup.mHead == pIParticleInstance)
						{
							theParticleGroup.mHead = pIParticleInstance.mNext;
						}
						if (theParticleGroup.mTail == pIParticleInstance)
						{
							theParticleGroup.mTail = pIParticleInstance.mPrev;
						}
						theParticleGroup.mCount--;
						pIParticleInstance = mNext;
						continue;
					}
				}
				if (mParticleDef != null)
				{
					PITexture pITexture = mDef.mTextureVector[mParticleDef.mTextureIdx];
					if (mParticleDef.mAnimSpeed == -1)
					{
						pIParticleInstance.mImgIdx = pIParticleInstance.mAnimFrameRand % pITexture.mNumCels;
					}
					else
					{
						pIParticleInstance.mImgIdx = ((int)(pIParticleInstance.mTicks * (float)mFramerate / (float)(mParticleDef.mAnimSpeed + 1)) + pIParticleInstance.mAnimFrameRand) % pITexture.mNumCels;
					}
				}
				if (theParticleGroup.mIsSuperEmitter || !mParticleDef.mSingleParticle)
				{
					if (mIsNewFrame)
					{
						float num3 = GetRandFloat() * GetRandFloat();
						float num4 = GetRandFloat() * GetRandFloat();
						float num5 = ((!theParticleGroup.mIsSuperEmitter) ? Math.Max(0f, (theParticleGroup.mWasEmitted ? mEmitterSrc.mValues[15].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[6].GetValueAt(mFrameNum)) * mParticleDef.mValues[20].GetValueAt(num2) * (mParticleDef.mValues[6].GetValueAt(mFrameNum) + pIParticleInstance.mVariationValues[6])) : (Math.Max(0f, (theParticleGroup.mWasEmitted ? mEmitterSrc.mValues[15].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[6].GetValueAt(mFrameNum)) * mEmitterSrc.mValues[39].GetValueAt(num2, 1f) * (mEmitterSrc.mValues[5].GetValueAt(mFrameNum) + pIParticleInstance.mVariationValues[6])) * 30f));
						pIParticleInstance.mVel.X += num3 * num5;
						pIParticleInstance.mVel.Y += num4 * num5;
					}
					float num6 = ((!theParticleGroup.mIsSuperEmitter) ? ((theParticleGroup.mWasEmitted ? mEmitterSrc.mValues[13].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[4].GetValueAt(mFrameNum)) * (mParticleDef.mValues[18].GetValueAt(num2) - 1f) * (mParticleDef.mValues[4].GetValueAt(mFrameNum) + pIParticleInstance.mVariationValues[4]) * 100f) : ((theParticleGroup.mWasEmitted ? mEmitterSrc.mValues[13].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[4].GetValueAt(mFrameNum)) * (mEmitterSrc.mValues[37].GetValueAt(num2, 1f) - 1f) * (mEmitterSrc.mValues[3].GetValueAt(mFrameNum) + pIParticleInstance.mVariationValues[4]) / 2f * 100f));
					num6 *= 1f + ((float)mFramerate - 100f) * 0.0005f;
					pIParticleInstance.mVel.Y += num6 / num;
					Vector2 vector = pIParticleInstance.mVel / num;
					if (theParticleGroup.mIsSuperEmitter)
					{
						vector *= mEmitterSrc.mValues[36].GetValueAt(num2, 1f);
					}
					else
					{
						vector *= mParticleDef.mValues[17].GetValueAt(num2);
					}
					Vector2 aPtA = default(Vector2);
					if (!flag && mLayerDef.mDeflectorVector.Count > 0)
					{
						Vector2 aPtA2 = GlobalPIEffect.TransformFPoint(pIParticleInstance.mTransform, new Vector2(0f, 0f));
						Vector2 mPos = pIParticleInstance.mPos;
						pIParticleInstance.mPos += vector;
						CalcParticleTransform(theLayer, theEmitterInstance, mEmitterSrc, mParticleDef, theParticleGroup, pIParticleInstance);
						aPtA = GlobalPIEffect.TransformFPoint(pIParticleInstance.mTransform, new Vector2(0f, 0f));
						for (int i = 0; i < mLayerDef.mDeflectorVector.Count; i++)
						{
							PIDeflector pIDeflector = mLayerDef.mDeflectorVector[i];
							if (pIDeflector.mActive.GetLastKeyframe(mFrameNum) < 0.99f)
							{
								continue;
							}
							for (int j = 1; j < pIDeflector.mCurPoints.Count; j++)
							{
								Vector2 vector2 = pIDeflector.mCurPoints[j - 1] - new Vector2(mDrawTransform.m02, mDrawTransform.m12);
								Vector2 vector3 = pIDeflector.mCurPoints[j] - new Vector2(mDrawTransform.m02, mDrawTransform.m12);
								SexyVector2 sexyVector = new SexyVector2(vector3.X - vector2.X, vector3.Y - vector2.Y).Normalize().Perp();
								Vector2 vector4 = new Vector2(sexyVector.x, sexyVector.y);
								vector4 = vector4 * pIDeflector.mThickness * pIParticleInstance.mThicknessHitVariation;
								Vector2 theIntersectionPoint = default(Vector2);
								float thePos = 0f;
								if (GlobalPIEffect.LineSegmentIntersects(aPtA2, aPtA, vector2 + vector4, vector3 + vector4, ref thePos, theIntersectionPoint) && !(GetRandFloatU() > pIDeflector.mHits))
								{
									float mBounce = pIDeflector.mBounce;
									mBounce = ((!theParticleGroup.mIsSuperEmitter) ? (mBounce * 0.5f) : (mBounce * ((theParticleGroup.mWasEmitted ? mEmitterSrc.mValues[6].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[7].GetValueAt(mFrameNum)) * mEmitterSrc.mValues[40].GetValueAt(num2, 1f) * (mEmitterSrc.mValues[6].GetValueAt(mFrameNum) + pIParticleInstance.mVariationValues[9]))));
									SexyVector2 sexyVector2 = new SexyVector2(vector.X, vector.Y);
									float num7 = sexyVector2.Dot(sexyVector);
									SexyVector2 sexyVector3 = sexyVector2 - sexyVector * 2f * num7;
									float num8 = Math.Min(1f, Math.Abs(sexyVector3.y / sexyVector3.x));
									sexyVector3.y *= 1f - num8 + num8 * (float)Math.Pow(mBounce, 0.5);
									pIParticleInstance.mVel = new Vector2(sexyVector3.x, sexyVector3.y) * 100f;
									if (mBounce > 0.001f)
									{
										pIParticleInstance.mPos = mPos;
									}
									CalcParticleTransform(theLayer, theEmitterInstance, mEmitterSrc, mParticleDef, theParticleGroup, pIParticleInstance);
									aPtA = GlobalPIEffect.TransformFPoint(pIParticleInstance.mTransform, new Vector2(0f, 0f));
								}
							}
						}
					}
					else
					{
						pIParticleInstance.mPos += vector;
						if (mLayerDef.mForceVector.Count > 0)
						{
							CalcParticleTransform(theLayer, theEmitterInstance, mEmitterSrc, mParticleDef, theParticleGroup, pIParticleInstance);
							aPtA = GlobalPIEffect.TransformFPoint(pIParticleInstance.mTransform, new Vector2(0f, 0f));
						}
					}
					for (int k = 0; k < mLayerDef.mForceVector.Count; k++)
					{
						PIForce pIForce = mLayerDef.mForceVector[k];
						if (pIForce.mActive.GetLastKeyframe(mFrameNum) < 0.99f)
						{
							continue;
						}
						bool flag2 = false;
						int num9 = 0;
						int num10 = 3;
						while (num9 < 4)
						{
							if (((pIForce.mCurPoints[num9].Y <= aPtA.Y && aPtA.Y < pIForce.mCurPoints[num10].Y) || (pIForce.mCurPoints[num10].Y <= aPtA.Y && aPtA.Y < pIForce.mCurPoints[num9].Y)) && aPtA.X < (pIForce.mCurPoints[num10].X - pIForce.mCurPoints[num9].X) * (aPtA.Y - pIForce.mCurPoints[num9].Y) / (pIForce.mCurPoints[num10].Y - pIForce.mCurPoints[num9].Y) + pIForce.mCurPoints[num9].X)
							{
								flag2 = !flag2;
							}
							num10 = num9++;
						}
						if (flag2)
						{
							float num11 = MathHelper.ToRadians(0f - pIForce.mDirection.GetValueAt(mFrameNum)) + MathHelper.ToRadians(0f - pIForce.mAngle.GetValueAt(mFrameNum));
							float num12 = 0.085f * (float)mFramerate / 100f;
							num12 *= 1f + ((float)mFramerate - 100f) * 0.004f;
							float num13 = pIForce.mStrength.GetValueAt(mFrameNum) * num12;
							pIParticleInstance.mVel.X += (float)Math.Cos(num11) * num13 * 100f;
							pIParticleInstance.mVel.Y += (float)Math.Sin(num11) * num13 * 100f;
						}
					}
					if (!theParticleGroup.mIsSuperEmitter && mParticleDef.mAngleAlignToMotion && mParticleDef.mAngleKeepAlignedToMotion)
					{
						pIParticleInstance.mImgAngle = (float)(0.0 - Math.Atan2(vector.Y, vector.X)) + MathHelper.ToRadians(-mParticleDef.mAngleAlignOffset);
					}
				}
				else if (mParticleDef.mSingleParticle)
				{
					bool flag3 = false;
					if (mEmitterInstanceDef.mEmitterGeom == 1 || mEmitterInstanceDef.mEmitterGeom == 4)
					{
						flag3 = mEmitterInstanceDef.mEmitAtPointsNum != 0;
					}
					else if (mEmitterInstanceDef.mEmitterGeom == 3)
					{
						flag3 = mEmitterInstanceDef.mEmitAtPointsNum * mEmitterInstanceDef.mEmitAtPointsNum2 != 0;
					}
					if (flag3)
					{
						Vector2 geomPos = GetGeomPos(theEmitterInstance, pIParticleInstance);
						pIParticleInstance.mEmittedPos = GetEmitterPos(theEmitterInstance, true);
						pIParticleInstance.mLastEmitterPos = pIParticleInstance.mEmittedPos;
						pIParticleInstance.mOrigPos = geomPos - pIParticleInstance.mEmittedPos;
						SexyTransform2D theMatrix = new SexyTransform2D(false);
						theMatrix.RotateDeg(theEmitterInstance.mEmitterInstanceDef.mValues[14].GetValueAt(mFrameNum));
						pIParticleInstance.mEmittedPos += GlobalPIEffect.TransformFPoint(theMatrix, geomPos);
					}
					if (mParticleDef.mAngleKeepAlignedToMotion && !mParticleDef.mAttachToEmitter)
					{
						Vector2 velocityAt = mEmitterInstanceDef.mPosition.GetVelocityAt(mFrameNum);
						if (velocityAt.Length() != 0f)
						{
							pIParticleInstance.mImgAngle = (float)(0.0 - Math.Atan2(velocityAt.Y, velocityAt.X));
						}
						else
						{
							pIParticleInstance.mImgAngle = 0f;
						}
						pIParticleInstance.mImgAngle += MathHelper.ToRadians(-mParticleDef.mAngleAlignOffset);
					}
				}
				if (mParticleDef != null)
				{
					bool flag4 = (!pIParticleInstance.mHasDrawn && mParticleDef.mGetColorFromLayer) || mParticleDef.mUpdateColorFromLayer;
					bool flag5 = (!pIParticleInstance.mHasDrawn && mParticleDef.mGetTransparencyFromLayer) || mParticleDef.mUpdateTransparencyFromLayer;
					if (flag4 || flag5)
					{
						Vector2 vector5 = GlobalPIEffect.TransformFPoint(pIParticleInstance.mTransform, new Vector2(0f, 0f));
						int num14 = (int)vector5.X + (int)theLayer.mBkgImgDrawOfs.X;
						int num15 = (int)vector5.Y + (int)theLayer.mBkgImgDrawOfs.Y;
						uint num16;
						if (theLayer.mBkgImage != null && num14 >= 0 && num15 >= 0 && num14 < theLayer.mBkgImage.mWidth && num15 < theLayer.mBkgImage.mHeight)
						{
							uint[] bits = theLayer.mBkgImage.GetBits();
							num16 = bits[num14 + num15 * theLayer.mBkgImage.mWidth];
						}
						else
						{
							num16 = 0u;
						}
						if (flag4)
						{
							pIParticleInstance.mBkgColor = (pIParticleInstance.mBkgColor & 0xFF000000u) | (num16 & 0xFFFFFF);
						}
						if (flag5)
						{
							pIParticleInstance.mBkgColor = (pIParticleInstance.mBkgColor & 0xFFFFFF) | (num16 & 0xFF000000u);
						}
					}
				}
				if (theParticleGroup.mIsSuperEmitter)
				{
					pIParticleInstance.mImgAngle += MathHelper.ToRadians(0f - (theParticleGroup.mWasEmitted ? mEmitterSrc.mValues[4].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[5].GetValueAt(mFrameNum)) * (mEmitterSrc.mValues[38].GetValueAt(num2, 1f) - 1f) * (mEmitterSrc.mValues[4].GetValueAt(mFrameNum) + pIParticleInstance.mVariationValues[5])) / num * 160f;
				}
				else if (!mParticleDef.mAngleKeepAlignedToMotion)
				{
					pIParticleInstance.mImgAngle += MathHelper.ToRadians(0f - (theParticleGroup.mWasEmitted ? mEmitterSrc.mValues[14].GetValueAt(mFrameNum) : theEmitterInstance.mEmitterInstanceDef.mValues[5].GetValueAt(mFrameNum)) * (mParticleDef.mValues[19].GetValueAt(num2) - 1f) * (mParticleDef.mValues[5].GetValueAt(mFrameNum) + pIParticleInstance.mVariationValues[5])) / num;
				}
				pIParticleInstance = mNext;
			}
		}

		public void DrawParticleGroup(Graphics g, PILayer theLayer, PIEmitterInstance theEmitterInstance, PIParticleGroup theParticleGroup, bool isDarkeningPass)
		{
			if (!theEmitterInstance.mWasActive)
			{
				return;
			}
			PIParticleInstance pIParticleInstance = theParticleGroup.mHead;
			while (pIParticleInstance != null)
			{
				PIParticleInstance mNext = pIParticleInstance.mNext;
				PIParticleDef mParticleDef = pIParticleInstance.mParticleDef;
				if ((!mParticleDef.mIntense || !mParticleDef.mPreserveColor) && isDarkeningPass)
				{
					pIParticleInstance = mNext;
					continue;
				}
				if (mParticleDef.mIntense && !isDarkeningPass)
				{
					mAdditiveList.Add(pIParticleInstance);
				}
				else
				{
					mNormalList.Add(pIParticleInstance);
					if (isDarkeningPass)
					{
						mDarken = true;
					}
				}
				pIParticleInstance = mNext;
			}
		}

		public void DrawLayerNormal(Graphics g, PILayer theLayer)
		{
			g.PushState();
			g.SetColorizeImages(true);
			PILayerDef mLayerDef = theLayer.mLayerDef;
			for (int i = 0; i < theLayer.mEmitterInstanceVector.Count; i++)
			{
				PIEmitterInstanceDef pIEmitterInstanceDef = mLayerDef.mEmitterInstanceDefVector[i];
				PIEmitterInstance pIEmitterInstance = theLayer.mEmitterInstanceVector[i];
				if (!pIEmitterInstance.mVisible)
				{
					continue;
				}
				if (pIEmitterInstanceDef.mIsSuperEmitter)
				{
					for (int j = 0; j < pIEmitterInstanceDef.mFreeEmitterIndices.Count; j++)
					{
						for (PIFreeEmitterInstance pIFreeEmitterInstance = (PIFreeEmitterInstance)pIEmitterInstance.mSuperEmitterGroup.mHead; pIFreeEmitterInstance != null; pIFreeEmitterInstance = (PIFreeEmitterInstance)pIFreeEmitterInstance.mNext)
						{
							DrawParticleGroupNomal(g, theLayer, pIEmitterInstance, pIFreeEmitterInstance.mEmitter.mParticleGroup, mDarken);
						}
					}
				}
				else
				{
					DrawParticleGroupNomal(g, theLayer, pIEmitterInstance, pIEmitterInstance.mParticleGroup, mDarken);
				}
			}
			g.PopState();
		}

		public void DrawLayerAdditive(Graphics g, PILayer theLayer)
		{
			g.PushState();
			g.SetColorizeImages(true);
			PILayerDef mLayerDef = theLayer.mLayerDef;
			for (int i = 0; i < theLayer.mEmitterInstanceVector.Count; i++)
			{
				PIEmitterInstanceDef pIEmitterInstanceDef = mLayerDef.mEmitterInstanceDefVector[i];
				PIEmitterInstance pIEmitterInstance = theLayer.mEmitterInstanceVector[i];
				if (!pIEmitterInstance.mVisible)
				{
					continue;
				}
				if (pIEmitterInstanceDef.mIsSuperEmitter)
				{
					for (int j = 0; j < pIEmitterInstanceDef.mFreeEmitterIndices.Count; j++)
					{
						for (PIFreeEmitterInstance pIFreeEmitterInstance = (PIFreeEmitterInstance)pIEmitterInstance.mSuperEmitterGroup.mHead; pIFreeEmitterInstance != null; pIFreeEmitterInstance = (PIFreeEmitterInstance)pIFreeEmitterInstance.mNext)
						{
							DrawParticleGroupAdditive(g, theLayer, pIEmitterInstance, pIFreeEmitterInstance.mEmitter.mParticleGroup, false);
						}
					}
				}
				else
				{
					DrawParticleGroupAdditive(g, theLayer, pIEmitterInstance, pIEmitterInstance.mParticleGroup, false);
				}
			}
			g.PopState();
		}

		public void DrawParticleGroupNomal(Graphics g, PILayer theLayer, PIEmitterInstance theEmitterInstance, PIParticleGroup theParticleGroup, bool isDarkeningPass)
		{
			Color color = new Color(mColor.mRed * theLayer.mColor.mRed / 255, mColor.mGreen * theLayer.mColor.mGreen / 255, mColor.mBlue * theLayer.mColor.mBlue / 255, mColor.mAlpha * theLayer.mColor.mAlpha / 255);
			bool flag = color != Color.White;
			int count = mNormalList.Count;
			g.SetDrawMode(0);
			for (int i = 0; i < count; i += mOptimizeValue)
			{
				PIParticleInstance pIParticleInstance = mNormalList[i];
				PIParticleDef mParticleDef = pIParticleInstance.mParticleDef;
				float mLifePct = pIParticleInstance.mLifePct;
				PIEmitter mEmitterSrc = pIParticleInstance.mEmitterSrc;
				PITexture pITexture = mDef.mTextureVector[mParticleDef.mTextureIdx];
				DeviceImage deviceImage = null;
				Rect theSrcRect;
				if (pITexture.mImageVector.Count != 0)
				{
					deviceImage = pITexture.mImageVector[pIParticleInstance.mImgIdx].GetDeviceImage();
					theSrcRect = new Rect(0, 0, deviceImage.mWidth, deviceImage.mHeight);
				}
				else
				{
					deviceImage = pITexture.mImageStrip.GetDeviceImage();
					theSrcRect = deviceImage.GetCelRect(pIParticleInstance.mImgIdx);
				}
				int num = 0;
				if (mParticleDef.mRandomGradientColor)
				{
					if (mParticleDef.mUseKeyColorsOnly)
					{
						int theIdx = (int)Math.Min(pIParticleInstance.mGradientRand * (float)mParticleDef.mColor.mInterpolatorPointVector.Count, mParticleDef.mColor.mInterpolatorPointVector.Count - 1);
						num = mParticleDef.mColor.GetKeyframeNum(theIdx);
					}
					else
					{
						float mGradientRand = pIParticleInstance.mGradientRand;
						num = mParticleDef.mColor.GetValueAt(mGradientRand);
					}
				}
				else if (mParticleDef.mUseNextColorKey)
				{
					int theIdx2 = pIParticleInstance.mNum / mParticleDef.mNumberOfEachColor % mParticleDef.mColor.mInterpolatorPointVector.Count;
					num = mParticleDef.mColor.GetKeyframeNum(theIdx2);
				}
				else
				{
					float theTime = GlobalPIEffect.WrapFloat(mLifePct, 1 + mParticleDef.mRepeatColor);
					num = mParticleDef.mColor.GetValueAt(theTime);
				}
				if (mParticleDef.mGetColorFromLayer)
				{
					num = (num & -16777216) | (int)(pIParticleInstance.mBkgColor & 0xFFFFFF);
				}
				if (mParticleDef.mGetTransparencyFromLayer)
				{
					num = (num & 0xFFFFFF) | (int)(pIParticleInstance.mBkgColor & 0xFF000000u);
				}
				float thePct = theEmitterInstance.mEmitterInstanceDef.mValues[10].GetValueAt(mFrameNum) * mEmitterSrc.mValues[20].GetValueAt(mFrameNum, 1f);
				num = (int)GlobalPIEffect.InterpColor(num, theEmitterInstance.mTintColor.ToInt(), thePct);
				int valueAt = mParticleDef.mAlpha.GetValueAt(GlobalPIEffect.WrapFloat(mLifePct, 1 + mParticleDef.mRepeatAlpha));
				valueAt = (int)((float)valueAt * (theEmitterInstance.mEmitterInstanceDef.mValues[9].GetValueAt(mFrameNum) * mParticleDef.mValues[22].GetValueAt(mFrameNum) * mEmitterSrc.mValues[18].GetValueAt(mFrameNum, 1f)));
				if (isDarkeningPass)
				{
					num = (int)(num & 0xFF000000u);
				}
				num &= 0xFFFFFF;
				num |= valueAt << 24;
				Color color2 = new Color((num >> 16) & 0xFF, (num >> 8) & 0xFF, num & 0xFF, (num >> 24) & 0xFF);
				if (flag)
				{
					color2 = new Color(color2.mRed * color.mRed / 255, color2.mGreen * color.mGreen / 255, color2.mBlue * color.mBlue / 255, color2.mAlpha * color.mAlpha / 255);
				}
				if (color2.mAlpha != 0)
				{
					g.SetColor(color2);
					CalcParticleTransform(theLayer, theEmitterInstance, mEmitterSrc, mParticleDef, theParticleGroup, pIParticleInstance);
					SexyTransform2D theMatrix = mDrawTransform * pIParticleInstance.mTransform;
					g.DrawImageMatrix(deviceImage, theMatrix, theSrcRect);
					pIParticleInstance.mHasDrawn = true;
				}
			}
		}

		public void DrawParticleGroupAdditive(Graphics g, PILayer theLayer, PIEmitterInstance theEmitterInstance, PIParticleGroup theParticleGroup, bool isDarkeningPass)
		{
			Color color = new Color(mColor.mRed * theLayer.mColor.mRed / 255, mColor.mGreen * theLayer.mColor.mGreen / 255, mColor.mBlue * theLayer.mColor.mBlue / 255, mColor.mAlpha * theLayer.mColor.mAlpha / 255);
			bool flag = color != Color.White;
			g.SetDrawMode(1);
			int count = mAdditiveList.Count;
			for (int i = 0; i < count; i += mOptimizeValue)
			{
				PIParticleInstance pIParticleInstance = mAdditiveList[i];
				PIParticleDef mParticleDef = pIParticleInstance.mParticleDef;
				float mLifePct = pIParticleInstance.mLifePct;
				PIEmitter mEmitterSrc = pIParticleInstance.mEmitterSrc;
				PITexture pITexture = mDef.mTextureVector[mParticleDef.mTextureIdx];
				DeviceImage deviceImage = null;
				Rect theSrcRect;
				if (pITexture.mImageVector.Count != 0)
				{
					deviceImage = pITexture.mImageVector[pIParticleInstance.mImgIdx].GetDeviceImage();
					theSrcRect = new Rect(0, 0, deviceImage.mWidth, deviceImage.mHeight);
				}
				else
				{
					deviceImage = pITexture.mImageStrip.GetDeviceImage();
					theSrcRect = deviceImage.GetCelRect(pIParticleInstance.mImgIdx);
				}
				int num = 0;
				if (mParticleDef.mRandomGradientColor)
				{
					if (mParticleDef.mUseKeyColorsOnly)
					{
						int theIdx = (int)Math.Min(pIParticleInstance.mGradientRand * (float)mParticleDef.mColor.mInterpolatorPointVector.Count, mParticleDef.mColor.mInterpolatorPointVector.Count - 1);
						num = mParticleDef.mColor.GetKeyframeNum(theIdx);
					}
					else
					{
						float mGradientRand = pIParticleInstance.mGradientRand;
						num = mParticleDef.mColor.GetValueAt(mGradientRand);
					}
				}
				else if (mParticleDef.mUseNextColorKey)
				{
					int theIdx2 = pIParticleInstance.mNum / mParticleDef.mNumberOfEachColor % mParticleDef.mColor.mInterpolatorPointVector.Count;
					num = mParticleDef.mColor.GetKeyframeNum(theIdx2);
				}
				else
				{
					float theTime = GlobalPIEffect.WrapFloat(mLifePct, 1 + mParticleDef.mRepeatColor);
					num = mParticleDef.mColor.GetValueAt(theTime);
				}
				if (mParticleDef.mGetColorFromLayer)
				{
					num = (num & -16777216) | (int)(pIParticleInstance.mBkgColor & 0xFFFFFF);
				}
				if (mParticleDef.mGetTransparencyFromLayer)
				{
					num = (num & 0xFFFFFF) | (int)(pIParticleInstance.mBkgColor & 0xFF000000u);
				}
				float thePct = theEmitterInstance.mEmitterInstanceDef.mValues[10].GetValueAt(mFrameNum) * mEmitterSrc.mValues[20].GetValueAt(mFrameNum, 1f);
				num = (int)GlobalPIEffect.InterpColor(num, theEmitterInstance.mTintColor.ToInt(), thePct);
				int valueAt = mParticleDef.mAlpha.GetValueAt(GlobalPIEffect.WrapFloat(mLifePct, 1 + mParticleDef.mRepeatAlpha));
				valueAt = (int)((float)valueAt * (theEmitterInstance.mEmitterInstanceDef.mValues[9].GetValueAt(mFrameNum) * mParticleDef.mValues[22].GetValueAt(mFrameNum) * mEmitterSrc.mValues[18].GetValueAt(mFrameNum, 1f)));
				if (isDarkeningPass)
				{
					num = (int)(num & 0xFF000000u);
				}
				num &= 0xFFFFFF;
				num |= valueAt << 24;
				Color color2 = new Color((num >> 16) & 0xFF, (num >> 8) & 0xFF, num & 0xFF, (num >> 24) & 0xFF);
				if (flag)
				{
					color2 = new Color(color2.mRed * color.mRed / 255, color2.mGreen * color.mGreen / 255, color2.mBlue * color.mBlue / 255, color2.mAlpha * color.mAlpha / 255);
				}
				if (color2.mAlpha != 0)
				{
					g.SetColor(color2);
					CalcParticleTransform(theLayer, theEmitterInstance, mEmitterSrc, mParticleDef, theParticleGroup, pIParticleInstance);
					SexyTransform2D theMatrix = mDrawTransform * pIParticleInstance.mTransform;
					g.DrawImageMatrix(deviceImage, theMatrix, theSrcRect);
					pIParticleInstance.mHasDrawn = true;
				}
			}
		}

		public PIEffect()
		{
			mLoaded = false;
			mFileIdx = 0;
			mAutoPadImages = true;
			mFrameNum = 0f;
			mUpdateCnt = 0;
			mCurNumParticles = 0;
			mCurNumEmitters = 0;
			mLastDrawnPixelCount = 0;
			mFirstFrameNum = 0;
			mLastFrameNum = 0;
			mAnimSpeed = 1f;
			mColor = Color.White;
			mDebug = false;
			mDrawBlockers = true;
			mEmitAfterTimeline = false;
			mDrawTransform.LoadIdentity();
			mEmitterTransform.LoadIdentity();
			mPoolSize = 256;
			mParticlePool = new ObjectPool<PIParticleInstance>(mPoolSize);
			mFreeEmitterPool = new ObjectPool<PIFreeEmitterInstance>(mPoolSize);
			mNormalList = new List<PIParticleInstance>();
			mAdditiveList = new List<PIParticleInstance>();
			mDef = new PIEffectDef();
		}

		public PIEffect(int poolSize)
		{
			mLoaded = false;
			mFileIdx = 0;
			mAutoPadImages = true;
			mFrameNum = 0f;
			mUpdateCnt = 0;
			mCurNumParticles = 0;
			mCurNumEmitters = 0;
			mLastDrawnPixelCount = 0;
			mFirstFrameNum = 0;
			mLastFrameNum = 0;
			mAnimSpeed = 1f;
			mColor = Color.White;
			mDebug = false;
			mDrawBlockers = true;
			mEmitAfterTimeline = false;
			mDrawTransform.LoadIdentity();
			mEmitterTransform.LoadIdentity();
			mPoolSize = poolSize;
			mParticlePool = new ObjectPool<PIParticleInstance>(poolSize);
			mFreeEmitterPool = new ObjectPool<PIFreeEmitterInstance>(poolSize);
			mNormalList = new List<PIParticleInstance>();
			mAdditiveList = new List<PIParticleInstance>();
			mDef = new PIEffectDef();
		}

		public PIEffect(PIEffect rhs)
		{
			mFileChecksum = rhs.mFileChecksum;
			mSrcFileName = rhs.mSrcFileName;
			mVersion = rhs.mVersion;
			mStartupState = rhs.mStartupState;
			mNotes = rhs.mNotes;
			mWidth = rhs.mWidth;
			mHeight = rhs.mHeight;
			mBkgColor = rhs.mBkgColor;
			mFramerate = rhs.mFramerate;
			mFirstFrameNum = rhs.mFirstFrameNum;
			mLastFrameNum = rhs.mLastFrameNum;
			mNotesParams = rhs.mNotesParams;
			mLastLifePct = rhs.mLastLifePct;
			mError = rhs.mError;
			mLoaded = rhs.mLoaded;
			mAnimSpeed = rhs.mAnimSpeed;
			mColor = rhs.mColor;
			mDebug = rhs.mDebug;
			mDrawBlockers = rhs.mDrawBlockers;
			mEmitAfterTimeline = rhs.mEmitAfterTimeline;
			mRandSeeds = rhs.mRandSeeds;
			mDrawTransform.CopyFrom(rhs.mDrawTransform);
			mEmitterTransform.CopyFrom(rhs.mEmitterTransform);
			mFileIdx = 0;
			mFrameNum = 0f;
			mUpdateCnt = 0;
			mIsNewFrame = false;
			mHasEmitterTransform = false;
			mHasDrawTransform = false;
			mDrawTransformSimple = false;
			mCurNumParticles = 0;
			mCurNumEmitters = 0;
			mLastDrawnPixelCount = 0;
			mDef = rhs.mDef;
			mDef.mRefCount++;
			mPoolSize = rhs.mPoolSize;
			mParticlePool = new ObjectPool<PIParticleInstance>(mPoolSize);
			mFreeEmitterPool = new ObjectPool<PIFreeEmitterInstance>(mPoolSize);
			mNormalList = new List<PIParticleInstance>();
			mAdditiveList = new List<PIParticleInstance>();
			Common.Resize(mLayerVector, rhs.mDef.mLayerDefVector.Count);
			Common.Resize(mDef.mLayerDefVector, rhs.mDef.mLayerDefVector.Count);
			for (int i = 0; i < mLayerVector.Count; i++)
			{
				PILayerDef pILayerDef = mDef.mLayerDefVector[i];
				PILayer pILayer = mLayerVector[i];
				pILayer.mLayerDef = pILayerDef;
				Common.Resize(pILayer.mEmitterInstanceVector, pILayerDef.mEmitterInstanceDefVector.Count);
				for (int j = 0; j < pILayerDef.mEmitterInstanceDefVector.Count; j++)
				{
					PIEmitterInstance pIEmitterInstance = rhs.mLayerVector[i].mEmitterInstanceVector[j];
					PIEmitterInstanceDef pIEmitterInstanceDef = pILayerDef.mEmitterInstanceDefVector[j];
					PIEmitterInstance pIEmitterInstance2 = pILayer.mEmitterInstanceVector[j];
					PIEmitter pIEmitter = mDef.mEmitterVector[pIEmitterInstanceDef.mEmitterDefIdx];
					pIEmitterInstance2.mEmitterInstanceDef = pIEmitterInstanceDef;
					pIEmitterInstance2.mTintColor = new Color(pIEmitterInstance.mTintColor);
					Common.Resize(pIEmitterInstance2.mParticleDefInstanceVector, pIEmitter.mParticleDefVector.Count);
					Common.Resize(pIEmitterInstance2.mSuperEmitterParticleDefInstanceVector, pIEmitterInstance.mSuperEmitterParticleDefInstanceVector.Count);
				}
			}
			ResetAnim();
		}

		public virtual void Dispose()
		{
			ResetAnim();
			Deref();
		}

		public PIEffect Duplicate()
		{
			return new PIEffect(this);
		}

		public virtual SharedImageRef GetImage(string theName, string theFilename)
		{
			return GlobalMembers.gSexyAppBase.GetSharedImage(Common.GetPathFrom(theFilename, Common.GetFileDir(mSrcFileName, true)));
		}

		public virtual void SetImageOpts(DeviceImage theImage)
		{
		}

		public virtual string WriteImage(string theName, int theIdx, DeviceImage theImage)
		{
			return WriteImage(theName, theIdx, null);
		}

		public virtual string WriteImage(string theName, int theIdx, DeviceImage theImage, int hasPadding)
		{
			throw new NotImplementedException();
		}

		public bool LoadEffect(string theFileName)
		{
			if (mDef.mRefCount > 1)
			{
				Deref();
			}
			Clear();
			mVersion = 0;
			mFileChecksum = 0;
			mSrcFileName = theFileName;
			mReadBuffer = new SexyFramework.Misc.Buffer();
			if (!GlobalMembers.gSexyAppBase.ReadBufferFromStream(theFileName, ref mReadBuffer))
			{
				return Fail("Unable to open file: " + theFileName);
			}
			mIsPPF = true;
			mBufPos = 0;
			mChecksumPos = GlobalPIEffect.PI_BUFSIZE;
			ReadString();
			if (mIsPPF)
			{
				mVersion = mReadBuffer.ReadInt32();
			}
			if (mVersion < 0)
			{
				Fail("PPF version too old");
			}
			mNotes = ReadString();
			short num = mReadBuffer.ReadShort();
			for (int i = 0; i < num; i++)
			{
				ExpectCmd("CMultiTexture");
				PITexture pITexture = new PITexture();
				pITexture.mName = ReadString();
				short num2 = (short)(pITexture.mNumCels = mReadBuffer.ReadShort());
				if (mIsPPF)
				{
					short num3 = mReadBuffer.ReadShort();
					pITexture.mPadded = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					string text = (pITexture.mFileName = ReadString());
					pITexture.mImageStrip = GetImage(pITexture.mName, text);
					if (pITexture.mImageStrip.GetDeviceImage() == null)
					{
						Fail("Unable to load image: " + text);
					}
					else if (pITexture.mImageStrip.GetDeviceImage().mNumCols == 1 && pITexture.mImageStrip.GetDeviceImage().mNumRows == 1)
					{
						pITexture.mImageStrip.GetDeviceImage().mNumCols = num2 / num3;
						pITexture.mImageStrip.GetDeviceImage().mNumRows = num3;
					}
					mDef.mTextureVector.Add(pITexture);
					continue;
				}
				throw new NotImplementedException();
			}
			short num4 = mReadBuffer.ReadShort();
			mDef.mEmitterVector.Capacity = num4;
			Common.Resize(mDef.mEmitterVector, num4);
			for (int j = 0; j < num4; j++)
			{
				ExpectCmd("CEmitterType");
				if (!mIsPPF)
				{
					mDef.mEmitterRefMap.Add(mStringVector.Count, j);
				}
				ReadEmitterType(mDef.mEmitterVector[j]);
			}
			List<bool> list = new List<bool>();
			list.Capacity = mDef.mEmitterVector.Count;
			Common.Resize(list, mDef.mEmitterVector.Count);
			List<bool> list2 = new List<bool>();
			list2.Capacity = mDef.mTextureVector.Count;
			Common.Resize(list2, mDef.mTextureVector.Count);
			short num5 = mReadBuffer.ReadShort();
			Common.Resize(mLayerVector, num5);
			Common.Resize(mDef.mLayerDefVector, num5);
			for (int k = 0; k < num5; k++)
			{
				PILayerDef pILayerDef = mDef.mLayerDefVector[k];
				PILayer pILayer = mLayerVector[k];
				pILayer.mLayerDef = pILayerDef;
				ExpectCmd("CLayer");
				pILayerDef.mName = ReadString();
				num4 = mReadBuffer.ReadShort();
				pILayer.mEmitterInstanceVector.Capacity = num4;
				Common.Resize(pILayer.mEmitterInstanceVector, num4);
				pILayerDef.mEmitterInstanceDefVector.Capacity = num4;
				Common.Resize(pILayerDef.mEmitterInstanceDefVector, num4);
				for (int l = 0; l < num4; l++)
				{
					PIEmitterInstanceDef pIEmitterInstanceDef = pILayerDef.mEmitterInstanceDefVector[l];
					PIEmitterInstance pIEmitterInstance = pILayer.mEmitterInstanceVector[l];
					pIEmitterInstance.mEmitterInstanceDef = pIEmitterInstanceDef;
					ExpectCmd("CEmitter");
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					pIEmitterInstanceDef.mFramesToPreload = mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					pIEmitterInstanceDef.mName = ReadString();
					pIEmitterInstanceDef.mEmitterGeom = mReadBuffer.ReadInt32();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					if ((mIsPPF ? mReadBuffer.ReadByte() : mReadBuffer.ReadInt32()) != 0 && pIEmitterInstanceDef.mEmitterGeom == 2)
					{
						pIEmitterInstanceDef.mEmitterGeom = 4;
					}
					pIEmitterInstanceDef.mEmitIn = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					pIEmitterInstanceDef.mEmitOut = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					uint num6 = (uint)((mReadBuffer.ReadByte() << 16) | -16777216);
					mReadBuffer.ReadByte();
					mReadBuffer.ReadByte();
					mReadBuffer.ReadByte();
					num6 |= (uint)(mReadBuffer.ReadByte() << 8);
					mReadBuffer.ReadByte();
					mReadBuffer.ReadByte();
					mReadBuffer.ReadByte();
					num6 |= mReadBuffer.ReadByte();
					mReadBuffer.ReadByte();
					mReadBuffer.ReadByte();
					mReadBuffer.ReadByte();
					pIEmitterInstance.mTintColor = new Color((int)num6);
					mReadBuffer.ReadInt32();
					pIEmitterInstanceDef.mEmitAtPointsNum = mReadBuffer.ReadInt32();
					pIEmitterInstanceDef.mEmitterDefIdx = mReadBuffer.ReadInt32();
					list[pIEmitterInstanceDef.mEmitterDefIdx] = true;
					PIEmitter pIEmitter = mDef.mEmitterVector[pIEmitterInstanceDef.mEmitterDefIdx];
					Common.Resize(pIEmitterInstance.mParticleDefInstanceVector, pIEmitter.mParticleDefVector.Count);
					for (int m = 0; m < pIEmitter.mParticleDefVector.Count; m++)
					{
						list2[pIEmitter.mParticleDefVector[m].mTextureIdx] = true;
					}
					ReadValue2D(pIEmitterInstanceDef.mPosition);
					int num7 = mReadBuffer.ReadShort();
					for (int n = 0; n < num7; n++)
					{
						ExpectCmd("CEPoint");
						mReadBuffer.ReadFloat();
						mReadBuffer.ReadFloat();
						PIValue2D pIValue2D = new PIValue2D();
						ReadEPoint(pIValue2D);
						pIEmitterInstanceDef.mPoints.Add(pIValue2D);
					}
					for (int num8 = 0; num8 < 17; num8++)
					{
						ReadValue(ref pIEmitterInstanceDef.mValues[num8]);
					}
					pIEmitterInstanceDef.mEmitAtPointsNum2 = mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					ReadValue(ref pIEmitterInstanceDef.mValues[17]);
					mReadBuffer.ReadInt32();
					ReadValue(ref pIEmitterInstanceDef.mValues[18]);
					short num9 = mReadBuffer.ReadShort();
					string theFilename = "";
					for (int num10 = 0; num10 < num9; num10++)
					{
						theFilename = ReadString();
					}
					bool flag = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					string theName = ReadString();
					if (flag)
					{
						pIEmitterInstance.mMaskImage = GetImage(theName, theFilename);
					}
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					pIEmitterInstanceDef.mInvertMask = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					pIEmitterInstanceDef.mIsSuperEmitter = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					int num11 = mReadBuffer.ReadShort();
					for (int num12 = 0; num12 < num11; num12++)
					{
						if (mIsPPF)
						{
							int item = mReadBuffer.ReadShort();
							pIEmitterInstanceDef.mFreeEmitterIndices.Add(item);
							list[l] = true;
							continue;
						}
						throw new NotImplementedException();
					}
					Common.Resize(pIEmitterInstance.mSuperEmitterParticleDefInstanceVector, num11);
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
				}
				short num13 = mReadBuffer.ReadShort();
				for (int num14 = 0; num14 < num13; num14++)
				{
					PIDeflector pIDeflector = new PIDeflector();
					ExpectCmd("CDeflector");
					pIDeflector.mName = ReadString();
					pIDeflector.mBounce = mReadBuffer.ReadInt32();
					pIDeflector.mHits = mReadBuffer.ReadInt32();
					pIDeflector.mThickness = mReadBuffer.ReadInt32();
					pIDeflector.mVisible = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					ReadValue2D(pIDeflector.mPos);
					int num15 = mReadBuffer.ReadShort();
					for (int num16 = 0; num16 < num15; num16++)
					{
						ExpectCmd("CEPoint");
						mReadBuffer.ReadFloat();
						mReadBuffer.ReadFloat();
						PIValue2D pIValue2D2 = new PIValue2D();
						ReadEPoint(pIValue2D2);
						pIDeflector.mPoints.Add(pIValue2D2);
					}
					Common.Resize(pIDeflector.mCurPoints, pIDeflector.mPoints.Count);
					ReadValue(ref pIDeflector.mActive);
					ReadValue(ref pIDeflector.mAngle);
					pILayerDef.mDeflectorVector.Add(pIDeflector);
				}
				short num17 = mReadBuffer.ReadShort();
				for (int num18 = 0; num18 < num17; num18++)
				{
					PIBlocker pIBlocker = new PIBlocker();
					ExpectCmd("CBlocker");
					pIBlocker.mName = ReadString();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					ReadValue2D(pIBlocker.mPos);
					int num19 = mReadBuffer.ReadShort();
					for (int num20 = 0; num20 < num19; num20++)
					{
						ExpectCmd("CEPoint");
						mReadBuffer.ReadFloat();
						mReadBuffer.ReadFloat();
						PIValue2D pIValue2D3 = new PIValue2D();
						ReadEPoint(pIValue2D3);
						pIBlocker.mPoints.Add(pIValue2D3);
					}
					ReadValue(ref pIBlocker.mActive);
					ReadValue(ref pIBlocker.mAngle);
					pILayerDef.mBlockerVector.Add(pIBlocker);
				}
				ReadValue2D(pILayerDef.mOffset);
				pILayerDef.mOrigOffset = pILayerDef.mOffset.GetValueAt(0f);
				ReadValue(ref pILayerDef.mAngle);
				ReadString();
				for (int num21 = 0; num21 < 32; num21++)
				{
					mReadBuffer.ReadByte();
				}
				int num22 = mReadBuffer.ReadShort();
				for (int num23 = 0; num23 < num22; num23++)
				{
					ReadString();
				}
				for (int num24 = 0; num24 < 36; num24++)
				{
					mReadBuffer.ReadByte();
				}
				short num25 = mReadBuffer.ReadShort();
				for (int num26 = 0; num26 < num25; num26++)
				{
					ExpectCmd("CForce");
					PIForce pIForce = new PIForce();
					pIForce.mName = ReadString();
					pIForce.mVisible = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					ReadValue2D(pIForce.mPos);
					ReadValue(ref pIForce.mActive);
					PIValue theValue = new PIValue();
					ReadValue(ref theValue);
					ReadValue(ref pIForce.mStrength);
					ReadValue(ref pIForce.mWidth);
					ReadValue(ref pIForce.mHeight);
					ReadValue(ref pIForce.mAngle);
					ReadValue(ref pIForce.mDirection);
					pILayerDef.mForceVector.Add(pIForce);
				}
				for (int num27 = 0; num27 < 28; num27++)
				{
					mReadBuffer.ReadByte();
				}
			}
			List<int> list3 = new List<int>();
			Common.Resize(list3, mDef.mEmitterVector.Count);
			int num28 = 0;
			for (int num29 = 0; num29 < mDef.mEmitterVector.Count; num29++)
			{
				if (list[num29])
				{
					list3[num29] = num28++;
				}
			}
			int num30 = 0;
			int num31 = 0;
			for (int num32 = 0; num32 < list.Count; num32++)
			{
				if (!list[num30])
				{
					mDef.mEmitterVector.RemoveAt(num31);
				}
				else
				{
					num31++;
				}
				num30++;
			}
			for (int num33 = 0; num33 < mDef.mLayerDefVector.Count; num33++)
			{
				PILayerDef pILayerDef2 = mDef.mLayerDefVector[num33];
				for (int num34 = 0; num34 < pILayerDef2.mEmitterInstanceDefVector.Count; num34++)
				{
					PIEmitterInstanceDef pIEmitterInstanceDef2 = pILayerDef2.mEmitterInstanceDefVector[num34];
					pIEmitterInstanceDef2.mEmitterDefIdx = list3[pIEmitterInstanceDef2.mEmitterDefIdx];
					for (int num35 = 0; num35 < pIEmitterInstanceDef2.mFreeEmitterIndices.Count; num35++)
					{
						pIEmitterInstanceDef2.mFreeEmitterIndices[num35] = list3[pIEmitterInstanceDef2.mFreeEmitterIndices[num35]];
					}
				}
			}
			List<int> list4 = new List<int>();
			Common.Resize(list4, mDef.mTextureVector.Count);
			int num36 = 0;
			for (int num37 = 0; num37 < mDef.mTextureVector.Count; num37++)
			{
				if (list2[num37])
				{
					list4[num37] = num36++;
				}
			}
			num30 = 0;
			num31 = 0;
			for (int num38 = 0; num38 < list2.Count; num38++)
			{
				if (!list2[num30])
				{
					mDef.mTextureVector.RemoveAt(num31);
				}
				else
				{
					num31++;
				}
				num30++;
			}
			for (int num39 = 0; num39 < mDef.mEmitterVector.Count; num39++)
			{
				PIEmitter pIEmitter2 = mDef.mEmitterVector[num39];
				for (int num40 = 0; num40 < pIEmitter2.mParticleDefVector.Count; num40++)
				{
					PIParticleDef pIParticleDef = pIEmitter2.mParticleDefVector[num40];
					pIParticleDef.mTextureIdx = list4[pIParticleDef.mTextureIdx];
				}
			}
			uint num41 = (uint)((mReadBuffer.ReadByte() << 16) | -16777216);
			mReadBuffer.ReadByte();
			mReadBuffer.ReadByte();
			mReadBuffer.ReadByte();
			num41 |= (uint)(mReadBuffer.ReadByte() << 8);
			mReadBuffer.ReadByte();
			mReadBuffer.ReadByte();
			mReadBuffer.ReadByte();
			num41 |= mReadBuffer.ReadByte();
			mReadBuffer.ReadByte();
			mReadBuffer.ReadByte();
			mReadBuffer.ReadByte();
			mBkgColor = new Color((int)num41);
			mReadBuffer.ReadInt32();
			mReadBuffer.ReadInt32();
			mFramerate = mReadBuffer.ReadShort();
			mReadBuffer.ReadShort();
			mReadBuffer.ReadShort();
			mReadBuffer.ReadShort();
			mWidth = mReadBuffer.ReadInt32();
			mHeight = mReadBuffer.ReadInt32();
			mReadBuffer.ReadInt32();
			mReadBuffer.ReadInt32();
			mReadBuffer.ReadInt32();
			mReadBuffer.ReadInt32();
			mReadBuffer.ReadInt32();
			mFirstFrameNum = mReadBuffer.ReadInt32();
			mLastFrameNum = mReadBuffer.ReadInt32();
			ReadString();
			mReadBuffer.ReadByte();
			mReadBuffer.ReadShort();
			mReadBuffer.ReadShort();
			if (mIsPPF && mVersion >= 1)
			{
				int num42 = mReadBuffer.ReadInt32();
				if (num42 > 0)
				{
					mStartupState.mData.Clear();
					mStartupState.mDataBitSize = num42 * 8;
					byte[] theData = new byte[num42];
					mReadBuffer.ReadBytes(ref theData, num42);
					mStartupState.mData.AddRange(theData);
					theData = null;
				}
			}
			else
			{
				mStartupState.Clear();
			}
			int num43 = 0;
			while (num43 < mNotes.Length)
			{
				string text2 = "";
				int num44 = mNotes.IndexOf('\n', num43);
				if (num44 != -1)
				{
					text2 = mNotes.Substring(num43, num44 - num43).Trim();
					num43 = num44 + 1;
				}
				else
				{
					text2 = mNotes.Substring(num43).Trim();
					num43 = mNotes.Length;
				}
				if (text2.Length > 0)
				{
					int num45 = text2.IndexOf(':');
					if (num45 != -1)
					{
						mNotesParams.Add(text2.Substring(0, num45).ToUpper(), text2.Substring(num45 + 1).Trim());
					}
					else
					{
						mNotesParams.Add(text2.ToUpper(), "");
					}
				}
			}
			string notesParam = GetNotesParam("Rand");
			int num46 = 0;
			while (num46 < notesParam.Length)
			{
				int num47 = notesParam.IndexOf(',', num46);
				if (num47 != -1)
				{
					mRandSeeds.Add(Convert.ToInt32(notesParam.Substring(num46, num47 - num46).Trim()));
					num46 = num47 + 1;
					continue;
				}
				mRandSeeds.Add(Convert.ToInt32(notesParam.Substring(num46).Trim()));
				break;
			}
			mEmitAfterTimeline = GetNotesParam("EmitAfter", "no") != "no";
			if (mError.Length == 0 && !GlobalMembers.gSexyAppBase.mReloadingResources)
			{
				WriteToCache();
			}
			return mLoaded = mError.Length == 0;
		}

		public void RefreshImageRes()
		{
			for (int i = 0; i < mDef.mTextureVector.Count; i++)
			{
				PITexture pITexture = mDef.mTextureVector[i];
				pITexture.mImageStrip = (pITexture.mImageStrip = GetImage(pITexture.mName, pITexture.mFileName));
			}
		}

		public bool SaveAsPPF(string theFileName)
		{
			return SaveAsPPF(theFileName, true);
		}

		public bool SaveAsPPF(string theFileName, bool saveInitialState)
		{
			throw new NotImplementedException();
		}

		public bool LoadState(SexyFramework.Misc.Buffer theBuffer)
		{
			return LoadState(theBuffer, false);
		}

		public bool LoadState(SexyFramework.Misc.Buffer theBuffer, bool shortened)
		{
			if (mError.Length != 0)
			{
				return false;
			}
			ResetAnim();
			theBuffer.mReadBitPos = (theBuffer.mReadBitPos + 7) & -8;
			int num = (int)theBuffer.ReadLong();
			int num2 = theBuffer.mReadBitPos / 8 + num;
			int num3 = theBuffer.ReadShort();
			if (!shortened)
			{
				string theFileName = theBuffer.ReadString();
				if (!mLoaded)
				{
					LoadEffect(theFileName);
				}
				int num4 = (int)theBuffer.ReadLong();
				if (num4 != mFileChecksum)
				{
					theBuffer.mReadBitPos = num2 * 8;
					return false;
				}
			}
			mFrameNum = theBuffer.ReadFloat();
			if (!shortened)
			{
				mRand.SRand(theBuffer.ReadString());
				mWantsSRand = false;
			}
			if (!shortened)
			{
				mEmitAfterTimeline = theBuffer.ReadBoolean();
				mEmitterTransform = theBuffer.ReadTransform2D();
				mDrawTransform = theBuffer.ReadTransform2D();
			}
			else if (num3 == 0)
			{
				theBuffer.ReadBoolean();
				theBuffer.ReadTransform2D();
				theBuffer.ReadTransform2D();
			}
			if (mFrameNum > 0f)
			{
				for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
				{
					PILayer pILayer = mLayerVector[i];
					PILayerDef pILayerDef = mDef.mLayerDefVector[i];
					for (int j = 0; j < pILayerDef.mEmitterInstanceDefVector.Count; j++)
					{
						PIEmitterInstance pIEmitterInstance = pILayer.mEmitterInstanceVector[j];
						PIEmitterInstanceDef pIEmitterInstanceDef = pILayerDef.mEmitterInstanceDefVector[j];
						if (theBuffer.ReadBoolean())
						{
							pIEmitterInstance.mTransform = theBuffer.ReadTransform2D();
						}
						pIEmitterInstance.mWasActive = theBuffer.ReadBoolean();
						pIEmitterInstance.mWithinLifeFrame = theBuffer.ReadBoolean();
						PIEmitter pIEmitter = mDef.mEmitterVector[pIEmitterInstanceDef.mEmitterDefIdx];
						for (int k = 0; k < pIEmitter.mParticleDefVector.Count; k++)
						{
							PIParticleDefInstance theParticleDefInstance = pIEmitterInstance.mParticleDefInstanceVector[k];
							LoadParticleDefInstance(theBuffer, theParticleDefInstance);
						}
						for (int l = 0; l < pIEmitterInstanceDef.mFreeEmitterIndices.Count; l++)
						{
							PIParticleDefInstance theParticleDefInstance2 = pIEmitterInstance.mSuperEmitterParticleDefInstanceVector[l];
							LoadParticleDefInstance(theBuffer, theParticleDefInstance2);
						}
						int num5 = (int)theBuffer.ReadLong();
						for (int m = 0; m < num5; m++)
						{
							PIFreeEmitterInstance pIFreeEmitterInstance = mFreeEmitterPool.Alloc();
							pIFreeEmitterInstance.Reset();
							int index = theBuffer.ReadShort();
							pIFreeEmitterInstance.mEmitterSrc = mDef.mEmitterVector[pIEmitterInstanceDef.mFreeEmitterIndices[index]];
							pIFreeEmitterInstance.mParentFreeEmitter = null;
							pIFreeEmitterInstance.mParticleDef = null;
							pIFreeEmitterInstance.mNum = m;
							LoadParticle(theBuffer, pILayer, pIFreeEmitterInstance);
							PIEmitter mEmitterSrc = pIFreeEmitterInstance.mEmitterSrc;
							Common.Resize(pIFreeEmitterInstance.mEmitter.mParticleDefInstanceVector, mEmitterSrc.mParticleDefVector.Count);
							for (int n = 0; n < mEmitterSrc.mParticleDefVector.Count; n++)
							{
								PIParticleDefInstance theParticleDefInstance3 = pIFreeEmitterInstance.mEmitter.mParticleDefInstanceVector[n];
								LoadParticleDefInstance(theBuffer, theParticleDefInstance3);
							}
							if (m > 0)
							{
								pIEmitterInstance.mSuperEmitterGroup.mTail.mNext = pIFreeEmitterInstance;
								pIFreeEmitterInstance.mPrev = pIEmitterInstance.mSuperEmitterGroup.mTail;
							}
							else
							{
								pIEmitterInstance.mSuperEmitterGroup.mHead = pIFreeEmitterInstance;
							}
							pIEmitterInstance.mSuperEmitterGroup.mTail = pIFreeEmitterInstance;
							pIEmitterInstance.mSuperEmitterGroup.mCount++;
							int num6 = (int)theBuffer.ReadLong();
							for (int num7 = 0; num7 < num6; num7++)
							{
								PIParticleInstance pIParticleInstance = mParticlePool.Alloc();
								pIParticleInstance.Reset();
								pIParticleInstance.mEmitterSrc = pIFreeEmitterInstance.mEmitterSrc;
								pIParticleInstance.mParentFreeEmitter = pIFreeEmitterInstance;
								int index2 = theBuffer.ReadShort();
								pIParticleInstance.mParticleDef = pIParticleInstance.mEmitterSrc.mParticleDefVector[index2];
								pIParticleInstance.mNum = num7;
								LoadParticle(theBuffer, pILayer, pIParticleInstance);
								CalcParticleTransform(pILayer, pIEmitterInstance, pIParticleInstance.mEmitterSrc, pIParticleInstance.mParticleDef, pIFreeEmitterInstance.mEmitter.mParticleGroup, pIParticleInstance);
								if (num7 > 0)
								{
									pIFreeEmitterInstance.mEmitter.mParticleGroup.mTail.mNext = pIParticleInstance;
									pIParticleInstance.mPrev = pIFreeEmitterInstance.mEmitter.mParticleGroup.mTail;
								}
								else
								{
									pIFreeEmitterInstance.mEmitter.mParticleGroup.mHead = pIParticleInstance;
								}
								pIFreeEmitterInstance.mEmitter.mParticleGroup.mTail = pIParticleInstance;
								pIFreeEmitterInstance.mEmitter.mParticleGroup.mCount++;
							}
						}
						int num8 = (int)theBuffer.ReadLong();
						for (int num9 = 0; num9 < num8; num9++)
						{
							PIParticleInstance pIParticleInstance2 = mParticlePool.Alloc();
							pIParticleInstance2.Reset();
							pIParticleInstance2.mEmitterSrc = pIEmitter;
							pIParticleInstance2.mParentFreeEmitter = null;
							int index3 = theBuffer.ReadShort();
							pIParticleInstance2.mParticleDef = pIParticleInstance2.mEmitterSrc.mParticleDefVector[index3];
							pIParticleInstance2.mNum = num9;
							LoadParticle(theBuffer, pILayer, pIParticleInstance2);
							CalcParticleTransform(pILayer, pIEmitterInstance, pIParticleInstance2.mEmitterSrc, pIParticleInstance2.mParticleDef, pIEmitterInstance.mParticleGroup, pIParticleInstance2);
							if (num9 > 0)
							{
								pIEmitterInstance.mParticleGroup.mTail.mNext = pIParticleInstance2;
								pIParticleInstance2.mPrev = pIEmitterInstance.mParticleGroup.mTail;
							}
							else
							{
								pIEmitterInstance.mParticleGroup.mHead = pIParticleInstance2;
							}
							pIEmitterInstance.mParticleGroup.mTail = pIParticleInstance2;
							pIEmitterInstance.mParticleGroup.mCount++;
						}
					}
				}
			}
			else
			{
				theBuffer.mReadBitPos = num2 * 8;
			}
			return true;
		}

		public bool SaveState(SexyFramework.Misc.Buffer theBuffer)
		{
			return SaveState(ref theBuffer, false);
		}

		public bool SaveState(ref SexyFramework.Misc.Buffer theBuffer, bool shortened)
		{
			if (mError.Length != 0)
			{
				return false;
			}
			theBuffer.mWriteBitPos = (theBuffer.mWriteBitPos + 7) & -8;
			int num = theBuffer.mWriteBitPos / 8;
			theBuffer.WriteLong(0L);
			theBuffer.WriteShort(1);
			if (!shortened)
			{
				theBuffer.WriteString(mSrcFileName);
				theBuffer.WriteLong(mFileChecksum);
			}
			theBuffer.WriteFloat(mFrameNum);
			if (!shortened)
			{
				theBuffer.WriteString(mRand.Serialize());
				theBuffer.WriteBoolean(mEmitAfterTimeline);
				theBuffer.WriteTransform2D(mEmitterTransform);
				theBuffer.WriteTransform2D(mDrawTransform);
			}
			if (mFrameNum > 0f)
			{
				for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
				{
					PILayer pILayer = mLayerVector[i];
					PILayerDef pILayerDef = mDef.mLayerDefVector[i];
					for (int j = 0; j < pILayer.mEmitterInstanceVector.Count; j++)
					{
						PIEmitterInstance pIEmitterInstance = pILayer.mEmitterInstanceVector[j];
						PIEmitterInstanceDef pIEmitterInstanceDef = pILayerDef.mEmitterInstanceDefVector[j];
						if (!GlobalPIEffect.IsIdentityMatrix(pIEmitterInstance.mTransform))
						{
							theBuffer.WriteBoolean(true);
							theBuffer.WriteTransform2D(pIEmitterInstance.mTransform);
						}
						else
						{
							theBuffer.WriteBoolean(false);
						}
						theBuffer.WriteBoolean(pIEmitterInstance.mWasActive);
						theBuffer.WriteBoolean(pIEmitterInstance.mWithinLifeFrame);
						Dictionary<PIEmitter, Dictionary<PIParticleDef, int>> dictionary = new Dictionary<PIEmitter, Dictionary<PIParticleDef, int>>();
						PIEmitter pIEmitter = mDef.mEmitterVector[pIEmitterInstanceDef.mEmitterDefIdx];
						for (int k = 0; k < pIEmitter.mParticleDefVector.Count; k++)
						{
							PIParticleDef key = pIEmitter.mParticleDefVector[k];
							PIParticleDefInstance theParticleDefInstance = pIEmitterInstance.mParticleDefInstanceVector[k];
							if (!dictionary.ContainsKey(pIEmitter))
							{
								dictionary.Add(pIEmitter, new Dictionary<PIParticleDef, int>());
							}
							if (!dictionary[pIEmitter].ContainsKey(key))
							{
								dictionary[pIEmitter].Add(key, k);
							}
							else
							{
								dictionary[pIEmitter][key] = k;
							}
							SaveParticleDefInstance(theBuffer, theParticleDefInstance);
						}
						Dictionary<PIEmitter, int> dictionary2 = new Dictionary<PIEmitter, int>();
						for (int l = 0; l < pIEmitterInstanceDef.mFreeEmitterIndices.Count; l++)
						{
							PIEmitter pIEmitter2 = mDef.mEmitterVector[pIEmitterInstanceDef.mFreeEmitterIndices[l]];
							for (int m = 0; m < pIEmitter2.mParticleDefVector.Count; m++)
							{
								PIParticleDef key2 = pIEmitter2.mParticleDefVector[m];
								if (!dictionary.ContainsKey(pIEmitter2))
								{
									dictionary.Add(pIEmitter2, new Dictionary<PIParticleDef, int>());
								}
								if (!dictionary[pIEmitter2].ContainsKey(key2))
								{
									dictionary[pIEmitter2].Add(key2, m);
								}
								else
								{
									dictionary[pIEmitter2][key2] = m;
								}
							}
							PIParticleDefInstance theParticleDefInstance2 = pIEmitterInstance.mSuperEmitterParticleDefInstanceVector[l];
							SaveParticleDefInstance(theBuffer, theParticleDefInstance2);
							dictionary2[pIEmitter2] = l;
						}
						PIFreeEmitterInstance pIFreeEmitterInstance = (PIFreeEmitterInstance)pIEmitterInstance.mSuperEmitterGroup.mHead;
						theBuffer.WriteLong(CountParticles(pIFreeEmitterInstance));
						while (pIFreeEmitterInstance != null)
						{
							theBuffer.WriteShort((short)dictionary2[pIFreeEmitterInstance.mEmitterSrc]);
							SaveParticle(theBuffer, pILayer, pIFreeEmitterInstance);
							PIEmitter mEmitterSrc = pIFreeEmitterInstance.mEmitterSrc;
							for (int n = 0; n < mEmitterSrc.mParticleDefVector.Count; n++)
							{
								PIParticleDefInstance theParticleDefInstance3 = pIFreeEmitterInstance.mEmitter.mParticleDefInstanceVector[n];
								SaveParticleDefInstance(theBuffer, theParticleDefInstance3);
							}
							PIParticleInstance pIParticleInstance = pIFreeEmitterInstance.mEmitter.mParticleGroup.mHead;
							theBuffer.WriteLong(CountParticles(pIParticleInstance));
							while (pIParticleInstance != null)
							{
								theBuffer.WriteShort((short)dictionary[pIParticleInstance.mEmitterSrc][pIParticleInstance.mParticleDef]);
								SaveParticle(theBuffer, pILayer, pIParticleInstance);
								pIParticleInstance = pIParticleInstance.mNext;
							}
							pIFreeEmitterInstance = (PIFreeEmitterInstance)pIFreeEmitterInstance.mNext;
						}
						PIParticleInstance pIParticleInstance2 = pIEmitterInstance.mParticleGroup.mHead;
						int num2 = CountParticles(pIParticleInstance2);
						theBuffer.WriteLong(num2);
						while (pIParticleInstance2 != null)
						{
							short theShort = (short)dictionary[pIParticleInstance2.mEmitterSrc][pIParticleInstance2.mParticleDef];
							theBuffer.WriteShort(theShort);
							SaveParticle(theBuffer, pILayer, pIParticleInstance2);
							pIParticleInstance2 = pIParticleInstance2.mNext;
						}
					}
				}
			}
			int num3 = theBuffer.mWriteBitPos / 8 - num - 4;
			int mWriteBitPos = theBuffer.mWriteBitPos;
			theBuffer.mWriteBitPos = num;
			theBuffer.WriteLong(num3);
			theBuffer.mWriteBitPos = mWriteBitPos;
			return true;
		}

		public void ResetAnim()
		{
			mFrameNum = 0f;
			for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
			{
				PILayerDef pILayerDef = mDef.mLayerDefVector[i];
				PILayer pILayer = mLayerVector[i];
				for (int j = 0; j < pILayer.mEmitterInstanceVector.Count; j++)
				{
					PIEmitterInstanceDef pIEmitterInstanceDef = pILayerDef.mEmitterInstanceDefVector[j];
					PIEmitterInstance pIEmitterInstance = pILayer.mEmitterInstanceVector[j];
					PIFreeEmitterInstance pIFreeEmitterInstance = (PIFreeEmitterInstance)pIEmitterInstance.mSuperEmitterGroup.mHead;
					while (pIFreeEmitterInstance != null)
					{
						PIFreeEmitterInstance pIFreeEmitterInstance2 = (PIFreeEmitterInstance)pIFreeEmitterInstance.mNext;
						PIParticleInstance pIParticleInstance = pIFreeEmitterInstance.mEmitter.mParticleGroup.mHead;
						while (pIParticleInstance != null)
						{
							PIParticleInstance mNext = pIParticleInstance.mNext;
							mParticlePool.Free(pIParticleInstance);
							pIParticleInstance = mNext;
						}
						mFreeEmitterPool.Free(pIFreeEmitterInstance);
						pIFreeEmitterInstance = pIFreeEmitterInstance2;
					}
					pIEmitterInstance.mSuperEmitterGroup.mHead = null;
					pIEmitterInstance.mSuperEmitterGroup.mTail = null;
					pIEmitterInstance.mSuperEmitterGroup.mCount = 0;
					PIParticleInstance pIParticleInstance2 = pIEmitterInstance.mParticleGroup.mHead;
					while (pIParticleInstance2 != null)
					{
						PIParticleInstance mNext2 = pIParticleInstance2.mNext;
						mParticlePool.Free(pIParticleInstance2);
						pIParticleInstance2 = mNext2;
					}
					pIEmitterInstance.mParticleGroup.mHead = null;
					pIEmitterInstance.mParticleGroup.mTail = null;
					pIEmitterInstance.mParticleGroup.mCount = 0;
					for (int k = 0; k < pIEmitterInstanceDef.mFreeEmitterIndices.Count; k++)
					{
						PIParticleDefInstance pIParticleDefInstance = pIEmitterInstance.mSuperEmitterParticleDefInstanceVector[k];
						pIParticleDefInstance.Reset();
					}
					PIEmitter pIEmitter = mDef.mEmitterVector[pIEmitterInstanceDef.mEmitterDefIdx];
					for (int l = 0; l < pIEmitter.mParticleDefVector.Count; l++)
					{
						PIParticleDefInstance pIParticleDefInstance2 = pIEmitterInstance.mParticleDefInstanceVector[l];
						pIParticleDefInstance2.Reset();
					}
					pIEmitterInstance.mWithinLifeFrame = true;
					pIEmitterInstance.mWasActive = false;
				}
			}
			mCurNumEmitters = 0;
			mCurNumParticles = 0;
			mLastDrawnPixelCount = 0;
			mWantsSRand = true;
		}

		public void Clear()
		{
			mError = "";
			ResetAnim();
			mStringVector.Clear();
			mNotesParams.Clear();
			mDef.mEmitterVector.Clear();
			mDef.mTextureVector.Clear();
			mDef.mLayerDefVector.Clear();
			mDef.mEmitterRefMap.Clear();
			mRandSeeds.Clear();
			mVersion = 0;
			mLoaded = false;
		}

		public PILayer GetLayer(int theIdx)
		{
			if (theIdx < mDef.mLayerDefVector.Count)
			{
				return mLayerVector[theIdx];
			}
			return null;
		}

		public PILayer GetLayer(string theName)
		{
			for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
			{
				if (theName.Length == 0 || mDef.mLayerDefVector[i].mName == theName)
				{
					return mLayerVector[i];
				}
			}
			return null;
		}

		public bool HasTimelineExpired()
		{
			return mFrameNum >= (float)mLastFrameNum;
		}

		public bool IsActive()
		{
			for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
			{
				PILayerDef pILayerDef = mDef.mLayerDefVector[i];
				PILayer pILayer = mLayerVector[i];
				if (!pILayer.mVisible)
				{
					continue;
				}
				for (int j = 0; j < pILayer.mEmitterInstanceVector.Count; j++)
				{
					PIEmitterInstanceDef pIEmitterInstanceDef = pILayerDef.mEmitterInstanceDefVector[j];
					PIEmitterInstance pIEmitterInstance = pILayer.mEmitterInstanceVector[j];
					if (pIEmitterInstance.mVisible)
					{
						if (pIEmitterInstanceDef.mValues[13].GetNextKeyframeTime(mFrameNum) >= mFrameNum)
						{
							return true;
						}
						if (pIEmitterInstance.mWithinLifeFrame)
						{
							return true;
						}
						if (pIEmitterInstance.mSuperEmitterGroup.mHead != null)
						{
							return true;
						}
						if (pIEmitterInstance.mParticleGroup.mHead != null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public string GetNotesParam(string theName)
		{
			return GetNotesParam(theName, "");
		}

		public string GetNotesParam(string theName, string theDefault)
		{
			if (mNotesParams.ContainsKey(theName.ToUpper()))
			{
				return mNotesParams[theName.ToUpper()];
			}
			return theDefault;
		}

		public void Update()
		{
			if (mError.Length > 0)
			{
				return;
			}
			mUpdateCnt++;
			bool flag = mFrameNum == 0f;
			if (mWantsSRand)
			{
				if (mRandSeeds.Count > 0)
				{
					mRand.SRand((uint)mRandSeeds[Common.Rand() % mRandSeeds.Count]);
				}
				else
				{
					mRand.SRand((uint)Common.Rand());
				}
				mWantsSRand = false;
			}
			if (flag && mStartupState.GetDataLen() != 0)
			{
				mStartupState.SeekFront();
				LoadState(mStartupState, true);
				mWantsSRand = false;
				return;
			}
			bool flag2 = true;
			while (mFrameNum < (float)mFirstFrameNum || flag2)
			{
				flag2 = false;
				mCurNumEmitters = 0;
				mCurNumParticles = 0;
				float num = 100f / mAnimSpeed;
				int num2 = (int)mFrameNum;
				if (flag)
				{
					mFrameNum += 0.0001f;
				}
				else
				{
					mFrameNum += (float)mFramerate / num;
				}
				mIsNewFrame = num2 != (int)mFrameNum;
				for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
				{
					PILayerDef pILayerDef = mDef.mLayerDefVector[i];
					PILayer pILayer = mLayerVector[i];
					if (!pILayer.mVisible)
					{
						continue;
					}
					for (int j = 0; j < pILayerDef.mDeflectorVector.Count; j++)
					{
						PIDeflector pIDeflector = pILayerDef.mDeflectorVector[j];
						SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
						float valueAt = pIDeflector.mAngle.GetValueAt(mFrameNum);
						if (valueAt != 0f)
						{
							sexyTransform2D.RotateDeg(valueAt);
						}
						Vector2 valueAt2 = pIDeflector.mPos.GetValueAt(mFrameNum);
						sexyTransform2D.Translate(valueAt2.X, valueAt2.Y);
						Vector2 valueAt3 = pILayerDef.mOffset.GetValueAt(mFrameNum);
						sexyTransform2D.Translate(valueAt3.X, valueAt3.Y);
						float valueAt4 = pILayerDef.mAngle.GetValueAt(mFrameNum);
						if (valueAt4 != 0f)
						{
							sexyTransform2D.RotateDeg(valueAt4);
						}
						SexyTransform2D theMatrix = mDrawTransform * sexyTransform2D;
						for (int k = 0; k < pIDeflector.mPoints.Count; k++)
						{
							pIDeflector.mCurPoints[k] = GlobalPIEffect.TransformFPoint(theMatrix, pIDeflector.mPoints[k].GetValueAt(mFrameNum));
						}
					}
					for (int l = 0; l < pILayerDef.mForceVector.Count; l++)
					{
						PIForce pIForce = pILayerDef.mForceVector[l];
						SexyTransform2D sexyTransform2D2 = new SexyTransform2D(false);
						sexyTransform2D2.Scale(pIForce.mWidth.GetValueAt(mFrameNum) / 2f, pIForce.mHeight.GetValueAt(mFrameNum) / 2f);
						float valueAt5 = pIForce.mAngle.GetValueAt(mFrameNum);
						if (valueAt5 != 0f)
						{
							sexyTransform2D2.RotateDeg(valueAt5);
						}
						Vector2 valueAt6 = pIForce.mPos.GetValueAt(mFrameNum);
						sexyTransform2D2.Translate(valueAt6.X, valueAt6.Y);
						Vector2 valueAt7 = pILayerDef.mOffset.GetValueAt(mFrameNum);
						sexyTransform2D2.Translate(valueAt7.X, valueAt7.Y);
						float valueAt8 = pILayerDef.mAngle.GetValueAt(mFrameNum);
						if (valueAt8 != 0f)
						{
							sexyTransform2D2.RotateDeg(valueAt8);
						}
						SexyTransform2D theMatrix2 = mDrawTransform * sexyTransform2D2;
						Vector2[] array = new Vector2[5]
						{
							new Vector2(-1f, -1f),
							new Vector2(1f, -1f),
							new Vector2(1f, 1f),
							new Vector2(-1f, 1f),
							new Vector2(0f, 0f)
						};
						for (int m = 0; m < 5; m++)
						{
							pIForce.mCurPoints[m] = GlobalPIEffect.TransformFPoint(theMatrix2, array[m]);
						}
					}
					for (int n = 0; n < pILayer.mEmitterInstanceVector.Count; n++)
					{
						PIEmitterInstanceDef pIEmitterInstanceDef = pILayerDef.mEmitterInstanceDefVector[n];
						PIEmitterInstance pIEmitterInstance = pILayer.mEmitterInstanceVector[n];
						int num3 = 0;
						int num4 = 0;
						int num5 = 1;
						while (pIEmitterInstance.mVisible && num5 > 0)
						{
							num3 = 0;
							num4 = 0;
							num5--;
							bool flag3 = pIEmitterInstanceDef.mValues[13].GetLastKeyframe(mFrameNum) > 0.99f;
							if (!flag3)
							{
								num5 = 0;
							}
							else if (!pIEmitterInstance.mWasActive)
							{
								num5 += (int)((float)pIEmitterInstanceDef.mFramesToPreload * num / (float)mFramerate);
							}
							pIEmitterInstance.mWasActive = flag3;
							float nextKeyframeTime = pIEmitterInstanceDef.mValues[13].GetNextKeyframeTime(0f);
							float lastKeyframeTime = pIEmitterInstanceDef.mValues[13].GetLastKeyframeTime((float)mLastFrameNum + 1f);
							float lastKeyframe = pIEmitterInstanceDef.mValues[13].GetLastKeyframe((float)mLastFrameNum + 1f);
							pIEmitterInstance.mWithinLifeFrame = mFrameNum >= nextKeyframeTime && (mFrameNum < lastKeyframeTime || lastKeyframe > 0.99f) && (mEmitAfterTimeline || mFrameNum < (float)mLastFrameNum);
							if (flag3 || (pIEmitterInstanceDef.mIsSuperEmitter && pIEmitterInstance.mWithinLifeFrame))
							{
								num3++;
							}
							if (pIEmitterInstanceDef.mIsSuperEmitter)
							{
								for (int num6 = 0; num6 < pIEmitterInstanceDef.mFreeEmitterIndices.Count; num6++)
								{
									PIEmitter theEmitter = mDef.mEmitterVector[pIEmitterInstanceDef.mFreeEmitterIndices[num6]];
									PIParticleDefInstance theParticleDefInstance = pIEmitterInstance.mSuperEmitterParticleDefInstanceVector[num6];
									UpdateParticleDef(pILayer, theEmitter, pIEmitterInstance, null, theParticleDefInstance, pIEmitterInstance.mSuperEmitterGroup, null);
								}
								UpdateParticleGroup(pILayer, pIEmitterInstance, pIEmitterInstance.mSuperEmitterGroup);
								PIFreeEmitterInstance pIFreeEmitterInstance = (PIFreeEmitterInstance)pIEmitterInstance.mSuperEmitterGroup.mHead;
								while (pIFreeEmitterInstance != null)
								{
									PIFreeEmitterInstance pIFreeEmitterInstance2 = (PIFreeEmitterInstance)pIFreeEmitterInstance.mNext;
									PIEmitter mEmitterSrc = pIFreeEmitterInstance.mEmitterSrc;
									for (int num7 = 0; num7 < mEmitterSrc.mParticleDefVector.Count; num7++)
									{
										PIParticleDef theParticleDef = mEmitterSrc.mParticleDefVector[num7];
										PIParticleDefInstance theParticleDefInstance2 = pIFreeEmitterInstance.mEmitter.mParticleDefInstanceVector[num7];
										UpdateParticleDef(pILayer, mEmitterSrc, pIEmitterInstance, theParticleDef, theParticleDefInstance2, pIFreeEmitterInstance.mEmitter.mParticleGroup, pIFreeEmitterInstance);
									}
									UpdateParticleGroup(pILayer, pIEmitterInstance, pIFreeEmitterInstance.mEmitter.mParticleGroup);
									num4 += pIFreeEmitterInstance.mEmitter.mParticleGroup.mCount;
									num3++;
									pIFreeEmitterInstance = pIFreeEmitterInstance2;
								}
							}
							else
							{
								PIEmitter pIEmitter = mDef.mEmitterVector[pIEmitterInstanceDef.mEmitterDefIdx];
								for (int num8 = 0; num8 < pIEmitter.mParticleDefVector.Count; num8++)
								{
									PIParticleGroup mParticleGroup = pIEmitterInstance.mParticleGroup;
									PIParticleDef theParticleDef2 = pIEmitter.mParticleDefVector[num8];
									PIParticleDefInstance theParticleDefInstance3 = pIEmitterInstance.mParticleDefInstanceVector[num8];
									UpdateParticleDef(pILayer, pIEmitter, pIEmitterInstance, theParticleDef2, theParticleDefInstance3, mParticleGroup, null);
								}
								UpdateParticleGroup(pILayer, pIEmitterInstance, pIEmitterInstance.mParticleGroup);
								num4 += pIEmitterInstance.mParticleGroup.mCount;
							}
						}
						mCurNumEmitters += num3;
						mCurNumParticles += num4;
					}
				}
				flag = false;
			}
		}

		public void DrawDarkenLayer(Graphics g, PILayer theLayer)
		{
			g.PushState();
			g.SetColorizeImages(true);
			PILayerDef mLayerDef = theLayer.mLayerDef;
			for (int i = 0; i < theLayer.mEmitterInstanceVector.Count; i++)
			{
				PIEmitterInstanceDef pIEmitterInstanceDef = mLayerDef.mEmitterInstanceDefVector[i];
				PIEmitterInstance pIEmitterInstance = theLayer.mEmitterInstanceVector[i];
				if (!pIEmitterInstance.mVisible)
				{
					continue;
				}
				if (pIEmitterInstanceDef.mIsSuperEmitter)
				{
					for (int j = 0; j < pIEmitterInstanceDef.mFreeEmitterIndices.Count; j++)
					{
						for (PIFreeEmitterInstance pIFreeEmitterInstance = (PIFreeEmitterInstance)pIEmitterInstance.mSuperEmitterGroup.mHead; pIFreeEmitterInstance != null; pIFreeEmitterInstance = (PIFreeEmitterInstance)pIFreeEmitterInstance.mNext)
						{
							DrawParticleGroup(g, theLayer, pIEmitterInstance, pIFreeEmitterInstance.mEmitter.mParticleGroup, true);
						}
					}
				}
				else
				{
					DrawParticleGroup(g, theLayer, pIEmitterInstance, pIEmitterInstance.mParticleGroup, true);
				}
			}
			g.PopState();
		}

		public void DrawLayer(Graphics g, PILayer theLayer)
		{
			g.PushState();
			mNormalList.Clear();
			mAdditiveList.Clear();
			PILayerDef mLayerDef = theLayer.mLayerDef;
			for (int i = 0; i < theLayer.mEmitterInstanceVector.Count; i++)
			{
				PIEmitterInstanceDef pIEmitterInstanceDef = mLayerDef.mEmitterInstanceDefVector[i];
				PIEmitterInstance pIEmitterInstance = theLayer.mEmitterInstanceVector[i];
				if (!pIEmitterInstance.mVisible)
				{
					continue;
				}
				mDarken = false;
				for (int j = 0; j < 2; j++)
				{
					if (pIEmitterInstanceDef.mIsSuperEmitter)
					{
						for (int k = 0; k < pIEmitterInstanceDef.mFreeEmitterIndices.Count; k++)
						{
							for (PIFreeEmitterInstance pIFreeEmitterInstance = (PIFreeEmitterInstance)pIEmitterInstance.mSuperEmitterGroup.mHead; pIFreeEmitterInstance != null; pIFreeEmitterInstance = (PIFreeEmitterInstance)pIFreeEmitterInstance.mNext)
							{
								DrawParticleGroup(g, theLayer, pIEmitterInstance, pIFreeEmitterInstance.mEmitter.mParticleGroup, j == 0);
							}
						}
					}
					else
					{
						DrawParticleGroup(g, theLayer, pIEmitterInstance, pIEmitterInstance.mParticleGroup, j == 0);
					}
				}
			}
			g.PopState();
		}

		public void DrawPhisycalLayer(Graphics g, PILayer theLayer)
		{
			g.PushState();
			g.SetColorizeImages(true);
			PILayerDef mLayerDef = theLayer.mLayerDef;
			g.SetDrawMode(0);
			for (int i = 0; i < mLayerDef.mBlockerVector.Count; i++)
			{
				PIBlocker pIBlocker = mLayerDef.mBlockerVector[i];
				bool flag = pIBlocker.mActive.GetLastKeyframe(mFrameNum) > 0.99f;
				if (!mDebug && !flag)
				{
					continue;
				}
				SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
				float valueAt = pIBlocker.mAngle.GetValueAt(mFrameNum);
				if (valueAt != 0f)
				{
					sexyTransform2D.RotateDeg(valueAt);
				}
				Vector2 valueAt2 = pIBlocker.mPos.GetValueAt(mFrameNum);
				sexyTransform2D.Translate(valueAt2.X, valueAt2.Y);
				Vector2 valueAt3 = mLayerDef.mOffset.GetValueAt(mFrameNum);
				sexyTransform2D.Translate(valueAt3.X, valueAt3.Y);
				float valueAt4 = mLayerDef.mAngle.GetValueAt(mFrameNum);
				if (valueAt4 != 0f)
				{
					sexyTransform2D.RotateDeg(valueAt4);
				}
				SexyTransform2D theMatrix = mDrawTransform * sexyTransform2D;
				Vector2[] array = new Vector2[512];
				int num = Math.Min(512, pIBlocker.mPoints.Count);
				for (int j = 0; j < num; j++)
				{
					array[j] = GlobalPIEffect.TransformFPoint(theMatrix, pIBlocker.mPoints[j].GetValueAt(mFrameNum));
				}
				Vector2[,] array2 = new Vector2[256, 3];
				int theNumTris = 0;
				Common.DividePoly(array, num, array2, 256, ref theNumTris);
				if (!flag)
				{
					continue;
				}
				for (int k = 0; k < theNumTris; k++)
				{
					if (theLayer.mBkgImage != null)
					{
						SexyVertex2D[] array3 = new SexyVertex2D[3];
						for (int l = 0; l < 3; l++)
						{
							array3[l] = new SexyVertex2D(array2[k, l].X, array2[k, l].Y, (array2[k, l].X + theLayer.mBkgImgDrawOfs.X) / (float)theLayer.mBkgImage.mWidth, (array2[k, l].Y + theLayer.mBkgImgDrawOfs.Y) / (float)theLayer.mBkgImage.mHeight);
						}
						g.SetColor(Color.White);
						g.DrawTriangleTex(theLayer.mBkgImage, array3[0], array3[1], array3[2]);
					}
					else
					{
						Vector2[] array4 = new Vector2[3];
						for (int m = 0; m < 3; m++)
						{
							array4[m] = array2[k, m];
						}
						g.SetColor(mBkgColor);
					}
				}
			}
			for (int n = 0; n < mLayerDef.mDeflectorVector.Count; n++)
			{
				PIDeflector pIDeflector = mLayerDef.mDeflectorVector[n];
				bool flag2 = pIDeflector.mActive.GetLastKeyframe(mFrameNum) > 0.99f;
				if ((!pIDeflector.mVisible || !flag2) && !mDebug)
				{
					continue;
				}
				if (flag2)
				{
					g.SetColor(255, 0, 0);
				}
				else
				{
					g.SetColor(64, 0, 0);
				}
				for (int num2 = 1; num2 < pIDeflector.mCurPoints.Count; num2++)
				{
					Vector2 vector = pIDeflector.mCurPoints[num2 - 1];
					Vector2 vector2 = pIDeflector.mCurPoints[num2];
					if (pIDeflector.mThickness <= 1.5f)
					{
						g.DrawLine((int)vector.X, (int)vector.Y, (int)vector2.X, (int)vector2.Y);
						continue;
					}
					SexyVector2 sexyVector = new SexyVector2(vector2.X - vector.X, vector2.Y - vector.Y).Normalize().Perp();
					Vector2 vector3 = GlobalPIEffect.TransformFPoint(thePoint: new Vector2(sexyVector.x, sexyVector.y), theMatrix: mDrawTransform);
					Vector2[] array5 = new Vector2[4]
					{
						vector + vector3 * pIDeflector.mThickness,
						vector2 + vector3 * pIDeflector.mThickness,
						vector2 - vector3 * pIDeflector.mThickness,
						vector - vector3 * pIDeflector.mThickness
					};
					for (int num3 = 0; num3 < 4; num3++)
					{
						vector = array5[num3];
						vector2 = array5[(num3 + 1) % 4];
						g.DrawLine((int)vector.X, (int)vector.Y, (int)vector2.X, (int)vector2.Y);
					}
				}
			}
			for (int num4 = 0; num4 < mLayerDef.mForceVector.Count; num4++)
			{
				PIForce pIForce = mLayerDef.mForceVector[num4];
				bool flag3 = pIForce.mActive.GetLastKeyframe(mFrameNum) > 0.99f;
				if ((pIForce.mVisible && flag3) || mDebug)
				{
					if (flag3)
					{
						g.SetColor(255, 0, 255);
					}
					else
					{
						g.SetColor(64, 0, 64);
					}
					for (int num5 = 0; num5 < 4; num5++)
					{
						Vector2 vector4 = pIForce.mCurPoints[num5];
						Vector2 vector5 = pIForce.mCurPoints[(num5 + 1) % 4];
						g.DrawLine((int)vector4.X, (int)vector4.Y, (int)vector5.X, (int)vector5.Y);
					}
					float num6 = MathHelper.ToRadians(0f - pIForce.mDirection.GetValueAt(mFrameNum)) + MathHelper.ToRadians(0f - pIForce.mAngle.GetValueAt(mFrameNum));
					Transform transform = new Transform();
					transform.RotateRad(0f - num6);
					Vector2[] array6 = new Vector2[3]
					{
						new Vector2(5f, 0f),
						new Vector2(-5f, -10f),
						new Vector2(-5f, 10f)
					};
					for (int num7 = 0; num7 < 3; num7++)
					{
						Vector2 vector6 = GlobalPIEffect.TransformFPoint(transform.GetMatrix(), array6[num7]) + pIForce.mCurPoints[4];
						Vector2 vector7 = GlobalPIEffect.TransformFPoint(transform.GetMatrix(), array6[(num7 + 1) % 3]) + pIForce.mCurPoints[4];
						g.DrawLine((int)vector6.X, (int)vector6.Y, (int)vector7.X, (int)vector7.Y);
					}
				}
			}
			g.PopState();
		}

		public void Draw(Graphics g)
		{
			mLastDrawnPixelCount = 0;
			for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
			{
				PILayer pILayer = mLayerVector[i];
				if (pILayer.mVisible)
				{
					DrawLayer(g, pILayer);
					DrawLayerNormal(g, pILayer);
					DrawLayerAdditive(g, pILayer);
					DrawPhisycalLayer(g, pILayer);
				}
			}
			mLastDrawnPixelCount *= (int)GlobalPIEffect.GetMatrixScale(mDrawTransform);
		}

		public void Draw(Graphics g, bool isDarkenise)
		{
			mLastDrawnPixelCount = 0;
			for (int i = 0; i < mDef.mLayerDefVector.Count; i++)
			{
				PILayer pILayer = mLayerVector[i];
				if (pILayer.mVisible)
				{
					if (isDarkenise)
					{
						DrawDarkenLayer(g, pILayer);
					}
					else
					{
						DrawLayer(g, pILayer);
					}
				}
			}
			mLastDrawnPixelCount *= (int)GlobalPIEffect.GetMatrixScale(mDrawTransform);
		}

		public bool CheckCache()
		{
			return true;
		}

		public bool SetCacheUpToDate()
		{
			return true;
		}

		public void WriteToCache()
		{
		}
	}
}