using System;

namespace SexyFramework.Misc
{
	public class SexyMath
	{
		public static float Fabs(float inX)
		{
			return Math.Abs(inX);
		}

		public static double Fabs(double inX)
		{
			return Math.Abs(inX);
		}

		public static float DegToRad(float inX)
		{
			return inX * (float)Math.PI / 180f;
		}

		public static float RadToDeg(float inX)
		{
			return inX * 180f / (float)Math.PI;
		}

		public static bool ApproxEquals(float inL, float inR, float inTol)
		{
			return Fabs(inL - inR) <= inTol;
		}

		public static bool ApproxEquals(double inL, double inR, double inTol)
		{
			return Fabs(inL - inR) <= inTol;
		}

		public static float Lerp(float inA, float inB, float inAlpha)
		{
			return inA + (inB - inA) * inAlpha;
		}

		public static double Lerp(double inA, double inB, double inAlpha)
		{
			return inA + (inB - inA) * inAlpha;
		}

		public static bool IsPowerOfTwo(uint inX)
		{
			if (inX != 0)
			{
				return (inX & (inX - 1)) == 0;
			}
			return false;
		}

		public static float SinF(float value)
		{
			return (float)Math.Sin(value);
		}

		public static float CosF(float value)
		{
			return (float)Math.Cos(value);
		}
	}
}
