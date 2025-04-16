using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public class Serialiser : Buffer
	{
		public enum TypeID : uint
		{
			SerialiserTypeSInt = 0u,
			SerialiserTypeUInt = 1048576u,
			SerialiserTypeSLong = 2097152u,
			SerialiserTypeULong = 3145728u,
			SerialiserTypeSShort = 4194304u,
			SerialiserTypeUShort = 5242880u,
			SerialiserTypeUChar = 6291456u,
			SerialiserTypeSChar = 7340032u,
			SerialiserTypeFloat = 8388608u,
			SerialiserTypeDouble = 9437184u,
			SerialiserTypeBool = 10485760u,
			SerialiserTypeSLongLong = 11534336u,
			SerialiserTypeULongLong = 12582912u,
			SerialiserTypeCurvedVal = 16777216u,
			SerialiserTypeStdString = 33554432u,
			SerialiserTypeUTF8String = 50331648u,
			SerialiserArray = 268435456u,
			SerialiserVector = 536870912u,
			SerialiserSpecialOne = 2147483648u,
			SerialiserSpecialTwo = 2415919104u,
			SerialiserTypeMask = 4293918720u
		}

		public enum PairID
		{
			Unknown,
			BoardUpdateCnt,
			BoardPieces,
			BoardBumpVelocities,
			BoardNextColumnCredit,
			BoardRand,
			BoardGameStats,
			BoardPoints,
			BoardPointsBreakdown,
			BoardMoneyDisp,
			BoardLevelBarPct,
			BoardCountdownBarPct,
			BoardLevelPointsTotal,
			BoardLevel,
			BoardPointMultiplier,
			BoardCurMoveCreditId,
			BoardCurMatchId,
			BoardGemFallDelay,
			BoardMoveCounter,
			BoardGameTicks,
			BoardIdleTicks,
			BoardLastMatchTick,
			BoardLastMatchTime,
			BoardMatchTallyCount,
			BoardSpeedModeFactor,
			BoardSpeedBonusAlpha,
			BoardSpeedBonusText,
			BoardSpeedometerPopup,
			BoardSpeedometerGlow,
			BoardSpeedBonusDisp,
			BoardSpeedNeedle,
			BoardSpeedBonusPoints,
			BoardSpeedBonusNum,
			BoardSpeedBonusCount,
			BoardSpeedBonusCountHighest,
			BoardSpeedBonusLastCount,
			BoardHasBoardSettled,
			BoardComboColors,
			BoardComboCount,
			BoardComboLen,
			BoardComboCountDisp,
			BoardComboFlashPct,
			BoardLastPlayerSwapColor,
			BoardWholeGameReplay,
			BoardNextPieceId,
			BoardScrambleDelayTicks,
			BoardGameFinished,
			BoardSwapData,
			BoardMoveData,
			BoardQueuedMoves,
			ReplayVersion,
			ReplayID,
			ReplaySaveBuffer,
			ReplayQueuedMoves,
			ReplayTutorialFlags,
			ReplayStateInfo,
			ReplayTicks,
			DigIdToTileData,
			DigTreasureEarnings,
			DigTextFlashTicks,
			DigNextBottomHypermixerWait,
			DigArtifactMinTiles,
			DigArtifactMaxTiles,
			DigNextArtifactTileCount,
			DigStartArtifactRow,
			DigHypercubes,
			DigGridDepth,
			DigRandRowIdx,
			DigFirstChestPt,
			DigArtifactPoss,
			DigPlacedArtifacts,
			DigCollectedArtifacts,
			DigTimeLimit,
			DigOldPieceData,
			DigRotatingCounter,
			DigChestOffsetY,
			DigChestPosX,
			DigChestSaveDataVec,
			SpeedBoard5SecChance,
			SpeedBoard5SecChanceStep,
			SpeedBoard5SecChanceLastRoll,
			SpeedBoard10SecChance,
			SpeedBoard10SecChanceStep,
			SpeedBoard10SecChanceLastRoll,
			SpeedBoardBonusTime,
			SpeedBoardBonusTimeDisp,
			SpeedBoardGameTicks,
			SpeedBoardCollectorExtendPct,
			SpeedBoardPanicScalePct,
			SpeedBoardTimeScaleOverride,
			SpeedBoardTotalBonusTime,
			IceStormStageNum,
			IceStormColCountColComboValueDisp,
			IceStormColCountStartTick,
			IceStormColCountStartUpdateTick,
			IceStormColCountDuration,
			IceStormColCountComboCount,
			IceStormColCountColComboAlpha,
			IceStormColCountColComboScale,
			IceStormColCountColComboY,
			IceStormColComboBonusPoints,
			IceStormColComboHighest,
			IceStormColClearBonusPoints,
			IceStormAnimUpdateCount,
			IceStormLoseColumn,
			IceStormLastIceRemoved,
			IceStormIceRemoved,
			IceStormStartDelay,
			IceStormLevelProgress,
			IceStormLevelProgressTotal,
			IceStormTotalLoseTicks,
			IceStormNextTryColStart,
			IceStormStageDuration,
			IceStormStageStartAtTick,
			IceStormShakeCooldown,
			IceStormBackDim,
			BflyDefMoveCountdown,
			BflyDefDropCountdown,
			BflyGrabbedAt,
			BflyDropCountdown,
			BflySpawnCountStart,
			BflyDropCountdownPerLevel,
			BflyMoveCountdownPerLevel,
			BflySpawnCountMax,
			BflySpawnCountPerLevel,
			BflySpawnCountAcc,
			BflyLidOpenPct,
			BflySpiderCol,
			BflySpiderWalkColFrom,
			BflySpiderWalkColTo,
			BflySpawnCount,
			BflySideSpawnChance,
			BflySideSpawnChancePerLevel,
			BflySideSpawnChanceMax,
			BflyAllowNewComboFloaters,
			BflySpiderWalkPct,
			BflyLidOpenTarget,
			PokerCardIdx,
			PokerCardScoreIdx,
			PokerGoal,
			PokerHands,
			PokerHandsDelat,
			PokerSkullsBusted,
			PokerBestHandsPts,
			PokerStartHands,
			PokerChipSoundDelay,
			PokerSkullHand,
			PokerSkullMax,
			PokerNumCoinFlips,
			PokerFlameBonus,
			PokerStarBonus,
			PokerScoreTally,
			PokerCurrentHandIdx,
			PokerFlameMoveCreditId,
			PokerLaserMoveCreditId,
			PokerHandCount,
			PokerSkullSpawnPct,
			PokerSkullBusterPct,
			PokerSkullBusterDisp,
			PokerBadFlip,
			PokerScoreHandPct,
			PokerCardBulgePct,
			PokerSkullScale,
			PokerHiLitePct,
			PokerCoinFlipPct,
			PokerCoinWonPct,
			PokerSkullCrusherAnimPct,
			PokerSkullBarLidPct,
			PokerScoreName,
			BlazingSpeedPercent,
			BlazingSpeedNum,
			MetricsVersion,
			GameSessionIdClassic,
			GameSessionIdZen,
			GameSessionIdDiamondMine,
			GameSessionIdButterfly
		}

		public enum Platforms
		{
			SerialiserPlatformUnknown,
			SerialiserPlatformIOSPhone,
			SerialiserPlatformIOSPad
		}

		public SaveFileHeader mHeader = new SaveFileHeader();

		public bool mNewerFile;

		public bool mNewerBoard;

		public GameChunkHeader[] mLoadedHeaders;

		public bool mChunkError;

		public bool mLoadingV1;

		public Serialiser()
		{
			mNewerFile = false;
			mNewerBoard = false;
			mChunkError = false;
			mLoadingV1 = false;
			mLoadedHeaders = new GameChunkHeader[10];
			for (int i = 0; i < 10; i++)
			{
				mLoadedHeaders[i] = new GameChunkHeader();
			}
		}

		public void Copyfrom(Serialiser data)
		{
			mHeader.Copyfrom(data.mHeader);
			mNewerFile = data.mNewerFile;
			mChunkError = data.mChunkError;
			mLoadingV1 = data.mLoadingV1;
			for (int i = 0; i < 10; i++)
			{
				mLoadedHeaders[i].CopyFrom(data.mLoadedHeaders[i]);
			}
			mDataBitSize = data.mDataBitSize;
			mReadBitPos = data.mReadBitPos;
			mWriteBitPos = data.mWriteBitPos;
			SetData(data.mData);
		}

		public void WriteFileHeader(int BoardVersion, int GameType)
		{
			WriteFileHeader(BoardVersion, GameType, 0);
		}

		public void WriteFileHeader(int BoardVersion, int GameType, int NumberGameChunks)
		{
			Seek(SeekMode.eWriteStart);
			mHeader.mOldHeader.mMagic = 37803;
			mHeader.mOldHeader.mGameVersion = 103;
			mHeader.mOldHeader.mBoardVersion = BoardVersion;
			mHeader.mOldHeader.mPlatform = 1;
			mHeader.mGameChunkCount = NumberGameChunks;
			mHeader.mGameType = GameType;
			mHeader.write(this);
		}

		public bool FinalizeFileHeader()
		{
			using (new BufferRestoreSeekRaii(this))
			{
				Seek(SeekMode.eReadStart);
				if (ReadInt32() != 37803)
				{
					return false;
				}
				SeekWriteByte(24);
				WriteInt32(mHeader.mGameChunkCount);
				return true;
			}
		}

		public int WriteGameChunkHeader(GameChunkId GameId)
		{
			return WriteGameChunkHeader(GameId, -1);
		}

		public int WriteGameChunkHeader(GameChunkId GameId, int Size)
		{
			int currWriteBytePos = GetCurrWriteBytePos();
			GameChunkHeader gameChunkHeader = new GameChunkHeader();
			gameChunkHeader.mMagic = 4557;
			gameChunkHeader.mId = (int)GameId;
			gameChunkHeader.mOffset = currWriteBytePos;
			gameChunkHeader.mSize = Size;
			gameChunkHeader.write(this);
			mHeader.mGameChunkCount++;
			return currWriteBytePos;
		}

		public bool FinalizeGameChunkHeader(int chunkBeginLoc)
		{
			int theInt = GetCurrWriteBytePos() - chunkBeginLoc - GameChunkHeader.size();
			using (new BufferRestoreSeekRaii(this))
			{
				SeekReadByte(chunkBeginLoc);
				if (ReadInt32() != 4557)
				{
					return false;
				}
				SeekWriteByte(chunkBeginLoc + 12);
				WriteInt32(theInt);
				return true;
			}
		}

		public void WriteValuePair(PairID id, int value)
		{
			WriteInt32((int)id);
			WriteInt32(value);
		}

		public void WriteValuePair(PairID id, uint value)
		{
			WriteInt32((int)((PairID)1048576 | id));
			WriteInt32((int)value);
		}

		public void WriteValuePair(PairID id, long value)
		{
			WriteInt32((int)((PairID)11534336 | id));
			WriteInt64(value);
		}

		public void WriteValuePair(PairID id, ulong value)
		{
			WriteInt32((int)((PairID)12582912 | id));
			WriteInt64((long)value);
		}

		public void WriteValuePair(PairID id, short value)
		{
			WriteInt32((int)((PairID)4194304 | id));
			WriteInt16(value);
		}

		public void WriteValuePair(PairID id, byte value)
		{
			WriteInt32((int)((PairID)6291456 | id));
			WriteByte(value);
		}

		public void WriteValuePair(PairID id, sbyte value)
		{
			WriteInt32((int)((PairID)7340032 | id));
			WriteByte((byte)value);
		}

		public void WriteValuePair(PairID id, float value)
		{
			WriteInt32((int)((PairID)8388608 | id));
			WriteFloat(value);
		}

		public void WriteValuePair(PairID id, double value)
		{
			WriteInt32((int)((PairID)9437184 | id));
			WriteDouble(value);
		}

		public void WriteValuePair(PairID id, bool value)
		{
			WriteInt32((int)((PairID)10485760 | id));
			WriteBoolean(value);
		}

		public void WriteValuePair(PairID id, CurvedVal value)
		{
			WriteInt32((int)((PairID)16777216 | id));
			WriteCurvedVal(value);
		}

		public void WriteArrayPair(PairID id, int num, int[] array)
		{
			WriteInt32((int)((PairID)268435456 | id));
			WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				WriteInt32(array[i]);
			}
		}

		public void WriteArrayPair(PairID id, int num, uint[] array)
		{
			WriteInt32((int)((PairID)269484032 | id));
			WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				WriteInt32((int)array[i]);
			}
		}

		public void WriteArrayPair(PairID id, int num, short[] array)
		{
			WriteInt32((int)((PairID)272629760 | id));
			WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				WriteInt16(array[i]);
			}
		}

		public void WriteArrayPair(PairID id, int num, byte[] array)
		{
			WriteInt32((int)((PairID)274726912 | id));
			WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				WriteByte(array[i]);
			}
		}

		public void WriteArrayPair(PairID id, int num, float[] array)
		{
			WriteInt32((int)((PairID)276824064 | id));
			WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				WriteFloat(array[i]);
			}
		}

		public void WriteArrayPair(PairID id, int num, double[] array)
		{
			WriteInt32((int)((PairID)277872640 | id));
			WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				WriteDouble(array[i]);
			}
		}

		public void WriteVectorPair(PairID id, List<int> array)
		{
			WriteInt32((int)((PairID)537919488 | id));
			int count = array.Count;
			WriteInt32(count);
			for (int i = 0; i < count; i++)
			{
				WriteInt32(array[i]);
			}
		}

		public void WriteByteVectorPair(PairID id, List<byte> buffer)
		{
			WriteInt32((int)((PairID)543162368 | id));
			int num = SexyFramework.Common.size(buffer);
			WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				WriteByte(buffer[i]);
			}
		}

		public void WriteBufferPair(PairID id, Buffer theBuffer)
		{
			WriteByteVectorPair(id, theBuffer.mData);
		}

		public void WriteStringPair(PairID id, string str)
		{
			WriteInt32((int)((PairID)33554432 | id));
			WriteString(str);
		}

		public void WriteSpecialBlock(PairID id, int num)
		{
			WriteInt32((int)((PairID)(-2146435072) | id));
			WriteInt32(num);
		}

		public void WriteSpecialBlock(PairID id, int num, int num2)
		{
			WriteInt32((int)((PairID)(-1877999616) | id));
			WriteInt32(num);
			WriteInt32(num2);
		}

		public bool ReadFileHeader(out int GameVersion, out int BoardVersion, out int platform)
		{
			GameVersion = -1;
			BoardVersion = -1;
			platform = -1;
			SeekFront();
			mHeader.mOldHeader.read(this);
			if (mHeader.mOldHeader.mMagic != 37803)
			{
				return false;
			}
			if (mHeader.mOldHeader.mGameVersion == 101)
			{
				mLoadingV1 = true;
			}
			else
			{
				mHeader.read(this, SaveFileHeader.ReadContent.Self);
			}
			GameVersion = mHeader.mOldHeader.mGameVersion;
			BoardVersion = mHeader.mOldHeader.mBoardVersion;
			platform = mHeader.mOldHeader.mPlatform;
			if (GameVersion < 101)
			{
				return false;
			}
			if (BoardVersion < 101)
			{
				return false;
			}
			if (GameVersion > 103)
			{
				mNewerFile = true;
			}
			if (BoardVersion > 101)
			{
				mNewerBoard = true;
			}
			if (mNewerFile)
			{
				SeekReadByte(GlobalMembersSerialiser.SaveFileHeader_GetOffsetToFirstChunk(mHeader));
			}
			if (!mLoadingV1)
			{
				for (int i = 0; i < 10; i++)
				{
					mLoadedHeaders[i] = new GameChunkHeader();
					mLoadedHeaders[i].zero();
				}
				using (new BufferRestoreSeekRaii(this))
				{
					int chunkBeginPos = 0;
					int num = 0;
					while (!mChunkError && num < mHeader.mGameChunkCount)
					{
						if (ReadGameChunkHeader(mLoadedHeaders[num], out chunkBeginPos))
						{
							Seek(SeekMode.eReadForward, mLoadedHeaders[num].mSize);
						}
						else
						{
							mChunkError = true;
						}
						num++;
					}
				}
				if (mChunkError)
				{
					return false;
				}
			}
			return true;
		}

		public bool ReadGameChunkHeader(GameChunkHeader header, out int chunkBeginPos)
		{
			chunkBeginPos = 0;
			if (mLoadingV1)
			{
				return true;
			}
			chunkBeginPos = GetCurrReadBytePos();
			header.read(this);
			if (header.mMagic != 4557)
			{
				chunkBeginPos = 0;
				return false;
			}
			return true;
		}

		public bool CheckReadGameChunkHeader(GameChunkId expectedChunkId, GameChunkHeader header, out int chunkBeginPos)
		{
			chunkBeginPos = 0;
			if (mLoadingV1)
			{
				return true;
			}
			bool flag = ReadGameChunkHeader(header, out chunkBeginPos);
			if (flag && expectedChunkId != (GameChunkId)header.mId)
			{
				return false;
			}
			return flag;
		}

		public void ReadValuePair(out int theValue)
		{
			ReadInt32();
			theValue = ReadInt32();
		}

		public void ReadValuePair(out uint theValue)
		{
			ReadInt32();
			theValue = (uint)ReadInt32();
		}

		public void ReadValuePair(out long theValue)
		{
			ReadInt32();
			theValue = ReadInt64();
		}

		public void ReadValuePair(out ulong theValue)
		{
			ReadInt32();
			theValue = (ulong)ReadInt64();
		}

		public void ReadValuePair(out short theValue)
		{
			ReadInt32();
			theValue = ReadInt16();
		}

		public void ReadValuePair(out byte theValue)
		{
			ReadInt32();
			theValue = ReadByte();
		}

		public void ReadValuePair(out sbyte theValue)
		{
			ReadInt32();
			theValue = (sbyte)ReadByte();
		}

		public void ReadValuePair(out float theValue)
		{
			ReadInt32();
			theValue = ReadFloat();
		}

		public void ReadValuePair(out double theValue)
		{
			ReadInt32();
			theValue = ReadDouble();
		}

		public void ReadValuePair(out bool theValue)
		{
			ReadInt32();
			theValue = ReadBoolean();
		}

		public void ReadValuePair(out CurvedVal theValue)
		{
			ReadInt32();
			theValue = ReadCurvedVal();
		}

		public int ReadArrayPair(int max, uint[] array)
		{
			ReadInt32();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				uint num2 = (uint)ReadInt32();
				if (i < max)
				{
					array[i] = num2;
				}
			}
			return num;
		}

		public int ReadArrayPair(int max, int[] array)
		{
			ReadInt32();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int num2 = ReadInt32();
				if (i < max)
				{
					array[i] = num2;
				}
			}
			return num;
		}

		public int ReadArrayPair(int max, short[] array)
		{
			ReadInt32();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				short num2 = ReadInt16();
				if (i < max)
				{
					array[i] = num2;
				}
			}
			return num;
		}

		public int ReadArrayPair(int max, sbyte[] array)
		{
			ReadInt32();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				sbyte b = (sbyte)ReadByte();
				if (i < max)
				{
					array[i] = b;
				}
			}
			return num;
		}

		public int ReadArrayPair(int max, float[] array)
		{
			ReadInt32();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				float num2 = ReadFloat();
				if (i < max)
				{
					array[i] = num2;
				}
			}
			return num;
		}

		public int ReadArrayPair(int max, double[] array)
		{
			ReadInt32();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				double num2 = ReadDouble();
				if (i < max)
				{
					array[i] = num2;
				}
			}
			return num;
		}

		public int ReadVectorPair(out List<int> array)
		{
			ReadInt32();
			int num = ReadInt32();
			array = new List<int>();
			for (int i = 0; i < num; i++)
			{
				array.Add(ReadInt32());
			}
			return num;
		}

		public void ReadByteVectorPair(out List<byte> buffer)
		{
			ReadInt32();
			int num = ReadInt32();
			buffer = new List<byte>();
			for (int i = 0; i < num; i++)
			{
				buffer.Add(ReadByte());
			}
		}

		public void ReadBufferPair(out Buffer theBuffer)
		{
			List<byte> buffer;
			ReadByteVectorPair(out buffer);
			theBuffer = new Buffer();
			theBuffer.SetData(buffer);
		}

		public void ReadStringPair(out string str)
		{
			ReadInt32();
			str = ReadString();
		}

		public void ReadSpecialBlock(out int num)
		{
			ReadInt32();
			num = ReadInt32();
		}

		public void ReadSpecialBlock(out int num, out int num2)
		{
			ReadInt32();
			num = ReadInt32();
			num2 = ReadInt32();
		}

		public void WriteIntArray(List<int> theArr)
		{
			WriteInt32(theArr.Count);
			for (int i = 0; i < theArr.Count; i++)
			{
				WriteInt32(theArr[i]);
			}
		}

		public void WriteCurvedVal(CurvedVal theCurvedVal)
		{
			int num = 0;
			num = ((theCurvedVal.mRamp != 0) ? ((theCurvedVal.mRamp == 1 && theCurvedVal.mInMin == theCurvedVal.mInMax && theCurvedVal.mOutMin == theCurvedVal.mOutMax) ? 1 : 2) : 0);
			WriteInt32(num);
			switch (num)
			{
			case 1:
				WriteDouble(theCurvedVal.mOutMin);
				break;
			case 2:
			{
				WriteString(theCurvedVal.mCurveCacheRecord.mDataStr);
				WriteByte(theCurvedVal.mMode);
				WriteByte(theCurvedVal.mRamp);
				WriteDouble(theCurvedVal.mIncRate);
				WriteDouble(theCurvedVal.mOutMin);
				WriteDouble(theCurvedVal.mOutMax);
				int theInt = (int)((theCurvedVal.mAppUpdateCountSrc != null) ? theCurvedVal.mAppUpdateCountSrc : GlobalMembers.gApp.mUpdateCount) - theCurvedVal.mInitAppUpdateCount;
				WriteInt32(theInt);
				WriteDouble(theCurvedVal.mCurOutVal);
				WriteDouble(theCurvedVal.mPrevOutVal);
				WriteDouble(theCurvedVal.mInMin);
				WriteDouble(theCurvedVal.mInMax);
				WriteBoolean(theCurvedVal.mNoClip);
				WriteBoolean(theCurvedVal.mSingleTrigger);
				WriteBoolean(theCurvedVal.mOutputSync);
				WriteBoolean(theCurvedVal.mTriggered);
				WriteBoolean(theCurvedVal.mIsHermite);
				WriteBoolean(theCurvedVal.mAutoInc);
				WriteDouble(theCurvedVal.mPrevInVal);
				WriteDouble(theCurvedVal.mInVal);
				break;
			}
			}
		}

		public List<int> ReadIntArray()
		{
			List<int> list = new List<int>();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				list.Add(ReadInt32());
			}
			return list;
		}

		public CurvedVal ReadCurvedVal()
		{
			CurvedVal curvedVal = new CurvedVal();
			switch (ReadInt32())
			{
			case 0:
				curvedVal.SetConstant(0.0);
				curvedVal.mRamp = 0;
				break;
			case 1:
			{
				double num2 = ReadDouble();
				curvedVal.SetConstant((float)num2);
				break;
			}
			case 2:
			{
				string text = ReadString();
				if (text.Length > 0)
				{
					curvedVal.SetCurve(text);
				}
				curvedVal.mMode = ReadByte();
				curvedVal.mRamp = ReadByte();
				curvedVal.mIncRate = ReadDouble();
				curvedVal.mOutMin = ReadDouble();
				curvedVal.mOutMax = ReadDouble();
				int num = ReadInt32();
				curvedVal.mInitAppUpdateCount = (int)((curvedVal.mAppUpdateCountSrc != null) ? curvedVal.mAppUpdateCountSrc : GlobalMembers.gApp.mUpdateCount) - num;
				curvedVal.mCurOutVal = ReadDouble();
				curvedVal.mPrevOutVal = ReadDouble();
				curvedVal.mInMin = ReadDouble();
				curvedVal.mInMax = ReadDouble();
				curvedVal.mNoClip = ReadBoolean();
				curvedVal.mSingleTrigger = ReadBoolean();
				curvedVal.mOutputSync = ReadBoolean();
				curvedVal.mTriggered = ReadBoolean();
				curvedVal.mIsHermite = ReadBoolean();
				curvedVal.mAutoInc = ReadBoolean();
				curvedVal.mPrevInVal = ReadDouble();
				curvedVal.mInVal = ReadDouble();
				break;
			}
			}
			return curvedVal;
		}

		public void WriteBytes(List<byte> buffer, int length)
		{
			int num = 0;
			foreach (byte item in buffer)
			{
				WriteByte(item);
				if (++num >= length)
				{
					break;
				}
			}
		}
	}
}
