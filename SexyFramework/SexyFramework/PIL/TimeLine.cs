using System.Collections.Generic;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class TimeLine
	{
		public List<KeyFrame> mKeyFrames = new List<KeyFrame>();

		public KeyFrame mPrev;

		public KeyFrame mNext;

		public KeyFrameData mCurrentSettings;

		public bool mLoop;

		public TimeLine()
		{
		}

		public TimeLine(TimeLine rhs)
		{
			if (rhs != null)
			{
				mCurrentSettings = rhs.mCurrentSettings.Clone();
				mLoop = rhs.mLoop;
				mPrev = null;
				mNext = null;
				for (int i = 0; i < Common.size(rhs.mKeyFrames); i++)
				{
					KeyFrameData k = rhs.mKeyFrames[i].second.Clone();
					AddKeyFrame(rhs.mKeyFrames[i].first, k);
				}
			}
		}

		public virtual void Dispose()
		{
			for (int i = 0; i < Common.size(mKeyFrames); i++)
			{
				mKeyFrames[i] = null;
			}
			mKeyFrames.Clear();
			mCurrentSettings = null;
		}

		public void Update(int frame)
		{
			mPrev = (mNext = null);
			if (Common.size(mKeyFrames) == 0)
			{
				return;
			}
			frame = ((mLoop && Common.size(mKeyFrames) > 1) ? (frame % mKeyFrames[Common.size(mKeyFrames) - 1].first) : frame);
			for (int i = 0; i < Common.size(mKeyFrames); i++)
			{
				if (mKeyFrames[i].first <= frame)
				{
					mPrev = mKeyFrames[i];
					continue;
				}
				mNext = mKeyFrames[i];
				break;
			}
			mCurrentSettings.CopyFrom(mPrev.second);
			if (mNext != null)
			{
				int num = mNext.first - mPrev.first;
				float num2 = (float)(frame - mPrev.first) / (float)num;
				for (int j = 0; j < mCurrentSettings.mNumInts; j++)
				{
					mCurrentSettings.mIntData[j] += (int)((float)(mNext.second.mIntData[j] - mPrev.second.mIntData[j]) * num2);
				}
				for (int k = 0; k < mCurrentSettings.mNumFloats; k++)
				{
					mCurrentSettings.mFloatData[k] += (mNext.second.mFloatData[k] - mPrev.second.mFloatData[k]) * num2;
				}
			}
		}

		public void AddKeyFrame(int frame, KeyFrameData k)
		{
			mKeyFrames.Add(new KeyFrame(frame, k));
			mKeyFrames.Sort(new KeyFrameSort());
		}

		public virtual void Serialize(Buffer b)
		{
			b.WriteBoolean(mLoop);
			mCurrentSettings.Serialize(b);
			b.WriteLong(mKeyFrames.Count);
			for (int i = 0; i < mKeyFrames.Count; i++)
			{
				b.WriteLong(mKeyFrames[i].first);
				mKeyFrames[i].second.Serialize(b);
			}
		}

		public virtual void Deserialize(Buffer b, GlobalMembers.KFDInstantiateFunc f)
		{
			mKeyFrames.Clear();
			mLoop = b.ReadBoolean();
			mCurrentSettings.Deserialize(b);
			int num = (int)b.ReadLong();
			for (int i = 0; i < num; i++)
			{
				b.ReadLong();
				KeyFrameData keyFrameData = f();
				keyFrameData.Deserialize(b);
			}
		}
	}
}
