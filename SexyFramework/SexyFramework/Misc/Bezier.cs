using System;
using Microsoft.Xna.Framework;
using SexyFramework.Graphics;

namespace SexyFramework.Misc
{
	public class Bezier
	{
		public float[] mTimes;

		public float[] mLengths;

		public float mTotalLength;

		public int mCount;

		public Vector2[] mControls;

		public Vector2[] mPositions;

		public int mCurveDetail;

		public SexyFramework.Graphics.Color mCurveColor = default(SexyFramework.Graphics.Color);

		public MemoryImage mImage;

		public int mImageX;

		public int mImageY;

		protected void SubdivideRender(SexyFramework.Graphics.Graphics g, Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3)
		{
			g.FillRect((int)P0.X, (int)P0.Y, 1, 1);
			float t = 0f;
			if (!Common._eq(Common.DistFromPointToLine(P0, P3, P1, ref t), 0f) || !Common._eq(Common.DistFromPointToLine(P0, P3, P2, ref t), 0f))
			{
				Vector2 vector = (P0 + P1) * 0.5f;
				Vector2 vector2 = (P1 + P2) * 0.5f;
				Vector2 vector3 = (vector + vector2) * 0.5f;
				Vector2 vector4 = (P2 + P3) * 0.5f;
				Vector2 vector5 = (vector2 + vector4) * 0.5f;
				Vector2 vector6 = (vector3 + vector5) * 0.5f;
				SubdivideRender(g, P0, vector, vector3, vector6);
				SubdivideRender(g, vector6, vector5, vector4, P3);
			}
		}

		protected float SubdivideLength(Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3)
		{
			float num = Distance(P0, P3, true);
			float num2 = Distance(P0, P1, true) + Distance(P1, P2, true) + Distance(P2, P3, true);
			float num3 = num - num2;
			if (num3 * num3 < 0.001f)
			{
				return 0.5f * (num + num2);
			}
			Vector2 vector = (P0 + P1) * 0.5f;
			Vector2 vector2 = (P1 + P2) * 0.5f;
			Vector2 vector3 = (vector + vector2) * 0.5f;
			Vector2 vector4 = (P2 + P3) * 0.5f;
			Vector2 vector5 = (vector2 + vector4) * 0.5f;
			Vector2 vector6 = (vector3 + vector5) * 0.5f;
			return SubdivideLength(P0, vector, vector3, vector6) + SubdivideLength(vector6, vector5, vector4, P3);
		}

		protected float SegmentArcLength(int i, float u1, float u2)
		{
			if (u2 <= u1)
			{
				return 0f;
			}
			if (u1 < 0f)
			{
				u1 = 0f;
			}
			if (u2 > 1f)
			{
				u2 = 1f;
			}
			Vector2 vector = mPositions[i];
			Vector2 vector2 = mControls[2 * i];
			Vector2 vector3 = mControls[2 * i + 1];
			Vector2 vector4 = mPositions[i + 1];
			float num = 1f - u2;
			Vector2 vector5 = vector * num + vector2 * u2;
			Vector2 vector6 = vector2 * num + vector3 * u2;
			Vector2 vector7 = vector5 * num + vector6 * u2;
			Vector2 vector8 = vector7 * num + (vector6 * num + (vector3 * num + vector4 * u2) * u2) * u2;
			float num2 = 1f - u1;
			vector6 = vector5 * num2 + vector7 * u1;
			Vector2 p = vector8;
			Vector2 vector9 = vector7 * num2 + vector8 * u1;
			Vector2 vector10 = vector6 * num2 + vector9 * u1;
			Vector2 p2 = ((vector * num2 + vector5 * u1) * num2 + vector6 * u1) * num2 + vector10 * u1;
			return SubdivideLength(p2, vector10, vector9, p);
		}

		public Bezier()
		{
			mTimes = null;
			mLengths = null;
			mTotalLength = 0f;
			mCount = 0;
			mControls = null;
			mPositions = null;
			mCurveDetail = -1;
			mImage = null;
			mImageX = 0;
			mImageY = 0;
		}

		public Bezier(Bezier rhs)
			: this()
		{
			CopyFrom(rhs);
		}

		public virtual void Dispose()
		{
			Clean();
		}

		public void CopyFrom(Bezier rhs)
		{
			if (this == rhs || rhs == null)
			{
				return;
			}
			Clean();
			mCount = rhs.mCount;
			mTotalLength = rhs.mTotalLength;
			mImageX = rhs.mImageX;
			mImageY = rhs.mImageY;
			mCurveDetail = rhs.mCurveDetail;
			mCurveColor = new SexyFramework.Graphics.Color(rhs.mCurveColor);
			if (rhs.mImage != null)
			{
				mImage = new MemoryImage(rhs.mImage);
			}
			if (mCount <= 0)
			{
				return;
			}
			mTimes = new float[mCount];
			mPositions = new Vector2[mCount];
			mControls = new Vector2[2 * (mCount - 1)];
			mLengths = new float[mCount - 1];
			for (int i = 0; i < 2 * (mCount - 1); i++)
			{
				if (i < mCount)
				{
					mTimes[i] = rhs.mTimes[i];
					mPositions[i] = rhs.mPositions[i];
				}
				if (i < mCount - 1)
				{
					mLengths[i] = rhs.mLengths[i];
				}
				mControls[i] = rhs.mControls[i];
			}
		}

		public bool Init(Vector2[] positions, Vector2[] controls, float[] times, int count)
		{
			if (mCount != 0)
			{
				return false;
			}
			if (count < 2 || positions == null || times == null || controls == null)
			{
				return false;
			}
			mPositions = new Vector2[count];
			mControls = new Vector2[2 * (count - 1)];
			mTimes = new float[count];
			mCount = count;
			for (int i = 0; i < count; i++)
			{
				mPositions[i] = positions[i];
				mTimes[i] = times[i];
			}
			for (int j = 0; j < 2 * (count - 1); j++)
			{
				mControls[j] = controls[j];
			}
			mLengths = new float[count - 1];
			mTotalLength = 0f;
			for (int k = 0; k < count - 1; k++)
			{
				mLengths[k] = SegmentArcLength(k, 0f, 1f);
				mTotalLength += mLengths[k];
			}
			return true;
		}

		public bool Init(Vector2[] positions, float[] times, int count)
		{
			if (mCount != 0)
			{
				return false;
			}
			if (count < 2 || positions == null || times == null)
			{
				return false;
			}
			mPositions = new Vector2[count];
			mControls = new Vector2[2 * (count - 1)];
			mTimes = new float[count];
			mCount = count;
			for (int i = 0; i < count; i++)
			{
				mPositions[i] = positions[i];
				mTimes[i] = times[i];
			}
			for (int j = 0; j < count - 1; j++)
			{
				if (j > 0)
				{
					mControls[2 * j] = mPositions[j] + (mPositions[j + 1] - mPositions[j - 1]) / 3f;
				}
				if (j < count - 2)
				{
					mControls[2 * j + 1] = mPositions[j + 1] - (mPositions[j + 2] - mPositions[j]) / 3f;
				}
			}
			mControls[0] = mControls[1] - (mPositions[1] - mPositions[0]) / 3f;
			mControls[2 * count - 3] = mControls[2 * count - 4] + (mPositions[count - 1] - mPositions[count - 2]) / 3f;
			mLengths = new float[count - 1];
			mTotalLength = 0f;
			for (int k = 0; k < count - 1; k++)
			{
				mLengths[k] = SegmentArcLength(k, 0f, 1f);
				mTotalLength += mLengths[k];
			}
			return true;
		}

		public Vector2 Evaluate(float t)
		{
			if (mCount < 2)
			{
				return new Vector2(0f, 0f);
			}
			if (t <= mTimes[0])
			{
				return mPositions[0];
			}
			if (t >= mTimes[mCount - 1])
			{
				return mPositions[mCount - 1];
			}
			int num = 0;
			for (num = 0; num < mCount - 1 && !(t < mTimes[num + 1]); num++)
			{
			}
			float num2 = mTimes[num];
			float num3 = mTimes[num + 1];
			float num4 = (t - num2) / (num3 - num2);
			Vector2 vector = mPositions[num + 1] - mControls[2 * num + 1] * 3f + mControls[2 * num] * 3f - mPositions[num];
			Vector2 vector2 = mControls[2 * num + 1] * 3f - mControls[2 * num] * 6f + mPositions[num] * 3f;
			Vector2 vector3 = mControls[2 * num] * 3f - mPositions[num] * 3f;
			return mPositions[num] + (vector3 + (vector2 + vector * num4) * num4) * num4;
		}

		public void Serialize(Buffer b)
		{
			b.WriteFloat(mTotalLength);
			b.WriteLong(mCount);
			b.WriteLong(mCurveDetail);
			b.WriteLong(mCurveColor.ToInt());
			for (int i = 0; i < mCount; i++)
			{
				b.WriteFloat(mTimes[i]);
				b.WriteFloat(mPositions[i].X);
				b.WriteFloat(mPositions[i].Y);
			}
			for (int j = 0; j < 2 * (mCount - 1); j++)
			{
				b.WriteFloat(mControls[j].X);
				b.WriteFloat(mControls[j].Y);
			}
			for (int k = 0; k < mCount - 1; k++)
			{
				b.WriteFloat(mLengths[k]);
			}
		}

		public void Deserialize(Buffer b)
		{
			Clean();
			mTotalLength = b.ReadFloat();
			mCount = (int)b.ReadLong();
			mCurveDetail = (int)b.ReadLong();
			mCurveColor = new SexyFramework.Graphics.Color((int)b.ReadLong());
			if (mCount > 0)
			{
				mTimes = new float[mCount];
				mPositions = new Vector2[mCount];
				mControls = new Vector2[2 * (mCount - 1)];
				mLengths = new float[mCount - 1];
				for (int i = 0; i < mCount; i++)
				{
					mTimes[i] = b.ReadFloat();
					mPositions[i].X = b.ReadFloat();
					mPositions[i].Y = b.ReadFloat();
				}
				for (int j = 0; j < 2 * (mCount - 1); j++)
				{
					mControls[j].X = b.ReadFloat();
					mControls[j].Y = b.ReadFloat();
				}
				for (int k = 0; k < mCount - 1; k++)
				{
					mLengths[k] = b.ReadFloat();
				}
			}
			if (mCurveDetail > 0)
			{
				GenerateCurveImage(mCurveColor, mCurveDetail);
			}
		}

		public void GenerateCurveImage(SexyFramework.Graphics.Color curve_color, int detail)
		{
			mCurveDetail = detail;
			mCurveColor = curve_color;
			mImage = null;
			mImage = new MemoryImage();
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			for (int i = 0; i < mCount; i++)
			{
				num5 += mTimes[i];
			}
			Vector2[] array = new Vector2[detail];
			for (int j = 0; j < detail; j++)
			{
				float num6 = (float)j / (float)detail;
				array[j] = Evaluate(num6 * num5);
				if (array[j].X < num)
				{
					num = array[j].X;
				}
				if (array[j].X > num3)
				{
					num3 = array[j].X;
				}
				if (array[j].Y < num2)
				{
					num2 = array[j].Y;
				}
				if (array[j].Y > num4)
				{
					num4 = array[j].Y;
				}
			}
			int num7 = (int)num3 - (int)num;
			int num8 = (int)num4 - (int)num2;
			mImageX = (int)num;
			mImageY = (int)num2;
			mImage.Create(num7, num8);
			mImage.Clear();
			SexyFramework.Graphics.Graphics graphics = new SexyFramework.Graphics.Graphics(mImage);
			graphics.SetColor(0, 0, 0, 0);
			graphics.FillRect(0, 0, num7, num8);
			graphics.SetColor(curve_color);
			for (int k = 0; k < detail; k++)
			{
				float num9 = array[k].X - num;
				if (num9 < 0f)
				{
					num9 = 0f;
				}
				else if (num9 >= (float)num7)
				{
					num9 = num7 - 1;
				}
				float num10 = array[k].Y - num2;
				if (num10 < 0f)
				{
					num10 = 0f;
				}
				else if (num10 >= (float)num8)
				{
					num10 = num8 - 1;
				}
				graphics.FillRect((int)num9, (int)num10, 1, 1);
			}
			graphics.ClearRenderContext();
		}

		public void Draw(SexyFramework.Graphics.Graphics g, float scale)
		{
			if (mImage != null)
			{
				if (Common._eq(scale, 1f))
				{
					g.DrawImage(mImage, mImageX, mImageY);
				}
				else
				{
					g.DrawImage(mImage, (int)((float)mImageX * scale), (int)((float)mImageY * scale), (int)((float)mImage.mWidth * scale), (int)((float)mImage.mHeight * scale));
				}
				return;
			}
			g.SetColor(255, 0, 255);
			for (int i = 0; i < mCount - 1; i++)
			{
				SubdivideRender(g, mPositions[i], mControls[2 * i], mControls[2 * i + 1], mPositions[i + 1]);
			}
			g.FillRect((int)(mPositions[mCount - 1].X - 2f), (int)(mPositions[mCount - 1].Y - 2f), 4, 4);
			g.SetColor(255, 0, 0);
			for (int j = 0; j < mCount; j++)
			{
				g.FillRect((int)(mPositions[j].X - 2f), (int)(mPositions[j].Y - 2f), 4, 4);
			}
			g.SetColor(255, 255, 0);
			for (int k = 0; k < 2 * mCount - 2; k++)
			{
				g.FillRect((int)(mControls[k].X - 2f), (int)(mControls[k].Y - 2f), 4, 4);
			}
		}

		public Vector2 Velocity(float t, bool clamp)
		{
			if (mCount < 2)
			{
				return new Vector2(0f, 0f);
			}
			if (t <= mTimes[0])
			{
				if (!clamp)
				{
					return new Vector2(0f, 0f);
				}
				return mPositions[0];
			}
			if (t >= mTimes[mCount - 1])
			{
				if (!clamp)
				{
					return new Vector2(0f, 0f);
				}
				return mPositions[mCount - 1];
			}
			int i;
			for (i = 0; i < mCount - 1 && !(t < mTimes[i + 1]); i++)
			{
			}
			float num = mTimes[i];
			float num2 = mTimes[i + 1];
			float num3 = (t - num) / (num2 - num);
			Vector2 vector = mPositions[i + 1] - mControls[2 * i + 1] * 3f + mControls[2 * i] * 3f - mPositions[i];
			Vector2 vector2 = mControls[2 * i + 1] * 6f - mControls[2 * i] * 12f + mPositions[i] * 6f;
			Vector2 vector3 = mControls[2 * i] * 3f - mPositions[i] * 3f;
			return vector3 + (vector2 + vector * num3 * 3f) * num3;
		}

		public Vector2 Velocity(float t)
		{
			return Velocity(t, true);
		}

		public Vector2 Acceleration(float t)
		{
			if (mCount < 2)
			{
				return new Vector2(0f, 0f);
			}
			if (t <= mTimes[0])
			{
				return mPositions[0];
			}
			if (t >= mTimes[mCount - 1])
			{
				return mPositions[mCount - 1];
			}
			int i;
			for (i = 0; i < mCount - 1 && !(t < mTimes[i + 1]); i++)
			{
			}
			float num = mTimes[i];
			float num2 = mTimes[i + 1];
			float num3 = (t - num) / (num2 - num);
			Vector2 vector = mPositions[i + 1] - mControls[2 * i + 1] * 3f + mControls[2 * i] * 3f - mPositions[i];
			Vector2 vector2 = mControls[2 * i + 1] * 6f - mControls[2 * i] * 12f + mPositions[i] * 6f;
			return vector2 + vector * num3 * 6f;
		}

		public float ArcLength(float t1, float t2)
		{
			if (t2 <= t1)
			{
				return 0f;
			}
			if (t1 < mTimes[0])
			{
				t1 = mTimes[0];
			}
			if (t2 > mTimes[mCount - 1])
			{
				t2 = mTimes[mCount - 1];
			}
			int i;
			for (i = 0; i < mCount - 1 && !(t1 < mTimes[i + 1]); i++)
			{
			}
			float u = (t1 - mTimes[i]) / (mTimes[i + 1] - mTimes[i]);
			int j;
			for (j = 0; j < mCount - 1 && !(t2 <= mTimes[j + 1]); j++)
			{
			}
			float u2 = (t2 - mTimes[j]) / (mTimes[j + 1] - mTimes[j]);
			if (i == j)
			{
				return SegmentArcLength(i, u, u2);
			}
			float num = SegmentArcLength(i, u, 1f);
			for (int k = i + 1; k < j; k++)
			{
				num += mLengths[k];
			}
			return num + SegmentArcLength(j, 0f, u2);
		}

		public float GetTotalLength()
		{
			return mTotalLength;
		}

		public bool IsInitialized()
		{
			return mCount > 0;
		}

		public int GetNumPoints()
		{
			return mCount;
		}

		public void Clean()
		{
			mTimes = null;
			mLengths = null;
			mControls = null;
			mPositions = null;
			mImage = null;
			mCount = 0;
			mTotalLength = 0f;
		}

		private static float Distance(Vector2 p1, Vector2 p2, bool sqrt)
		{
			float num = p2.X - p1.X;
			float num2 = p2.Y - p1.Y;
			float num3 = num * num + num2 * num2;
			if (!sqrt)
			{
				return num3;
			}
			return (float)Math.Sqrt(num3);
		}
	}
}
