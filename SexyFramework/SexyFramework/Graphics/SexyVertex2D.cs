namespace SexyFramework.Graphics
{
	public struct SexyVertex2D : SexyVertex
	{
		public const uint FVF = 452u;

		public float x;

		public float y;

		public float z;

		public float rhw;

		public Color color;

		public uint specular;

		public float u;

		public float v;

		public float u2;

		public float v2;

		public SexyVertex2D(float theX, float theY)
		{
			x = theX;
			y = theY;
			z = 0f;
			u = 0f;
			v = 0f;
			rhw = 1f;
			color = Color.Zero;
			specular = 0u;
			u2 = (v2 = 0f);
		}

		public SexyVertex2D(float theX, float theY, float theU, float theV)
		{
			x = theX;
			y = theY;
			u = theU;
			v = theV;
			z = 0f;
			rhw = 1f;
			color = Color.Zero;
			specular = 0u;
			u2 = (v2 = 0f);
		}

		public SexyVertex2D(float theX, float theY, float theU, float theV, uint theColor)
		{
			x = theX;
			y = theY;
			u = theU;
			v = theV;
			color = new Color((int)theColor);
			z = 0f;
			rhw = 1f;
			specular = 0u;
			u2 = (v2 = 0f);
		}

		public SexyVertex2D(float theX, float theY, float theZ, float theU, float theV, uint theColor)
		{
			x = theX;
			y = theY;
			z = theZ;
			u = theU;
			v = theV;
			color = new Color((int)theColor);
			z = 0f;
			rhw = 1f;
			specular = 0u;
			u2 = (v2 = 0f);
		}

		public SexyVertex2D(float px, float py, float pu, float pv, float pu2, float pv2)
			: this(px, py, pu, pv)
		{
			u2 = pu2;
			v2 = pv2;
		}

		public SexyVertex2D(float px, float py, uint theColor, float pu, float pv, float pu2, float pv2)
			: this(px, py, pu, pv, theColor)
		{
			u2 = pu2;
			v2 = pv2;
		}
	}
}
