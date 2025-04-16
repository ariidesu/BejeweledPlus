using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class ProfileMenuContainer : Bej3Widget, Bej3ScrollWidgetListener, ScrollWidgetListener
	{
		private Bej3Button mGoToStatsButton;

		private Bej3Button mGoToHighscoresButton;

		private Bej3Button mGoToBadgesButton;

		public ProfileMenuContainer(ProfileMenu parent)
			: base(Menu_Type.MENU_PROFILEMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mDoesSlideInFromBottom = (mCanAllowSlide = false);
			Resize(0, 0, mWidth - ConstantsWP.PROFILEMENU_PADDING_X * 2, ConstantsWP.PROFILEMENU_CONTAINER_HEIGHT);
			mGoToStatsButton = new Bej3Button(2, parent, Bej3ButtonType.BUTTON_TYPE_LONG);
			mGoToHighscoresButton = new Bej3Button(3, parent, Bej3ButtonType.BUTTON_TYPE_LONG);
			mGoToBadgesButton = new Bej3Button(4, parent, Bej3ButtonType.BUTTON_TYPE_LONG);
			mGoToStatsButton.SetLabel(GlobalMembers._ID("STATS", 3429));
			mGoToHighscoresButton.SetLabel(GlobalMembers._ID("TOP SCORES", 3430));
			mGoToBadgesButton.SetLabel(GlobalMembers._ID("BADGES", 3431));
			int theHeight = mGoToStatsButton.mComponentImage.mHeight;
			int buttonWidth = mGoToStatsButton.GetButtonWidth();
			buttonWidth = ConstantsWP.PROFILEMENU_GOTO_BUTTON_WIDTH;
			mGoToStatsButton.Resize((mWidth - buttonWidth) / 2, ConstantsWP.PROFILEMENU_STATS_Y + ConstantsWP.PROFILEMENU_STATS_BTN_OFFSET_Y, buttonWidth, theHeight);
			int buttonWidth2 = mGoToHighscoresButton.GetButtonWidth();
			buttonWidth2 = ConstantsWP.PROFILEMENU_GOTO_BUTTON_WIDTH;
			mGoToHighscoresButton.Resize((mWidth - buttonWidth2) / 2, ConstantsWP.PROFILEMENU_HIGHSCORES_Y + ConstantsWP.PROFILEMENU_SCORES_BTN_OFFSET_Y, buttonWidth2, theHeight);
			int buttonWidth3 = mGoToBadgesButton.GetButtonWidth();
			buttonWidth3 = ConstantsWP.PROFILEMENU_GOTO_BUTTON_WIDTH;
			mGoToBadgesButton.Resize((mWidth - buttonWidth3) / 2, ConstantsWP.PROFILEMENU_BADGES_Y + ConstantsWP.PROFILEMENU_BADGES_BTN_OFFSET_Y, buttonWidth3, theHeight);
			AddWidget(mGoToStatsButton);
			AddWidget(mGoToHighscoresButton);
			AddWidget(mGoToBadgesButton);
		}

		public override void Draw(Graphics g)
		{
		}

		public override void Update()
		{
			base.Update();
		}

		public override void ButtonDepress(int theId)
		{
		}

		public override void LinkUpAssets()
		{
		}

		public override void Hide()
		{
			base.Hide();
			mAlphaCurve.SetConstant(1.0);
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
		}

		public override void Show()
		{
			base.Show();
			mY = mTargetPos;
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public virtual void PageChanged(Bej3ScrollWidget scrollWidget, int pageH, int pageV)
		{
		}
	}
}
