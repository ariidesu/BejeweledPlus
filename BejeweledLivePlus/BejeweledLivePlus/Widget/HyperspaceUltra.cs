using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using System;
using System.Collections.Generic;

namespace BejeweledLivePlus.Widget
{
	internal class HyperspaceUltra : Hyperspace
	{
		public enum HyperSpaceState
		{
			Init = 0,
			SlideOver = 1,
			FadeTo3D = 2,
			GemRise = 3,
			GemFly = 4,
			BoardShatter = 5,
			PortalRide = 6,
			LandOnBoard = 7,
			SlideBack = 8,
			FadeFrom3D = 9,
			Outro = 10,
			Complete = 11,
			FullySlideOver = 12,
			DebugDrawEveryOther = 13,
			Nil = -1
		}

		public Board mBoard;
		public BoardInfo mBoardInfo = new BoardInfo();
		public GemInfo[,] mGemInfo = new GemInfo[GlobalMembers.NUM_ROWS, GlobalMembers.NUM_COLS];
		public List<GemInfo> mGemRenderOrder = new List<GemInfo>();
		public Graphics3D.PerspectiveCamera mCameraPersp = new Graphics3D.PerspectiveCamera();
		public SharedImageRef mBGImage;
		public HyperAnimSequence mAnimSeq;

		public CurvedVal mBGScale = new CurvedVal();
		public CurvedVal mXOffsetAnim = new CurvedVal();
		public CurvedVal mMinAlpha = new CurvedVal();
		public CurvedVal mShatterScale = new CurvedVal();
		public CurvedVal mWarpTubeTextureFade = new CurvedVal();
		public CurvedVal mRingFadeTunnelIn = new CurvedVal();
		public CurvedVal mRingFadeTunnelOut = new CurvedVal();
		public CurvedVal mPieceAlpha = new CurvedVal();
		public CurvedVal mBoardCenterSlide = new CurvedVal();

		public float mFadeTo3D;
		public float mFadeFrom3D;
		public float mBoardSlideFromX;
		public float mBoardSlideToX;
		public float mBoardSlideFromY;
		public float mBoardSlideToY;
		public int mGemHitCount;
		public int mGemHitTick;
		public SexyVector3 mBoardScreenPos = new SexyVector3(0f, 0f, 0f);
		public float mTicks;
		public float mUVAnimTicks;
		public float mStateStartTick;
		public HyperSpaceState mState;
		private bool mSlideBackStarted;
		private const float sGemMeshScale = 0.7f;

		private static readonly HyperMaterial[] mapColorIndexToMaterial = new HyperMaterial[]
		{
			new HyperMaterial(new[]{0.6f,0.6f,0.6f,0.6f}, new[]{1f,1f,1f,1f}, new[]{1f,1f,1f,0.6f}, 55f),
			new HyperMaterial(new[]{0.7f,0.7f,0.7f,0.7f}, new[]{1f,1f,1f,1f}, new[]{1f,1f,1f,0.6f}, 55f),
			new HyperMaterial(new[]{0.7f,0.7f,0.7f,0.7f}, new[]{1f,1f,1f,1f}, new[]{1f,1f,1f,0.6f}, 55f),
			new HyperMaterial(new[]{0.7f,0.7f,0.7f,0.7f}, new[]{1f,1f,1f,1f}, new[]{1f,1f,1f,0.6f}, 55f),
			new HyperMaterial(new[]{0.8f,0.8f,0.8f,0.7f}, new[]{1f,1f,1f,1f}, new[]{1f,1f,1f,0.6f}, 55f),
			new HyperMaterial(new[]{0.7f,0.7f,0.7f,0.7f}, new[]{1f,1f,1f,1f}, new[]{1f,1f,1f,0.6f}, 55f),
			new HyperMaterial(new[]{0.8f,0.8f,0.8f,0.7f}, new[]{1f,1f,1f,1f}, new[]{1f,1f,1f,0.6f}, 45f),
		};

		private static readonly SexyVector3[] mapColorIndexToLightOffset = new SexyVector3[]
		{
			new SexyVector3(50f, 150f, -200f),
			new SexyVector3(50f, 150f, -200f),
			new SexyVector3(50f, 150f, -200f),
			new SexyVector3(-100f, -100f, -200f),
			new SexyVector3(100f, 50f, 0f),
			new SexyVector3(50f, 150f, -200f),
			new SexyVector3(-100f, 200f, 0f),
		};

        private static readonly int[] mapHitCountToSound = new[] {
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_1,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_2,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_3,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_4,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_5,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_6,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_7
        };

        private static readonly int[] mapHitCountToZenSound = new[] {
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_ZEN_1,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_ZEN_2,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_ZEN_3,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_ZEN_4,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_ZEN_5,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_ZEN_6,
            GlobalMembersResourcesWP.SOUND_HYPERSPACE_GEM_LAND_ZEN_7
        };

        private static readonly Image[] indexToWarpLines = new[] {
            GlobalMembersResourcesWP.IMAGE_WARP_LINES_01,
            GlobalMembersResourcesWP.IMAGE_WARP_LINES_02,
            GlobalMembersResourcesWP.IMAGE_WARP_LINES_03,
            GlobalMembersResourcesWP.IMAGE_WARP_LINES_04
        };

        private static readonly float[] ambientLightColor = { 1f, 1f, 1f, 1f };
		private static readonly float[] diffuseLightColor = { 0.6f, 0.6f, 0.6f, 1f };
		private static readonly float[] specularLightColor = { 1f, 1f, 1f, 1f };

		public HyperspaceUltra(Board theBoard)
		{
			mBoard = theBoard;
			mMouseVisible = false;
			mAnimSeq = GlobalMembers.gApp.mHyperSpaceAnims[0];
			mAnimSeq.Reset();
			mCameraPersp.Init(GetFocalLength(), (float)GlobalMembers.gApp.mWidth / GlobalMembers.gApp.mHeight, 100f, GetZFarClip());
			mBoardInfo.Init(theBoard, mAnimSeq);
			for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
			{
				for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
				{
					mGemInfo[row, col] = new GemInfo();
					mGemInfo[row, col].Init(theBoard.mBoard[row, col], mAnimSeq);
				}
			}
			mState = HyperSpaceState.Nil;
			SetState(HyperSpaceState.Init);
		}

		public override float GetPieceAlpha()
		{
			return (float)mPieceAlpha.GetOutVal();
		}

		public override bool IsUsing3DTransition()
		{
			return true;
		}

		public override bool ShouldDrawBoardEffects()
		{
			return mState < HyperSpaceState.FadeTo3D || mState >= HyperSpaceState.LandOnBoard;
		}

		public override void SetBGImage(SharedImageRef inImage)
		{
			mBGImage = inImage;
		}

		public override void SkipToPortalRide()
		{
			SetState(HyperSpaceState.PortalRide);
		}

		private float GetFocalLength()
		{
			float anAspectRatio = (float)GlobalMembers.gApp.mWidth / GlobalMembers.gApp.mHeight;
			return 19.9f * anAspectRatio;
        }

		private float GetZFarClip()
		{
			switch (mState)
			{
			case HyperSpaceState.GemRise:
			case HyperSpaceState.GemFly:
			case HyperSpaceState.BoardShatter:
				return 30000f;
			default:
				return 100000f;
			}
		}

		public override void Update()
		{
			base.Update();
			mTicks += 1f;
			HyperSpaceState prev;
			do
			{
				prev = mState;
				switch (mState)
				{
				case HyperSpaceState.Init:
					SetState(HyperSpaceState.SlideOver);
					break;
				case HyperSpaceState.SlideOver:
					if (mSlidingHUD)
					{
						mBoard.UpdateSlidingHUD(true);
						UpdateBoardCenterSlide();
					}
					else
					{
						UpdateBoardCenterSlide();
					}
					if (mTicks - mStateStartTick > 160f)
						SetState(HyperSpaceState.FadeTo3D);
					break;
				case HyperSpaceState.FadeTo3D:
					if (mPieceAlpha.HasBeenTriggered())
						SetState(HyperSpaceState.GemRise);
					break;
				case HyperSpaceState.GemRise:
					if (mAnimSeq.GetCurFrame() >= 20)
						SetState(HyperSpaceState.GemFly);
					break;
				case HyperSpaceState.GemFly:
					if (mAnimSeq.GetCurFrame() >= 65)
						SetState(HyperSpaceState.BoardShatter);
					break;
				case HyperSpaceState.BoardShatter:
					if (mAnimSeq.GetCurFrame() >= 95)
						SetState(HyperSpaceState.PortalRide);
					break;
				case HyperSpaceState.PortalRide:
                    UpdateBoardCenterSlide();
                    if (mAnimSeq.GetCurFrame() >= 250)
						SetState(HyperSpaceState.LandOnBoard);
					break;
				case HyperSpaceState.LandOnBoard:
					if (mSlidingHUD)
						mBoard.UpdateSlidingHUD(false);
                    UpdateBoardCenterSlide();
                    if (mAnimSeq.IsComplete() && mSlideBackStarted)
						SetState(HyperSpaceState.SlideBack);
						break;
				case HyperSpaceState.SlideBack:
                    SetState(HyperSpaceState.FadeFrom3D);
                    break;
				case HyperSpaceState.FadeFrom3D:
					if (mFadeFrom3D <= 0f)
						SetState(HyperSpaceState.Outro);
					break;
				case HyperSpaceState.Outro:
                    if (mSlidingHUD)
                    {
                        mBoard.UpdateSlidingHUD(false);
                        UpdateBoardCenterSlide();
                    }
                    else
                    {
                        UpdateBoardCenterSlide();
                    }
                    if (mTicks - mStateStartTick > 0f)
						SetState(HyperSpaceState.Complete);
					break;
                case HyperSpaceState.Complete:
					if (mSlidingHUD)
					{
						mBoard.UpdateSlidingHUD(false);
						UpdateBoardCenterSlide();
					}
					else
					{
						UpdateBoardCenterSlide();
					}
					if (!mSlidingHUD && !mBoardCenterSlide.IsDoingCurve())
						SetState(HyperSpaceState.FullySlideOver);
                    break;
                }
			} while (mState != prev);

			UpdateAnimation();
			UpdateCamera();
			UpdateTransitionTo3D();
			UpdateBackground();
			Update3DGems();
			Update3DBoard();
			Update3DPortal();
			UpdateSounds();
			MarkDirtyFull();
		}

		public void SetState(HyperSpaceState state)
		{
			if (state == mState) return;
			mStateStartTick = mTicks;
			mState = state;
			switch (mState)
			{
			case HyperSpaceState.Init:
				mTicks = 0f;
				mAnimSeq.Reset();
				mMinAlpha.SetConstant(0.0);
				mXOffsetAnim.SetConstant(0.0);
				mRingFadeTunnelIn.SetConstant(0.0);
				mRingFadeTunnelOut.SetConstant(0.0);
				mWarpTubeTextureFade.SetConstant(1.0);
				mBGScale.SetConstant(1.0);
				mShatterScale.SetConstant(1.0);
				mPieceAlpha.SetConstant(1.0);
				mFadeTo3D = 1f;
				mFadeFrom3D = 0f;
				mGemHitCount = 0;
				mGemHitTick = (int)mTicks;
				mBoardCenterSlide.SetConstant(0.0);
				mBoardSlideFromX = 0f;
				mBoardSlideToX = 0f;
				mBoardSlideFromY = 0f;
				mBoardSlideToY = 0f;
				mSlideBackStarted = false;
				for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
					for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
						mGemInfo[row, col].mDraw3D = false;
				mSlidingHUD = false;
				mTransitionBoard = false;
				mBoard.mSlideBoardComponentsWithHUD = false;
                break;
			case HyperSpaceState.SlideOver:
				mFadeTo3D = 0f;
				mSlidingHUD = true;
				StartBoardCenterSlide(GetBoardCenterOffsetX(), GetBoardCenterOffsetY());
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SLIDING_HUD_CURVE_OVER, mBoard.mSlidingHUDCurve);
				mXOffsetAnim.SetCurve(GlobalMembers.MP("b+0,-234,0.009091,1,####         ~~auJ"));
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_Start);
				break;
			case HyperSpaceState.FadeTo3D:
                mPieceAlpha.ClearTrigger();
				mPieceAlpha.SetCurve(GlobalMembers.MP("b-0.02,1,0.2,1,~rgP         ~#DgP"));
				for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
					for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
						mGemInfo[row, col].mDraw3D = true;
				break;
			case HyperSpaceState.GemRise:
                (GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("FadeOut", (GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).mSongName, false);
                GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HYPERSPACE);
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_HideAll);
				mUVAnimTicks = 0f;
				mBGScale.SetCurve(GlobalMembers.MP("b+1,8,0.004762,1,####     l#Pr]'#*NA    1}dR)"));
				mMinAlpha.SetCurve(GlobalMembers.MP("b+0,1,0.004444,1,####Q####         O}P8x"));
				mRingFadeTunnelIn.SetCurve(GlobalMembers.MP("b+0,255,0.004255,1,##xa  @L6zN d~}Q&    I~P## T#<G{"));
				mWarpTubeTextureFade.SetCurve(GlobalMembers.MP("b+0,1,0.004348,1,####      W+(q>   I~cu?"));

				Image aWarpLine = indexToWarpLines[Misc.Common.Rand() % indexToWarpLines.Length];
				GlobalMembers.gApp.mHyperTube3DListener.SetTexture(0, aWarpLine);

				if (mBoard.GetHyperspaceTransType() == HYPERSPACETRANS.HYPERSPACETRANS_Zen)
                    GlobalMembers.gApp.mHyperTube3DListener.SetTexture(1, GlobalMembersResourcesWP.IMAGE_HYPERSPAZEN_INITIAL);
                else
                    GlobalMembers.gApp.mHyperTube3DListener.SetTexture(1, GlobalMembersResourcesWP.IMAGE_HYPERSPACE_INITIAL);
                break;
			case HyperSpaceState.GemFly:
				mXOffsetAnim.SetConstant(0.0);
				mPieceAlpha.SetConstant(1.0);
				break;
			case HyperSpaceState.BoardShatter:
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_BoardShatter);
				mShatterScale.SetCurve(GlobalMembers.MP("b+0.8,2.4,0.008333,1,#.ov         ~~###"));
				if (mBoard.GetHyperspaceTransType() == HYPERSPACETRANS.HYPERSPACETRANS_Zen)
				{
					GlobalMembersResourcesWP.POPANIM_ANIMS_BOARDSHATTER?.Play("zen");
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HYPERSPACE_SHATTER_ZEN);
				}
				else {
                    GlobalMembersResourcesWP.POPANIM_ANIMS_BOARDSHATTER?.Play("shatter");
                    GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HYPERSPACE_SHATTER_1);
                }
				break;
			case HyperSpaceState.PortalRide:
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_OldLevelClear);
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_NextBkg);
				mBGScale.SetConstant(1.0);
				mMinAlpha.SetConstant(1.0);
				mRingFadeTunnelIn.SetConstant(0.0);
				mWarpTubeTextureFade.SetConstant(1.0);
                if (mBoard.GetHyperspaceTransType() == HYPERSPACETRANS.HYPERSPACETRANS_Zen)
                    GlobalMembers.gApp.mHyperTube3DListener.SetTexture(1, GlobalMembersResourcesWP.IMAGE_HYPERSPAZEN);
                else
                    GlobalMembers.gApp.mHyperTube3DListener.SetTexture(1, GlobalMembersResourcesWP.IMAGE_HYPERSPACE);
                break;
			case HyperSpaceState.LandOnBoard:
				float savedOfsX = mBoard.mOfsX;
				float savedOfsY = mBoard.mOfsY;
				for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
				{
					for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
					{
						GemInfo gi = mGemInfo[row, col];
						if (gi.mBoardHitFrame > mAnimSeq.GetCurFrame())
							gi.mPiece.mAlpha.SetConstant(0.0);
						else
							gi.mDraw3D = false;
					}
				}
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_ZoomIn);
				mBoard.mOfsX = savedOfsX;
				mBoard.mOfsY = savedOfsY;
				break;
			case HyperSpaceState.SlideBack:
				StartSlideBack();
				break;
			case HyperSpaceState.FadeFrom3D:
				mFadeFrom3D = 1f;
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_SlideOver);
				break;
			case HyperSpaceState.Outro:
				for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
				{
					for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
					{
						GemInfo gi = mGemInfo[row, col];
						gi.mPiece.mAlpha.SetConstant(1.0);
						gi.mDraw3D = false;
						gi.mPiece.ClearHyperspaceEffects();
					}
				}
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_SlideOver);
                mSlidingHUD = true;
                GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SLIDING_HUD_CURVE_BACK, mBoard.mSlidingHUDCurve);
                break;
			case HyperSpaceState.Complete:
				ClearHyperspaceGemEffects();
				mBoard.mSlideBoardComponentsWithHUD = false;
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_Finish);
				mSlidingHUD = true;
				StartBoardCenterSlide(0f, 0f);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SLIDING_HUD_CURVE_BACK, mBoard.mSlidingHUDCurve);
                (GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("FadeIn", (GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).mSongName, false);
                break;
			case HyperSpaceState.FullySlideOver:
                mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_UltraFullySlideOver);
				break;
            }
		}

		private void StartSlideBack()
		{
			if (mSlideBackStarted)
				return;

			mSlideBackStarted = true;
			mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_SlideOver);
			mSlidingHUD = true;
			StartBoardCenterSlide(0f, 0f);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBOARD_SLIDING_HUD_CURVE_BACK, mBoard.mSlidingHUDCurve);
		}

		private bool HaveAll3DGemsLanded()
		{
			for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
			{
				for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
				{
					if (mGemInfo[row, col].mDraw3D)
						return false;
				}
			}
			return true;
		}

		private void ClearHyperspaceGemEffects()
		{
			for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
			{
				for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
				{
					mBoard.mBoard[row, col]?.ClearHyperspaceEffects();
					mGemInfo[row, col].mPiece?.ClearHyperspaceEffects();
				}
			}
		}

		private float GetBoardCenterOffsetX()
		{
			float boardCenterX = mBoard.mWidth * 0.5f;
			return boardCenterX - GlobalMembers.S(mBoard.GetBoardCenterX());
		}

		private float GetBoardCenterOffsetY()
		{
			float boardCenterY = mBoard.mHeight * 0.5f;
			return boardCenterY - GlobalMembers.S(mBoard.GetBoardCenterY());
		}

		private void StartBoardCenterSlide(float targetX, float targetY)
		{
			mBoardSlideFromX = GlobalMembers.S(mBoard.mOfsX);
			mBoardSlideToX = targetX;
			mBoardSlideFromY = GlobalMembers.S(mBoard.mOfsY);
			mBoardSlideToY = targetY;
			mBoardCenterSlide.SetCurve(GlobalMembers.MP("b+0,1,0.00625,1,####    G####     Y~Ws|"));
			UpdateBoardCenterSlide();
		}

		private void UpdateBoardCenterSlide()
		{
			mBoardCenterSlide.IncInVal();
			float pct = (float)mBoardCenterSlide.GetOutVal();
			mBoard.mOfsX = (mBoardSlideFromX + (mBoardSlideToX - mBoardSlideFromX) * pct) / GlobalMembers.S(1f);
			mBoard.mOfsY = (mBoardSlideFromY + (mBoardSlideToY - mBoardSlideFromY) * pct) / GlobalMembers.S(1f);
		}

		private void UpdateAnimation()
		{
			switch (mState)
			{
			case HyperSpaceState.Init:
			case HyperSpaceState.SlideOver:
			case HyperSpaceState.DebugDrawEveryOther:
			case HyperSpaceState.FadeTo3D:
			case HyperSpaceState.SlideBack:
			case HyperSpaceState.Complete:
			case HyperSpaceState.FullySlideOver:
				return;
			}
			mAnimSeq.Tick();
		}

		private void UpdateCamera()
		{
			mCameraPersp.Init(GetFocalLength(), (float)GlobalMembers.gApp.mWidth / GlobalMembers.gApp.mHeight, 100f, GetZFarClip());
			float xCamOff = 0f;
			switch (mState)
			{
			case HyperSpaceState.SlideOver:
			case HyperSpaceState.FadeTo3D:
			case HyperSpaceState.GemRise:
				mXOffsetAnim.IncInVal();
				xCamOff = 234f + (float)mXOffsetAnim.GetOutVal();
				break;
			}
			SexyVector3 rot = mAnimSeq.GetCameraRot();
			SexyVector3 pos = mAnimSeq.GetCameraPos();
			SexyVector3 cameraWorldPos = new SexyVector3(pos.x - xCamOff, pos.y, pos.z);

			SexyCoords3 c = new SexyCoords3();
			c.RotateRadX(rot.x);
			c.RotateRadY(rot.y);
			c.RotateRadZ(rot.z);
			c.Translate(cameraWorldPos.x, cameraWorldPos.y, cameraWorldPos.z);
			mCameraPersp.SetCoords(c);
		}

		private void UpdateTransitionTo3D()
		{
			switch (mState)
			{
			case HyperSpaceState.FadeTo3D:
				mFadeTo3D += 0.15f;
				if (mFadeTo3D >= 1f)
					mPieceAlpha.IncInVal();
				break;
			case HyperSpaceState.FadeFrom3D:
				mFadeFrom3D -= 0.1f;
				break;
			}
		}

		private void UpdateBackground()
		{
			switch (mState)
			{
			case HyperSpaceState.GemRise:
			case HyperSpaceState.GemFly:
			case HyperSpaceState.BoardShatter:
				mBGScale.IncInVal();
				break;
			}
		}

		private void Update3DBoard()
		{
			SexyVector3 rot = mAnimSeq.GetBoardRot();
			SexyVector3 pos = mAnimSeq.GetBoardPos();
			SexyVector3 scale = mAnimSeq.GetBoardScale();
			SexyCoords3 c = new SexyCoords3();
			c.RotateRadX(rot.x);
			c.RotateRadY(rot.y);
			c.RotateRadZ(rot.z);
			c.Scale(scale.x, scale.y, scale.z);
			c.Translate(pos.x, pos.y, pos.z);
			mBoardInfo.SetCoords(c);

			if (GlobalMembersResourcesWP.POPANIM_ANIMS_BOARDSHATTER != null && GlobalMembersResourcesWP.POPANIM_ANIMS_BOARDSHATTER.IsActive())
			{
                GlobalMembersResourcesWP.POPANIM_ANIMS_BOARDSHATTER.Update();
			}
		}

		private void Update3DGems()
		{
			if (mAnimSeq.GetCurFrame() >= 68)
			{
				for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
					for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
						mGemInfo[row, col].mPiece = mBoard.mBoard[row, col];
			}
			mGemRenderOrder.Clear();
			SexyCoords3 cameraCoords = mCameraPersp.GetCoords();
			int animSeqFrame = mAnimSeq.GetCurFrame();
			SexyMatrix4 matView = new SexyMatrix4();
			SexyMatrix4 matProj = new SexyMatrix4();
			SexyMatrix4 matWorld = new SexyMatrix4();
			SexyMatrix4 matWVP;
			matView = BuildTubeViewMatrix();
			mCameraPersp.GetProjectionMatrix(matProj);

			float[] mapIndexToScaleFactor = new[] { 1.14f, 1.04f, 1.17f, 1.04f, 1.1f, 1.09f, 1.1f, 1.0f };

			for (int row = 0; row < GlobalMembers.NUM_ROWS; row++)
			{
				for (int col = 0; col < GlobalMembers.NUM_COLS; col++)
				{
					GemInfo gi = mGemInfo[row, col];
					int colorIndex = GetGemColor(gi);
					if (mState == HyperSpaceState.LandOnBoard && gi.mDraw3D && mAnimSeq.GetCurFrame() >= gi.mBoardHitFrame)
					{
						gi.mPiece.mAlpha.SetConstant(1.0);
						gi.mDraw3D = false;
						gi.mPiece.ClearHyperspaceEffects();
						if (mAnimSeq.GetCurFrame() > 270 && mTicks - mGemHitTick > 2)
						{
							int[] aHitCountToSound = mBoard.GetHyperspaceTransType() == HYPERSPACETRANS.HYPERSPACETRANS_Zen ? mapHitCountToZenSound : mapHitCountToSound;
                            int landingSoundFx = aHitCountToSound[mGemHitCount % aHitCountToSound.Length];
                            int landingSoundFxPitch = mGemHitCount / aHitCountToSound.Length;
                            GlobalMembers.gApp.PlaySample(landingSoundFx, 0, 1.0, landingSoundFxPitch);
							mGemHitTick = (int)mTicks;
							mGemHitCount++;
						}
					}

					if (!gi.mDraw3D) continue;

					SexyVector3 rot = mAnimSeq.GetGemRot(row, col);
					SexyVector3 pos = mAnimSeq.GetGemPos(row, col);
					SexyVector3 scale = mAnimSeq.GetGemScale(row, col);
					SexyCoords3 gemCoords = new SexyCoords3();
					gemCoords.RotateRadX(rot.x);
					gemCoords.RotateRadY(rot.y);
					gemCoords.RotateRadZ(rot.z);
					if (colorIndex >= 0 && colorIndex < mapIndexToScaleFactor.Length)
					{
						float f = mapIndexToScaleFactor[colorIndex] * sGemMeshScale;
						scale = new SexyVector3(scale.x * f, scale.y * f, scale.z * f);
					}
					gemCoords.Scale(scale.x, scale.y, -scale.z);
					gemCoords.Translate(pos.x * sGemMeshScale, pos.y * sGemMeshScale, pos.z);
					gi.SetCoords(gemCoords);
					gi.mPos = pos;

					gemCoords.GetOutboundMatrix(matWorld);
					matWVP = MulMatrix(MulMatrix(matWorld, matView), matProj);
					gi.mPosScreen = new SexyVector3(matWVP.m[3, 0], matWVP.m[3, 1], matWVP.m[3, 2]);
					float w = matWVP.m[3, 3];
					float scaleFactor = gi.mPosScreen.z;
					if (w != 0f) gi.mPosScreen = new SexyVector3(gi.mPosScreen.x / w, gi.mPosScreen.y / w, gi.mPosScreen.z / w);
					gi.mScaleScreen = (scaleFactor < 500f) ? 3f : (3225f / scaleFactor);
					float dx = cameraCoords.t.x - gemCoords.t.x;
					float dy = cameraCoords.t.y - gemCoords.t.y;
					float dz = cameraCoords.t.z - gemCoords.t.z;
					gi.mDistToCamera = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
					mGemRenderOrder.Add(gi);
				}
			}

			mBoardInfo.mCoords.GetOutboundMatrix(matWorld);
			matWVP = MulMatrix(MulMatrix(matWorld, matView), matProj);
			mBoardScreenPos = new SexyVector3(matWVP.m[3, 0], matWVP.m[3, 1], matWVP.m[3, 2]);
			float wScreen = matWVP.m[3, 3];
			float scaleFactorScreen = mBoardScreenPos.z;
			if (mState < HyperSpaceState.LandOnBoard)
			{
				SexyMatrix4 scaleWVP = MulMatrix(MulMatrix(matWorld, matView), matProj);
				scaleFactorScreen = scaleWVP.m[3, 2];
			}
			if (wScreen != 0f)
			{
				mBoardScreenPos = new SexyVector3(
					mBoardScreenPos.x / wScreen,
					mBoardScreenPos.y / wScreen,
					mBoardScreenPos.z / wScreen);
			}

			if (mState >= HyperSpaceState.BoardShatter && mState < HyperSpaceState.LandOnBoard)
			{
				// TODO: Fix board position to match the animation
                mBoard.mOfsX = mBoardScreenPos.x * (GlobalMembers.gApp.mWidth * 0.5f);
                mBoard.mOfsY = mBoardScreenPos.y * (GlobalMembers.gApp.mHeight * -0.5f) + 165f;
            }
			if (mState < HyperSpaceState.LandOnBoard && scaleFactorScreen > 0.001f)
			{
				float boardScale = (float)(3328.328 / scaleFactorScreen);
				mBoard.mScale.SetConstant(boardScale);
			}
			if (mState == HyperSpaceState.PortalRide && mAnimSeq.GetCurFrame() >= 170)
			{
				mBoard.mShowBoard = true;
			}
			if (mState == HyperSpaceState.LandOnBoard && !mSlideBackStarted && HaveAll3DGemsLanded())
			{
				StartSlideBack();
			}

			mGemRenderOrder.Sort((a, b) =>
			{
				if (a.mDistToCamera < b.mDistToCamera) return 1;
				if (a.mDistToCamera > b.mDistToCamera) return -1;
				return 0;
			});
		}

		private void Update3DPortal()
		{
			switch (mState)
			{
			case HyperSpaceState.Init:
			case HyperSpaceState.SlideOver:
				return;
			case HyperSpaceState.GemRise:
				mUVAnimTicks += 1f;
				mMinAlpha.IncInVal();
				mRingFadeTunnelIn.IncInVal();
				mWarpTubeTextureFade.IncInVal();
				mUVAnimTicks += 1f;
				mMinAlpha.IncInVal();
				mShatterScale.IncInVal();
				mRingFadeTunnelIn.IncInVal();
				mWarpTubeTextureFade.IncInVal();
				break;
			case HyperSpaceState.GemFly:
			case HyperSpaceState.BoardShatter:
				mUVAnimTicks += 1f;
				mMinAlpha.IncInVal();
				mShatterScale.IncInVal();
				mRingFadeTunnelIn.IncInVal();
				mWarpTubeTextureFade.IncInVal();
				break;
			case HyperSpaceState.PortalRide:
			case HyperSpaceState.DebugDrawEveryOther:
				mUVAnimTicks += 1f;
				break;
			}
		}

		private void UpdateSounds()
		{
			if (mAnimSeq.GetCurFrame() == 68 || mAnimSeq.GetCurFrame() == 76)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_HYPERSPACE_SHATTER_2);
			}
		}

		private static int GetGemColor(GemInfo gi)
		{
			return gi.mPiece.mColor;
		}

		private static SexyMatrix4 MulMatrix(SexyMatrix4 a, SexyMatrix4 b)
		{
			SexyMatrix4 r = new SexyMatrix4();
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
				{
					float s = 0f;
					for (int k = 0; k < 4; k++) s += a.m[i, k] * b.m[k, j];
					r.m[i, j] = s;
				}
			return r;
		}
		
		public override void DrawBackground(Graphics g)
		{
		}

		public override void Draw(Graphics g)
		{
			GlobalMembers.gApp.mHyperTube3DListener.g = g;
			Graphics3D g3d = g.Get3D();
			if (g3d == null) return;
			g3d.ClearDepthBuffer();

			switch (mState)
			{
			case HyperSpaceState.SlideOver:
			case HyperSpaceState.FadeTo3D:
				Draw3DGems(g);
				break;
			case HyperSpaceState.GemRise:
			case HyperSpaceState.GemFly:
				Draw3DWarpTube(g);
				Draw3DGems(g);
				break;
			case HyperSpaceState.BoardShatter:
				Draw3DWarpTube(g);
				Draw2DBoardSmash(g);
				Draw3DGems(g);
				break;
			case HyperSpaceState.PortalRide:
				Draw3DWarpTube(g);
				if (mAnimSeq.GetCurFrame() < 120)
					Draw2DBoardSmash(g);
				Draw3DGems(g);
				break;
			case HyperSpaceState.LandOnBoard:
				Draw3DWarpTube(g);
				Draw3DGems(g);
				break;
			case HyperSpaceState.FadeFrom3D:
				Draw3DGems(g);
				break;
			}
		}

		private void Draw3DWarpTube(Graphics g)
		{
			if (GlobalMembers.gApp.mWarpTube3D == null) return;
			Graphics3D g3d = g.Get3D();
			RenderEffect fx = g3d.GetEffect(GlobalMembersResourcesWP.EFFECT_TUBE_3D);
			if (fx == null) return;

			g.PushState();
			try
			{
				g3d.SetBackfaceCulling(1, 0);
				g3d.SetDepthState(Graphics3D.ECompareFunc.COMPARE_LESS, true);
				g3d.SetBlend(Graphics3D.EBlendMode.BLEND_DEFAULT, Graphics3D.EBlendMode.BLEND_DEFAULT);
				g3d.SetTextureWrap(0, true);
				g3d.SetTextureLinearFilter(0, true);
				g3d.SetTextureLinearFilter(1, true);
				switch (mState)
				{
				case HyperSpaceState.GemRise:
				case HyperSpaceState.GemFly:
				case HyperSpaceState.BoardShatter:
					g3d.SetTextureWrap(1, true, false);
					break;
				default:
					g3d.SetTextureWrap(1, true);
					break;
				}
				var renderDevice = (SexyFramework.Drivers.Graphics.BaseXNARenderDevice)g3d.GetRenderDevice();

				using (RenderEffectAutoState autoFx = new RenderEffectAutoState(g, fx))
				{
					fx.SetFloat("time", mUVAnimTicks / 100f);
					float tubeFade = (float)mWarpTubeTextureFade.GetOutVal();
					float aV_x = (float)mMinAlpha.GetOutVal();
					float aV_y = tubeFade;
					float aV_z = 0.5f;
					float aV_w = 1f;
					fx.SetParameter("alphaVals", new float[] { aV_x, aV_y, aV_z, aV_w }, 4u);

					SexyMatrix4 matView = BuildTubeViewMatrix();
					g3d.SetViewTransform(matView);
					SexyMatrix4 matProj = new SexyMatrix4();
					mCameraPersp.GetProjectionMatrix(matProj);
					g3d.SetProjectionTransform(matProj);

					SexyCoords3 coords = new SexyCoords3();
					coords.Scale(1f, 1f, -1f);
					SexyMatrix4 matWorld = new SexyMatrix4();
					coords.GetOutboundMatrix(matWorld);

					renderDevice.RenderMesh(GlobalMembers.gApp.mWarpTube3D, matWorld);
				}
			}
			finally
			{
				g.PopState();
			}
            
			if (mState == HyperSpaceState.PortalRide || mState == HyperSpaceState.LandOnBoard)
			{
				Draw3DWarpTubeCap(g);
			}
		}

		private SexyMatrix4 BuildTubeViewMatrix()
		{
			SexyCoords3 cameraCoords = new SexyCoords3(mCameraPersp.GetCoords());
			cameraCoords.s = new SexyVector3(cameraCoords.s.x, cameraCoords.s.y, -cameraCoords.s.z);
			SexyMatrix4 matView = new SexyMatrix4();
			cameraCoords.GetInboundMatrix(matView);
			return matView;
		}

		private void Draw3DWarpTubeCap(Graphics g)
		{
			if (GlobalMembers.gApp.mWarpTubeCap3D == null) return;
			Graphics3D g3d = g.Get3D();
			RenderEffect fx = g3d.GetEffect(GlobalMembersResourcesWP.EFFECT_TUBECAP_3D);
			if (fx == null) return;

			g.PushState();
			try
			{
				g3d.SetBackfaceCulling(1, 0);
				g3d.SetDepthState(Graphics3D.ECompareFunc.COMPARE_LESS, false);
				g3d.SetBlend(Graphics3D.EBlendMode.BLEND_SRCCOLOR, Graphics3D.EBlendMode.BLEND_ONE);
				g3d.SetTextureWrap(0, true);
				g3d.SetTextureLinearFilter(0, true);
				var renderDevice = (SexyFramework.Drivers.Graphics.BaseXNARenderDevice)g3d.GetRenderDevice();

				using (RenderEffectAutoState autoFx = new RenderEffectAutoState(g, fx))
				{
					SexyMatrix4 matView = new SexyMatrix4();
					SexyMatrix4 matProj = new SexyMatrix4();
					matView = BuildTubeViewMatrix();
					mCameraPersp.GetProjectionMatrix(matProj);
					g3d.SetViewTransform(matView);
					g3d.SetProjectionTransform(matProj);

					SexyCoords3 c = new SexyCoords3();
					c.Scale(1f, 1f, -1f);
					SexyMatrix4 mWorld = new SexyMatrix4();
					c.GetOutboundMatrix(mWorld);

					renderDevice.RenderMesh(GlobalMembers.gApp.mWarpTubeCap3D, mWorld);
				}
			}
			finally
			{
				g.PopState();
			}
		}

		private void Draw2DBoardSmash(Graphics g)
		{
			var shatter = GlobalMembersResourcesWP.POPANIM_ANIMS_BOARDSHATTER;
			if (shatter == null) return;
			if (!shatter.mAnimRunning && !shatter.IsActive()) return;

			float baseScale = (float)mShatterScale.GetOutVal();
			float cx = mWidth / 2f;
			float cy = mHeight / 2f;

			g.PushState();
			g.Translate((int)cx, (int)cy);
			g.SetScale(baseScale, baseScale, 0f, 0f);
			shatter.Draw(g);
			g.PopState();
		}

		private void Draw3DGems(Graphics g)
		{
			Graphics3D g3d = g.Get3D();
			if (g3d == null) return;
			RenderEffect fx = g3d.GetEffect(GlobalMembersResourcesWP.EFFECT_GEM_3D);
			if (fx == null) return;

			g3d.SetBlend(Graphics3D.EBlendMode.BLEND_DEFAULT, Graphics3D.EBlendMode.BLEND_DEFAULT);

			var renderDev = (SexyFramework.Drivers.Graphics.BaseXNARenderDevice)g3d.GetRenderDevice();
			SexyMatrix4 sexyView = BuildTubeViewMatrix();
			sexyView.m[0, 0] += 0.003f;
			sexyView.m[3, 0] += 3.0f;
			sexyView.m[1, 1] += 0.003f;
			SexyMatrix4 sexyProj = new SexyMatrix4();
			mCameraPersp.GetProjectionMatrix(sexyProj);
			Microsoft.Xna.Framework.Matrix xnaView = renderDev.GetXNAMatrix(sexyView);
			Microsoft.Xna.Framework.Matrix xnaProj = renderDev.GetXNAMatrix(sexyProj);

			var stateMgr = renderDev.mStateMgr;
			using (RenderEffectAutoState autoFx = new RenderEffectAutoState(g, fx))
			{
				SexyVector3 camPos = mCameraPersp.GetCoords().t;
				camPos.z = 0f - camPos.z;
				fx.SetParameter("cameraPosition", new[] { camPos.x, camPos.y, camPos.z, 1f }, 4u);
				fx.SetParameter("ambientLightColor", ambientLightColor, 4u);
				fx.SetParameter("diffuseLightColor", diffuseLightColor, 4u);
				fx.SetParameter("specularLightColor", specularLightColor, 4u);
				fx.SetParameter("lightingCamOffest", new[] { 0f, 0f, 0f, 0f }, 4u);
				float frontFade = Math.Min(mFadeTo3D, 1f);
				fx.SetParameter("globalFade", new[] { frontFade, frontFade, frontFade, frontFade }, 4u);

				stateMgr.SetViewTransform(xnaView);
				stateMgr.SetProjectionTransform(xnaProj);
				g3d.SetDepthState(Graphics3D.ECompareFunc.COMPARE_LESS, true);
				g3d.SetBackfaceCulling(1, 0);

				foreach (GemInfo gi in mGemRenderOrder)
				{
					if (!gi.mDraw3D) continue;
					int colorIndex = GetGemColor(gi);
					if (colorIndex < 0 || colorIndex >= 7) continue;
					if (GlobalMembers.gApp.mGems3D[colorIndex] == null) continue;

					SexyVector3 lp = new SexyVector3(
						gi.mCoords.t.x + mapColorIndexToLightOffset[colorIndex].x,
						gi.mCoords.t.y + mapColorIndexToLightOffset[colorIndex].y,
						gi.mCoords.t.z + mapColorIndexToLightOffset[colorIndex].z);
					fx.SetParameter("lightPosition", new[] { lp.x, lp.y, lp.z, 1f }, 4u);
					HyperMaterial m = mapColorIndexToMaterial[colorIndex];
					fx.SetParameter("ambientMaterialColor", m.ambient, 4u);
					fx.SetParameter("diffuseMaterialColor", m.diffuse, 4u);
					fx.SetParameter("specularMaterialColor", m.specular, 4u);
					fx.SetParameter("specularPower", new[] { m.power, m.power, m.power, m.power }, 4u);

					SexyMatrix4 mWorld = new SexyMatrix4();
					gi.mCoords.GetOutboundMatrix(mWorld);
					renderDev.RenderMeshSinglePass(GlobalMembers.gApp.mGems3D[colorIndex], mWorld, 1);
				}

				Microsoft.Xna.Framework.Matrix outlineProj = xnaProj;
				outlineProj.M43 += 1.0f;
				stateMgr.SetProjectionTransform(outlineProj);
				float outlineFade = (mState == HyperSpaceState.FadeTo3D) ? (1f - (float)mPieceAlpha.GetOutVal()) : Math.Min(mFadeTo3D, 1f);
				fx.SetParameter("globalFade", new[] { outlineFade, outlineFade, outlineFade, outlineFade }, 4u);
				g3d.SetDepthState(Graphics3D.ECompareFunc.COMPARE_LESS, false);
				g3d.SetBackfaceCulling(1, 0);

				foreach (GemInfo gi in mGemRenderOrder)
				{
					if (!gi.mDraw3D) continue;
					int colorIndex = GetGemColor(gi);
					if (colorIndex < 0 || colorIndex >= 7) continue;
					if (GlobalMembers.gApp.mGems3D[colorIndex] == null) continue;

					float distScale = Math.Max(Math.Min(gi.mDistToCamera / 3425.0f, 1f), 0.5f);
					float outlineScale = 0.99f + distScale * 0.1f;
					SexyMatrix4 mWorldOutline = new SexyMatrix4();
					SexyCoords3 outlineCoords = new SexyCoords3();
					outlineCoords.CopyFrom(gi.mCoords);
					outlineCoords.Scale(outlineScale, outlineScale, outlineScale);
					outlineCoords.GetOutboundMatrix(mWorldOutline);
					renderDev.RenderMeshSinglePass(GlobalMembers.gApp.mGems3D[colorIndex], mWorldOutline, 0);
				}

				stateMgr.SetProjectionTransform(xnaProj);
				fx.SetParameter("globalFade", new[] { frontFade, frontFade, frontFade, frontFade }, 4u);
			}

			foreach (GemInfo gi in mGemRenderOrder)
			{
				if (gi.mDraw3D)
				{
					DrawBillboardEffects(g, gi);
				}
			}
		}

		private void DrawBillboardEffects(Graphics g, GemInfo gi)
		{
			if (gi.mPosScreen.z >= 1f) return;
			Piece piece = gi.mPiece;
			if (piece == null || piece.mBoundEffects.Count == 0) return;

			float screenX = mWidth * 0.5f + gi.mPosScreen.x * mWidth * 0.5f;
			float screenY = mHeight * 0.5f - gi.mPosScreen.y * mHeight * 0.5f;
			foreach (Effect effect in piece.mBoundEffects)
			{
				if (effect.mFXManager != mBoard.mPreFXManager && effect.mFXManager != mBoard.mPostFXManager)
				{
					continue;
				}
				g.PushState();
				g.Translate((int)screenX, (int)screenY);
				g.SetScale(gi.mScaleScreen, gi.mScaleScreen, 0f, 0f);
				g.Translate((int)(-effect.mX), (int)(-effect.mY));
				float prevAlpha = (float)piece.mAlpha.GetOutVal();
				piece.mAlpha.SetConstant(1.0);
				effect.Draw(g);
				piece.mAlpha.SetConstant(prevAlpha);
				g.PopState();
			}
		}
	}
}
