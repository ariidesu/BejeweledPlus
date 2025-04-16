using System;
using SexyFramework.Drivers;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class ButtonWidget : Widget
	{
		public enum ButtonLabel
		{
			BUTTON_LABEL_LEFT = -1,
			BUTTON_LABEL_CENTER,
			BUTTON_LABEL_RIGHT
		}

		public enum ButtonColor
		{
			COLOR_LABEL,
			COLOR_LABEL_HILITE,
			COLOR_DARK_OUTLINE,
			COLOR_LIGHT_OUTLINE,
			COLOR_MEDIUM_OUTLINE,
			COLOR_BKG,
			NUM_COLORS
		}

		public int mId;

		public string mLabel;

		public int mLabelJustify;

		public Font mFont;

		public Image mButtonImage;

		public Image mIconImage;

		public Image mOverImage;

		public Image mDownImage;

		public Image mDisabledImage;

		public Rect mNormalRect = default(Rect);

		public Rect mOverRect = default(Rect);

		public Rect mDownRect = default(Rect);

		public Rect mDisabledRect = default(Rect);

		public bool mInverted;

		public bool mBtnNoDraw;

		public bool mFrameNoDraw;

		public ButtonListener mButtonListener;

		public int mLastPressedBy;

		public double mOverAlpha;

		public double mOverAlphaSpeed;

		public double mOverAlphaFadeInSpeed;

		public int mLabelOffsetX;

		public int mLabelOffsetY;

		public ButtonWidget(int theId, ButtonListener theButtonListener)
		{
			mId = theId;
			mFont = null;
			mLabelJustify = 0;
			mButtonImage = null;
			mIconImage = null;
			mOverImage = null;
			mDownImage = null;
			mDisabledImage = null;
			mInverted = false;
			mBtnNoDraw = false;
			mFrameNoDraw = false;
			mButtonListener = theButtonListener;
			mHasAlpha = true;
			mOverAlpha = 0.0;
			mOverAlphaSpeed = 0.0;
			mOverAlphaFadeInSpeed = 0.0;
			mLabelOffsetX = (mLabelOffsetY = 0);
			SetColors3(GlobalMembers.gButtonWidgetColors, 6);
			mLastPressedBy = -1;
		}

		public override void Dispose()
		{
			if (mFont != null)
			{
				mFont.Dispose();
			}
			base.Dispose();
		}

		public virtual void SetFont(Font theFont)
		{
			if (mFont != null)
			{
				mFont.Dispose();
			}
			mFont = theFont.Duplicate();
		}

		public virtual bool IsButtonDown()
		{
			if (mIsDown && mIsOver)
			{
				return !mDisabled;
			}
			return false;
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			if (mBtnNoDraw)
			{
				return;
			}
			bool flag = IsButtonDown();
			flag ^= mInverted;
			int num = mLabelOffsetX;
			int num2 = mLabelOffsetY;
			if (mFont != null)
			{
				if (mLabelJustify == 0)
				{
					num += (mWidth - mFont.StringWidth(mLabel)) / 2;
				}
				else if (mLabelJustify == 1)
				{
					num += mWidth - mFont.StringWidth(mLabel);
				}
				num2 += (mHeight + mFont.GetAscent() - mFont.GetAscent() / 6 - 1) / 2;
			}
			int theX = 0;
			int theY = 0;
			if (mIconImage != null)
			{
				if (mLabelJustify == 0)
				{
					theX = (mWidth - mIconImage.GetWidth()) / 2 + mLabelOffsetX;
				}
				else if (mLabelJustify == 1)
				{
					theX = mWidth - mIconImage.GetWidth();
				}
				theY = (mHeight - mIconImage.GetHeight()) / 2 + mLabelOffsetY;
			}
			g.SetFont(mFont);
			if (mButtonImage == null && mDownImage == null)
			{
				if (!mFrameNoDraw)
				{
					g.SetColor(mColors[5]);
					g.FillRect(0, 0, mWidth, mHeight);
				}
				if (flag)
				{
					if (!mFrameNoDraw)
					{
						g.SetColor(mColors[2]);
						g.FillRect(0, 0, mWidth - 1, 1);
						g.FillRect(0, 0, 1, mHeight - 1);
						g.SetColor(mColors[3]);
						g.FillRect(0, mHeight - 1, mWidth, 1);
						g.FillRect(mWidth - 1, 0, 1, mHeight);
						g.SetColor(mColors[4]);
						g.FillRect(1, 1, mWidth - 3, 1);
						g.FillRect(1, 1, 1, mHeight - 3);
					}
					if (mIsOver)
					{
						g.SetColor(mColors[1]);
					}
					else
					{
						g.SetColor(mColors[0]);
					}
					if (mIconImage == null)
					{
						g.DrawString(mLabel, num, num2);
					}
					else
					{
						g.DrawImage(mIconImage, theX, theY);
					}
				}
				else
				{
					if (!mFrameNoDraw)
					{
						g.SetColor(mColors[3]);
						g.FillRect(0, 0, mWidth - 1, 1);
						g.FillRect(0, 0, 1, mHeight - 1);
						g.SetColor(mColors[2]);
						g.FillRect(0, mHeight - 1, mWidth, 1);
						g.FillRect(mWidth - 1, 0, 1, mHeight);
						g.SetColor(mColors[4]);
						g.FillRect(1, mHeight - 2, mWidth - 2, 1);
						g.FillRect(mWidth - 2, 1, 1, mHeight - 2);
					}
					if (mIsOver)
					{
						g.SetColor(mColors[1]);
					}
					else
					{
						g.SetColor(mColors[0]);
					}
					if (mIconImage == null)
					{
						g.DrawString(mLabel, num, num2);
					}
					else
					{
						g.DrawImage(mIconImage, theX, theY);
					}
				}
			}
			else if (!flag)
			{
				if (mDisabled && HaveButtonImage(mDisabledImage, mDisabledRect))
				{
					DrawButtonImage(g, mDisabledImage, mDisabledRect, 0, 0);
				}
				else if (mOverAlpha > 0.0 && HaveButtonImage(mOverImage, mOverRect))
				{
					if (HaveButtonImage(mButtonImage, mNormalRect) && mOverAlpha < 1.0)
					{
						DrawButtonImage(g, mButtonImage, mNormalRect, 0, 0);
					}
					g.SetColorizeImages(true);
					g.SetColor(new Color(255, 255, 255, (int)(mOverAlpha * 255.0)));
					DrawButtonImage(g, mOverImage, mOverRect, 0, 0);
					g.SetColorizeImages(false);
				}
				else if ((mIsOver || mIsDown) && HaveButtonImage(mOverImage, mOverRect))
				{
					DrawButtonImage(g, mOverImage, mOverRect, 0, 0);
				}
				else if (HaveButtonImage(mButtonImage, mNormalRect))
				{
					DrawButtonImage(g, mButtonImage, mNormalRect, 0, 0);
				}
				if (mIsOver)
				{
					g.SetColor(mColors[1]);
				}
				else
				{
					g.SetColor(mColors[0]);
				}
				if (mIconImage == null)
				{
					g.DrawString(mLabel, num, num2);
				}
				else
				{
					g.DrawImage(mIconImage, theX, theY);
				}
			}
			else
			{
				if (HaveButtonImage(mDownImage, mDownRect))
				{
					DrawButtonImage(g, mDownImage, mDownRect, 0, 0);
				}
				else if (HaveButtonImage(mOverImage, mOverRect))
				{
					DrawButtonImage(g, mOverImage, mOverRect, 0, 0);
				}
				else
				{
					DrawButtonImage(g, mButtonImage, mNormalRect, 0, 0);
				}
				g.SetColor(mColors[1]);
				if (mIconImage == null)
				{
					g.DrawString(mLabel, num, num2);
				}
				else
				{
					g.DrawImage(mIconImage, theX, theY);
				}
			}
		}

		public override void SetDisabled(bool isDisabled)
		{
			base.SetDisabled(isDisabled);
			if (HaveButtonImage(mDisabledImage, mDisabledRect))
			{
				MarkDirty();
			}
		}

		public override void MouseEnter()
		{
			base.MouseEnter();
			if (mOverAlphaFadeInSpeed == 0.0 && mOverAlpha > 0.0)
			{
				mOverAlpha = 0.0;
			}
			if (mIsDown || HaveButtonImage(mOverImage, mOverRect) || mColors[1] != mColors[0])
			{
				MarkDirty();
			}
			MarkDirty();
			mButtonListener.ButtonMouseEnter(mId);
		}

		public override void MouseLeave()
		{
			base.MouseLeave();
			if (mOverAlphaSpeed == 0.0 && mOverAlpha > 0.0)
			{
				mOverAlpha = 0.0;
			}
			else if (mOverAlphaSpeed > 0.0 && mOverAlpha == 0.0 && mWidgetManager.mApp.mHasFocus)
			{
				mOverAlpha = Math.Min(1.0, mOverAlphaSpeed * 10.0);
			}
			if (mIsDown || HaveButtonImage(mOverImage, mOverRect) || mColors[1] != mColors[0])
			{
				MarkDirty();
			}
			mButtonListener.ButtonMouseLeave(mId);
		}

		public override void MouseMove(int theX, int theY)
		{
			base.MouseMove(theX, theY);
			mButtonListener.ButtonMouseMove(mId, theX, theY);
		}

		public override void MouseDown(int theX, int theY, int theClickCount)
		{
			base.MouseDown(theX, theY, theClickCount);
		}

		public override void MouseDown(int theX, int theY, int theBtnNum, int theClickCount)
		{
			base.MouseDown(theX, theY, theBtnNum, theClickCount);
			mButtonListener.ButtonPress(mId, theClickCount);
			MarkDirty();
		}

		public override void MouseUp(int theX, int theY)
		{
			base.MouseUp(theX, theY);
		}

		public override void MouseUp(int theX, int theY, int theClickCount)
		{
			base.MouseUp(theX, theY, theClickCount);
		}

		public override void MouseUp(int theX, int theY, int theBtnNum, int theClickCount)
		{
			base.MouseUp(theX, theY, theBtnNum, theClickCount);
			if (mIsOver && mWidgetManager.mHasFocus)
			{
				mButtonListener.ButtonDepress(mId);
			}
			MarkDirty();
		}

		public override void Update()
		{
			base.Update();
			if (mIsDown && mIsOver)
			{
				mButtonListener.ButtonDownTick(mId);
			}
			if (!mIsDown && !mIsOver && mOverAlpha > 0.0)
			{
				if (mOverAlphaSpeed > 0.0)
				{
					mOverAlpha -= mOverAlphaSpeed;
					if (mOverAlpha < 0.0)
					{
						mOverAlpha = 0.0;
					}
				}
				else
				{
					mOverAlpha = 0.0;
				}
				MarkDirty();
			}
			else if (mIsOver && mOverAlphaFadeInSpeed > 0.0 && mOverAlpha < 1.0)
			{
				mOverAlpha += mOverAlphaFadeInSpeed;
				if (mOverAlpha > 1.0)
				{
					mOverAlpha = 1.0;
				}
				MarkDirty();
			}
		}

		public override void GotGamepadSelection(WidgetLinkDir theDirection)
		{
			base.GotGamepadSelection(theDirection);
			mIsOver = true;
		}

		public override void LostGamepadSelection()
		{
			base.LostGamepadSelection();
			mIsOver = false;
			mIsDown = false;
		}

		public override void GamepadButtonDown(GamepadButton theButton, int thePlayer, uint theFlags)
		{
			if (theButton == GamepadButton.GAMEPAD_BUTTON_A)
			{
				if ((theFlags & 1) == 0)
				{
					mLastPressedBy = thePlayer;
					OnPressed();
					mIsDown = true;
					if (mButtonListener != null)
					{
						mButtonListener.ButtonPress(mId, 1);
					}
					MarkDirty();
				}
			}
			else if (mIsDown)
			{
				if (mGamepadParent != null)
				{
					mGamepadParent.GamepadButtonDown(theButton, thePlayer, theFlags);
				}
			}
			else
			{
				base.GamepadButtonDown(theButton, thePlayer, theFlags);
			}
		}

		public override void GamepadButtonUp(GamepadButton theButton, int thePlayer, uint theFlags)
		{
			if (theButton == GamepadButton.GAMEPAD_BUTTON_A)
			{
				if (mIsDown)
				{
					mLastPressedBy = thePlayer;
					if (mButtonListener != null)
					{
						mButtonListener.ButtonDepress(mId);
					}
					mIsDown = false;
					MarkDirty();
				}
			}
			else
			{
				base.GamepadButtonUp(theButton, thePlayer, theFlags);
			}
		}

		public virtual void OnPressed()
		{
		}

		public virtual bool HaveButtonImage(Image theImage, Rect theRect)
		{
			if (theImage == null)
			{
				return theRect.mWidth != 0;
			}
			return true;
		}

		public virtual void DrawButtonImage(SexyFramework.Graphics.Graphics g, Image theImage, Rect theRect, int x, int y)
		{
			if (theRect.mWidth != 0)
			{
				g.DrawImage(theImage, x, y, theRect);
			}
			else
			{
				g.DrawImage(theImage, x, y);
			}
		}
	}
}
