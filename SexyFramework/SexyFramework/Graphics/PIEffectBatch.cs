using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class PIEffectBatch
	{
		public List<PIEffect> mPIEffectList;

		public PIEffectBatch()
		{
			mPIEffectList = new List<PIEffect>();
		}

		public void AddEffect(PIEffect item)
		{
			mPIEffectList.Add(item);
		}

		public void Clear()
		{
			mPIEffectList.Clear();
		}

		public void RemoveAt(int index)
		{
			mPIEffectList.RemoveAt(index);
		}

		public void Remove(PIEffect effect)
		{
			mPIEffectList.Remove(effect);
		}

		public void DrawBatch(Graphics g)
		{
			for (int i = 0; i < mPIEffectList.Count; i++)
			{
				PIEffect pIEffect = mPIEffectList[i];
				if (!pIEffect.mInUse)
				{
					continue;
				}
				for (int j = 0; j < pIEffect.mDef.mLayerDefVector.Count; j++)
				{
					PILayer pILayer = pIEffect.mLayerVector[j];
					if (pILayer.mVisible)
					{
						pIEffect.DrawLayer(g, pILayer);
                        pIEffect.DrawPhisycalLayer(g, pILayer);
                    }
				}
			}
		}
	}
}
