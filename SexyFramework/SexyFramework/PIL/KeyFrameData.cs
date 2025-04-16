using System;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class KeyFrameData
	{
		public int[] mIntData;

		public float[] mFloatData;

		public bool[] mBoolData;

		public int mNumInts;

		public int mNumFloats;

		public int mNumBools;

		public void CopyFrom(KeyFrameData k)
		{
			if (mNumInts != k.mNumInts)
			{
				mNumInts = k.mNumInts;
				if (mNumInts > 0)
				{
					mIntData = new int[mNumInts];
				}
				else
				{
					mIntData = null;
				}
			}
			if (mNumFloats != k.mNumFloats)
			{
				mNumFloats = k.mNumFloats;
				if (mNumFloats > 0)
				{
					mFloatData = new float[mNumFloats];
				}
				else
				{
					mFloatData = null;
				}
			}
			if (mNumBools != k.mNumBools)
			{
				mNumBools = k.mNumBools;
				if (mNumBools > 0)
				{
					mBoolData = new bool[mNumBools];
				}
				else
				{
					mBoolData = null;
				}
			}
			if (mNumInts > 0)
			{
				Array.Copy(k.mIntData, mIntData, mNumInts);
			}
			if (mNumFloats > 0)
			{
				Array.Copy(k.mFloatData, mFloatData, mNumFloats);
			}
			if (mNumBools > 0)
			{
				Array.Copy(k.mBoolData, mBoolData, mNumBools);
			}
		}

		public virtual void Init()
		{
		}

		public virtual KeyFrameData Clone()
		{
			return new KeyFrameData(this);
		}

		public KeyFrameData()
		{
		}

		public KeyFrameData(KeyFrameData k)
		{
			CopyFrom(k);
		}

		public virtual void Dispose()
		{
			mIntData = null;
			mFloatData = null;
			mBoolData = null;
		}

		public virtual void Serialize(SexyFramework.Misc.Buffer b)
		{
			b.WriteLong(mNumInts);
			b.WriteLong(mNumFloats);
			b.WriteLong(mNumBools);
			for (int i = 0; i < mNumInts; i++)
			{
				b.WriteLong(mIntData[i]);
			}
			for (int j = 0; j < mNumFloats; j++)
			{
				b.WriteFloat(mFloatData[j]);
			}
			for (int k = 0; k < mNumBools; k++)
			{
				b.WriteBoolean(mBoolData[k]);
			}
		}

		public virtual void Deserialize(SexyFramework.Misc.Buffer b)
		{
			mNumInts = (int)b.ReadLong();
			mNumFloats = (int)b.ReadLong();
			mNumBools = (int)b.ReadLong();
			for (int i = 0; i < mNumInts; i++)
			{
				mIntData[i] = (int)b.ReadLong();
			}
			for (int j = 0; j < mNumFloats; j++)
			{
				mFloatData[j] = b.ReadFloat();
			}
			for (int k = 0; k < mNumBools; k++)
			{
				mBoolData[k] = b.ReadBoolean();
			}
		}

		public static KeyFrameData Instantiate()
		{
			return null;
		}
	}
}
