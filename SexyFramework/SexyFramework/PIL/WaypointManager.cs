using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class WaypointManager : IDisposable
	{
		protected Bezier mCurve;

		protected List<Waypoint> mWaypoints = new List<Waypoint>();

		protected float mTotalTime;

		protected int mTotalFrames;

		protected Vector2 mLastPoint = default(Vector2);

		protected bool mLastFrameWasEnd;

		public bool mLoop;

		protected void Clean()
		{
			mCurve = null;
			mWaypoints.Clear();
		}

		public WaypointManager()
		{
			mTotalTime = 0f;
			mLoop = false;
			mTotalFrames = 0;
			mLastFrameWasEnd = false;
			mCurve = new Bezier();
		}

		public WaypointManager(WaypointManager rhs)
		{
			mCurve = null;
			CopyFrom(rhs);
		}

		public virtual void Dispose()
		{
			Clean();
		}

		public void CopyFrom(WaypointManager rhs)
		{
			if (this != rhs && rhs != null)
			{
				Clean();
				mTotalTime = rhs.mTotalTime;
				mTotalFrames = rhs.mTotalFrames;
				mLoop = rhs.mLoop;
				mLastPoint = rhs.mLastPoint;
				mCurve = new Bezier(rhs.mCurve);
				mLastFrameWasEnd = rhs.mLastFrameWasEnd;
				for (int i = 0; i < Common.size(rhs.mWaypoints); i++)
				{
					Waypoint waypoint = new Waypoint();
					waypoint.CopyFrom(rhs.mWaypoints[i]);
					mWaypoints.Add(waypoint);
				}
			}
		}

		public void Serialize(SexyFramework.Misc.Buffer b)
		{
			mCurve.Serialize(b);
			b.WriteFloat(mTotalTime);
			b.WriteLong(mTotalFrames);
			b.WriteBoolean(mLastFrameWasEnd);
			b.WriteFloat(mLastPoint.X);
			b.WriteFloat(mLastPoint.Y);
			b.WriteLong(mWaypoints.Count);
			for (int i = 0; i < mWaypoints.Count; i++)
			{
				mWaypoints[i].Serialize(b);
			}
		}

		public void Deserialize(SexyFramework.Misc.Buffer b)
		{
			mCurve.Deserialize(b);
			mTotalTime = b.ReadFloat();
			mTotalFrames = (int)b.ReadLong();
			mLastFrameWasEnd = b.ReadBoolean();
			mLastPoint.X = b.ReadFloat();
			mLastPoint.Y = b.ReadFloat();
			mWaypoints.Clear();
			int num = (int)b.ReadLong();
			for (int i = 0; i < num; i++)
			{
				Waypoint waypoint = new Waypoint();
				waypoint.Deserialize(b);
				mWaypoints.Add(waypoint);
			}
		}

		public void AddPoint(int frame, Vector2 p, bool linear, Vector2 c1)
		{
			Waypoint waypoint = new Waypoint();
			mWaypoints.Add(waypoint);
			waypoint.mTime = (float)frame / 100f;
			waypoint.mFrame = frame;
			float num = waypoint.mTime;
			int num2 = frame;
			if (Common.size(mWaypoints) > 1)
			{
				num -= mWaypoints[Common.size(mWaypoints) - 2].mTime;
				num2 -= mWaypoints[Common.size(mWaypoints) - 2].mFrame;
			}
			mTotalTime += num;
			mTotalFrames += num2;
			waypoint.mLinear = linear;
			waypoint.mPoint = p;
			waypoint.mControl1 = c1;
			Vector2 vector = c1 - p;
			waypoint.mControl2 = p - vector;
		}

		public void AddPoint(int frame, Vector2 p, bool linear)
		{
			AddPoint(frame, p, linear, Vector2.Zero);
		}

		public void AddPoint(int frame, Vector2 p)
		{
			AddPoint(frame, p, false);
		}

		public void Init(bool make_curve_image)
		{
			Vector2[] array = new Vector2[Common.size(mWaypoints)];
			Vector2[] array2 = new Vector2[2 * (Common.size(mWaypoints) - 1)];
			float[] array3 = new float[Common.size(mWaypoints)];
			int num = 0;
			for (int i = 0; i < Common.size(mWaypoints); i++)
			{
				Waypoint waypoint = mWaypoints[i];
				array3[i] = waypoint.mTime;
				array[i] = waypoint.mPoint;
				if (!waypoint.mLinear)
				{
					array2[num] = waypoint.mControl1;
					if (i > 0 && i < Common.size(mWaypoints) - 1)
					{
						array2[num + 1] = waypoint.mControl2;
					}
				}
				else if (i == 0 && Common.size(mWaypoints) == 1)
				{
					array2[0] = waypoint.mPoint;
				}
				else
				{
					Vector2 vector = default(Vector2);
					if (i < Common.size(mWaypoints) - 1)
					{
						vector = mWaypoints[i + 1].mPoint - mWaypoints[i].mPoint;
						array2[(i != 0) ? (num + 1) : 0] = mWaypoints[i].mPoint + vector / 2f;
					}
					if (i > 0)
					{
						vector = mWaypoints[i].mPoint - mWaypoints[i - 1].mPoint;
						array2[num] = mWaypoints[i - 1].mPoint + vector / 2f;
					}
				}
				num++;
				if (i > 0)
				{
					num++;
				}
			}
			mCurve.Init(array, array2, array3, Common.size(mWaypoints));
			if (make_curve_image)
			{
				mCurve.GenerateCurveImage(SexyFramework.Graphics.Color.White, 10000);
			}
		}

		public void Update(int frame)
		{
			if (mCurve.GetNumPoints() != 0)
			{
				if (frame > 0 && frame % mTotalFrames == 0)
				{
					mLastFrameWasEnd = true;
				}
				if (mLoop)
				{
					frame %= mTotalFrames;
				}
				else if (frame > mTotalFrames)
				{
					return;
				}
				mLastPoint = mCurve.Evaluate((float)frame / (float)mTotalFrames * mTotalTime);
			}
		}

		public void DebugDraw(SexyFramework.Graphics.Graphics g, float scale)
		{
		}

		public void DebugDraw(SexyFramework.Graphics.Graphics g)
		{
			DebugDraw(g, 1f);
		}

		public Vector2 GetLastPoint()
		{
			return mLastPoint;
		}

		public int GetNumPoints()
		{
			return mCurve.GetNumPoints();
		}

		public bool AtEnd()
		{
			return mLastFrameWasEnd;
		}
	}
}
