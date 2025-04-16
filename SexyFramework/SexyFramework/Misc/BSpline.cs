using System;
using System.Collections.Generic;

namespace SexyFramework.Misc
{
	public class BSpline
	{
		protected List<float> mXPoints = new List<float>();

		protected List<float> mYPoints = new List<float>();

		protected List<float> mArcLengths = new List<float>();

		protected List<float> mXCoef = new List<float>();

		protected List<float> mYCoef = new List<float>();

		protected float GetPoint(float t, ref List<float> theCoef)
		{
			int num = (int)Math.Floor(t);
			if (num < 0)
			{
				num = 0;
				t = 0f;
			}
			else if (num >= mXPoints.Count - 1)
			{
				num = mXPoints.Count - 2;
				t = num + 1;
			}
			float num2 = t - (float)num;
			num *= 4;
			float num3 = theCoef[num];
			float num4 = theCoef[num + 1];
			float num5 = theCoef[num + 2];
			float num6 = theCoef[num + 3];
			float num7 = num2 * num2;
			float num8 = num7 * num2;
			return num3 * num8 + num4 * num7 + num5 * num2 + num6;
		}

		protected void CalculateSplinePrv(ref List<float> thePoints, ref List<float> theCoef)
		{
			if (thePoints.Count < 2)
			{
				return;
			}
			int num = thePoints.Count - 1;
			int theNumVariables = num * 4;
			EquationSystem equationSystem = new EquationSystem(theNumVariables);
			equationSystem.SetCoefficient(2, 1f);
			equationSystem.NextEquation();
			int num2 = 0;
			int num3 = 0;
			while (num3 < num)
			{
				equationSystem.SetCoefficient(num2 + 3, 1f);
				equationSystem.SetConstantTerm(thePoints[num3]);
				equationSystem.NextEquation();
				equationSystem.SetCoefficient(num2, 1f);
				equationSystem.SetCoefficient(num2 + 1, 1f);
				equationSystem.SetCoefficient(num2 + 2, 1f);
				equationSystem.SetConstantTerm(thePoints[num3 + 1] - thePoints[num3]);
				equationSystem.NextEquation();
				equationSystem.SetCoefficient(num2, 3f);
				equationSystem.SetCoefficient(num2 + 1, 2f);
				equationSystem.SetCoefficient(num2 + 2, 1f);
				if (num3 < num - 1)
				{
					equationSystem.SetCoefficient(num2 + 6, -1f);
				}
				equationSystem.NextEquation();
				if (num3 < num - 1)
				{
					equationSystem.SetCoefficient(num2, 6f);
					equationSystem.SetCoefficient(num2 + 1, 2f);
					equationSystem.SetCoefficient(num2 + 5, -2f);
					equationSystem.NextEquation();
				}
				num3++;
				num2 += 4;
			}
			equationSystem.Solve();
			theCoef = equationSystem.sol;
		}

		protected void CalculateSplinePrvLinear(ref List<float> thePoints, ref List<float> theCoef)
		{
			if (thePoints.Count >= 2)
			{
				int num = thePoints.Count - 1;
				int num2 = num * 4;
				theCoef.Clear();
				for (int i = 0; i < num2; i++)
				{
					theCoef.Add(0f);
				}
				for (int j = 0; j < num; j++)
				{
					int num3 = j * 4;
					float num4 = thePoints[j];
					float num5 = thePoints[j + 1];
					theCoef[num3] = 0f;
					theCoef[num3 + 1] = 0f;
					theCoef[num3 + 2] = num5 - num4;
					theCoef[num3 + 3] = num4;
				}
			}
		}

		protected void CalculateSplinePrvSemiLinear(ref List<float> thePoints, ref List<float> theCoef)
		{
			if (thePoints.Count < 2)
			{
				return;
			}
			int num = thePoints.Count - 1;
			List<float> list = new List<float>();
			for (int i = 0; i < num; i++)
			{
				float num2 = mArcLengths[i];
				if (num2 <= 100f)
				{
					num2 = 1f;
				}
				else
				{
					num2 = 100f / num2;
				}
				num2 = 0.3f;
				float num3 = thePoints[i];
				float num4 = thePoints[i + 1];
				if (i > 0)
				{
					list.Add(num2 * num4 + (1f - num2) * num3);
				}
				else
				{
					list.Add(num3);
				}
				if (i < num - 1)
				{
					list.Add(num2 * num3 + (1f - num2) * num4);
				}
				else
				{
					list.Add(num4);
				}
			}
			thePoints = list;
			num = list.Count - 1;
			int num5 = num * 4;
			theCoef.Clear();
			for (int j = 0; j < num5; j++)
			{
				theCoef.Add(0f);
			}
			for (int i = 0; i < num; i++)
			{
				float num6 = list[i];
				float num7 = list[i + 1];
				int num8 = i * 4;
				if ((i & 1) != 0 && i < num - 1)
				{
					float num9 = list[i - 1];
					float num10 = list[i + 2];
					float value = num6;
					float num11 = num6 - num9;
					float num12 = -2f * (num7 - 2f * num6 + num9) - num11 + (num10 - num7);
					float value2 = 0f - num12 + num7 - 2f * num6 + num9;
					theCoef[num8] = num12;
					theCoef[num8 + 1] = value2;
					theCoef[num8 + 2] = num11;
					theCoef[num8 + 3] = value;
				}
				else
				{
					theCoef[num8] = 0f;
					theCoef[num8 + 1] = 0f;
					theCoef[num8 + 2] = num7 - num6;
					theCoef[num8 + 3] = num6;
				}
			}
		}

		protected void CalcArcLengths()
		{
			mArcLengths.Clear();
			int num = mXPoints.Count - 1;
			for (int i = 0; i < num; i++)
			{
				float num2 = mXPoints[i];
				float num3 = mYPoints[i];
				float num4 = mXPoints[i + 1];
				float num5 = mYPoints[i + 1];
				float item = (float)Math.Sqrt((num4 - num2) * (num4 - num2) + (num5 - num3) * (num5 - num3));
				mArcLengths.Add(item);
			}
		}

		public void Reset()
		{
			mXPoints.Clear();
			mYPoints.Clear();
			mArcLengths.Clear();
			mXCoef.Clear();
			mYCoef.Clear();
		}

		public void AddPoint(float x, float y)
		{
			mXPoints.Add(x);
			mYPoints.Add(y);
		}

		public void CalculateSpline()
		{
			CalculateSpline(false);
		}

		public void CalculateSpline(bool linear)
		{
			CalcArcLengths();
			if (linear)
			{
				CalculateSplinePrvLinear(ref mXPoints, ref mXCoef);
				CalculateSplinePrvLinear(ref mYPoints, ref mYCoef);
			}
			else
			{
				CalculateSplinePrv(ref mXPoints, ref mXCoef);
				CalculateSplinePrv(ref mYPoints, ref mYCoef);
			}
			CalcArcLengths();
		}

		public float GetXPoint(float t)
		{
			return GetPoint(t, ref mXCoef);
		}

		public float GetYPoint(float t)
		{
			return GetPoint(t, ref mYCoef);
		}

		public bool GetNextPoint(ref float x, ref float y, ref float t)
		{
			int num = (int)Math.Floor(t);
			if (num < 0 || num >= mXPoints.Count - 1)
			{
				x = GetXPoint(t);
				y = GetYPoint(t);
				return false;
			}
			float num2 = 1f / (mArcLengths[num] * 100f);
			float xPoint = GetXPoint(t);
			float yPoint = GetYPoint(t);
			float num3 = t;
			float xPoint2;
			float yPoint2;
			float num4;
			do
			{
				num3 += num2;
				xPoint2 = GetXPoint(num3);
				yPoint2 = GetYPoint(num3);
				num4 = (xPoint2 - xPoint) * (xPoint2 - xPoint) + (yPoint2 - yPoint) * (yPoint2 - yPoint);
			}
			while (!(num4 >= 1f) && !(num3 > (float)(mXPoints.Count - 1)));
			x = xPoint2;
			y = yPoint2;
			t = num3;
			return true;
		}

		public List<float> GetXPoints()
		{
			return mXPoints;
		}

		public List<float> GetYPoints()
		{
			return mYPoints;
		}

		public int GetMaxT()
		{
			return mXPoints.Count - 1;
		}
	}
}
