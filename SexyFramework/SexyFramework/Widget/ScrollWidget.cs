using System;
using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class ScrollWidget : Widget, ProxyWidgetListener, IDisposable
	{
		public enum ScrollMode
		{
			SCROLL_DISABLED,
			SCROLL_HORIZONTAL,
			SCROLL_VERTICAL,
			SCROLL_BOTH
		}

		public enum Colors
		{
			COLOR_BACKGROUND_COLOR
		}

		protected class Overlay
		{
			public Image image;

			public Point offset;
		}

		private static readonly int FRAMEWORK_UPDATE_RATE = 30;

		private static float SCROLL_TARGET_THRESHOLD_NORM = 0.01f;

		private static float SCROLL_VELOCITY_THRESHOLD_NORM = 0.0001f;

		private static float SCROLL_DEVIATION_DAMPING = 0.5f;

		private static float SCROLL_SPRINGBACK_TENSION = 0.1f;

		private static float SCROLL_VELOCITY_FILTER_WINDOW = 0.1f;

		private static float SCROLL_VELOCITY_DAMPING = 0.975f;

		private static float SCROLL_VELOCITY_DEVIATION_DAMPING = 0.85f;

		private static float SCROLL_DRAG_THRESHOLD = 4f;

		private static float SCROLL_PAGE_FLICK_THRESHOLD = 10f;

		private static float SCROLL_INDICATORS_FADE_IN_RATE = 1f / (0.2f * (float)FRAMEWORK_UPDATE_RATE);

		private static float SCROLL_INDICATORS_FADE_OUT_RATE = 1f / (0.5f * (float)FRAMEWORK_UPDATE_RATE);

		private static int SCROLL_INDICATORS_FLASH_TICKS = ticksForSeconds(1f);

		protected ScrollWidgetListener mListener;

		protected Widget mClient;

		protected Widget mClientLastDown;

		protected PageControl mPageControl;

		protected ProxyWidget mIndicatorsProxy;

		protected Image mIndicatorsImage;

		protected Image mBackgroundImage;

		protected bool mFillBackground;

		protected List<Overlay> mOverlays = new List<Overlay>();

		protected bool mDrawOverlays;

		protected ScrollMode mScrollMode;

		protected Insets mScrollInsets;

		protected FPoint mScrollTarget = new FPoint(0f, 0f);

		protected FPoint mScrollOffset = new FPoint(0f, 0f);

		protected FPoint mScrollVelocity = new FPoint(0f, 0f);

		protected FPoint mScrollTouchReference = new FPoint(0f, 0f);

		protected FPoint mScrollOffsetReference = new FPoint(0f, 0f);

		protected bool mBounceEnabled;

		protected bool mPagingEnabled;

		protected bool mIndicatorsEnabled;

		protected Insets mIndicatorsInsets = new Insets();

		protected int mIndicatorsFlashTimer;

		protected float mIndicatorsOpacity;

		protected int mCurrentPageHorizontal;

		protected int mCurrentPageVertical;

		protected bool mSeekScrollTarget;

		protected bool mScrollTracking;

		protected double mScrollLastTimestamp;

		protected FPoint mScrollMin = default(FPoint);

		protected FPoint mScrollMax = default(FPoint);

		protected FPoint mPageSize = default(FPoint);

		protected ScrollMode mScrollPractical;

		protected int mPageCountHorizontal;

		protected int mPageCountVertical;

		public bool mDragEnabled { get; set; }

		private static int ticksForSeconds(float seconds)
		{
			return (int)((float)FRAMEWORK_UPDATE_RATE * seconds);
		}

		private static float VectorNorm(FPoint v)
		{
			return v.mX * v.mX + v.mY * v.mY;
		}

		private static FPoint PointAddScaled(FPoint augend, FPoint addend, float factor, ref FPoint data)
		{
			float p = augend.mX + addend.mX * factor;
			float p_ = augend.mY + addend.mY * factor;
			data.SetValue(p, p_);
			return data;
		}

		public ScrollWidget(ScrollWidgetListener listener)
		{
			Init(listener);
		}

		public ScrollWidget()
		{
			Init(null);
		}

		public int GetScrollMode()
		{
			return (int)mScrollMode;
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
		}

		public void SetPageControl(PageControl pageControl)
		{
			mPageControl = pageControl;
			if (mPagingEnabled)
			{
				mPageControl.SetNumberOfPages(mPageCountHorizontal);
			}
		}

		public void SetScrollMode(ScrollMode mode)
		{
			mScrollMode = mode;
			CacheDerivedValues();
		}

		public void SetScrollInsets(Insets insets)
		{
			mScrollInsets = new Insets(insets);
			CacheDerivedValues();
		}

		public FPoint GetScrollOffset()
		{
			return mScrollOffset;
		}

		public void SetScrollOffset(FPoint anOffset, bool animated)
		{
			if (animated)
			{
				mScrollTarget.SetValue(anOffset.mX, anOffset.mY);
				mSeekScrollTarget = true;
				return;
			}
			mScrollOffset.SetValue(anOffset.mX, anOffset.mY);
			mScrollVelocity.SetValue(0f, 0f);
			if (mClient != null)
			{
				mClient.Move((int)mScrollOffset.mX, (int)mScrollOffset.mY);
			}
		}

		public void ScrollToMin(bool animated)
		{
			SetScrollOffset(new FPoint(mScrollInsets.mLeft, mScrollInsets.mTop), animated);
		}

		public void ScrollToPoint(Point point, bool animated)
		{
			if (!mIsDown)
			{
				FPoint anOffset = new FPoint(-point.mX, -point.mY);
				SetScrollOffset(anOffset, animated);
			}
		}

		public void ScrollRectIntoView(Rect rect, bool animated)
		{
			if (!mIsDown)
			{
				float num = rect.mX + rect.mWidth;
				float num2 = rect.mY + rect.mHeight;
				float val = Math.Max(Math.Min(0f, mScrollMin.mX), -rect.mX);
				float val2 = Math.Max(Math.Min(0f, mScrollMin.mY), -rect.mY);
				float val3 = Math.Min(mScrollMax.mX, (float)mWidth - num);
				float val4 = Math.Min(mScrollMax.mY, (float)mHeight - num2);
				FPoint anOffset = new FPoint(Math.Min(val3, Math.Max(val, mScrollOffset.mX)), Math.Min(val4, Math.Max(val2, mScrollOffset.mY)));
				SetScrollOffset(anOffset, animated);
			}
		}

		public void EnableBounce(bool enable)
		{
			mBounceEnabled = enable;
		}

		public void EnablePaging(bool enable)
		{
			mPagingEnabled = enable;
		}

		public void EnableIndicators(Image indicatorsImage)
		{
			mIndicatorsImage = indicatorsImage;
			mIndicatorsEnabled = null != indicatorsImage;
			if (mIndicatorsEnabled && mIndicatorsProxy == null)
			{
				mIndicatorsProxy = new ProxyWidget(this);
				mIndicatorsProxy.mMouseVisible = false;
				mIndicatorsProxy.mZOrder = int.MaxValue;
				mIndicatorsProxy.Resize(0, 0, mWidth, mHeight);
				base.AddWidget(mIndicatorsProxy);
			}
			else if (!mIndicatorsEnabled && mIndicatorsProxy != null)
			{
				base.RemoveWidget(mIndicatorsProxy);
				mIndicatorsProxy = null;
			}
		}

		public void SetIndicatorsInsets(Insets insets)
		{
			mIndicatorsInsets = new Insets(insets);
		}

		public void FlashIndicators()
		{
			mIndicatorsFlashTimer = SCROLL_INDICATORS_FLASH_TICKS;
		}

		public void SetPageHorizontal(int page, bool animated)
		{
			SetPage(page, mCurrentPageVertical, animated);
		}

		public void SetPageVertical(int page, bool animated)
		{
			SetPage(mCurrentPageHorizontal, page, animated);
		}

		public void SetPage(int hpage, int vpage, bool animated)
		{
			if (mPagingEnabled)
			{
				mCurrentPageHorizontal = Math.Max(0, Math.Min(hpage, mPageCountHorizontal - 1));
				mCurrentPageVertical = Math.Max(0, Math.Min(vpage, mPageCountVertical - 1));
				FPoint anOffset = new FPoint((float)mScrollInsets.mLeft - (float)mCurrentPageHorizontal * mPageSize.mX, (float)mScrollInsets.mTop - (float)mCurrentPageVertical * mPageSize.mY);
				SetScrollOffset(anOffset, animated);
			}
		}

		public int GetPageHorizontal()
		{
			return mCurrentPageHorizontal;
		}

		public int GetPageVertical()
		{
			return mCurrentPageVertical;
		}

		public void SetBackgroundImage(Image image)
		{
			mBackgroundImage = image;
		}

		public void EnableBackgroundFill(bool enable)
		{
			mFillBackground = enable;
		}

		public void AddOverlayImage(Image image, Point anOffset)
		{
			mDrawOverlays = true;
			foreach (Overlay mOverlay in mOverlays)
			{
				if (mOverlay.image == image)
				{
					mOverlay.offset = anOffset;
					return;
				}
			}
			Overlay overlay = new Overlay();
			overlay.image = image;
			overlay.offset = anOffset;
			mOverlays.Add(overlay);
		}

		public void EnableOverlays(bool enable)
		{
			mDrawOverlays = enable;
		}

		public override void AddWidget(Widget theWidget)
		{
			if (mClient == null)
			{
				mClient = theWidget;
				mClient.mWidgetFlagsMod.mRemoveFlags |= 16;
				mClient.Move((int)mScrollOffset.mX, (int)mScrollOffset.mY);
				base.AddWidget(mClient);
				CacheDerivedValues();
			}
		}

		public override void RemoveWidget(Widget theWidget)
		{
			if (theWidget == mClient)
			{
				mClient = null;
			}
			base.RemoveWidget(theWidget);
		}

		public override void Resize(int x, int y, int width, int height)
		{
			base.Resize(x, y, width, height);
			if (mIndicatorsProxy != null)
			{
				mIndicatorsProxy.Resize(0, 0, width, height);
			}
			CacheDerivedValues();
		}

		public override void Resize(Rect frame)
		{
			base.Resize(frame);
		}

		public void ClientSizeChanged()
		{
			if (mClient != null)
			{
				CacheDerivedValues();
			}
		}

		public override void TouchBegan(SexyAppBase.Touch touch)
		{
			if (!mDragEnabled)
			{
				return;
			}
			if (mClient != null)
			{
				if (mSeekScrollTarget)
				{
					if (mListener != null)
					{
						mListener.ScrollTargetInterrupted(this);
					}
					if (mPagingEnabled && mPageControl != null)
					{
						mPageControl.SetCurrentPage(mCurrentPageHorizontal);
					}
				}
				mScrollTouchReference.SetValue(touch.location.mX, touch.location.mY);
				mScrollOffsetReference.SetValue(mClient.mX, mClient.mY);
				mScrollOffset.SetValue(mScrollOffsetReference.mX, mScrollOffsetReference.mY);
				mScrollLastTimestamp = touch.timestamp;
				mScrollTracking = false;
				mSeekScrollTarget = false;
				if (mClientLastDown != null)
				{
					mClientLastDown = null;
				}
				mClientLastDown = GetClientWidgetAt(touch);
				mClientLastDown.mIsDown = true;
				mClientLastDown.mIsOver = true;
				mClientLastDown.TouchBegan(touch);
			}
			MarkDirty();
		}

		public override void TouchMoved(SexyAppBase.Touch touch)
		{
			if (!mDragEnabled)
			{
				return;
			}
			FPoint fPoint = new FPoint(touch.location.mX, touch.location.mY) - mScrollTouchReference;
			if (mClient != null)
			{
				if (!mScrollTracking && (mScrollPractical & ScrollMode.SCROLL_HORIZONTAL) != 0 && Math.Abs(fPoint.mX) > SCROLL_DRAG_THRESHOLD)
				{
					mScrollTracking = true;
				}
				if (!mScrollTracking && (mScrollPractical & ScrollMode.SCROLL_VERTICAL) != 0 && Math.Abs(fPoint.mY) > SCROLL_DRAG_THRESHOLD)
				{
					mScrollTracking = true;
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
				bool flag = mClientLastDown.GetInsetRect().Contains(thePoint);
				if (flag && !mClientLastDown.mIsOver)
				{
					mClientLastDown.mIsOver = true;
					mClientLastDown.MouseEnter();
				}
				else if (!flag && mClientLastDown.mIsOver)
				{
					mClientLastDown.MouseLeave();
					mClientLastDown.mIsOver = false;
				}
				touch.location.mX += point.mX;
				touch.location.mY += point.mY;
				touch.previousLocation.mX += point.mX;
				touch.previousLocation.mY += point.mY;
				mClientLastDown.TouchMoved(touch);
			}
			MarkDirty();
		}

		public override void TouchEnded(SexyAppBase.Touch touch)
		{
			if (mScrollTracking)
			{
				TouchMotion(touch);
				mScrollTracking = false;
				if (mPagingEnabled)
				{
					SnapToPage();
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
				touch.previousLocation.mY += point.mY;
				mClientLastDown.TouchEnded(touch);
				mClientLastDown.mIsDown = false;
				mClientLastDown = null;
			}
			MarkDirty();
		}

		public override void TouchesCanceled()
		{
			if (mClient != null && mClientLastDown != null && !mScrollTracking)
			{
				mClientLastDown.TouchesCanceled();
				mClientLastDown.mIsDown = false;
				mClientLastDown = null;
			}
			mScrollTracking = false;
			MarkDirty();
		}

		public override void Update()
		{
			base.Update();
			if (mVisible && !mDisabled)
			{
				if (mIsDown)
				{
					mIndicatorsFlashTimer = SCROLL_INDICATORS_FLASH_TICKS;
				}
				else
				{
					float num = Math.Min(0f, mScrollMin.mX);
					float num2 = Math.Min(0f, mScrollMin.mY);
					float num3 = mScrollMax.mX;
					float num4 = mScrollMax.mY;
					if (mSeekScrollTarget)
					{
						float num5 = VectorNorm(mScrollTarget - mScrollOffset);
						if (num5 < SCROLL_TARGET_THRESHOLD_NORM)
						{
							mScrollOffset.SetValue(mScrollTarget.mX, mScrollTarget.mY);
							mSeekScrollTarget = false;
							if (mListener != null)
							{
								mListener.ScrollTargetReached(this);
							}
							if (mPagingEnabled && mPageControl != null)
							{
								mPageControl.SetCurrentPage(mCurrentPageHorizontal);
							}
						}
						else
						{
							num = (num3 = mScrollTarget.mX);
							num2 = (num4 = mScrollTarget.mY);
						}
					}
					float num6 = VectorNorm(mScrollVelocity);
					if (num6 < SCROLL_VELOCITY_THRESHOLD_NORM)
					{
						mScrollVelocity.SetValue(0f, 0f);
					}
					else
					{
						bool flag = mScrollOffset.mX < num || mScrollOffset.mX >= num3;
						bool flag2 = mScrollOffset.mY < num2 || mScrollOffset.mY >= num4;
						FPoint fPoint = new FPoint(flag ? SCROLL_VELOCITY_DEVIATION_DAMPING : SCROLL_VELOCITY_DAMPING, flag2 ? SCROLL_VELOCITY_DEVIATION_DAMPING : SCROLL_VELOCITY_DAMPING);
						mScrollOffset = PointAddScaled(mScrollOffset, mScrollVelocity, 1f / (float)FRAMEWORK_UPDATE_RATE, ref mScrollOffset);
						FPoint fPoint2 = mScrollVelocity * fPoint;
						mScrollVelocity.SetValue(fPoint2.mX, fPoint2.mY);
					}
					if (mScrollOffset.mX < num)
					{
						if (mBounceEnabled || mSeekScrollTarget)
						{
							mScrollOffset.mX += SCROLL_SPRINGBACK_TENSION * (num - mScrollOffset.mX);
						}
						else
						{
							mScrollOffset.mX = num;
							mScrollVelocity.mX = 0f;
						}
					}
					else if (mScrollOffset.mX > num3)
					{
						if (mBounceEnabled || mSeekScrollTarget)
						{
							mScrollOffset.mX += SCROLL_SPRINGBACK_TENSION * (num3 - mScrollOffset.mX);
						}
						else
						{
							mScrollOffset.mX = num3;
							mScrollVelocity.mX = 0f;
						}
					}
					if (mScrollOffset.mY < num2)
					{
						if (mBounceEnabled || mSeekScrollTarget)
						{
							mScrollOffset.mY += SCROLL_SPRINGBACK_TENSION * (num2 - mScrollOffset.mY);
						}
						else
						{
							mScrollOffset.mY = num2;
							mScrollVelocity.mY = 0f;
						}
					}
					else if (mScrollOffset.mY > num4)
					{
						if (mBounceEnabled || mSeekScrollTarget)
						{
							mScrollOffset.mY += SCROLL_SPRINGBACK_TENSION * (num4 - mScrollOffset.mY);
						}
						else
						{
							mScrollOffset.mY = num4;
							mScrollVelocity.mY = 0f;
						}
					}
					if (mClient != null)
					{
						mClient.Move((int)mScrollOffset.mX, (int)mScrollOffset.mY);
					}
					if (mIndicatorsFlashTimer > 0)
					{
						mIndicatorsFlashTimer--;
					}
				}
				if (mIndicatorsFlashTimer > 0 && mIndicatorsOpacity < 1f)
				{
					mIndicatorsOpacity = Math.Max(1f, mIndicatorsOpacity + SCROLL_INDICATORS_FADE_IN_RATE);
				}
				else if (mIndicatorsFlashTimer == 0 && mIndicatorsOpacity > 0f)
				{
					mIndicatorsOpacity = Math.Max(0f, mIndicatorsOpacity - SCROLL_INDICATORS_FADE_OUT_RATE);
				}
			}
			MarkDirty();
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			base.Draw(g);
			if (mBackgroundImage != null)
			{
				g.DrawImage(mBackgroundImage, 0, 0);
			}
			else if (mFillBackground)
			{
				g.FillRect(0, 0, mWidth, mHeight);
			}
		}

		private static void DrawHorizontalStretchableImage(SexyFramework.Graphics.Graphics g, Image image, Rect destRect)
		{
			int width = image.GetWidth();
			int height = image.GetHeight();
			Rect theSrcRect = new Rect(0, 0, (width - 1) / 2, height);
			Rect theSrcRect2 = new Rect(theSrcRect.mWidth, 0, 1, height);
			Rect theSrcRect3 = new Rect(theSrcRect2.mX + theSrcRect2.mWidth, 0, width - theSrcRect.mWidth - theSrcRect2.mWidth, height);
			int theY = destRect.mY + (destRect.mHeight - height) / 2;
			Rect theDestRect = new Rect(destRect.mX + theSrcRect.mWidth, theY, destRect.mWidth - theSrcRect.mWidth - theSrcRect3.mWidth, height);
			g.DrawImage(image, destRect.mX, theY, theSrcRect);
			g.DrawImage(image, theDestRect, theSrcRect2);
			g.DrawImage(image, destRect.mX + destRect.mWidth - theSrcRect3.mWidth, theY, theSrcRect3);
		}

		private static void DrawVerticalStretchableImage(SexyFramework.Graphics.Graphics g, Image image, Rect destRect)
		{
			int width = image.GetWidth();
			int height = image.GetHeight();
			Rect theSrcRect = new Rect(0, 0, width, (height - 1) / 2);
			Rect theSrcRect2 = new Rect(0, theSrcRect.mHeight, width, 1);
			Rect theSrcRect3 = new Rect(0, theSrcRect2.mY + theSrcRect2.mHeight, width, height - theSrcRect.mHeight - theSrcRect2.mHeight);
			int theX = destRect.mX + (destRect.mWidth - width) / 2;
			Rect theDestRect = new Rect(theX, destRect.mY + theSrcRect.mHeight, width, destRect.mHeight - theSrcRect.mHeight - theSrcRect3.mHeight);
			g.DrawImage(image, theX, destRect.mY, theSrcRect);
			g.DrawImage(image, theDestRect, theSrcRect2);
			g.DrawImage(image, theX, destRect.mY + destRect.mHeight - theSrcRect3.mHeight, theSrcRect3);
		}

		public void DrawProxyWidget(SexyFramework.Graphics.Graphics g, ProxyWidget proxyWidget)
		{
			Color color = new Color(255, 255, 255, (int)(255f * mIndicatorsOpacity));
			if (color.mAlpha != 0)
			{
				int width = mIndicatorsImage.GetWidth();
				int height = mIndicatorsImage.GetHeight();
				Insets insets = mIndicatorsInsets;
				g.SetColor(color);
				g.SetColorizeImages(true);
				if ((mScrollPractical & ScrollMode.SCROLL_HORIZONTAL) != 0)
				{
					float num = (float)mWidth / (float)mClient.Width();
					int num2 = mWidth - insets.mLeft - insets.mRight - (((mScrollMode & ScrollMode.SCROLL_VERTICAL) != 0) ? width : 0);
					int num3 = (int)((float)num2 * num);
					int num4 = num2 - num3;
					float num5 = Math.Min(0, mWidth - mClient.mWidth - mScrollInsets.mRight);
					float num6 = mScrollInsets.mLeft;
					float num7 = 1f - (mScrollOffset.mX - num5) / (num6 - num5);
					int num8 = (int)((float)num4 * num7);
					int val = num8 + num3;
					num8 = Math.Min(Math.Max(0, num8), num2 - width);
					val = Math.Min(Math.Max(width, val), num2);
					Rect destRect = default(Rect);
					destRect.mX = insets.mLeft + num8;
					destRect.mY = mHeight - insets.mBottom - height;
					destRect.mWidth = val - num8;
					destRect.mHeight = height;
					DrawHorizontalStretchableImage(g, mIndicatorsImage, destRect);
				}
				if ((mScrollPractical & ScrollMode.SCROLL_VERTICAL) != 0)
				{
					float num9 = (float)mHeight / (float)mClient.Height();
					int num10 = mHeight - insets.mTop - insets.mBottom - (((mScrollMode & ScrollMode.SCROLL_HORIZONTAL) != 0) ? height : 0);
					int num11 = (int)((float)num10 * num9);
					int num12 = num10 - num11;
					float num13 = Math.Min(0, mHeight - mClient.mHeight - mScrollInsets.mBottom);
					float num14 = mScrollInsets.mTop;
					float num15 = 1f - (mScrollOffset.mY - num13) / (num14 - num13);
					int num16 = (int)((float)num12 * num15);
					int val2 = num16 + num11;
					num16 = Math.Min(Math.Max(0, num16), num10 - height);
					val2 = Math.Min(Math.Max(height, val2), num10);
					Rect destRect2 = default(Rect);
					destRect2.mX = mWidth - insets.mRight - width;
					destRect2.mY = insets.mTop + num16;
					destRect2.mWidth = width;
					destRect2.mHeight = val2 - num16;
					DrawVerticalStretchableImage(g, mIndicatorsImage, destRect2);
				}
			}
			if (!mDrawOverlays)
			{
				return;
			}
			g.SetColorizeImages(false);
			foreach (Overlay mOverlay in mOverlays)
			{
				g.DrawImage(mOverlay.image, mOverlay.offset.mX, mOverlay.offset.mY);
			}
		}

		public void ClearLastDown()
		{
			mClientLastDown = null;
		}

		protected void Init(ScrollWidgetListener listener)
		{
			mClient = null;
			mClientLastDown = null;
			mListener = listener;
			mPageControl = null;
			mIndicatorsProxy = null;
			mIndicatorsImage = null;
			mScrollMode = ScrollMode.SCROLL_VERTICAL;
			mScrollInsets = new Insets(0, 0, 0, 0);
			mScrollTracking = false;
			mSeekScrollTarget = false;
			mBounceEnabled = true;
			mPagingEnabled = false;
			mIndicatorsEnabled = false;
			mIndicatorsInsets = new Insets(0, 0, 0, 0);
			mIndicatorsFlashTimer = 0;
			mIndicatorsOpacity = 0f;
			mBackgroundImage = null;
			mFillBackground = false;
			mDrawOverlays = false;
			mScrollOffset.SetValue(0f, 0f);
			mScrollVelocity.SetValue(0f, 0f);
			mClip = true;
			mDragEnabled = true;
		}

		protected void SnapToPage()
		{
			FPoint fPoint = new FPoint((float)mScrollInsets.mLeft + mPageSize.mX / 2f, (float)mScrollInsets.mTop + mPageSize.mY / 2f);
			FPoint fPoint2 = fPoint - mScrollOffset;
			int val = (int)Math.Floor(fPoint2.mX / mPageSize.mX);
			int val2 = (int)Math.Floor(fPoint2.mY / mPageSize.mY);
			val = Math.Max(0, Math.Min(val, mPageCountHorizontal - 1));
			val2 = Math.Max(0, Math.Min(val2, mPageCountVertical - 1));
			FPoint fPoint3 = new FPoint((float)mScrollInsets.mLeft - (float)val * mPageSize.mX, (float)mScrollInsets.mTop - (float)val2 * mPageSize.mY);
			if (mScrollVelocity.mX > SCROLL_PAGE_FLICK_THRESHOLD && fPoint3.mX < mScrollOffset.mX)
			{
				val--;
			}
			else if (mScrollVelocity.mX < 0f - SCROLL_PAGE_FLICK_THRESHOLD && fPoint3.mX > mScrollOffset.mX)
			{
				val++;
			}
			if (mScrollVelocity.mY > SCROLL_PAGE_FLICK_THRESHOLD && fPoint3.mY < mScrollOffset.mY)
			{
				val2--;
			}
			else if (mScrollVelocity.mY < 0f - SCROLL_PAGE_FLICK_THRESHOLD && fPoint3.mY > mScrollOffset.mY)
			{
				val2++;
			}
			SetPage(val, val2, true);
		}

		protected void TouchMotion(SexyAppBase.Touch touch)
		{
			if (!mDragEnabled)
			{
				return;
			}
			FPoint fPoint = new FPoint(touch.location.mX, touch.location.mY) - mScrollTouchReference;
			FPoint fPoint2 = new FPoint(mScrollOffset);
			if ((mScrollPractical & ScrollMode.SCROLL_HORIZONTAL) != 0)
			{
				fPoint2.mX = mScrollOffsetReference.mX + fPoint.mX;
				float num = mScrollMin.mX;
				float num2 = mScrollMax.mX;
				if (fPoint2.mX < num)
				{
					fPoint2.mX = (mBounceEnabled ? (fPoint2.mX + SCROLL_DEVIATION_DAMPING * (num - fPoint2.mX)) : num);
					mScrollVelocity.mX = 0f;
				}
				else if (fPoint2.mX > num2)
				{
					fPoint2.mX = (mBounceEnabled ? (fPoint2.mX + SCROLL_DEVIATION_DAMPING * (num2 - fPoint2.mX)) : num2);
					mScrollVelocity.mX = 0f;
				}
				else
				{
					float num3 = fPoint2.mX - mScrollOffset.mX;
					double num4 = touch.timestamp - mScrollLastTimestamp;
					double num5 = (double)num3 / num4;
					double num6 = Math.Min(1.0, num4 / (double)SCROLL_VELOCITY_FILTER_WINDOW);
					if (double.IsNaN(num5))
					{
						num5 = 0.0;
					}
					float f = (float)(num6 * num5 + (1.0 - num6) * (double)mScrollVelocity.mX);
					if (!float.IsNaN(f))
					{
						mScrollVelocity.mX = f;
					}
				}
			}
			if ((mScrollPractical & ScrollMode.SCROLL_VERTICAL) != 0)
			{
				fPoint2.mY = mScrollOffsetReference.mY + fPoint.mY;
				float num7 = mScrollMin.mY;
				float num8 = mScrollMax.mY;
				if (fPoint2.mY < num7)
				{
					fPoint2.mY = (mBounceEnabled ? (fPoint2.mY + SCROLL_DEVIATION_DAMPING * (num7 - fPoint2.mY)) : num7);
					mScrollVelocity.mY = 0f;
				}
				else if (fPoint2.mY > num8)
				{
					fPoint2.mY = (mBounceEnabled ? (fPoint2.mY + SCROLL_DEVIATION_DAMPING * (num8 - fPoint2.mY)) : num8);
					mScrollVelocity.mY = 0f;
				}
				else
				{
					float num9 = fPoint2.mY - mScrollOffset.mY;
					double num10 = touch.timestamp - mScrollLastTimestamp;
					double num11 = (double)num9 / num10;
					double num12 = Math.Min(1.0, num10 / (double)SCROLL_VELOCITY_FILTER_WINDOW);
					if (double.IsNaN(num11))
					{
						num11 = 0.0;
					}
					float f2 = (float)(num12 * num11 + (1.0 - num12) * (double)mScrollVelocity.mY);
					if (!float.IsNaN(f2))
					{
						mScrollVelocity.mY = f2;
					}
				}
			}
			mScrollOffset.SetValue(fPoint2.mX, fPoint2.mY);
			mScrollLastTimestamp = touch.timestamp;
			mClient.Move((int)mScrollOffset.mX, (int)mScrollOffset.mY);
		}

		protected Widget GetClientWidgetAt(SexyAppBase.Touch touch)
		{
			int num = touch.location.mX - mClient.mX;
			int num2 = touch.location.mY - mClient.mY;
			int theWidgetX = 0;
			int theWidgetY = 0;
			int theFlags = 0x10 | mWidgetManager.GetWidgetFlags();
			bool found = false;
			Widget widgetAtHelper;
			if (mClientLastDown != null)
			{
				Point absPos = mClient.GetAbsPos();
				Point absPos2 = mClientLastDown.GetAbsPos();
				widgetAtHelper = mClientLastDown;
				theWidgetX = touch.location.mX + absPos.mX - absPos2.mX;
				theWidgetY = touch.location.mY + absPos.mY - absPos2.mY;
			}
			else
			{
				mClient.mWidgetFlagsMod.mRemoveFlags &= -17;
				widgetAtHelper = mClient.GetWidgetAtHelper(num, num2, theFlags, ref found, ref theWidgetX, ref theWidgetY);
				mClient.mWidgetFlagsMod.mRemoveFlags |= 16;
			}
			if (widgetAtHelper == null || widgetAtHelper.mDisabled)
			{
				theWidgetX = num;
				theWidgetY = num2;
				widgetAtHelper = mClient;
			}
			touch.previousLocation.mX += theWidgetX - touch.location.mX;
			touch.previousLocation.mY += theWidgetY - touch.location.mY;
			touch.location.mX = theWidgetX;
			touch.location.mY = theWidgetY;
			return widgetAtHelper;
		}

		protected void CacheDerivedValues()
		{
			if (mClient != null)
			{
				mScrollMin.mX = mWidth - mClient.mWidth - mScrollInsets.mRight;
				mScrollMin.mY = mHeight - mClient.mHeight - mScrollInsets.mBottom;
				mScrollMax.mX = mScrollInsets.mLeft;
				mScrollMax.mY = mScrollInsets.mTop;
				int num = ((mScrollMin.mX < mScrollMax.mX) ? 1 : 0) | ((mScrollMin.mY < mScrollMax.mY) ? 2 : 0);
				mScrollPractical = (ScrollMode)((int)mScrollMode & num);
			}
			else
			{
				mScrollMin.mX = (mScrollMax.mX = (mScrollMin.mY = (mScrollMax.mY = 0f)));
				mScrollPractical = ScrollMode.SCROLL_DISABLED;
			}
			if (mPagingEnabled)
			{
				mPageSize.mX = mWidth - mScrollInsets.mLeft - mScrollInsets.mRight;
				mPageSize.mY = mHeight - mScrollInsets.mTop - mScrollInsets.mBottom;
				if (mClient != null)
				{
					mPageCountHorizontal = (int)Math.Floor((float)mClient.Width() / mPageSize.mX);
					mPageCountVertical = (int)Math.Floor((float)mClient.Height() / mPageSize.mY);
				}
				else
				{
					mPageCountHorizontal = (mPageCountVertical = 0);
				}
			}
		}
	}
}
