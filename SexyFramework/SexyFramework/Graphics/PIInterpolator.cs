using System;
using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class PIInterpolator
	{
		public List<PIInterpolatorPoint> mInterpolatorPointVector = new List<PIInterpolatorPoint>();

		public int GetValueAt(float theTime)
		{
			if (mInterpolatorPointVector.Count == 1)
			{
				return mInterpolatorPointVector[0].mValue;
			}
			float aScaledTime = mInterpolatorPointVector[0].mTime + theTime * (mInterpolatorPointVector[mInterpolatorPointVector.Count - 1].mTime - mInterpolatorPointVector[0].mTime);
			for (int i = 1; i < mInterpolatorPointVector.Count; i++)
			{
				PIInterpolatorPoint aP1 = mInterpolatorPointVector[i - 1];
				PIInterpolatorPoint aP2 = mInterpolatorPointVector[i];
				if (aScaledTime >= aP1.mTime && aScaledTime <= aP2.mTime)
				{
					float aDenom = aP2.mTime - aP1.mTime;
					float aPct = (aDenom > 0f) ? (aScaledTime - aP1.mTime) / aDenom : 0f;
					if (aPct > 1f) aPct = 1f;
					return (int)GlobalPIEffect.InterpColor(aP1.mValue, aP2.mValue, aPct);
				}
				if (i == mInterpolatorPointVector.Count - 1)
				{
					return aP2.mValue;
				}
			}
			return 0;
		}

		public int GetKeyframeNum(int theIdx)
		{
			if (mInterpolatorPointVector.Count == 0)
			{
				return 0;
			}
			return mInterpolatorPointVector[theIdx % mInterpolatorPointVector.Count].mValue;
		}

		public float GetKeyframeTime(int theIdx)
		{
			if (mInterpolatorPointVector.Count == 0)
			{
				return 0f;
			}
			return mInterpolatorPointVector[theIdx % mInterpolatorPointVector.Count].mTime;
		}
	}
}
