using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class PauseMenu : Bej3Widget
	{
		private enum PAUSEMENU_BUTTON_IDS
		{
			BTN_PLAY_ID,
			BTN_RESTART_ID,
			BTN_MAINMENU_ID,
			BTN_HELP_ID
		}

		private GameMode mMode;

		private Bej3Button mRestartButton;

		private Bej3Button mMainMenuButton;

		private Bej3Button mPlayButton;

		private Bej3Button mHelpButton;

		public OptionsContainer mOptionsContainer;

		public bool mComingFromHelp;

		protected override void DialogFinished(Bej3Dialog dialog, Bej3ButtonType previousButtonType)
		{
			if (dialog != null)
			{
				mY = dialog.mY;
				SetVisible(true);
			}
		}

		public PauseMenu()
			: base(Menu_Type.MENU_PAUSEMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_MENU)
		{
			mComingFromHelp = false;
			mOptionsContainer = new OptionsContainer();
			AddWidget(mOptionsContainer);
			mRestartButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mRestartButton.SetLabel(GlobalMembers._ID("RESTART", 3422));
			AddWidget(mRestartButton);
			mMainMenuButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			AddWidget(mMainMenuButton);
			mPlayButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			mPlayButton.SetLabel(GlobalMembers._ID("RESUME", 3609));
			AddWidget(mPlayButton);
			mHelpButton = new Bej3Button(3, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mHelpButton.SetLabel(GlobalMembers._ID("HELP", 3423));
			AddWidget(mHelpButton);
			mFinalY = 106;
			SetMode(GameMode.MODE_CLASSIC);
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		public void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition() && (GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_ZEN || GlobalMembers.gApp.mMenus[19].mState == Bej3WidgetState.STATE_OUT) && GlobalMembers.gApp.mBoard.mHyperspace == null && !GlobalMembers.gApp.mBoard.mWantLevelup && GlobalMembers.gApp.mDialogMap.Count == 0 && mY == 106)
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

		public void SetMode(GameMode mode)
		{
			mMode = mode;
			mMainMenuButton.SetLabel(GlobalMembers._ID("MAIN MENU", 3424));
			mMainMenuButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			Bej3Widget.DisableWidget(mMainMenuButton, false);
			Bej3Widget.DisableWidget(mPlayButton, false);
			Bej3Widget.DisableWidget(mRestartButton, mode == GameMode.MODE_ZEN);
			Bej3Widget.DisableWidget(mHelpButton, false);
			mMainMenuButton.Resize(0, 0, ConstantsWP.PAUSEMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.PAUSEMENU_BUTTON_MAINMENU_X, ConstantsWP.PAUSEMENU_BUTTON_MAINMENU_Y, mMainMenuButton);
			if (mode == GameMode.MODE_ZEN)
			{
				mHelpButton.Resize(0, 0, ConstantsWP.PAUSEMENU_BUTTON_WIDTH, 0);
				Bej3Widget.CenterWidgetAt(ConstantsWP.PAUSEMENU_BUTTON_HELP_ZEN_X, ConstantsWP.PAUSEMENU_BUTTON_HELP_ZEN_Y, mHelpButton);
			}
			else
			{
				mHelpButton.Resize(0, 0, ConstantsWP.PAUSEMENU_BUTTON_WIDTH, 0);
				Bej3Widget.CenterWidgetAt(ConstantsWP.PAUSEMENU_BUTTON_HELP_X, ConstantsWP.PAUSEMENU_BUTTON_HELP_Y, mHelpButton);
			}
			mPlayButton.Resize(0, 0, ConstantsWP.PAUSEMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.PAUSEMENU_BUTTON_RESUME_X, ConstantsWP.PAUSEMENU_BUTTON_RESUME_Y, mPlayButton);
			mRestartButton.Resize(0, 0, ConstantsWP.PAUSEMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.PAUSEMENU_BUTTON_RESTART_X, ConstantsWP.PAUSEMENU_BUTTON_RESTART_Y, mRestartButton);
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			bool flag = mAllowSlide;
			if (mTargetPos == mY && IsInOutPosition())
			{
				GlobalMembers.gApp.mIgnoreSound = true;
			}
			base.AllowSlideIn(allow, previousTopButton);
			GlobalMembers.gApp.mIgnoreSound = false;
			if (!flag)
			{
				if (allow && mTargetPos != 0 && mState != Bej3WidgetState.STATE_OUT)
				{
					if (mInterfaceState == InterfaceState.INTERFACE_STATE_PAUSEMENU)
					{
						SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
					}
					else
					{
						SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
					}
					if (mTopButton != null)
					{
						mTopButton.SetDisabled(false);
					}
					if (mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME)
					{
						SetVisible(true);
					}
				}
			}
			else if (allow && GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_ZEN)
			{
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			}
		}

		public override bool SlideForDialog(bool slideOut, Bej3Dialog dialog, Bej3ButtonType previousButtonType)
		{
			int mId = dialog.mId;
			bool mIgnoreSound = mId != 22;
			GlobalMembers.gApp.mIgnoreSound = mIgnoreSound;
			bool result = base.SlideForDialog(slideOut, dialog, previousButtonType);
			GlobalMembers.gApp.mIgnoreSound = false;
			bool showing = !slideOut;
			if (!slideOut)
			{
				if (mId != 22 && mId != 50)
				{
					mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
					showing = false;
					bool flag = false;
					if (GlobalMembers.gApp.mBoard != null)
					{
						flag = GlobalMembers.gApp.mBoard.mInReplay;
						if (GlobalMembers.gApp.mBoard.mIllegalMoveTutorial && GlobalMembers.gApp.mBoard.mDeferredTutorialVector.Count > 0)
						{
							flag = true;
						}
					}
					if (GlobalMembers.gApp.mBoard == null || !flag)
					{
						SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
					}
					else if (flag)
					{
						SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
					}
				}
				else
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
				if (mTopButton != null)
				{
					mTopButton.SetDisabled(false);
				}
			}
			ResetFadedBack(showing);
			return result;
		}

		public override void Draw(Graphics g)
		{
			mAlphaCurve.SetConstant(1.0);
			GlobalMembers.gApp.mWidgetManager.FlushDeferredOverlayWidgets(1);
			DrawFadedBack(g);
			g.SetColor(Color.White);
			Bej3Widget.DrawDialogBox(g, mWidth);
			DeferOverlay(1);
		}

		public override void DrawOverlay(Graphics g)
		{
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME && GlobalMembers.gApp.mBoard != null && GlobalMembers.gApp.mBoard.WantWarningGlow())
			{
				g.PushState();
				g.SetColor(GlobalMembers.gApp.mBoard.GetWarningGlowColor());
				if (GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_BUTTERFLY)
				{
					g.SetDrawMode(Graphics.DrawMode.Additive);
				}
				g.SetColorizeImages(true);
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_DASHBOARD_DM_OVERLAY, 0, (int)GlobalMembers.IMG_SYOFS(706));
				g.PopState();
			}
		}

		public override void Show()
		{
			if (mState != Bej3WidgetState.STATE_IN)
			{
				ResetFadedBack(true);
				mOptionsContainer.Show();
				base.Show();
				if (mInterfaceState != InterfaceState.INTERFACE_STATE_PAUSEMENU)
				{
					Collapse(true);
				}
				mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
				if (mInterfaceState == InterfaceState.INTERFACE_STATE_PAUSEMENU || GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN)
				{
					SetVisible(true);
				}
				else
				{
					SetVisible(false);
				}
				if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN && !mComingFromHelp)
				{
					AllowSlideIn(true, null);
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
				}
				mAlphaCurve.SetCurve("b+1,0,0.03,1,~###,####         u####");
			}
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
		}

		public override void Hide()
		{
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME)
			{
				ResetFadedBack(false);
			}
			mOptionsContainer.Hide();
			base.Hide();
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_GAMEDETAILMENU && mTopButton != null)
			{
				mTopButton.SetDisabled(true);
			}
		}

		public override void ButtonMouseEnter(int theId)
		{
		}

		public override void ButtonDepress(int theId)
		{
			GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
			GlobalMembers.gApp.mProfile.WriteProfile();
			GlobalMembers.gApp.WriteToRegistry();
			switch (theId)
			{
			case 10001:
				if (mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME)
				{
					bool flag = true;
					if (GlobalMembers.gApp.mBoard != null && GlobalMembers.gApp.mBoard.mGameFinished)
					{
						flag = false;
					}
					if (flag)
					{
						Expand();
					}
				}
				else if (mInterfaceState == InterfaceState.INTERFACE_STATE_PAUSEMENU)
				{
					Collapse();
				}
				else if (mInterfaceState == InterfaceState.INTERFACE_STATE_GAMEDETAILMENU)
				{
					GlobalMembers.gApp.mMenus[6].ButtonDepress(10001);
				}
				break;
			case 1:
			{
				GlobalMembers.gApp.mBoard.RestartGameRequest();
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
				Bej3Dialog bej3Dialog2 = (Bej3Dialog)GlobalMembers.gApp.GetDialog(22);
				if (bej3Dialog2 != null)
				{
					Transition_SlideOut();
				}
				mAlphaCurve.SetConstant(1.0);
				break;
			}
			case 0:
				Collapse();
				break;
			case 2:
			{
				GlobalMembers.gApp.mBoard.MainMenuRequest();
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
				Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.GetDialog(22);
				if (bej3Dialog != null)
				{
					Transition_SlideOut();
				}
				mAlphaCurve.SetConstant(1.0);
				break;
			}
			case 3:
				switch (mMode)
				{
				case GameMode.MODE_DIAMOND_MINE:
					GlobalMembers.gApp.DoHelpDialog(22, 1);
					break;
				case GameMode.MODE_LIGHTNING:
					GlobalMembers.gApp.DoHelpDialog(10, 1);
					break;
				case GameMode.MODE_BUTTERFLY:
					GlobalMembers.gApp.DoHelpDialog(17, 1);
					break;
				case GameMode.MODE_ICESTORM:
					GlobalMembers.gApp.DoHelpDialog(20, 1);
					break;
				default:
					GlobalMembers.gApp.DoHelpDialog(0, 1);
					break;
				}
				Transition_SlideOut();
				break;
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mOptionsContainer.LinkUpAssets();
		}

		public override void PlayMenuMusic(bool isRestart = false)
		{
		}

		public void Collapse(bool fadeInstantly)
		{
			Collapse(fadeInstantly, false);
		}

		public void Collapse()
		{
			Collapse(false, false);
		}

		public void Collapse(bool fadeInstantly, bool fromRestart)
		{
			if (mInterfaceState != InterfaceState.INTERFACE_STATE_INGAME)
			{
				GlobalMembers.gApp.GoBackToGame(fromRestart);
			}
			mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
			ResetFadedBack(false);
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			GlobalMembers.gApp.DisableAllExceptThis(this, false);
			if (fromRestart && mTopButton != null)
			{
				mTopButton.SetDisabled(false);
			}
		}

		public void Expand()
		{
			Bej3Widget.DisableWidget(mOptionsContainer, false);
			mAlphaCurve.SetConstant(1.0);
			GlobalMembers.gApp.DoPauseMenu();
			mTargetPos = mFinalY;
			ResetFadedBack(true);
			GlobalMembers.gApp.DisableAllExceptThis(this, true);
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			Bej3Widget.mCurrentSlidingMenu = this;
		}
	}
}
