using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public class GameChunkHeader
	{
		public int mMagic;

		public int mHeaderSize;

		public int mId;

		public int mSize;

		public int mOffset;

		public GameChunkHeader()
		{
			mMagic = 0;
			mHeaderSize = size();
			mId = 0;
			mSize = 0;
			mOffset = 0;
		}

		public void CopyFrom(GameChunkHeader data)
		{
			mMagic = data.mMagic;
			mHeaderSize = data.mHeaderSize;
			mId = data.mId;
			mSize = data.mSize;
			mOffset = data.mOffset;
		}

		public static int size()
		{
			return 20;
		}

		public void write(Buffer buffer)
		{
			buffer.WriteInt32(mMagic);
			buffer.WriteInt32(mHeaderSize);
			buffer.WriteInt32(mId);
			buffer.WriteInt32(mSize);
			buffer.WriteInt32(mOffset);
		}

		public void read(Buffer buffer)
		{
			mMagic = buffer.ReadInt32();
			mHeaderSize = buffer.ReadInt32();
			mId = buffer.ReadInt32();
			mSize = buffer.ReadInt32();
			mOffset = buffer.ReadInt32();
		}

		public void zero()
		{
			mMagic = 0;
			mHeaderSize = 0;
			mId = 0;
			mSize = 0;
			mOffset = 0;
		}
	}
}
