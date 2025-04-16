using System.Runtime.InteropServices;

namespace SexyFramework.Misc
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct CritSect
	{
		public bool TryLock()
		{
			return false;
		}

		public void Lock()
		{
		}

		public void Unlock()
		{
		}
	}
}
