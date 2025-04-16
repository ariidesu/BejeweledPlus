using System;

namespace SexyFramework.Misc
{
	public struct DPoint
	{
		public double mX;

		public double mY;

		public DPoint(double theX, double theY)
		{
			mX = theX;
			mY = theY;
		}

		public DPoint(DPoint theTPoint)
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
			if (obj != null && obj is DPoint)
			{
				DPoint dPoint = (DPoint)obj;
				if (dPoint.mX == mX)
				{
					return dPoint.mY == mY;
				}
				return false;
			}
			return false;
		}

		public static bool operator ==(DPoint ImpliedObject, DPoint p)
		{
			if ((object)ImpliedObject == null)
			{
				return (object)p == null;
			}
			return ImpliedObject.Equals(p);
		}

		public static bool operator !=(DPoint ImpliedObject, DPoint p)
		{
			return !(ImpliedObject == p);
		}

		public double Magnitude()
		{
			return Math.Sqrt(mX * mX + mY * mY);
		}

		public static DPoint operator +(DPoint ImpliedObject, DPoint p)
		{
			return new DPoint(ImpliedObject.mX + p.mX, ImpliedObject.mY + p.mY);
		}

		public static DPoint operator -(DPoint ImpliedObject, DPoint p)
		{
			return new DPoint(ImpliedObject.mX - p.mX, ImpliedObject.mY - p.mY);
		}

		public static DPoint operator *(DPoint ImpliedObject, DPoint p)
		{
			return new DPoint(ImpliedObject.mX * p.mX, ImpliedObject.mY * p.mY);
		}

		public static DPoint operator /(DPoint ImpliedObject, DPoint p)
		{
			return new DPoint(ImpliedObject.mX / p.mX, ImpliedObject.mY / p.mY);
		}

		public static DPoint operator *(DPoint ImpliedObject, int s)
		{
			return new DPoint(ImpliedObject.mX * (double)s, ImpliedObject.mY * (double)s);
		}

		public static DPoint operator /(DPoint ImpliedObject, float s)
		{
			return new DPoint(ImpliedObject.mX / (double)s, ImpliedObject.mY / (double)s);
		}

		public static DPoint operator *(DPoint ImpliedObject, double s)
		{
			return new DPoint(ImpliedObject.mX * s, ImpliedObject.mY * s);
		}

		public static DPoint operator /(DPoint ImpliedObject, double s)
		{
			return new DPoint(ImpliedObject.mX / s, ImpliedObject.mY / s);
		}

		public static DPoint operator *(DPoint ImpliedObject, float s)
		{
			return new DPoint(ImpliedObject.mX * (double)s, ImpliedObject.mY * (double)s);
		}

		public static DPoint operator /(DPoint ImpliedObject, int s)
		{
			return new DPoint(ImpliedObject.mX / (double)s, ImpliedObject.mY / (double)s);
		}
	}
}
