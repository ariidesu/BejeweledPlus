namespace SexyFramework.PIL
{
	public class ForceSettings : KeyFrameData
	{
		public float mWidth
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

		public float mHeight
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

		public float mStrength
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

		public float mDirection
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

		public float mAngle
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

		public override void Init()
		{
		}

		public override KeyFrameData Clone()
		{
			return new ForceSettings(this);
		}

		protected void Reset()
		{
			mNumFloats = 5;
			mFloatData = new float[mNumFloats];
			mWidth = 0f;
			mHeight = 0f;
			mStrength = 0f;
			mDirection = 0f;
			mAngle = 0f;
		}

		public ForceSettings()
		{
			Reset();
		}

		public ForceSettings(ForceSettings rhs)
		{
			Reset();
			CopyFrom(rhs);
		}

		public new static KeyFrameData Instantiate()
		{
			return new ForceSettings();
		}
	}
}
