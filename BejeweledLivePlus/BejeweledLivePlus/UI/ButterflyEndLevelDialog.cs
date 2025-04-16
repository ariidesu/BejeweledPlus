using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.UI
{
	public class ButterflyEndLevelDialog : EndLevelDialog
	{
		public ButterflyBoard mButterflyBoard;

		public ButterflyEndLevelDialog(ButterflyBoard theBoard)
			: base(theBoard)
		{
			mButterflyBoard = theBoard;
			NudgeButtons(GlobalMembers.MS(-40));
			mRankBar.mY += GlobalMembers.MS(30);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void DrawStatsLabels(Graphics g)
		{
			string theString = GlobalMembers._ID("Butterflies Freed", 159);
			int theX = GlobalMembers.MS(230);
			int theY = GlobalMembers.MS(475);
			GlobalMembers.MS(48);
			g.WriteString(theString, theX, theY, -1, -1);
			g.WriteString(GlobalMembers._ID("Best Move", 160), GlobalMembers.MS(230), GlobalMembers.MS(475) + GlobalMembers.MS(48), -1, -1);
			g.WriteString(GlobalMembers._ID("Best Butterfly Combo", 161), GlobalMembers.MS(230), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 2, -1, -1);
			g.WriteString(GlobalMembers._ID("Total Time", 162), GlobalMembers.MS(230), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 3, -1, -1);
		}

		public override void DrawStatsText(Graphics g)
		{
			string theString = Common.CommaSeperate(mGameStats[28]);
			int theX = GlobalMembers.MS(750);
			int theY = GlobalMembers.MS(475);
			GlobalMembers.MS(48);
			g.WriteString(theString, theX, theY, -1, 1);
			g.WriteString(Common.CommaSeperate(mGameStats[25]), GlobalMembers.MS(750), GlobalMembers.MS(475) + GlobalMembers.MS(48), -1, 1);
			g.WriteString(Common.CommaSeperate(mGameStats[29]), GlobalMembers.MS(750), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 2, -1, 1);
			int num = mGameStats[0];
			g.WriteString(string.Format(GlobalMembers._ID("{0}:{1:d2}", 163), num / 60, num % 60), GlobalMembers.MS(750), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 3, -1, 1);
		}

		public override void DrawFrames(Graphics g)
		{
			g.Translate(GlobalMembers.MS(0), GlobalMembers.MS(60));
			DrawLabeledStatsFrame(g);
			DrawLabeledHighScores(g);
			g.Translate(GlobalMembers.MS(0), GlobalMembers.MS(-50));
			DrawSpecialGemDisplay(g);
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
		}
	}
}
