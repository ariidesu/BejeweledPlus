using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3SlideSelector : ScrollWidget, ScrollWidgetListener, Bej3ButtonListener, ButtonListener
	{
		private Bej3SlideSelectorListener mSlideListener;

		private Bej3ButtonListener mButtonListener;

		private List<Bej3Button> mItems = new List<Bej3Button>();

		private int mItemSize;

		private float mItemScale;

		private float mCachedItemPos;

		private int mContainerWidth;

		private int mItemBaseId;

		private int mLastAlpha;

		private int mLastDrawPos;

		private int mForceCenteringItem;

		private Point mDownPos = default(Point);

		private Point mThreshold = default(Point);

		public int mId;

		public bool mLocked;

		public Bej3Button mSelectedItem;

		public Bej3Checkbox mEnabledCheckbox;

		public Bej3SlideSelectorContainer mContainer;

		public static float[] Params = new float[4];

		private void DrawImagePart(Graphics g, Image img, Rect dest, Rect src)
		{
			for (int i = dest.mY; i < dest.mY + dest.mHeight; i += src.mHeight)
			{
				g.DrawImage(img, new Rect(dest.mX, i, src.mHeight, dest.mWidth), src);
			}
		}

		public Bej3SlideSelector(int theId, Bej3SlideSelectorListener theListener, Bej3ButtonListener theButtonListener, int itemSize, int containerWidth)
		{
			Init(this);
			mId = theId;
			mSlideListener = theListener;
			mButtonListener = theButtonListener;
			mItemSize = itemSize;
			mCachedItemPos = -999f;
			mSelectedItem = null;
			mContainerWidth = containerWidth;
			mItemBaseId = -1;
			mItemScale = 1f;
			mForceCenteringItem = -1;
			mLocked = false;
			mDownPos = new Point(-1, -1);
			mThreshold = new Point(0, 0);
			mContainer = new Bej3SlideSelectorContainer();
			AddWidget(mContainer);
			EnableBounce(true);
		}

		public override void Update()
		{
			base.Update();
			if (!mVisible || mDisabled)
			{
				return;
			}
			int num = mContainer.mX;
			float num2 = -99999f;
			Bej3Button bej3Button = null;
			if (mCachedItemPos != mScrollOffset.mX)
			{
				for (int i = 0; i < mItems.Count; i++)
				{
					Bej3Button bej3Button2 = mItems[i];
					if (bej3Button2.mButtonImage == null)
					{
						return;
					}
					int num3 = num + GetItemXPos(bej3Button2) + bej3Button2.mButtonImage.mWidth / 2;
					float num4 = 1f - (float)(mWidth / 2 - num3) / (float)mWidth;
					bool flag = num4 <= 1f;
					if (num4 > 1f)
					{
						num4 = 1f - (num4 - 1f);
					}
					if (num4 < 0f)
					{
						num4 = 0f;
					}
					if (num4 > 0.96f)
					{
						num4 = 1f;
					}
					num3 = GetItemXPos(bej3Button2);
					num3 = ((!flag) ? (num3 - (int)((float)mItemSize * (1f - num4) * ConstantsWP.BEJ3SLIDESELECTOR_ITEM_MULT * (1f - num4))) : (num3 + (int)((float)mItemSize * (1f - num4) * ConstantsWP.BEJ3SLIDESELECTOR_ITEM_MULT * (1f - num4))));
					bej3Button2.Resize(num3, mHeight / 2 - bej3Button2.mHeight / 2, 0, 0);
					if (num4 > num2)
					{
						bej3Button = bej3Button2;
						num2 = num4;
					}
					bej3Button2.mAlpha = num4 * num4 * num4;
					bej3Button2.mZenSize = mItemScale;
				}
				if (bej3Button != mSelectedItem && mForceCenteringItem == -1 && CenterOnItem(bej3Button.mId))
				{
					mSelectedItem = bej3Button;
					mSlideListener.SlideSelectorChanged(mId, mSelectedItem.mId);
				}
				if (num2 == 1f)
				{
					mCachedItemPos = mScrollOffset.mX;
				}
			}
			else
			{
				mForceCenteringItem = -1;
			}
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			DrawUnSelectedButtons(g);
			g.PopState();
			g.SetColorizeImages(true);
			g.SetColor(Color.White);
			DrawButton(g, mSelectedItem);
			g.ClearClipRect();
			DeferOverlay(0);
			mLastAlpha = g.GetFinalColor().mAlpha;
			mLastDrawPos = (int)g.mTransY;
		}

		public override void DrawOverlay(Graphics g)
		{
			g.Translate(GetAbsPos().mX, mLastDrawPos);
			if (mLastAlpha >= 200)
			{
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, mLastAlpha));
			}
		}

		public void DrawButton(Graphics g, Bej3Button btn)
		{
			if (btn != null)
			{
				g.SetScale(btn.mZenSize, btn.mZenSize, mWidth / 2, mHeight / 2);
				g.DrawImage(btn.mButtonImage, btn.GetAbsPos().mX + (btn.mWidth - btn.mButtonImage.mWidth) / 2, (btn.mHeight - btn.mButtonImage.mHeight) / 2);
			}
		}

		public void DrawUnSelectedButtons(Graphics g)
		{
			g.SetColorizeImages(false);
			Graphics3D graphics3D = g.Get3D();
			float num = 0.75f;
			Params[0] = 0.3f;
			Params[1] = 0.59f;
			Params[2] = 0.11f;
			Params[3] = num;
			graphics3D.GetEffect(GlobalMembersResourcesWP.EFFECT_BADGE_GRAYSCALE);
			g.SetColorizeImages(true);
			g.SetColor(new Color(90, 80, 0, (int)(GlobalMembers.gApp.mDialogObscurePct * 255f)));
			for (int i = 0; i < mItems.Count; i++)
			{
				Bej3Button bej3Button = mItems[i];
				if (bej3Button != mSelectedItem)
				{
					DrawButton(g, bej3Button);
				}
			}
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public override void SetDisabled(bool isDisabled)
		{
			base.SetDisabled(isDisabled);
			foreach (SexyFramework.Widget.Widget mWidget in mWidgets)
			{
				mWidget.SetDisabled(isDisabled);
			}
			for (int i = 0; i < mItems.Count; i++)
			{
				mItems[i].SetDisabled(isDisabled);
			}
		}

		public void AddItem(int itemId, int itemImageId)
		{
			Bej3Button bej3Button = new Bej3Button(itemId, this, Bej3ButtonType.BUTTON_TYPE_ZEN_SLIDE);
			bej3Button.SetImageId(itemImageId);
			bej3Button.mBtnNoDraw = true;
			mContainer.AddWidget(bej3Button);
			mItems.Add(bej3Button);
			if (mItemBaseId == -1)
			{
				mItemBaseId = itemId;
			}
		}

		public bool CenterOnItem(int itemId)
		{
			return CenterOnItem(itemId, false);
		}

		public bool CenterOnItem(int itemId, bool immediate)
		{
			if (immediate)
			{
				mForceCenteringItem = itemId;
				mCachedItemPos = -999f;
			}
			else
			{
				mForceCenteringItem = -1;
				mCachedItemPos = -999f;
			}
			bool flag = false;
			for (int i = 0; i < mItems.Count; i++)
			{
				Bej3Button bej3Button = mItems[i];
				if (bej3Button.mId != itemId)
				{
					continue;
				}
				if (bej3Button.mButtonImage == null)
				{
					return false;
				}
				int num = bej3Button.mX + bej3Button.mButtonImage.mWidth / 2 - mWidth / 2;
				SetScrollOffset(new FPoint(-num, 0f), !mDisabled || !immediate);
				flag = true;
				if (immediate)
				{
					mSelectedItem = bej3Button;
					mSlideListener.SlideSelectorChanged(mId, mSelectedItem.mId);
				}
				break;
			}
			if (!flag)
			{
				Bej3Button bej3Button2 = mItems[0];
				if (bej3Button2.mButtonImage == null)
				{
					return false;
				}
				int theX = bej3Button2.mX + bej3Button2.mButtonImage.mWidth / 2 - mWidth / 2;
				ScrollToPoint(new Point(theX, 0), !mDisabled || !immediate);
				mSelectedItem = bej3Button2;
				mSlideListener.SlideSelectorChanged(mId, mSelectedItem.mId);
			}
			return true;
		}

		public int GetItemXPos(Bej3Button item)
		{
			int num = ConstantsWP.ZENOPTIONS_AMBIENCE_ITEM_SIZE * 2;
			return num + (int)((float)((item.mId - mItemBaseId) * mItemSize) * mItemScale);
		}

		public override bool Intersects(WidgetContainer theWidget)
		{
			return base.Intersects(theWidget);
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			mContainer.Resize(0, 0, mContainerWidth, mHeight);
			SetScrollMode(ScrollMode.SCROLL_HORIZONTAL);
		}

		public override void TouchBegan(SexyAppBase.Touch touch)
		{
			mDownPos = new Point(touch.location.mX, touch.location.mY);
			base.TouchBegan(touch);
		}

		public override void TouchMoved(SexyAppBase.Touch touch)
		{
			Point point = default(Point);
			if (mDownPos.mX >= 0 && mDownPos.mY >= 0)
			{
				point = mDownPos - new Point(touch.location.mX, touch.location.mY);
			}
			if (Math.Abs(point.mX) > mThreshold.mX)
			{
				mLocked = true;
			}
			if (mLocked)
			{
				base.TouchMoved(touch);
			}
		}

		public override void TouchEnded(SexyAppBase.Touch touch)
		{
			mLocked = false;
			bool flag = mScrollTracking;
			base.TouchEnded(touch);
			if (flag && mSelectedItem != null)
			{
				CenterOnItem(mSelectedItem.mId);
			}
		}

		public void LinkUpAssets()
		{
			int num = mContainer.mX;
			for (int i = 0; i < mItems.Count; i++)
			{
				Bej3Button bej3Button = mItems[i];
				bej3Button.LinkUpAssets();
				if (bej3Button.mButtonImage != null)
				{
					int num2 = num + GetItemXPos(bej3Button) + bej3Button.mButtonImage.mWidth / 2;
					float num3 = 1f - (float)(mWidth / 2 - num2) / (float)mWidth;
					bool flag = num3 <= 1f;
					if (num3 > 1f)
					{
						num3 = 1f - (num3 - 1f);
					}
					if (num3 < 0f)
					{
						num3 = 0f;
					}
					if (num3 > 0.96f)
					{
						num3 = 1f;
					}
					num2 = GetItemXPos(bej3Button);
					num2 = ((!flag) ? (num2 - (int)((float)mItemSize * (1f - num3) * ConstantsWP.BEJ3SLIDESELECTOR_ITEM_MULT * (1f - num3))) : (num2 + (int)((float)mItemSize * (1f - num3) * ConstantsWP.BEJ3SLIDESELECTOR_ITEM_MULT * (1f - num3))));
					bej3Button.Resize(num2, mHeight / 2 - bej3Button.mHeight / 2, 0, 0);
					bej3Button.mAlpha = num3 * num3 * num3;
				}
			}
			mCachedItemPos = -999f;
		}

		public virtual void ButtonMouseEnter(int theId)
		{
		}

		public void ButtonDepress(int theId)
		{
			CenterOnItem(theId, true);
			mSlideListener.SlideSelectorChanged(mId, mSelectedItem.mId);
		}

		public void SetItemScale(float scale)
		{
			mItemScale = scale;
			LinkUpAssets();
		}

		public void SetThreshold(int x, int y)
		{
			mThreshold.mX = x;
			mThreshold.mY = y;
		}

		public int GetSelectedId()
		{
			if (mSelectedItem != null)
			{
				return mSelectedItem.mId;
			}
			return -1;
		}

		public void ButtonPress(int theId)
		{
		}

		public void ButtonMouseLeave(int theId)
		{
		}

		public bool ButtonsEnabled()
		{
			return true;
		}

		public void ButtonPress(int theId, int theClickCount)
		{
		}

		public void ButtonDownTick(int theId)
		{
		}

		public void ButtonMouseMove(int theId, int theX, int theY)
		{
			throw new NotImplementedException();
		}
	}
}
