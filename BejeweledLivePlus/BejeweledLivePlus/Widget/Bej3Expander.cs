using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3Expander : Bej3WidgetBase, Bej3ButtonListener, ButtonListener, CheckboxListener
	{
		private enum BEJ3EXPANDER_IDS
		{
			BTN_EXPAND_ID,
			BTN_INFO_ID,
			CHK_ENABLE_ID
		}

		private CheckboxListener mCheckListener;

		private Checkbox mCheckbox;

		private Bej3Button mBej3Button;

		private int mExpandedHeight;

		private int mTargetHeight;

		private int mTargetY;

		private float mCurrentY;

		private float mCurrentHeight;

		private bool mHasDivider;

		private float mTargetChildrenAlpha;

		private Bej3ExpanderListener mListener;

		private Bej3Button mExpandButton;

		private Bej3Button mInfoButton;

		private Label mHeadingLabel;

		private List<SexyFramework.Widget.Widget> mHeadingWidgets;

		private List<SexyFramework.Widget.Widget> mContainedWidgets;

		private string mInfoHeader;

		private string mInfoMessage;

		private bool mWidgetsAdded;

		public int mDividerOffset;

		public int mId;

		public int mMinHeight;

		public Bej3Checkbox mEnabledCheckbox;

		public float mChildrenAlpha;

		private void DisableChildren(bool disable)
		{
			if (!mEnabledCheckbox.IsChecked() || mTargetHeight == mMinHeight)
			{
				disable = true;
			}
			for (int i = 0; i < mContainedWidgets.Count; i++)
			{
				SexyFramework.Widget.Widget widget = mContainedWidgets[i];
				widget.SetDisabled(disable);
				widget.mMouseVisible = !disable;
				Bej3WidgetBase bej3WidgetBase = (Bej3WidgetBase)widget;
				if (bej3WidgetBase != null)
				{
					bej3WidgetBase.mGrayedOut = disable;
				}
			}
		}

		public Bej3Expander(int theId, Bej3ExpanderListener theListener, CheckboxListener theCheckListener, string theHeading, bool hasDivider, string theInfoHeader, string theInfoMessage)
		{
			mListener = theListener;
			mCheckListener = theCheckListener;
			mTargetY = 0;
			mHasDivider = hasDivider;
			mId = theId;
			mChildrenAlpha = 1f;
			mTargetHeight = 1;
			mHeadingWidgets = new List<SexyFramework.Widget.Widget>();
			mContainedWidgets = new List<SexyFramework.Widget.Widget>();
			mInfoHeader = theInfoHeader;
			mInfoMessage = theInfoMessage;
			mDividerOffset = ConstantsWP.EXPANDER_DIVIDER_DRAW_OFFSET;
			mWidgetsAdded = false;
			mWidgetFlagsMod.mRemoveFlags |= 8;
			if (mHasDivider)
			{
				mExpandedHeight = ConstantsWP.EXPANDER_MIN_HEIGHT + ConstantsWP.EXPANDER_DIVIDER_OFFSET;
				mTargetHeight = (mMinHeight = ConstantsWP.EXPANDER_MIN_HEIGHT + ConstantsWP.EXPANDER_DIVIDER_OFFSET);
			}
			else
			{
				mExpandedHeight += ConstantsWP.EXPANDER_MIN_HEIGHT;
				mTargetHeight = (mMinHeight = ConstantsWP.EXPANDER_MIN_HEIGHT);
			}
			mHeadingLabel = new Label(GlobalMembersResources.FONT_DIALOG, theHeading, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_CENTRE, Label_Alignment_Vertical.LABEL_ALIGNMENT_VERTICAL_CENTRE);
			mHeadingWidgets.Add(mHeadingLabel);
			AddWidget(mHeadingLabel);
			mInfoButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_INFO);
			mInfoButton.mClippingEnabled = false;
			mHeadingWidgets.Add(mInfoButton);
			AddWidget(mInfoButton);
			mExpandButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_DROPDOWN_RIGHT);
			mHeadingWidgets.Add(mExpandButton);
			mExpandButton.mClippingEnabled = false;
			AddWidget(mExpandButton);
			mEnabledCheckbox = new Bej3Checkbox(2, this);
			mHeadingWidgets.Add(mEnabledCheckbox);
			AddWidget(mEnabledCheckbox);
		}

		public override void Update()
		{
			if (mHeight != mTargetHeight)
			{
				mCurrentHeight += ((float)mTargetHeight - mCurrentHeight) * ConstantsWP.EXPANDER_SPEED;
				mHeight = (int)mCurrentHeight;
				if (Math.Abs(mHeight - mTargetHeight) < 1)
				{
					mHeight = mTargetHeight;
				}
			}
			if (mY != mTargetY)
			{
				mCurrentY += ((float)mTargetY - mCurrentY) * ConstantsWP.EXPANDER_SPEED;
				mY = (int)mCurrentY;
				if (Math.Abs(mY - mTargetY) < 1)
				{
					mY = mTargetY;
				}
			}
			if (mChildrenAlpha != mTargetChildrenAlpha)
			{
				mChildrenAlpha += (mTargetChildrenAlpha - mChildrenAlpha) * ConstantsWP.EXPANDER_SPEED;
				if (Math.Abs(mChildrenAlpha - mTargetChildrenAlpha) < 0.01f)
				{
					mChildrenAlpha = mTargetChildrenAlpha;
				}
			}
			base.Update();
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			if (mWidgetManager != null && mPriority > mWidgetManager.mMinDeferredOverlayPriority)
			{
				mWidgetManager.FlushDeferredOverlayWidgets(mPriority);
			}
			if (mClip && (theFlags.GetFlags() & 8) != 0)
			{
				g.ClipRect(0, 0, mWidth, mHeight);
			}
			if ((theFlags.GetFlags() & 4) != 0)
			{
				g.PushState();
				Draw(g);
				g.PopState();
			}
			for (int i = 0; i < mHeadingWidgets.Count; i++)
			{
				SexyFramework.Widget.Widget widget = mHeadingWidgets[i];
				if (widget.mVisible)
				{
					if (mWidgetManager != null && widget == mWidgetManager.mBaseModalWidget)
					{
						theFlags.mIsOver = true;
					}
					g.PushState();
					g.Translate(widget.mX, widget.mY);
					widget.DrawAll(theFlags, g);
					widget.mDirty = false;
					g.PopState();
				}
			}
			g.SetColor(new Color(255, 255, 255, (int)(255f * mChildrenAlpha)));
			g.PushColorMult();
			g.SetScale(1f, mChildrenAlpha, 0f, mMinHeight);
			if (mChildrenAlpha > 0f)
			{
				for (int j = 0; j < mContainedWidgets.Count; j++)
				{
					SexyFramework.Widget.Widget widget2 = mContainedWidgets[j];
					if (widget2.mVisible)
					{
						if (mWidgetManager != null && widget2 == mWidgetManager.mBaseModalWidget)
						{
							theFlags.mIsOver = true;
						}
						g.PushState();
						g.Translate(widget2.mX, widget2.mY);
						widget2.DrawAll(theFlags, g);
						widget2.mDirty = false;
						g.PopState();
					}
				}
			}
			g.PopColorMult();
			g.SetScale(1f, 1f, 0f, 0f);
		}

		public override void Draw(Graphics g)
		{
			if (mHasDivider)
			{
				Bej3Widget.DrawDividerCentered(g, mWidth / 2, mDividerOffset, true);
			}
		}

		public override void AddWidget(SexyFramework.Widget.Widget theWidget)
		{
			base.AddWidget(theWidget);
			if (!mHeadingWidgets.Contains(theWidget))
			{
				mContainedWidgets.Add(theWidget);
			}
			mWidgetsAdded = true;
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			if (theHeight < mMinHeight)
			{
				theHeight = mMinHeight;
			}
			mTargetY = theY;
			base.Resize(theX, theY, theWidth, theHeight);
			mCurrentY = mY;
			mCurrentHeight = mHeight;
			int num = 0;
			int num2 = 0;
			int num3 = ConstantsWP.EXPANDER_MIN_HEIGHT / 2;
			if (mHasDivider)
			{
				num3 += ConstantsWP.EXPANDER_DIVIDER_OFFSET;
			}
			mEnabledCheckbox.Resize(mWidth - ConstantsWP.EXPANDER_CHECKBOX_X - num, num3 + ConstantsWP.EXPANDER_CHECKBOX_Y, 0, 0);
			num2 = mExpandButton.mHeight;
			mExpandButton.Resize(ConstantsWP.EXPANDER_BUTTON_X, num3 - num2 / 2 + ConstantsWP.EXPANDER_BUTTON_Y_OFFSET, 0, 0);
			num = mInfoButton.mWidth;
			num2 = mInfoButton.mHeight;
			mInfoButton.Resize(mWidth - ConstantsWP.EXPANDER_INFO_BUTTON_X - num, num3 - num2 / 2 + ConstantsWP.EXPANDER_BUTTON_Y_OFFSET, 0, 0);
			num3 = (mHasDivider ? ConstantsWP.EXPANDER_DIVIDER_OFFSET : 0);
			mHeadingLabel.Resize(mWidth / 2, num3 + ConstantsWP.EXPANDER_LABEL_Y, 0, 0);
		}

		public virtual void ButtonDepress(int theId)
		{
			switch (theId)
			{
			case 0:
				if (mTargetHeight == mExpandedHeight)
				{
					Collapse(true);
				}
				else
				{
					Expand();
				}
				break;
			case 1:
				GlobalMembers.gApp.DoDialog(49, true, mInfoHeader, mInfoMessage, SexyFramework.GlobalMembers._ID("OK", 3219), 3, 3, 3);
				break;
			}
		}

		public virtual void CheckboxChecked(int theId, bool checked1)
		{
			int num = 2;
			mCheckListener.CheckboxChecked(mId, checked1);
			LinkUpAssets();
		}

		public override void LinkUpAssets()
		{
			mExpandButton.LinkUpAssets();
			mEnabledCheckbox.LinkUpAssets();
			mInfoButton.LinkUpAssets();
			DisableChildren(false);
			Resize(mX, mY, mWidth, mHeight);
		}

		public void Expand()
		{
			if (mWidgetsAdded || mTargetHeight != mExpandedHeight)
			{
				mTargetHeight = mExpandedHeight;
				mExpandButton.SetType(Bej3ButtonType.BUTTON_TYPE_DROPDOWN_DOWN);
				mTargetChildrenAlpha = 1f;
				DisableChildren(false);
				mListener.ExpanderChanged(mId, true);
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ZEN_MENUEXPAND);
				mWidgetsAdded = false;
			}
		}

		public void Collapse(bool notifyListener)
		{
			if (mWidgetsAdded || mTargetHeight != mMinHeight)
			{
				mTargetHeight = mMinHeight;
				mExpandButton.SetType(Bej3ButtonType.BUTTON_TYPE_DROPDOWN_RIGHT);
				mTargetChildrenAlpha = 0f;
				DisableChildren(true);
				if (notifyListener)
				{
					mListener.ExpanderChanged(mId, false);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ZEN_MENUEXPAND);
				}
				mWidgetsAdded = false;
			}
		}

		public void SetExpandedHeight(int theExpandedHeight)
		{
			mExpandedHeight = theExpandedHeight;
		}

		public int GetAbsTargetPosition()
		{
			return mTargetY + mTargetHeight;
		}

		public void MoveToY(int y)
		{
			mTargetY = y;
		}

		public void ForceSetToTarget()
		{
			mCurrentHeight = (mHeight = mTargetHeight);
			mCurrentY = (mY = mTargetY);
			mChildrenAlpha = mTargetChildrenAlpha;
		}

		public void ButtonPress(int theId)
		{
		}

		public void ButtonMouseEnter(int theId)
		{
		}

		public void ButtonMouseLeave(int theId)
		{
		}

		public bool ButtonsEnabled()
		{
			return false;
		}

		public void ButtonPress(int theId, int theClickCount)
		{
		}

		public void ButtonDownTick(int theId)
		{
		}

		public void ButtonMouseMove(int theId, int theX, int theY)
		{
		}
	}
}
