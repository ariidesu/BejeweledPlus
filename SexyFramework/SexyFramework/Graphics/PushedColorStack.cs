using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class PushedColorStack
	{
		private List<Color> mColors = new List<Color>();

		public Color this[int idx]
		{
			get
			{
				if (idx < 0)
				{
					return Color.White;
				}
				return mColors[idx];
			}
			set
			{
				if (idx >= 0)
				{
					mColors[idx] = value;
				}
			}
		}

		public int Count => mColors.Count;

		public void CopyFrom(PushedColorStack rhs)
		{
			if (this == rhs)
			{
				return;
			}
			mColors.Clear();
			foreach (Color mColor in rhs.mColors)
			{
				mColors.Add(mColor);
			}
		}

		public void Add(Color c)
		{
			mColors.Add(c);
		}

		public void Push_back(Color c)
		{
			mColors.Add(c);
		}

		public Color Back()
		{
			return Common.back(mColors);
		}

		public void Pop_back()
		{
			mColors.RemoveAt(mColors.Count);
		}

		public void RemoveAt(int idx)
		{
			if (idx >= 0)
			{
				mColors.RemoveAt(idx);
			}
		}

		public int Size()
		{
			return mColors.Count;
		}
	}
}
