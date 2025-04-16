using System;
using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class Emitter : MovableObject
	{
		public int mSuperEmitterIndex;

		public int mPoolIndex;

		public MemoryImage mAreaMask;

		public bool mInvertAreaMask;

		public System mSystem;

		public List<LineEmitterPoint> mLineEmitterPoints = new List<LineEmitterPoint>();

		public TimeLine mScaleTimeLine = new TimeLine();

		public TimeLine mSettingsTimeLine = new TimeLine();

		public List<Particle> mParticles = new List<Particle>();

		public List<ParticleTypeInfo> mParticleTypeInfo = new List<ParticleTypeInfo>();

		public List<Emitter> mEmitters = new List<Emitter>();

		public List<FreeEmitterInfo> mFreeEmitterInfo = new List<FreeEmitterInfo>();

		public Dictionary<int, ParticleSettings> mLastPTFrameSetting = new Dictionary<int, ParticleSettings>();

		public Dictionary<FreeEmitter, FreeEmitterSettings> mLastFEFrameSetting = new Dictionary<FreeEmitter, FreeEmitterSettings>();

		public EmitterSettings mLastSettings;

		public EmitterScale mLastScale;

		public MemoryImage mDebugMaskImage;

		public Point[] mMaskPoints;

		public FreeEmitter mParentEmitter;

		public Emitter mSuperEmitter;

		public float mNumSpawnAccumulator;

		public bool mDisableQuadRep;

		public int mFrameOffset;

		public int mEmitterType;

		public int mNumMaskPoints;

		public int mLastEmitAtX;

		public int mLastEmitAtY;

		public int mHandle;

		public WaypointManager mWaypointManager;

		public Rect mCullingRect = default(Rect);

		public Rect mClipRect = default(Rect);

		public string mName = "";

		public bool mDrawNewestFirst;

		public bool mWaitForParticles;

		public bool mUseAlternateCalcMethod;

		public bool mDeleteInvisParticles;

		public bool mParticlesMustHaveBeenVisible;

		public bool mEmissionCoordsAreOffsets;

		public int mPreloadFrames;

		public int mStartFrame;

		public int mEmitDir;

		public int mEmitAtXPoints;

		public int mEmitAtYPoints;

		public int mSerialIndex;

		public bool mLinearEmitAtPoints;

		public Color mTintColor = default(Color);

		public void Serialize(SexyFramework.Misc.Buffer b, GlobalMembers.GetIdByImageFunc f)
		{
			base.Serialize(b);
			b.WriteLong(mHandle);
			b.WriteBoolean(mDisableQuadRep);
			b.WriteBoolean(mUseAlternateCalcMethod);
			b.WriteFloat(mNumSpawnAccumulator);
			b.WriteLong(mStartFrame);
			b.WriteBoolean(mParticlesMustHaveBeenVisible);
			b.WriteLong(mLineEmitterPoints.Count);
			for (int i = 0; i < mLineEmitterPoints.Count; i++)
			{
				mLineEmitterPoints[i].Serialize(b);
			}
			mScaleTimeLine.Serialize(b);
			mSettingsTimeLine.Serialize(b);
			b.WriteLong(mParticleTypeInfo.Count);
			for (int j = 0; j < mParticleTypeInfo.Count; j++)
			{
				mParticleTypeInfo[j].first.Serialize(b, f);
				b.WriteLong(mParticleTypeInfo[j].second);
			}
			b.WriteLong(mParticles.Count);
			for (int k = 0; k < mParticles.Count; k++)
			{
				mParticles[k].Serialize(b);
			}
			b.WriteLong(mFreeEmitterInfo.Count);
			for (int l = 0; l < mFreeEmitterInfo.Count; l++)
			{
				mFreeEmitterInfo[l].first.Serialize(b, f);
				b.WriteLong(mFreeEmitterInfo[l].second);
			}
			b.WriteLong(mEmitters.Count);
			for (int m = 0; m < mEmitters.Count; m++)
			{
				mEmitters[m].Serialize(b, f);
			}
			b.WriteLong(mLastPTFrameSetting.Count);
			foreach (KeyValuePair<int, ParticleSettings> item in mLastPTFrameSetting)
			{
				b.WriteLong(item.Key);
				item.Value.Serialize(b);
			}
			b.WriteLong(mLastFEFrameSetting.Count);
			foreach (KeyValuePair<FreeEmitter, FreeEmitterSettings> item2 in mLastFEFrameSetting)
			{
				b.WriteLong(item2.Key.mSerialIndex);
				item2.Value.Serialize(b);
			}
			mLastSettings.Serialize(b);
			mLastScale.Serialize(b);
			if (mAreaMask != null)
			{
				b.WriteLong(f(mAreaMask));
			}
			else
			{
				b.WriteLong(-1L);
			}
			b.WriteBoolean(mInvertAreaMask);
			b.WriteLong(mFrameOffset);
			b.WriteLong(mEmitterType);
			b.WriteLong(mLastEmitAtX);
			b.WriteLong(mLastEmitAtY);
			mWaypointManager.Serialize(b);
			b.WriteLong(mCullingRect.mX);
			b.WriteLong(mCullingRect.mY);
			b.WriteLong(mCullingRect.mWidth);
			b.WriteLong(mCullingRect.mHeight);
			b.WriteLong(mClipRect.mX);
			b.WriteLong(mClipRect.mY);
			b.WriteLong(mClipRect.mWidth);
			b.WriteLong(mClipRect.mHeight);
			b.WriteString(mName);
			b.WriteBoolean(mDrawNewestFirst);
			b.WriteBoolean(mWaitForParticles);
			b.WriteBoolean(mDeleteInvisParticles);
			b.WriteBoolean(mEmissionCoordsAreOffsets);
			b.WriteLong(mPreloadFrames);
			b.WriteLong(mEmitDir);
			b.WriteLong(mEmitAtXPoints);
			b.WriteLong(mEmitAtYPoints);
			b.WriteLong(mSerialIndex);
			b.WriteBoolean(mLinearEmitAtPoints);
			b.WriteLong(mTintColor.ToInt());
			if (mParentEmitter == null)
			{
				b.WriteLong(-1L);
			}
			else
			{
				b.WriteLong(mParentEmitter.mSerialIndex);
			}
			if (mSuperEmitter == null)
			{
				b.WriteLong(-1L);
			}
			else
			{
				b.WriteLong(mSuperEmitter.mSerialIndex);
			}
		}

		public void Deserialize(SexyFramework.Misc.Buffer b, Dictionary<int, Deflector> deflector_ptr_map, Dictionary<int, FreeEmitter> fe_ptr_map, GlobalMembers.GetImageByIdFunc f)
		{
			base.Deserialize(b, deflector_ptr_map);
			Clear();
			mHandle = (int)b.ReadLong();
			mDisableQuadRep = b.ReadBoolean();
			mUseAlternateCalcMethod = b.ReadBoolean();
			mNumSpawnAccumulator = b.ReadFloat();
			mStartFrame = (int)b.ReadLong();
			mParticlesMustHaveBeenVisible = b.ReadBoolean();
			mSettingsTimeLine.mCurrentSettings = null;
			mScaleTimeLine.mCurrentSettings = null;
			Init();
			int num = (int)b.ReadLong();
			for (int i = 0; i < num; i++)
			{
				LineEmitterPoint lineEmitterPoint = new LineEmitterPoint();
				lineEmitterPoint.Deserialize(b);
				mLineEmitterPoints.Add(lineEmitterPoint);
			}
			mScaleTimeLine.Deserialize(b, EmitterScale.Instantiate);
			mSettingsTimeLine.Deserialize(b, EmitterSettings.Instantiate);
			num = (int)b.ReadLong();
			Dictionary<int, ParticleType> dictionary = new Dictionary<int, ParticleType>();
			for (int j = 0; j < num; j++)
			{
				ParticleType particleType = new ParticleType();
				particleType.Deserialize(b, f);
				dictionary.Add(particleType.mSerialIndex, particleType);
				int s = (int)b.ReadLong();
				mParticleTypeInfo.Add(new ParticleTypeInfo(particleType, s));
			}
			num = (int)b.ReadLong();
			for (int k = 0; k < num; k++)
			{
				Particle particle = mSystem.AllocateParticle();
				particle.Deserialize(b, deflector_ptr_map, dictionary);
				mParticles.Add(particle);
			}
			Dictionary<int, FreeEmitter> dictionary2 = new Dictionary<int, FreeEmitter>();
			num = (int)b.ReadLong();
			for (int l = 0; l < num; l++)
			{
				FreeEmitter freeEmitter = new FreeEmitter();
				freeEmitter.Deserialize(b, f);
				int s2 = (int)b.ReadLong();
				mFreeEmitterInfo.Add(new FreeEmitterInfo(freeEmitter, s2));
				dictionary2.Add(freeEmitter.mSerialIndex, freeEmitter);
			}
			num = (int)b.ReadLong();
			for (int m = 0; m < num; m++)
			{
				Emitter emitter = new Emitter();
				emitter.Deserialize(b, deflector_ptr_map, dictionary2, f);
				mEmitters.Add(emitter);
			}
			num = (int)b.ReadLong();
			for (int n = 0; n < num; n++)
			{
				int key = (int)b.ReadLong();
				ParticleSettings particleSettings = new ParticleSettings();
				particleSettings.Deserialize(b);
				if (dictionary.ContainsKey(key))
				{
					ParticleType particleType2 = dictionary[key];
					mLastPTFrameSetting.Add(key, particleSettings);
				}
			}
			num = (int)b.ReadLong();
			for (int num2 = 0; num2 < num; num2++)
			{
				int index = (int)b.ReadLong();
				FreeEmitterSettings freeEmitterSettings = new FreeEmitterSettings();
				freeEmitterSettings.Deserialize(b);
				mLastFEFrameSetting.Add(mFreeEmitterInfo[index].first, freeEmitterSettings);
			}
			mLastSettings.Deserialize(b);
			mLastScale.Deserialize(b);
			int num3 = (int)b.ReadLong();
			mInvertAreaMask = b.ReadBoolean();
			if (num3 != -1)
			{
				mAreaMask = (MemoryImage)f(num3);
				SetAreaMask(mAreaMask, mInvertAreaMask);
			}
			mFrameOffset = (int)b.ReadLong();
			mEmitterType = (int)b.ReadLong();
			mLastEmitAtX = (int)b.ReadLong();
			mLastEmitAtY = (int)b.ReadLong();
			mWaypointManager.Deserialize(b);
			mCullingRect.mX = (int)b.ReadLong();
			mCullingRect.mY = (int)b.ReadLong();
			mCullingRect.mWidth = (int)b.ReadLong();
			mCullingRect.mHeight = (int)b.ReadLong();
			mClipRect.mX = (int)b.ReadLong();
			mClipRect.mY = (int)b.ReadLong();
			mClipRect.mWidth = (int)b.ReadLong();
			mClipRect.mHeight = (int)b.ReadLong();
			mName = b.ReadString();
			mDrawNewestFirst = b.ReadBoolean();
			mWaitForParticles = b.ReadBoolean();
			mDeleteInvisParticles = b.ReadBoolean();
			mEmissionCoordsAreOffsets = b.ReadBoolean();
			mPreloadFrames = (int)b.ReadLong();
			mEmitDir = (int)b.ReadLong();
			mEmitAtXPoints = (int)b.ReadLong();
			mEmitAtYPoints = (int)b.ReadLong();
			mSerialIndex = (int)b.ReadLong();
			mLinearEmitAtPoints = b.ReadBoolean();
			int theColor = (int)b.ReadLong();
			mTintColor = new Color(theColor);
			int num4 = (int)b.ReadLong();
			if (num4 != -1 && fe_ptr_map.ContainsKey(num4))
			{
				mParentEmitter = fe_ptr_map[num4];
			}
			mSuperEmitterIndex = (int)b.ReadLong();
		}

		protected void UpdateLineEmitter(int frame)
		{
			for (int i = 0; i < Common.size(mLineEmitterPoints); i++)
			{
				LineEmitterPoint lineEmitterPoint = mLineEmitterPoints[i];
				PointKeyFrame pointKeyFrame = null;
				PointKeyFrame pointKeyFrame2 = null;
				for (int j = 0; j < Common.size(lineEmitterPoint.mKeyFramePoints); j++)
				{
					if (lineEmitterPoint.mKeyFramePoints[j].first <= frame)
					{
						pointKeyFrame = lineEmitterPoint.mKeyFramePoints[j];
						continue;
					}
					pointKeyFrame2 = lineEmitterPoint.mKeyFramePoints[j];
					break;
				}
				float num;
				if (pointKeyFrame2 == null)
				{
					num = 0f;
					pointKeyFrame2 = pointKeyFrame;
				}
				else
				{
					num = (float)(frame - pointKeyFrame.first) / (float)(pointKeyFrame2.first - pointKeyFrame.first);
				}
				lineEmitterPoint.mCurX = (float)pointKeyFrame.second.mX + (float)(pointKeyFrame2.second.mX - pointKeyFrame.second.mX) * num;
				lineEmitterPoint.mCurY = (float)pointKeyFrame.second.mY + (float)(pointKeyFrame2.second.mY - pointKeyFrame.second.mY) * num;
			}
		}

		protected int GetEmissionCoord(ref float x, ref float y, ref float angle)
		{
			int num = -1;
			if (mEmitterType == 0)
			{
				x = mX;
				y = mY;
				num = -1;
			}
			else if (mEmitterType == 1)
			{
				if (mEmitAtXPoints == 0 || Common.size(mLineEmitterPoints) == 1)
				{
					if (Common.size(mLineEmitterPoints) == 1)
					{
						x = mLineEmitterPoints[0].mCurX;
						y = mLineEmitterPoints[0].mCurY;
						num = 0;
					}
					else
					{
						int num2 = Common.Rand() % (Common.size(mLineEmitterPoints) - 1);
						GetXYFromLineIdx(num2, Common.FloatRange(0f, 1f), ref x, ref y);
						num = num2;
					}
				}
				else
				{
					int num3 = 0;
					List<int> list = new List<int>();
					for (int i = 1; i < Common.size(mLineEmitterPoints); i++)
					{
						int num4 = (int)Common.Distance(mLineEmitterPoints[i].mCurX, mLineEmitterPoints[i].mCurY, mLineEmitterPoints[i - 1].mCurX, mLineEmitterPoints[i - 1].mCurY, false);
						list.Add(num4);
						num3 += num4;
					}
					float num5 = ((mEmitAtXPoints == 1) ? 0f : ((float)num3 / (float)(mEmitAtXPoints - 1)));
					int num6 = (mLinearEmitAtPoints ? (mLastEmitAtX++ % mEmitAtXPoints) : (Common.Rand() % mEmitAtXPoints));
					int num7 = (int)Math.Ceiling((float)num6 * num5);
					int num8 = Common.size(mLineEmitterPoints) - 2;
					for (int j = 1; j < Common.size(mLineEmitterPoints); j++)
					{
						if (num7 <= list[j - 1])
						{
							num8 = j - 1;
							break;
						}
						num7 -= list[j - 1];
					}
					float pct = (float)num7 / (float)list[num8];
					GetXYFromLineIdx(num8, pct, ref x, ref y);
					num = num8;
				}
			}
			else if (mEmitterType == 2)
			{
				if (mEmitAtXPoints == 0)
				{
					angle = 0f - Common.DegreesToRadians(Common.Rand() % 360);
				}
				else
				{
					int num9 = (mLinearEmitAtPoints ? (mLastEmitAtX++ % mEmitAtXPoints) : (Common.Rand() % mEmitAtXPoints));
					float num10 = 360f / (float)mEmitAtXPoints;
					angle = 0f - Common.DegreesToRadians((float)num9 * num10);
				}
				float num11 = (float)Math.Sin(0f - mLastSettings.mAngle);
				float num12 = (float)Math.Cos(0f - mLastSettings.mAngle);
				float num13 = (float)Math.Sin(angle);
				float num14 = (float)Math.Cos(angle);
				x = mLastSettings.mXRadius * num14 * num12 - mLastSettings.mYRadius * num13 * num11;
				y = mLastSettings.mXRadius * num14 * num11 + mLastSettings.mYRadius * num13 * num12;
				angle *= -1f;
				num = 0;
			}
			else if (mEmitterType == 3)
			{
				if (mMaskPoints != null)
				{
					int num15 = Common.Rand() % mNumMaskPoints;
					x = (float)mMaskPoints[num15].mX + mX - (float)mDebugMaskImage.mWidth / 2f;
					y = (float)mMaskPoints[num15].mY + mY - (float)mDebugMaskImage.mHeight / 2f;
					if (!new Rect((int)(mX - mLastSettings.mXRadius / 2f), (int)(mY - mLastSettings.mYRadius / 2f), (int)mLastSettings.mXRadius, (int)mLastSettings.mYRadius).Contains((int)x, (int)y))
					{
						num = -1;
					}
				}
				else if (mEmitAtXPoints == 0 || mEmitAtYPoints == 0)
				{
					x = mX - mLastSettings.mXRadius / 2f + (float)(Common.Rand() % (int)mLastSettings.mXRadius);
					y = mY - mLastSettings.mYRadius / 2f + (float)(Common.Rand() % (int)mLastSettings.mYRadius);
					num = 0;
				}
				else
				{
					int num16 = (mLinearEmitAtPoints ? (mLastEmitAtX++ % mEmitAtXPoints) : (Common.Rand() % mEmitAtXPoints));
					int num17 = (mLinearEmitAtPoints ? (mLastEmitAtY++ % mEmitAtYPoints) : (Common.Rand() % mEmitAtYPoints));
					if (mEmitAtXPoints == 1)
					{
						x = mX;
					}
					else
					{
						float num18 = mLastSettings.mXRadius / (float)(mEmitAtXPoints - 1);
						x = mX - mLastSettings.mXRadius / 2f + (float)num16 * num18;
					}
					if (mEmitAtYPoints == 1)
					{
						y = mY;
					}
					else
					{
						float num19 = mLastSettings.mYRadius / (float)(mEmitAtYPoints - 1);
						y = mY - mLastSettings.mYRadius / 2f + (float)num17 * num19;
					}
					num = 0;
				}
				if (num != -1)
				{
					Common.RotatePoint(mLastSettings.mAngle, ref x, ref y, mX, mY);
				}
			}
			if (num != -1 && mEmitterType != 3 && (mParentEmitter != null || mEmissionCoordsAreOffsets))
			{
				x += mX;
				y += mY;
			}
			return num;
		}

		protected void GetXYFromLineIdx(int idx, float pct, ref float x, ref float y)
		{
			int num = (int)(mLineEmitterPoints[idx + 1].mCurX - mLineEmitterPoints[idx].mCurX);
			int num2 = (int)(mLineEmitterPoints[idx + 1].mCurY - mLineEmitterPoints[idx].mCurY);
			x = mLineEmitterPoints[idx].mCurX + (float)num * pct;
			y = mLineEmitterPoints[idx].mCurY + (float)num2 * pct;
		}

		protected void Clear()
		{
			if (mWaypointManager != null)
			{
				mWaypointManager.Dispose();
			}
			for (int i = 0; i < Common.size(mParticles); i++)
			{
				mSystem.DeleteParticle(mParticles[i]);
			}
			for (int j = 0; j < Common.size(mEmitters); j++)
			{
				if (mEmitters[j] != null)
				{
					mEmitters[j].Dispose();
				}
			}
			mParticles.Clear();
			mEmitters.Clear();
			mParticleTypeInfo.Clear();
			mFreeEmitterInfo.Clear();
			mLastFEFrameSetting.Clear();
			mLastFEFrameSetting.Clear();
			mWaypointManager = null;
		}

		protected void Init()
		{
			mSettingsTimeLine.mCurrentSettings = new EmitterSettings();
			mScaleTimeLine.mCurrentSettings = new EmitterScale();
			mLastSettings = (EmitterSettings)mSettingsTimeLine.mCurrentSettings;
			mLastScale = (EmitterScale)mScaleTimeLine.mCurrentSettings;
			mWaypointManager = new WaypointManager();
			mParticles.Capacity = 500;
			mEmitters.Capacity = 100;
		}

		protected void SpawnParticles(int frame)
		{
			for (int i = 0; i < Common.size(mParticleTypeInfo); i++)
			{
				ParticleTypeInfo particleTypeInfo = mParticleTypeInfo[i];
				ParticleType first = particleTypeInfo.first;
				if (first.mSingle && first.mNumCreated > 0)
				{
					continue;
				}
				int life_frames = 0;
				float emit_frame = 0f;
				ParticleSettings kfdata = null;
				ParticleVariance vardata = null;
				first.GetCreationParameters(frame, out life_frames, out emit_frame, out kfdata, out vardata);
				mLastPTFrameSetting[first.mSerialIndex] = kfdata;
				if (!((float)(frame - particleTypeInfo.second) >= emit_frame) && !mUseAlternateCalcMethod)
				{
					continue;
				}
				particleTypeInfo.second = frame;
				int num = 0;
				if (!mUseAlternateCalcMethod)
				{
					num = (int)(Math.Ceiling(1f / emit_frame) * (double)mLastScale.mNumberScale * (double)mCurrentLifetimeSettings.mNumberMult);
					if (!GlobalMembers.gSexyAppBase.Is3DAccelerated())
					{
						num = (int)((float)num * mSystem.mParticleScale2D);
					}
					if ((num <= 0 && mLastScale.mNumberScale > 0f && mCurrentLifetimeSettings.mNumberMult > 0f) || first.mSingle)
					{
						num = 1;
					}
				}
				else
				{
					float num2 = ((float)kfdata.mNumber + Common.SAFE_RAND(vardata.mNumberVar)) * mLastScale.mNumberScale * mCurrentLifetimeSettings.mNumberMult / 6.6666665f;
					mNumSpawnAccumulator += num2 * (GlobalMembers.gSexyAppBase.Is3DAccelerated() ? 1f : mSystem.mParticleScale2D);
					if (mNumSpawnAccumulator >= 1f)
					{
						num = (int)mNumSpawnAccumulator;
						mNumSpawnAccumulator -= num;
					}
				}
				if (life_frames == 0)
				{
					continue;
				}
				for (int j = 0; j < num; j++)
				{
					first.mNumCreated++;
					life_frames = first.GetRandomizedLife();
					float angle = 0f;
					float x = 0f;
					float y = 0f;
					if (!GetLaunchSettings(ref angle, ref x, ref y))
					{
						continue;
					}
					float velocity = mLastScale.mVelocityScale * mLastScale.mZoom * ((float)kfdata.mVelocity + Common.SAFE_RAND(vardata.mVelocityVar)) / 100f;
					Particle particle = mSystem.AllocateParticle();
					if (!first.mSingle)
					{
						particle.Reset(angle, velocity);
					}
					else
					{
						particle.Reset(0f, 0f);
					}
					particle.mColorKeyManager.CopyFrom(first.mColorKeyManager);
					particle.mAlphaKeyManager.CopyFrom(first.mAlphaKeyManager);
					particle.mParentType = first;
					particle.mLockSizeAspect = first.mLockSizeAspect;
					particle.mImage = first.mImage;
					particle.mImageRate = first.mImageRate;
					if (first.mRandomStartCel && first.mImage != null)
					{
						particle.mImageCel = ((particle.mImage.mNumCols > particle.mImage.mNumRows) ? (Common.Rand() % particle.mImage.mNumCols) : (Common.Rand() % particle.mImage.mNumRows));
					}
					particle.mAdditive = first.mAdditive;
					particle.mAdditiveWithNormal = first.mAdditiveWithNormal;
					if (life_frames != -1)
					{
						particle.mLife = (first.mSingle ? (-1) : ((int)(mLastScale.mLifeScale * (float)life_frames)));
					}
					else
					{
						particle.mLife = -1;
					}
					particle.mRefXOff = first.mRefXOff;
					particle.mRefYOff = first.mRefYOff;
					particle.mMotionAngleOffset = first.mMotionAngleOffset;
					particle.mAlignAngleToMotion = first.mAlignAngleToMotion;
					if (particle.mLife != -1)
					{
						particle.mColorKeyManager.SetLife(particle.mLife);
						particle.mAlphaKeyManager.SetLife(particle.mLife);
					}
					particle.mAngle = first.GetSpawnAngle();
					if (first.mNumSameColorKeyInRow > 0)
					{
						particle.mColorKeyManager.SetFixedColor(first.GetNextKeyColor());
					}
					particle.mMotionRand = (first.mSingle ? 0f : (mLastScale.mMotionRandScale * (kfdata.mMotionRand + Common.FloatRange(0f, vardata.mMotionRandVar)) / 100f));
					particle.SetXY(x, y);
					particle.mParentName = first.mName;
					particle.mWeight = (first.mSingle ? 0f : (mLastScale.mWeightScale * (kfdata.mWeight - Common.SAFE_RAND(vardata.mWeightVar / 2) + Common.SAFE_RAND(vardata.mWeightVar / 2)) / ModVal.M(2000f)));
					particle.mSpin = mLastScale.mSpinScale * (kfdata.mSpin - Common.FloatRange(0f, vardata.mSpinVar / 2f) + Common.FloatRange(0f, vardata.mSpinVar / 2f)) / 10f;
					particle.mBounce = mLastScale.mBounceScale * ((float)kfdata.mBounce - Common.SAFE_RAND(vardata.mBounceVar / 2) + Common.SAFE_RAND(vardata.mBounceVar / 2));
					if (particle.mBounce < 0f)
					{
						particle.mBounce = 0f;
					}
					particle.mFlipX = first.mFlipX;
					particle.mFlipY = first.mFlipY;
					if (particle.mImage != null)
					{
						int num3 = (int)((float)kfdata.mXSize * mSystem.mScale) + (int)Common.SAFE_RAND((float)vardata.mSizeXVar * mSystem.mScale);
						particle.mCurXSize = mLastScale.mSizeXScale * mLastScale.mZoom * ((float)num3 / (float)particle.mImage.GetCelWidth());
						if (!first.mLockSizeAspect)
						{
							num3 = (int)((float)kfdata.mYSize * mSystem.mScale) + (int)Common.SAFE_RAND((float)vardata.mSizeYVar * mSystem.mScale);
						}
						particle.mCurYSize = mLastScale.mSizeYScale * mLastScale.mZoom * ((float)num3 / (float)particle.mImage.GetCelHeight());
					}
					for (int k = 0; k < Common.size(first.mLifePctSettings); k++)
					{
						particle.AddLifetimeKeyFrame(first.mLifePctSettings[k].first, new LifetimeSettings(first.mLifePctSettings[k].second));
					}
					mParticles.Add(particle);
				}
			}
		}

		protected void SpawnEmitters(int frame)
		{
			for (int i = 0; i < Common.size(mFreeEmitterInfo); i++)
			{
				FreeEmitterInfo freeEmitterInfo = mFreeEmitterInfo[i];
				FreeEmitter first = freeEmitterInfo.first;
				int emitter_life = 0;
				float emit_frame = 0f;
				FreeEmitterSettings settings = null;
				FreeEmitterVariance variance = null;
				first.GetCreationParams(frame, out emitter_life, out emit_frame, out settings, out variance);
				float num = (float)settings.mZoom / 100f;
				mLastFEFrameSetting[first] = settings;
				if (!((float)(frame - freeEmitterInfo.second) >= emit_frame))
				{
					continue;
				}
				freeEmitterInfo.second = frame;
				int num2 = (int)(Math.Ceiling(1f / emit_frame) * (double)mLastScale.mNumberScale * (double)mCurrentLifetimeSettings.mNumberMult);
				if (num2 <= 0 && mLastScale.mNumberScale > 0f && mCurrentLifetimeSettings.mNumberMult > 0f)
				{
					num2 = 1;
				}
				for (int j = 0; j < num2; j++)
				{
					emitter_life = first.GetRandomizedLife();
					float angle = 0f;
					float x = 0f;
					float y = 0f;
					if (!GetLaunchSettings(ref angle, ref x, ref y))
					{
						continue;
					}
					float velocity = mLastScale.mVelocityScale * mLastScale.mZoom * num * ((float)settings.mVelocity + Common.SAFE_RAND(variance.mVelocityVar)) / 100f;
					Emitter emitter = new Emitter(first.mEmitter);
					emitter.mSystem = mSystem;
					mEmitters.Add(emitter);
					emitter.Launch(angle, velocity);
					emitter.mParentEmitter = first;
					emitter.mSuperEmitter = this;
					emitter.mLife = ((emitter_life == -1) ? (-1) : ((int)(mLastScale.mLifeScale * (float)emitter_life)));
					emitter.mX = x;
					emitter.mY = y;
					emitter.mMotionRand = mLastScale.mMotionRandScale * (float)(settings.mMotionRand + Common.IntRange(0, variance.mMotionRandVar)) / 100f;
					emitter.mWeight = mLastScale.mWeightScale * ((float)settings.mWeight - Common.SAFE_RAND(variance.mWeightVar / 2) + Common.SAFE_RAND(variance.mWeightVar / 2)) / ModVal.M(2000f);
					emitter.mSpin = mLastScale.mSpinScale * (settings.mSpin - Common.FloatRange(0f, variance.mSpinVar / 2f) + Common.FloatRange(0f, variance.mSpinVar / 2f)) / 10f;
					emitter.mBounce = mLastScale.mBounceScale * ((float)settings.mBounce - Common.SAFE_RAND(variance.mBounceVar / 2) + Common.SAFE_RAND(variance.mBounceVar));
					if (emitter.mBounce < 0f)
					{
						emitter.mBounce = 0f;
					}
					for (int k = 0; k < Common.size(first.mLifePctSettings); k++)
					{
						emitter.AddLifetimeKeyFrame(first.mLifePctSettings[k].second.mPct, new LifetimeSettings(first.mLifePctSettings[k].second));
					}
					for (int l = 0; l < Common.size(emitter.mScaleTimeLine.mKeyFrames); l++)
					{
						EmitterScale emitterScale = emitter.mScaleTimeLine.mKeyFrames[i].second as EmitterScale;
						emitterScale.mSizeXScale -= Common.SAFE_RAND(variance.mSizeXVar / 2) / 100f;
						emitterScale.mSizeXScale += Common.SAFE_RAND(variance.mSizeXVar / 2) / 100f;
						emitterScale.mSizeYScale -= Common.SAFE_RAND((first.mAspectLocked ? variance.mSizeXVar : variance.mSizeYVar) / 2) / 100f;
						emitterScale.mSizeYScale += Common.SAFE_RAND((first.mAspectLocked ? variance.mSizeXVar : variance.mSizeYVar) / 2) / 100f;
						emitterScale.mSizeXScale *= num;
						emitterScale.mSizeYScale *= num;
						if (emitterScale.mSizeXScale < 0f)
						{
							emitterScale.mSizeXScale = 0f;
						}
						if (emitterScale.mSizeYScale < 0f)
						{
							emitterScale.mSizeYScale = 0f;
						}
						emitterScale.mVelocityScale *= num;
						emitterScale.mZoom -= Common.SAFE_RAND(variance.mZoomVar / 2) / 100f;
						emitterScale.mZoom += Common.SAFE_RAND(variance.mZoomVar / 2) / 100f;
						emitterScale.mZoom *= num;
						if (emitterScale.mZoom < 0f)
						{
							emitterScale.mZoom = 0f;
						}
					}
				}
			}
		}

		protected bool GetLaunchSettings(ref float angle, ref float x, ref float y)
		{
			float num = Common.FloatRange(0f, mLastSettings.mEmissionRange / 2f);
			float num2 = Common.FloatRange(0f, mLastSettings.mEmissionRange / 2f);
			angle = mLastSettings.mEmissionAngle - num + num2;
			x = mX;
			y = mY;
			float angle2 = 0f;
			int emissionCoord = GetEmissionCoord(ref x, ref y, ref angle2);
			if (mEmitterType == 1 && emissionCoord >= 0 && Common.size(mLineEmitterPoints) > 1)
			{
				float mCurX = mLineEmitterPoints[emissionCoord].mCurX;
				float mCurY = mLineEmitterPoints[emissionCoord].mCurY;
				float mCurX2 = mLineEmitterPoints[emissionCoord + 1].mCurX;
				float mCurY2 = mLineEmitterPoints[emissionCoord + 1].mCurY;
				if (mEmitDir == 0 || (mEmitDir == 2 && Common.Rand() % 100 < 50))
				{
					angle += Common.AngleBetweenPoints(mCurX, mCurY, mCurX2, mCurY2);
				}
				else
				{
					angle = Common.AngleBetweenPoints(mCurX, mCurY, mCurX2, mCurY2) - angle;
				}
			}
			else if (mEmitterType == 2)
			{
				angle2 = ((mEmitDir != 1 && (mEmitDir != 2 || Common.Rand() % 100 >= 50)) ? (angle2 - Common.JL_PI / 2f) : (angle2 + Common.JL_PI / 2f));
				angle = angle2 - angle;
			}
			else if (mEmitterType == 3 && emissionCoord == -1)
			{
				return false;
			}
			angle += mLastSettings.mAngle;
			return true;
		}

		public Emitter()
		{
			mPreloadFrames = 0;
			mDrawNewestFirst = false;
			mEmitterType = 0;
			mEmitAtXPoints = 0;
			mEmitAtYPoints = 0;
			mEmitDir = 2;
			mNumMaskPoints = 0;
			mMaskPoints = null;
			mDebugMaskImage = null;
			mLastSettings = null;
			mParentEmitter = null;
			mSuperEmitter = null;
			mFrameOffset = 0;
			mWaitForParticles = false;
			mSystem = null;
			mPoolIndex = -1;
			mEmissionCoordsAreOffsets = false;
			mLastEmitAtX = 0;
			mLastEmitAtY = 0;
			mDeleteInvisParticles = false;
			mParticlesMustHaveBeenVisible = false;
			mSerialIndex = -1;
			mAreaMask = null;
			mSuperEmitterIndex = -1;
			mHandle = -1;
			mDisableQuadRep = true;
			mUseAlternateCalcMethod = false;
			mNumSpawnAccumulator = 0f;
			mStartFrame = 0;
			Init();
		}

		public Emitter(Emitter rhs)
			: this()
		{
			CopyFrom(rhs);
		}

		public override void Dispose()
		{
			Clear();
			base.Dispose();
			mMaskPoints = null;
			mDebugMaskImage = null;
		}

		public void CopyFrom(Emitter rhs)
		{
			CopyFrom((MovableObject)rhs);
			mTintColor = rhs.mTintColor;
			mEmitAtXPoints = rhs.mEmitAtXPoints;
			mEmitAtYPoints = rhs.mEmitAtYPoints;
			mEmitDir = rhs.mEmitDir;
			mPreloadFrames = rhs.mPreloadFrames;
			mDrawNewestFirst = rhs.mDrawNewestFirst;
			mClipRect = rhs.mClipRect;
			mCullingRect = rhs.mCullingRect;
			mNumMaskPoints = rhs.mNumMaskPoints;
			mEmitterType = rhs.mEmitterType;
			mParentEmitter = rhs.mParentEmitter;
			mWaitForParticles = rhs.mWaitForParticles;
			mSystem = rhs.mSystem;
			mDisableQuadRep = rhs.mDisableQuadRep;
			mDeleteInvisParticles = rhs.mDeleteInvisParticles;
			mUseAlternateCalcMethod = rhs.mUseAlternateCalcMethod;
			mNumSpawnAccumulator = rhs.mNumSpawnAccumulator;
			mStartFrame = rhs.mStartFrame;
			mParticlesMustHaveBeenVisible = rhs.mParticlesMustHaveBeenVisible;
			mMaskPoints = null;
			mDebugMaskImage = null;
			mAreaMask = rhs.mAreaMask;
			mInvertAreaMask = rhs.mInvertAreaMask;
			mNumMaskPoints = rhs.mNumMaskPoints;
			if (mNumMaskPoints > 0)
			{
				mMaskPoints = new Point[mNumMaskPoints];
				for (int i = 0; i < mNumMaskPoints; i++)
				{
					mMaskPoints[i] = new Point(rhs.mMaskPoints[i]);
				}
				mDebugMaskImage = new MemoryImage(rhs.mDebugMaskImage);
			}
			mScaleTimeLine = rhs.mScaleTimeLine;
			mSettingsTimeLine = rhs.mSettingsTimeLine;
			mLastScale = (EmitterScale)mScaleTimeLine.mCurrentSettings;
			mLastSettings = (EmitterSettings)mSettingsTimeLine.mCurrentSettings;
			Clear();
			for (int j = 0; j < Common.size(rhs.mParticleTypeInfo); j++)
			{
				AddParticleType(new ParticleType(rhs.mParticleTypeInfo[j].first));
			}
			for (int k = 0; k < Common.size(rhs.mFreeEmitterInfo); k++)
			{
				AddFreeEmitter(new FreeEmitter(rhs.mFreeEmitterInfo[k].first));
			}
			mLineEmitterPoints = rhs.mLineEmitterPoints;
			mWaypointManager = new WaypointManager(rhs.mWaypointManager);
		}

		public void ResetForReuse()
		{
			mWaitForParticles = false;
			mPoolIndex = -1;
			mLastEmitAtX = 0;
			mLastEmitAtY = 0;
			mNumSpawnAccumulator = 0f;
			for (int i = 0; i < Common.size(mParticles); i++)
			{
				mSystem.DeleteParticle(mParticles[i]);
			}
			mParticles.Clear();
			for (int j = 0; j < mEmitters.Count; j++)
			{
				mEmitters[j].ResetForReuse();
			}
			for (int k = 0; k < mParticleTypeInfo.Count; k++)
			{
				mParticleTypeInfo[k].first.ResetForReuse();
			}
		}

		public EmitterScale AddScaleKeyFrame(int frame, EmitterScale scale, int second_frame_time, bool make_new)
		{
			mScaleTimeLine.AddKeyFrame(frame, scale);
			if (second_frame_time != -1)
			{
				mScaleTimeLine.AddKeyFrame(second_frame_time, new EmitterScale(scale));
				if (make_new)
				{
					return new EmitterScale(scale);
				}
			}
			return null;
		}

		public EmitterScale AddScaleKeyFrame(int frame, EmitterScale scale, int second_frame_time)
		{
			return AddScaleKeyFrame(frame, scale, second_frame_time, false);
		}

		public EmitterScale AddScaleKeyFrame(int frame, EmitterScale scale)
		{
			return AddScaleKeyFrame(frame, scale, -1, false);
		}

		public EmitterSettings AddSettingsKeyFrame(int frame, EmitterSettings settings, int second_frame_time, bool make_new)
		{
			mSettingsTimeLine.AddKeyFrame(frame, settings);
			if (second_frame_time != -1)
			{
				mSettingsTimeLine.AddKeyFrame(second_frame_time, new EmitterSettings(settings));
				if (make_new)
				{
					return new EmitterSettings(settings);
				}
			}
			return null;
		}

		public EmitterSettings AddSettingsKeyFrame(int frame, EmitterSettings settings, int second_frame_time)
		{
			return AddSettingsKeyFrame(frame, settings, second_frame_time, false);
		}

		public EmitterSettings AddSettingsKeyFrame(int frame, EmitterSettings settings)
		{
			return AddSettingsKeyFrame(frame, settings, -1, false);
		}

		public int AddParticleType(ParticleType pt)
		{
			if (pt.mImage != null)
			{
				if (mDisableQuadRep)
				{
					((MemoryImage)pt.mImage).AddImageFlags(128u);
				}
				else
				{
					((MemoryImage)pt.mImage).RemoveImageFlags(128u);
				}
			}
			if (pt.mSingle)
			{
				pt.mEmitterAttachPct = 1f;
			}
			if (pt.GetSettingsTimeLineSize() == 0)
			{
				pt.AddSettingsKeyFrame(0, new ParticleSettings());
			}
			if (pt.GetVarTimeLineSize() == 0)
			{
				pt.AddVarianceKeyFrame(0, new ParticleVariance());
			}
			if (pt.mColorKeyManager.GetColorMode() == 0 && pt.mColorKeyManager.GetNumKeys() > 0)
			{
				pt.mColorKeyManager.SetColorMode(1);
			}
			if (pt.mColorKeyManager.GetColorMode() == 1 && !pt.mColorKeyManager.HasMaxIndex())
			{
				pt.mColorKeyManager.AddColorKey(1f, pt.mColorKeyManager.GetColorByIndex(pt.mColorKeyManager.GetNumKeys() - 1));
			}
			if (pt.mAlphaKeyManager.GetColorMode() == 0 && pt.mAlphaKeyManager.GetNumKeys() > 0)
			{
				pt.mAlphaKeyManager.SetColorMode(1);
			}
			if (pt.mAlphaKeyManager.GetColorMode() == 1 && !pt.mAlphaKeyManager.HasMaxIndex())
			{
				pt.mAlphaKeyManager.AddColorKey(1f, pt.mAlphaKeyManager.GetColorByIndex(pt.mAlphaKeyManager.GetNumKeys() - 1));
			}
			mParticleTypeInfo.Add(new ParticleTypeInfo(pt, 0));
			pt.mSerialIndex = Common.size(mParticleTypeInfo) - 1;
			mLastPTFrameSetting[pt.mSerialIndex] = new ParticleSettings();
			return Common.size(mParticleTypeInfo) - 1;
		}

		public void AddFreeEmitter(FreeEmitter f)
		{
			mFreeEmitterInfo.Add(new FreeEmitterInfo(f, 0));
			mLastFEFrameSetting[f] = new FreeEmitterSettings();
			f.mSerialIndex = Common.size(mFreeEmitterInfo) - 1;
		}

		public void Update(int frame, bool allow_creation)
		{
			if (mLife > 0 && mUpdateCount >= mLife)
			{
				allow_creation = false;
			}
			frame -= mFrameOffset;
			float num = mX;
			float num2 = mY;
			base.Update();
			float num3 = mX;
			float num4 = mY;
			if (Dead())
			{
				return;
			}
			if (mWaypointManager.GetNumPoints() > 0)
			{
				mWaypointManager.Update(frame);
				SetPos(mWaypointManager.GetLastPoint().X, mWaypointManager.GetLastPoint().Y);
			}
			else
			{
				mX = num;
				mY = num2;
				Move(num3 - num, num4 - num2);
			}
			mScaleTimeLine.Update(frame);
			mSettingsTimeLine.Update(frame);
			bool flag = Common.size(mSettingsTimeLine.mKeyFrames) == 0 || frame >= Common.back(mSettingsTimeLine.mKeyFrames).first;
			mLastScale.mSizeXScale *= mCurrentLifetimeSettings.mSizeXMult;
			mLastScale.mSizeYScale *= mCurrentLifetimeSettings.mSizeYMult;
			mLastScale.mZoom *= mCurrentLifetimeSettings.mZoomMult;
			if (!mLastSettings.mActive)
			{
				for (int i = 0; i < Common.size(mParticles); i++)
				{
					mSystem.DeleteParticle(mParticles[i]);
				}
				mParticles.Clear();
			}
			if (mEmitterType == 1)
			{
				UpdateLineEmitter(frame);
			}
			if (allow_creation && mLastSettings.mActive)
			{
				SpawnEmitters(frame);
				SpawnParticles(frame);
			}
			for (int j = 0; j < Common.size(mParticles); j++)
			{
				mParticles[j].Update();
				bool flag2 = flag && !mParticles[j].mLastFrameWasVisible;
				if (mParticles[j].Dead() || (mDeleteInvisParticles && (!mParticlesMustHaveBeenVisible || mParticles[j].mHasBeenVisible) && flag2) || (mCullingRect != Rect.ZERO_RECT && !mCullingRect.Intersects(mParticles[j].GetRect())))
				{
					mSystem.DeleteParticle(mParticles[j]);
					mParticles.RemoveAt(j);
					j--;
				}
			}
			for (int k = 0; k < Common.size(mEmitters); k++)
			{
				mEmitters[k].Update(frame, allow_creation);
				if (mEmitters[k].Dead())
				{
					mEmitters[k].Dispose();
					mEmitters.RemoveAt(k);
					k--;
				}
			}
		}

		public void Draw(SexyFramework.Graphics.Graphics g, float vis_mult, float tint_mult)
		{
			if (mClipRect != Rect.ZERO_RECT)
			{
				g.PushState();
				g.ClipRect(mClipRect);
			}
			int num = (mDrawNewestFirst ? (Common.size(mParticles) - 1) : 0);
			for (int i = num; mDrawNewestFirst ? (i >= 0) : (i < Common.size(mParticles)); i += ((!mDrawNewestFirst) ? 1 : (-1)))
			{
				Particle particle = mParticles[i];
				if (!particle.Dead() && (!particle.mAdditive || (particle.mAdditive && particle.mAdditiveWithNormal)))
				{
					float alpha_pct = mSystem.mAlphaPct * mLastPTFrameSetting[particle.mParentType.mSerialIndex].mGlobalVisibility * mLastSettings.mVisibility * vis_mult;
					particle.Draw(g, alpha_pct, mTintColor, mLastSettings.mTintStrength * tint_mult, mSystem.mScale);
				}
			}
			for (int j = num; mDrawNewestFirst ? (j >= 0) : (j < Common.size(mParticles)); j += ((!mDrawNewestFirst) ? 1 : (-1)))
			{
				Particle particle2 = mParticles[j];
				if (!particle2.Dead() && particle2.mAdditive)
				{
					float alpha_pct2 = mSystem.mAlphaPct * mLastPTFrameSetting[particle2.mParentType.mSerialIndex].mGlobalVisibility * mLastSettings.mVisibility * vis_mult;
					particle2.Draw(g, alpha_pct2, mTintColor, mLastSettings.mTintStrength * tint_mult, mSystem.mScale);
				}
			}
			num = (mDrawNewestFirst ? (Common.size(mEmitters) - 1) : 0);
			for (int k = num; mDrawNewestFirst ? (k >= 0) : (k < Common.size(mEmitters)); k += ((!mDrawNewestFirst) ? 1 : (-1)))
			{
				Emitter emitter = mEmitters[k];
				if (!emitter.Dead())
				{
					emitter.Draw(g, mLastSettings.mVisibility, mLastSettings.mTintStrength);
				}
			}
			if (mClipRect != Rect.ZERO_RECT)
			{
				g.PopState();
			}
		}

		public void Draw(SexyFramework.Graphics.Graphics g, float vis_mult)
		{
			Draw(g, vis_mult, 1f);
		}

		public void Draw(SexyFramework.Graphics.Graphics g)
		{
			Draw(g, 1f, 1f);
		}

		public void Move(float xamt, float yamt)
		{
			if (mParentEmitter != null || mWaypointManager.GetNumPoints() <= 0)
			{
				mX += xamt;
				mY += yamt;
				for (int i = 0; i < Common.size(mParticles); i++)
				{
					mParticles[i].SetX(mParticles[i].GetX() + xamt * mParticles[i].mParentType.mEmitterAttachPct);
					mParticles[i].SetY(mParticles[i].GetY() + yamt * mParticles[i].mParentType.mEmitterAttachPct);
				}
			}
		}

		public void SetPos(float x, float y)
		{
			float num = mX;
			float num2 = mY;
			mX = x;
			mY = y;
			for (int i = 0; i < Common.size(mParticles); i++)
			{
				Particle particle = mParticles[i];
				particle.SetX(particle.GetX() + (x - num) * particle.mParentType.mEmitterAttachPct);
				particle.SetY(particle.GetY() + (y - num2) * particle.mParentType.mEmitterAttachPct);
			}
		}

		public void SetAreaMask(MemoryImage mask, bool invert)
		{
		}

		public void AddLineEmitterKeyFrame(int point_num, int frame, Point p)
		{
			if (point_num >= Common.size(mLineEmitterPoints))
			{
				Common.Resize(mLineEmitterPoints, point_num + 1);
			}
			mLineEmitterPoints[point_num].mKeyFramePoints.Add(new PointKeyFrame(frame, p));
			mLineEmitterPoints[point_num].mKeyFramePoints.Sort(new SortPointKeyFrames());
		}

		public void DebugDraw(SexyFramework.Graphics.Graphics g, int size)
		{
		}

		public void ApplyForce(Force f)
		{
			for (int i = 0; i < Common.size(mParticles); i++)
			{
				f.Apply(mParticles[i]);
			}
			for (int j = 0; j < Common.size(mEmitters); j++)
			{
				f.Apply(mEmitters[j]);
				mEmitters[j].ApplyForce(f);
			}
		}

		public void ApplyDeflector(Deflector d)
		{
			for (int i = 0; i < Common.size(mParticles); i++)
			{
				d.Apply(mParticles[i]);
			}
			for (int j = 0; j < Common.size(mEmitters); j++)
			{
				d.Apply(mEmitters[j]);
				mEmitters[j].ApplyDeflector(d);
			}
		}

		public void GetParticlesOfType(int particle_type_handle, ref List<Particle> particles)
		{
			ParticleType particleType = GetParticleType(particle_type_handle);
			for (int i = 0; i < Common.size(mParticles); i++)
			{
				if (mParticles[i].mParentType == particleType)
				{
					particles.Add(mParticles[i]);
				}
			}
		}

		public ParticleType GetParticleType(int particle_type_handle)
		{
			return mParticleTypeInfo[particle_type_handle].first;
		}

		public int GetNumParticleTypes()
		{
			return Common.size(mParticleTypeInfo);
		}

		public ParticleType GetParticleTypeByIndex(int idx)
		{
			return mParticleTypeInfo[idx].first;
		}

		public int GetHandle()
		{
			return mHandle;
		}

		public int NumParticles()
		{
			int num = Common.size(mParticles);
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				num += mEmitters[i].NumParticles();
			}
			return num;
		}

		public void SetEmitterType(int t)
		{
			mEmitterType = t;
		}

		public void LoopSettingsTimeLine(bool l)
		{
			mSettingsTimeLine.mLoop = l;
		}

		public void LoopScaleTimeLine(bool l)
		{
			mScaleTimeLine.mLoop = l;
		}

		public override bool Dead()
		{
			bool result = base.Dead();
			if (mWaitForParticles && Common.size(mParticles) > 0)
			{
				return false;
			}
			return result;
		}

		public virtual bool Active()
		{
			if (mLastSettings != null)
			{
				return mLastSettings.mActive;
			}
			return false;
		}

		public void DisableQuadRep(bool val)
		{
			mDisableQuadRep = val;
			for (int i = 0; i < Common.size(mParticleTypeInfo); i++)
			{
				if (mParticleTypeInfo[i].first.mImage != null)
				{
					if (val)
					{
						((MemoryImage)mParticleTypeInfo[i].first.mImage).AddImageFlags(128u);
					}
					else
					{
						((MemoryImage)mParticleTypeInfo[i].first.mImage).RemoveImageFlags(128u);
					}
				}
			}
		}

		public override float GetX()
		{
			if (mWaypointManager.GetNumPoints() == 0 || mParentEmitter != null)
			{
				return base.GetX();
			}
			return mWaypointManager.GetLastPoint().X;
		}

		public override float GetY()
		{
			if (mWaypointManager.GetNumPoints() == 0 || mParentEmitter != null)
			{
				return base.GetY();
			}
			return mWaypointManager.GetLastPoint().Y;
		}

		public override bool CanInteract()
		{
			if (mWaypointManager.GetNumPoints() == 0 || mParentEmitter != null)
			{
				return base.CanInteract();
			}
			return false;
		}

		public int GetNumFreeEmitters()
		{
			return Common.size(mFreeEmitterInfo);
		}

		public int GetNumSingleParticles()
		{
			int num = 0;
			for (int i = 0; i < Common.size(mParticles); i++)
			{
				if (mParticles[i].mParentType.mSingle)
				{
					num++;
				}
			}
			return num;
		}

		public Emitter GetEmitter(int idx)
		{
			return mFreeEmitterInfo[idx].first.mEmitter;
		}

		public System GetSystem()
		{
			return mSystem;
		}
	}
}
