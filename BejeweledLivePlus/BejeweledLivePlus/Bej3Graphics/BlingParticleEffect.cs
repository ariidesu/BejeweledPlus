using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class BlingParticleEffect : ParticleEffect
	{
		public new int mPieceId;

		public bool mActive;

		private static SimpleObjectPool thePool_;

		public void initWithPIEffectAndPieceId(PIEffect thePIEffect, int thePieceId)
		{
			initWithPIEffect(thePIEffect);
			mPieceId = thePieceId;
			SetEmitAfterTimeline(true);
			mActive = true;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override void Update()
		{
			Piece pieceById = mFXManager.mBoard.GetPieceById(mPieceId);
			if (pieceById != null)
			{
				mX = pieceById.CX();
				mY = pieceById.CY();
			}
			else
			{
				Stop();
			}
			base.Update();
		}

		public void SetActive(bool theVal)
		{
			if (mActive == theVal)
			{
				return;
			}
			mActive = theVal;
			for (int i = 0; i < mPIEffect.mLayerVector.Count; i++)
			{
				PILayer layer = mPIEffect.GetLayer(i);
				for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
				{
					PIEmitterInstance pIEmitterInstance = layer.mEmitterInstanceVector[j];
					pIEmitterInstance.mNumberScale = (mActive ? 1f : 0f);
				}
			}
		}

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(4096, typeof(BlingParticleEffect));
		}

		public static BlingParticleEffect fromPIEffectAndPieceId(PIEffect thePIEffect, int thePieceId)
		{
			BlingParticleEffect blingParticleEffect = (BlingParticleEffect)thePool_.alloc();
			blingParticleEffect.initWithPIEffectAndPieceId(thePIEffect, thePieceId);
			return blingParticleEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}
	}
}
