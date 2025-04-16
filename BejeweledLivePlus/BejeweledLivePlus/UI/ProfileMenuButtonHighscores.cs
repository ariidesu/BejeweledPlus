using System.Collections.Generic;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class ProfileMenuButtonHighscores : ProfileMenuButton
	{
		private enum ProfileMenuScores
		{
			SCORES_CLASSIC,
			SCORES_DIAMOND_MINE,
			SCORES_MAX
		}

		private int mCurrentScore;

		private Label[] mHighscores = new Label[2];

		private Label[] mHighscoresToday = new Label[2];

		private Label[] mHighscoresHeading = new Label[2];

		private Label[] mHighscoresHeadingToday = new Label[2];

		private List<int> mRelevantHighscores = new List<int>();

		private Label mNoHighscoreMessageLabel;

		public ProfileMenuButtonHighscores(Bej3ButtonListener theListener)
			: base(6, theListener, GlobalMembers._ID("TOP SCORES", 3433))
		{
			mCurrentScore = -1;
			for (int i = 0; i < 2; i++)
			{
				mHighscoresHeading[i] = new Label(GlobalMembersResources.FONT_DIALOG, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
				mHighscoresHeading[i].Resize(ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_X_1, ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_Y_1, 0, 0);
				AddWidget(mHighscoresHeading[i]);
				mHighscores[i] = new Label(GlobalMembersResources.FONT_DIALOG, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
				mHighscores[i].Resize(ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_X_2, ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_Y_1, 0, 0);
				mHighscores[i].SetLayerColor(0, Bej3Widget.COLOR_DIALOG_4_FILL);
				AddWidget(mHighscores[i]);
				mHighscoresHeadingToday[i] = new Label(GlobalMembersResources.FONT_DIALOG, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
				mHighscoresHeadingToday[i].Resize(ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_X_1, ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_Y_2, 0, 0);
				mHighscoresHeadingToday[i].SetText(GlobalMembers._ID("", 2050 + i));
				AddWidget(mHighscoresHeadingToday[i]);
				mHighscoresToday[i] = new Label(GlobalMembersResources.FONT_DIALOG, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
				mHighscoresToday[i].Resize(ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_X_2, ConstantsWP.PROFILEMENU_BUTTON_MESSAGE_Y_2, 0, 0);
				mHighscoresToday[i].SetLayerColor(0, Bej3Widget.COLOR_DIALOG_4_FILL);
				AddWidget(mHighscoresToday[i]);
			}
			MakeChildrenTouchInvisible();
			SetNextHighScore();
		}

		public override void Update()
		{
			base.Update();
		}

		public void SetNextHighScore()
		{
			int num = mCurrentScore;
			mCurrentScore = (mCurrentScore + 1) % 2;
			if (mRelevantHighscores.Count == 0)
			{
				mNoHighscoreMessageLabel.FadeIn();
				num = (mCurrentScore = -1);
			}
			else
			{
				mNoHighscoreMessageLabel.FadeOut();
				mCurrentScore = (mCurrentScore + 1) % mRelevantHighscores.Count;
			}
			for (int i = 0; i < 2; i++)
			{
				if (i == mCurrentScore)
				{
					mHighscores[i].FadeIn();
					mHighscoresHeading[i].FadeIn();
					mHighscoresToday[i].FadeIn();
					mHighscoresHeadingToday[i].FadeIn();
					continue;
				}
				if (i == num)
				{
					mHighscores[i].FadeOut();
					mHighscoresHeading[i].FadeOut();
					mHighscoresToday[i].FadeOut();
					mHighscoresHeadingToday[i].FadeOut();
					continue;
				}
				mHighscores[i].FadeOut();
				mHighscoresHeading[i].FadeOut();
				mHighscoresToday[i].FadeOut();
				mHighscoresHeadingToday[i].FadeOut();
				mHighscores[i].mAlpha = 0f;
				mHighscoresHeading[i].mAlpha = 0f;
				mHighscoresToday[i].mAlpha = 0f;
				mHighscoresHeadingToday[i].mAlpha = 0f;
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mNoHighscoreMessageLabel.SetTextBlock(new Rect(ConstantsWP.PROFILEMENU_BUTTON_EXTRA_MESSAGE_X, ConstantsWP.PROFILEMENU_BUTTON_EXTRA_MESSAGE_Y, ConstantsWP.PROFILEMENU_BUTTON_EXTRA_MESSAGE_WIDTH, 0), true);
			mHighscores[0].SetText(Common.CommaSeperate(GlobalMembers.gApp.mProfile.GetModeHighScore(GameMode.MODE_CLASSIC)));
			mHighscores[1].SetText(Common.CommaSeperate(GlobalMembers.gApp.mProfile.GetModeHighScore(GameMode.MODE_DIAMOND_MINE)));
			mHighscoresToday[0].SetText(Common.CommaSeperate(GlobalMembers.gApp.mProfile.GetModeHighScoreToday(GameMode.MODE_CLASSIC)));
			mHighscoresToday[1].SetText(Common.CommaSeperate(GlobalMembers.gApp.mProfile.GetModeHighScoreToday(GameMode.MODE_DIAMOND_MINE)));
			mRelevantHighscores.Clear();
			if (GlobalMembers.gApp.mProfile.GetModeHighScore(GameMode.MODE_CLASSIC) > 0)
			{
				mRelevantHighscores.Add(0);
			}
			if (GlobalMembers.gApp.mProfile.GetModeHighScore(GameMode.MODE_DIAMOND_MINE) > 0)
			{
				mRelevantHighscores.Add(1);
			}
			if (mRelevantHighscores.Count > 0)
			{
				mNoHighscoreMessageLabel.mAlpha = 0f;
			}
		}
	}
}
