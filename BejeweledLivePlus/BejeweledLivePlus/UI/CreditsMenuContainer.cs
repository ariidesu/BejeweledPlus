using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	public class CreditsMenuContainer : Bej3Widget, ScrollWidgetListener
	{
		private List<string> mRoles = new List<string>();

		private List<string> mNames = new List<string>();

		private List<string> mSubheadings = new List<string>();

		private List<string> mExtraMessages = new List<string>();

		private List<Point> mRolePositions = new List<Point>();

		private List<Point> mNamePositions = new List<Point>();

		private List<Point> mSubheadingPositions = new List<Point>();

		private List<int> nameCount = new List<int>();

		private List<Point> mExtraMessagePositions = new List<Point>();

		private readonly int SUBHEADING1;

		private readonly int SUBHEADING2 = 9;

		private readonly int SUBHEADING3 = 20;

		private readonly int SUBHEADING4 = 26;

		private bool done;

		private void SplitUpString(string input, List<string> roles, List<int> nameCount1)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			do
			{
				num2 = input.IndexOf(",", num3);
				if (num2 != -1)
				{
					string text = input.Substring(num3, num2 - num3);
					roles.Add(text);
					if (text.Length > 0)
					{
						num++;
					}
					num3 = num2 + 1;
				}
				else
				{
					roles.Add(input.Substring(num3, input.Length - num3));
					num++;
				}
			}
			while (num2 != -1);
			nameCount1.Add(num);
		}

		private Color GetColor(Color baseColour, int baseY)
		{
			return baseColour;
		}

		private bool IsInVisibleRange(int absY, Graphics g)
		{
			if (absY > 0)
			{
				return absY < GlobalMembers.gApp.mHeight;
			}
			return false;
		}

		public CreditsMenuContainer()
			: base(Menu_Type.MENU_CREDITSMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mDoesSlideInFromBottom = (mCanAllowSlide = false);
			Resize(0, 0, GlobalMembers.gApp.mWidth - ConstantsWP.CREDITSMENU_PADDING_X * 2, ConstantsWP.CREDITSMENU_HEIGHT);
			mRoles.Add(GlobalMembers._ID("iOS Team", 3245));
			SplitUpString("", mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Production", 3246));
			SplitUpString(GlobalMembers._ID("Viktorya Hollings,Jim McDonagh,JP Vaughan", 3265), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Game Design", 3247));
			SplitUpString(GlobalMembers._ID("David Bishop,Antubel Moreda", 3266), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Programming", 3248));
			SplitUpString(GlobalMembers._ID("Yang Han,Alanna Kelly,Stuart Johnson,Robert Lester,Paolo Maninetti,Paul O'Donnell,PJ O'Halloran,Christian Schinkoethe,Narinder Singh Basran", 3267), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Art", 3249));
			SplitUpString(GlobalMembers._ID("Lee Davies,Riana McKeith", 3268), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Writing", 3250));
			SplitUpString(GlobalMembers._ID("David Bishop,Borja Guillan,Antubel Moreda", 3269), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Quality Assurance", 3251));
			SplitUpString(GlobalMembers._ID("Adam Beck,Didier Canovas,David Cleaveley,Aaron Collum,Colm Gallagher,Stephen Geddes,Kar Hay Ho,Brian Lelas,Carlos Losada,Philip Plunkett,Ildefonso Ranchal,Carl Sidebotham,Viacheslav Zakhariev", 3270), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Localization", 3252));
			SplitUpString(GlobalMembers._ID("Antonio Asensio Pérez,Karl Byrne,Mark Coffey,Jean De Merey,Shinobu Koiwa,Anthony Mackey,Silvie McCullough,John Paul Newman,Lorenzo Penati,Jessica Schuster,Jonathon Young", 3271), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Special Thanks", 3253));
			SplitUpString(GlobalMembers._ID("Ed Allard,Sharon Archbold,Sameer Baroova,Aoife Brennan,Paul Breslin,Bob Chamberlain,John Coleman,Valeria Colnaghi,Giordano Contestabile,Meriem Djazouli,Meredith Dorrance,Eddie Dowse,Plamen Dragozov,Brian Fiete,Kieran Gleeson,John Halloran,Gillian Hayes,Nicole LeMaster,Linda McGee,Cormac Mulhall,Kate O'Brien,Cathy Orr,Stiw Puljic,Guillaume Richard,David Roberts,Jennifer Staunton,Andrew Stein,John Vechey,David Ward,Ember Wardrop", 3272), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("PC/Mac Team", 3254));
			SplitUpString("", mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Production", 3255));
			SplitUpString(GlobalMembers._ID("Michael Guillory,Jason Kapalka,Sukhbir Sidhu", 3273), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Game Design", 3256));
			SplitUpString(GlobalMembers._ID("Brian Fiete,Jason Kapalka,Josh Langley", 3274), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Programming", 3257));
			SplitUpString(GlobalMembers._ID("Jeremy Bilas,Brian Fiete,Chris Hargrove,Josh Langley,Joe Mobley,Matt Scott,Jacob VanWingen", 3275), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Art", 3258));
			SplitUpString(GlobalMembers._ID("Jim Abraham,Misael Armendariz,Gene Blakefield,Marcia Broderick,Tysen Henderson,Matt Holmberg,Jordan Kotzebue,Josh Langley,Dereck McCaughan,Bill Olmstead,Rick Schmitz,Rich Werner", 3276), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Audio", 3259));
			SplitUpString(GlobalMembers._ID("Alexander Brandon (Funky Rustic),Peter Hajba,Gregory Hinde,Jason Kapalka,Zachary Throne", 3277), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Writing", 3260));
			SplitUpString(GlobalMembers._ID("Stephen Notley", 3278), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Quality Assurance", 3261));
			SplitUpString(GlobalMembers._ID("Sharon Bruhn,David Chan,Bob Church,David Cole,Bill Dennes,Jon Fleming,Michael Guillory,Abigail Houghton,Ed Miller,Mike Racioppi,DJ Stiner,Chad Zoellner", 3279), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Localization", 3262));
			SplitUpString(GlobalMembers._ID("Karl Byrne,Jean De Merey,Anthony Mackey,John Paul Newman,Lorenzo Penati,Antonio Pérez,Jessica Schuster,Jonathon Young", 3280), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Release Management", 3263));
			SplitUpString(GlobalMembers._ID("Irene Cheung,Daniel Landeck,Eric Olson,Nick Tomilson", 3281), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Special Thanks", 3264));
			SplitUpString(GlobalMembers._ID("Ed Allard,Yvette Camacho,Garth Chouteau,Leigh Daughtridge,Glenn Drover,Cristina Estrada-Eligio,Liz Harris,Amy Hevron,Curtis Kuhn,Nicole LeMaster,Kong Lu,Cathy Orr,Kelley Poston,Ron Powers,David Roberts,Ben Rotholtz,John Vechey,Eve Warmflash,Paula Wong,PopCap Beta Testers", 3282), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("WP7 Team", 4000));
			SplitUpString("", mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Executive Producer", 4001));
			SplitUpString(GlobalMembers._ID("D. Cicurel", 4002), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Production", 3255));
			SplitUpString(GlobalMembers._ID("D. Chen", 4004), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Programming", 3248));
			SplitUpString(GlobalMembers._ID("Y. Liu,X. Yin,L. Ran,C. Wang,C. Liu", 4006), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Art", 3258));
			SplitUpString(GlobalMembers._ID("M. Xu", 4008), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Quality Assurance", 3261));
			SplitUpString(GlobalMembers._ID("Y. Feng", 4010), mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("Development Team", 6000));
			SplitUpString("", mNames, nameCount);
			mRoles.Add(GlobalMembers._ID("", 9998));
			SplitUpString(GlobalMembers._ID("Thomas Valmorin,Kenneth Holm,George Applegate,EA QA India,EA LT", 6001), mNames, nameCount);
			mExtraMessages.Add(GlobalMembers._ID("Thank you for playing!", 3283));
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Draw(Graphics g)
		{
			g.SetColorizeImages(true);
			g.SetColor(Color.White);
			int num = ConstantsWP.CREDITSMENU_LOGO_Y + ConstantsWP.CREDITSMENU_SCROLL_START;
			num = ((-mY >= GlobalMembers.gApp.mHeight / 2) ? (num + ConstantsWP.CREDITSMENU_LOGO_FADE_OFFSET_TOP) : (num + ConstantsWP.CREDITSMENU_LOGO_FADE_OFFSET_BOTTOM));
			if (IsInVisibleRange(num + (int)g.mTransY, g))
			{
				Image iMAGE_MAIN_MENU_LOGO = GlobalMembersResourcesWP.IMAGE_MAIN_MENU_LOGO;
				int theX = mWidth / 2 - (int)((float)iMAGE_MAIN_MENU_LOGO.mWidth * 1.12f / 2f);
				int theY = ConstantsWP.CREDITSMENU_LOGO_Y + ConstantsWP.CREDITSMENU_SCROLL_START;
				g.DrawImage(iMAGE_MAIN_MENU_LOGO, theX, theY, (int)((float)iMAGE_MAIN_MENU_LOGO.mWidth * 1.12f), (int)((float)iMAGE_MAIN_MENU_LOGO.mHeight * 1.12f));
			}
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HEADER, 0, Bej3Widget.COLOR_HEADING_GLOW_1);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Bej3Widget.COLOR_SUBHEADING_1_FILL);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Bej3Widget.COLOR_SUBHEADING_1_STROKE);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, Bej3Widget.COLOR_DIALOG_WHITE);
			g.SetFont(GlobalMembersResources.FONT_HEADER);
			for (int i = 0; i < mSubheadings.Count; i++)
			{
				int num2 = mSubheadingPositions[i].mY + (int)g.mTransY;
				if (num2 > GlobalMembers.gApp.mHeight)
				{
					break;
				}
				if (num2 >= 0)
				{
					g.DrawString(mSubheadings[i], mSubheadingPositions[i].mX, mSubheadingPositions[i].mY);
				}
			}
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			for (int j = 0; j < mRoles.Count; j++)
			{
				int num3 = mRolePositions[j].mY + (int)g.mTransY;
				if (num3 > GlobalMembers.gApp.mHeight)
				{
					break;
				}
				if (num3 >= 0)
				{
					g.DrawString(mRoles[j], mRolePositions[j].mX, mRolePositions[j].mY);
				}
			}
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			for (int k = 0; k < mNames.Count; k++)
			{
				int num4 = mNamePositions[k].mY + (int)g.mTransY;
				if (num4 > GlobalMembers.gApp.mHeight)
				{
					break;
				}
				if (num4 >= 0)
				{
					g.DrawString(mNames[k], mNamePositions[k].mX, mNamePositions[k].mY);
				}
			}
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			for (int l = 0; l < mExtraMessages.Count; l++)
			{
				if (IsInVisibleRange(mExtraMessagePositions[l].mY + (int)g.mTransY, g))
				{
					g.DrawString(mExtraMessages[l], mExtraMessagePositions[l].mX, mExtraMessagePositions[l].mY);
				}
			}
		}

		public override void Show()
		{
			base.Show();
			if (done)
			{
				return;
			}
			Graphics graphics = new Graphics();
			graphics.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			int num = ConstantsWP.CREDITSMENU_START + ConstantsWP.CREDITSMENU_SCROLL_START;
			for (int i = 0; i < mRoles.Count; i++)
			{
				Point item = new Point(mWidth / 2 - graphics.StringWidth(mRoles[i]) / 2, num);
				mRolePositions.Add(item);
				int num2 = 0;
				if (nameCount[i] > 0)
				{
					num2 = ConstantsWP.CREDITSMENU_ROLE_DIST;
				}
				num2 += nameCount[i] * ConstantsWP.CREDITSMENU_NAME_HEIGHT;
				num += num2;
			}
			graphics.SetFont(GlobalMembersResources.FONT_DIALOG);
			int num3 = 0;
			int num4 = 0;
			num = mRolePositions[num3].mY + ConstantsWP.CREDITSMENU_NAME_HEIGHT;
			for (int j = 0; j < mNames.Count; j++)
			{
				int num5 = mWidth / 2;
				Point item2 = new Point(num5 - graphics.StringWidth(mNames[j]) / 2, num);
				mNamePositions.Add(item2);
				num += ConstantsWP.CREDITSMENU_NAME_HEIGHT;
				num4++;
				if (num4 == nameCount[num3])
				{
					num4 = 0;
					num3++;
					if (num3 < mRolePositions.Count)
					{
						num = mRolePositions[num3].mY + ConstantsWP.CREDITSMENU_NAME_HEIGHT;
					}
				}
			}
			graphics.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			num = mHeight - mExtraMessages.Count * ConstantsWP.CREDITSMENU_EXTRA_MESSAGE_HEIGHT - ConstantsWP.CREDITSMENU_EXTRA_MESSAGE_OFFSET;
			for (int k = 0; k < mExtraMessages.Count; k++)
			{
				Point item3 = new Point(mWidth / 2 - graphics.StringWidth(mExtraMessages[k]) / 2, num + 450);
				mExtraMessagePositions.Add(item3);
				num += ConstantsWP.CREDITSMENU_EXTRA_MESSAGE_HEIGHT;
			}
			mRolePositions[SUBHEADING1] = new Point(mRolePositions[SUBHEADING1].mX, mRolePositions[SUBHEADING1].mY + ConstantsWP.CREDITSMENU_SUB_HEADING_DIST);
			mRolePositions[SUBHEADING2] = new Point(mRolePositions[SUBHEADING2].mX, mRolePositions[SUBHEADING2].mY + ConstantsWP.CREDITSMENU_SUB_HEADING_DIST);
			mRolePositions[SUBHEADING3] = new Point(mRolePositions[SUBHEADING3].mX, mRolePositions[SUBHEADING3].mY + ConstantsWP.CREDITSMENU_SUB_HEADING_DIST);
			mRolePositions[SUBHEADING4] = new Point(mRolePositions[SUBHEADING4].mX, mRolePositions[SUBHEADING4].mY + ConstantsWP.CREDITSMENU_SUB_HEADING_DIST);
			mSubheadingPositions.Add(mRolePositions[SUBHEADING1]);
			mSubheadingPositions.Add(mRolePositions[SUBHEADING2]);
			mSubheadingPositions.Add(mRolePositions[SUBHEADING3]);
			mSubheadingPositions.Add(mRolePositions[SUBHEADING4]);
			mRolePositions.RemoveAt(SUBHEADING1);
			mRolePositions.RemoveAt(SUBHEADING2 - 1);
			mRolePositions.RemoveAt(SUBHEADING3 - 2);
			mRolePositions.RemoveAt(SUBHEADING4 - 3);
			mSubheadings.Add(mRoles[SUBHEADING1]);
			mSubheadings.Add(mRoles[SUBHEADING2]);
			mSubheadings.Add(mRoles[SUBHEADING3]);
			mSubheadings.Add(mRoles[SUBHEADING4]);
			mRoles.RemoveAt(SUBHEADING1);
			mRoles.RemoveAt(SUBHEADING2 - 1);
			mRoles.RemoveAt(SUBHEADING3 - 2);
			mRoles.RemoveAt(SUBHEADING4 - 3);
			graphics.SetFont(GlobalMembersResources.FONT_HEADER);
			for (int l = 0; l < mSubheadingPositions.Count; l++)
			{
				int theX = mWidth / 2 - graphics.StringWidth(mSubheadings[l]) / 2;
				mSubheadingPositions[l] = new Point(theX, mSubheadingPositions[l].mY);
			}
			done = true;
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public override void PlayMenuMusic(bool isRestart = false)
		{
		}

		public override void ButtonDepress(int theId)
		{
		}
	}
}
