using System.Collections.Generic;
using BejeweledLivePlus.UI;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class HighScoresContainer : SexyFramework.Widget.Widget, Bej3ScrollWidgetListener, ScrollWidgetListener
	{
		private List<Label> mEntryNumberLabels = new List<Label>();

		private List<Label> mPointLabels = new List<Label>();

		private List<HighScoreLabel> mNameLabels = new List<HighScoreLabel>();

		public HighScoreTable mScoreTable;

		private int mHighlightedRow;

		public int mMaxNameWidth;

		public int mUserPosition;

		public HighScoresMenuContainer mMenu;

		private int mNewWidth;

		private bool mSkipFirstReadLR = true;

		private static Color lineColour = new Color(252, 203, 153);

		private static Color rowColour1 = new Color(173, 120, 75);

		private static Color rowColour2 = new Color(204, 137, 80);

		private static Color hiLColour = new Color(247, 175, 115);

		private bool mNeedDrawLoadingWheel;

		private int mLoadingWheelDrawIndex;

		private static Image[] mLoadingImages = new Image[12]
		{
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_01,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_02,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_03,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_04,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_05,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_06,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_07,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_08,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_09,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_10,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_11,
			GlobalMembersResourcesWP.IMAGE_LR_LOADING_12
		};

		public HighScoresContainer(int width)
		{
			mScoreTable = null;
			mHighlightedRow = -1;
			mMaxNameWidth = ConstantsWP.HIGHSCORESWIDGET_NAME_WIDTH;
		}

		private void ClearList()
		{
			if (mEntryNumberLabels.Count != 0)
			{
				for (int i = 0; i < mEntryNumberLabels.Count; i++)
				{
					RemoveWidget(mEntryNumberLabels[i]);
					RemoveWidget(mPointLabels[i]);
					RemoveWidget(mNameLabels[i]);
				}
				mEntryNumberLabels.Clear();
				mPointLabels.Clear();
				mNameLabels.Clear();
			}
		}

		private void CreateList()
		{
			int num = 0;
			foreach (HighScoreEntryLive item in mScoreTable.mHighScoresLive)
			{
				num++;
				Label label = new Label(GlobalMembersResources.FONT_DIALOG, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
				label.SetLayerColor(0, Bej3Widget.COLOR_DIALOG_WHITE);
				label.SetText(num + ".");
				mEntryNumberLabels.Add(label);
				AddWidget(label);
				HighScoreLabel highScoreLabel = new HighScoreLabel(GlobalMembersResources.FONT_DIALOG, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_LEFT);
				highScoreLabel.SetLayerColor(0, Bej3Widget.COLOR_DIALOG_WHITE);
				highScoreLabel.SetText(item.mName);
				mNameLabels.Add(highScoreLabel);
				highScoreLabel.mMaxScollWidth = 340;
				AddWidget(highScoreLabel);
				Label label2 = new Label(GlobalMembersResources.FONT_DIALOG, Label_Alignment_Horizontal.LABEL_ALIGNMENT_HORIZONTAL_RIGHT);
				label2.SetLayerColor(0, Bej3Widget.COLOR_DIALOG_2_FILL);
				label2.SetText(item.mScore.ToString());
				mPointLabels.Add(label2);
				AddWidget(label2);
			}
			ChangeWidth(mNewWidth);
		}

		public void LinkUpAssets()
		{
		}

		public void ChangeWidth(int theNewWidth)
		{
			if (mScoreTable != null && mScoreTable.mHighScoresLive != null)
			{
				theNewWidth = 610;
				int count = mScoreTable.mHighScoresLive.Count;
				Resize(0, 0, theNewWidth, count * ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT - ConstantsWP.HIGHSCORESWIDGET_CONTAINER_OFFSET_Y * 2);
				if (mParent != null)
				{
					ScrollWidget scrollWidget = mParent as ScrollWidget;
					scrollWidget.ClientSizeChanged();
				}
				int hIGHSCORESWIDGET_ITEM_TEXT_OFFSET = ConstantsWP.HIGHSCORESWIDGET_ITEM_TEXT_OFFSET;
				for (int i = 0; i < count; i++)
				{
					mEntryNumberLabels[i].Resize(ConstantsWP.HIGHSCORESWIDGET_ENTRYNUMBER_X, ConstantsWP.HIGHSCORESWIDGET_ENTRYNUMBER_Y + ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT * i + hIGHSCORESWIDGET_ITEM_TEXT_OFFSET, 0, 0);
					mNameLabels[i].Resize(ConstantsWP.HIGHSCORESWIDGET_NAME_X - 70, ConstantsWP.HIGHSCORESWIDGET_NAME_Y + ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT * i + hIGHSCORESWIDGET_ITEM_TEXT_OFFSET, 0, 0);
					mPointLabels[i].Resize(mWidth - ConstantsWP.HIGHSCORESWIDGET_POINTS_X, ConstantsWP.HIGHSCORESWIDGET_POINTS_Y + ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT * i + hIGHSCORESWIDGET_ITEM_TEXT_OFFSET, 0, 0);
				}
			}
		}

		public void SelectModeView(HighScoresMenuContainer.HSMODE m)
		{
			HighScoreTable.HighScoreTableTime t = ((mMenu != null) ? mMenu.mCurrentDisplayView : HighScoreTable.HighScoreTableTime.TIME_RECENT);
			mScoreTable.ReadLeaderboard(t);
		}

		public void SetMode(string modeName)
		{
			mScoreTable = GlobalMembers.gApp.mHighScoreMgr.GetOrCreateTable(modeName);
			if (mSkipFirstReadLR)
			{
				mSkipFirstReadLR = false;
			}
			HighScoreTable.HighScoreTableTime t = ((mMenu != null) ? mMenu.mCurrentDisplayView : HighScoreTable.HighScoreTableTime.TIME_RECENT);
			mScoreTable.ReadLeaderboard(t);
		}

		public void ReadLeaderBoard(HighScoreTable.HighScoreTableTime t)
		{
			if (mScoreTable != null)
			{
				mScoreTable.ReadLeaderboard(t);
			}
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}

		public virtual void PageChanged(Bej3ScrollWidget scrollWidget, int pageH, int pageV)
		{
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, theHeight);
			if (theY > 0)
			{
				base.Resize(theX, theY, theWidth, theHeight);
			}
		}

		public override void Update()
		{
			base.Update();
			if (mScoreTable == null)
			{
				return;
			}
			UpdateLoadingWheel();
			HighScoreTable.LRState mLRState = mScoreTable.mLRState;
			if (GlobalMembers.mByAllTimeButton != null && GlobalMembers.mByTodayButton != null)
			{
				bool isLeaderboardLoading = GlobalMembers.isLeaderboardLoading;
				GlobalMembers.mByAllTimeButton.SetDisabled(isLeaderboardLoading);
				GlobalMembers.mByTodayButton.SetDisabled(isLeaderboardLoading);
			}
			switch (mLRState)
			{
			case HighScoreTable.LRState.LR_Idle:
				mNeedDrawLoadingWheel = false;
				break;
			case HighScoreTable.LRState.LR_Loading:
				mNeedDrawLoadingWheel = true;
				ClearList();
				break;
			case HighScoreTable.LRState.LR_Ready:
				mNeedDrawLoadingWheel = false;
				ClearList();
				CreateList();
				mScoreTable.mLRState = HighScoreTable.LRState.LR_Idle;
				break;
			case HighScoreTable.LRState.LR_Error:
			{
				mNeedDrawLoadingWheel = false;
				Dialog dialog = GlobalMembers.gApp.DoXBLErrorDialog();
				mScoreTable.mLRState = HighScoreTable.LRState.LR_Idle;
				dialog.mDialogListener = GlobalMembers.gApp;
				if (GlobalMembers.gApp.mBoard != null && GlobalMembers.gApp.mInterfaceState == InterfaceState.INTERFACE_STATE_GAMEDETAILMENU)
				{
					GlobalMembers.gApp.mBoard.mVisPausePct = 0f;
				}
				break;
			}
			}
		}

		public override void Draw(Graphics g)
		{
			switch (mScoreTable.mLRState)
			{
			case HighScoreTable.LRState.LR_Idle:
			case HighScoreTable.LRState.LR_Ready:
			{
				g.mTransY -= ConstantsWP.HIGHSCORESWIDGET_CONTAINER_OFFSET_Y;
				g.mClipRect.mY = mParent.GetAbsPos().mY;
				g.mClipRect.mHeight = mParent.mHeight;
				int count = mScoreTable.mHighScoresLive.Count;
				Rect rect = new Rect(ConstantsWP.LISTBOX_DIVIDER_OFFSET_1, 0, mWidth - ConstantsWP.LISTBOX_DIVIDER_OFFSET_1 * 2, ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT);
				for (int i = 0; i < count; i++)
				{
					if (i == mHighlightedRow)
					{
						g.SetColor(hiLColour);
					}
					else if (i % 2 == 0)
					{
						g.SetColor(rowColour1);
					}
					else
					{
						g.SetColor(rowColour2);
					}
					g.FillRect(rect.mX, rect.mY, rect.mWidth, rect.mHeight);
					rect.mY += ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT;
				}
				rect = new Rect(ConstantsWP.LISTBOX_LINE_OFFSET_1, -ConstantsWP.LISTBOX_DIVIDER_OFFSET_2 / 2, mWidth - ConstantsWP.LISTBOX_LINE_OFFSET_1 * 2, ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT);
				g.SetColor(lineColour);
				for (int j = 0; j < count; j++)
				{
					g.FillRect(rect.mX, rect.mY, rect.mWidth, ConstantsWP.LISTBOX_LINE_HEIGHT);
					rect.mY += ConstantsWP.HIGHSCORESWIDGET_ITEM_HEIGHT;
					if (j % 2 == 0)
					{
						rect.mY += ConstantsWP.HIGHSCORESWIDGET_ITEM_LINE_OFFSET_ODD;
					}
					else
					{
						rect.mY -= ConstantsWP.HIGHSCORESWIDGET_ITEM_LINE_OFFSET_ODD;
					}
				}
				g.SetColor(lineColour);
				g.FillRect(rect.mX, rect.mY, rect.mWidth, ConstantsWP.LISTBOX_LINE_HEIGHT_2);
				g.SetColor(Color.White);
				Bej3Widget.DrawImageBoxTileCenter(g, new Rect(ConstantsWP.LISTBOX_SHADOW_X, ConstantsWP.LISTBOX_SHADOW_Y - mY, mWidth - ConstantsWP.LISTBOX_SHADOW_X * 2, mParent.mHeight - ConstantsWP.LISTBOX_SHADOW_Y + ConstantsWP.LISTBOX_SHADOW_Y_BOTTOM), GlobalMembersResourcesWP.IMAGE_DIALOG_LISTBOX_SHADOW);
				break;
			}
			default:
			{
				int num = 3;
				break;
			}
			case HighScoreTable.LRState.LR_Loading:
				break;
			}
			DrawLoadingWheel(g);
		}

		private void UpdateLoadingWheel()
		{
			if (mNeedDrawLoadingWheel && mUpdateCnt % 15 == 0)
			{
				mLoadingWheelDrawIndex++;
				mLoadingWheelDrawIndex %= 12;
			}
		}

		private void DrawLoadingWheel(Graphics g)
		{
			if (mNeedDrawLoadingWheel)
			{
				float mTransY = g.mTransY;
				Rect mClipRect = g.mClipRect;
				g.mTransY = mParent.GetAbsPos().mY;
				int theX = (mParent.mWidth - 64) / 2;
				int theY = (mParent.mHeight - 64) / 2;
				g.mClipRect = new Rect(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
				g.DrawImage(mLoadingImages[mLoadingWheelDrawIndex], theX, theY);
				g.mTransY = mTransY;
				g.mClipRect = mClipRect;
			}
		}
	}
}
