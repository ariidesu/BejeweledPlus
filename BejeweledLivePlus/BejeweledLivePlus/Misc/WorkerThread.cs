using System.Threading;

namespace BejeweledLivePlus.Misc
{
	public class WorkerThread
	{
		public delegate void ThreadTask(object obj);

		protected ThreadTask mTask;

		protected object mTaskArg;

		protected bool mStopped;

		private ManualResetEvent mSignalEvent = new ManualResetEvent(true);

		private ManualResetEvent mDoneEvent = new ManualResetEvent(true);

		private void ThreadProc()
		{
			while (true)
			{
				mSignalEvent.WaitOne(1000);
				if (mStopped)
				{
					break;
				}
				if (mTask != null)
				{
					mTask(mTaskArg);
					mTask = null;
					mDoneEvent.Set();
				}
			}
			mDoneEvent.Set();
		}

		public WorkerThread()
		{
			mTask = null;
			mTaskArg = null;
			mStopped = false;
			Thread thread = new Thread(ThreadProc);
			thread.Start();
		}

		public virtual void Dispose()
		{
			WaitForTask();
			mStopped = true;
			mSignalEvent.Set();
			mDoneEvent.WaitOne(5000);
		}

		public void DoTask(ThreadTask theTask, object theTaskArg)
		{
			WaitForTask();
			mTask = theTask;
			mTaskArg = theTaskArg;
			mSignalEvent.Set();
		}

		public void WaitForTask()
		{
			while (mTask != null)
			{
				if (GlobalMembers.gApp.mResStreamsManager != null && GlobalMembers.gApp.mResStreamsManager.IsInitialized())
				{
					GlobalMembers.gApp.mResStreamsManager.Update();
				}
				mDoneEvent.WaitOne(10);
			}
			mDoneEvent.Reset();
		}

		public bool IsProcessingTask()
		{
			return mTask != null;
		}
	}
}
