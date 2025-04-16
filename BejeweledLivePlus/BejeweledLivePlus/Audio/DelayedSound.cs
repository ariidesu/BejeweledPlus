namespace BejeweledLivePlus.Audio
{
	internal class DelayedSound : UpdatedSound
	{
		private bool mIsFree = true;

		private bool mDoCountdown;

		private int mDelay;

		private int mUpdateCount;

		public DelayedSound(Sound inSound, int inDelay)
		{
			mSound = inSound;
			mDelay = inDelay;
		}

		public override void Dispose()
		{
			mSound.Dispose();
		}

		public override void Play()
		{
			if (mUpdateCount <= 0)
			{
				mIsFree = false;
				mDoCountdown = true;
			}
		}

		public override void Fade()
		{
			mSound.Fade();
		}

		public override void Update()
		{
			if (mDoCountdown)
			{
				mUpdateCount++;
			}
			if (mUpdateCount == mDelay)
			{
				mSound.Play();
			}
			else if (mUpdateCount > mDelay)
			{
				mIsFree = true;
				mDoCountdown = false;
				mSound.Update();
			}
		}

		public override void Pause(bool inPauseOn)
		{
			if (mUpdateCount <= mDelay)
			{
				mDoCountdown = !inPauseOn;
			}
			mSound.Pause(inPauseOn);
		}

		public override bool IsFree()
		{
			if (mIsFree)
			{
				return mSound.IsFree();
			}
			return false;
		}

		public override bool IsFading()
		{
			return mSound.IsFading();
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
	}
}
