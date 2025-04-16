using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class Force
	{
		protected TimeLine mTimeLine = new TimeLine();

		protected ForceSettings mLastSettings;

		protected float mLastAX;

		protected float mLastAY;

		public float mCenterX;

		public float mCenterY;

		public WaypointManager mWaypointManager;

		public System mSystem;

		public Force()
		{
			mCenterX = 0f;
			mCenterY = 0f;
			mLastAX = 0f;
			mLastAY = 0f;
			mTimeLine.mCurrentSettings = new ForceSettings();
			mLastSettings = (ForceSettings)mTimeLine.mCurrentSettings;
			mWaypointManager = new WaypointManager();
		}

		public virtual void Dispose()
		{
			mWaypointManager.Dispose();
			mWaypointManager = null;
		}

		public void ResetForReuse()
		{
			mTimeLine.mCurrentSettings = new ForceSettings();
			mLastSettings = (ForceSettings)mTimeLine.mCurrentSettings;
		}

		public void Update(int frame)
		{
			mTimeLine.Update(frame);
			float num = ModVal.M(2000f);
			mLastAX = mLastSettings.mStrength / num * (float)Math.Cos(mLastSettings.mAngle + mLastSettings.mDirection);
			mLastAY = (0f - mLastSettings.mStrength / num) * (float)Math.Sin(mLastSettings.mAngle + mLastSettings.mDirection);
			if (mWaypointManager.GetNumPoints() > 0)
			{
				mWaypointManager.Update(frame);
				mCenterX = mWaypointManager.GetLastPoint().X;
				mCenterY = mWaypointManager.GetLastPoint().Y;
			}
		}

		public void DebugDraw(SexyFramework.Graphics.Graphics g)
		{
		}

		public void Apply(MovableObject p)
		{
			if (p.CanInteract() && Common.RotatedRectsIntersect(new Rect((int)(mCenterX - mLastSettings.mWidth / 2f), (int)(mCenterY - mLastSettings.mHeight / 2f), (int)mLastSettings.mWidth, (int)mLastSettings.mHeight), r2: new Rect((int)(p.GetX() - 1f), (int)(p.GetY() - 1f), 2, 2), r1_angle: mLastSettings.mAngle, r2_angle: 0f - p.mAngle))
			{
				p.ApplyAcceleration(mLastAX, mLastAY);
			}
		}

		public void AddKeyFrame(int frame, ForceSettings p)
		{
			mTimeLine.AddKeyFrame(frame, p);
		}

		public void LoopTimeLine(bool l)
		{
			mTimeLine.mLoop = l;
		}

		public virtual void Serialize(SexyFramework.Misc.Buffer b)
		{
			mTimeLine.Serialize(b);
			mLastSettings.Serialize(b);
			b.WriteFloat(mLastAX);
			b.WriteFloat(mLastAY);
			b.WriteFloat(mCenterX);
			b.WriteFloat(mCenterY);
			mWaypointManager.Serialize(b);
		}

		public virtual void Deserialize(SexyFramework.Misc.Buffer b)
		{
			mTimeLine.Deserialize(b, ForceSettings.Instantiate);
			mLastSettings.Deserialize(b);
			mLastAX = b.ReadFloat();
			mLastAY = b.ReadFloat();
			mCenterX = b.ReadFloat();
			mCenterY = b.ReadFloat();
			mWaypointManager.Deserialize(b);
		}
	}
}
