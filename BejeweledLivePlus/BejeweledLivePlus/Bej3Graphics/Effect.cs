using System;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class Effect : IDisposable
	{
		public enum Type
		{
			TYPE_NONE,
			TYPE_CUSTOMCLASS,
			TYPE_BLAST_RING,
			TYPE_EMBER_BOTTOM,
			TYPE_EMBER_FADEINOUT_BOTTOM,
			TYPE_EMBER,
			TYPE_EMBER_FADEINOUT,
			TYPE_SMOKE_PUFF,
			TYPE_DROPLET,
			TYPE_STEAM_COMET,
			TYPE_FRUIT_SPARK,
			TYPE_GEM_SHARD,
			TYPE_COUNTDOWN_SHARD,
			TYPE_SPARKLE_SHARD,
			TYPE_STEAM,
			TYPE_GLITTER_SPARK,
			TYPE_CURSOR_RING,
			TYPE_LIGHT,
			TYPE_WALL_ROCK,
			TYPE_QUAKE_DUST,
			TYPE_HYPERCUBE_ENERGIZE,
			TYPE_TIME_BONUS,
			TYPE_PI,
			TYPE_POPANIM,
			NUM_TYPES
		}

		public enum FLAG
		{
			FLAG_SCALE_FADEINOUT = 1,
			FLAG_ALPHA_FADEINOUT = 2,
			FLAG_ALPHA_FADEINDELAY = 4,
			FLAG_HYPERSPACE_ONLY = 8
		}

		private static SimpleObjectPool thePool_;

		public Type mType;

		public bool mOverlay;

		public float mX;

		public float mY;

		public float mZ;

		public float mDX;

		public float mDY;

		public float mDZ;

		public Piece mPieceRel;

		public float mDXScalar;

		public float mDYScalar;

		public float mDZScalar;

		public float mGravity;

		public int mFrame;

		public float mDelay;

		public int mGemType;

		public float mDecel;

		public float mAlpha;

		public float mDAlpha;

		public float mMaxAlpha;

		public float mScale;

		public float mDScale;

		public float mLightSize;

		public float mLightIntensity;

		public float mMinScale;

		public float mMaxScale;

		public Color mColor = default(Color);

		public Color mColor2 = default(Color);

		public float mAngle;

		public float mDAngle;

		public int mStage;

		public float mTimer;

		public CurvedVal mCurvedAlpha = new CurvedVal();

		public CurvedVal mCurvedScale = new CurvedVal();

		public Image mImage;

		public bool mDoubleSpeed;

		public int mUpdateDiv;

		public bool mIsCyclingColor;

		public int mCurHue;

		public bool mIsAdditive;

		public float[] mValue = new float[4];

		public uint mPieceId;

		public int mFlags;

		public bool mDeleteMe;

		public int mRefCount;

		public EffectsManager mFXManager;

		public bool mDeleteImage;

		public bool mStopWhenPieceRelMissing;

		public static Effect alloc()
		{
			return alloc(Type.TYPE_NONE);
		}

		public static void initPool()
		{
			thePool_ = new SimpleObjectPool(8192, typeof(Effect));
		}

		public static Effect alloc(Type theType)
		{
			Effect effect = (Effect)thePool_.alloc();
			effect.init(theType);
			return effect;
		}

		public virtual void release()
		{
			Dispose();
			thePool_.release(this);
		}

		public Effect()
			: this(Type.TYPE_NONE)
		{
		}

		public void init(Type theType)
		{
			mDoubleSpeed = false;
			mOverlay = false;
			mX = 0f;
			mY = 0f;
			mZ = 0f;
			mPieceRel = null;
			mStopWhenPieceRelMissing = false;
			mDX = (mDY = (mDZ = 0f));
			mDXScalar = (mDYScalar = (mDZScalar = 1f));
			mGravity = 0f;
			mDelay = 0f;
			mLightSize = 0f;
			mLightIntensity = 0f;
			mScale = 1f;
			mDScale = 0f;
			mMinScale = 0f;
			mMaxScale = 10000f;
			mFrame = 0;
			mAngle = (mDAngle = 0f);
			mColor = new Color(255, 255, 255);
			mIsCyclingColor = false;
			mCurHue = 0;
			mUpdateDiv = 1;
			mAlpha = 1f;
			mDAlpha = -0.01f;
			mMaxAlpha = 1f;
			mImage = null;
			mFlags = 0;
			mIsAdditive = false;
			mDeleteImage = false;
			mDeleteMe = false;
			mRefCount = 0;
			SetType(theType);
		}

		public Effect(Type theType)
		{
			init(theType);
		}

		public virtual void Dispose()
		{
			if (mDeleteImage)
			{
				mImage.Dispose();
				mImage = null;
			}
		}

		public virtual void Update()
		{
			if (!mDeleteMe)
			{
				mCurvedAlpha.IncInVal();
				mCurvedScale.IncInVal();
			}
		}

		public virtual void Draw(Graphics g)
		{
		}

		public virtual void SetType(Type theType)
		{
			mType = theType;
			switch (mType)
			{
			case Type.TYPE_STEAM:
				mImage = GlobalMembersResourcesWP.IMAGE_FX_STEAM;
				mGravity = GlobalMembers.MS(-0.005f);
				mAngle = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
				mDAngle = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.04f);
				mAlpha = GlobalMembers.M(0.85f);
				mDAlpha = 0f;
				mValue[0] = GlobalMembers.M(0.5f);
				mValue[1] = GlobalMembers.M(-0.02f);
				mValue[2] = GlobalMembers.M(0.95f);
				break;
			case Type.TYPE_SPARKLE_SHARD:
				mUpdateDiv = BejeweledLivePlus.Misc.Common.Rand() % 4 + 3;
				mDX = GlobalMembers.S(-1f + (float)(BejeweledLivePlus.Misc.Common.Rand() % 20) * 0.1f);
				mDY = GlobalMembers.S((float)(BejeweledLivePlus.Misc.Common.Rand() % 50) * 0.1f);
				mColor = new Color(255, 255, 255);
				break;
			case Type.TYPE_GEM_SHARD:
				mFrame = BejeweledLivePlus.Misc.Common.Rand() % 40;
				mUpdateDiv = 0;
				mAlpha = 1f;
				mDAlpha = GlobalMembers.M(-0.005f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(-0.01f);
				mDecel = 1f;
				break;
			case Type.TYPE_GLITTER_SPARK:
				mImage = GlobalMembersResourcesWP.IMAGE_GEM_FRUIT_SPARK;
				mIsAdditive = true;
				mGravity = GlobalMembers.MS(0.01f);
				mAlpha = GlobalMembers.M(1f);
				mDAlpha = GlobalMembers.M(0f);
				mScale = GlobalMembers.M(0.5f);
				mDScale = GlobalMembers.M(-0.005f);
				break;
			case Type.TYPE_FRUIT_SPARK:
				mGravity = GlobalMembers.MS(0.005f);
				mDX = GlobalMembersUtils.GetRandFloat() * GlobalMembers.MS(1f);
				mDY = GlobalMembersUtils.GetRandFloat() * GlobalMembers.MS(1f);
				mScale = 0.2f;
				mAlpha = 1f;
				mDAlpha = -0.005f;
				mAngle = (GlobalMembers.gApp.Is3DAccelerated() ? (GlobalMembersUtils.GetRandFloat() * (float)Math.PI) : 0f);
				break;
			case Type.TYPE_EMBER_BOTTOM:
			case Type.TYPE_EMBER_FADEINOUT_BOTTOM:
			case Type.TYPE_EMBER:
			case Type.TYPE_EMBER_FADEINOUT:
				mImage = GlobalMembersResourcesWP.IMAGE_FIREPARTICLE;
				mColor = new Color(255, BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(64) + GlobalMembers.M(64), BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(32) + GlobalMembers.M(30));
				mGravity = GlobalMembers.M(-0f);
				mScale = GlobalMembers.M(0.75f);
				mDScale = GlobalMembers.M(0.005f);
				mAngle = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
				mDAngle = GlobalMembers.M(0.01f);
				if (mType == Type.TYPE_EMBER_FADEINOUT || mType == Type.TYPE_EMBER_FADEINOUT_BOTTOM)
				{
					mAlpha = 0.01f;
					mDAlpha = GlobalMembers.M(0.02f);
					mStage = 0;
				}
				break;
			case Type.TYPE_SMOKE_PUFF:
				mImage = GlobalMembersResourcesWP.IMAGE_SMOKE;
				mGravity = GlobalMembers.MS(-0.005f);
				mAlpha = GlobalMembers.M(0.5f);
				mDAlpha = GlobalMembers.M(-0.005f);
				break;
			case Type.TYPE_DROPLET:
				mImage = GlobalMembersResourcesWP.IMAGE_DRIP;
				mDAlpha = 0f;
				mGravity = GlobalMembers.MS(0.05f);
				break;
			case Type.TYPE_STEAM_COMET:
				mImage = GlobalMembersResourcesWP.IMAGE_FX_STEAM;
				mGravity = GlobalMembers.MS(-0.005f);
				mAngle = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
				mDAngle = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.04f);
				mAlpha = GlobalMembers.M(0.85f);
				mDAlpha = 0f;
				break;
			case Type.TYPE_COUNTDOWN_SHARD:
				break;
			}
		}
	}
}
