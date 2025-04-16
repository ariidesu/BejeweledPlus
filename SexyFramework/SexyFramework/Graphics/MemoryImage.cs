using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SexyFramework.Drivers.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class MemoryImage : Image
	{
		public class TriRep
		{
			public class Tri
			{
				public class Point
				{
					public float u;

					public float v;
				}

				public Point[] p = new Point[3];

				public Tri()
				{
				}

				public Tri(float inU0, float inV0, float inU1, float inV1, float inU2, float inV2)
				{
					p[0].u = inU0;
					p[0].v = inV0;
					p[1].u = inU1;
					p[1].v = inV1;
					p[2].u = inU2;
					p[2].v = inV2;
				}
			}

			public class Level
			{
				public class Region
				{
					public Rect mRect = default(Rect);

					public List<Tri> mTris = new List<Tri>();
				}

				public int mDetailX;

				public int mDetailY;

				public int mRegionWidth;

				public int mRegionHeight;

				public List<Region> mRegions = new List<Region>();

				public void GetRegionTris(ref List<Tri> outTris, MemoryImage inImage, Rect inSrcRect, int inAllowRotation)
				{
					if (mRegions.Count == 0 || mRegionWidth != inImage.mNumCols || mRegionHeight != inImage.mNumRows)
					{
						return;
					}
					int num = inImage.mWidth / mRegionWidth;
					int num2 = inImage.mHeight / mRegionHeight;
					if (inSrcRect.mWidth == num && inSrcRect.mHeight == num2)
					{
						int num3 = inSrcRect.mX / num;
						int num4 = inSrcRect.mY / num2;
						if (num3 < mRegionWidth && num4 < mRegionHeight)
						{
							Region region = mRegions[num4 * mRegionWidth + num3];
							outTris = region.mTris;
						}
					}
				}

				public Tri GetRegionTrisPtr(ref int outTriCount, MemoryImage inImage, Rect inSrcRect, int inAllowRotation)
				{
					if (mRegions.Count == 0)
					{
						return null;
					}
					if (mRegionWidth != inImage.mNumCols || mRegionHeight != inImage.mNumRows)
					{
						return null;
					}
					int num = inImage.mWidth / mRegionWidth;
					int num2 = inImage.mHeight / mRegionHeight;
					if (inSrcRect.mWidth != num || inSrcRect.mHeight != num2)
					{
						return null;
					}
					int num3 = inSrcRect.mX / num;
					int num4 = inSrcRect.mY / num2;
					if (num3 < mRegionWidth && num4 < mRegionHeight)
					{
						Region region = mRegions[num4 * mRegionWidth + num3];
						outTriCount = region.mTris.Count;
						return region.mTris[0];
					}
					return null;
				}
			}

			public List<Level> mLevels = new List<Level>();

			public Level GetMinLevel()
			{
				if (mLevels.Count != 0)
				{
					return mLevels[0];
				}
				return null;
			}

			public Level GetMaxLevel()
			{
				if (mLevels.Count != 0)
				{
					return mLevels[mLevels.Count - 1];
				}
				return null;
			}

			public Level GetLevelForScreenSpaceUsage(float inUsageFrac, int inAllowRotation)
			{
				if (mLevels.Count == 0)
				{
					return null;
				}
				for (int num = mLevels.Count - 1; num >= 0; num--)
				{
					Level result = mLevels[num];
					if (inUsageFrac > 0.001f)
					{
						return result;
					}
				}
				return null;
			}
		}

		public uint[] mBits;

		public int mBitsChangedCount;

		public uint[] mColorTable;

		public byte[] mColorIndices;

		public bool mForcedMode;

		public bool mHasTrans;

		public bool mHasAlpha;

		public bool mIsVolatile;

		public bool mPurgeBits;

		public bool mWantPal;

		public bool mDither16;

		public uint[] mNativeAlphaData;

		public byte[] mRLAlphaData;

		public byte[] mRLAdditiveData;

		public bool mBitsChanged;

		public SexyAppBase mApp;

		public TriRep mNormalTriRep = new TriRep();

		public TriRep mAdditiveTriRep = new TriRep();

		public MemoryImage()
		{
			mApp = GlobalMembers.gSexyAppBase;
		}

		public MemoryImage(MemoryImage rhs)
			: base(rhs)
		{
			mApp = rhs.mApp;
			mHasAlpha = rhs.mHasAlpha;
			mHasTrans = rhs.mHasTrans;
			mBitsChanged = rhs.mBitsChanged;
			mIsVolatile = rhs.mIsVolatile;
			mPurgeBits = rhs.mPurgeBits;
			mWantPal = rhs.mWantPal;
			mBitsChangedCount = rhs.mBitsChangedCount;
			bool flag = false;
			if (rhs.mBits == null && rhs.mColorTable == null)
			{
				rhs.GetBits();
				flag = true;
			}
			if (rhs.mBits == null)
			{
				mBits = null;
			}
			if (rhs.mColorTable == null)
			{
				mColorTable = null;
			}
			if (rhs.mColorIndices == null)
			{
				mColorIndices = null;
			}
			if (rhs.mNativeAlphaData != null)
			{
				if (rhs.mColorTable != null)
				{
				}
			}
			else
			{
				mNativeAlphaData = null;
			}
			if (rhs.mRLAlphaData != null)
			{
				mRLAlphaData = new byte[mWidth * mHeight];
			}
			else
			{
				mRLAlphaData = null;
			}
			if (rhs.mRLAdditiveData != null)
			{
				mRLAdditiveData = new byte[mWidth * mHeight];
			}
			else
			{
				mRLAdditiveData = null;
			}
			mApp.AddMemoryImage(this);
		}

		public override void Dispose()
		{
			base.Dispose();
			if (mRenderData != null)
			{
				GlobalMembers.gSexyAppBase.mGraphicsDriver.Remove3DData(this);
			}
			mRenderData = null;
		}

		private void Init()
		{
			mBits = null;
			mColorTable = null;
			mColorIndices = null;
			mNativeAlphaData = null;
			mRLAlphaData = null;
			mRLAdditiveData = null;
			mHasTrans = false;
			mHasAlpha = false;
			mBitsChanged = false;
			mForcedMode = false;
			mIsVolatile = false;
			mBitsChangedCount = 0;
			mPurgeBits = false;
			mWantPal = false;
			mDither16 = false;
			mApp.AddMemoryImage(this);
		}

		public virtual object GetNativeAlphaData(NativeDisplay theDisplay)
		{
			throw new NotImplementedException();
		}

		public virtual byte[] GetRLAlphaData()
		{
			throw new NotImplementedException();
		}

		public virtual byte[] GetRLAdditiveData(NativeDisplay theNative)
		{
			throw new NotImplementedException();
		}

		public virtual void PurgeBits()
		{
			mPurgeBits = true;
			if (mApp.Is3DAccelerated())
			{
				if (GetRenderData() == null)
				{
					return;
				}
			}
			else
			{
				if (mBits == null && mColorIndices == null)
				{
					return;
				}
				GetNativeAlphaData(GlobalMembers.gSexyAppBase.mGraphicsDriver.GetNativeDisplayInfo());
			}
			mBits = null;
			mBits = null;
			if (GetRenderData() != null)
			{
				mColorIndices = null;
				mColorIndices = null;
				mColorTable = null;
				mColorTable = null;
			}
		}

		public virtual void DeleteSWBuffers()
		{
			if (mNativeAlphaData != null || mRLAdditiveData != null || mRLAlphaData != null)
			{
				if (mBits == null && mColorIndices == null)
				{
					GetBits();
				}
				mNativeAlphaData = null;
				mNativeAlphaData = null;
				mRLAdditiveData = null;
				mRLAdditiveData = null;
				mRLAlphaData = null;
				mRLAlphaData = null;
			}
		}

		public virtual void Create(int theWidth, int theHeight)
		{
			mBits = null;
			mBits = null;
			mWidth = theWidth;
			mHeight = theHeight;
			mHasTrans = true;
			mHasAlpha = true;
		}

		public override MemoryImage AsMemoryImage()
		{
			return this;
		}

		public uint[] GetBits()
		{
			if (mBits == null)
			{
				int num = mWidth * mHeight;
				mBits = new uint[num];
				if (mColorTable != null)
				{
					for (int i = 0; i < num; i++)
					{
						mBits[i] = mColorTable[mColorIndices[i]];
					}
					mColorIndices = null;
					mColorIndices = null;
					mColorTable = null;
					mColorTable = null;
					mNativeAlphaData = null;
					mNativeAlphaData = null;
				}
				else if (mNativeAlphaData == null)
				{
					if (GetRenderData() != null && (GetRenderData() as XNATextureData).mTextures[0].mTexture != null)
					{
						(GetRenderData() as XNATextureData).mTextures[0].mTexture.GetData(mBits);
					}
					else
					{
						MemoryImage memoryImage = ((mAtlasImage != null) ? mAtlasImage.AsMemoryImage() : null);
						if (memoryImage != null)
						{
							uint[] bits = memoryImage.GetBits();
							Array.Copy(bits, mAtlasStartY * memoryImage.mWidth + mAtlasStartX, mBits, 0, mBits.Length);
						}
					}
				}
			}
			return mBits;
		}

		public virtual void SetBits(uint[] theBits, int theWidth, int theHeight)
		{
			SetBits(theBits, theWidth, theHeight, true);
		}

		public virtual void SetBits(uint[] theBits, int theWidth, int theHeight, bool commitBits)
		{
			mColorIndices = null;
			mColorIndices = null;
			mColorTable = null;
			mColorTable = null;
			mBits = null;
			mBits = new uint[theWidth * theHeight];
			mWidth = theWidth;
			mHeight = theHeight;
			mBits = theBits;
		}

		public virtual bool Palletize()
		{
			return true;
		}

		public static int GetWinding(int p0x, int p0y, int p1x, int p1y, int p2x, int p2y)
		{
			return (p1x - p0x) * (p2y - p0y) - (p1y - p0y) * (p2x - p0x);
		}

		public static void AddTri(ref List<TriRep.Tri> outTris, Vector2[] inTri, int inWidth, int inHeight, int inGroup)
		{
		}

		public virtual void SetImageMode(bool hasTrans, bool hasAlpha)
		{
			mForcedMode = true;
			mHasTrans = hasTrans;
			mHasAlpha = hasAlpha;
		}

		public virtual void BitsChanged()
		{
		}

		internal void Clear()
		{
		}
	}
}
