using System.Collections.Generic;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class ZenOptionsContainer : Bej3Widget, SliderListener, CheckboxListener, Bej3SlideSelectorListener
	{
		public ZenOptionsMode mMode;

		public List<SexyFramework.Widget.Widget> mAmbientSoundWidgets = new List<SexyFramework.Widget.Widget>();

		public List<SexyFramework.Widget.Widget> mMantrasWidgets = new List<SexyFramework.Widget.Widget>();

		public List<SexyFramework.Widget.Widget> mBreathModWidgets = new List<SexyFramework.Widget.Widget>();

		public Label mDescLabel;

		public bool mEnabled;

		public int mTotalHeight;

		public Label mBreathMod_DescLabel;

		public Bej3Checkbox mBreathModCheckbox;

		public Label mBreathModEnable;

		public Label mBreathMod_VisualIndicatorLabel;

		public Bej3Checkbox mBreathMod_VisualIndicatorCheckbox;

		public Label mBreathMod_SpeedLabel;

		public Bej3Slider mBreathMod_SpeedSlider;

		public Label mBreathMod_VolumeLabel;

		public Bej3Slider mBreathMod_VolumeSlider;

		public int mAmbientSoundStartDelay;

		public int mAmbientSoundId;

		public Label mAmbientSoundDescLabel;

		public Label mAmbientVolumeLabel;

		public Bej3Slider mAmbientVolumeSlider;

		public Bej3SlideSelector mAmbientSoundSelector;

		public Label mAmbientSoundItemNameLabel;

		public Bej3Button mAmbientSwipeLeftButton;

		public Bej3Button mAmbientSwipeRightButton;

		public Label mMantra_DescLabel;

		public Label mMantra_SubliminalLabel;

		public Bej3Checkbox mMantra_SubliminalCheckbox;

		public Label mMantra_SpeedVisibilityLabel;

		public Bej3Slider mMantra_SpeedVisibilitySlider;

		public Bej3SlideSelector mMantraSelector;

		public Label mMantraItemNameLabel;

		public Bej3Button mMantraSwipeLeftButton;

		public Bej3Button mMantraSwipeRightButton;

		private static string[] ambientSounds = new string[8]
		{
			GlobalMembers._ID("None", 3461),
			GlobalMembers._ID("Random", 3462),
			GlobalMembers._ID("Ocean Surf", 3463),
			GlobalMembers._ID("Crickets", 3464),
			GlobalMembers._ID("Rain Leaves", 3465),
			GlobalMembers._ID("Waterfall", 3466),
			GlobalMembers._ID("Coastal", 3467),
			GlobalMembers._ID("Forest", 3468)
		};

		private static string[] mantras = new string[7]
		{
			GlobalMembers._ID("None", 3469),
			GlobalMembers._ID("General", 3470),
			GlobalMembers._ID("Positive Thinking", 3471),
			GlobalMembers._ID("Quit Bad Habits", 3472),
			GlobalMembers._ID("Prosperity", 3473),
			GlobalMembers._ID("Self-Confidence", 3474),
			GlobalMembers._ID("Weight Loss", 3475)
		};

		public ZenOptionsContainer()
			: base(Menu_Type.MENU_ZENOPTIONSMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mDoesSlideInFromBottom = false;
			mEnabled = true;
			mAmbientSoundWidgets.Clear();
			mMantrasWidgets.Clear();
			mBreathModWidgets.Clear();
			mAmbientSoundStartDelay = 0;
			mAmbientSoundId = -1;
			int zENOPTIONS_AMBIENT_SOUND_DESC_Y = ConstantsWP.ZENOPTIONS_AMBIENT_SOUND_DESC_Y;
			mAmbientSoundDescLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Replace the game music with environmental audio tracks for greater relaxation and focus.", 3622), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE);
			mAmbientSoundDescLabel.SetTextBlock(new Rect((GlobalMembers.gApp.mWidth - ConstantsWP.ZENOPTIONS_DESC_WIDTH) / 2, ConstantsWP.ZENOPTIONS_DESC_Y, ConstantsWP.ZENOPTIONS_DESC_WIDTH, ConstantsWP.ZENOPTIONS_AMBIENT_SOUND_DESC_HEIGHT), true);
			mAmbientSoundDescLabel.SetTextBlockEnabled(true);
			mAmbientSoundDescLabel.SetTextBlockAlignment(0);
			AddWidget(mAmbientSoundDescLabel);
			mAmbientSoundWidgets.Add(mAmbientSoundDescLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.ZENOPTIONS_AMBIENT_SOUND_SLIDE_OFFSET_Y;
			mAmbientSoundSelector = new Bej3SlideSelector(9, this, this, ConstantsWP.ZENOPTIONS_AMBIENCE_ITEM_SIZE, ConstantsWP.ZENOPTIONS_AMBIENCE_WIDTH);
			mAmbientSoundSelector.Resize(0, zENOPTIONS_AMBIENT_SOUND_DESC_Y, ConstantsWP.ZENOPTIONS_AMBIENT_SOUND_SELECTOR_WIDTH, ConstantsWP.ZENOPTIONS_AMBIENT_SOUND_SELECTOR_HEIGHT + 75);
			mAmbientSoundSelector.SetThreshold(ConstantsWP.ZENOPTIONS_SLIDER_HOR_THRESHOLD, 0);
			mAmbientSoundSelector.AddItem(1, 779);
			mAmbientSoundSelector.AddItem(2, 790);
			mAmbientSoundSelector.AddItem(3, 785);
			mAmbientSoundSelector.AddItem(4, 781);
			mAmbientSoundSelector.AddItem(5, 789);
			mAmbientSoundSelector.AddItem(6, 792);
			mAmbientSoundSelector.AddItem(7, 780);
			mAmbientSoundSelector.AddItem(8, 782);
			AddWidget(mAmbientSoundSelector);
			mAmbientSoundWidgets.Add(mAmbientSoundSelector);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.ZENOPTIONS_AMBIENT_SOUND_SELECTOR_HEIGHT + ConstantsWP.ZENOPTIONS_SLIDE_ITEM_LABEL_OFFSET_Y;
			mAmbientSoundItemNameLabel = new Label(GlobalMembersResources.FONT_DIALOG, "");
			mAmbientSoundItemNameLabel.Resize(ConstantsWP.ZENOPTIONS_CENTER_X, zENOPTIONS_AMBIENT_SOUND_DESC_Y, 0, 0);
			mAmbientSoundItemNameLabel.SetLayerColor(0, Bej3Widget.COLOR_DIALOG_WHITE);
			AddWidget(mAmbientSoundItemNameLabel);
			mAmbientSoundWidgets.Add(mAmbientSoundItemNameLabel);
			mAmbientSwipeLeftButton = new Bej3Button(14, this, Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
			mAmbientSwipeLeftButton.Resize(ConstantsWP.ZENOPTIONS_BTN_LEFT_OFFSET, zENOPTIONS_AMBIENT_SOUND_DESC_Y - GlobalMembersResourcesWP.IMAGE_DIALOG_ARROW_SWIPE.mHeight / 2, 0, 0);
			AddWidget(mAmbientSwipeLeftButton);
			mAmbientSoundWidgets.Add(mAmbientSwipeLeftButton);
			mAmbientSwipeRightButton = new Bej3Button(15, this, Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
			mAmbientSwipeRightButton.Resize(ConstantsWP.ZENOPTIONS_BTN_RIGHT_OFFSET, zENOPTIONS_AMBIENT_SOUND_DESC_Y - GlobalMembersResourcesWP.IMAGE_DIALOG_ARROW_SWIPE.mHeight / 2, 0, 0);
			AddWidget(mAmbientSwipeRightButton);
			mAmbientSoundWidgets.Add(mAmbientSwipeRightButton);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.AMBIENT_VOL_OFFSET_Y;
			mAmbientVolumeSlider = new Bej3Slider(10, this);
			mAmbientVolumeSlider.Resize(ConstantsWP.ZENOPTIONS_ITEM_RIGHT_OFFSET - ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, zENOPTIONS_AMBIENT_SOUND_DESC_Y, ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, 0);
			mAmbientVolumeSlider.SetThreshold(ConstantsWP.ZENOPTIONS_SLIDER_HOR_THRESHOLD, 0);
			AddWidget(mAmbientVolumeSlider);
			mAmbientSoundWidgets.Add(mAmbientVolumeSlider);
			mAmbientVolumeLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Volume", 520), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mAmbientVolumeLabel.Resize(ConstantsWP.ZENOPTIONS_ITEM_LEFT_OFFSET, mAmbientVolumeSlider.mY + GlobalMembersResources.FONT_DIALOG.GetAscent(), 0, 0);
			AddWidget(mAmbientVolumeLabel);
			mAmbientSoundWidgets.Add(mAmbientVolumeLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y = ConstantsWP.ZENOPTIONS_MANTRAS_DESC_Y;
			mMantra_DescLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Positive textual affirmations are displayed on screen, helping to focus on beneficial areas for meditation.", 3623), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE);
			mMantra_DescLabel.SetTextBlock(new Rect((GlobalMembers.gApp.mWidth - ConstantsWP.ZENOPTIONS_DESC_WIDTH) / 2, ConstantsWP.ZENOPTIONS_DESC_Y, ConstantsWP.ZENOPTIONS_DESC_WIDTH, ConstantsWP.ZENOPTIONS_MANTRAS_DESC_HEIGHT), true);
			mMantra_DescLabel.SetTextBlockEnabled(true);
			mMantra_DescLabel.SetTextBlockAlignment(0);
			AddWidget(mMantra_DescLabel);
			mMantrasWidgets.Add(mMantra_DescLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.ZENOPTIONS_MANTRAS_SLIDE_OFFSET_Y;
			mMantraSelector = new Bej3SlideSelector(13, this, this, ConstantsWP.ZENOPTIONS_MANTRA_ITEM_SIZE, ConstantsWP.ZENOPTIONS_MANTRA_WIDTH);
			mMantraSelector.Resize(0, zENOPTIONS_AMBIENT_SOUND_DESC_Y, ConstantsWP.ZENOPTIONS_MANTRA_SELECTOR_WIDTH, ConstantsWP.ZENOPTIONS_MANTRA_SELECTOR_HEIGHT + 75);
			mMantraSelector.SetThreshold(ConstantsWP.ZENOPTIONS_SLIDER_HOR_THRESHOLD, 0);
			mMantraSelector.AddItem(11, 784);
			mMantraSelector.AddItem(12, 783);
			mMantraSelector.AddItem(13, 786);
			mMantraSelector.AddItem(14, 788);
			mMantraSelector.AddItem(15, 787);
			mMantraSelector.AddItem(16, 791);
			mMantraSelector.AddItem(17, 793);
			AddWidget(mMantraSelector);
			mMantrasWidgets.Add(mMantraSelector);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.ZENOPTIONS_MANTRA_SELECTOR_HEIGHT + ConstantsWP.ZENOPTIONS_SLIDE_ITEM_LABEL_OFFSET_Y;
			mMantraItemNameLabel = new Label(GlobalMembersResources.FONT_DIALOG, "");
			mMantraItemNameLabel.Resize(ConstantsWP.ZENOPTIONS_CENTER_X, zENOPTIONS_AMBIENT_SOUND_DESC_Y, 0, 0);
			mMantraItemNameLabel.SetLayerColor(0, Bej3Widget.COLOR_DIALOG_WHITE);
			AddWidget(mMantraItemNameLabel);
			mMantrasWidgets.Add(mMantraItemNameLabel);
			mMantraSwipeLeftButton = new Bej3Button(16, this, Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
			mMantraSwipeLeftButton.Resize(ConstantsWP.ZENOPTIONS_BTN_LEFT_OFFSET, zENOPTIONS_AMBIENT_SOUND_DESC_Y - GlobalMembersResourcesWP.IMAGE_DIALOG_ARROW_SWIPE.mHeight / 2, 0, 0);
			AddWidget(mMantraSwipeLeftButton);
			mMantrasWidgets.Add(mMantraSwipeLeftButton);
			mMantraSwipeRightButton = new Bej3Button(17, this, Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
			mMantraSwipeRightButton.Resize(ConstantsWP.ZENOPTIONS_BTN_RIGHT_OFFSET, zENOPTIONS_AMBIENT_SOUND_DESC_Y - GlobalMembersResourcesWP.IMAGE_DIALOG_ARROW_SWIPE.mHeight / 2, 0, 0);
			AddWidget(mMantraSwipeRightButton);
			mMantrasWidgets.Add(mMantraSwipeRightButton);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.MANTRA_SUBLIMINAL_CHECK_OFFSET_Y;
			mMantra_SubliminalLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Subliminal", 521), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mMantra_SubliminalCheckbox = new Bej3Checkbox(11, this);
			mMantra_SubliminalCheckbox.mClippingEnabled = true;
			int num = ConstantsWP.ZENOPTIONS_ITEM_LEFT_OFFSET + GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX.mWidth / 2;
			mMantra_SubliminalCheckbox.Resize(num, zENOPTIONS_AMBIENT_SOUND_DESC_Y, 0, 0);
			mMantra_SubliminalLabel.Resize(num + ConstantsWP.ZENOPTIONS_ENABLE_CHECK_OFFSET_X, mMantra_SubliminalCheckbox.mY + (GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX.mHeight + GlobalMembersResources.FONT_DIALOG.GetAscent()) / 2, 0, 0);
			AddWidget(mMantra_SubliminalCheckbox);
			AddWidget(mMantra_SubliminalLabel);
			mMantrasWidgets.Add(mMantra_SubliminalCheckbox);
			mMantrasWidgets.Add(mMantra_SubliminalLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.MANTRA_SPEED_VIS_SLIDER_OFFSET_Y;
			mMantra_SpeedVisibilitySlider = new Bej3Slider(12, this);
			mMantra_SpeedVisibilitySlider.Resize(ConstantsWP.ZENOPTIONS_ITEM_RIGHT_OFFSET - ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, zENOPTIONS_AMBIENT_SOUND_DESC_Y, ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, 0);
			mMantra_SpeedVisibilitySlider.SetThreshold(ConstantsWP.ZENOPTIONS_SLIDER_HOR_THRESHOLD, 0);
			mMantra_SpeedVisibilitySlider.mGrayOutWhenZero = false;
			AddWidget(mMantra_SpeedVisibilitySlider);
			mMantrasWidgets.Add(mMantra_SpeedVisibilitySlider);
			mMantra_SpeedVisibilityLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Visibility", 523), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mMantra_SpeedVisibilityLabel.Resize(ConstantsWP.ZENOPTIONS_ITEM_LEFT_OFFSET, mMantra_SpeedVisibilitySlider.mY + GlobalMembersResources.FONT_DIALOG.GetAscent(), 0, 0);
			AddWidget(mMantra_SpeedVisibilityLabel);
			mMantrasWidgets.Add(mMantra_SpeedVisibilityLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y = ConstantsWP.ZENOPTIONS_BREATH_MOD_DESC_Y;
			mBreathMod_DescLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Audio and visual feedback help modulate your breathing and create a sense of relaxation.", 3624), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE);
			mBreathMod_DescLabel.SetTextBlock(new Rect((GlobalMembers.gApp.mWidth - ConstantsWP.ZENOPTIONS_DESC_WIDTH) / 2, ConstantsWP.ZENOPTIONS_DESC_Y, ConstantsWP.ZENOPTIONS_DESC_WIDTH, ConstantsWP.ZENOPTIONS_BREATH_MOD_DESC_HEIGHT), true);
			mBreathMod_DescLabel.SetTextBlockEnabled(true);
			mBreathMod_DescLabel.SetTextBlockAlignment(0);
			AddWidget(mBreathMod_DescLabel);
			mBreathModWidgets.Add(mBreathMod_DescLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.ZENOPTIONS_BREATH_MOD_ENABLE_OFFSET_Y;
			int num2 = ConstantsWP.ZENOPTIONS_ITEM_LEFT_OFFSET + GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX.mWidth / 2;
			mBreathModCheckbox = new Bej3Checkbox(0, this);
			mBreathModCheckbox.mClippingEnabled = true;
			mBreathModCheckbox.Resize(num2, zENOPTIONS_AMBIENT_SOUND_DESC_Y, 0, 0);
			AddWidget(mBreathModCheckbox);
			mBreathModWidgets.Add(mBreathModCheckbox);
			mBreathModEnable = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Enable", 3625), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mBreathModEnable.Resize(num2 + ConstantsWP.ZENOPTIONS_ENABLE_CHECK_OFFSET_X, mBreathModCheckbox.mY + (GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX.mHeight + GlobalMembersResources.FONT_DIALOG.GetAscent()) / 2, 0, 0);
			AddWidget(mBreathModEnable);
			mBreathModWidgets.Add(mBreathModEnable);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.ZENOPTIONS_BREATHMOD_VIS_CHECK_OFFSET_Y;
			mBreathMod_VisualIndicatorLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Visual Indicator", 516), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mBreathMod_VisualIndicatorCheckbox = new Bej3Checkbox(6, this);
			mBreathMod_VisualIndicatorCheckbox.mClippingEnabled = true;
			mBreathMod_VisualIndicatorCheckbox.Resize(num2, zENOPTIONS_AMBIENT_SOUND_DESC_Y, 0, 0);
			mBreathMod_VisualIndicatorLabel.Resize(num2 + ConstantsWP.ZENOPTIONS_ENABLE_CHECK_OFFSET_X, mBreathMod_VisualIndicatorCheckbox.mY + (GlobalMembersResourcesWP.IMAGE_DIALOG_CHECKBOX.mHeight + GlobalMembersResources.FONT_DIALOG.GetAscent()) / 2, 0, 0);
			AddWidget(mBreathMod_VisualIndicatorCheckbox);
			AddWidget(mBreathMod_VisualIndicatorLabel);
			mBreathModWidgets.Add(mBreathMod_VisualIndicatorCheckbox);
			mBreathModWidgets.Add(mBreathMod_VisualIndicatorLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.BREATHMOD_SPEED_SLIDER_OFFSET_Y;
			mBreathMod_SpeedSlider = new Bej3Slider(7, this);
			mBreathMod_SpeedSlider.Resize(ConstantsWP.ZENOPTIONS_ITEM_RIGHT_OFFSET - ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, zENOPTIONS_AMBIENT_SOUND_DESC_Y, ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, 0);
			mBreathMod_SpeedSlider.SetThreshold(ConstantsWP.ZENOPTIONS_SLIDER_HOR_THRESHOLD, 0);
			mBreathMod_SpeedSlider.mGrayOutWhenZero = false;
			AddWidget(mBreathMod_SpeedSlider);
			mBreathModWidgets.Add(mBreathMod_SpeedSlider);
			mBreathMod_SpeedLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Speed", 517), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mBreathMod_SpeedLabel.Resize(ConstantsWP.ZENOPTIONS_ITEM_LEFT_OFFSET, mBreathMod_SpeedSlider.mY + GlobalMembersResources.FONT_DIALOG.GetAscent(), 0, 0);
			AddWidget(mBreathMod_SpeedLabel);
			mBreathModWidgets.Add(mBreathMod_SpeedLabel);
			zENOPTIONS_AMBIENT_SOUND_DESC_Y += ConstantsWP.BREATHMOD_VOL_SLIDER_OFFSET_Y;
			mBreathMod_VolumeSlider = new Bej3Slider(8, this);
			mBreathMod_VolumeSlider.Resize(ConstantsWP.ZENOPTIONS_ITEM_RIGHT_OFFSET - ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, zENOPTIONS_AMBIENT_SOUND_DESC_Y, ConstantsWP.ZENOPTIONS_SLIDER_WIDTH, 0);
			mBreathMod_VolumeSlider.SetThreshold(ConstantsWP.ZENOPTIONS_SLIDER_HOR_THRESHOLD, 0);
			AddWidget(mBreathMod_VolumeSlider);
			mBreathModWidgets.Add(mBreathMod_VolumeSlider);
			mBreathMod_VolumeLabel = new Label(GlobalMembersResources.FONT_DIALOG, GlobalMembers._ID("Volume", 518), Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
			mBreathMod_VolumeLabel.Resize(ConstantsWP.ZENOPTIONS_ITEM_LEFT_OFFSET, mBreathMod_VolumeSlider.mY + GlobalMembersResources.FONT_DIALOG.GetAscent(), 0, 0);
			AddWidget(mBreathMod_VolumeLabel);
			mBreathModWidgets.Add(mBreathMod_VolumeLabel);
			mTotalHeight = zENOPTIONS_AMBIENT_SOUND_DESC_Y + ConstantsWP.BREATHMOD_VOL_SLIDER_OFFSET_Y;
			Resize(0, 0, ConstantsWP.ZENOPTIONS_CENTER_X * 2, GlobalMembers.gApp.mHeight);
		}

		public void SliderReleased(int theId, double theVal)
		{
		}

		public override void LinkUpAssets()
		{
			if (GlobalMembers.gApp.mProfile.mAmbientSelection <= 0 || GlobalMembers.gApp.mProfile.mAmbientSelection >= 9)
			{
				GlobalMembers.gApp.mProfile.mAmbientSelection = 1;
			}
			if (GlobalMembers.gApp.mProfile.mMantraSelection <= 10 || GlobalMembers.gApp.mProfile.mMantraSelection >= 18)
			{
				GlobalMembers.gApp.mProfile.mMantraSelection = 11;
			}
			mBreathModCheckbox.SetChecked(GlobalMembers.gApp.mProfile.mBreathOn, false);
			mBreathMod_VisualIndicatorCheckbox.SetChecked(GlobalMembers.gApp.mProfile.mBreathVisual, false);
			mBreathMod_SpeedSlider.SetValue(GlobalMembers.gApp.mProfile.mBreathSpeed);
			mBreathMod_VolumeSlider.SetValue(GlobalMembers.gApp.mZenBreathVolume);
			mAmbientVolumeSlider.SetValue(GlobalMembers.gApp.mZenAmbientVolume);
			mMantra_SubliminalCheckbox.SetChecked(GlobalMembers.gApp.mProfile.mAffirmationSubliminal, false);
			if (GlobalMembers.gApp.mProfile.mAffirmationSubliminal)
			{
				mMantra_SpeedVisibilityLabel.SetText(GlobalMembers._ID("Visibility", 3517));
				mMantra_SpeedVisibilitySlider.SetValue(GlobalMembers.gApp.mProfile.mAffirmationSubliminality);
			}
			else
			{
				mMantra_SpeedVisibilityLabel.SetText(GlobalMembers._ID("Speed", 3518));
				mMantra_SpeedVisibilitySlider.SetValue(GlobalMembers.gApp.mProfile.mAffirmationSpeed);
			}
			mBreathModCheckbox.LinkUpAssets();
			mMantraSelector.SetDisabled(false);
			mMantraSelector.LinkUpAssets();
			mAmbientSoundSelector.SetDisabled(false);
			mAmbientSoundSelector.LinkUpAssets();
			mAmbientSoundSelector.CenterOnItem(GlobalMembers.gApp.mProfile.mAmbientSelection, true);
			mAmbientSoundItemNameLabel.SetText(GetAmbienceName(GlobalMembers.gApp.mProfile.mAmbientSelection));
			mMantraSelector.CenterOnItem(GlobalMembers.gApp.mProfile.mMantraSelection, true);
			mMantraItemNameLabel.SetText(GetMantraName(GlobalMembers.gApp.mProfile.mMantraSelection));
			base.LinkUpAssets();
			GrayOutOptions();
		}

		public void GrayOutOptions()
		{
			switch (mMode)
			{
			case ZenOptionsMode.MODE_AMBIENT_SOUNDS:
			{
				bool flag2 = !GlobalMembers.gApp.mProfile.mNoiseOn;
				mAmbientVolumeSlider.SetDisabled(flag2);
				mAmbientVolumeLabel.mGrayedOut = (mAmbientVolumeSlider.mGrayedOut = flag2 || mAmbientVolumeSlider.mVal == 0.0);
				mAmbientVolumeSlider.LinkUpAssets();
				break;
			}
			case ZenOptionsMode.MODE_MANTRAS:
			{
				bool disabled = !GlobalMembers.gApp.mProfile.mAffirmationOn;
				mMantra_SpeedVisibilitySlider.SetDisabled(disabled);
				mMantra_SubliminalCheckbox.SetDisabled(disabled);
				mMantra_SpeedVisibilityLabel.mGrayedOut = (mMantra_SpeedVisibilitySlider.mGrayedOut = disabled);
				mMantra_SubliminalLabel.mGrayedOut = disabled;
				mMantra_SpeedVisibilitySlider.LinkUpAssets();
				break;
			}
			case ZenOptionsMode.MODE_BREATH_MOD:
			{
				bool flag = !GlobalMembers.gApp.mProfile.mBreathOn;
				mBreathMod_VolumeSlider.SetDisabled(flag);
				mBreathMod_SpeedSlider.SetDisabled(flag);
				mBreathMod_VisualIndicatorCheckbox.SetDisabled(flag);
				mBreathMod_VolumeLabel.mGrayedOut = (mBreathMod_VolumeSlider.mGrayedOut = flag || mBreathMod_VolumeSlider.mVal == 0.0);
				mBreathMod_VisualIndicatorLabel.mGrayedOut = flag;
				mBreathMod_SpeedLabel.mGrayedOut = (mBreathMod_SpeedSlider.mGrayedOut = flag);
				mBreathMod_VolumeSlider.LinkUpAssets();
				mBreathMod_SpeedSlider.LinkUpAssets();
				break;
			}
			}
		}

		public override void Draw(Graphics g)
		{
			if (mMode == ZenOptionsMode.MODE_AMBIENT_SOUNDS)
			{
				Bej3Widget.DrawSwipeInlay(g, mAmbientSoundSelector.mY - ConstantsWP.ZENOPTIONS_INLAY_PAD_Y, mAmbientSoundSelector.mHeight + ConstantsWP.ZENOPTIONS_INLAY_PAD_Y * 3 - 75, mWidth, true);
			}
			else if (mMode == ZenOptionsMode.MODE_MANTRAS)
			{
				Bej3Widget.DrawSwipeInlay(g, mMantraSelector.mY - ConstantsWP.ZENOPTIONS_INLAY_PAD_Y, mAmbientSoundSelector.mHeight + ConstantsWP.ZENOPTIONS_INLAY_PAD_Y * 3 - 75, mWidth, true);
			}
		}

		public override void Update()
		{
			base.Update();
			bool flag = false;
			if (mAmbientSoundId >= 0 && --mAmbientSoundStartDelay <= 0)
			{
				flag = AmbientLoadSound(mAmbientSoundId);
				mAmbientSoundId = -1;
			}
			if (flag && GlobalMembers.gApp.mProfile.mNoiseOn && !GlobalMembers.gApp.IsMuted())
			{
				ZenBoard zenBoard = null;
				if (GlobalMembers.gApp.mBoard != null)
				{
					zenBoard = (ZenBoard)GlobalMembers.gApp.mBoard;
					zenBoard.PlayZenNoise();
				}
			}
		}

		public override void ButtonMouseEnter(int theId)
		{
		}

		public override void ButtonDepress(int theId)
		{
			switch (theId)
			{
			case 14:
			{
				int itemId4 = GlobalMembers.MAX(1, mAmbientSoundSelector.GetSelectedId() - 1);
				mAmbientSoundSelector.CenterOnItem(itemId4, false);
				break;
			}
			case 15:
			{
				int itemId3 = GlobalMembers.MIN(8, mAmbientSoundSelector.GetSelectedId() + 1);
				mAmbientSoundSelector.CenterOnItem(itemId3, false);
				break;
			}
			case 16:
			{
				int itemId2 = GlobalMembers.MAX(11, mMantraSelector.GetSelectedId() - 1);
				mMantraSelector.CenterOnItem(itemId2, false);
				break;
			}
			case 17:
			{
				int itemId = GlobalMembers.MIN(17, mMantraSelector.GetSelectedId() + 1);
				mMantraSelector.CenterOnItem(itemId, false);
				break;
			}
			}
		}

		public virtual void SliderVal(int theId, double theVal)
		{
			switch (theId)
			{
			case 7:
				GlobalMembers.gApp.mProfile.mBreathSpeed = (float)theVal;
				break;
			case 10:
				GlobalMembers.gApp.mZenAmbientVolume = (float)theVal;
				if (!GlobalMembers.gApp.IsMuted())
				{
					ZenBoard zenBoard = GlobalMembers.gApp.mBoard as ZenBoard;
					if (zenBoard != null && zenBoard.mNoiseSoundInstance != null)
					{
						zenBoard.mNoiseSoundInstance.SetVolume(GlobalMembers.gApp.mZenAmbientVolume);
					}
					GlobalMembers.gApp.mSoundManager.SetVolume(2, GlobalMembers.gApp.mZenAmbientVolume);
				}
				break;
			case 8:
				GlobalMembers.gApp.mZenBreathVolume = (float)theVal;
				if (!GlobalMembers.gApp.IsMuted())
				{
					(GlobalMembers.gApp.mBoard as ZenBoard)?.mBreathSoundInstance.SetVolume(GlobalMembers.gApp.mZenBreathVolume);
					GlobalMembers.gApp.mSoundManager.SetVolume(4, GlobalMembers.gApp.mZenBreathVolume);
				}
				break;
			case 12:
				if (mMantra_SubliminalCheckbox.IsChecked())
				{
					GlobalMembers.gApp.mProfile.mAffirmationSubliminality = (float)theVal;
				}
				else
				{
					GlobalMembers.gApp.mProfile.mAffirmationSpeed = (float)theVal;
				}
				break;
			}
			GrayOutOptions();
		}

		public void CheckboxChecked(int theId, bool checked1)
		{
			switch (theId)
			{
			case 0:
				GlobalMembers.gApp.mProfile.mBreathOn = checked1;
				break;
			case 6:
				GlobalMembers.gApp.mProfile.mBreathVisual = checked1;
				break;
			case 11:
				GlobalMembers.gApp.mProfile.mAffirmationSubliminal = checked1;
				if (checked1 && GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN && GlobalMembers.gApp.mBoard != null)
				{
					((ZenBoard)GlobalMembers.gApp.mBoard).mAffirmationPct = 0.751f;
				}
				if (checked1)
				{
					mMantra_SpeedVisibilityLabel.SetText(GlobalMembers._ID("Visibility", 3519));
					mMantra_SpeedVisibilitySlider.SetValue(GlobalMembers.gApp.mProfile.mAffirmationSubliminality);
				}
				else
				{
					mMantra_SpeedVisibilityLabel.SetText(GlobalMembers._ID("Speed", 3520));
					mMantra_SpeedVisibilitySlider.SetValue(GlobalMembers.gApp.mProfile.mAffirmationSpeed);
				}
				break;
			}
			GrayOutOptions();
		}

		public virtual void SlideSelectorChanged(int theSlideSelectorId, int theItemId)
		{
			switch (theSlideSelectorId)
			{
			case 9:
				AmbientSoundChanged(theItemId);
				break;
			case 13:
				MantraSelected(theItemId);
				break;
			}
		}

		public void AmbientSoundChanged(int theId)
		{
			bool mNoiseOn = GlobalMembers.gApp.mProfile.mNoiseOn;
			GlobalMembers.gApp.mProfile.mNoiseOn = theId != 1;
			if (mNoiseOn != GlobalMembers.gApp.mProfile.mNoiseOn)
			{
				GrayOutOptions();
			}
			bool flag = true;
			if (GlobalMembers.gApp.mProfile.mAmbientSelection == theId)
			{
				flag = false;
			}
			ZenBoard zenBoard = null;
			if (GlobalMembers.gApp.mBoard != null)
			{
				zenBoard = GlobalMembers.gApp.mBoard as ZenBoard;
			}
			if (flag && zenBoard != null)
			{
				zenBoard.StopZenNoise();
				mAmbientSoundStartDelay = ConstantsWP.ZENOPTIONS_AMBIENT_SOUND_START_DELAY_FRAMES;
				mAmbientSoundId = theId;
			}
			bool flag2 = false;
			if (flag)
			{
				if (mAmbientSoundStartDelay > 0)
				{
					mAmbientSoundItemNameLabel.SetText(GetAmbienceName(theId));
				}
				else
				{
					flag2 = AmbientLoadSound(theId);
				}
			}
			if (flag2 && GlobalMembers.gApp.mProfile.mNoiseOn && !GlobalMembers.gApp.IsMuted())
			{
				zenBoard.PlayZenNoise();
			}
		}

		public bool AmbientLoadSound(int theId)
		{
			GlobalMembers.gApp.mProfile.mAmbientSelection = theId;
			mAmbientSoundItemNameLabel.SetText(GetAmbienceName(theId));
			switch (theId)
			{
			case 1:
				GlobalMembers.gApp.mProfile.mNoiseFileName = string.Empty;
				break;
			case 2:
				GlobalMembers.gApp.mProfile.mNoiseFileName = "*";
				break;
			case 3:
				GlobalMembers.gApp.mProfile.mNoiseFileName = "Ocean Surf.caf";
				break;
			case 4:
				GlobalMembers.gApp.mProfile.mNoiseFileName = "Crickets.caf";
				break;
			case 5:
				GlobalMembers.gApp.mProfile.mNoiseFileName = "Rain Leaves.caf";
				break;
			case 6:
				GlobalMembers.gApp.mProfile.mNoiseFileName = "Waterfall.caf";
				break;
			case 7:
				GlobalMembers.gApp.mProfile.mNoiseFileName = "Coastal.caf";
				break;
			case 8:
				GlobalMembers.gApp.mProfile.mNoiseFileName = "Forest.caf";
				break;
			}
			ZenBoard zenBoard = null;
			if (GlobalMembers.gApp.mBoard != null)
			{
				zenBoard = (ZenBoard)GlobalMembers.gApp.mBoard;
			}
			zenBoard?.LoadAmbientSound();
			if (zenBoard != null)
			{
				return zenBoard.mNoiseSoundInstance != null;
			}
			return false;
		}

		public void MantraSelected(int theId)
		{
			bool mAffirmationOn = GlobalMembers.gApp.mProfile.mAffirmationOn;
			GlobalMembers.gApp.mProfile.mAffirmationOn = theId != 11;
			if (mAffirmationOn != GlobalMembers.gApp.mProfile.mAffirmationOn)
			{
				GrayOutOptions();
			}
			bool flag = GlobalMembers.gApp.mProfile.mMantraSelection != theId;
			GlobalMembers.gApp.mProfile.mMantraSelection = theId;
			mMantraItemNameLabel.SetText(GetMantraName(theId));
			switch (theId)
			{
			case 12:
				GlobalMembers.gApp.mProfile.mAffirmationFileName = "General.txt";
				break;
			case 13:
				GlobalMembers.gApp.mProfile.mAffirmationFileName = "Positive Thinking.txt";
				break;
			case 14:
				GlobalMembers.gApp.mProfile.mAffirmationFileName = "Quit Bad Habits.txt";
				break;
			case 15:
				GlobalMembers.gApp.mProfile.mAffirmationFileName = "Prosperity.txt";
				break;
			case 16:
				GlobalMembers.gApp.mProfile.mAffirmationFileName = "Self Confidence.txt";
				break;
			case 17:
				GlobalMembers.gApp.mProfile.mAffirmationFileName = "Weight Loss.txt";
				break;
			}
			ZenBoard zenBoard = GlobalMembers.gApp.mBoard as ZenBoard;
			if (zenBoard != null && flag)
			{
				zenBoard.LoadAffirmations();
			}
		}

		public string GetAmbienceName(int theId)
		{
			return ambientSounds[theId - 1];
		}

		public string GetMantraName(int theId)
		{
			return mantras[theId - 10 - 1];
		}

		public override void Show()
		{
			mAlphaCurve.SetConstant(1.0);
			base.Show();
			mY = mTargetPos;
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
		}

		public override void Hide()
		{
			mAlphaCurve.SetConstant(1.0);
			base.Hide();
		}

		public void SetMode(ZenOptionsMode mode)
		{
			mMode = mode;
			foreach (SexyFramework.Widget.Widget mAmbientSoundWidget in mAmbientSoundWidgets)
			{
				Bej3Widget.DisableWidget(mAmbientSoundWidget, mode != ZenOptionsMode.MODE_AMBIENT_SOUNDS);
			}
			foreach (SexyFramework.Widget.Widget mMantrasWidget in mMantrasWidgets)
			{
				Bej3Widget.DisableWidget(mMantrasWidget, mode != ZenOptionsMode.MODE_MANTRAS);
			}
			foreach (SexyFramework.Widget.Widget mBreathModWidget in mBreathModWidgets)
			{
				Bej3Widget.DisableWidget(mBreathModWidget, mode != ZenOptionsMode.MODE_BREATH_MOD);
			}
			GrayOutOptions();
		}

		public override void PlayMenuMusic()
		{
		}
	}
}
