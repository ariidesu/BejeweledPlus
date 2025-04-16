using System;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus.UI
{
	public class RankBarWidget : SexyFramework.Widget.Widget
	{
		private float mRankPointMultiplier;

		public bool mDrawCrown;

		public CurvedVal mRankupGlow = new CurvedVal();

		public long mDispRankPoints;

		public int mDispRank;

		public int mRankDelay;

		public bool mDrawText;

		public bool mDrawRankName;

		public Board mBoard;

		public RankUpDialog mRankUpDialog;

		public int mTimeShown;

		public bool mShown;

		public SexyFramework.Widget.Widget mPrevFocus;

		public bool mTobleroning;

		public float mTobleroningOffset;

		public int mTobleroningDirection;

		public int mTobleroningTimer;

		public bool mTobleroneWaiting;

		public int mTobleroningTarget;

		public RankBarWidget(int theWidth, Board theBoard, RankUpDialog theRankUpDialog, bool drawText)
			: this(theWidth, theBoard, theRankUpDialog, drawText, true)
		{
		}

		public RankBarWidget(int theWidth, Board theBoard, RankUpDialog theRankUpDialog)
			: this(theWidth, theBoard, theRankUpDialog, true, true)
		{
		}

		public RankBarWidget(int theWidth, Board theBoard)
			: this(theWidth, theBoard, null, true, true)
		{
		}

		public RankBarWidget(int theWidth)
			: this(theWidth, null, null, true, true)
		{
		}

		public RankBarWidget(int theWidth, Board theBoard, RankUpDialog theRankUpDialog, bool drawText, bool drawRankName)
		{
			mTimeShown = 0;
			mShown = false;
			mDrawCrown = false;
			mTobleroning = false;
			mBoard = theBoard;
			mRankUpDialog = theRankUpDialog;
			mDrawText = drawText;
			mDrawRankName = drawRankName;
			mPrevFocus = null;
			mDispRank = GlobalMembers.gApp.mProfile.mOfflineRank;
			mDispRankPoints = GlobalMembers.gApp.mProfile.mOfflineRankPoints;
			mRankDelay = 0;
			mTobleroningOffset = 0f;
			mTobleroneWaiting = true;
			mTobleroningDirection = 1;
			Resize(0, 0, theWidth, 0);
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			theHeight = GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_BG.mHeight;
			base.Resize(theX, theY, theWidth, theHeight);
		}

		public override void Update()
		{
			base.Update();
			if (!mShown)
			{
				return;
			}
			mTimeShown++;
			if (GlobalMembers.gApp.GetDialog(39) != null || mBoard == null)
			{
				return;
			}
			mRankupGlow.IncInVal();
			if (GlobalMembers.M(0) != 0 && mWidgetManager != null && mWidgetManager.mKeyDown[17])
			{
				mDispRankPoints = GlobalMembers.gApp.mProfile.mOfflineRankPoints - GlobalMembers.M(25000);
				mDispRank = (int)GlobalMembers.gApp.mProfile.GetRankAtPoints(mDispRankPoints);
			}
			if (mRankDelay > 0)
			{
				mRankDelay--;
			}
			else if (mDispRankPoints < GlobalMembers.gApp.mProfile.mOfflineRankPoints && mTimeShown >= GlobalMembers.M(150))
			{
				if (mTimeShown % GlobalMembers.M(20) == 0)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_RANK_COUNTUP);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eRANK_BAR_WIDGET_RANKUP_GLOW_ADD, mRankupGlow);
				}
				mDispRankPoints = (long)GlobalMembers.MIN(GlobalMembers.gApp.mProfile.mOfflineRankPoints, mDispRankPoints + (GlobalMembers.gApp.mProfile.mOfflineRankPoints - mDispRankPoints) / GlobalMembers.M(100) + ConstantsWP.RANKBARWIDGET_UPDATE_SPEED);
				int num = (int)GlobalMembers.gApp.mProfile.GetRankAtPoints(mDispRankPoints);
				if (num > mDispRank)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_RANKUP);
					mDispRank = num;
					mDispRankPoints = GlobalMembers.gApp.mProfile.GetRankPoints((uint)mDispRank);
					mRankDelay = GlobalMembers.M(140);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eRANK_BAR_WIDGET_RANKUP_GLOW, mRankupGlow);
					if (mRankUpDialog != null)
					{
						mRankUpDialog.DoRankUp();
					}
				}
			}
			if (mTobleroning && mRankDelay <= 0)
			{
				if (mTobleroneWaiting)
				{
					mTobleroningTimer--;
					if (mTobleroningTimer <= 0)
					{
						mTobleroningTimer = 150;
						mTobleroneWaiting = !mTobleroneWaiting;
						mTobleroningDirection = ((mTobleroningOffset != 0f) ? 1 : (-1));
						mTobleroningTarget = (int)mTobleroningOffset + mTobleroningDirection;
					}
				}
				else
				{
					float num2 = 0.01f;
					if (Math.Abs(mTobleroningOffset - (float)mTobleroningTarget) > num2)
					{
						mTobleroningOffset += num2 * (float)mTobleroningDirection;
					}
					else
					{
						mTobleroningTimer--;
						mTobleroningOffset = (int)(mTobleroningOffset + num2 * (float)mTobleroningDirection);
						if (mTobleroningTimer <= 0)
						{
							mTobleroningTimer = 150;
							mTobleroneWaiting = !mTobleroneWaiting;
						}
					}
				}
			}
			MarkDirty();
		}

		public override void Draw(Graphics g)
		{
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			g.SetColor(new Color(-1));
			int rank = GetRank();
			long rankPoints = GetRankPoints();
			long nextRankPoints = GetNextRankPoints();
			long rankPoints2 = GlobalMembers.gApp.mProfile.GetRankPoints((uint)rank);
			if (mRankDelay > 0)
			{
				rankPoints2 = GlobalMembers.gApp.mProfile.GetRankPoints((uint)(rank - 1));
			}
			float num = 0f;
			if (nextRankPoints != rankPoints2)
			{
				num = (float)GlobalMembers.MIN((double)(rankPoints - rankPoints2) / (double)(nextRankPoints - rankPoints2), 1.0);
			}
			g.DrawImageBox(new Rect(0, 0, mWidth, mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_BG);
			if (mDrawCrown)
			{
				g.DrawImage(GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_CROWN, 0, 0);
			}
			int num2 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1354) - GlobalMembersResourcesWP.ImgXOfs(1355));
			if (!mDrawCrown)
			{
				num2 -= ConstantsWP.RANKBARWIDGET_BG_OFFSET_NO_CROWN;
			}
			int num3 = mWidth - (GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_BG.mWidth - GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR.mWidth);
			if (!mDrawCrown)
			{
				num3 += ConstantsWP.RANKBARWIDGET_BG_OFFSET_NO_CROWN;
			}
			g.DrawImageBox(new Rect(num2, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_ID) - GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_BG_ID)), num3, GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR.mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR);
			num2 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_FILL_ID) - GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_BG_ID));
			num3 = 0;
			if (!mDrawCrown)
			{
				num2 -= ConstantsWP.RANKBARWIDGET_BG_OFFSET_NO_CROWN;
				num3 += ConstantsWP.RANKBARWIDGET_BG_OFFSET_NO_CROWN;
			}
			Rect theDest = new Rect(num2, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_FILL_ID) - GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_BG_ID)), num3 + mWidth - (GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_BG.mWidth - GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_FILL.mWidth), GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_FILL.mHeight);
			Rect mClipRect = g.mClipRect;
			int num4 = num3 + mWidth - (GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_BG.mWidth - GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_GLOW.mWidth);
			num4 -= ConstantsWP.RANKBARWIDGET_FILL_OFFSET * 2;
			g.ClipRect(num2, 0, (int)((float)num4 * num), mHeight);
			g.DrawImageBox(theDest, GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_FILL);
			g.mClipRect = mClipRect;
			if (mRankupGlow != null)
			{
				g.SetColorizeImages(true);
				g.SetColor(mRankupGlow);
				num2 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_GLOW_ID) - GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_BG_ID));
				if (!mDrawCrown)
				{
					num2 -= ConstantsWP.RANKBARWIDGET_BG_OFFSET_NO_CROWN;
				}
				g.DrawImageBox(new Rect(num2, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_GLOW_ID) - GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_DIALOG_PROGRESS_BAR_BG_ID)), num4 + ConstantsWP.RANKBARWIDGET_FILL_OFFSET * 2, GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_GLOW.mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_PROGRESS_BAR_GLOW);
			}
			if (!mDrawText)
			{
				return;
			}
			string rankName = GetRankName(rank, false);
			g.SetColor(new Color(-1));
			string theString = string.Format(GlobalMembers._ID("Rank: {0}", 427), rank + 1);
			float num5 = 1f;
			float num6 = g.GetFont().StringWidth(theString);
			g.SetScale(num5, num5, ConstantsWP.RANKBARWIDGET_TEXT_POS_1_X, ConstantsWP.RANKBARWIDGET_TEXT_SCALING_POS_Y);
			int num7 = mWidth / 2;
			if (mDrawCrown)
			{
				num7 += ConstantsWP.RANKBARWIDGET_TEXT_POS_6_X;
			}
			num2 = (int)((mDrawRankName && !mTobleroning) ? ((float)ConstantsWP.RANKBARWIDGET_TEXT_POS_1_X) : ((float)num7 - num6 / 2f));
			int num8 = (int)(mTobleroningOffset * (float)ConstantsWP.RANKBARWIDGET_TOBLERONE);
			if (mTobleroning)
			{
				g.mClipRect.mY += ConstantsWP.RANKBARWIDGET_TOBLERONE_CLIP_TOP;
				g.mClipRect.mHeight -= ConstantsWP.RANKBARWIDGET_TOBLERONE_CLIP_TOP + ConstantsWP.RANKBARWIDGET_TOBLERONE_CLIP_BOTTOM;
			}
			g.WriteString(theString, num2, ConstantsWP.RANKBARWIDGET_TEXT_POS_Y + num8, -1, -1);
			if (mTobleroning)
			{
				num8 += ConstantsWP.RANKBARWIDGET_TOBLERONE;
			}
			if (mDrawRankName)
			{
				int theJustification = 1;
				int num9;
				if (mTobleroning)
				{
					num9 = mWidth / 2 - g.StringWidth(rankName) / 2;
					if (mDrawCrown)
					{
						num9 += ConstantsWP.RANKBARWIDGET_TEXT_POS_6_X;
					}
					theJustification = -1;
				}
				else
				{
					num9 = mWidth - ConstantsWP.RANKBARWIDGET_TEXT_POS_4_X;
				}
				g.SetScale(num5, num5, mWidth - ConstantsWP.RANKBARWIDGET_TEXT_POS_4_X, ConstantsWP.RANKBARWIDGET_TEXT_SCALING_POS_Y);
				g.WriteString(rankName, num9, ConstantsWP.RANKBARWIDGET_TEXT_POS_Y + num8, -1, theJustification);
			}
			g.SetScale(1f, 1f, 0f, 0f);
		}

		public int GetRank()
		{
			if (mBoard != null)
			{
				return mDispRank;
			}
			return GlobalMembers.gApp.mProfile.mOfflineRank;
		}

		public long GetRankPoints()
		{
			if (mBoard != null)
			{
				return mDispRankPoints;
			}
			return GlobalMembers.gApp.mProfile.mOfflineRankPoints;
		}

		public long GetNextRankPoints()
		{
			if (mRankDelay > 0)
			{
				return GlobalMembers.gApp.mProfile.GetRankPoints((uint)GetRank());
			}
			return GlobalMembers.gApp.mProfile.GetRankPoints((uint)(GetRank() + 1));
		}

		public string GetRankName(int aRank, bool includeRankNumber)
		{
			return GlobalMembers.gApp.mRankNames[GlobalMembers.MIN(aRank, Common.size(GlobalMembers.gApp.mRankNames) - 1)];
		}

		public long GetRankPointsRemaining()
		{
			return (long)((float)(GetNextRankPoints() - GetRankPoints()) * mRankPointMultiplier + 999f) / 1000;
		}

		public void Shown(Board theBoard)
		{
			mBoard = theBoard;
			if (theBoard != null)
			{
				mRankPointMultiplier = theBoard.GetRankPointMultiplier();
			}
			mTimeShown = 0;
			mShown = true;
			mDispRank = GlobalMembers.gApp.mProfile.mOfflineRank;
			mDispRankPoints = GlobalMembers.gApp.mProfile.mOfflineRankPoints;
			mRankDelay = 0;
			mTobleroningOffset = 0f;
			mTobleroneWaiting = true;
			mTobleroningDirection = 1;
			mTobleroningTimer = 300;
		}

		public void Hidden()
		{
			mShown = false;
		}

		public bool FinishedRankUp()
		{
			return mDispRankPoints >= GlobalMembers.gApp.mProfile.mOfflineRankPoints;
		}

		public override void MouseEnter()
		{
			base.MouseEnter();
			if (GlobalMembers.gApp.mDebugKeysEnabled)
			{
				mPrevFocus = GlobalMembers.gApp.mWidgetManager.mFocusWidget;
				GlobalMembers.gApp.mWidgetManager.SetFocus(this);
			}
		}

		public override void MouseLeave()
		{
			base.MouseLeave();
			if (GlobalMembers.gApp.mDebugKeysEnabled && mPrevFocus != null)
			{
				GlobalMembers.gApp.mWidgetManager.SetFocus(mPrevFocus);
			}
		}

		public override void KeyChar(char theChar)
		{
		}
	}
}
