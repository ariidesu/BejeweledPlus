using System;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class ButterflyBoard : QuestBoard
	{
		public const int gGiantSpiderAppearingOffsY = -500;

		public int mDefaultMoveCountdown;

		public int mDefaultDropCountdown;

		public int mGrabbedAt;

		public bool mPlayedGrabSound;

		public int mDropCountdown;

		public int mSpawnCountStart;

		public bool mQueueSpawn;

		public float mDropCountdownPerLevel;

		public float mMoveCountdownPerLevel;

		public float mSpawnCountMax;

		public float mSpawnCountPerLevel;

		public float mSpawnCountAcc;

		public float mSideSpawnChance;

		public float mSideSpawnChancePerLevel;

		public float mSideSpawnChanceMax;

		public float mSpiderCol;

		public float mSpiderWalkColFrom;

		public float mSpiderWalkColTo;

		public float mSpawnCount;

		public bool mAllowNewComboFloaters;

		public CurvedVal mSpiderWalkPct = new CurvedVal();

		public ButterflyEffect mLastButterflyEffect;

		public PopAnim mSpider;

		public PopAnim mBiggerSpider;

		public bool mSpiderRandomBehaviour;

		public int mSpiderRandomState;

		public float mStartSpiderCol;

		public int mGrabCounter;

		public int mCurrentReleasedButterflies;

		public bool mSpotOnSpider;

		public bool mCountingForGameOver;

		public int mGameOverCountdown;

		public float mSpiderYOffset;

		public bool mSpiderDownAfterGrab;

		public ButterflyBoard()
		{
			mSpiderRandomBehaviour = false;
			mSpider = null;
			mLastButterflyEffect = null;
			mCurrentReleasedButterflies = 0;
			mSpotOnSpider = false;
			mCountingForGameOver = false;
			mGameOverCountdown = 0;
			mSpiderYOffset = 0f;
			mSpiderDownAfterGrab = true;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override int FindSets(bool fromUpdateSwap, Piece thePiece1, Piece thePiece2)
		{
			int num = 0;
			mForceReleaseButterfly = false;
			mForcedReleasedBflyPiece = null;
			num = base.FindSets(fromUpdateSwap, thePiece1, thePiece2);
			if (mForceReleaseButterfly && mForcedReleasedBflyPiece != null)
			{
				mGameStats[28]++;
				mLastButterflyEffect = ButterflyEffect.alloc(mForcedReleasedBflyPiece, this);
				if (!mIsPerpetual)
				{
					mLastButterflyEffect.mTargetY.mOutMin = GlobalMembers.M(590);
				}
				mPostFXManager.AddEffect(mLastButterflyEffect);
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTERFLYESCAPE);
			}
			return num;
		}

		public void OnButterflyMatched(Piece thePiece)
		{
			int moveStat = GetMoveStat(thePiece.mMoveCreditId, 28);
			MaxStat(29, moveStat, thePiece.mMoveCreditId);
			mAllowNewComboFloaters = true;
			AddPoints((int)thePiece.CX(), (int)thePiece.CY(), GlobalMembers.M(150) + GlobalMembers.M(5) * mGameStats[28] + (moveStat - 1) * GlobalMembers.M(100), Color.White, (uint)thePiece.mMatchId, true, true, thePiece.mMoveCreditId, false);
			mAllowNewComboFloaters = false;
			ReleaseButterfly(thePiece);
			if (GetLevelPointsTotal() < GetLevelPoints() || mIsPerpetual)
			{
				return;
			}
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.IsFlagSet(128u))
				{
					piece.mCounter++;
				}
			}
		}

		public void DoSpiderRandomMove()
		{
			if (mSpiderRandomState == 0)
			{
				if (mSpiderCol < mStartSpiderCol + 0.25f)
				{
					mSpiderCol += 0.013f;
				}
				else
				{
					mSpiderRandomState = 1;
				}
			}
			else if (mSpiderRandomState == 1)
			{
				if (mSpiderCol > mStartSpiderCol - 0.25f)
				{
					mSpiderCol -= 0.013f;
				}
				else
				{
					mSpiderRandomState = 2;
				}
			}
			else if (mSpiderRandomState == 2)
			{
				if (mSpiderCol < mStartSpiderCol)
				{
					mSpiderCol += 0.013f;
					return;
				}
				mSpiderRandomBehaviour = false;
				mSpider.Play("IDLE");
			}
			else if (mSpiderRandomState == 3 && --mGrabCounter == 0)
			{
				mSpiderRandomBehaviour = false;
				mSpider.Play("IDLE");
			}
		}

		public override int GetGameOverCountTreshold()
		{
			return GlobalMembers.M(250);
		}

		public override void GameOverAnnounce()
		{
			Announcement announcement = new Announcement(this, GlobalMembers._ID("GAME OVER", 95));
			announcement.mDarkenBoard = false;
			GlobalMembers.gApp.PlayVoice(GlobalMembersResourcesWP.SOUND_VOICE_GAMEOVER);
		}

		public override void UnloadContent()
		{
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_Butterfly");
			BejeweledLivePlusApp.UnloadContent("GamePlay_UI_Normal");
			if (mSpider != null && mSpider != null)
			{
				mSpider.Dispose();
			}
			if (mBiggerSpider != null && mBiggerSpider != null)
			{
				mBiggerSpider.Dispose();
			}
			base.UnloadContent();
		}

		public override void LoadContent(bool threaded)
		{
			if (threaded)
			{
				BejeweledLivePlusApp.LoadContentInBackground("GamePlay_UI_Normal");
			}
			else
			{
				BejeweledLivePlusApp.LoadContent("GamePlay_UI_Normal");
			}
			BejeweledLivePlusApp.LoadContent("GamePlayQuest_Butterfly");
			mLastButterflyEffect = null;
			mSpider = GlobalMembersResourcesWP.POPANIM_ANIMS_SPIDER.Duplicate();
			mBiggerSpider = GlobalMembersResourcesWP.POPANIM_ANIMS_LARGE_SPIDER.Duplicate();
			base.LoadContent(threaded);
		}

		public override void RefreshUI()
		{
			mHintButton.SetBorderGlow(true);
			mHintButton.Resize(ConstantsWP.BOARD_UI_HINT_BTN_X, ConstantsWP.BUTTERFLYBOARD_HINT_BTN_Y, ConstantsWP.BOARD_UI_HINT_BTN_WIDTH, 0);
			mHintButton.mHasAlpha = true;
			mHintButton.mDoFinger = true;
			mHintButton.mOverAlphaSpeed = 0.1;
			mHintButton.mOverAlphaFadeInSpeed = 0.2;
			mHintButton.mWidgetFlagsMod.mRemoveFlags |= 4;
			mHintButton.mDisabled = false;
			mHintButton.SetOverlayType(Bej3Button.BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_NONE);
		}

		public override void DrawUI(Graphics g)
		{
			DrawTopFrame(g);
			DrawBottomFrame(g);
		}

		public override void DrawWarningHUD(Graphics g)
		{
		}

		public override void DrawHUDText(Graphics g)
		{
		}

		public override void DrawBottomFrame(Graphics g)
		{
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BUTTERFLY, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BUTTERFLY_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BUTTERFLY_ID)));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_BG, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_BG_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_BG_ID)));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)));
			if (WantWarningGlow())
			{
				g.PushState();
				g.SetColor(GetWarningGlowColor());
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BUTTERFLY, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BUTTERFLY_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BUTTERFLY_ID)));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_BOTTOM_ID)));
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_SCORE_FRAME_ID)));
				g.PopState();
			}
		}

		public override void DrawTopFrame(Graphics g)
		{
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_WEB, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_WEB_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_WEB_ID)));
			g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP_ID)));
			if (WantWarningGlow())
			{
				g.PushState();
				g.SetColor(GetWarningGlowColor());
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP_ID)), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_INGAMEUI_BUTTERFLIES_BOARD_SEPERATOR_FRAME_TOP_ID)));
				g.SetDrawMode(Graphics.DrawMode.Normal);
				g.PopState();
			}
		}

		public override void DrawHUD(Graphics g)
		{
		}

		public override void PlayMenuMusic(bool isRestart = false)
		{
			CustomBassMusicInterface theMusicInterface =
				(CustomBassMusicInterface)(GlobalMembers.gApp.mMusicInterface);
			if (isRestart || theMusicInterface.mSongName != GetMusicName())
			{
				theMusicInterface.QueueEvent("FadeOut", theMusicInterface.mSongName, false);
				theMusicInterface.QueueEvent("Play", GetMusicName(), true);
			}
			// GlobalMembers.gApp.mMusic.PlaySongNoDelay(5, true);
		}

		public override void SetupBackground(int theDeltaIdx)
		{
			string empty = string.Empty;
			empty = $"images\\{GlobalMembers.gApp.mArtRes}\\backgrounds\\lion_tower_cascade_bfly";
			SetBackground(empty);
		}

		public override int GetBoardY()
		{
			return GlobalMembers.RS(ConstantsWP.BOARD_Y_BUTTERFLY);
		}

		public override bool WantsTutorialReplays()
		{
			return false;
		}

		public override void DoEndLevelDialog()
		{
			mEndLevelDialog = new ButterflyEndLevelDialog(this);
			mEndLevelDialog.SetQuestName(GetQuestName());
			GlobalMembers.gApp.AddDialog(38, mEndLevelDialog);
			BringToFront(mEndLevelDialog);
		}

		public override string GetMusicName()
		{
			return "Butterflies";
		}

		public override float GetModePointMultiplier()
		{
			if (mIsPerpetual)
			{
				return 5f;
			}
			return 1f;
		}

		public override float GetRankPointMultiplier()
		{
			return 4f;
		}

		public override bool WantsCalmEffects()
		{
			return true;
		}

		public override bool CanPlay()
		{
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.mDestRow == -1)
				{
					return false;
				}
			}
			return base.CanPlay();
		}

		public override void Init()
		{
			if (mIsPerpetual)
			{
				mUiConfig = EUIConfig.eUIConfig_StandardNoReplay;
			}
			base.Init();
		}

		public override void NewGame(bool restartingGame)
		{
			mDefaultDropCountdown = SexyFramework.GlobalMembers.sexyatoi(mParams, "DropCountdown");
			mDefaultMoveCountdown = SexyFramework.GlobalMembers.sexyatoi(mParams, "MoveCountdown");
			mDropCountdownPerLevel = SexyFramework.GlobalMembers.sexyatof(mParams, "DropCountdownPerLevel");
			mMoveCountdownPerLevel = SexyFramework.GlobalMembers.sexyatof(mParams, "MoveCountdownPerLevel");
			mSideSpawnChance = SexyFramework.GlobalMembers.sexyatof(mParams, "SideSpawnChance");
			mSideSpawnChancePerLevel = SexyFramework.GlobalMembers.sexyatof(mParams, "SideSpawnChancePerLevel");
			mSideSpawnChanceMax = SexyFramework.GlobalMembers.sexyatof(mParams, "SideSpawnChanceMax");
			mSpawnCountStart = SexyFramework.GlobalMembers.sexyatoi(mParams, "SpawnCountStart");
			mSpawnCountMax = SexyFramework.GlobalMembers.sexyatof(mParams, "SpawnCountMax");
			mSpawnCountPerLevel = SexyFramework.GlobalMembers.sexyatof(mParams, "SpawnCountPerLevel");
			if (mSpawnCountMax == 0f)
			{
				mSpawnCountMax = 1f;
			}
			if (mSpawnCountStart == 0)
			{
				mSpawnCountStart = 1;
			}
			mSpawnCount = mSpawnCountStart;
			mSpawnCountAcc = 0f;
			mQueueSpawn = false;
			mDropCountdown = 0;
			mPostFXManager.Clear();
			mLastButterflyEffect = null;
			if (mIsPerpetual)
			{
				mHighScoreIsLevelPoints = false;
				mShowLevelPoints = false;
			}
			mAllowNewComboFloaters = false;
			mGrabbedAt = -1;
			mPlayedGrabSound = false;
			mSpiderCol = 3f;
			mSpider.Play("DROP");
			mSpiderWalkPct.SetConstant(0.0);
			mBiggerSpider.Play("DROP");
			mCurrentReleasedButterflies = 0;
			mSpotOnSpider = false;
			mCountingForGameOver = false;
			mGameOverCountdown = 0;
			mSpiderYOffset = 0f;
			mSpiderDownAfterGrab = true;
			base.NewGame(restartingGame);
		}

		public override void GameOver(bool visible)
		{
			if (!mCountingForGameOver)
			{
				(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("FadeOut", GetMusicName(), false);
				(GlobalMembers.gApp.mMusicInterface as CustomBassMusicInterface).QueueEvent("Play", $"{GetMusicName()}_lose", true);
				// GlobalMembers.gApp.mMusic.PlaySongNoDelay(6, false);
				mSpotOnSpider = true;
				mCountingForGameOver = true;
				mGameOverCountdown = 200;
			}
			else if (--mGameOverCountdown < 0)
			{
				base.GameOver(visible);
				GlobalMembers.gApp.DisableOptionsButtons(false);
			}
		}

		public override void PieceTallied(Piece thePiece)
		{
			base.PieceTallied(thePiece);
			if (thePiece.IsFlagSet(128u) && (mIsPerpetual || mGameOverCount == 0))
			{
				OnButterflyMatched(thePiece);
			}
		}

		public override void SwapSucceeded(SwapData theSwapData)
		{
			if (!theSwapData.mForceSwap)
			{
				mDropCountdown--;
				if (CountButterflies() == 0)
				{
					mDropCountdown = 0;
				}
				if (mDropCountdown <= 0)
				{
					SpawnButterfly();
				}
			}
			base.SwapSucceeded(theSwapData);
		}

		public override void DoHypercube(Piece thePiece, int theColor)
		{
			base.DoHypercube(thePiece, theColor);
			QueueSpawnButterfly();
		}

		public new bool QueueSwap(Piece theSelected, int theSwappedRow, int theSwappedCol, bool forceSwap, bool playerSwapped, bool destroyTarget)
		{
			if (playerSwapped)
			{
				for (int i = 0; i < Common.size(mSwapDataVector); i++)
				{
					SwapData swapData = mSwapDataVector[i];
					if (!swapData.mForceSwap)
					{
						return false;
					}
				}
			}
			return base.QueueSwap(theSelected, theSwappedRow, theSwappedCol, forceSwap, playerSwapped, destroyTarget);
		}

		public override void UpdateGame()
		{
			base.UpdateGame();
			bool flag = mGameTicks <= GlobalMembers.M(150);
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.mDestRow == -1)
				{
					piece.mFallVelocity = 0.01f;
				}
			}
			if (mGrabbedAt >= 0 && mGameTicks == mGrabbedAt + GlobalMembers.M(30))
			{
				int mLevelCompleteCount2 = mLevelCompleteCount;
			}
			if (!mSpider.IsActive() && mGrabbedAt == -1)
			{
				mSpider.Play("IDLE");
			}
			if (mSpiderRandomBehaviour)
			{
				DoSpiderRandomMove();
			}
			else if (mSpiderWalkPct.IsDoingCurve())
			{
				if (!mSpiderWalkPct.IncInVal())
				{
					mSpiderCol = mSpiderWalkColTo;
					mSpider.Play("IDLE");
				}
				else
				{
					mSpiderCol = (float)((double)mSpiderWalkPct * (double)mSpiderWalkColTo + (1.0 - (double)mSpiderWalkPct) * (double)mSpiderWalkColFrom);
				}
			}
			else
			{
				Piece piece2 = null;
				for (int k = -1; k < (flag ? GlobalMembers.M(8) : GlobalMembers.M(5)); k++)
				{
					for (int l = 0; l < 8; l++)
					{
						Piece piece3 = mBoard[Math.Max(0, k), l];
						if (piece3 != null && piece3.IsFlagSet(128u) && (k > -1 || piece3.mDestRow < 0) && (piece2 == null || Math.Abs((float)piece3.mCol - mSpiderCol) < Math.Abs((float)piece2.mCol - mSpiderCol)))
						{
							piece2 = piece3;
						}
					}
					if (piece2 != null)
					{
						break;
					}
				}
				if (piece2 != null)
				{
					if (mSpiderCol == (float)piece2.mCol)
					{
						if (piece2.mDestRow >= 0 && SexyFramework.Common.Rand() % GlobalMembers.M(500) == 0)
						{
							int num = SexyFramework.Common.Rand(4);
							if (piece2.mRow == 0 && num > 0)
							{
								mSpiderRandomBehaviour = true;
								mSpider.Play("GRAB");
								mGrabCounter = 100;
								mSpiderRandomState = 3;
							}
							else
							{
								mSpiderRandomBehaviour = true;
								mSpider.Play("WALK");
								mSpiderRandomState = 0;
								mStartSpiderCol = mSpiderCol;
							}
						}
						if (piece2.mDestRow == -1)
						{
							if (mLevelCompleteCount == 0 && mGrabbedAt == -1 && (double)piece2.mDestPct >= GlobalMembers.M(0.3))
							{
								mSpider.Play("GRAB");
								mGrabbedAt = mGameTicks;
							}
							if (!mGameFinished && piece2.mDestRow == -1 && !piece2.mDestPct.IsDoingCurve())
							{
								if (!mPlayedGrabSound)
								{
									SexyFramework.GlobalMembers.gSexyApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTERFLY_DEATH1);
									mPlayedGrabSound = true;
									piece2.mForceScreenY = true;
									piece2.mForcedY = piece2.mY;
								}
								bool flag2 = false;
								Piece[,] array2 = mBoard;
								foreach (Piece piece4 in array2)
								{
									if (piece4 != null && piece4.mMoveCreditId != -1 && piece4.mDestRow >= 0)
									{
										flag2 = true;
									}
								}
								Piece[,] array3 = mBoard;
								foreach (Piece piece5 in array3)
								{
									if (piece5 != null && piece5.mDestRow < 0 && piece5.mRow != 0)
									{
										flag2 = false;
									}
								}
								if (mGameOverCount == 0 && !flag2)
								{
									if (mSpiderDownAfterGrab)
									{
										mSpiderYOffset += 0.5f;
										piece2.mForcedY += 0.5f;
										if (mSpiderYOffset > 30f)
										{
											mSpiderDownAfterGrab = false;
										}
									}
									else
									{
										mSpiderYOffset -= 6f;
										piece2.mForcedY -= 6f;
									}
									GameOver();
								}
							}
						}
						else if (SexyFramework.Common.Rand() % GlobalMembers.M(1500) == 0)
						{
							mSpider.Play("IDLE");
						}
					}
					else if (flag)
					{
						mSpiderCol = piece2.mCol;
					}
					else
					{
						mSpider.Play("WALK");
						mSpiderWalkColFrom = mSpiderCol;
						mSpiderWalkColTo = piece2.mCol;
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBUTTERFLY_SPIDER_WALK_PCT, mSpiderWalkPct);
						mSpiderWalkPct.mIncRate *= GlobalMembers.M(1.8) + (double)Math.Abs((float)piece2.mCol - mSpiderCol) * GlobalMembers.M(-0.1) + (double)piece2.mRow * GlobalMembers.M(-0.1);
					}
				}
			}
			mSpider.Resize(GlobalMembers.S(GetBoardX() + (int)(mSpiderCol * 100f) + ConstantsWP.BUTTERFLY_SPIDER_X_OFFSET), (int)(GlobalMembers.MS(-450f + mSpiderYOffset) + (float)ConstantsWP.BUTTERFLY_SPIDER_Y_OFFSET), GlobalMembers.S(mSpider.mAnimRect.mWidth), GlobalMembers.S(mSpider.mAnimRect.mHeight));
			if (mGameTicks >= GlobalMembers.M(150))
			{
				mSpider.Update();
			}
			if (mSpiderYOffset < -500f)
			{
				mBiggerSpider.Resize(GlobalMembers.S(GetBoardX() + ConstantsWP.BUTTERFLY_BIGSPIDER_X_OFFSET), GlobalMembers.S(ConstantsWP.BUTTERFLY_BIGSPIDER_Y_OFFSET), GlobalMembers.S(mSpider.mAnimRect.mWidth), GlobalMembers.S(mSpider.mAnimRect.mHeight));
				if (mGameOverCount < 400)
				{
					mBiggerSpider.Update();
				}
			}
			if (mQueueSpawn && Common.size(mLightningStorms) == 0)
			{
				SpawnButterfly();
				mQueueSpawn = false;
			}
			Piece[,] array4 = mBoard;
			foreach (Piece piece6 in array4)
			{
				if (piece6 == null || !piece6.IsFlagSet(128u))
				{
					continue;
				}
				piece6.mShakeScale = Math.Max(0f, 2f - (float)piece6.mRow) / 2f;
				if (piece6.mDestRow == -1)
				{
					piece6.mShakeScale *= (float)(1.0 - (double)piece6.mDestPct * (double)GlobalMembers.M(1f));
				}
				if (piece6.mCounter > 0 || Common.size(mDeferredTutorialVector) != 0 || Common.size(mLightningStorms) != 0)
				{
					continue;
				}
				int num6 = piece6.mRow - 1;
				if (num6 >= 0)
				{
					Piece pieceAtRowCol = GetPieceAtRowCol(num6, piece6.mCol);
					if (pieceAtRowCol != null || num6 == -1)
					{
						QueueSwap(piece6, num6, piece6.mCol, true, false);
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_ANIM_CURVE_B, piece6.mAnimCurve);
					}
					piece6.mCounter = (int)Math.Max(1.0, (double)mDefaultMoveCountdown - Math.Floor((float)mLevel * piece6.mMoveCountdownPerLevel));
					continue;
				}
				int num7;
				for (num7 = 0; num7 < 8 && mBoard[num7, piece6.mCol] != null && !IsPieceMatching(mBoard[num7, piece6.mCol]); num7++)
				{
				}
				if (num7 == 8 && Common.size(mLightningStorms) == 0)
				{
					if (piece6.mDestRow != -1)
					{
						piece6.mDestCol = piece6.mCol;
						piece6.mDestRow = -1;
						GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.ePIECE_DEST_PCT_D, piece6.mDestPct);
					}
					return;
				}
			}
		}

		public override void Update()
		{
			base.Update();
			mSpider.mColor = Color.FAlpha(GetAlpha());
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			int chunkBeginPos = 0;
			GameChunkHeader header = new GameChunkHeader();
			if (theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkButterflyBoard, header, out chunkBeginPos))
			{
				theBuffer.ReadValuePair(out mDefaultMoveCountdown);
				theBuffer.ReadValuePair(out mDefaultDropCountdown);
				theBuffer.ReadValuePair(out mGrabbedAt);
				theBuffer.ReadValuePair(out mDropCountdown);
				theBuffer.ReadValuePair(out mSpawnCountStart);
				theBuffer.ReadValuePair(out mDropCountdownPerLevel);
				theBuffer.ReadValuePair(out mMoveCountdownPerLevel);
				theBuffer.ReadValuePair(out mSpawnCountMax);
				theBuffer.ReadValuePair(out mSpawnCountPerLevel);
				theBuffer.ReadValuePair(out mSpawnCountAcc);
				theBuffer.ReadValuePair(out mSpiderCol);
				theBuffer.ReadValuePair(out mSpiderWalkColFrom);
				theBuffer.ReadValuePair(out mSpiderWalkColTo);
				theBuffer.ReadValuePair(out mSpawnCount);
				theBuffer.ReadValuePair(out mSideSpawnChance);
				theBuffer.ReadValuePair(out mSideSpawnChancePerLevel);
				theBuffer.ReadValuePair(out mSideSpawnChanceMax);
				theBuffer.ReadValuePair(out mAllowNewComboFloaters);
				theBuffer.ReadValuePair(out mSpiderWalkPct);
				base.LoadGameExtra(theBuffer);
			}
		}

		public override bool SaveGameExtra(Serialiser theBuffer)
		{
			int chunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkButterflyBoard);
			theBuffer.WriteValuePair(Serialiser.PairID.BflyDefMoveCountdown, mDefaultMoveCountdown);
			theBuffer.WriteValuePair(Serialiser.PairID.BflyDefDropCountdown, mDefaultDropCountdown);
			theBuffer.WriteValuePair(Serialiser.PairID.BflyGrabbedAt, mGrabbedAt);
			theBuffer.WriteValuePair(Serialiser.PairID.BflyDropCountdown, mDropCountdown);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpawnCountStart, mSpawnCountStart);
			theBuffer.WriteValuePair(Serialiser.PairID.BflyDropCountdownPerLevel, mDropCountdownPerLevel);
			theBuffer.WriteValuePair(Serialiser.PairID.BflyMoveCountdownPerLevel, mMoveCountdownPerLevel);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpawnCountMax, mSpawnCountMax);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpawnCountPerLevel, mSpawnCountPerLevel);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpawnCountAcc, mSpawnCountAcc);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpiderCol, mSpiderCol);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpiderWalkColFrom, mSpiderWalkColFrom);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpiderWalkColTo, mSpiderWalkColTo);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpawnCount, mSpawnCount);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySideSpawnChance, mSideSpawnChance);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySideSpawnChancePerLevel, mSideSpawnChance);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySideSpawnChanceMax, mSideSpawnChanceMax);
			theBuffer.WriteValuePair(Serialiser.PairID.BflyAllowNewComboFloaters, mAllowNewComboFloaters);
			theBuffer.WriteValuePair(Serialiser.PairID.BflySpiderWalkPct, mSpiderWalkPct);
			theBuffer.FinalizeGameChunkHeader(chunkBeginLoc);
			return base.SaveGameExtra(theBuffer);
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			GlobalMembers.M(380);
			GlobalMembers.M(150);
			GlobalMembers.M(30);
			if (g.mPushedColorVector.Count > 0)
			{
				g.PopColorMult();
			}
			g.PushState();
			g.Translate(mSpider.mX + 15, mSpider.mY);
			mSpider.Draw(g);
			g.PopState();
			DrawOverlayPreAnnounce(g, thePriority);
			if (mQuestGoal != null)
			{
				mQuestGoal.DrawScore(g);
			}
			if (mSpotOnSpider)
			{
				DrawIris(g, mSpider.mX + GlobalMembers.S(145), (int)GlobalMembers.S(60f + mSpiderYOffset));
			}
			DrawOverlayPostAnnounce(g, thePriority);
			if (mSpiderYOffset < -500f)
			{
				g.PushState();
				g.Translate(mBiggerSpider.mX, mBiggerSpider.mY);
				mBiggerSpider.Draw(g);
				g.PopState();
			}
		}

		public override void LevelUp()
		{
			if (!mIsPerpetual)
			{
				base.LevelUp();
			}
		}

		public void ReleaseButterfly(Piece thePiece)
		{
			mLastButterflyEffect = ButterflyEffect.alloc(thePiece, this);
			if (!mIsPerpetual)
			{
				mLastButterflyEffect.mTargetY.mOutMin = GlobalMembers.M(590);
			}
			mPostFXManager.AddEffect(mLastButterflyEffect);
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTERFLYESCAPE);
			thePiece.mCounter++;
			thePiece.mAlpha.SetConstant(0.0);
		}

		public void QueueSpawnButterfly()
		{
			mQueueSpawn = true;
		}

		public void SpawnButterfly()
		{
			mDropCountdown = (int)Math.Max(1.0, (double)mDefaultDropCountdown - Math.Floor((float)mLevel * mDropCountdownPerLevel));
			mSpawnCount = Math.Min(mSpawnCountMax, mSpawnCount + mSpawnCountPerLevel);
			mSideSpawnChance = Math.Min(mSideSpawnChanceMax, mSideSpawnChance + mSideSpawnChancePerLevel);
			mSpawnCountAcc += mSpawnCount;
			while ((double)mSpawnCountAcc >= 1.0)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 100; j++)
					{
						int theCol = ((!((double)(mRand.Next() % 100000) / 100000.0 < (double)mSideSpawnChance)) ? ((int)(mRand.Next() % 6 + 1)) : ((mRand.Next() % 2 != 0) ? 7 : 0));
						Piece pieceAtRowCol = GetPieceAtRowCol(7, theCol);
						if (pieceAtRowCol != null && !pieceAtRowCol.IsFlagSet(128u) && (pieceAtRowCol.mFlags == 0 || (i == 1 && pieceAtRowCol.mColor > -1)) && IsPieceStill(pieceAtRowCol))
						{
							Butterflyify(pieceAtRowCol);
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTERFLY_APPEAR);
							pieceAtRowCol.mMoveCountdownPerLevel = mMoveCountdownPerLevel;
							pieceAtRowCol.mCounter = (int)Math.Max(1.0, (double)mDefaultMoveCountdown - Math.Floor((float)mLevel * mMoveCountdownPerLevel));
							pieceAtRowCol.mTallied = false;
							i = 2;
							break;
						}
					}
				}
				mSpawnCountAcc -= 1f;
			}
		}

		public override void ReadyToPlay()
		{
			if (mSpawnCountAcc == 0f)
			{
				SpawnButterfly();
			}
		}

		public override Points AddPoints(int theX, int theY, int thePoints, Color theColor, uint theId, bool addtotube, bool usePointMultiplier, int theMoveCreditId, bool theForceAdd, int thePointType)
		{
			if (mIsPerpetual)
			{
				Points points = base.AddPoints(theX, theY, thePoints, theColor, theId, addtotube, usePointMultiplier, theMoveCreditId, theForceAdd, thePointType);
				if ((int)theId > 0 && (mAllowNewComboFloaters || mPointsManager.Find((uint)(-2 - theMoveCreditId)) != null))
				{
					int moveStat = GetMoveStat(theMoveCreditId, 28);
					if (moveStat >= 2 && points != null)
					{
						Points points2 = AddPoints(theX, theY, 0, Color.White, (uint)(-2 - theMoveCreditId), true, true, theMoveCreditId, true);
						points2.mX = points.mX;
						points2.mY = points.mY + points.mScale * (float)GlobalMembers.M(1000);
						points2.mTimer = points.mTimer;
						for (int i = 0; i < GlobalMembers.Max_LAYERS; i++)
						{
							points2.mColorCycle[i] = points.mColorCycle[i];
						}
						points2.mString = string.Format(GlobalMembers._ID("x{0} COMBO", 158), moveStat);
					}
				}
				return points;
			}
			return null;
		}

		public override bool WantTopLevelBar()
		{
			return false;
		}

		public int CountButterflies()
		{
			int num = 0;
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.IsFlagSet(128u) && !piece.mTallied)
				{
					num++;
				}
			}
			return num;
		}

		public override bool WantWarningGlow(bool forSound)
		{
			if (!mGameFinished && !mCountingForGameOver)
			{
				for (int i = 0; i < 8; i++)
				{
					Piece pieceAtRowCol = GetPieceAtRowCol(0, i);
					if (pieceAtRowCol != null && pieceAtRowCol.IsFlagSet(128u))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override Color GetWarningGlowColor()
		{
			int ticksLeft = GetTicksLeft();
			if (ticksLeft == 0)
			{
				return new Color(255, 0, 0, 127);
			}
			int num = ((GetTimeLimit() > 60) ? 1500 : 1000);
			float num2 = (float)(num - ticksLeft) / (float)num;
			int num3 = (int)((Math.Sin((float)mUpdateCnt * GlobalMembers.M(0.15f)) * 127.0 + 127.0) * (double)num2 * (double)GetPieceAlpha());
			return new Color(255, 0, 0, num3 / 2);
		}
	}
}
