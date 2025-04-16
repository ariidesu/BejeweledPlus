using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class DiamondEffect : IDisposable
	{
		public class Sparkle
		{
			public float mX;

			public float mY;

			public int mUpdateCnt;

			public Sparkle(float x, float y, int updateCnt)
			{
				mX = x;
				mY = y;
				mUpdateCnt = updateCnt;
			}
		}

		public class SubImage
		{
			public List<Sparkle> mSparkles = new List<Sparkle>();

			public int mImageId;
		}

		public List<SubImage> mSubImgs = new List<SubImage>();

		public int mBaseImg;

		public int mDirtImg;

		public int mDiamondId;

		public int mParticleCount;

		public DiamondEffect(int theDiamondId)
		{
			int[] array = new int[4] { 1185, 1195, 1205, 1214 };
			int[] array2 = new int[4] { 1190, 1200, 1209, 1219 };
			int[,] array3 = new int[4, 5]
			{
				{ 4, 1186, 1187, 1188, 1189 },
				{ 4, 1196, 1197, 1198, 1199 },
				{ 3, 1206, 1207, 1208, -1 },
				{ 4, 1215, 1216, 1217, 1218 }
			};
			int[,] array4 = new int[4, 5]
			{
				{ 4, 1191, 1192, 1193, 1194 },
				{ 4, 1201, 1202, 1203, 1204 },
				{ 4, 1210, 1211, 1212, 1213 },
				{ 4, 1220, 1221, 1222, 1223 }
			};
			mDiamondId = theDiamondId;
			mParticleCount = 0;
			theDiamondId--;
			mBaseImg = array[mDiamondId - 1];
			mDirtImg = array2[mDiamondId - 1];
			for (int i = 1; i <= array3[theDiamondId, 0]; i++)
			{
				SubImage item = new SubImage
				{
					mImageId = array3[theDiamondId, i]
				};
				mSubImgs.Add(item);
			}
			mParticleCount = array4[theDiamondId, 0];
			for (int j = 0; j < mSubImgs.Count; j++)
			{
				mSubImgs[j].mSparkles.Capacity = 64;
			}
		}

		public void Dispose()
		{
		}

		public void Update()
		{
			for (int i = 0; i < mSubImgs.Count; i++)
			{
				int num = 0;
				while (num < mSubImgs[i].mSparkles.Count)
				{
					if (mSubImgs[i].mSparkles[num].mUpdateCnt >= GlobalMembers.M(100))
					{
						mSubImgs[i].mSparkles[num] = mSubImgs[i].mSparkles[mSubImgs[i].mSparkles.Count - 1];
						mSubImgs[i].mSparkles.RemoveAt(mSubImgs[i].mSparkles.Count - 1);
					}
					else
					{
						mSubImgs[i].mSparkles[num].mUpdateCnt++;
						num++;
					}
				}
				if (Common.Rand() % GlobalMembers.M(15) == 0)
				{
					Sparkle item = new Sparkle(Common.Rand(1f), Common.Rand(1f), 0);
					mSubImgs[i].mSparkles.Add(item);
				}
			}
		}

		public void Draw(Graphics g, int theX, int theY, bool theDrawParticles)
		{
			if (theDrawParticles)
			{
				g.DrawImage(GlobalMembersResourcesWP.GetImageById(mBaseImg), theX, theY);
			}
		}

		public void DrawDirt(Graphics g, int theX, int theY)
		{
			if (mDirtImg != -1)
			{
				g.DrawImage(GlobalMembersResourcesWP.GetImageById(mDirtImg), theX, theY);
			}
		}

		public int GetExplodeSubId(int theRandSeed)
		{
			int[,] array = new int[4, 4]
			{
				{ 1191, 1192, 1193, 1194 },
				{ 1201, 1202, 1203, 1204 },
				{ 1210, 1211, 1212, 1213 },
				{ 1220, 1221, 1222, 1223 }
			};
			return array[mDiamondId - 1, theRandSeed % mParticleCount];
		}
	}
}
