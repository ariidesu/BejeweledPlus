using System.Globalization;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class ZenOptionsMenu : Bej3Widget
	{
		private ZenOptionsMode mMode;

		private ZenOptionsMode mTransitionToMode;

		private Label mHeadingLabel;

		private Bej3ScrollWidget mScrollWidget;

		private ZenOptionsContainer mZenOptionsContainer;

		private Label mDescLabel;

		private Bej3Button mAmbientSoundBtn;

		private Bej3Button mMantrasBtn;

		private Bej3Button mBreathModBtn;

		private Bej3Button mBackButton;

		private Bej3Button mResumeButton;

		private void Init()
		{
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE, GlobalMembers._ID("ZEN OPTIONS", 3631), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE);
			mHeadingLabel.Resize(GlobalMembers.gApp.mWidth / 2, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			mZenOptionsContainer = new ZenOptionsContainer();
			mZenOptionsContainer.Resize(0, ConstantsWP.ZENOPTIONS_CONTAINER_Y, ConstantsWP.ZENOPTIONS_CENTER_X * 2, GlobalMembers.gApp.mHeight - ConstantsWP.ZENOPTIONS_CONTAINER_Y);
			AddWidget(mZenOptionsContainer);
			int num = ConstantsWP.DIALOG_HEADING_LABEL_Y + GlobalMembersResources.FONT_HUGE.GetHeight();
			mDescLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("Customise your Zen Experience!", 3632), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE);
			mDescLabel.Resize(GlobalMembers.gApp.mWidth / 2, num, 0, 0);
			AddWidget(mDescLabel);
			num += GlobalMembersResources.FONT_SUBHEADER.GetHeight() * 3 / 2;
			mAmbientSoundBtn = new Bej3Button(18, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mAmbientSoundBtn.SetLabel(GlobalMembers._ID("AMBIENT SOUNDS", 3633));
			mAmbientSoundBtn.Resize(ConstantsWP.ZENOPTIONS_CENTER_X - ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH / 2, num, ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH, 0);
			AddWidget(mAmbientSoundBtn);
			num += ConstantsWP.ZENOPTIONS_BUTTON_STEP_Y;
			mMantrasBtn = new Bej3Button(19, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mMantrasBtn.SetLabel(GlobalMembers._ID("MANTRAS", 3634));
			mMantrasBtn.Resize(ConstantsWP.ZENOPTIONS_CENTER_X - ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH / 2, num, ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH, 0);
			AddWidget(mMantrasBtn);
			num += ConstantsWP.ZENOPTIONS_BUTTON_STEP_Y;
			mBreathModBtn = new Bej3Button(20, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mBreathModBtn.SetLabel(GlobalMembers._ID("BREATHING MODULATION", 3635));
			mBreathModBtn.Resize(ConstantsWP.ZENOPTIONS_CENTER_X - ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH / 2, num, ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH, 0);
			AddWidget(mBreathModBtn);
			mBackButton = new Bej3Button(21, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3636));
			mBackButton.Resize(ConstantsWP.ZENOPTIONS_CENTER_X - ConstantsWP.BEJ3BUTTON_AUTOSCALE_DEFAULT_WIDTH / 2, ConstantsWP.ZENOPTIONS_BACK_BTN_Y, ConstantsWP.BEJ3BUTTON_AUTOSCALE_DEFAULT_WIDTH, 0);
			AddWidget(mBackButton);
			num += ConstantsWP.ZENOPTIONS_BUTTON_STEP_Y;
			mResumeButton = new Bej3Button(22, this, Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			mResumeButton.SetLabel(GlobalMembers._ID("RESUME", 3637));
			mResumeButton.Resize(ConstantsWP.ZENOPTIONS_CENTER_X - ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH / 2, num, ConstantsWP.BEJ3BUTTON_LONG_DEFAULT_WIDTH, 0);
			AddWidget(mResumeButton);
			num += ConstantsWP.ZENOPTIONS_BUTTON_STEP_Y;
			SetMode(ZenOptionsMode.MODE_MENU_SELECT);
		}

		protected ZenOptionsMenu(Menu_Type type, bool hasBackButton, Bej3ButtonType topButtonType)
			: base(type, hasBackButton, topButtonType)
		{
			Init();
			LinkUpAssets();
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		public ZenOptionsMenu()
			: base(Menu_Type.MENU_ZENOPTIONSMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
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
				int theId = 10001;
				if (mMode == ZenOptionsMode.MODE_AMBIENT_SOUNDS || mMode == ZenOptionsMode.MODE_BREATH_MOD || mMode == ZenOptionsMode.MODE_MANTRAS)
				{
					theId = 21;
				}
				ButtonDepress(theId);
			}
		}

		public override void Draw(Graphics g)
		{
			GlobalMembers.gApp.mWidgetManager.FlushDeferredOverlayWidgets(1);
			DrawFadedBack(g);
			g.SetColor(Color.White);
			Bej3Widget.DrawDialogBox(g, mWidth, 0f, true, true);
			if (mMode != 0)
			{
				Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.MENU_DIVIDER_Y);
			}
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			base.DrawOverlay(g, thePriority);
		}

		public override void Update()
		{
			if (mTransitionToMode != mMode && IsInOutPosition())
			{
				SetMode(mTransitionToMode);
				if (mTransitionToMode == ZenOptionsMode.MODE_MENU_SELECT)
				{
					mTargetPos = ConstantsWP.ZENOPTIONS_MENU_SELECT_Y;
					mBackButton.mY = ConstantsWP.ZENOPTIONS_BACK_BTN_Y;
				}
				else if (mTransitionToMode == ZenOptionsMode.MODE_BREATH_MOD)
				{
					mTargetPos = ConstantsWP.ZENOPTIONS_MENU_BREATH_MOD_Y;
					mBackButton.mY = ConstantsWP.ZENOPTIONS_MENU_BREATH_MOD_BACK_BTN_Y;
				}
				else
				{
					mTargetPos = 0;
					mBackButton.mY = ConstantsWP.ZENOPTIONS_BACK_BTN_Y;
				}
			}
			base.Update();
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			base.AllowSlideIn(allow, previousTopButton);
		}

		public override void ButtonMouseEnter(int theId)
		{
		}

		public override void ButtonDepress(int theId)
		{
			switch (theId)
			{
			case 22:
			case 10001:
				Collapse();
				break;
			case 18:
				mTransitionToMode = ZenOptionsMode.MODE_AMBIENT_SOUNDS;
				mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
				break;
			case 19:
				mTransitionToMode = ZenOptionsMode.MODE_MANTRAS;
				mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
				break;
			case 20:
				mTransitionToMode = ZenOptionsMode.MODE_BREATH_MOD;
				mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
				break;
			case 21:
				mTransitionToMode = ZenOptionsMode.MODE_MENU_SELECT;
				mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
				break;
			}
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			LinkUpAssets();
		}

		public override void HideCompleted()
		{
			BejeweledLivePlusApp.UnloadContent("ZenOptions");
			base.HideCompleted();
		}

		public override void LinkUpAssets()
		{
			if (mZenOptionsContainer != null)
			{
				mZenOptionsContainer.LinkUpAssets();
			}
		}

		public override void Show()
		{
			BejeweledLivePlusApp.LoadContent("ZenOptions");
			LinkUpAssets();
			mZenOptionsContainer.Show();
			base.Show();
		}

		public override void ShowCompleted()
		{
			if (mTopButton != null)
			{
				mTopButton.SetDisabled(false);
			}
			base.ShowCompleted();
		}

		public override void Hide()
		{
			Bej3WidgetState mState2 = mState;
			base.Hide();
			mZenOptionsContainer.Hide();
		}

		public void Expand()
		{
			Bej3Widget.DisableWidget(mZenOptionsContainer, false);
			GlobalMembers.gApp.DoZenOptionsMenu();
			SetMode(ZenOptionsMode.MODE_MENU_SELECT);
			mTargetPos = ConstantsWP.ZENOPTIONS_MENU_SELECT_Y;
			ResetFadedBack(true);
			GlobalMembers.gApp.DisableAllExceptThis(this, true);
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			Bej3Widget.mCurrentSlidingMenu = this;
		}

		public void Collapse()
		{
			Collapse(false, false);
		}

		public void Collapse(bool fadeInstantly)
		{
			Collapse(fadeInstantly, false);
		}

		public void Collapse(bool fadeInstantly, bool fromRestart)
		{
			if (mZenOptionsContainer.mAmbientSoundId >= 0)
			{
				bool flag = mZenOptionsContainer.AmbientLoadSound(mZenOptionsContainer.mAmbientSoundId);
				mZenOptionsContainer.mAmbientSoundId = -1;
				mZenOptionsContainer.mAmbientSoundStartDelay = 0;
				if (flag && GlobalMembers.gApp.mProfile.mNoiseOn && !GlobalMembers.gApp.IsMuted())
				{
					ZenBoard zenBoard = null;
					if (GlobalMembers.gApp.mBoard != null)
					{
						zenBoard = (ZenBoard)GlobalMembers.gApp.mBoard;
						zenBoard.PlayZenNoise();
					}
				}
			}
			GlobalMembers.gApp.GoBackToGame();
			mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
			GlobalMembers.gApp.DisableAllExceptThis(this, false);
			if (mState == Bej3WidgetState.STATE_FADING_IN)
			{
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
			}
			Transition_SlideOut();
		}

		public void SetMode(ZenOptionsMode mode)
		{
			mMode = (mTransitionToMode = mode);
			mZenOptionsContainer.SetMode(mode);
			Bej3Widget.DisableWidget(mAmbientSoundBtn, mode != ZenOptionsMode.MODE_MENU_SELECT);
			Bej3Widget.DisableWidget(mMantrasBtn, mode != ZenOptionsMode.MODE_MENU_SELECT);
			Bej3Widget.DisableWidget(mBreathModBtn, mode != ZenOptionsMode.MODE_MENU_SELECT);
			Bej3Widget.DisableWidget(mBackButton, mode == ZenOptionsMode.MODE_MENU_SELECT);
			Bej3Widget.DisableWidget(mResumeButton, mode != ZenOptionsMode.MODE_MENU_SELECT);
			Bej3Widget.DisableWidget(mDescLabel, mode != ZenOptionsMode.MODE_MENU_SELECT);
			string name = CultureInfo.CurrentCulture.Name;
			switch (mode)
			{
			case ZenOptionsMode.MODE_MENU_SELECT:
				mHeadingLabel.Resize(GlobalMembers.gApp.mWidth / 2, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
				mHeadingLabel.SetText(GlobalMembers._ID("Zen Options", 3483));
				break;
			case ZenOptionsMode.MODE_AMBIENT_SOUNDS:
				switch (name)
				{
				case "de-DE":
				case "es-ES":
				case "it-IT":
					mHeadingLabel.Resize(GlobalMembers.gApp.mWidth / 2, ConstantsWP.DIALOG_HEADING_LABEL_Y + 45, 0, 0);
					break;
				default:
					mHeadingLabel.Resize(GlobalMembers.gApp.mWidth / 2, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
					break;
				}
				mHeadingLabel.SetText(GlobalMembers._ID("Ambient Sounds", 3484));
				break;
			case ZenOptionsMode.MODE_MANTRAS:
				mHeadingLabel.Resize(GlobalMembers.gApp.mWidth / 2, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
				mHeadingLabel.SetText(GlobalMembers._ID("Mantras", 3485));
				break;
			case ZenOptionsMode.MODE_BREATH_MOD:
				mHeadingLabel.Resize(GlobalMembers.gApp.mWidth / 2, ConstantsWP.DIALOG_HEADING_LABEL_Y + 45, 0, 0);
				mHeadingLabel.SetText(GlobalMembers._ID("Breathing Modulation", 3486));
				break;
			}
		}

		public override void PlayMenuMusic()
		{
		}
	}
}
