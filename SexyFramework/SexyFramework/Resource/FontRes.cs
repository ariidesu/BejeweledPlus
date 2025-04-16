using SexyFramework.Graphics;

namespace SexyFramework.Resource
{
	public class FontRes : BaseRes
	{
		public Font mFont;

		public Image mImage;

		public string mImagePath;

		public string mTags = "";

		public bool mSysFont;

		public bool mBold;

		public bool mItalic;

		public bool mUnderline;

		public bool mShadow;

		public int mSize;

		public FontRes()
		{
			mType = ResType.ResType_Font;
			mSysFont = false;
			mFont = null;
			mImage = null;
		}

		public override void DeleteResource()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				mResourceRef.Release();
			}
			if (mFont != null)
			{
				mFont.Dispose();
				mFont = null;
			}
			if (mImage != null)
			{
				mImage.Dispose();
				mImage = null;
			}
			if (mGlobalPtr != null)
			{
				mGlobalPtr.mResObject = null;
			}
		}

		public override void ApplyConfig()
		{
			if (mFont == null || mSysFont)
			{
				return;
			}
			ImageFont imageFont = (ImageFont)mFont;
			if (mTags.Length > 0)
			{
				mTags.ToCharArray();
				string[] array = mTags.Split(',', ' ', '\r', '\t', '\n');
				string[] array2 = array;
				foreach (string theTagName in array2)
				{
					imageFont.AddTag(theTagName);
				}
				imageFont.Prepare();
			}
		}
	}
}
