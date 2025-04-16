using System;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class CoinFlyEffect : Effect
	{
		private static SimpleObjectPool thePool_;

		public float mRotPct;

		public float mOrigX;

		public float mOrigY;

		public int mToX;

		public int mToY;

		public CurvedVal mTransPct = new CurvedVal();

		public CurvedVal mSinkPct = new CurvedVal();

		public new float mScale;

		public new Image mImage;

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(CoinFlyEffect));
		}

		public static CoinFlyEffect alloc(Point theFromCoord, Point theToCoord)
		{
			return alloc(theFromCoord, theToCoord, 1f);
		}

		public static CoinFlyEffect alloc(Point theFromCoord, Point theToCoord, float theScale)
		{
			CoinFlyEffect coinFlyEffect = (CoinFlyEffect)thePool_.alloc();
			coinFlyEffect.init(theFromCoord, theToCoord, theScale);
			return coinFlyEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}

		public CoinFlyEffect()
			: base(Type.TYPE_CUSTOMCLASS)
		{
		}

		public void init(Point theFromCoord, Point theToCoord, float theScale)
		{
			init(Type.TYPE_CUSTOMCLASS);
			mDAlpha = 0f;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_TRANS_PCT_COIN_FLY, mTransPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_SCALE_COIN_FLY, mCurvedScale, mTransPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_SINK_PCT_COIN_FLY, mSinkPct, mTransPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_ALPHA_COIN_FLY, mCurvedAlpha, mTransPct);
			mX = (mOrigX = theFromCoord.mX);
			mY = (mOrigY = theFromCoord.mY);
			mToX = theToCoord.mX;
			mToY = theToCoord.mY;
			mRotPct = 0f;
			mScale = theScale;
		}

		public CoinFlyEffect(int theCounter, Piece thePiece)
			: base(Type.TYPE_CUSTOMCLASS)
		{
			mDAlpha = 0f;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_TRANS_PCT_COIN_FLY, mTransPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_SCALE_COIN_FLY, mCurvedScale, mTransPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_SINK_PCT_COIN_FLY, mSinkPct, mTransPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_ALPHA_COIN_FLY, mCurvedAlpha, mTransPct);
			mToX = 240;
			mToY = 570;
			mScale = 1f;
			if (thePiece == null)
			{
				mX = (mOrigX = 240f);
				mY = (mOrigY = 260f);
				mRotPct = 0f;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_SINK_PCT_COIN_FLY_NULL, mSinkPct, mTransPct);
			}
			else
			{
				mX = (mOrigX = thePiece.CX());
				mY = (mOrigY = thePiece.CY());
				mRotPct = thePiece.mRotPct;
			}
		}

		public override void Update()
		{
			base.Update();
			mX = mOrigX - (float)(double)mTransPct * (mOrigX - (float)mToX);
			mY = mOrigY - (float)(double)mTransPct * (mOrigY - (float)mToY);
			mRotPct += GlobalMembers.M(0.04f);
			if (mRotPct >= 1f)
			{
				mRotPct -= 1f;
			}
			mTransPct.IncInVal();
			mSinkPct.IncInVal();
			if (mTransPct.HasBeenTriggered())
			{
				mDeleteMe = true;
				mFXManager.mBoard.DepositCoin();
			}
		}

		public override void Draw(Graphics g)
		{
			Transform transform = new Transform();
			transform.Scale((float)(double)mCurvedScale * mScale, (float)(double)mCurvedScale * mScale);
			int frame = Math.Min((int)(mRotPct * 20f), mImage.mNumCols * mImage.mNumRows - 1);
			g.PushState();
			g.SetColorizeImages(true);
			int num = (int)(96.0 * (double)mCurvedAlpha);
			g.SetDrawMode(Graphics.DrawMode.Additive);
			g.SetColor(new Color(num, num, num));
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColor(Color.White);
			g.mColor.mAlpha = (int)(255.0 * (double)mCurvedAlpha);
			if (GlobalMembers.gApp.mBoard != null)
			{
				g.mColor.mAlpha = (int)GlobalMembers.gApp.mBoard.GetAlpha();
			}
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.DrawImageTransform(mImage, transform, GlobalMembers.IMGSRCRECT(mImage, frame), GlobalMembers.S(mX), GlobalMembers.S(mY) + (float)((double)GlobalMembers.MS(150) * (double)mSinkPct));
			g.PopState();
		}
	}
}
