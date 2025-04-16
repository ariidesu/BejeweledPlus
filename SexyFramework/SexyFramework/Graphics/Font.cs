using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public abstract class Font
	{
		public int mAscent;

		public int mAscentPadding;

		public int mHeight;

		public int mLineSpacingOffset;

		public Font()
		{
			mAscent = 0;
			mHeight = 0;
			mAscentPadding = 0;
			mLineSpacingOffset = 0;
		}

		public Font(Font theFont)
		{
			mAscent = theFont.mAscent;
			mHeight = theFont.mHeight;
			mAscentPadding = theFont.mAscentPadding;
			mLineSpacingOffset = theFont.mLineSpacingOffset;
		}

		public virtual void Dispose()
		{
		}

		public virtual ImageFont AsImageFont()
		{
			return null;
		}

		public virtual int GetAscent()
		{
			return mAscent;
		}

		public virtual int GetAscentPadding()
		{
			return mAscentPadding;
		}

		public virtual int GetDescent()
		{
			return mHeight - mAscent;
		}

		public virtual int GetHeight()
		{
			return mHeight;
		}

		public virtual int GetLineSpacingOffset()
		{
			return mLineSpacingOffset;
		}

		public virtual int GetLineSpacing()
		{
			return mHeight + mLineSpacingOffset;
		}

		public virtual int StringWidth(string theString)
		{
			return 0;
		}

		public virtual int CharWidth(char theChar)
		{
			return StringWidth(string.Concat(theChar));
		}

		public virtual int CharWidthKern(char theChar, char thePrevChar)
		{
			return CharWidth(theChar);
		}

		public virtual void DrawString(Graphics g, int theX, int theY, string theString, Color theColor, Rect theClipRect)
		{
		}

		public abstract Font Duplicate();
	}
}
