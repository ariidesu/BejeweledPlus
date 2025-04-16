using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class UnderDialogWidget : Bej3Widget
	{
		public enum WIDGET_FADE
		{
			UNDERDIALOG_WIDGET_FADE = 160
		}

		public DeviceImage mShrunkScreen1;

		public DeviceImage mShrunkScreen2;

		public UnderDialogWidget()
			: base(Menu_Type.MENU_FADE_UNDERLAY, false, Bej3ButtonType.TOP_BUTTON_TYPE_NONE)
		{
			mCanAllowSlide = false;
			mMouseVisible = false;
			mHasAlpha = true;
			mShrunkScreen1 = null;
			mShrunkScreen2 = null;
			mClip = false;
			mUpdateWhenNotVisible = true;
		}

		public override void Dispose()
		{
			if (mShrunkScreen1 != null)
			{
				mShrunkScreen1.Dispose();
			}
			if (mShrunkScreen2 != null)
			{
				mShrunkScreen2.Dispose();
			}
			base.Dispose();
		}

		public void CreateImages()
		{
		}

		public void DrawPaused(Graphics g)
		{
		}

		public override void Update()
		{
			base.Update();
			mY = (mFinalY = 0);
			if (GlobalMembers.gApp.mDialogObscurePct > 0f && SexyFramework.GlobalMembers.gSexyAppBase.mHasFocus)
			{
				MarkDirty();
			}
		}

		public override void Draw(Graphics g)
		{
			mY = (mX = 0);
			g.mTransX = (g.mTransY = 0f);
			g.SetColor(GetFadeColor());
			g.mPushedColorVector.RemoveAt(0);
			GlobalMembers.gApp.mWidgetManager.FlushDeferredOverlayWidgets(int.MaxValue);
			g.SetColorizeImages(true);
			g.FillRect(SexyFramework.GlobalMembers.gSexyAppBase.mScreenBounds);
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			base.DrawAll(theFlags, g);
		}

		public override void PlayMenuMusic()
		{
		}

		public override void Show()
		{
			base.SetVisible(true);
		}

		public override void Hide()
		{
			base.SetVisible(false);
		}

		public override void AllowSlideIn(bool allow, Bej3Button previousTopButton)
		{
			base.SetVisible(true);
		}

		public static Color GetFadeColor()
		{
			return new Color(0, 0, 0, (int)(GlobalMembers.gApp.mDialogObscurePct * 160f));
		}
	}
}
