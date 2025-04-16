using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class ProfileMenu : ProfileMenuBase
	{
		private Bej3Button mEditButton;

		private Bej3Button mBackButton;

		private ProfileMenuContainer mContainer;

		private Bej3ScrollWidget mScrollWidget;

		private Label mPlayerNameLabel;

		private RankBarWidget mRankBarWidget;

		private Label mPlayerRankLabel;

		public ProfileMenu()
			: base(Menu_Type.MENU_PROFILEMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, ConstantsWP.PROFILEMENU_WIDTH, GlobalMembers.gApp.mHeight);
			mFinalY = 0;
			mContainer = new ProfileMenuContainer(this);
			mScrollWidget = new Bej3ScrollWidget(mContainer, false);
			mScrollWidget.Resize(ConstantsWP.PROFILEMENU_PADDING_X, ConstantsWP.PROFILEMENU_PADDING_TOP, mWidth - ConstantsWP.PROFILEMENU_PADDING_X * 2, mHeight - ConstantsWP.PROFILEMENU_PADDING_BOTTOM - ConstantsWP.PROFILEMENU_PADDING_TOP);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_DISABLED);
			mScrollWidget.AddWidget(mContainer);
			AddWidget(mScrollWidget);
			mPlayerImage = new ImageWidget(712, true);
			AddWidget(mPlayerImage);
			mPlayerNameLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mPlayerNameLabel.Resize(ConstantsWP.PROFILEMENU_NAME_LABEL_X, ConstantsWP.PROFILEMENU_NAME_LABEL_Y, 0, 0);
			AddWidget(mPlayerNameLabel);
			mPlayerRankLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mPlayerRankLabel.Resize(ConstantsWP.PROFILEMENU_RANK_LABEL_X, ConstantsWP.PROFILEMENU_RANK_LABEL_Y, 0, 0);
			AddWidget(mPlayerRankLabel);
			mEditButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mEditButton.SetLabel(GlobalMembers._ID("EDIT PROFILE", 3427));
			mEditButton.Resize(0, 0, ConstantsWP.PROFILEMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.PROFILEMENU_EDIT_X, ConstantsWP.PROFILEMENU_BOTTOM_BUTTON_Y, mEditButton, true, false);
			AddWidget(mEditButton);
			mBackButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3428));
			mBackButton.Resize(0, 0, ConstantsWP.PROFILEMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.PROFILEMENU_BACK_X, ConstantsWP.PROFILEMENU_BOTTOM_BUTTON_Y, mBackButton, true, false);
			AddWidget(mBackButton);
			mRankBarWidget = new RankBarWidget(ConstantsWP.PROFILEMENU_RANKBAR_WIDTH);
			mRankBarWidget.mDrawRankName = true;
			mRankBarWidget.mDrawCrown = false;
			mRankBarWidget.Resize(ConstantsWP.PROFILEMENU_RANKBAR_X, ConstantsWP.PROFILEMENU_RANKBAR_Y, ConstantsWP.PROFILEMENU_RANKBAR_WIDTH, 0);
			AddWidget(mRankBarWidget);
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

		public override void Update()
		{
			base.Update();
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.PROFILEMENU_DIVIDER_NAME);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.PROFILEMENU_DIVIDER_RANK);
		}

		public override void LinkUpAssets()
		{
			mContainer.LinkUpAssets();
			base.LinkUpAssets();
			Graphics graphics = new Graphics();
			graphics.SetFont(mPlayerNameLabel.GetFont());
			mPlayerNameLabel.SetText(Utils.GetEllipsisString(graphics, GlobalMembers.gApp.mProfile.mProfileName, ConstantsWP.PROFILEMENU_NAME_LABEL_WIDTH));
			if (GlobalMembers.gApp.mProfile.UsesPresetProfilePicture())
			{
				mPlayerImage.SetImage(712 + GlobalMembers.gApp.mProfile.GetProfilePictureId());
			}
			mPlayerImage.Resize(ConstantsWP.PROFILEMENU_PLAYER_IMAGE_X, ConstantsWP.PROFILEMENU_PLAYER_IMAGE_Y, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE);
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
				GlobalMembers.gApp.DoMainMenu(true);
				Transition_SlideOut();
				break;
			case 0:
				GlobalMembers.gApp.DoEditProfileMenu();
				Transition_SlideOut();
				break;
			case 2:
				GlobalMembers.gApp.DoStatsMenu();
				Transition_SlideOut();
				break;
			case 3:
				GlobalMembers.gApp.DoHighScoresMenu();
				Transition_SlideOut();
				break;
			case 4:
				if (GlobalMembers.gApp.mProfile.mDeferredBadgeVector.Count > 0)
				{
					GlobalMembers.gApp.DoBadgeMenu(2, GlobalMembers.gApp.mProfile.mDeferredBadgeVector);
					Transition_SlideOut();
				}
				else
				{
					GlobalMembers.gApp.DoBadgeMenu(0, null);
					Transition_SlideOut();
				}
				break;
			}
		}

		public override void InterfaceStateChanged(InterfaceState newState)
		{
			base.InterfaceStateChanged(newState);
			mContainer.InterfaceStateChanged(newState);
		}

		public override void Show()
		{
			Bej3WidgetState mState2 = mState;
			mContainer.Show();
			base.Show();
			mPlayerRankLabel.SetText(mRankBarWidget.GetRankName(GlobalMembers.gApp.mProfile.mOfflineRank, false));
			SetVisible(false);
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
		}

		public override void Hide()
		{
			mContainer.Hide();
			base.Hide();
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			WidgetDrawOverlay(g, thePriority);
		}

		public new void KeyChar(char theChar)
		{
		}
	}
}
