using System;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class GameDetailMenuContainer : Bej3Widget
	{
		private enum BUTTON_IDS
		{
			BTN_GAMECENTER_ID
		}

		public const int STAT_TYPE_MAX = 4;

		private GameMode mMode;

		private Label[] mStatsHeadingLabels = new Label[4];

		private Label[] mStatsLabels = new Label[4];

		private Label mSpecialGemsHeadingLabel;

		private RankBarWidget mRankBarWidget1;

		public HighScoresWidget mHighScoresWidgetPostGame;

		private string[] mSpecialStatsStrings = new string[3];

		private Label mStatsHeaderLabel;

		private Bej3Button mGameCenterButton;

		public bool mScrollLocked;

		public GameDetailMenuContainer()
			: base(Menu_Type.MENU_GAMEDETAILMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mScrollLocked = false;
			mDoesSlideInFromBottom = false;
			int gAMEDETAILMENU_POST_GAME_TAB_WIDTH = ConstantsWP.GAMEDETAILMENU_POST_GAME_TAB_WIDTH;
			Resize(ConstantsWP.GAMEDETAILMENU_POST_GAME_CONTAINER_X, ConstantsWP.GAMEDETAILMENU_POST_GAME_CONTAINER_Y, (ConstantsWP.GAMEDETAILMENU_POST_GAME_TAB_WIDTH + ConstantsWP.GAMEDETAILMENU_POST_GAME_TAB_PADDING) * 2, ConstantsWP.GAMEDETAILMENU_POST_GAME_CONTAINER_HEIGHT);
			mHighScoresWidgetPostGame = new HighScoresWidget(new Rect(ConstantsWP.GAMEDETAILMENU_HIGHSCORES_POST_GAME_X, ConstantsWP.GAMEDETAILMENU_HIGHSCORES_POST_GAME_Y, ConstantsWP.GAMEDETAILMENU_HIGHSCORES_POST_GAME_WIDTH, ConstantsWP.GAMEDETAILMENU_HIGHSCORES_POST_GAME_HEIGHT), false, ConstantsWP.GAMEDETAILMENU_POST_GAME_SCROLLWIDGET_CORRECTION);
			AddWidget(mHighScoresWidgetPostGame);
			int theWidth;
			int theX;
			if (GlobalMembers.gApp.mGameCenterIsAvailable)
			{
				theWidth = ConstantsWP.GAMEDETAILMENU_POST_GAME_RANKBAR_1_WIDTH_INCL_GC;
				theX = ConstantsWP.GAMEDETAILMENU_POST_GAME_RANKBAR_1_X_INCL_GC;
			}
			else
			{
				theWidth = ConstantsWP.GAMEDETAILMENU_POST_GAME_RANKBAR_1_WIDTH;
				theX = ConstantsWP.GAMEDETAILMENU_POST_GAME_RANKBAR_1_X;
			}
			mRankBarWidget1 = new RankBarWidget(theWidth);
			mRankBarWidget1.Resize(theX, ConstantsWP.GAMEDETAILMENU_POST_GAME_RANKBAR_1_Y, theWidth, 100);
			mRankBarWidget1.mDrawCrown = true;
			mRankBarWidget1.mTobleroning = true;
			mRankBarWidget1.mDrawText = true;
			AddWidget(mRankBarWidget1);
			if (GlobalMembers.gApp.mGameCenterIsAvailable)
			{
				mGameCenterButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_GAMECENTER);
				Bej3Widget.CenterWidgetAt(ConstantsWP.GAMEDETAILMENU_GAMECENTER_X, ConstantsWP.GAMEDETAILMENU_GAMECENTER_Y, mGameCenterButton);
				AddWidget(mGameCenterButton);
			}
			else
			{
				mGameCenterButton = null;
			}
			mSpecialGemsHeadingLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, string.Empty);
			mSpecialGemsHeadingLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_2_STROKE);
			mSpecialGemsHeadingLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_2_FILL);
			mSpecialGemsHeadingLabel.Resize(ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_HEADING_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_HEADING_Y, 0, 0);
			AddWidget(mSpecialGemsHeadingLabel);
			int gAMEDETAILMENU_POST_GAME_STATS_START_Y = ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_START_Y;
			mStatsHeaderLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("STATS", 3296), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE);
			mStatsHeaderLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_1_STROKE);
			mStatsHeaderLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_1_FILL);
			mStatsHeaderLabel.Resize(gAMEDETAILMENU_POST_GAME_TAB_WIDTH * 3 / 2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(mStatsHeaderLabel);
			int theX2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y += ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y_BIG;
			Label label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsHeadingLabels[0] = label;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y += ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y;
			label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsHeadingLabels[1] = label;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y += ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y;
			label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsHeadingLabels[2] = label;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y += ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y;
			label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsHeadingLabels[3] = label;
			theX2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_DATA_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y = ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_START_Y + ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y_BIG;
			label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsLabels[0] = label;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y += ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y;
			label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsLabels[1] = label;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y += ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y;
			label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsLabels[2] = label;
			gAMEDETAILMENU_POST_GAME_STATS_START_Y += ConstantsWP.GAMEDETAILMENU_POST_GAME_STATS_STEP_Y;
			label = new Label(GlobalMembersResources.FONT_DIALOG, string.Empty, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
			label.Resize(theX2, gAMEDETAILMENU_POST_GAME_STATS_START_Y, 0, 0);
			AddWidget(label);
			mStatsLabels[3] = label;
			for (int i = 0; i < 4; i++)
			{
				mStatsHeadingLabels[i].SetLayerColor(0, Bej3Widget.COLOR_DIALOG_WHITE);
				mStatsLabels[i].SetLayerColor(0, Bej3Widget.COLOR_DIALOG_4_FILL);
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public void SetMode(GameMode mode, GameDetailMenu.GAMEDETAILMENU_STATE state)
		{
			mMode = mode;
			mHighScoresWidgetPostGame.SetMode(mode);
			mHighScoresWidgetPostGame.SetHeading(GlobalMembers.gApp.GetModeHeading(mode));
			switch (mode)
			{
			case GameMode.MODE_CLASSIC:
				mStatsHeadingLabels[0].SetText(GlobalMembers._ID("Level Achieved:", 3297));
				mStatsHeadingLabels[1].SetText(GlobalMembers._ID("Best Move:", 3298));
				mStatsHeadingLabels[2].SetText(GlobalMembers._ID("Longest Cascade:", 3299));
				mStatsHeadingLabels[3].SetText(GlobalMembers._ID("Total Time:", 3300));
				mSpecialGemsHeadingLabel.SetText(GlobalMembers._ID("SPECIAL GEMS", 3301));
				break;
			case GameMode.MODE_DIAMOND_MINE:
				mStatsHeadingLabels[0].SetText(GlobalMembers._ID("Max Depth:", 3302));
				mStatsHeadingLabels[1].SetText(GlobalMembers._ID("Total Time:", 3303));
				mStatsHeadingLabels[2].SetText(GlobalMembers._ID("Best Move:", 3304));
				mStatsHeadingLabels[3].SetText(GlobalMembers._ID("Best Treasure:", 3305));
				mSpecialGemsHeadingLabel.SetText(GlobalMembers._ID("TREASURE FOUND", 3306));
				break;
			case GameMode.MODE_BUTTERFLY:
				mStatsHeadingLabels[0].SetText(GlobalMembers._ID("Butterflies Freed:", 3307));
				mStatsHeadingLabels[1].SetText(GlobalMembers._ID("Best Move:", 3308));
				mStatsHeadingLabels[2].SetText(GlobalMembers._ID("Best Butterfly Combo:", 3309));
				mStatsHeadingLabels[3].SetText(GlobalMembers._ID("Total Time:", 3310));
				mSpecialGemsHeadingLabel.SetText(GlobalMembers._ID("SPECIAL GEMS", 3311));
				break;
			case GameMode.MODE_POKER:
				mStatsHeadingLabels[0].SetText(GlobalMembers._ID("Best Hand:", 3312));
				mStatsHeadingLabels[1].SetText(GlobalMembers._ID("Number of Hands:", 3313));
				mStatsHeadingLabels[2].SetText(GlobalMembers._ID("Skulls Busted:", 3314));
				mStatsHeadingLabels[3].SetText(GlobalMembers._ID("Skull Coin Flips:", 3315));
				mSpecialGemsHeadingLabel.SetText(GlobalMembers._ID("SPECIAL GEMS", 3316));
				break;
			case GameMode.MODE_LIGHTNING:
				mStatsHeadingLabels[0].SetText(GlobalMembers._ID("Highest Multiplier:", 3317));
				mStatsHeadingLabels[1].SetText(GlobalMembers._ID("Best move:", 3318));
				mStatsHeadingLabels[2].SetText(GlobalMembers._ID("Longest Cascade:", 3319));
				mStatsHeadingLabels[3].SetText(GlobalMembers._ID("Total time:", 3320));
				mSpecialGemsHeadingLabel.SetText(GlobalMembers._ID("SPECIAL GEMS", 3301));
				break;
			case GameMode.MODE_ZEN:
			case GameMode.MODE_ICESTORM:
				break;
			}
		}

		public override void Update()
		{
			base.Update();
			mScrollLocked = mHighScoresWidgetPostGame.mScrollLocked;
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			int gAMEDETAILMENU_POST_GAME_TAB_WIDTH = ConstantsWP.GAMEDETAILMENU_POST_GAME_TAB_WIDTH;
			Bej3Widget.DrawInlayBox(g, new Rect(ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_Y, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_HEIGHT), mSpecialGemsHeadingLabel.GetTextWidth());
			Bej3Widget.DrawInlayBoxShadow(g, new Rect(ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_Y, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_HEIGHT));
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, Color.White);
			int num = 0;
			num = ConstantsWP.GAMEDETAILMENU_POST_GAME_TAB_WIDTH;
			Bej3Widget.DrawDividerCentered(g, ConstantsWP.GAMEDETAILMENU_POST_GAME_TAB_WIDTH / 2 + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_DIVIDER_Y);
			switch (mMode)
			{
			case GameMode.MODE_CLASSIC:
			case GameMode.MODE_LIGHTNING:
			case GameMode.MODE_BUTTERFLY:
			case GameMode.MODE_POKER:
			{
				Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_DIALOG_ICON_FLAME_LRG, 0, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_1_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_FLAME_GEM_POS_Y);
				Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_DIALOG_ICON_STAR_LRG, 0, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_2_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_STAR_GEM_POS_Y);
				Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_DIALOG_ICON_HYPERCUBE_LRG, 0, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_3_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_HYPERCUBE_POS_Y);
				int theX2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_1_X + num + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_X_1;
				int theY2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_Y;
				g.DrawString(mSpecialStatsStrings[0], theX2, theY2);
				theX2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_2_X + num + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_X_2;
				theY2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_Y;
				g.DrawString(mSpecialStatsStrings[1], theX2, theY2);
				theX2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_3_X + num + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_X_3;
				theY2 = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_Y;
				g.DrawString(mSpecialStatsStrings[2], theX2, theY2);
				break;
			}
			case GameMode.MODE_DIAMOND_MINE:
			{
				Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_DIALOG_MINE_TILES_GOLD, 0, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_1_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y);
				Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_DIALOG_MINE_TILES_GEM, 0, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_2_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y);
				Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_DIALOG_MINE_TILES_TREASURE, 0, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_3_X + gAMEDETAILMENU_POST_GAME_TAB_WIDTH, ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y);
				int theX = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_1_X + num + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_X_1;
				int theY = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_Y;
				g.DrawString(mSpecialStatsStrings[0], theX, theY);
				theX = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_2_X + num + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_X_1;
				theY = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_Y;
				g.DrawString(mSpecialStatsStrings[1], theX, theY);
				theX = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_POS_3_X + num + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_X_1;
				theY = ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_DIAMOND_MINE_POS_Y + ConstantsWP.GAMEDETAILMENU_POST_GAME_INLAY_1_GEM_TEXT_OFFSET_Y;
				g.DrawString(mSpecialStatsStrings[2], theX, theY);
				break;
			}
			}
			g.PopState();
		}

		public override void Show()
		{
			base.Show();
			LinkUpAssets();
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
		}

		public override void Hide()
		{
			base.Hide();
		}

		public override void HideCompleted()
		{
			mRankBarWidget1.Shown(null);
			base.HideCompleted();
			mHighScoresWidgetPostGame.UnNewScore();
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
		}

		public void GetStatsFromBoard(Board theBoard)
		{
			if (theBoard == null)
			{
				return;
			}
			switch (mMode)
			{
			case GameMode.MODE_CLASSIC:
				mStatsLabels[0].SetText(SexyFramework.Common.CommaSeperate(theBoard.mLevel + 1));
				mStatsLabels[1].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[25]));
				mStatsLabels[2].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[24]));
				mStatsLabels[3].SetText(Utils.GetTimeString(theBoard.mGameStats[0]));
				mSpecialStatsStrings[0] = string.Format(GlobalMembers._ID("x {0}", 3321), theBoard.mGameStats[17]);
				mSpecialStatsStrings[1] = string.Format(GlobalMembers._ID("x {0}", 3322), theBoard.mGameStats[18]);
				mSpecialStatsStrings[2] = string.Format(GlobalMembers._ID("x {0}", 3323), theBoard.mGameStats[19]);
				break;
			case GameMode.MODE_DIAMOND_MINE:
			{
				DigBoard digBoard = (DigBoard)theBoard;
				DigGoal digGoal = (DigGoal)digBoard.mQuestGoal;
				int aTime = digBoard.mGameStats[0];
				mStatsLabels[0].SetText(string.Format(GlobalMembers._ID("{0} m", 3324), SexyFramework.Common.CommaSeperate(digGoal.GetGridDepth() * 10)));
				mStatsLabels[1].SetText(Utils.GetTimeString(aTime));
				mStatsLabels[2].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[25]));
				int num = 0;
				for (int i = 0; i < digGoal.mCollectedArtifacts.Count; i++)
				{
					DigGoal.ArtifactData artifactData = digGoal.mArtifacts[digGoal.mCollectedArtifacts[i]];
					if (digGoal.mArtifactBaseValue * artifactData.mValue >= num)
					{
						num = digGoal.mArtifactBaseValue * artifactData.mValue;
					}
				}
				mStatsLabels[3].SetText($"{SexyFramework.Common.CommaSeperate(num)}");
				int num2 = 0;
				int num3 = 0;
				for (int j = 0; j < 3; j++)
				{
					num2 = GlobalMembers.MAX(digGoal.mTreasureEarnings[j], num2);
					num3 += digGoal.mTreasureEarnings[j];
				}
				if (num2 > 0)
				{
					int num4 = (int)((double)digGoal.mTreasureEarnings[0] * 100.0 / (double)num3 + 0.5);
					int num5 = (int)((double)digGoal.mTreasureEarnings[1] * 100.0 / (double)num3 + 0.5);
					int num6 = (int)((double)digGoal.mTreasureEarnings[2] * 100.0 / (double)num3 + 0.5);
					int num7 = num4 + num5 + num6;
					if (num7 != 100)
					{
						num4 -= num7 - 100;
					}
					mSpecialStatsStrings[0] = string.Format(GlobalMembers._ID("{0}%", 3490), num4);
					mSpecialStatsStrings[1] = string.Format(GlobalMembers._ID("{0}%", 3491), num5);
					mSpecialStatsStrings[2] = string.Format(GlobalMembers._ID("{0}%", 3492), num6);
				}
				else
				{
					mSpecialStatsStrings[0] = string.Format(GlobalMembers._ID("{0}%", 3493), 0);
					mSpecialStatsStrings[1] = string.Format(GlobalMembers._ID("{0}%", 3494), 0);
					mSpecialStatsStrings[2] = string.Format(GlobalMembers._ID("{0}%", 3495), 0);
				}
				break;
			}
			case GameMode.MODE_BUTTERFLY:
				mStatsLabels[0].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[28]));
				mStatsLabels[1].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[25]));
				mStatsLabels[2].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[29]));
				mStatsLabels[3].SetText(Utils.GetTimeString(theBoard.mGameStats[0]));
				mSpecialStatsStrings[0] = string.Format(GlobalMembers._ID("x {0}", 3325), theBoard.mGameStats[17]);
				mSpecialStatsStrings[1] = string.Format(GlobalMembers._ID("x {0}", 3326), theBoard.mGameStats[18]);
				mSpecialStatsStrings[2] = string.Format(GlobalMembers._ID("x {0}", 3327), theBoard.mGameStats[19]);
				break;
			case GameMode.MODE_POKER:
				throw new NotImplementedException();
			case GameMode.MODE_LIGHTNING:
				mStatsLabels[0].SetText(SexyFramework.Common.CommaSeperate(theBoard.mPointMultiplier));
				mStatsLabels[1].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[25]));
				mStatsLabels[2].SetText(SexyFramework.Common.CommaSeperate(theBoard.mGameStats[24]));
				mStatsLabels[3].SetText(Utils.GetTimeString(theBoard.mGameStats[0] - 3));
				mSpecialStatsStrings[0] = string.Format(GlobalMembers._ID("x {0}", 3331), theBoard.mGameStats[17]);
				mSpecialStatsStrings[1] = string.Format(GlobalMembers._ID("x {0}", 3332), theBoard.mGameStats[18]);
				mSpecialStatsStrings[2] = string.Format(GlobalMembers._ID("x {0}", 3333), theBoard.mGameStats[19]);
				break;
			}
			mRankBarWidget1.Shown(theBoard);
		}

		public override void ButtonDepress(int theId)
		{
			if (theId == 0)
			{
				switch (mMode)
				{
				case GameMode.MODE_CLASSIC:
					throw new NotImplementedException();
				case GameMode.MODE_DIAMOND_MINE:
					throw new NotImplementedException();
				case GameMode.MODE_BUTTERFLY:
					throw new NotImplementedException();
				case GameMode.MODE_LIGHTNING:
				case GameMode.MODE_ZEN:
					break;
				}
			}
		}
	}
}
