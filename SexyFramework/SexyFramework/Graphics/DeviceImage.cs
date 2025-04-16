using System;
using SexyFramework.Drivers;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class DeviceImage : MemoryImage
	{
		public IGraphicsDriver mDriver;

		public bool mSurfaceSet;

		public bool mNoLock;

		public bool mWantDeviceSurface;

		public bool mDrawToBits;

		public int mLockCount;

		public DeviceSurface mSurface;

		protected void DeleteAllNonSurfaceData()
		{
			mBits = null;
			mBits = null;
			mNativeAlphaData = null;
			mNativeAlphaData = null;
			mRLAdditiveData = null;
			mRLAdditiveData = null;
			mRLAlphaData = null;
			mRLAlphaData = null;
			mColorTable = null;
			mColorTable = null;
			mColorIndices = null;
			mColorIndices = null;
		}

		private void Init()
		{
			mSurface = null;
			mNoLock = false;
			mWantDeviceSurface = false;
			mDrawToBits = false;
			mSurfaceSet = false;
			mLockCount = 0;
		}

		public override DeviceImage AsDeviceImage()
		{
			return this;
		}

		public bool GenerateDeviceSurface()
		{
			if (mSurface != null)
			{
				return false;
			}
			return false;
		}

		public void DeleteDeviceSurface()
		{
			if (mSurface != null)
			{
				if (mColorTable == null && mBits == null && GetRenderData() == null)
				{
					GetBits();
				}
				mSurface = null;
				mSurface = null;
			}
		}

		public void ReInit()
		{
			if (mWantDeviceSurface)
			{
				GenerateDeviceSurface();
			}
		}

		public override void BitsChanged()
		{
			mSurface = null;
			mSurface = null;
		}

		public void CommitBits()
		{
			DeviceSurface mSurface2 = mSurface;
		}

		public virtual bool LockSurface()
		{
			return true;
		}

		public virtual bool UnlockSurface()
		{
			return true;
		}

		public virtual void SetSurface(IntPtr theSurface)
		{
			mSurfaceSet = true;
			if (mSurface != null)
			{
				int version = mDriver.GetVersion();
				if (mSurface.GetVersion() != version)
				{
					mSurface = null;
					mSurface = null;
				}
			}
			DeviceSurface mSurface2 = mSurface;
			mSurface.SetSurface(theSurface);
			mSurface.GetDimensions(mWidth, mHeight);
			mNoLock = false;
		}

		public override void Create(int theWidth, int theHeight)
		{
			base.Create(theWidth, theHeight);
			mBits = null;
			mWidth = theWidth;
			mHeight = theHeight;
			mBits = null;
			BitsChanged();
		}

		public void BltF(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, Color theColor, int theDrawMode)
		{
			theImage.mDrawn = true;
			CommitBits();
		}

		public void BltRotated(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, Color theColor, int theDrawMode, double theRot, float theRotCenterX, float theRotCenterY)
		{
		}

		public void BltStretched(Image theImage, Rect theDestRectOrig, Rect theSrcRectOrig, Rect theClipRect, Color theColor, int theDrawMode, int fastStretch)
		{
		}

		public void BltStretched(Image theImage, Rect theDestRectOrig, Rect theSrcRectOrig, Rect theClipRect, Color theColor, int theDrawMode, int fastStretch, int mirror)
		{
		}

		public override bool Palletize()
		{
			return false;
		}

		public override void PurgeBits()
		{
			if (!mSurfaceSet)
			{
				mPurgeBits = true;
				mBits = null;
				mBits = null;
				mColorIndices = null;
				mColorIndices = null;
				mColorTable = null;
				mColorTable = null;
			}
		}

		public void DeleteNativeData()
		{
			if (!mSurfaceSet)
			{
				DeleteDeviceSurface();
			}
		}

		public void DeleteExtraBuffers()
		{
			if (!mSurfaceSet)
			{
				DeleteDeviceSurface();
			}
		}

		public static int CheckCache(string theSrcFile, string theAltData)
		{
			return 0;
		}

		public static int SetCacheUpToDate(string theSrcFile, string theAltData)
		{
			return 0;
		}

		public static DeviceImage ReadFromCache(string theSrcFile, string theAltData)
		{
			return null;
		}

		public virtual void WriteToCache(string theSrcFile, string theAltData)
		{
		}

		internal void AddImageFlags(ImageFlags imageFlags)
		{
			mImageFlags |= imageFlags;
		}
	}
}
