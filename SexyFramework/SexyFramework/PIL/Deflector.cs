using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.PIL
{
	public class Deflector
	{
		protected TimeLine mTimeLine = new TimeLine();

		protected DeflectorSettings mLastSettings;

		protected Rect mUnrotatedRect = default(Rect);

		protected Point[] mRotatedPoints = new Point[4]
		{
			default(Point),
			default(Point),
			default(Point),
			default(Point)
		};

		protected float mRotX1;

		protected float mRotX2;

		protected float mRotY1;

		protected float mRotY2;

		protected float mBaseAngle;

		protected int mLastFrame;

		protected float mX1Off;

		protected float mY1Off;

		protected float mX2Off;

		protected float mY2Off;

		public int mSerialIndex;

		public float mX1;

		public float mY1;

		public float mX2;

		public float mY2;

		public float mCenterX;

		public float mCenterY;

		public WaypointManager mWaypointManager;

		public System mSystem;

		public Deflector()
		{
			mX1 = 0f;
			mY1 = 0f;
			mX2 = 0f;
			mY2 = 0f;
			mBaseAngle = 0f;
			mRotX1 = 0f;
			mRotX2 = 0f;
			mRotY1 = 0f;
			mRotY2 = 0f;
			mLastFrame = 0;
			mLastSettings = null;
			mX1Off = 0f;
			mY1Off = 0f;
			mX2Off = 0f;
			mY2Off = 0f;
			mCenterX = 0f;
			mCenterY = 0f;
			mSerialIndex = -1;
			mSystem = null;
			mLastSettings = new DeflectorSettings();
			mTimeLine.mCurrentSettings = mLastSettings;
			mWaypointManager = new WaypointManager();
		}

		public virtual void Dispose()
		{
			mWaypointManager.Dispose();
			mWaypointManager = null;
		}

		public void ResetForReuse()
		{
			mLastSettings = new DeflectorSettings();
			mTimeLine.mCurrentSettings = mLastSettings;
		}

		public void Update(int frame)
		{
			if (frame == 0)
			{
				if (mWaypointManager.GetNumPoints() == 0)
				{
					mCenterX = mX1 + (mX2 - mX1) * 0.5f;
					mCenterY = mY1 + (mY2 - mY1) * 0.5f;
				}
				mX1Off = mX1 - mCenterX;
				mY1Off = mY1 - mCenterY;
				mX2Off = mX2 - mCenterX;
				mY2Off = mY2 - mCenterY;
				mBaseAngle = Common.AngleBetweenPoints(mX1, mY1, mX2, mY2);
			}
			mLastFrame = frame;
			mTimeLine.Update(frame);
			if (mLastSettings.mThickness < 2)
			{
				mLastSettings.mThickness = 2;
			}
			if (mWaypointManager.GetNumPoints() > 0)
			{
				mWaypointManager.Update(frame);
				mCenterX = mWaypointManager.GetLastPoint().X;
				mCenterY = mWaypointManager.GetLastPoint().Y;
			}
			float x = mCenterX + mX1Off;
			float y = mCenterY + mY1Off;
			Common.RotatePoint(0f - mBaseAngle, ref x, ref y, mCenterX, mCenterY);
			float x2 = mCenterX + mX2Off;
			float y2 = mCenterY + mY2Off;
			Common.RotatePoint(0f - mBaseAngle, ref x2, ref y2, mCenterX, mCenterY);
			mUnrotatedRect.SetValue((int)x, (int)(y - (float)(mLastSettings.mThickness / 2)), (int)(x2 - x), mLastSettings.mThickness);
			mRotX1 = mCenterX + mX1Off;
			mRotX2 = mCenterX + mX2Off;
			mRotY1 = mCenterY + mY1Off;
			mRotY2 = mCenterY + mY2Off;
			Common.RotatePoint(mLastSettings.mAngle, ref mRotX1, ref mRotY1, mCenterX, mCenterY);
			Common.RotatePoint(mLastSettings.mAngle, ref mRotX2, ref mRotY2, mCenterX, mCenterY);
			float num = (float)mLastSettings.mThickness / 2f * (float)Math.Sin(mLastSettings.mAngle + mBaseAngle);
			float num2 = (float)mLastSettings.mThickness / 2f * (float)Math.Cos(mLastSettings.mAngle + mBaseAngle);
			mRotatedPoints[0].mX = (int)(mRotX1 - num);
			mRotatedPoints[0].mY = (int)(mRotY1 - num2);
			mRotatedPoints[1].mX = (int)(mRotX2 - num);
			mRotatedPoints[1].mY = (int)(mRotY2 - num2);
			mRotatedPoints[2].mX = (int)(mRotX2 + num);
			mRotatedPoints[2].mY = (int)(mRotY2 + num2);
			mRotatedPoints[3].mX = (int)(mRotX1 + num);
			mRotatedPoints[3].mY = (int)(mRotY1 + num2);
		}

		public void DebugDraw(SexyFramework.Graphics.Graphics g)
		{
		}

		public void AddKeyFrame(int frame, DeflectorSettings p)
		{
			mTimeLine.AddKeyFrame(frame, p);
		}

		public void Apply(MovableObject p)
		{
			if (!p.CanInteract())
			{
				return;
			}
			DeflectorCollInfo deflectorCollInfo = new DeflectorCollInfo();
			p.mDeflectorCollMap[this] = deflectorCollInfo;
			if (deflectorCollInfo.mIgnoresDeflector)
			{
				return;
			}
			Rect r = new Rect((int)p.GetX() - 1, (int)p.GetY() - 1, 2, 2);
			if (!Common.RotatedRectsIntersect(r, 0f - p.mAngle, mUnrotatedRect, mBaseAngle + mLastSettings.mAngle))
			{
				return;
			}
			if (Common.Rand() % 100 >= (int)(100f * mLastSettings.mHitChance))
			{
				deflectorCollInfo.mIgnoresDeflector = true;
				return;
			}
			float[] array = new float[4];
			float num = 3.4E+38f;
			int num2 = 5;
			float t = 0f;
			for (int i = 0; i < 4; i++)
			{
				array[i] = Common.DistFromPointToLine(mRotatedPoints[i], (i < 3) ? mRotatedPoints[i + 1] : mRotatedPoints[0], new Point((int)p.GetX(), (int)p.GetY()), ref t);
				if (array[i] < num)
				{
					num = array[i];
					num2 = i;
				}
			}
			Point point = ((num2 == 3) ? mRotatedPoints[0] : mRotatedPoints[num2 + 1]);
			SexyVector2 sexyVector = -new SexyVector2(point.mX - mRotatedPoints[num2].mX, point.mY - mRotatedPoints[num2].mY).Normalize().Perp();
			float num3 = mLastSettings.mBounceMult * p.mBounce / 100f;
			SexyVector2 sexyVector2 = new SexyVector2(p.mVX, p.mVY);
			SexyVector2 sexyVector3 = (sexyVector2 - sexyVector * 2f * sexyVector2.Dot(sexyVector)) * num3;
			p.SetX(p.GetX() + sexyVector.x * mLastSettings.mCollisionMult);
			p.SetY(p.GetY() + sexyVector.y * mLastSettings.mCollisionMult);
			p.mVX = sexyVector3.x;
			p.mVY = sexyVector3.y;
		}

		public virtual void Serialize(SexyFramework.Misc.Buffer b)
		{
			mTimeLine.Serialize(b);
			mLastSettings.Serialize(b);
			b.WriteLong(mUnrotatedRect.mX);
			b.WriteLong(mUnrotatedRect.mY);
			b.WriteLong(mUnrotatedRect.mWidth);
			b.WriteLong(mUnrotatedRect.mHeight);
			for (int i = 0; i < 4; i++)
			{
				b.WriteLong(mRotatedPoints[i].mX);
				b.WriteLong(mRotatedPoints[i].mY);
			}
			b.WriteFloat(mRotX1);
			b.WriteFloat(mRotX2);
			b.WriteFloat(mRotY1);
			b.WriteFloat(mRotY2);
			b.WriteFloat(mBaseAngle);
			b.WriteLong(mLastFrame);
			b.WriteFloat(mX1Off);
			b.WriteFloat(mY1Off);
			b.WriteFloat(mX2Off);
			b.WriteFloat(mY2Off);
			b.WriteLong(mSerialIndex);
			b.WriteFloat(mX1);
			b.WriteFloat(mY1);
			b.WriteFloat(mX2);
			b.WriteFloat(mY2);
			b.WriteFloat(mCenterX);
			b.WriteFloat(mCenterY);
			mWaypointManager.Serialize(b);
		}

		public virtual void Deserialize(SexyFramework.Misc.Buffer b)
		{
			mTimeLine.Deserialize(b, DeflectorSettings.Instantiate);
			mLastSettings.Deserialize(b);
			mUnrotatedRect.mX = (int)b.ReadLong();
			mUnrotatedRect.mY = (int)b.ReadLong();
			mUnrotatedRect.mWidth = (int)b.ReadLong();
			mUnrotatedRect.mHeight = (int)b.ReadLong();
			for (int i = 0; i < 4; i++)
			{
				mRotatedPoints[i].mX = (int)b.ReadLong();
				mRotatedPoints[i].mY = (int)b.ReadLong();
			}
			mRotX1 = b.ReadFloat();
			mRotX2 = b.ReadFloat();
			mRotY1 = b.ReadFloat();
			mRotY2 = b.ReadFloat();
			mBaseAngle = b.ReadFloat();
			mLastFrame = (int)b.ReadLong();
			mX1Off = b.ReadFloat();
			mY1Off = b.ReadFloat();
			mX2Off = b.ReadFloat();
			mY2Off = b.ReadFloat();
			mSerialIndex = (int)b.ReadLong();
			mX1 = b.ReadFloat();
			mY1 = b.ReadFloat();
			mX2 = b.ReadFloat();
			mY2 = b.ReadFloat();
			mCenterX = b.ReadFloat();
			mCenterY = b.ReadFloat();
			mWaypointManager.Deserialize(b);
		}

		public void LoopTimeLine(bool l)
		{
			mTimeLine.mLoop = l;
		}
	}
}
