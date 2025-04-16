using SexyFramework.Graphics;

namespace SexyFramework.Resource
{
	public class RenderEffectRes : BaseRes
	{
		public RenderEffectDefinition mRenderEffectDefinition;

		public string mSrcFilePath;

		public RenderEffectRes()
		{
			mType = ResType.ResType_RenderEffect;
			mRenderEffectDefinition = null;
		}

		public override void DeleteResource()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				mResourceRef.Release();
			}
			else if (mRenderEffectDefinition != null)
			{
				mRenderEffectDefinition.Dispose();
			}
			mRenderEffectDefinition = null;
			if (mGlobalPtr != null)
			{
				mGlobalPtr.mResObject = null;
			}
		}
	}
}
