namespace SexyFramework.PIL
{
	public class FreeEmitterVariance : KeyFrameData
	{
		public int mLifeVar
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

		public int mSizeXVar
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

		public int mSizeYVar
		{
			get
			{
				return mIntData[7];
			}
			set
			{
				mIntData[7] = value;
			}
		}

		public int mVelocityVar
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

		public int mWeightVar
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

		public float mSpinVar
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

		public int mMotionRandVar
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

		public int mBounceVar
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

		public int mZoomVar
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

		public override void Init()
		{
		}

		public override KeyFrameData Clone()
		{
			return new FreeEmitterVariance(this);
		}

		protected void Reset()
		{
			mNumInts = 8;
			mIntData = new int[mNumInts];
			mNumFloats = 1;
			mFloatData = new float[mNumFloats];
			mLifeVar = 0;
			mSizeXVar = 0;
			mSizeYVar = 0;
			mVelocityVar = 0;
			mWeightVar = 0;
			mSpinVar = 0f;
			mMotionRandVar = 0;
			mBounceVar = 0;
			mZoomVar = 0;
		}

		public FreeEmitterVariance()
		{
			Reset();
		}

		public FreeEmitterVariance(FreeEmitterVariance rhs)
		{
			Reset();
			CopyFrom(rhs);
		}

		public new static KeyFrameData Instantiate()
		{
			return new FreeEmitterVariance();
		}
	}
}
