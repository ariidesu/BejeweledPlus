using System;
using System.Collections.Generic;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class DigBoard : TimeLimitBoard
	{
		public string mMusicName = string.Empty;

		public int mMaxTicksLeft;

		public int mMegaTimeBonus;

		public CurvedVal mCvCrack = new CurvedVal();

		public RotatingCounter mRotatingCounter;

		public PopAnim mCogsAnim;

		public void TriggerDrillSmokeEffects()
		{
			int num = GetBoardY() + 800;
			Effect effect = Effect.alloc(Effect.Type.TYPE_STEAM);
			effect.mX = GetBoardX() - GlobalMembers.S(75);
			effect.mY = num - GlobalMembers.S(50);
			effect.mMinScale = 1f;
			effect.mMaxScale = 10f;
			effect.mScale = 5f;
			effect.mValue[0] = GlobalMembers.M(0.3f);
			effect.mValue[1] = GlobalMembers.M(-0.008f);
			effect.mValue[2] = GlobalMembers.M(0.95f);
			mPostFXManager.AddEffect(effect);
			int i = GetBoardX();
			int num2 = 40;
			for (int num3 = i + 800; i < num3; i += num2)
			{
				effect = Effect.alloc(Effect.Type.TYPE_STEAM);
				effect.mX = i;
				effect.mY = num + GlobalMembers.S(15);
				effect.mMinScale = 1f;
				effect.mMaxScale = 10f;
				effect.mScale = 2f;
				effect.mValue[0] = GlobalMembers.M(0.3f);
				effect.mValue[1] = GlobalMembers.M(-0.005f);
				effect.mValue[2] = GlobalMembers.M(0.95f);
				mPostFXManager.AddEffect(effect);
			}
			effect = Effect.alloc(Effect.Type.TYPE_STEAM);
			effect.mX = GetBoardX() + 800 + GlobalMembers.S(75);
			effect.mY = num - GlobalMembers.S(50);
			effect.mValue[0] = GlobalMembers.M(0.3f);
			effect.mValue[1] = GlobalMembers.M(-0.008f);
			effect.mValue[2] = GlobalMembers.M(0.95f);
			effect.mMinScale = 1f;
			effect.mMaxScale = 10f;
			effect.mScale = 5f;
			mPostFXManager.AddEffect(effect);
		}

		public void TriggerDrillDeathSmokeEffects()
		{
			int num = (GetBoardY() + 800) / 2;
			Effect effect = Effect.alloc(Effect.Type.TYPE_STEAM);
			effect.mX = GetBoardX() - GlobalMembers.S(75);
			effect.mY = num + GlobalMembers.S(175);
			effect.mMinScale = 1f;
			effect.mMaxScale = 10f;
			effect.mScale = 5f;
			effect.mValue[0] = GlobalMembers.M(0.3f);
			effect.mValue[1] = GlobalMembers.M(-0.002f);
			effect.mValue[2] = GlobalMembers.M(0.95f);
			mPostFXManager.AddEffect(effect);
			effect = Effect.alloc(Effect.Type.TYPE_STEAM);
			effect.mX = GetBoardX() - GlobalMembers.S(60);
			effect.mY = num - GlobalMembers.S(450);
			effect.mMinScale = 1f;
			effect.mMaxScale = 10f;
			effect.mScale = 3f;
			effect.mValue[0] = GlobalMembers.M(0.7f);
			effect.mValue[1] = GlobalMembers.M(-0.002f);
			effect.mValue[2] = GlobalMembers.M(0.95f);
			mPostFXManager.AddEffect(effect);
			effect = Effect.alloc(Effect.Type.TYPE_STEAM);
			effect.mX = GetBoardX() + 800 + GlobalMembers.S(75);
			effect.mY = num + GlobalMembers.S(175);
			effect.mValue[0] = GlobalMembers.M(0.3f);
			effect.mValue[1] = GlobalMembers.M(-0.002f);
			effect.mValue[2] = GlobalMembers.M(0.95f);
			effect.mMinScale = 1f;
			effect.mMaxScale = 10f;
			effect.mScale = 5f;
			mPostFXManager.AddEffect(effect);
			effect = Effect.alloc(Effect.Type.TYPE_STEAM);
			effect.mX = GetBoardX() + 800 - GlobalMembers.S(60);
			effect.mY = num - GlobalMembers.S(450);
			effect.mMinScale = 1f;
			effect.mMaxScale = 10f;
			effect.mScale = 3f;
			effect.mValue[0] = GlobalMembers.M(0.7f);
			effect.mValue[1] = GlobalMembers.M(-0.002f);
			effect.mValue[2] = GlobalMembers.M(0.95f);
			mPostFXManager.AddEffect(effect);
		}

		public DigBoard()
		{
			mMusicName = "BuriedTreasure";
			mMaxTicksLeft = 0;
			mRotatingCounter = new RotatingCounter(0, new Rect(ConstantsWP.DIG_BOARD_DEPTH_METER_CLIP_X, ConstantsWP.DIG_BOARD_DEPTH_METER_CLIP_Y, ConstantsWP.DIG_BOARD_DEPTH_METER_CLIP_W, ConstantsWP.DIG_BOARD_DEPTH_METER_CLIP_H), GlobalMembersResources.FONT_DIALOG);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_BOARD_CV_CRACK, mCvCrack);
			mCogsAnim = null;
		}

		public override void Dispose()
		{
			if (mCogsAnim != null)
			{
				mCogsAnim.Dispose();
				mCogsAnim = null;
			}
			if (mRotatingCounter != null)
			{
				mRotatingCounter.Dispose();
				mRotatingCounter = null;
			}
			base.Dispose();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_Dig");
			BejeweledLivePlusApp.UnloadContent("GamePlay_UI_Dig");
		}

		public override void LoadContent(bool threaded)
		{
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlay_UI_Dig");
				BejeweledLivePlusApp.LoadContentInBackground("GamePlayQuest_Dig");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlay_UI_Dig");
				BejeweledLivePlusApp.LoadContent("GamePlayQuest_Dig");
				if (mCogsAnim == null)
				{
					mCogsAnim = GlobalMembersResourcesWP.POPANIM_QUEST_DIG_COGS.Duplicate();
					mCogsAnim.mClip = true;
					mCogsAnim.Play("IDLE");
				}
			}
			base.LoadContent(threaded);
		}

		public override string GetExtraGameOverLogParams()
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			if (mIsPerpetual)
			{
				return $"distGold={digGoal.mTreasureEarnings[0]} distGems={digGoal.mTreasureEarnings[1]} distArtifacts={digGoal.mTreasureEarnings[2]} depth={digGoal.GetGridDepth()}";
			}
			return string.Empty;
		}

		public override float GetRankPointMultiplier()
		{
			return 2.6666667f;
		}

		public override int GetTicksLeft()
		{
			int ticksLeft = base.GetTicksLeft();
			if (mMaxTicksLeft == 0)
			{
				return ticksLeft;
			}
			return Math.Min(mMaxTicksLeft, ticksLeft);
		}

		public override void GameOver(bool visible)
		{
			mPoints = mLevelPointsTotal;
			bool flag = mGameOverCount > 0;
			base.GameOver(visible);
			if (mIsPerpetual && !flag && mGameOverCount > 0)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_DEATH, 0, GlobalMembers.M(1.0));
				(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("FadeOut", $"{GetMusicName()}", false);
				(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("Play", $"{GetMusicName()}_lose", true);
				// GlobalMembers.gApp.mMusic.PlaySongNoDelay(14, false);
			}
		}

		public override void DrawCountdownBar(Graphics g)
		{
			if (mOffsetY != 0)
			{
				g.Translate(0, mOffsetY);
			}
			if (mIsPerpetual)
			{
				g.SetColorizeImages(true);
				float num = (float)Math.Pow(GetBoardAlpha(), 4.0);
				g.SetColor(new Color(255, 255, 255, (int)(GetBoardAlpha() * (float)GlobalMembers.M(255))));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_BACK, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_BACK_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_BACK_ID)));
				g.SetColor(new Color(GlobalMembers.M(64), GlobalMembers.M(32), GlobalMembers.M(8), (int)(num * (float)GlobalMembers.M(255))));
				if (WantWarningGlow())
				{
					Color warningGlowColor = GetWarningGlowColor();
					if (warningGlowColor.mAlpha > 0)
					{
						g.PushState();
						g.SetDrawMode(Graphics.DrawMode.Additive);
						g.SetColor(warningGlowColor);
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME_ID)));
						g.PopState();
					}
				}
				Rect countdownBarRect = GetCountdownBarRect();
				countdownBarRect.mX -= GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_TIMER.mWidth / 2;
				countdownBarRect.mWidth = (int)(mCountdownBarPct * (float)countdownBarRect.mWidth + (float)mLevelBarSizeBias);
				g.FillRect(countdownBarRect);
				if ((double)mLevelBarBonusAlpha > 0.0)
				{
					Rect countdownBarRect2 = GetCountdownBarRect();
					countdownBarRect2.mX -= GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_TIMER.mWidth / 2;
					countdownBarRect2.mWidth = (int)((float)countdownBarRect2.mWidth * GetLevelPct());
					g.SetColor(new Color(GlobalMembers.M(240), GlobalMembers.M(255), 200, (int)((double)mLevelBarBonusAlpha * (double)GlobalMembers.M(255))));
					g.FillRect(countdownBarRect2);
				}
				Graphics3D graphics3D = g.Get3D();
				SexyTransform2D mDrawTransform = mCountdownBarPIEffect.mDrawTransform;
				Rect mClipRect = g.mClipRect;
				if (graphics3D != null)
				{
					countdownBarRect.Scale((float)(double)mScale, (float)(double)mScale, (int)GlobalMembers.S(ConstantsWP.DEVICE_HEIGHT_F), (int)GlobalMembers.S(ConstantsWP.DEVICE_WIDTH_F));
					mCountdownBarPIEffect.mDrawTransform.Translate(GlobalMembers.S(0f - ConstantsWP.DEVICE_HEIGHT_F), GlobalMembers.S(0f - ConstantsWP.DEVICE_WIDTH_F));
					mCountdownBarPIEffect.mDrawTransform.Scale((float)(double)mScale, (float)(double)mScale);
					mCountdownBarPIEffect.mDrawTransform.Translate(GlobalMembers.S(ConstantsWP.DEVICE_HEIGHT_F), GlobalMembers.S(ConstantsWP.DEVICE_WIDTH_F));
				}
				int num2 = (int)((double)GetAlpha() * (double)mAlphaCurve * (double)GlobalMembers.M(255));
				if (num2 == 255)
				{
					g.SetClipRect(countdownBarRect);
					g.SetColor(new Color(255, 255, 255, (int)((double)GetAlpha() * (double)mAlphaCurve * (double)GlobalMembers.M(255))));
					g.PushColorMult();
					mCountdownBarPIEffect.mColor = new Color(255, 255, 255, (int)((double)GetAlpha() * (double)mAlphaCurve * (double)GlobalMembers.M(255)));
					mCountdownBarPIEffect.Draw(g);
					mCountdownBarPIEffect.mDrawTransform = mDrawTransform;
					g.PopColorMult();
					g.SetColor(Color.White);
					g.SetClipRect(mClipRect);
				}
			}
			else
			{
				g.Translate(0, GlobalMembers.S(-mBoardUIOffsetY));
				base.DrawCountdownBar(g);
				g.Translate(0, GlobalMembers.S(mBoardUIOffsetY));
			}
			if (mOffsetY != 0)
			{
				g.Translate(0, -mOffsetY);
			}
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			if (mIsPerpetual)
			{
				if (digGoal.mAllClearAnimPlayed)
				{
					digGoal.DrawDigBarAnim(g, true);
				}
				else if (digGoal.mClearedAnimPlayed)
				{
					digGoal.DrawDigBarAnim(g, false);
				}
			}
			base.DrawOverlay(g, thePriority);
		}

		public override void RefreshUI()
		{
			if (mUiConfig == EUIConfig.eUIConfig_Standard || mUiConfig == EUIConfig.eUIConfig_StandardNoReplay)
			{
				mHintButton.SetOverlayType(Bej3Button.BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_DIAMOND_MINE);
				mHintButton.Resize(ConstantsWP.BOARD_UI_HINT_BTN_X, ConstantsWP.DIGBOARD_UI_HINT_BTN_Y, ConstantsWP.BOARD_UI_HINT_BTN_WIDTH, 0);
				mHintButton.mHasAlpha = true;
				mHintButton.mDoFinger = true;
				mHintButton.mOverAlphaSpeed = 0.1;
				mHintButton.mOverAlphaFadeInSpeed = 0.2;
				mHintButton.mWidgetFlagsMod.mRemoveFlags |= 4;
				mHintButton.mDisabled = false;
			}
			else if (mUiConfig == EUIConfig.eUIConfig_WithReset || mUiConfig == EUIConfig.eUIConfig_WithResetAndReplay)
			{
				mHintButton.Resize(ConstantsWP.BOARD_UI_HINT_BTN_X, ConstantsWP.BOARD_UI_HINT_BTN_Y, ConstantsWP.BOARD_UI_HINT_BTN_WIDTH, 0);
				mHintButton.mHasAlpha = true;
				mHintButton.mDoFinger = true;
				mHintButton.mOverAlphaSpeed = 0.1;
				mHintButton.mOverAlphaFadeInSpeed = 0.2;
				mHintButton.mWidgetFlagsMod.mRemoveFlags |= 4;
				mHintButton.mDisabled = false;
			}
		}

		public override void DrawUI(Graphics g)
		{
		}

		public override void DrawWarningHUD(Graphics g)
		{
			g.PushState();
			Color color = g.GetColor();
			g.SetDrawMode(Graphics.DrawMode.Additive);
			Color warningGlowColor = GetWarningGlowColor();
			g.SetColor(warningGlowColor);
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(mOffsetX, mOffsetY);
			}
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_METER, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_METER_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_METER_ID)));
			if (WantTopFrame())
			{
				if (WantTopLevelBar() || GetTimeLimit() > 0)
				{
					if (mOffsetY != 0)
					{
						g.Translate(-mOffsetX, mOffsetY);
					}
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME_ID)));
					Rect countdownBarRect = GetCountdownBarRect();
					if (warningGlowColor.mAlpha > 0)
					{
						Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_TIMER, GetTimeDrawX(), countdownBarRect.mY + countdownBarRect.mHeight / 2);
					}
					if (mOffsetY != 0)
					{
						g.Translate(mOffsetX, -mOffsetY);
					}
				}
				else
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)));
				}
			}
			if (WantBottomFrame())
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)));
			}
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColor(color);
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(-mOffsetX, -mOffsetY);
			}
			g.PopState();
		}

		public override void DrawHUDText(Graphics g)
		{
			g.PushState();
			Color color = g.GetColor();
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(mOffsetX, mOffsetY);
			}
			mRotatingCounter.Draw(g, 0f);
			float num = GlobalMembers.S(-1f);
			g.SetColor(new Color(0));
			g.SetFont(mRotatingCounter.mFont);
			g.WriteString("m", (int)Utils.Round((float)ConstantsWP.DIG_BOARD_DEPTH_METER_SYMBOL_X + num), ConstantsWP.DIG_BOARD_DEPTH_METER_SYMBOL_Y);
			if (mIsPerpetual)
			{
				DigGoal digGoal = (DigGoal)mQuestGoal;
				g.SetFont(GlobalMembersResources.FONT_SCORE);
				g.SetColor(new Color(GlobalMembers.M(16776960)));
				g.WriteString($"{SexyFramework.Common.CommaSeperate(mLevelPointsTotal)}", ConstantsWP.DIG_BOARD_SCORE_CENTER_X, ConstantsWP.DIG_BOARD_SCORE_BTM_Y);
				if (digGoal.mTextFlashTicks > 0 && digGoal.mTextFlashTicks / GlobalMembers.M(20) % GlobalMembers.M(2) == 1)
				{
					g.SetColor(new Color(GlobalMembers.M(16755200)));
					g.WriteString($"{SexyFramework.Common.CommaSeperate(mLevelPointsTotal)}", ConstantsWP.DIG_BOARD_SCORE_CENTER_X, ConstantsWP.DIG_BOARD_SCORE_BTM_Y);
				}
			}
			((ImageFont)mRotatingCounter.mFont).PopLayerColor(0);
			if (WantDrawTimer())
			{
				DrawTimer(g);
			}
			g.SetColor(color);
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(-mOffsetX, -mOffsetY);
			}
			g.PopState();
		}

		public override void DrawBottomFrame(Graphics g)
		{
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(mOffsetX, mOffsetY);
			}
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_LEVEL, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_LEVEL_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_LEVEL_ID)));
			if (WantBottomFrame())
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)));
			}
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(-mOffsetX, -mOffsetY);
			}
		}

		public override void DrawTopFrame(Graphics g)
		{
			if (mOffsetY != 0)
			{
				g.Translate(0, mOffsetY);
			}
			if (WantTopFrame())
			{
				if (WantCountdownBar() || WantTopLevelBar())
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_FRAME_ID)));
					Rect countdownBarRect = GetCountdownBarRect();
					Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_TIMER, GetTimeDrawX(), countdownBarRect.mY + countdownBarRect.mHeight / 2);
				}
				else
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BOARD_SEPERATOR_FRAME_ID)));
				}
			}
			if (mOffsetY != 0)
			{
				g.Translate(0, -mOffsetY);
			}
		}

		public override void DrawHUD(Graphics g)
		{
			g.SetDrawMode(Graphics.DrawMode.Normal);
			g.SetColorizeImages(true);
			g.SetColor(new Color(255, 255, 255, (int)(255f * GetAlpha())));
			DrawBackgroundElements(g);
			DrawDepthMeter(g);
			DrawFrame(g);
		}

		public void DrawBackgroundElements(Graphics g)
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			Rect mClipRect = g.mClipRect;
			int num = 100;
			g.SetClipRect(GlobalMembers.S((int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND_ID)), GlobalMembers.S((int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND_ID)), GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND.mWidth, GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND.mHeight + num);
			int num2 = (int)GlobalMembers.S(digGoal.GetGridDepthAsDouble() * 100.0);
			int num3 = GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND.mHeight;
			int num4 = 0;
			if (num2 >= num3)
			{
				num4 = num2 / num3;
			}
			if (num2 % num3 > 0)
			{
				num4++;
			}
			g.SetScale(1f, 1.2f, 0f, 0f);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND, (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND_ID)) + 0f), (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND_ID)) - (float)num2));
			for (int i = 1; i <= num4; i++)
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND, (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND_ID)) + 0f), (int)(GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND_ID)) + (float)(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_BACKGROUND.mHeight * i - num2)));
			}
			g.SetClipRect(mClipRect);
			g.SetScale(1f, 1f, 0f, 0f);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_HUD_SHADOW, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_HUD_SHADOW_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_HUD_SHADOW_ID)));
			if (mOffsetY != 0)
			{
				g.Translate(0, mOffsetY);
			}
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_SCORE_BAR_BACK, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_SCORE_BAR_BACK_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_SCORE_BAR_BACK_ID)));
			if (mOffsetY != 0)
			{
				g.Translate(0, -mOffsetY);
			}
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(mOffsetX, mOffsetY);
			}
			DrawCogs(g);
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(-mOffsetX, -mOffsetY);
			}
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			if (mCurrentHint != null)
			{
				mCurrentHint.Draw(g);
			}
		}

		public void DrawDepthMeter(Graphics g)
		{
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(mOffsetX, mOffsetY);
			}
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_METER, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_METER_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_DIAMOND_MINE_DEPTH_METER_ID)));
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(-mOffsetX, -mOffsetY);
			}
		}

		public void DrawCogs(Graphics g)
		{
			mCogsAnim.Draw(g);
			g.SetColorizeImages(true);
		}

		public void SetCogsAnim(bool on)
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			if (on && string.Compare(mCogsAnim.mLastPlayedFrameLabel, "DRILL", StringComparison.OrdinalIgnoreCase) != 0)
			{
				mCogsAnim.Play("DRILL");
			}
			else if (GetTicksLeft() == 0 && GetTimeLimit() > 0 && string.Compare(mCogsAnim.mLastPlayedFrameLabel, "DEATH", StringComparison.OrdinalIgnoreCase) != 0 && digGoal != null && !digGoal.mDigInProgress && IsBoardStill())
			{
				mCogsAnim.Play("DEATH");
			}
			else if (!on && (GetTicksLeft() != 0 || GetTimeLimit() <= 0) && string.Compare(mCogsAnim.mLastPlayedFrameLabel, "IDLE", StringComparison.OrdinalIgnoreCase) != 0)
			{
				mCogsAnim.Play("IDLE");
			}
		}

		public void UpdateCogsAnim()
		{
			mCogsAnim.Update();
		}

		public void DoTimeBonus(bool theIsMega)
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			TextNotifyEffect textNotifyEffect = TextNotifyEffect.alloc();
			textNotifyEffect.mDuration = GlobalMembers.M(200);
			int num;
			if (theIsMega)
			{
				num = mMegaTimeBonus;
				textNotifyEffect.mText = string.Format(GlobalMembers._ID("+{0} SECOND MEGA BONUS", 170), num);
			}
			else
			{
				num = mTimeBonus;
				textNotifyEffect.mText = string.Format(GlobalMembers._ID("+{0} SECOND BONUS", 171), num);
			}
			textNotifyEffect.mFont = GlobalMembersResources.FONT_HEADER;
			int num2 = textNotifyEffect.mFont.StringWidth(textNotifyEffect.mText);
			if (num2 > GlobalMembers.gApp.mWidth)
			{
				float num3 = num2 - GlobalMembers.gApp.mWidth;
				textNotifyEffect.mScale = Math.Max(1f - num3 / (float)GlobalMembers.gApp.mWidth, 0.1f);
			}
			textNotifyEffect.mX = GlobalMembers.RS(mWidth / 2);
			textNotifyEffect.mY = GlobalMembers.RS(ConstantsWP.DIG_BOARD_TIME_BONUS_TEXT_Y - textNotifyEffect.mFont.GetHeight() / 2);
			digGoal.mMessageFXManager.AddEffect(textNotifyEffect);
			mTimeLimit += num;
			int ticksLeft = base.GetTicksLeft();
			if (ticksLeft <= 9000)
			{
				mMaxTicksLeft = ticksLeft;
				mTimeLimit = 90;
				mGameTicks = 9000 - ticksLeft + GlobalMembers.M(250);
			}
			else
			{
				mMaxTicksLeft = ticksLeft;
				mTimeLimit = (ticksLeft + 99) / 100;
				mGameTicks = 250;
			}
		}

		public override void BlanksFilled(bool specialDropped)
		{
			base.BlanksFilled(specialDropped);
			if (mIsPerpetual && mColorCount == 4)
			{
				SetColorCount(5);
			}
		}

		public override void UpdateFalling()
		{
			base.UpdateFalling();
			DigGoal digGoal = (DigGoal)mQuestGoal;
			if (!digGoal.mInitPushAnimCv.IsDoingCurve())
			{
				return;
			}
			foreach (int key in digGoal.mIdToTileData.Keys)
			{
				Piece pieceById = GetPieceById(key);
				if (pieceById != null)
				{
					pieceById.mFallVelocity = 0f;
				}
			}
		}

		public override bool WantDrawBackground()
		{
			return !mIsPerpetual;
		}

		public override bool WantHypermixerBottomCheck()
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			return digGoal.mNextBottomHypermixerWait == 0;
		}

		public override bool TryingDroppedPieces(List<Piece> thePieceVector, int theTryCount)
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			int[] array = new int[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
			for (int i = 0; i < Common.size(thePieceVector); i++)
			{
				Piece piece = thePieceVector[i];
				int num = array[piece.mCol]--;
				if (num < 0)
				{
					continue;
				}
				DigGoal.OldPieceData oldPieceData = digGoal.mOldPieceData[num, piece.mCol];
				if (oldPieceData.mFlags != -1 && piece.mFlags == 0 && (theTryCount <= i || (oldPieceData.mFlags != 0 && theTryCount < 100)))
				{
					piece.mFlags = (int)((long)oldPieceData.mFlags & 7L);
					if (oldPieceData.mColor != -1 || ((ulong)oldPieceData.mFlags & 2uL) != 0)
					{
						piece.mColor = oldPieceData.mColor;
					}
				}
			}
			return true;
		}

		public override bool PiecesDropped(List<Piece> thePieceVector)
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			for (int i = 0; i < Common.size(thePieceVector); i++)
			{
				Piece piece = thePieceVector[i];
				digGoal.mOldPieceData[1, piece.mCol] = digGoal.mOldPieceData[0, piece.mCol];
				digGoal.mOldPieceData[0, piece.mCol].mColor = -1;
				digGoal.mOldPieceData[0, piece.mCol].mFlags = -1;
			}
			return true;
		}

		public override void SwapSucceeded(SwapData theSwapData)
		{
			base.SwapSucceeded(theSwapData);
			DigGoal digGoal = (DigGoal)mQuestGoal;
			if (digGoal.mNextBottomHypermixerWait > 0)
			{
				digGoal.mNextBottomHypermixerWait--;
			}
		}

		public override void RemoveFromPieceMap(int theId)
		{
			base.RemoveFromPieceMap(theId);
			DigGoal digGoal = (DigGoal)mQuestGoal;
			for (int i = 0; i < Common.size(digGoal.mMovingPieces); i++)
			{
				if (digGoal.mMovingPieces[i] != null && digGoal.mMovingPieces[i].mId == theId)
				{
					digGoal.mMovingPieces[i] = null;
				}
			}
		}

		public override void HypermixerDropped()
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			digGoal.mNextBottomHypermixerWait = GlobalMembers.M(10);
		}

		public override void KeyChar(char theChar)
		{
			if (GlobalMembers.gApp.mDebugKeysEnabled)
			{
				DigGoal digGoal = (DigGoal)mQuestGoal;
				base.KeyChar(theChar);
			}
		}

		public override string GetMusicName()
		{
			return mMusicName;
		}

		public override void Update()
		{
			base.Update();
			if (mQuestGoal != null)
			{
				DigGoal digGoal = (DigGoal)mQuestGoal;
				digGoal.ResyncInitPushAnim();
			}
			mRotatingCounter.Update();
		}

		public override bool WantFreezeAutoplay()
		{
			DigGoal digGoal = (DigGoal)mQuestGoal;
			if (!base.WantFreezeAutoplay())
			{
				return digGoal.CheckNeedScroll(true);
			}
			return true;
		}

		public override bool WantsCalmEffects()
		{
			return false;
		}

		public override void Init()
		{
			if (mIsPerpetual)
			{
				mUiConfig = EUIConfig.eUIConfig_StandardNoReplay;
				mLevelBarSizeBias = GlobalMembers.MS(24);
			}
			base.Init();
			if (mIsPerpetual)
			{
				SetColorCount(4);
			}
			mMaxTicksLeft = 0;
		}

		public override void NewGame(bool restartingGame)
		{
			base.NewGame(restartingGame);
			if (mParams.ContainsKey("MegaTimeBonus"))
			{
				mMegaTimeBonus = SexyFramework.GlobalMembers.sexyatoi(mParams["MegaTimeBonus"]);
			}
			if (mIsPerpetual)
			{
				DigGoal digGoal = (DigGoal)mQuestGoal;
				mPowerGemThreshold = digGoal.mPowerGemThresholdDepth0;
			}
			Bej3Widget.SetOverlayType(OVERLAY_TYPE.OVERLAY_RUSTY);
		}

		public override void PlayMenuMusic(bool isRestart = false)
		{
			if (mGameOverCount == 0)
			{
				// GlobalMembers.gApp.mMusic.PlaySongNoDelay(13, true);
				CustomBassMusicInterface theMusicInterface =
					(CustomBassMusicInterface)(GlobalMembers.gApp.mMusicInterface);
				if (isRestart || (theMusicInterface.mSongName != GetMusicName() &&
				                  theMusicInterface.mSongName != $"{GetMusicName()}_lose"))
				{
					theMusicInterface.QueueEvent("FadeOut", theMusicInterface.mSongName, false);
					theMusicInterface.QueueEvent("Play", GetMusicName(), true);
				}
			}
		}

		public override float GetGravityFactor()
		{
			return GlobalMembers.M(1.1f);
		}

		public override bool IsGameIdle()
		{
			return mQuestGoal.IsGameIdle();
		}

		public override int GetTimerYOffset()
		{
			return ConstantsWP.DIG_BOARD_TIMER_OFFSET_Y;
		}
	}
}
