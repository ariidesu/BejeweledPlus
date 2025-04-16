using System.Collections.Generic;
using SexyFramework.Drivers;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class WidgetManager : WidgetContainer
	{
		public class Pair<KEY, VALUE>
		{
			public KEY Key;

			public VALUE Value;

			public Pair(KEY k, VALUE v)
			{
				Key = k;
				Value = v;
			}
		}

		public Widget mDefaultTab;

		public SexyFramework.Graphics.Graphics mCurG;

		public SexyAppBase mApp;

		public MemoryImage mImage;

		public MemoryImage mTransientImage;

		public bool mLastHadTransients;

		public Widget mPopupCommandWidget;

		public List<Pair<Widget, int>> mDeferredOverlayWidgets = new List<Pair<Widget, int>>();

		public int mMinDeferredOverlayPriority;

		public bool mHasFocus;

		public Widget mFocusWidget;

		public Widget mLastDownWidget;

		public Widget mOverWidget;

		public Widget mBaseModalWidget;

		public Widget mGamepadSelectionWidget;

		public FlagsMod mLostFocusFlagsMod = new FlagsMod();

		public FlagsMod mBelowModalFlagsMod = new FlagsMod();

		public FlagsMod mDefaultBelowModalFlagsMod = new FlagsMod();

		public LinkedList<PreModalInfo> mPreModalInfoList = new LinkedList<PreModalInfo>();

		public Rect mMouseDestRect = default(Rect);

		public Rect mMouseSourceRect = default(Rect);

		public bool mMouseIn;

		public int mLastMouseX;

		public int mLastMouseY;

		public int mDownButtons;

		public int mActualDownButtons;

		public int mLastInputUpdateCnt;

		public bool[] mKeyDown = new bool[255];

		public int mLastDownButtonId;

		public int mWidgetFlags;

		public WidgetManager(SexyAppBase theApp)
		{
			mApp = theApp;
			mMinDeferredOverlayPriority = int.MaxValue;
			mWidgetManager = this;
			mMouseIn = false;
			mDefaultTab = null;
			mImage = null;
			mLastHadTransients = false;
			mPopupCommandWidget = null;
			mFocusWidget = null;
			mLastDownWidget = null;
			mOverWidget = null;
			mBaseModalWidget = null;
			mGamepadSelectionWidget = null;
			mDefaultBelowModalFlagsMod.mRemoveFlags = 48;
			mWidth = 0;
			mHeight = 0;
			mHasFocus = true;
			mUpdateCnt = 0;
			mLastDownButtonId = 0;
			mDownButtons = 0;
			mActualDownButtons = 0;
			mWidgetFlags = 61;
			for (int i = 0; i < 255; i++)
			{
				mKeyDown[i] = false;
			}
		}

		public override void Dispose()
		{
			FreeResources();
			base.Dispose();
		}

		public void FreeResources()
		{
		}

		public void AddBaseModal(Widget theWidget, FlagsMod theBelowFlagsMod)
		{
			PreModalInfo preModalInfo = new PreModalInfo();
			preModalInfo.mBaseModalWidget = theWidget;
			preModalInfo.mPrevBaseModalWidget = mBaseModalWidget;
			preModalInfo.mPrevFocusWidget = mFocusWidget;
			preModalInfo.mPrevBelowModalFlagsMod = mBelowModalFlagsMod;
			mPreModalInfoList.AddLast(preModalInfo);
			SetBaseModal(theWidget, theBelowFlagsMod);
		}

		public void AddBaseModal(Widget theWidget)
		{
			AddBaseModal(theWidget, mDefaultBelowModalFlagsMod);
		}

		public void RemoveBaseModal(Widget theWidget)
		{
			bool flag = true;
			while (mPreModalInfoList.Count > 0)
			{
				PreModalInfo value = mPreModalInfoList.Last.Value;
				if (flag && value.mBaseModalWidget != theWidget)
				{
					break;
				}
				bool flag2 = value.mPrevBaseModalWidget != null || mPreModalInfoList.Count == 1;
				SetBaseModal(value.mPrevBaseModalWidget, value.mPrevBelowModalFlagsMod);
				if (mFocusWidget == null)
				{
					mFocusWidget = value.mPrevFocusWidget;
					if (mFocusWidget != null)
					{
						mFocusWidget.GotFocus();
					}
				}
				mPreModalInfoList.RemoveLast();
				if (flag2)
				{
					break;
				}
				flag = false;
			}
		}

		public void Resize(Rect theMouseDestRect, Rect theMouseSourceRect)
		{
			mWidth = theMouseDestRect.mWidth + 2 * theMouseDestRect.mX;
			mHeight = theMouseDestRect.mHeight + 2 * theMouseDestRect.mY;
			mMouseDestRect = theMouseDestRect;
			mMouseSourceRect = theMouseSourceRect;
		}

		public new void DisableWidget(Widget theWidget)
		{
			if (mOverWidget == theWidget)
			{
				Widget theWidget2 = mOverWidget;
				mOverWidget = null;
				MouseLeave(theWidget2);
			}
			if (mLastDownWidget == theWidget)
			{
				Widget theWidget3 = mLastDownWidget;
				mLastDownWidget = null;
				DoMouseUps(theWidget3, mDownButtons);
				mDownButtons = 0;
			}
			if (mFocusWidget == theWidget)
			{
				Widget widget = mFocusWidget;
				mFocusWidget = null;
				widget.LostFocus();
			}
			if (mBaseModalWidget == theWidget)
			{
				mBaseModalWidget = null;
			}
		}

		public Widget GetAnyWidgetAt(int x, int y, ref int theWidgetX, ref int theWidgetY)
		{
			bool found = false;
			return GetWidgetAtHelper(x, y, GetWidgetFlags(), ref found, ref theWidgetX, ref theWidgetY);
		}

		public Widget GetWidgetAt(int x, int y, ref int theWidgetX, ref int theWidgetY)
		{
			Widget widget = GetAnyWidgetAt(x, y, ref theWidgetX, ref theWidgetY);
			if (widget != null && widget.mDisabled)
			{
				widget = null;
			}
			return widget;
		}

		public new void SetFocus(Widget aWidget)
		{
			if (aWidget == mFocusWidget)
			{
				return;
			}
			if (mFocusWidget != null)
			{
				mFocusWidget.LostFocus();
			}
			if (aWidget != null && aWidget.mWidgetManager == this)
			{
				mFocusWidget = aWidget;
				if (mHasFocus && mFocusWidget != null)
				{
					mFocusWidget.GotFocus();
				}
			}
			else
			{
				mFocusWidget = null;
			}
		}

		public void GotFocus()
		{
			if (!mHasFocus)
			{
				mHasFocus = true;
				if (mFocusWidget != null)
				{
					mFocusWidget.GotFocus();
				}
			}
		}

		public void LostFocus()
		{
			if (mHasFocus)
			{
				return;
			}
			mActualDownButtons = 0;
			for (int i = 0; i < 255; i++)
			{
				if (mKeyDown[i])
				{
					KeyUp((KeyCode)i);
				}
			}
			mHasFocus = false;
			if (mFocusWidget != null)
			{
				mFocusWidget.LostFocus();
			}
		}

		public void InitModalFlags(ModalFlags theModalFlags)
		{
			theModalFlags.mIsOver = mBaseModalWidget == null;
			theModalFlags.mOverFlags = GetWidgetFlags();
			theModalFlags.mUnderFlags = FlagsMod.GetModFlags(theModalFlags.mOverFlags, mBelowModalFlagsMod);
		}

		public void DrawWidgetsTo(SexyFramework.Graphics.Graphics g)
		{
			g.Translate(mMouseDestRect.mX, mMouseDestRect.mY);
			mCurG = new SexyFramework.Graphics.Graphics(g);
			List<Pair<Widget, int>> list = mDeferredOverlayWidgets;
			mDeferredOverlayWidgets.Clear();
			ModalFlags modalFlags = new ModalFlags();
			InitModalFlags(modalFlags);
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				if (current.mVisible)
				{
					g.PushState();
					g.SetFastStretch(!g.Is3D());
					g.SetLinearBlend(g.Is3D());
					g.Translate(-mMouseDestRect.mX, -mMouseDestRect.mY);
					g.Translate(current.mX, current.mY);
					current.DrawAll(modalFlags, g);
					g.PopState();
				}
			}
			FlushDeferredOverlayWidgets(int.MaxValue);
			mDeferredOverlayWidgets = list;
			mCurG = null;
		}

		public void DoMouseUps(Widget theWidget, int theDownCode)
		{
			int[] array = new int[3] { 1, -1, 3 };
			for (int i = 0; i < 3; i++)
			{
				if ((theDownCode & (1 << i)) != 0)
				{
					theWidget.mIsDown = false;
					theWidget.MouseUp(mLastMouseX - theWidget.mX, mLastMouseY - theWidget.mY, array[i]);
				}
			}
		}

		public void DoMouseUps()
		{
			if (mLastDownWidget != null && mDownButtons != 0)
			{
				DoMouseUps(mLastDownWidget, mDownButtons);
				mDownButtons = 0;
				mLastDownWidget = null;
			}
		}

		public void DeferOverlay(Widget theWidget, int thePriority)
		{
			theWidget.mIsFinishDrawOverlay = false;
			mDeferredOverlayWidgets.Add(new Pair<Widget, int>(theWidget, thePriority));
			if (thePriority < mMinDeferredOverlayPriority)
			{
				mMinDeferredOverlayPriority = thePriority;
			}
		}

		public void FlushDeferredOverlayWidgets(int theMaxPriority)
		{
			if (mCurG == null)
			{
				return;
			}
			SexyFramework.Graphics.Graphics graphics = new SexyFramework.Graphics.Graphics(mCurG);
			while (mMinDeferredOverlayPriority <= theMaxPriority)
			{
				int num = int.MaxValue;
				for (int i = 0; i < mDeferredOverlayWidgets.Count; i++)
				{
					Widget key = mDeferredOverlayWidgets[i].Key;
					if (key != null && !key.mIsFinishDrawOverlay)
					{
						int value = mDeferredOverlayWidgets[i].Value;
						if (value == mMinDeferredOverlayPriority)
						{
							graphics.PushState();
							graphics.Translate(-mMouseDestRect.mX, -mMouseDestRect.mY);
							graphics.Translate(key.mX, key.mY);
							graphics.SetFastStretch(graphics.Is3D());
							graphics.SetLinearBlend(graphics.Is3D());
							mDeferredOverlayWidgets[i].Key.mIsFinishDrawOverlay = true;
							key.DrawOverlay(graphics, value);
							graphics.PopState();
						}
						else if (value < num)
						{
							num = value;
						}
					}
				}
				mMinDeferredOverlayPriority = num;
				if (num == int.MaxValue)
				{
					mDeferredOverlayWidgets.Clear();
					break;
				}
			}
		}

		public bool DrawScreen()
		{
			ModalFlags modalFlags = new ModalFlags();
			InitModalFlags(modalFlags);
			bool result = false;
			mMinDeferredOverlayPriority = int.MaxValue;
			mDeferredOverlayWidgets.Clear();
			SexyFramework.Graphics.Graphics theGraphics = (mCurG = new SexyFramework.Graphics.Graphics(mImage));
			DeviceImage deviceImage = null;
			bool flag = false;
			if (mImage != null)
			{
				deviceImage = mImage.AsDeviceImage();
				if (deviceImage != null)
				{
					flag = deviceImage.LockSurface();
				}
			}
			SexyFramework.Graphics.Graphics graphics = new SexyFramework.Graphics.Graphics(theGraphics);
			graphics.Translate(-mMouseDestRect.mX, -mMouseDestRect.mY);
			bool flag2 = mApp.Is3DAccelerated();
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				if (current == mWidgetManager.mBaseModalWidget)
				{
					modalFlags.mIsOver = true;
				}
				if (current.mVisible)
				{
					graphics.PushState();
					graphics.SetFastStretch(!flag2);
					graphics.SetLinearBlend(flag2);
					graphics.Translate(current.mX, current.mY);
					current.DrawAll(modalFlags, graphics);
					result = true;
					current.mDirty = false;
					graphics.PopState();
				}
			}
			graphics.flush();
			FlushDeferredOverlayWidgets(int.MaxValue);
			if (deviceImage != null && flag)
			{
				deviceImage.UnlockSurface();
			}
			mCurG = null;
			return result;
		}

		public bool UpdateFrame()
		{
			ModalFlags modalFlags = new ModalFlags();
			InitModalFlags(modalFlags);
			mUpdateCnt++;
			mLastWMUpdateCount = mUpdateCnt;
			UpdateAll(modalFlags);
			return mDirty;
		}

		public bool UpdateFrameF(float theFrac)
		{
			ModalFlags modalFlags = new ModalFlags();
			InitModalFlags(modalFlags);
			UpdateFAll(modalFlags, theFrac);
			return mDirty;
		}

		public void SetPopupCommandWidget(Widget theList)
		{
			mPopupCommandWidget = theList;
			AddWidget(mPopupCommandWidget);
		}

		public void RemovePopupCommandWidget()
		{
			if (mPopupCommandWidget != null)
			{
				Widget theWidget = mPopupCommandWidget;
				mPopupCommandWidget = null;
				RemoveWidget(theWidget);
			}
		}

		public void MousePosition(int x, int y)
		{
			int num = mLastMouseX;
			int num2 = mLastMouseY;
			mLastMouseX = x;
			mLastMouseY = y;
			if (mLastMouseX == -1 && mLastMouseY == -1)
			{
				return;
			}
			int theWidgetX = 0;
			int theWidgetY = 0;
			Widget widgetAt = GetWidgetAt(x, y, ref theWidgetX, ref theWidgetY);
			if (widgetAt != mOverWidget)
			{
				Widget widget = mOverWidget;
				mOverWidget = null;
				if (widget != null)
				{
					MouseLeave(widget);
				}
				mOverWidget = widgetAt;
				if (widgetAt != null)
				{
					MouseEnter(widgetAt);
					widgetAt.MouseMove(theWidgetX, theWidgetY);
				}
			}
			else if (num != x || num2 != y)
			{
				widgetAt?.MouseMove(theWidgetX, theWidgetY);
			}
		}

		public void RehupMouse()
		{
			if (mLastDownWidget != null)
			{
				if (mOverWidget != null)
				{
					int theWidgetX = 0;
					int theWidgetY = 0;
					Widget widgetAt = GetWidgetAt(mLastMouseX, mLastMouseY, ref theWidgetX, ref theWidgetY);
					if (widgetAt != mLastDownWidget)
					{
						Widget theWidget = mOverWidget;
						mOverWidget = null;
						MouseLeave(theWidget);
					}
				}
			}
			else if (mMouseIn)
			{
				MousePosition(mLastMouseX, mLastMouseY);
			}
		}

		public void RemapMouse(ref int theX, ref int theY)
		{
			if (mMouseSourceRect.mWidth != 0 && mMouseSourceRect.mHeight != 0)
			{
				theX = (theX - mMouseSourceRect.mX) * mMouseDestRect.mWidth / mMouseSourceRect.mWidth + mMouseDestRect.mX;
				theY = (theY - mMouseSourceRect.mY) * mMouseDestRect.mHeight / mMouseSourceRect.mHeight + mMouseDestRect.mY;
			}
		}

		public bool MouseUp(int x, int y, int theClickCount)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			int num = ((theClickCount < 0) ? 2 : ((theClickCount != 3) ? 1 : 4));
			mActualDownButtons &= ~num;
			if (mLastDownWidget != null && (mDownButtons & num) != 0)
			{
				Widget widget = mLastDownWidget;
				mDownButtons &= ~num;
				if (mDownButtons == 0)
				{
					mLastDownWidget = null;
				}
				widget.mIsDown = false;
				Point absPos = widget.GetAbsPos();
				widget.MouseUp(x - absPos.mX, y - absPos.mY, theClickCount);
			}
			else
			{
				mDownButtons &= ~num;
			}
			MousePosition(x, y);
			return true;
		}

		public bool MouseDown(int x, int y, int theClickCount)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			if (theClickCount < 0)
			{
				mActualDownButtons |= 2;
			}
			else if (theClickCount == 3)
			{
				mActualDownButtons |= 4;
			}
			else
			{
				mActualDownButtons |= 1;
			}
			MousePosition(x, y);
			if (mPopupCommandWidget != null && mPopupCommandWidget.Contains(x, y))
			{
				RemovePopupCommandWidget();
			}
			int theWidgetX = 0;
			int theWidgetY = 0;
			Widget widgetAt = GetWidgetAt(x, y, ref theWidgetX, ref theWidgetY);
			if (mLastDownWidget != null)
			{
				widgetAt = mLastDownWidget;
			}
			if (theClickCount < 0)
			{
				mLastDownButtonId = -1;
				mDownButtons |= 2;
			}
			else if (theClickCount == 3)
			{
				mLastDownButtonId = 2;
				mDownButtons |= 4;
			}
			else
			{
				mLastDownButtonId = 1;
				mDownButtons |= 1;
			}
			mLastDownWidget = widgetAt;
			if (widgetAt != null)
			{
				if (widgetAt.WantsFocus())
				{
					SetFocus(widgetAt);
				}
				widgetAt.mIsDown = true;
				widgetAt.MouseDown(theWidgetX, theWidgetY, theClickCount);
			}
			return true;
		}

		public bool MouseMove(int x, int y)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			if (mDownButtons != 0)
			{
				return MouseDrag(x, y);
			}
			mMouseIn = true;
			MousePosition(x, y);
			return true;
		}

		public bool MouseDrag(int x, int y)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			mMouseIn = true;
			mLastMouseX = x;
			mLastMouseY = y;
			if (mOverWidget != null && mOverWidget != mLastDownWidget)
			{
				Widget theWidget = mOverWidget;
				mOverWidget = null;
				MouseLeave(theWidget);
			}
			if (mLastDownWidget != null)
			{
				Point absPos = mLastDownWidget.GetAbsPos();
				int x2 = x - absPos.mX;
				int y2 = y - absPos.mY;
				mLastDownWidget.MouseDrag(x2, y2);
				int theWidgetX = 0;
				int theWidgetY = 0;
				Widget widgetAt = GetWidgetAt(x, y, ref theWidgetX, ref theWidgetY);
				if (widgetAt == mLastDownWidget && widgetAt != null)
				{
					if (mOverWidget == null)
					{
						mOverWidget = mLastDownWidget;
						MouseEnter(mOverWidget);
					}
				}
				else if (mOverWidget != null)
				{
					Widget theWidget2 = mOverWidget;
					mOverWidget = null;
					MouseLeave(theWidget2);
				}
			}
			return true;
		}

		public bool MouseExit(int x, int y)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			mMouseIn = false;
			if (mOverWidget != null)
			{
				MouseLeave(mOverWidget);
				mOverWidget = null;
			}
			return true;
		}

		public void MouseWheel(int theDelta)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			if (mFocusWidget != null)
			{
				mFocusWidget.MouseWheel(theDelta);
			}
		}

		public int KeyChar(sbyte theChar)
		{
			return 0;
		}

		public bool KeyDown(KeyCode key)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			if (key >= KeyCode.KEYCODE_UNKNOWN && key < (KeyCode)255)
			{
				mKeyDown[(int)key] = true;
			}
			if (mFocusWidget != null)
			{
				mFocusWidget.KeyDown(key);
			}
			return true;
		}

		public bool KeyUp(KeyCode key)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			if (key >= KeyCode.KEYCODE_UNKNOWN && key < (KeyCode)255)
			{
				mKeyDown[(int)key] = false;
			}
			if (key == KeyCode.KEYCODE_TAB && mKeyDown[17])
			{
				return true;
			}
			if (mFocusWidget != null)
			{
				mFocusWidget.KeyUp(key);
			}
			return true;
		}

		public bool IsLeftButtonDown()
		{
			return false;
		}

		public bool IsMiddleButtonDown()
		{
			return false;
		}

		public bool IsRightButtonDown()
		{
			return false;
		}

		public void TouchBegan(SexyAppBase.Touch touch)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			mActualDownButtons |= 1;
			MousePosition(touch.location.mX, touch.location.mY);
			int theWidgetX = 0;
			int theWidgetY = 0;
			Widget widgetAt = GetWidgetAt(touch.location.mX, touch.location.mY, ref theWidgetX, ref theWidgetY);
			if (mLastDownWidget != null)
			{
				widgetAt = mLastDownWidget;
			}
			if (widgetAt != null)
			{
				Point absPos = widgetAt.GetAbsPos();
				touch.location.mX -= absPos.mX;
				touch.location.mY -= absPos.mY;
				touch.previousLocation.mX -= absPos.mX;
				touch.previousLocation.mY -= absPos.mY;
			}
			mLastDownButtonId = 1;
			mDownButtons |= 1;
			mLastDownWidget = widgetAt;
			if (widgetAt != null)
			{
				if (widgetAt.WantsFocus())
				{
					SetFocus(widgetAt);
				}
				widgetAt.mIsDown = true;
				widgetAt.TouchBegan(touch);
			}
		}

		public void TouchMoved(SexyAppBase.Touch touch)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			mMouseIn = true;
			mLastMouseX = touch.location.mX;
			mLastMouseY = touch.location.mY;
			if (mLastDownWidget == null)
			{
				return;
			}
			int theWidgetX = 0;
			int theWidgetY = 0;
			Widget widgetAt = GetWidgetAt(touch.location.mX, touch.location.mY, ref theWidgetX, ref theWidgetY);
			Point absPos = mLastDownWidget.GetAbsPos();
			touch.location.mX -= absPos.mX;
			touch.location.mY -= absPos.mY;
			touch.previousLocation.mX -= absPos.mX;
			touch.previousLocation.mY -= absPos.mY;
			mLastDownWidget.TouchMoved(touch);
			if (widgetAt == mLastDownWidget && widgetAt != null)
			{
				if (mOverWidget == null)
				{
					mOverWidget = mLastDownWidget;
					MouseEnter(mOverWidget);
				}
			}
			else if (mOverWidget != null)
			{
				Widget theWidget = mOverWidget;
				mOverWidget = null;
				MouseLeave(theWidget);
			}
		}

		public void TouchEnded(SexyAppBase.Touch touch)
		{
			mLastInputUpdateCnt = mUpdateCnt;
			int num = 1;
			mActualDownButtons &= ~num;
			if (mLastDownWidget != null && (mDownButtons & num) != 0)
			{
				Widget widget = mLastDownWidget;
				mDownButtons &= ~num;
				if (mDownButtons == 0)
				{
					mLastDownWidget = null;
				}
				Point absPos = widget.GetAbsPos();
				touch.location.mX -= absPos.mX;
				touch.location.mY -= absPos.mY;
				touch.previousLocation.mX -= absPos.mX;
				touch.previousLocation.mY -= absPos.mY;
				widget.mIsDown = false;
				widget.TouchEnded(touch);
			}
			else
			{
				mDownButtons &= ~num;
			}
			MousePosition((int)GlobalMembers.NO_TOUCH_MOUSE_POS.X, (int)GlobalMembers.NO_TOUCH_MOUSE_POS.Y);
		}

		public void TouchesCanceled()
		{
		}

		public Widget GetGamepadSelection()
		{
			return mGamepadSelectionWidget;
		}

		public void SetGamepadSelection(Widget theSelectedWidget, WidgetLinkDir theDirection)
		{
			Widget mGamepadSelectionWidget2 = mGamepadSelectionWidget;
		}

		public void GamepadButtonDown(GamepadButton theButton, int thePlayer, uint theFlags)
		{
		}

		public void GamepadButtonUp(GamepadButton theButton, int thePlayer, uint theFlags)
		{
		}

		public void GamepadAxisMove(GamepadAxis theAxis, int thePlayer, float theAxisValue)
		{
		}

		public IGamepad GetGamepadForPlayer(int thePlayer)
		{
			return null;
		}

		public int GetWidgetFlags()
		{
			if (!mHasFocus)
			{
				return FlagsMod.GetModFlags(mWidgetFlags, mLostFocusFlagsMod);
			}
			return mWidgetFlags;
		}

		protected void MouseEnter(Widget theWidget)
		{
			theWidget.mIsOver = true;
			theWidget.MouseEnter();
			if (theWidget.mDoFinger)
			{
				theWidget.ShowFinger(true);
			}
		}

		protected void MouseLeave(Widget theWidget)
		{
			theWidget.mIsOver = false;
			theWidget.MouseLeave();
			if (theWidget.mDoFinger)
			{
				theWidget.ShowFinger(false);
			}
		}

		protected void SetBaseModal(Widget theWidget, FlagsMod theBelowFlagsMod)
		{
			mBaseModalWidget = theWidget;
			mBelowModalFlagsMod = theBelowFlagsMod;
			if (mOverWidget != null && (mBelowModalFlagsMod.mRemoveFlags & 0x10) != 0 && IsBelow(mOverWidget, mBaseModalWidget))
			{
				Widget theWidget2 = mOverWidget;
				mOverWidget = null;
				MouseLeave(theWidget2);
			}
			if (mLastDownWidget != null && (mBelowModalFlagsMod.mRemoveFlags & 0x10) != 0 && IsBelow(mLastDownWidget, mBaseModalWidget))
			{
				Widget theWidget3 = mLastDownWidget;
				int theDownCode = mDownButtons;
				mDownButtons = 0;
				mLastDownWidget = null;
				DoMouseUps(theWidget3, theDownCode);
			}
			if (mFocusWidget != null && (mBelowModalFlagsMod.mRemoveFlags & 0x20) != 0 && IsBelow(mFocusWidget, mBaseModalWidget))
			{
				Widget widget = mFocusWidget;
				mFocusWidget = null;
				widget.LostFocus();
			}
		}
	}
}
