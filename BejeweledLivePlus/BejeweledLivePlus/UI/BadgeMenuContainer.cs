using System;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	internal class BadgeMenuContainer : Bej3Widget
	{
		private BadgeMenu mMenu;

		private static float[] Params;

		private Point GetBadgePosition(int badgeId)
		{
			Point absoluteBadgePosition = GetAbsoluteBadgePosition(badgeId, false);
			int badgePage = GetBadgePage(badgeId);
			absoluteBadgePosition.mX += ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH * badgePage;
			return absoluteBadgePosition;
		}

		public BadgeMenuContainer(BadgeMenu menu)
			: base(Menu_Type.MENU_BADGEMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mMenu = menu;
			Resize(0, 0, ConstantsWP.BADGEMENU_CONTAINER_PAGE_WIDTH * 3, ConstantsWP.BADGEMENU_CONTAINER_HEIGHT);
			mDoesSlideInFromBottom = false;
		}

		public void RenderBadges(Graphics g, bool isGrayscale, int onlyPage)
		{
			int theAlpha = g.GetFinalColor().mAlpha;
			for (int i = 0; i < 20; i++)
			{
				int badgePage = GetBadgePage(i);
				if (onlyPage != -1 && onlyPage != badgePage)
				{
					continue;
				}
				Point badgePosition = GetBadgePosition(i);
				int num = mMenu.mBadgeLevels[i];
				if ((num == 0 && isGrayscale) || (num > 0 && !isGrayscale))
				{
					Bej3Widget.DrawImageCentered(g, BadgeMenu.GetSmallBadgeImage(i), 0, badgePosition.mX, badgePosition.mY);
				}
				if (!isGrayscale)
				{
					if (num > 0 && Profile.IsEliteBadge(i))
					{
						Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS, 5, badgePosition.mX, badgePosition.mY);
					}
					else
					{
						Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS, num, badgePosition.mX, badgePosition.mY);
					}
				}
			}
			g.SetColor(new Color(255, 255, 255, theAlpha));
		}

		public void RenderBadges(Graphics g, bool isGrayscale)
		{
			int num = -1;
			int theAlpha = g.GetFinalColor().mAlpha;
			for (int i = 0; i < 20; i++)
			{
				int badgePage = GetBadgePage(i);
				if (num == -1 || num == badgePage)
				{
					Point badgePosition = GetBadgePosition(i);
					Badge badgeByIndex = BadgeManager.GetBadgeManagerInstance().GetBadgeByIndex(i);
					if (mMenu.mBadgeStatus[i])
					{
						Bej3Widget.DrawImageCentered(g, badgeByIndex.mSmallIcon, 0, badgePosition.mX, badgePosition.mY);
						Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS, (int)badgeByIndex.mLevel, badgePosition.mX, badgePosition.mY);
					}
					else
					{
						Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS, 0, badgePosition.mX, badgePosition.mY);
					}
				}
			}
			g.SetColor(new Color(255, 255, 255, theAlpha));
		}

		public void RenderGrayBadges(Graphics g)
		{
			float mScaleX = g.mScaleX;
			float mScaleY = g.mScaleY;
			for (int i = 0; i < 20; i++)
			{
				Point badgePosition = GetBadgePosition(i);
				Badge badgeByIndex = BadgeManager.GetBadgeManagerInstance().GetBadgeByIndex(i);
				g.SetScale(1.3f, 1.3f, badgePosition.mX, badgePosition.mY);
				if (!mMenu.mBadgeStatus[i])
				{
					Bej3Widget.DrawImageCentered(g, badgeByIndex.mSmallIconGray, 0, badgePosition.mX, badgePosition.mY);
					Bej3Widget.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RINGS, 0, badgePosition.mX, badgePosition.mY);
				}
			}
			g.mScaleX = mScaleX;
			g.mScaleY = mScaleY;
		}

		public void RenderNormalBadges(Graphics g)
		{
			g.SetColorizeImages(true);
			RenderBadges(g, false);
		}

		public Point GetAbsoluteBadgePosition(int badgeId, bool offsetY)
		{
			Point result = default(Point);
			int num = (GlobalMembers.gApp.mWidth - ConstantsWP.BADGE_MENU_BADGES_PADDING_X * 2) / (ConstantsWP.BADGE_MENU_BADGES_PER_ROW + 1);
			int num2 = ConstantsWP.BADGE_MENU_BADGES_PADDING_X + num;
			int num3 = badgeId % ConstantsWP.BADGES_FOR_PAGE;
			GetBadgePage(badgeId);
			int num4;
			if (Profile.IsEliteBadge(badgeId))
			{
				num4 = num3 % ConstantsWP.BADGE_MENU_BADGES_PER_ROW;
				num4 = num2 - ConstantsWP.BADGE_MENU_ELITE_BADGE_PADDING + num4 * (num + ConstantsWP.BADGE_MENU_ELITE_BADGE_PADDING);
				result.mY = ConstantsWP.BADGE_MENU_ELITE_BADGES_POS_Y;
			}
			else
			{
				int num5;
				if (num3 <= 2)
				{
					switch (num3)
					{
					case 0:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2 - ConstantsWP.BADGE_MENU_BADGES_POS_1;
						break;
					case 2:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2 + ConstantsWP.BADGE_MENU_BADGES_POS_1;
						break;
					default:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2;
						break;
					}
					num5 = 0;
				}
				else if (num3 <= 5)
				{
					switch (num3)
					{
					case 3:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2 - ConstantsWP.BADGE_MENU_BADGES_POS_2;
						break;
					case 5:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2 + ConstantsWP.BADGE_MENU_BADGES_POS_2;
						break;
					default:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2;
						break;
					}
					num5 = 1;
				}
				else
				{
					switch (num3)
					{
					case 6:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2 - ConstantsWP.BADGE_MENU_BADGES_POS_1;
						break;
					case 8:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2 + ConstantsWP.BADGE_MENU_BADGES_POS_1;
						break;
					default:
						num4 = ConstantsWP.PROFILEMENU_BADGE_SINGLE_SECTION_WIDTH / 2;
						break;
					}
					num5 = 2;
				}
				result.mY = ConstantsWP.BADGE_MENU_BADGES_POS_Y + num5 * ConstantsWP.BADGE_MENU_BADGES_POS_Y_DELTA;
			}
			result.mX = num4;
			if (offsetY)
			{
				result.mY += GetAbsPos().mY;
			}
			return result;
		}

		public override void TouchEnded(SexyAppBase.Touch touch)
		{
			base.TouchEnded(touch);
			if (mMenu.ContainerTouchEnded(touch))
			{
				return;
			}
			Tooltip tooltip = null;
			if (GlobalMembers.gApp.mTooltipManager.GetNOfTooltips() > 0)
			{
				tooltip = GlobalMembers.gApp.mTooltipManager.GetTooltip(0);
			}
			if (tooltip != null)
			{
				int num = tooltip.mOffsetPos.mX;
				int num2 = tooltip.mOffsetPos.mY;
				int num3 = tooltip.mWidth;
				int num4 = tooltip.mHeight;
				int num5 = touch.location.mX + mMenu.mScrollWidget.mX - mMenu.mScrollWidget.mWidth * mMenu.mCurrentPage;
				int num6 = touch.location.mY + mMenu.mScrollWidget.mY;
				if (num5 > num && num5 < num + num3 && num6 > num2 && num6 < num2 + num4)
				{
					GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
					return;
				}
			}
			GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
			int num7 = -1;
			for (int i = 0; i < 20; i++)
			{
				Point badgePosition = GetBadgePosition(i);
				if (Math.Abs(badgePosition.mX - touch.location.mX) < ConstantsWP.BADGE_MENU_TOOLTIP_TOLERANCE && Math.Abs(badgePosition.mY - touch.location.mY) < ConstantsWP.BADGE_MENU_TOOLTIP_TOLERANCE)
				{
					num7 = i;
					break;
				}
			}
			if (num7 == -1)
			{
				return;
			}
			Badge badge = mMenu.mBadgeManager.mBadgeClass[num7];
			Point badgePosition2 = GetBadgePosition(num7);
			badgePosition2.mY += GetAbsPos().mY;
			int badgePage = GetBadgePage(num7);
			int num8 = badgePosition2.mX - badgePage * ConstantsWP.BADGEMENU_CONTAINER_PAGE_WIDTH;
			int theArrowDir;
			if (num8 < GlobalMembers.gApp.mWidth / 2)
			{
				theArrowDir = 2;
				badgePosition2.mX += ConstantsWP.BADGE_MENU_TOOLTIP_OFFSET;
			}
			else if (num8 > GlobalMembers.gApp.mWidth / 2)
			{
				theArrowDir = 3;
				badgePosition2.mX -= ConstantsWP.BADGE_MENU_TOOLTIP_OFFSET;
			}
			else if (badgePosition2.mY > mHeight / 2)
			{
				theArrowDir = 1;
				badgePosition2.mY -= ConstantsWP.BADGE_MENU_TOOLTIP_OFFSET;
			}
			else
			{
				theArrowDir = 0;
				badgePosition2.mY += ConstantsWP.BADGE_MENU_TOOLTIP_OFFSET;
			}
			int num9 = mWidth / 3;
			int num10 = ConstantsWP.BADGE_MENU_TOOLTIP_WIDTH_WIDE;
			if (badgePosition2.mX - num10 / 2 < badgePage * num9 || badgePosition2.mX + num10 / 2 > badgePage * num9 + num9)
			{
				int num11 = badgePosition2.mX - badgePage * num9;
				if (num11 > num9 / 2)
				{
					num11 = num9 - num11;
				}
				num10 = num11 * 2;
			}
			badgePosition2.mX -= badgePage * ConstantsWP.BADGEMENU_CONTAINER_PAGE_WIDTH;
			GlobalMembers.gApp.mTooltipManager.RequestTooltip(this, badge.GetTooltipHeader(), badge.GetTooltipBody() + $" ({badge.mGPoints}G)", badgePosition2, num10, theArrowDir, 500, null, null, 0, -1);
		}

		public override void TouchBegan(SexyAppBase.Touch touch)
		{
			base.TouchBegan(touch);
			GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
		}

		public override void Draw(Graphics g)
		{
			RenderGrayBadges(g);
			RenderNormalBadges(g);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Show()
		{
			base.Show();
			mY = 0;
		}

		public static int GetBadgePage(int badgeId)
		{
			return badgeId / ConstantsWP.BADGES_FOR_PAGE;
		}

		public override void Hide()
		{
		}
	}
}
