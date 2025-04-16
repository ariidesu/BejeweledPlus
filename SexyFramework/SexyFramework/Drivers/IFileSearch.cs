namespace SexyFramework.Drivers
{
	public class IFileSearch
	{
		public enum SearchType
		{
			UNKNOWN,
			PAK_FILE_INTERNAL,
			DRIVER_INTERNAL
		}

		protected SearchType mSearchType;

		public virtual void Dispose()
		{
		}

		public SearchType GetSearchType()
		{
			return mSearchType;
		}

		protected IFileSearch()
		{
			mSearchType = SearchType.UNKNOWN;
		}
	}
}
