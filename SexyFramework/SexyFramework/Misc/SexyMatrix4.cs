using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public class SexyMatrix4
	{
		public Matrix mMatrix;

		public float[,] m = new float[4, 4];

		public float m00
		{
			get
			{
				return m[0, 0];
			}
			set
			{
				m[0, 0] = value;
			}
		}

		public float m01
		{
			get
			{
				return m[0, 1];
			}
			set
			{
				m[0, 1] = value;
			}
		}

		public float m02
		{
			get
			{
				return m[0, 2];
			}
			set
			{
				m[0, 2] = value;
			}
		}

		public float m03
		{
			get
			{
				return m[0, 3];
			}
			set
			{
				m[0, 3] = value;
			}
		}

		public float m10
		{
			get
			{
				return m[1, 0];
			}
			set
			{
				m[1, 0] = value;
			}
		}

		public float m11
		{
			get
			{
				return m[1, 1];
			}
			set
			{
				m[1, 1] = value;
			}
		}

		public float m12
		{
			get
			{
				return m[1, 2];
			}
			set
			{
				m[1, 2] = value;
			}
		}

		public float m13
		{
			get
			{
				return m[1, 3];
			}
			set
			{
				m[1, 3] = value;
			}
		}

		public float m20
		{
			get
			{
				return m[2, 0];
			}
			set
			{
				m[2, 0] = value;
			}
		}

		public float m21
		{
			get
			{
				return m[2, 1];
			}
			set
			{
				m[2, 1] = value;
			}
		}

		public float m22
		{
			get
			{
				return m[2, 2];
			}
			set
			{
				m[2, 2] = value;
			}
		}

		public float m23
		{
			get
			{
				return m[2, 3];
			}
			set
			{
				m[2, 3] = value;
			}
		}

		public float m30
		{
			get
			{
				return m[3, 0];
			}
			set
			{
				m[3, 0] = value;
			}
		}

		public float m31
		{
			get
			{
				return m[3, 1];
			}
			set
			{
				m[3, 1] = value;
			}
		}

		public float m32
		{
			get
			{
				return m[3, 2];
			}
			set
			{
				m[3, 2] = value;
			}
		}

		public float m33
		{
			get
			{
				return m[3, 3];
			}
			set
			{
				m[3, 3] = value;
			}
		}

		public SexyMatrix4()
		{
		}

		public void Swap(SexyMatrix4 lhs)
		{
			float[,] array = lhs.m;
			lhs.m = m;
			m = array;
		}

		public void CopyTo(SexyMatrix4 lhs)
		{
			for (int i = 0; i < 4; i++)
			{
				lhs.m[i, 0] = m[i, 0];
				lhs.m[i, 1] = m[i, 1];
				lhs.m[i, 2] = m[i, 2];
				lhs.m[i, 3] = m[i, 3];
			}
		}

		public SexyMatrix4(SexyMatrix4 rhs)
		{
			rhs.CopyTo(this);
		}

		public SexyMatrix4(float in00, float in01, float in02, float in03, float in10, float in11, float in12, float in13, float in20, float in21, float in22, float in23, float in30, float in31, float in32, float in33)
		{
			m00 = in00;
			m01 = in00;
			m02 = in02;
			m03 = in03;
			m10 = in10;
			m11 = in11;
			m12 = in12;
			m13 = in13;
			m20 = in20;
			m21 = in21;
			m22 = in22;
			m23 = in23;
			m30 = in30;
			m31 = in31;
			m32 = in32;
			m32 = in33;
		}

		public void LoadIdentity()
		{
			m[0, 1] = (m[0, 2] = (m[0, 3] = (m[1, 0] = (m[1, 2] = (m[1, 3] = (m[2, 0] = (m[2, 1] = (m[2, 3] = (m[3, 0] = (m[3, 1] = (m[3, 2] = 0f)))))))))));
			m[0, 0] = (m[1, 1] = (m[2, 2] = (m[3, 3] = 1f)));
		}

		public static SexyVector3 operator *(SexyMatrix4 ImpliedObject, SexyVector2 theVec)
		{
			SexyVector3 result = default(SexyVector3);
			Vector3 position = new Vector3(theVec.mVector, 0f);
			result.mVector = Vector3.Transform(position, ImpliedObject.mMatrix);
			return result;
		}

		public static SexyVector3 operator *(SexyMatrix4 ImpliedObject, SexyVector3 theVec)
		{
			SexyVector3 result = default(SexyVector3);
			result.mVector = Vector3.Transform(theVec.mVector, ImpliedObject.mMatrix);
			return result;
		}

		public static SexyMatrix4 operator *(SexyMatrix4 ImpliedObject, SexyMatrix4 theMat)
		{
			SexyMatrix4 sexyMatrix = new SexyMatrix4();
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					float num = 0f;
					for (int k = 0; k < 4; k++)
					{
						num += ImpliedObject.m[i, k] * theMat.m[k, j];
					}
					sexyMatrix.m[i, j] = num;
				}
			}
			return sexyMatrix;
		}
	}
}
