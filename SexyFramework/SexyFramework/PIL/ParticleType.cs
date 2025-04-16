using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class ParticleType
	{
		public TimeLine mSettingsTimeLine = new TimeLine();

		public TimeLine mVarTimeLine = new TimeLine();

		public int mSameColorKeyCount;

		public int mLastColorKeyIndex;

		public float mLastSpawnAngle = float.MaxValue;

		public bool mImageSetByPINLoader;

		public List<LifetimeSettingPct> mLifePctSettings = new List<LifetimeSettingPct>();

		public Image mImage;

		public string mImageName = "";

		public string mName = "";

		public ColorKeyManager mColorKeyManager;

		public ColorKeyManager mAlphaKeyManager;

		public bool mUseNewLifeRandomization;

		public bool mLockSizeAspect = true;

		public bool mAdditive;

		public bool mAdditiveWithNormal;

		public bool mFlipX;

		public bool mFlipY;

		public bool mLoopTimeline;

		public bool mAlignAngleToMotion;

		public bool mSingle;

		public float mMotionAngleOffset;

		public float mInitAngle;

		public float mAngleRange;

		public float mInitAngleStep;

		public float mEmitterAttachPct;

		public int mNumSameColorKeyInRow;

		public int mNumCreated;

		public int mRefXOff;

		public int mRefYOff;

		public int mImageRate = 1;

		public int mXOff;

		public int mYOff;

		public bool mRandomStartCel;

		public int mSerialIndex = -1;

		public ParticleType()
		{
			mColorKeyManager = new ColorKeyManager();
			mAlphaKeyManager = new ColorKeyManager();
			mSettingsTimeLine.mCurrentSettings = new ParticleSettings();
			mVarTimeLine.mCurrentSettings = new ParticleVariance();
		}

		public ParticleType(ParticleType rhs)
			: this()
		{
			CopyFrom(rhs);
		}

		public virtual void Dispose()
		{
			mColorKeyManager = null;
			mAlphaKeyManager = null;
		}

		public void CopyFrom(ParticleType rhs)
		{
			if (this != rhs && rhs != null)
			{
				mSettingsTimeLine = rhs.mSettingsTimeLine;
				mVarTimeLine = rhs.mVarTimeLine;
				mSameColorKeyCount = rhs.mSameColorKeyCount;
				mLastColorKeyIndex = rhs.mLastColorKeyIndex;
				mImageSetByPINLoader = rhs.mImageSetByPINLoader;
				mInitAngleStep = rhs.mInitAngleStep;
				mLastSpawnAngle = rhs.mLastSpawnAngle;
				mLifePctSettings.Clear();
				for (int i = 0; i < Common.size(rhs.mLifePctSettings); i++)
				{
					AddSettingAtLifePct(rhs.mLifePctSettings[i].first, new LifetimeSettings(rhs.mLifePctSettings[i].second));
				}
				mImageRate = rhs.mImageRate;
				if (mImageSetByPINLoader)
				{
					mImage = GlobalMembers.gSexyAppBase.CopyImage(rhs.mImage);
				}
				else
				{
					mImage = rhs.mImage;
				}
				mImageName = rhs.mImageName;
				mName = rhs.mName;
				mColorKeyManager = rhs.mColorKeyManager;
				mAlphaKeyManager = rhs.mAlphaKeyManager;
				mXOff = rhs.mXOff;
				mYOff = rhs.mYOff;
				mRandomStartCel = rhs.mRandomStartCel;
				mLockSizeAspect = rhs.mLockSizeAspect;
				mAdditive = rhs.mAdditive;
				mAdditiveWithNormal = rhs.mAdditiveWithNormal;
				mFlipX = rhs.mFlipX;
				mFlipY = rhs.mFlipY;
				mLoopTimeline = rhs.mLoopTimeline;
				mAlignAngleToMotion = rhs.mAlignAngleToMotion;
				mSingle = rhs.mSingle;
				mMotionAngleOffset = rhs.mMotionAngleOffset;
				mAngleRange = rhs.mAngleRange;
				mInitAngle = rhs.mInitAngle;
				mEmitterAttachPct = rhs.mEmitterAttachPct;
				mNumSameColorKeyInRow = rhs.mNumSameColorKeyInRow;
				mNumCreated = rhs.mNumCreated;
				mRefXOff = rhs.mRefXOff;
				mRefYOff = rhs.mRefYOff;
			}
		}

		public void ResetForReuse()
		{
			mNumCreated = 0;
		}

		public void Serialize(Buffer b, GlobalMembers.GetIdByImageFunc f)
		{
			mSettingsTimeLine.Serialize(b);
			mVarTimeLine.Serialize(b);
			b.WriteLong(mSameColorKeyCount);
			b.WriteLong(mLastColorKeyIndex);
			b.WriteBoolean(mImageSetByPINLoader);
			b.WriteLong(mLifePctSettings.Count);
			for (int i = 0; i < mLifePctSettings.Count; i++)
			{
				b.WriteFloat(mLifePctSettings[i].first);
				mLifePctSettings[i].second.Serialize(b);
			}
			b.WriteBoolean(mImage != null);
			if (mImage != null)
			{
				b.WriteLong(f(mImage));
			}
			b.WriteString(mImageName);
			b.WriteString(mName);
			mColorKeyManager.Serialize(b);
			mAlphaKeyManager.Serialize(b);
			b.WriteBoolean(mLockSizeAspect);
			b.WriteBoolean(mAdditive);
			b.WriteBoolean(mAdditiveWithNormal);
			b.WriteBoolean(mFlipX);
			b.WriteBoolean(mFlipY);
			b.WriteBoolean(mLoopTimeline);
			b.WriteBoolean(mAlignAngleToMotion);
			b.WriteBoolean(mSingle);
			b.WriteFloat(mMotionAngleOffset);
			b.WriteFloat(mInitAngle);
			b.WriteFloat(mAngleRange);
			b.WriteFloat(mEmitterAttachPct);
			b.WriteLong(mNumSameColorKeyInRow);
			b.WriteLong(mNumCreated);
			b.WriteLong(mRefXOff);
			b.WriteLong(mRefYOff);
			b.WriteLong(mImageRate);
			b.WriteLong(mSerialIndex);
			b.WriteFloat(mInitAngleStep);
			b.WriteFloat(mLastSpawnAngle);
			b.WriteBoolean(mRandomStartCel);
			b.WriteLong(mXOff);
			b.WriteLong(mYOff);
		}

		public void Deserialize(Buffer b, GlobalMembers.GetImageByIdFunc f)
		{
			mSettingsTimeLine.Deserialize(b, ParticleSettings.Instantiate);
			mVarTimeLine.Deserialize(b, ParticleVariance.Instantiate);
			mSameColorKeyCount = (int)b.ReadLong();
			mLastColorKeyIndex = (int)b.ReadLong();
			mImageSetByPINLoader = b.ReadBoolean();
			int num = (int)b.ReadLong();
			for (int i = 0; i < num; i++)
			{
				float f2 = b.ReadFloat();
				LifetimeSettings lifetimeSettings = new LifetimeSettings();
				lifetimeSettings.Deserialize(b);
				mLifePctSettings.Add(new LifetimeSettingPct(f2, lifetimeSettings));
			}
			mImage = null;
			if (b.ReadBoolean())
			{
				mImage = f((int)b.ReadLong());
			}
			mImageName = b.ReadString();
			mName = b.ReadString();
			mColorKeyManager.Deserialize(b);
			mAlphaKeyManager.Deserialize(b);
			mLockSizeAspect = b.ReadBoolean();
			mAdditive = b.ReadBoolean();
			mAdditiveWithNormal = b.ReadBoolean();
			mFlipX = b.ReadBoolean();
			mFlipY = b.ReadBoolean();
			mLoopTimeline = b.ReadBoolean();
			mAlignAngleToMotion = b.ReadBoolean();
			mSingle = b.ReadBoolean();
			mMotionAngleOffset = b.ReadFloat();
			mInitAngle = b.ReadFloat();
			mAngleRange = b.ReadFloat();
			mEmitterAttachPct = b.ReadFloat();
			mNumSameColorKeyInRow = (int)b.ReadLong();
			mNumCreated = (int)b.ReadLong();
			mRefXOff = (int)b.ReadLong();
			mRefYOff = (int)b.ReadLong();
			mImageRate = (int)b.ReadLong();
			mSerialIndex = (int)b.ReadLong();
			mInitAngleStep = b.ReadFloat();
			mLastSpawnAngle = b.ReadFloat();
			mRandomStartCel = b.ReadBoolean();
			mXOff = (int)b.ReadLong();
			mYOff = (int)b.ReadLong();
		}

		public void GetCreationParameters(int current_frame, out int life_frames, out float emit_frame, out ParticleSettings kfdata, out ParticleVariance vardata)
		{
			mSettingsTimeLine.Update(current_frame);
			mVarTimeLine.Update(current_frame);
			if (Common.size(mSettingsTimeLine.mKeyFrames) == 0)
			{
				kfdata = new ParticleSettings();
				mSettingsTimeLine.mCurrentSettings = kfdata;
			}
			else
			{
				kfdata = (ParticleSettings)mSettingsTimeLine.mCurrentSettings;
			}
			if (Common.size(mVarTimeLine.mKeyFrames) == 0)
			{
				vardata = new ParticleVariance();
				mVarTimeLine.mCurrentSettings = vardata;
			}
			else
			{
				vardata = (ParticleVariance)mVarTimeLine.mCurrentSettings;
			}
			if (kfdata.mLife == -1)
			{
				life_frames = -1;
			}
			else
			{
				int mLife = kfdata.mLife;
				Common.SAFE_RAND(vardata.mLifeVar / 2);
				Common.SAFE_RAND(vardata.mLifeVar / 2);
				life_frames = GetRandomizedLife();
				if (life_frames < 0)
				{
					life_frames = 0;
				}
			}
			float num = kfdata.mNumber + (int)Common.SAFE_RAND(vardata.mNumberVar);
			if (num == 0f)
			{
				emit_frame = float.MaxValue;
			}
			else
			{
				emit_frame = 100f / num;
			}
		}

		public int GetRandomizedLife()
		{
			ParticleSettings particleSettings = mSettingsTimeLine.mCurrentSettings as ParticleSettings;
			ParticleVariance particleVariance = mVarTimeLine.mCurrentSettings as ParticleVariance;
			int num = 0;
			if (particleSettings.mLife == -1)
			{
				return -1;
			}
			num = (mUseNewLifeRandomization ? (particleSettings.mLife + (int)Common.SAFE_RAND(particleVariance.mLifeVar)) : (particleSettings.mLife - (int)Common.SAFE_RAND(particleVariance.mLifeVar / 2) + (int)Common.SAFE_RAND(particleVariance.mLifeVar / 2)));
			if (num < 0)
			{
				num = 0;
			}
			return 10 * num;
		}

		public ParticleSettings AddSettingsKeyFrame(int frame_time, ParticleSettings @params, int second_frame_time, bool make_new)
		{
			mSettingsTimeLine.AddKeyFrame(frame_time, @params);
			if (second_frame_time != -1)
			{
				ParticleSettings k = new ParticleSettings(@params);
				mSettingsTimeLine.AddKeyFrame(second_frame_time, k);
				if (make_new)
				{
					return new ParticleSettings(@params);
				}
			}
			return null;
		}

		public ParticleSettings AddSettingsKeyFrame(int frame_time, ParticleSettings @params, int second_frame_time)
		{
			return AddSettingsKeyFrame(frame_time, @params, second_frame_time, false);
		}

		public ParticleSettings AddSettingsKeyFrame(int frame_time, ParticleSettings @params)
		{
			return AddSettingsKeyFrame(frame_time, @params, -1, false);
		}

		public ParticleVariance AddVarianceKeyFrame(int frame_time, ParticleVariance @params, int second_frame_time, bool make_new)
		{
			mVarTimeLine.AddKeyFrame(frame_time, @params);
			if (second_frame_time != -1)
			{
				ParticleVariance k = new ParticleVariance(@params);
				mSettingsTimeLine.AddKeyFrame(second_frame_time, k);
				if (make_new)
				{
					return new ParticleVariance(@params);
				}
			}
			return null;
		}

		public ParticleVariance AddVarianceKeyFrame(int frame_time, ParticleVariance @params, int second_frame_time)
		{
			return AddVarianceKeyFrame(frame_time, @params, second_frame_time, false);
		}

		public ParticleVariance AddVarianceKeyFrame(int frame_time, ParticleVariance @params)
		{
			return AddVarianceKeyFrame(frame_time, @params, -1, false);
		}

		public ParticleSettings GetSettingsKeyFrame(int frame_time)
		{
			for (int i = 0; i < Common.size(mSettingsTimeLine.mKeyFrames); i++)
			{
				if (mSettingsTimeLine.mKeyFrames[i].first == frame_time)
				{
					return (ParticleSettings)mSettingsTimeLine.mKeyFrames[i].second;
				}
			}
			return null;
		}

		public ParticleVariance GetVarianceKeyFrame(int frame_time)
		{
			for (int i = 0; i < Common.size(mVarTimeLine.mKeyFrames); i++)
			{
				if (mVarTimeLine.mKeyFrames[i].first == frame_time)
				{
					return (ParticleVariance)mVarTimeLine.mKeyFrames[i].second;
				}
			}
			return null;
		}

		public bool EndOfSettingsTimeLine(int frame)
		{
			if (Common.size(mSettingsTimeLine.mKeyFrames) == 0)
			{
				return true;
			}
			return frame >= Common.back(mSettingsTimeLine.mKeyFrames).first;
		}

		public bool EndOfVarianceTimeLine(int frame)
		{
			if (Common.size(mVarTimeLine.mKeyFrames) == 0)
			{
				return true;
			}
			return frame >= Common.back(mVarTimeLine.mKeyFrames).first;
		}

		public float GetSpawnAngle()
		{
			if (mInitAngleStep == 0f)
			{
				return mInitAngle - Common.FloatRange(0f, mAngleRange / 2f) + Common.FloatRange(0f, mAngleRange / 2f);
			}
			if (Common._eq(mLastSpawnAngle, float.MaxValue))
			{
				mLastSpawnAngle = mInitAngle - Common.FloatRange(0f, mAngleRange / 2f) + Common.FloatRange(0f, mAngleRange / 2f);
			}
			else
			{
				mLastSpawnAngle += mInitAngleStep;
				if (Common._geq(mLastSpawnAngle, mInitAngle + mAngleRange / 2f))
				{
					mLastSpawnAngle -= mAngleRange;
				}
			}
			return mLastSpawnAngle;
		}

		public void LoopTimeLine(bool l)
		{
			mSettingsTimeLine.mLoop = l;
		}

		public void LoopVarTimeLine(bool l)
		{
			mVarTimeLine.mLoop = l;
		}

		public LifetimeSettings AddSettingAtLifePct(float pct, LifetimeSettings s, float second_frame_pct, bool make_new)
		{
			mLifePctSettings.Add(new LifetimeSettingPct(pct, s));
			mLifePctSettings.Sort(new LifePctSort());
			if (second_frame_pct >= 0f)
			{
				AddSettingAtLifePct(second_frame_pct, new LifetimeSettings(s), -1f, false);
				if (make_new)
				{
					return new LifetimeSettings(s);
				}
			}
			return null;
		}

		public LifetimeSettings AddSettingAtLifePct(float pct, LifetimeSettings s, float second_frame_pct)
		{
			return AddSettingAtLifePct(pct, s, second_frame_pct, false);
		}

		public LifetimeSettings AddSettingAtLifePct(float pct, LifetimeSettings s)
		{
			return AddSettingAtLifePct(pct, s, -1f, false);
		}

		public int GetSettingsTimeLineSize()
		{
			return Common.size(mSettingsTimeLine.mKeyFrames);
		}

		public int GetVarTimeLineSize()
		{
			return Common.size(mVarTimeLine.mKeyFrames);
		}

		public Color GetNextKeyColor()
		{
			if (mNumSameColorKeyInRow <= 0)
			{
				return Color.White;
			}
			if (++mSameColorKeyCount >= mNumSameColorKeyInRow)
			{
				mSameColorKeyCount = 0;
				mLastColorKeyIndex = (mLastColorKeyIndex + 1) % mColorKeyManager.GetNumKeys();
			}
			return mColorKeyManager.GetColorByIndex(mLastColorKeyIndex);
		}
	}
}
