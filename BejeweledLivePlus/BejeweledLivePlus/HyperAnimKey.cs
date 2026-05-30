using SexyFramework.Misc;

namespace BejeweledLivePlus
{
	public class HyperAnimKey
	{
		public SexyVector3 mPos;

		public SexyVector3 mRot;

		public SexyVector3 mScale;

		public HyperAnimKey()
		{
			mPos = new SexyVector3(0f, 0f, 0f);
			mRot = new SexyVector3(0f, 0f, 0f);
			mScale = new SexyVector3(1f, 1f, 1f);
		}
	}
}
