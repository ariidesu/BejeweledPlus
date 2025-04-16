using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class ColorKey
	{
		protected Color mColor;

		public ColorKey()
		{
		}

		public ColorKey(Color c)
		{
			mColor = c;
		}

		public virtual void Dispose()
		{
		}

		public Color GetInterpolatedColor(ColorKey next_color, float pct)
		{
			Color result = new Color(mColor);
			result.mRed += (int)((float)(next_color.mColor.mRed - mColor.mRed) * pct);
			result.mGreen += (int)((float)(next_color.mColor.mGreen - mColor.mGreen) * pct);
			result.mBlue += (int)((float)(next_color.mColor.mBlue - mColor.mBlue) * pct);
			result.mAlpha += (int)((float)(next_color.mColor.mAlpha - mColor.mAlpha) * pct);
			result.mRed = Math.Max(Math.Min(255, result.mRed), 0);
			result.mGreen = Math.Max(Math.Min(255, result.mGreen), 0);
			result.mBlue = Math.Max(Math.Min(255, result.mBlue), 0);
			result.mAlpha = Math.Max(Math.Min(255, result.mAlpha), 0);
			return result;
		}

		public Color GetColor()
		{
			return mColor;
		}

		public void SetColor(Color c)
		{
			mColor = c;
		}

		public void Serialize(SexyFramework.Misc.Buffer b)
		{
			b.WriteLong(mColor.ToInt());
		}

		public void Deserialize(SexyFramework.Misc.Buffer b)
		{
			mColor = new Color((int)b.ReadLong());
		}
	}
}
