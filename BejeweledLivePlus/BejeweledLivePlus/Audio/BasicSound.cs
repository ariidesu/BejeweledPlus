using SexyFramework.Sound;

namespace BejeweledLivePlus.Audio
{
	public abstract class BasicSound : Sound
	{
		protected SoundManager m_SoundManager;

		protected int m_SoundID = -1;

		protected abstract bool FindFreeSoundInstance(ref SoundInstance outInstance);
	}
}
