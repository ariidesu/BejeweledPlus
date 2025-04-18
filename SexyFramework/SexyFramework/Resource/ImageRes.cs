using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Resource
{
	public class ImageRes : BaseRes
	{
		public SharedImageRef mImage = new SharedImageRef();

		public string mAlphaImage = "";

		public string mAlphaGridImage = "";

		public string mVariant = "";

		public Point mOffset;

		public bool mAutoFindAlpha;

		public bool mPalletize;

		public bool mA4R4G4B4;

		public bool mA8R8G8B8;

		public bool mDither16;

		public bool mDDSurface;

		public bool mPurgeBits;

		public bool mMinimizeSubdivisions;

		public bool mCubeMap;

		public bool mVolumeMap;

		public bool mNoTriRep;

		public bool m2DBig;

		public bool mIsAtlas;

		public int mRows = 1;

		public int mCols = 1;

		public int mAlphaColor;

		public AnimInfo mAnimInfo = new AnimInfo();

		public string mAtlasName;

		public int mAtlasX;

		public int mAtlasY;

		public int mAtlasW;

		public int mAtlasH;

		public ImageRes()
		{
			mType = ResType.ResType_Image;
			mAtlasName = null;
		}

		public override void DeleteResource()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				mResourceRef.Release();
			}
			if (mGlobalPtr != null)
			{
				mGlobalPtr.mResObject = null;
			}
			mImage.Release();
		}

		public override void ApplyConfig()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				return;
			}
			DeviceImage deviceImage = mImage.GetDeviceImage();
			if (deviceImage == null)
			{
				return;
			}
			deviceImage.ReplaceImageFlags(0u);
			if (mNoTriRep)
			{
				deviceImage.AddImageFlags(ImageFlags.ImageFlag_NoTriRep);
			}
			deviceImage.mNumRows = mRows;
			deviceImage.mNumCols = mCols;
			if (mDither16)
			{
				deviceImage.mDither16 = true;
			}
			if (mA4R4G4B4)
			{
				deviceImage.AddImageFlags(ImageFlags.ImageFlag_UseA4R4G4B4);
			}
			if (mA8R8G8B8)
			{
				deviceImage.AddImageFlags(ImageFlags.ImageFlag_UseA8R8G8B8);
			}
			if (mMinimizeSubdivisions)
			{
				deviceImage.AddImageFlags(ImageFlags.ImageFlag_MinimizeNumSubdivisions);
			}
			if (mCubeMap)
			{
				deviceImage.AddImageFlags(ImageFlags.ImageFlag_CubeMap);
			}
			else if (mVolumeMap)
			{
				deviceImage.AddImageFlags(ImageFlags.ImageFlag_VolumeMap);
			}
			if (mAnimInfo.mAnimType != 0)
			{
				deviceImage.mAnimInfo = new AnimInfo(mAnimInfo);
			}
			if (mIsAtlas)
			{
				deviceImage.AddImageFlags(513u);
			}
			if (mAtlasName != null)
			{
				deviceImage.mAtlasImage = GlobalMembers.gSexyAppBase.mResourceManager.LoadImage(mAtlasName).GetImage();
				deviceImage.mAtlasStartX = mAtlasX;
				deviceImage.mAtlasStartY = mAtlasY;
				deviceImage.mAtlasEndX = mAtlasX + mAtlasW;
				deviceImage.mAtlasEndY = mAtlasY + mAtlasH;
			}
			deviceImage.CommitBits();
			deviceImage.mPurgeBits = mPurgeBits;
			if (mDDSurface)
			{
				deviceImage.CommitBits();
				if (!deviceImage.mHasAlpha)
				{
					deviceImage.mWantDeviceSurface = true;
					deviceImage.mPurgeBits = true;
				}
			}
			if (deviceImage.mPurgeBits)
			{
				lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
				{
					deviceImage.PurgeBits();
				}
			}
		}
	}
}
