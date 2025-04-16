using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.AELib
{
	public class Layer
	{
		protected SharedImageRef mImage;

		public Timeline mAnchorPoint = new Timeline();

		public Timeline mPosition = new Timeline();

		public Timeline mScale = new Timeline();

		public Timeline mRotation = new Timeline();

		public Timeline mOpacity = new Timeline();

		public string mLayerName;

		public int mWidth;

		public int mHeight;

		public int mXOff;

		public int mYOff;

		public bool mAdditive;

		public Layer()
		{
		}

		public Layer(Layer other)
		{
			CopyFrom(other);
		}

		public void CopyFrom(Layer rhs)
		{
			mAdditive = rhs.mAdditive;
			mLayerName = rhs.mLayerName;
			mWidth = rhs.mWidth;
			mHeight = rhs.mHeight;
			mImage = rhs.mImage;
			mXOff = rhs.mXOff;
			mYOff = rhs.mYOff;
			mAnchorPoint = new Timeline(rhs.mAnchorPoint);
			mPosition = new Timeline(rhs.mPosition);
			mScale = new Timeline(rhs.mScale);
			mRotation = new Timeline(rhs.mRotation);
			mOpacity = new Timeline(rhs.mOpacity);
		}

		public virtual void Reset()
		{
			mAnchorPoint.Reset();
			mPosition.Reset();
			mScale.Reset();
			mRotation.Reset();
			mOpacity.Reset();
		}

		public void AddAnchorPoint(int frame, float x, float y)
		{
			mAnchorPoint.AddKeyframe(frame, x, y);
		}

		public void AddPosition(int frame, float x, float y)
		{
			mPosition.AddKeyframe(frame, x, y);
		}

		public void AddScale(int frame, float sx, float sy)
		{
			mScale.AddKeyframe(frame, sx, sy);
		}

		public void AddRotation(int frame, float angle_radians)
		{
			mRotation.AddKeyframe(frame, angle_radians);
		}

		public void AddOpacity(int frame, float pct)
		{
			mOpacity.AddKeyframe(frame, pct);
		}

		public void EnsureTimelineDefaults(long comp_width, long comp_height)
		{
			if (!mOpacity.HasInitialValue())
			{
				mOpacity.AddKeyframe(0, 1f);
			}
			if (!mRotation.HasInitialValue())
			{
				mRotation.AddKeyframe(0, 0f);
			}
			if (!mScale.HasInitialValue())
			{
				mScale.AddKeyframe(0, 1f, 1f);
			}
			if (!mPosition.HasInitialValue())
			{
				mPosition.AddKeyframe(0, (float)comp_width / 2f, (float)comp_height / 2f);
			}
			if (mImage != null && mImage.GetImage() != null && !mAnchorPoint.HasInitialValue())
			{
				mAnchorPoint.AddKeyframe(0, (float)mImage.GetImage().GetCelWidth() / 2f, (float)mImage.GetImage().GetCelHeight() / 2f);
			}
		}

		public virtual Layer Duplicate()
		{
			return new Layer(this);
		}

		public virtual Image GetImage()
		{
			return mImage.GetImage();
		}

		public virtual bool IsLayerBase()
		{
			return true;
		}

		public virtual bool NeedsTranslatedFrame()
		{
			return false;
		}

		public void SetImage(SharedImageRef img)
		{
			mImage = img;
		}

		public virtual void Draw(SexyFramework.Graphics.Graphics g)
		{
			Draw(g, null);
		}

		public virtual void Draw(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans)
		{
			Draw(g, ctrans, -1);
		}

		public virtual void Draw(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans, int frame)
		{
			Draw(g, ctrans, frame, 1f);
		}

		public virtual bool isValid()
		{
			if (mImage.mSharedImage == null)
			{
				return mImage.mUnsharedImage != null;
			}
			return true;
		}

		public virtual void Draw(SexyFramework.Graphics.Graphics g, CumulativeTransform ctrans, int frame, float scale)
		{
			float value = 0f;
			mOpacity.GetValue(frame, ref value);
			float num = 255f * value;
			if (ctrans != null)
			{
				num *= ctrans.mOpacity;
			}
			if (!(num <= 0f))
			{
				if (num != 255f)
				{
					g.SetColorizeImages(true);
					g.SetColor(255, 255, 255, (int)num);
				}
				float value2 = 0f;
				float value3 = 0f;
				mAnchorPoint.GetValue(frame, ref value2, ref value3);
				value2 *= scale;
				value3 *= scale;
				int num2 = mImage.mWidth / 2;
				int num3 = mImage.mHeight / 2;
				value2 -= (float)num2;
				value3 -= (float)num3;
				float value4 = 0f;
				float value5 = 0f;
				mScale.GetValue(frame, ref value4, ref value5);
				float value6 = 0f;
				mRotation.GetValue(frame, ref value6);
				float value7 = 0f;
				float value8 = 0f;
				mPosition.GetValue(frame, ref value7, ref value8);
				value7 *= scale;
				value8 *= scale;
				SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
				sexyTransform2D.Translate(0f - value2 + (float)mXOff, 0f - value3 + (float)mYOff);
				sexyTransform2D.Scale(value4, value5);
				if (value6 != 0f)
				{
					sexyTransform2D.RotateRad(0f - value6);
				}
				sexyTransform2D.Translate(value7, value8);
				if (mAdditive || ctrans.mForceAdditive)
				{
					g.SetDrawMode(1);
				}
				sexyTransform2D = ctrans.mTrans * sexyTransform2D;
				g.DrawImageMatrix(mImage.GetImage(), sexyTransform2D);
				g.SetDrawMode(0);
				g.SetColorizeImages(false);
			}
		}
	}
}
