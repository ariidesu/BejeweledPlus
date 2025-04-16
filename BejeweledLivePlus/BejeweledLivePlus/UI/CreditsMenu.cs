using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class CreditsMenu : Bej3Widget, Bej3ButtonListener, ButtonListener
	{
		private enum CreditsMenu_IDS
		{
			BTN_BACK_ID
		}

		private Label mHeadingLabel;

		private CreditsMenuScrollWidget mScrollWidget;

		private CreditsMenuContainer mContainer;

		private Bej3Button mBackButton;

		public CreditsMenu()
			: base(Menu_Type.MENU_CREDITSMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.CREDITSMENU_HEADING_LABEL_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetText(GlobalMembers._ID("CREDITS", 3243));
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			mContainer = new CreditsMenuContainer();
			mScrollWidget = new CreditsMenuScrollWidget(mContainer);
			mScrollWidget.Resize(ConstantsWP.CREDITSMENU_PADDING_X, ConstantsWP.CREDITSMENU_PADDING_TOP, mWidth - ConstantsWP.CREDITSMENU_PADDING_X * 2, mHeight - ConstantsWP.CREDITSMENU_PADDING_TOP - ConstantsWP.CREDITSMENU_PADDING_BOTTOM);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_VERTICAL);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.AddWidget(mContainer);
			AddWidget(mScrollWidget);
			mBackButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE, true);
			mBackButton.Resize(0, 0, ConstantsWP.ABOUTMENU_BACK_BUTTON_WIDTH, 0);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3244));
			Bej3Widget.CenterWidgetAt(mWidth / 2, ConstantsWP.CREDITSMENU_BUTTON_Y, mBackButton, true, false);
			AddWidget(mBackButton);
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				ButtonDepress(0);
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawSwipeInlay(g, mScrollWidget.mY, mScrollWidget.mHeight, mWidth);
		}

		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
			if (mContainer != null)
			{
				mContainer.SetVisible(isVisible);
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mContainer.LinkUpAssets();
		}

		public override void Show()
		{
			mContainer.Show();
			base.Show();
			mScrollWidget.Restart();
			mScrollWidget.mAnimate = true;
			mTargetPos = 106;
			SetVisible(false);
		}

		public override void Hide()
		{
			base.Hide();
			mContainer.Hide();
		}

		public override void ButtonDepress(int theId)
		{
			switch (theId)
			{
			case 10001:
				GlobalMembers.gApp.DoMainMenu();
				Transition_SlideOut();
				break;
			case 0:
				GlobalMembers.gApp.DoOptionsMenu();
				Transition_SlideOut();
				break;
			}
		}
	}
}
