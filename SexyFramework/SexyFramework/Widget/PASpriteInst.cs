using System;
using System.Collections.Generic;
using SexyFramework.Graphics;

namespace SexyFramework.Widget
{
	public class PASpriteInst : IDisposable
	{
		public PASpriteInst mParent;

		public int mDelayFrames;

		public float mFrameNum;

		public int mFrameRepeats;

		public bool mOnNewFrame;

		public int mLastUpdated;

		public PATransform mCurTransform = new PATransform(true);

		public Color mCurColor = default(Color);

		public List<PAObjectInst> mChildren = new List<PAObjectInst>();

		public PASpriteDef mDef;

		public List<PAParticleEffect> mParticleEffectVector = new List<PAParticleEffect>();

		public virtual void Dispose()
		{
			for (int i = 0; i < mChildren.Count; i++)
			{
				if (mChildren[i].mSpriteInst != null)
				{
					mChildren[i].mSpriteInst.Dispose();
				}
			}
			while (mParticleEffectVector.Count > 0)
			{
				if (mParticleEffectVector[mParticleEffectVector.Count - 1].mEffect != null)
				{
					mParticleEffectVector[mParticleEffectVector.Count - 1].mEffect.Dispose();
				}
				mParticleEffectVector.RemoveAt(mParticleEffectVector.Count - 1);
			}
		}

		public PAObjectInst GetObjectInst(string theName)
		{
			string text = "";
			string theName2 = "";
			int num = theName.IndexOf('\\');
			if (num != -1)
			{
				text = theName.Substring(0, num);
				theName2 = theName.Substring(num + 1);
			}
			else
			{
				text = theName;
			}
			for (int i = 0; i < mChildren.Count; i++)
			{
				PAObjectInst pAObjectInst = mChildren[i];
				if (pAObjectInst.mName != null && pAObjectInst.mName == text)
				{
					if (num == -1)
					{
						return pAObjectInst;
					}
					if (pAObjectInst.mSpriteInst == null)
					{
						return null;
					}
					return pAObjectInst.mSpriteInst.GetObjectInst(theName2);
				}
			}
			return null;
		}
	}
}
