using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class HighScoresMenu : Bej3Widget, Bej3ScrollWidgetListener, ScrollWidgetListener
	{
		private enum HIGHSCORESMENU_BUTTON_IDS
		{
			BTN_LEFT_ID,
			BTN_RIGHT_ID,
			BTN_XBL_ID,
			BTN_MAINMENU_ID,
			BTN_PLAYAGAIN_ID,
			BTN_TODAY_ID,
			BTN_WEEK_ID,
			BTN_ALLTIME_ID
		}

		private Bej3ScrollWidget mScrollWidget;

		private HighScoresMenuContainer mContainer;

		private bool mAllowScrolling;

		private int mCurrentPage;

		private int mScrollingToPage;

		private bool wasDown;

		private Label mHeadingLabel;

		private Label mBottomMessageLabel;

		private Bej3Button mMainMenuButton;

		private Bej3Button mPlayAgainButton;

		private Bej3Button mSlideLeftButton;

		private Bej3Button mSlideRightButton;

		private Bej3Button mByTodayButton;

		private Bej3Button mByAllTimeButton;

		private Bej3Button mXBLButton;

		private void SetUpSlideButtons()
		{
			int pageHorizontal = mScrollWidget.GetPageHorizontal();
			bool flag = pageHorizontal > 0;
			mSlideLeftButton.SetVisible(flag);
			mSlideLeftButton.SetDisabled(!flag);
			flag = pageHorizontal < 3;
			mSlideRightButton.SetVisible(flag);
			mSlideRightButton.SetDisabled(!flag);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public override void Update()
		{
			base.Update();
			SetUpSlideButtons();
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawSwipeInlay(g, mScrollWidget.mY, mScrollWidget.mHeight - 75, mWidth, true);
		}

		public override void DrawOverlay(Graphics g)
		{
			float num = (float)(double)mAlphaCurve;
			g.SetColorizeImages(true);
			g.SetColor(new Color(255, 255, 255, (int)(255f * num)));
		}

		public override void ButtonMouseEnter(int theId)
		{
		}

		public override void ButtonDepress(int theId)
		{
			switch (theId)
			{
			case 10001:
				GlobalMembers.gApp.DoMainMenu();
				Transition_SlideOut();
				break;
			case 3:
				GlobalMembers.gApp.DoMainMenu();
				Transition_SlideOut();
				break;
			case 4:
			{
				GameMode gameMode = (GameMode)mScrollWidget.GetPageHorizontal();
				if (gameMode == GameMode.MODE_ZEN)
				{
					gameMode = GameMode.MODE_BUTTERFLY;
				}
				GlobalMembers.gApp.DoNewGame(gameMode);
				Transition_SlideOut();
				break;
			}
			case 1:
				mScrollingToPage = mScrollWidget.GetPageHorizontal() + 1;
				mScrollWidget.SetPageHorizontal(mScrollingToPage, true);
				mContainer.SelectModeView(mContainer.mCurrentDisplayMode + 1);
				break;
			case 0:
				mScrollingToPage = mScrollWidget.GetPageHorizontal() - 1;
				mScrollWidget.SetPageHorizontal(mScrollingToPage, true);
				mContainer.SelectModeView(mContainer.mCurrentDisplayMode - 1);
				break;
			case 5:
				if (!GlobalMembers.isLeaderboardLoading)
				{
					mContainer.SelectTimeView(HighScoreTable.HighScoreTableTime.TIME_RECENT);
					mByTodayButton.HighLighted(true);
					mByAllTimeButton.HighLighted(false);
				}
				break;
			case 7:
				if (!GlobalMembers.isLeaderboardLoading)
				{
					mContainer.SelectTimeView(HighScoreTable.HighScoreTableTime.TIME_ALLTIME);
					mByTodayButton.HighLighted(false);
					mByAllTimeButton.HighLighted(true);
				}
				break;
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mMainMenuButton.LinkUpAssets();
			mPlayAgainButton.LinkUpAssets();
			mContainer.LinkUpAssets();
			mSlideLeftButton.LinkUpAssets();
			mSlideRightButton.LinkUpAssets();
			mSlideRightButton.Resize(mWidth - mSlideRightButton.mWidth - ConstantsWP.HIGHSCORES_MENU_SLIDE_BUTTON_OFFSET_X + 10, ConstantsWP.HIGHSCORES_MENU_SLIDE_BUTTON_Y, 0, 0);
			mScrollWidget.SetPageHorizontal(mCurrentPage, true);
			SetUpSlideButtons();
		}

		public override void Show()
		{
			mContainer.Show();
			base.Show();
			SetVisible(false);
			ResetFadedBack(true);
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
			mSlideLeftButton.EnableSlideGlow(true);
			mSlideRightButton.EnableSlideGlow(true);
			mContainer.SelectModeView((HighScoresMenuContainer.HSMODE)mCurrentPage);
		}

		public override void Hide()
		{
			base.Hide();
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
			int pageHorizontal = scrollWidget.GetPageHorizontal();
			if (mCurrentPage != pageHorizontal)
			{
				mCurrentPage = pageHorizontal;
				LinkUpAssets();
			}
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public virtual void PageChanged(Bej3ScrollWidget scrollWidget, int pageH, int pageV)
		{
			if (pageH >= 0 && pageH < 4 && pageH != mCurrentPage)
			{
				mCurrentPage = pageH;
				mContainer.SelectModeView((HighScoresMenuContainer.HSMODE)mCurrentPage);
				SetUpSlideButtons();
			}
		}

		public HighScoresMenu()
			: base(Menu_Type.MENU_HIGHSCORESMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			mAllowScrolling = true;
			mCurrentPage = 0;
			mScrollingToPage = 0;
			wasDown = false;
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, ConstantsWP.HIGHSCORES_MENU_WIDTH, GlobalMembers.gApp.mHeight);
			mFinalY = 0;
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.HIGHSCORES_MENU_HEADING_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetText(GlobalMembers._ID("LeaderBoards", 3381));
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			mBottomMessageLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mBottomMessageLabel.SetTextBlock(new Rect(ConstantsWP.HIGHSCORES_MENU_BOTTOM_MESSAGE_X, ConstantsWP.HIGHSCORES_MENU_BOTTOM_MESSAGE_Y, ConstantsWP.SLIDE_BUTTON_MESSAGE_WIDTH, 0), true);
			mBottomMessageLabel.SetTextBlockEnabled(true);
			mBottomMessageLabel.SetText(GlobalMembers._ID("Swipe for more leaderboards", 3344));
			mBottomMessageLabel.SetClippingEnabled(false);
			mBottomMessageLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_3_FILL);
			mBottomMessageLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_3_STROKE);
			AddWidget(mBottomMessageLabel);
			mContainer = new HighScoresMenuContainer();
			mScrollWidget = new Bej3ScrollWidget(this, false);
			mScrollWidget.Resize(ConstantsWP.HIGHSCORES_MENU_CONTAINER_PADDING_X, ConstantsWP.HIGHSCORES_MENU_CONTAINER_TOP, mWidth - ConstantsWP.HIGHSCORES_MENU_CONTAINER_PADDING_X * 2, GlobalMembers.gApp.mHeight - ConstantsWP.HIGHSCORES_MENU_CONTAINER_BOTTOM - ConstantsWP.HIGHSCORES_MENU_CONTAINER_TOP + 75);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.EnablePaging(true);
			mScrollWidget.SetScrollInsets(new Insets(0, 0, 0, 0));
			mScrollWidget.AddWidget(mContainer);
			mScrollWidget.SetPageHorizontal(0, false);
			AddWidget(mScrollWidget);
			mMainMenuButton = new Bej3Button(3, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mMainMenuButton.SetLabel(GlobalMembers._ID("MAIN MENU", 3293));
			Bej3Widget.CenterWidgetAt(ConstantsWP.HIGHSCORES_MENU_MODE_BTN_BACK_X - 155, ConstantsWP.HIGHSCORES_MENU_MODE_BTN_BACK_Y + 110, mMainMenuButton, true, false);
			AddWidget(mMainMenuButton);
			mPlayAgainButton = new Bej3Button(4, this, Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			mPlayAgainButton.SetLabel(GlobalMembers._ID("PLAY", 3290));
			Bej3Widget.CenterWidgetAt(ConstantsWP.HIGHSCORES_MENU_MODE_BTN_BACK_X + 155, ConstantsWP.HIGHSCORES_MENU_MODE_BTN_BACK_Y + 110, mPlayAgainButton, true, false);
			AddWidget(mPlayAgainButton);
			mSlideLeftButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
			mSlideLeftButton.Resize(ConstantsWP.HIGHSCORES_MENU_SLIDE_BUTTON_OFFSET_X - 10, ConstantsWP.HIGHSCORES_MENU_SLIDE_BUTTON_Y, 0, 0);
			AddWidget(mSlideLeftButton);
			mSlideRightButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
			mSlideRightButton.Resize(0, 0, 0, 0);
			AddWidget(mSlideRightButton);
			mByTodayButton = new Bej3Button(5, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mByTodayButton.SetLabel(GlobalMembers._ID("Recent", 7789));
			mByTodayButton.Resize(0, 0, 290, 18);
			Bej3Widget.CenterWidgetAt(170, 210, mByTodayButton, true, false);
			AddWidget(mByTodayButton);
			GlobalMembers.mByTodayButton = mByTodayButton;
			mByTodayButton.HighLighted(true);
			mByAllTimeButton = new Bej3Button(7, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mByAllTimeButton.SetLabel(GlobalMembers._ID("AllTime", 7791));
			mByAllTimeButton.Resize(0, 0, 290, 18);
			Bej3Widget.CenterWidgetAt(GlobalMembers.gApp.mWidth - 170, 210, mByAllTimeButton, true, false);
			AddWidget(mByAllTimeButton);
			GlobalMembers.mByAllTimeButton = mByAllTimeButton;
			if (GlobalMembers.gApp.mGameCenterIsAvailable)
			{
				mXBLButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_GAMECENTER);
				Bej3Widget.CenterWidgetAt(GlobalMembers.gApp.mWidth - 80, 170, mXBLButton);
				AddWidget(mXBLButton);
			}
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				ButtonDepress(10001);
			}
		}
	}
}
