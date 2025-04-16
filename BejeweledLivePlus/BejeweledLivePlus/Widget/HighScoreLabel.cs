using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Widget
{
	public class HighScoreLabel : Label
	{
		public int mMaxScollWidth;

		public HighScoreLabel(Font font, Label_Alignment_Horizontal horizontalAlignment)
			: base(font, horizontalAlignment)
		{
		}

		public override void Update()
		{
			base.Update();
			if (mMaxScollWidth > 0 && mMaxScollWidth < mTextWidth && mUpdateCnt % 15 == 0)
			{
				mDrawOffset.mX--;
				if (mDrawOffset.mX + mTextWidth + 10 < 0)
				{
					mDrawOffset.mX = mMaxScollWidth + 10;
				}
			}
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			g.SetColorizeImages(true);
			for (int i = 0; i < mLayerColours.Count; i++)
			{
				if (mFont == GlobalMembersResources.FONT_DIALOG && mGrayedOut && i == 0)
				{
					Utils.SetFontLayerColor(mFont, i, Bej3WidgetBase.GreyedOutColor);
				}
				else
				{
					Utils.SetFontLayerColor(mFont, i, mLayerColours[i]);
				}
			}
			Color color = default(Color);
			color = ((!mGrayedOut) ? new Color(mColor) : new Color(Bej3WidgetBase.GreyedOutColor));
			if (color.mAlpha == 0)
			{
				return;
			}
			g.SetColor(color);
			g.SetFont(mFont);
			if (!mClippingEnabled)
			{
				g.ClearClipRect();
			}
			if (mUsesCustomClipRect)
			{
				g.SetClipRect((int)((float)mCustomClipRect.mX - g.mTransX), (int)((float)mCustomClipRect.mY - g.mTransY), mCustomClipRect.mWidth, mCustomClipRect.mHeight);
			}
			if (mScale != 1f)
			{
				int num;
				int num2;
				if (!mScaleOverriden)
				{
					num = mDrawOffset.mX;
					num2 = mDrawOffset.mY;
				}
				else
				{
					num = mBasePosition.mX - mX;
					num2 = mBasePosition.mY - mY;
				}
				Utils.PushScale(g, mScale, mScale, num, num2);
			}
			if (mMaxScollWidth > 0)
			{
				g.mClipRect.mWidth = mMaxScollWidth;
			}
			if (mIsTextBlock)
			{
				int theY = 0;
				if (mCenterTextBlockInY)
				{
					theY = mTextBlockOffsetY;
				}
				int theX = 0;
				int num3 = mTextBlock.mWidth;
				int num4 = mTextBlock.mHeight;
				if (mForceSplitHeading)
				{
					theY = -mSplitYOffset;
					num3 /= (int)mScale;
					num4 /= (int)mScale;
				}
				g.WriteWordWrapped(new Rect(theX, theY, num3, num4), mText, mFont.GetLineSpacing() - mLineSpacingOffset, mTextBlockAlignment);
			}
			else
			{
				g.DrawString(mText, mDrawOffset.mX, mDrawOffset.mY - mDescentOffset);
			}
			if (mScale != 1f)
			{
				Utils.PopScale(g);
			}
			g.PopState();
		}
	}
}
