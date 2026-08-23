using System;
using System.Collections.Generic;

namespace BejeweledLivePlus
{
	public sealed class OnlineHighScoreTable : HighScoreTable
	{
		private const int ONLINE_READ_TIMEOUT_UPDATES = 300;

		private HighScoreMgr mHighScoreMgr;

		private LocalHighScoreTable mLocalTable;

		private List<HighScoreEntryLive> mOnlineHighScores = new List<HighScoreEntryLive>();

		private object mLeaderboardReadLock = new object();

		private int mOnlineReadUpdates;

		private LRState mOnlineState;

		private bool mUsingLocalData;

		public override bool SupportsTimeViews
		{
			get
			{
				return CanReadOnline() && !mUsingLocalData;
			}
		}

		public override bool IsUsingLocalData
		{
			get
			{
				return mUsingLocalData;
			}
		}

		internal OnlineHighScoreTable(string modeKey, HighScoreMgr manager, LocalHighScoreTable localTable)
			: base(modeKey)
		{
			mHighScoreMgr = manager;
			mLocalTable = localTable;
			mOnlineState = LRState.LR_Idle;
			mUsingLocalData = !CanReadOnline();
			mHighScoresLive = mLocalTable.mHighScoresLive;
		}

		internal LocalHighScoreTable GetLocalTable()
		{
			return mLocalTable;
		}

		private bool CanReadOnline()
		{
			return mHighScoreMgr.IsOnlineAvailable();
		}

		public override bool Submit(string theName, int theValue, int thePicture)
		{
			bool submittedLocally = mLocalTable.Submit(theName, theValue, thePicture);
			mHighScoresLive = mLocalTable.mHighScoresLive;
			if (CanReadOnline())
			{
				SubmitHighScoreToXBLA(theValue);
			}
			return submittedLocally;
		}

		public override void ReadLeaderboard(HighScoreTableTime t)
		{
			mOnlineReadUpdates = 0;
			if (!CanReadOnline())
			{
				UseLocalData();
				return;
			}

			mUsingLocalData = false;
			mLRState = LRState.LR_Loading;
			mOnlineState = LRState.LR_Loading;
			GlobalMembers.isLeaderboardLoading = true;
			BeginOnlineRead(t);
		}

		public override void UpdateReadState()
		{
			if (mUsingLocalData)
			{
				return;
			}

			if (!CanReadOnline())
			{
				UseLocalData();
				return;
			}

			switch (mOnlineState)
			{
			case LRState.LR_Ready:
				mHighScoresLive = mOnlineHighScores;
				mLRState = LRState.LR_Ready;
				GlobalMembers.isLeaderboardLoading = false;
				break;
			case LRState.LR_Error:
				mHighScoreMgr.DisableOnline();
				UseLocalData();
				break;
			case LRState.LR_Loading:
				if (++mOnlineReadUpdates >= ONLINE_READ_TIMEOUT_UPDATES)
				{
					mHighScoreMgr.DisableOnline();
					UseLocalData();
				}
				break;
			}
		}

		private void SubmitHighScoreToXBLA(int theScore)
		{
			try
			{
				// SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
				// LeaderboardIdentity leaderboardId = LeaderboardIdentity.Create(LeaderboardKey.BestScoreRecent, mMode);
				// LeaderboardEntry leaderboard = signedInGamer.LeaderboardWriter.GetLeaderboard(leaderboardId);
				// leaderboard.Rating = theScore;
				// leaderboard.Columns.SetValue("TimeStamp", DateTime.Now);
				// leaderboard.Columns.SetValue("BestScore", theScore);
			}
			catch (Exception)
			{
			}
		}

		private void BeginOnlineRead(HighScoreTableTime t)
		{
			try
			{
				// SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
				// LeaderboardKey key = t == HighScoreTableTime.TIME_RECENT ? LeaderboardKey.BestScoreRecent : LeaderboardKey.BestScoreLifeTime;
				// LeaderboardIdentity leaderboardId = LeaderboardIdentity.Create(key, mMode);
				// LeaderboardReader.BeginRead(leaderboardId, signedInGamer, 10, LeaderboardReadCallback, signedInGamer);
			}
			catch (Exception)
			{
				mOnlineState = LRState.LR_Error;
				GlobalMembers.isLeaderboardLoading = false;
			}
		}

		private void LeaderboardReadCallback(IAsyncResult result)
		{
			lock (mLeaderboardReadLock)
			{
				// SignedInGamer signedInGamer = result.AsyncState as SignedInGamer;
				// if (signedInGamer != null)
				// {
				// 	try
				// 	{
				// 		leaderboardReader = LeaderboardReader.EndRead(result);
				// 		CanPageUp = leaderboardReader.CanPageUp;
				// 		CanPageDown = leaderboardReader.CanPageDown;
				// 		CreateRankList();
				// 		mOnlineState = LRState.LR_Ready;
				// 	}
				// 	catch (Exception)
				// 	{
				// 		mOnlineState = LRState.LR_Error;
				// 	}
				// }
				// else
				// {
					mOnlineState = LRState.LR_Error;
				// }
				GlobalMembers.isLeaderboardLoading = false;
			}
		}

		private void CreateRankList()
		{
			// mOnlineHighScores.Clear();
			// int count = leaderboardReader.Entries.Count;
			// for (int i = 0; i < count; i++)
			// {
			// 	LeaderboardEntry liveEntry = leaderboardReader.Entries[i];
			// 	HighScoreEntryLive highScoreEntryLive = new HighScoreEntryLive();
			// 	highScoreEntryLive.Init(liveEntry);
			// 	mOnlineHighScores.Add(highScoreEntryLive);
			// }
		}

		private void UseLocalData()
		{
			mUsingLocalData = true;
			mLocalTable.ReadLeaderboard(HighScoreTableTime.TIME_ALLTIME);
			mHighScoresLive = mLocalTable.mHighScoresLive;
			CanPageUp = false;
			CanPageDown = false;
			mLRState = LRState.LR_Ready;
			GlobalMembers.isLeaderboardLoading = false;
		}
	}
}
