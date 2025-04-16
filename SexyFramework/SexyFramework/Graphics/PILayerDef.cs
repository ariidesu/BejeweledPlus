using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SexyFramework.Graphics
{
	public class PILayerDef : IDisposable
	{
		public string mName;

		public List<PIEmitterInstanceDef> mEmitterInstanceDefVector = new List<PIEmitterInstanceDef>();

		public List<PIDeflector> mDeflectorVector = new List<PIDeflector>();

		public List<PIBlocker> mBlockerVector = new List<PIBlocker>();

		public List<PIForce> mForceVector = new List<PIForce>();

		public PIValue2D mOffset = new PIValue2D();

		public Vector2 mOrigOffset = default(Vector2);

		public PIValue mAngle = new PIValue();

		public virtual void Dispose()
		{
			mOffset.Dispose();
			mAngle.Dispose();
			foreach (PIEmitterInstanceDef item in mEmitterInstanceDefVector)
			{
				item.Dispose();
			}
			mEmitterInstanceDefVector.Clear();
		}
	}
}
