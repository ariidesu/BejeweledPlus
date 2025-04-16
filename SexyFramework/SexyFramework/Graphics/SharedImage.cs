namespace SexyFramework.Graphics
{
	public class SharedImage
	{
		public DeviceImage mImage;

		public int mRefCount;

		public bool mLoading;

		public new string ToString()
		{
			if ("RefCount(" + mRefCount + "):" + mImage == null)
			{
				return "NULL";
			}
			return mImage.ToString();
		}
	}
}
