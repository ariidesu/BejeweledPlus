using System;

namespace SexyFramework.Graphics
{
	public class SortedKern : IComparable
	{
		public char mKey;

		public char mValue;

		public int mOffset;

		public SortedKern()
		{
			mKey = '0';
			mValue = '0';
			mOffset = 0;
		}

		public SortedKern(char inKey, char inValue, int inOffset)
		{
			mKey = inKey;
			mValue = inValue;
			mOffset = inOffset;
		}

		public int CompareTo(object obj)
		{
			SortedKern sortedKern = obj as SortedKern;
			if (mKey < sortedKern.mKey)
			{
				return -1;
			}
			if (mKey > sortedKern.mKey)
			{
				return 1;
			}
			if (mValue < sortedKern.mValue)
			{
				return -1;
			}
			if (mValue > sortedKern.mValue)
			{
				return 1;
			}
			return 0;
		}

		private static int Compare(SortedKern a, SortedKern b)
		{
			if (a.mKey < b.mKey)
			{
				return -1;
			}
			if (a.mKey > b.mKey)
			{
				return 1;
			}
			if (a.mValue < b.mValue)
			{
				return -1;
			}
			if (a.mValue > b.mValue)
			{
				return 1;
			}
			return 0;
		}
	}
}
