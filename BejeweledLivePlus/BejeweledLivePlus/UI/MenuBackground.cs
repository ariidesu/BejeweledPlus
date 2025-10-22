using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class MenuBackground : Bej3Widget
	{
		private enum NUM_CLOUD
		{
			NUMBER_OF_CLOUDS = 50
		}

		private bool mNeedTransition;

		private float mMainMenuAlpha;

		public MenuCloud[] mClouds = new MenuCloud[50];

		public MenuBackground()
			: base(Menu_Type.MENU_BACKGROUND, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			for (int i = 0; i < 50; i++)
			{
				mClouds[i] = new MenuCloud();
			}
			mMainMenuAlpha = 1f;
			Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mDoesSlideInFromBottom = (mCanAllowSlide = false);
			for (int j = 0; j < 50; j++)
			{
				float theX = Common.Rand(ConstantsWP.MAIN_MENU_WIDTH + ConstantsWP.MAIN_MENU_CLOUD_EXTRA_WIDTH * 2) - ConstantsWP.MAIN_MENU_CLOUD_EXTRA_WIDTH;
				float num = ConstantsWP.MAIN_MENU_CLOUD_Y + Common.Rand(GlobalMembers.gApp.mHeight - ConstantsWP.MAIN_MENU_CLOUD_Y);
				float mScale = (float)(Common.Rand(1000) % 1000) / 1000f;
				mClouds[j].mPosition = new FPoint(theX, num);
				mClouds[j].mScale = mScale;
				int theAlpha = (int)(255f * (1f - ((float)mHeight - num) / (float)(mHeight - ConstantsWP.MAIN_MENU_CLOUD_Y)));
				mClouds[j].mColour = new Color(255, 255, 255, theAlpha);
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(true, true);
			base.Dispose();
		}

		public override void Update()
		{
			base.Update();
			if (mState != Bej3WidgetState.STATE_IN || mInterfaceState == InterfaceState.INTERFACE_STATE_LOADING || GlobalMembersResourcesWP.IMAGE_MAIN_MENU_CLOUDS == null)
			{
				return;
			}
			for (int i = 0; i < 50; i++)
			{
				mClouds[i].mPosition.mX += mClouds[i].mScale * ConstantsWP.MAIN_MENU_CLOUD_SPEED;
				if (mClouds[i].mPosition.mX > (float)(mWidth + ConstantsWP.MAIN_MENU_CLOUD_EXTRA_WIDTH * 2))
				{
					mClouds[i].mPosition.mX = -ConstantsWP.MAIN_MENU_CLOUD_EXTRA_WIDTH - GlobalMembersResourcesWP.IMAGE_MAIN_MENU_CLOUDS.GetCelWidth();
				}
			}
		}

		public override void Draw(Graphics g)
		{
			g.SetColorizeImages(true);
			if (mInterfaceState == InterfaceState.INTERFACE_STATE_LOADING)
			{
				g.SetColor(Color.Black);
				g.FillRect(0, 0, mWidth, mHeight);
			}
			else
			{
				if ((mInterfaceState == InterfaceState.INTERFACE_STATE_HELPMENU && ((HelpDialog)GlobalMembers.gApp.mMenus[8]).mHelpDialogState == HelpDialog.HELPDIALOG_STATE.HELPDIALOG_STATE_INGAME) || (GlobalMembers.gApp.mGameInProgress && GlobalMembers.gApp.mCurrentGameMode != GameMode.MODE_DIAMOND_MINE))
				{
					return;
				}
				if (mInterfaceState != InterfaceState.INTERFACE_STATE_MAINMENU)
				{
					if (mMainMenuAlpha > 0f)
					{
						mMainMenuAlpha -= ConstantsWP.MENU_BACKGROUND_FADE_OUT_SPEED;
					}
				}
				else if (mMainMenuAlpha < 1f)
				{
					mMainMenuAlpha += ConstantsWP.MENU_BACKGROUND_FADE_IN_SPEED;
					if (mMainMenuAlpha > 1f)
					{
						mMainMenuAlpha = 1f;
					}
				}
				g.ClearClipRect();
				g.SetColor(new Color(255, 255, 255, 255));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_MAIN_MENU_BACKGROUND, 0, 0, mWidth, mHeight);
				if (GlobalMembersResourcesWP.IMAGE_MAIN_MENU_CLOUDS != null)
				{
					for (int i = 0; i < 50; i++)
					{
						g.SetColor(mClouds[i].mColour);
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_MAIN_MENU_CLOUDS, (int)mClouds[i].mPosition.mX, (int)mClouds[i].mPosition.mY);
					}
				}
				if (GlobalMembersResourcesWP.IMAGE_MAIN_MENU_PILLARL != null)
				{
					g.SetColor(Color.White);
				}
			}
		}

		public override void Show()
		{
			base.Show();
			mY = 0;
		}

		public override void Hide()
		{
			base.Hide();
			if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_DIAMOND_MINE)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eMENUBACKGROUND_HIDE_CURVE, mAlphaCurve);
			}
			mTargetPos = (mY = 0);
		}

		public override void LinkUpAssets()
		{
			base.LinkUpAssets();
		}

		public void Move(int x)
		{
			mX += x;
		}

		public override void InterfaceStateChanged(InterfaceState newState)
		{
			mNeedTransition = mInterfaceState == InterfaceState.INTERFACE_STATE_LOADING;
			base.InterfaceStateChanged(newState);
		}

		public override bool NeedsShowTransition()
		{
			return mNeedTransition;
		}

		public override int GetShowCurve()
		{
			if (mNeedTransition && mInterfaceState != 0)
			{
				return 26;
			}
			return base.GetShowCurve();
		}

		public override void PlayMenuMusic(bool isRestart = false)
		{
		}

		public void SetupForWelcome()
		{
			mAlphaCurve.SetConstant(1.0);
			GlobalMembers.gApp.mDialogObscurePct = 0.99f;
		}
	}
}
