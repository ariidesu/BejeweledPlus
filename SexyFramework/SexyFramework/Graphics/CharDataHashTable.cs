using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class CharDataHashTable
	{
		public enum ECharDataHash
		{
			HASH_BITS = 10,
			HASH_BUCKET_COUNT = 1024,
			HASH_BUCKET_MASK = 1023
		}

		public bool mOrderedHash;

		public List<CharData> mCharData = new List<CharData>();

		public List<CharDataHashEntry> mHashEntries = new List<CharDataHashEntry>(1024);

		protected int GetBucketIndex(char inChar)
		{
			if (mOrderedHash)
			{
				return inChar & 0x3FF;
			}
			uint num = 3203386110u;
			num ^= inChar;
			num *= 1540483477;
			num ^= num >> 13;
			num *= 1540483477;
			num ^= num >> 15;
			return (int)(num & 0x3FF);
		}

		public CharDataHashTable()
		{
			mOrderedHash = false;
			for (int i = 0; i < 1024; i++)
			{
				mHashEntries.Add(new CharDataHashEntry());
			}
		}

		public CharData GetCharData(char inChar, bool inAllowAdd)
		{
			int num = GetBucketIndex(inChar);
			CharDataHashEntry charDataHashEntry = mHashEntries[num];
			if (charDataHashEntry.mChar == inChar && charDataHashEntry.mDataIndex != ushort.MaxValue)
			{
				return mCharData[charDataHashEntry.mDataIndex];
			}
			if (charDataHashEntry.mChar == 0)
			{
				if (!inAllowAdd)
				{
					return null;
				}
				charDataHashEntry.mChar = inChar;
				charDataHashEntry.mDataIndex = (ushort)mCharData.Count;
				mCharData.Add(new CharData());
				CharData charData = mCharData[charDataHashEntry.mDataIndex];
				charData.mHashEntryIndex = num;
				return charData;
			}
			while (charDataHashEntry.mChar != inChar)
			{
				if (charDataHashEntry.mNext == uint.MaxValue)
				{
					if (!inAllowAdd)
					{
						return null;
					}
					charDataHashEntry.mNext = (uint)mHashEntries.Count;
					mHashEntries.Add(new CharDataHashEntry());
					charDataHashEntry = mHashEntries[num];
					CharDataHashEntry charDataHashEntry2 = mHashEntries[(int)charDataHashEntry.mNext];
					charDataHashEntry2.mChar = inChar;
					charDataHashEntry2.mDataIndex = (ushort)mCharData.Count;
					mCharData.Add(new CharData());
					CharData charData2 = mCharData[charDataHashEntry2.mDataIndex];
					charData2.mHashEntryIndex = (int)charDataHashEntry.mNext;
					return charData2;
				}
				num = (int)charDataHashEntry.mNext;
				charDataHashEntry = mHashEntries[num];
			}
			return mCharData[charDataHashEntry.mDataIndex];
		}
	}
}
