using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class IntroDialog : Bej3Dialog
	{
		private Label mExtraMessageLabel1;

		private Label mExtraMessageLabel2;

		private Label mExtraMessageLabel3;

		private Label mExtraMessageLabel4;

		private Label mExtraMessageLabel5;

		public IntroDialog()
			: base(58, true, GlobalMembers._ID("WELCOME TO BEJEWELED", 3347), "", GlobalMembers._ID("CONTINUE", 3348), 3, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			mExtraMessageLabel1 = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("New to Bejeweled?", 3349));
			mExtraMessageLabel1.SetTextBlock(new Rect(ConstantsWP.INTRO_DIALOG_TEXT_X, ConstantsWP.INTRO_DIALOG_TEXT_1_Y, ConstantsWP.INTRO_DIALOG_TEXT_WIDTH, 0), false);
			mExtraMessageLabel1.SetTextBlockEnabled(true);
			AddWidget(mExtraMessageLabel1);
			mExtraMessageLabel2 = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("^700443^Start with ^7514BB^Classic^700443^.", 3350));
			mExtraMessageLabel2.SetTextBlock(new Rect(ConstantsWP.INTRO_DIALOG_TEXT_X, ConstantsWP.INTRO_DIALOG_TEXT_2_Y, ConstantsWP.INTRO_DIALOG_TEXT_WIDTH, 0), false);
			mExtraMessageLabel2.SetTextBlockEnabled(true);
			mExtraMessageLabel2.SetLayerColor(0, Color.White);
			AddWidget(mExtraMessageLabel2);
			mExtraMessageLabel3 = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("Already a Bejeweled Pro?", 3351));
			mExtraMessageLabel3.SetTextBlock(new Rect(ConstantsWP.INTRO_DIALOG_TEXT_X, ConstantsWP.INTRO_DIALOG_TEXT_3_Y, ConstantsWP.INTRO_DIALOG_TEXT_WIDTH, 0), false);
			mExtraMessageLabel3.SetTextBlockEnabled(true);
			AddWidget(mExtraMessageLabel3);
			mExtraMessageLabel4 = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("^700443^Try all-new ^7514BB^Butterflies^700443^,\ndig for treasure in ^7514BB^Diamond Mine^700443^\nor relax body and mind in ^7514BB^Zen^700443^ mode.", 3352));
			mExtraMessageLabel4.SetTextBlock(new Rect(ConstantsWP.INTRO_DIALOG_TEXT_X, ConstantsWP.INTRO_DIALOG_TEXT_4_Y, ConstantsWP.INTRO_DIALOG_TEXT_WIDTH, 0), false);
			mExtraMessageLabel4.SetTextBlockEnabled(true);
			mExtraMessageLabel4.SetLayerColor(0, Color.White);
			AddWidget(mExtraMessageLabel4);
			((Bej3Button)mYesButton).SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public new void Draw(Graphics g)
		{
			base.Draw(g);
		}

		public new int GetPreferredHeight(int theWidth)
		{
			return ConstantsWP.INTRO_DIALOG_HEIGHT;
		}

		public new void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
		}

		public new void Kill()
		{
			GlobalMembers.gApp.mProfile.mHasSeenIntro = true;
			base.Kill();
		}
	}
}
