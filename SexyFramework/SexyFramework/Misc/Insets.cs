namespace SexyFramework.Misc
{
	public class Insets
	{
		public int mLeft;

		public int mTop;

		public int mRight;

		public int mBottom;

		public Insets()
		{
		}

		public Insets(int theLeft, int theTop, int theRight, int theBottom)
		{
			mLeft = theLeft;
			mTop = theTop;
			mRight = theRight;
			mBottom = theBottom;
		}

		public Insets(Insets rhs)
		{
			mLeft = rhs.mLeft;
			mTop = rhs.mTop;
			mRight = rhs.mRight;
			mBottom = rhs.mBottom;
		}
	}
}
