using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus.UI
{
	public class MainMenu : Bej3Widget
	{
		private enum MAINMENU_IDS
		{
			BTN_MYSTERY_ID
		}

		public MainMenuScrollContainer mContainer;

		private ScrollWidget mScrollWidget;

		private CurvedVal mBJ3LogoAlpha = new CurvedVal();

		private bool mBJ3LogoShowing;

		private bool mIntroFinished;

		private CurvedVal mShowCurve = new CurvedVal();

		public static int mScrollwidgetPage;

		public List<PartnerLogo> mPartnerLogos = new List<PartnerLogo>();

		public float mPartnerBlackAlpha;

		public Graphics3D.PerspectiveCamera mCamera = new Graphics3D.PerspectiveCamera();

		public Graphics3D.PerspectiveCamera mButtonCamera = new Graphics3D.PerspectiveCamera();

		public CurvedVal mRotation = new CurvedVal();

		public CurvedVal mButtonRotationAdd = new CurvedVal();

		public CurvedVal mForeBlackAlpha = new CurvedVal();

		public CurvedVal mBkgBlackAlpha = new CurvedVal();

		public CurvedVal mLogoAlpha = new CurvedVal();

		public CurvedVal mLoaderAlpha = new CurvedVal();

		public CurvedVal mTipTextAlpha = new CurvedVal();

		public int mHighestVirtFPS;

		public Font mUserNameFont;

		public float mDispLoadPct;

		public bool mLoaded;

		public bool mFinishedLoadSequence;

		public bool mSwitchedMusic;

		public bool mHasLoaderResources;

		public bool mLoadingThreadCompleted;

		public bool mDrawDeviceId;

		private static bool preFlight = false;

		private static CurvedVal txtAlpha = new CurvedVal(GlobalMembers.MP("b;0,1,0.01,5,####  ,####K~###      ^~###m####"));

		private static readonly int NUM_LOADERBAR_POINTS = 40;

		private void GoToPage(int page)
		{
			mScrollwidgetPage = page;
			mScrollWidget.ScrollToPoint(new Point(ConstantsWP.MAIN_MENU_TAB_WIDTH * mScrollwidgetPage, 0), true);
			mContainer.MakeButtonsFullyVisible();
		}

		public MainMenu()
			: base(Menu_Type.MENU_MAINMENU, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mBJ3LogoShowing = false;
			mIntroFinished = false;
			mScrollWidget = null;
			mCanAllowSlide = false;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_SHOW_CURVE, mShowCurve);
			mCanAllowSlide = false;
			mDoesSlideInFromBottom = false;
			mLoadingThreadCompleted = false;
			mPartnerBlackAlpha = 0f;
			mBJ3LogoAlpha.SetConstant(0.0);
			int integer = SexyFramework.GlobalMembers.gSexyApp.GetInteger("NumLogos", 0);
			for (int i = 0; i < integer; i++)
			{
				mPartnerBlackAlpha = 1f;
				string @string = SexyFramework.GlobalMembers.gSexyApp.GetString($"Logo{i + 1}File");
				PartnerLogo partnerLogo = new PartnerLogo();
				partnerLogo.mImage = SexyFramework.GlobalMembers.gSexyApp.GetImage(@string, true, true, false);
				partnerLogo.mTime = (partnerLogo.mOrgTime = SexyFramework.GlobalMembers.gSexyApp.GetInteger($"Logo{i + 1}HoldTime", 300));
				if (partnerLogo.mImage != null)
				{
					partnerLogo.mImage.AddImageFlags(8u);
					mPartnerLogos.Add(partnerLogo);
				}
			}
			mHasLoaderResources = true;
			mClip = false;
			mHighestVirtFPS = 0;
			mHasAlpha = true;
			mRotation.SetConstant(0.0);
			mContainer = null;
			mLoaded = false;
			mSwitchedMusic = false;
			mFinishedLoadSequence = false;
			mBkgBlackAlpha.SetConstant(0.0);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_FORE_BLACK_ALPHA, mForeBlackAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_LOGO_ALPHA_FADE_IN, mLogoAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_LOADER_ALPHA_FADE_IN, mLoaderAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_TIP_TEXT_ALPHA_FADE_IN, mTipTextAlpha);
			mContainer = null;
			mDispLoadPct = 0f;
			mDrawDeviceId = false;
			base.SystemButtonPressed += OnSystemButtonPressed;
		}

		private void OnSystemButtonPressed(SystemButtonPressedArgs args)
		{
			try
			{
				if (args.button == SystemButtons.Back)
				{
					args.processed = true;
					if (mContainer.CurrentPage == 0)
					{
						(GlobalMembers.gApp.mMenus[5] as MainMenuOptions)?.OnSystemButtonPressed(args);
					}
					else
					{
						mContainer.ButtonDepress(11);
					}
				}
			}
			catch (Exception)
			{
				GlobalMembers.gApp.WantExit = true;
			}
		}

		public override void Dispose()
		{
			GlobalMembers.KILL_WIDGET(mContainer);
			GlobalMembers.KILL_WIDGET(mScrollWidget);
			RemoveAllWidgets(true, false);
			base.Dispose();
		}

		public virtual int GetBoardX()
		{
			return GlobalMembers.RS(ConstantsWP.BOARD_X);
		}

		public int GetBoardCenterX()
		{
			return GetBoardX() + 400;
		}

		public virtual void QuitGameRequest()
		{
			Bej3Dialog bej3Dialog = (Bej3Dialog)GlobalMembers.gApp.DoDialog(33, true, GlobalMembers._ID("QUIT GAME?", 5000), GlobalMembers._ID("Are you sure you want to quit the game?", 5001), "", 1);
			int dIALOG_RESTART_GAME_WIDTH = ConstantsWP.DIALOG_RESTART_GAME_WIDTH;
			bej3Dialog.Resize(GlobalMembers.S(GetBoardCenterX()) - dIALOG_RESTART_GAME_WIDTH / 2, mHeight / 2, dIALOG_RESTART_GAME_WIDTH, bej3Dialog.GetPreferredHeight(dIALOG_RESTART_GAME_WIDTH));
			Bej3Button bej3Button = (Bej3Button)bej3Dialog.mYesButton;
			bej3Button.SetLabel(GlobalMembers._ID("QUIT GAME", 5002));
			bej3Dialog.SetButtonPosition(bej3Button, 0);
			bej3Dialog.mResult = int.MaxValue;
			bej3Button = (Bej3Button)bej3Dialog.mNoButton;
			bej3Button.SetLabel(GlobalMembers._ID("CANCEL", 3239));
			bej3Button.SetType(Bej3ButtonType.BUTTON_TYPE_LONG_PURPLE);
			bej3Dialog.mFlushPriority = 1;
			bej3Dialog.SetTopButtonType(Bej3ButtonType.TOP_BUTTON_TYPE_CLOSED);
		}

		public override void TouchBegan(SexyAppBase.Touch touch)
		{
		}

		public override void GotFocus()
		{
			if (mIntroFinished)
			{
				ShowLogo();
			}
		}

		public void SetupBackground()
		{
		}

		public override void Resize(Rect theRect)
		{
			base.Resize(theRect);
		}

		public override void UpdateAll(ModalFlags theFlags)
		{
			if (mVisible)
			{
				base.UpdateAll(theFlags);
			}
		}

		public override void Update()
		{
			if (mLoaded)
			{
				mBJ3LogoAlpha.IncInVal();
			}
			base.Update();
			if (Common.size(mPartnerLogos) > 0)
			{
				mPartnerBlackAlpha = 1f;
				PartnerLogo partnerLogo = mPartnerLogos[0];
				if (partnerLogo.mAlpha < 255 && partnerLogo.mTime == partnerLogo.mOrgTime)
				{
					partnerLogo.mAlpha += GlobalMembers.M(5);
					if (partnerLogo.mAlpha >= 255)
					{
						partnerLogo.mAlpha = 255;
					}
				}
				else if (--partnerLogo.mTime <= 0)
				{
					partnerLogo.mAlpha -= GlobalMembers.M(5);
					if (partnerLogo.mAlpha <= 0)
					{
						partnerLogo.mImage.Dispose();
						mPartnerLogos.RemoveAt(0);
					}
				}
				MarkDirty();
				return;
			}
			mPartnerBlackAlpha = Math.Max(0f, mPartnerBlackAlpha - GlobalMembers.M(0.05f));
			if (mLoaded && (double)mRotation == 0.0 && !mFinishedLoadSequence)
			{
				mFinishedLoadSequence = true;
				if (GlobalMembers.gApp.mHasFocus)
				{
					if (GlobalMembers.gApp.mProfile.mProfileName.Length == 0)
					{
						GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_WELCOMETOBEJEWELED);
					}
					else
					{
						GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_WELCOMEBACK);
					}
				}
			}
			if (mLoaded && mHasLoaderResources && (double)mLogoAlpha == 0.0)
			{
				BejeweledLivePlusApp.UnloadContent("Loader");
				mHasLoaderResources = false;
			}
			if (mLoaded && (double)mRotation == 0.0 && GlobalMembers.gApp.mProfile.mProfileName.Length == 0 && GlobalMembers.gApp.GetDialog(1) == null)
			{
				if (GlobalMembers.gApp.mLastUser.Length != 0)
				{
					GlobalMembers.gApp.mProfile.LoadProfile(GlobalMembers.gApp.mLastUser);
					GlobalMembers.gApp.mLastUser = "";
				}
				else if (GlobalMembers.gApp.mDialogMap.Count == 0)
				{
					GlobalMembers.gApp.DoWelcomeDialog();
				}
			}
			float num = GlobalMembers.gApp.mResourceManager.GetLoadResourcesListProgress(GlobalMembers.gApp.initialLoadGroups);
			mDispLoadPct += (num - mDispLoadPct) * ConstantsWP.LOADING_SMOOTH_STEP;
			if (!mLoaded && (float)(double)mLoaderAlpha >= 1f)
			{
				GlobalMembers.gApp.DoInitWhileLoading();
			}
			if (num >= 1f && !mLoaded && mDispLoadPct >= 0.995f)
			{
				OnLoaded();
				GlobalMembers.gApp.mCurveValCache.GetCurvedValMult(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_BUTTON_ROTATION_ADD, mButtonRotationAdd);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_LOGO_ALPHA_FADE_OUT, mLogoAlpha);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_LOADER_ALPHA_FADE_OUT, mLoaderAlpha);
				GlobalMembers.gApp.mCurveValCache.GetCurvedValMult(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_TIP_TEXT_ALPHA_FADE_OUT, mTipTextAlpha);
				if (!mSwitchedMusic)
				{
					mSwitchedMusic = true;
				}
				mLoaded = true;
				LoadingComplete();
				mContainer.Show();
				GlobalMembers.gApp.GoToInterfaceState(InterfaceState.INTERFACE_STATE_MAINMENU);
				if (GlobalMembers.gApp.mMusicInterface.isPlayingUserMusic())
				{
					GlobalMembers.gApp.ConfirmUserMusic();
				}
			}
			bool mLoaded2 = mLoaded;
			float num2 = (float)SexyFramework.GlobalMembers.gSexyApp.mScreenBounds.mWidth / (float)SexyFramework.GlobalMembers.gSexyApp.mScreenBounds.mHeight;
			float inFovDegrees = GlobalMembers.M(38.5f) * num2;
			mCamera.Init(inFovDegrees, num2, 0.1f, 1000f);
			mButtonCamera.Init(inFovDegrees, num2, 0.1f, 1000f);
			SexyCoords3 sexyCoords = new SexyCoords3();
			sexyCoords.Translate(0f, GlobalMembers.M(-0.4f), 0f);
			mCamera.SetCoords(mCamera.GetCoords().Leave(sexyCoords));
			mButtonCamera.SetCoords(mButtonCamera.GetCoords().Leave(sexyCoords));
			SexyCoords3 sexyCoords2 = new SexyCoords3();
			sexyCoords2.RotateRadZ((float)((double)mRotation * (double)GlobalMembers.M(-0.78f)));
			mCamera.SetCoords(mCamera.GetCoords().Leave(sexyCoords2));
			sexyCoords2.RotateRadZ((float)(double)mButtonRotationAdd);
			mButtonCamera.SetCoords(mButtonCamera.GetCoords().Leave(sexyCoords2));
			SexyVector3 sexyVector = default(SexyVector3);
			new FPoint((float)(GlobalMembers.gApp.mScreenBounds.mX + GlobalMembers.MS(160)) + sexyVector.x * (float)GlobalMembers.gApp.mScreenBounds.mWidth, sexyVector.y * (float)GlobalMembers.gApp.mScreenBounds.mHeight);
			new FPoint(mWidth / 2, mHeight / 2);
			SexyVector3 inEyePos = new SexyVector3(GlobalMembers.M(-0.802f), GlobalMembers.M(1.93f), GlobalMembers.M(0.64f));
			mButtonCamera.EyeToScreen(inEyePos);
			new FPoint((float)(GlobalMembers.gApp.mScreenBounds.mX + GlobalMembers.MS(160)) + sexyVector.x * (float)GlobalMembers.gApp.mScreenBounds.mWidth, sexyVector.y * (float)GlobalMembers.gApp.mScreenBounds.mHeight);
			new FPoint((float)(GlobalMembers.gApp.mScreenBounds.mX + GlobalMembers.MS(160)) + sexyVector.x * (float)GlobalMembers.gApp.mScreenBounds.mWidth, sexyVector.y * (float)GlobalMembers.gApp.mScreenBounds.mHeight);
			if (mScrollWidget != null)
			{
				int num3 = ConstantsWP.MAIN_MENU_TAB_WIDTH * mScrollwidgetPage;
				if (mScrollWidget.GetScrollOffset().mX != (float)(-num3))
				{
					mScrollWidget.mIsDown = false;
					mScrollWidget.ScrollToPoint(new Point(num3, 0), true);
				}
			}
			MarkDirty();
		}

		public override void Draw(Graphics g)
		{
			if (mUpdateCnt == 0)
			{
				Update();
			}
			if (mLoaded && !preFlight)
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_MAIN_MENU_LOGO, GlobalMembers.gApp.mWidth - 1, GlobalMembers.gApp.mHeight - 1);
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_BUTTON_LARGE, GlobalMembers.gApp.mWidth - 1, GlobalMembers.gApp.mHeight - 1);
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_MAIN_MENU_BACKGROUND, GlobalMembers.gApp.mWidth - 1, GlobalMembers.gApp.mHeight - 1);
				preFlight = true;
			}
			float num = (float)Math.Min(1.0, (double)GlobalMembers.M(2.3f) - (double)mDispLoadPct * (double)mBkgBlackAlpha * (double)GlobalMembers.M(2f));
			float num2 = (float)Math.Max(0.0, Math.Min(1.0, (double)mDispLoadPct * (double)mBkgBlackAlpha * (double)GlobalMembers.M(3f)));
			num -= num2 * GlobalMembers.M(0.4f);
			Math.Min(1.0, GlobalMembers.M(1.9) - (double)(mDispLoadPct * GlobalMembers.M(2.1f)));
			if (mLoaded)
			{
				DeferOverlay(1);
			}
			bool mLoaded2 = mLoaded;
			if (mLogoAlpha != null)
			{
				g.SetColorizeImages(true);
				g.SetColor(mLogoAlpha);
				int num3 = mWidth / 2 - GlobalMembersResourcesWP.IMAGE_LOADER_POPCAP_LOADER_POPCAP.mWidth / 2;
				int num4 = mHeight / 2 - GlobalMembersResourcesWP.IMAGE_LOADER_POPCAP_LOADER_POPCAP.mHeight / 2;
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_LOADER_POPCAP_LOADER_POPCAP, num3, num4);
				if (GlobalMembers.gApp.mResourceManager.mCurLocSet == 1145390149)
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_LOADER_POPCAP_WHITE_GERMAN_REGISTERED, num3 + (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_LOADER_POPCAP_WHITE_GERMAN_REGISTERED_ID) - GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_LOADER_POPCAP_LOADER_POPCAP_ID)), num4 + (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_LOADER_POPCAP_WHITE_GERMAN_REGISTERED_ID) - GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_LOADER_POPCAP_LOADER_POPCAP_ID)));
				}
				else
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_LOADER_POPCAP_BLACK_TM, num3 + (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_LOADER_POPCAP_BLACK_TM_ID) - GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_LOADER_POPCAP_LOADER_POPCAP_ID)), num4 + (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_LOADER_POPCAP_BLACK_TM_ID) - GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_LOADER_POPCAP_LOADER_POPCAP_ID)));
				}
			}
			if ((double)mLoaderAlpha > 0.0)
			{
				DrawLoadingBar(g);
			}
			if (Common.size(GlobalMembers.gApp.mTips) != 0 && GlobalMembers.gApp.mTipIdx > 0)
			{
				g.mTransX = 0f;
				g.mTransY = 0f;
				g.SetColor(new Color(255, 255, 255, (int)(255.0 * (double)mTipTextAlpha * Math.Max(0.0, Math.Min(1.0, (double)mDispLoadPct * GlobalMembers.M(2.0) - GlobalMembers.M(0.15))))));
				g.SetFont(GlobalMembersResources.FONT_DIALOG);
				((ImageFont)g.mFont).PushLayerColor("GLOW", new Color(64, 0, 32, 128));
				((ImageFont)g.mFont).PushLayerColor("OUTLINE", new Color(0, 0, 0, 0));
				g.WriteString(GlobalMembers.gApp.mTips[(GlobalMembers.gApp.mTipIdx - 1) % Common.size(GlobalMembers.gApp.mTips)], mWidth / 2, GlobalMembers.MS(1165));
				((ImageFont)g.mFont).PopLayerColor("OUTLINE");
				((ImageFont)g.mFont).PopLayerColor("GLOW");
			}
			if (mLoaded)
			{
				mHighestVirtFPS = (int)Math.Max(mHighestVirtFPS, GlobalMembers.gApp.mCurVFPS);
			}
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			if (mLoaded)
			{
				g.SetColorizeImages(true);
				g.SetColor(mBJ3LogoAlpha);
				g.SetColor(new Color(255, 255, 255, 255));
				Transform transform = new Transform();
				transform.Scale(1.35f, 1.1f);
				g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_MAIN_MENU_LOGO, transform, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_MAIN_MENU_LOGO_ID)) + 290, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_MAIN_MENU_LOGO_ID)) + 90);
			}
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			base.DrawAll(theFlags, g);
			mWidgetManager.FlushDeferredOverlayWidgets(1);
		}

		public override void ButtonMouseEnter(int theId)
		{
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_MOUSEOVER);
		}

		public override void ButtonDepress(int theId)
		{
			if (Common.size(GlobalMembers.gApp.mTooltipManager.mTooltips) > 0)
			{
				GlobalMembers.gApp.mTooltipManager.ClearTooltipsWithAnimation();
			}
			switch (theId)
			{
			case 7:
				GoToPage(1);
				break;
			case 8:
				GoToPage(2);
				break;
			case 9:
				GoToPage(0);
				break;
			}
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
		}

		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
		}

		public void OnLoaded()
		{
			GlobalMembers.gApp.ExtractResources();
			GlobalMembers.gApp.DoLoadingThreadCompleted();
			mLoadingThreadCompleted = true;
			GlobalMembers.gApp.LoadTempleMeshes();
			SetupBackground();
		}

		public void LoadingComplete()
		{
			mContainer = new MainMenuScrollContainer(this);
			mContainer.Resize(0, 0, ConstantsWP.MAIN_MENU_WIDTH, mHeight);
			mScrollWidget = new ScrollWidget(mContainer);
			mScrollWidget.Resize(0, 0, mWidth, mHeight);
			mScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.SCROLL_DISABLED);
			mScrollWidget.EnableBounce(true);
			mScrollWidget.AddWidget(mContainer);
			AddWidget(mScrollWidget);
			mContainer.mScrollWidget = mScrollWidget;
		}

		public bool mIsFullGame()
		{
			if (mContainer != null)
			{
				return mContainer.mIsFullGame;
			}
			return false;
		}

		public void HideLogo()
		{
			if (mBJ3LogoShowing)
			{
				mBJ3LogoShowing = false;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMAIN_MENU_BEJ3_LOGO_ALPHA, mBJ3LogoAlpha);
			}
		}

		public void ShowLogo()
		{
			if (mLoaded)
			{
				mIntroFinished = true;
				if (!mBJ3LogoShowing)
				{
					mBJ3LogoShowing = true;
					mBJ3LogoAlpha = mShowCurve;
				}
			}
		}

		public void DrawLoadingBar(Graphics g)
		{
			Graphics3D graphics3D = g.Get3D();
			SexyVertex2D[] array = new SexyVertex2D[NUM_LOADERBAR_POINTS * 2];
			float num = 0f;
			for (int i = 0; i < NUM_LOADERBAR_POINTS; i++)
			{
				float num2 = 1f / (float)(NUM_LOADERBAR_POINTS - 1) * mDispLoadPct;
				if (i == 0 || i == NUM_LOADERBAR_POINTS - 2)
				{
					num2 = 1f / (float)(NUM_LOADERBAR_POINTS - 1);
				}
				num2 *= GlobalMembers.M(1.022f);
				float num3 = (0f - GlobalMembers.M_PI) / 2f + num * GlobalMembers.M_PI * 2f;
				num += num2;
				float theU = ((i == 0) ? 0f : ((i != NUM_LOADERBAR_POINTS - 1) ? 0.5f : 1f));
				for (int j = 0; j < 2; j++)
				{
					float num4 = GlobalMembers.MS(180) + j * GlobalMembers.MS(60);
					array[i * 2 + j] = new SexyVertex2D(g.mTransX + (float)(mWidth / 2) + (float)Math.Cos(num3) * num4, (float)(mHeight / 2) + (float)Math.Sin(num3) * num4, theU, j);
				}
			}
			graphics3D.SetTexture(0, GlobalMembersResourcesWP.IMAGE_LOADER_WHITEDOT);
			float num5 = (float)((0.5 * Math.Sin((float)mUpdateCnt * GlobalMembers.M(0.03f)) * 0.5 * (double)GlobalMembers.M(0.5f) + (double)GlobalMembers.M(0.75f)) * (double)mLoaderAlpha);
			graphics3D.DrawPrimitive(708u, Graphics3D.EPrimitiveType.PT_TriangleStrip, array, NUM_LOADERBAR_POINTS * 2 - 2, new Color(255,255,210, (int)(255f * num5)), 1, 0f, 0f, true, 0u);
		}

		public override void Show()
		{
			if (mInterfaceState != 0)
			{
				BejeweledLivePlusApp.LoadContent("MainMenu");
				CustomBassMusicInterface theMusicInterface =
					(CustomBassMusicInterface)(GlobalMembers.gApp.mMusicInterface);
				if (theMusicInterface.mSongName != "MainMenu")
				{
					theMusicInterface.QueueEvent("Switch", theMusicInterface.mSongName, false);
					theMusicInterface.QueueEvent("FadeOut", theMusicInterface.mSongName, false);
					theMusicInterface.QueueEvent("Play", "MainMenu", false);
				}
			}
			if (mContainer != null && mInterfaceState != 0)
			{
				mContainer.Show();
			}
			base.Show();
			mBJ3LogoShowing = false;
			mAllowFade = true;
			mY = 0;
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
		}

		public override void Hide()
		{
			HideLogo();
			base.Hide();
			SetVisible(false);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBEJ3_WIDGET_HIDE_CURVE, mAlphaCurve);
			mTargetPos = mFinalY;
		}

		public override void HideCompleted()
		{
			base.HideCompleted();
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME)
			{
				BejeweledLivePlusApp.UnloadContent("MainMenu");
			}
		}

		public override void InterfaceStateChanged(InterfaceState newState)
		{
			if (mContainer != null)
			{
				mContainer.InterfaceStateChanged(newState);
			}
			base.InterfaceStateChanged(newState);
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
			if (mContainer != null)
			{
				mContainer.LinkUpAssets();
			}
		}

		public override void PlayMenuMusic(bool isRestart = false)
		{
			if (!GlobalMembers.gApp.mMusicInterface.isPlayingUserMusic())
			{
				CustomBassMusicInterface theMusicInterface =
					(CustomBassMusicInterface)(GlobalMembers.gApp.mMusicInterface);
				
				if (mInterfaceState == InterfaceState.INTERFACE_STATE_LOADING)
				{
					// theMusicInterface.QueueEvent("Play", "LoadingScreen", false);
					// GlobalMembers.gApp.mMusic.PlaySongNoDelay(0, true);
				}
				else
				{
					// theMusicInterface.QueueEvent("FadeOut", "LoadingScreen", false);
					// theMusicInterface.QueueEvent("Play", "MainMenu", false);
					// GlobalMembers.gApp.mMusic.PlaySongNoDelay(1, true);
				}
			}
		}

		public void WantBuyGame()
		{
			mContainer.ButtonDepress(9);
		}
	}
}
