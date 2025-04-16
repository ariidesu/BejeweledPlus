using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SexyFramework.Graphics
{
	public class PIDeflector : IDisposable
	{
		public string mName;

		public float mBounce;

		public float mHits;

		public float mThickness;

		public bool mVisible;

		public PIValue2D mPos = new PIValue2D();

		public PIValue mActive = new PIValue();

		public PIValue mAngle = new PIValue();

		public List<PIValue2D> mPoints = new List<PIValue2D>();

		public List<Vector2> mCurPoints = new List<Vector2>();

		public virtual void Dispose()
		{
			mPos.Dispose();
			mActive.Dispose();
			mAngle.Dispose();
			foreach (PIValue2D mPoint in mPoints)
			{
				mPoint.Dispose();
			}
			mPoints.Clear();
			mCurPoints.Clear();
		}
	}
}
