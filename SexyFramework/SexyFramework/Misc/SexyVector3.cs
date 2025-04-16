using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public struct SexyVector3
	{
		public Vector3 mVector;

		public float x
		{
			get
			{
				return mVector.X;
			}
			set
			{
				mVector.X = value;
			}
		}

		public float y
		{
			get
			{
				return mVector.Y;
			}
			set
			{
				mVector.Y = value;
			}
		}

		public float z
		{
			get
			{
				return mVector.Z;
			}
			set
			{
				mVector.Z = value;
			}
		}

		public float[] ToFloatArray()
		{
			return new float[3] { x, y, z };
		}

		public SexyVector3(float theX, float theY, float theZ)
		{
			mVector = new Vector3(theX, theY, theZ);
		}

		public SexyVector3(SexyVector3 rhs)
		{
			mVector = rhs.mVector;
		}

		public SexyVector3(Vector3 rhs)
		{
			mVector = rhs;
		}

		public float Dot(SexyVector3 v)
		{
			return Vector3.Dot(mVector, v.mVector);
		}

		public SexyVector3 Cross(SexyVector3 v)
		{
			return new SexyVector3(Vector3.Cross(mVector, v.mVector));
		}

		public SexyVector3 CopyFrom(SexyVector3 v)
		{
			mVector = v.mVector;
			return this;
		}

		public static SexyVector3 operator -(SexyVector3 ImpliedObject)
		{
			return new SexyVector3(0f - ImpliedObject.x, 0f - ImpliedObject.y, 0f - ImpliedObject.z);
		}

		public static SexyVector3 operator +(SexyVector3 ImpliedObject, SexyVector3 v)
		{
			return new SexyVector3(ImpliedObject.x + v.x, ImpliedObject.y + v.y, ImpliedObject.z + v.z);
		}

		public static SexyVector3 operator -(SexyVector3 ImpliedObject, SexyVector3 v)
		{
			return new SexyVector3(ImpliedObject.x - v.x, ImpliedObject.y - v.y, ImpliedObject.z - v.z);
		}

		public static SexyVector3 operator *(SexyVector3 ImpliedObject, float t)
		{
			return new SexyVector3(t * ImpliedObject.x, t * ImpliedObject.y, t * ImpliedObject.z);
		}

		public static SexyVector3 operator *(SexyVector3 ImpliedObject, SexyVector3 v)
		{
			return new SexyVector3(Vector3.Multiply(ImpliedObject.mVector, v.mVector));
		}

		public static SexyVector3 operator /(SexyVector3 ImpliedObject, float t)
		{
			return new SexyVector3(Vector3.Divide(ImpliedObject.mVector, t));
		}

		public static SexyVector3 operator /(SexyVector3 ImpliedObject, SexyVector3 v)
		{
			return new SexyVector3(Vector3.Divide(ImpliedObject.mVector, v.mVector));
		}

		public float Magnitude()
		{
			return mVector.Length();
		}

		public SexyVector3 Normalize()
		{
			mVector.Normalize();
			return this;
		}

		public bool ApproxEquals(SexyVector3 inV)
		{
			return ApproxEquals(inV, 0.001f);
		}

		public bool ApproxEquals(SexyVector3 inV, float inTol)
		{
			if (SexyMath.ApproxEquals(x, inV.x, inTol) && SexyMath.ApproxEquals(y, inV.y, inTol))
			{
				return SexyMath.ApproxEquals(z, inV.z, inTol);
			}
			return false;
		}

		public bool ApproxZero()
		{
			return ApproxZero(0.001f);
		}

		public bool ApproxZero(float inTol)
		{
			return ApproxEquals(new SexyVector3(0f, 0f, 0f), inTol);
		}

		public SexyVector3 Enter(SexyAxes3 inAxes)
		{
			return new SexyVector3(Dot(inAxes.vX), Dot(inAxes.vY), Dot(inAxes.vZ));
		}

		public SexyVector3 Enter(SexyCoords3 inCoords)
		{
			return (this - inCoords.t.Enter(inCoords.r)) / inCoords.s;
		}

		public SexyVector3 Leave(SexyAxes3 inAxes)
		{
			return new SexyVector3(x * inAxes.vX.x + y * inAxes.vY.x + z * inAxes.vZ.x, x * inAxes.vX.y + y * inAxes.vY.y + z * inAxes.vZ.y, x * inAxes.vX.z + y * inAxes.vY.z + z * inAxes.vZ.z);
		}

		public SexyVector3 Leave(SexyCoords3 inCoords)
		{
			return this * inCoords.s.Leave(inCoords.r) + inCoords.t;
		}
	}
}
