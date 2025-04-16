using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class ZenInfoDialog : Bej3Dialog
	{
		private Label mExtraMessageLabel1;

		private Label mExtraMessageLabel2;

		private Label mExtraMessageLabel3;

		private Label mExtraMessageLabel4;

		private Label mExtraMessageLabel5;

		public ZenInfoDialog()
			: base(44, true, GlobalMembers._ID("ZEN", 3443), "", GlobalMembers._ID("CONTINUE", 3444), 3, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
		{
			LinkUpAssets();
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, ConstantsWP.ZENINFODIALOG_WIDTH, GlobalMembers.gApp.mHeight);
			mTargetPos = 0;
			if (mTopButton != null)
			{
				mTopButton.mId = 1001;
			}
			int zENINFODIALOG_MSG_START_Y = ConstantsWP.ZENINFODIALOG_MSG_START_Y;
			int num = ConstantsWP.ZENINFODIALOG_WIDTH - ConstantsWP.ZENINFODIALOG_MSG_LEFT_X * 2;
			mExtraMessageLabel1 = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("Relax as you play this endless mode", 3445));
			mExtraMessageLabel1.SetClippingEnabled(false);
			mExtraMessageLabel1.SetTextBlock(new Rect(ConstantsWP.ZENINFODIALOG_MSG_LEFT_X, zENINFODIALOG_MSG_START_Y, num, ConstantsWP.ZENINFODIALOG_MSG_HEIGHT), false);
			mExtraMessageLabel1.SetTextBlockEnabled(true);
			AddWidget(mExtraMessageLabel1);
			zENINFODIALOG_MSG_START_Y += ConstantsWP.ZENINFODIALOG_MSG_HEIGHT;
			mExtraMessageLabel2 = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Chill out with soothing ambient sounds", 3446));
			mExtraMessageLabel2.SetClippingEnabled(false);
			mExtraMessageLabel2.SetTextBlock(new Rect(ConstantsWP.ZENINFODIALOG_MSG_LEFT_X + ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, zENINFODIALOG_MSG_START_Y, num - ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, ConstantsWP.ZENINFODIALOG_MSG_HEIGHT), false);
			mExtraMessageLabel2.SetTextBlockEnabled(true);
			mExtraMessageLabel2.SetTextBlockAlignment(-1);
			AddWidget(mExtraMessageLabel2);
			zENINFODIALOG_MSG_START_Y += ConstantsWP.ZENINFODIALOG_MSG_HEIGHT;
			mExtraMessageLabel3 = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Boost your brain with positive mantras", 3447));
			mExtraMessageLabel3.SetClippingEnabled(false);
			mExtraMessageLabel3.SetTextBlock(new Rect(ConstantsWP.ZENINFODIALOG_MSG_LEFT_X + ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, zENINFODIALOG_MSG_START_Y, num - ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, ConstantsWP.ZENINFODIALOG_MSG_HEIGHT), false);
			mExtraMessageLabel3.SetTextBlockEnabled(true);
			mExtraMessageLabel3.SetTextBlockAlignment(-1);
			AddWidget(mExtraMessageLabel3);
			zENINFODIALOG_MSG_START_Y += ConstantsWP.ZENINFODIALOG_MSG_HEIGHT;
			mExtraMessageLabel4 = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Release stress with breathing modulation", 3448));
			mExtraMessageLabel4.SetClippingEnabled(false);
			mExtraMessageLabel4.SetTextBlock(new Rect(ConstantsWP.ZENINFODIALOG_MSG_LEFT_X + ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, zENINFODIALOG_MSG_START_Y, num - ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, ConstantsWP.ZENINFODIALOG_MSG_HEIGHT), false);
			mExtraMessageLabel4.SetTextBlockEnabled(true);
			mExtraMessageLabel4.SetTextBlockAlignment(-1);
			AddWidget(mExtraMessageLabel4);
			zENINFODIALOG_MSG_START_Y += ConstantsWP.ZENINFODIALOG_MSG_HEIGHT;
			mExtraMessageLabel5 = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("Headphones are recommended", 3449));
			mExtraMessageLabel5.SetClippingEnabled(false);
			mExtraMessageLabel5.SetTextBlock(new Rect(ConstantsWP.ZENINFODIALOG_MSG_LEFT_X, zENINFODIALOG_MSG_START_Y, num, ConstantsWP.ZENINFODIALOG_MSG_HEIGHT), false);
			mExtraMessageLabel5.SetTextBlockEnabled(true);
			mExtraMessageLabel5.SetTextBlockAlignment(0);
			AddWidget(mExtraMessageLabel5);
			Bej3Widget.CenterWidgetAt(ConstantsWP.ZENINFODIALOG_WIDTH / 2, ConstantsWP.MENU_BOTTOM_BUTTON_Y, mYesButton, true, false);
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
			((Bej3Button)mYesButton).SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_DIVIDER_GEM, mExtraMessageLabel2.mX - ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, mExtraMessageLabel2.mY + ConstantsWP.ZENINFODIALOG_GEM_BULLET_Y_OFFSET);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_DIVIDER_GEM, mExtraMessageLabel3.mX - ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, mExtraMessageLabel3.mY + ConstantsWP.ZENINFODIALOG_GEM_BULLET_Y_OFFSET);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_DIVIDER_GEM, mExtraMessageLabel4.mX - ConstantsWP.ZENINFODIALOG_GEM_BULLET_WIDTH, mExtraMessageLabel4.mY + ConstantsWP.ZENINFODIALOG_GEM_BULLET_Y_OFFSET);
		}
	}
}
