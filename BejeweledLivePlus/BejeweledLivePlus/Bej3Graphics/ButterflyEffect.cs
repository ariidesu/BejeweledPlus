using System;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class ButterflyEffect : Effect
	{
		public CurvedVal mTargetX = new CurvedVal();

		public CurvedVal mTargetY = new CurvedVal();

		public ParticleEffect mSparkles;

		public int mColorIdx;

		public float mFlap;

		public float mAccel;

		public ButterflyBoard mOwner;

		private static SimpleObjectPool thePool_;

		public ButterflyEffect()
			: base(Type.TYPE_CUSTOMCLASS)
		{
		}

		public void init(Piece thePiece, ButterflyBoard Owner)
		{
			init(Type.TYPE_CUSTOMCLASS);
			mX = thePiece.GetScreenX();
			mY = thePiece.GetScreenY();
			mColorIdx = thePiece.mColor;
			mDAlpha = 0f;
			mFlap = 0f;
			mAccel = 0f;
			mCurvedAlpha.SetConstant(1.0);
			mOwner = Owner;
			mOwner.mCurrentReleasedButterflies++;
			mSparkles = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_BUTTERFLY);
			mSparkles.SetEmitAfterTimeline(true);
			mSparkles.mDoDrawTransform = false;
			mSparkles.SetEmitterTint(0, 0, GlobalMembers.gGemColors[mColorIdx]);
			mSparkles.SetEmitterTint(0, 1, GlobalMembers.gGemColors[mColorIdx]);
			mSparkles.mDoubleSpeed = true;
			GlobalMembers.gApp.mBoard.mPostFXManager.AddEffect(mSparkles);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_TARGET_X_BUTTERFLY, mTargetX);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_TARGET_Y_BUTTERFLY, mTargetY);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_ALPHA_BUTTERFLY, mCurvedAlpha);
		}

		public override void Dispose()
		{
			mOwner.mCurrentReleasedButterflies--;
			mSparkles.Stop();
			base.Dispose();
		}

		public override void Update()
		{
			base.Update();
			mX += (float)(((double)mTargetX - (double)mX) * (double)mAccel);
			mY += (float)(((double)mTargetY - (double)mY) * (double)mAccel);
			mFlap += (((int)mX == (int)(double)mTargetX && (int)mY == (int)(double)mTargetY) ? GlobalMembers.M(0.05f) : GlobalMembers.M(0.4f));
			if ((double)mFlap >= Math.PI * 2.0)
			{
				mFlap -= (float)Math.PI * 2f;
			}
			mAccel = Math.Min(mAccel + GlobalMembers.M(0.0002f), 1f);
			mSparkles.mX = mX + 50f;
			mSparkles.mY = mY + 50f;
			if ((mX > 1900f || mCurvedAlpha.HasBeenTriggered()) && !mDeleteMe)
			{
				int num = (int)mSparkles.mX;
				int num2 = (int)mSparkles.mY;
				ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_BUTTERFLY_CREATE);
				particleEffect.SetEmitterTint(0, 0, GlobalMembers.gGemColors[mColorIdx]);
				particleEffect.SetEmitterTint(0, 1, GlobalMembers.gGemColors[mColorIdx]);
				particleEffect.SetEmitterTint(0, 2, GlobalMembers.gGemColors[mColorIdx]);
				particleEffect.mDoubleSpeed = true;
				particleEffect.mX = num;
				particleEffect.mY = num2;
				GlobalMembers.gApp.mBoard.mPostFXManager.AddEffect(particleEffect);
				mDeleteMe = true;
			}
			mTargetX.IncInVal();
			mTargetY.IncInVal();
			int mCurrentReleasedButterfly = mOwner.mCurrentReleasedButterflies;
			int num3 = 0;
		}

		public override void Draw(Graphics g)
		{
			float sx = (float)(Math.Cos(mFlap) * 0.25 + 0.75);
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColorizeImages(true);
			float num = (float)((double)mCurvedAlpha * 255.0);
			if (GlobalMembers.gApp.mBoard != null)
			{
				num *= GlobalMembers.gApp.mBoard.GetAlpha();
			}
			g.SetColor(new Color(255, 255, 255, (int)num));
			g.DrawImageCel(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_SHADOW, (int)GlobalMembers.S(mX), (int)GlobalMembers.S(mY), mColorIdx);
			Transform transform = new Transform();
			transform.Translate(GlobalMembers.S(ConstantsWP.BUTTERFLY_DRAW_OFFSET_1), 0f);
			transform.Scale(sx, 1f);
			g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, transform, GlobalMembers.IMGSRCRECT(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, mColorIdx), GlobalMembers.S(mX + (float)ConstantsWP.BUTTERFLY_DRAW_OFFSET_2), GlobalMembers.S(mY + (float)ConstantsWP.BUTTERFLY_DRAW_OFFSET_3));
			transform.Scale(-1f, 1f);
			g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, transform, GlobalMembers.IMGSRCRECT(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_WINGS, mColorIdx), GlobalMembers.S(mX + (float)ConstantsWP.BUTTERFLY_DRAW_OFFSET_4), GlobalMembers.S(mY + (float)ConstantsWP.BUTTERFLY_DRAW_OFFSET_3));
			g.DrawImageCel(GlobalMembersResourcesWP.IMAGE_BUTTERFLY_BODY, (int)GlobalMembers.S(mX), (int)GlobalMembers.S(mY), mColorIdx);
			g.SetColor(Color.White);
			g.SetColorizeImages(false);
		}

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(ButterflyEffect));
		}

		public static ButterflyEffect alloc(Piece thePiece, ButterflyBoard owner)
		{
			ButterflyEffect butterflyEffect = (ButterflyEffect)thePool_.alloc();
			butterflyEffect.init(thePiece, owner);
			return butterflyEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}
	}
}
