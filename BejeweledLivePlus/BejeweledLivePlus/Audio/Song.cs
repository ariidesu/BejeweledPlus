namespace BejeweledLivePlus.Audio
{
	internal struct Song
	{
		public int mID;

		public bool mLoop;

		public float mFadeSpeed;

		public static Song DefaultSong = new Song(-1, false, 1f);

		public Song(int inID, bool inLoop, float inFadeSpeed)
		{
			mID = inID;
			mLoop = inLoop;
			mFadeSpeed = inFadeSpeed;
		}
	}
}
