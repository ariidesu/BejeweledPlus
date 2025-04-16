using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class SpeedText : SexyFramework.Widget.Widget
	{
		public enum STATE
		{
			STATE_ZOOMON,
			STATE_HOLD,
			STATE_ZOOMOFF
		}

		public List<int> mOldX;

		private Image mImage;

		private Image mOutlineImage;

		private float mAlpha;

		private float mTimer;

		private int mTextX;

		private int mTextY;

		private int mState;

		private bool mFading;

		public SpeedText(int theCY, int theIndex)
		{
			mFading = false;
			mClip = false;
			mTextX = GlobalMembers.gApp.mWidth;
			mTextY = GlobalMembers.S(5);
			mState = 0;
			mMouseVisible = false;
		}

		public override void Update()
		{
			WidgetUpdate();
			if (mOldX.Count > 20)
			{
				mOldX.RemoveAt(0);
			}
			if (!mFading)
			{
				mOldX.Add(mTextX);
			}
			switch (mState)
			{
			case 0:
			{
				int num2 = GlobalMembers.gApp.mWidth / 2 - mImage.mWidth / 2;
				mTextX -= GlobalMembers.M(30);
				if (mTextX < num2)
				{
					mTextX = num2;
					mTimer = GlobalMembers.M(1.5f);
					mState++;
				}
				break;
			}
			case 1:
				mTimer -= 0.01f;
				if (mTimer <= 0f)
				{
					mState++;
				}
				break;
			case 2:
			{
				int num = -mImage.mWidth;
				mTextX -= GlobalMembers.S(50);
				if (mTextX < num)
				{
					mTextX = num;
					mFading = true;
					if (mOldX.Count > 0)
					{
						mOldX.RemoveAt(0);
						break;
					}
					mState++;
					mParent.RemoveWidget(this);
					GlobalMembers.gApp.SafeDeleteWidget(this);
				}
				break;
			}
			}
			MarkDirty();
		}

		public override void Draw(Graphics g)
		{
			g.SetColor(new Color(255, 255, 255));
			int[] array = new int[2]
			{
				mTextY,
				GlobalMembers.gApp.mHeight - mTextY - mImage.mHeight
			};
			for (int i = 0; i < 2; i++)
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.DrawImage(mImage, mTextX, array[i]);
				g.SetDrawMode(Graphics.DrawMode.Normal);
				g.DrawImage(mOutlineImage, mTextX, array[i]);
			}
			int num = GlobalMembers.M(200);
			for (int j = 0; j < mOldX.Count; j++)
			{
				g.SetColor(new Color(0, 255, 255, num));
				num -= GlobalMembers.M(9);
				if (num <= 0)
				{
					break;
				}
				if (mOldX[j] != mTextX)
				{
					g.DrawImage(mImage, mOldX[j], mTextY);
					g.DrawImage(mImage, mOldX[j], GlobalMembers.gApp.mHeight - mTextY - mImage.mHeight);
				}
			}
			g.SetDrawMode(Graphics.DrawMode.Normal);
		}
	}
}
