using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class Particle : MovableObject
	{
		public int mPoolIndex = -1;

		public float mForcedAlpha = -100f;

		public float mOriginalXSize;

		public float mOriginalYSize;

		public string mParentName = "";

		public Image mImage;

		public ParticleType mParentType;

		public ColorKeyManager mColorKeyManager;

		public ColorKeyManager mAlphaKeyManager;

		public int mImageCel;

		public int mImageRate = 1;

		public int mRefXOff;

		public int mRefYOff;

		public Rect mRect = default(Rect);

		public int mWidth;

		public int mHeight;

		public bool mHasBeenVisible;

		public bool mLastFrameWasVisible = true;

		public bool mAdditive;

		public bool mAdditiveWithNormal;

		public bool mLockSizeAspect;

		public bool mFlipX;

		public bool mFlipY;

		public bool mAlignAngleToMotion;

		public bool mForceDeletion;

		public float mForceFadeoutRate;

		public float mMotionAngleOffset;

		public float mCurXSize;

		public float mCurYSize;

		protected Transform mGlobalTransform = new Transform();

		public override void Reset()
		{
		}

		public override void Deserialize(Buffer b, Dictionary<int, Deflector> deflector_ptr_map)
		{
		}

		public Particle()
		{
			mColorKeyManager = new ColorKeyManager();
			mAlphaKeyManager = new ColorKeyManager();
		}

		public Particle(float spawn_angle, float velocity)
		{
			Reset(spawn_angle, velocity);
			mColorKeyManager = new ColorKeyManager();
			mAlphaKeyManager = new ColorKeyManager();
		}

		public override void Dispose()
		{
			mColorKeyManager = null;
			mAlphaKeyManager = null;
		}

		private void DoDraw(SexyFramework.Graphics.Graphics g, Transform t, float scale)
		{
			if (g.Is3D())
			{
				g.DrawImageTransformF(mImage, t, mImage.GetCelRect(mImageCel), (mX + (float)mParentType.mXOff * mCurXSize + (float)mRefXOff * mCurXSize) * scale, (mY + (float)mParentType.mYOff * mCurYSize + (float)mRefYOff * mCurYSize) * scale);
			}
			else
			{
				g.DrawImageTransform(mImage, t, mImage.GetCelRect(mImageCel), (mX + (float)mParentType.mXOff * mCurXSize + (float)mRefXOff * mCurXSize) * scale, (mY + (float)mParentType.mYOff * mCurYSize + (float)mRefYOff * mCurYSize) * scale);
			}
		}

		private void DoDraw(SexyFramework.Graphics.Graphics g, Transform t)
		{
			DoDraw(g, t, 1f);
		}

		public virtual void Reset(float spawn_angle, float velocity)
		{
			base.Reset();
			mForceDeletion = false;
			mForceFadeoutRate = 0f;
			mImage = null;
			mCurXSize = 1f;
			mCurYSize = 1f;
			mLockSizeAspect = true;
			mOriginalXSize = 1f;
			mOriginalYSize = 1f;
			mParentType = null;
			mAdditive = (mAdditiveWithNormal = false);
			mFlipX = (mFlipY = false);
			mRefXOff = (mRefYOff = 0);
			mLife = 0;
			mAlignAngleToMotion = false;
			mMotionAngleOffset = 0f;
			mHasBeenVisible = false;
			mLastFrameWasVisible = true;
			Launch(spawn_angle, velocity);
		}

		public override void Update()
		{
			if (!mInitialized)
			{
				mOriginalXSize = mCurXSize;
				mOriginalYSize = mCurYSize;
			}
			base.Update();
			if (Dead())
			{
				return;
			}
			if (mImage != null && mImageRate > 0 && mUpdateCount % mImageRate == 0)
			{
				mImageCel = (mImageCel + 1) % (mImage.mNumCols * mImage.mNumRows);
			}
			if (mAlignAngleToMotion)
			{
				float num = Common.AngleBetweenPoints(mVX, 0f - mVY, 0f, 0f);
				mAngle = num - mMotionAngleOffset;
			}
			mColorKeyManager.Update(mX, mY);
			if (mForceFadeoutRate <= 0f)
			{
				mAlphaKeyManager.Update(mX, mY);
			}
			else
			{
				if (mForcedAlpha < 0f)
				{
					mForcedAlpha = mAlphaKeyManager.GetColor().mAlpha;
				}
				mForcedAlpha -= mForceFadeoutRate;
				if (mForcedAlpha <= 0f)
				{
					mForceDeletion = true;
				}
			}
			LifetimeSettings interpLifetimeSettings = GetInterpLifetimeSettings();
			mCurXSize = mOriginalXSize * interpLifetimeSettings.mSizeXMult;
			mCurYSize = mOriginalYSize * interpLifetimeSettings.mSizeYMult;
		}

		public virtual void Draw(SexyFramework.Graphics.Graphics g, float alpha_pct, Color tint_color, float tint_strength, float scale)
		{
			if (mImage == null || alpha_pct == 0f || mCurXSize == 0f || (!mLockSizeAspect && mCurYSize == 0f))
			{
				mLastFrameWasVisible = false;
				return;
			}
			mGlobalTransform.Reset();
			float num = (mLockSizeAspect ? mCurXSize : mCurYSize);
			if (!Common._eq(mCurXSize * scale, 1f, 1E-06f) || (!Common._eq(mCurYSize * scale, 1f, 1E-06f) && !mLockSizeAspect) || mFlipX || mFlipY)
			{
				mGlobalTransform.Scale(mFlipX ? (0f - mCurXSize) : mCurXSize, num * (mFlipY ? (-1f) : 1f));
			}
			if (mAngle != 0f)
			{
				mGlobalTransform.Translate((float)mRefXOff * mCurXSize, (float)mRefYOff * num);
				mGlobalTransform.RotateRad(0f - mAngle);
				mGlobalTransform.Translate((float)(-mRefXOff) * mCurXSize, (float)(-mRefYOff) * num);
			}
			Color color = mColorKeyManager.GetColor();
			color.mRed -= (int)((float)(color.mRed - tint_color.mRed) * tint_strength);
			color.mGreen -= (int)((float)(color.mGreen - tint_color.mGreen) * tint_strength);
			color.mBlue -= (int)((float)(color.mBlue - tint_color.mBlue) * tint_strength);
			if (mForcedAlpha > 0f)
			{
				color.mAlpha = (int)mForcedAlpha;
			}
			else
			{
				color.mAlpha = mAlphaKeyManager.GetColor().mAlpha;
			}
			if (!Common._eq(alpha_pct, 1f, 1E-06f) || color != Color.White)
			{
				g.SetColorizeImages(true);
				color.mAlpha = (int)((float)color.mAlpha * alpha_pct);
				g.SetColor(color);
			}
			if (color.mAlpha > 0)
			{
				mHasBeenVisible = true;
			}
			mLastFrameWasVisible = color.mAlpha != 0;
			if (mAdditive)
			{
				if (mAdditiveWithNormal)
				{
					DoDraw(g, mGlobalTransform, scale);
				}
				g.SetDrawMode(1);
			}
			DoDraw(g, mGlobalTransform, scale);
			g.SetColorizeImages(false);
			g.SetDrawMode(0);
		}

		public virtual void Draw(SexyFramework.Graphics.Graphics g, float alpha_pct, Color tint_color, float tint_strength)
		{
			Draw(g, alpha_pct, tint_color, tint_strength, 1f);
		}

		public override bool Dead()
		{
			if (!mForceDeletion)
			{
				return base.Dead();
			}
			return true;
		}

		public virtual float GetWidth()
		{
			return (float)mImage.GetWidth() * mCurXSize;
		}

		public virtual float GetHeight()
		{
			if (mLockSizeAspect)
			{
				return (float)mImage.GetHeight() * mCurXSize;
			}
			return (float)mImage.GetHeight() * mCurYSize;
		}

		public virtual Rect GetRect()
		{
			float width = GetWidth();
			float height = GetHeight();
			mRect.mX = (int)(mX - width / 2f);
			mRect.mY = (int)(mY - height / 2f);
			mRect.mWidth = (int)width;
			mRect.mHeight = (int)height;
			return mRect;
		}

		public override void Serialize(Buffer b)
		{
			base.Serialize(b);
			b.WriteFloat(mOriginalXSize);
			b.WriteFloat(mOriginalYSize);
			b.WriteLong(mImageCel);
			b.WriteLong(mParentType.mSerialIndex);
			mColorKeyManager.Serialize(b);
			mAlphaKeyManager.Serialize(b);
			b.WriteLong(mImageRate);
			b.WriteLong(mRefXOff);
			b.WriteLong(mRefYOff);
			b.WriteBoolean(mAdditive);
			b.WriteBoolean(mAdditiveWithNormal);
			b.WriteBoolean(mLockSizeAspect);
			b.WriteBoolean(mFlipX);
			b.WriteBoolean(mFlipY);
			b.WriteBoolean(mAlignAngleToMotion);
			b.WriteFloat(mMotionAngleOffset);
			b.WriteFloat(mCurXSize);
			b.WriteFloat(mCurYSize);
			b.WriteBoolean(mHasBeenVisible);
			b.WriteBoolean(mLastFrameWasVisible);
		}

		public void Deserialize(Buffer b, Dictionary<int, Deflector> deflector_ptr_map, Dictionary<int, ParticleType> ptype_ptr_map)
		{
			base.Deserialize(b, deflector_ptr_map);
			mOriginalXSize = b.ReadFloat();
			mOriginalYSize = b.ReadFloat();
			mImageCel = (int)b.ReadLong();
			int key = (int)b.ReadLong();
			if (ptype_ptr_map.ContainsKey(key))
			{
				mParentType = ptype_ptr_map[key];
			}
			mImage = mParentType.mImage;
			mColorKeyManager.Deserialize(b);
			mAlphaKeyManager.Deserialize(b);
			mImageRate = (int)b.ReadLong();
			mRefXOff = (int)b.ReadLong();
			mRefYOff = (int)b.ReadLong();
			mAdditive = b.ReadBoolean();
			mAdditiveWithNormal = b.ReadBoolean();
			mLockSizeAspect = b.ReadBoolean();
			mFlipX = b.ReadBoolean();
			mFlipY = b.ReadBoolean();
			mAlignAngleToMotion = b.ReadBoolean();
			mMotionAngleOffset = b.ReadFloat();
			mCurXSize = b.ReadFloat();
			mCurYSize = b.ReadFloat();
			mHasBeenVisible = b.ReadBoolean();
			mLastFrameWasVisible = b.ReadBoolean();
		}
	}
}
