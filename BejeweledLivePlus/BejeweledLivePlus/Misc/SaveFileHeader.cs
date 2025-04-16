using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public class SaveFileHeader
	{
		public enum ReadContent
		{
			Self,
			SelfAndOldHeader
		}

		public SaveFileHeaderV101 mOldHeader = new SaveFileHeaderV101();

		public int mSize;

		public int mGameType;

		public int mGameChunkCount;

		public SaveFileHeader()
		{
			mSize = size() - SaveFileHeaderV101.size();
			mGameType = 0;
			mGameChunkCount = 0;
		}

		public void Copyfrom(SaveFileHeader data)
		{
			mOldHeader.Copyfrom(data.mOldHeader);
			mSize = data.mSize;
			mGameType = data.mGameType;
			mGameChunkCount = data.mGameChunkCount;
		}

		public static int size()
		{
			return SaveFileHeaderV101.size() + 12;
		}

		public int sizeofOldHeader()
		{
			return SaveFileHeaderV101.size();
		}

		public void write(Buffer buffer)
		{
			mOldHeader.write(buffer);
			buffer.WriteInt32(mSize);
			buffer.WriteInt32(mGameType);
			buffer.WriteInt32(mGameChunkCount);
		}

		public void read(Buffer buffer, ReadContent content)
		{
			if (content == ReadContent.SelfAndOldHeader)
			{
				mOldHeader.read(buffer);
			}
			mSize = buffer.ReadInt32();
			mGameType = buffer.ReadInt32();
			mGameChunkCount = buffer.ReadInt32();
		}
	}
}
