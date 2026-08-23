using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class HighScoresMenuContainer : Bej3Widget, Bej3ButtonListener, ButtonListener
	{
		public enum HSMODE
		{
			HIGHSCORES_CLASSIC,
			HIGHSCORES_LIGHTNING,
			HIGHSCORES_POKER,
			HIGHSCORES_BUTTERFLIES,
			HIGHSCORES_ICE_STORM,
			HIGHSCORES_INFERNO_STORM,
			HIGHSCORES_DIAMOND_MINE,
			HIGHSCORES_BLITZ,
			HIGHSCORES_MATCH_BOMB,
			HIGHSCORES_TIME_BOMB,
			HIGHSCORES_MAX_MODES
		}

		public HSMODE mCurrentDisplayMode;

		public HighScoreTable.HighScoreTableTime mCurrentDisplayView;

		private HighScoresWidget[] mHighscoreWidgets = new HighScoresWidget[(int)HSMODE.HIGHSCORES_MAX_MODES];

		public bool mScrollLocked;

		public Bej3ScrollWidget mLockedScrollWidget;

		public HighScoresMenuContainer()
			: base(Menu_Type.MENU_HIGHSCORESMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mLockedScrollWidget = null;
			mDoesSlideInFromBottom = (mCanAllowSlide = false);
			int hIGHSCORES_MENU_MODEWIDTH = ConstantsWP.HIGHSCORES_MENU_MODEWIDTH;
			int num = (int)HSMODE.HIGHSCORES_MAX_MODES;
			int hIGHSCORES_MENU_CONTAINER_HEIGHT = ConstantsWP.HIGHSCORES_MENU_CONTAINER_HEIGHT;
			int hIGHSCORES_MENU_FRAME_WIDTH = hIGHSCORES_MENU_MODEWIDTH - ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			Rect size = new Rect((ConstantsWP.HIGHSCORES_MENU_WIDTH - ConstantsWP.HIGHSCORES_MENU_CONTAINER_PADDING_X * 2) / 2 - hIGHSCORES_MENU_FRAME_WIDTH / 2, ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_TOP, hIGHSCORES_MENU_FRAME_WIDTH, hIGHSCORES_MENU_CONTAINER_HEIGHT - ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_TOP - ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_TOP);
			Resize(0, 0, ConstantsWP.HIGHSCORES_MENU_CONTAINER_WIDTH * num, hIGHSCORES_MENU_CONTAINER_HEIGHT);
			mHighscoreWidgets[0] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[0].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_CLASSIC));
			mHighscoreWidgets[0].SetMode(GameMode.MODE_CLASSIC);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[1] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[1].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_LIGHTNING));
			mHighscoreWidgets[1].SetMode(GameMode.MODE_LIGHTNING);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[2] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[2].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_POKER));
			mHighscoreWidgets[2].SetMode(GameMode.MODE_POKER);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[3] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[3].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_BUTTERFLY));
			mHighscoreWidgets[3].SetMode(GameMode.MODE_BUTTERFLY);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[4] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[4].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_ICESTORM));
			mHighscoreWidgets[4].SetMode(GameMode.MODE_ICESTORM);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[5] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[5].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_INFERNOSTORM));
			mHighscoreWidgets[5].SetMode(GameMode.MODE_INFERNOSTORM);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[6] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[6].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_DIAMOND_MINE));
			mHighscoreWidgets[6].SetMode(GameMode.MODE_DIAMOND_MINE);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[7] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[7].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_BLITZ));
			mHighscoreWidgets[7].SetMode(GameMode.MODE_BLITZ);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[8] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[8].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_TIMEBOMB));
			mHighscoreWidgets[8].SetMode(GameMode.MODE_TIMEBOMB);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[9] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[9].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_REALTIMEBOMB));
			mHighscoreWidgets[9].SetMode(GameMode.MODE_REALTIMEBOMB);
			for (int i = 0; i < mHighscoreWidgets.Length; i++)
			{
				AddWidget(mHighscoreWidgets[i]);
				mHighscoreWidgets[i].mContainer.mMenu = this;
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public override void Show()
		{
			base.Show();
			HighScoresWidget[] array = mHighscoreWidgets;
			foreach (HighScoresWidget highScoresWidget in array)
			{
				highScoresWidget.mContainer.mScoreTable.mLRState = HighScoreTable.LRState.LR_Idle;
			}
			mY = 0;
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Draw(Graphics g)
		{
		}

		public void AllowScrolling(bool allow)
		{
			for (int i = 0; i < mHighscoreWidgets.Length; i++)
			{
				mHighscoreWidgets[i].AllowScrolling(allow);
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			for (int i = 0; i < mHighscoreWidgets.Length; i++)
			{
				mHighscoreWidgets[i].LinkUpAssets();
			}
		}

		public void SelectTimeView(HighScoreTable.HighScoreTableTime t)
		{
			if (!CurrentModeSupportsTimeViews())
			{
				t = HighScoreTable.HighScoreTableTime.TIME_ALLTIME;
			}
			mCurrentDisplayView = t;
			mHighscoreWidgets[(int)mCurrentDisplayMode].ReadLeaderBoard(t);
		}

		public void SelectModeView(HSMODE m)
		{
			if (m >= HSMODE.HIGHSCORES_CLASSIC && m < HSMODE.HIGHSCORES_MAX_MODES)
			{
				mCurrentDisplayMode = m;
				if (!CurrentModeSupportsTimeViews())
				{
					mCurrentDisplayView = HighScoreTable.HighScoreTableTime.TIME_ALLTIME;
				}
				mHighscoreWidgets[(int)mCurrentDisplayMode].SelectModeView(m);
			}
		}

		public bool CurrentModeSupportsTimeViews()
		{
			HighScoreTable table = mHighscoreWidgets[(int)mCurrentDisplayMode].mContainer.mScoreTable;
			return table != null && table.SupportsTimeViews;
		}

		public bool CurrentModeIsLoading()
		{
			HighScoreTable table = mHighscoreWidgets[(int)mCurrentDisplayMode].mContainer.mScoreTable;
			return table != null && table.mLRState == HighScoreTable.LRState.LR_Loading;
		}

		public void ForceAllTimeView()
		{
			mCurrentDisplayView = HighScoreTable.HighScoreTableTime.TIME_ALLTIME;
		}

		public static GameMode GetGameMode(HSMODE mode)
		{
			switch (mode)
			{
			case HSMODE.HIGHSCORES_CLASSIC:
				return GameMode.MODE_CLASSIC;
			case HSMODE.HIGHSCORES_LIGHTNING:
				return GameMode.MODE_LIGHTNING;
			case HSMODE.HIGHSCORES_POKER:
				return GameMode.MODE_POKER;
			case HSMODE.HIGHSCORES_BUTTERFLIES:
				return GameMode.MODE_BUTTERFLY;
			case HSMODE.HIGHSCORES_ICE_STORM:
				return GameMode.MODE_ICESTORM;
			case HSMODE.HIGHSCORES_INFERNO_STORM:
				return GameMode.MODE_INFERNOSTORM;
			case HSMODE.HIGHSCORES_DIAMOND_MINE:
				return GameMode.MODE_DIAMOND_MINE;
			case HSMODE.HIGHSCORES_BLITZ:
				return GameMode.MODE_BLITZ;
			case HSMODE.HIGHSCORES_MATCH_BOMB:
				return GameMode.MODE_TIMEBOMB;
			case HSMODE.HIGHSCORES_TIME_BOMB:
				return GameMode.MODE_REALTIMEBOMB;
			default:
				return GameMode.MODE_CLASSIC;
			}
		}
	}
}
