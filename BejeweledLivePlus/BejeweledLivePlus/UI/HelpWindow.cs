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
	public class HelpWindow : SexyFramework.Widget.Widget
	{
		public List<PopAnim> mPopAnims = new List<PopAnim>();

		public List<string> mCaptions = new List<string>();

		public List<int> mXOfs = new List<int>();

		public List<float> mTextWidthScale = new List<float>();

		public string mHeaderText;

		public bool mSeenByUser;

		public static CurvedVal mHelpAlpha = new CurvedVal();

		public HelpWindow()
		{
			mSeenByUser = false;
		}

		public override void Dispose()
		{
			mPopAnims.Clear();
			RemoveAllWidgets(true, false);
		}

		public override void Update()
		{
			base.Update();
			if (mSeenByUser)
			{
				for (int i = 0; i < Common.size(mPopAnims); i++)
				{
					mPopAnims[i].Update();
				}
			}
			mHelpAlpha.IncInVal();
			MarkDirty();
		}

		public override void Draw(Graphics g)
		{
			int num = (int)((float)mWidth / ((float)Common.size(mPopAnims) + GlobalMembers.M(0.1f)));
			int val = 0;
			for (int i = 0; i < Common.size(mPopAnims); i++)
			{
				val = Math.Max(val, new Rect(0, 0, (int)((float)mPopAnims[i].mAnimRect.mWidth * ConstantsWP.HELPDIALOG_POPANIM_SCALE), (int)((float)mPopAnims[i].mAnimRect.mHeight * ConstantsWP.HELPDIALOG_POPANIM_SCALE)).mHeight);
			}
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, Bej3Widget.COLOR_DIALOG_WHITE);
			g.SetColor(mHelpAlpha);
			int hELPDIALOG_WINDOW_ANIMATION_Y_OFFSET = ConstantsWP.HELPDIALOG_WINDOW_ANIMATION_Y_OFFSET;
			for (int j = 0; j < Common.size(mPopAnims); j++)
			{
				float hELPDIALOG_POPANIM_SCALE = ConstantsWP.HELPDIALOG_POPANIM_SCALE;
				Rect rect = new Rect(0, 0, (int)((float)mPopAnims[j].mAnimRect.mWidth * hELPDIALOG_POPANIM_SCALE), (int)((float)mPopAnims[j].mAnimRect.mHeight * hELPDIALOG_POPANIM_SCALE));
				g.mClipRect.mY += ConstantsWP.HELPDIALOG_WINDOW_CLIP_OFFSET_TOP;
				Transform transform = new Transform();
				Rect rect2 = rect;
				rect2.Inflate(ConstantsWP.HELPDIALOG_WINDOW_BACKGROUND_EXTRA_SIZE, ConstantsWP.HELPDIALOG_WINDOW_BACKGROUND_EXTRA_SIZE);
				transform.Scale((float)rect2.mWidth / ConstantsWP.HELPDIALOG_WINDOW_BACKGROUND_SCALE, (float)rect2.mHeight / ConstantsWP.HELPDIALOG_WINDOW_BACKGROUND_SCALE);
				g.DrawImageTransformF(GlobalMembersResourcesWP.IMAGE_DIALOG_HELP_GLOW, transform, (float)num * ((float)j + GlobalMembers.M(0.56f)) + (float)mXOfs[j] + (float)ConstantsWP.HELPDIALOG_WINDOW_BACKGROUND_OFFSET, mHeight / 2 + hELPDIALOG_WINDOW_ANIMATION_Y_OFFSET);
				g.PushState();
				PopAnim popAnim = mPopAnims[j];
				g.Translate((int)((float)num * ((float)j + GlobalMembers.M(0.56f)) - (float)GlobalMembers.S(rect.mWidth / 2 + mXOfs[j])), mHeight / 2 - GlobalMembers.S(rect.mHeight / 2) + hELPDIALOG_WINDOW_ANIMATION_Y_OFFSET);
				Rect theRect = GlobalMembers.S(mPopAnims[j].mAnimRect);
				theRect.mX = (int)((float)theRect.mX * ConstantsWP.HELPDIALOG_POPANIM_CLIP_SCALE);
				theRect.mY = (int)((float)theRect.mY * ConstantsWP.HELPDIALOG_POPANIM_CLIP_SCALE);
				theRect.mWidth = (int)((float)theRect.mWidth * ConstantsWP.HELPDIALOG_POPANIM_CLIP_SCALE);
				theRect.mHeight = (int)((float)theRect.mHeight * ConstantsWP.HELPDIALOG_POPANIM_CLIP_SCALE);
				g.ClipRect(theRect);
				popAnim.mColor = mHelpAlpha;
				popAnim.Draw(g);
				g.ClearClipRect();
				g.PopState();
				g.SetFont(GlobalMembersResources.FONT_DIALOG);
				g.SetColor(Color.White);
				int theMaxWidth = 0;
				int theLineCount = 0;
				int wordWrappedHeight = g.GetWordWrappedHeight(ConstantsWP.HELPDIALOG_TEXT_WIDTH, mCaptions[j], -1, ref theMaxWidth, ref theLineCount);
				Rect theRect2 = new Rect(ConstantsWP.HELPDIALOG_TEXT_X, ConstantsWP.HELPDIALOG_TEXT_Y - wordWrappedHeight / 2, ConstantsWP.HELPDIALOG_TEXT_WIDTH, ConstantsWP.HELPDIALOG_TEXT_HEIGHT);
				g.WriteWordWrapped(theRect2, mCaptions[j], -1, 0);
			}
		}

		public override void SetVisible(bool isVisible)
		{
			base.SetVisible(isVisible);
			ResetAnimation();
		}

		public void ResetAnimation()
		{
			ResetAnimation(false);
		}

		public void ResetAnimation(bool fullReset)
		{
			for (int i = 0; i < Common.size(mPopAnims); i++)
			{
				mPopAnims[i].ResetAnim();
				mPopAnims[i].Play();
			}
		}

		public void PlayAnimation()
		{
			for (int i = 0; i < Common.size(mPopAnims); i++)
			{
				mPopAnims[i].ResetAnim();
				mPopAnims[i].Play();
			}
		}
	}
}
