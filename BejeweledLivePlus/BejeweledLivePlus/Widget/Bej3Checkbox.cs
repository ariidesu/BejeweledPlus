using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3Checkbox : Checkbox
	{
		private int mUncheckImageId;

		private int mCheckedImageId;

		public bool mGrayedOut;

		public bool mGrayOutWhenDisabled;

		public bool mClippingEnabled;

		private static int x = 0;

		private static int y = 0;

		private static Point checkOffset = default(Point);

		public Bej3Checkbox(int theId, CheckboxListener theCheckboxListener, int theUncheckedImage, int theCheckedImage)
			: base(GlobalMembersResourcesWP.GetImageById(theUncheckedImage), GlobalMembersResourcesWP.GetImageById(theCheckedImage), theId, theCheckboxListener)
		{
			mGrayOutWhenDisabled = true;
			mUncheckImageId = theUncheckedImage;
			mCheckedImageId = theCheckedImage;
			mGrayedOut = false;
			mClippingEnabled = false;
		}

		public Bej3Checkbox(int theId, CheckboxListener theCheckboxListener, int theUncheckedImage)
			: this(theId, theCheckboxListener, theUncheckedImage, 1334)
		{
		}

		public Bej3Checkbox(int theId, CheckboxListener theCheckboxListener)
			: this(theId, theCheckboxListener, 1333, 1334)
		{
		}

		public override void Draw(Graphics g)
		{
			if (!mClippingEnabled)
			{
				g.ClearClipRect();
			}
			checkOffset = new Point((int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1334) - GlobalMembersResourcesWP.ImgXOfs(1333)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1334) - GlobalMembersResourcesWP.ImgYOfs(1333)));
			x = mWidth / 2 - mUncheckedImage.GetCelWidth() / 2;
			y = mHeight / 2 - mUncheckedImage.GetCelHeight() / 2;
			Image image = null;
			image = ((!mGrayedOut) ? mUncheckedImage : GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX_UNSELECTED);
			g.DrawImage(image, x, y);
			if (mChecked)
			{
				if (mGrayedOut)
				{
					g.SetColorizeImages(true);
					g.SetColor(Bej3WidgetBase.GreyedOutColor);
				}
				g.DrawImage(mCheckedImage, x + checkOffset.mX, y + checkOffset.mY);
			}
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			theWidth = ConstantsWP.BEJ3CHECKBOX_SIZE;
			theHeight = ConstantsWP.BEJ3CHECKBOX_SIZE;
			theX -= theWidth / 2;
			theY -= theHeight / 2;
			base.Resize(theX, theY, theWidth, theHeight);
		}

		public void LinkUpAssets()
		{
			mUncheckedImage = GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX;
			mCheckedImage = GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX_CHECKED;
		}

		public override void SetDisabled(bool isDisabled)
		{
			base.SetDisabled(isDisabled);
			if (mGrayOutWhenDisabled)
			{
				mGrayedOut = mDisabled;
			}
		}

		public void SetClippingEnabled(bool enable)
		{
		}

		public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
		{
			mChecked = !mChecked;
			if (mListener != null)
			{
				mListener.CheckboxChecked(mId, mChecked);
			}
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_TICK);
			MarkDirty();
		}
	}
}
