using SexyFramework.Graphics;
using SexyFramework.Resource;
using BejeweledLivePlus.Widget;

namespace BejeweledLivePlus.UI
{
	public class ProfileMenuBase : Bej3Widget
	{
		private static int loadedGroup = -1;

		private SharedImageRef mPlayerImageRef;

		protected bool mLoading;

		public ImageWidget mPlayerImage;

		public int mLoadedProfilePictureId;

		public ProfileMenuBase(Menu_Type type, bool hasCloseButton, Bej3ButtonType topButtonType)
			: base(type, hasCloseButton, topButtonType)
		{
			mLoading = false;
			mLoadedProfilePictureId = -1;
		}

		protected bool SetPlayerImage(int profilePictureId)
		{
			int imageId = profilePictureId + (int)ProfilePictureConstants.FIRST_PROFILE_PICTURE_BIG;
			ResourceRef resourceRef = GlobalMembers.gApp.mResourceManager.GetImageRef(GlobalMembersResourcesWP.GetStringIdById(imageId));
			if (resourceRef == null)
			{
				return false;
			}
			SharedImageRef sharedImageRef = resourceRef.GetSharedImageRef();
			mPlayerImage.SetImage(imageId);
			bool result = sharedImageRef != null && sharedImageRef.GetImage() != null && mPlayerImage.GetImage() != null;
			if (result)
			{
				mPlayerImageRef?.Release();
				mPlayerImageRef = sharedImageRef;
			}
			else
			{
				sharedImageRef?.Release();
			}
			resourceRef.Release();
			return result;
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
					if (SetPlayerImage(num2))
					{
						mLoading = false;
						return;
					}
					BejeweledLivePlusApp.UnloadContent($"ProfilePic_{num2}", true);
				}
				UnloadPlayerImages();
				BejeweledLivePlusApp.LoadContent($"ProfilePic_{num2}", false);
				loadedGroup = SetPlayerImage(num2) ? num2 : -1;
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
					if (SetPlayerImage(num))
					{
						mLoading = false;
						return;
					}
					BejeweledLivePlusApp.UnloadContent($"ProfilePic_{num}", true);
				}
				UnloadPlayerImages();
				BejeweledLivePlusApp.LoadContent($"ProfilePic_{num}", false);
				loadedGroup = SetPlayerImage(num) ? num : -1;
			}
			mLoading = false;
		}

		public virtual void UnloadPlayerImages(int exceptThis)
		{
			for (int i = 0; i < (int)ProfilePictureConstants.NUMBER_OF_PROFILE_IMAGES; i++)
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
			for (int i = 0; i < (int)ProfilePictureConstants.NUMBER_OF_PROFILE_IMAGES; i++)
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
				mPlayerImageRef?.Release();
				mPlayerImageRef = null;
				UnloadPlayerImages(-2);
			}
		}
	}
}
