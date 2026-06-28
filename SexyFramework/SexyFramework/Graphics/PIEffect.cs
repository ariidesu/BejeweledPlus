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
				PIValuePoint2D aValuePoint2D = new PIValuePoint2D();
				aValuePoint2D.mValue = vector;
				aValuePoint2D.mTime = num2;
				theValue2D.mValuePoint2DVector.Add(aValuePoint2D);
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
				PIValuePoint2D aValuePoint2D = new PIValuePoint2D();
				aValuePoint2D.mTime = mReadBuffer.ReadInt32();
				aValuePoint2D.mValue.X = mReadBuffer.ReadFloat();
				aValuePoint2D.mValue.Y = mReadBuffer.ReadFloat();
				theValue2D.mValuePoint2DVector.Add(aValuePoint2D);
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
					PIValuePoint aValuePoint = theValue.mValuePointVector[i];
					aValuePoint.mValue = vector.Y;
					aValuePoint.mTime = num3;
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
				PIParticleDef aParticleDef = new PIParticleDef();
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
				aParticleDef.mIntense = mReadBuffer.ReadBoolean();
				aParticleDef.mSingleParticle = mReadBuffer.ReadBoolean();
				aParticleDef.mPreserveColor = mReadBuffer.ReadBoolean();
				aParticleDef.mAttachToEmitter = mReadBuffer.ReadBoolean();
				aParticleDef.mAttachVal = mReadBuffer.ReadFloat();
				aParticleDef.mFlipHorz = mReadBuffer.ReadBoolean();
				aParticleDef.mFlipVert = mReadBuffer.ReadBoolean();
				aParticleDef.mAnimStartOnRandomFrame = mReadBuffer.ReadBoolean();
				aParticleDef.mRepeatColor = mReadBuffer.ReadInt32();
				aParticleDef.mRepeatAlpha = mReadBuffer.ReadInt32();
				aParticleDef.mLinkTransparencyToColor = mReadBuffer.ReadBoolean();
				aParticleDef.mName = ReadString();
				aParticleDef.mAngleAlignToMotion = mReadBuffer.ReadBoolean();
				aParticleDef.mAngleRandomAlign = mReadBuffer.ReadBoolean();
				aParticleDef.mAngleKeepAlignedToMotion = mReadBuffer.ReadBoolean();
				aParticleDef.mAngleValue = mReadBuffer.ReadInt32();
				aParticleDef.mAngleAlignOffset = mReadBuffer.ReadInt32();
				aParticleDef.mAnimSpeed = mReadBuffer.ReadInt32();
				aParticleDef.mRandomGradientColor = mReadBuffer.ReadBoolean();
				mReadBuffer.ReadInt32();
				aParticleDef.mTextureIdx = mReadBuffer.ReadInt32();
				int num2 = mReadBuffer.ReadShort();
				for (int j = 0; j < num2; j++)
				{
					ExpectCmd("CColorPoint");
					byte b = mReadBuffer.ReadByte();
					byte b2 = mReadBuffer.ReadByte();
					byte b3 = mReadBuffer.ReadByte();
					ulong num3 = 0xFF000000u | ((ulong)b << 16) | ((ulong)b2 << 8) | b3;
					float mTime = mReadBuffer.ReadFloat();
					PIInterpolatorPoint aColorPoint = new PIInterpolatorPoint();
					aColorPoint.mValue = (int)num3;
					aColorPoint.mTime = mTime;
					aParticleDef.mColor.mInterpolatorPointVector.Add(aColorPoint);
				}
				int num4 = mReadBuffer.ReadShort();
				for (int k = 0; k < num4; k++)
				{
					ExpectCmd("CAlphaPoint");
					byte mValue = mReadBuffer.ReadByte();
					float mTime2 = mReadBuffer.ReadFloat();
					PIInterpolatorPoint anAlphaPoint = new PIInterpolatorPoint();
					anAlphaPoint.mValue = mValue;
					anAlphaPoint.mTime = mTime2;
					aParticleDef.mAlpha.mInterpolatorPointVector.Add(anAlphaPoint);
				}
				for (int l = 0; l < (int)PIParticleDef.PIParticleDefValue.VALUE_VISIBILITY + 1; l++)
				{
					ReadValue(ref aParticleDef.mValues[l]);
				}
				aParticleDef.mRefPointOfs.X = mReadBuffer.ReadFloat();
				aParticleDef.mRefPointOfs.Y = mReadBuffer.ReadFloat();
				if (!mIsPPF)
				{
					Image image = mDef.mTextureVector[aParticleDef.mTextureIdx].mImageVector[0].GetImage();
					aParticleDef.mRefPointOfs.X /= image.mWidth;
					aParticleDef.mRefPointOfs.Y /= image.mHeight;
				}
				mReadBuffer.ReadInt32();
				mReadBuffer.ReadInt32();
				aParticleDef.mLockAspect = mReadBuffer.ReadBoolean();
				ReadValue(ref aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_Y]);
				ReadValue(ref aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_Y_VARIATION]);
				ReadValue(ref aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_Y_OVER_LIFE]);
				aParticleDef.mAngleRange = mReadBuffer.ReadInt32();
				aParticleDef.mAngleOffset = mReadBuffer.ReadInt32();
				aParticleDef.mGetColorFromLayer = mReadBuffer.ReadBoolean();
				aParticleDef.mUpdateColorFromLayer = mReadBuffer.ReadBoolean();
				aParticleDef.mUseEmitterAngleAndRange = mReadBuffer.ReadBoolean();
				ReadValue(ref aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_EMISSION_ANGLE]);
				ReadValue(ref aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_EMISSION_RANGE]);
				mReadBuffer.ReadInt32();
				PIValue aDiscardValue = new PIValue();
				ReadValue(ref aDiscardValue);
				aParticleDef.mUseKeyColorsOnly = mReadBuffer.ReadBoolean();
				aParticleDef.mUpdateTransparencyFromLayer = mReadBuffer.ReadBoolean();
				aParticleDef.mUseNextColorKey = mReadBuffer.ReadBoolean();
				aParticleDef.mNumberOfEachColor = mReadBuffer.ReadInt32();
				aParticleDef.mGetTransparencyFromLayer = mReadBuffer.ReadBoolean();
				if (theEmitter.mOldestInFront)
				{
					theEmitter.mParticleDefVector.Insert(0, aParticleDef);
				}
				else
				{
					theEmitter.mParticleDefVector.Add(aParticleDef);
				}
			}
			mReadBuffer.ReadInt32();
			for (int aValueIdx = 0; aValueIdx < (int)PIEmitter.PIEmitterValue.NUM_VALUES; aValueIdx++)
			{
				ReadValue(ref theEmitter.mValues[aValueIdx]);
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
			float anUpdateRate = 100f / mAnimSpeed;
			float aLifeTicks = theParticle.mLife * anUpdateRate;
			theParticle.mLifePctInt = (int)(unchecked((long)((double)theParticle.mLifePct * int.MaxValue)) & 0xFFFFFFFFL);
			if (aLifeTicks > 0f)
			{
				theParticle.mLifePctIntInc = (int)(unchecked((long)((double)int.MaxValue / aLifeTicks)) & 0xFFFFFFFFL);
			}
			else
			{
				theParticle.mLifePctIntInc = 0;
			}
			if (theParticle.mLifePctInt < 0)
			{
				theParticle.mLifePctInt = int.MaxValue;
			}
			if (theParticle.mParticleDef != null && theParticle.mParticleDef.mSingleParticle)
			{
				theParticle.mLifePctInt = 1;
				theParticle.mLifePctIntInc = 0;
				theParticle.mLifePctInc = 0f;
			}
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
			return GetGeomPos(theEmitterInstance, theParticleInstance, ref theTravelAngle, ref temp, true);
		}

		public Vector2 GetGeomPos(PIEmitterInstance theEmitterInstance, PIParticleInstance theParticleInstance)
		{
			float temp = 0f;
			bool temp2 = false;
			return GetGeomPos(theEmitterInstance, theParticleInstance, ref temp, ref temp2, false);
		}

		public Vector2 GetGeomPos(PIEmitterInstance theEmitterInstance, PIParticleInstance theParticleInstance, ref float theTravelAngle, ref bool isMaskedOut)
		{
			return GetGeomPos(theEmitterInstance, theParticleInstance, ref theTravelAngle, ref isMaskedOut, true);
		}

		public Vector2 GetGeomPos(PIEmitterInstance theEmitterInstance, PIParticleInstance theParticleInstance, ref float theTravelAngle, ref bool isMaskedOut, bool wantTravelAngle)
		{
			Vector2 thePoint = default(Vector2);
			PIEmitterInstanceDef anEmitterInstanceDef = theEmitterInstance.mEmitterInstanceDef;
			switch ((PIEmitterInstanceDef.PIEmitterGEOM)anEmitterInstanceDef.mEmitterGeom)
			{
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_LINE:
			{
				if (anEmitterInstanceDef.mPoints.Count < 2)
				{
					break;
				}
				int aSegmentIdx = 0;
				float aSegmentT = 0f;
				int aTotalLength = 0;
				for (int aPtIdx = 0; aPtIdx < anEmitterInstanceDef.mPoints.Count - 1; aPtIdx++)
				{
					Vector2 aPt1 = anEmitterInstanceDef.mPoints[aPtIdx].GetValueAt(mFrameNum);
					Vector2 aPt2 = anEmitterInstanceDef.mPoints[aPtIdx + 1].GetValueAt(mFrameNum);
					Vector2 aDelta = aPt2 - aPt1;
					float aLenSq = aDelta.X * aDelta.X + aDelta.Y * aDelta.Y;
					aTotalLength += (int)aLenSq;
				}
				float aPos;
				if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					int aPtIdx = theParticleInstance.mNum % anEmitterInstanceDef.mEmitAtPointsNum;
					aPos = (float)(aPtIdx * aTotalLength) / (float)(anEmitterInstanceDef.mEmitAtPointsNum - 1);
				}
				else
				{
					aPos = GetRandFloatU() * (float)aTotalLength;
				}
				aTotalLength = 0;
				for (int aPtIdx = 0; aPtIdx < anEmitterInstanceDef.mPoints.Count - 1; aPtIdx++)
				{
					Vector2 aPt1 = anEmitterInstanceDef.mPoints[aPtIdx].GetValueAt(mFrameNum);
					Vector2 aPt2 = anEmitterInstanceDef.mPoints[aPtIdx + 1].GetValueAt(mFrameNum);
					Vector2 aDelta = aPt2 - aPt1;
					float aLenSq = aDelta.X * aDelta.X + aDelta.Y * aDelta.Y;
					if (aPos >= (float)aTotalLength && aPos <= (float)aTotalLength + aLenSq)
					{
						aSegmentT = (aPos - (float)aTotalLength) / aLenSq;
						aSegmentIdx = aPtIdx;
						break;
					}
					aTotalLength += (int)aLenSq;
				}
				Vector2 aSegStart = anEmitterInstanceDef.mPoints[aSegmentIdx].GetValueAt(mFrameNum);
				Vector2 aSegEnd = anEmitterInstanceDef.mPoints[aSegmentIdx + 1].GetValueAt(mFrameNum);
				Vector2 aSegDir = aSegEnd - aSegStart;
				thePoint = aSegStart * (1f - aSegmentT) + aSegEnd * aSegmentT;
				float aSign = ((!anEmitterInstanceDef.mEmitIn) ? 1f : (anEmitterInstanceDef.mEmitOut ? GetRandSign() : (-1f)));
				if (wantTravelAngle)
				{
					float aLineAngle = (float)Math.Atan2(aSegDir.Y, aSegDir.X) + GlobalPIEffect.M_PI / 2f + aSign * GlobalPIEffect.M_PI / 2f;
					theTravelAngle += aLineAngle;
				}
				break;
			}
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_ECLIPSE:
			{
				float anXRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_XRADIUS].GetValueAt(mFrameNum);
				float aYRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_YRADIUS].GetValueAt(mFrameNum);
				float anAngle;
				if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					int aPtIdx = theParticleInstance.mNum % anEmitterInstanceDef.mEmitAtPointsNum;
					anAngle = (float)aPtIdx * GlobalPIEffect.M_PI * 2f / (float)anEmitterInstanceDef.mEmitAtPointsNum;
					if (anAngle > GlobalPIEffect.M_PI)
					{
						anAngle -= GlobalPIEffect.M_PI * 2f;
					}
				}
				else
				{
					anAngle = GetRandFloat() * GlobalPIEffect.M_PI;
				}
				if (anXRadius > aYRadius)
				{
					float aPower = 1f + (anXRadius / aYRadius - 1f) * 0.3f;
					anAngle = ((anAngle < (0f - GlobalPIEffect.M_PI) / 2f)
						? ((float)((double)GlobalPIEffect.M_PI + Math.Pow((anAngle + GlobalPIEffect.M_PI) / (GlobalPIEffect.M_PI / 2f), aPower) * (double)GlobalPIEffect.M_PI / 2.0))
						: ((anAngle < 0f)
							? ((float)((0.0 - Math.Pow((0f - anAngle) / (GlobalPIEffect.M_PI / 2f), aPower)) * (double)GlobalPIEffect.M_PI / 2.0))
							: ((!(anAngle < GlobalPIEffect.M_PI / 2f))
								? ((float)((double)GlobalPIEffect.M_PI - Math.Pow((GlobalPIEffect.M_PI - anAngle) / (GlobalPIEffect.M_PI / 2f), aPower) * (double)GlobalPIEffect.M_PI / 2.0))
								: ((float)(Math.Pow(anAngle / (GlobalPIEffect.M_PI / 2f), aPower) * (double)GlobalPIEffect.M_PI / 2.0)))));
				}
				else if (aYRadius > anXRadius)
				{
					float aPower = 1f + (aYRadius / anXRadius - 1f) * 0.3f;
					anAngle = ((anAngle < (0f - GlobalPIEffect.M_PI) / 2f)
						? ((float)((double)((0f - GlobalPIEffect.M_PI) / 2f) - Math.Pow(((0f - GlobalPIEffect.M_PI) / 2f - anAngle) / (GlobalPIEffect.M_PI / 2f), aPower) * (double)GlobalPIEffect.M_PI / 2.0))
						: ((anAngle < 0f)
							? ((float)((double)((0f - GlobalPIEffect.M_PI) / 2f) + Math.Pow((anAngle + GlobalPIEffect.M_PI / 2f) / (GlobalPIEffect.M_PI / 2f), aPower) * (double)GlobalPIEffect.M_PI / 2.0))
							: ((!(anAngle < GlobalPIEffect.M_PI / 2f))
								? ((float)((double)(GlobalPIEffect.M_PI / 2f) + Math.Pow((anAngle - GlobalPIEffect.M_PI / 2f) / (GlobalPIEffect.M_PI / 2f), aPower) * (double)GlobalPIEffect.M_PI / 2.0))
								: ((float)((double)(GlobalPIEffect.M_PI / 2f) - Math.Pow((GlobalPIEffect.M_PI / 2f - anAngle) / (GlobalPIEffect.M_PI / 2f), aPower) * (double)GlobalPIEffect.M_PI / 2.0)))));
				}
				thePoint = new Vector2((float)(Math.Cos(anAngle) * (double)anXRadius), (float)(Math.Sin(anAngle) * (double)aYRadius));
				if (wantTravelAngle)
				{
					float aSign = ((!anEmitterInstanceDef.mEmitIn) ? 1f : (anEmitterInstanceDef.mEmitOut ? GetRandSign() : (-1f)));
					float aTravelDir = anAngle + aSign * GlobalPIEffect.M_PI / 2f;
					theTravelAngle += aTravelDir;
				}
				break;
			}
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_CIRCLE:
			{
				float aRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_XRADIUS].GetValueAt(mFrameNum);
				float anAngle;
				if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					int aPtIdx = theParticleInstance.mNum % anEmitterInstanceDef.mEmitAtPointsNum;
					anAngle = (float)aPtIdx * GlobalPIEffect.M_PI * 2f / (float)anEmitterInstanceDef.mEmitAtPointsNum;
				}
				else
				{
					anAngle = GetRandFloat() * GlobalPIEffect.M_PI;
				}
				thePoint = new Vector2((float)Math.Cos(anAngle) * aRadius, (float)Math.Sin(anAngle) * aRadius);
				if (wantTravelAngle)
				{
					float aSign = ((!anEmitterInstanceDef.mEmitIn) ? 1f : (anEmitterInstanceDef.mEmitOut ? GetRandSign() : (-1f)));
					float aTravelDir = anAngle + aSign * GlobalPIEffect.M_PI / 2f;
					theTravelAngle += aTravelDir;
				}
				break;
			}
			case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_AREA:
			{
				float aWidth = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_XRADIUS].GetValueAt(mFrameNum);
				float aHeight = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_YRADIUS].GetValueAt(mFrameNum);
				if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
				{
					float aXIdx = theParticleInstance.mNum % anEmitterInstanceDef.mEmitAtPointsNum;
					float aYIdx = theParticleInstance.mNum / anEmitterInstanceDef.mEmitAtPointsNum % anEmitterInstanceDef.mEmitAtPointsNum2;
					if (anEmitterInstanceDef.mEmitAtPointsNum > 1)
					{
						thePoint.X = (float)((double)(aXIdx / (float)(anEmitterInstanceDef.mEmitAtPointsNum - 1)) - 0.5) * aWidth;
					}
					if (anEmitterInstanceDef.mEmitAtPointsNum2 > 1)
					{
						thePoint.Y = (float)((double)(aYIdx / (float)(anEmitterInstanceDef.mEmitAtPointsNum2 - 1)) - 0.5) * aHeight;
					}
				}
				else
				{
					thePoint = new Vector2(GetRandFloat() * aWidth / 2f, GetRandFloat() * aHeight / 2f);
				}
				if (theEmitterInstance.mMaskImage.GetDeviceImage() != null)
				{
					float aNormX = thePoint.X / aWidth + 0.5f;
					float aNormY = thePoint.Y / aHeight + 0.5f;
					int aMaskW = theEmitterInstance.mMaskImage.mWidth;
					int aMaskH = theEmitterInstance.mMaskImage.mHeight;
					int aMaskX = Math.Min((int)(aNormX * (float)aMaskW), aMaskW - 1);
					int aMaskY = Math.Min((int)(aNormY * (float)aMaskH), aMaskH - 1);
					uint[] bits = theEmitterInstance.mMaskImage.GetDeviceImage().GetBits();
					uint aPixel = bits[aMaskX + aMaskY * aMaskW];
					if (((aPixel & 0x80000000u) == 0) ^ anEmitterInstanceDef.mInvertMask)
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
			Vector2 aPos = theEmitterInstance.mEmitterInstanceDef.mPosition.GetValueAt(mFrameNum);
			if (doTransform)
			{
				aPos = GlobalPIEffect.TransformFPoint(theEmitterInstance.mTransform, aPos);
				aPos = GlobalPIEffect.TransformFPoint(mEmitterTransform, aPos);
				aPos += theEmitterInstance.mOffset;
			}
			return aPos;
		}

		public int CountParticles(PIParticleInstance theStart)
		{
			int aCount = 0;
			while (theStart != null)
			{
				aCount++;
				theStart = theStart.mNext;
			}
			return aCount;
		}

		public void CalcParticleTransform(PILayer theLayer, PIEmitterInstance theEmitterInstance, PIEmitter theEmitter, PIParticleDef theParticleDef, PIParticleGroup theParticleGroup, PIParticleInstance theParticleInstance)
		{
			float aLifePct = theParticleInstance.mLifePct;
			float aScaleX = 1f;
			float aScaleY = 1f;
			float aRefXScale = 1f;
			float aRefYScale = 1f;
			Rect aSrcRect = Rect.ZERO_RECT;
			if (theParticleDef != null)
			{
				PITexture aTexture = mDef.mTextureVector[theParticleDef.mTextureIdx];
				if (aTexture.mImageVector.Count != 0)
				{
					DeviceImage anImage = aTexture.mImageVector[theParticleInstance.mImgIdx].GetDeviceImage();
					aSrcRect = new Rect(0, 0, anImage.mWidth, anImage.mHeight);
				}
				else
				{
					DeviceImage anImage = aTexture.mImageStrip.GetDeviceImage();
					if (anImage == null)
					{
						aTexture.mImageStrip = GetImage(aTexture.mName, aTexture.mFileName);
						anImage = aTexture.mImageStrip.GetDeviceImage();
					}
					aSrcRect = anImage.GetCelRect(theParticleInstance.mImgIdx);
					if (aTexture.mPadded)
					{
						aSrcRect.mX++;
						aSrcRect.mWidth -= 2;
						aSrcRect.mY++;
						aSrcRect.mHeight -= 2;
					}
				}
				if (theParticleDef.mSingleParticle)
				{
					theParticleInstance.mSrcSizeXMult = (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_SIZE_X].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_SIZE_X].GetValueAt(mFrameNum))
						* (theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_X].GetValueAt(mFrameNum) + theParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_X]);
					theParticleInstance.mSrcSizeYMult = (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_SIZE_Y].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_SIZE_Y].GetValueAt(mFrameNum))
						* (theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_Y].GetValueAt(mFrameNum) + theParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_Y]);
				}
				float aSizeX = Math.Max(theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_X_OVER_LIFE].GetValueAt(aLifePct) * theParticleInstance.mSrcSizeXMult, 0.1f);
				float aSizeY = Math.Max(theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_Y_OVER_LIFE].GetValueAt(aLifePct) * theParticleInstance.mSrcSizeYMult, 0.1f);
				int aScaleRef = Math.Max(aSrcRect.mWidth, aSrcRect.mHeight);
				aRefXScale = (float)aScaleRef / (float)aSrcRect.mWidth;
				aRefYScale = (float)aScaleRef / (float)aSrcRect.mHeight;
				aScaleX = aSizeX / (float)aScaleRef * 2f;
				aScaleY = aSizeY / (float)aScaleRef * 2f;
			}
			SexyTransform2D aBaseRotTrans = new SexyTransform2D(false);
			float anEmitterRot = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ANGLE].GetValueAt(mFrameNum);
			if (anEmitterRot != 0f)
			{
				aBaseRotTrans.RotateDeg(anEmitterRot);
			}
			if (theParticleInstance.mParentFreeEmitter != null && theParticleInstance.mParentFreeEmitter.mImgAngle != 0f)
			{
				aBaseRotTrans.RotateRad(0f - theParticleInstance.mParentFreeEmitter.mImgAngle);
			}
			SexyTransform2D aTransform = new SexyTransform2D(false);
			float aScaleFactor = 1f;
			if (theParticleDef != null)
			{
				aTransform.Translate((0f - theParticleDef.mRefPointOfs.X) * aRefXScale * (float)aSrcRect.mWidth, (0f - theParticleDef.mRefPointOfs.Y) * aRefYScale * (float)aSrcRect.mHeight);
				if (theParticleDef.mFlipHorz)
				{
					aTransform.Scale(-1f, 1f);
				}
				if (theParticleDef.mFlipVert)
				{
					aTransform.Scale(1f, -1f);
				}
			}
			float aRot = 0f;
			aScaleFactor *= aScaleX * aScaleY;
			if (aScaleX != 1f || aScaleY != 1f)
			{
				aTransform.Scale(aScaleX, aScaleY);
			}
			if (theParticleInstance.mImgAngle != 0f)
			{
				aRot += theParticleInstance.mImgAngle;
			}
			if (theParticleDef != null && theParticleDef.mAttachToEmitter)
			{
				float anAttachRot = ((theParticleInstance.mParentFreeEmitter == null)
					? (MathHelper.ToRadians(theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ANGLE].GetValueAt(mFrameNum)) * theParticleDef.mAttachVal)
					: ((theParticleInstance.mParentFreeEmitter.mImgAngle - theParticleInstance.mOrigEmitterAng) * theParticleDef.mAttachVal));
				if (anAttachRot != 0f)
				{
					aRot += anAttachRot;
				}
			}
			if (theParticleDef != null && theParticleDef.mSingleParticle && (!theParticleDef.mAngleKeepAlignedToMotion || theParticleDef.mAttachToEmitter))
			{
				aRot += MathHelper.ToRadians(theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ANGLE].GetValueAt(mFrameNum));
			}
			aTransform.RotateRad(aRot);
			Vector2 aParticlePos = theParticleInstance.mPos;
			if (theParticleDef != null && theParticleDef.mAttachToEmitter)
			{
				SexyTransform2D aBackTrans = new SexyTransform2D(false);
				aBackTrans.RotateRad(theParticleInstance.mOrigEmitterAng);
				Vector2 aBackPoint = aBackTrans * aParticlePos;
				Vector2 aCurRotPos = aBaseRotTrans * aBackPoint;
				aParticlePos = aParticlePos * (1f - theParticleDef.mAttachVal) + aCurRotPos * theParticleDef.mAttachVal;
			}
			aTransform.Translate(aParticlePos.X, aParticlePos.Y);
			if (theParticleDef != null && theParticleDef.mSingleParticle)
			{
				theParticleInstance.mZoom = (theParticleGroup.mWasEmitted
						? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_ZOOM].GetValueAt(mFrameNum)
						: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ZOOM].GetValueAt(mFrameNum))
					* theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_ZOOM].GetValueAt(mFrameNum, 1f);
			}
			aScaleFactor *= theParticleInstance.mZoom * theParticleInstance.mZoom;
			if (theParticleInstance.mZoom != 1f)
			{
				aTransform.Scale(theParticleInstance.mZoom, theParticleInstance.mZoom);
			}
			Vector2 anEmitterPos = theParticleInstance.mEmittedPos;
			if (theParticleDef != null && theParticleDef.mSingleParticle)
			{
				Vector2 aCurEmitPos = aBaseRotTrans * theParticleInstance.mOrigPos;
				aCurEmitPos += GetEmitterPos(theEmitterInstance, !theParticleGroup.mWasEmitted);
				anEmitterPos = aCurEmitPos;
			}
			else if (theParticleDef != null && theParticleDef.mAttachToEmitter && !theParticleGroup.mIsSuperEmitter)
			{
				Vector2 aCurEmitPos;
				if (theParticleInstance.mParentFreeEmitter != null)
				{
					aCurEmitPos = theParticleInstance.mParentFreeEmitter.mLastEmitterPos + theParticleInstance.mParentFreeEmitter.mOrigPos + theParticleInstance.mParentFreeEmitter.mPos;
				}
				else
				{
					aCurEmitPos = GlobalPIEffect.TransformFPoint(aBaseRotTrans, theParticleInstance.mOrigPos);
					aCurEmitPos += GetEmitterPos(theEmitterInstance, !theParticleGroup.mWasEmitted);
				}
				anEmitterPos = anEmitterPos * (1f - theParticleDef.mAttachVal) + aCurEmitPos * theParticleDef.mAttachVal;
			}
			theParticleInstance.mLastEmitterPos = anEmitterPos;
			aTransform.Translate(anEmitterPos.X, anEmitterPos.Y);
			Vector2 anOffset = theLayer.mLayerDef.mOffset.GetValueAt(mFrameNum) - theLayer.mLayerDef.mOrigOffset;
			aTransform.Translate(anOffset.X, anOffset.Y);
			float aLayerAngle = theLayer.mLayerDef.mAngle.GetValueAt(mFrameNum);
			if (aLayerAngle != 0f)
			{
				aTransform.RotateDeg(aLayerAngle);
			}
			theParticleInstance.mTransform = aTransform;
			theParticleInstance.mTransformScaleFactor = aScaleFactor;
		}

		public void UpdateParticleDef(PILayer theLayer, PIEmitter theEmitter, PIEmitterInstance theEmitterInstance, PIParticleDef theParticleDef, PIParticleDefInstance theParticleDefInstance, PIParticleGroup theParticleGroup, PIFreeEmitterInstance theFreeEmitter)
		{
			PIEmitterInstanceDef anEmitterInstanceDef = theEmitterInstance.mEmitterInstanceDef;
			float anUpdateRate = 100f / mAnimSpeed;
			float anEmitterLifePct = 0f;
			if (theFreeEmitter != null)
			{
				anEmitterLifePct = theFreeEmitter.mLifePct;
			}
			if (theParticleDefInstance.mTicks % 25 == 0 && !theParticleGroup.mIsSuperEmitter)
			{
				if (theParticleDefInstance.mTicks == 0)
				{
					theParticleDefInstance.mCurNumberVariation = GetRandFloat() * 0.5f * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_NUMBER_VARIATION].GetValueAt(mFrameNum) / 2f;
				}
				else
				{
					theParticleDefInstance.mCurNumberVariation = GetRandFloat() * 0.75f * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_NUMBER_VARIATION].GetValueAt(mFrameNum) / 2f;
				}
			}
			theParticleDefInstance.mTicks++;
			float aNumber;
			if (theParticleGroup.mIsSuperEmitter)
			{
				aNumber = theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_NUMBER].GetValueAt(mFrameNum)
					* (theParticleGroup.mWasEmitted
						? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_NUMBER].GetValueAt(mFrameNum)
						: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_NUMBER].GetValueAt(mFrameNum));
			}
			else
			{
				aNumber = (theParticleGroup.mWasEmitted
						? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_NUMBER].GetValueAt(mFrameNum)
						: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_NUMBER].GetValueAt(mFrameNum))
					* (theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_NUMBER].GetValueAt(mFrameNum) + theParticleDefInstance.mCurNumberVariation)
					* theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_NUMBER_OVER_LIFE].GetValueAt(anEmitterLifePct, 1f);
				aNumber = Math.Max(0f, aNumber);
				if (theParticleGroup.mWasEmitted && anEmitterLifePct >= 1f)
				{
					aNumber = 0f;
				}
			}
			aNumber *= theEmitterInstance.mNumberScale;
			if (theParticleGroup.mIsSuperEmitter)
			{
				aNumber *= 30f;
			}
			else if (!theParticleGroup.mWasEmitted)
			{
				switch ((PIEmitterInstanceDef.PIEmitterGEOM)anEmitterInstanceDef.mEmitterGeom)
				{
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_LINE:
				{
					if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						aNumber *= (float)anEmitterInstanceDef.mEmitAtPointsNum;
						break;
					}
					int aTotalLength = 0;
					for (int aPtIdx = 0; aPtIdx < anEmitterInstanceDef.mPoints.Count - 1; aPtIdx++)
					{
						Vector2 aPt1 = anEmitterInstanceDef.mPoints[aPtIdx].GetValueAt(mFrameNum);
						Vector2 aPt2 = anEmitterInstanceDef.mPoints[aPtIdx + 1].GetValueAt(mFrameNum);
						Vector2 aDelta = aPt2 - aPt1;
						float aLen = (float)Math.Sqrt(aDelta.X * aDelta.X + aDelta.Y * aDelta.Y);
						aTotalLength += (int)aLen;
					}
					aNumber *= (float)aTotalLength / 35f;
					break;
				}
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_ECLIPSE:
				{
					float anXRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_XRADIUS].GetValueAt(mFrameNum);
					float aYRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_YRADIUS].GetValueAt(mFrameNum);
					if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						aNumber *= (float)anEmitterInstanceDef.mEmitAtPointsNum;
						break;
					}
					float aCircumference = 6.28318f * (float)Math.Sqrt((anXRadius * anXRadius + aYRadius * aYRadius) / 2f);
					aNumber *= aCircumference / 35f;
					break;
				}
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_CIRCLE:
				{
					float aRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_XRADIUS].GetValueAt(mFrameNum);
					if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						aNumber *= (float)anEmitterInstanceDef.mEmitAtPointsNum;
						break;
					}
					float aCircumference = 6.28318f * (float)Math.Sqrt(aRadius * aRadius);
					aNumber *= aCircumference / 35f;
					break;
				}
				case PIEmitterInstanceDef.PIEmitterGEOM.GEOM_AREA:
				{
					if (anEmitterInstanceDef.mEmitAtPointsNum != 0)
					{
						aNumber *= (float)(anEmitterInstanceDef.mEmitAtPointsNum * anEmitterInstanceDef.mEmitAtPointsNum2);
						break;
					}
					float anXRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_XRADIUS].GetValueAt(mFrameNum);
					float aYRadius = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_YRADIUS].GetValueAt(mFrameNum);
					aNumber *= 1f + anXRadius * aYRadius / 900f / 4f;
					break;
				}
				}
			}

			theParticleDefInstance.mNumberAcc += aNumber / anUpdateRate;
			if ((!anEmitterInstanceDef.mIsSuperEmitter && !theEmitterInstance.mWasActive) || !theEmitterInstance.mWithinLifeFrame)
			{
				theParticleDefInstance.mNumberAcc = 0f;
			}
			bool wantsGeomPos = true;
			if (!theParticleGroup.mIsSuperEmitter && theParticleDef.mSingleParticle)
			{
				int aTargetCount = ((anEmitterInstanceDef.mEmitterGeom == 1 || anEmitterInstanceDef.mEmitterGeom == 4)
					? anEmitterInstanceDef.mEmitAtPointsNum
					: ((anEmitterInstanceDef.mEmitterGeom != 3) ? 1 : (anEmitterInstanceDef.mEmitAtPointsNum * anEmitterInstanceDef.mEmitAtPointsNum2)));
				if (aTargetCount == 0)
				{
					wantsGeomPos = false;
					aTargetCount = 1;
				}
				int aCurrentCount = 0;
				for (PIParticleInstance aPI = theParticleGroup.mHead; aPI != null; aPI = aPI.mNext)
				{
					if (aPI.mParticleDef == theParticleDef)
					{
						aCurrentCount++;
					}
				}
				theParticleDefInstance.mNumberAcc = aTargetCount - aCurrentCount;
			}
			while (theParticleDefInstance.mNumberAcc >= 1f)
			{
				theParticleDefInstance.mNumberAcc -= 1f;
				PIParticleInstance aParticleInstance;
				if (theParticleGroup.mIsSuperEmitter)
				{
					PIFreeEmitterInstance aFreeEmitterInstance = mFreeEmitterPool.Alloc();
					aFreeEmitterInstance.Reset();
					Common.Resize(aFreeEmitterInstance.mEmitter.mParticleDefInstanceVector, theEmitter.mParticleDefVector.Count);
					aParticleInstance = aFreeEmitterInstance;
				}
				else
				{
					aParticleInstance = mParticlePool.Alloc();
					aParticleInstance.Reset();
				}
				aParticleInstance.mParticleDef = theParticleDef;
				aParticleInstance.mEmitterSrc = theEmitter;
				aParticleInstance.mParentFreeEmitter = theFreeEmitter;
				aParticleInstance.mNum = theParticleDefInstance.mParticlesEmitted++;
				float aTravelAngle;
				if (theParticleGroup.mIsSuperEmitter)
				{
					aTravelAngle = (theParticleGroup.mWasEmitted
						? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_EMISSION_ANGLE].GetValueAt(mFrameNum)
						: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_EMISSION_ANGLE].GetValueAt(mFrameNum))
						+ (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_EMISSION_RANGE].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_EMISSION_RANGE].GetValueAt(mFrameNum)) * GetRandFloat() / 2f;
				}
				else if (!theParticleDef.mUseEmitterAngleAndRange)
				{
					aTravelAngle = theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_EMISSION_ANGLE].GetValueAt(mFrameNum)
						+ theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_EMISSION_RANGE].GetValueAt(mFrameNum) * GetRandFloat() / 2f;
				}
				else if (!theParticleGroup.mWasEmitted)
				{
					aTravelAngle = (theParticleGroup.mWasEmitted
						? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_EMISSION_ANGLE].GetValueAt(mFrameNum)
						: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_EMISSION_ANGLE].GetValueAt(mFrameNum))
						+ (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_EMISSION_RANGE].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_EMISSION_RANGE].GetValueAt(mFrameNum)) * GetRandFloat() / 2f;
				}
				else
				{
					aTravelAngle = theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_EMISSION_ANGLE].GetValueAt(mFrameNum)
						+ theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_EMISSION_RANGE].GetValueAt(mFrameNum) * GetRandFloat() / 2f;
				}
				aTravelAngle = MathHelper.ToRadians(0f - aTravelAngle);
				float anEmitterAngle = theFreeEmitter?.mImgAngle ?? MathHelper.ToRadians(0f - theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ANGLE].GetValueAt(mFrameNum));
				aTravelAngle += anEmitterAngle;
				aParticleInstance.mOrigEmitterAng = anEmitterAngle;
				if (theParticleDef != null && theParticleDef.mAnimStartOnRandomFrame)
				{
					aParticleInstance.mAnimFrameRand = (int)(mRand.Next() & 0x7FFF);
				}
				else
				{
					aParticleInstance.mAnimFrameRand = 0;
				}
				aParticleInstance.mZoom = (theParticleGroup.mWasEmitted
						? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_ZOOM].GetValueAt(mFrameNum)
						: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ZOOM].GetValueAt(mFrameNum))
					* theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_ZOOM].GetValueAt(mFrameNum, 1f);
				if (!theParticleGroup.mIsSuperEmitter)
				{
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_LIFE] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_LIFE_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_X] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_X_VARIATION].GetValueAt(mFrameNum);
					if (theParticleDef == null || theParticleDef.mLockAspect)
					{
						aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_Y] = aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_X];
					}
					else
					{
						aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_Y] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_Y_VARIATION].GetValueAt(mFrameNum);
					}
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_VELOCITY] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_VELOCITY_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_WEIGHT] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_WEIGHT_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SPIN] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SPIN_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_MOTION_RAND] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_MOTION_RAND_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_BOUNCE] = GetVariationScalar() * theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_BOUNCE_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mSrcSizeXMult = (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_SIZE_X].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_SIZE_X].GetValueAt(mFrameNum))
						* (theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_X].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_X]);
					aParticleInstance.mSrcSizeYMult = (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_SIZE_Y].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_SIZE_Y].GetValueAt(mFrameNum))
						* (theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SIZE_Y].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_Y]);
					if (theParticleGroup.mWasEmitted)
					{
						aParticleInstance.mSrcSizeXMult *= (1f + theFreeEmitter.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_X]) * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SIZE_X_OVER_LIFE].GetValueAt(anEmitterLifePct, 1f);
						aParticleInstance.mSrcSizeYMult *= (1f + theFreeEmitter.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_Y]) * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SIZE_X_OVER_LIFE].GetValueAt(anEmitterLifePct, 1f);
						aParticleInstance.mZoom *= (1f + theFreeEmitter.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_ZOOM]) * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_ZOOM_OVER_LIFE].GetValueAt(anEmitterLifePct, 1f);
					}
				}
				else
				{
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_LIFE] = GetVariationScalar() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_LIFE_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_X] = GetRandFloat() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SIZE_X_VARIATION].GetValueAt(mFrameNum);
					if (theParticleDef == null || theParticleDef.mLockAspect)
					{
						aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_Y] = aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_X];
					}
					else
					{
						aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SIZE_Y] = GetRandFloat() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SIZE_Y_VARIATION].GetValueAt(mFrameNum);
					}
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_VELOCITY] = GetVariationScalar() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_VELOCITY_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_WEIGHT] = GetVariationScalar() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_WEIGHT_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SPIN] = GetVariationScalar() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SPIN_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_MOTION_RAND] = GetVariationScalar() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_MOTION_RAND_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_BOUNCE] = GetVariationScalar() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_BOUNCE_VARIATION].GetValueAt(mFrameNum);
					aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_ZOOM] = GetVariationScalar() * theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_ZOOM_VARIATION].GetValueAt(mFrameNum);
				}
				float aBaseAngle = aTravelAngle;
				aParticleInstance.mGradientRand = GetRandFloatU();
				aParticleInstance.mTicks = 0f;
				aParticleInstance.mThicknessHitVariation = GetRandFloat();
				aParticleInstance.mImgAngle = 0f;
				if (theParticleGroup.mIsSuperEmitter)
				{
					aParticleInstance.mLife = (theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_LIFE].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_LIFE]) * 5f
						* (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_LIFE].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_LIFE].GetValueAt(mFrameNum));
				}
				else
				{
					aParticleInstance.mLife = (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_LIFE].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_LIFE].GetValueAt(mFrameNum))
						* (theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_LIFE].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_LIFE]);
				}
				if (aParticleInstance.mLife <= 1E-08f)
				{
					aParticleInstance.mLifePct = 1f;
				}
				float aLifeTicks = aParticleInstance.mLife * anUpdateRate;
				if (aLifeTicks > 0f)
				{
					aParticleInstance.mLifePctInc = 1f / aLifeTicks;
					aParticleInstance.mLifePctIntInc = (int)(unchecked((long)((double)int.MaxValue / aLifeTicks)) & 0xFFFFFFFFL);
				}
				else
				{
					aParticleInstance.mLifePctInc = 0f;
					aParticleInstance.mLifePctIntInc = 0;
				}
				aParticleInstance.mLifePctInt = 0;
				if (theParticleDef != null && theParticleDef.mSingleParticle)
				{
					aParticleInstance.mLifePctInt = 1;
					aParticleInstance.mLifePctIntInc = 0;
					aParticleInstance.mLifePctInc = 0f;
				}
				Vector2 aGeomOffset = default(Vector2);
				if (theParticleGroup.mWasEmitted)
				{
					aParticleInstance.mEmittedPos = theFreeEmitter.mLastEmitterPos + theFreeEmitter.mPos;
					aParticleInstance.mLastEmitterPos = aParticleInstance.mEmittedPos;
				}
				else
				{
					aParticleInstance.mEmittedPos = GetEmitterPos(theEmitterInstance, true);
					aParticleInstance.mLastEmitterPos = aParticleInstance.mEmittedPos;
					bool isMaskedOut = false;
					if (wantsGeomPos)
					{
						aGeomOffset = GetGeomPos(theEmitterInstance, aParticleInstance, ref aBaseAngle, ref isMaskedOut) - aParticleInstance.mEmittedPos;
					}
					if (isMaskedOut)
					{
						continue;
					}
				}
				aParticleInstance.mVel = new Vector2((float)Math.Cos(aBaseAngle), (float)Math.Sin(aBaseAngle));
				if (theParticleGroup.mIsSuperEmitter)
				{
					aParticleInstance.mVel = aParticleInstance.mVel
						* ((theParticleGroup.mWasEmitted
								? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_VELOCITY].GetValueAt(mFrameNum)
								: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_VELOCITY].GetValueAt(mFrameNum))
							* (theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_VELOCITY].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_VELOCITY]))
						* 160f;
				}
				else
				{
					aParticleInstance.mVel *= (theParticleGroup.mWasEmitted
							? theEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_VELOCITY].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_VELOCITY].GetValueAt(mFrameNum))
						* (theParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_VELOCITY].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_VELOCITY]);
				}
				if (!theParticleGroup.mIsSuperEmitter)
				{
					if (theParticleDef.mAngleAlignToMotion)
					{
						if (aParticleInstance.mVel.Length() == 0f)
						{
							aBaseAngle = 0f;
							if (Math.Cos(aBaseAngle) > 0.0)
							{
								aParticleInstance.mImgAngle = 0f;
							}
							else
							{
								aParticleInstance.mImgAngle = GlobalPIEffect.M_PI;
							}
							if (theParticleDef.mSingleParticle && theParticleDef.mAngleKeepAlignedToMotion && !theParticleDef.mAttachToEmitter)
							{
								aParticleInstance.mImgAngle += MathHelper.ToRadians(theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ANGLE].GetValueAt(mFrameNum));
							}
						}
						else
						{
							aParticleInstance.mImgAngle = 0f - aBaseAngle;
						}
						aParticleInstance.mImgAngle += MathHelper.ToRadians(-theParticleDef.mAngleAlignOffset);
					}
					else if (theParticleDef.mAngleRandomAlign)
					{
						aParticleInstance.mImgAngle = MathHelper.ToRadians(0f - ((float)theParticleDef.mAngleOffset + GetRandFloat() * (float)theParticleDef.mAngleRange / 2f));
					}
					else
					{
						aParticleInstance.mImgAngle = MathHelper.ToRadians(-theParticleDef.mAngleValue);
					}
				}
				aParticleInstance.mOrigPos = aGeomOffset;
				SexyTransform2D aTransform = new SexyTransform2D(false);
				aTransform.RotateDeg(theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ANGLE].GetValueAt(mFrameNum));
				aParticleInstance.mEmittedPos += GlobalPIEffect.TransformFPoint(aTransform, aGeomOffset);
				if (theEmitter.mOldestInFront)
				{
					if (theParticleGroup.mHead != null)
					{
						theParticleGroup.mHead.mPrev = aParticleInstance;
					}
					aParticleInstance.mNext = theParticleGroup.mHead;
					if (theParticleGroup.mTail == null)
					{
						theParticleGroup.mTail = aParticleInstance;
					}
					theParticleGroup.mHead = aParticleInstance;
				}
				else
				{
					if (theParticleGroup.mTail != null)
					{
						theParticleGroup.mTail.mNext = aParticleInstance;
					}
					aParticleInstance.mPrev = theParticleGroup.mTail;
					if (theParticleGroup.mHead == null)
					{
						theParticleGroup.mHead = aParticleInstance;
					}
					theParticleGroup.mTail = aParticleInstance;
				}
				theParticleGroup.mCount++;
			}
		}

		public void FreeParticle(PIParticleInstance theParticleInstance, PIParticleGroup theParticleGroup)
		{
			if (theParticleGroup.mIsSuperEmitter)
			{
				mFreeEmitterPool.Free((PIFreeEmitterInstance)theParticleInstance);
			}
			else
			{
				mParticlePool.Free(theParticleInstance);
			}
			if (theParticleInstance.mPrev != null)
			{
				theParticleInstance.mPrev.mNext = theParticleInstance.mNext;
			}
			if (theParticleInstance.mNext != null)
			{
				theParticleInstance.mNext.mPrev = theParticleInstance.mPrev;
			}
			if (theParticleGroup.mHead == theParticleInstance)
			{
				theParticleGroup.mHead = theParticleInstance.mNext;
			}
			if (theParticleGroup.mTail == theParticleInstance)
			{
				theParticleGroup.mTail = theParticleInstance.mPrev;
			}
			theParticleGroup.mCount--;
		}

		public void UpdateParticleGroup(PILayer theLayer, PIEmitterInstance theEmitterInstance, PIParticleGroup theParticleGroup)
		{
			float anUpdateRate = 100f / mAnimSpeed;
			PIParticleInstance aParticleInstance = theParticleGroup.mHead;
			PILayerDef aLayerDef = theLayer.mLayerDef;
			PIEmitterInstanceDef anEmitterInstanceDef = theEmitterInstance.mEmitterInstanceDef;
			while (aParticleInstance != null)
			{
				PIParticleInstance aNext = aParticleInstance.mNext;
				PIEmitter anEmitter = aParticleInstance.mEmitterSrc;
				PIParticleDef aParticleDef = aParticleInstance.mParticleDef;
				float anEmitterLifePct = 0f;
				if (aParticleInstance.mParentFreeEmitter != null)
				{
					anEmitterLifePct = aParticleInstance.mParentFreeEmitter.mLifePct;
				}
				bool isNew = aParticleInstance.mTicks == 0f;
				aParticleInstance.mTicks += 1f / anUpdateRate;
				float aLifePct;
				if (aParticleDef != null && aParticleDef.mSingleParticle)
				{
					float aNextToggleTime = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ACTIVE].GetNextKeyframeTime(mFrameNum);
					int aNextKeyIdx = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ACTIVE].GetNextKeyframeIdx(mFrameNum);
					aLifePct = ((!(aNextToggleTime >= mFrameNum) || aNextKeyIdx != 1) ? 0.02f : Math.Min(1f, (mFrameNum + (float)anEmitterInstanceDef.mFramesToPreload) / Math.Max(1f, aNextToggleTime)));
				}
				else
				{
					aLifePct = aParticleInstance.mTicks / aParticleInstance.mLife;
				}
				aParticleInstance.mLifePct = aLifePct;
				if (!(aParticleInstance.mLifePct < 0.9999999f && aParticleInstance.mLife > 1E-08f && (theEmitterInstance.mWasActive || anEmitterInstanceDef.mIsSuperEmitter)))
				{
					if (theParticleGroup.mIsSuperEmitter && ((PIFreeEmitterInstance)aParticleInstance).mEmitter.mParticleGroup.mHead != null)
					{
						aParticleInstance = aNext;
						continue;
					}
					if (!(!theParticleGroup.mIsSuperEmitter && aParticleDef != null && aParticleDef.mSingleParticle && theEmitterInstance.mWasActive))
					{
						FreeParticle(aParticleInstance, theParticleGroup);
						aParticleInstance = aNext;
						continue;
					}
				}
				if (aParticleDef != null)
				{
					PITexture aTexture = mDef.mTextureVector[aParticleDef.mTextureIdx];
					if (aParticleDef.mAnimSpeed == -1)
					{
						aParticleInstance.mImgIdx = aParticleInstance.mAnimFrameRand % aTexture.mNumCels;
					}
					else
					{
						aParticleInstance.mImgIdx = ((int)(aParticleInstance.mTicks * (float)mFramerate / (float)(aParticleDef.mAnimSpeed + 1)) + aParticleInstance.mAnimFrameRand) % aTexture.mNumCels;
					}
				}
				if (theParticleGroup.mIsSuperEmitter || !aParticleDef.mSingleParticle)
				{
					if (mIsNewFrame)
					{
						float aRand1 = GetRandFloat() * GetRandFloat();
						float aRand2 = GetRandFloat() * GetRandFloat();
						float aMotionRand;
						if (theParticleGroup.mIsSuperEmitter)
						{
							aMotionRand = Math.Max(0f,
								(theParticleGroup.mWasEmitted
									? anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_MOTION_RAND].GetValueAt(mFrameNum)
									: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_MOTION_RAND].GetValueAt(mFrameNum))
								* anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_MOTION_RAND_OVER_LIFE].GetValueAt(aLifePct, 1f)
								* (anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_MOTION_RAND].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_MOTION_RAND])
								* 30f);
						}
						else
						{
							aMotionRand = Math.Max(0f,
								(theParticleGroup.mWasEmitted
									? anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_MOTION_RAND].GetValueAt(mFrameNum)
									: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_MOTION_RAND].GetValueAt(mFrameNum))
								* aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_MOTION_RAND_OVER_LIFE].GetValueAt(aLifePct)
								* (aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_MOTION_RAND].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_MOTION_RAND]));
						}
						aParticleInstance.mVel.X += aRand1 * aMotionRand;
						aParticleInstance.mVel.Y += aRand2 * aMotionRand;
					}
					float aWeight;
					if (theParticleGroup.mIsSuperEmitter)
					{
						aWeight = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_WEIGHT].GetValueAt(mFrameNum)
							* (anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_WEIGHT_OVER_LIFE].GetValueAt(aLifePct, 1f) - 1f)
							* (anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_WEIGHT].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_WEIGHT])
							/ 2f * 100f;
					}
					else
					{
						aWeight = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_WEIGHT].GetValueAt(mFrameNum)
							* (aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_WEIGHT_OVER_LIFE].GetValueAt(aLifePct) - 1f)
							* (aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_WEIGHT].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_WEIGHT])
							* 100f;
					}
					aWeight *= 1f + ((float)mFramerate - 100f) * 0.0005f;
					aParticleInstance.mVel.Y += aWeight / anUpdateRate;
					Vector2 aCurVel = aParticleInstance.mVel / anUpdateRate;
					if (theParticleGroup.mIsSuperEmitter)
					{
						aCurVel *= anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_VELOCITY_OVER_LIFE].GetValueAt(aLifePct, 1f);
					}
					else
					{
						aCurVel *= aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_VELOCITY_OVER_LIFE].GetValueAt(aLifePct);
					}
					Vector2 aCurPhysPoint = default(Vector2);
					if (!isNew && aLayerDef.mDeflectorVector.Count > 0)
					{
						Vector2 aPrevPhysPoint = GlobalPIEffect.TransformFPoint(aParticleInstance.mTransform, new Vector2(0f, 0f));
						Vector2 aPrevPos = aParticleInstance.mPos;
						aParticleInstance.mPos += aCurVel;
						CalcParticleTransform(theLayer, theEmitterInstance, anEmitter, aParticleDef, theParticleGroup, aParticleInstance);
						aCurPhysPoint = GlobalPIEffect.TransformFPoint(aParticleInstance.mTransform, new Vector2(0f, 0f));
						for (int aDeflectorIdx = 0; aDeflectorIdx < aLayerDef.mDeflectorVector.Count; aDeflectorIdx++)
						{
							PIDeflector aDeflector = aLayerDef.mDeflectorVector[aDeflectorIdx];
							if (aDeflector.mActive.GetLastKeyframe(mFrameNum) < 0.99f)
							{
								continue;
							}
							for (int aPtIdx = 1; aPtIdx < aDeflector.mCurPoints.Count; aPtIdx++)
							{
								Vector2 aPt1 = aDeflector.mCurPoints[aPtIdx - 1] - new Vector2(mDrawTransform.m02, mDrawTransform.m12);
								Vector2 aPt2 = aDeflector.mCurPoints[aPtIdx] - new Vector2(mDrawTransform.m02, mDrawTransform.m12);
								SexyVector2 aLineNormal = new SexyVector2(aPt2.X - aPt1.X, aPt2.Y - aPt1.Y).Normalize().Perp();
								Vector2 aLineTranslate = new Vector2(aLineNormal.x, aLineNormal.y);
								aLineTranslate = aLineTranslate * aDeflector.mThickness * aParticleInstance.mThicknessHitVariation;
								Vector2 aCollPoint = default(Vector2);
								float aPos = 0f;
								if (GlobalPIEffect.LineSegmentIntersects(aPrevPhysPoint, aCurPhysPoint, aPt1 + aLineTranslate, aPt2 + aLineTranslate, ref aPos, aCollPoint) && !(GetRandFloatU() > aDeflector.mHits))
								{
									float aBounce = aDeflector.mBounce;
									if (theParticleGroup.mIsSuperEmitter)
									{
										aBounce *= (theParticleGroup.mWasEmitted
											? anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_BOUNCE].GetValueAt(mFrameNum)
											: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_BOUNCE].GetValueAt(mFrameNum))
											* anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_BOUNCE_OVER_LIFE].GetValueAt(aLifePct, 1f)
											* (anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_BOUNCE].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_BOUNCE]);
									}
									else
									{
										aBounce *= 0.5f;
									}
									SexyVector2 aCurVelVec = new SexyVector2(aCurVel.X, aCurVel.Y);
									float aDot = aCurVelVec.Dot(aLineNormal);
									SexyVector2 aNewVel = aCurVelVec - aLineNormal * 2f * aDot;
									float aPctBounce = Math.Min(1f, Math.Abs(aNewVel.y / aNewVel.x));
									aNewVel.y *= 1f - aPctBounce + aPctBounce * (float)Math.Pow(aBounce, 0.5);
									aParticleInstance.mVel = new Vector2(aNewVel.x, aNewVel.y) * 100f;
									if (aBounce > 0.001f)
									{
										aParticleInstance.mPos = aPrevPos;
									}
									CalcParticleTransform(theLayer, theEmitterInstance, anEmitter, aParticleDef, theParticleGroup, aParticleInstance);
									aCurPhysPoint = GlobalPIEffect.TransformFPoint(aParticleInstance.mTransform, new Vector2(0f, 0f));
								}
							}
						}
					}
					else
					{
						aParticleInstance.mPos += aCurVel;
						if (aLayerDef.mForceVector.Count > 0)
						{
							CalcParticleTransform(theLayer, theEmitterInstance, anEmitter, aParticleDef, theParticleGroup, aParticleInstance);
							aCurPhysPoint = GlobalPIEffect.TransformFPoint(aParticleInstance.mTransform, new Vector2(0f, 0f));
						}
					}
					for (int aForceIdx = 0; aForceIdx < aLayerDef.mForceVector.Count; aForceIdx++)
					{
						PIForce aForce = aLayerDef.mForceVector[aForceIdx];
						if (aForce.mActive.GetLastKeyframe(mFrameNum) < 0.99f)
						{
							continue;
						}
						bool inside = false;
						int i = 0;
						int j = 3;
						while (i < 4)
						{
							if (((aForce.mCurPoints[i].Y <= aCurPhysPoint.Y && aCurPhysPoint.Y < aForce.mCurPoints[j].Y)
								|| (aForce.mCurPoints[j].Y <= aCurPhysPoint.Y && aCurPhysPoint.Y < aForce.mCurPoints[i].Y))
								&& aCurPhysPoint.X < (aForce.mCurPoints[j].X - aForce.mCurPoints[i].X) * (aCurPhysPoint.Y - aForce.mCurPoints[i].Y) / (aForce.mCurPoints[j].Y - aForce.mCurPoints[i].Y) + aForce.mCurPoints[i].X)
							{
								inside = !inside;
							}
							j = i++;
						}
						if (inside)
						{
							float aDir = MathHelper.ToRadians(0f - aForce.mDirection.GetValueAt(mFrameNum)) + MathHelper.ToRadians(0f - aForce.mAngle.GetValueAt(mFrameNum));
							float aFrameStrength = 0.085f * (float)mFramerate / 100f;
							aFrameStrength *= 1f + ((float)mFramerate - 100f) * 0.004f;
							float aStrength = aForce.mStrength.GetValueAt(mFrameNum) * aFrameStrength;
							aParticleInstance.mVel.X += (float)Math.Cos(aDir) * aStrength * 100f;
							aParticleInstance.mVel.Y += (float)Math.Sin(aDir) * aStrength * 100f;
						}
					}
					if (!theParticleGroup.mIsSuperEmitter && aParticleDef.mAngleAlignToMotion && aParticleDef.mAngleKeepAlignedToMotion)
					{
						aParticleInstance.mImgAngle = (float)(0.0 - Math.Atan2(aCurVel.Y, aCurVel.X)) + MathHelper.ToRadians(-aParticleDef.mAngleAlignOffset);
					}
				}
				else if (aParticleDef.mSingleParticle)
				{
					bool needsRefresh = false;
					if (anEmitterInstanceDef.mEmitterGeom == 1 || anEmitterInstanceDef.mEmitterGeom == 4)
					{
						needsRefresh = anEmitterInstanceDef.mEmitAtPointsNum != 0;
					}
					else if (anEmitterInstanceDef.mEmitterGeom == 3)
					{
						needsRefresh = anEmitterInstanceDef.mEmitAtPointsNum * anEmitterInstanceDef.mEmitAtPointsNum2 != 0;
					}
					if (needsRefresh)
					{
						Vector2 aGeomPos = GetGeomPos(theEmitterInstance, aParticleInstance);
						aParticleInstance.mEmittedPos = GetEmitterPos(theEmitterInstance, true);
						aParticleInstance.mLastEmitterPos = aParticleInstance.mEmittedPos;
						aParticleInstance.mOrigPos = aGeomPos - aParticleInstance.mEmittedPos;
						SexyTransform2D aTransform = new SexyTransform2D(false);
						aTransform.RotateDeg(theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ANGLE].GetValueAt(mFrameNum));
						aParticleInstance.mEmittedPos += GlobalPIEffect.TransformFPoint(aTransform, aGeomPos);
					}
					if (aParticleDef.mAngleKeepAlignedToMotion && !aParticleDef.mAttachToEmitter)
					{
						Vector2 aVelocity = anEmitterInstanceDef.mPosition.GetVelocityAt(mFrameNum);
						if (aVelocity.Length() != 0f)
						{
							aParticleInstance.mImgAngle = (float)(0.0 - Math.Atan2(aVelocity.Y, aVelocity.X));
						}
						else
						{
							aParticleInstance.mImgAngle = 0f;
						}
						aParticleInstance.mImgAngle += MathHelper.ToRadians(-aParticleDef.mAngleAlignOffset);
					}
				}
				if (aParticleDef != null)
				{
					bool wantColor = (!aParticleInstance.mHasDrawn && aParticleDef.mGetColorFromLayer) || aParticleDef.mUpdateColorFromLayer;
					bool wantTransparency = (!aParticleInstance.mHasDrawn && aParticleDef.mGetTransparencyFromLayer) || aParticleDef.mUpdateTransparencyFromLayer;
					if (wantColor || wantTransparency)
					{
						Vector2 aDrawPoint = GlobalPIEffect.TransformFPoint(aParticleInstance.mTransform, new Vector2(0f, 0f));
						int aCheckX = (int)aDrawPoint.X + (int)theLayer.mBkgImgDrawOfs.X;
						int aCheckY = (int)aDrawPoint.Y + (int)theLayer.mBkgImgDrawOfs.Y;
						uint aColor;
						if (theLayer.mBkgImage != null && aCheckX >= 0 && aCheckY >= 0 && aCheckX < theLayer.mBkgImage.mWidth && aCheckY < theLayer.mBkgImage.mHeight)
						{
							uint[] bits = theLayer.mBkgImage.GetBits();
							aColor = bits[aCheckX + aCheckY * theLayer.mBkgImage.mWidth];
						}
						else
						{
							aColor = 0u;
						}
						if (wantColor)
						{
							aParticleInstance.mBkgColor = (aParticleInstance.mBkgColor & 0xFF000000u) | (aColor & 0xFFFFFF);
						}
						if (wantTransparency)
						{
							aParticleInstance.mBkgColor = (aParticleInstance.mBkgColor & 0xFFFFFF) | (aColor & 0xFF000000u);
						}
					}
				}
				if (theParticleGroup.mIsSuperEmitter)
				{
					aParticleInstance.mImgAngle += MathHelper.ToRadians(0f
						- (theParticleGroup.mWasEmitted
							? anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SPIN].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_SPIN].GetValueAt(mFrameNum))
						* (anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SPIN_OVER_LIFE].GetValueAt(aLifePct, 1f) - 1f)
						* (anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_F_SPIN].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SPIN]))
						/ anUpdateRate * 160f;
				}
				else if (!aParticleDef.mAngleKeepAlignedToMotion)
				{
					aParticleInstance.mImgAngle += MathHelper.ToRadians(0f
						- (theParticleGroup.mWasEmitted
							? anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_SPIN].GetValueAt(mFrameNum)
							: theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_SPIN].GetValueAt(mFrameNum))
						* (aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SPIN_OVER_LIFE].GetValueAt(aLifePct) - 1f)
						* (aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_SPIN].GetValueAt(mFrameNum) + aParticleInstance.mVariationValues[(int)PIParticleInstance.PIParticleVariation.VARIATION_SPIN]))
						/ anUpdateRate;
				}
				aParticleInstance = aNext;
			}
		}

		public void DrawParticleGroup(Graphics g, PILayer theLayer, PIEmitterInstance theEmitterInstance, PIParticleGroup theParticleGroup, bool isDarkeningPass)
		{
			if (!theEmitterInstance.mWasActive)
			{
				return;
			}
			Color aColorMult = new Color(theLayer.mColor.mRed * mColor.mRed / 255, theLayer.mColor.mGreen * mColor.mGreen / 255, theLayer.mColor.mBlue * mColor.mBlue / 255, theLayer.mColor.mAlpha * mColor.mAlpha / 255);
			bool hasColor = aColorMult != Color.White;

			{
				PIParticleInstance aParticleInstance = theParticleGroup.mHead;
				while (aParticleInstance != null)
				{
					PIParticleInstance aNext = aParticleInstance.mNext;
					float aLifePct = aParticleInstance.mLifePct;
					PIParticleDef aParticleDef = aParticleInstance.mParticleDef;
					bool isIntense = aParticleDef.mIntense;
					if ((isIntense && aParticleDef.mPreserveColor) || !isDarkeningPass)
					{
						PIEmitter anEmitter = aParticleInstance.mEmitterSrc;
						float anEmitterLifePct = 0f;
						if (aParticleInstance.mParentFreeEmitter != null)
						{
							anEmitterLifePct = aParticleInstance.mParentFreeEmitter.mLifePct;
						}
						if (!isIntense || isDarkeningPass)
						{
							g.SetDrawMode(0);
						}
						else
						{
							g.SetDrawMode(1);
						}
						PITexture aTexture = mDef.mTextureVector[aParticleDef.mTextureIdx];
						DeviceImage anImage;
						Rect aSrcRect;
						if (aTexture.mImageVector.Count > 0)
						{
							anImage = aTexture.mImageVector[aParticleInstance.mImgIdx].GetDeviceImage();
							aSrcRect = new Rect(0, 0, anImage.mWidth, anImage.mHeight);
						}
						else
						{
							anImage = aTexture.mImageStrip.GetDeviceImage();
							aSrcRect = anImage.GetCelRect(aParticleInstance.mImgIdx);
						}
						int aColorI;
						if (aParticleDef.mRandomGradientColor)
						{
							if (aParticleDef.mUseKeyColorsOnly)
							{
								int aKeyframe = (int)Math.Min(aParticleDef.mColor.mInterpolatorPointVector.Count * aParticleInstance.mGradientRand, aParticleDef.mColor.mInterpolatorPointVector.Count - 1);
								aColorI = aParticleDef.mColor.GetKeyframeNum(aKeyframe);
							}
							else
							{
								aColorI = aParticleDef.mColor.GetValueAt(aParticleInstance.mGradientRand);
							}
						}
						else if (aParticleDef.mUseNextColorKey)
						{
							int aKeyframe = aParticleInstance.mNum / aParticleDef.mNumberOfEachColor % aParticleDef.mColor.mInterpolatorPointVector.Count;
							aColorI = aParticleDef.mColor.GetKeyframeNum(aKeyframe);
						}
						else
						{
							float aColorPosUsed = GlobalPIEffect.WrapFloat(aLifePct, aParticleDef.mRepeatColor + 1);
							aColorI = aParticleDef.mColor.GetValueAt(aColorPosUsed);
						}
						if (aParticleDef.mGetColorFromLayer)
						{
							aColorI = (int)((uint)aColorI & 0xFF000000u) | (int)(aParticleInstance.mBkgColor & 0xFFFFFF);
						}
						if (aParticleDef.mGetTransparencyFromLayer)
						{
							aColorI = (aColorI & 0xFFFFFF) | (int)(aParticleInstance.mBkgColor & 0xFF000000u);
						}
						float aTintPct = theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_TINT_STRENGTH].GetValueAt(mFrameNum) * anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_TINT_STRENGTH].GetValueAt(mFrameNum, 1f);
						aColorI = (int)GlobalPIEffect.InterpColor(aColorI, theEmitterInstance.mTintColor.ToInt(), aTintPct);
						int anAlpha = aParticleDef.mAlpha.GetValueAt(GlobalPIEffect.WrapFloat(aLifePct, aParticleDef.mRepeatAlpha + 1));
						anAlpha = (int)((float)anAlpha *
							(theEmitterInstance.mEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_VISIBILITY].GetValueAt(mFrameNum)
							* aParticleDef.mValues[(int)PIParticleDef.PIParticleDefValue.VALUE_VISIBILITY].GetValueAt(mFrameNum)
							* anEmitter.mValues[(int)PIEmitter.PIEmitterValue.VALUE_VISIBILITY].GetValueAt(mFrameNum, 1f)));
						if (isDarkeningPass)
						{
							aColorI = (int)((uint)aColorI & 0xFF000000u);
						}
						Color aColor = new Color((anAlpha << 24) | (aColorI & 0xFFFFFF));
						if (hasColor)
						{
							aColor = new Color(aColorMult.mRed * aColor.mRed / 255, aColorMult.mGreen * aColor.mGreen / 255, aColorMult.mBlue * aColor.mBlue / 255, aColorMult.mAlpha * aColor.mAlpha / 255);
						}
						if (aColor.mAlpha != 0)
						{
							g.SetColor(aColor);
							CalcParticleTransform(theLayer, theEmitterInstance, anEmitter, aParticleDef, theParticleGroup, aParticleInstance);
							SexyTransform2D aMatrix = mDrawTransform * aParticleInstance.mTransform;
							g.DrawImageMatrix(anImage, aMatrix, aSrcRect);
							mLastDrawnPixelCount = (int)Math.Abs(aParticleInstance.mTransformScaleFactor * anImage.mHeight * anImage.mWidth + mLastDrawnPixelCount);
							aParticleInstance.mHasDrawn = true;
						}
					}
					aParticleInstance = aNext;
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
			Common.Resize(mLayerVector, rhs.mDef.mLayerDefVector.Count);
			Common.Resize(mDef.mLayerDefVector, rhs.mDef.mLayerDefVector.Count);
			for (int i = 0; i < mLayerVector.Count; i++)
			{
				PILayerDef aLayerDef = mDef.mLayerDefVector[i];
				PILayer aLayer = mLayerVector[i];
				aLayer.mLayerDef = aLayerDef;
				Common.Resize(aLayer.mEmitterInstanceVector, aLayerDef.mEmitterInstanceDefVector.Count);
				for (int j = 0; j < aLayerDef.mEmitterInstanceDefVector.Count; j++)
				{
					PIEmitterInstance anEmitterInstance = rhs.mLayerVector[i].mEmitterInstanceVector[j];
					PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[j];
					PIEmitterInstance aLocalEmitterInstance = aLayer.mEmitterInstanceVector[j];
					PIEmitter anEmitter = mDef.mEmitterVector[anEmitterInstanceDef.mEmitterDefIdx];
					aLocalEmitterInstance.mEmitterInstanceDef = anEmitterInstanceDef;
					aLocalEmitterInstance.mTintColor = new Color(anEmitterInstance.mTintColor);
					Common.Resize(aLocalEmitterInstance.mParticleDefInstanceVector, anEmitter.mParticleDefVector.Count);
					Common.Resize(aLocalEmitterInstance.mSuperEmitterParticleDefInstanceVector, anEmitterInstance.mSuperEmitterParticleDefInstanceVector.Count);
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
				PITexture aTexture = new PITexture();
				aTexture.mName = ReadString();
				short num2 = (short)(aTexture.mNumCels = mReadBuffer.ReadShort());
				if (mIsPPF)
				{
					short num3 = mReadBuffer.ReadShort();
					aTexture.mPadded = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					string text = (aTexture.mFileName = ReadString());
					aTexture.mImageStrip = GetImage(aTexture.mName, text);
					if (aTexture.mImageStrip.GetDeviceImage() == null)
					{
						Fail("Unable to load image: " + text);
					}
					else if (aTexture.mImageStrip.GetDeviceImage().mNumCols == 1 && aTexture.mImageStrip.GetDeviceImage().mNumRows == 1)
					{
						aTexture.mImageStrip.GetDeviceImage().mNumCols = num2 / num3;
						aTexture.mImageStrip.GetDeviceImage().mNumRows = num3;
					}
					mDef.mTextureVector.Add(aTexture);
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
				PILayerDef aLayerDef = mDef.mLayerDefVector[k];
				PILayer aLayer = mLayerVector[k];
				aLayer.mLayerDef = aLayerDef;
				ExpectCmd("CLayer");
				aLayerDef.mName = ReadString();
				num4 = mReadBuffer.ReadShort();
				aLayer.mEmitterInstanceVector.Capacity = num4;
				Common.Resize(aLayer.mEmitterInstanceVector, num4);
				aLayerDef.mEmitterInstanceDefVector.Capacity = num4;
				Common.Resize(aLayerDef.mEmitterInstanceDefVector, num4);
				for (int l = 0; l < num4; l++)
				{
					PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[l];
					PIEmitterInstance anEmitterInstance = aLayer.mEmitterInstanceVector[l];
					anEmitterInstance.mEmitterInstanceDef = anEmitterInstanceDef;
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
					anEmitterInstanceDef.mFramesToPreload = mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					anEmitterInstanceDef.mName = ReadString();
					anEmitterInstanceDef.mEmitterGeom = mReadBuffer.ReadInt32();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
					if ((mIsPPF ? mReadBuffer.ReadByte() : mReadBuffer.ReadInt32()) != 0 && anEmitterInstanceDef.mEmitterGeom == 2)
					{
						anEmitterInstanceDef.mEmitterGeom = 4;
					}
					anEmitterInstanceDef.mEmitIn = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					anEmitterInstanceDef.mEmitOut = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
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
					anEmitterInstance.mTintColor = new Color((int)num6);
					mReadBuffer.ReadInt32();
					anEmitterInstanceDef.mEmitAtPointsNum = mReadBuffer.ReadInt32();
					anEmitterInstanceDef.mEmitterDefIdx = mReadBuffer.ReadInt32();
					list[anEmitterInstanceDef.mEmitterDefIdx] = true;
					PIEmitter anEmitter = mDef.mEmitterVector[anEmitterInstanceDef.mEmitterDefIdx];
					Common.Resize(anEmitterInstance.mParticleDefInstanceVector, anEmitter.mParticleDefVector.Count);
					for (int m = 0; m < anEmitter.mParticleDefVector.Count; m++)
					{
						list2[anEmitter.mParticleDefVector[m].mTextureIdx] = true;
					}
					ReadValue2D(anEmitterInstanceDef.mPosition);
					int num7 = mReadBuffer.ReadShort();
					for (int n = 0; n < num7; n++)
					{
						ExpectCmd("CEPoint");
						mReadBuffer.ReadFloat();
						mReadBuffer.ReadFloat();
						PIValue2D pIValue2D = new PIValue2D();
						ReadEPoint(pIValue2D);
						anEmitterInstanceDef.mPoints.Add(pIValue2D);
					}
					for (int num8 = 0; num8 < 17; num8++)
					{
						ReadValue(ref anEmitterInstanceDef.mValues[num8]);
					}
					anEmitterInstanceDef.mEmitAtPointsNum2 = mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					ReadValue(ref anEmitterInstanceDef.mValues[17]);
					mReadBuffer.ReadInt32();
					ReadValue(ref anEmitterInstanceDef.mValues[18]);
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
						anEmitterInstance.mMaskImage = GetImage(theName, theFilename);
					}
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					anEmitterInstanceDef.mInvertMask = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					anEmitterInstanceDef.mIsSuperEmitter = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					int num11 = mReadBuffer.ReadShort();
					for (int num12 = 0; num12 < num11; num12++)
					{
						if (mIsPPF)
						{
							int item = mReadBuffer.ReadShort();
							anEmitterInstanceDef.mFreeEmitterIndices.Add(item);
							list[l] = true;
							continue;
						}
						throw new NotImplementedException();
					}
					Common.Resize(anEmitterInstance.mSuperEmitterParticleDefInstanceVector, num11);
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadFloat();
					mReadBuffer.ReadFloat();
				}
				short num13 = mReadBuffer.ReadShort();
				for (int num14 = 0; num14 < num13; num14++)
				{
					PIDeflector aDeflector = new PIDeflector();
					ExpectCmd("CDeflector");
					aDeflector.mName = ReadString();
					aDeflector.mBounce = mReadBuffer.ReadInt32();
					aDeflector.mHits = mReadBuffer.ReadInt32();
					aDeflector.mThickness = mReadBuffer.ReadInt32();
					aDeflector.mVisible = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					ReadValue2D(aDeflector.mPos);
					int num15 = mReadBuffer.ReadShort();
					for (int num16 = 0; num16 < num15; num16++)
					{
						ExpectCmd("CEPoint");
						mReadBuffer.ReadFloat();
						mReadBuffer.ReadFloat();
						PIValue2D pIValue2D2 = new PIValue2D();
						ReadEPoint(pIValue2D2);
						aDeflector.mPoints.Add(pIValue2D2);
					}
					Common.Resize(aDeflector.mCurPoints, aDeflector.mPoints.Count);
					ReadValue(ref aDeflector.mActive);
					ReadValue(ref aDeflector.mAngle);
					aLayerDef.mDeflectorVector.Add(aDeflector);
				}
				short num17 = mReadBuffer.ReadShort();
				for (int num18 = 0; num18 < num17; num18++)
				{
					PIBlocker aBlocker = new PIBlocker();
					ExpectCmd("CBlocker");
					aBlocker.mName = ReadString();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					mReadBuffer.ReadInt32();
					ReadValue2D(aBlocker.mPos);
					int num19 = mReadBuffer.ReadShort();
					for (int num20 = 0; num20 < num19; num20++)
					{
						ExpectCmd("CEPoint");
						mReadBuffer.ReadFloat();
						mReadBuffer.ReadFloat();
						PIValue2D pIValue2D3 = new PIValue2D();
						ReadEPoint(pIValue2D3);
						aBlocker.mPoints.Add(pIValue2D3);
					}
					ReadValue(ref aBlocker.mActive);
					ReadValue(ref aBlocker.mAngle);
					aLayerDef.mBlockerVector.Add(aBlocker);
				}
				ReadValue2D(aLayerDef.mOffset);
				aLayerDef.mOrigOffset = aLayerDef.mOffset.GetValueAt(0f);
				ReadValue(ref aLayerDef.mAngle);
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
					PIForce aForce = new PIForce();
					aForce.mName = ReadString();
					aForce.mVisible = (mIsPPF ? (mReadBuffer.ReadByte() != 0) : (mReadBuffer.ReadInt32() != 0));
					ReadValue2D(aForce.mPos);
					ReadValue(ref aForce.mActive);
					PIValue theValue = new PIValue();
					ReadValue(ref theValue);
					ReadValue(ref aForce.mStrength);
					ReadValue(ref aForce.mWidth);
					ReadValue(ref aForce.mHeight);
					ReadValue(ref aForce.mAngle);
					ReadValue(ref aForce.mDirection);
					aLayerDef.mForceVector.Add(aForce);
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
			for (int aLayerIdx = 0; aLayerIdx < mDef.mLayerDefVector.Count; aLayerIdx++)
			{
				PILayerDef aLayerDef = mDef.mLayerDefVector[aLayerIdx];
				for (int anEmitterInstanceIdx = 0; anEmitterInstanceIdx < aLayerDef.mEmitterInstanceDefVector.Count; anEmitterInstanceIdx++)
				{
					PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[anEmitterInstanceIdx];
					anEmitterInstanceDef.mEmitterDefIdx = list3[anEmitterInstanceDef.mEmitterDefIdx];
					for (int aFreeEmitterIdx = 0; aFreeEmitterIdx < anEmitterInstanceDef.mFreeEmitterIndices.Count; aFreeEmitterIdx++)
					{
						anEmitterInstanceDef.mFreeEmitterIndices[aFreeEmitterIdx] = list3[anEmitterInstanceDef.mFreeEmitterIndices[aFreeEmitterIdx]];
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
					PIParticleDef aParticleDef = pIEmitter2.mParticleDefVector[num40];
					aParticleDef.mTextureIdx = list4[aParticleDef.mTextureIdx];
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
				PITexture aTexture = mDef.mTextureVector[i];
				aTexture.mImageStrip = (aTexture.mImageStrip = GetImage(aTexture.mName, aTexture.mFileName));
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
					PILayer aLayer = mLayerVector[i];
					PILayerDef aLayerDef = mDef.mLayerDefVector[i];
					for (int j = 0; j < aLayerDef.mEmitterInstanceDefVector.Count; j++)
					{
						PIEmitterInstance anEmitterInstance = aLayer.mEmitterInstanceVector[j];
						PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[j];
						if (theBuffer.ReadBoolean())
						{
							anEmitterInstance.mTransform = theBuffer.ReadTransform2D();
						}
						anEmitterInstance.mWasActive = theBuffer.ReadBoolean();
						anEmitterInstance.mWithinLifeFrame = theBuffer.ReadBoolean();
						PIEmitter anEmitter = mDef.mEmitterVector[anEmitterInstanceDef.mEmitterDefIdx];
						for (int k = 0; k < anEmitter.mParticleDefVector.Count; k++)
						{
							PIParticleDefInstance theParticleDefInstance = anEmitterInstance.mParticleDefInstanceVector[k];
							LoadParticleDefInstance(theBuffer, theParticleDefInstance);
						}
						for (int l = 0; l < anEmitterInstanceDef.mFreeEmitterIndices.Count; l++)
						{
							PIParticleDefInstance theParticleDefInstance2 = anEmitterInstance.mSuperEmitterParticleDefInstanceVector[l];
							LoadParticleDefInstance(theBuffer, theParticleDefInstance2);
						}
						int num5 = (int)theBuffer.ReadLong();
						for (int m = 0; m < num5; m++)
						{
							PIFreeEmitterInstance aChildEmitterInstance = mFreeEmitterPool.Alloc();
							aChildEmitterInstance.Reset();
							int index = theBuffer.ReadShort();
							aChildEmitterInstance.mEmitterSrc = mDef.mEmitterVector[anEmitterInstanceDef.mFreeEmitterIndices[index]];
							aChildEmitterInstance.mParentFreeEmitter = null;
							aChildEmitterInstance.mParticleDef = null;
							aChildEmitterInstance.mNum = m;
							LoadParticle(theBuffer, aLayer, aChildEmitterInstance);
							PIEmitter mEmitterSrc = aChildEmitterInstance.mEmitterSrc;
							Common.Resize(aChildEmitterInstance.mEmitter.mParticleDefInstanceVector, mEmitterSrc.mParticleDefVector.Count);
							for (int n = 0; n < mEmitterSrc.mParticleDefVector.Count; n++)
							{
								PIParticleDefInstance theParticleDefInstance3 = aChildEmitterInstance.mEmitter.mParticleDefInstanceVector[n];
								LoadParticleDefInstance(theBuffer, theParticleDefInstance3);
							}
							if (m > 0)
							{
								anEmitterInstance.mSuperEmitterGroup.mTail.mNext = aChildEmitterInstance;
								aChildEmitterInstance.mPrev = anEmitterInstance.mSuperEmitterGroup.mTail;
							}
							else
							{
								anEmitterInstance.mSuperEmitterGroup.mHead = aChildEmitterInstance;
							}
							anEmitterInstance.mSuperEmitterGroup.mTail = aChildEmitterInstance;
							anEmitterInstance.mSuperEmitterGroup.mCount++;
							int num6 = (int)theBuffer.ReadLong();
							for (int num7 = 0; num7 < num6; num7++)
							{
								PIParticleInstance aParticleInstance = mParticlePool.Alloc();
								aParticleInstance.Reset();
								aParticleInstance.mEmitterSrc = aChildEmitterInstance.mEmitterSrc;
								aParticleInstance.mParentFreeEmitter = aChildEmitterInstance;
								int index2 = theBuffer.ReadShort();
								aParticleInstance.mParticleDef = aParticleInstance.mEmitterSrc.mParticleDefVector[index2];
								aParticleInstance.mNum = num7;
								LoadParticle(theBuffer, aLayer, aParticleInstance);
								CalcParticleTransform(aLayer, anEmitterInstance, aParticleInstance.mEmitterSrc, aParticleInstance.mParticleDef, aChildEmitterInstance.mEmitter.mParticleGroup, aParticleInstance);
								if (num7 > 0)
								{
									aChildEmitterInstance.mEmitter.mParticleGroup.mTail.mNext = aParticleInstance;
									aParticleInstance.mPrev = aChildEmitterInstance.mEmitter.mParticleGroup.mTail;
								}
								else
								{
									aChildEmitterInstance.mEmitter.mParticleGroup.mHead = aParticleInstance;
								}
								aChildEmitterInstance.mEmitter.mParticleGroup.mTail = aParticleInstance;
								aChildEmitterInstance.mEmitter.mParticleGroup.mCount++;
							}
						}
						int num8 = (int)theBuffer.ReadLong();
						for (int num9 = 0; num9 < num8; num9++)
						{
							PIParticleInstance aParticleInstance2 = mParticlePool.Alloc();
							aParticleInstance2.Reset();
							aParticleInstance2.mEmitterSrc = anEmitter;
							aParticleInstance2.mParentFreeEmitter = null;
							int index3 = theBuffer.ReadShort();
							aParticleInstance2.mParticleDef = aParticleInstance2.mEmitterSrc.mParticleDefVector[index3];
							aParticleInstance2.mNum = num9;
							LoadParticle(theBuffer, aLayer, aParticleInstance2);
							CalcParticleTransform(aLayer, anEmitterInstance, aParticleInstance2.mEmitterSrc, aParticleInstance2.mParticleDef, anEmitterInstance.mParticleGroup, aParticleInstance2);
							if (num9 > 0)
							{
								anEmitterInstance.mParticleGroup.mTail.mNext = aParticleInstance2;
								aParticleInstance2.mPrev = anEmitterInstance.mParticleGroup.mTail;
							}
							else
							{
								anEmitterInstance.mParticleGroup.mHead = aParticleInstance2;
							}
							anEmitterInstance.mParticleGroup.mTail = aParticleInstance2;
							anEmitterInstance.mParticleGroup.mCount++;
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
					PILayer aLayer = mLayerVector[i];
					PILayerDef aLayerDef = mDef.mLayerDefVector[i];
					for (int j = 0; j < aLayer.mEmitterInstanceVector.Count; j++)
					{
						PIEmitterInstance anEmitterInstance = aLayer.mEmitterInstanceVector[j];
						PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[j];
						if (!GlobalPIEffect.IsIdentityMatrix(anEmitterInstance.mTransform))
						{
							theBuffer.WriteBoolean(true);
							theBuffer.WriteTransform2D(anEmitterInstance.mTransform);
						}
						else
						{
							theBuffer.WriteBoolean(false);
						}
						theBuffer.WriteBoolean(anEmitterInstance.mWasActive);
						theBuffer.WriteBoolean(anEmitterInstance.mWithinLifeFrame);
						Dictionary<PIEmitter, Dictionary<PIParticleDef, int>> dictionary = new Dictionary<PIEmitter, Dictionary<PIParticleDef, int>>();
						PIEmitter anEmitter = mDef.mEmitterVector[anEmitterInstanceDef.mEmitterDefIdx];
						for (int k = 0; k < anEmitter.mParticleDefVector.Count; k++)
						{
							PIParticleDef key = anEmitter.mParticleDefVector[k];
							PIParticleDefInstance theParticleDefInstance = anEmitterInstance.mParticleDefInstanceVector[k];
							if (!dictionary.ContainsKey(anEmitter))
							{
								dictionary.Add(anEmitter, new Dictionary<PIParticleDef, int>());
							}
							if (!dictionary[anEmitter].ContainsKey(key))
							{
								dictionary[anEmitter].Add(key, k);
							}
							else
							{
								dictionary[anEmitter][key] = k;
							}
							SaveParticleDefInstance(theBuffer, theParticleDefInstance);
						}
						Dictionary<PIEmitter, int> dictionary2 = new Dictionary<PIEmitter, int>();
						for (int l = 0; l < anEmitterInstanceDef.mFreeEmitterIndices.Count; l++)
						{
							PIEmitter pIEmitter2 = mDef.mEmitterVector[anEmitterInstanceDef.mFreeEmitterIndices[l]];
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
							PIParticleDefInstance theParticleDefInstance2 = anEmitterInstance.mSuperEmitterParticleDefInstanceVector[l];
							SaveParticleDefInstance(theBuffer, theParticleDefInstance2);
							dictionary2[pIEmitter2] = l;
						}
						PIFreeEmitterInstance aChildEmitterInstance = (PIFreeEmitterInstance)anEmitterInstance.mSuperEmitterGroup.mHead;
						theBuffer.WriteLong(CountParticles(aChildEmitterInstance));
						while (aChildEmitterInstance != null)
						{
							theBuffer.WriteShort((short)dictionary2[aChildEmitterInstance.mEmitterSrc]);
							SaveParticle(theBuffer, aLayer, aChildEmitterInstance);
							PIEmitter mEmitterSrc = aChildEmitterInstance.mEmitterSrc;
							for (int n = 0; n < mEmitterSrc.mParticleDefVector.Count; n++)
							{
								PIParticleDefInstance theParticleDefInstance3 = aChildEmitterInstance.mEmitter.mParticleDefInstanceVector[n];
								SaveParticleDefInstance(theBuffer, theParticleDefInstance3);
							}
							PIParticleInstance aParticleInstance = aChildEmitterInstance.mEmitter.mParticleGroup.mHead;
							theBuffer.WriteLong(CountParticles(aParticleInstance));
							while (aParticleInstance != null)
							{
								theBuffer.WriteShort((short)dictionary[aParticleInstance.mEmitterSrc][aParticleInstance.mParticleDef]);
								SaveParticle(theBuffer, aLayer, aParticleInstance);
								aParticleInstance = aParticleInstance.mNext;
							}
							aChildEmitterInstance = (PIFreeEmitterInstance)aChildEmitterInstance.mNext;
						}
						PIParticleInstance aParticleInstance2 = anEmitterInstance.mParticleGroup.mHead;
						int num2 = CountParticles(aParticleInstance2);
						theBuffer.WriteLong(num2);
						while (aParticleInstance2 != null)
						{
							short theShort = (short)dictionary[aParticleInstance2.mEmitterSrc][aParticleInstance2.mParticleDef];
							theBuffer.WriteShort(theShort);
							SaveParticle(theBuffer, aLayer, aParticleInstance2);
							aParticleInstance2 = aParticleInstance2.mNext;
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
			for (int aLayerIdx = 0; aLayerIdx < mDef.mLayerDefVector.Count; aLayerIdx++)
			{
				PILayerDef aLayerDef = mDef.mLayerDefVector[aLayerIdx];
				PILayer aLayer = mLayerVector[aLayerIdx];
				for (int anEmitterInstanceIdx = 0; anEmitterInstanceIdx < aLayer.mEmitterInstanceVector.Count; anEmitterInstanceIdx++)
				{
					PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[anEmitterInstanceIdx];
					PIEmitterInstance anEmitterInstance = aLayer.mEmitterInstanceVector[anEmitterInstanceIdx];
					PIFreeEmitterInstance aChildEmitterInstance = (PIFreeEmitterInstance)anEmitterInstance.mSuperEmitterGroup.mHead;
					while (aChildEmitterInstance != null)
					{
						PIFreeEmitterInstance aChildNext = (PIFreeEmitterInstance)aChildEmitterInstance.mNext;
						PIParticleInstance aParticleInstance = aChildEmitterInstance.mEmitter.mParticleGroup.mHead;
						while (aParticleInstance != null)
						{
							PIParticleInstance aNext = aParticleInstance.mNext;
							mParticlePool.Free(aParticleInstance);
							aParticleInstance = aNext;
						}
						mFreeEmitterPool.Free(aChildEmitterInstance);
						aChildEmitterInstance = aChildNext;
					}
					anEmitterInstance.mSuperEmitterGroup.mHead = null;
					anEmitterInstance.mSuperEmitterGroup.mTail = null;
					anEmitterInstance.mSuperEmitterGroup.mCount = 0;
					PIParticleInstance aLooseParticle = anEmitterInstance.mParticleGroup.mHead;
					while (aLooseParticle != null)
					{
						PIParticleInstance aNext = aLooseParticle.mNext;
						mParticlePool.Free(aLooseParticle);
						aLooseParticle = aNext;
					}
					anEmitterInstance.mParticleGroup.mHead = null;
					anEmitterInstance.mParticleGroup.mTail = null;
					anEmitterInstance.mParticleGroup.mCount = 0;
					for (int aFreeEmitterIdx = 0; aFreeEmitterIdx < anEmitterInstanceDef.mFreeEmitterIndices.Count; aFreeEmitterIdx++)
					{
						PIParticleDefInstance aParticleDefInstance = anEmitterInstance.mSuperEmitterParticleDefInstanceVector[aFreeEmitterIdx];
						aParticleDefInstance.Reset();
					}
					PIEmitter anEmitter = mDef.mEmitterVector[anEmitterInstanceDef.mEmitterDefIdx];
					for (int aParticleDefIdx = 0; aParticleDefIdx < anEmitter.mParticleDefVector.Count; aParticleDefIdx++)
					{
						PIParticleDefInstance aParticleDefInstance = anEmitterInstance.mParticleDefInstanceVector[aParticleDefIdx];
						aParticleDefInstance.Reset();
					}
					anEmitterInstance.mWithinLifeFrame = true;
					anEmitterInstance.mWasActive = false;
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
			for (int aLayerIdx = 0; aLayerIdx < mDef.mLayerDefVector.Count; aLayerIdx++)
			{
				PILayerDef aLayerDef = mDef.mLayerDefVector[aLayerIdx];
				PILayer aLayer = mLayerVector[aLayerIdx];
				if (!aLayer.mVisible)
				{
					continue;
				}
				for (int anEmitterInstanceIdx = 0; anEmitterInstanceIdx < aLayer.mEmitterInstanceVector.Count; anEmitterInstanceIdx++)
				{
					PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[anEmitterInstanceIdx];
					PIEmitterInstance anEmitterInstance = aLayer.mEmitterInstanceVector[anEmitterInstanceIdx];
					if (anEmitterInstance.mVisible)
					{
						if (anEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ACTIVE].GetNextKeyframeTime(mFrameNum) >= mFrameNum)
						{
							return true;
						}
						if (anEmitterInstance.mWithinLifeFrame)
						{
							return true;
						}
						if (anEmitterInstance.mSuperEmitterGroup.mHead != null)
						{
							return true;
						}
						if (anEmitterInstance.mParticleGroup.mHead != null)
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
			bool isFirstFrame = mFrameNum == 0f;
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
			if (isFirstFrame && mStartupState.GetDataLen() != 0)
			{
				mStartupState.SeekFront();
				LoadState(mStartupState, true);
				mWantsSRand = false;
				return;
			}
			bool firstIteration = true;
			while (mFrameNum < (float)mFirstFrameNum || firstIteration)
			{
				firstIteration = false;
				mCurNumEmitters = 0;
				mCurNumParticles = 0;
				float anUpdateRate = 100f / mAnimSpeed;
				int aPrevFrame = (int)mFrameNum;
				if (isFirstFrame)
				{
					mFrameNum += 0.0001f;
				}
				else
				{
					mFrameNum += (float)mFramerate / anUpdateRate;
				}
				mIsNewFrame = aPrevFrame != (int)mFrameNum;
				for (int aLayerIdx = 0; aLayerIdx < mDef.mLayerDefVector.Count; aLayerIdx++)
				{
					PILayerDef aLayerDef = mDef.mLayerDefVector[aLayerIdx];
					PILayer aLayer = mLayerVector[aLayerIdx];
					if (!aLayer.mVisible)
					{
						continue;
					}
					for (int aDeflectorIdx = 0; aDeflectorIdx < aLayerDef.mDeflectorVector.Count; aDeflectorIdx++)
					{
						PIDeflector aDeflector = aLayerDef.mDeflectorVector[aDeflectorIdx];
						SexyTransform2D aTransform = new SexyTransform2D(false);
						float aDeflectorAng = aDeflector.mAngle.GetValueAt(mFrameNum);
						if (aDeflectorAng != 0f)
						{
							aTransform.RotateDeg(aDeflectorAng);
						}
						Vector2 aDeflectorPos = aDeflector.mPos.GetValueAt(mFrameNum);
						aTransform.Translate(aDeflectorPos.X, aDeflectorPos.Y);
						Vector2 anOffset = aLayerDef.mOffset.GetValueAt(mFrameNum);
						aTransform.Translate(anOffset.X, anOffset.Y);
						float aLayerAngle = aLayerDef.mAngle.GetValueAt(mFrameNum);
						if (aLayerAngle != 0f)
						{
							aTransform.RotateDeg(aLayerAngle);
						}
						SexyTransform2D aFinalTrans = mDrawTransform * aTransform;
						for (int aPtIdx = 0; aPtIdx < aDeflector.mPoints.Count; aPtIdx++)
						{
							aDeflector.mCurPoints[aPtIdx] = GlobalPIEffect.TransformFPoint(aFinalTrans, aDeflector.mPoints[aPtIdx].GetValueAt(mFrameNum));
						}
					}
					for (int aForceIdx = 0; aForceIdx < aLayerDef.mForceVector.Count; aForceIdx++)
					{
						PIForce aForce = aLayerDef.mForceVector[aForceIdx];
						SexyTransform2D aTransform = new SexyTransform2D(false);
						aTransform.Scale(aForce.mWidth.GetValueAt(mFrameNum) / 2f, aForce.mHeight.GetValueAt(mFrameNum) / 2f);
						float aForceAngle = aForce.mAngle.GetValueAt(mFrameNum);
						if (aForceAngle != 0f)
						{
							aTransform.RotateDeg(aForceAngle);
						}
						Vector2 aForcePos = aForce.mPos.GetValueAt(mFrameNum);
						aTransform.Translate(aForcePos.X, aForcePos.Y);
						Vector2 anOffset = aLayerDef.mOffset.GetValueAt(mFrameNum);
						aTransform.Translate(anOffset.X, anOffset.Y);
						float aLayerAngle = aLayerDef.mAngle.GetValueAt(mFrameNum);
						if (aLayerAngle != 0f)
						{
							aTransform.RotateDeg(aLayerAngle);
						}
						SexyTransform2D aFinalTrans = mDrawTransform * aTransform;
						Vector2[] aBoxPoints = sForceBoxPoints;
						for (int aPtIdx = 0; aPtIdx < 5; aPtIdx++)
						{
							aForce.mCurPoints[aPtIdx] = GlobalPIEffect.TransformFPoint(aFinalTrans, aBoxPoints[aPtIdx]);
						}
					}
					for (int anEmitterInstanceIdx = 0; anEmitterInstanceIdx < aLayer.mEmitterInstanceVector.Count; anEmitterInstanceIdx++)
					{
						PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[anEmitterInstanceIdx];
						PIEmitterInstance anEmitterInstance = aLayer.mEmitterInstanceVector[anEmitterInstanceIdx];
						int anEmitterCount = 0;
						int aParticleCount = 0;
						int aRemainingPasses = 1;
						while (anEmitterInstance.mVisible && aRemainingPasses > 0)
						{
							anEmitterCount = 0;
							aParticleCount = 0;
							aRemainingPasses--;
							bool isActive = anEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ACTIVE].GetLastKeyframe(mFrameNum) > 0.99f;
							if (!isActive)
							{
								aRemainingPasses = 0;
							}
							else if (!anEmitterInstance.mWasActive)
							{
								aRemainingPasses += (int)((float)anEmitterInstanceDef.mFramesToPreload * anUpdateRate / (float)mFramerate);
							}
							anEmitterInstance.mWasActive = isActive;
							float aFirstActiveTime = anEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ACTIVE].GetNextKeyframeTime(0f);
							float aLastActiveTime = anEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ACTIVE].GetLastKeyframeTime((float)mLastFrameNum + 1f);
							float aLastActiveValue = anEmitterInstanceDef.mValues[(int)PIEmitterInstanceDef.PIEmitterValue.VALUE_ACTIVE].GetLastKeyframe((float)mLastFrameNum + 1f);
							anEmitterInstance.mWithinLifeFrame = mFrameNum >= aFirstActiveTime && (mFrameNum < aLastActiveTime || aLastActiveValue > 0.99f) && (mEmitAfterTimeline || mFrameNum < (float)mLastFrameNum);
							if (isActive || (anEmitterInstanceDef.mIsSuperEmitter && anEmitterInstance.mWithinLifeFrame))
							{
								anEmitterCount++;
							}
							if (anEmitterInstanceDef.mIsSuperEmitter)
							{
								for (int aFreeEmitterIdx = 0; aFreeEmitterIdx < anEmitterInstanceDef.mFreeEmitterIndices.Count; aFreeEmitterIdx++)
								{
									PIEmitter anEmitter = mDef.mEmitterVector[anEmitterInstanceDef.mFreeEmitterIndices[aFreeEmitterIdx]];
									PIParticleDefInstance aParticleDefInstance = anEmitterInstance.mSuperEmitterParticleDefInstanceVector[aFreeEmitterIdx];
									UpdateParticleDef(aLayer, anEmitter, anEmitterInstance, null, aParticleDefInstance, anEmitterInstance.mSuperEmitterGroup, null);
								}
								UpdateParticleGroup(aLayer, anEmitterInstance, anEmitterInstance.mSuperEmitterGroup);
								PIFreeEmitterInstance aChildEmitterInstance = (PIFreeEmitterInstance)anEmitterInstance.mSuperEmitterGroup.mHead;
								while (aChildEmitterInstance != null)
								{
									PIFreeEmitterInstance aChildNext = (PIFreeEmitterInstance)aChildEmitterInstance.mNext;
									PIEmitter anEmitter = aChildEmitterInstance.mEmitterSrc;
									for (int aParticleDefIdx = 0; aParticleDefIdx < anEmitter.mParticleDefVector.Count; aParticleDefIdx++)
									{
										PIParticleDef aParticleDef = anEmitter.mParticleDefVector[aParticleDefIdx];
										PIParticleDefInstance aParticleDefInstance = aChildEmitterInstance.mEmitter.mParticleDefInstanceVector[aParticleDefIdx];
										UpdateParticleDef(aLayer, anEmitter, anEmitterInstance, aParticleDef, aParticleDefInstance, aChildEmitterInstance.mEmitter.mParticleGroup, aChildEmitterInstance);
									}
									UpdateParticleGroup(aLayer, anEmitterInstance, aChildEmitterInstance.mEmitter.mParticleGroup);
									aParticleCount += aChildEmitterInstance.mEmitter.mParticleGroup.mCount;
									anEmitterCount++;
									aChildEmitterInstance = aChildNext;
								}
							}
							else
							{
								PIEmitter anEmitter = mDef.mEmitterVector[anEmitterInstanceDef.mEmitterDefIdx];
								for (int aParticleDefIdx = 0; aParticleDefIdx < anEmitter.mParticleDefVector.Count; aParticleDefIdx++)
								{
									PIParticleGroup aParticleGroup = anEmitterInstance.mParticleGroup;
									PIParticleDef aParticleDef = anEmitter.mParticleDefVector[aParticleDefIdx];
									PIParticleDefInstance aParticleDefInstance = anEmitterInstance.mParticleDefInstanceVector[aParticleDefIdx];
									UpdateParticleDef(aLayer, anEmitter, anEmitterInstance, aParticleDef, aParticleDefInstance, aParticleGroup, null);
								}
								UpdateParticleGroup(aLayer, anEmitterInstance, anEmitterInstance.mParticleGroup);
								aParticleCount += anEmitterInstance.mParticleGroup.mCount;
							}
						}
						mCurNumEmitters += anEmitterCount;
						mCurNumParticles += aParticleCount;
					}
				}
				isFirstFrame = false;
			}
		}

		public void DrawDarkenLayer(Graphics g, PILayer theLayer)
		{
			g.PushState();
			g.SetColorizeImages(true);
			PILayerDef aLayerDef = theLayer.mLayerDef;
			for (int anEmitterInstanceIdx = 0; anEmitterInstanceIdx < theLayer.mEmitterInstanceVector.Count; anEmitterInstanceIdx++)
			{
				PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[anEmitterInstanceIdx];
				PIEmitterInstance anEmitterInstance = theLayer.mEmitterInstanceVector[anEmitterInstanceIdx];
				if (!anEmitterInstance.mVisible)
				{
					continue;
				}
				if (anEmitterInstanceDef.mIsSuperEmitter)
				{
					for (int aFreeEmitterIdx = 0; aFreeEmitterIdx < anEmitterInstanceDef.mFreeEmitterIndices.Count; aFreeEmitterIdx++)
					{
						for (PIFreeEmitterInstance aChildEmitterInstance = (PIFreeEmitterInstance)anEmitterInstance.mSuperEmitterGroup.mHead; aChildEmitterInstance != null; aChildEmitterInstance = (PIFreeEmitterInstance)aChildEmitterInstance.mNext)
						{
							DrawParticleGroup(g, theLayer, anEmitterInstance, aChildEmitterInstance.mEmitter.mParticleGroup, true);
						}
					}
				}
				else
				{
					DrawParticleGroup(g, theLayer, anEmitterInstance, anEmitterInstance.mParticleGroup, true);
				}
			}
			g.PopState();
		}

		public void DrawLayer(Graphics g, PILayer theLayer)
		{
			g.PushState();
			g.SetColorizeImages(true);
			PILayerDef aLayerDef = theLayer.mLayerDef;
			for (int anEmitterInstanceIdx = 0; anEmitterInstanceIdx < theLayer.mEmitterInstanceVector.Count; anEmitterInstanceIdx++)
			{
				PIEmitterInstanceDef anEmitterInstanceDef = aLayerDef.mEmitterInstanceDefVector[anEmitterInstanceIdx];
				PIEmitterInstance anEmitterInstance = theLayer.mEmitterInstanceVector[anEmitterInstanceIdx];
				if (!anEmitterInstance.mVisible)
				{
					continue;
				}
				for (int aPass = 0; aPass < 2; aPass++)
				{
					bool isDarkeningPass = (aPass == 0);
					if (anEmitterInstanceDef.mIsSuperEmitter)
					{
						for (int aFreeEmitterIdx = 0; aFreeEmitterIdx < anEmitterInstanceDef.mFreeEmitterIndices.Count; aFreeEmitterIdx++)
						{
							for (PIFreeEmitterInstance aChildEmitterInstance = (PIFreeEmitterInstance)anEmitterInstance.mSuperEmitterGroup.mHead; aChildEmitterInstance != null; aChildEmitterInstance = (PIFreeEmitterInstance)aChildEmitterInstance.mNext)
							{
								DrawParticleGroup(g, theLayer, anEmitterInstance, aChildEmitterInstance.mEmitter.mParticleGroup, isDarkeningPass);
							}
						}
					}
					else
					{
						DrawParticleGroup(g, theLayer, anEmitterInstance, anEmitterInstance.mParticleGroup, isDarkeningPass);
					}
				}
			}
			g.PopState();
		}

        private Vector2[] mTempBlockerPoints = new Vector2[512];
        private Vector2[,] mTempTris = new Vector2[256, 3];
        private SexyVertex2D[] mTempVertices = new SexyVertex2D[3];
        // Per-frame Force box points; reused to avoid array alloc in PIEffect.Update.
        private static readonly Vector2[] sForceBoxPoints = new Vector2[5]
        {
            new Vector2(-1f, -1f),
            new Vector2(1f, -1f),
            new Vector2(1f, 1f),
            new Vector2(-1f, 1f),
            new Vector2(0f, 0f)
        };
        public void DrawPhisycalLayer(Graphics g, PILayer theLayer)
		{
			g.PushState();
			g.SetColorizeImages(true);
			PILayerDef mLayerDef = theLayer.mLayerDef;
			g.SetDrawMode(0);
			for (int i = 0; i < mLayerDef.mBlockerVector.Count; i++)
			{
				PIBlocker aBlocker = mLayerDef.mBlockerVector[i];
				bool flag = aBlocker.mActive.GetLastKeyframe(mFrameNum) > 0.99f;
				if (!mDebug && !flag)
				{
					continue;
				}
				SexyTransform2D aTransform = new SexyTransform2D(false);
				float valueAt = aBlocker.mAngle.GetValueAt(mFrameNum);
				if (valueAt != 0f)
				{
					aTransform.RotateDeg(valueAt);
				}
				Vector2 valueAt2 = aBlocker.mPos.GetValueAt(mFrameNum);
				aTransform.Translate(valueAt2.X, valueAt2.Y);
				Vector2 valueAt3 = mLayerDef.mOffset.GetValueAt(mFrameNum);
				aTransform.Translate(valueAt3.X, valueAt3.Y);
				float valueAt4 = mLayerDef.mAngle.GetValueAt(mFrameNum);
				if (valueAt4 != 0f)
				{
					aTransform.RotateDeg(valueAt4);
				}
				SexyTransform2D theMatrix = mDrawTransform * aTransform;
                //Vector2[] array = new Vector2[512];
                //int num = Math.Min(512, aBlocker.mPoints.Count);
                //for (int j = 0; j < num; j++)
                //{
                //	array[j] = GlobalPIEffect.TransformFPoint(theMatrix, aBlocker.mPoints[j].GetValueAt(mFrameNum));
                //}
                //Vector2[,] array2 = new Vector2[256, 3];
                int num = Math.Min(512, aBlocker.mPoints.Count);
                for (int j = 0; j < num; j++)
                {
                    mTempBlockerPoints[j] = GlobalPIEffect.TransformFPoint(theMatrix, aBlocker.mPoints[j].GetValueAt(mFrameNum));
                }
                int theNumTris = 0;
                Common.DividePoly(mTempBlockerPoints, num, mTempTris, 256, ref theNumTris);
				//Common.DividePoly(array, num, array2, 256, ref theNumTris);
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
							array3[l] = new SexyVertex2D(mTempTris[k, l].X, mTempTris[k, l].Y, (mTempTris[k, l].X + theLayer.mBkgImgDrawOfs.X) / (float)theLayer.mBkgImage.mWidth, (mTempTris[k, l].Y + theLayer.mBkgImgDrawOfs.Y) / (float)theLayer.mBkgImage.mHeight);
						}
						g.SetColor(Color.White);
						g.DrawTriangleTex(theLayer.mBkgImage, array3[0], array3[1], array3[2]);
					}
					else
					{
						Vector2[] array4 = new Vector2[3];
						for (int m = 0; m < 3; m++)
						{
							array4[m] = mTempTris[k, m];
						}
						g.SetColor(mBkgColor);
					}
				}
			}
			for (int n = 0; n < mLayerDef.mDeflectorVector.Count; n++)
			{
				PIDeflector aDeflector = mLayerDef.mDeflectorVector[n];
				bool flag2 = aDeflector.mActive.GetLastKeyframe(mFrameNum) > 0.99f;
				if ((!aDeflector.mVisible || !flag2) && !mDebug)
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
				for (int num2 = 1; num2 < aDeflector.mCurPoints.Count; num2++)
				{
					Vector2 vector = aDeflector.mCurPoints[num2 - 1];
					Vector2 vector2 = aDeflector.mCurPoints[num2];
					if (aDeflector.mThickness <= 1.5f)
					{
						g.DrawLine((int)vector.X, (int)vector.Y, (int)vector2.X, (int)vector2.Y);
						continue;
					}
					SexyVector2 sexyVector = new SexyVector2(vector2.X - vector.X, vector2.Y - vector.Y).Normalize().Perp();
					Vector2 vector3 = GlobalPIEffect.TransformFPoint(thePoint: new Vector2(sexyVector.x, sexyVector.y), theMatrix: mDrawTransform);
					Vector2[] array5 = new Vector2[4]
					{
						vector + vector3 * aDeflector.mThickness,
						vector2 + vector3 * aDeflector.mThickness,
						vector2 - vector3 * aDeflector.mThickness,
						vector - vector3 * aDeflector.mThickness
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
				PIForce aForce = mLayerDef.mForceVector[num4];
				bool flag3 = aForce.mActive.GetLastKeyframe(mFrameNum) > 0.99f;
				if ((aForce.mVisible && flag3) || mDebug)
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
						Vector2 vector4 = aForce.mCurPoints[num5];
						Vector2 vector5 = aForce.mCurPoints[(num5 + 1) % 4];
						g.DrawLine((int)vector4.X, (int)vector4.Y, (int)vector5.X, (int)vector5.Y);
					}
					float num6 = MathHelper.ToRadians(0f - aForce.mDirection.GetValueAt(mFrameNum)) + MathHelper.ToRadians(0f - aForce.mAngle.GetValueAt(mFrameNum));
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
						Vector2 vector6 = GlobalPIEffect.TransformFPoint(transform.GetMatrix(), array6[num7]) + aForce.mCurPoints[4];
						Vector2 vector7 = GlobalPIEffect.TransformFPoint(transform.GetMatrix(), array6[(num7 + 1) % 3]) + aForce.mCurPoints[4];
						g.DrawLine((int)vector6.X, (int)vector6.Y, (int)vector7.X, (int)vector7.Y);
					}
				}
			}
			g.PopState();
		}

		public void Draw(Graphics g)
		{
			mLastDrawnPixelCount = 0;
			for (int aLayerIdx = 0; aLayerIdx < mDef.mLayerDefVector.Count; aLayerIdx++)
			{
				PILayer aLayer = mLayerVector[aLayerIdx];
				if (aLayer.mVisible)
				{
					DrawLayer(g, aLayer);
					DrawPhisycalLayer(g, aLayer);
				}
			}
			mLastDrawnPixelCount *= (int)GlobalPIEffect.GetMatrixScale(mDrawTransform);
		}

		public void Draw(Graphics g, bool isDarkenise)
		{
			mLastDrawnPixelCount = 0;
			for (int aLayerIdx = 0; aLayerIdx < mDef.mLayerDefVector.Count; aLayerIdx++)
			{
				PILayer aLayer = mLayerVector[aLayerIdx];
				if (aLayer.mVisible)
				{
					if (isDarkenise)
					{
						DrawDarkenLayer(g, aLayer);
					}
					else
					{
						DrawLayer(g, aLayer);
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