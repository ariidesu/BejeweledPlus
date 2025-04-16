namespace SexyFramework.PIL
{
	public class KeyFrame
	{
		public int first;

		public KeyFrameData second;

		public KeyFrame()
		{
		}

		public KeyFrame(int k, KeyFrameData data)
		{
			first = k;
			second = data;
		}
	}
}
