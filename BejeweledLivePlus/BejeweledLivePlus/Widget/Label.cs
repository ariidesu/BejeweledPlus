using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Widget
{
	public class Label : Bej3WidgetBase
	{
		protected string mText = string.Empty;

		protected ImageFont mFont;

		protected Label_Alignment_Horizontal mHorizontalAlignment;

		protected Label_Alignment_Vertical mVerticalAlignment;

		protected new bool mClippingEnabled;

		protected bool mIsTextBlock;

		protected Rect mTextBlock;

		protected int mTextBlockOffsetY;

		protected int mTextBlockAlignment;

		protected bool mCenterTextBlockInY;

		protected Point mBasePosition = default(Point);

		protected Point mDrawOffset = default(Point);

		protected int mDescentOffset;

		protected int mMaxWidth;

		protected bool mScaleOverriden;

		protected float mScale;

		protected int mSplitYOffset;

		protected bool mForceSplitHeading;

		protected int mTextWidth;

		protected Rect mCustomClipRect;

		protected bool mUsesCustomClipRect;

		protected List<Color> mLayerColours = new List<Color>();

		public int mLineSpacingOffset;

		public void Init(Font font, string theText, Label_Alignment_Horizontal horizontalAlignment, Label_Alignment_Vertical verticalAlignment)
		{
			mLineSpacingOffset = 0;
			mScaleOverriden = false;
			mTextBlockAlignment = 0;
			mWidgetFlagsMod.mRemoveFlags |= 8;
			mWidgetFlagsMod.mRemoveFlags |= 16;
			mUsesCustomClipRect = false;
			mTextWidth = 0;
			mForceSplitHeading = false;
			mCenterTextBlockInY = false;
			mClippingEnabled = true;
			mMaxWidth = 0;
			mScale = 1f;
			mIsTextBlock = false;
			mTextBlockOffsetY = 0;
			SetFont(font);
			SetText(theText);
			SetAlignment(horizontalAlignment, verticalAlignment);
		}

		public Label(Font font, string theText, Label_Alignment_Horizontal horizontalAlignment, Label_Alignment_Vertical verticalAlignment)
		{
			Init(font, theText, horizontalAlignment, verticalAlignment);
		}

		public Label(Font font, string theText, Label_Alignment_Horizontal horizontalAlignment)
		{
			Label_Alignment_Vertical verticalAlignment = Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_CENTRE;
			Init(font, theText, horizontalAlignment, verticalAlignment);
		}

		public Label(Font font, string theText)
		{
			Label_Alignment_Horizontal horizontalAlignment = Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE;
			Label_Alignment_Vertical verticalAlignment = Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_CENTRE;
			Init(font, theText, horizontalAlignment, verticalAlignment);
		}

		public Label(Font font, Label_Alignment_Horizontal horizontalAlignment, Label_Alignment_Vertical verticalAlignment)
		{
			Init(font, "", horizontalAlignment, verticalAlignment);
		}

		public Label(Font font, Label_Alignment_Horizontal horizontalAlignment)
		{
			Label_Alignment_Vertical verticalAlignment = Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_CENTRE;
			Init(font, "", horizontalAlignment, verticalAlignment);
		}

		public Label(Font font)
		{
			Label_Alignment_Horizontal horizontalAlignment = Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE;
			Label_Alignment_Vertical verticalAlignment = Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_CENTRE;
			Init(font, "", horizontalAlignment, verticalAlignment);
		}

		public override void Update()
		{
			WidgetUpdate();
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
				g.ClearClipRect();
				g.mClipRect.mX -= 5000;
				g.mClipRect.mWidth += 10000;
				g.mClipRect.mY -= 5000;
				g.mClipRect.mHeight += 10000;
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

		public void SetText(string theText)
		{
			if (mForceSplitHeading)
			{
				mForceSplitHeading = false;
				mIsTextBlock = false;
			}
			mText = theText;
			CalcOffset();
		}

		public void SetAlignment(Label_Alignment_Horizontal horizontalAlignment, Label_Alignment_Vertical verticalAlignment)
		{
			mHorizontalAlignment = horizontalAlignment;
			mVerticalAlignment = verticalAlignment;
			CalcOffset();
		}

		public void SetClippingEnabled(bool clippingEnabled)
		{
			mClippingEnabled = clippingEnabled;
		}

		public void SetFont(Font theFont)
		{
			mFont = (ImageFont)theFont;
			mDescentOffset = mFont.GetHeight() - mFont.GetAscent();
			CalcOffset();
			mLayerColours.Clear();
			for (int i = 0; i < mFont.GetLayerCount(); i++)
			{
				mLayerColours.Add(Color.White);
			}
			if (theFont == GlobalMembersResources.FONT_HUGE)
			{
				mLayerColours[0] = Bej3Widget.COLOR_HEADING_GLOW_1;
			}
			else if (theFont == GlobalMembersResources.FONT_SUBHEADER)
			{
				mLayerColours[1] = Bej3Widget.COLOR_SUBHEADING_1_FILL;
				mLayerColours[0] = Bej3Widget.COLOR_SUBHEADING_1_STROKE;
			}
			else if (theFont == GlobalMembersResources.FONT_DIALOG)
			{
				mLayerColours[0] = Bej3Widget.COLOR_DIALOG_1_FILL;
			}
		}

		public Font GetFont()
		{
			return mFont;
		}

		public void SetColor(Color theColor)
		{
			mColor = new Color(theColor);
		}

		public void CalcOffset()
		{
			if (mIsTextBlock)
			{
				base.Resize(mTextBlock.mX, mTextBlock.mY, mTextBlock.mWidth, mTextBlock.mHeight);
				int visibleHeight = GetVisibleHeight(mTextBlock.mWidth);
				mTextBlockOffsetY = mHeight / 2 - visibleHeight / 2;
				return;
			}
			int num = mFont.StringWidth(mText);
			int num2 = mFont.GetHeight();
			if (!mScaleOverriden)
			{
				mScale = 1f;
				if (mMaxWidth > 0 && num > mMaxWidth)
				{
					mScale = (float)mMaxWidth / (float)num;
					if (mScale < 0.5f)
					{
						mForceSplitHeading = true;
						mScale = 0.5f;
						mIsTextBlock = true;
						SetTextBlock(new Rect(mX, mY, mMaxWidth, 0), true);
					}
					num = mMaxWidth;
					num2 *= (int)mScale;
				}
			}
			mTextWidth = num;
			Point point = default(Point);
			switch (mHorizontalAlignment)
			{
			case Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE:
				point.mX = -num / 2;
				mDrawOffset.mX = 0;
				break;
			case Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT:
				point.mX = 0;
				mDrawOffset.mX = 0;
				break;
			case Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT:
				point.mX = -num;
				mDrawOffset.mX = 0;
				break;
			default:
				point.mX = 0;
				mDrawOffset.mX = 0;
				break;
			}
			switch (mVerticalAlignment)
			{
			case Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_CENTRE:
				point.mY = -num2 / 2;
				mDrawOffset.mY = num2;
				break;
			case Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_TOP:
				point.mY = 0;
				mDrawOffset.mY = num2;
				break;
			case Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_BOTTOM:
				point.mY = 0;
				mDrawOffset.mY = num2;
				break;
			default:
				point.mY = 0;
				mDrawOffset.mY = 0;
				break;
			}
			int theX = mBasePosition.mX + point.mX;
			int theY = mBasePosition.mY + point.mY + mDescentOffset;
			base.Resize(theX, theY, num, num2);
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			mBasePosition.mX = theX;
			mBasePosition.mY = theY;
			if (mForceSplitHeading)
			{
				mTextBlock.mX = (int)((float)theX - (float)mTextBlock.mWidth * mScale / 2f);
				mTextBlock.mY = (int)((float)theY - (float)mTextBlock.mHeight * mScale / 2f);
			}
			CalcOffset();
		}

		public void SetTextBlock(Rect theBlock, bool centerInY)
		{
			mTextBlock = theBlock;
			mCenterTextBlockInY = centerInY;
			CalcOffset();
		}

		public void SetTextBlockEnabled(bool enabled)
		{
			mIsTextBlock = enabled;
			CalcOffset();
		}

		public void SetTextBlockAlignment(int theAlignment)
		{
			mTextBlockAlignment = theAlignment;
		}

		public int GetVisibleHeight(int theWidth)
		{
			if (mIsTextBlock)
			{
				Graphics graphics = new Graphics();
				graphics.SetFont(mFont);
				graphics.SetScale(mScale, mScale, 0f, 0f);
				return GetWordWrappedHeight(graphics, theWidth, mText, mFont.GetLineSpacing() - mLineSpacingOffset);
			}
			return mHeight;
		}

		public int GetVisibleWidth(int theHeight)
		{
			if (mIsTextBlock)
			{
				Graphics graphics = new Graphics();
				graphics.SetFont(mFont);
				graphics.SetScale(mScale, mScale, 0f, 0f);
				int theY = 0;
				if (mCenterTextBlockInY)
				{
					theY = mTextBlockOffsetY;
				}
				int theX = 0;
				int num = mTextBlock.mWidth;
				int num2 = mTextBlock.mHeight;
				if (mForceSplitHeading)
				{
					theY = -mSplitYOffset;
					num /= (int)mScale;
					num2 /= (int)mScale;
				}
				int theMaxWidth = 0;
				graphics.WriteWordWrapped(new Rect(theX, theY, num, num2), mText, mFont.GetLineSpacing() - mLineSpacingOffset, mTextBlockAlignment, ref theMaxWidth);
				return theMaxWidth;
			}
			return mWidth;
		}

		public void SetMaximumWidth(int maxWidth, int splitYOffset)
		{
			mMaxWidth = maxWidth;
			mSplitYOffset = splitYOffset;
			if (mMaxWidth <= 0 && mForceSplitHeading)
			{
				mIsTextBlock = false;
				mForceSplitHeading = false;
			}
			CalcOffset();
		}

		public void SetMaximumWidth(int maxWidth)
		{
			SetMaximumWidth(maxWidth, ConstantsWP.DIALOG_HEADING_LABEL_SPLIT_Y);
		}

		public void SetCustomClipRect(Rect theClipRect)
		{
			mUsesCustomClipRect = true;
			mCustomClipRect = theClipRect;
		}

		public int GetTextWidth()
		{
			return mTextWidth;
		}

		public Rect GetTextBlock()
		{
			return mTextBlock;
		}

		public void SetScale(float scale)
		{
			mScale = scale;
			mScaleOverriden = mScale != 1f;
		}

		public string GetText()
		{
			return mText;
		}

		public void SetLayerColor(int layer, Color colour)
		{
			GlobalMembers.DBG_ASSERT(layer < mLayerColours.Count && layer >= 0);
			mLayerColours[layer] = new Color(colour);
		}
	}
}
