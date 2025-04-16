using System;
using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class SimpleObjectPool<T> : IDisposable where T : new()
	{
		public static int OBJECT_POOL_SIZE = 8192;

		public int mNumPools;

		public int mNumAvailObjects;

		public List<T> mDataPools;

		public List<T> mFreeData;

		public SimpleObjectPool()
		{
			mNumPools = 0;
			mNumAvailObjects = 0;
			mFreeData = null;
			mDataPools = null;
			mDataPools = new List<T>();
			mFreeData = new List<T>();
		}

		public virtual void Dispose()
		{
			mDataPools.Clear();
			mFreeData.Clear();
		}

		public void GrowPool()
		{
		}

		public T Alloc()
		{
			T val = default(T);
			val = new T();
			mDataPools.Add(val);
			return val;
		}

		public void Free(T thePtr)
		{
			mDataPools.Remove(thePtr);
		}
	}
}
