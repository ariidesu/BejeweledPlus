using System.Collections.Generic;
using SexyFramework.Graphics;

namespace SexyFramework.AELib
{
	public class CompositionMgr
	{
		protected Dictionary<string, Composition> mCompositions = new Dictionary<string, Composition>();

		public AECommon.LoadCompImageFunc mLoadImageFunc = Composition.DefaultLoadImageFunc;

		public AECommon.PostLoadCompImageFunc mPostLoadImageFunc = Composition.DefaultPostLoadImageFunc;

		public AECommon.PreLayerDrawFunc mPreLayerDrawFunc;

		public CompositionMgr()
		{
		}

		public CompositionMgr(CompositionMgr other)
		{
			CopyFrom(other);
		}

		public bool isValid()
		{
			foreach (Composition value in mCompositions.Values)
			{
				if (!value.isValid())
				{
					return false;
				}
			}
			return true;
		}

		public void CopyFrom(CompositionMgr other)
		{
			mLoadImageFunc = other.mLoadImageFunc;
			mPostLoadImageFunc = other.mPostLoadImageFunc;
			mPreLayerDrawFunc = other.mPreLayerDrawFunc;
			mCompositions = new Dictionary<string, Composition>();
			foreach (KeyValuePair<string, Composition> mComposition in other.mCompositions)
			{
				mCompositions.Add(mComposition.Key, new Composition(mComposition.Value));
			}
		}

		public bool LoadFromFile(string file_name)
		{
			List<Composition> list = new List<Composition>();
			if (!AECommon.LoadPAX(file_name, list, mLoadImageFunc, mPostLoadImageFunc))
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				string key = list[i].mLayerName.ToLower();
				if (!mCompositions.ContainsKey(key))
				{
					mCompositions.Add(key, null);
				}
				mCompositions[key] = new Composition(list[i]);
			}
			return true;
		}

		public void UpdateAll()
		{
			foreach (KeyValuePair<string, Composition> mComposition in mCompositions)
			{
				mComposition.Value.Update();
			}
		}

		public void Update(string comp_name)
		{
			if (mCompositions.ContainsKey(comp_name))
			{
				mCompositions[comp_name].Update();
			}
		}

		public void DrawAll(SexyFramework.Graphics.Graphics g)
		{
			DrawAll(g, null);
		}

		public void DrawAll(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans)
		{
			DrawAll(g, ctrans, 1f);
		}

		public void DrawAll(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans, float scale)
		{
			foreach (KeyValuePair<string, Composition> mComposition in mCompositions)
			{
				mComposition.Value.Draw(g, ctrans, -1, scale);
			}
		}

		public void Draw(SexyFramework.Graphics.Graphics g, string comp_name)
		{
			Draw(g, comp_name, null);
		}

		public void Draw(SexyFramework.Graphics.Graphics g, string comp_name, CumulativeTransform ctrans)
		{
			Draw(g, comp_name, ctrans, -1);
		}

		public void Draw(SexyFramework.Graphics.Graphics g, string comp_name, CumulativeTransform ctrans, int frame)
		{
			Draw(g, comp_name, ctrans, frame, 1f);
		}

		public void Draw(SexyFramework.Graphics.Graphics g, string comp_name, CumulativeTransform ctrans, int frame, float scale)
		{
			if (mCompositions.ContainsKey(comp_name.ToLower()))
			{
				mCompositions[comp_name.ToLower()].Draw(g, ctrans, frame, scale);
			}
		}

		public Composition GetComposition(string comp_name)
		{
			if (mCompositions.ContainsKey(comp_name.ToLower()))
			{
				return mCompositions[comp_name.ToLower()];
			}
			return null;
		}

		public void GetListOfComps(List<string> comp_names)
		{
			foreach (KeyValuePair<string, Composition> mComposition in mCompositions)
			{
				comp_names.Add(mComposition.Key);
			}
			comp_names.Sort();
		}

		public void GetAllCompositions(List<Composition> comps)
		{
			foreach (KeyValuePair<string, Composition> mComposition in mCompositions)
			{
				comps.Add(mComposition.Value);
			}
		}
	}
}
