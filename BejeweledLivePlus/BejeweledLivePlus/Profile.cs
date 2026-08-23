using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.GamerServices;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class Profile : IDisposable
	{
		public enum RecentBadgeNum
		{
			MAX_NUMBER_OF_RECENT_BADGES = 3
		}

		public const int MAX_CHARS = 32;

		private List<int> pointRanges = new List<int>();

		private Image mProfilePicture;

		private int mImageNumber;

		private uint mProfileId;

		private uint mLastProfileId;

		public List<string> mProfileList = new List<string>();

		public DateTime mLastServerGMTTime = DateTime.Now;

		public DateTime mLastServerTimeDelta = DateTime.Now;

		public DateTime mLastTickOriginTime = DateTime.Now;

		public DateTime mLastTickOriginCalcTime = DateTime.Now;

		public ulong mGamesPlayedToday;

		public string mLastFacebookId = string.Empty;

		public string mProfileName = string.Empty;

		public int mFlags;

		public bool mNeedMoveProfileFiles;

		public string mFacebookName = string.Empty;

		public string mFacebookPassword = string.Empty;

		public bool mFacebookAutoLogin;

		public bool mIsNew;

		public StatsDoubleBuffer mStats = new StatsDoubleBuffer();

		public int[] mStatsToday = new int[40];

		public int mStatsTodayDay;

		public int mStatsTodayYear;

		public bool[,] mQuestsCompleted = new bool[BejeweledLivePlusAppConstants.NUM_QUEST_SETS, BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET + BejeweledLivePlusAppConstants.QUESTS_OPTIONAL_PER_SET];

		public bool[] mEndlessModeUnlocked = new bool[4];

		public int mLastQuestPage;

		public bool mLastQuestBlink;

		public int[] mGameStats = new int[5];

		public bool[] mBadgeStatus = new bool[20];

		public int[] mHighScores = new int[7];

		public int[] mHighScoresToday = new int[7];

		public int[] mRecentBadges = new int[3];

		public int mOfflineRank;

		public long mOfflineRankPoints;

		public int mOnlineRank;

		public long mOnlineRankPoints;

		public int mOfflineGames;

		public int mOnlineGames;

		public bool mCustomCursors;

		public ulong mTutorialFlags;

		public int mTipIdx;

		public int[] mOfflineBoostCounts = new int[5];

		public bool mBreathOn;

		public bool mBreathVisual;

		public float mBreathSpeed;

		public bool mNoiseOn;

		public string mNoiseFileName;

		public bool mBeatOn;

		public string mSBAFileName;

		public bool mAffirmationOn;

		public string mAffirmationFileName;

		public bool mAffirmationSubliminal;

		public float mAffirmationSpeed;

		public float mAffirmationSubliminality;

		public int mAmbientSelection;

		public int mMantraSelection;

		public bool[] mQuestHelpShown = new bool[BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET];

		public Dictionary<string, string> mLocalDataMap = new Dictionary<string, string>();

		public bool mAllowAnalytics;

		public bool mAutoHint;

		public RATE_GAME_CHOICE mRateGameChoice;

		public int mRateGameSeenAt;

		public ulong mTotalGamesPlayed;

		public List<int> mDeferredBadgeVector = new List<int>();

		public int[] mPreAwardedBadgeLevels = new int[20];

		public bool mHasSeenIntro;

		public bool mHasSeenGetBlitzDialog;

		public bool mHasSeenOpenBlitzDialog;

		public bool mMetricsPostAllAwardedBadges;

		public Last3MatchScoreManager mLast3MatchScoreManager = new Last3MatchScoreManager();

		private object mGetProfileLock = new object();

		public static bool IsEliteBadge(int badgeId)
		{
			return false;
		}

		protected void ClearProfile()
		{
			mProfileId = GetNewProfileId();
			mProfileName = "";
			mFacebookName = "";
			mFacebookPassword = "";
			mFacebookAutoLogin = false;
			mIsNew = true;
			mOfflineRank = 0;
			mOfflineRankPoints = 0L;
			mOnlineRank = 0;
			mOnlineRankPoints = 0L;
			mOfflineGames = 0;
			mOnlineGames = 0;
			mFlags = 0;
			mLastQuestPage = 0;
			mLastQuestBlink = false;
			ClearQuestHelpShown();
			mStatsTodayDay = (mStatsTodayYear = 0);
			mStats.ClearAll();
			for (int i = 0; i < 40; i++)
			{
				mStatsToday[i] = 0;
			}
			for (int j = 0; j < 5; j++)
			{
				mGameStats[j] = 0;
			}
			for (int k = 0; k < 20; k++)
			{
				mBadgeStatus[k] = false;
			}
			for (int l = 0; l < 4; l++)
			{
				mEndlessModeUnlocked[l] = false;
			}
			for (int m = 0; m < 5; m++)
			{
				mOfflineBoostCounts[m] = 0;
			}
			mCustomCursors = GlobalMembers.gApp.mCustomCursorsEnabled;
			mTutorialFlags = 0uL;
			mTipIdx = 0;
			mNeedMoveProfileFiles = false;
			for (int n = 0; n < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; n++)
			{
				for (int num = 0; num < BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET + BejeweledLivePlusAppConstants.QUESTS_OPTIONAL_PER_SET; num++)
				{
					mQuestsCompleted[n, num] = false;
				}
			}
			mLocalDataMap.Clear();
			mBreathOn = false;
			mBreathVisual = true;
			mBreathSpeed = 0.42f;
			mNoiseOn = false;
			mNoiseFileName = string.Empty;
			mBeatOn = false;
			mSBAFileName = string.Empty;
			mAffirmationOn = false;
			mAffirmationFileName = string.Empty;
			mAffirmationSubliminal = false;
			mAffirmationSpeed = 0.5f;
			mAffirmationSubliminality = 0.5f;
			for (int num2 = 0; num2 < 7; num2++)
			{
				mHighScores[num2] = 0;
				mHighScoresToday[num2] = 0;
			}
			for (int num3 = 0; num3 < 3; num3++)
			{
				mRecentBadges[num3] = -1;
			}
			mImageNumber = 0;
			mProfilePicture = null;
			mAutoHint = true;
			mAmbientSelection = (mMantraSelection = 0);
			mAllowAnalytics = true;
			mRateGameChoice = RATE_GAME_CHOICE.RATE_GAME_CHOICE_NOT_SEEN;
			mRateGameSeenAt = 0;
			mHasSeenIntro = false;
			mTotalGamesPlayed = 0uL;
			mDeferredBadgeVector.Clear();
			for (int num4 = 0; num4 < 20; num4++)
			{
				mPreAwardedBadgeLevels[num4] = 0;
			}
			mHasSeenGetBlitzDialog = false;
			mHasSeenOpenBlitzDialog = false;
			mMetricsPostAllAwardedBadges = true;
			mLast3MatchScoreManager.Clear();
		}

		protected bool LoadProfileHelper(string theProfileName)
		{
			string theFileName = GetProfileDir(theProfileName) + "\\profile.dat";
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			if (!SexyFramework.GlobalMembers.gSexyApp.ReadBufferFromStorage(theFileName, buffer))
			{
				return false;
			}
			if (buffer.ReadInt32() != 958131957)
			{
				return false;
			}
			int num = buffer.ReadInt32();
			if (num < 71)
			{
				return false;
			}
			mProfileName = theProfileName;
			mIsNew = false;
			mOfflineRank = buffer.ReadInt32();
			mStatsTodayDay = buffer.ReadInt32();
			mStatsTodayYear = buffer.ReadInt32();
			if (num > 71)
			{
				int num2 = buffer.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					mStats[i] = buffer.ReadInt32();
				}
				int num3 = buffer.ReadInt32();
				for (int j = 0; j < num3; j++)
				{
					mStatsToday[j] = buffer.ReadInt32();
				}
				int num4 = buffer.ReadInt32();
				for (int k = 0; k < num4; k++)
				{
					mGameStats[k] = buffer.ReadInt32();
				}
				int num5 = buffer.ReadInt32();
				for (int l = 0; l < num5; l++)
				{
					mBadgeStatus[l] = buffer.ReadBoolean();
				}
				int num6 = buffer.ReadInt32();
				for (int m = 0; m < num6; m++)
				{
					mEndlessModeUnlocked[m] = buffer.ReadBoolean();
				}
			}
			else
			{
				for (int n = 0; n < 38; n++)
				{
					mStats[n] = buffer.ReadInt32();
				}
				for (int num7 = 38; num7 < 40; num7++)
				{
					mStats[num7] = 0;
				}
				for (int num8 = 0; num8 < 38; num8++)
				{
					mStatsToday[num8] = buffer.ReadInt32();
				}
				for (int num9 = 38; num9 < 40; num9++)
				{
					mStatsToday[num9] = 0;
				}
				for (int num10 = 0; num10 < 5; num10++)
				{
					mGameStats[num10] = buffer.ReadInt32();
				}
				for (int num11 = 5; num11 < 5; num11++)
				{
					mGameStats[num11] = 0;
				}
				for (int num12 = 0; num12 < 20; num12++)
				{
					mBadgeStatus[num12] = buffer.ReadBoolean();
				}
				for (int num13 = 0; num13 < 4; num13++)
				{
					mEndlessModeUnlocked[num13] = buffer.ReadBoolean();
				}
				for (int num14 = 4; num14 < 4; num14++)
				{
					mEndlessModeUnlocked[num14] = false;
				}
			}
			mCustomCursors = buffer.ReadBoolean();
			mTutorialFlags = (ulong)buffer.ReadInt64();
			mTipIdx = buffer.ReadInt32();
			if (num >= 67)
			{
				mLastQuestPage = buffer.ReadInt32();
				mLastQuestBlink = buffer.ReadBoolean();
			}
			for (int num15 = 0; num15 < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; num15++)
			{
				for (int num16 = 0; num16 < BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET + BejeweledLivePlusAppConstants.QUESTS_OPTIONAL_PER_SET; num16++)
				{
					mQuestsCompleted[num15, num16] = buffer.ReadBoolean();
				}
			}
			mFacebookName = buffer.ReadString();
			mFacebookPassword = buffer.ReadString();
			mFacebookAutoLogin = buffer.ReadBoolean();
			mLocalDataMap.Clear();
			int num17 = buffer.ReadInt32();
			for (int num18 = 0; num18 < num17; num18++)
			{
				string key = buffer.ReadString();
				string value = buffer.ReadString();
				mLocalDataMap.Add(key, value);
			}
			for (int num19 = 0; num19 < 5; num19++)
			{
				mOfflineBoostCounts[num19] = buffer.ReadInt32();
			}
			mOnlineRank = buffer.ReadInt32();
			mOnlineRankPoints = buffer.ReadInt32();
			if (num <= 60)
			{
				mOnlineRankPoints = mStats[1];
			}
			else
			{
				mOfflineRankPoints = buffer.ReadInt32();
			}
			mOfflineGames = buffer.ReadInt32();
			mOnlineGames = buffer.ReadInt32();
			mFlags = buffer.ReadInt32();
			if (num >= 61)
			{
				mOfflineRankPoints |= buffer.ReadInt32();
				mOnlineRankPoints |= buffer.ReadInt32();
			}
			mOnlineRank = (int)GetRankAtPoints(mOnlineRankPoints);
			mOfflineRank = (int)GetRankAtPoints(mOfflineRankPoints);
			mBreathOn = buffer.ReadBoolean();
			mBreathVisual = buffer.ReadBoolean();
			mBreathSpeed = buffer.ReadFloat();
			mNoiseOn = buffer.ReadBoolean();
			mNoiseFileName = buffer.ReadString();
			mBeatOn = buffer.ReadBoolean();
			mSBAFileName = buffer.ReadString();
			mAffirmationOn = buffer.ReadBoolean();
			mAffirmationFileName = buffer.ReadString();
			mAffirmationSubliminal = buffer.ReadBoolean();
			mAffirmationSpeed = buffer.ReadFloat();
			mAffirmationSubliminality = buffer.ReadFloat();
			if (num >= 69)
			{
				int num20 = buffer.ReadInt32();
				bool flag = num20 == BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET;
				for (int num21 = 0; num21 < num20; num21++)
				{
					if (flag)
					{
						mQuestHelpShown[num21] = buffer.ReadBoolean();
					}
					else
					{
						buffer.ReadBoolean();
					}
				}
			}
			mAllowAnalytics = buffer.ReadBoolean();
			mAutoHint = buffer.ReadBoolean();
			mImageNumber = buffer.ReadInt32();
			if (num > 71)
			{
				int num22 = buffer.ReadInt32();
				for (int num23 = 0; num23 < num22; num23++)
				{
					mRecentBadges[num23] = buffer.ReadInt32();
				}
			}
			else
			{
				for (int num24 = 0; num24 < 3; num24++)
				{
					mRecentBadges[num24] = buffer.ReadInt32();
				}
				for (int num25 = 3; num25 < 3; num25++)
				{
					mRecentBadges[num25] = 0;
				}
			}
			mAmbientSelection = buffer.ReadInt32();
			mMantraSelection = buffer.ReadInt32();
			mRateGameChoice = (RATE_GAME_CHOICE)buffer.ReadInt32();
			mRateGameSeenAt = buffer.ReadInt32();
			if (num != 73)
			{
				mRateGameChoice = RATE_GAME_CHOICE.RATE_GAME_CHOICE_NOT_SEEN;
				mRateGameSeenAt = mStats[0];
			}
			mHasSeenIntro = buffer.ReadBoolean();
			mTotalGamesPlayed = (ulong)buffer.ReadInt64();
			mProfileId = 1u;
			if (num > 71)
			{
				mDeferredBadgeVector.Clear();
				int num26 = buffer.ReadInt32();
				for (int num27 = 0; num27 < num26; num27++)
				{
					mDeferredBadgeVector.Add(buffer.ReadInt32());
				}
				mProfileId = (uint)buffer.ReadInt32();
			}
			if (num > 72)
			{
				mHasSeenGetBlitzDialog = buffer.ReadBoolean();
				mHasSeenOpenBlitzDialog = buffer.ReadBoolean();
				mMetricsPostAllAwardedBadges = buffer.ReadBoolean();
				mLast3MatchScoreManager.Load(buffer);
			}
			else
			{
				mHasSeenGetBlitzDialog = false;
				mHasSeenOpenBlitzDialog = false;
				mMetricsPostAllAwardedBadges = true;
				mLast3MatchScoreManager.Clear();
			}
			ListAddName(theProfileName);
			return true;
		}

		protected uint GetNewProfileId()
		{
			return mLastProfileId++;
		}

		public bool ListAddName(string theName)
		{
			for (int i = 0; i < mProfileList.Count; i++)
			{
				if (string.Compare(mProfileList[i], theName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return false;
				}
			}
			mProfileList.Add(theName);
			return true;
		}

		public bool ListRemoveName(string theName)
		{
			bool result = false;
			for (int i = 0; i < mProfileList.Count; i++)
			{
				if (string.Compare(mProfileList[i], theName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					mProfileList.RemoveAt(i);
					result = true;
					break;
				}
			}
			return result;
		}

		public void MoveProfileFilesHelper(string theFrom, string theTo)
		{
		}

		public void ClearQuestHelpShown()
		{
			for (int i = 0; i < mQuestHelpShown.Length; i++)
			{
				mQuestHelpShown[i] = false;
			}
		}

		public bool WantQuestHelp(int theQuestId)
		{
			if (theQuestId < BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET && theQuestId >= 0)
			{
				return !mQuestHelpShown[theQuestId];
			}
			return true;
		}

		public void SetQuestHelpShown(int theQuestId)
		{
			SetQuestHelpShown(theQuestId, true);
		}

		public void SetQuestHelpShown(int theQuestId, bool theVal)
		{
			if (theQuestId < BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET && theQuestId >= 0)
			{
				mQuestHelpShown[theQuestId] = theVal;
			}
		}

		public Profile()
		{
			mLastProfileId = 0u;
			pointRanges.Clear();
			int num = 0;
			for (int i = 0; i <= Common.size(GlobalMembers.gApp.mRankNames); i++)
			{
				pointRanges.Add(num + 250 * i);
				num += 250 * i;
			}
			ClearProfile();
			mLastServerGMTTime = DateTime.Now;
			mLastServerTimeDelta = DateTime.Now;
			mGamesPlayedToday = 0uL;
			mLastTickOriginTime = DateTime.Now;
			mLastTickOriginCalcTime = DateTime.Now;
		}

		public void Dispose()
		{
			if (mNeedMoveProfileFiles)
			{
				MoveProfileFiles();
			}
			WriteProfile();
			WriteProfileList();
		}

		public static string GetTotalTimePlayed()
		{
			int aTime = GlobalMembers.gApp.mProfile.mStats[0];
			return Utils.GetTimeString(aTime);
		}

		public static string GetTotalTimePlayedToday()
		{
			int val = GlobalMembers.gApp.mProfile.mStats[0] - GlobalMembers.gApp.mProfile.mStatsToday[0];
			return Utils.GetTimeString(Math.Max(0, val));
		}

		public void ReadProfileList(ref string theLastPlayer)
		{
			bool flag = false;
			mProfileList.Clear();
			string empty = string.Empty;
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			string theFileName = BejeweledLivePlus.Misc.Common.GetAppDataFolder() + "users\\users.dat";
			if (SexyFramework.GlobalMembers.gSexyApp.ReadBufferFromStorage(theFileName, buffer))
			{
				int num = buffer.ReadInt32();
				int num2 = buffer.ReadInt32();
				if (num2 < 8 || num2 > 8 || num != 958131957)
				{
					WriteProfileList();
					return;
				}
				theLastPlayer = buffer.ReadString();
				int val = buffer.ReadInt32();
				mLastProfileId = Math.Max(mLastProfileId, (uint)val);
				if (num2 >= 6)
				{
					mLastFacebookId = buffer.ReadString();
					mLastServerGMTTime = DateTime.FromFileTime(buffer.ReadInt64());
					mLastServerTimeDelta = DateTime.FromFileTime(buffer.ReadInt64());
					mGamesPlayedToday = (ulong)buffer.ReadInt64();
				}
				if (num2 >= 7)
				{
					mLastTickOriginTime = DateTime.FromFileTime(buffer.ReadInt64());
					mLastTickOriginCalcTime = DateTime.FromFileTime(buffer.ReadInt64());
				}
				while (true)
				{
					empty = buffer.ReadString();
					if (!(empty != string.Empty))
					{
						break;
					}
					if (!LoadProfileHelper(empty))
					{
						flag = true;
						DeleteProfile(empty);
					}
					else
					{
						ListAddName(empty);
					}
				}
			}
			if (flag)
			{
				WriteProfileList();
			}
		}

		public void WriteProfileList()
		{
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			buffer.WriteInt32(958131957);
			buffer.WriteInt32(8);
			buffer.WriteString(mProfileName);
			GlobalMembers.gApp.RegistryWriteString("LastUser", mProfileName);
			buffer.WriteInt32((int)mLastProfileId);
			buffer.WriteString(mLastFacebookId);
			buffer.WriteInt64(mLastServerGMTTime.ToFileTime());
			buffer.WriteInt64(mLastServerTimeDelta.ToFileTime());
			buffer.WriteInt64((long)mGamesPlayedToday);
			buffer.WriteInt64(mLastTickOriginTime.ToFileTime());
			buffer.WriteInt64(mLastTickOriginCalcTime.ToFileTime());
			for (int i = 0; i < mProfileList.Count; i++)
			{
				buffer.WriteString(mProfileList[i]);
			}
			string theFileName = BejeweledLivePlus.Misc.Common.GetAppDataFolder() + "users\\users.dat";
			GlobalMembers.gApp.WriteBufferToFile(theFileName, buffer);
		}

		public List<string> GetProfileList()
		{
			return mProfileList;
		}

		public bool LoadProfile(string theProfileName)
		{
			return LoadProfile(theProfileName, true);
		}

		public bool LoadProfile(string theProfileName, bool saveCurrent)
		{
			if (mNeedMoveProfileFiles)
			{
				MoveProfileFiles();
			}
			if (saveCurrent)
			{
				WriteProfile();
			}
			if (!LoadProfileHelper(theProfileName))
			{
				ListRemoveName(theProfileName);
				return false;
			}
			return true;
		}

		public bool CreateProfile(string theProfileName)
		{
			return CreateProfile(theProfileName, true);
		}

		public bool CreateProfile(string theProfileName, bool clearProfile)
		{
			if (theProfileName != string.Empty && !ListAddName(theProfileName))
			{
				string text = theProfileName;
				for (int i = 0; i < mProfileList.Count; i++)
				{
					if (string.Compare(mProfileList[i], text, StringComparison.OrdinalIgnoreCase) == 0)
					{
						text = mProfileList[i];
						break;
					}
				}
				string theDialogLines = string.Format(SexyFramework.GlobalMembers._ID("There is already a user named {0}", 412), text);
				GlobalMembers.gApp.DoDialog(0, true, SexyFramework.GlobalMembers._ID("SAME NAME", 413), theDialogLines, SexyFramework.GlobalMembers._ID("OK", 414), 3);
				return false;
			}
			BejeweledLivePlus.Misc.Common.Deltree(GetProfileDir(theProfileName));
			if (mNeedMoveProfileFiles)
			{
				MoveProfileFiles();
			}
			WriteProfile();
			if (clearProfile)
			{
				ClearProfile();
			}
			mProfileName = theProfileName;
			BejeweledLivePlus.Misc.Common.MkDir(GetProfileDir(theProfileName));
			WriteProfileList();
			return true;
		}

		public bool RenameProfile(string theOldProfileName, string theNewProfileName)
		{
			if (string.Compare(theOldProfileName, theNewProfileName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				ListRemoveName(theOldProfileName);
				ListAddName(theNewProfileName);
				mProfileName = theNewProfileName;
				return true;
			}
			WriteProfile();
			string profileDir = GetProfileDir(theOldProfileName);
			string profileDir2 = GetProfileDir(theNewProfileName);
			BejeweledLivePlus.Misc.Common.Deltree(profileDir2);
			BejeweledLivePlus.Misc.Common.MkDir(profileDir2);
			SexyFramework.GlobalMembers.gSexyApp.mFileDriver.MoveFile(profileDir, profileDir2);
			ListRemoveName(theOldProfileName);
			ListAddName(theNewProfileName);
			if (string.Compare(theOldProfileName, mProfileName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				mProfileName = theNewProfileName;
			}
			BejeweledLivePlus.Misc.Common.Deltree(profileDir);
			return true;
		}

		public bool DeleteProfile(string theProfileName)
		{
			if (string.Compare(theProfileName, mProfileName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				mProfileName = string.Empty;
			}
			BejeweledLivePlus.Misc.Common.Deltree(GetProfileDir(theProfileName));
			if (!ListRemoveName(theProfileName))
			{
				return false;
			}
			return false;
		}

		public bool WriteProfile()
		{
			if (mProfileName == string.Empty)
			{
				return false;
			}
			string text = GetProfileDir(mProfileName) + "\\profile.dat";
			BejeweledLivePlus.Misc.Common.MkDir(SexyFramework.Common.GetFileDir(text, false));
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			buffer.WriteInt32(958131957);
			buffer.WriteInt32(73);
			buffer.WriteInt32(mOfflineRank);
			buffer.WriteInt32(mStatsTodayDay);
			buffer.WriteInt32(mStatsTodayYear);
			buffer.WriteInt32(40);
			for (int i = 0; i < 40; i++)
			{
				buffer.WriteInt32(mStats[i]);
			}
			buffer.WriteInt32(40);
			for (int j = 0; j < 40; j++)
			{
				buffer.WriteInt32(mStatsToday[j]);
			}
			buffer.WriteInt32(5);
			for (int k = 0; k < 5; k++)
			{
				buffer.WriteInt32(mGameStats[k]);
			}
			buffer.WriteInt32(20);
			for (int l = 0; l < 20; l++)
			{
				buffer.WriteBoolean(mBadgeStatus[l]);
			}
			buffer.WriteInt32(4);
			for (int m = 0; m < 4; m++)
			{
				buffer.WriteBoolean(mEndlessModeUnlocked[m]);
			}
			buffer.WriteBoolean(mCustomCursors);
			buffer.WriteInt64((long)mTutorialFlags);
			buffer.WriteInt32(mTipIdx);
			buffer.WriteInt32(mLastQuestPage);
			buffer.WriteBoolean(mLastQuestBlink);
			for (int n = 0; n < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; n++)
			{
				for (int num = 0; num < BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET + BejeweledLivePlusAppConstants.QUESTS_OPTIONAL_PER_SET; num++)
				{
					buffer.WriteBoolean(mQuestsCompleted[n, num]);
				}
			}
			buffer.WriteString(mFacebookName);
			buffer.WriteString(mFacebookPassword);
			buffer.WriteBoolean(mFacebookAutoLogin);
			buffer.WriteInt32(mLocalDataMap.Count);
			Dictionary<string, string>.Enumerator enumerator = mLocalDataMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				buffer.WriteString(enumerator.Current.Key);
				buffer.WriteString(enumerator.Current.Value);
			}
			for (int num2 = 0; num2 < 5; num2++)
			{
				buffer.WriteInt32(mOfflineBoostCounts[num2]);
			}
			buffer.WriteInt32(mOnlineRank);
			buffer.WriteInt32((int)(mOnlineRankPoints & 0xFFFFFFFFu));
			buffer.WriteInt32((int)(mOfflineRankPoints & 0xFFFFFFFFu));
			buffer.WriteInt32(mOfflineGames);
			buffer.WriteInt32(mOnlineGames);
			buffer.WriteInt32(mFlags);
			buffer.WriteInt32((int)(mOfflineRankPoints >> 32));
			buffer.WriteInt32((int)(mOnlineRankPoints >> 32));
			buffer.WriteBoolean(mBreathOn);
			buffer.WriteBoolean(mBreathVisual);
			buffer.WriteFloat(mBreathSpeed);
			buffer.WriteBoolean(mNoiseOn);
			buffer.WriteString(mNoiseFileName);
			buffer.WriteBoolean(mBeatOn);
			buffer.WriteString(mSBAFileName);
			buffer.WriteBoolean(mAffirmationOn);
			buffer.WriteString(mAffirmationFileName);
			buffer.WriteBoolean(mAffirmationSubliminal);
			buffer.WriteFloat(mAffirmationSpeed);
			buffer.WriteFloat(mAffirmationSubliminality);
			buffer.WriteInt32(BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET);
			for (int num3 = 0; num3 < BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET; num3++)
			{
				buffer.WriteBoolean(mQuestHelpShown[num3]);
			}
			buffer.WriteBoolean(mAllowAnalytics);
			buffer.WriteBoolean(mAutoHint);
			buffer.WriteInt32(mImageNumber);
			buffer.WriteInt32(3);
			for (int num4 = 0; num4 < 3; num4++)
			{
				buffer.WriteInt32(mRecentBadges[num4]);
			}
			buffer.WriteInt32(mAmbientSelection);
			buffer.WriteInt32(mMantraSelection);
			buffer.WriteInt32((int)mRateGameChoice);
			buffer.WriteInt32(mRateGameSeenAt);
			buffer.WriteBoolean(mHasSeenIntro);
			buffer.WriteInt64((long)mTotalGamesPlayed);
			int count = mDeferredBadgeVector.Count;
			buffer.WriteInt32(count);
			for (int num5 = 0; num5 < count; num5++)
			{
				buffer.WriteInt32(mDeferredBadgeVector[num5]);
			}
			buffer.WriteInt32((int)mProfileId);
			buffer.WriteBoolean(mHasSeenGetBlitzDialog);
			buffer.WriteBoolean(mHasSeenOpenBlitzDialog);
			buffer.WriteBoolean(mMetricsPostAllAwardedBadges);
			mLast3MatchScoreManager.Save(buffer);
			SexyFramework.GlobalMembers.gSexyApp.WriteBufferToFile(text, buffer);
			return true;
		}

		public bool GetAnyProfile()
		{
			for (int i = 0; i < mProfileList.Count; i++)
			{
				if (LoadProfile(mProfileList[i]))
				{
					return true;
				}
			}
			return false;
		}

		public int GetProfileCount()
		{
			return mProfileList.Count;
		}

		public string GetProfile(int theProfile)
		{
			if (theProfile < 0 || theProfile >= GetProfileCount())
			{
				return string.Empty;
			}
			return mProfileList[theProfile];
		}

		public void MoveProfileFiles()
		{
			MoveProfileFilesHelper(GetProfileDir(string.Empty), GetProfileDir(mProfileName));
			mNeedMoveProfileFiles = false;
		}

		public string GetProfileDir(string theProfileName)
		{
			string saveDataPath = SexyFramework.GlobalMembers.gFileDriver.GetSaveDataPath();
			string result = saveDataPath + "users\\" + GlobalMembersProfile.ToUserDirectoryName(theProfileName);
			if (theProfileName == string.Empty)
			{
				result = saveDataPath + "users\\_temp";
			}
			return result;
		}

		public bool HasClearedTutorial(int theTutorial)
		{
			return (mTutorialFlags & (ulong)(1L << theTutorial)) != 0;
		}

		public void SetTutorialCleared(int theTutorial)
		{
			SetTutorialCleared(theTutorial, true);
		}

		public void SetTutorialCleared(int theTutorial, bool isCleared)
		{
			if (isCleared)
			{
				mTutorialFlags |= (ulong)(1L << theTutorial);
			}
			else
			{
				mTutorialFlags &= (ulong)(~(1L << theTutorial));
			}
		}

		public long GetRankPoints(uint theRank)
		{
			if (theRank == 0)
			{
				return 0L;
			}
			if (theRank >= pointRanges.Count)
			{
				return pointRanges[pointRanges.Count - 1];
			}
			return pointRanges[(int)theRank];
		}

		public float GetRankAtPoints(long thePoints)
		{
			for (int i = 0; i < pointRanges.Count - 1; i++)
			{
				if (thePoints < pointRanges[i])
				{
					return Math.Max(0, i - 1);
				}
			}
			if (thePoints >= pointRanges[pointRanges.Count - 2])
			{
				return pointRanges.Count - 2;
			}
			return 0f;
		}

		public List<int> GetPointRanges()
		{
			return pointRanges;
		}

		public string GetRankName()
		{
			return GlobalMembers.gApp.mRankNames[Math.Min(mOfflineRank, Common.size(GlobalMembers.gApp.mRankNames) - 1)];
		}

		public string GetRankName(GameMode mode)
		{
			return "Jeweler";
		}

		public int GetModeHighScore(GameMode mode)
		{
			return mHighScores[(int)mode];
		}

		public int GetModeHighScoreToday(GameMode mode)
		{
			return mHighScoresToday[(int)mode];
		}

		public bool UsesPresetProfilePicture()
		{
			return true;
		}

		public int GetProfilePictureId()
		{
			return mImageNumber;
		}

		public void SetProfilePictureId(int id)
		{
			mImageNumber = id;
		}

		public void AddRecentBadge(int badgeId)
		{
			for (int i = 0; i < 3; i++)
			{
				if (mRecentBadges[i] == badgeId)
				{
					return;
				}
			}
			for (int num = 1; num >= 0; num--)
			{
				mRecentBadges[num + 1] = mRecentBadges[num];
			}
			mRecentBadges[0] = badgeId;
		}

		public void UpdateRank(Board theBoard)
		{
			if (theBoard != null)
			{
				GlobalMembers.gApp.mProfile.mOfflineRankPoints += GetRankPointsBracket((int)((float)theBoard.mGameStats[1] / theBoard.GetRankPointMultiplier()));
				int num = GlobalMembers.gApp.mProfile.mOfflineRank;
				GlobalMembers.gApp.mProfile.mOfflineRank = (int)GlobalMembers.gApp.mProfile.GetRankAtPoints(GlobalMembers.gApp.mProfile.mOfflineRankPoints);
				if (GlobalMembers.gApp.mProfile.mOfflineRank > num)
				{
					string theEventString = $"RankUp Rank={GlobalMembers.gApp.mProfile.mOfflineRank} RankPoints={GlobalMembers.gApp.mProfile.mOfflineRankPoints} Seconds={GlobalMembers.gApp.mProfile.mStats[0]}";
					GlobalMembers.gApp.LogStatString(theEventString);
				}
			}
		}

		public int GetRankPointsBracket(int score)
		{
			int[] array = new int[27]
			{
				0, 25000, 50000, 75000, 100000, 125000, 150000, 175000, 200000, 225000,
				250000, 275000, 300000, 350000, 400000, 450000, 500000, 600000, 700000, 800000,
				900000, 1000000, 1200000, 1400000, 1600000, 1800000, 2000000
			};
			int num = 50;
			if (score <= 0)
			{
				return 0;
			}
			if (score > array[array.Length - 1])
			{
				return 1350;
			}
			for (int i = 1; i < array.Length; i++)
			{
				if (score > array[i - 1] && score <= array[i])
				{
					num += i * 50;
					break;
				}
			}
			return num;
		}

		public uint GetProfileId()
		{
			return mProfileId;
		}

		public void TryToGetLiveProfile()
		{
			// SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
			// try
			// {
			// 	signedInGamer.BeginGetProfile(GetProfileCallback, signedInGamer);
			// }
			// catch (Exception)
			// {
			// }
		}

		private void GetProfileCallback(IAsyncResult result)
		{
			lock (mGetProfileLock)
			{
				// Gamer gamer = result.AsyncState as Gamer;
				// if (gamer != null)
				// {
				// 	try
				// 	{
				// 		GamerProfile gamerProfile = gamer.EndGetProfile(result);
				// 		gamerProfile.GetGamerPicture();
				// 		return;
				// 	}
				// 	catch (Exception)
				// 	{
				// 		return;
				// 	}
				// }
			}
		}
	}
}
