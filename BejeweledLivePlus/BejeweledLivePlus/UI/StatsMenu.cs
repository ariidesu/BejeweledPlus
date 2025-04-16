using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	internal class StatsMenu : Bej3Widget, Bej3ButtonListener, ButtonListener
	{
		private enum STATSMENU_BUTTON_IDS
		{
			BTN_PROFILE_ID
		}

		private Label mHeadingLabel;

		private StatsMenuContainer mContainer;

		private ScrollWidget mScrollWidget;

		private Bej3Button mProfileButton;

		public StatsMenu()
			: base(Menu_Type.MENU_STATSMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mFinalY = 0;
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.STATS_MENU_HEADING_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetText(GlobalMembers._ID("STATS", 3439));
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			mProfileButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE, true);
			mProfileButton.SetLabel(GlobalMembers._ID("BACK", 3440));
			Bej3Widget.CenterWidgetAt(320, 966, mProfileButton, true, false);
			AddWidget(mProfileButton);
			mContainer = new StatsMenuContainer();
			mScrollWidget = new ScrollWidget(mContainer);
			mScrollWidget.Resize(ConstantsWP.STATS_MENU_CONTAINER_PADDING_X, ConstantsWP.STATS_MENU_CONTAINER_PADDING_TOP, mWidth - ConstantsWP.STATS_MENU_CONTAINER_PADDING_X * 2, mHeight - ConstantsWP.STATS_MENU_CONTAINER_PADDING_TOP - ConstantsWP.STATS_MENU_CONTAINER_PADDING_BOTTOM);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_VERTICAL);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.AddWidget(mContainer);
			AddWidget(mScrollWidget);
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				ButtonDepress(10002);
			}
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.MENU_DIVIDER_Y);
			DeferOverlay(0);
		}

		public override void DrawOverlay(Graphics g)
		{
		}

		public override void ButtonMouseEnter(int theId)
		{
		}

		public override void ButtonDepress(int theId)
		{
			if (theId == 10001)
			{
				GlobalMembers.gApp.DoMainMenu();
				((MainMenuOptions)GlobalMembers.gApp.mMenus[5]).Expand();
				Transition_SlideOut();
			}
			else
			{
				GlobalMembers.gApp.DoMainMenu();
				((MainMenuOptions)GlobalMembers.gApp.mMenus[5]).Expand();
				Transition_SlideOut();
			}
		}

		public override void LinkUpAssets()
		{
			mContainer.LinkUpAssets();
			base.LinkUpAssets();
		}

		public override void Show()
		{
			mContainer.Show();
			base.Show();
			SetVisible(false);
		}

		public override void ShowCompleted()
		{
			mTopButton.SetType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			mTopButton.SetDisabled(false);
		}

		public override void Hide()
		{
			base.Hide();
			mContainer.Hide();
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
			if (mInterfaceState != InterfaceState.INTERFACE_STATE_PROFILEMENU)
			{
				((ProfileMenuBase)GlobalMembers.gApp.mMenus[12]).UnloadPlayerImages();
			}
		}
	}
}
