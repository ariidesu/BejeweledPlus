using System;

namespace SexyFramework.Misc
{
	public class SexyAxes3
	{
		public SexyVector3 vX = default(SexyVector3);

		public SexyVector3 vY = default(SexyVector3);

		public SexyVector3 vZ = default(SexyVector3);

		public SexyAxes3()
		{
			vX = new SexyVector3(1f, 0f, 0f);
			vY = new SexyVector3(0f, 1f, 0f);
			vZ = new SexyVector3(0f, 0f, 1f);
		}

		public SexyAxes3(SexyAxes3 inA)
		{
			vX = inA.vX;
			vY = inA.vY;
			vZ = inA.vZ;
		}

		public SexyAxes3(SexyVector3 inX, SexyVector3 inY, SexyVector3 inZ)
		{
			vX = inX;
			vY = inY;
			vZ = inZ;
		}

		public void CopyFrom(SexyAxes3 inA)
		{
			vX = inA.vX;
			vY = inA.vY;
			vZ = inA.vZ;
		}

		public SexyAxes3 Enter(SexyAxes3 inAxes)
		{
			return new SexyAxes3(vX.Enter(inAxes), vY.Enter(inAxes), vZ.Enter(inAxes));
		}

		public SexyAxes3 Leave(SexyAxes3 inAxes)
		{
			return new SexyAxes3(vX.Leave(inAxes), vY.Leave(inAxes), vZ.Leave(inAxes));
		}

		private void EndterSelf(SexyAxes3 inAxes)
		{
			vX = vX.Enter(inAxes);
			vY = vX.Enter(inAxes);
			vZ = vZ.Enter(inAxes);
		}

		private void LeaveSelf(SexyAxes3 inAxes)
		{
			vX = vX.Leave(inAxes);
			vY = vX.Leave(inAxes);
			vZ = vZ.Leave(inAxes);
		}

		public SexyAxes3 Inverse()
		{
			return new SexyAxes3().Enter(this);
		}

		public SexyAxes3 OrthoNormalize()
		{
			SexyAxes3 sexyAxes = new SexyAxes3(this);
			sexyAxes.vX = sexyAxes.vY.Cross(sexyAxes.vZ).Normalize();
			sexyAxes.vY = sexyAxes.vZ.Cross(sexyAxes.vX).Normalize();
			sexyAxes.vZ = sexyAxes.vX.Cross(sexyAxes.vY).Normalize();
			return sexyAxes;
		}

		public SexyAxes3 DeltaTo(SexyAxes3 inAxes)
		{
			return inAxes.Inverse().Leave(this);
		}

		public SexyAxes3 SlerpTo(SexyAxes3 inAxes, float inAlpha)
		{
			return SlerpTo(inAxes, inAlpha, false);
		}

		public SexyAxes3 SlerpTo(SexyAxes3 inAxes, float inAlpha, bool inFastButLessAccurate)
		{
			return SexyQuat3.Slerp(new SexyQuat3(this), new SexyQuat3(inAxes), inAlpha, inFastButLessAccurate);
		}

		public void RotateRadAxis(float inRot, SexyVector3 inNormalizedAxis)
		{
			SexyAxes3 inAxes = SexyQuat3.AxisAngle(inNormalizedAxis, inRot);
			LeaveSelf(inAxes);
		}

		public void RotateRadX(float inRot)
		{
			double num = Math.Sin(inRot);
			double num2 = Math.Cos(inRot);
			SexyAxes3 sexyAxes = new SexyAxes3();
			sexyAxes.vY.y = (float)num2;
			sexyAxes.vZ.y = (float)(0.0 - num);
			sexyAxes.vY.z = (float)num;
			sexyAxes.vZ.z = (float)num2;
			LeaveSelf(sexyAxes);
		}

		public void RotateRadY(float inRot)
		{
			double num = Math.Sin(inRot);
			double num2 = Math.Cos(inRot);
			SexyAxes3 sexyAxes = new SexyAxes3();
			sexyAxes.vX.x = (float)num2;
			sexyAxes.vX.z = (float)(0.0 - num);
			sexyAxes.vZ.x = (float)num;
			sexyAxes.vZ.z = (float)num2;
			LeaveSelf(sexyAxes);
		}

		public void RotateRadZ(float inRot)
		{
			double num = Math.Sin(inRot);
			double num2 = Math.Cos(inRot);
			SexyAxes3 sexyAxes = new SexyAxes3();
			sexyAxes.vX.x = (float)num2;
			sexyAxes.vX.y = (float)num;
			sexyAxes.vY.x = (float)(0.0 - num);
			sexyAxes.vY.y = (float)num2;
			LeaveSelf(sexyAxes);
		}

		public void LookAt(SexyVector3 inTargetDir, SexyVector3 inUpVector)
		{
			SexyVector3 v = inTargetDir.Normalize();
			if (!(SexyMath.Fabs(inUpVector.Dot(v)) > 1f - GlobalMembers.SEXYMATH_EPSILON))
			{
				SexyAxes3 sexyAxes = new SexyAxes3();
				sexyAxes.vZ = v;
				sexyAxes.vX = inUpVector.Cross(sexyAxes.vZ).Normalize();
				sexyAxes.vY = sexyAxes.vZ.Cross(sexyAxes.vX).Normalize();
				LeaveSelf(sexyAxes);
			}
		}
	}
}
