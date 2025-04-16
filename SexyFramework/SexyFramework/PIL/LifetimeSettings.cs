using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class LifetimeSettings
	{
		public float mSizeXMult = 1f;

		public float mSizeYMult = 1f;

		public float mVelocityMult = 1f;

		public float mWeightMult = 1f;

		public float mSpinMult = 1f;

		public float mMotionRandMult = 1f;

		public float mBounceMult = 1f;

		public float mZoomMult = 1f;

		public float mNumberMult = 1f;

		public float mPct;

		public LifetimeSettings()
		{
		}

		public LifetimeSettings(LifetimeSettings rhs)
			: this()
		{
			if (rhs != null)
			{
				mSizeXMult = rhs.mSizeXMult;
				mVelocityMult = rhs.mVelocityMult;
				mWeightMult = rhs.mWeightMult;
				mSpinMult = rhs.mSpinMult;
				mMotionRandMult = rhs.mMotionRandMult;
				mBounceMult = rhs.mBounceMult;
				mZoomMult = rhs.mZoomMult;
				mNumberMult = rhs.mNumberMult;
				mPct = rhs.mPct;
			}
		}

		public void Reset()
		{
			mSizeXMult = 1f;
			mVelocityMult = 1f;
			mWeightMult = 1f;
			mSpinMult = 1f;
			mMotionRandMult = 1f;
			mBounceMult = 1f;
			mZoomMult = 1f;
			mNumberMult = 1f;
			mPct = 0f;
		}

		public void Serialize(Buffer b)
		{
			b.WriteFloat(mSizeXMult);
			b.WriteFloat(mSizeYMult);
			b.WriteFloat(mVelocityMult);
			b.WriteFloat(mWeightMult);
			b.WriteFloat(mSpinMult);
			b.WriteFloat(mMotionRandMult);
			b.WriteFloat(mBounceMult);
			b.WriteFloat(mZoomMult);
			b.WriteFloat(mNumberMult);
			b.WriteFloat(mPct);
		}

		public void Deserialize(Buffer b)
		{
			mSizeXMult = b.ReadFloat();
			mSizeYMult = b.ReadFloat();
			mVelocityMult = b.ReadFloat();
			mWeightMult = b.ReadFloat();
			mSpinMult = b.ReadFloat();
			mMotionRandMult = b.ReadFloat();
			mBounceMult = b.ReadFloat();
			mZoomMult = b.ReadFloat();
			mNumberMult = b.ReadFloat();
			mPct = b.ReadFloat();
		}
	}
}
