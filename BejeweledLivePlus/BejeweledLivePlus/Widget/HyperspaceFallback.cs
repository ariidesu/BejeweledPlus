using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Widget
{
	internal class HyperspaceFallback : Hyperspace
	{
		public Board mBoard;

		public CrystalBall mFromBall;

		public CrystalBall mToBall;

		public CurvedVal mFromCenterPct = new CurvedVal();

		public CurvedVal mToCenterPct = new CurvedVal();

		public int mDelay;

		private bool mWidgetsAdded;

		public HyperspaceFallback(Board theBoard)
		{
			mBoard = theBoard;
			mFromBall = new CrystalBall("", "", "", 0, null, Bej3Widget.COLOR_CRYSTALBALL_FONT);
			mToBall = new CrystalBall("", "", "", 0, null, Bej3Widget.COLOR_CRYSTALBALL_FONT);
			mFromBall.mWidth = 0;
			mFromBall.mHeight = 0;
			mToBall.mWidth = 0;
			mToBall.mHeight = 0;
			mDelay = GlobalMembers.M(0);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eHYPERSPACE_FROM_CENTER_PCT, mFromCenterPct);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eHYPERSPACE_TO_CENTER_PCT, mToCenterPct, mFromCenterPct);
			mFromBall.mFullPct.SetCurve(GlobalMembers.MP("b+0,1,0.003333,1,~###x~###   ]####     J####"), mFromCenterPct);
			mFromBall.mScale.SetCurve(GlobalMembers.MP("b+0,1,0,1,~###|~###  @/k=] 3####     R####"), mFromCenterPct);
			mToBall.mFullPct.SetCurve(GlobalMembers.MP("b+0,1,0.003333,1,####    i####     8~###"), mFromCenterPct);
			mToBall.mScale.SetCurve(GlobalMembers.MP("b+0,1,0.003333,1,####   d####      =~###"), mFromCenterPct);
		}

		public override void Dispose()
		{
		}

		public override void Update()
		{
			base.Update();
			if (mDelay > 0)
			{
				mDelay--;
				return;
			}
			if (!mWidgetsAdded)
			{
				if (mWidgetManager != null)
				{
					mWidgetManager.AddWidget(mFromBall);
					mWidgetManager.AddWidget(mToBall);
				}
				mWidgetsAdded = true;
			}
			if (!mFromCenterPct.IncInVal())
			{
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_Finish);
			}
			if (mUpdateCnt == GlobalMembers.M(200))
			{
				mBoard.HyperspaceEvent(HYPERSPACEEVENT.HYPERSPACEEVENT_OldLevelClear);
				mToBall.mImage = mBoard.mBackground.GetBackgroundImage();
			}
			if (mUpdateCnt == GlobalMembers.M(250))
			{
				mBoard.RandomizeBoard();
			}
			mFromBall.Update();
			mToBall.Update();
		}

		public override float GetPieceAlpha()
		{
			return mBoard.GetPieceAlpha();
		}

		public override void Draw(Graphics g)
		{
			FPoint[] array = new FPoint[2]
			{
				new FPoint(mWidth / 2, mHeight / 2),
				new FPoint(mWidth / 2, mHeight / 2)
			};
			CrystalBall[] array2 = new CrystalBall[2] { mFromBall, mToBall };
			float[] array3 = new float[2]
			{
				(float)(double)mFromCenterPct,
				(float)(double)mToCenterPct
			};
			for (int i = GlobalMembers.M(1); i < 2; i++)
			{
				g.PushState();
				FPoint fPoint = new FPoint(mWidth / 2, mHeight / 2) * array3[i] + array[i] * (1f - array3[i]);
				g.Translate((int)fPoint.mX, (int)fPoint.mY);
				array2[i].mOffset = fPoint - new FPoint((int)fPoint.mX, (int)fPoint.mY);
				array2[i].Draw(g);
				g.PopState();
			}
		}

		public override void DrawBackground(Graphics g)
		{
		}

		public override bool IsUsing3DTransition()
		{
			return false;
		}

		public override void SetBGImage(SharedImageRef inImage)
		{
			mFromBall.mImage = new SharedImageRef(inImage);
		}
	}
}
