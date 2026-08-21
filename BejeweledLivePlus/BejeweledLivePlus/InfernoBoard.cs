using System;
using System.Collections.Generic;
using System.Globalization;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Sound;
using SexyFramework.Widget;

namespace BejeweledLivePlus
{
	public class InfernoBoard : QuestBoard
	{
		public sealed class ColCountData
		{
			public int mColComboValueDisp;

			public int mColComboStartTick;

			public int mColComboStartUpdateTick;

			public int mColComboDuration;

			public int mColComboCount;

			public CurvedVal mColComboAlpha = new CurvedVal();

			public CurvedVal mColComboScale = new CurvedVal();

			public CurvedVal mColComboY = new CurvedVal();

			public string mText = string.Empty;

			public Insets mInsets = new Insets();
		}

		public sealed class LevelData
		{
			public int mDoubleColumnCount;

			public int mMaxActiveColumnCount;

			public float mFireSpeed;

			public int mDurationTicks;

			public float mEdgeSpeedMult;

			public CurvedVal mProgressCurve = new CurvedVal();

			public LevelData()
			{
			}

			public LevelData(LevelData rhs)
			{
				mDoubleColumnCount = rhs.mDoubleColumnCount;
				mMaxActiveColumnCount = rhs.mMaxActiveColumnCount;
				mFireSpeed = rhs.mFireSpeed;
				mDurationTicks = rhs.mDurationTicks;
				mEdgeSpeedMult = rhs.mEdgeSpeedMult;
				mProgressCurve.CopyFrom(rhs.mProgressCurve);
			}
		}

		public sealed class ColData : IDisposable
		{
			public double mFreezeTime;

			public float mPreviousStrength;

			public float mStrength;

			public float mReverseVelocity;

			public float mSpeedMultiplier;

			public float mClearAmount;

			public int mLogicalColumn;

			public int mBaseColumn;

			public int mSize;

			public int mTick;

			public int mPanicOffset;

			public int mPanicRandom;

			public int mPanicTicks;

			public int mClearDelay;

			public int mAnimationDelay;

			public int mConfiguredSize;

			public bool mCracked;

			public bool mActive;

			public bool mClearing;

			public double mEdgeSpeedMultiplier;

			public double mRandomSpeedMultiplier;

			public CurvedVal mDangerY = new CurvedVal();

			public CurvedVal mDangerAlpha = new CurvedVal();

			public CurvedVal mBottomFrostPct = new CurvedVal();

			public CurvedVal mBumpY = new CurvedVal();

			public CurvedVal mReprieveRampUp = new CurvedVal();

			public CurvedVal mWarningAlpha = new CurvedVal();

			public string mIceAnimation = "idle";

			public string mPanicAnimation = string.Empty;

			public PIEffect mDangerSnowHardTop;

			public PIEffect mDangerSnowSoft;

			public PopAnim mIceAnimSingle;

			public PopAnim mIceAnimDouble;

			public PopAnim mFrostPanicAnim;

			public InfernoBoard mParent;

		public PopAnim GetIceAnim()
			{
				return mSize == 2 ? mIceAnimDouble : mIceAnimSingle;
			}

			public int GetIceAnimXOffset()
			{
				return mSize == 2 ? ConstantsWP.INFERNOBOARD_ICEANIM_OFFSET_X_2 : ConstantsWP.INFERNOBOARD_ICEANIM_OFFSET_X_1;
			}

			public void Reset(InfernoBoard parent)
			{
				mParent = parent;
				mDangerY.mAppUpdateCountSrc = parent.mCurveUpdateCount;
				mDangerAlpha.mAppUpdateCountSrc = parent.mCurveUpdateCount;
				mBottomFrostPct.mAppUpdateCountSrc = parent.mCurveUpdateCount;
				mBumpY.mAppUpdateCountSrc = parent.mCurveUpdateCount;
				mFreezeTime = 0.0;
				mPreviousStrength = 0f;
				mStrength = 0f;
				mReverseVelocity = 0f;
				mSpeedMultiplier = 0f;
				mClearAmount = 0f;
				mLogicalColumn = 0;
				mBaseColumn = 0;
				mSize = 1;
				mTick = 0;
				mPanicOffset = 0;
				mPanicRandom = 0;
				mPanicTicks = -1;
				mClearDelay = 0;
				mAnimationDelay = 0;
				mConfiguredSize = 1;
				mCracked = false;
				mActive = false;
				mClearing = false;
				mEdgeSpeedMultiplier = 1.0;
				mRandomSpeedMultiplier = 1.0;
				mIceAnimation = "idle";
				mDangerY.SetConstant(0.0);
				mDangerAlpha.SetConstant(0.0);
				mBottomFrostPct.SetConstant(0.0);
				mBumpY.SetConstant(0.0);
				mReprieveRampUp.SetConstant(1.0);
				ResetPanicAnim();
				SetColSize(1, true);
			}

			public void ResetDangerSnowEffects()
			{
				PIEffect[] effects = { mDangerSnowHardTop, mDangerSnowSoft };
				for (int effectIndex = 0; effectIndex < effects.Length; effectIndex++)
				{
					PIEffect effect = effects[effectIndex];
					if (effect == null) continue;
					effect.ResetAnim();
					effect.mEmitAfterTimeline = true;
					for (int layerIndex = 0; layerIndex < effect.mLayerVector.Count; layerIndex++)
					{
						PILayer layer = effect.mLayerVector[layerIndex];
						for (int emitterIndex = 0; emitterIndex < layer.mEmitterInstanceVector.Count; emitterIndex++)
						{
							layer.mEmitterInstanceVector[emitterIndex].mNumberScale = 0f;
						}
					}
				}
			}

			public void SetColSize(int size, bool force)
			{
				if (mSize == size && !force)
				{
					return;
				}
				mSize = size;
				if (mDangerSnowHardTop != null)
				{
					for (int i = 0; i < mDangerSnowHardTop.mLayerVector.Count; i++)
					{
						PILayer layer = mDangerSnowHardTop.mLayerVector[i];
						for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
						{
							PIEmitterInstanceDef emitterInstanceDef = layer.mEmitterInstanceVector[j].mEmitterInstanceDef;
							if (emitterInstanceDef.mPoints.Count >= 2 &&
								emitterInstanceDef.mPoints[0].mValuePoint2DVector.Count > 0 &&
								emitterInstanceDef.mPoints[1].mValuePoint2DVector.Count > 0)
							{
								int scale = size * 100;
								emitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue.X = -((scale >= 0 ? scale : scale + 1) >> 1);
								emitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue.Y = 0f;
								emitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue.X = scale / 2;
								emitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue.Y = 0f;
							}
						}
					}
				}
				if (mDangerSnowSoft != null)
				{
					for (int i = 0; i < mDangerSnowSoft.mLayerVector.Count; i++)
					{
						PILayer layer = mDangerSnowSoft.mLayerVector[i];
						for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
						{
							PIEmitterInstanceDef emitterInstanceDef = layer.mEmitterInstanceVector[j].mEmitterInstanceDef;
							if (emitterInstanceDef.mPoints.Count >= 2 &&
								emitterInstanceDef.mPoints[0].mValuePoint2DVector.Count > 0 &&
								emitterInstanceDef.mPoints[1].mValuePoint2DVector.Count > 0)
							{
								int scale = size * 100;
								emitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue.X = -((scale >= 0 ? scale : scale + 1) >> 1);
								emitterInstanceDef.mPoints[0].mValuePoint2DVector[0].mValue.Y = 0f;
								emitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue.X = scale / 2;
								emitterInstanceDef.mPoints[1].mValuePoint2DVector[0].mValue.Y = 0f;
							}
						}
					}
				}
				PopAnim anim = GetIceAnim();
				if (anim != null)
				{
					anim.Play(mIceAnimation, false);
				}
			}

			public double GetLosePct(int extraTicks)
			{
				if (mPanicTicks < 0 || mParent == null)
				{
					return 0.0;
				}
				double input = ((mPanicTicks + extraTicks) / 100.0) / mParent.mSecondsUntilLose;
				input *= mEdgeSpeedMultiplier;
				double result = mParent.mLoseFramePct.GetOutVal(input);
				return Math.Min(1.0, result);
			}

			public double GetPrevLosePct()
			{
				return mPanicTicks < 1 ? 0.0 : GetLosePct(-1);
			}

			public double GetCrushPct()
			{
				PopAnim theAnim = GetIceAnim();
				if (theAnim == null || theAnim.mMainSpriteInst == null || theAnim.mMainSpriteInst.mDef == null)
				{
					return 0.0;
				}
				string theLabel = theAnim.mMainSpriteInst.mDef.mName;
				if (theLabel != "crush" && theLabel != "crush2D")
				{
					return 0.0;
				}
				int theDuration = theAnim.mMainSpriteInst.mDef.mWorkAreaDuration - theAnim.mMainSpriteInst.mDef.mWorkAreaStart;
				if (theDuration <= 0)
				{
					return 0.0;
				}
				return Math.Min(1.0, theAnim.mMainSpriteInst.mFrameNum /
					(double)(theAnim.mMainSpriteInst.mDef.mWorkAreaDuration - theAnim.mMainSpriteInst.mDef.mWorkAreaStart));
			}

			public void ResetPanicAnim()
			{
				mRandomSpeedMultiplier = 1.0;
				if (mPanicAnimation != "blue")
				{
					mPanicAnimation = "blue";
					if (mFrostPanicAnim != null)
					{
						mFrostPanicAnim.Play(mPanicAnimation, true);
						mFrostPanicAnim.mTransform.LoadIdentity();
					}
				}
			}

			public float AddRevVel(double theVelocity, bool theUnused)
			{
				mReverseVelocity += (float)theVelocity;
				return mReverseVelocity;
			}

			public void Dispose()
			{
				mDangerSnowHardTop?.Dispose();
				mDangerSnowSoft?.Dispose();
				mIceAnimSingle?.Dispose();
				mIceAnimDouble?.Dispose();
				mFrostPanicAnim?.Dispose();
				mDangerSnowHardTop = null;
				mDangerSnowSoft = null;
				mIceAnimSingle = null;
				mIceAnimDouble = null;
				mFrostPanicAnim = null;
			}
		}

		public readonly List<ColData> mColData = new List<ColData>(8);

		public readonly List<LevelData> mLevelData = new List<LevelData>();

		public readonly List<int> mColCountBonus = new List<int>();

		public readonly List<float> mMultiplierIceReq = new List<float>();

		public readonly List<int> mPendingColumnPieceIds = new List<int>();

		public readonly ColCountData mColCountData = new ColCountData();

		public bool mGoalSurvival;

		public bool mGoalScore;

		public new int mStartDelay;

		public int mMaxActiveColCount;

		public int mDoubleColCount;

		public int mStartColCount;

		public double mDoubleEdgeMult;

		public double mEdgeSpeedMult;

		public int mRemoveBonusColumn;

		public float mFireSpeedIncrLevel;

		public float mBaseFireSpeed;

		public float mFireSpeed;

		public int mLevelDurationTicks;

		public double mFreezeMax;

		public float mFireSpeedMult;

		public float mDoubleColSpeedMult;

		public float mMatchPushStr;

		public float mSpecialGemPushMod;

		public int mColDestroyBonus;

		public double mMaxRandFireSpeedColDelta;

		public double mFreezeDurationPerNegStrength;

		public int mSecondsUntilLose;

		public int mStageNum;

		public int mStageDuration;

		public float mLastIceRemoved;

		public float mIceRemoved;

		public float mIceToRemove;

		public int mLevelProgress;

		public int mLevelProgressTotal;

		public int mStageStartAtTick;

		public int mNextTryColStart;

		public int mTotalLoseTicks;

		public int mLoseColumn;

		public int mAnimUpdateCount;

		public int mShakeCooldown;

		public int mStormyStartTick;

		public int mColComboBonusPoints;

		public int mColComboHighest;

		public int mColClearBonusPoints;

		public CurvedVal mBackDim = new CurvedVal();

		public CurvedVal mDeathAnimPct = new CurvedVal();

		public CurvedVal mLoseFramePct = new CurvedVal();

		public CurvedVal mIntroSnow = new CurvedVal();

		public CurvedVal mDarkenBoard = new CurvedVal();

		public CurvedVal mCvLevelProgress = new CurvedVal();

		public CurvedVal mColCountOverTime = new CurvedVal();

		public CurvedVal mColDistrib = new CurvedVal();

		public CurvedVal mCvRowFireSpeed = new CurvedVal();

		public CurvedVal mColComboCoolDownVsCount = new CurvedVal();

		public CurvedVal mReprieveStr = new CurvedVal();

		public CurvedVal mCvShakey = new CurvedVal();

		public CurvedVal mMultiplierTextAlpha = new CurvedVal();

		public CurvedVal mMultiplierTextScale = new CurvedVal();

		public CurvedVal mMultiplierTextX = new CurvedVal();

		public CurvedVal mMultiplierTextY = new CurvedVal();

		public CurvedVal mMultiplierFlash = new CurvedVal();

		public CurvedVal mIntroSpeedMod = new CurvedVal();

		public double mIceMeterFlashPct;

		public int mGameOverStartUpdateTick;

		public bool mReprieveActive;

		public int mReprieveStartTick;

		public int mHypermixerDelay;

		public int mComboPointId;

		public int mComboPointY;

		public int mComboPointRotation;

		public bool mStormy;

		public bool mAllowSpeedBonus;

		public SoundInstance mWindSound;

		private static CurvedVal mCvYFade = new CurvedVal();

		private static CurvedVal mCvScaleIn = new CurvedVal();

		private static CurvedVal mCvWobbleIn = new CurvedVal();

		private static CurvedVal mCvDeathStormySnow = new CurvedVal();

		private static CurvedVal mCvDeathStormySnowSoundFade = new CurvedVal();

		private static CurvedVal mCvPanicScale = new CurvedVal();

		private static CurvedVal mCvLavaShakey = new CurvedVal();

		private static CurvedVal mCvTopSnow = new CurvedVal();

		private static CurvedVal mCvStormySnow = new CurvedVal();

		private static CurvedVal mCvIceAlpha = new CurvedVal();

		private static bool mMultiplierTextCurvesLoaded;

		private static bool mDeathStormySnowCurvesLoaded;

		private static bool mLavaCurvesLoaded;

		private static bool mIceAlphaCurveLoaded;

		public MemoryImage mMultiplierTextImage;

		private uint[] mMultiplierTextSourceBits;

		private int mMultiplierTextRenderedValue = int.MinValue;

		private readonly Ref<int> mCurveUpdateCount = new Ref<int>(0);

		public InfernoBoard(bool allowSpeedBonus = false)
		{
			mAllowSpeedBonus = allowSpeedBonus;
			mShowPointMultiplier = true;
			mDoDrawGameElements = false;
			mStageNum = 0;
			mLoseColumn = -1;
			mFireSpeedMult = 1f;
			mDoubleColSpeedMult = 1f;
			mDoubleEdgeMult = 1.0;
			mEdgeSpeedMult = 1.0;
			mColDestroyBonus = 0;
			mLastIceRemoved = 0f;
			mIceRemoved = 0f;
			mBackDim.SetConstant(0.0);
			mBackDim.mAppUpdateCountSrc = mCurveUpdateCount;
			mDeathAnimPct.SetConstant(0.0);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_LOSE_FRAME_PCT, mLoseFramePct);
			mIntroSnow.SetConstant(0.0);
			mDarkenBoard.SetConstant(1.0);
			mReprieveStr.SetConstant(0.0);
			mCvShakey.SetConstant(0.0);
			mColCountOverTime.SetConstant(0.0);
			mColDistrib.SetConstant(0.0);
			mCvRowFireSpeed.SetConstant(1.0);
			mColComboCoolDownVsCount.SetConstant(100.0);
			mIntroSpeedMod.SetConstant(1.0);
			mIceMeterFlashPct = 0.0;
			mComboPointId = 10000000;
			for (int i = 0; i < 8; i++)
			{
				mColData.Add(new ColData());
			}
		}

		public override void Dispose()
		{
			if (mWindSound != null)
			{
				mWindSound.Release();
				mWindSound = null;
			}
			for (int i = 0; i < mColData.Count; i++)
			{
				mColData[i].Dispose();
			}
			mColData.Clear();
			mMultiplierTextImage?.Dispose();
			mMultiplierTextImage = null;
			mMultiplierTextSourceBits = null;
			base.Dispose();
		}

		public override void LoadContent(bool threaded)
		{
			base.LoadContent(threaded);
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlayQuest_Lightning");
				BejeweledLivePlusApp.LoadContentInBackground("GamePlayQuest_Inferno");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlayQuest_Lightning");
				BejeweledLivePlusApp.LoadContent("GamePlayQuest_Inferno");
				for (int i = 0; i < 8; i++)
				{
					ColData col = mColData[i];
					col.mDangerSnowHardTop?.Dispose();
					col.mDangerSnowHardTop = GlobalMembersResourcesWP.PIEFFECT_DANGERSNOW_HARD_TOP.Duplicate();
					col.mDangerSnowHardTop.mEmitAfterTimeline = true;
					col.mDangerSnowHardTop.mDrawTransform.LoadIdentity();
					col.mDangerSnowHardTop.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
					col.mDangerSnowHardTop.mHasDrawTransform = true;
					for (int layerIdx = 0; layerIdx < col.mDangerSnowHardTop.mLayerVector.Count; layerIdx++)
					{
						PILayer layer = col.mDangerSnowHardTop.mLayerVector[layerIdx];
						for (int emitterIdx = 0; emitterIdx < layer.mEmitterInstanceVector.Count; emitterIdx++)
						{
							layer.mEmitterInstanceVector[emitterIdx].mNumberScale = 0f;
						}
					}
					col.mDangerSnowSoft?.Dispose();
					col.mDangerSnowSoft = GlobalMembersResourcesWP.PIEFFECT_DANGERSNOW_SOFT.Duplicate();
					col.mDangerSnowSoft.mEmitAfterTimeline = true;
					col.mDangerSnowSoft.mDrawTransform.LoadIdentity();
					col.mDangerSnowSoft.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
					col.mDangerSnowSoft.mHasDrawTransform = true;
					for (int layerIdx = 0; layerIdx < col.mDangerSnowSoft.mLayerVector.Count; layerIdx++)
					{
						PILayer layer = col.mDangerSnowSoft.mLayerVector[layerIdx];
						for (int emitterIdx = 0; emitterIdx < layer.mEmitterInstanceVector.Count; emitterIdx++)
						{
							layer.mEmitterInstanceVector[emitterIdx].mNumberScale = 0f;
						}
					}
					if (col.mIceAnimSingle == null)
					{
						col.mIceAnimSingle = GlobalMembersResourcesWP.POPANIM_ANIMS_COLUMN1.Duplicate();
						col.mIceAnimSingle.mClip = true;
					}
					if (col.mIceAnimDouble == null)
					{
						col.mIceAnimDouble = GlobalMembersResourcesWP.POPANIM_ANIMS_COLUMN2.Duplicate();
						col.mIceAnimDouble.mClip = true;
					}
					if (col.mFrostPanicAnim == null)
					{
						col.mFrostPanicAnim = GlobalMembersResourcesWP.POPANIM_ANIMS_FROSTPANIC.Duplicate();
					}
					col.SetColSize(col.mSize, true);
				}
				ConfigureBarEmitters();
				LinkUpAssets();
				PopAnim iceStormUI = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMUI;
				iceStormUI.mId = 1001;
				iceStormUI.mListener = GlobalMembers.gApp;
				iceStormUI.Play("idle", true);
				PopAnim iceStormFill = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMFILL;
				float liquidX = GlobalMembers.S(
					(int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID_ID));
				float liquidY = GlobalMembers.S(
					(int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID_ID));
				float scaleX = (float)GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID.mWidth / GlobalMembers.S(436f);
				float scaleY = (float)GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID.mHeight / GlobalMembers.S(196f);
				SexyTransform2D transform = new SexyTransform2D(true);
				transform.m00 = 0f;
				transform.m01 = -scaleX;
				transform.m02 = liquidX + GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID.mWidth + GlobalMembers.S(252f) * scaleX;
				transform.m10 = scaleY;
				transform.m11 = 0f;
				transform.m12 = liquidY - GlobalMembers.S(198f) * scaleY;
				iceStormFill.SetTransform(transform);
				iceStormFill.mId = 1000;
				iceStormFill.mListener = GlobalMembers.gApp;
				iceStormFill.Play("loop", true);
			}
		}

		public override void UnloadContent()
		{
			if (mWindSound != null)
			{
				mWindSound.Release();
				mWindSound = null;
			}
			for (int i = 0; i < mColData.Count; i++)
			{
				mColData[i].Dispose();
			}
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_Inferno");
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_Lightning");
			base.UnloadContent();
		}

		public override void Init()
		{
			if (mIsPerpetual)
			{
				mUiConfig = EUIConfig.eUIConfig_StandardNoReplay;
			}
			base.Init();
			mPendingColumnPieceIds.Clear();
			mIntroSpeedMod.SetConstant(1.0);
			mDarkenBoard.SetConstant(1.0);
			mIntroSnow.SetConstant(0.0);
			mDeathAnimPct.SetConstant(0.0);
			mColCountData.mColComboCount = 0;
			mLoseColumn = -1;
			mAnimUpdateCount = 0;
			mColComboBonusPoints = 0;
			mColComboHighest = 0;
			mColClearBonusPoints = 0;
			mReprieveActive = false;
			mStartDelay = 150;
			mDoubleEdgeMult = 1.0;
			mFireSpeedMult = 1f;
			mMatchPushStr = 0.125f;
			mSpecialGemPushMod = 2f;
			mColDestroyBonus = 0;
			mStormyStartTick = -1;
			mStormy = false;
			mReprieveStartTick = 0;
			mShakeCooldown = 0;
			mGameOverStartUpdateTick = 0;
			ClearComboPoints();
			mComboPointId = 10000000;
			mComboPointRotation = 0;
			mBackDim.SetConstant(0.0);
			mMultiplierFlash.SetConstant(0.0);
			mNextTryColStart = mGameTicks + 25;
			mTotalLoseTicks = 500;
			mIceMeterFlashPct = 0.0;
			mSecondsUntilLose = 10;
			mDoubleColSpeedMult = 1f;
			mStartColCount = -1;
			mRemoveBonusColumn = 0;
			mGoalSurvival = false;
			mGoalScore = false;
			mLevelProgress = 0;
			mFireSpeedIncrLevel = 0f;
			mLevelProgressTotal = 0;
			mMaxRandFireSpeedColDelta = 0.0;
			mIceRemoved = 0f;
			mIceToRemove = 0f;
			mLastIceRemoved = 0f;
			mStageStartAtTick = mGameTicks;
			mHypermixerDelay = 0;
			mAllowLevelUp = !mIsPerpetual;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_COL_COUNT_OVER_TIME, mColCountOverTime);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_COL_DISTRIB, mColDistrib);
			mCvLevelProgress.SetConstant(1.0);
			mMaxActiveColCount = -1;
			mDoubleColCount = 0;
			mStageNum = 0;
			mMultiplierTextAlpha.SetConstant(0.0);
			mMultiplierTextY.SetConstant(0.0);
			mMultiplierTextX.SetConstant(0.0);
			mMultiplierTextScale.SetConstant(0.0);
			mCvShakey.SetConstant(0.0);
			mReprieveStr.SetConstant(0.0);
			GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.ResetAnim();
			GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mEmitAfterTimeline = true;
			GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mDrawTransform.LoadIdentity();
			GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
			GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mHasDrawTransform = true;
			for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector.Count; i++)
			{
				PILayer layer = GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector[i];
				for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
				{
					layer.mEmitterInstanceVector[j].mNumberScale = 0f;
				}
			}
			GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.ResetAnim();
			GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mEmitAfterTimeline = true;
			GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mDrawTransform.LoadIdentity();
			GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
			GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mHasDrawTransform = true;
			float numberScale = SexyFramework.GlobalMembers.gIs3D ? 1f : 0.5f;
			for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector.Count; i++)
			{
				PILayer layer = GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector[i];
				for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
				{
					layer.mEmitterInstanceVector[j].mNumberScale = numberScale;
				}
			}
		}

		public override void InitUI()
		{
			CallBoardInitUI();
		}

		private void SetIceStormUITransform(PopAnim iceStormUI, bool alignSteamToColumns)
		{
			iceStormUI.mTransform.LoadIdentity();
			if (alignSteamToColumns)
			{
				const float firstSteamCenterX = 624f;
				const float lastSteamCenterX = 1518.5f;
				const float jetCenterY = 3f;
				float firstColumnCenterX = (GlobalMembers.S(GetColScreenX(0)) +
					GlobalMembers.S(GetColScreenX(1))) * 0.5f;
				float lastColumnCenterX = (GlobalMembers.S(GetColScreenX(7)) +
					GlobalMembers.S(GetColScreenX(8))) * 0.5f;
				float combinedScale = (lastColumnCenterX - firstColumnCenterX) /
					(lastSteamCenterX - firstSteamCenterX);
				float drawScale = iceStormUI.mDrawScale == 0f ? 1f : iceStormUI.mDrawScale;
				float rootScale = combinedScale / drawScale;
				iceStormUI.mTransform.m00 = rootScale;
				iceStormUI.mTransform.m11 = rootScale;
				iceStormUI.mTransform.m02 =
					firstColumnCenterX - firstSteamCenterX * combinedScale;
				iceStormUI.mTransform.m12 =
					GlobalMembers.S(GetBoardY()) - jetCenterY * combinedScale - GlobalMembers.S(20f);
			}
			iceStormUI.mTransDirty = true;
		}

		private void DrawIceStormUIPam(Graphics g)
		{
			PopAnim iceStormUI = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMUI;
			g.PushState();
			float alpha = GetAlpha();
			g.SetColor(Color.FAlpha(alpha));
			g.SetColorizeImages(alpha < 1f);
			bool steamAlignedToColumns = iceStormUI.mMainSpriteInst?.mDef?.mName == "multiplierup";
			if (!steamAlignedToColumns)
			{
				g.Translate(GlobalMembers.S(-40), GlobalMembers.S(80));
			}
			EnableDarkenColor(g, 200);
			iceStormUI.Draw(g);
			DisableDarkenColor(g);
			g.PopState();
		}

		public override void NewGame(bool restartingGame)
		{
			PopAnim iceStormUI = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMUI;
			SetIceStormUITransform(iceStormUI, false);
			iceStormUI.Play("idle", true);
			string goal = mParams.ContainsKey("Goal") ? mParams["Goal"].ToUpperInvariant() : string.Empty;
			mGoalSurvival = goal == "SURVIVAL";
			mGoalScore = goal == "S";
			mLevelData.Clear();
			for (int level = 1; ; level++)
			{
				string levelKey = $"Level{level}";
				if (!mParams.ContainsKey(levelKey))
				{
					break;
				}
				List<string> levelValues = new List<string>();
				Utils.SplitAndConvertStr(mParams[levelKey], levelValues, ',', true, 6);
				if (levelValues.Count < 5)
				{
					continue;
				}
				LevelData levelData = new LevelData
				{
					mDoubleColumnCount = SexyFramework.GlobalMembers.sexyatoi(levelValues[0]),
					mMaxActiveColumnCount = SexyFramework.GlobalMembers.sexyatoi(levelValues[1]),
					mFireSpeed = float.Parse(levelValues[2], CultureInfo.InvariantCulture),
					mDurationTicks = (int)(float.Parse(levelValues[3], CultureInfo.InvariantCulture) * 100f),
					mEdgeSpeedMult = float.Parse(levelValues[4], CultureInfo.InvariantCulture)
				};
				if (levelValues.Count == 6)
				{
					levelData.mProgressCurve.SetCurve(levelValues[5]);
				}
				else if (mLevelData.Count != 0)
				{
					levelData.mProgressCurve.CopyFrom(mLevelData[mLevelData.Count - 1].mProgressCurve);
				}
				mLevelData.Add(levelData);
			}
			mMaxActiveColCount = SexyFramework.GlobalMembers.sexyatoi(mParams, "MaxActiveColCount");
			mDoubleColCount = SexyFramework.GlobalMembers.sexyatoi(mParams, "DoubleColCount");
			mStartColCount = SexyFramework.GlobalMembers.sexyatoi(mParams, "StartColCount");
			mDoubleEdgeMult = SexyFramework.GlobalMembers.sexyatof(mParams, "DoubleEdgeMult");
			mRemoveBonusColumn = SexyFramework.GlobalMembers.sexyatoi(mParams, "RemoveBonusColumn");
			mFireSpeedIncrLevel = SexyFramework.GlobalMembers.sexyatof(mParams, "FireSpeedIncrLevel");
			mBaseFireSpeed = SexyFramework.GlobalMembers.sexyatof(mParams, "FireSpeed");
			mLevelDurationTicks = (int)(SexyFramework.GlobalMembers.sexyatof(mParams, "LevelDurationSec") * 100f);
			mFreezeMax = SexyFramework.GlobalMembers.sexyatof(mParams, "FreezeMax");
			mFireSpeedMult = SexyFramework.GlobalMembers.sexyatof(mParams, "FireSpeedMult");
			mMatchPushStr = SexyFramework.GlobalMembers.sexyatof(mParams, "MatchPushStr");
			mSpecialGemPushMod = SexyFramework.GlobalMembers.sexyatof(mParams, "SpecialGemPushMod");
			mColDestroyBonus = (int)(SexyFramework.GlobalMembers.sexyatoi(mParams, "ColDestroyBonus") / GetModePointMultiplier());
			mDoubleColSpeedMult = 1f;
			if (mParams.ContainsKey("DoubleColSpeedMult"))
			{
				mDoubleColSpeedMult = SexyFramework.GlobalMembers.sexyatof(mParams, "DoubleColSpeedMult");
			}
			mColCountBonus.Clear();
			if (mParams.ContainsKey("ColCountBonus"))
			{
				Utils.SplitAndConvertStr(mParams["ColCountBonus"], mColCountBonus, ',', false, -1);
			}
			mMultiplierIceReq.Clear();
			if (mParams.ContainsKey("MultiplierIceReq"))
			{
				Utils.SplitAndConvertStr(mParams["MultiplierIceReq"], mMultiplierIceReq, ',', false, -1);
				for (int i = 0; i < mMultiplierIceReq.Count; i++)
				{
					mMultiplierIceReq[i] *= 8f;
				}
			}
			mMaxRandFireSpeedColDelta = SexyFramework.GlobalMembers.sexyatof(mParams, "MaxRandFireSpeedColDelta");
			mFreezeDurationPerNegStrength = SexyFramework.GlobalMembers.sexyatof(mParams, "FreezeDurationPerNegStrength");
			mSecondsUntilLose = SexyFramework.GlobalMembers.sexyatoi(mParams, "SecondsUntilLose");
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_COL_COMBO_COOL_DOWN_VS_COUNT, mColComboCoolDownVsCount);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_CV_ROW_FIRE_SPEED, mCvRowFireSpeed);
			if (mParams.ContainsKey("LevelProgress"))
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_CV_LEVEL_PROGRESS, mCvLevelProgress);
			}
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_REPRIEVE_STR, mReprieveStr);
			if (mIsPerpetual)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_INTRO_SNOW,
					mIntroSnow);
				float numberScale = (float)mIntroSnow.GetOutVal() * 0.2f *
					(SexyFramework.GlobalMembers.gIs3D ? 1f : 0.5f);
				for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector.Count; i++)
				{
					PILayer layer = GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector[i];
					for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
					{
						layer.mEmitterInstanceVector[j].mNumberScale = numberScale;
					}
				}
				for (int i = 0; i <= 499; i++)
				{
					GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.Update();
				}
				RefreshIceToRemove();
			}
			InitLavaCols();
			SyncToLevel(mStageNum);
			base.NewGame(restartingGame);
			mFireSpeed = mBaseFireSpeed;
			Bej3Widget.SetOverlayType(OVERLAY_TYPE.OVERLAY_ICE);
		}

		public void SyncLevelDataToLevel(int level)
		{
			if (mLevelData.Count == 0)
			{
				if (level > 0)
				{
					mBaseFireSpeed += mFireSpeedIncrLevel * mFireSpeed * mFireSpeedMult;
				}
				return;
			}
			if (level < mLevelData.Count)
			{
				LevelData data = mLevelData[Math.Max(0, level)];
				mMaxActiveColCount = data.mMaxActiveColumnCount;
				mDoubleColCount = data.mDoubleColumnCount;
				mStageDuration = data.mDurationTicks;
				mBaseFireSpeed = data.mFireSpeed * mFireSpeedMult;
				mEdgeSpeedMult = data.mEdgeSpeedMult;
				mCvLevelProgress.CopyFrom(data.mProgressCurve);
				return;
			}
			int extra = level - mLevelData.Count + 1;
			LevelData last = mLevelData[mLevelData.Count - 1];
			LevelData previous = mLevelData[Math.Max(0, mLevelData.Count - 2)];
			mMaxActiveColCount = last.mMaxActiveColumnCount;
			mDoubleColCount = last.mDoubleColumnCount;
			mStageDuration = last.mDurationTicks + (last.mDurationTicks - previous.mDurationTicks) * extra;
			mBaseFireSpeed = (last.mFireSpeed + (last.mFireSpeed - previous.mFireSpeed) * extra) * mFireSpeedMult;
			mEdgeSpeedMult = last.mEdgeSpeedMult + (last.mEdgeSpeedMult - previous.mEdgeSpeedMult) * extra;
		}

		public void SyncToLevel(int level)
		{
			SyncLevelDataToLevel(level);
			mStageStartAtTick = mGameTicks;
			bool hasDouble = mMaxActiveColCount > 0 && mDoubleColCount > 0;
			for (int i = 0; i < 8; i++)
			{
				bool useDouble = mDoubleColCount > 0 &&
					(mMaxActiveColCount - mDoubleColCount <= 0 || hasDouble && (i < 2 || i > 5));
				if (useDouble)
				{
					mColData[i].mConfiguredSize = 2;
					for (int j = 0; j < mColData[i].mSize && i + j < 8; j++)
					{
						mColData[i + j].mConfiguredSize = 2;
						mColData[i + j].mBaseColumn = 2 * (i / 2);
					}
				}
				else if (mDoubleColCount == 0 || hasDouble)
				{
					mColData[i].mConfiguredSize = 1;
					for (int j = 0; j < mColData[i].mSize && i + j < 8; j++)
					{
						mColData[i + j].mConfiguredSize = 1;
						mColData[i + j].mBaseColumn = i;
					}
				}
			}
			if (mStartColCount >= 0)
			{
				AddStageUpExtraColumns(mStartColCount);
			}
		}

		public void SyncProgress()
		{
			if (mIsPerpetual)
			{
				float delta = mIceRemoved - mLastIceRemoved;
				if (mGameTicks - mStageStartAtTick >= mStageDuration)
				{
					mStageNum++;
					SyncToLevel(mStageNum);
				}
				mIceMeterFlashPct = Math.Min(1.5, mIceMeterFlashPct + delta * 0.1f);
				mLastIceRemoved = mIceRemoved;
				mLevelProgress = (int)(mIceRemoved * 10000f);
				mLevelProgressTotal = (int)(mIceToRemove * 10000f);
			}
			else
			{
				mMaxActiveColCount = (int)mColCountOverTime.GetInVal();
			}
		}

		public float RefreshIceToRemove()
		{
			if (mMultiplierIceReq.Count == 0)
			{
				mIceToRemove = 10f;
			}
			else
			{
				int index = Math.Min(mMultiplierIceReq.Count - 1, Math.Max(0, mPointMultiplier - 1));
				mIceToRemove = mMultiplierIceReq[index];
			}
			return mIceToRemove;
		}

		public void RefreshMultiplierText()
		{
			if (!mReprieveActive || GlobalMembers.gApp.mGraphicsDriver == null)
			{
				return;
			}
			if (mMultiplierTextImage == null)
			{
				mMultiplierTextImage = new MemoryImage();
				mMultiplierTextImage.Create(GlobalMembers.S(850), GlobalMembers.S(70));
				mMultiplierTextImage.mIsVolatile = true;
				mMultiplierTextImage.SetImageMode(true, true);
			}
			if (mMultiplierTextRenderedValue != mPointMultiplier)
			{
				mMultiplierTextRenderedValue = mPointMultiplier;
				mMultiplierTextSourceBits = null;
			}
			if (mMultiplierTextSourceBits == null)
			{
				mMultiplierTextImage.PurgeBits();
				mMultiplierTextImage.mBits = null;
				Graphics graphics = new Graphics(mMultiplierTextImage);
				graphics.ClearRect(0, 0, mMultiplierTextImage.mWidth, mMultiplierTextImage.mHeight);
				graphics.SetFont(GlobalMembersResources.FONT_SUBHEADER);
				graphics.SetColor(Color.White);
			ImageFont font = (ImageFont)GlobalMembersResources.FONT_SUBHEADER;
			font.PushLayerColor(0, Color.Black);
			font.PushLayerColor(1, Color.White);
				string text = string.Format(GlobalMembers._ID("MULTIPLIER {0}x", 561), mPointMultiplier);
				graphics.WriteString(text, mMultiplierTextImage.mWidth / 2,
					(int)(60f * GlobalMembers.gApp.mWidth / 1200f), -1, 0);
			font.PopLayerColor(1);
			font.PopLayerColor(0);
				graphics.flush();
				graphics.Dispose();
				uint[] renderedBits = mMultiplierTextImage.GetBits();
				mMultiplierTextSourceBits = new uint[renderedBits.Length];
				Array.Copy(renderedBits, mMultiplierTextSourceBits, renderedBits.Length);
			}
			MemoryImage textCycleImage = GlobalMembersResourcesWP.IMAGE_INFERNO_TEXT_CYCLE.AsMemoryImage();
			if (textCycleImage == null || textCycleImage.mHeight <= 0)
			{
				return;
			}
			uint[] textBits = mMultiplierTextImage.GetBits();
			uint[] textCycleBits = textCycleImage.GetBits();
			int w = mMultiplierTextImage.mWidth;
			int h = mMultiplierTextImage.mHeight;
			int textCycleOffset = mAnimUpdateCount * 2;
			for (int y = 0; y < h; y++)
			{
				uint textCycleColor = textCycleBits[(textCycleOffset + y) % textCycleImage.mHeight];
				for (int x = 0; x < w; x++)
				{
					int index = y * w + x;
					uint src = mMultiplierTextSourceBits[index];
					uint alpha = src & 0xff000000;
					uint coverage = src & 0xff;
					textBits[index] = alpha |
						(((textCycleColor & 0xff0000) * coverage >> 8) & 0xff0000) |
						(((textCycleColor & 0xff00) * coverage >> 8) & 0xff00) |
						((textCycleColor & 0xff) * coverage >> 8);
				}
			}
			mMultiplierTextImage.BitsChanged();
		}

		public void InitLavaCols()
		{
			for (int i = 0; i < 8; i++)
			{
				mColData[i].Reset(this);
				mColData[i].mLogicalColumn = i;
			}
		}

		public void GetActiveColCounts(out int singleCount, out int doubleCount)
		{
			singleCount = 0;
			doubleCount = 0;
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				if (col.mLogicalColumn == i && col.mActive && !col.mClearing)
				{
					if (col.mSize == 2) doubleCount++; else singleCount++;
				}
			}
		}

		public void GetColAllocations(List<int> inactiveSingles, List<int> activeColumns)
		{
			inactiveSingles.Clear();
			activeColumns.Clear();
			for (int i = 0; i < 8; i++)
			{
				ColData root = mColData[i];
				if (root.mBaseColumn != i)
				{
					continue;
				}
				if (root.mActive)
				{
					activeColumns.Add(i);
					continue;
				}
				bool childActive = false;
				for (int j = 1; j < root.mConfiguredSize && i + j < 8; j++)
				{
					childActive |= mColData[i + j].mActive;
				}
				if (!childActive) inactiveSingles.Add(i);
			}
		}

		public bool TryStartNewCol()
		{
			List<int> candidates = new List<int>();
			List<int> active = new List<int>();
			GetColAllocations(candidates, active);
			GetActiveColCounts(out int singles, out int doubles);
			if (singles + doubles >= mMaxActiveColCount || candidates.Count == 0)
			{
				return false;
			}
			if (candidates.Count > 1)
			{
				int last = candidates[candidates.Count - 1];
				candidates.RemoveAt(candidates.Count - 1);
				for (int i = candidates.Count - 1; i > 0; i--)
				{
					int j = (int)(mRand.Next() % (uint)(i + 1));
					int value = candidates[i];
					candidates[i] = candidates[j];
					candidates[j] = value;
				}
				candidates.Add(last);
			}
			int edgeActive = (mColData[0].mActive ? 1 : 0) + (mColData[7].mActive ? 1 : 0);
			for (int n = 0; n < candidates.Count; n++)
			{
				int col = candidates[n];
				ColData root = mColData[col];
				bool blocked = false;
				for (int j = 0; j < mColData[n].mConfiguredSize && col + j < 8; j++)
				{
					blocked |= mColData[col + j].mActive;
				}
				if (blocked)
				{
					continue;
				}
				bool edgeCandidate = col == 0 || col == (root.mConfiguredSize == 1 ? 7 : 6);
				if (edgeCandidate && candidates.Count >= 2)
				{
					if (mDoubleColCount < 3)
					{
						if (mDoubleColCount < 1)
						{
							if ((edgeActive == 0 && active.Count <= 2) || (edgeActive == 1 && active.Count <= 4))
							{
								continue;
							}
						}
						else if ((edgeActive == 0 && active.Count <= 1) || (edgeActive == 1 && active.Count <= 3))
						{
							continue;
						}
					}
					else if ((edgeActive == 0 && active.Count == 0) || (edgeActive == 1 && active.Count <= 1))
					{
						continue;
					}
				}
				bool stormBlocked = false;
				for (int j = 0; j < root.mSize && col + j < 8; j++)
				{
					stormBlocked |= IsColInStorm(col + j);
				}
				if (!stormBlocked)
				{
					ToggleColActive(col, root.mConfiguredSize, true);
					ResetNextColStartTime();
					return true;
				}
			}
			return false;
		}

		public void AddStageUpExtraColumns(int count)
		{
			while (count-- > 0)
			{
				if (!TryStartNewCol())
				{
					break;
				}
			}
		}

		public void ToggleColActive(int column, int size, bool active)
		{
			ColData root = mColData[column];
			if (root.mActive == active) return;
			root.mClearing = false;
			root.mCracked = false;
			root.mPreviousStrength = 0f;
			root.mStrength = 0f;
			root.mActive = active;
			root.mEdgeSpeedMultiplier = mEdgeSpeedMult;
			for (int j = 0; j < root.mSize; j++)
			{
				ColData child = mColData[column + j];
				child.mActive = active;
				child.mLogicalColumn = child.mBaseColumn;
			}
			root.SetColSize(size, false);
			for (int j = 0; j < root.mSize; j++)
			{
				ColData child = mColData[column + j];
				child.mActive = active;
				child.mLogicalColumn = active ? column : child.mBaseColumn;
			}
			for (int j = 0; j < root.mSize; j++)
			{
				mColData[column + j].mActive = active;
			}
			root.mLogicalColumn = root.mSize == 2 ? column - (column % 2) : column;
			if (active)
			{
				root.mTick = 50;
				root.mSpeedMultiplier = GlobalMembersUtils.GetRandFloatU() * (float)mMaxRandFireSpeedColDelta;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
					PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_BOTTOM_FROST_PCT_ACTIVATE,
					root.mBottomFrostPct);
			}
			else
			{
				root.mTick = 0;
			}
		}

		public void ResetNextColStartTime()
		{
			GetActiveColCounts(out int singles, out int doubles);
			int next = mGameTicks + 75;
			int active = singles + doubles;
			if (mMaxActiveColCount > 2 && active > 2)
			{
				next += 75 * (active - 2);
			}
			mNextTryColStart = Math.Min(mNextTryColStart, next);
		}

		public bool IsColInStorm(int column)
		{
			for (int i = 0; i < mLightningStorms.Count; i++)
			{
				LightningStorm storm = mLightningStorms[i];
				if (storm == null) continue;
				if (storm.mStormType == (int)LightningStorm.STORM.STORM_HYPERCUBE && storm.mColor == -1)
				{
					return true;
				}
				if (storm.mStormType == (int)LightningStorm.STORM.STORM_BOTH && storm.mOriginCol == column)
				{
					return true;
				}
			}
			return false;
		}

		public double CalcReverseVelocity(ColData column)
		{
			double threshold = 8.0 - mReprieveStr.mInMax;
			if (column.mStrength <= threshold) return 0.0;
			double curveValue = mReprieveStr.GetOutVal(column.mStrength - threshold);
			return Math.Max(column.mReverseVelocity, curveValue);
		}

		public float DoBoardShake(bool intense)
		{
			double theDamp = 1.0;
			if (mShakeCooldown > 0)
			{
				theDamp = 0.5 * Math.Max(0.0, Math.Min(1.0, (200.0 - mShakeCooldown) / 200.0)) + 0.5;
			}
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				intense
					? PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_CV_SHAKEY_INTENSE
					: PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_CV_SHAKEY,
				mCvShakey);
			mShakeCooldown = intense ? 300 : 200;
			if (intense)
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ICE_STORM_FINAL_THUD);
			}
			else
			{
				SoundInstance theSound = GlobalMembers.gApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_TOWER_HITS_TOP1);
				if (theSound != null)
				{
					theSound.SetVolume(theSound.GetVolume() * theDamp);
					theSound.Play(false, true);
				}
			}
			mCvShakey.mOutMax *= theDamp;
			return (float)mCvShakey.mOutMax;
		}

		public void SetColumnStr(ColData theColData, double theStrength)
		{
			theColData.mStrength = (float)theStrength;
		}

		public bool TryClearCol(int theColumn, int theRow)
		{
			ColData theColData = mColData[theColumn];
			if (!theColData.mActive || theColData.mClearing || theColData.mClearAmount != 0f)
			{
				return false;
			}
			GetActiveColCounts(out _, out _);
			theColData.mClearing = true;
			theColData.mClearAmount = Math.Max(1f, theColData.mStrength);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ICE_COLUMN_BREAK);
			IncrementColCleared(theColumn, theRow);
			AddToStat((int)STAT.STAT_ICESTORM_COLUMNS_SMASHED, 1, -1, true);
			return true;
		}

		public void IncrementColCleared(int column, int row)
		{
			mColClearBonusPoints += mColDestroyBonus * mPointMultiplier;
			if (mColCountBonus.Count == 0)
			{
				return;
			}
			if (!IsColComboActive()) mColCountData.mColComboCount = 0;
			mColCountData.mColComboCount++;
			mColComboHighest = Math.Max(mColComboHighest, mColCountData.mColComboCount);
			if (mColCountBonus.Count != 0)
			{
				int index = Math.Min(mColCountBonus.Count - 1, Math.Max(0, mColCountData.mColComboCount - 1));
				mColCountData.mColComboValueDisp = mColCountBonus[index];
			}
			int comboIndex = Math.Min(mColCountBonus.Count - 1, Math.Max(0, mColCountData.mColComboCount - 1));
			if (mColCountBonus.Count > 0 && comboIndex >= 2)
			{
				GlobalMembers.gApp.PlaySample(
					comboIndex < 9 ? GlobalMembersResourcesWP.SOUND_ICE_STORM_COLUMNCOMBO : GlobalMembersResourcesWP.SOUND_ICE_STORM_COLUMNCOMBO_MEGA,
					0, 1.0, comboIndex - 2);
			}
			if (comboIndex >= 2 && mIsPerpetual && IsColComboActive() && mColCountData.mColComboValueDisp > 0)
			{
				for (int i = 0; i < 2; i++)
				{
					Points point = mPointsManager?.Find((uint)mComboPointId);
					if (point != null)
					{
						point.mState = (int)Points.POINTSSTATE.STATE_FADING;
						mComboPointId++;
					}
				}
				mComboPointRotation = (int)mRand.Next();
				int pointX = GetBoardX() + Math.Min(500, 100 * column + (int)(mRand.Next() % 100)) + 100;
				mComboPointY = GetBoardY() + 100 * Math.Min(4, row) + (int)(mRand.Next() % 100);
				for (int i = 0; i < 2; i++)
				{
					Points point = mPointsManager?.Add(pointX, mComboPointY, 0, Color.White,
						(uint)(mComboPointId + i), true, -1, true);
					if (point == null) continue;
					if (i == 0)
					{
						string comboText;
						switch (mColCountData.mColComboCount)
						{
							case 3:
								comboText = string.Empty;
								break;
							case 4:
								comboText = GlobalMembers._ID("COOL ", 549);
								break;
							case 5:
								comboText = GlobalMembers._ID("CHILL ", 550);
								break;
							case 6:
								comboText = GlobalMembers._ID("FROSTY ", 551);
								break;
							case 7:
								comboText = GlobalMembers._ID("ICY ", 552);
								break;
							case 8:
								comboText = GlobalMembers._ID("GLACIAL ", 553);
								break;
							case 9:
								comboText = GlobalMembers._ID("POLAR ", 554);
								break;
							case 10:
								comboText = GlobalMembers._ID("ARCTIC ", 555);
								break;
							case 11:
								comboText = GlobalMembers._ID("SUB ZERO ", 556);
								break;
							case 12:
								comboText = GlobalMembers._ID("ICE ICE ", 557);
								break;
							default:
								comboText = GlobalMembers._ID("max ", 558);
								break;
						}
						point.mString = string.Format(GlobalMembers._ID("{0}COMBO", 559), comboText);
					}
					else
					{
						point.mString = string.Format(GlobalMembers._ID("{0}x", 560), mColCountData.mColComboCount);
					}
					point.mState = 0;
					point.mTimer = 20f;
					point.mScale = 0.01f;
					point.mDestScale = 0.01f;
					point.mLimitY = !SexyFramework.GlobalMembers.gIs3D;
					ConfigureColComboPoints(point, true);
				}
				Points comboPoint = mPointsManager?.Find((uint)mComboPointId);
				Points comboCountPoint = mPointsManager?.Find((uint)(mComboPointId + 1));
				if (comboPoint != null && comboCountPoint != null)
				{
					float halfWidth = Math.Max(comboPoint.mFont.StringWidth(comboPoint.mString),
						comboCountPoint.mFont.StringWidth(comboCountPoint.mString)) * 0.75f;
					float alignedX = Math.Max(ConstantsWP.POINTS_LIMIT + halfWidth,
						Math.Min(GlobalMembers.gApp.mWidth - ConstantsWP.POINTS_LIMIT - halfWidth,
							GlobalMembers.S(pointX)));
					comboPoint.mX = GlobalMembers.RS(alignedX);
					comboCountPoint.mX = comboPoint.mX;
				}
			}
			mColCountData.mColComboStartTick = mGameTicks;
			mColCountData.mColComboStartUpdateTick = mUpdateCnt;
			mColCountData.mColComboDuration = GetComboCooldown(mColCountData.mColComboCount);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_COL_COMBO_SCALE, mColCountData.mColComboScale);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_COL_COMBO_ALPHA_CLEARING, mColCountData.mColComboAlpha);
			mColCountData.mColComboAlpha.Intercept(null, 0.01, false);
			mColCountData.mColComboY.SetConstant(0.0);
			mPoints += mColCountData.mColComboValueDisp;
			mColComboBonusPoints += mColCountData.mColComboValueDisp;
			if (mPointsBreakdown.Count > 0)
			{
				mPointsBreakdown[mPointsBreakdown.Count - 1][4] += mColCountData.mColComboValueDisp;
			}
		}

		public void ClearComboPoints()
		{
			if (mPointsManager != null)
			{
				for (int i = 0; i < 2; i++)
				{
					Points point = mPointsManager.Find((uint)(mComboPointId + i));
					if (point != null)
					{
						point.mAlpha = 0f;
						point.mDeleteMe = true;
					}
				}
			}
			mComboPointId += 2;
		}

		public bool IsColComboActive()
		{
			return mColCountData.mColComboCount > 0 && mColCountData.mColComboStartTick + mColCountData.mColComboDuration >= mGameTicks;
		}

		public double GetColComboPct()
		{
			if (mColCountBonus.Count <= 3) return 0.0;
			return Math.Min(1.0, Math.Max(0.0, (mColCountData.mColComboCount - 3.0) / (mColCountBonus.Count - 3.0)));
		}

		public int GetComboCooldown(int comboCount)
		{
			float capped = Math.Min(comboCount, (float)mColComboCoolDownVsCount.mInMax);
			return (int)mColComboCoolDownVsCount.GetOutVal(capped);
		}

		public void ConfigureColComboPoints(Points thePoints, bool theClearing)
		{
			if (thePoints == null)
			{
				return;
			}
			for (int i = 0; i <= 4 && i < thePoints.mColorCycle.Length; i++)
			{
				thePoints.mColorCycle[i].ClearColors();
			}
			thePoints.mColorCycling = true;
			if (!theClearing && !IsColComboActive())
			{
				thePoints.mColorCycle[0].PushColor(new Color(12111336));
				thePoints.mColorCycle[1].PushColor(new Color(7786999));
				thePoints.mColorCycle[2].PushColor(new Color(1573119));
				thePoints.mColorCycle[0].PushColor(new Color(12111336));
				thePoints.mColorCycle[1].PushColor(new Color(12111336));
				thePoints.mColorCycle[2].PushColor(new Color(1573119));
				return;
			}
			Color[] colors =
			{
				new Color(16766833, 255),
				new Color(16777015, 255),
				new Color(11417088, 255)
			};
			if (mColCountData.mColComboCount > 9)
			{
				colors[0] = Color.White;
				colors[1] = Color.White;
				colors[2] = new Color(255, 255);
			}
			else if (mColCountData.mColComboCount > 6)
			{
				colors[0] = new Color(16766833, 255);
				colors[1] = new Color(16777015, 255);
				colors[2] = new Color(11417088, 255);
			}
			CurvedVal theColorLerp = new CurvedVal();
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
				PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_CONFIGURE_COL_COMBO_POINTS_LERP_PCT,
				theColorLerp);
			for (int i = 0; i < 3; i++)
			{
				thePoints.mColorCycle[i].SetSpeed(2f);
				thePoints.mColorCycle[i].PushColor(colors[i]);
				if (theClearing && i == 1)
				{
					thePoints.mColorCycle[i].PushColor(Utils.ColorLerp(Color.Black, colors[2],
						(float)theColorLerp.GetOutVal((float)GetColComboPct())));
				}
				else if (theClearing)
				{
					thePoints.mColorCycle[i].PushColor(colors[(i + 1) % 3]);
				}
				else
				{
					thePoints.mColorCycle[i].PushColor(colors[i]);
				}
			}
		}

		public void DoInfernoPoints(string theText, int thePoints, float theX, int theY, int theMoveCreditId, float theScale)
		{
			Points thePoint = AddPoints((int)theX, theY, thePoints, new Color(16755268), uint.MaxValue, true, true, theMoveCreditId, true);
			if (thePoint == null)
			{
				return;
			}
			int comboValue = IsColComboActive() ? mColCountData.mColComboValueDisp : 0;
			thePoint.mValue += (uint)comboValue;
			thePoint.mString = thePoint.mValue.ToString(CultureInfo.InvariantCulture);
			thePoint.mState = (int)Points.POINTSSTATE.STATE_VERT_SHIFTING;
			thePoint.mLimitY = !SexyFramework.GlobalMembers.gIs3D;
			thePoint.mScale = 1f;
			thePoint.mDestScale = theScale;
			thePoint.mDY = 1f;
			ConfigureColComboPoints(thePoint, comboValue > 0);
			if (!string.IsNullOrEmpty(theText))
			{
				Points theTextPoint = AddPoints((int)theX, theY - 100, 0, new Color(16755268), uint.MaxValue, true, false, -1, true);
				if (theTextPoint != null)
				{
					theTextPoint.mString = theText;
					theTextPoint.mState = (int)Points.POINTSSTATE.STATE_VERT_SHIFTING;
					theTextPoint.mLimitY = !SexyFramework.GlobalMembers.gIs3D;
					theTextPoint.mScale = 1f;
					theTextPoint.mDestScale = theScale;
					theTextPoint.mDY = 1f;
					ConfigureColComboPoints(theTextPoint, false);
				}
			}
		}

		public override void ProcessMatches(List<MatchSet> matches, Dictionary<Piece, int> tallySet, bool fromUpdateSwapping)
		{
			for (int i = 0; i < matches.Count; i++)
			{
				MatchSet match = matches[i];
				if (match.mPieces.Count < 3) continue;
				int column = match.mPieces[0].mCol;
				bool vertical = true;
				for (int j = 1; j < match.mPieces.Count; j++)
				{
					vertical &= match.mPieces[j].mCol == column;
				}
				if (vertical)
				{
					for (int j = 0; j < match.mPieces.Count; j++)
					{
						mPendingColumnPieceIds.Add(match.mPieces[j].mId);
					}
					int theColumn = Math.Max(0, Math.Min(7, match.mPieces[0].mCol));
					ColData root = mColData[mColData[theColumn].mLogicalColumn];
					if (mIsPerpetual && root.mActive && WantsTutorial(13))
					{
						DeferTutorialDialog(13, match.mPieces[0]);
						CheckForTutorialDialogs();
						mStartDelay += 150;
					}
				}
			}
		}

		public override void PieceTallied(Piece piece)
		{
			int row = piece.FindRowFromY();
			int column = piece.FindColFromX();
			if (row >= 0 && row < 8 && column >= 0 && column < 8)
			{
				ColData columnData = mColData[column];
				ColData root = mColData[columnData.mLogicalColumn];
				if (mPendingColumnPieceIds.Remove(piece.mId))
				{
					TryClearCol(root.mLogicalColumn, row);
				}
				if (root.mActive)
				{
					columnData.AddRevVel(mMatchPushStr, true);
					if (piece.IsFlagSet(2u))
					{
						columnData.AddRevVel((mSpecialGemPushMod - 1f) * mMatchPushStr, true);
					}
					if (IsColInStorm(column))
					{
						TryClearCol(root.mLogicalColumn, row);
					}
				}
			}
			base.PieceTallied(piece);
		}

		public override void Flamify(Piece piece)
		{
			if (piece.mCol >= 0 && piece.mCol < 8)
			{
				mColData[piece.mCol].AddRevVel((mSpecialGemPushMod - 1f) * mMatchPushStr, true);
			}
			base.Flamify(piece);
		}

		public void UpdateLava(bool advance, bool canLose, bool allowColumnStart)
		{
			if (!mLavaCurvesLoaded)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_UPDATE_LAVA_PANIC_SCALE, mCvPanicScale);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_UPDATE_LAVA_CV_SHAKEY, mCvLavaShakey);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_UPDATE_LAVA_CV_TOP_SNOW, mCvTopSnow);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_UPDATE_LAVA_STORMY_SNOW, mCvStormySnow);
				mLavaCurvesLoaded = true;
			}
			mLoseColumn = -1;
			float baseFireSpeed = GetBaseFireSpeed();
			float maximumLosePct = 0f;
			float currentMaximumLosePct = 0f;
			float stormySnowPct = 0f;
			bool allColumnsHaveTicks = true;
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				ColData root = mColData[Math.Max(0, Math.Min(7, col.mLogicalColumn))];
				if (root.mClearing)
				{
					col.mReverseVelocity = 0f;
					root.mPanicOffset = 0;
					root.mPanicRandom = 0;
					root.mPanicTicks = -1;
				}
				else if (col.mReverseVelocity > 0f)
				{
					col.mReverseVelocity = Math.Max(0f, col.mReverseVelocity - 0.003f);
				}
				if (i != col.mLogicalColumn) continue;
				if (col.mActive && col.mTick <= 0) allColumnsHaveTicks = false;
				col.mPreviousStrength = col.mStrength;
				string iceAnimation;
				if (root.mClearing)
				{
					iceAnimation = SexyFramework.GlobalMembers.gIs3D ? "crush" : "crush2D";
				}
				else if (root.mStrength < 8f)
				{
					iceAnimation = "idle";
				}
				else
				{
					iceAnimation = "crack";
				}
				if (col.mIceAnimation != iceAnimation)
				{
					bool oldCrush = col.mIceAnimation == "crush" || col.mIceAnimation == "crush2D";
					bool newCrush = iceAnimation == "crush" || iceAnimation == "crush2D";
					PopAnim iceAnim = col.GetIceAnim();
					if (iceAnim != null)
					{
						if (oldCrush || newCrush) iceAnim.Play(iceAnimation, true);
						else iceAnim.BlendTo(iceAnimation, 20, 0);
					}
					col.mIceAnimation = iceAnimation;
				}
				string panicAnimation = "blue";
				if (iceAnimation == "crack")
				{
					double losePct = col.GetLosePct(0);
					if (losePct > 1.8) panicAnimation = "panicslow";
					else if (losePct > 1.44) panicAnimation = "red";
					else if (losePct > 0.94) panicAnimation = "panicfast";
					maximumLosePct = Math.Max(maximumLosePct, (float)losePct);
					if (losePct > 0.6) mStormy = true;
					if (col.mPanicTicks >= 0)
					{
						PopAnim iceAnim = col.GetIceAnim();
						PASpriteDef crackDef = iceAnim?.FindSpriteDef("crack");
						if (iceAnim?.mMainSpriteInst != null && crackDef != null)
						{
							iceAnim.mMainSpriteInst.mFrameNum = crackDef.mWorkAreaStart +
								(float)(Math.Min(1.0, losePct * 0.9) * crackDef.mWorkAreaDuration);
						}
					}
					col.mRandomSpeedMultiplier = (float)mCvPanicScale.GetOutVal(losePct);
				}
				if (col.mPanicAnimation != panicAnimation)
				{
					if (panicAnimation == "blue" && col.mDangerAlpha.GetOutVal() == 0.0)
					{
						col.ResetPanicAnim();
					}
					else
					{
						col.mPanicAnimation = panicAnimation;
						col.mFrostPanicAnim?.BlendTo(panicAnimation, 1, 0);
					}
				}
				col.GetIceAnim()?.Update();
				col.mFrostPanicAnim?.Update();
			}

			if (maximumLosePct <= 0.5f)
			{
				if (mBackDim.GetOutVal() != 0.0)
				{
					mBackDim.Intercept("b;0,1,0.01,1,~###         ~####");
				}
			}
			else
			{
				double warningPct = (maximumLosePct - 0.5) / 0.3;
				mBackDim.SetConstant(Math.Min(1.0, warningPct));
			}
			mStormy = maximumLosePct > 0.6f;

			for (int j = 0; j < 8; j++)
			{
				ColData col = mColData[j];
				if (col.mLogicalColumn != j) continue;
				float delta = 0f;
				bool clearDelay = false;
				if (col.mActive && !col.mClearing && col.mClearDelay > 0)
				{
					clearDelay = true;
					if (advance)
					{
						col.mClearDelay = Math.Min(col.mClearDelay, 20);
					}
					col.mClearDelay--;
				}
				else if (!advance || mReprieveActive && col.mClearDelay <= 0)
				{
					delta = 0f;
				}
				else if (col.mActive && !col.mClearing && col.mTick < 1)
				{
					int edge = j;
					if (j >= 4) edge = 7 - j;
					if (col.mSize == 2 && j >= 4) edge--;
					double speed = mCvRowFireSpeed.GetOutVal(col.mStrength / 8f) * mColDistrib.GetOutVal(edge / 4f);
					if (col.mSize >= 2 && edge == 0) speed *= mDoubleEdgeMult;
					double positiveDelta = speed * baseFireSpeed;
					if (col.mAnimationDelay < 1) col.mReprieveRampUp.IncInVal();
					else col.mAnimationDelay--;
					delta = (float)(positiveDelta * col.mReprieveRampUp.GetOutVal());
					delta += delta * col.mSpeedMultiplier;
					if (col.mSize >= 2) delta *= mDoubleColSpeedMult;
				}
				else if (col.mActive && !col.mClearing)
				{
					if (allColumnsHaveTicks) col.mTick = Math.Min(col.mTick, 20);
					col.mTick--;
				}
				float reverseVelocity = 0f;
				for (int k = 0; k < col.mSize && j + k < 8; k++) reverseVelocity += mColData[j + k].mReverseVelocity;
				if (!clearDelay)
				{
					delta -= reverseVelocity;
				}
				if (delta < 0f && col.mStrength > 0f && !mReprieveActive)
				{
					mIceRemoved += Math.Min(col.mStrength, -delta);
					col.mFreezeTime = Math.Min(mFreezeMax, col.mFreezeTime - delta * mFreezeDurationPerNegStrength);
				}
				if (delta > 0f && col.mFreezeTime > 0.0)
				{
					col.mFreezeTime = Math.Max(0.0, col.mFreezeTime - 1.0);
					delta *= (float)Math.Max(0.0, 1.0 - col.mFreezeTime / Math.Max(1.0, mFreezeMax));
				}
				float newStrength = Math.Min(8f, Math.Max(-0.2f, col.mStrength + Math.Max(-0.2f, delta)));
				if (!col.mCracked || col.mClearing || newStrength > 0.5f)
				{
					if (!col.mCracked && newStrength >= 0.5f) col.mCracked = true;
				}
				else
				{
					newStrength = 0.5f;
				}
				SetColumnStr(col, newStrength);
				if (col.mClearing || col.mStrength < 8f)
				{
					col.mPanicOffset = 0;
					col.mPanicRandom = 0;
					if (col.mPanicTicks >= 0) col.mPanicTicks = -1;
				}
				else
				{
					if (advance)
					{
						if (col.mPanicTicks < 0)
						{
							col.mPanicTicks = 0;
							DoBoardShake(false);
						}
						else
						{
							double oldLosePct = col.GetLosePct(0);
							col.mPanicTicks++;
							if (oldLosePct < 0.93 && col.GetLosePct(0) >= 0.93)
							{
								DoBoardShake(true);
							}
						}
					}
					double losePct = col.GetLosePct(0);
					currentMaximumLosePct = Math.Max(currentMaximumLosePct, (float)losePct);
					if (losePct > 0.55)
					{
						stormySnowPct = Math.Max(stormySnowPct,
							Math.Min(1f, (float)((losePct - 0.55) / 0.45)) * 0.5f);
					}
					if (losePct >= 1.0) mLoseColumn = col.mSize == 2 ? j + 1 : j;
					float panicPct = mTotalLoseTicks > 0 ? Math.Min(1f, Math.Max(0f, (float)col.mPanicTicks / mTotalLoseTicks)) : 0f;
					float panicOffsetScale = (float)mCvLavaShakey.GetOutVal(panicPct);
					if ((mGameTicks & 1) == 0)
					{
						int randomRange = Math.Max(0, (int)(panicOffsetScale));
						int target = randomRange == 0 ? 0 : (int)(mRand.Next() % (uint)randomRange) - randomRange / 2;
						int distance = Math.Abs(target - col.mPanicOffset);
						col.mPanicOffset += Math.Sign(target - col.mPanicOffset) * Math.Min(distance, 2);
					}
				}
				if (col.mFreezeTime > 0.0 && !col.mActive)
				{
					col.mFreezeTime = Math.Max(0.0, col.mFreezeTime - 1.0);
				}
				float fxScale = SexyFramework.GlobalMembers.gIs3D ? 1f : 0.5f;
				if (col.mDangerSnowSoft != null)
				{
					float numberScale = delta > 0f && col.mStrength <= 5.328f ? fxScale : 0f;
					for (int layerIdx = 0; layerIdx < col.mDangerSnowSoft.mLayerVector.Count; layerIdx++)
					{
						PILayer layer = col.mDangerSnowSoft.mLayerVector[layerIdx];
						for (int emitterIdx = 0; emitterIdx < layer.mEmitterInstanceVector.Count; emitterIdx++)
						{
							layer.mEmitterInstanceVector[emitterIdx].mNumberScale = numberScale;
						}
					}
				}
				if (col.mDangerSnowHardTop != null)
				{
					float numberScale = (float)mCvTopSnow.GetOutVal(col.GetLosePct(0)) * fxScale;
					for (int layerIdx = 0; layerIdx < col.mDangerSnowHardTop.mLayerVector.Count; layerIdx++)
					{
						PILayer layer = col.mDangerSnowHardTop.mLayerVector[layerIdx];
						for (int emitterIdx = 0; emitterIdx < layer.mEmitterInstanceVector.Count; emitterIdx++)
						{
							layer.mEmitterInstanceVector[emitterIdx].mNumberScale = numberScale;
						}
					}
					col.mDangerSnowHardTop.Update();
				}
				col.mDangerSnowSoft?.Update();
			}
			float stormySnow = (float)mCvStormySnow.GetOutVal(stormySnowPct);
			if (!mGameFinished && mIsPerpetual)
			{
				float numberScale = stormySnow * (SexyFramework.GlobalMembers.gIs3D ? 1f : 0.5f);
				for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector.Count; i++)
				{
					PILayer layer = GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector[i];
					for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
					{
						layer.mEmitterInstanceVector[j].mNumberScale = numberScale;
					}
				}
				UpdateWindSound(stormySnow);
			}
			CustomBassMusicInterface musicInterface = GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface;
			SongInfo songInfo = musicInterface?.FindSong(musicInterface.mSongName);
			if (songInfo != null)
			{
				for (int i = 0; i < songInfo.mTracks.Count; i++)
				{
					songInfo.mTracks[i].mVolume.SetInVal(currentMaximumLosePct);
				}
				musicInterface.mForceParamUpdate = true;
			}
			for (int row = 0; row < 8; row++)
			{
				for (int col = 0; col < 8; col++)
				{
					Piece piece = GetPieceAtRowCol(row, col);
					if (piece != null)
					{
						ColData root = mColData[mColData[col].mLogicalColumn];
						piece.mShakeScale = (float)(Math.Pow(Math.Max(0.0, root.GetLosePct(0) - 0.65) / 0.35, 3.0) * 0.75 *
							(1.0 - mDeathAnimPct.GetOutVal()));
					}
				}
			}
			mOffsetX = 0;
			mOffsetY = 0;
			if (mCvShakey.IsDoingCurve() && (int)mCvShakey.GetOutVal() >= 1)
			{
				int shake = (int)mCvShakey.GetOutVal();
				mOffsetX = (int)(-((shake / 2f) - (mRand.Next() % (uint)shake) * ConstantsWP.INFERNOBOARD_SHAKE_DIST));
				mOffsetY = (int)(-((shake / 2f) - (mRand.Next() % (uint)shake) * ConstantsWP.INFERNOBOARD_SHAKE_DIST));
			}
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				if (col.mLogicalColumn != i || !col.mActive || !col.mClearing) continue;
				double crushPct = col.GetCrushPct();
				if (col.mClearAmount <= 0f || crushPct < 0.1)
				{
					if (crushPct >= 1.0 && (col.GetIceAnim() == null || !col.GetIceAnim().IsActive()))
						ToggleColActive(i, col.mSize, false);
				}
				else
				{
					mIceRemoved += col.mClearAmount;
					col.mClearAmount = 0f;
					DoInfernoPoints(string.Empty, mColDestroyBonus,
						GetBoardX() + GetColX(i) + col.mSize * 50,
						GetBoardY() + 900, 1, 0.75f);
					ResetNextColStartTime();
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_BOTTOM_FROST_PCT_DEACTIVATE, col.mBottomFrostPct);
				}
			}
			if (mLoseColumn >= 0 && canLose && !mReprieveActive && mStartDelay <= 0 && IsBoardStill())
			{
				GameOverAtCol(mLoseColumn);
			}
			GetActiveColCounts(out int activeSingles, out int activeDoubles);
			int activeNonClearing = activeSingles + activeDoubles;
			if (!allowColumnStart || activeNonClearing >= mMaxActiveColCount)
			{
				mNextTryColStart++;
			}
			if (allowColumnStart && (mGameTicks >= mNextTryColStart || activeNonClearing == 0))
			{
				TryStartNewCol();
			}
		}

		public float GetBaseFireSpeed()
		{
			if (!mIsPerpetual) return mBaseFireSpeed;
			float levelSpeed = 0f;
			if (mStageNum > 0 && mLevelData.Count > 0)
			{
				int index = Math.Min(mLevelData.Count - 1, mStageNum - 1);
				levelSpeed = mLevelData[index].mFireSpeed * mFireSpeedMult;
			}
			float stagePct = (float)(mGameTicks - mStageStartAtTick) / mStageDuration;
			float blend = (float)mCvLevelProgress.GetOutVal(stagePct);
			return (levelSpeed + (mBaseFireSpeed - levelSpeed) * blend) *
				(float)mIntroSpeedMod.GetOutVal();
		}

		public override void DoUpdate()
		{
			int previousGameTicks = mGameTicks;
			if (mDeathAnimPct.GetOutFinalVal() == 0.0 || mGameOverCount > 0)
			{
				CallBoardDoUpdate();
			}
			bool gameTicksAdvanced = mGameTicks != previousGameTicks;
			if (!IsGamePaused()) mAnimUpdateCount++;
			float numberScale = (float)mIntroSnow.GetOutVal() * 0.2f *
				(SexyFramework.GlobalMembers.gIs3D ? 1f : 0.5f);
			for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector.Count; i++)
			{
				PILayer layer = GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector[i];
				for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
				{
					layer.mEmitterInstanceVector[j].mNumberScale = numberScale;
				}
			}
			GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.Update();
			if (!mDeathStormySnowCurvesLoaded)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DO_UPDATE_STORMY_SNOW, mCvDeathStormySnow);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DO_UPDATE_STORMY_SNOW_SOUND_FADE, mCvDeathStormySnowSoundFade);
				mDeathStormySnowCurvesLoaded = true;
			}
			if (mIsPerpetual && mGameOverStartUpdateTick > 0)
			{
				double deathRampInput = Math.Pow(mGameOverStartUpdateTick / 200.0, 1.3) * 0.5 + 0.5;
				float stormySnow = (float)mCvDeathStormySnow.GetOutVal(deathRampInput);
				float deathFxScale = SexyFramework.GlobalMembers.gIs3D ? 1f : 0.5f;
				float deathNumberScale = stormySnow * deathFxScale;
				for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector.Count; i++)
				{
					PILayer layer = GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector[i];
					for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
					{
						layer.mEmitterInstanceVector[j].mNumberScale = deathNumberScale;
					}
				}
				float stormySoundFade = (float)mCvDeathStormySnowSoundFade.GetOutVal((mGameTicks - mStormyStartTick) / 100.0);
				UpdateWindSound(stormySnow * stormySoundFade);
			}
			GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.Update();
			if (!IsGamePaused() && mDeathAnimPct.GetOutFinalVal() == 0.0 && mGameTicks >= 2 && GlobalMembers.gApp.mDialogList.Count == 0)
			{
				if (mStartDelay > 0) mStartDelay--;
				if (mReprieveActive && mReprieveStartTick <= mGameTicks)
				{
					mReprieveActive = false;
					for (int i = 0; i < 8; i++)
					{
						ColData column = mColData[i];
						if (column.mLogicalColumn == i && column.mActive && column.mClearDelay == 0 && CalcReverseVelocity(column) != 0.0)
							mReprieveActive = true;
					}
					if (mReprieveStartTick + 300 < mGameTicks) mReprieveActive = false;
				}
				if (mReprieveActive && IsColComboActive() && !IsGameSuspended() && (mGameTicks & 1) == 0)
				{
					mColCountData.mColComboDuration++;
				}
				else if (mGoalSurvival)
				{
					mLevelProgress++;
				}
				else if (mGoalScore)
				{
					mLevelProgress = mPoints;
				}
				bool updateLava = mStartDelay == 0 && !IsGameSuspended();
				mStormy = false;
				if (mReprieveActive)
				{
					for (int i = 0; i < 8; i++)
					{
						ColData column = mColData[i];
						if (column.mLogicalColumn != i || !column.mActive) continue;
						if (column.mClearDelay < 1) column.mReverseVelocity = (float)CalcReverseVelocity(column);
					}
					UpdateLava(updateLava, updateLava, false);
					mStormy = false;
				}
				else
				{
					UpdateLava(updateLava, updateLava, updateLava);
				}
				if (mStormy)
				{
					if (mStormyStartTick == -1) mStormyStartTick = mGameTicks;
				}
				else
				{
					mStormyStartTick = -1;
				}
				if (gameTicksAdvanced && mStormyStartTick >= 0 &&
					(mGameTicks - mStormyStartTick) % 100 == 4)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ICE_WARNING);
				}
			}
			else
			{
				mOffsetX = 0;
				mOffsetY = 0;
			}
			if (!IsGameSuspended() && mIsPerpetual && mLevelProgressTotal > 0)
			{
				float levelProgressPct = (float)mLevelProgress / mLevelProgressTotal;
				if (levelProgressPct >= 1f)
				{
					mReprieveActive = true;
					mReprieveStartTick = mGameTicks + 200;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_MULTIPLIER_TEXT_ALPHA, mMultiplierTextAlpha);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_MULTIPLIER_TEXT_X, mMultiplierTextX);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_MULTIPLIER_TEXT_Y, mMultiplierTextY);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_MULTIPLIER_TEXT_SCALE, mMultiplierTextScale);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_MULTIPLIER_FLASH, mMultiplierFlash);
					PopAnim iceStormUI = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMUI;
					SetIceStormUITransform(iceStormUI, true);
					iceStormUI.Play("multiplierup", true);
					AddDeferredSound(GlobalMembersResourcesWP.SOUND_ICE_STORM_STEAM_BUILD_UP, 0, 0.6);
					AddDeferredSound(GlobalMembersResourcesWP.SOUND_ICE_STORM_STEAM_VALVE, 0, 0.3);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ICE_STORM_MULTIPLER_UP);
					mPointMultiplier++;
					RefreshIceToRemove();
					mIceRemoved = 0f;
					for (int i = 0; i < 8; i++)
					{
						ColData column = mColData[i];
						column.mClearDelay = column.mActive && column.mLogicalColumn == i ? 80 + 5 * i : 0;
					}
					List<int> order = new List<int>(8);
					for (int i = 0; i < 8; i++) order.Add(i);
					for (int i = order.Count - 1; i > 0; i--)
					{
						int j = (int)(mRand.Next() % (uint)(i + 1));
						int value = order[i];
						order[i] = order[j];
						order[j] = value;
					}
					int animationDelay = 0;
					int activeCount = 0;
					for (int i = 0; i < order.Count; i++)
					{
						ColData column = mColData[order[i]];
						if (!column.mActive || column.mClearing) continue;
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_REPRIEVE_RAMP_UP, column.mReprieveRampUp);
						column.mAnimationDelay = animationDelay;
						animationDelay += (int)(mRand.Next() % 30) + 30 + 20 * activeCount++;
					}
				}
			}
		}

		public override void Update()
		{
			mCurveUpdateCount.value = mUpdateCnt;
			if (mStartDelay == 0)
			{
				SyncProgress();
			}
			int backgroundColor = mBackDim.GetOutVal() == 0.0 ? 255 :
				(int)((1.0 - mBackDim.GetOutVal()) * 155.0 + 100.0);
			if (mBackground != null)
			{
				mBackground.mColor = new Color(backgroundColor, backgroundColor, backgroundColor);
			}
			Points point = mPointsManager?.Find((uint)mComboPointId);
			Points point2 = mPointsManager?.Find((uint)(mComboPointId + 1));
			if (point != null && point2 != null)
			{
				point2.mAlpha = point.mAlpha;
			}
			base.Update();
			mCurveUpdateCount.value = mUpdateCnt;
			if (mBackground != null)
			{
				mBackground.mWantAnim = false;
			}
			if (!IsGamePaused())
			{
				GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMUI.Update();
				if (mDeathAnimPct.GetOutVal() == 0.0)
				{
					GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMFILL.Update();
				}
			}
			mShakeCooldown = Math.Max(0, mShakeCooldown - 1);
			if (mIsPerpetual)
			{
				if (!mMultiplierTextCurvesLoaded)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_UPDATE_Y_FADE, mCvYFade);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_UPDATE_SCALE_IN, mCvScaleIn);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_UPDATE_WOBBLE_IN, mCvWobbleIn);
					mMultiplierTextCurvesLoaded = true;
				}
				for (int i = 0; i < 2; i++)
				{
					Points comboPoint = mPointsManager?.Find((uint)(mComboPointId + i));
					if (comboPoint == null) continue;
					if (IsColComboActive() && mColCountData.mColComboDuration != 0)
					{
						if (comboPoint.mState != (int)Points.POINTSSTATE.STATE_FADING)
						{
							float updatePct = Math.Max(0f, Math.Min(1f,
								(float)(mUpdateCnt - mColCountData.mColComboStartUpdateTick) /
								mColCountData.mColComboDuration));
							float gamePct = Math.Max(0f, Math.Min(1f,
								(float)(mGameTicks - mColCountData.mColComboStartTick) /
								mColCountData.mColComboDuration));
							float alphaPct = (float)mCvYFade.GetOutVal(gamePct);
							comboPoint.mAlpha = 1f - alphaPct;
							float scale = (float)mCvScaleIn.GetOutVal(updatePct) + (float)GetColComboPct() * 0.115f;
							if (i == 1) scale *= 1.25f;
							comboPoint.mScale = scale;
							comboPoint.mDestScale = scale;
							comboPoint.mY = mComboPointY + i * (comboPoint.mScale * 80f) + alphaPct * 100f;
							float rotation = (float)mCvWobbleIn.GetOutVal(updatePct);
							if (mComboPointRotation % 2 == 0) rotation = -rotation;
							comboPoint.mRotation = rotation * ((mComboPointRotation % 1000000) / 1000000f * 0.75f + 0.75f);
						}
					}
					else
					{
						comboPoint.mState = (int)Points.POINTSSTATE.STATE_FADING;
					}
				}
			}
			mIceMeterFlashPct = Math.Max(0.0, mIceMeterFlashPct - 0.005);
			if (!IsColComboActive() && mColCountData.mColComboAlpha.GetOutFinalVal() != 0.0)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_COL_COMBO_ALPHA, mColCountData.mColComboAlpha);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_COL_COMBO_Y, mColCountData.mColComboY);
			}
			mDeathAnimPct.IncInVal();
			if (mDeathAnimPct.GetOutVal() >= 1.0)
			{
				base.GameOver(true);
			}
		}

		public void UpdateWindSound(double theVolume)
		{
			if (theVolume > 0.0 && mWindSound == null)
			{
				mWindSound = GlobalMembers.gApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_ICE_STORM_WIND);
				if (mWindSound != null)
				{
					mWindSound.Play(true, false);
				}
			}
			if (mWindSound != null)
			{
				mWindSound.SetVolume(theVolume);
				if (mWindSound.GetVolume() == 0.0)
				{
					mWindSound.Release();
					mWindSound = null;
				}
			}
		}

		public override bool CanPlay()
		{
			return base.CanPlay() && mLoseColumn < 0 && mDeathAnimPct.GetOutVal() == 0.0;
		}

		public override bool AllowSpeedBonus()
		{
			return mAllowSpeedBonus;
		}

		public override string GetSavedGameName()
		{
			return mAllowSpeedBonus ? "inferno_storm.sav" : base.GetSavedGameName();
		}

		public override void GameOver(bool visible)
		{
			if (mDeathAnimPct.GetOutVal() > 0.0) return;
			CustomBassMusicInterface musicInterface = (CustomBassMusicInterface)GlobalMembers.gApp.mMusicInterface;
			musicInterface.QueueEvent("FadeOut", GetMusicName(), false);
			musicInterface.QueueEvent("Play", GetMusicName() + "_lose", true);
			AddDeferredSound(GlobalMembersResourcesWP.SOUND_ICE_STORM_GAMEOVER, 0, 1.0);
			mGameOverStartUpdateTick = mUpdateCnt;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DEATH_ANIM_PCT, mDeathAnimPct);
			PopAnim iceStormUI = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMUI;
			iceStormUI.Play("idle", true);
			mOffsetX = 0;
			mOffsetY = 0;
			for (int row = 0; row < 8; row++)
			{
				for (int col = 0; col < 8; col++)
				{
					Piece piece = mBoard[row, col];
					if (piece != null)
					{
						piece.ClearFlag(1u);
						piece.ClearBoundEffects();
						piece.mShakeScale = 0f;
						piece.mShakeOfsX = 0f;
						piece.mShakeOfsY = 0f;
					}
				}
			}
		}

		public void GameOverAtCol(int theColumn)
		{
			mLoseColumn = theColumn;
			GameOver(true);
		}

		public override bool ExtraSaveGameInfo()
		{
			return true;
		}

		public override bool WantsHideOnPause()
		{
			return true;
		}

		public override bool WantHypermixerEdgeCheck()
		{
			return mHypermixerDelay == 0;
		}

		public override bool WantHypermixerBottomCheck()
		{
			return false;
		}

		public override bool WantTopLevelBar()
		{
			return GetLevelPoints() > 0;
		}

		public override int GetHintTime()
		{
			return mIsPerpetual ? 5 : base.GetHintTime();
		}

		public override float GetModePointMultiplier()
		{
			if (mIsPerpetual)
			{
				return 2f;
			}
			return base.GetModePointMultiplier();
		}

		public override float GetRankPointMultiplier()
		{
			return 6.6667f;
		}

		public override int GetBottomWidgetOffset()
		{
			return mIsPerpetual ? -50 : 0;
		}

		public override bool WantDrawButtons()
		{
			return !mIsPerpetual;
		}

		public override bool WantDrawScore()
		{
			return !mIsPerpetual;
		}

		public override int GetBoardX()
		{
			return ConstantsWP.INFERNOBOARD_BOARD_X;
		}

		public override int GetBoardY()
		{
			return ConstantsWP.INFERNOBOARD_BOARD_Y;
		}

		public override int GetTitleY()
		{
			return 65;
		}

		public override Rect GetCountdownBarRect()
		{
			return new Rect();
		}

		public override Image GetMultiplierImage()
		{
			return GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_MULTIPLIER;
		}

		public override int GetMultiplierImageX()
		{
			return GlobalMembers.S((int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_MULTIPLIER_ID));
		}

		public override int GetMultiplierImageY()
		{
			return GlobalMembers.S((int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_MULTIPLIER_ID));
		}

		public override int GetLevelPoints()
		{
			return mIsPerpetual ? (int)(mIceToRemove * 10000f) : base.GetLevelPoints();
		}

		public override int GetLevelPointsTotal()
		{
			return mIsPerpetual ? (int)(mIceRemoved * 10000f) : base.GetLevelPointsTotal();
		}

		public override string GetMusicName()
		{
			return "Icestorm";
		}

		public override bool WantWarningGlow()
		{
			for (int i = 0; i < 8; i++)
			{
				if (mColData[i].mStrength > 8f) return true;
			}
			return base.WantWarningGlow();
		}

		public override void SetupBackground(int deltaIdx)
		{
			SetBackground("images\\960\\backgrounds\\pointy_ice_path_purple");
		}

		public override void RefreshUI()
		{
			CallBoardRefreshUI();
		}

		public override void UpdateCountPopups()
		{
			base.UpdateCountPopups();
		}

		public override int GetGemCountPopupThreshold()
		{
			return 0;
		}

		public void DrawLava(Graphics g, int priority)
		{
			if (!mContentLoaded) return;
			g.PushState();
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				if (col.mLogicalColumn != i) continue;
				if ((priority == 0 && col.mClearing) || (priority == 1 && !col.mClearing)) continue;
				int x = GlobalMembers.S(GetBoardX() + 100 * i) +
					GlobalMembers.S(col.GetIceAnimXOffset());
				int y = GlobalMembers.S(GetBoardY()) - GlobalMembers.S(8);
				float visibleStrength = col.mActive && col.mCracked && !col.mClearing ? Math.Max(0.5f, col.mStrength) : Math.Max(0f, col.mStrength);
				g.PushState();
				g.Translate(x, y);
				g.SetClipRect(-(int)g.mTransX, -(int)g.mTransY,
					GlobalMembers.S(800) + (int)g.mTransX,
					GlobalMembers.S(800) + (int)g.mTransY);
				g.Translate(GlobalMembers.S(col.mSize == 1 ? -40 : -24),
					GlobalMembers.S(col.mSize == 1
							? ConstantsWP.INFERNOBOARD_MIN_Y_ONE_COLUMN
							: ConstantsWP.INFERNOBOARD_MIN_Y_TWO_COLUMN));
				g.Translate(0, (int)GlobalMembers.S((float)(col.mBumpY.GetOutVal() + (8f - visibleStrength) * 100f)));
				double losePct = col.GetLosePct(0);
				if (losePct <= 1.3)
				{
					if (col.mWarningAlpha.GetOutFinalVal() == 1.0)
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_WARNING_ALPHA_2, col.mWarningAlpha);
						col.mWarningAlpha.Intercept(null, 0.01, false);
					}
				}
				else if (col.mWarningAlpha.GetOutFinalVal() == 0.0)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_WARNING_ALPHA_1, col.mWarningAlpha);
					col.mWarningAlpha.Intercept(null, 0.01, false);
				}
				float glowPct = (float)Math.Min(1.0, Math.Max(0.0, col.mWarningAlpha.GetOutVal()));
				if (glowPct > 0f)
				{
					float pulse = (float)((Math.Sin((mUpdateCnt % 101) * Math.PI * 2.0 / 100.0) + 1.0) * 0.5);
					Image glow = col.mSize == 2
						? GlobalMembersResourcesWP.IMAGE_ANIMS_COLUMN2_COLUMN2_GLOW
						: GlobalMembersResourcesWP.IMAGE_ANIMS_COLUMN1_COLUMN1_GLOW;
					g.PushState();
					g.SetColor(new Color(255, 255, 255, (int)(255f * pulse * glowPct * GetAlpha())));
					g.SetColorizeImages(true);
					g.DrawImage(glow, col.mSize == 1 ? GlobalMembers.S(18) : 0, 0);
					g.SetColorizeImages(false);
					g.PopState();
				}
				EnableDarkenColor(g, 150);
				PopAnim anim = col.GetIceAnim();
				if (anim != null)
				{
					anim.Draw(g);
				}
				DisableDarkenColor(g);
				g.ClearClipRect();
				g.PopState();
			}
			g.PopState();
		}

		public void DrawTopSkullFrame(Graphics g)
		{
			if (!mContentLoaded) return;
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				if (col.mLogicalColumn != i || col.mDangerAlpha.GetOutVal() <= 0.0) continue;
				Image mountain = col.mSize == 2
					? GlobalMembersResourcesWP.IMAGE_QUEST_INFERNO_LAVA_MOUNTAINDOUBLE
					: GlobalMembersResourcesWP.IMAGE_QUEST_INFERNO_LAVA_MOUNTAINSINGLE;
				float framePct = (float)col.mDangerAlpha.GetOutVal();
				float rise = (float)col.mDangerY.GetOutVal();
				g.PushState();
				g.SetClipRect(0, 0, GlobalMembers.gApp.mWidth,
					GlobalMembers.S((int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_INFERNO_LAVA_UI_TOP_FRAME_ID) +
							ConstantsWP.INFERNOBOARD_TOPSKULL_RISE_OFFSET_1));
				float columnCenter = i + col.mSize * 0.5f;
				g.Translate((int)GlobalMembers.S(GetBoardX() + columnCenter * 100f),
					GlobalMembers.S((int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_INFERNO_LAVA_UI_TOP_FRAME_ID)));
				if (framePct < 1f)
				{
					g.SetColor(Color.FAlpha(framePct));
					g.SetColorizeImages(true);
				}
				g.PushState();
				g.Translate(0, (int)GlobalMembers.S(rise));
				double losePct = col.GetLosePct(0);
				double mountainPct = Math.Min(1.0, Math.Max(0.0, (losePct - 0.4) / 0.53));
				if (mountainPct > 0.0)
				{
					Utils.DrawImageCentered(g, mountain, 0,
						GlobalMembers.S(95) - (float)(mountain.mHeight * mountainPct), 1f, 1f);
				}
				g.PopState();
				g.ClearClipRect();
				g.Translate(0, ConstantsWP.INFERNOBOARD_TOPSKULL_OFFSET_Y_0);
				float frostScale = framePct * 0.5f + 0.5f;
				int frostUnderY = (int)(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_INFERNO_LAVA_FROST_TOP_UNDER_ID) -
					GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_INFERNO_LAVA_FROST_TOP_ID));
				Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_QUEST_INFERNO_LAVA_FROST_TOP_UNDER,
					GlobalMembers.S(0), GlobalMembers.S(frostUnderY) + GlobalMembers.S(0),
					col.mSize * frostScale, frostScale);
				if (col.mDangerSnowHardTop != null)
				{
					g.PushColorMult();
					col.mDangerSnowHardTop.Draw(g);
					g.PopColorMult();
				}
				Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_QUEST_INFERNO_LAVA_FROST_TOP,
					GlobalMembers.S(0), GlobalMembers.S(0),
					col.mSize * frostScale, frostScale);
				if (col.mFrostPanicAnim?.mMainSpriteInst != null)
				{
					float panicScale = (float)col.mRandomSpeedMultiplier;
					PASpriteInst panicSprite = col.mFrostPanicAnim.mMainSpriteInst;
					PAObjectPos panicObject = panicSprite.mDef.mFrames[(int)panicSprite.mFrameNum]
						.mFrameObjectPosVector[0];
					Image panicImage = col.mFrostPanicAnim.mImageVector[panicObject.mResNum]
						.mImages[0].GetImage();
					float panicCenterX = panicImage.mWidth * 0.5f;
					g.PushState();
					g.Translate((int)Math.Round(-panicCenterX),
						ConstantsWP.INFERNOBOARD_TOPSKULL_OFFSET_Y_1 + (int)GlobalMembers.S(rise));
					g.SetColor(Color.FAlpha(framePct));
					g.SetColorizeImages(true);
					g.PushColorMult();
					col.mFrostPanicAnim.mTransform.LoadIdentity();
					col.mFrostPanicAnim.mTransform.Translate(
						-panicCenterX, GlobalMembers.S(ConstantsWP.INFERNOBOARD_TOPSKULL_OFFSET_Y_2));
					col.mFrostPanicAnim.mTransform.Scale(panicScale, panicScale);
					col.mFrostPanicAnim.mTransform.Translate(
						panicCenterX,
						GlobalMembers.S(-ConstantsWP.INFERNOBOARD_TOPSKULL_OFFSET_Y_2 +
								(1.5f - panicScale) * 40f));
					col.mFrostPanicAnim.Draw(g);
					g.PopColorMult();
					g.PopState();
				}
				g.SetColor(Color.White);
				g.SetColorizeImages(false);
				g.ClearClipRect();
				g.PopState();
			}
		}

		public void DrawIceMeter(Graphics g)
		{
			g.Translate(GlobalMembers.S(mOffsetX), GlobalMembers.S(mOffsetY));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_PIPE,
				GlobalMembers.S((int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_PIPE_ID)),
				GlobalMembers.S((int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_PIPE_ID)));
			int liquidX = GlobalMembers.S(
				(int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID_ID));
			int liquidY = GlobalMembers.S(
				(int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID_ID));
			PopAnim iceStormFill = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMFILL;
			g.PushState();
			g.SetClipRect(liquidX, liquidY,
				GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID.mWidth,
				GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID.mHeight);
			g.Translate((int)((78f - (1f - mLevelBarPct) * 410f) *
				GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_LIQUID.mWidth / 436f), 0);
			EnableDarkenColor(g, 200);
			iceStormFill.Draw(g);
			DisableDarkenColor(g);
			if (mIceMeterFlashPct > 0.0)
			{
				g.SetColor(Color.FAlpha((float)Math.Min(1.0, mIceMeterFlashPct)));
				g.SetColorizeImages(true);
				g.SetDrawMode(Graphics.DrawMode.Additive);
				iceStormFill.Draw(g);
				g.SetDrawMode(Graphics.DrawMode.Normal);
				g.SetColorizeImages(false);
			}
			g.SetColor(Color.White);
			g.ClearClipRect();
			g.PopState();
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_METER,
				GlobalMembers.S((int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_ID)),
				GlobalMembers.S((int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_ID)));
			if (mGameOverStartUpdateTick >= 1)
			{
				if (!mIceAlphaCurveLoaded)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
						PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DRAW_ICE_ALPHA,
						mCvIceAlpha);
					mIceAlphaCurveLoaded = true;
				}
				float frostPct = (float)mCvIceAlpha.GetOutVal(
					(mUpdateCnt - mGameOverStartUpdateTick) / 100f);
				if (frostPct > 0f)
				{
					g.PushState();
					g.SetColor(Color.FAlpha(frostPct));
					g.SetColorizeImages(true);
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_ICE,
						GlobalMembers.S((int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_ICE_ID)),
						GlobalMembers.S((int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_METER_ICE_ID)));
					g.PopState();
				}
			}
			g.Translate(GlobalMembers.S(-mOffsetX), GlobalMembers.S(-mOffsetY));
		}

		public override void DrawUI(Graphics g)
		{
			float alpha = GetAlpha();
			g.SetColor(Color.FAlpha(alpha));
			g.SetColorizeImages(alpha < 1f);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_TOP_FRAME,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_TOP_FRAME_ID)) + mTransBoardOffsetX,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_TOP_FRAME_ID)) - mTransBoardOffsetY);
			DrawIceMeter(g);
			DrawMenuWidget(g);
			DrawScoreWidget(g);
			if (mIsPerpetual)
			{
				DrawScore(g);
			}
			DrawPointMultiplier(g, false);
			DrawPointMultiplier(g, true);
			g.SetColorizeImages(false);
		}

		public override void DrawTopFrame(Graphics g)
		{
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_INFERNO_LAVA_UI_TOP_FRAME,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_QUEST_INFERNO_LAVA_UI_TOP_FRAME_ID) - 160f),
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_INFERNO_LAVA_UI_TOP_FRAME_ID) + 10f));
		}

		public override void DrawBottomFrame(Graphics g)
		{
		}

		public void EnableDarkenColor(Graphics g, int thePriority)
		{
			if (thePriority == 0 || mDarkenBoard.GetOutVal() >= 1.0)
			{
				return;
			}
			float theDarken = (float)Math.Min(1.0, Math.Max(0.0, mDarkenBoard.GetOutVal()));
			int theColor = (int)(255f - thePriority + thePriority * theDarken);
			g.SetColor(new Color(theColor, theColor, theColor));
			g.SetColorizeImages(true);
			g.PushColorMult();
		}

		public void DisableDarkenColor(Graphics g)
		{
			if (mDarkenBoard.GetOutVal() < 1.0)
			{
				g.PopColorMult();
			}
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			g.SetColor(Color.FAlpha(GetAlpha()));
			g.PushColorMult();
			g.PushState();
			float alpha = GetAlpha();
			g.SetColor(Color.FAlpha(alpha));
			g.SetColorizeImages(true);
			if (thePriority == 3)
			{
				DrawLavaParticles(g, false);
				Piece tutorialIrisPiece = GetTutorialIrisPiece();
				if (mWarningGlowAlpha > 0f || tutorialIrisPiece != null)
				{
					if (tutorialIrisPiece != null)
					{
						g.SetColor(new Color(3355443, (int)(mTutorialPieceIrisPct.GetOutVal() * 255f)));
					}
					else
					{
						g.SetColor(new Color(3355443, (int)(mWarningGlowAlpha * 125f)));
					}
					g.PushColorMult();
					DrawLavaParticles(g, false);
					g.PopColorMult();
					g.SetColor(Color.FAlpha(alpha));
				}
				g.PopColorMult();
			}
			else
			{
				if (thePriority == 1)
				{
					DrawLavaParticles(g, true);
				}
				if (mGameOverStartUpdateTick >= 1)
				{
					if (!mIceAlphaCurveLoaded)
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(
							PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DRAW_ICE_ALPHA,
							mCvIceAlpha);
						mIceAlphaCurveLoaded = true;
					}
					g.SetColorizeImages(true);
					float frostPct = (float)mCvIceAlpha.GetOutVal(
						(mUpdateCnt - mGameOverStartUpdateTick) / 100f);
					if (frostPct > 0.0)
					{
						g.PushState();
						g.SetClipRect(GlobalMembers.S(GetBoardX()), GlobalMembers.S(GetBoardY()),
							GlobalMembers.S(800), GlobalMembers.S(800));
						g.SetColor(new Color(255, 255, 255, (int)(frostPct * 255f)));
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_INFERNO_LAVA_FROST_LOSE,
							GlobalMembers.S(GetBoardX() - 30), GlobalMembers.S(GetBoardY() - 40));
						g.PopState();
					}
				}
				if (mIsPerpetual && mMultiplierTextAlpha.GetOutVal() > 0.0)
				{
					RefreshMultiplierText();
					g.PushState();
					float multiplierTextAlpha = (float)mMultiplierTextAlpha.GetOutVal();
					g.SetColor(Color.FAlpha(multiplierTextAlpha));
					g.SetColorizeImages(multiplierTextAlpha < 1f);
					float scale = (float)mMultiplierTextScale.GetOutVal();
					if (mMultiplierTextImage != null)
					{
						int x = GlobalMembers.S(10) + (int)GlobalMembers.S(
							GetBoardCenterX() + (float)mMultiplierTextX.GetOutVal());
						int y = (int)GlobalMembers.S((float)mMultiplierTextY.GetOutVal());
						Utils.MyDrawImageRotated(g, mMultiplierTextImage, x, y, 0.0, scale, scale);
					}
					g.PopState();
				}
				base.DrawOverlay(g, thePriority);
				g.PopColorMult();
			}
			g.PopState();
			g.SetColorizeImages(false);
		}

		public void DrawLavaParticles(Graphics g, bool drawFrost)
		{
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				if (col.mLogicalColumn != i) continue;
				g.PushState();
				g.Translate(GetBoardX() + (int)GlobalMembers.S((i + col.mSize * 0.5f) * 100f),
					GlobalMembers.S(GetBoardY() + 820));
				if (!drawFrost)
				{
					col.mDangerSnowSoft?.Draw(g);
				}
				else
				{
					double pct = col.mBottomFrostPct.GetOutVal();
					if (pct > 0.0)
					{
						g.SetColor(Color.FAlpha((float)(pct * GetAlpha())));
						g.SetColorizeImages(true);
						Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_QUEST_INFERNO_LAVA_FROST_BOTTOM, GlobalMembers.S(-4), GlobalMembers.S(-16),
							col.mSize * (float)(pct * 0.5 + 0.3),
							(float)(pct * 0.5 + 0.5));
						g.SetColor(Color.White);
						g.SetColorizeImages(false);
					}
				}
				g.PopState();
			}
		}

		public override void DrawScore(Graphics g)
		{
			if (!mIsPerpetual)
			{
				base.DrawScore(g);
				return;
			}
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			g.SetColor(new Color(255, 255, 255, (int)(GetAlpha() * 255f)));
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), "GLOW", new Color(-1627389952));
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Color.White);
			string text = SexyFramework.Common.CommaSeperate(mDispPoints);
			int x = mWidth / 2;
			int y = (int)((GlobalMembers.IMG_SYOFS(1091) + GlobalMembersResources.FONT_DIALOG.mAscent) / 2f) - mTransScoreOffsetY - 50;
			g.PushState();
			g.SetScale(ConstantsWP.BOARD_LEVEL_SCORE_SCALE, ConstantsWP.BOARD_LEVEL_SCORE_SCALE,
				x, y - g.GetFont().GetAscent() / 2);
			g.WriteString(text, x, y + ConstantsWP.INFERNOBOARD_BOARD_Y / 2);
			g.PopState();
		}

		public override void DrawScoreWidget(Graphics g)
		{
			if (!mIsPerpetual)
			{
				base.DrawScoreWidget(g);
			}
		}

		public new void DrawPointMultiplier(Graphics g, bool front)
		{
			base.DrawPointMultiplier(g, front);
		}

		public override void DrawMenuWidget(Graphics g)
		{
			if (!mIsPerpetual) base.DrawMenuWidget(g);
		}

		public override void SwapSucceeded(SwapData swapData)
		{
			base.SwapSucceeded(swapData);
			if (mHypermixerDelay > 0) mHypermixerDelay--;
		}

		public override void HypermixerDropped()
		{
			mHypermixerDelay = 15;
		}

		public void ContinueGameScramble()
		{
			mStartDelay = 150;
			InitLavaCols();
			SyncToLevel(mStageNum);
			mLoseColumn = -1;
			mDeathAnimPct.SetConstant(0.0);
			mGameOverStartUpdateTick = 0;
		}

		public override void Draw(Graphics g)
		{
			float alpha = GetAlpha();
			bool popAlpha = false;
			if (alpha > 0f && alpha < 1f)
			{
				g.SetColor(Color.FAlpha(alpha));
				g.SetColorizeImages(true);
				g.PushColorMult();
				popAlpha = true;
			}
			if (alpha > 0f)
			{
				GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.Draw(g);
			}
			double snowAlpha = mIntroSnow.GetOutVal();
			if (snowAlpha > 0.0)
			{
				bool popSnow = false;
				if (snowAlpha < 1.0)
				{
					g.SetColor(Color.FAlpha((float)snowAlpha));
					g.PushColorMult();
					g.SetColorizeImages(true);
					popSnow = true;
				}
				g.Translate(GlobalMembers.S(-160), 0);
				GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.Draw(g);
				g.Translate(GlobalMembers.S(160), 0);
				if (popSnow)
				{
					g.PopColorMult();
					g.SetColorizeImages(alpha < 1f);
				}
			}
			base.Draw(g);
			DrawLava(g, 0);
			DrawLava(g, 1);
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_ICE_STORM_ICE_BOTTOM,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_BOTTOM_ID)) + mTransBoardOffsetX,
				(int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_ICE_STORM_ICE_BOTTOM_ID)) - mTransBoardOffsetY);
			DrawTopSkullFrame(g);
			if (mOffsetX != 0 || mOffsetY != 0)
			{
				g.Translate(GlobalMembers.S(mOffsetX), GlobalMembers.S(mOffsetY));
			}
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawGameElements(g);
			}
			DrawGameElements(g);
			CallBoardDrawGameElements(g);
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawGameElementsPost(g);
			}
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				if (col.mLogicalColumn == i)
				{
					if (col.mStrength >= 8f && !col.mClearing)
					{
						if (col.mDangerAlpha.GetOutFinalVal() != 1.0)
						{
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DANGER_ALPHA_ASCEND, col.mDangerAlpha);
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DANGER_Y_ASCEND, col.mDangerY);
						}
					}
					else if (col.mDangerAlpha.GetOutFinalVal() != 0.0)
					{
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DANGER_ALPHA_DESCEND, col.mDangerAlpha);
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eINFERNO_BOARD_DANGER_Y_DESCEND, col.mDangerY);
					}
				}
			}
			DrawIceStormUIPam(g);
			if (popAlpha)
			{
				g.PopColorMult();
				g.SetColorizeImages(false);
			}
			DeferOverlay(3);
		}

		public override bool SaveGameExtra(Serialiser buffer)
		{
			if (mGameClosing || mReprieveActive || mDeathAnimPct.GetOutFinalVal() == 1.0) return false;
			int chunk = buffer.WriteGameChunkHeader(GameChunkId.eChunkInfernoBoard);
			buffer.WriteValuePair(Serialiser.PairID.IceStormStageNum, mStageNum);
			for (int i = 0; i < 8; i++)
			{
				if (mColData[i].mClearing) return false;
				buffer.WriteShort((short)mColData[i].mSize);
			}
			for (int i = 0; i < 8; i++)
			{
				ColData col = mColData[i];
				buffer.WriteFloat(col.mPreviousStrength);
				buffer.WriteFloat(col.mStrength);
				buffer.WriteFloat(col.mReverseVelocity);
				buffer.WriteFloat(col.mSpeedMultiplier);
				buffer.WriteFloat(col.mClearAmount);
				buffer.WriteDouble(col.mFreezeTime);
				buffer.WriteDouble(col.mRandomSpeedMultiplier);
				buffer.WriteDouble(col.mEdgeSpeedMultiplier);
				buffer.WriteBoolean(col.mCracked);
				buffer.WriteBoolean(col.mActive);
				buffer.WriteBoolean(col.mClearing);
				buffer.WriteShort((short)col.mPanicOffset);
				buffer.WriteShort((short)col.mPanicRandom);
				buffer.WriteShort((short)col.mClearDelay);
				buffer.WriteShort((short)col.mConfiguredSize);
				buffer.WriteShort((short)col.mBaseColumn);
				buffer.WriteLong(col.mPanicTicks);
				buffer.WriteLong(col.mTick);
				buffer.WriteLong(col.mLogicalColumn);
				buffer.WriteLong(col.mAnimationDelay);
				buffer.WriteCurvedVal(col.mDangerY);
				buffer.WriteCurvedVal(col.mDangerAlpha);
				buffer.WriteCurvedVal(col.mBottomFrostPct);
				buffer.WriteCurvedVal(col.mBumpY);
				buffer.WriteCurvedVal(col.mReprieveRampUp);
			}
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountColComboValueDisp, mColCountData.mColComboValueDisp);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountStartTick, mColCountData.mColComboStartTick);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountStartUpdateTick, mColCountData.mColComboStartUpdateTick);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountDuration, mColCountData.mColComboDuration);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountComboCount, mColCountData.mColComboCount);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountColComboAlpha, mColCountData.mColComboAlpha);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountColComboScale, mColCountData.mColComboScale);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColCountColComboY, mColCountData.mColComboY);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColComboBonusPoints, mColComboBonusPoints);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColComboHighest, mColComboHighest);
			buffer.WriteValuePair(Serialiser.PairID.IceStormColClearBonusPoints, mColClearBonusPoints);
			buffer.WriteValuePair(Serialiser.PairID.IceStormAnimUpdateCount, mAnimUpdateCount);
			buffer.WriteValuePair(Serialiser.PairID.IceStormLoseColumn, mLoseColumn);
			buffer.WriteValuePair(Serialiser.PairID.IceStormLastIceRemoved, mLastIceRemoved);
			buffer.WriteValuePair(Serialiser.PairID.IceStormIceRemoved, mIceRemoved);
			buffer.WriteValuePair(Serialiser.PairID.IceStormStartDelay, mStartDelay);
			buffer.WriteValuePair(Serialiser.PairID.IceStormLevelProgress, mLevelProgress);
			buffer.WriteValuePair(Serialiser.PairID.IceStormLevelProgressTotal, mLevelProgressTotal);
			buffer.WriteValuePair(Serialiser.PairID.IceStormTotalLoseTicks, mTotalLoseTicks);
			buffer.WriteValuePair(Serialiser.PairID.IceStormNextTryColStart, mNextTryColStart);
			buffer.WriteValuePair(Serialiser.PairID.IceStormStageDuration, mStageDuration);
			buffer.WriteValuePair(Serialiser.PairID.IceStormStageStartAtTick, mStageStartAtTick);
			buffer.WriteValuePair(Serialiser.PairID.IceStormShakeCooldown, mShakeCooldown);
			buffer.WriteValuePair(Serialiser.PairID.IceStormBackDim, mBackDim);
			buffer.FinalizeGameChunkHeader(chunk);
			return base.SaveGameExtra(buffer);
		}

		public override void LoadGameExtra(Serialiser buffer)
		{
			mCurveUpdateCount.value = mUpdateCnt;
			ClearComboPoints();
			PopAnim iceStormUI = GlobalMembersResourcesWP.POPANIM_QUEST_INFERNO_ICESTORMUI;
			SetIceStormUITransform(iceStormUI, false);
			iceStormUI.Play("idle", true);
			mIntroSpeedMod.SetConstant(1.0);
			mDarkenBoard.SetConstant(1.0);
			for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector.Count; i++)
			{
				PILayer layer = GlobalMembersResourcesWP.PIEFFECT_ICE_STORMY.mLayerVector[i];
				for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
				{
					layer.mEmitterInstanceVector[j].mNumberScale = 0f;
				}
			}
			for (int i = 0; i < GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector.Count; i++)
			{
				PILayer layer = GlobalMembersResourcesWP.PIEFFECT_BLOWING_SNOW.mLayerVector[i];
				for (int j = 0; j < layer.mEmitterInstanceVector.Count; j++)
				{
					layer.mEmitterInstanceVector[j].mNumberScale = 0f;
				}
			}
			mColCountData.mText = string.Empty;
			if (mWindSound != null)
			{
				mWindSound.Release();
				mWindSound = null;
			}
			mReprieveActive = false;
			int chunkBegin;
			GameChunkHeader header = new GameChunkHeader();
			if (buffer.CheckReadGameChunkHeader(GameChunkId.eChunkInfernoBoard, header, out chunkBegin))
			{
				buffer.ReadValuePair(out mStageNum);
				SyncToLevel(mStageNum);
				for (int i = 0; i < 8; i++)
				{
					mColData[i].Reset(this);
					mColData[i].ResetDangerSnowEffects();
					int savedSize = buffer.ReadShort();
					mColData[i].SetColSize(savedSize, true);
				}
				for (int i = 0; i < 8; i++)
				{
					ColData col = mColData[i];
					col.mPreviousStrength = buffer.ReadFloat();
					col.mStrength = buffer.ReadFloat();
					col.mReverseVelocity = buffer.ReadFloat();
					col.mSpeedMultiplier = buffer.ReadFloat();
					col.mClearAmount = buffer.ReadFloat();
					col.mFreezeTime = buffer.ReadDouble();
					col.mRandomSpeedMultiplier = buffer.ReadDouble();
					col.mEdgeSpeedMultiplier = buffer.ReadDouble();
					col.mCracked = buffer.ReadBoolean();
					col.mActive = buffer.ReadBoolean();
					col.mClearing = buffer.ReadBoolean();
					col.mPanicOffset = buffer.ReadShort();
					col.mPanicRandom = buffer.ReadShort();
					col.mClearDelay = buffer.ReadShort();
					col.mConfiguredSize = buffer.ReadShort();
					col.mBaseColumn = buffer.ReadShort();
					col.mPanicTicks = (int)buffer.ReadLong();
					col.mTick = (int)buffer.ReadLong();
					col.mLogicalColumn = (int)buffer.ReadLong();
					col.mAnimationDelay = (int)buffer.ReadLong();
					buffer.ReadCurvedVal(col.mDangerY);
					buffer.ReadCurvedVal(col.mDangerAlpha);
					buffer.ReadCurvedVal(col.mBottomFrostPct);
					buffer.ReadCurvedVal(col.mBumpY);
					buffer.ReadCurvedVal(col.mReprieveRampUp);
					col.mParent = this;
					col.mIceAnimation = string.Empty;
					col.mPanicAnimation = "blue";
					if (col.mFrostPanicAnim != null)
					{
						col.mFrostPanicAnim.Play("blue", true);
						col.mFrostPanicAnim.mTransform.LoadIdentity();
					}
				}
				int savedComboValueDisp;
				int savedComboStartTick;
				int savedComboStartUpdateTick;
				int savedComboDuration;
				int savedComboCount;
				buffer.ReadValuePair(out savedComboValueDisp);
				buffer.ReadValuePair(out savedComboStartTick);
				buffer.ReadValuePair(out savedComboStartUpdateTick);
				buffer.ReadValuePair(out savedComboDuration);
				buffer.ReadValuePair(out savedComboCount);
				mColCountData.mColComboValueDisp = savedComboValueDisp;
				mColCountData.mColComboStartTick = savedComboStartTick;
				mColCountData.mColComboStartUpdateTick = savedComboStartUpdateTick;
				mColCountData.mColComboDuration = savedComboDuration;
				mColCountData.mColComboCount = savedComboCount;
				buffer.ReadValuePair(mColCountData.mColComboAlpha);
				buffer.ReadValuePair(mColCountData.mColComboScale);
				buffer.ReadValuePair(mColCountData.mColComboY);
				buffer.ReadValuePair(out mColComboBonusPoints);
				buffer.ReadValuePair(out mColComboHighest);
				buffer.ReadValuePair(out mColClearBonusPoints);
				buffer.ReadValuePair(out mAnimUpdateCount);
				buffer.ReadValuePair(out mLoseColumn);
				buffer.ReadValuePair(out mLastIceRemoved);
				buffer.ReadValuePair(out mIceRemoved);
				buffer.ReadValuePair(out mStartDelay);
				int savedProgress;
				buffer.ReadValuePair(out savedProgress);
				mLevelProgress = savedProgress;
				int savedTotal;
				buffer.ReadValuePair(out savedTotal);
				mLevelProgressTotal = savedTotal;
				buffer.ReadValuePair(out mTotalLoseTicks);
				buffer.ReadValuePair(out mNextTryColStart);
				buffer.ReadValuePair(out mStageDuration);
				buffer.ReadValuePair(out mStageStartAtTick);
				buffer.ReadValuePair(out mShakeCooldown);
				buffer.ReadValuePair(mBackDim);
				base.LoadGameExtra(buffer);
				mCurveUpdateCount.value = mUpdateCnt;
				SyncProgress();
				RefreshIceToRemove();
			}
		}
	}
}
