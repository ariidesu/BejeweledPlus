using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class PILayer
	{
		public PILayerDef mLayerDef;

		public List<PIEmitterInstance> mEmitterInstanceVector = new List<PIEmitterInstance>();

		public bool mVisible;

		public Color mColor = default(Color);

		public DeviceImage mBkgImage;

		public Vector2 mBkgImgDrawOfs = default(Vector2);

		public SexyTransform2D mBkgTransform = new SexyTransform2D(false);

		public PILayer()
		{
			mVisible = true;
			mColor = new Color(Color.White);
			mBkgImage = null;
			mBkgTransform.LoadIdentity();
			mEmitterInstanceVector.Capacity = 10;
		}

		public void SetVisible(bool isVisible)
		{
			mVisible = isVisible;
		}

		public PIEmitterInstance GetEmitter()
		{
			return GetEmitter(0);
		}

		public PIEmitterInstance GetEmitter(int theIdx)
		{
			if (theIdx < mEmitterInstanceVector.Count)
			{
				return mEmitterInstanceVector[theIdx];
			}
			return null;
		}

		public PIEmitterInstance GetEmitter(string theName)
		{
			for (int i = 0; i < mEmitterInstanceVector.Count; i++)
			{
				if (theName.Length == 0 || mEmitterInstanceVector[i].mEmitterInstanceDef.mName == theName)
				{
					return mEmitterInstanceVector[i];
				}
			}
			return null;
		}
	}
}
