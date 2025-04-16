namespace SexyFramework.Graphics
{
	public class CharDataHashEntry
	{
		public ushort mChar;

		public ushort mDataIndex;

		public uint mNext;

		public CharDataHashEntry()
		{
			mChar = 0;
			mDataIndex = ushort.MaxValue;
			mNext = uint.MaxValue;
		}
	}
}
