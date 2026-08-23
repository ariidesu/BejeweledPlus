using System;
using System.Collections.Generic;
using System.Text;
using Buffer = SexyFramework.Misc.Buffer;
using MTRand = SexyFramework.Misc.MTRand;

namespace BejeweledLivePlus
{
	public sealed class LocalHighScoreTable : HighScoreTable
	{
		private const int MAX_ENTRIES = 10;

		private const int SCORE_CHECKSUM_KEY = 0x42BEEF;

		public LocalHighScoreTable(string modeKey)
			: base(modeKey)
		{
		}

		public override bool SupportsTimeViews
		{
			get
			{
				return false;
			}
		}

		public override bool IsUsingLocalData
		{
			get
			{
				return true;
			}
		}

		public override bool Submit(string theName, int theValue, int thePicture)
		{
			for (int i = 0; i < mHighScoresLive.Count; i++)
			{
				if (string.Equals(mHighScoresLive[i].mName, theName ?? string.Empty, StringComparison.Ordinal) && mHighScoresLive[i].mScore == theValue)
				{
					return false;
				}
			}

			int insertAt = 0;
			while (insertAt < mHighScoresLive.Count && mHighScoresLive[insertAt].mScore >= theValue)
			{
				insertAt++;
			}

			if (insertAt >= MAX_ENTRIES)
			{
				return false;
			}

			int timestamp = unchecked((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
			mHighScoresLive.Insert(insertAt, new HighScoreEntryLive(theName ?? string.Empty, theValue, timestamp, true, thePicture));
			if (mHighScoresLive.Count > MAX_ENTRIES)
			{
				mHighScoresLive.RemoveAt(MAX_ENTRIES);
			}

			UpdateRanks();
			if (mManager != null)
			{
				mManager.mNeedSave = true;
			}
			return true;
		}

		public override void ReadLeaderboard(HighScoreTableTime t)
		{
			CanPageUp = false;
			CanPageDown = false;
			UpdateRanks();
			mLRState = LRState.LR_Ready;
			GlobalMembers.isLeaderboardLoading = false;
		}

		internal void GenerateDefaults(int baseScore, int primaryIncrement, int secondaryIncrement, bool onlyIfEmpty, MTRand random, IReadOnlyList<string> names)
		{
			if (onlyIfEmpty && mHighScoresLive.Count > 0)
			{
				return;
			}

			mHighScoresLive.Clear();
			HashSet<int> usedNames = new HashSet<int>();
			for (int i = 0; i < MAX_ENTRIES; i++)
			{
				int nameIndex;
				do
				{
					nameIndex = (int)random.Next((uint)names.Count);
				}
				while (!usedNames.Add(nameIndex));

				int score = i <= 4 ? baseScore + 5 * primaryIncrement + (4 - i) * secondaryIncrement : baseScore + (9 - i) * primaryIncrement;
				mHighScoresLive.Add(new HighScoreEntryLive(names[nameIndex], score, 0, false));
			}

			UpdateRanks();
			if (mManager != null)
			{
				mManager.mNeedSave = true;
			}
		}

		internal void Save(Buffer buffer, int version)
		{
			for (int i = 0; i < MAX_ENTRIES; i++)
			{
				HighScoreEntryLive entry = i < mHighScoresLive.Count ? mHighScoresLive[i] : null;
				string name = entry?.mName ?? string.Empty;
				int score = entry?.mScore ?? -1;
				int timestamp = entry?.mTimestamp ?? 0;

				buffer.WriteUTF8String(name);
				buffer.WriteLong(score);
				buffer.WriteLong(timestamp);
				buffer.WriteLong(CalculateChecksum(name, score));
				if (version >= 2)
				{
					buffer.WriteLong(entry?.mPicture ?? 0);
				}
			}
		}

		internal void Load(Buffer buffer, int version)
		{
			mHighScoresLive.Clear();
			HashSet<string> loadedEntries = new HashSet<string>(StringComparer.Ordinal);
			for (int i = 0; i < MAX_ENTRIES; i++)
			{
				string name = buffer.ReadUTF8String();
				int score = (int)buffer.ReadLong();
				int timestamp = (int)buffer.ReadLong();
				int checksum = (int)buffer.ReadLong();
				int picture = version >= 2 ? (int)buffer.ReadLong() : 0;

				if (checksum != CalculateChecksum(name, score))
				{
					score %= 256;
				}
				string entryKey = (name ?? string.Empty) + "\0" + score;
				if (score >= 0 && loadedEntries.Add(entryKey))
				{
					mHighScoresLive.Add(new HighScoreEntryLive(name, score, timestamp, false, picture));
				}
			}

			mHighScoresLive.Sort((left, right) => right.mScore.CompareTo(left.mScore));
			UpdateRanks();
		}

		private static int CalculateChecksum(string name, int score)
		{
			int checksum = score ^ SCORE_CHECKSUM_KEY;
			byte[] bytes = Encoding.UTF8.GetBytes(name ?? string.Empty);
			for (int i = 0; i < Math.Min(255, bytes.Length); i++)
			{
				checksum ^= 17 * bytes[i];
			}
			return checksum;
		}

		private void UpdateRanks()
		{
			for (int i = 0; i < mHighScoresLive.Count; i++)
			{
				mHighScoresLive[i].mRank = i + 1;
			}
		}
	}
}
