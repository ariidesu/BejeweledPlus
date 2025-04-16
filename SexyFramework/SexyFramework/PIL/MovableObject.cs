using System;
using System.Collections.Generic;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class MovableObject : IDisposable
	{
		protected List<LifetimeSettingKeyFrame> mLifetimeKeyFrames = new List<LifetimeSettingKeyFrame>();

		protected LifetimeSettings mCurrentLifetimeSettings = new LifetimeSettings();

		protected int mKeyFrameIndex;

		protected float mOriginalWeight;

		protected float mOriginalBounce;

		protected float mMotionRandAccum;

		protected float mAX;

		protected float mAY;

		protected float mX;

		protected float mY;

		protected bool mInitialized;

		public Dictionary<Deflector, DeflectorCollInfo> mDeflectorCollMap = new Dictionary<Deflector, DeflectorCollInfo>();

		public int mUpdateCount;

		public int mLife;

		public float mVX;

		public float mVY;

		public float mMotionRand;

		public float mWeight;

		public float mAngle;

		public float mSpin;

		public float mBounce;

		protected LifetimeSettings mInterpLifetimeSettings = new LifetimeSettings();

		protected LifetimeSettings GetInterpLifetimeSettings()
		{
			if (Common.size(mLifetimeKeyFrames) == 0)
			{
				return new LifetimeSettings();
			}
			if (mKeyFrameIndex == Common.size(mLifetimeKeyFrames) || mKeyFrameIndex + 1 == Common.size(mLifetimeKeyFrames))
			{
				return mLifetimeKeyFrames[mKeyFrameIndex].second;
			}
			LifetimeSettings second = mLifetimeKeyFrames[mKeyFrameIndex + 1].second;
			LifetimeSettings second2 = mLifetimeKeyFrames[mKeyFrameIndex].second;
			float num = (float)(mUpdateCount - mLifetimeKeyFrames[mKeyFrameIndex].first) / (float)(mLifetimeKeyFrames[mKeyFrameIndex + 1].first - mLifetimeKeyFrames[mKeyFrameIndex].first);
			mInterpLifetimeSettings.Reset();
			mInterpLifetimeSettings.mMotionRandMult += (second.mMotionRandMult - second2.mMotionRandMult) * num;
			mInterpLifetimeSettings.mSizeXMult += (second.mSizeXMult - second2.mSizeXMult) * num;
			mInterpLifetimeSettings.mSizeYMult += (second.mSizeYMult - second2.mSizeYMult) * num;
			mInterpLifetimeSettings.mSpinMult += (second.mSpinMult - second2.mSpinMult) * num;
			mInterpLifetimeSettings.mVelocityMult += (second.mVelocityMult - second2.mVelocityMult) * num;
			mInterpLifetimeSettings.mWeightMult += (second.mWeightMult - second2.mWeightMult) * num;
			mInterpLifetimeSettings.mZoomMult += (second.mZoomMult - second2.mZoomMult) * num;
			mInterpLifetimeSettings.mNumberMult += (second.mNumberMult - second2.mNumberMult) * num;
			return mInterpLifetimeSettings;
		}

		public MovableObject()
		{
			Reset();
		}

		public MovableObject(MovableObject rhs)
			: this()
		{
			CopyFrom(rhs);
		}

		public virtual void Dispose()
		{
			for (int i = 0; i < Common.size(mLifetimeKeyFrames); i++)
			{
				mLifetimeKeyFrames[i].second = null;
			}
		}

		public void CopyFrom(MovableObject rhs)
		{
			if (this != rhs)
			{
				mLife = rhs.mLife;
				mVX = rhs.mVX;
				mVY = rhs.mVY;
				mMotionRand = rhs.mMotionRand;
				mX = rhs.mX;
				mY = rhs.mY;
				mWeight = rhs.mWeight;
				mAngle = rhs.mAngle;
				mSpin = rhs.mSpin;
				mBounce = rhs.mBounce;
				mUpdateCount = 0;
				mKeyFrameIndex = 0;
				mOriginalWeight = rhs.mOriginalWeight;
				mOriginalBounce = rhs.mOriginalBounce;
				mMotionRandAccum = 0f;
				mAX = 0f;
				mAY = 0f;
				mInitialized = rhs.mInitialized;
				for (int i = 0; i < mLifetimeKeyFrames.Count; i++)
				{
					mLifetimeKeyFrames[i].second = null;
				}
				mLifetimeKeyFrames.Clear();
				for (int j = 0; j < Common.size(rhs.mLifetimeKeyFrames); j++)
				{
					AddLifetimeKeyFrame((mLife == 0) ? 0f : ((float)rhs.mLifetimeKeyFrames[j].first / (float)mLife), new LifetimeSettings(rhs.mLifetimeKeyFrames[j].second));
				}
			}
		}

		public virtual void Serialize(SexyFramework.Misc.Buffer b)
		{
			b.WriteLong(mKeyFrameIndex);
			b.WriteFloat(mOriginalWeight);
			b.WriteFloat(mOriginalBounce);
			b.WriteFloat(mMotionRandAccum);
			b.WriteFloat(mAX);
			b.WriteFloat(mAY);
			b.WriteFloat(mX);
			b.WriteFloat(mY);
			b.WriteBoolean(mInitialized);
			b.WriteLong(mUpdateCount);
			b.WriteLong(mLife);
			b.WriteFloat(mVX);
			b.WriteFloat(mVY);
			b.WriteFloat(mMotionRand);
			b.WriteFloat(mWeight);
			b.WriteFloat(mAngle);
			b.WriteFloat(mSpin);
			b.WriteFloat(mBounce);
			mCurrentLifetimeSettings.Serialize(b);
			b.WriteLong(mLifetimeKeyFrames.Count);
			for (int i = 0; i < mLifetimeKeyFrames.Count; i++)
			{
				b.WriteLong(mLifetimeKeyFrames[i].first);
				mLifetimeKeyFrames[i].second.Serialize(b);
			}
			b.WriteLong(mDeflectorCollMap.Count);
			foreach (KeyValuePair<Deflector, DeflectorCollInfo> item in mDeflectorCollMap)
			{
				b.WriteLong(item.Key.mSerialIndex);
				b.WriteLong(item.Value.mLastCollFrame);
				b.WriteBoolean(item.Value.mIgnoresDeflector);
			}
		}

		public virtual void Deserialize(SexyFramework.Misc.Buffer b, Dictionary<int, Deflector> deflector_ptr_map)
		{
			mKeyFrameIndex = (int)b.ReadLong();
			mOriginalWeight = b.ReadFloat();
			mOriginalBounce = b.ReadFloat();
			mMotionRandAccum = b.ReadFloat();
			mAX = b.ReadFloat();
			mAY = b.ReadFloat();
			mX = b.ReadFloat();
			mY = b.ReadFloat();
			mInitialized = b.ReadBoolean();
			mUpdateCount = (int)b.ReadLong();
			mLife = (int)b.ReadLong();
			mVX = b.ReadFloat();
			mVY = b.ReadFloat();
			mMotionRand = b.ReadFloat();
			mWeight = b.ReadFloat();
			mAngle = b.ReadFloat();
			mSpin = b.ReadFloat();
			mBounce = b.ReadFloat();
			mCurrentLifetimeSettings.Deserialize(b);
			int num = (int)b.ReadLong();
			mLifetimeKeyFrames.Clear();
			for (int i = 0; i < num; i++)
			{
				int f = (int)b.ReadLong();
				LifetimeSettings lifetimeSettings = new LifetimeSettings();
				lifetimeSettings.Deserialize(b);
				mLifetimeKeyFrames.Add(new LifetimeSettingKeyFrame(f, lifetimeSettings));
			}
			mDeflectorCollMap.Clear();
			num = (int)b.ReadLong();
			for (int j = 0; j < num; j++)
			{
				int key = (int)b.ReadLong();
				int f2 = (int)b.ReadLong();
				bool b2 = b.ReadBoolean();
				if (deflector_ptr_map.ContainsKey(key))
				{
					Deflector key2 = deflector_ptr_map[key];
					mDeflectorCollMap.Add(key2, new DeflectorCollInfo(f2, b2));
				}
			}
		}

		public virtual void Launch(float angle, float velocity)
		{
			mVX = (float)Math.Cos(angle) * velocity;
			mVY = (0f - (float)Math.Sin(angle)) * velocity;
		}

		public virtual LifetimeSettings AddLifetimeKeyFrame(float pct, LifetimeSettings s, float second_frame_pct, bool make_new)
		{
			LifetimeSettingKeyFrame lifetimeSettingKeyFrame = new LifetimeSettingKeyFrame();
			lifetimeSettingKeyFrame.first = (int)(pct * (float)mLife);
			lifetimeSettingKeyFrame.second = s;
			lifetimeSettingKeyFrame.second.mPct = pct;
			mLifetimeKeyFrames.Add(lifetimeSettingKeyFrame);
			mLifetimeKeyFrames.Sort(new LifeFrameSort());
			if (second_frame_pct >= 0f)
			{
				AddLifetimeKeyFrame(second_frame_pct, new LifetimeSettings(s));
				if (make_new)
				{
					return new LifetimeSettings(s);
				}
			}
			return null;
		}

		public virtual LifetimeSettings AddLifetimeKeyFrame(float pct, LifetimeSettings s, float second_frame_pct)
		{
			return AddLifetimeKeyFrame(pct, s, second_frame_pct, false);
		}

		public virtual LifetimeSettings AddLifetimeKeyFrame(float pct, LifetimeSettings s)
		{
			return AddLifetimeKeyFrame(pct, s, -1f, false);
		}

		public virtual void ClearLifetimeFrames()
		{
			mLifetimeKeyFrames.Clear();
			AddLifetimeKeyFrame(0f, new LifetimeSettings(mCurrentLifetimeSettings));
		}

		public virtual void Reset()
		{
			mLife = -1;
			mUpdateCount = 0;
			mMotionRand = 0f;
			mWeight = 0f;
			mAngle = 0f;
			mSpin = 0f;
			mX = 0f;
			mY = 0f;
			mKeyFrameIndex = 0;
			mOriginalWeight = 1f;
			mOriginalBounce = 0f;
			mInitialized = false;
			mMotionRandAccum = 0f;
			mAX = 0f;
			mAY = 0f;
			mVX = 0f;
			mVY = 0f;
			for (int i = 0; i < Common.size(mLifetimeKeyFrames); i++)
			{
				mLifetimeKeyFrames[i] = null;
			}
			mLifetimeKeyFrames.Clear();
			mCurrentLifetimeSettings = new LifetimeSettings();
			mDeflectorCollMap.Clear();
		}

		public virtual void Update()
		{
			if (!mInitialized)
			{
				mInitialized = true;
				mOriginalWeight = mWeight;
				mOriginalBounce = mBounce;
			}
			mUpdateCount++;
			if (!Dead())
			{
				if (Common.size(mLifetimeKeyFrames) > 0 && mKeyFrameIndex + 1 < Common.size(mLifetimeKeyFrames) && mUpdateCount >= mLifetimeKeyFrames[mKeyFrameIndex + 1].first)
				{
					mKeyFrameIndex++;
				}
				mCurrentLifetimeSettings = GetInterpLifetimeSettings();
				mBounce = mOriginalBounce * mCurrentLifetimeSettings.mBounceMult;
				mWeight = mOriginalWeight * mCurrentLifetimeSettings.mWeightMult;
				mVY += mWeight;
				mVX += mAX;
				mVY += mAY;
				mX += mVX * mCurrentLifetimeSettings.mVelocityMult + mMotionRandAccum;
				mY += mVY * mCurrentLifetimeSettings.mVelocityMult + mMotionRandAccum;
				if (mMotionRand > 0f)
				{
					mMotionRandAccum += ((0f - mMotionRand) / 2f + Common.FloatRange(0f, mMotionRand)) * mCurrentLifetimeSettings.mMotionRandMult / 10f;
				}
				mAngle += mSpin * mCurrentLifetimeSettings.mSpinMult;
				mAX = (mAY = 0f);
			}
		}

		public virtual bool Dead()
		{
			if (mLife > 0)
			{
				return mUpdateCount >= mLife;
			}
			return false;
		}

		public virtual void ApplyAcceleration(float ax, float ay)
		{
			mAX += ax;
			mAY += ay;
		}

		public virtual float GetX()
		{
			return mX;
		}

		public virtual float GetY()
		{
			return mY;
		}

		public virtual void SetX(float x)
		{
			mX = x;
		}

		public virtual void SetY(float y)
		{
			mY = y;
		}

		public virtual void SetXY(float x, float y)
		{
			SetX(x);
			SetY(y);
		}

		public virtual bool CanInteract()
		{
			return true;
		}
	}
}
