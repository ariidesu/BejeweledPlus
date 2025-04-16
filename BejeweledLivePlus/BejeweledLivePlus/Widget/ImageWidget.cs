using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Widget
{
	public class ImageWidget : Bej3WidgetBase
	{
		private int mImageId;

		private Image mImage;

		private bool mIsImageBox;

		private Rect mImageBox;

		private Point mImageSize;

		private IMAGEWIDGET_ALIGNMENT mAlignment;

		private Point mAlignmentOffset;

		public bool mMirror;

		public float mScale;

		public Color mAdditiveOverlayColor;

		private void DrawImage(Graphics g)
		{
			if (mImage == null)
			{
				return;
			}
			if (mIsImageBox)
			{
				g.DrawImageBoxStretch(mImageBox, mImage);
				return;
			}
			Rect theDestRect = new Rect(mAlignmentOffset.mX, mAlignmentOffset.mY, mImageSize.mX, mImageSize.mY);
			Rect celRect = mImage.GetCelRect(0);
			theDestRect.mWidth = (int)((float)theDestRect.mWidth * mScale);
			theDestRect.mHeight = (int)((float)theDestRect.mHeight * mScale);
			g.DrawImageMirror(mImage, theDestRect, celRect, mMirror);
			if (mAdditiveOverlayColor != Color.White)
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.SetColor(mAdditiveOverlayColor);
				g.DrawImageMirror(mImage, theDestRect, celRect, mMirror);
			}
		}

		public ImageWidget(int theImage)
		{
			bool flag = false;
			mMirror = false;
			mImageId = theImage;
			mScale = 1f;
			mIsImageBox = false;
			mImageBox = new Rect(0, 0, 0, 0);
			mImage = null;
			mAdditiveOverlayColor = Color.White;
			mImageSize = default(Point);
			mAlignment = IMAGEWIDGET_ALIGNMENT.IMAGEWIDGET_ALIGNMENT_TOPLEFT;
			mAlignmentOffset = default(Point);
			mClippingEnabled = flag;
			SetImage(theImage);
		}

		public ImageWidget(int theImage, bool clippingEnabled)
		{
			mMirror = false;
			mImageId = theImage;
			mScale = 1f;
			mIsImageBox = false;
			mImageBox = new Rect(0, 0, 0, 0);
			mImage = null;
			mAdditiveOverlayColor = Color.White;
			mImageSize = default(Point);
			mAlignment = IMAGEWIDGET_ALIGNMENT.IMAGEWIDGET_ALIGNMENT_TOPLEFT;
			mAlignmentOffset = default(Point);
			mClippingEnabled = clippingEnabled;
			SetImage(theImage);
		}

		public override void Draw(Graphics g)
		{
			g.SetColorizeImages(true);
			Color color = mColor;
			g.SetColor(color);
			if (!mClippingEnabled)
			{
				Rect mClipRect = g.mClipRect;
				g.ClearClipRect();
				DrawImage(g);
				g.SetClipRect(mClipRect);
			}
			else
			{
				DrawImage(g);
			}
		}

		public override void LinkUpAssets()
		{
			if (mImageId != -1)
			{
				mImage = GlobalMembersResourcesWP.GetImageById(mImageId);
			}
			IMAGEWIDGET_ALIGNMENT iMAGEWIDGET_ALIGNMENT = mAlignment;
			if (iMAGEWIDGET_ALIGNMENT == IMAGEWIDGET_ALIGNMENT.IMAGEWIDGET_ALIGNMENT_CENTRE)
			{
				mAlignmentOffset = new Point(-mImageSize.mX / 2, -mImageSize.mY / 2);
			}
			else
			{
				mAlignmentOffset = new Point(0, 0);
			}
		}

		public void SetImage(int imageId)
		{
			mImageId = imageId;
			LinkUpAssets();
			if (mImage != null)
			{
				mImageSize = new Point(mImage.mWidth, mImage.mHeight);
			}
		}

		public void SetImage(Image image)
		{
			mImageId = -1;
			mImage = image;
			LinkUpAssets();
			mImageSize = new Point(mImage.GetCelWidth(), mImage.GetCelHeight());
		}

		public Image GetImage()
		{
			return mImage;
		}

		public int GetImageId()
		{
			return mImageId;
		}

		public void SetImageBox(Rect theImageBox)
		{
			mImageBox = theImageBox;
			mIsImageBox = mImageBox.mWidth != 0 && mImageBox.mHeight != 0;
		}

		public void SetImageSize(int width, int height)
		{
			mImageSize = new Point(width, height);
		}

		public Point GetImageSize()
		{
			return mImageSize;
		}

		public void SetAlignment(IMAGEWIDGET_ALIGNMENT alignment)
		{
			mAlignment = alignment;
			LinkUpAssets();
		}
	}
}
