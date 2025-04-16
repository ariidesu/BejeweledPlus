using SexyFramework.Drivers;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class Slider : Widget
	{
		public SliderListener mListener;

		public double mVal;

		public int mId;

		public Image mTrackImage;

		public Image mThumbImage;

		public bool mDragging;

		public int mRelX;

		public int mRelY;

		public bool mHorizontal;

		public float mSlideSpeed;

		public bool mSlidingLeft;

		public bool mSlidingRight;

		public bool mStepMode;

		public int mNumSteps;

		public int mCurStep;

		public int mStepSound;

		public int mKnobSize;

		public Color mOutlineColor = default(Color);

		public Color mBkgColor = default(Color);

		public Color mSliderColor = default(Color);

		public Slider(Image theTrackImage, Image theThumbImage, int theId, SliderListener theListener)
		{
			mTrackImage = theTrackImage;
			mThumbImage = theThumbImage;
			mId = theId;
			mListener = theListener;
			mVal = 0.0;
			mOutlineColor = new Color(Color.White);
			mBkgColor = new Color(80, 80, 80);
			mSliderColor = new Color(Color.White);
			mKnobSize = 5;
			mDragging = false;
			mHorizontal = true;
			mRelX = (mRelY = 0);
			mSlideSpeed = 0.01f;
			mSlidingLeft = false;
			mSlidingRight = false;
			mStepSound = -1;
			mStepMode = false;
			mNumSteps = 1;
			mCurStep = 0;
		}

		public virtual void SetValue(double theValue)
		{
			double num = mVal;
			mVal = theValue;
			if (mVal < 0.0)
			{
				mVal = 0.0;
			}
			else if (mVal > 1.0)
			{
				mVal = 1.0;
			}
			if (mVal != num)
			{
				mListener.SliderVal(mId, mVal);
			}
			MarkDirtyFull();
		}

		public virtual void SetStepMode(int num_steps, int cur_step, int step_sound)
		{
			mStepMode = true;
			mNumSteps = num_steps;
			SetStepValue(cur_step);
			mStepSound = step_sound;
		}

		public virtual void SetStepValue(int cur_step)
		{
			if (cur_step < 0)
			{
				cur_step = 0;
			}
			if (cur_step > mNumSteps)
			{
				cur_step = mNumSteps;
			}
			if (mCurStep != cur_step)
			{
				mCurStep = cur_step;
				SetValue((double)cur_step / (double)mNumSteps);
				if (mStepSound != -1)
				{
					GlobalMembers.gSexyApp.PlaySample(mStepSound);
				}
			}
		}

		public override void Update()
		{
			base.Update();
			if (mIsGamepadSelection)
			{
				if (mSlidingLeft)
				{
					SetValue(mVal - (double)mSlideSpeed);
				}
				if (mSlidingRight)
				{
					SetValue(mVal + (double)mSlideSpeed);
				}
			}
			else
			{
				mSlidingLeft = false;
				mSlidingRight = false;
			}
		}

		public virtual bool HasTransparencies()
		{
			return true;
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			if (mTrackImage != null)
			{
				int num = (mHorizontal ? (mTrackImage.GetWidth() / 3) : mTrackImage.GetWidth());
				int num2 = (mHorizontal ? mTrackImage.GetHeight() : (mTrackImage.GetHeight() / 3));
				Rect theSrcRect = new Rect(0, 0, num, num2);
				if (mHorizontal)
				{
					int theY = (mHeight - num2) / 2;
					g.DrawImage(mTrackImage, 0, theY, theSrcRect);
					g.PushState();
					g.ClipRect(num, theY, mWidth - num * 2, num2);
					for (int i = 0; i < (mWidth - num * 2 + num - 1) / num; i++)
					{
						g.DrawImage(mTrackImage, num + i * num, theY, new Rect(num, 0, num, num2));
					}
					g.PopState();
					g.DrawImage(mTrackImage, mWidth - num, theY, new Rect(num * 2, 0, num, num2));
				}
				else
				{
					int theX = (mWidth - num) / 2;
					g.DrawImage(mTrackImage, theX, 0, theSrcRect);
					g.PushState();
					g.ClipRect(theX, num2, num, mHeight - num2 * 2);
					for (int j = 0; j < (mHeight - num2 * 2 + num2 - 1) / num2; j++)
					{
						g.DrawImage(mTrackImage, theX, num2 + j * num2, theSrcRect);
					}
					g.PopState();
					g.DrawImage(mTrackImage, theX, mHeight - num2, theSrcRect);
				}
			}
			else if (mTrackImage == null)
			{
				g.SetColor(mOutlineColor);
				g.FillRect(0, 0, mWidth, mHeight);
				g.SetColor(mBkgColor);
				g.FillRect(1, 1, mWidth - 2, mHeight - 2);
			}
			if (mHorizontal && mThumbImage != null)
			{
				g.DrawImage(mThumbImage, (int)(mVal * (double)(mWidth - mThumbImage.GetCelWidth())), (mHeight - mThumbImage.GetCelHeight()) / 2);
			}
			else if (!mHorizontal && mThumbImage != null)
			{
				g.DrawImage(mThumbImage, (mWidth - mThumbImage.GetCelWidth()) / 2, (int)(mVal * (double)(mHeight - mThumbImage.GetCelHeight())));
			}
			else if (mThumbImage == null)
			{
				g.SetColor(mSliderColor);
				if (mHorizontal)
				{
					g.FillRect((int)(mVal * (double)(mWidth - mKnobSize)), 0, mKnobSize, mHeight);
				}
				else
				{
					g.FillRect(0, (int)(mVal * (double)(mHeight - mKnobSize)), mWidth, mKnobSize);
				}
			}
		}

		public override void MouseMove(int x, int y)
		{
			if (mHorizontal)
			{
				int num = ((mThumbImage == null) ? mKnobSize : mThumbImage.GetCelWidth());
				int num2 = (int)(mVal * (double)(mWidth - num));
				if (x >= num2 && x < num2 + num)
				{
					mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_DRAGGING);
				}
				else
				{
					mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_POINTER);
				}
			}
			else
			{
				int num3 = ((mThumbImage == null) ? mKnobSize : mThumbImage.GetCelHeight());
				int num4 = (int)(mVal * (double)(mHeight - num3));
				if (y >= num4 && y < num4 + num3)
				{
					mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_DRAGGING);
				}
				else
				{
					mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_POINTER);
				}
			}
		}

		public override void MouseDown(int x, int y, int theClickCount)
		{
			if (mHorizontal)
			{
				int num = ((mThumbImage == null) ? mKnobSize : mThumbImage.GetCelWidth());
				int num2 = (int)(mVal * (double)(mWidth - num));
				if (x >= num2 && x < num2 + num)
				{
					mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_DRAGGING);
					mDragging = true;
					mRelX = x - num2;
				}
				else
				{
					double value = (double)x / (double)mWidth;
					SetValue(value);
				}
			}
			else
			{
				int num3 = ((mThumbImage == null) ? mKnobSize : mThumbImage.GetCelHeight());
				int num4 = (int)(mVal * (double)(mHeight - num3));
				if (y >= num4 && y < num4 + num3)
				{
					mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_DRAGGING);
					mDragging = true;
					mRelY = y - num4;
				}
				else
				{
					double value2 = (double)y / (double)mHeight;
					SetValue(value2);
				}
			}
		}

		public override void MouseDrag(int x, int y)
		{
			if (mDragging)
			{
				double num = mVal;
				if (mHorizontal)
				{
					int num2 = ((mThumbImage == null) ? mKnobSize : mThumbImage.GetCelWidth());
					mVal = (double)(x - mRelX) / (double)(mWidth - num2);
				}
				else
				{
					int num3 = ((mThumbImage == null) ? mKnobSize : mThumbImage.GetCelHeight());
					mVal = (double)(y - mRelY) / (double)(mHeight - num3);
				}
				if (mVal < 0.0)
				{
					mVal = 0.0;
				}
				if (mVal > 1.0)
				{
					mVal = 1.0;
				}
				if (mVal != num)
				{
					mListener.SliderVal(mId, mVal);
					MarkDirtyFull();
				}
			}
		}

		public override void MouseUp(int x, int y)
		{
			mDragging = false;
			mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_POINTER);
			mListener.SliderVal(mId, mVal);
			mListener.SliderReleased(mId, mVal);
		}

		public override void MouseLeave()
		{
			if (!mDragging)
			{
				mWidgetManager.mApp.SetCursor(ECURSOR.CURSOR_POINTER);
			}
		}

		public override void GamepadButtonDown(GamepadButton theButton, int player, uint flags)
		{
			switch (theButton)
			{
			case GamepadButton.GAMEPAD_BUTTON_LEFT:
				if (mStepMode)
				{
					SetStepValue(mCurStep - 1);
				}
				else
				{
					mSlidingLeft = true;
				}
				break;
			case GamepadButton.GAMEPAD_BUTTON_RIGHT:
				if (mStepMode)
				{
					SetStepValue(mCurStep + 1);
				}
				else
				{
					mSlidingRight = true;
				}
				break;
			default:
				base.GamepadButtonDown(theButton, player, flags);
				break;
			}
		}

		public override void GamepadButtonUp(GamepadButton theButton, int player, uint flags)
		{
			switch (theButton)
			{
			case GamepadButton.GAMEPAD_BUTTON_LEFT:
				mSlidingLeft = false;
				break;
			case GamepadButton.GAMEPAD_BUTTON_RIGHT:
				mSlidingRight = false;
				break;
			default:
				base.GamepadButtonUp(theButton, player, flags);
				break;
			}
		}
	}
}
