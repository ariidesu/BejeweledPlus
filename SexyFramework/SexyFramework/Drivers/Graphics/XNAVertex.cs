using Microsoft.Xna.Framework;
using SexyFramework.Graphics;

namespace SexyFramework.Drivers.Graphics
{
	public class XNAVertex
	{
		public SexyFramework.Graphics.Color mColor;

		public static float GetCoord(SexyVertex2D theVertex, int theCoord)
		{
			switch (theCoord)
			{
			case 0:
				return theVertex.x;
			case 1:
				return theVertex.y;
			case 2:
				return theVertex.z;
			case 3:
				return theVertex.u;
			case 4:
				return theVertex.v;
			default:
				return 0f;
			}
		}

		public static SexyFramework.Graphics.Color UnPackColor(uint color)
		{
			return new SexyFramework.Graphics.Color(((int)color >> 16) & 0xFF, ((int)color >> 8) & 0xFF, (int)(color & 0xFF), ((int)color >> 24) & 0xFF);
		}

		public Microsoft.Xna.Framework.Color GetXNAColor()
		{
			return new Microsoft.Xna.Framework.Color(mColor.mRed, mColor.mGreen, mColor.mBlue, mColor.mAlpha);
		}

		public void SetPosition(float theX, float theY, float theZ)
		{
		}

		public static uint TexCoordOffset()
		{
			return 24u;
		}
	}
}
