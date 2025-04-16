using System.Collections.Generic;
using System.Linq;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class NewUserDialog : Bej3Dialog, Bej3EditListener, EditListener
	{
		protected int mTextBoxOffset;

		public Bej3EditWidget mNameWidget;

		public string mOrigString;

		public bool mHasForcedUppercase;

		public virtual void EditWidgetGotFocus(int id)
		{
		}

		public virtual void EditWidgetLostFocus(int id)
		{
		}

		public virtual bool AllowKey(int id, KeyCode key)
		{
			return true;
		}

		public virtual void EditWidgetText(int theId, string theString)
		{
			ButtonDepress(1000);
		}

		protected override void SlideInFinished()
		{
			mWidgetManager.SetFocus(mNameWidget);
			base.SlideInFinished();
		}

		public NewUserDialog(bool isRename)
			: base((!isRename) ? 1 : 2, true, isRename ? GlobalMembers._ID("EDIT NAME", 3414) : GlobalMembers._ID("WELCOME!", 303), "", "", 2, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
		{
			bool flag = true;
			mTextBoxOffset = ConstantsWP.WELCOME_DIALOG_TEXTBOX_Y_OFFSET;
			mNameWidget = new Bej3EditWidget(1, this);
			mNameWidget.SetFont(GlobalMembersResources.FONT_DIALOG);
			mNameWidget.SetColor(0, new Color(0, 0, 0, 0));
			mNameWidget.SetColor(1, new Color(0, 0, 0, 0));
			mNameWidget.SetColor(2, Color.White);
			mNameWidget.SetColor(3, Color.White);
			mNameWidget.SetColor(4, Color.Black);
			mNameWidget.mMaxChars = 16;
			mNameWidget.mCursorOffset = GlobalMembers.M(-5);
			mNameWidget.SetText("");
			AddWidget(mNameWidget);
			mHasForcedUppercase = false;
			mNameWidget.mVisible = true;
			mMessageLabel.SetText(GlobalMembers._ID("Enter your name:", 305));
			mMessageLabel.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			if (flag)
			{
				mButtons.Clear();
				Bej3Button bej3Button = new Bej3Button(mYesButton.mId, this, Bej3ButtonType.BUTTON_TYPE_LONG);
				bej3Button.SetLabel(mYesButton.mLabel);
				GlobalMembers.KILL_WIDGET(mYesButton);
				mYesButton = bej3Button;
				mButtons.Add(mYesButton);
				AddWidget(mYesButton);
				bej3Button = new Bej3Button(mNoButton.mId, this, Bej3ButtonType.BUTTON_TYPE_LONG);
				bej3Button.SetLabel(mNoButton.mLabel);
				GlobalMembers.KILL_WIDGET(mNoButton);
				mNoButton = bej3Button;
				mButtons.Add(mNoButton);
				AddWidget(mNoButton);
			}
			LinkUpAssets();
		}

		public NewUserDialog(bool isRename, bool doButtons)
			: base((!isRename) ? 1 : 2, true, isRename ? GlobalMembers._ID("EDIT NAME", 3414) : GlobalMembers._ID("WELCOME!", 303), "", "", doButtons ? 2 : 0, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.BUTTON_TYPE_LONG, Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
		{
			mTextBoxOffset = ConstantsWP.WELCOME_DIALOG_TEXTBOX_Y_OFFSET;
			mNameWidget = new Bej3EditWidget(1, this);
			mNameWidget.SetFont(GlobalMembersResources.FONT_DIALOG);
			mNameWidget.SetColor(0, new Color(0, 0, 0, 0));
			mNameWidget.SetColor(1, new Color(0, 0, 0, 0));
			mNameWidget.SetColor(2, Color.White);
			mNameWidget.SetColor(3, Color.White);
			mNameWidget.SetColor(4, Color.Black);
			mNameWidget.mMaxChars = 16;
			mNameWidget.mCursorOffset = GlobalMembers.M(-5);
			mNameWidget.SetText("");
			AddWidget(mNameWidget);
			mHasForcedUppercase = false;
			mNameWidget.mVisible = true;
			mMessageLabel.SetText(GlobalMembers._ID("Enter your name:", 305));
			mMessageLabel.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			if (doButtons)
			{
				mButtons.Clear();
				Bej3Button bej3Button = new Bej3Button(mYesButton.mId, this, Bej3ButtonType.BUTTON_TYPE_LONG);
				bej3Button.SetLabel(mYesButton.mLabel);
				GlobalMembers.KILL_WIDGET(mYesButton);
				mYesButton = bej3Button;
				mButtons.Add(mYesButton);
				AddWidget(mYesButton);
				bej3Button = new Bej3Button(mNoButton.mId, this, Bej3ButtonType.BUTTON_TYPE_LONG);
				bej3Button.SetLabel(mNoButton.mLabel);
				GlobalMembers.KILL_WIDGET(mNoButton);
				mNoButton = bej3Button;
				mButtons.Add(mNoButton);
				AddWidget(mNoButton);
			}
			LinkUpAssets();
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			theWidth = ConstantsWP.NEWUSERDIALOG_WIDTH;
			base.Resize(theX, theY, theWidth, theHeight);
			mY = ((mId == 2) ? ConstantsWP.MENU_Y_POS_HIDDEN : GlobalMembers.gApp.mHeight);
			mTargetPos = ConstantsWP.NEWUSERDIALOG_Y;
			mNameWidget.Resize((mWidth - ConstantsWP.NEWUSERDIALOG_TEXTBOX_WIDTH) / 2, mHeight - mTextBoxOffset, ConstantsWP.NEWUSERDIALOG_TEXTBOX_WIDTH, ConstantsWP.EDITWIDGET_HEIGHT);
			mHeadingLabel.mY = ConstantsWP.NEWUSERDIALOG_HEADING_Y;
		}

		public override void Draw(Graphics g)
		{
			Bej3Widget.DrawDialogBox(g, mWidth);
			Bej3Widget.DrawDividerCentered(g, mWidth / 2, ConstantsWP.NEWUSERDIALOG_DIVIDER_Y, false);
		}

		public virtual bool AllowChar(int theId, char theChar)
		{
			if (mNameWidget.mFont.CharWidth(theChar) == 0)
			{
				return false;
			}
			List<char> list = new List<char>();
			for (int i = 97; i <= 122; i++)
			{
				list.Add((char)i);
				list.Add((char)(i - 32));
			}
			for (int j = 48; j <= 57; j++)
			{
				list.Add((char)j);
			}
			list.Add(' ');
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k] == theChar)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool AllowText(int theId, string theText)
		{
			if (theText.Length == 1 && !mHasForcedUppercase)
			{
				mNameWidget.mString = mNameWidget.mString;
				mNameWidget.mString.ToArray()[0].ToString().ToUpper();
				mHasForcedUppercase = true;
			}
			return true;
		}

		public override int GetPreferredHeight(int theWidth)
		{
			return ConstantsWP.NEWUSERDIALOG_HEIGHT;
		}

		public string GetName()
		{
			return mNameWidget.mString;
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mMessageLabel.SetTextBlock(new Rect(ConstantsWP.DIALOGBOX_MESSAGE_PADDING_X, ConstantsWP.WELCOME_DIALOG_MESSAGE_Y, mWidth - ConstantsWP.DIALOGBOX_MESSAGE_PADDING_X * 2, mHeight - ConstantsWP.WELCOME_DIALOG_MESSAGE_Y), false);
		}

		public override void Kill()
		{
			base.Kill();
			mNameWidget.LostFocus();
		}
	}
}
