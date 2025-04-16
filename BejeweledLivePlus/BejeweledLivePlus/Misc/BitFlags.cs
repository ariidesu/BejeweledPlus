namespace BejeweledLivePlus.Misc
{
	public class BitFlags
	{
		public uint mFlags;

		public BitFlags()
		{
			Clear();
		}

		public void Clear()
		{
			mFlags = 0u;
		}

		public void EnableAll()
		{
			mFlags = uint.MaxValue;
		}

		public bool IsBitSet(int theBit)
		{
			return (mFlags & (uint)(1 << theBit)) != 0;
		}

		public void SetBit(int theBit)
		{
			mFlags |= (uint)(1 << theBit);
		}

		public void ClearBit(int theBit)
		{
			mFlags &= (uint)(~(1 << theBit));
		}
	}
}
