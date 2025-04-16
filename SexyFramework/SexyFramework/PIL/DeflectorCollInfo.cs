namespace SexyFramework.PIL
{
	public class DeflectorCollInfo
	{
		public int mLastCollFrame;

		public bool mIgnoresDeflector;

		public DeflectorCollInfo()
		{
		}

		public DeflectorCollInfo(int f, bool b)
		{
			mLastCollFrame = f;
			mIgnoresDeflector = b;
		}
	}
}
