using System;
using System.Collections.Generic;
using System.Globalization;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework
{
	public class CurvedVal
	{
		public enum Mode
		{
			MODE_CLAMP,
			MODE_REPEAT,
			MODE_PING_PONG
		}

		public enum Ramp
		{
			RAMP_NONE,
			RAMP_LINEAR,
			RAMP_SLOW_TO_FAST,
			RAMP_FAST_TO_SLOW,
			RAMP_SLOW_FAST_SLOW,
			RAMP_FAST_SLOW_FAST,
			RAMP_CURVEDATA
		}

		public class DataPoint
		{
			public float mX;

			public float mY;

			public float mAngleDeg;
		}

		public class CurveCacheRecord
		{
			public float[] mTable = new float[CV_NUM_SPLINE_POINTS];

			public SexyMathHermite mHermiteCurve = new SexyMathHermite();

			public string mDataStr;
		}

		public static int DFLAG_NOCLIP = 1;

		public static int DFLAG_SINGLETRIGGER = 2;

		public static int DFLAG_OUTPUTSYNC = 4;

		public static int DFLAG_HERMITE = 8;

		public static int DFLAG_AUTOINC = 16;

		public static int CV_NUM_SPLINE_POINTS = 256;

		public static double PI = 3.141590118408203;

		public static Dictionary<string, CurveCacheRecord> mCurveCacheMap = new Dictionary<string, CurveCacheRecord>();

		public static CurvedVal gsFastCurveData = new CurvedVal();

		public double mIncRate;

		public double mOutMin;

		public double mOutMax;

		public string mDataP;

		public string mCurDataPStr;

		public int mInitAppUpdateCount;

		public Ref<int> mAppUpdateCountSrc;

		public CurvedVal mLinkedVal;

		public CurveCacheRecord mCurveCacheRecord;

		public double mCurOutVal;

		public double mPrevOutVal;

		public double mInMin;

		public double mInMax;

		public byte mMode;

		public byte mRamp;

		public bool mNoClip;

		public bool mSingleTrigger;

		public bool mOutputSync;

		public bool mTriggered;

		public bool mIsHermite;

		public bool mAutoInc;

		public double mPrevInVal;

		public double mInVal;

		private static int SIGN(int aVal)
		{
			if (aVal >= 0)
			{
				if (aVal <= 0)
				{
					return 0;
				}
				return 1;
			}
			return -1;
		}

		private static float SIGN(float aVal)
		{
			if (!(aVal < 0f))
			{
				if (!(aVal > 0f))
				{
					return 0f;
				}
				return 1f;
			}
			return -1f;
		}

		private static float CVCharToFloat(sbyte theChar)
		{
			if (theChar >= 92)
			{
				theChar--;
			}
			return (float)(theChar - 35) / 90f;
		}

		private static int CVCharToInt(sbyte theChar)
		{
			if (theChar >= 92)
			{
				theChar--;
			}
			return theChar - 35;
		}

		private static float CVStrToAngle(string theStr)
		{
			int num = 0;
			num += CVCharToInt((sbyte)theStr[0]);
			num *= 90;
			num += CVCharToInt((sbyte)theStr[1]);
			num *= 90;
			num += CVCharToInt((sbyte)theStr[2]);
			return (float)num * 360f / 729000f;
		}

		public static implicit operator double(CurvedVal ImpliedObject)
		{
			return ImpliedObject.GetOutVal();
		}

		public static implicit operator Color(CurvedVal ImpliedObject)
		{
			return new Color(255, 255, 255, (int)(255.0 * ImpliedObject.GetOutVal()));
		}

		public void CopyFrom(CurvedVal rhs)
		{
			if (this != rhs)
			{
				mMode = rhs.mMode;
				mRamp = rhs.mRamp;
				mSingleTrigger = rhs.mSingleTrigger;
				mNoClip = rhs.mNoClip;
				mOutputSync = rhs.mOutputSync;
				mTriggered = rhs.mTriggered;
				mIsHermite = rhs.mIsHermite;
				mAutoInc = rhs.mAutoInc;
				mInitAppUpdateCount = rhs.mInitAppUpdateCount;
				if (GlobalMembers.gSexyAppBase != null)
				{
					mAppUpdateCountSrc = GlobalMembers.gSexyAppBase.mUpdateCount;
				}
				else
				{
					mAppUpdateCountSrc = null;
				}
				mOutMin = rhs.mOutMin;
				mOutMax = rhs.mOutMax;
				mInMin = rhs.mInMin;
				mInMax = rhs.mInMax;
				mCurOutVal = rhs.mCurOutVal;
				mInVal = rhs.mInVal;
				mPrevInVal = rhs.mPrevInVal;
				mIncRate = rhs.mIncRate;
				mPrevOutVal = rhs.mPrevOutVal;
				mCurveCacheRecord = rhs.mCurveCacheRecord;
				mLinkedVal = rhs.mLinkedVal;
				mDataP = rhs.mDataP;
				mCurDataPStr = rhs.mCurDataPStr;
			}
		}

		protected void InitVarDefaults()
		{
			mMode = 0;
			mRamp = 0;
			mCurveCacheRecord = null;
			mSingleTrigger = false;
			mNoClip = false;
			mOutputSync = false;
			mTriggered = false;
			mIsHermite = false;
			mAutoInc = false;
			mInitAppUpdateCount = 0;
			mOutMin = 0.0;
			mOutMax = 1.0;
			mInMin = 0.0;
			mInMax = 1.0;
			mLinkedVal = null;
			mCurOutVal = 0.0;
			mInVal = 0.0;
			mPrevInVal = 0.0;
			mIncRate = 0.0;
			mPrevOutVal = 0.0;
			mDataP = "";
			mCurDataPStr = null;
			if (mCurveCacheMap != null)
			{
				mCurveCacheMap.Clear();
			}
		}

		protected bool CheckCurveChange()
		{
			if (mDataP != null && mDataP != mCurDataPStr)
			{
				mCurDataPStr = mDataP;
				ParseDataString(mCurDataPStr);
				return true;
			}
			return false;
		}

		protected bool CheckClamping()
		{
			CheckCurveChange();
			if (mMode == 0)
			{
				if (mInVal < mInMin)
				{
					mInVal = mInMin;
					return false;
				}
				if (mInVal > mInMax)
				{
					mInVal = mInMax;
					return false;
				}
			}
			else if (mMode == 1 || mMode == 2)
			{
				double num = mInMax - mInMin;
				if (mInVal > mInMax || mInVal < mInMin)
				{
					mInVal = mInMin + (mInVal - mInMin + num) % num;
				}
			}
			return true;
		}

		protected void GenerateTable(List<DataPoint> theDataPointVector, float[] theBuffer, int theSize)
		{
			BSpline bSpline = new BSpline();
			for (int i = 0; i < theDataPointVector.Count; i++)
			{
				DataPoint dataPoint = theDataPointVector[i];
				bSpline.AddPoint(dataPoint.mX, dataPoint.mY);
			}
			bSpline.CalculateSpline();
			bool flag = true;
			int num = 0;
			float num2 = 0f;
			for (int j = 1; j < theDataPointVector.Count; j++)
			{
				DataPoint dataPoint2 = theDataPointVector[j - 1];
				DataPoint dataPoint3 = theDataPointVector[j];
				int num3 = (int)((double)(dataPoint2.mX * (float)(theSize - 1)) + 0.5);
				int num4 = (int)((double)(dataPoint3.mX * (float)(theSize - 1)) + 0.5);
				for (int k = num3; k <= num4; k++)
				{
					float t = (float)(j - 1) + (float)(k - num3) / (float)(num4 - num3);
					float yPoint = bSpline.GetYPoint(t);
					float xPoint = bSpline.GetXPoint(t);
					int num5 = (int)((double)(xPoint * (float)(theSize - 1)) + 0.5);
					if (num5 < num || num5 > num4)
					{
						continue;
					}
					if (!flag)
					{
						if (num5 > num + 1)
						{
							for (int l = num; l <= num5; l++)
							{
								float num6 = (float)(l - num) / (float)(num5 - num);
								float num7 = num6 * yPoint + (1f - num6) * num2;
								if (!mNoClip)
								{
									num7 = Math.Min(Math.Max(num7, 0f), 1f);
								}
								theBuffer[l] = num7;
							}
						}
						else
						{
							float num8 = yPoint;
							if (!mNoClip)
							{
								num8 = Math.Min(Math.Max(num8, 0f), 1f);
							}
							theBuffer[num5] = num8;
						}
					}
					num = num5;
					num2 = yPoint;
					flag = false;
				}
			}
			for (int m = 0; m < theDataPointVector.Count; m++)
			{
				DataPoint dataPoint4 = theDataPointVector[m];
				int num9 = (int)((double)(dataPoint4.mX * (float)(theSize - 1)) + 0.5);
				theBuffer[num9] = dataPoint4.mY;
			}
		}

		protected void ParseDataString(string theString)
		{
			if (theString.Length <= 0)
			{
				return;
			}
			mIncRate = 0.0;
			mOutMin = 0.0;
			mOutMax = 1.0;
			mSingleTrigger = false;
			mNoClip = false;
			mOutputSync = false;
			mIsHermite = false;
			mAutoInc = false;
			int num = 0;
			int num2 = 0;
			if (theString[0] >= 'a' && theString[0] <= 'b')
			{
				num2 = theString[0] - 97;
			}
			num++;
			if (num2 >= 1)
			{
				int num3 = CVCharToInt((sbyte)theString[num++]);
				mNoClip = (num3 & DFLAG_NOCLIP) != 0;
				mSingleTrigger = (num3 & DFLAG_SINGLETRIGGER) != 0;
				mOutputSync = (num3 & DFLAG_OUTPUTSYNC) != 0;
				mIsHermite = (num3 & DFLAG_HERMITE) != 0;
				mAutoInc = (num3 & DFLAG_AUTOINC) != 0;
			}
			int num4 = theString.IndexOf(',', num);
			if (num4 == -1)
			{
				mIsHermite = true;
				return;
			}
			double result = 0.0;
			double.TryParse(theString.Substring(num, num4 - num), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
			mOutMin = (float)result;
			num = num4 + 1;
			num4 = theString.IndexOf(',', num);
			if (num4 == -1)
			{
				return;
			}
			result = 0.0;
			double.TryParse(theString.Substring(num, num4 - num), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
			mOutMax = (float)result;
			num = num4 + 1;
			num4 = theString.IndexOf(',', num);
			if (num4 == -1)
			{
				return;
			}
			result = 0.0;
			double.TryParse(theString.Substring(num, num4 - num), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
			mIncRate = (float)result;
			num = num4 + 1;
			if (num2 >= 1)
			{
				num4 = theString.IndexOf(',', num);
				if (num4 == -1)
				{
					return;
				}
				result = 0.0;
				double.TryParse(theString.Substring(num, num4 - num), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
				mInMax = (float)result;
				num = num4 + 1;
			}
			string key = theString.Substring(num);
			if (!mCurveCacheMap.ContainsKey(key))
			{
				CurveCacheRecord curveCacheRecord = null;
				curveCacheRecord = new CurveCacheRecord();
				mCurveCacheMap.Add(key, curveCacheRecord);
				mCurveCacheRecord = curveCacheRecord;
				List<DataPoint> list = new List<DataPoint>();
				float num5 = 0f;
				while (num < theString.Length)
				{
					sbyte theChar = (sbyte)theString[num++];
					DataPoint dataPoint = new DataPoint();
					dataPoint.mX = num5;
					dataPoint.mY = CVCharToFloat(theChar);
					if (mIsHermite)
					{
						string theStr = theString.Substring(num, 3);
						dataPoint.mAngleDeg = CVStrToAngle(theStr);
						num += 3;
					}
					else
					{
						dataPoint.mAngleDeg = 0f;
					}
					list.Add(dataPoint);
					while (num < theString.Length)
					{
						theChar = (sbyte)theString[num++];
						if (theChar == 32)
						{
							num5 += 0.1f;
							continue;
						}
						num5 = Math.Min(num5 + CVCharToFloat(theChar) * 0.1f, 1f);
						break;
					}
				}
				GenerateTable(list, mCurveCacheRecord.mTable, CV_NUM_SPLINE_POINTS);
				mCurveCacheRecord.mDataStr = theString;
				mCurveCacheRecord.mHermiteCurve.mPoints.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					DataPoint dataPoint2 = list[i];
					float inFxPrime = (float)Math.Tan(SexyMath.DegToRad(dataPoint2.mAngleDeg));
					mCurveCacheRecord.mHermiteCurve.mPoints.Add(new SexyMathHermite.SPoint(dataPoint2.mX, dataPoint2.mY, inFxPrime));
				}
				mCurveCacheRecord.mHermiteCurve.Rebuild();
			}
			else
			{
				mCurveCacheRecord = mCurveCacheMap[key];
			}
		}

		public CurvedVal()
		{
			InitVarDefaults();
		}

		public CurvedVal(string theData, CurvedVal theLinkedVal)
		{
			InitVarDefaults();
			SetCurve(theData, theLinkedVal);
		}

		public CurvedVal(string theDataP)
			: this(theDataP, null)
		{
		}

		public void SetCurve(string theData, CurvedVal theLinkedVal)
		{
			mDataP = theData;
			mCurDataPStr = theData;
			if (mAppUpdateCountSrc != null)
			{
				mInitAppUpdateCount = mAppUpdateCountSrc;
			}
			mTriggered = false;
			mLinkedVal = theLinkedVal;
			mRamp = 6;
			ParseDataString(theData);
			mInVal = mInMin;
		}

		public void SetCurve(string theData)
		{
			SetCurve(theData, null);
		}

		public void SetCurveMult(string theData)
		{
			SetCurveMult(theData, null);
		}

		public void SetCurveMult(string theData, CurvedVal theLinkedVal)
		{
			double outVal = GetOutVal();
			SetCurve(theData, theLinkedVal);
			mOutMax *= outVal;
		}

		public void SetConstant(double theValue)
		{
			mInVal = 0.0;
			mTriggered = false;
			mLinkedVal = null;
			mRamp = 1;
			mInMin = (mInMax = 0.0);
			mOutMin = (mOutMax = theValue);
		}

		public bool IsInitialized()
		{
			return mRamp != 0;
		}

		public void SetMode(int theMode)
		{
			mMode = (byte)theMode;
		}

		public void SetRamp(int theRamp)
		{
			mRamp = (byte)theRamp;
		}

		public void SetOutRange(double theMin, double theMax)
		{
			mOutMin = theMin;
			mOutMax = theMax;
		}

		public void SetInRange(double theMin, double theMax)
		{
			mInMin = theMin;
			mInMax = theMax;
		}

		public double GetOutVal()
		{
			return mCurOutVal = GetOutVal(GetInVal());
		}

		public double GetOutVal(double theInVal)
		{
			switch ((Ramp)mRamp)
			{
			case Ramp.RAMP_NONE:
			case Ramp.RAMP_LINEAR:
				if (mMode == 2)
				{
					if (theInVal - mInMin <= (mInMax - mInMin) * 0.5)
					{
						return mOutMin + (theInVal - mInMin) / (mInMax - mInMin) * (mOutMax - mOutMin) * 2.0;
					}
					return mOutMin + (1.0 - (theInVal - mInMin) / (mInMax - mInMin)) * (mOutMax - mOutMin) * 2.0;
				}
				if (mInMin == mInMax)
				{
					return mOutMin;
				}
				return mOutMin + (theInVal - mInMin) / (mInMax - mInMin) * (mOutMax - mOutMin);
			case Ramp.RAMP_SLOW_TO_FAST:
			{
				double num8 = (theInVal - mInMin) / (mInMax - mInMin) * PI * 0.5;
				if (mMode == 2)
				{
					num8 *= 2.0;
				}
				if (num8 > PI * 0.5)
				{
					num8 = PI - num8;
				}
				return mOutMin + (1.0 - Math.Cos(num8)) * (mOutMax - mOutMin);
			}
			case Ramp.RAMP_FAST_TO_SLOW:
			{
				double num9 = (theInVal - mInMin) / (mInMax - mInMin) * PI * 0.5;
				if (mMode == 2)
				{
					num9 *= 2.0;
				}
				return mOutMin + Math.Sin(num9) * (mOutMax - mOutMin);
			}
			case Ramp.RAMP_SLOW_FAST_SLOW:
			{
				double num6 = (theInVal - mInMin) / (mInMax - mInMin) * PI;
				if (mMode == 2)
				{
					num6 *= 2.0;
				}
				return mOutMin + (0.0 - Math.Cos(num6) + 1.0) * 0.5 * (mOutMax - mOutMin);
			}
			case Ramp.RAMP_FAST_SLOW_FAST:
			{
				double num7 = (theInVal - mInMin) / (mInMax - mInMin) * PI;
				if (mMode == 2)
				{
					num7 *= 2.0;
				}
				if (num7 > PI)
				{
					num7 = PI * 2.0 - num7;
				}
				if (num7 < PI * 0.5)
				{
					return mOutMin + Math.Sin(num7) * 0.5 * (mOutMax - mOutMin);
				}
				return mOutMin + (2.0 - Math.Sin(num7)) * 0.5 * (mOutMax - mOutMin);
			}
			case Ramp.RAMP_CURVEDATA:
			{
				CheckCurveChange();
				if (mCurveCacheRecord == null)
				{
					return 0.0;
				}
				if (mInMax - mInMin == 0.0)
				{
					return 0.0;
				}
				float num = (float)Math.Min((theInVal - mInMin) / (mInMax - mInMin), 1.0);
				if (mMode == 2)
				{
					num = ((!(num > 0.5f)) ? (num * 2f) : ((1f - num) * 2f));
				}
				if (mIsHermite)
				{
					double num2 = mOutMin + (double)mCurveCacheRecord.mHermiteCurve.Evaluate(num) * (mOutMax - mOutMin);
					if (!mNoClip)
					{
						num2 = ((!(mOutMin < mOutMax)) ? Math.Max(Math.Min(num2, mOutMin), mOutMax) : Math.Min(Math.Max(num2, mOutMin), mOutMax));
					}
					return num2;
				}
				float num3 = num * (float)(CV_NUM_SPLINE_POINTS - 1);
				int num4 = (int)num3;
				if (num4 == CV_NUM_SPLINE_POINTS - 1)
				{
					return mOutMin + (double)mCurveCacheRecord.mTable[num4] * (mOutMax - mOutMin);
				}
				float num5 = num3 - (float)num4;
				return mOutMin + (double)(mCurveCacheRecord.mTable[num4] * (1f - num5) + mCurveCacheRecord.mTable[num4 + 1] * num5) * (mOutMax - mOutMin);
			}
			default:
				return mOutMin;
			}
		}

		public double GetOutValDelta()
		{
			return GetOutVal() - mPrevOutVal;
		}

		public double GetOutFinalVal()
		{
			return GetOutVal(mInMax);
		}

		public double GetInVal()
		{
			double num = mInVal;
			if (mLinkedVal != null)
			{
				num = ((!mLinkedVal.mOutputSync) ? mLinkedVal.GetInVal() : mLinkedVal.GetOutVal());
			}
			else if (mAutoInc)
			{
				if (mAppUpdateCountSrc != null)
				{
					num = mInMin + (double)((int)mAppUpdateCountSrc - mInitAppUpdateCount) * mIncRate;
				}
				num = ((mMode != 1 && mMode != 2) ? Math.Min(num, mInMax) : ((num - mInMin) % (mInMax - mInMin) + mInMin));
			}
			if (mMode == 2)
			{
				double num2 = (float)((num - mInMin) / (mInMax - mInMin));
				if (num2 > 0.5)
				{
					return mInMin + (1.0 - num2) * 2.0 * (mInMax - mInMin);
				}
				return mInMin + num2 * 2.0 * (mInMax - mInMin);
			}
			return num;
		}

		public bool SetInVal(double theVal)
		{
			return SetInVal(theVal, false);
		}

		public bool SetInVal(double theVal, bool theRealignAutoInc)
		{
			mPrevOutVal = GetOutVal();
			mTriggered = false;
			mPrevInVal = theVal;
			if (mAutoInc && theRealignAutoInc)
			{
				mInitAppUpdateCount -= (int)((theVal - mInVal) * 100.0);
			}
			mInVal = theVal;
			if (!CheckClamping())
			{
				if (!mTriggered)
				{
					mTriggered = true;
					return false;
				}
				return mSingleTrigger;
			}
			return true;
		}

		public bool IncInVal(double theInc)
		{
			mPrevOutVal = GetOutVal();
			mPrevInVal = mInVal;
			mInVal += theInc;
			if (!CheckClamping())
			{
				if (!mTriggered)
				{
					mTriggered = true;
					return false;
				}
				return mSingleTrigger;
			}
			return true;
		}

		public bool IncInVal()
		{
			if (mIncRate == 0.0)
			{
				return false;
			}
			return IncInVal(mIncRate);
		}

		public bool IncInValScalar(float theScalar)
		{
			if (mIncRate != 0.0)
			{
				return IncInVal(mIncRate * (double)theScalar);
			}
			return false;
		}

		public void Intercept(string theDataP, CurvedVal theInterceptCv, double theCheckInIncrPct, bool theStopAtLocalMin)
		{
			double theTargetOutVal = ((theInterceptCv == null) ? this : theInterceptCv);
			SetCurve(theDataP);
			SetInVal(FindClosestInToOutVal(theTargetOutVal, theCheckInIncrPct, 0.0, 1.0, theStopAtLocalMin), true);
		}

		public void Intercept(string theData, CurvedVal theInterceptCv, double theCheckInIncrPct)
		{
			Intercept(theData, theInterceptCv, theCheckInIncrPct, false);
		}

		public void Intercept(string theData, CurvedVal theInterceptCv)
		{
			Intercept(theData, theInterceptCv, 0.01, false);
		}

		public void Intercept(string theData)
		{
			Intercept(theData, null, 0.01, false);
		}

		public double FindClosestInToOutVal(double theTargetOutVal, double theCheckInIncrPct, double theCheckInRangeMinPct, double theCheckInRangeMaxPct)
		{
			return FindClosestInToOutVal(theTargetOutVal, theCheckInIncrPct, theCheckInRangeMinPct, theCheckInRangeMaxPct, false);
		}

		public double FindClosestInToOutVal(double theTargetOutVal, double theCheckInIncrPct, double theCheckInRangeMinPct)
		{
			return FindClosestInToOutVal(theTargetOutVal, theCheckInIncrPct, theCheckInRangeMinPct, 1.0, false);
		}

		public double FindClosestInToOutVal(double theTargetOutVal, double theCheckInIncrPct)
		{
			return FindClosestInToOutVal(theTargetOutVal, theCheckInIncrPct, 0.0, 1.0, false);
		}

		public double FindClosestInToOutVal(double theTargetOutVal)
		{
			return FindClosestInToOutVal(theTargetOutVal, 0.01, 0.0, 1.0, false);
		}

		public double FindClosestInToOutVal(double theTargetOutVal, double theCheckInIncrPct, double theCheckInRangeMinPct, double theCheckInRangeMaxPct, bool theStopAtLocalMin)
		{
			double num = mInMax - mInMin;
			double num2 = mInMin + num * theCheckInRangeMaxPct;
			double num3 = 0.0;
			double num4 = -1.0;
			for (double num5 = mInMin + num * theCheckInRangeMinPct; num5 <= num2; num5 += num * theCheckInIncrPct)
			{
				double num6 = Math.Abs(theTargetOutVal - GetOutVal(num5));
				if (num4 < 0.0 || num6 < num3)
				{
					num3 = num6;
					num4 = num5;
				}
				else if (theStopAtLocalMin)
				{
					return num4;
				}
			}
			return num4;
		}

		public double GetInValAtUpdate(int theUpdateCount)
		{
			return mInMin + (double)theUpdateCount * mIncRate;
		}

		public int GetLengthInUpdates()
		{
			if (mIncRate == 0.0)
			{
				return -1;
			}
			return (int)Math.Ceiling((mInMax - mInMin) / mIncRate);
		}

		public bool CheckInThreshold(double theInVal)
		{
			double inVal = mInVal;
			double num = mPrevInVal;
			if (mAutoInc)
			{
				inVal = GetInVal();
				num = inVal - mIncRate * 1.5;
			}
			if (theInVal > num)
			{
				return theInVal <= inVal;
			}
			return false;
		}

		public bool CheckUpdatesFromEndThreshold(int theUpdateCount)
		{
			return CheckInThreshold(GetInValAtUpdate(GetLengthInUpdates() - theUpdateCount));
		}

		public bool HasBeenTriggered()
		{
			if (mAutoInc)
			{
				mTriggered = GetInVal() == mInMax;
			}
			return mTriggered;
		}

		public void ClearTrigger()
		{
			mTriggered = false;
		}

		public bool IsDoingCurve()
		{
			if (GetInVal() != mInMax)
			{
				return mRamp != 0;
			}
			return false;
		}
	}
}
