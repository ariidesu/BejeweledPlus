using System;

namespace SexyFramework.Misc
{
	public class AutoCrit : IDisposable
	{
		private CritSect mCritSec;

		public AutoCrit(CritSect theCritSect)
		{
			mCritSec = theCritSect;
			mCritSec.Lock();
		}

		public void Dispose()
		{
			mCritSec.Unlock();
		}
	}
}
