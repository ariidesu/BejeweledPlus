using SexyFramework.Sound;

namespace BejeweledLivePlus.Audio
{
	public class SoundFactory
	{
		private static SoundManager m_SoundManager;

		public static void SetSoundManager(SoundManager inSoundManager)
		{
			m_SoundManager = inSoundManager;
		}

		public static Sound GetSound(int inSoundID, int inDelay)
		{
			return GetSound(inSoundID, inDelay, true);
		}

		public static Sound GetSound(int inSoundID, int inDelay, bool inAutoRelease)
		{
			BurstSound burstSound = new BurstSound(inSoundID, m_SoundManager, inAutoRelease);
			if (inDelay > 0)
			{
				return new DelayedSound(burstSound, inDelay);
			}
			return burstSound;
		}

		public static Sound GetStaggeredSound(int inSoundID, int inStaggerTime)
		{
			return new StaggeredSound(new BurstSound(inSoundID, m_SoundManager, true), inStaggerTime);
		}

		public static Sound GetLoopingSound(int inSoundID, int inDelay, float inFadeInSpeed, float inFadeOutSpeed)
		{
			LoopingSound loopingSound = new LoopingSound(inSoundID, m_SoundManager);
			if (inDelay > 0)
			{
				if (inFadeInSpeed < 1f || inFadeOutSpeed < 1f)
				{
					return new DelayedSound(new FadedSound(loopingSound, inFadeInSpeed, inFadeOutSpeed), inDelay);
				}
				return new DelayedSound(loopingSound, inDelay);
			}
			if (inFadeInSpeed < 1f || inFadeOutSpeed < 1f)
			{
				return new FadedSound(loopingSound, inFadeInSpeed, inFadeOutSpeed);
			}
			return loopingSound;
		}
	}
}
