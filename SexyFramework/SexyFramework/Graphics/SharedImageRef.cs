namespace SexyFramework.Graphics
{
	public class SharedImageRef
	{
		public SharedImage mSharedImage;

		public MemoryImage mUnsharedImage;

		public bool mOwnsUnshared;

		public int mWidth
		{
			get
			{
				if (mSharedImage != null)
				{
					return mSharedImage.mImage.mWidth;
				}
				return 0;
			}
		}

		public int mHeight
		{
			get
			{
				if (mSharedImage != null)
				{
					return mSharedImage.mImage.mHeight;
				}
				return 0;
			}
		}

		public SharedImageRef()
		{
			mSharedImage = new SharedImage();
			mUnsharedImage = null;
			mOwnsUnshared = false;
		}

		public SharedImageRef(SharedImageRef theSharedImageRef)
		{
			mSharedImage = theSharedImageRef.mSharedImage;
			if (mSharedImage != null)
			{
				mSharedImage.mRefCount++;
			}
			mUnsharedImage = theSharedImageRef.mUnsharedImage;
			mOwnsUnshared = false;
		}

		public SharedImageRef(SharedImage theSharedImage)
		{
			mSharedImage = theSharedImage;
			if (theSharedImage != null)
			{
				mSharedImage.mRefCount++;
			}
			mUnsharedImage = null;
			mOwnsUnshared = false;
		}

		public virtual void Dispose()
		{
			Release();
		}

		public void CopyFrom(SharedImageRef theSharedImageRef)
		{
			Release();
			mSharedImage = theSharedImageRef.mSharedImage;
			mUnsharedImage = theSharedImageRef.mUnsharedImage;
			if (mSharedImage != null)
			{
				mSharedImage.mRefCount++;
			}
		}

		public void CopyFrom(SharedImage theSharedImage)
		{
			Release();
			mSharedImage = theSharedImage;
			mSharedImage.mRefCount++;
		}

		public void CopyFrom(MemoryImage theUnsharedImage)
		{
			Release();
			mUnsharedImage = theUnsharedImage;
		}

		public void Release()
		{
			if (mOwnsUnshared)
			{
				if (mUnsharedImage != null)
				{
					mUnsharedImage.Dispose();
				}
				mUnsharedImage = null;
			}
			if (mSharedImage != null && --mSharedImage.mRefCount == 0)
			{
				GlobalMembers.gSexyAppBase.mCleanupSharedImages = true;
			}
			mSharedImage = null;
		}

		internal DeviceImage GetDeviceImage()
		{
			if (mSharedImage != null)
			{
				return mSharedImage.mImage;
			}
			return null;
		}

		public Image GetImage()
		{
			return GetMemoryImage();
		}

		public MemoryImage GetMemoryImage()
		{
			if (mUnsharedImage != null)
			{
				return mUnsharedImage;
			}
			return GetDeviceImage();
		}
	}
}
