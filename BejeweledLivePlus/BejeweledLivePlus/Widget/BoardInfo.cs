using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Widget
{
	public class BoardInfo
	{
		public SexyCoords3 mCoords = new SexyCoords3();

		public Board mBoard;

		public void SetCoords(SexyCoords3 inCoords)
		{
			mCoords = inCoords;
		}

		public void Init(Board board, HyperAnimSequence animSeq)
		{
			mBoard = board;
			SexyVector3 rot = animSeq.GetBoardRot();
			SexyVector3 pos = animSeq.GetBoardPos();
			SexyVector3 scale = animSeq.GetBoardScale();
			SexyCoords3 c = new SexyCoords3();
			c.RotateRadX(rot.x);
			c.RotateRadY(rot.y);
			c.RotateRadZ(rot.z);
			c.Scale(scale.x, scale.y, scale.z);
			c.Translate(pos.x, pos.y, pos.z);
			SetCoords(c);
		}
	}
}
