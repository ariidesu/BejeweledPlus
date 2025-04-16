using System;

namespace SexyFramework.Resource
{
	public class ResourceManagerException : Exception
	{
		private string msg;

		public ResourceManagerException(string p)
			: base(p)
		{
			msg = p;
		}
	}
}
