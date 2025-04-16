using System.Collections.Generic;

namespace SexyFramework.AELib
{
	public class Timeline
	{
		protected List<Keyframe> mKeyframes = new List<Keyframe>();

		protected bool mPingForward;

		protected int mLastPingPongFrame = -1;

		public int mLoopType = -1;

		public int mLoopFrame = -1;

		public Timeline()
		{
		}

		public Timeline(Timeline rhs)
		{
			CopyFrom(rhs);
		}

		public void CopyFrom(Timeline rhs)
		{
			mPingForward = rhs.mPingForward;
			mLastPingPongFrame = rhs.mLastPingPongFrame;
			mLoopType = rhs.mLoopType;
			mLoopFrame = rhs.mLoopFrame;
			mKeyframes.Clear();
			for (int i = 0; i < rhs.mKeyframes.Count; i++)
			{
				mKeyframes.Add(new Keyframe(rhs.mKeyframes[i]));
			}
		}

		public void Reset()
		{
			mPingForward = false;
			mLastPingPongFrame = -1;
		}

		public bool HasInitialValue()
		{
			if (mKeyframes.Count > 0)
			{
				return mKeyframes[0].mFrame == 0;
			}
			return false;
		}

		public void AddKeyframe(int frame, float value1)
		{
			AddKeyframe(frame, value1, 0f);
		}

		public void AddKeyframe(int frame, float value1, float value2)
		{
			for (int i = 0; i < mKeyframes.Count; i++)
			{
				if (mKeyframes[i].mFrame == frame)
				{
					mKeyframes[i].mValue1 = value1;
					mKeyframes[i].mValue2 = value2;
					return;
				}
			}
			mKeyframes.Add(new Keyframe(frame, value1, value2));
			mKeyframes.Sort(new Keyframe.KeyFrameSort());
		}

		public void GetValue(int frame, ref float value1)
		{
			float value2 = 0f;
			GetValue(frame, ref value1, ref value2);
		}

		public void GetValue(int frame, ref float value1, ref float value2)
		{
			if (mKeyframes.Count == 0)
			{
				return;
			}
			if (mKeyframes.Count == 1)
			{
				value1 = mKeyframes[0].mValue1;
				value2 = mKeyframes[0].mValue2;
				return;
			}
			int mFrame = mKeyframes[mKeyframes.Count - 1].mFrame;
			if (mLoopType == 10 && frame > mFrame)
			{
				frame = mLoopFrame + frame % (mFrame - mLoopFrame);
			}
			else if (mLoopType == 11 && frame > mFrame)
			{
				frame = mLoopFrame + frame % (mFrame - mLoopFrame);
				if (frame < mLastPingPongFrame)
				{
					mPingForward = !mPingForward;
				}
				mLastPingPongFrame = frame;
				if (!mPingForward)
				{
					frame = mFrame - frame + mLoopFrame;
				}
				else if (frame >= mFrame || frame < mLastPingPongFrame)
				{
					mPingForward = !mPingForward;
				}
			}
			for (int i = 1; i < mKeyframes.Count; i++)
			{
				Keyframe keyframe = mKeyframes[i];
				if (keyframe.mFrame > frame)
				{
					Keyframe keyframe2 = mKeyframes[i - 1];
					float num = (float)(frame - keyframe2.mFrame) / (float)(keyframe.mFrame - keyframe2.mFrame);
					value1 = (1f - num) * keyframe2.mValue1 + num * keyframe.mValue1;
					value2 = (1f - num) * keyframe2.mValue2 + num * keyframe.mValue2;
					return;
				}
			}
			value1 = mKeyframes[mKeyframes.Count - 1].mValue1;
			value2 = mKeyframes[mKeyframes.Count - 1].mValue2;
		}
	}
}
