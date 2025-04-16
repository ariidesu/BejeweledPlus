using System;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	internal class HintDialog : Bej3Dialog, CheckboxListener
	{
		private enum HINTDIALOG_IDS
		{
			CHK_DISABLE_ID
		}

		public Bej3Checkbox mNoHintsCheckbox;

		public bool mWantDisableBox;

		public TextContainer mTextContainer;

		public ScrollWidget mScrollWidget;

		public HintDialog(string theHeader, string theText, bool allowReplay, bool disableBox, Piece tutorialPiece, Board theBoard)
			: base(18, true, theHeader, string.Empty, string.Empty, allowReplay ? 2 : 3, Bej3ButtonType.BUTTON_TYPE_CUSTOM, Bej3ButtonType.BUTTON_TYPE_CUSTOM, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			mTextContainer = null;
			mYesButton.mBtnNoDraw = true;
			mYesButton.Resize(0, 0, 0, 0);
			if (mTopButton != null)
			{
				mTopButton.mId = mYesButton.mId;
			}
			mHeadingLabel.SetVisible(false);
			Resize(0, 0, 0, 0);
			mWantDisableBox = disableBox;
			if (mWantDisableBox)
			{
				mNoHintsCheckbox = new Bej3Checkbox(0, this);
				mNoHintsCheckbox.mChecked = false;
			}
			else
			{
				mNoHintsCheckbox = null;
			}
			int num;
			if (allowReplay)
			{
				num = ConstantsWP.HINTDIALOG_TEXT_WIDTH_REPLAY;
				mNoButton.mLabel = GlobalMembers._ID("REPLAY", 266);
				mNoButton.SetVisible(true);
				((Bej3Button)mNoButton).SetType(Bej3ButtonType.BUTTON_TYPE_HINT_CAMERA);
				((Bej3Button)mNoButton).Resize(0, 0, 0, 0);
				Bej3Widget.CenterWidgetAt(ConstantsWP.HINTDIALOG_TEXT_X + num + (ConstantsWP.HINTDIALOG_WIDTH - (ConstantsWP.HINTDIALOG_TEXT_X + num)) / 2, ConstantsWP.HINTDIALOG_BUTTON_Y, mNoButton);
			}
			else
			{
				num = ConstantsWP.HINTDIALOG_TEXT_WIDTH_NO_REPLAY;
			}
			mTextContainer = new TextContainer(theText, num);
			if (mTextContainer.mText.GetTextBlock().mHeight <= ConstantsWP.HINTDIALOG_TEXT_NO_SCROLL_HEIGHT)
			{
				mScrollWidget = null;
				mTextContainer.mX = ConstantsWP.HINTDIALOG_TEXT_X;
				mTextContainer.mY = ConstantsWP.HINTDIALOG_TEXTCONTAINER_Y - mTextContainer.mHeight / 2;
				AddWidget(mTextContainer);
			}
			else
			{
				mScrollWidget = new ScrollWidget(mTextContainer);
				mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_VERTICAL);
				mScrollWidget.EnableBounce(true);
				mScrollWidget.Resize(ConstantsWP.HINTDIALOG_TEXT_X, ConstantsWP.HINTDIALOG_TEXT_Y, num, ConstantsWP.HINTDIALOG_TEXT_HEIGHT);
				mScrollWidget.AddWidget(mTextContainer);
				AddWidget(mScrollWidget);
			}
			Resize(ConstantsWP.HINTDIALOG_X, ConstantsWP.HINTDIALOG_Y, ConstantsWP.HINTDIALOG_WIDTH, ConstantsWP.HINTDIALOG_HEIGHT);
			BringToFront(mNoButton);
			mDialogListener = GlobalMembers.gApp.mBoard;
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		public override void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back)
			{
				args.processed = true;
				ButtonDepress(10001);
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			if (mNoHintsCheckbox != null)
			{
				mNoHintsCheckbox.Dispose();
				mNoHintsCheckbox = null;
			}
			base.Dispose();
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth, 0f, false);
			if (mScrollWidget != null)
			{
				Rect rect = mScrollWidget.GetRect();
				Bej3Widget.DrawSwipeInlay(g, rect.mY, rect.mHeight, mWidth, false);
			}
		}

		public override void Update()
		{
			GlobalMembers.gApp.mDoFadeBackForDialogs = false;
			base.Update();
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			mTargetPos = theY;
			theY = GlobalMembers.gApp.mHeight;
			superSubResize(theX, theY, theWidth, theHeight);
			mY = ConstantsWP.MENU_Y_POS_HIDDEN;
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			base.AllowSlideIn(allow, previousTopButton);
			bool mDoFadeBackForDialogs = false;
			if (Bej3Widget.mCurrentSlidingMenu != null && Bej3Widget.mCurrentSlidingMenu.mShouldFadeBehind)
			{
				mDoFadeBackForDialogs = true;
			}
			GlobalMembers.gApp.mDoFadeBackForDialogs = mDoFadeBackForDialogs;
		}

		public void CheckboxChecked(int theId, bool isChecked)
		{
			throw new NotImplementedException();
		}
	}
}
