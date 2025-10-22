using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	internal class MainMenuOptions : ProfileMenuBase
	{
		private enum MainMenuOptions_BUTTON_IDS
		{
			BTN_PROFILE_ID,
			BTN_OPTIONS_ID,
			BTN_MOREGAMES_ID,
			BTN_HELP_ID,
			BTN_BACK_ID
		}

		private int mPlayerHeight;

		private Bej3Button mProfileButton;

		private Bej3Button mOptionsButton;

		private Bej3Button mHelpButton;

		private Bej3Button mBackButton;

		private Label mPlayerNameLabel;

		private RankBarWidget mRankBarWidget;

		public bool mExpandOnShow;

		public bool mCanSlideIn;

		public bool mFirstShow;

		public bool didFadeIn;

		public MainMenuOptions()
			: base(Menu_Type.MENU_MAINMENUOPTIONSMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_MENU)
		{
			mExpandOnShow = false;
			mCanSlideIn = false;
			mFirstShow = false;
			didFadeIn = false;
			int num = 15;
			mPlayerHeight = 460;
			int num2 = 450;
			int num3 = 220;
			int num4 = -130;
			float num5 = 1.3f;
			int num6 = 84;
			mPlayerNameLabel = new Label(GlobalMembersResources.FONT_DIALOG);
			mPlayerNameLabel.Resize(ConstantsWP.PROFILEMENU_NAME_LABEL_X, ConstantsWP.PROFILEMENU_NAME_LABEL_Y + num2 - num6 - num, 0, 0);
			AddWidget(mPlayerNameLabel);
			mRankBarWidget = new RankBarWidget(ConstantsWP.PROFILEMENU_RANKBAR_WIDTH);
			mRankBarWidget.mDrawRankName = false;
			mRankBarWidget.mDrawCrown = false;
			mRankBarWidget.Resize(ConstantsWP.PROFILEMENU_RANKBAR_X + num4, ConstantsWP.PROFILEMENU_RANKBAR_Y + num3, (int)((float)ConstantsWP.PROFILEMENU_RANKBAR_WIDTH * num5), 0);
			AddWidget(mRankBarWidget);
			Resize(0, GlobalMembers.gApp.mHeight, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mFinalY = ConstantsWP.MENU_Y_POS_HIDDEN;
			mPlayerImage = new ImageWidget(712, true);
			mPlayerImage.Resize(ConstantsWP.MAINMENU_OPTIONSMENU_PROFILE_X, ConstantsWP.MAINMENU_OPTIONSMENU_PROFILE_Y - num + 10, (int)((float)ConstantsWP.LARGE_PROFILE_PICTURE_SIZE * 2.3f), (int)((double)ConstantsWP.LARGE_PROFILE_PICTURE_SIZE * 2.3));
			mPlayerImage.mScale = 2.3f;
			AddWidget(mPlayerImage);
			mProfileButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mProfileButton.SetLabel(GlobalMembers._ID("STATS", 3296));
			mProfileButton.Resize(0, 0, ConstantsWP.MAINMENU_OPTIONSMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAINMENU_OPTIONSMENU_PROFILE_X, ConstantsWP.MAINMENU_OPTIONSMENU_PROFILE_Y + mPlayerHeight + num6, mProfileButton);
			AddWidget(mProfileButton);
			mOptionsButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mOptionsButton.mFont = GlobalMembersResources.FONT_SUBHEADER;
			mOptionsButton.SetLabel(GlobalMembers._ID("OPTIONS", 3403));
			mOptionsButton.Resize(0, 0, ConstantsWP.MAINMENU_OPTIONSMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.MAINMENU_OPTIONSMENU_OPTIONS_X, ConstantsWP.MAINMENU_OPTIONSMENU_OPTIONS_Y + mPlayerHeight + num6, mOptionsButton);
			AddWidget(mOptionsButton);
			mBackButton = new Bej3Button(4, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3405));
			mBackButton.Resize(0, 0, ConstantsWP.MAINMENU_OPTIONSMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_BACK_X, ConstantsWP.MAINMENU_OPTIONSMENU_HELP_Y + mPlayerHeight + num6, mBackButton);
			AddWidget(mBackButton);
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		public virtual void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button != 0)
			{
				return;
			}
			if (mY == ConstantsWP.MENU_Y_POS_HIDDEN)
			{
				if ((GlobalMembers.gApp.mInterfaceState != InterfaceState.INTERFACE_STATE_MAINMENU || !GlobalMembers.gApp.mMenus[6].mVisible) && GlobalMembers.gApp.mMainMenu.mContainer.CurrentPage == 0)
				{
					GlobalMembers.gApp.mMainMenu.QuitGameRequest();
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
					Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.GetDialog(33);
					bej3Dialog.mDialogListener = GlobalMembers.gApp;
					if (bej3Dialog != null)
					{
						Transition_SlideOut();
					}
				}
			}
			else
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

		public override void Draw(Graphics g)
		{
			g.SetColorizeImages(true);
			DrawFadedBack(g);
			g.SetColorizeImages(false);
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.PROFILEMENU_DIVIDER_NAME - 80);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.PROFILEMENU_DIVIDER_RANK + 200);
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			base.DrawAll(theFlags, g);
		}

		public override void UpdateAll(ModalFlags theFlags)
		{
			base.UpdateAll(theFlags);
		}

		public override void Update()
		{
			SetTopButtonType((mTargetPos < 800) ? Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS : Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			base.Update();
			Graphics graphics = new Graphics();
			graphics.SetFont(mPlayerNameLabel.GetFont());
			mPlayerNameLabel.SetText(Utils.GetEllipsisString(graphics, GlobalMembers.gApp.mProfile.mProfileName, ConstantsWP.PROFILEMENU_NAME_LABEL_WIDTH));
			if (!didFadeIn && mState == Bej3WidgetState.STATE_FADING_IN && mCanSlideIn)
			{
				didFadeIn = true;
				Bej3Widget.mCurrentSlidingMenu = this;
				AllowSlideIn(true, null);
				mCanSlideIn = false;
				if (!mFirstShow)
				{
					mY = GlobalMembers.gApp.mHeight;
				}
			}
			if (mFirstShow && mState == Bej3WidgetState.STATE_IN && mY == mTargetPos)
			{
				IntroDialog theDialog = new IntroDialog();
				GlobalMembers.gApp.AddDialog(theDialog);
				Transition_SlideOut();
				mFirstShow = false;
			}
			if (mState == Bej3WidgetState.STATE_FADING_IN)
			{
				GlobalMembers.gApp.DisableAllExceptThis(this, true);
			}
		}

		public override void ButtonDepress(int theId)
		{
			GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
			switch (theId)
			{
			case 10001:
				if (mTargetPos == ConstantsWP.MENU_Y_POS_HIDDEN)
				{
					Expand();
				}
				else
				{
					Collapse();
				}
				break;
			case 4:
				Collapse();
				break;
			case 1:
				GlobalMembers.gApp.DoHelpAndOptionsMenu();
				Transition_SlideOut();
				break;
			case 0:
				GlobalMembers.gApp.DoStatsMenu();
				Transition_SlideOut();
				break;
			case 3:
				GlobalMembers.gApp.DoHelpDialog(0, 2);
				Transition_SlideOut();
				break;
			case 2:
				GlobalMembers.gApp.DoEditProfileMenu();
				Transition_SlideOut();
				break;
			}
		}

		public override void Show()
		{
			if (GlobalMembers.gApp.mProfile.UsesPresetProfilePicture())
			{
				mPlayerImage.SetImage(712 + GlobalMembers.gApp.mProfile.GetProfilePictureId());
			}

			int num = mY;
			base.Show();
			mY = num;
			ResetFadedBack(false);
			if (mExpandOnShow)
			{
				Expand();
			}
			else
			{
				Collapse(true);
				if (mY != GlobalMembers.gApp.mHeight)
				{
					mY = mTargetPos;
				}
			}
			SetVisible(false);
			if (didFadeIn)
			{
				Transition_FadeIn();
			}
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
			bool disableOthers = mTargetPos == ConstantsWP.MAINMENU_OPTIONSMENU_EXPANDED_POS;
			GlobalMembers.gApp.DisableAllExceptThis(this, disableOthers);
		}

		public override void Hide()
		{
			base.Hide();
			mCurrentTansitionState = TRANSITION_STATE.TRANSITION_STATE_IDLE;
			if ((double)mAlphaCurve != 0.0)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBEJ3_WIDGET_HIDE_CURVE, mAlphaCurve);
			}
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
		}

		public void Expand()
		{
			mTargetPos = ConstantsWP.MAINMENU_OPTIONSMENU_EXPANDED_POS - mPlayerHeight;
			ResetFadedBack(true);
			GlobalMembers.gApp.DisableAllExceptThis(this, true);
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
		}

		public void Collapse()
		{
			Collapse(false);
		}

		public void Collapse(bool fadeInstantly)
		{
			mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
			ResetFadedBack(false);
			GlobalMembers.gApp.DisableAllExceptThis(this, false);
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			bool flag = mAllowSlide;
			base.AllowSlideIn(allow, previousTopButton);
			if (allow && mY == mTargetPos && (mState == Bej3WidgetState.STATE_FADING_IN || mState == Bej3WidgetState.STATE_IN))
			{
				GlobalMembers.gApp.DoRateGameDialog();
			}
			if (!flag && previousTopButton != null)
			{
				if (mExpandOnShow)
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
				else
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
				}
			}
			if (allow && (mState == Bej3WidgetState.STATE_IN || mState == Bej3WidgetState.STATE_FADING_IN) && mTopButton != null)
			{
				mTopButton.SetDisabled(false);
			}
		}

		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
		}

		public override bool SlideForDialog(bool slideOut, Bej3Dialog dialog, Bej3ButtonType previousButtonType)
		{
			GlobalMembers.gApp.mIgnoreSound = true;
			bool result = base.SlideForDialog(slideOut, dialog, previousButtonType);
			GlobalMembers.gApp.mIgnoreSound = false;
			if (dialog.mId == 56)
			{
				Expand();
			}
			return result;
		}

		public override void KeyChar(char theChar)
		{
			switch (theChar)
			{
			case '+':
				ConstantsWP.DASHBOARD_SLIDER_SPEED += 0.1f;
				break;
			case '-':
				ConstantsWP.DASHBOARD_SLIDER_SPEED -= 0.1f;
				break;
			case '[':
				ConstantsWP.DASHBOARD_SLIDER_SPEED_SCALAR -= 0.001f;
				break;
			case ']':
				ConstantsWP.DASHBOARD_SLIDER_SPEED_SCALAR += 0.001f;
				break;
			case '(':
				ConstantsWP.TOP_BUTTON_ANIMATION_SPEED -= 0.1f;
				break;
			case ')':
				ConstantsWP.TOP_BUTTON_ANIMATION_SPEED += 0.1f;
				break;
			}
		}
	}
}
