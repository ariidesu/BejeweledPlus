using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class PAObjectPos
	{
		public string mName;

		public int mObjectNum;

		public bool mIsSprite;

		public bool mIsAdditive;

		public bool mHasSrcRect;

		public byte mResNum;

		public int mPreloadFrames;

		public int mAnimFrameNum;

		public float mTimeScale;

		public PATransform mTransform = new PATransform(true);

		public Rect mSrcRect = default(Rect);

		public int mColorInt;

		public PAObjectPos()
		{
		}

		public PAObjectPos(PAObjectPos inObj)
		{
			mName = inObj.mName;
			mObjectNum = inObj.mObjectNum;
			mIsSprite = inObj.mIsSprite;
			mIsAdditive = inObj.mIsAdditive;
			mHasSrcRect = inObj.mHasSrcRect;
			mResNum = inObj.mResNum;
			mPreloadFrames = inObj.mPreloadFrames;
			mAnimFrameNum = inObj.mAnimFrameNum;
			mTimeScale = inObj.mTimeScale;
			mTransform.CopyFrom(inObj.mTransform);
			mSrcRect = inObj.mSrcRect;
			mColorInt = inObj.mColorInt;
		}
	}
}
