using System;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class Image : IDisposable
	{
		protected ImageFlags mImageFlags;

		public object mRenderData;

		public string mFileName;

		public string mNameForRes = "";

		public bool mDrawn;

		public string mFilePath;

		public int mWidth;

		public int mHeight;

		public Rect mCelRect = default(Rect);

		public Rect mRect = Rect.ZERO_RECT;

		public int mNumRows = 1;

		public int mNumCols = 1;

		public AnimInfo mAnimInfo;

		public Image mAtlasImage;

		public int mAtlasStartX;

		public int mAtlasStartY;

		public int mAtlasEndX;

		public int mAtlasEndY;

		public Vector2 mVectorU = new Vector2(1f, 0f);

		public Vector2 mVectorV = new Vector2(0f, 1f);

		public Vector2 mVectorBase = new Vector2(0f, 0f);

		public bool mAtlasValidate;

		public Image()
		{
			mImageFlags = ImageFlags.ImageFlag_NONE;
			mRenderData = null;
			mAtlasImage = null;
			mAtlasStartX = 0;
			mAtlasStartY = 0;
			mAtlasEndX = 0;
			mAtlasEndY = 0;
			mWidth = 0;
			mHeight = 0;
			mNumRows = 1;
			mNumCols = 1;
			mAnimInfo = null;
			mDrawn = false;
		}

		public Image(Image theImage)
		{
			mImageFlags = theImage.mImageFlags;
			mRenderData = null;
			mWidth = theImage.mWidth;
			mHeight = theImage.mHeight;
			mNumRows = theImage.mNumRows;
			mNumCols = theImage.mNumCols;
			mAtlasImage = theImage.mAtlasImage;
			mAtlasStartX = theImage.mAtlasStartX;
			mAtlasStartY = theImage.mAtlasStartY;
			mAtlasEndX = theImage.mAtlasEndX;
			mAtlasEndY = theImage.mAtlasEndY;
			mDrawn = false;
			if (theImage.mAnimInfo != null)
			{
				mAnimInfo = theImage.mAnimInfo;
			}
			else
			{
				mAnimInfo = null;
			}
		}

		public void InitAtalasState()
		{
			if (mAtlasValidate)
			{
				return;
			}
			if (mAtlasImage != null)
			{
				float x = (float)mAtlasStartX / (float)mAtlasImage.mWidth;
				float num = (float)mAtlasStartY / (float)mAtlasImage.mHeight;
				float x2 = (float)mAtlasEndX / (float)mAtlasImage.mWidth;
				float num2 = (float)mAtlasEndY / (float)mAtlasImage.mHeight;
				mVectorBase = new Vector2(x, num);
				if (num2 < num)
				{
					mVectorU = new Vector2(x, num2) - mVectorBase;
					mVectorV = new Vector2(x2, num) - mVectorBase;
				}
				else
				{
					mVectorU = new Vector2(x2, num) - mVectorBase;
					mVectorV = new Vector2(x, num2) - mVectorBase;
				}
			}
			mAtlasValidate = true;
		}

		public virtual void Dispose()
		{
			if (mAnimInfo != null)
			{
				mAnimInfo.Dispose();
			}
			mAnimInfo = null;
		}

		public virtual MemoryImage AsMemoryImage()
		{
			return null;
		}

		public virtual DeviceImage AsDeviceImage()
		{
			return null;
		}

		public int GetWidth()
		{
			return mWidth;
		}

		public int GetHeight()
		{
			return mHeight;
		}

		public Rect GetRect()
		{
			return new Rect(0, 0, mWidth, mHeight);
		}

		public int GetCelWidth()
		{
			return mWidth / mNumCols;
		}

		public int GetCelHeight()
		{
			return mHeight / mNumRows;
		}

		public int GetCelCount()
		{
			return mNumCols * mNumRows;
		}

		public int GetAnimCel(int theTime)
		{
			if (mAnimInfo == null)
			{
				return 0;
			}
			return mAnimInfo.GetCel(theTime);
		}

		public Rect GetAnimCelRect(int theTime)
		{
			int animCel = GetAnimCel(theTime);
			int celWidth = GetCelWidth();
			int celHeight = GetCelHeight();
			if (mNumCols > 1)
			{
				return new Rect(animCel * celWidth, 0, celWidth, mHeight);
			}
			return new Rect(0, animCel * celHeight, mWidth, celHeight);
		}

		public Rect GetCelRect(int theCel)
		{
			mCelRect.mHeight = GetCelHeight();
			mCelRect.mWidth = GetCelWidth();
			mCelRect.mX = theCel % mNumCols * mCelRect.mWidth;
			mCelRect.mY = theCel / mNumCols * mCelRect.mHeight;
			return mCelRect;
		}

		public Rect GetCelRect(int theCol, int theRow)
		{
			mCelRect.mHeight = GetCelHeight();
			mCelRect.mWidth = GetCelWidth();
			mCelRect.mX = theCol * mCelRect.mWidth;
			mCelRect.mY = theRow * mCelRect.mHeight;
			return mCelRect;
		}

		public void CopyAttributes(Image from)
		{
			mNumCols = from.mNumCols;
			mNumRows = from.mNumRows;
			mAnimInfo = null;
			if (from.mAnimInfo != null)
			{
				mAnimInfo = new AnimInfo(from.mAnimInfo);
			}
		}

		public void ReplaceImageFlags(uint inFlags)
		{
			mImageFlags = (ImageFlags)inFlags;
		}

		public void AddImageFlags(uint inFlags)
		{
			mImageFlags |= (ImageFlags)inFlags;
		}

		public void RemoveImageFlags(uint inFlags)
		{
			mImageFlags &= (ImageFlags)(~inFlags);
		}

		public bool HasImageFlag(uint inFlag)
		{
			return ((uint)mImageFlags & inFlag) != 0;
		}

		public ulong GetImageFlags()
		{
			return (ulong)mImageFlags;
		}

		public object GetRenderData()
		{
			return mRenderData;
		}

		public void SetRenderData(object inRenderData)
		{
			mRenderData = inRenderData;
		}

		public bool CreateRenderData()
		{
			MemoryImage memoryImage = AsMemoryImage();
			if (memoryImage != null && GlobalMembers.gSexyAppBase.mGraphicsDriver != null && GlobalMembers.gSexyAppBase.mGraphicsDriver.GetRenderDevice3D() != null)
			{
				mRenderData = GlobalMembers.gSexyAppBase.mAppDriver.GetOptimizedRenderData(memoryImage.mFileName);
				if (mRenderData != null)
				{
					return true;
				}
			}
			return false;
		}
	}
}
