using SexyFramework.Drivers.Graphics;

namespace SexyFramework.Graphics
{
	public class RenderEffectAutoState : IDisposable
	{
		protected RenderEffect mEffect;

		protected object mRunHandle;

		protected int mPassCount;

		protected int mCurrentPass;

		public RenderEffectAutoState(Graphics inGraphics, RenderEffect inEffect)
			: this(inGraphics, inEffect, 1)
		{
		}

		public RenderEffectAutoState(Graphics inGraphics)
			: this(inGraphics, null, 1)
		{
		}

		public RenderEffectAutoState()
			: this(null, null, 1)
		{
		}

		public RenderEffectAutoState(Graphics inGraphics, RenderEffect inEffect, int inDefaultPassCount)
		{
			mEffect = inEffect;
			mPassCount = inDefaultPassCount;
			mCurrentPass = 0;
			if (mEffect != null)
			{
				mPassCount = mEffect.Begin(out mRunHandle, (inGraphics != null) ? inGraphics.GetRenderContext() : new HRenderContext());
				if (mCurrentPass < mPassCount)
				{
					mEffect.BeginPass(mRunHandle, mCurrentPass);
				}
			}
		}

		public virtual void Dispose()
		{
			if (mEffect != null)
			{
				if (mCurrentPass < mPassCount)
				{
					mEffect.EndPass(mRunHandle, mCurrentPass);
				}
				mEffect.End(mRunHandle);
			}
		}

		public void Reset(Graphics inGraphics, RenderEffect inEffect)
		{
			Reset(inGraphics, inEffect, 1);
		}

		public void Reset(Graphics inGraphics)
		{
			Reset(inGraphics, null, 1);
		}

		public void Reset()
		{
			Reset(null, null, 1);
		}

		public void Reset(Graphics inGraphics, RenderEffect inEffect, int inDefaultPassCount)
		{
			if (mEffect != null)
			{
				if (mCurrentPass < mPassCount)
				{
					mEffect.EndPass(mRunHandle, mCurrentPass);
				}
				mEffect.End(mRunHandle);
			}
			mEffect = inEffect;
			mPassCount = inDefaultPassCount;
			mCurrentPass = 0;
			if (mEffect != null)
			{
				mPassCount = mEffect.Begin(out mRunHandle, (inGraphics != null) ? inGraphics.GetRenderContext() : new HRenderContext(null));
				if (mCurrentPass < mPassCount)
				{
					mEffect.BeginPass(mRunHandle, mCurrentPass);
				}
			}
		}

		public void MG_StartPass()
		{
			if (mEffect != null && mCurrentPass < mPassCount)
			{
				mEffect.BeginPass(mRunHandle, mCurrentPass);
				(mEffect as XNARenderEffect).MG_ApplyPass();
			}
		}

		public void NextPass()
		{
			if (mEffect != null && mCurrentPass < mPassCount)
			{
				mEffect.EndPass(mRunHandle, mCurrentPass);
			}
			mCurrentPass++;
			if (mEffect != null && mCurrentPass < mPassCount)
			{
				mEffect.BeginPass(mRunHandle, mCurrentPass);
			}
		}

		public bool IsDone()
		{
			return mCurrentPass >= mPassCount;
		}

		public bool PassUsesVertexShader()
		{
			if (mEffect == null)
			{
				return false;
			}
			return mEffect.PassUsesVertexShader(mCurrentPass);
		}

		public bool PassUsesPixelShader()
		{
			if (mEffect == null)
			{
				return false;
			}
			return mEffect.PassUsesPixelShader(mCurrentPass);
		}

		public static implicit operator bool(RenderEffectAutoState ImpliedObject)
		{
			return !ImpliedObject.IsDone();
		}
	}
}
