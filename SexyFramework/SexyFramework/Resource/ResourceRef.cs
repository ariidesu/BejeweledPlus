using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace SexyFramework.Resource
{
	public class ResourceRef
	{
		public BaseRes mBaseResP;

		public ResourceRef()
		{
		}

		public ResourceRef(ResourceRef theResourceRef)
		{
			mBaseResP = theResourceRef.mBaseResP;
			if (mBaseResP != null)
			{
				mBaseResP.mRefCount++;
			}
		}

		public virtual void Dispose()
		{
			Release();
		}

		public virtual ResourceRef CopyFrom(ResourceRef theResourceRef)
		{
			Release();
			mBaseResP = theResourceRef.mBaseResP;
			if (mBaseResP != null)
			{
				mBaseResP.mRefCount++;
			}
			return this;
		}

		public bool HasResource()
		{
			return mBaseResP != null;
		}

		public void Release()
		{
			if (mBaseResP != null && mBaseResP.mParent != null)
			{
				mBaseResP.mParent.Deref(mBaseResP);
			}
			mBaseResP = null;
		}

		public string GetId()
		{
			if (mBaseResP == null)
			{
				return "";
			}
			return mBaseResP.mId;
		}

		public SharedImageRef GetSharedImageRef()
		{
			if (mBaseResP == null || mBaseResP.mType != 0)
			{
				return null;
			}
			return new SharedImageRef(((ImageRes)mBaseResP).mImage);
		}

		public Image GetImage()
		{
			if (mBaseResP == null || mBaseResP.mType != 0)
			{
				return null;
			}
			return ((ImageRes)mBaseResP).mImage.GetImage();
		}

		public MemoryImage GetMemoryImage()
		{
			if (mBaseResP == null || mBaseResP.mType != 0)
			{
				return null;
			}
			return ((ImageRes)mBaseResP).mImage.GetMemoryImage();
		}

		public MemoryImage GetDeviceImage()
		{
			if (mBaseResP == null || mBaseResP.mType != 0)
			{
				return null;
			}
			return ((ImageRes)mBaseResP).mImage.GetDeviceImage();
		}

		public int GetSoundID()
		{
			if (mBaseResP == null || mBaseResP.mType != ResType.ResType_Sound)
			{
				return 0;
			}
			return ((SoundRes)mBaseResP).mSoundId;
		}

		public Font GetFont()
		{
			if (mBaseResP == null || mBaseResP.mType != ResType.ResType_Font)
			{
				return null;
			}
			return ((FontRes)mBaseResP).mFont;
		}

		public ImageFont GetImageFont()
		{
			if (mBaseResP == null || mBaseResP.mType != ResType.ResType_Font)
			{
				return null;
			}
			return (ImageFont)((FontRes)mBaseResP).mFont;
		}

		public PopAnim GetPopAnim()
		{
			if (mBaseResP == null || mBaseResP.mType != ResType.ResType_PopAnim)
			{
				return null;
			}
			return ((PopAnimRes)mBaseResP).mPopAnim;
		}

		public PIEffect GetPIEffect()
		{
			if (mBaseResP == null || mBaseResP.mType != ResType.ResType_PIEffect)
			{
				return null;
			}
			return ((PIEffectRes)mBaseResP).mPIEffect;
		}

		public RenderEffectDefinition GetRenderEffect()
		{
			if (mBaseResP == null || mBaseResP.mType != ResType.ResType_RenderEffect)
			{
				return null;
			}
			return ((RenderEffectRes)mBaseResP).mRenderEffectDefinition;
		}

		public GenericResFile GetGenericResFile()
		{
			if (mBaseResP == null || mBaseResP.mType != ResType.ResType_GenericResFile)
			{
				return null;
			}
			return ((GenericResFileRes)mBaseResP).mGenericResFile;
		}
	}
}
