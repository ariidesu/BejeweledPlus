using Microsoft.Xna.Framework.Graphics;

namespace SexyFramework.Drivers.Graphics
{
	public class XNATextureDataPiece
	{
		public Texture2D mTexture;

		public TextureCube mCubeTexture;

		public Texture3D mVolumeTexture;

		public int mTexFormat;

		public int mWidth;

		public int mHeight;

		public XNATextureDataPiece()
		{
			mTexture = null;
			mCubeTexture = null;
			mVolumeTexture = null;
			mTexFormat = 0;
			mWidth = 0;
			mHeight = 0;
		}
	}
}
