using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class DialogButton : ButtonWidget
	{
		public Image mComponentImage;

		public int mTranslateX;

		public int mTranslateY;

		public int mTextOffsetX;

		public int mTextOffsetY;

		private Rect mBoundBox = default(Rect);

		public DialogButton(Image theComponentImage, int theId, ButtonListener theListener)
			: base(theId, theListener)
		{
			mComponentImage = theComponentImage;
			if (mComponentImage != null && mComponentImage.GetCelCount() == 3)
			{
				mNormalRect = mComponentImage.GetCelRect(0);
				mOverRect = mComponentImage.GetCelRect(1);
				mDownRect = mComponentImage.GetCelRect(2);
			}
			mTextOffsetX = (mTextOffsetY = 0);
			mTranslateX = (mTranslateY = 1);
			mDoFinger = true;
			SetColors3(GlobalMembers.gDialogButtonColors, 6);
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			if (mBtnNoDraw)
			{
				return;
			}
			if (mComponentImage == null)
			{
				base.Draw(g);
				return;
			}
			bool flag = IsButtonDown();
			mBoundBox.mX = 0;
			mBoundBox.mY = 0;
			mBoundBox.mWidth = mWidth;
			mBoundBox.mHeight = mHeight;
			if (mNormalRect.mWidth == 0)
			{
				if (flag)
				{
					g.Translate(mTranslateX, mTranslateY);
				}
				g.DrawImageBox(mBoundBox, mComponentImage);
			}
			else
			{
				if (mDisabled && mDisabledRect.mWidth > 0 && mDisabledRect.mHeight > 0)
				{
					g.DrawImageBox(mDisabledRect, mBoundBox, mComponentImage);
				}
				else if (IsButtonDown())
				{
					g.DrawImageBox(mDownRect, mBoundBox, mComponentImage);
				}
				else if (mOverAlpha > 0.0)
				{
					if (mOverAlpha < 1.0)
					{
						g.DrawImageBox(mNormalRect, mBoundBox, mComponentImage);
					}
					g.SetColorizeImages(true);
					g.SetColor(255, 255, 255, (int)(mOverAlpha * 255.0));
					g.DrawImageBox(mOverRect, mBoundBox, mComponentImage);
					g.SetColorizeImages(false);
				}
				else if (mIsOver)
				{
					g.DrawImageBox(mOverRect, mBoundBox, mComponentImage);
				}
				else
				{
					g.DrawImageBox(mNormalRect, mBoundBox, mComponentImage);
				}
				if (flag)
				{
					g.Translate(mTranslateX, mTranslateY);
				}
			}
			if (mFont != null)
			{
				g.SetFont(mFont);
				if (mIsOver)
				{
					g.SetColor(mColors[1]);
				}
				else
				{
					g.SetColor(mColors[0]);
				}
				int num = (mWidth - mFont.StringWidth(mLabel)) / 2;
				int num2 = (mHeight + mFont.GetAscent() - mFont.GetAscentPadding() - mFont.GetAscent() / 6 - 1) / 2;
				g.DrawString(mLabel, num + mTextOffsetX, num2 + mTextOffsetY);
			}
			if (mIconImage != null)
			{
				if (mIsOver)
				{
					g.SetColor(mColors[1]);
				}
				else
				{
					g.SetColor(mColors[0]);
				}
				int num3 = (mWidth - mIconImage.GetWidth()) / 2;
				int num4 = (mHeight - mIconImage.GetHeight()) / 2;
				g.DrawImage(mIconImage, num3 + mTextOffsetX, num4 + mTextOffsetY);
			}
			if (flag)
			{
				g.Translate(-mTranslateX, -mTranslateY);
			}
		}
	}
}
