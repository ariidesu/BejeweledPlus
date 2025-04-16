using System;
using System.Linq;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3ScrollWidget : ScrollWidget
	{
		private bool mTouched;

		public bool mLocked;

		public bool mAllowScrolling;

		public int mScrollDownOffset;

		public Bej3ScrollWidget(Bej3ScrollWidgetListener listener, bool useIndicators)
			: base(listener)
		{
			mLocked = false;
			mAllowScrolling = true;
			mTouched = false;
			mScrollDownOffset = 0;
			if (useIndicators)
			{
				EnableIndicators(GlobalMembersResourcesWP.IMAGE_DIALOG_SCROLLBAR);
				SetIndicatorsInsets(new Insets(ConstantsWP.SCROLLWIDGET_INSET_X, ConstantsWP.SCROLLWIDGET_INSET_Y, ConstantsWP.SCROLLWIDGET_INSET_X, ConstantsWP.SCROLLWIDGET_INSET_Y));
			}
		}

		public Bej3ScrollWidget(Bej3ScrollWidgetListener listener)
			: base(listener)
		{
			bool flag = true;
			mLocked = false;
			mAllowScrolling = true;
			mTouched = false;
			mScrollDownOffset = 0;
			if (flag)
			{
				EnableIndicators(GlobalMembersResourcesWP.IMAGE_DIALOG_SCROLLBAR);
				SetIndicatorsInsets(new Insets(ConstantsWP.SCROLLWIDGET_INSET_X, ConstantsWP.SCROLLWIDGET_INSET_Y, ConstantsWP.SCROLLWIDGET_INSET_X, ConstantsWP.SCROLLWIDGET_INSET_Y));
			}
		}

		public void AllowScrolling(bool allow)
		{
			mAllowScrolling = allow;
		}

		public override void TouchBegan(SexyAppBase.Touch touch)
		{
			if (!mTouched)
			{
				if (mClient != null)
				{
					bool flag = true;
					if (mSeekScrollTarget)
					{
						if (mListener != null)
						{
							mListener.ScrollTargetInterrupted(this);
						}
						if (mPagingEnabled)
						{
							flag = false;
						}
					}
					mScrollTouchReference = new FPoint(touch.location.mX, touch.location.mY);
					mScrollOffsetReference = new FPoint(mClient.mX, mClient.mY);
					mScrollOffset = new FPoint(mScrollOffsetReference);
					mScrollLastTimestamp = touch.timestamp;
					mScrollTracking = false;
					if (flag)
					{
						mSeekScrollTarget = false;
					}
					mClientLastDown = GetClientWidgetAt(touch);
					mClientLastDown.mIsDown = true;
					mClientLastDown.mIsOver = true;
					mClientLastDown.TouchBegan(touch);
				}
				MarkDirty();
			}
			mTouched = true;
		}

		public override void TouchMoved(SexyAppBase.Touch touch)
		{
			if (!mAllowScrolling)
			{
				return;
			}
			bool flag = false;
			Bej3ScrollWidget bej3ScrollWidget = null;
			if (mClientLastDown != null)
			{
				bej3ScrollWidget = mClientLastDown as Bej3ScrollWidget;
			}
			if (bej3ScrollWidget != null)
			{
				flag = bej3ScrollWidget.mLocked;
			}
			FPoint fPoint = new FPoint(touch.location.mX, touch.location.mY) - mScrollTouchReference;
			fPoint.mY += mScrollDownOffset;
			if (flag)
			{
				fPoint.mX = (fPoint.mY = 0f);
			}
			if (mClient != null)
			{
				if (!mScrollTracking && (mScrollPractical & ScrollMode.SCROLL_HORIZONTAL) != 0 && Math.Abs(fPoint.mX) > (float)ConstantsWP.BEJ3SCROLLWIDGET_TOLERANCE)
				{
					mScrollTouchReference.mX = touch.location.mX;
					mScrollTracking = true;
					mLocked = true;
				}
				if (!mScrollTracking && (mScrollPractical & ScrollMode.SCROLL_VERTICAL) != 0 && Math.Abs(fPoint.mY) > (float)ConstantsWP.BEJ3SCROLLWIDGET_TOLERANCE)
				{
					mScrollTouchReference.mY = touch.location.mY;
					mScrollTracking = true;
					mLocked = true;
				}
				if (mScrollTracking && mClientLastDown != null)
				{
					mClientLastDown.TouchesCanceled();
					mClientLastDown.mIsDown = false;
					mClientLastDown = null;
				}
			}
			if (mScrollTracking)
			{
				TouchMotion(touch);
			}
			else if (mClientLastDown != null)
			{
				Point point = GetAbsPos() - mClientLastDown.GetAbsPos();
				Point point2 = new Point(touch.location.mX, touch.location.mY);
				Point point3 = point2 + point;
				Point thePoint = new Point(point3.mX + mClientLastDown.mX, point3.mY + mClientLastDown.mY);
				bool flag2 = mClientLastDown.GetInsetRect().Contains(thePoint);
				if (flag2 && !mClientLastDown.mIsOver)
				{
					mClientLastDown.mIsOver = true;
					mClientLastDown.MouseEnter();
				}
				else if (!flag2 && mClientLastDown.mIsOver)
				{
					mClientLastDown.MouseLeave();
					mClientLastDown.mIsOver = false;
				}
				touch.location.mX += point.mX;
				touch.location.mY += point.mY;
				touch.previousLocation.mX += point.mX;
				touch.location.mY += point.mY;
				mClientLastDown.TouchMoved(touch);
			}
			MarkDirty();
		}

		public override void TouchEnded(SexyAppBase.Touch touch)
		{
			mLocked = false;
			mTouched = false;
			if (mScrollTracking)
			{
				TouchMotion(touch);
				mScrollTracking = false;
				if (mPagingEnabled)
				{
					SnapToCurrentPage();
				}
			}
			else if (mClientLastDown != null)
			{
				Point point = GetAbsPos() - mClientLastDown.GetAbsPos();
				Point point2 = new Point(touch.location.mX, touch.location.mY);
				Point point3 = point2 + point;
				touch.location.mX += point.mX;
				touch.location.mY += point.mY;
				touch.previousLocation.mX += point.mX;
				touch.location.mY += point.mY;
				mClientLastDown.TouchEnded(touch);
				mClientLastDown.mIsDown = false;
				mClientLastDown = null;
			}
			MarkDirty();
		}

		public override void TouchesCanceled()
		{
			mTouched = false;
			mIsDown = false;
			base.TouchesCanceled();
			SnapToCurrentPage();
		}

		public void SnapToCurrentPage()
		{
			SnapToPage();
			((Bej3ScrollWidgetListener)mListener).PageChanged(this, GetPageHorizontal(), GetPageVertical());
		}

		public override void Update()
		{
			SetPage(GetPageHorizontal(), GetPageVertical(), true);
			base.Update();
		}

		public int GetPageCount()
		{
			return mPageCountHorizontal;
		}

		public int GetCurrentPage()
		{
			return mCurrentPageVertical;
		}

		public void SetPageHorizontalAfterInterrupt(int page)
		{
			mTouched = true;
			SetPageHorizontal(page, true);
		}

		public SexyFramework.Widget.Widget GetDirectChildAt(SexyAppBase.Touch touch)
		{
			SexyFramework.Widget.Widget widget = GetClientWidgetAt(touch);
			SexyFramework.Widget.Widget widget2 = mWidgets.First();
			while (widget.mParent != null && widget.mParent != widget2)
			{
				widget = (SexyFramework.Widget.Widget)widget.mParent;
			}
			return widget;
		}

		public bool IsScrolling()
		{
			return mSeekScrollTarget;
		}

		public bool HasReachedScrollTarget()
		{
			return mScrollOffset == mScrollTarget;
		}

		public FPoint GetScrollVelocity()
		{
			return new FPoint(mScrollVelocity);
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
		}
	}
}
