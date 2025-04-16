using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public class Transform
	{
		protected SexyTransform2D mTransForm = new SexyTransform2D(false);

		protected bool mNeedCalcMatrix;

		public bool mComplex;

		public bool mHaveRot;

		public bool mHaveScale;

		public float mTransX1;

		public float mTransY1;

		public float mTransX2;

		public float mTransY2;

		public float mScaleX;

		public float mScaleY;

		public float mRot;

		protected void MakeComplex()
		{
			if (!mComplex)
			{
				mComplex = true;
				CalcMatrix();
			}
		}

		protected void CalcMatrix()
		{
			if (mNeedCalcMatrix)
			{
				mNeedCalcMatrix = false;
				mTransForm.LoadIdentity();
				mTransForm.Translate(mTransX1, mTransX2);
				mTransForm.m02 = mTransX1;
				mTransForm.m12 = mTransY1;
				mTransForm.m22 = 1f;
				if (mHaveScale)
				{
					mTransForm.m00 = mScaleX;
					mTransForm.m11 = mScaleY;
				}
				else if (mHaveRot)
				{
					mTransForm.RotateRad(mRot);
				}
				if (mTransX2 != 0f || mTransY2 != 0f)
				{
					mTransForm.Translate(mTransX2, mTransY2);
				}
			}
		}

		public Transform()
		{
			Reset();
		}

		public void Reset()
		{
			mNeedCalcMatrix = true;
			mComplex = false;
			mTransX1 = (mTransY1 = 0f);
			mTransX2 = (mTransY2 = 0f);
			mScaleX = (mScaleY = 1f);
			mRot = 0f;
			mHaveRot = false;
			mHaveScale = false;
			mTransForm.LoadIdentity();
		}

		public void Translate(float tx, float ty)
		{
			if (!mComplex)
			{
				mNeedCalcMatrix = true;
				if (mHaveRot || mHaveScale)
				{
					mTransX2 += tx;
					mTransY2 += ty;
				}
				else
				{
					mTransX1 += tx;
					mTransY1 += ty;
				}
			}
			else
			{
				mTransForm.Translate(tx, ty);
			}
		}

		public void RotateRad(float rot)
		{
			if (!mComplex)
			{
				if (mHaveScale)
				{
					MakeComplex();
					mTransForm.RotateRad(rot);
				}
				else
				{
					mNeedCalcMatrix = true;
					mHaveRot = true;
					mRot += rot;
				}
			}
			else
			{
				mTransForm.RotateRad(rot);
			}
		}

		public void RotateDeg(float rot)
		{
			RotateRad(MathHelper.ToRadians(rot));
		}

		public void Scale(float sx, float sy)
		{
			if (!mComplex)
			{
				if (mHaveRot || mTransX1 != 0f || mTransY1 != 0f || (sx < 0f && mScaleX * sx != -1f) || sy < 0f || ((mTransX2 != 0f || mTransY2 != 0f) && sx != sy))
				{
					MakeComplex();
					mTransForm.Scale(sx, sy);
					return;
				}
				mNeedCalcMatrix = true;
				mHaveScale = true;
				mScaleX *= sx;
				mScaleY *= sy;
				mTransX2 *= sx;
				mTransY2 *= sy;
			}
			else
			{
				mTransForm.Scale(sx, sy);
			}
		}

		public SexyTransform2D GetMatrix()
		{
			CalcMatrix();
			return mTransForm;
		}

		public void SetMatrix(SexyTransform2D mat)
		{
			mTransForm.mMatrix = mat.mMatrix;
			mNeedCalcMatrix = false;
			mComplex = true;
		}
	}
}
