using System;
using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public struct Rect
	{
		public static Rect ZERO_RECT = new Rect(0, 0, 0, 0);

		public static Rect INVALIDATE_RECT = new Rect(-1, -1, -1, -1);

		public int mX;

		public int mY;

		public int mWidth;

		public int mHeight;

		public Rect(int theX, int theY, int theWidth, int theHeight)
		{
			mX = theX;
			mY = theY;
			mWidth = theWidth;
			mHeight = theHeight;
		}

		public Rect(Rect theTRect)
		{
			mX = theTRect.mX;
			mY = theTRect.mY;
			mWidth = theTRect.mWidth;
			mHeight = theTRect.mHeight;
		}

		public void SetValue(int theX, int theY, int theWidth, int theHeight)
		{
			mX = theX;
			mY = theY;
			mWidth = theWidth;
			mHeight = theHeight;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Rect)
			{
				Rect rect = (Rect)obj;
				if (rect.mX == mX && rect.mY == mY && rect.mWidth == mWidth)
				{
					return rect.mHeight == mHeight;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void setValue(ref int x, ref int y, ref int width, ref int height)
		{
			mX = x;
			mY = y;
			mWidth = width;
			mHeight = height;
		}

		public void setValue(int x, int y, int width, int height)
		{
			mX = x;
			mY = y;
			mWidth = width;
			mHeight = height;
		}

		public bool Intersects(Rect theTRect)
		{
			if (theTRect.mX + theTRect.mWidth > mX && theTRect.mY + theTRect.mHeight > mY && theTRect.mX < mX + mWidth)
			{
				return theTRect.mY < mY + mHeight;
			}
			return false;
		}

		public bool Intersects(int x, int y, int w, int h)
		{
			if (x + w > mX && y + h > mY && x < mX + mWidth)
			{
				return y < mY + mHeight;
			}
			return false;
		}

		public Rect Intersection(Rect theTRect)
		{
			int num = Math.Max(mX, theTRect.mX);
			int num2 = Math.Min(mX + mWidth, theTRect.mX + theTRect.mWidth);
			int num3 = Math.Max(mY, theTRect.mY);
			int num4 = Math.Min(mY + mHeight, theTRect.mY + theTRect.mHeight);
			if (num2 - num < 0 || num4 - num3 < 0)
			{
				return new Rect(0, 0, 0, 0);
			}
			return new Rect(num, num3, num2 - num, num4 - num3);
		}

		public Rect Union(Rect theTRect)
		{
			int num = Math.Min(mX, theTRect.mX);
			int num2 = Math.Max(mX + mWidth, theTRect.mX + theTRect.mWidth);
			int num3 = Math.Min(mY, theTRect.mY);
			int num4 = Math.Max(mY + mHeight, theTRect.mY + theTRect.mHeight);
			return new Rect(num, num3, num2 - num, num4 - num3);
		}

		public bool Contains(int theX, int theY)
		{
			if (theX >= mX && theX < mX + mWidth && theY >= mY)
			{
				return theY < mY + mHeight;
			}
			return false;
		}

		public bool Contains(Point thePoint)
		{
			if (thePoint.mX >= mX && thePoint.mX < mX + mWidth && thePoint.mY >= mY)
			{
				return thePoint.mY < mY + mHeight;
			}
			return false;
		}

		public void Offset(int theX, int theY)
		{
			mX += theX;
			mY += theY;
		}

		public void Offset(Vector2 thePoint)
		{
			mX += (int)thePoint.X;
			mY += (int)thePoint.Y;
		}

		public Rect Inflate(int theX, int theY)
		{
			mX -= theX;
			mWidth += theX * 2;
			mY -= theY;
			mHeight += theY * 2;
			return this;
		}

		public void Scale(double theScaleX, double theScaleY)
		{
			mX = (int)((double)mX * theScaleX);
			mY = (int)((double)mY * theScaleY);
			mWidth = (int)((double)mWidth * theScaleX);
			mHeight = (int)((double)mHeight * theScaleY);
		}

		public void Scale(double theScaleX, double theScaleY, int theCenterX, int theCenterY)
		{
			Offset(-theCenterX, -theCenterY);
			Scale(theScaleX, theScaleY);
			Offset(theCenterX, theCenterY);
		}

		public static bool operator ==(Rect ImpliedObject, Rect theRect)
		{
			if (ImpliedObject.mX == theRect.mX && ImpliedObject.mY == theRect.mY && ImpliedObject.mWidth == theRect.mWidth)
			{
				return ImpliedObject.mHeight == theRect.mHeight;
			}
			return false;
		}

		public static bool operator !=(Rect ImpliedObject, Rect theRect)
		{
			return !(ImpliedObject == theRect);
		}
	}
}
