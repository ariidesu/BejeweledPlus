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
			for (int i = 1; i < mValuePoint2DVector.Count; i++)
			{
				PIValuePoint2D pIValuePoint2D = mValuePoint2DVector[i - 1];
				PIValuePoint2D pIValuePoint2D2 = mValuePoint2DVector[i];
				if ((theTime >= pIValuePoint2D.mTime && theTime <= pIValuePoint2D2.mTime) || i == mValuePoint2DVector.Count - 1)
				{
					return mLastPoint = pIValuePoint2D.mValue + (pIValuePoint2D2.mValue - pIValuePoint2D.mValue) * Math.Min(1f, (theTime - pIValuePoint2D.mTime) / (pIValuePoint2D2.mTime - pIValuePoint2D.mTime));
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
			for (int i = 1; i < mValuePoint2DVector.Count; i++)
			{
				PIValuePoint2D pIValuePoint2D = mValuePoint2DVector[i - 1];
				PIValuePoint2D pIValuePoint2D2 = mValuePoint2DVector[i];
				if ((theTime >= pIValuePoint2D.mTime && theTime <= pIValuePoint2D2.mTime) || i == mValuePoint2DVector.Count - 1)
				{
					return mLastVelocity = pIValuePoint2D2.mValue - pIValuePoint2D.mValue;
				}
			}
			return mLastVelocity = new Vector2(0f, 0f);
		}
	}
}
