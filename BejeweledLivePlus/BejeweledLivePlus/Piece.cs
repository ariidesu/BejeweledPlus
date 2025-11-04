using System;
using System.Collections.Generic;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus
{
	public class Piece : IDisposable
	{
		public Board mBoard;

		public int mId;

		public int mCol;

		public int mRow;

		public float mX;

		public float mY;

		public int mCreatedTick;

		public float mFallVelocity;

		public CurvedVal mScale = new CurvedVal();

		public CurvedVal mAlpha = new CurvedVal();

		public CurvedVal mSelectorAlpha = new CurvedVal();

		public float mRotPct;

		public bool mIsElectrocuting;

		public float mElectrocutePercent;

		public bool mDestructing;

		public bool mIsExploding;

		public bool mCanMatch;

		public bool mCanSwap;

		public bool mCanScramble;

		public bool mTallied;

		public bool mCanDestroy;

		public bool mIsBulging;

		public int mImmunityCount;

		public int mMoveCreditId;

		public int mLastMoveCreditId;

		public int mMatchId;

		public CurvedVal mDestPct = new CurvedVal();

		public int mDestCol;

		public int mDestRow;

		public int mChangedTick;

		public int mSwapTick;

		public int mLastActiveTick;

		public int mLastColor;

		public int mColor;

		public int mDestColor;

		public int mFlags;

		public int mDisallowFlags;

		public int mExplodeDelay;

		public int mExplodeSourceId;

		public int mExplodeSourceFlags;

		public bool mSelected;

		public float mHidePct;

		public int mCounter;

		public int mCounterDelay;

		public int mTimer;

		public int mTimerThreshold;

		public float mMoveCountdownPerLevel;

		public float mShakeOfsX;

		public float mShakeOfsY;

		public float mShakeTime;

		public float mShakeScale;

		public float mShakeAngle;

		public float mSpinFrame;

		public float mSpinSpeed;

		public float mDestSpinSpeed;

		public float mSpinSpeedHoldTime;

		public CurvedVal mHintAlpha = new CurvedVal();

		public CurvedVal mHintArrowPos = new CurvedVal();

		public MemoryImage mOverlay;

		public MemoryImage mOverlayGlow;

		public CurvedVal mOverlayCurve = new CurvedVal();

		public CurvedVal mOverlayBulge = new CurvedVal();

		public CurvedVal mAnimCurve = new CurvedVal();

		public float mFlyVX;

		public float mFlyVY;

		public float mFlyAY;

		public List<Effect> mBoundEffects = new List<Effect>();

		public bool mForceScreenY;

		public float mForcedY;

		private static SimpleObjectPool pool_ = new SimpleObjectPool(128, typeof(Piece));

		public static Piece alloc(Board board, int id)
		{
			Piece piece = (Piece)pool_.alloc();
			piece.mBoard = board;
			if (id < 0)
			{
				id = board.mNextPieceId++;
			}
			piece.Init(id);
			return piece;
		}

		public static Piece alloc(Board board)
		{
			return alloc(board, -1);
		}

		public void release()
		{
			ClearBoundEffects();
			if (mBoard != null)
			{
				mBoard.RemoveFromPieceMap(mId);
			}
			if (mOverlay != null)
			{
				mOverlay.Dispose();
			}
			if (mOverlayGlow != null)
			{
				mOverlayGlow.Dispose();
			}
			pool_.release(this);
		}

		public Piece()
		{
		}

		public Piece(Board theBoard)
		{
			mBoard = theBoard;
			Init(mBoard.mNextPieceId++);
		}

		public Piece(Board theBoard, int theId)
		{
			mBoard = theBoard;
			Init(theId);
		}

		public void Dispose()
		{
			ClearBoundEffects();
			if (mBoard != null)
			{
				mBoard.RemoveFromPieceMap(mId);
			}
			if (mOverlay != null)
			{
				mOverlay.Dispose();
			}
			if (mOverlayGlow != null)
			{
				mOverlayGlow.Dispose();
			}
		}

		public void Init(int theId)
		{
			mCreatedTick = 0;
			mFlags = 0;
			mDisallowFlags = 0;
			mColor = -1;
			mDestColor = -1;
			mExplodeDelay = 0;
			mExplodeSourceId = -1;
			mExplodeSourceFlags = 0;
			mLastActiveTick = 0;
			mChangedTick = 0;
			mLastColor = -1;
			mSwapTick = -1;
			mIsElectrocuting = false;
			mElectrocutePercent = 0f;
			mDestructing = false;
			mIsExploding = false;
			mCanMatch = true;
			mCanSwap = true;
			mCanScramble = true;
			mTallied = false;
			mCanDestroy = true;
			mIsBulging = false;
			mImmunityCount = 0;
			mMoveCreditId = -1;
			mLastMoveCreditId = -1;
			mMatchId = -1;
			mX = 0f;
			mY = 0f;
			mCol = 0;
			mRow = 0;
			mFallVelocity = 0f;
			mDestCol = 0;
			mDestRow = 0;
			mAlpha.SetConstant(1.0);
			mRotPct = 0f;
			mSelected = false;
			mHidePct = 0f;
			mScale.SetConstant(1.0);
			mCounter = 0;
			mCounterDelay = 0;
			mTimer = 0;
			mTimerThreshold = 100;
			mShakeOfsX = 0f;
			mShakeOfsY = 0f;
			mShakeScale = 0f;
			mShakeAngle = 0f;
			mShakeTime = 0f;
			mSpinFrame = 0f;
			mSpinSpeed = 0f;
			mDestSpinSpeed = 0f;
			mSpinSpeedHoldTime = 0f;
			mOverlay = (mOverlayGlow = null);
			mFlyVX = 0f;
			mFlyVY = 0f;
			mFlyAY = 0f;
			mForceScreenY = false;
			mForcedY = 0f;
			mSelectorAlpha = new CurvedVal();
			mDestPct = new CurvedVal();
			mId = theId;
			mBoard.AddToPieceMap(mId, this);
		}

		public Piece CopyForm(Piece obj)
		{
			if (obj != null)
			{
				mBoard = obj.mBoard;
				mId = obj.mId;
				mCol = obj.mCol;
				mRow = obj.mRow;
				mX = obj.mX;
				mY = obj.mY;
				mCreatedTick = obj.mCreatedTick;
				mFallVelocity = obj.mFallVelocity;
				mRotPct = obj.mRotPct;
				mIsElectrocuting = obj.mIsElectrocuting;
				mElectrocutePercent = obj.mElectrocutePercent;
				mDestructing = obj.mDestructing;
				mIsExploding = obj.mIsExploding;
				mCanMatch = obj.mCanMatch;
				mCanSwap = obj.mCanMatch;
				mCanScramble = obj.mCanScramble;
				mTallied = obj.mTallied;
				mCanDestroy = obj.mCanDestroy;
				mIsBulging = obj.mIsBulging;
				mImmunityCount = obj.mImmunityCount;
				mMoveCreditId = obj.mMoveCreditId;
				mLastMoveCreditId = obj.mLastMoveCreditId;
				mMatchId = obj.mMatchId;
				mDestCol = obj.mDestCol;
				mDestRow = obj.mDestRow;
				mChangedTick = obj.mChangedTick;
				mSwapTick = obj.mSwapTick;
				mLastActiveTick = obj.mLastActiveTick;
				mLastColor = obj.mLastColor;
				mColor = obj.mColor;
				mDestColor = obj.mDestColor;
				mFlags = obj.mFlags;
				mDisallowFlags = obj.mDisallowFlags;
				mExplodeDelay = obj.mExplodeDelay;
				mExplodeSourceId = obj.mExplodeSourceId;
				mExplodeSourceFlags = obj.mExplodeSourceFlags;
				mSelected = obj.mSelected;
				mHidePct = obj.mHidePct;
				mCounter = obj.mCounter;
				mCounterDelay = obj.mCounterDelay;
				mTimer = obj.mTimer;
				mTimerThreshold = obj.mTimerThreshold;
				mMoveCountdownPerLevel = obj.mMoveCountdownPerLevel;
				mShakeOfsX = obj.mShakeOfsX;
				mShakeOfsY = obj.mShakeOfsY;
				mShakeTime = obj.mShakeTime;
				mShakeScale = obj.mShakeScale;
				mShakeAngle = obj.mShakeAngle;
				mSpinFrame = obj.mSpinFrame;
				mSpinSpeed = obj.mSpinSpeed;
				mDestSpinSpeed = obj.mDestSpinSpeed;
				mSpinSpeedHoldTime = obj.mSpinSpeedHoldTime;
				mFlyVX = obj.mFlyAY;
				mFlyVY = obj.mFlyVX;
				mFlyAY = obj.mFlyAY;
				mForceScreenY = obj.mForceScreenY;
				mForcedY = obj.mForcedY;
			}
			return this;
		}

		public bool IsShrinking()
		{
			return mScale.mRamp == 6;
		}

		public bool IsButton()
		{
			return IsFlagSet(6144u);
		}

		public void BindEffect(Effect theEffect)
		{
			theEffect.mPieceRel = this;
			theEffect.mRefCount++;
			mBoundEffects.Add(theEffect);
		}

		public void ClearBoundEffects()
		{
			while (mBoundEffects.Count > 0)
			{
				mBoundEffects[mBoundEffects.Count - 1].mPieceRel = null;
				mBoundEffects[mBoundEffects.Count - 1].mRefCount--;
				mBoundEffects.RemoveAt(mBoundEffects.Count - 1);
			}
		}

		public void ClearHyperspaceEffects()
		{
			for (int i = 0; i < mBoundEffects.Count; i++)
			{
				Effect effect = mBoundEffects[i];
				if ((effect.mFlags & 8) != 0)
				{
					effect.mPieceRel = null;
					effect.mRefCount--;
					mBoundEffects.RemoveAt(i);
					i--;
				}
			}
		}

		public void CancelMatch()
		{
			mScale.SetConstant(1.0);
		}

		public float CX()
		{
			return GetScreenX() + 50f;
		}

		public float CY()
		{
			return GetScreenY() + 50f;
		}

		public float GetScreenX()
		{
			return mX + (float)mBoard.GetBoardX();
		}

		public float GetScreenY()
		{
			if (!mForceScreenY)
			{
				return mY + (float)mBoard.GetBoardY();
			}
			return mForcedY + (float)mBoard.GetBoardY();
		}

		public int FindRowFromY()
		{
			return mBoard.GetRowAt((int)mY);
		}

		public int FindColFromX()
		{
			return mBoard.GetColAt((int)mX);
		}

		public static float GetAngleRadius(float theAngle, int theColor, int theFrame)
		{
			while (theAngle >= (float)Math.PI * 2f)
			{
				theAngle -= (float)Math.PI * 2f;
			}
			while (theAngle < 0f)
			{
				theAngle += (float)Math.PI * 2f;
			}
			float num = 256f * theAngle / ((float)Math.PI * 2f);
			int num2 = (int)num;
			if (theColor < 0)
			{
				theColor = 0;
			}
			if (num2 < 0)
			{
				return GlobalMembers.GEM_OUTLINE_RADIUS_POINTS[theFrame, theColor, 0] * ConstantsWP.GEM_PRE_SCALE_FACTOR;
			}
			if (num2 >= 255)
			{
				return GlobalMembers.GEM_OUTLINE_RADIUS_POINTS[theFrame, theColor, 255] * ConstantsWP.GEM_PRE_SCALE_FACTOR;
			}
			float num3 = num - (float)num2;
			if (theColor < 0)
			{
				theColor = 0;
			}
			return GlobalMembers.GEM_OUTLINE_RADIUS_POINTS[theFrame, theColor, num2] * ConstantsWP.GEM_PRE_SCALE_FACTOR * (1f - num3) + GlobalMembers.GEM_OUTLINE_RADIUS_POINTS[theFrame, theColor, num2 + 1] * ConstantsWP.GEM_PRE_SCALE_FACTOR * num3;
		}

		public float GetAngleRadius(float theAngle)
		{
			int theFrame = (int)Math.Min(19f, 20f * mRotPct);
			return GetAngleRadius(theAngle, mColor, theFrame);
		}

		public void Save(Serialiser theBuffer)
		{
			theBuffer.WriteFloat(mX);
			theBuffer.WriteFloat(mY);
			theBuffer.WriteInt32(mCreatedTick);
			theBuffer.WriteFloat(mFallVelocity);
			theBuffer.WriteCurvedVal(mScale);
			theBuffer.WriteCurvedVal(mAlpha);
			theBuffer.WriteCurvedVal(mSelectorAlpha);
			theBuffer.WriteFloat(mRotPct);
			theBuffer.WriteBoolean(mDestructing);
			theBuffer.WriteBoolean(mIsExploding);
			theBuffer.WriteBoolean(mCanMatch);
			theBuffer.WriteBoolean(mCanScramble);
			theBuffer.WriteBoolean(mTallied);
			theBuffer.WriteBoolean(mCanDestroy);
			theBuffer.WriteBoolean(mIsBulging);
			theBuffer.WriteBoolean(mCanSwap);
			theBuffer.WriteInt32(mImmunityCount);
			theBuffer.WriteInt32(mMoveCreditId);
			theBuffer.WriteInt32(mLastMoveCreditId);
			theBuffer.WriteInt32(mMatchId);
			theBuffer.WriteCurvedVal(mDestPct);
			theBuffer.WriteInt32(mDestCol);
			theBuffer.WriteInt32(mDestRow);
			theBuffer.WriteInt32(mChangedTick);
			theBuffer.WriteInt32(mSwapTick);
			theBuffer.WriteInt32(mLastActiveTick);
			theBuffer.WriteInt32(mLastColor);
			theBuffer.WriteInt32(mColor);
			theBuffer.WriteInt32(mFlags);
			theBuffer.WriteInt32(mExplodeDelay);
			theBuffer.WriteInt32(mExplodeSourceId);
			theBuffer.WriteInt32(mExplodeSourceFlags);
			theBuffer.WriteBoolean(mSelected);
			theBuffer.WriteInt32(mCounter);
			theBuffer.WriteInt32(mCounterDelay);
			theBuffer.WriteFloat(mMoveCountdownPerLevel);
			theBuffer.WriteFloat(mShakeOfsX);
			theBuffer.WriteFloat(mShakeOfsY);
			theBuffer.WriteFloat(mShakeTime);
			theBuffer.WriteFloat(mShakeScale);
			theBuffer.WriteFloat(mShakeAngle);
			theBuffer.WriteFloat(mSpinFrame);
			theBuffer.WriteFloat(mSpinSpeed);
			theBuffer.WriteFloat(mDestSpinSpeed);
			theBuffer.WriteFloat(mSpinSpeedHoldTime);
		}

		public void Load(Serialiser theBuffer, int Version)
		{
			mX = theBuffer.ReadFloat();
			mY = theBuffer.ReadFloat();
			mCreatedTick = theBuffer.ReadInt32();
			mFallVelocity = theBuffer.ReadFloat();
			mScale = theBuffer.ReadCurvedVal();
			mAlpha = theBuffer.ReadCurvedVal();
			mSelectorAlpha = theBuffer.ReadCurvedVal();
			mRotPct = theBuffer.ReadFloat();
			mDestructing = theBuffer.ReadBoolean();
			mIsExploding = theBuffer.ReadBoolean();
			mCanMatch = theBuffer.ReadBoolean();
			mCanScramble = theBuffer.ReadBoolean();
			mTallied = theBuffer.ReadBoolean();
			mCanDestroy = theBuffer.ReadBoolean();
			mIsBulging = theBuffer.ReadBoolean();
			mCanSwap = theBuffer.ReadBoolean();
			mImmunityCount = theBuffer.ReadInt32();
			mMoveCreditId = theBuffer.ReadInt32();
			mLastMoveCreditId = theBuffer.ReadInt32();
			mMatchId = theBuffer.ReadInt32();
			mDestPct = theBuffer.ReadCurvedVal();
			mDestCol = theBuffer.ReadInt32();
			mDestRow = theBuffer.ReadInt32();
			mChangedTick = theBuffer.ReadInt32();
			mSwapTick = theBuffer.ReadInt32();
			mLastActiveTick = theBuffer.ReadInt32();
			mLastColor = theBuffer.ReadInt32();
			mColor = theBuffer.ReadInt32();
			mFlags = theBuffer.ReadInt32();
			mExplodeDelay = theBuffer.ReadInt32();
			mExplodeSourceId = theBuffer.ReadInt32();
			mExplodeSourceFlags = theBuffer.ReadInt32();
			mSelected = theBuffer.ReadBoolean();
			mCounter = theBuffer.ReadInt32();
			mCounterDelay = theBuffer.ReadInt32();
			if (Version > 101)
			{
				mMoveCountdownPerLevel = theBuffer.ReadFloat();
			}
			mShakeOfsX = theBuffer.ReadFloat();
			mShakeOfsY = theBuffer.ReadFloat();
			mShakeTime = theBuffer.ReadFloat();
			mShakeScale = theBuffer.ReadFloat();
			mShakeAngle = theBuffer.ReadFloat();
			mSpinFrame = theBuffer.ReadFloat();
			mSpinSpeed = theBuffer.ReadFloat();
			mDestSpinSpeed = theBuffer.ReadFloat();
			mSpinSpeedHoldTime = theBuffer.ReadFloat();
		}

		public void Update()
		{
			mOverlayBulge.IncInVal();
			mOverlayCurve.IncInVal();
			if (mShakeScale > 0f)
			{
				mShakeAngle = (float)Math.PI * GlobalMembersUtils.GetRandFloat();
				mShakeOfsX = (float)Math.Cos(mShakeAngle) * mShakeScale * 100f / 20f;
				mShakeOfsY = (float)Math.Sin(mShakeAngle) * mShakeScale * 100f / 20f;
			}
			else
			{
				mShakeOfsX = (mShakeOfsY = 0f);
			}
			float mElectrocutePercent2 = mElectrocutePercent;
			float num19 = 0f;
			if (IsFlagSet(128u) && !mBoard.IsPieceSwapping(this) && (mId * 10 + mBoard.mUpdateCnt) % 400 == 0)
			{
				switch (BejeweledLivePlus.Misc.Common.Rand(3))
				{
				case 0:
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ANIM_CURVE_C, mAnimCurve);
					break;
				case 1:
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ANIM_CURVE_D, mAnimCurve);
					break;
				case 2:
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ANIM_CURVE_E, mAnimCurve);
					break;
				}
			}
			int val = 40;
			if (IsFlagSet(1u))
			{
				bool flag = mBoard.WantsCalmEffects();
				// int num = 1;
				// num = ((((int)GlobalMembers.gApp.mUpdateCount + mId) % GlobalMembers.M(3) == 0) ? 1 : 0);
				for (int i = 0; i < 4; i++)
				{
					bool flag2 = BejeweledLivePlus.Misc.Common.Rand() % 5 != 0;
					EffectsManager effectsManager;
					Effect effect;
					if (flag2)
					{
						effectsManager = ((BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(32) == 0) ? mBoard.mPostFXManager : mBoard.mPreFXManager);
						effect = effectsManager.AllocEffect(Effect.Type.TYPE_EMBER_FADEINOUT);
						effect.mAngle = 0f;
						effect.mDAngle = 0f;
						effect.mAlpha = 0f;
						effect.mDAlpha = GlobalMembers.M(0.0075f) + GlobalMembers.M(0.0015f) * GlobalMembersUtils.GetRandFloat();
						if (SexyFramework.GlobalMembers.gIs3D)
						{
							effect.mScale = GlobalMembers.M(0.12f) + GlobalMembers.M(0.035f) * GlobalMembersUtils.GetRandFloat();
							effect.mDScale = GlobalMembers.M(0.01f) + GlobalMembers.M(0.005f) * GlobalMembersUtils.GetRandFloat();
						}
						effect.mDY = GlobalMembers.M(-0.12f) + GlobalMembers.M(-0.05f) * GlobalMembersUtils.GetRandFloat();
						if (flag)
						{
							effect.mDScale *= GlobalMembers.M(0.67f);
						}
						bool flag3 = mBoard.mWantsReddishFlamegems && BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(4) <= GlobalMembers.M(0) && effectsManager == mBoard.mPreFXManager;
						if (mColor == 3 || flag3)
						{
							effect.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(128), GlobalMembers.M(128));
							if (flag3)
							{
								effect.mType = Effect.Type.TYPE_EMBER_FADEINOUT_BOTTOM;
							}
						}
						else
						{
							effect.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255));
						}
					}
					else
					{
						effectsManager = ((BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(2) == 0) ? mBoard.mPostFXManager : mBoard.mPreFXManager);
						effect = effectsManager.AllocEffect(Effect.Type.TYPE_EMBER);
						effect.mAlpha = 1f;
						if (SexyFramework.GlobalMembers.gIs3D)
						{
							effect.mScale = GlobalMembers.M(2f);
						}
						effect.mDScale = GlobalMembers.M(-0.01f);
						effect.mFrame = 0;
						if (SexyFramework.GlobalMembers.gIs3D)
						{
							effect.mImage = GlobalMembersResourcesWP.IMAGE_SPARKLET;
						}
						else
						{
							effect.mImage = GlobalMembersResourcesWP.IMAGE_SPARKLET_FAT;
						}
						effect.mDY = (GlobalMembers.M(-0.4f) +
						              GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.15f));
						effect.mColor = new Color(GlobalMembers.M(128), BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(32) + GlobalMembers.M(48), BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(24) + GlobalMembers.M(24));
						bool flag4 = mBoard.mWantsReddishFlamegems && BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(3) <= GlobalMembers.M(0) && effectsManager == mBoard.mPreFXManager;
						if (mColor == 3 || flag4)
						{
							effect.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(0), GlobalMembers.M(0));
							if (flag4)
							{
								effect.mType = Effect.Type.TYPE_EMBER_BOTTOM;
							}
						}
						else if (mColor == 0)
						{
							effect.mColor = new Color(GlobalMembers.M(240), GlobalMembers.M(128), GlobalMembers.M(64));
						}
					}
					if (flag)
					{
						effect.mDY *= GlobalMembers.M(0.67f);
						effect.mDAlpha *= GlobalMembers.M(0.67f);
					}
					float num2 = (float)Math.PI + Math.Abs(GlobalMembersUtils.GetRandFloat() * (float)Math.PI * 2f);
					float angleRadius = GetAngleRadius(num2);
					if (flag2 && BejeweledLivePlus.Misc.Common.Rand() % 2 != 0)
					{
						float num3 = (float)Math.Sin(num2);
						if (num3 > 0f)
						{
							float num4 = (float)Math.Cos(num2);
							float num5 = ((!(num4 < 0f)) ? GlobalMembers.M(-0.001f) : GlobalMembers.M(0.001f));
							float num6 = ((float)Math.Cos(num2 + num5) + (float)Math.Cos(num2 + num5 * 2f)) / 2f;
							float num7 = ((float)Math.Sin(num2 + num5) + (float)Math.Sin(num2 + num5 * 2f)) / 2f;
							float num8 = (float)Math.Atan2(num7 - num3, num6 - num4);
							float num9 = GlobalMembers.M(0.12f) + GlobalMembers.M(0.05f) * GlobalMembersUtils.GetRandFloat();
							float num10 = (float)Math.Cos(num8) * num9;
							float num11 = (float)Math.Sin(num8) * num9;
							effect.mDX = (effect.mDX + num10) / 2f;
							effect.mDY = (effect.mDY + num11) / 2f;
						}
					}
					float num12 = GetScreenX();
					if (effectsManager == mBoard.mPostFXManager)
					{
						num12 += (float)(double)mBoard.mSideXOff;
					}
					effect.mX = num12 + 50f + (float)Math.Cos(num2) * angleRadius;
					effect.mY = GetScreenY() + 50f + (float)Math.Sin(num2) * angleRadius + GlobalMembers.M(2f);
					if (BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(6) != 0 || mBoard.mHyperspace != null)
					{
						effect.mX -= num12;
						effect.mY -= GetScreenY();
						effect.mPieceRel = this;
					}
					effectsManager.AddEffect(effect);
				}
				if (((int)GlobalMembers.gApp.mUpdateCount + mId) % GlobalMembers.M(val) == 0 || BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(40) == 0)
				{
					Effect effect2 = mBoard.mPostFXManager.AllocEffect(Effect.Type.TYPE_LIGHT);
					effect2.mFlags = 2;
					effect2.mX = CX() + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(20f);
					effect2.mY = CY() + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(20f) + GlobalMembers.M(-5f);
					effect2.mZ = GlobalMembers.M(0.08f);
					effect2.mValue[0] = GlobalMembers.M(30f);
					effect2.mValue[1] = GlobalMembers.M(-0.1f);
					effect2.mAlpha = GlobalMembers.M(0f);
					effect2.mDAlpha = GlobalMembers.M(0.1f);
					effect2.mScale = GlobalMembers.M(140f);
					effect2.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(140), GlobalMembers.M(48));
					if (BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(2) != 0 || mBoard.mHyperspace != null)
					{
						effect2.mPieceId = (uint)mId;
					}
					mBoard.mPostFXManager.AddEffect(effect2);
				}
			}
			if (IsFlagSet(2u) && (((int)GlobalMembers.gApp.mUpdateCount + mId) % GlobalMembers.M(val) == 0 || BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(40) == 0))
			{
				Effect effect3 = mBoard.mPostFXManager.AllocEffect(Effect.Type.TYPE_LIGHT);
				effect3.mFlags = 2;
				effect3.mX = CX() + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(30f);
				effect3.mY = CY() + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(30f) + GlobalMembers.M(0f);
				effect3.mZ = GlobalMembers.M(0.08f);
				effect3.mValue[0] = GlobalMembers.M(30f);
				effect3.mValue[1] = GlobalMembers.M(-0.3f);
				effect3.mAlpha = GlobalMembers.M(0f);
				effect3.mDAlpha = GlobalMembers.M(0.07f);
				effect3.mScale = GlobalMembers.M(140f);
				effect3.mColor = new Color((int)GlobalMembers.gApp.HSLToRGB(BejeweledLivePlus.Misc.Common.Rand() % 255, GlobalMembers.M(255), GlobalMembers.M(128)));
				if (BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(2) != 0)
				{
					effect3.mPieceId = (uint)mId;
				}
				mBoard.mPostFXManager.AddEffect(effect3);
			}
			if (IsFlagSet(512u))
			{
				if (mOverlay == null)
				{
					StampOverlay();
				}
				if (mCounter <= 8)
				{
					float num13 = Math.Max(1, 8 - mCounter);
					int num14 = BejeweledLivePlus.Misc.Common.Rand() % 4;
					int num15 = Math.Max(GlobalMembers.M(2), mCounter * GlobalMembers.M(1));
					int num16 = 0;
					if (num15 < GlobalMembers.M(20) && mBoard.mUpdateCnt % num15 == 0)
					{
						num16 = GlobalMembers.M(1);
					}
					while (num16-- != 0)
					{
						Effect effect4 = mBoard.mPostFXManager.AllocEffect(Effect.Type.TYPE_STEAM);
						float num17 = (GlobalMembers.M(0.25f) + GlobalMembers.M(0.05f) * Math.Abs(GlobalMembersUtils.GetRandFloat())) * num13;
						float num18 = (float)Math.PI / 4f + (float)num14 * (float)Math.PI / 2f;
						effect4.mX = CX() + mShakeOfsX + (float)Math.Cos(num18) * 100f * (GlobalMembers.M(0.6f) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.1f));
						effect4.mY = CY() + mShakeOfsY + (float)Math.Sin(num18) * 100f * (GlobalMembers.M(0.6f) + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.1f));
						effect4.mIsAdditive = false;
						effect4.mScale = GlobalMembers.M(0.1f);
						effect4.mDScale = GlobalMembers.M(0.03f);
						effect4.mMaxScale = GlobalMembers.M(2f);
						effect4.mDX = num17 * (float)Math.Cos(num18);
						effect4.mDY = num17 * (float)Math.Sin(num18);
						mBoard.mPostFXManager.AddEffect(effect4);
					}
				}
			}
			for (int j = 0; j < mBoundEffects.Count; j++)
			{
				Effect effect5 = mBoundEffects[j];
				if (effect5.mDeleteMe)
				{
					effect5.mRefCount--;
					mBoundEffects.RemoveAt(j);
					j--;
				}
			}
		}

		public bool SetFlag(uint theFlag)
		{
			if ((theFlag & (uint)mDisallowFlags) != 0)
			{
				return false;
			}
			mFlags |= (int)theFlag;
			return true;
		}

		public bool CanSetFlag(uint theFlag)
		{
			return (theFlag & mDisallowFlags) == 0;
		}

		public bool IsFlagSet(uint theFlag)
		{
			return (mFlags & theFlag) != 0;
		}

		public void SetDisallowFlag(uint theFlag)
		{
			SetDisallowFlag(theFlag, true);
		}

		public void SetDisallowFlag(uint theFlag, bool theValue)
		{
			if (theValue)
			{
				mDisallowFlags |= (int)theFlag;
			}
			else
			{
				mDisallowFlags &= (int)(~theFlag);
			}
			mFlags &= ~mDisallowFlags;
		}

		public void ClearDisallowFlag(uint theFlag)
		{
			mDisallowFlags &= (int)(~theFlag);
			mFlags &= ~mDisallowFlags;
		}

		public void ClearDisallowFlags()
		{
			mDisallowFlags = 0;
		}

		public void ClearFlag(uint theFlag)
		{
			mFlags &= (int)(~theFlag);
		}

		public void ClearFlags()
		{
			mFlags = 0;
		}

		public void StampOverlay()
		{
			string theString = $"{mCounter}";
			ImageFont imageFont = (ImageFont)GlobalMembersResources.FONT_SCORE;
			Utils.SetFontLayerColor(imageFont, "Layer_3", new Color(0, 0, 0, 128));
			int num = (int)((float)imageFont.StringWidth(theString) * GlobalMembers.M(1.5f));
			int num2 = Math.Max(GlobalMembers.S(100), imageFont.mHeight - imageFont.mAscent) + GlobalMembers.S(18);
			if (mOverlay == null)
			{
				mOverlay = new MemoryImage();
				mOverlay.SetImageMode(true, true);
				mOverlay.ReplaceImageFlags(128u);
			}
			if (mOverlayGlow == null)
			{
				mOverlayGlow = new MemoryImage();
				mOverlayGlow.SetImageMode(true, true);
				mOverlayGlow.ReplaceImageFlags(128u);
			}
			if (num > mOverlay.mWidth || num2 > mOverlay.mHeight)
			{
				mOverlay.Create(num, num2);
				mOverlayGlow.Create(num, num2);
			}
			Graphics graphics = new Graphics(mOverlay);
			graphics.SetColorizeImages(true);
			Color color = default(Color);
			color = GlobalMembers.gGemColors[mColor];
			color.mRed /= 2;
			color.mGreen /= 2;
			color.mBlue /= 2;
			graphics.SetFont(imageFont);
			imageFont.PushLayerColor(0, Color.Black);
			imageFont.PushLayerColor(1, color);
			imageFont.PushLayerColor(2, Color.White);
			imageFont.PushLayerColor(3, Color.White);
			int theX = (mOverlay.mWidth - graphics.StringWidth(theString)) / 2 + GlobalMembers.MS(0);
			int theY = mOverlay.mHeight / 2 + graphics.GetFont().mHeight - graphics.GetFont().mAscent + GlobalMembers.MS(28);
			graphics.SetColor(new Color(255, 255, 255));
			graphics.DrawString(theString, theX, theY);
			imageFont.PopLayerColor(0);
			imageFont.PopLayerColor(1);
			imageFont.PopLayerColor(2);
			imageFont.PopLayerColor(3);
			imageFont.PushLayerColor(0, Color.Black);
			imageFont.PushLayerColor(1, Color.White);
			imageFont.PushLayerColor(2, new Color(255, 0, 0));
			imageFont.PushLayerColor(3, new Color(255, 0, 0));
			Graphics graphics2 = new Graphics(mOverlayGlow);
			graphics2.SetColorizeImages(true);
			graphics2.SetFont(imageFont);
			graphics2.SetColor(new Color(255, 255, 255));
			graphics2.DrawString(theString, theX, theY);
			imageFont.PopLayerColor(0);
			imageFont.PopLayerColor(1);
			imageFont.PopLayerColor(2);
			imageFont.PopLayerColor(3);
			if (mCounter <= 8)
			{
				float num3 = Math.Max(1, 8 - mCounter);
				mShakeScale = num3 * GlobalMembers.M(0.15f);
				if (mOverlayCurve.mIncRate == 0.0)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_OVERLAY_CURVE, mOverlayCurve);
					mOverlayCurve.mMode = 1;
				}
			}
			else
			{
				mOverlayCurve.mInVal = 0.0;
				mOverlayCurve.mIncRate = 0.0;
			}
		}
	}
}
