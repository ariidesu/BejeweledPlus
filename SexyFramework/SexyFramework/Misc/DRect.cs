using System;
using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public struct DRect
	{
		public static DRect ZERO_RECT = new DRect(0.0, 0.0, 0.0, 0.0);

		public static DRect INVALIDATE_RECT = new DRect(-1.0, -1.0, -1.0, -1.0);

		public double mX;

		public double mY;

		public double mWidth;

		public double mHeight;

		public DRect(double theX, double theY, double theWidth, double theHeight)
		{
			mX = theX;
			mY = theY;
			mWidth = theWidth;
			mHeight = theHeight;
		}

		public DRect(DRect theTRect)
		{
			mX = theTRect.mX;
			mY = theTRect.mY;
			mWidth = theTRect.mWidth;
			mHeight = theTRect.mHeight;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is DRect)
			{
				DRect dRect = (DRect)obj;
				if (dRect.mX == mX && dRect.mY == mY && dRect.mWidth == mWidth)
				{
					return dRect.mHeight == mHeight;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Intersects(DRect theTRect)
		{
			if (!(theTRect.mX + theTRect.mWidth <= mX) && !(theTRect.mY + theTRect.mHeight <= mY) && !(theTRect.mX >= mX + mWidth))
			{
				return !(theTRect.mY >= mY + mHeight);
			}
			return false;
		}

		public DRect Intersection(DRect theTRect)
		{
			double num = Math.Max(mX, theTRect.mX);
			double num2 = Math.Min(mX + mWidth, theTRect.mX + theTRect.mWidth);
			double num3 = Math.Max(mY, theTRect.mY);
			double num4 = Math.Min(mY + mHeight, theTRect.mY + theTRect.mHeight);
			if (num2 - num < 0.0 || num4 - num3 < 0.0)
			{
				return new DRect(0.0, 0.0, 0.0, 0.0);
			}
			return new DRect(num, num3, num2 - num, num4 - num3);
		}

		public DRect Union(DRect theTRect)
		{
			double num = Math.Min(mX, theTRect.mX);
			double num2 = Math.Max(mX + mWidth, theTRect.mX + theTRect.mWidth);
			double num3 = Math.Min(mY, theTRect.mY);
			double num4 = Math.Max(mY + mHeight, theTRect.mY + theTRect.mHeight);
			return new DRect(num, num3, num2 - num, num4 - num3);
		}

		public bool Contains(double theX, double theY)
		{
			if (theX >= mX && theX < mX + mWidth && theY >= mY)
			{
				return theY < mY + mHeight;
			}
			return false;
		}

		public bool Contains(Vector2 thePoint)
		{
			if ((double)thePoint.X >= mX && (double)thePoint.X < mX + mWidth && (double)thePoint.Y >= mY)
			{
				return (double)thePoint.Y < mY + mHeight;
			}
			return false;
		}

		public void Offset(double theX, double theY)
		{
			mX += theX;
			mY += theY;
		}

		public void Offset(Vector2 thePoint)
		{
			mX += thePoint.X;
			mY += thePoint.Y;
		}

		public DRect Inflate(double theX, double theY)
		{
			mX -= theX;
			mWidth += theX * 2.0;
			mY -= theY;
			mHeight += theY * 2.0;
			return this;
		}

		public void Scale(double theScaleX, double theScaleY)
		{
			mX *= theScaleX;
			mY *= theScaleY;
			mWidth *= theScaleX;
			mHeight *= theScaleY;
		}

		public void Scale(double theScaleX, double theScaleY, double theCenterX, double theCenterY)
		{
			Offset(0.0 - theCenterX, 0.0 - theCenterY);
			Scale(theScaleX, theScaleY);
			Offset(theCenterX, theCenterY);
		}

		public static bool operator ==(DRect ImpliedObject, DRect theRect)
		{
			if ((object)ImpliedObject == null)
			{
				return (object)theRect == null;
			}
			return ImpliedObject.Equals(theRect);
		}

		public static bool operator !=(DRect ImpliedObject, DRect theRect)
		{
			return !(ImpliedObject == theRect);
		}
	}
}
