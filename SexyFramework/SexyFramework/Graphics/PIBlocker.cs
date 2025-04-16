using System;
using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class PIBlocker : IDisposable
	{
		public string mName;

		public int mUseLayersBelowForBg;

		public PIValue2D mPos = new PIValue2D();

		public PIValue mActive = new PIValue();

		public PIValue mAngle = new PIValue();

		public List<PIValue2D> mPoints = new List<PIValue2D>();

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
		}
	}
}
