using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public struct GraphicsOperation
	{
		public enum IMAGE_TYPE
		{
			TYPE_DRAWIMAGE_XY,
			TYPE_DRAWIMAGE_XYWH,
			TYPE_DRAWIMAGE_XYR,
			TYPE_DRAWIMAGE_RR,
			TYPE_DRAWIMAGECEL_XYC,
			TYPE_DRAWIMAGECEL_RC,
			TYPE_DRAWIMAGEROTATED_XYAR,
			TYPE_DRAWIMAGEROTATED_XYAXYR,
			TYPE_SETSCALE
		}

		public int mTimestamp;

		public IMAGE_TYPE mType;

		public Image mImage;

		public FRect mDestRect;

		public Rect mSrcRect;

		public Color mColor;

		public int mDrawMode;

		public float mFloat;

		public FRect mFRect;

		public GraphicsOperation(Graphics g, IMAGE_TYPE theType, int Timestamp)
		{
			mType = theType;
			mDrawMode = g.mDrawMode;
			mColor = (g.mColorizeImages ? g.mColor : Color.White);
			mTimestamp = Timestamp;
			mImage = null;
			mDestRect = default(FRect);
			mSrcRect = default(Rect);
			mFRect = default(FRect);
			mFloat = 0f;
		}

		public void Execute(Graphics g)
		{
			g.SetDrawMode(mDrawMode);
			g.SetColor(mColor);
			Rect theDestRect = new Rect((int)mDestRect.mX, (int)mDestRect.mY, (int)mDestRect.mWidth, (int)mDestRect.mHeight);
			switch (mType)
			{
			case IMAGE_TYPE.TYPE_DRAWIMAGE_XY:
				g.DrawImageF(mImage, mDestRect.mX, mDestRect.mY);
				break;
			case IMAGE_TYPE.TYPE_DRAWIMAGE_XYWH:
				g.DrawImage(mImage, (int)mDestRect.mX, (int)mDestRect.mY, mSrcRect.mWidth, mSrcRect.mHeight);
				break;
			case IMAGE_TYPE.TYPE_DRAWIMAGE_XYR:
				g.DrawImage(mImage, (int)mDestRect.mX, (int)mDestRect.mY, mSrcRect);
				break;
			case IMAGE_TYPE.TYPE_DRAWIMAGE_RR:
				g.DrawImage(mImage, theDestRect, mSrcRect);
				break;
			case IMAGE_TYPE.TYPE_DRAWIMAGECEL_XYC:
				g.DrawImageCel(mImage, (int)mDestRect.mX, (int)mDestRect.mY, mSrcRect.mX);
				break;
			case IMAGE_TYPE.TYPE_DRAWIMAGECEL_RC:
				g.DrawImageCel(mImage, theDestRect, mSrcRect.mX);
				break;
			case IMAGE_TYPE.TYPE_DRAWIMAGEROTATED_XYAR:
				g.DrawImageRotated(mImage, (int)mDestRect.mX, (int)mDestRect.mY, mFloat, mSrcRect);
				break;
			case IMAGE_TYPE.TYPE_DRAWIMAGEROTATED_XYAXYR:
				g.DrawImageRotated(mImage, (int)mDestRect.mX, (int)mDestRect.mY, mFloat, (int)mDestRect.mWidth, (int)mDestRect.mHeight, mSrcRect);
				break;
			}
		}
	}
}
