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
			PIInterpolatorPoint pIInterpolatorPoint = mInterpolatorPointVector[0];
			PIInterpolatorPoint pIInterpolatorPoint2 = mInterpolatorPointVector[mInterpolatorPointVector.Count - 1];
			float num = pIInterpolatorPoint.mTime + theTime * (pIInterpolatorPoint2.mTime - pIInterpolatorPoint.mTime);
			for (int i = 1; i < mInterpolatorPointVector.Count; i++)
			{
				PIInterpolatorPoint pIInterpolatorPoint3 = mInterpolatorPointVector[i - 1];
				PIInterpolatorPoint pIInterpolatorPoint4 = mInterpolatorPointVector[i];
				if (num > pIInterpolatorPoint3.mTime && num < pIInterpolatorPoint4.mTime)
				{
					return (int)GlobalPIEffect.InterpColor(pIInterpolatorPoint3.mValue, pIInterpolatorPoint4.mValue, (num - pIInterpolatorPoint3.mTime) / (pIInterpolatorPoint4.mTime - pIInterpolatorPoint3.mTime));
				}
				if (i == mInterpolatorPointVector.Count - 1)
				{
					if (num >= pIInterpolatorPoint4.mTime)
					{
						return pIInterpolatorPoint4.mValue;
					}
					return pIInterpolatorPoint3.mValue;
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
