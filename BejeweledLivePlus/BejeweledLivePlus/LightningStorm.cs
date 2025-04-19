using System;
using System.Collections.Generic;
using System.Linq;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class LightningStorm : IDisposable
	{
		public enum STORM
		{
			STORM_HORZ,
			STORM_VERT,
			STORM_BOTH,
			STORM_SHORT,
			STORM_STAR,
			STORM_SCREEN,
			STORM_FLAMING,
			STORM_HYPERCUBE
		}

		public Board mBoard;

		public int mCX;

		public int mCY;

		public int mUpdateCnt;

		public int mColor;

		public int mStormLength;

		public int mLastElectroSound;

		public int mStartPieceFlags;

		public int mMatchType;

		public int mLightningCount;

		public int mStormType;

		public float mExplodeTimer;

		public float mHoldDelay;

		public int mElectrocuterId;

		public int mMoveCreditId;

		public int mMatchId;

		public int mOriginCol;

		public int mOriginRow;

		public int mDist;

		public float mTimer;

		public List<LightningZap> mZaps = new List<LightningZap>();

		public List<Lightning> mLightningVector = new List<Lightning>();

		public List<int> mPieceIds = new List<int>();

		public List<ElectrocutedCel> mElectrocutedCelVector = new List<ElectrocutedCel>();

		public CurvedVal mNovaScale = new CurvedVal();

		public CurvedVal mNovaAlpha = new CurvedVal();

		public CurvedVal mNukeScale = new CurvedVal();

		public CurvedVal mNukeAlpha = new CurvedVal();

		public CurvedVal mLightingAlpha = new CurvedVal();

		public float mGemAlpha;

		public int mDoneDelay;

		private Piece[] UL_anElectrocutedPieces = new Piece[64];

		private Piece[] UL_anElectrocuterPieces = new Piece[64];

		private Piece[] UL_aMatchingPieces = new Piece[64];

		private static SexyVertex2D[,] DL_aTriVertices = new SexyVertex2D[(Bej3Com.NUM_LIGTNING_POINTS - 1) * 2, 3];

		public LightningStorm(Board theBoard, Piece thePiece)
		{
			Init(theBoard, thePiece, 2);
		}

		public LightningStorm(Board theBoard, Piece thePiece, int theType)
		{
			Init(theBoard, thePiece, theType);
		}

		public void Init(Board theBoard, Piece thePiece, int theType)
		{
			mBoard = theBoard;
			mUpdateCnt = 0;
			mLightningCount = 1;
			mGemAlpha = 1f;
			mMatchType = thePiece.mColor;
			mPieceIds.Add(thePiece.mId);
			mDoneDelay = 0;
			mLastElectroSound = 0;
			mStartPieceFlags = thePiece.mFlags;
			mMoveCreditId = thePiece.mMoveCreditId;
			mMatchId = thePiece.mMatchId;
			mExplodeTimer = 0f;
			if (thePiece.IsFlagSet(1u))
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eLIGHTNING_STORM_NOVA_SCALE, mNovaScale);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eLIGHTNING_STORM_NOVA_ALPHA, mNovaAlpha, mNovaScale);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eLIGHTNING_STORM_NUKE_SCALE, mNukeScale, mNovaScale);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eLIGHTNING_STORM_NUKE_ALPHA, mNukeAlpha, mNovaScale);
				mStormType = 6;
			}
			else
			{
				mStormType = theType;
			}
			mElectrocuterId = thePiece.mId;
			mCX = (int)thePiece.CX() - mBoard.GetBoardX();
			mCY = (int)thePiece.CY() - mBoard.GetBoardY();
			mOriginCol = thePiece.mCol;
			mOriginRow = thePiece.mRow;
			thePiece.mIsElectrocuting = true;
			if (mStormType != 7)
			{
				thePiece.mElectrocutePercent = 0.9f;
			}
			mColor = -1;
			mTimer = 0f;
			mDist = 0;
			mHoldDelay = 1f;
			mStormLength = ((mStormType == 3) ? GlobalMembers.M(3) : GlobalMembers.M(7));
			if (mStormType != 7)
			{
				for (int i = ((mStormType == 6) ? (-1) : 0); i <= ((mStormType == 6) ? 1 : 0); i++)
				{
					int rowAt = mBoard.GetRowAt((int)thePiece.mY + 50 + i * 100);
					if (rowAt >= 0 && rowAt < 8 && mStormType != 1)
					{
						if (mMatchType < 0)
						{
							mMatchType = SexyFramework.Common.Rand(Bej3Com.gElectColors.Length);
						}
						LightningZap item = new LightningZap(mBoard, Math.Max(0, mCX - mStormLength * 100 - 50), (int)thePiece.mY + 50 + i * 100, Math.Min(mBoard.GetColX(8), mCX + mStormLength * 100 + 50), (int)thePiece.mY + 50 + i * 100, Bej3Com.gElectColors[mMatchType], GlobalMembers.M(10f), mStormType == 6);
						mZaps.Add(item);
					}
					int colAt = mBoard.GetColAt((int)thePiece.mX + 50 + i * 100);
					if (colAt >= 0 && colAt < 8 && mStormType != 0)
					{
						LightningZap item2 = new LightningZap(mBoard, (int)thePiece.mX + 50 + i * 100, Math.Max(0, mCY - mStormLength * 100 - 50), (int)thePiece.mX + 50 + i * 100, Math.Min(mBoard.GetRowY(8), mCY + mStormLength * 100 + 50), Bej3Com.gElectColors[mMatchType], GlobalMembers.M(10f), mStormType == 6);
						mZaps.Add(item2);
					}
				}
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eLIGHTNING_STORM_LIGHTNING_ALPHA, mLightingAlpha);
			}
			else if (thePiece != null && thePiece.mColor == -1)
			{
				thePiece.mDestructing = true;
			}
		}

		public void Dispose()
		{
			for (int i = 0; i < Common.size(mZaps); i++)
			{
				if (mZaps[i] != null)
				{
					mZaps[i].Dispose();
				}
			}
			for (int j = 0; j < Common.size(mLightningVector); j++)
			{
				if (mLightningVector[j] != null)
				{
					mLightningVector[j].Dispose();
				}
			}
		}

		public void AddLightning(int theStartX, int theStartY, int theEndX, int theEndY)
		{
			Lightning lightning = new Lightning();
			lightning.mPercentDone = 0f;
			float num = theEndY - theStartY;
			float num2 = theEndX - theStartX;
			float num3 = (float)Math.Atan2(num, num2);
			float num4 = (float)Math.Sqrt(num2 * num2 + num * num);
			lightning.mPullX = (float)Math.Cos((double)num3 - 1.570795) * num4 * 0.4f;
			lightning.mPullY = (float)Math.Sin(num3 - 1.570795f) * num4 * 0.4f;
			for (int i = 0; i < Bej3Com.NUM_LIGTNING_POINTS; i++)
			{
				float num5 = (float)i / (float)(Bej3Com.NUM_LIGTNING_POINTS - 1);
				float mX = (float)theStartX * (1f - num5) + (float)theEndX * num5;
				float mY = (float)theStartY * (1f - num5) + (float)theEndY * num5;
				FPoint fPoint = lightning.mPoints[i, 0];
				FPoint fPoint2 = lightning.mPoints[i, 1];
				fPoint.mX = mX;
				fPoint.mY = mY;
				fPoint2.mX = mX;
				fPoint2.mY = mY;
				lightning.mPoints[i, 0] = fPoint;
				lightning.mPoints[i, 1] = fPoint2;
			}
			mLightningVector.Add(lightning);
		}

		public void UpdateLightning()
		{
			if (mDoneDelay > 0)
			{
				return;
			}
			bool flag = mBoard.WantsCalmEffects();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = false;
			bool flag3 = false;
			Piece[,] array = mBoard.mBoard;
			foreach (Piece piece in array)
			{
				if (piece == null)
				{
					continue;
				}
				if (piece.mExplodeDelay > 0)
				{
					flag3 = true;
				}
				if (piece.mIsElectrocuting)
				{
					if (flag)
					{
						if (piece.IsFlagSet(2u))
						{
							piece.mElectrocutePercent += GlobalMembers.M(0.0075f);
						}
						else
						{
							piece.mElectrocutePercent += GlobalMembers.M(0.01f);
						}
					}
					else if (piece.IsFlagSet(2u))
					{
						piece.mElectrocutePercent += GlobalMembers.M(0.01f);
					}
					else
					{
						piece.mElectrocutePercent += 0.015f;
					}
					if (piece.mElectrocutePercent > 1f)
					{
						mBoard.SetMoveCredit(piece, mMoveCreditId);
						piece.mExplodeSourceId = mElectrocuterId;
						piece.mExplodeSourceFlags |= mStartPieceFlags;
						if (!mBoard.TriggerSpecial(piece, mBoard.GetPieceById(mElectrocuterId)))
						{
							piece.mExplodeDelay = 1;
							piece.mMatchId = 1;
						}
					}
					else
					{
						if ((double)piece.mElectrocutePercent < 0.04)
						{
							UL_anElectrocuterPieces[num2++] = piece;
						}
						UL_anElectrocutedPieces[num++] = piece;
					}
				}
				else if ((piece.mColor == mColor || mColor == -1) && piece.GetScreenY() > -100f && !piece.IsFlagSet(6144u))
				{
					UL_aMatchingPieces[num3++] = piece;
				}
			}
			int num4 = (int)((20f / (num + 1f) + 5f) / 1.67f);
			if (flag)
			{
				num4 = (int)((float)num4 * GlobalMembers.M(1.4f));
			}
			if (mColor == -1)
			{
				num4 /= GlobalMembers.M(2);
			}
			if (num3 > 0 && (Common.size(mLightningVector) == 0 || mBoard.mRand.Next() % num4 == 0))
			{
				Piece piece2 = null;
				Piece piece3 = null;
				if (num2 > 0)
				{
					piece3 = UL_anElectrocuterPieces[mBoard.mRand.Next() % num2];
				}
				else if (num > 0)
				{
					piece3 = UL_anElectrocutedPieces[mBoard.mRand.Next() % num];
				}
				if (piece3 != null)
				{
					int num5 = int.MaxValue;
					for (int k = 0; k < num3; k++)
					{
						Piece piece4 = UL_aMatchingPieces[k];
						int num6 = Math.Min(Math.Abs(piece4.mCol - piece3.mCol), Math.Abs(piece4.mRow - piece3.mRow));
						if (num6 < num5)
						{
							piece2 = piece4;
						}
					}
					AddLightning((int)piece2.mX + 50, (int)piece2.mY + 50, (int)piece3.mX + 50, (int)piece3.mY + 50);
					Effect effect = mBoard.mPostFXManager.AllocEffect(Effect.Type.TYPE_LIGHT);
					effect.mFlags = 2;
					effect.mX = piece2.CX();
					effect.mY = piece2.CY();
					effect.mZ = GlobalMembers.M(0.08f);
					effect.mValue[0] = GlobalMembers.M(16.1f);
					effect.mValue[1] = GlobalMembers.M(-0.8f);
					effect.mAlpha = GlobalMembers.M(0f);
					effect.mDAlpha = GlobalMembers.M(0.1f);
					effect.mScale = GlobalMembers.M(140f);
					mBoard.mPostFXManager.AddEffect(effect);
					if (mUpdateCnt - mLastElectroSound >= GlobalMembers.M(20) || mLastElectroSound == 0)
					{
						if (flag)
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ELECTRO_PATH2, GlobalMembers.M(0), GlobalMembers.MS(0.67), GlobalMembers.MS(-1.0));
						}
						else
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_ELECTRO_PATH2);
						}
						mLastElectroSound = 0;
					}
				}
				else
				{
					piece2 = UL_aMatchingPieces[mBoard.mRand.Next() % num3];
				}
				piece2.mIsElectrocuting = true;
			}
			float num7 = (SexyFramework.GlobalMembers.gIs3D ? GlobalMembers.M(32f) : GlobalMembers.M(24f));
			for (int l = 0; l < Common.size(mLightningVector); l++)
			{
				Lightning lightning = mLightningVector[l];
				lightning.mPercentDone += 0.012f;
				if (lightning.mPercentDone > 1f)
				{
					lightning.Dispose();
					mLightningVector.RemoveAt(l);
					l--;
					continue;
				}
				float num8 = Math.Max(0f, 1f - (1f - lightning.mPercentDone) * 3f);
				if ((!flag || mUpdateCnt % GlobalMembers.M(5) != 0) && (flag || mUpdateCnt % 2 != 0))
				{
					continue;
				}
				float mX = lightning.mPoints[0, 0].mX;
				float mY = lightning.mPoints[0, 0].mY;
				float mX2 = lightning.mPoints[Bej3Com.NUM_LIGTNING_POINTS - 1, 0].mX;
				float mY2 = lightning.mPoints[Bej3Com.NUM_LIGTNING_POINTS - 1, 0].mY;
				for (int m = 0; m < Bej3Com.NUM_LIGTNING_POINTS; m++)
				{
					float num9 = (float)m / (float)(Bej3Com.NUM_LIGTNING_POINTS - 1);
					float num10 = 1f - Math.Abs(1f - num9 * 2f);
					float num11 = mX * (1f - num9) + mX2 * num9 + num10 * (GlobalMembersUtils.GetRandFloat() * 40f + num8 * lightning.mPullX);
					float num12 = mY * (1f - num9) + mY2 * num9 + num10 * (GlobalMembersUtils.GetRandFloat() * 40f + num8 * lightning.mPullY);
					if (flag)
					{
						num11 = mX * (1f - num9) + mX2 * num9 + num10 * (GlobalMembersUtils.GetRandFloat() * 15f + num8 * lightning.mPullX);
						num12 = mY * (1f - num9) + mY2 * num9 + num10 * (GlobalMembersUtils.GetRandFloat() * 15f + num8 * lightning.mPullY);
					}
					FPoint fPoint = lightning.mPoints[m, 0];
					FPoint fPoint2 = lightning.mPoints[m, 1];
					if (m == 0 || m == Bej3Com.NUM_LIGTNING_POINTS - 1)
					{
						fPoint.mX = num11;
						fPoint.mY = num12;
						fPoint2.mX = num11;
						fPoint2.mY = num12;
					}
					else
					{
						fPoint.mX = num11 + GlobalMembersUtils.GetRandFloat() * num7;
						fPoint.mY = num12 + GlobalMembersUtils.GetRandFloat() * num7;
						fPoint2.mX = num11 + GlobalMembersUtils.GetRandFloat() * num7;
						fPoint2.mY = num12 + GlobalMembersUtils.GetRandFloat() * num7;
					}
					lightning.mPoints[m, 0] = fPoint;
					lightning.mPoints[m, 1] = fPoint2;
				}
			}
			if (flag3 || num != 0 || num3 != 0 || Common.size(mLightningVector) != 0 || flag2)
			{
				return;
			}
			for (int n = 0; n < 8; n++)
			{
				for (int num13 = 0; num13 < 8; num13++)
				{
					Piece piece5 = mBoard.mBoard[n, num13];
					if (piece5 != null)
					{
						piece5.mFallVelocity = 0f;
					}
				}
			}
			mDoneDelay = 10;
		}

		public void DrawLightning(Graphics g)
		{
			Graphics3D graphics3D = g.Get3D();
			g.PushState();
			g.Translate(GlobalMembers.S(mBoard.GetBoardX()), GlobalMembers.S(mBoard.GetBoardY()));
			int num = 0;
			for (int i = 0; i < Common.size(mLightningVector); i++)
			{
				Lightning lightning = mLightningVector[i];
				float num2 = Math.Min((1f - lightning.mPercentDone) * 8f, 1f) * mBoard.GetPieceAlpha();
				int num3 = (int)((double)num2 * 255.0);
				if (graphics3D != null)
				{
					num = 0;
					for (int j = 0; j < Bej3Com.NUM_LIGTNING_POINTS - 1; j++)
					{
						FPoint fPoint = lightning.mPoints[j, 0];
						FPoint fPoint2 = lightning.mPoints[j, 1];
						FPoint fPoint3 = lightning.mPoints[j + 1, 0];
						FPoint fPoint4 = lightning.mPoints[j + 1, 1];
						float v = (float)j / (float)(Bej3Com.NUM_LIGTNING_POINTS - 1);
						float v2 = (float)(j + 1) / (float)(Bej3Com.NUM_LIGTNING_POINTS - 1);
						float x = GlobalMembers.S(fPoint.mX);
						float y = GlobalMembers.S(fPoint.mY);
						float x2 = GlobalMembers.S(fPoint4.mX);
						float y2 = GlobalMembers.S(fPoint4.mY);
						float x3 = GlobalMembers.S(fPoint3.mX);
						float y3 = GlobalMembers.S(fPoint3.mY);
						if (j == 0)
						{
							DL_aTriVertices[num, 0].x = x;
							DL_aTriVertices[num, 0].y = y;
							DL_aTriVertices[num, 0].u = 0.5f;
							DL_aTriVertices[num, 0].v = v;
							DL_aTriVertices[num, 1] = DL_aTriVertices[num, 1];
							DL_aTriVertices[num, 1].x = x2;
							DL_aTriVertices[num, 1].y = y2;
							DL_aTriVertices[num, 1].u = 1f;
							DL_aTriVertices[num, 1].v = v2;
							DL_aTriVertices[num, 2] = DL_aTriVertices[num, 2];
							DL_aTriVertices[num, 2].x = x3;
							DL_aTriVertices[num, 2].y = y3;
							DL_aTriVertices[num, 2].u = 0f;
							DL_aTriVertices[num, 2].v = v2;
							num++;
						}
						else if (j == Bej3Com.NUM_LIGTNING_POINTS - 2)
						{
							DL_aTriVertices[num, 0].x = x;
							DL_aTriVertices[num, 0].y = y;
							DL_aTriVertices[num, 0].u = 0f;
							DL_aTriVertices[num, 0].v = v;
							DL_aTriVertices[num, 1].x = GlobalMembers.S(fPoint2.mX);
							DL_aTriVertices[num, 1].y = GlobalMembers.S(fPoint2.mY);
							DL_aTriVertices[num, 1].u = 1f;
							DL_aTriVertices[num, 1].v = v;
							DL_aTriVertices[num, 2].x = x3;
							DL_aTriVertices[num, 2].y = y3;
							DL_aTriVertices[num, 2].u = 0.5f;
							DL_aTriVertices[num, 2].v = v2;
							num++;
						}
						else
						{
							DL_aTriVertices[num, 0].x = x;
							DL_aTriVertices[num, 0].y = y;
							DL_aTriVertices[num, 0].u = 0f;
							DL_aTriVertices[num, 0].v = v;
							DL_aTriVertices[num, 1].x = x2;
							DL_aTriVertices[num, 1].y = y2;
							DL_aTriVertices[num, 1].u = 1f;
							DL_aTriVertices[num, 1].v = v2;
							DL_aTriVertices[num, 2].x = x3;
							DL_aTriVertices[num, 2].y = y3;
							DL_aTriVertices[num, 2].u = 0f;
							DL_aTriVertices[num, 2].v = v2;
							num++;
							DL_aTriVertices[num, 0].x = x;
							DL_aTriVertices[num, 0].y = y;
							DL_aTriVertices[num, 0].u = 0f;
							DL_aTriVertices[num, 0].v = v;
							DL_aTriVertices[num, 1].x = GlobalMembers.S(fPoint2.mX);
							DL_aTriVertices[num, 1].y = GlobalMembers.S(fPoint2.mY);
							DL_aTriVertices[num, 1].u = 1f;
							DL_aTriVertices[num, 1].v = v;
							DL_aTriVertices[num, 2].x = x2;
							DL_aTriVertices[num, 2].y = y2;
							DL_aTriVertices[num, 2].u = 1f;
							DL_aTriVertices[num, 2].v = v2;
							num++;
						}
					}
					int num4 = SexyFramework.Common.Rand(5);
					Color theColor = Color.White;
					switch (num4)
					{
					case 0:
						theColor = Color.White;
						break;
					case 1:
						theColor = Color.Red;
						break;
					case 2:
						theColor = Color.Blue;
						break;
					case 3:
						theColor = Color.Green;
						break;
					case 4:
						theColor = Color.Yellow;
						break;
					}
					if (mColor >= 0)
					{
						theColor = new Color((int)((float)Bej3Com.gElectColors[mColor].mRed * num2), (int)((float)Bej3Com.gElectColors[mColor].mGreen * num2), (int)((float)Bej3Com.gElectColors[mColor].mBlue * num2));
					}
					g.DrawTrianglesTex(GlobalMembersResourcesWP.IMAGE_LIGHTNING_TEX, DL_aTriVertices, num, theColor, 1, g.mTransX, g.mTransY, true, Rect.INVALIDATE_RECT);
					g.DrawTrianglesTex(GlobalMembersResourcesWP.IMAGE_LIGHTNING_CENTER, DL_aTriVertices, num, new Color(num3, num3, num3), 1, g.mTransX, g.mTransY, true, Rect.INVALIDATE_RECT);
				}
				else
				{
					g.SetDrawMode(1);
					Color color = new Color((int)((float)Bej3Com.gElectColors[mColor].mRed * num2), (int)((float)Bej3Com.gElectColors[mColor].mGreen * num2), (int)((float)Bej3Com.gElectColors[mColor].mBlue * num2));
					Point[] array = new Point[3];
					for (int k = 0; k < Bej3Com.NUM_LIGTNING_POINTS - 1; k++)
					{
						FPoint fPoint5 = lightning.mPoints[k, 0];
						FPoint fPoint6 = lightning.mPoints[k, 1];
						FPoint fPoint7 = lightning.mPoints[k + 1, 0];
						FPoint fPoint8 = lightning.mPoints[k + 1, 1];
						float num5 = GlobalMembers.S(fPoint5.mX * 0.3f) + fPoint6.mX * 0.7f;
						float num6 = GlobalMembers.S(fPoint5.mY * 0.3f) + fPoint6.mY * 0.7f;
						float num7 = GlobalMembers.S(fPoint6.mX * 0.3f) + fPoint5.mX * 0.7f;
						float num8 = GlobalMembers.S(fPoint6.mY * 0.3f) + fPoint5.mY * 0.7f;
						float num9 = GlobalMembers.S(fPoint7.mX * 0.3f) + fPoint8.mX * 0.7f;
						float num10 = GlobalMembers.S(fPoint7.mY * 0.3f) + fPoint8.mY * 0.7f;
						float num11 = GlobalMembers.S(fPoint8.mX * 0.3f) + fPoint7.mX * 0.7f;
						float num12 = GlobalMembers.S(fPoint8.mY * 0.3f) + fPoint7.mY * 0.7f;
						float num13 = GlobalMembers.S(fPoint5.mX);
						float num14 = GlobalMembers.S(fPoint5.mY);
						float num15 = GlobalMembers.S(fPoint8.mX);
						float num16 = GlobalMembers.S(fPoint8.mY);
						float num17 = GlobalMembers.S(fPoint7.mX);
						float num18 = GlobalMembers.S(fPoint7.mY);
						g.SetColor(color);
						array[0].mX = (int)num13;
						array[0].mY = (int)num14;
						array[1].mX = (int)num15;
						array[1].mY = (int)num16;
						array[2].mX = (int)num17;
						array[2].mY = (int)num18;
						g.PolyFill(array, 3, false);
						array[0].mX = (int)num13;
						array[0].mY = (int)num14;
						array[1].mX = (int)GlobalMembers.S(fPoint6.mX);
						array[1].mY = (int)GlobalMembers.S(fPoint6.mY);
						array[2].mX = (int)num15;
						array[2].mY = (int)num16;
						g.PolyFill(array, 3, false);
						g.SetColor(new Color(num3, num3, num3));
						array[0].mX = (int)num5;
						array[0].mY = (int)num6;
						array[1].mX = (int)num11;
						array[1].mY = (int)num12;
						array[2].mX = (int)num9;
						array[2].mY = (int)num10;
						g.PolyFill(array, 3, false);
						array[0].mX = (int)num5;
						array[0].mY = (int)num6;
						array[1].mX = (int)num7;
						array[1].mY = (int)num8;
						array[2].mX = (int)num11;
						array[2].mY = (int)num12;
						g.PolyFill(array, 3, false);
					}
					g.SetDrawMode(0);
				}
			}
			g.PopState();
		}

		public void Update()
		{
			mUpdateCnt++;
			mNovaScale.IncInVal();
			mNukeScale.IncInVal();
			if (mStormType == 6 && mNukeScale.CheckInThreshold(GlobalMembers.M(1.6f)))
			{
				GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BOMB_EXPLODE, 0);
			}
			for (int i = 0; i < Common.size(mZaps); i++)
			{
				LightningZap lightningZap = mZaps[i];
				lightningZap.Update();
			}
			mGemAlpha = Math.Max(0f, mGemAlpha - GlobalMembers.M(0.01f) * 1.67f);
			if (mStormType == 7)
			{
				UpdateLightning();
			}
		}

		public void Draw(Graphics g)
		{
			if (mNovaAlpha != null)
			{
				g.SetColorizeImages(true);
				g.SetColor(mNovaAlpha);
				Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BOOM_NOVA, GlobalMembers.S(mBoard.GetBoardX() + mCX), GlobalMembers.S(mBoard.GetBoardY() + mCY), (float)(double)mNovaScale, (float)(double)mNovaScale);
			}
			if (mNukeAlpha != null)
			{
				g.SetColorizeImages(true);
				g.SetColor(mNukeAlpha);
				Utils.DrawImageCentered(g, GlobalMembersResourcesWP.IMAGE_BOOM_NUKE, GlobalMembers.S(mBoard.GetBoardX() + mCX), GlobalMembers.S(mBoard.GetBoardY() + mCY), (float)(double)mNukeScale, (float)(double)mNukeScale);
			}
			for (int i = 0; i < mZaps.Count(); i++)
			{
				LightningZap lightningZap = mZaps[i];
				lightningZap.Draw(g);
			}
			if (mStormType == 7)
			{
				DrawLightning(g);
			}
		}
	}
}
