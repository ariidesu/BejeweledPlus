namespace BejeweledLivePlus.Audio
{
	internal class StaggeredSound : UpdatedSound
	{
		private int mStaggerTime;

		private int mStaggerCount;

		public StaggeredSound(Sound inSound, int inStaggerTime)
		{
			mSound = inSound;
			mStaggerTime = inStaggerTime;
			mStaggerCount = inStaggerTime;
		}

		public override void Dispose()
		{
			mSound.Dispose();
		}

		public override void Play()
		{
			if (mStaggerCount >= mStaggerTime)
			{
				mStaggerCount = 0;
				mSound.Play();
			}
		}

		public override void Fade()
		{
			mSound.Fade();
		}

		public override void Update()
		{
			mStaggerCount++;
		}

		public override void Pause(bool inPauseOn)
		{
			mSound.Pause(inPauseOn);
		}

		public override bool IsFree()
		{
			if (mStaggerCount > mStaggerTime * 1000)
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
