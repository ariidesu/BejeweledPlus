namespace SexyFramework.PIL
{
	public class DeflectorSettings : KeyFrameData
	{
		public int mThickness
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

		public bool mActive
		{
			get
			{
				return mBoolData[0];
			}
			set
			{
				mBoolData[0] = value;
			}
		}

		public float mAngle
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

		public float mBounceMult
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

		public float mHitChance
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

		public float mCollisionMult
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

		public override void Init()
		{
		}

		public override KeyFrameData Clone()
		{
			return new DeflectorSettings(this);
		}

		protected void Reset()
		{
			mNumInts = 1;
			mNumBools = 1;
			mNumFloats = 4;
			mIntData = new int[mNumInts];
			mFloatData = new float[mNumFloats];
			mBoolData = new bool[mNumBools];
			mThickness = 2;
			mAngle = 0f;
			mBounceMult = 1f;
			mHitChance = 1f;
			mActive = true;
			mCollisionMult = 1f;
		}

		public DeflectorSettings()
		{
			Reset();
		}

		public DeflectorSettings(DeflectorSettings rhs)
		{
			Reset();
			CopyFrom(rhs);
		}

		public new static KeyFrameData Instantiate()
		{
			return new DeflectorSettings();
		}
	}
}
