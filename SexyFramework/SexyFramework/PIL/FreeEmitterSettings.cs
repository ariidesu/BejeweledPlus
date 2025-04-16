namespace SexyFramework.PIL
{
	public class FreeEmitterSettings : KeyFrameData
	{
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

		public int mNumber
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

		public int mVelocity
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

		public int mWeight
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

		public int mMotionRand
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

		public int mBounce
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

		public int mZoom
		{
			get
			{
				return mIntData[6];
			}
			set
			{
				mIntData[6] = value;
			}
		}

		public float mSpin
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

		public override void Init()
		{
		}

		public override KeyFrameData Clone()
		{
			return new FreeEmitterSettings(this);
		}

		protected void Reset()
		{
			mNumInts = 7;
			mIntData = new int[mNumInts];
			mNumFloats = 1;
			mFloatData = new float[mNumFloats];
			mLife = 0;
			mNumber = 0;
			mVelocity = 0;
			mWeight = 0;
			mSpin = 0f;
			mMotionRand = 0;
			mBounce = 0;
			mZoom = 100;
		}

		public FreeEmitterSettings()
		{
			Reset();
		}

		public FreeEmitterSettings(FreeEmitterSettings rhs)
		{
			Reset();
			CopyFrom(rhs);
		}

		public new static KeyFrameData Instantiate()
		{
			return new FreeEmitterSettings();
		}
	}
}
