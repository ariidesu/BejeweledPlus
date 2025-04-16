using System;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Graphics;

namespace SexyFramework.Drivers.Graphics
{
	public class XNATextureData : IDisposable
	{
		public XNATextureDataPiece[] mTextures;

		private BaseXNARenderDevice mDevice;

		public int mPaletteIndex;

		public bool mOptimizedLoad;

		public int mWidth;

		public int mHeight;

		public int mTexVecWidth;

		public int mTexVecHeight;

		public int mTexPieceWidth;

		public int mTexPieceHeight;

		public int mBitsChangedCount;

		public int mTexMemSize;

		public int mTexMemOriginalSize;

		public ulong mTexMemFlushRevision;

		public float mMaxTotalU;

		public float mMaxTotalV;

		public PixelFormat mPixelFormat;

		public ulong mImageFlags;

		public XNATextureData(BaseXNARenderDevice theDevice)
		{
			mDevice = theDevice;
			mWidth = 0;
			mHeight = 0;
			mTexVecWidth = 0;
			mTexVecHeight = 0;
			mBitsChangedCount = 0;
			mTexMemSize = 0;
			mTexMemOriginalSize = 0;
			mTexMemFlushRevision = 0uL;
			mTexPieceWidth = 64;
			mTexPieceHeight = 64;
			mTextures = new XNATextureDataPiece[3];
			mTextures[0] = new XNATextureDataPiece();
			mPaletteIndex = -1;
			mPixelFormat = PixelFormat.PixelFormat_Unknown;
			mImageFlags = 0uL;
			mOptimizedLoad = false;
		}

		public virtual void Dispose()
		{
			ReleaseTextures();
		}

		private void ReleaseTextures()
		{
			for (int i = 0; i < mTextures.Length; i++)
			{
				if (mTextures[i] != null)
				{
					if (mTextures[i].mTexture != null)
					{
						GlobalMembers.gTotalGraphicsMemory -= mTextures[i].mTexture.Width * mTextures[i].mTexture.Height * 4;
						mTextures[i].mTexture.Dispose();
						mTextures[i].mTexture = null;
					}
					if (mTextures[i].mCubeTexture != null)
					{
						mTextures[i].mCubeTexture.Dispose();
						mTextures[i].mCubeTexture = null;
					}
					if (mTextures[i].mVolumeTexture != null)
					{
						mTextures[i].mVolumeTexture.Dispose();
						mTextures[i].mVolumeTexture = null;
					}
				}
			}
		}

		public void CreateTextures(ref MemoryImage theImage, BaseXNARenderDevice theDevice, bool commitBits)
		{
			theImage.DeleteSWBuffers();
			PixelFormat pixelFormat = PixelFormat.PixelFormat_A8R8G8B8;
			if (!theImage.mHasAlpha && !theImage.mHasTrans)
			{
				pixelFormat = PixelFormat.PixelFormat_X8R8G8B8;
			}
			if (theImage.HasImageFlag(4u) && pixelFormat == PixelFormat.PixelFormat_A8R8G8B8 && (theDevice.mSupportedTextureFormats & 2) != 0)
			{
				pixelFormat = PixelFormat.PixelFormat_A4R4G4B4;
			}
			if (pixelFormat == PixelFormat.PixelFormat_A8R8G8B8 && (theDevice.mSupportedTextureFormats & 1) == 0)
			{
				pixelFormat = PixelFormat.PixelFormat_A4R4G4B4;
			}
			bool flag = false;
			if (mWidth != theImage.mWidth || mHeight != theImage.mHeight || pixelFormat != mPixelFormat || theImage.GetImageFlags() != mImageFlags)
			{
				ReleaseTextures();
				mPixelFormat = pixelFormat;
				mImageFlags = theImage.GetImageFlags();
				flag = true;
			}
			int height = theImage.GetHeight();
			int width = theImage.GetWidth();
			if (mPaletteIndex != -1)
			{
				mTexMemSize += 1024;
				mTexMemOriginalSize += 1024;
			}
			int num = 4;
			switch (pixelFormat)
			{
			case PixelFormat.PixelFormat_Palette8:
				num = 1;
				break;
			case PixelFormat.PixelFormat_R5G6B5:
				num = 2;
				break;
			case PixelFormat.PixelFormat_A4R4G4B4:
				num = 2;
				break;
			}
			if ((mImageFlags & 0x20) != 0)
			{
				XNATextureDataPiece xNATextureDataPiece = mTextures[0];
				if (flag)
				{
					if (xNATextureDataPiece.mCubeTexture == null)
					{
						mPixelFormat = PixelFormat.PixelFormat_Unknown;
						return;
					}
					int num2 = theImage.GetWidth() * theImage.GetHeight() * num;
					mTexMemSize += num2;
					mTexMemOriginalSize += num2;
				}
				mWidth = theImage.GetWidth();
				mHeight = theImage.GetHeight();
				mBitsChangedCount = theImage.mBitsChangedCount;
				mPixelFormat = pixelFormat;
				return;
			}
			if ((mImageFlags & 0x40) != 0)
			{
				XNATextureDataPiece xNATextureDataPiece2 = mTextures[0];
				if (flag)
				{
					if (xNATextureDataPiece2.mVolumeTexture == null)
					{
						mPixelFormat = PixelFormat.PixelFormat_Unknown;
						return;
					}
					int num3 = theImage.GetWidth() * theImage.GetHeight() * num;
					mTexMemSize += num3;
					mTexMemOriginalSize += num3;
				}
				mWidth = theImage.GetWidth();
				mHeight = theImage.GetHeight();
				mBitsChangedCount = theImage.mBitsChangedCount;
				mPixelFormat = pixelFormat;
				return;
			}
			int num4 = 0;
			for (int i = 0; i < height; i += mTexPieceHeight)
			{
				int num5 = 0;
				while (num5 < width)
				{
					XNATextureDataPiece xNATextureDataPiece3 = mTextures[num4];
					if (flag)
					{
						xNATextureDataPiece3.mTexture = theDevice.CreateTexture2D(xNATextureDataPiece3.mWidth, xNATextureDataPiece3.mHeight, pixelFormat, false, this, mTextures);
						if (xNATextureDataPiece3.mTexture == null)
						{
							mPixelFormat = PixelFormat.PixelFormat_Unknown;
							return;
						}
						mTexMemSize += xNATextureDataPiece3.mWidth * xNATextureDataPiece3.mHeight * num;
					}
					if (theImage.HasImageFlag(16u))
					{
						if (theImage.mBits == null)
						{
						}
					}
					else if (commitBits)
					{
						mDevice.CopyImageToTexture(ref xNATextureDataPiece3.mTexture, xNATextureDataPiece3.mTexFormat, theImage, num5, i, xNATextureDataPiece3.mWidth, xNATextureDataPiece3.mHeight, pixelFormat);
					}
					num5 += mTexPieceWidth;
					num4++;
				}
			}
			if (flag)
			{
				mTexMemOriginalSize += theImage.GetWidth() * theImage.GetHeight() * num;
			}
			mWidth = theImage.mWidth;
			mHeight = theImage.mHeight;
			mBitsChangedCount = theImage.mBitsChangedCount;
			mPixelFormat = pixelFormat;
		}

		private void CheckCreateTextures(ref MemoryImage theImage, ref BaseXNARenderDevice theDevice)
		{
			if (mPixelFormat == PixelFormat.PixelFormat_Unknown || theImage.mWidth != mWidth || theImage.mHeight != mHeight || theImage.mBitsChangedCount != mBitsChangedCount || theImage.GetImageFlags() != mImageFlags)
			{
				if (mOptimizedLoad)
				{
					mImageFlags = theImage.GetImageFlags();
				}
				else
				{
					CreateTextures(ref theImage, theDevice, true);
				}
			}
		}

		public Texture2D GetTexture(MemoryImage theOrigImage, int x, int y, ref int width, ref int height, ref float u1, ref float v1, ref float u2, ref float v2)
		{
			if ((mImageFlags & 0x60) != 0)
			{
				return null;
			}
			int num = x / mTexPieceWidth;
			int num2 = y / mTexPieceHeight;
			XNATextureDataPiece xNATextureDataPiece = mTextures[num2 * mTexVecWidth + num];
			int num3 = x % mTexPieceWidth;
			int num4 = y % mTexPieceHeight;
			int num5 = num3 + width;
			int num6 = num4 + height;
			if (num5 > xNATextureDataPiece.mWidth)
			{
				num5 = xNATextureDataPiece.mWidth;
			}
			if (num6 > xNATextureDataPiece.mHeight)
			{
				num6 = xNATextureDataPiece.mHeight;
			}
			width = num5 - num3;
			height = num6 - num4;
			if ((mImageFlags & 0x200) != 0)
			{
				u1 = (float)num3 / (float)theOrigImage.mWidth;
				v1 = (float)num4 / (float)theOrigImage.mHeight;
				u2 = (float)num5 / (float)theOrigImage.mWidth;
				v2 = (float)num6 / (float)theOrigImage.mHeight;
			}
			else
			{
				u1 = (float)num3 / (float)xNATextureDataPiece.mWidth;
				v1 = (float)num4 / (float)xNATextureDataPiece.mHeight;
				u2 = (float)num5 / (float)xNATextureDataPiece.mWidth;
				v2 = (float)num6 / (float)xNATextureDataPiece.mHeight;
			}
			return xNATextureDataPiece.mTexture;
		}

		private Texture2D GetTextureF(float x, float y, ref float width, ref float height, ref float u1, ref float v1, ref float u2, ref float v2)
		{
			if ((mImageFlags & 0x60) != 0)
			{
				return null;
			}
			int num = (int)(x / (float)mTexPieceWidth);
			int num2 = (int)(y / (float)mTexPieceHeight);
			XNATextureDataPiece xNATextureDataPiece = mTextures[num2 * mTexVecWidth + num];
			float num3 = x - (float)(num * mTexPieceWidth);
			float num4 = y - (float)(num2 * mTexPieceHeight);
			float num5 = num3 + width;
			float num6 = num4 + height;
			if (num5 > (float)xNATextureDataPiece.mWidth)
			{
				num5 = xNATextureDataPiece.mWidth;
			}
			if (num6 > (float)xNATextureDataPiece.mHeight)
			{
				num6 = xNATextureDataPiece.mHeight;
			}
			width = num5 - num3;
			height = num6 - num4;
			u1 = num3 / (float)xNATextureDataPiece.mWidth;
			v1 = num4 / (float)xNATextureDataPiece.mHeight;
			u2 = num5 / (float)xNATextureDataPiece.mWidth;
			v2 = num6 / (float)xNATextureDataPiece.mHeight;
			return xNATextureDataPiece.mTexture;
		}

		private TextureCube GetCubeTexture()
		{
			if ((mImageFlags & 0x20) == 0)
			{
				return null;
			}
			return mTextures[0].mCubeTexture;
		}

		private Texture3D GetVolumeTexture()
		{
			if ((mImageFlags & 0x40) == 0)
			{
				return null;
			}
			return mTextures[0].mVolumeTexture;
		}
	}
}
