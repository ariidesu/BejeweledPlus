using SexyFramework.Drivers;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class Dialog : Widget, ButtonListener
	{
		public enum ButtonLabel
		{
			BUTTONS_NONE,
			BUTTONS_YES_NO,
			BUTTONS_OK_CANCEL,
			BUTTONS_FOOTER
		}

		public enum ButtonID
		{
			ID_YES = 1000,
			ID_NO = 1001,
			ID_OK = 1000,
			ID_CANCEL = 1001,
			ID_FOOTER = 1000
		}

		public enum ButtonColor
		{
			COLOR_HEADER,
			COLOR_LINES,
			COLOR_FOOTER,
			COLOR_BUTTON_TEXT,
			COLOR_BUTTON_TEXT_HILITE,
			COLOR_BKG,
			COLOR_OUTLINE,
			NUM_COLORS
		}

		public DialogListener mDialogListener;

		public Image mComponentImage;

		public bool mStretchBG;

		public DialogButton mYesButton;

		public DialogButton mNoButton;

		public int mNumButtons;

		public string mDialogHeader;

		public string mDialogFooter;

		public string mDialogLines;

		public int mButtonMode;

		public Font mHeaderFont;

		public Font mLinesFont;

		public int mTextAlign;

		public int mLineSpacingOffset;

		public int mButtonHeight;

		public Insets mBackgroundInsets = new Insets();

		public Insets mContentInsets;

		public int mSpaceAfterHeader;

		public bool mDragging;

		public int mDragMouseX;

		public int mDragMouseY;

		public int mId;

		public bool mIsModal;

		public int mResult;

		public int mButtonHorzSpacing;

		public int mButtonSidePadding;

		public Dialog()
		{
		}

		public Dialog(Image theComponentImage, Image theButtonComponentImage, int theId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
		{
			mId = theId;
			mResult = int.MaxValue;
			mComponentImage = theComponentImage;
			mStretchBG = false;
			mIsModal = isModal;
			mContentInsets = new Insets(24, 24, 24, 24);
			mTextAlign = 0;
			mLineSpacingOffset = 0;
			mSpaceAfterHeader = 10;
			mButtonSidePadding = 0;
			mButtonHorzSpacing = 8;
			mDialogListener = GlobalMembers.gSexyAppBase;
			mDialogHeader = theDialogHeader;
			mDialogFooter = theDialogFooter;
			mButtonMode = theButtonMode;
			SexyAppBase gSexyAppBase = GlobalMembers.gSexyAppBase;
			if (mButtonMode == 1 || mButtonMode == 2)
			{
				mYesButton = new DialogButton(theButtonComponentImage, 1000, this);
				AddWidget(mYesButton);
				mNoButton = new DialogButton(theButtonComponentImage, 1001, this);
				AddWidget(mNoButton);
				mYesButton.SetGamepadParent(this);
				mNoButton.SetGamepadParent(this);
				mYesButton.SetGamepadLinks(null, null, null, mNoButton);
				mNoButton.SetGamepadLinks(null, null, mYesButton, null);
				if (mButtonMode == 1)
				{
					mYesButton.mLabel = gSexyAppBase.GetString("DIALOG_BUTTON_YES", GlobalMembers.DIALOG_YES_STRING);
					mNoButton.mLabel = gSexyAppBase.GetString("DIALOG_BUTTON_NO", GlobalMembers.DIALOG_NO_STRING);
				}
				else
				{
					mYesButton.mLabel = gSexyAppBase.GetString("DIALOG_BUTTON_OK", GlobalMembers.DIALOG_OK_STRING);
					mNoButton.mLabel = gSexyAppBase.GetString("DIALOG_BUTTON_CANCEL", GlobalMembers.DIALOG_CANCEL_STRING);
				}
			}
			else if (mButtonMode == 3)
			{
				mYesButton = new DialogButton(theButtonComponentImage, 1000, this);
				mYesButton.mLabel = mDialogFooter;
				mYesButton.SetGamepadParent(this);
				AddWidget(mYesButton);
				mNoButton = null;
			}
			else
			{
				mYesButton = null;
				mNoButton = null;
				mNumButtons = 0;
			}
			mDialogLines = theDialogLines;
			mButtonHeight = theButtonComponentImage?.GetCelHeight() ?? 24;
			mHasTransparencies = true;
			mHasAlpha = true;
			mHeaderFont = null;
			mLinesFont = null;
			mDragging = false;
			mPriority = 1;
			if (theButtonComponentImage == null)
			{
				GlobalMembers.gDialogColors[3, 0] = 0;
				GlobalMembers.gDialogColors[3, 1] = 0;
				GlobalMembers.gDialogColors[3, 2] = 0;
				GlobalMembers.gDialogColors[4, 0] = 0;
				GlobalMembers.gDialogColors[4, 1] = 0;
				GlobalMembers.gDialogColors[4, 2] = 0;
			}
			else
			{
				GlobalMembers.gDialogColors[3, 0] = 255;
				GlobalMembers.gDialogColors[3, 1] = 255;
				GlobalMembers.gDialogColors[3, 2] = 255;
				GlobalMembers.gDialogColors[4, 0] = 255;
				GlobalMembers.gDialogColors[4, 1] = 255;
				GlobalMembers.gDialogColors[4, 2] = 255;
			}
			SetColors3(GlobalMembers.gDialogColors, 7);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, false);
			if (mHeaderFont != null)
			{
				mHeaderFont.Dispose();
			}
			if (mLinesFont != null)
			{
				mLinesFont.Dispose();
			}
			base.Dispose();
		}

		public virtual void SetButtonFont(Font theFont)
		{
			if (mYesButton != null)
			{
				mYesButton.SetFont(theFont);
			}
			if (mNoButton != null)
			{
				mNoButton.SetFont(theFont);
			}
		}

		public virtual void SetHeaderFont(Font theFont)
		{
			if (mHeaderFont != null)
			{
				mHeaderFont.Dispose();
			}
			mHeaderFont = theFont.Duplicate();
		}

		public virtual void SetLinesFont(Font theFont)
		{
			if (mLinesFont != null)
			{
				mLinesFont.Dispose();
			}
			mLinesFont = theFont.Duplicate();
		}

		public override void SetColor(int theIdx, Color theColor)
		{
			base.SetColor(theIdx, theColor);
			switch (theIdx)
			{
			case 3:
				if (mYesButton != null)
				{
					mYesButton.SetColor(0, theColor);
				}
				if (mNoButton != null)
				{
					mNoButton.SetColor(0, theColor);
				}
				break;
			case 4:
				if (mYesButton != null)
				{
					mYesButton.SetColor(1, theColor);
				}
				if (mNoButton != null)
				{
					mNoButton.SetColor(1, theColor);
				}
				break;
			}
		}

		public virtual int GetPreferredHeight(int theWidth)
		{
			EnsureFonts();
			int num = mContentInsets.mTop + mContentInsets.mBottom + mBackgroundInsets.mTop + mBackgroundInsets.mBottom;
			bool flag = false;
			if (mDialogHeader.Length > 0 && mHeaderFont != null)
			{
				num += mHeaderFont.GetHeight() - mHeaderFont.GetAscentPadding();
				flag = true;
			}
			if (mDialogLines.Length > 0 && mLinesFont != null)
			{
				if (flag)
				{
					num += mSpaceAfterHeader;
				}
				SexyFramework.Graphics.Graphics graphics = new SexyFramework.Graphics.Graphics();
				graphics.SetFont(mLinesFont);
				num += GetWordWrappedHeight(graphics, theWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight - 4, mDialogLines, mLinesFont.GetLineSpacing() + mLineSpacingOffset);
				flag = true;
			}
			if (mDialogFooter.Length != 0 && mButtonMode != 3 && mHeaderFont != null)
			{
				if (flag)
				{
					num += 8;
				}
				num += mHeaderFont.GetLineSpacing();
				flag = true;
			}
			if (mYesButton != null)
			{
				if (flag)
				{
					num += 8;
				}
				num += mButtonHeight + 8;
			}
			return num;
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			EnsureFonts();
			Rect rect = new Rect(mBackgroundInsets.mLeft, mBackgroundInsets.mTop, mWidth - mBackgroundInsets.mLeft - mBackgroundInsets.mRight, mHeight - mBackgroundInsets.mTop - mBackgroundInsets.mBottom);
			if (mComponentImage != null)
			{
				if (!mStretchBG)
				{
					g.DrawImageBox(rect, mComponentImage);
				}
				else
				{
					g.DrawImage(mComponentImage, rect, new Rect(0, 0, mComponentImage.mWidth, mComponentImage.mHeight));
				}
			}
			else
			{
				int theRed = GlobalMembers.gDialogColors[6, 0];
				int theGreen = GlobalMembers.gDialogColors[6, 1];
				int theBlue = GlobalMembers.gDialogColors[6, 1];
				g.SetColor(GetColor(6, new Color(theRed, theGreen, theBlue)));
				g.DrawRect(12, 12, mWidth - 24 - 1, mHeight - 24 - 1);
				int theRed2 = GlobalMembers.gDialogColors[5, 0];
				int theGreen2 = GlobalMembers.gDialogColors[5, 1];
				int theBlue2 = GlobalMembers.gDialogColors[5, 1];
				g.SetColor(GetColor(5, new Color(theRed2, theGreen2, theBlue2)));
				g.FillRect(13, 13, mWidth - 24 - 2, mHeight - 24 - 2);
				g.SetColor(0, 0, 0, 128);
				g.FillRect(mWidth - 12, 24, 12, mHeight - 36);
				g.FillRect(24, mHeight - 12, mWidth - 24, 12);
			}
			int num = mContentInsets.mTop + mBackgroundInsets.mTop;
			if (mDialogHeader.Length > 0)
			{
				num += mHeaderFont.GetAscent() - mHeaderFont.GetAscentPadding();
				g.SetFont(mHeaderFont);
				g.SetColor(mColors[0]);
				WriteCenteredLine(g, num, mDialogHeader);
				num += mHeaderFont.GetHeight() - mHeaderFont.GetAscent();
				num += mSpaceAfterHeader;
			}
			g.SetFont(mLinesFont);
			g.SetColor(mColors[1]);
			Rect theRect = new Rect(mBackgroundInsets.mLeft + mContentInsets.mLeft + 2, num, mWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight - 4, 0);
			num += WriteWordWrapped(g, theRect, mDialogLines, mLinesFont.GetLineSpacing() + mLineSpacingOffset, mTextAlign);
			if (mDialogFooter.Length != 0 && mButtonMode != 3)
			{
				num += 8;
				num += mHeaderFont.GetLineSpacing();
				g.SetFont(mHeaderFont);
				g.SetColor(mColors[2]);
				WriteCenteredLine(g, num, mDialogFooter);
			}
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			if (mYesButton != null && mNoButton != null)
			{
				int num = (mWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight - mButtonSidePadding * 2 - mButtonHorzSpacing) / 2;
				int num2 = mButtonHeight;
				mYesButton.Resize(mBackgroundInsets.mLeft + mContentInsets.mLeft + mButtonSidePadding, mHeight - mContentInsets.mBottom - mBackgroundInsets.mBottom - num2, num, num2);
				mNoButton.Resize(mYesButton.mX + num + mButtonHorzSpacing, mYesButton.mY, num, num2);
			}
			else if (mYesButton != null)
			{
				int num3 = mButtonHeight;
				mYesButton.Resize(mContentInsets.mLeft + mBackgroundInsets.mLeft, mHeight - mContentInsets.mBottom - mBackgroundInsets.mBottom - num3, mWidth - mContentInsets.mLeft - mContentInsets.mRight - mBackgroundInsets.mLeft - mBackgroundInsets.mRight, num3);
			}
		}

		public override void MouseDown(int x, int y, int theClickCount)
		{
			base.MouseDown(x, y, theClickCount);
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
			if (theClickCount == 1)
			{
				mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_DRAGGING);
				mDragging = true;
				mDragMouseX = x;
				mDragMouseY = y;
			}
			base.MouseDown(x, y, theBtnNum, theClickCount);
		}

		public override void MouseDrag(int x, int y)
		{
			if (mDragging)
			{
				int num = mX + x - mDragMouseX;
				int num2 = mY + y - mDragMouseY;
				if (num < -8)
				{
					num = -8;
				}
				else if (num + mWidth > mWidgetManager.mWidth + 8)
				{
					num = mWidgetManager.mWidth - mWidth + 8;
				}
				if (num2 < -8)
				{
					num2 = -8;
				}
				else if (num2 + mHeight > mWidgetManager.mHeight + 8)
				{
					num2 = mWidgetManager.mHeight - mHeight + 8;
				}
				mDragMouseX = mX + x - num;
				mDragMouseY = mY + y - num2;
				if (mDragMouseX < 8)
				{
					mDragMouseX = 8;
				}
				else if (mDragMouseX > mWidth - 9)
				{
					mDragMouseX = mWidth - 9;
				}
				if (mDragMouseY < 8)
				{
					mDragMouseY = 8;
				}
				else if (mDragMouseY > mHeight - 9)
				{
					mDragMouseY = mHeight - 9;
				}
				Move(num, num2);
			}
		}

		public override void MouseUp(int x, int y)
		{
			base.MouseUp(x, y);
		}

		public override void MouseUp(int x, int y, int theClickCount)
		{
			base.MouseUp(x, y, theClickCount);
		}

		public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
		{
			if (mDragging)
			{
				mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_POINTER);
				mDragging = false;
			}
			base.MouseUp(x, y, theBtnNum, theClickCount);
		}

		public override void Update()
		{
			base.Update();
		}

		public virtual bool IsModal()
		{
			return mIsModal;
		}

		public virtual int WaitForResult(bool autoKill)
		{
			if (autoKill)
			{
				GlobalMembers.gSexyAppBase.KillDialog(mId);
			}
			return mResult;
		}

		public virtual void GameAxisMove(GamepadAxis theAxis, int theMovement, int player)
		{
		}

		public virtual void GameButtonDown(GamepadButton theButton, int player, uint flags)
		{
		}

		public virtual void GameButtonUp(GamepadButton theButton, int player, uint flags)
		{
		}

		public override void GotFocus()
		{
			base.GotFocus();
			if (mYesButton != null)
			{
				mWidgetManager.SetGamepadSelection(mYesButton, WidgetLinkDir.LINK_DIR_NONE);
			}
			else if (mNoButton != null)
			{
				mWidgetManager.SetGamepadSelection(mNoButton, WidgetLinkDir.LINK_DIR_NONE);
			}
		}

		public void EnsureFonts()
		{
		}

		public virtual void ButtonPress(int theId)
		{
			mDialogListener.DialogButtonPress(mId, theId);
		}

		public virtual void ButtonDepress(int theId)
		{
			mResult = theId;
			mDialogListener.DialogButtonDepress(mId, theId);
		}

		public void ButtonPress(int theId, int theClickCount)
		{
			ButtonPress(theId);
		}

		public void ButtonDownTick(int theId)
		{
		}

		public void ButtonMouseEnter(int theId)
		{
		}

		public void ButtonMouseLeave(int theId)
		{
		}

		public void ButtonMouseMove(int theId, int theX, int theY)
		{
		}
	}
}
