using System;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class HelpAndOptionsMenu : Bej3Widget, CheckboxListener, SliderListener
	{
		private enum OptionsMenu_BUTTON_IDS
		{
			BTN_CREDITS_ID,
			BTN_ABOUT_ID,
			BTN_BACK_ID,
			BTN_LEGAL_ID,
			BTN_HELP_ID
		}

		private OptionsContainer mContainer;

		private Bej3Button mCreditsButton;

		private Bej3Button mAboutButton;

		private Bej3Button mLegalButton;

		private Bej3Button mBackButton;

		private Bej3Button mHelpButton;

		public HelpAndOptionsMenu()
			: base(Menu_Type.MENU_HELPANDOPTIONSMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			Resize(0, GlobalMembers.gApp.mHeight, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mFinalY = 256;
			mContainer = new OptionsContainer();
			AddWidget(mContainer);
			mAboutButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mAboutButton.SetLabel(GlobalMembers._ID("ABOUT", 3419));
			mAboutButton.Resize(0, 0, ConstantsWP.OPTIONSMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.OPTIONSMENU_ABOUT_X, ConstantsWP.OPTIONSMENU_ABOUT_Y + 120, mAboutButton);
			mCreditsButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mCreditsButton.SetLabel(GlobalMembers._ID("CREDITS", 3605));
			mCreditsButton.Resize(0, 0, ConstantsWP.OPTIONSMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.OPTIONSMENU_CREDITS_X, ConstantsWP.OPTIONSMENU_CREDITS_Y + 120, mCreditsButton);
			mLegalButton = new Bej3Button(3, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mLegalButton.SetLabel(GlobalMembers._ID("LEGAL", 3421));
			mLegalButton.Resize(0, 0, ConstantsWP.OPTIONSMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.OPTIONSMENU_LEGAL_X, ConstantsWP.OPTIONSMENU_LEGAL_Y + 120, mLegalButton);
			mBackButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3606));
			mBackButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_BACK_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y + 35, mBackButton);
			AddWidget(mBackButton);
			mHelpButton = new Bej3Button(4, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mHelpButton.SetLabel(GlobalMembers._ID("HELP", 3337));
			mHelpButton.Resize(0, 0, ConstantsWP.OPTIONSMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.OPTIONSMENU_ABOUT_X - (ConstantsWP.OPTIONSMENU_ABOUT_X - ConstantsWP.OPTIONSMENU_LEGAL_X) / 2, ConstantsWP.OPTIONSMENU_ABOUT_Y, mHelpButton);
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				ButtonDepress(2);
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
				((MainMenuOptions)GlobalMembers.gApp.mMenus[5]).Expand();
				Transition_SlideOut();
				break;
			case 0:
				GlobalMembers.gApp.DoCreditsMenu();
				Transition_SlideOut();
				break;
			case 1:
				GlobalMembers.gApp.DoAboutMenu();
				Transition_SlideOut();
				break;
			case 3:
				GlobalMembers.gApp.DoLegalMenu();
				Transition_SlideOut();
				break;
			case 2:
				GlobalMembers.gApp.DoMainMenu();
				((MainMenuOptions)GlobalMembers.gApp.mMenus[5]).Expand();
				Transition_SlideOut();
				break;
			case 4:
				GlobalMembers.gApp.DoHelpDialog(0, 3);
				Transition_SlideOut();
				break;
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mContainer.LinkUpAssets();
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
			mContainer.Show();
			base.Show();
			ResetFadedBack(true);
			SetVisible(false);
		}

		public override void Hide()
		{
			mContainer.Hide();
			base.Hide();
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
		}

		public override void SetDisabled(bool isDisabled)
		{
			base.SetDisabled(isDisabled);
			mContainer.SetDisabled(isDisabled);
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
