using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
// using Microsoft.Xna.Framework.GamerServices;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus.UI
{
	public class MainMenuScrollContainer : Bej3Widget, Bej3ButtonListener, ButtonListener, ScrollWidgetListener
	{
		public enum MAINMENU_BUTTON_IDS
		{
			BTN_CLASSIC_ID,
			BTN_ZEN_ID,
			BTN_DIAMOND_MINE_ID,
			BTN_LIGHTNING_ID,
			BTN_BLITZ_ID,
			BTN_BUTTERFLY_ID,
			BTN_COMING_SOON_ID,
			BTN_MORE_1_ID,
			BTN_MORE_2_ID,
			BTN_MORE_3_ID,
			BTN_MORE_4_ID,
			BTN_MORE_5_ID
		}

		public enum PAGE_NUM
		{
			MAIN_PAGE,
			EXPEND_PAGE
		}

		private bool mSlide_Left;

		private bool mSlide_Right;

		private int mSlide_Count;

		private List<CrystalBall> mButtons;

		public CrystalBall mClassicButton;

		private CrystalBall mLeaderBoardButton;

		private CrystalBall mAchievementButton;

		private CrystalBall mBuyFullGameButton;

		public bool mIsFullGame = true; // !Guide.IsTrialMode;

		private CrystalBall mButterflyButton;

		private CrystalBall mZenButton;

		private CrystalBall mDiamondMineButton;

		private CrystalBall mLightningButton;

		private CrystalBall mComingSoonButton;

		private ArrowButton Tst1Button;

		private ArrowButton Tst2Button;

		private int mCrystalBallCountdown;

		private PAGE_NUM mCurrentPage;

		public ScrollWidget mScrollWidget;

		public bool mFlag;

		public static CurvedVal alpha = new CurvedVal();

		public static bool hasValue = false;

		public int CurrentPage => (int)mCurrentPage;

		public MainMenuScrollContainer(MainMenu parent)
			: base(Menu_Type.MENU_MAINMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mButtons = new List<CrystalBall>();
			mCrystalBallCountdown = 150;
			mDoesSlideInFromBottom = (mCanAllowSlide = false);
			mZenButton = new CrystalBall(GlobalMembers._ID("ZEN", 3366), GlobalMembers._ID("", 3367), GlobalMembers._ID("", 3368), 1, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_ZEN_SCALE + 0.1f);
			mButtons.Add(mZenButton);
			AddWidget(mZenButton);
			mDiamondMineButton = new CrystalBall(GlobalMembers._ID("DIAMOND", 3369), GlobalMembers._ID("MINE", 3370), GlobalMembers._ID("", 3371), 2, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_DIAMONDMINE_SCALE);
			mButtons.Add(mDiamondMineButton);
			AddWidget(mDiamondMineButton);
			mClassicButton = new CrystalBall(GlobalMembers._ID("CLASSIC", 3372), GlobalMembers._ID("", 3373), GlobalMembers._ID("", 3374), 0, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_SCALE);
			mButtons.Add(mClassicButton);
			AddWidget(mClassicButton);
			mLightningButton = new CrystalBall(GlobalMembers._ID("LIGHTNING", 3375), GlobalMembers._ID("", 3376), GlobalMembers._ID("", 3377), 3, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_LIGHTNING_SCALE);
			mButtons.Add(mLightningButton);
			AddWidget(mLightningButton);
			mButterflyButton = new CrystalBall(GlobalMembers._ID("BUTTERFLIES", 3378), GlobalMembers._ID("", 3379), GlobalMembers._ID("", 3380), 5, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_SCALE + 0.1f);
			mButtons.Add(mButterflyButton);
			AddWidget(mButterflyButton);
			mLeaderBoardButton = new CrystalBall(GlobalMembers._ID("LeaderBoards", 3381), GlobalMembers._ID("", 3382), GlobalMembers._ID("", 3383), 7, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_SCALE - 0.3f);
			mButtons.Add(mLeaderBoardButton);
			AddWidget(mLeaderBoardButton);
			mAchievementButton = new CrystalBall(GlobalMembers._ID("Achievements", 3384), GlobalMembers._ID("", 3385), GlobalMembers._ID("", 3386), 8, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_SCALE - 0.3f);
			mButtons.Add(mAchievementButton);
			AddWidget(mAchievementButton);
			mBuyFullGameButton = new CrystalBall(GlobalMembers._ID("Buy", 3387), GlobalMembers._ID("FullGame", 3388), GlobalMembers._ID("", 3389), 9, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_SCALE);
			mBuyFullGameButton.mFontScale = 0.9f;
			mButtons.Add(mBuyFullGameButton);
			AddWidget(mBuyFullGameButton);
			Tst1Button = new ArrowButton(GlobalMembers._ID("Bonus", 3393), GlobalMembers._ID("Games", 3394), GlobalMembers._ID("", 3392), 10, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_SCALE - 0.3f);
			Tst1Button.Resize(0, 0, 180, 180);
			AddWidget(Tst1Button);
			Tst2Button = new ArrowButton(GlobalMembers._ID("Help &", 3390), GlobalMembers._ID("About", 3391), GlobalMembers._ID("", 7669), 11, this, Bej3Widget.COLOR_CRYSTALBALL_FONT, ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_SCALE - 0.3f);
			Tst2Button.Resize(0, 0, 180, 180);
			Tst2Button.IsLeft = true;
			AddWidget(Tst2Button);
			initButtonPosition();
			mCurrentPage = PAGE_NUM.MAIN_PAGE;
			mFlag = true;
			mSlide_Left = false;
			mSlide_Right = false;
			mSlide_Count = 0;
			for (int i = 0; i < Common.size(mButtons); i++)
			{
				mButtons[i].SetVisible(false);
				mButtons[i].mAlpha = mAlphaCurve;
			}
			mScrollWidget = new ScrollWidget(this);
			mScrollWidget.Resize(ConstantsWP.BADGEMENU_CONTAINER_PADDING_X, ConstantsWP.BADGEMENU_CONTAINER_TOP, mWidth - ConstantsWP.BADGEMENU_CONTAINER_PADDING_X * 2, ConstantsWP.BADGEMENU_CONTAINER_HEIGHT);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.EnablePaging(true);
			mScrollWidget.SetScrollInsets(new Insets(0, 0, 0, 0));
			mScrollWidget.SetPageHorizontal(0, false);
		}

		public void initButtonPosition()
		{
			int num = 40;
			int num2 = 40;
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_BLITZ_X - 75, ConstantsWP.MAIN_MENU_BUTTON_BLITZ_Y + 470 + num, Tst2Button);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_DIAMONDMINE_X + 75, ConstantsWP.MAIN_MENU_BUTTON_BLITZ_Y + 470 + num, Tst1Button);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_X, ConstantsWP.MAIN_MENU_BUTTON_BLITZ_Y + 410 + num, mBuyFullGameButton);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_X, ConstantsWP.MAIN_MENU_BUTTON_BLITZ_Y + 250 + num, mAchievementButton);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_X, ConstantsWP.MAIN_MENU_BUTTON_BLITZ_Y + 120 + num, mLeaderBoardButton);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_X + 2 * ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_X, ConstantsWP.MAIN_MENU_BUTTON_BUTTERFLIES_Y + num - num2, mButterflyButton);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_BLITZ_X, ConstantsWP.MAIN_MENU_BUTTON_BLITZ_Y + num, mLightningButton);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_X, ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_Y + num, mClassicButton);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_DIAMONDMINE_X, ConstantsWP.MAIN_MENU_BUTTON_DIAMONDMINE_Y + num, mDiamondMineButton);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAIN_MENU_BUTTON_ZEN_X + 2 * ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_X, ConstantsWP.MAIN_MENU_BUTTON_ZEN_Y + num + num2, mZenButton);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public override void Update()
		{
			base.Update();
			if (mSlide_Left)
			{
				if ((float)mSlide_Count < ConstantsWP.DEVICE_WIDTH_F / 20f)
				{
					for (int i = 0; i < Common.size(mButtons); i++)
					{
						mButtons[i].mX -= 20;
					}
					mSlide_Count++;
				}
				else
				{
					mSlide_Count = 0;
					mSlide_Left = false;
				}
			}
			if (mSlide_Right)
			{
				if ((float)mSlide_Count < ConstantsWP.DEVICE_WIDTH_F / 20f)
				{
					for (int j = 0; j < Common.size(mButtons); j++)
					{
						mButtons[j].mX += 20;
					}
					mSlide_Count++;
				}
				else
				{
					mSlide_Count = 0;
					mSlide_Right = false;
				}
			}
			if (mFlag)
			{
				if (!hasValue)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_SCROLL_CONTAINER_UPDATE_ALPHA, alpha);
					hasValue = true;
				}
				int count = mButtons.Count;
				for (int k = 0; k < Common.size(mButtons); k++)
				{
					mButtons[k].SetVisible(true);
					mButtons[k].mAlpha = alpha;
				}
				((MainMenu)GlobalMembers.gApp.mMenus[2]).ShowLogo();
				((MainMenuOptions)GlobalMembers.gApp.mMenus[5]).mCanSlideIn = true;
				GlobalMembers.gApp.ClearUpdateBacklog(false);
				mFlag = false;
			}
			mBuyFullGameButton.SetVisible(!mIsFullGame);
			mLeaderBoardButton.SetDisabled(!mIsFullGame);
			mLeaderBoardButton.mFontColor.SetColor(mIsFullGame ? Bej3Widget.COLOR_CRYSTALBALL_FONT.mRed : 127, mIsFullGame ? Bej3Widget.COLOR_CRYSTALBALL_FONT.mGreen : 127, mIsFullGame ? Bej3Widget.COLOR_CRYSTALBALL_FONT.mBlue : 127, 255);
			mAchievementButton.SetDisabled(!mIsFullGame);
			mAchievementButton.mFontColor.SetColor(mIsFullGame ? Bej3Widget.COLOR_CRYSTALBALL_FONT.mRed : 127, mIsFullGame ? Bej3Widget.COLOR_CRYSTALBALL_FONT.mGreen : 127, mIsFullGame ? Bej3Widget.COLOR_CRYSTALBALL_FONT.mBlue : 127, 255);
		}

		public override void Draw(Graphics g)
		{
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
		}

		public void EnterMain()
		{
			mFlag = true;
			mCurrentPage = PAGE_NUM.MAIN_PAGE;
			initButtonPosition();
		}

		public override void Show()
		{
			if (mInterfaceState != 0)
			{
				Tst1Button.mLabel = GlobalMembers._ID("Bonus", 3393);
				Tst1Button.mLabel2 = GlobalMembers._ID("Games", 3394);
				Tst2Button.mLabel = GlobalMembers._ID("Help &", 3390);
				Tst2Button.mLabel2 = GlobalMembers._ID("About", 3391);
				Tst2Button.mLabel3 = GlobalMembers._ID("", 7669);
				EnterMain();
				base.Show();
				mY = 0;
			}
		}

		public void EnableButtons(bool enabled)
		{
			for (int i = 0; i < Common.size(mButtons); i++)
			{
				mButtons[i].mDisabled = !enabled;
			}
		}

		public override void ButtonDepress(int theId)
		{
			if (Common.size(GlobalMembers.gApp.mTooltipManager.mTooltips) > 0)
			{
				GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
			}
			if (GlobalMembers.gApp.mGameInProgress || GlobalMembers.gApp.mMenus[5].IsTransitioning())
			{
				return;
			}
			switch (theId)
			{
			case 0:
				GlobalMembers.gApp.DoNewGame(GameMode.MODE_CLASSIC);
				break;
			case 1:
				if (mIsFullGame)
				{
					GlobalMembers.gApp.DoNewGame(GameMode.MODE_ZEN);
				}
				else
				{
					GlobalMembers.gApp.DoTrialDialog(theId);
				}
				break;
			case 2:
				GlobalMembers.gApp.DoNewGame(GameMode.MODE_DIAMOND_MINE);
				break;
			case 3:
				if (mIsFullGame)
				{
					GlobalMembers.gApp.DoNewGame(GameMode.MODE_LIGHTNING);
				}
				else
				{
					GlobalMembers.gApp.DoTrialDialog(theId);
				}
				break;
			case 4:
			{
				GlobalMembers.gApp.GoToBlitz();
				Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.GetDialog(48);
				break;
			}
			case 5:
				GlobalMembers.gApp.DoNewGame(GameMode.MODE_BUTTERFLY);
				break;
			case 6:
			{
				Bej3Dialog bej3Dialog2 = (Bej3Dialog)GlobalMembers.gApp.DoDialog(0, true, GlobalMembers._ID("Coming Soon", 3399), GlobalMembers._ID("We're working hard to bring you more game modes in future updates. Stay tuned!", 3400), GlobalMembers._ID("BACK", 3606), 3, 3, 3);
				GlobalMembers.gApp.mMenus[5].Transition_SlideOut();
				break;
			}
			case 7:
				if (mIsFullGame)
				{
					GlobalMembers.gApp.DoHighScoresMenu();
				}
				else
				{
					GlobalMembers.gApp.DoTrialDialog(theId);
				}
				break;
			case 8:
				if (mIsFullGame)
				{
					GlobalMembers.gApp.DoBadgeMenu(2, GlobalMembers.gApp.mProfile.mDeferredBadgeVector);
				}
				else
				{
					GlobalMembers.gApp.DoTrialDialog(theId);
				}
				break;
			case 9:
				// if (Guide.IsTrialMode)
				// {
				// 	GlobalMembers.gApp.BuyGame();
				// }
				// mIsFullGame = !Guide.IsTrialMode;
				break;
			case 10:
				if (mCurrentPage == PAGE_NUM.MAIN_PAGE)
				{
					if (!mSlide_Right)
					{
						Tst1Button.mLabel = GlobalMembers._ID("More", 3395);
						Tst1Button.mLabel2 = GlobalMembers._ID("Games", 3396);
						Tst2Button.mLabel = GlobalMembers._ID("Back", 3640);
						Tst2Button.mLabel2 = "";
						Tst2Button.mLabel3 = "";
						mCurrentPage = PAGE_NUM.EXPEND_PAGE;
						mFlag = true;
						mSlide_Left = true;
					}
				}
				else if (!mSlide_Left)
				{
					GlobalMembers.gApp.OpenURLWithWarning(GlobalMembers._ID("http://mg.eamobile.com/?rId=1560", 8888));
				}
				break;
			case 11:
				if (mCurrentPage == PAGE_NUM.EXPEND_PAGE)
				{
					if (!mSlide_Left)
					{
						Tst1Button.mLabel = GlobalMembers._ID("Bonus", 3393);
						Tst1Button.mLabel2 = GlobalMembers._ID("Games", 3394);
						Tst2Button.mLabel = GlobalMembers._ID("Help &", 3390);
						Tst2Button.mLabel2 = GlobalMembers._ID("About", 3391);
						Tst2Button.mLabel3 = "";
						mCurrentPage = PAGE_NUM.MAIN_PAGE;
						mFlag = true;
						mSlide_Right = true;
					}
				}
				else if (!mSlide_Right)
				{
					GlobalMembers.gApp.DoOptionsMenu();
				}
				break;
			}
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
			scrollWidget.mIsDown = false;
			scrollWidget.ScrollToPoint(new Point(ConstantsWP.MAIN_MENU_TAB_WIDTH * MainMenu.mScrollwidgetPage, 0), true);
		}

		public override void TouchBegan(SexyAppBase.Touch touch)
		{
			base.TouchBegan(touch);
			GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mClassicButton.LinkUpAssets();
			mZenButton.LinkUpAssets();
			mDiamondMineButton.LinkUpAssets();
		}

		public override int GetShowCurve()
		{
			return 25;
		}

		public void MakeButtonsFullyVisible()
		{
			for (int i = 0; i < Common.size(mButtons); i++)
			{
				mButtons[i].mAlpha.SetConstant(1.0);
			}
		}
	}
}
