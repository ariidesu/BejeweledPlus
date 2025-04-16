using System;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Widget
{
	internal class HyperspaceWhirlpool : Hyperspace
	{
		public enum HyperSpaceState
		{
			HyperSpaceState_Init = 0,
			HyperSpaceState_CloseBoard = 1,
			HyperSpaceState_SlideOver = 2,
			HyperSpaceState_Whirlpool = 3,
			HyperSpaceState_PortalRide = 4,
			HyperSpaceState_SlideBack = 5,
			HyperSpaceState_OpenBoard = 6,
			HyperSpaceState_Complete = 7,
			HyperSpaceState_DebugDrawEveryOther = 8,
			HyperSpaceState_Max = 9,
			HyperSpaceState_Nil = -1
		}

		public Board mBoard;

		public bool mSlidingHUD;

		public bool mTransitionBoard;

		public HyperSpaceState mState;

		public bool mIs3d;

		public GlobalMembers.HyperRing[] mHyperRings;

		public float mRingRotAcc;

		public float mRingRotAcc2;

		public float mRingXAcc;

		public float mRingXAcc2;

		public float mRingYAcc;

		public float mRingYAcc2;

		public float mCameraX;

		public float mCameraY;

		public float mScoreFloaterX;

		public float mScoreFloaterY;

		public int mPortalDelay;

		public float mPortalPercent;

		public bool mShowBkg;

		public int mFlashDelay;

		public float mFlashPercent;

		public float mTransPercent;

		public bool mIsDone;

		public int mDoneDelay;

		public int mEndSoundDelay;

		public int mEndTextPos;

		public float mPictureRingPos;

		public float mYOffset;

		public float mYOffsetVel;

		public float mYOffset2;

		public float mYOffset2Vel;

		public int mEffectUpdate;

		public float mWarpLineAcc;

		public float mWarpLineAdd;

		public float mWarpLineSpeed;

		public float mShakeFactor;

		public float mStretchFactor;

		public float mStretchVel;

		public bool mFirstShowBkg;

		public int mTransitionPos;

		public float mWhirlpoolFrame;

		public double mWhirlpoolFade;

		public double mWhirlpoolRot;

		public double mWhirlpoolRotAcc;

		private GlobalMembers.WarpPoint[,] mWarpPoints = new GlobalMembers.WarpPoint[GlobalMembers.NUM_WARP_ROWS, GlobalMembers.NUM_WARP_COLS];

		public float mWarpSpeed;

		public int mWarpDelay;

		public int mUISuckDelay;

		public float mWarpSizeMult;

		public CurvedVal mUIWarpPercentAdd = new CurvedVal();

		public double mUIWarpPercent;

		public bool mFirstWhirlDraw;

		public int mHyperspaceDelay;

		public float mInterfaceRestorePercent;

		private static SexyVertex2D[,] DWS_aTriVertices = new SexyVertex2D[GlobalMembers.NUM_WARP_COLS * GlobalMembers.NUM_WARP_ROWS * 2, 3];

		private static SexyVertex2D[,] DP_vertices = new SexyVertex2D[GlobalMembers.NUM_HYPER_RINGS * GlobalMembers.NUM_RING_POINTS * 2, 3];

		public void EndWhirlpool(bool bFromLoading)
		{
		}

		public void EndWhirlpool()
		{
			EndWhirlpool(false);
		}

		public void UpdateWhirlpool()
		{
			if (mWhirlpoolFade > 0.0)
			{
				mWhirlpoolFrame -= GlobalMembers.M(0.1f);
				if (mWhirlpoolFrame < 0f)
				{
					mWhirlpoolFrame += 5f;
				}
				mWhirlpoolRotAcc += GlobalMembers.M(0.0005f);
				mWhirlpoolRot -= mWhirlpoolRotAcc;
				MarkDirty();
			}
			mWhirlpoolFade += 0.02;
			if (mWhirlpoolFade > 1.0)
			{
				mWhirlpoolFade = 1.0;
			}
			mWarpSizeMult += 0.001f;
			if (mWarpDelay > 0)
			{
				mWarpDelay--;
			}
			else
			{
				mWarpSpeed += 0.02f;
			}
			if (mUISuckDelay > 0)
			{
				mUISuckDelay--;
			}
			else
			{
				mUIWarpPercentAdd.IncInVal(0.05000000074505806);
				mUIWarpPercent += mUIWarpPercentAdd.GetOutVal() * (double)ConstantsWP.WHIRLPOOL_BKG_WARP_SPEED;
				if (mUIWarpPercent > 1.0)
				{
					mUIWarpPercent = 1.0;
				}
			}
			int num = GlobalMembers.gApp.mWidth;
			int num2 = GlobalMembers.gApp.mHeight;
			for (int i = 1; i < GlobalMembers.NUM_WARP_ROWS - 1; i++)
			{
				for (int j = 1; j < GlobalMembers.NUM_WARP_COLS - 1; j++)
				{
					GlobalMembers.WarpPoint warpPoint = mWarpPoints[i, j];
					int a = j;
					int b = GlobalMembers.NUM_WARP_COLS - 1 - j;
					int a2 = i;
					int b2 = GlobalMembers.NUM_WARP_ROWS - 1 - i;
					int num3 = GlobalMembers.MIN(GlobalMembers.MIN(a, b), GlobalMembers.MIN(a2, b2));
					if (mWarpDelay == 0)
					{
						warpPoint.mDist -= (float)(0.35 * (double)num3 * (double)mWarpSpeed);
						if (warpPoint.mDist < 0f)
						{
							warpPoint.mDist = 0f;
						}
						warpPoint.mRot += (float)(0.001 * (double)num3 * (double)mWarpSpeed);
					}
					if (warpPoint.mRot < 0f)
					{
						warpPoint.mRot += (float)Math.PI * 2f;
					}
					else if (warpPoint.mRot > (float)Math.PI * 2f)
					{
						warpPoint.mRot -= (float)Math.PI * 2f;
					}
					int num4 = (int)((double)(warpPoint.mRot * 4096f) / (Math.PI * 2.0)) % 4096;
					warpPoint.mX = GlobalMembers.COS_TAB[num4] * warpPoint.mDist * mWarpSizeMult + (float)(num / 2);
					warpPoint.mY = GlobalMembers.SIN_TAB[num4] * warpPoint.mDist * mWarpSizeMult + (float)(num2 / 2);
					mWarpPoints[i, j] = warpPoint;
				}
			}
			MarkDirty();
		}

		public void DoWhirlpool()
		{
			if (GlobalMembers.gApp.Is3DAccelerated())
			{
				int num = GlobalMembers.gApp.mWidth;
				int num2 = GlobalMembers.gApp.mHeight;
				for (int i = 0; i < GlobalMembers.NUM_WARP_ROWS; i++)
				{
					for (int j = 0; j < GlobalMembers.NUM_WARP_COLS; j++)
					{
						GlobalMembers.WarpPoint warpPoint = mWarpPoints[i, j];
						warpPoint.mX = j * num / (GlobalMembers.NUM_WARP_COLS - 1);
						warpPoint.mY = i * num2 / (GlobalMembers.NUM_WARP_ROWS - 1);
						warpPoint.mZ = 0f;
						warpPoint.mVelX = 0f;
						warpPoint.mVelY = 0f;
						warpPoint.mU = warpPoint.mX / (float)num;
						warpPoint.mV = warpPoint.mY / (float)num2;
						float num3 = warpPoint.mX - (float)(num / 2);
						float num4 = warpPoint.mY - (float)(num2 / 2);
						warpPoint.mRot = (float)(Math.Atan2(num4, num3) + Math.PI * 2.0);
						warpPoint.mDist = (float)Math.Sqrt(num3 * num3 + num4 * num4);
						mWarpPoints[i, j] = warpPoint;
					}
				}
				mWarpSpeed = 0f;
				mWarpDelay = ConstantsWP.WHIRLPOOL_BKG_WARP_DELAY;
				mUISuckDelay = ConstantsWP.WHIRLPOOL_BKG_WARP_DELAY;
			}
			mUIWarpPercent = 0.0;
			mUIWarpPercentAdd.SetInVal(0.0);
			mWarpSizeMult = 1f;
			mWhirlpoolFade = 0.0;
			mWhirlpoolFrame = 0f;
			mWhirlpoolRot = 0.0;
			mWhirlpoolRotAcc = 0.0;
			mFirstWhirlDraw = true;
		}

		public void Draw3DWhirlpoolState(Graphics g)
		{
			if (!mShowBkg)
			{
				Graphics3D graphics3D = g.Get3D();
				g.PushState();
				graphics3D.SetBackfaceCulling(0, 0);
				int num = 0;
				g.SetDrawMode(Graphics.DrawMode.Normal);
				for (int i = 0; i < GlobalMembers.NUM_WARP_ROWS - 1; i++)
				{
					for (int j = 0; j < GlobalMembers.NUM_WARP_COLS - 1; j++)
					{
						GlobalMembers.WarpPoint warpPoint = mWarpPoints[i, j];
						GlobalMembers.WarpPoint warpPoint2 = mWarpPoints[i, j + 1];
						GlobalMembers.WarpPoint warpPoint3 = mWarpPoints[i + 1, j];
						GlobalMembers.WarpPoint warpPoint4 = mWarpPoints[i + 1, j + 1];
						SexyVertex2D sexyVertex2D = DWS_aTriVertices[num++, 0];
						sexyVertex2D.x = warpPoint.mX;
						sexyVertex2D.y = warpPoint.mY;
						sexyVertex2D.u = warpPoint.mU;
						sexyVertex2D.v = warpPoint.mV;
						DWS_aTriVertices[num - 1, 0] = sexyVertex2D;
						sexyVertex2D = DWS_aTriVertices[num - 1, 1];
						sexyVertex2D.x = warpPoint2.mX;
						sexyVertex2D.y = warpPoint2.mY;
						sexyVertex2D.u = warpPoint2.mU;
						sexyVertex2D.v = warpPoint2.mV;
						DWS_aTriVertices[num - 1, 1] = sexyVertex2D;
						sexyVertex2D = DWS_aTriVertices[num - 1, 2];
						sexyVertex2D.x = warpPoint4.mX;
						sexyVertex2D.y = warpPoint4.mY;
						sexyVertex2D.u = warpPoint4.mU;
						sexyVertex2D.v = warpPoint4.mV;
						DWS_aTriVertices[num - 1, 2] = sexyVertex2D;
						sexyVertex2D = DWS_aTriVertices[num++, 0];
						sexyVertex2D.x = warpPoint.mX;
						sexyVertex2D.y = warpPoint.mY;
						sexyVertex2D.u = warpPoint.mU;
						sexyVertex2D.v = warpPoint.mV;
						DWS_aTriVertices[num - 1, 0] = sexyVertex2D;
						sexyVertex2D = DWS_aTriVertices[num - 1, 1];
						sexyVertex2D.x = warpPoint4.mX;
						sexyVertex2D.y = warpPoint4.mY;
						sexyVertex2D.u = warpPoint4.mU;
						sexyVertex2D.v = warpPoint4.mV;
						DWS_aTriVertices[num - 1, 1] = sexyVertex2D;
						sexyVertex2D = DWS_aTriVertices[num - 1, 2];
						sexyVertex2D.x = warpPoint3.mX;
						sexyVertex2D.y = warpPoint3.mY;
						sexyVertex2D.u = warpPoint3.mU;
						sexyVertex2D.v = warpPoint3.mV;
						DWS_aTriVertices[num - 1, 2] = sexyVertex2D;
					}
				}
				g.DrawTrianglesTex(mBoard.mBackground.GetBackgroundImage().GetImage(), DWS_aTriVertices, num);
				g.PopState();
			}
			if (mWhirlpoolFade > 0.0)
			{
				g.SetColorizeImages(true);
				g.SetColor(new Color(255, 255, 255, (int)(255.0 * mWhirlpoolFade)));
				Image iMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE_COVER = GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE_COVER;
				int num2 = iMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE_COVER.GetWidth() / 2;
				int num3 = iMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE_COVER.GetHeight() / 2;
				int num4 = mWidth / 2 - num2;
				int num5 = mHeight / 2 - num3;
				int num6 = (int)g.mScaleX;
				int num7 = (int)g.mScaleY;
				Transform transform = new Transform();
				transform.Scale(ConstantsWP.WHIRLPOOL_IMG_SCALE, ConstantsWP.WHIRLPOOL_IMG_SCALE);
				transform.Translate(num2 + num4, num3 + num5);
				g.DrawImageTransform(iMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE_COVER, transform, 0f, 0f);
				g.mScaleX = num6;
				g.mScaleY = num7;
				g.SetColorizeImages(false);
			}
			if (mWhirlpoolFade > 0.0)
			{
				g.SetColorizeImages(true);
				g.SetDrawMode(Graphics.DrawMode.Additive);
				int num8 = (int)mWhirlpoolFrame;
				int num9 = num8 + 1;
				if (num9 == 5)
				{
					num9 = 0;
				}
				float num10 = mWhirlpoolFrame - (float)num8;
				Rect celRect = GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE.GetCelRect(num8);
				Rect celRect2 = GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE.GetCelRect(num9);
				int num11 = (mWidth - celRect.mWidth) / 2;
				int num12 = (mHeight - celRect.mHeight) / 2;
				g.SetColor(new Color(255, 255, 255, (int)(255.0 * mWhirlpoolFade * (1.0 - (double)num10))));
				Transform transform2 = new Transform();
				if (g.mColor.mAlpha > 0 && g.mColor.mAlpha <= 255)
				{
					transform2.RotateRad((float)mWhirlpoolRot);
					transform2.Scale(ConstantsWP.WHIRLPOOL_IMG_SCALE, ConstantsWP.WHIRLPOOL_IMG_SCALE);
					transform2.Translate(num11 + celRect.mWidth / 2, num12 + celRect.mHeight / 2);
					g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE, transform2, celRect, 0f, 0f);
				}
				g.SetColor(new Color(255, 255, 255, (int)(255.0 * mWhirlpoolFade * (double)num10)));
				if (g.mColor.mAlpha > 0 && g.mColor.mAlpha <= 255)
				{
					transform2.Reset();
					transform2.RotateRad((float)mWhirlpoolRot);
					transform2.Scale(ConstantsWP.WHIRLPOOL_IMG_SCALE, ConstantsWP.WHIRLPOOL_IMG_SCALE);
					transform2.Translate(num11 + celRect2.mWidth / 2, num12 + celRect2.mHeight / 2);
					g.DrawImageTransform(GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_BLACK_HOLE, transform2, celRect2, 0f, 0f);
				}
				g.SetDrawMode(Graphics.DrawMode.Normal);
				g.SetColorizeImages(false);
			}
		}

		public void DoHyperspace()
		{
		}

		public void UpdateLevelTransition()
		{
		}

		protected void Update3dPortal()
		{
			if (mBoard.IsGamePaused())
			{
				return;
			}
			if (mFlashDelay > 0)
			{
				if (--mFlashDelay == 0)
				{
					mFlashPercent = 1f;
					mShowBkg = true;
				}
				if (mFlashDelay < 20)
				{
					mFlashPercent = (float)(20 - mFlashDelay) / 20f;
					MarkDirty();
				}
			}
			else if ((double)mFlashPercent > 0.0)
			{
				mFlashPercent -= 0.015f;
				MarkDirty();
			}
			if (mShowBkg)
			{
				if (mPortalDelay > 0)
				{
					mPortalDelay--;
				}
				else if ((double)mPortalPercent < 1.0)
				{
					float num = mPortalPercent / 50f + 0.001f;
					float num2 = (1.001f - mPortalPercent) * 5f;
					if (num2 < 1f)
					{
						num *= num2;
					}
					mPortalPercent += num;
				}
				if (mEndSoundDelay > 0)
				{
					mEndSoundDelay--;
				}
				if (mDoneDelay > 0)
				{
					if (--mDoneDelay == 0)
					{
						mIsDone = true;
					}
				}
				else if ((double)mPortalPercent >= 1.0)
				{
					mPortalPercent = 1f;
					mDoneDelay = 2;
				}
				mRingRotAcc += 0.006f;
				mRingRotAcc2 += 0.0093f;
				mRingXAcc += 0.006f;
				mRingXAcc += 0.0017f;
				mRingYAcc += 0.01f;
				mRingYAcc += 0.003f;
				GlobalMembers.HyperRing hyperRing = mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1];
				hyperRing.mFromRot += (float)(Math.Sin(mRingRotAcc) * 0.0020000000949949026);
				hyperRing.mFromRot += (float)(Math.Sin(mRingRotAcc2) * 0.0010000000474974513);
				hyperRing.mToRot = hyperRing.mFromRot;
				hyperRing.mFromX += (float)(Math.Sin(mRingXAcc) * 4.0) + (float)(Math.Sin(mRingXAcc2) * 5.0);
				hyperRing.mToX = hyperRing.mFromX;
				hyperRing.mFromY += (float)(Math.Sin(mRingYAcc) * 4.0) + (float)(Math.Sin(mRingYAcc2) * 5.0);
				hyperRing.mToY = hyperRing.mFromY;
				mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1] = hyperRing;
			}
			mTransPercent += 0.25f;
			if ((double)mTransPercent >= 1.0)
			{
				for (int i = 0; i < GlobalMembers.NUM_HYPER_RINGS - 1; i++)
				{
					GlobalMembers.HyperRing hyperRing2 = mHyperRings[i];
					GlobalMembers.HyperRing hyperRing3 = mHyperRings[i + 1];
					hyperRing2.mFromX = hyperRing2.mToX;
					hyperRing2.mToX = hyperRing3.mFromX;
					hyperRing2.mFromY = hyperRing2.mToY;
					hyperRing2.mToY = hyperRing3.mFromY;
					hyperRing2.mFromRot = hyperRing2.mToRot;
					hyperRing2.mToRot = hyperRing3.mFromRot;
					mHyperRings[i] = hyperRing2;
					mHyperRings[i + 1] = hyperRing3;
				}
				mTransPercent -= 1f;
			}
			float num3 = mHyperRings[0].mCurX - mCameraX;
			float num4 = mHyperRings[0].mCurY - mCameraY;
			mCameraX += num3 * ConstantsWP.HYPERSPACE_TUNNEL_WIDTH_SCALE;
			mCameraY += num4 * ConstantsWP.HYPERSPACE_TUNNEL_WIDTH_SCALE;
			float num5 = mHyperRings[1].mCurX - mScoreFloaterX;
			float num6 = mHyperRings[1].mCurY - mScoreFloaterY;
			mScoreFloaterX += (float)((double)num5 * 0.08);
			mScoreFloaterY += (float)((double)num6 * 0.08);
			int num7 = GlobalMembers.RS(mWidth);
			int num8 = GlobalMembers.RS(mHeight);
			if ((double)mPortalPercent < 1.0)
			{
				for (int j = 0; j < GlobalMembers.NUM_HYPER_RINGS; j++)
				{
					GlobalMembers.HyperRing hyperRing4 = mHyperRings[j];
					float num9 = hyperRing4.mFromRot * (1f - mTransPercent) + hyperRing4.mToRot * mTransPercent;
					float num10 = (hyperRing4.mCurX = hyperRing4.mFromX * (1f - mTransPercent) + hyperRing4.mToX * mTransPercent);
					float num11 = (hyperRing4.mCurY = hyperRing4.mFromY * (1f - mTransPercent) + hyperRing4.mToY * mTransPercent);
					int num12 = (int)(num9 * 4096f / 3.14159f * 2f);
					if (num12 < 0)
					{
						num12 = 4096 - num12;
					}
					float num13 = 1f - mPortalPercent;
					float num14 = 800f - (float)j * num13 * 5000f / (float)GlobalMembers.NUM_HYPER_RINGS;
					float num15 = (float)GlobalMembers.MAX_Z / ((float)GlobalMembers.MAX_Z - num14);
					int num16 = (int)(200f - (float)j * num13 * 150f / (float)GlobalMembers.NUM_HYPER_RINGS);
					float num17 = 1f - mPortalPercent;
					hyperRing4.mCurScreenX = (num10 - mCameraX) * num17 * num15 + (float)(num7 / 2);
					hyperRing4.mCurScreenY = (num11 - mCameraY) * num17 * num15 + (float)(num8 / 2);
					hyperRing4.mCurScreenRadius = (float)num16 * num15;
					for (int k = 0; k < GlobalMembers.NUM_RING_POINTS; k++)
					{
						GlobalMembers.HyperPoint hyperPoint = hyperRing4.mHyperPoints[k];
						int num18 = (4096 * k / GlobalMembers.NUM_RING_POINTS + num12) % 4096;
						hyperPoint.mX = (GlobalMembers.COS_TAB[num18] * (float)num16 + (num10 - mCameraX) * num17) * num15 + (float)(num7 / 2);
						hyperPoint.mY = (GlobalMembers.SIN_TAB[num18] * (float)num16 + (num11 - mCameraY) * num17) * num15 + (float)(num8 / 2);
						hyperPoint.mV += 0.006f;
						if (hyperPoint.mV > 1f)
						{
							hyperPoint.mV -= 1f;
						}
						hyperRing4.mHyperPoints[k] = hyperPoint;
					}
					mHyperRings[j] = hyperRing4;
				}
			}
			if (mShowBkg)
			{
				MarkDirty();
			}
			else
			{
				MarkDirtyFull();
			}
		}

		protected void Draw3dPortal(Graphics g)
		{
			int num = 0;
			int num2 = 0;
			GlobalMembers.RS(mWidth);
			GlobalMembers.RS(mHeight);
			if (mShowBkg)
			{
				g.SetDrawMode(Graphics.DrawMode.Normal);
				int num3 = (int)((double)mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1].mCurScreenRadius * 2.5);
				Rect theDestRect = new Rect((int)GlobalMembers.S(mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1].mCurScreenX - (float)(num3 / 2)) + num, (int)GlobalMembers.S(mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1].mCurScreenY - (float)(num3 / 2)) + num2, GlobalMembers.S(num3), GlobalMembers.S(num3));
				if ((double)mPortalPercent < 1.0)
				{
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_TUNNELEND, theDestRect, GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_TUNNELEND.GetCelRect(0));
				}
			}
			if ((double)mPortalPercent < 1.0)
			{
				g.PushState();
				g.SetDrawMode((!mShowBkg) ? Graphics.DrawMode.Additive : Graphics.DrawMode.Normal);
				g.SetColorizeImages(true);
				Image iMAGE_HYPERSPACE_WHIRLPOOL_HYPERSPACE_NORMAL = GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_HYPERSPACE_NORMAL;
				int num4 = 0;
				for (int num5 = GlobalMembers.NUM_HYPER_RINGS - 2; num5 >= 0; num5--)
				{
					int num6 = GlobalMembers.MIN(255, 384 - num5 * 360 / GlobalMembers.NUM_HYPER_RINGS);
					Color color = new Color(num6, num6, num6, 255);
					for (int i = 0; i < GlobalMembers.NUM_RING_POINTS; i++)
					{
						GlobalMembers.HyperPoint hyperPoint = mHyperRings[num5].mHyperPoints[i];
						GlobalMembers.HyperPoint hyperPoint2 = mHyperRings[num5].mHyperPoints[(i + 1) % GlobalMembers.NUM_RING_POINTS];
						GlobalMembers.HyperPoint hyperPoint3 = mHyperRings[num5 + 1].mHyperPoints[i];
						GlobalMembers.HyperPoint hyperPoint4 = mHyperRings[num5 + 1].mHyperPoints[(i + 1) % GlobalMembers.NUM_RING_POINTS];
						SexyVertex2D sexyVertex2D = new SexyVertex2D(GlobalMembers.S(hyperPoint.mX + (float)num), GlobalMembers.S(hyperPoint.mY + (float)num2), hyperPoint.mU, hyperPoint.mV);
						SexyVertex2D sexyVertex2D2 = new SexyVertex2D(GlobalMembers.S(hyperPoint2.mX + (float)num), GlobalMembers.S(hyperPoint2.mY + (float)num2), hyperPoint2.mU, hyperPoint2.mV);
						SexyVertex2D sexyVertex2D3 = new SexyVertex2D(GlobalMembers.S(hyperPoint3.mX + (float)num), GlobalMembers.S(hyperPoint3.mY + (float)num2), hyperPoint3.mU, hyperPoint3.mV);
						SexyVertex2D sexyVertex2D4 = new SexyVertex2D(GlobalMembers.S(hyperPoint4.mX + (float)num), GlobalMembers.S(hyperPoint4.mY + (float)num2), hyperPoint4.mU, hyperPoint4.mV);
						if (!(sexyVertex2D.v < 0f))
						{
							if (i == GlobalMembers.NUM_RING_POINTS - 1)
							{
								sexyVertex2D2.u += 1f;
								sexyVertex2D4.u += 1f;
							}
							if (sexyVertex2D.v > sexyVertex2D3.v)
							{
								sexyVertex2D3.v += 1f;
								sexyVertex2D4.v += 1f;
							}
							DP_vertices[num4, 0] = sexyVertex2D;
							DP_vertices[num4, 0].color = color;
							DP_vertices[num4, 1] = sexyVertex2D2;
							DP_vertices[num4, 1].color = color;
							DP_vertices[num4, 2] = sexyVertex2D3;
							DP_vertices[num4, 2].color = color;
							num4++;
							DP_vertices[num4, 0] = sexyVertex2D2;
							DP_vertices[num4, 0].color = color;
							DP_vertices[num4, 1] = sexyVertex2D3;
							DP_vertices[num4, 1].color = color;
							DP_vertices[num4, 2] = sexyVertex2D4;
							DP_vertices[num4, 2].color = color;
							num4++;
						}
					}
				}
				Image theTexture = (mShowBkg ? iMAGE_HYPERSPACE_WHIRLPOOL_HYPERSPACE_NORMAL : GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_INITIAL);
				g.DrawTrianglesTex(theTexture, DP_vertices, num4);
				g.PopState();
			}
			if (mShowBkg && (double)mPortalPercent < 1.0)
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.SetColorizeImages(true);
				int num7 = (int)(mPortalPercent * 400f);
				if (num7 > 255)
				{
					num7 = 255;
				}
				if (num7 > 0)
				{
					g.SetColor(new Color(num7, num7, num7));
					int num3 = (int)(mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1].mCurScreenRadius * 4f);
					g.DrawImage(theDestRect: new Rect((int)GlobalMembers.S(mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1].mCurScreenX - (float)(num3 / 2) + (float)num), (int)GlobalMembers.S(mHyperRings[GlobalMembers.NUM_HYPER_RINGS - 1].mCurScreenY - (float)(num3 / 2) + (float)num2), GlobalMembers.S(num3), GlobalMembers.S(num3)), theImage: GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_FIRERING, theSrcRect: GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_FIRERING.GetCelRect(mUpdateCnt / 2 % GlobalMembersResourcesWP.IMAGE_HYPERSPACE_WHIRLPOOL_FIRERING.GetCelCount()));
				}
				g.SetColorizeImages(false);
				g.SetDrawMode(Graphics.DrawMode.Normal);
			}
			if (mFlashPercent > 0f)
			{
				int theAlpha = GlobalMembers.MIN((int)(255.0 * (double)mFlashPercent), 255);
				g.SetColor(new Color(255, 255, 255, theAlpha));
				g.FillRect(0, 0, mWidth, mHeight);
			}
		}

		public HyperspaceWhirlpool(Board theBoard)
		{
			BejeweledLivePlusApp.LoadContent("HyperspaceWhirlpool_Common");
			if (GlobalMembers.gApp.mCurrentGameMode == GameMode.MODE_ZEN)
			{
				BejeweledLivePlusApp.LoadContent("HyperspaceWhirlpool_Zen");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("HyperspaceWhirlpool_Normal");
			}
			mBoard = theBoard;
			mMouseVisible = false;
			mIsDone = false;
			mDoneDelay = 0;
			mHyperRings = new GlobalMembers.HyperRing[GlobalMembers.NUM_HYPER_RINGS];
			for (int i = 0; i < GlobalMembers.NUM_HYPER_RINGS; i++)
			{
				mHyperRings[i].Init();
			}
			mIs3d = GlobalMembers.gApp.Is3DAccelerated();
			if (!GlobalMembers.gTableInitialized)
			{
				for (int j = 0; j < 4096; j++)
				{
					GlobalMembers.SIN_TAB[j] = (float)Math.Sin((double)j * 3.14159 * 2.0 / 4096.0);
					GlobalMembers.COS_TAB[j] = (float)Math.Cos((double)j * 3.14159 * 2.0 / 4096.0);
				}
				GlobalMembers.gTableInitialized = true;
			}
			mState = HyperSpaceState.HyperSpaceState_Nil;
			SetState(HyperSpaceState.HyperSpaceState_Init);
		}

		public override void Update()
		{
			WidgetUpdate();
			HyperSpaceState hyperSpaceState;
			do
			{
				hyperSpaceState = mState;
				switch (mState)
				{
				case HyperSpaceState.HyperSpaceState_Init:
					SetState(HyperSpaceState.HyperSpaceState_CloseBoard);
					break;
				case HyperSpaceState.HyperSpaceState_CloseBoard:
					if (mTransitionBoard)
					{
						mBoard.UpdateBoardTransition(true);
					}
					else
					{
						SetState(HyperSpaceState.HyperSpaceState_SlideOver);
					}
					break;
				case HyperSpaceState.HyperSpaceState_SlideOver:
					if (mSlidingHUD)
					{
						mBoard.UpdateSlidingHUD(true);
					}
					else
					{
						SetState(HyperSpaceState.HyperSpaceState_Whirlpool);
					}
					break;
				case HyperSpaceState.HyperSpaceState_Whirlpool:
					mBoard.mSideAlpha.SetConstant(1.0);
					mBoard.mShowBoard = false;
					UpdateWhirlpool();
					if (mUIWarpPercent == 1.0)
					{
						SetState(HyperSpaceState.HyperSpaceState_PortalRide);
					}
					break;
				case HyperSpaceState.HyperSpaceState_PortalRide:
					if (!mShowBkg)
					{
						UpdateWhirlpool();
					}
					Update3dPortal();
					if (mShowBkg && !mBoard.mShowBoard)
					{
						mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_Start);
						mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_OldLevelClear);
						mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_ZoomIn);
					}
					if (mIsDone)
					{
						SetState(HyperSpaceState.HyperSpaceState_SlideBack);
					}
					break;
				case HyperSpaceState.HyperSpaceState_SlideBack:
					if (mSlidingHUD)
					{
						mBoard.UpdateSlidingHUD(false);
					}
					else
					{
						SetState(HyperSpaceState.HyperSpaceState_OpenBoard);
					}
					break;
				case HyperSpaceState.HyperSpaceState_OpenBoard:
					if (mTransitionBoard)
					{
						mBoard.UpdateBoardTransition(false);
					}
					else
					{
						SetState(HyperSpaceState.HyperSpaceState_Complete);
					}
					break;
				}
			}
			while (mState != hyperSpaceState);
		}

		public override void Draw(Graphics g)
		{
			Graphics3D graphics3D = g.Get3D();
			if (graphics3D == null)
			{
				return;
			}
			switch (mState)
			{
			case HyperSpaceState.HyperSpaceState_Whirlpool:
				Draw3DWhirlpoolState(g);
				break;
			case HyperSpaceState.HyperSpaceState_PortalRide:
				if (!mShowBkg)
				{
					Draw3DWhirlpoolState(g);
				}
				graphics3D.ClearDepthBuffer();
				Draw3dPortal(g);
				break;
			case HyperSpaceState.HyperSpaceState_SlideOver:
			case HyperSpaceState.HyperSpaceState_SlideBack:
				break;
			}
		}

		public void InterfaceOrientationChanged(UI_ORIENTATION theOrientation)
		{
			Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
		}

		public override float GetPieceAlpha()
		{
			return 1f;
		}

		public override bool IsUsing3DTransition()
		{
			return true;
		}

		public void SetState(HyperSpaceState state)
		{
			if (state == mState)
			{
				return;
			}
			mState = state;
			switch (mState)
			{
			case HyperSpaceState.HyperSpaceState_Init:
			{
				GlobalMembers.gApp.DisableOptionsButtons(true);
				GlobalMembers.gApp.mBoard.DisableUI(false);
				mWhirlpoolFade = 0.0;
				mWhirlpoolRot = 0.0;
				mWhirlpoolRotAcc = 0.0;
				mInterfaceRestorePercent = 0f;
				mBoard.mFirstDraw = true;
				mFirstWhirlDraw = false;
				mWarpSpeed = 0f;
				mUIWarpPercentAdd.SetOutRange(0.0, 1.0);
				mUIWarpPercentAdd.SetMode(0);
				mUIWarpPercentAdd.SetRamp(2);
				DoWhirlpool();
				mFlashDelay = ConstantsWP.WHIRLPOOL_TO_HYPERSPACE_FADE_TIME;
				mFlashPercent = 0f;
				mShowBkg = false;
				mRingRotAcc = 0f;
				mRingRotAcc2 = 0f;
				mTransPercent = 0f;
				mRingXAcc = 0f;
				mRingXAcc2 = 0f;
				mRingYAcc = 0f;
				mRingYAcc2 = 0f;
				mScoreFloaterX = 0f;
				mScoreFloaterY = 0f;
				mCameraX = 0f;
				mCameraY = 0f;
				mPortalDelay = ConstantsWP.HYPERSPACE_TUNNEL_TIME;
				mEndSoundDelay = 100;
				mPortalPercent = 0f;
				for (int i = 0; i < GlobalMembers.NUM_HYPER_RINGS; i++)
				{
					GlobalMembers.HyperRing hyperRing = mHyperRings[i];
					hyperRing.mFromRot = 0f;
					hyperRing.mToRot = 0f;
					hyperRing.mFromX = 0f;
					hyperRing.mFromY = 0f;
					hyperRing.mToX = 0f;
					hyperRing.mToY = 0f;
					hyperRing.mCurX = 0f;
					hyperRing.mCurY = 0f;
					hyperRing.mCurScreenX = 0f;
					hyperRing.mCurScreenY = 0f;
					for (int j = 0; j < GlobalMembers.NUM_RING_POINTS; j++)
					{
						GlobalMembers.HyperPoint hyperPoint = hyperRing.mHyperPoints[j];
						hyperPoint.mU = (float)j / (float)GlobalMembers.NUM_RING_POINTS;
						hyperPoint.mV = (float)i * 0.02f - (float)GlobalMembers.NUM_HYPER_RINGS * 0.02f;
						hyperRing.mHyperPoints[j] = hyperPoint;
					}
					mHyperRings[i] = hyperRing;
				}
				mEndTextPos = 800;
				mYOffset = 4f;
				mYOffsetVel = 0.3f;
				mYOffset2 = 0f;
				mYOffset2Vel = 0f;
				mEffectUpdate = 0;
				mWarpLineAcc = 0f;
				mWarpLineAdd = 0f;
				mWarpLineSpeed = 0.12f;
				mShakeFactor = 0f;
				mStretchFactor = 1f;
				mStretchVel = 0f;
				mFirstShowBkg = true;
				mSlidingHUD = false;
				mTransitionBoard = false;
				break;
			}
			case HyperSpaceState.HyperSpaceState_CloseBoard:
				mTransitionBoard = true;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_TRANSITION_BOARD_CURVE_CLOSE, mBoard.mTransitionBoardCurve);
				break;
			case HyperSpaceState.HyperSpaceState_SlideOver:
				mSlidingHUD = true;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SLIDING_HUD_CURVE_OVER, mBoard.mSlidingHUDCurve);
				break;
			case HyperSpaceState.HyperSpaceState_Whirlpool:
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HYPERSPACE);
				break;
			case HyperSpaceState.HyperSpaceState_PortalRide:
				mBoard.RandomizeBoard();
				mBoard.MoveGemsOffscreen();
				break;
			case HyperSpaceState.HyperSpaceState_SlideBack:
				mSlidingHUD = true;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SLIDING_HUD_CURVE_BACK, mBoard.mSlidingHUDCurve);
				break;
			case HyperSpaceState.HyperSpaceState_OpenBoard:
				mTransitionBoard = true;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_TRANSITION_BOARD_CURVE_OPEN, mBoard.mTransitionBoardCurve);
				break;
			case HyperSpaceState.HyperSpaceState_Complete:
				mWidgetManager.SetFocus(mBoard);
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_Finish);
				GlobalMembers.gApp.DisableOptionsButtons(false);
				GlobalMembers.gApp.mBoard.DisableUI(false);
				break;
			}
		}
	}
}
