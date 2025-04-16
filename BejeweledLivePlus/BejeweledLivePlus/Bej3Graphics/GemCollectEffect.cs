using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class GemCollectEffect : ParticleEffect
	{
		public int mTargetX;

		public int mTargetY;

		public int mOX;

		public int mOY;

		public CurvedVal mTravelPct;

		public CurvedVal mArc;

		private static SimpleObjectPool thePool_;

		public void initWith(Piece thePiece, PIEffect theEffect, int theTargetX, int theTargetY)
		{
			initWithPIEffect(theEffect);
			SetEmitAfterTimeline(true);
			mTargetX = theTargetX;
			mTargetY = theTargetY;
			mOX = (int)thePiece.CX() - mTargetX;
			mOY = (int)thePiece.CY() - mTargetY;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_TRAVEL_PCT_GEM_COLLECT, mTravelPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_ARC_GEM_COLLECT, mArc, mTravelPct);
		}

		public override void Update()
		{
			base.Update();
			mX = (float)mTargetX + (float)mOX * (float)(double)mTravelPct;
			mY = (float)mTargetY + (float)mOY * (float)(double)mTravelPct - (float)(double)mArc;
			if (mTravelPct.HasBeenTriggered())
			{
				Stop();
			}
		}

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(4096, typeof(GemCollectEffect));
		}

		public static GemCollectEffect from(Piece thePiece, PIEffect theEffect, int theTargetX, int theTargetY)
		{
			GemCollectEffect gemCollectEffect = (GemCollectEffect)thePool_.alloc();
			gemCollectEffect.initWith(thePiece, theEffect, theTargetX, theTargetY);
			return gemCollectEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}
	}
}
