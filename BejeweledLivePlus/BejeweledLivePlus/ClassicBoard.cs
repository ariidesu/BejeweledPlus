using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus
{
	public class ClassicBoard : Board
	{
		public ClassicBoard()
		{
			mParams["Title"] = "Classic";
		}

		public override void Dispose()
		{
			int mGameOverCount2 = mGameOverCount;
			int num = 0;
			base.Dispose();
		}

		public override bool WantsLevelBasedBackground()
		{
			return true;
		}

		public override void NewGame(bool restartingGame)
		{
			base.NewGame(restartingGame);
		}

		public override bool LoadGame(Serialiser theBuffer)
		{
			return LoadGame(theBuffer, true);
		}

		public override bool LoadGame(Serialiser theBuffer, bool resetReplay)
		{
			if (!base.LoadGame(theBuffer, resetReplay))
			{
				return false;
			}
			return true;
		}

		public override bool AllowSpeedBonus()
		{
			return false;
		}

		public override bool WantAnnihilatorReplacement()
		{
			return true;
		}

		public override void GameOverExit()
		{
			SubmitHighscore();
			GlobalMembers.gApp.DoGameDetailMenu(GameMode.MODE_CLASSIC, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_POST_GAME);
		}

		public override void GameOverAnnounce()
		{
			new Announcement(this, GlobalMembers._ID("NO MORE\nMOVES", 164));
			GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_NOMOREMOVES);
			(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("Play", "Classic_lose", true);
			// GlobalMembers.gApp.mMusic.PlaySongNoDelay(3, false);
		}

		public override void HyperspaceEvent(HYPERSPACEEVENT inEvent)
		{
			base.HyperspaceEvent(inEvent);
			int num = 5;
		}

		public override string GetSavedGameName()
		{
			return "classic.sav";
		}

		public override bool SupportsReplays()
		{
			return true;
		}

		public override float GetModePointMultiplier()
		{
			return 1f + (float)mLevel * 1f;
		}

		public override float GetRankPointMultiplier()
		{
			return 1f;
		}

		public override void LoadContent(bool threaded)
		{
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlay_UI_Normal");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlay_UI_Normal");
			}
			base.LoadContent(threaded);
		}

		public override void UnloadContent()
		{
			BejeweledLivePlusApp.UnloadContent("GamePlay_UI_Normal");
			base.UnloadContent();
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			base.DrawGameElements(g);
		}

		public override void Update()
		{
			CustomBassMusicInterface theMusicInterface =
				(CustomBassMusicInterface)(GlobalMembers.gApp.mMusicInterface);
			theMusicInterface.SetTempo("Classic", 110);
			base.Update();
		}

		public override void PlayMenuMusic()
		{
			CustomBassMusicInterface theMusicInterface =
				(CustomBassMusicInterface)(GlobalMembers.gApp.mMusicInterface);
			if (theMusicInterface.mSongName != "Classic")
			{
				theMusicInterface.QueueEvent("FadeOut", theMusicInterface.mSongName, false);
				theMusicInterface.QueueEvent("Play", "Classic", false);
			}
			// GlobalMembers.gApp.mMusic.PlaySongNoDelay(2, true);
		}

		public override void SubmitHighscore()
		{
			HighScoreTable orCreateTable = GlobalMembers.gApp.mHighScoreMgr.GetOrCreateTable(GlobalMembers.gApp.GetModeHeading(GameMode.MODE_CLASSIC));
			if (orCreateTable.Submit(GlobalMembers.gApp.mProfile.mProfileName, mPoints, GlobalMembers.gApp.mProfile.GetProfilePictureId()))
			{
				GlobalMembers.gApp.SaveHighscores();
			}
		}
	}
}
