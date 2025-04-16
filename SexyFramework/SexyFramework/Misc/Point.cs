using System;

namespace SexyFramework.Misc
{
	public struct Point
	{
		public int mX;

		public int mY;

		public Point(int theX, int theY)
		{
			mX = theX;
			mY = theY;
		}

		public Point(Point theTPoint)
		{
			mX = theTPoint.mX;
			mY = theTPoint.mY;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj != null)
			{
				Point point = (Point)obj;
				result = point.mX == mX && point.mY == mY;
			}
			return result;
		}

		public static bool operator ==(Point ImpliedObject, Point p)
		{
			if ((object)ImpliedObject == null)
			{
				return (object)p == null;
			}
			return ImpliedObject.Equals(p);
		}

		public static bool operator !=(Point ImpliedObject, Point p)
		{
			return !(ImpliedObject == p);
		}

		public int Magnitude()
		{
			return (int)Math.Sqrt(mX * mX + mY * mY);
		}

		public static Point operator +(Point ImpliedObject, Point p)
		{
			ImpliedObject.mX += p.mX;
			ImpliedObject.mY += p.mY;
			return ImpliedObject;
		}

		public static Point operator -(Point ImpliedObject, Point p)
		{
			return new Point(ImpliedObject.mX - p.mX, ImpliedObject.mY - p.mY);
		}

		public static Point operator *(Point ImpliedObject, Point p)
		{
			return new Point(ImpliedObject.mX * p.mX, ImpliedObject.mY * p.mY);
		}

		public static Point operator /(Point ImpliedObject, Point p)
		{
			return new Point(ImpliedObject.mX / p.mX, ImpliedObject.mY / p.mY);
		}

		public static Point operator *(Point ImpliedObject, int s)
		{
			return new Point(ImpliedObject.mX * s, ImpliedObject.mY * s);
		}

		public static Point operator *(Point ImpliedObject, double s)
		{
			return new Point((int)((double)ImpliedObject.mX * s), (int)((double)ImpliedObject.mY * s));
		}

		public static Point operator *(Point ImpliedObject, float s)
		{
			return new Point((int)((float)ImpliedObject.mX * s), (int)((float)ImpliedObject.mY * s));
		}

		public static Point operator /(Point ImpliedObject, float s)
		{
			return new Point((int)((float)ImpliedObject.mX / s), (int)((float)ImpliedObject.mY / s));
		}

		public static Point operator /(Point ImpliedObject, double s)
		{
			return new Point((int)((double)ImpliedObject.mX / s), (int)((double)ImpliedObject.mY / s));
		}

		public static Point operator /(Point ImpliedObject, int s)
		{
			return new Point(ImpliedObject.mX / s, ImpliedObject.mY / s);
		}
	}
}
