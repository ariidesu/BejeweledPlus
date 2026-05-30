using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Widget
{
	public class GemInfo
	{
		public SexyVector3 mLightingCamOffset = new SexyVector3(0f, 0f, 0f);

		public SexyCoords3 mCoords = new SexyCoords3();

		public SexyVector3 mPos = new SexyVector3(0f, 0f, 0f);

		public SexyVector3 mPosScreen = new SexyVector3(0f, 0f, 0f);

		public float mScaleScreen;

		public Piece mPiece;

		public float mDistToCamera;

		public int mBoardHitFrame;

		public int mColorIndexStart;

		public int mColorIndexEnd;

		public bool mDraw3D;

		public void SetCoords(SexyCoords3 inCoords)
		{
			mCoords = inCoords;
		}

		public void Init(Piece piece, HyperAnimSequence animSeq)
		{
			mPiece = piece;
			mColorIndexStart = piece.mColor;
			mColorIndexEnd = -1;
			mDraw3D = false;
			mBoardHitFrame = animSeq.GetGemHitFrame(piece.mRow, piece.mCol);

			SexyVector3 rot = animSeq.GetGemRot(piece.mRow, piece.mCol);
			SexyVector3 pos = animSeq.GetGemPos(piece.mRow, piece.mCol);
			SexyVector3 scale = animSeq.GetGemScale(piece.mRow, piece.mCol);
			mPos = pos;

			SexyCoords3 c = new SexyCoords3();
			c.RotateRadX(rot.x);
			c.RotateRadY(rot.y);
			c.RotateRadZ(rot.z);
			c.Scale(scale.x, scale.y, scale.z);
			c.Translate(pos.x, pos.y, pos.z);

			mLightingCamOffset = new SexyVector3(-c.t.x, -c.t.y, 0f);
			SetCoords(c);
		}
	}
}
