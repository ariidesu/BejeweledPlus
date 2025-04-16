using System;
using SexyFramework.Drivers;

namespace SexyFramework.Resource
{
	public class PFILE
	{
		private string mFilename;

		private IFile mFileHandler;

		public PFILE(string theFilename, string mode)
		{
			mFilename = theFilename;
		}

		internal void Close()
		{
			try
			{
				if (mFileHandler != null)
				{
					mFileHandler.Close();
					mFileHandler = null;
				}
			}
			catch (Exception)
			{
			}
		}

		public bool Open()
		{
			try
			{
				mFileHandler = Common.GetGameFileDriver().CreateFile(mFilename);
				mFileHandler.ForceLoad();
				return !mFileHandler.HasError();
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool Open<T>()
		{
			try
			{
				mFileHandler = Common.GetGameFileDriver().CreateFile(mFilename);
				mFileHandler.ForceLoadObject<T>();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public byte[] GetData()
		{
			if (mFileHandler != null)
			{
				return mFileHandler.GetBuffer();
			}
			return null;
		}

		public object GetObject()
		{
			if (mFileHandler != null)
			{
				return mFileHandler.GetObject();
			}
			return null;
		}

		public bool IsEndOfFile()
		{
			return true;
		}
	}
}
