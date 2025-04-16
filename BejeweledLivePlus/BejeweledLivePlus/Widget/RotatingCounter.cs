using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class RotatingCounter : SexyFramework.Widget.Widget
	{
		public List<RotatingSeg> mSegs = new List<RotatingSeg>();

		public new Rect mClip = default(Rect);

		public Font mFont;

		public int mMaxNum;

		public int mCurNumber;

		public RotatingCounter(int displayNum, Rect clip, Font font)
		{
			mClip = clip;
			mFont = font;
			mMaxNum = (int)(Math.Pow(10.0, 3.0) - 1.0);
			mCurNumber = -1;
			mSegs.Capacity = 3;
			for (int i = 0; i < 3; i++)
			{
				mSegs.Add(new RotatingSeg());
				mSegs[i].mChar = '0';
				mSegs[i].mIndex = i;
				mSegs[i].mFont = font;
				mSegs[i].mNumRevs = 0;
			}
			ResetCounter(displayNum);
		}

		public override void Dispose()
		{
			foreach (RotatingSeg mSeg in mSegs)
			{
				mSeg?.Dispose();
			}
			mSegs.Clear();
			base.Dispose();
		}

		public void IncByNum(int theNum)
		{
			if (mCurNumber > mMaxNum || theNum == 0)
			{
				return;
			}
			bool flag = mCurNumber + theNum > mMaxNum;
			int num = Math.Min(mCurNumber + theNum, mMaxNum);
			int num2 = mCurNumber;
			for (int num3 = 2; num3 >= 0; num3--)
			{
				mSegs[num3].mNumRevs = num - num2;
				num /= 10;
				num2 /= 10;
			}
			if (flag)
			{
				for (int i = 0; i < 3; i++)
				{
					mSegs[i].mNumRevs++;
					mSegs[i].mDoMax = true;
				}
			}
			mCurNumber = Math.Min(mCurNumber + theNum, mMaxNum);
		}

		public static char CharFromNum(int num)
		{
			char[] array = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			char result = array[0];
			if (num >= 0 && num <= 9)
			{
				result = array[num];
			}
			return result;
		}

		public void ResetCounter(int theNum)
		{
			if (mCurNumber != theNum)
			{
				mCurNumber = theNum;
				bool flag = ((theNum > mMaxNum) ? true : false);
				int num = mClip.mWidth / 3;
				int num2 = (int)Math.Pow(10.0, 2.0);
				string text = "MAX";
				for (int i = 0; i < 3; i++)
				{
					Rect rect = new Rect(mClip.mX + num * i, mClip.mY, num, mClip.mHeight);
					char c = '\0';
					c = ((theNum < mMaxNum) ? CharFromNum(mCurNumber / num2 % 10) : text[i]);
					mSegs[i].mChar = c;
					mSegs[i].mNumRevs = (flag ? 1 : 0);
					mSegs[i].mIndex = i;
					mSegs[i].mDoMax = flag;
					mSegs[i].mClip = rect;
					mSegs[i].mFont = mFont;
					mSegs[i].mY = rect.mY + (rect.mHeight + mFont.GetAscent()) / 2;
					num2 /= 10;
				}
			}
		}

		public new virtual void Update()
		{
			for (int i = 0; i < 3; i++)
			{
				if (mSegs[i].mNumRevs <= 0)
				{
					continue;
				}
				bool flag = true;
				for (int j = i + 1; j < 3; j++)
				{
					if (mSegs[j].mChar != '9')
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					if (i == 0)
					{
						int num = 0;
						num++;
					}
					mSegs[i].mY -= 15;
					if (mSegs[i].mY <= mClip.mY)
					{
						mSegs[i].mChar = mSegs[i].GetNextChar();
						mSegs[i].mNumRevs--;
						mSegs[i].mY = mClip.mY + (mClip.mHeight + mFont.GetAscent()) / 2;
					}
				}
			}
		}

		public virtual void Draw(Graphics g, float yOffs)
		{
			g.PushState();
			g.SetFont(mFont);
			g.SetColor(new Color(255, 0, 255));
			Utils.SetFontLayerColor((ImageFont)mFont, 0, Color.Black);
			for (int i = 0; i < 3; i++)
			{
				mSegs[i].Draw(g, yOffs);
			}
			g.PopState();
		}
	}
}
