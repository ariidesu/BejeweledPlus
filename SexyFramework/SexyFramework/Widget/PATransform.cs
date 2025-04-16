using SexyFramework.Misc;

namespace SexyFramework.Widget
{
	public struct PATransform
	{
		public SexyTransform2D mMatrix;

		public PATransform Clone()
		{
			PATransform result = new PATransform(true);
			mMatrix.CopyTo(result.mMatrix);
			return result;
		}

		public PATransform(bool init)
		{
			mMatrix = new SexyTransform2D(true);
			mMatrix.LoadIdentity();
		}

		public void CopyFrom(PATransform rhs)
		{
			mMatrix.CopyFrom(rhs.mMatrix);
		}

		public void TransformSrc(PATransform theSrcTransform, ref PATransform outTran)
		{
			outTran.mMatrix.CopyFrom(mMatrix * theSrcTransform.mMatrix);
		}

		public void InterpolateTo(PATransform theNextTransform, float thePct, ref PATransform outTran)
		{
			outTran.mMatrix.mMatrix = mMatrix.mMatrix * (1f - thePct) + theNextTransform.mMatrix.mMatrix * thePct;
		}
	}
}
