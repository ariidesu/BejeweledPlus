namespace SexyFramework.Resource
{
	public class CharStack
	{
		private char[] mCharStack = new char[2097152];

		private int mCount;

		private int mStart;

		public int Count => mCount;

		public char Peek()
		{
			return mCharStack[mStart];
		}

		public void Pop()
		{
			mStart--;
			mCount--;
		}

		public void Push(char c)
		{
			mStart++;
			mCharStack[mStart] = c;
			mCount++;
		}

		public void Clear()
		{
			mCount = 0;
			mStart = 0;
			for (int i = 0; i < 2097152; i++)
			{
				mCharStack[i] = '0';
			}
		}
	}
}
