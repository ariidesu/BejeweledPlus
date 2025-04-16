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
			HIGHSCORES_DIAMOND_MINE,
			HIGHSCORES_BUTTERFLIES,
			HIGHSCORES_MAX_MODES
		}

		public HSMODE mCurrentDisplayMode;

		public HighScoreTable.HighScoreTableTime mCurrentDisplayView;

		private HighScoresWidget[] mHighscoreWidgets = new HighScoresWidget[4];

		public bool mScrollLocked;

		public Bej3ScrollWidget mLockedScrollWidget;

		public HighScoresMenuContainer()
			: base(Menu_Type.MENU_HIGHSCORESMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mLockedScrollWidget = null;
			mDoesSlideInFromBottom = (mCanAllowSlide = false);
			int hIGHSCORES_MENU_MODEWIDTH = ConstantsWP.HIGHSCORES_MENU_MODEWIDTH;
			int num = 4;
			int hIGHSCORES_MENU_CONTAINER_HEIGHT = ConstantsWP.HIGHSCORES_MENU_CONTAINER_HEIGHT;
			Rect size = new Rect((ConstantsWP.HIGHSCORES_MENU_WIDTH - ConstantsWP.HIGHSCORES_MENU_CONTAINER_PADDING_X * 2) / 2 - ConstantsWP.HIGHSCORES_MENU_MODEWIDTH / 2, ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_TOP, hIGHSCORES_MENU_MODEWIDTH, hIGHSCORES_MENU_CONTAINER_HEIGHT - ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_TOP - ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_TOP);
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
			mHighscoreWidgets[2].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_DIAMOND_MINE));
			mHighscoreWidgets[2].SetMode(GameMode.MODE_DIAMOND_MINE);
			size.mX += hIGHSCORES_MENU_MODEWIDTH + ConstantsWP.HIGHSCORES_MENU_MODE_PADDING_X;
			mHighscoreWidgets[3] = new HighScoresWidget(size, true, ConstantsWP.HIGHSCORES_MENU_SCROLLWIDGET_CORRECTION);
			mHighscoreWidgets[3].SetHeading(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_BUTTERFLY));
			mHighscoreWidgets[3].SetMode(GameMode.MODE_BUTTERFLY);
			for (int i = 0; i < 4; i++)
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
			for (int i = 0; i < 4; i++)
			{
				mHighscoreWidgets[i].AllowScrolling(allow);
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			for (int i = 0; i < 4; i++)
			{
				mHighscoreWidgets[i].LinkUpAssets();
			}
		}

		public void SelectTimeView(HighScoreTable.HighScoreTableTime t)
		{
			mCurrentDisplayView = t;
			mHighscoreWidgets[(int)mCurrentDisplayMode].ReadLeaderBoard(t);
		}

		public void SelectModeView(HSMODE m)
		{
			if (m >= HSMODE.HIGHSCORES_CLASSIC && m < HSMODE.HIGHSCORES_MAX_MODES)
			{
				mCurrentDisplayMode = m;
				mHighscoreWidgets[(int)mCurrentDisplayMode].SelectModeView(m);
			}
		}
	}
}
