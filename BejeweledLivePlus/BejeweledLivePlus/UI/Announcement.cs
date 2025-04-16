using System;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.UI
{
	public class Announcement : IDisposable
	{
		public Point mPos = default(Point);

		public string mText = string.Empty;

		public CurvedVal mAlpha = new CurvedVal();

		public CurvedVal mScale = new CurvedVal();

		public CurvedVal mHorzScaleMult = new CurvedVal();

		public bool mDarkenBoard;

		public bool mBlocksPlay;

		public Board mBoard;

		public Font mFont;

		public bool mGoAnnouncement;

		public bool mTimeAnnouncement;

		public Announcement(Board theBoard)
			: this(theBoard, string.Empty)
		{
		}

		public Announcement(Board theBoard, string theText)
		{
			mBoard = theBoard;
			if (mBoard != null)
			{
				mPos = new Point(mBoard.GetBoardCenterX(), mBoard.GetBoardCenterY());
				if (!mBoard.mShowBoard)
				{
					mPos.mX = GlobalMembers.S(GlobalMembers.M(800));
				}
			}
			mText = theText;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_ALPHA, mAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eANNOUNCEMENT_SCALE, mScale);
			mHorzScaleMult.SetConstant(1.0);
			mDarkenBoard = true;
			mBlocksPlay = true;
			mFont = GlobalMembersResources.FONT_HUGE;
			mGoAnnouncement = false;
			mTimeAnnouncement = false;
			if (mBoard != null)
			{
				mBoard.mAnnouncements.Add(this);
			}
		}

		public void Dispose()
		{
		}

		public void End()
		{
			if (mAlpha.GetInVal() < 0.85)
			{
				mAlpha.SetInVal(0.85);
				mScale.SetInVal(0.85);
				mHorzScaleMult.SetInVal(0.85);
			}
		}

		public virtual void Update()
		{
			mAlpha.IncInVal();
			mScale.IncInVal();
			mHorzScaleMult.IncInVal();
			if (!mAlpha.IsDoingCurve() && !mScale.IsDoingCurve())
			{
				if (mBoard != null)
				{
					if (mGoAnnouncement)
					{
						mBoard.mGoAnnouncementDone = true;
					}
					else if (mTimeAnnouncement)
					{
						mBoard.mTimeAnnouncementDone = true;
					}
					mBoard.mAnnouncements.RemoveAt(0);
					Dispose();
				}
			}
			else if (mDarkenBoard && mBoard != null)
			{
				mBoard.mBoardDarkenAnnounce = (float)(double)mAlpha;
			}
		}

		public virtual void Draw(Graphics g)
		{
			if ((double)mScale == 0.0 || mBoard.mSuspendingGame)
			{
				return;
			}
			g.PushState();
			g.SetFont(mFont);
			g.SetColor(Color.White);
			if (mBoard != null)
			{
				g.mColor.mAlpha = (int)((double)mAlpha * (double)mBoard.GetPieceAlpha() * 255.0);
			}
			else
			{
				g.mColor.mAlpha = (int)((double)mAlpha * 255.0);
			}
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, Bej3Widget.COLOR_INGAME_ANNOUNCEMENT);
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 1, Color.White);
			int num = GlobalMembers.S(mPos.mX) + ((mBoard != null) ? ((int)GlobalMembers.S((double)mBoard.mSideXOff * (double)mBoard.mSlideXScale)) : 0);
			int num2 = GlobalMembers.S(mPos.mY);
			float num3 = (float)(double)mScale;
			Utils.PushScale(g, (float)((double)num3 * (double)mHorzScaleMult), num3, num, num2);
			int num4 = 1;
			string text = mText;
			foreach (char c in text)
			{
				if (c == '\n')
				{
					num4++;
				}
			}
			int num5 = 0;
			int num6 = 0;
			for (int j = 0; j < mText.Length; j++)
			{
				if (mText[j] == '\n')
				{
					g.WriteString(mText.Substring(num6, j - num6), num, num2 - (num4 - num5) * GlobalMembers.MS(140) + GlobalMembers.MS(175));
					num6 = j + 1;
					num5++;
				}
			}
			g.WriteString(mText.Substring(num6), num, num2 - (num4 - num5) * GlobalMembers.MS(140) + GlobalMembers.MS(175));
			Utils.PopScale(g);
			g.PopState();
		}
	}
}
