using System;

namespace SexyFramework.PIL
{
	public class EmitterSettings : KeyFrameData
	{
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

		public float mVisibility
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

		public float mEmissionAngle
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

		public float mEmissionRange
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

		public float mTintStrength
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

		public float mXRadius
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

		public float mYRadius
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

		public override void Init()
		{
		}

		public override KeyFrameData Clone()
		{
			return new EmitterSettings(this);
		}

		protected void Reset()
		{
			mNumFloats = 7;
			mNumBools = 1;
			mFloatData = new float[mNumFloats];
			mBoolData = new bool[mNumBools];
			mVisibility = 1f;
			mEmissionAngle = 0f;
			mEmissionRange = (float)Math.PI * 2f;
			mTintStrength = 0f;
			mActive = true;
			mAngle = 0f;
			mXRadius = 0f;
			mYRadius = 0f;
		}

		public EmitterSettings()
		{
			Reset();
		}

		public EmitterSettings(EmitterSettings emitterSettings)
		{
			Reset();
			CopyFrom(emitterSettings);
		}

		public new static KeyFrameData Instantiate()
		{
			return new EmitterSettings();
		}
	}
}
