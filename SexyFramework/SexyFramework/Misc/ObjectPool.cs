using System.Collections.Generic;
using System.Linq;

namespace SexyFramework.Misc
{
	public class ObjectPool<T> where T : new()
	{
		public int mPoolSize;

		public int mNumAvailObjects;

		public List<T> mDataPools;

		public List<T> mFreePools;

		public int mNextAvailIndex;

		public ObjectPool(int size)
		{
			mNumAvailObjects = 0;
			mNextAvailIndex = 0;
			mPoolSize = size;
			mFreePools = new List<T>();
			mNumAvailObjects += mPoolSize;
		}

		public virtual void Dispose()
		{
			mFreePools.Clear();
		}

		public void GrowPool()
		{
			mNumAvailObjects += mPoolSize;
			mDataPools.Capacity = mNumAvailObjects;
		}

		public T Alloc()
		{
			if (mFreePools.Count > 0)
			{
				T result = mFreePools.Last();
				mFreePools.RemoveAt(mFreePools.Count - 1);
				return result;
			}
			return new T();
		}

		public void Free(T thePtr)
		{
			mFreePools.Add(thePtr);
		}
	}
}
