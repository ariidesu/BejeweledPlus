using System;
using System.Collections.Generic;

namespace SexyFramework.AELib
{
	public class Keyframe : IComparable
	{
		public class KeyFrameSort : Comparer<Keyframe>
		{
			public override int Compare(Keyframe x, Keyframe y)
			{
				if (x.mFrame < y.mFrame)
				{
					return -1;
				}
				if (x.mFrame > y.mFrame)
				{
					return 1;
				}
				return 0;
			}
		}

		public int mFrame;

		public float mValue1;

		public float mValue2;

		public Keyframe()
		{
		}

		public Keyframe(Keyframe rhs)
		{
			mFrame = rhs.mFrame;
			mValue1 = rhs.mValue1;
			mValue2 = rhs.mValue2;
		}

		public Keyframe(int frame, float v1, float v2)
		{
			mFrame = frame;
			mValue1 = v1;
			mValue2 = v2;
		}

		public Keyframe(int frame, float v1)
		{
			mFrame = frame;
			mValue1 = v1;
			mValue2 = 0f;
		}

		public int CompareTo(object obj)
		{
			Keyframe keyframe = obj as Keyframe;
			if (keyframe != null)
			{
				return mFrame - keyframe.mFrame;
			}
			throw new ArgumentException("object is not a Keyframe.");
		}
	}
}
