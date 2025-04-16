using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace SexyFramework.Sound
{
	public class XSoundInstance : SoundInstance
	{
		private SoundEffectInstance m_SoundInstance;

		private static Stack<XSoundInstance> unusedObjects = new Stack<XSoundInstance>(20);

		public int m_SoundID;

		public float mBaseVolume;

		public float mBasePan;

		private float mVolume;

		private float mPan;

		private float mPitch;

		private bool didPlay;

		private bool mIsReleased;

		private int referenceCount_;

		public static XSoundInstance GetNewXSoundInstance(int id, SoundEffectInstance instance)
		{
			if (unusedObjects.Count > 0)
			{
				XSoundInstance xSoundInstance = unusedObjects.Pop();
				xSoundInstance.Reset(id, instance);
				return xSoundInstance;
			}
			return new XSoundInstance(id, instance);
		}

		public XSoundInstance(int id, SoundEffectInstance instance)
		{
			Reset(id, instance);
		}

		public override void retain()
		{
			referenceCount_++;
		}

		public override void releaseRef()
		{
			referenceCount_--;
		}

		public void Reset(int id, SoundEffectInstance instance)
		{
			m_SoundInstance = instance;
			m_SoundID = id;
			didPlay = false;
			mBaseVolume = 1f;
			mVolume = 1f;
			mBasePan = 0f;
			mPan = 0f;
			mPitch = 0f;
			mIsReleased = false;
			referenceCount_ = 0;
		}

		public override void Release()
		{
			if (m_SoundInstance != null)
			{
				m_SoundInstance.Stop();
				m_SoundInstance.Dispose();
				m_SoundInstance = null;
			}
			referenceCount_ = 0;
			mIsReleased = true;
		}

		public override void SetBaseVolume(double theBaseVolume)
		{
			mBaseVolume = (float)theBaseVolume;
		}

		public override void SetBasePan(int theBasePan)
		{
			mBasePan = (float)theBasePan / 100f;
		}

		public override void SetBaseRate(double theBaseRate)
		{
		}

		public override void AdjustPitch(double theNumSteps)
		{
			mPitch = (float)theNumSteps;
		}

		public override void SetVolume(double theVolume)
		{
			mVolume = (float)theVolume;
			if (m_SoundInstance != null)
			{
				m_SoundInstance.Volume = (float)theVolume;
			}
		}

		public override void SetMasterVolumeIdx(int theVolumeIdx)
		{
		}

		public override void SetPan(int thePosition)
		{
			mPan = (float)thePosition / 10000f;
		}

		public override bool Play(bool looping, bool autoRelease)
		{
			Stop();
			didPlay = true;
			if (m_SoundInstance.State == SoundState.Stopped)
			{
				try
				{
					m_SoundInstance.IsLooped = looping;
				}
				catch
				{
				}
			}
			m_SoundInstance.Play();
			return true;
		}

		public override void Stop()
		{
			if (m_SoundInstance != null)
			{
				m_SoundInstance.Stop();
			}
		}

		public override void Pause()
		{
			if (m_SoundInstance != null)
			{
				m_SoundInstance.Pause();
			}
		}

		public override void Resume()
		{
			if (m_SoundInstance != null)
			{
				m_SoundInstance.Resume();
			}
		}

		public override bool IsPlaying()
		{
			if (m_SoundInstance != null)
			{
				return m_SoundInstance.State == SoundState.Playing;
			}
			return false;
		}

		public override bool IsReleased()
		{
			return mIsReleased;
		}

		public override double GetVolume()
		{
			return mVolume;
		}

		public override bool IsDormant()
		{
			if (didPlay && m_SoundInstance.State == SoundState.Stopped)
			{
				return referenceCount_ <= 0;
			}
			return false;
		}

		public void PrepareForReuse()
		{
			unusedObjects.Push(this);
		}
	}
}
