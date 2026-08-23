using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class TimeBombBoard : QuestBoard
	{
		public int mBombCountdown;

		public int mDropCountdown;

		public int mMaxBombs;

		public bool mDecrementCounterGems;

		public int mBombDropCountdown;

		public int mBombCount;

		public bool mBoardWasStill;

		public bool mPendingBombDrop;

		public bool mRealTimeBombs;

		public int mMaxBombsPerUpdate;

		public int mDropCountDownPerUpdate;

		public int mMaxBombsUpdateScore;

		public int mDropCountDownUpdateScore;

		public int mBombCountDownUpdateScore;

		public int mMaxBombsAmount;

		public int mMinDropCountDownAmount;

		public int mBombCountDownAmount;

		public int mBombCountDown2ndAmount;

		public int mBombCountDown3rdAmount;

		public int mCurMaxBombsPerUpdate;

		public int mCurDropCountDownPerUpdate;

		public bool mAllowNewComboFloaters;

		public TimeBombBoard(bool realTimeBombs = false)
		{
			mRealTimeBombs = realTimeBombs;
		}

		public override void LoadContent(bool threaded)
		{
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlay_UI_Normal");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlay_UI_Normal");
			}
			BejeweledLivePlusApp.LoadContent("GamePlayQuest_TimeBomb");
			base.LoadContent(threaded);
		}

		public override void UnloadContent()
		{
			BejeweledLivePlusApp.UnloadContent("GamePlay_UI_Normal");
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_TimeBomb");
			base.UnloadContent();
		}

		public override void Init()
		{
			mUiConfig = EUIConfig.eUIConfig_StandardNoReplay;
			base.Init();
		}

		public override void RefreshUI()
		{
			mHintButton.Resize(ConstantsWP.BOARD_UI_HINT_BTN_X, ConstantsWP.BOARD_UI_HINT_BTN_Y, ConstantsWP.BOARD_UI_HINT_BTN_WIDTH, 0);
			mHintButton.mHasAlpha = true;
			mHintButton.mDoFinger = true;
			mHintButton.mOverAlphaSpeed = 0.1;
			mHintButton.mOverAlphaFadeInSpeed = 0.2;
			mHintButton.mWidgetFlagsMod.mRemoveFlags |= 4;
			mHintButton.mDisabled = false;
			mHintButton.SetOverlayType(Bej3Button.BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_NONE);
		}

		public override void NewGame(bool restartingGame)
		{
			mBombCountdown = SexyFramework.GlobalMembers.sexyatoi(mParams, "BombCountdown");
			mDropCountdown = SexyFramework.GlobalMembers.sexyatoi(mParams, "DropCountdown");
			mMaxBombs = SexyFramework.GlobalMembers.sexyatoi(mParams, "MaxBombs");
			mMaxBombsPerUpdate = SexyFramework.GlobalMembers.sexyatoi(mParams, "MaxBombsPerUpdate");
			mDropCountDownPerUpdate = SexyFramework.GlobalMembers.sexyatoi(mParams, "DropCountDownPerUpdate");
			mMaxBombsUpdateScore = SexyFramework.GlobalMembers.sexyatoi(mParams, "MaxBombsUpdateScore");
			mDropCountDownUpdateScore = SexyFramework.GlobalMembers.sexyatoi(mParams, "DropCountDownUpdateScore");
			mBombCountDownUpdateScore = SexyFramework.GlobalMembers.sexyatoi(mParams, "BombCountDownUpdateScore");
			mMaxBombsAmount = SexyFramework.GlobalMembers.sexyatoi(mParams, "MaxBombsAmount");
			mMinDropCountDownAmount = SexyFramework.GlobalMembers.sexyatoi(mParams, "MinDropCountDownAmount");
			mBombCountDownAmount = SexyFramework.GlobalMembers.sexyatoi(mParams, "BombCountDownAmount");
			mBombCountDown2ndAmount = SexyFramework.GlobalMembers.sexyatoi(mParams, "BombCountDown2ndAmount");
			mBombCountDown3rdAmount = SexyFramework.GlobalMembers.sexyatoi(mParams, "BombCountDown3rdAmount");
			mCurMaxBombsPerUpdate = 1;
			mCurDropCountDownPerUpdate = 1;
			mBombDropCountdown = 0;
			mBombCount = 0;
			mBoardWasStill = false;
			mPendingBombDrop = false;
			mDecrementCounterGems = false;
			base.NewGame(restartingGame);
		}

		public override void ReadyToPlay()
		{
			mDecrementCounterGems = true;
		}

		public override void Update()
		{
			base.Update();
			if (mGoAnnouncementDone && !mDecrementCounterGems)
			{
				ReadyToPlay();
			}
		}

		public override float GetModePointMultiplier()
		{
			if (mIsPerpetual)
			{
				return 10f;
			}
			return 1f;
		}

		public override float GetSpeedBonusRamp()
		{
			return GlobalMembers.M(0.12f);
		}

		public override float GetSpeedBonusMaxIncrement()
		{
			return GlobalMembers.M(0.16f);
		}

		public override bool DecrementCounterGem(Piece thePiece, bool immediate)
		{
			if (mDecrementCounterGems)
			{
				return base.DecrementCounterGem(thePiece, immediate);
			}
			return false;
		}

		public override bool PiecesDropped(List<Piece> thePieceVector)
		{
			mPendingBombDrop = false;
			int count = Common.size(thePieceVector);
			if (count > 0 && (mBombDropCountdown <= 0 || mBombCount == 0) && !HasSet() && mLevelCompleteCount == 0 && mGameOverCount == 0 && mBombCount < mMaxBombs)
			{
				for (int i = 0; i < 100; i++)
				{
					Piece piece = thePieceVector[(int)(mRand.Next() % count)];
					if (piece.mRow <= 4 && piece.mFlags == 0)
					{
						mPendingBombDrop = true;
						Bombify(piece, mBombCountdown, mRealTimeBombs);
						break;
					}
				}
			}
			return base.PiecesDropped(thePieceVector);
		}

		public override void BlanksFilled(bool specialDropped)
		{
			if (mPendingBombDrop)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BOMB_APPEARS);
				mBombDropCountdown = mDropCountdown;
				mBombCount++;
			}
			base.BlanksFilled(specialDropped);
		}

		public override void SwapSucceeded(SwapData theSwapData)
		{
			mBombDropCountdown--;
			base.SwapSucceeded(theSwapData);
		}

		public override void PieceTallied(Piece thePiece)
		{
			base.PieceTallied(thePiece);
			if (thePiece.IsFlagSet(96u))
			{
				mBombCount--;
				AddToStat(38, 1, thePiece.mMoveCreditId);
				int movingDefused = GetMoveStat(thePiece.mMoveCreditId, 38);
				MaxStat(39, movingDefused, thePiece.mMoveCreditId);
				int points = 5 * (mGameStats[38] + 20 * movingDefused + 10);
				mAllowNewComboFloaters = true;
				AddPoints((int)thePiece.CX(), (int)thePiece.CY(), points, Color.White, (uint)thePiece.mMatchId, true, true, thePiece.mMoveCreditId, false);
				mAllowNewComboFloaters = false;
			}
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd, int thePointType)
		{
			if (mIsPerpetual)
			{
				Points points = base.AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, thePointType);
				if ((int)theId > 0 &&
				    (mAllowNewComboFloaters || mPointsManager.Find((uint)(-2 - theMoveCreditId)) != null))
				{
					int moveStat = GetMoveStat(theMoveCreditId, 38);
					if (moveStat >= 2 && points != null)
					{
						Points points2 = AddPoints(theX, theY, 0, Color.White, (uint)(-2 - theMoveCreditId), true, true,
							theMoveCreditId, true);
						points2.mX = points.mX;
						points2.mY = points.mY + points.mScale * (float)GlobalMembers.M(1000);
						points2.mTimer = points.mTimer;
						for (int i = 0; i < GlobalMembers.Max_LAYERS; i++)
						{
							points2.mColorCycle[i] = points.mColorCycle[i];
						}

						points2.mString = string.Format(GlobalMembers._ID("x{0} COMBO", 158), moveStat);
					}
				}

				if (mMaxBombsUpdateScore > 0 && mPoints / (mCurMaxBombsPerUpdate * mMaxBombsUpdateScore) >= 1)
				{
					if (mMaxBombs < mMaxBombsAmount)
					{
						mMaxBombs += mMaxBombsPerUpdate;
						mCurMaxBombsPerUpdate++;
					}
				}
				if (mDropCountDownUpdateScore > 0 && mPoints / (mCurDropCountDownPerUpdate * mDropCountDownUpdateScore) >= 1)
				{
					if (mDropCountdown > mMinDropCountDownAmount)
					{
						mDropCountdown -= mDropCountDownPerUpdate;
						mCurDropCountDownPerUpdate++;
					}
				}
				if (mPoints >= mBombCountDownUpdateScore && mBombCountdown > mBombCountDownAmount)
				{
					mBombCountdown = mBombCountDownAmount;
				}
				if (mPoints >= 2 * mBombCountDownUpdateScore && mBombCountdown > mBombCountDown2ndAmount)
				{
					mBombCountdown = mBombCountDown2ndAmount;
				}
				if (mPoints >= 4 * mBombCountDownUpdateScore && mBombCountdown > mBombCountDown3rdAmount)
				{
					mBombCountdown = mBombCountDown3rdAmount;
				}
			
				return points;
			}

			return null;
		}

		public override bool WantSpecialPiece(List<Piece> thePieceVector)
		{
			return false;
		}

		public override bool DropSpecialPiece(List<Piece> thePieceVector)
		{
			return false;
		}

		public override bool WantsHideOnPause()
		{
			// real-time bombs must not tick while the pause menu hides the board
			return mRealTimeBombs;
		}

		public override bool AllowSpeedBonus()
		{
			return mIsPerpetual && mRealTimeBombs;
		}

		public override string GetMusicName()
		{
			return "QuestBomb";
		}

		public override string GetSavedGameName()
		{
			return mRealTimeBombs ? "real_time_bomb.sav" : "time_bomb.sav";
		}

		public override bool SaveGameExtra(Serialiser theBuffer)
		{
			int aChunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkTimeBombBoard);
			theBuffer.WriteValuePair(Serialiser.PairID.TimeBombMaxBombs, mMaxBombs);
			theBuffer.WriteValuePair(Serialiser.PairID.TimeBombCurMaxBombsPerUpdate, mCurMaxBombsPerUpdate);
			theBuffer.WriteValuePair(Serialiser.PairID.TimeBombDropCountdown, mDropCountdown);
			theBuffer.WriteValuePair(Serialiser.PairID.TimeBombCurDropCountDownPerUpdate, mCurDropCountDownPerUpdate);
			theBuffer.WriteValuePair(Serialiser.PairID.TimeBombBombCount, mBombCount);
			theBuffer.WriteValuePair(Serialiser.PairID.TimeBombBombDropCountdown, mBombDropCountdown);
			theBuffer.WriteValuePair(Serialiser.PairID.TimeBombBombCountdown, mBombCountdown);
			theBuffer.FinalizeGameChunkHeader(aChunkBeginLoc);
			return base.SaveGameExtra(theBuffer);
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			int aChunkBeginPos = 0;
			GameChunkHeader aHeader = new GameChunkHeader();
			if (theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkTimeBombBoard, aHeader, out aChunkBeginPos))
			{
				theBuffer.ReadValuePair(out mMaxBombs);
				theBuffer.ReadValuePair(out mCurMaxBombsPerUpdate);
				theBuffer.ReadValuePair(out mDropCountdown);
				theBuffer.ReadValuePair(out mCurDropCountDownPerUpdate);
				theBuffer.ReadValuePair(out mBombCount);
				theBuffer.ReadValuePair(out mBombDropCountdown);
				theBuffer.ReadValuePair(out mBombCountdown);
			}
			base.LoadGameExtra(theBuffer);
		}

		public override void SetupBackground(int theDeltaIdx)
		{
			string empty = string.Empty;
			empty = $"images\\{GlobalMembers.gApp.mArtRes}\\backgrounds\\water_bubbles_city";
			SetBackground(empty);
		}

		public override int WantExpandedTopWidget()
		{
			return 1;
		}
		
		public override bool WantTopLevelBar()
		{
			return false;
		}

		public override void DrawScore(Graphics g)
		{
			int x = ConstantsWP.NUM_BUTTERFLY_DISPLAY_X;
			int y = ConstantsWP.NUM_BUTTERFLY_DISPLAY_Y;
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			g.SetColor(new Color(255, 255, 255, (int)(255f * GetAlpha())));
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Bej3Widget.COLOR_SUBHEADING_4_STROKE);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Bej3Widget.COLOR_SUBHEADING_4_FILL);
			g.WriteString(SexyFramework.Common.CommaSeperate(mGameStats[38]), x, y, -1, 0);
			g.SetColor(Color.White);
		}
		
		public override void DrawUI(Graphics g)
		{
			DrawTopFrame(g);
			DrawBottomFrame(g);
			base.DrawUI(g);
		}

		public override void DrawTopFrame(Graphics g)
		{
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP_ID)));
		}
		
		public override void DrawBottomFrame(Graphics g)
		{
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_TIMEBOMB_BOMB, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_TIMEBOMB_BOMB_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_TIMEBOMB_BOMB_ID)));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_BG, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_BG_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_BG_ID)));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)));
			if (WantWarningGlow())
			{
				g.PushState();
				g.SetColor(GetWarningGlowColor());
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_TIMEBOMB_BOMB, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_TIMEBOMB_BOMB_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_TIMEBOMB_BOMB_ID)));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)));
				g.PopState();
			}
		}

		public override void GameOverExit()
		{
			SubmitHighscore();
			GameMode leaderboardMode = mRealTimeBombs ? GameMode.MODE_REALTIMEBOMB : GameMode.MODE_TIMEBOMB;
			GlobalMembers.gApp.DoGameDetailMenu(leaderboardMode, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_POST_GAME);
		}

		public override void SubmitHighscore()
		{
			GameMode leaderboardMode = mRealTimeBombs ? GameMode.MODE_REALTIMEBOMB : GameMode.MODE_TIMEBOMB;
			HighScoreTable table = GlobalMembers.gApp.mHighScoreMgr.GetOrCreateTable(GlobalMembers.gApp.GetModeHeading(leaderboardMode));
			if (table.Submit(GlobalMembers.gApp.mProfile.mProfileName, mPoints, GlobalMembers.gApp.mProfile.GetProfilePictureId()))
			{
				GlobalMembers.gApp.SaveHighscores();
			}
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			base.DrawOverlay(g, thePriority);
			int x = ConstantsWP.BFLY_SCORE_DISPLAY_X;
			int y = ConstantsWP.BFLY_SCORE_DISPLAY_Y;
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			g.SetColor(new Color(255, 255, 255, (int)(255f * GetAlpha())));
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, new Color(255, 255, 255, 255));
			g.WriteString(Common.CommaSeperate(mGameStats[1]), x, y, -1, 0);
			g.SetColor(Color.White);
		}
	}
}
