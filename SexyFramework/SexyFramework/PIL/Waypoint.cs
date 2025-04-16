using Microsoft.Xna.Framework;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.PIL
{
	public class Waypoint
	{
		public bool mLinear;

		public Vector2 mControl1 = default(Vector2);

		public Vector2 mControl2 = default(Vector2);

		public Vector2 mPoint = default(Vector2);

		public float mTime;

		public int mFrame;

		public void CopyFrom(Waypoint rhs)
		{
			mLinear = rhs.mLinear;
			mControl1 = rhs.mControl1;
			mControl2 = rhs.mControl2;
			mPoint = rhs.mPoint;
			mTime = rhs.mTime;
			mFrame = rhs.mFrame;
		}

		public void Serialize(Buffer b)
		{
			b.WriteBoolean(mLinear);
			b.WriteFloat(mTime);
			b.WriteLong(mFrame);
			b.WriteFloat(mControl1.X);
			b.WriteFloat(mControl1.Y);
			b.WriteFloat(mControl2.X);
			b.WriteFloat(mControl2.Y);
			b.WriteFloat(mPoint.X);
			b.WriteFloat(mPoint.Y);
		}

		public void Deserialize(Buffer b)
		{
			mLinear = b.ReadBoolean();
			mTime = b.ReadFloat();
			mFrame = (int)b.ReadLong();
			mControl1.X = b.ReadFloat();
			mControl1.Y = b.ReadFloat();
			mControl2.X = b.ReadFloat();
			mControl2.Y = b.ReadFloat();
			mPoint.X = b.ReadFloat();
			mPoint.Y = b.ReadFloat();
		}
	}
}
