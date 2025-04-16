namespace SexyFramework.Resource
{
	public class GenericResFileRes : BaseRes
	{
		public GenericResFile mGenericResFile;

		public GenericResFileRes()
		{
			mType = ResType.ResType_GenericResFile;
			mGenericResFile = null;
		}

		public override void DeleteResource()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				mResourceRef.Release();
			}
			mGenericResFile = null;
			if (mGlobalPtr != null)
			{
				mGlobalPtr.mResObject = null;
			}
		}
	}
}
