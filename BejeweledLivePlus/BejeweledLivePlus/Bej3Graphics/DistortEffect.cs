using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class DistortEffect
	{
		public int mType;

		public FPoint mCenter = default(FPoint);

		public FPoint mMoveDelta = default(FPoint);

		public CurvedVal mTexturePos = new CurvedVal();

		public CurvedVal mMovePct = new CurvedVal();

		public CurvedVal mIntensity = new CurvedVal();

		public CurvedVal mRadius = new CurvedVal();

		public DistortEffect()
		{
			mType = 0;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_TEXTURE_POS, mTexturePos);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_RADIUS, mRadius);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_INTENSITY, mIntensity);
			mMovePct.SetConstant(0.0);
		}
	}
}
