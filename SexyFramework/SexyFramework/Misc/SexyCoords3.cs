namespace SexyFramework.Misc
{
	public class SexyCoords3
	{
		public SexyVector3 t = default(SexyVector3);

		public SexyAxes3 r = new SexyAxes3();

		public SexyVector3 s = default(SexyVector3);

		public SexyCoords3()
		{
			t = new SexyVector3(0f, 0f, 0f);
			r = new SexyAxes3();
			s = new SexyVector3(1f, 1f, 1f);
		}

		public SexyCoords3(SexyCoords3 inC)
		{
			t = inC.t;
			r = new SexyAxes3(inC.r);
			s = inC.s;
		}

		public SexyCoords3(SexyAxes3 inR)
		{
			t = new SexyVector3(0f, 0f, 0f);
			r = new SexyAxes3(inR);
			s = new SexyVector3(1f, 1f, 1f);
		}

		public SexyCoords3(SexyVector3 inT, SexyAxes3 inR, SexyVector3 inS)
		{
			t = inT;
			r = new SexyAxes3(inR);
			s = inS;
		}

		public SexyCoords3 CopyFrom(SexyCoords3 inC)
		{
			t = inC.t;
			r = inC.r;
			s = inC.s;
			return this;
		}

		public SexyCoords3 Enter(SexyCoords3 inCoords)
		{
			return new SexyCoords3(t.Enter(inCoords), r.Enter(inCoords.r), s / inCoords.s);
		}

		public SexyCoords3 Leave(SexyCoords3 inCoords)
		{
			return new SexyCoords3(t.Leave(inCoords), r.Leave(inCoords.r), s * inCoords.s);
		}

		public SexyCoords3 Inverse()
		{
			return new SexyCoords3().Enter(this);
		}

		public SexyCoords3 DeltaTo(SexyCoords3 inCoords)
		{
			return inCoords.Inverse().Leave(this);
		}

		public void Translate(float inX, float inY, float inZ)
		{
			t += new SexyVector3(inX, inY, inZ);
		}

		public void RotateRadAxis(float inRot, SexyVector3 inNormalizedAxis)
		{
			r.RotateRadAxis(inRot, inNormalizedAxis);
		}

		public void RotateRadX(float inRot)
		{
			r.RotateRadX(inRot);
		}

		public void RotateRadY(float inRot)
		{
			r.RotateRadY(inRot);
		}

		public void RotateRadZ(float inRot)
		{
			r.RotateRadZ(inRot);
		}

		public void Scale(float inX, float inY, float inZ)
		{
			s *= new SexyVector3(inX, inY, inZ);
		}

		public bool LookAt(SexyVector3 inTargetPos, SexyVector3 inUpVector)
		{
			SexyVector3 sexyVector = t - inTargetPos;
			if (sexyVector.ApproxZero())
			{
				return false;
			}
			sexyVector = sexyVector.Normalize();
			if (SexyMath.Fabs(inUpVector.Dot(sexyVector)) > 1f - GlobalMembers.SEXYMATH_EPSILON)
			{
				return false;
			}
			r.vZ = sexyVector;
			r.vX = inUpVector.Cross(r.vZ).Normalize();
			r.vY = r.vZ.Cross(r.vX).Normalize();
			return true;
		}

		public bool LookAt(SexyVector3 inViewPos, SexyVector3 inTargetPos, SexyVector3 inUpVector)
		{
			t = inViewPos;
			return LookAt(inTargetPos, inUpVector);
		}

		public void GetInboundMatrix(SexyMatrix4 outM)
		{
			if (outM != null)
			{
				SexyVector3 sexyVector = new SexyVector3(-t);
				SexyVector3 v = new SexyVector3(r.vX / s.x);
				SexyVector3 v2 = new SexyVector3(r.vY / s.y);
				SexyVector3 v3 = new SexyVector3(r.vZ / s.z);
				outM.m[0, 0] = v.x;
				outM.m[0, 1] = v2.x;
				outM.m[0, 2] = v3.x;
				outM.m[0, 3] = 0f;
				outM.m[1, 0] = v.y;
				outM.m[1, 1] = v2.y;
				outM.m[1, 2] = v3.y;
				outM.m[1, 3] = 0f;
				outM.m[2, 0] = v.z;
				outM.m[2, 1] = v2.z;
				outM.m[2, 2] = v3.z;
				outM.m[2, 3] = 0f;
				outM.m[3, 0] = sexyVector.Dot(v);
				outM.m[3, 1] = sexyVector.Dot(v2);
				outM.m[3, 2] = sexyVector.Dot(v3);
				outM.m[3, 3] = 1f;
			}
		}

		public void GetOutboundMatrix(SexyMatrix4 outM)
		{
			if (outM != null)
			{
				outM.m[0, 0] = r.vX.x * s.x;
				outM.m[0, 1] = r.vX.y * s.x;
				outM.m[0, 2] = r.vX.z * s.x;
				outM.m[0, 3] = 0f;
				outM.m[1, 0] = r.vY.x * s.y;
				outM.m[1, 1] = r.vY.y * s.y;
				outM.m[1, 2] = r.vY.z * s.y;
				outM.m[1, 3] = 0f;
				outM.m[2, 0] = r.vZ.x * s.z;
				outM.m[2, 1] = r.vZ.y * s.z;
				outM.m[2, 2] = r.vZ.z * s.z;
				outM.m[2, 3] = 0f;
				outM.m[3, 0] = t.x;
				outM.m[3, 1] = t.y;
				outM.m[3, 2] = t.z;
				outM.m[3, 3] = 1f;
			}
		}
	}
}
