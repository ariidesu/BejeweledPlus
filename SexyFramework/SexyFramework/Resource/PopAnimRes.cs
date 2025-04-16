using SexyFramework.Widget;

namespace SexyFramework.Resource
{
	public class PopAnimRes : BaseRes
	{
		public PopAnim mPopAnim;

		public PopAnimRes()
		{
			mType = ResType.ResType_PopAnim;
			mPopAnim = null;
		}

		public override void DeleteResource()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				mResourceRef.Release();
			}
			else if (mPopAnim != null)
			{
				mPopAnim.Dispose();
			}
			mPopAnim = null;
			if (mGlobalPtr != null)
			{
				mGlobalPtr.mResObject = null;
			}
		}
	}
}
