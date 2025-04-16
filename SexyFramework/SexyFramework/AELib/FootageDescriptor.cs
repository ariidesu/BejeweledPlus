namespace SexyFramework.AELib
{
	internal class FootageDescriptor
	{
		public string mShortName;

		public long mId;

		public long mWidth;

		public long mHeight;

		public string mFullName;

		public FootageDescriptor()
		{
			mShortName = "";
			mId = -1L;
			mWidth = 0L;
			mHeight = 0L;
			mFullName = "";
		}

		public FootageDescriptor(string sn, long id, string fn, long w, long h)
		{
			mShortName = sn;
			mId = id;
			mFullName = fn;
			mWidth = w;
			mHeight = h;
		}
	}
}
