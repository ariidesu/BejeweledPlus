using System;
using System.IO;
using SexyFramework.Drivers.Leaderboard;

// using Microsoft.Xna.Framework.GamerServices;

namespace BejeweledLivePlus
{
	public class HighScoreEntryLive
	{
		// public GamerProfile mProfile;

		public int mRank;

		public int mScore;

		public string mName;

		public int mTimestamp;

		public bool mIsNew;

		public int mPicture;

		public Stream mPicStream;

		private object mGetProfileLock = new object();

		public HighScoreEntryLive()
		{
			mRank = -1;
			mScore = -1;
			mName = string.Empty;
			mTimestamp = 0;
			mIsNew = false;
			mPicture = 0;
			mPicStream = null;
		}

		public HighScoreEntryLive(string name, int score, int timestamp, bool isNew, int picture = 0)
			: this()
		{
			mName = name;
			mScore = score;
			mTimestamp = timestamp;
			mIsNew = isNew;
			mPicture = picture;
		}

		public void Init(LeaderboardEntry liveEntry)
		{
			try
			{
				// mName = liveEntry.Gamer.Gamertag;
				// mScore = liveEntry.Columns.GetValueInt32("BestScore");
			}
			catch (Exception)
			{
			}
		}

		private void GetProfileCallback(IAsyncResult result)
		{
		}
	}
}
