using System;
using System.Collections.Generic;
using System.Text;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Sound;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class ZenBoard : Board
	{
		public Bej3Button mZenOptionsButton;

		public int mLastNoiseIdx;

		public int mNoiseSoundId;

		public SoundInstance mNoiseSoundInstance;

		public SoundInstance mBreathSoundInstance;

		public CurvedVal mNoiseVolume = new CurvedVal();

		public float mZenUIBoardAlpha;

		public CurvedVal mBreathPct = new CurvedVal();

		public float mAffirmationPct;

		public string mAffirmationDesc = string.Empty;

		public string mAffirmationText = string.Empty;

		public int mAffirmationIndex;

		public List<string> mAffirmations = new List<string>();

		public List<string> mSubliminalAffirmations = new List<string>();

		public Point mAffirmationOrigin = default(Point);

		public CurvedVal mAffirmationCenterPct = new CurvedVal();

		public CurvedVal mAffirmationZoom = new CurvedVal();

		public CurvedVal mAffirmationAlpha = new CurvedVal();

		public bool mDebugMantras;

		public CurvedVal mDynamicSpeed = new CurvedVal();

		public bool mHasZenOptionsDialog;

		public bool mUsingAmbientMusicVolume;

		public double mMasterMusicVolume;

		public ZenBoard()
		{
			if (Common.size(GlobalMembers.gApp.mSBAFiles) > 0 && GlobalMembers.gApp.mProfile.mSBAFileName.Length == 0)
			{
				GlobalMembers.gApp.mProfile.mSBAFileName = Common.front(GlobalMembers.gApp.mSBAFiles);
			}
			mZenUIBoardAlpha = 1f;
			mDynamicSpeed.SetConstant(1.0);
			mLastNoiseIdx = -1;
			mAffirmationIndex = -1;
			mDebugMantras = false;
			mHasZenOptionsDialog = false;
			if (Common.size(GlobalMembers.gApp.mAffirmationFiles) > 0)
			{
				if (GlobalMembers.gApp.mProfile.mAffirmationFileName.Length == 0)
				{
					GlobalMembers.gApp.mProfile.mAffirmationFileName = Common.front(GlobalMembers.gApp.mAffirmationFiles);
				}
				if (GlobalMembers.gApp.mProfile.mAffirmationOn)
				{
					LoadAffirmations();
				}
			}
			mAffirmationPct = 0f;
			mUsingAmbientMusicVolume = false;
			if (GlobalMembers.gApp.mProfile.mBeatOn)
			{
				StartupSBA();
			}
			mNoiseSoundInstance = null;
			mNoiseSoundId = -1;
			mBreathSoundInstance = null;
			mParams["Title"] = "Zen";
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eZEN_BOARD_NOISE_VOLUME_INIT, mNoiseVolume);
			mZenOptionsButton = null;
		}

		public override void Dispose()
		{
			GlobalMembers.gApp.mBinauralManager.Load("");
			StopZenNoise();
			if (GlobalMembers.gApp.mProfile.mNoiseOn)
			{
				GlobalMembers.gApp.mZenAmbientMusicVolume = GlobalMembers.gApp.mMusicVolume;
				GlobalMembers.gApp.SetMusicVolume(mMasterMusicVolume);
			}
			if (mNoiseSoundInstance != null)
			{
				mNoiseSoundInstance.Release();
			}
			mNoiseSoundInstance = null;
			if (mNoiseSoundId != -1)
			{
				GlobalMembers.gApp.mSoundManager.ReleaseSound(mNoiseSoundId);
			}
			mNoiseSoundId = -1;
		}

		public override void UnloadContent()
		{
			BejeweledLivePlusApp.UnloadContent("GamePlay_UI_Normal");
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
			base.LoadContent(threaded);
		}

		public new void NewGame(bool restartingGame)
		{
			base.NewGame(restartingGame);
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			base.LoadGameExtra(theBuffer);
		}

		public override void DrawScore(Graphics g)
		{
			base.DrawScore(g);
		}

		public override void DrawScoreWidget(Graphics g)
		{
		}

		public override int GetLevelPoints()
		{
			return GlobalMembers.M(2500) + Math.Min(mLevel, GlobalMembers.M(30)) * GlobalMembers.M(750);
		}

		public void StartupSBA()
		{
			if (GlobalMembers.gApp.mProfile.mBeatOn)
			{
				GlobalMembers.gApp.mBinauralManager.Load("binaural\\" + GlobalMembers.gApp.mProfile.mSBAFileName);
			}
			else
			{
				GlobalMembers.gApp.mBinauralManager.Load("");
			}
		}

		public void LoadAffirmations()
		{
			mAffirmationIndex = -1;
			mAffirmations.Clear();
			mSubliminalAffirmations.Clear();
			bool flag = false;
			EncodingParser encodingParser = new EncodingParser();
			if (!encodingParser.OpenFile(GlobalMembers.gApp.mResourceManager.GetLocaleFolder(true) + "affirmations\\" + GlobalMembers.gApp.mProfile.mAffirmationFileName))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			char theChar = '\0';
			while (encodingParser.GetChar(ref theChar) == EncodingParser.GetCharReturnType.SUCCESSFUL)
			{
				if (theChar == '\n' || theChar == '\r')
				{
					string text = stringBuilder.ToString().Trim();
					if (stringBuilder.Length > 0)
					{
						if (stringBuilder[0] == '#')
						{
							if (string.Compare(text.Substring(0, 6), "#DESC:", StringComparison.OrdinalIgnoreCase) == 0)
							{
								mAffirmationDesc = text.Substring(6).Trim();
							}
							else if (string.Compare(text.Substring(0, 5), "#DESC", StringComparison.OrdinalIgnoreCase) == 0)
							{
								mAffirmationDesc = text.Substring(5).Trim();
							}
							else
							{
								flag = true;
							}
						}
						else if (flag)
						{
							mSubliminalAffirmations.Add(text);
						}
						else
						{
							mAffirmations.Add(text);
						}
					}
					stringBuilder.Clear();
				}
				else
				{
					stringBuilder.Append(theChar);
				}
			}
			string text2 = stringBuilder.ToString().Trim();
			if (text2 != string.Empty)
			{
				if (flag)
				{
					mSubliminalAffirmations.Add(text2);
				}
				else
				{
					mAffirmations.Add(text2);
				}
			}
		}

		public void LoadAmbientSound()
		{
			if (mNoiseSoundInstance != null)
			{
				mNoiseSoundInstance.Release();
				mNoiseSoundInstance = null;
			}
			if (mNoiseSoundId != -1)
			{
				GlobalMembers.gApp.mSoundManager.ReleaseSound(mNoiseSoundId);
				mNoiseSoundId = -1;
			}
			if (mInterfaceState != InterfaceState.INTERFACE_STATE_INGAME && mInterfaceState != InterfaceState.INTERFACE_STATE_PAUSEMENU && mInterfaceState != InterfaceState.INTERFACE_STATE_ZENOPTIONSMENU)
			{
				return;
			}
			if (GlobalMembers.gApp.mProfile.mNoiseFileName == "*")
			{
				int num = (int)(mRand.Next() % Common.size(GlobalMembers.gApp.mAmbientFiles));
				if (num == mLastNoiseIdx)
				{
					mLastNoiseIdx = (mLastNoiseIdx + 1) % Common.size(GlobalMembers.gApp.mAmbientFiles);
				}
				else
				{
					mLastNoiseIdx = num;
				}
			}
			string mNoiseFileName = GlobalMembers.gApp.mProfile.mNoiseFileName;
			if (string.IsNullOrEmpty(mNoiseFileName))
			{
				mNoiseSoundId = -1;
				mNoiseSoundInstance = null;
			}
			else if (mNoiseFileName == "*")
			{
				mNoiseSoundId = GlobalMembers.gApp.mSoundManager.LoadSound("ambient\\" + SexyFramework.Common.GetFileName(GlobalMembers.gApp.mAmbientFiles[mLastNoiseIdx], true));
			}
			else
			{
				mNoiseSoundId = GlobalMembers.gApp.mSoundManager.LoadSound("ambient\\" + SexyFramework.Common.GetFileName(GlobalMembers.gApp.mProfile.mNoiseFileName, true));
			}
			if (mNoiseSoundId != -1)
			{
				mNoiseSoundInstance = GlobalMembers.gApp.mSoundManager.GetSoundInstance(mNoiseSoundId);
				if (mNoiseSoundInstance != null)
				{
					mNoiseSoundInstance.retain();
					if (!GlobalMembers.gApp.IsMuted())
					{
						GlobalMembers.gApp.mSoundManager.SetVolume(2, GlobalMembers.gApp.mZenAmbientVolume);
					}
					mNoiseSoundInstance.SetMasterVolumeIdx(2);
					if (GlobalMembers.gApp.mProfile.mNoiseOn)
					{
						mNoiseSoundInstance.SetVolume(mNoiseVolume);
					}
					else
					{
						mNoiseSoundInstance.SetVolume(0.0);
					}
				}
			}
			RehupMusicVolume();
		}

		public override void GameOver(bool visible)
		{
			Piece pieceAtRowCol = GetPieceAtRowCol((int)(mRand.Next() % 8), (int)(mRand.Next() % 8));
			if (pieceAtRowCol != null)
			{
				Hypercubeify(pieceAtRowCol);
			}
		}

		public override void HyperspaceEvent(HYPERSPACEEVENT inEvent)
		{
			base.HyperspaceEvent(inEvent);
			switch (inEvent)
			{
			case HYPERSPACEEVENT.HYPERSPACEEVENT_OldLevelClear:
				CalcBadges();
				mBadgeManager.SyncBadges();
				if (Common.size(GlobalMembers.gApp.mProfile.mDeferredBadgeVector) > 0)
				{
					mZenDoBadgeAward = true;
				}
				break;
			case HYPERSPACEEVENT.HYPERSPACEEVENT_Finish:
			{
				if (mLevel + 1 >= 5 && !GlobalMembers.gApp.mProfile.mEndlessModeUnlocked[1])
				{
					GlobalMembers.gApp.UnlockEndlessMode(EEndlessMode.ENDLESS_BUTTERFLY);
				}
				long thePoints = GlobalMembers.gApp.mProfile.mOfflineRankPoints + GlobalMembers.gApp.mProfile.GetRankPointsBracket((int)((float)mGameStats[1] / GetRankPointMultiplier()));
				int num = (int)GlobalMembers.gApp.mProfile.GetRankAtPoints(thePoints);
				if (num != GlobalMembers.gApp.mProfile.mOfflineRank)
				{
					mDoRankUp = true;
				}
				if (GlobalMembers.gApp.mProfile.mNoiseFileName == "*")
				{
					LoadAmbientSound();
					PlayZenNoise();
				}
				break;
			}
			}
		}

		public void RehupMusicVolume()
		{
			if (mNoiseSoundInstance != null && GlobalMembers.gApp.mProfile.mNoiseOn)
			{
				if (!mUsingAmbientMusicVolume)
				{
					mMasterMusicVolume = GlobalMembers.gApp.mMusicVolume;
					GlobalMembers.gApp.SetMusicVolume(GlobalMembers.gApp.mZenAmbientMusicVolume);
					mUsingAmbientMusicVolume = true;
				}
			}
			else if (mUsingAmbientMusicVolume)
			{
				GlobalMembers.gApp.mZenAmbientMusicVolume = GlobalMembers.gApp.mMusicVolume;
				GlobalMembers.gApp.SetMusicVolume(mMasterMusicVolume);
				mUsingAmbientMusicVolume = false;
			}
		}

		public override bool AllowPowerups()
		{
			return true;
		}

		public override bool WantsCalmEffects()
		{
			return true;
		}

		public override int GetMinComplementLevel()
		{
			return 1;
		}

		public override float GetGravityFactor()
		{
			return (float)((double)GlobalMembers.M(0.9f) * (double)mDynamicSpeed);
		}

		public override float GetSwapSpeed()
		{
			return (float)((double)GlobalMembers.M(0.9f) * (double)mDynamicSpeed);
		}

		public override float GetMatchSpeed()
		{
			return (float)((double)GlobalMembers.M(0.75f) * (double)mDynamicSpeed);
		}

		public override float GetBoardAlpha()
		{
			return (float)((double)mZenUIBoardAlpha * (double)mAlpha);
		}

		public override bool WantsLevelBasedBackground()
		{
			return true;
		}

		public override bool WantAnnihilatorReplacement()
		{
			return true;
		}

		public override void ExplodeAt(int theCol, int theRow)
		{
			base.ExplodeAt(theCol, theRow);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eZEN_BOARD_DYNAMIC_SPEED, mDynamicSpeed);
		}

		public void FireAffirmation()
		{
			mAffirmationPct = GlobalMembers.M(0.7601f);
			if (!GlobalMembers.gApp.mProfile.mAffirmationSubliminal)
			{
				return;
			}
			int count = mAffirmations.Count;
			int count2 = mSubliminalAffirmations.Count;
			if (count <= 0 && count2 <= 0)
			{
				return;
			}
			if (count2 > 0)
			{
				mAffirmationIndex++;
				if (mAffirmationIndex >= count2)
				{
					mAffirmationIndex = 0;
				}
			}
			else if (count > 0)
			{
				mAffirmationIndex++;
				if (mAffirmationIndex >= count)
				{
					mAffirmationIndex = 0;
				}
			}
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eZEN_BOARD_AFFIRMATION_CENTER_PCT, mAffirmationCenterPct);
			mAffirmationCenterPct.mIncRate *= GlobalMembers.M(1.25) + GlobalMembers.M(-1.0) * (double)GlobalMembers.gApp.mProfile.mAffirmationSubliminality;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eZEN_BOARD_AFFIRMATION_ZOOM, mAffirmationZoom, mAffirmationCenterPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eZEN_BOARD_AFFIRMATION_ALPHA, mAffirmationAlpha, mAffirmationCenterPct);
			mAffirmationAlpha.mOutMax *= GlobalMembers.M(2.5) + Math.Max(GlobalMembers.M(-2.0), GlobalMembers.M(-3.0) * (1.0 - (double)GlobalMembers.gApp.mProfile.mAffirmationSubliminality));
		}

		public override void SwapSucceeded(SwapData theSwapData)
		{
			base.SwapSucceeded(theSwapData);
			Point point = default(Point);
			int num = 0;
			Piece[,] array = mBoard;
			foreach (Piece piece in array)
			{
				if (piece != null && piece.mMoveCreditId == theSwapData.mPiece1.mMoveCreditId)
				{
					point.mX += (int)piece.CX();
					point.mY += (int)piece.CY();
					num++;
				}
			}
			if (num > 0)
			{
				mAffirmationOrigin = point / num;
			}
			if (((double)mAffirmationPct > GlobalMembers.M(0.75) && (double)mAffirmationPct < GlobalMembers.M(0.76)) || mDebugMantras)
			{
				FireAffirmation();
			}
		}

		public override HYPERSPACETRANS GetHyperspaceTransType()
		{
			return HYPERSPACETRANS.HYPERSPACETRANS_Zen;
		}

		public override string GetSavedGameName()
		{
			return "zen.sav";
		}

		public override bool AllowSpeedBonus()
		{
			return false;
		}

		public override bool AllowNoMoreMoves()
		{
			return false;
		}

		public override bool SupportsReplays()
		{
			return false;
		}

		public override float GetModePointMultiplier()
		{
			return 1f;
		}

		public override float GetRankPointMultiplier()
		{
			return 0.4f;
		}

		public override void DrawOverlay(Graphics g, int thePriority)
		{
			base.DrawOverlay(g, thePriority);
			if (GlobalMembers.gApp.mProfile.mBreathOn && GlobalMembers.gApp.mProfile.mBreathVisual)
			{
				g.SetColorizeImages(true);
			}
		}

		public override void FirstDraw()
		{
			base.FirstDraw();
		}

		public override void Draw(Graphics g)
		{
			if (!mContentLoaded)
			{
				return;
			}
			base.Draw(g);
			if (GlobalMembers.gApp.mProfile.mBreathOn && GlobalMembers.gApp.mProfile.mBreathVisual && mHyperspace == null)
			{
				int num = (int)GlobalMembers.S(800.0 * (double)mBreathPct);
				int theY = GlobalMembers.S(GetBoardY() + 800) - num;
				Rect theRect = new Rect(GlobalMembers.S(GetBoardX()), theY, GlobalMembers.S(800), num);
				if ((double)mSideXOff != 0.0)
				{
					theRect.mX += (int)((double)mSideXOff * (double)mSlideXScale);
				}
				g.SetColor(new Color(GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(0) + (int)(((double)GlobalMembers.M(40) * (double)mBreathPct + (double)GlobalMembers.M(24)) * (double)GetBoardAlpha() * (double)GetAlpha())));
				g.FillRect(theRect);
				num = (int)GlobalMembers.S(1200.0 * (double)mBreathPct);
				theY = GlobalMembers.S(1200) - num;
				Rect theRect2 = new Rect(GlobalMembers.gApp.mScreenBounds.mX, theY, GlobalMembers.gApp.mScreenBounds.mWidth, num);
				g.SetColor(new Color(GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(0) + (int)(((double)GlobalMembers.M(40) * (double)mBreathPct + (double)GlobalMembers.M(24)) * (1.0 - (double)GetBoardAlpha()) * (double)GetAlpha())));
				g.FillRect(theRect2);
			}
			if (GlobalMembers.gApp.mProfile.mAffirmationSubliminal && GlobalMembers.gApp.mProfile.mAffirmationOn && (double)mAffirmationAlpha != 0.0 && mAffirmationIndex > -1)
			{
				string theString = string.Empty;
				if (mSubliminalAffirmations.Count > 0)
				{
					theString = mSubliminalAffirmations[mAffirmationIndex];
				}
				else if (mAffirmations.Count > 0)
				{
					theString = mAffirmations[mAffirmationIndex];
				}
				g.PushState();
				g.SetColor(Color.White);
				g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
				Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, new Color(255, 255, 255, 0));
				Utils.SetFontLayerColor((ImageFont)g.GetFont(), 1, new Color(255, 255, 255, (int)((double)GlobalMembers.M(40) * (double)mAffirmationAlpha * (double)GetAlpha())));
				Point point = new Point(GlobalMembers.S(GetBoardCenterX()), GlobalMembers.S(GetBoardCenterY())) * mAffirmationCenterPct + GlobalMembers.S(mAffirmationOrigin) * (1.0 - (double)mAffirmationCenterPct);
				float num2 = (float)((double)mAffirmationZoom * (double)GlobalMembers.MS(300) / (double)g.StringWidth(theString));
				Utils.PushScale(g, num2, num2, point.mX, point.mY);
				g.WriteString(theString, point.mX, point.mY + GlobalMembers.MS(25));
				Utils.PopScale(g);
				g.PopState();
			}
			base.DrawGameElements(g);
			if (GlobalMembers.gApp.mProfile.mAffirmationOn && !GlobalMembers.gApp.mProfile.mAffirmationSubliminal)
			{
				float num3 = (float)((double)(Math.Min(1f, Math.Min(mAffirmationPct * 7f, (GlobalMembers.M(0.9f) - mAffirmationPct) * 7f)) * GetBoardAlpha()) * (1.0 - (double)mTransitionBoardCurve));
				if (num3 > 0f && !string.IsNullOrEmpty(mAffirmationText))
				{
					((ImageFont)GlobalMembersResources.FONT_DIALOG).PushLayerColor("MAIN", new Color(GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255)));
					g.SetFont(GlobalMembersResources.FONT_DIALOG);
					Rect levelBarRect = GetLevelBarRect();
					int num4 = levelBarRect.mX + levelBarRect.mWidth / 2;
					int num5 = levelBarRect.mY + levelBarRect.mHeight / 2 - GlobalMembersResources.FONT_DIALOG.GetHeight() / 2 + GlobalMembersResources.FONT_DIALOG.mAscent;
					num4 += mTransBoardOffsetX;
					num5 -= mTransBoardOffsetY;
					g.SetColor(new Color(255, 255, 255, (int)(255f * num3)));
					Utils.PushScale(g, 0.8f, 0.8f, num4, num5);
					g.DrawString(mAffirmationText, num4 - GlobalMembersResources.FONT_DIALOG.StringWidth(mAffirmationText) / 2, num5 - 2);
					Utils.PopScale(g);
					((ImageFont)GlobalMembersResources.FONT_DIALOG).PopLayerColor("MAIN");
				}
			}
		}

		public override void DrawMenuWidget(Graphics g)
		{
			base.DrawMenuWidget(g);
		}

		public override void DoUpdate()
		{
			if (mHasZenOptionsDialog)
			{
				mSideAlpha.IncInVal();
			}
			else
			{
				base.DoUpdate();
			}
		}

		public override void Update()
		{
			if (!mContentLoaded)
			{
				return;
			}
			if (mNoiseVolume.IsDoingCurve())
			{
				mNoiseVolume.IncInVal();
				if (GlobalMembers.gApp.mProfile.mNoiseOn && mNoiseSoundInstance != null)
				{
					mNoiseSoundInstance.SetVolume(mNoiseVolume);
				}
			}
			if (mHasZenOptionsDialog)
			{
				mZenUIBoardAlpha = Math.Max(0f, mZenUIBoardAlpha - GlobalMembers.M(0.03f));
			}
			else
			{
				mZenUIBoardAlpha = Math.Min(1f, mZenUIBoardAlpha + GlobalMembers.M(0.05f));
			}
			base.Update();
			if (GlobalMembers.gApp.mProfile.mBreathOn)
			{
				float num = (float)(double)mBreathPct;
				if (!mBreathPct.IsDoingCurve())
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eZEN_BOARD_BREATH_PCT, mBreathPct);
				}
				mBreathPct.mIncRate = GlobalMembers.M(0.00057) + (double)GlobalMembers.gApp.mProfile.mBreathSpeed * GlobalMembers.M(0.001);
				mBreathPct.IncInVal();
				if (!GlobalMembers.gApp.IsMuted())
				{
					GlobalMembers.M(-6.0);
					float mBreathSpeed = GlobalMembers.gApp.mProfile.mBreathSpeed;
					GlobalMembers.MS(20.0);
					if (num < 0.01f && (double)mBreathPct >= 0.009999999776482582)
					{
						mBreathSoundInstance = GlobalMembers.gApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_BREATH_IN);
						if (mBreathSoundInstance != null)
						{
							mBreathSoundInstance.Play(false, true);
							mBreathSoundInstance.SetVolume(GlobalMembers.gApp.mZenBreathVolume);
						}
					}
					else if (num > 0.99f && (double)mBreathPct <= 0.9900000095367432)
					{
						mBreathSoundInstance = GlobalMembers.gApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_BREATH_OUT);
						if (mBreathSoundInstance != null)
						{
							mBreathSoundInstance.Play(false, true);
							mBreathSoundInstance.SetVolume(GlobalMembers.gApp.mZenBreathVolume);
						}
					}
				}
			}
			if (GlobalMembers.gApp.mProfile.mAffirmationOn && (double)GetBoardAlpha() == 1.0)
			{
				if ((double)mAffirmationPct < GlobalMembers.M(0.75) || (double)mAffirmationPct >= GlobalMembers.M(0.76))
				{
					if (GlobalMembers.gApp.mProfile.mAffirmationSubliminal)
					{
						mAffirmationPct += GlobalMembers.MS(0.001f);
					}
					else
					{
						mAffirmationPct += GlobalMembers.MS(5E-05f) + GlobalMembers.gApp.mProfile.mAffirmationSpeed * GlobalMembers.MS(0.001f);
					}
				}
				if (mAffirmationPct > 1f && mAffirmations.Count > 0)
				{
					mAffirmationIndex = (mAffirmationIndex + 1) % mAffirmations.Count;
					mAffirmationText = mAffirmations[mAffirmationIndex];
					mAffirmationPct = 0f;
					if (!GlobalMembers.gApp.mProfile.mAffirmationSubliminal && GlobalMembers.gApp.mProfile.mAffirmationOn)
					{
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ZEN_MANTRA1, 0, GlobalMembers.M(0.3));
					}
				}
			}
			mDynamicSpeed.IncInVal();
			if (GetSelectedPiece() != null || mMoveCounter > 0)
			{
				SetTutorialCleared(11);
			}
			GlobalMembers.gApp.mProfile.mStats[2] = mPoints;
		}

		public override void ButtonDepress(int theId)
		{
			if (mGameOverCount <= GlobalMembers.M(200))
			{
				SetTutorialCleared(11);
				if (theId == 5)
				{
					((ZenOptionsMenu)GlobalMembers.gApp.mMenus[19]).Expand();
					((ZenOptionsMenu)GlobalMembers.gApp.mMenus[19]).Transition_SlideThenFadeIn();
				}
				else
				{
					base.ButtonDepress(theId);
				}
			}
		}

		public override void KeyChar(char theChar)
		{
			base.KeyChar(theChar);
		}

		public override void PlayMenuMusic()
		{
			CustomBassMusicInterface theMusicInterface =
				(CustomBassMusicInterface)(GlobalMembers.gApp.mMusicInterface);
			if (theMusicInterface.mSongName != "Zen")
			{
				theMusicInterface.QueueEvent("FadeOut", theMusicInterface.mSongName, false);
				theMusicInterface.QueueEvent("Play", "Zen", false);
			}
			// GlobalMembers.gApp.mMusic.PlaySongNoDelay(4, true);
		}

		public override void InitUI()
		{
			base.InitUI();
			if (mZenOptionsButton == null)
			{
				mZenOptionsButton = new Bej3Button(5, this, Bej3ButtonType.BUTTON_TYPE_LONG);
				mZenOptionsButton.SetLabel(GlobalMembers._ID("ZEN OPTIONS", 3442));
				AddWidget(mZenOptionsButton);
			}
		}

		public override void RefreshUI()
		{
			if (mUiConfig == EUIConfig.eUIConfig_Standard || mUiConfig == EUIConfig.eUIConfig_StandardNoReplay)
			{
				mHintButton.Resize(ConstantsWP.BOARD_UI_HINT_BTN_X, ConstantsWP.BOARD_UI_HINT_BTN_Y, ConstantsWP.BOARD_UI_HINT_BTN_WIDTH, 0);
				mHintButton.mHasAlpha = true;
				mHintButton.mDoFinger = true;
				mHintButton.mOverAlphaSpeed = 0.1;
				mHintButton.mOverAlphaFadeInSpeed = 0.2;
				mHintButton.mWidgetFlagsMod.mRemoveFlags |= 4;
				mHintButton.mDisabled = false;
				mHintButton.SetOverlayType(Bej3Button.BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_NONE);
			}
			if (mReplayButton != null)
			{
				mReplayButton.Resize(GlobalMembers.IMGRECT_S(GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON, 0f, GetBottomWidgetOffset()));
				mReplayButton.mButtonImage = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON;
				mReplayButton.mNormalRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelRect(0);
				mReplayButton.mOverImage = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON;
				mReplayButton.mOverRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelRect(1);
				mReplayButton.mDownImage = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON;
				mReplayButton.mDownRect = GlobalMembersResourcesWP.IMAGE_INGAMEUI_REPLAY_BUTTON.GetCelRect(1);
				mReplayButton.mHasAlpha = true;
				mReplayButton.mDoFinger = true;
				mReplayButton.mOverAlphaSpeed = 0.1;
				mReplayButton.mOverAlphaFadeInSpeed = 0.2;
				mReplayButton.mLabel = "";
			}
			mZenOptionsButton.Resize(ConstantsWP.ZENBOARD_UI_ZEN_BTN_X, ConstantsWP.ZENBOARD_UI_ZEN_BTN_Y, ConstantsWP.ZENBOARD_UI_ZEN_BTN_WIDTH, 0);
			mZenOptionsButton.mHasAlpha = true;
			mZenOptionsButton.mDoFinger = true;
			mZenOptionsButton.mOverAlphaSpeed = 0.1;
			mZenOptionsButton.mOverAlphaFadeInSpeed = 0.2;
			mZenOptionsButton.mWidgetFlagsMod.mRemoveFlags |= 4;
			mZenOptionsButton.mDisabled = false;
			mZenOptionsButton.SetOverlayType(Bej3Button.BUTTON_OVERLAY_TYPE.BUTTON_OVERLAY_NONE);
		}

		public override void DrawButtons(Graphics g)
		{
			if (!mIsWholeGameReplay)
			{
				g.SetDrawMode(Graphics.DrawMode.Normal);
				float mTransX = g.mTransX;
				float mTransY = g.mTransY;
				g.Translate(mZenOptionsButton.mX + (int)GlobalMembers.S(mSideXOff) + mOffsetX, mZenOptionsButton.mY + mOffsetY);
				mZenOptionsButton.Draw(g);
				g.SetColor(Color.White);
				g.mTransX = mTransX;
				g.mTransY = mTransY;
			}
			base.DrawButtons(g);
		}

		public void MuteZenSounds()
		{
			if (GlobalMembers.gApp.mProfile.mNoiseOn)
			{
				GlobalMembers.gApp.mSoundManager.SetVolume(2, 0.0);
				StopZenNoise();
			}
			if (GlobalMembers.gApp.mProfile.mBreathOn)
			{
				GlobalMembers.gApp.mSoundManager.SetVolume(4, 0.0);
			}
		}

		public void UnmuteZenSounds()
		{
			if (GlobalMembers.gApp.mProfile.mNoiseOn)
			{
				mNoiseSoundInstance.Play(true, false);
				GlobalMembers.gApp.mSoundManager.SetVolume(2, GlobalMembers.gApp.mZenAmbientVolume);
			}
			if (GlobalMembers.gApp.mProfile.mBreathOn)
			{
				GlobalMembers.gApp.mSoundManager.SetVolume(4, GlobalMembers.gApp.mZenBreathVolume);
			}
		}

		public void PlayZenNoise()
		{
			if (GlobalMembers.gApp.mMuteCount <= 0 && mNoiseSoundInstance != null && !mNoiseSoundInstance.IsPlaying())
			{
				mNoiseSoundInstance.Play(true, false);
				mNoiseSoundInstance.SetVolume(GlobalMembers.gApp.mZenAmbientVolume);
			}
		}

		public void StopZenNoise()
		{
			if (mNoiseSoundInstance != null && mNoiseSoundInstance.IsPlaying())
			{
				mNoiseSoundInstance.Stop();
			}
		}

		public void MusicVolumeChanged()
		{
			if (GlobalMembers.gApp.mProfile.mNoiseOn)
			{
				GlobalMembers.gApp.mZenAmbientMusicVolume = GlobalMembers.gApp.mMusicVolume;
			}
		}

		public override void ShowCompleted()
		{
			base.ShowCompleted();
			GlobalMembers.gApp.mMenus[2].SetVisible(false);
		}

		private static int NegDiv(int num, int den)
		{
			if (num >= 0 || num < -den)
			{
				return num / den;
			}
			return -1;
		}
	}
}
