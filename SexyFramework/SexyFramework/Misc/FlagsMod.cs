namespace SexyFramework.Misc
{
	public class FlagsMod
	{
		public int mRemoveFlags;

		public int mAddFlags;

		public static int GetModFlags(int theFlags, FlagsMod theFlagMod)
		{
			return (theFlags | theFlagMod.mAddFlags) & ~theFlagMod.mRemoveFlags;
		}

		public static void ModFlags(ref int theFlags, FlagsMod theFlagMod)
		{
			theFlags = (theFlags | theFlagMod.mAddFlags) & ~theFlagMod.mRemoveFlags;
		}
	}
}
