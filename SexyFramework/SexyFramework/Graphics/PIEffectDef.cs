using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class PIEffectDef
	{
		public int mRefCount;

		public List<PIEmitter> mEmitterVector = new List<PIEmitter>();

		public List<PITexture> mTextureVector = new List<PITexture>();

		public List<PILayerDef> mLayerDefVector = new List<PILayerDef>();

		public Dictionary<int, int> mEmitterRefMap = new Dictionary<int, int>();

		public PIEffectDef()
		{
			mRefCount = 1;
		}

		public virtual void Dispose()
		{
			for (int i = 0; i < mEmitterVector.Count; i++)
			{
				mEmitterVector[i] = null;
			}
			for (int j = 0; j < mTextureVector.Count; j++)
			{
				mTextureVector[j] = null;
			}
			mEmitterVector.Clear();
			mTextureVector.Clear();
		}
	}
}
