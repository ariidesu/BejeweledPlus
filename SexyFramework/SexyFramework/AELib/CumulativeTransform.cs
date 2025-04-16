using System;
using SexyFramework.Misc;

namespace SexyFramework.AELib
{
	public class CumulativeTransform : IDisposable
	{
		public float mOpacity = 1f;

		public bool mForceAdditive;

		public SexyTransform2D mTrans;

		public CumulativeTransform()
		{
			mTrans = new SexyTransform2D(true);
		}

		public CumulativeTransform(CumulativeTransform rhs)
		{
			CopyFrom(rhs);
		}

		public virtual void Dispose()
		{
		}

		public void CopyFrom(CumulativeTransform other)
		{
			mOpacity = other.mOpacity;
			mForceAdditive = other.mForceAdditive;
			mTrans = other.mTrans;
		}

		public void Reset()
		{
			mOpacity = 1f;
			mForceAdditive = false;
			mTrans.LoadIdentity();
		}
	}
}
