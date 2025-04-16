using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class SexyVertex3DLit : SexyVertex
	{
		public float x;

		public float y;

		public float z;

		public float nx;

		public float ny;

		public float nz;

		public uint color;

		public float u;

		public float v;

		public static readonly int FVF = 338;

		public void MakeDefaultNormal()
		{
			SexyVector3 sexyVector = new SexyVector3(x, y, z).Normalize();
			nx = sexyVector.x;
			ny = sexyVector.y;
			nz = sexyVector.z;
		}

		public SexyVertex3DLit()
		{
		}

		public SexyVertex3DLit(SexyVector3 thePos, SexyVector3 theNormal)
		{
			x = thePos.x;
			y = thePos.y;
			z = thePos.z;
			nx = theNormal.x;
			ny = theNormal.y;
			nz = theNormal.z;
			color = 0u;
		}

		public SexyVertex3DLit(SexyVector3 thePos, SexyVector3 theNormal, float theU, float theV)
		{
			x = thePos.x;
			y = thePos.y;
			z = thePos.z;
			nx = theNormal.x;
			ny = theNormal.y;
			nz = theNormal.z;
			u = theU;
			v = theV;
			color = 0u;
		}

		public SexyVertex3DLit(SexyVector3 thePos, SexyVector3 theNormal, float theU, float theV, uint theColor)
		{
			x = thePos.x;
			y = thePos.y;
			z = thePos.z;
			nx = theNormal.x;
			ny = theNormal.y;
			nz = theNormal.z;
			u = theU;
			v = theV;
			color = theColor;
		}

		public SexyVertex3DLit(SexyVector3 thePos)
		{
			x = thePos.x;
			y = thePos.y;
			z = thePos.z;
			MakeDefaultNormal();
			color = 0u;
		}

		public SexyVertex3DLit(SexyVector3 thePos, float theU, float theV)
		{
			x = thePos.x;
			y = thePos.y;
			z = thePos.z;
			u = theU;
			v = theV;
			MakeDefaultNormal();
			color = 0u;
		}

		public SexyVertex3DLit(SexyVector3 thePos, float theU, float theV, uint theColor)
		{
			x = thePos.x;
			y = thePos.y;
			z = thePos.z;
			u = theU;
			v = theV;
			color = theColor;
			MakeDefaultNormal();
		}
	}
}
