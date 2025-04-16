using System.Collections.Generic;
using SexyFramework.Drivers;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public abstract class Widget : WidgetContainer
	{
		public bool mVisible;

		public bool mMouseVisible;

		public bool mDisabled;

		public bool mHasFocus;

		public bool mIsDown;

		public bool mIsOver;

		public bool mHasTransparencies;

		public bool mDoFinger;

		public bool mWantsFocus;

		public bool mIsGamepadSelection;

		public int mDataMenuId;

		public List<Color> mColors = new List<Color>();

		public Insets mMouseInsets = new Insets();

		public Widget mTabPrev;

		public Widget mTabNext;

		public Widget mGamepadParent;

		public Widget mGamepadLinkUp;

		public Widget mGamepadLinkDown;

		public Widget mGamepadLinkLeft;

		public Widget mGamepadLinkRight;

		public static bool mWriteColoredString = true;

		private Color GetColor_aColor = default(Color);

		public bool mIsFinishDrawOverlay;

		public Widget()
		{
			mWidgetManager = null;
			mVisible = true;
			mDisabled = false;
			mIsDown = false;
			mIsOver = false;
			mDoFinger = false;
			mMouseVisible = true;
			mHasFocus = false;
			mHasTransparencies = false;
			mWantsFocus = false;
			mTabPrev = null;
			mTabNext = null;
			mIsGamepadSelection = false;
			mGamepadParent = null;
			mGamepadLinkUp = null;
			mGamepadLinkDown = null;
			mGamepadLinkLeft = null;
			mGamepadLinkRight = null;
			mDataMenuId = -1;
			mColors = new List<Color>();
			mMouseInsets = new Insets();
		}

		public override void Dispose()
		{
			mColors.Clear();
			base.Dispose();
		}

		public void CopyFrom(Widget rhs)
		{
			CopyFrom((WidgetContainer)rhs);
			mVisible = rhs.mVisible;
			mMouseVisible = rhs.mMouseVisible;
			mDisabled = rhs.mDisabled;
			mHasFocus = rhs.mHasFocus;
			mIsDown = rhs.mIsDown;
			mIsOver = rhs.mIsOver;
			mHasTransparencies = rhs.mHasTransparencies;
			mDoFinger = rhs.mDoFinger;
			mWantsFocus = rhs.mWantsFocus;
			mIsGamepadSelection = rhs.mIsGamepadSelection;
			mDataMenuId = rhs.mDataMenuId;
			mTabPrev = rhs.mTabPrev;
			mTabNext = rhs.mTabNext;
			mGamepadParent = rhs.mGamepadParent;
			mGamepadLinkUp = rhs.mGamepadLinkUp;
			mGamepadLinkDown = rhs.mGamepadLinkDown;
			mGamepadLinkLeft = rhs.mGamepadLinkLeft;
			mGamepadLinkRight = rhs.mGamepadLinkRight;
			mMouseInsets.mLeft = rhs.mMouseInsets.mLeft;
			mMouseInsets.mRight = rhs.mMouseInsets.mRight;
			mMouseInsets.mBottom = rhs.mMouseInsets.mBottom;
			mMouseInsets.mTop = rhs.mMouseInsets.mTop;
			mColors.Clear();
			for (int i = 0; i < rhs.mColors.Count; i++)
			{
				mColors.Add(new Color(rhs.mColors[i]));
			}
		}

		public virtual void OrderInManagerChanged()
		{
		}

		public virtual void SetVisible(bool isVisible)
		{
			if (mVisible != isVisible)
			{
				mVisible = isVisible;
				if (mVisible)
				{
					MarkDirty();
				}
				else
				{
					MarkDirtyFull();
				}
				if (mWidgetManager != null)
				{
					mWidgetManager.RehupMouse();
				}
			}
		}

		public virtual void WidgetSetVisible(bool isVisible)
		{
			SetVisible(isVisible);
		}

		public virtual void SetColors3(int[,] theColors, int theNumColors)
		{
			mColors.Clear();
			for (int i = 0; i < theNumColors; i++)
			{
				SetColor(i, new Color(theColors[i, 0], theColors[i, 1], theColors[i, 2]));
			}
			MarkDirty();
		}

		public virtual void SetColors4(int[,] theColors, int theNumColors)
		{
			mColors.Clear();
			for (int i = 0; i < theNumColors; i++)
			{
				SetColor(i, new Color(theColors[i, 0], theColors[i, 1], theColors[i, 2], theColors[i, 3]));
			}
			MarkDirty();
		}

		public virtual void SetColor(int theIdx, Color theColor)
		{
			if (theIdx >= mColors.Count)
			{
				Common.Resize(mColors, theIdx + 1);
			}
			mColors[theIdx] = theColor;
			MarkDirty();
		}

		public virtual Color GetColor(int theIdx)
		{
			if (theIdx < mColors.Count)
			{
				return mColors[theIdx];
			}
			return GetColor_aColor;
		}

		public virtual Color GetColor(int theIdx, Color theDefaultColor)
		{
			if (theIdx < mColors.Count)
			{
				return mColors[theIdx];
			}
			return theDefaultColor;
		}

		public virtual void SetDisabled(bool isDisabled)
		{
			if (mDisabled != isDisabled)
			{
				mDisabled = isDisabled;
				if (isDisabled && mWidgetManager != null)
				{
					mWidgetManager.DisableWidget(this);
				}
				MarkDirty();
				if (!isDisabled && mWidgetManager != null && Contains(mWidgetManager.mLastMouseX, mWidgetManager.mLastMouseY))
				{
					mWidgetManager.MousePosition(mWidgetManager.mLastMouseX, mWidgetManager.mLastMouseY);
				}
			}
		}

		public virtual void ShowFinger(bool on)
		{
			WidgetManager mWidgetManager2 = mWidgetManager;
		}

		public void superSubResize(int theX, int theY, int theWidth, int theHeight)
		{
			if (mX != theX || mY != theY || mWidth != theWidth || mHeight != theHeight)
			{
				MarkDirtyFull();
				mX = theX;
				mY = theY;
				mWidth = theWidth;
				mHeight = theHeight;
				MarkDirty();
				if (mWidgetManager != null)
				{
					mWidgetManager.RehupMouse();
				}
			}
		}

		public virtual void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			superSubResize(theX, theY, theWidth, theHeight);
		}

		public void WidgetResize(int theX, int theY, int theWidth, int theHeight)
		{
			Resize(theX, theY, theWidth, theHeight);
		}

		public virtual void Resize(Rect theRect)
		{
			Resize(theRect.mX, theRect.mY, theRect.mWidth, theRect.mHeight);
		}

		public void WidgetResize(Rect theRect)
		{
			Resize(theRect);
		}

		public virtual void Move(int theNewX, int theNewY)
		{
			Resize(theNewX, theNewY, mWidth, mHeight);
		}

		public virtual bool WantsFocus()
		{
			return mWantsFocus;
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
		}

		public virtual void DrawOverlay(SexyFramework.Graphics.Graphics g)
		{
		}

		public virtual void DrawOverlay(SexyFramework.Graphics.Graphics g, int thePriority)
		{
			DrawOverlay(g);
		}

		public void WidgetDrawOverlay(SexyFramework.Graphics.Graphics g, int thePriority)
		{
			DrawOverlay(g, thePriority);
		}

		public override void Update()
		{
			WidgetUpdate();
		}

		public void WidgetUpdate()
		{
			base.Update();
		}

		public override void UpdateF(float theFrac)
		{
		}

		public virtual void GotFocus()
		{
			mHasFocus = true;
		}

		public virtual void LostFocus()
		{
			mHasFocus = false;
		}

		public virtual bool IsPointVisible(int x, int y)
		{
			return true;
		}

		public virtual void KeyChar(char theChar)
		{
		}

		public virtual void KeyDown(KeyCode theKey)
		{
			if (theKey != KeyCode.KEYCODE_TAB)
			{
				return;
			}
			if (mWidgetManager.mKeyDown[16])
			{
				if (mTabPrev != null)
				{
					mWidgetManager.SetFocus(mTabPrev);
				}
			}
			else if (mTabNext != null)
			{
				mWidgetManager.SetFocus(mTabNext);
			}
		}

		public virtual void KeyUp(KeyCode theKey)
		{
		}

		public virtual void MouseEnter()
		{
		}

		public virtual void MouseLeave()
		{
		}

		public virtual void MouseMove(int x, int y)
		{
		}

		public virtual void MouseDown(int x, int y, int theClickCount)
		{
			if (theClickCount == 3)
			{
				MouseDown(x, y, 2, 1);
			}
			else if (theClickCount >= 0)
			{
				MouseDown(x, y, 0, theClickCount);
			}
			else
			{
				MouseDown(x, y, 1, -theClickCount);
			}
		}

		public virtual void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
		}

		public virtual void MouseUp(int x, int y)
		{
		}

		public virtual void MouseUp(int x, int y, int theLastDownButtonId)
		{
			MouseUp(x, y);
			if (theLastDownButtonId == 3)
			{
				MouseUp(x, y, 2, 1);
			}
			else if (theLastDownButtonId >= 0)
			{
				MouseUp(x, y, 0, theLastDownButtonId);
			}
			else
			{
				MouseUp(x, y, 1, -theLastDownButtonId);
			}
		}

		public virtual void MouseUp(int x, int y, int theBtnNum, int theClickCount)
		{
		}

		public virtual void MouseDrag(int x, int y)
		{
		}

		public virtual void MouseWheel(int theDelta)
		{
		}

		public virtual void TouchBegan(SexyAppBase.Touch touch)
		{
			int x = touch.location.mX;
			int y = touch.location.mY;
			MouseDown(x, y, 1);
		}

		public virtual void TouchMoved(SexyAppBase.Touch touch)
		{
			int x = touch.location.mX;
			int y = touch.location.mY;
			MouseDrag(x, y);
		}

		public virtual void TouchEnded(SexyAppBase.Touch touch)
		{
			int x = touch.location.mX;
			int y = touch.location.mY;
			MouseUp(x, y, 1);
		}

		public void WidgetTouchEnded(SexyAppBase.Touch touch)
		{
			TouchEnded(touch);
		}

		public virtual void TouchesCanceled()
		{
		}

		public virtual void SetGamepadLinks(Widget up, Widget down, Widget left, Widget right)
		{
			mGamepadLinkUp = up;
			mGamepadLinkDown = down;
			mGamepadLinkLeft = left;
			mGamepadLinkRight = right;
		}

		public virtual void SetGamepadParent(Widget theParent)
		{
			mGamepadParent = theParent;
		}

		public virtual void GotGamepadSelection(WidgetLinkDir theDirection)
		{
			mIsGamepadSelection = true;
		}

		public virtual void LostGamepadSelection()
		{
			mIsGamepadSelection = false;
		}

		public virtual void GamepadButtonDown(GamepadButton theButton, int thePlayer, uint theFlags)
		{
			switch (theButton)
			{
			case GamepadButton.GAMEPAD_BUTTON_UP:
				if (mGamepadLinkUp != null && mWidgetManager != null)
				{
					Widget widget2 = mGamepadLinkUp;
					while (widget2 != null && !widget2.mVisible)
					{
						widget2 = widget2.mGamepadLinkUp;
					}
					if (widget2 != null)
					{
						mWidgetManager.SetGamepadSelection(widget2, WidgetLinkDir.LINK_DIR_UP);
					}
				}
				break;
			case GamepadButton.GAMEPAD_BUTTON_DOWN:
				if (mGamepadLinkDown != null && mWidgetManager != null)
				{
					Widget widget3 = mGamepadLinkDown;
					while (widget3 != null && !widget3.mVisible)
					{
						widget3 = widget3.mGamepadLinkDown;
					}
					if (widget3 != null)
					{
						mWidgetManager.SetGamepadSelection(widget3, WidgetLinkDir.LINK_DIR_DOWN);
					}
				}
				break;
			case GamepadButton.GAMEPAD_BUTTON_LEFT:
				if (mGamepadLinkLeft != null && mWidgetManager != null)
				{
					Widget widget4 = mGamepadLinkLeft;
					while (widget4 != null && !widget4.mVisible)
					{
						widget4 = widget4.mGamepadLinkLeft;
					}
					if (widget4 != null)
					{
						mWidgetManager.SetGamepadSelection(widget4, WidgetLinkDir.LINK_DIR_LEFT);
					}
				}
				break;
			case GamepadButton.GAMEPAD_BUTTON_RIGHT:
				if (mGamepadLinkRight != null && mWidgetManager != null)
				{
					Widget widget = mGamepadLinkRight;
					while (widget != null && !widget.mVisible)
					{
						widget = widget.mGamepadLinkRight;
					}
					if (widget != null)
					{
						mWidgetManager.SetGamepadSelection(widget, WidgetLinkDir.LINK_DIR_RIGHT);
					}
				}
				break;
			}
			if (mGamepadParent != null)
			{
				mGamepadParent.GamepadButtonDown(theButton, thePlayer, theFlags);
			}
		}

		public virtual void GamepadButtonUp(GamepadButton theButton, int thePlayer, uint theFlags)
		{
			if (mGamepadParent != null)
			{
				mGamepadParent.GamepadButtonUp(theButton, thePlayer, theFlags);
			}
		}

		public virtual void GamepadAxisMove(GamepadAxis theAxis, int thePlayer, float theAxisValue)
		{
			if (mGamepadParent != null)
			{
				mGamepadParent.GamepadAxisMove(theAxis, thePlayer, theAxisValue);
			}
		}

		public virtual Rect WriteCenteredLine(SexyFramework.Graphics.Graphics g, int anOffset, string theLine)
		{
			Font font = g.GetFont();
			int num = font.StringWidth(theLine);
			int theX = (mWidth - num) / 2;
			g.DrawString(theLine, theX, anOffset);
			return new Rect(theX, anOffset - font.GetAscent(), num, font.GetHeight());
		}

		public virtual int WriteString(SexyFramework.Graphics.Graphics g, string theString, int theX, int theY, int theWidth, int theJustification, bool drawString, int theOffset, int theLength)
		{
			bool flag = g.mWriteColoredString;
			g.mWriteColoredString = mWriteColoredString;
			int result = g.WriteString(theString, theX, theY, theWidth, theJustification, drawString, theOffset, theLength);
			g.mWriteColoredString = flag;
			return result;
		}

		public virtual int WriteWordWrapped(SexyFramework.Graphics.Graphics g, Rect theRect, string theLine, int theLineSpacing, int theJustification)
		{
			bool flag = g.mWriteColoredString;
			g.mWriteColoredString = mWriteColoredString;
			int result = g.WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification);
			g.mWriteColoredString = flag;
			return result;
		}

		public int GetWordWrappedHeight(SexyFramework.Graphics.Graphics g, int theWidth, string theLine, int aLineSpacing)
		{
			int theMaxWidth = 0;
			int theLineCount = 0;
			return g.GetWordWrappedHeight(theWidth, theLine, aLineSpacing, ref theMaxWidth, ref theLineCount);
		}

		public virtual int GetNumDigits(int theNumber)
		{
			int num = 10;
			int num2 = 1;
			while (theNumber >= num)
			{
				num2++;
				num *= 10;
			}
			return num2;
		}

		public virtual void WriteNumberFromStrip(SexyFramework.Graphics.Graphics g, int theNumber, int theX, int theY, Image theNumberStrip, int aSpacing)
		{
			int num = 10;
			int num2 = 1;
			while (theNumber >= num)
			{
				num2++;
				num *= 10;
			}
			if (theNumber == 0)
			{
				num = 10;
			}
			int num3 = theNumberStrip.GetWidth() / 10;
			for (int i = 0; i < num2; i++)
			{
				num /= 10;
				int num4 = theNumber / num % 10;
				g.PushState();
				g.ClipRect(theX + i * (num3 + aSpacing), theY, num3, theNumberStrip.GetHeight());
				g.DrawImage(theNumberStrip, theX + i * (num3 + aSpacing) - num4 * num3, theY);
				g.PopState();
			}
		}

		public virtual bool Contains(int theX, int theY)
		{
			if (theX >= mX && theX < mX + mWidth && theY >= mY)
			{
				return theY < mY + mHeight;
			}
			return false;
		}

		public virtual Rect GetInsetRect()
		{
			return new Rect(mX + mMouseInsets.mLeft, mY + mMouseInsets.mTop, mWidth - mMouseInsets.mLeft - mMouseInsets.mRight, mHeight - mMouseInsets.mTop - mMouseInsets.mBottom);
		}

		public void DeferOverlay(int thePriority)
		{
			mWidgetManager.DeferOverlay(this, thePriority);
		}

		public void WidgetRemovedHelper()
		{
			if (mWidgetManager == null)
			{
				return;
			}
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.WidgetRemovedHelper();
			}
			mWidgetManager.DisableWidget(this);
			LinkedList<PreModalInfo>.Enumerator enumerator2 = mWidgetManager.mPreModalInfoList.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				PreModalInfo current = enumerator2.Current;
				if (current.mPrevBaseModalWidget == this)
				{
					current.mPrevBaseModalWidget = null;
				}
				if (current.mPrevFocusWidget == this)
				{
					current.mPrevFocusWidget = null;
				}
			}
			RemovedFromManager(mWidgetManager);
			MarkDirtyFull(this);
			if (mWidgetManager.GetGamepadSelection() == this)
			{
				mWidgetManager.SetGamepadSelection(null, WidgetLinkDir.LINK_DIR_NONE);
			}
			mWidgetManager = null;
		}

		public int Left()
		{
			return mX;
		}

		public int Top()
		{
			return mY;
		}

		public int Right()
		{
			return mX + mWidth;
		}

		public int Bottom()
		{
			return mY + mHeight;
		}

		public int Width()
		{
			return mWidth;
		}

		public int Height()
		{
			return mHeight;
		}

		public void Layout(int theLayoutFlags, Widget theRelativeWidget, int theLeftPad, int theTopPad, int theWidthPad, int theHeightPad)
		{
			int num = theRelativeWidget.Left();
			int num2 = theRelativeWidget.Top();
			if (theRelativeWidget == mParent)
			{
				num = 0;
				num2 = 0;
			}
			int num3 = theRelativeWidget.Width();
			int num4 = theRelativeWidget.Height();
			int num5 = num + num3;
			int num6 = num2 + num4;
			int num7 = Left();
			int num8 = Top();
			int num9 = Width();
			int num10 = Height();
			for (int num11 = 1; num11 < 4194304; num11 <<= 1)
			{
				if ((theLayoutFlags & num11) != 0)
				{
					switch ((LayoutFlags)num11)
					{
					case LayoutFlags.LAY_SameWidth:
						num9 = num3 + theWidthPad;
						break;
					case LayoutFlags.LAY_SameHeight:
						num10 = num4 + theHeightPad;
						break;
					case LayoutFlags.LAY_Above:
						num8 = num2 - num10 + theTopPad;
						break;
					case LayoutFlags.LAY_Below:
						num8 = num6 + theTopPad;
						break;
					case LayoutFlags.LAY_Right:
						num7 = num5 + theLeftPad;
						break;
					case LayoutFlags.LAY_Left:
						num7 = num - num9 + theLeftPad;
						break;
					case LayoutFlags.LAY_SameLeft:
						num7 = num + theLeftPad;
						break;
					case LayoutFlags.LAY_SameRight:
						num7 = num5 - num9 + theLeftPad;
						break;
					case LayoutFlags.LAY_SameTop:
						num8 = num2 + theTopPad;
						break;
					case LayoutFlags.LAY_SameBottom:
						num8 = num6 - num10 + theTopPad;
						break;
					case LayoutFlags.LAY_GrowToRight:
						num9 = num5 - num7 + theWidthPad;
						break;
					case LayoutFlags.LAY_GrowToLeft:
						num9 = num - num7 + theWidthPad;
						break;
					case LayoutFlags.LAY_GrowToTop:
						num10 = num2 - num8 + theHeightPad;
						break;
					case LayoutFlags.LAY_GrowToBottom:
						num10 = num6 - num8 + theHeightPad;
						break;
					case LayoutFlags.LAY_SetLeft:
						num7 = theLeftPad;
						break;
					case LayoutFlags.LAY_SetTop:
						num8 = theTopPad;
						break;
					case LayoutFlags.LAY_SetWidth:
						num9 = theWidthPad;
						break;
					case LayoutFlags.LAY_SetHeight:
						num10 = theHeightPad;
						break;
					case LayoutFlags.LAY_HCenter:
						num7 = num + (num3 - num9) / 2 + theLeftPad;
						break;
					case LayoutFlags.LAY_VCenter:
						num8 = num2 + (num4 - num10) / 2 + theTopPad;
						break;
					}
				}
			}
			Resize(num7, num8, num9, num10);
		}
	}
}
