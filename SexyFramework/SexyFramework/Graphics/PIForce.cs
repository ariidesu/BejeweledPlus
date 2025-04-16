using System;
using Microsoft.Xna.Framework;

namespace SexyFramework.Graphics
{
	public class PIForce : IDisposable
	{
		public string mName;

		public bool mVisible;

		public PIValue2D mPos = new PIValue2D();

		public PIValue mStrength = new PIValue();

		public PIValue mDirection = new PIValue();

		public PIValue mActive = new PIValue();

		public PIValue mAngle = new PIValue();

		public PIValue mWidth = new PIValue();

		public PIValue mHeight = new PIValue();

		public Vector2[] mCurPoints = new Vector2[5];

		public virtual void Dispose()
		{
			mPos.Dispose();
			mActive.Dispose();
			mAngle.Dispose();
			mStrength.Dispose();
			mWidth.Dispose();
			mHeight.Dispose();
			mDirection.Dispose();
		}
	}
}
