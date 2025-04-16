using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class RotatingSeg : SexyFramework.Widget.Widget
	{
		public char mChar;

		public int mNumRevs;

		public new Rect mClip = default(Rect);

		public Font mFont;

		public bool mDoMax;

		public int mIndex;

		public RotatingSeg()
		{
			mChar = '0';
			mNumRevs = 0;
			mClip = default(Rect);
			mFont = null;
			mDoMax = false;
			mIndex = 0;
		}

		public RotatingSeg(char theChar, int numRevs, int index, bool doMax, Rect clip, Font font)
		{
			mChar = theChar;
			mNumRevs = numRevs;
			mClip = clip;
			mFont = font;
			mDoMax = doMax;
			mIndex = index;
			mY = mClip.mY + (mClip.mHeight + mFont.GetAscent()) / 2;
		}

		public virtual void Draw(Graphics g, float yOffs)
		{
			Rect clipRect = mClip;
			clipRect.Offset(0, (int)yOffs);
			g.SetClipRect(clipRect);
			g.DrawString(mChar.ToString(), mClip.mX + (mClip.mWidth - mFont.CharWidth(mChar)) / 2, (int)((float)(mY - ConstantsWP.ROTATING_COUNTER_OFFSET_Y) + yOffs));
			if ((float)(mY - ConstantsWP.ROTATING_COUNTER_OFFSET_Y) + yOffs + (float)mClip.mHeight - (float)mFont.GetAscent() < (float)(mClip.mY + mClip.mHeight))
			{
				char nextChar = GetNextChar();
				g.DrawString(nextChar.ToString(), mClip.mX + (mClip.mWidth - mFont.CharWidth(nextChar)) / 2, mY - (int)((float)ConstantsWP.ROTATING_COUNTER_OFFSET_Y + yOffs + (float)mClip.mHeight));
			}
		}

		public char GetNextChar()
		{
			char result = '\0';
			if (mDoMax && mNumRevs == 1)
			{
				switch (mIndex)
				{
				case 0:
					result = 'M';
					break;
				case 1:
					result = 'A';
					break;
				case 2:
					result = 'X';
					break;
				}
			}
			else if (mChar == '9')
			{
				result = '0';
			}
			else
			{
				switch (mChar)
				{
				case '0':
					result = '1';
					break;
				case '1':
					result = '2';
					break;
				case '2':
					result = '3';
					break;
				case '3':
					result = '4';
					break;
				case '4':
					result = '5';
					break;
				case '5':
					result = '6';
					break;
				case '6':
					result = '7';
					break;
				case '7':
					result = '8';
					break;
				case '8':
					result = '9';
					break;
				default:
					result = '0';
					break;
				}
			}
			return result;
		}
	}
}
