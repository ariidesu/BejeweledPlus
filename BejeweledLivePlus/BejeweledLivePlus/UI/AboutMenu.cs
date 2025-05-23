using System.Collections.Generic;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	internal class AboutMenu : Bej3Widget, CheckboxListener
	{
		private enum AboutMenu_IDS
		{
			BTN_TERMS_ID,
			BTN_BACK_ID,
			CHK_ANALYTICS_ID,
			BTN_POPCAP_COM_ID,
			BTN_SUPPORT_ID
		}

		private Label mHeadingLabel;

		private List<SexyFramework.Widget.Widget> mMessageLabels = new List<SexyFramework.Widget.Widget>();

		private Bej3Button mBackButton;

		public AboutMenu()
			: base(Menu_Type.MENU_ABOUTMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight - ConstantsWP.ABOUTMENU_OFFSET_Y);
			mFinalY = ConstantsWP.ABOUTMENU_OFFSET_Y;
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.ABOUTMENU_HEADING_LABEL_X, ConstantsWP.ABOUTMENU_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetText(GlobalMembers._ID("ABOUT", 3521));
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			Font fONT_DIALOG = GlobalMembersResources.FONT_DIALOG;
			int aBOUTMENU_MESSAGE_2_LABEL_Y = ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_Y;
			Label label = new Label(fONT_DIALOG);
			label.SetText(GlobalMembers._ID("© 2012 Electronic Arts Inc. Bejeweled and PopCap are trademarks of Electronic Arts Inc.", 3523));
			label.SetTextBlockEnabled(true);
			int visibleHeight = label.GetVisibleHeight(ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_WIDTH);
			label.SetTextBlock(new Rect(ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_X, aBOUTMENU_MESSAGE_2_LABEL_Y, ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_WIDTH, visibleHeight), false);
			AddWidget(label);
			mMessageLabels.Add(label);
			aBOUTMENU_MESSAGE_2_LABEL_Y += visibleHeight + ConstantsWP.ABOUTMENU_MESSAGE_2_TEXT_OFFSET;
			label = new Label(fONT_DIALOG);
			label.SetText(GlobalMembers._ID("For Support visit us at:", 3075));
			label.SetTextBlockEnabled(true);
			visibleHeight = label.GetVisibleHeight(ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_WIDTH);
			label.SetTextBlock(new Rect(ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_X, aBOUTMENU_MESSAGE_2_LABEL_Y, ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_WIDTH, visibleHeight), false);
			AddWidget(label);
			mMessageLabels.Add(label);
			aBOUTMENU_MESSAGE_2_LABEL_Y += visibleHeight + ConstantsWP.ABOUTMENU_MESSAGE_2_LINK_HEIGHT;
			Bej3HyperlinkWidget bej3HyperlinkWidget = new Bej3HyperlinkWidget(4, this);
			bej3HyperlinkWidget.Resize(ConstantsWP.ABOUTMENU_LINK_X, aBOUTMENU_MESSAGE_2_LABEL_Y, ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_WIDTH, ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_HEIGHT);
			bej3HyperlinkWidget.SetFont(GlobalMembersResources.FONT_DIALOG);
			bej3HyperlinkWidget.SetLayerColor(0, Bej3Widget.COLOR_HYPERLINK_FILL);
			bej3HyperlinkWidget.mUnderlineSize = 0;
			bej3HyperlinkWidget.mLabel = GlobalMembers._ID("help@eamobile.com", 3076);
			Bej3Widget.CenterWidgetAt(ConstantsWP.ABOUTMENU_LINK_X, aBOUTMENU_MESSAGE_2_LABEL_Y, bej3HyperlinkWidget);
			AddWidget(bej3HyperlinkWidget);
			mMessageLabels.Add(bej3HyperlinkWidget);
			aBOUTMENU_MESSAGE_2_LABEL_Y += ConstantsWP.ABOUTMENU_VERSION_Y_OFFSET + ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_HEIGHT - ConstantsWP.ABOUTMENU_POST_LINK_OFFSET_Y;
			label = new Label(fONT_DIALOG);
			label.Resize(ConstantsWP.ABOUTMENU_LINK_X, aBOUTMENU_MESSAGE_2_LABEL_Y, ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_WIDTH, 0);
			label.SetText(GlobalMembers._ID("Version:", 3077));
			AddWidget(label);
			mMessageLabels.Add(label);
			aBOUTMENU_MESSAGE_2_LABEL_Y += ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_HEIGHT_2;
			label = new Label(fONT_DIALOG);
			label.Resize(ConstantsWP.ABOUTMENU_LINK_X, aBOUTMENU_MESSAGE_2_LABEL_Y, ConstantsWP.ABOUTMENU_MESSAGE_2_LABEL_WIDTH, 0);
			label.SetText(GlobalMembers.gApp.mVersion);
			AddWidget(label);
			mMessageLabels.Add(label);
			mBackButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE, true);
			mBackButton.SetLabel(GlobalMembers._ID("BACK", 3524));
			Bej3Widget.CenterWidgetAt(ConstantsWP.ABOUTMENU_CLOSE_BUTTON_X, ConstantsWP.ABOUTMENU_CLOSE_BUTTON_Y, mBackButton, true, false);
			AddWidget(mBackButton);
			LinkUpAssets();
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				ButtonDepress(1);
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawLightBox(g, new Rect(ConstantsWP.ABOUTMENU_BOX_1_X, ConstantsWP.ABOUTMENU_BOX_1_Y, ConstantsWP.ABOUTMENU_BOX_1_W, ConstantsWP.ABOUTMENU_BOX_1_H));
		}

		public override void Update()
		{
			base.Update();
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
		}

		public override void Hide()
		{
			base.Hide();
		}

		public override void Show()
		{
			base.Show();
			mTargetPos = ConstantsWP.ABOUTMENU_OFFSET_Y;
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
			case 1:
				GlobalMembers.gApp.DoOptionsMenu();
				Transition_SlideOut();
				break;
			}
		}

		public virtual void CheckboxChecked(int theId, bool check)
		{
			if (theId == 2 && check != GlobalMembers.gApp.mProfile.mAllowAnalytics)
			{
				GlobalMembers.gApp.mProfile.mAllowAnalytics = check;
				GlobalMembers.gApp.mProfile.WriteProfile();
				LinkUpAssets();
			}
		}
	}
}
