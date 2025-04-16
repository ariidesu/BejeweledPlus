using System;
using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class EditWidget : Widget
	{
		public enum COLOR
		{
			COLOR_BKG,
			COLOR_OUTLINE,
			COLOR_TEXT,
			COLOR_HILITE,
			COLOR_HILITE_TEXT,
			NUM_COLORS
		}

		public struct WidthCheck
		{
			public Font mFont;

			public int mWidth;
		}

		public int mId;

		public string mString;

		public string mPasswordDisplayString = "";

		public Font mFont;

		public int mClipInset;

		public int mTextInset;

		public int mCursorOffset;

		public int mHiliteWidthAdd;

		public List<WidthCheck> mWidthCheckList = new List<WidthCheck>();

		internal static int[,] gEditWidgetColors = new int[5, 3]
		{
			{ 255, 255, 255 },
			{ 0, 0, 0 },
			{ 0, 0, 0 },
			{ 0, 0, 0 },
			{ 255, 255, 255 }
		};

		public EditListener mEditListener;

		public bool mShowingCursor;

		public bool mDrawSelOverride;

		public bool mHadDoubleClick;

		public int mCursorPos;

		public int mHilitePos;

		public int mBlinkAcc;

		public int mBlinkDelay;

		public int mLeftPos;

		public int mMaxChars;

		public int mMaxPixels;

		public string mPasswordChar;

		public string mUndoString;

		public int mUndoCursor;

		public int mUndoHilitePos;

		public int mLastModifyIdx;

		public EditWidget(int theId, EditListener theEditListener)
		{
			mId = theId;
			mEditListener = theEditListener;
			mFont = null;
			mHadDoubleClick = false;
			mHilitePos = -1;
			mLastModifyIdx = -1;
			mLeftPos = 0;
			mUndoCursor = 0;
			mUndoHilitePos = 0;
			mLastModifyIdx = 0;
			mBlinkAcc = 0;
			mCursorPos = 0;
			mShowingCursor = false;
			mDrawSelOverride = false;
			mMaxChars = -1;
			mMaxPixels = -1;
			mPasswordChar = null;
			mBlinkDelay = 40;
			mClipInset = 4;
			mTextInset = 4;
			mCursorOffset = 0;
			mHiliteWidthAdd = 0;
			SetColors3(gEditWidgetColors, 5);
		}

		public void ClearWidthCheckFonts()
		{
			mWidthCheckList.Clear();
		}

		public void AddWidthCheckFont(Font theFont, int theMaxPixels)
		{
			WidthCheck item = default(WidthCheck);
			item.mWidth = theMaxPixels;
			item.mFont = theFont.Duplicate();
			mWidthCheckList.Add(item);
		}

		public void AddWidthCheckFont(Font theFont)
		{
			int num = -1;
			WidthCheck item = default(WidthCheck);
			item.mWidth = num;
			item.mFont = theFont.Duplicate();
			mWidthCheckList.Add(item);
		}

		public virtual void SetText(string theText, bool leftPosToZero)
		{
			mString = theText;
			mCursorPos = mString.Length;
			mHilitePos = 0;
			if (leftPosToZero)
			{
				mLeftPos = 0;
			}
			else
			{
				FocusCursor(true);
			}
			MarkDirty();
		}

		public virtual void SetText(string theText)
		{
			SetText(theText, true);
		}

		protected string GetDisplayString()
		{
			if (mPasswordChar == " ")
			{
				return mString;
			}
			if (mPasswordDisplayString.Length != mString.Length)
			{
				mPasswordDisplayString = mString;
			}
			return mPasswordDisplayString;
		}

		public override bool WantsFocus()
		{
			return true;
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			FocusCursor(false);
		}

		public virtual void SetFont(Font theFont, Font theWidthCheckFont)
		{
			mFont = theFont.Duplicate();
			ClearWidthCheckFonts();
			if (theWidthCheckFont != null)
			{
				AddWidthCheckFont(theWidthCheckFont, -1);
			}
		}

		public virtual void SetFont(Font theFont)
		{
			mFont = theFont.Duplicate();
			Font font = null;
			ClearWidthCheckFonts();
			if (font != null)
			{
				AddWidthCheckFont(font, -1);
			}
		}

		public int MAX(int x, int y)
		{
			return (x > y) ? x : y;
		}

		public int MIN(int x, int y)
		{
			return (x < y) ? x : y;
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			string displayString = GetDisplayString();
			g.SetColor(mColors[0]);
			g.FillRect(0, 0, mWidth, mHeight);
			for (int i = 0; i < 2; i++)
			{
				g.PushState();
				g.SetFont(mFont);
				if (i == 1)
				{
					int num = mFont.StringWidth(displayString.Substring(0, mCursorPos)) - mFont.StringWidth(displayString.Substring(0, mLeftPos)) + mTextInset;
					int y = num + 2;
					if (mHilitePos != -1 && mCursorPos != mHilitePos)
					{
						y = mFont.StringWidth(displayString.Substring(0, mHilitePos)) - mFont.StringWidth(displayString.Substring(0, mLeftPos)) + mTextInset;
					}
					if (!mShowingCursor)
					{
						num += 2;
					}
					num = MIN(MAX(0, num), mWidth - 8);
					y = MIN(MAX(0, y), mWidth - 8);
					int num2 = 0;
					if (mHilitePos != -1 && mHilitePos != mCursorPos)
					{
						num2 = mHiliteWidthAdd;
					}
					g.ClipRect(mClipInset + MIN(num, y) + mCursorOffset, (mHeight - mFont.GetHeight()) / 2, Math.Abs(y - num) + num2, mFont.GetHeight());
				}
				else
				{
					g.ClipRect(mClipInset, 0, mWidth - mClipInset * 2, mHeight);
				}
				bool flag = mHasFocus || mDrawSelOverride;
				if (i == 1 && flag)
				{
					g.SetColor(mColors[3]);
					g.FillRect(0, 0, mWidth, mHeight);
				}
				if (i == 0 || !flag)
				{
					g.SetColor(mColors[2]);
				}
				else
				{
					g.SetColor(mColors[4]);
				}
				g.DrawString(displayString.Substring(mLeftPos), mTextInset, (mHeight - mFont.GetHeight()) / 2 + mFont.GetAscent());
				g.PopState();
			}
			g.SetColor(mColors[1]);
			g.DrawRect(0, 0, mWidth - 1, mHeight - 1);
		}

		protected virtual void UpdateCaretPos()
		{
		}

		public override void GotFocus()
		{
			base.GotFocus();
			bool mTabletPC = mWidgetManager.mApp.mTabletPC;
			mWidgetManager.mApp.ShowKeyboard();
			mShowingCursor = true;
			mBlinkAcc = 0;
			MarkDirty();
		}

		public override void LostFocus()
		{
			base.LostFocus();
			bool mTabletPC = mWidgetManager.mApp.mTabletPC;
			mWidgetManager.mApp.HideKeyboard();
			mShowingCursor = false;
			MarkDirty();
		}

		public override void Update()
		{
			base.Update();
			if (mHasFocus)
			{
				if (mWidgetManager.mApp.mTabletPC)
				{
					UpdateCaretPos();
				}
				if (++mBlinkAcc > mBlinkDelay)
				{
					MarkDirty();
					mBlinkAcc = 0;
					mShowingCursor = !mShowingCursor;
				}
			}
		}

		public void EnforceMaxPixels()
		{
			if (mMaxPixels <= 0 && Common.size(mWidthCheckList) == 0)
			{
				return;
			}
			if (Common.size(mWidthCheckList) == 0)
			{
				while (mFont.StringWidth(mString) > mMaxPixels)
				{
					mString = mString.Substring(0, mString.Length - 1);
				}
				return;
			}
			for (int i = 0; i < mWidthCheckList.Count; i++)
			{
				int num = mWidthCheckList[i].mWidth;
				if (num <= 0)
				{
					num = mMaxPixels;
					if (num <= 0)
					{
						continue;
					}
				}
				while (mWidthCheckList[i].mFont.StringWidth(mString) > num)
				{
					mString = mString.Substring(0, mString.Length - 1);
				}
			}
		}

		public virtual bool IsPartOfWord(char theChar)
		{
			if ((theChar < 'A' || theChar > 'Z') && (theChar < 'a' || theChar > 'z') && (theChar < '0' || theChar > '9') && (theChar < '¿' || theChar > 'ˇ'))
			{
				return theChar == '_';
			}
			return true;
		}

		protected virtual void ProcessKey(KeyCode theKey, char theChar)
		{
			bool flag = mWidgetManager.mKeyDown[16];
			bool flag2 = mWidgetManager.mKeyDown[17];
			if (theKey == KeyCode.KEYCODE_SHIFT || theKey == KeyCode.KEYCODE_CONTROL || theKey == KeyCode.KEYCODE_COMMAND)
			{
				return;
			}
			bool flag3 = false;
			bool flag4 = !flag;
			if (flag && mHilitePos == -1)
			{
				mHilitePos = mCursorPos;
			}
			string text = mString;
			int num = mCursorPos;
			int num2 = mHilitePos;
			int num3 = mLeftPos;
			switch (theChar)
			{
			case '\u0003':
			case '\u0018':
				if (mHilitePos != -1 && mHilitePos != mCursorPos)
				{
					if (mCursorPos < mHilitePos)
					{
						mWidgetManager.mApp.CopyToClipboard(GetDisplayString().Substring(mCursorPos, mHilitePos));
					}
					else
					{
						mWidgetManager.mApp.CopyToClipboard(GetDisplayString().Substring(mHilitePos, mCursorPos));
					}
					if (theChar == '\u0003')
					{
						flag4 = false;
						break;
					}
					mString = mString.Substring(0, MIN(mCursorPos, mHilitePos)) + mString.Substring(MAX(mCursorPos, mHilitePos));
					mCursorPos = MIN(mCursorPos, mHilitePos);
					mHilitePos = -1;
					flag3 = true;
				}
				break;
			case '\u0016':
			{
				string clipboard = mWidgetManager.mApp.GetClipboard();
				if (clipboard.Length <= 0)
				{
					break;
				}
				string text3 = "";
				for (int i = 0; i < clipboard.Length && clipboard.ToString()[i] != '\r' && clipboard.ToString()[i] != '\n'; i++)
				{
					if (mFont.CharWidth(clipboard.ToString()[i]) != 0 && mEditListener.AllowChar(mId, clipboard.ToString()[i]))
					{
						text3 += char.ToString(clipboard.ToString()[i]);
					}
				}
				if (mHilitePos == -1)
				{
					mString = mString.Substring(0, mCursorPos) + text3 + mString.Substring(mCursorPos);
				}
				else
				{
					mString = mString.Substring(0, MIN(mCursorPos, mHilitePos)) + text3 + mString.Substring(MAX(mCursorPos, mHilitePos));
					mCursorPos = MIN(mCursorPos, mHilitePos);
					mHilitePos = -1;
				}
				mCursorPos += text3.Length;
				flag3 = true;
				break;
			}
			case '\u001a':
			{
				mLastModifyIdx = -1;
				string text2 = mString;
				int num6 = mCursorPos;
				int num7 = mHilitePos;
				mString = mUndoString;
				mCursorPos = mUndoCursor;
				mHilitePos = mUndoHilitePos;
				mUndoString = text2;
				mUndoCursor = num6;
				mUndoHilitePos = num7;
				flag4 = false;
				break;
			}
			default:
				switch (theKey)
				{
				case KeyCode.KEYCODE_LEFT:
					if (flag2)
					{
						while (mCursorPos > 0 && !IsPartOfWord(mString[mCursorPos - 1]))
						{
							mCursorPos--;
						}
						while (mCursorPos > 0 && IsPartOfWord(mString[mCursorPos - 1]))
						{
							mCursorPos--;
						}
					}
					else if (flag || mHilitePos == -1)
					{
						mCursorPos--;
					}
					else
					{
						mCursorPos = MIN(mCursorPos, mHilitePos);
					}
					break;
				case KeyCode.KEYCODE_RIGHT:
					if (flag2)
					{
						while (mCursorPos < mString.Length - 1 && IsPartOfWord(mString[mCursorPos + 1]))
						{
							mCursorPos++;
						}
						while (mCursorPos < mString.Length - 1 && !IsPartOfWord(mString[mCursorPos + 1]))
						{
							mCursorPos++;
						}
					}
					if (flag || mHilitePos == -1)
					{
						mCursorPos++;
					}
					else
					{
						mCursorPos = MAX(mCursorPos, mHilitePos);
					}
					break;
				case KeyCode.KEYCODE_BACK:
					if (mString.Length <= 0)
					{
						break;
					}
					if (mHilitePos != -1 && mHilitePos != mCursorPos)
					{
						mString = mString.Substring(0, MIN(mCursorPos, mHilitePos)) + mString.Substring(MAX(mCursorPos, mHilitePos));
						mCursorPos = MIN(mCursorPos, mHilitePos);
						mHilitePos = -1;
						flag3 = true;
						break;
					}
					if (mCursorPos > 0)
					{
						mString = mString.Substring(0, mCursorPos - 1) + mString.Substring(mCursorPos);
					}
					else
					{
						mString = mString.Substring(mCursorPos);
					}
					mCursorPos--;
					mHilitePos = -1;
					if (mCursorPos != mLastModifyIdx)
					{
						flag3 = true;
					}
					mLastModifyIdx = mCursorPos - 1;
					break;
				case KeyCode.KEYCODE_DELETE:
					if (mString.Length <= 0)
					{
						break;
					}
					if (mHilitePos != -1 && mHilitePos != mCursorPos)
					{
						mString = mString.Substring(0, MIN(mCursorPos, mHilitePos)) + mString.Substring(MAX(mCursorPos, mHilitePos));
						mCursorPos = MIN(mCursorPos, mHilitePos);
						mHilitePos = -1;
						flag3 = true;
						break;
					}
					if (mCursorPos < mString.Length)
					{
						mString = mString.Substring(0, mCursorPos) + mString.Substring(mCursorPos + 1);
					}
					if (mCursorPos != mLastModifyIdx)
					{
						flag3 = true;
					}
					mLastModifyIdx = mCursorPos;
					break;
				case KeyCode.KEYCODE_HOME:
					mCursorPos = 0;
					break;
				case KeyCode.KEYCODE_END:
					mCursorPos = mString.Length;
					break;
				case KeyCode.KEYCODE_RETURN:
					mEditListener.EditWidgetText(mId, mString);
					break;
				default:
				{
					string theString = theChar.ToString();
					uint num4 = theChar;
					uint num5 = 127u;
					if (GlobalMembers.gSexyAppBase.mbAllowExtendedChars)
					{
						num5 = 255u;
					}
					if (num4 >= 32 && num4 <= num5 && mFont.StringWidth(theString) > 0 && mEditListener.AllowChar(mId, theChar))
					{
						if (mHilitePos != -1 && mHilitePos != mCursorPos)
						{
							mString = mString.Substring(0, MIN(mCursorPos, mHilitePos)) + theChar + mString.Substring(MAX(mCursorPos, mHilitePos));
							mCursorPos = MIN(mCursorPos, mHilitePos);
							mHilitePos = -1;
							flag3 = true;
						}
						else
						{
							mString = mString.Substring(0, mCursorPos) + theChar + mString.Substring(mCursorPos);
							if (mCursorPos != mLastModifyIdx + 1)
							{
								flag3 = true;
							}
							mLastModifyIdx = mCursorPos;
							mHilitePos = -1;
						}
						mCursorPos++;
						FocusCursor(false);
					}
					else
					{
						flag4 = false;
					}
					break;
				}
				}
				break;
			}
			if (mMaxChars != -1 && mString.Length > mMaxChars)
			{
				mString = mString.Substring(0, mMaxChars);
			}
			EnforceMaxPixels();
			if (mCursorPos < 0)
			{
				mCursorPos = 0;
			}
			else if (mCursorPos > mString.Length)
			{
				mCursorPos = mString.Length;
			}
			if (num != mCursorPos)
			{
				mBlinkAcc = 0;
				mShowingCursor = true;
			}
			FocusCursor(true);
			if (flag4 || mHilitePos == mCursorPos)
			{
				mHilitePos = -1;
			}
			if (!mEditListener.AllowText(mId, mString))
			{
				mString = text;
				mCursorPos = num;
				mHilitePos = num2;
				mLeftPos = num3;
			}
			else if (flag3)
			{
				mUndoString = text;
				mUndoCursor = num;
				mUndoHilitePos = num2;
			}
			MarkDirty();
		}

		public override void KeyDown(KeyCode theKey)
		{
			if ((theKey < (KeyCode)65 || theKey >= KeyCode.KEYCODE_ASCIIEND) && mEditListener.AllowKey(mId, theKey))
			{
				ProcessKey(theKey, '\0');
			}
			base.KeyDown(theKey);
		}

		public override void KeyChar(char theChar)
		{
			ProcessKey(KeyCode.KEYCODE_UNKNOWN, theChar);
			base.KeyChar(theChar);
		}

		public virtual int GetCharAt(int x, int y)
		{
			int result = 0;
			string displayString = GetDisplayString();
			for (int i = mLeftPos; i < displayString.Length; i++)
			{
				string theString = displayString.Substring(mLeftPos, i - mLeftPos);
				string theString2 = displayString.Substring(mLeftPos, i - mLeftPos + 1);
				int num = mFont.StringWidth(theString);
				int num2 = mFont.StringWidth(theString2);
				if (x >= (num + num2) / 2 + 5)
				{
					result = i + 1;
				}
			}
			return result;
		}

		public virtual void FocusCursor(bool bigJump)
		{
			while (mCursorPos < mLeftPos)
			{
				if (bigJump)
				{
					mLeftPos = MAX(0, mLeftPos - 10);
				}
				else
				{
					mLeftPos = MAX(0, mLeftPos - 1);
				}
				MarkDirty();
			}
			if (mFont == null)
			{
				return;
			}
			string displayString = GetDisplayString();
			while (mWidth - 8 > 0 && mFont.StringWidth(displayString.Substring(0, mCursorPos)) - mFont.StringWidth(displayString.Substring(0, mLeftPos)) >= mWidth - 8)
			{
				if (bigJump)
				{
					mLeftPos = MIN(mLeftPos + 10, mString.Length - 1);
				}
				else
				{
					mLeftPos = MIN(mLeftPos + 1, mString.Length - 1);
				}
				MarkDirty();
			}
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
			base.MouseDown(x, y, theBtnNum, theClickCount);
			mWidgetManager.mApp.ShowKeyboard();
			mHilitePos = -1;
			mCursorPos = GetCharAt(x, y);
			if (theClickCount > 1)
			{
				mHadDoubleClick = true;
				HiliteWord();
			}
			MarkDirty();
			FocusCursor(false);
		}

		public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
		{
			base.MouseUp(x, y, theBtnNum, theClickCount);
			if (mHilitePos == mCursorPos)
			{
				mHilitePos = -1;
			}
			if (mHadDoubleClick)
			{
				mHilitePos = -1;
				mCursorPos = GetCharAt(x, y);
				mHadDoubleClick = false;
				HiliteWord();
			}
			MarkDirty();
		}

		protected virtual void HiliteWord()
		{
			string displayString = GetDisplayString();
			if (mCursorPos < displayString.Length)
			{
				mHilitePos = mCursorPos;
				while (mHilitePos > 0 && IsPartOfWord(displayString[mHilitePos - 1]))
				{
					mHilitePos--;
				}
				while (mCursorPos < displayString.Length - 1 && IsPartOfWord(displayString[mCursorPos + 1]))
				{
					mCursorPos++;
				}
				if (mCursorPos < displayString.Length)
				{
					mCursorPos++;
				}
			}
		}

		public override void MouseDrag(int x, int y)
		{
			base.MouseDrag(x, y);
			if (mHilitePos == -1)
			{
				mHilitePos = mCursorPos;
			}
			mCursorPos = GetCharAt(x, y);
			MarkDirty();
			FocusCursor(false);
		}

		public override void MouseEnter()
		{
			base.MouseEnter();
			mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_TEXT);
		}

		public override void MouseLeave()
		{
			base.MouseLeave();
			mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_POINTER);
		}

		public override void MarkDirty()
		{
			if (mColors[0].mAlpha != 255)
			{
				base.MarkDirtyFull();
			}
			else
			{
				base.MarkDirty();
			}
		}
	}
}
