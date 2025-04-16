namespace BejeweledLivePlus.Audio
{
	internal class FadedSound : UpdatedSound
	{
		private bool mIsFree = true;

		private float mFadeInSpeed;

		private float mFadeOutSpeed;

		private float mTargetVolume;

		private float mLastTarget = -1f;

		public FadedSound(Sound inSound, float inFadeInSpeed, float inFadeOutSpeed)
		{
			mSound = inSound;
			mFadeInSpeed = inFadeInSpeed;
			mFadeOutSpeed = inFadeOutSpeed;
		}

		public override void Dispose()
		{
			mSound.Dispose();
		}

		public override void Play()
		{
			if (mIsFree)
			{
				mIsFree = false;
				mTargetVolume = mSound.GetVolume();
				mSound.Play();
			}
		}

		public override void Fade()
		{
			mTargetVolume = 0f;
		}

		public override void Update()
		{
			float volume = mSound.GetVolume();
			if (volume == mTargetVolume)
			{
				return;
			}
			if (mTargetVolume == 0f)
			{
				volume -= mFadeOutSpeed;
				if (volume < 0f)
				{
					volume = 0f;
				}
			}
			else
			{
				volume += mFadeInSpeed;
				if (volume > mTargetVolume)
				{
					volume = mTargetVolume;
				}
			}
			if (volume == 0f)
			{
				mIsFree = true;
			}
			else
			{
				mSound.SetVolume(volume);
			}
		}

		public override void Pause(bool inPauseOn)
		{
			if (inPauseOn)
			{
				CacheTargetVolume();
			}
			else
			{
				RestoreTargetVolume();
			}
			mSound.Pause(inPauseOn);
		}

		public override bool IsFree()
		{
			return mIsFree;
		}

		public override bool IsFading()
		{
			if (mTargetVolume == 0f)
			{
				return mSound.GetVolume() > 0f;
			}
			return false;
		}

		public override bool IsLooping()
		{
			return mSound.IsLooping();
		}

		public override float GetVolume()
		{
			return mSound.GetVolume();
		}

		public override void SetPan(int inPan)
		{
			mSound.SetPan(inPan);
		}

		public override void SetPitch(float inPitch)
		{
			mSound.SetPitch(inPitch);
		}

		public override void SetVolume(float inVolume)
		{
			mSound.SetVolume(inVolume);
		}

		public override void EnableAutoUnload()
		{
			mSound.EnableAutoUnload();
		}

		protected void CacheTargetVolume()
		{
			if (mTargetVolume != 0f)
			{
				mLastTarget = mTargetVolume;
				mTargetVolume = 0f;
			}
		}

		protected void RestoreTargetVolume()
		{
			if (mLastTarget != -1f)
			{
				mTargetVolume = mLastTarget;
				mLastTarget = -1f;
			}
		}
	}
}
