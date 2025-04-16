using System;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public static class GlobalPIEffect
	{
		public static float M_PI = 3.14159f;

		public static int PI_BUFSIZE = 1024;

		public static int PI_QUANT_SIZE = 256;

		internal static IntPtr GetData(PIEffect theEffect, IntPtr thePtr, int theSize)
		{
			return thePtr;
		}

		public static bool IsIdentityMatrix(SexyTransform2D theMatrix)
		{
			if (theMatrix.m01 == 0f && theMatrix.m02 == 0f && theMatrix.m10 == 0f && theMatrix.m12 == 0f && theMatrix.m20 == 0f && theMatrix.m21 == 0f && theMatrix.m00 == 1f && theMatrix.m11 == 1f)
			{
				return theMatrix.m22 == 1f;
			}
			return false;
		}

		public static float GetMatrixScale(SexyTransform2D theMatrix)
		{
			return 0f;
		}

		public static Vector2 TransformFPoint(SexyTransform2D theMatrix, Vector2 thePoint)
		{
			return Vector2.Transform(thePoint, theMatrix.mMatrix);
		}

		internal static float WrapFloat(float theNum, int theRepeat)
		{
			if (theRepeat == 1)
			{
				return theNum;
			}
			theNum *= (float)theRepeat;
			return theNum - (float)(int)theNum;
		}

		public static float DegToRad(float theDeg)
		{
			return theDeg * M_PI / 180f;
		}

		public static uint InterpColor(int theColor1, int theColor2, float thePct)
		{
			uint num = (uint)(thePct * 255f);
			num = ((num >= 0) ? ((num > 255) ? 255u : num) : 0u);
			int num2 = (int)(255 - num);
			long num3 = ((((uint)(theColor1 & -16777216) >> 24) * num2 + ((uint)(theColor2 & -16777216) >> 24) * num << 16) & 0xFF000000u) | ((((uint)(theColor1 & 0xFF0000) >> 16) * num2 + ((theColor2 & 0xFF0000) >> 16) * num << 8) & 0xFF0000) | ((((theColor1 & 0xFF00) >> 8) * num2 + ((theColor2 & 0xFF00) >> 8) * num) & 0xFF00) | (((theColor1 & 0xFF) * num2 + (theColor2 & 0xFF) * num >> 8) & 0xFF);
			return (uint)num3;
		}

		public static bool LineSegmentIntersects(Vector2 aPtA1, Vector2 aPtA2, Vector2 aPtB1, Vector2 aPtB2, ref float thePos, Vector2 theIntersectionPoint)
		{
			double num = (aPtB2.X - aPtB1.Y) * (aPtA2.X - aPtA1.X) - (aPtB2.X - aPtB1.X) * (aPtA2.Y - aPtA1.Y);
			if (num != 0.0)
			{
				double num2 = (double)((aPtB2.X - aPtB1.X) * (aPtA1.Y - aPtB1.Y) - (aPtB2.Y - aPtB1.Y) * (aPtA1.X - aPtB1.X)) / num;
				if (num2 >= 0.0 && num2 <= 1.0)
				{
					double num3 = (double)((aPtA2.X - aPtA1.X) * (aPtA1.Y - aPtB1.Y) - (aPtA2.Y - aPtA1.Y) * (aPtA1.X - aPtB1.Y)) / num;
					if (num3 >= 0.0 && num3 <= 1.0)
					{
						if (thePos != 0f)
						{
							thePos = (float)num2;
						}
						theIntersectionPoint = aPtA1 + (aPtA2 - aPtA1) * (float)num2;
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}

		internal static void GetBestStripSize(int theCount, int theCelWidth, int theCelHeight, ref int theNumCols, ref int theNumRows)
		{
			float num = 100f;
			theNumCols = theCount;
			theNumRows = 1;
			for (int i = 1; i <= theCount; i++)
			{
				int num2 = theCount / i;
				if (num2 * i == theCount)
				{
					float num3 = (float)(theCelWidth * num2) / (float)(theCelHeight * i);
					float num4 = Math.Max(num3, 1f / num3);
					if (num4 + 0.0001f < num)
					{
						theNumRows = i;
						theNumCols = num2;
						num = num4;
					}
				}
			}
		}

		public static float TIME_TO_X(float theTime, float aMinTime, float aMaxTime)
		{
			return (float)((double)((theTime - aMinTime) / (aMaxTime - aMinTime) * (float)(PI_QUANT_SIZE - 1)) + 0.5);
		}
	}
}
