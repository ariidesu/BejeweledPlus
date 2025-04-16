using System.Collections.Generic;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class FreeEmitter
	{
		public TimeLine mSettingsTimeLine = new TimeLine();

		public TimeLine mVarianceTimeLine = new TimeLine();

		public List<LifetimeSettingPct> mLifePctSettings = new List<LifetimeSettingPct>();

		public Emitter mEmitter;

		public bool mAspectLocked = true;

		public int mSerialIndex = -1;

		public FreeEmitter()
		{
			mSettingsTimeLine.mCurrentSettings = new FreeEmitterSettings();
			mVarianceTimeLine.mCurrentSettings = new FreeEmitterVariance();
		}

		public FreeEmitter(FreeEmitter rhs)
			: this()
		{
			CopyFrom(rhs);
		}

		public virtual void Dispose()
		{
			if (mEmitter != null)
			{
				mEmitter.Dispose();
			}
			mEmitter = null;
			mLifePctSettings.Clear();
		}

		public void CopyFrom(FreeEmitter rhs)
		{
			if (rhs != null)
			{
				mSettingsTimeLine = rhs.mSettingsTimeLine;
				mVarianceTimeLine = rhs.mVarianceTimeLine;
				mAspectLocked = rhs.mAspectLocked;
				if (mEmitter == null)
				{
					mEmitter = new Emitter(rhs.mEmitter);
				}
				else
				{
					mEmitter.CopyFrom(rhs.mEmitter);
				}
				for (int i = 0; i < Common.size(mLifePctSettings); i++)
				{
					mLifePctSettings[i].second = null;
				}
				mLifePctSettings.Clear();
				for (int j = 0; j < Common.size(rhs.mLifePctSettings); j++)
				{
					AddLifetimeKeyFrame(rhs.mLifePctSettings[j].first, new LifetimeSettings(rhs.mLifePctSettings[j].second));
				}
			}
		}

		public void GetCreationParams(int frame, out int emitter_life, out float emit_frame, out FreeEmitterSettings settings, out FreeEmitterVariance variance)
		{
			mSettingsTimeLine.Update(frame);
			mVarianceTimeLine.Update(frame);
			if (Common.size(mSettingsTimeLine.mKeyFrames) == 0)
			{
				settings = new FreeEmitterSettings();
				mSettingsTimeLine.mCurrentSettings = settings;
			}
			else
			{
				settings = (FreeEmitterSettings)mSettingsTimeLine.mCurrentSettings;
			}
			if (Common.size(mVarianceTimeLine.mKeyFrames) == 0)
			{
				variance = new FreeEmitterVariance();
				mVarianceTimeLine.mCurrentSettings = variance;
			}
			else
			{
				variance = (FreeEmitterVariance)mVarianceTimeLine.mCurrentSettings;
			}
			emitter_life = GetRandomizedLife();
			emit_frame = 100f / (float)settings.mNumber;
		}

		public int GetRandomizedLife()
		{
			FreeEmitterSettings freeEmitterSettings = mSettingsTimeLine.mCurrentSettings as FreeEmitterSettings;
			FreeEmitterVariance freeEmitterVariance = mVarianceTimeLine.mCurrentSettings as FreeEmitterVariance;
			int num = freeEmitterSettings.mLife - (int)Common.SAFE_RAND(freeEmitterVariance.mLifeVar / 2) + (int)Common.SAFE_RAND(freeEmitterVariance.mLifeVar / 2);
			return 10 * num;
		}

		public LifetimeSettings AddLifetimeKeyFrame(float pct, LifetimeSettings s, float second_frame_pct, bool make_new)
		{
			LifetimeSettingPct lifetimeSettingPct = new LifetimeSettingPct(pct, s);
			lifetimeSettingPct.second.mPct = pct;
			mLifePctSettings.Add(lifetimeSettingPct);
			mLifePctSettings.Sort(new LifePctSort());
			if (second_frame_pct >= 0f)
			{
				AddLifetimeKeyFrame(second_frame_pct, new LifetimeSettings(s));
				if (make_new)
				{
					return new LifetimeSettings(s);
				}
			}
			return null;
		}

		public LifetimeSettings AddLifetimeKeyFrame(float pct, LifetimeSettings s, float second_frame_pct)
		{
			return AddLifetimeKeyFrame(pct, s, second_frame_pct, false);
		}

		public LifetimeSettings AddLifetimeKeyFrame(float pct, LifetimeSettings s)
		{
			return AddLifetimeKeyFrame(pct, s, -1f, false);
		}

		public FreeEmitterSettings AddSettingsKeyFrame(int frame, FreeEmitterSettings s, int second_frame_time, bool make_new)
		{
			mSettingsTimeLine.AddKeyFrame(frame, s);
			if (second_frame_time != -1)
			{
				mSettingsTimeLine.AddKeyFrame(second_frame_time, new FreeEmitterSettings(s));
				if (make_new)
				{
					return new FreeEmitterSettings(s);
				}
			}
			return null;
		}

		public FreeEmitterSettings AddSettingsKeyFrame(int frame, FreeEmitterSettings s, int second_frame_time)
		{
			return AddSettingsKeyFrame(frame, s, second_frame_time, false);
		}

		public FreeEmitterSettings AddSettingsKeyFrame(int frame, FreeEmitterSettings s)
		{
			return AddSettingsKeyFrame(frame, s, -1, false);
		}

		public FreeEmitterVariance AddVarianceKeyFrame(int frame, FreeEmitterVariance v, int second_frame_time, bool make_new)
		{
			mVarianceTimeLine.AddKeyFrame(frame, v);
			if (second_frame_time != -1)
			{
				mVarianceTimeLine.AddKeyFrame(second_frame_time, new FreeEmitterVariance(v));
				if (make_new)
				{
					return new FreeEmitterVariance(v);
				}
			}
			return null;
		}

		public FreeEmitterVariance AddVarianceKeyFrame(int frame, FreeEmitterVariance v, int second_frame_time)
		{
			return AddVarianceKeyFrame(frame, v, second_frame_time, false);
		}

		public FreeEmitterVariance AddVarianceKeyFrame(int frame, FreeEmitterVariance v)
		{
			return AddVarianceKeyFrame(frame, v, -1, false);
		}

		public void LoopSettingsTimeLine(bool l)
		{
			mSettingsTimeLine.mLoop = l;
		}

		public void LoopVarianceTimeLine(bool l)
		{
			mVarianceTimeLine.mLoop = l;
		}

		public void Serialize(Buffer b, GlobalMembers.GetIdByImageFunc f)
		{
			mSettingsTimeLine.Serialize(b);
			mVarianceTimeLine.Serialize(b);
			b.WriteBoolean(mAspectLocked);
			b.WriteLong(mSerialIndex);
			b.WriteLong(mLifePctSettings.Count);
			for (int i = 0; i < mLifePctSettings.Count; i++)
			{
				b.WriteFloat(mLifePctSettings[i].first);
				mLifePctSettings[i].second.Serialize(b);
			}
			mEmitter.Serialize(b, f);
		}

		public void Deserialize(Buffer b, GlobalMembers.GetImageByIdFunc f)
		{
			mSettingsTimeLine.Deserialize(b, FreeEmitterSettings.Instantiate);
			mVarianceTimeLine.Deserialize(b, FreeEmitterVariance.Instantiate);
			mAspectLocked = b.ReadBoolean();
			mSerialIndex = (int)b.ReadLong();
			int num = (int)b.ReadLong();
			for (int i = 0; i < num; i++)
			{
				float f2 = b.ReadFloat();
				LifetimeSettings lifetimeSettings = new LifetimeSettings();
				lifetimeSettings.Deserialize(b);
				mLifePctSettings.Add(new LifetimeSettingPct(f2, lifetimeSettings));
			}
			Dictionary<int, Deflector> deflector_ptr_map = new Dictionary<int, Deflector>();
			Dictionary<int, FreeEmitter> fe_ptr_map = new Dictionary<int, FreeEmitter>();
			mEmitter.Deserialize(b, deflector_ptr_map, fe_ptr_map, f);
		}
	}
}
