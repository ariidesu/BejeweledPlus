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
			float aMinTime = mValuePointVector[0].mTime;
			float aMaxTime = mValuePointVector[mValuePointVector.Count - 1].mTime;
			mQuantTable.Clear();
			Common.Resize(mQuantTable, GlobalPIEffect.PI_QUANT_SIZE);
			bool isFirstSample = true;
			int aLastQuantIdx = 0;
			float aLastValue = 0f;
			float aCurT = aMinTime;
			float aTStep = (aMaxTime - aMinTime) / (float)GlobalPIEffect.PI_QUANT_SIZE / 2f;
			int aCurKeyIdx = 0;
			while (true)
			{
				Vector2 aPt = mBezier.Evaluate(aCurT);
				int aCurQuantIdx = (int)GlobalPIEffect.TIME_TO_X(aPt.X, aMinTime, aMaxTime);
				bool isDone = false;
				while (aPt.X >= mValuePointVector[aCurKeyIdx + 1].mTime)
				{
					aCurKeyIdx++;
					if (aCurKeyIdx >= mValuePointVector.Count - 1)
					{
						isDone = true;
						break;
					}
				}
				if (isDone)
				{
					break;
				}
				if (aPt.X >= mValuePointVector[aCurKeyIdx].mTime)
				{
					if (!isFirstSample && aCurQuantIdx > aLastQuantIdx + 1)
					{
						for (int i = aLastQuantIdx; i <= aCurQuantIdx; i++)
						{
							float aFrac = (float)(i - aLastQuantIdx) / (float)(aCurQuantIdx - aLastQuantIdx);
							float aValue = aFrac * aPt.Y + (1f - aFrac) * aLastValue;
							mQuantTable[i] = aValue;
						}
					}
					else
					{
						mQuantTable[aCurQuantIdx] = aPt.Y;
					}
					aLastQuantIdx = aCurQuantIdx;
					aLastValue = aPt.Y;
				}
				isFirstSample = false;
				aCurT += aTStep;
			}
			for (int aKeyIdx = 0; aKeyIdx < mValuePointVector.Count; aKeyIdx++)
			{
				mQuantTable[(int)GlobalPIEffect.TIME_TO_X(mValuePointVector[aKeyIdx].mTime, aMinTime, aMaxTime)] = mValuePointVector[aKeyIdx].mValue;
			}
		}

		private static readonly float[] sErrorFactor = new float[4] { 0.1f, 0.1f, 0.1f, 0.5f };
		private static readonly float[] sBezierFactors = new float[3] { 1f, 0.75f, 1.25f };

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
			float aPrevTime = mLastTime;
			mLastTime = theTime;
			if (mValuePointVector.Count == 1)
			{
				return mLastValue = mValuePointVector[0].mValue;
			}
			if (mBezier.IsInitialized())
			{
				float aMinTime = mValuePointVector[0].mTime;
				float aMaxTime = mValuePointVector[mValuePointVector.Count - 1].mTime;
				if (aMaxTime <= 1.001f)
				{
					if (mQuantTable.Count == 0)
					{
						QuantizeCurve();
					}
					float aQPos = GlobalPIEffect.TIME_TO_X(theTime, aMinTime, aMaxTime);
					if (aQPos <= 0f)
					{
						return mLastValue = mValuePointVector[0].mValue;
					}
					if (aQPos >= (float)(GlobalPIEffect.PI_QUANT_SIZE - 1))
					{
						return mLastValue = mValuePointVector[mValuePointVector.Count - 1].mValue;
					}
					int aLeft = (int)aQPos;
					float aFrac = aQPos - (float)aLeft;
					mLastValue = mQuantTable[aLeft] * (1f - aFrac) + mQuantTable[aLeft + 1] * aFrac;
					return mLastValue;
				}
				float aMaxError = Math.Min(0.1f, (aMaxTime - aMinTime) / 1000f);
				if (theTime <= aMinTime)
				{
					return mLastValue = mValuePointVector[0].mValue;
				}
				if (theTime >= aMaxTime)
				{
					return mLastValue = mValuePointVector[mValuePointVector.Count - 1].mValue;
				}
				float aL = aMinTime;
				float aR = aMaxTime;
				Vector2 aPt = default(Vector2);
				float aTryT = 0f;
				bool isBigChange = (theTime - aPrevTime) / (aMaxTime - aMinTime) > 0.05f;
				float[] anErrorFactor = sErrorFactor;
				float[] aFactors = sBezierFactors;
				for (int aTryCount = 0; aTryCount < 1000; aTryCount++)
				{
					float aWantError = aMaxError;
					if (aTryCount < 4 && !isBigChange)
					{
						aWantError *= anErrorFactor[aTryCount];
					}
					aTryT = ((aTryCount >= 3 || mLastCurveTDelta == 0f || isBigChange)
						? (aL + (aR - aL) / 2f)
						: (mLastCurveT + mLastCurveTDelta * aFactors[aTryCount]));
					if (aTryT >= aL && aTryT <= aR)
					{
						aPt = mBezier.Evaluate(aTryT);
						float aDiff = aPt.X - theTime;
						if (Math.Abs(aDiff) <= aWantError)
						{
							break;
						}
						if (aDiff < 0f)
						{
							aL = aTryT;
						}
						else
						{
							aR = aTryT;
						}
					}
				}
				mLastCurveTDelta = mLastCurveTDelta * 0.5f + (aTryT - mLastCurveT) * 0.5f;
				mLastCurveT = aTryT;
				return mLastValue = aPt.Y;
			}
			for (int aKeyIdx = 1; aKeyIdx < mValuePointVector.Count; aKeyIdx++)
			{
				PIValuePoint aP1 = mValuePointVector[aKeyIdx - 1];
				PIValuePoint aP2 = mValuePointVector[aKeyIdx];
				if (theTime >= aP1.mTime && theTime <= aP2.mTime)
				{
					float aDenom = aP2.mTime - aP1.mTime;
					float aPct = (aDenom > 0f) ? (theTime - aP1.mTime) / aDenom : 0f;
					if (aPct > 1f) aPct = 1f;
					return mLastValue = aP1.mValue + (aP2.mValue - aP1.mValue) * aPct;
				}
				if (aKeyIdx == mValuePointVector.Count - 1)
				{
					if (theTime >= aP2.mTime)
					{
						mLastValue = aP2.mValue;
					}
					else
					{
						mLastValue = aP1.mValue;
					}
					return mLastValue;
				}
			}
			return mLastValue = theDefault;
		}

		public float GetLastKeyframe(float theTime)
		{
			for (int aKeyIdx = mValuePointVector.Count - 1; aKeyIdx >= 0; aKeyIdx--)
			{
				PIValuePoint aPt = mValuePointVector[aKeyIdx];
				if (theTime >= aPt.mTime)
				{
					return aPt.mValue;
				}
			}
			return 0f;
		}

		public float GetLastKeyframeTime(float theTime)
		{
			for (int aKeyIdx = mValuePointVector.Count - 1; aKeyIdx >= 0; aKeyIdx--)
			{
				PIValuePoint aPt = mValuePointVector[aKeyIdx];
				if (theTime >= aPt.mTime)
				{
					return aPt.mTime;
				}
			}
			return 0f;
		}

		public float GetNextKeyframeTime(float theTime)
		{
			for (int aKeyIdx = 0; aKeyIdx < mValuePointVector.Count; aKeyIdx++)
			{
				PIValuePoint aPt = mValuePointVector[aKeyIdx];
				if (aPt.mTime >= theTime)
				{
					return aPt.mTime;
				}
			}
			return 0f;
		}

		public int GetNextKeyframeIdx(float theTime)
		{
			for (int aKeyIdx = 0; aKeyIdx < mValuePointVector.Count; aKeyIdx++)
			{
				PIValuePoint aPt = mValuePointVector[aKeyIdx];
				if (aPt.mTime >= theTime)
				{
					return aKeyIdx;
				}
			}
			return -1;
		}
	}
}
