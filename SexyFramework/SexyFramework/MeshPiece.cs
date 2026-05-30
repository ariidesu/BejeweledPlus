using System;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Graphics;

namespace SexyFramework
{
	public class MeshPiece : IDisposable
	{
		public string mObjectName;

		public string mSetName;

		public SharedImageRef mTexture = new SharedImageRef();

		public SharedImageRef mBumpTexture = new SharedImageRef();

		public int mSexyVF;
		public int mVertexSize;
		public int mVertexBufferCount;
		public int mIndexBufferCount;
		public byte[] mVertexData;
		public byte[] mIndexData;

		public VertexBuffer mXYZBuffer;
		public VertexBuffer mNormalBuffer;
		public VertexBuffer mColorBuffer;
		public VertexBuffer mTexCoords0Buffer;
		public VertexBuffer mTexCoords1Buffer;
		public IndexBuffer mIndexBuffer;

		public virtual void Dispose()
		{
			mXYZBuffer?.Dispose(); mXYZBuffer = null;
			mNormalBuffer?.Dispose(); mNormalBuffer = null;
			mColorBuffer?.Dispose(); mColorBuffer = null;
			mTexCoords0Buffer?.Dispose(); mTexCoords0Buffer = null;
			mTexCoords1Buffer?.Dispose(); mTexCoords1Buffer = null;
			mIndexBuffer?.Dispose(); mIndexBuffer = null;
		}
	}
}
