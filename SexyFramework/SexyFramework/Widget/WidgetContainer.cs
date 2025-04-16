using System;
using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class WidgetContainer : IDisposable
	{
		public LinkedList<Widget> mWidgets = new LinkedList<Widget>();

		public WidgetManager mWidgetManager;

		public WidgetContainer mParent;

		public bool mUpdateIteratorModified;

		public LinkedListNode<Widget> mUpdateIterator;

		public int mLastWMUpdateCount;

		public int mUpdateCnt;

		public int mX;

		public int mY;

		public int mWidth;

		public int mHeight;

		public Rect mRect = default(Rect);

		public Rect mHelperRect = default(Rect);

		public int mPriority;

		public int mZOrder;

		public bool mDirty;

		public bool mHasAlpha;

		public bool mClip;

		public FlagsMod mWidgetFlagsMod = new FlagsMod();

		private static int aDepthCount;

		public event SystemButtonPressed SystemButtonPressed;

		public WidgetContainer()
		{
			mX = 0;
			mY = 0;
			mWidth = 0;
			mHeight = 0;
			mParent = null;
			mWidgetManager = null;
			mUpdateIteratorModified = false;
			mLastWMUpdateCount = 0;
			mUpdateCnt = 0;
			mDirty = false;
			mHasAlpha = false;
			mClip = true;
			mPriority = 0;
			mZOrder = 0;
			mUpdateIterator = null;
			SystemButtonPressed += Default_SystemButtonPressed;
		}

		public void SignalSystemButtonPressed(SystemButtonPressedArgs args)
		{
			this.SystemButtonPressed(args);
		}

		private void Default_SystemButtonPressed(SystemButtonPressedArgs args)
		{
		}

		public virtual void Dispose()
		{
		}

		public void CopyFrom(WidgetContainer rhs)
		{
			if (rhs != null)
			{
				mWidgetManager = rhs.mWidgetManager;
				mParent = rhs.mParent;
				mUpdateIteratorModified = rhs.mUpdateIteratorModified;
				mLastWMUpdateCount = rhs.mLastWMUpdateCount;
				mUpdateCnt = rhs.mUpdateCnt;
				mX = rhs.mX;
				mY = rhs.mY;
				mWidth = rhs.mWidth;
				mHeight = rhs.mHeight;
				mPriority = rhs.mPriority;
				mZOrder = rhs.mZOrder;
				mDirty = rhs.mDirty;
				mHasAlpha = rhs.mHasAlpha;
				mClip = rhs.mClip;
				mUpdateIterator = rhs.mUpdateIterator;
				mWidgetFlagsMod.mAddFlags = rhs.mWidgetFlagsMod.mAddFlags;
				mWidgetFlagsMod.mRemoveFlags = rhs.mWidgetFlagsMod.mRemoveFlags;
				mRect.SetValue(rhs.mRect.mX, rhs.mRect.mY, rhs.mRect.mWidth, rhs.mRect.mWidth);
				mHelperRect.SetValue(rhs.mHelperRect.mX, rhs.mHelperRect.mY, rhs.mHelperRect.mWidth, rhs.mHelperRect.mWidth);
				mWidgets.Clear();
				LinkedList<Widget>.Enumerator enumerator = rhs.mWidgets.GetEnumerator();
				while (enumerator.MoveNext())
				{
					mWidgets.AddLast(enumerator.Current);
				}
			}
		}

		public virtual Rect GetRect()
		{
			mRect.mX = mX;
			mRect.mY = mY;
			mRect.mWidth = mWidth;
			mRect.mHeight = mHeight;
			return mRect;
		}

		public virtual bool Intersects(WidgetContainer theWidget)
		{
			return GetRect().Intersects(theWidget.GetRect());
		}

		public virtual void AddWidget(Widget theWidget)
		{
			if (!mWidgets.Contains(theWidget))
			{
				InsertWidgetHelper(mWidgets.Last, theWidget);
				theWidget.mWidgetManager = mWidgetManager;
				theWidget.mParent = this;
				if (mWidgetManager != null)
				{
					theWidget.AddedToManager(mWidgetManager);
					theWidget.MarkDirtyFull();
					mWidgetManager.RehupMouse();
				}
				MarkDirty();
			}
		}

		public virtual void RemoveWidget(Widget theWidget)
		{
			if (mWidgets.Contains(theWidget))
			{
				LinkedListNode<Widget> linkedListNode = mWidgets.Find(theWidget);
				theWidget.WidgetRemovedHelper();
				theWidget.mParent = null;
				if (linkedListNode == mUpdateIterator)
				{
					mUpdateIterator = linkedListNode.Next;
					mUpdateIteratorModified = true;
				}
				mWidgets.Remove(linkedListNode);
			}
		}

		public virtual bool HasWidget(Widget theWidget)
		{
			return mWidgets.Contains(theWidget);
		}

		public virtual void DisableWidget(Widget theWidget)
		{
		}

		public virtual void RemoveAllWidgets(bool doDelete, bool recursive)
		{
			while (mWidgets.Count > 0)
			{
				Widget value = mWidgets.First.Value;
				RemoveWidget(value);
				if (recursive)
				{
					value.RemoveAllWidgets(doDelete, recursive);
				}
				if (doDelete)
				{
					value?.Dispose();
				}
			}
		}

		public virtual void SetFocus(Widget theWidget)
		{
		}

		public virtual bool IsBelow(Widget theWidget1, Widget theWidget2)
		{
			bool found = false;
			return IsBelowHelper(theWidget1, theWidget2, ref found);
		}

		public virtual void MarkAllDirty()
		{
			MarkDirty();
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.mDirty = true;
				enumerator.Current.MarkAllDirty();
			}
		}

		public virtual void BringToFront(Widget theWidget)
		{
			if (mWidgets.Contains(theWidget))
			{
				LinkedListNode<Widget> linkedListNode = mWidgets.Find(theWidget);
				if (linkedListNode == mUpdateIterator)
				{
					mUpdateIterator = mUpdateIterator.Next;
					mUpdateIteratorModified = true;
				}
				mWidgets.Remove(linkedListNode);
				InsertWidgetHelper(mWidgets.Last, theWidget);
				theWidget.OrderInManagerChanged();
			}
		}

		public virtual void BringToBack(Widget theWidget)
		{
			if (mWidgets.Contains(theWidget))
			{
				LinkedListNode<Widget> linkedListNode = mWidgets.Find(theWidget);
				if (linkedListNode == mUpdateIterator)
				{
					mUpdateIterator = mUpdateIterator.Next;
					mUpdateIteratorModified = true;
				}
				mWidgets.Remove(linkedListNode);
				InsertWidgetHelper(mWidgets.First, theWidget);
				theWidget.OrderInManagerChanged();
			}
		}

		public virtual void PutBehind(Widget theWidget, Widget theRefWidget)
		{
			if (theRefWidget != null)
			{
				theWidget.mZOrder = theRefWidget.mZOrder;
			}
			if (mWidgets.Contains(theWidget))
			{
				LinkedListNode<Widget> linkedListNode = mWidgets.Find(theWidget);
				if (linkedListNode == mUpdateIterator)
				{
					mUpdateIterator = mUpdateIterator.Next;
					mUpdateIteratorModified = true;
				}
				mWidgets.Remove(linkedListNode);
				LinkedListNode<Widget> where = mWidgets.Find(theRefWidget);
				InsertWidgetHelper(where, theWidget);
				theWidget.OrderInManagerChanged();
			}
		}

		public virtual void PutInfront(Widget theWidget, Widget theRefWidget)
		{
			if (theRefWidget != null)
			{
				theWidget.mZOrder = theRefWidget.mZOrder;
			}
			if (mWidgets.Contains(theWidget))
			{
				LinkedListNode<Widget> linkedListNode = mWidgets.Find(theWidget);
				if (linkedListNode == mUpdateIterator)
				{
					mUpdateIterator = mUpdateIterator.Next;
					mUpdateIteratorModified = true;
				}
				mWidgets.Remove(linkedListNode);
				LinkedListNode<Widget> linkedListNode2 = mWidgets.Find(theRefWidget);
				linkedListNode2 = linkedListNode2.Next;
				InsertWidgetHelper(linkedListNode2, theWidget);
				theWidget.OrderInManagerChanged();
			}
		}

		public virtual Point GetAbsPos()
		{
			if (mParent == null)
			{
				return new Point(mX, mY);
			}
			return new Point(mX + mParent.GetAbsPos().mX, mY + mParent.GetAbsPos().mY);
		}

		public virtual void MarkDirty()
		{
			if (mParent != null)
			{
				mParent.MarkDirty(this);
			}
			else
			{
				mDirty = true;
			}
		}

		public virtual void MarkDirty(WidgetContainer theWidget)
		{
			if (theWidget.mDirty)
			{
				return;
			}
			MarkDirty();
			theWidget.mDirty = true;
			if (mParent != null)
			{
				return;
			}
			if (theWidget.mHasAlpha)
			{
				MarkDirtyFull(theWidget);
				return;
			}
			bool flag = false;
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				if (current == theWidget)
				{
					flag = true;
				}
				else if (flag && current.mVisible && current.Intersects(theWidget))
				{
					MarkDirty(current);
				}
			}
		}

		public virtual void MarkDirtyFull()
		{
			if (mParent != null)
			{
				mParent.MarkDirtyFull(this);
			}
			else
			{
				mDirty = true;
			}
		}

		public virtual void MarkDirtyFull(WidgetContainer theWidget)
		{
			MarkDirtyFull();
			theWidget.mDirty = true;
			if (mParent != null)
			{
				return;
			}
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			LinkedListNode<Widget> linkedListNode = null;
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == theWidget)
				{
					linkedListNode = mWidgets.Find(enumerator.Current);
					break;
				}
			}
			if (linkedListNode == null)
			{
				return;
			}
			LinkedListNode<Widget> linkedListNode2 = linkedListNode;
			for (linkedListNode2 = linkedListNode2.Previous; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Previous)
			{
				Widget value = linkedListNode2.Value;
				if (value.mVisible)
				{
					if (value.mHasTransparencies && value.mHasAlpha)
					{
						mHelperRect.setValue(0, 0, mWidth, mHeight);
						Rect rect = theWidget.GetRect().Intersection(mHelperRect);
						if (value.Contains(rect.mX, rect.mY) && value.Contains(rect.mX + rect.mWidth - 1, rect.mY + rect.mHeight - 1))
						{
							value.MarkDirty();
							break;
						}
					}
					if (value.Intersects(theWidget))
					{
						MarkDirty(value);
					}
				}
			}
			linkedListNode2 = linkedListNode;
			while (linkedListNode2.Next != null)
			{
				Widget value2 = linkedListNode2.Value;
				if (value2.mVisible && value2.Intersects(theWidget))
				{
					MarkDirty(value2);
				}
				linkedListNode2 = linkedListNode2.Next;
			}
		}

		public virtual void AddedToManager(WidgetManager theWidgetManager)
		{
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				current.mWidgetManager = theWidgetManager;
				current.AddedToManager(theWidgetManager);
				MarkDirty();
			}
		}

		public virtual void RemovedFromManager(WidgetManager theWidgetManager)
		{
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				theWidgetManager.DisableWidget(current);
				current.RemovedFromManager(theWidgetManager);
				current.mWidgetManager = null;
			}
			if (theWidgetManager.mPopupCommandWidget == this)
			{
				theWidgetManager.mPopupCommandWidget = null;
			}
		}

		public virtual void Update()
		{
			mUpdateCnt++;
		}

		public virtual void UpdateAll(ModalFlags theFlags)
		{
			if ((theFlags.GetFlags() & 2) != 0)
			{
				MarkDirty();
			}
			WidgetManager widgetManager = mWidgetManager;
			if (widgetManager == null)
			{
				return;
			}
			if ((theFlags.GetFlags() & 1) != 0 && mLastWMUpdateCount != mWidgetManager.mUpdateCnt)
			{
				mLastWMUpdateCount = mWidgetManager.mUpdateCnt;
				Update();
			}
			mUpdateIterator = mWidgets.First;
			while (mUpdateIterator != null)
			{
				mUpdateIteratorModified = false;
				Widget value = mUpdateIterator.Value;
				if (value == widgetManager.mBaseModalWidget)
				{
					theFlags.mIsOver = true;
				}
				value.UpdateAll(theFlags);
				if (!mUpdateIteratorModified)
				{
					mUpdateIterator = mUpdateIterator.Next;
				}
			}
			mUpdateIteratorModified = true;
		}

		public virtual void UpdateF(float theFrac)
		{
		}

		public virtual void UpdateFAll(ModalFlags theFlags, float theFrac)
		{
			WidgetManager widgetManager = mWidgetManager;
			if (widgetManager == null)
			{
				return;
			}
			if ((theFlags.GetFlags() & 1) != 0)
			{
				UpdateF(theFrac);
			}
			mUpdateIterator = mWidgets.First;
			while (mUpdateIterator != null)
			{
				mUpdateIteratorModified = false;
				Widget value = mUpdateIterator.Value;
				if (value == widgetManager.mBaseModalWidget)
				{
					theFlags.mIsOver = true;
				}
				value.UpdateFAll(theFlags, theFrac);
				if (!mUpdateIteratorModified)
				{
					mUpdateIterator = mUpdateIterator.Next;
				}
			}
			mUpdateIteratorModified = true;
		}

		public virtual int GetScreenX()
		{
			int num = mX;
			if (mParent != null)
			{
				num += mParent.GetScreenX();
			}
			return num;
		}

		public virtual int GetScreenY()
		{
			int num = mY;
			if (mParent != null)
			{
				num += mParent.GetScreenY();
			}
			return num;
		}

		public virtual bool IsVisible()
		{
			int screenX = GetScreenX();
			int screenY = GetScreenY();
			bool result = true;
			if (screenX + mWidth < 0 || screenY + mHeight < 0 || screenX > GlobalMembers.gSexyApp.mScreenBounds.mWidth || screenY > GlobalMembers.gSexyApp.mScreenBounds.mHeight)
			{
				result = false;
			}
			return result;
		}

		public virtual void Draw(SexyFramework.Graphics.Graphics g)
		{
		}

		public virtual void DrawAll(ModalFlags theFlags, SexyFramework.Graphics.Graphics g)
		{
			if (!IsVisible())
			{
				return;
			}
			if (mWidgetManager != null && mPriority > mWidgetManager.mMinDeferredOverlayPriority)
			{
				mWidgetManager.FlushDeferredOverlayWidgets(mPriority);
			}
			if (mClip && (theFlags.GetFlags() & 8) != 0)
			{
				g.ClipRect(0, 0, mWidth, mHeight);
			}
			if (mWidgets.Count == 0)
			{
				if ((theFlags.GetFlags() & 4) != 0)
				{
					Draw(g);
				}
				return;
			}
			if ((theFlags.GetFlags() & 4) != 0)
			{
				g.PushState();
				Draw(g);
				g.PopState();
			}
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				if (current.mVisible)
				{
					if (mWidgetManager != null && current == mWidgetManager.mBaseModalWidget)
					{
						theFlags.mIsOver = true;
					}
					g.PushState();
					g.Translate(current.mX, current.mY);
					current.DrawAll(theFlags, g);
					current.mDirty = false;
					g.PopState();
				}
			}
		}

		public virtual void SysColorChangedAll()
		{
			SysColorChanged();
			if (mWidgets.Count > 0)
			{
				aDepthCount++;
			}
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				current.SysColorChangedAll();
			}
		}

		public virtual void SysColorChanged()
		{
		}

		public Widget GetWidgetAtHelper(int x, int y, int theFlags, ref bool found, ref int theWidgetX, ref int theWidgetY)
		{
			bool flag = false;
			FlagsMod.ModFlags(ref theFlags, mWidgetFlagsMod);
			for (LinkedListNode<Widget> linkedListNode = mWidgets.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
			{
				Widget value = linkedListNode.Value;
				int theFlags2 = theFlags;
				FlagsMod.ModFlags(ref theFlags2, value.mWidgetFlagsMod);
				if (flag)
				{
					FlagsMod.ModFlags(ref theFlags2, mWidgetManager.mBelowModalFlagsMod);
				}
				if ((theFlags2 & 0x10) != 0 && value.mVisible)
				{
					bool found2 = false;
					Widget widgetAtHelper = value.GetWidgetAtHelper(x - value.mX, y - value.mY, theFlags2, ref found2, ref theWidgetX, ref theWidgetY);
					if (widgetAtHelper != null || found2)
					{
						found = true;
						return widgetAtHelper;
					}
					if (value.mMouseVisible && value.GetInsetRect().Contains(x, y))
					{
						found = true;
						if (value.IsPointVisible(x - value.mX, y - value.mY))
						{
							theWidgetX = x - value.mX;
							theWidgetY = y - value.mY;
							return value;
						}
					}
				}
				flag |= value == mWidgetManager.mBaseModalWidget;
			}
			found = false;
			return null;
		}

		public bool IsBelowHelper(Widget theWidget1, Widget theWidget2, ref bool found)
		{
			LinkedList<Widget>.Enumerator enumerator = mWidgets.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Widget current = enumerator.Current;
				if (current == theWidget1)
				{
					found = true;
					return true;
				}
				if (current == theWidget2)
				{
					found = true;
					return false;
				}
				bool result = current.IsBelowHelper(theWidget1, theWidget2, ref found);
				if (found)
				{
					return result;
				}
			}
			return false;
		}

		public void InsertWidgetHelper(LinkedListNode<Widget> where, Widget theWidget)
		{
			LinkedListNode<Widget> linkedListNode;
			for (linkedListNode = where; linkedListNode != mWidgets.Last; linkedListNode = linkedListNode.Next)
			{
				Widget value = linkedListNode.Value;
				if (value.mZOrder >= theWidget.mZOrder)
				{
					if (linkedListNode != mWidgets.First)
					{
						value = linkedListNode.Previous.Value;
						if (value.mZOrder > theWidget.mZOrder)
						{
							break;
						}
					}
					mWidgets.AddAfter(linkedListNode, theWidget);
					return;
				}
			}
			while (linkedListNode != mWidgets.First)
			{
				linkedListNode = linkedListNode.Previous;
				Widget value2 = linkedListNode.Value;
				if (value2.mZOrder <= theWidget.mZOrder)
				{
					mWidgets.AddAfter(linkedListNode.Next, theWidget);
					return;
				}
			}
			mWidgets.AddFirst(theWidget);
		}
	}
}
