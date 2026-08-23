using System;
using System.Collections.Generic;
using System.Globalization;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Sound;
using SexyFramework.Widget;

namespace BejeweledLivePlus
{
	public class PokerBoard : QuestBoard
	{
		public sealed class CardSlot
		{
			public int mCardIdx = -2;

			public int mCardScoreIdx = -2;

			public int mCardType = -1;

			public int mCardEffectState;

			public CurvedVal mFlipPct = new CurvedVal();

			public CurvedVal mDealPct = new CurvedVal();

			public PIEffect mCardEffect;

			public PIEffect mSecondaryCardEffect;
		}

		private sealed class PokerHandsExamplesContainer : Bej3WidgetBase
		{
			private static readonly ResourceId[] HAND_IMAGE_IDS =
			{
				ResourceId.IMAGE_INGAMEUI_POKER_PAIR_ID,
				ResourceId.IMAGE_INGAMEUI_POKER_SPECTRUM_ID,
				ResourceId.IMAGE_INGAMEUI_POKER_2_PAIR_ID,
				ResourceId.IMAGE_INGAMEUI_POKER_3_OF_A_KIND_ID,
				ResourceId.IMAGE_INGAMEUI_POKER_FULL_HOUSE_ID,
				ResourceId.IMAGE_INGAMEUI_POKER_4_OF_A_KIND_ID,
				ResourceId.IMAGE_INGAMEUI_POKER_FLUSH_ID
			};

			private static readonly string[] HAND_DESCRIPTIONS =
			{
				"A hand containing two", "matching cards.",
				"A hand containing five", "no matching cards.",
				"A hand containing two", "pairs of matching cards.",
				"A hand containing three", "matching cards.",
				"A hand containing three of", "a kind and a pair.",
				"A hand containing four", "matching cards.",
				"A hand in which all", "five cards are the same."
			};

			public PokerHandsExamplesContainer()
			{
				Resize(0, 0, GlobalMembers.S(7000), GlobalMembers.S(1000));
			}

			public override void Draw(Graphics g)
			{
				int skullBarYShift = ConstantsWP.POKER_SKULL_BAR_Y_SHIFT;
				Image[] handImages =
				{
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_PAIR,
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SPECTRUM,
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_2_PAIR,
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_3_OF_A_KIND,
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_FULL_HOUSE,
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_4_OF_A_KIND,
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_FLUSH
				};
				g.PushState();
				g.SetClipRect(
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_CONTAINER_ID)),
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_CONTAINER_ID) + skullBarYShift),
					GlobalMembers.S(7000),
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_EXAMPLE_CONTAINER.GetHeight());
				for (int i = 0; i <= 6; i++)
				{
					int pageX = 1000 * i;
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_EXAMPLE_CONTAINER,
						(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_CONTAINER_ID) + pageX),
						(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_CONTAINER_ID) + skullBarYShift));
					g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
					g.SetColor(Color.White);
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Color.White);
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, new Color(255, 0, 255));
					g.WriteString(GlobalMembers._ID(HAND_NAMES[i], 588 + i),
						GlobalMembers.S(pageX + 400), GlobalMembers.S(skullBarYShift + 250));
					g.SetFont(GlobalMembersResources.FONT_DIALOG);
					g.SetColor(Color.White);
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, Color.White);
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 1, new Color(0, 0, 0, 0));
					g.WriteString(GlobalMembers._ID(HAND_DESCRIPTIONS[i * 2], 5022 + i * 2),
						GlobalMembers.S(pageX + 400), GlobalMembers.S(skullBarYShift + 300));
					g.WriteString(GlobalMembers._ID(HAND_DESCRIPTIONS[i * 2 + 1], 5023 + i * 2),
						GlobalMembers.S(pageX + 400), GlobalMembers.S(skullBarYShift + 350));
				}
				for (int i = 0; i <= 6; i++)
				{
					g.DrawImage(handImages[i],
						(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(HAND_IMAGE_IDS[i]) + 1000 * i),
						(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(HAND_IMAGE_IDS[i]) + skullBarYShift));
				}
				g.PopState();
			}
		}

		private sealed class PokerHandsExamplesWidget : Bej3WidgetBase, Bej3ScrollWidgetListener, Bej3ButtonListener
		{
			private readonly Bej3ScrollWidget mScrollWidget;

			private readonly PokerHandsExamplesContainer mContainer;

			private readonly Bej3Button mLeftArrow;

			private readonly Bej3Button mRightArrow;

			private readonly Label mSwipeLabel;

			public PokerHandsExamplesWidget()
			{
				int skullBarYShift = ConstantsWP.POKER_SKULL_BAR_Y_SHIFT;
				mContainer = new PokerHandsExamplesContainer();
				mScrollWidget = new Bej3ScrollWidget(this, false);
				mScrollWidget.Resize(0, 0, GlobalMembers.S(1000), GlobalMembers.S(750));
				mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
				mScrollWidget.EnableBounce(true);
				mScrollWidget.EnablePaging(true);
				mScrollWidget.AddWidget(mContainer);
				mScrollWidget.SetPageHorizontal(0, false);
				AddWidget(mScrollWidget);

				mLeftArrow = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
				mLeftArrow.Resize(GlobalMembers.S(20), GlobalMembers.S(skullBarYShift + 620), 0, 0);
				AddWidget(mLeftArrow);
				mLeftArrow.SetVisible(false);

				mRightArrow = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
				mRightArrow.Resize(GlobalMembers.S(590), GlobalMembers.S(skullBarYShift + 620), 0, 0);
				AddWidget(mRightArrow);

				mSwipeLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("Swipe for more examples", 5041));
				if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "es")
				{
					mSwipeLabel.SetScale(0.8f);
				}
				mSwipeLabel.Resize(GlobalMembers.S(390), GlobalMembers.S(skullBarYShift + 650), 0, 0);
				AddWidget(mSwipeLabel);
				Resize(0, 0, GlobalMembers.S(1000), GlobalMembers.S(skullBarYShift + 750));
			}

			public override void Draw(Graphics g)
			{
				int skullBarYShift = ConstantsWP.POKER_SKULL_BAR_Y_SHIFT;
				g.PushState();
				g.SetClipRect(
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_BACKGROUND_ID)),
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_BACKGROUND_ID) + skullBarYShift),
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_EXAMPLE_BACKGROUND.GetWidth(),
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_EXAMPLE_BACKGROUND.GetHeight() +
					GlobalMembers.S(ConstantsWP.HANDS_EXAMPLES_WIDGET_HEIGHT_IPHONE5_EXTRA_HEIGHT));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_EXAMPLE_BACKGROUND,
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_BACKGROUND_ID)),
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_EXAMPLE_BACKGROUND_ID) + skullBarYShift));
				g.PopState();
			}

			public override void Update()
			{
				bool disabled = mScrollWidget.GetScrollVelocity().mX > 40.0f;
				mLeftArrow.SetDisabled(disabled);
				mRightArrow.SetDisabled(disabled);
				base.Update();
			}

			public void RefreshAssets()
			{
				mLeftArrow.SetType(Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
				mRightArrow.SetType(Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
				mSwipeLabel.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			}

			private void UpdateButtonsVisibility()
			{
				int page = mScrollWidget.GetPageHorizontal();
				mLeftArrow.SetVisible(page != 0);
				mRightArrow.SetVisible(page != mScrollWidget.GetPageCount() - 1);
			}

			public void PageChanged(Bej3ScrollWidget scrollWidget, int pageH, int pageV)
			{
				UpdateButtonsVisibility();
			}

			public void ScrollTargetReached(ScrollWidget scrollWidget)
			{
			}

			public void ScrollTargetInterrupted(ScrollWidget scrollWidget)
			{
			}

			public bool ButtonsEnabled()
			{
				return true;
			}

			public void ButtonDepress(int theId)
			{
				if (theId == 0)
				{
					mScrollWidget.SetPageHorizontal(mScrollWidget.GetPageHorizontal() - 1, true);
				}
				else if (theId == 1)
				{
					mScrollWidget.SetPageHorizontal(mScrollWidget.GetPageHorizontal() + 1, true);
				}
				UpdateButtonsVisibility();
			}

			public void ButtonPress(int theId)
			{
			}

			public void ButtonPress(int theId, int theClickCount)
			{
			}

			public void ButtonDownTick(int theId)
			{
			}

			public void ButtonMouseEnter(int theId)
			{
			}

			public void ButtonMouseLeave(int theId)
			{
			}

			public void ButtonMouseMove(int theId, int theX, int theY)
			{
			}
		}

		public readonly CardSlot[] mCardSlots = new CardSlot[5];

		public int mCurrentCardIdx;

		public CurvedVal mScoreHandPct = new CurvedVal();

		public int mScoreHandCardIdx;

		public CurvedVal mCardBulgePct = new CurvedVal();

		public CurvedVal mSkullScale = new CurvedVal();

		public int mMaxCountdown;

		public CurvedVal mSkullCrusherAnimPct = new CurvedVal();

		public CurvedVal mSkullBarLidPct = new CurvedVal();

		public int mGoal;

		public int mHandsLeft;

		public int mHandsDealt;

		public int mSkullSpawnCount;

		public int mSkullsBusted;

		public int mHandsTotal;

		public int mChipSoundDelay;

		public int mBestHandPts;

		public float mSkullBusterPct;

		public float mSkullBusterDisp;

		public int mSkullHand;

		public int mSkullMax;

		public float mSkullSpawnPct;

		public float mCountdownMultPerHand;

		public readonly List<int> mHandValues = new List<int>();

		public readonly List<float> mHandBuster = new List<float>();

		public readonly List<float> mHandCountdown = new List<float>();

		public int mNumCoinFlips;

		public readonly int[] mHandCount = new int[7];

		public float mHandBusterScale = 1.0f;

		public int mFlameBonus;

		public int mStarBonus;

		public int mSuperBonus;

		public int mPendingFlameCount;

		public int mPendingStarCount;

		public float mHandAnimTimer;

		public bool mScoreTextVisible = true;

		public bool mHandWindowActive;

		public bool mWasTutorialDialogActive;

		public bool mTobleroneEnabled = true;

		public bool mSkullWindowState;

		public bool mSkullTutorialShown;

		public float mTobleronePct;

		public int mTobleroneDirection = 1;

		public int mTobleroneTimer;

		public bool mTobleroneFlip = true;

		public int mTobleroneTarget;

		public string mScoreName = string.Empty;

		public int mScoreTally;

		public int mCurrentHandIdx;

		public int mFlameMoveCreditId;

		public int mLaserMoveCreditId;

		public CurvedVal mCoinFlipPct = new CurvedVal();

		public CurvedVal mCoinWonPct = new CurvedVal();

		public bool mBadFlip;

		public bool mCoinFlipFinished;

		public bool mDrawCardsOverlay;

		public bool mAllCardsFlipped;

		public bool mCoinSoundActive;

		public SoundInstance mCoinSound;

		public PIEffect mPokerLevelBarPIEffect;

		public Bej3Button mInfoButton;

		public Bej3Button mResumeButton;

		public Bej3Button mExampleButton;

		public bool mButtonState0;

		public bool mButtonState1;

		public bool mHandWindowModal;

		public ParticleEffect mSkullExplodeEffect;

		private PokerHandsExamplesWidget mHandsExamplesWidget;

		private static readonly string[] HAND_NAMES =
		{
			"Pair", "Spectrum", "2 Pair", "3 of a Kind", "Full House", "4 of a Kind", "Flush"
		};

		public PokerBoard()
		{
			for (int i = 0; i < mCardSlots.Length; i++)
			{
				mCardSlots[i] = new CardSlot();
			}
			mUiConfig = EUIConfig.eUIConfig_Quest;
			mCountdownMultPerHand = 0.15f;
			mHandBusterScale = 1.0f;
			mScoreTextVisible = true;
			mTobleroneDirection = 1;
			mTobleroneFlip = true;
		}

		public override void Dispose()
		{
			RestoreSharedFontState();
			((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			GlobalMembers.gApp.DisableOptionsButtons(false);
			if (mPokerLevelBarPIEffect != null)
			{
				mPokerLevelBarPIEffect.Dispose();
				mPokerLevelBarPIEffect = null;
			}
			base.Dispose();
		}

		public override void BackToMenu()
		{
			RestoreSharedFontState();
			((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			GlobalMembers.gApp.DisableOptionsButtons(false);
			base.BackToMenu();
		}

		private static void RestoreSharedFontState()
		{
			ImageFont headerFont = (ImageFont)GlobalMembersResources.FONT_HUGE;
			Utils.SetFontLayerColor(headerFont, 0, Color.White);
			Utils.SetFontLayerColor(headerFont, 1, Color.White);
		}

		public override void LoadContent(bool threaded)
		{
			base.LoadContent(threaded);
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlay_UI_Normal");
				BejeweledLivePlusApp.LoadContentInBackground("GamePlayQuest_Poker");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlay_UI_Normal");
				BejeweledLivePlusApp.LoadContent("GamePlayQuest_Poker");
				GlobalMembersResourcesWP.PIEFFECT_DISCOBALL.mEmitAfterTimeline = true;
				GlobalMembersResourcesWP.PIEFFECT_STARBURST.mEmitAfterTimeline = true;
				if (mPokerLevelBarPIEffect != null)
				{
					mPokerLevelBarPIEffect.Dispose();
					mPokerLevelBarPIEffect = null;
				}
				mPokerLevelBarPIEffect = GlobalMembersResourcesWP.PIEFFECT_LEVELBAR.Duplicate();
				mPokerLevelBarPIEffect.mDrawBlockers = true;
				mPokerLevelBarPIEffect.mEmitAfterTimeline = true;
				mPokerLevelBarPIEffect.mDrawTransform.LoadIdentity();
				mPokerLevelBarPIEffect.mDrawTransform.Scale(1.0f, 1.0f);
				Rect skullBarRect = new Rect(
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_BAR_BACKGROUND_ID)),
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_BAR_BACKGROUND_ID) +
						ConstantsWP.POKER_SKULL_BAR_Y_SHIFT + GetPokerUIYOffset()),
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SKULL_BAR_BACKGROUND.GetWidth(),
					GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SKULL_BAR_BACKGROUND.GetHeight());
				mPokerLevelBarPIEffect.mDrawTransform.Translate(skullBarRect.mX, skullBarRect.mY);
				for (int i = 0; i < 2; i++)
				{
					PILayer layer = mPokerLevelBarPIEffect.GetLayer(i);
					PIDeflector deflector = layer.mLayerDef.mDeflectorVector[0];
					deflector.mPoints[0].mValuePoint2DVector[0].mValue =
						new FPoint(skullBarRect.mWidth, 0.0f).ToXnaVector2();
					deflector.mPoints[2].mValuePoint2DVector[0].mValue =
						new FPoint(0.0f, skullBarRect.mHeight).ToXnaVector2();
					deflector.mPoints[3].mValuePoint2DVector[0].mValue =
						new FPoint(skullBarRect.mWidth, skullBarRect.mHeight).ToXnaVector2();
				}
			}
		}

		public override void UnloadContent()
		{
			if (mPokerLevelBarPIEffect != null)
			{
				mPokerLevelBarPIEffect.Dispose();
				mPokerLevelBarPIEffect = null;
			}
			base.UnloadContent();
			BejeweledLivePlusApp.UnloadContent("GamePlay_UI_Normal");
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_Poker");
		}

		public override void Init()
		{
			if (mIsPerpetual)
			{
				mUiConfig = EUIConfig.eUIConfig_StandardNoReplay;
			}
			base.Init();
			if (mInfoButton == null)
			{
				mInfoButton = new Bej3Button(8, this, Bej3ButtonType.BUTTON_TYPE_CUSTOM);
				mInfoButton.mPlayPressSound = false;
				AddWidget(mInfoButton);
			}
			if (mResumeButton == null)
			{
				mResumeButton = new Bej3Button(9, this, Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
				mResumeButton.mPlayPressSound = false;
				mResumeButton.SetLabel(GlobalMembers._ID("RESUME", 5040));
				AddWidget(mResumeButton);
			}
			if (mExampleButton == null)
			{
				mExampleButton = new Bej3Button(10, this, Bej3ButtonType.BUTTON_TYPE_LONG);
				mExampleButton.mPlayPressSound = false;
				mExampleButton.SetLabel(GlobalMembers._ID("EXAMPLES", 5038));
				AddWidget(mExampleButton);
			}
			if (mHandsExamplesWidget == null)
			{
				mHandsExamplesWidget = new PokerHandsExamplesWidget();
				AddWidget(mHandsExamplesWidget);
			}
			mResumeButton.mVisible = false;
			mExampleButton.mVisible = false;
			mInfoButton.mVisible = true;
			mHandsExamplesWidget.SetVisible(false);
			mWantShowPoints = mIsPerpetual;
			mHighScoreIsLevelPoints = false;
			mShowLevelPoints = !mIsPerpetual;
		}

		public override void SetupBackground(int theDeltaIdx)
		{
			SetBackground($"images\\{GlobalMembers.gApp.mArtRes}\\backgrounds\\poker");
		}

		private Point GetTopWidgetPos()
		{
			return new Point(ConstantsWP.POKER_CRUSHER_BAR_X,
				ConstantsWP.POKER_CRUSHER_BAR_Y + GetPokerUIYOffset());
		}

		private int GetPokerUIYOffset()
		{
			return GetBoardY() - ConstantsWP.POKER_BOARD_Y;
		}

		private static int GetVirtualTop()
		{
			return (int)ConstantsWP.DEVICE_VIRTUAL_NEGATIVE_HEIGHT_F;
		}

		private static int GetVirtualBottom()
		{
			return (int)ConstantsWP.DEVICE_VIRTUAL_HEIGHT_F;
		}

		public override void InitUI()
		{
			base.InitUI();
			mHintButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG);
			mHintButton.SetLabel(GlobalMembers._ID("HINT", 3220));
			mHintButton.mPlayPressSound = false;
			if (mHelpButton != null)
			{
				mHelpButton.mBtnNoDraw = true;
				mHelpButton.mVisible = false;
			}
			if (mResetButton != null)
			{
				mResetButton.mBtnNoDraw = true;
				mResetButton.mVisible = false;
				mResetButton.mMouseVisible = false;
			}
		}

		public override void RefreshUI()
		{
			base.RefreshUI();
			mHintButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG);
			mHintButton.SetBorderGlow(true);
			mHintButton.Resize(ConstantsWP.BOARD_UI_HINT_BTN_X, ConstantsWP.BOARD_UI_HINT_BTN_Y,
				ConstantsWP.BOARD_UI_HINT_BTN_WIDTH, 0);
			mInfoButton.SetupCustomButton(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_INFO,
				GlobalMembers.S(ConstantsWP.POKER_INFO_BUTTON_X),
				GlobalMembers.S(ConstantsWP.POKER_INFO_BUTTON_Y + ConstantsWP.POKER_SKULL_BAR_Y_SHIFT +
					GetPokerUIYOffset()));
			mResumeButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			mExampleButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG);
			if (mHandsExamplesWidget != null)
			{
				mHandsExamplesWidget.RefreshAssets();
				mHandsExamplesWidget.Resize(0, GlobalMembers.S(GetPokerUIYOffset()), GlobalMembers.S(1000),
					GlobalMembers.S(ConstantsWP.POKER_SKULL_BAR_Y_SHIFT + 750));
				mHandsExamplesWidget.SetVisible(false);
			}
			mResumeButton.Resize(GlobalMembers.S(ConstantsWP.POKER_RESUME_BUTTON_X),
				GlobalMembers.S(ConstantsWP.POKER_RESUME_BUTTON_Y + GetPokerUIYOffset()),
				GlobalMembers.S(ConstantsWP.POKER_BUTTON_WIDTH), GlobalMembers.S(ConstantsWP.POKER_BUTTON_HEIGHT));
			mResumeButton.SetVisible(false);
			mExampleButton.Resize(GlobalMembers.S(ConstantsWP.POKER_EXAMPLE_BUTTON_X),
				GlobalMembers.S(ConstantsWP.POKER_EXAMPLE_BUTTON_Y + GetPokerUIYOffset()),
				GlobalMembers.S(ConstantsWP.POKER_BUTTON_WIDTH), GlobalMembers.S(ConstantsWP.POKER_BUTTON_HEIGHT));
			mExampleButton.SetVisible(false);
		}

		public override void ButtonDepress(int theId)
		{
			base.ButtonDepress(theId);
			switch (theId)
			{
			case 8:
				ActivateHandWindow(false);
				break;
			case 9:
				DeactivateHandWindow();
				((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
				GlobalMembers.gApp.DisableOptionsButtons(false);
				break;
			case 10:
				ToggleHandExamples();
				break;
			}
		}

		private void ActivateHandWindow(bool modal)
		{
			if (mGameFinished)
			{
				return;
			}
			mHandWindowModal = modal;
			mHandWindowActive = true;
			mButtonState0 = true;
			mButtonState1 = false;
			mInfoButton.SetDisabled(true);
			mHintButton.SetDisabled(true);
			mResumeButton.SetVisible(true);
			mExampleButton.SetVisible(true);
			mExampleButton.SetLabel(GlobalMembers._ID("EXAMPLES", 5038));
			mHandsExamplesWidget.SetVisible(false);
			if (mHandWindowModal)
			{
				GlobalMembers.gApp.DisableOptionsButtons(true);
				((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
			}
		}

		private void DeactivateHandWindow()
		{
			mHandWindowActive = false;
			mButtonState0 = false;
			mButtonState1 = false;
			mInfoButton.SetDisabled(false);
			mHintButton.SetDisabled(false);
			mResumeButton.SetVisible(false);
			mExampleButton.SetVisible(false);
			mSkullExplodeEffect = null;
			mHandsExamplesWidget.SetVisible(false);
			if (mHandWindowModal)
			{
				((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
				GlobalMembers.gApp.DisableOptionsButtons(false);
			}
		}

		private void ToggleHandExamples()
		{
			if (mButtonState1)
			{
				mExampleButton.SetLabel(GlobalMembers._ID("EXAMPLES", 5038));
				mExampleButton.mTextScale = 1f;
				mHandsExamplesWidget.SetVisible(false);
				mButtonState1 = false;
				mButtonState0 = true;
			}
			else
			{
				mExampleButton.SetLabel(GlobalMembers._ID("HANDS LIST", 5039));
				string language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
				mExampleButton.mTextScale = language == "fr" || language == "es" ? 0.8f :
					(language == "it" || language == "de" ? 0.75f : 1f);
				mHandsExamplesWidget.SetVisible(true);
				mButtonState1 = true;
				mButtonState0 = false;
			}
		}

		public override void NewGame(bool restartingGame)
		{
			if (mCoinSound != null)
			{
				mCoinSound.Release();
				mCoinSound = null;
			}
			mGoal = SexyFramework.GlobalMembers.sexyatoi(mParams, "Goal");
			mHandsLeft = SexyFramework.GlobalMembers.sexyatoi(mParams, "Hands");
			mHandsTotal = mHandsLeft;
			int startingMoney = SexyFramework.GlobalMembers.sexyatoi(mParams, "StartingMoney");
			mFlameBonus = SexyFramework.GlobalMembers.sexyatoi(mParams, "FlameBonus");
			mStarBonus = SexyFramework.GlobalMembers.sexyatoi(mParams, "StarBonus");
			mSuperBonus = SexyFramework.GlobalMembers.sexyatoi(mParams, "SuperBonus");
			mSkullMax = SexyFramework.GlobalMembers.sexyatoi(mParams, "SkullMax");
			mHandValues.Clear();
			mHandBuster.Clear();
			mHandCountdown.Clear();
			Utils.SplitAndConvertStr(mParams["HandValues"], mHandValues, ',', true, -1);
			Utils.SplitAndConvertStr(mParams["HandBuster"], mHandBuster, ',', true, -1);
			Utils.SplitAndConvertStr(mParams["HandCountdown"], mHandCountdown, ',', true, -1);
			for (int i = 0; i < Math.Min(6, mHandBuster.Count); i++)
			{
				mHandBuster[i] *= mHandBusterScale;
			}
			mChipSoundDelay = 0;
			mCurrentCardIdx = 0;
			mCurrentHandIdx = -1;
			mScoreHandCardIdx = -1;
			mScoreHandPct.SetConstant(0.0);
			mCardBulgePct.SetConstant(0.0);
			mSkullScale.SetConstant(0.0);
			mSkullCrusherAnimPct.SetConstant(0.0);
			mSkullBarLidPct.SetConstant(0.0);
			mCoinFlipPct.SetConstant(0.0);
			mCoinWonPct.SetConstant(0.0);
			mHandsDealt = 0;
			mSkullSpawnCount = 0;
			mSkullsBusted = 0;
			mBestHandPts = 0;
			mSkullHand = -1;
			mFlameMoveCreditId = -1;
			mLaserMoveCreditId = -1;
			mNumCoinFlips = 0;
			mPendingFlameCount = 0;
			mPendingStarCount = 0;
			mHandAnimTimer = 0.0f;
			mSkullBusterPct = 0f;
			mSkullBusterDisp = 0f;
			mSkullSpawnPct = 0f;
			mScoreTally = 0;
			mScoreName = string.Empty;
			mSkullWindowState = false;
			mSkullTutorialShown = false;
			mCoinSoundActive = false;
			mCoinFlipFinished = true;
			mDrawCardsOverlay = false;
			mAllCardsFlipped = false;
			mHandWindowActive = false;
			DeactivateHandWindow();
			((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			GlobalMembers.gApp.DisableOptionsButtons(false);
			Array.Clear(mHandCount, 0, mHandCount.Length);
			mMaxCountdown = mIsPerpetual ? 510 : 450;
			for (int i = 0; i < 5; i++)
			{
				mCardSlots[i].mCardIdx = -2;
				mCardSlots[i].mCardScoreIdx = -2;
				mCardSlots[i].mCardType = -1;
				mCardSlots[i].mCardEffectState = 0;
				mCardSlots[i].mFlipPct.SetConstant(0.0);
				mCardSlots[i].mDealPct.SetConstant(0.0);
				DisposeCardEffects(mCardSlots[i]);
			}
			mPoints = startingMoney;
			base.NewGame(restartingGame);
		}

		private static void DisposeCardEffects(CardSlot slot)
		{
			if (slot.mCardEffect != null)
			{
				slot.mCardEffect.Dispose();
				slot.mCardEffect = null;
			}
			if (slot.mSecondaryCardEffect != null)
			{
				slot.mSecondaryCardEffect.Dispose();
				slot.mSecondaryCardEffect = null;
			}
		}

		public override string GetMusicName()
		{
			return "Poker";
		}

		public override void ReadyToPlay()
		{
			if (mHandsDealt == 0)
			{
				ResetCards(false);
			}
		}

		public override int GetBoardX()
		{
			return base.GetBoardX();
		}

		public override int GetBoardY()
		{
			return ConstantsWP.POKER_BOARD_Y + mBoardSlideYComp;
		}

		public override int GetTitleY()
		{
			return 65;
		}

		public override Rect GetCountdownBarRect()
		{
			return new Rect(0, 0, 0, 0);
		}

		public override int GetSidebarTextY()
		{
			return 300;
		}

		public override Rect GetLevelBarRect()
		{
			Rect celRect = GlobalMembersResourcesWP.IMAGE_POKER_SCORE_BKG.GetCelRect(0);
			celRect.Offset(GlobalMembers.S(GetBoardCenterX()) - celRect.mWidth / 2,
				GlobalMembers.S((int)(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_POKER_SCORE_BKG_ID) +
					80.0f + GetPokerUIYOffset())));
			return celRect;
		}

		public override int GetLevelPoints()
		{
			return mGoal;
		}

		public override float GetRankPointMultiplier()
		{
			return 1.3333f;
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId,
			bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd, int thePointType)
		{
			return null;
		}

		public Point GetCardUIPos(int cardIdx)
		{
			return new Point(ConstantsWP.POKER_CARDS_X + ConstantsWP.POKER_CARD_PITCH * cardIdx,
				ConstantsWP.POKER_CARDS_Y + ConstantsWP.POKER_CARDS_Y_SHIFT + GetPokerUIYOffset());
		}

		public override bool WantTopLevelBar()
		{
			return false;
		}

		public override bool WantTopFrame()
		{
			return false;
		}

		public override bool WantBottomFrame()
		{
			return false;
		}

		public override bool WantDrawScore()
		{
			return true;
		}

		public override bool WantDrawButtons()
		{
			return true;
		}

		public override bool CanPlay()
		{
			if (mSkullCrusherAnimPct.IsDoingCurve() ||
				mScoreHandPct.IsDoingCurve() || mCardSlots[4].mFlipPct.IsDoingCurve() ||
				mCoinFlipPct.IsDoingCurve() || mCoinWonPct.IsDoingCurve())
			{
				if (mHintCooldownTicks > 0)
				{
					mHintCooldownTicks = 25;
				}
				return false;
			}
			if (!mIsPerpetual && mPoints >= GetLevelPoints())
			{
				if (mHintCooldownTicks > 0)
				{
					mHintCooldownTicks = 25;
				}
				return false;
			}
			return base.CanPlay();
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
			if (!mButtonState0)
			{
				base.MouseDown(x, y, theBtnNum, theClickCount);
			}
		}

		public override bool CheckWin()
		{
			return !mIsPerpetual && mGoal > 0 && mPoints >= mGoal;
		}

		public override void GameOverAnnounce()
		{
			UI.Announcement announcement = new UI.Announcement(this, GlobalMembers._ID("GAME OVER", 5037));
			announcement.mPos.mY -= ConstantsWP.POKER_ANNOUNCEMENT_Y_OFFSET;
			GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_GAMEOVER);
		}

		public override bool IsGameSuspended()
		{
			if (mScoreHandPct.IsDoingCurve() || mCoinFlipPct.IsDoingCurve())
			{
				return false;
			}
			return base.IsGameSuspended();
		}

		public override void Flamify(Piece thePiece)
		{
			mFlameMoveCreditId = thePiece.mMoveCreditId;
			base.Flamify(thePiece);
		}

		public override void Laserify(Piece thePiece)
		{
			mLaserMoveCreditId = thePiece.mMoveCreditId;
			base.Laserify(thePiece);
		}

		public override void DoHypercube(Piece thePiece, Piece theSwappedPiece)
		{
			if (mCurrentCardIdx < 5)
			{
				FlipCard(theSwappedPiece, null);
			}
			base.DoHypercube(thePiece, theSwappedPiece);
		}

		public override void SwapSucceeded(SwapData theSwapData)
		{
			if (theSwapData.mPiece1 != null && theSwapData.mPiece2 != null && mCurrentCardIdx < 5)
			{
				Piece piece1 = theSwapData.mPiece1;
				Piece piece2 = theSwapData.mPiece2;
				bool useBoth = piece1.mMatchId >= 0 && piece2.mMatchId >= 0 &&
					Math.Abs(piece1.mMatchId - piece2.mMatchId) == 1 &&
					!piece1.IsFlagSet(PIECEFLAG.PIECEFLAG_HYPERCUBE) &&
					!piece2.IsFlagSet(PIECEFLAG.PIECEFLAG_HYPERCUBE);
				if (useBoth)
				{
					FlipCard(piece1, piece2);
				}
				else
				{
					Piece selected = piece1;
					if (piece1.mMatchId == -1 || piece2.mMatchId > piece1.mMatchId ||
						piece2.IsFlagSet(PIECEFLAG.PIECEFLAG_HYPERCUBE))
					{
						selected = piece2;
					}
					FlipCard(selected, null);
				}
			}
			base.SwapSucceeded(theSwapData);
		}

		private void FlipCard(Piece thePiece, Piece theTarget)
		{
			CardSlot slot = mCardSlots[mCurrentCardIdx];
			slot.mCardIdx = thePiece.mColor;
			slot.mCardScoreIdx = theTarget == null ? -2 : theTarget.mColor;
			slot.mCardType = 0;
			slot.mCardEffectState = 0;
			if (thePiece.IsFlagSet(PIECEFLAG.PIECEFLAG_HYPERCUBE))
			{
				slot.mCardType = 1;
				slot.mCardIdx = thePiece.mLastColor;
			}
			Point cardUIPos = GetCardUIPos(mCurrentCardIdx);
			float cardScale = GlobalMembers.S(1.0f);
			int cardTargetX = (int)(cardUIPos.mX + GlobalMembersResourcesWP.IMAGE_CARDS_FRONT.GetCelWidth() / 2.0f / cardScale);
			int cardTargetY = (int)(cardUIPos.mY + GlobalMembersResourcesWP.IMAGE_CARDS_FRONT.GetCelHeight() / 2.0f / cardScale);
			Piece[] cardPieces = { thePiece, theTarget };
			Color[] wildColors =
			{
				new Color(255, 0, 0),
				new Color(192, 192, 192),
				new Color(0, 255, 0),
				new Color(255, 255, 0),
				new Color(255, 0, 255),
				new Color(255, 128, 0),
				new Color(0, 128, 255)
			};
			for (int i = 0; i <= 1; i++)
			{
				if (cardPieces[i] != null)
				{
					CardGemEffect cardGemEffect = CardGemEffect.alloc(cardPieces[i],
						cardTargetX, cardTargetY);
					if (cardPieces[i].IsFlagSet(PIECEFLAG.PIECEFLAG_HYPERCUBE))
					{
						cardGemEffect.mColor = wildColors[cardPieces[i].mLastColor];
					}
					mPostFXManager.AddEffect(cardGemEffect);
					cardGemEffect.AddGem(cardPieces[i]);
				}
			}
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_CARD_FLIP_PCT,
				slot.mFlipPct);
			if (slot.mCardType != 1)
			{
				if (mFlameMoveCreditId == thePiece.mMoveCreditId)
				{
					slot.mCardType = 2;
					slot.mCardEffectState = mFlameBonus;
				}
				if (mLaserMoveCreditId == thePiece.mMoveCreditId)
				{
					slot.mCardType = 3;
					slot.mCardEffectState = mStarBonus;
				}
			}
			mFlameMoveCreditId = -1;
			mLaserMoveCreditId = -1;
			mPendingFlameCount = 0;
			mPendingStarCount = 0;
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_CARDFLIP);
			mCurrentCardIdx++;
			mAllCardsFlipped = mCurrentCardIdx == 5;
			int hand = CalculateHand();
			if (hand - 1 != mCurrentHandIdx)
			{
				mCurrentHandIdx = hand - 1;
			}
		}

		public int CalculateModifiedHand(int[] cards)
		{
			int[] counts = new int[7];
			for (int i = 0; i < cards.Length; i++)
			{
				if (cards[i] >= 0 && cards[i] < 7)
				{
					counts[cards[i]]++;
				}
			}
			int groups = 0;
			int first = 0;
			int second = 0;
			for (int i = 0; i < 7; i++)
			{
				if (counts[i] > 1)
				{
					groups++;
					if (counts[i] > first)
					{
						second = first;
						first = counts[i];
					}
					else if (counts[i] > second)
					{
						second = counts[i];
					}
				}
			}
			if (groups == 1)
			{
				if (first == 2)
				{
					return 1;
				}
				if (first == 3)
				{
					return 4;
				}
				if (first == 4)
				{
					return 6;
				}
				if (first == 5)
				{
					return 7;
				}
			}
			if (groups == 2)
			{
				return Math.Max(first, second) == 2 ? 3 : 5;
			}
			return 2;
		}

		public int CalculateHand()
		{
			int count = Math.Min(5, mCurrentCardIdx + 1);
			if (count <= 0)
			{
				return 0;
			}

			List<List<int>> choices = new List<List<int>>(count);
			for (int i = 0; i < count; i++)
			{
				List<int> values = new List<int>();
				if (mCardSlots[i].mCardType == 1)
				{
					for (int color = 0; color <= 6; color++)
					{
						values.Add(color);
					}
				}
				else
				{
					values.Add(mCardSlots[i].mCardIdx);
					if (mCardSlots[i].mCardScoreIdx >= 0)
					{
						values.Add(mCardSlots[i].mCardScoreIdx);
					}
				}
				choices.Add(values);
			}

			int[] indices = new int[count];
			int[] selected = new int[count];
			int[] bestValues = new int[count];
			int best = -1;
			while (true)
			{
				for (int i = 0; i < count; i++)
				{
					selected[i] = choices[i][indices[i]];
				}
				int hand = CalculateModifiedHand(selected);
				if (hand > best)
				{
					best = hand;
					Array.Copy(selected, bestValues, count);
				}

				int slot = 0;
				while (slot < count)
				{
					indices[slot]++;
					if (indices[slot] < choices[slot].Count)
					{
						break;
					}
					indices[slot] = 0;
					slot++;
				}
				if (slot == count)
				{
					break;
				}
			}

			if (mCurrentCardIdx >= 2)
			{
				for (int i = 0; i < count; i++)
				{
					CardSlot card = mCardSlots[i];
					if (card.mCardType == 1)
					{
						card.mCardIdx = bestValues[i];
					}
					else if (card.mCardScoreIdx >= 0 && bestValues[i] == card.mCardScoreIdx)
					{
						int value = card.mCardIdx;
						card.mCardIdx = card.mCardScoreIdx;
						card.mCardScoreIdx = value;
					}
				}
			}
			return best;
		}

		private void ScoreHand()
		{
			GlobalMembersResourcesWP.PIEFFECT_DISCOBALL.ResetAnim();
			GlobalMembersResourcesWP.PIEFFECT_STARBURST.ResetAnim();
			int hand = CalculateHand();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				hand == 7
					? PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_SCORE_HAND_PCT_A
					: PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_SCORE_HAND_PCT_B,
				mScoreHandPct);
			if (hand == 7)
			{
				AddToStat(32, 1, -1);
			}
			TallyScore(mHandValues[hand - 1], GlobalMembers._ID(HAND_NAMES[hand - 1], 588 + hand - 1));
			mHandCount[hand - 1]++;
		}

		private void TallyScore(int points, string scoreName)
		{
			mPoints += points;
			mGameStats[1] += points;
			if (mCurrentHandIdx >= 0 && mSkullHand >= 0)
			{
				mSkullBusterPct = Math.Min(1f, mSkullBusterPct + mHandBuster[mCurrentHandIdx]);
			}
			if (points > mBestHandPts)
			{
				mBestHandPts = points;
			}
			if (points > 0)
			{
				mScoreName = scoreName;
				mScoreTally = points;
			}
			else
			{
				mScoreName = GlobalMembers._ID("No Score", 385);
				mScoreTally = 0;
			}
			if (points > 0)
			{
				DoFanfare(mCurrentHandIdx);
			}
		}

		private void DoFanfare(int handIdx)
		{
			int sound = GlobalMembersResourcesWP.SOUND_POKERSCORE;
			if (handIdx >= 4)
			{
				if (handIdx == 4)
				{
					sound = GlobalMembersResourcesWP.SOUND_POKER_FULLHOUSE;
				}
				else if (handIdx == 5)
				{
					sound = GlobalMembersResourcesWP.SOUND_POKER_4OFAKIND;
				}
				else
				{
					sound = GlobalMembersResourcesWP.SOUND_POKER_FLUSH;
				}
			}
			GlobalMembers.gApp.PlaySample(sound);
		}

		public override void Update()
		{
			base.Update();
			if (mSuspendingGame || mUserPaused || mGameFinished ||
				GlobalMembers.gApp.mDialogList.Count != 0)
			{
				return;
			}
			mInfoButton.SetDisabled(!IsBoardStill() || !CanPlay() || mButtonState0);
			mHandAnimTimer += 0.68f;
			if (mHandAnimTimer >= 20.0f)
			{
				mHandAnimTimer -= 20.0f;
			}

			mCardBulgePct.IncInVal();
			mCoinWonPct.IncInVal();
			mSkullScale.IncInVal();
			mCoinFlipPct.IncInVal();
			mPokerLevelBarPIEffect.Update();
			for (int i = 0; i < mCardSlots.Length; i++)
			{
				CardSlot slot = mCardSlots[i];
				slot.mDealPct.IncInVal();
				if (slot.mCardEffect != null)
				{
					slot.mCardEffect.Update();
					if (slot.mSecondaryCardEffect != null)
					{
						slot.mSecondaryCardEffect.Update();
					}
				}
				else if (slot.mFlipPct.GetOutVal() == 2.0)
				{
					if (slot.mCardType == 2)
					{
						slot.mCardEffect = GlobalMembersResourcesWP.PIEFFECT_FLAME_CARD.Duplicate();
						slot.mCardEffect.mDrawBlockers = true;
					}
					else if (slot.mCardType == 3)
					{
						slot.mCardEffect = GlobalMembersResourcesWP.PIEFFECT_STAR_CARD.Duplicate();
						slot.mCardEffect.mDrawBlockers = true;
					}
				}
			}

			if (mDispPoints != mPoints && mDispPoints >= 1 &&
				GlobalMembers.gApp.mDialogList.Count == 0 && --mChipSoundDelay < 0)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_POKERCHIPS);
				mChipSoundDelay = 10;
			}

			if (mSkullCrusherAnimPct.GetOutVal() == 1.0 &&
				mSkullBusterDisp == mSkullBusterPct && mSkullBusterPct > 0.0f)
			{
				ResetCards(true);
			}

			if (mSkullWindowState && mCoinWonPct.GetOutVal() >= 1.0)
			{
				if (mSkullCrusherAnimPct.GetOutVal() == 0.0 && mSkullBusterDisp == mSkullBusterPct)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULL_BUSTER);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_SKULL_CRUSHER_ANIM_PCT,
						mSkullCrusherAnimPct);
					mSkullWindowState = false;
				}
			}
			else if (mCurrentCardIdx == 5 && mLevelCompleteCount == 0 &&
				mDeferredTutorialVector.Count == 0 &&
				!mScoreHandPct.IsDoingCurve() && !mSkullCrusherAnimPct.IsDoingCurve())
			{
				if (mScoreHandPct.GetOutVal() == 0.0 && !mCardSlots[4].mFlipPct.IsDoingCurve() &&
					mGameOverCount == 0 && mLevelCompleteCount == 0)
				{
					ScoreHand();
				}
				if (mScoreHandPct.GetOutVal() == 1.0 && mCoinFlipPct.GetOutVal() == 0.0 &&
					mCoinWonPct.GetOutVal() == 0.0)
				{
					if (mSkullHand < mCurrentHandIdx)
					{
						if (mSkullBusterDisp == mSkullBusterPct)
						{
							if (mSkullBusterPct < 1.0f)
							{
								ResetCards(true);
							}
							else if (mSkullCrusherAnimPct.GetOutVal() == 0.0)
							{
								GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULL_BUSTER);
								GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
									PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_SKULL_CRUSHER_ANIM_PCT,
									mSkullCrusherAnimPct);
							}
						}
					}
					else
					{
						if (mSkullBusterDisp < mSkullBusterPct && mSkullBusterPct >= 1.0f)
						{
							mSkullWindowState = true;
						}
						DoCoinFlip();
					}
				}
			}

			if (mScoreHandPct.IsDoingCurve())
			{
				mScoreHandPct.IncInVal();
				GlobalMembersResourcesWP.PIEFFECT_DISCOBALL.Update();
				GlobalMembersResourcesWP.PIEFFECT_STARBURST.Update();
				if (mSkullHand >= mCurrentHandIdx)
				{
					if (mCoinSound != null)
					{
						CurvedVal coinVolume = new CurvedVal();
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
							PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_UPDATE_COIN_VOL_SCORE_HAND,
							coinVolume, mScoreHandPct);
						mCoinSound.SetVolume(Math.Min((float)coinVolume.GetOutVal(), 1.0f));
					}
					else
					{
						mCoinSound = SexyFramework.GlobalMembers.gSexyApp.mSoundManager.GetSoundInstance(
							GlobalMembersResourcesWP.SOUND_SKULLCOIN_FLIP);
						if (mCoinSound != null)
						{
							mCoinSound.Play(true, false);
						}
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COINAPPEAR);
					}
				}
				CurvedVal darkenPct = new CurvedVal();
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_UPDATE_DARKEN_PCT,
					darkenPct, mScoreHandPct);
				mBoardDarken = (float)darkenPct.GetOutVal();
				if (mScoreHandPct.GetOutVal() > 0.35 && mScoreHandCardIdx <= 4 &&
					!mCardBulgePct.IsDoingCurve())
				{
						mScoreHandCardIdx++;
						while (mScoreHandCardIdx <= 4 && mCardSlots[mScoreHandCardIdx].mCardType < 2)
						{
							mScoreHandCardIdx++;
						}
					if (mScoreHandCardIdx <= 4)
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
							PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_CARD_BULGE_PCT,
							mCardBulgePct);
						CardSlot slot = mCardSlots[mScoreHandCardIdx];
						if (slot.mCardEffectState != 0)
						{
							int x = GetBoardCenterX() - ConstantsWP.POKER_CARD_SCORE_OFFSET_X + 140 * mScoreHandCardIdx;
							mPointsManager.Add(x, GetBoardCenterY(), slot.mCardEffectState,
								GlobalMembers.gGemColors[Math.Max(0, Math.Min(6, slot.mCardIdx))],
								(uint)mScoreHandCardIdx, false, -1, false);
							mPoints += slot.mCardEffectState;
							mLevelPointsTotal += slot.mCardEffectState;
							mScoreTally += slot.mCardEffectState;
							mGameStats[1] += slot.mCardEffectState;
						}
					}
				}
			}
			if (mCoinFlipPct.IsDoingCurve() && mCoinSound != null)
			{
				CurvedVal coinVolume = new CurvedVal();
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_UPDATE_COIN_VOL_COIN_FLIP,
					coinVolume, mCoinFlipPct);
				mCoinSound.SetVolume(Math.Min((float)coinVolume.GetOutVal(), 1.0f));
			}
			if (CanPlay() || mSkullWindowState && mCoinWonPct.GetOutVal() >= 0.9)
			{
				mSkullBusterDisp += (mSkullBusterPct - mSkullBusterDisp) / 20.0f;
				if (Math.Abs(mSkullBusterPct - mSkullBusterDisp) < 0.001f)
				{
					mSkullBusterDisp = mSkullBusterPct;
				}
			}
			if (mSkullHand == -1 && mSkullBarLidPct.GetOutVal() == 1.0 &&
				mSkullCrusherAnimPct.HasBeenTriggered())
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_SKULL_BAR_LID_PCT_A,
					mSkullBarLidPct);
			}
			if (mSkullExplodeEffect != null)
			{
				mSkullExplodeEffect.Update();
				if (mSkullExplodeEffect.mDeleteMe)
				{
					mSkullExplodeEffect.Dispose();
					mSkullExplodeEffect = null;
				}
			}
			if (mCoinWonPct.GetOutVal() == 1.0)
			{
				if (mSkullWindowState)
				{
					mCoinFlipFinished = false;
				}
				else
				{
					mInfoButton.SetDisabled(false);
					((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
					GlobalMembers.gApp.DisableOptionsButtons(false);
					mCoinFlipPct.SetConstant(0.0);
					mCoinWonPct.SetConstant(0.0);
					mSkullSpawnPct = 0.0f;
					if (!mSkullCrusherAnimPct.IsDoingCurve())
					{
						ResetCards(true);
					}
				}
			}
			if (mCoinFlipPct.GetOutVal() == 1.0 && IsBoardStill() && mCoinFlipFinished)
			{
				if (mBadFlip)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULLCOINLANDS);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULLCOINLOSE);
					GameOver(true);
				}
				else if (!mCoinWonPct.IsDoingCurve())
				{
					mCoinSoundActive = false;
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULLCOINLANDS);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULLCOINWIN);
					UI.Announcement announcement = new UI.Announcement(this, GlobalMembers._ID("SAFE!", 384));
					announcement.mPos.mY -= ConstantsWP.POKER_ANNOUNCEMENT_Y_OFFSET;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_COIN_WON_PCT,
						mCoinWonPct);
					mCoinWonPct.SetMode(0);
				}
				if (mCoinSound != null)
				{
					mCoinSound.Release();
					mCoinSound = null;
				}
			}
			if (mTobleroneEnabled && !mGameFinished && mTobleroneFlip)
			{
				if (--mTobleroneTimer <= 0)
				{
					mTobleroneTimer = 150;
					mTobleroneFlip = false;
					mTobleroneDirection = mTobleronePct == 0.0f ? -1 : 1;
					mTobleroneTarget = (int)(mTobleronePct + mTobleroneDirection);
				}
			}
			else if (mTobleroneEnabled && !mGameFinished && Math.Abs(mTobleronePct - mTobleroneTarget) <= 0.01f)
			{
				mTobleronePct = (int)(mTobleronePct + 0.01f * mTobleroneDirection);
				if (--mTobleroneTimer <= 0)
				{
					mTobleroneTimer = 150;
					mTobleroneFlip = true;
				}
			}
			else if (mTobleroneEnabled && !mGameFinished)
			{
				mTobleronePct += 0.01f * mTobleroneDirection;
			}
		}

		public override void DoUpdate()
		{
			base.DoUpdate();
		}

		public void DoCoinFlip(bool allowGoodFlip = false)
		{
			mCoinSoundActive = true;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_COIN_FLIP_PCT,
				mCoinFlipPct);
			mCoinFlipPct.SetMode(0);
			mBadFlip = mGameStats[3] != 0 && !allowGoodFlip &&
				(mNumCoinFlips == 2 || mRand.Next(mNumCoinFlips + 2) > 0);
			AddToStat(3, 1, -1);
			mNumCoinFlips++;
		}

		public void ResetCards(bool clearCards)
		{
			if (mIsPerpetual)
			{
				if (mCurrentHandIdx >= 0)
				{
					mSkullSpawnPct += mHandCountdown[mCurrentHandIdx] *
						(mCountdownMultPerHand * mHandsDealt + 1.0f);
				}
				if (mSkullBusterPct < 1.0f)
				{
					if (mSkullSpawnPct >= 1.0f)
					{
						if (mSkullHand == -1)
						{
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
								PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_SKULL_BAR_LID_PCT_B,
								mSkullBarLidPct);
						}
						mSkullSpawnCount++;
						mSkullHand = Math.Min(mSkullMax, mSkullHand + 1);
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULL_APPEAR);
						mSkullSpawnPct = 0.0f;
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
							PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_SKULL_SCALE,
							mSkullScale);
					}
				}
				else if (mSkullHand >= 0)
				{
					if (mSkullExplodeEffect == null)
					{
						mSkullExplodeEffect = new ParticleEffect();
						mSkullExplodeEffect.initWithPIEffect(GlobalMembersResourcesWP.PIEFFECT_SKULL_EXPLODE);
						mSkullExplodeEffect.mFXManager = mPostFXManager;
						mSkullExplodeEffect.mX = ConstantsWP.POKER_SKULLEXPLODE_X;
						mSkullExplodeEffect.mY = ConstantsWP.POKER_SKULLEXPLODE_Y -
							mSkullHand * ConstantsWP.POKER_SKULLEXPLODE_Y_STEP +
							ConstantsWP.POKER_SKULL_BAR_Y_SHIFT + GetPokerUIYOffset();
					}
					mSkullHand = Math.Max(-1, mSkullHand - 1);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_SKULL_BUSTED);
					mSkullsBusted++;
					mSkullSpawnPct = 0.0f;
					mSkullBusterPct = 0.0f;
					mSkullBusterDisp = 0.0f;
					mSkullCrusherAnimPct.SetConstant(0.0);
				}
				mHandsDealt++;
			}
			else
			{
				mLevelPointsTotal = mPoints;
				if (mPoints >= GetLevelPoints())
				{
					return;
				}
				mHandsLeft--;
				if (mHandsLeft < 0)
				{
					GameOver(true);
					return;
				}
			}
			mCurrentCardIdx = 0;
			mCurrentHandIdx = -1;
			mScoreHandCardIdx = -1;
			mScoreTally = 0;
			mCoinFlipFinished = true;
			for (int i = 0; i < 5; i++)
			{
				mCardSlots[i].mCardIdx = -2;
				mCardSlots[i].mCardScoreIdx = -2;
				mCardSlots[i].mCardType = 0;
				mCardSlots[i].mCardEffectState = 0;
				mCardSlots[i].mFlipPct.SetConstant(0.0);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_CARD_DEAL_PCT_1 + i,
					mCardSlots[i].mDealPct);
				if (clearCards)
				{
					DisposeCardEffects(mCardSlots[i]);
				}
				mCardSlots[i].mCardEffect = null;
			}
			mScoreHandPct.SetConstant(0.0);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_CARDDEAL);
			mAllCardsFlipped = false;
		}

		public override void DrawGameElements(Graphics g)
		{
			base.DrawGameElements(g);
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			if (mButtonState1)
			{
				int virtualTop = GetVirtualTop();
				int boardBottom = GlobalMembers.S(GetBoardY() + 800);
				g.PushState();
				g.SetColorizeImages(true);
				g.SetColor(new Color(0, 0, 0, 128));
				g.FillRect(GlobalMembers.S(GetBoardX()), virtualTop,
					mWidth, boardBottom - virtualTop);
				g.PopState();
			}
		}

		private static int ScaleDesign(float value)
		{
			return (int)GlobalMembers.S(value);
		}

		private static float Lerp(float from, float to, float pct)
		{
			return from + (to - from) * pct;
		}

		private void DrawCardEffect(Graphics g, PIEffect effect, float cardX, float cardY, float scale, int alpha)
		{
			if (effect == null)
			{
				return;
			}
			effect.mColor = new Color(255, 255, 255, alpha);
			effect.mDrawTransform.LoadIdentity();
			effect.mDrawTransform.Scale(GlobalMembers.S(scale * 0.7f), GlobalMembers.S(scale * 0.7f));
			effect.mDrawTransform.Translate(
				ScaleDesign(cardX) + GlobalMembersResourcesWP.IMAGE_CARDS_FRONT.GetCelWidth() / 2f,
				ScaleDesign(cardY) + GlobalMembersResourcesWP.IMAGE_CARDS_FRONT.GetCelHeight() / 2f);
			effect.Draw(g);
			effect.mDrawTransform.LoadIdentity();
		}

		private void DrawCards(Graphics g)
		{
			Image frontImage = GlobalMembersResourcesWP.IMAGE_CARDS_FRONT;
			Image faceImage = GlobalMembersResourcesWP.IMAGE_CARDS_FACE;
			Image backImage = GlobalMembersResourcesWP.IMAGE_CARDS_BACK;
			Image shadowImage = GlobalMembersResourcesWP.IMAGE_CARDS_SHADOW;
			Image smallFaceImage = GlobalMembersResourcesWP.IMAGE_CARDS_SMALL_FACE;
			float boardAlpha = GetAlpha();

			Color[] wildColors =
			{
				Color.White,
				Color.Red,
				new Color(192, 192, 192),
				Color.Green,
				Color.Yellow,
				new Color(255, 0, 255),
				new Color(255, 128, 0),
				new Color(0, 128, 255)
			};

			for (int i = 0; i < mCardSlots.Length; i++)
			{
				CardSlot slot = mCardSlots[i];
				if (slot.mCardType == -1)
				{
					break;
				}

				Point cardPos = GetCardUIPos(i);
				float cardX = cardPos.mX;
				float cardY = cardPos.mY;
				CurvedVal drawPosPct = new CurvedVal();
				if (slot.mDealPct.IsDoingCurve())
				{
					float deckX = GetBoardCenterX() - GlobalMembers.RS(GlobalMembersResourcesWP.IMAGE_POKER_BKG.GetWidth()) / 2.0f +
						GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_POKER_BKG_ID);
					float deckY = GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_POKER_BKG_ID) +
						GlobalMembers.RS(ConstantsWP.DEVICE_VIRTUAL_NEGATIVE_HEIGHT_F) + GetPokerUIYOffset();
					if (!mIsPerpetual && mHandsTotal > 0)
					{
						deckX += (1.0f - (float)mHandsLeft / mHandsTotal) * 12.0f;
					}
					float dealPct = (float)slot.mDealPct.GetOutVal();
					cardX = (int)Lerp(deckX, cardX, dealPct);
					cardY = (int)Lerp(deckY, cardY, dealPct);
				}
				else
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_CARDS_POS_PCT,
						drawPosPct, mScoreHandPct);
					float drawPos = (float)drawPosPct.GetOutVal();
					float scoreX = GetBoardCenterX() - 320.0f + 140.0f * i;
					cardX = (int)Lerp(cardX, scoreX, drawPos);
					cardY = (int)Lerp(cardY, GetBoardCenterY() - 90.0f, drawPos);
				}

				mDrawCardsOverlay = false;
				CurvedVal drawAlpha = new CurvedVal();
				CurvedVal flipY = new CurvedVal();
				CurvedVal flipShadowAlpha = new CurvedVal();
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_CARDS_ALPHA,
					drawAlpha, mScoreHandPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_CARDS_Y_POPUP,
					flipY, slot.mFlipPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_CARDS_SHADOW_ALPHA,
					flipShadowAlpha, slot.mFlipPct);
				float alphaPct = (float)drawAlpha.GetOutVal();
				if (alphaPct < 0.65f)
				{
					break;
				}
				mDrawCardsOverlay = true;
				float flip = (float)slot.mFlipPct.GetOutVal();
				float flipYValue = (float)flipY.GetOutVal();
				float cardDrawY = cardY - flipYValue;
				int alpha = (int)(boardAlpha * alphaPct * 255.0f);
				int shadowAlpha = (int)(boardAlpha * alphaPct * (float)flipShadowAlpha.GetOutVal() *
					slot.mDealPct.GetInVal() * 255.0f);
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, shadowAlpha));
				g.DrawImage(shadowImage,
					ScaleDesign(cardX - 10.0f + GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_CARDS_SHADOW_ID)),
					ScaleDesign(cardY + GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_CARDS_SHADOW_ID)));
				g.SetColor(new Color(255, 255, 255, alpha));

				if (flip < 1.0f)
				{
					float backPct = 1.0f - flip;
					int destWidth = (int)(backImage.GetWidth() * backPct);
					int destX = ScaleDesign(cardX) + (backImage.GetWidth() - destWidth) / 2;
					g.DrawImage(backImage, destX, ScaleDesign(cardDrawY), destWidth, backImage.GetHeight());
				}
				else
				{
					float frontPct = flip - 1.0f;
					float scale = 1.0f;
					if (drawPosPct.IsDoingCurve() && drawPosPct.GetOutVal() > 0.0)
					{
						scale += (float)drawPosPct.GetOutVal() * 0.5f;
					}
					if (mScoreHandCardIdx == i && mCardBulgePct.IsDoingCurve())
					{
						scale += (float)mCardBulgePct.GetOutVal() * 0.25f;
					}
					g.PushState();
					g.SetScale(scale, scale,
						ScaleDesign(cardX) + frontImage.GetCelWidth() / 2.0f,
						ScaleDesign(cardY) + frontImage.GetCelHeight() / 2.0f);
					int frontWidth = (int)(frontImage.GetCelWidth() * frontPct);
					int frontX = ScaleDesign(cardX) + (frontImage.GetCelWidth() - frontWidth) / 2;
					Rect frontDest = new Rect(frontX, ScaleDesign(cardDrawY), frontWidth, frontImage.GetCelHeight());
					g.DrawImage(frontImage, frontDest, frontImage.GetCelRect(0));
					if (flip == 2.0f)
					{
						int faceWidth = (int)(faceImage.GetCelWidth() * frontPct);
						int faceX = ScaleDesign(cardX + 4.0f) + (faceImage.GetCelWidth() - faceWidth) / 2;
						Rect faceDest = new Rect(faceX, ScaleDesign(cardDrawY + 4.0f), faceWidth, faceImage.GetCelHeight());
						if (slot.mCardType == 1)
						{
							Color wildColor = wildColors[slot.mCardIdx + 1];
							wildColor.mAlpha = alpha;
							g.SetColor(wildColor);
							g.DrawImageCel(faceImage, faceDest, 0);
							g.SetColor(new Color(255, 255, 255, alpha));
						}
						else
						{
							g.DrawImageCel(faceImage, faceDest, slot.mCardIdx + 1);
						}
						if (slot.mCardType != 1 && slot.mCardScoreIdx >= 0)
						{
							Transform smallTransform = new Transform();
							smallTransform.Translate(GlobalMembers.S(-25), GlobalMembers.S(40));
							smallTransform.Scale(frontPct, 1.0f);
							Rect smallRect = smallFaceImage.GetCelRect(slot.mCardScoreIdx);
							g.DrawImageTransformF(smallFaceImage, smallTransform, smallRect,
								ScaleDesign(cardX) + frontImage.GetCelWidth() / 2.0f,
								ScaleDesign(cardY) + frontImage.GetCelHeight() / 2.0f - GlobalMembers.S(flipYValue));
						}
						if (slot.mCardEffect != null)
						{
							DrawCardEffect(g, slot.mCardEffect, cardX, cardY, scale, alpha);
							if (slot.mSecondaryCardEffect != null)
							{
								DrawCardEffect(g, slot.mSecondaryCardEffect, cardX, cardY, scale, alpha);
							}
						}
					}
					g.PopState();
				}
				g.SetColor(Color.White);
				g.SetColorizeImages(false);
			}
		}

		private void DrawSkullCoinImageCel(Graphics g, int x, int y, int frame)
		{
			Image[] coinImages =
			{
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET1,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET1,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET2,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET3,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET4,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SIDE,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET4,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET3,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET2,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET1,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET1,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET1,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET2,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET3,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET4,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SIDE,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET4,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET3,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET2,
				GlobalMembersResourcesWP.IMAGE_SKULL_COIN_SET1
			};
			int[] coinCels = { 0, 1, 0, 0, 0, 0, 1, 1, 1, 2, 3, 4, 2, 2, 2, 0, 3, 3, 3, 5 };
			int index = Math.Max(0, Math.Min(coinImages.Length - 1, frame));
			Image image = coinImages[index];
			g.DrawImageCel(image, x - image.GetCelWidth() / 2, y - image.GetCelHeight() / 2, coinCels[index]);
		}

		private void DrawBulbFanfare(Graphics g)
		{
			CurvedVal alphaCurve = new CurvedVal();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_BULB_FANFARE_ALPHA,
				alphaCurve, mScoreHandPct);
			int alpha = (int)((float)alphaCurve.GetOutVal() * 255.0f * GetAlpha());
			g.SetColor(new Color(255, 255, 255, alpha));
			g.PushColorMult();
			int phase = mTicksInPlay / 10;
			for (int i = 0; i < 5; i++)
			{
				bool lit = (i - phase) % 3 == 0;
				Image image = lit ? GlobalMembersResourcesWP.IMAGE_POKER_LIGHT_LIT : GlobalMembersResourcesWP.IMAGE_POKER_LIGHT_UNLIT;
				if (lit)
				{
					g.SetDrawMode(Graphics.DrawMode.Additive);
				}
				Utils.DrawImageCentered(g, image,
					GlobalMembers.S(GetBoardCenterX() + 180 * i - 450),
					GlobalMembers.S(GetBoardCenterY() - 450 + ConstantsWP.POKER_BOARD_LIGHT_BULBS_OFFSET_Y));
				Utils.DrawImageCentered(g, image,
					GlobalMembers.S(GetBoardCenterX() - 180 * i + 450),
					GlobalMembers.S(GetBoardCenterY() + 450 - ConstantsWP.POKER_BOARD_LIGHT_BULBS_OFFSET_Y));
				if (lit)
				{
					g.SetDrawMode(Graphics.DrawMode.Normal);
				}
			}
			for (int i = 0; i < 5; i++)
			{
				bool lit = (i - phase) % 3 == 0;
				Image image = lit ? GlobalMembersResourcesWP.IMAGE_POKER_LIGHT_LIT : GlobalMembersResourcesWP.IMAGE_POKER_LIGHT_UNLIT;
				if (lit)
				{
					g.SetDrawMode(Graphics.DrawMode.Additive);
				}
				Utils.DrawImageCentered(g, image,
					GlobalMembers.S(GetBoardCenterX() - 450),
					GlobalMembers.S(GetBoardCenterY() - 180 * i + 450));
				Utils.DrawImageCentered(g, image,
					GlobalMembers.S(GetBoardCenterX() + 450),
					GlobalMembers.S(GetBoardCenterY() + 180 * i - 450));
				if (lit)
				{
					g.SetDrawMode(Graphics.DrawMode.Normal);
				}
			}
			g.PopColorMult();
			g.SetColor(Color.White);
		}

		private void DrawStarFanfare(Graphics g)
		{
			CurvedVal alphaCurve = new CurvedVal();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_STAR_FANFARE_ALPHA,
				alphaCurve, mScoreHandPct);
			float alpha = (float)alphaCurve.GetOutVal() * GetAlpha();
			g.SetColor(new Color(255, 255, 255, (int)(alpha * 255.0f)));
			g.PushColorMult();
			GlobalMembersResourcesWP.PIEFFECT_STARBURST.mDrawTransform.LoadIdentity();
			GlobalMembersResourcesWP.PIEFFECT_STARBURST.mDrawTransform.Translate(GetBoardCenterX(), GetBoardCenterY());
			GlobalMembersResourcesWP.PIEFFECT_STARBURST.mDrawTransform.Scale(
				ConstantsWP.POKER_STARBURST_SCALE_X, ConstantsWP.POKER_STARBURST_SCALE_Y);
			GlobalMembersResourcesWP.PIEFFECT_STARBURST.Draw(g);
			g.PopColorMult();
		}

		private void DrawDiscoFanfare(Graphics g)
		{
			CurvedVal alphaCurve = new CurvedVal();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_DISCO_FANFARE_ALPHA,
				alphaCurve, mScoreHandPct);
			float boardAlpha = GetAlpha();
			float alpha = (float)alphaCurve.GetOutVal() * boardAlpha;
			g.SetColor(new Color(255, 255, 255, (int)(alpha * 255.0f)));
			g.PushColorMult();
			GlobalMembersResourcesWP.PIEFFECT_DISCOBALL.mDrawTransform.LoadIdentity();
			GlobalMembersResourcesWP.PIEFFECT_DISCOBALL.mDrawTransform.Translate(
				GlobalMembers.S(ConstantsWP.POKER_DISCOBALL_X),
				GlobalMembers.S(ConstantsWP.POKER_DISCOBALL_Y));
			GlobalMembersResourcesWP.PIEFFECT_DISCOBALL.mDrawTransform.Scale(
				ConstantsWP.POKER_DISCOBALL_SCALE_X, ConstantsWP.POKER_DISCOBALL_SCALE_Y);
			if (boardAlpha == 1.0f)
			{
				GlobalMembersResourcesWP.PIEFFECT_DISCOBALL.Draw(g);
			}
			g.PopColorMult();
		}

		private void DrawCurrentHand(Graphics g)
		{
			int skullBarYShift = ConstantsWP.POKER_SKULL_BAR_Y_SHIFT + GetPokerUIYOffset();
			Image handShape = GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SHAPE_3_COPY_3;
			Image handList = GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_HAND_LIST;
			g.SetColor(Color.White);
			g.DrawImage(handShape,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_SHAPE_3_COPY_3_ID)),
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_SHAPE_3_COPY_3_ID) + skullBarYShift));
			g.DrawImage(handList,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_HAND_LIST_ID)),
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_HAND_LIST_ID) + skullBarYShift));

			for (int i = 0; i <= 6; i++)
			{
				int rowY = skullBarYShift - 70 * i;
				if (mSkullHand < i)
				{
					g.PushState();
					g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
					g.SetColor(Color.White);
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Color.Black);
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Color.White);
					g.WriteString(mHandValues[i].ToString(), GlobalMembers.S(584), GlobalMembers.S(rowY + 670));
					g.PopState();
				}
				else if (mSkullScale.IsDoingCurve() && mSkullHand == i)
				{
					float skullScale = (float)mSkullScale.GetOutVal();
					Transform transform = new Transform();
					float scale = skullScale * 2.0f + 0.2f;
					transform.Scale(scale, scale);
					int alpha = (int)(Math.Min(1.0f, 1.0f - (skullScale * 5.0f - 4.0f)) * 255.0f);
					g.PushState();
					g.SetColor(new Color(255, 255, 255, alpha));
					g.DrawImageTransformF(GlobalMembersResourcesWP.IMAGE_POKER_LARGE_SKULL,
						transform, GlobalMembers.S(580), GlobalMembers.S(rowY + 652));
					g.PopState();
				}
				else
				{
					g.PushState();
					g.SetColor(Color.White);
					g.DrawImageF(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_BAR_SKULL_COPY_3,
						GlobalMembers.S(105), GlobalMembers.S(rowY + 620));
					g.PopState();
				}

				g.PushState();
				g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
				g.SetColor(Color.White);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Color.Black);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Color.White);
				g.WriteString(GlobalMembers._ID(HAND_NAMES[i], 588 + i),
					GlobalMembers.S(288), GlobalMembers.S(rowY + 670));
				g.PopState();
			}

			if (mCurrentHandIdx >= 0)
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.DrawImageF(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_HAND_HIGHLIGHTED,
					GlobalMembers.S(89), GlobalMembers.S(skullBarYShift - 70 * mCurrentHandIdx + 604));
				g.SetDrawMode(Graphics.DrawMode.Normal);
			}
			g.SetColor(Color.White);
		}

		private void DrawHandWindow(Graphics g)
		{
			g.PushState();
			g.SetColorizeImages(true);
			g.SetColor(new Color(0, 0, 0, 128));
			g.FillRect(GlobalMembers.S(GetBoardX()), GetVirtualTop(),
				mWidth, ConstantsWP.DEVICE_VIRTUAL_VISIBLE_HEIGHT);
			g.PopState();
			DrawCurrentHand(g);
			DrawGameElements(g);
			if (mIsPerpetual && mSkullCrusherAnimPct.IsDoingCurve())
			{
				g.SetColor(Color.White);
				DrawSkullBar(g);
			}
			if (mSkullExplodeEffect != null)
			{
				mSkullExplodeEffect.Draw(g);
			}
			Dialog tutorialDialog = GlobalMembers.gApp.GetDialog(18);
			if (mSkullTutorialShown || tutorialDialog != null)
			{
				if (tutorialDialog != null && mSkullTutorialShown)
				{
					mSkullTutorialShown = false;
				}
				mWasTutorialDialogActive = true;
				mResumeButton.SetVisible(false);
				mExampleButton.SetVisible(false);
			}
			else
			{
				if (mWasTutorialDialogActive)
				{
					((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
				}
				mResumeButton.SetVisible(true);
				mExampleButton.SetVisible(true);
				DrawHandWindowButtons(g);
			}
			g.SetColor(Color.White);
		}

		private void DrawHandWindowButtons(Graphics g)
		{
			g.PushState();
			g.Translate(mResumeButton.mX, mResumeButton.mY);
			mResumeButton.Draw(g);
			g.PopState();
			g.PushState();
			g.Translate(mExampleButton.mX, mExampleButton.mY);
			mExampleButton.Draw(g);
			g.PopState();
		}

		private void DrawSkullBar(Graphics g)
		{
			Point topWidgetPos = GetTopWidgetPos();
			Image background = GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SKULL_BAR_BACKGROUND;
			int backgroundX = (int)GlobalMembers.S(
				GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_BAR_BACKGROUND_ID) +
				topWidgetPos.mX);
			int backgroundY = (int)GlobalMembers.S(
				GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_BAR_BACKGROUND_ID) +
				topWidgetPos.mY);
			g.PushState();
			g.DrawImage(background, backgroundX, backgroundY);
			if (mSkullHand >= 0 || mSkullCrusherAnimPct.IsDoingCurve())
			{
				g.PushState();
				int fillY = backgroundY + GlobalMembers.S(ConstantsWP.POKER_SKULL_BAR_Y_SHIFT);
				g.ClipRect(backgroundX, fillY,
					(int)(background.GetWidth() * mSkullBusterDisp), background.GetHeight());
				Color oldColor = g.mColor;
				g.SetColor(new Color(8, 32, 96));
				g.FillRect(backgroundX, fillY,
					(int)(background.GetWidth() * mSkullBusterDisp), background.GetHeight());
				g.SetColor(oldColor);
				mPokerLevelBarPIEffect.Draw(g);
				g.PopState();

				g.PushState();
				g.SetScale(ConstantsWP.POKER_BAR_SKULL_SCALE, ConstantsWP.POKER_BAR_SKULL_SCALE,
					GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_POKER_BAR_SKULL_ID) +
						topWidgetPos.mX + ConstantsWP.POKER_SKULL_SLASH_OFFS_X),
					GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_POKER_BAR_SKULL_ID) +
						topWidgetPos.mY + ConstantsWP.POKER_SKULL_SLASH_OFFS_Y));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_POKER_BAR_SKULL,
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_POKER_BAR_SKULL_ID) +
						topWidgetPos.mX + ConstantsWP.POKER_SKULL_SLASH_OFFS_X),
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_POKER_BAR_SKULL_ID) +
						topWidgetPos.mY + ConstantsWP.POKER_SKULL_SLASH_OFFS_Y));
				g.PopState();

				Transform slashTransform = new Transform();
				float slashX = ConstantsWP.POKER_SKULL_SLASH_OFFS_X + topWidgetPos.mX;
				float slashY = ConstantsWP.POKER_SKULL_SLASH_OFFS_Y + topWidgetPos.mY;
				if (mSkullCrusherAnimPct.IsDoingCurve())
				{
					CurvedVal slashPosition = new CurvedVal();
					CurvedVal slashScale = new CurvedVal();
					CurvedVal slashAlphaCurve = new CurvedVal();
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_SKULL_BAR_SKULL_X,
						slashPosition, mSkullCrusherAnimPct);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_SKULL_BAR_SCALE,
						slashScale, mSkullCrusherAnimPct);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_SKULL_BAR_SHADOW_ALPHA,
						slashAlphaCurve, mSkullCrusherAnimPct);
					slashX += ConstantsWP.POKER_SKULL_SLASH_CURVE_OFFS_X;
					slashY += (float)slashPosition.GetOutVal() *
						(ConstantsWP.POKER_SKULL_SLASH_CURVE_OFFS_Y -
						ConstantsWP.POKER_SKULL_SLASH_CURVE_OFFS_Y_STEP * mSkullHand);
					slashTransform.Scale((float)slashScale.GetOutVal(), (float)slashScale.GetOutVal());
					g.PushState();
					g.SetColorizeImages(true);
					g.SetColor(new Color(255, 255, 255, (int)slashAlphaCurve.GetOutVal()));
					Image slashShadow = GlobalMembersResourcesWP.IMAGE_POKER_SLASH_SHADOW;
					float shadowX = GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_POKER_SLASH_SHADOW_ID) + slashX);
					float shadowY = GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_POKER_SLASH_SHADOW_ID) + slashY);
					g.DrawImageTransformF(slashShadow, slashTransform,
						shadowX + slashShadow.GetWidth() / 2.0f,
						shadowY + slashShadow.GetHeight() / 2.0f);
					g.PopState();
				}
				else
				{
					slashTransform.Scale(ConstantsWP.POKER_BAR_SKULL_SLASH_SCALE, ConstantsWP.POKER_BAR_SKULL_SLASH_SCALE);
				}
				g.SetColor(Color.White);
				float slashImageX = GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_POKER_SKULL_SLASH_ID) +
					slashX - ConstantsWP.POKER_SKULL_SLASH_OFFSET_X);
				float slashImageY = GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_POKER_SKULL_SLASH_ID) +
					slashY) - ConstantsWP.POKER_SKULL_SLASH_OFFSET_Y;
				g.DrawImageTransformF(GlobalMembersResourcesWP.IMAGE_POKER_SKULL_SLASH, slashTransform,
					slashImageX + GlobalMembersResourcesWP.IMAGE_POKER_SKULL_SLASH.GetWidth() / 2.0f,
					slashImageY + GlobalMembersResourcesWP.IMAGE_POKER_SKULL_SLASH.GetHeight() / 2.0f);
				g.SetColor(Color.White);
			}
			if (mSkullBarLidPct.GetOutVal() < 1.0)
			{
				Image shutter = GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SKULL_SHUTTER;
				g.DrawImage(shutter,
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_SHUTTER_ID) + topWidgetPos.mX),
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_SHUTTER_ID) + topWidgetPos.mY),
					shutter.GetWidth(), (int)(shutter.GetHeight() * (1.0 - mSkullBarLidPct.GetOutVal())));
			}
			if (mSkullCrusherAnimPct.IsDoingCurve())
			{
				g.PushState();
				g.SetColorizeImages(true);
				CurvedVal glowAlpha = new CurvedVal();
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_SKULL_BAR_GLOW,
					glowAlpha, mSkullCrusherAnimPct);
				g.SetDrawMode(Graphics.DrawMode.Additive);
				int glow = (int)((float)glowAlpha.GetOutVal() * 255.0f);
				g.SetColor(new Color(glow, glow, glow));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SKULL_BAR_GLOW,
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_BAR_GLOW_ID) + topWidgetPos.mX),
					(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_BAR_GLOW_ID) + topWidgetPos.mY));
				g.PopState();
			}
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SKULL_FRAME_BOTTOM,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_FRAME_BOTTOM_ID) + topWidgetPos.mX),
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_FRAME_BOTTOM_ID) + topWidgetPos.mY));
			g.SetColor(Color.White);
			g.PopState();
		}

		public override void DrawScoreWidget(Graphics g)
		{
			g.SetColor(Color.FAlpha(GetAlpha()));
			g.PushColorMult();
			g.SetColor(Color.FAlpha(GetBoardAlpha()));
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			g.SetColor(Color.White);
			if (mIsPerpetual)
			{
				DrawSkullBar(g);
			}
			g.SetColor(Color.White);
			g.PopColorMult();
		}

		public override void DrawScore(Graphics g)
		{
			g.PushState();
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			g.SetColor(Color.White);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, Color.White);
			g.WriteString(SexyFramework.Common.CommaSeperate(mDispPoints),
				GlobalMembers.S(GetBoardCenterX()),
				GlobalMembers.S(ConstantsWP.POKER_NEED_MESSAGE_Y + GetPokerUIYOffset()));
			((ImageFont)g.mFont).PopLayerColor("GLOW");
			g.PopState();
		}

		private void DrawText(Graphics g)
		{
			if (!mTobleroneEnabled || mSkullHand < 0 || mSkullHand < mCurrentHandIdx)
			{
				DrawScore(g);
				return;
			}
			int tobleroneHeight = GlobalMembers.S(ConstantsWP.POKER_TOBLERONE);
			int offset = (int)(mTobleronePct * tobleroneHeight);
			g.PushState();
			g.mClipRect.mY = (int)g.mTransY + ConstantsWP.POKER_TOBLERONE_CLIP_Y_OFFSET +
				GlobalMembers.S(GetPokerUIYOffset());
			g.mClipRect.mHeight = GlobalMembers.S(52);
			g.SetColor(Color.White);
			int textY = GlobalMembers.S(ConstantsWP.POKER_NEED_MESSAGE_Y + GetPokerUIYOffset()) + offset;
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, Color.White);
			g.WriteString(SexyFramework.Common.CommaSeperate(mDispPoints),
				GlobalMembers.S(GetBoardCenterX()),
				textY);
			((ImageFont)g.mFont).PopLayerColor("GLOW");
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Color.Black);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Color.White);
			g.WriteString(string.Format(GlobalMembers._ID("Need {0} or better", 377),
				GlobalMembers._ID(HAND_NAMES[mSkullHand + 1], 589 + mSkullHand)),
				GlobalMembers.S(GetBoardCenterX()),
				textY + tobleroneHeight);
			((ImageFont)g.mFont).PopLayerColor("GLOW");
			g.PopState();
		}

		private void DrawPokerOverlayText(Graphics g)
		{
			CurvedVal textAlpha = new CurvedVal();
			CurvedVal textScale = new CurvedVal();
			CurvedVal textY = new CurvedVal();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_OVERLAY_TEXT_ALPHA,
				textAlpha, mScoreHandPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_OVERLAY_TEXT_POS_PCT,
				textScale, mScoreHandPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_OVERLAY_TEXT_Y_BUMP,
				textY, mScoreHandPct);
			float alpha = (float)textAlpha.GetOutVal() * GetAlpha();
			float scale = (float)textScale.GetOutVal();
			float y = (float)textY.GetOutVal();

			g.PushState();
			g.SetScale(scale, scale, GlobalMembers.S(75),
				GlobalMembers.S(10 + GetPokerUIYOffset()));
			g.SetFont(GlobalMembersResources.FONT_HUGE);
			g.SetColor(new Color(255, 255, 255, (int)(alpha * 255.0f)));
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 1,
				new Color(0, 0, 0, (int)(alpha * 255.0f)));
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 0,
				new Color(255, 255, 255, (int)(alpha * 255.0f)));
			if (mDrawCardsOverlay)
			{
				g.WriteString(mScoreName, GlobalMembers.S(GetBoardCenterX()),
					GlobalMembers.S((int)(ConstantsWP.POKER_SCORENAME_ONBOARD_Y - y +
						ConstantsWP.POKER_BOARD_Y_SHIFT + GetPokerUIYOffset())));
			}
			g.PopState();

			if (mScoreTally < 1)
			{
				RestoreSharedFontState();
				return;
			}

			g.PushState();
			if (mSkullHand < mCurrentHandIdx)
			{
				if (mCardBulgePct.IsDoingCurve())
				{
					g.PushState();
					g.SetFont(GlobalMembersResources.FONT_HUGE);
					int boardAlpha = (int)(GetAlpha() * 255.0f);
					g.SetColor(new Color(255, 255, 255, boardAlpha));
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 1,
						new Color(0, 0, 0, boardAlpha));
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 0,
						new Color(255, 255, 255, boardAlpha));
					float tallyScale = 1.0f + (float)mCardBulgePct.GetOutVal() * 0.25f;
					g.SetScale(tallyScale, tallyScale, GlobalMembers.S(GetBoardCenterX()),
						GlobalMembers.S(785 + GetPokerUIYOffset()));
					string scoreText = string.Format(GlobalMembers._ID("+{0}", 378),
						SexyFramework.Common.CommaSeperate(mScoreTally));
					g.WriteString(scoreText, GlobalMembers.S(GetBoardCenterX()),
						GlobalMembers.S((int)(ConstantsWP.POKER_SCORE_ONBOARD_Y + y +
							ConstantsWP.POKER_SKULL_BAR_Y_SHIFT + GetPokerUIYOffset())));
					g.PopState();
				}
				else if (mDrawCardsOverlay)
				{
					g.PushState();
					g.SetFont(GlobalMembersResources.FONT_HUGE);
					g.SetColor(new Color(255, 255, 255, (int)(alpha * 255.0f)));
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 1,
						new Color(0, 0, 0, (int)(alpha * 255.0f)));
					Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 0,
						new Color(255, 255, 255, (int)(alpha * 255.0f)));
					g.SetScale(scale, scale, GlobalMembers.S(167),
						GlobalMembers.S(10 + GetPokerUIYOffset()));
					string scoreText = string.Format(GlobalMembers._ID("+{0}", 379),
						SexyFramework.Common.CommaSeperate(mScoreTally));
					g.WriteString(scoreText, GlobalMembers.S(GetBoardCenterX()),
						GlobalMembers.S((int)(ConstantsWP.POKER_SCORE_ONBOARD_Y + y +
							ConstantsWP.POKER_SKULL_BAR_Y_SHIFT + GetPokerUIYOffset())));
					g.PopState();
				}
			}
			else if (mCoinFlipPct.GetOutVal() == 0.0)
			{
				CurvedVal coinPosition = new CurvedVal();
				CurvedVal coinScale = new CurvedVal();
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_OVERLAY_SKULL_POS_PCT,
					coinPosition, mScoreHandPct);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_OVERLAY_SKULL_SCALE,
					coinScale, mScoreHandPct);
				DeactivateHandWindow();
				mInfoButton.SetDisabled(true);
				float position = (float)coinPosition.GetOutVal();
				int coinX = (int)(140.0f + (GetBoardCenterX() - 140.0f) * position);
				int coinY = (int)(110.0f + 730.0f * position) + GetPokerUIYOffset();
				float skullScale = (float)coinScale.GetOutVal();
				g.SetScale(skullScale, skullScale, GlobalMembers.S(coinX), GlobalMembers.S(coinY));
				DrawSkullCoinImageCel(g, GlobalMembers.S(coinX), GlobalMembers.S(coinY), (int)mHandAnimTimer);
				GlobalMembers.gApp.DisableOptionsButtons(true);
				((UI.PauseMenu)GlobalMembers.gApp.mMenus[7]).SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
			}
			g.PopState();
			RestoreSharedFontState();
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			if (mScoreHandPct.GetOutVal() == 0.0 || mScoreTally >= 1)
			{
				DrawCards(g);
			}
			base.DrawOverlay(g, thePriority);
			g.SetColor(Color.FAlpha(GetAlpha()));
			g.PushColorMult();
			g.SetColor(Color.White);
			Dialog tutorialDialog = GlobalMembers.gApp.GetDialog(18);
			if (tutorialDialog != null && tutorialDialog.mDialogHeader == GlobalMembers._ID("Poker skull", 3230))
			{
				DrawSkullBar(g);
			}
			if (mScoreHandPct.IsDoingCurve())
			{
				switch (mCurrentHandIdx)
				{
				case 4:
					DrawBulbFanfare(g);
					break;
				case 5:
					DrawStarFanfare(g);
					break;
				case 6:
					DrawDiscoFanfare(g);
					break;
				}
				DrawPokerOverlayText(g);
				DrawCards(g);
			}
			if (mSkullCrusherAnimPct.IsDoingCurve() || mSkullScale.IsDoingCurve())
			{
				ActivateHandWindow(true);
			}
			if (mButtonState0 && !mGameFinished)
			{
				DrawHandWindow(g);
			}
			else if (mButtonState1)
			{
				int boardBottom = GlobalMembers.S(GetBoardY() + 800);
				g.PushState();
				g.SetColorizeImages(true);
				g.SetColor(new Color(0, 0, 0, 128));
				g.FillRect(GlobalMembers.S(GetBoardX()), boardBottom,
					mWidth, GetVirtualBottom() - boardBottom);
				g.PopState();
				g.PushState();
				g.Translate(mHandsExamplesWidget.mX, mHandsExamplesWidget.mY);
				ModalFlags modalFlags = new ModalFlags();
				mWidgetManager.InitModalFlags(modalFlags);
				mHandsExamplesWidget.DrawAll(modalFlags, g);
				g.PopState();
				DrawHandWindowButtons(g);
			}
			if (mCoinFlipPct.GetOutVal() > 0.0 || mCoinWonPct.GetOutVal() > 0.0)
			{
				CurvedVal coinY = new CurvedVal();
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_OVERLAY_COIN_Y_PCT,
					coinY, mCoinFlipPct);
				int coinFrame = (int)mHandAnimTimer;
				if (!mCoinFlipPct.IsDoingCurve())
				{
					coinFrame = mBadFlip ? 10 : 0;
				}
				g.SetColorizeImages(true);
				if (mCoinWonPct.GetOutVal() > 0.0)
				{
					CurvedVal coinAlpha = new CurvedVal();
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.ePOKER_BOARD_DRAW_OVERLAY_COIN_ALPHA,
						coinAlpha, mCoinWonPct);
					g.SetColor(new Color(255, 255, 255,
						(int)((float)coinAlpha.GetOutVal() * 255.0f)));
				}
				if (GetAlpha() == 0.0f)
				{
					mCoinWonPct.SetConstant(1.0);
				}
				else
				{
					DrawSkullCoinImageCel(g, GlobalMembers.S(GetBoardCenterX()),
						GlobalMembers.S((int)(ConstantsWP.POKER_SKULLCOIN_Y +
							(float)coinY.GetOutVal() * -650.0f + GetPokerUIYOffset())), coinFrame);
				}
			}
			g.SetColor(Color.White);
			g.PopColorMult();
		}

		public override void DrawHUDText(Graphics g)
		{
		}

		public override void DrawWarningHUD(Graphics g)
		{
		}

		public override void DrawBottomFrame(Graphics g)
		{
		}

		public override void DrawTopFrame(Graphics g)
		{
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_POKER_SKULL_FRAME_TOP,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_FRAME_TOP_ID)),
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_POKER_SKULL_FRAME_TOP_ID) +
					ConstantsWP.POKER_BOARD_SKULL_FRAME_TOP_Y + GetPokerUIYOffset()));
		}

		public override void DrawUI(Graphics g)
		{
			mBoardSlideYComp = mHyperspace != null
				? (int)((double)GlobalMembers.S(mOfsY) * (double)mSlidingHUDCurve.GetOutVal())
				: 0;
			DrawCheckboard(g);
			DrawTopFrame(g);
			DrawBottomFrame(g);
			DrawScoreWidget(g);
			DrawText(g);
			if (WantDrawButtons())
			{
				DrawButtons(g);
			}
		}

		public override bool SaveGameExtra(Serialiser theBuffer)
		{
			int chunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkPokerBoard);
			bool extended = theBuffer.mHeader.mOldHeader.mGameVersion > 104;
			for (int i = 0; i < 5; i++)
			{
				theBuffer.WriteInt32(mCardSlots[i].mCardIdx);
				theBuffer.WriteInt32(mCardSlots[i].mCardScoreIdx);
				theBuffer.WriteInt32(mCardSlots[i].mCardType);
				theBuffer.WriteCurvedVal(mCardSlots[i].mFlipPct);
				theBuffer.WriteCurvedVal(mCardSlots[i].mDealPct);
				if (extended)
				{
					theBuffer.WriteInt32(mCardSlots[i].mCardEffectState);
				}
			}
			theBuffer.WriteValuePair(Serialiser.PairID.PokerCardIdx, mCurrentCardIdx);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerCardScoreIdx, mScoreHandCardIdx);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerGoal, mGoal);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerHands, mHandsLeft);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerHandsDelat, mHandsDealt);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullsBusted, mSkullsBusted);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerBestHandsPts, mBestHandPts);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerStartHands, mHandsTotal);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerChipSoundDelay, mChipSoundDelay);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullHand, mSkullHand);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullMax, mSkullMax);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerNumCoinFlips, mNumCoinFlips);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerFlameBonus, mFlameBonus);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerStarBonus, mStarBonus);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerScoreTally, mScoreTally);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerCurrentHandIdx, mCurrentHandIdx);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerFlameMoveCreditId, mFlameMoveCreditId);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerLaserMoveCreditId, mLaserMoveCreditId);
			theBuffer.WriteArrayPair(Serialiser.PairID.PokerHandCount, mHandCount.Length, mHandCount);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullSpawnPct, mSkullSpawnPct);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullBusterPct, mSkullBusterPct);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullBusterDisp, mSkullBusterDisp);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerBadFlip, mBadFlip);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerScoreHandPct, mScoreHandPct);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerCardBulgePct, mCardBulgePct);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullScale, mSkullScale);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerCoinFlipPct, mCoinFlipPct);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerCoinWonPct, mCoinWonPct);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullCrusherAnimPct, mSkullCrusherAnimPct);
			theBuffer.WriteValuePair(Serialiser.PairID.PokerSkullBarLidPct, mSkullBarLidPct);
			theBuffer.WriteStringPair(Serialiser.PairID.PokerScoreName, mScoreName);
			if (extended)
			{
				theBuffer.WriteValuePair((Serialiser.PairID)170, mPendingFlameCount);
				theBuffer.WriteValuePair((Serialiser.PairID)171, mPendingStarCount);
				theBuffer.WriteValuePair((Serialiser.PairID)172, mSkullSpawnCount);
			}
			theBuffer.FinalizeGameChunkHeader(chunkBeginLoc);
			return base.SaveGameExtra(theBuffer);
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			int chunkBeginPos = 0;
			GameChunkHeader header = new GameChunkHeader();
			if (theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkPokerBoard, header, out chunkBeginPos))
			{
				bool extended = theBuffer.mHeader.mOldHeader.mGameVersion >= 105;
				for (int i = 0; i < 5; i++)
				{
					mCardSlots[i].mCardIdx = theBuffer.ReadInt32();
					mCardSlots[i].mCardScoreIdx = theBuffer.ReadInt32();
					mCardSlots[i].mCardType = theBuffer.ReadInt32();
					theBuffer.ReadCurvedVal(mCardSlots[i].mFlipPct);
					theBuffer.ReadCurvedVal(mCardSlots[i].mDealPct);
					mCardSlots[i].mCardEffectState = extended
						? theBuffer.ReadInt32()
						: mCardSlots[i].mCardType == 2
							? mFlameBonus
							: mCardSlots[i].mCardType == 3 ? mStarBonus : 0;
					DisposeCardEffects(mCardSlots[i]);
				}
				theBuffer.ReadValuePair(out mCurrentCardIdx);
				theBuffer.ReadValuePair(out mScoreHandCardIdx);
				theBuffer.ReadValuePair(out mGoal);
				theBuffer.ReadValuePair(out mHandsLeft);
				theBuffer.ReadValuePair(out mHandsDealt);
				theBuffer.ReadValuePair(out mSkullsBusted);
				theBuffer.ReadValuePair(out mBestHandPts);
				theBuffer.ReadValuePair(out mHandsTotal);
				theBuffer.ReadValuePair(out mChipSoundDelay);
				theBuffer.ReadValuePair(out mSkullHand);
				theBuffer.ReadValuePair(out mSkullMax);
				theBuffer.ReadValuePair(out mNumCoinFlips);
				theBuffer.ReadValuePair(out mFlameBonus);
				theBuffer.ReadValuePair(out mStarBonus);
				theBuffer.ReadValuePair(out mScoreTally);
				theBuffer.ReadValuePair(out mCurrentHandIdx);
				theBuffer.ReadValuePair(out mFlameMoveCreditId);
				theBuffer.ReadValuePair(out mLaserMoveCreditId);
				theBuffer.ReadArrayPair(mHandCount.Length, mHandCount);
				theBuffer.ReadValuePair(out mSkullSpawnPct);
				theBuffer.ReadValuePair(out mSkullBusterPct);
				theBuffer.ReadValuePair(out mSkullBusterDisp);
				theBuffer.ReadValuePair(out mBadFlip);
				theBuffer.ReadValuePair(mScoreHandPct);
				theBuffer.ReadValuePair(mCardBulgePct);
				theBuffer.ReadValuePair(mSkullScale);
				theBuffer.ReadValuePair(mCoinFlipPct);
				theBuffer.ReadValuePair(mCoinWonPct);
				theBuffer.ReadValuePair(mSkullCrusherAnimPct);
				theBuffer.ReadValuePair(mSkullBarLidPct);
				theBuffer.ReadStringPair(out mScoreName);
				if (extended)
				{
					theBuffer.ReadValuePair(out mPendingFlameCount);
					theBuffer.ReadValuePair(out mPendingStarCount);
					theBuffer.ReadValuePair(out mSkullSpawnCount);
				}
			}
			base.LoadGameExtra(theBuffer);
			int dealtCardCount = Math.Min(mCurrentCardIdx, mCardSlots.Length);
			for (int i = 0; i < dealtCardCount; i++)
			{
				CardSlot slot = mCardSlots[i];
				if (slot.mCardType >= 0 && !slot.mFlipPct.IsDoingCurve())
				{
					slot.mFlipPct.SetConstant(2.0);
				}
				if (slot.mCardType >= 0 && !slot.mDealPct.IsDoingCurve())
				{
					slot.mDealPct.SetConstant(1.0);
				}
			}
		}
	}
}
