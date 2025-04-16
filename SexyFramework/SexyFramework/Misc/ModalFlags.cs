namespace SexyFramework.Misc
{
	public class ModalFlags
	{
		public int mOverFlags;

		public int mUnderFlags;

		public bool mIsOver;

		public void ModFlags(FlagsMod theFlagsMod)
		{
			FlagsMod.ModFlags(ref mOverFlags, theFlagsMod);
			FlagsMod.ModFlags(ref mOverFlags, theFlagsMod);
		}

		public int GetFlags()
		{
			if (!mIsOver)
			{
				return mUnderFlags;
			}
			return mOverFlags;
		}
	}
}
