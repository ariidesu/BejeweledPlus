using System.Collections.Generic;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	internal class ProfileMenuButtonBadges : ProfileMenuButton
	{
		private List<int> mBadgesDisplayed = new List<int>();

		private Label mNoBadgesMessageLabel;

		public ProfileMenuButtonBadges(Bej3ButtonListener theListener)
			: base(6, theListener, "")
		{
			MakeChildrenTouchInvisible();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.SetScale(ConstantsWP.PROFILEMENU_BADGE_SCALE, ConstantsWP.PROFILEMENU_BADGE_SCALE, mWidth / 2, 0f);
			for (int i = 0; i < mBadgesDisplayed.Count; i++)
			{
				int badgeId = mBadgesDisplayed[i];
				int theCel = 1;
				if (Profile.IsEliteBadge(badgeId))
				{
					theCel = 5;
				}
				Image smallBadgeImage = BadgeMenu.GetSmallBadgeImage(badgeId);
				g.DrawImageCel(smallBadgeImage, (int)((float)(ConstantsWP.PROFILEMENU_BADGE_X + i * ConstantsWP.PROFILEMENU_BADGE_WIDTH) - (float)(smallBadgeImage.GetCelWidth() / 2) * ConstantsWP.PROFILEMENU_BADGE_SCALE), (int)((float)ConstantsWP.PROFILEMENU_BADGE_Y - (float)(smallBadgeImage.GetCelHeight() / 2) * ConstantsWP.PROFILEMENU_BADGE_SCALE), 0);
				g.DrawImageCel(GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS, (int)((float)(ConstantsWP.PROFILEMENU_BADGE_X + i * ConstantsWP.PROFILEMENU_BADGE_WIDTH) - (float)(GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS.GetCelWidth() / 2) * ConstantsWP.PROFILEMENU_BADGE_SCALE), (int)((float)ConstantsWP.PROFILEMENU_BADGE_Y - (float)(GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS.GetCelHeight() / 2) * ConstantsWP.PROFILEMENU_BADGE_SCALE), theCel);
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			mBadgesDisplayed.Clear();
			for (int i = 0; i < 3; i++)
			{
				int num = GlobalMembers.gApp.mProfile.mRecentBadges[i];
				if (num >= 0)
				{
					mBadgesDisplayed.Add(num);
				}
			}
			mNoBadgesMessageLabel.SetTextBlock(new Rect(ConstantsWP.PROFILEMENU_BUTTON_EXTRA_MESSAGE_X, ConstantsWP.PROFILEMENU_BUTTON_EXTRA_MESSAGE_Y, ConstantsWP.PROFILEMENU_BUTTON_EXTRA_MESSAGE_WIDTH, 0), true);
			mNoBadgesMessageLabel.SetVisible(mBadgesDisplayed.Count == 0);
		}
	}
}
