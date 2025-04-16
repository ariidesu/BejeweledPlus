using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public class Curve
	{
		public const int mPointCount = 4;

		public FPoint[] mPoint = new FPoint[4];

		public Curve()
		{
			for (int i = 0; i < 4; i++)
			{
				mPoint[i] = new FPoint(0f, 0f);
			}
		}

		public FPoint GetBezierPoint(float theT)
		{
			float num = 1f - theT;
			FPoint result = default(FPoint);
			result.mX = num * num * num * mPoint[0].mX + 3f * theT * (num * num) * mPoint[1].mX + 3f * theT * theT * num * mPoint[2].mX + theT * theT * theT * mPoint[3].mX;
			result.mY = num * num * num * mPoint[0].mY + 3f * theT * (num * num) * mPoint[1].mY + 3f * theT * theT * num * mPoint[2].mY + theT * theT * theT * mPoint[3].mY;
			return result;
		}

		public FPoint GetSplinePoint(float t)
		{
			FPoint result = default(FPoint);
			result.mX = mPoint[0].mX + 3f * t * (mPoint[1].mX - mPoint[0].mX) + t * t / 2f * 6f * (mPoint[0].mX - 2f * mPoint[1].mX + mPoint[2].mX) + t * t * t * (0f - mPoint[0].mX + 3f * mPoint[1].mX - 3f * mPoint[2].mX + mPoint[3].mX);
			result.mY = mPoint[0].mY + 3f * t * (mPoint[1].mY - mPoint[0].mY) + t * t / 2f * 6f * (mPoint[0].mY - 2f * mPoint[1].mY + mPoint[2].mY) + t * t * t * (0f - mPoint[0].mY + 3f * mPoint[1].mY - 3f * mPoint[2].mY + mPoint[3].mY);
			return result;
		}
	}
}
