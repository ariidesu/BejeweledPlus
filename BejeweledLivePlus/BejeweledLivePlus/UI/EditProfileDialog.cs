using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class EditProfileDialog : ProfileMenuBase, ScrollWidgetListener, Bej3EditListener, EditListener
	{
		private enum EditProfileDialog_IDS
		{
			BTN_SAVE_ID,
			BTN_EDIT_NAME_ID
		}

		private bool mDrawnSinceChange;

		private Label mGetPicLabel;

		private Bej3Button mSaveButton;

		private ScrollWidget mImageLibraryScrollWidget;

		private EditProfileDialogImageContainer mContainer;

		private int mSelectedProfilePicture;

		private Label mHeadingLabel;

		private Bej3Button mEditNameButton;

		private Label mPlayerNameLabel;

		public string mDisplayName;

		public bool mFirstTime;

		public virtual void EditWidgetText(int id, string text)
		{
		}

		private void HighlightSelectedButton()
		{
			int num = ((mSelectedProfilePicture < 0) ? GlobalMembers.gApp.mProfile.GetProfilePictureId() : mSelectedProfilePicture);
			if (num >= 0 && num < 30)
			{
				mContainer.mSelection = new Point(mContainer.mImageLibrary[num].mX + mContainer.mImageLibrary[num].mWidth / 2, mContainer.mImageLibrary[num].mY + mContainer.mImageLibrary[num].mHeight / 2);
				mContainer.mSelectedImg = GlobalMembersResourcesWP.GetImageById(742 + num);
			}
			if (mState == Bej3WidgetState.STATE_IN || mState == Bej3WidgetState.STATE_FADING_IN)
			{
				GlobalMembers.gApp.mProfile.SetProfilePictureId(num);
				GlobalMembers.gApp.mProfile.WriteProfile();
				GlobalMembers.gApp.mProfile.WriteProfileList();
			}
		}

		public EditProfileDialog()
			: base(Menu_Type.MENU_EDITPROFILEMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			mFirstTime = false;
			mSelectedProfilePicture = -1;
			mDrawnSinceChange = false;
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, ConstantsWP.EDITPROFILEMENU_WIDTH, GlobalMembers.gApp.mHeight);
			mFinalY = 0;
			mUpdateWhenNotVisible = true;
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.EDITPROFILEMENU_HEADING_X, ConstantsWP.EDITPROFILEMENU_HEADING_Y, 0, 0);
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			AddWidget(mHeadingLabel);
			mGetPicLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mGetPicLabel.Resize(ConstantsWP.EDITPROFILEMENU_GET_PIC_X, ConstantsWP.EDITPROFILEMENU_GET_PIC_Y, 0, 0);
			mGetPicLabel.SetText(GlobalMembers._ID("Choose a profile picture:", 3284));
			AddWidget(mGetPicLabel);
			mPlayerNameLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mPlayerNameLabel.SetText(GlobalMembers.gApp.mProfile.mProfileName);
			mPlayerNameLabel.Resize(ConstantsWP.EDITPROFILEMENU_HEADING_X, ConstantsWP.EDITPROFILEMENU_HEADING_Y, 0, 0);
			mPlayerNameLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH - 50);
			AddWidget(mPlayerNameLabel);
			mEditNameButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LONG, true);
			mEditNameButton.SetLabel(GlobalMembers._ID("EDIT NAME", 3285));
			Bej3Widget.CenterWidgetAt(ConstantsWP.EDITPROFILEMENU_EDIT_NAME_X, ConstantsWP.EDITPROFILEMENU_EDIT_NAME_Y, mEditNameButton, true, false);
			int num = 100;
			mSaveButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			Bej3Widget.CenterWidgetAt(ConstantsWP.EDITPROFILEMENU_SAVE_X, ConstantsWP.EDITPROFILEMENU_SAVE_Y + num, mSaveButton, true, false);
			AddWidget(mSaveButton);
			mPlayerImage = new ImageWidget(712, true);
			mPlayerImage.Resize(ConstantsWP.EDITPROFILEMENU_PLAYER_IMAGE_X, ConstantsWP.EDITPROFILEMENU_PLAYER_IMAGE_Y, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE);
			AddWidget(mPlayerImage);
			mImageLibraryScrollWidget = new ScrollWidget(this);
			mContainer = new EditProfileDialogImageContainer(this);
			mImageLibraryScrollWidget.Resize(ConstantsWP.EDITPROFILEMENU_IMAGE_LIBRARY_X, ConstantsWP.EDITPROFILEMENU_IMAGE_LIBRARY_Y, ConstantsWP.EDITPROFILEMENU_IMAGE_LIBRARY_WIDTH, ConstantsWP.EDITPROFILEMENU_IMAGE_LIBRARY_HEIGHT);
			mImageLibraryScrollWidget.AddWidget(mContainer);
			mImageLibraryScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_DISABLED);
			AddWidget(mImageLibraryScrollWidget);
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				ButtonDepress(0);
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			int num = (int)(double)mAlphaCurve;
			g.SetColor(new Color(255, 255, 255, 255 * num * num * num));
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.EDITPROFILEMENU_DIVIDER_1_Y);
			mDrawnSinceChange = true;
		}

		public override void ButtonMouseEnter(int theId)
		{
		}

		public override void ButtonDepress(int theId)
		{
			mDrawnSinceChange = false;
			mWidgetManager.SetFocus(null);
			if (theId >= 100000)
			{
				mSelectedProfilePicture = theId - 100000;
				SetUpPlayerImage(mSelectedProfilePicture);
				HighlightSelectedButton();
				return;
			}
			switch (theId)
			{
			case 10001:
				GlobalMembers.gApp.DoMainMenu();
				((MainMenuOptions)GlobalMembers.gApp.mMenus[5]).Expand();
				Transition_SlideOut();
				break;
			case 1:
				GlobalMembers.gApp.DoRenameUserDialog();
				break;
			case 0:
				GlobalMembers.gApp.mProfile.SetProfilePictureId(mPlayerImage.GetImageId() - 712);
				GlobalMembers.gApp.mProfile.RenameProfile(GlobalMembers.gApp.mProfile.mProfileName, mDisplayName);
				GlobalMembers.gApp.mProfile.WriteProfile();
				GlobalMembers.gApp.mProfile.WriteProfileList();
				mSelectedProfilePicture = -1;
				GlobalMembers.gApp.DoMainMenu();
				((MainMenuOptions)GlobalMembers.gApp.mMenus[5]).Expand();
				Transition_SlideOut();
				break;
			}
		}

		public override void LinkUpAssets()
		{
			mEditNameButton.LinkUpAssets();
			mSaveButton.LinkUpAssets();
			SetUpPlayerImage(mSelectedProfilePicture);
			mContainer.LinkUpAssets();
			base.LinkUpAssets();
			HighlightSelectedButton();
		}

		public override void Show()
		{
			SetDisplayedName(GlobalMembers.gApp.mProfile.mProfileName);
			mSelectedProfilePicture = -1;
			mTopButton.SetType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			mContainer.Show();
			base.Show();
			SetVisible(false);
		}

		public override void ShowCompleted()
		{
			mTopButton.SetType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			mTopButton.SetDisabled(false);
		}

		public override void Hide()
		{
			mSelectedProfilePicture = -1;
			base.Hide();
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
		}

		public override bool ButtonsEnabled()
		{
			bool result = base.ButtonsEnabled();
			if (mLoading)
			{
				result = false;
			}
			if (!mDrawnSinceChange)
			{
				result = false;
			}
			return result;
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public virtual void SetupForFirstShow(bool firstTime)
		{
			base.ShowBackButton(!firstTime);
			mFirstTime = firstTime;
			mEditNameButton.mVisible = (mEditNameButton.mMouseVisible = !firstTime);
			mEditNameButton.mDisabled = firstTime;
			if (firstTime)
			{
				mPlayerImage.Resize(ConstantsWP.EDITPROFILEMENU_PLAYER_IMAGE_X_FIRSTTIME, ConstantsWP.EDITPROFILEMENU_PLAYER_IMAGE_Y_FIRSTTIME, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE);
				mPlayerNameLabel.SetFont(GlobalMembersResources.FONT_HUGE);
				mPlayerNameLabel.Resize(ConstantsWP.EDITPROFILEMENU_NAME_LABEL_X_FIRST_TIME, ConstantsWP.EDITPROFILEMENU_NAME_LABEL_Y_FIRST_TIME, 0, 0);
				mHeadingLabel.SetText("");
				mSaveButton.SetLabel(GlobalMembers._ID("CONTINUE", 3573));
				mSaveButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
			}
			else
			{
				mPlayerImage.Resize(ConstantsWP.EDITPROFILEMENU_PLAYER_IMAGE_X, ConstantsWP.EDITPROFILEMENU_PLAYER_IMAGE_Y, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE, ConstantsWP.LARGE_PROFILE_PICTURE_SIZE);
				mPlayerNameLabel.SetFont(GlobalMembersResources.FONT_HUGE);
				mPlayerNameLabel.Resize(ConstantsWP.EDITPROFILEMENU_NAME_LABEL_X, ConstantsWP.EDITPROFILEMENU_NAME_LABEL_Y + 30, 0, 0);
				mHeadingLabel.SetText(GlobalMembers._ID("EDIT PROFILE", 3287));
				mSaveButton.SetLabel(GlobalMembers._ID("BACK", 3288));
				mSaveButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			}
		}

		public void SetDisplayedName(string name)
		{
			mDisplayName = name;
		}

		public void ResetDisplayedPicture()
		{
			mSelectedProfilePicture = -1;
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
		}

		public virtual bool AllowKey(int theId, KeyCode theKey)
		{
			return true;
		}

		public virtual bool AllowChar(int theId, char theChar)
		{
			return true;
		}

		public virtual bool AllowText(int theId, string theText)
		{
			return true;
		}

		public override void GotFocus()
		{
			base.GotFocus();
		}

		public virtual void EditWidgetGotFocus(int theId)
		{
		}

		public virtual void EditWidgetLostFocus(int theId)
		{
		}

		public bool IsEditingName()
		{
			return GlobalMembers.gApp.IsKeyboardShowing();
		}

		public override void SetUpPlayerImage(int overridePresetId)
		{
			if (mState == Bej3WidgetState.STATE_OUT || mPlayerImage == null)
			{
				return;
			}
			bool flag = mLoading;
			mLoading = true;
			if (overridePresetId >= 0 || GlobalMembers.gApp.mProfile.UsesPresetProfilePicture())
			{
				int num = (mLoadedProfilePictureId = ((overridePresetId < 0) ? GlobalMembers.gApp.mProfile.GetProfilePictureId() : overridePresetId));
				for (int i = 0; i < 30; i++)
				{
					BejeweledLivePlusApp.LoadContent($"ProfilePic_{i}", false);
				}
				mPlayerImage.SetImage(num + 712);
			}
			if (!flag)
			{
				LinkUpAssets();
			}
			mLoading = false;
		}
	}
}
