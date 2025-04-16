using System;

namespace SexyFramework.Misc
{
	public class SexyQuat3
	{
		public SexyVector3 v = default(SexyVector3);

		public float s;

		public SexyQuat3()
		{
			v = new SexyVector3(0f, 0f, 0f);
			s = 1f;
		}

		public SexyQuat3(SexyQuat3 inQ)
		{
			v = inQ.v;
			s = inQ.s;
		}

		public SexyQuat3(SexyVector3 inV, float inS)
		{
			v = inV;
			s = inS;
		}

		public SexyQuat3(SexyAxes3 inAxes)
		{
			SexyAxes3 sexyAxes = inAxes.OrthoNormalize();
			uint[] array = new uint[3] { 1u, 2u, 0u };
			float[,] array2 = new float[3, 3]
			{
				{
					sexyAxes.vX.x,
					sexyAxes.vX.y,
					sexyAxes.vX.z
				},
				{
					sexyAxes.vY.x,
					sexyAxes.vY.y,
					sexyAxes.vY.z
				},
				{
					sexyAxes.vZ.x,
					sexyAxes.vZ.y,
					sexyAxes.vZ.z
				}
			};
			float[] array3 = new float[4];
			float num = array2[0, 0] + array2[1, 1] + array2[2, 2];
			float num2;
			if (num > 0f)
			{
				num2 = (float)Math.Sqrt(num + 1f);
				s = num2 * 0.5f;
				num2 = 0.5f / num2;
				v.x = (array2[1, 2] - array2[2, 1]) * num2;
				v.y = (array2[2, 0] - array2[0, 2]) * num2;
				v.z = (array2[0, 1] - array2[1, 0]) * num2;
				return;
			}
			uint num3 = 0u;
			if (array2[1, 1] > array2[0, 0])
			{
				num3 = 1u;
			}
			if (array2[2, 2] > array2[num3, num3])
			{
				num3 = 2u;
			}
			uint num4 = array[num3];
			uint num5 = array[num4];
			num2 = (float)Math.Sqrt(array2[num3, num3] - (array2[num4, num4] + array2[num5, num5]) + 1f);
			array3[num3] = num2 * 0.5f;
			if (num2 != 0f)
			{
				num2 = 0.5f / num2;
			}
			s = (array2[num4, num5] - array2[num5, num4]) * num2;
			array3[num4] = (array2[num3, num4] + array2[num4, num3]) * num2;
			array3[num5] = (array2[num3, num5] + array2[num5, num3]) * num2;
			v.x = array3[0];
			v.y = array3[1];
			v.z = array3[2];
		}

		public static implicit operator SexyAxes3(SexyQuat3 ImpliedObject)
		{
			float x = ImpliedObject.v.x;
			float y = ImpliedObject.v.y;
			float z = ImpliedObject.v.z;
			float num = ImpliedObject.s;
			float num2 = x * 2f;
			float num3 = y * 2f;
			float num4 = z * 2f;
			float num5 = num * 2f;
			float num6 = x * num2;
			float num7 = y * num3;
			float num8 = z * num4;
			float num9 = x * num3;
			float num10 = x * num4;
			float num11 = x * num5;
			float num12 = y * num4;
			float num13 = y * num5;
			float num14 = z * num5;
			return new SexyAxes3(new SexyVector3(1f - (num7 + num8), num9 + num14, num10 - num13), new SexyVector3(num9 - num14, 1f - (num6 + num8), num12 + num11), new SexyVector3(num10 + num13, num12 - num11, 1f - (num6 + num7)));
		}

		public static SexyQuat3 AxisAngle(SexyVector3 inAxis, float inAngleRad)
		{
			SexyQuat3 sexyQuat = new SexyQuat3();
			inAngleRad *= 0.5f;
			sexyQuat.v = inAxis.Normalize();
			sexyQuat.v *= (float)Math.Sin(inAngleRad);
			sexyQuat.s = (float)Math.Cos(inAngleRad);
			return sexyQuat;
		}

		public static SexyQuat3 Slerp(SexyQuat3 inL, SexyQuat3 inR, float inAlpha, bool inLerpOnly)
		{
			if (inL.ApproxEquals(new SexyQuat3(inR), GlobalMembers.SEXYMATH_EPSILONSQ))
			{
				return inL;
			}
			float num = Math.Min(Math.Max(inAlpha, 0f), 1f);
			float num2 = 1f - num;
			SexyQuat3 sexyQuat = new SexyQuat3(new SexyQuat3(inR));
			float num3 = inL.Dot(new SexyQuat3(inR));
			if (num3 < 0f)
			{
				num3 = 0f - num3;
				sexyQuat = new SexyQuat3(-sexyQuat.v, 0f - sexyQuat.s);
			}
			if (1f - num3 > GlobalMembers.SEXYMATH_EPSILON && !inLerpOnly)
			{
				float num4 = (float)Math.Acos(num3);
				if (num4 >= GlobalMembers.SEXYMATH_EPSILON && GlobalMembers.SEXYMATH_PI - num4 >= GlobalMembers.SEXYMATH_EPSILON)
				{
					float num5 = (float)Math.Sin(num4);
					float num6 = 1f / num5;
					float num7 = (float)Math.Sin((double)num2 * (double)num4) * num6;
					float num8 = (float)Math.Sin((double)num * (double)num4) * num6;
					float inS = inL.s * num7 + sexyQuat.s * num8;
					SexyVector3 inV = inL.v * num7 + sexyQuat.v * num8;
					return new SexyQuat3(inV, inS).Normalize();
				}
			}
			float inS2 = inL.s * num2 + sexyQuat.s * num;
			SexyVector3 inV2 = inL.v * num2 + sexyQuat.v * num;
			return new SexyQuat3(inV2, inS2).Normalize();
		}

		public void CopyFrom(SexyQuat3 inQ)
		{
			v = inQ.v;
			s = inQ.s;
		}

		public static SexyQuat3 operator +(SexyQuat3 ImpliedObject, SexyQuat3 inQ)
		{
			return new SexyQuat3(ImpliedObject.v + inQ.v, ImpliedObject.s + inQ.s);
		}

		public static SexyQuat3 operator -(SexyQuat3 ImpliedObject, SexyQuat3 inQ)
		{
			return new SexyQuat3(ImpliedObject.v - inQ.v, ImpliedObject.s - inQ.s);
		}

		public static SexyQuat3 operator *(SexyQuat3 ImpliedObject, SexyQuat3 inQ)
		{
			return new SexyQuat3(inQ.v * ImpliedObject.s + ImpliedObject.v * inQ.s + ImpliedObject.v.Cross(inQ.v), ImpliedObject.s * inQ.s - ImpliedObject.v.Dot(inQ.v));
		}

		public static SexyQuat3 operator *(SexyQuat3 ImpliedObject, float inT)
		{
			return new SexyQuat3(ImpliedObject.v * inT, ImpliedObject.s * inT);
		}

		public static SexyQuat3 operator /(SexyQuat3 ImpliedObject, float inT)
		{
			return new SexyQuat3(ImpliedObject.v / inT, ImpliedObject.s / inT);
		}

		public float Dot(SexyQuat3 inQ)
		{
			return v.Dot(inQ.v) + s * inQ.s;
		}

		public float Magnitude()
		{
			return (float)Math.Sqrt((double)v.x * (double)v.x + (double)(v.y * v.y) + (double)(v.z * v.z) + (double)(s * s));
		}

		public SexyQuat3 Normalize()
		{
			float num = Magnitude();
			if (num == 0f)
			{
				return this;
			}
			return this / num;
		}

		public bool ApproxEquals(SexyQuat3 inQ, float inTol)
		{
			if (SexyMath.ApproxEquals(s, inQ.s, inTol))
			{
				return v.ApproxEquals(inQ.v, inTol);
			}
			return false;
		}
	}
}
