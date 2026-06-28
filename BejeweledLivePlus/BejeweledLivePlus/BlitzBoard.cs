using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Sound;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class BlitzBoard : Board
	{
		public int mMinutes = 1;

		public int mBoostsEnabled;

		public bool mDroppingMultiplier;

		public bool mDidTimeUp;

		public int mTimeUpCount;

		public bool mReadyForDrop;

		public int mWantGemsCleared;

		public int mDropGameTick;

		public bool mDidExtraTime;

		public int mCoinDelay;

		public int mScrambleDelayTicks;

		public int mHurrahPieceCount;

		public int mPreHurrahPoints;

		public CurvedVal mLastHurrahAlpha = new CurvedVal();

		public int mLastHurrahUpdates;

		public int mStarMedalIdx;

		public bool mPosted;

		public bool mGotBestScore;

		public int mPrevSpeedBonusPoints;

		public int mPostCoinCounter;

		public bool mReadyForStatsDisplay;

		public List<int> mSpeedBonusBreakdown = new List<int>();

		public List<int> mMultiplierTimes = new List<int>();

		public List<int> mMultiplierColors = new List<int>();

		public CurvedVal mGameSpeed = new CurvedVal();
		
		private bool mDrawingOverlay;

		public BlitzBoard()
		{
			mShowPointMultiplier = true;
			mMinutes = 1;
			mBoostsEnabled = 0;
			mDroppingMultiplier = false;
			mDidTimeUp = false;
			mTimeUpCount = 0;
			mReadyForDrop = true;
			mWantGemsCleared = 0;
			mDropGameTick = 0;
			mDidExtraTime = false;
			mCoinDelay = 0;
			mScrambleDelayTicks = 0;
			mHurrahPieceCount = 0;
			mPreHurrahPoints = 0;
			mStarMedalIdx = 0;
			mPosted = false;
			mGotBestScore = false;
			mPrevSpeedBonusPoints = 0;
			mPostCoinCounter = 0;
			mReadyForStatsDisplay = false;
			mLastHurrahAlpha.SetConstant(0.0);
			mLastHurrahUpdates = 0;
			mGameSpeed.SetConstant(1.0);
			mParams["Title"] = "Blitz";
		}

		public override string GetSavedGameName()
		{
			return "blitz.sav";
		}

		public override string GetMusicName()
		{
			return "Blitz";
		}

		public override int GetTimeLimit()
		{
			return 60 * mMinutes;
		}

		public override float GetModePointMultiplier()
		{
			return 5f;
		}

		public override float GetGameSpeed()
		{
			return (float)mGameSpeed.GetOutVal();
		}

		public override int WantExpandedTopWidget()
		{
			return 1;
		}

		public override float GetRankPointMultiplier()
		{
			return 5.6666665f;
		}

		public override int GetLevelPoints()
		{
			return 0;
		}

		public override int GetLevelPointsTotal()
		{
			return 0;
		}

		public override int GetTicksLeft()
		{
			if (mInUReplay)
			{
				return mUReplayTicksLeft;
			}
			int timeLimit = GetTimeLimit();
			if (timeLimit == 0)
			{
				return -1;
			}
			return System.Math.Min(timeLimit * 100, System.Math.Max(0, timeLimit * 100 - System.Math.Max(0, mGameTicks - 250)));
		}

		public int GetCurTimeSegment()
		{
			if (mTimeExpired)
			{
				return 0;
			}
			int num = (GetTicksLeft() + 499) / 500;
			if (num < 1)
			{
				num = 1;
			}
			if (!mDidExtraTime && (mBoostsEnabled & 1) != 0)
			{
				num++;
			}
			return num;
		}

		public override void Init()
		{
			base.Init();
			mDropGameTick = 0;
			mReadyForDrop = true;
			mDroppingMultiplier = false;
			mDidTimeUp = false;
			mHurrahPieceCount = 0;
			mTimeUpCount = 0;
			mPosted = false;
			mGotBestScore = false;
			mComboLen = 0;
			mWantGemsCleared = 0;
			mPreHurrahPoints = 0;
			mDidExtraTime = false;
			mCoinDelay = 0;
			mScrambleDelayTicks = 0;
			mStarMedalIdx = 0;
			mPrevSpeedBonusPoints = 0;
			mPostCoinCounter = 0;
			mReadyForStatsDisplay = false;
			mLastHurrahAlpha.SetConstant(0.0);
			mLastHurrahUpdates = 0;
			mGameSpeed.SetConstant(1.0);
			mSpeedBonusBreakdown.Clear();
			while (mSpeedBonusBreakdown.Count < 15)
			{
				mSpeedBonusBreakdown.Add(0);
			}
			mMultiplierTimes.Clear();
			mMultiplierColors.Clear();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_TIMER_INFLATE, mTimerInflate);
			mTimerAlpha.SetConstant(1.0);
			mGoDelayCount = 100;
		}

		public override void NewGame(bool restartingGame)
		{
			base.NewGame(restartingGame);
			mTimeDelayCount = 0;
			mReadyDelayCount = 0;
			mGoDelayCount = 0;
			mWantTimeAnnouncement = false;
		}

		public override void SetupBackground(int theDeltaIdx)
		{
			if (theDeltaIdx == 0 && mBackground != null)
			{
				return;
			}
			SetBackground($"images\\{GlobalMembers.gApp.mArtRes}\\backgrounds\\blitz");
		}

		public override void UpdateGame()
		{
			if (GetTicksLeft() == 1 && !mDidExtraTime && (mBoostsEnabled & 1) != 0)
			{
				mDidExtraTime = true;
			}
			base.UpdateGame();
			if (mGameTicks == 200)
			{
				Announcement go = new Announcement(this, GlobalMembers._ID("GO!", 141));
				go.mAlpha.mIncRate *= GlobalMembers.M(3.0);
				go.mScale.mIncRate *= GlobalMembers.M(3.0);
				go.mDarkenBoard = false;
				go.mBlocksPlay = false;
				go.mGoAnnouncement = true;
				GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_GO);
			}
			if (mCoinDelay > 0)
			{
				mCoinDelay--;
			}
			if (mScrambleDelayTicks > 0)
			{
				mScrambleDelayTicks--;
			}
			if (mGameOverCount == 350 && !mIsWholeGameReplay)
			{
				mWholeGameReplay.mReplayTicks = mUpdateCnt;
			}
			if (GetMaxMovesStat(4) < 8)
			{
				mReadyForDrop = true;
			}
		}

		public override void Update()
		{
			int prevGameTicks = mGameTicks;
			mLastHurrahAlpha.IncInVal();
			mLastHurrahUpdates++;
			mGameSpeed.IncInVal();
			base.Update();
			CustomBassMusicInterface theMusicInterface = GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface;
			SongInfo songInfo = theMusicInterface.FindSong(theMusicInterface.mSongName);
			if (songInfo != null)
			{
				double trackVol = System.Math.Min(0.99, (double)mPoints / ((double)GlobalMembers.gApp.mArtRes * 250000.0 / 1200.0));
				for (int trackId = 0; trackId < songInfo.mTracks.Count; trackId++)
				{
					songInfo.mTracks[trackId].mVolume.SetInVal(trackVol);
				}
				theMusicInterface.mForceParamUpdate = true;
			}
			if (mGameTicks != prevGameTicks)
			{
				if (!WantWarningGlow())
				{
					int ticksLeft = GetTicksLeft();
					if (ticksLeft % 100 == 0 && ticksLeft > 0 && ticksLeft <= GlobalMembers.M(800))
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TICK, 0, GlobalMembers.M(0.3), (ticksLeft / 100 % 2 == 0) ? 0f : GlobalMembers.M(-5f));
					}
				}
			}
			if (mDidTimeUp)
			{
				mTimeUpCount++;
			}
			if (mTimeExpired)
			{
				mPointMultPosPct.SetConstant(1.0);
			}
			if (mWantGemsCleared == 0)
			{
				mWantGemsCleared = 20;
			}
			if (mGameTicks % 400 == 0 && mGameTicks != prevGameTicks)
			{
				int roll = (int)(mRand.Next(5) % 5);
				if (mWantGemsCleared - roll >= 5)
				{
					mWantGemsCleared -= (int)(mRand.Next(5) % 5);
				}
				else
				{
					mWantGemsCleared = 5;
				}
			}
			if (!mDidTimeUp && !mKilling)
			{
				theMusicInterface.SetTempo(GetMusicName(), System.Math.Sqrt(mDispPoints) / 7.0 + 96.0);
			}
		}

		public override void Draw(Graphics g)
		{
			if ((double)mSlideUIPct < 1.0 || mGameOverCount <= 0)
			{
				base.Draw(g);
				DrawGameElements(g);
				DrawTimer(g);
			}
			else
			{
				DeferOverlay(0);
			}
		}

		public override bool WantDrawTimer()
		{
			return false;
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			base.DrawAll(theFlags, g);
			if (mPostCoinCounter >= 210 && !mIsWholeGameReplay)
			{
				mReadyForStatsDisplay = true;
			}
		}

		public override void DrawLevelBar(Graphics g)
		{
		}

		public void DrawMultiplierLarge(Graphics g)
		{
			if (mPointMultiplier <= 1)
			{
				return;
			}
			if (!mPointMultTextMorph.IsDoingCurve() && !mPointMultAlpha.IsDoingCurve())
			{
				return;
			}

			Image backImg = GlobalMembersResourcesWP.IMAGE_MULTIPLIER_LARGE_BACK;
			Image frontImg = GlobalMembersResourcesWP.IMAGE_MULTIPLIER_LARGE_FRONT;
			if (backImg == null || frontImg == null)
			{
				return;
			}

			int srcX = (int)GlobalMembers.S(mSrcPointMultPos.mX);
			int srcY = (int)GlobalMembers.S(mSrcPointMultPos.mY);

			Image multiplierImage = GetMultiplierImage();
			int multiplierImageX = GetMultiplierImageX();
			int multiplierImageY = GetMultiplierImageY();
			int hudX = multiplierImageX + multiplierImage.GetCelWidth() / 2;
			int hudY = multiplierImageY + ConstantsWP.BOARD_MULTIPLIER_Y + ConstantsWP.BOARD_MULTIPLIER_LARGE_Y;

			double t = mPointMultPosPct.GetOutVal();
			double w1 = System.Math.Max(0.0, 2.0 * t - 1.0);
			double w2 = System.Math.Max(0.0, 1.0 - 2.0 * t);
			double w3 = 1.0 - System.Math.Abs(2.0 * t - 1.0);

			int midX = GlobalMembers.S(GetBoardCenterX());
			int midY = GlobalMembers.S(GetBoardCenterY());

			int drawX = (int)((double)srcX * w2 + (double)midX * w3 + (double)hudX * w1);
			int drawY = (int)((double)srcY * w2 + (double)midY * w3 + (double)hudY * w1);
			int yAdd = (int)(mPointMultYAdd.GetOutVal() * (double)GlobalMembers.gApp.mArtRes / 1200.0);
			drawY += yAdd;

			int celIndex = mPointMultiplier - 1;
			if (celIndex < 0) celIndex = 0;
			int celCount = backImg.GetCelCount();
			if (celIndex >= celCount) celIndex = celCount - 1;

			float scaleF = (float)mPointMultScale.GetOutVal();

			int celWidth = backImg.GetCelWidth();
			int celHeight = backImg.GetCelHeight();

			double pieceAlpha = GetPieceAlpha();
			double slidePct = mSlideUIPct.GetOutVal();
			double multAlpha = mPointMultAlpha.GetOutVal();
			double textMorph = mPointMultTextMorph.GetOutVal();

			int backAlpha = (int)(pieceAlpha * 255.0 * (1.0 - slidePct) * multAlpha * (1.0 - textMorph) * 0.5);
			if (backAlpha > 0)
			{
				g.PushState();
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, backAlpha));
				if (scaleF != 1.0f)
				{
					g.SetScale(scaleF, scaleF, drawX, drawY);
				}
				GlobalMembers.gGR.DrawImageCel(g, backImg, drawX - celWidth / 2, drawY - celHeight / 2, celIndex);
				g.PopState();
			}

			int frontAlpha = (int)(pieceAlpha * 255.0 * (1.0 - slidePct) * (1.0 - textMorph));
			if (frontAlpha > 0)
			{
				g.PushState();
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, frontAlpha));
				if (scaleF != 1.0f)
				{
					g.SetScale(scaleF, scaleF, drawX, drawY);
				}
				GlobalMembers.gGR.DrawImageCel(g, frontImg, drawX - celWidth / 2, drawY - celHeight / 2, celIndex);
				g.PopState();
			}
		}

		public override void DrawScore(Graphics g)
		{
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			string theString = SexyFramework.Common.CommaSeperate(mDispPoints);
			int num = mWidth / 2;
			int num2 = (int)(GlobalMembers.IMG_SYOFS(897) + (float)GlobalMembersResources.FONT_DIALOG.mAscent) / 2;
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Color.White);
			float mScaleX = g.mScaleX;
			float mScaleY = g.mScaleY;
			g.SetScale(ConstantsWP.BOARD_LEVEL_SCORE_SCALE, ConstantsWP.BOARD_LEVEL_SCORE_SCALE, num, num2 - g.GetFont().GetAscent() / 2);
			g.WriteString(theString, num, num2);
			g.mScaleX = mScaleX;
			g.mScaleY = mScaleY;
		}

		public override void DrawTimer(Graphics g)
		{
			if ((mTimeExpired && mHurrahPieceCount > 0) || mIsWholeGameReplay)
			{
				return;
			}
			int ticksLeft = GetTicksLeft();
			Rect countdownBarRect = GetCountdownBarRect();
			int drawX = GetTimeDrawX();
			int restY = countdownBarRect.mY + countdownBarRect.mHeight / 2 + 8;
			int peakY = GlobalMembers.MS(500);
			double inflateVal = mTimerInflate.GetOutVal();
			double deflate = 1.0 - inflateVal;
			int drawY = (int)((double)peakY * inflateVal + (double)restY * deflate);
			float scaleF = (float)(inflateVal + 0.8f);
			string timeStr = string.Format(GlobalMembers._ID("{0}:{1:d2}", 148), (ticksLeft + 99) / 100 / 60, (ticksLeft + 99) / 100 % 60);
			g.PushState();
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, new Color(0, 0, 0, 85));
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 1, Color.White);
			g.SetColor(new Color(255, 255, 255, (int)((double)(255f * GetAlpha()) * (double)mTimerAlpha)));
			g.SetScale(scaleF, scaleF, drawX, drawY);
			g.WriteString(timeStr, drawX, drawY + GetTimerYOffset());
			g.PopState();
		}

		public override int GetTimerYOffset()
		{
			return 0;
		}

		public override void DrawWarningHUD(Graphics g)
		{
			g.PushState();
			Color color = g.GetColor();
			g.SetDrawMode(Graphics.DrawMode.Additive);
			g.SetColorizeImages(true);
			Color warningGlowColor = GetWarningGlowColor();
			g.SetColor(warningGlowColor);
			g.PushColorMult();
			mDrawingOverlay = true;
			DrawFrame(g);
			mDrawingOverlay = false;
			g.PopColorMult();
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColor(color);
			g.PopState();
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd)
		{
			int curTimeSegment = GetCurTimeSegment();
			mSpeedBonusBreakdown[curTimeSegment] += mSpeedBonusPoints - mPrevSpeedBonusPoints;
			mPrevSpeedBonusPoints = mSpeedBonusPoints;
			int prevPoints = mPoints;
			Points result = base.AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd);
			mPointsBreakdown[mPointsBreakdown.Count - 1][0] += mPoints - prevPoints;
			return result;
		}

		public override void IncPointMult(Piece thePieceFrom)
		{
			base.IncPointMult(thePieceFrom);
			if (!mTimeExpired)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BACKGROUND_CHANGE);
				int num = (GetTicksLeft() + 99) / 100;
				if (num < 0)
				{
					num = 0;
				}
				int secondNum = num;
				if (!mDidExtraTime && (mBoostsEnabled & 1) != 0)
				{
					secondNum = num + 5;
				}
				mMultiplierTimes.Add(secondNum);
				mMultiplierColors.Add(thePieceFrom.mColor);
			}
		}

		public override bool WantSpecialPiece(List<Piece> thePieceVector)
		{
			mDroppingMultiplier = false;
			if (!mReadyForDrop || mWantGemsCleared == 0 || mDidTimeUp)
			{
				return false;
			}
			int maxMovesStat = GetMaxMovesStat(4);
			int pointMultiplier = mPointMultiplier;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					if (piece != null && piece.IsFlagSet(16u))
					{
						pointMultiplier++;
					}
				}
			}
			if (pointMultiplier == 1)
			{
				if (mGameTicks - mDropGameTick < 1000 && mDropGameTick != 0)
				{
					return false;
				}
				if (maxMovesStat < 12)
				{
					return false;
				}
			}
			else if (maxMovesStat < mWantGemsCleared)
			{
				return false;
			}
			mDroppingMultiplier = true;
			if (pointMultiplier >= 9)
			{
				mDroppingMultiplier = false;
			}
			return mDroppingMultiplier;
		}

		public override bool DropSpecialPiece(List<Piece> thePieceVector)
		{
			int size = Common.size(thePieceVector);
			if (size == 0)
			{
				return false;
			}
			int index = (int)mRand.Next() % size;
			for (int attempt = 0; attempt < 7; attempt++)
			{
				thePieceVector[index].mColor = (int)mRand.Next() % 7;
				int colorCount = 0;
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						Piece piece = mBoard[i, j];
						if (piece != null && piece.mY > 0f && piece.mColor == thePieceVector[index].mColor)
						{
							colorCount++;
						}
					}
				}
				if (colorCount > 3)
				{
					break;
				}
			}
			thePieceVector[index].SetFlag(16u);
			if (WantsTutorial(5))
			{
				DeferTutorialDialog(5, thePieceVector[index]);
			}
			mDropGameTick = mGameTicks;
			mReadyForDrop = false;
			mWantGemsCleared = 0;
			return true;
		}

		public override bool PiecesDropped(List<Piece> thePieceVector)
		{
			// TODO: Boosts
			return true;
		}

		public override void GameOver()
		{
			GameOver(true);
		}

		public override void GameOver(bool visible)
		{
			if (mWantLevelup || mHyperspace != null)
			{
				return;
			}
			mCursorSelectPos = new Point(-1, -1);
			if (!mDidTimeUp)
			{
				mPreHurrahPoints = mPoints;
				(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("FadeOut", GetMusicName(), false);
				(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("Play", GetMusicName() + "_lose", true);
				GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_TIMEUP);
				mDidTimeUp = true;
				mTimeExpired = true;
				Announcement timeUp = new Announcement(this, GlobalMembers._ID("TIME UP", 480));
				timeUp.mDarkenBoard = false;
				mGameSpeed.SetCurve("b;0,1,0.003333,1,~###    I~### qy|Em   e}###");
			}
			if (mSpeedBonusCount > 0)
			{
				EndSpeedBonus();
			}
			int specialCount = 0;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					if (piece == null)
					{
						continue;
					}
					if (piece.IsFlagSet(PIECEFLAG.PIECEFLAG_FLAME) || piece.IsFlagSet(PIECEFLAG.PIECEFLAG_HYPERCUBE) || piece.IsFlagSet(PIECEFLAG.PIECEFLAG_LASER) || piece.IsFlagSet(PIECEFLAG.PIECEFLAG_POINT_MULTIPLIER) || piece.IsFlagSet(PIECEFLAG.PIECEFLAG_DETONATOR) || piece.IsFlagSet(PIECEFLAG.PIECEFLAG_SCRAMBLE) || piece.IsFlagSet(PIECEFLAG.PIECEFLAG_BLAST_GEM) || piece.IsFlagSet(PIECEFLAG.PIECEFLAG_TIME_BONUS))
					{
						if (piece.IsFlagSet(PIECEFLAG.PIECEFLAG_COIN))
						{
							piece.mDestructing = true;
						}
						if (mTimeUpCount == 0)
						{
							piece.mExplodeDelay = GlobalMembers.M(175) + specialCount * GlobalMembers.M(25);
						}
						else
						{
							piece.mExplodeDelay = GlobalMembers.M(25) + specialCount * GlobalMembers.M(25);
						}
						specialCount++;
						mHurrahPieceCount++;
					}
				}
			}
			if (specialCount == 0)
			{
				for (int k = 0; k < 8; k++)
				{
					for (int l = 0; l < 8; l++)
					{
						Piece piece2 = mBoard[k, l];
						if (piece2 != null && piece2.IsFlagSet(PIECEFLAG.PIECEFLAG_COIN) && !piece2.mTallied)
						{
							piece2.mDestructing = true;
							TallyPiece(piece2, true);
							piece2.mAlpha.SetConstant(1.0);
							piece2.ClearFlag(PIECEFLAG.PIECEFLAG_COIN);
							specialCount++;
						}
					}
				}
			}
			if (specialCount > 0 && (double)mLastHurrahAlpha == 0.0)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_LAST_HURRAH_ALPHA_A, mLastHurrahAlpha);
				mLastHurrahUpdates = 0;
			}
			if (specialCount == 0)
			{
				base.GameOver(false);
				if ((double)mLastHurrahAlpha > 0.0)
				{
					mGameOverCount = GlobalMembers.M(200);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_LAST_HURRAH_ALPHA_B, mLastHurrahAlpha);
				}
			}
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			base.DrawOverlay(g, thePriority);
			DrawMultiplierLarge(g);
			if ((double)mLastHurrahAlpha != 0.0)
			{
				g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
				g.SetColor(Color.FAlpha((float)((double)mLastHurrahAlpha * (double)GetPieceAlpha())));
				float lhScale = 1.25f + (float)System.Math.Sin((float)mLastHurrahUpdates * 0.06f) * 0.15f;
				int lhYOff = 5;
				string lhText = GlobalMembers._ID("Last Hurrah", 482);
				int lhX = GetBoardCenterX() - g.StringWidth(lhText) / 3;
				int lhY = ConstantsWP.SPEEDBOARD_LAST_HURRAH_Y - lhYOff;
				float origScaleX = g.mScaleX, origScaleY = g.mScaleY, origScaleOrigX = g.mScaleOrigX, origScaleOrigY = g.mScaleOrigY;
				g.SetScale(lhScale, lhScale, lhX, lhY);
				g.WriteString(lhText, lhX, lhY);
				g.SetScale(origScaleX, origScaleY, origScaleOrigX, origScaleOrigY);
			}
		}

		public override void GameOverExit()
		{
			SubmitHighscore();
			GlobalMembers.gApp.DoGameDetailMenu(GameMode.MODE_BLITZ, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_POST_GAME);
		}

		public override void SubmitHighscore()
		{
			HighScoreTable orCreateTable = GlobalMembers.gApp.mHighScoreMgr.GetOrCreateTable(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_BLITZ));
			if (orCreateTable.Submit(GlobalMembers.gApp.mProfile.mProfileName, mPoints, GlobalMembers.gApp.mProfile.GetProfilePictureId()))
			{
				GlobalMembers.gApp.SaveHighscores();
			}
		}
		
		public override void PlayMenuMusic(bool isRestart = false)
		{
			CustomBassMusicInterface theMusicInterface = (CustomBassMusicInterface)GlobalMembers.gApp.mMusicInterface;
			if (isRestart || (theMusicInterface.mSongName != GetMusicName() &&
			                  theMusicInterface.mSongName != $"{GetMusicName()}_lose"))
			{
				theMusicInterface.QueueEvent("FadeOut", theMusicInterface.mSongName, false);
				theMusicInterface.QueueEvent("Play", GetMusicName(), true);
			}
		}

		public override Image GetMultiplierImage()
		{
			return GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_MULTIPLIER;
		}

		public override int GetMultiplierImageX()
		{
			return (int)GlobalMembers.IMG_SXOFS(899);
		}

		public override int GetMultiplierImageY()
		{
			return (int)GlobalMembers.IMG_SYOFS(899);
		}
	}
}
