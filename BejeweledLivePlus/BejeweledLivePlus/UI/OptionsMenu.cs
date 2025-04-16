using System;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class OptionsMenu : Bej3Widget, CheckboxListener, SliderListener
	{
		private enum OptionsMenu_BUTTON_IDS
		{
			BTN_ABOUT_ID,
			BTN_EULA_ID,
			BTN_PP_ID,
			BTN_TOS_ID,
			BTN_HELP_ID,
			BTN_CREDITS_ID,
			BTN_BACK_ID
		}

		private Bej3Button mEULAButton;

		private Bej3Button mPrivacyButton;

		private Bej3Button mTermsButton;

		private Bej3Button mCreditsButton;

		private Bej3Button mAboutButton;

		private Bej3Button mBackButton;

		private Bej3Button mHelpButton;

		public OptionsMenu()
			: base(Menu_Type.MENU_OPTIONSMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			Resize(0, GlobalMembers.gApp.mHeight, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mFinalY = 255;
			mBackButton = new Bej3Button(6, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3606));
			mBackButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_BACK_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y, mBackButton, true, false);
			AddWidget(mBackButton);
			mCreditsButton = new Bej3Button(5, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mCreditsButton.SetLabel(GlobalMembers._ID("CREDITS", 3605));
			mCreditsButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_BACK_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y - 50, mCreditsButton);
			AddWidget(mCreditsButton);
			mHelpButton = new Bej3Button(4, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mHelpButton.SetLabel(GlobalMembers._ID("HELP", 3337));
			mHelpButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_BACK_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y - 50 - 93, mHelpButton);
			AddWidget(mHelpButton);
			mTermsButton = new Bej3Button(3, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mTermsButton.SetLabel(GlobalMembers._ID("TERMS OF SERVICE", 3359));
			mTermsButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_TERMS_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y - 50 - 186, mTermsButton);
			AddWidget(mTermsButton);
			mPrivacyButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mPrivacyButton.SetLabel(GlobalMembers._ID("PRIVACY POLICY", 3358));
			mPrivacyButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_PRIVACY_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y - 50 - 279, mPrivacyButton);
			AddWidget(mPrivacyButton);
			mEULAButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mEULAButton.SetLabel(GlobalMembers._ID("END USER LICENSE AGREEMENT", 3357));
			mEULAButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_EULA_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y - 50 - 372, mEULAButton);
			AddWidget(mEULAButton);
			mAboutButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mAboutButton.SetLabel(GlobalMembers._ID("ABOUT", 3419));
			mAboutButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_EULA_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y - 50 - 465, mAboutButton);
			AddWidget(mAboutButton);
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

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public override void Draw(Graphics g)
		{
			DrawFadedBack(g);
			Bej3Widget.DrawDialogBox(g, mWidth);
		}

		public override void Update()
		{
			base.Update();
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
		}

		public override void ButtonDepress(int theId)
		{
			GlobalMembers.gApp.mProfile.WriteProfile();
			GlobalMembers.gApp.WriteToRegistry();
			switch (theId)
			{
			case 10001:
				GlobalMembers.gApp.DoMainMenu();
				Transition_SlideOut();
				break;
			case 5:
				GlobalMembers.gApp.DoCreditsMenu();
				Transition_SlideOut();
				break;
			case 0:
				GlobalMembers.gApp.DoAboutMenu();
				Transition_SlideOut();
				break;
			case 6:
				GlobalMembers.gApp.DoMainMenu();
				Transition_SlideOut();
				break;
			case 4:
				GlobalMembers.gApp.DoHelpDialog(0, 3);
				Transition_SlideOut();
				break;
			case 1:
				GlobalMembers.gApp.OpenURLWithWarning(GlobalMembers._ID("http://tos.ea.com/legalapp/mobileeula/US/en/OTHER", 3581));
				break;
			case 2:
				GlobalMembers.gApp.OpenURLWithWarning(GlobalMembers._ID("http://tos.ea.com/legalapp/WEBPRIVACY/US/en/PC/", 3582));
				break;
			case 3:
				GlobalMembers.gApp.OpenURLWithWarning(GlobalMembers._ID("http://tos.ea.com/legalapp/WEBTERMS/US/en/PC/", 3583));
				break;
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
		}

		public override void Show()
		{
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_OPTIONSMENU)
			{
				mTargetPos = 0;
			}
			else
			{
				mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
			}
			base.Show();
			ResetFadedBack(true);
			SetVisible(false);
		}

		public override void Hide()
		{
			base.Hide();
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
		}

		public override void SetDisabled(bool isDisabled)
		{
			base.SetDisabled(isDisabled);
		}

		public void CheckboxChecked(int theId, bool isChecked)
		{
			throw new NotImplementedException();
		}

		public void SliderVal(int theId, double theVal)
		{
			throw new NotImplementedException();
		}

		public void SliderReleased(int theId, double theVal)
		{
			throw new NotImplementedException();
		}
	}
}
