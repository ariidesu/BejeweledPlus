using System;

namespace SexyFramework.Graphics
{
	public struct Color
	{
		public int mRed;

		public int mGreen;

		public int mBlue;

		public int mAlpha;

		public static Color Zero = new Color(0, 0, 0, 0);

		public static readonly Color Black = new Color(0, 0, 0);

		public static readonly Color White = new Color(255, 255, 255);

		public static readonly Color Red = new Color(255, 0, 0);

		public static readonly Color Green = new Color(0, 255, 0);

		public static readonly Color Blue = new Color(0, 0, 255);

		public static readonly Color Yellow = new Color(255, 255, 0);

		public Color(Color color)
		{
			mRed = color.mRed;
			mGreen = color.mGreen;
			mBlue = color.mBlue;
			mAlpha = color.mAlpha;
		}

		public Color(int theColor)
		{
			mAlpha = (theColor >> 24) & 0xFF;
			mRed = (theColor >> 16) & 0xFF;
			mGreen = (theColor >> 8) & 0xFF;
			mBlue = theColor & 0xFF;
			if (mAlpha == 0)
			{
				mAlpha = 255;
			}
		}

		public Color(uint theColor)
		{
			mAlpha = (int)((theColor >> 24) & 0xFF);
			mRed = (int)((theColor >> 16) & 0xFF);
			mGreen = (int)((theColor >> 8) & 0xFF);
			mBlue = (int)(theColor & 0xFF);
			if (mAlpha == 0)
			{
				mAlpha = 255;
			}
		}

		public Color(int theColor, int theAlpha)
		{
			mRed = (theColor >> 16) & 0xFF;
			mGreen = (theColor >> 8) & 0xFF;
			mBlue = theColor & 0xFF;
			mAlpha = theAlpha;
		}

		public Color(int theRed, int theGreen, int theBlue)
		{
			mRed = theRed;
			mGreen = theGreen;
			mBlue = theBlue;
			mAlpha = 255;
		}

		public Color(int theRed, int theGreen, int theBlue, int theAlpha)
		{
			mRed = theRed;
			mGreen = theGreen;
			mBlue = theBlue;
			mAlpha = theAlpha;
		}

		public Color(SexyRGBA theColor)
		{
			mRed = theColor.r;
			mGreen = theColor.g;
			mBlue = theColor.b;
			mAlpha = theColor.a;
		}

		public Color(int[] theElements)
		{
			mRed = theElements[0];
			mGreen = theElements[1];
			mBlue = theElements[2];
			mAlpha = 255;
		}

		public override string ToString()
		{
			return "(" + mRed + "," + mGreen + "," + mBlue + "," + mAlpha + ")";
		}

		public Color Clone()
		{
			return new Color(this);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Color)
			{
				Color color = (Color)obj;
				if (mRed == color.mRed && mBlue == color.mBlue && mGreen == color.mGreen)
				{
					return mAlpha == color.mAlpha;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int ToInt()
		{
			return (mAlpha << 24) | (mRed << 16) | (mGreen << 8) | mBlue;
		}

		public SexyRGBA ToRGBA()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(Color theColor1, Color theColor2)
		{
			if (theColor1.mRed == theColor2.mRed && theColor1.mGreen == theColor2.mGreen && theColor1.mBlue == theColor2.mBlue)
			{
				return theColor1.mAlpha == theColor2.mAlpha;
			}
			return false;
		}

		public static bool operator !=(Color theColor1, Color theColor2)
		{
			return !(theColor1 == theColor2);
		}

		public static Color operator *(Color theColor1, Color theColor2)
		{
			return new Color(theColor1.mRed * theColor2.mRed / 255, theColor1.mGreen * theColor2.mGreen / 255, theColor1.mBlue * theColor2.mBlue / 255, theColor1.mAlpha * theColor2.mAlpha / 255);
		}

		public static Color operator *(Color theColor1, float theAlphaPct)
		{
			return new Color(theColor1.mRed, theColor1.mGreen, theColor1.mBlue, (int)((float)theColor1.mAlpha * theAlphaPct) / 255);
		}

		public static Color FAlpha(float theAlpha)
		{
			return new Color(255, 255, 255, (int)(255f * theAlpha));
		}

		public void SetColor(int p, int p_2, int p_3, int p_4)
		{
			mRed = p;
			mGreen = p_2;
			mBlue = p_3;
			mAlpha = p_4;
		}
	}
}
