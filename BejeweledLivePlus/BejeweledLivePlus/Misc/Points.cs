using System;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public class Points : IDisposable
	{
		public enum POINTSSTATE
		{
			STATE_RISING,
			STATE_FADING,
			STATE_VERT_SHIFTING
		}

		public float mX;

		public float mY;

		public float mDY;

		public bool mLimitY;

		public int mState;

		public int mScalePoints;

		public int mCorrectedPoints;

		public int mWidth;

		public int mHeight;

		public BejeweledLivePlusApp mApp;

		public float mTimer;

		public string mString = string.Empty;

		public ImageFont mFont;

		public int mHue;

		public MemoryImage[] mImage = new MemoryImage[GlobalMembers.Max_LAYERS];

		public string mSubString = string.Empty;

		public int mSubStringShowTick;

		public ImageFont mSubFont;

		public Color mSubColor = default(Color);

		public int mLayerCount;

		public int mDelay;

		public float mScale;

		public float mDestScale;

		public float mScaleAdd;

		public float mScaleDampening;

		public float mScaleDifMult;

		public ColorCycle[] mColorCycle = new ColorCycle[GlobalMembers.Max_LAYERS];

		public bool mColorCycling;

		public int mUpdateCnt;

		public Color mColor = default(Color);

		public float mWobbleSin;

		public float mWobbleScale;

		public bool mDoBounce;

		public float mAlpha;

		public bool mWantCachedImage;

		public MemoryImage mCachedImage;

		public float mRotation;

		public uint mId;

		public int mMoveCreditId;

		public uint mValue;

		public bool mDrawn;

		public bool mDeleteMe;

		public Points(BejeweledLivePlusApp theApp, Font theFont, string theString, int theX, int theY, float theLife, int theJustification, Color theColor, int theAnim)
		{
			mLimitY = true;
			mDelay = 0;
			mColorCycling = false;
			for (int i = 0; i < mImage.Length; i++)
			{
				mImage[i] = null;
			}
			mRotation = 0f;
			mApp = theApp;
			mFont = (ImageFont)theFont;
			mTimer = theLife;
			mColor = theColor;
			mWobbleSin = 0f;
			mDoBounce = false;
			mWantCachedImage = false;
			mCachedImage = null;
			for (int j = 0; j < GlobalMembers.Max_LAYERS; j++)
			{
				mImage[j] = null;
			}
			if (theAnim < 3)
			{
				for (int k = 0; k < GlobalMembers.Max_LAYERS; k++)
				{
					mColorCycle[k] = new ColorCycle();
					mColorCycle[k].mCyclePos = Math.Abs(GlobalMembersUtils.GetRandFloat());
					mColorCycle[k].mCycleColors.Clear();
					mColorCycle[k].mColor = new Color(0, 0, 0, 0);
				}
			}
			for (int l = 0; l < GlobalMembers.Max_LAYERS; l++)
			{
				mColorCycle[l].SetSpeed(ModVal.M(1.8f));
			}
			Color item = mColor;
			item.mRed = (int)Math.Min(255f, (float)item.mRed * 1.5f);
			item.mGreen = (int)Math.Min(255f, (float)item.mGreen * 1.5f);
			item.mBlue = (int)Math.Min(255f, (float)item.mBlue * 1.5f);
			Color item2 = mColor;
			item2.mRed = (int)Math.Min(255f, (float)item2.mRed * 0.5f);
			item2.mGreen = (int)Math.Min(255f, (float)item2.mGreen * 0.5f);
			item2.mBlue = (int)Math.Min(255f, (float)item2.mBlue * 0.5f);
			switch (theAnim)
			{
			case 0:
				mColorCycle[0].mCycleColors.Add(item2);
				mColorCycle[0].mCycleColors.Add(item2);
				break;
			case 1:
				mColorCycle[0].mCycleColors.Add(item2);
				mColorCycle[0].mCycleColors.Add(item);
				break;
			case 2:
				mColorCycle[0].mCycleColors.Add(item2);
				mColorCycle[0].mCycleColors.Add(item);
				mColorCycle[0].SetPosition(0.25f);
				mColorCycle[2].mCycleColors.Add(item2);
				mColorCycle[2].mCycleColors.Add(item);
				mColorCycle[2].SetPosition(0.75f);
				break;
			default:
				mColorCycle[0].SetPosition(0.25f);
				mColorCycle[2].SetPosition(0.5f);
				break;
			}
			mString = theString;
			RestartWobble();
			mX = theX;
			mY = theY;
			mScale = 0f;
			mScaleAdd = 0f;
			mDY = GlobalMembers.MS(1.2f);
			mUpdateCnt = 0;
			mSubStringShowTick = -1;
			mSubFont = null;
			mId = uint.MaxValue;
			mMoveCreditId = -1;
			mState = 0;
			mAlpha = 1f;
			mLayerCount = mFont.GetLayerCount();
			mDrawn = false;
			mDeleteMe = false;
		}

		public virtual void Dispose()
		{
			for (int i = 0; i < 3; i++)
			{
				if (mImage[i] != null)
				{
					mImage[i].Dispose();
				}
			}
			if (mCachedImage != null)
			{
				mCachedImage.Dispose();
			}
		}

		public virtual void Update()
		{
			mUpdateCnt++;
			int num = mFont.StringWidth(mString);
			if (mX + (float)(num / 2) * ModVal.M(1.5f) > 1920f)
			{
				mX = 1920f - (float)(num / 2) * ModVal.M(1.5f);
			}
			if (mDelay > 0)
			{
				mDelay--;
				return;
			}
			mWobbleSin += ModVal.M(0.03f) * GlobalMembers.M_PI * 2f;
			if (mWobbleSin > GlobalMembers.M_PI * 2f)
			{
				mWobbleSin -= GlobalMembers.M_PI * 2f;
			}
			mWobbleScale -= ModVal.M(0.005f);
			if (mWobbleScale < 0f)
			{
				mWobbleScale = 0f;
			}
			for (int i = 0; i < 3; i++)
			{
				mColorCycle[i].Update();
			}
			float num2 = mDestScale - mScale;
			if (mState == 0)
			{
				mScaleAdd += num2 * mScaleDifMult;
				mScaleAdd *= mScaleDampening;
				mScale += mScaleAdd;
				if (mScale < mDestScale && !mDoBounce)
				{
					mScale = mDestScale;
				}
			}
			else if (mState != 2)
			{
				mAlpha -= ModVal.M(0.05f);
				mScale -= ModVal.M(0.03f);
				if (mScale <= 0f || mAlpha <= 0f)
				{
					mDeleteMe = true;
				}
			}
			float num3 = (float)Math.Pow(Math.Min(mY / ModVal.M(50f), 1f), ModVal.M(0.015f));
			mDY *= num3;
			mY -= mDY;
			if (mLimitY)
			{
				mY = Math.Max(ModVal.M(75f), mY);
			}
			mTimer -= 0.01f;
			if (mTimer <= 0f && mState == 0)
			{
				StartFading();
			}
		}

		public virtual void Draw(Graphics g)
		{
			if (mDelay > 0)
			{
				return;
			}
			bool flag = mApp.Is3DAccelerated();
			float num = (float)((double)mScale + Math.Sin(mWobbleSin) * (double)mWobbleScale);
			if (!mDoBounce)
			{
				num = mScale;
			}
			if (mWantCachedImage)
			{
				UpdateCachedImage();
			}
			if (mWantCachedImage && mCachedImage != null)
			{
				Transform transform = new Transform();
				transform.Scale(num, num);
				transform.RotateRad(mRotation);
				g.DrawImageTransform(mCachedImage, transform, GlobalMembers.S(mX), GlobalMembers.S(mY));
			}
			else
			{
				if (mRotation != 0f && SexyFramework.GlobalMembers.gIs3D)
				{
					SexyTransform2D theTransform = new SexyTransform2D(true);
					theTransform.Translate(0f - GlobalMembers.S(g.mTransX + mX), 0f - GlobalMembers.S(g.mTransY + mY));
					theTransform.RotateRad(mRotation);
					theTransform.Translate(GlobalMembers.S(g.mTransX + mX), GlobalMembers.S(g.mTransY + mY));
					g.Get3D().PushTransform(theTransform);
				}
				SetupForDraw(g);
				int num2 = mFont.StringWidth(mString);
				int ascent = mFont.GetAscent();
				if (SexyFramework.GlobalMembers.gIs3D)
				{
					g.mClipRect.mWidth += 1000;
					g.mClipRect.mX -= 500;
				}
				float num3 = num;
				Utils.PushScale(g, num3, num3, GlobalMembers.S(mX), GlobalMembers.S(mY));
				int num4 = 0;
				int num5 = (int)(GlobalMembers.S(mY) + (float)ascent * ModVal.M(0.2f));
				if (mX != 0f)
				{
					num4 = (int)(GlobalMembers.S(mX) - (float)(num2 / 2));
					if ((float)num4 < ConstantsWP.POINTS_LIMIT)
					{
						mX = GlobalMembers.RS(ConstantsWP.POINTS_LIMIT + (float)(num2 / 2));
						num4 = (int)ConstantsWP.POINTS_LIMIT;
					}
					if ((float)num4 > (float)GlobalMembers.gApp.mWidth - ConstantsWP.POINTS_LIMIT - (float)num2)
					{
						mX = GlobalMembers.RS((float)GlobalMembers.gApp.mWidth - ConstantsWP.POINTS_LIMIT - (float)num2 + (float)(num2 / 2));
						num4 = (int)((float)GlobalMembers.gApp.mWidth - ConstantsWP.POINTS_LIMIT - (float)num2);
					}
				}
				if ((float)num5 < ConstantsWP.POINTS_LIMIT_TOP)
				{
					mY = ConstantsWP.POINTS_LIMIT_TOP - (float)ascent * ModVal.M(0.2f);
					num5 = (int)ConstantsWP.POINTS_LIMIT_TOP;
				}
				g.DrawString(mString, num4, num5);
				Utils.PopScale(g);
				int fontLayerCount = Utils.GetFontLayerCount(mFont);
				for (int i = 0; i < fontLayerCount; i++)
				{
					Utils.SetFontLayerColor(mFont, i, Color.White);
				}
				if (mRotation != 0f && SexyFramework.GlobalMembers.gIs3D)
				{
					g.Get3D().PopTransform();
				}
			}
			if (mSubStringShowTick >= 0 && mSubStringShowTick <= mUpdateCnt && mSubString.Length != 0 && mSubFont != null)
			{
				int num6 = mSubFont.StringWidth(mSubString);
				int ascent2 = mSubFont.GetAscent();
				g.SetFont(mSubFont);
				int theNum = (int)(mY + ModVal.M(-80f));
				double num7 = (float)(mUpdateCnt - mSubStringShowTick) / ModVal.M(20f);
				if (num7 < 1.0)
				{
					num = (float)((double)ModVal.M(1f) + (double)ModVal.M(0.5f) * (1.0 - num7));
				}
				g.SetScale(num, num, GlobalMembers.S(mX), GlobalMembers.S(theNum));
				double num8 = Math.Min(mAlpha, num7);
				int theLayer = ((!(ModVal.M(1f) >= 0f) || !(ModVal.M(1f) <= -0f)) ? (Utils.GetFontLayerCount(mSubFont) - 1) : 0);
				g.mColor.mAlpha = (int)(255.0 * num8);
				g.SetColorizeImages(num8 < 1.0);
				mSubFont.PushLayerColor(theLayer, mSubColor);
				g.DrawString(mSubString, (int)(GlobalMembers.S(mX) - (float)(num6 / 2)), (int)((float)GlobalMembers.S(theNum) + (float)ascent2 * ModVal.M(0.2f)));
				mSubFont.PopLayerColor(theLayer);
			}
			if (!flag)
			{
				g.SetFastStretch(false);
			}
			g.SetColorizeImages(false);
			mDrawn = true;
		}

		public void RestartWobble()
		{
			mWobbleScale = ModVal.M(0.3f);
		}

		public void StartFading()
		{
			mState = 1;
		}

		public void SetupForDraw(Graphics g)
		{
			bool flag = mApp.Is3DAccelerated();
			g.SetColorizeImages(true);
			g.SetDrawMode(0);
			g.SetFont(mFont);
			Color color = default(Color);
			int num = 0;
			color = mColor;
			color.mRed = Math.Max(0, Math.Min(255, (int)((float)color.mRed * 0.25f)));
			color.mGreen = Math.Max(0, Math.Min(255, (int)((float)color.mGreen * 0.25f)));
			color.mBlue = Math.Max(0, Math.Min(255, (int)((float)color.mBlue * 0.25f)));
			color.mAlpha = Math.Min(255, (int)(mAlpha * 255f));
			g.SetColor(color);
			if (!flag)
			{
				g.SetFastStretch(true);
			}
			color = mColor;
			color.mRed = Math.Max(0, Math.Min(255, color.mRed + num));
			color.mGreen = Math.Max(0, Math.Min(255, color.mGreen + num));
			color.mBlue = Math.Max(0, Math.Min(255, color.mBlue + num));
			color.mAlpha = Math.Min(255, (int)(mAlpha * 255f));
			g.SetColor(new Color(255, 255, 255, (int)(mAlpha * 255f)));
			if (mFont == GlobalMembersResources.FONT_FLOATERS)
			{
				for (int i = 0; i < Utils.GetFontLayerCount(mFont); i++)
				{
					if (i != mLayerCount - 1)
					{
						mColorCycle[i].GetColor();
					}
					color.mAlpha *= (int)mAlpha;
					Utils.SetFontLayerColor(mFont, i, color);
				}
			}
			g.SetColor(Color.White);
		}

		public void UpdateCachedImage()
		{
			int num = mFont.StringWidth(mString);
			int ascent = mFont.GetAscent();
			if (mCachedImage == null)
			{
				mCachedImage = new MemoryImage();
				mCachedImage.Create((int)((float)num * ModVal.M(1.1f)), (int)((float)ascent * ModVal.M(1.1f)));
				mCachedImage.mIsVolatile = true;
				mCachedImage.SetImageMode(true, true);
			}
			Graphics graphics = new Graphics(mCachedImage);
			SetupForDraw(graphics);
			graphics.WriteString(mString, mCachedImage.mWidth / 2, mCachedImage.mHeight / 2 + GlobalMembers.MS(24));
		}
	}
}
