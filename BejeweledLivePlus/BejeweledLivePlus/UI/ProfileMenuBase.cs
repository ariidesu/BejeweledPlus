using BejeweledLivePlus.Widget;

namespace BejeweledLivePlus.UI
{
	public class ProfileMenuBase : Bej3Widget
	{
		private static int loadedGroup;

		protected bool mLoading;

		public ImageWidget mPlayerImage;

		public int mLoadedProfilePictureId;

		public ProfileMenuBase(Menu_Type type, bool hasCloseButton, Bej3ButtonType topButtonType)
			: base(type, hasCloseButton, topButtonType)
		{
			mLoading = false;
			mLoadedProfilePictureId = -1;
		}

		public virtual void SetUpPlayerImage()
		{
			int num = -1;
			if (mState == Bej3WidgetState.STATE_OUT || mPlayerImage == null)
			{
				return;
			}
			mLoading = true;
			if (num != 0 || GlobalMembers.gApp.mProfile.UsesPresetProfilePicture())
			{
				int num2 = (mLoadedProfilePictureId = ((num < 0) ? GlobalMembers.gApp.mProfile.GetProfilePictureId() : num));
				if (loadedGroup == num2)
				{
					mPlayerImage.SetImage(num2 + 712);
					mLoading = false;
					return;
				}
				UnloadPlayerImages();
				BejeweledLivePlusApp.LoadContent($"ProfilePic_{num2}", false);
				mPlayerImage.SetImage(num2 + 712);
				loadedGroup = num2;
			}
			mLoading = false;
		}

		public virtual void SetUpPlayerImage(int overridePresetId)
		{
			if (mState == Bej3WidgetState.STATE_OUT || mPlayerImage == null)
			{
				return;
			}
			mLoading = true;
			if (overridePresetId != 0 || GlobalMembers.gApp.mProfile.UsesPresetProfilePicture())
			{
				int num = (mLoadedProfilePictureId = ((overridePresetId < 0) ? GlobalMembers.gApp.mProfile.GetProfilePictureId() : overridePresetId));
				if (loadedGroup == num)
				{
					mPlayerImage.SetImage(num + 712);
					mLoading = false;
					return;
				}
				UnloadPlayerImages();
				BejeweledLivePlusApp.LoadContent($"ProfilePic_{num}", false);
				mPlayerImage.SetImage(num + 712);
				loadedGroup = num;
			}
			mLoading = false;
		}

		public virtual void UnloadPlayerImages(int exceptThis)
		{
			for (int i = 0; i < 30; i++)
			{
				if (i != exceptThis && i != GlobalMembers.gApp.mProfile.GetProfilePictureId())
				{
					BejeweledLivePlusApp.UnloadContent($"ProfilePic_{i}");
				}
			}
			loadedGroup = -1;
		}

		public virtual void UnloadPlayerImages()
		{
			int num = -1;
			for (int i = 0; i < 30; i++)
			{
				if (i != num && i != GlobalMembers.gApp.mProfile.GetProfilePictureId())
				{
					BejeweledLivePlusApp.UnloadContent($"ProfilePic_{i}");
				}
			}
			loadedGroup = -1;
		}

		public override void Show()
		{
			base.Show();
			SetUpPlayerImage();
			ResetFadedBack(true);
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
			if (mInterfaceState != InterfaceState.INTERFACE_STATE_PROFILEMENU && mInterfaceState != InterfaceState.INTERFACE_STATE_EDITPROFILEMENU && mInterfaceState != InterfaceState.INTERFACE_STATE_STATSMENU)
			{
				UnloadPlayerImages(-2);
			}
		}
	}
}
