using System;
using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class TimeBonusEffect : Effect
	{
		private enum BatchType
		{
			Electro,
			Center
		}

		private static SimpleObjectPool thePool_ = null;

		private static TriangleArray batchElectro_;

		private static TriangleArray batchCenter_;

		private bool mUpdated;

		public List<ElectroBolt> mElectroBoltVector = new List<ElectroBolt>();

		public int mGemColor;

		public int mTimeBonus;

		public CurvedVal mCirclePct = new CurvedVal();

		public CurvedVal mRadiusScale = new CurvedVal();

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(TimeBonusEffect));
		}

		public static TimeBonusEffect alloc(Piece thePiece)
		{
			TimeBonusEffect timeBonusEffect = (TimeBonusEffect)thePool_.alloc();
			timeBonusEffect.init(thePiece);
			return timeBonusEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}

		public static void batchInit()
		{
			batchElectro_ = new TriangleArray();
			batchCenter_ = new TriangleArray();
		}

		public static void batchBegin()
		{
			batchElectro_.clear();
			batchCenter_.clear();
		}

		public static void batchEnd(Graphics g)
		{
			g.SetDrawMode(Graphics.DrawMode.Additive);
			if (batchElectro_.count() > 0)
			{
				g.DrawTrianglesTex(GlobalMembersResourcesWP.IMAGE_ELECTROTEX, batchElectro_.toArray(), batchElectro_.count());
			}
			if (batchCenter_.count() > 0)
			{
				g.DrawTrianglesTex(GlobalMembersResourcesWP.IMAGE_ELECTROTEX_CENTER, batchCenter_.toArray(), batchCenter_.count());
			}
		}

		private static void batchAddTriangle(BatchType type, SexyVertex2D v1, SexyVertex2D v2, SexyVertex2D v3)
		{
			switch (type)
			{
			case BatchType.Electro:
				batchElectro_.add(v1, v2, v3);
				break;
			case BatchType.Center:
				batchCenter_.add(v1, v2, v3);
				break;
			}
		}

		public TimeBonusEffect()
			: base(Type.TYPE_TIME_BONUS)
		{
		}

		public void init(Piece thePiece)
		{
			init(Type.TYPE_TIME_BONUS);
			mPieceRel = thePiece;
			mGemColor = thePiece.mColor;
			mTimeBonus = thePiece.mCounter;
			mDAlpha = 0f;
			mRadiusScale.SetConstant(1.0);
			mUpdated = false;
		}

		public override void Dispose()
		{
			mElectroBoltVector.Clear();
			base.Dispose();
		}

		public override void Update()
		{
			mUpdated = true;
			mCirclePct.IncInVal();
			mRadiusScale.IncInVal();
			Piece piece = mPieceRel;
			if (piece != null)
			{
				mOverlay = false;
				mX = piece.GetScreenX() + 50f;
				mY = piece.GetScreenY() + 50f;
				mTimeBonus = piece.mCounter;
				for (int i = 0; i < mFXManager.mBoard.mLightningStorms.Count; i++)
				{
					if (mFXManager.mBoard.mLightningStorms[i].mStormType == 7 && mFXManager.mBoard.mLightningStorms[i].mColor == piece.mColor)
					{
						mOverlay = true;
					}
				}
			}
			int theFrame = 0;
			if (piece != null)
			{
				theFrame = Math.Min(19, (int)(20f * piece.mRotPct));
			}
			float num = (float)Math.Min(4, mTimeBonus - 1) / 4f;
			bool flag = GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.12f) * num;
			if (!SexyFramework.GlobalMembers.gIs3D)
			{
				flag = GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.001f) * num;
			}
			if (mElectroBoltVector.Count < Math.Min(GlobalMembers.M(3), mTimeBonus * 2 - 1) || flag)
			{
				ElectroBolt electroBolt = new ElectroBolt();
				electroBolt.mHitOtherGem = false;
				electroBolt.mCrossover = !electroBolt.mHitOtherGem && GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.02f);
				if (electroBolt.mHitOtherGem)
				{
					electroBolt.mAngStart = Math.Abs(GlobalMembersUtils.GetRandFloat()) * (float)Math.PI * 2f;
					Piece pieceAtScreenXY = mFXManager.mBoard.GetPieceAtScreenXY((int)(mX + 50f + (float)Math.Cos(electroBolt.mAngStart) * 100f * GlobalMembers.M(0.6f)), (int)(mY + 50f + (float)Math.Sin(electroBolt.mAngStart) * 100f * GlobalMembers.M(0.6f)));
					if (pieceAtScreenXY != null && pieceAtScreenXY != piece)
					{
						electroBolt.mHitOtherGemId = pieceAtScreenXY.mId;
					}
					else
					{
						electroBolt.mHitOtherGem = false;
					}
				}
				if (electroBolt.mHitOtherGem)
				{
					electroBolt.mAngEnd = (float)Math.PI + electroBolt.mAngStart + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(0.5f);
					electroBolt.mAngStartD = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.03f);
					electroBolt.mAngEndD = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.03f);
				}
				else if (electroBolt.mCrossover)
				{
					electroBolt.mAngStart = Math.Abs(GlobalMembersUtils.GetRandFloat()) * (float)Math.PI * 2f;
					electroBolt.mAngEnd = electroBolt.mAngStart;
					electroBolt.mAngStartD = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.02f);
					if (electroBolt.mAngStartD < 0f)
					{
						electroBolt.mAngStartD += GlobalMembers.M(-0.02f);
					}
					else
					{
						electroBolt.mAngStartD += GlobalMembers.M(0.02f);
					}
					electroBolt.mAngEndD = 0f - electroBolt.mAngStartD + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.02f);
				}
				else
				{
					electroBolt.mAngStart = Math.Abs(GlobalMembersUtils.GetRandFloat()) * (float)Math.PI * 2f;
					electroBolt.mAngEnd = electroBolt.mAngStart + Math.Abs(GlobalMembersUtils.GetRandFloat()) * GlobalMembers.M(0.5f) + GlobalMembers.M(0.5f);
					electroBolt.mAngStartD = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.0075f);
					electroBolt.mAngEndD = electroBolt.mAngStartD + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.002f);
				}
				electroBolt.mNumMidPoints = 2;
				for (int j = 0; j < electroBolt.mNumMidPoints; j++)
				{
					electroBolt.mMidPointsPos[j] = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(10f);
					electroBolt.mMidPointsPosD[j] = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(0.2f);
				}
				mElectroBoltVector.Add(electroBolt);
			}
			for (int k = 0; k < mElectroBoltVector.Count; k++)
			{
				ElectroBolt electroBolt2 = mElectroBoltVector[k];
				electroBolt2.mAngStart += electroBolt2.mAngStartD;
				electroBolt2.mAngEnd += electroBolt2.mAngEndD;
				bool flag2 = false;
				for (int l = 0; l < electroBolt2.mNumMidPoints; l++)
				{
					electroBolt2.mMidPointsPos[l] += electroBolt2.mMidPointsPosD[l];
					if (electroBolt2.mHitOtherGem)
					{
						if (Math.Abs(electroBolt2.mMidPointsPos[l]) >= (float)GlobalMembers.M(25))
						{
							electroBolt2.mMidPointsPosD[l] *= -0.65f;
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.2f))
						{
							electroBolt2.mMidPointsPos[l] = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(15f);
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.05f))
						{
							electroBolt2.mMidPointsPosD[l] += GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(1.5f);
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.05f))
						{
							electroBolt2.mMidPointsPosD[l] = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(1.5f);
						}
					}
					else if (electroBolt2.mCrossover)
					{
						if (Math.Abs(electroBolt2.mMidPointsPos[l]) >= (float)GlobalMembers.M(25))
						{
							electroBolt2.mMidPointsPosD[l] *= -0.65f;
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.2f))
						{
							electroBolt2.mMidPointsPos[l] = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(15f);
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.1f))
						{
							electroBolt2.mMidPointsPosD[l] += GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(1.5f);
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.1f))
						{
							electroBolt2.mMidPointsPosD[l] = GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(1.5f);
						}
					}
					else
					{
						if (electroBolt2.mMidPointsPos[l] <= 0f)
						{
							electroBolt2.mMidPointsPos[l] = 0f;
							electroBolt2.mMidPointsPosD[l] = GlobalMembersUtils.GetRandFloatU() * GlobalMembers.M(0.1f);
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.05f))
						{
							float num2 = (GlobalMembers.M(4f) - electroBolt2.mMidPointsPos[l]) * GlobalMembers.M(0.1f);
							electroBolt2.mMidPointsPosD[l] = num2 + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(1f);
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.025f))
						{
							electroBolt2.mMidPointsPos[l] = GlobalMembersUtils.GetRandFloatU() * GlobalMembers.M(18f);
						}
						else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.04f))
						{
							electroBolt2.mMidPointsPosD[l] += GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(2.5f);
						}
						if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.1f))
						{
							float num3 = 0f;
							float num4 = 0f;
							if (l - 1 >= 0)
							{
								num3 = electroBolt2.mMidPointsPos[l - 1];
							}
							if (l + 1 < electroBolt2.mNumMidPoints)
							{
								num4 = electroBolt2.mMidPointsPos[l + 1];
							}
							electroBolt2.mMidPointsPos[l] = (electroBolt2.mMidPointsPos[l] + num3 + num4) / 3f;
						}
						if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.2f))
						{
							int num5 = l + BejeweledLivePlus.Misc.Common.Rand() % 3 - 1;
							if (num5 >= 0 && num5 < electroBolt2.mNumMidPoints)
							{
								float num6 = electroBolt2.mMidPointsPos[num5] - electroBolt2.mMidPointsPos[l];
								electroBolt2.mMidPointsPosD[l] += num6 * GlobalMembers.M(0.2f);
							}
						}
						if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.1f))
						{
							float num7 = 0f;
							float num8 = 0f;
							if (l - 1 >= 0)
							{
								num7 = electroBolt2.mMidPointsPosD[l - 1];
							}
							if (l + 1 < electroBolt2.mNumMidPoints)
							{
								num8 = electroBolt2.mMidPointsPosD[l + 1];
							}
							electroBolt2.mMidPointsPosD[l] = (num7 + num8) / 2f;
						}
					}
					if (electroBolt2.mMidPointsPos[l] > GlobalMembers.M(12f) || (num <= 0f && GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.01f)))
					{
						flag2 = true;
					}
				}
				if (electroBolt2.mHitOtherGem)
				{
					float num9 = Piece.GetAngleRadius(electroBolt2.mAngStart, mGemColor, theFrame) + GlobalMembers.M(0f);
					float num10 = (float)Math.Cos(electroBolt2.mAngStart) * num9;
					float num11 = (float)Math.Sin(electroBolt2.mAngStart) * num9;
					Piece pieceById = GlobalMembers.gApp.mBoard.GetPieceById(electroBolt2.mHitOtherGemId);
					if (pieceById != null)
					{
						float angleRadius = pieceById.GetAngleRadius(electroBolt2.mAngEnd);
						float num12 = (pieceById.mX - mX) / GlobalMembers.S(1f) + (float)Math.Cos(electroBolt2.mAngEnd) * angleRadius;
						float num13 = (pieceById.mY - mY) / GlobalMembers.S(1f) + (float)Math.Sin(electroBolt2.mAngEnd) * angleRadius;
						float num14 = num10 - num12;
						float num15 = num11 - num13;
						if (Math.Sqrt(num14 * num14 + num15 * num15) > (double)GlobalMembers.M(70f))
						{
							flag2 = true;
						}
						float num16 = (float)Math.Atan2(pieceById.mY - mY, pieceById.mX - mX);
						float num17 = (float)Math.Cos(electroBolt2.mAngStart) * (float)Math.Cos(num16) + (float)Math.Sin(electroBolt2.mAngStart) * (float)Math.Sin(num16);
						if (num17 < GlobalMembers.M(0.8f))
						{
							flag2 = true;
						}
						float num18 = (float)Math.Cos(electroBolt2.mAngEnd) * (float)Math.Cos(num16 + (float)Math.PI) + (float)Math.Sin(electroBolt2.mAngEnd) * (float)Math.Sin(num16 + (float)Math.PI);
						if (num18 < GlobalMembers.M(0.8f))
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
					if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.001f))
					{
						flag2 = true;
					}
				}
				else if (electroBolt2.mCrossover)
				{
					if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.001f))
					{
						flag2 = true;
					}
					if (Math.Abs(electroBolt2.mAngStart - electroBolt2.mAngEnd) >= (float)Math.PI * 2f)
					{
						flag2 = true;
					}
				}
				else if (GlobalMembersUtils.GetRandFloatU() < GlobalMembers.M(0.005f))
				{
					flag2 = true;
				}
				if (flag2)
				{
					mElectroBoltVector.RemoveAt(k);
					k--;
				}
			}
			if (BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(30) == 0)
			{
				Effect effect = mFXManager.AllocEffect(Type.TYPE_LIGHT);
				effect.mFlags = 2;
				effect.mX = mX + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(20f);
				effect.mY = mY + GlobalMembersUtils.GetRandFloat() * GlobalMembers.M(20f);
				effect.mZ = GlobalMembers.M(0.08f);
				effect.mValue[0] = GlobalMembers.M(30f);
				effect.mValue[1] = GlobalMembers.M(-0.1f);
				effect.mAlpha = GlobalMembers.M(0f);
				effect.mDAlpha = GlobalMembers.M(0.1f);
				effect.mScale = GlobalMembers.M(140f);
				effect.mColor = new Color(GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255));
				if (BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(2) != 0 && mPieceRel != null)
				{
					effect.mPieceId = (uint)mPieceRel.mId;
				}
				mFXManager.AddEffect(effect);
			}
			if (mPieceRel != null && (piece == null || (!piece.IsFlagSet(131072u) && mElectroBoltVector.Count == 0)))
			{
				mDeleteMe = true;
			}
		}

		public void DrawElectroLine(Graphics g, Image theImage, float theStartX, float theStartY, float theEndX, float theEndY, float theWidth, Color theColor1, Color theColor2)
		{
			float num = theEndX - theStartX;
			float num2 = theEndY - theStartY;
			float num3 = (float)Math.Atan2(num2, num);
			float num4 = (float)Math.Cos(num3);
			float num5 = (float)Math.Cos(num3 + (float)Math.PI / 2f);
			float num6 = (float)Math.Sin(num3 + (float)Math.PI / 2f);
			float num7 = theStartX + num4 * (0f - theWidth);
			float num8 = theEndX + num4 * theWidth;
			uint theColor3 = (uint)theColor1.ToInt();
			uint theColor4 = (uint)theColor2.ToInt();
			BatchType type = BatchType.Electro;
			if (theImage == GlobalMembersResourcesWP.IMAGE_ELECTROTEX_CENTER)
			{
				type = BatchType.Center;
			}
			batchAddTriangle(type, new SexyVertex2D(num7 + num5 * theWidth, theStartY + num6 * theWidth, 0f, 0f, theColor3), new SexyVertex2D(num7 + num5 * (0f - theWidth), theStartY + num6 * (0f - theWidth), 0f, 1f, theColor3), new SexyVertex2D(theStartX + num5 * theWidth, theStartY + num6 * theWidth, 0.5f, 0f, theColor3));
			batchAddTriangle(type, new SexyVertex2D(num7 + num5 * (0f - theWidth), theStartY + num6 * (0f - theWidth), 0f, 1f, theColor3), new SexyVertex2D(theStartX + num5 * theWidth, theStartY + num6 * theWidth, 0.5f, 0f, theColor3), new SexyVertex2D(theStartX + num5 * (0f - theWidth), theStartY + num6 * (0f - theWidth), 0.5f, 1f, theColor3));
			batchAddTriangle(type, new SexyVertex2D(theStartX + num5 * theWidth, theStartY + num6 * theWidth, 0.5f, 0f, theColor3), new SexyVertex2D(theStartX + num5 * (0f - theWidth), theStartY + num6 * (0f - theWidth), 0.5f, 1f, theColor3), new SexyVertex2D(theEndX + num5 * theWidth, theEndY + num6 * theWidth, 0.5f, 0f, theColor4));
			batchAddTriangle(type, new SexyVertex2D(theStartX + num5 * (0f - theWidth), theStartY + num6 * (0f - theWidth), 0.5f, 1f, theColor3), new SexyVertex2D(theEndX + num5 * theWidth, theEndY + num6 * theWidth, 0.5f, 0f, theColor4), new SexyVertex2D(theEndX + num5 * (0f - theWidth), theEndY + num6 * (0f - theWidth), 0.5f, 1f, theColor4));
			batchAddTriangle(type, new SexyVertex2D(theEndX + num5 * theWidth, theEndY + num6 * theWidth, 0.5f, 0f, theColor4), new SexyVertex2D(theEndX + num5 * (0f - theWidth), theEndY + num6 * (0f - theWidth), 0.5f, 1f, theColor4), new SexyVertex2D(num8 + num5 * theWidth, theEndY + num6 * theWidth, 1f, 0f, theColor4));
			batchAddTriangle(type, new SexyVertex2D(theEndX + num5 * (0f - theWidth), theEndY + num6 * (0f - theWidth), 0.5f, 1f, theColor4), new SexyVertex2D(num8 + num5 * theWidth, theEndY + num6 * theWidth, 1f, 0f, theColor4), new SexyVertex2D(num8 + num5 * (0f - theWidth), theEndY + num6 * (0f - theWidth), 1f, 1f, theColor4));
		}

		public override void Draw(Graphics g)
		{
			if (GlobalMembers.gGR.mIgnoreDraws || !mUpdated)
			{
				return;
			}
			float num = mAlpha * mFXManager.mAlpha;
			if (GlobalMembers.gApp.mBoard != null)
			{
				num *= GlobalMembers.gApp.mBoard.GetAlpha();
			}
			g.SetColor(new Color(255, 255, 255, (int)(255f * num)));
			g.PushColorMult();
			int num2 = (int)(mX - 50f);
			int num3 = (int)(mY - 50f);
			Piece piece = mPieceRel;
			int theFrame = 0;
			if (piece != null)
			{
				float num4 = (float)(double)piece.mScale;
				if (num4 != 1f)
				{
					Utils.PushScale(g, num4, num4, GlobalMembers.S(piece.CX()), GlobalMembers.S(piece.CY()));
				}
				theFrame = Math.Min(19, (int)(20f * piece.mRotPct));
				if (piece.mRotPct == 0f)
				{
					g.SetDrawMode(Graphics.DrawMode.Additive);
					g.SetColorizeImages(true);
					g.SetColor(new Color(255, 255, 255, (int)(Math.Min((float)mElectroBoltVector.Count * GlobalMembers.M(6f) + (float)GlobalMembers.M(64), 255f) * mAlpha)));
					Rect celRect = GlobalMembersResourcesWP.IMAGE_GEMOUTLINES.GetCelRect(0);
					celRect.mX += GlobalMembers.S(num2);
					celRect.mY += GlobalMembers.S(num3);
					GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_GEMOUTLINES, GlobalMembers.S(num2) + ConstantsWP.GEMOUTLINES_OFFSET_1, GlobalMembers.S(num3) + ConstantsWP.GEMOUTLINES_OFFSET_2, mGemColor);
					g.SetDrawMode(Graphics.DrawMode.Normal);
				}
			}
			g.SetDrawMode(Graphics.DrawMode.Additive);
			for (int i = 0; i < mElectroBoltVector.Count; i++)
			{
				ElectroBolt electroBolt = mElectroBoltVector[i];
				Color color = (electroBolt.mCrossover ? GlobalMembersEffects.gCrossoverColors[mGemColor] : GlobalMembersEffects.gArcColors[mGemColor]);
				color.mAlpha = (int)((float)GlobalMembers.M(255) * mAlpha);
				g.SetColor(color);
				float num5 = 0f;
				float angleRadius = Piece.GetAngleRadius(electroBolt.mAngStart, mGemColor, theFrame);
				angleRadius = (float)(((double)mCirclePct * GlobalMembers.MS(48.0) + (1.0 - (double)mCirclePct) * (double)angleRadius) * (double)mRadiusScale);
				float num6 = (float)Math.Cos(electroBolt.mAngStart) * angleRadius;
				float num7 = (float)Math.Sin(electroBolt.mAngStart) * angleRadius;
				float num8 = num6;
				float num9 = num7;
				float angleRadius2 = Piece.GetAngleRadius(electroBolt.mAngEnd, mGemColor, theFrame);
				angleRadius2 = (float)(((double)mCirclePct * GlobalMembers.MS(48.0) + (1.0 - (double)mCirclePct) * (double)angleRadius2) * (double)mRadiusScale);
				float num10 = (float)Math.Cos(electroBolt.mAngEnd) * angleRadius2;
				float num11 = (float)Math.Sin(electroBolt.mAngEnd) * angleRadius2;
				if (electroBolt.mHitOtherGem)
				{
					Piece pieceById = mFXManager.mBoard.GetPieceById(electroBolt.mHitOtherGemId);
					if (pieceById != null)
					{
						angleRadius2 = Piece.GetAngleRadius(electroBolt.mAngEnd, mGemColor, theFrame);
						angleRadius2 = (float)((double)mCirclePct * GlobalMembers.MS(48.0) + (1.0 - (double)mCirclePct) * (double)angleRadius2);
						num10 = (pieceById.mX - (float)num2) / GlobalMembers.S(1f) + (float)Math.Cos(electroBolt.mAngEnd) * angleRadius2;
						num11 = (pieceById.mY - (float)num3) / GlobalMembers.S(1f) + (float)Math.Sin(electroBolt.mAngEnd) * angleRadius2;
					}
				}
				for (int j = 0; j < electroBolt.mNumMidPoints + 1; j++)
				{
					float num12 = (float)(j + 1) / (float)(electroBolt.mNumMidPoints + 1);
					float num13 = (float)((double)electroBolt.mAngStart * (1.0 - (double)num12) + (double)(electroBolt.mAngEnd * num12));
					float num14 = 0f;
					if (j < electroBolt.mNumMidPoints)
					{
						num14 = electroBolt.mMidPointsPos[j];
					}
					float angleRadius3 = Piece.GetAngleRadius(num13, mGemColor, theFrame);
					angleRadius3 = (float)(((double)mCirclePct * GlobalMembers.MS(48.0) + (1.0 - (double)mCirclePct) * (double)angleRadius3 + (double)num14) * (double)mRadiusScale);
					float num15 = (float)Math.Cos(num13) * angleRadius3;
					float num16 = (float)Math.Sin(num13) * angleRadius3;
					if (electroBolt.mCrossover || electroBolt.mHitOtherGem)
					{
						float num17 = (float)Math.Atan2(num11 - num7, num10 - num6);
						num15 = (float)((double)num8 * (1.0 - (double)num12) + (double)(num10 * num12));
						num16 = (float)((double)num9 * (1.0 - (double)num12) + (double)(num11 * num12));
						if (j < electroBolt.mNumMidPoints)
						{
							num15 += (float)Math.Sin(num17) * electroBolt.mMidPointsPos[j];
							num16 += (float)Math.Cos(num17) * electroBolt.mMidPointsPos[j];
						}
					}
					Color theColor = new Color(color);
					Color theColor2 = new Color(color);
					if (!electroBolt.mCrossover && !electroBolt.mHitOtherGem)
					{
						theColor.mAlpha = (int)Math.Max(2f, 255f * (1f - num5 * GlobalMembers.M(0.03f)));
						theColor2.mAlpha = (int)Math.Max(2f, 255f * (1f - num14 * GlobalMembers.M(0.03f)));
					}
					theColor.mAlpha = (int)((float)theColor.mAlpha * num);
					theColor2.mAlpha = (int)((float)theColor2.mAlpha * num);
					DrawElectroLine(g, GlobalMembersResourcesWP.IMAGE_ELECTROTEX, (float)GlobalMembers.S(num2 + 50) + GlobalMembers.S(num6), (float)GlobalMembers.S(num3 + 50) + GlobalMembers.S(num7), (float)GlobalMembers.S(num2 + 50) + GlobalMembers.S(num15), (float)GlobalMembers.S(num3 + 50) + GlobalMembers.S(num16), electroBolt.mHitOtherGem ? GlobalMembers.MS(8f) : (electroBolt.mCrossover ? GlobalMembers.MS(9f) : GlobalMembers.MS(6f)), theColor, theColor2);
					Color theColor3 = new Color(Color.White);
					Color theColor4 = new Color(Color.White);
					if (!electroBolt.mCrossover && !electroBolt.mHitOtherGem)
					{
						theColor3.mAlpha = (int)Math.Max(2f, 255f * (GlobalMembers.M(0.85f) - num5 * GlobalMembers.M(0.04f)));
						theColor4.mAlpha = (int)Math.Max(2f, 255f * (GlobalMembers.M(0.85f) - num14 * GlobalMembers.M(0.04f)));
					}
					if (electroBolt.mCrossover)
					{
						theColor3.mAlpha = (int)((double)theColor3.mAlpha * GlobalMembers.M(0.5));
						theColor4.mAlpha = (int)((double)theColor4.mAlpha * GlobalMembers.M(0.5));
					}
					theColor3.mAlpha = (int)((float)theColor3.mAlpha * num);
					theColor4.mAlpha = (int)((float)theColor4.mAlpha * num);
					DrawElectroLine(g, GlobalMembersResourcesWP.IMAGE_ELECTROTEX_CENTER, (float)GlobalMembers.S(num2 + 50) + GlobalMembers.S(num6), (float)GlobalMembers.S(num3 + 50) + GlobalMembers.S(num7), (float)GlobalMembers.S(num2 + 50) + GlobalMembers.S(num15), (float)GlobalMembers.S(num3 + 50) + GlobalMembers.S(num16), electroBolt.mHitOtherGem ? GlobalMembers.MS(8f) : (electroBolt.mCrossover ? GlobalMembers.MS(8f) : GlobalMembers.MS(6f)), theColor3, theColor4);
					num6 = num15;
					num7 = num16;
					angleRadius = angleRadius3;
					num5 = num14;
				}
			}
			if (mTimeBonus > 0)
			{
				g.SetDrawMode(Graphics.DrawMode.Normal);
				g.SetFont(GlobalMembersResources.FONT_HEADER);
				((ImageFont)g.mFont).PushLayerColor("GLOW", new Color(GlobalMembers.M(255), GlobalMembers.M(255), GlobalMembers.M(255), (int)((double)((float)GlobalMembers.M(255) * 0.5f) * (1.0 + (double)(float)Math.Cos((float)mFXManager.mBoard.mUpdateCnt * GlobalMembers.M(0.15f))))));
				g.SetColorizeImages(true);
				g.SetColor(Color.White);
				Image imageById = GlobalMembersResourcesWP.GetImageById(888 + mGemColor);
				GlobalMembers.gGR.DrawImageCel(g, imageById, GlobalMembers.S(num2) - ConstantsWP.SPEEDBOARD_GEMNUMS_OFFSET, GlobalMembers.S(num3) - ConstantsWP.SPEEDBOARD_GEMNUMS_OFFSET, mTimeBonus / 5 - 1);
				if (piece != null && mFXManager.mBoard.GetTicksLeft() <= GlobalMembers.M(500) && mFXManager.mBoard.mGameTicks / GlobalMembers.M(18) % 2 == 0)
				{
					g.SetColor(new Color(GlobalMembers.M(255), GlobalMembers.M(200), GlobalMembers.M(200), GlobalMembers.M(255)));
					GlobalMembers.gGR.DrawImageCel(g, GlobalMembersResourcesWP.IMAGE_LIGHTNING_GEMNUMS_WHITE, GlobalMembers.S(num2) - ConstantsWP.SPEEDBOARD_GEMNUMS_OFFSET, GlobalMembers.S(num3) - ConstantsWP.SPEEDBOARD_GEMNUMS_OFFSET, mTimeBonus / 5 - 1);
				}
				((ImageFont)g.mFont).PopLayerColor("GLOW");
			}
			if (piece != null && (double)piece.mScale != 1.0)
			{
				Utils.PopScale(g);
			}
			g.PopColorMult();
		}
	}
}
