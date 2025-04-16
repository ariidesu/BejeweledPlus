using Microsoft.Xna.Framework;
using SexyFramework.Graphics;

namespace SexyFramework.Widget
{
	public class CursorWidget : Widget
	{
		public Image mImage;

		public CursorWidget()
		{
			mImage = null;
			mMouseVisible = false;
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			if (mImage != null)
			{
				g.DrawImage(mImage, 0, 0);
			}
		}

		public void SetImage(Image theImage)
		{
			mImage = theImage;
			if (mImage != null)
			{
				Resize(mX, mY, theImage.mWidth, theImage.mHeight);
			}
		}

		public Vector2 GetHotspot()
		{
			if (mImage == null)
			{
				return new Vector2(0f, 0f);
			}
			return new Vector2(mImage.GetWidth() / 2, mImage.GetHeight() / 2);
		}
	}
}
