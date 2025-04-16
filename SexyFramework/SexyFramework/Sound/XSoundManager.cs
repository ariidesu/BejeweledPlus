using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SexyFramework.Sound
{
	public class XSoundManager : SoundManager
	{
		public static int MAX_SOURCE_SOUNDS = 4096;

		public static int ACTIVE_SOUNDS_LIMIT = 32;

		private ContentManager mContent;

		private XSoundEntry[] m_sounds = new XSoundEntry[MAX_SOURCE_SOUNDS];

		private List<XSoundInstance> mInstances;

		public XSoundManager(ContentManager cmgr)
		{
			mContent = cmgr;
			mInstances = new List<XSoundInstance>();
			for (int i = 0; i < MAX_SOURCE_SOUNDS; i++)
			{
				m_sounds[i] = null;
			}
		}

		public override int GetFreeSoundId()
		{
			for (int i = 0; i < MAX_SOURCE_SOUNDS; i++)
			{
				if (m_sounds[i] == null)
				{
					return i;
				}
			}
			return -1;
		}

		public override void ReleaseSound(int theSfxID)
		{
		}

		public override bool LoadSound(uint theSfxID, string theFilename)
		{
			SoundEffect soundEffect = mContent.Load<SoundEffect>(theFilename);
			XSoundEntry xSoundEntry = new XSoundEntry();
			xSoundEntry.m_SoundEffect = soundEffect;
			xSoundEntry.mFileName = theFilename;
			m_sounds[theSfxID] = xSoundEntry;
			return true;
		}

		public override int LoadSound(string theFilename)
		{
			for (int i = 0; i < MAX_SOURCE_SOUNDS; i++)
			{
				if (m_sounds[i] != null && m_sounds[i].mFileName == theFilename)
				{
					return i;
				}
			}
			for (int num = MAX_SOURCE_SOUNDS - 1; num >= 0; num--)
			{
				if (m_sounds[num] == null)
				{
					if (!LoadSound((uint)num, theFilename))
					{
						break;
					}
					return num;
				}
			}
			return -1;
		}

		public override void Update()
		{
			for (int num = mInstances.Count - 1; num >= 0; num--)
			{
				if (mInstances[num] == null)
				{
					mInstances.RemoveAt(num);
				}
				else if (mInstances[num].IsReleased())
				{
					mInstances[num].PrepareForReuse();
					mInstances.RemoveAt(num);
				}
				else if (mInstances[num].IsDormant())
				{
					if (!mInstances[num].IsReleased())
					{
						mInstances[num].Release();
					}
					mInstances[num].PrepareForReuse();
					mInstances.RemoveAt(num);
				}
			}
		}

		public override bool Initialized()
		{
			return true;
		}

		public override void SetVolume(double theVolume)
		{
			SetMasterVolume(theVolume);
		}

		public override void SetVolume(int theVolIdx, double theVolume)
		{
			SetVolume(theVolume);
		}

		public override void SetBasePan(uint theSfxID, int theBasePan)
		{
			if (theBasePan < -100 || theBasePan > 100)
			{
				return;
			}
			m_sounds[theSfxID].m_BasePan = (float)theBasePan / 100f;
			for (int i = 0; i < mInstances.Count; i++)
			{
				if (mInstances[i].m_SoundID == (int)theSfxID)
				{
					mInstances[i].SetBasePan(theBasePan);
				}
			}
		}

		public override void SetBaseVolume(uint theSfxID, double theBaseVolume)
		{
			if (theBaseVolume < 0.0 || theBaseVolume > 1.0)
			{
				return;
			}
			m_sounds[theSfxID].m_BaseVolume = (float)theBaseVolume;
			for (int i = 0; i < mInstances.Count; i++)
			{
				if (mInstances[i].m_SoundID == (int)theSfxID)
				{
					mInstances[i].SetBaseVolume(m_sounds[theSfxID].m_BaseVolume);
				}
			}
		}

		public void ReleaseFreeChannels()
		{
		}

		public override SoundInstance GetSoundInstance(int theSfxID)
		{
			if (mInstances.Count >= ACTIVE_SOUNDS_LIMIT)
			{
				return null;
			}
			SoundEffectInstance instance = m_sounds[theSfxID].m_SoundEffect.CreateInstance();
			XSoundInstance newXSoundInstance = XSoundInstance.GetNewXSoundInstance(theSfxID, instance);
			mInstances.Add(newXSoundInstance);
			return newXSoundInstance;
		}

		public override SoundInstance GetExistSoundInstance(int theSfxID)
		{
			if (theSfxID > MAX_SOURCE_SOUNDS)
			{
				return null;
			}
			if (m_sounds[theSfxID] == null)
			{
				return null;
			}
			for (int i = 0; i < mInstances.Count; i++)
			{
				if (mInstances[i] != null && mInstances[i].m_SoundID == theSfxID)
				{
					return mInstances[i];
				}
			}
			return null;
		}

		public override void ReleaseSounds()
		{
			for (int i = 0; i < MAX_SOURCE_SOUNDS; i++)
			{
				if (m_sounds[i] != null)
				{
					m_sounds[i] = null;
				}
			}
		}

		public override void ReleaseChannels()
		{
		}

		public override double GetMasterVolume()
		{
			return m_MasterVolume;
		}

		public override void SetMasterVolume(double theVolume)
		{
			m_MasterVolume = (float)theVolume;
		}

		public override void Flush()
		{
		}

		public override void StopAllSounds()
		{
			for (int i = 0; i < mInstances.Count; i++)
			{
				mInstances[i].Stop();
			}
		}

		public override int GetNumSounds()
		{
			int num = 0;
			for (int i = 0; i < MAX_SOURCE_SOUNDS; i++)
			{
				if (m_sounds[i] != null)
				{
					num++;
				}
			}
			return num;
		}
	}
}
