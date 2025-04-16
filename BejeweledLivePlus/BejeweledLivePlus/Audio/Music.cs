using System;
using SexyFramework;
using SexyFramework.Drivers.App;

namespace BejeweledLivePlus.Audio
{
	public class Music : IDisposable
	{
		private MusicInterface mMusicInterface;

		private bool mEnabled;

		private Song mCurrentSong = Song.DefaultSong;

		private Song mNextSong = Song.DefaultSong;

		public Music(MusicInterface inMusicInterface)
		{
			mMusicInterface = inMusicInterface;
			mEnabled = false;
			mCurrentSong = Song.DefaultSong;
			mNextSong = Song.DefaultSong;
		}

		public void RegisterCallBack()
		{
			mMusicInterface.RegisterCallback(OnSongChanged);
		}

		public void OnSongChanged(object sender, SongChangedEventArgs args)
		{
			mCurrentSong = new Song(args.songID, args.loop, 1f);
		}

		public void Dispose()
		{
			mMusicInterface.UnloadAllMusic();
		}

		public void Enable(bool inEnable)
		{
			if (mEnabled && !inEnable)
			{
				mNextSong = mCurrentSong;
				mCurrentSong = Song.DefaultSong;
				mMusicInterface.StopAllMusic();
			}
			mEnabled = inEnable;
		}

		public void LoadMusic(int inSongID, string inFileName)
		{
			mMusicInterface.LoadMusic(inSongID, inFileName, WP7AppDriver.sWP7AppDriverInstance.mContentManager);
		}

		public void PlaySong(int inSongID, float inFadeSpeed, bool inLoop)
		{
			PlaySong(inSongID, inFadeSpeed, inLoop, false);
		}

		public void PlaySongNoDelay(int inSongID, bool inLoop)
		{
			if (!IsPlaying(inSongID, false))
			{
				mCurrentSong = new Song(inSongID, inLoop, 1f);
				mMusicInterface.PlayMusic(mCurrentSong.mID, 0, !mCurrentSong.mLoop, 0L);
			}
		}

		public void PlaySong(int inSongID, float inFadeSpeed, bool inLoop, bool inForce)
		{
			if (!IsPlaying(inSongID, inForce) && !DelaySong(inSongID, inFadeSpeed, inLoop))
			{
				mCurrentSong = new Song(inSongID, inLoop, 1f);
				mMusicInterface.PlayMusic(mCurrentSong.mID, 0, !mCurrentSong.mLoop, 0L);
			}
		}

		public void FadeOut()
		{
			mCurrentSong = Song.DefaultSong;
			mNextSong = mCurrentSong;
			mMusicInterface.FadeOutAll();
		}

		public void StopAll()
		{
			mMusicInterface.StopAllMusic();
		}

		public void Update()
		{
			if (mEnabled && !mMusicInterface.IsPlaying(mCurrentSong.mID))
			{
				if (mNextSong.mID != -1)
				{
					mMusicInterface.FadeIn(mNextSong.mID, 0, mNextSong.mFadeSpeed, !mNextSong.mLoop);
				}
				mCurrentSong = mNextSong;
				mNextSong = Song.DefaultSong;
			}
		}

		private bool IsPlaying(int inSongID, bool inForceStop)
		{
			if (mMusicInterface.IsPlaying(inSongID))
			{
				return !inForceStop;
			}
			return false;
		}

		private bool DelaySong(int inSongID, float inFadeSpeed, bool inLoop)
		{
			if (mEnabled && !mMusicInterface.IsPlaying(mCurrentSong.mID))
			{
				return false;
			}
			mNextSong = new Song(inSongID, inLoop, inFadeSpeed);
			if (mEnabled)
			{
				mMusicInterface.FadeOut(mCurrentSong.mID, true, inFadeSpeed);
			}
			return true;
		}

		public bool IsUserMusicPlaying()
		{
			return mMusicInterface.isPlayingUserMusic();
		}
	}
}
