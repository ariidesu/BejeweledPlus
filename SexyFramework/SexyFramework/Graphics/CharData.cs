using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class CharData
	{
		public Rect mImageRect = default(Rect);

		public Point mOffset = default(Point);

		public ushort mKerningFirst;

		public ushort mKerningCount;

		public int mWidth;

		public int mOrder;

		public int mHashEntryIndex;

		public CharData()
		{
			mKerningFirst = 0;
			mKerningCount = 0;
			mWidth = 0;
			mOrder = 0;
		}
	}
}
