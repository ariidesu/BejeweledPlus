using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class GraphicsState
	{
		public static Image mStaticImage = new Image();

		protected static Point[] mPFPoints = null;

		public Image mDestImage;

		public float mTransX;

		public float mTransY;

		public float mScaleX;

		public float mScaleY;

		public float mScaleOrigX;

		public float mScaleOrigY;

		public Rect mClipRect = default(Rect);

		public Rect mDestRect = default(Rect);

		public Rect mSrcRect = default(Rect);

		public PushedColorStack mPushedColorVector = new PushedColorStack();

		public Color mFinalColor = default(Color);

		public Color mColor = default(Color);

		public Font mFont;

		public int mDrawMode;

		public bool mColorizeImages;

		public bool mFastStretch;

		public bool mWriteColoredString;

		public bool mLinearBlend;

		public bool mIs3D;

		public void CopyStateFrom(GraphicsState theState)
		{
			mDestImage = theState.mDestImage;
			mTransX = theState.mTransX;
			mTransY = theState.mTransY;
			mClipRect = theState.mClipRect;
			mFont = theState.mFont;
			mPushedColorVector = theState.mPushedColorVector;
			mColor = theState.mColor;
			mFinalColor = theState.mFinalColor;
			mDrawMode = theState.mDrawMode;
			mColorizeImages = theState.mColorizeImages;
			mFastStretch = theState.mFastStretch;
			mWriteColoredString = theState.mWriteColoredString;
			mLinearBlend = theState.mLinearBlend;
			mScaleX = theState.mScaleX;
			mScaleY = theState.mScaleY;
			mScaleOrigX = theState.mScaleOrigX;
			mScaleOrigY = theState.mScaleOrigY;
			mIs3D = theState.mIs3D;
		}
	}
}
