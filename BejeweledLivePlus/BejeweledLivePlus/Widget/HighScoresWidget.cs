using BejeweledLivePlus.UI;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class HighScoresWidget : Bej3WidgetBase
	{
		private bool mDrawHeading;

		private int mGameMode;

		private Label mTopScoresHeadingLabel;

		public HighScoresContainer mContainer;

		public bool mScrollLocked;

		public Bej3ScrollWidget mScrollWidget;

		public HighScoresWidget(Rect size, bool drawHeading, int scrollwidgetCorrectionOfffset)
		{
			mScrollLocked = false;
			mDrawHeading = drawHeading;
			mTopScoresHeadingLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mTopScoresHeadingLabel.SetText(GlobalMembers._ID("TOP SCORES", 3346));
			mTopScoresHeadingLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_2_STROKE);
			mTopScoresHeadingLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_2_FILL);
			AddWidget(mTopScoresHeadingLabel);
			mContainer = new HighScoresContainer(mWidth);
			mScrollWidget = new Bej3ScrollWidget(mContainer);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.mScrollDownOffset = scrollwidgetCorrectionOfffset;
			AllowScrolling(true);
			mScrollWidget.AddWidget(mContainer);
			AddWidget(mScrollWidget);
			base.Resize(size);
		}

		public HighScoresWidget(Rect size, bool drawHeading)
		{
			int mScrollDownOffset = 0;
			mScrollLocked = false;
			mDrawHeading = drawHeading;
			mTopScoresHeadingLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mTopScoresHeadingLabel.SetText(GlobalMembers._ID("TOP SCORES", 3346));
			mTopScoresHeadingLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_2_STROKE);
			mTopScoresHeadingLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_2_FILL);
			AddWidget(mTopScoresHeadingLabel);
			mContainer = new HighScoresContainer(mWidth);
			mScrollWidget = new Bej3ScrollWidget(mContainer);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.mScrollDownOffset = mScrollDownOffset;
			AllowScrolling(true);
			mScrollWidget.AddWidget(mContainer);
			AddWidget(mScrollWidget);
			base.Resize(size);
		}

		public override void Update()
		{
			mScrollLocked = mScrollWidget.mLocked;
			base.Update();
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawInlayBox(g, new Rect(0, 0, mWidth, mHeight), mTopScoresHeadingLabel.GetTextWidth(), true);
			DeferOverlay(0);
		}

		public override void DrawOverlay(Graphics g)
		{
		}

		public void SetHeading(string theHeading)
		{
			mTopScoresHeadingLabel.SetText(theHeading);
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			mTopScoresHeadingLabel.Resize(mWidth / 2, ConstantsWP.HIGHSCORESWIDGET_TOPSCORES_HEADING_Y, 0, 0);
			mContainer.ChangeWidth(mWidth);
			mScrollWidget.Resize(0, ConstantsWP.HIGHSCORESWIDGET_BOX_Y, mWidth, mHeight - GlobalMembersResourcesWP.IMAGE_DIALOG_LISTBOX_FOOTER.mHeight - ConstantsWP.LISTBOX_FOOTER_OFFSET - ConstantsWP.HIGHSCORESWIDGET_BOX_Y);
		}

		public override void LinkUpAssets()
		{
			if (mContainer != null)
			{
				mContainer.LinkUpAssets();
			}
		}

		public void SetMode(GameMode gameMode)
		{
			mGameMode = (int)gameMode;
			string modeHeading = GlobalMembers.gApp.GetModeHeading((GameMode)mGameMode);
			mContainer.SetMode(modeHeading);
			mScrollWidget.ScrollToMin(false);
		}

		public int GetMode()
		{
			return mGameMode;
		}

		public void AllowScrolling(bool allow)
		{
			if (allow)
			{
				mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_VERTICAL);
			}
			else
			{
				mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_DISABLED);
			}
		}

		public void SetMaxNameWidth(int maxWidth)
		{
			mContainer.mMaxNameWidth = maxWidth;
		}

		public override void TouchEnded(SexyAppBase.Touch touch)
		{
			base.TouchEnded(touch);
			Point absPos = mScrollWidget.GetAbsPos();
			touch.location.mX -= absPos.mX;
			touch.location.mY -= absPos.mY;
			touch.previousLocation.mX -= absPos.mX;
			touch.previousLocation.mY -= absPos.mY;
			mScrollWidget.TouchEnded(touch);
		}

		public void CenterOnUser()
		{
			int a = (mContainer.mUserPosition + 1) * ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT - mHeight / 2 + ConstantsWP.HIGHSCORESWIDGET_SCROLL_TO_OFFSET;
			a = GlobalMembers.MAX(0, GlobalMembers.MIN(a, mContainer.mHeight - mScrollWidget.mHeight));
			mScrollWidget.ScrollToPoint(new Point(0, a), false);
		}

		public void UnNewScore()
		{
		}

		public void SelectModeView(HighScoresMenuContainer.HSMODE m)
		{
			mContainer.SelectModeView(m);
		}

		public void ReadLeaderBoard(HighScoreTable.HighScoreTableTime t)
		{
			mContainer.ReadLeaderBoard(t);
		}
	}
}
