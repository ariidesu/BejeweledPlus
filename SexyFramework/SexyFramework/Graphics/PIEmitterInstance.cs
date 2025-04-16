using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class PIEmitterInstance : PIEmitterBase
	{
		public PIEmitterInstanceDef mEmitterInstanceDef;

		public bool mWasActive;

		public bool mWithinLifeFrame;

		public List<PIParticleDefInstance> mSuperEmitterParticleDefInstanceVector = new List<PIParticleDefInstance>();

		public PIParticleGroup mSuperEmitterGroup = new PIParticleGroup();

		public Color mTintColor = default(Color);

		public SharedImageRef mMaskImage = new SharedImageRef();

		public SexyTransform2D mTransform = new SexyTransform2D(false);

		public Vector2 mOffset = default(Vector2);

		public float mNumberScale;

		public bool mVisible;

		public PIEmitterInstance()
		{
			mWasActive = false;
			mWithinLifeFrame = true;
			mSuperEmitterGroup.mIsSuperEmitter = true;
			mTransform.LoadIdentity();
			mNumberScale = 1f;
			mVisible = true;
		}

		public void SetVisible(bool isVisible)
		{
			mVisible = isVisible;
		}
	}
}
