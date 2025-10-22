using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus.UI
{
	public class HelpDialog : Bej3Widget, CheckboxListener, Bej3ScrollWidgetListener, ScrollWidgetListener
	{
		public enum HELP_WINDOW
		{
			HELP_WINDOW_COUNT = 4
		}

		public enum HELPDIALOG_STATE
		{
			HELPDIALOG_STATE_PREGAME,
			HELPDIALOG_STATE_INGAME,
			HELPDIALOG_STATE_MAINMENU,
			HELPDIALOG_STATE_OPTIONS,
			HELPDIALOG_STATE_FIRSTGAME
		}

		private enum HELPDIALOG_IDS
		{
			CHK_DISABLE_HINTS,
			BTN_LEFT_ID,
			BTN_RIGHT_ID,
			BTN_CLOSE_HELP_ID
		}

		private bool mIsSetUp;

		private int mCurrentPage;

		private int mScrollingToPage;

		private int mNumWindows;

		private Label mHeadingLabel;

		private Bej3Button mCloseButton;

		private bool mIsBack;

		private bool mShowCheckbox;

		private bool mHasDrawn;

		private bool mFirstDraw;

		private List<ResourceRef> mAnimRefVector = new List<ResourceRef>();

		private Bej3Checkbox mCheckbox;

		private Label mDisableHintLabel;

		private HelpDialogContainer mContainer;

		private Bej3ScrollWidget mScrollWidget;

		private HelpWindow[] mHelpWindow = new HelpWindow[4];

		private Bej3Button mSlideLeftButton;

		private Bej3Button mSlideRightButton;

		private Label mSwipeMsgLabel;

		public static int mTutorialFlag;

		public HELPDIALOG_STATE mHelpDialogState;

		private void HandleCloseButton()
		{
			switch (mHelpDialogState)
			{
			case HELPDIALOG_STATE.HELPDIALOG_STATE_PREGAME:
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).mComingFromHelp = true;
				GlobalMembers.gApp.mProfile.SetTutorialCleared(mTutorialFlag);
				GlobalMembers.gApp.StartSetupGame(true);
				Transition_SlideOut();
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).mComingFromHelp = false;
				break;
			case HELPDIALOG_STATE.HELPDIALOG_STATE_INGAME:
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).mComingFromHelp = true;
				GlobalMembers.gApp.DoPauseMenu();
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).Expand();
				mVisible = true;
				Transition_SlideOut();
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).mComingFromHelp = false;
				break;
			case HELPDIALOG_STATE.HELPDIALOG_STATE_MAINMENU:
				GlobalMembers.gApp.DoMainMenu(true);
				Transition_SlideOut();
				break;
			case HELPDIALOG_STATE.HELPDIALOG_STATE_OPTIONS:
				GlobalMembers.gApp.DoOptionsMenu();
				Transition_SlideOut();
				break;
			case HELPDIALOG_STATE.HELPDIALOG_STATE_FIRSTGAME:
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).mComingFromHelp = true;
				GlobalMembers.gApp.mProfile.SetTutorialCleared(mTutorialFlag);
				GlobalMembers.gApp.StartSetupGame(true);
				if (mIsBack)
				{
					((PauseMenu)GlobalMembers.gApp.mMenus[7]).ButtonDepress(10001);
				}
				Transition_SlideOut();
				((PauseMenu)GlobalMembers.gApp.mMenus[7]).mComingFromHelp = false;
				break;
			}
		}

		private void SetUpSlideButtons()
		{
			int pageHorizontal = mScrollWidget.GetPageHorizontal();
			bool flag = pageHorizontal > 0;
			mSlideLeftButton.SetVisible(flag);
			mSlideLeftButton.SetDisabled(!flag);
			flag = pageHorizontal < mNumWindows - 1;
			mSlideRightButton.SetVisible(flag);
			mSlideRightButton.SetDisabled(!flag);
		}

		public HelpDialog()
			: base(Menu_Type.MENU_HELPMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			mCurrentPage = 0;
			mScrollingToPage = 0;
			mNumWindows = 0;
			mIsSetUp = false;
			Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mFinalY = 0;
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.HELPDIALOG_HEADING_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetText(GlobalMembers._ID("HELP", 3337));
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			int num = 30;
			int theHeight = 50;
			mSwipeMsgLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mSwipeMsgLabel.SetTextBlock(new Rect(ConstantsWP.HELPDIALOG_SWIPE_MSG_LABEL_X, ConstantsWP.HELPDIALOG_SWIPE_MSG_LABEL_Y - num - 10, ConstantsWP.SLIDE_BUTTON_MESSAGE_WIDTH, theHeight), true);
			mSwipeMsgLabel.SetTextBlockEnabled(true);
			mSwipeMsgLabel.SetClippingEnabled(false);
			mSwipeMsgLabel.SetText(GlobalMembers._ID("Swipe for more help", 3338));
			mSwipeMsgLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_3_FILL);
			mSwipeMsgLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_3_STROKE);
			AddWidget(mSwipeMsgLabel);
			mDisableHintLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Help", 3339), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mDisableHintLabel.Resize(ConstantsWP.HELPDIALOG_DISABLE_HINT_LABEL_X, ConstantsWP.HELPDIALOG_DISABLE_HINT_LABEL_Y, 0, 0);
			mCheckbox = new Bej3Checkbox(0, this);
			mCheckbox.Resize(ConstantsWP.HELPDIALOG_DISABLE_HINT_X, ConstantsWP.HELPDIALOG_DISABLE_HINT_Y, 0, 0);
			mCheckbox.mGrayOutWhenDisabled = false;
			ConstantsWP.HELPDIALOG_DIVIDER_BOX_1_W += mDisableHintLabel.GetTextWidth();
			int num2 = mWidth / 2 - ConstantsWP.HELPDIALOG_DIVIDER_BOX_1_W / 2;
			num2 = ConstantsWP.HELPDIALOG_DIVIDER_BOX_1_X - num2;
			ConstantsWP.HELPDIALOG_DIVIDER_BOX_1_X -= num2;
			mCheckbox.mX -= num2;
			mDisableHintLabel.mX -= num2;
			mContainer = new HelpDialogContainer();
			mScrollWidget = new Bej3ScrollWidget(this, false);
			mScrollWidget.Resize(ConstantsWP.HELPDIALOG_CONTAINER_X, ConstantsWP.HELPDIALOG_CONTAINER_Y, ConstantsWP.HELPDIALOG_CONTAINER_WIDTH, ConstantsWP.HELPDIALOG_CONTAINER_HEIGHT + 75);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.AddWidget(mContainer);
			mScrollWidget.EnablePaging(true);
			mScrollWidget.SetScrollInsets(new Insets(0, 0, ConstantsWP.HELPDIALOG_CONTAINER_TAB_PADDING, 0));
			AddWidget(mScrollWidget);
			mCloseButton = new Bej3Button(3, this, Bej3ButtonType.BUTTON_TYPE_LONG, true);
			Bej3Widget.CenterWidgetAt(mWidth / 2, ConstantsWP.HELPDIALOG_CLOSE_Y, mCloseButton, true, false);
			AddWidget(mCloseButton);
			mSlideLeftButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
			mSlideLeftButton.Resize(ConstantsWP.HELPDIALOG_SLIDE_BUTTON_OFFSET_X - 18, ConstantsWP.HELPDIALOG_SLIDE_BUTTON_Y - 15, 0, 0);
			AddWidget(mSlideLeftButton);
			mSlideRightButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
			mSlideRightButton.Resize(0, 0, 0, 0);
			AddWidget(mSlideRightButton);
			for (int i = 0; i < 4; i++)
			{
				mHelpWindow[i] = null;
			}
			mIsBack = false;
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				mIsBack = true;
				ButtonDepress(3);
				mIsBack = false;
			}
		}

		public static Rect GetWindowRect(int window)
		{
			return GetWindowRect(window, false);
		}

		public static Rect GetWindowRect(int window, bool useLargePadding)
		{
			return new Rect(ConstantsWP.HELPDIALOG_CONTAINER_TAB_PADDING + window * (ConstantsWP.HELPDIALOG_CONTAINER_TAB_PADDING + ConstantsWP.HELPDIALOG_CONTAINER_TAB_WIDTH), ConstantsWP.HELPDIALOG_WINDOW_Y, ConstantsWP.HELPDIALOG_CONTAINER_TAB_WIDTH, ConstantsWP.HELPDIALOG_WINDOW_HEIGHT);
		}

		public static string GetContentName(int tutorialFlag)
		{
			switch (tutorialFlag)
			{
			case 0:
			case 21:
				return "Help_Basic";
			case 22:
				return "Help_DiamondMine";
			default:
				return "";
			}
		}

		public static int GetWindowCountForMode(int tutorialFlags)
		{
			switch (tutorialFlags)
			{
			case 0:
			case 10:
			case 18:
			case 20:
			case 21:
			case 22:
				return 3;
			case 17:
				return 2;
			default:
				return 1;
			}
		}

		public void SetupHelp()
		{
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eHELP_DIALOG_ALPHA, HelpWindow.mHelpAlpha);
			mNumWindows = GetWindowCountForMode(mTutorialFlag);
			for (int i = 0; i < mNumWindows; i++)
			{
				mHelpWindow[i] = new HelpWindow();
				mHelpWindow[i].SetVisible(true);
				mHelpWindow[i].Resize(GetWindowRect(i));
				mContainer.AddWidget(mHelpWindow[i]);
			}
			mContainer.SetNumberOfWindows(mNumWindows, false);
			mScrollWidget.ClientSizeChanged();
			mCurrentPage = (mScrollingToPage = 0);
			mScrollWidget.SetPageHorizontal(mCurrentPage, false);
			if (mTutorialFlag == 0 || mTutorialFlag == 21)
			{
				if (mTutorialFlag == 21)
				{
					mHelpWindow[0].mHeaderText = GlobalMembers._ID("Score as many points as possible until there are no more moves.", 243);
				}
				mHelpWindow[0].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_SWAP3);
				mHelpWindow[0].mXOfs.Add(ConstantsWP.HELPDIALOG_OFFSET_BASICS_1);
				mHelpWindow[0].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[0].mCaptions.Add(GlobalMembers._ID("Swap adjacent gems to make rows of three.", 244));
				mHelpWindow[1].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_MATCH4);
				mHelpWindow[1].mXOfs.Add(ConstantsWP.HELPDIALOG_OFFSET_BASICS_2);
				mHelpWindow[1].mTextWidthScale.Add(GlobalMembers.M(0f));
				mHelpWindow[1].mCaptions.Add(GlobalMembers._ID("Match 4 or more gems to create Special Gems.", 245));
				mHelpWindow[2].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_STARGEM);
				mHelpWindow[2].mXOfs.Add(ConstantsWP.HELPDIALOG_OFFSET_BASICS_3);
				mHelpWindow[2].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[2].mCaptions.Add(GlobalMembers._ID("Make an L or T match to create a Star Gem!", 246));
			}
			else if (mTutorialFlag == 10)
			{
				mHelpWindow[0].SetVisible(true);
				mHelpWindow[0].mHeaderText = "";
				mHelpWindow[0].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_LIGHTNING_MATCH);
				mHelpWindow[0].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[0].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[0].mCaptions.Add(GlobalMembers._ID("Match Time Gems to earn extra time.", 247));
				FrameAnimation frameAnimation = new FrameAnimation(GlobalMembersResourcesWP.ATLASIMAGE_EX_HELP_LIGHTNING_01, "atlases\\help_lightning_01.plist");
				mHelpWindow[0].AddWidget(frameAnimation);
				frameAnimation.Resize(94, 30, 190, 248);
				mHelpWindow[1].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_LIGHTNING_TIME);
				mHelpWindow[1].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[1].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[1].mCaptions.Add(GlobalMembers._ID("Extra time is added to your next round, where point values increase!", 248));
				FrameAnimation frameAnimation2 = new FrameAnimation(GlobalMembersResourcesWP.ATLASIMAGE_EX_HELP_LIGHTNING_02, "atlases\\help_lightning_02.plist");
				mHelpWindow[1].AddWidget(frameAnimation2);
				frameAnimation2.Resize(77, 30, 256, 248);
				mHelpWindow[2].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_LIGHTNING_SPEED);
				mHelpWindow[2].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[2].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[2].mCaptions.Add(GlobalMembers._ID("Make matches quickly for a Speed Bonus. Max it out for Blazing Speed!", 249));
				FrameAnimation frameAnimation3 = new FrameAnimation(GlobalMembersResourcesWP.ATLASIMAGE_EX_HELP_LIGHTNING_03, "atlases\\help_lightning_03.plist");
				mHelpWindow[2].AddWidget(frameAnimation3);
				frameAnimation3.Resize(94, 30, 190, 248);
			}
			else if (mTutorialFlag == 17)
			{
				mHelpWindow[0].SetVisible(true);
				mHelpWindow[0].mHeaderText = "";
				mHelpWindow[0].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_BFLY_MATCH);
				mHelpWindow[0].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[0].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[0].mCaptions.Add(GlobalMembers._ID("Match butterfly gems with like colored gems to release them.", 250));
				mHelpWindow[1].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_BFLY_SPIDER);
				mHelpWindow[1].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[1].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[1].mCaptions.Add(GlobalMembers._ID("Don't let any of the butterflies reach the spider!", 251));
			}
			else if (mTutorialFlag == 18)
			{
				mHelpWindow[0].SetVisible(true);
				mHelpWindow[0].mHeaderText = "";
				mHelpWindow[0].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_POKER_MATCH);
				mHelpWindow[0].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[0].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[0].mCaptions.Add(GlobalMembers._ID("Make poker hands with gem matches.", 252));
				mHelpWindow[1].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_POKER_SKULLHAND);
				mHelpWindow[1].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[1].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[1].mCaptions.Add(GlobalMembers._ID("When Skulls appear, try to avoid the hands that they occupy.", 253));
				mHelpWindow[2].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_POKER_SKULL_CLEAR);
				mHelpWindow[2].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[2].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[2].mCaptions.Add(GlobalMembers._ID("Remove Skulls by filling the Eliminator bar.  Better hands fill it faster.", 254));
			}
			else if (mTutorialFlag == 20)
			{
				mHelpWindow[0].SetVisible(true);
				mHelpWindow[0].mHeaderText = GlobalMembers._ID("Score as many points as you can before the ice reaches the top!", 255);
				mHelpWindow[0].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_ICESTORM_HORIZ);
				mHelpWindow[0].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[0].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[0].mCaptions.Add(GlobalMembers._ID("Make matches to push down the rising ice columns.", 256));
				mHelpWindow[1].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_ICESTORM_VERT);
				mHelpWindow[1].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[1].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[1].mCaptions.Add(GlobalMembers._ID("Make vertical matches to smash ice columns and earn mega bonus points.", 257));
				mHelpWindow[2].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_ICESTORM_METER);
				mHelpWindow[2].mXOfs.Add(GlobalMembers.M(0));
				mHelpWindow[2].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[2].mCaptions.Add(GlobalMembers._ID("Clearing ice fills the blue meter and increases your score multiplier.", 258));
			}
			else if (mTutorialFlag == 22)
			{
				mHelpWindow[0].mHeaderText = "";
				mHelpWindow[0].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_DIAMOND_MATCH);
				mHelpWindow[0].mXOfs.Add(ConstantsWP.HELPDIALOG_OFFSET_DIAMOND_1);
				mHelpWindow[0].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[0].mCaptions.Add(GlobalMembers._ID("Make matches directly next to ground to dig down.", 259));
				mHelpWindow[1].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_DIAMOND_ADVANCE);
				mHelpWindow[1].mXOfs.Add(ConstantsWP.HELPDIALOG_OFFSET_DIAMOND_2);
				mHelpWindow[1].mTextWidthScale.Add(GlobalMembers.M(0f));
				mHelpWindow[1].mCaptions.Add(GlobalMembers._ID("Clear all ground tiles down to the white line to advance.", 260));
				mHelpWindow[2].mPopAnims.Add(GlobalMembersResourcesWP.POPANIM_HELP_DIAMOND_GOLD);
				mHelpWindow[2].mXOfs.Add(ConstantsWP.HELPDIALOG_OFFSET_DIAMOND_3);
				mHelpWindow[2].mTextWidthScale.Add(GlobalMembers.M(1f));
				mHelpWindow[2].mCaptions.Add(GlobalMembers._ID("Make matches next to gold, gems and artifacts to earn points.", 261));
			}
			for (int j = 0; j < mNumWindows; j++)
			{
				for (int k = 0; k < Common.size(mHelpWindow[j].mPopAnims); k++)
				{
					PopAnim popAnim = mHelpWindow[j].mPopAnims[k];
					popAnim.mClip = true;
					for (int l = 0; l < Common.size(popAnim.mMainSpriteInst.mParticleEffectVector); l++)
					{
						PAParticleEffect pAParticleEffect = popAnim.mMainSpriteInst.mParticleEffectVector[l];
					}
					for (int m = 0; m < Common.size(popAnim.mMainSpriteInst.mChildren); m++)
					{
						PASpriteInst mSpriteInst = popAnim.mMainSpriteInst.mChildren[m].mSpriteInst;
						if (mSpriteInst != null)
						{
							for (int n = 0; n < Common.size(mSpriteInst.mParticleEffectVector); n++)
							{
								PAParticleEffect pAParticleEffect2 = mSpriteInst.mParticleEffectVector[n];
							}
						}
					}
					mHelpWindow[j].mPopAnims[k].Play();
				}
			}
			LinkUpAssets();
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
		}

		public override void Update()
		{
			base.Update();
			if ((mState == Bej3WidgetState.STATE_FADING_IN || mState == Bej3WidgetState.STATE_IN) && mAllowSlide && !mIsSetUp)
			{
				BejeweledLivePlusApp.LoadContent(GetContentName(mTutorialFlag));
				SetupHelp();
				mIsSetUp = true;
				mContainer.SetVisible(mIsSetUp);
				GlobalMembers.gApp.ClearUpdateBacklog(false);
			}
			SetUpSlideButtons();
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawSwipeInlay(g, mScrollWidget.mY, mScrollWidget.mHeight - 75, mWidth, true);
			mHasDrawn = true;
		}

		public override void DrawOverlay(Graphics g)
		{
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mContainer.LinkUpAssets();
			mCheckbox.SetChecked((GlobalMembers.gApp.mProfile.mTutorialFlags & 0x80000) != 0, false);
			mSlideLeftButton.LinkUpAssets();
			mSlideRightButton.LinkUpAssets();
			mSlideRightButton.Resize(mWidth - mSlideRightButton.mWidth - ConstantsWP.HELPDIALOG_SLIDE_BUTTON_OFFSET_X + 18, ConstantsWP.HELPDIALOG_SLIDE_BUTTON_Y - 15, 0, 0);
			mScrollWidget.SetPageHorizontal(mCurrentPage, true);
			SetUpSlideButtons();
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			base.AllowSlideIn(allow, previousTopButton);
			if (allow && mHelpDialogState == HELPDIALOG_STATE.HELPDIALOG_STATE_PREGAME)
			{
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
			}
		}

		public override void Show()
		{
			mIsSetUp = false;
			BejeweledLivePlusApp.LoadContent(GetContentName(mTutorialFlag));
			mContainer.Show();
			base.Show();
			mTargetPos = 106;
			mY = ConstantsWP.MENU_Y_POS_HIDDEN;
			mContainer.SetVisible(true);
			switch (mHelpDialogState)
			{
			case HELPDIALOG_STATE.HELPDIALOG_STATE_INGAME:
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				break;
			case HELPDIALOG_STATE.HELPDIALOG_STATE_PREGAME:
				if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN)
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
				else
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
				break;
			}
			SetVisible(false);
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
			mSlideLeftButton.EnableSlideGlow(true);
			mSlideRightButton.EnableSlideGlow(true);
		}

		public override void Hide()
		{
			base.Hide();
			mContainer.Hide();
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
			for (int i = 0; i < 4; i++)
			{
				GlobalMembers.KILL_WIDGET(mHelpWindow[i]);
			}
			BejeweledLivePlusApp.UnloadContent(GetContentName(mTutorialFlag));
			GlobalMembers.gApp.mProfile.SetTutorialCleared(mTutorialFlag);
			if (mTutorialFlag == 21)
			{
				GlobalMembers.gApp.mProfile.SetTutorialCleared(0);
			}
			if (mTutorialFlag == 0)
			{
				GlobalMembers.gApp.mProfile.SetTutorialCleared(21);
			}
			GlobalMembers.gApp.mProfile.WriteProfile();
			mContainer.mWindowCount = 0;
		}

		public override void PlayMenuMusic(bool isRestart = false)
		{
		}

		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
			if (mContainer != null)
			{
				mContainer.SetVisible(isVisible);
			}
		}

		public void SetMode(int theTutorialFlag)
		{
			SetMode(theTutorialFlag, true);
		}

		public void SetMode(int theTutorialFlag, bool showCheckbox)
		{
			mTutorialFlag = theTutorialFlag;
			for (int i = 0; i < Common.size(mAnimRefVector); i++)
			{
				mAnimRefVector[i].GetPopAnim().Play();
			}
			mFirstDraw = true;
			mHasDrawn = false;
		}

		public void SetHelpDialogState(HELPDIALOG_STATE state)
		{
			mHelpDialogState = state;
			if (mHelpDialogState == HELPDIALOG_STATE.HELPDIALOG_STATE_PREGAME || mHelpDialogState == HELPDIALOG_STATE.HELPDIALOG_STATE_FIRSTGAME)
			{
				mCloseButton.SetLabel(GlobalMembers._ID("PLAY", 3341));
				mCloseButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			}
			else
			{
				mCloseButton.SetLabel(GlobalMembers._ID("BACK", 3342));
				mCloseButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			}
			Bej3Widget.CenterWidgetAt(mWidth / 2, ConstantsWP.HELPDIALOG_CLOSE_Y, mCloseButton, true, false);
		}

		public override void ButtonDepress(int theId)
		{
			switch (theId)
			{
			case 10001:
				if (mHelpDialogState == HELPDIALOG_STATE.HELPDIALOG_STATE_INGAME)
				{
					GlobalMembers.gApp.GoBackToGame(false);
					Transition_SlideOut();
				}
				else if (mHelpDialogState == HELPDIALOG_STATE.HELPDIALOG_STATE_PREGAME)
				{
					GlobalMembers.gApp.mProfile.SetTutorialCleared(mTutorialFlag);
					GlobalMembers.gApp.DoMainMenu();
					Transition_SlideOut();
				}
				else
				{
					GlobalMembers.gApp.DoMainMenu();
					Transition_SlideOut();
				}
				break;
			case 2:
				mScrollingToPage = mScrollWidget.GetPageHorizontal() + 1;
				mScrollWidget.SetPageHorizontal(mScrollingToPage, true);
				break;
			case 1:
				mScrollingToPage = mScrollWidget.GetPageHorizontal() - 1;
				mScrollWidget.SetPageHorizontal(mScrollingToPage, true);
				break;
			case 3:
				HandleCloseButton();
				break;
			}
		}

		public void CheckboxChecked(int theId, bool Checked)
		{
			if (theId == 0)
			{
				GlobalMembers.gApp.mProfile.SetTutorialCleared(19, Checked);
			}
			LinkUpAssets();
		}

		public void PageChanged(Bej3ScrollWidget scrollWidget, int pageH, int pageV)
		{
			for (int i = 0; i < 4 && mHelpWindow[i] != null; i++)
			{
				if (i == pageH)
				{
					mHelpWindow[i].mSeenByUser = true;
					mHelpWindow[i].SetVisible(true);
				}
				else
				{
					mHelpWindow[i].mSeenByUser = false;
				}
			}
			mCurrentPage = pageH;
			SetUpSlideButtons();
		}

		public void ScrollTargetReached(ScrollWidget scrollWidget)
		{
			int pageHorizontal = scrollWidget.GetPageHorizontal();
			if (mCurrentPage != pageHorizontal)
			{
				mCurrentPage = pageHorizontal;
				LinkUpAssets();
			}
			int pageHorizontal2 = scrollWidget.GetPageHorizontal();
			for (int i = 0; i < 4 && mHelpWindow[i] != null; i++)
			{
				if (i == pageHorizontal2)
				{
					mHelpWindow[i].mSeenByUser = true;
					continue;
				}
				mHelpWindow[i].mSeenByUser = false;
				mHelpWindow[i].ResetAnimation();
			}
		}

		public void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}
	}
}
