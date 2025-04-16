using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class OptionsContainer : Bej3Widget, CheckboxListener, SliderListener
	{
		private enum OptionsMenuContainer_BUTTON_IDS
		{
			CHK_AUTOHINT_ID,
			CHK_MUTE_ID,
			CHK_TUTORIAL_ID,
			SLD_MUSIC_ID,
			SLD_FX_ID,
			SLD_VOICE_ID
		}

		private Label mMuteLabel;

		public Bej3Checkbox mMuteCheckbox;

		private Label mTutorialLabel;

		private Bej3Checkbox mTutorialCheckbox;

		private Label mAutoHintLabel;

		private Bej3Checkbox mAutoHintCheckbox;

		private Bej3Slider mMusicSlider;

		private Bej3Slider mFXSlider;

		private Bej3Slider mVoiceSlider;

		private ImageWidget mVoiceImage;

		private ImageWidget mFXImage;

		private ImageWidget mMusicImage;

		public OptionsContainer()
			: base(Menu_Type.MENU_OPTIONSMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mDoesSlideInFromBottom = false;
			Resize(0, ConstantsWP.OPTIONSMENU_CONTAINER_Y, ConstantsWP.OPTIONSMENU_CONTAINER_W, ConstantsWP.OPTIONSMENU_CONTAINER_H);
			mAutoHintLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mAutoHintLabel.Resize(ConstantsWP.OPTIONSMENU_AUTOHINT_LABEL_X, ConstantsWP.OPTIONSMENU_AUTOHINT_LABEL_Y, 0, 0);
			mAutoHintLabel.SetText(GlobalMembers._ID("Auto-Hint", 3415));
			AddWidget(mAutoHintLabel);
			mMuteLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("Mute All Sounds", 3416), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mMuteLabel.Resize(ConstantsWP.OPTIONSMENU_MUTE_LABEL_X, ConstantsWP.OPTIONSMENU_MUTE_LABEL_Y, 0, 0);
			AddWidget(mMuteLabel);
			mTutorialLabel = new Label(GlobalMembersResources.FONT_SUBHEADER, GlobalMembers._ID("Help", 3417), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mTutorialLabel.Resize(ConstantsWP.OPTIONSMENU_TUTORIAL_LABEL_X, ConstantsWP.OPTIONSMENU_TUTORIAL_LABEL_Y, 0, 0);
			AddWidget(mTutorialLabel);
			mVoiceImage = new ImageWidget(1365);
			mVoiceImage.Resize((int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_SFX_ICONS_VOICES_ID)) + ConstantsWP.OPTIONSMENU_VOICE_LABEL_X, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_SFX_ICONS_VOICES_ID)) + ConstantsWP.OPTIONSMENU_VOICE_LABEL_Y, 0, 0);
			AddWidget(mVoiceImage);
			mFXImage = new ImageWidget(1363);
			mFXImage.Resize((int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_SFX_ICONS_SOUND_ID)) + ConstantsWP.OPTIONSMENU_FX_LABEL_X, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_SFX_ICONS_SOUND_ID)) + ConstantsWP.OPTIONSMENU_FX_LABEL_Y, 0, 0);
			AddWidget(mFXImage);
			mMusicImage = new ImageWidget(1361);
			mMusicImage.Resize((int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_SFX_ICONS_MUSIC_ID)) + ConstantsWP.OPTIONSMENU_MUSIC_LABEL_X, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_SFX_ICONS_MUSIC_ID)) + ConstantsWP.OPTIONSMENU_MUSIC_LABEL_Y, 0, 0);
			AddWidget(mMusicImage);
			mAutoHintCheckbox = new Bej3Checkbox(0, this);
			mAutoHintCheckbox.Resize(ConstantsWP.OPTIONSMENU_AUTOHINT_CHECKBOX_X, ConstantsWP.OPTIONSMENU_AUTOHINT_CHECKBOX_Y, 0, 0);
			mAutoHintCheckbox.mGrayOutWhenDisabled = false;
			AddWidget(mAutoHintCheckbox);
			mMuteCheckbox = new Bej3Checkbox(1, this);
			mMuteCheckbox.Resize(ConstantsWP.OPTIONSMENU_MUTE_CHECKBOX_X, ConstantsWP.OPTIONSMENU_MUTE_CHECKBOX_Y, 0, 0);
			mMuteCheckbox.mGrayOutWhenDisabled = false;
			AddWidget(mMuteCheckbox);
			mTutorialCheckbox = new Bej3Checkbox(2, this);
			mTutorialCheckbox.Resize(ConstantsWP.OPTIONSMENU_TUTORIAL_CHECKBOX_X, ConstantsWP.OPTIONSMENU_TUTORIAL_CHECKBOX_Y, 0, 0);
			mTutorialCheckbox.mGrayOutWhenDisabled = false;
			AddWidget(mTutorialCheckbox);
			mMusicSlider = new Bej3Slider(3, this);
			mMusicSlider.Resize(ConstantsWP.OPTIONSMENU_MUSIC_SLIDER_X, ConstantsWP.OPTIONSMENU_MUSIC_SLIDER_Y, ConstantsWP.OPTIONSMENU_SLIDER_WIDTH, 0);
			AddWidget(mMusicSlider);
			mFXSlider = new Bej3Slider(4, this);
			mFXSlider.Resize(ConstantsWP.OPTIONSMENU_MUSIC_SLIDER_X, ConstantsWP.OPTIONSMENU_FX_SLIDER_Y, ConstantsWP.OPTIONSMENU_SLIDER_WIDTH, 0);
			AddWidget(mFXSlider);
			mVoiceSlider = new Bej3Slider(5, this);
			mVoiceSlider.Resize(ConstantsWP.OPTIONSMENU_MUSIC_SLIDER_X, ConstantsWP.OPTIONSMENU_VOICE_SLIDER_Y, ConstantsWP.OPTIONSMENU_SLIDER_WIDTH, 0);
			AddWidget(mVoiceSlider);
			Show();
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public override void Show()
		{
			int num = mY;
			base.Show();
			mY = num;
		}

		public override void PlayMenuMusic()
		{
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			if (!(g.mTransY >= (float)SexyFramework.GlobalMembers.gSexyAppBase.mHeight))
			{
				base.DrawAll(theFlags, g);
			}
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawLightBox(g, new Rect(ConstantsWP.OPTIONSMENU_BOX_1_X, ConstantsWP.OPTIONSMENU_BOX_1_Y, ConstantsWP.OPTIONSMENU_BOX_1_W, ConstantsWP.OPTIONSMENU_BOX_1_H));
			Bej3Widget.DrawLightBox(g, new Rect(ConstantsWP.OPTIONSMENU_BOX_2_X, ConstantsWP.OPTIONSMENU_BOX_2_Y, ConstantsWP.OPTIONSMENU_BOX_2_W, ConstantsWP.OPTIONSMENU_BOX_2_H));
		}

		public void CheckboxChecked(int theId, bool isChecked)
		{
			switch (theId)
			{
			case 0:
				GlobalMembers.gApp.mProfile.mAutoHint = isChecked;
				break;
			case 1:
				if (isChecked)
				{
					if (GlobalMembers.gApp.mMuteCount <= 0)
					{
						GlobalMembers.gApp.Mute();
					}
				}
				else if (GlobalMembers.gApp.mMuteCount > 0)
				{
					GlobalMembers.gApp.Unmute();
				}
				UpdateControls();
				break;
			case 2:
				GlobalMembers.gApp.mProfile.SetTutorialCleared(19, !isChecked);
				break;
			}
		}

		public void SliderVal(int theId, double theVal)
		{
			switch (theId)
			{
			case 3:
				GlobalMembers.gApp.SetMusicVolume(theVal);
				if (GlobalMembers.gApp.mGameInProgress && GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN && GlobalMembers.gApp.mBoard != null)
				{
					(GlobalMembers.gApp.mBoard as ZenBoard)?.MusicVolumeChanged();
				}
				UpdateControls();
				break;
			case 4:
				GlobalMembers.gApp.SetSfxVolume(theVal);
				break;
			case 5:
				GlobalMembers.gApp.mVoiceVolume = theVal;
				if (!GlobalMembers.gApp.IsMuted())
				{
					GlobalMembers.gApp.mSoundManager.SetVolume(1, GlobalMembers.gApp.mVoiceVolume);
				}
				break;
			}
		}

		public void SliderReleased(int theId, double theVal)
		{
			switch (theId)
			{
			case 4:
				if (!GlobalMembers.gApp.IsMuted())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_COMBO_2);
				}
				break;
			case 5:
				if (!GlobalMembers.gApp.IsMuted())
				{
					GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_GO);
				}
				break;
			}
			UpdateControls();
		}

		public override void LinkUpAssets()
		{
			mAutoHintCheckbox.LinkUpAssets();
			mMuteCheckbox.LinkUpAssets();
			mMusicSlider.LinkUpAssets();
			mFXSlider.LinkUpAssets();
			mVoiceSlider.LinkUpAssets();
			mMuteCheckbox.SetChecked(GlobalMembers.gApp.IsMuted(), false);
			mTutorialCheckbox.SetChecked(!GlobalMembers.gApp.mProfile.HasClearedTutorial(19), false);
			UpdateControls();
			base.LinkUpAssets();
			UpdateValues();
		}

		public void UpdateValues()
		{
			mMusicSlider.SetValue(GlobalMembers.gApp.mMusicVolume);
			mFXSlider.SetValue(GlobalMembers.gApp.mSfxVolume);
			mVoiceSlider.SetValue(GlobalMembers.gApp.mVoiceVolume);
			mAutoHintCheckbox.SetChecked(GlobalMembers.gApp.mProfile.mAutoHint, false);
		}

		public void UpdateControls()
		{
			bool flag = GlobalMembers.gApp.mMuteCount > 0;
			GlobalMembers.gApp.IsMusicEnabled();
			mMusicSlider.SetDisabled(flag);
			mVoiceSlider.SetDisabled(flag);
			mFXSlider.SetDisabled(flag);
			if (flag || GlobalMembers.gApp.GetMusicVolume() == 0.0)
			{
				mMusicImage.SetImage(1362);
				mMusicImage.mGrayedOut = true;
				mMusicSlider.mGrayedOut = true;
			}
			else
			{
				mMusicImage.SetImage(1361);
				mMusicImage.mGrayedOut = false;
				mMusicSlider.mGrayedOut = false;
			}
			if (flag || GlobalMembers.gApp.mVoiceVolume == 0.0)
			{
				mVoiceImage.SetImage(1366);
				mVoiceImage.mGrayedOut = true;
				mVoiceSlider.mGrayedOut = true;
			}
			else
			{
				mVoiceImage.SetImage(1365);
				mVoiceImage.mGrayedOut = false;
				mVoiceSlider.mGrayedOut = false;
			}
			if (flag || GlobalMembers.gApp.GetSfxVolume() == 0.0)
			{
				mFXImage.SetImage(1364);
				mFXImage.mGrayedOut = true;
				mFXSlider.mGrayedOut = true;
			}
			else
			{
				mFXImage.SetImage(1363);
				mFXImage.mGrayedOut = false;
				mFXSlider.mGrayedOut = false;
			}
			mMusicSlider.LinkUpAssets();
			mVoiceSlider.LinkUpAssets();
			mFXSlider.LinkUpAssets();
		}

		public override void SetDisabled(bool isDisabled)
		{
			base.SetDisabled(isDisabled);
			UpdateControls();
		}
	}
}
