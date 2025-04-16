namespace SexyFramework.Misc
{
	public class PerfTimer
	{
		protected ulong mStart;

		protected double mDuration;

		protected bool mRunning;

		protected void CalcDuration()
		{
			mDuration = Common.SexyTime() - mStart;
		}

		public PerfTimer()
		{
			mRunning = false;
		}

		public void Start()
		{
			mStart = Common.SexyTime();
			mRunning = true;
		}

		public void Stop()
		{
			CalcDuration();
			mRunning = false;
		}

		public double GetDuration()
		{
			if (mRunning)
			{
				CalcDuration();
			}
			return mDuration;
		}

		public bool IsRunning()
		{
			return mRunning;
		}
	}
}
