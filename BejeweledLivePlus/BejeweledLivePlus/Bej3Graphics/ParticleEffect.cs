using Microsoft.Xna.Framework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class ParticleEffect : Effect
	{
		public PIEffect mPIEffect;

		public bool mIsFirstUpdate;

		public bool mDoDrawTransform;

		private Transform PE_Update_trans = new Transform();

		private static SimpleObjectPool thePool_;

		public ParticleEffect()
			: base(Type.TYPE_PI)
		{
		}

		public void initWithPIEffect(PIEffect thePIEffect)
		{
			init(Type.TYPE_PI);
			mPIEffect = thePIEffect.Duplicate();
			mDoDrawTransform = false;
			mDAlpha = 0f;
			mIsFirstUpdate = true;
		}

		public override void Dispose()
		{
			mPIEffect.Dispose();
			base.Dispose();
		}

		public override void Update()
		{
			base.Update();
			PE_Update_trans.Reset();
			PE_Update_trans.Scale(mScale, mScale);
			PE_Update_trans.RotateRad(mAngle);
			Piece piece = mPieceRel;
			if (mPieceRel != null)
			{
				mStopWhenPieceRelMissing = true;
			}
			if (mStopWhenPieceRelMissing && piece == null)
			{
				Stop();
			}
			if (piece != null)
			{
				mX = piece.CX();
				mY = piece.CY();
				if (mFXManager.mBoard != null)
				{
					mX += 1260f * (float)(double)mFXManager.mBoard.mSlideUIPct;
				}
				if (mFXManager.mBoard != null && mFXManager.mBoard.mPostFXManager == mFXManager)
				{
					mX += (float)(double)mFXManager.mBoard.mSideXOff * mFXManager.mBoard.mSlideXScale;
				}
				if (piece.mHidePct > 0f)
				{
					mPIEffect.mColor.mAlpha = (int)(255f - piece.mHidePct * 255f);
				}
			}
			PE_Update_trans.Translate(mX, mY);
			if (mDoDrawTransform)
			{
				PE_Update_trans.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
				mPIEffect.mDrawTransform = PE_Update_trans.GetMatrix();
			}
			else
			{
				mPIEffect.mDrawTransform.LoadIdentity();
				mPIEffect.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
				mPIEffect.mEmitterTransform = PE_Update_trans.GetMatrix();
			}
			if (mIsFirstUpdate)
			{
				mPIEffect.ResetAnim();
				mIsFirstUpdate = false;
			}
			else
			{
				mPIEffect.Update();
			}
			if (!mPIEffect.IsActive())
			{
				mDeleteMe = true;
			}
		}

		public override void Draw(Graphics g)
		{
			if (!mIsFirstUpdate)
			{
				mPIEffect.mColor = new SexyFramework.Graphics.Color(255, 255, 255, (int)(255f * mFXManager.mAlpha * mAlpha));
				if (GlobalMembers.gApp.mBoard != null)
				{
					mPIEffect.mColor.mAlpha = (int)((float)mPIEffect.mColor.mAlpha * GlobalMembers.gApp.mBoard.GetAlpha());
				}
				mPIEffect.Draw(g);
			}
		}

		public void SetEmitAfterTimeline(bool emitAfterTimeline)
		{
			mPIEffect.mEmitAfterTimeline = emitAfterTimeline;
		}

		public bool SetLineEmitterPoint(int theLayerIdx, int theEmitterIdx, int thePointIdx, int theKeyFrame, FPoint thePoint)
		{
			PILayer layer = mPIEffect.GetLayer(theLayerIdx);
			if (layer == null)
			{
				return false;
			}
			PIEmitterInstance emitter = layer.GetEmitter(theEmitterIdx);
			if (emitter == null)
			{
				return false;
			}
			if (emitter.mEmitterInstanceDef.mEmitterGeom != 1)
			{
				return false;
			}
			if (thePointIdx >= emitter.mEmitterInstanceDef.mPoints.Count)
			{
				return false;
			}
			if (theKeyFrame >= emitter.mEmitterInstanceDef.mPoints[thePointIdx].mValuePoint2DVector.Count)
			{
				return false;
			}
			emitter.mEmitterInstanceDef.mPoints[thePointIdx].mValuePoint2DVector[theKeyFrame].mValue = new Vector2(thePoint.mX, thePoint.mY);
			return true;
		}

		public bool SetEmitterTint(int theLayerIdx, int theEmitterIdx, SexyFramework.Graphics.Color theColor)
		{
			PILayer layer = mPIEffect.GetLayer(theLayerIdx);
			if (layer == null)
			{
				return false;
			}
			PIEmitterInstance emitter = layer.GetEmitter(theEmitterIdx);
			if (emitter == null)
			{
				return false;
			}
			emitter.mTintColor = theColor;
			return true;
		}

		public void Stop()
		{
			SetEmitAfterTimeline(false);
			if (mPIEffect.mFrameNum < (float)(mPIEffect.mLastFrameNum - 1))
			{
				mPIEffect.mFrameNum = mPIEffect.mLastFrameNum - 1;
			}
		}

		public PILayer GetLayer(int theIdx)
		{
			return mPIEffect.GetLayer(theIdx);
		}

		public PILayer GetLayer(string theName)
		{
			return mPIEffect.GetLayer(theName);
		}

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(4096, typeof(ParticleEffect));
		}

		public static ParticleEffect fromPIEffect(PIEffect thePIEffect)
		{
			ParticleEffect particleEffect = (ParticleEffect)thePool_.alloc();
			particleEffect.initWithPIEffect(thePIEffect);
			return particleEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}
	}
}
