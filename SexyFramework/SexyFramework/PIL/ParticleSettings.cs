namespace SexyFramework.PIL
{
	public class ParticleSettings : KeyFrameData
	{
		public float mWeight
		{
			get
			{
				return mFloatData[0];
			}
			set
			{
				mFloatData[0] = value;
			}
		}

		public float mSpin
		{
			get
			{
				return mFloatData[1];
			}
			set
			{
				mFloatData[1] = value;
			}
		}

		public float mMotionRand
		{
			get
			{
				return mFloatData[2];
			}
			set
			{
				mFloatData[2] = value;
			}
		}

		public float mGlobalVisibility
		{
			get
			{
				return mFloatData[3];
			}
			set
			{
				mFloatData[3] = value;
			}
		}

		public int mLife
		{
			get
			{
				return mIntData[0];
			}
			set
			{
				mIntData[0] = value;
			}
		}

		public int mXSize
		{
			get
			{
				return mIntData[1];
			}
			set
			{
				mIntData[1] = value;
			}
		}

		public int mYSize
		{
			get
			{
				return mIntData[2];
			}
			set
			{
				mIntData[2] = value;
			}
		}

		public int mVelocity
		{
			get
			{
				return mIntData[3];
			}
			set
			{
				mIntData[3] = value;
			}
		}

		public int mBounce
		{
			get
			{
				return mIntData[4];
			}
			set
			{
				mIntData[4] = value;
			}
		}

		public int mNumber
		{
			get
			{
				return mIntData[5];
			}
			set
			{
				mIntData[5] = value;
			}
		}

		public override void Init()
		{
		}

		public override KeyFrameData Clone()
		{
			return new ParticleSettings(this);
		}

		protected void Reset()
		{
			mNumInts = 6;
			mNumFloats = 4;
			mIntData = new int[mNumInts];
			mFloatData = new float[mNumFloats];
			mLife = 0;
			mXSize = 0;
			mYSize = 0;
			mVelocity = 0;
			mBounce = 0;
			mNumber = 0;
			mWeight = 0f;
			mSpin = 0f;
			mMotionRand = 0f;
			mGlobalVisibility = 1f;
		}

		public ParticleSettings()
		{
			Reset();
		}

		public ParticleSettings(ParticleSettings rhs)
		{
			Reset();
			CopyFrom(rhs);
		}

		public new static KeyFrameData Instantiate()
		{
			return new ParticleSettings();
		}
	}
}
