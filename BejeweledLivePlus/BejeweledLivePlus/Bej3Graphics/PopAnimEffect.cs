using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class PopAnimEffect : Effect
	{
		private bool mUpdatedOnce;

		public PopAnim mPopAnim;

		public new bool mDoubleSpeed;

		private static SimpleObjectPool thePool_;

		public PopAnimEffect()
			: base(Type.TYPE_POPANIM)
		{
		}

		public void initWithPopAnim(PopAnim thePopAnim)
		{
			init(Type.TYPE_POPANIM);
			mPopAnim = thePopAnim.Duplicate();
			mDAlpha = 0f;
			mDoubleSpeed = true;
			mUpdatedOnce = false;
		}

		public override void Dispose()
		{
			mPopAnim.Dispose();
			mPopAnim = null;
			base.Dispose();
		}

		public virtual void Play()
		{
			mPopAnim.Play();
		}

		public virtual void Play(string theComposition)
		{
			mPopAnim.Play(theComposition);
		}

		public override void Update()
		{
			mUpdatedOnce = true;
			base.Update();
			int num = (int)mScale;
			int num2 = (int)(0.625 * (double)mPopAnim.mAnimRect.mWidth * (double)num);
			int num3 = (int)(0.625 * (double)mPopAnim.mAnimRect.mHeight * (double)num);
			if (GlobalMembers.gApp.mResourceManager.mCurArtRes == 480)
			{
				num2 = (int)((float)num2 * 0.5f);
				num3 = (int)((float)num3 * 0.5f);
			}
			Transform transform = new Transform();
			transform.Reset();
			transform.Scale(num, num);
			if (mAngle != 0f)
			{
				transform.Translate(-num2 / 2, -num3 / 2);
				transform.RotateRad(mAngle);
				transform.Translate(num2 / 2, num3 / 2);
			}
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
			}
			transform.Translate(GlobalMembers.S(mX) - (float)(num2 / 2), GlobalMembers.S(mY) - (float)(num3 / 2));
			mPopAnim.SetTransform(transform.GetMatrix());
			mPopAnim.Update();
			if (mDoubleSpeed)
			{
				mPopAnim.Update();
			}
			if (!mPopAnim.IsActive())
			{
				mDeleteMe = true;
			}
		}

		public override void Draw(Graphics g)
		{
			if (!mUpdatedOnce)
			{
				Update();
			}
			mPopAnim.mColor = new Color(255, 255, 255, (int)(255f * mFXManager.mAlpha));
			if (GlobalMembers.gApp.mBoard != null)
			{
				mPopAnim.mColor.mAlpha = (int)((float)mPopAnim.mColor.mAlpha * GlobalMembers.gApp.mBoard.GetAlpha());
			}
			mPopAnim.Draw(g);
		}

		public void Stop()
		{
		}

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(PopAnimEffect));
		}

		public static PopAnimEffect fromPopAnim(PopAnim thePopAnim)
		{
			PopAnimEffect popAnimEffect = (PopAnimEffect)thePool_.alloc();
			popAnimEffect.initWithPopAnim(thePopAnim);
			return popAnimEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}
	}
}
