using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class CardGemEffect : Effect
	{
		private sealed class Gem
		{
			public float mX;
			public float mY;
			public float mDX;
			public float mDY;
			public ParticleEffect mEffect;
		}

		private static SimpleObjectPool thePool_;

		private readonly List<Gem> mGems = new List<Gem>();

		public int mTargetX;

		public int mTargetY;

		public float mOriginX;

		public float mOriginY;

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(CardGemEffect));
		}

		public static CardGemEffect alloc(Piece thePiece, int theTargetX, int theTargetY)
		{
			CardGemEffect cardGemEffect = (CardGemEffect)thePool_.alloc();
			cardGemEffect.init(thePiece, theTargetX, theTargetY);
			return cardGemEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}

		public CardGemEffect()
			: base(Type.TYPE_CUSTOMCLASS)
		{
		}

		public void init(Piece thePiece, int theTargetX, int theTargetY)
		{
			init(Type.TYPE_CUSTOMCLASS);
			mTargetX = theTargetX;
			mTargetY = theTargetY;
			mOriginX = thePiece.CX() - theTargetX;
			mOriginY = thePiece.CY() - theTargetY;
			mGemType = thePiece.mColor;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_SCALE_CARD_GEM,
				mCurvedScale);
			mGems.Clear();
		}

		public void AddGem(Piece thePiece)
		{
			Color[] colors =
			{
				new Color(255, 255, 255),
				new Color(255, 128, 128),
				new Color(255, 255, 255),
				new Color(128, 255, 128),
				new Color(255, 255, 128),
				new Color(255, 128, 255),
				new Color(255, 192, 128),
				new Color(128, 192, 255)
			};
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_CARD_GEM_SPARKLE);
			particleEffect.SetEmitAfterTimeline(true);
			particleEffect.SetEmitterTint(0, 0, colors[thePiece.mColor + 1]);
			mFXManager.AddEffect(particleEffect);
			mGems.Add(new Gem { mEffect = particleEffect });
		}

		public override void Update()
		{
			for (int i = 0; i < mGems.Count; i++)
			{
				Gem gem = mGems[i];
				gem.mDX += GlobalMembersUtils.GetRandFloat();
				gem.mDY += GlobalMembersUtils.GetRandFloat();
				gem.mX += gem.mDX;
				gem.mY += gem.mDY;
				float travel = (float)mCurvedScale.GetOutVal();
				gem.mEffect.mX = mTargetX + (mOriginX + gem.mX) * travel;
				gem.mEffect.mY = mTargetY + (mOriginY + gem.mY) * travel;
			}
			if (mCurvedScale.HasBeenTriggered())
			{
				for (int i = 0; i < mGems.Count; i++)
				{
					mGems[i].mEffect.SetEmitAfterTimeline(false);
				}
				mDeleteMe = true;
			}
		}

		public override void Draw(Graphics g)
		{
			CurvedVal scaleCurve = new CurvedVal();
			CurvedVal glowCurve = new CurvedVal();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_GEM_SCALE_CARD_GEM,
				scaleCurve, mCurvedScale);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_GLOW_CARD_GEM,
				glowCurve, mCurvedScale);
			Transform transform = new Transform();
			float scale = (float)scaleCurve.GetOutVal();
			transform.Scale(scale, scale);
			for (int i = 0; i < mGems.Count; i++)
			{
				Gem gem = mGems[i];
				float travel = (float)mCurvedScale.GetOutVal();
				float x = GlobalMembers.S(mTargetX) + GlobalMembers.S(mOriginX + gem.mX) * travel;
				float y = GlobalMembers.S(mTargetY) + GlobalMembers.S(mOriginY + gem.mY) * travel;
				if (mGemType < 0)
				{
					g.PushState();
					g.SetColorizeImages(true);
					g.SetColor(mColor);
					g.DrawImageTransformF(GlobalMembersResourcesWP.IMAGE_CARDS_WILD, transform, x, y);
					g.PopState();
				}
				else
				{
					Image gemImage = GlobalMembersResourcesWP.GetImageById((int)ResourceId.IMAGE_GEMS_RED_ID + mGemType);
					g.SetDrawMode(Graphics.DrawMode.Normal);
					g.SetColorizeImages(false);
					g.DrawImageTransformF(gemImage, transform, GlobalMembersResourcesWP.IMAGE_GEMS_BLUE.GetCelRect(0), x, y);
					int glow = (int)((float)glowCurve.GetOutVal() * 255.0f);
					g.SetDrawMode(Graphics.DrawMode.Additive);
					g.SetColorizeImages(true);
					g.SetColor(new Color(glow, glow, glow));
					g.DrawImageTransformF(gemImage, transform, GlobalMembersResourcesWP.IMAGE_GEMS_BLUE.GetCelRect(0), x, y);
				}
			}
			g.SetColorizeImages(false);
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColor(Color.White);
		}

		public override void Dispose()
		{
			mGems.Clear();
			base.Dispose();
		}
	}
}
