using SexyFramework.Sound;

namespace BejeweledLivePlus.Audio
{
	internal class LoopingSound : BasicSound
	{
		private SoundInstance mSoundInstance;

		private bool mPaused;

		private bool mUnloadSource;

		private float mVolume = 1f;

		public LoopingSound(int inSoundID, SoundManager inSoundManager)
		{
			m_SoundID = inSoundID;
			m_SoundManager = inSoundManager;
		}

		public override void Dispose()
		{
		}

		public override void Play()
		{
			if (mSoundInstance == null && FindFreeSoundInstance(ref mSoundInstance))
			{
				mSoundInstance.SetVolume(GetVolume());
				mSoundInstance.Play(true, false);
			}
		}

		protected override bool FindFreeSoundInstance(ref SoundInstance outInstance)
		{
			SoundInstance soundInstance = m_SoundManager.GetSoundInstance(m_SoundID);
			if (soundInstance != null)
			{
				outInstance = soundInstance;
			}
			return soundInstance != null;
		}

		public override void Fade()
		{
		}

		public override void Update()
		{
		}

		public override void Pause(bool inPauseOn)
		{
			mPaused = inPauseOn;
			if (mSoundInstance != null)
			{
				if (mPaused)
				{
					mSoundInstance.Release();
				}
				else if (FindFreeSoundInstance(ref mSoundInstance))
				{
					mSoundInstance.SetVolume(GetVolume());
					mSoundInstance.Play(true, false);
				}
			}
		}

		public override bool IsFree()
		{
			return mSoundInstance == null;
		}

		public override bool IsFading()
		{
			return false;
		}

		public override bool IsLooping()
		{
			return mSoundInstance != null;
		}

		public override float GetVolume()
		{
			if (!mPaused)
			{
				return mVolume;
			}
			return 0f;
		}

		public override void SetPan(int inPan)
		{
		}

		public override void SetPitch(float inPitch)
		{
		}

		public override void SetVolume(float inVolume)
		{
			mVolume = inVolume;
			if (mSoundInstance != null)
			{
				mSoundInstance.SetVolume(inVolume);
			}
		}

		public override void EnableAutoUnload()
		{
			mUnloadSource = true;
		}
	}
}
