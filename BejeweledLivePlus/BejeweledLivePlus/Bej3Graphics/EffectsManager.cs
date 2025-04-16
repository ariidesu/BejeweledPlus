using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class EffectsManager : SexyFramework.Widget.Widget, IDisposable
	{
		public Board mBoard;

		public bool mDeferOverlay;

		public List<Effect>[] mEffectList = new List<Effect>[24];

		public List<DistortEffect> mDistortEffectList = new List<DistortEffect>();

		public bool mApplyBoardTransformToDraw;

		public bool mDisableMask;

		public bool mDoDistorts;

		public bool mIs3d;

		public SharedRenderTarget mHeightImageSharedRT = new SharedRenderTarget();

		public bool mHeightImageDirty;

		public bool mRewindEffect;

		public float mAlpha;

		public new int mUpdateCnt;

		private Transform DrawTypeEmberTrans = new Transform();

		private static Transform DD_aTrans = new Transform();

		public EffectsManager(Board theBoard, bool deferOverlay)
		{
			mBoard = theBoard;
			mDeferOverlay = deferOverlay;
			mApplyBoardTransformToDraw = false;
			mDisableMask = false;
			mIs3d = false;
			mAlpha = 1f;
			mUpdateCnt = 0;
			mX = (mY = 0);
			mClip = false;
			mMouseVisible = false;
			mHeightImageDirty = false;
			mHasAlpha = true;
			mRewindEffect = false;
			for (int i = 0; i < 24; i++)
			{
				mEffectList[i] = new List<Effect>();
			}
		}

		public EffectsManager(Board theBoard)
			: this(theBoard, false)
		{
		}

		public override void Dispose()
		{
			Clear();
			base.Dispose();
		}

		public void CreateDistortionMap()
		{
			DeviceImage deviceImage = mHeightImageSharedRT.GetCurrentLockImage();
			if (deviceImage != null && (deviceImage.mWidth != GlobalMembers.gApp.mScreenBounds.mWidth / 4 || deviceImage.mHeight != GlobalMembers.gApp.mScreenBounds.mHeight / 4))
			{
				mHeightImageSharedRT.Unlock();
				deviceImage = null;
			}
			if (deviceImage == null && SexyFramework.GlobalMembers.gIs3D)
			{
				mHeightImageSharedRT.Lock(GlobalMembers.gApp.mScreenBounds.mWidth / 4, GlobalMembers.gApp.mScreenBounds.mHeight / 4, 8u, "HeightImage");
				ClearDistortionMap(null);
			}
		}

		public void ClearDistortionMap(Graphics g)
		{
		}

		public DeviceImage GetHeightImage()
		{
			CreateDistortionMap();
			return mHeightImageSharedRT.GetCurrentLockImage();
		}

		public virtual void UpdateTypeEmber(int type)
		{
			for (int i = 0; i < mEffectList[type].Count; i++)
			{
				Effect effect = mEffectList[type][i];
				effect.mX += effect.mDX;
				effect.mY += effect.mDY;
				effect.mZ += effect.mDZ;
				effect.mDY += effect.mGravity;
				effect.mDX *= effect.mDXScalar;
				effect.mDY *= effect.mDYScalar;
				effect.mDZ *= effect.mDZScalar;
				if ((effect.mFlags & 4) != 0 && effect.mDelay > 0f)
				{
					effect.mDelay -= 0.01f;
					if (effect.mDelay <= 0f)
					{
						effect.mDelay = 0f;
					}
				}
				else
				{
					effect.mAlpha += effect.mDAlpha;
					effect.mScale += effect.mDScale;
				}
				effect.mScale += effect.mDScale;
				effect.mAngle += effect.mDAngle;
				if (effect.mType == Effect.Type.TYPE_EMBER_FADEINOUT || effect.mType == Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM)
				{
					effect.mAlpha += effect.mDAlpha;
					if (effect.mAlpha >= 1f)
					{
						effect.mDeleteMe = true;
					}
					effect.mFrame = (int)(12f * effect.mAlpha);
				}
				else if (effect.mColor.mRed + effect.mColor.mGreen == 0)
				{
					effect.mScale += 0.02f;
					effect.mAlpha -= 0.01f;
					if (effect.mAlpha <= 0f)
					{
						effect.mDeleteMe = true;
					}
				}
				if (effect.mScale < effect.mMinScale)
				{
					effect.mDeleteMe = true;
					effect.mScale = effect.mMinScale;
				}
				else if (effect.mScale > effect.mMaxScale)
				{
					effect.mScale = effect.mMaxScale;
					if ((effect.mFlags & 1) != 0)
					{
						effect.mDScale = 0f - effect.mDScale;
					}
				}
				if (effect.mAlpha > effect.mMaxAlpha)
				{
					effect.mAlpha = effect.mMaxAlpha;
					if ((effect.mFlags & 2) != 0)
					{
						effect.mDAlpha = 0f - effect.mDAlpha;
					}
					else
					{
						effect.mDeleteMe = true;
					}
				}
				else if (effect.mAlpha <= 0f)
				{
					effect.mDeleteMe = true;
				}
			}
		}

		public virtual void DrawTypeEmber(Effect.Type type, Graphics g, bool isOverlay)
		{
			float num = mAlpha;
			if (mBoard != null)
			{
				num *= GlobalMembers.gApp.mBoard.GetAlpha();
			}
			int num2 = 1;
			if (type == Effect.Type.TYPE_EMBER_BOTTOM || type == Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM)
			{
				num2 = 0;
			}
			g.SetDrawMode((num2 != 0) ? 1 : 0);
			for (int i = 0; i < mEffectList[(int)type].Count; i++)
			{
				Effect effect = mEffectList[(int)type][i];
				if (effect.mOverlay != isOverlay || effect.mDeleteMe)
				{
					continue;
				}
				Piece mPieceRel = effect.mPieceRel;
				if (mPieceRel != null && (double)mPieceRel.mAlpha <= 0.0)
				{
					continue;
				}
				int num3 = 0;
				int num4 = 0;
				if (mPieceRel != null)
				{
					num3 = (int)mPieceRel.GetScreenX();
					num4 = (int)mPieceRel.GetScreenY();
					effect.mX += num3;
					effect.mY += num4;
				}
				float num5 = Math.Min(1f, effect.mAlpha) * num;
				g.SetColorizeImages(true);
				Color mColor = effect.mColor;
				mColor.mAlpha = (int)(200f * num5);
				g.SetColor(mColor);
				if (type == Effect.Type.TYPE_EMBER_FADEINOUT || type == Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM)
				{
					float num6 = Math.Min(63f, 64f * effect.mAlpha);
					Color mColor2 = effect.mColor;
					mColor2.mAlpha = (int)((float)mColor2.mAlpha * mAlpha);
					Transform drawTypeEmberTrans = DrawTypeEmberTrans;
					drawTypeEmberTrans.Reset();
					Rect rect = default(Rect);
					drawTypeEmberTrans.Scale(effect.mScale, effect.mScale);
					if (effect.mAngle != 0f)
					{
						drawTypeEmberTrans.RotateRad(effect.mAngle);
					}
					g.SetColor(mColor2);
					rect = effect.mImage.GetCelRect((int)num6);
					GlobalMembers.gGR.DrawImageTransformF(g, effect.mImage, drawTypeEmberTrans, rect, GlobalMembers.S(effect.mX), GlobalMembers.S(effect.mY));
				}
				else
				{
					Transform drawTypeEmberTrans2 = DrawTypeEmberTrans;
					drawTypeEmberTrans2.Reset();
					drawTypeEmberTrans2.Scale(effect.mScale, effect.mScale);
					drawTypeEmberTrans2.RotateRad(effect.mAngle);
					GlobalMembers.gGR.DrawImageTransformF(g, effect.mImage, drawTypeEmberTrans2, new Rect(0, 0, effect.mImage.mWidth, effect.mImage.mHeight), GlobalMembers.S(effect.mX), GlobalMembers.S(effect.mY));
				}
				if (mPieceRel != null)
				{
					effect.mX -= num3;
					effect.mY -= num4;
				}
			}
		}

		public virtual void GarbageCollectEffects(int type)
		{
			int count = mEffectList[type].Count;
			for (int num = count - 1; num >= 0; num--)
			{
				Effect effect = mEffectList[type][num];
				if (effect.mDeleteMe)
				{
					FreeEffect(effect);
					mEffectList[type].RemoveAt(num);
				}
			}
		}

		public override void Update()
		{
			base.Update();
			if (mBoard != null && (mBoard.mBoardHidePct == 1f || mBoard.mSuspendingGame))
			{
				return;
			}
			mUpdateCnt++;
			mIs3d = GlobalMembers.gApp.Is3DAccelerated();
			mWidth = GlobalMembers.gApp.mWidth;
			mHeight = GlobalMembers.gApp.mHeight;
			if (mDistortEffectList.Count != 0)
			{
				int count = mDistortEffectList.Count;
				for (int num = count - 1; num >= 0; num--)
				{
					DistortEffect distortEffect = mDistortEffectList[num];
					bool flag = true;
					flag &= !distortEffect.mTexturePos.IncInVal();
					flag &= !distortEffect.mRadius.IncInVal();
					flag &= !distortEffect.mIntensity.IncInVal();
					if (flag & !distortEffect.mMovePct.IncInVal())
					{
						mDistortEffectList.RemoveAt(num);
					}
				}
			}
			int num2 = 0;
			UpdateTypeEmber(3);
			UpdateTypeEmber(4);
			UpdateTypeEmber(5);
			UpdateTypeEmber(6);
			for (int i = 0; i < 24; i++)
			{
				int num3 = 0;
				if (mUpdateCnt % 24 == i)
				{
					GarbageCollectEffects(i);
				}
				if (i >= 3 && i <= 6)
				{
					continue;
				}
				List<Effect>.Enumerator enumerator = mEffectList[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					Effect current = enumerator.Current;
					current.Update();
					num2++;
					num3++;
					if (current.mDoubleSpeed)
					{
						current.Update();
					}
					int num4 = ((current.mUpdateDiv == 0) ? 1 : current.mUpdateDiv);
					current.mX += current.mDX / (float)num4;
					current.mY += current.mDY / (float)num4;
					current.mZ += current.mDZ / (float)num4;
					if (mUpdateCnt % num4 == 0)
					{
						current.mDY += current.mGravity;
						current.mDX *= current.mDXScalar;
						current.mDY *= current.mDYScalar;
						current.mDZ *= current.mDZScalar;
						if ((current.mFlags & 4) != 0 && current.mDelay > 0f)
						{
							current.mDelay -= 0.01f;
							if (current.mDelay <= 0f)
							{
								current.mDelay = 0f;
							}
						}
						else
						{
							current.mAlpha += current.mDAlpha;
							current.mScale += current.mDScale;
						}
					}
					switch (current.mType)
					{
					case Effect.Type.TYPE_SPARKLE_SHARD:
						current.mDX *= 0.98f;
						current.mDY *= 0.98f;
						if (mUpdateCnt % current.mUpdateDiv == 0)
						{
							current.mFrame = (current.mFrame + 1) % 40;
						}
						if (current.mFrame == 14)
						{
							current.mDeleteMe = true;
						}
						break;
					case Effect.Type.TYPE_EMBER_BOTTOM:
					case Effect.Type.TYPE_EMBER:
					case Effect.Type.TYPE_EMBER_FADEINOUT:
						current.mScale += current.mDScale;
						current.mAngle += current.mDAngle;
						if (current.mType == Effect.Type.TYPE_EMBER_FADEINOUT || current.mType == Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM)
						{
							current.mAlpha += current.mDAlpha;
							if (current.mAlpha >= 1f)
							{
								current.mDeleteMe = true;
							}
							current.mFrame = (int)(12f * current.mAlpha);
						}
						else if (current.mColor.mRed + current.mColor.mGreen == 0)
						{
							current.mScale += 0.02f;
							current.mAlpha -= 0.01f;
							if (current.mAlpha <= 0f)
							{
								current.mDeleteMe = true;
							}
						}
						break;
					case Effect.Type.TYPE_COUNTDOWN_SHARD:
					{
						current.mAngle += current.mDAngle;
						Effect effect2 = AllocEffect(Effect.Type.TYPE_SMOKE_PUFF);
						effect2.mScale *= current.mScale * (GlobalMembers.M(0f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(0.45f));
						effect2.mAlpha *= GlobalMembers.M(0.3f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(0.05f);
						effect2.mAlpha *= current.mAlpha;
						effect2.mAngle = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
						effect2.mDAngle = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * GlobalMembers.M(0.1f);
						effect2.mIsAdditive = GlobalMembers.M(0) > 0;
						effect2.mGravity = 0f;
						effect2.mDY = GlobalMembers.MS(-1f);
						effect2.mX = current.mX;
						effect2.mY = current.mY;
						AddEffect(effect2);
						if (current.mScale < GlobalMembers.M(0.02f))
						{
							current.mDeleteMe = true;
						}
						break;
					}
					case Effect.Type.TYPE_SMOKE_PUFF:
						current.mDX *= 0.95f;
						break;
					case Effect.Type.TYPE_STEAM_COMET:
						if (current.mScale < GlobalMembers.M(0.1f))
						{
							current.mAlpha = 0f;
							break;
						}
						if (mUpdateCnt % GlobalMembers.M(10) == GlobalMembers.M(9))
						{
							current.mDX += GlobalMembers.M(1.3f) * GlobalMembersUtils.GetRandFloat();
							current.mDY += GlobalMembers.M(1.3f) * GlobalMembersUtils.GetRandFloat();
						}
						if (mUpdateCnt % 2 != 0)
						{
							Effect effect = AllocEffect(Effect.Type.TYPE_STEAM);
							effect.mDX = 0f;
							effect.mDY = 0f;
							effect.mX = current.mX;
							effect.mY = current.mY;
							effect.mIsAdditive = false;
							effect.mScale = current.mScale;
							effect.mDScale = GlobalMembers.M(0f);
							AddEffect(effect);
						}
						break;
					case Effect.Type.TYPE_DROPLET:
						if (current.mScale < 0f)
						{
							current.mAlpha = 0f;
						}
						current.mAngle = GlobalMembers.M(0f) + (float)Math.Atan2(current.mDX, current.mDY);
						break;
					case Effect.Type.TYPE_BLAST_RING:
						current.mScale += 0.8f;
						current.mAlpha -= 0.02f;
						break;
					case Effect.Type.TYPE_STEAM:
					{
						float num5 = current.mDX * current.mDX + current.mDY * current.mDY;
						current.mDX *= current.mValue[2];
						if (num5 > current.mValue[0] * current.mValue[0])
						{
							current.mDY *= current.mValue[2];
						}
						else
						{
							current.mDAlpha = current.mValue[1];
						}
						current.mAngle += current.mDAngle;
						current.mScale += current.mDScale;
						break;
					}
					case Effect.Type.TYPE_GLITTER_SPARK:
						current.mAlpha = ((BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(5) == 0) ? 1f : GlobalMembers.M(0.25f));
						break;
					case Effect.Type.TYPE_FRUIT_SPARK:
						current.mDY += current.mGravity;
						current.mAngle += (mIs3d ? (current.mDY * 0.02f) : 0f);
						current.mScale += 0.05f;
						if (current.mScale > 0.5f)
						{
							current.mScale = 0.5f;
						}
						break;
					case Effect.Type.TYPE_GEM_SHARD:
						if (mUpdateCnt % 2 == 0)
						{
							current.mFrame = (current.mFrame + 1) % 40;
						}
						current.mDX *= current.mDecel;
						current.mDY *= current.mDecel;
						current.mAngle += current.mDAngle;
						current.mValue[0] += current.mValue[2];
						current.mValue[1] += GlobalMembers.M(1f) * current.mValue[3];
						break;
					case Effect.Type.TYPE_NONE:
						if (current.mUpdateDiv == 0 || mUpdateCnt % current.mUpdateDiv == 0)
						{
							if (current.mIsCyclingColor)
							{
								current.mCurHue = (current.mCurHue + 8) % 256;
								current.mColor = new Color((int)GlobalMembers.gApp.HSLToRGB(current.mCurHue, 255, 128));
							}
							current.mAngle += current.mDAngle;
						}
						break;
					case Effect.Type.TYPE_LIGHT:
						current.mLightSize = current.mScale;
						current.mLightIntensity = current.mAlpha;
						break;
					case Effect.Type.TYPE_WALL_ROCK:
						current.mDX *= current.mDecel;
						if (current.mY > (float)GlobalMembers.RS(GlobalMembers.gApp.mHeight))
						{
							current.mDeleteMe = true;
						}
						break;
					case Effect.Type.TYPE_QUAKE_DUST:
						if (current.mAlpha >= 1f)
						{
							current.mAlpha = 1f;
							current.mDAlpha = -0.01f;
						}
						break;
					case Effect.Type.TYPE_HYPERCUBE_ENERGIZE:
						current.mAngle += current.mDAngle;
						break;
					}
					if (current.mScale < current.mMinScale)
					{
						current.mDeleteMe = true;
						current.mScale = current.mMinScale;
					}
					else if (current.mScale > current.mMaxScale)
					{
						current.mScale = current.mMaxScale;
						if ((current.mFlags & 1) != 0)
						{
							current.mDScale = 0f - current.mDScale;
						}
					}
					if (current.mAlpha > current.mMaxAlpha)
					{
						current.mAlpha = current.mMaxAlpha;
						if ((current.mFlags & 2) != 0)
						{
							current.mDAlpha = 0f - current.mDAlpha;
						}
						else
						{
							current.mDeleteMe = true;
						}
					}
					else if (current.mAlpha <= 0f)
					{
						current.mDeleteMe = true;
					}
					if (current.mCurvedAlpha.HasBeenTriggered())
					{
						current.mDeleteMe = true;
					}
					if (current.mCurvedScale.HasBeenTriggered())
					{
						current.mDeleteMe = true;
					}
				}
			}
		}

		public override void Draw(Graphics g)
		{
			DoDraw(g, false);
			if (mWidgetManager != null)
			{
				DeferOverlay(1);
			}
			else if (mDeferOverlay)
			{
				DrawOverlay(g);
			}
		}

		public override void DrawOverlay(Graphics g)
		{
			if (mBoard != null && mAlpha > 0f && !mBoard.mKilling && !mBoard.mIsWholeGameReplay && !mBoard.mSuspendingGame && (mDistortEffectList.Count > 0 || (mHeightImageDirty && mWidgetManager.mImage == g.mDestImage)))
			{
				Graphics3D graphics3D = g.Get3D();
				if (graphics3D != null && graphics3D.SupportsPixelShaders())
				{
					RenderDistortEffects(g);
				}
				mHeightImageDirty = false;
			}
			DoDraw(g, true);
		}

		public void DoDraw(Graphics g, bool isOverlay)
		{
			if (mAlpha == 0f || (mBoard != null && ((mBoard.mGameOverCount > 0 && (double)mBoard.mSlideUIPct >= 1.0) || mBoard.mSuspendingGame)))
			{
				return;
			}
			Graphics3D graphics3D = g.Get3D();
			if (mDisableMask)
			{
				graphics3D?.SetMasking(Graphics3D.EMaskMode.MASKMODE_NONE);
			}
			DrawTypeEmber(Effect.Type.TYPE_EMBER_BOTTOM, g, isOverlay);
			DrawTypeEmber(Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM, g, isOverlay);
			DrawTypeEmber(Effect.Type.TYPE_EMBER, g, isOverlay);
			DrawTypeEmber(Effect.Type.TYPE_EMBER_FADEINOUT, g, isOverlay);
			for (int i = 0; i < 24; i++)
			{
				if (i >= 3 && i <= 6)
				{
					continue;
				}
				if (i == 21)
				{
					TimeBonusEffect.batchBegin();
				}
				List<Effect>.Enumerator enumerator = mEffectList[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					Effect current = enumerator.Current;
					if (current.mOverlay != isOverlay || current.mDeleteMe)
					{
						continue;
					}
					Piece mPieceRel = current.mPieceRel;
					if (mPieceRel != null && (double)mPieceRel.mAlpha <= 0.0)
					{
						continue;
					}
					if (mPieceRel != null && current.mType != Effect.Type.TYPE_PI && current.mType != Effect.Type.TYPE_POPANIM && current.mType != Effect.Type.TYPE_TIME_BONUS && current.mType != Effect.Type.TYPE_CUSTOMCLASS)
					{
						current.mX += mPieceRel.GetScreenX();
						current.mY += mPieceRel.GetScreenY();
						if (mBoard != null && mBoard.mPostFXManager == this)
						{
							current.mX += (float)(double)mBoard.mSideXOff * mBoard.mSlideXScale;
						}
					}
					if (current.mType == Effect.Type.TYPE_CUSTOMCLASS)
					{
						current.Draw(g);
						continue;
					}
					float num = Math.Min(1f, current.mAlpha) * mAlpha;
					if (mBoard != null)
					{
						mAlpha *= GlobalMembers.gApp.mBoard.GetAlpha();
					}
					switch (current.mType)
					{
					case Effect.Type.TYPE_NONE:
					{
						g.SetColorizeImages(true);
						Color mColor4 = current.mColor;
						mColor4.mAlpha = (int)(255f * num);
						g.SetColor(mColor4);
						g.SetDrawMode(current.mIsAdditive ? 1 : 0);
						if (current.mImage != null)
						{
							Rect celRect2 = current.mImage.GetCelRect(current.mFrame);
							Utils.MyDrawImageRotated(g, current.mImage, celRect2, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, current.mScale, current.mScale);
							g.SetColorizeImages(false);
						}
						break;
					}
					case Effect.Type.TYPE_GEM_SHARD:
					{
						Color mColor8 = current.mColor;
						float num9 = mAlpha * current.mAlpha;
						if (num9 > 0f)
						{
							num9 = (float)Math.Sqrt(num9);
						}
						mColor8.mAlpha = (int)(num9 * GlobalMembers.M(64f));
						float num10 = 0.2f;
						float num11 = 0.3f;
						float num16 = num10 % num11;
						g.SetColorizeImages(true);
						g.SetColor(mColor8);
						float mScale3 = current.mScale;
						DD_aTrans.Reset();
						DD_aTrans.RotateRad(current.mAngle);
						float num12 = GlobalMembers.M(0.25f);
						float num13 = (float)Math.Sin(current.mValue[0]);
						if (num13 > 0f && num13 < num12)
						{
							num13 = num12;
						}
						if (num13 < 0f && num13 > 0f - num12)
						{
							num13 = 0f - num12;
						}
						float num14 = (float)Math.Sin(current.mValue[1]);
						if (num14 > 0f && num14 < num12)
						{
							num14 = num12;
						}
						if (num14 < 0f && num14 > 0f - num12)
						{
							num14 = 0f - num12;
						}
						DD_aTrans.Scale(GlobalMembers.M(1.4f) * num13 * mScale3, GlobalMembers.M(1.4f) * num14 * mScale3);
						g.SetColorizeImages(true);
						g.SetDrawMode(0);
						DD_aTrans.Scale(GlobalMembers.M(1.15f) * current.mScale, GlobalMembers.M(1.15f) * current.mScale);
						mColor8.mAlpha = (int)(num9 * GlobalMembers.M(255f));
						g.SetColor(mColor8);
						int num15 = current.GetHashCode() * mUpdateCnt % 8;
						GlobalMembers.gGR.DrawImageTransform(g, GlobalMembersResourcesWP.IMAGE_SM_SHARDS, DD_aTrans, new Rect(GlobalMembersResourcesWP.IMAGE_SM_SHARDS.GetCelWidth() * num15, 0, GlobalMembersResourcesWP.IMAGE_SM_SHARDS.GetCelWidth(), GlobalMembersResourcesWP.IMAGE_SM_SHARDS.GetCelHeight()), (int)GlobalMembers.S(current.mX), (int)GlobalMembers.S(current.mY));
						break;
					}
					case Effect.Type.TYPE_SPARKLE_SHARD:
					{
						Image iMAGE_SPARKLE = GlobalMembersResourcesWP.IMAGE_SPARKLE;
						g.SetDrawMode(1);
						g.SetColorizeImages(true);
						current.mColor.mAlpha = (int)(255f * num);
						g.SetColor(current.mColor);
						int num7 = (int)(current.mScale * (float)iMAGE_SPARKLE.GetCelWidth());
						int num8 = (int)(current.mScale * (float)iMAGE_SPARKLE.GetCelHeight());
						GlobalMembers.gGR.DrawImageCel(g, iMAGE_SPARKLE, new Rect((int)GlobalMembers.S(current.mX) - num7 / 2, (int)GlobalMembers.S(current.mY) - num8 / 2, num7, num8), current.mFrame);
						g.SetColorizeImages(false);
						g.SetDrawMode(0);
						break;
					}
					case Effect.Type.TYPE_STEAM:
					{
						g.SetDrawMode(0);
						g.SetColorizeImages(true);
						Color mColor7 = current.mColor;
						mColor7.mAlpha = (int)(255f * num);
						g.SetColor(mColor7);
						int mFrame2 = current.mFrame;
						Utils.MyDrawImageRotated(g, current.mImage, current.mImage.GetCelRect(mFrame2), GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, current.mScale, current.mScale);
						break;
					}
					case Effect.Type.TYPE_GLITTER_SPARK:
					{
						g.SetDrawMode(1);
						g.SetColorizeImages(true);
						Color color2 = new Color(255, 255, 128, (int)(255f * num));
						g.SetColor(color2);
						Image iMAGE_GEM_FRUIT_SPARK2 = GlobalMembersResourcesWP.IMAGE_GEM_FRUIT_SPARK;
						float mScale2 = current.mScale;
						mScale2 *= num;
						Utils.MyDrawImageRotated(g, iMAGE_GEM_FRUIT_SPARK2, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, mScale2, mScale2);
						break;
					}
					case Effect.Type.TYPE_FRUIT_SPARK:
					{
						g.SetDrawMode(1);
						g.SetColorizeImages(true);
						Color color = new Color(255, 255, 255, (int)(255f * num));
						g.SetColor(color);
						Image iMAGE_GEM_FRUIT_SPARK = GlobalMembersResourcesWP.IMAGE_GEM_FRUIT_SPARK;
						float mScale = current.mScale;
						mScale *= num;
						Utils.MyDrawImageRotated(g, iMAGE_GEM_FRUIT_SPARK, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, mScale, mScale);
						break;
					}
					case Effect.Type.TYPE_EMBER_BOTTOM:
					case Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM:
					case Effect.Type.TYPE_EMBER:
					case Effect.Type.TYPE_EMBER_FADEINOUT:
					{
						g.SetColorizeImages(true);
						Color mColor5 = current.mColor;
						int num5 = ((GlobalMembers.M(1) != 0) ? GlobalMembers.M(1) : (mColor5.mRed + mColor5.mGreen + mColor5.mBlue));
						if (current.mType == Effect.Type.TYPE_EMBER_BOTTOM || current.mType == Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM)
						{
							num5 = 0;
						}
						g.SetDrawMode((num5 != 0) ? 1 : 0);
						mColor5.mAlpha = (int)(200f * num);
						g.SetColor(mColor5);
						if (current.mType == Effect.Type.TYPE_EMBER_FADEINOUT || current.mType == Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM)
						{
							float num6 = Math.Min(63, (int)(64f * current.mAlpha));
							Color mColor6 = current.mColor;
							mColor6.mAlpha = (int)((float)mColor6.mAlpha * mAlpha);
							Transform transform2 = new Transform();
							Rect rect = default(Rect);
							if (graphics3D != null)
							{
								transform2.Scale(current.mScale, current.mScale);
								if (current.mAngle != 0f)
								{
									transform2.RotateRad(current.mAngle);
								}
							}
							g.SetColor(mColor6);
							rect = current.mImage.GetCelRect((int)num6);
							if (graphics3D != null)
							{
								GlobalMembers.gGR.DrawImageTransformF(g, current.mImage, transform2, rect, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY));
							}
							else
							{
								GlobalMembers.gGR.DrawImageTransform(g, current.mImage, transform2, rect, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY));
							}
						}
						else
						{
							Utils.MyDrawImageRotated(g, current.mImage, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, current.mScale, current.mScale);
						}
						break;
					}
					case Effect.Type.TYPE_COUNTDOWN_SHARD:
					{
						g.SetColorizeImages(true);
						Color mColor3 = current.mColor;
						mColor3.mAlpha = (int)(255f * num);
						g.SetColor(mColor3);
						g.SetDrawMode(0);
						if (current.mImage != null)
						{
							Rect celRect = current.mImage.GetCelRect(current.mFrame, 0);
							Utils.MyDrawImageRotated(g, current.mImage, celRect, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, current.mScale, current.mScale);
							g.SetColorizeImages(false);
						}
						break;
					}
					case Effect.Type.TYPE_SMOKE_PUFF:
					case Effect.Type.TYPE_STEAM_COMET:
					{
						g.SetDrawMode(0);
						g.SetColorizeImages(true);
						Color mColor2 = current.mColor;
						mColor2.mAlpha = (int)(255f * num);
						g.SetColor(mColor2);
						int mFrame = current.mFrame;
						Utils.MyDrawImageRotated(g, current.mImage, current.mImage.GetCelRect(mFrame), GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, current.mScale, current.mScale);
						break;
					}
					case Effect.Type.TYPE_DROPLET:
					{
						g.SetDrawMode(0);
						g.SetColorizeImages(true);
						Color mColor = current.mColor;
						mColor.mAlpha = (int)(255f * num);
						g.SetColor(mColor);
						float num4 = (float)Math.Sqrt(current.mScale);
						int theCel = 0;
						Utils.MyDrawImageRotated(g, current.mImage, current.mImage.GetCelRect(theCel), GlobalMembers.S(current.mX), GlobalMembers.S(current.mY), current.mAngle, num4, num4);
						break;
					}
					case Effect.Type.TYPE_WALL_ROCK:
						g.SetColor(new Color(-1));
						g.SetDrawMode(0);
						g.DrawImageCel(current.mImage, (int)(GlobalMembers.S(current.mX) - (float)(current.mImage.GetCelWidth() / 2)), (int)(GlobalMembers.S(current.mY) - (float)(current.mImage.GetCelHeight() / 2)), current.mFrame);
						if (current.mColor.ToInt() != -1)
						{
							g.SetColorizeImages(true);
							g.SetDrawMode(1);
							g.SetColor(current.mColor);
							g.DrawImageCel(current.mImage, (int)(GlobalMembers.S(current.mX) - (float)(current.mImage.GetCelWidth() / 2)), (int)(GlobalMembers.S(current.mY) - (float)(current.mImage.GetCelHeight() / 2)), current.mFrame);
							g.SetDrawMode(0);
						}
						break;
					case Effect.Type.TYPE_QUAKE_DUST:
						g.SetColor(Color.White);
						g.SetDrawMode(0);
						g.SetColorizeImages(true);
						g.mColor.mAlpha = (int)(current.mAlpha * 255f);
						g.DrawImage(current.mImage, (int)GlobalMembers.S(current.mX), (int)GlobalMembers.S(current.mY));
						break;
					case Effect.Type.TYPE_HYPERCUBE_ENERGIZE:
					{
						g.SetDrawMode(1);
						g.SetColorizeImages(true);
						Transform transform = new Transform();
						transform.Reset();
						int num2 = (int)((double)current.mCurvedAlpha * (double)mAlpha);
						g.SetColor(new Color(num2, num2, num2));
						float num3 = (float)Math.Pow(current.mCurvedScale, GlobalMembers.M(2.5));
						transform.Scale((float)((double)current.mCurvedAlpha / 160.0), (float)((double)current.mCurvedAlpha / 320.0));
						transform.RotateDeg(current.mAngle * 0.1f * num3 + 15f);
						GlobalMembers.gGR.DrawImageTransform(g, GlobalMembersResourcesWP.IMAGE_HYPERFLARELINE, transform, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY));
						transform.Reset();
						transform.Scale((float)((double)current.mCurvedAlpha / 250.0), (float)((double)current.mCurvedAlpha / 500.0));
						transform.RotateDeg((0f - current.mAngle) * 0.2f * num3 - 45f);
						GlobalMembers.gGR.DrawImageTransform(g, GlobalMembersResourcesWP.IMAGE_HYPERFLARELINE, transform, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY));
						transform.Reset();
						transform.Scale((float)((double)current.mCurvedAlpha / 300.0), (float)((double)current.mCurvedAlpha / 600.0));
						transform.RotateDeg(current.mAngle * 0.05f * num3 + 60f);
						GlobalMembers.gGR.DrawImageTransform(g, GlobalMembersResourcesWP.IMAGE_HYPERFLARELINE, transform, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY));
						transform.Reset();
						transform.Scale((float)((double)current.mCurvedScale * (double)GlobalMembers.M(0.6f)), (float)((double)current.mCurvedScale * (double)GlobalMembers.M(0.6f)));
						GlobalMembers.gGR.DrawImageTransform(g, GlobalMembersResourcesWP.IMAGE_HYPERFLARERING, transform, GlobalMembers.S(current.mX), GlobalMembers.S(current.mY));
						break;
					}
					default:
						current.Draw(g);
						break;
					case Effect.Type.TYPE_LIGHT:
						break;
					}
					if (mPieceRel != null && current.mType != Effect.Type.TYPE_PI && current.mType != Effect.Type.TYPE_POPANIM && current.mType != Effect.Type.TYPE_TIME_BONUS)
					{
						current.mX -= mPieceRel.GetScreenX();
						current.mY -= mPieceRel.GetScreenY();
						if (mBoard != null && mBoard.mPostFXManager == this)
						{
							current.mX -= (float)((double)mBoard.mSideXOff * (double)mBoard.mSlideXScale);
						}
					}
				}
				if (i == 21)
				{
					TimeBonusEffect.batchEnd(g);
				}
			}
		}

		public override void AddedToManager(WidgetManager theMgr)
		{
			base.AddedToManager(theMgr);
		}

		public bool IsEmpty()
		{
			for (int i = 0; i < 24; i++)
			{
				if (mEffectList[i].Count != 0)
				{
					return false;
				}
			}
			return true;
		}

		public Effect AllocEffect()
		{
			return Effect.alloc();
		}

		public Effect AllocEffect(Effect.Type theType)
		{
			return Effect.alloc(theType);
		}

		public void FreeEffect(Effect theEffect)
		{
			theEffect?.release();
		}

		public void AddEffect(Effect theEffect)
		{
			mEffectList[(int)theEffect.mType].Add(theEffect);
			theEffect.mFXManager = this;
		}

		public void FreePieceEffect(int thePieceId)
		{
			for (int i = 0; i < 24; i++)
			{
				List<Effect> list = mEffectList[i];
				for (int num = list.Count - 1; num >= 0; num--)
				{
					Effect effect = list[num];
					if (effect.mPieceRel != null && effect.mPieceRel.mId == thePieceId)
					{
						effect.Dispose();
						list.RemoveAt(num);
					}
				}
			}
		}

		public void FreePieceEffect(Piece piece)
		{
			for (int i = 0; i < 24; i++)
			{
				List<Effect> list = mEffectList[i];
				for (int num = list.Count - 1; num >= 0; num--)
				{
					Effect effect = list[num];
					if (effect.mPieceRel == piece)
					{
						effect.Dispose();
						list.RemoveAt(num);
					}
				}
			}
		}

		public void AddSteamExplosion(float theX, float theY, Color theColor)
		{
			for (int i = 0; i < GlobalMembers.M(12); i++)
			{
				Effect effect = AllocEffect(Effect.Type.TYPE_STEAM);
				float num = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
				float num2 = GlobalMembers.MS(0f) + GlobalMembers.MS(4f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
				effect.mDX = num2 * (float)Math.Cos(num);
				effect.mDY = num2 * (float)Math.Sin(num);
				effect.mX = theX + (float)Math.Cos(num) * GlobalMembers.M(25f);
				effect.mY = theY + (float)Math.Sin(num) * GlobalMembers.M(25f);
				effect.mIsAdditive = false;
				effect.mScale = GlobalMembers.M(0.1f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(1f);
				effect.mDScale = GlobalMembers.M(0.02f);
				AddEffect(effect);
			}
			for (int j = 0; j < GlobalMembers.M(12); j++)
			{
				Effect effect2 = AllocEffect(Effect.Type.TYPE_DROPLET);
				float num3 = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
				float num4 = GlobalMembers.MS(1f) + GlobalMembers.MS(5f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
				effect2.mDX = num4 * (float)Math.Cos(num3);
				effect2.mDY = num4 * (float)Math.Sin(num3) + GlobalMembers.M(-1.5f);
				effect2.mX = theX + (float)Math.Cos(num3) * GlobalMembers.M(25f);
				effect2.mY = theY + (float)Math.Sin(num3) * GlobalMembers.M(25f);
				effect2.mIsAdditive = false;
				effect2.mScale = GlobalMembers.M(0.6f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(0.2f);
				effect2.mDScale = GlobalMembers.M(-0.01f);
				effect2.mAlpha = GlobalMembers.M(0.6f);
				effect2.mColor = new Color(11184844);
				AddEffect(effect2);
			}
			for (int k = 0; k < GlobalMembers.M(3); k++)
			{
				Effect effect3 = AllocEffect(Effect.Type.TYPE_STEAM_COMET);
				float num5 = (float)k * (float)Math.PI / 3f + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.2f);
				float num6 = GlobalMembers.MS(6f) + GlobalMembers.MS(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat());
				effect3.mValue[0] = theX;
				effect3.mValue[1] = theY;
				effect3.mDX = num6 * (float)Math.Cos(num5);
				effect3.mDY = num6 * (float)Math.Sin(num5);
				effect3.mX = theX + (float)Math.Cos(num5) * GlobalMembers.M(25f);
				effect3.mY = theY + (float)Math.Sin(num5) * GlobalMembers.M(25f);
				effect3.mIsAdditive = false;
				effect3.mScale = GlobalMembers.M(2.5f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(0.1f);
				effect3.mDScale = GlobalMembers.M(-0.08f);
				AddEffect(effect3);
			}
			for (int l = 0; l < GlobalMembers.M(12); l++)
			{
				Effect effect4 = AllocEffect(Effect.Type.TYPE_GEM_SHARD);
				effect4.mColor = theColor;
				int num7 = 0;
				double num8 = 0.0;
				float num9 = 0f;
				num8 = GlobalMembersUtils.GetRandFloat() * 3.14159f;
				num7 = (int)(GlobalMembersUtils.GetRandFloat() * (float)GlobalMembers.S(GlobalMembers.M(48)));
				effect4.mX = theX + (float)(int)(GlobalMembers.M(1f) * (float)num7 * (float)Math.Cos(num8));
				effect4.mY = theY + (float)(int)(GlobalMembers.M(1f) * (float)num7 * (float)Math.Sin(num8));
				num8 = (float)Math.Atan2(effect4.mY - theY, effect4.mX - theX) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.3f);
				num9 = GlobalMembers.MS(4.5f) + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.MS(1.5f);
				effect4.mDX = (float)Math.Cos(num8) * num9;
				effect4.mDY = (float)Math.Sin(num8) * num9 + GlobalMembers.MS(-2f);
				effect4.mDecel = GlobalMembers.M(0.99f) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.005f);
				effect4.mAngle = (float)num8;
				effect4.mDAngle = GlobalMembers.M(0.05f) * GlobalMembersUtils.GetRandFloat();
				effect4.mGravity = GlobalMembers.M(0.06f);
				effect4.mValue[0] = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f;
				effect4.mValue[1] = GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f;
				effect4.mValue[2] = GlobalMembers.M(0.045f) * (GlobalMembers.M(3f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(1f));
				effect4.mValue[3] = GlobalMembers.M(0.045f) * (GlobalMembers.M(3f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(1f));
				effect4.mDAlpha = GlobalMembers.M(-0.0025f) * (GlobalMembers.M(2f) * Math.Abs(GlobalMembersUtils.GetRandFloat()) + GlobalMembers.M(4f));
				AddEffect(effect4);
			}
		}

		public DistortEffect AddHeatwave(float x, float y, float theScale)
		{
			DistortEffect distortEffect = new DistortEffect();
			distortEffect.mCenter = new FPoint(x, y);
			distortEffect.mRadius.mOutMax *= theScale;
			mDistortEffectList.Add(distortEffect);
			return distortEffect;
		}

		public void Clear()
		{
			for (int i = 0; i < 24; i++)
			{
				mEffectList[i].GetEnumerator();
				mEffectList[i].Clear();
			}
		}

		public void RemovePieceFromEffects(Piece thePiece)
		{
			for (int i = 0; i < 24; i++)
			{
				List<Effect>.Enumerator enumerator = mEffectList[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					Effect current = enumerator.Current;
					if (current.mPieceRel == thePiece)
					{
						current.mPieceRel = null;
						current.mDeleteMe = true;
					}
				}
			}
		}

		public void BltDouble(Graphics g, Image theImage, FRect theDestRect, Color theColor, float theDestScale)
		{
			FRect fRect = new FRect(theDestRect);
			float pu = fRect.mX / (float)GlobalMembers.gApp.mScreenBounds.mWidth * theDestScale;
			float pu2 = (fRect.mX + fRect.mWidth) / (float)GlobalMembers.gApp.mScreenBounds.mWidth * theDestScale;
			float pv = fRect.mY / (float)GlobalMembers.gApp.mScreenBounds.mHeight * theDestScale;
			float pv2 = (fRect.mY + fRect.mHeight) / (float)GlobalMembers.gApp.mScreenBounds.mHeight * theDestScale;
			SexyVertex2D[] theVertices = new SexyVertex2D[4]
			{
				new SexyVertex2D(fRect.mX - 0.5f, fRect.mY - 0.5f, 0f, 0f, pu, pv),
				new SexyVertex2D(fRect.mX + fRect.mWidth - 0.5f, fRect.mY - 0.5f, 1f, 0f, pu2, pv),
				new SexyVertex2D(fRect.mX - 0.5f, fRect.mY + fRect.mHeight - 0.5f, 0f, 1f, pu, pv2),
				new SexyVertex2D(fRect.mX + fRect.mWidth - 0.5f, fRect.mY + fRect.mHeight - 0.5f, 1f, 1f, pu2, pv2)
			};
			g.Get3D().SetTexture(0, theImage);
			g.Get3D().DrawPrimitive(708u, Graphics3D.EPrimitiveType.PT_TriangleStrip, theVertices, 2, theColor, 0, 0f, 0f, true, 0u);
		}

		public void BltDoubleFromSrcRect(Graphics g, Image theImage, FRect theDestRect, FRect theSrcRect, Color theColor)
		{
			BltDoubleFromSrcRect(g, theImage, theDestRect, theSrcRect, theColor, 1f);
		}

		public void BltDoubleFromSrcRect(Graphics g, Image theImage, FRect theDestRect, FRect theSrcRect, Color theColor, float theDestScale)
		{
			float pu = theDestRect.mX / (float)mWidth * theDestScale;
			float pu2 = (theDestRect.mX + theDestRect.mWidth) / (float)mWidth * theDestScale;
			float pv = theDestRect.mY / (float)mHeight * theDestScale;
			float pv2 = (theDestRect.mY + theDestRect.mHeight) / (float)mHeight * theDestScale;
			float pu3 = (theSrcRect.mX + g.mTransX) / (float)GlobalMembers.gApp.mScreenBounds.mWidth;
			float pu4 = (theSrcRect.mX + g.mTransX + theSrcRect.mWidth) / (float)GlobalMembers.gApp.mScreenBounds.mWidth;
			float pv3 = theSrcRect.mY / (float)GlobalMembers.gApp.mScreenBounds.mHeight;
			float pv4 = (theSrcRect.mY + theSrcRect.mHeight) / (float)GlobalMembers.gApp.mScreenBounds.mHeight;
			SexyVertex2D[] theVertices = new SexyVertex2D[4]
			{
				new SexyVertex2D(theDestRect.mX + g.mTransX - 0.5f, theDestRect.mY - 0.5f, pu3, pv3, pu, pv),
				new SexyVertex2D(theDestRect.mX + g.mTransX + theDestRect.mWidth - 0.5f, theDestRect.mY - 0.5f, pu4, pv3, pu2, pv),
				new SexyVertex2D(theDestRect.mX + g.mTransX - 0.5f, theDestRect.mY + theDestRect.mHeight - 0.5f, pu3, pv4, pu, pv2),
				new SexyVertex2D(theDestRect.mX + g.mTransX + theDestRect.mWidth - 0.5f, theDestRect.mY + theDestRect.mHeight - 0.5f, pu4, pv4, pu2, pv2)
			};
			g.Get3D().SetTexture(0, theImage);
			g.Get3D().DrawPrimitive(708u, Graphics3D.EPrimitiveType.PT_TriangleStrip, theVertices, 2, theColor, 0, 0f, 0f, true, 0u);
		}

		public void RenderDistortEffects(Graphics g)
		{
			DeviceImage heightImage = GetHeightImage();
			if (heightImage == null)
			{
				return;
			}
			Graphics3D graphics3D = g.Get3D();
			if (graphics3D == null || !graphics3D.SupportsPixelShaders())
			{
				return;
			}
			Graphics graphics = new Graphics(heightImage);
			Graphics3D graphics3D2 = graphics.Get3D();
			graphics.PushState();
			graphics.mTransX = 0f;
			graphics.mTransY = 0f;
			graphics3D2.SetTextureLinearFilter(0, true);
			graphics3D2.SetTextureLinearFilter(1, true);
			graphics3D2.SetTextureWrap(0, true);
			graphics3D2.SetTextureWrap(1, true);
			graphics.SetColor(new Color(128, 128, 128, 255));
			graphics.FillRect(0, 0, heightImage.mWidth, heightImage.mHeight);
			graphics.SetColorizeImages(true);
			graphics.SetColor(Color.White);
			RenderEffect effect = graphics3D2.GetEffect(GlobalMembersResourcesWP.EFFECT_WAVE);
			using (RenderEffectAutoState renderEffectAutoState = new RenderEffectAutoState(graphics, effect))
			{
				while (!renderEffectAutoState.IsDone())
				{
					List<DistortEffect>.Enumerator enumerator = mDistortEffectList.GetEnumerator();
					while (enumerator.MoveNext())
					{
						DistortEffect current = enumerator.Current;
						float num = (float)((double)current.mIntensity * (double)mAlpha);
						float num2 = (float)(double)current.mTexturePos;
						float[] array = new float[4]
						{
							Math.Max(1f - Math.Abs(num2) * 3f, 0f) * num,
							Math.Max(1f - Math.Abs(num2 - 0.333f) * 3f, 0f) * num,
							Math.Max(1f - Math.Abs(num2 - 0.667f) * 3f, 0f) * num,
							Math.Max(1f - Math.Abs(num2 - 1f) * 3f, 0f) * num
						};
						float num3 = (float)(double)current.mRadius;
						new Rect((int)(GlobalMembers.S((double)current.mCenter.mX + (double)current.mMoveDelta.mX * (double)current.mMovePct - (double)num3) / (double)GlobalMembers.M(4f)), (int)(GlobalMembers.S((double)current.mCenter.mY + (double)current.mMoveDelta.mY * (double)current.mMovePct - (double)num3) / (double)GlobalMembers.M(4f)), (int)(GlobalMembers.S(num3) / 2f), (int)(GlobalMembers.S(num3) / 2f));
						Color color = new Color((int)(array[0] * 255f), (int)(array[1] * 255f), (int)(array[2] * 255f), (int)(array[3] * 255f));
						graphics.SetColor(color);
					}
					renderEffectAutoState.NextPass();
				}
			}
			graphics.PopState();
			if (mDistortEffectList.Count > 0 || GlobalMembers.M(0) != 0)
			{
				SharedRenderTarget sharedRenderTarget = new SharedRenderTarget();
				Image image = sharedRenderTarget.LockScreenImage("DistortFull");
				g.PushState();
				g.mTransX = 0f;
				g.mTransY = 0f;
				g.SetColor(Color.White);
				graphics3D.SetBlend(Graphics3D.EBlendMode.BLEND_ONE, Graphics3D.EBlendMode.BLEND_ZERO);
				g.DrawImage(image, 0, 0);
				graphics3D.SetTexture(1, heightImage);
				graphics3D.SetTextureLinearFilter(0, true);
				graphics3D.SetTextureLinearFilter(1, true);
				effect = graphics3D.GetEffect(GlobalMembersResourcesWP.EFFECT_SCREEN_DISTORT);
				float[] array2 = new float[4];
				array2[0] = (array2[1] = GlobalMembers.M(0.02f));
				effect.SetVector4("Params", array2);
				Rect rect = new Rect(0, 0, image.mWidth, image.mHeight);
				using (RenderEffectAutoState renderEffectAutoState2 = new RenderEffectAutoState(g, effect))
				{
					while (!renderEffectAutoState2.IsDone())
					{
						List<DistortEffect>.Enumerator enumerator2 = mDistortEffectList.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							DistortEffect current2 = enumerator2.Current;
							double num6 = (double)current2.mTexturePos;
							float num4 = (float)(double)current2.mRadius;
							Rect theTRect = new Rect((int)GlobalMembers.S((double)current2.mCenter.mX + (double)current2.mMoveDelta.mX * (double)current2.mMovePct - (double)num4), (int)GlobalMembers.S((double)current2.mCenter.mY + (double)current2.mMoveDelta.mY * (double)current2.mMovePct - (double)num4), (int)(GlobalMembers.S(num4) * 2f), (int)(GlobalMembers.S(num4) * 2f));
							theTRect = rect.Intersection(theTRect);
							g.DrawImage(image, theTRect, theTRect);
						}
						renderEffectAutoState2.NextPass();
					}
				}
				graphics3D.SetTexture(1, null);
				sharedRenderTarget.Unlock();
				g.PopState();
				return;
			}
			SharedRenderTarget sharedRenderTarget2 = new SharedRenderTarget();
			Image theImage = sharedRenderTarget2.LockScreenImage("DistortQuadA");
			graphics3D.SetTextureLinearFilter(0, true);
			graphics3D.SetTextureLinearFilter(1, true);
			effect = graphics3D.GetEffect(GlobalMembersResourcesWP.EFFECT_SCREEN_DISTORT);
			float[] array3 = new float[4];
			array3[0] = (array3[1] = GlobalMembers.M(0.02f));
			effect.SetVector4("Params", array3);
			graphics3D.SetTexture(1, heightImage);
			int num5 = ((mBoard != null) ? mBoard.mDistortionQuads.Count : 0);
			using (RenderEffectAutoState renderEffectAutoState3 = new RenderEffectAutoState(g, effect))
			{
				while (!renderEffectAutoState3.IsDone())
				{
					for (int i = 0; i < num5; i++)
					{
						Board.DistortionQuad distortionQuad = mBoard.mDistortionQuads[i];
						FRect fRect = new FRect(distortionQuad.x1, distortionQuad.y1, distortionQuad.x2 - distortionQuad.x1, distortionQuad.y2 - distortionQuad.y1);
						BltDoubleFromSrcRect(g, theImage, fRect, fRect, new Color(255, 255, 255, 64));
					}
					renderEffectAutoState3.NextPass();
				}
			}
			sharedRenderTarget2.Unlock();
			graphics3D.SetTexture(1, null);
			theImage = sharedRenderTarget2.LockScreenImage("DistortQuadB");
			for (int j = 0; j < num5; j++)
			{
				Board.DistortionQuad distortionQuad2 = mBoard.mDistortionQuads[j];
				FRect fRect2 = new FRect(distortionQuad2.x1, distortionQuad2.y1, distortionQuad2.x2 - distortionQuad2.x1, distortionQuad2.y2 - distortionQuad2.y1);
				BltDoubleFromSrcRect(g, theImage, fRect2, fRect2, new Color(255, 255, 255, 255));
			}
			sharedRenderTarget2.Unlock();
		}
	}
}
