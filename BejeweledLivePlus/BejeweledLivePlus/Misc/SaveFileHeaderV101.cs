using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public class SaveFileHeaderV101
	{
		public int mMagic;

		public int mGameVersion;

		public int mBoardVersion;

		public int mPlatform;

		public SaveFileHeaderV101()
		{
			mMagic = 0;
			mGameVersion = 0;
			mBoardVersion = 0;
			mPlatform = 0;
		}

		public void Copyfrom(SaveFileHeaderV101 data)
		{
			mMagic = data.mMagic;
			mGameVersion = data.mGameVersion;
			mBoardVersion = data.mBoardVersion;
			mPlatform = data.mPlatform;
		}

		public static int size()
		{
			return 16;
		}

		public void write(Buffer buffer)
		{
			buffer.WriteInt32(mMagic);
			buffer.WriteInt32(mGameVersion);
			buffer.WriteInt32(mBoardVersion);
			buffer.WriteInt32(mPlatform);
		}

		public void read(Buffer buffer)
		{
			mMagic = buffer.ReadInt32();
			mGameVersion = buffer.ReadInt32();
			mBoardVersion = buffer.ReadInt32();
			mPlatform = buffer.ReadInt32();
		}
	}
}
