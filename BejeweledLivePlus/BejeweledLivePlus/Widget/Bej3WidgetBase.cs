using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3WidgetBase : SexyFramework.Widget.Widget
	{
		private float mAlphaStep;

		public Color mColor = default(Color);

		public float mAlpha;

		public bool mClippingEnabled;

		public bool mGrayedOut;

		public static readonly Color GreyedOutColor = new Color(150, 150, 150, 255);

		public Bej3WidgetBase()
		{
			mAlphaStep = 0f;
			mAlpha = 1f;
			mColor = Color.White;
			mClippingEnabled = true;
			mGrayedOut = false;
		}

		public void Bej3WidgetBaseDrawAll(ModalFlags theFlags, Graphics g)
		{
			base.DrawAll(theFlags, g);
		}

		public override void Update()
		{
			if (mAlphaStep != 0f)
			{
				mAlpha += mAlphaStep;
				if (mAlpha < 0f)
				{
					mAlpha = 0f;
					mAlphaStep = 0f;
				}
				if (mAlpha > 1f)
				{
					mAlpha = 1f;
					mAlphaStep = 0f;
				}
				mColor.mAlpha = (int)(255f * mAlpha);
			}
			base.Update();
		}

		public virtual void Fade(float step)
		{
			mAlphaStep = step;
		}

		public virtual void FadeIn(float step)
		{
			mAlphaStep = step;
		}

		public virtual void FadeOut(float step)
		{
			if (step > 0f)
			{
				step = 0f - step;
			}
			mAlphaStep = step;
		}

		public virtual void FadeIn()
		{
			FadeIn(0.01f);
		}

		public virtual void FadeOut()
		{
			FadeOut(0.01f);
		}

		public virtual void LinkUpAssets()
		{
		}
	}
}
