using System;
using Microsoft.Xna.Framework;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class PIParticleInstance : IDisposable
	{
		public enum PIParticleVariation
		{
			VARIATION_LIFE,
			VARIATION_SIZE_X,
			VARIATION_SIZE_Y,
			VARIATION_VELOCITY,
			VARIATION_WEIGHT,
			VARIATION_SPIN,
			VARIATION_MOTION_RAND,
			VARIATION_BOUNCE,
			VARIATION_ZOOM,
			NUM_VARIATIONS
		}

		public PIParticleInstance mPrev;

		public PIParticleInstance mNext;

		public PIParticleDef mParticleDef;

		public PIEmitter mEmitterSrc;

		public int mNum;

		public PIFreeEmitterInstance mParentFreeEmitter;

		public Vector2 mPos = default(Vector2);

		public Vector2 mOrigPos = default(Vector2);

		public Vector2 mEmittedPos = default(Vector2);

		public Vector2 mLastEmitterPos = default(Vector2);

		public Vector2 mVel = default(Vector2);

		public float mImgAngle;

		public float[] mVariationValues = new float[9];

		public float mZoom;

		public float mSrcSizeXMult;

		public float mSrcSizeYMult;

		public float mGradientRand;

		public float mOrigEmitterAng;

		public int mAnimFrameRand;

		public SexyTransform2D mTransform = new SexyTransform2D(false);

		public float mTransformScaleFactor;

		public int mImgIdx;

		public float mThicknessHitVariation;

		public float mTicks;

		public float mLife;

		public float mLifePct;

		public bool mHasDrawn;

		public uint mBkgColor;

		public static int mCount;

		public PIParticleInstance()
		{
			mPrev = null;
			mNext = null;
			mTransformScaleFactor = 1f;
			mImgIdx = 0;
			mBkgColor = uint.MaxValue;
			mSrcSizeXMult = 1f;
			mSrcSizeYMult = 1f;
			mParentFreeEmitter = null;
			mHasDrawn = false;
			mCount++;
		}

		public void Reset()
		{
			mPos.X = 0f;
			mPos.Y = 0f;
			mPrev = null;
			mNext = null;
			mParticleDef = null;
			mEmitterSrc = null;
			mParentFreeEmitter = null;
			mTransformScaleFactor = 1f;
			mImgIdx = 0;
			mBkgColor = uint.MaxValue;
			mSrcSizeXMult = 1f;
			mSrcSizeYMult = 1f;
			mParentFreeEmitter = null;
			mHasDrawn = false;
			mTicks = 0f;
			mLife = 0f;
			mLifePct = 0f;
		}

		public void Init()
		{
		}

		public void Release()
		{
		}

		public virtual void Dispose()
		{
			mCount--;
		}
	}
}
