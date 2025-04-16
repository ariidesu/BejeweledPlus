using System.Collections.Generic;

namespace SexyFramework.Widget
{
	public class PopAnimDef
	{
		public PASpriteDef mMainSpriteDef;

		public List<PASpriteDef> mSpriteDefVector = new List<PASpriteDef>();

		public LinkedList<string> mObjectNamePool = new LinkedList<string>();

		public int mRefCount;

		public PopAnimDef()
		{
			mRefCount = 0;
			mMainSpriteDef = null;
		}

		public void Dispose()
		{
			if (mMainSpriteDef != null)
			{
				mMainSpriteDef.Dispose();
				mMainSpriteDef = null;
			}
		}
	}
}
