using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.AELib
{
	public class Composition : Layer
	{
		protected List<CompLayer> mLayers = new List<CompLayer>();

		protected int mMaxDuration;

		public AECommon.LoadCompImageFunc mLoadImageFunc = DefaultLoadImageFunc;

		public AECommon.PostLoadCompImageFunc mPostLoadImageFunc = DefaultPostLoadImageFunc;

		public AECommon.PreLayerDrawFunc mPreLayerDrawFunc;

		public object mPreLayerDrawData;

		public int mUpdateCount;

		public bool mLoop;

		public int mMaxFrame = -1;

		public Composition()
		{
		}

		public Composition(Composition other)
		{
			CopyFrom(other);
		}

		public void CopyFrom(Composition other)
		{
			CopyFrom((Layer)other);
			CopyLayersFrom(other);
			mMaxDuration = other.mMaxDuration;
			mLoadImageFunc = other.mLoadImageFunc;
			mPostLoadImageFunc = other.mPostLoadImageFunc;
			mPreLayerDrawFunc = other.mPreLayerDrawFunc;
			mPreLayerDrawData = other.mPreLayerDrawData;
			mUpdateCount = other.mUpdateCount;
			mLoop = other.mLoop;
			mMaxFrame = other.mMaxFrame;
		}

		public void CopyLayersFrom(Composition other)
		{
			mLayers.Clear();
			if (other.mLayers != null)
			{
				for (int i = 0; i < other.mLayers.Count; i++)
				{
					CompLayer compLayer = other.mLayers[i];
					mLayers.Add(new CompLayer(compLayer.mSource.Duplicate(), compLayer.mStartFrameOnComp, compLayer.mDuration, compLayer.mLayerOffsetStart));
				}
			}
		}

		public override Layer Duplicate()
		{
			return new Composition(this);
		}

		public bool Done()
		{
			return mUpdateCount >= mMaxDuration;
		}

		public override bool isValid()
		{
			foreach (CompLayer mLayer in mLayers)
			{
				if (!mLayer.mSource.isValid())
				{
					return false;
				}
			}
			return true;
		}

		public bool LoadFromFile(string file_name)
		{
			return LoadFromFile(file_name, "Main");
		}

		public bool LoadFromFile(string file_name, string main_composition_name)
		{
			List<Composition> list = new List<Composition>();
			if (!AECommon.LoadPAX(file_name, list, mLoadImageFunc, mPostLoadImageFunc) || list.Count == 0)
			{
				return false;
			}
			Composition composition = null;
			for (int i = 0; i < list.Count; i++)
			{
				Composition composition2 = list[i];
				if (composition2.mLayerName.ToLower().Equals(main_composition_name.ToLower()))
				{
					composition = composition2;
					break;
				}
			}
			if (composition == null)
			{
				composition = list[0];
			}
			CopyFrom(composition);
			return true;
		}

		public void AddLayer(CompLayer c)
		{
			mLayers.Add(new CompLayer(c));
		}

		public void AddLayer(Layer l, int start_frame, int duration, int layer_offset)
		{
			mLayers.Add(new CompLayer(l, start_frame, duration, layer_offset));
		}

		public void Update()
		{
			mUpdateCount++;
			if (mMaxFrame == -1)
			{
				mMaxFrame = mMaxDuration;
			}
			if (mLoop && mUpdateCount >= mMaxFrame)
			{
				Reset();
				mUpdateCount = 1;
			}
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			Draw(g, null);
		}

		public override void Draw(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans)
		{
			Draw(g, ctrans, -1);
		}

		public override void Draw(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans, int frame)
		{
			Draw(g, ctrans, frame, 1f);
		}

		public override void Draw(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans, int frame, float scale)
		{
			if (frame == -1)
			{
				frame = mUpdateCount;
			}
			bool flag = false;
			for (int num = mLayers.Count - 1; num >= 0; num--)
			{
				CompLayer compLayer = mLayers[num];
				if (frame >= compLayer.mStartFrameOnComp && frame < compLayer.mStartFrameOnComp + compLayer.mDuration)
				{
					int frame2 = frame - compLayer.mStartFrameOnComp + compLayer.mLayerOffsetStart;
					CumulativeTransform cumulativeTransform = ctrans;
					CumulativeTransform cumulativeTransform2 = new CumulativeTransform();
					if (ctrans == null)
					{
						ctrans = cumulativeTransform2;
					}
					else if (!flag)
					{
						flag = true;
						float value = 1f;
						float value2 = 0f;
						mOpacity.GetValue(frame, ref value);
						ctrans.mOpacity *= value;
						SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
						mAnchorPoint.GetValue(frame, ref value, ref value2);
						float num2 = value * scale;
						float num3 = value2 * scale;
						sexyTransform2D.Translate(0f - num2, 0f - num3);
						float value3 = 1f;
						float value4 = 1f;
						mScale.GetValue(frame, ref value3, ref value4);
						sexyTransform2D.Scale(value3, value4);
						mRotation.GetValue(frame, ref value);
						if (value != 0f)
						{
							sexyTransform2D.RotateRad(0f - value);
						}
						mPosition.GetValue(frame, ref value, ref value2);
						sexyTransform2D.Translate(value * scale, value2 * scale);
						ctrans.mTrans *= sexyTransform2D;
					}
					if (compLayer.mSource.IsLayerBase() && mPreLayerDrawFunc != null)
					{
						mPreLayerDrawFunc(g, compLayer.mSource, mPreLayerDrawData);
					}
					CumulativeTransform other = new CumulativeTransform(ctrans);
					if (mAdditive)
					{
						ctrans.mForceAdditive = true;
					}
					if (compLayer.mSource.NeedsTranslatedFrame())
					{
						compLayer.mSource.Draw(g, ctrans, frame2, scale);
					}
					else
					{
						compLayer.mSource.Draw(g, ctrans, frame, scale);
					}
					ctrans.CopyFrom(other);
					ctrans = cumulativeTransform;
				}
			}
		}

		public override void Reset()
		{
			mUpdateCount = 0;
			for (int i = 0; i < mLayers.Count; i++)
			{
				mLayers[i].mSource.Reset();
			}
		}

		public Layer GetLayerAtIdx(int idx)
		{
			return mLayers[idx].mSource;
		}

		public override bool NeedsTranslatedFrame()
		{
			return true;
		}

		public int GetMaxDuration()
		{
			return mMaxDuration;
		}

		public int GetUpdateCount()
		{
			return mUpdateCount;
		}

		public int GetNumLayers()
		{
			return mLayers.Count;
		}

		public override bool IsLayerBase()
		{
			return false;
		}

		public void SetMaxDuration(int m)
		{
			mMaxDuration = m;
		}

		public static void DefaultPostLoadImageFunc(SharedImageRef img, Layer l)
		{
		}

		public static SharedImageRef DefaultLoadImageFunc(string file_dir, string file_name)
		{
			return GlobalMembers.gSexyApp.GetSharedImage(file_dir + file_name);
		}

		public void Dispose()
		{
		}
	}
}
