using System.Collections.Generic;
using SexyFramework;

namespace BejeweledLivePlus.Widget
{
	public class SpreadCurve
	{
		public List<float> mVals;

		public int mSize;

		public SpreadCurve(int theSize)
		{
			mVals = new List<float>(theSize);
			mSize = theSize;
			for (int i = 0; i < mSize; i++)
			{
				mVals.Add((float)i / (float)theSize);
			}
		}

		public SpreadCurve()
		{
			int num = 256;
			mVals = new List<float>(num);
			mSize = num;
			for (int i = 0; i < mSize; i++)
			{
				mVals.Add((float)i / (float)num);
			}
		}

		public SpreadCurve(CurvedVal theCurve, int theSize)
		{
			mVals = new List<float>(theSize);
			mSize = theSize;
			for (int i = 0; i < mSize; i++)
			{
				mVals.Add((float)i / (float)theSize);
			}
			SetToCurve(theCurve);
		}

		public SpreadCurve(CurvedVal theCurve)
		{
			int num = 256;
			mVals = new List<float>(num);
			mSize = num;
			for (int i = 0; i < mSize; i++)
			{
				mVals.Add((float)i / (float)num);
			}
			SetToCurve(theCurve);
		}

		public void SetToCurve(CurvedVal theCurve)
		{
			List<double> list = new List<double>(mSize);
			for (int i = 0; i < mSize; i++)
			{
				list.Add(0.0);
			}
			GlobalMembers.DBG_ASSERT(theCurve.mOutMax <= 1.0);
			GlobalMembers.DBG_ASSERT(theCurve.mInMax >= 0.0);
			double num = 0.0;
			for (int j = 0; j < mSize; j++)
			{
				double outVal = theCurve.GetOutVal((float)j / (float)mSize);
				list[j] += outVal;
				num += outVal;
			}
			int k = 0;
			double num2 = 0.0;
			double num3 = mSize;
			for (int l = 0; l < mSize; l++)
			{
				mVals[l] = 1f;
			}
			for (int m = 0; m < mSize; m++)
			{
				num2 += list[m] / num * (num3 - 1.0);
				double num4 = (double)m / (num3 - 1.0);
				for (; (double)k <= num2; k++)
				{
					if ((double)k < num3)
					{
						GlobalMembers.DBG_ASSERT(num4 <= 1.0 && num4 >= 0.0);
						mVals[k] = (float)num4;
					}
				}
			}
		}

		public float GetOutVal(float theVal)
		{
			int index = GlobalMembers.MAX(0, (int)GlobalMembers.MIN(mSize - 1, theVal * (float)(mSize - 1)));
			return mVals[index];
		}
	}
}
