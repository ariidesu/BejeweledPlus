using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class LightningZap : IDisposable
	{
		public Board mBoard;

		public FPoint mStartPoint;

		public FPoint mEndPoint;

		public List<FPoint>[] mPoints = new List<FPoint>[2]
		{
			new List<FPoint>(),
			new List<FPoint>()
		};

		public float mPercentDone;

		public float mTimer;

		public float mDoneTime;

		public float mAngle;

		public float mLength;

		public Color mColor;

		public int mUpdates;

		public int mFrame;

		public bool mDeleteMe;

		public bool mFlaming;

		private static SexyVertex2D[,] LZ_Draw_aTriList_1 = new SexyVertex2D[Bej3Com.MAX_TRIS, 3];

		private static SexyVertex2D[,] LZ_Draw_aTriList_2 = new SexyVertex2D[Bej3Com.MAX_TRIS, 3];

		private static SexyVertex2D[] LZ_Draw_aTriVertices = new SexyVertex2D[4];

		public LightningZap(Board theBoard, int theStartX, int theStartY, int theEndX, int theEndY, Color theColor, float theTime, bool isFlamimg)
		{
			mBoard = theBoard;
			mDeleteMe = false;
			mFlaming = isFlamimg;
			mPercentDone = 0f;
			mDoneTime = theTime;
			mTimer = 0f;
			mColor = theColor;
			mStartPoint = new FPoint(theStartX, theStartY);
			mEndPoint = new FPoint(theEndX, theEndY);
			float num = mEndPoint.mY - mStartPoint.mY;
			float num2 = mEndPoint.mX - mStartPoint.mX;
			mAngle = (float)Math.Atan2(num, num2);
			mLength = (float)Math.Sqrt(num2 * num2 + num * num);
			mFrame = 0;
			mUpdates = 0;
			Update();
		}

		public void Dispose()
		{
		}

		public void Update()
		{
			Image iMAGE_LIGHTNING = GlobalMembersResourcesWP.IMAGE_LIGHTNING;
			bool flag = mBoard.WantsCalmEffects();
			mUpdates++;
			if (flag)
			{
				mTimer += GlobalMembers.M(0.05f);
			}
			else
			{
				mTimer += GlobalMembers.M(0.1f);
			}
			float num = mEndPoint.mX - mStartPoint.mX;
			float num2 = mEndPoint.mY - mStartPoint.mY;
			float num3 = Math.Max(GlobalMembers.M(1f), (float)Math.Sqrt(num * num + num2 * num2));
			float num4 = ConstantsWP.LIGHTNING_THICKNESS;
			float num5 = num4 * num2 / num3;
			float num6 = num4 * num / num3;
			if ((flag && mUpdates % GlobalMembers.M(7) == 0) || (!flag && mUpdates % GlobalMembers.M(4) == 0))
			{
				mPoints[0].Clear();
				mPoints[1].Clear();
				mFrame = BejeweledLivePlus.Misc.Common.Rand() % 5;
				int num7 = (int)Math.Max(1f, GlobalMembers.M(0.5f) * GlobalMembers.S(mLength) / (float)iMAGE_LIGHTNING.GetCelHeight()) + 1;
				for (int i = 0; i < num7; i++)
				{
					FPoint[] array = new FPoint[2]
					{
						default(FPoint),
						default(FPoint)
					};
					float num8 = (float)i / (float)(num7 - 1);
					int num9 = 1;
					if (i != 0 && i < num7 - 1)
					{
						num9 = Math.Max(GlobalMembers.M(80), (int)(GlobalMembers.M(160f) * 0.5f * mTimer / mDoneTime));
					}
					if (flag)
					{
						num9 = Math.Max(1, (int)((float)num9 * GlobalMembers.M(0.5f)));
					}
					array[0].mX = (array[1].mX = mStartPoint.mX + num8 * num + (float)(num9 / 2) - (float)(BejeweledLivePlus.Misc.Common.Rand() % num9));
					array[0].mY = (array[1].mY = mStartPoint.mY + num8 * num2 + (float)(num9 / 2) - (float)(BejeweledLivePlus.Misc.Common.Rand() % num9));
					array[0].mX -= num5;
					array[1].mX += num5;
					array[0].mY += num6;
					array[1].mY -= num6;
					mPoints[0].Add(array[0]);
					mPoints[1].Add(array[1]);
				}
			}
			mPercentDone = mTimer / mDoneTime;
			if (mPercentDone >= 1f)
			{
				mDeleteMe = true;
			}
		}

		public void Draw(Graphics g)
		{
			Image iMAGE_LIGHTNING = GlobalMembersResourcesWP.IMAGE_LIGHTNING;
			Graphics3D graphics3D = g.Get3D();
			float num;
			float num2 = (num = 5f / 32f);
			num2 *= (float)mFrame;
			num += num2;
			num2 += GlobalMembers.M(0.02f);
			num += GlobalMembers.M(-0.02f);
			float num3 = Math.Max(0f, Math.Min((1f - mPercentDone) * 8f, 1f)) * mBoard.GetPieceAlpha();
			bool flag = GlobalMembers.gApp.Is3DAccelerated();
			int num4 = Common.size(mPoints[0]);
			g.PushState();
			g.Translate(GlobalMembers.S(mBoard.GetBoardX()), GlobalMembers.S(mBoard.GetBoardY()));
			g.SetColorizeImages(true);
			graphics3D?.SetTextureWrap(0, true);
			int num5 = 0;
			int mAlpha = Math.Min(255, (int)(GlobalMembers.M(800f) * num3));
			Color color = mColor;
			color.mAlpha = mAlpha;
			Color theColor = default(Color);
			theColor.mRed = (GlobalMembers.M(255) + mColor.mRed) / 2;
			theColor.mGreen = (GlobalMembers.M(255) + mColor.mGreen) / 2;
			theColor.mBlue = (GlobalMembers.M(255) + mColor.mBlue) / 2;
			theColor.mAlpha = mAlpha;
			float num6 = 0f;
			float num7 = 0f;
			if (flag)
			{
				num6 = g.mTransX;
				num7 = g.mTransY;
			}
			for (int i = 0; i < num4 - 1; i++)
			{
				float v = 0.1f;
				float v2 = 0.9f;
				float num8 = GlobalMembers.M(0f);
				SexyVertex2D[] lZ_Draw_aTriVertices = LZ_Draw_aTriVertices;
				float num9 = mPoints[0][i].mX - mPoints[0][i + 1].mX + mPoints[1][i].mX - mPoints[1][i + 1].mX;
				float num10 = mPoints[0][i].mY - mPoints[0][i + 1].mY + mPoints[1][i].mY - mPoints[1][i + 1].mY;
				float num11 = 1f / Math.Max(GlobalMembers.M(1f), (float)Math.Sqrt(num9 * num9 + num10 * num10));
				float num12 = num9;
				num9 = num8 * (num10 * num11);
				num10 = num8 * (num12 * num11);
				float x = num6 + GlobalMembers.S(mPoints[0][i].mX - num9);
				float y = num7 + GlobalMembers.S(mPoints[0][i].mY - num10);
				float x2 = num6 + GlobalMembers.S(mPoints[1][i].mX + num9);
				float y2 = num7 + GlobalMembers.S(mPoints[1][i].mY + num10);
				float x3 = num6 + GlobalMembers.S(mPoints[1][i + 1].mX + num9);
				float y3 = num7 + GlobalMembers.S(mPoints[1][i + 1].mY + num10);
				float x4 = num6 + GlobalMembers.S(mPoints[0][i + 1].mX - num9);
				float y4 = num7 + GlobalMembers.S(mPoints[0][i + 1].mY - num10);
				lZ_Draw_aTriVertices[0].x = x;
				lZ_Draw_aTriVertices[0].y = y;
				lZ_Draw_aTriVertices[0].u = 0f;
				lZ_Draw_aTriVertices[0].v = v;
				lZ_Draw_aTriVertices[1].x = x2;
				lZ_Draw_aTriVertices[1].y = y2;
				lZ_Draw_aTriVertices[1].u = 1f;
				lZ_Draw_aTriVertices[1].v = v;
				lZ_Draw_aTriVertices[2].x = x3;
				lZ_Draw_aTriVertices[2].y = y3;
				lZ_Draw_aTriVertices[2].u = 1f;
				lZ_Draw_aTriVertices[2].v = v2;
				lZ_Draw_aTriVertices[3].x = x4;
				lZ_Draw_aTriVertices[3].y = y4;
				lZ_Draw_aTriVertices[3].u = 0f;
				lZ_Draw_aTriVertices[3].v = v2;
				LZ_Draw_aTriList_1[num5, 0] = LZ_Draw_aTriVertices[0];
				LZ_Draw_aTriList_1[num5, 1] = LZ_Draw_aTriVertices[1];
				LZ_Draw_aTriList_1[num5, 2] = LZ_Draw_aTriVertices[2];
				LZ_Draw_aTriList_1[num5 + 1, 0] = LZ_Draw_aTriVertices[2];
				LZ_Draw_aTriList_1[num5 + 1, 1] = LZ_Draw_aTriVertices[3];
				LZ_Draw_aTriList_1[num5 + 1, 2] = LZ_Draw_aTriVertices[0];
				lZ_Draw_aTriVertices = LZ_Draw_aTriVertices;
				lZ_Draw_aTriVertices[0].u = num2;
				lZ_Draw_aTriVertices[1].u = num;
				lZ_Draw_aTriVertices[2].u = num;
				lZ_Draw_aTriVertices[3].u = num2;
				LZ_Draw_aTriList_2[num5, 0] = LZ_Draw_aTriVertices[0];
				LZ_Draw_aTriList_2[num5, 1] = LZ_Draw_aTriVertices[1];
				LZ_Draw_aTriList_2[num5, 2] = LZ_Draw_aTriVertices[2];
				LZ_Draw_aTriList_2[num5 + 1, 0] = LZ_Draw_aTriVertices[2];
				LZ_Draw_aTriList_2[num5 + 1, 1] = LZ_Draw_aTriVertices[3];
				LZ_Draw_aTriList_2[num5 + 1, 2] = LZ_Draw_aTriVertices[0];
				num5 += 2;
				// if (num5 >= Bej3Com.MAX_TRIS)
				// {
				// 	break;
				// }
			}
			if (flag)
			{
				g.DrawTrianglesTex(GlobalMembersResourcesWP.IMAGE_GRITTYBLURRY, LZ_Draw_aTriList_1, num5, color, 1, 0f, 0f, true, Rect.INVALIDATE_RECT);
				g.DrawTrianglesTex(GlobalMembersResourcesWP.IMAGE_GRITTYBLURRY, LZ_Draw_aTriList_1, num5, color, 1, 0f, 0f, true, Rect.INVALIDATE_RECT);
				g.DrawTrianglesTex(iMAGE_LIGHTNING, LZ_Draw_aTriList_1, num5, theColor, 1, 0f, 0f, true, Rect.INVALIDATE_RECT);
				g.DrawTrianglesTex(iMAGE_LIGHTNING, LZ_Draw_aTriList_2, num5, theColor, 1, 0f, 0f, true, Rect.INVALIDATE_RECT);
			}
			else
			{
				g.SetColor(color);
				g.SetDrawMode(1);
				g.DrawTrianglesTex(iMAGE_LIGHTNING, LZ_Draw_aTriList_1, num5);
				g.DrawTrianglesTex(iMAGE_LIGHTNING, LZ_Draw_aTriList_2, num5);
			}
			g.PopState();
		}
	}
}
