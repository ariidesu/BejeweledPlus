using System.Collections.Generic;

namespace BejeweledLivePlus
{
	public abstract class HighScoreTable
	{
		public enum HighScoreTableTime
		{
			TIME_RECENT,
			TIME_ALLTIME
		}

		public enum LRState
		{
			LR_Idle,
			LR_Loading,
			LR_Ready,
			LR_Error
		}

		public List<HighScoreEntryLive> mHighScoresLive = new List<HighScoreEntryLive>();

		private string mModeKey = string.Empty;

		public HighScoreMgr mManager;

		public bool CanPageUp;

		public bool CanPageDown;

		public LRState mLRState;

		public int mMode { get; set; }

		public abstract bool SupportsTimeViews { get; }

		public abstract bool IsUsingLocalData { get; }

		protected HighScoreTable(string modeKey)
		{
			mHighScoresLive = new List<HighScoreEntryLive>();
			mModeKey = modeKey;
			mMode = GlobalMembers.gApp != null ? GlobalMembers.gApp.ModeHeadingToGameMode(mModeKey) : (int)GameMode.MODE_MAX;
		}

		public abstract bool Submit(string theName, int theValue, int thePicture);

		public abstract void ReadLeaderboard(HighScoreTableTime t);

		public virtual void UpdateReadState()
		{
		}
	}
}
