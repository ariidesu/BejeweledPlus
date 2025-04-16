using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus.Widget
{
	public class Bej3Dialog : Dialog, Bej3ButtonListener, ButtonListener
	{
		public new enum ButtonID
		{
			ID_CANCEL = 1002
		}

		protected List<DialogButton> mButtons;

		protected Label mHeadingLabel;

		protected Label mMessageLabel;

		protected int mTargetPos;

		protected float mAnimationFraction;

		protected bool mFinishedTransition;

		protected bool mAllowSlide;

		protected CurvedVal mAlphaCurve = new CurvedVal();

		public bool mCanSlideInMenus;

		public Bej3Button mTopButton;

		public int mFlushPriority;

		public bool mIsKilling;

		public bool mCanEscape;

		public bool mAllowDrag;

		public int mScaleCenterX;

		public int mScaleCenterY;

		public LinkedList<SexyFramework.Widget.Widget> mMouseInvisibleChildren;

		public bool mOpenURLWhenDone;

		protected bool FinishedSlide()
		{
			return Bej3Widget.FloatEquals(mY, mTargetPos, ConstantsWP.DASHBOARD_SLIDER_SPEED);
		}

		protected virtual void SlideInFinished()
		{
		}

		protected virtual void KilledFinished()
		{
			if (mOpenURLWhenDone)
			{
				GlobalMembers.gApp.OpenLastConfirmedWebsite();
			}
		}

		public Bej3Dialog(int theId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode, Bej3ButtonType buttonType1, Bej3ButtonType buttonType2, Bej3ButtonType topButtonType)
			: base(null, null, theId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode)
		{
			mHeadingLabel = null;
			mMessageLabel = null;
			mButtons = new List<DialogButton>();
			mMouseInvisibleChildren = new LinkedList<SexyFramework.Widget.Widget>();
			mCanSlideInMenus = true;
			mAnimationFraction = 0f;
			mOpenURLWhenDone = false;
			mAlphaCurve.SetConstant(1.0);
			mZOrder = 3;
			mFlushPriority = -1;
			mClip = false;
			mIsKilling = false;
			mCanEscape = false;
			mAllowDrag = true;
			mScaleCenterX = 0;
			mScaleCenterY = 0;
			SetColor(0, new Color(255, 255, 255));
			SetColor(1, new Color(0, 0, 0));
			mSpaceAfterHeader = GlobalMembers.MS(45);
			mContentInsets = new Insets(GlobalMembers.MS(90), GlobalMembers.MS(22), GlobalMembers.MS(90), GlobalMembers.MS(45));
			mButtonHorzSpacing = GlobalMembers.MS(10);
			mButtonSidePadding = GlobalMembers.MS(25);
			mLineSpacingOffset = GlobalMembers.MS(-6);
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.SetText(theDialogHeader);
			AddWidget(mHeadingLabel);
			mMessageLabel = new Label(GlobalMembersResources.FONT_DIALOG);
			mMessageLabel.SetText(theDialogLines);
			mMessageLabel.SetTextBlockEnabled(true);
			AddWidget(mMessageLabel);
			int num = 1000;
			string text = "";
			if (mYesButton != null)
			{
				int theId2 = mYesButton.mId;
				text = mYesButton.mLabel;
				Bej3Button bej3Button = new Bej3Button(theId2, this, buttonType1);
				bej3Button.SetLabel(text);
				GlobalMembers.KILL_WIDGET(mYesButton);
				mYesButton = bej3Button;
				mButtons.Add(mYesButton);
				AddWidget(mYesButton);
			}
			text = "";
			if (mNoButton != null)
			{
				num = mNoButton.mId;
				text = mNoButton.mLabel;
				Bej3Button bej3Button2 = new Bej3Button(num, this, buttonType2);
				bej3Button2.SetLabel(text);
				GlobalMembers.KILL_WIDGET(mNoButton);
				mNoButton = bej3Button2;
				mButtons.Add(mNoButton);
				AddWidget(mNoButton);
			}
			int theId3 = 1002;
			if (mYesButton != null)
			{
				theId3 = mYesButton.mId;
			}
			if (mNoButton != null)
			{
				theId3 = mNoButton.mId;
			}
			if (topButtonType != Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
			{
				mTopButton = new Bej3Button(theId3, this, topButtonType);
				int theX = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(711));
				int theY = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(711));
				int celWidth = GlobalMembersResourcesWP.IMAGE_DASHBOARD_MENU_UP.GetCelWidth();
				int celHeight = GlobalMembersResourcesWP.IMAGE_DASHBOARD_MENU_UP.GetCelHeight();
				mTopButton.Resize(theX, theY, celWidth, celHeight);
				AddWidget(mTopButton);
			}
			else
			{
				mTopButton = null;
			}
			SetTopButtonType(topButtonType);
			LinkUpAssets();
			bool flag = false;
			if (mCanSlideInMenus)
			{
				Bej3ButtonType previousButtonType = Bej3ButtonType.TOP_BUTTON_TYPE_NONE;
				if (mTopButton != null)
				{
					previousButtonType = mTopButton.GetType();
				}
				flag = Bej3Widget.SlideCurrent(true, this, previousButtonType);
			}
			Bej3Button previousTopButton = null;
			if (Bej3Widget.mCurrentSlidingMenu != null)
			{
				previousTopButton = Bej3Widget.mCurrentSlidingMenu.mTopButton;
			}
			AllowSlideIn(!flag, previousTopButton);
			base.SystemButtonPressed += OnSystemButtonPressed;
			GlobalMembers.gApp.ClearUpdateBacklog(false);
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			Bej3Widget.ClearSlide(this);
			base.Dispose();
		}

		public virtual void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back)
			{
				args.processed = true;
				ButtonDepress(1002);
			}
		}

		public override void SetButtonFont(Font theFont)
		{
			base.SetHeaderFont(theFont);
			mHeadingLabel.SetFont(theFont);
		}

		public override void SetHeaderFont(Font theFont)
		{
			base.SetHeaderFont(theFont);
			mHeadingLabel.SetFont(theFont);
		}

		public override void SetLinesFont(Font theFont)
		{
			base.SetLinesFont(theFont);
			mMessageLabel.SetFont(theFont);
		}

		public override void Draw(Graphics g)
		{
			new Rect(mBackgroundInsets.mLeft, mBackgroundInsets.mTop, mWidth - mBackgroundInsets.mLeft - mBackgroundInsets.mRight, mHeight - mBackgroundInsets.mTop - mBackgroundInsets.mBottom);
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.MENU_DIVIDER_DIALOG_Y, false);
		}

		public void PreDraw(Graphics g)
		{
			mWidgetManager.FlushDeferredOverlayWidgets(mFlushPriority);
			g.SetColor(new Color(255, 255, 255, (int)(255.0 * (double)mAlphaCurve)));
			g.PushColorMult();
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			mWidgetManager.FlushDeferredOverlayWidgets(10);
			PreDraw(g);
			base.DrawAll(theFlags, g);
			PostDraw(g);
		}

		public void PostDraw(Graphics g)
		{
			g.PopColorMult();
		}

		public override void Update()
		{
			base.Update();
			mAlphaCurve.IncInVal();
			bool flag = Bej3Widget.FloatEquals((float)mY + mAnimationFraction, mTargetPos, ConstantsWP.DASHBOARD_SLIDER_SPEED);
			if (!flag && mAllowSlide)
			{
				int num = ((mY < mTargetPos) ? 1 : (-1));
				mAnimationFraction += (float)mY + ((float)(mTargetPos - mY) - mAnimationFraction) * ConstantsWP.DASHBOARD_SLIDER_SPEED_SCALAR + ConstantsWP.DASHBOARD_SLIDER_SPEED * (float)num;
				mY = (int)mAnimationFraction;
				mAnimationFraction -= mY;
			}
			if (mAllowSlide && Bej3Widget.FloatEquals((float)mY + mAnimationFraction, mTargetPos, ConstantsWP.DASHBOARD_SLIDER_SPEED))
			{
				mY = mTargetPos;
				mAnimationFraction = 0f;
			}
			if (!mAlphaCurve.HasBeenTriggered())
			{
				flag = false;
			}
			if (mFinishedTransition)
			{
				return;
			}
			Bej3ButtonType previousButtonType = Bej3ButtonType.TOP_BUTTON_TYPE_NONE;
			if (mTopButton != null)
			{
				previousButtonType = mTopButton.GetType();
			}
			if (!flag)
			{
				return;
			}
			if (mIsKilling)
			{
				Bej3Widget.NotifyCurrentDialogFinished(this, previousButtonType);
				if (mCanSlideInMenus)
				{
					Bej3Widget.SlideCurrent(false, this, previousButtonType);
				}
				if (GlobalMembers.gApp.mBoard != null)
				{
					GlobalMembers.gApp.mBoard.DialogClosed(mId);
				}
				KilledFinished();
				GlobalMembers.gApp.KillDialog(this);
			}
			if (!mIsKilling)
			{
				SlideInFinished();
			}
			Bej3Widget.NotifyCurrentDialogFinishedSlidingIn(this, previousButtonType);
			mFinishedTransition = true;
		}

		public bool find(LinkedList<SexyFramework.Widget.Widget> list, SexyFramework.Widget.Widget emu)
		{
			LinkedList<SexyFramework.Widget.Widget>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == emu)
				{
					return false;
				}
			}
			return true;
		}

		public void SetChildrenMouseVisible(bool isVisible)
		{
			while (true)
			{
				SexyFramework.Widget.Widget widget = this;
				bool flag;
				do
				{
					LinkedList<SexyFramework.Widget.Widget>.Enumerator enumerator = widget.mWidgets.GetEnumerator();
					flag = false;
					while (enumerator.MoveNext())
					{
						SexyFramework.Widget.Widget current = enumerator.Current;
						if (current.mMouseVisible != isVisible && find(mMouseInvisibleChildren, current))
						{
							widget = current;
							flag = true;
							break;
						}
					}
				}
				while (flag);
				if (widget == this)
				{
					break;
				}
				widget.mMouseVisible = isVisible;
			}
		}

		public override int GetPreferredHeight(int theWidth)
		{
			theWidth = Math.Max(ConstantsWP.DIALOGBOX_MIN_WIDTH, theWidth);
			LinkUpAssets();
			int visibleHeight = mMessageLabel.GetVisibleHeight(theWidth - ConstantsWP.DIALOGBOX_MESSAGE_PADDING_X * 2);
			visibleHeight += mMessageLabel.mY;
			visibleHeight += ConstantsWP.DIALOGBOX_EXTRA_HEIGHT;
			int num = Common.size(mButtons);
			visibleHeight += num * ConstantsWP.DIALOGBOX_BUTTON_MEASURE_HEIGHT;
			return Math.Max(visibleHeight, ConstantsWP.DIALOGBOX_MIN_HEIGHT);
		}

		public virtual void Kill()
		{
			if (!mIsKilling)
			{
				mIsKilling = true;
				mTargetPos = ConstantsWP.MENU_Y_POS_HIDDEN;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBEJ3_WIDGET_HIDE_CURVE, mAlphaCurve);
				mFinishedTransition = false;
				bool mDoFadeBackForDialogs = false;
				if (Bej3Widget.mCurrentSlidingMenu != null && Bej3Widget.mCurrentSlidingMenu.mShouldFadeBehind)
				{
					mDoFadeBackForDialogs = true;
				}
				GlobalMembers.gApp.mDoFadeBackForDialogs = mDoFadeBackForDialogs;
			}
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
			mDragging = false;
		}

		public override void ButtonPress(int theId)
		{
			if (!mIsKilling)
			{
				base.ButtonPress(theId);
			}
		}

		public override void ButtonDepress(int theId)
		{
			if (mResult == int.MaxValue)
			{
				if (!mIsKilling)
				{
					base.ButtonDepress(theId);
				}
				if (theId == 1002)
				{
					mResult = theId;
					mDialogListener.DialogButtonDepress(mId, theId);
				}
			}
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			theWidth = Math.Max(ConstantsWP.DIALOGBOX_MIN_WIDTH, theWidth);
			theHeight = Math.Max(ConstantsWP.DIALOGBOX_MIN_HEIGHT, theHeight);
			theHeight = Math.Min(ConstantsWP.DIALOGBOX_MAX_HEIGHT, theHeight);
			theWidth = GlobalMembers.gApp.mWidth;
			theX = GlobalMembers.gApp.mWidth / 2 - theWidth / 2;
			mTargetPos = GlobalMembers.gApp.mHeight - theHeight;
			theY = ConstantsWP.MENU_Y_POS_HIDDEN;
			mFinishedTransition = false;
			superSubResize(theX, theY, theWidth, theHeight);
			mHeadingLabel.SetMaximumWidth(0);
			mHeadingLabel.Resize(mWidth / 2, ConstantsWP.DIALOGBOX_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetMaximumWidth(mWidth - ConstantsWP.DIALOGBOX_HEADING_LABEL_MAX_WIDTH_OFFSET, ConstantsWP.DIALOGBOX_HEADING_LABEL_SPLIT_Y);
			LinkUpAssets();
		}

		public virtual void LinkUpAssets()
		{
			mComponentImage = null;
			switch (Common.size(mButtons))
			{
			case 1:
				mButtons[0].Resize(mWidth / 2 - ConstantsWP.DIALOGBOX_BUTTON_WIDTH_1_BUTTON / 2, mHeight - mButtons[0].mHeight - ConstantsWP.DIALOGBOX_BUTTON_1_Y_1_BUTTON, ConstantsWP.DIALOGBOX_BUTTON_WIDTH_1_BUTTON, 0);
				break;
			case 2:
				mButtons[1].Resize(mWidth / 2 - ConstantsWP.DIALOGBOX_BUTTON_WIDTH_2_BUTTONS / 2, mHeight - mButtons[0].mHeight - ConstantsWP.DIALOGBOX_BUTTON_1_Y_2_BUTTONS, ConstantsWP.DIALOGBOX_BUTTON_WIDTH_2_BUTTONS, 0);
				mButtons[0].Resize(mWidth / 2 - ConstantsWP.DIALOGBOX_BUTTON_WIDTH_2_BUTTONS / 2, mHeight - mButtons[1].mHeight - ConstantsWP.DIALOGBOX_BUTTON_2_Y_2_BUTTONS, ConstantsWP.DIALOGBOX_BUTTON_WIDTH_2_BUTTONS, 0);
				break;
			case 3:
				mButtons[2].Resize(mWidth / 2 - ConstantsWP.DIALOGBOX_BUTTON_WIDTH_3_BUTTONS / 2, mHeight - mButtons[0].mHeight - ConstantsWP.DIALOGBOX_BUTTON_1_Y_3_BUTTONS, ConstantsWP.DIALOGBOX_BUTTON_WIDTH_3_BUTTONS, 0);
				mButtons[1].Resize(mWidth / 2 - ConstantsWP.DIALOGBOX_BUTTON_WIDTH_3_BUTTONS / 2, mHeight - mButtons[1].mHeight - ConstantsWP.DIALOGBOX_BUTTON_2_Y_3_BUTTONS, ConstantsWP.DIALOGBOX_BUTTON_WIDTH_3_BUTTONS, 0);
				mButtons[0].Resize(mWidth / 2 - mButtons[2].mWidth / 2, mHeight - mButtons[2].mHeight - ConstantsWP.DIALOGBOX_BUTTON_3_Y_3_BUTTONS, ConstantsWP.DIALOGBOX_BUTTON_WIDTH_3_BUTTONS, 0);
				break;
			}
			if (mMessageLabel != null)
			{
				mMessageLabel.SetTextBlock(new Rect(ConstantsWP.DIALOGBOX_MESSAGE_PADDING_X, ConstantsWP.DIALOGBOX_MESSAGE_PADDING_TOP, mWidth - ConstantsWP.DIALOGBOX_MESSAGE_PADDING_X * 2, mHeight - ConstantsWP.DIALOGBOX_MESSAGE_PADDING_TOP), false);
			}
			for (int i = 0; i < Common.size(mButtons); i++)
			{
				Bej3Button bej3Button = (Bej3Button)mButtons[i];
				bej3Button.mClippingEnabled = false;
			}
		}

		public void AddButton(DialogButton theButton)
		{
			mButtons.Add(theButton);
			AddWidget(theButton);
			LinkUpAssets();
		}

		public void SetButtonPosition(DialogButton theButton, int thePosition)
		{
			for (int i = 0; i < Common.size(mButtons); i++)
			{
				RemoveWidget(mButtons[i]);
			}
			for (int j = 0; j < Common.size(mButtons); j++)
			{
				Bej3Button bej3Button = (Bej3Button)mButtons[j];
				if (theButton == bej3Button)
				{
					mButtons.RemoveAt(j);
					mButtons.Insert(thePosition, bej3Button);
					break;
				}
			}
			for (int k = 0; k < Common.size(mButtons); k++)
			{
				AddWidget(mButtons[k]);
			}
			LinkUpAssets();
		}

		public void SizeToContent()
		{
			int preferredHeight = GetPreferredHeight(mWidth);
			if (preferredHeight > GlobalMembers.gApp.mHeight)
			{
				preferredHeight = GlobalMembers.gApp.mHeight;
				mY = 0;
			}
			else
			{
				mY += (mHeight - preferredHeight) / 2;
			}
			Resize(mX, mY, mWidth, preferredHeight);
		}

		public override void GotFocus()
		{
			base.GotFocus();
		}

		public virtual void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			bool flag = mAllowSlide;
			mAllowSlide = allow;
			if (!flag)
			{
			}
			bool visible = allow;
			SetVisible(visible);
			if (mAllowSlide)
			{
				if (!flag)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBEJ3_WIDGET_SHOW_CURVE, mAlphaCurve);
				}
			}
			else
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBEJ3_WIDGET_HIDE_CURVE, mAlphaCurve);
			}
			bool mDoFadeBackForDialogs = allow;
			if (Bej3Widget.mCurrentSlidingMenu != null && Bej3Widget.mCurrentSlidingMenu.mShouldFadeBehind)
			{
				mDoFadeBackForDialogs = true;
			}
			GlobalMembers.gApp.mDoFadeBackForDialogs = mDoFadeBackForDialogs;
		}

		public void SetTopButtonType(Bej3ButtonType type)
		{
			if (mTopButton != null)
			{
				mTopButton.SetType(type);
				mTopButton.SetDisabled(false);
			}
		}

		public int GetTargetPosition()
		{
			return mTargetPos;
		}

		public Label GetMessageLabel()
		{
			return mMessageLabel;
		}

		public virtual bool ButtonsEnabled()
		{
			return Bej3Widget.FloatEquals(mY, mTargetPos);
		}

		public void ForceSetToTarget()
		{
			mY = mTargetPos;
		}

		public void SetMessageFont(Font theFont)
		{
			mMessageLabel.SetFont(theFont);
			SizeToContent();
		}
	}
}
