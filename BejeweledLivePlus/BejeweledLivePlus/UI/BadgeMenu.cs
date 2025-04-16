using System.Collections.Generic;
using BejeweledLivePlus.Localization;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus.UI
{
	internal class BadgeMenu : Bej3Widget, Bej3ButtonListener, ButtonListener, Bej3ScrollWidgetListener, ScrollWidgetListener
	{
		public enum BADGEMENU_STATE
		{
			BADGEMENU_STATE_MENU,
			BADGEMENU_STATE_AWARDING,
			BADGEMENU_STATE_PROFILE_AWARDING
		}

		public enum BTN_BADGEMENU_ID
		{
			BTN_CLOSE_BADGES_ID,
			BTN_LEFT_ID,
			BTN_RIGHT_ID
		}

		public enum BADGE_MENU_PAGE
		{
			BADGE_MENU_PAGE_COUNT = 3
		}

		private PIEffect mBadgeEffect;

		private int mScaledBadgeLevel;

		private ResourceRef mBadgeIconRef = new ResourceRef();

		private ResourceRef mBadgeRingRef = new ResourceRef();

		private Image mBadgeIcon;

		private Image mBadgeRing;

		private CurvedVal mWidgetScale = new CurvedVal();

		private BadgeMenuContainer mContainer;

		private int mScrollingToPage;

		private CurvedVal mBadgeAnimPct = new CurvedVal();

		private CurvedVal mBadgeScale = new CurvedVal();

		private CurvedVal mBadgeAlpha = new CurvedVal();

		private CurvedVal mAwardShadowAlpha = new CurvedVal();

		private Label mHeadingLabel;

		private Bej3Button mCloseButton;

		private Bej3Button mSlideLeftButton;

		private Bej3Button mSlideRightButton;

		private Label mBottomMessageLabel;

		private Label mCumuBadgeScoreLabel;

		public Bej3ScrollWidget mScrollWidget;

		public int mCurrentPage;

		public BADGEMENU_STATE mBadgemenuState;

		public int mCurrentBadge;

		public List<int> mDeferredBadgeVector = new List<int>();

		public BadgeManager mBadgeManager;

		public int BeHigher = 60;

		public int[] mBadgeLevels = new int[20];

		public bool[] mBadgeStatus = new bool[20];

		private static Image[] aBadgeIds = new Image[20]
		{
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_LEVELORD,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_BEJEWELER,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_DIAMOND_MINE,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_DIAMOND_MINE,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_RELIC_HUNTER,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_RELIC_HUNTER,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_ELECTRIFIER,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_ELECTRIFIER,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_HIGH_VOLTAGE,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_HIGH_VOLTAGE,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_BUTTERFLY_MONARCH,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_BUTTERFLY_MONARCH,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_BUTTERFLY_BONANZA,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_BUTTERFLY_BONANZA,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_CHROMATIC,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_STELLAR,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_BLASTER,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_SUPERSTAR,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_CHAIN_REACTION,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_LUCKY_STREAK
		};

		private static Image[] aRingIds = new Image[5]
		{
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_BRONZE,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_SILVER,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_GOLD,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_PLATINUM,
			GlobalMembersResourcesWP.IMAGE_BADGES_BIG_ELITE
		};

		private void SetUpSlideButtons()
		{
			if (!IsInProfileMenu())
			{
				int pageHorizontal = mScrollWidget.GetPageHorizontal();
				bool flag = pageHorizontal > 0;
				mSlideLeftButton.SetVisible(flag);
				mSlideLeftButton.SetDisabled(!flag);
				flag = pageHorizontal < 2;
				mSlideRightButton.SetVisible(flag);
				mSlideRightButton.SetDisabled(!flag);
			}
		}

		public BadgeMenu(bool isInProfileMenu)
			: base(Menu_Type.MENU_BADGEMENU, true, Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
		{
			mCurrentPage = 0;
			mScrollingToPage = 0;
			Resize(0, ConstantsWP.MENU_Y_POS_HIDDEN, ConstantsWP.BADGE_MENU_WIDTH, GlobalMembers.gApp.mHeight);
			mFinalY = 106;
			mBadgeManager = BadgeManager.GetBadgeManagerInstance();
			int num = 80;
			mHeadingLabel = new Label(GlobalMembersResources.FONT_HUGE);
			mHeadingLabel.Resize(ConstantsWP.BADGE_MENU_HEADING_X, ConstantsWP.DIALOG_HEADING_LABEL_Y, 0, 0);
			mHeadingLabel.SetText(GlobalMembers._ID("ACHIEVEMENTS", 4011));
			mHeadingLabel.SetMaximumWidth(ConstantsWP.DIALOG_HEADING_LABEL_MAX_WIDTH);
			mCumuBadgeScoreLabel = new Label(GlobalMembersResources.FONT_DIALOG);
			AddWidget(mCumuBadgeScoreLabel);
			mCumuBadgeScoreLabel.Resize(ConstantsWP.BADGE_MENU_HEADING_X, ConstantsWP.DIALOG_HEADING_LABEL_Y + 100, 0, 0);
			mCumuBadgeScoreLabel.SetText("20/200");
			mContainer = new BadgeMenuContainer(this);
			mScrollWidget = new Bej3ScrollWidget(this, false);
			mScrollWidget.Resize(ConstantsWP.BADGEMENU_CONTAINER_PADDING_X, ConstantsWP.BADGEMENU_CONTAINER_TOP, mWidth - ConstantsWP.BADGEMENU_CONTAINER_PADDING_X * 2, ConstantsWP.BADGEMENU_CONTAINER_HEIGHT + num);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.EnablePaging(true);
			mScrollWidget.SetScrollInsets(new Insets(0, 0, 0, 0));
			mScrollWidget.AddWidget(mContainer);
			mScrollWidget.SetPageHorizontal(0, false);
			AddWidget(mScrollWidget);
			mCloseButton = new Bej3Button(0, this, Bej3ButtonType.BUTTON_TYPE_LONG, true);
			mCloseButton.SetLabel(GlobalMembers._ID("CLOSE", 3487));
			Bej3Widget.CenterWidgetAt(mWidth / 2, ConstantsWP.MENU_BOTTOM_BUTTON_Y + num, mCloseButton, true, false);
			mCloseButton.mBtnNoDraw = true;
			AddWidget(mCloseButton);
			if (!IsInProfileMenu())
			{
				mSlideLeftButton = new Bej3Button(1, this, Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
				mSlideLeftButton.Resize(ConstantsWP.BADGEMENU_SLIDE_BUTTON_OFFSET_X, ConstantsWP.BADGEMENU_SLIDE_BUTTON_Y + BeHigher, 0, 0);
				AddWidget(mSlideLeftButton);
				mSlideRightButton = new Bej3Button(2, this, Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE);
				mSlideRightButton.Resize(0, BeHigher, 0, 0);
				AddWidget(mSlideRightButton);
				mBottomMessageLabel = null;
			}
			else
			{
				mSlideLeftButton = null;
				mSlideRightButton = null;
				mBottomMessageLabel = null;
			}
			mBottomMessageLabel = new Label(GlobalMembersResources.FONT_SUBHEADER);
			mBottomMessageLabel.SetTextBlock(new Rect(ConstantsWP.HIGHSCORES_MENU_BOTTOM_MESSAGE_X, ConstantsWP.BADGEMENU_SLIDE_BUTTON_Y + 35 + BeHigher, ConstantsWP.SLIDE_BUTTON_MESSAGE_WIDTH, 0), true);
			mBottomMessageLabel.SetTextBlockEnabled(true);
			mBottomMessageLabel.SetText(GlobalMembers._ID("Swipe for more achievments", 3552));
			mBottomMessageLabel.SetClippingEnabled(false);
			mBottomMessageLabel.SetLayerColor(1, Bej3Widget.COLOR_SUBHEADING_3_FILL);
			mBottomMessageLabel.SetLayerColor(0, Bej3Widget.COLOR_SUBHEADING_3_STROKE);
			AddWidget(mBottomMessageLabel);
			SetMode(BADGEMENU_STATE.BADGEMENU_STATE_MENU, null);
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			if (args.button == SystemButtons.Back && !IsInOutPosition())
			{
				args.processed = true;
				ButtonDepress(10001);
			}
		}

		public override void PlayMenuMusic()
		{
		}

		public override void Draw(Graphics g)
		{
			g.SetColor(Color.White);
			Bej3Widget.DrawDialogBox(g, mWidth);
			g.SetColorizeImages(true);
			if (mHeadingLabel != null && mHeadingLabel.mVisible)
			{
				g.Translate(mHeadingLabel.mX, mHeadingLabel.mY);
				mHeadingLabel.Draw(g);
				g.Translate(-mHeadingLabel.mX, -mHeadingLabel.mY);
			}
			if (mCloseButton != null && mCloseButton.mVisible)
			{
				mCloseButton.mBtnNoDraw = false;
				g.Translate(mCloseButton.mX, mCloseButton.mY);
				mCloseButton.Draw(g);
				g.Translate(-mCloseButton.mX, -mCloseButton.mY);
				mCloseButton.mBtnNoDraw = true;
			}
			Bej3Widget.DrawSwipeInlay(g, mScrollWidget.mY + GlobalMembers.S(10), mScrollWidget.mHeight - GlobalMembers.S(150), mWidth, true);
			DeferOverlay(0);
		}

		public override void DrawOverlay(Graphics g)
		{
			if ((mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING || mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING) && (double)mBadgeAlpha > 0.0 && mCurrentBadge < Common.size(mDeferredBadgeVector))
			{
				g.Translate(-mX, -mY);
				Graphics3D graphics3D = g.Get3D();
				graphics3D = null;
				g.SetColor(Color.White);
				mBadgeManager.SetBadge(mDeferredBadgeVector[mCurrentBadge]);
				Badge mBadge = mBadgeManager.mBadge;
				SexyTransform2D theTransform = new SexyTransform2D(true);
				graphics3D?.PopTransform(ref theTransform);
				Color color = g.GetColor();
				bool colorizeImages = g.GetColorizeImages();
				g.SetColorizeImages(true);
				g.SetColor(new Color(0, 0, 0, (int)((double)mBadgeAlpha * (double)mAwardShadowAlpha * (double)ConstantsWP.BADGEMENU_AWARDED_ALPHA)));
				g.FillRect(-(int)g.mTransX, -(int)g.mTransY, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
				int num = (mWidth - (int)((float)GlobalMembersResourcesWP.IMAGE_AWARD_GLOW.GetWidth() * ConstantsWP.BOARD_BADGE_AWARD_SCALE)) / 2;
				int num2 = (mHeight - (int)((float)GlobalMembersResourcesWP.IMAGE_AWARD_GLOW.GetHeight() * ConstantsWP.BOARD_BADGE_AWARD_SCALE)) / 2;
				g.SetScale(ConstantsWP.BOARD_BADGE_AWARD_SCALE, ConstantsWP.BOARD_BADGE_AWARD_SCALE, num, num2);
				int num3 = (int)((double)mBadgeAlpha * (double)mAwardShadowAlpha * (double)GlobalMembers.M(255));
				g.SetColor(new Color(0, 0, 0, (int)((float)num3 * GlobalMembers.M(1f))));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_AWARD_GLOW, num, num2);
				g.SetColor(new Color(num3 * (int)GlobalMembers.M(0.7f), num3 * (int)GlobalMembers.M(1f), num3 * (int)GlobalMembers.M(0.9f)));
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_AWARD_GLOW, num, num2);
				g.mScaleX = 1f;
				g.mScaleY = 1f;
				g.SetDrawMode(Graphics.DrawMode.Normal);
				string theString = string.Format(GlobalMembers._ID("{0}^FFFFFF^ \"{1}\" Badge", 71), mBadge.GetBadgeLevelName(), mBadge.GetTooltipHeader());
				string awardString = mBadge.GetAwardString();
				g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
				g.SetColor(new Color(255, 224, 0, (int)((double)mBadgeAlpha * (double)mAwardShadowAlpha * 255.0)));
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Bej3Widget.COLOR_SUBHEADING_6_FILL);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Bej3Widget.COLOR_SUBHEADING_6_STROKE);
				g.WriteString(GlobalMembers._ID("You have earned the", 3488), mWidth / 2, ConstantsWP.BADGEMENU_AWARDED_TOP_TEXT_Y);
				g.SetFont(GlobalMembersResources.FONT_DIALOG);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, Bej3Widget.COLOR_DIALOG_WHITE);
				int num4 = 0;
				g.SetColor(new Color(255, 255, 255, (int)((double)mBadgeAlpha * (double)mAwardShadowAlpha * 255.0)));
				g.WriteString(theString, mWidth / 2, num4 + ConstantsWP.BADGEMENU_AWARDED_TOP_TEXT_Y + GlobalMembersResources.FONT_SUBHEADER.GetHeight());
				if (Strings.LANG == "DE-DE")
				{
					g.WriteString("-Abzeichen", mWidth / 2, num4 + ConstantsWP.BADGEMENU_AWARDED_TOP_TEXT_Y + 2 * GlobalMembersResources.FONT_SUBHEADER.GetHeight() + 3);
				}
				g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
				g.SetColor(new Color(255, 224, 0, (int)((double)mBadgeAlpha * (double)mAwardShadowAlpha * 255.0)));
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Bej3Widget.COLOR_SUBHEADING_6_FILL);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Bej3Widget.COLOR_SUBHEADING_6_STROKE);
				g.WriteWordWrapped(new Rect(ConstantsWP.BADGEMENU_AWARDED_DESC_PAD_X, ConstantsWP.BADGEMENU_AWARDED_BTM_TEXT_Y, mWidth - ConstantsWP.BADGEMENU_AWARDED_DESC_PAD_X * 2, mHeight - ConstantsWP.BADGEMENU_AWARDED_BTM_TEXT_Y), awardString, -1, 0);
				g.SetColor(Color.White);
				float num5 = (float)((double)(1f - GlobalMembers.M(0.36f)) * (double)mBadgeScale + (double)GlobalMembers.M(0.36f));
				Transform transform = new Transform();
				transform.Scale(num5, num5);
				Point absoluteBadgePosition = mContainer.GetAbsoluteBadgePosition(mBadge.mIdx, true);
				int num6 = (int)((double)(mWidth / 2) * (double)mBadgeScale + (double)absoluteBadgePosition.mX * (1.0 - (double)mBadgeScale));
				int num7 = (int)((double)(mHeight / 2) * (double)mBadgeScale + (double)absoluteBadgePosition.mY * (1.0 - (double)mBadgeScale));
				int num8 = mScaledBadgeLevel;
				mBadgeIcon = aBadgeIds[mBadge.mIdx];
				mBadgeRing = aRingIds[num8];
				g.DrawImageTransform(mBadgeIcon, transform, num6, num7);
				g.DrawImageTransform(mBadgeRing, transform, num6, num7);
				g.SetColorizeImages(colorizeImages);
				g.SetColor(color);
				graphics3D?.PushTransform(theTransform);
			}
			if (mBadgeEffect != null)
			{
				mBadgeEffect.Draw(g);
			}
		}

		public override void Update()
		{
			base.Update();
			SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
			if (mBadgemenuState != BADGEMENU_STATE.BADGEMENU_STATE_AWARDING && mBadgemenuState != BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING)
			{
				return;
			}
			if ((double)mBadgeScale < (double)GlobalMembers.M(0.1f))
			{
				if (mBadgeEffect == null)
				{
					if (mCurrentBadge < Common.size(mDeferredBadgeVector))
					{
						InitBadgeEffect();
						mBadgeStatus[mDeferredBadgeVector[mCurrentBadge]] = mBadgeManager.mBadgeClass[mDeferredBadgeVector[mCurrentBadge]].mUnlocked;
					}
				}
				else if (mBadgeEffect.HasTimelineExpired())
				{
					mBadgeEffect = null;
					if (mCurrentBadge < Common.size(mDeferredBadgeVector))
					{
						DoNextBadge();
					}
				}
			}
			if (mBadgeEffect != null)
			{
				mBadgeEffect.Update();
			}
			if ((double)mWidgetScale == 0.0 && mBadgeAnimPct.IsDoingCurve() && (double)mBadgeScale < 1.0)
			{
				GlobalMembers.OutputDebugStrF("BadgeMenu: Begin dialog scale\n");
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_WIDGET_SCALE, mWidgetScale);
				if (mHeadingLabel != null)
				{
					mHeadingLabel.SetVisible(true);
				}
			}
			bool flag = mCurrentBadge < Common.size(mDeferredBadgeVector);
			if (mCloseButton != null)
			{
				mCloseButton.SetDisabled(flag);
			}
			if (!flag && mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING)
			{
				SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				if (mTopButton != null)
				{
					mTopButton.SetDisabled(false);
				}
				mBadgemenuState = BADGEMENU_STATE.BADGEMENU_STATE_MENU;
			}
			if ((double)mAwardShadowAlpha < 0.4000000059604645)
			{
				mAllowSlide = true;
			}
			mWidgetScale.IncInVal();
		}

		public override void ButtonMouseEnter(int theId)
		{
		}

		public override void ButtonDepress(int theId)
		{
			GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
			switch (theId)
			{
			case 10001:
				if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING)
				{
					GlobalMembers.gApp.GoBackToGame();
				}
				else
				{
					GlobalMembers.gApp.DoMainMenu();
				}
				Transition_SlideOut();
				break;
			case 0:
				if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING)
				{
					if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN)
					{
						GlobalMembers.gApp.GoBackToGame();
						Transition_SlideOut();
						break;
					}
					if (GlobalMembers.gApp.mBoard != null)
					{
						GlobalMembers.gApp.mBoard.SubmitHighscore();
					}
					GlobalMembers.gApp.DoGameDetailMenu(GlobalMembers.gApp.mCurrentGameMode, GameDetailMenu.GAMEDETAILMENU_STATE.STATE_POST_GAME);
					Transition_SlideOut();
					GlobalMembers.gApp.mMenus[6].AllowSlideIn(false, mTopButton);
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
				}
				else
				{
					GlobalMembers.gApp.DoMainMenu();
					Transition_SlideOut();
				}
				break;
			case 2:
				if (mScrollWidget.GetScrollMode() != 0)
				{
					mScrollingToPage = mScrollWidget.GetPageHorizontal() + 1;
					mScrollWidget.SetPageHorizontal(mScrollingToPage, true);
				}
				break;
			case 1:
				if (mScrollWidget.GetScrollMode() != 0)
				{
					mScrollingToPage = mScrollWidget.GetPageHorizontal() - 1;
					mScrollWidget.SetPageHorizontal(mScrollingToPage, true);
				}
				break;
			}
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			if (!IsInProfileMenu())
			{
				mSlideRightButton.LinkUpAssets();
				mSlideRightButton.Resize(mWidth - mSlideRightButton.mWidth - ConstantsWP.BADGEMENU_SLIDE_BUTTON_OFFSET_X, ConstantsWP.BADGEMENU_SLIDE_BUTTON_Y + BeHigher, 0, 0);
			}
			SetUpSlideButtons();
		}

		public override void Show()
		{
			if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING || mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING)
			{
				BejeweledLivePlusApp.LoadContent("AwardGlow");
			}
			BejeweledLivePlusApp.LoadContent("Badges");
			LinkUpAssets();
			mContainer.Show();
			base.Show();
			ResetFadedBack(true);
			SetVisible(false);
			mY = ConstantsWP.MENU_Y_POS_HIDDEN;
			AddWidget(GlobalMembers.gApp.mTooltipManager);
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
			if (!IsInProfileMenu())
			{
				mSlideLeftButton.EnableSlideGlow(true);
				mSlideRightButton.EnableSlideGlow(true);
			}
		}

		public override void Hide()
		{
			mContainer.Hide();
			base.Hide();
			RemoveWidget(GlobalMembers.gApp.mTooltipManager);
		}

		public override void HideCompleted()
		{
			if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING)
			{
				BejeweledLivePlusApp.UnloadContent("AwardGlow");
				UnloadBigBadge();
				SetMode(BADGEMENU_STATE.BADGEMENU_STATE_MENU, null);
			}
			BejeweledLivePlusApp.UnloadContent("Badges");
			base.HideCompleted();
		}

		public void DoNextBadge()
		{
			UnloadBigBadge();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_ANIM_PCT_2, mBadgeAnimPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_SCALE_2, mBadgeScale, mBadgeAnimPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_ALPHA_2, mBadgeAlpha, mBadgeAnimPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_AWARD_SHADOW_ALPHA_2, mAwardShadowAlpha, mBadgeAnimPct);
			mCurrentBadge++;
			if (mCurrentBadge < Common.size(mDeferredBadgeVector))
			{
				Badge badge = mBadgeManager.mBadgeClass[mDeferredBadgeVector[mCurrentBadge]];
				if (badge != null)
				{
					int badgePage = BadgeMenuContainer.GetBadgePage(mBadgeManager.mBadgeClass[mDeferredBadgeVector[mCurrentBadge]].mIdx);
					if (mCurrentPage != badgePage)
					{
						mScrollingToPage = badgePage;
						mScrollWidget.SetPageHorizontal(mScrollingToPage, true);
					}
				}
				GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BADGEAWARDED);
				mScaledBadgeLevel = (int)(mBadgeManager.mBadgeClass[mDeferredBadgeVector[mCurrentBadge]].mLevel - 1);
			}
			else
			{
				GlobalMembers.gApp.mProfile.mDeferredBadgeVector.Clear();
				mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_HORIZONTAL);
			}
		}

		public void UnloadBigBadge()
		{
			if (mBadgeIconRef.HasResource())
			{
				mBadgeIconRef.Release();
			}
			if (mBadgeRingRef.HasResource())
			{
				mBadgeRingRef.Release();
			}
		}

		public void InitBadgeEffect()
		{
			if (mBadgeEffect == null)
			{
				mBadgeEffect = GlobalMembersResourcesWP.PIEFFECT_BADGE_UPGRADE.Duplicate();
				Point absoluteBadgePosition = mContainer.GetAbsoluteBadgePosition(mDeferredBadgeVector[mCurrentBadge], true);
				mBadgeEffect.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
				mBadgeEffect.mDrawTransform.Translate(absoluteBadgePosition.mX, absoluteBadgePosition.mY);
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BADGEFALL);
			}
		}

		public void SetMode(BADGEMENU_STATE state, List<int> deferredBadgeVector)
		{
			mBadgemenuState = state;
			if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING || mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING)
			{
				Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
				mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_DISABLED);
				if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN)
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS);
				}
				else
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
				}
				Common.Resize(mDeferredBadgeVector, deferredBadgeVector.Count);
				for (int i = 0; i < deferredBadgeVector.Count; i++)
				{
					mDeferredBadgeVector[i] = deferredBadgeVector[i];
				}
				mWidgetScale.SetConstant(0.0);
				mCurrentBadge = -1;
				mBadgeEffect = null;
				mScaledBadgeLevel = 0;
				for (int j = 0; j < 20; j++)
				{
					mBadgeStatus[j] = mBadgeManager.mBadgeClass[j].mUnlocked;
				}
				mBadgeManager.SyncBadges();
				DoNextBadge();
				mCloseButton.SetDisabled(true);
				if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING)
				{
					mCloseButton.SetLabel(GlobalMembers._ID("CONTINUE", 3088));
				}
				else
				{
					mCloseButton.SetLabel(GlobalMembers._ID("BACK", 3089));
				}
				if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING)
				{
					mCloseButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_GREEN);
				}
				else
				{
					mCloseButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
				}
				mAllowSlide = false;
			}
			else if (state == BADGEMENU_STATE.BADGEMENU_STATE_MENU)
			{
				mDeferredBadgeVector.Clear();
				mBadgeManager.SyncBadges();
				for (int k = 0; k < 20; k++)
				{
					mBadgeStatus[k] = GlobalMembers.gApp.mProfile.mBadgeStatus[k];
				}
				mBadgeAnimPct = new CurvedVal();
				mBadgeScale = new CurvedVal();
				mBadgeAlpha = new CurvedVal();
				mAwardShadowAlpha = new CurvedVal();
				mWidgetScale = new CurvedVal();
				mWidgetScale.SetConstant(1.0);
				mCurrentBadge = -1;
				mBadgeEffect = null;
				mScaledBadgeLevel = 0;
				mCloseButton.SetDisabled(false);
				mCloseButton.SetVisible(true);
				mCloseButton.SetLabel(GlobalMembers._ID("BACK", 3090));
				mCloseButton.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
				if (mTopButton != null)
				{
					mTopButton.mBtnNoDraw = false;
				}
			}
			int num = 0;
			Badge[] mBadgeClass = mBadgeManager.mBadgeClass;
			foreach (Badge badge in mBadgeClass)
			{
				if (badge.mUnlocked)
				{
					num += badge.mGPoints;
				}
			}
			mCumuBadgeScoreLabel.SetText($"G : {num}/200");
		}

		public new int GetState()
		{
			return (int)mBadgemenuState;
		}

		public static Image GetSmallBadgeImage(int badgeId)
		{
			Image[] array = new Image[0];
			return array[badgeId];
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			bool flag = false;
			if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING || mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING)
			{
				flag = mAllowSlide;
			}
			base.AllowSlideIn(allow, previousTopButton);
			if (mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING || mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING)
			{
				mAllowSlide = flag;
				if (mInterfaceState == InterfaceState.INTERFACE_STATE_BADGEMENU)
				{
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_MENU);
					SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
				}
			}
			mContainer.AllowSlideIn(allow, previousTopButton);
		}

		public bool ContainerTouchEnded(SexyAppBase.Touch touch)
		{
			if ((mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_AWARDING || mBadgemenuState == BADGEMENU_STATE.BADGEMENU_STATE_PROFILE_AWARDING) && mCurrentBadge < Common.size(mDeferredBadgeVector))
			{
				if (mBadgeAnimPct.GetInVal() < 0.75 && (double)mBadgeScale == 1.0)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_ANIM_PCT_1, mBadgeAnimPct);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_SCALE_1, mBadgeScale, mBadgeAnimPct);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_ALPHA_1, mBadgeAlpha, mBadgeAnimPct);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBADGE_MENU_AWARD_SHADOW_ALPHA_1, mAwardShadowAlpha, mBadgeAnimPct);
					GlobalMembers.OutputDebugStrF("BadgeMenu: Mouse click - Spawn next badge\n");
				}
				else
				{
					if (mBadgeEffect != null)
					{
						mBadgeEffect.Clear();
						mBadgeEffect = null;
					}
					int num = mDeferredBadgeVector[mCurrentBadge];
					mBadgeStatus[num] = mBadgeManager.mBadgeClass[num].mUnlocked;
					DoNextBadge();
					GlobalMembers.OutputDebugStrF("BadgeMenu: Mouse click - Dispatch of current badge\n");
				}
				return true;
			}
			return false;
		}

		public virtual void PageChanged(Bej3ScrollWidget scrollWidget, int pageH, int pageV)
		{
			if (mCurrentPage != pageH)
			{
				GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
			}
			mCurrentPage = pageH;
			SetUpSlideButtons();
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
			int pageHorizontal = scrollWidget.GetPageHorizontal();
			if (mCurrentPage != pageHorizontal)
			{
				mCurrentPage = pageHorizontal;
				LinkUpAssets();
			}
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public bool IsInProfileMenu()
		{
			return false;
		}
	}
}
