using SexyFramework;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class CreditsMenuScrollWidget : ScrollWidget
	{
		private bool mTouched;

		public bool mAnimate;

		public CreditsMenuScrollWidget(ScrollWidgetListener listener)
			: base(listener)
		{
			mTouched = false;
		}

		public new FPoint GetScrollOffset()
		{
			return mScrollOffset;
		}

		public float GetVelocity()
		{
			return mScrollVelocity.mY;
		}

		public override void Update()
		{
			if (mScrollVelocity.mY > ConstantsWP.CREDITSMENU_SPEED && mScrollVelocity.mY <= ConstantsWP.CREDITSMENU_SPEED_CHANGE)
			{
				mScrollVelocity.mY -= ConstantsWP.CREDITSMENU_SPEED_CHANGE;
			}
			if (float.IsNaN(mScrollVelocity.mY))
			{
				mScrollVelocity.mY = ConstantsWP.CREDITSMENU_SPEED;
			}
			if (mAnimate)
			{
				base.Update();
				if (mScrollOffset.mY < mScrollMin.mY)
				{
					Restart(true);
				}
			}
		}

		public override void TouchBegan(SexyAppBase.Touch touch)
		{
			if (!mTouched)
			{
				base.TouchBegan(touch);
			}
			mTouched = true;
		}

		public override void TouchEnded(SexyAppBase.Touch touch)
		{
			mTouched = false;
			base.TouchEnded(touch);
		}

		public override void TouchesCanceled()
		{
			mTouched = false;
			base.TouchesCanceled();
		}

		public void Restart(bool wasFinished)
		{
			ScrollToMin(false);
		}

		public void Restart()
		{
			Restart(false);
		}
	}
}
