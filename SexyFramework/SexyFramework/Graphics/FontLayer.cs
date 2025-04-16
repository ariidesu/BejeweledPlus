using System.Collections.Generic;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class FontLayer
	{
		public struct KerningValue
		{
			public int mInt;

			public ushort mChar;

			public short mOffset;
		}

		public FontData mFontData;

		public Dictionary<string, string> mExtendedInfo = new Dictionary<string, string>();

		public string mLayerName;

		public List<string> mRequiredTags = new List<string>();

		public List<string> mExcludedTags = new List<string>();

		public KerningValue[] mKerningData = new KerningValue[0];

		public CharDataHashTable mCharDataHashTable = new CharDataHashTable();

		public Color mColorMult = default(Color);

		public Color mColorAdd = default(Color);

		public SharedImageRef mImage = new SharedImageRef();

		public bool mImageIsWhite;

		public string mImageFileName;

		public int mDrawMode;

		public Point mOffset = default(Point);

		public int mSpacing;

		public int mMinPointSize;

		public int mMaxPointSize;

		public int mPointSize;

		public int mAscent;

		public int mAscentPadding;

		public int mHeight;

		public int mDefaultHeight;

		public int mLineSpacingOffset;

		public int mBaseOrder;

		public bool mUseAlphaCorrection;

		public FontLayer()
		{
			mFontData = null;
			mDrawMode = -1;
			mSpacing = 0;
			mPointSize = 0;
			mAscent = 0;
			mAscentPadding = 0;
			mMinPointSize = -1;
			mMaxPointSize = -1;
			mHeight = 0;
			mDefaultHeight = 0;
			mColorMult = new Color(Color.White);
			mColorAdd = new Color(0, 0, 0, 0);
			mLineSpacingOffset = 0;
			mBaseOrder = 0;
			mImageIsWhite = false;
			mUseAlphaCorrection = true;
			mCharDataHashTable.mOrderedHash = ImageFont.mOrderedHash;
		}

		public FontLayer(FontData theFontData)
		{
			mFontData = theFontData;
			mDrawMode = -1;
			mSpacing = 0;
			mPointSize = 0;
			mAscent = 0;
			mAscentPadding = 0;
			mMinPointSize = -1;
			mMaxPointSize = -1;
			mHeight = 0;
			mDefaultHeight = 0;
			mColorMult = new Color(Color.White);
			mColorAdd = new Color(0, 0, 0, 0);
			mLineSpacingOffset = 0;
			mBaseOrder = 0;
			mImageIsWhite = false;
			mUseAlphaCorrection = true;
			mCharDataHashTable.mOrderedHash = ImageFont.mOrderedHash;
		}

		public FontLayer(FontLayer theFontLayer)
		{
			mFontData = theFontLayer.mFontData;
			mRequiredTags = theFontLayer.mRequiredTags;
			mExcludedTags = theFontLayer.mExcludedTags;
			mImage = new SharedImageRef(theFontLayer.mImage);
			mImageIsWhite = theFontLayer.mImageIsWhite;
			mDrawMode = theFontLayer.mDrawMode;
			mOffset = theFontLayer.mOffset;
			mSpacing = theFontLayer.mSpacing;
			mMinPointSize = theFontLayer.mMinPointSize;
			mMaxPointSize = theFontLayer.mMaxPointSize;
			mPointSize = theFontLayer.mPointSize;
			mAscent = theFontLayer.mAscent;
			mAscentPadding = theFontLayer.mAscentPadding;
			mHeight = theFontLayer.mHeight;
			mDefaultHeight = theFontLayer.mDefaultHeight;
			mColorMult = new Color(theFontLayer.mColorMult);
			mColorAdd = new Color(theFontLayer.mColorAdd);
			mLineSpacingOffset = theFontLayer.mLineSpacingOffset;
			mBaseOrder = theFontLayer.mBaseOrder;
			mExtendedInfo = theFontLayer.mExtendedInfo;
			mKerningData = theFontLayer.mKerningData;
			mCharDataHashTable = theFontLayer.mCharDataHashTable;
			mUseAlphaCorrection = theFontLayer.mUseAlphaCorrection;
			mLayerName = theFontLayer.mLayerName;
		}

		public CharData GetCharData(char theChar)
		{
			return mCharDataHashTable.GetCharData(theChar, true);
		}
	}
}
