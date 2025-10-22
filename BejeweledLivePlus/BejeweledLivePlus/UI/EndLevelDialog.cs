using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class EndLevelDialog : Bej3Dialog
	{
		public enum EWidgetId
		{
			eId_MainMenu,
			eId_ChooseQuest,
			eId_PlayAgain,
			eId_Badges
		}

		public Board mBoard;

		public Dictionary<int, DialogButton> mBtns = new Dictionary<int, DialogButton>();

		public List<HighScoreEntryLive> mHighScores = new List<HighScoreEntryLive>();

		public CurvedVal mCountupPct = new CurvedVal();

		public RankBarWidget mRankBar;

		public int mPoints;

		public int[] mGameStats = new int[40];

		public List<List<int>> mPointsBreakdown;

		public int mLevel;

		public int mPointMultiplier;

		public EndLevelDialog(Board theBoard)
			: base(38, true, "", "", "", 0, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
		{
			mBoard = theBoard;
			mPointsBreakdown = mBoard.mPointsBreakdown;
			for (int i = 0; i < 40; i++)
			{
				mGameStats[i] = mBoard.mGameStats[i];
			}
			mPoints = mBoard.mPoints;
			mLevel = mBoard.mLevel;
			mPointMultiplier = mBoard.mPointMultiplier;
			mCountupPct.SetCurve(GlobalMembers.MP("b+0,1,0.016667,1,####  M#1^;       S~TEC"));
			Resize(GlobalMembers.MS(0), GlobalMembers.MS(0), GlobalMembers.MS(1600), GlobalMembers.MS(1200));
			mContentInsets.mBottom = GlobalMembers.MS(60);
			mFlushPriority = 100;
			mAllowDrag = false;
			mRankBar = new RankBarWidget(1195, mBoard);
			mRankBar.Move(GlobalMembers.MS(200), GlobalMembers.MS(240));
			AddWidget(mRankBar);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, false);
			base.Dispose();
		}

		public override void ButtonDepress(int theId)
		{
			base.ButtonDepress(theId);
			switch (theId)
			{
			case 0:
			{
				Board board2 = GlobalMembers.gApp.mBoard;
				if (board2 != null)
				{
					Kill();
					board2.BackToMenu();
				}
				break;
			}
			case 1:
			{
				Board board3 = GlobalMembers.gApp.mBoard;
				if (board3 != null && board3.mBoardType == EBoardType.eBoardType_Quest)
				{
					Kill();
				}
				break;
			}
			case 2:
			{
				Board board = GlobalMembers.gApp.mBoard;
				if (board != null)
				{
					Kill();
					board.Init();
					board.NewGame(true);
					mWidgetManager.SetFocus(board);
				}
				break;
			}
			}
		}

		public override void Update()
		{
			if (GlobalMembers.gApp.GetDialog(39) == null)
			{
				base.Update();
				mCountupPct.IncInVal();
			}
		}

		public override void Draw(Graphics g)
		{
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), "Main", new Color(255, 255, 255, 255));
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), "OUTLINE", new Color(255, 255, 255, 255));
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), "GLOW", new Color(255, 255, 255, 255));
			g.DrawImageBox(new Rect(GlobalMembers.MS(110), GlobalMembers.MS(0), GlobalMembers.MS(1380), GlobalMembers.MS(1200)), GlobalMembersResourcesWP.IMAGE_GAMEOVER_DIALOG);
			g.SetColor(new Color(-1));
			g.SetFont(GlobalMembersResources.FONT_HEADER);
			((ImageFont)g.GetFont()).PushLayerColor("Main", new Color(GlobalMembers.M(8931352)));
			((ImageFont)g.GetFont()).PushLayerColor("LAYER_2", new Color(GlobalMembers.M(15253648)));
			((ImageFont)g.GetFont()).PushLayerColor("LAYER_3", new Color(0, 0, 0, 0));
			g.WriteString(GlobalMembers._ID("Final Score:", 236), GlobalMembers.MS(800), GlobalMembers.MS(140));
			((ImageFont)g.GetFont()).PopLayerColor("Main");
			((ImageFont)g.GetFont()).PopLayerColor("LAYER_2");
			((ImageFont)g.GetFont()).PopLayerColor("LAYER_3");
			((ImageFont)g.GetFont()).PushLayerColor("Main", new Color(GlobalMembers.M(16777215)));
			((ImageFont)g.GetFont()).PushLayerColor("LAYER_2", new Color(GlobalMembers.M(11558960)));
			((ImageFont)g.GetFont()).PushLayerColor("LAYER_3", new Color(0, 0, 0, 0));
			g.WriteString(SexyFramework.Common.CommaSeperate((int)((double)mPoints * (double)mCountupPct)), GlobalMembers.MS(800), GlobalMembers.MS(220));
			((ImageFont)g.GetFont()).PopLayerColor("Main");
			((ImageFont)g.GetFont()).PopLayerColor("LAYER_2");
			((ImageFont)g.GetFont()).PopLayerColor("LAYER_3");
			g.SetColor(new Color(-1));
			DrawFrames(g);
		}

		public new void KeyDown(KeyCode theKey)
		{
			switch (theKey)
			{
			case KeyCode.KEYCODE_ESCAPE:
				ButtonDepress(0);
				break;
			case KeyCode.KEYCODE_SPACE:
				ButtonDepress(0);
				break;
			}
		}

		public void SetQuestName(string theQuest)
		{
		}

		public void NudgeButtons(int theOffset)
		{
			Dictionary<int, DialogButton>.Enumerator enumerator = mBtns.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.mY += theOffset;
			}
		}

		public virtual void DrawSpecialGemDisplay(Graphics g)
		{
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			((ImageFont)GlobalMembersResources.FONT_DIALOG).PushLayerColor("GLOW", new Color(0, 0, 0, 0));
			((ImageFont)GlobalMembersResources.FONT_DIALOG).PushLayerColor("OUTLINE", new Color(0, 0, 0, 0));
			g.SetColor(new Color(GlobalMembers.M(16777215)));
			g.WriteString(string.Format(GlobalMembers._ID("x {0}", 230), mGameStats[17]), GlobalMembers.MS(400), GlobalMembers.MS(900), -1, -1);
			g.WriteString(string.Format(GlobalMembers._ID("x {0}", 231), mGameStats[18]), GlobalMembers.MS(780), GlobalMembers.MS(900), -1, -1);
			g.WriteString(string.Format(GlobalMembers._ID("x {0}", 232), mGameStats[19]), GlobalMembers.MS(1150), GlobalMembers.MS(900), -1, -1);
			((ImageFont)GlobalMembersResources.FONT_DIALOG).PopLayerColor("OUTLINE");
			((ImageFont)GlobalMembersResources.FONT_DIALOG).PopLayerColor("GLOW");
		}

		private string Unkern(string theString)
		{
			char[] value = new char[theString.Length * 2];
			string text = new string(value);
			for (int i = 0; i < theString.Length; i++)
			{
				text = text + theString[i] + "~";
			}
			return text;
		}

		public virtual void DrawHighScores(Graphics g)
		{
		}

		public virtual void DrawStatsLabels(Graphics g)
		{
		}

		public virtual void DrawStatsText(Graphics g)
		{
		}

		public virtual void DrawLabeledHighScores(Graphics g)
		{
			g.DrawImageBox(new Rect(GlobalMembers.MS(800), GlobalMembers.MS(385) - GlobalMembers.MS(0), GlobalMembers.MS(480), GlobalMembersResourcesWP.IMAGE_GAMEOVER_SECTION_LABEL.GetHeight()), GlobalMembersResourcesWP.IMAGE_GAMEOVER_SECTION_LABEL);
			g.SetColor(new Color(-1));
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			((ImageFont)g.GetFont()).PushLayerColor("Main", new Color(GlobalMembers.M(8931352)));
			((ImageFont)g.GetFont()).PushLayerColor("Outline", new Color(GlobalMembers.M(16777215)));
			((ImageFont)g.GetFont()).PushLayerColor("Glow", new Color(0, 0, 0, 0));
			g.WriteString(GlobalMembers._ID("Top Scores:", 234), GlobalMembers.MS(1085), GlobalMembers.MS(435));
			((ImageFont)g.GetFont()).PopLayerColor("Main");
			((ImageFont)g.GetFont()).PopLayerColor("Outline");
			((ImageFont)g.GetFont()).PopLayerColor("Glow");
			g.PushState();
			g.Translate(GlobalMembers.MS(0), GlobalMembers.MS(60));
			DrawHighScores(g);
			g.PopState();
		}

		public virtual void DrawStatsFrame(Graphics g)
		{
			g.DrawImageBox(new Rect(GlobalMembers.MS(195), GlobalMembers.MS(385), GlobalMembers.MS(602), GlobalMembers.MS(282)), GlobalMembersResourcesWP.IMAGE_GAMEOVER_LIGHT_BOX);
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			((ImageFont)g.GetFont()).PushLayerColor("Main", new Color(255, 255, 255, 255));
			((ImageFont)g.GetFont()).PushLayerColor("OUTLINE", new Color(GlobalMembers.M(4210688)));
			((ImageFont)g.GetFont()).PushLayerColor("GLOW", new Color(0, 0, 0, 0));
			g.SetColor(new Color(GlobalMembers.M(16053456)));
			DrawStatsLabels(g);
			g.SetColor(new Color(GlobalMembers.M(16777056)));
			DrawStatsText(g);
			((ImageFont)g.GetFont()).PopLayerColor("Main");
			((ImageFont)g.GetFont()).PopLayerColor("OUTLINE");
			((ImageFont)g.GetFont()).PopLayerColor("GLOW");
		}

		public virtual void DrawLabeledStatsFrame(Graphics g)
		{
			g.DrawImageBox(new Rect(GlobalMembers.MS(195), GlobalMembers.MS(385) - GlobalMembers.MS(0), GlobalMembers.MS(600), GlobalMembersResourcesWP.IMAGE_GAMEOVER_SECTION_LABEL.GetHeight()), GlobalMembersResourcesWP.IMAGE_GAMEOVER_SECTION_LABEL);
			g.SetColor(new Color(-1));
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			((ImageFont)g.GetFont()).PushLayerColor("Main", new Color(GlobalMembers.M(8931352)));
			((ImageFont)g.GetFont()).PushLayerColor("OUTLINE", new Color(GlobalMembers.M(16777215)));
			((ImageFont)g.GetFont()).PushLayerColor("GLOW", new Color(0, 0, 0, 0));
			g.WriteString(GlobalMembers._ID("Statistics", 235), GlobalMembers.MS(485), GlobalMembers.MS(435));
			((ImageFont)g.GetFont()).PopLayerColor("Main");
			((ImageFont)g.GetFont()).PopLayerColor("OUTLINE");
			((ImageFont)g.GetFont()).PopLayerColor("GLOW");
			g.PushState();
			g.Translate(GlobalMembers.MS(0), GlobalMembers.MS(60));
			DrawStatsFrame(g);
			g.PopState();
		}

		public virtual void DrawFrames(Graphics g)
		{
			DrawStatsFrame(g);
			DrawHighScores(g);
		}

		public void OptionConfirmed()
		{
		}
	}
}
