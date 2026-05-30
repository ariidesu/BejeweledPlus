using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class PIValue2D : IDisposable
	{
		public List<PIValuePoint2D> mValuePoint2DVector = new List<PIValuePoint2D>();

		public Bezier mBezier = new Bezier();

		public float mLastTime;

		public Vector2 mLastPoint = default(Vector2);

		public float mLastVelocityTime;

		public Vector2 mLastVelocity = default(Vector2);

		public PIValue2D()
		{
			mLastTime = -1f;
		}

		public virtual void Dispose()
		{
			mBezier.Dispose();
			mValuePoint2DVector.Clear();
		}

		public Vector2 GetValueAt(float theTime)
		{
			if (mLastTime == theTime)
			{
				return mLastPoint;
			}
			mLastTime = theTime;
			if (mValuePoint2DVector.Count == 1)
			{
				return mLastPoint = mValuePoint2DVector[0].mValue;
			}
			if (mBezier.IsInitialized())
			{
				return mLastPoint = mBezier.Evaluate(theTime);
			}
			for (int aKeyIdx = 1; aKeyIdx < mValuePoint2DVector.Count; aKeyIdx++)
			{
				PIValuePoint2D aP1 = mValuePoint2DVector[aKeyIdx - 1];
				PIValuePoint2D aP2 = mValuePoint2DVector[aKeyIdx];
				if ((theTime >= aP1.mTime && theTime <= aP2.mTime) || aKeyIdx == mValuePoint2DVector.Count - 1)
				{
					float aDenom = aP2.mTime - aP1.mTime;
					float aPct = (aDenom > 0f) ? Math.Min(1f, (theTime - aP1.mTime) / aDenom) : 0f;
					return mLastPoint = aP1.mValue + (aP2.mValue - aP1.mValue) * aPct;
				}
			}
			return mLastPoint = new Vector2(0f, 0f);
		}

		public Vector2 GetVelocityAt(float theTime)
		{
			if (mLastVelocityTime == theTime)
			{
				return mLastVelocity;
			}
			mLastVelocityTime = theTime;
			if (mValuePoint2DVector.Count <= 1)
			{
				return new Vector2(0f, 0f);
			}
			if (mBezier.IsInitialized())
			{
				return mLastVelocity = mBezier.Velocity(theTime, false);
			}
			for (int aKeyIdx = 1; aKeyIdx < mValuePoint2DVector.Count; aKeyIdx++)
			{
				PIValuePoint2D aP1 = mValuePoint2DVector[aKeyIdx - 1];
				PIValuePoint2D aP2 = mValuePoint2DVector[aKeyIdx];
				if ((theTime >= aP1.mTime && theTime <= aP2.mTime) || aKeyIdx == mValuePoint2DVector.Count - 1)
				{
					return mLastVelocity = aP2.mValue - aP1.mValue;
				}
			}
			return mLastVelocity = new Vector2(0f, 0f);
		}
	}
}
