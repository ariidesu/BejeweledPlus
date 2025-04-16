using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class PIValue : IDisposable
	{
		public List<float> mQuantTable = new List<float>();

		public List<PIValuePoint> mValuePointVector = new List<PIValuePoint>();

		public Bezier mBezier = new Bezier();

		public float mLastTime;

		public float mLastValue;

		public float mLastCurveT;

		public float mLastCurveTDelta;

		public PIValue()
		{
			mLastTime = -1f;
			mLastCurveT = 0f;
			mLastCurveTDelta = 0.01f;
		}

		public virtual void Dispose()
		{
			mBezier.Dispose();
			mValuePointVector.Clear();
		}

		public void QuantizeCurve()
		{
			float mTime = mValuePointVector[0].mTime;
			float mTime2 = mValuePointVector[mValuePointVector.Count - 1].mTime;
			mQuantTable.Clear();
			Common.Resize(mQuantTable, GlobalPIEffect.PI_QUANT_SIZE);
			bool flag = true;
			int num = 0;
			float num2 = 0f;
			float num3 = mTime;
			float num4 = (mTime2 - mTime) / (float)GlobalPIEffect.PI_QUANT_SIZE / 2f;
			int num5 = 0;
			while (true)
			{
				Vector2 vector = mBezier.Evaluate(num3);
				int num6 = (int)GlobalPIEffect.TIME_TO_X(vector.X, mTime, mTime2);
				bool flag2 = false;
				while (vector.X >= mValuePointVector[num5 + 1].mTime)
				{
					num5++;
					if (num5 >= mValuePointVector.Count - 1)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					break;
				}
				if (vector.X >= mValuePointVector[num5].mTime)
				{
					if (!flag && num6 > num + 1)
					{
						for (int i = num; i <= num6; i++)
						{
							float num7 = (float)(i - num) / (float)(num6 - num);
							float value = num7 * vector.Y + (1f - num7) * num2;
							mQuantTable[i] = value;
						}
					}
					else
					{
						float y = vector.Y;
						mQuantTable[num6] = y;
					}
					num = num6;
					num2 = vector.Y;
				}
				flag = false;
				num3 += num4;
			}
			for (int j = 0; j < mValuePointVector.Count; j++)
			{
				mQuantTable[(int)GlobalPIEffect.TIME_TO_X(mValuePointVector[j].mTime, mTime, mTime2)] = mValuePointVector[j].mValue;
			}
		}

		public float GetValueAt(float theTime)
		{
			return GetValueAt(theTime, 0f);
		}

		public float GetValueAt(float theTime, float theDefault)
		{
			if (mLastTime == theTime)
			{
				return mLastValue;
			}
			float num = mLastTime;
			mLastTime = theTime;
			if (mValuePointVector.Count == 1)
			{
				return mLastValue = mValuePointVector[0].mValue;
			}
			if (mBezier.IsInitialized())
			{
				float mTime = mValuePointVector[0].mTime;
				float mTime2 = mValuePointVector[mValuePointVector.Count - 1].mTime;
				if (mTime2 <= 1.001f)
				{
					if (mQuantTable.Count == 0)
					{
						QuantizeCurve();
					}
					float num2 = GlobalPIEffect.TIME_TO_X(theTime, mTime, mTime2);
					if (num2 <= 0f)
					{
						return mLastValue = mValuePointVector[0].mValue;
					}
					if (num2 >= (float)(GlobalPIEffect.PI_QUANT_SIZE - 1))
					{
						return mLastValue = mValuePointVector[mValuePointVector.Count - 1].mValue;
					}
					int num3 = (int)num2;
					float num4 = num2 - (float)num3;
					mLastValue = mQuantTable[num3] * (1f - num4) + mQuantTable[num3 + 1] * num4;
					return mLastValue;
				}
				float num5 = Math.Min(0.1f, (mTime2 - mTime) / 1000f);
				if (theTime <= mTime)
				{
					return mLastValue = mValuePointVector[0].mValue;
				}
				if (theTime >= mTime2)
				{
					return mLastValue = mValuePointVector[mValuePointVector.Count - 1].mValue;
				}
				float num6 = mTime;
				float num7 = mTime2;
				Vector2 vector = default(Vector2);
				float num8 = 0f;
				bool flag = (theTime - num) / (mTime2 - mTime) > 0.05f;
				float[] array = new float[4] { 0.1f, 0.1f, 0.1f, 0.5f };
				float[] array2 = new float[3] { 1f, 0.75f, 1.25f };
				for (int i = 0; i < 1000; i++)
				{
					float num9 = num5;
					if (i < 4 && !flag)
					{
						num9 *= array[i];
					}
					num8 = ((i >= 3 || mLastCurveTDelta == 0f || flag) ? (num6 + (num7 - num6) / 2f) : (mLastCurveT + mLastCurveTDelta * array2[i]));
					if (num8 >= num6 && num8 <= num7)
					{
						vector = mBezier.Evaluate(num8);
						float num10 = vector.X - theTime;
						if (Math.Abs(num10) <= num9)
						{
							break;
						}
						if (num10 < 0f)
						{
							num6 = num8;
						}
						else
						{
							num7 = num8;
						}
					}
				}
				mLastCurveTDelta = mLastCurveTDelta * 0.5f + (num8 - mLastCurveT) * 0.5f;
				mLastCurveT = num8;
				return mLastValue = vector.Y;
			}
			for (int j = 1; j < mValuePointVector.Count; j++)
			{
				PIValuePoint pIValuePoint = mValuePointVector[j - 1];
				PIValuePoint pIValuePoint2 = mValuePointVector[j];
				if (theTime > pIValuePoint.mTime && theTime < pIValuePoint2.mTime)
				{
					return mLastValue = pIValuePoint.mValue + (pIValuePoint2.mValue - pIValuePoint.mValue) * (theTime - pIValuePoint.mTime) / (pIValuePoint2.mTime - pIValuePoint.mTime);
				}
				if (j == mValuePointVector.Count - 1)
				{
					if (theTime >= pIValuePoint2.mTime)
					{
						mLastValue = pIValuePoint2.mValue;
					}
					else
					{
						mLastValue = pIValuePoint.mValue;
					}
					return mLastValue;
				}
			}
			return mLastValue = theDefault;
		}

		public float GetLastKeyframe(float theTime)
		{
			for (int num = mValuePointVector.Count - 1; num >= 0; num--)
			{
				PIValuePoint pIValuePoint = mValuePointVector[num];
				if (theTime >= pIValuePoint.mTime)
				{
					return pIValuePoint.mValue;
				}
			}
			return 0f;
		}

		public float GetLastKeyframeTime(float theTime)
		{
			for (int num = mValuePointVector.Count - 1; num >= 0; num--)
			{
				PIValuePoint pIValuePoint = mValuePointVector[num];
				if (theTime >= pIValuePoint.mTime)
				{
					return pIValuePoint.mTime;
				}
			}
			return 0f;
		}

		public float GetNextKeyframeTime(float theTime)
		{
			for (int i = 0; i < mValuePointVector.Count; i++)
			{
				PIValuePoint pIValuePoint = mValuePointVector[i];
				if (pIValuePoint.mTime >= theTime)
				{
					return pIValuePoint.mTime;
				}
			}
			return 0f;
		}

		public int GetNextKeyframeIdx(float theTime)
		{
			for (int i = 0; i < mValuePointVector.Count; i++)
			{
				PIValuePoint pIValuePoint = mValuePointVector[i];
				if (pIValuePoint.mTime >= theTime)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
