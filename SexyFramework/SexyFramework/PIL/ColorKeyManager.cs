using System;
using System.Collections.Generic;
using System.Linq;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class ColorKeyManager
	{
		protected List<ColorKeyTimeEntry> mTimeline = new List<ColorKeyTimeEntry>();

		protected Color mCurrentColor = default(Color);

		protected SharedImageRef mImage;

		protected string mImgFileName = "";

		protected string mImgVariant = "";

		protected int mColorMode;

		protected int mLife;

		protected int mUpdateCount;

		public bool mUpdateImagePosColor;

		public int mGradientRepeat;

		public ColorKeyManager()
		{
			mColorMode = 0;
			mLife = 0;
			mUpdateCount = 0;
			mUpdateImagePosColor = false;
			mGradientRepeat = 1;
			mCurrentColor = new Color(Color.White);
		}

		public void CopyFrom(ColorKeyManager rhs)
		{
			if (rhs != null)
			{
				mCurrentColor = rhs.mCurrentColor.Clone();
				mImage = rhs.mImage;
				mImgFileName = rhs.mImgFileName;
				mImgVariant = rhs.mImgVariant;
				mColorMode = rhs.mColorMode;
				mLife = rhs.mLife;
				mUpdateCount = rhs.mUpdateCount;
				mUpdateImagePosColor = rhs.mUpdateImagePosColor;
				mGradientRepeat = rhs.mGradientRepeat;
				mTimeline.Clear();
				mTimeline.AddRange(rhs.mTimeline.ToArray());
			}
		}

		public virtual void Dispose()
		{
			mTimeline.Clear();
		}

		public void Serialize(SexyFramework.Misc.Buffer b)
		{
			b.WriteLong(mColorMode);
			b.WriteLong(mLife);
			b.WriteLong(mUpdateCount);
			b.WriteLong(mCurrentColor.ToInt());
			b.WriteLong(mTimeline.Count);
			for (int i = 0; i < mTimeline.Count; i++)
			{
				b.WriteFloat(mTimeline[i].first);
				mTimeline[i].second.Serialize(b);
			}
			b.WriteString(mImgFileName);
			b.WriteString(mImgVariant);
		}

		public void Deserialize(SexyFramework.Misc.Buffer b)
		{
			mColorMode = (int)b.ReadLong();
			mLife = (int)b.ReadLong();
			mUpdateCount = (int)b.ReadLong();
			long num = b.ReadLong();
			mCurrentColor = new Color((int)num);
			int num2 = (int)b.ReadLong();
			mTimeline.Clear();
			for (int i = 0; i < num2; i++)
			{
				float pt = b.ReadFloat();
				ColorKey colorKey = new ColorKey();
				colorKey.Deserialize(b);
				mTimeline.Add(new ColorKeyTimeEntry(pt, colorKey));
			}
			mImgFileName = b.ReadString();
			mImgVariant = b.ReadString();
			if (mImgFileName.Length > 0)
			{
				bool isNew = false;
				mImage = GlobalMembers.gSexyAppBase.GetSharedImage(mImgFileName, mImgVariant, ref isNew, true, false);
			}
		}

		public void Update(float x, float y)
		{
			if (++mUpdateCount >= mLife || mColorMode == 0)
			{
				return;
			}
			if (mUpdateCount == 1 && mColorMode == 2)
			{
				int num = Common.Rand() % Common.size(mTimeline);
				int num2 = num;
				if (Common.size(mTimeline) > 1)
				{
					while (num2 == num)
					{
						num2 = Common.Rand() % Common.size(mTimeline);
					}
				}
				mCurrentColor = mTimeline[num].second.GetInterpolatedColor(mTimeline[num2].second, Common.FloatRange(0f, 1f));
			}
			else if (mUpdateCount == 1 && mColorMode == 3)
			{
				mCurrentColor = mTimeline[Common.Rand() % Common.size(mTimeline)].second.GetColor();
			}
			else
			{
				if ((mColorMode == 4 && (mUpdateCount == 1 || mUpdateImagePosColor)) || mColorMode != 1)
				{
					return;
				}
				float num3 = (float)mUpdateCount / (float)mLife;
				float num4 = 1f / (float)mGradientRepeat;
				float val = num3 - (float)(int)(num3 / num4) * num4;
				val = Math.Max(Math.Min(num4, val), 0f);
				int num5 = -1;
				int num6 = -1;
				for (int i = 0; i < Common.size(mTimeline); i++)
				{
					if (val >= mTimeline[i].first / (float)mGradientRepeat)
					{
						num5 = i;
						continue;
					}
					num6 = i;
					break;
				}
				if (num6 == -1)
				{
					num6 = num5;
				}
				float num7 = mTimeline[num5].first / (float)mGradientRepeat;
				float num8 = mTimeline[num6].first / (float)mGradientRepeat;
				float pct = (val - num7) / (num8 - num7);
				mCurrentColor = mTimeline[num5].second.GetInterpolatedColor(mTimeline[num6].second, pct);
			}
		}

		public void SetFixedColor(Color c)
		{
			mCurrentColor = c;
			mColorMode = 3;
			mUpdateCount = 2;
		}

		public void AddColorKey(float pct, Color c)
		{
			mTimeline.Add(new ColorKeyTimeEntry(pct, new ColorKey(c)));
			mTimeline.Sort(new SortColorKeys());
			mCurrentColor = mTimeline[0].second.GetColor();
		}

		public void AddAlphaKey(float pct, int alpha)
		{
			AddColorKey(pct, new Color(255, 255, 255, alpha));
		}

		public void ForceTransition(int new_life, Color final_color)
		{
			mUpdateCount = 0;
			mLife = new_life;
			mColorMode = 1;
			AddColorKey(0f, mCurrentColor);
			AddColorKey(1f, final_color);
		}

		public Color GetColor()
		{
			return mCurrentColor;
		}

		public void SetLife(int l)
		{
			mLife = l;
		}

		public void SetColorMode(int m)
		{
			mColorMode = m;
		}

		public void SetImage(SharedImageRef r, string filename, string variant)
		{
			mImage = r;
			mImgFileName = filename;
			mImgVariant = variant;
		}

		public bool HasMaxIndex()
		{
			if (Common.size(mTimeline) == 0)
			{
				return false;
			}
			return Common._eq(mTimeline.Last().first, 1f, 1E-06f);
		}

		public int GetNumKeys()
		{
			return Common.size(mTimeline);
		}

		public int GetColorMode()
		{
			return mColorMode;
		}

		public Color GetColorByIndex(int i)
		{
			return mTimeline[i].second.GetColor();
		}
	}
}
