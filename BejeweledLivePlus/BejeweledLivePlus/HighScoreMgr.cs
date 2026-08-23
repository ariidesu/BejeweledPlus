using System;
using System.Collections.Generic;
using Buffer = SexyFramework.Misc.Buffer;
using MTRand = SexyFramework.Misc.MTRand;

namespace BejeweledLivePlus
{
	public class HighScoreMgr
	{
		private static readonly string[] DEFAULT_NAMES =
		{
			"Abi", "Bill", "Bob", "Brian", "Chad", "Chris", "David", "Derek", "DJ",
			"Ed", "Ellen", "Heather", "Jake", "Jason", "Jeremy", "Josh", "Katie", "Leah",
			"Matt", "Michael", "Misael", "John", "Rick", "Sharon", "Snackers", "Stephen", "Tysen"
		};

		public Dictionary<string, HighScoreTable> mHighScoreMap = new Dictionary<string, HighScoreTable>();

		public bool mNeedSave;

		private bool mOnlineFailed;

		public HighScoreMgr()
		{
			mNeedSave = false;
		}

		public bool Submit(string theTable, string theName, int theValue, int thePicture)
		{
			HighScoreTable orCreateTable = GetOrCreateTable(theTable);
			if (orCreateTable.Submit(theName, theValue, thePicture))
			{
				GlobalMembers.gApp.SaveHighscores();
				return true;
			}
			return false;
		}

		public HighScoreTable GetOrCreateTable(string theTable)
		{
			string tableId = CanonicalizeTableId(theTable);
			HighScoreTable table = null;
			if (mHighScoreMap.TryGetValue(tableId, out table))
			{
				return table;
			}

			LocalHighScoreTable localTable = new LocalHighScoreTable(tableId);
			localTable.mManager = this;
			table = SupportsOnlineTable(tableId) ? new OnlineHighScoreTable(tableId, this, localTable) : localTable;
			table.mManager = this;
			mHighScoreMap.Add(tableId, table);
			mNeedSave = true;
			return table;
		}

		public bool Load(Buffer buffer)
		{
			try
			{
				int version = (int)buffer.ReadLong();
				uint key = unchecked((uint)buffer.ReadLong());
				if (version < GlobalMembersHighScoreMgr.HIGHSCORE_VERSION_MIN || version > GlobalMembersHighScoreMgr.HIGHSCORE_VERSION || key != GlobalMembersHighScoreMgr.HIGHSCORE_KEY)
				{
					return false;
				}

				int tableCount = (int)buffer.ReadLong();
				if (tableCount < 0 || tableCount > 256)
				{
					return false;
				}

				Dictionary<string, HighScoreTable> loadedTables = new Dictionary<string, HighScoreTable>();
				for (int i = 0; i < tableCount; i++)
				{
					string tableId = CanonicalizeTableId(buffer.ReadUTF8String());
					LocalHighScoreTable localTable = new LocalHighScoreTable(tableId);
					localTable.mManager = this;
					localTable.Load(buffer, version);
					HighScoreTable table = SupportsOnlineTable(tableId) ? new OnlineHighScoreTable(tableId, this, localTable) : localTable;
					table.mManager = this;
					loadedTables[tableId] = table;
				}

				mHighScoreMap.Clear();
				foreach (KeyValuePair<string, HighScoreTable> pair in loadedTables)
				{
					mHighScoreMap.Add(pair.Key, pair.Value);
				}
				mNeedSave = false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void Save(Buffer buffer)
		{
			int version = GlobalMembersHighScoreMgr.HIGHSCORE_VERSION;
			buffer.WriteLong(version);
			buffer.WriteLong(unchecked((int)GlobalMembersHighScoreMgr.HIGHSCORE_KEY));
			buffer.WriteLong(mHighScoreMap.Count);

			List<string> tableIds = new List<string>(mHighScoreMap.Keys);
			tableIds.Sort(StringComparer.Ordinal);
			foreach (string tableId in tableIds)
			{
				buffer.WriteUTF8String(tableId);
				GetLocalTable(mHighScoreMap[tableId]).Save(buffer, version);
			}
		}

		public void GenerateNativeDefaults()
		{
			MTRand random = new MTRand(1234);
			GenerateDefaults("CLASSIC", 35000, 5000, 10000, random);
			GenerateDefaults("LIGHTNING", 50000, 10000, 50000, random);
			GenerateDefaults("POKER", 50000, 10000, 50000, random);
			GenerateDefaults("BUTTERFLIES", 50000, 10000, 50000, random);
			GenerateDefaults("DIAMOND MINE", 50000, 10000, 50000, random);
			GenerateDefaults("ICE STORM", 50000, 10000, 50000, random);
			GenerateDefaults("BLITZ", 50000, 10000, 50000, random);
			GenerateDefaults("MATCH BOMB", 50000, 10000, 50000, random);
			GenerateDefaults("TIME BOMB", 50000, 10000, 50000, random);
			GenerateDefaults("INFERNOSTORM", 50000, 10000, 50000, random);
		}

		internal void DisableOnline()
		{
			mOnlineFailed = true;
		}

		internal bool IsOnlineAvailable()
		{
			return !mOnlineFailed && GlobalMembers.gApp != null && GlobalMembers.gApp.mGameCenterIsAvailable;
		}

		private void GenerateDefaults(string tableId, int baseScore, int primaryIncrement, int secondaryIncrement, MTRand random)
		{
			LocalHighScoreTable table = GetLocalTable(GetOrCreateTable(tableId));
			table.GenerateDefaults(baseScore, primaryIncrement, secondaryIncrement, true, random, DEFAULT_NAMES);
		}

		private static LocalHighScoreTable GetLocalTable(HighScoreTable table)
		{
			OnlineHighScoreTable onlineTable = table as OnlineHighScoreTable;
			if (onlineTable != null)
			{
				return onlineTable.GetLocalTable();
			}
			return (LocalHighScoreTable)table;
		}

		private static bool SupportsOnlineTable(string tableId)
		{
			switch (tableId)
			{
			case "BLITZ":
			case "MATCH BOMB":
			case "TIME BOMB":
				return false;
			default:
				return true;
			}
		}

		private static string CanonicalizeTableId(string tableName)
		{
			string input = tableName ?? string.Empty;
			switch (input.Trim().ToUpperInvariant())
			{
			case "CLASSIC":
				return "CLASSIC";
			case "LIGHTNING":
				return "LIGHTNING";
			case "POKER":
				return "POKER";
			case "BUTTERFLIES":
				return "BUTTERFLIES";
			case "DIAMOND MINE":
				return "DIAMOND MINE";
			case "ICE STORM":
			case "ICESTORM":
				return "ICE STORM";
			case "INFERNO STORM":
			case "INFERNOSTORM":
				return "INFERNOSTORM";
			case "BLITZ":
				return "BLITZ";
			case "MATCH BOMB":
				return "MATCH BOMB";
			case "TIME BOMB":
				return "TIME BOMB";
			}

			return GlobalMembers.gApp != null ? GlobalMembers.gApp.GetHighScoreTableId(input) : input;
		}
	}
}
