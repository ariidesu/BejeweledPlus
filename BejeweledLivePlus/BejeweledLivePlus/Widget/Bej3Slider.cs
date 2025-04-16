using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3Slider : Slider
	{
		public bool mGrayedOut;

		public bool mGrayOutWhenDisabled;

		public bool mClippingEnabled;

		public bool mLocked;

		public Point mThreshold = default(Point);

		public Point mDownPos = default(Point);

		public bool mGrayOutWhenZero;

		private static int xOfsFill;

		public Bej3Slider(int theId, SliderListener theListener)
			: base(GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL, GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HANDLE, theId, theListener)
		{
			mGrayedOut = true;
			mGrayOutWhenDisabled = false;
			mClippingEnabled = true;
			mLocked = false;
			mThreshold = new Point(0, 0);
			mDownPos = new Point(-1, -1);
			mGrayOutWhenZero = true;
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			theHeight = ConstantsWP.BEJ3SLIDER_HEIGHT;
			theY -= theHeight / 2;
			base.Resize(theX, theY, theWidth, theHeight);
		}

		public void LinkUpAssets()
		{
			mThumbImage = GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HANDLE;
			if (mGrayedOut || (mVal == 0.0 && mGrayOutWhenZero))
			{
				mTrackImage = GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL_UNSELECTE;
			}
			else
			{
				mTrackImage = GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL;
			}
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Draw(Graphics g)
		{
			if (!mClippingEnabled)
			{
				g.ClearClipRect();
			}
			int num = (mHorizontal ? (mTrackImage.GetWidth() / 3) : mTrackImage.GetWidth());
			int num2 = (mHorizontal ? mTrackImage.GetHeight() : (mTrackImage.GetHeight() / 3));
			if (mHorizontal)
			{
				int theY = mHeight / 2 - mTrackImage.GetCelHeight() / 2;
				g.DrawImageBox(new Rect(ConstantsWP.BEJ3SLIDER_X_OFFSET, theY, mWidth - ConstantsWP.BEJ3SLIDER_WIDTH_OFFSET - ConstantsWP.BEJ3SLIDER_X_OFFSET, mTrackImage.GetCelHeight()), mTrackImage);
				xOfsFill = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1369) - GlobalMembersResourcesWP.ImgXOfs(1368));
				int num3 = (int)(mVal * (double)(mWidth - mThumbImage.GetCelWidth())) + mThumbImage.GetCelWidth() / 2;
				int theY2 = mHeight / 2 - GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL_FILL.GetCelHeight() / 2;
				Rect mClipRect = g.mClipRect;
				int num4 = num3;
				g.ClipRect(xOfsFill + ConstantsWP.BEJ3SLIDER_X_OFFSET, theY2, num4 - ConstantsWP.BEJ3SLIDER_X_OFFSET, GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL_FILL.GetCelHeight());
				Image theComponentImage = (mGrayedOut ? GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL_FILL_UNSE : GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL_FILL);
				g.DrawImageBox(new Rect(xOfsFill + ConstantsWP.BEJ3SLIDER_X_OFFSET, theY2, mWidth - ConstantsWP.BEJ3SLIDER_WIDTH_OFFSET - xOfsFill * 2 - ConstantsWP.BEJ3SLIDER_X_OFFSET, GlobalMembersResourcesWP.IMAGE_DIALOG_SLIDER_BAR_HORIZONTAL_FILL.GetCelHeight()), theComponentImage);
				g.mClipRect = mClipRect;
			}
			else
			{
				int theX = (mWidth - num) / 2;
				g.DrawImage(mTrackImage, theX, 0, new Rect(0, 0, num, num2));
				for (int i = 0; i < (mHeight - num2 * 2 + num2 - 1) / num2; i++)
				{
					g.DrawImage(mTrackImage, theX, num2 + i * num2, new Rect(0, num2, num, num2));
				}
				g.DrawImage(mTrackImage, theX, mHeight - num2, new Rect(0, num2 * 2, num, num2));
			}
			if (mHorizontal && mThumbImage != null)
			{
				Rect rect = default(Rect);
				g.DrawImage(theSrcRect: (!mGrayedOut) ? mThumbImage.GetCelRect(1) : mThumbImage.GetCelRect(2), theImage: mThumbImage, theX: (int)(mVal * (double)(mWidth - mThumbImage.GetCelWidth())), theY: (mHeight - mThumbImage.GetCelHeight()) / 2);
				if (mDragging)
				{
					rect = mThumbImage.GetCelRect(0);
					g.DrawImage(mThumbImage, (int)(mVal * (double)(mWidth - mThumbImage.GetCelWidth())), (mHeight - mThumbImage.GetCelHeight()) / 2, rect);
				}
			}
			else if (!mHorizontal && mThumbImage != null)
			{
				g.DrawImage(mThumbImage, (mWidth - mThumbImage.GetCelWidth()) / 2, (int)(mVal * (double)(mHeight - mThumbImage.GetCelHeight())));
			}
		}

		public override void SetDisabled(bool isDisabled)
		{
			base.SetDisabled(isDisabled);
			if (mGrayOutWhenDisabled)
			{
				mGrayedOut = mDisabled;
			}
			LinkUpAssets();
		}

		public override void MouseDown(int x, int y, int theClickCount)
		{
			base.MouseDown(x, y, theClickCount);
			mDragging = true;
			int num = ((mThumbImage == null) ? mKnobSize : mThumbImage.GetCelWidth());
			int num2 = (int)(mVal * (double)(mWidth - num));
			mRelX = x - num2;
		}

		public override void MouseDrag(int x, int y)
		{
			base.MouseDrag(x, y);
		}

		public override void MouseUp(int x, int y)
		{
			mLocked = false;
			base.MouseUp(x, y);
		}

		public void SetGrayedOut(bool grayedOut)
		{
		}

		public override void TouchesCanceled()
		{
			base.MouseUp(-1, -1);
			base.TouchesCanceled();
		}

		public void SetThreshold(int x, int y)
		{
			mThreshold.mX = x;
			mThreshold.mY = y;
		}
	}
}
