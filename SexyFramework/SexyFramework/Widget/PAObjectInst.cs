using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class PAObjectInst
	{
		public string mName;

		public PASpriteInst mSpriteInst;

		public PATransform mBlendSrcTransform = new PATransform(true);

		public Color mBlendSrcColor = default(Color);

		public bool mIsBlending;

		public SexyTransform2D mTransform = new SexyTransform2D(false);

		public Color mColorMult = default(Color);

		public bool mPredrawCallback;

		public bool mImagePredrawCallback;

		public bool mPostdrawCallback;

		public PAObjectInst()
		{
			mName = null;
			mSpriteInst = null;
			mPredrawCallback = true;
			mPostdrawCallback = true;
			mImagePredrawCallback = true;
			mIsBlending = false;
		}
	}
}
