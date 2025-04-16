using System;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Misc
{
	public class CrystalBall : ButtonWidget
	{
		private int mEffectStartCountdown;

		public CurvedVal mAlpha = new CurvedVal();

		public SharedImageRef mImage = new SharedImageRef();

		public Rect mImageSrcRect = default(Rect);

		public PIEffect mGlowEffect;

		public PIEffect mRayEffect;

		public CurvedVal mFullPct = new CurvedVal();

		public CurvedVal mScale = new CurvedVal();

		public FPoint mOffset = default(FPoint);

		public float mZ;

		public CurvedVal mXBob = new CurvedVal();

		public CurvedVal mYBob = new CurvedVal();

		public float mTextAlpha;

		public CurvedVal mLeftArrowPct = new CurvedVal();

		public CurvedVal mRightArrowPct = new CurvedVal();

		public float mMouseOverPct;

		public float mBaseAlpha;

		public Color mColor = default(Color);

		public Color mFontColor = default(Color);

		public float mExtraFontScaling;

		public new int mUpdateCnt;

		public int mFlushPriority;

		public bool mLocked;

		public bool mDoBob;

		public bool mTextIsQuestionMark;

		public bool mHasCrest;

		public float[] mDists = new float[GlobalMembers.NUM_DIST_POINTS];

		public float[] mTexDists = new float[GlobalMembers.NUM_DIST_POINTS];

		public float[] mAlphas = new float[GlobalMembers.NUM_DIST_POINTS];

		public float[] mDistMults = new float[GlobalMembers.NUM_RADIAL_POINTS + 1];

		public float[] mSins = new float[GlobalMembers.NUM_RADIAL_POINTS + 1];

		public float[] mCoss = new float[GlobalMembers.NUM_RADIAL_POINTS + 1];

		public string mLabel2 = string.Empty;

		public string mLabel3 = string.Empty;

		public int mAnimationFrameOffset;

		public bool mPressed;

		public float mOriginalCrystalScale;

		public int mOriginalX;

		public int mOriginalY;

		public bool mRestartGame;

		public float mFontScale = 1f;

		public bool IsInVisibleRange()
		{
			int num = GetAbsPos().mX + mWidth / 2;
			if (num + ConstantsWP.CRYSTALBALL_STOP_DRAW_OFFSET < 0 || num - ConstantsWP.CRYSTALBALL_STOP_DRAW_OFFSET > GlobalMembers.gApp.mWidth)
			{
				return false;
			}
			return true;
		}

		public CrystalBall(string theLabel1, string theLabel2, string theLabel3, int theId, ButtonListener theListener, Color fontColour)
			: this(theLabel1, theLabel2, theLabel3, theId, theListener, fontColour, 1f)
		{
		}

		public CrystalBall(string theLabel1, string theLabel2, string theLabel3, int theId, ButtonListener theListener, Color fontColour, float scale)
			: base(theId, theListener)
		{
			mPressed = false;
			mOriginalCrystalScale = 1f;
			mRestartGame = true;
			mAlpha.SetConstant(1.0);
			Point point = new Point((int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL.GetCelWidth() * scale), (int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL.GetCelHeight() * scale));
			mWidth = point.mX;
			mHeight = point.mY;
			mFlushPriority = -1;
			mScale.SetConstant(scale * ConstantsWP.CRYSTALBALL_BASE_SCALE);
			mOriginalCrystalScale = (float)(double)mScale;
			mOriginalX = mX;
			mOriginalY = mY;
			mZ = 0f;
			mClip = false;
			mLabel = theLabel1;
			mLabel2 = theLabel2;
			mLabel3 = theLabel3;
			mDoFinger = theLabel1.Length != 0;
			mTextAlpha = 1f;
			mMouseOverPct = 0f;
			mBaseAlpha = 0f;
			mColor = Color.White;
			mLocked = false;
			mDoBob = true;
			mUpdateCnt = 0;
			mTextIsQuestionMark = false;
			mHasCrest = false;
			mExtraFontScaling = 0f;
			mGlowEffect = GlobalMembersResourcesWP.PIEFFECT_CRYSTALBALL.Duplicate();
			mGlowEffect.mEmitAfterTimeline = true;
			mRayEffect = GlobalMembersResourcesWP.PIEFFECT_CRYSTALRAYS.Duplicate();
			mRayEffect.mEmitAfterTimeline = true;
			mFontColor = fontColour;
			if (SexyFramework.GlobalMembers.gIs3D)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eCRYSTAL_BALL_X_BOB, mXBob);
				mXBob.SetMode(1);
				mXBob.mInitAppUpdateCount = Common.Rand() % 100;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eCRYSTAL_BALL_Y_BOB, mYBob);
				mYBob.SetMode(1);
				mYBob.mInitAppUpdateCount = Common.Rand() % 100;
			}
			mEffectStartCountdown = Common.Rand(200);
			mWidgetFlagsMod.mRemoveFlags |= 8;
			mAnimationFrameOffset = Common.Rand(200);
		}

		public override void Dispose()
		{
		}

		public void StartInGameTransition(bool restartGame)
		{
			mPressed = true;
			mRestartGame = restartGame;
			mOriginalX = mX;
			mOriginalY = mY;
		}

		public void ManageTransitionEffect()
		{
			if (mPressed)
			{
				if ((float)(double)mScale < 1.5f)
				{
					mScale.SetConstant((float)(double)mScale + 0.02f);
					int num = ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_X - mWidth / 2;
					int num2 = ConstantsWP.MAIN_MENU_BUTTON_CLASSIC_Y - mHeight / 2;
					int num3 = (num - mX) / 50;
					int num4 = (num2 - mY) / 50;
					if ((float)(double)mAlpha >= 0.02f)
					{
						mAlpha.SetConstant((float)(double)mAlpha - 0.01f);
					}
					Resize(mX + num3, mY + num4, mWidth, mHeight);
				}
				else
				{
					mAlpha.SetConstant(0.0);
					GlobalMembers.gApp.StartSetupGame(mRestartGame);
					mPressed = false;
				}
			}
			else if ((float)(double)mScale > mOriginalCrystalScale)
			{
				mScale.SetConstant((float)(double)mScale - 0.01f);
				int num5 = mOriginalX;
				int num6 = mOriginalY;
				int num7 = (num5 - mX) / 3;
				int num8 = (num6 - mY) / 3;
				if ((float)(double)mAlpha < 1f)
				{
					mAlpha.SetConstant((float)(double)mAlpha + 0.005f);
				}
				Resize(mX + num7, mY + num8, mWidth, mHeight);
			}
			else if ((float)(double)mScale < mOriginalCrystalScale)
			{
				mScale.SetConstant(mOriginalCrystalScale);
				Resize(mOriginalX, mOriginalY, mWidth, mHeight);
				mAlpha.SetConstant(1.0);
			}
		}

		public override void Resize(int x, int y, int width, int height)
		{
			base.Resize(x, y, width, height);
			SexyTransform2D mDrawTransform = new SexyTransform2D(true);
			mDrawTransform.LoadIdentity();
			float num = (float)(double)mScale;
			if (mZ > 0f)
			{
				num *= ModVal.M(0.00195f) / mZ;
			}
			mDrawTransform.Scale(num * GlobalMembers.MAGIC_SCALE * (1f + (float)(double)mFullPct * ModVal.M(0.5f)), num * GlobalMembers.MAGIC_SCALE * (1f + (float)(double)mFullPct * ModVal.M(0.2f)));
			mGlowEffect.mDrawTransform = mDrawTransform;
			mRayEffect.mDrawTransform = mDrawTransform;
		}

		public void DrawCrystal(Graphics g)
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
			int theCel = (int)(mLocked ? ModVal.M(0f) : ((float)((mUpdateCnt + mAnimationFrameOffset) / 4 % 20)));
			num3 /= ConstantsWP.CRYSTALBALL_BASE_SCALE;
			num3 += mMouseOverPct * ConstantsWP.CRYSTALBALL_HIGHLIGHT_SCALE_2;
			g.SetDrawMode(1);
			int num7 = (int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL_SHADOW.GetCelWidth() * num3);
			int num8 = (int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL_SHADOW.GetCelHeight() * num3);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_CRYSTALBALL_SHADOW, -num7 / 2, -num8 / 2, num7, num8);
			g.SetDrawMode(0);
			num7 = (int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL.GetCelWidth() * num3);
			num8 = (int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL.GetCelHeight() * num3);
			g.DrawImageCel(GlobalMembersResourcesWP.IMAGE_CRYSTALBALL, new Rect(-num7 / 2, -num8 / 2, num7, num8), theCel);
			if (mIsDown)
			{
				g.SetDrawMode(1);
				num7 = (int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL_GLOW.GetCelWidth() * num3);
				num8 = (int)((float)GlobalMembersResourcesWP.IMAGE_CRYSTALBALL_GLOW.GetCelHeight() * num3);
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_CRYSTALBALL_GLOW, -num7 / 2, -num8 / 2, num7, num8);
				g.SetDrawMode(0);
			}
			if (!mLocked)
			{
				mGlowEffect.mDrawTransform.LoadIdentity();
				mGlowEffect.mDrawTransform.Scale(GlobalMembers.S(theNum), GlobalMembers.S(theNum2));
				mGlowEffect.Draw(g);
			}
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
			num4 = ConstantsWP.CRYSTALBALL_FONT_SCALE * mFontScale;
			Utils.PushScale(g, num4, num4, mWidth / 2, mHeight / 2);
			g.PushState();
			g.mClipRect.mX -= 1000;
			g.mClipRect.mWidth += 2000;
			g.mClipRect.mY -= 1000;
			g.mClipRect.mHeight += 2000;
			if ((float)(double)mScale == mOriginalCrystalScale)
			{
				if (mLabel3.Length != 0)
				{
					g.DrawString(mLabel, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel) / 2, mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_3_1_Y);
					g.DrawString(mLabel2, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel2) / 2, mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_3_2_Y);
					g.DrawString(mLabel3, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel3) / 2, mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_3_3_Y);
				}
				else if (mLabel2.Length != 0)
				{
					g.DrawString(mLabel, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel) / 2, mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_2_1_Y);
					g.DrawString(mLabel2, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel2) / 2, mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_2_2_Y);
				}
				else
				{
					g.DrawString(mLabel, mWidth / 2 - GlobalMembersResources.FONT_CRYSTALBALL.StringWidth(mLabel) / 2, mHeight / 2 + ConstantsWP.CRYSTALBALL_TEXT_1_1_Y);
				}
			}
			g.PopState();
			Utils.PopScale(g);
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
					DrawCrystal(g);
				}
			}
		}

		public override void DrawOverlay(Graphics g)
		{
			DrawCrystal(g);
		}

		public override void Update()
		{
			WidgetUpdate();
			if (mVisible && IsInVisibleRange())
			{
				mUpdateCnt++;
				if (mGlowEffect != null)
				{
					mGlowEffect.Update();
				}
				if (mEffectStartCountdown > 0)
				{
					mEffectStartCountdown--;
				}
				if (mEffectStartCountdown <= 0)
				{
					mRayEffect.Update();
				}
				if (mIsDown)
				{
					mMouseOverPct = Math.Min(1f, mMouseOverPct + 0.05f);
				}
				else
				{
					mMouseOverPct = Math.Max(0f, mMouseOverPct - 0.05f);
				}
				mAlpha.IncInVal();
			}
		}

		public override void MouseEnter()
		{
			base.MouseEnter();
		}

		public override void MouseUp(int theX, int theY, int theBtnNum, int theClickCount)
		{
			GlobalMembers.gApp.mLatestClickedBall = this;
			base.MouseUp(theX, theY, theBtnNum, theClickCount);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_MOUSEOVER);
		}

		public override void MouseDown(int theX, int theY, int theBtnNum, int theClickCount)
		{
			base.MouseDown(theX, theY, theBtnNum, theClickCount);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_PRESS);
		}

		public SexyVertex2D GetVertex(int theAngIdx, int theDistIdx)
		{
			float num = mDists[theDistIdx] * mDistMults[theAngIdx];
			float num2 = (float)GlobalMembers.S(1200) / 2f + (float)(GlobalMembers.S(1920) - GlobalMembers.S(1200)) / 2f * (float)(double)mFullPct;
			float num3 = (float)GlobalMembers.S(1200) / 2f;
			float num4 = num2 / num3 / 1.6f;
			float num5 = mTexDists[theDistIdx] * mDistMults[theAngIdx];
			float val = mAlphas[theDistIdx];
			SexyVertex2D result = new SexyVertex2D(0f + mCoss[theAngIdx] * num2 * num, 0f + mSins[theAngIdx] * num3 * num, 0.5f + mCoss[theAngIdx] * 0.5f * num5 * num4, 0.5f + mSins[theAngIdx] * 0.5f * num5, (uint)new Color(255, 255, 255, (int)(Math.Max(0f, val) * 255f)).ToInt());
			return result;
		}

		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
		}

		public void LinkUpAssets()
		{
		}
	}
}
