namespace SexyFramework.Graphics
{
	public class SexyVertex3D : SexyVertex
	{
		public const uint FVF = 322u;

		public float x;

		public float y;

		public float z;

		public uint color;

		public float u;

		public float v;

		public SexyVertex3D()
		{
		}

		public SexyVertex3D(float theX, float theY, float theZ)
		{
			x = theX;
			y = theY;
			z = theZ;
			color = 0u;
		}

		public SexyVertex3D(float theX, float theY, float theZ, float theU, float theV)
		{
			x = theX;
			y = theY;
			z = theZ;
			u = theU;
			v = theV;
			color = 0u;
		}

		public SexyVertex3D(float theX, float theY, float theZ, float theU, float theV, uint theColor)
		{
			x = theX;
			y = theY;
			z = theZ;
			u = theU;
			v = theV;
			color = theColor;
		}
	}
}
