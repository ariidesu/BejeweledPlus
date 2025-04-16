using SexyFramework.Drivers;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public class Checkbox : Widget
	{
		protected CheckboxListener mListener;

		public int mId;

		public bool mChecked;

		public Image mUncheckedImage;

		public Image mCheckedImage;

		public Rect mCheckedRect = default(Rect);

		public Rect mUncheckedRect = default(Rect);

		public Color mOutlineColor = default(Color);

		public Color mBkgColor = default(Color);

		public Color mCheckColor = default(Color);

		public virtual void SetChecked(bool isChecked)
		{
			SetChecked(isChecked, true);
		}

		public virtual void SetChecked(bool @checked, bool tellListener)
		{
			mChecked = @checked;
			if (tellListener && mListener != null)
			{
				mListener.CheckboxChecked(mId, mChecked);
			}
			MarkDirty();
		}

		public virtual bool IsChecked()
		{
			return mChecked;
		}

		public override void MouseDown(int x, int y, int theClickCount)
		{
			base.MouseDown(x, y, theClickCount);
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
			base.MouseDown(x, y, theBtnNum, theClickCount);
		}

		public override void MouseUp(int x, int y, int theClickCount)
		{
			base.MouseUp(x, y, theClickCount);
		}

		public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
		{
			base.MouseUp(x, y, theBtnNum, theClickCount);
			mChecked = !mChecked;
			if (mListener != null)
			{
				mListener.CheckboxChecked(mId, mChecked);
			}
			MarkDirty();
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			base.Draw(g);
			if (mCheckedRect.mWidth == 0 && mCheckedImage != null && mUncheckedImage != null)
			{
				if (mChecked)
				{
					g.DrawImage(mCheckedImage, 0, 0);
				}
				else
				{
					g.DrawImage(mUncheckedImage, 0, 0);
				}
			}
			else if (mCheckedRect.mWidth != 0 && mUncheckedImage != null)
			{
				if (mChecked)
				{
					g.DrawImage(mUncheckedImage, 0, 0, mCheckedRect);
				}
				else
				{
					g.DrawImage(mUncheckedImage, 0, 0, mUncheckedRect);
				}
			}
			else if (mUncheckedImage == null && mCheckedImage == null)
			{
				g.SetColor(mOutlineColor);
				g.FillRect(0, 0, mWidth, mHeight);
				g.SetColor(mBkgColor);
				g.FillRect(1, 1, mWidth - 2, mHeight - 2);
				if (mChecked)
				{
					g.SetColor(mCheckColor);
					g.DrawLine(1, 1, mWidth - 2, mHeight - 2);
					g.DrawLine(mWidth - 1, 1, 1, mHeight - 2);
				}
			}
		}

		public override void GamepadButtonDown(GamepadButton theButton, int player, uint flags)
		{
			if (theButton == GamepadButton.GAMEPAD_BUTTON_A)
			{
				if ((flags & 1) == 0)
				{
					OnPressed();
					mIsDown = true;
					mChecked = !mChecked;
					if (mListener != null)
					{
						mListener.CheckboxChecked(mId, mChecked);
					}
					MarkDirty();
				}
			}
			else
			{
				base.GamepadButtonDown(theButton, player, flags);
			}
		}

		public override void GamepadButtonUp(GamepadButton theButton, int player, uint flags)
		{
			if (theButton != GamepadButton.GAMEPAD_BUTTON_A)
			{
				base.GamepadButtonUp(theButton, player, flags);
			}
		}

		public virtual void OnPressed()
		{
		}

		public Checkbox(Image theUncheckedImage, Image theCheckedImage, int theId, CheckboxListener theCheckboxListener)
		{
			mUncheckedImage = theUncheckedImage;
			mCheckedImage = theCheckedImage;
			mId = theId;
			mListener = theCheckboxListener;
			mChecked = false;
			mOutlineColor = new Color(Color.White);
			mBkgColor = new Color(new Color(80, 80, 80));
			mCheckColor = new Color(new Color(255, 255, 0));
			mDoFinger = true;
		}
	}
}
