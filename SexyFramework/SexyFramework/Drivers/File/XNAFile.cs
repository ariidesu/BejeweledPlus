using System;
using SexyFramework.Drivers.App;

namespace SexyFramework.Drivers.File
{
	public class XNAFile : IFile
	{
		protected string mFileName;

		protected bool mIsLoaded;

		protected byte[] mData;

		protected Status mStatus;

		protected object mDataObject;

		protected XNAFileDriver mFileDriver;

		public XNAFile(string name, XNAFileDriver driver)
		{
			mFileName = name;
			mIsLoaded = false;
			mData = null;
			mStatus = Status.READ_PENDING;
			mFileDriver = driver;
			mDataObject = null;
		}

		public override void Dispose()
		{
			mData = null;
		}

		public override bool IsLoaded()
		{
			return mIsLoaded;
		}

		public override bool HasError()
		{
			return mStatus == Status.READ_ERROR;
		}

		public override void AsyncLoad()
		{
		}

		public override bool ForceLoad()
		{
			try
			{
				mData = mFileDriver.GetContentManager().Load<byte[]>(mFileName);
				mStatus = Status.READ_COMPLETE;
				return true;
			}
			catch (Exception)
			{
				mStatus = Status.READ_ERROR;
				return false;
			}
		}

		public override byte[] GetBuffer()
		{
			return mData;
		}

		public override bool ForceLoadObject<T>()
		{
			try
			{
				mDataObject = ((WP7ContentManager)mFileDriver.GetContentManager()).LoadResDirectly<T>(mFileName);
				mStatus = Status.READ_COMPLETE;
				return true;
			}
			catch (Exception)
			{
				mStatus = Status.READ_ERROR;
				return false;
			}
		}

		public override object GetObject()
		{
			return mDataObject;
		}

		public override uint GetSize()
		{
			return (uint)mData.Length;
		}

		public override void Close()
		{
		}

		public override void DirectSeek(long theSeekPoint)
		{
		}

		public override bool DirectRead(byte theBuffer, long theReadSize)
		{
			return false;
		}

		public override Status DirectReadStatus()
		{
			return mStatus;
		}

		public override long DirectReadBlockSize()
		{
			return 0L;
		}
	}
}
