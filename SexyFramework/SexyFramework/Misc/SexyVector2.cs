using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public struct SexyVector2
	{
		public Vector2 mVector;

		public static SexyVector2 Zero = new SexyVector2(0f, 0f);

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

		public SexyVector2(bool init)
		{
			mVector = new Vector2(0f, 0f);
		}

		public SexyVector2(float theX, float theY)
		{
			mVector = new Vector2(theX, theY);
		}

		public SexyVector2(Vector2 v)
		{
			mVector = v;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(SexyVector2 obj)
		{
			return mVector == obj.mVector;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public float Dot(SexyVector2 v)
		{
			return Vector2.Dot(mVector, v.mVector);
		}

		public static SexyVector2 operator +(SexyVector2 ImpliedObject, SexyVector2 v)
		{
			return new SexyVector2(ImpliedObject.mVector + v.mVector);
		}

		public static SexyVector2 operator -(SexyVector2 ImpliedObject, SexyVector2 v)
		{
			return new SexyVector2(ImpliedObject.mVector - v.mVector);
		}

		public static SexyVector2 operator -(SexyVector2 ImpliedObject)
		{
			return new SexyVector2(-ImpliedObject.mVector);
		}

		public static SexyVector2 operator *(SexyVector2 ImpliedObject, float t)
		{
			return new SexyVector2(ImpliedObject.mVector * t);
		}

		public static SexyVector2 operator /(SexyVector2 ImpliedObject, float t)
		{
			return new SexyVector2(ImpliedObject.x / t, ImpliedObject.y / t);
		}

		public SexyVector2 AddSelf(SexyVector2 v)
		{
			x += v.x;
			y += v.y;
			return this;
		}

		public SexyVector2 SubSelf(SexyVector2 v)
		{
			x -= v.x;
			y -= v.y;
			return this;
		}

		public SexyVector2 MulSelf(SexyVector2 v)
		{
			x *= v.x;
			y *= v.y;
			return this;
		}

		public SexyVector2 DivSelf(SexyVector2 v)
		{
			x /= v.x;
			y /= v.y;
			return this;
		}

		public static bool operator ==(SexyVector2 ImpliedObject, SexyVector2 v)
		{
			return ImpliedObject.Equals(v);
		}

		public static bool operator !=(SexyVector2 ImpliedObject, SexyVector2 v)
		{
			return !(ImpliedObject.mVector == v.mVector);
		}

		public float Magnitude()
		{
			return mVector.Length();
		}

		public float MagnitudeSquared()
		{
			return mVector.LengthSquared();
		}

		public SexyVector2 Normalize()
		{
			mVector.Normalize();
			return this;
		}

		public SexyVector2 Perp()
		{
			return new SexyVector2(0f - y, x);
		}

		public static implicit operator Vector2(SexyVector2 ImpliedObject)
		{
			return new Vector2(ImpliedObject.x, ImpliedObject.y);
		}
	}
}
