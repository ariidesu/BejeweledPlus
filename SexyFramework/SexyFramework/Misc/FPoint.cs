using System;
using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public struct FPoint
	{
		public float mX;

		public float mY;

		public FPoint(float theX, float theY)
		{
			mX = theX;
			mY = theY;
		}

		public FPoint(FPoint theTPoint)
		{
			mX = theTPoint.mX;
			mY = theTPoint.mY;
		}

		public FPoint CopyForm(FPoint obj)
		{
			mX = obj.mX;
			mY = obj.mY;
			return this;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is FPoint)
			{
				Point point = (Point)obj;
				if ((float)point.mX == mX)
				{
					return (float)point.mY == mY;
				}
				return false;
			}
			return false;
		}

		public static bool operator ==(FPoint ImpliedObject, FPoint p)
		{
			if ((object)ImpliedObject == null)
			{
				return (object)p == null;
			}
			return ImpliedObject.Equals(p);
		}

		public static bool operator !=(FPoint ImpliedObject, FPoint p)
		{
			return !(ImpliedObject == p);
		}

		public float Magnitude()
		{
			return (float)Math.Sqrt(mX * mX + mY * mY);
		}

		public static FPoint operator +(FPoint ImpliedObject, FPoint p)
		{
			return new FPoint(ImpliedObject.mX + p.mX, ImpliedObject.mY + p.mY);
		}

		public static FPoint operator -(FPoint ImpliedObject, FPoint p)
		{
			return new FPoint(ImpliedObject.mX - p.mX, ImpliedObject.mY - p.mY);
		}

		public static FPoint operator *(FPoint ImpliedObject, FPoint p)
		{
			return new FPoint(ImpliedObject.mX * p.mX, ImpliedObject.mY * p.mY);
		}

		public static FPoint operator /(FPoint ImpliedObject, FPoint p)
		{
			return new FPoint(ImpliedObject.mX / p.mX, ImpliedObject.mY / p.mY);
		}

		public static FPoint operator *(FPoint ImpliedObject, int s)
		{
			return new FPoint(ImpliedObject.mX * (float)s, ImpliedObject.mY * (float)s);
		}

		public static FPoint operator /(FPoint ImpliedObject, float s)
		{
			return new FPoint(ImpliedObject.mX / s, ImpliedObject.mY / s);
		}

		public static FPoint operator *(FPoint ImpliedObject, double s)
		{
			return new FPoint((float)((double)ImpliedObject.mX * s), (float)((double)ImpliedObject.mY * s));
		}

		public static FPoint operator /(FPoint ImpliedObject, double s)
		{
			return new FPoint((float)((double)ImpliedObject.mX / s), (float)((double)ImpliedObject.mY / s));
		}

		public static FPoint operator *(FPoint ImpliedObject, float s)
		{
			return new FPoint(ImpliedObject.mX * s, ImpliedObject.mY * s);
		}

		public static FPoint operator /(FPoint ImpliedObject, int s)
		{
			return new FPoint(ImpliedObject.mX / (float)s, ImpliedObject.mY / (float)s);
		}

		internal void SetValue(float p, float p_2)
		{
			mX = p;
			mY = p_2;
		}

		public Vector2 ToXnaVector2()
		{
			return new Vector2(mX, mY);
		}
	}
}
