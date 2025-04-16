using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.UI
{
	internal class ClassicEndLevelDialog : EndLevelDialog
	{
		public ClassicBoard mClassicBoard;

		public ClassicEndLevelDialog(ClassicBoard theBoard)
			: base(theBoard)
		{
			mClassicBoard = theBoard;
			NudgeButtons(GlobalMembers.MS(-40));
			mRankBar.mY += GlobalMembers.MS(30);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void DrawStatsLabels(Graphics g)
		{
			string theString = GlobalMembers._ID("Level Achieved", 165);
			int theX = GlobalMembers.MS(230);
			int theY = GlobalMembers.MS(475);
			GlobalMembers.MS(48);
			g.WriteString(theString, theX, theY, -1, -1);
			g.WriteString(GlobalMembers._ID("Best Move", 166), GlobalMembers.MS(230), GlobalMembers.MS(475) + GlobalMembers.MS(48), -1, -1);
			g.WriteString(GlobalMembers._ID("Longest Cascade", 167), GlobalMembers.MS(230), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 2, -1, -1);
			g.WriteString(GlobalMembers._ID("Total Time", 168), GlobalMembers.MS(230), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 3, -1, -1);
		}

		public override void DrawStatsText(Graphics g)
		{
			string theString = Common.CommaSeperate(mLevel + 1);
			int theX = GlobalMembers.MS(760);
			int theY = GlobalMembers.MS(475);
			GlobalMembers.MS(48);
			g.WriteString(theString, theX, theY, -1, 1);
			g.WriteString(Common.CommaSeperate(mGameStats[25]), GlobalMembers.MS(760), GlobalMembers.MS(475) + GlobalMembers.MS(48), -1, 1);
			g.WriteString(Common.CommaSeperate(mGameStats[24]), GlobalMembers.MS(760), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 2, -1, 1);
			int num = mGameStats[0];
			g.WriteString(string.Format(GlobalMembers._ID("{0}:{1:d2}", 169), num / 60, num % 60), GlobalMembers.MS(760), GlobalMembers.MS(475) + GlobalMembers.MS(48) * 3, -1, 1);
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
