using System;
using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public struct FRect
	{
		public float mX;

		public float mY;

		public float mWidth;

		public float mHeight;

		public static FRect ZeroRect = new FRect(0f, 0f, 0f, 0f);

		public static FRect ZERO_RECT = new FRect(0f, 0f, 0f, 0f);

		public static FRect INVALIDATE_RECT = new FRect(-1f, -1f, -1f, -1f);

		public FRect(float theX, float theY, float theWidth, float theHeight)
		{
			mX = theX;
			mY = theY;
			mWidth = theWidth;
			mHeight = theHeight;
		}

		public FRect(FRect theTRect)
		{
			mX = theTRect.mX;
			mY = theTRect.mY;
			mWidth = theTRect.mWidth;
			mHeight = theTRect.mHeight;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is FRect)
			{
				FRect fRect = (FRect)obj;
				if (fRect.mX == mX && fRect.mY == mY && fRect.mWidth == mWidth)
				{
					return fRect.mHeight == mHeight;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Intersects(FRect theTRect)
		{
			if (!(theTRect.mX + theTRect.mWidth <= mX) && !(theTRect.mY + theTRect.mHeight <= mY) && !(theTRect.mX >= mX + mWidth))
			{
				return !(theTRect.mY >= mY + mHeight);
			}
			return false;
		}

		public FRect Intersection(FRect theTRect)
		{
			float num = Math.Max(mX, theTRect.mX);
			float num2 = Math.Min(mX + mWidth, theTRect.mX + theTRect.mWidth);
			float num3 = Math.Max(mY, theTRect.mY);
			float num4 = Math.Min(mY + mHeight, theTRect.mY + theTRect.mHeight);
			if (num2 - num < 0f || num4 - num3 < 0f)
			{
				return new FRect(0f, 0f, 0f, 0f);
			}
			return new FRect(num, num3, num2 - num, num4 - num3);
		}

		public FRect Union(FRect theTRect)
		{
			float num = Math.Min(mX, theTRect.mX);
			float num2 = Math.Max(mX + mWidth, theTRect.mX + theTRect.mWidth);
			float num3 = Math.Min(mY, theTRect.mY);
			float num4 = Math.Max(mY + mHeight, theTRect.mY + theTRect.mHeight);
			return new FRect(num, num3, num2 - num, num4 - num3);
		}

		public bool Contains(float theX, float theY)
		{
			if (theX >= mX && theX < mX + mWidth && theY >= mY)
			{
				return theY < mY + mHeight;
			}
			return false;
		}

		public bool Contains(Vector2 thePoint)
		{
			if (thePoint.X >= mX && thePoint.X < mX + mWidth && thePoint.Y >= mY)
			{
				return thePoint.Y < mY + mHeight;
			}
			return false;
		}

		public void Offset(float theX, float theY)
		{
			mX += theX;
			mY += theY;
		}

		public void Offset(Vector2 thePoint)
		{
			mX += thePoint.X;
			mY += thePoint.Y;
		}

		public FRect Inflate(float theX, float theY)
		{
			mX -= theX;
			mWidth += theX * 2f;
			mY -= theY;
			mHeight += theY * 2f;
			return this;
		}

		public void Scale(double theScaleX, double theScaleY)
		{
			mX = (float)((double)mX * theScaleX);
			mY = (float)((double)mY * theScaleY);
			mWidth = (float)((double)mWidth * theScaleX);
			mHeight = (float)((double)mHeight * theScaleY);
		}

		public void Scale(double theScaleX, double theScaleY, float theCenterX, float theCenterY)
		{
			Offset(0f - theCenterX, 0f - theCenterY);
			Scale(theScaleX, theScaleY);
			Offset(theCenterX, theCenterY);
		}

		public static bool operator ==(FRect ImpliedObject, FRect theRect)
		{
			if ((object)ImpliedObject == null)
			{
				return (object)theRect == null;
			}
			return ImpliedObject.Equals(theRect);
		}

		public static bool operator !=(FRect ImpliedObject, FRect theRect)
		{
			return !(ImpliedObject == theRect);
		}
	}
}
