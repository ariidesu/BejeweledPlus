using System;
using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3Button : DialogButton
	{
		public enum BUTTON_OVERLAY_TYPE
		{
			BUTTON_OVERLAY_NONE,
			BUTTON_OVERLAY_DIAMOND_MINE,
			BUTTON_OVERLAY_ICE_STORM
		}

		protected BUTTON_OVERLAY_TYPE mOverlayType;

		public Bej3ButtonType mType;

		private Image mTypeImage;

		private Rect mIconSrcRect = default(Rect);

		private Point mIconOffset = default(Point);

		private Rect mButtonSrcRect = default(Rect);

		private float mTypeImageRotation;

		private float mTargetTypeImageRotation;

		private int mImageId;

		private bool mSlideGlowEnabled;

		private bool mBorderGlowEnabled;

		private static bool mSlideGlowBrightening = true;

		private static float mSlideGlow = 0f;

		private static int mSlideGlowTimer = 0;

		private static float mTopButtonGlow = 0f;

		private static int mTopButtonGlowGoingUp = 1;

		public Rect mInsideImageRect = default(Rect);

		public bool mSizeToContent;

		public float mAlpha;

		public bool mClippingEnabled;

		public float mZenSize;

		public bool mPlayPressSound;

		private int width;

		private string s = GlobalMembers._ID("OK", 3218);

		private int xg;

		private int yg;

		public int iconX;

		public int iconY;

		protected bool mIsHighLighted;

		private static bool topButtonAnimating = false;

		private static int numberOfFlashes = -1;

		private bool CalcButtonWidth(string text, int newWidth)
		{
			if (mFont == null || string.IsNullOrEmpty(text))
			{
				return false;
			}
			newWidth = Math.Max(mFont.StringWidth(text) + ConstantsWP.BEJ3BUTTON_AUTOSCALE_TEXT_WIDTH_OFFSET, ConstantsWP.BEJ3BUTTON_AUTOSCALE_MIN_WIDTH);
			return true;
		}

		protected bool IsTopButton()
		{
			if (mType >= Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
			{
				return mType <= Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS;
			}
			return false;
		}

		protected void DrawTopButton(Graphics g)
		{
			Color color = g.mPushedColorVector[g.mPushedColorVector.Count - 1];
			g.PopColorMult();
			g.ClearClipRect();
			g.SetColor(Color.White);
			if (mDisabled && mDisabledRect.mWidth > 0 && mDisabledRect.mHeight > 0)
			{
				g.DrawImage(mComponentImage, mInsideImageRect, mDisabledRect);
			}
			else if (IsButtonDown())
			{
				g.DrawImage(mComponentImage, mInsideImageRect, mDownRect);
			}
			else if (mOverAlpha > 0.0)
			{
				if (mOverAlpha < 1.0)
				{
					g.DrawImage(mComponentImage, mInsideImageRect, mNormalRect);
				}
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, (int)(mOverAlpha * 255.0)));
				g.DrawImage(mComponentImage, mInsideImageRect, mOverRect);
				g.SetColorizeImages(false);
			}
			else if (mIsOver)
			{
				g.DrawImage(mComponentImage, mInsideImageRect, mOverRect);
			}
			else
			{
				g.DrawImage(mComponentImage, mInsideImageRect, mNormalRect);
			}
			if (mType != Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
			{
				g.SetColorizeImages(true);
				int num = 255;
				if (mType == Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
				{
					num = 255;
				}
				else if (mType == Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS)
				{
					num = 255;
				}
				g.SetColor(new Color(255, 255, 255, (int)((float)num * mTopButtonGlow)));
				g.DrawImage(mComponentImage, mInsideImageRect, mDownRect);
			}
			g.SetColor(color);
			g.PushColorMult();
		}

		protected void DrawZenSlideButton(Graphics g)
		{
			if (!mBtnNoDraw)
			{
				g.SetColorizeImages(true);
				g.SetColor(new Color((int)(255f * mAlpha), (int)(255f * mAlpha), (int)(255f * mAlpha), (int)(255f * mAlpha)));
				g.SetScale(mZenSize, mZenSize, mWidth / 2, mHeight / 2);
				g.DrawImage(mButtonImage, mWidth / 2 - mButtonImage.mWidth / 2, mHeight / 2 - mButtonImage.mHeight / 2);
			}
		}

		protected void DrawBackButton(Graphics g)
		{
			Image iMAGE_DASHBOARD_MENU_UP = GlobalMembersResourcesWP.IMAGE_DASHBOARD_MENU_UP;
			int theY = mHeight / 2 - iMAGE_DASHBOARD_MENU_UP.GetCelHeight() / 2;
			g.DrawImage(iMAGE_DASHBOARD_MENU_UP, mWidth / 2 - iMAGE_DASHBOARD_MENU_UP.GetCelWidth() / 2, theY);
		}

		protected void DrawHintOkButton(Graphics g)
		{
			Image iMAGE_DIALOG_BUTTON_SMALL_BLUE = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_SMALL_BLUE;
			if (mIsDown && mIsOver)
			{
				int theX = mWidth / 2 - mDownRect.mWidth / 2;
				int theY = mHeight / 2 - mDownRect.mHeight / 2;
				g.DrawImage(iMAGE_DIALOG_BUTTON_SMALL_BLUE, theX, theY, mDownRect);
			}
			else
			{
				int theX = mWidth / 2 - mNormalRect.mWidth / 2;
				int theY = mHeight / 2 - mNormalRect.mHeight / 2;
				g.DrawImage(iMAGE_DIALOG_BUTTON_SMALL_BLUE, theX, theY, mNormalRect);
			}
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			g.SetColorizeImages(true);
			g.SetColor(Color.White);
			s = GlobalMembers._ID("OK", 3218);
			width = g.StringWidth(s);
			g.DrawString(s, mWidth / 2 - width / 2 + ConstantsWP.BEJ3BUTTON_HINT_OK_OFFSET_X, mHeight / 2 + ConstantsWP.BEJ3BUTTON_HINT_OK_OFFSET_Y);
		}

		protected void DrawCameraButton(Graphics g)
		{
			int num = 0;
			int num2 = 0;
			Image iMAGE_DIALOG_BUTTON_SMALL_BLUE = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_SMALL_BLUE;
			if (mIsDown && mIsOver)
			{
				num = mWidth / 2 - mDownRect.mWidth / 2;
				num2 = mHeight / 2 - mDownRect.mHeight / 2;
				g.DrawImage(iMAGE_DIALOG_BUTTON_SMALL_BLUE, num, num2, mDownRect);
				num += (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_REPLAY_ID) - GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_BUTTON_SMALL_BLUE_ID));
				num2 += (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_REPLAY_ID) - GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_BUTTON_SMALL_BLUE_ID));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY, num, num2, GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY.GetCelRect(0));
			}
			else
			{
				num = mWidth / 2 - mNormalRect.mWidth / 2;
				num2 = mHeight / 2 - mNormalRect.mHeight / 2;
				g.DrawImage(iMAGE_DIALOG_BUTTON_SMALL_BLUE, num, num2, mNormalRect);
				num += (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_REPLAY_ID) - GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_BUTTON_SMALL_BLUE_ID));
				num2 += (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_REPLAY_ID) - GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_BUTTON_SMALL_BLUE_ID));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY, num, num2, GlobalMembersResourcesWP.IMAGE_DIALOG_REPLAY.GetCelRect(1));
			}
		}

		protected void DrawProfilePictureButton(Graphics g)
		{
			g.DrawImage(mButtonImage, mWidth / 2 - mButtonImage.GetCelWidth() / 2, mHeight / 2 - mButtonImage.GetCelHeight() / 2);
		}

		protected void DrawDropDownButton(Graphics g)
		{
			int num;
			int num2;
			if (mIsDown && mIsOver)
			{
				num = mWidth / 2 - mDownRect.mWidth / 2;
				num2 = mHeight / 2 - mDownRect.mHeight / 2;
				g.DrawImage(mComponentImage, num, num2, mDownRect);
			}
			else
			{
				num = mWidth / 2 - mNormalRect.mWidth / 2;
				num2 = mHeight / 2 - mNormalRect.mHeight / 2;
				g.DrawImage(mComponentImage, num, num2, mNormalRect);
			}
			num += ConstantsWP.BEJ3BUTTON_DROPDOWN_OFFSET_X;
			num2 += ConstantsWP.BEJ3BUTTON_DROPDOWN_OFFSET_Y;
			if (mTypeImageRotation == 0f)
			{
				g.DrawImage(mTypeImage, num, num2, mIconSrcRect);
			}
			else
			{
				g.DrawImageRotatedF(mTypeImage, num, num2, mTypeImageRotation, ConstantsWP.BEJ3BUTTON_DROPDOWN_ROT_CENTER_X, ConstantsWP.BEJ3BUTTON_DROPDOWN_ROT_CENTER_Y, mIconSrcRect);
			}
		}

		protected void DrawInfoButton(Graphics g)
		{
			int num;
			int num2;
			if (mIsDown && mIsOver)
			{
				num = mWidth / 2 - mDownRect.mWidth / 2;
				num2 = mHeight / 2 - mDownRect.mHeight / 2;
				g.DrawImage(mComponentImage, num, num2, mDownRect);
			}
			else
			{
				num = mWidth / 2 - mNormalRect.mWidth / 2;
				num2 = mHeight / 2 - mNormalRect.mHeight / 2;
				g.DrawImage(mComponentImage, num, num2, mNormalRect);
			}
			g.DrawImage(mTypeImage, num + mIconOffset.mX, num2 + mIconOffset.mY, mIconSrcRect);
		}

		protected void DrawSwipeButton(Graphics g)
		{
			if (!mBtnNoDraw)
			{
				int num = mWidth / 2 - mNormalRect.mWidth / 2;
				int num2 = mHeight / 2 - mNormalRect.mHeight / 2;
				int num3 = 0;
				if (mSlideGlowEnabled)
				{
					xg = num + (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1322) - GlobalMembersResourcesWP.ImgXOfs(1321));
					yg = num2 + (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1322) - GlobalMembersResourcesWP.ImgYOfs(1321));
					g.SetColorizeImages(true);
					g.SetColor(new Color(255, 255, 255, (int)(255f * mSlideGlow)));
					int num4 = ((mType != Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE) ? 1 : (-1));
					num3 = (int)mSlideGlow * num4 * ConstantsWP.BEJ3BUTTON_SLIDE_ARROW_DIST;
					g.mClipRect.mX += num3;
					g.Translate(num3, 0);
					g.DrawImageMirror(GlobalMembersResourcesWP.IMAGE_DIALOG_ARROW_SWIPEGLOW, xg, yg, mType == Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
					g.SetColor(new Color(255, 255, 255, 255));
				}
				if (mIsDown)
				{
					g.DrawImageMirror(mComponentImage, num, num2, mDownRect, mType == Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
				}
				else
				{
					g.DrawImageMirror(mComponentImage, num, num2, mNormalRect, mType == Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE);
				}
				g.Translate(-num3, 0);
			}
		}

		protected void DrawGameCenterButton(Graphics g)
		{
			Image iMAGE_DIALOG_BUTTON_GAMECENTER = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_GAMECENTER;
			int theId = 1328;
			int theId2 = 1329;
			iconX = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId) - GlobalMembersResourcesWP.ImgXOfs(theId2));
			iconY = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId) - GlobalMembersResourcesWP.ImgYOfs(theId2));
			if (mDisabled && mDisabledRect.mWidth > 0 && mDisabledRect.mHeight > 0)
			{
				g.DrawImage(mComponentImage, 0, 0, mDisabledRect);
				g.DrawImageCel(iMAGE_DIALOG_BUTTON_GAMECENTER, iconX, iconY, 0);
			}
			else if (IsButtonDown())
			{
				g.DrawImage(mComponentImage, 0, 0, mDownRect);
				g.DrawImageCel(iMAGE_DIALOG_BUTTON_GAMECENTER, iconX, iconY, 0);
			}
			else if (mOverAlpha > 0.0)
			{
				if (mOverAlpha < 1.0)
				{
					g.DrawImage(mComponentImage, 0, 0, mNormalRect);
					g.DrawImageCel(iMAGE_DIALOG_BUTTON_GAMECENTER, iconX, iconY, 0);
				}
				g.SetColor(new Color(255, 255, 255, (int)((double)(mAlpha / 255f) * mOverAlpha * 255.0)));
				g.DrawImage(mComponentImage, 0, 0, mOverRect);
				g.DrawImageCel(iMAGE_DIALOG_BUTTON_GAMECENTER, iconX, iconY, 0);
			}
			else if (mIsOver)
			{
				g.DrawImage(mComponentImage, 0, 0, mOverRect);
				g.DrawImageCel(iMAGE_DIALOG_BUTTON_GAMECENTER, iconX, iconY, 0);
			}
			else
			{
				g.DrawImage(mComponentImage, 0, 0, mNormalRect);
				g.DrawImageCel(iMAGE_DIALOG_BUTTON_GAMECENTER, iconX, iconY, 1);
			}
		}

		protected void DrawOtherButton(Graphics g)
		{
			if (mComponentImage == null)
			{
				base.Draw(g);
			}
			else
			{
				if (mBtnNoDraw)
				{
					return;
				}
				if (mType == Bej3ButtonType.BUTTON_TYPE_LONG || mType == Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE || mType == Bej3ButtonType.BUTTON_TYPE_LONG_GREEN)
				{
					switch (mOverlayType)
					{
					case BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_DIAMOND_MINE:
						g.DrawImageBox(new Rect(0, 0, mWidth, mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_DIAMOND_MINE);
						if (GlobalMembers.gApp.mBoard.WantWarningGlow())
						{
							g.PushState();
							g.SetColor(GlobalMembers.gApp.mBoard.GetWarningGlowColor());
							g.SetDrawMode(1);
							g.SetColorizeImages(true);
							g.DrawImageBox(new Rect(0, 0, mWidth, mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_DIAMOND_MINE);
							g.PopState();
						}
						break;
					case BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_ICE_STORM:
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_ICE_STORM, 0, 0);
						if (GlobalMembers.gApp.mBoard.WantWarningGlow())
						{
							g.PushState();
							g.SetColor(GlobalMembers.gApp.mBoard.GetWarningGlowColor());
							g.SetDrawMode(1);
							g.SetColorizeImages(true);
							g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_ICE_STORM, 0, 0);
							g.PopState();
						}
						break;
					default:
						g.DrawImageBox(new Rect(0, 0, mWidth, mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME);
						if (mBorderGlowEnabled && GlobalMembers.gApp.mBoard != null && GlobalMembers.gApp.mBoard.WantWarningGlow())
						{
							g.PushState();
							g.SetColor(GlobalMembers.gApp.mBoard.GetWarningGlowColor());
							g.SetDrawMode(Graphics.DrawMode.Additive);
							g.SetColorizeImages(true);
							g.DrawImageBox(new Rect(0, 0, mWidth, mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME);
							g.PopState();
						}
						break;
					}
				}
				bool flag = IsButtonDown();
				if (mNormalRect.mWidth == 0)
				{
					if (flag)
					{
						g.Translate(mTranslateX, mTranslateY);
					}
					g.DrawImageBox(mInsideImageRect, mComponentImage);
				}
				else
				{
					if (mDisabled && mDisabledRect.mWidth > 0 && mDisabledRect.mHeight > 0)
					{
						g.DrawImageBox(mDisabledRect, mInsideImageRect, mComponentImage);
					}
					else if (IsButtonDown())
					{
						g.DrawImageBox(mDownRect, mInsideImageRect, mComponentImage);
					}
					else if (mOverAlpha > 0.0)
					{
						if (mOverAlpha < 1.0)
						{
							g.DrawImageBox(mNormalRect, mInsideImageRect, mComponentImage);
						}
						g.SetColorizeImages(true);
						Color mColor = g.mColor;
						g.SetColor(new Color(255, 255, 255, (int)(mOverAlpha * 255.0)));
						g.DrawImageBox(mOverRect, mInsideImageRect, mComponentImage);
						g.SetColorizeImages(false);
						g.mColor = mColor;
					}
					else if (mIsOver)
					{
						g.DrawImageBox(mOverRect, mInsideImageRect, mComponentImage);
					}
					else
					{
						g.DrawImageBox(mNormalRect, mInsideImageRect, mComponentImage);
					}
					if (flag)
					{
						g.Translate(mTranslateX, mTranslateY);
					}
				}
				if (mIsHighLighted)
				{
					g.DrawImageBox(mDownRect, mInsideImageRect, mComponentImage);
				}
				if (mFont != null)
				{
					g.SetFont(mFont);
					int num = g.mColor.mAlpha;
					if (mIsOver)
					{
						g.SetColor(mColors[1]);
					}
					else
					{
						g.SetColor(mColors[0]);
					}
					g.mColor.mAlpha = num;
					int num2 = (mWidth - mFont.StringWidth(mLabel)) / 2;
					int num3 = (mHeight - mFont.GetHeight()) / 2 + mFont.GetAscent();
					Utils.SetFontLayerColor((ImageFont)mFont, 1, Color.White);
					if (mType == Bej3ButtonType.BUTTON_TYPE_LONG)
					{
						Utils.SetFontLayerColor((ImageFont)mFont, 1, Bej3Widget.COLOR_SUBHEADING_4_FILL);
						Utils.SetFontLayerColor((ImageFont)mFont, 0, Bej3Widget.COLOR_SUBHEADING_4_STROKE);
					}
					else if (mType == Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE)
					{
						Utils.SetFontLayerColor((ImageFont)mFont, 1, Bej3Widget.COLOR_SUBHEADING_5_FILL);
						Utils.SetFontLayerColor((ImageFont)mFont, 0, Bej3Widget.COLOR_SUBHEADING_5_STROKE);
					}
					else if (mType == Bej3ButtonType.BUTTON_TYPE_LONG_GREEN)
					{
						Utils.SetFontLayerColor((ImageFont)mFont, 1, Bej3Widget.COLOR_SUBHEADING_6_FILL);
						Utils.SetFontLayerColor((ImageFont)mFont, 0, Bej3Widget.COLOR_SUBHEADING_6_STROKE);
					}
					else if (mType == Bej3ButtonType.BUTTON_TYPE_CUSTOM)
					{
						Utils.SetFontLayerColor((ImageFont)mFont, 1, Bej3Widget.COLOR_SUBHEADING_4_FILL);
						Utils.SetFontLayerColor((ImageFont)mFont, 0, Bej3Widget.COLOR_SUBHEADING_4_STROKE);
					}
					else if (mType == Bej3ButtonType.BUTTON_TYPE_CUSTOM_INV)
					{
						Utils.SetFontLayerColor((ImageFont)mFont, 0, Bej3Widget.COLOR_SUBHEADING_4_FILL);
						Utils.SetFontLayerColor((ImageFont)mFont, 1, Bej3Widget.COLOR_SUBHEADING_4_STROKE);
					}
					if (mType == Bej3ButtonType.BUTTON_TYPE_LONG || mType == Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE || mType == Bej3ButtonType.BUTTON_TYPE_LONG_GREEN)
					{
						num3 += ConstantsWP.BEJ3BUTTON_TEXT_OFFSET_Y;
					}
					g.DrawString(mLabel, num2 + mTextOffsetX, num3 + mTextOffsetY);
				}
				if (mIconImage != null)
				{
					if (mIsOver)
					{
						g.SetColor(mColors[1]);
					}
					else
					{
						g.SetColor(mColors[0]);
					}
					int num4 = (mWidth - mIconImage.GetWidth()) / 2;
					int num5 = (mHeight - mIconImage.GetHeight()) / 2;
					g.DrawImage(mIconImage, num4 + mTextOffsetX, num5 + mTextOffsetY);
				}
				if (flag)
				{
					g.Translate(-mTranslateX, -mTranslateY);
				}
			}
		}

		public void HighLighted(bool enable)
		{
			mIsHighLighted = enable;
		}

		public Bej3Button(int theId, Bej3ButtonListener theListener)
			: base(null, theId, theListener)
		{
			initButton(theId, theListener, Bej3ButtonType.BUTTON_TYPE_CUSTOM, false);
		}

		public Bej3Button(int theId, Bej3ButtonListener theListener, Bej3ButtonType theType)
			: base(null, theId, theListener)
		{
			initButton(theId, theListener, theType, false);
		}

		public Bej3Button(int theId, Bej3ButtonListener theListener, Bej3ButtonType theType, bool sizeToContent)
			: base(null, theId, theListener)
		{
			initButton(theId, theListener, theType, sizeToContent);
		}

		public void initButton(int theId, Bej3ButtonListener theListener, Bej3ButtonType theType, bool sizeToContent)
		{
			mTargetTypeImageRotation = 0f;
			mTypeImageRotation = 0f;
			mClippingEnabled = true;
			mImageId = -1;
			mZenSize = 1f;
			mSizeToContent = sizeToContent;
			mSlideGlowEnabled = true;
			mOverlayType = BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_NONE;
			mBorderGlowEnabled = false;
			mType = theType;
			mPlayPressSound = true;
			SetType(theType);
			int theWidth = 0;
			if (mType == Bej3ButtonType.BUTTON_TYPE_LONG || mType == Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE || mType == Bej3ButtonType.BUTTON_TYPE_LONG_GREEN)
			{
				theWidth = ConstantsWP.BEJ3BUTTON_AUTOSCALE_DEFAULT_WIDTH;
			}
			Resize(0, 0, theWidth, 0);
			mTypeImageRotation = mTargetTypeImageRotation;
		}

		public override void MouseEnter()
		{
			base.MouseEnter();
			LinkUpAssets();
		}

		public override void MouseLeave()
		{
			base.MouseLeave();
			LinkUpAssets();
		}

		public override void MouseMove(int theX, int theY)
		{
			base.MouseMove(theX, theY);
			LinkUpAssets();
		}

		public override void MouseDown(int theX, int theY, int theBtnNum, int theClickCount)
		{
			base.MouseDown(theX, theY, theBtnNum, theClickCount);
			mIsDown = true;
			mIsOver = true;
			if (mPlayPressSound)
			{
				if (IsTopButton() && GlobalMembers.gApp.GetDialog(18) == null)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_PRESS);
				}
				else
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_PRESS);
				}
			}
			LinkUpAssets();
		}

		public override void MouseUp(int theX, int theY, int theBtnNum, int theClickCount)
		{
			Bej3ButtonListener bej3ButtonListener = mButtonListener as Bej3ButtonListener;
			if (bej3ButtonListener != null && bej3ButtonListener.ButtonsEnabled())
			{
				if (mPlayPressSound)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_MOUSEOVER);
				}
				base.MouseUp(theX, theY, theBtnNum, theClickCount);
			}
			mIsDown = false;
			mIsOver = false;
			LinkUpAssets();
		}

		public override void TouchesCanceled()
		{
			mIsDown = false;
			mIsOver = false;
			LinkUpAssets();
		}

		public override void Update()
		{
			if (mTargetTypeImageRotation != mTypeImageRotation)
			{
				float num = mTargetTypeImageRotation - mTypeImageRotation;
				mTypeImageRotation += num * ConstantsWP.BEJ3BUTTON_ROTATION_SPEED;
				if (Math.Abs(mTypeImageRotation - mTargetTypeImageRotation) < 0.01f)
				{
					mTypeImageRotation = mTargetTypeImageRotation;
				}
			}
			base.Update();
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
		}

		public override void Draw(Graphics g)
		{
			if (!mClippingEnabled)
			{
				g.ClearClipRect();
			}
			g.SetColorizeImages(true);
			switch (mType)
			{
			case Bej3ButtonType.BUTTON_TYPE_PROFILE_PICTURE:
				DrawProfilePictureButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_ZEN_SLIDE:
				DrawZenSlideButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_BACK:
				DrawBackButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_HINT_CAMERA:
				DrawCameraButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_HINT_OK:
				DrawHintOkButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_DROPDOWN_UP:
			case Bej3ButtonType.BUTTON_TYPE_DROPDOWN_DOWN:
			case Bej3ButtonType.BUTTON_TYPE_DROPDOWN_RIGHT:
				DrawDropDownButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_INFO:
				DrawInfoButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE:
			case Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE:
				DrawSwipeButton(g);
				break;
			case Bej3ButtonType.TOP_BUTTON_TYPE_MENU:
			case Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED:
			case Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS:
				DrawTopButton(g);
				break;
			case Bej3ButtonType.BUTTON_TYPE_GAMECENTER:
				DrawGameCenterButton(g);
				break;
			default:
				DrawOtherButton(g);
				break;
			}
		}

		public void SetBorderGlow(bool Value)
		{
			mBorderGlowEnabled = Value;
		}

		public override void SetDisabled(bool isDisabled)
		{
			if (isDisabled != mDisabled)
			{
				bool flag = mDisabled;
				if (mType == Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED)
				{
					isDisabled = true;
				}
				base.SetDisabled(isDisabled);
				if (flag != isDisabled)
				{
					LinkUpAssets();
				}
			}
		}

		public new Bej3ButtonType GetType()
		{
			return mType;
		}

		public void SetType(Bej3ButtonType theType)
		{
			mType = theType;
			if (mFont == null)
			{
				SetFont(GlobalMembersResources.FONT_SUBHEADER);
			}
			LinkUpAssets();
		}

		public void SetupCustomButton(Image image, int x, int y)
		{
			SetupCustomButton(image, x, y, -1, -1);
		}

		public void SetupCustomButton(Image image, int x, int y, int width)
		{
			SetupCustomButton(image, x, y, width, -1);
		}

		public void SetupCustomButton(Image image, int x, int y, int width, int height)
		{
			mComponentImage = image;
			if (mComponentImage != null)
			{
				int theCel = 1;
				if (mComponentImage.GetCelCount() == 0)
				{
					theCel = 0;
				}
				mNormalRect = mComponentImage.GetCelRect(0);
				mOverRect = mComponentImage.GetCelRect(theCel);
				mDownRect = mComponentImage.GetCelRect(theCel);
				int theWidth = ((width >= 0) ? width : mComponentImage.GetCelWidth());
				int theHeight = ((height >= 0) ? height : mComponentImage.GetCelHeight());
				Resize(x, y, theWidth, theHeight);
			}
		}

		public virtual void LinkUpAssets()
		{
			mTargetTypeImageRotation = 0f;
			mTypeImage = null;
			if (IsTopButton())
			{
				mInsideImageRect = new Rect(0, 0, mWidth, mHeight);
			}
			switch (mType)
			{
			case Bej3ButtonType.BUTTON_TYPE_LONG_GREEN:
				mTypeImage = null;
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE;
				if (mComponentImage != null)
				{
					mNormalRect = mComponentImage.GetCelRect(3);
					mOverRect = mComponentImage.GetCelRect(2);
					mDownRect = mComponentImage.GetCelRect(2);
				}
				if (mSizeToContent)
				{
					CalcButtonWidth(mLabel, mWidth);
				}
				break;
			case Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE:
				mTypeImage = null;
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE;
				if (mComponentImage != null)
				{
					mNormalRect = mComponentImage.GetCelRect(1);
					mOverRect = mComponentImage.GetCelRect(0);
					mDownRect = mComponentImage.GetCelRect(0);
				}
				if (mSizeToContent)
				{
					CalcButtonWidth(mLabel, mWidth);
				}
				break;
			case Bej3ButtonType.BUTTON_TYPE_LONG:
				mTypeImage = null;
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE;
				if (mComponentImage != null)
				{
					mNormalRect = mComponentImage.GetCelRect(5);
					mOverRect = mComponentImage.GetCelRect(4);
					mDownRect = mComponentImage.GetCelRect(4);
				}
				if (mSizeToContent)
				{
					CalcButtonWidth(mLabel, mWidth);
				}
				break;
			case Bej3ButtonType.BUTTON_TYPE_HINT_CAMERA:
			{
				Image iMAGE_DIALOG_BUTTON_SMALL_BLUE = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_SMALL_BLUE;
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_SMALL_BLUE;
				if (iMAGE_DIALOG_BUTTON_SMALL_BLUE != null)
				{
					mNormalRect = iMAGE_DIALOG_BUTTON_SMALL_BLUE.GetCelRect(1);
					mDownRect = iMAGE_DIALOG_BUTTON_SMALL_BLUE.GetCelRect(0);
					mOverRect = iMAGE_DIALOG_BUTTON_SMALL_BLUE.GetCelRect(0);
				}
				break;
			}
			case Bej3ButtonType.BUTTON_TYPE_HINT_OK:
			{
				Image iMAGE_DIALOG_BUTTON_SMALL_BLUE2 = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_SMALL_BLUE;
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_SMALL_BLUE;
				if (iMAGE_DIALOG_BUTTON_SMALL_BLUE2 != null)
				{
					mDownRect = iMAGE_DIALOG_BUTTON_SMALL_BLUE2.GetCelRect(0);
					mOverRect = iMAGE_DIALOG_BUTTON_SMALL_BLUE2.GetCelRect(0);
					mNormalRect = iMAGE_DIALOG_BUTTON_SMALL_BLUE2.GetCelRect(1);
				}
				break;
			}
			case Bej3ButtonType.BUTTON_TYPE_GAMECENTER:
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_GAMECENTER_BG;
				if (mComponentImage != null)
				{
					mDownRect = (mOverRect = (mNormalRect = mComponentImage.GetCelRect(0)));
				}
				break;
			case Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE:
			case Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE:
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DIALOG_ARROW_SWIPE;
				if (mComponentImage != null)
				{
					mNormalRect = mComponentImage.GetCelRect(1);
					mOverRect = mComponentImage.GetCelRect(0);
					mDownRect = mComponentImage.GetCelRect(0);
				}
				break;
			case Bej3ButtonType.BUTTON_TYPE_ZEN_SLIDE:
				mButtonImage = GlobalMembersResourcesWP.GetImageById(mImageId);
				if (mButtonImage != null)
				{
					Resize(mX, mY, mButtonImage.mWidth, mButtonImage.mHeight);
				}
				break;
			case Bej3ButtonType.TOP_BUTTON_TYPE_MENU:
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DASHBOARD_MENU_UP;
				mNormalRect = mComponentImage.GetCelRect(0);
				mOverRect = (mDownRect = mComponentImage.GetCelRect(1));
				break;
			case Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED:
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DASHBOARD_CLOSED_BUTTON;
				mNormalRect = (mOverRect = (mDownRect = (mInsideImageRect = mComponentImage.GetCelRect(0))));
				mDisabled = true;
				break;
			case Bej3ButtonType.TOP_BUTTON_TYPE_DISMISS:
				mComponentImage = GlobalMembersResourcesWP.IMAGE_DASHBOARD_MENU_DOWN;
				mNormalRect = mComponentImage.GetCelRect(0);
				mOverRect = (mDownRect = mComponentImage.GetCelRect(1));
				break;
			default:
				mTypeImage = null;
				break;
			}
			if (mTypeImage != null)
			{
				mIconSrcRect = (mDisabled ? mTypeImage.GetCelRect(1) : mTypeImage.GetCelRect(0));
			}
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			if (mType == Bej3ButtonType.BUTTON_TYPE_BACK)
			{
				Image iMAGE_DASHBOARD_MENU_UP = GlobalMembersResourcesWP.IMAGE_DASHBOARD_MENU_UP;
				if (iMAGE_DASHBOARD_MENU_UP == null)
				{
					return;
				}
				theWidth = iMAGE_DASHBOARD_MENU_UP.GetCelWidth();
				theHeight = iMAGE_DASHBOARD_MENU_UP.GetCelHeight();
			}
			else if (mType == Bej3ButtonType.BUTTON_TYPE_LEFT_SWIPE || mType == Bej3ButtonType.BUTTON_TYPE_RIGHT_SWIPE)
			{
				Image iMAGE_DIALOG_ARROW_SWIPEGLOW = GlobalMembersResourcesWP.IMAGE_DIALOG_ARROW_SWIPEGLOW;
				theWidth = iMAGE_DIALOG_ARROW_SWIPEGLOW.GetCelWidth() * 2;
				theHeight = iMAGE_DIALOG_ARROW_SWIPEGLOW.GetCelHeight();
			}
			else if (mType == Bej3ButtonType.BUTTON_TYPE_DROPDOWN_DOWN || mType == Bej3ButtonType.BUTTON_TYPE_DROPDOWN_RIGHT || mType == Bej3ButtonType.BUTTON_TYPE_DROPDOWN_UP || mType == Bej3ButtonType.BUTTON_TYPE_INFO)
			{
				theWidth = (theHeight = (int)ConstantsWP.BEJ3BUTTON_DROPDOWN_SIZE);
			}
			else if (mType == Bej3ButtonType.BUTTON_TYPE_HINT_OK || mType == Bej3ButtonType.BUTTON_TYPE_HINT_CAMERA)
			{
				Image iMAGE_DIALOG_BUTTON_SMALL_BLUE = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_SMALL_BLUE;
				theWidth = iMAGE_DIALOG_BUTTON_SMALL_BLUE.GetCelWidth();
				theHeight = iMAGE_DIALOG_BUTTON_SMALL_BLUE.GetCelHeight();
			}
			else if (mType == Bej3ButtonType.BUTTON_TYPE_ZEN_SLIDE)
			{
				if (mButtonImage != null)
				{
					theWidth = mButtonImage.mWidth;
					theHeight = mButtonImage.mHeight;
				}
			}
			else if (mType == Bej3ButtonType.BUTTON_TYPE_GAMECENTER)
			{
				Image iMAGE_DIALOG_BUTTON_GAMECENTER_BG = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_GAMECENTER_BG;
				theWidth = iMAGE_DIALOG_BUTTON_GAMECENTER_BG.GetCelWidth();
				theHeight = iMAGE_DIALOG_BUTTON_GAMECENTER_BG.GetCelHeight();
			}
			else if (mComponentImage != null)
			{
				if (mType == Bej3ButtonType.BUTTON_TYPE_LONG || mType == Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE || mType == Bej3ButtonType.BUTTON_TYPE_LONG_GREEN)
				{
					switch (mOverlayType)
					{
					case BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_DIAMOND_MINE:
						theHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_DIAMOND_MINE.GetCelHeight();
						break;
					case BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_ICE_STORM:
						theHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_ICE_STORM.GetCelHeight();
						break;
					default:
						theHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME.GetCelHeight();
						break;
					}
				}
				else
				{
					theHeight = Math.Max(theHeight, mComponentImage.GetCelHeight());
				}
			}
			base.Resize(theX, theY, theWidth, theHeight);
			SetOverlayType(mOverlayType);
		}

		public void SetOverlayType(BUTTON_OVERLAY_TYPE type)
		{
			mOverlayType = type;
			mInsideImageRect = new Rect(0, 0, mWidth, mHeight);
			if (mType == Bej3ButtonType.BUTTON_TYPE_LONG || mType == Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE || mType == Bej3ButtonType.BUTTON_TYPE_LONG_GREEN)
			{
				switch (mOverlayType)
				{
				case BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_DIAMOND_MINE:
				{
					mHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_DIAMOND_MINE.GetCelHeight();
					int num5 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1330) - GlobalMembersResourcesWP.ImgXOfs(1326));
					int num6 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1330) - GlobalMembersResourcesWP.ImgYOfs(1326));
					mInsideImageRect.mX += num5;
					mInsideImageRect.mY += num6;
					mInsideImageRect.mWidth -= GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_DIAMOND_MINE.GetCelWidth() - GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE.GetCelWidth();
					mInsideImageRect.mHeight -= GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_DIAMOND_MINE.GetCelHeight() - GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE.GetCelHeight();
					break;
				}
				case BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_ICE_STORM:
				{
					mHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_ICE_STORM.GetCelHeight();
					int num3 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1330) - GlobalMembersResourcesWP.ImgXOfs(1327));
					int num4 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1330) - GlobalMembersResourcesWP.ImgYOfs(1327));
					mInsideImageRect.mX += num3;
					mInsideImageRect.mY += num4;
					mInsideImageRect.mWidth -= GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_ICE_STORM.GetCelWidth() - GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE.GetCelWidth();
					mInsideImageRect.mHeight -= GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME_ICE_STORM.GetCelHeight() - GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE.GetCelHeight();
					mTextOffsetY = ConstantsWP.BEJ3BUTTON_ICESTORM_TEXT_OFFSET_Y;
					break;
				}
				default:
				{
					mHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME.GetCelHeight();
					int num = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1330) - GlobalMembersResourcesWP.ImgXOfs(1325)) + ConstantsWP.BEJ3BUTTON_INSIDE_RECT_OFFSET;
					int num2 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1330) - GlobalMembersResourcesWP.ImgYOfs(1325)) + ConstantsWP.BEJ3BUTTON_INSIDE_RECT_OFFSET;
					mInsideImageRect.mX += num;
					mInsideImageRect.mY += num2;
					mInsideImageRect.mWidth -= GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME.GetCelWidth() - GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE.GetCelWidth();
					mInsideImageRect.mHeight -= GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_FRAME.GetCelHeight() - GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE.GetCelHeight();
					break;
				}
				}
			}
		}

		public void SetLabel(string theLabel)
		{
			mLabel = theLabel;
			LinkUpAssets();
		}

		public void SetImageId(int theId)
		{
			mImageId = theId;
			LinkUpAssets();
		}

		public void EnableSlideGlow(bool enabled)
		{
			mSlideGlowEnabled = enabled;
		}

		public int GetButtonWidth()
		{
			if (mFont != null && !string.IsNullOrEmpty(mLabel))
			{
				return Math.Max(mFont.StringWidth(mLabel) + ConstantsWP.BEJ3BUTTON_AUTOSCALE_TEXT_WIDTH_OFFSET, ConstantsWP.BEJ3BUTTON_AUTOSCALE_MIN_WIDTH);
			}
			return 0;
		}

		public static void UpdateStatics()
		{
			InterfaceState mInterfaceState = GlobalMembers.gApp.mInterfaceState;
			if ((BejeweledLivePlusApp.mIdleTicksForButton > 500 && mInterfaceState != InterfaceState.INTERFACE_STATE_INGAME) || topButtonAnimating)
			{
				topButtonAnimating = true;
				mTopButtonGlow += 0.01f * (float)mTopButtonGlowGoingUp;
				if (mTopButtonGlow >= 0.99f || mTopButtonGlow < 0.01f)
				{
					mTopButtonGlowGoingUp = -mTopButtonGlowGoingUp;
				}
				if (mTopButtonGlowGoingUp != 0 && mTopButtonGlow <= 0.01f)
				{
					numberOfFlashes++;
					if (numberOfFlashes >= 2)
					{
						numberOfFlashes = 0;
						topButtonAnimating = false;
						BejeweledLivePlusApp.mIdleTicksForButton = 0;
					}
				}
			}
			if (mSlideGlowBrightening)
			{
				mSlideGlow += 0.02f;
				if (mSlideGlow >= 1f)
				{
					mSlideGlow = 1f;
					mSlideGlowBrightening = false;
				}
			}
			else
			{
				mSlideGlow -= 0.02f;
				if (mSlideGlow <= 0f)
				{
					mSlideGlow = 0f;
					mSlideGlowBrightening = true;
				}
			}
		}
	}
}
