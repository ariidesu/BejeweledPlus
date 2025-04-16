using System;
using System.Collections.Generic;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.Misc
{
	public class ColorCycle : IDisposable
	{
		public static int gNumCycleColors = GlobalMembers.gCycleColors.Length;

		public Color mColor = default(Color);

		public float mCyclePos;

		public float mSpeed;

		public float mBrightness;

		public float mAlpha;

		public bool mLooping = true;

		public List<Color> mCycleColors = new List<Color>();

		public ColorCycle()
		{
			for (int i = 0; i < gNumCycleColors; i++)
			{
				mCycleColors.Add(GlobalMembers.gCycleColors[i]);
			}
			mBrightness = 0f;
			mLooping = true;
			mAlpha = 1f;
			Restart();
		}

		public void Dispose()
		{
		}

		public void SetSpeed(float aSpeed)
		{
			mSpeed = aSpeed;
		}

		public void Update()
		{
			if (mSpeed == 0f)
			{
				return;
			}
			if (mCycleColors.Count == 0)
			{
				mColor = new Color(0, 0, 0, 0);
				return;
			}
			if (mCycleColors.Count == 1)
			{
				mColor = mCycleColors[0];
				return;
			}
			mCyclePos += mSpeed * 0.01f;
			if (mCyclePos >= 1f && !mLooping)
			{
				mCyclePos = 1f;
				mColor = mCycleColors[mCycleColors.Count - 1];
				return;
			}
			while (mCyclePos >= 1f)
			{
				mCyclePos -= 1f;
			}
			float num = mCyclePos * (float)mCycleColors.Count;
			int num2 = (int)num;
			int num3 = (num2 + 1) % mCycleColors.Count;
			if (!mLooping && num3 < num2)
			{
				num3 = num2;
			}
			Color[] array = new Color[2]
			{
				mCycleColors[num2],
				mCycleColors[num3]
			};
			float num4 = num - (float)num2;
			mColor.mRed = (int)(num4 * (float)array[1].mRed + (1f - num4) * (float)array[0].mRed);
			mColor.mGreen = (int)(num4 * (float)array[1].mGreen + (1f - num4) * (float)array[0].mGreen);
			mColor.mBlue = (int)(num4 * (float)array[1].mBlue + (1f - num4) * (float)array[0].mBlue);
			mColor.mAlpha = (int)(mAlpha * (num4 * (float)array[1].mAlpha + (1f - num4) * (float)array[0].mAlpha));
			if (mBrightness != 0f)
			{
				int num5 = (int)(mBrightness * 255f);
				if (num5 > 0)
				{
					mColor.mRed = Math.Min(255, mColor.mRed + num5);
					mColor.mGreen = Math.Min(255, mColor.mGreen + num5);
					mColor.mBlue = Math.Min(255, mColor.mBlue + num5);
				}
				else
				{
					mColor.mRed = Math.Max(0, mColor.mRed + num5);
					mColor.mGreen = Math.Max(0, mColor.mGreen + num5);
					mColor.mBlue = Math.Max(0, mColor.mBlue + num5);
				}
			}
		}

		public Color GetColor()
		{
			return mColor;
		}

		public void SetPosition(float thePos)
		{
			while (thePos >= 1f)
			{
				thePos -= 1f;
			}
			mCyclePos = thePos;
			Update();
		}

		public void SetBrightness(float theBrightness)
		{
			mBrightness = theBrightness;
		}

		public void Restart()
		{
			mCyclePos = 0f;
			mSpeed = 1f;
		}

		public void ClearColors()
		{
			mCycleColors.Clear();
			mCyclePos = 0f;
		}

		public void PushColor(Color theColor)
		{
			mCycleColors.Add(theColor);
		}

		public static implicit operator Color(ColorCycle ImpliedObject)
		{
			return new Color(ImpliedObject.mColor);
		}
	}
}
