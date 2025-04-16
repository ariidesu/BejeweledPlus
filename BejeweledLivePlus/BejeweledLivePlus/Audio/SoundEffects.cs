using System;
using System.Collections.Generic;
using SexyFramework.Sound;

namespace BejeweledLivePlus.Audio
{
	public class SoundEffects : IDisposable
	{
		private SoundManager m_SoundManager;

		private Dictionary<int, Sound> mSounds = new Dictionary<int, Sound>();

		private int mChainedSound1 = -1;

		private int mChainedSound2 = -1;

		private int mChainedWait;

		public SoundEffects(SoundManager soundManager)
		{
			m_SoundManager = soundManager;
			SoundFactory.SetSoundManager(soundManager);
		}

		public void Dispose()
		{
			StopAll();
		}

		public void Play(int inSoundID)
		{
			Play(inSoundID, new SoundAttribs());
		}

		public void Play(int inSoundID, SoundAttribs inAttribs)
		{
			Sound sound = null;
			if (mSounds.ContainsKey(inSoundID))
			{
				sound = mSounds[inSoundID];
			}
			else
			{
				mSounds.Add(inSoundID, null);
			}
			if (sound == null)
			{
				sound = ((inAttribs.stagger <= 0) ? SoundFactory.GetSound(inSoundID, inAttribs.delay) : SoundFactory.GetStaggeredSound(inSoundID, inAttribs.stagger));
				mSounds[inSoundID] = sound;
			}
			sound.Play();
		}

		public void Loop(int inSoundID)
		{
			Loop(inSoundID, new SoundAttribs());
		}

		public void Loop(int inSoundID, SoundAttribs inAttribs)
		{
			Sound sound = null;
			if (mSounds.ContainsKey(inSoundID))
			{
				sound = mSounds[inSoundID];
			}
			else
			{
				mSounds.Add(inSoundID, null);
			}
			if (sound == null)
			{
				sound = SoundFactory.GetLoopingSound(inSoundID, inAttribs.delay, inAttribs.fadein, inAttribs.fadeout);
				mSounds[inSoundID] = sound;
			}
			sound.Play();
		}

		public void Update()
		{
			bool flag = false;
			int[] array = new int[mSounds.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<int, Sound> mSound in mSounds)
			{
				Sound value = mSound.Value;
				if (value.IsFree())
				{
					if (mSound.Key == mChainedSound1)
					{
						flag = true;
					}
					array[num++] = mSound.Key;
				}
				else
				{
					value.Update();
				}
			}
			for (int i = 0; i < num; i++)
			{
				mSounds.Remove(array[i]);
			}
			if (flag)
			{
				PlayNextInChain();
			}
		}

		internal bool IsLooping(int p)
		{
			return true;
		}

		internal void Stop(int inSoundID)
		{
			m_SoundManager.GetExistSoundInstance(inSoundID)?.Release();
			Stop(inSoundID, false);
		}

		internal void Stop(int inSoundID, bool inUnload)
		{
			Sound outSound = null;
			if (FindSound(inSoundID, ref outSound))
			{
				if (inUnload)
				{
					outSound.EnableAutoUnload();
				}
				mSounds.Remove(inSoundID);
			}
		}

		internal void StopAll()
		{
			if (m_SoundManager != null)
			{
				m_SoundManager.StopAllSounds();
			}
		}

		internal void Fade(int inSoundID, bool inUnload)
		{
			Sound outSound = null;
			if (FindSound(inSoundID, ref outSound))
			{
				if (inUnload)
				{
					outSound.EnableAutoUnload();
				}
				outSound.Fade();
			}
		}

		internal void Fade(int inSoundID)
		{
			Fade(inSoundID, false);
		}

		internal void PauseLoopingSounds(bool p)
		{
		}

		internal void PlayChained(int p, int p_2, int aDelay)
		{
		}

		private bool FindSound(int inSoundID, ref Sound outSound)
		{
			if (mSounds.ContainsKey(inSoundID))
			{
				outSound = mSounds[inSoundID];
				return true;
			}
			return false;
		}

		private void PlayNextInChain()
		{
			SoundAttribs soundAttribs = new SoundAttribs();
			soundAttribs.delay = mChainedWait;
			Play(mChainedSound2, soundAttribs);
			mChainedSound1 = -1;
			mChainedSound2 = -1;
			mChainedWait = 0;
		}
	}
}
