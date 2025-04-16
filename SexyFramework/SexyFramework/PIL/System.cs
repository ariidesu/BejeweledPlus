using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class System
	{
		public delegate void FPSCallback(System s, int last_fps);

		private int mEmitterIteratorIndex;

		private int mForceIteratorIndex;

		private int mDeflectorIteratorIndex;

		private int mParticlePoolSize;

		private int mParticlePoolIndex;

		private int mParticlePoolGrowAmt;

		private int mStartingParticlePoolSize;

		private int mLastEmitterHandle;

		private List<Particle> mParticlePool;

		protected PerfTimer mFPSTimer = new PerfTimer();

		protected int mFrameCount;

		protected List<EmitterUpdatePair> mEmitters = new List<EmitterUpdatePair>();

		protected List<Deflector> mDeflectors = new List<Deflector>();

		protected List<Force> mForces = new List<Force>();

		protected Dictionary<int, Emitter> mEmitterHandleMap = new Dictionary<int, Emitter>();

		protected int mUpdateCount;

		protected int mLife;

		protected int mMinSpawnFrame;

		protected int mMaxParticleCount;

		protected float mLastX;

		protected float mLastY;

		protected bool mWaitForEmitters;

		protected bool mForceStopEmitting;

		public float mAlphaPct;

		public float mScale;

		public float mParticleScale2D;

		public int mLowWatermark;

		public int mHighWatermark;

		public int mFPSInterval;

		public int mMaxParticles;

		public FPSCallback mFPSCallback;

		public static void KillParticlesFPSCallback(System s, int last_fps)
		{
			if (last_fps > s.mHighWatermark)
			{
				return;
			}
			int totalParticles = s.GetTotalParticles();
			Emitter emitter = null;
			int handle = 0;
			while ((emitter = s.GetNextEmitter(ref handle)) != null)
			{
				int num = emitter.NumParticles();
				float num2 = (float)num / (float)totalParticles;
				int num3 = (int)((float)(s.mHighWatermark - last_fps) * ModVal.M(2f) * num2);
				if (last_fps < s.mLowWatermark)
				{
					num3 += (int)((float)num3 * (float)(s.mLowWatermark - last_fps) / (float)s.mLowWatermark);
				}
				List<Particle> v = new List<Particle>();
				s.GetOldestParticles(num3, ref v);
				for (int i = 0; i < Common.size(v); i++)
				{
					if (v[i] != null)
					{
						v[i].mForceDeletion = true;
					}
				}
			}
		}

		public static void FadeParticlesFPSCallback(System s, int last_fps)
		{
			if (last_fps > s.mHighWatermark)
			{
				if (s.mMaxParticles > 0)
				{
					s.mMaxParticles++;
				}
				return;
			}
			int totalParticles = s.GetTotalParticles();
			if (s.mMaxParticles == -1)
			{
				s.mMaxParticles = totalParticles;
			}
			if (last_fps < (s.mLowWatermark + s.mHighWatermark) / 2)
			{
				s.mMaxParticles -= ((last_fps >= s.mLowWatermark) ? 1 : 2);
			}
			Emitter emitter = null;
			int handle = 0;
			while ((emitter = s.GetNextEmitter(ref handle)) != null)
			{
				int num = emitter.NumParticles();
				float num2 = (float)num / (float)totalParticles;
				float num3 = ModVal.M(10f);
				int num4 = (int)((float)(s.mHighWatermark - last_fps) * ModVal.M(2f) * num2);
				if (last_fps < s.mLowWatermark)
				{
					float num5 = (float)(s.mLowWatermark - last_fps) / (float)s.mLowWatermark;
					num4 += (int)((float)num4 * num5);
					num3 += num3 * num5;
				}
				List<Particle> v = new List<Particle>();
				s.GetOldestParticles(num4, ref v);
				for (int i = 0; i < Common.size(v); i++)
				{
					if (v[i] != null)
					{
						v[i].mForceFadeoutRate = num3;
					}
				}
			}
		}

		protected void Clean()
		{
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				if (mEmitters[i].emitter != null)
				{
					mEmitters[i].emitter.Dispose();
				}
			}
			for (int j = 0; j < Common.size(mForces); j++)
			{
				if (mForces[j] != null)
				{
					mForces[j].Dispose();
				}
			}
			for (int k = 0; k < Common.size(mDeflectors); k++)
			{
				if (mDeflectors[k] != null)
				{
					mDeflectors[k].Dispose();
				}
			}
			for (int l = 0; l < mParticlePoolSize; l++)
			{
				if (mParticlePool[l] != null)
				{
					mParticlePool[l].Dispose();
				}
			}
			mEmitters.Clear();
			mForces.Clear();
			mDeflectors.Clear();
			mParticlePool.Clear();
			mParticlePool = null;
		}

		public System()
			: this(100, 200)
		{
		}

		public System(int particle_pool_size, int particle_pool_grow)
		{
			mUpdateCount = 0;
			mParticleScale2D = 1f;
			mWaitForEmitters = true;
			mLife = -1;
			mForceStopEmitting = false;
			mEmitterIteratorIndex = 0;
			mForceIteratorIndex = 0;
			mDeflectorIteratorIndex = 0;
			mParticlePoolGrowAmt = particle_pool_grow;
			mStartingParticlePoolSize = particle_pool_size;
			mParticlePoolSize = particle_pool_size;
			mParticlePoolIndex = 0;
			mLastX = 0f;
			mLastY = 0f;
			mMinSpawnFrame = -1;
			mAlphaPct = 1f;
			mScale = 1f;
			mMaxParticleCount = 0;
			mLowWatermark = -100;
			mHighWatermark = 100000;
			mFrameCount = 0;
			mFPSInterval = 100;
			mMaxParticles = -1;
			mLastEmitterHandle = 0;
			mParticlePool = new List<Particle>(mParticlePoolSize);
			for (int i = 0; i < particle_pool_size; i++)
			{
				Particle particle = new Particle();
				mParticlePool.Add(particle);
				particle.mPoolIndex = -1;
			}
		}

		public virtual void Dispose()
		{
			Clean();
		}

		public void ResetForReuse()
		{
			mUpdateCount = 0;
			mWaitForEmitters = true;
			mLife = -1;
			mForceStopEmitting = false;
			mEmitterIteratorIndex = 0;
			mForceIteratorIndex = 0;
			mDeflectorIteratorIndex = 0;
			mLastX = 0f;
			mLastY = 0f;
			mMinSpawnFrame = -1;
			mAlphaPct = 1f;
			mScale = 1f;
			mMaxParticleCount = 0;
			mFrameCount = 0;
			mMaxParticles = -1;
			mFPSTimer = new PerfTimer();
			for (int i = 0; i < mEmitters.Count; i++)
			{
				mEmitters[i].emitter.ResetForReuse();
				mEmitters[i].value = 0;
			}
			for (int j = 0; j < mDeflectors.Count; j++)
			{
				mDeflectors[j].ResetForReuse();
			}
			for (int k = 0; k < mForces.Count; k++)
			{
				mForces[k].ResetForReuse();
			}
		}

		public Particle AllocateParticle()
		{
			if (mParticlePoolIndex >= mParticlePoolSize)
			{
				mParticlePoolSize += mParticlePoolGrowAmt;
				for (int i = mParticlePoolIndex; i < mParticlePoolSize; i++)
				{
					mParticlePool.Add(new Particle());
				}
			}
			Particle particle = mParticlePool[mParticlePoolIndex];
			particle.mPoolIndex = mParticlePoolIndex++;
			if (mParticlePoolIndex > mMaxParticleCount)
			{
				mMaxParticleCount = mParticlePoolIndex;
			}
			return particle;
		}

		public void DeleteParticle(Particle p)
		{
			mParticlePool.Insert(mParticlePoolIndex - 1, p);
			p.mPoolIndex = mParticlePoolIndex - 1;
			mParticlePoolIndex--;
		}

		private void GetOldestParticles(int num, ref List<Particle> v)
		{
			if (num > 0)
			{
				List<Particle> list = new List<Particle>();
				for (int i = 0; i < mParticlePoolIndex; i++)
				{
					list.Add(mParticlePool[i]);
				}
				list.Sort(new SortOldestParticles());
				if (num > list.Count)
				{
					num = list.Count;
				}
				Particle[] array = new Particle[num];
				list.CopyTo(array);
				v.Clear();
				v.AddRange(array);
			}
		}

		public int AddEmitter(Emitter e)
		{
			e.mSystem = this;
			e.Move(mLastX, mLastY);
			mEmitters.Add(new EmitterUpdatePair(e, 0));
			mLastEmitterHandle++;
			mEmitterHandleMap[mLastEmitterHandle] = e;
			e.mHandle = mLastEmitterHandle;
			e.mSerialIndex = mLastEmitterHandle;
			return mLastEmitterHandle;
		}

		public void DeleteEmitter(int handle)
		{
			if (!mEmitterHandleMap.ContainsKey(handle))
			{
				return;
			}
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				if (mEmitters[i].emitter.mHandle == handle)
				{
					mEmitters[i].emitter.Dispose();
					mEmitters[i].emitter = null;
					mEmitters.RemoveAt(i);
					break;
				}
			}
		}

		public void Update()
		{
			if (!mFPSTimer.IsRunning())
			{
				mFPSTimer.Start();
			}
			for (int i = 0; i < Common.size(mForces); i++)
			{
				mForces[i].Update(mUpdateCount);
				for (int j = 0; j < Common.size(mEmitters); j++)
				{
					mEmitters[j].emitter.ApplyForce(mForces[i]);
				}
			}
			for (int k = 0; k < Common.size(mDeflectors); k++)
			{
				mDeflectors[k].Update(mUpdateCount);
				for (int l = 0; l < Common.size(mEmitters); l++)
				{
					mEmitters[l].emitter.ApplyDeflector(mDeflectors[k]);
				}
			}
			for (int m = 0; m < Common.size(mEmitters); m++)
			{
				EmitterUpdatePair emitterUpdatePair = mEmitters[m];
				if (emitterUpdatePair.emitter.mPreloadFrames <= 0)
				{
					continue;
				}
				for (int n = 0; n < emitterUpdatePair.emitter.mPreloadFrames; n++)
				{
					emitterUpdatePair.value++;
					bool allow_creation = (mLife < 0 && !mForceStopEmitting) || (emitterUpdatePair.value < mLife && !mForceStopEmitting);
					if (mUpdateCount < mMinSpawnFrame)
					{
						allow_creation = false;
					}
					emitterUpdatePair.emitter.Update(emitterUpdatePair.value, allow_creation);
				}
				emitterUpdatePair.emitter.mPreloadFrames = 0;
			}
			int totalParticles = GetTotalParticles();
			mUpdateCount++;
			for (int num = 0; num < Common.size(mEmitters); num++)
			{
				mEmitters[num].value++;
				bool allow_creation2 = (mLife < 0 && !mForceStopEmitting) || (mEmitters[num].value + mEmitters[num].emitter.mStartFrame < mLife && !mForceStopEmitting);
				if (mUpdateCount < mMinSpawnFrame || (mMaxParticles > 0 && totalParticles > mMaxParticles))
				{
					allow_creation2 = false;
				}
				mEmitters[num].emitter.Update(mEmitters[num].value + mEmitters[num].emitter.mStartFrame, allow_creation2);
			}
			if (mFPSTimer.GetDuration() >= (double)mFPSInterval)
			{
				mFPSTimer.Stop();
				int last_fps = (int)((double)(mFrameCount * mFPSInterval) / mFPSTimer.GetDuration() + 0.5) * (1000 / mFPSInterval);
				if (mFPSCallback != null)
				{
					mFPSCallback(this, last_fps);
				}
				mFrameCount = 0;
				mFPSTimer.Start();
			}
		}

		public void Draw(SexyFramework.Graphics.Graphics g)
		{
			Draw(g, false, 0, 0);
		}

		public void Draw(SexyFramework.Graphics.Graphics g, bool draw_debug_info, int debugx, int debugy)
		{
			mFrameCount++;
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				mEmitters[i].emitter.Draw(g);
			}
		}

		public void SetLife(int life)
		{
			mLife = life;
		}

		public void WaitForEmitters(bool w)
		{
			mWaitForEmitters = w;
		}

		public bool Done()
		{
			if (mLife < 0 || mUpdateCount < mLife)
			{
				return false;
			}
			if (!mWaitForEmitters)
			{
				return true;
			}
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				if (mEmitters[i].emitter.NumParticles() - mEmitters[i].emitter.GetNumSingleParticles() > 0)
				{
					return false;
				}
			}
			return true;
		}

		public int GetUpdateCount()
		{
			return mUpdateCount;
		}

		public void ForceStopEmitting(bool f)
		{
			mForceStopEmitting = f;
			if (f)
			{
				mLife = 0;
			}
		}

		public void Move(float xamt, float yamt)
		{
			mLastX += xamt;
			mLastY += yamt;
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				mEmitters[i].emitter.Move(xamt, yamt);
			}
		}

		public void SetPos(float x, float y)
		{
			mLastX = x;
			mLastY = y;
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				mEmitters[i].emitter.SetPos(x, y);
			}
		}

		public void AddForce(Force f)
		{
			mForces.Add(f);
			f.mSystem = this;
		}

		public void AddDeflector(Deflector d)
		{
			mDeflectors.Add(d);
			d.mSystem = this;
			d.mSerialIndex = Common.size(mDeflectors) - 1;
		}

		public void SetMinSpawnFrame(int f)
		{
			mMinSpawnFrame = f;
		}

		public Emitter GetEmitter(int emitter_handle)
		{
			if (mEmitterHandleMap.ContainsKey(emitter_handle))
			{
				return mEmitterHandleMap[emitter_handle];
			}
			return null;
		}

		public Emitter GetNextEmitter(ref int handle)
		{
			if (mEmitterIteratorIndex < Common.size(mEmitters))
			{
				handle = mEmitterIteratorIndex;
				return mEmitters[mEmitterIteratorIndex++].emitter;
			}
			mEmitterIteratorIndex = 0;
			return null;
		}

		public Force GetNextForce()
		{
			if (mForceIteratorIndex < Common.size(mForces))
			{
				return mForces[mForceIteratorIndex++];
			}
			mForceIteratorIndex = 0;
			return null;
		}

		public Deflector GetNextDeflector()
		{
			if (mDeflectorIteratorIndex < Common.size(mDeflectors))
			{
				return mDeflectors[mDeflectorIteratorIndex++];
			}
			mDeflectorIteratorIndex = 0;
			return null;
		}

		public int GetTotalParticles()
		{
			int num = 0;
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				num += mEmitters[i].emitter.NumParticles();
			}
			return num;
		}

		public float GetLastX()
		{
			return mLastX;
		}

		public float GetLastY()
		{
			return mLastY;
		}

		public int GetMaxParticleCount()
		{
			return mMaxParticleCount;
		}

		public void ResetMaxParticleCount()
		{
			mMaxParticleCount = 0;
		}

		public bool LoadPINFile(string file_name, List<string> image_names)
		{
			Buffer buffer = new Buffer();
			if (!GlobalMembers.gSexyAppBase.ReadBufferFromFile(file_name, buffer))
			{
				return false;
			}
			long num = -1L;
			Emitter emitter = null;
			Emitter emitter2 = null;
			ParticleType particleType = null;
			Dictionary<float, LifetimeSettings> dictionary = new Dictionary<float, LifetimeSettings>();
			while (!buffer.AtEnd())
			{
				dictionary.Clear();
				if (num == -1)
				{
					num = buffer.ReadLong();
					if (num != PINCommon.SECTION_EMITTER && num != PINCommon.SECTION_FREE_EMITTER && num != PINCommon.SECTION_PARTICLE_TYPE)
					{
						return false;
					}
				}
				else if (num == PINCommon.SECTION_EMITTER)
				{
					Dictionary<int, EmitterSettings> dictionary2 = new Dictionary<int, EmitterSettings>();
					Dictionary<int, EmitterScale> dictionary3 = new Dictionary<int, EmitterScale>();
					string mName = buffer.ReadString().Trim();
					bool flag = buffer.ReadBoolean();
					Emitter emitter3 = null;
					if (flag)
					{
						emitter2 = new Emitter();
						emitter3 = emitter2;
					}
					else
					{
						emitter = new Emitter();
						emitter3 = emitter;
					}
					emitter3.mName = mName;
					long num2 = buffer.ReadLong();
					emitter3.SetEmitterType((int)num2);
					buffer.ReadBoolean();
					int theRed = buffer.ReadInt32();
					int theGreen = buffer.ReadInt32();
					int theBlue = buffer.ReadInt32();
					emitter3.mTintColor = new Color(theRed, theGreen, theBlue);
					bool flag2 = false;
					while (!flag2 && !buffer.AtEnd())
					{
						int num3 = buffer.ReadInt32();
						if (num3 == PINCommon.KEYFRAME_ENTRY_END)
						{
							continue;
						}
						if (num3 == PINCommon.SECTION_END)
						{
							flag2 = true;
							break;
						}
						int key = Common.StrToInt(buffer.ReadString());
						float num4 = Common.StrToFloat(buffer.ReadString());
						buffer.ReadBoolean();
						buffer.ReadBoolean();
						Common.StrToFloat(buffer.ReadString());
						Common.StrToFloat(buffer.ReadString());
						Common.StrToFloat(buffer.ReadString());
						Common.StrToFloat(buffer.ReadString());
						if (num3 >= PINCommon.EM_FIRST_SCALE && num3 <= PINCommon.EM_LAST_SCALE)
						{
							EmitterScale emitterScale = new EmitterScale();
							switch ((EM_Prop)num3)
							{
							case EM_Prop.EM_First_Prop:
								emitterScale.mLifeScale = num4 / 100f;
								break;
							case EM_Prop.EM_Number:
								emitterScale.mNumberScale = num4 / 100f;
								break;
							case EM_Prop.EM_Size:
								emitterScale.mSizeXScale = num4 / 100f;
								break;
							case EM_Prop.EM_SizeY:
								emitterScale.mSizeYScale = num4 / 100f;
								break;
							case EM_Prop.EM_Velocity:
								emitterScale.mVelocityScale = num4 / 100f;
								break;
							case EM_Prop.EM_Weight:
								emitterScale.mWeightScale = num4 / 100f;
								break;
							case EM_Prop.EM_Spin:
								emitterScale.mSpinScale = num4 / 100f;
								break;
							case EM_Prop.EM_MotionRand:
								emitterScale.mMotionRandScale = num4 / 100f;
								break;
							case EM_Prop.EM_Zoom:
								emitterScale.mZoom = num4 / 100f;
								break;
							case EM_Prop.EM_Bounce:
								emitterScale.mBounceScale = num4 / 100f;
								break;
							}
							dictionary3[key] = emitterScale;
						}
						else
						{
							EmitterSettings emitterSettings = new EmitterSettings();
							switch ((EM_Prop)num3)
							{
							case EM_Prop.EM_Visibility:
								emitterSettings.mVisibility = num4;
								break;
							case EM_Prop.EM_TintStrength:
								emitterSettings.mTintStrength = num4;
								break;
							case EM_Prop.EM_EmissionAngle:
								emitterSettings.mEmissionAngle = Common.DegreesToRadians(num4);
								break;
							case EM_Prop.EM_EmissionRange:
								emitterSettings.mEmissionRange = Common.DegreesToRadians(num4);
								break;
							case EM_Prop.EM_Active:
								emitterSettings.mActive = num4 != 0f;
								break;
							case EM_Prop.EM_Angle:
								emitterSettings.mAngle = Common.DegreesToRadians(num4);
								break;
							case EM_Prop.EM_RadiusX:
								emitterSettings.mXRadius = num4;
								break;
							case EM_Prop.EM_RadiusY:
								emitterSettings.mYRadius = num4;
								break;
							}
							dictionary2[key] = emitterSettings;
						}
					}
					num = -1L;
					Dictionary<int, EmitterSettings>.Enumerator enumerator = dictionary2.GetEnumerator();
					while (enumerator.MoveNext())
					{
						emitter3.AddSettingsKeyFrame(enumerator.Current.Key, new EmitterSettings(enumerator.Current.Value));
					}
					Dictionary<int, EmitterScale>.Enumerator enumerator2 = dictionary3.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						emitter3.AddScaleKeyFrame(enumerator2.Current.Key, new EmitterScale(enumerator2.Current.Value));
					}
				}
				else if (num == PINCommon.SECTION_FREE_EMITTER)
				{
					FreeEmitter freeEmitter = new FreeEmitter();
					Dictionary<int, FreeEmitterSettings> dictionary4 = new Dictionary<int, FreeEmitterSettings>();
					Dictionary<int, FreeEmitterVariance> dictionary5 = new Dictionary<int, FreeEmitterVariance>();
					Dictionary<int, EmitterSettings> dictionary6 = new Dictionary<int, EmitterSettings>();
					Dictionary<int, EmitterScale> dictionary7 = new Dictionary<int, EmitterScale>();
					emitter = new Emitter();
					emitter.mName = buffer.ReadString().Trim();
					buffer.ReadInt32();
					buffer.ReadBoolean();
					bool flag3 = false;
					while (!flag3 && !buffer.AtEnd())
					{
						int num5 = buffer.ReadInt32();
						if (num5 == PINCommon.KEYFRAME_ENTRY_END)
						{
							continue;
						}
						if (num5 == PINCommon.SECTION_END)
						{
							flag3 = true;
							break;
						}
						float num6 = Common.StrToFloat(buffer.ReadString());
						float num7 = Common.StrToFloat(buffer.ReadString());
						buffer.ReadBoolean();
						buffer.ReadBoolean();
						Common.StrToFloat(buffer.ReadString());
						Common.StrToFloat(buffer.ReadString());
						Common.StrToFloat(buffer.ReadString());
						Common.StrToFloat(buffer.ReadString());
						if (num5 >= PINCommon.FIRST_FE_SETTING && num5 <= PINCommon.LAST_FE_SETTING)
						{
							FreeEmitterSettings freeEmitterSettings = new FreeEmitterSettings();
							switch ((FE)num5)
							{
							case FE.FE_First_Entry:
								freeEmitterSettings.mLife = (int)num7;
								break;
							case FE.FE_FNumber:
								freeEmitterSettings.mNumber = (int)num7;
								break;
							case FE.FE_FVelocity:
								freeEmitterSettings.mVelocity = (int)num7;
								break;
							case FE.FE_FWeight:
								freeEmitterSettings.mWeight = (int)num7;
								break;
							case FE.FE_FSpin:
								freeEmitterSettings.mSpin = Common.DegreesToRadians(num7);
								break;
							case FE.FE_FMotionRand:
								freeEmitterSettings.mMotionRand = (int)num7;
								break;
							case FE.FE_FBounce:
								freeEmitterSettings.mBounce = (int)num7;
								break;
							case FE.FE_FZoom:
								freeEmitterSettings.mZoom = (int)num7;
								break;
							}
							dictionary4[(int)num6] = freeEmitterSettings;
						}
						else if (num5 >= PINCommon.FIRST_FE_VARIANCE && num5 <= PINCommon.LAST_FE_VARIANCE)
						{
							FreeEmitterVariance freeEmitterVariance = new FreeEmitterVariance();
							switch ((FE)num5)
							{
							case FE.FE_FLifeVariation:
								freeEmitterVariance.mLifeVar = (int)num7;
								break;
							case FE.FE_SizeVariation:
								freeEmitterVariance.mSizeXVar = (int)num7;
								break;
							case FE.FE_SizeYVariation:
								freeEmitterVariance.mSizeYVar = (int)num7;
								break;
							case FE.FE_FVelocityVariation:
								freeEmitterVariance.mVelocityVar = (int)num7;
								break;
							case FE.FE_FWeightVariation:
								freeEmitterVariance.mWeightVar = (int)num7;
								break;
							case FE.FE_FSpinVariation:
								freeEmitterVariance.mSpinVar = Common.DegreesToRadians(num7);
								break;
							case FE.FE_FMotionRandVariation:
								freeEmitterVariance.mMotionRandVar = (int)num7;
								break;
							case FE.FE_FBounceVariation:
								freeEmitterVariance.mBounceVar = (int)num7;
								break;
							case FE.FE_FZoomVariation:
								freeEmitterVariance.mZoomVar = (int)num7;
								break;
							}
							dictionary5[(int)num6] = freeEmitterVariance;
						}
						else if (num5 >= PINCommon.FIRST_FE_LIFE && num5 <= PINCommon.LAST_FE_LIFE)
						{
							LifetimeSettings lifetimeSettings = new LifetimeSettings();
							switch ((FE)num5)
							{
							case FE.FE_NumberOverLife:
								lifetimeSettings.mNumberMult = num7 / 100f;
								break;
							case FE.FE_SizeOverLife:
								lifetimeSettings.mSizeXMult = num7 / 100f;
								break;
							case FE.FE_SizeYOverLife:
								lifetimeSettings.mSizeYMult = num7 / 100f;
								break;
							case FE.FE_FVelocityOverLife:
								lifetimeSettings.mVelocityMult = num7 / 100f;
								break;
							case FE.FE_FWeightOverLife:
								lifetimeSettings.mWeightMult = num7 / 100f;
								break;
							case FE.FE_FSpinOverLife:
								lifetimeSettings.mSpinMult = num7 / 100f;
								break;
							case FE.FE_FMotionRandOverLife:
								lifetimeSettings.mMotionRandMult = num7 / 100f;
								break;
							case FE.FE_FBounceOverLife:
								lifetimeSettings.mBounceMult = num7 / 100f;
								break;
							case FE.FE_FZoomOverLife:
								lifetimeSettings.mZoomMult = num7 / 100f;
								break;
							}
							dictionary[(int)num6] = lifetimeSettings;
						}
						else if (num5 >= PINCommon.FIRST_FE_EMITTER_SCALE && num5 <= PINCommon.LAST_FE_EMITTER_SCALE)
						{
							EmitterScale emitterScale2 = new EmitterScale();
							switch ((FE)num5)
							{
							case FE.FE_Life:
								emitterScale2.mLifeScale = num7 / 100f;
								break;
							case FE.FE_Number:
								emitterScale2.mNumberScale = num7 / 100f;
								break;
							case FE.FE_Size:
								emitterScale2.mSizeXScale = num7 / 100f;
								break;
							case FE.FE_SizeY:
								emitterScale2.mSizeYScale = num7 / 100f;
								break;
							case FE.FE_Velocity:
								emitterScale2.mVelocityScale = num7 / 100f;
								break;
							case FE.FE_Weight:
								emitterScale2.mWeightScale = num7 / 100f;
								break;
							case FE.FE_Spin:
								emitterScale2.mSpinScale = num7 / 100f;
								break;
							case FE.FE_MotionRand:
								emitterScale2.mMotionRandScale = num7 / 100f;
								break;
							case FE.FE_Bounce:
								emitterScale2.mBounceScale = num7 / 100f;
								break;
							case FE.FE_Zoom:
								emitterScale2.mZoom = num7 / 100f;
								break;
							}
							dictionary7[(int)num6] = emitterScale2;
						}
						else
						{
							EmitterSettings emitterSettings2 = new EmitterSettings();
							switch ((FE)num5)
							{
							case FE.FE_Visibility:
								emitterSettings2.mVisibility = num7;
								break;
							case FE.FE_TintStrength:
								emitterSettings2.mTintStrength = num7;
								break;
							case FE.FE_EmissionAngle:
								emitterSettings2.mEmissionAngle = num7;
								break;
							case FE.FE_EmissionRange:
								emitterSettings2.mEmissionRange = num7;
								break;
							}
							dictionary6[(int)num6] = emitterSettings2;
						}
					}
					num = -1L;
					Dictionary<int, EmitterSettings>.Enumerator enumerator3 = dictionary6.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						emitter.AddSettingsKeyFrame(enumerator3.Current.Key, new EmitterSettings(enumerator3.Current.Value));
					}
					Dictionary<int, EmitterScale>.Enumerator enumerator4 = dictionary7.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						emitter.AddScaleKeyFrame(enumerator4.Current.Key, new EmitterScale(enumerator4.Current.Value));
					}
					freeEmitter.mEmitter = emitter;
					Dictionary<float, LifetimeSettings>.Enumerator enumerator5 = dictionary.GetEnumerator();
					while (enumerator5.MoveNext())
					{
						freeEmitter.AddLifetimeKeyFrame(enumerator5.Current.Key, new LifetimeSettings(enumerator5.Current.Value));
					}
					Dictionary<int, FreeEmitterSettings>.Enumerator enumerator6 = dictionary4.GetEnumerator();
					while (enumerator6.MoveNext())
					{
						freeEmitter.AddSettingsKeyFrame(enumerator6.Current.Key, new FreeEmitterSettings(enumerator6.Current.Value));
					}
					Dictionary<int, FreeEmitterVariance>.Enumerator enumerator7 = dictionary5.GetEnumerator();
					while (enumerator7.MoveNext())
					{
						freeEmitter.AddVarianceKeyFrame(enumerator7.Current.Key, new FreeEmitterVariance(enumerator7.Current.Value));
					}
					emitter2.AddFreeEmitter(freeEmitter);
					emitter = emitter2.GetEmitter(emitter2.GetNumFreeEmitters() - 1);
				}
				else
				{
					if (num != PINCommon.SECTION_PARTICLE_TYPE)
					{
						continue;
					}
					particleType = new ParticleType();
					particleType.mName = buffer.ReadString().Trim();
					particleType.mColorKeyManager.SetColorMode(1);
					particleType.mAlphaKeyManager.SetColorMode(1);
					bool flag4 = false;
					bool flag5 = false;
					bool flag6 = false;
					bool flag7 = false;
					Dictionary<int, ParticleSettings> dictionary8 = new Dictionary<int, ParticleSettings>();
					Dictionary<int, ParticleVariance> dictionary9 = new Dictionary<int, ParticleVariance>();
					bool flag8 = false;
					while (!flag8 && !buffer.AtEnd())
					{
						int num8 = buffer.ReadInt32();
						if (num8 == PINCommon.KEYFRAME_ENTRY_END || num8 == PINCommon.MISC_PT_PROP_GRADIENT_END)
						{
							continue;
						}
						if (num8 == PINCommon.SECTION_END)
						{
							flag8 = true;
							break;
						}
						if (num8 == PINCommon.MISC_PT_PROP_REF_PT)
						{
							particleType.mRefXOff = Common.StrToInt(buffer.ReadString());
							particleType.mRefYOff = Common.StrToInt(buffer.ReadString());
							continue;
						}
						if (num8 == PINCommon.MISC_PT_PROP_SHAPE_NAME)
						{
							string text = buffer.ReadString().Trim();
							if (image_names != null)
							{
								bool flag9 = false;
								for (int i = 0; i < Common.size(image_names); i++)
								{
									if (Common.StrEquals(image_names[i], text))
									{
										flag9 = true;
										break;
									}
								}
								if (!flag9)
								{
									image_names.Add(text);
								}
							}
							particleType.mImageName = text;
							particleType.mImage = GlobalMembers.gSexyAppBase.mResourceManager.LoadImage(text).GetImage();
							if (particleType.mImage != null)
							{
								particleType.mImageSetByPINLoader = true;
							}
							continue;
						}
						if (num8 == PINCommon.MISC_PT_PROP_COLOR_GRADIENT)
						{
							float pct = Common.StrToFloat(buffer.ReadString());
							int theRed2 = buffer.ReadInt32();
							int theGreen2 = buffer.ReadInt32();
							int theBlue2 = buffer.ReadInt32();
							particleType.mColorKeyManager.AddColorKey(pct, new Color(theRed2, theGreen2, theBlue2));
							continue;
						}
						if (num8 == PINCommon.MISC_PT_PROP_ALPHA_GRADIENT)
						{
							float pct2 = Common.StrToFloat(buffer.ReadString());
							int alpha = buffer.ReadInt32();
							particleType.mAlphaKeyManager.AddAlphaKey(pct2, alpha);
							continue;
						}
						switch (num8)
						{
						case 1000:
							particleType.mLockSizeAspect = buffer.ReadBoolean();
							break;
						case 1001:
							particleType.mSingle = buffer.ReadBoolean();
							break;
						case 1002:
							particleType.mAdditive = buffer.ReadBoolean();
							break;
						case 1003:
							buffer.ReadBoolean();
							break;
						case 1004:
							flag5 = buffer.ReadBoolean();
							break;
						case 1005:
							if (flag5)
							{
								particleType.mAngleRange = Common.DegreesToRadians(buffer.ReadLong());
							}
							else
							{
								buffer.ReadInt32();
							}
							break;
						case 1006:
							if (flag5)
							{
								particleType.mInitAngle = Common.DegreesToRadians(buffer.ReadLong());
							}
							else
							{
								buffer.ReadInt32();
							}
							break;
						case 1007:
							buffer.ReadBoolean();
							break;
						case 1008:
							flag4 = (particleType.mAlignAngleToMotion = buffer.ReadBoolean());
							break;
						case 1010:
							if (flag4)
							{
								particleType.mMotionAngleOffset = Common.DegreesToRadians(buffer.ReadLong());
							}
							else
							{
								buffer.ReadInt32();
							}
							break;
						case 1009:
							if (!flag4 && !flag5)
							{
								particleType.mInitAngle = Common.DegreesToRadians(buffer.ReadLong());
							}
							else
							{
								buffer.ReadInt32();
							}
							break;
						case 1011:
							buffer.ReadBoolean();
							break;
						case 1012:
							particleType.mEmitterAttachPct = Common.StrToFloat(buffer.ReadString());
							break;
						case 1013:
							buffer.ReadInt32();
							break;
						case 1014:
							particleType.mFlipX = buffer.ReadBoolean();
							break;
						case 1015:
							particleType.mFlipY = buffer.ReadBoolean();
							break;
						case 1016:
							flag6 = buffer.ReadBoolean();
							break;
						case 1019:
						{
							bool flag10 = buffer.ReadBoolean();
							if (flag6)
							{
								particleType.mColorKeyManager.SetColorMode(flag10 ? 3 : 2);
							}
							break;
						}
						case 1017:
							if (buffer.ReadBoolean())
							{
								particleType.mColorKeyManager.SetColorMode(4);
							}
							break;
						case 1018:
						{
							bool mUpdateImagePosColor = buffer.ReadBoolean();
							particleType.mColorKeyManager.mUpdateImagePosColor = mUpdateImagePosColor;
							break;
						}
						case 1020:
							if (buffer.ReadBoolean())
							{
								particleType.mAlphaKeyManager.SetColorMode(4);
							}
							break;
						case 1021:
							particleType.mAlphaKeyManager.mUpdateImagePosColor = buffer.ReadBoolean();
							break;
						case 1022:
							flag7 = buffer.ReadBoolean();
							break;
						case 1023:
							if (flag7)
							{
								particleType.mNumSameColorKeyInRow = buffer.ReadInt32();
							}
							else
							{
								buffer.ReadInt32();
							}
							break;
						case 1024:
						{
							int num12 = buffer.ReadInt32();
							if (num12 < 1)
							{
								num12 = 1;
							}
							particleType.mColorKeyManager.mGradientRepeat = num12;
							break;
						}
						case 1025:
						{
							int num11 = buffer.ReadInt32();
							if (num11 < 1)
							{
								num11 = 1;
							}
							particleType.mAlphaKeyManager.mGradientRepeat = num11;
							break;
						}
						case 2010:
						case 2011:
						case 2012:
							buffer.ReadString();
							buffer.ReadString();
							buffer.ReadBoolean();
							buffer.ReadBoolean();
							buffer.ReadString();
							buffer.ReadString();
							buffer.ReadString();
							buffer.ReadString();
							break;
						case 2000:
						case 2001:
						case 2002:
						case 2003:
						case 2004:
						case 2005:
						case 2006:
						case 2007:
						case 2008:
						case 2009:
						case 2013:
						case 2014:
						case 2015:
						case 2016:
						case 2017:
						case 2018:
						case 2019:
						case 2020:
						case 2021:
						case 2022:
						case 2023:
						case 2024:
						case 2025:
						case 2026:
						case 2027:
						case 2028:
						case 2029:
						{
							float num9 = Common.StrToFloat(buffer.ReadString());
							float num10 = Common.StrToFloat(buffer.ReadString());
							buffer.ReadBoolean();
							buffer.ReadBoolean();
							Common.StrToFloat(buffer.ReadString());
							Common.StrToFloat(buffer.ReadString());
							Common.StrToFloat(buffer.ReadString());
							Common.StrToFloat(buffer.ReadString());
							if (num8 >= PINCommon.PT_FIRST_SETTING && num8 <= PINCommon.PT_LAST_SETTING)
							{
								ParticleSettings particleSettings = new ParticleSettings();
								switch ((PT)num8)
								{
								case PT.PT_First_Entry:
									particleSettings.mLife = (int)num10;
									break;
								case PT.PT_Number:
									particleSettings.mNumber = (int)num10;
									break;
								case PT.PT_Size:
									particleSettings.mXSize = (int)num10;
									break;
								case PT.PT_SizeY:
									particleSettings.mYSize = (int)num10;
									break;
								case PT.PT_Velocity:
									particleSettings.mVelocity = (int)num10;
									break;
								case PT.PT_Weight:
									particleSettings.mWeight = num10;
									break;
								case PT.PT_Spin:
									particleSettings.mSpin = Common.DegreesToRadians(num10);
									break;
								case PT.PT_MotionRand:
									particleSettings.mMotionRand = num10;
									break;
								case PT.PT_Bounce:
									particleSettings.mBounce = (int)num10;
									break;
								case PT.PT_Visibility:
									particleSettings.mGlobalVisibility = num10 / 100f;
									break;
								}
								dictionary8[(int)num9] = particleSettings;
							}
							else if (num8 >= PINCommon.PT_FIRST_VARIANCE && num8 <= PINCommon.PT_LAST_VARIANCE)
							{
								ParticleVariance particleVariance = new ParticleVariance();
								switch ((PT)num8)
								{
								case PT.PT_LifeVariation:
									particleVariance.mLifeVar = (int)num10;
									break;
								case PT.PT_NumberVariation:
									particleVariance.mNumberVar = (int)num10;
									break;
								case PT.PT_SizeVariation:
									particleVariance.mSizeXVar = (int)num10;
									break;
								case PT.PT_SizeYVariation:
									particleVariance.mSizeYVar = (int)num10;
									break;
								case PT.PT_VelocityVariation:
									particleVariance.mVelocityVar = (int)num10;
									break;
								case PT.PT_WeightVariation:
									particleVariance.mWeightVar = (int)num10;
									break;
								case PT.PT_SpinVariation:
									particleVariance.mSpinVar = Common.DegreesToRadians(num10);
									break;
								case PT.PT_MotionRandVariation:
									particleVariance.mMotionRandVar = num10;
									break;
								case PT.PT_BounceVariation:
									particleVariance.mBounceVar = (int)num10;
									break;
								}
								dictionary9[(int)num9] = particleVariance;
							}
							else
							{
								LifetimeSettings lifetimeSettings2 = new LifetimeSettings();
								switch ((PT)num8)
								{
								case PT.PT_SizeOverLife:
									lifetimeSettings2.mSizeXMult = num10 / 100f;
									break;
								case PT.PT_SizeYOverLife:
									lifetimeSettings2.mSizeYMult = num10 / 100f;
									break;
								case PT.PT_VelocityOverLife:
									lifetimeSettings2.mVelocityMult = num10 / 100f;
									break;
								case PT.PT_WeightOverLife:
									lifetimeSettings2.mWeightMult = num10 / 100f;
									break;
								case PT.PT_SpinOverLife:
									lifetimeSettings2.mSpinMult = num10 / 100f;
									break;
								case PT.PT_MotionRandOverLife:
									lifetimeSettings2.mMotionRandMult = num10 / 100f;
									break;
								case PT.PT_BounceOverLife:
									lifetimeSettings2.mBounceMult = num10 / 100f;
									break;
								}
							}
							break;
						}
						}
					}
					Dictionary<int, ParticleSettings>.Enumerator enumerator8 = dictionary8.GetEnumerator();
					while (enumerator8.MoveNext())
					{
						particleType.AddSettingsKeyFrame(enumerator8.Current.Key, new ParticleSettings(enumerator8.Current.Value));
					}
					Dictionary<int, ParticleVariance>.Enumerator enumerator9 = dictionary9.GetEnumerator();
					while (enumerator9.MoveNext())
					{
						particleType.AddVarianceKeyFrame(enumerator9.Current.Key, new ParticleVariance(enumerator9.Current.Value));
					}
					Dictionary<float, LifetimeSettings>.Enumerator enumerator10 = dictionary.GetEnumerator();
					while (enumerator10.MoveNext())
					{
						particleType.AddSettingAtLifePct(enumerator10.Current.Key, new LifetimeSettings(enumerator10.Current.Value));
					}
					emitter.AddParticleType(particleType);
					particleType = null;
				}
			}
			if (emitter2 != null)
			{
				AddEmitter(emitter2);
			}
			else
			{
				AddEmitter(emitter);
			}
			return true;
		}

		public void SetImageName(string image_name, Image img)
		{
			for (int i = 0; i < Common.size(mEmitters); i++)
			{
				Emitter emitter = mEmitters[i].emitter;
				for (int j = 0; j < Common.size(emitter.mParticleTypeInfo); j++)
				{
					ParticleType first = emitter.mParticleTypeInfo[j].first;
					if (Common.StrEquals(first.mImageName, image_name))
					{
						if (first.mImageSetByPINLoader)
						{
							first.mImage.Dispose();
							first.mImage = null;
						}
						first.mImage = img;
						return;
					}
				}
			}
		}

		public void Serialize(Buffer b, GlobalMembers.GetIdByImageFunc f)
		{
			b.WriteLong(mParticlePoolGrowAmt);
			b.WriteLong(mStartingParticlePoolSize);
			b.WriteFloat(mParticleScale2D);
			b.WriteLong(mEmitterIteratorIndex);
			b.WriteLong(mForceIteratorIndex);
			b.WriteLong(mDeflectorIteratorIndex);
			b.WriteLong(mUpdateCount);
			b.WriteLong(mLife);
			b.WriteLong(mMinSpawnFrame);
			b.WriteLong(mMaxParticleCount);
			b.WriteFloat(mLastX);
			b.WriteFloat(mLastY);
			b.WriteBoolean(mWaitForEmitters);
			b.WriteBoolean(mForceStopEmitting);
			b.WriteFloat(mAlphaPct);
			b.WriteFloat(mScale);
			b.WriteLong(mLastEmitterHandle);
			b.WriteLong(mForces.Count);
			for (int i = 0; i < mForces.Count; i++)
			{
				mForces[i].Serialize(b);
			}
			b.WriteLong(mDeflectors.Count);
			for (int j = 0; j < mDeflectors.Count; j++)
			{
				mDeflectors[j].Serialize(b);
			}
			b.WriteLong(mEmitters.Count);
			for (int k = 0; k < mEmitters.Count; k++)
			{
				mEmitters[k].emitter.Serialize(b, f);
				b.WriteLong(mEmitters[k].value);
				bool flag = false;
				foreach (KeyValuePair<int, Emitter> item in mEmitterHandleMap)
				{
					if (item.Value.Equals(mEmitters[k].emitter))
					{
						flag = true;
						b.WriteLong(item.Key);
						break;
					}
				}
				if (!flag)
				{
					b.WriteLong(-1L);
				}
			}
		}

		public static System Deserialize(Buffer b, GlobalMembers.GetImageByIdFunc f)
		{
			int particle_pool_grow = (int)b.ReadLong();
			int particle_pool_size = (int)b.ReadLong();
			System system = new System(particle_pool_size, particle_pool_grow);
			system.mParticleScale2D = b.ReadFloat();
			system.mEmitterIteratorIndex = (int)b.ReadLong();
			system.mForceIteratorIndex = (int)b.ReadLong();
			system.mDeflectorIteratorIndex = (int)b.ReadLong();
			system.mUpdateCount = (int)b.ReadLong();
			system.mLife = (int)b.ReadLong();
			system.mMinSpawnFrame = (int)b.ReadLong();
			system.mMaxParticleCount = (int)b.ReadLong();
			system.mLastX = b.ReadFloat();
			system.mLastY = b.ReadFloat();
			system.mWaitForEmitters = b.ReadBoolean();
			system.mForceStopEmitting = b.ReadBoolean();
			system.mAlphaPct = b.ReadFloat();
			system.mScale = b.ReadFloat();
			system.mLastEmitterHandle = (int)b.ReadLong();
			int num = (int)b.ReadLong();
			for (int i = 0; i < num; i++)
			{
				Force force = new Force();
				force.Deserialize(b);
				system.mForces.Add(force);
			}
			num = (int)b.ReadLong();
			Dictionary<int, Deflector> dictionary = new Dictionary<int, Deflector>();
			for (int j = 0; j < num; j++)
			{
				Deflector deflector = new Deflector();
				deflector.Deserialize(b);
				dictionary.Add(deflector.mSerialIndex, deflector);
				system.mDeflectors.Add(deflector);
			}
			num = (int)b.ReadLong();
			Dictionary<int, FreeEmitter> fe_ptr_map = new Dictionary<int, FreeEmitter>();
			for (int k = 0; k < num; k++)
			{
				Emitter emitter = new Emitter();
				emitter.mSystem = system;
				emitter.Deserialize(b, dictionary, fe_ptr_map, f);
				int value = (int)b.ReadLong();
				system.mEmitters.Add(new EmitterUpdatePair(emitter, value));
				int num2 = (int)b.ReadLong();
				if (num2 != -1)
				{
					system.mEmitterHandleMap.Add(num2, emitter);
				}
			}
			return system;
		}
	}
}
