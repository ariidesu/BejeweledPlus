namespace SexyFramework.PIL
{
	public class EmitterScale : KeyFrameData
	{
		public float mLifeScale
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

		public float mNumberScale
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

		public float mSizeXScale
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

		public float mSizeYScale
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

		public float mVelocityScale
		{
			get
			{
				return mFloatData[4];
			}
			set
			{
				mFloatData[4] = value;
			}
		}

		public float mWeightScale
		{
			get
			{
				return mFloatData[5];
			}
			set
			{
				mFloatData[5] = value;
			}
		}

		public float mSpinScale
		{
			get
			{
				return mFloatData[6];
			}
			set
			{
				mFloatData[6] = value;
			}
		}

		public float mMotionRandScale
		{
			get
			{
				return mFloatData[7];
			}
			set
			{
				mFloatData[7] = value;
			}
		}

		public float mZoom
		{
			get
			{
				return mFloatData[8];
			}
			set
			{
				mFloatData[8] = value;
			}
		}

		public float mBounceScale
		{
			get
			{
				return mFloatData[9];
			}
			set
			{
				mFloatData[9] = value;
			}
		}

		public override void Init()
		{
		}

		public override KeyFrameData Clone()
		{
			return new EmitterScale(this);
		}

		protected void Reset()
		{
			mNumFloats = 10;
			mFloatData = new float[mNumFloats];
			mLifeScale = 1f;
			mNumberScale = 1f;
			mSizeXScale = 1f;
			mSizeYScale = 1f;
			mVelocityScale = 1f;
			mWeightScale = 1f;
			mSpinScale = 1f;
			mMotionRandScale = 1f;
			mZoom = 1f;
			mBounceScale = 1f;
		}

		public EmitterScale()
		{
			Reset();
		}

		public EmitterScale(EmitterScale rhs)
		{
			Reset();
			CopyFrom(rhs);
		}

		public new static KeyFrameData Instantiate()
		{
			return new EmitterScale();
		}
	}
}
