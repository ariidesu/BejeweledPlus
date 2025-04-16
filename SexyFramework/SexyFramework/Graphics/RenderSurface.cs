namespace SexyFramework.Graphics
{
	public class RenderSurface
	{
		public int mData;

		public object mPtr;

		private uint mRefCount;

		public RenderSurface()
		{
			mRefCount = 0u;
			mData = 0;
			mPtr = null;
		}

		public virtual void Dispose()
		{
		}

		public void AddRef()
		{
			mRefCount++;
		}

		public void Release()
		{
			mRefCount--;
			uint mRefCount2 = mRefCount;
			int num = 0;
		}
	}
}
