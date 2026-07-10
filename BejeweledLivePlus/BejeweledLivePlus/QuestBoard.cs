using System.Collections.Generic;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus
{
	public class QuestBoard : Board
	{
		public class GameTypeData
		{
			public int mPlays;

			public int mScoreTotal;

			public int mTimeTotal;

			public int mHighScore;

			public int mLowScore;

			public int mHypermixerCount;
		}

		public static Dictionary<string, GameTypeData> mGameData = new Dictionary<string, GameTypeData>();

		public uint[,] mGridState = new uint[8, 8];

		public QuestGoal mQuestGoal;

		public bool mIsPerpetual;

		public bool mAllowLevelUp;

		public bool mWantHyperMixers;

		public bool mWantShowPoints;

		public int mPowerGemThreshold;

		public int mLevelCompleteTicksStart;

		public int mLevelCompleteTicksAnnounce;

		public int mColorCountStart;

		public new int mUpdateCnt;

		public int mColorCount;

		public int mTicksInPlay;

		public bool mDoDrawGameElements;

		public bool mHelpHasBeenClosed;

		public EndLevelDialog mEndLevelDialog;

		public bool mRecordHighScores;

		public bool mHighScoreIsLevelPoints;

		public ButtonWidget mHelpButton;

		public CurvedVal mMenuButtonFlash = new CurvedVal();

		public bool mNeverAllowCascades;

		public int mDefaultBaseScore;

		public int mDefaultBaseScoreIncr;

		public int mQuestId;

		public bool mWantPointComplements;

		public Announcement mGameOverAnnouncement;

		public bool CallBoardCheckWin()
		{
			return base.CheckWin();
		}

		public void CallBoardFillInBlanks(bool allowCascades)
		{
			base.FillInBlanks(allowCascades);
		}

		public void CallBoardDrawHypercube(Graphics g, Piece thePiece)
		{
			base.DrawHypercube(g, thePiece);
		}

		public bool CallBoardIsHypermixerCancelledBy(Piece thePiece)
		{
			return base.IsHypermixerCancelledBy(thePiece);
		}

		public bool CallBoardIsGameSuspended()
		{
			return base.IsGameSuspended();
		}

		public Rect CallBoardGetCountdownBarRect()
		{
			return base.GetCountdownBarRect();
		}

		public bool CallBoardWantsBackground()
		{
			return base.WantsBackground();
		}

		public int CallBoardGetUICenterX()
		{
			return base.GetUICenterX();
		}

		public bool CallBoardWantBottomFrame()
		{
			return base.WantBottomFrame();
		}

		public bool CallBoardWantTopFrame()
		{
			return base.WantTopFrame();
		}

		public bool CallBoardWantTopLevelBar()
		{
			return base.WantTopLevelBar();
		}

		public bool CallBoardWantWarningGlow()
		{
			return CallBoardWantWarningGlow(false);
		}

		public bool CallBoardWantWarningGlow(bool forSound)
		{
			return base.WantWarningGlow(forSound);
		}

		public void CallBoardSetupBackground(int idx)
		{
			base.SetupBackground(idx);
		}

		public int CallBoardGetTimeDrawX()
		{
			return base.GetTimeDrawX();
		}

		public uint CallBoardGetRandSeedOverride()
		{
			return base.GetRandSeedOverride();
		}

		public bool CallBoardIsGameIdle()
		{
			return base.IsGameIdle();
		}

		public int CallBoardGetBoardX()
		{
			return base.GetBoardX();
		}

		public int CallBoardGetBoardY()
		{
			return base.GetBoardY();
		}

		public void CallBoardDrawUI(Graphics g)
		{
			base.DrawUI(g);
		}

		public void CallBoardDrawBottomFrame(Graphics g)
		{
			base.DrawBottomFrame(g);
		}

		public void CallBoardDeletePiece(Piece thePiece)
		{
			base.DeletePiece(thePiece);
		}

		public QuestBoard()
		{
			mQuestGoal = null;
			mDoDrawGameElements = true;
			mLevelCompleteTicksStart = 200;
			mLevelCompleteTicksAnnounce = 200;
			mQuestId = -1;
			mUiConfig = EUIConfig.eUIConfig_Quest;
			mHelpButton = null;
		}

		public override void Dispose()
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.Dispose();
			}
			if (!mIsPerpetual && GlobalMembers.gApp.mQuestMenu != null)
			{
				GlobalMembers.gApp.mQuestMenu.mBackground.SetVisible(true);
			}
			base.Dispose();
		}

		public override void LoadContent(bool threaded)
		{
			base.LoadContent(threaded);
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlayQuest");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlayQuest");
			}
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest");
		}

		public override void Init()
		{
			base.Init();
			mWantShowPoints = false;
			mWantHyperMixers = false;
			mWantPointComplements = false;
			mColorCountStart = 7;
			mColorCount = mColorCountStart;
			mNeverAllowCascades = false;
			mUpdateCnt = 0;
			mHighScoreIsLevelPoints = true;
			mShowLevelPoints = true;
			mTicksInPlay = 0;
			mAllowLevelUp = true;
			mDefaultBaseScore = GlobalMembers.M(10);
			mDefaultBaseScoreIncr = GlobalMembers.M(15);
			mBoardType = EBoardType.eBoardType_Quest;
			mEndLevelDialog = null;
			mRecordHighScores = mIsPerpetual;
			if (mParams.ContainsKey("PowerGemThreshold"))
			{
				mPowerGemThreshold = SexyFramework.GlobalMembers.sexyatoi(mParams["PowerGemThreshold"]);
			}
			bool flag = false;
			if (!mIsPerpetual && GlobalMembers.gApp.mProfile.WantQuestHelp(mQuestId))
			{
				flag = true;
			}
			else if (mIsPerpetual && mParams.ContainsKey("HelpText") && !string.IsNullOrEmpty(mParams["HelpText"]))
			{
				int num = 0;
				switch ((EEndlessMode)GlobalMembers.gApp.mLastDataParserId)
				{
				case EEndlessMode.ENDLESS_POKER:
					num = 0;
					break;
				case EEndlessMode.ENDLESS_BUTTERFLY:
					num = 1;
					break;
				case EEndlessMode.ENDLESS_ICESTORM:
					num = 2;
					break;
				case EEndlessMode.ENDLESS_GOLDRUSH:
					num = 3;
					break;
				}
				if ((GlobalMembers.gApp.mProfile.mFlags & (1 << num)) == 0)
				{
					flag = true;
					GlobalMembers.gApp.mProfile.mFlags |= 1 << num;
				}
			}
			if (flag)
			{
				mTimerAlpha.SetConstant(0.0);
			}
		}

		public override void InitUI()
		{
			if (mUiConfig != EUIConfig.eUIConfig_Quest)
			{
				base.InitUI();
				return;
			}
			if (mHintButton == null)
			{
				mHintButton = new Bej3Button(0, this);
				AddWidget(mHintButton);
			}
			if (mHelpButton == null)
			{
				mHelpButton = new ButtonWidget(4, this);
				AddWidget(mHelpButton);
			}
			if (mResetButton == null)
			{
				mResetButton = new ButtonWidget(2, this);
				AddWidget(mResetButton);
			}
			mReplayButton = null;
		}

		public override void RefreshUI()
		{
			if (mHelpButton != null)
			{
				mHelpButton.Resize(GlobalMembers.S(GetUICenterX() - 125), GlobalMembers.MS(755), GlobalMembers.MS(250), GlobalMembers.S(GlobalMembers.M(120) + GetBottomWidgetOffset()));
				mHelpButton.mBtnNoDraw = true;
				mHelpButton.mDoFinger = true;
				mHelpButton.mHasAlpha = true;
				mHelpButton.mDoFinger = true;
				mHelpButton.mOverAlphaSpeed = 0.1;
				mHelpButton.mOverAlphaFadeInSpeed = 0.2;
			}
		}

		public override void BoardSettled()
		{
			if ((double)mTimerAlpha > 0.0 || (!mIsPerpetual && !GlobalMembers.gApp.mProfile.WantQuestHelp(mQuestId)))
			{
				mHelpHasBeenClosed = true;
				ReadyToPlay();
			}
			else
			{
				OpenHelpDialog();
			}
			base.BoardSettled();
		}

		public override void DialogClosed(int theId)
		{
			if (theId == 40 && !mHelpHasBeenClosed)
			{
				mHelpHasBeenClosed = true;
				ReadyToPlay();
				StartTimer();
			}
			base.DialogClosed(theId);
		}

		public virtual void ReadyToPlay()
		{
		}

		public override bool AllowUI()
		{
			if (mLevelCompleteCount <= GlobalMembers.M(0))
			{
				return mGameOverCount <= GlobalMembers.M(0);
			}
			return false;
		}

		public override bool AllowSpeedBonus()
		{
			return false;
		}

		public override bool AllowNoMoreMoves()
		{
			return false;
		}

		public override bool AllowHints()
		{
			return true;
		}

		public override bool WantColorCombos()
		{
			return false;
		}

		public override bool WantHyperMixers()
		{
			return mWantHyperMixers;
		}

		public override void NewHyperMixer()
		{
			GameTypeData questStats = GetQuestStats();
			questStats.mHypermixerCount++;
		}

		public override int GetSidebarTextY()
		{
			if (mQuestGoal != null)
			{
				int sidebarTextY = mQuestGoal.GetSidebarTextY();
				if (sidebarTextY > 0)
				{
					return sidebarTextY;
				}
			}
			return base.GetSidebarTextY();
		}

		public override bool WantsTutorial(int theTutorialFlag)
		{
			return base.WantsTutorial(theTutorialFlag);
		}

		public override bool WantWarningGlow()
		{
			return WantWarningGlow(false);
		}

		public override bool WantWarningGlow(bool forSound)
		{
			if (mLevelCompleteCount != 0)
			{
				return false;
			}
			if (mQuestGoal != null)
			{
				return mQuestGoal.WantWarningGlow();
			}
			return base.WantWarningGlow(forSound);
		}

		public virtual void DoEndLevelDialog()
		{
			if (mQuestGoal == null || !mQuestGoal.DoEndLevelDialog())
			{
				mEndLevelDialog = new EndLevelDialog(this);
				mEndLevelDialog.SetQuestName(GetQuestName());
				GlobalMembers.gApp.AddDialog(38, mEndLevelDialog);
				BringToFront(mEndLevelDialog);
			}
		}

		public override bool WantBottomFrame()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.WantBottomFrame();
			}
			return base.WantBottomFrame();
		}

		public override bool WantTopLevelBar()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.WantTopLevelBar();
			}
			return base.WantTopLevelBar();
		}

		public override bool WantTopFrame()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.WantTopFrame();
			}
			return base.WantTopFrame();
		}

		public override bool WantPointComplements()
		{
			return mWantPointComplements;
		}

		public override bool WantsTutorialReplays()
		{
			return false;
		}

		public override void CelDestroyedBySpecial(int theCol, int theRow)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.CelDestroyedBySpecial(theCol, theRow);
			}
		}

		public override bool WantDrawTimer()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.WantDrawTimer();
			}
			return true;
		}

		public override bool WantsBackground()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.WantsBackground();
			}
			return base.WantsBackground();
		}

		public override void GameOverAnnounce()
		{
			base.GameOverAnnounce();
		}

		public virtual int GetTitleY()
		{
			return GlobalMembers.M(90);
		}

		public override bool CheckWin()
		{
			if (mIsPerpetual)
			{
				return false;
			}
			if (mQuestGoal != null)
			{
				return mQuestGoal.CheckWin();
			}
			return base.CheckWin();
		}

		public override void SetupBackground()
		{
			SetupBackground(0);
		}

		public override void SetupBackground(int theDeltaIdx)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.SetupBackground(theDeltaIdx);
			}
			else
			{
				base.SetupBackground(theDeltaIdx);
			}
		}

		public override int GetUICenterX()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.GetUICenterX();
			}
			return base.GetUICenterX();
		}

		public GameTypeData GetQuestStats()
		{
			string questName = GetQuestName();
			GameTypeData gameTypeData;
			if (mGameData.ContainsKey(questName))
			{
				gameTypeData = mGameData[questName];
			}
			else
			{
				gameTypeData = new GameTypeData();
				gameTypeData.mPlays = 0;
				gameTypeData.mScoreTotal = 0;
				gameTypeData.mTimeTotal = 0;
				gameTypeData.mHighScore = -1;
				gameTypeData.mLowScore = -1;
				gameTypeData.mHypermixerCount = 0;
				mGameData.Add(questName, gameTypeData);
			}
			return gameTypeData;
		}

		public void RecordQuestStats()
		{
		}

		public void StartTimer()
		{
			if ((double)mTimerAlpha == 0.0)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_TIMER_INFLATE, mTimerInflate);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_TIMER_ALPHA, mTimerAlpha);
			}
		}

		public void DoQuestPortal(Announcement theAnnouncement, bool theWon)
		{
			GlobalMembers.gApp.KillDialog(40);
			Flatten();
			GlobalMembers.gApp.DoQuestMenu(false);
			GlobalMembers.gApp.mQuestMenu.mDrawGemsAsOverlay = false;
			if (theAnnouncement != null)
			{
				GlobalMembers.gApp.mQuestMenu.SetPortalAnnouncement(theAnnouncement, theWon);
			}
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_QUEST_PORTAL_PCT_OPEN, mQuestPortalPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_QUEST_PORTAL_CENTER_PCT_OPEN, mQuestPortalPct, GlobalMembers.gApp.mBoard.mQuestPortalPct);
		}

		public override bool WantAutoload()
		{
			return true;
		}

		public override string GetMusicName()
		{
			if (GetTimeLimit() == 0)
			{
				return "QuestTurnBased";
			}
			return "QuestTimeBased";
		}

		public override bool SaveGameExtra(Serialiser theBuffer)
		{
			if (mQuestGoal != null && mQuestGoal.ExtraSaveGameInfo() && !mQuestGoal.SaveGameExtra(theBuffer))
			{
				return false;
			}
			if (ExtraSaveGameInfo())
			{
				int chunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkQuestBoard);
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						theBuffer.WriteInt32((int)mGridState[i, j]);
					}
				}
				theBuffer.FinalizeGameChunkHeader(chunkBeginLoc);
			}
			return true;
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			if (mQuestGoal != null && mQuestGoal.ExtraSaveGameInfo())
			{
				mQuestGoal.LoadGameExtra(theBuffer);
			}
			if (!ExtraSaveGameInfo())
			{
				return;
			}
			int chunkBeginPos = 0;
			GameChunkHeader header = new GameChunkHeader();
			if (!theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkQuestBoard, header, out chunkBeginPos))
			{
				return;
			}
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					mGridState[i, j] = (uint)theBuffer.ReadInt32();
				}
			}
		}

		public virtual bool ExtraSaveGameInfo()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.ExtraSaveGameInfo();
			}
			return false;
		}

		public override int GetTimeDrawX()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.GetTimeDrawX();
			}
			return base.GetTimeDrawX();
		}

		public override string GetSavedGameName()
		{
			if (mIsPerpetual)
			{
				switch ((EEndlessMode)GlobalMembers.gApp.mLastDataParserId)
				{
				case EEndlessMode.ENDLESS_POKER:
					return "poker.sav";
				case EEndlessMode.ENDLESS_BUTTERFLY:
					return "butterfly.sav";
				case EEndlessMode.ENDLESS_ICESTORM:
					return "ice_storm.sav";
				case EEndlessMode.ENDLESS_GOLDRUSH:
					return "diamond_mine.sav";
				}
			}
			return string.Empty;
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, 1);
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, false, 1);
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, -1, false, 1);
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, true, -1, false, 1);
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, true, true, -1, false, 1);
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor)
		{
			return AddPoints(theX, theY, thePoints, theColor, uint.MaxValue, true, true, -1, false, 1);
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd, int thePointType)
		{
			if (mWantShowPoints || (mQuestGoal != null && mQuestGoal.mWantShowPoints))
			{
				return base.AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, thePointType);
			}
			return null;
		}

		public override string GetLoggingGameName()
		{
			if (!mParams.ContainsKey("Title"))
			{
				mParams.Add("Title", "");
			}
			if (mIsPerpetual)
			{
				return "Perpetual " + mParams["Title"];
			}
			return mParams["Title"];
		}

		public override void LevelUp()
		{
			if (mGameOverPiece != null || !mAllowLevelUp || mLevelCompleteCount != 0)
			{
				return;
			}
			if (mIsPerpetual)
			{
				DoLevelUp();
				return;
			}
			LogGameOver("QuestCompleted " + GetExtraGameOverLogParams());
			mGameFinished = true;
			mLevelCompleteCount = mLevelCompleteTicksStart;
			if (mRecordHighScores)
			{
				GlobalMembers.gApp.mHighScoreMgr.Submit(GetQuestName(), GlobalMembers.gApp.mProfile.mProfileName, mLevelPointsTotal, GlobalMembers.gApp.mProfile.GetProfilePictureId());
			}
		}

		public virtual void DoLevelUp()
		{
			base.LevelUp();
		}

		public override void GameOver(bool visible)
		{
			if ((mQuestGoal == null || mQuestGoal.AllowGameOver()) && !CheckWin())
			{
				SubmitHighscore();
				bool flag = mGameOverCount > 0;
				base.GameOver(true);
				if (mIsPerpetual)
				{
				}
			}
		}

		public override void GameOverExit()
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.GameOverExit();
			}
			RecordQuestStats();
			if (mRecordHighScores)
			{
				GlobalMembers.gApp.DoGameDetailMenu(GlobalMembers.gApp.mCurrentGameMode, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_POST_GAME);
			}
			else if (mIsPerpetual)
			{
				GlobalMembers.gApp.DoSecretMenu();
			}
			else
			{
				BackToQuestMenu();
			}
		}

		public override void BackToMenu()
		{
			base.BackToMenu();
		}

		public void BackToQuestMenu()
		{
			if (GlobalMembers.gApp.mQuestMenu != null && GlobalMembers.gApp.mQuestMenu.mVisible)
			{
				GlobalMembers.KILL_WIDGET(GlobalMembers.gApp.mBoard);
				GlobalMembers.gApp.mBoard = null;
			}
			else
			{
				GlobalMembers.gApp.DoQuestMenu();
			}
		}

		public override void DrawMenuWidget(Graphics g)
		{
			if (mUiConfig != EUIConfig.eUIConfig_Quest)
			{
				base.DrawMenuWidget(g);
			}
			else if (mSidebarText != string.Empty)
			{
				g.SetFont(GlobalMembersResources.FONT_DIALOG);
				((ImageFont)GlobalMembersResources.FONT_DIALOG).PushLayerColor("MAIN", Color.White);
				((ImageFont)GlobalMembersResources.FONT_DIALOG).PushLayerColor("GLOW", Color.Black);
				string text = mSidebarText;
				if (mHelpButton.mIsOver)
				{
					text += GlobalMembers._ID("\n^FFD0FF^- more -", 415);
				}
				else
				{
					text += GlobalMembers._ID("\n^FF80FF^- more -", 416);
				}
				g.SetColor(new Color(255, 255, 255, 255));
				g.SetColor(Color.White);
				((ImageFont)GlobalMembersResources.FONT_DIALOG).PopLayerColor("MAIN");
				((ImageFont)GlobalMembersResources.FONT_DIALOG).PopLayerColor("GLOW");
			}
		}

		public override uint GetRandSeedOverride()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.GetRandSeedOverride();
			}
			return base.GetRandSeedOverride();
		}

		public virtual void TryGenerateDefaultScores()
		{
		}

		public string GetQuestName()
		{
			return GlobalMembers.gApp.GetModeHeading(GlobalMembers.gApp.mCurrentGameMode);
		}

		public override void ButtonDepress(int theId)
		{
			if (AllowUI())
			{
				if (theId == 4)
				{
					OpenHelpDialog();
				}
				base.ButtonDepress(theId);
			}
		}

		public void OpenHelpDialog()
		{
			GlobalMembers.gApp.mProfile.SetQuestHelpShown(mQuestId);
		}

		public override bool IsHypermixerCancelledBy(Piece thePiece)
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.IsHypermixerCancelledBy(thePiece);
			}
			return base.IsHypermixerCancelledBy(thePiece);
		}

		public override void DrawHypercube(Graphics g, Piece thePiece)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawHypercube(g, thePiece);
			}
			else
			{
				base.DrawHypercube(g, thePiece);
			}
		}

		public virtual void SetColorCount(int theColorCount)
		{
			if (mColorCount == theColorCount)
			{
				return;
			}
			mColorCount = theColorCount;
			GlobalMembers.M(1);
			mNewGemColors.Clear();
			for (int i = 0; i < theColorCount; i++)
			{
				if (theColorCount <= 6 && i == 5)
				{
					i = 6;
				}
				mNewGemColors.Add(i);
				mNewGemColors.Add(i);
			}
		}

		public override void DrawCountPopups(Graphics g)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawCountPopups(g);
			}
		}

		public override void ProcessMatches(List<MatchSet> theMatches, Dictionary<Piece, int> theTallySet, bool fromUpdateSwapping)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.ProcessMatches(theMatches, theTallySet, fromUpdateSwapping);
			}
			base.ProcessMatches(theMatches, theTallySet, fromUpdateSwapping);
		}

		public override void DeletePiece(Piece thePiece)
		{
			if (mQuestGoal == null || mQuestGoal.DeletePiece(thePiece))
			{
				base.DeletePiece(thePiece);
			}
		}

		public override bool CreateMatchPowerup(int theMatchCount, Piece thePiece, Dictionary<Piece, int> thePieceSet)
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.CreateMatchPowerup(theMatchCount, thePiece, thePieceSet);
			}
			return base.CreateMatchPowerup(theMatchCount, thePiece, thePieceSet);
		}

		public override bool PiecesDropped(List<Piece> thePieceVector)
		{
			if (mQuestGoal != null && !mQuestGoal.PiecesDropped(thePieceVector))
			{
				return false;
			}
			return base.PiecesDropped(thePieceVector);
		}

		public virtual void DoQuestBonus()
		{
			DoQuestBonus(1f);
		}

		public virtual void DoQuestBonus(float iOpt_multiplier)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DoQuestBonus(iOpt_multiplier);
			}
		}

		public virtual void DoQuestPenalty()
		{
			DoQuestPenalty(1f);
		}

		public virtual void DoQuestPenalty(float iOpt_multiplier)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DoQuestPenalty();
			}
		}

		public override int GetLevelPoints()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.GetLevelPoints();
			}
			return 0;
		}

		public override void DoHypercube(Piece thePiece, int theColor)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DoHypergem(thePiece, theColor);
			}
			base.DoHypercube(thePiece, theColor);
		}

		public override void SwapSucceeded(SwapData theSwapData)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.SwapSucceeded(theSwapData);
			}
			base.SwapSucceeded(theSwapData);
		}

		public override void FillInBlanks()
		{
			FillInBlanks(true);
		}

		public override void FillInBlanks(bool allowCascades)
		{
			if (mNeverAllowCascades)
			{
				allowCascades = false;
			}
			if (mGameTicks > 0)
			{
				CheckWin();
			}
			if (mGameOverCount != 0 || mLevelCompleteCount != 0)
			{
				allowCascades = false;
			}
			if (mQuestGoal != null)
			{
				mQuestGoal.FillInBlanks(allowCascades);
			}
			else
			{
				base.FillInBlanks(allowCascades);
			}
		}

		public override void DrawQuestPieces(Graphics g, Piece[] pPieces, int numPieces, bool thePostFX)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawPieces(g, pPieces, numPieces, thePostFX);
			}
		}

		public override void DrawPieces(Graphics g, bool thePostFX)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawPiecesPre(g, thePostFX);
			}
			base.DrawPieces(g, thePostFX);
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawPiecesPost(g, thePostFX);
			}
		}

		public override void DrawPieceShadow(Graphics g, Piece thePiece)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawPieceShadow(g, thePiece);
			}
			base.DrawPieceShadow(g, thePiece);
		}

		public override Rect GetCountdownBarRect()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.GetCountdownBarRect();
			}
			return base.GetCountdownBarRect();
		}

		public override bool WantSpecialPiece(List<Piece> thePieceVector)
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.WantSpecialPiece(thePieceVector);
			}
			return false;
		}

		public override bool DropSpecialPiece(List<Piece> thePieceVector)
		{
			if (mQuestGoal != null && mQuestGoal.DropSpecialPiece(thePieceVector))
			{
				return true;
			}
			return false;
		}

		public override void BlanksFilled(bool specialDropped)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.BlanksFilled(specialDropped);
			}
		}

		public override void SwapFailed(SwapData theSwapData)
		{
			base.SwapFailed(theSwapData);
		}

		public override void PieceDestroyedInSwap(Piece thePiece)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.PieceDestroyedInSwap(thePiece);
			}
		}

		public override int GetBoardX()
		{
			if (mQuestGoal != null)
			{
				int boardX = mQuestGoal.GetBoardX();
				if (boardX > 0)
				{
					return boardX;
				}
			}
			return base.GetBoardX();
		}

		public override int GetBoardY()
		{
			if (mQuestGoal != null)
			{
				int boardY = mQuestGoal.GetBoardY();
				if (boardY > 0)
				{
					return boardY;
				}
			}
			return base.GetBoardY();
		}

		public override bool IsGameSuspended()
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.IsGameSuspended();
			}
			return base.IsGameSuspended();
		}

		public override bool TriggerSpecial(Piece aPiece)
		{
			return TriggerSpecial(aPiece, null);
		}

		public override bool TriggerSpecial(Piece aPiece, Piece aSpecialPiece)
		{
			if (mQuestGoal != null && mQuestGoal.TriggerSpecial(aPiece, aSpecialPiece))
			{
				return true;
			}
			return base.TriggerSpecial(aPiece, aSpecialPiece);
		}

		public virtual TextNotifyEffect ShowQuestText(string i_text)
		{
			TextNotifyEffect textNotifyEffect = TextNotifyEffect.alloc();
			textNotifyEffect.mDuration = GlobalMembers.M(200);
			textNotifyEffect.mText = i_text;
			textNotifyEffect.mX = GlobalMembers.M(1000);
			textNotifyEffect.mY = GlobalMembers.M(620);
			mPostFXManager.AddEffect(textNotifyEffect);
			return textNotifyEffect;
		}

		public override void KeyChar(char theChar)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.KeyChar(theChar);
			}
			base.KeyChar(theChar);
		}

		public override bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped)
		{
			return TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, false);
		}

		public override bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap)
		{
			return TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, true, false);
		}

		public override bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol)
		{
			return TrySwap(theSelected, theSwappedRow, theSwappedCol, false, true, false);
		}

		public override bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped, bool destroyTarget)
		{
			if (mQuestGoal != null)
			{
				return mQuestGoal.TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, destroyTarget);
			}
			return base.TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, destroyTarget);
		}

		public virtual bool CallBaseTrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped)
		{
			return CallBaseTrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, false);
		}

		public virtual bool CallBaseTrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap)
		{
			return CallBaseTrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, true, false);
		}

		public virtual bool CallBaseTrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol)
		{
			return CallBaseTrySwap(theSelected, theSwappedRow, theSwappedCol, false, true, false);
		}

		public virtual bool CallBaseTrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped, bool destroyTarget)
		{
			return base.TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, destroyTarget);
		}

		public override void TallyPiece(Piece thePiece, bool thePieceDestroyed)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.TallyPiece(thePiece, thePieceDestroyed);
			}
			base.TallyPiece(thePiece, thePieceDestroyed);
		}

		public override void PieceTallied(Piece thePiece)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.PieceTallied(thePiece);
			}
			base.PieceTallied(thePiece);
		}

		public override void DoHypercube(Piece thePiece, Piece theSwappedPiece)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DoHypercube(thePiece, theSwappedPiece);
			}
			base.DoHypercube(thePiece, theSwappedPiece);
		}

		public override void ExamineBoard()
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.ExamineBoard();
			}
			base.ExamineBoard();
		}

		public override void NewGame()
		{
			NewGame(false);
		}

		public override void NewGame(bool restartingGame)
		{
			mUpdateCnt = 0;
			mWantShowPoints = mIsPerpetual;
			mRecordHighScores = mIsPerpetual;
			mHasBoardSettled = false;
			mHelpHasBeenClosed = false;
			mColorCountStart = 0;
			if (mParams.ContainsKey("ColorCount"))
			{
				int.TryParse(mParams["ColorCount"], out mColorCountStart);
				SetColorCount(mColorCountStart);
			}
			if (mParams.ContainsKey("RecordHighScores"))
			{
				bool.TryParse(mParams["RecordHighScores"], out mRecordHighScores);
			}
			if (mParams.ContainsKey("HighScoreBase"))
			{
				int.TryParse(mParams["HighScoreBase"], out mDefaultBaseScore);
			}
			if (mParams.ContainsKey("HighScoreIncr"))
			{
				int.TryParse(mParams["HighScoreIncr"], out mDefaultBaseScoreIncr);
			}
			if (mParams.ContainsKey("WantPointComplements"))
			{
				bool.TryParse(mParams["WantPointComplements"], out mWantPointComplements);
			}
			if (mParams.ContainsKey("NeverAllowCascades"))
			{
				bool.TryParse(mParams["NeverAllowCascades"], out mNeverAllowCascades);
			}
			if (!mParams.ContainsKey("ShowAutohints") || !bool.TryParse(mParams["ShowAutohints"], out mShowAutohints))
			{
				mShowAutohints = true;
			}
			string text = string.Empty;
			if (mParams.ContainsKey("Goal"))
			{
				text = mParams["Goal"].ToUpper();
			}
			if (mQuestGoal != null)
			{
				if (mQuestGoal != null)
				{
					mQuestGoal.Dispose();
				}
				mQuestGoal = null;
			}
			if (text == "DIG")
			{
				mQuestGoal = new DigGoal(this);
			}
			else if (text == "CATCH BUTTERFLIES")
			{
				mQuestGoal = new ButterflyGoal(this);
			}
			if (mQuestGoal != null)
			{
				mQuestGoal.NewGamePreSetup();
			}
			base.NewGame(restartingGame);
			if (mQuestGoal != null)
			{
				mQuestGoal.NewGame();
			}
			mSidebarText = mParams["Description"];
			if (mParams.ContainsKey("HyperMixers"))
			{
				bool.TryParse("HyperMixers", out mWantHyperMixers);
			}
		}

		public override void Update()
		{
			base.Update();
			if (mRewinding || mUserPaused || GlobalMembers.gApp.GetDialog(23) != null)
			{
				return;
			}
			if (!mIsPerpetual && mGameOverCount == GlobalMembers.M(300))
			{
				DoQuestPortal(null, false);
				mQuestPortalPct.SetInVal(GlobalMembers.M(0.5));
			}
			mTicksInPlay++;
			if (mLevelCompleteCount > 0)
			{
				if (mLevelCompleteCount > mLevelCompleteTicksAnnounce - 1)
				{
					if (IsBoardStill())
					{
						mLevelCompleteCount--;
					}
				}
				else if (mLevelCompleteCount == mLevelCompleteTicksAnnounce - 2)
				{
					if (IsBoardStill() && mLevelBarPct == GetLevelPct())
					{
						GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_CHALLENGECOMPLETE);
						mLevelCompleteCount--;
						if (GlobalMembers.gApp.mQuestMenu != null)
						{
							Announcement announcement = new Announcement(null, GlobalMembers._ID("QUEST\nCOMPLETED", 417));
							DoQuestPortal(announcement, true);
							if (GlobalMembers.gApp.mQuestMenu != null && GlobalMembers.gApp.mQuestMenu.IsLastQuestCompleted())
							{
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_ALPHA_QUEST_1, announcement.mAlpha);
							}
							else
							{
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_ALPHA_QUEST_2, announcement.mAlpha);
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_SCALE_QUEST, announcement.mScale);
							}
						}
					}
				}
				else
				{
					if ((mLevelCompleteCount > 50 || mDeferredTutorialVector.Count == 0) && GlobalMembers.gApp.GetDialog(41) == null)
					{
						mLevelCompleteCount--;
					}
					if (mLevelCompleteCount == GlobalMembers.M(100))
					{
						bool flag = GlobalMembers.gApp.mProfile.mQuestsCompleted[GlobalMembers.gApp.mQuestMenu.mQuestSetNum, GlobalMembers.gApp.mQuestMenu.mGemOver];
						GlobalMembers.gApp.mProfile.mQuestsCompleted[GlobalMembers.gApp.mQuestMenu.mQuestSetNum, GlobalMembers.gApp.mQuestMenu.mGemOver] = true;
						CalcBadges();
						GlobalMembers.gApp.mProfile.mQuestsCompleted[GlobalMembers.gApp.mQuestMenu.mQuestSetNum, GlobalMembers.gApp.mQuestMenu.mGemOver] = flag;
					}
					if (mLevelCompleteCount == 0)
					{
						if (!mIsPerpetual)
						{
							QuestMenu mQuestMenu = GlobalMembers.gApp.mQuestMenu;
						}
						GameOverExit();
					}
				}
			}
			if (mQuestGoal != null)
			{
				mQuestGoal.Update();
			}
			if (mHelpButton != null)
			{
				mHelpButton.mMouseVisible = (double)GetAlpha() * (double)mSideAlpha == 1.0;
				mHelpButton.mDisabled = false;
			}
			mUpdateCnt++;
		}

		public override void DoUpdate()
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DoUpdate();
			}
			base.DoUpdate();
		}

		public override void Draw(Graphics g)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.Draw(g);
			}
			base.Draw(g);
			if (!mIsPerpetual)
			{
				g.SetFont(GlobalMembersResources.FONT_HEADER);
				((ImageFont)g.mFont).PushLayerColor("OUTLINE", Color.Black);
				((ImageFont)g.mFont).PushLayerColor("GLOW", new Color(0, 0, 0, 128));
				g.WriteString(mParams["TitleText"], GlobalMembers.S(GetUICenterX()), GlobalMembers.S(GetTitleY()));
				((ImageFont)g.mFont).PopLayerColor("OUTLINE");
				((ImageFont)g.mFont).PopLayerColor("GLOW");
			}
			if (mDoDrawGameElements)
			{
				if (mQuestGoal != null)
				{
					mQuestGoal.DrawGameElements(g);
				}
				DrawGameElements(g);
				base.DrawGameElements(g);
				if (mQuestGoal != null)
				{
					mQuestGoal.DrawGameElementsPost(g);
				}
			}
		}

		public override void DrawPiece(Graphics g, Piece thePiece)
		{
			DrawPiece(g, thePiece, 1f);
		}

		public override void DrawPiece(Graphics g, Piece thePiece, float theScale)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawPiecePre(g, thePiece, theScale);
			}
			base.DrawPiece(g, thePiece, theScale);
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawPiecePost(g, thePiece, theScale);
			}
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			base.DrawOverlay(g, thePriority);
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawOverlay(g);
			}
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawOverlayPost(g);
			}
		}

		public override void DrawOverlayPreAnnounce(Graphics g, int thePriority)
		{
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawOverlay(g);
			}
			base.DrawOverlayPreAnnounce(g, thePriority);
		}

		public override void DrawOverlayPostAnnounce(Graphics g, int thePriority)
		{
			base.DrawOverlayPostAnnounce(g, thePriority);
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawOverlayPost(g);
			}
		}

		public override void DrawGameElements(Graphics g)
		{
		}

		public override void DrawScoreWidget(Graphics g)
		{
			if (mQuestGoal == null || !mQuestGoal.DrawScoreWidget(g))
			{
				DrawSpeedBonus(g);
				DrawLevelBar(g);
				g.SetColor(Color.FAlpha(GetAlpha()));
			}
		}

		public override void DrawScore(Graphics g)
		{
			if (mQuestGoal == null || !mQuestGoal.DrawScore(g))
			{
				base.DrawScore(g);
			}
		}

		public override void DrawUI(Graphics g)
		{
			base.DrawUI(g);
		}

		public override void DrawFrame(Graphics g)
		{
			base.DrawFrame(g);
		}

		public override bool CanPlay()
		{
			if (mQuestGoal != null && !mQuestGoal.CanPlay())
			{
				return false;
			}
			return base.CanPlay();
		}

		public override int GetPowerGemThreshold()
		{
			if (mQuestGoal != null && mQuestGoal.GetPowerGemThreshold() > 0)
			{
				return mQuestGoal.GetPowerGemThreshold();
			}
			if (mPowerGemThreshold > 0)
			{
				return mPowerGemThreshold;
			}
			return base.GetPowerGemThreshold();
		}

		public override bool GetTooltipText(Piece thePiece, ref string theHeader, ref string theBody)
		{
			string theHeader2 = theHeader;
			string theBody2 = theBody;
			if (mQuestGoal != null && mQuestGoal.GetTooltipText(thePiece, ref theHeader2, ref theBody2))
			{
				theHeader = theHeader2;
				theBody = theBody2;
				return true;
			}
			return base.GetTooltipText(thePiece, ref theHeader, ref theBody);
		}

		public override void SubmitHighscore()
		{
			if ((mQuestGoal == null || mQuestGoal.AllowGameOver()) && !CheckWin() && mRecordHighScores && mGameOverCount == 0)
			{
				TryGenerateDefaultScores();
				string questName = GetQuestName();
				HighScoreTable orCreateTable = GlobalMembers.gApp.mHighScoreMgr.GetOrCreateTable(questName);
				if (orCreateTable.Submit(GlobalMembers.gApp.mProfile.mProfileName, mHighScoreIsLevelPoints ? mLevelPointsTotal : mPoints, GlobalMembers.gApp.mProfile.GetProfilePictureId()))
				{
					GlobalMembers.gApp.SaveHighscores();
				}
			}
		}
	}
}
