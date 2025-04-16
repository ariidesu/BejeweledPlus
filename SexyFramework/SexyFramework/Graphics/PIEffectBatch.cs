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
					}
				}
			}
			for (int k = 0; k < mPIEffectList.Count; k++)
			{
				PIEffect pIEffect2 = mPIEffectList[k];
				if (!pIEffect2.mInUse)
				{
					continue;
				}
				for (int l = 0; l < pIEffect2.mDef.mLayerDefVector.Count; l++)
				{
					PILayer pILayer2 = pIEffect2.mLayerVector[l];
					if (pILayer2.mVisible)
					{
						pIEffect2.DrawLayerNormal(g, pILayer2);
					}
				}
			}
			for (int m = 0; m < mPIEffectList.Count; m++)
			{
				PIEffect pIEffect3 = mPIEffectList[m];
				if (!pIEffect3.mInUse)
				{
					continue;
				}
				for (int n = 0; n < pIEffect3.mDef.mLayerDefVector.Count; n++)
				{
					PILayer pILayer3 = pIEffect3.mLayerVector[n];
					if (pILayer3.mVisible)
					{
						pIEffect3.DrawLayerAdditive(g, pILayer3);
					}
				}
			}
			for (int num = 0; num < mPIEffectList.Count; num++)
			{
				PIEffect pIEffect4 = mPIEffectList[num];
				if (!pIEffect4.mInUse)
				{
					continue;
				}
				for (int num2 = 0; num2 < pIEffect4.mDef.mLayerDefVector.Count; num2++)
				{
					PILayer pILayer4 = pIEffect4.mLayerVector[num2];
					if (pILayer4.mVisible)
					{
						pIEffect4.DrawPhisycalLayer(g, pILayer4);
					}
				}
			}
		}
	}
}
