using System;
using System.Collections.Generic;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Sound;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class SpeedBoard : Board
	{
		private bool mDrawingOverlay;

		public int mPreHurrahPoints;

		public int mSpeedTier;

		public int mPointsGoal;

		public int mPrevPointsGoal;

		public int mPMDropLevel;

		public bool mUsePM;

		public bool mUseCheckpoints;

		public bool mDidTimeUp;

		public EffectsManager mTimeFXManager;

		public SoundInstance mHumSoundEffect;

		public int mTimeUpCount;

		public int mTotalGameTicks;

		public int mBonusTime;

		public int mTotalBonusTime;

		public float mBonusTimeDisp;

		public int mTimedPenaltyAmnesty;

		public float mTimedPenaltyVel;

		public float mTimedPenaltyAccel;

		public float mTimedPenaltyJerk;

		public float mTimedLevelBonus;

		public double mBonusPenalty;

		public float mPointMultiplierStart;

		public float mAddPointMultiplierPerLevel;

		public bool mReadyForDrop;

		public int mWantGemsCleared;

		public int mDropGameTick;

		public int mTimeStart;

		public int mTimeChange;

		public int mMaxTicksLeft;

		public int mPointsGoalStart;

		public int mAddPointsGoalPerLevel;

		public double mPointsGoalAddPower;

		public QuasiRandom m5SecChance = new QuasiRandom();

		public QuasiRandom m10SecChance = new QuasiRandom();

		public float m5SecChanceDec;

		public float m10SecChanceDec;

		public float mTimeStep;

		public float mLevelTimeStep;

		public float mGameTicksF;

		public float mTimeScaleOverride;

		public CurvedVal mCollectorExtendPct = new CurvedVal();

		public CurvedVal mCollectedTimeAlpha = new CurvedVal();

		public CurvedVal mLastHurrahAlpha = new CurvedVal();

		public int mLastHurrahUpdates;

		public float mPanicScalePct;

		public float mCurTempo;

		public SpeedBoard()
		{
			mShowPointMultiplier = true;
			mTimeFXManager = new EffectsManager(this);
			mLevelBarSizeBias = ConstantsWP.SPEEDBOARD_LEVELBAR_SIZE_BIAS;
			mHumSoundEffect = GlobalMembers.gApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_LIGHTNING_HUMLOOP);
			if (mHumSoundEffect != null && GlobalMembers.gApp.mSfxVolume > 0.0)
			{
				mHumSoundEffect.SetVolume(0.0);
			}
			mUiConfig = EUIConfig.eUIConfig_StandardNoReplay;
			mDrawingOverlay = false;
			mCurTempo = 0f;
			mParams["Title"] = "Lightning";
		}

		public override void Dispose()
		{
			mTimeFXManager = null;
			if (mHumSoundEffect != null)
			{
				mHumSoundEffect.Release();
			}
			base.Dispose();
		}

		public override string GetSavedGameName()
		{
			return "speed.sav";
		}

		public override string GetMusicName()
		{
			return "Speed";
		}

		public override bool SaveGameExtra(Serialiser theBuffer)
		{
			int chunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkSpeedBoard);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoard5SecChance, m5SecChance.mChance);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoard5SecChanceStep, m5SecChance.mSteps);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoard5SecChanceLastRoll, m5SecChance.mLastRoll);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoard10SecChance, m10SecChance.mChance);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoard10SecChanceStep, m10SecChance.mSteps);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoard10SecChanceLastRoll, m10SecChance.mLastRoll);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoardBonusTime, mBonusTime);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoardBonusTimeDisp, mBonusTimeDisp);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoardGameTicks, mGameTicksF);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoardCollectorExtendPct, mCollectorExtendPct);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoardPanicScalePct, mPanicScalePct);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoardTimeScaleOverride, mTimeScaleOverride);
			theBuffer.WriteValuePair(Serialiser.PairID.SpeedBoardTotalBonusTime, mTotalBonusTime);
			theBuffer.FinalizeGameChunkHeader(chunkBeginLoc);
			return !mDidTimeUp;
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			int chunkBeginPos = 0;
			GameChunkHeader header = new GameChunkHeader();
			if (theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkSpeedBoard, header, out chunkBeginPos))
			{
				theBuffer.ReadValuePair(out m5SecChance.mChance);
				theBuffer.ReadValuePair(out m5SecChance.mSteps);
				theBuffer.ReadValuePair(out m5SecChance.mLastRoll);
				theBuffer.ReadValuePair(out m10SecChance.mChance);
				theBuffer.ReadValuePair(out m10SecChance.mSteps);
				theBuffer.ReadValuePair(out m10SecChance.mLastRoll);
				theBuffer.ReadValuePair(out mBonusTime);
				theBuffer.ReadValuePair(out mBonusTimeDisp);
				theBuffer.ReadValuePair(out mGameTicksF);
				theBuffer.ReadValuePair(out mCollectorExtendPct);
				theBuffer.ReadValuePair(out mPanicScalePct);
				theBuffer.ReadValuePair(out mTimeScaleOverride);
				theBuffer.ReadValuePair(out mTotalBonusTime);
			}
		}

		public override int GetHintTime()
		{
			return 5;
		}

		public override void Init()
		{
			base.Init();
			mPreHurrahPoints = 0;
			mSpeedTier = 0;
			mPrevPointsGoal = 0;
			mPMDropLevel = 0;
			mPointsGoal = 2500;
			mDoThirtySecondVoice = false;
			mUsePM = false;
			mDidTimeUp = false;
			mTimeUpCount = 0;
			mBonusTime = 0;
			mTotalBonusTime = 0;
			mBonusTimeDisp = 0f;
			mTotalGameTicks = 0;
			mReadyForDrop = true;
			mWantGemsCleared = 0;
			mDropGameTick = 0;
			mBonusPenalty = 0.0;
			mTimedPenaltyAmnesty = GlobalMembers.M(250);
			mUsePM = SexyFramework.GlobalMembers.sexyatoi(mParams, "UsePM") != 0;
			mPointsGoalStart = (mPointsGoal = SexyFramework.GlobalMembers.sexyatoi(mParams, "PointsGoalStart"));
			mAddPointsGoalPerLevel = SexyFramework.GlobalMembers.sexyatoi(mParams, "AddPointsGoalPerLevel");
			mPointsGoalAddPower = SexyFramework.GlobalMembers.sexyatof(mParams, "PointsGoalAddPower");
			mTimeStart = SexyFramework.GlobalMembers.sexyatoi(mParams, "TimeStart");
			mTimeChange = SexyFramework.GlobalMembers.sexyatoi(mParams, "TimeChange");
			m5SecChance.Init(mParams["5SecChanceCurve"]);
			m10SecChance.Init(mParams["10SecChanceCurve"]);
			m5SecChanceDec = SexyFramework.GlobalMembers.sexyatof(mParams, "5SecChanceDec");
			m10SecChanceDec = SexyFramework.GlobalMembers.sexyatof(mParams, "10SecChanceDec");
			mTimedPenaltyVel = SexyFramework.GlobalMembers.sexyatof(mParams, "TimedPenaltyVelocity");
			mTimedPenaltyAccel = SexyFramework.GlobalMembers.sexyatof(mParams, "TimedPenaltyAccel");
			mTimedPenaltyJerk = SexyFramework.GlobalMembers.sexyatof(mParams, "TimedPenaltyJerk");
			mTimedLevelBonus = SexyFramework.GlobalMembers.sexyatof(mParams, "TimedLevelBonus");
			mTimeStep = SexyFramework.GlobalMembers.sexyatof(mParams, "TimeStep");
			mLevelTimeStep = SexyFramework.GlobalMembers.sexyatof(mParams, "LevelTimeStep");
			mPointMultiplierStart = SexyFramework.GlobalMembers.sexyatof(mParams, "PointMultiplierStart");
			mAddPointMultiplierPerLevel = SexyFramework.GlobalMembers.sexyatof(mParams, "AddPointMultiplierPerLevel");
			mUseCheckpoints = mPointsGoalStart > 0 && mTimeStart > 0;
			mMaxTicksLeft = 6000;
			mPanicScalePct = 0f;
			mGameTicksF = 0f;
			mTimeScaleOverride = 0f;
			mCurTempo = 0f;
			mCollectedTimeAlpha.SetConstant(1.0);
			mCollectorExtendPct.SetConstant(0.0);
			mLastHurrahAlpha.SetConstant(0.0);
		}

		public override int GetBoardY()
		{
			return GlobalMembers.RS(ConstantsWP.SPEEDBOARD_BOARD_Y);
		}

		public override void GameOverExit()
		{
			SubmitHighscore();
			GlobalMembers.gApp.DoGameDetailMenu(GameMode.MODE_LIGHTNING, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_POST_GAME);
		}

		public override void GameOverAnnounce()
		{
			new Announcement(this, GlobalMembers._ID("TIME UP", 478));
			GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_TIMEUP);
		}

		public override void PieceTallied(Piece thePiece)
		{
			if (thePiece.IsFlagSet(131072u))
			{
				AddToStat(34, 1, thePiece.mMoveCreditId);
				int mBonusTime2 = mBonusTime;
				mBonusTime += thePiece.mCounter;
				mTotalBonusTime += thePiece.mCounter;
				SpeedCollectEffect speedCollectEffect = SpeedCollectEffect.alloc(this, new Point((int)thePiece.CX(), (int)thePiece.CY()), new Point(ConstantsWP.SPEEDBOARD_COLLECT_EFFECT_DEST_X, ConstantsWP.SPEEDBOARD_COLLECT_EFFECT_DEST_Y), Res.GetImageByID((ResourceId)(1147 + SexyFramework.Common.Rand() % GlobalMembers.M(9))), thePiece.mCounter, (float)GlobalMembers.MS(1.0));
				mTimeFXManager.AddEffect(speedCollectEffect);
				speedCollectEffect.Init(thePiece);
				if (!thePiece.IsFlagSet(4u))
				{
					thePiece.mAlpha.SetConstant(0.0);
				}
				else
				{
					thePiece.ClearFlag(131072u);
				}
				if (thePiece.mCounter == 5)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TIMEBONUS_5, (int)((double)GetPanPosition(thePiece) * GlobalMembers.M(0.5)), 1.0, GlobalMembers.M(0.1) * (double)mBonusTime);
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TIMEBONUS_10, (int)((double)GetPanPosition(thePiece) * GlobalMembers.M(0.5)), 1.0, GlobalMembers.M(0.1) * (double)mBonusTime);
				}
				int num = Math.Max(0, mBonusTime - 60);
				if (num > 0)
				{
					AddPoints((int)thePiece.CX(), (int)thePiece.CY(), thePiece.mCounter * GlobalMembers.M(50), GlobalMembers.gGemColors[thePiece.mColor], (uint)thePiece.mMatchId, true, true, thePiece.mMoveCreditId);
				}
				string str = $"+{thePiece.mCounter:d} sec";
				str = GlobalMembers._ID(str, 479);
				Points points = new Points(GlobalMembers.gApp, GlobalMembersResources.FONT_HEADER, str, (int)thePiece.CX(), (int)thePiece.CY(), GlobalMembers.M(1f), 0, GlobalMembers.gGemColors[thePiece.mColor], GlobalMembers.M(-1));
				points.mDestScale = GlobalMembers.M(1.5f);
				points.mScaleDifMult = GlobalMembers.M(0.2f);
				points.mScaleDampening = GlobalMembers.M(0.8f);
				points.mDY *= GlobalMembers.M(0.2f);
				if (thePiece.IsFlagSet(4u))
				{
					thePiece.mCounter = 0;
				}
			}
			base.PieceTallied(thePiece);
		}

		public override Rect GetLevelBarRect()
		{
			return new Rect((int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME_ID)), GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME.mWidth, GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME.mHeight);
		}

		public override Rect GetCountdownBarRect()
		{
			return new Rect((int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_BACK_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_BACK_ID)), GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_BACK.mWidth, GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_BACK.mHeight);
		}

		public override int GetTimeDrawX()
		{
			Rect levelBarRect = GetLevelBarRect();
			levelBarRect.mX += GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_TIMER.mWidth / 2;
			levelBarRect.mWidth -= GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_TIMER.mWidth + ConstantsWP.SPEEDBOARD_TIMEDRAW_X_OFFSET;
			return levelBarRect.mX + (int)((float)levelBarRect.mWidth * mCountdownBarPct);
		}

		public override bool CanTimeUp()
		{
			if (mBonusTime == 0)
			{
				return base.CanTimeUp();
			}
			return true;
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
			int num = GlobalMembers.M(250);
			int val = (int)Math.Min((float)timeLimit * 100f, Math.Max(0f, (float)timeLimit * 100f - Math.Max(0f, mGameTicksF - (float)num)));
			return Math.Min(mMaxTicksLeft, val);
		}

		public override bool WantsTutorial(int theTutorialFlag)
		{
			return base.WantsTutorial(theTutorialFlag);
		}

		public override int GetTimeLimit()
		{
			return 60;
		}

		public override int GetLevelPoints()
		{
			return mPointsGoalStart + mAddPointsGoalPerLevel * mSpeedTier;
		}

		public override int GetLevelPointsTotal()
		{
			return mLevelPointsTotal - (int)mBonusPenalty;
		}

		public override void LevelUp()
		{
			mSpeedTier++;
			mBonusPenalty = 0.0;
			mLevelPointsTotal = 0;
			mTimedPenaltyAmnesty = GlobalMembers.M(500);
			double num = mTimedLevelBonus;
			mTimedPenaltyVel = (float)Math.Max(0.0, (double)mTimedPenaltyVel - (double)mTimedPenaltyAccel * num);
			mTimedPenaltyAccel = (float)Math.Max(0.0, (double)mTimedPenaltyAccel - (double)mTimedPenaltyJerk * num);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BACKGROUND_CHANGE);
		}

		public override int WantExpandedTopWidget()
		{
			return 1;
		}

		public override string GetTopWidgetButtonText()
		{
			return base.GetTopWidgetButtonText();
		}

		public override float GetModePointMultiplier()
		{
			return 5f;
		}

		public override float GetRankPointMultiplier()
		{
			return 5.6666665f;
		}

		public override void GameOver()
		{
			GameOver(false);
		}

		public override void GameOver(bool visible)
		{
			if (mWantLevelup || mHyperspace != null || !mTimeFXManager.IsEmpty())
			{
				return;
			}
			if (mBonusTime == 0 && Common.size(mPointsBreakdown) <= mPointMultiplier)
			{
				AddPointBreakdownSection();
			}
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					if (piece != null && piece.IsFlagSet(131072u))
					{
						if (mBonusTime == 0)
						{
							Points points = AddPoints((int)piece.CX(), (int)piece.CY(), piece.mCounter * GlobalMembers.M(50), GlobalMembers.gGemColors[piece.mColor], (uint)piece.mMatchId, true, true, piece.mMoveCreditId);
							points.mTimer *= GlobalMembers.M(1.5f);
						}
						else if (piece.mCounter >= 10)
						{
							Laserify(piece);
						}
						else
						{
							Flamify(piece);
						}
						piece.ClearFlag(131072u);
						piece.mCounter = 0;
					}
				}
			}
			if (mBonusTime > 0)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_LIGHTNING_ENERGIZE);
				GlobalMembers.gApp.mCurveValCache.GetCurvedValMult(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_COLLECTOR_EXTEND_PCT_A, mCollectorExtendPct);
				m5SecChance.Step(mLevelTimeStep);
				m10SecChance.Step(mLevelTimeStep);
				m5SecChance.mChance.IncInVal();
				m10SecChance.mChance.IncInVal();
				mTimeExpired = false;
				GlobalMembers.gApp.PlaySample(Res.GetSoundByID((ResourceId)(1577 + Math.Min(3, mPointMultiplier - 1))));
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_PREV_POINT_MULT_ALPHA, mPrevPointMultAlpha);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_POS_PCT_2, mPointMultPosPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_SCALE_2, mPointMultScale, mPointMultPosPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_ALPHA_2, mPointMultAlpha, mPointMultPosPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_Y_ADD_2, mPointMultYAdd, mPointMultPosPct);
				mPointMultTextMorph.SetConstant(0.0);
				mPointMultiplier++;
				AddPointBreakdownSection();
				mMaxTicksLeft = mBonusTime * 100;
				mGameTicks = Math.Max(0, (60 - mBonusTime) * 100 + GlobalMembers.M(0));
				mGameTicksF = mGameTicks;
				mBonusTime = 0;
				mTimeScaleOverride = 0f;
				LightningBarFillEffect lightningBarFillEffect = LightningBarFillEffect.alloc();
				lightningBarFillEffect.mOverlay = true;
				mPostFXManager.AddEffect(lightningBarFillEffect);
			}
			else
			{
				if (mSpeedBonusFlameModePct > 0f)
				{
					return;
				}
				int num = 0;
				mCursorSelectPos = new Point(-1, -1);
				int num2 = 0;
				if (!mDidTimeUp)
				{
					mPreHurrahPoints = mPoints;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_COLLECTED_TIME_ALPHA, mCollectedTimeAlpha);
					// GlobalMembers.gApp.mMusic.PlaySongNoDelay(12, false);
					(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("FadeOut", (GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).mSongName, false);
					(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("Play", "Speed_lose", true);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BOMB_EXPLODE, 0, GlobalMembers.M(1), GlobalMembers.M(-2.0));
					GlobalMembers.gApp.PlayVoice(new VoicePlayArgs(GlobalMembersResourcesWP.SOUND_VOICE_TIMEUP, 0, 1.0, -2, new SoundPlayConditionWaitUpdates(GlobalMembersResourcesWP.SOUND_BOMB_EXPLODE)));
					mDidTimeUp = true;
					new Announcement(this, GlobalMembers._ID("TIME UP", 480));
				}
				if (mSpeedBonusCount > 0)
				{
					EndSpeedBonus();
				}
				bool flag = false;
				for (int k = 0; k < 8; k++)
				{
					for (int l = 0; l < 8; l++)
					{
						Piece piece2 = mBoard[k, l];
						if (piece2 == null)
						{
							continue;
						}
						flag |= piece2.IsFlagSet(1024u);
						if (piece2.IsFlagSet(1u) || piece2.IsFlagSet(2u) || piece2.IsFlagSet(4u) || piece2.IsFlagSet(16u) || piece2.IsFlagSet(2048u) || piece2.IsFlagSet(4096u) || piece2.IsFlagSet(524288u) || piece2.IsFlagSet(131072u))
						{
							if (piece2.IsFlagSet(1024u))
							{
								piece2.mDestructing = true;
							}
							if (mTimeUpCount == 0)
							{
								piece2.mExplodeDelay = GlobalMembers.M(175) + num2 * GlobalMembers.M(25);
							}
							else
							{
								piece2.mExplodeDelay = GlobalMembers.M(25) + num2 * GlobalMembers.M(25);
							}
							num2++;
							num++;
						}
					}
				}
				if (num2 == 0)
				{
					for (int m = 0; m < 8; m++)
					{
						for (int n = 0; n < 8; n++)
						{
							Piece piece3 = mBoard[m, n];
							if (piece3 != null && piece3.IsFlagSet(1024u) && !piece3.mTallied)
							{
								if (piece3.IsFlagSet(1024u))
								{
									piece3.mDestructing = true;
								}
								TallyPiece(piece3, true);
								piece3.mAlpha.SetConstant(1.0);
								piece3.ClearFlag(1024u);
								num2++;
							}
						}
					}
				}
				if (num2 > 0 && (double)mLastHurrahAlpha == 0.0)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_LAST_HURRAH_ALPHA_A, mLastHurrahAlpha);
					mLastHurrahUpdates = 0;
				}
				if (num2 == 0)
				{
					base.GameOver(false);
					if ((double)mLastHurrahAlpha > 0.0)
					{
						mGameOverCount = GlobalMembers.M(200);
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_LAST_HURRAH_ALPHA_B, mLastHurrahAlpha);
					}
				}
			}
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
			return base.AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, thePointType);
		}

		public override bool WantSpecialPiece(List<Piece> thePieceVector)
		{
			if (mUsePM)
			{
				if (mPMDropLevel < mSpeedTier)
				{
					return mSpeedTier < 8;
				}
				return false;
			}
			if (mTimeStart == 0)
			{
				if (mPMDropLevel < mSpeedTier)
				{
					return mSpeedTier < 8;
				}
				return false;
			}
			bool flag = false;
			if (mReadyForDrop && mWantGemsCleared != 0 && !mDidTimeUp && flag)
			{
				return true;
			}
			return false;
		}

		public override float GetLevelPct()
		{
			int levelPoints = GetLevelPoints();
			if (levelPoints > 0)
			{
				int levelPointsTotal = GetLevelPointsTotal();
				float num = Math.Min(1f, Math.Max(0f, 0.5f + (float)levelPointsTotal / (float)levelPoints * 0.5f));
				if (mDidTimeUp)
				{
					num = 0f;
				}
				if (num <= 0f && IsBoardStill() && Common.size(mDeferredTutorialVector) == 0 && mGameOverCount == 0)
				{
					mTimeExpired = true;
					GameOver();
				}
				int num2 = (int)(num * (float)GlobalMembers.M(4000));
				int num3 = GlobalMembers.M(35) + (int)((float)num2 * GlobalMembers.M(0.1f));
				if (mUpdateCnt - mLastWarningTick >= num3 && num2 > 0 && num2 <= GlobalMembers.M(1000) && WantWarningGlow())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COUNTDOWN_WARNING, 0, Math.Min(1.0, GlobalMembers.M(0.5) - (double)((float)num2 * GlobalMembers.M(0.0005f))));
					mLastWarningTick = mUpdateCnt;
				}
				return num;
			}
			int timeLimit = GetTimeLimit();
			float num4 = 0f;
			bool flag = mUpdateCnt % 20 == 0;
			if (timeLimit != 0)
			{
				num4 = Math.Max(0f, (float)GetTicksLeft() / ((float)timeLimit * 100f));
				if (num4 <= 0f && IsBoardStill() && Common.size(mDeferredTutorialVector) == 0 && mGameOverCount == 0)
				{
					mTimeExpired = true;
					GameOver();
				}
				int ticksLeft = GetTicksLeft();
				int num5 = GlobalMembers.M(35) + (int)((float)ticksLeft * GlobalMembers.M(0.1f));
				if (mUseCheckpoints)
				{
					if (mUpdateCnt - mLastWarningTick >= num5 && ticksLeft > 0 && ticksLeft <= GlobalMembers.M(1000) && WantWarningGlow())
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COUNTDOWN_WARNING, 0, Math.Min(1.0, GlobalMembers.M(0.5) - (double)((float)ticksLeft * GlobalMembers.M(0.0005f))));
						mLastWarningTick = mUpdateCnt;
					}
				}
				else if (!mUserPaused && mUpdateCnt - mLastWarningTick >= num5 && ticksLeft > 0 && WantWarningGlow())
				{
					int num6 = ((GetTimeLimit() > 60) ? 1500 : 1000);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COUNTDOWN_WARNING, 0, Math.Min(1.0, GlobalMembers.M(0.5) - (double)((float)ticksLeft / (float)num6 / 2f)));
					mLastWarningTick = mUpdateCnt;
				}
				if (ticksLeft == 3000 && mDoThirtySecondVoice)
				{
					flag = true;
					GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_THIRTYSECONDS);
					if (mInUReplay)
					{
						mUReplayTicksLeft--;
					}
				}
			}
			if (mUseCheckpoints)
			{
				num4 = (float)(mPoints - mPrevPointsGoal) / (float)(mPointsGoal - mPrevPointsGoal);
			}
			if (flag && WriteUReplayCmd(7))
			{
				mUReplayBuffer.WriteShort((short)GetTicksLeft());
			}
			return num4;
		}

		public override bool DropSpecialPiece(List<Piece> thePieceVector)
		{
			if (mUsePM)
			{
				int index = (int)mRand.Next() % Common.size(thePieceVector);
				for (int i = 0; i < 7; i++)
				{
					thePieceVector[index].mColor = (int)mRand.Next() % 7;
					int num = 0;
					for (int j = 0; j < 8; j++)
					{
						for (int k = 0; k < 8; k++)
						{
							Piece piece = mBoard[j, k];
							if (piece != null && piece.GetScreenY() > 0f && piece.mColor == thePieceVector[index].mColor)
							{
								num++;
							}
						}
					}
					if (num > 3)
					{
						break;
					}
				}
				thePieceVector[index].SetFlag(16u);
				if (WantsTutorial(4))
				{
					DeferTutorialDialog(4, thePieceVector[index]);
				}
				mPMDropLevel++;
			}
			else
			{
				int index2 = (int)mRand.Next() % Common.size(thePieceVector);
				for (int l = 0; l < 7; l++)
				{
					thePieceVector[index2].mColor = (int)mRand.Next() % 7;
					int num2 = 0;
					for (int m = 0; m < 8; m++)
					{
						for (int n = 0; n < 8; n++)
						{
							Piece piece2 = mBoard[m, n];
							if (piece2 != null && piece2.mY > 0f && piece2.mColor == thePieceVector[index2].mColor)
							{
								num2++;
							}
						}
					}
					if (num2 > 3)
					{
						break;
					}
				}
				Blastify(thePieceVector[index2]);
				mDropGameTick = mGameTicks;
				mReadyForDrop = false;
				mWantGemsCleared = 0;
				mPMDropLevel++;
			}
			return true;
		}

		public override bool PiecesDropped(List<Piece> thePieceVector)
		{
			int num = 0;
			if (mGameTicks > 100 && !mTimeExpired && GetTicksLeft() > 0)
			{
				for (int i = 0; i < Common.size(thePieceVector); i++)
				{
					m5SecChance.Step();
					m10SecChance.Step();
					if (m10SecChance.Check((float)(mRand.Next() % 100000) / 100000f))
					{
						num = 10;
					}
					else if (m5SecChance.Check((float)(mRand.Next() % 100000) / 100000f))
					{
						num = 5;
					}
				}
			}
			if (num > 0)
			{
				int index = (int)mRand.Next() % Common.size(thePieceVector);
				Piece piece = thePieceVector[index];
				if (piece.mFlags == 0)
				{
					piece.SetFlag(131072u);
					piece.mCounter = num;
					StartTimeBonusEffect(piece);
					if (WantsTutorial(9))
					{
						DeferTutorialDialog(9, piece);
					}
					if (piece.mCounter == 5)
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TIMEBONUS_APPEARS_5, GetPanPosition(piece));
					}
					else
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TIMEBONUS_APPEARS_10, GetPanPosition(piece));
					}
				}
			}
			return true;
		}

		public void TimeCollected(int theTimeCollected)
		{
			if (theTimeCollected <= 5)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_LIGHTNING_TUBE_FILL_5);
			}
			else
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_LIGHTNING_TUBE_FILL_10);
			}
			double mOutMin = mCollectorExtendPct;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_COLLECTOR_EXTEND_PCT_B, mCollectorExtendPct);
			mCollectorExtendPct.mOutMin = mOutMin;
			mCollectorExtendPct.mOutMax = Math.Min(1.0, (double)mBonusTime / 60.0);
		}

		public override void Update()
		{
			int num = mGameTicks;
			mLastHurrahAlpha.IncInVal();
			mLastHurrahUpdates++;
			base.Update();
			if (mUpdateCnt % 10 == 0 && mHumSoundEffect != null)
			{
				mHumSoundEffect.SetVolume((double)mCollectorExtendPct * GlobalMembers.M(0.1));
			}
			if (mGameTicks != num)
			{
				if (mTimeScaleOverride == 0f)
				{
					float num2 = Math.Min(1f, GlobalMembers.M(0.7f) + (float)GetTicksLeft() / GlobalMembers.M(600f) * GlobalMembers.M(0.3f));
					mGameTicksF += num2;
				}
				else
				{
					mGameTicksF += mTimeScaleOverride;
				}
				m5SecChance.Step(mTimeStep / 100f);
				m10SecChance.Step(mTimeStep / 100f);
				mTotalGameTicks++;
				if (!WantWarningGlow())
				{
					int ticksLeft = GetTicksLeft();
					if (ticksLeft % 100 == 0 && ticksLeft > 0 && ticksLeft <= GlobalMembers.M(800) && ticksLeft != mMaxTicksLeft)
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TICK, 0, GlobalMembers.M(0.3), (ticksLeft / 100 % 2 == 0) ? 0f : GlobalMembers.M(-5f));
					}
				}
				if (mGameTicks % GlobalMembers.M(150) == 0)
				{
					for (int i = 0; i < 8; i++)
					{
						for (int j = 0; j < 8; j++)
						{
							Piece piece = mBoard[i, j];
							if (piece != null && piece.IsFlagSet(262144u))
							{
								piece.mCounter--;
								if (piece.mCounter <= 0)
								{
									piece.ClearFlag(131072u);
									piece.ClearFlag(262144u);
								}
							}
						}
					}
				}
			}
			if (!mDidTimeUp)
			{
				mPreHurrahPoints = mPoints;
			}
			if (mDidTimeUp)
			{
				mTimeUpCount++;
			}
			float num3 = Math.Min(GetLevelPct() + 0.65f, 1f);
			mBonusTimeDisp += ((float)mBonusTime - mBonusTimeDisp) / 50f;
			if (mTimedPenaltyAmnesty > 0)
			{
				mTimedPenaltyAmnesty--;
			}
			else
			{
				mBonusPenalty += (double)(mTimedPenaltyVel * num3) / 100.0;
				mTimedPenaltyVel += mTimedPenaltyAccel * num3 / 100f;
				mTimedPenaltyAccel += mTimedPenaltyJerk * num3 / 100f;
			}
			if (mWantGemsCleared == 0)
			{
				mWantGemsCleared = GlobalMembers.M(20);
			}
			if (mGameTicks % GlobalMembers.M(400) == 0 && num != mGameTicks)
			{
				mWantGemsCleared = Math.Max(5, mWantGemsCleared - (int)mRand.Next() % 5);
			}
			if (mUseCheckpoints && mPoints > mPointsGoal)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BACKGROUND_CHANGE);
				mSpeedTier++;
				mPrevPointsGoal = mPointsGoal;
				if (mUsePM)
				{
					mPointsGoal += (int)((double)mPointsGoalStart + (double)mAddPointsGoalPerLevel * Math.Pow(mSpeedTier, mPointsGoalAddPower)) * (mSpeedTier + 1);
				}
				else
				{
					mPointsGoal += (int)((double)mPointsGoalStart + (double)mAddPointsGoalPerLevel * Math.Pow(mSpeedTier, mPointsGoalAddPower));
				}
				mGameTicks = 0;
			}
			Math.Pow(mCollectorExtendPct, GlobalMembers.M(0.7));
			mTimeFXManager.Update();
			if (GetTicksLeft() > 1250 || mBonusTime > 0)
			{
				mPanicScalePct = Math.Max(0f, mPanicScalePct - GlobalMembers.M(0.01f));
			}
			else if (mBonusTime == 0)
			{
				mPanicScalePct = Math.Min(1f, mPanicScalePct + GlobalMembers.M(0.01f));
			}
			CustomBassMusicInterface theMusicInterface = (CustomBassMusicInterface)GlobalMembers.gApp.mMusicInterface;
			if (theMusicInterface.mSongName == "Speed")
			{
				theMusicInterface.SetTempo("Speed", 125 + (int)((mPointMultiplier - 1) * 3.5f));
			}

			SongInfo speedSongInfo = theMusicInterface.FindSong("Speed");
			if (speedSongInfo != null)
			{
				for (int trackId = 16; trackId < speedSongInfo.mTracks.Count; trackId++)
				{
					speedSongInfo.mTracks[trackId].mVolume.SetInVal(mPanicScalePct);
				}

				theMusicInterface.mForceParamUpdate = true;
			}
			
			if (mTimeUpCount > 0 && mTimeUpCount < GlobalMembers.M(100) && mUpdateCnt % GlobalMembers.M(3) == 0)
			{
				mX = (int)((double)(GlobalMembersUtils.GetRandFloat() * (float)(GlobalMembers.M(100) - mTimeUpCount) / (float)GlobalMembers.M(100)) * GlobalMembers.MS(12.0));
				mY = (int)((double)(GlobalMembersUtils.GetRandFloat() * (float)(GlobalMembers.M(100) - mTimeUpCount) / (float)GlobalMembers.M(100)) * GlobalMembers.MS(12.0));
			}
			if (mSpeedBonusFlameModePct > 0f)
			{
				if (mTimeScaleOverride == 0f)
				{
					int ticksLeft2 = GetTicksLeft();
					int num4 = (int)(800f * mSpeedBonusFlameModePct);
					if ((double)ticksLeft2 * GlobalMembers.M(1.3) < (double)num4)
					{
						mTimeScaleOverride = (float)ticksLeft2 / (float)num4;
					}
				}
			}
			else
			{
				mTimeScaleOverride = 0f;
			}
		}

		public override void RefreshUI()
		{
			base.RefreshUI();
			mHintButton.SetBorderGlow(true);
			mHintButton.Resize(ConstantsWP.BOARD_UI_HINT_BTN_X, ConstantsWP.SPEEDBOARD_HINT_BTN_Y, ConstantsWP.BOARD_UI_HINT_BTN_WIDTH, 0);
		}

		public override void DrawLevelBar(Graphics g)
		{
		}

		public override void KeyChar(char theChar)
		{
			base.KeyChar(theChar);
		}

		public override void DrawScore(Graphics g)
		{
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			string theString = SexyFramework.Common.CommaSeperate(mDispPoints);
			int num = mWidth / 2;
			int num2 = (int)((GlobalMembers.IMG_SYOFS(897) + (float)GlobalMembersResources.FONT_DIALOG.mAscent) / 2f - (float)mTransScoreOffsetY);
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Color.White);
			float mScaleX = g.mScaleX;
			float mScaleY = g.mScaleY;
			g.SetScale(ConstantsWP.BOARD_LEVEL_SCORE_SCALE, ConstantsWP.BOARD_LEVEL_SCORE_SCALE, num, num2 - g.GetFont().GetAscent() / 2);
			g.WriteString(theString, num, num2);
			g.mScaleX = mScaleX;
			g.mScaleY = mScaleY;
		}

		public override void DrawFrame(Graphics g)
		{
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_BOARD_SEPARATOR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_BOARD_SEPARATOR_FRAME_ID) + (float)mTransBoardOffsetX), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_BOARD_SEPARATOR_FRAME_ID) - (float)mTransBoardOffsetY));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME_ID) + (float)mTransBoardOffsetX), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME_ID) - (float)mTransBoardOffsetY));
			Rect countdownBarRect = GetCountdownBarRect();
			int num = countdownBarRect.mY + countdownBarRect.mHeight / 2;
			if (mDrawingOverlay)
			{
				Color color = g.mPushedColorVector[g.mPushedColorVector.Count - 1];
				g.PopColorMult();
				g.SetColorizeImages(false);
				g.SetDrawMode(Graphics.DrawMode.Normal);
				Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_TIMER, GetTimeDrawX(), num);
				g.SetColorizeImages(true);
				g.SetColor(color);
				g.PushColorMult();
				g.SetDrawMode(Graphics.DrawMode.Additive);
			}
			Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_TIMER, GetTimeDrawX(), num);
			DrawFrame2(g);
		}

		public void DrawFrame2(Graphics g)
		{
			g.SetColorizeImages(true);
			g.SetColor(new Color(255, 255, 255, (int)(255f * GetBoardAlpha())));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_EXTRA_TIME_METER, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_EXTRA_TIME_METER_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_EXTRA_TIME_METER_ID)));
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			g.SetColor(new Color(255, 255, 255, (int)((double)(255f * GetAlpha()) * (double)mCollectedTimeAlpha)));
			int num = (int)((double)mCollectorExtendPct * 60.0 + 0.5);
			string theString = string.Format(GlobalMembers._ID("+{0:d}:{1:d2}", 481), num / 60, num % 60);
			g.WriteString(theString, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_EXTRA_TIME_METER_ID)) + GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_EXTRA_TIME_METER.mWidth / 2 + ConstantsWP.SPEEDBOARD_EXTRATIME_X_OFFSET, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_EXTRA_TIME_METER_ID)) + GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_EXTRA_TIME_METER.mHeight / 2 + ConstantsWP.SPEEDBOARD_EXTRATIME_Y_OFFSET, 0, 0);
			DrawLevelBar(g);
			if ((double)mLastHurrahAlpha != 0.0)
			{
				g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
				g.SetColor(Color.FAlpha((float)((double)mLastHurrahAlpha * (double)GetPieceAlpha())));
				float aScale = 1.25f + (float)Math.Sin((float)mLastHurrahUpdates * 0.06f) * 0.15f;
				int num2 = 5;
				float origScaleX = g.mScaleX, origScaleY = g.mScaleY, origScaleOrigX = g.mScaleOrigX, origScaleOrigY = g.mScaleOrigY;
				string text = GlobalMembers._ID("Last Hurrah", 482);
				g.SetScale(aScale, aScale, GetBoardCenterX() - g.StringWidth(text) / 3, ConstantsWP.SPEEDBOARD_LAST_HURRAH_Y - num2);
				g.WriteString(text, GetBoardCenterX() - g.StringWidth(text) / 3, (int)(ConstantsWP.SPEEDBOARD_LAST_HURRAH_Y - num2));
				g.SetScale(origScaleX, origScaleY, origScaleOrigX, origScaleOrigY);
			}
			g.SetColor(Color.FAlpha(GetAlpha()));
			DrawSpeedBonus(g);
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			mTimeFXManager.Draw(g);
			base.DrawOverlay(g, thePriority);
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			base.DrawGameElements(g);
		}

		public override void DrawUI(Graphics g)
		{
			DrawMenuWidget(g);
			base.DrawUI(g);
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_Lightning");
		}

		public override void LoadContent(bool threaded)
		{
			base.LoadContent(threaded);
			BejeweledLivePlusApp.LoadContent("GamePlayQuest_Lightning");
			ConfigureBarEmitters();
		}

		public override void SubmitHighscore()
		{
			HighScoreTable orCreateTable = GlobalMembers.gApp.mHighScoreMgr.GetOrCreateTable(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_LIGHTNING));
			if (orCreateTable.Submit(GlobalMembers.gApp.mProfile.mProfileName, mPoints, GlobalMembers.gApp.mProfile.GetProfilePictureId()))
			{
				GlobalMembers.gApp.SaveHighscores();
			}
		}

		public override void DrawCountdownBar(Graphics g)
		{
			if (mOffsetY != 0)
			{
				g.Translate(0, mOffsetY);
			}
			g.SetColorizeImages(true);
			float num = (float)Math.Pow(GetBoardAlpha(), 4.0);
			g.SetColor(new Color(255, 255, 255, (int)(GetBoardAlpha() * (float)GlobalMembers.M(255))));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_BACK, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_BACK_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_BACK_ID)));
			g.SetColor(new Color(GlobalMembers.M(64), GlobalMembers.M(32), GlobalMembers.M(8), (int)(num * (float)GlobalMembers.M(255))));
			if (WantWarningGlow())
			{
				Color warningGlowColor = GetWarningGlowColor();
				if (warningGlowColor.mAlpha > 0)
				{
					g.PushState();
					g.SetDrawMode(Graphics.DrawMode.Additive);
					g.SetColor(warningGlowColor);
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_LIGHTNING_PROGRESS_BAR_FRAME_ID)));
					g.PopState();
				}
			}
			Rect countdownBarRect = GetCountdownBarRect();
			countdownBarRect.mWidth = (int)(mCountdownBarPct * (float)countdownBarRect.mWidth + (float)mLevelBarSizeBias);
			g.FillRect(countdownBarRect);
			if ((double)mLevelBarBonusAlpha > 0.0)
			{
				Rect countdownBarRect2 = GetCountdownBarRect();
				countdownBarRect2.mX -= GlobalMembersResourcesWP.IMAGE_INGAMEUI_LIGHTNING_TIMER.mWidth / 2;
				countdownBarRect2.mWidth = (int)((float)countdownBarRect2.mWidth * GetLevelPct());
				g.SetColor(new Color(GlobalMembers.M(240), GlobalMembers.M(255), 200, (int)((double)mLevelBarBonusAlpha * (double)GlobalMembers.M(255))));
				g.FillRect(countdownBarRect2);
			}
			Graphics3D graphics3D = g.Get3D();
			SexyTransform2D mDrawTransform = mCountdownBarPIEffect.mDrawTransform;
			Rect mClipRect = g.mClipRect;
			if (graphics3D != null)
			{
				countdownBarRect.Scale(mScale, mScale, (int)ConstantsWP.DEVICE_HEIGHT_F, (int)ConstantsWP.DEVICE_WIDTH_F);
				mCountdownBarPIEffect.mDrawTransform.Translate(0f - ConstantsWP.DEVICE_HEIGHT_F, 0f - ConstantsWP.DEVICE_WIDTH_F);
				mCountdownBarPIEffect.mDrawTransform.Scale((float)(double)mScale, (float)(double)mScale);
				mCountdownBarPIEffect.mDrawTransform.Translate(ConstantsWP.DEVICE_HEIGHT_F, ConstantsWP.DEVICE_WIDTH_F);
			}
			g.SetClipRect(countdownBarRect);
			mCountdownBarPIEffect.mColor = new Color(255, 255, 255, (int)(num * (float)GlobalMembers.M(255)));
			mCountdownBarPIEffect.Draw(g);
			mCountdownBarPIEffect.mDrawTransform = mDrawTransform;
			g.SetColor(Color.White);
			g.SetClipRect(mClipRect);
			if (mOffsetY != 0)
			{
				g.Translate(0, -mOffsetY);
			}
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

		public override bool WantsTutorialReplays()
		{
			return false;
		}

		public override int GetTimerYOffset()
		{
			return ConstantsWP.SPEEDBOARD_TIMEDRAW_Y_OFFSET;
		}

		public override void PlayMenuMusic()
		{
			CustomBassMusicInterface theMusicInterface = (CustomBassMusicInterface)GlobalMembers.gApp.mMusicInterface;
			if (theMusicInterface.mSongName != "Speed")
			{
				theMusicInterface.QueueEvent("FadeOut", theMusicInterface.mSongName, false);
				theMusicInterface.QueueEvent("Play", "Speed", true);
			}
			// GlobalMembers.gApp.mMusic.PlaySongNoDelay(11, true);
		}

		public override Color GetWarningGlowColor()
		{
			int ticksLeft = GetTicksLeft();
			int theAlpha;
			if (ticksLeft == 0)
			{
				theAlpha = 127;
			}
			else
			{
				int num = 1000;
				float num2 = (float)(num - ticksLeft) / (float)num;
				theAlpha = (int)((float)((int)(Math.Sin((float)mUpdateCnt * GlobalMembers.M(0.15f)) * 127.0) + 127) * num2 * GetPieceAlpha());
			}
			if (mBonusTime > 0)
			{
				return new Color(255, 255, 0, theAlpha);
			}
			return new Color(255, 0, 0, theAlpha);
		}
		
		public override bool WantWarningGlow()
		{
			return WantWarningGlow(false);
		}

		public new bool WantWarningGlow(bool forSound)
		{
			if (mBonusTime <= 0)
			{
				return base.WantWarningGlow();
			}
			return false;
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
