using System;
using System.Collections.Generic;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Sound;
using SexyFramework.Widget;

namespace BejeweledLivePlus
{
	public class Board : Bej3Widget, Bej3ButtonListener, ButtonListener, DialogListener, SliderListener
	{
		public enum BUTTON
		{
			BUTTON_HINT,
			BUTTON_MENU,
			BUTTON_RESET,
			BUTTON_REPLAY,
			BUTTON_QUEST_HELP,
			BUTTON_ZEN_OPTIONS
		}

		public enum EUIConfig
		{
			eUIConfig_Standard,
			eUIConfig_WithReset,
			eUIConfig_WithResetAndReplay,
			eUIConfig_Quest,
			eUIConfig_StandardNoReplay
		}

		private struct MyPiece
		{
			public Piece piece;

			public float alpha;

			public float scale;

			public int offsX;

			public int offsY;
		}

		public class DeferredSound
		{
			public int mId;

			public int mOnGameTick;

			public double mVolume;
		}

		public class DistortionQuad
		{
			public float x1;

			public float y1;

			public float x2;

			public float y2;

			public DistortionQuad()
			{
			}

			public DistortionQuad(float inX1, float inY1, float inX2, float inY2)
			{
				x1 = inX1;
				y1 = inY1;
				x2 = inX2;
				y2 = inY2;
			}
		}

		private int FUDGE;

		public bool mContentLoaded;

		public bool mShouldUnloadContentWhenDone;

		public CurvedVal mSlidingHUDCurve = new CurvedVal();

		public CurvedVal mTransitionBoardCurve = new CurvedVal();

		public int mTransBoardOffsetX;

		public int mTransBoardOffsetY;

		public int mTransLevelOffsetY;

		public int mTransScoreOffsetY;

		public int mTransDashboardOffsetY;

		public int mTransReplayOffsetY;

		public int mTransOptionsBtnOffsetX;

		public int mTransHintBtnOffsetX;

		public Dictionary<string, string> mParams = new Dictionary<string, string>();

		public int mFullLaser;

		public bool mFromDebugMenu;

		public float mUpdateAcc;

		public int mNextPieceId;

		public Piece[,] mBoard = new Piece[8, 8];

		public float[] mBumpVelocities = new float[8];

		public int[] mNextColumnCredit = new int[8];

		public Dictionary<int, Piece> mPieceMap = new Dictionary<int, Piece>();

		public MTRand mRand = new MTRand();

		public EUIConfig mUiConfig;

		public int mLastHitSoundTick;

		public List<SwapData> mSwapDataVector = new List<SwapData>();

		public List<MoveData> mMoveDataVector = new List<MoveData>();

		public List<QueuedMove> mQueuedMoveVector = new List<QueuedMove>();

		public List<StateInfo> mStateInfoVector = new List<StateInfo>();

		public int[] mGameStats = new int[40];

		public int mGameOverCount;

		public int mLevelCompleteCount;

		public int mPoints;

		public bool mInLoadSave;

		public Color[] mBoardColors = new Color[2];

		public List<List<int>> mPointsBreakdown = new List<List<int>>();

		public int mDispPoints;

		public float mLevelBarPct;

		public float mCountdownBarPct;

		public int mLevelBarSizeBias;

		public List<int> mBackgroundIdxSet = new List<int>();

		public bool mGameFinished;

		public CurvedVal mLevelBarBonusAlpha = new CurvedVal();

		public int mLevelPointsTotal;

		public int mLevel;

		public int mHypermixerCheckRow;

		public int mPointMultiplier;

		public bool mShowPointMultiplier;

		public int mCurMoveCreditId;

		public int mCurMatchId;

		public int mGemFallDelay;

		public bool mTimeExpired;

		public int mLastWarningTick;

		public int mScrambleUsesLeft;

		public int mMoveCounter;

		public int mGameTicks;

		public int mIdleTicks;

		public int mSettlingDelay;

		public int mLastMatchTick;

		public int mLastMatchTime;

		public int mMatchTallyCount;

		public int mLastMatchTally;

		public CurvedVal mSpeedModeFactor = new CurvedVal();

		public float mSpeedBonusAlpha;

		public string mSpeedBonusText = string.Empty;

		public CurvedVal mSpeedometerPopup = new CurvedVal();

		public CurvedVal mSpeedometerGlow = new CurvedVal();

		public CurvedVal mSpeedBonusDisp = new CurvedVal();

		public float mSpeedNeedle;

		public int mSpeedBonusPoints;

		public bool mFavorComboGems;

		public List<int> mFavorGemColors = new List<int>();

		public List<int> mNewGemColors = new List<int>();

		public double mSpeedBonusNum;

		public int mSpeedBonusCount;

		public int mSpeedBonusCountHighest;

		public int mSpeedBonusLastCount;

		public int mSpeedMedCount;

		public int mSpeedHighCount;

		public bool mHardwareSpeedBonusDraw;

		public PIEffect mSpeedFirePIEffect = new PIEffect();

		public PIEffect[] mSpeedFireBarPIEffect = new PIEffect[2];

		public PIEffect mLevelBarPIEffect = new PIEffect();

		public PIEffect mCountdownBarPIEffect = new PIEffect();

		public CurvedVal mSpeedBonusPointsGlow = new CurvedVal();

		public CurvedVal mSpeedBonusPointsScale = new CurvedVal();

		public float mSpeedBonusFlameModePct;

		public EBoardType mBoardType;

		public bool mHasBoardSettled;

		public bool mContinuedFromLoad;

		public int mBoardUIOffsetY;

		public int[] mComboColors = new int[5];

		public int mComboCount;

		public int mLastComboCount;

		public int mComboLen;

		public float mComboCountDisp;

		public CurvedVal mComboFlashPct = new CurvedVal();

		public float mComboSelectorAngle;

		public int mLastPlayerSwapColor;

		public int mMoneyDisp;

		public int mMoneyDispGoal;

		public float mComboBonusSlowdownPct;

		public bool mWantNewCoin;

		public int mCoinsEarned;

		public int mPendingCoinAnimations;

		public List<LightningStorm> mLightningStorms = new List<LightningStorm>();

		public PointsManager mPointsManager;

		public Serialiser mLastMoveSave = new Serialiser();

		public SexyFramework.Misc.Buffer mTestSave = new SexyFramework.Misc.Buffer();

		public EffectsManager mPreFXManager;

		public EffectsManager mPostFXManager;

		public bool mUserPaused;

		public float mBoardHidePct;

		public float mVisPausePct;

		public SexyFramework.Misc.Buffer mUReplayBuffer = new SexyFramework.Misc.Buffer();

		public bool mInUReplay;

		public bool mWasLevelUpReplay;

		public int mUReplayVersion;

		public int mUReplayTotalTicks;

		public int mUReplayLastTick;

		public int mUReplayTicksLeft;

		public int mUReplayGameFlags;

		public bool mIllegalMoveTutorial;

		public bool mReplayWasTutorial;

		public CurvedVal mReplayPulsePct = new CurvedVal();

		public bool mWantReplaySave;

		public MoveData mReplayStartMove = new MoveData();

		public ReplayData mWholeGameReplay = new ReplayData();

		public bool mShowAutohints;

		public bool mHasReplayData;

		public bool mWatchedCurReplay;

		public int mReplayIgnoredMoves;

		public bool mReplayHadIgnoredMoves;

		public ReplayData mCurReplayData = new ReplayData();

		public Serialiser mPreReplaySave = new Serialiser();

		public SoundInstance mRewindSound;

		public SoundInstance mFlameSound;

		public int mRecordTimestamp;

		public int mPlaybackTimestamp;

		public bool mInReplay;

		public bool mIsOneMoveReplay;

		public bool mIsWholeGameReplay;

		public bool mHadReplayError;

		public bool mRewinding;

		public MTRand mRewindRand = new MTRand();

		public CurvedVal mReplayWidgetShowPct = new CurvedVal();

		public CurvedVal mReplayFadeout = new CurvedVal();

		public CurvedVal mPrevPointMultAlpha = new CurvedVal();

		public Point mSrcPointMultPos = default(Point);

		public CurvedVal mPointMultPosPct = new CurvedVal();

		public CurvedVal mPointMultTextMorph = new CurvedVal();

		public CurvedVal mPointMultScale = new CurvedVal();

		public CurvedVal mPointMultAlpha = new CurvedVal();

		public CurvedVal mPointMultYAdd = new CurvedVal();

		public CurvedVal mPointMultDarkenPct = new CurvedVal();

		public Color mPointMultColor = default(Color);

		public CurvedVal mTimerInflate = new CurvedVal();

		public CurvedVal mTimerAlpha = new CurvedVal();

		public List<int> mPointMultSoundQueue = new List<int>();

		public int mPointMultSoundDelay;

		public int mBottomFillRow;

		public int mGemCountValueDisp;

		public int mGemCountValueCheck;

		public CurvedVal mGemCountAlpha = new CurvedVal();

		public CurvedVal mGemScalarAlpha = new CurvedVal();

		public CurvedVal mGemCountCurve = new CurvedVal();

		public int mCascadeCountValueDisp;

		public int mCascadeCountValueCheck;

		public CurvedVal mCascadeCountAlpha = new CurvedVal();

		public CurvedVal mCascadeScalarAlpha = new CurvedVal();

		public CurvedVal mCascadeCountCurve = new CurvedVal();

		public CurvedVal mComplementAlpha = new CurvedVal();

		public CurvedVal mComplementScale = new CurvedVal();

		public int mComplementNum;

		public int mLastComplement;

		public string mSidebarText = string.Empty;

		public bool mShowLevelPoints;

		public int mHintCooldownTicks;

		public int mWantHintTicks;

		public int mTutorialFlags;

		public List<DeferredTutorial> mDeferredTutorialVector = new List<DeferredTutorial>();

		public CurvedVal mTutorialPieceIrisPct = new CurvedVal();

		public bool mShowMoveCredit;

		public bool mDoThirtySecondVoice;

		public CurvedVal mSunPosition = new CurvedVal();

		public bool mSunFired;

		public int mLastSunTick;

		public float mBoardDarken;

		public float mBoardDarkenAnnounce;

		public Color mWarningGlowColor = default(Color);

		public float mWarningGlowAlpha;

		public bool mMouseDown;

		public int mMouseDownX;

		public int mMouseDownY;

		public Piece mMouseUpPiece;

		public Piece mCheatPiece;

		public bool mCheatInputingScore;

		public string mCheatScoreStr;

		public bool mSlowedDown;

		public int mSlowDownCounter;

		public CurvedVal mCoinCatcherPct = new CurvedVal();

		public double mCoinCatcherPctPct;

		public bool mCoinCatcherAppearing;

		public int mBackgroundIdx;

		public Background mBackground;

		public Bej3Button mHintButton;

		public ButtonWidget mReplayButton;

		public ButtonWidget mResetButton;

		public bool mFirstDraw;

		public int mScrambleDelayTicks;

		public Slider mTimeSlider;

		public int mSliderSetTicks;

		public bool mWantLevelup;

		public Hyperspace mHyperspace;

		public bool mDoAutoload;

		public new CurvedVal mAlpha = new CurvedVal();

		public CurvedVal mScale = new CurvedVal();

		public bool mKilling;

		public CurvedVal mSlideUIPct = new CurvedVal();

		public CurvedVal mSideAlpha = new CurvedVal();

		public CurvedVal mSideXOff = new CurvedVal();

		public CurvedVal mBoostShowPct = new CurvedVal();

		public int mStartDelay;

		public int mWantHelpDialog;

		public CurvedVal mRestartPct = new CurvedVal();

		public DeviceImage mRestartPrevImage;

		public DeviceImage mFlattenedImage;

		public bool mFlattening;

		public bool mIsFacebookGame;

		public int mBoostsEnabled;

		public Point mCursorSelectPos = default(Point);

		public int mOffsetX;

		public int mOffsetY;

		public CurvedVal mNukeRadius = new CurvedVal();

		public CurvedVal mNukeAlpha = new CurvedVal();

		public CurvedVal mNovaRadius = new CurvedVal();

		public CurvedVal mNovaAlpha = new CurvedVal();

		public CurvedVal mGameOverPieceScale = new CurvedVal();

		public CurvedVal mGameOverPieceGlow = new CurvedVal();

		public Piece mGameOverPiece;

		public bool mShowBoard;

		public bool mWantsReddishFlamegems;

		private bool mTrialPromptShown;

		private bool mBuyFullGameShown;

		public bool mHyperspacePassed;

		public List<DeferredSound> mDeferredSounds = new List<DeferredSound>();

		public CurvedVal mQuestPortalPct = new CurvedVal();

		public CurvedVal mQuestPortalCenterPct = new CurvedVal();

		public Point mQuestPortalOrigin = default(Point);

		public bool mNeedsMaskCleared;

		public List<Piece> mDistortionPieces = new List<Piece>();

		public List<DistortionQuad> mDistortionQuads = new List<DistortionQuad>();

		public List<Announcement> mAnnouncements = new List<Announcement>();

		public Messager mMessager;

		public BadgeManager mBadgeManager;

		public bool mDoRankUp;

		public bool mZenDoBadgeAward;

		public bool mWantTimeAnnouncement;

		public int mTimeDelayCount;

		public int mReadyDelayCount;

		public int mGoDelayCount;

		public float mSlideXScale;

		public bool mGameClosing;

		public bool mGoAnnouncementDone;

		public bool mTimeAnnouncementDone;

		public bool mBackgroundLoadedThreaded;

		public bool mSuspendingGame;

		public bool mForceReleaseButterfly;

		public Piece mForcedReleasedBflyPiece;

		public int mNOfIntentionalMatchesDuringCascade;

		private List<AchievementHint> mAchievementHints = new List<AchievementHint>();

		protected AchievementHint mCurrentHint;

		public readonly int aMenuYPosHidden = ConstantsWP.MENU_Y_POS_HIDDEN;

		private bool mLevelup;

		private float BumpColumn_MAX_DIST = 100f * GlobalMembers.M(2f);

		private static int[,] FM_aSwapArray = new int[4, 2]
		{
			{ 1, 0 },
			{ -1, 0 },
			{ 0, 1 },
			{ 0, -1 }
		};

		private Dictionary<Piece, int> FS_aBulgeTriggerPieceSet = new Dictionary<Piece, int>();

		private Dictionary<Piece, int> FS_aDelayingPieceSet = new Dictionary<Piece, int>();

		private Dictionary<Piece, int> FS_aTallyPieceSet = new Dictionary<Piece, int>();

		private Dictionary<Piece, int> FS_aPowerupPieceSet = new Dictionary<Piece, int>();

		private List<MatchSet> FS_aMatchedSets = new List<MatchSet>();

		private Dictionary<int, int> FS_aMoveCreditSet = new Dictionary<int, int>();

		private Dictionary<Piece, Pair<int, Piece>> FS_aDeferPowerupMap = new Dictionary<Piece, Pair<int, Piece>>();

		private Dictionary<Piece, int> FS_aDeferLaserSet = new Dictionary<Piece, int>();

		private List<Piece> FS_aDeferExplodeVector = new List<Piece>();

		private int[] UpdateComplements_gComplementPoints = new int[6]
		{
			GlobalMembers.M(3),
			GlobalMembers.M(6),
			GlobalMembers.M(12),
			GlobalMembers.M(20),
			GlobalMembers.M(30),
			GlobalMembers.M(45)
		};

		private static float Update_aSpeed = 1f;

		private MyPiece[] DSP_pNormalPieces = new MyPiece[128];

		private MyPiece[] DSP_pHyperCubes = new MyPiece[64];

		private MyPiece[] DSP_pButterflies = new MyPiece[64];

		private Color[] DSP_gemColors = new Color[7]
		{
			new Color(255, 255, 255),
			new Color(192, 192, 192),
			new Color(32, 192, 32),
			new Color(224, 192, 32),
			new Color(255, 255, 255),
			new Color(255, 160, 32),
			new Color(255, 255, 255)
		};

		private Piece[] DP_pStdPieces = new Piece[128];

		private Piece[] DP_pShadowPieces = new Piece[128];

		private Piece[] DP_pQuestPieces = new Piece[128];

		private void ClipCollapsingBoard(Graphics g)
		{
			g.SetClipRect(GlobalMembers.S(GetBoardX()), GlobalMembers.S(GetBoardY()) + mTransBoardOffsetY + ConstantsWP.BOARD_FRAME_OVERLAP_Y, mWidth, GlobalMembers.S(100) * 8 - ConstantsWP.BOARD_FRAME_OVERLAP_Y - mTransBoardOffsetY * 2);
		}

		public Board()
			: base(Menu_Type.MENU_GAMEBOARD, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mShouldUnloadContentWhenDone = true;
			mBackgroundLoadedThreaded = false;
			FUDGE = 75;
			mCheatPiece = null;
			mCheatInputingScore = false;
			mCheatScoreStr = "";
			mSlowedDown = false;
			mSlowDownCounter = 0;
			mDoesSlideInFromBottom = (mCanAllowSlide = false);
			mContentLoaded = false;
			mFirstDraw = true;
			mReplayButton = null;
			mReplayPulsePct.SetConstant(-1.0);
			mReplayWasTutorial = false;
			mHintButton = null;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					mBoard[i, j] = null;
				}
			}
			mBoardType = EBoardType.eBoardType_Normal;
			mGameFinished = false;
			mNextPieceId = 1;
			mBottomFillRow = 7;
			int seed = BejeweledLivePlus.Misc.Common.Rand();
			mRand.SRand((uint)seed);
			for (int k = 0; k < 40; k++)
			{
				mGameStats[k] = 0;
			}
			mFullLaser = 1;
			mHasAlpha = true;
			mUpdateAcc = 0f;
			mGameOverCount = 0;
			mLevelCompleteCount = 0;
			mScrambleDelayTicks = 0;
			mOffsetX = 0;
			mOffsetY = 0;
			mLevelBarSizeBias = 0;
			mInLoadSave = false;
			mComplementNum = -1;
			mLastComplement = -1;
			mLastHitSoundTick = 0;
			mClip = false;
			mShowAutohints = true;
			mMouseDown = false;
			mMouseDownX = 0;
			mMouseDownY = 0;
			mMouseUpPiece = null;
			mContinuedFromLoad = false;
			mUiConfig = EUIConfig.eUIConfig_Standard;
			mSidebarText = "";
			mShowLevelPoints = false;
			mFavorComboGems = false;
			mWantNewCoin = false;
			mCoinsEarned = 0;
			mUserPaused = false;
			mBoardHidePct = 0f;
			mVisPausePct = 0f;
			mInUReplay = false;
			mWasLevelUpReplay = false;
			mUReplayLastTick = 0;
			mUReplayGameFlags = 3;
			mRewindSound = null;
			mRecordTimestamp = 0;
			mPlaybackTimestamp = 0;
			mHasReplayData = false;
			mWatchedCurReplay = false;
			mReplayIgnoredMoves = 0;
			mReplayHadIgnoredMoves = false;
			mInReplay = false;
			mIsOneMoveReplay = false;
			mIsWholeGameReplay = false;
			mRewinding = false;
			mTimeExpired = false;
			mHadReplayError = false;
			mLastWarningTick = 0;
			mShowMoveCredit = false;
			mDoThirtySecondVoice = true;
			mSunFired = false;
			mLastSunTick = 0;
			mCoinCatcherAppearing = false;
			mCoinCatcherPctPct = 0.0;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_COIN_CATCHER_PCT, mCoinCatcherPct);
			mPendingCoinAnimations = 0;
			mPointsManager = new PointsManager();
			mPointsManager.Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			AddWidget(mPointsManager);
			mBackground = null;
			mBackgroundIdx = -1;
			mTimeSlider = null;
			mPreFXManager = new EffectsManager(this);
			mPreFXManager.Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mPostFXManager = new EffectsManager(this, true);
			mPostFXManager.Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mTutorialFlags = (int)GlobalMembers.gApp.mProfile.mTutorialFlags;
			mWholeGameReplay.mTutorialFlags = GlobalMembers.gApp.mProfile.mTutorialFlags;
			mWholeGameReplay.mReplayTicks = 0;
			mWidgetFlagsMod.mAddFlags |= 2;
			mSideAlpha.SetConstant(1.0);
			mSideXOff.SetConstant(0.0);
			mAlpha.SetConstant(1.0);
			mScale.SetConstant(1.0);
			mKilling = false;
			mHardwareSpeedBonusDraw = GlobalMembers.gApp.mGraphicsDriver.GetRenderDevice3D() != null && GlobalMembers.gApp.mGraphicsDriver.GetRenderDevice3D().SupportsImageRenderTargets() && GlobalMembers.gApp.mGraphicsDriver.GetRenderDevice3D().SupportsPixelShaders();
			if (GlobalMembers.M(1) != 0)
			{
				mHardwareSpeedBonusDraw = false;
			}
			mSpeedFirePIEffect = null;
			mSpeedFireBarPIEffect[0] = null;
			mSpeedFireBarPIEffect[1] = null;
			mLevelBarPIEffect = null;
			mCountdownBarPIEffect = null;
			mHyperspace = null;
			mWantLevelup = false;
			mFlameSound = null;
			mSliderSetTicks = -1;
			mRestartPrevImage = null;
			mStartDelay = 0;
			mWantHelpDialog = -1;
			mFlattenedImage = null;
			mCursorSelectPos = new Point(-1, -1);
			mUReplayVersion = 0;
			mMoveCounter = 0;
			mShowPointMultiplier = false;
			mShowBoard = true;
			mGameOverPiece = null;
			mResetButton = null;
			mNeedsMaskCleared = false;
			mWantsReddishFlamegems = false;
			for (int l = 0; l < 7; l++)
			{
				mNewGemColors.Add(l);
				mNewGemColors.Add(l);
			}
			mBadgeManager = BadgeManager.GetBadgeManagerInstance();
			mMessager = null;
			mMessager = new Messager();
			mMessager.Init(GlobalMembersResources.FONT_DIALOG);
			mFlattening = false;
			mSlideXScale = 0f;
			mGameClosing = false;
			mGoAnnouncementDone = false;
			mTimeAnnouncementDone = false;
			mSuspendingGame = false;
			mForceReleaseButterfly = false;
			mForcedReleasedBflyPiece = null;
			GlobalMembers.gComplementStr[0] = GlobalMembers._ID("Good", 3561);
			GlobalMembers.gComplementStr[1] = GlobalMembers._ID("Excellent", 3562);
			GlobalMembers.gComplementStr[2] = GlobalMembers._ID("Awesome", 3563);
			GlobalMembers.gComplementStr[3] = GlobalMembers._ID("Spectacular", 3564);
			GlobalMembers.gComplementStr[4] = GlobalMembers._ID("Extraordinary", 3565);
			GlobalMembers.gComplementStr[5] = GlobalMembers._ID("Unbelievable", 3566);
			GlobalMembers.gComplementStr[6] = GlobalMembers._ID("Blazing Speed", 3567);
			mNOfIntentionalMatchesDuringCascade = 0;
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button != 0 || IsInOutPosition() || GlobalMembers.gApp.GetDialog(18) != null)
			{
				return;
			}
			if ((mWantLevelup || mHyperspace != null) && !mHyperspacePassed)
			{
				if (mHyperspace == null)
				{
					return;
				}
				HyperspaceWhirlpool hyperspaceWhirlpool = mHyperspace as HyperspaceWhirlpool;
				hyperspaceWhirlpool.mShowBkg = true;
				hyperspaceWhirlpool.mIsDone = true;
				mShowBoard = false;
				mHyperspacePassed = true;
				hyperspaceWhirlpool.SetState(HyperspaceWhirlpool.HyperSpaceState.HyperSpaceState_PortalRide);
				{
					foreach (Announcement mAnnouncement in mAnnouncements)
					{
						if (mAnnouncement.mText == GlobalMembers._ID("LEVEL\nCOMPLETE", 139))
						{
							mAnnouncements.Remove(mAnnouncement);
							break;
						}
					}
					return;
				}
			}
			if (GlobalMembers.gApp.mDialogMap.Count == 0)
			{
				if (mInReplay)
				{
					BackToGame();
				}
				else if (GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_ZEN || GlobalMembers.gApp.mMenus[19].mY == ConstantsWP.MENU_Y_POS_HIDDEN || GlobalMembers.gApp.mMenus[19].mY == 0)
				{
					args.processed = true;
					GlobalMembers.gApp.mLosfocus = true;
					(GlobalMembers.gApp.mMenus[7] as PauseMenu)?.ButtonDepress(10001);
				}
			}
		}

		public override void Dispose()
		{
			while (mLightningStorms.Count > 0)
			{
				if (mLightningStorms[mLightningStorms.Count - 1] != null)
				{
					mLightningStorms[mLightningStorms.Count - 1].Dispose();
				}
				mLightningStorms.RemoveAt(mLightningStorms.Count - 1);
			}
			ClearAllPieces();
			RemoveAllWidgets(true, false);
			GlobalMembers.KILL_WIDGET_NOW(mPreFXManager);
			if (GlobalMembers.gApp.mBlitzBackground != mBackground && GlobalMembers.gApp.mBlitzBackgroundLo != mBackground)
			{
				GlobalMembers.KILL_WIDGET_NOW(mBackground);
			}
			GlobalMembers.KILL_WIDGET_NOW(mHyperspace);
			if (mRewindSound != null)
			{
				mRewindSound.Release();
				mRewindSound = null;
			}
			if (mSpeedFirePIEffect != null)
			{
				mSpeedFirePIEffect.Dispose();
			}
			if (mSpeedFireBarPIEffect[0] != null)
			{
				mSpeedFireBarPIEffect[0].Dispose();
			}
			if (mSpeedFireBarPIEffect[1] != null)
			{
				mSpeedFireBarPIEffect[1].Dispose();
			}
			if (mLevelBarPIEffect != null)
			{
				mLevelBarPIEffect.Dispose();
			}
			if (mCountdownBarPIEffect != null)
			{
				mCountdownBarPIEffect.Dispose();
			}
			while (mAnnouncements.Count > 0)
			{
				if (mAnnouncements[mAnnouncements.Count - 1] != null)
				{
					mAnnouncements[mAnnouncements.Count - 1].Dispose();
				}
				mAnnouncements.RemoveAt(mAnnouncements.Count - 1);
			}
			if (mFlameSound != null)
			{
				mFlameSound.Release();
			}
			if (mShouldUnloadContentWhenDone)
			{
				UnloadContent();
			}
			base.Dispose();
		}

		public virtual int GetGameOverCountTreshold()
		{
			return GlobalMembers.M(400);
		}

		public virtual void Pause()
		{
			mUserPaused = true;
		}

		public virtual void Unpause()
		{
			mUserPaused = false;
			PlayMenuMusic();
			mSuspendingGame = false;
		}

		public virtual void UnloadContent()
		{
			mContentLoaded = false;
		}

		public virtual void LoadContent(bool threaded)
		{
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlay");
				mBackgroundIdx = -1;
				SetupBackground(1);
				mBackgroundLoadedThreaded = true;
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlay");
				LinkUpAssets();
				RefreshUI();
				mContentLoaded = true;
			}
		}

		public override void Show()
		{
			if (mState != Bej3WidgetState.STATE_IN)
			{
				LoadContent(false);
			}
			base.Show();
			mY = 0;
		}

		public override void Hide()
		{
			base.Hide();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBEJ3_WIDGET_HIDE_CURVE, mAlphaCurve);
			mTargetPos = 0;
		}

		public virtual void RestartGameRequest()
		{
			Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.DoDialog(22, true, GlobalMembers._ID("RESTART GAME?", 3236), GlobalMembers._ID("Abandon the current game and start again? Your current game will be lost.", 3237), "", 1);
			int dIALOG_RESTART_GAME_WIDTH = ConstantsWP.DIALOG_RESTART_GAME_WIDTH;
			bej3Dialog.Resize(GlobalMembers.S(GetBoardCenterX()) - dIALOG_RESTART_GAME_WIDTH / 2, mHeight / 2, dIALOG_RESTART_GAME_WIDTH, bej3Dialog.GetPreferredHeight(dIALOG_RESTART_GAME_WIDTH));
			Bej3Button bej3Button = (Bej3Button)bej3Dialog.mYesButton;
			bej3Button.SetLabel(GlobalMembers._ID("RESTART GAME", 3238));
			bej3Dialog.SetButtonPosition(bej3Button, 0);
			bej3Button = (Bej3Button)bej3Dialog.mNoButton;
			bej3Button.SetLabel(GlobalMembers._ID("CANCEL", 3239));
			bej3Button.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			bej3Dialog.mDialogListener = this;
			bej3Dialog.mFlushPriority = 1;
			bej3Dialog.SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
		}

		public void DoPrompt()
		{
			mTrialPromptShown = true;
			if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_BUTTERFLY)
			{
				Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.DoDialog(51, true, GlobalMembers._ID("PROMPT", 3789), string.Format(GlobalMembers._ID("In the trial version you can only reach a maximum of {0} points. Unlock the full version to enjoy the full experience.", 3797), SexyFramework.Common.CommaSeperate(95000)), "", 3);
				bej3Dialog.mDialogListener = this;
				((Bej3Button)bej3Dialog.mYesButton).SetLabel(GlobalMembers._ID("OK", 116));
			}
			else if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_DIAMOND_MINE)
			{
				Bej3Dialog bej3Dialog2 = (Bej3Dialog)GlobalMembers.gApp.DoDialog(51, true, GlobalMembers._ID("PROMPT", 3789), string.Format(GlobalMembers._ID("In the trial version you can only reach a maximum of {0} points. Unlock the full version to enjoy the full experience.", 3797), SexyFramework.Common.CommaSeperate(50000)), "", 3);
				bej3Dialog2.mDialogListener = this;
				((Bej3Button)bej3Dialog2.mYesButton).SetLabel(GlobalMembers._ID("OK", 116));
			}
			else if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_CLASSIC)
			{
				Bej3Dialog bej3Dialog3 = (Bej3Dialog)GlobalMembers.gApp.DoDialog(51, true, GlobalMembers._ID("PROMPT", 3789), GlobalMembers._ID("In the trial version you can only reach maximum Level of the 4th. Unlock the full version to enjoy the full experience.", 3800), "", 3);
				bej3Dialog3.mDialogListener = this;
				((Bej3Button)bej3Dialog3.mYesButton).SetLabel(GlobalMembers._ID("OK", 116));
			}
		}

		public virtual void MainMenuRequest()
		{
			Bej3Dialog bej3Dialog = (GlobalMembers.gApp.mMainMenu.mIsFullGame() ? ((Bej3Dialog)GlobalMembers.gApp.DoDialog(50, true, GlobalMembers._ID("PROMPT", 3789), GlobalMembers._ID("Do you wish to go to main menu? Your game will be saved.", 3790), "", 1)) : ((Bej3Dialog)GlobalMembers.gApp.DoDialog(50, true, GlobalMembers._ID("PROMPT", 3789), GlobalMembers._ID("Do you wish to go to main menu? Your game is not saved in the trial game.", 3799), "", 1)));
			int dIALOG_RESTART_GAME_WIDTH = ConstantsWP.DIALOG_RESTART_GAME_WIDTH;
			bej3Dialog.Resize(GlobalMembers.S(GlobalMembers.gApp.mBoard.GetBoardCenterX()) - dIALOG_RESTART_GAME_WIDTH / 2, mHeight / 2, dIALOG_RESTART_GAME_WIDTH, bej3Dialog.GetPreferredHeight(dIALOG_RESTART_GAME_WIDTH));
			Bej3Button bej3Button = (Bej3Button)bej3Dialog.mYesButton;
			bej3Button.SetLabel(GlobalMembers._ID("MAIN MENU", 3293));
			bej3Dialog.SetButtonPosition(bej3Button, 0);
			bej3Button = (Bej3Button)bej3Dialog.mNoButton;
			bej3Button.SetLabel(GlobalMembers._ID("CANCEL", 3239));
			bej3Button.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			bej3Dialog.mDialogListener = this;
			bej3Dialog.mFlushPriority = 1;
			bej3Dialog.SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
		}

		public virtual bool WantFreezeAutoplay()
		{
			if (GlobalMembers.gApp.mAutoPlay == BejeweledLivePlusApp.EAutoPlayState.AUTOPLAY_TEST_HYPER)
			{
				return GlobalMembers.gApp.mAutoLevelUpCount >= GlobalMembers.M(300);
			}
			return mWantLevelup;
		}

		public virtual bool WriteUReplayCmd(int theCmd)
		{
			if (mInUReplay || !WantsWholeGameReplay())
			{
				return false;
			}
			byte b = (byte)theCmd;
			int num = mUpdateCnt - mUReplayLastTick;
			if (num > 255)
			{
				b |= 0x40;
			}
			else if (num > 0)
			{
				b |= 0x80;
			}
			mUReplayBuffer.WriteByte(b);
			if (num > 255)
			{
				mUReplayBuffer.WriteShort((short)num);
			}
			else if (num > 0)
			{
				mUReplayBuffer.WriteByte((byte)num);
			}
			mUReplayLastTick = mUpdateCnt;
			return true;
		}

		public virtual void InitUI()
		{
			if ((mUiConfig == EUIConfig.eUIConfig_Standard || mUiConfig == EUIConfig.eUIConfig_WithResetAndReplay || mUiConfig == EUIConfig.eUIConfig_WithReset || mUiConfig == EUIConfig.eUIConfig_StandardNoReplay) && mHintButton == null)
			{
				mHintButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG);
				mHintButton.SetLabel(GlobalMembers._ID("HINT", 3220));
				mHintButton.mPlayPressSound = false;
				AddWidget(mHintButton);
			}
			switch (mUiConfig)
			{
			case EUIConfig.eUIConfig_WithReset:
			case EUIConfig.eUIConfig_WithResetAndReplay:
			case EUIConfig.eUIConfig_Quest:
				if (mResetButton == null)
				{
					mResetButton = new Bej3Button(2, this);
					AddWidget(mResetButton);
				}
				break;
			}
			if ((mUiConfig == EUIConfig.eUIConfig_Standard || mUiConfig == EUIConfig.eUIConfig_WithResetAndReplay) && mReplayButton == null)
			{
				mReplayButton = new Bej3Button(3, this);
				mReplayButton.mMouseVisible = false;
				mReplayButton.mBtnNoDraw = true;
				AddWidget(mReplayButton);
			}
		}

		public virtual void RefreshUI()
		{
			if (mUiConfig == EUIConfig.eUIConfig_Standard || mUiConfig == EUIConfig.eUIConfig_StandardNoReplay)
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
			if (mReplayButton != null)
			{
				mReplayButton.Resize(GlobalMembers.IMGRECT_S(GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON, 0f, GetBottomWidgetOffset()));
				mReplayButton.mButtonImage = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON;
				mReplayButton.mNormalRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelRect(0);
				mReplayButton.mOverImage = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON;
				mReplayButton.mOverRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelRect(1);
				mReplayButton.mDownImage = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON;
				mReplayButton.mDownRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelRect(1);
				mReplayButton.mHasAlpha = true;
				mReplayButton.mDoFinger = true;
				mReplayButton.mOverAlphaSpeed = 0.1;
				mReplayButton.mOverAlphaFadeInSpeed = 0.2;
				mReplayButton.mLabel = "";
			}
		}

		public void RemoveAllButtons()
		{
			RemoveWidget(mHintButton);
			if (mHintButton != null)
			{
				mHintButton.Dispose();
			}
			RemoveWidget(mReplayButton);
			if (mReplayButton != null)
			{
				mReplayButton.Dispose();
			}
			RemoveWidget(mResetButton);
			if (mResetButton != null)
			{
				mResetButton.Dispose();
			}
		}

		public virtual void Init()
		{
			mShouldUnloadContentWhenDone = true;
			while (mLightningStorms.Count > 0)
			{
				if (mLightningStorms[mLightningStorms.Count - 1] != null)
				{
					mLightningStorms[mLightningStorms.Count - 1].Dispose();
				}
				mLightningStorms.RemoveAt(mLightningStorms.Count - 1);
			}
			mMoveDataVector.Clear();
			mQueuedMoveVector.Clear();
			ClearAllPieces();
			mBoardColors[0] = new Color(6, 6, 6, GlobalMembers.M(160));
			mBoardColors[1] = new Color(24, 24, 24, GlobalMembers.M(160));
			mBoardUIOffsetY = 0;
			mGameFinished = false;
			mGameTicks = 0;
			mIdleTicks = 0;
			mGameOverCount = 0;
			mSettlingDelay = 0;
			mLastMatchTick = -1000;
			mLastMatchTime = 1000;
			mMatchTallyCount = 0;
			mLastMatchTally = 0;
			mUpdateCnt = 0;
			mSpeedNeedle = 50f;
			mSpeedBonusAlpha = 0f;
			mSpeedBonusPoints = 0;
			mSpeedModeFactor.SetConstant(3.0);
			mSpeedBonusNum = 0.0;
			mSpeedBonusCount = 0;
			mSpeedBonusCountHighest = 0;
			mSpeedBonusLastCount = 0;
			mSpeedBonusFlameModePct = 0f;
			mSpeedMedCount = 0;
			mSpeedHighCount = 0;
			mSpeedBonusPointsScale.SetConstant(0.0);
			mHypermixerCheckRow = 3;
			mLastWarningTick = 0;
			mCurMoveCreditId = 0;
			mCurMatchId = 0;
			mGemFallDelay = 0;
			mPointMultiplier = 1;
			mPoints = 0;
			mPointsBreakdown.Clear();
			AddPointBreakdownSection();
			mDispPoints = 0;
			mLevelBarPct = 0f;
			mCountdownBarPct = 0f;
			mLevelPointsTotal = 0;
			mLevel = 0;
			mScrambleUsesLeft = 2;
			mDeferredSounds.Clear();
			mComboCount = 0;
			mLastComboCount = 0;
			mComboCountDisp = 0f;
			mComboSelectorAngle = 22f;
			mLastPlayerSwapColor = -1;
			mMoneyDisp = GlobalMembers.gApp.GetCoinCount();
			mMoneyDispGoal = 0;
			mComboBonusSlowdownPct = 0f;
			mGemCountValueDisp = 0;
			mCascadeCountValueDisp = 0;
			mGemCountValueCheck = 0;
			mCascadeCountValueCheck = 0;
			mHintCooldownTicks = 0;
			mWantHintTicks = 0;
			mBoardDarken = 0f;
			mBoardDarkenAnnounce = 0f;
			mWarningGlowAlpha = 0f;
			mTimeExpired = false;
			mPointMultPosPct.SetConstant(1.0);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_TIMER_INFLATE, mTimerInflate);
			mTimerAlpha.SetConstant(1.0);
			for (int i = 0; i < 8; i++)
			{
				mBumpVelocities[i] = 0f;
				mNextColumnCredit[i] = -1;
			}
			InitUI();
			mZenDoBadgeAward = false;
			mDoRankUp = false;
			mBadgeManager.LinkBoard(this);
			mWantTimeAnnouncement = false;
			mTimeDelayCount = 0;
			mReadyDelayCount = 0;
			mGoDelayCount = 0;
			mLastMoveSave.Clear();
			mPreFXManager.Clear();
			mPostFXManager.Clear();
			while (mAnnouncements.Count > 0)
			{
				if (mAnnouncements[mAnnouncements.Count - 1] != null)
				{
					mAnnouncements[mAnnouncements.Count - 1].Dispose();
				}
				mAnnouncements.RemoveAt(mAnnouncements.Count - 1);
			}
			GlobalMembers.gGR.ClearOperationsFrom(0);
			mGoAnnouncementDone = false;
			mTimeAnnouncementDone = false;
			mTransitionBoardCurve.SetConstant(0.0);
			mSlidingHUDCurve.SetConstant(0.0);
			mTransBoardOffsetX = 0;
			mTransBoardOffsetY = 0;
			mTransLevelOffsetY = 0;
			mTransScoreOffsetY = 0;
			mTransDashboardOffsetY = 0;
			mTransReplayOffsetY = 0;
			mTransOptionsBtnOffsetX = 0;
			mTransHintBtnOffsetX = 0;
			mSuspendingGame = false;
			mIllegalMoveTutorial = false;
			mWantReplaySave = false;
			mForceReleaseButterfly = false;
			mForcedReleasedBflyPiece = null;
			mNOfIntentionalMatchesDuringCascade = 0;
		}

		public void ConfigureBarEmitters()
		{
			if (WantTopLevelBar())
			{
				Rect levelBarRect = GetLevelBarRect();
				mLevelBarPIEffect.mEmitAfterTimeline = true;
				mLevelBarPIEffect.mDrawTransform.LoadIdentity();
				mLevelBarPIEffect.mDrawTransform.Translate(levelBarRect.mX, levelBarRect.mY);
				PILayer layer = mLevelBarPIEffect.GetLayer(0);
				PIDeflector pIDeflector = layer.mLayerDef.mDeflectorVector[0];
				pIDeflector.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, 0f).ToXnaVector2();
				pIDeflector.mPoints[2].mValuePoint2DVector[0].mValue = new FPoint(0f, levelBarRect.mHeight).ToXnaVector2();
				pIDeflector.mPoints[3].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, levelBarRect.mHeight).ToXnaVector2();
				layer = mLevelBarPIEffect.GetLayer(1);
				pIDeflector = layer.mLayerDef.mDeflectorVector[0];
				pIDeflector.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, 0f).ToXnaVector2();
				pIDeflector.mPoints[2].mValuePoint2DVector[0].mValue = new FPoint(0f, levelBarRect.mHeight).ToXnaVector2();
				pIDeflector.mPoints[3].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, levelBarRect.mHeight).ToXnaVector2();
			}
			else
			{
				Rect levelBarRect = GetCountdownBarRect();
				mCountdownBarPIEffect.mEmitAfterTimeline = true;
				mCountdownBarPIEffect.mDrawTransform.LoadIdentity();
				mCountdownBarPIEffect.mDrawTransform.Translate(levelBarRect.mX, levelBarRect.mY);
				PILayer layer = mCountdownBarPIEffect.GetLayer(0);
				PIDeflector pIDeflector = layer.mLayerDef.mDeflectorVector[0];
				pIDeflector.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, 0f).ToXnaVector2();
				pIDeflector.mPoints[2].mValuePoint2DVector[0].mValue = new FPoint(0f, levelBarRect.mHeight).ToXnaVector2();
				pIDeflector.mPoints[3].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, levelBarRect.mHeight).ToXnaVector2();
				layer = mCountdownBarPIEffect.GetLayer(1);
				pIDeflector = layer.mLayerDef.mDeflectorVector[0];
				pIDeflector.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, 0f).ToXnaVector2();
				pIDeflector.mPoints[2].mValuePoint2DVector[0].mValue = new FPoint(0f, levelBarRect.mHeight).ToXnaVector2();
				pIDeflector.mPoints[3].mValuePoint2DVector[0].mValue = new FPoint(levelBarRect.mWidth, levelBarRect.mHeight).ToXnaVector2();
			}
		}

		public virtual Rect GetLevelBarRect()
		{
			if (WantTopLevelBar())
			{
				int boardCenterX = GetBoardCenterX();
				int theNum = (int)GlobalMembersResourcesWP.ImgYOfs(1092);
				Rect celRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_BACK.GetCelRect(0);
				celRect.Offset(GlobalMembers.S(boardCenterX) - celRect.mWidth / 2, GlobalMembers.S(theNum));
				return celRect;
			}
			return new Rect(0, 0, 0, 0);
		}

		public virtual Rect GetCountdownBarRect()
		{
			int boardCenterX = GetBoardCenterX();
			int theNum = GetBoardY() + 800 + GlobalMembers.M(30);
			Rect celRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_BACK.GetCelRect(0);
			celRect.Offset(GlobalMembers.S(boardCenterX) - celRect.mWidth / 2, GlobalMembers.S(theNum) - celRect.mHeight / 2);
			return celRect;
		}

		public Piece GetTutorialIrisPiece()
		{
			if (mDeferredTutorialVector.Count > 0)
			{
				return GetPieceById(mDeferredTutorialVector[0].mPieceId);
			}
			return null;
		}

		public virtual string GetSavedGameName()
		{
			return string.Empty;
		}

		public virtual string GetMusicName()
		{
			return "Classic";
		}

		public virtual bool AllowNoMoreMoves()
		{
			if (mLevel != 0)
			{
				return mSpeedBonusFlameModePct == 0f;
			}
			return false;
		}

		public virtual bool AllowSpeedBonus()
		{
			return true;
		}

		public virtual bool AllowPowerups()
		{
			return true;
		}

		public virtual bool AllowLaserGems()
		{
			return true;
		}

		public virtual bool AllowHints()
		{
			return true;
		}

		public virtual bool AllowTooltips()
		{
			return false;
		}

		public virtual bool HasLargeExplosions()
		{
			return false;
		}

		public virtual bool ForceSwaps()
		{
			return false;
		}

		public virtual bool CanPlay()
		{
			if (mAnnouncements.Count > 0)
			{
				foreach (Announcement mAnnouncement in mAnnouncements)
				{
					if (mAnnouncement.mBlocksPlay)
					{
						return false;
					}
				}
			}
			if (mDeferredTutorialVector.Count > 0)
			{
				return false;
			}
			if (!mHasBoardSettled)
			{
				return false;
			}
			if (mReadyDelayCount != 0)
			{
				return false;
			}
			if (mLevelCompleteCount != 0)
			{
				return false;
			}
			if (mGameOverCount != 0)
			{
				return false;
			}
			if (GetTicksLeft() == 0)
			{
				return false;
			}
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.IsFlagSet(96u) && piece.mCounter == 0)
				{
					return false;
				}
			}
			if (mHyperspace != null)
			{
				return false;
			}
			return true;
		}

		public virtual bool WantsBackground()
		{
			return true;
		}

		public virtual bool WantsLevelBasedBackground()
		{
			return false;
		}

		public virtual bool IsGameSuspended()
		{
			if (mInUReplay)
			{
				return (mUReplayGameFlags & 2) != 0;
			}
			if (mReadyDelayCount <= 0 && mScrambleDelayTicks <= 0 && !mTimeExpired)
			{
				if (mLevelCompleteCount == 0 && GlobalMembers.gApp.GetDialog(18) == null && GlobalMembers.gApp.GetDialog(39) == null)
				{
					return mLightningStorms.Count != 0;
				}
				return true;
			}
			return true;
		}

		public virtual bool CanPiecesFall()
		{
			if (mInUReplay)
			{
				return (mUReplayGameFlags & 1) != 0;
			}
			if (GlobalMembers.gApp.GetDialog(18) == null && mGemFallDelay == 0 && mLightningStorms.Count == 0)
			{
				return mHyperspace == null;
			}
			return false;
		}

		public virtual bool IsGamePaused()
		{
			if ((mUserPaused || !GlobalMembers.gApp.mHasFocus || GlobalMembers.gApp.GetDialog(21) != null || GlobalMembers.gApp.GetDialog(22) != null) && WantsHideOnPause())
			{
				return !mInReplay;
			}
			return false;
		}

		public virtual int GetTimeLimit()
		{
			return 0;
		}

		public virtual int GetTimeDrawX()
		{
			return GlobalMembers.S(GetBoardCenterX());
		}

		public virtual int GetHintTime()
		{
			return 15;
		}

		public virtual bool WantsHideOnPause()
		{
			return GetTimeLimit() != 0;
		}

		public virtual bool WantHypermixerEdgeCheck()
		{
			return false;
		}

		public virtual bool WantHypermixerBottomCheck()
		{
			return true;
		}

		public virtual bool WantAnnihilatorReplacement()
		{
			return false;
		}

		public virtual bool SupportsReplays()
		{
			return false;
		}

		public virtual bool WantsWholeGameReplay()
		{
			return false;
		}

		public virtual bool WantsCalmEffects()
		{
			return false;
		}

		public virtual bool WantsTutorialReplays()
		{
			return true;
		}

		public virtual int GetGemCountPopupThreshold()
		{
			return 10;
		}

		public virtual int GetMinComplementLevel()
		{
			return 0;
		}

		public virtual float GetGravityFactor()
		{
			return (float)(1.0 + ((double)mSpeedModeFactor - 1.0) * (double)GlobalMembers.M(0.65f));
		}

		public virtual float GetSwapSpeed()
		{
			return (float)(double)mSpeedModeFactor;
		}

		public virtual float GetMatchSpeed()
		{
			return (float)(double)mSpeedModeFactor;
		}

		public virtual float GetGameSpeed()
		{
			return 1f;
		}

		public virtual float GetSpeedModeFactorScale()
		{
			return 1f;
		}

		public virtual float GetModePointMultiplier()
		{
			return 1f;
		}

		public virtual float GetRankPointMultiplier()
		{
			return GetModePointMultiplier();
		}

		public virtual string GetTopWidgetButtonText()
		{
			return string.Empty;
		}

		public virtual int GetBottomWidgetOffset()
		{
			return 0;
		}

		public virtual bool WantColorCombos()
		{
			return false;
		}

		public virtual int WantExpandedTopWidget()
		{
			return 0;
		}

		public virtual bool WantHyperMixers()
		{
			return false;
		}

		public virtual bool WantBulgeCascades()
		{
			return true;
		}

		public virtual bool WantDrawTimer()
		{
			return true;
		}

		public virtual bool WantsTutorial(int theTutorialFlag)
		{
			for (int i = 0; i < mDeferredTutorialVector.Count; i++)
			{
				if (mDeferredTutorialVector[i].mTutorialFlag == theTutorialFlag)
				{
					return false;
				}
			}
			if (!GlobalMembers.gApp.HasClearedTutorial(theTutorialFlag) && mSpeedBonusFlameModePct == 0f)
			{
				return !mTimeExpired;
			}
			return false;
		}

		public virtual bool WantAutoload()
		{
			return true;
		}

		public virtual void HypermixerDropped()
		{
		}

		public virtual HYPERSPACETRANS GetHyperspaceTransType()
		{
			return HYPERSPACETRANS.HYPERSPACETRANS_Classic;
		}

		public bool CheckLoad()
		{
			if (true)
			{
				return LoadGame();
			}
			return false;
		}

		public static void ParseGridLayout(string theStr, List<GridData> outGrid, bool theEnforceStdGridSize)
		{
			int num = 0;
			for (int i = 0; i < theStr.Length; i++)
			{
				if (char.IsLetter(theStr[i]))
				{
					GridData gridData = outGrid[outGrid.Count - 1];
					if (num == 0)
					{
						gridData = outGrid[outGrid.Count - 1];
						num = gridData.mTiles.Count - 1;
					}
					gridData.At((num - 1) / 8, (num - 1) % 8).mAttr = theStr[i];
				}
				else
				{
					if (!char.IsDigit(theStr[i]))
					{
						continue;
					}
					if (theEnforceStdGridSize && (outGrid.Count == 0 || num >= 64))
					{
						num = 0;
						outGrid.Add(new GridData());
						for (int j = 0; j < 64; j++)
						{
							outGrid[outGrid.Count - 1].mTiles.Add(new GridData.TileData());
						}
					}
					else if (!theEnforceStdGridSize && (outGrid.Count == 0 || num / 8 >= outGrid[outGrid.Count - 1].GetRowCount()))
					{
						if (outGrid.Count == 0)
						{
							outGrid.Add(new GridData());
						}
						outGrid[outGrid.Count - 1].AddRow();
					}
					string s = theStr[i].ToString();
					int result = 0;
					int.TryParse(s, out result);
					outGrid[outGrid.Count - 1].At(num / 8, num % 8).mBack = (uint)result;
					num++;
				}
			}
		}

		public void ReloadConfig()
		{
			GlobalMembers.gApp.LoadConfigs();
			if (mMessager != null)
			{
				mMessager.AddMessage("Reloaded configs");
			}
		}

		public void ReplayGame()
		{
			string savedGameName = GetSavedGameName();
			if (!string.IsNullOrEmpty(savedGameName))
			{
				Serialiser theBuffer = new Serialiser();
				if (GlobalMembers.gApp.ReadBufferFromStorage(GlobalMembers.gApp.mProfile.GetProfileDir(GlobalMembers.gApp.mProfile.mProfileName) + "\\" + savedGameName, theBuffer))
				{
					LoadGame(theBuffer, false);
				}
			}
		}

		public virtual void NewGame()
		{
			NewGame(false);
		}

		public virtual void NewGame(bool restartingGame)
		{
			BejeweledLivePlusApp.mAllowRating = true;
			mUserPaused = false;
			mVisPausePct = 0f;
			uint randSeedOverride = GetRandSeedOverride();
			if (randSeedOverride != 0)
			{
				mRand.SRand(randSeedOverride);
			}
			for (int i = 0; i < 40; i++)
			{
				mGameStats[i] = 0;
			}
			if (mParams.ContainsKey("BackgroundIdx"))
			{
				int result = 0;
				int.TryParse(mParams["BackgroundIdx"], out result);
				mBackgroundIdx = result;
			}
			if (mParams.ContainsKey("BackgroundIdxSet"))
			{
				Utils.SplitAndConvertStr(mParams["BackgroundIdxSet"], mBackgroundIdxSet);
			}
			mBackgroundIdx %= GlobalMembers.gBackgroundNames.Length;
			if (WantsLevelBasedBackground())
			{
				if (!restartingGame || mBackgroundIdx != 0)
				{
					mBackgroundIdx = -1;
					SetupBackground(1);
				}
			}
			else
			{
				SetupBackground();
			}
			ConfigureBarEmitters();
			mHasBoardSettled = false;
			mContinuedFromLoad = false;
			if (WantAutoload())
			{
				if (CheckLoad())
				{
					mContinuedFromLoad = true;
				}
				else
				{
					NewCombo();
					FillInBlanks(false, true);
				}
			}
			else
			{
				NewCombo();
				FillInBlanks(false);
			}
			if (!mContinuedFromLoad)
			{
				GlobalMembers.gApp.LogStatString($"GameStart Title=\"{GetLoggingGameName()}\"");
				if (GetTimeLimit() >= 0)
				{
					mTimeDelayCount = 1;
				}
				else
				{
					mTimeDelayCount = 0;
				}
				mReadyDelayCount = GlobalMembers.M(0);
				mGoDelayCount = GlobalMembers.M(25);
			}
			else
			{
				mTimeDelayCount = 0;
				if (WantsHideOnPause())
				{
					mReadyDelayCount = GlobalMembers.M(120);
					mGoDelayCount = GlobalMembers.M(120);
				}
				else
				{
					mGoDelayCount = GlobalMembers.M(25);
				}
			}
			mSettlingDelay = 0;
			mSuspendingGame = false;
		}

		public virtual bool SaveGame(Serialiser theBuffer)
		{
			if (GlobalMembers.gApp != null && GlobalMembers.gApp.mProfile != null)
			{
				if (mGameClosing)
				{
					SyncUnAwardedBadges(GlobalMembers.gApp.mProfile.mDeferredBadgeVector);
				}
				if (mGameClosing && !IsGameIdle())
				{
					GlobalMembers.gApp.mProfile.mStats.Swap(false);
					GlobalMembers.gApp.mProfile.WriteProfile();
				}
				else
				{
					GlobalMembers.gApp.mProfile.mStats.Swap(true);
				}
			}
			if (mLightningStorms.Count != 0 || mHyperspace != null)
			{
				return false;
			}
			theBuffer.Clear();
			theBuffer.WriteFileHeader(101, (int)GlobalMembers.gApp.mCurrentGameMode);
			int chunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkBoard);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardUpdateCnt, mUpdateCnt);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.BoardPieces, 64);
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					theBuffer.WriteBoolean(piece != null);
					if (piece != null)
					{
						if (piece.IsFlagSet(2u))
						{
							int mMatchId = piece.mMatchId;
						}
						theBuffer.WriteInt32(i);
						theBuffer.WriteInt32(j);
						theBuffer.WriteInt32(piece.mId);
						piece.Save(theBuffer);
					}
				}
			}
			theBuffer.WriteArrayPair(Serialiser.PairID.BoardBumpVelocities, 8, mBumpVelocities);
			theBuffer.WriteArrayPair(Serialiser.PairID.BoardNextColumnCredit, 8, mNextColumnCredit);
			string str = mRand.Serialize();
			theBuffer.WriteStringPair(Serialiser.PairID.BoardRand, str);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.BoardSwapData, mSwapDataVector.Count);
			SaveSwapData(theBuffer, mSwapDataVector);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.BoardMoveData, mMoveDataVector.Count);
			SaveMoveData(theBuffer, mMoveDataVector, 103);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.BoardQueuedMoves, mQueuedMoveVector.Count);
			SaveQueuedMoves(theBuffer, mQueuedMoveVector);
			theBuffer.WriteArrayPair(Serialiser.PairID.BoardGameStats, 40, mGameStats);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardPoints, mPoints);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.BoardPointsBreakdown, mPointsBreakdown.Count, 5);
			for (int k = 0; k < mPointsBreakdown.Count; k++)
			{
				for (int l = 0; l < 5; l++)
				{
					theBuffer.WriteInt32(mPointsBreakdown[k][l]);
				}
			}
			theBuffer.WriteValuePair(Serialiser.PairID.BoardMoneyDisp, mMoneyDisp);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardLevelBarPct, mLevelBarPct);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardCountdownBarPct, mCountdownBarPct);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardLevelPointsTotal, mLevelPointsTotal);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardLevel, mLevel);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardPointMultiplier, mPointMultiplier);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardCurMoveCreditId, mCurMoveCreditId);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardCurMatchId, mCurMatchId);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardGemFallDelay, mGemFallDelay);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardMoveCounter, mMoveCounter);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardGameTicks, mGameTicks);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardIdleTicks, mIdleTicks);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardLastMatchTick, mLastMatchTick);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardLastMatchTime, mLastMatchTime);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardMatchTallyCount, mMatchTallyCount);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedModeFactor, mSpeedModeFactor);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedBonusAlpha, mSpeedBonusAlpha);
			theBuffer.WriteStringPair(Serialiser.PairID.BoardSpeedBonusText, mSpeedBonusText);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedometerPopup, mSpeedometerPopup);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedometerGlow, mSpeedometerGlow);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedBonusDisp, mSpeedBonusDisp);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedNeedle, mSpeedNeedle);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedBonusPoints, mSpeedBonusPoints);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedBonusNum, mSpeedBonusNum);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedBonusCount, mSpeedBonusCount);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedBonusCountHighest, mSpeedBonusCountHighest);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardSpeedBonusLastCount, mSpeedBonusLastCount);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardHasBoardSettled, mHasBoardSettled);
			theBuffer.WriteArrayPair(Serialiser.PairID.BoardComboColors, 5, mComboColors);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardComboCount, mComboCount);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardComboLen, mComboLen);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardComboCountDisp, mComboCountDisp);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardComboFlashPct, mComboFlashPct);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardLastPlayerSwapColor, mLastPlayerSwapColor);
			if (mUpdateCnt > 0)
			{
				SaveReplayData(theBuffer, mWholeGameReplay);
			}
			theBuffer.WriteValuePair(Serialiser.PairID.BoardNextPieceId, mNextPieceId);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardScrambleDelayTicks, mScrambleDelayTicks);
			theBuffer.WriteValuePair(Serialiser.PairID.BoardGameFinished, mGameFinished);
			theBuffer.FinalizeGameChunkHeader(chunkBeginLoc);
			bool result = SaveGameExtra(theBuffer);
			theBuffer.FinalizeFileHeader();
			return result;
		}

		public virtual bool LoadGame(Serialiser theBuffer)
		{
			return LoadGame(theBuffer, true);
		}

		public virtual bool LoadGame(Serialiser theBuffer, bool resetReplay)
		{
			mInLoadSave = true;
			theBuffer.SeekFront();
			int GameVersion;
			int BoardVersion;
			int platform;
			if (!theBuffer.ReadFileHeader(out GameVersion, out BoardVersion, out platform))
			{
				mInLoadSave = false;
				return false;
			}
			int chunkBeginPos = 0;
			GameChunkHeader header = new GameChunkHeader();
			if (!theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkBoard, header, out chunkBeginPos))
			{
				mInLoadSave = false;
				return false;
			}
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					if (piece != null)
					{
						piece.mTallied = true;
						DeletePiece(piece);
					}
				}
			}
			theBuffer.ReadValuePair(out mUpdateCnt);
			int num = 0;
			mPreFXManager.Clear();
			mPostFXManager.Clear();
			Piece piece2 = null;
			for (int k = 0; k < 8; k++)
			{
				for (int l = 0; l < 8; l++)
				{
					if ((piece2 = mBoard[k, l]) != null)
					{
						piece2.release();
						mBoard[k, l] = null;
					}
				}
			}
			int num2;
			theBuffer.ReadSpecialBlock(out num2);
			for (int m = 0; m < num2; m++)
			{
				if (theBuffer.ReadBoolean())
				{
					int num3 = theBuffer.ReadInt32();
					int num4 = theBuffer.ReadInt32();
					int id = theBuffer.ReadInt32();
					piece2 = Piece.alloc(this, id);
					num = Math.Max(num, piece2.mId);
					piece2.mCol = num4;
					piece2.mRow = num3;
					piece2.Load(theBuffer, GameVersion);
					mBoard[num3, num4] = piece2;
					StartPieceEffect(piece2);
				}
			}
			mBumpVelocities = new float[8];
			theBuffer.ReadArrayPair(8, mBumpVelocities);
			mNextColumnCredit = new int[8];
			theBuffer.ReadArrayPair(8, mNextColumnCredit);
			string str;
			theBuffer.ReadStringPair(out str);
			mRand.SRand(str);
			int num5;
			theBuffer.ReadSpecialBlock(out num5);
			LoadSwapData(theBuffer, mSwapDataVector, num5);
			theBuffer.ReadSpecialBlock(out num5);
			LoadMoveData(theBuffer, mMoveDataVector, num5, GameVersion);
			theBuffer.ReadSpecialBlock(out num5);
			LoadQueuedMoves(theBuffer, mQueuedMoveVector, num5);
			mNextPieceId = num + 1;
			mGameStats = new int[40];
			theBuffer.ReadArrayPair(40, mGameStats);
			theBuffer.ReadValuePair(out mPoints);
			mDispPoints = mPoints;
			int num6;
			theBuffer.ReadSpecialBlock(out num5, out num6);
			mPointsBreakdown = new List<List<int>>();
			for (int n = 0; n < num5; n++)
			{
				mPointsBreakdown.Add(new List<int>());
				for (int num7 = 0; num7 < 5; num7++)
				{
					mPointsBreakdown[n].Add(0);
				}
			}
			for (int num8 = 0; num8 < num5; num8++)
			{
				for (int num9 = 0; num9 < num6; num9++)
				{
					mPointsBreakdown[num8][num9] = (int)theBuffer.ReadLong();
				}
			}
			theBuffer.ReadValuePair(out mMoneyDisp);
			mMoneyDispGoal = mMoneyDisp;
			theBuffer.ReadValuePair(out mLevelBarPct);
			theBuffer.ReadValuePair(out mCountdownBarPct);
			theBuffer.ReadValuePair(out mLevelPointsTotal);
			theBuffer.ReadValuePair(out mLevel);
			theBuffer.ReadValuePair(out mPointMultiplier);
			theBuffer.ReadValuePair(out mCurMoveCreditId);
			theBuffer.ReadValuePair(out mCurMatchId);
			theBuffer.ReadValuePair(out mGemFallDelay);
			theBuffer.ReadValuePair(out mMoveCounter);
			theBuffer.ReadValuePair(out mGameTicks);
			theBuffer.ReadValuePair(out mIdleTicks);
			theBuffer.ReadValuePair(out mLastMatchTick);
			theBuffer.ReadValuePair(out mLastMatchTime);
			theBuffer.ReadValuePair(out mMatchTallyCount);
			theBuffer.ReadValuePair(out mSpeedModeFactor);
			theBuffer.ReadValuePair(out mSpeedBonusAlpha);
			theBuffer.ReadStringPair(out mSpeedBonusText);
			theBuffer.ReadValuePair(out mSpeedometerPopup);
			theBuffer.ReadValuePair(out mSpeedometerGlow);
			theBuffer.ReadValuePair(out mSpeedBonusDisp);
			theBuffer.ReadValuePair(out mSpeedNeedle);
			theBuffer.ReadValuePair(out mSpeedBonusPoints);
			theBuffer.ReadValuePair(out mSpeedBonusNum);
			theBuffer.ReadValuePair(out mSpeedBonusCount);
			theBuffer.ReadValuePair(out mSpeedBonusCountHighest);
			theBuffer.ReadValuePair(out mSpeedBonusLastCount);
			theBuffer.ReadValuePair(out mHasBoardSettled);
			mComboColors = new int[5];
			theBuffer.ReadArrayPair(5, mComboColors);
			theBuffer.ReadValuePair(out mComboCount);
			theBuffer.ReadValuePair(out mComboLen);
			theBuffer.ReadValuePair(out mComboCountDisp);
			theBuffer.ReadValuePair(out mComboFlashPct);
			theBuffer.ReadValuePair(out mLastPlayerSwapColor);
			if (mUpdateCnt > 0)
			{
				LoadReplayData(theBuffer, mWholeGameReplay);
			}
			else
			{
				mWholeGameReplay.mReplayMoves.Clear();
			}
			while (mLightningStorms.Count > 0)
			{
				if (mLightningStorms[mLightningStorms.Count - 1] != null)
				{
					mLightningStorms[mLightningStorms.Count - 1].Dispose();
				}
				mLightningStorms.RemoveAt(mLightningStorms.Count - 1);
			}
			while (mAnnouncements.Count > 0)
			{
				if (mAnnouncements[mAnnouncements.Count - 1] != null)
				{
					mAnnouncements[mAnnouncements.Count - 1].Dispose();
				}
				mAnnouncements.RemoveAt(mAnnouncements.Count - 1);
			}
			mPrevPointMultAlpha.SetConstant(0.0);
			mPointMultPosPct.SetConstant(1.0);
			mPointMultTextMorph.SetConstant(0.0);
			mPointMultScale.SetConstant(1.0);
			mPointMultAlpha.SetConstant(0.0);
			mPointMultYAdd.SetConstant(0.0);
			mTimerInflate.SetConstant(0.0);
			mTimerAlpha.SetConstant(1.0);
			mGemCountValueDisp = 0;
			mGemCountValueCheck = 0;
			mGemCountAlpha.SetConstant(0.0);
			mGemScalarAlpha.SetConstant(0.0);
			mGemCountCurve.SetConstant(0.0);
			mCascadeCountValueDisp = 0;
			mCascadeCountValueCheck = 0;
			mCascadeCountAlpha.SetConstant(0.0);
			mCascadeScalarAlpha.SetConstant(0.0);
			mCascadeCountCurve.SetConstant(0.0);
			mComplementAlpha.SetConstant(0.0);
			mComplementScale.SetConstant(0.0);
			mComplementNum = -1;
			mLastComplement = -1;
			mSideXOff.SetConstant(0.0);
			mSideAlpha.SetConstant(1.0);
			mScale.SetConstant(1.0);
			mAlpha.SetConstant(1.0);
			int theValue;
			theBuffer.ReadValuePair(out theValue);
			mNextPieceId = Math.Max(theValue, num + 1);
			theBuffer.ReadValuePair(out mScrambleDelayTicks);
			theBuffer.ReadValuePair(out mGameFinished);
			mWantHintTicks = 0;
			mDeferredTutorialVector.Clear();
			GlobalMembers.KILL_WIDGET(mHyperspace);
			mHyperspace = null;
			mWantLevelup = false;
			LoadGameExtra(theBuffer);
			if (resetReplay)
			{
				HideReplayWidget();
				mReplayWidgetShowPct.SetConstant(0.0);
				mHasReplayData = false;
			}
			GlobalMembers.gApp.LogStatString($"GameLoaded Title=\"{GetLoggingGameName()}\" Level={mLevel} Misc.Points={mPoints}");
			mLevelBarPIEffect.Update();
			if (WantsLevelBasedBackground() && mBackgroundIdx != mLevel)
			{
				mBackgroundIdx = mLevel - 1;
				SetupBackground(1);
			}
			mInLoadSave = false;
			return true;
		}

		public bool LoadGame()
		{
			string savedGameName = GetSavedGameName();
			if (string.IsNullOrEmpty(savedGameName))
			{
				return false;
			}
			Serialiser theBuffer = new Serialiser();
			if (!GlobalMembers.gApp.ReadBufferFromStorage(GlobalMembers.gApp.mProfile.GetProfileDir(GlobalMembers.gApp.mProfile.mProfileName) + "\\" + savedGameName, theBuffer))
			{
				return false;
			}
			return LoadGame(theBuffer);
		}

		public bool HasSavedGame()
		{
			string savedGameName = GetSavedGameName();
			if (string.IsNullOrEmpty(savedGameName))
			{
				return false;
			}
			return BejeweledLivePlus.Misc.Common.FileExists(GlobalMembers.gApp.mProfile.GetProfileDir(GlobalMembers.gApp.mProfile.mProfileName) + "\\" + savedGameName);
		}

		public bool DeleteSavedGame()
		{
			string savedGameName = GetSavedGameName();
			if (string.IsNullOrEmpty(savedGameName))
			{
				return false;
			}
			return GlobalMembers.gApp.mFileDriver.DeleteFile(GlobalMembers.gApp.mProfile.GetProfileDir(GlobalMembers.gApp.mProfile.mProfileName) + "\\" + savedGameName);
		}

		public bool SaveGame()
		{
			mInLoadSave = true;
			string savedGameName = GetSavedGameName();
			if (string.IsNullOrEmpty(savedGameName))
			{
				mInLoadSave = false;
				return false;
			}
			Serialiser theBuffer = new Serialiser();
			if (!SafeSaveGame(theBuffer))
			{
				mInLoadSave = false;
				return false;
			}
			bool result = GlobalMembers.gApp.WriteBufferToFile(GlobalMembers.gApp.mProfile.GetProfileDir(GlobalMembers.gApp.mProfile.mProfileName) + "\\" + savedGameName, theBuffer);
			mInLoadSave = false;
			return result;
		}

		public virtual bool SaveGameExtra(Serialiser theBuffer)
		{
			return true;
		}

		public virtual void LoadGameExtra(Serialiser theBuffer)
		{
		}

		public bool SaveSwapData(Serialiser theBuffer, List<SwapData> theSwapDataVector)
		{
			for (int i = 0; i < theSwapDataVector.Count; i++)
			{
				SwapData swapData = theSwapDataVector[i];
				theBuffer.WriteInt32((swapData.mPiece1 != null) ? swapData.mPiece1.mId : 0);
				theBuffer.WriteInt32((swapData.mPiece2 != null) ? swapData.mPiece2.mId : 0);
				theBuffer.WriteInt32(swapData.mSwapDir.mX);
				theBuffer.WriteInt32(swapData.mSwapDir.mY);
				theBuffer.WriteCurvedVal(swapData.mSwapPct);
				theBuffer.WriteCurvedVal(swapData.mGemScale);
				theBuffer.WriteBoolean(swapData.mForwardSwap);
				theBuffer.WriteInt32(swapData.mHoldingSwap);
				theBuffer.WriteBoolean(swapData.mIgnore);
				theBuffer.WriteBoolean(swapData.mForceSwap);
				theBuffer.WriteBoolean(swapData.mDestroyTarget);
			}
			return true;
		}

		public bool LoadSwapData(Serialiser theBuffer, List<SwapData> theSwapDataVector, int size)
		{
			theSwapDataVector.Clear();
			for (int i = 0; i < size; i++)
			{
				SwapData swapData = new SwapData();
				swapData.mPiece1 = GetPieceById(theBuffer.ReadInt32());
				swapData.mPiece2 = GetPieceById(theBuffer.ReadInt32());
				swapData.mSwapDir.mX = theBuffer.ReadInt32();
				swapData.mSwapDir.mY = theBuffer.ReadInt32();
				swapData.mSwapPct = theBuffer.ReadCurvedVal();
				swapData.mGemScale = theBuffer.ReadCurvedVal();
				swapData.mForwardSwap = theBuffer.ReadBoolean();
				swapData.mHoldingSwap = theBuffer.ReadInt32();
				swapData.mIgnore = theBuffer.ReadBoolean();
				swapData.mForceSwap = theBuffer.ReadBoolean();
				swapData.mDestroyTarget = theBuffer.ReadBoolean();
				theSwapDataVector.Add(swapData);
			}
			return true;
		}

		public bool SaveMoveData(Serialiser theBuffer, List<MoveData> theMoveDataVector, int saveGameVersion)
		{
			for (int i = 0; i < (short)theMoveDataVector.Count; i++)
			{
				MoveData moveData = theMoveDataVector[i];
				theBuffer.WriteInt32(moveData.mUpdateCnt);
				theBuffer.WriteInt32(moveData.mSelectedId);
				theBuffer.WriteInt32(moveData.mSwappedRow);
				theBuffer.WriteInt32(moveData.mSwappedCol);
				theBuffer.WriteInt32(moveData.mMoveCreditId);
				if (saveGameVersion > 102)
				{
					theBuffer.WriteInt32(40);
				}
				for (int j = 0; j < 40; j++)
				{
					theBuffer.WriteInt32(moveData.mStats[j]);
				}
			}
			return true;
		}

		public bool LoadMoveData(Serialiser theBuffer, List<MoveData> theMoveDataVector, int size, int saveGameVersion)
		{
			theMoveDataVector.Clear();
			for (int i = 0; i < size; i++)
			{
				MoveData moveData = new MoveData();
				moveData.mUpdateCnt = theBuffer.ReadInt32();
				moveData.mSelectedId = theBuffer.ReadInt32();
				moveData.mSwappedRow = theBuffer.ReadInt32();
				moveData.mSwappedCol = theBuffer.ReadInt32();
				moveData.mMoveCreditId = theBuffer.ReadInt32();
				if (saveGameVersion > 102)
				{
					int num = theBuffer.ReadInt32();
					for (int j = 0; j < num; j++)
					{
						moveData.mStats[j] = theBuffer.ReadInt32();
					}
					for (int k = num; k < 40; k++)
					{
						moveData.mStats[k] = 0;
					}
				}
				else
				{
					for (int l = 0; l < 38; l++)
					{
						moveData.mStats[l] = theBuffer.ReadInt32();
					}
					for (int m = 38; m < 40; m++)
					{
						moveData.mStats[m] = 0;
					}
				}
				theMoveDataVector.Add(moveData);
			}
			return true;
		}

		public bool SaveQueuedMoves(SexyFramework.Misc.Buffer theBuffer, List<QueuedMove> theQueuedMoves)
		{
			for (int i = 0; i < theQueuedMoves.Count; i++)
			{
				QueuedMove queuedMove = theQueuedMoves[i];
				theBuffer.WriteInt32(queuedMove.mUpdateCnt);
				theBuffer.WriteInt32(queuedMove.mSelectedId);
				theBuffer.WriteInt32(queuedMove.mSwappedCol);
				theBuffer.WriteInt32(queuedMove.mSwappedRow);
				theBuffer.WriteBoolean(queuedMove.mForceSwap);
				theBuffer.WriteBoolean(queuedMove.mPlayerSwapped);
				theBuffer.WriteBoolean(queuedMove.mDestroyTarget);
			}
			return true;
		}

		public bool LoadQueuedMoves(SexyFramework.Misc.Buffer theBuffer, List<QueuedMove> theQueuedMoves, int size)
		{
			theQueuedMoves.Clear();
			for (int i = 0; i < size; i++)
			{
				QueuedMove queuedMove = new QueuedMove();
				queuedMove.mUpdateCnt = theBuffer.ReadInt32();
				queuedMove.mSelectedId = theBuffer.ReadInt32();
				queuedMove.mSwappedCol = theBuffer.ReadInt32();
				queuedMove.mSwappedRow = theBuffer.ReadInt32();
				queuedMove.mForceSwap = theBuffer.ReadBoolean();
				queuedMove.mPlayerSwapped = theBuffer.ReadBoolean();
				queuedMove.mDestroyTarget = theBuffer.ReadBoolean();
				theQueuedMoves.Add(queuedMove);
			}
			return true;
		}

		public bool SaveReplayData(Serialiser theBuffer, ReplayData theReplayData)
		{
			theBuffer.WriteValuePair(Serialiser.PairID.ReplayVersion, 1);
			string str = "BlitzDeluxe";
			theBuffer.WriteStringPair(Serialiser.PairID.ReplayID, str);
			theBuffer.WriteBufferPair(Serialiser.PairID.ReplaySaveBuffer, theReplayData.mSaveBuffer);
			if (theReplayData.mSaveBuffer.GetDataLen() == 0)
			{
				return false;
			}
			theBuffer.WriteSpecialBlock(Serialiser.PairID.ReplayQueuedMoves, theReplayData.mReplayMoves.Count);
			SaveQueuedMoves(theBuffer, theReplayData.mReplayMoves);
			theBuffer.WriteValuePair(Serialiser.PairID.ReplayTutorialFlags, theReplayData.mTutorialFlags);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.ReplayStateInfo, theReplayData.mStateInfoVector.Count);
			for (int i = 0; i < theReplayData.mStateInfoVector.Count; i++)
			{
				StateInfo stateInfo = theReplayData.mStateInfoVector[i];
				theBuffer.WriteInt32(stateInfo.mUpdateCnt);
				theBuffer.WriteInt32(stateInfo.mPoints);
				theBuffer.WriteInt32(stateInfo.mMoneyDisp);
				theBuffer.WriteInt32(stateInfo.mNextPieceId);
				theBuffer.WriteInt32(stateInfo.mIdleTicks);
			}
			theBuffer.WriteValuePair(Serialiser.PairID.ReplayTicks, theReplayData.mReplayTicks);
			return true;
		}

		public bool LoadReplayData(Serialiser theBuffer, ReplayData theReplayData)
		{
			int theValue;
			theBuffer.ReadValuePair(out theValue);
			if (theValue < 1 || theValue > 1)
			{
				return false;
			}
			string str;
			theBuffer.ReadStringPair(out str);
			SexyFramework.Misc.Buffer theBuffer2;
			theBuffer.ReadBufferPair(out theBuffer2);
			theReplayData.mSaveBuffer.SetData(theBuffer2.mData);
			if (theReplayData.mSaveBuffer.GetDataLen() == 0)
			{
				return false;
			}
			int GameVersion;
			int BoardVersion;
			int platform;
			bool flag = theReplayData.mSaveBuffer.ReadFileHeader(out GameVersion, out BoardVersion, out platform);
			theReplayData.mSaveBuffer.SeekFront();
			if (!flag || GameVersion < 101 || GameVersion > 103)
			{
				return false;
			}
			int theValue2;
			theBuffer.ReadValuePair(out theValue2);
			LoadQueuedMoves(theBuffer, theReplayData.mReplayMoves, theValue2);
			theBuffer.ReadValuePair(out theReplayData.mTutorialFlags);
			theReplayData.mStateInfoVector.Clear();
			theBuffer.ReadSpecialBlock(out theValue2);
			for (int i = 0; i < theValue2; i++)
			{
				StateInfo stateInfo = new StateInfo();
				stateInfo.mUpdateCnt = theBuffer.ReadInt32();
				stateInfo.mPoints = theBuffer.ReadInt32();
				stateInfo.mMoneyDisp = theBuffer.ReadInt32();
				stateInfo.mNextPieceId = theBuffer.ReadInt32();
				stateInfo.mIdleTicks = theBuffer.ReadInt32();
				theReplayData.mStateInfoVector.Add(stateInfo);
			}
			theBuffer.ReadValuePair(out theReplayData.mReplayTicks);
			return true;
		}

		public bool SaveReplay(ReplayData theReplayData)
		{
			if (mReplayStartMove.mPreSaveBuffer.GetDataLen() == 0)
			{
				return false;
			}
			theReplayData.mSaveBuffer = mReplayStartMove.mPreSaveBuffer;
			theReplayData.mReplayMoves = mQueuedMoveVector;
			theReplayData.mTutorialFlags = (ulong)mTutorialFlags;
			theReplayData.mReplayTicks = 0;
			return true;
		}

		public void LoadReplay(ReplayData theReplayData)
		{
			mHadReplayError = false;
			theReplayData.mSaveBuffer.SeekFront();
			if (!LoadGame(theReplayData.mSaveBuffer))
			{
				mHadReplayError = true;
			}
			mQueuedMoveVector = theReplayData.mReplayMoves;
			mTutorialFlags = (int)theReplayData.mTutorialFlags;
			mStateInfoVector = theReplayData.mStateInfoVector;
		}

		public bool SafeSaveGame(Serialiser theBuffer)
		{
			if (!SaveGame(theBuffer))
			{
				if (mMoveDataVector.Count > 0)
				{
					theBuffer.Copyfrom(mMoveDataVector[mMoveDataVector.Count - 1].mPreSaveBuffer);
				}
				else
				{
					if (mLastMoveSave.GetDataLen() <= 0)
					{
						return false;
					}
					theBuffer.Copyfrom(mLastMoveSave);
				}
			}
			return true;
		}

		public void RewindToReplay(ReplayData theReplayData)
		{
			Serialiser serialiser = new Serialiser();
			SafeSaveGame(serialiser);
			mPreReplaySave.Copyfrom(serialiser);
			if (mRewindSound == null)
			{
				mRewindSound = SexyFramework.GlobalMembers.gSexyApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_REWIND);
			}
			if (mRewindSound != null && GlobalMembers.gApp.mMuteCount <= 0)
			{
				mRewindSound.SetVolume((GlobalMembers.gApp.mMuteCount > 0) ? 0.0 : GlobalMembers.gApp.mSfxVolume);
				mRewindSound.Play(true, false);
			}
			mInReplay = true;
			mRewinding = true;
			mPlaybackTimestamp = GlobalMembers.gGR.GetLastOperationTimestamp();
			((PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
			DisableUI(true);
			LoadReplay(theReplayData);
			if (mHadReplayError)
			{
				mHadReplayError = false;
			}
		}

		public void DoReplaySetup()
		{
			mWantReplaySave = false;
			if (!SupportsReplays() || mReplayStartMove.mPartOfReplay)
			{
				return;
			}
			HideReplayWidget();
			if (SaveReplay(mCurReplayData))
			{
				mReplayStartMove.mPartOfReplay = true;
				GlobalMembers.gGR.ClearOperationsTo(mReplayStartMove.mUpdateCnt - 1);
				mHasReplayData = true;
				if (!mInReplay)
				{
					mWatchedCurReplay = false;
					mReplayIgnoredMoves = 0;
					mReplayHadIgnoredMoves = false;
				}
				if (!mWantLevelup && mHyperspace == null)
				{
					ShowReplayWidget();
				}
			}
		}

		public void HideReplayWidget()
		{
			if ((double)mReplayWidgetShowPct > 0.0)
			{
				mReplayButton.mMouseVisible = false;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_REPLAY_WIDGET_HIDE_PCT, mReplayWidgetShowPct);
			}
		}

		public void ShowReplayWidget()
		{
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_REPLAY_WIDGET_SHOW_PCT, mReplayWidgetShowPct);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_REPLAY_POPUP);
			mReplayButton.mMouseVisible = true;
		}

		public bool LoadUReplay(SexyFramework.Misc.Buffer theBuffer)
		{
			int num = (int)theBuffer.ReadLong();
			if (num != 1354408503)
			{
				return false;
			}
			int num2 = (int)theBuffer.ReadLong();
			if (num2 > 3 || num2 < 1)
			{
				return false;
			}
			mUReplayVersion = num2;
			theBuffer.ReadLong();
			mBoostsEnabled = (int)theBuffer.ReadLong();
			if (num2 >= 2)
			{
				mUReplayTotalTicks = (int)theBuffer.ReadLong();
			}
			else
			{
				mUReplayTotalTicks = 0;
			}
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					piece.release();
					mBoard[i, j] = null;
				}
			}
			int num3 = (int)theBuffer.ReadLong();
			theBuffer.ReadLong();
			byte[] array = new byte[num3];
			byte[] dataPtr = theBuffer.GetDataPtr();
			for (int k = 0; k < num3; k++)
			{
				array[k] = dataPtr[k + theBuffer.mReadBitPos / 8];
			}
			mUReplayBuffer.Clear();
			mUReplayBuffer.WriteBytes(array, num3);
			mUReplayTicksLeft = 6000;
			mInUReplay = true;
			array = null;
			return true;
		}

		public bool SaveUReplay(ref SexyFramework.Misc.Buffer theBuffer)
		{
			theBuffer.WriteLong(1354408503L);
			theBuffer.WriteLong(3L);
			theBuffer.WriteLong(1L);
			theBuffer.WriteLong(mBoostsEnabled);
			theBuffer.WriteLong(mUpdateCnt);
			theBuffer.WriteLong(mUReplayBuffer.GetDataLen());
			theBuffer.WriteLong(mUReplayBuffer.GetDataLen());
			theBuffer.WriteBytes(mUReplayBuffer.GetDataPtr(), mUReplayBuffer.GetDataLen());
			return true;
		}

		public virtual void BoardSettled()
		{
		}

		public virtual void DialogClosed(int theId)
		{
		}

		public void TallyCoin(Piece thePiece)
		{
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_TREASUREFIND);
			int[] array = new int[5]
			{
				GlobalMembers.M(250),
				GlobalMembers.M(500),
				GlobalMembers.M(1000),
				GlobalMembers.M(2500),
				GlobalMembers.M(5000)
			};
			if (!thePiece.mDestructing)
			{
				AddPoints((int)thePiece.CX(), (int)thePiece.CY(), array[0], GlobalMembers.gGemColors[thePiece.mColor], (uint)thePiece.mMatchId);
			}
			mPendingCoinAnimations++;
			mCoinCatcherAppearing = true;
			int[] array2 = new int[4] { 10, 25, 50, 100 };
			if (!mInReplay)
			{
				mCoinsEarned += array2[thePiece.mCounter];
			}
		}

		public virtual void TallyPiece(Piece thePiece, bool thePieceDestroyed)
		{
			if (thePiece.mTallied)
			{
				return;
			}
			thePiece.mTallied = true;
			PieceTallied(thePiece);
			if (!thePieceDestroyed)
			{
				return;
			}
			if (!thePiece.IsFlagSet(65536u))
			{
				AddToStat(4, 1, thePiece.mMoveCreditId);
				if (thePiece.mColor > -1)
				{
					AddToStat(5 + thePiece.mColor, 1, thePiece.mMoveCreditId);
				}
				if (thePiece.mMoveCreditId != -1)
				{
					MaxStat(25, GetMoveStat(thePiece.mMoveCreditId, 1));
					MaxStat(33, GetMoveStat(thePiece.mMoveCreditId, 4));
					UpdateSpecialGemsStats(thePiece.mMoveCreditId);
				}
			}
			if (thePiece.IsFlagSet(16u))
			{
				IncPointMult(thePiece);
			}
		}

		public virtual void PieceTallied(Piece thePiece)
		{
			if (thePiece.IsFlagSet(1024u))
			{
				thePiece.mAlpha.SetConstant(0.0);
				TallyCoin(thePiece);
			}
			if (thePiece.IsFlagSet(512u) && mGameOverPiece == null)
			{
				int num = (int)thePiece.CX();
				int num2 = (int)thePiece.CY();
				Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_LIGHT);
				effect.mFlags = 2;
				effect.mX = num;
				effect.mY = num2;
				effect.mZ = GlobalMembers.M(0.08f);
				effect.mValue[0] = GlobalMembers.M(45.1f);
				effect.mValue[1] = GlobalMembers.M(-0.5f);
				effect.mAlpha = GlobalMembers.M(0.3f);
				effect.mDAlpha = GlobalMembers.M(0.06f);
				effect.mScale = GlobalMembers.M(300f);
				mPostFXManager.AddEffect(effect);
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_GEM_COUNTDOWN_DESTROYED);
				if (!mIsOneMoveReplay)
				{
					mPostFXManager.AddSteamExplosion(num, num2, GlobalMembers.gGemColors[thePiece.mColor]);
				}
				BumpColumn(thePiece, GlobalMembers.M(2f));
				for (int i = 0; i < GlobalMembers.M(12); i++)
				{
					Effect effect2 = mPostFXManager.AllocEffect(Effect.Type.TYPE_COUNTDOWN_SHARD);
					effect2.mColor = GlobalMembers.gGemColors[thePiece.mColor];
					effect2.mX = num;
					effect2.mY = num2;
					mPostFXManager.AddEffect(effect2);
				}
			}
		}

		public void AddToStat(int theStatNum, int theNumber, int theMoveCreditId)
		{
			AddToStat(theStatNum, theNumber, theMoveCreditId, true);
		}

		public void AddToStat(int theStatNum, int theNumber)
		{
			AddToStat(theStatNum, theNumber, -1, true);
		}

		public void AddToStat(int theStatNum)
		{
			AddToStat(theStatNum, 1, -1, true);
		}

		public void AddToStat(int theStatNum, int theNumber, int theMoveCreditId, bool addToProfile)
		{
			mGameStats[theStatNum] += theNumber;
			if (mGameStats[theStatNum] < 0)
			{
				mGameStats[theStatNum] = int.MaxValue;
			}
			if (!mIsWholeGameReplay && !mInReplay && addToProfile)
			{
				GlobalMembers.gApp.mProfile.mStats[theStatNum] += theNumber;
				if (GlobalMembers.gApp.mProfile.mStats[theStatNum] < 0)
				{
					GlobalMembers.gApp.mProfile.mStats[theStatNum] = int.MaxValue;
				}
			}
			if (theMoveCreditId == -1)
			{
				return;
			}
			for (int i = 0; i < mMoveDataVector.Count; i++)
			{
				if (mMoveDataVector[i].mMoveCreditId == theMoveCreditId)
				{
					mMoveDataVector[i].mStats[theStatNum] += theNumber;
				}
			}
		}

		public void MaxStat(int theStatNum, int theNumber)
		{
			MaxStat(theStatNum, theNumber, -1);
		}

		public void MaxStat(int theStatNum, int theNumber, int theMoveCreditId)
		{
			mGameStats[theStatNum] = Math.Max(mGameStats[theStatNum], theNumber);
			if (!mIsWholeGameReplay)
			{
				GlobalMembers.gApp.mProfile.mStats[theStatNum] = Math.Max(GlobalMembers.gApp.mProfile.mStats[theStatNum], theNumber);
			}
			if (theMoveCreditId == -1)
			{
				return;
			}
			for (int i = 0; i < mMoveDataVector.Count; i++)
			{
				if (mMoveDataVector[i].mMoveCreditId == theMoveCreditId)
				{
					mMoveDataVector[i].mStats[theStatNum] = Math.Max(mMoveDataVector[i].mStats[theStatNum], theNumber);
				}
			}
		}

		public int GetMoveStat(int theMoveCreditId, int theStatNum)
		{
			return GetMoveStat(theMoveCreditId, theStatNum, 0);
		}

		public int GetMoveStat(int theMoveCreditId, int theStatNum, int theDefault)
		{
			for (int i = 0; i < mMoveDataVector.Count; i++)
			{
				if (mMoveDataVector[i].mMoveCreditId == theMoveCreditId)
				{
					return mMoveDataVector[i].mStats[theStatNum];
				}
			}
			return theDefault;
		}

		public int GetTotalMovesStat(int theStatNum)
		{
			int num = 0;
			for (int i = 0; i < mMoveDataVector.Count; i++)
			{
				num += mMoveDataVector[i].mStats[theStatNum];
			}
			return num;
		}

		public int GetMaxMovesStat(int theStatNum)
		{
			int num = 0;
			for (int i = 0; i < mMoveDataVector.Count; i++)
			{
				if (i == 0 || mMoveDataVector[i].mStats[theStatNum] > num)
				{
					num = mMoveDataVector[i].mStats[theStatNum];
				}
			}
			return num;
		}

		public void UpdateDeferredSounds()
		{
			if (mDeferredSounds.Count == 0)
			{
				return;
			}
			for (int i = 0; i < mDeferredSounds.Count; i++)
			{
				if (mGameTicks >= mDeferredSounds[i].mOnGameTick)
				{
					GlobalMembers.gApp.PlaySample(mDeferredSounds[i].mId, 0, mDeferredSounds[i].mVolume);
					mDeferredSounds.RemoveAt(i);
					i--;
				}
			}
		}

		public void AddDeferredSound(int theSoundId, int theDelayGameTicks)
		{
			AddDeferredSound(theSoundId, theDelayGameTicks, 1.0);
		}

		public void AddDeferredSound(int theSoundId, int theDelayGameTicks, double theVol)
		{
			DeferredSound deferredSound = new DeferredSound();
			deferredSound.mId = theSoundId;
			deferredSound.mOnGameTick = mGameTicks + theDelayGameTicks;
			deferredSound.mVolume = theVol;
			mDeferredSounds.Add(deferredSound);
		}

		public void DoSpeedText(int anIndex)
		{
			WriteUReplayCmd(9);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_FLAMEBONUS);
			GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_BLAZINGSPEED, 0, 1.0, -2);
			DoComplement(GlobalMembers.gComplementCount);
			PIEffect pIEffect = GlobalMembersResourcesWP.PIEFFECT_SPEEDBOARD_FLAME.Duplicate();
			pIEffect.mDrawTransform.Translate(ConstantsWP.SPEEDBOARD_BLAZING_SPEED_EFFECT_1_X, ConstantsWP.SPEEDBOARD_BLAZING_SPEED_EFFECT_1_Y);
			pIEffect.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
			mSpeedFireBarPIEffect[0] = pIEffect;
			pIEffect = GlobalMembersResourcesWP.PIEFFECT_SPEEDBOARD_FLAME.Duplicate();
			pIEffect.mDrawTransform.Translate(ConstantsWP.SPEEDBOARD_BLAZING_SPEED_EFFECT_2_X, ConstantsWP.SPEEDBOARD_BLAZING_SPEED_EFFECT_2_Y);
			pIEffect.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
			mSpeedFireBarPIEffect[1] = pIEffect;
			mSpeedBonusFlameModePct = 1f;
			AddToStat(23);
		}

		public virtual void DoComplement(int theComplementNum)
		{
			Announcement announcement = null;
			switch (GlobalMembers.gComplements[theComplementNum])
			{
			case 1746:
				announcement = new Announcement(this, GlobalMembers._ID("GOOD", 3561));
				break;
			case 1741:
				announcement = new Announcement(this, GlobalMembers._ID("EXCELLENT", 3562));
				break;
			case 1738:
				announcement = new Announcement(this, GlobalMembers._ID("AWESOME", 3563));
				break;
			case 1750:
				announcement = new Announcement(this, GlobalMembers._ID("SPECTACULAR", 3564));
				break;
			case 1742:
				announcement = new Announcement(this, GlobalMembers._ID("EXTRAORDINARY", 3565));
				break;
			case 1753:
				announcement = new Announcement(this, GlobalMembers._ID("UNBELIEVABLE", 3566));
				break;
			case 1739:
				announcement = new Announcement(this, GlobalMembers._ID("BLAZINGSPEED", 3567));
				break;
			}
			announcement.mBlocksPlay = false;
			announcement.mAlpha.mIncRate *= GlobalMembers.M(3.0);
			announcement.mScale.mIncRate *= GlobalMembers.M(3.0);
			announcement.mDarkenBoard = false;
			announcement.mGoAnnouncement = true;
			GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.GetSoundById(GlobalMembers.gComplements[theComplementNum]));
			mComplementNum = theComplementNum;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_COMPLEMENT_ALPHA, mComplementAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_COMPLEMENT_SCALE, mComplementScale, mComplementAlpha);
			mLastComplement = theComplementNum;
		}

		public virtual void NewHyperMixer()
		{
		}

		public virtual void NewCombo()
		{
			if (!WantColorCombos())
			{
				return;
			}
			mComboCountDisp = 0f;
			mComboCount = 0;
			mLastComboCount = 0;
			bool flag;
			do
			{
				flag = true;
				int[] array = new int[1];
				int[] array2 = array;
				for (int i = 0; i < mComboLen; i++)
				{
					mComboColors[i] = (int)(mRand.Next() % 7);
					if (++array2[mComboColors[i]] >= 3)
					{
						flag = false;
					}
				}
			}
			while (!flag);
		}

		public virtual bool ComboProcess(int theColor)
		{
			if (!WantColorCombos())
			{
				return false;
			}
			if (mComboColors[mComboCount] == theColor)
			{
				mComboCount++;
				if (mComboCount == mComboLen)
				{
					ComboCompleted();
				}
				return true;
			}
			if (mComboCount > 0 && mComboColors[mComboCount - 1] == theColor && mComboCount == mLastComboCount)
			{
				mLastComboCount = mComboCount - 1;
			}
			return false;
		}

		public virtual void ComboFailed()
		{
			if (WantColorCombos())
			{
				mComboCount = Math.Max(0, mComboCount - 1);
			}
		}

		public virtual void ComboCompleted()
		{
			if (WantColorCombos())
			{
				mComboCount = 0;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_COMBO_FLASH_PCT, mComboFlashPct);
			}
		}

		public virtual void DepositCoin()
		{
			if (!mIsWholeGameReplay && !mInUReplay)
			{
				mMoneyDispGoal += 100;
			}
			if (mCoinCatcherPctPct >= 1.0)
			{
				mCoinCatcherPctPct = 4.0;
			}
			mPendingCoinAnimations--;
		}

		public virtual void CoinBalanceChanged()
		{
			mMoneyDisp = (mMoneyDispGoal = GlobalMembers.gApp.GetCoinCount());
		}

		public int GetPanPosition(int theX)
		{
			int num = GetColX(7) + 100;
			int num2 = theX - (GetBoardX() + num / 2);
			double num3 = (double)num2 / (double)num * 2.0;
			return (int)(num3 * (double)GlobalMembers.M(800));
		}

		public int GetPanPosition(Piece thePiece)
		{
			if (thePiece == null)
			{
				return 0;
			}
			return GetPanPosition((int)(thePiece.GetScreenX() + 50f));
		}

		public Points DoAddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd)
		{
			return DoAddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, 1);
		}

		public Points DoAddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId)
		{
			return DoAddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, false, 1);
		}

		public Points DoAddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier)
		{
			return DoAddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, -1, false, 1);
		}

		public Points DoAddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube)
		{
			return DoAddPoints(theX, theY, thePoints, theColor, theId, addtotube, true, -1, false, 1);
		}

		public Points DoAddPoints(int theX, int theY, int thePoints, Color theColor, uint theId)
		{
			return DoAddPoints(theX, theY, thePoints, theColor, theId, true, true, -1, false, 1);
		}

		public Points DoAddPoints(int theX, int theY, int thePoints, Color theColor)
		{
			return DoAddPoints(theX, theY, thePoints, theColor, uint.MaxValue, true, true, -1, false, 1);
		}

		public Points DoAddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd, int thePointType)
		{
			if (thePoints <= 0 && !theForceAdd)
			{
				return null;
			}
			float num = mPointMultiplier;
			if (!usePointMultiplier)
			{
				num = 1f;
			}
			int num2 = (int)((float)thePoints * num);
			while (num2 > 0)
			{
				int num3 = Math.Min(num2, 10);
				double y = GlobalMembers.M(0.8);
				int num4 = (int)((float)GetMoveStat(theMoveCreditId, 1) / GetModePointMultiplier());
				double num5 = Math.Pow(num4 + num3, y) - Math.Pow(num4, y);
				num5 *= GlobalMembers.M(3.0);
				if (addtotube)
				{
					mLevelPointsTotal += (int)num5;
				}
				num2 -= num3;
				int num6 = (int)((float)num3 * GetModePointMultiplier());
				AddToStat(1, num6, theMoveCreditId, true);
				mPoints += num6;
				if (mPoints < 0)
				{
					mPoints = int.MaxValue;
				}
			}
			return mPointsManager.Add(theX, theY, thePoints, theColor, theId, usePointMultiplier, theMoveCreditId, theForceAdd);
		}

		public virtual Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, 1);
		}

		public virtual Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, false, 1);
		}

		public virtual Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, -1, false, 1);
		}

		public virtual Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, addtotube, true, -1, false, 1);
		}

		public virtual Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId)
		{
			return AddPoints(theX, theY, thePoints, theColor, theId, true, true, -1, false, 1);
		}

		public virtual Points AddPoints(int theX, int theY, int thePoints, Color theColor)
		{
			return AddPoints(theX, theY, thePoints, theColor, uint.MaxValue, true, true, -1, false, 1);
		}

		public virtual Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd, int thePointType)
		{
			if (mInUReplay)
			{
				return null;
			}
			if (WriteUReplayCmd(3))
			{
				mUReplayBuffer.WriteShort(EncodeX(theX));
				mUReplayBuffer.WriteShort(EncodeY(theY));
				mUReplayBuffer.WriteLong(thePoints);
				mUReplayBuffer.WriteLong(theColor.ToInt());
				mUReplayBuffer.WriteShort((short)theId);
				mUReplayBuffer.WriteBoolean(usePointMultiplier);
			}
			int num = mPoints;
			Points result = DoAddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, thePointType);
			int num2 = mPoints - num;
			mPointsBreakdown[mPointsBreakdown.Count - 1][thePointType] += num2;
			return result;
		}

		public virtual void AddPointBreakdownSection()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 5; i++)
			{
				list.Add(0);
			}
			mPointsBreakdown.Add(list);
		}

		public virtual int GetLevelPoints()
		{
			return GlobalMembers.M(2500) + mLevel * GlobalMembers.M(750);
		}

		public virtual int GetLevelPointsTotal()
		{
			return mLevelPointsTotal;
		}

		public virtual void LevelUp()
		{
			if (!mGameFinished && !mWantLevelup && mHyperspace == null && mGameOverCount <= 0)
			{
				GlobalMembers.gApp.LogStatString($"LevelUp Level={mLevel + 2} Misc.Points={mPoints} Seconds={mGameStats[0]}\n");
				mWantLevelup = true;
				mLevelup = true;
				mHyperspacePassed = false;
			}
		}

		public virtual void HyperspaceEvent(HYPERSPACEEVENT inEvent)
		{
			switch (inEvent)
			{
			case HYPERSPACEEVENT.HYPERSPACEEVENT_Start:
				mHyperspace.SetBGImage(mBackground.GetBackgroundImage());
				SetupBackground(1);
				mBackground.mVisible = false;
				break;
			case HYPERSPACEEVENT.HYPERSPACEEVENT_HideAll:
				mSideAlpha.SetConstant(1.0);
				mShowBoard = false;
				RandomizeBoard();
				break;
			case HYPERSPACEEVENT.HYPERSPACEEVENT_OldLevelClear:
				if (mLevelup)
				{
					mLevel++;
					mLevelup = false;
				}
				mLevelPointsTotal = 0;
				mMoveCounter = 0;
				break;
			case HYPERSPACEEVENT.HYPERSPACEEVENT_ZoomIn:
				mBackground.mVisible = true;
				mShowBoard = true;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SCALE_HYPERSPACE_ZOOM, mScale);
				break;
			case HYPERSPACEEVENT.HYPERSPACEEVENT_SlideOver:
			{
				mScale.SetConstant(1.0);
				Piece[,] array = mBoard;
				foreach (Piece piece in array)
				{
					if (piece != null && piece.IsFlagSet(1u))
					{
						piece.ClearHyperspaceEffects();
					}
				}
				break;
			}
			case HYPERSPACEEVENT.HYPERSPACEEVENT_Finish:
			{
				if (mHasReplayData && GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_ZEN)
				{
					ShowReplayWidget();
				}
				mBackground.SetVisible(true);
				GlobalMembers.KILL_WIDGET(mHyperspace);
				mHyperspace = null;
				GlobalMembers.gApp.mProfile.WriteProfile();
				SaveGame();
				Announcement announcement = new Announcement(this, string.Format(GlobalMembers._ID("LEVEL {0}", 541), mLevel + 1));
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_ALPHA_BOARD, announcement.mAlpha);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_SCALE_BOARD, announcement.mScale);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_HORZ_SCALE_MULT_BOARD, announcement.mHorzScaleMult);
				announcement.mBlocksPlay = false;
				announcement.mDarkenBoard = false;
				if (!mInReplay)
				{
					GlobalMembers.gApp.DisableOptionsButtons(false);
				}
				break;
			}
			}
		}

		public void RandomizeBoard()
		{
			RandomizeBoard(false);
		}

		public void RandomizeBoard(bool clearFlags)
		{
			bool flag = false;
			List<Piece> list = new List<Piece>();
			do
			{
				list.Clear();
				list.Capacity = 64;
				Piece[,] array = mBoard;
				foreach (Piece piece in array)
				{
					if (piece != null)
					{
						list.Add(piece);
						piece.mDestCol = (piece.mCol = 7 - piece.mCol);
						piece.mDestRow = (piece.mRow = 7 - piece.mRow);
					}
				}
				foreach (Piece item in list)
				{
					if (item != null)
					{
						mBoard[item.mDestRow, item.mDestCol] = item;
					}
				}
				int[,] array2 = new int[3, 2]
				{
					{ 1, 0 },
					{ 1, 1 },
					{ 0, 1 }
				};
				for (int k = 0; (k < GlobalMembers.M(16) || !FindMove(null, 0, true, true)) && k < GlobalMembers.M(20); k++)
				{
					int num = (int)((mRand.Next() % 8) & -2);
					int num2 = (int)((mRand.Next() % 8) & -2);
					Piece piece2 = mBoard[num2, num];
					if (piece2 == null || piece2.mDestCol != piece2.mCol || piece2.mDestRow != piece2.mRow)
					{
						continue;
					}
					for (int l = 0; l < 4; l++)
					{
						for (int m = 0; m < 3; m++)
						{
							int num3 = num + array2[m, 0];
							int num4 = num2 + array2[m, 1];
							Piece piece3 = mBoard[num2, num];
							Piece piece4 = mBoard[num4, num3];
							int mDestCol = piece3.mDestCol;
							piece3.mDestCol = piece4.mDestCol;
							piece4.mDestCol = mDestCol;
							mDestCol = piece3.mDestRow;
							piece3.mDestRow = piece4.mDestRow;
							piece4.mDestRow = mDestCol;
							mBoard[piece3.mDestRow, piece3.mDestCol] = piece3;
							mBoard[piece4.mDestRow, piece4.mDestCol] = piece4;
						}
						if ((l & 1) == 0 && !HasSet())
						{
							break;
						}
					}
				}
				flag = FindMove(null, 0, true, true);
				foreach (Piece item2 in list)
				{
					if (item2 != null)
					{
						mBoard[item2.mRow, item2.mCol] = item2;
					}
				}
			}
			while (!flag);
			foreach (Piece item3 in list)
			{
				if (item3 != null)
				{
					item3.mCol = item3.mDestCol;
					item3.mRow = item3.mDestRow;
					item3.mX = GetColX(item3.mCol);
					item3.mY = GetRowY(item3.mRow);
					mBoard[item3.mRow, item3.mCol] = item3;
				}
			}
			int[] array3;
			do
			{
				for (int n = 0; n < GlobalMembers.M(3); n++)
				{
					Piece piece5 = mBoard[mRand.Next() % 8, mRand.Next() % 8];
					Piece piece6 = mBoard[mRand.Next() % 8, mRand.Next() % 8];
					if (piece5 == null || piece6 == null)
					{
						continue;
					}
					for (int num5 = 0; num5 < 2; num5++)
					{
						Piece piece7 = mBoard[piece5.mRow, piece5.mCol];
						mBoard[piece5.mRow, piece5.mCol] = mBoard[piece6.mRow, piece6.mCol];
						mBoard[piece6.mRow, piece6.mCol] = piece7;
						int mCol = piece5.mCol;
						piece5.mCol = piece6.mCol;
						piece6.mCol = mCol;
						mCol = piece5.mRow;
						piece5.mRow = piece6.mRow;
						piece6.mRow = mCol;
						if (!HasSet())
						{
							break;
						}
					}
				}
				array3 = new int[4];
			}
			while (!FindMove(array3, 3, true, true, true) || array3[1] < 4);
			Piece[,] array4 = mBoard;
			foreach (Piece piece8 in array4)
			{
				if (piece8 != null)
				{
					piece8.mX = GetColX(piece8.mCol);
					piece8.mY = GetRowY(piece8.mRow);
				}
			}
		}

		public virtual void GameOverAnnounce()
		{
			if (GetTicksLeft() == 0 && GetTimeLimit() > 0)
			{
				new Announcement(this, GlobalMembers._ID("TIME UP", 94));
				GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_TIMEUP);
			}
			else
			{
				new Announcement(this, GlobalMembers._ID("GAME OVER", 95));
				GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_GAMEOVER);
			}
		}

		public virtual string GetLoggingGameName()
		{
			return mParams["Title"];
		}

		public virtual void LogGameOver(string theExtraParams)
		{
			GlobalMembers.gApp.LogStatString(string.Format("GameOver Title=\"{0}\" Seconds={1} Misc.Points={2} Level={3} PointMult={4}{5}{6}", GetLoggingGameName(), mGameStats[0], mPoints, mLevel + 1, mPointMultiplier, (theExtraParams.Length > 0) ? " " : string.Empty, theExtraParams));
		}

		public virtual string GetExtraGameOverLogParams()
		{
			return string.Empty;
		}

		public virtual void GameOver()
		{
			GameOver(true);
		}

		public virtual void GameOver(bool visible)
		{
			mCursorSelectPos = new Point(-1, -1);
			if (!mGameFinished && mGameOverCount <= 0 && mLevelCompleteCount <= 0 && !mWantLevelup && mHyperspace == null)
			{
				GlobalMembers.gApp.mProfile.mLast3MatchScoreManager.Update(mGameStats[1]);
				LogGameOver(GetExtraGameOverLogParams());
				mGameFinished = true;
				DeleteSavedGame();
				if (visible)
				{
					GameOverAnnounce();
				}
				HideReplayWidget();
				mHasReplayData = false;
				mGameOverCount = 1;
				CalcBadges();
				GlobalMembers.gApp.mProfile.mTotalGamesPlayed++;
				GlobalMembers.gApp.mProfile.WriteProfile();
			}
		}

		public virtual void CalcBadges()
		{
			SyncUnAwardedBadges(GlobalMembers.gApp.mProfile.mDeferredBadgeVector);
		}

		public virtual void BombExploded(Piece thePiece)
		{
			if (mLevelCompleteCount == 0 && mGameOverPiece == null)
			{
				GameOver();
				mGameOverPiece = thePiece;
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TIMEBOMBEXPLODE);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_NUKE_RADIUS, mNukeRadius);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_NUKE_ALPHA, mNukeAlpha, mNukeRadius);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_NOVA_RADIUS, mNovaRadius, mNukeRadius);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_NOVA_ALPHA, mNovaAlpha, mNukeRadius);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GAME_OVER_PIECE_SCALE, mGameOverPieceScale, mNukeRadius);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GAME_OVER_PIECE_GLOW, mGameOverPieceGlow, mNukeRadius);
				mNukeRadius.IncInVal(GlobalMembers.M(0.1f));
			}
		}

		public void UpdateBombExplode()
		{
			if (!mNukeRadius.IsDoingCurve())
			{
				return;
			}
			bool flag = !mNukeRadius.IncInVal();
			mGameOverPiece.Update();
			float num = (float)((double)mNovaRadius * (double)GlobalMembers.MS(280f));
			float num2 = mGameOverPiece.mX + 50f;
			float num3 = mGameOverPiece.mY + 50f;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece pieceAtRowCol = GetPieceAtRowCol(i, j);
					if (pieceAtRowCol == null || pieceAtRowCol == mGameOverPiece)
					{
						continue;
					}
					float num4 = pieceAtRowCol.mX + 50f - num2;
					float num5 = pieceAtRowCol.mY + 50f - num3;
					float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
					if (num6 < num)
					{
						for (int k = 0; k < GlobalMembers.M(3); k++)
						{
							Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_GEM_SHARD);
							effect.mColor = GlobalMembers.gGemColors[pieceAtRowCol.mColor];
							float mAngle = (float)k * 0.503f + (float)(BejeweledLivePlus.Misc.Common.Rand() % 100) / 800f;
							effect.mX = pieceAtRowCol.CX() + GlobalMembersUtils.GetRandFloat() * 100f / 2f;
							effect.mY = pieceAtRowCol.CY() + GlobalMembersUtils.GetRandFloat() * 100f / 2f;
							effect.mAngle = mAngle;
							effect.mDAngle = GlobalMembers.M(0.05f) * GlobalMembersUtils.GetRandFloat();
							effect.mScale = GlobalMembers.M(1f);
							effect.mAlpha = GlobalMembers.M(1f);
							effect.mDecel = GlobalMembers.M(0.8f) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.15f);
							float num7 = (float)Math.Atan2(num5, num4);
							float num8 = GlobalMembers.M(1f);
							effect.mDX = num8 * GlobalMembers.M(4f) * GlobalMembersUtils.GetRandFloat() + (float)Math.Cos(num7) * GlobalMembers.M(16f);
							effect.mDY = num8 * GlobalMembers.M(4f) * GlobalMembersUtils.GetRandFloat() + (float)Math.Sin(num7) * GlobalMembers.M(16f);
							effect.mGravity = num8 * GlobalMembers.M(0.05f);
							effect.mValue[0] = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f;
							effect.mValue[1] = effect.mValue[0] + GlobalMembers.M(0.25f) * (float)Math.PI * 2f;
							effect.mValue[2] = ((!(GlobalMembersUtils.GetRandFloat() > 0f)) ? 1 : 0);
							effect.mValue[3] = GlobalMembers.M(0.045f) * (GlobalMembers.M(3f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(1f));
							effect.mDAlpha = (float)(GlobalMembers.M(-0.005) * (double)(GlobalMembers.M(4f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(2f)));
							mPostFXManager.AddEffect(effect);
						}
						DeletePiece(pieceAtRowCol);
					}
				}
			}
			if (mNukeRadius.GetInVal() >= (double)GlobalMembers.M(0.58f))
			{
				mNukeRadius.GetInVal();
				double num9 = (double)GlobalMembers.M(1.65f);
			}
			if (mNukeRadius.CheckInThreshold(GlobalMembers.M(1.65f)))
			{
				mGameOverPiece.mFlags = 0;
				mPostFXManager.Clear();
				mShowBoard = false;
			}
			if (flag)
			{
				GameOver();
			}
		}

		public virtual void SetupBackground()
		{
			SetupBackground(0);
		}

		public virtual void SetupBackground(int theDeltaIdx)
		{
			if (theDeltaIdx == 0 && mBackgroundIdx >= 0 && mBackground != null)
			{
				return;
			}
			int num = GlobalMembers.gBackgroundNames.Length;
			if (!WantsBackground())
			{
				return;
			}
			string empty = string.Empty;
			int num2 = mBackgroundIdx;
			if (theDeltaIdx == 0)
			{
				if (mBackgroundIdx < 0)
				{
					if (mBackgroundIdxSet.Count == 0)
					{
						mBackgroundIdx = (int)(mRand.Next() % num);
					}
					else
					{
						mBackgroundIdx = mBackgroundIdxSet[BejeweledLivePlus.Misc.Common.Rand() % mBackgroundIdxSet.Count];
					}
					num2 = mBackgroundIdx % GlobalMembers.gBackgroundNames.Length;
				}
			}
			else
			{
				mBackgroundIdx = (mBackgroundIdx + theDeltaIdx + GlobalMembers.aDesiredOrderList.Length) % GlobalMembers.aDesiredOrderList.Length;
				num2 = GetLoadIndexFromBackgroundIndex();
			}
			empty = GlobalMembers.gBackgroundNames[num2];
			empty = ((empty.IndexOf(".pam") == -1) ? ($"images\\{GlobalMembers.gApp.mArtRes}\\backgrounds\\" + empty) : ($"images\\{GlobalMembers.gApp.mArtRes}\\backgrounds\\" + BejeweledLivePlus.Misc.Common.GetFileName(empty, true) + "\\" + empty));
			if (GlobalMembers.gApp.mForceBkg.Length != 0)
			{
				empty = GlobalMembers.gApp.mForceBkg;
			}
			if (GlobalMembers.gApp.mTestBkg.Length != 0)
			{
				empty = GlobalMembers.gApp.mTestBkg;
			}
			SetBackground(empty);
		}

		public void SetBackground(string Path)
		{
			if (mBackgroundLoadedThreaded)
			{
				mBackgroundLoadedThreaded = false;
				return;
			}
			GlobalMembers.KILL_WIDGET_NOW(mBackground);
			mBackground = new Background(Path, true, false);
			mBackground.mZOrder = -1;
			mBackground.mAllowRescale = false;
			mBackground.Resize(0, 0, mWidth, mHeight);
			GlobalMembers.gApp.mWidgetManager.AddWidget(mBackground);
			GlobalMembers.gApp.ClearUpdateBacklog(false);
		}

		public int GetLoadIndexFromBackgroundIndex()
		{
			return GlobalMembers.aDesiredOrderList[mBackgroundIdx];
		}

		public virtual void IncPointMult(Piece thePieceFrom)
		{
			int num = mPointMultiplier + 1;
			if (!mTimeExpired)
			{
				if (mBackground != null)
				{
					mBackground.mScoreWaitLevel = mPointMultiplier;
				}
				if (mPointMultSoundQueue.Count == 0)
				{
					mPointMultSoundDelay = 0;
				}
				mPointMultSoundQueue.Add(GlobalMembersResourcesWP.GetSoundById(1577 + Math.Min(3, mPointMultiplier - 1)));
				mPointMultiplier++;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_PREV_POINT_MULT_ALPHA, mPrevPointMultAlpha);
				if (thePieceFrom == null)
				{
					mSrcPointMultPos = new Point(GetBoardCenterX(), GetBoardCenterY());
				}
				else
				{
					mSrcPointMultPos = new Point((int)thePieceFrom.CX(), (int)thePieceFrom.CY());
					if (thePieceFrom.mColor == 6)
					{
						mSrcPointMultPos.mY += GlobalMembers.M(-10);
					}
					if (thePieceFrom.mColor == 4)
					{
						mSrcPointMultPos.mY += GlobalMembers.M(12);
					}
				}
				mPointMultTextMorph.SetConstant(0.0);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_POS_PCT_1, mPointMultPosPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_SCALE_1, mPointMultScale, mPointMultPosPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_ALPHA_1, mPointMultAlpha, mPointMultPosPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_Y_ADD_1, mPointMultYAdd, mPointMultPosPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_DARKEN_PCT, mPointMultDarkenPct, mPointMultPosPct);
				Color[] array = new Color[7]
				{
					new Color(255, 128, 128),
					new Color(255, 255, 255),
					new Color(128, 255, 128),
					new Color(255, 255, 128),
					new Color(255, 128, 255),
					new Color(255, 192, 128),
					new Color(128, 255, 255)
				};
				if (thePieceFrom == null)
				{
					mPointMultColor = new Color(255, 255, 255);
				}
				else
				{
					mPointMultColor = array[thePieceFrom.mColor];
				}
			}
			if (thePieceFrom != null)
			{
				AddPoints((int)thePieceFrom.CX(), (int)thePieceFrom.CY(), 1000 * num, GlobalMembers.gGemColors[thePieceFrom.mColor], (uint)thePieceFrom.mMatchId, false, false, -1);
			}
		}

		public virtual void Flamify(Piece thePiece)
		{
			if (thePiece.mColor != -1 && thePiece.SetFlag(1u))
			{
				if (WantsCalmEffects())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_POWERGEM_CREATED, GlobalMembers.M(0), GlobalMembers.M(0.5), GlobalMembers.M(-2.0));
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_POWERGEM_CREATED);
				}
				thePiece.ClearDisallowFlags();
				thePiece.ClearFlag(736u);
				thePiece.mImmunityCount = GlobalMembers.M(25);
				PopAnimEffect popAnimEffect = PopAnimEffect.fromPopAnim(GlobalMembersResourcesWP.POPANIM_FLAMEGEMCREATION);
				popAnimEffect.mX = thePiece.CX();
				popAnimEffect.mY = thePiece.CY();
				popAnimEffect.mScale = 2f;
				popAnimEffect.mDoubleSpeed = true;
				popAnimEffect.Play("Creation_Below Gem_Horizontal");
				popAnimEffect.Update();
				mPreFXManager.AddEffect(popAnimEffect);
				popAnimEffect = PopAnimEffect.fromPopAnim(GlobalMembersResourcesWP.POPANIM_FLAMEGEMCREATION);
				popAnimEffect.mX = thePiece.CX();
				popAnimEffect.mY = thePiece.CY();
				popAnimEffect.mScale = 2f;
				popAnimEffect.mOverlay = true;
				popAnimEffect.mDoubleSpeed = true;
				popAnimEffect.Play("Creation_Above Gem");
				popAnimEffect.Update();
				mPostFXManager.AddEffect(popAnimEffect);
			}
		}

		public virtual void Hypercubeify(Piece thePiece)
		{
			Hypercubeify(thePiece, true);
		}

		public virtual void Hypercubeify(Piece thePiece, bool theStartEffect)
		{
			if (!thePiece.CanSetFlag(2u))
			{
				return;
			}
			thePiece.ClearFlags();
			thePiece.SetFlag(2u);
			thePiece.mChangedTick = mUpdateCnt;
			thePiece.mLastColor = thePiece.mColor;
			thePiece.mColor = -1;
			thePiece.mImmunityCount = GlobalMembers.M(25);
			if (theStartEffect)
			{
				StartHypercubeEffect(thePiece);
				if (WantsCalmEffects())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HYPERCUBE_CREATE, GlobalMembers.M(0), GlobalMembers.M(0.4), GlobalMembers.M(-3.0));
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HYPERCUBE_CREATE);
				}
			}
		}

		public virtual void Laserify(Piece thePiece)
		{
			if (thePiece.mColor != -1 && thePiece.SetFlag(4u))
			{
				thePiece.mShakeScale = 0f;
				if (WantsCalmEffects())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_LASERGEM_CREATED, GlobalMembers.M(0), GlobalMembers.M(0.5), GlobalMembers.M(-2.0));
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_LASERGEM_CREATED);
				}
				thePiece.ClearDisallowFlags();
				thePiece.ClearFlag(736u);
				thePiece.mImmunityCount = GlobalMembers.M(25);
				StartLaserGemEffect(thePiece);
			}
		}

		public void Bombify(Piece thePiece, int theCounter)
		{
			Bombify(thePiece, theCounter, false);
		}

		public void Bombify(Piece thePiece)
		{
			Bombify(thePiece, 20, false);
		}

		public void Bombify(Piece thePiece, int theCounter, bool realTime)
		{
			if (thePiece.CanSetFlag(512u) || !(realTime ? thePiece.CanSetFlag(64u) : thePiece.CanSetFlag(32u)))
			{
				thePiece.ClearFlags();
				thePiece.ClearDisallowFlags();
				if (realTime)
				{
					thePiece.SetFlag(576u);
				}
				else
				{
					thePiece.SetFlag(544u);
				}
				thePiece.mCounter = theCounter;
				thePiece.mImmunityCount = GlobalMembers.M(25);
			}
		}

		public void Doomify(Piece thePiece, int theCounter)
		{
			if (thePiece.CanSetFlag(256u) && thePiece.CanSetFlag(512u))
			{
				thePiece.ClearFlags();
				thePiece.ClearDisallowFlags();
				thePiece.SetFlag(768u);
				thePiece.mColor = -1;
				thePiece.mCounter = theCounter;
				thePiece.mImmunityCount = GlobalMembers.M(25);
			}
		}

		public void Coinify(Piece thePiece)
		{
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COIN_CREATED);
			if (thePiece.IsFlagSet(1024u))
			{
				thePiece.mCounter = Math.Min(thePiece.mCounter + 1, 3);
			}
			else
			{
				if (!thePiece.CanSetFlag(1024u))
				{
					return;
				}
				thePiece.ClearFlags();
				thePiece.SetFlag(1024u);
				thePiece.mColor = 1;
				thePiece.mImmunityCount = GlobalMembers.M(25);
			}
			thePiece.mCounter = 3;
		}

		public void Butterflyify(Piece thePiece)
		{
			if (thePiece.CanSetFlag(128u))
			{
				thePiece.ClearFlags();
				thePiece.ClearBoundEffects();
				thePiece.SetFlag(128u);
				StartPieceEffect(thePiece);
				ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_BUTTERFLY_CREATE);
				particleEffect.mPieceRel = thePiece;
				particleEffect.SetEmitterTint(0, 0, GlobalMembers.gGemColors[thePiece.mColor]);
				particleEffect.SetEmitterTint(0, 1, GlobalMembers.gGemColors[thePiece.mColor]);
				particleEffect.SetEmitterTint(0, 2, GlobalMembers.gGemColors[thePiece.mColor]);
				particleEffect.mDoubleSpeed = false;
				mPostFXManager.AddEffect(particleEffect);
			}
		}

		public void Blastify(Piece thePiece)
		{
			if (thePiece.CanSetFlag(524288u))
			{
				thePiece.SetFlag(524288u);
				StartPieceEffect(thePiece);
			}
		}

		public void StartPieceEffect(Piece thePiece)
		{
			if (thePiece.IsFlagSet(16u))
			{
				StartMultiplierGemEffect(thePiece);
			}
			else if (thePiece.IsFlagSet(4u))
			{
				StartLaserGemEffect(thePiece);
			}
			else if (thePiece.IsFlagSet(128u))
			{
				StartButterflyEffect(thePiece);
			}
			else if (thePiece.IsFlagSet(2u))
			{
				StartHypercubeEffect(thePiece);
			}
			else if (thePiece.IsFlagSet(6144u))
			{
				StartBoostGemEffect(thePiece);
			}
			else if (thePiece.IsFlagSet(524288u))
			{
				StartBlastgemEffect(thePiece);
			}
			else if (thePiece.IsFlagSet(131072u))
			{
				StartTimeBonusEffect(thePiece);
			}
		}

		public void Start3DFireGemEffect(Piece thePiece)
		{
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_FIREGEM_HYPERSPACE);
			particleEffect.SetEmitAfterTimeline(true);
			particleEffect.mDoDrawTransform = true;
			particleEffect.mFlags |= 8;
			thePiece.BindEffect(particleEffect);
			mPreFXManager.AddEffect(particleEffect);
		}

		public void StartLaserGemEffect(Piece thePiece)
		{
			ParticleEffect theEffect = NewBottomLaserEffect(thePiece.mColor);
			thePiece.BindEffect(theEffect);
			mPreFXManager.AddEffect(theEffect);
			theEffect = NewTopLaserEffect(thePiece.mColor);
			thePiece.BindEffect(theEffect);
			mPostFXManager.AddEffect(theEffect);
		}

		public ParticleEffect NewTopLaserEffect(int theGemColor)
		{
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_STARGEM);
			particleEffect.SetEmitAfterTimeline(true);
			particleEffect.mDoDrawTransform = true;
			for (int i = 0; i < 7; i++)
			{
				PILayer layer = particleEffect.GetLayer(i + 1);
				if (i == theGemColor)
				{
					layer.SetVisible(true);
				}
				else
				{
					layer.SetVisible(false);
				}
			}
			PILayer layer2 = particleEffect.GetLayer(theGemColor + 1);
			layer2.GetEmitter("Glow")?.SetVisible(false);
			return particleEffect;
		}

		public ParticleEffect NewBottomLaserEffect(int theGemColor)
		{
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_STARGEM);
			particleEffect.SetEmitAfterTimeline(true);
			particleEffect.mDoDrawTransform = true;
			particleEffect.mDoubleSpeed = true;
			for (int i = 0; i < 7; i++)
			{
				PILayer layer = particleEffect.GetLayer(i + 1);
				if (i == theGemColor)
				{
					layer.SetVisible(true);
				}
				else
				{
					layer.SetVisible(false);
				}
			}
			particleEffect.GetLayer("Top").SetVisible(false);
			PILayer layer2 = particleEffect.GetLayer(theGemColor + 1);
			layer2.GetEmitter("Stars")?.SetVisible(false);
			return particleEffect;
		}

		public void StartMultiplierGemEffect(Piece thePiece)
		{
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ANIM_CURVE_A, thePiece.mAnimCurve);
			thePiece.mAnimCurve.SetMode(1);
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_MULTIPLIER);
			particleEffect.mPieceRel = thePiece;
			particleEffect.mDoDrawTransform = true;
			particleEffect.mDoubleSpeed = true;
			particleEffect.SetEmitAfterTimeline(true);
			for (int i = 0; i < 7; i++)
			{
				PILayer layer = particleEffect.GetLayer(i + 1);
				if (i == thePiece.mColor)
				{
					layer.SetVisible(true);
				}
				else
				{
					layer.SetVisible(false);
				}
			}
			mPostFXManager.AddEffect(particleEffect);
		}

		public void StartButterflyEffect(Piece thePiece)
		{
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_BUTTERFLY);
			particleEffect.mPieceRel = thePiece;
			particleEffect.SetEmitAfterTimeline(true);
			particleEffect.mDoDrawTransform = false;
			particleEffect.SetEmitterTint(0, 0, GlobalMembers.gGemColors[thePiece.mColor]);
			particleEffect.SetEmitterTint(0, 1, GlobalMembers.gGemColors[thePiece.mColor]);
			mPostFXManager.AddEffect(particleEffect);
		}

		public void StartHypercubeEffect(Piece thePiece)
		{
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_HYPERCUBE);
			particleEffect.mPieceRel = thePiece;
			particleEffect.SetEmitAfterTimeline(true);
			particleEffect.mDoDrawTransform = true;
			mPreFXManager.AddEffect(particleEffect);
		}

		public void StartBlastgemEffect(Piece thePiece)
		{
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_BLASTGEM);
			particleEffect.mPieceRel = thePiece;
			particleEffect.SetEmitAfterTimeline(true);
			particleEffect.mDoDrawTransform = true;
			mPreFXManager.AddEffect(particleEffect);
		}

		public void StartTimeBonusEffect(Piece thePiece)
		{
			TimeBonusEffect timeBonusEffect = TimeBonusEffect.alloc(thePiece);
			timeBonusEffect.mX = 50f;
			timeBonusEffect.mY = 50f;
			timeBonusEffect.mZ = GlobalMembers.M(0.08f);
			mPostFXManager.AddEffect(timeBonusEffect);
		}

		public void StartBoostGemEffect(Piece thePiece)
		{
		}

		public Piece GetSelectedPiece()
		{
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.mSelected)
				{
					return piece;
				}
			}
			return null;
		}

		public List<int> GetNewGemColors()
		{
			List<int> list = new List<int>();
			list.AddRange(mNewGemColors);
			if (mFavorComboGems)
			{
				for (int i = 0; i < mComboLen; i++)
				{
					list.Add(mComboColors[i]);
				}
			}
			for (int j = 0; j < mFavorGemColors.Count; j++)
			{
				list.Add(mFavorGemColors[j]);
			}
			return list;
		}

		public Piece CreateNewPiece(int theRow, int theCol)
		{
			if (theRow == -1 && theCol == -1)
			{
				return null;
			}
			Piece piece = Piece.alloc(this);
			piece.mCreatedTick = mUpdateCnt;
			piece.mLastActiveTick = mUpdateCnt;
			piece.mCol = theCol;
			piece.mRow = theRow;
			piece.mX = GetColX(theCol);
			piece.mY = GetRowY(theRow);
			mBoard[theRow, theCol] = piece;
			return piece;
		}

		public void DrawButterfly(Graphics g, int anOfsX, int anOfsY, Piece pPiece, float aScale)
		{
			g.SetScale(aScale, aScale, GlobalMembers.S(pPiece.GetScreenX() + 50f), GlobalMembers.S(pPiece.GetScreenY() + 50f));
			GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_BUTTERFLY_SHADOW, GlobalMembers.S(anOfsX), GlobalMembers.S(anOfsY), pPiece.mColor);
			g.SetScale(1f, 1f, GlobalMembers.S(pPiece.GetScreenX() + 50f), GlobalMembers.S(pPiece.GetScreenY() + 50f));
			Transform transform = new Transform();
			transform.Translate(GlobalMembers.S(ConstantsWP.BUTTERFLY_DRAW_OFFSET_1), 0f);
			transform.Scale((float)((1.0 - (double)pPiece.mAnimCurve) * (double)aScale), aScale);
			GlobalMembers.gGR.DrawImageTransform(g, GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, transform, GlobalMembers.IMGSRCRECT(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, pPiece.mColor), GlobalMembers.S(anOfsX + ConstantsWP.BUTTERFLY_DRAW_OFFSET_2), GlobalMembers.S(anOfsY + ConstantsWP.BUTTERFLY_DRAW_OFFSET_3));
			transform.Scale(-1f, 1f);
			GlobalMembers.gGR.DrawImageTransform(g, GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, transform, GlobalMembers.IMGSRCRECT(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, pPiece.mColor), GlobalMembers.S(anOfsX + ConstantsWP.BUTTERFLY_DRAW_OFFSET_4), GlobalMembers.S(anOfsY + ConstantsWP.BUTTERFLY_DRAW_OFFSET_3));
			g.SetScale(aScale, aScale, GlobalMembers.S(pPiece.GetScreenX() + 50f), GlobalMembers.S(pPiece.GetScreenY() + 50f));
			GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_BUTTERFLY_BODY, GlobalMembers.S(anOfsX), GlobalMembers.S(anOfsY), pPiece.mColor);
			g.SetScale(1f, 1f, GlobalMembers.S(pPiece.GetScreenX() + 50f), GlobalMembers.S(pPiece.GetScreenY() + 50f));
		}

		public short EncodeX(float theX)
		{
			return (short)((theX - (float)GetBoardX()) / 100f * 256f);
		}

		public short EncodeY(float theY)
		{
			return (short)((theY - (float)GetRowY(0)) / 100f * 256f);
		}

		public float DecodeX(short theX)
		{
			return (float)(theX * 100) / 256f + (float)GetBoardX();
		}

		public float DecodeY(short theY)
		{
			return (float)(theY * 100) / 256f + (float)GetRowY(0);
		}

		public void EncodeSpeedBonus()
		{
			if (WriteUReplayCmd(8))
			{
				mUReplayBuffer.WriteByte((byte)mSpeedBonusCount);
				mUReplayBuffer.WriteByte((byte)(Math.Max(0.0, Math.Min(1.0, mSpeedBonusNum)) * 255.0));
			}
		}

		public byte EncodePieceFlags(int thePieceFlags)
		{
			if ((thePieceFlags & 1) != 0)
			{
				return 1;
			}
			if ((thePieceFlags & 4) != 0)
			{
				return 2;
			}
			if ((thePieceFlags & 2) != 0)
			{
				return 3;
			}
			if ((thePieceFlags & 0x10) != 0)
			{
				return 4;
			}
			if ((thePieceFlags & 0x400) != 0)
			{
				return 5;
			}
			if ((thePieceFlags & 0x800) != 0)
			{
				return 8;
			}
			if ((thePieceFlags & 0x1000) != 0)
			{
				return 7;
			}
			if ((thePieceFlags & 0x2000) != 0)
			{
				return 6;
			}
			return 0;
		}

		public int DecodePieceFlags(byte theType)
		{
			switch (theType)
			{
			case 1:
				return 1;
			case 2:
				return 4;
			case 3:
				return 2;
			case 4:
				return 16;
			case 5:
				return 1024;
			case 8:
				return 2048;
			case 7:
				return 4096;
			case 6:
				return 8192;
			default:
				return 0;
			}
		}

		public void EncodeLightningStorm(LightningStorm theLightningStorm)
		{
			if (theLightningStorm.mStormType == 2)
			{
				if (WriteUReplayCmd(14))
				{
					EncodePieceRef(GetPieceById(theLightningStorm.mElectrocuterId));
				}
			}
			else if (theLightningStorm.mStormType == 7 && WriteUReplayCmd(15))
			{
				EncodePieceRef(GetPieceById(theLightningStorm.mElectrocuterId));
				mUReplayBuffer.WriteByte((byte)theLightningStorm.mColor);
			}
		}

		public void EncodePieceRef(Piece thePiece)
		{
			if (thePiece == null)
			{
				mUReplayBuffer.WriteByte(byte.MaxValue);
			}
			else
			{
				mUReplayBuffer.WriteByte((byte)(thePiece.mRow * 8 + thePiece.mCol));
			}
		}

		public Piece DecodePieceRef()
		{
			byte b = mUReplayBuffer.ReadByte();
			if (b == byte.MaxValue)
			{
				return null;
			}
			return mBoard[b / 8, b % 8];
		}

		public virtual bool WantPointComplements()
		{
			return true;
		}

		public virtual bool IsHypermixerCancelledBy(Piece thePiece)
		{
			return thePiece.IsFlagSet(2u);
		}

		public void DrawCheckboard(Graphics g)
		{
			float mTransX = g.mTransX;
			float mTransY = g.mTransY;
			if ((double)mSideXOff != 0.0)
			{
				g.Translate((int)((double)mSideXOff * (double)mSlideXScale), 0);
			}
			else
			{
				g.Translate((int)((double)GlobalMembers.S(1260) * (double)mSlideUIPct), 0);
			}
			float num = mSpeedBonusFlameModePct * (float)GlobalMembers.M(60);
			Color theColor = mBoardColors[0];
			theColor.mAlpha = (int)((float)theColor.mAlpha * GetBoardAlpha());
			Color color = Utils.ColorLerp(theColor2: new Color((int)GlobalMembers.M(180f), (int)(GlobalMembers.M(100f) + (float)Math.Sin(num) * GlobalMembers.M(14f)), (int)(GlobalMembers.M(48f) + (float)Math.Sin(num) * GlobalMembers.M(8f)), (int)(GlobalMembers.M(200f) * GetBoardAlpha())), theColor1: theColor, theT: Math.Min(1f, mSpeedBonusFlameModePct * 5f));
			Color theColor3 = mBoardColors[1];
			theColor3.mAlpha = (int)((float)theColor3.mAlpha * GetBoardAlpha());
			Color color2 = Utils.ColorLerp(theColor2: new Color((int)GlobalMembers.M(160f), (int)(GlobalMembers.M(90f) + (float)Math.Sin(num) * GlobalMembers.M(12f)), (int)(GlobalMembers.M(40f) + (float)Math.Sin(num) * GlobalMembers.M(7f)), (int)(GlobalMembers.M(200f) * GetBoardAlpha())), theColor1: theColor3, theT: Math.Min(1f, mSpeedBonusFlameModePct * 5f));
			int[] array = new int[9];
			for (int i = 0; i < 9; i++)
			{
				array[i] = GlobalMembers.S(GetColScreenX(i));
			}
			if (mBoardUIOffsetY != 0)
			{
				g.Translate(0, GlobalMembers.S(mBoardUIOffsetY));
			}
			int num2 = 8;
			for (int j = 0; j < num2; j++)
			{
				int num3 = GlobalMembers.S(GetRowScreenY(j));
				int num4 = GlobalMembers.S(GetRowScreenY(j + 1));
				for (int k = 0; k < 8; k++)
				{
					int num5 = array[k];
					int num6 = array[k + 1];
					if ((j + k) % 2 == 0)
					{
						g.SetColor(color);
					}
					else
					{
						g.SetColor(color2);
					}
					g.FillRect(num5, num3, num6 - num5, num4 - num3);
				}
			}
			if (g.mTransX != mTransX || g.mTransY != mTransY)
			{
				g.mTransX = mTransX;
				g.mTransY = mTransY;
			}
			g.SetColor(Color.White);
		}

		public int GetColX(int theCol)
		{
			return theCol * 100;
		}

		public int GetRowY(int theRow)
		{
			return theRow * 100;
		}

		public int GetColScreenX(int theCol)
		{
			return GetColX(theCol) + GetBoardX();
		}

		public int GetRowScreenY(int theRow)
		{
			return GetRowY(theRow) + GetBoardY();
		}

		public int GetColAt(int theX)
		{
			for (int i = 0; i < 8; i++)
			{
				int colX = GetColX(i);
				if (theX >= colX && theX < colX + 100)
				{
					return i;
				}
			}
			return -1;
		}

		public int GetRowAt(int theY)
		{
			for (int i = 0; i < 8; i++)
			{
				int rowY = GetRowY(i);
				if (theY >= rowY && theY < rowY + 100)
				{
					return i;
				}
			}
			return -1;
		}

		public virtual int GetBoardX()
		{
			return GlobalMembers.RS(ConstantsWP.BOARD_X);
		}

		public virtual int GetBoardY()
		{
			return GlobalMembers.RS(ConstantsWP.BOARD_Y);
		}

		public int GetBoardCenterX()
		{
			return GetBoardX() + 400;
		}

		public int GetBoardCenterY()
		{
			return GetBoardY() + 400;
		}

		public virtual float GetAlpha()
		{
			return (float)(double)mAlpha;
		}

		public virtual float GetBoardAlpha()
		{
			return GetAlpha();
		}

		public virtual float GetPieceAlpha()
		{
			return (1f - mBoardHidePct) * GetBoardAlpha();
		}

		public Piece GetPieceAtRowCol(int theRow, int theCol)
		{
			if (theRow < 0 || theRow >= 8 || theCol < 0 || theCol >= 8)
			{
				return null;
			}
			return mBoard[theRow, theCol];
		}

		public Piece GetPieceAtXY(int theX, int theY)
		{
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && (float)theX >= piece.mX && (float)theY >= piece.mY && (float)theX < piece.mX + 100f && (float)theY < piece.mY + 100f)
				{
					return piece;
				}
			}
			return null;
		}

		public Piece GetPieceAtScreenXY(int theX, int theY)
		{
			return GetPieceAtXY(theX - GetBoardX(), theY - GetBoardY());
		}

		public Piece GetPieceById(int theId)
		{
			if (theId == -1)
			{
				return null;
			}
			if (mPieceMap.ContainsKey(theId))
			{
				return mPieceMap[theId];
			}
			return null;
		}

		public Piece GetRandomPieceOnGrid()
		{
			List<Piece> list = new List<Piece>();
			list.Capacity = 64;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					if (piece != null)
					{
						list.Add(piece);
					}
				}
			}
			if (list.Count > 0)
			{
				return list[(int)(mRand.Next() % list.Count)];
			}
			return null;
		}

		public virtual void DeletePiece(Piece thePiece)
		{
			TallyPiece(thePiece, true);
			if (WriteUReplayCmd(4))
			{
				EncodePieceRef(thePiece);
			}
			for (int i = 0; i < mSwapDataVector.Count; i++)
			{
				SwapData swapData = mSwapDataVector[i];
				if (swapData.mPiece1 == thePiece || swapData.mPiece2 == thePiece)
				{
					if ((double)swapData.mSwapPct < 0.0 && swapData.mPiece1 != null && swapData.mPiece2 != null)
					{
						int mCol = swapData.mPiece1.mCol;
						swapData.mPiece1.mCol = swapData.mPiece2.mCol;
						swapData.mPiece2.mCol = mCol;
						mCol = swapData.mPiece1.mRow;
						swapData.mPiece1.mRow = swapData.mPiece2.mRow;
						swapData.mPiece2.mRow = mCol;
						Piece piece = mBoard[swapData.mPiece1.mRow, swapData.mPiece1.mCol];
						mBoard[swapData.mPiece1.mRow, swapData.mPiece1.mCol] = mBoard[swapData.mPiece2.mRow, swapData.mPiece2.mCol];
						mBoard[swapData.mPiece2.mRow, swapData.mPiece2.mCol] = piece;
					}
					if (swapData.mPiece1 != null)
					{
						swapData.mPiece1.mX = GetColX(swapData.mPiece1.mCol);
						swapData.mPiece1.mY = GetRowY(swapData.mPiece1.mRow);
						swapData.mPiece1 = null;
					}
					if (swapData.mPiece2 != null)
					{
						swapData.mPiece2.mX = GetColX(swapData.mPiece2.mCol);
						swapData.mPiece2.mY = GetRowY(swapData.mPiece2.mRow);
						swapData.mPiece2 = null;
					}
					mSwapDataVector.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < thePiece.mRow; j++)
			{
				Piece piece2 = mBoard[j, thePiece.mCol];
				if (piece2 != null)
				{
					SetMoveCredit(piece2, thePiece.mMoveCreditId);
				}
			}
			mPreFXManager.RemovePieceFromEffects(thePiece);
			mPostFXManager.RemovePieceFromEffects(thePiece);
			mNextColumnCredit[thePiece.mCol] = Math.Max(mNextColumnCredit[thePiece.mCol], thePiece.mMoveCreditId);
			mBoard[thePiece.mRow, thePiece.mCol] = null;
			thePiece.release();
			thePiece = null;
		}

		public void ClearAllPieces()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (mBoard[i, j] != null)
					{
						mBoard[i, j].release();
						mBoard[i, j] = null;
					}
				}
			}
			mSwapDataVector.Clear();
		}

		public void AddToPieceMap(int theId, Piece thePiece)
		{
			mPieceMap.Add(theId, thePiece);
		}

		public virtual void RemoveFromPieceMap(int theId)
		{
			if (mPieceMap.ContainsKey(theId))
			{
				mPieceMap.Remove(theId);
			}
		}

		public bool IsPieceSwapping(Piece thePiece, bool includeIgnored)
		{
			return IsPieceSwapping(thePiece, includeIgnored, false);
		}

		public bool IsPieceSwapping(Piece thePiece)
		{
			return IsPieceSwapping(thePiece, false, false);
		}

		public bool IsPieceSwapping(Piece thePiece, bool includeIgnored, bool onlyCheckForwardSwaps)
		{
			foreach (SwapData item in mSwapDataVector)
			{
				if ((!item.mIgnore || includeIgnored) && ((item.mForwardSwap && item.mHoldingSwap == 0) || !onlyCheckForwardSwaps) && (item.mPiece1 == thePiece || item.mPiece2 == thePiece))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsPieceMatching(Piece thePiece)
		{
			return thePiece.IsShrinking();
		}

		public bool IsPieceStill(Piece thePiece)
		{
			if (thePiece.mFallVelocity == 0f && (double)thePiece.mDestPct == 0.0 && thePiece.mExplodeDelay == 0 && !thePiece.mDestPct.IsDoingCurve() && (float)GetRowY(thePiece.mRow) == thePiece.mY && (thePiece.mCanMatch || thePiece.IsFlagSet(65536u)))
			{
				if (!thePiece.IsFlagSet(8192u) && !IsPieceMatching(thePiece))
				{
					return !IsPieceSwapping(thePiece, false, false);
				}
				return false;
			}
			return false;
		}

		public bool WillPieceBeStill(Piece thePiece)
		{
			if (!IsPieceMatching(thePiece) && !IsPieceSwapping(thePiece, false, true) && thePiece.mCanMatch && thePiece.mExplodeDelay == 0 && (double)thePiece.mDestPct == 0.0)
			{
				return !thePiece.IsFlagSet(8192u);
			}
			return false;
		}

		public bool CanBakeShadow(Piece thePiece)
		{
			if (thePiece.mRotPct == 0f)
			{
				return !thePiece.IsFlagSet(6165u);
			}
			return false;
		}

		public bool IsBoardStill()
		{
			if (mSettlingDelay != 0)
			{
				return false;
			}
			if (mLightningStorms.Count != 0)
			{
				return false;
			}
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && !IsPieceStill(piece))
				{
					return false;
				}
			}
			if (HasSet())
			{
				return false;
			}
			if (!mHasBoardSettled)
			{
				mHasBoardSettled = true;
				BoardSettled();
			}
			return true;
		}

		public virtual bool IsGameIdle()
		{
			if (mSettlingDelay != 0)
			{
				return false;
			}
			if (mLightningStorms.Count != 0)
			{
				return false;
			}
			if (mScrambleDelayTicks != 0)
			{
				return false;
			}
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && !IsPieceStill(piece) && !IsPieceSwapping(piece, false, false))
				{
					return false;
				}
			}
			if (HasSet(null))
			{
				return false;
			}
			return true;
		}

		public virtual void DoHypercube(Piece thePiece, int theColor)
		{
			if (theColor == -1)
			{
				AddToStat(37, 1, thePiece.mMoveCreditId);
			}
			AddToStat(14, 1, thePiece.mMoveCreditId);
			ComboProcess(theColor);
			thePiece.mDestructing = true;
			LightningStorm lightningStorm = new LightningStorm(this, thePiece, 7);
			lightningStorm.mColor = theColor;
			mLightningStorms.Add(lightningStorm);
			EncodeLightningStorm(lightningStorm);
			Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_HYPERCUBE_ENERGIZE);
			effect.mX = thePiece.CX();
			effect.mY = thePiece.CY();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_ALPHA_HYPERCUBE, effect.mCurvedAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_SCALE_HYPERCUBE, effect.mCurvedScale);
			effect.mDScale = 0f;
			effect.mDAlpha = 0f;
			effect.mAngle = 0f;
			effect.mDAngle = 1f;
			effect.mIsAdditive = true;
			effect.mGravity = 0f;
			effect.mOverlay = true;
			mPostFXManager.AddEffect(effect);
		}

		public virtual void DoHypercube(Piece thePiece, Piece theSwappedPiece)
		{
			DoHypercube(thePiece, theSwappedPiece.mColor);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ELECTRO_PATH);
			LightningStorm lightningStorm = mLightningStorms[mLightningStorms.Count - 1];
			lightningStorm.AddLightning((int)thePiece.mX + 50, (int)thePiece.mY + 50, (int)theSwappedPiece.mX + 50, (int)theSwappedPiece.mY + 50);
			Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_LIGHT);
			effect.mFlags = 2;
			effect.mX = theSwappedPiece.CX();
			effect.mY = theSwappedPiece.CY();
			effect.mZ = GlobalMembers.M(0.04f);
			effect.mValue[0] = GlobalMembers.M(16.1f);
			effect.mValue[1] = GlobalMembers.M(-0.8f);
			effect.mAlpha = GlobalMembers.M(0f);
			effect.mDAlpha = GlobalMembers.M(0.1f);
			effect.mScale = GlobalMembers.M(140f);
			mPostFXManager.AddEffect(effect);
		}

		public virtual void ExamineBoard()
		{
		}

		public virtual bool WantSpecialPiece(List<Piece> thePieceVector)
		{
			return false;
		}

		public virtual bool DropSpecialPiece(List<Piece> thePieceVector)
		{
			return false;
		}

		public virtual bool TryingDroppedPieces(List<Piece> thePieceVector, int theTryCount)
		{
			return true;
		}

		public virtual bool PiecesDropped(List<Piece> thePieceVector)
		{
			return true;
		}

		public int NumPiecesWithFlag(int theFlag)
		{
			int num = 0;
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece.IsFlagSet((uint)theFlag))
				{
					num++;
				}
			}
			return num;
		}

		public virtual bool CanTimeUp()
		{
			return IsBoardStill();
		}

		public virtual int GetTicksLeft()
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
			int num = GlobalMembers.M(50);
			return Math.Min(timeLimit * 100, Math.Max(0, timeLimit * 100 - Math.Max(0, mGameTicks - num)));
		}

		public virtual float GetLevelPct()
		{
			int levelPoints = GetLevelPoints();
			bool flag = mUpdateCnt % 20 == 0;
			float result = ((levelPoints == 0) ? 0f : Math.Min(1f, (float)GetLevelPointsTotal() / (float)levelPoints));
			if (flag && WriteUReplayCmd(7))
			{
				mUReplayBuffer.WriteShort((short)GetTicksLeft());
			}
			return result;
		}

		public virtual float GetCountdownPct()
		{
			int timeLimit = GetTimeLimit();
			bool flag = mUpdateCnt % 20 == 0;
			float result = Math.Max(0f, (float)GetTicksLeft() / ((float)timeLimit * 100f));
			CheckCountdownBar();
			int ticksLeft = GetTicksLeft();
			int num = GlobalMembers.M(35) + (int)((float)ticksLeft * GlobalMembers.M(0.1f));
			if (mUpdateCnt - mLastWarningTick >= num && ticksLeft > 0 && WantWarningGlow())
			{
				int num2 = ((GetTimeLimit() > 60) ? 1500 : 1000);
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COUNTDOWN_WARNING, 0, Math.Min(1.0, GlobalMembers.M(1.0) - (double)((float)ticksLeft / (float)num2 / 3f)));
				mLastWarningTick = mUpdateCnt;
			}
			if (ticksLeft == 3000 && mDoThirtySecondVoice && mLevelCompleteCount == 0)
			{
				flag = true;
				GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_THIRTYSECONDS);
				if (mInUReplay)
				{
					mUReplayTicksLeft--;
				}
			}
			if (flag && WriteUReplayCmd(7))
			{
				mUReplayBuffer.WriteShort((short)GetTicksLeft());
			}
			return result;
		}

		public void ResolveMysteryGem()
		{
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece.IsFlagSet(8192u))
				{
					piece.ClearFlag(8192u);
					int num = 0;
					switch (mRand.Next() % 3)
					{
					case 0u:
						num = 1;
						Flamify(piece);
						break;
					case 1u:
						num = 3;
						Hypercubeify(piece);
						break;
					case 2u:
						num = 2;
						Laserify(piece);
						break;
					}
					if (WriteUReplayCmd(19))
					{
						EncodePieceRef(piece);
						mUReplayBuffer.WriteByte((byte)num);
					}
					return;
				}
			}
		}

		public virtual int GetPowerGemThreshold()
		{
			return 5;
		}

		public virtual void SwapSucceeded(SwapData theSwapData)
		{
		}

		public virtual void SwapFailed(SwapData theSwapData)
		{
			if (mWantReplaySave)
			{
				mWantReplaySave = false;
			}
		}

		public virtual bool IsSwapLegal(Piece theSelected, int theSwappedRow, int theSwappedCol)
		{
			if (mLightningStorms.Count != 0)
			{
				return false;
			}
			if (!theSelected.mCanSwap)
			{
				return false;
			}
			Piece pieceAtRowCol = GetPieceAtRowCol(theSwappedRow, theSwappedCol);
			if (!IsPieceStill(theSelected) || theSelected.IsFlagSet(256u))
			{
				return false;
			}
			if (pieceAtRowCol != null && (!pieceAtRowCol.mCanSwap || !IsPieceStill(pieceAtRowCol) || pieceAtRowCol.IsFlagSet(256u)))
			{
				return false;
			}
			if (mDeferredTutorialVector.Count > 0)
			{
				return false;
			}
			int value = theSwappedCol - theSelected.mCol;
			int value2 = theSwappedRow - theSelected.mRow;
			if ((theSelected.IsButton() && Math.Abs(value) + Math.Abs(value2) != 0) || (!theSelected.IsButton() && Math.Abs(value) + Math.Abs(value2) != 1))
			{
				return false;
			}
			return true;
		}

		public virtual bool QueueSwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped)
		{
			return QueueSwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, false);
		}

		public virtual bool QueueSwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap)
		{
			return QueueSwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, true, false);
		}

		public virtual bool QueueSwap(Piece theSelected, int theSwappedRow, int theSwappedCol)
		{
			return QueueSwap(theSelected, theSwappedRow, theSwappedCol, false, true, false);
		}

		public virtual bool QueueSwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped, bool destroyTarget)
		{
			if (!IsSwapLegal(theSelected, theSwappedRow, theSwappedCol))
			{
				return false;
			}
			QueuedMove queuedMove = new QueuedMove();
			queuedMove.mUpdateCnt = mUpdateCnt;
			queuedMove.mSelectedId = theSelected.mId;
			queuedMove.mSwappedRow = theSwappedRow;
			queuedMove.mSwappedCol = theSwappedCol;
			queuedMove.mForceSwap = forceSwap;
			queuedMove.mPlayerSwapped = playerSwapped;
			queuedMove.mDestroyTarget = destroyTarget;
			mQueuedMoveVector.Add(queuedMove);
			if (WantsWholeGameReplay())
			{
				mWholeGameReplay.mReplayMoves.Add(queuedMove);
			}
			return true;
		}

		public void PushMoveData(Piece theSelected, int theSwappedRow, int theSwappedCol)
		{
			MoveData moveData = new MoveData();
			moveData.mUpdateCnt = mUpdateCnt;
			moveData.mSelectedId = theSelected.mId;
			moveData.mSwappedRow = theSwappedRow;
			moveData.mSwappedCol = theSwappedCol;
			moveData.mPreSaveBuffer.WriteBytes(mLastMoveSave.mData, mLastMoveSave.GetDataLen());
			moveData.mMoveCreditId = mCurMoveCreditId++;
			for (int i = 0; i < 40; i++)
			{
				moveData.mStats[i] = 0;
			}
			mMoveDataVector.Add(moveData);
		}

		private bool CheckTrialGameFinished()
		{
			bool result = false;
			if (!GlobalMembers.gApp.mMainMenu.mIsFullGame())
			{
				switch (GlobalMembers.gApp.mCurrentGameMode)
				{
				case GameMode.MODE_CLASSIC:
					if (mLevel >= 4)
					{
						if (!mBuyFullGameShown)
						{
							GlobalMembers.gApp.DoTrialDialog(0);
							mBuyFullGameShown = true;
						}
						result = true;
					}
					break;
				case GameMode.MODE_BUTTERFLY:
					if (mDispPoints >= 95000)
					{
						if (!mBuyFullGameShown)
						{
							GlobalMembers.gApp.DoTrialDialog(5);
							mBuyFullGameShown = true;
						}
						result = true;
					}
					break;
				case GameMode.MODE_DIAMOND_MINE:
					if (mLevelPointsTotal >= 50000)
					{
						if (!mBuyFullGameShown)
						{
							GlobalMembers.gApp.DoTrialDialog(2);
							mBuyFullGameShown = true;
						}
						result = true;
					}
					break;
				}
			}
			return result;
		}

		public virtual bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped)
		{
			return TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, false);
		}

		public virtual bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap)
		{
			return TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, true, false);
		}

		public virtual bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol)
		{
			return TrySwap(theSelected, theSwappedRow, theSwappedCol, false, true, false);
		}

		public virtual bool TrySwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped, bool destroyTarget)
		{
			if (CheckTrialGameFinished())
			{
				return false;
			}
			if (theSelected == null)
			{
				return false;
			}
			if (theSwappedRow < 0 || theSwappedRow >= 8 || theSwappedCol < 0 || theSwappedCol >= 8)
			{
				return false;
			}
			if (!IsSwapLegal(theSelected, theSwappedRow, theSwappedCol))
			{
				return false;
			}
			if (playerSwapped)
			{
				mLastPlayerSwapColor = theSelected.mColor;
			}
			if (mHasReplayData && !mInReplay)
			{
				if (mWatchedCurReplay)
				{
					mReplayIgnoredMoves = 3;
				}
				else
				{
					mReplayIgnoredMoves++;
				}
			}
			Serialiser theBuffer = new Serialiser();
			if (SaveGame(theBuffer))
			{
				mLastMoveSave = theBuffer;
			}
			if (playerSwapped)
			{
				theSelected.mSelected = false;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SELECTOR_ALPHA, theSelected.mSelectorAlpha);
			}
			int mMoveCreditId = mCurMoveCreditId;
			PushMoveData(theSelected, theSwappedRow, theSwappedCol);
			Piece pieceAtRowCol = GetPieceAtRowCol(theSwappedRow, theSwappedCol);
			theSelected.mMoveCreditId = mMoveCreditId;
			if (pieceAtRowCol != null)
			{
				pieceAtRowCol.mMoveCreditId = mMoveCreditId;
			}
			if (theSelected.IsFlagSet(4096u))
			{
				Dictionary<Piece, bool> dictionary = new Dictionary<Piece, bool>();
				List<Piece> list = new List<Piece>();
				Piece[,] array = mBoard;
				foreach (Piece piece in array)
				{
					if (piece != null && IsPieceStill(piece) && piece.mCanScramble && !piece.IsFlagSet(6144u))
					{
						list.Add(piece);
						dictionary[piece] = true;
					}
				}
				if (mIdleTicks == 0 || list.Count < 10 || mScrambleDelayTicks >= GlobalMembers.M(150))
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_START_ROTATE);
					return true;
				}
				int mMoveCreditId2 = mCurMoveCreditId;
				mLastMoveSave.Clear();
				SaveGame(mLastMoveSave);
				PushMoveData(theSelected, theSwappedRow, theSwappedCol);
				for (int k = 0; k < list.Count; k++)
				{
					Piece piece2 = list[k];
					piece2.mDestCol = piece2.mCol;
					piece2.mDestRow = piece2.mRow;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_DEST_PCT_A, piece2.mDestPct);
				}
				mLastMatchTick = mIdleTicks;
				mScrambleDelayTicks = 200;
				bool flag = false;
				if (WriteUReplayCmd(21))
				{
					EncodePieceRef(theSelected);
					flag = true;
				}
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SCRAMBLE);
				int num = 0;
				List<Piece> list2;
				List<Piece> list3;
				while (true)
				{
					list2 = new List<Piece>();
					list3 = new List<Piece>();
					List<Piece> list4 = list;
					Dictionary<Piece, bool> dictionary2 = dictionary;
					for (int l = 0; l < list4.Count; l++)
					{
						Piece piece3 = list4[l];
						for (int m = 0; m < 8; m++)
						{
							Piece piece4 = mBoard[piece3.mRow, m];
							if (piece4 != null && piece4 != piece3 && dictionary2.ContainsKey(piece4))
							{
								list2.Add(piece3);
								list3.Add(piece4);
								break;
							}
						}
					}
					bool flag2 = false;
					for (int n = 0; n < 2; n++)
					{
						for (int num2 = 0; num2 < list2.Count; num2++)
						{
							int index = ((n == 0) ? num2 : (list2.Count - 1 - num2));
							Piece piece5 = list2[index];
							Piece piece6 = list3[index];
							Piece piece7 = mBoard[piece5.mRow, piece5.mCol];
							mBoard[piece5.mRow, piece5.mCol] = mBoard[piece6.mRow, piece6.mCol];
							mBoard[piece6.mRow, piece6.mCol] = piece7;
							int mCol = piece5.mCol;
							piece5.mCol = piece6.mCol;
							piece6.mCol = mCol;
							mCol = piece5.mRow;
							piece5.mRow = piece6.mRow;
							piece6.mRow = mCol;
							float num3 = piece5.mX;
							piece5.mX = piece6.mX;
							piece6.mX = num3;
							num3 = piece5.mY;
							piece5.mY = piece6.mY;
							piece6.mY = num3;
							piece5.mMoveCreditId = mMoveCreditId2;
						}
						if (n == 0 && (HasSet() || num == 250))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						break;
					}
					num++;
				}
				if (flag)
				{
					mUReplayBuffer.WriteByte((byte)list2.Count);
					for (int num4 = 0; num4 < list2.Count; num4++)
					{
						EncodePieceRef(list2[num4]);
						EncodePieceRef(list3[num4]);
					}
				}
				if (--mScrambleUsesLeft == 0)
				{
					DeletePiece(theSelected);
				}
				return true;
			}
			if (theSelected.IsFlagSet(2u) && pieceAtRowCol != null)
			{
				if (!pieceAtRowCol.mCanDestroy)
				{
					return false;
				}
				mWantHintTicks = 0;
				DecrementAllCounterGems(false);
				DoHypercube(theSelected, pieceAtRowCol);
				return true;
			}
			if (pieceAtRowCol != null && pieceAtRowCol.IsFlagSet(2u) && pieceAtRowCol != null)
			{
				if (!theSelected.mCanDestroy)
				{
					return false;
				}
				mWantHintTicks = 0;
				DecrementAllCounterGems(false);
				DoHypercube(pieceAtRowCol, theSelected);
				return true;
			}
			if (playerSwapped)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_START_ROTATE);
			}
			if (WriteUReplayCmd(1))
			{
				Piece pieceAtRowCol2 = GetPieceAtRowCol(theSwappedRow, theSwappedCol);
				mUReplayBuffer.WriteByte(0);
				EncodePieceRef(theSelected);
				EncodePieceRef(pieceAtRowCol2);
			}
			theSelected.mLastActiveTick = mUpdateCnt;
			if (pieceAtRowCol != null)
			{
				pieceAtRowCol.mLastActiveTick = mUpdateCnt - 1;
			}
			SwapData swapData = new SwapData();
			swapData.mPiece1 = theSelected;
			swapData.mPiece2 = pieceAtRowCol;
			swapData.mSwapDir = new Point(theSwappedCol - theSelected.mCol, theSwappedRow - theSelected.mRow);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SWAP_PCT_1, swapData.mSwapPct);
			if (SexyFramework.GlobalMembers.gIs3D)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_SCALE_1, swapData.mGemScale);
			}
			swapData.mSwapPct.mIncRate *= GetSwapSpeed();
			swapData.mGemScale.mIncRate *= GetSwapSpeed();
			swapData.mForwardSwap = true;
			swapData.mHoldingSwap = 0;
			swapData.mIgnore = false;
			swapData.mForceSwap = forceSwap;
			swapData.mDestroyTarget = destroyTarget;
			mSwapDataVector.Add(swapData);
			return true;
		}

		public bool TrySwapAndRecord(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap)
		{
			return TrySwapAndRecord(theSelected, theSwappedRow, theSwappedCol, forceSwap, true);
		}

		public bool TrySwapAndRecord(Piece theSelected, int theSwappedRow, int theSwappedCol)
		{
			return TrySwapAndRecord(theSelected, theSwappedRow, theSwappedCol, false, true);
		}

		public bool TrySwapAndRecord(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped)
		{
			if (TrySwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped))
			{
				return true;
			}
			return false;
		}

		public virtual void PieceDestroyedInSwap(Piece thePiece)
		{
		}

		public void BumpColumn(Piece thePiece, float thePower)
		{
			float num = 0f;
			for (int num2 = 7; num2 >= 0; num2--)
			{
				float num3 = 0f;
				for (int i = thePiece.mCol; i <= thePiece.mCol; i++)
				{
					Piece pieceAtRowCol = GetPieceAtRowCol(num2, i);
					if (pieceAtRowCol != null && pieceAtRowCol.mY < thePiece.mY)
					{
						float num4 = Math.Abs(pieceAtRowCol.mY - thePiece.mY);
						float num5 = Math.Min(1f, num4 / BumpColumn_MAX_DIST);
						num5 = 1f - num5;
						num3 = thePower * GlobalMembers.M(-3.75f) * num5;
						if (num3 > -0.9f && num3 < 0f)
						{
							num3 = 0f;
						}
						if (num == 0f)
						{
							num = num3;
						}
						pieceAtRowCol.mFallVelocity = Math.Min(pieceAtRowCol.mFallVelocity, num);
					}
					mBumpVelocities[i] = Math.Max(num, num3);
				}
			}
		}

		public void BumpColumns(int theX, int theY, float thePower)
		{
			if (mInUReplay)
			{
				return;
			}
			for (int i = 0; i < 8; i++)
			{
				float num = 0f;
				float num2 = 0f;
				Piece piece = null;
				for (int num3 = 7; num3 >= -1; num3--)
				{
					Piece pieceAtRowCol = GetPieceAtRowCol(num3, i);
					bool flag = false;
					float num4;
					float num5;
					if (pieceAtRowCol != null && pieceAtRowCol.GetScreenY() + 50f < (float)theY)
					{
						num4 = pieceAtRowCol.GetScreenX() + 50f - (float)theX;
						num5 = pieceAtRowCol.GetScreenY() + 50f - (float)theY;
						flag = true;
					}
					else
					{
						if (num3 != -1)
						{
							continue;
						}
						num4 = GetColScreenX(i) + 50 - theX;
						num5 = GetRowScreenY(num3) + 50 - theY;
					}
					float num6 = (float)Math.Atan2(num5, num4);
					float num7 = (float)Math.Sqrt(num4 * num4 + num5 * num5) / 100f;
					float num8 = thePower / (Math.Max(0f, num7 - GlobalMembers.M(1f)) + GlobalMembers.M(1f)) * Math.Abs((float)Math.Sin(num6));
					num2 = num8 * GlobalMembers.M(-5.25f);
					if (num2 > -0.9f && num2 < 0f)
					{
						num2 = 0f;
					}
					if (!flag)
					{
						continue;
					}
					if (num == 0f)
					{
						num = num2;
					}
					pieceAtRowCol.mFallVelocity = Math.Min(pieceAtRowCol.mFallVelocity, num);
					if (IsPieceSwapping(pieceAtRowCol))
					{
						piece = null;
						for (int j = num3; j < 8; j++)
						{
							Piece piece2 = mBoard[j, i];
							if (piece2 != null)
							{
								piece2.mFallVelocity = Math.Max(0f, piece2.mFallVelocity);
							}
						}
					}
					else if (piece == null)
					{
						piece = pieceAtRowCol;
					}
				}
				if (piece != null && WriteUReplayCmd(12))
				{
					EncodePieceRef(piece);
					mUReplayBuffer.WriteShort((short)(piece.mFallVelocity / 100f * 256f * 100f));
				}
				mBumpVelocities[i] = Math.Min(num, num2);
			}
		}

		public virtual void CelDestroyedBySpecial(int theCol, int theRow)
		{
		}

		public void UpdateLightning()
		{
			new List<Piece>();
			bool flag = WantsCalmEffects();
			GlobalMembers.M(0.05f);
			int num = 0;
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.mIsElectrocuting)
				{
					num++;
				}
			}
			int num2 = 0;
			mBoardDarken = Math.Max(mBoardDarken - 0.02f, 0f);
			if (mLightningStorms.Count > 0)
			{
				mBoardDarken = Math.Min(mBoardDarken + 0.05f, 1f);
			}
			for (int k = 0; k < mLightningStorms.Count; k++)
			{
				LightningStorm lightningStorm = mLightningStorms[k];
				lightningStorm.Update();
				bool flag2 = false;
				switch ((LightningStorm.STORM)lightningStorm.mStormType)
				{
				case LightningStorm.STORM.STORM_HORZ:
				case LightningStorm.STORM.STORM_VERT:
				case LightningStorm.STORM.STORM_BOTH:
				case LightningStorm.STORM.STORM_SHORT:
				case LightningStorm.STORM.STORM_STAR:
				case LightningStorm.STORM.STORM_SCREEN:
				case LightningStorm.STORM.STORM_FLAMING:
				{
					int num3 = ((lightningStorm.mStormType == 6) ? 1 : 0);
					bool flag3 = true;
					for (int l = 1; l < lightningStorm.mPieceIds.Count; l++)
					{
						Piece pieceById = GetPieceById(lightningStorm.mPieceIds[l]);
						if (pieceById == null || !pieceById.mCanDestroy)
						{
							continue;
						}
						if (flag)
						{
							pieceById.mElectrocutePercent += GlobalMembers.M(0.01f);
						}
						else
						{
							pieceById.mElectrocutePercent += GlobalMembers.M(0.015f);
						}
						if (pieceById.mElectrocutePercent >= 1f)
						{
							Piece pieceById2 = GetPieceById(lightningStorm.mElectrocuterId);
							if (!pieceById.mDestructing)
							{
								SetMoveCredit(pieceById, lightningStorm.mMoveCreditId);
								pieceById.mMatchId = lightningStorm.mMatchId;
								pieceById.mExplodeSourceId = lightningStorm.mElectrocuterId;
								pieceById.mExplodeSourceFlags |= lightningStorm.mStartPieceFlags;
								if (pieceById.IsFlagSet(16u))
								{
									pieceById.mExplodeDelay = 5;
								}
								else if (!TriggerSpecial(pieceById, pieceById2) && !pieceById.mDestructing)
								{
									pieceById.mExplodeDelay = 1;
								}
							}
							lightningStorm.mPieceIds.RemoveAt(l);
							l--;
						}
						else
						{
							flag3 = false;
						}
					}
					for (int m = 0; m < lightningStorm.mElectrocutedCelVector.Count; m++)
					{
						ElectrocutedCel electrocutedCel = lightningStorm.mElectrocutedCelVector[m];
						if (flag)
						{
							electrocutedCel.mElectrocutePercent += GlobalMembers.M(0.01f);
						}
						else
						{
							electrocutedCel.mElectrocutePercent += GlobalMembers.M(0.015f);
						}
						if (electrocutedCel.mElectrocutePercent >= 1f)
						{
							CelDestroyedBySpecial(electrocutedCel.mCol, electrocutedCel.mRow);
							lightningStorm.mElectrocutedCelVector.RemoveAt(m);
							m--;
						}
					}
					lightningStorm.mTimer -= 0.01f;
					if (!(lightningStorm.mTimer <= 0f))
					{
						break;
					}
					if (flag)
					{
						lightningStorm.mTimer = GlobalMembers.M(0.15f);
					}
					else
					{
						lightningStorm.mTimer = GlobalMembers.M(0.1f);
					}
					int[,] array2 = new int[8, 2]
					{
						{ 1, 0 },
						{ -1, 0 },
						{ 0, 1 },
						{ 0, -1 },
						{ -1, -1 },
						{ -1, 1 },
						{ 1, -1 },
						{ 1, 1 }
					};
					int num4 = 0;
					int num5 = 4;
					if (lightningStorm.mStormType == 0)
					{
						num5 = 2;
					}
					if (lightningStorm.mStormType == 1)
					{
						num4 = 2;
					}
					if (lightningStorm.mStormType == 4)
					{
						num5 = 8;
					}
					if (lightningStorm.mStormType == 5)
					{
						int num6 = Math.Min(lightningStorm.mDist, 7);
						int num7 = 0;
						for (int n = -num6; n <= num6; n++)
						{
							array2[num7, 0] = n;
							array2[num7++, 1] = -num6;
							array2[num7, 0] = n;
							array2[num7++, 1] = num6;
						}
						for (int num8 = -num6 + 1; num8 <= num6 - 1; num8++)
						{
							array2[num7, 0] = -num6;
							array2[num7++, 1] = num8;
							array2[num7, 0] = num6;
							array2[num7++, 1] = num8;
						}
						num5 = num7;
					}
					for (int num9 = num4; num9 < num5; num9++)
					{
						for (int num10 = -num3; num10 <= num3; num10++)
						{
							int num11 = ((lightningStorm.mStormType == 5) ? 1 : lightningStorm.mDist);
							int num12 = lightningStorm.mCX + (num11 * array2[num9, 0] + array2[num9, 1] * num10) * 100;
							int num13 = lightningStorm.mCY + (num11 * array2[num9, 1] + array2[num9, 0] * num10) * 100;
							Piece pieceAtXY = GetPieceAtXY(num12, num13);
							if (num11 > lightningStorm.mStormLength || num12 < 0 || num12 >= GetColX(8) || num13 < 0 || num13 >= GetRowY(8))
							{
								continue;
							}
							if (num12 != lightningStorm.mCX || num13 != lightningStorm.mCY)
							{
								ElectrocutedCel electrocutedCel2 = new ElectrocutedCel();
								electrocutedCel2.mCol = GetColAt(num12);
								electrocutedCel2.mRow = GetRowAt(num13);
								electrocutedCel2.mElectrocutePercent = 0.01f;
								lightningStorm.mElectrocutedCelVector.Add(electrocutedCel2);
							}
							if (pieceAtXY == null || pieceAtXY.mDestructing)
							{
								continue;
							}
							bool flag4 = false;
							for (int num14 = 0; num14 < mLightningStorms.Count; num14++)
							{
								for (int num15 = 0; num15 < mLightningStorms[num14].mPieceIds.Count; num15++)
								{
									if (pieceAtXY.mId == mLightningStorms[num14].mPieceIds[num15])
									{
										flag4 = true;
									}
								}
							}
							if (flag4)
							{
								continue;
							}
							flag3 = false;
							if (pieceAtXY.mElectrocutePercent == 0f)
							{
								lightningStorm.mPieceIds.Add(pieceAtXY.mId);
								pieceAtXY.mElectrocutePercent = 0.01f;
								for (int num16 = 0; num16 < 8; num16++)
								{
									Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_SPARKLE_SHARD);
									effect.mX = (float)(num12 + GetBoardX()) + GlobalMembersUtils.GetRandFloat() * (float)Math.Abs(array2[num9, 0]) * 100f / 3f;
									effect.mY = (float)(num13 + GetBoardY()) + GlobalMembersUtils.GetRandFloat() * (float)Math.Abs(array2[num9, 1]) * 100f / 3f;
									effect.mDX = (int)((double)GlobalMembersUtils.GetRandFloat() * ((double)Math.Abs(array2[num9, 1]) + 0.5) * (double)GlobalMembers.M(10));
									effect.mDY = (int)((double)GlobalMembersUtils.GetRandFloat() * ((double)Math.Abs(array2[num9, 0]) + 0.5) * (double)GlobalMembers.M(10));
									mPostFXManager.AddEffect(effect);
								}
							}
							pieceAtXY.mShakeScale = Math.Min(1f, Math.Max(pieceAtXY.mShakeScale, pieceAtXY.mElectrocutePercent));
						}
					}
					if (lightningStorm.mDist == 0)
					{
						if (flag)
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ELECTRO_EXPLODE, 0, GlobalMembers.M(0.6), GlobalMembers.M(-1.0));
						}
						else
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ELECTRO_EXPLODE);
						}
					}
					lightningStorm.mDist++;
					int mStormLength = lightningStorm.mStormLength;
					if (lightningStorm.mDist < mStormLength)
					{
						flag3 = false;
					}
					if (!flag3)
					{
						break;
					}
					mComboBonusSlowdownPct = 1f;
					Piece pieceById3 = GetPieceById(lightningStorm.mPieceIds[0]);
					if (pieceById3 != null)
					{
						pieceById3.ClearFlag(4u);
						if (pieceById3.IsFlagSet(2u))
						{
							TriggerSpecial(pieceById3, pieceById3);
						}
						else
						{
							CelDestroyedBySpecial(pieceById3.mCol, pieceById3.mRow);
							pieceById3.mExplodeDelay = 1;
							pieceById3.mExplodeSourceId = pieceById3.mId;
							pieceById3.mExplodeSourceFlags |= pieceById3.mFlags;
						}
					}
					lightningStorm.mHoldDelay -= GlobalMembers.M(0.25f);
					for (int num17 = 0; num17 < 8; num17++)
					{
						for (int num18 = 0; num18 < 8; num18++)
						{
							Piece piece2 = mBoard[num17, num18];
							if (piece2 != null)
							{
								piece2.mFallVelocity = 0f;
							}
						}
					}
					if (lightningStorm.mHoldDelay <= 0f)
					{
						flag2 = true;
					}
					break;
				}
				}
				if (lightningStorm.mDoneDelay > 0 && --lightningStorm.mDoneDelay == 0)
				{
					flag2 = true;
				}
				if (flag2)
				{
					lightningStorm.Dispose();
					mLightningStorms.RemoveAt(k);
					if (mLightningStorms.Count == 0 && !mInUReplay)
					{
						FillInBlanks();
					}
					k--;
				}
				else
				{
					if (lightningStorm.mUpdateCnt < num2)
					{
						break;
					}
					num2 += GlobalMembers.M(25);
				}
			}
		}

		public int FindStormIdxFor(Piece thePiece)
		{
			for (int i = 0; i < mLightningStorms.Count; i++)
			{
				LightningStorm lightningStorm = mLightningStorms[i];
				if (lightningStorm.mElectrocuterId == thePiece.mId)
				{
					return i;
				}
			}
			return -1;
		}

		public void DrawLightning(Graphics g)
		{
			for (int i = 0; i < mLightningStorms.Count; i++)
			{
				LightningStorm lightningStorm = mLightningStorms[i];
				lightningStorm.Draw(g);
			}
		}

		public void ExplodeAtHelper(int theX, int theY)
		{
			int num = theX;
			int num2 = theY;
			Piece pieceAtScreenXY = GetPieceAtScreenXY(theX, theY);
			if (pieceAtScreenXY != null)
			{
				num = (int)(pieceAtScreenXY.GetScreenX() + 50f);
				num2 = (int)(pieceAtScreenXY.GetScreenY() + 50f);
			}
			GlobalMembers.gExplodePoints[GlobalMembers.gExplodeCount, 0] = num;
			GlobalMembers.gExplodePoints[GlobalMembers.gExplodeCount, 1] = num2;
			GlobalMembers.gExplodeCount++;
			Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_LIGHT);
			effect.mFlags = 2;
			effect.mX = theX;
			effect.mY = theY;
			effect.mZ = GlobalMembers.M(0.08f);
			effect.mValue[0] = GlobalMembers.M(45.1f);
			effect.mValue[1] = GlobalMembers.M(-0.5f);
			effect.mAlpha = GlobalMembers.M(0.3f);
			effect.mDAlpha = GlobalMembers.M(0.06f);
			effect.mScale = GlobalMembers.M(300f);
			mPostFXManager.AddEffect(effect);
			int[,] array = new int[9, 2]
			{
				{ -1, -1 },
				{ 0, -1 },
				{ 1, -1 },
				{ -1, 0 },
				{ 1, 0 },
				{ -1, 1 },
				{ 0, 1 },
				{ 1, 1 },
				{ 0, 0 }
			};
			int[,] array2 = new int[13, 2]
			{
				{ -1, -1 },
				{ 0, -1 },
				{ 1, -1 },
				{ -1, 0 },
				{ 1, 0 },
				{ -1, 1 },
				{ 0, 1 },
				{ 1, 1 },
				{ 0, -2 },
				{ -2, 0 },
				{ 2, 0 },
				{ 0, 2 },
				{ 0, 0 }
			};
			int num3 = (HasLargeExplosions() ? 13 : 9);
			int[,] array3 = ((num3 == 9) ? array : array2);
			for (int i = 0; i < num3; i++)
			{
				int num4 = theX + array3[i, 0] * 100;
				int num5 = theY + array3[i, 1] * 100;
				int num6;
				int num7;
				if (pieceAtScreenXY != null)
				{
					num6 = pieceAtScreenXY.mCol + array3[i, 0];
					num7 = pieceAtScreenXY.mRow + array3[i, 1];
				}
				else
				{
					num6 = GetColAt(theX - GetBoardX());
					num7 = GetRowAt(theY - GetBoardY());
				}
				if (num6 >= 0 && num6 < 8 && num7 >= 0 && num7 < 8)
				{
					CelDestroyedBySpecial(num6, num7);
				}
				Piece pieceAtScreenXY2 = GetPieceAtScreenXY(num4, num5);
				if (pieceAtScreenXY2 == null || (pieceAtScreenXY2.mExplodeDelay != 0 && i != 8))
				{
					continue;
				}
				int num8 = (int)(pieceAtScreenXY2.GetScreenX() + 50f);
				int num9 = (int)(pieceAtScreenXY2.GetScreenY() + 50f);
				bool flag = false;
				int num10 = (int)(GlobalMembers.M(0.013f) * (float)(num4 - theX));
				int num11 = (int)(GlobalMembers.M(0.013f) * (float)(num5 - theY));
				if (array3[i, 0] != 0 || array3[i, 1] != 0)
				{
					if (WantsCalmEffects())
					{
						for (int j = 0; j < GlobalMembers.M(8); j++)
						{
							Effect effect2 = mPostFXManager.AllocEffect(Effect.Type.TYPE_SPARKLE_SHARD);
							float num12 = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
							float num13 = GlobalMembers.M(0f) + GlobalMembers.M(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect2.mDX = num13 * (float)Math.Cos(num12) + (float)num10;
							effect2.mDY = num13 * (float)Math.Sin(num12) + (float)num11;
							effect2.mDX *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect2.mDY *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							float num14 = Math.Abs(GlobalMembersUtils.GetRandFloat());
							num14 *= num14;
							effect2.mX = num14 * (float)num + (1f - num14) * (float)num8 + (float)Math.Cos(num12) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect2.mY = num14 * (float)num2 + (1f - num14) * (float)num9 + (float)Math.Sin(num12) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect2.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255));
							effect2.mIsAdditive = true;
							effect2.mDScale = GlobalMembers.M(0.015f);
							effect2.mScale = GlobalMembers.M(0.1f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1f);
							mPostFXManager.AddEffect(effect2);
						}
					}
					else
					{
						int num15 = GlobalMembers.M(3);
						for (int k = 0; k < num15; k++)
						{
							Effect effect3 = mPostFXManager.AllocEffect(Effect.Type.TYPE_STEAM);
							float num16 = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
							float num17 = GlobalMembers.M(0f) + GlobalMembers.M(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect3.mDX = num17 * (float)Math.Cos(num16) + (float)num10;
							effect3.mDY = num17 * (float)Math.Sin(num16) + (float)num11;
							effect3.mDX *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect3.mDY *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							float num18 = Math.Abs(GlobalMembersUtils.GetRandFloat());
							num18 *= num18;
							effect3.mX = num18 * (float)num + (1f - num18) * (float)num8 + (float)Math.Cos(num16) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect3.mY = num18 * (float)num2 + (1f - num18) * (float)num9 + (float)Math.Sin(num16) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
							effect3.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(96), GlobalMembers.M(32), GlobalMembers.M(64));
							effect3.mIsAdditive = true;
							effect3.mDScale = GlobalMembers.M(0.015f);
							effect3.mScale = GlobalMembers.M(0.1f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1f);
							mPostFXManager.AddEffect(effect3);
						}
					}
					int num19 = GlobalMembers.M(5);
					for (int l = 0; l < num19; l++)
					{
						Effect effect4 = mPostFXManager.AllocEffect(Effect.Type.TYPE_GEM_SHARD);
						effect4.mColor = GlobalMembers.gGemColors[pieceAtScreenXY2.mColor];
						float num20 = GlobalMembers.M(1.2f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1.2f);
						float num21;
						if (num10 != 0 || num11 != 0)
						{
							num21 = GlobalMembersUtils.GetRandFloat() * 3.14159f;
							int num22 = (int)(GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(48f));
							effect4.mX = num8 + (int)(GlobalMembers.M(1f) * (float)num22 * (float)Math.Cos(num21));
							effect4.mY = num9 + (int)(GlobalMembers.M(1f) * (float)num22 * (float)Math.Sin(num21));
							num21 = (float)Math.Atan2(effect4.mY - (float)num2, effect4.mX - (float)num) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.3f);
							num20 = GlobalMembers.M(3.5f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1f);
							effect4.mDX = (float)Math.Cos(num21) * num20;
							effect4.mDY = (float)Math.Sin(num21) * num20 + GlobalMembers.M(-2f);
							effect4.mDecel = (float)(GlobalMembers.M(0.98) + (double)(GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.005f)));
						}
						else
						{
							int num23 = l * (int)((float)(l + 120) / 120f);
							num21 = (float)((double)l * 0.503 + (double)((float)(BejeweledLivePlus.Misc.Common.Rand() % 100) / 800f));
							effect4.mDX = (float)Math.Cos(num21) * num20 + (float)num10;
							effect4.mDY = (float)Math.Sin(num21) * num20 + GlobalMembers.MS(-2f) + (float)num11;
							effect4.mX = (float)(num8 + (int)(GlobalMembers.M(1.2f) * (float)num23 * effect4.mDX)) + GlobalMembers.M(14f);
							effect4.mY = (float)(num9 + (int)(GlobalMembers.M(1.2f) * (float)num23 * effect4.mDY)) + GlobalMembers.M(10f);
						}
						effect4.mAngle = num21;
						effect4.mDAngle = GlobalMembers.M(0.05f) * GlobalMembersUtils.GetRandFloat();
						effect4.mGravity = GlobalMembers.M(0.06f);
						effect4.mValue[0] = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f;
						effect4.mValue[1] = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f;
						effect4.mValue[2] = GlobalMembers.M(0.045f) * (GlobalMembers.M(3f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(1f));
						effect4.mValue[3] = GlobalMembers.M(0.045f) * (GlobalMembers.M(3f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(1f));
						effect4.mDAlpha = (float)(GlobalMembers.M(-0.0025) * (double)(GlobalMembers.M(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(4f)));
						mPostFXManager.AddEffect(effect4);
					}
					int num24 = GlobalMembers.M(3);
					for (int m = 0; m < num24; m++)
					{
						Effect effect5 = mPostFXManager.AllocEffect(Effect.Type.TYPE_STEAM);
						float num25 = (float)m * (float)Math.PI * 2f / 20f;
						float num26 = GlobalMembers.M(0.5f) + GlobalMembers.M(5.75f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect5.mDX = num26 * (float)Math.Cos(num25) + (float)num10;
						effect5.mDY = num26 * (float)Math.Sin(num25) + (float)num11;
						effect5.mX = (float)num8 + (float)Math.Cos(num25) * GlobalMembers.M(25f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect5.mY = (float)num9 + (float)Math.Sin(num25) * GlobalMembers.M(25f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect5.mIsAdditive = false;
						effect5.mScale = GlobalMembers.M(0.5f);
						effect5.mDScale = GlobalMembers.M(0.005f);
						effect5.mValue[1] *= 1f - Math.Abs(GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.5f));
						effect5.mColor = new Color(128, 128, 128);
						mPostFXManager.AddEffect(effect5);
					}
					if (pieceAtScreenXY2.IsFlagSet(1u))
					{
						flag = true;
					}
				}
				if (pieceAtScreenXY2.mImmunityCount != 0)
				{
					continue;
				}
				if (pieceAtScreenXY != null)
				{
					SetMoveCredit(pieceAtScreenXY2, pieceAtScreenXY.mMoveCreditId);
					if (pieceAtScreenXY.mMatchId == -1)
					{
						pieceAtScreenXY.mMatchId = mCurMoveCreditId++;
					}
					pieceAtScreenXY2.mMatchId = pieceAtScreenXY.mMatchId;
				}
				if (flag)
				{
					if (WantsCalmEffects())
					{
						pieceAtScreenXY2.mExplodeDelay = GlobalMembers.M(25);
					}
					else
					{
						pieceAtScreenXY2.mExplodeDelay = GlobalMembers.M(15);
					}
					pieceAtScreenXY2.mExplodeSourceId = pieceAtScreenXY.mId;
					pieceAtScreenXY2.mExplodeSourceFlags |= pieceAtScreenXY.mFlags;
				}
				else if (pieceAtScreenXY2.IsFlagSet(16u))
				{
					pieceAtScreenXY2.mExplodeDelay = GlobalMembers.M(5);
					pieceAtScreenXY2.mExplodeSourceId = pieceAtScreenXY.mId;
					pieceAtScreenXY2.mExplodeSourceFlags |= pieceAtScreenXY.mFlags;
				}
				else
				{
					if (mInUReplay)
					{
						continue;
					}
					if (pieceAtScreenXY2.IsFlagSet(4u))
					{
						int num27 = FindStormIdxFor(pieceAtScreenXY2);
						if (num27 != -1)
						{
							LightningStorm lightningStorm = mLightningStorms[num27];
							if (lightningStorm.mUpdateCnt == 0 && (lightningStorm.mStormType == 0 || lightningStorm.mStormType == 1))
							{
								lightningStorm.Dispose();
								mLightningStorms.RemoveAt(num27);
								pieceAtScreenXY2.mDestructing = false;
							}
						}
					}
					if (((pieceAtScreenXY2.IsFlagSet(524289u) && !pieceAtScreenXY2.IsFlagSet(4u)) || !TriggerSpecial(pieceAtScreenXY2, pieceAtScreenXY)) && pieceAtScreenXY2.mCanDestroy)
					{
						pieceAtScreenXY2.mIsExploding = true;
						if (pieceAtScreenXY != null)
						{
							pieceAtScreenXY2.mExplodeSourceId = pieceAtScreenXY.mId;
							pieceAtScreenXY2.mExplodeSourceFlags |= pieceAtScreenXY.mFlags;
						}
						AddPoints((int)pieceAtScreenXY2.CX(), (int)pieceAtScreenXY2.GetScreenY(), GlobalMembers.M(20), GlobalMembers.gGemColors[pieceAtScreenXY2.mColor], (uint)pieceAtScreenXY2.mMatchId, true, true, pieceAtScreenXY2.mMoveCreditId);
						DeletePiece(pieceAtScreenXY2);
					}
				}
			}
		}

		public virtual void ExplodeAt(int theX, int theY)
		{
			GlobalMembers.gExplodeCount = 0;
			GlobalMembers.gShardCount = 0;
			if (WriteUReplayCmd(10))
			{
				mUReplayBuffer.WriteShort(EncodeX(theX));
				mUReplayBuffer.WriteShort(EncodeY(theY));
			}
			
			int aChainReactionCount = 0;
			Piece aPiece = GetPieceAtScreenXY(theX, theY);
			if (aPiece != null)
			{
				aChainReactionCount = GetMoveStat(aPiece.mMoveCreditId, 3);
			}

			if (WantsCalmEffects())
			{
				BumpColumns(theX, theY, GlobalMembers.M(0.6f));
			}
			else
			{
				BumpColumns(theX, theY, GlobalMembers.M(1f));
			}
			ExplodeAtHelper(theX, theY);
			if (GlobalMembers.M(1) != 0)
			{
				PopAnimEffect popAnimEffect = PopAnimEffect.fromPopAnim(GlobalMembersResourcesWP.POPANIM_FLAMEGEMEXPLODE);
				popAnimEffect.mX = theX;
				popAnimEffect.mY = theY;
				popAnimEffect.mOverlay = true;
				popAnimEffect.mScale = 2f;
				if (!WantsCalmEffects())
				{
					popAnimEffect.mDoubleSpeed = true;
				}
				popAnimEffect.Play();
				mPostFXManager.AddEffect(popAnimEffect);
				mPostFXManager.AddHeatwave(theX, theY, aChainReactionCount + 1);
			}
		}

		public void SmallExplodeAt(Piece thePiece, float theCenterX, float theCenterY, bool process, bool fromFlame)
		{
			if (WriteUReplayCmd(11))
			{
				EncodePieceRef(thePiece);
			}
			int num = (int)thePiece.GetScreenX();
			int num2 = (int)thePiece.GetScreenY();
			int num3 = num + 50;
			int num4 = num2 + 50;
			if (!thePiece.IsFlagSet(6144u))
			{
				AddPoints(num3, num2, GlobalMembers.M(50), GlobalMembers.gGemColors[thePiece.mColor], (uint)thePiece.mMatchId, true, true, thePiece.mMoveCreditId);
			}
			if (thePiece.IsFlagSet(1024u) && !mInUReplay)
			{
				DeletePiece(thePiece);
				return;
			}
			float num5 = GlobalMembers.M(0.01f) * ((float)num3 - theCenterX);
			float num6 = GlobalMembers.M(0.01f) * ((float)num4 - theCenterY);
			if (num5 == 0f)
			{
				num6 *= GlobalMembers.M(2f);
			}
			if (num6 == 0f)
			{
				num5 *= GlobalMembers.M(2f);
			}
			if (WantsCalmEffects())
			{
				int num7 = GlobalMembers.M(3);
				for (int i = 0; i < num7; i++)
				{
					Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_SPARKLE_SHARD);
					float num8 = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
					float num9 = GlobalMembers.M(0f) + GlobalMembers.M(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
					effect.mDX = num9 * (float)Math.Cos(num8) + num5;
					effect.mDY = num9 * (float)Math.Sin(num8) + num6;
					if (fromFlame)
					{
						effect.mDX *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect.mDY *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						float num10 = Math.Abs(GlobalMembersUtils.GetRandFloat());
						num10 *= num10;
						effect.mX = num10 * theCenterX + (1f - num10) * (float)num3 + (float)Math.Cos(num8) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect.mY = num10 * theCenterY + (1f - num10) * (float)num4 + (float)Math.Sin(num8) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(96), GlobalMembers.M(32), GlobalMembers.M(64));
						effect.mIsAdditive = true;
						effect.mDScale = GlobalMembers.M(0.015f);
					}
					else
					{
						effect.mX = (float)num3 + (float)Math.Cos(num8) * GlobalMembers.M(24f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect.mY = (float)num4 + (float)Math.Sin(num8) * GlobalMembers.M(24f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect.mColor = GlobalMembers.gGemColors[thePiece.mColor];
						effect.mIsAdditive = false;
						effect.mDScale = GlobalMembers.M(0.02f);
					}
					effect.mScale = GlobalMembers.M(0.1f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1f);
					mPostFXManager.AddEffect(effect);
				}
			}
			else
			{
				int num11 = GlobalMembers.M(3);
				for (int j = 0; j < num11; j++)
				{
					Effect effect2 = mPostFXManager.AllocEffect(Effect.Type.TYPE_STEAM);
					float num12 = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
					float num13 = GlobalMembers.M(0f) + GlobalMembers.M(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
					effect2.mDX = num13 * (float)Math.Cos(num12) + num5;
					effect2.mDY = num13 * (float)Math.Sin(num12) + num6;
					if (fromFlame)
					{
						effect2.mDX *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect2.mDY *= GlobalMembers.M(2.5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						float num14 = Math.Abs(GlobalMembersUtils.GetRandFloat());
						num14 *= num14;
						effect2.mX = num14 * theCenterX + (1f - num14) * (float)num3 + (float)Math.Cos(num12) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect2.mY = num14 * theCenterY + (1f - num14) * (float)num4 + (float)Math.Sin(num12) * GlobalMembers.M(64f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect2.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(96), GlobalMembers.M(32), GlobalMembers.M(64));
						effect2.mIsAdditive = true;
						effect2.mDScale = GlobalMembers.M(0.015f);
					}
					else
					{
						effect2.mX = (float)num3 + (float)Math.Cos(num12) * GlobalMembers.M(24f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect2.mY = (float)num4 + (float)Math.Sin(num12) * GlobalMembers.M(24f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
						effect2.mColor = GlobalMembers.gGemColors[thePiece.mColor];
						effect2.mIsAdditive = false;
						effect2.mDScale = GlobalMembers.M(0.02f);
					}
					effect2.mScale = GlobalMembers.M(0.1f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1f);
					mPostFXManager.AddEffect(effect2);
				}
			}
			int num15 = GlobalMembers.M(3);
			for (int k = 0; k < num15; k++)
			{
				Effect effect3 = mPostFXManager.AllocEffect(Effect.Type.TYPE_GEM_SHARD);
				effect3.mColor = GlobalMembers.gGemColors[thePiece.mColor];
				float num16 = GlobalMembers.M(1.2f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1.2f);
				float num17;
				if (num5 != 0f || num6 != 0f)
				{
					num17 = GlobalMembersUtils.GetRandFloat() * 3.14159f;
					int num18 = (int)(GlobalMembersUtils.GetRandFloat() * GlobalMembers.S(GlobalMembers.M(48f)));
					effect3.mX = num3 + (int)(GlobalMembers.M(1f) * (float)num18 * (float)Math.Cos(num17));
					effect3.mY = num4 + (int)(GlobalMembers.M(1f) * (float)num18 * (float)Math.Sin(num17));
					num17 = (float)Math.Atan2(effect3.mY - theCenterY, effect3.mX - theCenterX) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.3f);
					num16 = GlobalMembers.M(3.5f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1f);
					effect3.mDX = (float)Math.Cos(num17) * num16;
					effect3.mDY = (float)Math.Sin(num17) * num16 + GlobalMembers.M(-2f);
					effect3.mDecel = (float)(GlobalMembers.M(0.98) + (double)(GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.005f)));
				}
				else
				{
					int num19 = GlobalMembers.S(k * (int)((float)(k + 120) / 120f));
					num17 = (float)k * 0.503f + (float)(BejeweledLivePlus.Misc.Common.Rand() % 100) / 800f;
					effect3.mDX = (float)Math.Cos(num17) * num16 + num5;
					effect3.mDY = (float)Math.Sin(num17) * num16 + GlobalMembers.M(-2f) + num6;
					effect3.mX = (float)(num3 + (int)(GlobalMembers.M(1.2f) * (float)num19 * effect3.mDX)) + GlobalMembers.M(14f);
					effect3.mY = (float)(num4 + (int)(GlobalMembers.M(1.2f) * (float)num19 * effect3.mDY)) + GlobalMembers.M(10f);
				}
				effect3.mAngle = num17;
				effect3.mDAngle = GlobalMembers.M(0.05f) * GlobalMembersUtils.GetRandFloat();
				effect3.mGravity = GlobalMembers.M(0.06f);
				effect3.mValue[0] = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f;
				effect3.mValue[1] = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f;
				effect3.mValue[2] = GlobalMembers.M(0.045f) * (GlobalMembers.M(3f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(1f));
				effect3.mValue[3] = GlobalMembers.M(0.045f) * (GlobalMembers.M(3f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(1f));
				effect3.mDAlpha = GlobalMembers.M(-0.0025f) * (GlobalMembers.M(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(4f));
				mPostFXManager.AddEffect(effect3);
			}
			int num20 = GlobalMembers.M(3);
			for (int l = 0; l < num20; l++)
			{
				Effect effect4 = mPostFXManager.AllocEffect(Effect.Type.TYPE_STEAM);
				float num21 = (float)l * (float)Math.PI * 2f / 20f;
				float num22 = GlobalMembers.M(0.5f) + GlobalMembers.M(5.75f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
				effect4.mDX = num22 * (float)Math.Cos(num21) + num5;
				effect4.mDY = num22 * (float)Math.Sin(num21) + num6;
				effect4.mX = (float)num3 + (float)Math.Cos(num21) * GlobalMembers.M(25f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
				effect4.mY = (float)num4 + (float)Math.Sin(num21) * GlobalMembers.M(25f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
				effect4.mIsAdditive = false;
				effect4.mScale = GlobalMembers.M(0.5f);
				effect4.mDScale = GlobalMembers.M(0.005f);
				effect4.mValue[1] *= 1f - Math.Abs(GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.5f));
				effect4.mColor = new Color(128, 128, 128);
				mPostFXManager.AddEffect(effect4);
			}
			if (thePiece.mElectrocutePercent > 0f)
			{
				EffectsManager effectsManager = mPostFXManager;
				int num23 = GlobalMembers.M(3);
				for (int m = 0; m < num23; m++)
				{
					Effect effect5 = effectsManager.AllocEffect(Effect.Type.TYPE_FRUIT_SPARK);
					float num24 = GlobalMembers.M(1f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(2f);
					float num25 = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
					effect5.mScale = GlobalMembers.M(1f) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.5f);
					effect5.mDX = num24 * (float)Math.Cos(num25);
					effect5.mDY = num24 * (float)Math.Sin(num25);
					effect5.mX = (float)(num + 50) + (float)Math.Cos(num25) * 100f / 2f;
					effect5.mY = (float)(num2 + 50) + (float)Math.Sin(num25) * 100f / 2f;
					effect5.mIsAdditive = true;
					effect5.mAlpha = 1f;
					effect5.mDAlpha = GlobalMembers.M(-0.005f);
					effect5.mGravity = 0f;
					effectsManager.AddEffect(effect5);
				}
			}
			if (!mInUReplay)
			{
				thePiece.mIsExploding = true;
				DeletePiece(thePiece);
			}
		}

		public bool FindRandomMove(int[] theCoords)
		{
			return FindRandomMove(theCoords, false);
		}

		public bool FindRandomMove(int[] theCoords, bool thePowerGemMove)
		{
			bool flag = BejeweledLivePlus.Misc.Common.Rand(2) == 0;
			int num = BejeweledLivePlus.Misc.Common.Rand(10);
			for (int i = 0; i < 2; i++)
			{
				for (int num2 = num; num2 >= 0; num2--)
				{
					if (FindMove(theCoords, num2, true, true, flag, null, thePowerGemMove))
					{
						return true;
					}
				}
				for (int num3 = num; num3 >= 0; num3--)
				{
					if (FindMove(theCoords, num3, true, true, !flag, null, thePowerGemMove))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool FindMove(int[] theCoords, int theMoveNum, bool horz, bool vert, bool reverse, Piece theIncludePiece)
		{
			return FindMove(theCoords, theMoveNum, horz, vert, reverse, theIncludePiece, false);
		}

		public bool FindMove(int[] theCoords, int theMoveNum, bool horz, bool vert, bool reverse)
		{
			return FindMove(theCoords, theMoveNum, horz, vert, reverse, null, false);
		}

		public bool FindMove(int[] theCoords, int theMoveNum, bool horz, bool vert)
		{
			return FindMove(theCoords, theMoveNum, horz, vert, false, null, false);
		}

		public bool FindMove(int[] theCoords, int theMoveNum, bool horz, bool vert, bool reverse, Piece theIncludePiece, bool powerGemMove)
		{
			int num = 0;
			int num2 = (reverse ? 7 : 0);
			int num3 = (reverse ? (-1) : 8);
			for (int num4 = num2; num4 != num3; num4 = ((!reverse) ? (num4 + 1) : (num4 - 1)))
			{
				for (int i = 0; i < 8; i++)
				{
					Piece piece = mBoard[num4, i];
					if (piece == null || (piece != null && (!WillPieceBeStill(piece) || piece.IsFlagSet(256u) || !piece.mCanSwap)))
					{
						continue;
					}
					for (int j = 0; j < 4; j++)
					{
						int num5 = i + FM_aSwapArray[j, 0];
						int num6 = num4 + FM_aSwapArray[j, 1];
						if (num5 < 0 || num5 >= 8 || num6 < 0 || num6 >= 8)
						{
							continue;
						}
						Piece piece2 = piece;
						bool flag = false;
						bool flag2 = theIncludePiece == null;
						if (piece != null && piece.IsFlagSet(2u) && mBoard[num6, num5] != null)
						{
							if (theIncludePiece != null && piece.mColor == theIncludePiece.mColor)
							{
								flag2 = true;
							}
							flag = true;
						}
						if (mBoard[num6, num5] != null && mBoard[num6, num5].mColor != -1 && WillPieceBeStill(mBoard[num6, num5]))
						{
							mBoard[num4, i] = mBoard[num6, num5];
							mBoard[num6, num5] = piece2;
							flag2 |= theIncludePiece == mBoard[num4, i];
							int num7 = i;
							int k = i;
							while (num7 > 0 && mBoard[num4, num7 - 1] != null && mBoard[num4, i].mColor == mBoard[num4, num7 - 1].mColor && WillPieceBeStill(mBoard[num4, num7 - 1]))
							{
								flag2 |= theIncludePiece == mBoard[num4, num7 - 1];
								num7--;
							}
							for (; k < 7 && mBoard[num4, k + 1] != null && mBoard[num4, i].mColor == mBoard[num4, k + 1].mColor && WillPieceBeStill(mBoard[num4, k + 1]); k++)
							{
								flag2 |= theIncludePiece == mBoard[num4, k + 1];
							}
							int num8 = num4;
							int l = num4;
							while (num8 > 0 && mBoard[num8 - 1, i] != null && mBoard[num4, i].mColor == mBoard[num8 - 1, i].mColor && WillPieceBeStill(mBoard[num8 - 1, i]))
							{
								flag2 |= theIncludePiece == mBoard[num8 - 1, i];
								num8--;
							}
							for (; l < 7 && mBoard[l + 1, i] != null && mBoard[num4, i].mColor == mBoard[l + 1, i].mColor && WillPieceBeStill(mBoard[l + 1, i]); l++)
							{
								flag2 |= theIncludePiece == mBoard[l + 1, i];
							}
							piece2 = mBoard[num4, i];
							mBoard[num4, i] = mBoard[num6, num5];
							mBoard[num6, num5] = piece2;
							if (powerGemMove)
							{
								if ((k - num7 >= 3 && horz) || (l - num8 >= 3 && vert))
								{
									flag = true;
								}
								if (k - num7 >= 2 && horz && l - num8 >= 2 && vert)
								{
									flag = true;
								}
							}
							else if ((k - num7 >= 2 && horz) || (l - num8 >= 2 && vert))
							{
								flag = true;
							}
						}
						if (!flag || !flag2)
						{
							continue;
						}
						if (num == theMoveNum)
						{
							if (theCoords != null)
							{
								theCoords[0] = i;
								theCoords[1] = num4;
								theCoords[2] = num5;
								theCoords[3] = num6;
							}
							return true;
						}
						num++;
					}
				}
			}
			return false;
		}

		public bool HasSet()
		{
			return HasSet(null);
		}

		public bool HasSet(Piece theCheckPiece)
		{
			for (int i = 0; i < 8; i++)
			{
				int num = 0;
				int num2 = -1;
				bool flag = false;
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					if (piece != null)
					{
						if (piece.mColor != -1 && piece.mColor == num2)
						{
							flag = flag || piece == theCheckPiece;
							if (++num >= 3 && flag)
							{
								return true;
							}
						}
						else
						{
							num2 = piece.mColor;
							num = 1;
							flag = piece == theCheckPiece || theCheckPiece == null;
						}
					}
					else
					{
						num2 = -1;
					}
				}
			}
			for (int j = 0; j < 8; j++)
			{
				int num3 = 0;
				int num4 = -1;
				bool flag2 = false;
				for (int i = 0; i < 8; i++)
				{
					Piece piece2 = mBoard[i, j];
					if (piece2 != null)
					{
						if (piece2.mColor != -1 && piece2.mColor == num4)
						{
							flag2 = flag2 || piece2 == theCheckPiece;
							if (++num3 >= 3 && flag2)
							{
								return true;
							}
						}
						else
						{
							num4 = piece2.mColor;
							num3 = 1;
							flag2 = piece2 == theCheckPiece || theCheckPiece == null;
						}
					}
					else
					{
						num4 = -1;
					}
				}
			}
			return false;
		}

		public virtual bool HasIllegalSet()
		{
			for (int i = 0; i < 8; i++)
			{
				int num = 0;
				int num2 = -1;
				bool flag = false;
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					if (piece != null)
					{
						bool flag2 = piece.mCreatedTick == mUpdateCnt && piece.IsFlagSet(16u);
						if (piece.mColor != -1 && piece.mColor == num2)
						{
							flag = flag || flag2;
							if (++num >= 3 && flag)
							{
								return true;
							}
						}
						else
						{
							num2 = piece.mColor;
							num = 1;
							flag = flag2;
						}
					}
					else
					{
						num2 = -1;
					}
				}
			}
			for (int j = 0; j < 8; j++)
			{
				int num3 = 0;
				int num4 = -1;
				bool flag3 = false;
				for (int i = 0; i < 8; i++)
				{
					Piece piece2 = mBoard[i, j];
					if (piece2 != null)
					{
						bool flag4 = piece2.mCreatedTick == mUpdateCnt && piece2.IsFlagSet(16u);
						if (piece2.mColor != -1 && piece2.mColor == num4)
						{
							flag3 = flag3 || flag4;
							if (++num3 >= 3 && flag3)
							{
								return true;
							}
						}
						else
						{
							num4 = piece2.mColor;
							num3 = 1;
							flag3 = flag4;
						}
					}
					else
					{
						num4 = -1;
					}
				}
			}
			return false;
		}

		public virtual bool TriggerSpecial(Piece thePiece)
		{
			return TriggerSpecial(thePiece, null);
		}

		public virtual bool TriggerSpecial(Piece thePiece, Piece theSrc)
		{
			if (thePiece.mDestructing)
			{
				return false;
			}
			if (thePiece.IsFlagSet(1u) && !thePiece.IsFlagSet(4u))
			{
				thePiece.mExplodeDelay = 1;
				thePiece.mExplodeSourceId = theSrc?.mId ?? (-1);
				thePiece.mExplodeSourceFlags |= theSrc?.mFlags ?? (-1);
				return true;
			}
			if (thePiece.IsFlagSet(524288u))
			{
				thePiece.mExplodeDelay = 1;
				thePiece.mExplodeSourceId = theSrc?.mId ?? (-1);
				thePiece.mExplodeSourceFlags |= theSrc?.mFlags ?? (-1);
				return true;
			}
			if (thePiece.IsFlagSet(2u) && FindStormIdxFor(thePiece) == -1)
			{
				int theColor = ((theSrc != null) ? ((theSrc.mColor != -1) ? theSrc.mColor : theSrc.mLastColor) : ((thePiece.mLastColor == -1) ? ((int)(mRand.Next() % 7)) : thePiece.mLastColor));
				DoHypercube(thePiece, theColor);
				return true;
			}
			if (thePiece.IsFlagSet(4u) && FindStormIdxFor(thePiece) == -1)
			{
				thePiece.mDestructing = true;
				if (thePiece.IsFlagSet(1u))
				{
					AddToStat(31, 1, thePiece.mMoveCreditId);
				}
				AddToStat(13, 1, thePiece.mMoveCreditId);
				LightningStorm lightningStorm = new LightningStorm(this, thePiece, 2);
				mLightningStorms.Add(lightningStorm);
				EncodeLightningStorm(lightningStorm);
				return true;
			}
			if (thePiece.IsFlagSet(16u))
			{
				thePiece.mDestructing = true;
				thePiece.mExplodeDelay = 1;
				thePiece.mExplodeSourceId = theSrc?.mId ?? (-1);
				thePiece.mExplodeSourceFlags |= theSrc?.mFlags ?? (-1);
				return true;
			}
			return false;
		}

		public int FindSets(bool fromUpdateSwapping, Piece thePiece1)
		{
			return FindSets(fromUpdateSwapping, thePiece1, null);
		}

		public int FindSets(bool fromUpdateSwapping)
		{
			return FindSets(fromUpdateSwapping, null, null);
		}

		public int FindSets()
		{
			return FindSets(false, null, null);
		}

		public virtual int FindSets(bool fromUpdateSwapping, Piece thePiece1, Piece thePiece2)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			FS_aBulgeTriggerPieceSet.Clear();
			FS_aDelayingPieceSet.Clear();
			FS_aTallyPieceSet.Clear();
			FS_aPowerupPieceSet.Clear();
			FS_aMatchedSets.Clear();
			bool flag2 = false;
			foreach (SwapData item in mSwapDataVector)
			{
				if (!item.mSwapPct.HasBeenTriggered() && item.mForceSwap)
				{
					flag2 = true;
				}
			}
			FS_aMoveCreditSet.Clear();
			FS_aDeferPowerupMap.Clear();
			FS_aDeferLaserSet.Clear();
			FS_aDeferExplodeVector.Clear();
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					bool flag3 = false;
					int num5 = 0;
					int num6 = -1;
					int num7 = 0;
					int num8 = 0;
					int num9 = 0;
					int num10 = 0;
					for (int k = 0; k < 8; k++)
					{
						int num11;
						int num12;
						if (i == 0)
						{
							num11 = j;
							num12 = k;
						}
						else
						{
							num11 = k;
							num12 = j;
						}
						int num13 = -1;
						bool flag4 = false;
						Piece piece = mBoard[num12, num11];
						bool flag5 = piece != null && (IsPieceStill(piece) | FS_aTallyPieceSet.ContainsKey(piece)) && !FS_aDelayingPieceSet.ContainsKey(piece);
						if (piece != null && (WillPieceBeStill(piece) || FS_aTallyPieceSet.ContainsKey(piece)))
						{
							num13 = ((piece.mChangedTick != mUpdateCnt) ? piece.mColor : piece.mLastColor);
							if (num13 == num6 && num13 != -1)
							{
								flag3 = flag3 || !flag5 || flag2;
								num10 = num11;
								num9 = num12;
								flag4 = true;
								num5++;
							}
						}
						if (flag4 && k != 7)
						{
							continue;
						}
						if (num5 >= 3)
						{
							int num14 = 0;
							bool flag6 = false;
							MatchSet matchSet = new MatchSet();
							bool flag7 = false;
							bool flag8 = false;
							int num15 = -1;
							int num16 = -1;
							matchSet.mMatchId = mCurMoveCreditId++;
							matchSet.mMoveCreditId = -1;
							matchSet.mExplosionCount = 0;
							bool flag9 = false;
							for (int l = num7; l <= num9; l++)
							{
								for (int m = num8; m <= num10; m++)
								{
									Piece piece2 = mBoard[l, m];
									if (piece2 == null)
									{
										continue;
									}
									flag9 |= FS_aBulgeTriggerPieceSet.ContainsKey(piece2);
									int num17 = 0;
									bool flag10 = false;
									int[,] array = new int[4, 2]
									{
										{ -1, 0 },
										{ 1, 0 },
										{ 0, -1 },
										{ 0, 1 }
									};
									for (int n = 0; n < 4; n++)
									{
										for (int num18 = 1; num18 < 8; num18++)
										{
											Piece pieceAtRowCol = GetPieceAtRowCol(piece2.mRow + array[n, 0] * num18, piece2.mCol + array[n, 1] * num18);
											if (pieceAtRowCol == null || pieceAtRowCol.mColor != piece2.mColor)
											{
												break;
											}
											bool flag11 = !IsPieceStill(pieceAtRowCol) && WillPieceBeStill(pieceAtRowCol);
											if (n / 2 != i)
											{
												num17++;
												flag10 = flag10 || flag11;
											}
											else
											{
												flag8 = flag8 || flag11;
											}
										}
									}
									flag8 = flag8 || (num17 >= 2 && flag10);
									if (piece2.mColor == num6)
									{
										flag6 |= piece2.mSwapTick == mUpdateCnt;
										matchSet.mPieces.Add(piece2);
									}
									num15 = Math.Max(num15, piece2.mMoveCreditId);
									num16 = Math.Max(num16, piece2.mLastMoveCreditId);
								}
							}
							if (num15 == -1)
							{
								num15 = num16;
							}
							matchSet.mMoveCreditId = num15;
							if (flag3 || flag8)
							{
								flag = true;
								for (int num19 = num7; num19 <= num9; num19++)
								{
									for (int num20 = num8; num20 <= num10; num20++)
									{
										Piece piece3 = mBoard[num19, num20];
										if (piece3 != null)
										{
											piece3.mMoveCreditId = num15;
											FS_aDelayingPieceSet[piece3] = 0;
										}
									}
								}
							}
							else
							{
								num++;
								if (!flag6)
								{
									num2++;
								}
								List<Piece> list = new List<Piece>();
								List<Piece> list2 = new List<Piece>();
								for (int num21 = num7; num21 <= num9; num21++)
								{
									for (int num22 = num8; num22 <= num10; num22++)
									{
										Piece piece4 = mBoard[num21, num22];
										if (piece4 != null && piece4.IsFlagSet(16u))
										{
											if (WriteUReplayCmd(13))
											{
												EncodePieceRef(piece4);
											}
											IncPointMult(piece4);
											piece4.ClearFlag(16u);
											mPostFXManager.FreePieceEffect(piece4.mId);
										}
									}
								}
								for (int num23 = num7; num23 <= num9; num23++)
								{
									for (int num24 = num8; num24 <= num10; num24++)
									{
										Piece piece5 = mBoard[num23, num24];
										if (piece5 == null)
										{
											continue;
										}
										piece5.mMatchId = matchSet.mMatchId;
										piece5.mMoveCreditId = num15;
										if (piece5.IsFlagSet(1024u))
										{
											TallyCoin(piece5);
											piece5.ClearFlag(1024u);
										}
										if (!flag7)
										{
											flag7 = ComboProcess(piece5.mColor);
										}
										bool flag12 = false;
										if (piece5.IsFlagSet(524289u))
										{
											FS_aDeferExplodeVector.Add(piece5);
											matchSet.mExplosionCount++;
										}
										if (piece5.IsFlagSet(4u) && piece5.mChangedTick != mUpdateCnt)
										{
											int num25 = FindStormIdxFor(piece5);
											if (num25 == -1)
											{
												AddToStat(13, 1, num15);
												LightningStorm lightningStorm = new LightningStorm(this, piece5, (mFullLaser != 0) ? 2 : 3);
												mLightningStorms.Add(lightningStorm);
												EncodeLightningStorm(lightningStorm);
											}
										}
										else if (piece5.mChangedTick == mUpdateCnt && piece5.mColor > -1 && piece5.mColor < 7)
										{
											if (piece5.mFlags == 0 || piece5.IsFlagSet(1u) || piece5.IsFlagSet(128u) || piece5.IsFlagSet(131072u))
											{
												if (piece5.IsFlagSet(128u))
												{
													mForceReleaseButterfly = true;
													mForcedReleasedBflyPiece = piece5;
												}
												if (AllowPowerups())
												{
													if (WantsTutorial(2))
													{
														DeferTutorialDialog(2, piece5);
														flag9 = true;
													}
													else if (!flag9)
													{
														piece5.mScale.SetConstant(1.0);
														piece5.mChangedTick = mUpdateCnt;
														FS_aPowerupPieceSet[piece5] = 0;
														piece5.mMoveCreditId = num15;
														FS_aDeferLaserSet[piece5] = 0;
													}
												}
												flag12 = false;
											}
										}
										else if (piece5.mChangedTick != mUpdateCnt)
										{
											flag12 = piece5.mFlags == 0;
											if (flag6)
											{
												GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_A, piece5.mScale);
											}
											else if (WantBulgeCascades())
											{
												GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_B, piece5.mScale);
											}
											else
											{
												GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_C, piece5.mScale);
											}
											if (!SexyFramework.GlobalMembers.gIs3D)
											{
												piece5.mRotPct = 0f;
											}
											piece5.mScale.mIncRate *= GetMatchSpeed();
											piece5.mChangedTick = mUpdateCnt;
											piece5.mLastColor = piece5.mColor;
											FS_aTallyPieceSet[piece5] = 0;
										}
										if (flag12)
										{
											if (piece5.mLastActiveTick > num14)
											{
												num14 = piece5.mLastActiveTick;
												list2.Clear();
											}
											if (piece5.mLastActiveTick == num14)
											{
												list2.Add(piece5);
											}
											list.Add(piece5);
										}
									}
								}
								if (list.Count > 0)
								{
									Piece piece6 = list2[list2.Count / 2];
									for (int num26 = 0; num26 < list.Count; num26++)
									{
										Piece piece7 = list[num26];
										Piece pieceAtRowCol2;
										Piece pieceAtRowCol3;
										if (i == 0)
										{
											pieceAtRowCol2 = GetPieceAtRowCol(piece7.mRow, piece7.mCol + 1);
											pieceAtRowCol3 = GetPieceAtRowCol(piece7.mRow, piece7.mCol - 1);
										}
										else
										{
											pieceAtRowCol2 = GetPieceAtRowCol(piece7.mRow + 1, piece7.mCol);
											pieceAtRowCol3 = GetPieceAtRowCol(piece7.mRow - 1, piece7.mCol);
										}
										if (pieceAtRowCol2 != null && pieceAtRowCol2.mColor == piece7.mColor && pieceAtRowCol3 != null && pieceAtRowCol3.mColor == piece7.mColor)
										{
											piece6 = piece7;
										}
									}
									int num27 = Math.Max(Math.Abs(num8 - piece6.mCol), Math.Abs(num7 - piece6.mRow));
									int num28 = Math.Max(Math.Abs(num10 - piece6.mCol), Math.Abs(num9 - piece6.mRow));
									Piece piece8 = null;
									for (int num29 = 1; num29 <= 2; num29++)
									{
										piece8 = ((num27 > num28) ? ((i != 0) ? GetPieceAtRowCol(piece6.mRow, Math.Max(num8, piece6.mCol - num29)) : GetPieceAtRowCol(Math.Max(num7, piece6.mRow - num29), piece6.mCol)) : ((i != 0) ? GetPieceAtRowCol(piece6.mRow, Math.Min(num10, piece6.mCol + num29)) : GetPieceAtRowCol(Math.Min(num9, piece6.mRow + num29), piece6.mCol)));
										if (piece8 != null && piece8.mFlags == 0)
										{
											break;
										}
									}
									if (AllowPowerups())
									{
										if (num5 >= 6 && piece6.CanSetFlag(4u) && piece6.CanSetFlag(1u))
										{
											if (WantsTutorial(6))
											{
												DeferTutorialDialog(6, piece6);
												flag9 = true;
											}
											else if (!flag9)
											{
												piece6.mMoveCreditId = num15;
												if (piece8 != null)
												{
													piece8.mMoveCreditId = num15;
												}
												FS_aDeferPowerupMap[piece6] = new Pair<int, Piece>(num5, piece8);
											}
										}
										else if (num5 >= 5 && !piece6.IsFlagSet(4u) && !FS_aBulgeTriggerPieceSet.ContainsKey(piece6))
										{
											if (WantsTutorial(3))
											{
												DeferTutorialDialog(3, piece6);
												flag9 = true;
											}
											else if (!flag9)
											{
												piece6.mMoveCreditId = num15;
												if (piece8 != null)
												{
													piece8.mMoveCreditId = num15;
												}
												FS_aDeferPowerupMap[piece6] = new Pair<int, Piece>(num5, piece8);
											}
										}
										else if (num5 >= 4)
										{
											if (WantsTutorial(1))
											{
												DeferTutorialDialog(1, piece6);
												flag9 = true;
											}
											else if (!flag9)
											{
												piece6.mMoveCreditId = num15;
												if (piece8 != null)
												{
													piece8.mMoveCreditId = num15;
												}
												FS_aDeferPowerupMap[piece6] = new Pair<int, Piece>(num5, piece8);
											}
										}
									}
									if (CreateMatchPowerup(num5, piece6, FS_aTallyPieceSet))
									{
										piece6.mScale.SetConstant(1.0);
									}
								}
								if (flag9)
								{
									for (int num30 = num7; num30 <= num9; num30++)
									{
										for (int num31 = num8; num31 <= num10; num31++)
										{
											Piece piece9 = mBoard[num30, num31];
											if (piece9 != null)
											{
												FS_aBulgeTriggerPieceSet[piece9] = 0;
												if (FS_aDeferLaserSet.ContainsKey(piece9))
												{
													FS_aDeferLaserSet.Remove(piece9);
												}
											}
										}
									}
								}
								FS_aMatchedSets.Add(matchSet);
							}
						}
						num6 = num13;
						num5 = 1;
						flag3 = !flag5;
						num8 = num11;
						num7 = num12;
						num10 = num11;
						num9 = num12;
					}
				}
			}
			for (int num32 = 0; num32 < FS_aDeferExplodeVector.Count; num32++)
			{
				Piece piece10 = FS_aDeferExplodeVector[num32];
				piece10.mExplodeDelay = 1;
				piece10.mExplodeSourceId = piece10.mId;
				piece10.mExplodeSourceFlags |= piece10.mFlags;
				piece10.mScale.SetConstant(1.0);
			}
			foreach (Piece key in FS_aDeferLaserSet.Keys)
			{
				if (key.IsFlagSet(524289u))
				{
					key.mExplodeDelay = 1;
					key.mExplodeSourceId = key.mId;
					key.mExplodeSourceFlags = key.mFlags;
					key.mFlags = 0;
				}
				if (AllowLaserGems())
				{
					AddToStat(4, 1, key.mMoveCreditId);
					AddToStat(18, 1, key.mMoveCreditId);
					Laserify(key);
				}
				else
				{
					AddToStat(4, 1, key.mMoveCreditId);
					AddToStat(17, 1, key.mMoveCreditId);
					Flamify(key);
				}
			}
			Dictionary<Piece, Pair<int, Piece>>.Enumerator enumerator3 = FS_aDeferPowerupMap.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				Piece piece11 = enumerator3.Current.Key;
				if (piece11.IsFlagSet(4u))
				{
					piece11 = enumerator3.Current.Value.second;
				}
				if (piece11.mFlags == 0 && !FS_aBulgeTriggerPieceSet.ContainsKey(piece11))
				{
					FS_aPowerupPieceSet[piece11] = 0;
					if (enumerator3.Current.Value.first > 5)
					{
						AddToStat(4, 1, piece11.mMoveCreditId);
						AddToStat(17, 1, piece11.mMoveCreditId);
						AddToStat(18, 1, piece11.mMoveCreditId);
						AddToStat(30, 1, piece11.mMoveCreditId);
						Laserify(piece11);
						Flamify(piece11);
						piece11.mScale.SetConstant(1.0);
					}
					else if (enumerator3.Current.Value.first > 4)
					{
						AddToStat(4, 1, piece11.mMoveCreditId);
						AddToStat(19, 1, piece11.mMoveCreditId);
						Hypercubeify(piece11);
						piece11.mScale.SetConstant(1.0);
					}
					else
					{
						Flamify(piece11);
						AddToStat(4, 1, piece11.mMoveCreditId);
						AddToStat(17, 1, piece11.mMoveCreditId);
					}
					piece11.mChangedTick = mUpdateCnt;
					if (piece11.mColor != -1)
					{
						piece11.mLastColor = piece11.mColor;
					}
					piece11.mScale.SetConstant(1.0);
				}
			}
			if (FS_aMatchedSets.Count > 0)
			{
				ProcessMatches(FS_aMatchedSets, FS_aTallyPieceSet, fromUpdateSwapping);
			}
			bool flag13 = false;
			for (int num33 = 0; num33 < FS_aMatchedSets.Count; num33++)
			{
				MatchSet matchSet2 = FS_aMatchedSets[num33];
				Piece piece12 = null;
				bool flag14 = false;
				bool flag15 = false;
				int num34 = 0;
				int num35 = 0;
				bool flag16 = false;
				for (int num36 = 0; num36 < matchSet2.mPieces.Count; num36++)
				{
					Piece piece13 = matchSet2.mPieces[num36];
					flag16 |= piece13.mSwapTick == mUpdateCnt;
					num34 += (int)piece13.GetScreenX();
					num35 += (int)piece13.GetScreenY();
					if (FS_aPowerupPieceSet.ContainsKey(piece13))
					{
						piece12 = piece13;
					}
					if (piece13.IsFlagSet(4u) && piece13.mChangedTick != mUpdateCnt)
					{
						flag14 = true;
					}
					if (FS_aBulgeTriggerPieceSet.ContainsKey(piece13))
					{
						flag15 = true;
					}
				}
				if (WriteUReplayCmd(2))
				{
					int num37 = 0;
					if (fromUpdateSwapping)
					{
						num37 |= 1;
					}
					if (flag16)
					{
						num37 |= 2;
					}
					if (flag15)
					{
						num37 |= 4;
					}
					if (flag14)
					{
						num37 |= 8;
					}
					mUReplayBuffer.WriteByte((byte)num37);
					if (!flag16)
					{
						mUReplayBuffer.WriteByte((byte)GetMaxMovesStat(27));
					}
					mUReplayBuffer.WriteByte((byte)matchSet2.mPieces.Count);
					for (int num38 = 0; num38 < matchSet2.mPieces.Count; num38++)
					{
						Piece piece14 = matchSet2.mPieces[num38];
						EncodePieceRef(piece14);
						if (piece14.IsFlagSet(7u) && !piece14.mDestructing && !FS_aTallyPieceSet.ContainsKey(piece14))
						{
							mUReplayBuffer.WriteByte(EncodePieceFlags(piece14.mFlags));
						}
						else
						{
							mUReplayBuffer.WriteByte(0);
						}
					}
				}
				if (fromUpdateSwapping && mSpeedBonusFlameModePct > 0f)
				{
					int num39 = 0;
					Piece piece15 = null;
					for (int num40 = 0; num40 < matchSet2.mPieces.Count; num40++)
					{
						Piece piece16 = matchSet2.mPieces[num40];
						if (piece16.mSwapTick > num39)
						{
							piece15 = piece16;
							num39 = piece16.mSwapTick;
						}
					}
					if (piece15 != null)
					{
						piece15.SetFlag(32768u);
						AddToStat(16, 1);
						piece15.mExplodeDelay = 1;
						piece15.mExplodeSourceId = piece15.mId;
						piece15.mExplodeSourceFlags |= piece15.mFlags;
					}
				}
				if (flag15)
				{
					for (int num41 = 0; num41 < matchSet2.mPieces.Count; num41++)
					{
						Piece piece17 = matchSet2.mPieces[num41];
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_D, piece17.mScale);
						piece17.mIsBulging = true;
						piece17.mExplodeDelay = 0;
						piece17.mExplodeSourceId = piece17.mId;
						piece17.mExplodeSourceFlags |= piece17.mFlags;
						int num42 = FindStormIdxFor(piece17);
						if (num42 != -1)
						{
							if (mLightningStorms[num42] != null)
							{
								mLightningStorms[num42].Dispose();
							}
							mLightningStorms.RemoveAt(num42);
						}
						FS_aPowerupPieceSet[piece17] = 0;
					}
					continue;
				}
				flag13 = true;
				if (flag14)
				{
					for (int num43 = 0; num43 < matchSet2.mPieces.Count; num43++)
					{
						Piece piece18 = matchSet2.mPieces[num43];
						piece18.mScale.SetConstant(1.0);
						piece18.mCanMatch = false;
					}
				}
				else if (piece12 != null)
				{
					for (int num44 = 0; num44 < matchSet2.mPieces.Count; num44++)
					{
						Piece piece19 = matchSet2.mPieces[num44];
						if (piece19 == piece12 || (piece19.mFlags != 0 && !piece19.IsFlagSet(128u) && !piece19.IsFlagSet(131072u)))
						{
							continue;
						}
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_E, piece19.mScale);
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_DEST_PCT_B, piece19.mDestPct);
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ALPHA, piece19.mAlpha);
						piece19.mDestPct.mIncRate *= GetMatchSpeed();
						piece19.mScale.mIncRate *= GetMatchSpeed();
						piece19.mAlpha.mIncRate *= GetMatchSpeed();
						piece19.mDestCol = piece12.mCol;
						piece19.mDestRow = piece12.mRow;
						int num45 = piece12.mCol - piece19.mCol;
						int num46 = piece12.mRow - piece19.mRow;
						if (!piece12.IsFlagSet(1u))
						{
							continue;
						}
						PopAnimEffect popAnimEffect = PopAnimEffect.fromPopAnim(GlobalMembersResourcesWP.POPANIM_FLAMEGEMCREATION);
						popAnimEffect.mPieceRel = piece19;
						popAnimEffect.mX = piece19.CX();
						popAnimEffect.mY = piece19.CY();
						popAnimEffect.mOverlay = true;
						popAnimEffect.mDoubleSpeed = true;
						if (num45 != 0)
						{
							popAnimEffect.Play("smear horizontal");
							if (num45 < 0)
							{
								popAnimEffect.mAngle = (float)Math.PI;
							}
						}
						else
						{
							popAnimEffect.Play("smear vertical");
							if (num46 < 0)
							{
								popAnimEffect.mAngle = (float)Math.PI;
							}
						}
						mPostFXManager.AddEffect(popAnimEffect);
					}
				}
				int count = matchSet2.mPieces.Count;
				int num47 = num34 / count;
				int num48 = num35 / count;
				num3 += num47;
				num4 += num48;
				int num49 = 0;
				if (flag16)
				{
					mNOfIntentionalMatchesDuringCascade++;
				}
				MaxStat(39, mNOfIntentionalMatchesDuringCascade);
				AddToStat(26, 1, matchSet2.mMoveCreditId);
				int num50 = Math.Max(1, GetMoveStat(matchSet2.mMoveCreditId, 26));
				num49 = 50 * num50 + (count - 3) * 50;
				if (count >= 5)
				{
					num49 += (count - 4) * 350;
				}
				AddPoints(num47 + 50, num48 + 50 - 8, num49, GlobalMembers.gPointColors[matchSet2.mPieces[0].mColor], (uint)matchSet2.mMatchId, true, true, matchSet2.mMoveCreditId, false, 0);
				MaxStat(24, count, matchSet2.mMoveCreditId);
				FS_aMoveCreditSet[matchSet2.mMoveCreditId] = 0;
			}
			if (flag13)
			{
				int panPosition = GetPanPosition(num3 / num + 50);
				if (num > 1)
				{
					if (WantsCalmEffects())
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DOUBLESET, panPosition, GlobalMembers.M(0.6), GlobalMembers.M(0.0));
					}
					else
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DOUBLESET, panPosition);
					}
				}
				int num51 = GetMaxMovesStat(27) + 1;
				if (num2 == 0)
				{
					num51 = 1;
				}
				if (num51 > 6)
				{
					num51 = 6;
				}
				if (fromUpdateSwapping && mSpeedBonusCount > 0)
				{
					if (mSpeedBonusNum > 0.01)
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_FLAMESPEED1, panPosition, 1.0, mSpeedBonusNum * (double)GlobalMembers.M(1f));
					}
					else
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.GetSoundById(1721 + Math.Min(8, mSpeedBonusCount)), panPosition, 1.0, mSpeedBonusNum * (double)GlobalMembers.M(1f));
					}
				}
				else if (num51 == 1 && WantsCalmEffects())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ZEN_COMBO_2, panPosition);
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COMBO_1 + num51, panPosition);
				}
				foreach (int key2 in FS_aMoveCreditSet.Keys)
				{
					AddToStat(27, 1, key2);
				}
			}
			foreach (Piece key3 in FS_aTallyPieceSet.Keys)
			{
				if (!key3.mIsBulging)
				{
					TallyPiece(key3, !FS_aPowerupPieceSet.ContainsKey(key3));
				}
			}
			if (flag && fromUpdateSwapping && (FS_aDelayingPieceSet.ContainsKey(thePiece1) || FS_aDelayingPieceSet.ContainsKey(thePiece2)))
			{
				return 2;
			}
			if (num <= 0)
			{
				return 0;
			}
			return 1;
		}

		public virtual void ShowHint(bool fromButton)
		{
			if (mInReplay)
			{
				return;
			}
			mHintCooldownTicks = GlobalMembers.M(300);
			mWantHintTicks = 0;
			if (!fromButton && !mShowAutohints)
			{
				return;
			}
			int[] array = new int[4];
			if (FindRandomMove(array))
			{
				Piece thePiece = mBoard[array[3], array[2]];
				Piece piece = mBoard[array[1], array[0]];
				if (piece != null && piece.IsFlagSet(2u))
				{
					thePiece = piece;
				}
				if (WriteUReplayCmd(16))
				{
					EncodePieceRef(thePiece);
				}
				ShowHint(thePiece, fromButton);
			}
		}

		public void ShowHint(Piece thePiece, bool theShowArrow)
		{
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_HINT_ALPHA, thePiece.mHintAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_HINT_ARROW_POS, thePiece.mHintArrowPos);
			if (WantsCalmEffects())
			{
				thePiece.mHintAlpha.mIncRate *= GlobalMembers.M(0.5f);
				thePiece.mHintArrowPos.mIncRate *= GlobalMembers.M(0.5f);
			}
			if (theShowArrow)
			{
				ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_HINTFLASH);
				particleEffect.mPieceRel = thePiece;
				particleEffect.mDoubleSpeed = true;
				mPostFXManager.AddEffect(particleEffect);
			}
		}

		public virtual void FillInBlanks(bool allowCascades)
		{
			FillInBlanks(allowCascades, false);
		}

		public virtual void FillInBlanks()
		{
			FillInBlanks(true, false);
		}

		public virtual void FillInBlanks(bool allowCascades, bool creatingNewBoard)
		{
			if (mGameOverPiece != null)
			{
				return;
			}
			for (int num = mBottomFillRow; num >= 0; num--)
			{
				for (int i = 0; i < 8; i++)
				{
					Piece piece = mBoard[num, i];
					if (piece != null && piece.mExplodeDelay > 0)
					{
						return;
					}
				}
			}
			List<Piece> list = new List<Piece>();
			bool flag = false;
			bool flag2;
			do
			{
				flag2 = false;
				for (int j = 0; j < 8; j++)
				{
					bool flag3 = false;
					int num2 = -GetBoardY();
					int mMoveCreditId = mNextColumnCredit[j];
					double num3 = mBumpVelocities[j];
					int num4 = 0;
					int num5 = 0;
					for (int num6 = mBottomFillRow; num6 >= 0; num6--)
					{
						Piece piece2 = mBoard[num6, j];
						if (piece2 != null)
						{
							piece2.mCanMatch = true;
							if (piece2.mY < (float)num2)
							{
								num2 = (int)piece2.mY;
							}
							if (flag3 && (piece2.mDestRow == -1 || piece2.mDestPct.IsDoingCurve() || IsPieceSwapping(piece2) || IsPieceMatching(piece2)))
							{
								if (num5 > 0 && WriteUReplayCmd(5))
								{
									mUReplayBuffer.WriteByte((byte)j);
									mUReplayBuffer.WriteByte((byte)num4);
									mUReplayBuffer.WriteByte((byte)num5);
								}
								num5 = 0;
								mBoard[num6 + 1, j] = null;
								flag3 = false;
							}
							if (flag3)
							{
								flag2 = true;
								if ((double)piece2.mFallVelocity == 0.0)
								{
									piece2.mFallVelocity += mBumpVelocities[j] + 1f;
									piece2.mLastActiveTick = mUpdateCnt;
								}
								num5++;
								num3 = piece2.mFallVelocity;
								piece2.mRow++;
								mBoard[num6, j] = null;
								mBoard[num6 + 1, j] = piece2;
								mMoveCreditId = piece2.mMoveCreditId;
							}
						}
						else if (flag3)
						{
							mBoard[num6 + 1, j] = null;
							num5++;
						}
						else
						{
							num4 = num6;
							num5 = 0;
							flag3 = true;
						}
					}
					if (num5 > 0 && WriteUReplayCmd(5))
					{
						mUReplayBuffer.WriteByte((byte)j);
						mUReplayBuffer.WriteByte((byte)num4);
						mUReplayBuffer.WriteByte((byte)num5);
					}
					if (flag3)
					{
						flag2 = true;
						Piece piece3 = CreateNewPiece(0, j);
						if (creatingNewBoard)
						{
							piece3.mY = num2 - 100;
						}
						else
						{
							piece3.mFallVelocity = (float)(num3 - 0.550000011920929);
							piece3.mMoveCreditId = mMoveCreditId;
							piece3.mY = (float)(num2 - 100) - (float)(mRand.Next() % GlobalMembers.M(15)) - GlobalMembers.M(10f);
							if (piece3.GetScreenY() > -100f)
							{
								piece3.mY = -100 - GetBoardY();
							}
						}
						list.Add(piece3);
						int num7 = 0;
						for (int num8 = mBottomFillRow; num8 >= 0; num8--)
						{
							Piece piece4 = mBoard[num8, j];
							if (piece4 != null)
							{
								float num9 = piece4.GetScreenY();
								if (piece4.GetScreenY() < (float)(-GetBoardY()))
								{
									num7++;
									num9 = -num7 * 100 * 2 - GetBoardY();
								}
								double num10 = (float)GetRowY(piece4.mRow) - num9;
								double num11 = Math.Sqrt(2.0 * num10 / (GlobalMembers.M(0.265) * (double)GetGravityFactor()));
								int val = (int)num11;
								mSettlingDelay = Math.Max(mSettlingDelay, val);
							}
						}
					}
					mNextColumnCredit[j] = -1;
				}
				flag = flag || flag2;
			}
			while (flag2);
			if (list.Count <= 0)
			{
				return;
			}
			int num12 = 0;
			bool flag4 = AllowNoMoreMoves();
			bool flag5 = WantSpecialPiece(list);
			bool flag6 = false;
			bool flag7 = false;
			List<int> newGemColors = GetNewGemColors();
			while (true)
			{
				flag6 = false;
				for (int k = 0; k < list.Count; k++)
				{
					list[k].ClearFlags();
					list[k].mColor = -1;
					list[k].mCanDestroy = true;
					list[k].mCanMatch = true;
					list[k].mCounter = 0;
				}
				if (flag5)
				{
					flag6 = DropSpecialPiece(list);
				}
				if (WantHyperMixers())
				{
					bool flag8 = true;
					Piece[,] array = mBoard;
					int upperBound = array.GetUpperBound(0);
					int upperBound2 = array.GetUpperBound(1);
					for (int l = array.GetLowerBound(0); l <= upperBound; l++)
					{
						int num13 = array.GetLowerBound(1);
						while (num13 <= upperBound2)
						{
							Piece thePiece = array[l, num13];
							if (!IsHypermixerCancelledBy(thePiece))
							{
								num13++;
								continue;
							}
							goto IL_04b2;
						}
						continue;
						IL_04b2:
						flag8 = false;
						break;
					}
					if (flag8)
					{
						int[] array2 = new int[4];
						bool flag9 = false;
						if (WantHypermixerBottomCheck() && FindMove(array2, 0, true, true, true) && array2[1] < mHypermixerCheckRow && array2[3] < mHypermixerCheckRow)
						{
							flag9 = true;
						}
						if (!flag9 && WantHypermixerEdgeCheck())
						{
							int num14 = 0;
							bool flag10 = false;
							bool flag11 = false;
							while ((!flag10 || !flag11) && FindMove(array2, num14++, true, true, false))
							{
								if (array2[0] <= GlobalMembers.M(3) || array2[2] <= GlobalMembers.M(3))
								{
									flag10 = true;
								}
								if (array2[0] >= 8 - GlobalMembers.M(4) || array2[2] >= 8 - GlobalMembers.M(4))
								{
									flag11 = true;
								}
							}
							if (!flag10 || !flag11)
							{
								flag9 = true;
							}
						}
						if (flag9)
						{
							Piece piece5;
							do
							{
								piece5 = list[BejeweledLivePlus.Misc.Common.Rand(list.Count)];
							}
							while (piece5.mFlags != 0);
							piece5.mColor = -1;
							piece5.SetFlag(2u);
							flag7 = true;
							HypermixerDropped();
						}
					}
				}
				for (int m = 0; m < list.Count; m++)
				{
					Piece piece6 = list[m];
					if (piece6.mFlags == 0 || piece6.IsFlagSet(96u))
					{
						piece6.mColor = newGemColors[(int)mRand.Next((ulong)newGemColors.Count)];
					}
					int num15 = (mHasBoardSettled ? GlobalMembers.M(200) : GlobalMembers.M(200000));
					if (num12 >= num15 && m == 0)
					{
						allowCascades = true;
						flag4 = true;
					}
				}
				if (TryingDroppedPieces(list, num12))
				{
					flag4 |= FindMove(null, 0, true, true, false, null, num12 < GetPowerGemThreshold());
				}
				if (!allowCascades)
				{
					flag4 &= !HasSet();
				}
				flag4 &= !HasIllegalSet();
				if (flag4)
				{
					flag4 &= PiecesDropped(list);
				}
				if (flag4)
				{
					break;
				}
				num12++;
			}
			if (list.Count == 64 && mGameTicks > 0 && WantAnnihilatorReplacement())
			{
				for (int n = 0; n < 2; n++)
				{
					for (int num16 = 0; num16 < 64; num16++)
					{
						Piece piece7 = list[(int)(mRand.Next() % list.Count)];
						if (piece7.mFlags == 0)
						{
							piece7.mColor = -1;
							piece7.SetFlag(2u);
							break;
						}
					}
				}
			}
			if (flag7)
			{
				NewHyperMixer();
			}
			BlanksFilled(flag6);
			for (int num17 = 0; num17 < list.Count; num17++)
			{
				Piece piece8 = list[num17];
				if (WriteUReplayCmd(0))
				{
					EncodePieceRef(piece8);
					mUReplayBuffer.WriteByte(EncodePieceFlags(piece8.mFlags));
					mUReplayBuffer.WriteByte((byte)piece8.mColor);
					mUReplayBuffer.WriteShort((short)((piece8.GetScreenY() - (float)GetRowY(0)) / 100f * 256f));
					if (piece8.mFallVelocity < 0f && WriteUReplayCmd(12))
					{
						EncodePieceRef(piece8);
						mUReplayBuffer.WriteShort((short)(piece8.mFallVelocity / 100f * 256f * 100f));
					}
				}
				StartPieceEffect(piece8);
			}
		}

		public virtual void BlanksFilled(bool specialDropped)
		{
			if (specialDropped)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_MULTIPLIER_APPEARS);
			}
		}

		public void MatchMade(SwapData theSwapData)
		{
			int num = mIdleTicks - mLastMatchTick;
			mMoveCounter++;
			if (theSwapData != null && !theSwapData.mForceSwap)
			{
				DecrementAllCounterGems(false);
			}
			mWantHintTicks = 0;
			if (!AllowSpeedBonus())
			{
				return;
			}
			mMatchTallyCount++;
			if (mSpeedBonusCount >= 9 && mSpeedBonusFlameModePct == 0f && (GetTimeLimit() == 0 || GetTicksLeft() >= 5))
			{
				float num2 = (float)num - GlobalMembers.M(100f);
				float num4;
				if (num2 >= 0f)
				{
					float num3 = GlobalMembers.M(100f) - GlobalMembers.M(180f);
					num4 = Math.Max(0f, Math.Min(1.5f, 1f - num2 / num3));
				}
				else
				{
					num4 = 1.5f;
				}
				float num5 = (float)((double)num4 - mSpeedBonusNum);
				if (num5 > 0f)
				{
					mSpeedBonusNum = Math.Min(1.0, mSpeedBonusNum + (double)Math.Min(0.1f, num5 * GetSpeedBonusRamp()));
					if (mSpeedBonusNum >= 1.0 && mSpeedBonusFlameModePct == 0f)
					{
						mSpeedBonusNum = 1.0;
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_DISP_ON, mSpeedBonusDisp);
						DoSpeedText(0);
					}
				}
			}
			if (mSpeedBonusCount > 0 || (mLastMatchTime >= 0 && num + mLastMatchTime <= 300))
			{
				if (mSpeedBonusCount == 0)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_DISP_ON, mSpeedBonusDisp);
				}
				mSpeedBonusCount++;
				mSpeedBonusCountHighest = Math.Max(mSpeedBonusCountHighest, mSpeedBonusCount);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_POINTS_GLOW, mSpeedBonusPointsGlow);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_POINTS_SCALE_ON, mSpeedBonusPointsScale);
				List<Points>.Enumerator enumerator = mPointsManager.mPointsList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Points current = enumerator.Current;
					if (current.mUpdateCnt == 0)
					{
						int num6 = Math.Min(200, (mSpeedBonusCount + 1) * 20);
						mSpeedBonusPoints += (int)((float)(num6 * mPointMultiplier) * GetModePointMultiplier());
						AddPoints((int)current.mX, (int)current.mY, num6, current.mColor, current.mId, false, true, current.mMoveCreditId, false, 2);
						current.mUpdateCnt++;
						break;
					}
				}
			}
			mLastMatchTime = num;
			mLastMatchTick = mIdleTicks;
			EncodeSpeedBonus();
		}

		public virtual bool CreateMatchPowerup(int theMatchCount, Piece thePiece, Dictionary<Piece, int> thePieceSet)
		{
			return false;
		}

		public void UpdateSpeedBonus()
		{
			if (GlobalMembers.gApp.GetDialog(18) != null || mHyperspace != null || mLightningStorms.Count != 0)
			{
				mLastMatchTick = mIdleTicks;
			}
			int num = mIdleTicks - mLastMatchTick;
			float num2 = 1f;
			if (mMoveCounter == 0)
			{
				num2 = 2f;
			}
			else if (mMoveCounter == 1)
			{
				num2 = 1.5f;
			}
			float num3 = (float)((double)GlobalMembers.M(180f) + mSpeedBonusNum * (double)(GlobalMembers.M(100f) - GlobalMembers.M(180f)));
			num3 *= num2;
			if ((float)num >= num3 && mSpeedBonusNum > 0.0 && mSpeedBonusFlameModePct == 0f)
			{
				mSpeedBonusNum *= GlobalMembers.M(0.993f);
				if (mUpdateCnt % 10 == 0)
				{
					EncodeSpeedBonus();
				}
				if (mSpeedBonusNum <= 0.005)
				{
					mSpeedBonusNum = 0.0;
					EncodeSpeedBonus();
				}
			}
			mSpeedModeFactor.SetConstant((1.0 + mSpeedBonusNum * (double)GlobalMembers.M(0.65f)) * (double)GetSpeedModeFactorScale());
			mSpeedNeedle += (float)(((0.5 - mSpeedBonusNum) * GlobalMembers.MS(132.0) - (double)mSpeedNeedle) * (double)GlobalMembers.M(0.1f));
			float num4 = (GlobalMembers.M(100f) + (float)Math.Min(10, mSpeedBonusCount + 1) * GlobalMembers.M(13.75f)) * num2;
			if ((float)num >= num4 && mSpeedBonusCount > 0)
			{
				EndSpeedBonus();
			}
		}

		public virtual void EndSpeedBonus()
		{
			mLastMatchTick = -1000;
			mLastMatchTime = 1000;
			mSpeedBonusLastCount = mSpeedBonusCount;
			mSpeedBonusCount = 0;
			mSpeedBonusNum = 0.0;
			EncodeSpeedBonus();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_DISP_OFF, mSpeedBonusDisp);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_POINTS_SCALE_OFF_NORMAL, mSpeedBonusPointsScale);
		}

		public virtual bool AllowUI()
		{
			if (mLevelCompleteCount <= GlobalMembers.M(0))
			{
				return mGameOverCount <= GlobalMembers.M(0);
			}
			return false;
		}

		public void DoGemCountPopup(int theCount)
		{
			mGemCountValueDisp = theCount;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_COUNT_CURVE, mGemCountCurve);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_COUNT_ALPHA, mGemCountAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_SCALAR_ALPHA, mGemScalarAlpha);
		}

		public void DoCascadePopup(int theCount)
		{
			mCascadeCountValueDisp = theCount;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_CASCADE_COUNT_CURVE, mCascadeCountCurve);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_CASCADE_COUNT_ALPHA, mCascadeCountAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_SCALAR_ALPHA, mGemScalarAlpha);
		}

		public virtual void UpdateCountPopups()
		{
			int totalMovesStat = GetTotalMovesStat(4);
			if (totalMovesStat >= GetGemCountPopupThreshold() && totalMovesStat > mGemCountValueCheck)
			{
				DoGemCountPopup(totalMovesStat);
				if (WriteUReplayCmd(17))
				{
					mUReplayBuffer.WriteByte((byte)Math.Min(255, totalMovesStat));
				}
			}
			if (totalMovesStat == 0 || totalMovesStat > mGemCountValueCheck)
			{
				mGemCountValueCheck = totalMovesStat;
			}
			else if (totalMovesStat < mGemCountValueCheck - 4)
			{
				mGemCountValueCheck = totalMovesStat + 4;
			}
			int maxMovesStat = GetMaxMovesStat(27);
			if (maxMovesStat >= GlobalMembers.M(3) && maxMovesStat > mCascadeCountValueCheck)
			{
				DoCascadePopup(maxMovesStat);
				if (WriteUReplayCmd(18))
				{
					mUReplayBuffer.WriteByte((byte)maxMovesStat);
				}
			}
			mCascadeCountValueCheck = maxMovesStat;
		}

		public virtual int CalcAwesomeness(int theMoveCreditId)
		{
			int num = (int)Math.Max(0.0, Math.Pow(Math.Max(0, GetMoveStat(theMoveCreditId, 27) - 1), GlobalMembers.M(1.5f)));
			int moveStat = GetMoveStat(theMoveCreditId, 12);
			num += Math.Max(0, moveStat * 2 - 1);
			moveStat = GetMoveStat(theMoveCreditId, 13);
			num += Math.Max(0, (int)((double)moveStat * 2.5) - 1);
			moveStat = GetMoveStat(theMoveCreditId, 14);
			num += Math.Max(0, moveStat * 3 - 1);
			moveStat = GetMoveStat(theMoveCreditId, 17);
			num += moveStat;
			moveStat = GetMoveStat(theMoveCreditId, 18);
			num += moveStat;
			moveStat = GetMoveStat(theMoveCreditId, 19);
			num += moveStat * 2;
			moveStat = GetMoveStat(theMoveCreditId, 24);
			num += Math.Max(0, (moveStat - 5) * 8);
			return num + (int)Math.Pow((double)GetMoveStat(theMoveCreditId, 4) / GlobalMembers.M(15.0), GlobalMembers.M(1.5));
		}

		public void UpdateComplements()
		{
			if (!WantPointComplements())
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < mMoveDataVector.Count; i++)
			{
				num += CalcAwesomeness(mMoveDataVector[i].mMoveCreditId);
			}
			int num2 = -1;
			for (int num3 = GlobalMembers.gComplementCount - 1; num3 >= 0; num3--)
			{
				if (num >= UpdateComplements_gComplementPoints[num3])
				{
					num2 = num3;
					if (num3 <= mLastComplement)
					{
						break;
					}
					if (SupportsReplays() && num3 >= 1 && !mIsOneMoveReplay && !mIsWholeGameReplay && !mWantReplaySave)
					{
						mWantReplaySave = true;
						if (mReplayStartMove != null)
						{
							mReplayStartMove.CopyFrom(mMoveDataVector[0]);
						}
						else
						{
							mReplayStartMove = new MoveData();
							mReplayStartMove.CopyFrom(mMoveDataVector[0]);
						}
					}
					if (num3 >= GetMinComplementLevel())
					{
						DoComplement(num3);
					}
					break;
				}
			}
			if (num == 0 || num2 > mLastComplement)
			{
				mLastComplement = num2;
			}
			else if (num2 < mLastComplement - 1)
			{
				mLastComplement = num2 + 1;
			}
		}

		public virtual void DoCombineAnim(Piece i_fromPiece, Piece i_tgtPiece)
		{
			if (i_fromPiece == i_tgtPiece || (i_fromPiece.mFlags != 0 && !i_fromPiece.IsFlagSet(128u)))
			{
				return;
			}
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_E, i_fromPiece.mScale);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_DEST_PCT_B, i_fromPiece.mDestPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ALPHA, i_fromPiece.mAlpha);
			i_fromPiece.mDestPct.mIncRate *= GetMatchSpeed();
			i_fromPiece.mDestCol = i_tgtPiece.mCol;
			i_fromPiece.mDestRow = i_tgtPiece.mRow;
			int num = i_tgtPiece.mCol - i_fromPiece.mCol;
			int num2 = i_tgtPiece.mRow - i_fromPiece.mRow;
			if (!i_tgtPiece.IsFlagSet(1u))
			{
				return;
			}
			PopAnimEffect popAnimEffect = PopAnimEffect.fromPopAnim(GlobalMembersResourcesWP.POPANIM_FLAMEGEMCREATION);
			popAnimEffect.mPieceRel = i_fromPiece;
			popAnimEffect.mX = i_fromPiece.CX();
			popAnimEffect.mY = i_fromPiece.CY();
			popAnimEffect.mOverlay = true;
			if (num != 0)
			{
				popAnimEffect.Play("smear horizontal");
				if (num < 0)
				{
					popAnimEffect.mAngle = (float)Math.PI;
				}
			}
			else
			{
				popAnimEffect.Play("smear vertical");
				if (num2 < 0)
				{
					popAnimEffect.mAngle = (float)Math.PI;
				}
			}
			mPostFXManager.AddEffect(popAnimEffect);
		}

		public virtual void ProcessMatches(List<MatchSet> theMatches, Dictionary<Piece, int> theTallySet)
		{
			ProcessMatches(theMatches, theTallySet, false);
		}

		public virtual void ProcessMatches(List<MatchSet> theMatches, Dictionary<Piece, int> theTallySet, bool fromUpdateSwapping)
		{
		}

		public bool DecrementAllCounterGems(bool immediate)
		{
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && !piece.IsFlagSet(320u))
				{
					DecrementCounterGem(piece, immediate);
				}
			}
			return false;
		}

		public void DecrementAllDoomGems(bool immediate)
		{
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.IsFlagSet(256u))
				{
					DecrementCounterGem(piece, immediate);
				}
			}
		}

		public virtual bool DecrementCounterGem(Piece thePiece, bool immediate)
		{
			if (thePiece.mCounter > 0)
			{
				if (immediate)
				{
					thePiece.mCounter--;
					if (thePiece.mCounter == 0 && thePiece.IsFlagSet(512u) && mGameOverCount == 0)
					{
						GameOver();
					}
				}
				else if (thePiece.IsFlagSet(512u))
				{
					thePiece.mSpinFrame = 1f;
					thePiece.mSpinSpeed = GlobalMembers.M(0.33f);
				}
				else if (thePiece.IsFlagSet(128u))
				{
					thePiece.mCounter--;
				}
			}
			return false;
		}

		public void SetMoveCredit(Piece thePiece, int theMoveCreditId)
		{
			thePiece.mMoveCreditId = Math.Max(thePiece.mMoveCreditId, theMoveCreditId);
		}

		public void SetTutorialCleared(int theTutorial)
		{
			SetTutorialCleared(theTutorial, true);
		}

		public void SetTutorialCleared(int theTutorial, bool isCleared)
		{
			if (isCleared)
			{
				mTutorialFlags |= 1 << theTutorial;
			}
			else
			{
				mTutorialFlags &= ~(1 << theTutorial);
			}
			if (!mIsWholeGameReplay)
			{
				GlobalMembers.gApp.mProfile.SetTutorialCleared(theTutorial, isCleared);
			}
		}

		public void DeferTutorialDialog(int theTutorialFlag, Piece thePiece)
		{
			if (WantsTutorialReplays())
			{
				mReplayStartMove.CopyFrom(mMoveDataVector[0]);
				mReplayStartMove.mPartOfReplay = true;
				GlobalMembers.gGR.ClearOperationsTo(mReplayStartMove.mUpdateCnt - 1);
				mReplayIgnoredMoves = 0;
				mReplayHadIgnoredMoves = false;
			}
			DeferredTutorial deferredTutorial = new DeferredTutorial();
			deferredTutorial.mTutorialFlag = theTutorialFlag;
			deferredTutorial.mPieceId = thePiece.mId;
			SaveReplay(deferredTutorial.mReplayData);
			mDeferredTutorialVector.Add(deferredTutorial);
			HideReplayWidget();
		}

		public void CheckForTutorialDialogs()
		{
			if (GlobalMembers.gApp.GetDialog(18) != null || GlobalMembers.gApp.GetDialog(19) != null)
			{
				return;
			}
			if (mLevelCompleteCount != 0 || mGameOverCount != 0 || GlobalMembers.gApp.HasClearedTutorial(19))
			{
				mDeferredTutorialVector.Clear();
				return;
			}
			while (mDeferredTutorialVector.Count > 0)
			{
				if (mTimeExpired)
				{
					mDeferredTutorialVector.Clear();
					break;
				}
				DeferredTutorial deferredTutorial = mDeferredTutorialVector[0];
				if (deferredTutorial.mPieceId == -2)
				{
					mDeferredTutorialVector.RemoveAt(0);
					break;
				}
				if (GetPieceById(deferredTutorial.mPieceId) == null)
				{
					SetTutorialCleared(deferredTutorial.mTutorialFlag, false);
					mDeferredTutorialVector.RemoveAt(0);
					if (((PauseMenu)GlobalMembers.gApp.mMenus[7]).mTopButton != null)
					{
						((PauseMenu)GlobalMembers.gApp.mMenus[7]).mTopButton.SetDisabled(false);
					}
					continue;
				}
				if (mHasReplayData)
				{
					HideReplayWidget();
					mHasReplayData = false;
				}
				string theHeader = string.Empty;
				string theText = string.Empty;
				switch ((TutorialsEnum)deferredTutorial.mTutorialFlag)
				{
				case TutorialsEnum.TUTORIAL_FLAME:
					theHeader = GlobalMembers._ID("FLAME GEM", 96);
					theText = GlobalMembers._ID("You made a FLAME GEM by matching 4 Gems in a row. Match it for an explosion!", 3221);
					break;
				case TutorialsEnum.TUTORIAL_LASER:
					theHeader = GlobalMembers._ID("STAR GEM", 98);
					theText = GlobalMembers._ID("You made a STAR GEM by creating two intersecting matches!", 3222);
					break;
				case TutorialsEnum.TUTORIAL_HYPERCUBE:
					theHeader = GlobalMembers._ID("HYPERCUBE", 100);
					theText = GlobalMembers._ID("You made a HYPERCUBE by matching 5 Gems in a row. Swap it to trigger!", 3223);
					break;
				case TutorialsEnum.TUTORIAL_SUPERNOVA:
					theHeader = GlobalMembers._ID("SUPERNOVA GEM", 102);
					theText = GlobalMembers._ID("You made a SUPERNOVA GEM by matching 6+ Gems in a row. Match it to release the force of a million suns. ", 3224);
					break;
				case TutorialsEnum.TUTORIAL_MULTIPLIER:
					theHeader = GlobalMembers._ID("MULTIPLIER GEM", 104);
					theText = GlobalMembers._ID("You have received a MULTIPLIER GEM! Match it to multiply your score for the rest of the game.", 3225);
					break;
				case TutorialsEnum.TUTORIAL_COIN:
					theHeader = GlobalMembers._ID("COIN", 106);
					theText = GlobalMembers._ID("You have received a COIN! Collect them to buy Boosts to power you up!", 3226);
					break;
				case TutorialsEnum.TUTORIAL_TIME_BONUS:
					theHeader = GlobalMembers._ID("TIME BONUS", 108);
					theText = GlobalMembers._ID("You have received a TIME GEM! Collect them to extend your game after the timer bar empties!", 3227);
					break;
				case TutorialsEnum.TUTORIAL_INFERNO:
					theHeader = GlobalMembers._ID("VERTICAL MATCH", 110);
					theText = GlobalMembers._ID("Making a VERTICAL MATCH will destroy an ice column. Destroy more ice columns to increase your multiplier bonus!", 3228);
					break;
				case TutorialsEnum.TUTORIAL_DIG_DARKROCK:
					theHeader = GlobalMembers._ID("DARK ROCK", 112);
					theText = GlobalMembers._ID("Dark rocks can't be destroyed by normal matching. Use special Gems to destroy them!", 3229);
					break;
				case TutorialsEnum.TUTORIAL_POKER_SKULL:
					theHeader = GlobalMembers._ID("Poker skull", 3230);
					theText = GlobalMembers._ID("Destroy the skull by filling the skull bar!", 3231);
					break;
				}
				bool allowReplay = deferredTutorial.mReplayData.mSaveBuffer.GetDataLen() > 0 && WantsTutorialReplays();
				if (!GlobalMembers.gApp.mProfile.HasClearedTutorial(deferredTutorial.mTutorialFlag))
				{
					Bej3Widget.mCurrentSlidingMenu = GlobalMembers.gApp.mMenus[7];
					Piece pieceById = GetPieceById(deferredTutorial.mPieceId);
					HintDialog hintDialog = new HintDialog(theHeader, theText, allowReplay, true, pieceById, this);
					GlobalMembers.gApp.AddDialog(18, hintDialog);
					hintDialog.mFlushPriority = 1;
					GlobalMembers.gApp.mMenus[7].Transition_SlideOut();
					SetTutorialCleared(deferredTutorial.mTutorialFlag);
					mTutorialPieceIrisPct.SetCurve("b;0,1,0.028571,1,####         ~~###");
					break;
				}
				mDeferredTutorialVector.RemoveAt(0);
				if (((PauseMenu)GlobalMembers.gApp.mMenus[7]).mTopButton != null)
				{
					((PauseMenu)GlobalMembers.gApp.mMenus[7]).mTopButton.SetDisabled(false);
				}
			}
		}

		public bool UpdateBulging()
		{
			bool result = false;
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.mIsBulging)
				{
					result = true;
					if (!piece.mScale.IncInVal())
					{
						piece.mScale.SetConstant(1.0);
						piece.mIsBulging = false;
					}
				}
			}
			return result;
		}

		public void FlipHeldSwaps()
		{
			for (int i = 0; i < mSwapDataVector.Count; i++)
			{
				SwapData swapData = mSwapDataVector[i];
				if (swapData.mHoldingSwap > 0 && swapData.mPiece1 != null && swapData.mPiece2 != null)
				{
					int mCol = swapData.mPiece1.mCol;
					swapData.mPiece1.mCol = swapData.mPiece2.mCol;
					swapData.mPiece2.mCol = mCol;
					mCol = swapData.mPiece1.mRow;
					swapData.mPiece1.mRow = swapData.mPiece2.mRow;
					swapData.mPiece2.mRow = mCol;
					mBoard[swapData.mPiece1.mRow, swapData.mPiece1.mCol] = swapData.mPiece1;
					mBoard[swapData.mPiece2.mRow, swapData.mPiece2.mCol] = swapData.mPiece2;
					swapData.mSwapDir.mX = -swapData.mSwapDir.mX;
					swapData.mSwapDir.mY = -swapData.mSwapDir.mY;
				}
			}
		}

		public void UpdateSwapping()
		{
			for (int i = 0; i < mSwapDataVector.Count; i++)
			{
				SwapData swapData = mSwapDataVector[i];
				bool flag = false;
				swapData.mGemScale.IncInVal();
				if (!swapData.mSwapPct.IncInValScalar(1f))
				{
					flag = true;
				}
				int num = GetColX(swapData.mPiece1.mCol) + swapData.mSwapDir.mX * 100 / 2;
				int num2 = GetRowY(swapData.mPiece1.mRow) + swapData.mSwapDir.mY * 100 / 2;
				swapData.mPiece1.mX = (float)((double)num - (double)swapData.mSwapPct * (double)swapData.mSwapDir.mX * 100.0 / 2.0);
				swapData.mPiece1.mY = (float)((double)num2 - (double)swapData.mSwapPct * (double)swapData.mSwapDir.mY * 100.0 / 2.0);
				if (!swapData.mDestroyTarget && swapData.mPiece2 != null)
				{
					swapData.mPiece2.mX = (float)((double)num + (double)swapData.mSwapPct * (double)swapData.mSwapDir.mX * 100.0 / 2.0);
					swapData.mPiece2.mY = (float)((double)num2 + (double)swapData.mSwapPct * (double)swapData.mSwapDir.mY * 100.0 / 2.0);
				}
				if (!flag)
				{
					continue;
				}
				if (WriteUReplayCmd(1))
				{
					if (swapData.mForwardSwap)
					{
						mUReplayBuffer.WriteByte(2);
					}
					else
					{
						mUReplayBuffer.WriteByte(3);
					}
					EncodePieceRef(swapData.mPiece1);
					EncodePieceRef(swapData.mPiece2);
				}
				if (swapData.mForwardSwap && !mInUReplay)
				{
					bool flag2 = swapData.mForceSwap || ForceSwaps();
					int num3 = swapData.mPiece1.mRow + swapData.mSwapDir.mY;
					int num4 = swapData.mPiece1.mCol + swapData.mSwapDir.mX;
					for (int j = 0; j < 2; j++)
					{
						if (swapData.mDestroyTarget)
						{
							PieceDestroyedInSwap(swapData.mPiece2);
							swapData.mPiece2.Dispose();
							swapData.mPiece2 = null;
						}
						if (swapData.mPiece2 != null)
						{
							int mCol = swapData.mPiece1.mCol;
							swapData.mPiece1.mCol = swapData.mPiece2.mCol;
							swapData.mPiece2.mCol = mCol;
							mCol = swapData.mPiece1.mRow;
							swapData.mPiece1.mRow = swapData.mPiece2.mRow;
							swapData.mPiece2.mRow = mCol;
							mBoard[swapData.mPiece1.mRow, swapData.mPiece1.mCol] = swapData.mPiece1;
							mBoard[swapData.mPiece2.mRow, swapData.mPiece2.mCol] = swapData.mPiece2;
						}
						else
						{
							int mCol2 = swapData.mPiece1.mCol;
							swapData.mPiece1.mCol = num4;
							num4 = mCol2;
							mCol2 = swapData.mPiece1.mRow;
							swapData.mPiece1.mRow = num3;
							num3 = mCol2;
							mBoard[swapData.mPiece1.mRow, swapData.mPiece1.mCol] = swapData.mPiece1;
							mBoard[num3, num4] = swapData.mPiece2;
						}
						swapData.mIgnore = j == 0;
						swapData.mPiece1.mSwapTick = mUpdateCnt;
						if (swapData.mPiece2 != null)
						{
							swapData.mPiece2.mSwapTick = mUpdateCnt;
						}
						mLastComboCount = mComboCount;
						int num5 = 0;
						if (j == 1 || (num5 = FindSets(true, swapData.mPiece1, swapData.mPiece2)) != 0 || flag2)
						{
							if ((num5 == 2 && !flag2) || (swapData.mPiece1 != null && swapData.mPiece1.mIsBulging) || (swapData.mPiece2 != null && swapData.mPiece2.mIsBulging))
							{
								swapData.mHoldingSwap++;
								if (swapData.mHoldingSwap <= GlobalMembers.M(400))
								{
									flag = false;
									continue;
								}
								num5 = 0;
								swapData.mHoldingSwap = 0;
							}
							if (num5 != 0)
							{
								MatchMade(swapData);
							}
							else if (flag2)
							{
								DecrementAllDoomGems(false);
							}
							if (j == 0 && mLastComboCount == mComboCount && swapData.mPiece1.mColor == mLastPlayerSwapColor)
							{
								ComboFailed();
							}
							if (j == 0)
							{
								AddToStat(15);
								AddToStat(20);
								SwapSucceeded(swapData);
							}
							break;
						}
						if (WriteUReplayCmd(1))
						{
							mUReplayBuffer.WriteByte(1);
							EncodePieceRef(swapData.mPiece1);
							EncodePieceRef(swapData.mPiece2);
						}
						AddToStat(15);
						SwapFailed(swapData);
						if (GlobalMembers.gApp.mProfile.mStats[20] < 3 && !mInReplay && !GlobalMembers.gApp.mProfile.HasClearedTutorial(19))
						{
							GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
							mIllegalMoveTutorial = true;
							Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.DoDialog(18, true, GlobalMembers._ID("Illegal Move", 114), GlobalMembers._ID("Each swap must create a row of three or more identical gems.", 115), GlobalMembers._ID("OK", 116), 3);
							GlobalMembers.gApp.mMenus[7].Transition_SlideOut();
							bej3Dialog.mFlushPriority = 1;
							if (swapData.mPiece1 != null)
							{
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SELECTOR_ALPHA, swapData.mPiece1.mSelectorAlpha);
								if (swapData.mPiece2 != null)
								{
									GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SELECTOR_ALPHA, swapData.mPiece2.mSelectorAlpha);
								}
								if (swapData.mPiece1.mRow >= 4)
								{
									bej3Dialog.Move(GlobalMembers.S(GetBoardCenterX()) - bej3Dialog.mWidth / 2, (int)(GlobalMembers.S(swapData.mPiece1.CY()) - (float)bej3Dialog.mHeight + (float)GlobalMembers.MS(-220)));
								}
								else
								{
									bej3Dialog.Move(GlobalMembers.S(GetBoardCenterX()) - bej3Dialog.mWidth / 2, (int)(GlobalMembers.S(swapData.mPiece1.CY()) + (float)GlobalMembers.MS(220)));
								}
							}
						}
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_START_ROTATE);
						swapData.mHoldingSwap = 0;
						swapData.mForwardSwap = false;
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SWAP_PCT_2, swapData.mSwapPct);
						if (SexyFramework.GlobalMembers.gIs3D)
						{
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_SCALE_2, swapData.mGemScale);
						}
						swapData.mIgnore = false;
						flag = false;
					}
				}
				if (!flag || mInUReplay)
				{
					continue;
				}
				for (i = 0; i < mSwapDataVector.Count; i++)
				{
					SwapData swapData2 = mSwapDataVector[i];
					if (swapData2 == swapData)
					{
						mSwapDataVector.RemoveAt(i);
						i--;
						break;
					}
				}
			}
		}

		public virtual void UpdateFalling()
		{
			if (!CanPiecesFall())
			{
				if (mGemFallDelay > 0)
				{
					mGemFallDelay--;
				}
				Piece[,] array = mBoard;
				foreach (Piece piece in array)
				{
					if (piece != null && piece.mFallVelocity != 0f)
					{
						piece.mFallVelocity = 0.01f;
					}
				}
				for (int k = 0; k < 8; k++)
				{
					mBumpVelocities[k] = 0f;
				}
				return;
			}
			int num = 0;
			int num2 = 0;
			for (int l = 0; l < 8; l++)
			{
				float num3 = 1200f;
				float mFallVelocity = 0f;
				for (int num4 = 7; num4 >= 0; num4--)
				{
					Piece piece2 = mBoard[num4, l];
					if (piece2 != null && (piece2.mFallVelocity < 0f || (!IsPieceMatching(piece2) && !IsPieceSwapping(piece2, false, false))))
					{
						piece2.mY += piece2.mFallVelocity;
						if (piece2.mY >= (float)GetRowY(num4))
						{
							piece2.mY = GetRowY(num4);
							if (piece2.mFallVelocity >= GlobalMembers.M(2f))
							{
								num++;
								num2 += (int)piece2.GetScreenX() + 50;
							}
							if (piece2.mFallVelocity > 0f)
							{
								piece2.mFallVelocity = 0f;
							}
						}
						else if (piece2.mY >= num3 - 100f)
						{
							piece2.mY = num3 - 100f;
							piece2.mFallVelocity = mFallVelocity;
						}
						else
						{
							piece2.mFallVelocity += GetGravityFactor() * GlobalMembers.M(0.21995f);
						}
						if (piece2.mFallVelocity != 0f)
						{
							piece2.mLastActiveTick = mUpdateCnt;
						}
						num3 = piece2.mY;
						mFallVelocity = piece2.mFallVelocity;
					}
				}
			}
			if (num > 0 && Math.Abs(mLastHitSoundTick - mUpdateCnt) >= GlobalMembers.M(8))
			{
				mLastHitSoundTick = mUpdateCnt;
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_GEM_HIT, GetPanPosition(num2 / num));
			}
		}

		public void UpdateHint()
		{
			if (GlobalMembers.gApp.mProfile.mAutoHint && mBoardHidePct == 0f && IsBoardStill() && CanPlay())
			{
				mWantHintTicks++;
				if (mWantHintTicks == GetHintTime() * 100)
				{
					ShowHint(false);
				}
			}
		}

		public void UpdateLevelBar()
		{
			if (mLevelBarPIEffect == null)
			{
				return;
			}
			float levelPct = GetLevelPct();
			if (mLevelBarPct < levelPct)
			{
				if ((double)mTimerAlpha == 0.0)
				{
					mLevelBarPct = Math.Min(levelPct, mLevelBarPct + (levelPct - mLevelBarPct) * GlobalMembers.M(0.0255f) + GlobalMembers.M(0.0012f));
				}
				else
				{
					mLevelBarPct = Math.Min(levelPct, mLevelBarPct + (levelPct - mLevelBarPct) * GlobalMembers.M(0.025f) + GlobalMembers.M(0.0005f));
				}
			}
			else
			{
				mLevelBarPct = Math.Max(levelPct, mLevelBarPct + (levelPct - mLevelBarPct) * GlobalMembers.M(0.05f) - GlobalMembers.M(0.0001f));
			}
			UpdateLevelBarEffect();
			CheckWin();
		}

		public virtual void UpdateLevelBarEffect()
		{
			Rect levelBarRect = GetLevelBarRect();
			PILayer layer = mLevelBarPIEffect.GetLayer(0);
			PIEmitterInstance emitter = layer.GetEmitter(0);
			emitter.mEmitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(0f, levelBarRect.mHeight / 2).ToXnaVector2();
			emitter.mEmitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue = new FPoint(mLevelBarPct * (float)levelBarRect.mWidth + (float)mLevelBarSizeBias, levelBarRect.mHeight / 2).ToXnaVector2();
			layer = mLevelBarPIEffect.GetLayer(1);
			emitter = layer.GetEmitter(0);
			emitter.mEmitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(0f, levelBarRect.mHeight / 2).ToXnaVector2();
			emitter.mEmitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue = new FPoint(mLevelBarPct * (float)levelBarRect.mWidth + (float)mLevelBarSizeBias, levelBarRect.mHeight / 2).ToXnaVector2();
		}

		public void UpdateCountdownBar()
		{
			if (mCountdownBarPIEffect == null)
			{
				return;
			}
			float countdownPct = GetCountdownPct();
			if (mCountdownBarPct < countdownPct)
			{
				if ((double)mTimerAlpha == 0.0)
				{
					mCountdownBarPct = Math.Min(countdownPct, mCountdownBarPct + (countdownPct - mCountdownBarPct) * GlobalMembers.M(0.0275f) + GlobalMembers.M(0.00125f));
				}
				else
				{
					mCountdownBarPct = Math.Min(countdownPct, mCountdownBarPct + (countdownPct - mCountdownBarPct) * GlobalMembers.M(0.025f) + GlobalMembers.M(0.0005f));
				}
			}
			else
			{
				mCountdownBarPct = Math.Max(countdownPct, mCountdownBarPct + (countdownPct - mCountdownBarPct) * GlobalMembers.M(0.05f) - GlobalMembers.M(0.0001f));
			}
			Rect countdownBarRect = GetCountdownBarRect();
			PILayer layer = mCountdownBarPIEffect.GetLayer(0);
			PIEmitterInstance emitter = layer.GetEmitter(0);
			emitter.mEmitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(0f, countdownBarRect.mHeight / 2).ToXnaVector2();
			emitter.mEmitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue = new FPoint(mCountdownBarPct * (float)countdownBarRect.mWidth, countdownBarRect.mHeight / 2).ToXnaVector2();
			layer = mCountdownBarPIEffect.GetLayer(1);
			emitter = layer.GetEmitter(0);
			emitter.mEmitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue = new FPoint(0f, countdownBarRect.mHeight / 2).ToXnaVector2();
			emitter.mEmitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue = new FPoint(mCountdownBarPct * (float)countdownBarRect.mWidth, countdownBarRect.mHeight / 2).ToXnaVector2();
			CheckCountdownBar();
		}

		public virtual void CheckCountdownBar()
		{
			float num = Math.Max(0f, (float)GetTicksLeft() / ((float)GetTimeLimit() * 100f));
			if (GetTimeLimit() > 0 && num <= 0f && CanTimeUp() && mDeferredTutorialVector.Count == 0 && mGameOverCount == 0)
			{
				GameOver();
				if (mGameFinished)
				{
					mTimeExpired = true;
				}
			}
		}

		public virtual bool CheckWin()
		{
			int levelPoints = GetLevelPoints();
			if (GetTimeLimit() == 0)
			{
				int levelPointsTotal = GetLevelPointsTotal();
				if (mLevelBarPct >= 1f && levelPointsTotal >= levelPoints && mSpeedBonusFlameModePct == 0f)
				{
					LevelUp();
					return true;
				}
			}
			else if (levelPoints > 0 && GetLevelPointsTotal() >= levelPoints)
			{
				LevelUp();
				return true;
			}
			return false;
		}

		public virtual bool WantWarningGlow()
		{
			return WantWarningGlow(false);
		}

		public virtual bool WantWarningGlow(bool forSound)
		{
			int timeLimit = GetTimeLimit();
			int ticksLeft = GetTicksLeft();
			int num = ((timeLimit > 60) ? 1500 : 1000);
			if (timeLimit > 0)
			{
				return ticksLeft < num;
			}
			return false;
		}

		public virtual float GetSpeedBonusRamp()
		{
			return GlobalMembers.M(0.075f);
		}

		public virtual Color GetWarningGlowColor()
		{
			int ticksLeft = GetTicksLeft();
			if (ticksLeft == 0)
			{
				return new Color(255, 0, 0, 127);
			}
			int num = ((GetTimeLimit() > 60) ? 1500 : 1000);
			float num2 = (float)(num - ticksLeft) / (float)num;
			int theAlpha = (int)(((float)Math.Sin((float)mUpdateCnt * GlobalMembers.M(0.15f)) * 127f + 127f) * num2 * GetPieceAlpha());
			return new Color(255, 0, 0, theAlpha);
		}

		public virtual bool WantTopLevelBar()
		{
			return GetTimeLimit() == 0;
		}

		public virtual bool WantTopFrame()
		{
			return true;
		}

		public virtual bool WantBottomFrame()
		{
			return true;
		}

		public virtual bool WantDrawButtons()
		{
			return true;
		}

		public virtual bool WantDrawScore()
		{
			return true;
		}

		public virtual bool WantDrawBackground()
		{
			return true;
		}

		public virtual bool WantCountdownBar()
		{
			return GetTimeLimit() > 0;
		}

		public void UpdateMoveData()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < mMoveDataVector.Count; i++)
			{
				MoveData moveData = mMoveDataVector[i];
				if (moveData.mPartOfReplay)
				{
					num2++;
				}
				if (mLightningStorms.Count == 0)
				{
					bool flag = false;
					Piece[,] array = mBoard;
					foreach (Piece piece in array)
					{
						if (piece != null && piece.mMoveCreditId == moveData.mMoveCreditId)
						{
							if (IsPieceStill(piece))
							{
								piece.mLastMoveCreditId = piece.mMoveCreditId;
								piece.mMoveCreditId = -1;
								piece.mLastActiveTick = 0;
							}
							else
							{
								flag = true;
							}
						}
					}
					for (int l = 0; l < 8; l++)
					{
						if (mNextColumnCredit[l] == moveData.mMoveCreditId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						Piece[,] array2 = mBoard;
						foreach (Piece piece2 in array2)
						{
							if (piece2 != null && piece2.mLastMoveCreditId == moveData.mMoveCreditId)
							{
								piece2.mLastMoveCreditId = -1;
							}
						}
						mMoveDataVector.RemoveAt(i);
						i--;
						continue;
					}
				}
				if (moveData.mPartOfReplay)
				{
					num++;
				}
			}
			if (num2 == 1 && num == 0 && mHasReplayData)
			{
				if (!mWantLevelup && mHyperspace == null && mDeferredTutorialVector.Count == 0)
				{
					ShowReplayWidget();
				}
				if (!mInReplay)
				{
					mReplayHadIgnoredMoves = mReplayIgnoredMoves > 0;
					mReplayIgnoredMoves = 0;
				}
			}
		}

		public virtual uint GetRandSeedOverride()
		{
			return 0u;
		}

		public void UpdateReplayPopup()
		{
			if (SupportsReplays())
			{
				if (mHasReplayData && mReplayIgnoredMoves >= 3)
				{
					mHasReplayData = false;
					HideReplayWidget();
				}
				if (GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON != null)
				{
					int num = GlobalMembers.S(GlobalMembers.gApp.mWidth / 2);
					int num2 = (int)(0f - (GlobalMembers.IMG_SYOFS(1094) + (float)GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelHeight()));
					mReplayButton.Resize(GlobalMembers.IMGRECT_NS(GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON, num, (float)((double)ConstantsWP.REPLAY_OFFSET_Y + (double)num2 * (1.0 - (double)mReplayWidgetShowPct))));
				}
			}
		}

		public void UpdateReplay()
		{
			GlobalMembers.gGR.mIgnoreDraws = false;
			GlobalMembers.gGR.mRecordDraws = false;
			if (mRewinding)
			{
				mRewindRand.SRand((uint)((int)GlobalMembers.gApp.mUpdateCount / GlobalMembers.M(3)));
				int num = mPlaybackTimestamp - GlobalMembers.gGR.GetFirstOperationTimestamp();
				int num2 = Math.Min(GlobalMembers.M(2) + num / GlobalMembers.M(100), num / GlobalMembers.M(20));
				mPlaybackTimestamp -= num2;
				if (num2 <= 0)
				{
					mPlaybackTimestamp--;
				}
				if (GlobalMembers.gGR.GetLastOperationTimestamp() < 0 || num <= 0)
				{
					if (mRewindSound != null)
					{
						mRewindSound.Release();
						mRewindSound = null;
					}
					mRewinding = false;
					mPlaybackTimestamp = mUpdateCnt;
					GlobalMembers.gGR.ClearOperationsFrom(0);
					ToggleReplayPulse(true);
				}
				else
				{
					GlobalMembers.gGR.mIgnoreDraws = true;
				}
				return;
			}
			if (mInReplay)
			{
				mRewindRand.SRand((uint)((int)GlobalMembers.gApp.mUpdateCount / GlobalMembers.M(7)));
			}
			int num3 = mUpdateCnt - GlobalMembers.gGR.GetFirstOperationTimestamp();
			if (GlobalMembers.gGR.GetFirstOperationTimestamp() == -1)
			{
				num3 = 0;
			}
			if ((double)mAlpha == 1.0 && !IsBoardStill() && (mReplayIgnoredMoves == 0 || !mHasReplayData) && (!mHasReplayData || num3 < 500))
			{
				int mUpdateCnt2 = mUpdateCnt;
				GlobalMembers.gGR.GetLastOperationTimestamp();
				GlobalMembers.gGR.mRecordDraws = true;
				GlobalMembers.gGR.SetTimestamp(mUpdateCnt);
				GlobalMembers.gGR.ClearOperationsTo(mUpdateCnt - ConstantsWP.BOARD_MAX_REWIND_TIME);
			}
			if (mReplayStartMove.mPartOfReplay && !mHasReplayData)
			{
				GlobalMembers.gGR.ClearOperationsTo(mReplayStartMove.mUpdateCnt - 1);
			}
			GlobalMembers.gGR.mIgnoreDraws = false;
			if (!mInReplay || mQueuedMoveVector.Count != 0 || !IsBoardStill())
			{
				return;
			}
			if (mWantLevelup && !mReplayWasTutorial)
			{
				mWasLevelUpReplay = true;
			}
			if (((mReplayIgnoredMoves > 0 || mReplayHadIgnoredMoves) && !mIsOneMoveReplay && !mIsWholeGameReplay) || mWasLevelUpReplay)
			{
				if ((double)mReplayFadeout == 1.0)
				{
					LoadGame(mPreReplaySave, false);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_REPLAY_FADEOUT_TO_CLEAR, mReplayFadeout);
					mInReplay = false;
					if (mCurrentHint != null)
					{
						OnAchievementHintFinished(mCurrentHint);
						mCurrentHint = null;
					}
					((PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(mReplayWasTutorial ? Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS : Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
					DisableUI(false);
					GlobalMembers.gApp.DisableOptionsButtons(false);
					ToggleReplayPulse(false);
					if (mWasLevelUpReplay)
					{
						mWasLevelUpReplay = false;
						if (mHasReplayData && GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_ZEN)
						{
							ShowReplayWidget();
						}
					}
				}
				else if ((double)mReplayFadeout == 0.0 && (mReplayFadeout.GetInVal() == 0.0 || mReplayFadeout.GetInVal() == 1.0))
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_REPLAY_FADEOUT_TO_WHITE, mReplayFadeout);
					ToggleReplayPulse(false);
				}
			}
			else if (!mIsWholeGameReplay || mGameOverCount >= 350)
			{
				mInReplay = false;
				if (mCurrentHint != null)
				{
					OnAchievementHintFinished(mCurrentHint);
					mCurrentHint = null;
				}
				DisableUI(false);
				if (!mReplayWasTutorial)
				{
					((PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
				}
				GlobalMembers.gApp.DisableOptionsButtons(false);
				ToggleReplayPulse(false);
			}
		}

		public void BackToGame()
		{
			int num = mLevel;
			LoadGame(mPreReplaySave, false);
			int num2 = mLevel;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_REPLAY_FADEOUT_TO_CLEAR, mReplayFadeout);
			double num3 = (double)mReplayFadeout;
			mInReplay = false;
			if (mCurrentHint != null)
			{
				OnAchievementHintFinished(mCurrentHint);
				mCurrentHint = null;
			}
			((PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(mReplayWasTutorial ? Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS : Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			DisableUI(false);
			GlobalMembers.gApp.DisableOptionsButtons(false);
			ToggleReplayPulse(false);
			mWasLevelUpReplay = num != num2;
			mRewinding = false;
			mHasReplayData = true;
			if (mRewindSound != null)
			{
				mRewindSound.Stop();
				mRewindSound = null;
			}
			if (!mReplayWasTutorial)
			{
				ShowReplayWidget();
			}
			else
			{
				HideReplayWidget();
			}
		}

		public virtual void UpdateTooltip()
		{
			if (!AllowTooltips())
			{
				return;
			}
			string theBody = string.Empty;
			string theHeader = string.Empty;
			GlobalMembers.S(GlobalMembers.M(500));
			Point point = default(Point);
			Point point2 = new Point((int)((float)mWidgetManager.mLastMouseX / GlobalMembers.S(1f)), (int)((float)mWidgetManager.mLastMouseY / GlobalMembers.S(1f)));
			if (new Rect(0, 0, 0, 0).Contains(point2.mX, point2.mY))
			{
				return;
			}
			Piece pieceAtScreenXY = GetPieceAtScreenXY(point2.mX, point2.mY);
			if (pieceAtScreenXY == null)
			{
				return;
			}
			int num = (int)((float)point2.mX - (pieceAtScreenXY.GetScreenX() + 50f));
			int num2 = (int)((float)point2.mY - (pieceAtScreenXY.GetScreenY() + 50f));
			float num3 = (float)Math.Sqrt((float)num * (float)num + (float)(num2 * num2));
			if ((double)num3 <= 100.0 * GlobalMembers.M(0.35) && GetTooltipText(pieceAtScreenXY, ref theHeader, ref theBody))
			{
				point = new Point((int)GlobalMembers.S(pieceAtScreenXY.GetScreenX() + 50f), (int)GlobalMembers.S(pieceAtScreenXY.GetScreenY() + 50f));
				if ((float)point.mX > (float)mWidth * GlobalMembers.M(0.7f))
				{
					point.mX -= GlobalMembers.MS(48);
				}
				else
				{
					point.mX += GlobalMembers.MS(48);
				}
			}
		}

		public virtual bool GetTooltipText(Piece thePiece, ref string theHeader, ref string theBody)
		{
			bool result = false;
			if (thePiece.IsFlagSet(1u))
			{
				if (thePiece.IsFlagSet(4u))
				{
					theHeader = GlobalMembers._ID("SUPERNOVA GEM", 117);
					theBody = GlobalMembers._ID("Created by matching 6+ Gems in a row, this powerful Gem explodes with the force of a million suns when matched.", 118);
				}
				else
				{
					theHeader = GlobalMembers._ID("FLAME GEM", 119);
					theBody = GlobalMembers._ID("Created by forming 4 Gems of the same color in a line. Explodes when matched!", 120);
				}
				result = true;
			}
			else if (thePiece.IsFlagSet(4u))
			{
				theHeader = GlobalMembers._ID("STAR GEM", 121);
				theBody = GlobalMembers._ID("Created by making two intersecting matches. Match it to fire lightning 4 ways!", 122);
				result = true;
			}
			else if (thePiece.IsFlagSet(2u))
			{
				theHeader = GlobalMembers._ID("HYPERCUBE", 123);
				theBody = GlobalMembers._ID("Created by matching 5 Gems in a line. Swap it with a Gem to zap all Gems of the same color onscreen.", 124);
				result = true;
			}
			else if (thePiece.IsFlagSet(16u))
			{
				theHeader = GlobalMembers._ID("MULTIPLIER GEM", 125);
				theBody = GlobalMembers._ID("Randomly drops onto your board. Match it to increase your score multiplier by 1!", 126);
				result = true;
			}
			else if (thePiece.IsFlagSet(2048u))
			{
				theHeader = GlobalMembers._ID("DETONATOR", 127);
				theBody = GlobalMembers._ID("Click to detonate all Special Gems on the board.", 128);
				result = true;
			}
			else if (thePiece.IsFlagSet(4096u))
			{
				theHeader = GlobalMembers._ID("SCRAMBLER", 129);
				theBody = GlobalMembers._ID("Click to scramble all Gems on the board.", 130);
				result = true;
			}
			else if (thePiece.IsFlagSet(1024u))
			{
				theHeader = GlobalMembers._ID("COIN", 131);
				theBody = GlobalMembers._ID("Clear this Gem to collect the coin inside! Save money to buy Boosts!", 132);
				result = true;
			}
			else if (thePiece.IsFlagSet(128u))
			{
				theHeader = GlobalMembers._ID("BUTTERFLIES", 133);
				theBody = GlobalMembers._ID("Match butterflies with like-colored Gems to free them.", 134);
				result = true;
			}
			else if (thePiece.IsFlagSet(131072u))
			{
				theHeader = GlobalMembers._ID("TIME GEM", 135);
				theBody = string.Format(GlobalMembers._ID("Match this Gem to add {0} seconds to the clock!", 136), SexyFramework.Common.CommaSeperate(thePiece.mCounter));
				result = true;
			}
			else if (thePiece.IsFlagSet(96u))
			{
				theHeader = GlobalMembers._ID("TIME BOMB", 137);
				theBody = GlobalMembers._ID("Match this Gem before the counter reaches zero!", 138);
				result = true;
			}
			return result;
		}

		public virtual void UpdatePoints()
		{
			if (mUpdateCnt % GlobalMembers.M(4) != 0)
			{
				return;
			}
			if (mDispPoints < mPoints)
			{
				mDispPoints += (int)((float)(mPoints - mDispPoints) * GlobalMembers.M(0.2f)) + 1;
			}
			else if (mDispPoints > mPoints)
			{
				mDispPoints += (int)((float)(mPoints - mDispPoints) * GlobalMembers.M(0.2f)) - 1;
			}
			if (mMoneyDisp < mMoneyDispGoal)
			{
				mMoneyDisp += (int)((float)(mMoneyDispGoal - mMoneyDisp) * GlobalMembers.M(0.5f) + 1f);
				if (mMoneyDisp == mMoneyDispGoal)
				{
					mCoinCatcherAppearing = false;
				}
			}
		}

		public virtual void UpdateGame()
		{
			if (mAnnouncements.Count > 0)
			{
				mAnnouncements[0].Update();
			}
			mSunPosition.IncInVal();
			mAlpha.IncInVal();
			mSideAlpha.IncInVal();
			mSideXOff.IncInVal();
			mScale.IncInVal();
			mPrevPointMultAlpha.IncInVal();
			mPointMultPosPct.IncInVal();
			mPointMultTextMorph.IncInVal();
			mSpeedBonusDisp.IncInVal();
			mSpeedBonusPointsGlow.IncInVal();
			mSpeedBonusPointsScale.IncInVal();
			mTutorialPieceIrisPct.IncInVal();
			mGemCountCurve.IncInVal();
			mGemCountAlpha.IncInVal();
			mGemScalarAlpha.IncInVal();
			mCascadeCountCurve.IncInVal();
			mCascadeCountAlpha.IncInVal();
			mGemScalarAlpha.IncInVal();
			mBoostShowPct.IncInVal();
			mTimerInflate.IncInVal();
			mTimerAlpha.IncInVal();
			if (mKilling)
			{
				return;
			}
			if (mPointMultPosPct.CheckUpdatesFromEndThreshold(1))
			{
				mPointMultTextMorph.SetCurve("b+0,1,0.02,1,####         ~~###");
			}
			if (mBoostShowPct.CheckInThreshold(GlobalMembers.M(0.5f)))
			{
				ResolveMysteryGem();
			}
			if (!IsGameSuspended())
			{
				mGameTicks++;
				if (mGameTicks % 100 == 0)
				{
					AddToStat(0);
				}
				UpdateDeferredSounds();
			}
			FlipHeldSwaps();
			FindSets();
			FlipHeldSwaps();
			if (mGameOverPiece != null)
			{
				UpdateBombExplode();
			}
			else
			{
				if (mLightningStorms.Count == 0)
				{
					FillInBlanks();
				}
				UpdateMoveData();
				UpdateSwapping();
				UpdateFalling();
			}
			mPointMultSoundDelay = Math.Max(0, mPointMultSoundDelay - 1);
			if (mPointMultSoundDelay == 0 && mPointMultSoundQueue.Count > 0)
			{
				GlobalMembers.gApp.PlaySample(mPointMultSoundQueue[0]);
				mPointMultSoundQueue.RemoveAt(0);
				mPointMultSoundDelay = 40;
			}
			if (!CanPlay())
			{
				Piece selectedPiece = GetSelectedPiece();
				if (selectedPiece != null)
				{
					selectedPiece.mSelected = false;
					selectedPiece.mSelectorAlpha.SetConstant(0.0);
				}
			}
			for (int i = 0; i < 8; i++)
			{
				mBumpVelocities[i] = Math.Min(0f, mBumpVelocities[i] + GlobalMembers.M(0.21995f));
			}
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece == null)
				{
					continue;
				}
				if (piece.mImmunityCount != 0)
				{
					piece.mImmunityCount--;
				}
				if (piece.IsFlagSet(352u) && piece.mCounter == 0 && IsBoardStill())
				{
					BombExploded(piece);
				}
				if (piece.mSpinSpeed != 0f)
				{
					int num = 20;
					piece.mSpinFrame += piece.mSpinSpeed;
					if (piece.mSpinFrame < 0f)
					{
						piece.mSpinFrame += num;
					}
					if (piece.IsFlagSet(512u))
					{
						if (piece.mSpinFrame >= (float)num)
						{
							piece.mSpinFrame = 0f;
							piece.mSpinSpeed = 0f;
						}
						if (piece.mSpinSpeed != 0f && piece.mSpinFrame >= 5f && piece.mSpinFrame <= 10f)
						{
							piece.mCounter--;
							piece.StampOverlay();
							piece.mSpinFrame = 16f;
						}
					}
				}
				if (piece.IsFlagSet(64u) && mGameOverCount == 0 && mLevelCompleteCount == 0 && !IsGameSuspended())
				{
					piece.mTimer = (piece.mTimer + 1) % piece.mTimerThreshold;
					if (piece.mTimer == 0)
					{
						DecrementCounterGem(piece, false);
					}
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num2 = 0;
			int num3 = 0;
			Piece[,] array2 = mBoard;
			foreach (Piece piece2 in array2)
			{
				if (piece2 == null || piece2.mExplodeDelay <= 0 || --piece2.mExplodeDelay != 0)
				{
					continue;
				}
				if (piece2.IsFlagSet(32768u))
				{
					int mImmunityCount = piece2.mImmunityCount;
					piece2.mImmunityCount = 1;
					num3 += (int)piece2.CX();
					num2++;
					ExplodeAt((int)piece2.CX(), (int)piece2.CY());
					flag = true;
					piece2.mImmunityCount = mImmunityCount;
					piece2.ClearFlag(32768u);
				}
				else if (piece2.IsFlagSet(524289u) || (piece2.IsFlagSet(4u) && piece2.mImmunityCount > 0))
				{
					if (piece2.IsFlagSet(524288u))
					{
						int num4 = mGameStats[21];
						AddPoints((int)piece2.CX(), (int)piece2.CY(), 1000 * num4, GlobalMembers.gGemColors[piece2.mColor], (uint)piece2.mMatchId, false, false, -1);
						AddToStat(21, 1, piece2.mMoveCreditId);
						int mMoveCreditId = piece2.mMoveCreditId;
						int num5 = 0;
						Piece[,] array3 = mBoard;
						foreach (Piece piece3 in array3)
						{
							if (piece3 == null || !piece3.IsFlagSet(525335u))
							{
								continue;
							}
							if (piece3.IsFlagSet(1024u))
							{
								if (num5 == 0)
								{
									num5++;
								}
								TallyCoin(piece3);
								piece3.ClearFlag(1024u);
							}
							else
							{
								piece3.mMoveCreditId = mMoveCreditId;
								piece3.mExplodeDelay = GlobalMembers.M(1) + num5 * GlobalMembers.M(25);
								piece3.mImmunityCount = 0;
								num5++;
							}
						}
					}
					else
					{
						AddToStat(12, 1, piece2.mMoveCreditId);
					}
					num3 += (int)piece2.CX();
					num2++;
					AddPoints((int)piece2.CX(), (int)piece2.CY(), GlobalMembers.M(20), Color.White, (uint)piece2.mMatchId, true, true, piece2.mMoveCreditId);
					ExplodeAt((int)piece2.CX(), (int)piece2.CY());
					flag = true;
				}
				else
				{
					if (piece2.IsFlagSet(1046u) && TriggerSpecial(piece2, null))
					{
						continue;
					}
					if (piece2.IsFlagSet(128u))
					{
						TallyPiece(piece2, true);
						DeletePiece(piece2);
						continue;
					}
					if (piece2.IsFlagSet(16u))
					{
						if (!flag3)
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_MULTIPLIER_HURRAHED);
							flag3 = true;
						}
					}
					else
					{
						flag2 = true;
					}
					if (piece2.IsFlagSet(6144u))
					{
						AddPoints((int)piece2.CX(), (int)piece2.CY(), GlobalMembers.M(300), Color.White, (uint)piece2.mMatchId, true, true, piece2.mMoveCreditId);
					}
					SmallExplodeAt(piece2, piece2.CX(), piece2.CY(), true, false);
				}
			}
			if (flag)
			{
				if (WantsCalmEffects())
				{
					if (num2 > 0)
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_PREBLAST, 0, GlobalMembers.M(0.5), GlobalMembers.M(-1.0));
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_GEM_SHATTERS, GetPanPosition(num3 / num2), GlobalMembers.M(0.5), GlobalMembers.M(-1.0));
					}
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BOMB_EXPLODE, 0, GlobalMembers.M(0.5), GlobalMembers.M(-1.0));
				}
				else
				{
					if (num2 > 0)
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_PREBLAST);
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_GEM_SHATTERS, GetPanPosition(num3 / num2));
					}
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BOMB_EXPLODE);
				}
			}
			else if (flag2)
			{
				if (WantsCalmEffects())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SMALL_EXPLODE, 0, GlobalMembers.M(0.5), GlobalMembers.M(-1.0));
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SMALL_EXPLODE);
				}
			}
			Piece[,] array4 = mBoard;
			foreach (Piece piece4 in array4)
			{
				if (piece4 == null)
				{
					continue;
				}
				if (!piece4.mScale.IncInVal())
				{
					if ((double)piece4.mScale > 1.0)
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_F, piece4.mScale);
					}
					else if ((double)piece4.mScale == 1.0)
					{
						piece4.mScale.SetConstant(1.0);
					}
					else if ((double)piece4.mScale == 0.0 && !piece4.mDestPct.IsDoingCurve())
					{
						DeletePiece(piece4);
						continue;
					}
				}
				piece4.mSelectorAlpha.IncInVal();
				if (piece4.mRotPct != 0f || piece4.mSelected)
				{
					piece4.mRotPct += GlobalMembers.M(0.02f);
					if (piece4.mRotPct >= 1f)
					{
						piece4.mRotPct = 0f;
					}
				}
				if ((double)piece4.mDestPct != 0.0)
				{
					piece4.mX = (float)((double)GetColX(piece4.mCol) * (1.0 - (double)piece4.mDestPct) + (double)GetColX(piece4.mDestCol) * (double)piece4.mDestPct);
					piece4.mY = (float)((double)GetRowY(piece4.mRow) * (1.0 - (double)piece4.mDestPct) + (double)GetRowY(piece4.mDestRow) * (double)piece4.mDestPct);
				}
				else
				{
					piece4.mFlyVY += piece4.mFlyAY;
					piece4.mX += piece4.mFlyVX;
					piece4.mY += piece4.mFlyVY;
				}
			}
			if (mLightningStorms.Count == 0)
			{
				FillInBlanks();
			}
			if (IsBoardStill())
			{
				mNOfIntentionalMatchesDuringCascade = 0;
				CheckForTutorialDialogs();
				if (mGameOverCount == 0 && !ForceSwaps() && !FindMove(null, 0, true, true) && !mWantLevelup)
				{
					GameOver();
				}
			}
			UpdateLightning();
			if (IsGameIdle())
			{
				mIdleTicks++;
			}
			if (mComboFlashPct.IsInitialized() && !mComboFlashPct.HasBeenTriggered())
			{
				mComboCountDisp = Math.Min(mComboLen, mComboCountDisp + GlobalMembers.M(0.04f));
			}
			else if (mComboCountDisp < (float)mComboCount)
			{
				mComboCountDisp = Math.Min(mComboCount, mComboCountDisp + GlobalMembers.M(0.04f));
			}
			else
			{
				mComboCountDisp = Math.Max(mComboCount, mComboCountDisp - GlobalMembers.M(0.04f));
			}
			if (mComboFlashPct.IsInitialized() && !mComboFlashPct.IncInVal())
			{
				NewCombo();
			}
			UpdateSpeedBonus();
			UpdateCountPopups();
			UpdateComplements();
			if (WantTopLevelBar())
			{
				UpdateLevelBar();
			}
			else
			{
				UpdateCountdownBar();
			}
			UpdateHint();
			if (GlobalMembers.gApp.mMenus[8].GetState() == Bej3WidgetState.STATE_OUT && GlobalMembers.gApp.mMenus[11].GetState() == Bej3WidgetState.STATE_OUT && mGameOverCount > 0)
			{
				mDeferredTutorialVector.Clear();
				if (++mGameOverCount >= GetGameOverCountTreshold() && GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_GAMEDETAILMENU && GlobalMembers.gApp.mDialogList.Count == 0 && !mQuestPortalPct.IsDoingCurve() && GlobalMembers.gApp.mProfile.mDeferredBadgeVector.Count == 0)
				{
					GameOverExit();
				}
			}
			float num9 = (float)(double)mAlphaCurve;
			mPostFXManager.mAlpha = GetPieceAlpha() * num9;
			mPreFXManager.mAlpha = GetPieceAlpha() * num9;
			if (IsBoardStill() && GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_DIAMOND_MINE)
			{
				if (!mSunPosition.IsDoingCurve())
				{
					float num10 = Math.Max(0.1f, (float)mGameStats[15] / ((float)mGameTicks / 100f));
					if ((GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.01f) && mLastSunTick == 0 && mUpdateCnt >= GlobalMembers.M(50)) || (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.003f) * num10 && !mSunFired) || GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.0006f) * num10)
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SUN_POSITION, mSunPosition);
						mSunFired = true;
						mLastSunTick = mUpdateCnt;
					}
				}
				else
				{
					mSunFired = false;
				}
			}
			if ((double)mComboFlashPct == 0.0)
			{
				int num11 = (int)(44.0 - (double)mComboLen * 5.5);
				float num12 = -mComboCount * num11 + (mComboLen - 1) * num11 / 2;
				mComboSelectorAngle += (num12 - mComboSelectorAngle) / 20f;
			}
			if (mWantLevelup && mDeferredTutorialVector.Count == 0 && mQueuedMoveVector.Count == 0 && IsBoardStill() && !mInReplay && !mWasLevelUpReplay)
			{
				if (!mInReplay)
				{
					GlobalMembers.gApp.DisableOptionsButtons(true);
				}
				GlobalMembers.KILL_WIDGET(mHyperspace);
				mHyperspace = null;
				Announcement announcement = new Announcement(this, GlobalMembers._ID("LEVEL\nCOMPLETE", 139));
				announcement.mBlocksPlay = false;
				announcement.mDarkenBoard = false;
				GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_LEVELCOMPLETE);
				GlobalMembers.gApp.mForceBkg = string.Empty;
				SexyFramework.GlobalMembers.gSexyApp.mGraphicsDriver.GetRenderDevice3D();
				mHyperspace = new HyperspaceWhirlpool(this);
				if (mHyperspace != null)
				{
					if (mHyperspace.IsUsing3DTransition())
					{
						HideReplayWidget();
						mHyperspace.SetBGImage(mBackground.GetBackgroundImage());
						mHyperspace.Resize(0, 0, mWidth, mHeight);
						mWidgetManager.AddWidget(mHyperspace);
						mWidgetManager.PutInfront(mHyperspace, this);
					}
					else
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_ALPHA_LEVEL_UP, mAlpha);
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SCALE_LEVEL_UP, mScale);
						mHyperspace.SetBGImage(mBackground.GetBackgroundImage());
						SetupBackground(1);
						mHyperspace.Resize(0, 0, mWidth, mHeight);
						mWidgetManager.AddWidget(mHyperspace);
						mWidgetManager.PutBehind(mHyperspace, this);
						mBackground.mVisible = false;
					}
				}
				mWantLevelup = false;
			}
			if ((double)mPointMultDarkenPct > 0.0 && !mTimeExpired)
			{
				if (mLightningStorms.Count > 0)
				{
					mBoardDarken = (float)Math.Max(mBoardDarken, mPointMultDarkenPct);
				}
				else
				{
					mBoardDarken = (float)(double)mPointMultDarkenPct;
				}
			}
			if (mBoardDarkenAnnounce > 0f)
			{
				mBoardDarken = Math.Max(mBoardDarken, mBoardDarkenAnnounce);
			}
			if (mHintCooldownTicks > 0)
			{
				mHintCooldownTicks--;
				mHintButton.mMouseVisible = false;
			}
			else
			{
				mHintButton.mMouseVisible = true;
			}
			if (mLightningStorms.Count == 0 && mSpeedBonusFlameModePct > 0f)
			{
				mSpeedBonusFlameModePct = Math.Max(0f, mSpeedBonusFlameModePct - GlobalMembers.M(0.00125f));
				if (mSpeedBonusFlameModePct == 0f)
				{
					mSpeedBonusNum = 0.0;
					EncodeSpeedBonus();
				}
			}
			if (mPendingCoinAnimations == 0 && mGameOverCount == 0 && !mInReplay)
			{
				mMoneyDispGoal = GlobalMembers.gApp.GetCoinCount();
			}
			int num13 = 0;
			if (CanPiecesFall())
			{
				num13 |= 1;
			}
			if (!IsGameSuspended())
			{
				num13 |= 2;
			}
			if (num13 != mUReplayGameFlags)
			{
				if (WriteUReplayCmd(6))
				{
					mUReplayBuffer.WriteByte((byte)num13);
				}
				mUReplayGameFlags = num13;
			}
			if (mSettlingDelay > 0)
			{
				mSettlingDelay--;
			}
			if (mScrambleDelayTicks > 0)
			{
				mScrambleDelayTicks--;
			}
			CheckTrialGameFinished();
		}

		public void DoUReplayUpdate()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num5 = 0;
			int num6 = 0;
			Piece[,] array;
			while (!mUReplayBuffer.AtEnd())
			{
				int mReadBitPos = mUReplayBuffer.mReadBitPos;
				byte b = mUReplayBuffer.ReadByte();
				int num7 = mUReplayLastTick;
				if ((b & 0x40) != 0)
				{
					num7 += mUReplayBuffer.ReadShort();
					b &= 0x3F;
				}
				else if ((b & 0x80) != 0)
				{
					num7 += mUReplayBuffer.ReadByte();
					b &= 0x3F;
				}
				if (mUpdateCnt >= num7)
				{
					switch ((UREPLAYCMD)b)
					{
					case UREPLAYCMD.UREPLAYCMD_ADD_GEM:
					{
						int num22 = mUReplayBuffer.ReadByte();
						Piece piece15 = Piece.alloc(this);
						piece15.mFlags = DecodePieceFlags(mUReplayBuffer.ReadByte());
						piece15.mCol = num22 % 8;
						piece15.mRow = num22 / 8;
						piece15.mColor = (sbyte)mUReplayBuffer.ReadByte();
						piece15.mX = GetColX(piece15.mCol);
						piece15.mY = (float)(mUReplayBuffer.ReadShort() * 100) / 256f + (float)GetRowY(0);
						if (piece15.mRow >= 0 && piece15.mRow < 8 && piece15.mCol >= 0 && piece15.mCol < 8)
						{
							if (piece15.IsFlagSet(1024u))
							{
								GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COIN_CREATED, GlobalMembers.M(0), GlobalMembers.M(0.1f));
								piece15.mCounter = 3;
								ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_COINSPARKLE);
								particleEffect.mPieceRel = piece15;
								particleEffect.mDoubleSpeed = true;
								mPostFXManager.AddEffect(particleEffect);
							}
							StartPieceEffect(piece15);
							mBoard[piece15.mRow, piece15.mCol] = piece15;
						}
						else
						{
							piece15.release();
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_SWAP_GEMS:
					{
						int num24 = mUReplayBuffer.ReadByte();
						Piece piece18 = DecodePieceRef();
						Piece piece19 = DecodePieceRef();
						if ((num24 == 2 || num24 == 1) && piece18 != null && piece19 != null)
						{
							Piece piece20 = mBoard[piece18.mRow, piece18.mCol];
							mBoard[piece18.mRow, piece18.mCol] = mBoard[piece19.mRow, piece19.mCol];
							mBoard[piece19.mRow, piece19.mCol] = piece20;
							int mCol2 = piece18.mCol;
							piece18.mCol = piece19.mCol;
							piece19.mCol = mCol2;
							mCol2 = piece18.mRow;
							piece18.mRow = piece19.mRow;
							piece19.mRow = mCol2;
						}
						for (int num25 = 0; num25 < mSwapDataVector.Count; num25++)
						{
							SwapData swapData = mSwapDataVector[num25];
							if (swapData.mPiece1 == piece18 || swapData.mPiece2 == piece19)
							{
								if (swapData.mPiece1 != null)
								{
									swapData.mPiece1.mX = GetColX(swapData.mPiece1.mCol);
									swapData.mPiece1.mY = GetRowY(swapData.mPiece1.mRow);
									swapData.mPiece1 = null;
								}
								if (swapData.mPiece2 != null)
								{
									swapData.mPiece2.mX = GetColX(swapData.mPiece2.mCol);
									swapData.mPiece2.mY = GetRowY(swapData.mPiece2.mRow);
									swapData.mPiece2 = null;
								}
								mSwapDataVector.RemoveAt(num25);
								num25--;
							}
						}
						if (num24 == 2 || num24 == 3 || piece18 == null || piece19 == null)
						{
							break;
						}
						if (num24 == 0)
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_START_ROTATE);
						}
						else
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_START_ROTATE);
						}
						SwapData swapData2 = new SwapData();
						swapData2.mPiece1 = piece18;
						swapData2.mPiece2 = piece19;
						swapData2.mSwapDir = new Point(piece19.mCol - piece18.mCol, piece19.mRow - piece18.mRow);
						if (num24 == 0)
						{
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SELECTOR_ALPHA, swapData2.mPiece1.mSelectorAlpha);
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SWAP_PCT_3, swapData2.mSwapPct);
							if (SexyFramework.GlobalMembers.gIs3D)
							{
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_SCALE_1, swapData2.mGemScale);
							}
							swapData2.mSwapPct.mIncRate *= GetSwapSpeed();
							swapData2.mGemScale.mIncRate *= GetSwapSpeed();
							swapData2.mForwardSwap = true;
						}
						else
						{
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SWAP_PCT_4, swapData2.mSwapPct);
							if (SexyFramework.GlobalMembers.gIs3D)
							{
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_GEM_SCALE_2, swapData2.mGemScale);
							}
							swapData2.mForwardSwap = false;
						}
						swapData2.mIgnore = false;
						swapData2.mForceSwap = false;
						mSwapDataVector.Add(swapData2);
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_MATCH_FOUND:
					{
						List<Piece> list3 = new List<Piece>();
						Piece piece6 = null;
						int num10 = 0;
						int num11 = mUReplayBuffer.ReadByte();
						flag = (num11 & 1) != 0;
						bool flag4 = (num11 & 2) != 0;
						if (!flag4)
						{
							if (mUReplayVersion >= 3)
							{
								num4 = mUReplayBuffer.ReadByte();
							}
							num3++;
						}
						int num12 = mUReplayBuffer.ReadByte();
						for (int k = 0; k < num12; k++)
						{
							Piece piece7 = DecodePieceRef();
							piece7.mX = GetColX(piece7.mCol);
							piece7.mY = GetRowY(piece7.mRow);
							num10 += (int)piece7.GetScreenX();
							int num13 = mUReplayBuffer.ReadByte();
							if (piece7 == null)
							{
								continue;
							}
							if (piece7.IsFlagSet(1024u))
							{
								TallyCoin(piece7);
								piece7.ClearFlag(1024u);
							}
							list3.Add(piece7);
							if (num13 != 0)
							{
								if (num13 == 1)
								{
									Flamify(piece7);
								}
								else if (num13 == 2 && !piece7.IsFlagSet(4u))
								{
									Laserify(piece7);
								}
								else if (num13 == 3)
								{
									Hypercubeify(piece7);
								}
								piece6 = piece7;
							}
						}
						num += num10 / num12;
						num2++;
						if ((num11 & 4) != 0)
						{
							break;
						}
						if (piece6 != null)
						{
							for (int l = 0; l < list3.Count; l++)
							{
								Piece piece8 = list3[l];
								if (piece8 == piece6 || (piece8.mFlags != 0 && !piece8.IsFlagSet(16384u)))
								{
									continue;
								}
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_E, piece8.mScale);
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_DEST_PCT_B, piece8.mDestPct);
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ALPHA, piece8.mAlpha);
								piece8.mScale.mIncRate *= GetMatchSpeed();
								piece8.mDestPct.mIncRate *= GetMatchSpeed();
								piece8.mAlpha.mIncRate *= GetMatchSpeed();
								piece8.mDestCol = piece6.mCol;
								piece8.mDestRow = piece6.mRow;
								int num14 = piece6.mCol - piece8.mCol;
								int num15 = piece6.mRow - piece8.mRow;
								if (!piece6.IsFlagSet(1u))
								{
									continue;
								}
								PopAnimEffect popAnimEffect = PopAnimEffect.fromPopAnim(GlobalMembersResourcesWP.POPANIM_FLAMEGEMCREATION);
								popAnimEffect.mPieceRel = piece8;
								popAnimEffect.mX = piece8.CX();
								popAnimEffect.mY = piece8.CY();
								popAnimEffect.mOverlay = true;
								popAnimEffect.mDoubleSpeed = true;
								if (num14 != 0)
								{
									popAnimEffect.Play("smear horizontal");
									if (num14 < 0)
									{
										popAnimEffect.mAngle = (float)Math.PI;
									}
								}
								else
								{
									popAnimEffect.Play("smear vertical");
									if (num15 < 0)
									{
										popAnimEffect.mAngle = (float)Math.PI;
									}
								}
								mPostFXManager.AddEffect(popAnimEffect);
							}
						}
						else
						{
							if ((num11 & 8) != 0)
							{
								break;
							}
							for (int m = 0; m < list3.Count; m++)
							{
								Piece piece9 = list3[m];
								if (flag4)
								{
									GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_A, piece9.mScale);
								}
								else if (WantBulgeCascades())
								{
									GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_B, piece9.mScale);
								}
								else
								{
									GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_B, piece9.mScale);
								}
								if (!SexyFramework.GlobalMembers.gIs3D)
								{
									piece9.mRotPct = 0f;
								}
								piece9.mScale.mIncRate *= GetMatchSpeed();
							}
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_ADD_POINTS:
					{
						int theX = (int)DecodeX(mUReplayBuffer.ReadShort());
						int theY2 = (int)DecodeY(mUReplayBuffer.ReadShort());
						int thePoints = (int)mUReplayBuffer.ReadLong();
						Color theColor = new Color((int)mUReplayBuffer.ReadLong());
						int theId = mUReplayBuffer.ReadShort();
						bool usePointMultiplier = mUReplayBuffer.ReadBoolean();
						DoAddPoints(theX, theY2, thePoints, theColor, (uint)theId, true, usePointMultiplier, -1);
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_DELETE_GEM:
					{
						Piece piece22 = DecodePieceRef();
						if (piece22 != null)
						{
							DeletePiece(piece22);
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_FILL_BLANKS:
					{
						byte b2 = mUReplayBuffer.ReadByte();
						byte b3 = mUReplayBuffer.ReadByte();
						byte b4 = mUReplayBuffer.ReadByte();
						int num20 = b3;
						for (int num21 = 0; num21 < b4; num21++)
						{
							mBoard[num20, b2] = mBoard[num20 - 1, b2];
							if (mBoard[num20, b2] != null)
							{
								mBoard[num20, b2].mRow++;
							}
							num20--;
						}
						mBoard[num20, b2] = null;
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_GAME_FLAGS:
						mUReplayGameFlags = mUReplayBuffer.ReadByte();
						break;
					case UREPLAYCMD.UREPLAYCMD_TIME_LEFT:
						mUReplayTicksLeft = mUReplayBuffer.ReadShort();
						break;
					case UREPLAYCMD.UREPLAYCMD_SPEED_BONUS_UPDATE:
					{
						int num18 = mUReplayBuffer.ReadByte();
						mSpeedBonusNum = (float)(int)mUReplayBuffer.ReadByte() / 255f;
						if (num18 == mSpeedBonusCount)
						{
							break;
						}
						if (num18 > 0)
						{
							if (mSpeedBonusCount == 0)
							{
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_DISP_ON, mSpeedBonusDisp);
							}
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_POINTS_GLOW, mSpeedBonusPointsGlow);
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_POINTS_SCALE_ON, mSpeedBonusPointsScale);
							mSpeedBonusCountHighest = Math.Max(mSpeedBonusCountHighest, mSpeedBonusCount);
						}
						else
						{
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_DISP_OFF, mSpeedBonusDisp);
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_POINTS_SCALE_OFF_UREPLAY, mSpeedBonusPointsScale);
						}
						mSpeedBonusCount = num18;
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_BLAZING_SPEED:
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SPEED_BONUS_DISP_ON, mSpeedBonusDisp);
						DoSpeedText(0);
						break;
					case UREPLAYCMD.UREPLAYCMD_EXPLODE:
					{
						int num16 = (int)DecodeX(mUReplayBuffer.ReadShort());
						int theY = (int)DecodeY(mUReplayBuffer.ReadShort());
						num6 += num16;
						num5++;
						flag2 = true;
						ExplodeAt(num16, theY);
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_SMALL_EXPLODE:
					{
						flag3 = true;
						Piece piece10 = DecodePieceRef();
						if (piece10 != null)
						{
							SmallExplodeAt(piece10, piece10.CX(), piece10.CY(), true, false);
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_SET_VELOCITY:
					{
						Piece piece23 = DecodePieceRef();
						float val = (float)mUReplayBuffer.ReadShort() / 25600f * 100f;
						if (piece23 == null)
						{
							break;
						}
						for (int num26 = 0; num26 <= piece23.mRow; num26++)
						{
							Piece pieceAtRowCol = GetPieceAtRowCol(num26, piece23.mCol);
							if (pieceAtRowCol != null)
							{
								pieceAtRowCol.mFallVelocity = Math.Min(pieceAtRowCol.mFallVelocity, val);
							}
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_COLLECTED_MULTIPLIER:
					{
						Piece piece21 = DecodePieceRef();
						if (piece21 != null)
						{
							IncPointMult(piece21);
							piece21.ClearFlag(16u);
							mPostFXManager.FreePieceEffect(piece21.mId);
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_FIRING_STAR_GEM:
					{
						Piece piece17 = DecodePieceRef();
						if (piece17 != null)
						{
							LightningStorm item = new LightningStorm(this, piece17, 2);
							mLightningStorms.Add(item);
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_FIRING_HYPERCUBE:
					{
						Piece piece16 = DecodePieceRef();
						int num23 = mUReplayBuffer.ReadByte();
						if (piece16 != null)
						{
							LightningStorm lightningStorm = new LightningStorm(this, piece16, 7);
							lightningStorm.mColor = num23;
							mLightningStorms.Add(lightningStorm);
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_SHOW_GEM_HINT:
					{
						Piece piece14 = DecodePieceRef();
						if (piece14 != null)
						{
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_HINT_ALPHA, piece14.mHintAlpha);
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_HINT_ARROW_POS, piece14.mHintArrowPos);
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_GEM_COUNT_POPUP:
						DoGemCountPopup(mUReplayBuffer.ReadByte());
						break;
					case UREPLAYCMD.UREPLAYCMD_CASCADE_COUNT_POPUP:
						DoCascadePopup(mUReplayBuffer.ReadByte());
						break;
					case UREPLAYCMD.UREPLAYCMD_MYSTERY_DECIDED:
					{
						Piece piece13 = DecodePieceRef();
						int num19 = mUReplayBuffer.ReadByte();
						if (piece13 != null)
						{
							piece13.ClearFlag(8192u);
							switch (num19)
							{
							case 1:
								Flamify(piece13);
								break;
							case 2:
								Laserify(piece13);
								break;
							case 3:
								Hypercubeify(piece13);
								break;
							}
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_DETONATE:
					{
						Piece piece11 = DecodePieceRef();
						if (piece11 == null)
						{
							break;
						}
						array = mBoard;
						foreach (Piece piece12 in array)
						{
							if (piece12 != null && piece12.IsFlagSet(1024u))
							{
								TallyCoin(piece12);
								piece12.ClearFlag(1024u);
							}
						}
						break;
					}
					case UREPLAYCMD.UREPLAYCMD_SCRAMBLE:
					{
						int num8 = mUReplayBuffer.ReadByte();
						List<Piece> list = new List<Piece>();
						List<Piece> list2 = new List<Piece>();
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SCRAMBLE);
						for (int i = 0; i < num8; i++)
						{
							Piece piece = DecodePieceRef();
							Piece piece2 = DecodePieceRef();
							if (piece != null && piece2 != null)
							{
								list.Add(piece);
								list2.Add(piece2);
								piece.mDestCol = piece.mCol;
								piece.mDestRow = piece.mRow;
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_DEST_PCT_C, piece.mDestPct);
							}
						}
						for (int j = 0; j < list.Count; j++)
						{
							Piece piece3 = list[j];
							Piece piece4 = list2[j];
							Piece piece5 = mBoard[piece3.mRow, piece3.mCol];
							mBoard[piece3.mRow, piece3.mCol] = mBoard[piece4.mRow, piece4.mCol];
							mBoard[piece4.mRow, piece4.mCol] = piece5;
							int mCol = piece3.mCol;
							piece3.mCol = piece4.mCol;
							piece4.mCol = mCol;
							mCol = piece3.mRow;
							piece3.mRow = piece4.mRow;
							piece4.mRow = mCol;
							float num9 = piece3.mX;
							piece3.mX = piece4.mX;
							piece4.mX = num9;
							num9 = piece3.mY;
							piece3.mY = piece4.mY;
							piece4.mY = num9;
						}
						if (mScrambleUsesLeft > 1)
						{
							mScrambleUsesLeft--;
						}
						break;
					}
					}
					mUReplayLastTick = mUpdateCnt;
					continue;
				}
				mUReplayBuffer.mReadBitPos = mReadBitPos;
				break;
			}
			if ((mUReplayLastTick == 0 && mUReplayBuffer.AtEnd()) || mUpdateCnt == mUReplayTotalTicks)
			{
				mInReplay = false;
				if (mCurrentHint != null)
				{
					OnAchievementHintFinished(mCurrentHint);
					mCurrentHint = null;
				}
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(mReplayWasTutorial ? Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS : Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
				GlobalMembers.gApp.DisableOptionsButtons(false);
				ToggleReplayPulse(false);
			}
			if (flag2)
			{
				if (num5 > 0)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_PREBLAST);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_GEM_SHATTERS, GetPanPosition(num6 / num5));
				}
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BOMB_EXPLODE);
			}
			else if (flag3)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SMALL_EXPLODE);
			}
			base.Update();
			if (num2 > 0)
			{
				int panPosition = GetPanPosition(num / num2 + 50);
				if (num2 > 1)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DOUBLESET, panPosition);
				}
				int num27 = num4 + 1;
				if (num27 > 6)
				{
					num27 = 6;
				}
				if (flag && mSpeedBonusCount > 0)
				{
					if (mSpeedBonusNum > 0.01)
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_FLAMESPEED1, panPosition, 1.0, mSpeedBonusNum * (double)GlobalMembers.M(1f));
					}
					else
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.GetSoundById(1721 + Math.Min(8, mSpeedBonusCount)), panPosition, 1.0, mSpeedBonusNum * (double)GlobalMembers.M(1f));
					}
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COMBO_1 + num27, panPosition);
				}
			}
			if (mAnnouncements.Count > 0)
			{
				mAnnouncements[0].Update();
			}
			mSunPosition.IncInVal();
			mAlpha.IncInVal();
			mScale.IncInVal();
			mPrevPointMultAlpha.IncInVal();
			mPointMultPosPct.IncInVal();
			mPointMultTextMorph.IncInVal();
			mSpeedBonusDisp.IncInVal();
			mSpeedBonusPointsGlow.IncInVal();
			mSpeedBonusPointsScale.IncInVal();
			mTutorialPieceIrisPct.IncInVal();
			mGemCountCurve.IncInVal();
			mGemCountAlpha.IncInVal();
			mGemScalarAlpha.IncInVal();
			mCascadeCountCurve.IncInVal();
			mCascadeCountAlpha.IncInVal();
			mGemScalarAlpha.IncInVal();
			mBoostShowPct.IncInVal();
			mTimerInflate.IncInVal();
			mTimerAlpha.IncInVal();
			if (mPointMultPosPct.CheckUpdatesFromEndThreshold(1))
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_POINT_MULT_TEXT_MORPH, mPointMultTextMorph);
			}
			array = mBoard;
			foreach (Piece piece24 in array)
			{
				if (piece24 != null && !piece24.mScale.IncInVal() && (double)piece24.mScale > 1.0)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SCALE_F, piece24.mScale);
				}
			}
			array = mBoard;
			int upperBound = array.GetUpperBound(0);
			int upperBound2 = array.GetUpperBound(1);
			for (int n = array.GetLowerBound(0); n <= upperBound; n++)
			{
				for (int num17 = array.GetLowerBound(1); num17 <= upperBound2; num17++)
				{
					array[n, num17]?.mSelectorAlpha.IncInVal();
				}
			}
			array = mBoard;
			foreach (Piece piece25 in array)
			{
				if (piece25 != null && (double)piece25.mDestPct != 0.0)
				{
					piece25.mX = (float)((double)GetColX(piece25.mCol) * (1.0 - (double)piece25.mDestPct) + (double)GetColX(piece25.mDestCol) * (double)piece25.mDestPct);
					piece25.mY = (float)((double)GetRowY(piece25.mRow) * (1.0 - (double)piece25.mDestPct) + (double)GetRowY(piece25.mDestRow) * (double)piece25.mDestPct);
				}
			}
			array = mBoard;
			foreach (Piece piece26 in array)
			{
				if (piece26 != null)
				{
					piece26.mDestPct.IncInVal();
					piece26.mAlpha.IncInVal();
					piece26.mHintAlpha.IncInVal();
					piece26.mHintArrowPos.IncInVal();
				}
			}
			if (mFlameSound == null)
			{
				mFlameSound = SexyFramework.GlobalMembers.gSexyApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_FLAMELOOP);
				if (mFlameSound != null)
				{
					mFlameSound.SetVolume((GlobalMembers.gApp.mMuteCount > 0) ? 0.0 : GlobalMembers.gApp.mSfxVolume);
					mFlameSound.Play(true, false);
				}
			}
			if (mFlameSound != null)
			{
				mFlameSound.SetVolume(Math.Max(0.0, 1.0 - (1.0 - mSpeedBonusNum) * GlobalMembers.M(2.5)) * (double)mAlpha * (double)GetPieceAlpha());
			}
			mPreFXManager.Update();
			mPostFXManager.Update();
			if (!IsGameSuspended() || mLevelCompleteCount != 0)
			{
				mLevelBarPIEffect.Update();
				mCountdownBarPIEffect.Update();
				if (mSpeedBonusNum > 0.0)
				{
					mSpeedFirePIEffect.Update();
				}
				for (int num28 = 0; num28 < 2; num28++)
				{
					if (mSpeedFireBarPIEffect[num28] == null)
					{
						continue;
					}
					mSpeedFireBarPIEffect[num28].Update();
					if (!mSpeedFireBarPIEffect[num28].IsActive())
					{
						if (mSpeedFireBarPIEffect[num28] != null)
						{
							mSpeedFireBarPIEffect[num28].Dispose();
						}
						mSpeedFireBarPIEffect[num28] = null;
					}
				}
			}
			array = mBoard;
			upperBound = array.GetUpperBound(0);
			upperBound2 = array.GetUpperBound(1);
			for (int n = array.GetLowerBound(0); n <= upperBound; n++)
			{
				for (int num17 = array.GetLowerBound(1); num17 <= upperBound2; num17++)
				{
					array[n, num17]?.Update();
				}
			}
			UpdateLevelBar();
			UpdateCountdownBar();
			UpdateSwapping();
			UpdateFalling();
			UpdateLightning();
			UpdatePoints();
			if (mLightningStorms.Count == 0 && mSpeedBonusFlameModePct > 0f)
			{
				mSpeedBonusFlameModePct = (float)Math.Max(0.0, (double)mSpeedBonusFlameModePct - GlobalMembers.M(1.0 / 800.0));
			}
		}

		public virtual void BackToMenu()
		{
			if (!mFromDebugMenu)
			{
				if (!mKilling)
				{
					GlobalMembers.gApp.DoPlayMenu();
					mBackground.SetVisible(false);
					if ((double)mAlpha > 0.0)
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_ALPHA_BACK_TO_MENU, mAlpha);
					}
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SCALE_BACK_TO_MENU, mScale);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BACKTOMAIN);
					mKilling = true;
					DisableUI(true);
				}
			}
			else
			{
				GlobalMembers.gApp.DoMenu();
			}
		}

		public virtual void GameOverExit()
		{
			BackToMenu();
		}

		public virtual void DoUpdate()
		{
			if (GlobalMembers.gApp.GetDialog(41) != null || GlobalMembers.gApp.GetDialog(43) != null || GlobalMembers.gApp.GetDialog(39) != null)
			{
				GlobalMembers.gGR.mRecordDraws = false;
				base.Update();
				return;
			}
			if (GlobalMembers.gApp.GetDialog(18) != null)
			{
				GlobalMembers.gGR.mRecordDraws = false;
			}
			if (((GlobalMembers.gApp.mProfile.mDeferredBadgeVector.Count > 0 && (mGameFinished || (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN && mZenDoBadgeAward))) || mDoRankUp) && (mGameOverCount == 0 || mGameOverCount >= GlobalMembers.M(300)))
			{
				GlobalMembers.gGR.mRecordDraws = false;
				RankBarWidget rankBarWidget = (RankBarWidget)(object)GlobalMembers.gApp.GetDialog(34);
				if (mDoRankUp && GlobalMembers.gApp.GetDialog(23) == null)
				{
					RankUpDialog theDialog = new RankUpDialog(this);
					GlobalMembers.gApp.AddDialog(theDialog);
					mGameStats[1] = 0;
					GlobalMembers.gApp.mMenus[7].Transition_SlideOut();
					mDoRankUp = false;
				}
				else if (GlobalMembers.gApp.mProfile.mDeferredBadgeVector.Count > 0 && (mGameFinished || (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN && mZenDoBadgeAward)) && rankBarWidget == null && mHyperspace == null)
				{
					if (GlobalMembers.gApp.mMenus[8].GetState() == Bej3WidgetState.STATE_OUT)
					{
						GlobalMembers.gApp.DoBadgeMenu(1, GlobalMembers.gApp.mProfile.mDeferredBadgeVector);
						GlobalMembers.gApp.mMenus[7].Transition_SlideOut();
						GlobalMembers.gApp.mProfile.mDeferredBadgeVector.Clear();
						mZenDoBadgeAward = false;
					}
					return;
				}
				if (GlobalMembers.gApp.GetDialog(23) != null)
				{
					mSlideUIPct.IncInVal();
					return;
				}
			}
			bool mHadReplayError2 = mHadReplayError;
			if (mInUReplay)
			{
				DoUReplayUpdate();
				return;
			}
			if ((double)mAlpha == 1.0 && (double)mScale == 1.0 && mUpdateCnt >= 200)
			{
				UpdateTooltip();
			}
			if (!mUserPaused)
			{
				mPreFXManager.Update();
				mPostFXManager.Update();
			}
			if (mUserPaused || !GlobalMembers.gApp.mHasFocus || GlobalMembers.gApp.mInterfaceState == InterfaceState.INTERFACE_STATE_GAMEDETAILMENU || (GlobalMembers.gApp.GetDialog(40) != null && (double)mTimerAlpha == 1.0) || GlobalMembers.gApp.GetDialog(21) != null || GlobalMembers.gApp.GetDialog(22) != null)
			{
				mBoardHidePct = Math.Min(1f, mBoardHidePct + GlobalMembers.M(0.035f));
				if (GlobalMembers.gApp.mDialogList.Count == 0 && mStartDelay == 0)
				{
					mVisPausePct = Math.Min(1f, mVisPausePct + GlobalMembers.M(0.035f));
				}
				base.Update();
				if (mVisPausePct >= 1f)
				{
					bool mSuspendingGame2 = mSuspendingGame;
				}
				return;
			}
			if ((double)mScale == 1.0)
			{
				mBoardHidePct = Math.Max(0f, mBoardHidePct - GlobalMembers.M(0.075f));
				mVisPausePct = Math.Max(0f, mVisPausePct - GlobalMembers.M(0.075f));
			}
			if (GlobalMembers.gApp.GetDialog(19) != null)
			{
				return;
			}
			if (mStartDelay > 0)
			{
				if (mStartDelay == GlobalMembers.M(10))
				{
					CheckForTutorialDialogs();
					if (GlobalMembers.gApp.GetDialog(19) != null)
					{
						return;
					}
				}
				if (--mStartDelay == 0)
				{
					DisableUI(false);
				}
				return;
			}
			Piece[,] array = mBoard;
			int upperBound = array.GetUpperBound(0);
			int upperBound2 = array.GetUpperBound(1);
			for (int i = array.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
				{
					array[i, j]?.Update();
				}
			}
			if (GlobalMembers.gApp.mDialogMap.Count != 0 && !mInReplay)
			{
				if (GlobalMembers.gApp.GetDialog(18) != null && mAnnouncements.Count > 0)
				{
					mAnnouncements[0].Update();
				}
				if (GlobalMembers.gApp.GetDialog(44) == null)
				{
					return;
				}
			}
			if ((double)mAlpha == 1.0 && (double)mScale == 1.0 && GlobalMembers.gApp.mDialogMap.Count == 0 && mGameOverCount == 0 && mHasBoardSettled)
			{
				int timeLimit = GetTimeLimit();
				if (timeLimit > 0 && mTimeDelayCount > 0)
				{
					mWantTimeAnnouncement = true;
					if (--mTimeDelayCount == 0)
					{
						Announcement announcement = new Announcement(this, $"{timeLimit / 60}:{timeLimit % 60:d2}");
						announcement.mBlocksPlay = false;
						announcement.mAlpha.mIncRate *= GlobalMembers.M(2.0);
						announcement.mScale.mIncRate *= GlobalMembers.M(2.0);
						announcement.mDarkenBoard = false;
						announcement.mTimeAnnouncement = true;
						if (!GlobalMembers.gApp.mMainMenu.mIsFullGame() && !mTrialPromptShown)
						{
							DoPrompt();
						}
					}
				}
				if (mReadyDelayCount > 0 && --mReadyDelayCount == GlobalMembers.M(110))
				{
					if (GlobalMembers.M(1) != 0)
					{
						Announcement announcement2 = new Announcement(this, GlobalMembers._ID("GET READY", 140));
						announcement2.mBlocksPlay = false;
						announcement2.mAlpha.mIncRate *= GlobalMembers.M(3.0);
						announcement2.mScale.mIncRate *= GlobalMembers.M(3.0);
						announcement2.mDarkenBoard = false;
					}
					GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_GETREADY);
					if (!GlobalMembers.gApp.mMainMenu.mIsFullGame() && !mTrialPromptShown)
					{
						DoPrompt();
					}
				}
				if ((((double)mTimerInflate == 0.0 && (!mWantTimeAnnouncement || mTimeAnnouncementDone)) || GetTimeLimit() == 0 || mGoDelayCount > 1) && mGoDelayCount >= 0 && --mGoDelayCount == 0)
				{
					Announcement announcement3 = new Announcement(this, GlobalMembers._ID("GO!", 141));
					announcement3.mBlocksPlay = false;
					announcement3.mAlpha.mIncRate *= GlobalMembers.M(3.0);
					announcement3.mScale.mIncRate *= GlobalMembers.M(3.0);
					announcement3.mDarkenBoard = false;
					announcement3.mGoAnnouncement = true;
					GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_GO);
					if (!GlobalMembers.gApp.mMainMenu.mIsFullGame() && !mTrialPromptShown)
					{
						DoPrompt();
					}
				}
			}
			if (mCountdownBarPIEffect != null || mLevelCompleteCount != 0)
			{
				mCountdownBarPIEffect.Update();
			}
			if (!IsGameSuspended() || mLevelCompleteCount != 0)
			{
				if (mLevelBarPIEffect != null)
				{
					mLevelBarPIEffect.Update();
				}
				if (mSpeedBonusNum > 0.0 && mSpeedFirePIEffect != null)
				{
					mSpeedFirePIEffect.Update();
				}
				if (!mRewinding)
				{
					for (int k = 0; k < 2; k++)
					{
						if (mSpeedFireBarPIEffect[k] == null)
						{
							continue;
						}
						mSpeedFireBarPIEffect[k].Update();
						if (!mSpeedFireBarPIEffect[k].IsActive())
						{
							if (mSpeedFireBarPIEffect[k] != null)
							{
								mSpeedFireBarPIEffect[k].Dispose();
							}
							mSpeedFireBarPIEffect[k] = null;
						}
					}
				}
			}
			if (SupportsReplays())
			{
				UpdateReplayPopup();
				UpdateReplay();
			}
			else if (mInReplay || (WantsTutorialReplays() && (mTutorialFlags & 0x80000) == 0))
			{
				UpdateReplay();
			}
			if (mRewinding)
			{
				return;
			}
			if (mMouseDown)
			{
				MouseDrag(mWidgetManager.mLastMouseX, mWidgetManager.mLastMouseY);
			}
			if (mWantReplaySave && IsBoardStill())
			{
				DoReplaySetup();
			}
			for (int l = 0; l < mQueuedMoveVector.Count; l++)
			{
				QueuedMove queuedMove = mQueuedMoveVector[l];
				if (mUpdateCnt < queuedMove.mUpdateCnt)
				{
					continue;
				}
				if (queuedMove.mUpdateCnt == mUpdateCnt)
				{
					Piece pieceById = GetPieceById(queuedMove.mSelectedId);
					if (pieceById != null)
					{
						TrySwap(pieceById, queuedMove.mSwappedRow, queuedMove.mSwappedCol, queuedMove.mForceSwap, queuedMove.mPlayerSwapped);
					}
					else
					{
						mHadReplayError = true;
					}
				}
				int num = mUpdateCnt - 1;
				if (mReplayStartMove.mPartOfReplay && !mWantLevelup && !mInReplay)
				{
					num = mReplayStartMove.mUpdateCnt;
				}
				if (queuedMove.mUpdateCnt <= num && IsBoardStill())
				{
					mQueuedMoveVector.RemoveAt(l);
					l--;
				}
			}
			Piece[,] array2 = mBoard;
			foreach (Piece piece in array2)
			{
				if (piece != null)
				{
					piece.mDestPct.IncInVal();
					piece.mAlpha.IncInVal();
					piece.mHintAlpha.IncInVal();
					piece.mHintArrowPos.IncInVal();
				}
			}
			if (mCoinCatcherAppearing)
			{
				mCoinCatcherPctPct = Math.Min(4.0, mCoinCatcherPctPct + 0.028);
			}
			else if (!mTimeExpired || mPointMultiplier == 0)
			{
				mCoinCatcherPctPct = Math.Max(0.0, mCoinCatcherPctPct - 0.028);
			}
			mCoinCatcherPct.SetInVal(Math.Min(1.0, mCoinCatcherPctPct));
			if (UpdateBulging())
			{
				Piece[,] array3 = mBoard;
				foreach (Piece piece2 in array3)
				{
					if (piece2 == null)
					{
						continue;
					}
					piece2.mSelectorAlpha.IncInVal();
					if (piece2.mRotPct != 0f || piece2.mSelected)
					{
						piece2.mRotPct += GlobalMembers.M(0.02f);
						if (piece2.mRotPct >= 1f)
						{
							piece2.mRotPct = 0f;
						}
					}
				}
				return;
			}
			UpdatePoints();
			base.Update();
			UpdateGame();
			if (SupportsReplays())
			{
				UpdateReplayPopup();
			}
			if (mInReplay)
			{
				if (mStateInfoVector.Count > 0)
				{
					StateInfo stateInfo = mStateInfoVector[0];
					if (stateInfo.mUpdateCnt == mUpdateCnt)
					{
						if (stateInfo.mPoints != mPoints || stateInfo.mMoneyDisp != GlobalMembers.gApp.GetCoinCount() || stateInfo.mNextPieceId != mNextPieceId || stateInfo.mIdleTicks != mIdleTicks)
						{
							mHadReplayError = true;
						}
						mStateInfoVector.RemoveAt(0);
					}
				}
			}
			else if (mUpdateCnt % 100 == 0 && WantsWholeGameReplay())
			{
				StateInfo stateInfo2 = new StateInfo();
				stateInfo2.mUpdateCnt = mUpdateCnt;
				stateInfo2.mPoints = mPoints;
				stateInfo2.mMoneyDisp = GlobalMembers.gApp.GetCoinCount();
				stateInfo2.mNextPieceId = mNextPieceId;
				stateInfo2.mIdleTicks = mIdleTicks;
				mWholeGameReplay.mStateInfoVector.Add(stateInfo2);
			}
			if (mIsWholeGameReplay && GlobalMembers.gApp.GetDialog(18) != null)
			{
				GlobalMembers.gApp.KillDialog(18);
				mDeferredTutorialVector.RemoveAt(0);
				mTutorialPieceIrisPct.SetConstant(0.0);
			}
			mBadgeManager.Update();
		}

		public override void Update()
		{
			if (!mContentLoaded || mSuspendingGame)
			{
				return;
			}
			if (mCurrentHint == null)
			{
				if (mAchievementHints.Count > 0)
				{
					mCurrentHint = mAchievementHints[0];
				}
			}
			else if (!mInReplay)
			{
				mCurrentHint.Update();
			}
			if (!AllowUI())
			{
				DisableUI(true);
			}
			if (mMessager != null)
			{
				mMessager.Update();
			}
			if (mRestartPrevImage != null)
			{
				if (!mRestartPct.IncInVal())
				{
					mRestartPrevImage = null;
				}
				for (int i = 0; i < GlobalMembers.M(3); i++)
				{
					Effect effect = mPostFXManager.AllocEffect(Effect.Type.TYPE_NONE);
					effect.mImage = GlobalMembersResourcesWP.IMAGE_VERTICAL_STREAK;
					effect.mScale = GlobalMembers.M(1f) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.2f);
					effect.mDX = GlobalMembers.M(0);
					effect.mDY = GlobalMembers.M(6);
					effect.mX = -160f + GlobalMembersUtils.GetRandFloat() * 1920f;
					effect.mY = (float)((double)mRestartPct * 1200.0);
					effect.mIsAdditive = true;
					effect.mAlpha = 1f;
					effect.mDAlpha = GlobalMembers.M(-0.03f);
					effect.mGravity = 0f;
					mPostFXManager.AddEffect(effect);
				}
				return;
			}
			if (mUpdateCnt == 0 && !mInReplay && WantsWholeGameReplay())
			{
				SaveGame(mWholeGameReplay.mSaveBuffer);
			}
			float num = 1f;
			float num2 = GlobalMembers.M(0.95f);
			if (mRewinding)
			{
				Update_aSpeed = 1f;
			}
			else if (mInReplay && !mIsWholeGameReplay)
			{
				int num3 = mUpdateCnt - mPlaybackTimestamp;
				num = ((num3 >= 10) ? GlobalMembers.M(0.85f) : (Update_aSpeed = GlobalMembers.M(0.3f)));
				if (mIsOneMoveReplay && num3 < GlobalMembers.M(3))
				{
					num = (Update_aSpeed = GlobalMembers.M(0.03f));
				}
				num2 = GlobalMembers.M(0.98f);
				if (mSwapDataVector.Count > 0)
				{
					num = GlobalMembers.M(0.4f);
				}
			}
			Update_aSpeed = (float)((double)(num2 * Update_aSpeed) + (1.0 - (double)num2) * (double)num);
			if (Math.Abs(Update_aSpeed - num) < 0.01f)
			{
				Update_aSpeed = num;
			}
			Update_aSpeed *= GetGameSpeed();
			mUpdateAcc += Update_aSpeed;
			int num4 = (int)mUpdateAcc;
			mUpdateAcc -= num4;
			for (int j = 0; j < num4; j++)
			{
				DoUpdate();
			}
			float num5 = (float)(double)mAlphaCurve;
			mPreFXManager.mAlpha = GetPieceAlpha() * num5;
			mPostFXManager.mAlpha = GetPieceAlpha() * num5;
			if (GlobalMembers.gApp.GetDialog(23) == null && mQuestPortalPct.IsDoingCurve() && !mQuestPortalPct.IncInVal())
			{
				mQuestPortalPct.SetConstant(0.0);
				mQuestPortalCenterPct.SetConstant(0.0);
				mNeedsMaskCleared = true;
				if (mGameOverCount > 0)
				{
					mGameOverCount = GlobalMembers.M(400);
				}
				DoUpdate();
			}
			if (mHintButton != null && mHintCooldownTicks == 0)
			{
				mHintButton.mMouseVisible = (double)GetAlpha() * (double)mSideAlpha == 1.0;
			}
			if (mResetButton != null)
			{
				mResetButton.mMouseVisible = (double)GetAlpha() * (double)mSideAlpha == 1.0;
			}
			if (mBackground != null)
			{
				mBackground.mWantAnim = mHyperspace == null && (double)GetAlpha() == 1.0 && (double)mScale == 1.0 && mHasBoardSettled && !mInReplay && !mSideXOff.IsDoingCurve() && GlobalMembers.gApp.mDialogList.Count == 0;
			}
			if ((double)mAlpha == 0.0 && mKilling && GlobalMembers.gApp.mBoard == this)
			{
				GlobalMembers.KILL_WIDGET(GlobalMembers.gApp.mBoard);
				GlobalMembers.gApp.mBoard = null;
			}
		}

		public bool PieceNeedsEffect(Piece thePiece)
		{
			if (thePiece.mColor >= 0)
			{
				if (!thePiece.IsFlagSet(65536u) || thePiece.mColor < 0)
				{
					return !thePiece.IsFlagSet(482u);
				}
				return true;
			}
			return false;
		}

		public void DrawGemLighting(Graphics g, Piece thePiece, float theX, float theY, float theZ, float theBrightness, float theFalloffFactor, float theFalloffBias)
		{
			GlobalMembers.gFrameLightCount++;
			int theCel = (int)Math.Min(thePiece.mRotPct * (float)GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount(), GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount() - 1);
			DeviceImage deviceImage = (DeviceImage)GlobalMembersResourcesWP.GetImageById(862 + thePiece.mColor);
			deviceImage.GetCelRect(theCel);
			int num = (int)GlobalMembers.S(thePiece.GetScreenX());
			int num2 = (int)GlobalMembers.S(thePiece.GetScreenY());
			float theX2 = GlobalMembers.S(theX) - (float)num;
			float theY2 = GlobalMembers.S(theY) - (float)num2;
			SexyVector3 sexyVector = new SexyVector3(theX2, theY2, theZ).Normalize();
			float[] array = new float[4] { sexyVector.x, sexyVector.y, sexyVector.z, theBrightness };
			Color color = new Color((int)((array[0] + 1f) * 0.5f * 255f), (int)((array[1] + 1f) * 0.5f * 255f), (int)((array[2] + 1f) * 0.5f * 255f), (int)(array[3] * 255f));
			g.SetColor(color);
			g.DrawImageCel(deviceImage, num, num2, theCel);
		}

		public void DrawGemEffectsLighting(Graphics g, bool thePostFX, uint gemMask)
		{
			int[] array = new int[2] { 17, 21 };
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (mPostFXManager.mEffectList[array[i]].Count > 0)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			Graphics3D graphics3D = g.Get3D();
			if (GetPieceAlpha() < 1f || mInReplay || graphics3D == null || !graphics3D.SupportsPixelShaders())
			{
				return;
			}
			g.SetColorizeImages(true);
			g.SetDrawMode(Graphics.DrawMode.Additive);
			RenderEffect effect = graphics3D.GetEffect(GlobalMembersResourcesWP.EFFECT_GEM_LIGHT);
			using (RenderEffectAutoState renderEffectAutoState = new RenderEffectAutoState(g, effect))
			{
				while (!renderEffectAutoState.IsDone())
				{
					for (int j = 0; j < array.Length; j++)
					{
						foreach (Effect item in mPostFXManager.mEffectList[array[j]])
						{
							if (!(item.mLightSize > 0f))
							{
								continue;
							}
							Piece[,] array2 = mBoard;
							foreach (Piece piece in array2)
							{
								if (piece != null && !IsPieceSwapping(piece, false, false) && piece != mGameOverPiece && (!thePostFX || piece.mElectrocutePercent > 0f || (gemMask & (1 << piece.mColor + 1)) != 0) && (!thePostFX || !piece.IsFlagSet(6144u)))
								{
									float num = piece.CX() - item.mX;
									float num2 = piece.CY() - item.mY;
									if (item.mPieceId != piece.mId && num * num + num2 * num2 <= item.mLightSize * item.mLightSize)
									{
										DrawGemLighting(g, piece, item.mX, item.mY, item.mZ, item.mLightIntensity, item.mValue[0], item.mValue[1]);
									}
								}
							}
						}
					}
					renderEffectAutoState.NextPass();
				}
			}
			g.SetColorizeImages(false);
			g.SetDrawMode(Graphics.DrawMode.Normal);
		}

		public void DrawGemLightningLighting(Graphics g, Piece thePiece, LightningStorm anInfo)
		{
			if (!PieceNeedsEffect(thePiece))
			{
				return;
			}
			int theX = (int)GlobalMembers.S(thePiece.GetScreenX());
			int theY = (int)GlobalMembers.S(thePiece.GetScreenY());
			for (int i = 0; i < 2; i++)
			{
				int num = GetRowAt(anInfo.mCY) - thePiece.mRow;
				int num2 = GetColAt(anInfo.mCX) - thePiece.mCol;
				if ((i == 0 && Math.Abs(num) == 1 && (anInfo.mStormType == 0 || anInfo.mStormType == 2)) || (i == 1 && Math.Abs(num2) == 1 && (anInfo.mStormType == 1 || anInfo.mStormType == 2)))
				{
					GlobalMembers.gFrameLightCount++;
					int num3 = GlobalMembers.M(-50);
					num3 = ((i != 0) ? (num3 * num2) : (num3 * num));
					int theCel = (int)Math.Min(thePiece.mRotPct * (float)GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount(), GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount() - 1);
					DeviceImage deviceImage = (DeviceImage)GlobalMembersResourcesWP.GetImageById(862 + thePiece.mColor);
					Rect celRect = deviceImage.GetCelRect(theCel);
					float num4 = (float)(celRect.mX - num3 + 50) / (float)deviceImage.mWidth;
					float num5 = (float)(celRect.mY - num3 + 50) / (float)deviceImage.mHeight;
					float[] array = new float[4] { num4, num5, 0f, 0f };
					float[] array2 = new float[4]
					{
						0f,
						0f,
						(float)(GlobalMembers.M(0.3) * (double)anInfo.mLightingAlpha),
						0f
					};
					if (i == 0)
					{
						array[3] = 0.4f * GlobalMembers.M(0.3f);
						array2[1] = (float)((double)num * GlobalMembers.M(0.8) * (double)anInfo.mLightingAlpha);
					}
					else
					{
						array[2] = 0.5f * GlobalMembers.M(0.3f);
						array2[0] = (float)((double)num2 * GlobalMembers.M(0.8) * (double)anInfo.mLightingAlpha);
					}
					Color color = new Color((int)((array2[0] + 1f) * 0.5f * 255f), (int)((array2[1] + 1f) * 0.5f * 255f), (int)((array2[2] + 1f) * 0.5f * 255f), (int)((array2[3] + 1f) * 0.5f * 255f));
					g.SetColor(color);
					g.DrawImageCel(deviceImage, theX, theY, theCel);
				}
			}
		}

		public void DrawGemSun(Graphics g, Piece thePiece, RenderEffect aEffect)
		{
			if (!PieceNeedsEffect(thePiece))
			{
				return;
			}
			Graphics3D graphics3D = g.Get3D();
			if (!(GetPieceAlpha() < 1f) && graphics3D != null && graphics3D.SupportsPixelShaders())
			{
				int num = (int)((double)(thePiece.CX() - (float)GetBoardX() + (thePiece.CY() - (float)GetRowY(0))) * 0.707);
				int num2 = (int)((double)num - (double)mSunPosition);
				if (Math.Abs(num2) <= GlobalMembers.M(160))
				{
					GlobalMembers.gFrameLightCount++;
					int theCel = (int)Math.Min(thePiece.mRotPct * (float)GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount(), GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount() - 1);
					DeviceImage deviceImage = (DeviceImage)GlobalMembersResourcesWP.GetImageById(862 + thePiece.mColor);
					deviceImage.GetCelRect(theCel);
					float num3 = (float)Math.Atan2(GlobalMembers.M(20f), num2);
					float[] array = new float[4]
					{
						(0f - (float)Math.Cos(num3)) * 0.707f,
						(0f - (float)Math.Cos(num3)) * 0.707f,
						(float)Math.Sin(num3),
						0f
					};
					Color color = new Color((int)((array[0] + 1f) * 0.5f * 255f), (int)((array[1] + 1f) * 0.5f * 255f), (int)((array[2] + 1f) * 0.5f * 255f), (int)((array[3] + 1f) * 0.5f * 255f));
					g.SetColorizeImages(true);
					g.SetColor(color);
					g.DrawImageCel(deviceImage, (int)GlobalMembers.S(thePiece.GetScreenX()), (int)GlobalMembers.S(thePiece.GetScreenY()), theCel);
				}
			}
		}

		public void DrawLaser(Graphics g, Piece thePiece, bool theFront)
		{
			g.SetColor(Color.White);
			for (int i = 0; i < 4; i++)
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = (float)mUpdateCnt * GlobalMembers.M(0.06f);
				switch (i)
				{
				case 0:
					num = (float)Math.Sin(num4);
					num2 = (float)Math.Sin(num4);
					num3 = (float)Math.Cos(num4);
					break;
				case 1:
					num = 0f - (float)Math.Sin(num4 + (float)Math.PI / 4f);
					num2 = (float)Math.Sin(num4 + (float)Math.PI / 4f);
					num3 = (float)Math.Cos(num4 + (float)Math.PI / 4f);
					break;
				case 2:
					num = (float)Math.Sin(num4 + (float)Math.PI / 2f);
					num2 = 0f - (float)Math.Sin(num4 + (float)Math.PI / 2f);
					num3 = (float)Math.Cos(num4 + (float)Math.PI / 2f);
					break;
				case 3:
					num = (float)Math.Sin(num4 + (float)Math.PI * 3f / 4f);
					num2 = (float)Math.Sin(num4 + (float)Math.PI * 3f / 4f);
					num3 = 0f - (float)Math.Cos(num4 + (float)Math.PI * 3f / 4f);
					break;
				}
				if ((num3 < 0f) ^ theFront)
				{
					Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_SPARKLET_BIG, GlobalMembers.S(thePiece.CX() + num * 100f * GlobalMembers.M(0.41f)), GlobalMembers.S(thePiece.CY() + num2 * 100f * GlobalMembers.M(0.41f)), 1f + num3 * GlobalMembers.M(0.3f), 1f + num3 * GlobalMembers.M(0.3f));
				}
				if (theFront && num3 > GlobalMembers.M(-0.5f))
				{
					DrawGemLighting(g, thePiece, thePiece.CX() + num * 100f * GlobalMembers.M(0.41f), thePiece.CY() + num2 * 100f * GlobalMembers.M(0.41f), num3 * GlobalMembers.M(0.07f), 1f, 20.1f, 0f);
				}
			}
		}

		public virtual void DrawHypercube(Graphics g, Piece thePiece)
		{
			int theCel = (int)GlobalMembers.gApp.mUpdateCount / GlobalMembers.M(8) % GlobalMembersResourcesWP.IMAGE_HYPERCUBE_FRAME.GetCelCount();
			int num = (int)GlobalMembers.S(thePiece.GetScreenX() - 16f);
			int num2 = (int)GlobalMembers.S(thePiece.GetScreenY() - 16f);
			g.SetColor(Color.White);
			g.mColor.mAlpha = (int)(GetPieceAlpha() * 255f);
			GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_HYPERCUBE_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_HYPERCUBE_FRAME_ID)) + num, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_HYPERCUBE_FRAME_ID)) + num2, theCel);
			g.SetDrawMode(Graphics.DrawMode.Additive);
			GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_HYPERCUBE_COLORGLOW, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_HYPERCUBE_COLORGLOW_ID)) + num, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_HYPERCUBE_COLORGLOW_ID)) + num2, theCel);
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColor(Color.White);
		}

		public void DrawBombGem(Graphics g, Piece thePiece)
		{
		}

		public void DrawDoomGem(Graphics g, Piece thePiece)
		{
		}

		public virtual void DrawPieceShadow(Graphics g, Piece thePiece)
		{
			if (!thePiece.IsFlagSet(128u))
			{
				float num = (float)(double)thePiece.mScale;
				int num2 = (int)thePiece.GetScreenX();
				int num3 = (int)thePiece.GetScreenY();
				int theNum = (int)((float)num2 + thePiece.mShakeOfsX);
				int theNum2 = (int)((float)num3 + thePiece.mShakeOfsY);
				if (num != 1f)
				{
					g.SetScale(num, num, GlobalMembers.S(num2 + 50), GlobalMembers.S(num3 + 50));
				}
				bool colorizeImages = g.GetColorizeImages();
				g.SetColorizeImages(true);
				float num4 = ((mHyperspace == null || thePiece.mColor < 0) ? ((float)((double)thePiece.mAlpha * (double)GetPieceAlpha())) : ((float)((double)thePiece.mAlpha * (double)mHyperspace.GetPieceAlpha())));
				g.SetColor(new Color(255, 255, 255, (int)(255f * num4)));
				if (thePiece.IsFlagSet(16u))
				{
					int theCel = (int)Math.Min(thePiece.mRotPct * (float)GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount(), GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount() - 1);
					GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.GetImageById(809 + thePiece.mColor), GlobalMembers.S(theNum), GlobalMembers.S(theNum2), theCel);
				}
				else if (thePiece.mColor <= -1 || thePiece.mColor >= 7)
				{
					thePiece.IsFlagSet(4u);
				}
				g.SetColorizeImages(colorizeImages);
				if (num != 1f)
				{
					g.SetScale(1f, 1f, GlobalMembers.S(num2 + 50), GlobalMembers.S(num3 + 50));
				}
			}
		}

		public virtual void DrawPiece(Graphics g, Piece thePiece)
		{
			DrawPiece(g, thePiece, 1f);
		}

		public virtual void DrawPiece(Graphics g, Piece thePiece, float theScale)
		{
			Graphics3D graphics3D = g.Get3D();
			float num = (float)(double)thePiece.mScale;
			float num2 = ((mHyperspace == null || thePiece.mColor < 0) ? ((float)((double)thePiece.mAlpha * (double)GetPieceAlpha())) : ((float)((double)thePiece.mAlpha * (double)mHyperspace.GetPieceAlpha())));
			if (num2 == 0f)
			{
				return;
			}
			num *= theScale;
			int num3 = (int)thePiece.GetScreenX();
			int num4 = (int)thePiece.GetScreenY();
			int num5 = (int)((float)num3 + thePiece.mShakeOfsX);
			int num6 = (int)((float)num4 + thePiece.mShakeOfsY);
			bool flag = false;
			if (num != 1f)
			{
				g.SetScale(num, num, GlobalMembers.S(num3 + 50), GlobalMembers.S(num4 + 50));
			}
			if (mShowMoveCredit)
			{
				g.SetColor(Color.White);
				g.SetFont(GlobalMembersResources.FONT_DIALOG);
				if (thePiece.mMoveCreditId != -1)
				{
					g.DrawString($"{thePiece.mMoveCreditId}", GlobalMembers.S(num3 + GlobalMembers.M(10)), GlobalMembers.S(num4 + GlobalMembers.M(20)));
				}
				if (thePiece.mCounter != 0)
				{
					g.DrawString($"{thePiece.mCounter}", GlobalMembers.S(num3 + GlobalMembers.M(80)), GlobalMembers.S(num4 + GlobalMembers.M(20)));
				}
			}
			g.SetColorizeImages(true);
			g.SetColor(new Color(255, 255, 255, (int)(255f * num2)));
			if (thePiece.IsFlagSet(2u))
			{
				DrawHypercube(g, thePiece);
			}
			else if (thePiece.IsFlagSet(96u))
			{
				DrawBombGem(g, thePiece);
			}
			else if (thePiece.IsFlagSet(256u))
			{
				DrawDoomGem(g, thePiece);
			}
			else if (!thePiece.IsFlagSet(128u))
			{
				flag = true;
			}
			if (flag && thePiece.mColor >= 0)
			{
				if (CanBakeShadow(thePiece))
				{
					if (graphics3D == null)
					{
						float num7 = num;
						if (num7 > 1f)
						{
							num7 = (num7 - 1f) * 2f + 1f;
						}
						int val = (int)((0f - num7 + 2f) * 16f / 2f - 1f);
						val = Math.Max(0, Math.Min(val, 14));
						Image image = GlobalMembers.gApp.mShrunkenGems[thePiece.mColor, val];
						g.SetScale(1f, 1f, 0f, 0f);
						GlobalMembers.gGR.DrawImage(g, image, GlobalMembers.S(num5) - (image.mWidth - GlobalMembers.S(100)) / 2, GlobalMembers.S(num6) - (image.mHeight - GlobalMembers.S(100)) / 2);
					}
					else
					{
						GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_GEMS_SHADOWED, GlobalMembers.S(num5), GlobalMembers.S(num6), thePiece.mColor);
					}
				}
				else
				{
					new Color(255, 255, 255);
					new Color(192, 192, 192);
					new Color(32, 192, 32);
					new Color(224, 192, 32);
					new Color(255, 255, 255);
					new Color(255, 160, 32);
					new Color(255, 255, 255);
					Image imageById = GlobalMembersResourcesWP.GetImageById(802 + thePiece.mColor);
					float num8 = thePiece.mRotPct * (float)imageById.GetCelCount();
					Rect celRect = imageById.GetCelRect((int)num8);
					Rect celRect2 = imageById.GetCelRect(((int)num8 + 1) % imageById.GetCelCount());
					float[] array = new float[4]
					{
						(float)(celRect2.mX - celRect.mX) / (float)imageById.mWidth,
						(float)(celRect2.mY - celRect.mY) / (float)imageById.mHeight,
						num8 - (float)(int)num8,
						0f
					};
					if (imageById.mAtlasImage != null)
					{
						array[0] /= imageById.mAtlasImage.mWidth;
						array[1] /= imageById.mAtlasImage.mHeight;
					}
					GlobalMembers.gGR.DrawImageCel(g, imageById, GlobalMembers.S(num5), GlobalMembers.S(num6), (int)num8);
				}
				g.SetColorizeImages(false);
			}
			if (thePiece.IsFlagSet(8192u))
			{
				g.PushState();
				g.SetColorizeImages(true);
				g.SetColor(Color.FAlpha((float)((GlobalMembers.M(0.25) * (double)mBoostShowPct + (double)GlobalMembers.M(0.75f)) * (double)GetPieceAlpha())));
				float num9 = (float)(GlobalMembers.M(1.0) + (double)mBoostShowPct * (double)GlobalMembers.MS(0.25f));
				Utils.PushScale(g, num9, num9, GlobalMembers.S(thePiece.CX()), GlobalMembers.S(thePiece.CY()));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_GREENQUESTION, GlobalMembers.S(num5) + GlobalMembers.MS(34), GlobalMembers.S(num6) + GlobalMembers.MS(17));
				Utils.PopScale(g);
				g.PopState();
			}
			if (thePiece.IsFlagSet(512u) && !thePiece.IsShrinking() && thePiece.mOverlay != null && (thePiece.mSpinFrame < 5f || thePiece.mSpinFrame > 15f))
			{
				float num10 = thePiece.mSpinFrame * (float)Math.PI * 2f / 20f;
				float num11 = (float)((double)thePiece.mScale * (double)GlobalMembers.M(0.8f) * (1.0 + (double)thePiece.mOverlayBulge));
				int num12 = (int)((float)thePiece.mOverlay.mWidth * num11);
				num12 = (int)((float)num12 * (float)Math.Cos(num10));
				int num13 = (int)((float)thePiece.mOverlay.mHeight * num11);
				g.SetDrawMode(Graphics.DrawMode.Normal);
				g.SetColorizeImages(true);
				int num14 = (int)((double)(GlobalMembers.S(num5 + 50) - num12 / 2) + (double)thePiece.mScale * (double)GlobalMembers.MS(0));
				int theY = (int)((float)GlobalMembers.S(num6 + 50) - (float)num13 * GlobalMembers.M(0.65f));
				int num15 = (int)((double)GlobalMembers.M(255) * (double)thePiece.mAlpha);
				g.SetColor(Color.White);
				num14 += (int)((float)Math.Sin(num10) * 100f / 2f);
				int num16 = (int)((double)(float)num15 * (double)thePiece.mOverlayCurve);
				Image iMAGE_BOMBGLOWS_DANGERGLOW = GlobalMembersResourcesWP.IMAGE_BOMBGLOWS_DANGERGLOW;
				int num17 = (int)((float)iMAGE_BOMBGLOWS_DANGERGLOW.GetCelWidth() * num11 * 2f);
				int num18 = (int)((float)iMAGE_BOMBGLOWS_DANGERGLOW.GetCelHeight() * num11 * 2f);
				int theX = GlobalMembers.S(num3 + 50) - num17 / 2;
				int theY2 = GlobalMembers.S(num4 + 50) - num18 / 2;
				g.SetColor(new Color(255, 255, 255, (int)((double)((float)num16 * GlobalMembers.M(1f)) * (double)thePiece.mAlpha)));
				g.DrawImageCel(iMAGE_BOMBGLOWS_DANGERGLOW, new Rect(theX, theY2, num17, num18), thePiece.mColor);
				g.SetColor(new Color(255, 255, 255, (int)((double)num16 * (double)thePiece.mAlpha)));
				g.DrawImage(thePiece.mOverlayGlow, num14, theY, num12, num13);
				num15 = 255 - num16;
				g.SetColor(new Color(255, 255, 255, (int)((double)num15 * (double)thePiece.mAlpha)));
				g.DrawImage(thePiece.mOverlay, num14, theY, num12, num13);
			}
			if (thePiece.mHidePct > 0f)
			{
				float num19 = 0.15f + thePiece.mHidePct * 0.85f;
				g.SetColor(new Color(128, 128, 128, (int)(num19 * 255f)));
				g.FillRect(GlobalMembers.S(num3) + 1, GlobalMembers.S(num4) + 1, GlobalMembers.S(100) - 2, GlobalMembers.S(100) - 2);
			}
			if ((double)thePiece.mSelectorAlpha != 0.0)
			{
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, (int)(255.0 * (double)thePiece.mSelectorAlpha * (double)GetPieceAlpha())));
				GlobalMembers.gGR.DrawImage(g, GlobalMembersResourcesWP.IMAGE_SELECTOR, GlobalMembers.S(num3), GlobalMembers.S(num4));
			}
			if (num != 1f)
			{
				g.SetScale(1f, 1f, GlobalMembers.S(num3 + 50), GlobalMembers.S(num4 + 50));
			}
		}

		public void DrawStandardPieces(Graphics g, Piece[] pPieces, int numPieces)
		{
			DrawStandardPieces(g, pPieces, numPieces, 1f);
		}

		public void DrawStandardPieces(Graphics g, Piece[] pPieces, int numPieces, float theScale)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Piece piece = null;
			Graphics3D graphics3D = g.Get3D();
			for (int i = 0; i < numPieces; i++)
			{
				Piece piece2;
				if ((piece2 = pPieces[i]) == null)
				{
					continue;
				}
				float num4 = ((mHyperspace == null || piece2.mColor < 0) ? ((float)((double)piece2.mAlpha * (double)GetPieceAlpha())) : ((float)((double)piece2.mAlpha * (double)mHyperspace.GetPieceAlpha())));
				if (!(num4 < 0.01f))
				{
					float scale = (float)((double)piece2.mScale * (double)theScale);
					int offsX = (int)(piece2.GetScreenX() + piece2.mShakeOfsX);
					int offsY = (int)(piece2.GetScreenY() + piece2.mShakeOfsY);
					if (piece2.IsFlagSet(2u))
					{
						DSP_pHyperCubes[num2].piece = piece2;
						DSP_pHyperCubes[num2].alpha = num4;
						DSP_pHyperCubes[num2].scale = scale;
						DSP_pHyperCubes[num2].offsX = offsX;
						DSP_pHyperCubes[num2].offsY = offsY;
						num2++;
					}
					else if (piece2.IsFlagSet(128u))
					{
						DSP_pButterflies[num3].piece = piece2;
						DSP_pButterflies[num3].alpha = num4;
						DSP_pButterflies[num3].scale = scale;
						DSP_pButterflies[num3].offsX = offsX;
						DSP_pButterflies[num3].offsY = offsY;
						num3++;
					}
					else if (piece2.mColor >= 0)
					{
						DSP_pNormalPieces[num].piece = piece2;
						DSP_pNormalPieces[num].alpha = num4;
						DSP_pNormalPieces[num].scale = scale;
						DSP_pNormalPieces[num].offsX = offsX;
						DSP_pNormalPieces[num].offsY = offsY;
						num++;
					}
					if ((double)piece2.mSelectorAlpha != 0.0)
					{
						piece = piece2;
					}
				}
			}
			g.SetColorizeImages(true);
			int num5 = (int)GlobalMembers.gApp.mUpdateCount / GlobalMembers.M(10);
			for (int i = 0; i < num; i++)
			{
				Piece piece2 = DSP_pNormalPieces[i].piece;
				float scale = DSP_pNormalPieces[i].scale;
				float num4 = DSP_pNormalPieces[i].alpha;
				int offsX = DSP_pNormalPieces[i].offsX;
				int offsY = DSP_pNormalPieces[i].offsY;
				if (scale != 1f)
				{
					g.SetScale(scale, scale, GlobalMembers.S(piece2.GetScreenX() + 50f), GlobalMembers.S(piece2.GetScreenY() + 50f));
				}
				g.SetColor(new Color(255, 255, 255, (int)(255f * num4)));
				if (CanBakeShadow(piece2))
				{
					if (graphics3D == null)
					{
						float num6 = scale;
						if (num6 > 1f)
						{
							num6 = (num6 - 1f) * 2f + 1f;
						}
						int val = (int)((0f - num6 + 2f) * 16f / 2f - 1f);
						val = Math.Max(0, Math.Min(val, 14));
						Image image = GlobalMembers.gApp.mShrunkenGems[piece2.mColor, val];
						g.SetScale(1f, 1f, 0f, 0f);
						GlobalMembers.gGR.DrawImage(g, image, GlobalMembers.S(offsX) - (image.mWidth - GlobalMembers.S(100)) / 2, GlobalMembers.S(offsY) - (image.mHeight - GlobalMembers.S(100)) / 2);
					}
					else
					{
						GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_GEMS_SHADOWED, GlobalMembers.S(offsX), GlobalMembers.S(offsY), piece2.mColor);
					}
				}
				else
				{
					Image imageById = GlobalMembersResourcesWP.GetImageById(802 + piece2.mColor);
					float num7 = piece2.mRotPct * (float)imageById.GetCelCount();
					GlobalMembers.gGR.DrawImageCel(g, imageById, GlobalMembers.S(offsX), GlobalMembers.S(offsY), (int)num7);
				}
				if (scale != 1f)
				{
					g.SetScale(1f, 1f, GlobalMembers.S(piece2.GetScreenX() + 50f), GlobalMembers.S(piece2.GetScreenY() + 50f));
				}
			}
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					Piece piece2 = DSP_pHyperCubes[i].piece;
					DrawHypercube(g, piece2);
				}
			}
			if (num3 > 0)
			{
				g.SetColor(Color.White);
				for (int j = 0; j < num3; j++)
				{
					DrawButterfly(g, DSP_pButterflies[j].offsX, DSP_pButterflies[j].offsY, DSP_pButterflies[j].piece, DSP_pButterflies[j].scale);
				}
			}
			if (piece != null)
			{
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, (int)(255.0 * (double)piece.mSelectorAlpha * (double)GetPieceAlpha())));
				GlobalMembers.gGR.DrawImage(g, GlobalMembersResourcesWP.IMAGE_SELECTOR, (int)GlobalMembers.S(piece.GetScreenX()), (int)GlobalMembers.S(piece.GetScreenY()));
			}
			g.SetColorizeImages(false);
		}

		public void DrawShadowPieces(Graphics g, Piece[] pPieces, int numPieces)
		{
			DrawShadowPieces(g, pPieces, numPieces, 1f);
		}

		public void DrawShadowPieces(Graphics g, Piece[] pPieces, int numPieces, float theScale)
		{
			if (pPieces == null || numPieces == 0)
			{
				return;
			}
			bool colorizeImages = g.GetColorizeImages();
			g.SetColorizeImages(true);
			for (int i = 0; i < numPieces; i++)
			{
				Piece piece;
				if ((piece = pPieces[i]) != null)
				{
					int theNum = (int)(piece.GetScreenX() + piece.mShakeOfsX);
					int theNum2 = (int)(piece.GetScreenY() + piece.mShakeOfsY);
					float num = (float)(double)piece.mScale;
					if (num != 1f)
					{
						g.SetScale(num, num, GlobalMembers.S(piece.GetScreenX() + 50f), GlobalMembers.S(piece.GetScreenY() + 50f));
					}
					float num2 = ((mHyperspace == null || piece.mColor < 0) ? ((float)((double)piece.mAlpha * (double)GetPieceAlpha())) : ((float)((double)piece.mAlpha * (double)mHyperspace.GetPieceAlpha())));
					g.SetColor(new Color(255, 255, 255, (int)(255f * num2)));
					if (!piece.IsFlagSet(128u) && ((piece.mColor > -1 && piece.mColor < 7) || piece.IsFlagSet(4u)))
					{
						int theCel = (int)Math.Min(piece.mRotPct * (float)GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount(), GlobalMembersResourcesWP.IMAGE_GEMS_RED.GetCelCount() - 1);
						GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.GetImageById(809 + piece.mColor), GlobalMembers.S(theNum), GlobalMembers.S(theNum2), theCel);
					}
					if (num != 1f)
					{
						g.SetScale(1f, 1f, GlobalMembers.S(piece.GetScreenX() + 50f), GlobalMembers.S(piece.GetScreenY() + 50f));
					}
				}
			}
			g.SetColorizeImages(colorizeImages);
		}

		public virtual void DrawPieceText(Graphics g, Piece thePiece)
		{
			DrawPieceText(g, thePiece, 1f);
		}

		public virtual void DrawPieceText(Graphics g, Piece thePiece, float theScale)
		{
		}

		public virtual void DrawFrame(Graphics g)
		{
			DrawTopFrame(g);
			DrawBottomFrame(g);
		}

		public virtual void DrawTopFrame(Graphics g)
		{
			if (WantTopFrame())
			{
				if (WantCountdownBar() || WantTopLevelBar())
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_PROGRESS_BAR_FRAME_ID) + (float)mTransBoardOffsetX), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_PROGRESS_BAR_FRAME_ID) - (float)mTransBoardOffsetY));
				}
				else
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME, (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME))) + (float)mTransBoardOffsetX), (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME))) - (float)mTransBoardOffsetY));
				}
			}
		}

		public virtual void DrawBottomFrame(Graphics g)
		{
			if (WantBottomFrame())
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME, (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME_ID)) + (float)mTransBoardOffsetX), (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME_ID)) - (float)mTransBoardOffsetY));
			}
		}

		public virtual void DrawLevelBar(Graphics g)
		{
			g.PushState();
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColorizeImages(true);
			float num = (float)Math.Pow(GetBoardAlpha(), 4.0);
			g.SetColor(new Color(255, 255, 255, (int)(GetBoardAlpha() * (float)GlobalMembers.M(255))));
			Image theImage = null;
			int num2 = 0;
			int num3 = 0;
			Rect levelBarRect = GetLevelBarRect();
			levelBarRect.mX += GlobalMembers.S(mTransBoardOffsetX);
			levelBarRect.mY -= GlobalMembers.S(mTransBoardOffsetY);
			if (WantTopLevelBar())
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_BACK, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_PROGRESS_BAR_BACK_ID) + (float)mTransBoardOffsetX), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_PROGRESS_BAR_BACK_ID) - (float)mTransBoardOffsetY));
			}
			g.SetColor(new Color(GlobalMembers.M(53), GlobalMembers.M(104), GlobalMembers.M(238), (int)(num * (float)GlobalMembers.M(255))));
			if (WantWarningGlow())
			{
				Color warningGlowColor = GetWarningGlowColor();
				if (warningGlowColor.mAlpha > 0)
				{
					Color color = g.GetColor();
					g.SetDrawMode(Graphics.DrawMode.Additive);
					g.SetColor(warningGlowColor);
					Utils.DrawImageCentered(g, theImage, num2, num3);
					g.SetDrawMode(Graphics.DrawMode.Normal);
					g.SetColor(color);
				}
			}
			levelBarRect.mWidth = (int)(mLevelBarPct * (float)levelBarRect.mWidth + (float)mLevelBarSizeBias);
			g.FillRect(levelBarRect);
			if ((double)mLevelBarBonusAlpha > 0.0)
			{
				Rect levelBarRect2 = GetLevelBarRect();
				levelBarRect2.mWidth = (int)((float)levelBarRect2.mWidth * GetLevelPct());
				levelBarRect2.mX += mTransBoardOffsetX;
				levelBarRect2.mY -= mTransBoardOffsetY;
				g.SetColor(new Color(GlobalMembers.M(240), GlobalMembers.M(255), 200, (int)((double)mLevelBarBonusAlpha * (double)GlobalMembers.M(255))));
				g.FillRect(levelBarRect2);
			}
			Graphics3D graphics3D = g.Get3D();
			SexyTransform2D mDrawTransform = mLevelBarPIEffect.mDrawTransform;
			Rect mClipRect = g.mClipRect;
			if (graphics3D != null)
			{
				levelBarRect.Scale(mScale, mScale, GlobalMembers.S(960), GlobalMembers.S(600));
				mLevelBarPIEffect.mDrawTransform.Translate(GlobalMembers.S(-960), GlobalMembers.S(-600));
				mLevelBarPIEffect.mDrawTransform.Scale((float)(double)mScale, (float)(double)mScale);
				mLevelBarPIEffect.mDrawTransform.Translate(GlobalMembers.S(960), GlobalMembers.S(600));
			}
			int num4 = (int)((double)mAlphaCurve * (double)GetAlpha() * 255.0);
			if (num4 == 255)
			{
				g.SetClipRect(levelBarRect);
				mLevelBarPIEffect.mColor = new Color(255, 255, 255, (int)(num * (float)GlobalMembers.M(255)));
				mLevelBarPIEffect.Draw(g);
				mLevelBarPIEffect.mDrawTransform = mDrawTransform;
				g.SetColor(new Color(255, 255, 255, (int)(num * (float)GlobalMembers.M(150))));
				if (!mWantLevelup && mHyperspace == null)
				{
					int num5 = GlobalMembersResourcesWP.IMAGE_LEVELBAR_ENDPIECE.mWidth / 2;
					g.SetClipRect(levelBarRect.mX, levelBarRect.mY, levelBarRect.mWidth + num5, levelBarRect.mHeight);
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_LEVELBAR_ENDPIECE, levelBarRect.mX + levelBarRect.mWidth - num5, levelBarRect.mY);
				}
			}
			g.SetClipRect(mClipRect);
			g.PopState();
		}

		public virtual void DrawCountdownBar(Graphics g)
		{
			g.SetColorizeImages(true);
			float num = (float)Math.Pow(GetBoardAlpha(), 4.0);
			g.SetColor(new Color(255, 255, 255, (int)(GetBoardAlpha() * (float)GlobalMembers.M(255))));
			Rect countdownBarRect = GetCountdownBarRect();
			int boardCenterX = GetBoardCenterX();
			int theNum = GetBoardY() + 800 + GlobalMembers.M(30);
			Image iMAGE_INGAMEUI_PROGRESS_BAR_BACK = GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_BACK;
			Utils.DrawImageCentered(g, iMAGE_INGAMEUI_PROGRESS_BAR_BACK, GlobalMembers.S(boardCenterX), GlobalMembers.S(theNum));
			g.SetColor(new Color(GlobalMembers.M(64), GlobalMembers.M(32), GlobalMembers.M(8), (int)(num * (float)GlobalMembers.M(255))));
			if (WantWarningGlow())
			{
				Color warningGlowColor = GetWarningGlowColor();
				if (warningGlowColor.mAlpha > 0)
				{
					Color color = g.GetColor();
					g.SetDrawMode(Graphics.DrawMode.Additive);
					g.SetColor(warningGlowColor);
					Utils.DrawImageCentered(g, iMAGE_INGAMEUI_PROGRESS_BAR_BACK, GlobalMembers.S(boardCenterX), GlobalMembers.S(theNum));
					g.SetDrawMode(Graphics.DrawMode.Normal);
					g.SetColor(color);
				}
			}
			countdownBarRect.mWidth = (int)(mCountdownBarPct * (float)countdownBarRect.mWidth + (float)mLevelBarSizeBias);
			g.FillRect(countdownBarRect);
			if ((double)mLevelBarBonusAlpha > 0.0)
			{
				Rect countdownBarRect2 = GetCountdownBarRect();
				countdownBarRect2.mWidth = (int)((float)countdownBarRect2.mWidth * GetLevelPct());
				g.SetColor(new Color(GlobalMembers.M(240), GlobalMembers.M(255), 200, (int)((double)mLevelBarBonusAlpha * (double)GlobalMembers.M(255))));
				g.FillRect(countdownBarRect2);
			}
			Graphics3D graphics3D = g.Get3D();
			SexyTransform2D mDrawTransform = mCountdownBarPIEffect.mDrawTransform;
			Rect mClipRect = g.mClipRect;
			if (graphics3D != null)
			{
				countdownBarRect.Scale(mScale, mScale, GlobalMembers.S(960), GlobalMembers.S(600));
				mCountdownBarPIEffect.mDrawTransform.Translate(GlobalMembers.S(-960), GlobalMembers.S(-600));
				mCountdownBarPIEffect.mDrawTransform.Scale((float)(double)mScale, (float)(double)mScale);
				mCountdownBarPIEffect.mDrawTransform.Translate(GlobalMembers.S(960), GlobalMembers.S(600));
			}
			g.SetClipRect(countdownBarRect);
			mCountdownBarPIEffect.mColor = new Color(255, 255, 255, (int)(num * (float)GlobalMembers.M(255)));
			mCountdownBarPIEffect.Draw(g);
			mCountdownBarPIEffect.mDrawTransform = mDrawTransform;
			g.SetColor(Color.White);
			g.mClipRect = mClipRect;
		}

		public virtual void DrawCountPopups(Graphics g)
		{
		}

		public void DrawComplements(Graphics g)
		{
			if (!mSuspendingGame)
			{
				g.SetDrawMode(Graphics.DrawMode.Normal);
				if ((double)mComplementAlpha != 0.0)
				{
					g.SetColorizeImages(true);
					g.SetColor(new Color(255, 255, 255, (int)(255.0 * (double)mComplementAlpha * (double)GetPieceAlpha())));
					g.SetFont(GlobalMembersResources.FONT_HUGE);
					int num = g.StringWidth(GlobalMembers.gComplementStr[mComplementNum]);
					Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Bej3Widget.COLOR_CRYSTALBALL_FONT);
					g.DrawString(GlobalMembers.gComplementStr[mComplementNum], (int)(GlobalMembers.S((float)GetBoardCenterX()) - (float)(num / 2)), GlobalMembers.S(GetBoardCenterY()) + g.GetFont().GetAscent() / 2);
				}
			}
		}

		public void DrawPointMultiplier(Graphics g, bool front)
		{
			if (!mShowPointMultiplier)
			{
				return;
			}
			g.SetDrawMode(Graphics.DrawMode.Normal);
			Rect mClipRect = g.mClipRect;
			Graphics3D graphics3D = g.Get3D();
			SexyTransform2D theTransform = default(SexyTransform2D);
			if (graphics3D != null && mIsWholeGameReplay)
			{
				SexyTransform2D theTransform2 = new SexyTransform2D(true);
				graphics3D.PopTransform(ref theTransform2);
				theTransform.CopyFrom(theTransform2);
			}
			Image multiplierImage = GetMultiplierImage();
			int multiplierImageX = GetMultiplierImageX();
			int multiplierImageY = GetMultiplierImageY();
			int num = multiplierImageX + multiplierImage.GetCelWidth() / 2;
			int num2 = multiplierImageY + ConstantsWP.BOARD_MULTIPLIER_Y;
			new Point(num + GlobalMembers.MS(-3), num2 + GlobalMembers.MS(-2));
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Bej3Widget.COLOR_SUBHEADING_1_STROKE);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Bej3Widget.COLOR_SUBHEADING_1_FILL);
			if (!front)
			{
				g.DrawImage(multiplierImage, multiplierImageX, multiplierImageY);
				int num3 = 0;
				if ((double)mPrevPointMultAlpha != 0.0)
				{
					Color color = mPrevPointMultAlpha;
					num3 = color.mAlpha;
					g.SetColor(color);
					g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
					string theString = string.Format(GlobalMembers._ID("x{0}", 144), mPointMultiplier - 1);
					g.DrawString(theString, (int)((float)num - (float)g.StringWidth(theString) * 0.5f), num2);
				}
				if ((double)mPointMultPosPct == 1.0 && (mPointMultiplier >= 1 || (mTimeExpired && mPointMultiplier > 0)))
				{
					g.SetColor(new Color(255, 255, 255, 255 - num3));
					g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
					string theString2 = string.Format(GlobalMembers._ID("x{0}", 145), mPointMultiplier);
					g.DrawString(theString2, (int)((double)((float)num - (float)g.StringWidth(theString2) * 0.5f) + (double)GlobalMembers.S(-700) * (double)mSlideUIPct), num2);
				}
				return;
			}
			if (mPointMultiplier > 1 && (mPointMultTextMorph.IsDoingCurve() || mPointMultAlpha.IsDoingCurve()))
			{
				string theString3 = string.Format(GlobalMembers._ID("x{0}", 146), mPointMultiplier);
				Color color2 = mPointMultColor;
				color2.mRed = (int)SexyMath.Lerp(mPointMultColor.mRed, 255.0, mPointMultAlpha);
				color2.mGreen = (int)SexyMath.Lerp(mPointMultColor.mGreen, 255.0, mPointMultAlpha);
				color2.mBlue = (int)SexyMath.Lerp(mPointMultColor.mBlue, 255.0, mPointMultAlpha);
				color2.mAlpha = (int)((double)(255f * GetPieceAlpha()) * (1.0 - (double)mSlideUIPct) * (double)mPointMultAlpha * (1.0 - (double)mPointMultTextMorph) * 0.5);
				g.SetColor(color2);
				float num4 = (float)(double)mPointMultScale;
				g.SetScale(num4, num4, num, num2 + ConstantsWP.BOARD_MULTIPLIER_LARGE_Y);
				g.SetFont(GlobalMembersResources.FONT_HEADER);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HEADER, 0, Color.White);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HEADER, 1, Color.White);
				g.DrawString(theString3, (int)((float)num - (float)g.StringWidth(theString3) * 0.5f), (int)((float)num2 + (float)ConstantsWP.BOARD_MULTIPLIER_LARGE_Y_OFFSET * num4 * ConstantsWP.BOARD_MULTIPLIER_LARGE_Y_SCALE_OFFSET));
				g.SetScale(1f, 1f, 0f, 0f);
			}
			if (graphics3D != null && mIsWholeGameReplay)
			{
				graphics3D.PushTransform(theTransform);
			}
			g.mClipRect = mClipRect;
		}

		public void DrawReplayOverlay(Graphics g)
		{
			bool flag = false;
			g.SetDrawMode(Graphics.DrawMode.Normal);
			if (mRewinding)
			{
				g.PushState();
				g.Translate((int)(0f - g.mTransX), (int)(0f - g.mTransY));
				GlobalMembers.gGR.ExecuteOperations(g, GlobalMembers.gGR.GetLastOperationTimestamp());
				g.PopState();
				GlobalMembers.gGR.ClearOperationsFrom(mPlaybackTimestamp);
				flag = true;
			}
			if (mInReplay)
			{
				flag = true;
			}
			if (!flag || mIsOneMoveReplay || mIsWholeGameReplay)
			{
				return;
			}
			MTRand.SetRandAllowed(true);
			if (mRewinding)
			{
				g.SetColor(new Color(GlobalMembers.M(64), GlobalMembers.M(64), GlobalMembers.M(64), GlobalMembers.M(128)));
				g.FillRect(0, 0, mWidth, mHeight);
				g.SetDrawMode(Graphics.DrawMode.Normal);
				for (int i = 0; i < (int)mRewindRand.Next() % GlobalMembers.M(10) + GlobalMembers.M(6); i++)
				{
					g.SetColor(new Color(255, 255, 255, GlobalMembers.M(64)));
					g.FillRect(0, (int)(mRewindRand.Next() % mHeight), mWidth, (int)(mRewindRand.Next() % GlobalMembers.M(2) + GlobalMembers.M(1)));
				}
			}
			else if (mInReplay)
			{
				g.SetColor(new Color(GlobalMembers.M(64), GlobalMembers.M(64), GlobalMembers.M(64), GlobalMembers.M(70)));
				g.FillRect(0, 0, mWidth, mHeight);
				for (int j = 0; j < (int)mRewindRand.Next() % GlobalMembers.M(2) + GlobalMembers.M(3); j++)
				{
					g.SetColor(new Color(150, 150, 150, GlobalMembers.M(50)));
					g.FillRect(0, (int)(mRewindRand.Next() % mHeight), mWidth, (int)(mRewindRand.Next() % GlobalMembers.M(2) + GlobalMembers.M(1)));
				}
			}
			MTRand.SetRandAllowed(false);
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			DrawOverlayPreAnnounce(g, thePriority);
			DrawOverlayPostAnnounce(g, thePriority);
		}

		public virtual void DrawOverlayPreAnnounce(Graphics g, int thePriority)
		{
			g.SetDrawMode(Graphics.DrawMode.Normal);
			float num = (float)(double)mAlphaCurve;
			g.SetColor(new Color(255, 255, 255, (int)(255f * num)));
			if (num != 1f)
			{
				g.PushColorMult();
			}
			DrawPointMultiplier(g, false);
			if ((double)mSlideUIPct >= 1.0 && mGameOverCount > 0)
			{
				if ((double)mTutorialPieceIrisPct != 0.0 && mTimeExpired)
				{
					DrawIris(g, GlobalMembers.MS(800), GlobalMembers.MS(100));
				}
				if (GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_BADGEMENU && GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_GAMEDETAILMENU && mHyperspace == null)
				{
					g.PushState();
					mPostFXManager.Draw(g);
					g.PopState();
				}
				return;
			}
			if (mSpeedFireBarPIEffect[0] != null)
			{
				mSpeedFireBarPIEffect[0].Draw(g);
			}
			if (mSpeedFireBarPIEffect[1] != null)
			{
				mSpeedFireBarPIEffect[1].Draw(g);
			}
			if (mBoardDarken > 0f)
			{
				Rect mScreenBounds = GlobalMembers.gApp.mScreenBounds;
				mScreenBounds.Offset(-mX, -mY);
				g.SetClipRect(mScreenBounds);
				g.SetColor(new Color(0, 0, 0, (int)(GetBoardAlpha() * mBoardDarken * (float)GlobalMembers.M(128))));
				g.FillRect(-mX, -mY, mWidth, mHeight);
				g.SetColor(Color.White);
				DrawPieces(g, true);
			}
			if (GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_BADGEMENU && GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_GAMEDETAILMENU && mHyperspace == null && mUpdateCnt > 130)
			{
				g.PushState();
				mPostFXManager.Draw(g);
				g.PopState();
			}
			Graphics3D graphics3D = g.Get3D();
			DrawLightning(g);
			if (WantsHideOnPause())
			{
				if (mVisPausePct > 0f)
				{
					DrawPauseText(g, mVisPausePct);
				}
				else if (mSuspendingGame)
				{
					DrawPauseText(g, 1f);
				}
			}
			mWidgetManager.FlushDeferredOverlayWidgets(1);
			DrawCountPopups(g);
			DrawComplements(g);
			DrawPointMultiplier(g, true);
			DrawReplayOverlay(g);
			Piece tutorialIrisPiece = GetTutorialIrisPiece();
			if (tutorialIrisPiece != null)
			{
				DrawIris(g, (int)GlobalMembers.S(tutorialIrisPiece.CX()), (int)GlobalMembers.S(tutorialIrisPiece.CY()));
			}
			if (mGameOverPiece != null)
			{
				if (graphics3D != null)
				{
					graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_TEST_INSIDE);
					g.SetColor(Color.White);
					if (mShowBoard)
					{
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_BOOM_FTOP_WIDGET, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_BOOM_FTOP_WIDGET))), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_BOOM_FTOP_WIDGET))));
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_BOOM_FBOTTOM_WIDGET, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_BOOM_FBOTTOM_WIDGET))), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_BOOM_FBOTTOM_WIDGET))));
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_BOOM_BOARD, GlobalMembers.S(GetColScreenX(0) + GlobalMembers.M(0)), GlobalMembers.S(GetRowScreenY(0) + GlobalMembers.M(-20)));
						g.PushState();
						g.ClipRect(GlobalMembers.S(GetColScreenX(0) + GlobalMembers.M(0)), GlobalMembers.S(GetRowScreenY(0) + GlobalMembers.M(-20)), GlobalMembers.S(800), GlobalMembers.S(800));
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_BOOM_CRATER, (int)(GlobalMembers.S(mGameOverPiece.CX()) - (float)(GlobalMembersResourcesWP.IMAGE_BOOM_CRATER.mWidth / 2)), (int)(GlobalMembers.S(mGameOverPiece.CY()) - (float)(GlobalMembersResourcesWP.IMAGE_BOOM_CRATER.mHeight / 2)));
						g.PopState();
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_BOOM_FGRIDBAR_TOP, GlobalMembers.S(GetColScreenX(0)) + GlobalMembers.MS(-16), GlobalMembers.S(GetRowScreenY(0)) + GlobalMembers.MS(-33));
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_BOOM_FGRIDBAR_BOT, GlobalMembers.S(GetColScreenX(0)) + GlobalMembers.MS(-7), GlobalMembers.S(GetRowScreenY(8)) + GlobalMembers.MS(-7));
					}
				}
				graphics3D?.SetMasking(Graphics3D.EMaskMode.MASKMODE_NONE);
				if (mShowBoard)
				{
					DrawPiece(g, mGameOverPiece, (float)(double)mGameOverPieceScale);
					DrawPieceText(g, mGameOverPiece, (float)(double)mGameOverPieceScale);
				}
				if ((double)mGameOverPieceGlow > 0.0 && mShowBoard)
				{
					float num2 = (float)((double)mGameOverPieceScale * GlobalMembers.M(0.5));
					g.SetColorizeImages(true);
					g.SetColor(mGameOverPieceGlow);
					Transform transform = new Transform();
					transform.Scale(num2, num2);
					g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_ANGRYBOMB, transform, GlobalMembers.S(mGameOverPiece.GetScreenX() + 50f), GlobalMembers.S(mGameOverPiece.GetScreenY() + 50f));
					g.SetColorizeImages(false);
				}
				g.SetColorizeImages(true);
				g.SetColor(mNovaAlpha);
				Transform transform2 = new Transform();
				transform2.Scale((float)(double)mNovaRadius, (float)(double)mNovaRadius);
				g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_BOOM_NOVA, transform2, GlobalMembers.S(mGameOverPiece.CX()), GlobalMembers.S(mGameOverPiece.CY()));
			}
			g.SetDrawMode(Graphics.DrawMode.Normal);
		}

		public virtual void DrawOverlayPostAnnounce(Graphics g, int thePriority)
		{
			g.SetDrawMode(Graphics.DrawMode.Normal);
			if (mAnnouncements.Count > 0)
			{
				mAnnouncements[0].Draw(g);
			}
			if (mGameOverPiece != null && (double)mNukeRadius > 0.0)
			{
				Transform transform = new Transform();
				g.SetColor(mNukeAlpha);
				transform.Reset();
				transform.Scale((float)(double)mNukeRadius, (float)(double)mNukeRadius);
				g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_BOOM_NUKE, transform, GlobalMembers.S(mGameOverPiece.CX()), GlobalMembers.S(mGameOverPiece.CY()));
			}
			if ((double)mReplayFadeout != 0.0)
			{
				int num = 80;
				g.SetColor(mReplayFadeout);
				g.FillRect(GlobalMembers.S(-160), 0, GlobalMembers.S(1920), GlobalMembers.S(1200) + num);
			}
			if (mRestartPrevImage != null)
			{
				DeviceImage deviceImage = GlobalMembers.gApp.mRestartRT.Lock(GlobalMembers.gApp.mScreenBounds.mWidth, GlobalMembers.gApp.mScreenBounds.mHeight);
				g.SetColor(Color.White);
				int val = (int)((double)GlobalMembers.gApp.mScreenBounds.mHeight * (double)mRestartPct);
				int num2 = Math.Max(0, Math.Min(GlobalMembers.gApp.mScreenBounds.mHeight, val));
				Rect theSrcRect = new Rect(0, num2, GlobalMembers.gApp.mScreenBounds.mWidth, GlobalMembers.gApp.mScreenBounds.mHeight - num2);
				Rect theDestRect = new Rect(GlobalMembers.gApp.mScreenBounds.mX, num2, GlobalMembers.gApp.mScreenBounds.mWidth, GlobalMembers.gApp.mScreenBounds.mHeight - num2);
				if (mRestartPrevImage == deviceImage)
				{
					g.DrawImage(mRestartPrevImage, theDestRect, theSrcRect);
				}
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.SetDrawMode(Graphics.DrawMode.Normal);
				GlobalMembers.gApp.mRestartRT.Unlock();
			}
			if (mMessager != null)
			{
				mMessager.Draw(g, GlobalMembers.MS(10), mMessager.mFont.mHeight * (mMessager.mMessages.Count + 1) + GlobalMembers.MS(10));
			}
			if ((double)mAlphaCurve != 1.0)
			{
				g.PopColorMult();
			}
		}

		public virtual void DrawGameElements(Graphics g)
		{
			if (!mContentLoaded)
			{
				return;
			}
			Rect mClipRect = g.mClipRect;
			if (mHyperspace != null)
			{
				ClipCollapsingBoard(g);
			}
			g.SetDrawMode(Graphics.DrawMode.Normal);
			GlobalMembers.gFrameLightCount = 0;
			g.SetColorizeImages(true);
			Color color = new Color(255, 255, 255, (int)((double)(255f * GetAlpha()) * (double)mSideAlpha));
			g.SetColor(color);
			if (AllowSpeedBonus() && (double)mSpeedBonusDisp != 0.0)
			{
				g.SetColor(mSpeedBonusDisp);
				ImageFont imageFont = (ImageFont)GlobalMembersResources.FONT_SUBHEADER;
				g.SetFont(imageFont);
				string theString = GlobalMembers._ID("SPEED", 3234) + " " + string.Format(GlobalMembers._ID("+{0}", 3235), (int)((float)Math.Min(200, ((mSpeedBonusCount == 0 ? mSpeedBonusLastCount : mSpeedBonusCount) + 1) * 20) * GetModePointMultiplier()));
				int num = g.StringWidth(theString);
				float num2 = (float)(double)mSpeedBonusPointsScale;
				g.SetScale(num2, num2, ConstantsWP.SPEEDBOARD_SPEED_BONUS_SCALE_X + num / 2, ConstantsWP.SPEEDBOARD_SPEED_BONUS_SCALE_Y);
				Utils.SetFontLayerColor(imageFont, 0, new Color(0, 0, 0, 0));
				Utils.SetFontLayerColor(imageFont, 1, Color.White);
				string theString2 = string.Format("{0} CHAIN", mSpeedBonusCount == 0 ? mSpeedBonusLastCount : mSpeedBonusCount);
				g.DrawString(theString, ConstantsWP.SPEEDBOARD_SPEED_BONUS_X, ConstantsWP.SPEEDBOARD_SPEED_BONUS_Y);
				g.DrawString(theString2, ConstantsWP.SPEEDBOARD_SPEED_BONUS_X_2, ConstantsWP.SPEEDBOARD_SPEED_BONUS_Y_2);
				if (mSpeedBonusNum > 0.0)
				{
					Utils.SetFontLayerColor(imageFont, 1, new Color(14716992));
					int num3 = Math.Max(g.StringWidth(theString), g.StringWidth(theString2));
					float num4 = (float)(mSpeedBonusNum * GlobalMembers.M(1.8) + GlobalMembers.M(-0.95));
					int num5 = (int)((float)num3 * num4);
					if (num5 > 0)
					{
						if (mSpeedBonusNum < 1.0)
						{
							g.SetClipRect(0, 0, num5 + ConstantsWP.SPEEDBOARD_SPEED_BONUS_X, mHeight);
						}
						g.DrawString(theString, ConstantsWP.SPEEDBOARD_SPEED_BONUS_X, ConstantsWP.SPEEDBOARD_SPEED_BONUS_Y);
						g.DrawString(theString2, ConstantsWP.SPEEDBOARD_SPEED_BONUS_X_2, ConstantsWP.SPEEDBOARD_SPEED_BONUS_Y_2);
						g.ClearClipRect();
					}
				}
				g.SetScale(1f, 1f, 0f, 0f);
			}
			g.SetColor(color);
			float mTransX = g.mTransX;
			float mTransY = g.mTransY;
			if ((double)mSideXOff != 0.0)
			{
				g.Translate((int)((double)mSideXOff * (double)mSlideXScale), 0);
			}
			else
			{
				g.Translate((int)((double)GlobalMembers.S(1260) * (double)mSlideUIPct), 0);
			}
			DrawPieces(g, false);
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && !mSuspendingGame && (double)piece.mHintAlpha != 0.0)
				{
					g.SetColor(new Color(255, 255, 255, (int)((double)GetPieceAlpha() * (double)piece.mHintAlpha * 255.0)));
					g.SetColorizeImages(true);
					Transform transform = new Transform();
					transform.Translate(0f, (int)GlobalMembers.S(piece.mHintArrowPos));
					Point[] array2 = new Point[4]
					{
						new Point(1, 0),
						new Point(0, 0),
						new Point(0, 0),
						new Point(0, 1)
					};
					for (int k = 0; k < 4; k++)
					{
						g.DrawImageTransformF(GlobalMembersResourcesWP.IMAGE_HINTARROW, transform, GlobalMembers.S(piece.CX() + (float)array2[k].mX), GlobalMembers.S(piece.CY() + (float)array2[k].mY));
						transform.RotateDeg(90f);
					}
				}
			}
			g.SetColor(color);
			if (mIsWholeGameReplay && !GlobalMembers.gApp.Is3DAccelerated())
			{
				g.SetColor(Color.White);
				g.PushState();
				g.Translate(mTimeSlider.mX, mTimeSlider.mY);
				mTimeSlider.Draw(g);
				g.SetFont(GlobalMembersResources.FONT_HEADER);
				((ImageFont)g.mFont).PushLayerColor("OUTLINE", new Color(64, 0, 32));
				((ImageFont)g.mFont).PushLayerColor("GLOW", new Color(224, 64, 160));
				int theX = (int)(mTimeSlider.mVal * (double)(mTimeSlider.mWidth - mTimeSlider.mThumbImage.GetCelWidth()) + (double)(mTimeSlider.mThumbImage.GetCelWidth() / 2));
				int ticksLeft = GetTicksLeft();
				string theString3 = string.Format(GlobalMembers._ID("{0}:{1:d2}", 151), (ticksLeft + 99) / 100 / 60, (ticksLeft + 99) / 100 % 60);
				g.WriteString(theString3, theX, mTimeSlider.mHeight / 2 + GlobalMembers.MS(17));
				((ImageFont)g.mFont).PopLayerColor("OUTLINE");
				((ImageFont)g.mFont).PopLayerColor("GLOW");
				g.PopState();
			}
			g.mTransX = mTransX;
			g.mTransY = mTransY;
			if (mHyperspace != null)
			{
				g.SetClipRect(mClipRect);
			}
		}

		public virtual void DrawSpeedBonus(Graphics g)
		{
		}

		public void DrawDistortion(Graphics g)
		{
			mDistortionPieces.Clear();
		}

		public virtual void DrawPieces(Graphics g)
		{
			DrawPieces(g, false);
		}

		public virtual void DrawPieces(Graphics g, bool thePostFX)
		{
			int numPieces = 0;
			int numPieces2 = 0;
			int numPieces3 = 0;
			uint num = 0u;
			Graphics3D graphics3D = g.Get3D();
			graphics3D?.SetTexture(0, GlobalMembersResourcesWP.IMAGE_GEMS_RED);
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece == null || piece == mGameOverPiece || (thePostFX && !(piece.mElectrocutePercent > 0f) && (num & (1 << piece.mColor + 1)) == 0) || (thePostFX && (piece.IsFlagSet(4096u) || piece.IsFlagSet(2048u))))
				{
					continue;
				}
				if (piece.IsFlagSet(65536u))
				{
					DP_pQuestPieces[numPieces3++] = piece;
					continue;
				}
				DP_pStdPieces[numPieces++] = piece;
				if (!piece.IsFlagSet(1u) && !CanBakeShadow(piece))
				{
					DP_pShadowPieces[numPieces2++] = piece;
				}
			}
			if (!thePostFX)
			{
				if (!mSuspendingGame)
				{
					DrawShadowPieces(g, DP_pShadowPieces, numPieces2);
				}
				if (GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_BADGEMENU && GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_GAMEDETAILMENU && mHyperspace == null)
				{
					mPreFXManager.Draw(g);
				}
				g.SetDrawMode(Graphics.DrawMode.Normal);
				g.SetColorizeImages(false);
				num = uint.MaxValue;
			}
			else
			{
				for (int k = 0; k < mLightningStorms.Count; k++)
				{
					if (mLightningStorms[k].mStormType == 7)
					{
						num |= (uint)(1 << mLightningStorms[k].mColor + 1);
					}
				}
			}
			if (!mSuspendingGame)
			{
				DrawStandardPieces(g, DP_pStdPieces, numPieces);
			}
			DrawQuestPieces(g, DP_pQuestPieces, numPieces3, thePostFX);
			if (mSuspendingGame)
			{
				return;
			}
			if (mLightningStorms.Count > 0 && graphics3D != null && graphics3D.SupportsPixelShaders() && GetPieceAlpha() >= 0.99f)
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.SetColorizeImages(true);
				RenderEffect effect = graphics3D.GetEffect(GlobalMembersResourcesWP.EFFECT_GEM_SUN);
				using (RenderEffectAutoState renderEffectAutoState = new RenderEffectAutoState(g, effect))
				{
					while (!renderEffectAutoState.IsDone())
					{
						for (int l = 0; l < mLightningStorms.Count; l++)
						{
							LightningStorm lightningStorm = mLightningStorms[l];
							if ((double)lightningStorm.mLightingAlpha != 0.0)
							{
								continue;
							}
							Piece[,] array2 = mBoard;
							foreach (Piece piece2 in array2)
							{
								if (piece2 != null && piece2 != mGameOverPiece && !IsPieceSwapping(piece2) && (!thePostFX || piece2.mElectrocutePercent > 0f || (num & (1 << piece2.mColor + 1)) != 0) && (!thePostFX || (!piece2.IsFlagSet(4096u) && !piece2.IsFlagSet(2048u))))
								{
									DrawGemLightningLighting(g, piece2, lightningStorm);
								}
							}
						}
						renderEffectAutoState.NextPass();
					}
				}
				g.SetDrawMode(Graphics.DrawMode.Normal);
			}
			if (mSunPosition.IsInitialized() && !mSunPosition.HasBeenTriggered())
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				RenderEffect effect2 = graphics3D.GetEffect(GlobalMembersResourcesWP.EFFECT_GEM_SUN);
				using (RenderEffectAutoState renderEffectAutoState2 = new RenderEffectAutoState(g, effect2))
				{
					while (!renderEffectAutoState2.IsDone())
					{
						Piece[,] array3 = mBoard;
						foreach (Piece piece3 in array3)
						{
							if (piece3 != null && piece3 != mGameOverPiece && !IsPieceSwapping(piece3) && (!thePostFX || piece3.mElectrocutePercent > 0f || (num & (1 << piece3.mColor + 1)) != 0) && (!thePostFX || (!piece3.IsFlagSet(4096u) && !piece3.IsFlagSet(2048u))))
							{
								DrawGemSun(g, piece3, effect2);
							}
						}
						renderEffectAutoState2.NextPass();
					}
				}
				g.SetDrawMode(Graphics.DrawMode.Normal);
			}
			for (int num4 = 0; num4 < mSwapDataVector.Count; num4++)
			{
				SwapData swapData = mSwapDataVector[num4];
				float num5 = (float)(double)swapData.mGemScale;
				if (!swapData.mDestroyTarget && swapData.mPiece2 != null && (double)swapData.mSwapPct <= 3.1415927410125732)
				{
					DrawPiece(g, swapData.mPiece2, 1f - num5);
				}
				DrawPiece(g, swapData.mPiece1, 1f + num5);
				if (!swapData.mDestroyTarget && swapData.mPiece2 != null && (double)swapData.mSwapPct > 3.1415927410125732)
				{
					DrawPiece(g, swapData.mPiece2, 1f - num5);
				}
			}
			if (mCursorSelectPos.mX != -1 && GetSelectedPiece() == null)
			{
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, (int)(255f * GetPieceAlpha())));
				GlobalMembers.gGR.DrawImage(g, GlobalMembersResourcesWP.IMAGE_SELECTOR, GlobalMembers.S(GetBoardX() + GetColX(mCursorSelectPos.mX)), GlobalMembers.S(GetBoardY() + GetRowY(mCursorSelectPos.mY)));
			}
		}

		public virtual void DrawQuestPieces(Graphics g, Piece[] pPieces, int numPieces)
		{
			DrawQuestPieces(g, pPieces, numPieces, false);
		}

		public virtual void DrawQuestPieces(Graphics g, Piece[] pPieces, int numPieces, bool thePostFX)
		{
		}

		public virtual void DrawButtons(Graphics g)
		{
			if (!mIsWholeGameReplay)
			{
				g.SetDrawMode(Graphics.DrawMode.Normal);
				float mTransX = g.mTransX;
				float mTransY = g.mTransY;
				g.Translate(mHintButton.mX + (int)GlobalMembers.S(mSideXOff) + mOffsetX, mHintButton.mY + mOffsetY);
				mHintButton.Draw(g);
				g.SetColor(Color.White);
				g.mTransX = mTransX;
				g.mTransY = mTransY;
				if (mResetButton != null)
				{
					g.Translate(mResetButton.mX + (int)GlobalMembers.S(mSideXOff), mResetButton.mY);
					mResetButton.Draw(g);
					g.SetColorizeImages(false);
					g.mTransX = mTransX;
					g.mTransY = mTransY;
				}
			}
		}

		public void DrawComboUIGem(Graphics g, int aCombo)
		{
		}

		public void DrawIris(Graphics g, int theCX, int theCY)
		{
			Graphics3D graphics3D = g.Get3D();
			if (graphics3D != null)
			{
				g.PushState();
				g.SetColorizeImages(true);
				g.SetColor(new Color(GlobalMembers.M(0), GlobalMembers.M(0), GlobalMembers.M(0), (int)(255f * ConstantsWP.BOARD_IRIS_ALPHA)));
				int num = (int)((float)GlobalMembersResourcesWP.IMAGE_BOARD_IRIS.mWidth * ConstantsWP.BOARD_IRIS_SCALE);
				int num2 = (int)((float)GlobalMembersResourcesWP.IMAGE_BOARD_IRIS.mHeight * ConstantsWP.BOARD_IRIS_SCALE);
				Rect rect = new Rect(theCX - num / 2, theCY - num2 / 2, num, num2);
				g.FillRect(0, 0, rect.mX, mHeight);
				g.FillRect(rect.mX + rect.mWidth, 0, mWidth - rect.mX + rect.mWidth, mHeight);
				g.FillRect(rect.mX, 0, rect.mWidth, rect.mY);
				g.FillRect(rect.mX, rect.mY + rect.mHeight, rect.mWidth, mHeight - rect.mY + rect.mHeight);
				g.SetColor(new Color(255, 255, 255, 255));
				g.SetScale(ConstantsWP.BOARD_IRIS_SCALE, ConstantsWP.BOARD_IRIS_SCALE, theCX, theCY);
				Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BOARD_IRIS, 0, theCX, theCY);
				g.PopState();
			}
		}

		public virtual int GetUICenterX()
		{
			return GlobalMembers.RS(ConstantsWP.BOARD_UI_SCORE_CENTER_X);
		}

		public virtual void DrawScoreWidget(Graphics g)
		{
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			if (!mContentLoaded)
			{
				return;
			}
			if (mFlattening)
			{
				base.DrawAll(theFlags, g);
				return;
			}
			if (mFlattenedImage != null && SexyFramework.GlobalMembers.gIs3D && mFlattenedImage.GetRenderData() != null)
			{
				Flatten();
			}
			if ((double)mAlpha == 0.0)
			{
				return;
			}
			Graphics3D graphics3D = null;
			if (mNeedsMaskCleared)
			{
				graphics3D?.ClearMask();
			}
			if ((double)mQuestPortalPct != 0.0 && graphics3D != null)
			{
				graphics3D.ClearMask();
				graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_WRITE_MASKONLY);
				Transform transform = new Transform();
				transform.Scale((float)((double)mQuestPortalPct * (double)GlobalMembers.M(0.93f)), (float)((double)mQuestPortalPct * (double)GlobalMembers.M(0.93f)));
				g.SetColor(Color.White);
				g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_BOOM_NUKE, transform, (int)((double)GlobalMembers.S(800) + (double)GlobalMembers.S(mQuestPortalOrigin.mX - 800) * (1.0 - (double)mQuestPortalCenterPct)), (int)((double)GlobalMembers.S(600) + (double)GlobalMembers.S(mQuestPortalOrigin.mY - 600) * (1.0 - (double)mQuestPortalCenterPct)));
				graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_TEST_INSIDE);
				float num = (float)Math.Min(1.0, GlobalMembers.M(0.5) + (double)mQuestPortalPct * (double)GlobalMembers.M(0.75f));
				SexyTransform2D theTransform = default(SexyTransform2D);
				theTransform.Translate(-mWidth / 2, -mHeight / 2);
				theTransform.Scale(num, num);
				theTransform.Translate(mWidth / 2, mHeight / 2);
				theTransform.Translate((int)((double)GlobalMembers.S(mQuestPortalOrigin.mX - 800) * (1.0 - (double)mQuestPortalCenterPct) * (double)GlobalMembers.M(1f)), (int)((double)GlobalMembers.S(mQuestPortalOrigin.mY - 600) * (1.0 - (double)mQuestPortalCenterPct) * (double)GlobalMembers.M(1f)));
				graphics3D.PushTransform(theTransform);
			}
			bool flag = (double)mScale != 1.0;
			if (graphics3D != null && flag)
			{
				float num2 = (float)SexyFramework.GlobalMembers.gSexyApp.mScreenBounds.mWidth / 2f;
				float num3 = (float)SexyFramework.GlobalMembers.gSexyApp.mScreenBounds.mHeight / 2f;
				SexyTransform2D theTransform2 = default(SexyTransform2D);
				theTransform2.Translate(0f - num2, 0f - num3);
				theTransform2.Scale((float)(double)mScale, (float)(double)mScale);
				theTransform2.Translate(num2, num3);
				graphics3D.PushTransform(theTransform2);
			}
			if (mGameOverPiece != null)
			{
				graphics3D?.ClearMask();
			}
			if ((double)mNovaRadius != 0.0 && mShowBoard && graphics3D != null)
			{
				graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_WRITE_MASKONLY);
				Transform transform2 = new Transform();
				transform2.Scale((float)((double)mNovaRadius * (double)GlobalMembers.M(0.93f)), (float)((double)mNovaRadius * (double)GlobalMembers.M(0.93f)));
				g.SetColor(Color.White);
				g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_BOOM_NUKE, transform2, (int)GlobalMembers.S(mGameOverPiece.CX()), (int)GlobalMembers.S(mGameOverPiece.CY()));
				graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_TEST_OUTSIDE);
			}
			if (mFlattenedImage != null)
			{
				g.DrawImage(mFlattenedImage, GlobalMembers.gApp.mScreenBounds.mX, 0);
			}
			else if (mShowBoard)
			{
				base.DrawAll(theFlags, g);
			}
			else
			{
				if (mHyperspace != null)
				{
					g.PushState();
					g.Translate(mHyperspace.mX, mHyperspace.mY);
					mHyperspace.DrawBackground(g);
					g.PopState();
				}
				DeferOverlay(1);
			}
			if (flag || (double)mQuestPortalPct != 0.0)
			{
				Graphics mCurG = mWidgetManager.mCurG;
				g.PushState();
				g.Translate((int)(0f - g.mTransX), (int)(0f - g.mTransY));
				mWidgetManager.mCurG = g;
				mWidgetManager.FlushDeferredOverlayWidgets(10);
				g.PopState();
				mWidgetManager.mCurG = mCurG;
			}
			if (graphics3D != null && flag)
			{
				graphics3D.PopTransform();
			}
			if (graphics3D != null && (double)mQuestPortalPct != 0.0)
			{
				graphics3D.PopTransform();
			}
		}

		public override void Draw(Graphics g)
		{
			if (!mContentLoaded)
			{
				return;
			}
			Rect mClipRect = g.mClipRect;
			if (mHyperspace != null)
			{
				ClipCollapsingBoard(g);
			}
			if (mFirstDraw)
			{
				FirstDraw();
				mFirstDraw = false;
			}
			DeferOverlay(1);
			if (!((double)mSlideUIPct >= 1.0) || mGameOverCount <= 0)
			{
				g.SetColorizeImages(true);
				DrawCheckboard(g);
				if (mHyperspace != null)
				{
					g.SetClipRect(mClipRect);
				}
				if (mWarningGlowAlpha > 0f)
				{
					g.SetColor(mWarningGlowColor);
					g.mColor.mAlpha = (int)(mWarningGlowAlpha * 255f);
					g.SetColorizeImages(true);
					int num = 800;
					int theNum = 800;
					int boardX = GetBoardX();
					int boardY = GetBoardY();
					float num2 = GlobalMembers.M(0.5f) + GlobalMembers.M(2.5f) * (float)Math.Pow(mWarningGlowAlpha, GlobalMembers.M(0.5f));
					int theStretchedHeight = (int)((float)GlobalMembersResourcesWP.IMAGE_DANGERBORDERUP.GetHeight() * num2);
					int num3 = (int)((float)GlobalMembersResourcesWP.IMAGE_DANGERBORDERLEFT.GetWidth() * num2);
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_DANGERBORDERUP, GlobalMembers.S(boardX), GlobalMembers.S(boardY), GlobalMembers.S(num), theStretchedHeight);
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_DANGERBORDERLEFT, GlobalMembers.S(boardX), GlobalMembers.S(boardY), num3, GlobalMembers.S(theNum));
					g.DrawImageMirror(GlobalMembersResourcesWP.IMAGE_DANGERBORDERLEFT, new Rect(GlobalMembers.S(boardX + num) - num3, GlobalMembers.S(boardY), num3, GlobalMembers.S(theNum)), new Rect(0, 0, GlobalMembersResourcesWP.IMAGE_DANGERBORDERLEFT.GetWidth(), GlobalMembersResourcesWP.IMAGE_DANGERBORDERLEFT.GetHeight()));
					Transform transform = new Transform();
					transform.Scale(num, 0f - num2);
					g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_DANGERBORDERUP, transform, GlobalMembers.S(boardX + num / 2), GlobalMembers.S(theNum) + GlobalMembersResourcesWP.IMAGE_DANGERBORDERUP.GetHeight() / 2);
					g.SetColor(new Color(-1));
					g.SetColorizeImages(false);
				}
				if (!mIsWholeGameReplay)
				{
					DrawUI(g);
				}
				if (mCurrentHint != null && !mInReplay)
				{
					mCurrentHint.Draw(g);
				}
				g.SetColor(Color.White);
			}
		}

		public virtual void FirstDraw()
		{
		}

		public virtual void DrawUI(Graphics g)
		{
			if ((double)mSideAlpha != 1.0)
			{
				g.SetColor(mSideAlpha);
				g.PushColorMult();
			}
			float mTransX = g.mTransX;
			float mTransY = g.mTransY;
			if ((double)mSideXOff != 0.0)
			{
				g.Translate((int)((double)mSideXOff * (double)mSlideXScale), 0);
			}
			DrawBars(g);
			if ((double)mSideXOff != 0.0)
			{
				g.mTransX = mTransX;
				g.mTransY = mTransY;
			}
			DrawHUD(g);
			if ((double)mSideXOff != 0.0)
			{
				g.mTransX = mTransX;
				g.mTransY = mTransY;
				g.Translate((int)GlobalMembers.S(mSideXOff), 0);
			}
			if (WantWarningGlow())
			{
				DrawWarningHUD(g);
			}
			if (WantDrawButtons())
			{
				DrawButtons(g);
			}
			DrawHUDText(g);
			if ((double)mSideXOff != 0.0)
			{
				g.mTransX = mTransX;
				g.mTransY = mTransY;
			}
			if ((double)mSideAlpha != 1.0)
			{
				g.PopColorMult();
			}
		}

		public virtual void DrawHUD(Graphics g)
		{
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColorizeImages(true);
			g.SetColor(new Color(255, 255, 255, (int)(255f * GetAlpha())));
			float mTransX = g.mTransX;
			float mTransY = g.mTransY;
			if ((double)mSideXOff != 0.0)
			{
				g.Translate((int)(double)mSideXOff, 0);
			}
			DrawReplayWidget(g);
			DrawScoreWidget(g);
			DrawMenuWidget(g);
			if ((double)mSideXOff != 0.0)
			{
				g.mTransX = mTransX;
				g.mTransY = mTransY;
				g.Translate((int)((double)mSideXOff * (double)mSlideXScale), 0);
			}
			DrawFrame(g);
			if ((double)mSideXOff != 0.0)
			{
				g.mTransX = mTransX;
				g.mTransY = mTransY;
			}
		}

		public virtual void DrawWarningHUD(Graphics g)
		{
			Color color = g.GetColor().Clone();
			bool flag = WantTopFrame() || WantBottomFrame();
			if (flag)
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.SetColor(GetWarningGlowColor());
			}
			if (WantTopFrame())
			{
				if (WantTopLevelBar() || GetTimeLimit() > 0)
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_FRAME, (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_FRAME))) + (float)mTransBoardOffsetX), (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_PROGRESS_BAR_FRAME))) - (float)mTransBoardOffsetY));
				}
				else
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME, (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME))) + (float)mTransBoardOffsetX), (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME))) - (float)mTransBoardOffsetY));
				}
			}
			if (WantBottomFrame())
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME, (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME))) + (float)mTransBoardOffsetX), (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(GlobalMembersResourcesWP.GetIdByImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BOARD_SEPERATOR_FRAME))) + (float)mTransBoardOffsetY));
			}
			if (flag)
			{
				g.SetColor(color);
				g.SetDrawMode(Graphics.DrawMode.Normal);
			}
		}

		public virtual void DrawHUDText(Graphics g)
		{
			if (WantExpandedTopWidget() == 0)
			{
				int num = (int)(GlobalMembers.IMG_SXOFS(1094) + (float)GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelWidth());
				int num2 = num + (mWidth - num) / 2;
				int num3 = (int)((GlobalMembers.IMG_SYOFS(1091) + (float)GlobalMembersResources.FONT_DIALOG.mAscent) / 2f - (float)mTransLevelOffsetY);
				g.SetFont(GlobalMembersResources.FONT_DIALOG);
				float mScaleX = g.mScaleX;
				float mScaleY = g.mScaleY;
				g.SetScale(ConstantsWP.BOARD_LEVEL_SCORE_SCALE, ConstantsWP.BOARD_LEVEL_SCORE_SCALE, num2, num3 - g.GetFont().GetAscent() / 2);
				Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Color.White);
				g.WriteString(string.Format(GlobalMembers._ID("Level {0}", 3232), SexyFramework.Common.CommaSeperate(mLevel + 1)), num2, num3);
				g.mScaleX = mScaleX;
				g.mScaleY = mScaleY;
			}
			else
			{
				string topWidgetButtonText = GetTopWidgetButtonText();
				if (topWidgetButtonText != string.Empty)
				{
					g.SetFont(GlobalMembersResources.FONT_DIALOG);
					g.WriteString(topWidgetButtonText, GlobalMembers.S(GetUICenterX()), GlobalMembers.MS(262));
				}
			}
			if (WantDrawScore())
			{
				DrawScore(g);
			}
			if (WantDrawTimer())
			{
				DrawTimer(g);
			}
		}

		public virtual void DrawMenuWidget(Graphics g)
		{
		}

		public virtual void DrawBars(Graphics g)
		{
			if (WantCountdownBar())
			{
				DrawCountdownBar(g);
			}
			else if (WantTopLevelBar())
			{
				DrawLevelBar(g);
			}
		}

		public virtual void DrawTimer(Graphics g)
		{
			if (GetTimeLimit() != 0 && !mIsWholeGameReplay)
			{
				int ticksLeft = GetTicksLeft();
				Rect countdownBarRect = GetCountdownBarRect();
				Point point = new Point(GlobalMembers.S(GetBoardCenterX()), GlobalMembers.MS(500));
				Point point2 = new Point(GetTimeDrawX(), countdownBarRect.mY + countdownBarRect.mHeight / 2);
				Point point3 = point + point2;
				string theString = string.Format(GlobalMembers._ID("{0}:{1:d2}", 148), (ticksLeft + 99) / 100 / 60, (ticksLeft + 99) / 100 % 60);
				g.SetFont(GlobalMembersResources.FONT_SCORE);
				g.SetColor(new Color(255, 255, 255, (int)((double)(255f * GetAlpha()) * (double)mTimerAlpha)));
				g.WriteString(theString, point2.mX, point2.mY + GetTimerYOffset());
			}
		}

		public void Flatten()
		{
			mFlattenedImage = null;
			mFlattening = true;
			DeviceImage theDestImage = GlobalMembers.gApp.mRestartRT.Lock(GlobalMembers.gApp.mScreenBounds.mWidth, GlobalMembers.gApp.mScreenBounds.mHeight);
			Graphics graphics = new Graphics(theDestImage);
			graphics.Translate(-GlobalMembers.gApp.mScreenBounds.mX, 0);
			bool flag = GlobalMembers.gApp.mUnderDialogWidget.mVisible;
			bool flag2 = GlobalMembers.gApp.mTooltipManager.mVisible;
			bool flag3 = GlobalMembers.gApp.mQuestMenu != null && GlobalMembers.gApp.mQuestMenu.mVisible;
			bool flag4 = GlobalMembers.gApp.mQuestMenu != null && GlobalMembers.gApp.mQuestMenu.mBackground.mVisible;
			GlobalMembers.gApp.mUnderDialogWidget.mVisible = false;
			GlobalMembers.gApp.mTooltipManager.mVisible = false;
			if (flag3)
			{
				GlobalMembers.gApp.mQuestMenu.mVisible = false;
			}
			if (flag4)
			{
				GlobalMembers.gApp.mQuestMenu.mBackground.mVisible = false;
			}
			mWidgetManager.DrawWidgetsTo(graphics);
			GlobalMembers.gApp.mUnderDialogWidget.mVisible = flag;
			GlobalMembers.gApp.mTooltipManager.mVisible = flag2;
			if (flag3)
			{
				GlobalMembers.gApp.mQuestMenu.mVisible = true;
			}
			if (flag4)
			{
				GlobalMembers.gApp.mQuestMenu.mBackground.mVisible = true;
			}
			GlobalMembers.gApp.mRestartRT.Unlock();
			mFlattening = false;
			mFlattenedImage = theDestImage;
			MarkAllDirty();
		}

		public override void RemovedFromManager(WidgetManager theWidgetManager)
		{
			base.RemovedFromManager(theWidgetManager);
			if (mBackground != null && mBackground.mParent != null && (GlobalMembers.gApp.mBoard == null || GlobalMembers.gApp.mBoard == this))
			{
				mBackground.mParent.RemoveWidget(mBackground);
			}
			if (mGameOverCount == 0 && GlobalMembers.gApp.mMainMenu.mIsFullGame())
			{
				SaveGame();
			}
			GlobalMembers.gGR.mIgnoreDraws = false;
			GlobalMembers.gGR.mRecordDraws = false;
		}

		public void DrawPauseText(Graphics g, float alpha)
		{
			if (alpha != 0f)
			{
				if (alpha > 1f)
				{
					alpha = 1f;
				}
				if (g.mPushedColorVector.Count > 0)
				{
					g.PopColorMult();
				}
				g.SetFont(GlobalMembersResources.FONT_HUGE);
				g.SetColor(new Color(255, 255, 255, (int)(255f * alpha)));
				Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Bej3Widget.COLOR_INGAME_ANNOUNCEMENT);
				string theString = GlobalMembers._ID("PAUSED", 147);
				g.DrawString(theString, GlobalMembers.S(GetBoardCenterX()) - g.GetFont().StringWidth(theString) / 2, GlobalMembers.MS(540));
			}
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
			if (mInReplay || !CanPlay() || GlobalMembers.gApp.TouchedToolTip(x, y))
			{
				return;
			}
			mCursorSelectPos = new Point(-1, -1);
			mMouseDown = true;
			mMouseDownX = x;
			mMouseDownY = y;
			int num = GetColAt((int)((float)x / GlobalMembers.S(1f)));
			int num2 = GetRowAt((int)((float)y / GlobalMembers.S(1f)));
			Piece selectedPiece = GetSelectedPiece();
			if (theBtnNum != 0)
			{
				if (selectedPiece != null)
				{
					selectedPiece.mSelected = false;
					selectedPiece.mSelectorAlpha.SetConstant(0.0);
				}
				return;
			}
			Piece piece = GetPieceAtScreenXY((int)((float)x / GlobalMembers.S(1f)), (int)((float)y / GlobalMembers.S(1f)));
			if (selectedPiece == piece)
			{
				return;
			}
			bool flag = false;
			if (piece == null)
			{
				piece = GetPieceAtRowCol(num2, num);
				if (piece != null)
				{
					flag = true;
				}
			}
			else
			{
				num = piece.mCol;
				num2 = piece.mRow;
				Piece piece2 = MoveAssistedPiece(piece, x, y, selectedPiece);
				if (piece2 != null)
				{
					piece = piece2;
					num = piece.mCol;
					num2 = piece.mRow;
				}
			}
			if (piece != null && !piece.mCanSwap)
			{
				flag = true;
			}
			if (!flag && piece != selectedPiece)
			{
				if (selectedPiece != null)
				{
					if (mLightningStorms.Count == 0 && !QueueSwap(selectedPiece, num2, num))
					{
						selectedPiece.mSelected = false;
						selectedPiece.mSelectorAlpha.SetConstant(0.0);
						if (piece != null)
						{
							piece.mSelected = true;
							piece.mSelectorAlpha.SetConstant(1.0);
						}
					}
				}
				else if (piece != null)
				{
					if (piece.IsButton())
					{
						QueueSwap(piece, piece.mRow, piece.mCol, false, true);
						return;
					}
					piece.mSelected = true;
					piece.mSelectorAlpha.SetConstant(1.0);
					SexyFramework.GlobalMembers.gSexyApp.PlaySample(GlobalMembersResourcesWP.SOUND_SELECT);
				}
			}
			else if (selectedPiece != null)
			{
				selectedPiece.mSelected = false;
				selectedPiece.mSelectorAlpha.SetConstant(0.0);
			}
		}

		public override void MouseUp(int x, int y)
		{
			mMouseDown = false;
			Piece selectedPiece = GetSelectedPiece();
			if (selectedPiece != null && selectedPiece == mMouseUpPiece && !IsPieceSwapping(selectedPiece))
			{
				selectedPiece.mSelected = false;
				selectedPiece.mSelectorAlpha.SetConstant(0.0);
				mMouseUpPiece = null;
			}
			else
			{
				mMouseUpPiece = selectedPiece;
			}
		}

		public override void MouseDrag(int x, int y)
		{
			base.MouseDrag(x, y);
			if (!CanPlay())
			{
				return;
			}
			Piece selectedPiece = GetSelectedPiece();
			if (!mMouseDown || selectedPiece == null)
			{
				return;
			}
			int num = x - mMouseDownX;
			int num2 = y - mMouseDownY;
			if (Math.Abs(num) < GlobalMembers.MS(40) && Math.Abs(num2) < GlobalMembers.MS(40))
			{
				return;
			}
			Point point = new Point(-1, -1);
			if (Math.Abs(num) > Math.Abs(num2))
			{
				if (num > 0 && selectedPiece.mCol < 7)
				{
					point = new Point(selectedPiece.mCol + 1, selectedPiece.mRow);
				}
				else if (num < 0 && selectedPiece.mCol > 0)
				{
					point = new Point(selectedPiece.mCol - 1, selectedPiece.mRow);
				}
			}
			else if (num2 > 0 && selectedPiece.mRow < 7)
			{
				point = new Point(selectedPiece.mCol, selectedPiece.mRow + 1);
			}
			else if (num2 < 0 && selectedPiece.mRow > 0)
			{
				point = new Point(selectedPiece.mCol, selectedPiece.mRow - 1);
			}
			if (point != new Point(-1, -1))
			{
				QueueSwap(selectedPiece, point.mY, point.mX);
			}
		}

		public override void KeyDown(KeyCode theKeyCode)
		{
			Point point = default(Point);
			bool flag = false;
			bool flag2 = false;
			if (mInReplay)
			{
				return;
			}
			switch (theKeyCode)
			{
			case KeyCode.KEYCODE_ESCAPE:
				if (!mKilling)
				{
					ButtonDepress(1);
				}
				break;
			case KeyCode.KEYCODE_LEFT:
				flag2 = true;
				point = new Point(-1, 0);
				break;
			case KeyCode.KEYCODE_RIGHT:
				flag2 = true;
				point = new Point(1, 0);
				break;
			case KeyCode.KEYCODE_UP:
				flag2 = true;
				point = new Point(0, -1);
				break;
			case KeyCode.KEYCODE_DOWN:
				flag2 = true;
				point = new Point(0, 1);
				break;
			case KeyCode.KEYCODE_SPACE:
			{
				Piece selectedPiece = GetSelectedPiece();
				if (selectedPiece != null)
				{
					selectedPiece.mSelected = false;
					selectedPiece.mSelectorAlpha.SetConstant(0.0);
				}
				else
				{
					flag = true;
				}
				break;
			}
			case (KeyCode)65:
				flag = true;
				point = new Point(-1, 0);
				break;
			case (KeyCode)68:
				flag = true;
				point = new Point(1, 0);
				break;
			case (KeyCode)87:
				flag = true;
				point = new Point(0, -1);
				break;
			case (KeyCode)83:
				flag = true;
				point = new Point(0, 1);
				break;
			}
			Piece piece = null;
			if (mTimeExpired || !CanPlay())
			{
				return;
			}
			if (flag && GetSelectedPiece() == null)
			{
				Piece piece2 = null;
				piece2 = ((mCursorSelectPos.mX != -1) ? GetPieceAtScreenXY(GetBoardX() + GetColX(mCursorSelectPos.mX) + 50, GetBoardY() + GetRowY(mCursorSelectPos.mY) + 50) : GetPieceAtScreenXY((int)((float)mWidgetManager.mLastMouseX / GlobalMembers.S(1f)), (int)((float)mWidgetManager.mLastMouseY / GlobalMembers.S(1f))));
				if (piece2 != null)
				{
					if (piece2.IsFlagSet(6144u))
					{
						if (mCursorSelectPos.mX != -1)
						{
							piece = piece2;
						}
					}
					else if ((point.mX != 0 || point.mY != 0 || mCursorSelectPos.mX != -1) && !IsPieceSwapping(piece2))
					{
						piece2.mSelected = true;
						piece2.mSelectorAlpha.SetConstant(1.0);
					}
				}
			}
			if (point.mX == 0 && point.mY == 0 && piece == null)
			{
				return;
			}
			Piece piece3 = piece;
			if (piece3 == null)
			{
				piece3 = GetSelectedPiece();
			}
			if (piece3 != null && (IsGameSuspended() || !QueueSwap(piece3, piece3.mRow + point.mY, piece3.mCol + point.mX, false, true)))
			{
				piece3.mSelected = false;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_SELECTOR_ALPHA, piece3.mSelectorAlpha);
			}
			else if (mCursorSelectPos.mX == -1)
			{
				if (!flag)
				{
					mCursorSelectPos = new Point(3 + Math.Max(0, point.mX), 3 + Math.Max(0, point.mY));
				}
			}
			else if (flag2)
			{
				mCursorSelectPos.mX = Math.Max(0, Math.Min(7, mCursorSelectPos.mX + point.mX));
				mCursorSelectPos.mY = Math.Max(0, Math.Min(7, mCursorSelectPos.mY + point.mY));
			}
		}

		public override void KeyChar(char theChar)
		{
			if (!mInReplay && theChar == ' ' && mCursorSelectPos.mX == -1 && (WantsHideOnPause() || mUserPaused) && !mInReplay)
			{
				mUserPaused = !mUserPaused;
			}
		}

		public override void ButtonPress(int theId)
		{
			if (theId != 0)
			{
				base.ButtonPress(theId);
			}
		}

		public override void ButtonMouseEnter(int theId)
		{
			if (theId != 0)
			{
				base.ButtonMouseEnter(theId);
			}
			else
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HINT);
			}
		}

		public override void ButtonMouseLeave(int theId)
		{
			if (theId != 0)
			{
				base.ButtonMouseLeave(theId);
			}
		}

		public override void ButtonDepress(int theId)
		{
			if (mInReplay || !AllowUI())
			{
				return;
			}
			if (theId != 0)
			{
				base.ButtonDepress(theId);
			}
			switch ((BUTTON)theId)
			{
			case BUTTON.BUTTON_HINT:
				if (mBoardHidePct == 0f)
				{
					ShowHint(true);
				}
				break;
			case BUTTON.BUTTON_MENU:
				GlobalMembers.gApp.DoPauseMenu();
				break;
			case BUTTON.BUTTON_REPLAY:
				mWatchedCurReplay = true;
				mReplayWasTutorial = false;
				RewindToReplay(mCurReplayData);
				mReplayWidgetShowPct.SetConstant(1.0);
				HideReplayWidget();
				GlobalMembers.gApp.DisableOptionsButtons(true);
				break;
			case BUTTON.BUTTON_RESET:
				break;
			}
		}

		public virtual void SliderVal(int theId, double theVal)
		{
			mSliderSetTicks = (int)((1.0 - theVal) * (double)mUReplayTotalTicks + 0.5);
		}

		public virtual void DialogButtonDepress(int theDialogId, int theButtonId)
		{
			Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.GetDialog(theDialogId);
			if (theDialogId == 18)
			{
				if (mDeferredTutorialVector.Count > 0)
				{
					DeferredTutorial deferredTutorial = mDeferredTutorialVector[0];
					switch (theButtonId)
					{
					case 1000:
						mDeferredTutorialVector.RemoveAt(0);
						if (((HintDialog)bej3Dialog).mNoHintsCheckbox.IsChecked())
						{
							SetTutorialCleared(19);
							mTutorialFlags = (int)GlobalMembers.gApp.mProfile.mTutorialFlags;
							mDeferredTutorialVector.Clear();
						}
						break;
					case 1001:
					{
						mReplayWasTutorial = true;
						ReplayData mReplayData = deferredTutorial.mReplayData;
						RewindToReplay(mReplayData);
						mReplayIgnoredMoves = 0;
						SetTutorialCleared(deferredTutorial.mTutorialFlag, false);
						GlobalMembers.gApp.DisableOptionsButtons(true);
						break;
					}
					default:
						mDeferredTutorialVector.RemoveAt(0);
						SetTutorialCleared(19);
						mTutorialFlags = (int)GlobalMembers.gApp.mProfile.mTutorialFlags;
						mDeferredTutorialVector.Clear();
						break;
					}
				}
				if (mIllegalMoveTutorial)
				{
					mIllegalMoveTutorial = false;
				}
				bej3Dialog.mVisible = false;
				mTutorialPieceIrisPct.SetConstant(0.0);
				bej3Dialog.Kill();
			}
			if (theDialogId == 22)
			{
				if (theButtonId == 1000)
				{
					bej3Dialog.Kill();
					bej3Dialog.mCanSlideInMenus = false;
					((PauseMenu)GlobalMembers.gApp.mMenus[7]).Collapse(false, true);
					DeleteSavedGame();
					mWantLevelup = false;
					mHyperspace = null;
					Init();
					mHasReplayData = false;
					NewGame(true);
					for (int i = 0; i < 2; i++)
					{
						if (mSpeedFireBarPIEffect[i] != null)
						{
							mSpeedFireBarPIEffect[i].Dispose();
							mSpeedFireBarPIEffect[i] = null;
						}
					}
					HideReplayWidget();
					GlobalMembers.gApp.GoBackToGame();
					return;
				}
				bej3Dialog.Kill();
			}
			if (theDialogId == 50)
			{
				if (theButtonId == 1000)
				{
					bej3Dialog.Kill();
					if (!GlobalMembers.gApp.mMainMenu.mIsFullGame())
					{
						DeleteSavedGame();
					}
					bej3Dialog.mCanSlideInMenus = true;
					((PauseMenu)GlobalMembers.gApp.mMenus[7]).Collapse(false, true);
					GlobalMembers.gApp.DoMainMenu();
					return;
				}
				bej3Dialog.Kill();
			}
			if (theDialogId == 51)
			{
				bej3Dialog.Kill();
			}
		}

		public virtual void DisableUI(bool disabled)
		{
			mHintButton.SetDisabled(disabled);
			if (mResetButton != null)
			{
				mResetButton.SetDisabled(disabled);
			}
		}

		public virtual int GetSidebarTextY()
		{
			return GlobalMembers.M(320);
		}

		public virtual void DrawScore(Graphics g)
		{
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			string text = SexyFramework.Common.CommaSeperate(mDispPoints);
			if (mShowLevelPoints)
			{
				text = string.Format(GlobalMembers._ID("{0} of {1}", 157), text, SexyFramework.Common.CommaSeperate(GetLevelPoints()));
			}
			int num = (int)GlobalMembers.IMG_SXOFS(1094) / 2;
			int num2 = (int)(GlobalMembers.IMG_SYOFS(1091) + (float)GlobalMembersResources.FONT_DIALOG.mAscent) / 2 - mTransScoreOffsetY;
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Color.White);
			float mScaleX = g.mScaleX;
			float mScaleY = g.mScaleY;
			g.SetScale(ConstantsWP.BOARD_LEVEL_SCORE_SCALE, ConstantsWP.BOARD_LEVEL_SCORE_SCALE, num, num2 - g.GetFont().GetAscent() / 2);
			g.WriteString(text, num, num2);
			g.mScaleX = mScaleX;
			g.mScaleY = mScaleY;
		}

		public void DrawReplayWidget(Graphics g)
		{
			if (mReplayButton == null)
			{
				return;
			}
			if ((double)mReplayWidgetShowPct > 0.0)
			{
				bool flag = mReplayButton.mIsDown && mReplayButton.mIsOver && !mReplayButton.mDisabled;
				Rect theRect = ((flag ^ mReplayButton.mInverted) ? mReplayButton.mButtonImage.GetCelRect(1) : mReplayButton.mButtonImage.GetCelRect(0));
				mReplayButton.DrawButtonImage(g, mReplayButton.mButtonImage, theRect, mReplayButton.mX, mReplayButton.mY);
			}
			if ((double)mReplayPulsePct >= 0.0)
			{
				int celWidth = GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY.GetCelWidth();
				int celHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY.GetCelHeight();
				int theX = (mWidth - celWidth) / 2;
				int theY = (int)((GlobalMembers.IMG_SYOFS(1091) - (float)celHeight) / 2f);
				g.DrawImageCel(GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY, theX, theY, 1);
				if ((double)mReplayPulsePct > 0.0)
				{
					g.PushState();
					g.SetDrawMode(Graphics.DrawMode.Additive);
					g.SetColor(new Color(255, 255, 255, (int)(255.0 * (double)mReplayPulsePct)));
					g.DrawImageCel(GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY, theX, theY, 0);
					g.PopState();
				}
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			if (mLevelBarPIEffect == null && GlobalMembersResourcesWP.PIEFFECT_LEVELBAR != null)
			{
				mLevelBarPIEffect = GlobalMembersResourcesWP.PIEFFECT_LEVELBAR.Duplicate();
			}
			if (mCountdownBarPIEffect == null && GlobalMembersResourcesWP.PIEFFECT_COUNTDOWNBAR != null)
			{
				mCountdownBarPIEffect = GlobalMembersResourcesWP.PIEFFECT_COUNTDOWNBAR.Duplicate();
			}
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			if (Bej3Widget.mCurrentSlidingMenu == this)
			{
				if (GlobalMembers.gApp.mMenus[7] != null)
				{
					GlobalMembers.gApp.mMenus[7].AllowSlideIn(allow, previousTopButton);
				}
				Bej3Widget.mCurrentSlidingMenu = GlobalMembers.gApp.mMenus[7];
			}
			base.AllowSlideIn(allow, previousTopButton);
		}

		public override bool SlideForDialog(bool slideOut, Bej3Dialog dialog, Bej3ButtonType previousButtonType)
		{
			mY = 0;
			return GlobalMembers.gApp.mMenus[7].SlideForDialog(slideOut, dialog, previousButtonType);
		}

		public override int GetShowCurve()
		{
			return 24;
		}

		public void ToggleReplayPulse(bool on)
		{
			if (on)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_REPLAY_PULSE_PCT, mReplayPulsePct);
				mReplayPulsePct.SetMode(1);
			}
			else
			{
				mReplayPulsePct.SetConstant(-1.0);
			}
		}

		public void SyncUnAwardedBadges(List<int> deferredBadgeVector)
		{
			bool flag = mGameOverCount == 0;
			for (int i = 0; i < 20; i++)
			{
				Badge badgeByIndex = mBadgeManager.GetBadgeByIndex(i);
				if (badgeByIndex == null || (flag && !badgeByIndex.WantsMidGameCalc()) || !badgeByIndex.CanUnlock())
				{
					continue;
				}
				for (int j = 0; j < deferredBadgeVector.Count; j++)
				{
					if (deferredBadgeVector[j] == badgeByIndex.mIdx)
					{
						deferredBadgeVector.RemoveAt(j);
					}
				}
				deferredBadgeVector.Add(badgeByIndex.mIdx);
				GlobalMembers.gApp.mProfile.mBadgeStatus[badgeByIndex.mIdx] = true;
				badgeByIndex.mUnlocked = true;
				GlobalMembers.gApp.mProfile.AddRecentBadge(badgeByIndex.mIdx);
			}
		}

		public virtual void SubmitHighscore()
		{
		}

		public virtual int GetTimerYOffset()
		{
			return 0;
		}

		public virtual Image GetMultiplierImage()
		{
			return null;
		}

		public virtual int GetMultiplierImageX()
		{
			return 0;
		}

		public virtual int GetMultiplierImageY()
		{
			return 0;
		}

		public void MoveGemsOffscreen()
		{
			for (int i = 0; i < 8; i++)
			{
				int num = GetBoardY() - (11 - i) * 100;
				for (int j = 0; j < 8; j++)
				{
					Piece piece = mBoard[i, j];
					piece.mY = num;
				}
			}
		}

		public void UpdateSlidingHUD(bool slidingOff)
		{
			mSlidingHUDCurve.IncInVal(ConstantsWP.BOARD_SLIDING_HUD_SPEED);
			if (!mSlidingHUDCurve.IsDoingCurve())
			{
				((HyperspaceWhirlpool)mHyperspace).mSlidingHUD = false;
				if (slidingOff)
				{
					mSlidingHUDCurve.SetConstant(1.0);
				}
				else
				{
					mSlidingHUDCurve.SetConstant(0.0);
				}
			}
			int num = (int)((GlobalMembers.IMG_SYOFS(1091) + (float)GlobalMembersResources.FONT_DIALOG.mAscent) / 2f);
			mTransBoardOffsetX = (int)((double)mWidth * (double)mSlidingHUDCurve);
			mTransLevelOffsetY = (mTransScoreOffsetY = (int)((double)(num + GlobalMembersResources.FONT_DIALOG.GetDescent()) * (double)mSlidingHUDCurve));
			mTransDashboardOffsetY = (int)((double)(GlobalMembers.gApp.mHeight - ConstantsWP.MENU_Y_POS_HIDDEN) * (double)mSlidingHUDCurve);
			GlobalMembers.gApp.mMenus[7].SetTargetPosition(ConstantsWP.MENU_Y_POS_HIDDEN + mTransDashboardOffsetY);
			mTransHintBtnOffsetX = (int)((double)(GlobalMembers.gApp.mWidth - ConstantsWP.BOARD_UI_HINT_BTN_X) * (double)mSlidingHUDCurve);
			mHintButton.mX = ConstantsWP.BOARD_UI_HINT_BTN_X + mTransHintBtnOffsetX;
			if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN)
			{
				ZenBoard zenBoard = this as ZenBoard;
				if (zenBoard != null)
				{
					mTransOptionsBtnOffsetX = (int)((double)(ConstantsWP.ZENBOARD_UI_ZEN_BTN_X + ConstantsWP.ZENBOARD_UI_ZEN_BTN_WIDTH) * (double)mSlidingHUDCurve);
					zenBoard.mZenOptionsButton.mX = ConstantsWP.ZENBOARD_UI_ZEN_BTN_X - mTransOptionsBtnOffsetX;
				}
			}
		}

		public void UpdateBoardTransition(bool closingBoard)
		{
			mTransitionBoardCurve.IncInVal();
			if (!mTransitionBoardCurve.IsDoingCurve())
			{
				((HyperspaceWhirlpool)mHyperspace).mTransitionBoard = false;
				if (closingBoard)
				{
					mTransitionBoardCurve.SetConstant(1.0);
				}
				else
				{
					mTransitionBoardCurve.SetConstant(0.0);
				}
			}
			mTransBoardOffsetY = (int)((double)((float)(GlobalMembers.S(100) * 8) / 2f) * (double)mTransitionBoardCurve);
		}

		public void UpdateSpecialGemsStats(int MoveCreditId)
		{
			int moveStat = GetMoveStat(MoveCreditId, 12);
			int moveStat2 = GetMoveStat(MoveCreditId, 13);
			int moveStat3 = GetMoveStat(MoveCreditId, 14);
			int moveStat4 = GetMoveStat(MoveCreditId, 31);
			int theNumber = moveStat + moveStat2 + moveStat3 + moveStat4;
			MaxStat(38, theNumber);
		}

		public bool PointInPiece(Piece thePiece, int x, int y)
		{
			int num = (int)thePiece.GetScreenX();
			int num2 = (int)thePiece.GetScreenY();
			if (x >= GlobalMembers.S(num) && y >= GlobalMembers.S(num2) && x < GlobalMembers.S(num + 100))
			{
				return y < GlobalMembers.S(num2 + 100);
			}
			return false;
		}

		public bool PointInPiece(Piece thePiece, int x, int y, int theFuzzFactor)
		{
			if (!PointInPiece(thePiece, x, y) && !PointInPiece(thePiece, x - theFuzzFactor, y - theFuzzFactor) && !PointInPiece(thePiece, x, y - theFuzzFactor) && !PointInPiece(thePiece, x + theFuzzFactor, y - theFuzzFactor) && !PointInPiece(thePiece, x + theFuzzFactor, y) && !PointInPiece(thePiece, x + theFuzzFactor, y + theFuzzFactor) && !PointInPiece(thePiece, x, y + theFuzzFactor) && !PointInPiece(thePiece, x - theFuzzFactor, y + theFuzzFactor))
			{
				return PointInPiece(thePiece, x - theFuzzFactor, y);
			}
			return true;
		}

		public Piece MoveAssistedPiece(Piece pSelectedPiece, int x, int y, Piece pPrevSelectedPiece)
		{
			if (pSelectedPiece == null)
			{
				return null;
			}
			Piece result = null;
			int num = 0;
			int iLaserCount;
			int iMultiCount;
			int iFlameCount = (iLaserCount = (iMultiCount = 0));
			int theDirXResult;
			int theDirYResult;
			int num2 = FindBestGemMove(pSelectedPiece, out theDirXResult, out theDirYResult, out iFlameCount, out iMultiCount, out iLaserCount);
			if (num2 >= 3)
			{
				return null;
			}
			if (pSelectedPiece.IsFlagSet(2u))
			{
				return null;
			}
			for (int i = pSelectedPiece.mRow - 1; i <= pSelectedPiece.mRow + 1; i++)
			{
				if (i < 0 || i >= 8)
				{
					continue;
				}
				for (int j = pSelectedPiece.mCol - 1; j <= pSelectedPiece.mCol + 1; j++)
				{
					if (j < 0 || j >= 8 || (j == pSelectedPiece.mCol && i == pSelectedPiece.mRow))
					{
						continue;
					}
					Piece pieceAtRowCol = GetPieceAtRowCol(i, j);
					if (pieceAtRowCol == pPrevSelectedPiece || pieceAtRowCol == null || !PointInPiece(pieceAtRowCol, mMouseDownX, mMouseDownY, GlobalMembers.S(FUDGE)))
					{
						continue;
					}
					iFlameCount = (iLaserCount = (iMultiCount = 0));
					if (pieceAtRowCol.IsFlagSet(2u))
					{
						return pieceAtRowCol;
					}
					num2 = FindBestGemMove(pieceAtRowCol, out theDirXResult, out theDirYResult, out iFlameCount, out iMultiCount, out iLaserCount);
					if (num2 >= 3)
					{
						num2 += iFlameCount * 6;
						num2 += iMultiCount * 20;
						num2 += iLaserCount * 13;
						if (num2 > num)
						{
							result = pieceAtRowCol;
							num = num2;
						}
					}
				}
			}
			return result;
		}

		public int FindBestGemMove(Piece aPiece, out int theDirXResult, out int theDirYResult, out int iFlameCount, out int iMultiCount, out int iLaserCount)
		{
			int[,] array = new int[4, 2]
			{
				{ 1, 0 },
				{ -1, 0 },
				{ 0, 1 },
				{ 0, -1 }
			};
			int num = -1;
			theDirXResult = 0;
			theDirYResult = 0;
			iFlameCount = 0;
			iMultiCount = 0;
			iLaserCount = 0;
			for (int i = 0; i < 4; i++)
			{
				int num2 = array[i, 0];
				int num3 = array[i, 1];
				int num4 = EvalGemSwap(aPiece, num2, num3, out iFlameCount, out iMultiCount, out iLaserCount);
				if (num4 > num)
				{
					num = num4;
					theDirXResult = num2;
					theDirYResult = num3;
				}
			}
			return num;
		}

		public int EvalGemSwap(Piece aPiece, int theDirX, int theDirY, out int iFlameCount, out int iMultiCount, out int iLaserCount)
		{
			int mCol = aPiece.mCol;
			int mRow = aPiece.mRow;
			int num = 0;
			iFlameCount = 0;
			iMultiCount = 0;
			iLaserCount = 0;
			int num2 = mCol + theDirX;
			int num3 = mRow + theDirY;
			if (num2 >= 0 && num2 < 8 && num3 >= 0 && num3 < 8)
			{
				Piece piece = mBoard[mRow, mCol];
				if (mBoard[num3, num2] != null)
				{
					if (mBoard[num3, num2].IsFlagSet(2u))
					{
						Piece[,] array = mBoard;
						foreach (Piece piece2 in array)
						{
							if (piece2 != null && aPiece.mColor == piece2.mColor)
							{
								num++;
								if (piece2.IsFlagSet(16u))
								{
									iMultiCount++;
								}
								if (piece2.IsFlagSet(4u))
								{
									iLaserCount++;
								}
								if (piece2.IsFlagSet(1u))
								{
									iFlameCount++;
								}
							}
						}
					}
					else if (mBoard[num3, num2].mColor >= 0 && mBoard[num3, num2].mColor <= 7)
					{
						mBoard[mRow, mCol] = mBoard[num3, num2];
						mBoard[num3, num2] = piece;
						MoveAssistEval moveAssistEval = new MoveAssistEval();
						MoveAssistEval moveAssistEval2 = new MoveAssistEval();
						MoveAssistEval moveAssistEval3 = new MoveAssistEval();
						MoveAssistEval moveAssistEval4 = new MoveAssistEval();
						int largestSetAtRow = GetLargestSetAtRow(mRow, moveAssistEval);
						int largestSetAtRow2 = GetLargestSetAtRow(num3, moveAssistEval2);
						int largestSetAtCol = GetLargestSetAtCol(mCol, moveAssistEval3);
						int largestSetAtCol2 = GetLargestSetAtCol(num2, moveAssistEval4);
						int num4 = Math.Max(largestSetAtRow, largestSetAtRow2);
						int num5 = Math.Max(largestSetAtCol, largestSetAtCol2);
						if (num4 >= 3)
						{
							num += num4;
							iFlameCount = ((largestSetAtRow > largestSetAtRow2) ? moveAssistEval.Flame : moveAssistEval2.Flame);
							iLaserCount = ((largestSetAtRow > largestSetAtRow2) ? moveAssistEval.Laser : moveAssistEval2.Laser);
							iMultiCount = ((largestSetAtRow > largestSetAtRow2) ? moveAssistEval.Multiplier : moveAssistEval2.Multiplier);
						}
						if (num5 >= 3)
						{
							num += num5;
							iFlameCount = ((largestSetAtCol > largestSetAtCol2) ? moveAssistEval3.Flame : moveAssistEval4.Flame);
							iLaserCount = ((largestSetAtCol > largestSetAtCol2) ? moveAssistEval3.Laser : moveAssistEval4.Laser);
							iMultiCount = ((largestSetAtCol > largestSetAtCol2) ? moveAssistEval3.Multiplier : moveAssistEval4.Multiplier);
						}
						piece = mBoard[mRow, mCol];
						mBoard[mRow, mCol] = mBoard[num3, num2];
						mBoard[num3, num2] = piece;
					}
				}
			}
			return num;
		}

		public int GetLargestSetAtCol(int theCol, MoveAssistEval pEval)
		{
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < 8; i++)
			{
				Piece piece = mBoard[i, theCol];
				if (piece != null)
				{
					if (piece.mColor <= 7 && piece.mColor == num2)
					{
						num++;
						if (piece.IsFlagSet(1u))
						{
							num4++;
						}
						else if (piece.IsFlagSet(4u))
						{
							num5++;
						}
						else if (piece.IsFlagSet(16u))
						{
							num6++;
						}
						if (num > num3)
						{
							num3 = num;
							if (pEval != null)
							{
								pEval.Flame = num4;
								pEval.Laser = num5;
								pEval.Multiplier = num6;
							}
						}
					}
					else
					{
						num2 = piece.mColor;
						num = 1;
						num4 = (num5 = (num6 = 0));
					}
				}
				else
				{
					num4 = (num5 = (num6 = 0));
					num2 = -1;
				}
			}
			return num3;
		}

		public int GetLargestSetAtRow(int theRow, MoveAssistEval pEval)
		{
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < 8; i++)
			{
				Piece piece = mBoard[theRow, i];
				if (piece != null)
				{
					if (piece.mColor <= 7 && piece.mColor == num2)
					{
						num++;
						if (piece.IsFlagSet(1u))
						{
							num4++;
						}
						else if (piece.IsFlagSet(4u))
						{
							num5++;
						}
						else if (piece.IsFlagSet(16u))
						{
							num6++;
						}
						if (num > num3)
						{
							num3 = num;
							if (pEval != null)
							{
								pEval.Flame = num4;
								pEval.Laser = num5;
								pEval.Multiplier = num6;
							}
						}
					}
					else
					{
						num2 = piece.mColor;
						num = 1;
						num4 = (num5 = (num6 = 0));
					}
				}
				else
				{
					num2 = -1;
					num4 = (num5 = (num6 = 0));
				}
			}
			return num3;
		}

		public virtual void SliderReleased(int theId, double theVal)
		{
		}

		public virtual void DialogButtonPress(int theDialogId, int theButtonId)
		{
		}

		public void BOARD_MSG(string theMsg)
		{
			if (mMessager != null)
			{
				mMessager.AddMessage(theMsg);
			}
		}

		public void ShowAchievementHint(string achName)
		{
			mAchievementHints.Add(new AchievementHint(achName, OnAchievementHintFinished));
		}

		public void OnAchievementHintFinished(AchievementHint sender)
		{
			mAchievementHints.Remove(sender);
			mCurrentHint = null;
		}
	}
}
