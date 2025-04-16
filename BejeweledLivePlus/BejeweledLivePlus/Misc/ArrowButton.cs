using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Misc
{
	internal class ArrowButton : CrystalBall
	{
		public bool IsLeft;

		private Image[] Imag = new Image[11]
		{
			GlobalMembersResourcesWP.IMAGE_ARROW_01,
			GlobalMembersResourcesWP.IMAGE_ARROW_02,
			GlobalMembersResourcesWP.IMAGE_ARROW_03,
			GlobalMembersResourcesWP.IMAGE_ARROW_04,
			GlobalMembersResourcesWP.IMAGE_ARROW_05,
			GlobalMembersResourcesWP.IMAGE_ARROW_06,
			GlobalMembersResourcesWP.IMAGE_ARROW_07,
			GlobalMembersResourcesWP.IMAGE_ARROW_08,
			GlobalMembersResourcesWP.IMAGE_ARROW_09,
			GlobalMembersResourcesWP.IMAGE_ARROW_10,
			GlobalMembersResourcesWP.IMAGE_ARROW_01
		};

		public ArrowButton(string theLabel1, string theLabel2, string theLabel3, int theId, ButtonListener theListener, Color fontColour, float scale)
			: base(theLabel1, theLabel2, theLabel3, theId, theListener, fontColour, scale)
		{
			IsLeft = false;
		}

		public override void Draw(Graphics g)
		{
			if ((mParent == null || mWidth != 0) && IsInVisibleRange())
			{
				if ((float)(double)mScale > mOriginalCrystalScale)
				{
					DeferOverlay(2);
				}
				else
				{
					DrawArrow(g);
				}
			}
		}

		public override void DrawOverlay(Graphics g)
		{
			DrawArrow(g);
		}

		public void DrawArrow(Graphics g)
		{
			float num = (float)(double)mAlpha;
			int num2 = (int)(num * 255f);
			mColor.mAlpha = num2;
			mTextAlpha = num;
			g.SetColor(new Color(255, 255, 255, num2));
			Graphics3D graphics3D = g.Get3D();
			if (graphics3D != null && (mOffset.mX != 0f || mOffset.mY != 0f))
			{
				SexyTransform2D theTransform = new SexyTransform2D(true);
				theTransform.Translate(mOffset.mX, mOffset.mY);
				graphics3D.PushTransform(theTransform);
			}
			g.PushState();
			g.TranslateF((float)mWidth / 2f, (float)mHeight / 2f);
			if (!mLocked && mDoBob)
			{
				g.TranslateF((float)(GlobalMembers.S(mXBob) * (1.0 - (double)mFullPct)), (float)(GlobalMembers.S(mYBob) * (1.0 - (double)mFullPct)));
			}
			float num3 = (float)(double)mScale;
			float num4 = num3;
			num4 += mMouseOverPct * ConstantsWP.CRYSTALBALL_HIGHLIGHT_SCALE_1;
			if (mZ > 0f)
			{
				num4 *= ModVal.M(0.00255f) / mZ;
			}
			float theNum = num4 * GlobalMembers.MAGIC_SCALE * (1f + (float)(double)mFullPct * ModVal.M(0.5f));
			float theNum2 = num4 * GlobalMembers.MAGIC_SCALE * (1f + (float)(double)mFullPct * ModVal.M(0.2f));
			float num5 = 1f - (float)(double)mFullPct;
			int num6 = (int)Math.Max(0f, num5 * 255f);
			num6 = (int)((float)num6 * num);
			int theAlpha = (int)((float)(num2 * g.GetFinalColor().mAlpha) / 255f);
			mRayEffect.mColor = new Color(num6, num6, num6, theAlpha);
			mGlowEffect.mColor = new Color(num6, num6, num6, theAlpha);
			if (!mLocked)
			{
				mRayEffect.mDrawTransform.LoadIdentity();
				mRayEffect.mDrawTransform.Scale(GlobalMembers.S(theNum), GlobalMembers.S(theNum2));
				mRayEffect.Draw(g);
			}
			if (mLocked)
			{
				g.SetColor(new Color(mColor.mRed, mColor.mGreen, mColor.mBlue, (int)((float)mColor.mAlpha * ModVal.M(0.5f))));
			}
			else
			{
				g.SetColor(mColor);
			}
			g.SetColorizeImages(true);
			g.SetDrawMode(0);
			int num7 = (int)(mLocked ? ModVal.M(0f) : ((float)((mUpdateCnt + mAnimationFrameOffset) / 10 % 25)));
			num3 /= ConstantsWP.CRYSTALBALL_BASE_SCALE;
			num3 += mMouseOverPct * ConstantsWP.CRYSTALBALL_HIGHLIGHT_SCALE_2;
			g.SetDrawMode(0);
			SexyVertex2D[] array = new SexyVertex2D[4];
			if (!mIsDown)
			{
				if (!IsLeft)
				{
					array[0].x = mX;
					array[0].y = mY + mHeight;
					array[0].u = 0f;
					array[0].v = 1f;
					array[1].x = mX;
					array[1].y = mY;
					array[1].u = 0f;
					array[1].v = 0f;
					array[2].x = mX + mWidth;
					array[2].y = mY + mHeight;
					array[2].u = 1f;
					array[2].v = 1f;
					array[3].x = mX + mWidth;
					array[3].y = mY;
					array[3].u = 1f;
					array[3].v = 0f;
					g.DrawTrianglesTexStrip(Imag[(num7 < 10) ? num7 : 0], array, 2);
				}
				else
				{
					array[0].x = mX;
					array[0].y = mY + mHeight;
					array[0].u = 1f;
					array[0].v = 0f;
					array[1].x = mX;
					array[1].y = mY;
					array[1].u = 1f;
					array[1].v = 1f;
					array[2].x = mX + mWidth;
					array[2].y = mY + mHeight;
					array[2].u = 0f;
					array[2].v = 0f;
					array[3].x = mX + mWidth;
					array[3].y = mY;
					array[3].u = 0f;
					array[3].v = 1f;
					g.DrawTrianglesTexStrip(Imag[(num7 < 10) ? (10 - num7) : 0], array, 2);
				}
			}
			else if (!IsLeft)
			{
				array[0].x = mX + 5;
				array[0].y = mY + mHeight + 5;
				array[0].u = 0f;
				array[0].v = 1f;
				array[1].x = mX + 5;
				array[1].y = mY + 5;
				array[1].u = 0f;
				array[1].v = 0f;
				array[2].x = mX + mWidth + 5;
				array[2].y = mY + mHeight + 5;
				array[2].u = 1f;
				array[2].v = 1f;
				array[3].x = mX + mWidth + 5;
				array[3].y = mY + 5;
				array[3].u = 1f;
				array[3].v = 0f;
				g.DrawTrianglesTexStrip(Imag[(num7 < 10) ? num7 : 0], array, 2);
			}
			else
			{
				array[0].x = mX + 5;
				array[0].y = mY + mHeight + 5;
				array[0].u = 1f;
				array[0].v = 0f;
				array[1].x = mX + 5;
				array[1].y = mY + 5;
				array[1].u = 1f;
				array[1].v = 1f;
				array[2].x = mX + mWidth + 5;
				array[2].y = mY + mHeight + 5;
				array[2].u = 0f;
				array[2].v = 0f;
				array[3].x = mX + mWidth + 5;
				array[3].y = mY + 5;
				array[3].u = 0f;
				array[3].v = 1f;
				g.DrawTrianglesTexStrip(Imag[(num7 < 10) ? (10 - num7) : 0], array, 2);
			}
			g.SetDrawMode(Graphics.DrawMode.Additive);
			if (mIsDown)
			{
				if (!IsLeft)
				{
					array[0].x = mX + 5;
					array[0].y = mY + mHeight + 5;
					array[0].u = 0f;
					array[0].v = 1f;
					array[1].x = mX + 5;
					array[1].y = mY + 5;
					array[1].u = 0f;
					array[1].v = 0f;
					array[2].x = mX + mWidth + 5;
					array[2].y = mY + mHeight + 5;
					array[2].u = 1f;
					array[2].v = 1f;
					array[3].x = mX + mWidth + 5;
					array[3].y = mY + 5;
					array[3].u = 1f;
					array[3].v = 0f;
					g.DrawTrianglesTexStrip(GlobalMembersResourcesWP.IMAGE_ARROW_GLOW, array, 2);
				}
				else
				{
					array[0].x = mX + 5;
					array[0].y = mY + mHeight + 5;
					array[0].u = 1f;
					array[0].v = 0f;
					array[1].x = mX + 5;
					array[1].y = mY + 5;
					array[1].u = 1f;
					array[1].v = 1f;
					array[2].x = mX + mWidth + 5;
					array[2].y = mY + mHeight + 5;
					array[2].u = 0f;
					array[2].v = 0f;
					array[3].x = mX + mWidth + 5;
					array[3].y = mY + 5;
					array[3].u = 0f;
					array[3].v = 1f;
					g.DrawTrianglesTexStrip(GlobalMembersResourcesWP.IMAGE_ARROW_GLOW, array, 2);
				}
			}
			g.SetDrawMode(Graphics.DrawMode.Normal);
			if (graphics3D != null && (mOffset.mX != 0f || mOffset.mY != 0f))
			{
				graphics3D.PopTransform();
			}
			g.PopState();
			if (mTextIsQuestionMark || mLabel.Length <= 0)
			{
				return;
			}
			g.SetFont(GlobalMembersResources.FONT_CRYSTALBALL);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_CRYSTALBALL, 0, mFontColor);
			g.SetColor(new Color(255, 255, 255, (int)(255f * mTextAlpha)));
			num4 = ConstantsWP.CRYSTALBALL_FONT_SCALE;
			Utils.PushScale(g, num4, num4, mWidth / 2, mHeight / 2);
			g.PushState();
			g.mClipRect.mX -= 1000;
			g.mClipRect.mWidth += 2000;
			g.mClipRect.mY -= 1000;
			g.mClipRect.mHeight += 2000;
			if ((float)(double)mScale == mOriginalCrystalScale)
			{
				int num8 = (IsLeft ? 20 : 0);
				if (mLabel3.Length != 0)
				{
					g.DrawString(mLabel, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel) / 2 + num8 + (mIsDown ? 5 : 0), mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_3_1_Y + (mIsDown ? 5 : 0));
					g.DrawString(mLabel2, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel2) / 2 + num8 + (mIsDown ? 5 : 0), mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_3_2_Y + (mIsDown ? 5 : 0));
					g.DrawString(mLabel3, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel3) / 2 + num8 + (mIsDown ? 5 : 0), mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_3_3_Y + (mIsDown ? 5 : 0));
				}
				else if (mLabel2.Length != 0)
				{
					g.DrawString(mLabel, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel) / 2 + num8 + (mIsDown ? 5 : 0), mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_2_1_Y + (mIsDown ? 5 : 0));
					g.DrawString(mLabel2, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel2) / 2 + num8 + (mIsDown ? 5 : 0), mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_2_2_Y + (mIsDown ? 5 : 0));
				}
				else
				{
					g.DrawString(mLabel, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel) / 2 + num8 + (mIsDown ? 5 : 0), mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_1_1_Y + (mIsDown ? 5 : 0));
				}
			}
			g.PopState();
			Utils.PopScale(g);
		}
	}
}
