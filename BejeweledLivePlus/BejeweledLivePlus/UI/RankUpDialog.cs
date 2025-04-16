using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class RankUpDialog : Bej3Dialog
	{
		public Board mBoard;

		public RankBarWidget mRankBarWidget;

		public CurvedVal mRankUpAnimPct = new CurvedVal();

		public Label mMessageLabel2;

		public RankUpDialog(Board theBoard)
			: base(43, true, GlobalMembers._ID("RANK UP", 429), "", "", 3, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
		{
			mRankUpAnimPct.SetConstant(0.0);
			mBoard = theBoard;
			mContentInsets.mTop = GlobalMembers.MS(17);
			mRankBarWidget = null;
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, GlobalMembers.gApp.mWidth, ConstantsWP.RANKUPDIALOG_HEIGHT);
			mYesButton.mLabel = GlobalMembers._ID("CONTINUE", 3069);
			mYesButton.mX = ConstantsWP.RANKUPDIALOG_BUTTON_OK_X;
			mYesButton.mY = ConstantsWP.RANKUPDIALOG_BUTTON_OK_Y;
			((Bej3Button)mYesButton).SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
			mYesButton = null;
			mRankBarWidget = new RankBarWidget(ConstantsWP.RANKUPDIALOG_RANKBAR_WIDTH, mBoard, this, true);
			mRankBarWidget.Move((mWidth - ConstantsWP.RANKUPDIALOG_RANKBAR_WIDTH) / 2, ConstantsWP.RANKUPDIALOG_RANKBAR_Y);
			mRankBarWidget.mDrawCrown = true;
			mRankBarWidget.mDrawRankName = false;
			AddWidget(mRankBarWidget);
			mRankBarWidget.Shown(mBoard);
			mMessageLabel.SetTextBlock(new Rect(mWidth / 2 - ConstantsWP.RANKUPDIALOG_MSG_1_WIDTH / 2, ConstantsWP.RANKUPDIALOG_MSG_1_Y, ConstantsWP.RANKUPDIALOG_MSG_1_WIDTH, ConstantsWP.RANKUPDIALOG_MSG_1_HEIGHT), false);
			mMessageLabel2 = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("You have been promoted to:", 431));
			mMessageLabel2.SetLayerColor(0, Bej3Widget.COLOR_SCORE);
			mMessageLabel2.SetLayerColor(1, Color.White);
			mMessageLabel2.Resize(mWidth / 2, ConstantsWP.RANKUPDIALOG_MSG_2_Y, 0, 0);
			AddWidget(mMessageLabel2);
			GlobalMembers.gApp.mProfile.UpdateRank(theBoard);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, false);
			base.Dispose();
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			mHeadingLabel.Resize(mWidth / 2, ConstantsWP.DIALOGBOX_HEADING_LABEL_Y - 20, 0, 0);
			mY = ConstantsWP.MENU_Y_POS_HIDDEN;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.SetColor(new Color(255, 255, 255, (int)(255f * mBoard.GetBoardAlpha())));
			g.SetColorizeImages(true);
			int rank = mRankBarWidget.GetRank();
			int num = mWidth / 2;
			int rANKUPDIALOG_MSG_5_Y = ConstantsWP.RANKUPDIALOG_MSG_5_Y;
			CurvedVal curvedVal = new CurvedVal();
			CurvedVal curvedVal2 = new CurvedVal();
			CurvedVal curvedVal3 = new CurvedVal();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eRANK_UP_DIALOG_DRAW_TEXT_SCALE, curvedVal, mRankUpAnimPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eRANK_UP_DIALOG_DRAW_GLOW_SCALE, curvedVal2, mRankUpAnimPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eRANK_UP_DIALOG_DRAW_GLOW_ALPHA, curvedVal3, mRankUpAnimPct);
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			g.SetColor(Color.White);
			g.PushState();
			g.SetScale((float)(double)curvedVal, (float)(double)curvedVal, num, (float)rANKUPDIALOG_MSG_5_Y - (float)ConstantsWP.RANKUPDIALOG_FONT_SCALE_OFFSET);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Bej3Widget.COLOR_SUBHEADING_1_STROKE);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Bej3Widget.COLOR_SUBHEADING_1_FILL);
			g.WriteString(mRankBarWidget.GetRankName(rank, false), num, rANKUPDIALOG_MSG_5_Y);
			g.PopState();
			float num2 = (float)(double)curvedVal2;
			if (num2 > 0f)
			{
				g.PushState();
				g.SetScale(num2, num2, num, (float)rANKUPDIALOG_MSG_5_Y - (float)ConstantsWP.RANKUPDIALOG_FONT_SCALE_OFFSET);
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.SetColor(new Color(255, 255, 255, (int)(255.0 * (double)curvedVal3)));
				g.WriteString(mRankBarWidget.GetRankName(rank, false), num, rANKUPDIALOG_MSG_5_Y);
				g.PopState();
			}
		}

		public override void Update()
		{
			base.Update();
		}

		public override void ButtonDepress(int theId)
		{
			base.ButtonDepress(theId);
			Kill();
		}

		public void DoRankUp()
		{
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eRANK_UP_DIALOG_ANIM_PCT, mRankUpAnimPct);
		}
	}
}
