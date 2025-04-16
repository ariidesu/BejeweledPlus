using System;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class GoldCollectEffect : IDisposable
	{
		public bool mLayerOnTop;

		public float mLayerOnTopSwitchPct;

		public BSpline mSpline = new BSpline();

		public CurvedVal mSplineInterp = new CurvedVal();

		public CurvedVal mAlphaOut = new CurvedVal();

		public CurvedVal mScaleCv = new CurvedVal();

		public CurvedVal mPointBlinkCv = new CurvedVal();

		public CurvedVal mStreamerMag = new CurvedVal();

		public CurvedVal mParticleEmitOverTime = new CurvedVal();

		public PIEffect mSparkles;

		public int mGlowRGB;

		public int mGlowRGB2;

		public int mUpdateCnt;

		public DigGoal mGoal;

		public bool mDeleteMe;

		public double mX;

		public double mY;

		public int mImageId;

		public int mSrcImageId;

		public int mGlowImageId;

		public DigGoal.TileData mTileData = new DigGoal.TileData();

		public bool mCentering;

		public Point mStartPoint = default(Point);

		public Point mTargetPoint = default(Point);

		public Point mLastPoint = default(Point);

		public double mLastRotation;

		public double mStopGlowAtPct;

		public float mTimeMod;

		public bool mAddedPoints;

		public bool mIsNugget;

		public int mExtraSplineTime;

		public int mStartedAtTick;

		public int mVal;

		public ETreasureType mTreasureType;

		public int mDisplayVal;

		public string mDisplayName = string.Empty;

		public double mExtraScaling;

		public bool mUseBaseSparkles;

		private static SexyVertex2D[,] DrawStreamer_tris = new SexyVertex2D[2, 3];

		public GoldCollectEffect(DigGoal theGoal, DigGoal.TileData theData)
		{
			mGoal = theGoal;
			mTileData = new DigGoal.TileData(theData);
			mTreasureType = ETreasureType.eTreasure_Gold;
			mLayerOnTop = false;
			mSparkles = null;
			mVal = 0;
			mImageId = -1;
			mSrcImageId = -1;
			mUpdateCnt = 0;
			mLastRotation = 0.0;
			mCentering = false;
			mTimeMod = 0f;
			mAddedPoints = false;
			mDeleteMe = false;
			mIsNugget = false;
			mDisplayVal = 0;
			mGlowImageId = -1;
			mExtraScaling = 1.0;
			mStopGlowAtPct = 1.0;
			mGlowRGB = 16777215;
			mGlowRGB2 = 16777215;
			mUseBaseSparkles = true;
			mExtraSplineTime = 0;
			mStartedAtTick = theGoal.mQuestBoard.mGameTicks;
			mParticleEmitOverTime.SetConstant(1.0);
			mPointBlinkCv.SetConstant(0.0);
			mScaleCv.SetConstant(1.0);
		}

		public void Dispose()
		{
			if (mSparkles != null)
			{
				mSparkles.Dispose();
			}
		}

		public void Init()
		{
			mLastPoint = mStartPoint;
			mX = mStartPoint.mX;
			mY = mStartPoint.mY;
			if (mCentering)
			{
				mExtraSplineTime += (int)(mSplineInterp.mInMax * (double)mTimeMod * 100.0);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SPLINE_INTERP_ARTIFACT, mSplineInterp);
				mSplineInterp.SetInRange(0.0, mSplineInterp.mInMax + (double)((float)mExtraSplineTime / 100f));
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SCALE_CV_ARTIFACT, mScaleCv);
				mScaleCv.SetInRange(0.0, mScaleCv.mInMax);
				if (mDisplayVal > 0)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_POINT_BLINK_CV, mPointBlinkCv);
					mPointBlinkCv.SetInRange(0.0, mPointBlinkCv.mInMax);
				}
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_STREAMER_MAG, mStreamerMag);
				mStreamerMag.SetInRange(0.0, mStreamerMag.mInMax);
				Image imageById = GlobalMembersResourcesWP.GetImageById(mSrcImageId);
				Image imageById2 = GlobalMembersResourcesWP.GetImageById(mImageId);
				mScaleCv.mOutMin = (double)imageById.GetWidth() / (double)imageById2.GetWidth();
			}
			else
			{
				mExtraSplineTime += (int)(mSplineInterp.mInMax * (double)mTimeMod * 100.0);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SPLINE_INTERP, mSplineInterp);
				mSplineInterp.SetInRange(0.0, mSplineInterp.mInMax + (double)((float)mExtraSplineTime / 100f));
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SCALE_CV, mScaleCv);
				mScaleCv.SetInRange(0.0, mScaleCv.mInMax + (double)((float)mExtraSplineTime / 100f));
				mSpline.AddPoint((float)mX, (float)mY);
				if (mIsNugget)
				{
					mLayerOnTopSwitchPct = GlobalMembers.M(1f);
				}
				else if (BejeweledLivePlus.Misc.Common.Rand() % 2 == 0)
				{
					mLayerOnTopSwitchPct = GlobalMembers.M(0.78f);
				}
				else
				{
					mLayerOnTopSwitchPct = GlobalMembers.M(0.78f);
				}
				mSpline.AddPoint(mTargetPoint.mX, mTargetPoint.mY);
				mSpline.CalculateSpline();
			}
			mSparkles = (mUseBaseSparkles ? GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_COLLECT_BASE.Duplicate() : GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_COLLECT_GOLD.Duplicate());
			mSparkles.mDrawTransform.LoadIdentity();
			mSparkles.mDrawTransform.Scale(GlobalMembers.S(1f), GlobalMembers.S(1f));
			mSparkles.mEmitAfterTimeline = true;
		}

		public double CalcRotation()
		{
			if (mCentering)
			{
				return 0.0;
			}
			if (!mSplineInterp.HasBeenTriggered())
			{
				double num = Math.Atan2((double)mLastPoint.mY - mY, mX - (double)mLastPoint.mX);
				double num2 = num - mLastRotation;
				num2 = ((num2 < 0.0) ? (-1.0) : 1.0) * Math.Min(GlobalMembers.M(0.03), Math.Abs(num2));
				mLastRotation += num2;
				mLastPoint.mX = (int)mX;
				mLastPoint.mY = (int)mY;
			}
			return mLastRotation;
		}

		public void Update()
		{
			mUpdateCnt++;
			if (!mCentering || (mUpdateCnt > mExtraSplineTime && mSplineInterp.mInMax - mSplineInterp.mInVal <= GlobalMembers.M(0.15)))
			{
				mPointBlinkCv.IncInVal();
				mStreamerMag.IncInVal();
				mAlphaOut.IncInVal();
			}
			mScaleCv.IncInVal();
			if (mCentering)
			{
				mX = (double)mStartPoint.mX + (double)mSplineInterp * (double)(GlobalMembers.RS(ConstantsWP.DIG_BOARD_ITEM_DEST_X) - mStartPoint.mX);
				mY = (double)mStartPoint.mY + (double)mSplineInterp * (double)(GlobalMembers.RS(ConstantsWP.DIG_BOARD_ITEM_DEST_Y) - mStartPoint.mY);
				double num = mSplineInterp;
				mSplineInterp.IncInVal();
				if (num < GlobalMembers.M(0.95) && (double)mSplineInterp >= GlobalMembers.M(0.95))
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_ARTIFACT_SHOWCASE, 0, GlobalMembers.M(1.0));
				}
				if (mStreamerMag.mInVal / mStreamerMag.mInMax > GlobalMembers.M(0.9))
				{
					mExtraSplineTime = 0;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SPLINE_INTERP, mSplineInterp);
					mCentering = false;
					mSpline.AddPoint((float)mX, (float)mY);
					mLayerOnTopSwitchPct = GlobalMembers.M(0.75f);
					mSpline.AddPoint(mTargetPoint.mX, mTargetPoint.mY);
					mSpline.CalculateSpline();
					double mOutMin = mScaleCv.mOutMin;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SCALE_CV_UPDATE, mScaleCv);
					mScaleCv.mOutMin = mOutMin * mExtraScaling;
				}
			}
			else
			{
				mX = mSpline.GetXPoint((float)((double)mSplineInterp * (double)mSpline.GetMaxT()));
				mY = mSpline.GetYPoint((float)((double)mSplineInterp * (double)mSpline.GetMaxT()));
			}
			mLayerOnTop = mCentering || (double)mSplineInterp < (double)mLayerOnTopSwitchPct;
			double num2 = GlobalMembers.M(0.75);
			if ((double)mLayerOnTopSwitchPct < 1.0)
			{
				num2 = mLayerOnTopSwitchPct;
			}
			if (mSparkles != null)
			{
				if ((double)mSplineInterp > num2 && !mCentering)
				{
					double num3 = ((!(num2 >= mStopGlowAtPct)) ? Math.Max(0.0, 1.0 - ((double)mSplineInterp - num2) / (mStopGlowAtPct - num2)) : 0.0);
					mSparkles.GetLayer(GlobalMembers.M(0)).mColor = new Color(mGlowRGB, (int)(255.0 * num3));
					mSparkles.GetLayer(GlobalMembers.M(1)).GetEmitter(0).mNumberScale = (float)num3;
				}
				else
				{
					mSparkles.GetLayer(GlobalMembers.M(0)).mColor = new Color(mGlowRGB);
					mSparkles.GetLayer(GlobalMembers.M(1)).GetEmitter(0).mNumberScale = 1f;
				}
				if (!mUseBaseSparkles)
				{
					mSparkles.GetLayer(GlobalMembers.M(1)).mColor = new Color(mGlowRGB2);
				}
				if (!mCentering)
				{
					mSparkles.GetLayer(GlobalMembers.M(1)).GetEmitter(0).mNumberScale *= (float)mParticleEmitOverTime.GetOutVal(mSplineInterp);
				}
				if (!GlobalMembers.gApp.Is3DAccelerated())
				{
					mSparkles.GetLayer(GlobalMembers.M(1)).GetEmitter(0).mNumberScale *= GlobalMembers.M(0.5f);
				}
				mSparkles.mEmitterTransform.LoadIdentity();
				mSparkles.mEmitterTransform.Scale((float)GlobalMembers.S(mScaleCv), (float)GlobalMembers.S(mScaleCv));
				mSparkles.mEmitterTransform.Translate((float)(mX + (double)GlobalMembers.M(-30)), (float)(mY + (double)GlobalMembers.M(-20)));
				mSparkles.Update();
			}
			if (mCentering)
			{
				return;
			}
			if (!mSplineInterp.IncInVal())
			{
				if (mSparkles != null)
				{
					mSparkles.mEmitAfterTimeline = false;
					mSparkles.mFrameNum = mSparkles.mLastFrameNum - 1;
				}
				mAlphaOut.mAppUpdateCountSrc = mGoal.mUpdateCnt;
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_ALPHA_OUT, mAlphaOut);
			}
			if (!mAddedPoints && (!mSplineInterp.IsDoingCurve() || mSplineInterp.GetInVal() / mSplineInterp.mInMax > GlobalMembers.M(0.85)))
			{
				mGoal.GoldAnimDoPoints(mTreasureType, mVal);
				mAddedPoints = true;
			}
			if (mAlphaOut.IsDoingCurve())
			{
				if (mAlphaOut.IncInVal())
				{
				}
			}
			else
			{
				if (!mAlphaOut.HasBeenTriggered())
				{
					return;
				}
				if (mSparkles != null)
				{
					if (!mSparkles.IsActive() || mSparkles.mCurNumParticles == GlobalMembers.M(1))
					{
						mDeleteMe = true;
					}
					else if (mUpdateCnt > GlobalMembers.M(1000))
					{
						mDeleteMe = true;
					}
				}
				else
				{
					mDeleteMe = true;
				}
			}
		}

		public void Draw(Graphics g, int pass)
		{
			g.PushState();
			if (pass == 0)
			{
				if (mSparkles != null)
				{
					mSparkles.Draw(g);
				}
				return;
			}
			Image imageById = GlobalMembersResourcesWP.GetImageById(mImageId);
			if (mGlowImageId != -1 && (double)mStreamerMag > 0.0)
			{
				DrawStreamer(g);
				Image imageById2 = GlobalMembersResourcesWP.GetImageById(mGlowImageId);
				g.PushState();
				g.SetColor(new Color(GlobalMembers.M(16777215), (int)((double)GlobalMembers.M(255) * ((double)mStreamerMag / mStreamerMag.mOutMax))));
				g.SetColorizeImages(true);
				int num = 0;
				int num2 = 0;
				Utils.MyDrawImageRotated(g, imageById2, (float)GlobalMembers.S(mX + (double)num), (float)GlobalMembers.S(mY + (double)num2), CalcRotation(), (float)(double)mScaleCv, (float)(double)mScaleCv);
				g.PopState();
			}
			bool linearBlend = g.GetLinearBlend();
			if (imageById == GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_ITEM_DMGEM_BIG)
			{
				g.SetLinearBlend(false);
			}
			Utils.MyDrawImageRotated(g, imageById, (float)GlobalMembers.S(mX), (float)GlobalMembers.S(mY), CalcRotation(), (float)(double)mScaleCv, (float)(double)mScaleCv);
			g.SetLinearBlend(linearBlend);
			if (!mLayerOnTop)
			{
				g.ClearClipRect();
			}
			if ((double)mPointBlinkCv > 0.0)
			{
				if ((double)mPointBlinkCv < 1.0)
				{
					g.SetColorizeImages(true);
					g.SetColor(new Color(GlobalMembers.M(16777215), (int)((double)GlobalMembers.M(255) * (double)mPointBlinkCv)));
				}
				g.SetColor(new Color(-1));
				g.SetFont(GlobalMembersResources.FONT_HUGE);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 0, Bej3Widget.COLOR_DIGBOARD_SCORE_GLOW);
				Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 1, Bej3Widget.COLOR_DIGBOARD_SCORE_STROKE);
				int num3 = (int)(mY + (double)ConstantsWP.DIG_BOARD_ITEM_DESC_OFFSET_Y);
				float mScaleX = g.mScaleX;
				float mScaleY = g.mScaleY;
				string theString = $"+{SexyFramework.Common.CommaSeperate(mDisplayVal)}";
				GlobalMembersResources.FONT_HUGE.StringWidth(theString);
				int num4 = (int)GlobalMembers.S(mX);
				int num5 = GlobalMembers.S(num3 + GlobalMembers.M(5)) + imageById.GetHeight() / 2;
				g.SetScale(ConstantsWP.DIG_BOARD_FLOATING_SCORE_SCALE, ConstantsWP.DIG_BOARD_FLOATING_SCORE_SCALE, num4, num5);
				g.WriteString(theString, num4, num5);
				g.mScaleX = mScaleX;
				g.mScaleY = mScaleY;
				g.SetColorizeImages(false);
			}
			g.PopState();
		}

		public void DrawStreamer(Graphics g)
		{
			g.PushState();
			g.Translate((int)GlobalMembers.S(mX), (int)GlobalMembers.S(mY));
			g.SetColorizeImages(true);
			double num = (double)mStreamerMag * GlobalMembers.M(1.0);
			double num2 = (double)mStreamerMag * GlobalMembers.M(1.0);
			double num3 = 6.2831854820251465 * ((double)(mUpdateCnt % GlobalMembers.M(501)) / GlobalMembers.M(500.0));
			double num4 = 6.2831854820251465 / GlobalMembers.M(20.0);
			for (int i = 0; i <= GlobalMembers.M(1); i++)
			{
				double num5 = 0.0;
				Image iMAGE_QUEST_DIG_STREAK = GlobalMembersResourcesWP.IMAGE_QUEST_DIG_STREAK;
				if (i == 0)
				{
					g.SetColor(new Color(GlobalMembers.M(16763904), GlobalMembers.M(255)));
					num5 = GlobalMembers.M(0.215);
				}
				else
				{
					g.SetColor(new Color(GlobalMembers.M(16776960), GlobalMembers.M(255)));
				}
				int num6 = 0;
				for (double num7 = num3; num7 < num3 + 6.2831854820251465; num7 += num4)
				{
					num6++;
					if (num6 % 2 != 0)
					{
						continue;
					}
					double num8 = ((i == 0) ? num : num2);
					float theNum = (float)(num8 * Math.Cos(num7 - num5));
					float theNum2 = (float)(num8 * Math.Sin(num7 - num5));
					float theNum3 = (float)(num8 * Math.Cos(num7 + num4 / 2.0));
					float theNum4 = (float)(num8 * Math.Sin(num7 + num4 / 2.0));
					float theNum5 = (float)(num8 * Math.Cos(num7 + num4 + num5));
					float theNum6 = (float)(num8 * Math.Sin(num7 + num4 + num5));
					float theNum7 = 0f;
					float theNum8 = 0f;
					DrawStreamer_tris[0, 0].x = GlobalMembers.S(theNum7);
					DrawStreamer_tris[0, 0].y = GlobalMembers.S(theNum8);
					DrawStreamer_tris[0, 0].u = GlobalMembers.M(0.5f);
					DrawStreamer_tris[0, 0].v = GlobalMembers.M(0f);
					DrawStreamer_tris[0, 1].x = GlobalMembers.S(theNum3);
					DrawStreamer_tris[0, 1].y = GlobalMembers.S(theNum4);
					DrawStreamer_tris[0, 1].u = GlobalMembers.M(0.5f);
					DrawStreamer_tris[0, 1].v = GlobalMembers.M(1f);
					DrawStreamer_tris[0, 2].x = GlobalMembers.S(theNum);
					DrawStreamer_tris[0, 2].y = GlobalMembers.S(theNum2);
					DrawStreamer_tris[0, 2].u = GlobalMembers.M(0f);
					DrawStreamer_tris[0, 2].v = GlobalMembers.M(1f);
					DrawStreamer_tris[1, 0].x = GlobalMembers.S(theNum7);
					DrawStreamer_tris[1, 0].y = GlobalMembers.S(theNum8);
					DrawStreamer_tris[1, 0].u = GlobalMembers.M(0.5f);
					DrawStreamer_tris[1, 0].v = GlobalMembers.M(0f);
					DrawStreamer_tris[1, 1].x = GlobalMembers.S(theNum3);
					DrawStreamer_tris[1, 1].y = GlobalMembers.S(theNum4);
					DrawStreamer_tris[1, 1].u = GlobalMembers.M(0.5f);
					DrawStreamer_tris[1, 1].v = GlobalMembers.M(1f);
					DrawStreamer_tris[1, 2].x = GlobalMembers.S(theNum5);
					DrawStreamer_tris[1, 2].y = GlobalMembers.S(theNum6);
					DrawStreamer_tris[1, 2].u = GlobalMembers.M(1f);
					DrawStreamer_tris[1, 2].v = GlobalMembers.M(1f);
					if (GlobalMembers.M(0) != 0)
					{
						g.PushState();
						g.SetColor(new Color(-1));
						for (int j = 0; j < 2; j++)
						{
							for (int k = 0; k <= 3; k++)
							{
								g.DrawLine((int)DrawStreamer_tris[j, k].x, (int)DrawStreamer_tris[j, k].y, (int)DrawStreamer_tris[j, (k + 1) % 3].x, (int)DrawStreamer_tris[j, (k + 1) % 3].y);
							}
						}
						g.PopState();
					}
					g.DrawTrianglesTex(iMAGE_QUEST_DIG_STREAK, DrawStreamer_tris, 2);
				}
			}
			g.PopState();
		}
	}
}
