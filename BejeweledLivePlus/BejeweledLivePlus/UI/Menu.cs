using System.Collections.Generic;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class Menu : SexyFramework.Widget.Widget, Bej3ButtonListener, ButtonListener
	{
		public ButtonWidget mClassicButton;

		public ButtonWidget mBlitz1Button;

		public ButtonWidget mBlitz2Button;

		public ButtonWidget mZenButton;

		public ButtonWidget mQuitButton;

		public ButtonWidget mMainMenuButton;

		public ButtonWidget mQuestModeButton;

		public ButtonWidget mSecretModeButton;

		public List<ButtonWidget> mQuestButton = new List<ButtonWidget>();

		public Menu()
		{
			InitWidgets();
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, false);
			mQuestButton.Clear();
			base.Dispose();
		}

		public void InitWidgets()
		{
			mClassicButton = new ButtonWidget(1000, this);
			mClassicButton.SetFont(GlobalMembersResources.FONT_DIALOG);
			mClassicButton.mLabel = GlobalMembers._ID("Play Classic", 293);
			AddWidget(mClassicButton);
			mBlitz1Button = new ButtonWidget(1001, this);
			mBlitz1Button.SetFont(GlobalMembersResources.FONT_DIALOG);
			mBlitz1Button.mLabel = GlobalMembers._ID("Time Bonus", 294);
			AddWidget(mBlitz1Button);
			mBlitz2Button = new ButtonWidget(1002, this);
			mBlitz2Button.SetFont(GlobalMembersResources.FONT_DIALOG);
			mBlitz2Button.mLabel = GlobalMembers._ID("Tug of War", 295);
			AddWidget(mBlitz2Button);
			mZenButton = new ButtonWidget(1005, this);
			mZenButton.SetFont(GlobalMembersResources.FONT_DIALOG);
			mZenButton.mLabel = GlobalMembers._ID("Play Zen", 296);
			AddWidget(mZenButton);
			mQuestModeButton = new ButtonWidget(1003, this);
			mQuestModeButton.SetFont(GlobalMembersResources.FONT_DIALOG);
			mQuestModeButton.mLabel = GlobalMembers._ID("Play Quests", 297);
			AddWidget(mQuestModeButton);
			mSecretModeButton = new ButtonWidget(1020, this);
			mSecretModeButton.SetFont(GlobalMembersResources.FONT_DIALOG);
			mSecretModeButton.mLabel = GlobalMembers._ID("Secret Modes", 298);
			AddWidget(mSecretModeButton);
			mQuitButton = new ButtonWidget(1004, this);
			mQuitButton.SetFont(GlobalMembersResources.FONT_DIALOG);
			mQuitButton.mLabel = GlobalMembers._ID("Quit", 299);
			AddWidget(mQuitButton);
			mMainMenuButton = new ButtonWidget(1006, this);
			mMainMenuButton.SetFont(GlobalMembersResources.FONT_DIALOG);
			mMainMenuButton.mLabel = GlobalMembers._ID("Main Menu", 300);
			AddWidget(mMainMenuButton);
		}

		public override void Resize(Rect theRect)
		{
			base.Resize(theRect);
			mClassicButton.Resize(theRect.mWidth / 2 - GlobalMembers.S(650), GlobalMembers.S(40), GlobalMembers.S(250), GlobalMembers.S(100));
			mBlitz1Button.Resize(theRect.mWidth / 2 - GlobalMembers.S(375), GlobalMembers.S(40), GlobalMembers.S(250), GlobalMembers.S(50));
			mBlitz2Button.Resize(theRect.mWidth / 2 - GlobalMembers.S(375), GlobalMembers.S(90), GlobalMembers.S(250), GlobalMembers.S(50));
			mZenButton.Resize(theRect.mWidth / 2 - GlobalMembers.S(100), GlobalMembers.S(40), GlobalMembers.S(200), GlobalMembers.S(100));
			mQuestModeButton.Resize(theRect.mWidth / 2 + GlobalMembers.S(125), GlobalMembers.S(40), GlobalMembers.S(250), GlobalMembers.S(100));
			mSecretModeButton.Resize(theRect.mWidth / 2 + GlobalMembers.S(400), GlobalMembers.S(40), GlobalMembers.S(250), GlobalMembers.S(100));
			mMainMenuButton.Resize(theRect.mWidth / 2 - GlobalMembers.S(250), mHeight - GlobalMembers.S(80), GlobalMembers.S(250), GlobalMembers.S(60));
			mQuitButton.Resize(theRect.mWidth / 2 - GlobalMembers.S(-50), mHeight - GlobalMembers.S(80), GlobalMembers.S(250), GlobalMembers.S(60));
			for (int i = 0; i < mQuestButton.Count; i++)
			{
				int theX = theRect.mWidth / 2 - GlobalMembers.S(620) + i % 3 * GlobalMembers.S(420);
				int theY = GlobalMembers.MS(190) + i / 3 * GlobalMembers.S(55);
				mQuestButton[i].Resize(theX, theY, GlobalMembers.S(400), GlobalMembers.S(45));
			}
		}

		public override void Draw(Graphics g)
		{
			g.SetColor(new Color(64, 64, 64));
			g.FillRect(0, 0, mWidth, mHeight);
			g.SetColor(new Color(32, 32, 32));
			g.FillRect(mWidth / 2 - GlobalMembers.MS(660), GlobalMembers.MS(160), GlobalMembers.MS(1320), GlobalMembers.MS(930));
		}

		public override void KeyChar(char theChar)
		{
			char c = theChar;
			if (c == 'r')
			{
				GlobalMembers.gApp.LoadConfigs();
				base.RemoveAllWidgets(true, false);
				mQuestButton.Clear();
				InitWidgets();
				Resize(GlobalMembers.gApp.mScreenBounds);
			}
		}

		public bool ButtonsEnabled()
		{
			return false;
		}

		public void ButtonPress(int theId)
		{
		}

		public void ButtonPress(int theId, int theClickCount)
		{
		}

		public void ButtonDepress(int theId)
		{
		}

		public void ButtonDownTick(int theId)
		{
		}

		public void ButtonMouseEnter(int theId)
		{
		}

		public void ButtonMouseLeave(int theId)
		{
		}

		public void ButtonMouseMove(int theId, int theX, int theY)
		{
		}
	}
}
