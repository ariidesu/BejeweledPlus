using System.Collections.Generic;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class GameDetailMenu : Bej3Widget, Bej3ScrollWidgetListener, ScrollWidgetListener
	{
		public enum GAMEDETAILMENU_IDS
		{
			BTN_PLAY_ID,
			BTN_BACK_ID,
			BTN_RIGHT_ID,
			BTN_LEFT_ID
		}

		public enum GAMEDETAILMENU_STATE
		{
			STATE_PRE_GAME,
			STATE_POST_GAME
		}

		public const int MAX_TABS = 2;

		protected GameMode mMode;

		protected GAMEDETAILMENU_STATE mGameMenuState;

		protected Bej3Button mPlayButton;

		protected Bej3Button mBackButton;

		protected Label mHeadingLabel;

		protected HighScoresWidget mHighScoresWidgetPreGame;

		protected List<SexyFramework.Widget.Widget> mDefaultWidgetsPreGame = new List<SexyFramework.Widget.Widget>();

		protected List<SexyFramework.Widget.Widget> mDefaultWidgetsPostGame = new List<SexyFramework.Widget.Widget>();

		protected Bej3Button mSlideLeftButton;

		protected Bej3Button mSlideRightButton;

		protected Label mSwipeMsgLabel;

		protected Label mModeDescriptionLabel;

		protected Bej3ScrollWidget mScrollWidget;

		protected GameDetailMenuContainer mEndGameContainer;

		protected bool mAllowScrolling;

		protected bool wasDown;

		private bool mIgnoreSetMode;

		private int ScrollTargetReached_lastPage = -1;

		private void Init()
		{
			mIgnoreSetMode = false;
			mMode = GameMode.MODE_CLASSIC;
			mGameMenuState = GAMEDETAILMENU_STATE.STATE_PRE_GAME;
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			AddWidget(mHeadingLabel);
			mPlayButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			AddWidget(mPlayButton);
			mBackButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3289));
			AddWidget(mBackButton);
			mScrollWidget = new Bej3ScrollWidget(this, false);
			mEndGameContainer = new GameDetailMenuContainer();
			mScrollWidget.Resize(ConstantsWP.GAMEDETAILMENU_POST_GAME_CONTAINER_X, ConstantsWP.GAMEDETAILMENU_POST_GAME_CONTAINER_Y, ConstantsWP.GAMEDETAILMENU_POST_GAME_CONTAINER_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_CONTAINER_HEIGHT + 75);
			mScrollWidget.AddWidget(mEndGameContainer);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.EnablePaging(true);
			mScrollWidget.SetScrollInsets(new Insets(0, 0, 0, 0));
			mScrollWidget.SetPageHorizontal(0, false);
			AddWidget(mScrollWidget);
			mDefaultWidgetsPostGame.Add(mScrollWidget);
			mDefaultWidgetsPostGame.Add(mEndGameContainer);
			mModeDescriptionLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mModeDescriptionLabel.SetTextBlockEnabled(true);
			mModeDescriptionLabel.SetTextBlock(new Rect(ConstantsWP.GAMEDETAILMENU_MODE_DESCRIPTION_X, ConstantsWP.GAMEDETAILMENU_MODE_DESCRIPTION_Y, ConstantsWP.GAMEDETAILMENU_MODE_DESCRIPTION_WIDTH, ConstantsWP.GAMEDETAILMENU_MODE_DESCRIPTION_HEIGHT), true);
			mModeDescriptionLabel.SetClippingEnabled(false);
			mDefaultWidgetsPreGame.Add(mModeDescriptionLabel);
			AddWidget(mModeDescriptionLabel);
			mSlideLeftButton = new Bej3Button(3, this, Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
			mSlideLeftButton.Resize(ConstantsWP.GAMEDETAILMENU_POST_GAME_SLIDE_BUTTON_OFFSET_X, ConstantsWP.GAMEDETAILMENU_POST_GAME_SLIDE_BUTTON_Y, 0, 0);
			AddWidget(mSlideLeftButton);
			mDefaultWidgetsPostGame.Add(mSlideLeftButton);
			mSlideRightButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
			mSlideRightButton.Resize(0, 0, 0, 0);
			AddWidget(mSlideRightButton);
			mDefaultWidgetsPostGame.Add(mSlideRightButton);
			mHighScoresWidgetPreGame = new HighScoresWidget(new Rect(ConstantsWP.GAMEDETAILMENU_HIGHSCORES_X, ConstantsWP.GAMEDETAILMENU_HIGHSCORES_Y, ConstantsWP.GAMEDETAILMENU_HIGHSCORES_WIDTH, ConstantsWP.GAMEDETAILMENU_HIGHSCORES_HEIGHT), false);
			mDefaultWidgetsPreGame.Add(mHighScoresWidgetPreGame);
			AddWidget(mHighScoresWidgetPreGame);
			mSwipeMsgLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, string.Empty);
			mSwipeMsgLabel.SetTextBlock(new Rect(ConstantsWP.GAMEDETAILMENU_POST_GAME_SWIPE_MSG_X, ConstantsWP.GAMEDETAILMENU_POST_GAME_SWIPE_MSG_Y_1 - 50, ConstantsWP.SLIDE_BUTTON_MESSAGE_WIDTH, 100), true);
			mSwipeMsgLabel.SetTextBlockEnabled(true);
			mSwipeMsgLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_3_FILL);
			mSwipeMsgLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_3_STROKE);
			AddWidget(mSwipeMsgLabel);
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
		}

		private void SetUpSlideButtons()
		{
			if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_POST_GAME)
			{
				mHeadingLabel.SetVisible(true);
				mSlideRightButton.Resize(mWidth - mSlideRightButton.mWidth - ConstantsWP.GAMEDETAILMENU_POST_GAME_SLIDE_BUTTON_OFFSET_X, ConstantsWP.GAMEDETAILMENU_POST_GAME_SLIDE_BUTTON_Y, 0, 0);
				int pageHorizontal = mScrollWidget.GetPageHorizontal();
				bool flag = pageHorizontal > 0;
				mSlideLeftButton.SetVisible(flag);
				mSlideLeftButton.SetDisabled(!flag);
				flag = pageHorizontal < 1;
				mSlideRightButton.SetVisible(flag);
				mSlideRightButton.SetDisabled(!flag);
				mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
				mHeadingLabel.Resize(ConstantsWP.GAMEDETAILMENU_HEADING_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
				switch (pageHorizontal)
				{
				case 0:
					mSwipeMsgLabel.SetText(GlobalMembers._ID("Swipe for stats", 3294));
					break;
				case 1:
					mSwipeMsgLabel.SetText(GlobalMembers._ID("Swipe for top scores", 3295));
					break;
				}
			}
		}

		protected GameDetailMenu(Menu_Type type, bool hasBackButton, Bej3ButtonType topButtonType)
			: base(type, hasBackButton, topButtonType)
		{
			mAllowScrolling = true;
			wasDown = false;
			Init();
			LinkUpAssets();
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		public GameDetailMenu()
			: base(Menu_Type.MENU_GAMEDETAILMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			Init();
			LinkUpAssets();
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back)
			{
				args.processed = true;
				ButtonDepress(10001);
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			if (mY < ConstantsWP.MENU_Y_POS_HIDDEN)
			{
				base.DrawAll(theFlags, g);
				return;
			}
			Draw(g);
			if (mTopButton != null)
			{
				g.Translate(mTopButton.mX, mTopButton.mY);
				mTopButton.Draw(g);
				g.Translate(-mTopButton.mX, -mTopButton.mY);
			}
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			double num = (double)mAlphaCurve;
			Bej3Widget.DrawDialogBox(g, mWidth);
			if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_POST_GAME)
			{
				Bej3Widget.DrawSwipeInlay(g, mScrollWidget.mY, mScrollWidget.mHeight - 75, mWidth, true);
			}
			g.PopState();
		}

		public override void UpdateAll(ModalFlags theFlags)
		{
			if (!mVisible)
			{
				return;
			}
			if (mY < ConstantsWP.MENU_Y_POS_HIDDEN || mState == Bej3WidgetState.STATE_FADING_IN)
			{
				base.UpdateAll(theFlags);
				return;
			}
			Update();
			if (mTopButton != null)
			{
				mTopButton.Update();
			}
		}

		public override void Update()
		{
			base.Update();
			if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_POST_GAME)
			{
				SetUpSlideButtons();
			}
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			bool mAllowSlide2 = mAllowSlide;
			base.AllowSlideIn(allow, previousTopButton);
			if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_PRE_GAME)
			{
				if (!mAllowSlide && allow && mState == Bej3WidgetState.STATE_FADING_IN)
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
			}
			else
			{
				if (previousTopButton != null)
				{
					SetTopButtonType(previousTopButton.GetType());
				}
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			}
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
			case 1:
				GlobalMembers.gApp.DoMainMenu();
				Transition_SlideOut();
				break;
			case 0:
				if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_PRE_GAME)
				{
					GlobalMembers.gApp.StartSetupGame(true, true);
				}
				else if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_POST_GAME)
				{
					if (GlobalMembers.gApp.mBoard != null)
					{
						GlobalMembers.gApp.mBoard.mShouldUnloadContentWhenDone = false;
					}
					mIgnoreSetMode = true;
					GlobalMembers.gApp.DoNewGame(mMode);
					GlobalMembers.gApp.mBoard.mAlphaCurve.SetConstant(1.0);
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
					mIgnoreSetMode = false;
					Transition_SlideOut();
				}
				if (mInterfaceState != InterfaceState.INTERFACE_STATE_HELPMENU)
				{
					Transition_SlideOut();
				}
				break;
			case 2:
				mScrollWidget.SetPageHorizontal(mScrollWidget.GetPageHorizontal() + 1, true);
				break;
			case 3:
				mScrollWidget.SetPageHorizontal(mScrollWidget.GetPageHorizontal() - 1, true);
				mEndGameContainer.mHighScoresWidgetPostGame.ReadLeaderBoard(HighScoreTable.HighScoreTableTime.TIME_RECENT);
				break;
			}
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			this?.LinkUpAssets();
		}

		public void SetMode(GameMode mode, GAMEDETAILMENU_STATE state)
		{
			if (mode == GameMode.MODE_ZEN)
			{
				int num = 1;
			}
			if (mIgnoreSetMode)
			{
				return;
			}
			mMode = mode;
			mGameMenuState = state;
			mHeadingLabel.SetMaximumWidth(0);
			mHeadingLabel.Resize(ConstantsWP.GAMEDETAILMENU_HEADING_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			mSwipeMsgLabel.SetVisible(state == GAMEDETAILMENU_STATE.STATE_POST_GAME);
			if (mMode != GameMode.MODE_ZEN)
			{
				switch (mGameMenuState)
				{
				case GAMEDETAILMENU_STATE.STATE_PRE_GAME:
				{
					for (int k = 0; k < mDefaultWidgetsPostGame.Count; k++)
					{
						Bej3Widget.DisableWidget(mDefaultWidgetsPostGame[k], true);
					}
					mEndGameContainer.Hide();
					for (int l = 0; l < mDefaultWidgetsPreGame.Count; l++)
					{
						Bej3Widget.DisableWidget(mDefaultWidgetsPreGame[l], false);
					}
					mHighScoresWidgetPreGame.SetMode(mode);
					mHeadingLabel.SetText(GlobalMembers.gApp.GetModeHeading(mMode));
					mPlayButton.SetLabel(GlobalMembers._ID("PLAY", 3290));
					mBackButton.SetLabel(GlobalMembers._ID("BACK", 3291));
					mModeDescriptionLabel.SetText(GlobalMembers.gApp.GetModeHint(mMode));
					break;
				}
				case GAMEDETAILMENU_STATE.STATE_POST_GAME:
				{
					for (int i = 0; i < mDefaultWidgetsPreGame.Count; i++)
					{
						Bej3Widget.DisableWidget(mDefaultWidgetsPreGame[i], true);
					}
					for (int j = 0; j < mDefaultWidgetsPostGame.Count; j++)
					{
						Bej3Widget.DisableWidget(mDefaultWidgetsPostGame[j], false);
					}
					mEndGameContainer.Show();
					mPlayButton.SetLabel(GlobalMembers._ID("PLAY AGAIN", 3292));
					mBackButton.SetLabel(GlobalMembers._ID("MAIN MENU", 3293));
					mEndGameContainer.SetMode(mMode, mGameMenuState);
					break;
				}
				}
			}
			Bej3Widget.CenterWidgetAt(ConstantsWP.GAMEDETAILMENU_PLAYBUTTON_X, ConstantsWP.GAMEDETAILMENU_PLAYBUTTON_Y + 50, mPlayButton, true, false);
			Bej3Widget.CenterWidgetAt(ConstantsWP.GAMEDETAILMENU_BACKBUTTON_X, ConstantsWP.GAMEDETAILMENU_PLAYBUTTON_Y + 50, mBackButton, true, false);
		}

		public override void HideCompleted()
		{
			mEndGameContainer.HideCompleted();
			base.HideCompleted();
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME)
			{
				BejeweledLivePlusApp.UnloadContent("MainMenu");
			}
		}

		public override void LinkUpAssets()
		{
			if (mPlayButton != null && mBackButton != null && mHighScoresWidgetPreGame != null)
			{
				mPlayButton.LinkUpAssets();
				mBackButton.LinkUpAssets();
				mHighScoresWidgetPreGame.LinkUpAssets();
				SetUpSlideButtons();
				base.LinkUpAssets();
			}
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
			int pageHorizontal = mScrollWidget.GetPageHorizontal();
			if (ScrollTargetReached_lastPage != pageHorizontal)
			{
				ScrollTargetReached_lastPage = pageHorizontal;
				LinkUpAssets();
			}
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public virtual void PageChanged(Bej3ScrollWidget scrollWidget, int pageH, int pageV)
		{
			if (pageH == 0)
			{
				mEndGameContainer.mHighScoresWidgetPostGame.ReadLeaderBoard(HighScoreTable.HighScoreTableTime.TIME_RECENT);
			}
			SetUpSlideButtons();
		}

		public override void Show()
		{
			if (!mVisible)
			{
				mY = ConstantsWP.MENU_Y_POS_HIDDEN;
				if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_PRE_GAME)
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
				else
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
			}
			mScrollWidget.SetPageHorizontal(0, false);
			LinkUpAssets();
			base.Show();
			mAlphaCurve.SetConstant(1.0);
			if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_POST_GAME)
			{
				SetTargetPosition(50);
				Transition_SlideThenFadeIn();
				GlobalMembers.gApp.mMenus[7].SetVisible(false);
				GlobalMembers.gApp.mMenus[7].Hide();
				mEndGameContainer.mHighScoresWidgetPostGame.CenterOnUser();
			}
			else
			{
				SetVisible(false);
				mHighScoresWidgetPreGame.CenterOnUser();
			}
			ResetFadedBack(true);
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
			if (mGameMenuState == GAMEDETAILMENU_STATE.STATE_POST_GAME)
			{
				mSlideLeftButton.EnableSlideGlow(true);
				mSlideRightButton.EnableSlideGlow(true);
			}
		}

		public override void Hide()
		{
			Bej3WidgetState mState2 = mState;
			base.Hide();
			mEndGameContainer.Hide();
		}

		public void GetStatsFromBoard(Board theBoard)
		{
			mEndGameContainer.GetStatsFromBoard(theBoard);
			switch (mMode)
			{
			case GameMode.MODE_CLASSIC:
			{
				int mPoints4 = theBoard.mPoints;
				mHeadingLabel.SetText(Common.CommaSeperate(mPoints4));
				break;
			}
			case GameMode.MODE_DIAMOND_MINE:
			{
				int mLevelPointsTotal = theBoard.mLevelPointsTotal;
				mHeadingLabel.SetText($"{Common.CommaSeperate(mLevelPointsTotal)}");
				break;
			}
			case GameMode.MODE_BUTTERFLY:
			{
				int mPoints3 = theBoard.mPoints;
				mHeadingLabel.SetText(Common.CommaSeperate(mPoints3));
				break;
			}
			case GameMode.MODE_POKER:
			{
				int mPoints2 = theBoard.mPoints;
				mHeadingLabel.SetText(Common.CommaSeperate(mPoints2));
				break;
			}
			case GameMode.MODE_LIGHTNING:
			{
				int mPoints = theBoard.mPoints;
				mHeadingLabel.SetText(Common.CommaSeperate(mPoints));
				break;
			}
			}
			GlobalMembers.gApp.mProfile.UpdateRank(theBoard);
		}

		public GAMEDETAILMENU_STATE GetGameMenuState()
		{
			return mGameMenuState;
		}
	}
}
