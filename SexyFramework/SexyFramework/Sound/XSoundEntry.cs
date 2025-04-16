using Microsoft.Xna.Framework.Audio;

namespace SexyFramework.Sound
{
	internal class XSoundEntry
	{
		public SoundEffect m_SoundEffect;

		public float m_BaseVolume;

		public float m_BasePan;

		public string mFileName = string.Empty;

		public void Dispose()
		{
			m_SoundEffect.Dispose();
		}
	}
}
