using System;

namespace SexyFramework.Resource
{
	public abstract class DataElement : IDisposable
	{
		public bool mIsList;

		public DataElement()
		{
			mIsList = false;
		}

		public virtual void Dispose()
		{
		}

		public abstract DataElement Duplicate();
	}
}
