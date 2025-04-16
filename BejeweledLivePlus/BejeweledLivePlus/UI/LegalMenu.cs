using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class LegalMenu : Bej3Widget, CheckboxListener
	{
		private enum LegalMenu_IDS
		{
			BTN_BACK_ID,
			BTN_EULA_ID,
			BTN_PRIVACY_ID,
			BTN_TERMS_ID,
			CHK_ANALYTICS_ID
		}

		private Label mHeadingLabel;

		private Bej3Button mBackButton;

		private Label mAnalyticsLabelHeading;

		private Label mAnalyticsLabel;

		private Bej3Checkbox mAnalyticsCheckbox;

		private Bej3Button mEULAButton;

		private Bej3Button mPrivacyButton;

		private Bej3Button mTermsButton;

		public LegalMenu()
			: base(Menu_Type.MENU_LEGALMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.LEGALMENU_HEADING_LABEL_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetText(GlobalMembers._ID("LEGAL", 3353));
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			mAnalyticsLabelHeading = new Label(GlobalMembersResources.FONT_SUBHEADER, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mAnalyticsLabelHeading.Resize(ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_HEADING_X, ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_HEADING_Y, 0, 0);
			mAnalyticsLabelHeading.SetText(GlobalMembers._ID("USER SHARING", 3354));
			AddWidget(mAnalyticsLabelHeading);
			mAnalyticsLabel = new Label(GlobalMembersResources.FONT_DIALOG);
			mAnalyticsLabel.Resize(ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_X, ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_Y, 0, 0);
			mAnalyticsLabel.SetTextBlock(new Rect(ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_X, ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_Y, ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_WIDTH, ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_HEIGHT), true);
			mAnalyticsLabel.SetTextBlockEnabled(true);
			mAnalyticsLabel.SetTextBlockAlignment(-1);
			mAnalyticsLabel.mLineSpacingOffset = ConstantsWP.LEGALMENU_ANONYMOUS_STATS_LABEL_LINESPACING_OFFSET;
			mAnalyticsLabel.SetClippingEnabled(false);
			mAnalyticsLabel.SetText(GlobalMembers._ID("If you no longer wish to send anonymous game play information to PopCap, uncheck the box and we will disable the transmission of such information.", 3355));
			AddWidget(mAnalyticsLabel);
			mAnalyticsCheckbox = new Bej3Checkbox(4, this);
			mAnalyticsCheckbox.Resize(ConstantsWP.LEGALMENU_ANONYMOUS_STATS_CHECKBOX_X, ConstantsWP.LEGALMENU_ANONYMOUS_STATS_CHECKBOX_Y, 0, 0);
			mAnalyticsCheckbox.mGrayOutWhenDisabled = false;
			AddWidget(mAnalyticsCheckbox);
			mBackButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3356));
			mBackButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_BACK_X, ConstantsWP.LEGALMENU_BUTTON_BACK_Y, mBackButton, true, false);
			AddWidget(mBackButton);
			mEULAButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mEULAButton.SetLabel(GlobalMembers._ID("END USER LICENSE AGREEMENT", 3357));
			mEULAButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_EULA_X, ConstantsWP.LEGALMENU_BUTTON_EULA_Y, mEULAButton);
			AddWidget(mEULAButton);
			mPrivacyButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mPrivacyButton.SetLabel(GlobalMembers._ID("PRIVACY POLICY", 3358));
			mPrivacyButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_PRIVACY_X, ConstantsWP.LEGALMENU_BUTTON_PRIVACY_Y, mPrivacyButton);
			AddWidget(mPrivacyButton);
			mTermsButton = new Bej3Button(3, this, Bej3ButtonType.BUTTON_TYPE_LONG);
			mTermsButton.SetLabel(GlobalMembers._ID("TERMS OF SERVICE", 3359));
			mTermsButton.Resize(0, 0, ConstantsWP.LEGALMENU_BUTTON_WIDTH, 0);
			Bej3Widget.CenterWidgetAt(ConstantsWP.LEGALMENU_BUTTON_TERMS_X, ConstantsWP.LEGALMENU_BUTTON_TERMS_Y, mTermsButton);
			AddWidget(mTermsButton);
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

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.MENU_DIVIDER_Y);
			Bej3Widget.DrawLightBox(g, new Rect(ConstantsWP.LEGALMENU_BOX_1_X, ConstantsWP.LEGALMENU_BOX_1_Y, ConstantsWP.LEGALMENU_BOX_1_W, ConstantsWP.LEGALMENU_BOX_1_H));
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mAnalyticsCheckbox.SetChecked(GlobalMembers.gApp.mProfile.mAllowAnalytics, false);
			mAnalyticsLabelHeading.SetText(GlobalMembers._ID("USER SHARING", 3360));
			if (GlobalMembers.gApp.mProfile.mAllowAnalytics)
			{
				mAnalyticsLabel.SetText(GlobalMembers._ID("If you no longer wish to send anonymous game play information to PopCap, uncheck the box and we will disable the transmission of such information.", 3361));
			}
			else
			{
				mAnalyticsLabel.SetText(GlobalMembers._ID("If you wish to send anonymous game play information to PopCap, check the box and we will enable the transmission of such information.", 3362));
			}
		}

		public override void Show()
		{
			base.Show();
			mTargetPos = 106;
			SetVisible(false);
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

		public virtual void CheckboxChecked(int theId, bool isChecked)
		{
			if (theId == 4 && isChecked != GlobalMembers.gApp.mProfile.mAllowAnalytics)
			{
				GlobalMembers.gApp.mProfile.mAllowAnalytics = isChecked;
				GlobalMembers.gApp.mProfile.WriteProfile();
				LinkUpAssets();
			}
		}
	}
}
