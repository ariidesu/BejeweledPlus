using SexyFramework.Graphics;

namespace SexyFramework.Resource
{
	public class PIEffectRes : BaseRes
	{
		public PIEffect mPIEffect;

		public PIEffectRes()
		{
			mType = ResType.ResType_PIEffect;
			mPIEffect = null;
		}

		public override void DeleteResource()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				mResourceRef.Release();
			}
			mPIEffect = null;
			if (mGlobalPtr != null)
			{
				mGlobalPtr.mResObject = null;
			}
		}
	}
}
