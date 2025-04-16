using System.Collections.Generic;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class LineEmitterPoint
	{
		public List<PointKeyFrame> mKeyFramePoints = new List<PointKeyFrame>();

		public float mCurX;

		public float mCurY;

		public virtual void Serialize(Buffer b)
		{
			b.WriteFloat(mCurX);
			b.WriteFloat(mCurY);
			b.WriteLong(mKeyFramePoints.Count);
			for (int i = 0; i < mKeyFramePoints.Count; i++)
			{
				b.WriteLong(mKeyFramePoints[i].first);
				b.WriteLong(mKeyFramePoints[i].second.mX);
				b.WriteLong(mKeyFramePoints[i].second.mY);
			}
		}

		public virtual void Deserialize(Buffer b)
		{
			mCurX = b.ReadFloat();
			mCurY = b.ReadFloat();
			int num = (int)b.ReadLong();
			mKeyFramePoints.Clear();
			for (int i = 0; i < num; i++)
			{
				int f = (int)b.ReadLong();
				int theX = (int)b.ReadLong();
				int theY = (int)b.ReadLong();
				mKeyFramePoints.Add(new PointKeyFrame(f, new Point(theX, theY)));
			}
		}
	}
}
