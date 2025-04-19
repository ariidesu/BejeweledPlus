using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using BejeweledLivePlus.Audio;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Localization;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
// using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.GamerServices;
using SexyFramework;
using SexyFramework.Drivers.App;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Sound;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class BejeweledLivePlusApp : SexyApp, PopAnimListener, IDisposable
	{
		public delegate void InitStepFunc();

		public delegate bool ExtractResourceFunc(ResourceManager manager);

		public enum EAutoPlayState
		{
			AUTOPLAY_OFF,
			AUTOPLAY_RANDOM,
			AUTOPLAY_NO_DELAY,
			AUTOPLAY_NO_DELAY_WITH_INV,
			AUTOPLAY_TEST_HYPER,
			AUTOPLAY__COUNT
		}

		internal static List<string> mLoadedResourceGroups = new List<string>();

		public bool mGameCenterIsAvailable;

		private bool mWantExit;

		private int mTouchOffsetX;

		private int mTouchOffsetY;

		public PreCalculatedCurvedValManager mCurveValCache;

		private List<VoicePlayArgs> mPendingVoice = new List<VoicePlayArgs>();

		private bool mDisplayTitleUpdate;

		public string[] initialLoadGroups = new string[44]
		{
			"Common", "Fonts", "MainMenu", "GamePlay", "HyperspaceWhirlpool_Common", "HyperspaceWhirlpool_Normal", "AwardGlow", "GamePlay_UI_Normal", "GamePlay_UI_Dig_Common", "GamePlay_UI_Dig",
			"GamePlayQuest_Lightning", "GamePlayQuest_Dig", "GamePlayQuest_Butterfly_Common", "GamePlayQuest_Butterfly", "Badges", "BADGES_BIG_ELITE", "BADGES_BIG_BRONZE", "BADGES_BIG_SILVER", "BADGES_BIG_GOLD", "BADGES_BIG_PLATINUM",
			"BADGES_BIG_LEVELORD", "BADGES_BIG_BEJEWELER", "BADGES_BIG_DIAMOND_MINE", "BADGES_BIG_RELIC_HUNTER", "BADGES_BIG_ELECTRIFIER", "BADGES_BIG_HIGH_VOLTAGE", "BADGES_BIG_BUTTERFLY_MONARCH", "BADGES_BIG_BUTTERFLY_BONANZA", "BADGES_BIG_CHROMATIC", "BADGES_BIG_STELLAR",
			"BADGES_BIG_BLASTER", "BADGES_BIG_SUPERSTAR", "BADGES_BIG_CHAIN_REACTION", "BADGES_BIG_LUCKY_STREAK", "Help_Basic", "Help_Bfly", "Help_DiamondMine", "Help_Lightning", "Help_Bfly", "ProfilePic_0",
			"ZenOptions", "BadgesGrayIcon", "AtlasEx", null
		};

		private Dictionary<string, InitStepFunc> initSteps_;

		private Dictionary<string, ExtractResourceFunc> resExtract_;

		public bool mLosfocus;

		public EAutoPlayState mAutoPlay;

		public string mVersion = GlobalMembers.Version;

		public bool mGameInProgress;

		public InterfaceState mInterfaceState;

		public InterfaceState mPreviousInterfaceState;

		public GameMode mCurrentGameMode;

		public bool mShowBackground;

		public Bej3Widget[] mMenus = new Bej3Widget[20];

		public MainMenu mMainMenu;

		public int mAutoSerializeInterval;

		public int mAutoSerializeIntervalAmnesty;

		public int mAutoSerializeMode;

		public int mAutoLevelUpCount;

		public bool mAutoSerializeModeRandomized;

		public bool mDiamondMineFirstLaunch;

		public double mVoiceVolume = 0.85;

		public double mZenAmbientVolume = 0.85;

		public double mZenAmbientMusicVolume = 0.5;

		public double mZenBinauralVolume = 0.5;

		public double mZenBreathVolume = 0.5;

		public bool mIgnoreSound;

		public string mWebRoot = string.Empty;

		public bool mForcedWebRoot;

		public bool mReInit;

		public bool mWas3D;

		public bool mResChanged;

		public bool mResForced = true;

		public bool mJumpToZen;

		public bool mJumpToClassic;

		public bool mJumpToSpeed;

		public bool mAnimateBackground;

		public int mJumpToQuest;

		public int mArtRes;

		public int mPreInitArtRes;

		public int mPauseFrames;

		public bool mFocusedAfterLoad;

		public string mTestBkg = "";

		public string mForceBkg = "";

		//public Music mMusic;

		public SoundEffects mSoundPlayer;

		public SexyFramework.Misc.Buffer mStatsDumpBuffer = new SexyFramework.Misc.Buffer();

		public int[] mBoostCosts = new int[5];

		public string mLinkWarningLocation = string.Empty;

		public bool mDoFadeBackForDialogs;

		public bool mRegCodeNotNeeded;

		public bool mWantDataUpdateOnFocus;

		public int mWaitingForRegUpdateTicks;

		public string mBuyCoinsURL = string.Empty;

		public string mClientId = string.Empty;

		public BinauralManager mBinauralManager;

		public Dictionary<int, PIEffect>[] mQuestObjPIEffects = new Dictionary<int, PIEffect>[BejeweledLivePlusAppConstants.NUM_QUEST_SETS];

		public Image[,] mShrunkenGems = new Image[7, 15];

		public Dictionary<MemoryImage, MemoryImage> mAlphaImages = new Dictionary<MemoryImage, MemoryImage>();

		public Profile mProfile;

		public string mSwitchProfileName = string.Empty;

		public int mQuitCountdown;

		public HighScoreMgr mHighScoreMgr = new HighScoreMgr();

		public int mMaxGamesPerDay;

		public QuestDataParser mLastDataParser;

		public int mLastDataParserId;

		public QuestDataParser mDefaultQuestDataParser;

		public QuestDataParser mQuestDataParser;

		public QuestDataParser mSecretModeDataParser;

		public QuestDataParser mSpeedModeDataParser;

		public QuestMenu mQuestMenu;

		public SecretMenu mSecretMenu;

		public Background mBlitzBackground;

		public Background mBlitzBackgroundLo;

		public Board mBoard;

		public TooltipManager mTooltipManager;

		public SharedRenderTarget mRestartRT = new SharedRenderTarget();

		public SoundInstance mCurVoice;

		public int mCurVoiceId;

		public SoundInstance mNextVoice;

		public int mNextVoiceId;

		public bool mInterruptCurVoice;

		public string mLastUser = "";

		public bool mCreatingFBUser;

		public bool mNeedMusicStart;

		public Bej3P3DListener[] mGems3DListener = new Bej3P3DListener[7];

		public Mesh[] mGems3D = new Mesh[7];

		public Mesh mWarpTube3D;

		public Mesh mWarpTubeCap3D;

		public List<string> mAffirmationFiles = new List<string>();

		public List<string> mSBAFiles = new List<string>();

		public List<string> mAmbientFiles = new List<string>();

		public List<string> mTips = new List<string>();

		public List<string> mRankNames = new List<string>();

		public List<List<int>> mBadgeCutoffs = new List<List<int>>();

		public List<List<int>> mBadgeSecondaryCutoffs = new List<List<int>>();

		public UnderDialogWidget mUnderDialogWidget;

		public float mDialogObscurePct;

		public int mTipIdx;

		public string mQueuedStatsCalls = string.Empty;

		public bool mStatsStalled;

		public static bool mAllowRating;

		public static int mIdleTicksForButton;

		public CrystalBall mLatestClickedBall;

		private bool mIsDisposed;

		private List<Menu_Type>[] gInterfaceDefinitions = new List<Menu_Type>[17];

		private static bool mIsLoadingCompleted = false;

		private int SfxUpdateCount;

		private int LastSfxId = -1;

		public int ElapsedTime { get; set; }

		public bool WantExit
		{
			get
			{
				return mWantExit;
			}
			set
			{
				mWantExit = value;
			}
		}

		public BejeweledLivePlusApp(Game xnaGame)
		{
			GlobalMembers.gApp = this;
			GlobalMembers.gGameMain = xnaGame;
			((WP7AppDriver)mAppDriver).InitXNADriver(xnaGame);
			mFileDriver.InitFileDriver(this);
			GlobalMembers.gGR = new GraphicsRecorder();
			mArtRes = 960;
			mShowBackground = true;
			mDialogObscurePct = 0f;
			mDoFadeBackForDialogs = false;
		}

		// public void HandleGameUpdateRequired(GameUpdateRequiredException ex)
		// {
		// 	GameMain gameMain = (GameMain)GlobalMembers.gGameMain;
		// 	gameMain.GamerService.Enabled = false;
		// 	mDisplayTitleUpdate = true;
		// }

		private void DisplayTitleUpdate()
		{
			// if (!Guide.IsVisible)
			// {
			// 	mDisplayTitleUpdate = false;
			// 	List<string> list = new List<string>();
			// 	list.Add(GlobalMembers._ID("No", 7702));
			// 	list.Add(GlobalMembers._ID("Yes", 7701));
			// 	Guide.BeginShowMessageBox(GlobalMembers._ID("Title Update Available", 7703), GlobalMembers._ID("An update is available! This update is required to connect to Xbox LIVE. Update now?", 7704), list, 1, MessageBoxIcon.Alert, UpdateDialogGetResult, null);
			// }
		}

		protected void UpdateDialogGetResult(IAsyncResult result)
		{
			// int? num = Guide.EndShowMessageBox(result);
			// if (num.HasValue && num.Value > 0)
			// {
			// 	if (Guide.IsTrialMode)
			// 	{
			// 		Guide.ShowMarketplace(PlayerIndex.One);
			// 		return;
			// 	}
			// 	MarketplaceDetailTask val = new MarketplaceDetailTask();
			// 	val.ContentType = (MarketplaceContentType)1;
			// 	val.Show();
			// }
		}

		private void dumpStringResource()
		{
			string name = CultureInfo.CurrentCulture.Name;
			switch (name)
			{
			case "de-DE":
			case "es-ES":
			case "fr-FR":
			case "it-IT":
			case "en-US":
			{
				string fileName = $"Strings.{name}.resx";
				if (name == "en-US")
				{
					fileName = "Strings.resx";
				}
				mPopLoc.dumpLocalizedTextResource(fileName);
				break;
			}
			}
		}

		public override void Init()
		{
			base.Init();
			InitStepLocalization();
			InitStepLoadResources();
			InitStepPrepareCurvedVal();
			SetUpInterfaceStateDefinitions();
			while (!mResourceManager.IsResourceLoaded("IMAGE_LOADER_POPCAP_WHITE_GERMAN_REGISTERED"))
			{
				Thread.Sleep(200);
			}
			if (!GlobalMembersResourcesWP.ExtractInitResources(mResourceManager))
			{
				ShowResourceError(true);
			}
			if (!GlobalMembersResourcesWP.ExtractLoader_960Resources(mResourceManager))
			{
				ShowResourceError(true);
			}

			(mMusicInterface as CustomBassMusicInterface).ReadMusicXML();
			(mMusicInterface as CustomBassMusicInterface).QueueEvent("Play", "LoadingScreen", false);
			// mMusic = new Music(mMusicInterface);
			// mMusic.RegisterCallBack();
			mSoundPlayer = new SoundEffects(mSoundManager);
			// mMusic.LoadMusic(0, "music\\LoadingScreen");
			// mMusic.LoadMusic(1, "music\\MainMenu");
			mUnderDialogWidget = new UnderDialogWidget();
			mUnderDialogWidget.Resize(0, 0, mWidth, mHeight);
			mWidgetManager.AddWidget(mUnderDialogWidget);
			mUnderDialogWidget.SetVisible(false);
			mUnderDialogWidget.CreateImages();
			mMainMenu = new MainMenu();
			mMainMenu.Resize(new Rect(0, 0, mWidth, mHeight));
			mWidgetManager.AddWidget(mMainMenu);
			mWidgetManager.SetFocus(mMainMenu);
			mMenus[0] = new MenuBackground();
			mWidgetManager.AddWidget(mMenus[0]);
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_LOADING);
		}

		private void InitStepLocalization()
		{
			string text = "EN-US";
			text = ((Strings.Culture.TwoLetterISOLanguageName == "fr") ? "FR-FR" : ((Strings.Culture.TwoLetterISOLanguageName == "de") ? "DE-DE" : ((Strings.Culture.TwoLetterISOLanguageName == "es") ? "ES-ES" : ((!(Strings.Culture.TwoLetterISOLanguageName == "it")) ? "EN-US" : "IT-IT"))));
			mResourceManager.mCurLocSet = ((uint)text[0] << 24) | ((uint)text[1] << 16) | ((uint)text[3] << 8) | text[4];
			new LocalizedString();
		}

		private void InitStepLoadResources()
		{
			Res.InitResources(this);
			mResourceManager.mBaseArtRes = 1200;
			mResourceManager.mCurArtRes = 960;
			mResourceManager.PrepareLoadResourcesList(initialLoadGroups);
		}

		private void InitStepLoadProperties()
		{
			if (!LoadProperties(mResourceManager.GetLocaleFolder(true) + "properties\\defaultFramework.xml", true, false, false))
			{
				LoadProperties("properties\\defaultFramework.xml", true, false, false);
			}
			if (!LoadProperties(mResourceManager.GetLocaleFolder(true) + "properties\\defaultUIConstants.xml", true, false, false))
			{
				LoadProperties("properties\\defaultUIConstants.xml", true, false, false);
			}
			if (!LoadProperties(mResourceManager.GetLocaleFolder(true) + "properties\\defaultFilenames.xml", true, false, false))
			{
				LoadProperties("properties\\defaultFilenames.xml", true, false, false);
			}
		}

		private void InitStepLoadConfigs()
		{
			BejeweledLivePlus.Misc.Common.SRand((uint)DateTime.Now.ToFileTime());
			LoadConfigs();
		}

		private void InitStepPrepareCurvedVal()
		{
			mCurveValCache = new PreCalculatedCurvedValManager();
		}

		private void InitStepLoadExtraConfigs()
		{
			EncodingParser encodingParser = new EncodingParser();
			StringBuilder stringBuilder = new StringBuilder();
			char theChar = '\0';
			if (!encodingParser.OpenFile(mResourceManager.GetLocaleFolder(true) + "properties\\tips.txt") && !encodingParser.OpenFile("properties\\tips.txt"))
			{
				Popup("Failed to open properties\\tips.txt");
			}
			while (encodingParser.GetChar(ref theChar) == EncodingParser.GetCharReturnType.SUCCESSFUL || stringBuilder.Length > 0)
			{
				if (theChar == '\n' || theChar == '\r' || encodingParser.EndOfFile())
				{
					string text = stringBuilder.ToString().Trim();
					if (text.Length > 0)
					{
						mTips.Add(text);
					}
					stringBuilder.Clear();
				}
				else
				{
					stringBuilder.Append(theChar);
				}
			}
			EncodingParser encodingParser2 = new EncodingParser();
			if (!encodingParser2.OpenFile(mResourceManager.GetLocaleFolder(true) + "properties\\ranks.txt") && !encodingParser2.OpenFile("properties\\ranks.txt"))
			{
				Popup("Failed to open properties\\ranks.txt");
			}
			stringBuilder.Clear();
			while (encodingParser2.GetChar(ref theChar) == EncodingParser.GetCharReturnType.SUCCESSFUL || stringBuilder.Length > 0)
			{
				if (theChar == '\n' || theChar == '\r' || encodingParser2.EndOfFile())
				{
					string text2 = stringBuilder.ToString().Trim();
					if (text2.Length > 0)
					{
						mRankNames.Add(text2);
					}
					stringBuilder.Clear();
				}
				else
				{
					stringBuilder.Append(theChar);
				}
			}
		}

		private void InitStepLoadProfile()
		{
			mProfile = new Profile();
			string theLastPlayer = "Test";
			mProfile.ReadProfileList(ref theLastPlayer);
			RegistryReadString("LastUser", ref theLastPlayer);
			if (!mProfile.LoadProfile(theLastPlayer, false) && !mProfile.GetAnyProfile())
			{
				mProfile.CreateProfile("Player", true);
				mProfile.LoadProfile("Player", true);
			}
		}

		private void InitStepLoadHighScores()
		{
			LoadHighscores();
		}

		private void InitStepSetupMusics()
		{
			mAffirmationFiles.Add("General.txt");
			mAffirmationFiles.Add("Positive Thinking.txt");
			mAffirmationFiles.Add("Prosperity.txt");
			mAffirmationFiles.Add("Quit Bad Habits.txt");
			mAffirmationFiles.Add("Self Confidence.txt");
			mAffirmationFiles.Add("Weight Loss.txt");
			mAmbientFiles.Add("Coastal");
			mAmbientFiles.Add("Crickets");
			mAmbientFiles.Add("Forest");
			mAmbientFiles.Add("Ocean Surf");
			mAmbientFiles.Add("Rain Leaves");
			mAmbientFiles.Add("Waterfall");
			// mMusic.LoadMusic(2, "music\\Classic");
			// mMusic.LoadMusic(3, "music\\Classic_lose");
			// mMusic.LoadMusic(4, "music\\Zen");
			// mMusic.LoadMusic(13, "music\\Diamond_mine");
			// mMusic.LoadMusic(14, "music\\Diamond_mine_lose");
			// mMusic.LoadMusic(5, "music\\Butterfly");
			// mMusic.LoadMusic(6, "music\\Butterfly_lose");
			// mMusic.LoadMusic(11, "music\\Lightning");
			// mMusic.LoadMusic(12, "music\\Lightning_end");
		}

		private void InitStepSetupToolTipMgr()
		{
			mTooltipManager = new TooltipManager();
			mTooltipManager.Resize(0, 0, GlobalMembers.S(1600), GlobalMembers.S(1200));
			mTooltipManager.mZOrder = 10;
		}

		private void InitStepPreInitEffects()
		{
			PreInitEffects();
		}

		private void PrepareInitSteps()
		{
			if (initSteps_ == null)
			{
				initSteps_ = new Dictionary<string, InitStepFunc>();
				initSteps_["LOAD_PROPERTIES"] = InitStepLoadProperties;
				initSteps_["LOAD_CONFIGS"] = InitStepLoadConfigs;
				initSteps_["LOAD_EXTRACONFIGS"] = InitStepLoadExtraConfigs;
				initSteps_["LOAD_PROFILE"] = InitStepLoadProfile;
				initSteps_["LOAD_HIGHSCORES"] = InitStepLoadHighScores;
				initSteps_["SETUP_TOOLTIPMGR"] = InitStepSetupToolTipMgr;
				initSteps_["PREINIT_EFFECT"] = InitStepPreInitEffects;
				initSteps_["SETUP_MUSICS"] = InitStepSetupMusics;
			}
		}

		public void DoInitWhileLoading()
		{
			PrepareInitSteps();
			foreach (string key in initSteps_.Keys)
			{
				if (initSteps_[key] != null)
				{
					initSteps_[key]();
					initSteps_.Remove(key);
					break;
				}
			}
		}

		private void PrepareResExtractor()
		{
			if (resExtract_ == null)
			{
				resExtract_ = new Dictionary<string, ExtractResourceFunc>();
				resExtract_["IMAGE_ALPHA_ALPHA_UP"] = GlobalMembersResourcesWP.ExtractCommon_960Resources;
				resExtract_["IMAGE_LEVELBAR_ENDPIECE"] = GlobalMembersResourcesWP.ExtractGamePlay_UI_Normal_960Resources;
				resExtract_["IMAGE_GEMSNORMAL_BLUE"] = GlobalMembersResourcesWP.ExtractGamePlay_960Resources;
				resExtract_["IMAGE_ARROW_GLOW"] = GlobalMembersResourcesWP.ExtractMainMenu_960Resources;
				resExtract_["IMAGE_INGAMEUI_LIGHTNING_TIMER"] = GlobalMembersResourcesWP.ExtractGamePlayQuest_Lightning_960Resources;
				resExtract_["IMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE_COVER"] = GlobalMembersResourcesWP.ExtractHyperspaceWhirlpool_Common_960Resources;
				resExtract_["IMAGE_HYPERSPACE_WHIRLPOOL_HYPERSPACE_NORMAL"] = GlobalMembersResourcesWP.ExtractHyperspaceWhirlpool_Normal_960Resources;
				resExtract_["IMAGE_AWARD_GLOW"] = GlobalMembersResourcesWP.ExtractAwardGlow_960Resources;
				resExtract_["SOUND_ZEN_NECKLACE_4"] = GlobalMembersResourcesWP.ExtractCommon_CommonResources;
				resExtract_["POPANIM_FLAMEGEMEXPLODE"] = GlobalMembersResourcesWP.ExtractGamePlay_CommonResources;
				resExtract_["POPANIM_QUEST_DIG_COGS"] = GlobalMembersResourcesWP.ExtractGamePlay_UI_Dig_CommonResources;
				resExtract_["IMAGE_QUEST_DIG_COGS_COGS_96X96_2"] = GlobalMembersResourcesWP.ExtractGamePlay_UI_Dig_960Resources;
				resExtract_["IMAGE_WALLROCKS_SMALL_BROWN"] = GlobalMembersResourcesWP.ExtractGamePlayQuest_Dig_960Resources;
				resExtract_["POPANIM_ANIMS_LARGE_SPIDER"] = GlobalMembersResourcesWP.ExtractGamePlayQuest_Butterfly_CommonResources;
				resExtract_["IMAGE_ANIMS_LARGE_SPIDER_LARGE_SPIDER_71X36"] = GlobalMembersResourcesWP.ExtractGamePlayQuest_Butterfly_960Resources;
				resExtract_["SOUND_VOICE_WELCOMETOBEJEWELED"] = GlobalMembersResourcesWP.ExtractCommon_ENUSResources;
				resExtract_["IMAGE_BADGES_SMALL_UNKNOWN"] = GlobalMembersResourcesWP.ExtractBadges_960Resources;
				resExtract_["IMAGE_BADGES_BIG_ELITE"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_ELITE_960Resources;
				resExtract_["IMAGE_BADGES_BIG_BRONZE"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_BRONZE_960Resources;
				resExtract_["IMAGE_BADGES_BIG_SILVER"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_SILVER_960Resources;
				resExtract_["IMAGE_BADGES_BIG_GOLD"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_GOLD_960Resources;
				resExtract_["IMAGE_BADGES_BIG_PLATINUM"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_PLATINUM_960Resources;
				resExtract_["IMAGE_BADGES_BIG_LEVELORD"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_LEVELORD_960Resources;
				resExtract_["IMAGE_BADGES_BIG_BEJEWELER"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_BEJEWELER_960Resources;
				resExtract_["IMAGE_BADGES_BIG_DIAMOND_MINE"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_DIAMOND_MINE_960Resources;
				resExtract_["IMAGE_BADGES_BIG_RELIC_HUNTER"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_RELIC_HUNTER_960Resources;
				resExtract_["IMAGE_BADGES_BIG_ELECTRIFIER"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_ELECTRIFIER_960Resources;
				resExtract_["IMAGE_BADGES_BIG_HIGH_VOLTAGE"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_HIGH_VOLTAGE_960Resources;
				resExtract_["IMAGE_BADGES_BIG_BUTTERFLY_MONARCH"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_BUTTERFLY_MONARCH_960Resources;
				resExtract_["IMAGE_BADGES_BIG_BUTTERFLY_BONANZA"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_BUTTERFLY_BONANZA_960Resources;
				resExtract_["IMAGE_BADGES_BIG_CHROMATIC"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_CHROMATIC_960Resources;
				resExtract_["IMAGE_BADGES_BIG_STELLAR"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_STELLAR_960Resources;
				resExtract_["IMAGE_BADGES_BIG_BLASTER"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_BLASTER_960Resources;
				resExtract_["IMAGE_BADGES_BIG_SUPERSTAR"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_SUPERSTAR_960Resources;
				resExtract_["IMAGE_BADGES_BIG_CHAIN_REACTION"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_CHAIN_REACTION_960Resources;
				resExtract_["IMAGE_BADGES_BIG_LUCKY_STREAK"] = GlobalMembersResourcesWP.ExtractBADGES_BIG_LUCKY_STREAK_960Resources;
				resExtract_["POPANIM_HELP_STARGEM"] = GlobalMembersResourcesWP.ExtractHelp_Basic_CommonResources;
				resExtract_["POPANIM_HELP_BFLY_SPIDER"] = GlobalMembersResourcesWP.ExtractHelp_Bfly_CommonResources;
				resExtract_["POPANIM_HELP_DIAMOND_GOLD"] = GlobalMembersResourcesWP.ExtractHelp_DiamondMine_CommonResources;
				resExtract_["POPANIM_HELP_LIGHTNING_SPEED"] = GlobalMembersResourcesWP.ExtractHelp_Lightning_CommonResources;
				resExtract_["IMAGE_PP29"] = GlobalMembersResourcesWP.ExtractProfilePic_0Resources;
				resExtract_["IMAGE_ZEN_OPTIONS_WEIGHT_LOSS"] = GlobalMembersResourcesWP.ExtractZenOptions_960Resources;
				resExtract_["BADGES_GRAY_ICON"] = GlobalMembersResourcesWP.ExtractBadgesGrayIconResources;
				resExtract_["LR_LOADING"] = GlobalMembersResourcesWP.ExtractLRLoadingResources;
				resExtract_["ATLAS_EX"] = GlobalMembersResourcesWP.ExtractAtlasExResources;
			}
		}

		public void ExtractResources()
		{
			PrepareResExtractor();
			List<string> list = new List<string>();
			foreach (string key in resExtract_.Keys)
			{
				if (mResourceManager.IsResourceLoaded(key) && resExtract_[key] != null)
				{
					resExtract_[key](mResourceManager);
					list.Add(key);
				}
			}
			foreach (string item in list)
			{
				resExtract_.Remove(item);
			}
		}

		public void PreInitEffects()
		{
			Effect.initPool();
			PopAnimEffect.initPool();
			ParticleEffect.initPool();
			BlingParticleEffect.initPool();
			GemCollectEffect.initPool();
			LightningBarFillEffect.initPool();
			TimeBonusEffect.initPool();
			ButterflyEffect.initPool();
			CoinFlyEffect.initPool();
			PointsEffect.initPool();
			TextNotifyEffect.initPool();
			SpeedCollectEffect.initPool();
			TimeBonusEffect.batchInit();
		}

		public override bool Update(int gameTime)
		{
			ElapsedTime = gameTime;
			for (int i = 0; i < mPendingVoice.Count; i++)
			{
				bool flag = true;
				VoicePlayArgs voicePlayArgs = mPendingVoice[i];
				if (voicePlayArgs.Condition != null)
				{
					voicePlayArgs.Condition.update();
				}
				if (voicePlayArgs.Condition != null && !voicePlayArgs.Condition.shouldActivate())
				{
					flag = false;
				}
				if (flag)
				{
					PlayVoice(voicePlayArgs.SoundID, voicePlayArgs.Pan, voicePlayArgs.Volume, voicePlayArgs.InterruptID);
					mPendingVoice.RemoveAt(i);
					i--;
				}
			}
			return base.Update(gameTime);
		}

		public override void Draw(int gameTime)
		{
			base.Draw(gameTime);
			if (mDisplayTitleUpdate)
			{
				DisplayTitleUpdate();
			}
		}

		public void OnActivated()
		{
			if (mMusicInterface != null)
			{
				mMusicInterface.OnActived();
				mMusicInterface.ResumeAllMusic();
			}
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_PAUSEMENU && mMenus[7].mTargetPos == ConstantsWP.MENU_Y_POS_HIDDEN && mMenus[7].mTopButton.mType != Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
			{
				mMenus[7].ButtonDepress(10001);
			}
		}

		public void OnDeactivated()
		{
			if (mMusicInterface != null)
			{
				mMusicInterface.PauseAllMusic();
				mMusicInterface.OnDeactived();
			}
			if (mBoard != null && GlobalMembers.gApp.mMainMenu.mIsFullGame() && !mBoard.mGameFinished)
			{
				mBoard.SaveGame();
			}
			if (mProfile != null)
			{
				mProfile.WriteProfile();
				mProfile.WriteProfileList();
			}
			WriteToRegistry();
			if (GlobalMembers.gApp.GetDialog(18) != null || mBoard == null || mInterfaceState != InterfaceState.INTERFACE_STATE_INGAME)
			{
				return;
			}
			if (mBoard.mHyperspace != null || mBoard.mWantLevelup)
			{
				if (mBoard != null && GlobalMembers.gApp.mMainMenu.mIsFullGame() && !mBoard.mGameFinished)
				{
					mBoard.SaveGame();
				}
			}
			else
			{
				if (GlobalMembers.gApp.mDialogMap.Count != 0)
				{
					return;
				}
				if (mBoard.mInReplay)
				{
					mBoard.BackToGame();
					mMenus[7].ButtonDepress(10001);
				}
				else if (GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_ZEN || GlobalMembers.gApp.mMenus[19].mY == ConstantsWP.MENU_Y_POS_HIDDEN || GlobalMembers.gApp.mMenus[19].mY == 0)
				{
					Bej3Widget bej3Widget = GlobalMembers.gApp.mMenus[7];
					mLosfocus = true;
					Bej3Widget bej3Widget2 = mMenus[8];
					if (mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME && mInterfaceState != InterfaceState.INTERFACE_STATE_PAUSEMENU && mDialogMap.Count == 0 && (!mMenus[8].mVisible || !mBoard.mWantLevelup))
					{
						mMenus[7].ButtonDepress(10001);
					}
				}
			}
		}

		public void OnServiceActivated()
		{
			if (mMusicInterface != null)
			{
				mMusicInterface.OnServiceActived();
			}
		}

		public void OnServiceDeactivated()
		{
			if (mMusicInterface != null)
			{
				mMusicInterface.OnServiceDeactived();
			}
		}

		public void OnExiting()
		{
			if (GlobalMembers.gApp.mMainMenu.mIsFullGame())
			{
				if (mBoard != null && !mBoard.mGameFinished)
				{
					mBoard.SaveGame();
				}
			}
			else if (mBoard != null)
			{
				mBoard.DeleteSavedGame();
			}
			if (mProfile != null)
			{
				mProfile.WriteProfile();
				mProfile.WriteProfileList();
			}
			WriteToRegistry();
		}

		public void OnHardwardBackButtonPressed()
		{
			SexyFramework.GlobalMembers.IsBackButtonPressed = true;
		}

		public bool IsHardwareBackButtonPressed()
		{
			return SexyFramework.GlobalMembers.IsBackButtonPressed;
		}

		public void OnHardwareBackButtonPressProcessed()
		{
			SexyFramework.GlobalMembers.IsBackButtonPressed = false;
		}

		protected override void ReorderSystemButtonHandler(SystemButtons button, List<SexyFramework.Widget.Widget> handlers)
		{
			SexyFramework.Widget.Widget[] array = handlers.ToArray();
			SexyFramework.Widget.Widget widget = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (widget == null)
				{
					if (array[i] is MainMenu)
					{
						widget = array[i];
					}
				}
				else
				{
					SexyFramework.Widget.Widget widget2 = array[i];
					array[i] = widget;
					array[i - 1] = widget2;
				}
			}
			handlers.Clear();
			handlers.AddRange(array);
		}

		public void GetTouchInputOffset(ref int x, ref int y)
		{
			x = mTouchOffsetX;
			y = mTouchOffsetY;
		}

		public void SetTouchInputOffset(int x, int y)
		{
			mTouchOffsetX = x;
			mTouchOffsetY = y;
		}

		public void PopAnimPlaySample(string theSampleName, int thePan, double theVolume, double theNumSteps)
		{
		}

		public PIEffect PopAnimLoadParticleEffect(string theEffectName)
		{
			return null;
		}

		public bool PopAnimObjectPredraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, SexyFramework.Graphics.Color theColor)
		{
			return false;
		}

		public bool PopAnimObjectPostdraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, SexyFramework.Graphics.Color theColor)
		{
			return false;
		}

		public ImagePredrawResult PopAnimImagePredraw(int theId, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Image theImage, Graphics g, int theDrawCount)
		{
			return ImagePredrawResult.ImagePredraw_DontAsk;
		}

		public void PopAnimStopped(int theId)
		{
		}

		public void PopAnimCommand(int theId, string theCommand, string theParam)
		{
		}

		public bool PopAnimCommand(int theId, PASpriteInst theSpriteInst, string theCommand, string theParam)
		{
			return false;
		}

		public override void TouchBegan(Touch theTouch)
		{
			base.TouchBegan(theTouch);
			mIdleTicksForButton = 0;
		}

		public override void TouchEnded(Touch theTouch)
		{
			base.TouchEnded(theTouch);
			mIdleTicksForButton = 0;
		}

		public override void TouchMoved(Touch theTouch)
		{
			base.TouchMoved(theTouch);
			mIdleTicksForButton = 0;
		}

		public override void TouchesCanceled()
		{
			base.TouchesCanceled();
			mIdleTicksForButton = 0;
		}

		private void SetUpInterfaceStateDefinitions()
		{
			for (int i = 0; i < 20; i++)
			{
				mMenus[i] = null;
			}
			for (int j = 0; j < 17; j++)
			{
				gInterfaceDefinitions[j] = new List<Menu_Type>();
			}
			gInterfaceDefinitions[0].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[0].Add(Menu_Type.MENU_MAINMENU);
			gInterfaceDefinitions[1].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[1].Add(Menu_Type.MENU_MAINMENU);
			gInterfaceDefinitions[1].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[1].Add(Menu_Type.MENU_MAINMENUOPTIONSMENU);
			gInterfaceDefinitions[3].Add(Menu_Type.MENU_GAMEBOARD);
			gInterfaceDefinitions[3].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[3].Add(Menu_Type.MENU_GAMEDETAILMENU);
			gInterfaceDefinitions[4].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[4].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[4].Add(Menu_Type.MENU_BADGEMENU);
			gInterfaceDefinitions[6].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[6].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[6].Add(Menu_Type.MENU_STATSMENU);
			gInterfaceDefinitions[7].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[7].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[7].Add(Menu_Type.MENU_HIGHSCORESMENU);
			gInterfaceDefinitions[8].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[8].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[8].Add(Menu_Type.MENU_EDITPROFILEMENU);
			gInterfaceDefinitions[9].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[9].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[9].Add(Menu_Type.MENU_CREDITSMENU);
			gInterfaceDefinitions[10].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[10].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[10].Add(Menu_Type.MENU_ABOUTMENU);
			gInterfaceDefinitions[11].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[11].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[11].Add(Menu_Type.MENU_LEGALMENU);
			gInterfaceDefinitions[14].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[14].Add(Menu_Type.MENU_GAMEBOARD);
			gInterfaceDefinitions[14].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[14].Add(Menu_Type.MENU_HELPMENU);
			gInterfaceDefinitions[2].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[2].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[2].Add(Menu_Type.MENU_OPTIONSMENU);
			gInterfaceDefinitions[15].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[15].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[15].Add(Menu_Type.MENU_HELPANDOPTIONSMENU);
			gInterfaceDefinitions[13].Add(Menu_Type.MENU_GAMEBOARD);
			gInterfaceDefinitions[13].Add(Menu_Type.MENU_ZENOPTIONSMENU);
			gInterfaceDefinitions[5].Add(Menu_Type.MENU_BACKGROUND);
			gInterfaceDefinitions[5].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[5].Add(Menu_Type.MENU_PROFILEMENU);
			gInterfaceDefinitions[12].Add(Menu_Type.MENU_GAMEBOARD);
			gInterfaceDefinitions[12].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[12].Add(Menu_Type.MENU_PAUSEMENU);
			gInterfaceDefinitions[16].Add(Menu_Type.MENU_GAMEBOARD);
			gInterfaceDefinitions[16].Add(Menu_Type.MENU_FADE_UNDERLAY);
			gInterfaceDefinitions[16].Add(Menu_Type.MENU_PAUSEMENU);
		}

		private void SetContentSpecificConstants()
		{
			ConstantsWP.EDITWIDGET_HEIGHT = GlobalMembersResourcesWP.IMAGE_DIALOG_TEXTBOX.GetCelHeight();
		}

		private void DoNewBlitzGame(int theMinutes)
		{
		}

		private void DoNewClassicGame()
		{
			GlobalMembers.KILL_WIDGET_NOW(mBoard);
			mBoard = new ClassicBoard();
			mBoard.Resize(0, 0, mWidth, mHeight);
			mBoard.Init();
			if (!DoSavedGameCheck())
			{
				DoGameDetailMenu(GameMode.MODE_CLASSIC, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_PRE_GAME);
			}
			else
			{
				mNeedMusicStart = true;
			}
		}

		private void DoNewSpeedGame(int theId)
		{
			mLastDataParserId = theId;
			mLastDataParser = mSpeedModeDataParser;
			GlobalMembers.KILL_WIDGET_NOW(mBoard);
			mBoard = new SpeedBoard();
			mBoard.Resize(new Rect(0, 0, mWidth, mHeight));
			mBoard.mParams = mSpeedModeDataParser.mQuestDataVector[theId].mParams;
			mBoard.Init();
			if (!DoSavedGameCheck())
			{
				DoGameDetailMenu(GameMode.MODE_LIGHTNING, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_PRE_GAME);
			}
			else
			{
				mNeedMusicStart = true;
			}
		}

		private void DoNewZenGame()
		{
			mLastDataParserId = -1;
			mLastDataParser = null;
			GlobalMembers.KILL_WIDGET_NOW(mBoard);
			if (mBinauralManager == null)
			{
				mBinauralManager = new BinauralManager();
			}
			mBoard = new ZenBoard();
			mBoard.Resize(0, 0, mWidth, mHeight);
			mBoard.Init();
			StartSetupGame(true);
		}

		private void DoNewEndlessGame(EEndlessMode theId)
		{
			DoNewConfigGame((int)theId, mSecretModeDataParser, true);
		}

		private void DoNewConfigGame(int theId, QuestDataParser theParams, bool isPerprtual)
		{
			mLastDataParserId = theId;
			mLastDataParser = theParams;
			GlobalMembers.KILL_WIDGET(mSecretMenu);
			GlobalMembers.KILL_WIDGET_NOW(mBoard);
			switch (theParams.mQuestDataVector[theId].mParams["Challenge"].ToUpper())
			{
			case "BUTTERFLIES":
			case "BUTTERFLY":
				mBoard = new ButterflyBoard();
				break;
			case "DIG":
				mBoard = new DigBoard();
				break;
			}
			mBoard.mParams = theParams.mQuestDataVector[theId].mParams;
			mBoard.Resize(0, 0, mWidth, mHeight);
			QuestBoard questBoard = (QuestBoard)mBoard;
			questBoard.mQuestId = (isPerprtual ? 1000 : 0) + theId;
			questBoard.mIsPerpetual = isPerprtual;
			questBoard.mShowLevelPoints = !isPerprtual;
			mBoard.Init();
			if (!DoSavedGameCheck())
			{
				DoGameDetailMenu(mCurrentGameMode, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_PRE_GAME);
			}
			else
			{
				mNeedMusicStart = true;
			}
		}

		private void DoNewQuest(int theId)
		{
		}

		public bool HasClearedTutorial(int theTutorialFlag)
		{
			if ((mProfile.mTutorialFlags & (ulong)(1L << theTutorialFlag)) == 0)
			{
				return (mProfile.mTutorialFlags & 0x80000) != 0;
			}
			return true;
		}

		public void StartSetupGame(bool deleteSavedGame)
		{
			if (mCurrentGameMode == GameMode.MODE_ZEN)
			{
				deleteSavedGame = false;
			}
			if (deleteSavedGame)
			{
				mBoard.DeleteSavedGame();
			}
			bool flag = false;
			int num = 0;
			num = GetTutorialFlagsForMode(mCurrentGameMode);
			if (!HasClearedTutorial(num))
			{
				if (num == 22)
				{
					mDiamondMineFirstLaunch = true;
				}
				DoHelpDialog(num, 0);
			}
			else
			{
				mShowBackground = false;
				mWidgetManager.AddWidget(mBoard);
				mWidgetManager.SetFocus(mBoard);
				GoToInterfaceState(InterfaceState.INTERFACE_STATE_INGAME);
				mBoard.NewGame();
				if (mCurrentGameMode == GameMode.MODE_ZEN)
				{
					ZenBoard zenBoard = (ZenBoard)mBoard;
					zenBoard.LoadAffirmations();
					zenBoard.LoadAmbientSound();
					zenBoard.PlayZenNoise();
				}
				if (Bej3Widget.mCurrentSlidingMenu != null)
				{
					Bej3Widget.mCurrentSlidingMenu.mSlidingForDialog = null;
				}
				Bej3Widget.mCurrentSlidingMenu = mBoard;
				mGameInProgress = true;
			}
			if (mCurrentGameMode == GameMode.MODE_ZEN && !mProfile.HasClearedTutorial(23))
			{
				if (mInterfaceState == InterfaceState.INTERFACE_STATE_HELPMENU)
				{
					Bej3Widget.mCurrentSlidingMenu = mMenus[8];
				}
				else
				{
					Bej3Widget.mCurrentSlidingMenu = mMenus[7];
				}
				ZenInfoDialog theDialog = new ZenInfoDialog();
				AddDialog(44, theDialog);
				GlobalMembers.gApp.mProfile.mTutorialFlags ^= 8388608uL;
				if (Bej3Widget.mCurrentSlidingMenu != null)
				{
					Bej3Widget.mCurrentSlidingMenu.Transition_SlideOut();
				}
			}
			ClearUpdateBacklog(false);
		}

		public void DoGameDetailMenu(GameMode mode, GameDetailMenu.GAMEDETAILMENU_STATE state)
		{
			if (state == GameDetailMenu.GAMEDETAILMENU_STATE.STATE_PRE_GAME)
			{
				StartSetupGame(true);
				return;
			}
			GameDetailMenu gameDetailMenu = (GameDetailMenu)mMenus[6];
			gameDetailMenu.SetMode(mode, state);
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_GAMEDETAILMENU);
			if (state == GameDetailMenu.GAMEDETAILMENU_STATE.STATE_POST_GAME && mBoard != null)
			{
				gameDetailMenu.GetStatsFromBoard(mBoard);
			}
			if (state == GameDetailMenu.GAMEDETAILMENU_STATE.STATE_PRE_GAME && GetDialogCount() == 0)
			{
				mMenus[6].Transition_SlideThenFadeIn();
			}
		}

		public void DoHelpDialog(int tutorialFlags, int state)
		{
			HelpDialog helpDialog = (HelpDialog)mMenus[8];
			helpDialog.SetMode(tutorialFlags);
			helpDialog.SetHelpDialogState((HelpDialog.HELPDIALOG_STATE)state);
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_HELPMENU);
			if (state == 0)
			{
				mMenus[8].mY = ConstantsWP.MENU_Y_POS_HIDDEN;
				mMenus[8].Transition_SlideThenFadeIn();
				mMenus[8].AllowSlideIn(true, null);
			}
		}

		public void FindFiles(string theSearch, List<string> theFileVector)
		{
		}

		public void UpdateVoices()
		{
			if (mCurVoice != null)
			{
				double num = mCurVoice.GetVolume();
				if (mNextVoice != null && mInterruptCurVoice)
				{
					num = GlobalMembers.MAX(0.0, num - (double)GlobalMembers.M(0.05f));
					mCurVoice.SetVolume(num);
				}
				if (!mCurVoice.IsPlaying() || num == 0.0)
				{
					mCurVoice.Release();
					mCurVoice = null;
					mCurVoiceId = -1;
				}
			}
			if (mCurVoice == null && mNextVoice != null)
			{
				mCurVoice = mNextVoice;
				mCurVoiceId = mNextVoiceId;
				mNextVoice = null;
				mNextVoiceId = -1;
				mInterruptCurVoice = false;
				mCurVoice.Play(false, false);
			}
		}

		public void UpdateStatsCalls()
		{
		}

		public void KillStatsCall()
		{
		}

		public static void LoadContent(string theGroup, bool notifyMenus)
		{
		}

		public static void LoadContent(string theGroup)
		{
			LoadContent(theGroup, true);
		}

		public static void UnloadContent(string theGroup, bool force)
		{
		}

		public static void UnloadContent(string theGroup)
		{
			UnloadContent(theGroup, false);
		}

		public static void LoadContentInBackground(string theGroup)
		{
		}

		public static void WaitUntilGroupLoaded(string theGroup)
		{
		}

		public void ContentLoaded()
		{
		}

		public static void LoadContentQuestMenu(int category)
		{
		}

		public override void Dispose()
		{
			if (mIsDisposed)
			{
				return;
			}
			try
			{
				// mMusic.Dispose();
				// mMusic = null;
				mSoundPlayer.Dispose();
				mSoundPlayer = null;
				mProfile.Dispose();
				mProfile = null;
				if (mCurveValCache != null)
				{
					mCurveValCache = null;
				}
			}
			finally
			{
				mIsDisposed = true;
				GC.SuppressFinalize(this);
			}
		}

		public void DisableOptionsButtons(bool disable)
		{
			mMenus[7].SetDisabledTopButton(disable);
			if (mCurrentGameMode == GameMode.MODE_ZEN && mBoard != null)
			{
				((ZenBoard)mBoard).mZenOptionsButton.mMouseVisible = !disable;
				((ZenBoard)mBoard).mZenOptionsButton.SetDisabled(disable);
			}
		}

		public void DoBadgeMenu(int state, List<int> deferredBadgeVector)
		{
			BadgeMenu badgeMenu = (BadgeMenu)mMenus[11];
			badgeMenu.SetMode((BadgeMenu.BADGEMENU_STATE)state, deferredBadgeVector);
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_BADGEMENU);
			if (state == 1)
			{
				badgeMenu.ShowBackButton(false);
			}
		}

		public Dialog DoXBLErrorDialog()
		{
			return DoDialog(57, true, GlobalMembers._ID("ERROR", 4900), GlobalMembers._ID("Unable to connect to Xbox LIVE at this time. Please check your connection.", 4901), GlobalMembers._ID("OK", 414), 3);
		}

		public void DoProfileMenu()
		{
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_PROFILEMENU);
		}

		public void DoStatsMenu()
		{
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_STATSMENU);
		}

		public void DoHighScoresMenu()
		{
			mMenus[3].mVisible = true;
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_HIGHSCORESMENU);
		}

		public void DoEditProfileMenu(bool isFirstTime)
		{
			((EditProfileDialog)mMenus[15]).SetupForFirstShow(isFirstTime);
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_EDITPROFILEMENU);
		}

		public void DoEditProfileMenu()
		{
			DoEditProfileMenu(false);
		}

		public void DoOptionsMenu()
		{
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_OPTIONSMENU);
		}

		public void DoHelpAndOptionsMenu()
		{
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_HELPANDOPTIONSMENU);
		}

		public void DoMainMenu(bool openMainMenuOptions)
		{
			if (mBoard != null)
			{
				mBoard.mGameClosing = true;
				mBoard.UnloadContent();
				GlobalMembers.KILL_WIDGET_NOW(mBoard);
				mBoard = null;
			}
			if (mQuestMenu != null)
			{
				mQuestMenu.SetVisible(true);
			}
			mGameInProgress = false;
			Bej3Widget.SetOverlayType(Bej3Widget.OVERLAY_TYPE.OVERLAY_NONE);
			mShowBackground = true;
			((MainMenuOptions)mMenus[5]).mExpandOnShow = openMainMenuOptions;
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_MAINMENU);
		}

		public void DoMainMenu()
		{
			DoMainMenu(false);
		}

		public void DoCreditsMenu()
		{
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_CREDITSMENU);
		}

		public void DoAboutMenu()
		{
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_ABOUTMENU);
		}

		public void DoZenOptionsMenu()
		{
			mBoard.Pause();
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_ZENOPTIONSMENU);
		}

		public void DoPauseMenu()
		{
			PauseMenu pauseMenu = (PauseMenu)mMenus[7];
			pauseMenu.SetMode(mCurrentGameMode);
			mBoard.Pause();
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_PAUSEMENU);
		}

		public void DoLegalMenu()
		{
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_LEGALMENU);
		}

		public void GoBackToGame()
		{
			mBoard.Unpause();
			GoToInterfaceState(InterfaceState.INTERFACE_STATE_INGAME);
		}

		public string GetModeHeading(GameMode mode)
		{
			switch (mode)
			{
			case GameMode.MODE_CLASSIC:
				return GlobalMembers._ID("CLASSIC", 3208);
			case GameMode.MODE_ZEN:
				return GlobalMembers._ID("ZEN", 3209);
			case GameMode.MODE_DIAMOND_MINE:
				return GlobalMembers._ID("DIAMOND MINE", 3210);
			case GameMode.MODE_LIGHTNING:
				return GlobalMembers._ID("LIGHTNING", 3211);
			case GameMode.MODE_BUTTERFLY:
				return GlobalMembers._ID("BUTTERFLIES", 3212);
			case GameMode.MODE_POKER:
				return GlobalMembers._ID("POKER", 3213);
			case GameMode.MODE_ICESTORM:
				return GlobalMembers._ID("ICESTORM", 3214);
			default:
				return "";
			}
		}

		public string GetModeHint(GameMode mode)
		{
			switch (mode)
			{
			case GameMode.MODE_CLASSIC:
				return GlobalMembers._ID("Score as many points as possible until there are no more moves.", 3554);
			case GameMode.MODE_DIAMOND_MINE:
				return GlobalMembers._ID("Score as many points as you can before the time runs out.", 3555);
			case GameMode.MODE_ZEN:
				return GlobalMembers._ID("Relax body and mind in this endless mode.", 3556);
			case GameMode.MODE_LIGHTNING:
				return GlobalMembers._ID("Score as many points as possible until there are no more moves.", 3557);
			case GameMode.MODE_BUTTERFLY:
				return GlobalMembers._ID("Score as many points as you can before a Butterfly Gem reaches the top.", 3558);
			default:
				return string.Empty;
			}
		}

		public static int GetTutorialFlagsForMode(GameMode mode)
		{
			int result = 0;
			switch (mode)
			{
			case GameMode.MODE_CLASSIC:
				result = 21;
				break;
			case GameMode.MODE_ZEN:
				result = 0;
				break;
			case GameMode.MODE_DIAMOND_MINE:
				result = 22;
				break;
			case GameMode.MODE_LIGHTNING:
				result = 10;
				break;
			case GameMode.MODE_BUTTERFLY:
				result = 17;
				break;
			}
			return result;
		}

		public int ModeHeadingToGameMode(string theHeading)
		{
			for (int i = 0; i < 7; i++)
			{
				string modeHeading = GetModeHeading((GameMode)i);
				string highScoreTableId = GetHighScoreTableId(modeHeading);
				if (modeHeading == theHeading || highScoreTableId == theHeading)
				{
					return i;
				}
			}
			return 7;
		}

		public string GetHighScoreTableId(string theLocalisedTableName)
		{
			if (theLocalisedTableName == GlobalMembers._ID("CLASSIC", 3208))
			{
				return "CLASSIC";
			}
			if (theLocalisedTableName == GlobalMembers._ID("ZEN", 3209))
			{
				return "ZEN";
			}
			if (theLocalisedTableName == GlobalMembers._ID("DIAMOND MINE", 3210))
			{
				return "DIAMOND MINE";
			}
			if (theLocalisedTableName == GlobalMembers._ID("LIGHTNING", 3211))
			{
				return "LIGHTNING";
			}
			if (theLocalisedTableName == GlobalMembers._ID("BUTTERFLIES", 3212))
			{
				return "BUTTERFLIES";
			}
			return theLocalisedTableName;
		}

		public bool ChangeProfileName(string newName)
		{
			return false;
		}

		public void DisableAllExceptThis(SexyFramework.Widget.Widget allExceptThis, bool disableOthers)
		{
			for (int i = 0; i < 20; i++)
			{
				if (mMenus[i] != null && mMenus[i] != allExceptThis && mMenus[i].GetWidgetState() == Bej3WidgetState.STATE_IN)
				{
					mMenus[i].SetDisabled(disableOthers);
				}
			}
		}

		public override void LoadingThreadCompleted()
		{
		}

		public void LoadConfigs()
		{
			if (mQuestDataParser != null)
			{
				mQuestDataParser.Dispose();
			}
			if (mDefaultQuestDataParser != null)
			{
				mDefaultQuestDataParser.Dispose();
			}
			if (mSecretModeDataParser != null)
			{
				mSecretModeDataParser.Dispose();
			}
			if (mSpeedModeDataParser != null)
			{
				mSpeedModeDataParser.Dispose();
			}
			mQuestDataParser = new QuestDataParser();
			if (!mQuestDataParser.LoadDescriptor(mResourceManager.GetLocaleFolder(true) + "properties\\quest.cfg") && !mQuestDataParser.LoadDescriptor("properties\\quest.cfg"))
			{
				Popup(mQuestDataParser.mError);
			}
			mDefaultQuestDataParser = new QuestDataParser();
			if (!mDefaultQuestDataParser.LoadDescriptor(mResourceManager.GetLocaleFolder(true) + "properties\\defaultquest.cfg") && !mDefaultQuestDataParser.LoadDescriptor("properties\\defaultquest.cfg"))
			{
				Popup(mDefaultQuestDataParser.mError);
			}
			mSecretModeDataParser = new QuestDataParser();
			if (!mSecretModeDataParser.LoadDescriptor(mResourceManager.GetLocaleFolder(true) + "properties\\secret.cfg") && !mSecretModeDataParser.LoadDescriptor("properties\\secret.cfg"))
			{
				Popup(mSecretModeDataParser.mError);
			}
			mSpeedModeDataParser = new QuestDataParser();
			if (!mSpeedModeDataParser.LoadDescriptor(mResourceManager.GetLocaleFolder(true) + "properties\\speed.cfg") && !mSpeedModeDataParser.LoadDescriptor("properties\\speed.cfg"))
			{
				Popup(mSpeedModeDataParser.mError);
			}
		}

		public void LoadHighscores()
		{
		}

		public bool LoadTempleMeshes()
		{
			return false;
		}

		public void SaveHighscores(bool theForceSave)
		{
		}

		public void SaveHighscores()
		{
			SaveHighscores(false);
		}

		public MemoryImage GetOrCreateAlphaImage(MemoryImage theSrcImage)
		{
			return new MemoryImage();
		}

		public void LogStatString(string theEventString)
		{
		}

		public void QueueStatsCall(string theStatsCall)
		{
		}

		public int GetBoostIdx(string theName)
		{
			return -1;
		}

		public override void ShutdownHook()
		{
		}

		public override void WriteToRegistry()
		{
			base.WriteToRegistry();
			RegistryWriteInteger("MusicVolume", (int)(mMusicVolume * 100.0));
			RegistryWriteInteger("SfxVolume", (int)(mSfxVolume * 100.0));
			RegistryWriteInteger("Muted", (mMuteCount - mAutoMuteCount > 0) ? 1 : 0);
			RegistryWriteInteger("ScreenMode", (!mIsWindowed) ? 1 : 0);
			RegistryWriteInteger("PreferredX", mPreferredX);
			RegistryWriteInteger("PreferredY", mPreferredY);
			RegistryWriteInteger("PreferredWidth", mPreferredWidth);
			RegistryWriteInteger("PreferredHeight", mPreferredHeight);
			RegistryWriteInteger("CustomCursors", mCustomCursorsEnabled ? 1 : 0);
			RegistryWriteInteger("InProgress", 0);
			RegistryWriteBoolean("WaitForVSync", mWaitForVSync);
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			if (GlobalMembers.gApp.mProfile != null)
			{
				ulong num = ((GlobalMembers.gApp.mProfile.mLastFacebookId.Length > 0) ? ulong.Parse(GlobalMembers.gApp.mProfile.mLastFacebookId) : 0);
				buffer.WriteLong((long)num);
				buffer.WriteLong((long)num >> 32);
				buffer.WriteByte((byte)GlobalMembers.gApp.mProfile.mProfileList.Count);
				buffer.WriteShort((short)Math.Min(65535, mProfile.mStats[0] / 60));
				buffer.WriteShort((short)Math.Min(65535, GlobalMembers.gApp.mProfile.mOnlineGames));
				buffer.WriteShort((short)Math.Min(65535, GlobalMembers.gApp.mProfile.mOfflineGames));
				buffer.WriteLong(GlobalMembers.gApp.mProfile.mOfflineRankPoints / 1000);
				RegistryWriteData("GameData", buffer.GetDataPtr(), (ulong)buffer.GetDataLen());
			}
			if (mProfile != null)
			{
				RegistryWriteString("LastUser", mProfile.mProfileName);
			}
			if (mResChanged)
			{
				RegistryWriteInteger("ArtRes", mArtRes);
			}
			RegistryWriteInteger("VoiceVolume", (int)(mVoiceVolume * 100.0));
			RegistryWriteInteger("ZenAmbientVolume", (int)(mZenAmbientVolume * 100.0));
			RegistryWriteInteger("ZenAmbientMusicVolume", (int)(mZenAmbientMusicVolume * 100.0));
			RegistryWriteInteger("ZenBinauralVolume", (int)(mZenBinauralVolume * 100.0));
			RegistryWriteInteger("ZenBreathVolume", (int)(mZenBreathVolume * 100.0));
			RegistryWriteBoolean("RegCodeNotNeeded", mRegCodeNotNeeded);
			RegistryWriteBoolean("AnimateBackground", mAnimateBackground);
			RegistryWriteString("ClientId", mClientId);
			RegistryWriteInteger("TipIdx", mTipIdx);
			RegistrySave();
		}

		public override void ReadFromRegistry()
		{
			base.ReadFromRegistry();
			int theValue = 0;
			if (RegistryReadInteger("VoiceVolume", ref theValue))
			{
				mVoiceVolume = (double)theValue / 100.0;
			}
			if (RegistryReadInteger("ZenAmbientVolume", ref theValue))
			{
				mZenAmbientVolume = (double)theValue / 100.0;
			}
			if (RegistryReadInteger("ZenAmbientMusicVolume", ref theValue))
			{
				mZenAmbientMusicVolume = (double)theValue / 100.0;
			}
			if (RegistryReadInteger("ZenBinauralVolume", ref theValue))
			{
				mZenBinauralVolume = (double)theValue / 100.0;
			}
			if (RegistryReadInteger("ZenBreathVolume", ref theValue))
			{
				mZenBreathVolume = (double)theValue / 100.0;
			}
			RegistryReadBoolean("RegCodeNotNeeded", ref mRegCodeNotNeeded);
			RegistryReadBoolean("AnimateBackground", ref mAnimateBackground);
			RegistryReadString("ClientId", ref mClientId);
			RegistryReadInteger("TipIdx", ref mTipIdx);
		}

		public override Dialog NewDialog(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
		{
			Bej3Dialog bej3Dialog = new Bej3Dialog(theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
			bej3Dialog.SetHeaderFont(GlobalMembersResources.FONT_HUGE);
			bej3Dialog.SetLinesFont(GlobalMembersResources.FONT_DIALOG);
			bej3Dialog.SetButtonFont(GlobalMembersResources.FONT_DIALOG);
			return bej3Dialog;
		}

		public override void ModalOpen()
		{
		}

		public void AskQuit()
		{
		}

		public void UnlockEndlessMode(EEndlessMode theMode)
		{
		}

		public void QueueQuit()
		{
		}

		public bool DoSavedGameCheck()
		{
			if (!GlobalMembers.gApp.mMainMenu.mIsFullGame())
			{
				mBoard.DeleteSavedGame();
			}
			if (!mBoard.HasSavedGame())
			{
				return false;
			}
			Bej3Dialog bej3Dialog = (Bej3Dialog)DoDialog(20, true, GlobalMembers._ID("Resume?", 75), GlobalMembers._ID("Resume a saved game or start a new one ? Your saved game will be lost if you start a new one.", 3184), "", 2, 3, 3);
			mIgnoreSound = true;
			bej3Dialog.mYesButton.mLabel = GlobalMembers._ID("RESUME GAME", 3185);
			((Bej3Button)bej3Dialog.mYesButton).SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			bej3Dialog.mNoButton.mLabel = GlobalMembers._ID("NEW GAME", 3186);
			Bej3Button bej3Button = new Bej3Button(1002, bej3Dialog, Bej3ButtonType.BUTTON_TYPE_LONG);
			bej3Button.SetLabel(GlobalMembers._ID("CANCEL", 3187));
			bej3Button.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			bej3Dialog.AddButton(bej3Button);
			bej3Dialog.SetButtonPosition(bej3Dialog.mYesButton, 0);
			bej3Dialog.mWidth = ConstantsWP.RESUME_DIALOG_WIDTH;
			bej3Dialog.SizeToContent();
			if (bej3Dialog.mTopButton != null)
			{
				bej3Dialog.mTopButton.mId = 1002;
			}
			mWidgetManager.SetFocus(bej3Dialog);
			mIgnoreSound = false;
			return true;
		}

		public void DoPlayMenu()
		{
			DoMainMenu();
		}

		public void UserChanged()
		{
		}

		public void DoNewGame(GameMode mode)
		{
			mCurrentGameMode = mode;
			switch (mode)
			{
			case GameMode.MODE_CLASSIC:
				DoNewClassicGame();
				break;
			case GameMode.MODE_ZEN:
				DoNewZenGame();
				break;
			case GameMode.MODE_DIAMOND_MINE:
				DoNewEndlessGame(EEndlessMode.ENDLESS_GOLDRUSH);
				break;
			case GameMode.MODE_LIGHTNING:
				DoNewSpeedGame(0);
				break;
			case GameMode.MODE_BUTTERFLY:
				DoNewEndlessGame(EEndlessMode.ENDLESS_BUTTERFLY);
				break;
			}
		}

		public void ConfirmUserMusic()
		{
			string theDialogHeader = GlobalMembers._ID("MUSIC", 3601);
			string theDialogLines = GlobalMembers._ID("Do you want to interrupt your music and use game background music?", 7710);
			Bej3Dialog bej3Dialog = (Bej3Dialog)DoDialog(52, true, theDialogHeader, theDialogLines, string.Empty, 1);
			bej3Dialog.SetButtonPosition(bej3Dialog.mYesButton, 0);
			bej3Dialog.mWidth = ConstantsWP.RESUME_DIALOG_WIDTH;
			bej3Dialog.SizeToContent();
			mWidgetManager.SetFocus(bej3Dialog);
		}

		public void DoTrialDialog(int theId)
		{
			string theDialogLines = string.Empty;
			string theDialogHeader = GlobalMembers._ID("PROMPT", 3789);
			string mLabel = GlobalMembers._ID("BUY FULL GAME", 3795);
			switch (theId)
			{
			case 0:
			case 2:
			case 5:
				theDialogLines = GlobalMembers._ID("You can't proceed in with trial game ,would you like to buy the full game?", 3792);
				theDialogHeader = GlobalMembers._ID("End Of The Trial Version", 3796);
				break;
			case 1:
			case 3:
				theDialogLines = GlobalMembers._ID("You can't play this mode with trial game ,would you like to buy the full game?", 3793);
				break;
			case 8:
				theDialogLines = GlobalMembers._ID("You can't browse the achievements with trial game ,would you like to buy the full game?", 3794);
				break;
			case 7:
				theDialogLines = GlobalMembers._ID("The Leaderboards are only available in the full game.", 3801);
				mLabel = GlobalMembers._ID("UPDATE", 3802);
				break;
			}
			Bej3Dialog bej3Dialog = (Bej3Dialog)DoDialog(51, true, theDialogHeader, theDialogLines, "", 1);
			mIgnoreSound = true;
			bej3Dialog.mYesButton.mLabel = mLabel;
			bej3Dialog.mNoButton.mLabel = GlobalMembers._ID("CANCEL", 3239);
			((Bej3Button)bej3Dialog.mNoButton).SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			bej3Dialog.SetButtonPosition(bej3Dialog.mYesButton, 0);
			bej3Dialog.mWidth = ConstantsWP.RESUME_DIALOG_WIDTH;
			bej3Dialog.SizeToContent();
			Rect theRect = new Rect(bej3Dialog.mYesButton.mRect);
			theRect.mWidth += 37;
			bej3Dialog.mYesButton.Resize(theRect);
			theRect.mY = bej3Dialog.mNoButton.mY;
			bej3Dialog.mNoButton.Resize(theRect);
			mWidgetManager.SetFocus(bej3Dialog);
			mIgnoreSound = false;
		}

		public void GoToBlitz()
		{
		}

		public void DoSecretMenu()
		{
		}

		public void DoQuestMenu(bool killBoard)
		{
		}

		public void DoQuestMenu()
		{
			DoQuestMenu(true);
		}

		public void DoMenu(bool doMusic)
		{
		}

		public void DoMenu()
		{
			DoMenu(true);
		}

		public void DoWelcomeDialog()
		{
		}

		public void DoNewUserDialog()
		{
		}

		public void DoChangePictureDialog(bool allowCancel)
		{
		}

		public void DoUserSelectionDialog()
		{
		}

		public void DoRegisteredDialog()
		{
		}

		public void GoToInterfaceState(InterfaceState newState, bool onlyOrder)
		{
			mPreviousInterfaceState = mInterfaceState;
			mMenus[2] = mMainMenu;
			mMenus[1] = mBoard;
			mMenus[3] = mUnderDialogWidget;
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_MAINMENU)
			{
				mGameInProgress = false;
			}
			mInterfaceState = newState;
			int num = -1;
			bool flag = (mInterfaceState == InterfaceState.INTERFACE_STATE_GAMEDETAILMENU && ((GameDetailMenu)mMenus[6]).GetGameMenuState() == GameDetailMenu.GAMEDETAILMENU_STATE.STATE_PRE_GAME) || (mInterfaceState == InterfaceState.INTERFACE_STATE_BADGEMENU && ((BadgeMenu)mMenus[11]).GetState() == 0);
			for (int i = 0; i < 20; i++)
			{
				if (mMenus[i] == null)
				{
					continue;
				}
				mWidgetManager.BringToFront(mMenus[i]);
				if (onlyOrder)
				{
					continue;
				}
				bool flag2 = false;
				if ((i != 0 || mShowBackground) && (i != 1 || !flag))
				{
					for (int j = 0; j < Common.size(gInterfaceDefinitions[(int)newState]); j++)
					{
						if (gInterfaceDefinitions[(int)newState][j] == mMenus[i].mType)
						{
							num = i;
							flag2 = true;
							break;
						}
					}
				}
				mMenus[i].InterfaceStateChanged(newState);
				if (flag2)
				{
					mWidgetManager.SetFocus(mMenus[i]);
					mMenus[i].Show();
				}
				else
				{
					mMenus[i].AllowSlideIn(false, null);
					mMenus[i].LostFocus();
					mMenus[i].Hide();
				}
			}
			if (!onlyOrder && num >= 0 && mMenus[num] != null)
			{
				mWidgetManager.SetFocus(mMenus[num]);
			}
			ClearUpdateBacklog(false);
		}

		public void GoToInterfaceState(InterfaceState newState)
		{
			GoToInterfaceState(newState, false);
		}

		public bool TouchedToolTip(int x, int y)
		{
			return false;
		}

		public override void HandleCmdLineParam(string theParamName, string theParamValue)
		{
		}

		public override void PreDDInterfaceInitHook()
		{
		}

		public void PrepareForSoftwareRendering()
		{
		}

		public override void Set3DAcclerated(bool is3D, bool reinit)
		{
		}

		public override void Done3dTesting()
		{
		}

		public bool LoadingThreadLoadGroup(string group)
		{
			return false;
		}

		public bool IsLoadingCompleted()
		{
			return mIsLoadingCompleted;
		}

		public void DoLoadingThreadCompleted()
		{
			mNumLoadingThreadTasks = mResourceManager.GetNumResources("Common", true, true) + mResourceManager.GetNumResources("Fonts", true, true) + mResourceManager.GetNumResources("MainMenu", true, true) + mResourceManager.GetNumResources("GamePlay", true, true);
			LoadingThreadLoadGroup("Common");
			LoadingThreadLoadGroup("MainMenu");
			LoadingThreadLoadGroup("GamePlay");
			foreach (ExtractResourceFunc value in resExtract_.Values)
			{
				value?.Invoke(mResourceManager);
			}
			resExtract_.Clear();
			Resources.LinkUpFonts(mArtRes);
			SetContentSpecificConstants();
			mMenus[13] = new StatsMenu();
			mMenus[2] = mMainMenu;
			mMenus[11] = new BadgeMenu(true);
			mMenus[5] = new MainMenuOptions();
			mMenus[7] = new PauseMenu();
			mMenus[14] = new HighScoresMenu();
			mMenus[12] = new ProfileMenu();
			mMenus[9] = new OptionsMenu();
			mMenus[18] = new LegalMenu();
			mMenus[17] = new AboutMenu();
			mMenus[8] = new HelpDialog();
			mMenus[6] = new GameDetailMenu();
			mMenus[16] = new CreditsMenu();
			mMenus[15] = new EditProfileDialog();
			mMenus[19] = new ZenOptionsMenu();
			mMenus[10] = new HelpAndOptionsMenu();
			for (int i = 0; i < 20; i++)
			{
				if (mMenus[i] != null)
				{
					mWidgetManager.AddWidget(mMenus[i]);
				}
			}
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, new SexyFramework.Graphics.Color(0, 0, 0, 0));
			mIsLoadingCompleted = true;
		}

		public Bej3Dialog DoRenameUserDialog()
		{
			NewUserDialog newUserDialog = new NewUserDialog(true, true);
			newUserDialog.mOrigString = mProfile.mProfileName;
			newUserDialog.mNameWidget.SetText(((EditProfileDialog)GlobalMembers.gApp.mMenus[15]).mDisplayName);
			AddDialog(newUserDialog);
			return newUserDialog;
		}

		public override void SwitchScreenMode(bool wantWindowed, bool is3d, bool force)
		{
		}

		public void SwitchScreenMode(bool wantWindowed, bool is3d)
		{
			SwitchScreenMode(wantWindowed, is3d, false);
		}

		public void ResizeWindowedButton()
		{
		}

		public Bej3Dialog OpenURLWithWarning(string theURL)
		{
			mLinkWarningLocation = theURL;
			Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.DoDialog(53, true, GlobalMembers._ID("VISIT WEBSITE?", 3196), GlobalMembers._ID("Minimize Bejeweled Live + and go to external website?", 3197), "", 1, 3, 4);
			((Bej3Button)bej3Dialog.mYesButton).SetLabel(GlobalMembers._ID("VISIT WEBSITE", 3198));
			((Bej3Button)bej3Dialog.mNoButton).SetLabel(GlobalMembers._ID("CANCEL", 3199));
			bej3Dialog.SetButtonPosition(bej3Dialog.mYesButton, 0);
			return bej3Dialog;
		}

		public new bool OpenURL(string theURL, bool shutdownOnOpen)
		{
			mWantDataUpdateOnFocus = true;
			return base.OpenURL(theURL, shutdownOnOpen);
		}

		public virtual bool OpenURL(string theURL)
		{
			return OpenURL(theURL, false);
		}

		public void OpenLastConfirmedWebsite()
		{
			OpenURL(mLinkWarningLocation);
		}

		public override void GotFocus()
		{
		}

		public override void LostFocus()
		{
			bool flag = mBoard != null && mBoard.mGameOverCount == 0 && mGameInProgress;
			if (flag)
			{
				mBoard.SyncUnAwardedBadges(mProfile.mDeferredBadgeVector);
			}
			mProfile.WriteProfile();
			WriteToRegistry();
			if (flag)
			{
				mBoard.SaveGame();
			}
			base.LostFocus();
			if (mSoundManager != null)
			{
				mSoundManager.SetVolume(2, 0.0);
				mSoundManager.SetVolume(3, 0.0);
				mSoundManager.SetVolume(4, 0.0);
			}
			WP7AppDriver wP7AppDriver = mAppDriver as WP7AppDriver;
			if (wP7AppDriver != null && mGameInProgress && mBoard.WantsHideOnPause())
			{
				mBoard.mSuspendingGame = true;
			}
			if (mGameInProgress && mBoard.mSuspendingGame && mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME)
			{
				mBoard.Pause();
			}
			mMusicInterface.PauseAllMusic();
		}

		public virtual void AppEnteredBackground()
		{
		}

		public int GetCoinCount()
		{
			return -1;
		}

		public void AddToCoinCount(int theDelta)
		{
		}

		public int GetBoostCost(int theBoostId)
		{
			return -1;
		}

		public int GetRank()
		{
			return -1;
		}

		public long GetRankPoints()
		{
			return -1L;
		}

		public bool IsVoicePending(int soundId)
		{
			bool result = false;
			foreach (VoicePlayArgs item in mPendingVoice)
			{
				if (item.SoundID == soundId)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public void PlayVoice(VoicePlayArgs args)
		{
			mPendingVoice.Add(args);
		}

		public void PlayVoice(int theSoundId, int thePan, double theVolume, int theInterruptId)
		{
			if (mIgnoreSound)
			{
				return;
			}
			if (mNextVoice != null)
			{
				mNextVoice.Release();
			}
			if (mMuteCount <= 0 && !(mVoiceVolume <= 0.0))
			{
				mNextVoice = mSoundManager.GetSoundInstance(theSoundId);
				if (mNextVoice != null)
				{
					mNextVoice.SetMasterVolumeIdx(1);
					mNextVoice.SetVolume(theVolume * mVoiceVolume);
					mNextVoice.SetPan(thePan);
					mInterruptCurVoice = mCurVoiceId == theInterruptId || (mNextVoiceId == theInterruptId && mInterruptCurVoice) || theInterruptId == -2;
					mNextVoiceId = theSoundId;
				}
			}
		}

		public void PlayVoice(int theSoundId, int thePan, double theVolume)
		{
			PlayVoice(theSoundId, thePan, theVolume, -1);
		}

		public void PlayVoice(int theSoundId, int thePan)
		{
			PlayVoice(theSoundId, thePan, 1.0, -1);
		}

		public void PlayVoice(int theSoundId)
		{
			PlayVoice(theSoundId, 0, 1.0, -1);
		}

		public void BuyGame()
		{
			// Guide.ShowMarketplace(PlayerIndex.One);
		}

		public void PlaySample(int theSoundId, int thePan, double theVolume, double theNumSteps)
		{
			if (!mIgnoreSound && mSoundManager != null && !(theVolume <= 0.0))
			{
				SoundInstance soundInstance = mSoundManager.GetSoundInstance(theSoundId);
				if (soundInstance != null && !soundInstance.IsPlaying())
				{
					soundInstance.SetVolume((mMuteCount > 0) ? 0.0 : (theVolume * mSfxVolume));
					soundInstance.SetPan(thePan);
					soundInstance.AdjustPitch(theNumSteps);
					soundInstance.Play(false, true);
				}
			}
		}

		public void PlaySample(int theSoundId, int thePan, double theVolume)
		{
			PlaySample(theSoundId, thePan, theVolume, 0.0);
		}

		public override void PlaySample(int theSoundId, int thePan)
		{
			PlaySample(theSoundId, thePan, 1.0, 0.0);
		}

		public override void PlaySample(int theSoundId)
		{
			PlaySample(theSoundId, 0, 1.0, 0.0);
		}

		public override bool DebugKeyDown(int theKey)
		{
			return false;
		}

		public override bool IsUIOrientationAllowed(UI_ORIENTATION theOrientation)
		{
			return false;
		}

		public override void UpdateFrames()
		{
			// mMusic.Update();
			mMusicInterface.Update();
			mSoundPlayer.Update();
			UpdateVoices();
			base.UpdateFrames();
			if (mDoFadeBackForDialogs || (mDialogMap.Count > 0 && GetDialog(40) == null && GetDialog(18) == null && (GetDialog(1) == null || GetDialog(1).mButtonMode != 0)))
			{
				if (mDialogObscurePct < 1f && mDoFadeBackForDialogs)
				{
					mDialogObscurePct = Math.Min(1f, mDialogObscurePct + GlobalMembers.M(0.05f));
					if (!mUnderDialogWidget.mVisible)
					{
						mUnderDialogWidget.SetVisible(true);
						mUnderDialogWidget.mUpdateCnt = GlobalMembers.M(100);
					}
					else
					{
						mUnderDialogWidget.MarkDirty();
					}
				}
				else if (!mDoFadeBackForDialogs)
				{
					mDialogObscurePct = Math.Max(0f, mDialogObscurePct - GlobalMembers.M(0.04f));
				}
			}
			else
			{
				mDialogObscurePct = Math.Max(0f, mDialogObscurePct - GlobalMembers.M(0.04f));
				if (mDialogObscurePct == 0f && mUnderDialogWidget.mVisible)
				{
					mUnderDialogWidget.SetVisible(false);
				}
			}
			if ((mInterfaceState == InterfaceState.INTERFACE_STATE_PAUSEMENU || mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME) && mMenus[7].mTargetPos == ConstantsWP.MENU_Y_POS_HIDDEN && mMenus[7].mTopButton.mType == Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS && !mBoard.mReplayWasTutorial)
			{
				mMenus[7].ButtonDepress(10001);
			}
			Bej3Button.UpdateStatics();
		}

		public override void DialogButtonDepress(int theDialogId, int theButtonId)
		{
			Bej3Dialog bej3Dialog = (Bej3Dialog)GetDialog(theDialogId);
			if (theDialogId == 33)
			{
				switch (theButtonId)
				{
				case 1000:
					GlobalMembers.gApp.WantExit = true;
					bej3Dialog.Kill();
					return;
				case 1001:
					bej3Dialog.Kill();
					return;
				}
				bej3Dialog.mResult = int.MaxValue;
			}
			if (theDialogId == 57)
			{
				switch (mInterfaceState)
				{
				case InterfaceState.INTERFACE_STATE_GAMEDETAILMENU:
				{
					GameDetailMenu gameDetailMenu = mMenus[6] as GameDetailMenu;
					gameDetailMenu.mFinalY = 50;
					mMenus[6].ButtonDepress(2);
					break;
				}
				case InterfaceState.INTERFACE_STATE_HIGHSCORESMENU:
					mMenus[14].ButtonDepress(10001);
					DoMainMenu();
					break;
				}
			}
			if (theDialogId == 51)
			{
				if (theButtonId == 1000)
				{
					GoToInterfaceState(InterfaceState.INTERFACE_STATE_MAINMENU);
					DoMainMenu();
					((MainMenu)mMenus[2]).WantBuyGame();
				}
				if (mBoard != null)
				{
					mBoard.DeleteSavedGame();
					GoToInterfaceState(InterfaceState.INTERFACE_STATE_MAINMENU);
					DoMainMenu();
				}
			}
			if (theDialogId == 33 && theButtonId == 1000)
			{
				GlobalMembers.gApp.QueueQuit();
			}
			switch (theDialogId)
			{
			case 20:
				switch (theButtonId)
				{
				case 1002:
					GlobalMembers.KILL_WIDGET_NOW(mBoard);
					break;
				case 1001:
					DoGameDetailMenu(mCurrentGameMode, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_PRE_GAME);
					break;
				default:
					if (mLatestClickedBall != null)
					{
						mLatestClickedBall.StartInGameTransition(false);
					}
					mBoard.LoadContent(false);
					StartSetupGame(false);
					break;
				}
				break;
			case 53:
				if (theButtonId == 1000)
				{
					// WebBrowserTask val = new WebBrowserTask();
					// val.Uri = new Uri(mLinkWarningLocation);
					// val.Show();
				}
				break;
			case 52:
				// This won't work for now
				if (true) break;
				if (theButtonId == 1000)
				{
					mMusicInterface.stopUserMusic();
				}
				//GlobalMembers.gApp.mMusic.PlaySongNoDelay(1, true);
				break;
			}
			bej3Dialog.Kill();
		}

		public override void ButtonPress(int theId)
		{
			base.ButtonPress(theId);
		}

		public override void ButtonDepress(int theId)
		{
			base.ButtonDepress(theId);
			if (theId == 1001)
			{
				GlobalMembers.gApp.SwitchScreenMode(true, Is3DAccelerated());
			}
		}

		public void DrawWaiter(Graphics g, int theX, int theY, int theUpdateCnt, int theAlpha)
		{
		}

		public virtual bool ShouldReInit()
		{
			return mReInit;
		}

		public virtual bool FrameNeedsSwapScreenImage()
		{
			return false;
		}

		public virtual Dialog DoDialog(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode, int buttonType1, int buttonType2)
		{
			Bej3Dialog bej3Dialog = (Bej3Dialog)base.DoDialog(theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode);
			((Bej3Button)bej3Dialog.mYesButton)?.SetType((Bej3ButtonType)buttonType1);
			((Bej3Button)bej3Dialog.mNoButton)?.SetType((Bej3ButtonType)buttonType2);
			bej3Dialog.Resize(0, bej3Dialog.mY, mWidth, bej3Dialog.mHeight);
			return bej3Dialog;
		}

		public override Dialog DoDialog(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
		{
			return DoDialog(theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode, 3, 3);
		}

		public void EnableMusic(bool enable)
		{
		}

		public bool IsMusicEnabled()
		{
			return mMuteCount > 0;
		}

		public override void SetMusicVolume(double theVolume)
		{
			base.SetMusicVolume(theVolume);
		}

		public int GetSysFps()
		{
			return -1;
		}

		public int GetFrameStep()
		{
			return 1;
		}

		public long GetFrameTimer()
		{
			return -1L;
		}

		public override void Mute(bool autoMute)
		{
			base.Mute(autoMute);
			if (mBoard != null)
			{
				ZenBoard zenBoard = null;
				if (mBoard is ZenBoard)
				{
					zenBoard = mBoard as ZenBoard;
				}
				zenBoard?.MuteZenSounds();
			}
		}

		public void Mute()
		{
			Mute(false);
		}

		public override void Unmute(bool autoMute)
		{
			base.Unmute(autoMute);
			if (mBoard != null)
			{
				ZenBoard zenBoard = null;
				if (mBoard is ZenBoard)
				{
					zenBoard = mBoard as ZenBoard;
				}
				zenBoard?.UnmuteZenSounds();
			}
		}

		public void Unmute()
		{
			Unmute(false);
		}

		public bool IsKeyboardShowing()
		{
			return false;
		}

		public void DoRateGameDialog()
		{
		}

		public void DoGiftGameDialog()
		{
		}

		public void RateGame()
		{
		}

		public virtual void DrawSpecial(Graphics g)
		{
		}

		public void DoMoreGamesDialog()
		{
		}
	}
}
