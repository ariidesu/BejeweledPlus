using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Utilities;
using SexyFramework.Drivers.App;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Drivers.Graphics
{
	public class BaseXNARenderDevice : RenderDevice3D
	{
		public enum ClipperType
		{
			Clipper_Less,
			Clipper_Greater,
			Clipper_Equal,
			Clipper_GreaterEqual,
			Clipper_LessEqual
		}

		private List<VertexPositionColorTexture> DPC_l2 = new List<VertexPositionColorTexture>();

		private List<VertexPositionColorTexture> DPC_l1 = new List<VertexPositionColorTexture>();

		public GraphicsDeviceManager mDevice;

		// Sexy2D shader (replaces BasicEffect for 2D batched rendering).
		// Falls back to BasicEffect if Sexy2D fails to load (e.g. content not built).
		protected Effect mSexy2DEffect;
		protected EffectParameter mSexy2DWVP;
		protected EffectParameter mSexy2DTex0;
		protected EffectTechnique mSexy2DTextured;
		protected EffectTechnique mSexy2DUntextured;

		// Fallback. Used when mSexy2DEffect is null.
		protected BasicEffect mBasicEffect;

		public BaseXNAStateManager mStateMgr;

		public BlendState mBlendState;

		public Matrix mProjectionMatrix;

		public Matrix mViewMatrix;

		private float mPixelOffset;

		private int mMinTextureWidth;

		private int mMinTextureHeight;

		private int mMaxTextureWidth;

		private int mMaxTextureHeight;

		private int mMaxTextureAspectRatio;

		private uint mRenderModeFlags;

		public uint mSupportedTextureFormats;

		public bool mTextureSizeMustBePow2;

		public bool mRenderTargetMustBePow2;

		private ulong mDefaultVertexSize;

		public ulong mDefaultVertexFVF;

		private int mWidth;

		private int mHeight;

		private int mScreenWidth;

		private int mScreenHeight;

		private bool mSceneBegun;

		private VertexPositionColorTexture[] mBatchedTriangleBuffer;

		private short[] mBatchedIndexBuffer;

		private int mBatchedTriangleIndex;

		private int mBatchedIndexIndex;

		private static int mBatchedTriangleSize = 1200;

		private IGraphicsDriver mGraphicsDriver;

		private Texture2D mTexture;

		private Game mGame;

		public Image mImage;

		public Transform mTransform = new Transform();

		private HRenderContext mCurrentContex;

		private HRenderContext GlobalRenderContex;

		private Stack<SexyTransform2D> mTransformStack;

		private static bool SUPPORT_HW_CLIP = false;

		public SpriteBatch mSpriteBatch;

		public VertexPositionColorTexture[] mTmpVPCTBuffer;

		public VertexPositionColor[] mTmpVPCBuffer;

		// Scratch buffers reused by per-call paths so we don't allocate on the hot path.
		private VertexPositionColorTexture[] mScratchTri = new VertexPositionColorTexture[3];
		private VertexPositionColorTexture[] mScratchPoly = new VertexPositionColorTexture[64];
		private VertexPositionColor[] mScratchLine = new VertexPositionColor[2];

		private BlendState mNormalState;

		private BlendState mAdditiveState;

		private int mCurDrawMode;

		public RenderTarget2D mScreenTarget;

		public Microsoft.Xna.Framework.Color[] mScreenImageData;

		public Rectangle mRenderRect = new Rectangle(0, 0, 640, 1066);

		public BaseXNARenderDevice(IGraphicsDriver theDriver)
		{
			mDevice = new GraphicsDeviceManager((theDriver as XNAGraphicsDriver).GetMainGame());
			mDevice.IsFullScreen = true;
			mDevice.SynchronizeWithVerticalRetrace = false;
			mGame = (theDriver as XNAGraphicsDriver).GetMainGame();
			mStateMgr = new BaseXNAStateManager(ref mDevice);
			mStateMgr.mRenderDevice = this;
			mDevice.SynchronizeWithVerticalRetrace = false;
			mDevice.ApplyChanges();
			mTransformStack = new Stack<SexyTransform2D>();
			mBatchedTriangleBuffer = new VertexPositionColorTexture[mBatchedTriangleSize];
			mBatchedIndexBuffer = new short[mBatchedTriangleSize * 2];
			mCurrentContex = null;
		}

		public BaseXNARenderDevice(Game game)
		{
			mGame = game;
			mDevice = new GraphicsDeviceManager(game);
			mDevice.SynchronizeWithVerticalRetrace = false;
			mStateMgr = new BaseXNAStateManager(ref mDevice);
			mStateMgr.mRenderDevice = this;
			mDevice.ApplyChanges();
			mTransformStack = new Stack<SexyTransform2D>();
			mBatchedTriangleBuffer = new VertexPositionColorTexture[mBatchedTriangleSize];
			mBatchedIndexBuffer = new short[mBatchedTriangleSize * 2];
			mCurrentContex = null;
			mDevice.IsFullScreen = false;
		}

		public void Init()
		{
			SetViewport(0, 0, mWidth, mHeight, 0f, 1f);
		}

		public override RenderDevice3D Get3D()
		{
			return this;
		}

		public override bool CanFillPoly()
		{
			return true;
		}

		public override HRenderContext CreateContext(Image theDestImage, HRenderContext theSourceContext)
		{
			if (theSourceContext == null)
			{
				RenderTarget2D renderTarget2D = null;
				if (theDestImage != null)
				{
					HRenderContext hRenderContext = new HRenderContext();
					XNATextureData xNATextureData = theDestImage.GetRenderData() as XNATextureData;
					if (xNATextureData != null && xNATextureData.mTextures[0].mTexture != null)
					{
						renderTarget2D = (RenderTarget2D)xNATextureData.mTextures[0].mTexture;
					}
					else
					{
						renderTarget2D = new RenderTarget2D(mDevice.GraphicsDevice, theDestImage.GetWidth(), theDestImage.GetHeight(), false, 0, 0, 0, RenderTargetUsage.PreserveContents);
						XNATextureData xNATextureData2 = new XNATextureData(null);
						theDestImage.SetRenderData(xNATextureData2);
						xNATextureData2.mWidth = renderTarget2D.Width;
						xNATextureData2.mHeight = renderTarget2D.Height;
						xNATextureData2.mTexPieceWidth = renderTarget2D.Width;
						xNATextureData2.mTexPieceHeight = renderTarget2D.Height;
						xNATextureData2.mTexVecWidth = 1;
						xNATextureData2.mTexVecHeight = 1;
						xNATextureData2.mPixelFormat = PixelFormat.PixelFormat_A8R8G8B8;
						xNATextureData2.mMaxTotalU = 1f;
						xNATextureData2.mMaxTotalV = 1f;
						xNATextureData2.mImageFlags = theDestImage.GetImageFlags();
						xNATextureData2.mOptimizedLoad = true;
						xNATextureData2.mTextures[0].mWidth = renderTarget2D.Width;
						xNATextureData2.mTextures[0].mHeight = renderTarget2D.Height;
						xNATextureData2.mTextures[0].mTexture = renderTarget2D;
					}
					hRenderContext.mHandlePtr = renderTarget2D;
					return hRenderContext;
				}
				return null;
			}
			return theSourceContext;
		}

		public override void DeleteContext(HRenderContext theContext)
		{
		}

		public override void SetCurrentContext(HRenderContext theContext)
		{
			if (theContext != mCurrentContex)
			{
				if (mBatchedTriangleIndex > 0)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				if (theContext == null || theContext.GetPointer() == null)
				{
					mDevice.GraphicsDevice.SetRenderTarget(mScreenTarget);
					mStateMgr.SetProjectionTransform(Matrix.CreateOrthographicOffCenter(0f, mScreenTarget.Width, mScreenTarget.Height, 0f, -1000f, 1000f));
					SetViewport(0, 0, mScreenTarget.Width, mScreenTarget.Height, 0f, 1f);
					mCurrentContex = theContext;
				}
				else
				{
					RenderTarget2D renderTarget2D = theContext.GetPointer() as RenderTarget2D;
					mDevice.GraphicsDevice.SetRenderTarget(renderTarget2D);
					mStateMgr.SetProjectionTransform(Matrix.CreateOrthographicOffCenter(0f, renderTarget2D.Width, renderTarget2D.Height, 0f, -1000f, 1000f));
					mCurrentContex = theContext;
				}
			}
		}

		public override HRenderContext GetCurrentContext()
		{
			return mCurrentContex;
		}

		public override void PushState()
		{
			mStateMgr.PushState();
		}

		public override void PopState()
		{
			mStateMgr.PopState();
		}

		public override int Flush(uint inFlushFlags)
		{
			DoCommitAllRenderState();
			FlushBufferedTriangles();
			return 0;
		}

		public override void SetRenderRect(int theX, int theY, int theWidth, int theHeight)
		{
			mRenderRect = new Rectangle(theX, theY, theWidth, theHeight);
		}

		public override int Present(Rect theSrcRect, Rect theDestRect)
		{
			if (mBatchedTriangleIndex > 0)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}
			PresentScreenImage();
			return 0;
		}

		public override uint GetCapsFlags()
		{
			return uint.MaxValue;
		}

		public override uint GetMaxTextureStages()
		{
			return 0u;
		}

		public override string GetInfoString(EInfoString inInfoStr)
		{
			return "";
		}

		public override void GetBackBufferDimensions(ref uint outWidth, ref uint outHeight)
		{
		}

		public override int SceneBegun()
		{
			return 0;
		}

		public override bool CreateImageRenderData(ref MemoryImage inImage)
		{
			if (inImage != null && inImage.mRenderData != null)
			{
				XNATextureData xNATextureData = inImage.GetRenderData() as XNATextureData;
				if (xNATextureData != null)
				{
					if (xNATextureData.mOptimizedLoad)
					{
						xNATextureData.mImageFlags = inImage.GetImageFlags();
					}
					return true;
				}
				inImage.SetRenderData(null);
			}
			if (inImage != null)
			{
				SharedImageRef sharedImageRef = GlobalMembers.gSexyApp.mResourceManager.LoadImage(inImage.mNameForRes);
				inImage.Dispose();
				inImage = null;
				inImage = sharedImageRef.GetMemoryImage();
				if (inImage != null && inImage.mRenderData != null)
				{
					return true;
				}
			}
			return false;
		}

		public override void RemoveImageRenderData(MemoryImage img)
		{
			XNATextureData xNATextureData = img.GetRenderData() as XNATextureData;
			if (xNATextureData == null)
			{
				return;
			}
			for (int i = 0; i < xNATextureData.mTextures.Length; i++)
			{
				if (xNATextureData.mTextures[i] != null && xNATextureData.mTextures[i].mTexture != null && mStateMgr.mLastXNATextureSlots[0] == xNATextureData.mTextures[i].mTexture)
				{
					mStateMgr.mLastXNATextureSlots[0] = null;
				}
			}
			xNATextureData.Dispose();
			img.SetRenderData(null);
		}

		public override int RecoverImageBitsFromRenderData(MemoryImage inImage)
		{
			return 0;
		}

		public override int GetTextureMemorySize(MemoryImage theImage)
		{
			return 0;
		}

		public override PixelFormat GetTextureFormat(MemoryImage theImage)
		{
			return PixelFormat.PixelFormat_A4R4G4B4;
		}

		public override void AdjustVertexUVsEx(uint theVertexFormat, SexyVertex[] theVertices, int theVertexCount, int theVertexSize)
		{
		}

		public void AdjustVertsForAtlas(int inTextureIndex, ref VertexPositionColorTexture[] inVerts, int inStartIndex, int inVertCount, uint inVertFormat, int inStride, int inTexUVOfs)
		{
		}

		public Image SetupAtlasState(int inTextureIndex, Image inImage)
		{
			if (inImage == null)
			{
				return null;
			}
			if (inImage.mAtlasImage != null)
			{
				return inImage.mAtlasImage;
			}
			return inImage;
		}

		public override void DrawPrimitiveEx(uint theVertexFormat, Graphics3D.EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, SexyFramework.Graphics.Color theColor, int theDrawMode, float tx, float ty, bool blend, uint theFlags)
		{
			int num = 0;
			switch (thePrimitiveType)
			{
			case Graphics3D.EPrimitiveType.PT_PointList:
				num = thePrimitiveCount;
				break;
			case Graphics3D.EPrimitiveType.PT_LineList:
				num = thePrimitiveCount * 2;
				break;
			case Graphics3D.EPrimitiveType.PT_LineStrip:
				num = 1 + thePrimitiveCount;
				break;
			case Graphics3D.EPrimitiveType.PT_TriangleList:
				num = thePrimitiveCount * 3;
				break;
			case Graphics3D.EPrimitiveType.PT_TriangleStrip:
				num = 2 + thePrimitiveCount;
				break;
			case Graphics3D.EPrimitiveType.PT_TriangleFan:
				num = 2 + thePrimitiveCount;
				break;
			}
			if (num == 0 || thePrimitiveCount == 0 || !PreDraw())
			{
				return;
			}
			mStateMgr.PushState();
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
			SetupDrawMode(theDrawMode);
			mImage.InitAtalasState();
			if (mScratchPoly.Length < num) mScratchPoly = new VertexPositionColorTexture[num];
			VertexPositionColorTexture[] array = mScratchPoly;
			if ((theVertexFormat & 4) != 0 && (color.PackedValue != 0 || tx != 0f || ty != 0f || mTransformStack.Count != 0))
			{
				for (int i = 0; i < num; i++)
				{
					theVertices[i].x += tx;
					theVertices[i].y += ty;
					if (theVertices[i].color == SexyFramework.Graphics.Color.Zero)
					{
						theVertices[i].color = theColor;
					}
					if (mTransformStack.Count != 0)
					{
						SexyVector2 sexyVector = new SexyVector2(theVertices[i].x, theVertices[i].y);
						sexyVector = mTransformStack.Peek() * sexyVector;
						theVertices[i].x = sexyVector.x;
						theVertices[i].y = sexyVector.y;
					}
				}
			}
			for (int j = 0; j < num; j++)
			{
				array[j].Position.X = theVertices[j].x;
				array[j].Position.Y = theVertices[j].y;
				array[j].Position.Z = 0f;
				array[j].TextureCoordinate = mImage.mVectorBase + mImage.mVectorU * theVertices[j].u + mImage.mVectorV * theVertices[j].v;
				if (theVertices[j].color == SexyFramework.Graphics.Color.Zero)
				{
					array[j].Color = color;
				}
				else
				{
					array[j].Color = GetXNAColor(theVertices[j].color);
				}
			}
			mStateMgr.SetWorldTransform(Matrix.Identity);
			mStateMgr.mStateDirty = true;
			DrawPrimitiveInternal((int)thePrimitiveType, thePrimitiveCount, array, 0uL, theVertexFormat, true, Matrix.Identity);
			mStateMgr.mStateDirty = false;
			mStateMgr.PopState();
		}

		public override void SetBltDepth(float inDepth)
		{
		}

		public override void PushTransform(SexyTransform2D theTransform, bool concatenate)
		{
			if (mTransformStack.Count == 0 || !concatenate)
			{
				mTransformStack.Push(theTransform);
				return;
			}
			SexyTransform2D sexyTransform2D = mTransformStack.Peek();
			mTransformStack.Push(sexyTransform2D * theTransform);
		}

		public override void PopTransform()
		{
			if (mTransformStack.Count != 0)
			{
				mTransformStack.Pop();
			}
		}

		public override void PopTransform(ref SexyTransform2D theTransform)
		{
			if (mTransformStack.Count != 0)
			{
				theTransform = mTransformStack.Pop();
				return;
			}
			SexyTransform2D sexyTransform2D = default(SexyTransform2D);
			sexyTransform2D.LoadIdentity();
			theTransform = sexyTransform2D;
		}

		public override void ClearColorBuffer(SexyFramework.Graphics.Color inColor)
		{
			mDevice.GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(inColor.mRed, inColor.mGreen, inColor.mBlue, inColor.mAlpha));
		}

		public override void SetMaterialAmbient(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
		{
		}

		public override void SetMaterialDiffuse(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
		{
		}

		public override void SetMaterialSpecular(SexyFramework.Graphics.Color inColor, int inVertexColorComponent, float inPower)
		{
		}

		public override void SetMaterialEmissive(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
		{
		}

		public override void SetWorldTransform(SexyMatrix4 inMatrix)
		{
			mStateMgr.SetWorldTransform(GetXNAMatrix(inMatrix));
		}

		public override void SetViewTransform(SexyMatrix4 inMatrix)
		{
			mStateMgr.SetViewTransform(GetXNAMatrix(inMatrix));
		}

		public override void SetProjectionTransform(SexyMatrix4 inMatrix)
		{
			mStateMgr.SetProjectionTransform(GetXNAMatrix(inMatrix));
		}

		public override void SetTextureTransform(int inTextureIndex, SexyMatrix4 inMatrix, int inNumDimensions)
		{
		}

		public override void SetTextureWrap(int inTextureIndex, bool inWrapU, bool inWrapV)
		{
			if (inWrapU || inWrapV)
			{
				mStateMgr.SetSamplerState(inTextureIndex, SamplerState.LinearWrap);
			}
			else
			{
				mStateMgr.SetSamplerState(inTextureIndex, SamplerState.LinearClamp);
			}
		}

		public override void SetTextureLinearFilter(int inTextureIndex, bool inLinear)
		{
			if (!inLinear)
			{
				mStateMgr.SetSamplerState(inTextureIndex, SamplerState.PointClamp);
			}
			else
			{
				mStateMgr.SetSamplerState(inTextureIndex, SamplerState.LinearClamp);
			}
		}

		public override void SetTextureCoordSource(int inTextureIndex, int inUVComponent, Graphics3D.ETexCoordGen inTexGen)
		{
		}

		public override void SetTextureFactor(int inTextureFactor)
		{
		}

		public override void ClearDepthBuffer()
		{
			mDevice.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.ClearOptions.DepthBuffer, Microsoft.Xna.Framework.Color.Black, 1f, 0);
		}

		public override void SetDepthState(Graphics3D.ECompareFunc inDepthTestFunc, bool inDepthWriteEnabled)
		{
			bool depthTestEnabled = inDepthWriteEnabled
				|| (inDepthTestFunc != Graphics3D.ECompareFunc.COMPARE_ALWAYS
					&& inDepthTestFunc != Graphics3D.ECompareFunc.COMPARE_NEVER);
			mStateMgr.SetDepthStencilState(StateCache.GetDepth(depthTestEnabled, inDepthWriteEnabled, GetXNACompareFunc(inDepthTestFunc)));
		}

		public override void SetAlphaTest(Graphics3D.ECompareFunc inAlphaTestFunc, int inRefAlpha)
		{
		}

		public override void SetColorWriteState(int inWriteRedEnabled, int inWriteGreenEnabled, int inWriteBlueEnabled, int inWriteAlphaEnabled)
		{
		}

		public override void SetWireframe(int inWireframe)
		{
		}

		public override void SetBlend(Graphics3D.EBlendMode inSrcBlend, Graphics3D.EBlendMode inDestBlend)
		{
			mStateMgr.SetBlendOverride(inSrcBlend, inDestBlend);
		}

		public override void SetBackfaceCulling(int inCullClockwise, int inCullCounterClockwise)
		{
			if (inCullClockwise == 1 || inCullCounterClockwise == 1)
			{
				CullMode mode = inCullClockwise == 1 ? CullMode.CullClockwiseFace : CullMode.CullCounterClockwiseFace;
				mStateMgr.SetRasterizerState(StateCache.GetRaster(mode, FillMode.Solid));
			}
			else
			{
				mStateMgr.SetRasterizerState(StateCache.GetRaster(CullMode.None, FillMode.Solid));
            }
        }

		public override void SetLightingEnabled(int inLightingEnabled)
		{
		}

		public override void SetLightEnabled(int inLightIndex, int inEnabled)
		{
		}

		private struct MeshVertexPNUC : IVertexType
		{
			public Vector3 Position;
			public Vector3 Normal;
			public Microsoft.Xna.Framework.Color Color;
			public Vector2 TexCoord0;

			public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

			VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
		}

		// SexyVF flag bits matching JS at bejewel/js/gf/ext/_jsfext_webgl.js:339-352.
		private const int SexyVF_XYZ = 0x002;
		private const int SexyVF_Normal = 0x010;
		private const int SexyVF_Diffuse = 0x040;
		private const int SexyVF_Tex1 = 0x100;
		private const int SexyVF_Tex2 = 0x200;

		public void SetupMesh(Mesh theMesh)
		{
			GraphicsDevice gd = mDevice.GraphicsDevice;
			foreach (var aPiece in theMesh.mPieces)
			{
				if (aPiece.mVertexData == null) continue;
				MeshVertexPNUC[] vertices = new MeshVertexPNUC[aPiece.mVertexBufferCount];
				using (BinaryReader br = new BinaryReader(new MemoryStream(aPiece.mVertexData)))
				{
					for (int i = 0; i < aPiece.mVertexBufferCount; i++)
					{
						vertices[i].Position.X = br.ReadSingle();
						vertices[i].Position.Y = br.ReadSingle();
						vertices[i].Position.Z = br.ReadSingle();
						vertices[i].Normal.X = br.ReadSingle();
						vertices[i].Normal.Y = br.ReadSingle();
						vertices[i].Normal.Z = br.ReadSingle();
						byte r = br.ReadByte();
						byte g = br.ReadByte();
						byte b = br.ReadByte();
						byte a = br.ReadByte();
						vertices[i].Color = new Microsoft.Xna.Framework.Color(r, g, b, a);
						vertices[i].TexCoord0.X = br.ReadSingle();
						vertices[i].TexCoord0.Y = br.ReadSingle();
					}
				}
				aPiece.mXYZBuffer = new VertexBuffer(gd, MeshVertexPNUC.VertexDeclaration, aPiece.mVertexBufferCount, BufferUsage.WriteOnly);
				aPiece.mXYZBuffer.SetData(vertices);
				aPiece.mNormalBuffer = null;
				aPiece.mColorBuffer = null;
				aPiece.mTexCoords0Buffer = null;
				aPiece.mTexCoords1Buffer = null;

				ushort[] indices = new ushort[aPiece.mIndexBufferCount];
				System.Buffer.BlockCopy(aPiece.mIndexData, 0, indices, 0, aPiece.mIndexBufferCount * 2);
				aPiece.mIndexBuffer = new IndexBuffer(gd, IndexElementSize.SixteenBits, aPiece.mIndexBufferCount, BufferUsage.WriteOnly);
				aPiece.mIndexBuffer.SetData(indices);

				// Free the raw bytes; we won't need them again.
				aPiece.mVertexData = null;
				aPiece.mIndexData = null;
			}
		}

		private bool mLogBlendOnce = true;
		private bool mLoggedRenderMeshError;
		private bool mLoggedRenderMeshSinglePassError;

		private static void SetEffectMatrixForSemantic(EffectParameter param, Matrix matrix, XNAMatrixSemanticFlags flags)
		{
			if (param == null)
			{
				return;
			}

			if ((flags & XNAMatrixSemanticFlags.Transpose) != 0)
			{
				// Prime uploads _TRANSPOSE semantics as raw matrix memory. In MonoGame,
				// SetValueTranspose is the raw upload path because SetValue(Matrix)
				// transposes into the effect constant buffer internally.
				param.SetValueTranspose(matrix);
				return;
			}

			param.SetValue(matrix);
		}

		private static Matrix ComposeSemanticMatrix(XNAMatrixSemanticFlags flags, Matrix world, Matrix view, Matrix proj)
		{
			Matrix result = Matrix.Identity;
			bool hasMatrix = false;

			if ((flags & XNAMatrixSemanticFlags.World) != 0)
			{
				result = world;
				hasMatrix = true;
			}
			if ((flags & XNAMatrixSemanticFlags.View) != 0)
			{
				result = hasMatrix ? result * view : view;
				hasMatrix = true;
			}
			if ((flags & XNAMatrixSemanticFlags.Projection) != 0)
			{
				result = hasMatrix ? result * proj : proj;
				hasMatrix = true;
			}

			return hasMatrix ? result : Matrix.Identity;
		}

		private void CommitActiveEffectMatrices(XNARenderEffect activeRE)
		{
			Matrix world = mStateMgr.mXNAWorldMatrix;
			Matrix view = mStateMgr.mXNAViewMatrix;
			Matrix proj = mStateMgr.mXNAProjectionMatrix;

			CommitEffectMatrices(activeRE, world, view, proj);
		}

		private void CommitEffectMatrices(XNARenderEffect activeRE, Matrix world, Matrix view, Matrix proj)
		{
			if (activeRE == null)
			{
				return;
			}

			foreach (var binding in activeRE.mMatrixSemantics)
			{
				Matrix matrix = ComposeSemanticMatrix(binding.Flags, world, view, proj);
				SetEffectMatrixForSemantic(binding.Param, matrix, binding.Flags);
			}
		}

		public void RenderMeshSinglePass(Mesh theMesh, SexyMatrix4 theMatrix, int passIndex)
		{
			SetupMesh(theMesh);
			GraphicsDevice gd = mDevice.GraphicsDevice;
			SetWorldTransform(theMatrix);
			SetupDrawMode(0);
			theMesh.mListener?.MeshPreDraw(theMesh);

			XNARenderEffect activeRE = (mStateMgr.mActiveEffects != null && mStateMgr.mActiveEffects.Count > 0)
				? mStateMgr.mActiveEffects[mStateMgr.mActiveEffects.Count - 1] : null;
			Effect activeEffect = activeRE?.mDefinition.mEffect;

			foreach (var aPiece in theMesh.mPieces)
			{
				if (aPiece.mIndexBuffer == null || aPiece.mXYZBuffer == null) continue;
				try
				{
					gd.SetVertexBuffer(aPiece.mXYZBuffer);
					gd.Indices = aPiece.mIndexBuffer;

					if (aPiece.mTexture != null && aPiece.mTexture.GetImage() != null)
					{
						SetTexture(0, aPiece.mTexture.GetImage());
					}
					else
					{
						SetTexture(0, null);
					}
					if (aPiece.mBumpTexture != null && aPiece.mBumpTexture.GetImage() != null)
					{
						SetTexture(1, aPiece.mBumpTexture.GetImage());
					}
					else
					{
						SetTexture(1, null);
					}
					theMesh.mListener?.MeshPreDrawSet(theMesh, aPiece.mObjectName, aPiece.mSetName, aPiece.mBumpTexture != null && aPiece.mBumpTexture.GetImage() != null);
					SetupDrawMode(0);

					if (activeRE != null)
					{
						DoCommitEffectRenderState(activeRE);
					}
					else
					{
						DoCommitAllRenderState();
					}

					int triCount = aPiece.mIndexBufferCount / 3;
					if (activeEffect != null)
					{
						var passes = activeEffect.CurrentTechnique.Passes;
						if (passIndex >= 0 && passIndex < passes.Count)
						{
							passes[passIndex].Apply();
							gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, triCount);
						}
					}
					else
					{
						mBasicEffect.CurrentTechnique.Passes[0].Apply();
						gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, triCount);
					}

					theMesh.mListener?.MeshPostDrawSet(theMesh, aPiece.mObjectName, aPiece.mSetName);
				}
				catch (Exception ex)
				{
					if (!mLoggedRenderMeshSinglePassError)
					{
						mLoggedRenderMeshSinglePassError = true;
						Console.WriteLine($"[RenderMeshSinglePass] {ex.GetType().Name}: {ex.Message}");
					}
				}
			}

			theMesh.mListener?.MeshPostDraw(theMesh);
		}

		// Port of JS JSFExt_RenderMesh (bejewel/js/gf/ext/_jsfext_webgl.js:494).
		// Binds the currently-active Effect's matrix params and per-piece textures,
		// then draws each piece as an indexed TRIANGLES list.
		// Cross-checked against Prime D3DInterface::RenderMesh (D3DInterface.cpp:3629).
		public override void RenderMesh(Mesh theMesh, SexyMatrix4 theMatrix, SexyFramework.Graphics.Color theColor, bool doSetup)
		{
			SetupMesh(theMesh);

			GraphicsDevice gd = mDevice.GraphicsDevice;

			// Push the world matrix; view/projection are set by the caller (HyperspaceUltra).
			SetWorldTransform(theMatrix);
			SetupDrawMode(0);

			// Notify the listener (Prime mirrors this with MeshPreDraw).
			theMesh.mListener?.MeshPreDraw(theMesh);

			// Rebind whatever Effect the active RenderEffect set up. RenderEffect.Begin
			// has already configured techniques and uniforms; we just need to feed it
			// world/view/proj from the state manager and bind per-piece textures.
			XNARenderEffect activeRE = (mStateMgr.mActiveEffects != null && mStateMgr.mActiveEffects.Count > 0)
				? mStateMgr.mActiveEffects[mStateMgr.mActiveEffects.Count - 1] : null;
			Effect activeEffect = activeRE?.mDefinition.mEffect;

			foreach (var aPiece in theMesh.mPieces)
			{
				if (aPiece.mIndexBuffer == null || aPiece.mXYZBuffer == null) continue;

				try
				{
					// Bind the piece's diffuse texture first; listeners may override
					// the active texture slots afterwards, matching JS / Prime order.
					if (aPiece.mTexture != null && aPiece.mTexture.GetImage() != null)
					{
						SetTexture(0, aPiece.mTexture.GetImage());
					}
					else
					{
						SetTexture(0, null);
					}
					if (aPiece.mBumpTexture != null && aPiece.mBumpTexture.GetImage() != null)
					{
						SetTexture(1, aPiece.mBumpTexture.GetImage());
					}
					else
					{
						SetTexture(1, null);
					}

					// Per-piece listener hook (used by Hyperspace to push EFFECT_TUBE_3D's
					// IMAGE_WARP_LINES_01 / IMAGE_HYPERSPACE_INITIAL into TS0/TS1).
					bool hasBump = aPiece.mBumpTexture != null && aPiece.mBumpTexture.GetImage() != null;
					theMesh.mListener?.MeshPreDrawSet(theMesh, aPiece.mObjectName, aPiece.mSetName, hasBump);
					SetupDrawMode(0);

					// Sync the active effect and the device state from the state manager
					// AFTER the listener ran, so listener texture overrides win.
					if (activeRE != null)
					{
						DoCommitEffectRenderState(activeRE);
					}
					else
					{
						DoCommitAllRenderState();
					}

					gd.SetVertexBuffer(aPiece.mXYZBuffer);
					gd.Indices = aPiece.mIndexBuffer;

					// RenderMesh is a single draw under the caller's active pass/state.
					int triCount = aPiece.mIndexBufferCount / 3;
					if (activeEffect == null)
					{
						mBasicEffect.CurrentTechnique.Passes[0].Apply();
					}
					else if (activeRE.mCurrentPass >= 0 && activeRE.mCurrentPass < activeEffect.CurrentTechnique.Passes.Count)
					{
						activeEffect.CurrentTechnique.Passes[activeRE.mCurrentPass].Apply();
					}
					gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, triCount);

					theMesh.mListener?.MeshPostDrawSet(theMesh, aPiece.mObjectName, aPiece.mSetName);
				}
				catch (Exception ex)
				{
					if (!mLoggedRenderMeshError)
					{
						mLoggedRenderMeshError = true;
						Console.WriteLine($"[RenderMesh] {ex.GetType().Name}: {ex.Message}");
					}
				}
			}

			theMesh.mListener?.MeshPostDraw(theMesh);
		}

		public void DoCommitAllRenderState()
		{
			Texture2D tex = mStateMgr.mXNATextureSlots[0];
			GraphicsDevice gd = mDevice.GraphicsDevice;

			if (mSexy2DEffect != null)
			{
				mSexy2DEffect.CurrentTechnique = (tex != null) ? mSexy2DTextured : mSexy2DUntextured;
				if (mSexy2DWVP != null) mSexy2DWVP.SetValue(mStateMgr.mXNAWorldMatrix * mStateMgr.mXNAViewMatrix * mStateMgr.mXNAProjectionMatrix);
				if (mSexy2DTex0 != null) mSexy2DTex0.SetValue(tex);
			}
			else
			{
				mBasicEffect.Projection = mStateMgr.mXNAProjectionMatrix;
				mBasicEffect.View = mStateMgr.mXNAViewMatrix;
				mBasicEffect.World = mStateMgr.mXNAWorldMatrix;
				mBasicEffect.VertexColorEnabled = true;
				mBasicEffect.Texture = tex;
				mBasicEffect.TextureEnabled = (tex != null);
			}

			gd.Textures[0]        = tex;
			gd.SamplerStates[0]   = mStateMgr.mXNASamplerStateSlots[0] ?? SamplerState.LinearClamp;
			gd.RasterizerState    = mStateMgr.mXNARasterizerState;
			gd.BlendState         = mStateMgr.mXNABlendState;
			gd.DepthStencilState  = mStateMgr.mXNADepthStencilState;
		}

		public void DoCommitLastAllRenderState()
		{
			Texture2D tex = mStateMgr.mLastXNATextureSlots[0];
			GraphicsDevice gd = mDevice.GraphicsDevice;
			Matrix proj = mStateMgr.mProjectMatrixDirty ? mStateMgr.mXNALastProjectionMatrix : mStateMgr.mXNAProjectionMatrix;

			if (mSexy2DEffect != null)
			{
				mSexy2DEffect.CurrentTechnique = (tex != null) ? mSexy2DTextured : mSexy2DUntextured;
				if (mSexy2DWVP != null) mSexy2DWVP.SetValue(mStateMgr.mXNALastWorldMatrix * mStateMgr.mXNAViewMatrix * proj);
				if (mSexy2DTex0 != null) mSexy2DTex0.SetValue(tex);
			}
			else
			{
				mBasicEffect.Projection = proj;
				mBasicEffect.View = mStateMgr.mXNAViewMatrix;
				mBasicEffect.World = mStateMgr.mXNALastWorldMatrix;
				mBasicEffect.VertexColorEnabled = true;
				mBasicEffect.Texture = tex;
				mBasicEffect.TextureEnabled = (tex != null);
			}

			gd.Textures[0]        = tex;
			gd.SamplerStates[0]   = mStateMgr.mXNALastSamplerStateSlots[0] ?? SamplerState.LinearClamp;
			gd.RasterizerState    = mStateMgr.mXNARasterizerState;
			gd.BlendState         = mStateMgr.mXNALastBlendState ?? mStateMgr.mXNABlendState;
			gd.DepthStencilState  = mStateMgr.mXNADepthStencilState;
		}
		
		public void DoCommitEffectRenderState(RenderEffect aEffect)
		{
			XNARenderEffect xna = aEffect as XNARenderEffect;
			if (xna == null) return;

			CommitActiveEffectMatrices(xna);

			if (xna.mParamTex0 != null) xna.mParamTex0.SetValue(mStateMgr.mXNATextureSlots[0]);
			if (xna.mParamTex1 != null) xna.mParamTex1.SetValue(mStateMgr.mXNATextureSlots[1]);
			if (xna.mParamTex2 != null) xna.mParamTex2.SetValue(mStateMgr.mXNATextureSlots[2]);

			GraphicsDevice gd = mDevice.GraphicsDevice;
			for (int i = 0; i < 3; i++)
			{
				gd.Textures[i] = mStateMgr.mXNATextureSlots[i];
                if (mStateMgr.mXNASamplerStateSlots[i] != null)
					gd.SamplerStates[i] = mStateMgr.mXNASamplerStateSlots[i];
			}
			gd.RasterizerState   = mStateMgr.mXNARasterizerState;
			gd.BlendState        = mStateMgr.mXNABlendState;
			gd.DepthStencilState = mStateMgr.mXNADepthStencilState;
		}

		public void DoCommitEffectLastRenderState(RenderEffect aEffect)
		{
			XNARenderEffect xna = aEffect as XNARenderEffect;
			if (xna == null) return;

			Matrix proj = mStateMgr.mProjectMatrixDirty ? mStateMgr.mXNALastProjectionMatrix : mStateMgr.mXNAProjectionMatrix;
			Matrix world = mStateMgr.mXNALastWorldMatrix;
			Matrix view = mStateMgr.mXNAViewMatrix;
			CommitEffectMatrices(xna, world, view, proj);

			GraphicsDevice gd = mDevice.GraphicsDevice;
			for (int i = 0; i < 3; i++)
			{
				gd.Textures[i] = mStateMgr.mLastXNATextureSlots[i];
				if (mStateMgr.mXNALastSamplerStateSlots[i] != null)
					gd.SamplerStates[i] = mStateMgr.mXNALastSamplerStateSlots[i];
			}
			gd.RasterizerState   = mStateMgr.mXNARasterizerState;
			gd.BlendState        = mStateMgr.mXNALastBlendState ?? mStateMgr.mXNABlendState;
			gd.DepthStencilState = mStateMgr.mXNADepthStencilState;
		}

		public override void ClearRect(Rect theRect)
		{
		}

		public override void FillRect(Rect theRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
		{
			if (PreDraw())
			{
				SetupDrawMode(theDrawMode);
				float num = (float)theRect.mX + mPixelOffset;
				float num2 = (float)theRect.mY + mPixelOffset;
				float num3 = theRect.mWidth;
				float num4 = theRect.mHeight;
				float z = 0f;
				Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
				mTmpVPCBuffer[0].Position = new Vector3(num, num2, z);
				mTmpVPCBuffer[0].Color = color;
				mTmpVPCBuffer[1].Position = new Vector3(num, num2 + num4, z);
				mTmpVPCBuffer[1].Color = color;
				mTmpVPCBuffer[2].Position = new Vector3(num + num3, num2, z);
				mTmpVPCBuffer[2].Color = color;
				mTmpVPCBuffer[3].Position = new Vector3(num + num3, num2 + num4, z);
				mTmpVPCBuffer[3].Color = color;
				SetTextureDirect(0, null);
				mStateMgr.SetWorldTransform(Matrix.Identity);
				DrawPrimitiveInternal(5, 2, mTmpVPCBuffer, 32uL, mDefaultVertexFVF, true, Matrix.Identity);
			}
		}

		public override void FillScanLinesWithCoverage(Span theSpans, int theSpanCount, SexyFramework.Graphics.Color theColor, int theDrawMode, string theCoverage, int theCoverX, int theCoverY, int theCoverWidth, int theCoverHeight)
		{
		}

		public override void FillPoly(SexyFramework.Misc.Point[] theVertices, int theNumVertices, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, int tx, int ty)
		{
			if (theNumVertices < 3 || !PreDraw())
			{
				return;
			}
			SetupDrawMode(theDrawMode);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
			float z = 0f;
			if (mScratchPoly.Length < theNumVertices) mScratchPoly = new VertexPositionColorTexture[theNumVertices];
			for (int i = 0; i < theNumVertices; i++)
			{
				mScratchPoly[i].Position = new Vector3((float)theVertices[i].mX + (float)tx, (float)theVertices[i].mY + (float)ty, z);
				mScratchPoly[i].Color = color;
				if (mTransformStack.Count != 0)
				{
					SexyVector2 sexyVector = new SexyVector2(mScratchPoly[i].Position.X, mScratchPoly[i].Position.Y);
					sexyVector = mTransformStack.Peek() * sexyVector;
					mScratchPoly[i].Position.X = sexyVector.x;
					mScratchPoly[i].Position.Y = sexyVector.y;
				}
			}
			DrawPolyClipped(theClipRect, mScratchPoly, theNumVertices);
		}

		public void DrawPolyClipped(Rect theClipRect, VertexPositionColorTexture[] theList)
		{
			DrawPolyClipped(theClipRect, theList, theList.Length);
		}

		public void DrawPolyClipped(Rect theClipRect, VertexPositionColorTexture[] theList, int theCount)
		{
			DPC_l2.Clear();
			DPC_l1.Clear();
			for (int i = 0; i < theCount; i++) DPC_l1.Add(theList[i]);
			int mX = theClipRect.mX;
			int num = mX + theClipRect.mWidth;
			int mY = theClipRect.mY;
			int num2 = mY + theClipRect.mHeight;
			ClipPoints(0, mX, ClipperType.Clipper_Less, DPC_l1, DPC_l2);
			DPC_l1.Clear();
			ClipPoints(1, mY, ClipperType.Clipper_Less, DPC_l2, DPC_l1);
			DPC_l2.Clear();
			ClipPoints(0, num, ClipperType.Clipper_GreaterEqual, DPC_l1, DPC_l2);
			DPC_l1.Clear();
			ClipPoints(1, num2, ClipperType.Clipper_GreaterEqual, DPC_l2, DPC_l1);
			CheckBatchAndCommit();
			if (DPC_l1.Count >= 3)
			{
				int n = DPC_l1.Count;
				if (mScratchPoly.Length < n) mScratchPoly = new VertexPositionColorTexture[n];
				for (int i = 0; i < n; i++) mScratchPoly[i] = DPC_l1[i];
				BufferedDrawPrimitive(6, n - 2, mScratchPoly, (int)mDefaultVertexSize, mDefaultVertexFVF, Matrix.Identity);
			}
		}

		public void ClipPoint(int index, float clipValue, ClipperType type, VertexPositionColorTexture vertex1, VertexPositionColorTexture vertex2, List<VertexPositionColorTexture> outList)
		{
			float vertexValue = GetVertexValue(index, vertex1);
			float vertexValue2 = GetVertexValue(index, vertex2);
			switch (type)
			{
			case ClipperType.Clipper_Less:
				if (vertexValue >= clipValue)
				{
					if (vertexValue2 >= clipValue)
					{
						outList.Add(vertex2);
						break;
					}
					float t3 = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
					outList.Add(Interpolate(vertex1, vertex2, t3));
				}
				else if (vertexValue2 >= clipValue)
				{
					float t4 = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
					outList.Add(Interpolate(vertex1, vertex2, t4));
					outList.Add(vertex2);
				}
				break;
			case ClipperType.Clipper_GreaterEqual:
				if (vertexValue < clipValue)
				{
					if (vertexValue2 < clipValue)
					{
						outList.Add(vertex2);
						break;
					}
					float t = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
					outList.Add(Interpolate(vertex1, vertex2, t));
				}
				else if (vertexValue2 < clipValue)
				{
					float t2 = (clipValue - vertexValue) / (vertexValue2 - vertexValue);
					outList.Add(Interpolate(vertex1, vertex2, t2));
					outList.Add(vertex2);
				}
				break;
			case ClipperType.Clipper_Greater:
			case ClipperType.Clipper_Equal:
				break;
			}
		}

		public void ClipPoints(int index, float clipValue, ClipperType type, List<VertexPositionColorTexture> inList, List<VertexPositionColorTexture> outList)
		{
			if (inList.Count >= 2)
			{
				ClipPoint(index, clipValue, type, inList[inList.Count - 1], inList[0], outList);
				for (int i = 0; i < inList.Count - 1; i++)
				{
					ClipPoint(index, clipValue, type, inList[i], inList[i + 1], outList);
				}
			}
		}

		public float GetVertexValue(int index, VertexPositionColorTexture vertex)
		{
			switch (index)
			{
			case 0:
				return vertex.Position.X;
			case 1:
				return vertex.Position.Y;
			case 2:
				return vertex.Position.Z;
			default:
				return 0f;
			}
		}

		private VertexPositionColorTexture Interpolate(VertexPositionColorTexture v1, VertexPositionColorTexture v2, float t)
		{
			VertexPositionColorTexture result = v1;
			result.Position.X = v1.Position.X + t * (v2.Position.X - v1.Position.X);
			result.Position.Y = v1.Position.Y + t * (v2.Position.Y - v1.Position.Y);
			result.TextureCoordinate.X = v1.TextureCoordinate.X + t * (v2.TextureCoordinate.X - v1.TextureCoordinate.X);
			result.TextureCoordinate.Y = v1.TextureCoordinate.Y + t * (v2.TextureCoordinate.Y - v1.TextureCoordinate.Y);
			if (v1.Color != v2.Color)
			{
				Vector4 color = Vector4.Lerp(v1.Color.ToVector4(), v2.Color.ToVector4(), t);
				Microsoft.Xna.Framework.Color color2 = new Microsoft.Xna.Framework.Color(color);
				result.Color = color2;
			}
			return result;
		}

		public override void DrawLine(double theStartX, double theStartY, double theEndX, double theEndY, SexyFramework.Graphics.Color theColor, int theDrawMode, bool antiAlias)
		{
			if (PreDraw())
			{
				SetupDrawMode(theDrawMode);
				float x = (float)theStartX;
				float y = (float)theStartY;
				float x2 = (float)theEndX;
				float y2 = (float)theEndY;
				float z = 0f;
				Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
				mScratchLine[0] = new VertexPositionColor(new Vector3(x, y, z), color);
				mScratchLine[1] = new VertexPositionColor(new Vector3(x2, y2, z), color);
				SetTextureDirect(0, null);
				mStateMgr.SetWorldTransform(Matrix.Identity);
				CheckBatchAndCommit();
				FlushBufferedTriangles();
				DoCommitAllRenderState();
				DrawPrimitiveInternal(3, 1, mScratchLine, 32uL, mDefaultVertexFVF, false, Matrix.Identity);
			}
		}

		public override void Blt(Image theImage, int theX, int theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
		{
			BltNoClipF(theImage, theX, theY, theSrcRect, theColor, theDrawMode, false);
		}

		public override void BltF(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
		{
			FRect theTRect = new FRect(theClipRect.mX, theClipRect.mY, theClipRect.mWidth, theClipRect.mHeight);
			FRect fRect = new FRect(theX, theY, theSrcRect.mWidth, theSrcRect.mHeight);
			FRect fRect2 = fRect.Intersection(theTRect);
			if (fRect2.mWidth != fRect.mWidth || fRect2.mHeight != fRect.mHeight)
			{
				if (fRect2.mWidth != 0f && fRect2.mHeight != 0f)
				{
					BltClipF(theImage, theX, theY, theSrcRect, theClipRect, theColor, theDrawMode);
				}
			}
			else
			{
				BltNoClipF(theImage, theX, theY, theSrcRect, theColor, theDrawMode, true);
			}
		}

		public override void BltRotated(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, double theRot, float theRotCenterX, float theRotCenterY)
		{
			mTransform.Reset();
			mTransform.Translate(0f - theRotCenterX, 0f - theRotCenterY);
			mTransform.RotateRad((float)theRot);
			mTransform.Translate(theX + theRotCenterX, theY + theRotCenterY);
			BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, mTransform.GetMatrix(), true, 0f, 0f, false);
		}

		private void BltTransformed(Image theImage, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, Rect theSrcRect, SexyTransform2D theTransform, bool linearFilter, float theX, float theY, bool center)
		{
			if (!PreDraw())
			{
				return;
			}
			if (mTransformStack.Count != 0)
			{
				if (theX != 0f || theY != 0f)
				{
					SexyTransform2D sexyTransform2D = new SexyTransform2D(false);
					if (center)
					{
						sexyTransform2D.Translate((float)(-theSrcRect.mWidth) / 2f, (float)(-theSrcRect.mHeight) / 2f);
					}
					sexyTransform2D = theTransform * sexyTransform2D;
					sexyTransform2D.Translate(theX, theY);
					sexyTransform2D = mTransformStack.Peek() * sexyTransform2D;
					BltTransformHelper(theImage, theClipRect, theColor, theDrawMode, theSrcRect, sexyTransform2D, linearFilter, theX, theY, center);
				}
				else
				{
					SexyTransform2D theTransform2 = mTransformStack.Peek() * theTransform;
					BltTransformHelper(theImage, theClipRect, theColor, theDrawMode, theSrcRect, theTransform2, linearFilter, theX, theY, center);
				}
			}
			else
			{
				BltTransformHelper(theImage, theClipRect, theColor, theDrawMode, theSrcRect, theTransform, linearFilter, theX, theY, center);
			}
		}

		public override void BltMatrix(Image theImage, float x, float y, SexyTransform2D theMatrix, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, Rect theSrcRect, bool blend)
		{
			BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, theMatrix, blend, x, y, true);
		}

		public override void BltTriangles(Image theImage, SexyVertex2D[,] theVertices, int theNumTriangles, SexyFramework.Graphics.Color theColor, int theDrawMode, float tx, float ty, bool blend, Rect theClipRect)
		{
            Image image = theImage;
            image.InitAtalasState();
            theImage = SetupAtlasState(0, theImage);
            MemoryImage inImage = theImage as MemoryImage;
            if (!CreateImageRenderData(ref inImage))
            {
                return;
            }
            SetupDrawMode(theDrawMode);
            XNATextureData xNATextureData = inImage.GetRenderData() as XNATextureData;
            if (xNATextureData == null)
            {
                return;
            }
            if (!((double)xNATextureData.mMaxTotalU <= 1.0) || !((double)xNATextureData.mMaxTotalV <= 1.0))
            {
                return;
            }
            SetTextureDirect(0, xNATextureData.mTextures[0].mTexture);
            float z = 0f;
            bool flag = mTransformStack.Count != 0;
            bool flag2 = theClipRect != Rect.INVALIDATE_RECT && (theClipRect.mX != 0 || theClipRect.mY != 0 || theClipRect.mWidth != mScreenWidth || theClipRect.mHeight != mScreenHeight);
            CheckBatchAndCommit();
            if (flag)
            {
                SexyMatrix3 sexyMatrix = mTransformStack.Peek();
                for (int k = 0; k < theNumTriangles; k++)
                {
                    if (mBatchedTriangleIndex > mBatchedTriangleSize - 3)
                    {
                        DoCommitAllRenderState();
                        FlushBufferedTriangles();
                    }
                    float ax0 = theVertices[k, 0].x + tx, ay0 = theVertices[k, 0].y + ty;
                    float ax1 = theVertices[k, 1].x + tx, ay1 = theVertices[k, 1].y + ty;
                    float ax2 = theVertices[k, 2].x + tx, ay2 = theVertices[k, 2].y + ty;
                    float tx0 = ax0 * sexyMatrix.m00 + ay0 * sexyMatrix.m01 + sexyMatrix.m02;
                    float ty0 = ax0 * sexyMatrix.m10 + ay0 * sexyMatrix.m11 + sexyMatrix.m12;
                    float tx1 = ax1 * sexyMatrix.m00 + ay1 * sexyMatrix.m01 + sexyMatrix.m02;
                    float ty1 = ax1 * sexyMatrix.m10 + ay1 * sexyMatrix.m11 + sexyMatrix.m12;
                    float tx2 = ax2 * sexyMatrix.m00 + ay2 * sexyMatrix.m01 + sexyMatrix.m02;
                    float ty2 = ax2 * sexyMatrix.m10 + ay2 * sexyMatrix.m11 + sexyMatrix.m12;
                    Vector2 uv0 = image.mVectorBase + image.mVectorU * theVertices[k, 0].u + image.mVectorV * theVertices[k, 0].v;
                    Vector2 uv1 = image.mVectorBase + image.mVectorU * theVertices[k, 1].u + image.mVectorV * theVertices[k, 1].v;
                    Vector2 uv2 = image.mVectorBase + image.mVectorU * theVertices[k, 2].u + image.mVectorV * theVertices[k, 2].v;
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(tx0, ty0, z), (theVertices[k, 0].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[k, 0].color) : GetXNAColor(theColor), new Vector2(uv0.X * xNATextureData.mMaxTotalU, uv0.Y * xNATextureData.mMaxTotalV));
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(tx1, ty1, z), (theVertices[k, 1].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[k, 1].color) : GetXNAColor(theColor), new Vector2(uv1.X * xNATextureData.mMaxTotalU, uv1.Y * xNATextureData.mMaxTotalV));
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(tx2, ty2, z), (theVertices[k, 2].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[k, 2].color) : GetXNAColor(theColor), new Vector2(uv2.X * xNATextureData.mMaxTotalU, uv2.Y * xNATextureData.mMaxTotalV));
                    AdjustVertsForAtlas(0, ref mBatchedTriangleBuffer, mBatchedTriangleIndex - 3, 3, 0u, 32, 0);
                    if (!SUPPORT_HW_CLIP && flag2)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            mScratchTri[l] = mBatchedTriangleBuffer[mBatchedTriangleIndex - (3 - l)];
                        }
                        mBatchedTriangleIndex -= 3;
                        DrawPolyClipped(theClipRect, mScratchTri);
                    }
                }
                return;
            }
            if (!SUPPORT_HW_CLIP && flag2)
            {
                for (int m = 0; m < theNumTriangles; m++)
                {
                    if (mBatchedTriangleIndex > mBatchedTriangleSize - 3)
                    {
                        DoCommitAllRenderState();
                        FlushBufferedTriangles();
                    }
                    Vector2 uv0m = image.mVectorBase + image.mVectorU * theVertices[m, 0].u + image.mVectorV * theVertices[m, 0].v;
                    Vector2 uv1m = image.mVectorBase + image.mVectorU * theVertices[m, 1].u + image.mVectorV * theVertices[m, 1].v;
                    Vector2 uv2m = image.mVectorBase + image.mVectorU * theVertices[m, 2].u + image.mVectorV * theVertices[m, 2].v;
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(theVertices[m, 0].x, theVertices[m, 0].y, z), (theVertices[m, 0].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[m, 0].color) : GetXNAColor(theColor), new Vector2(uv0m.X * xNATextureData.mMaxTotalU, uv0m.Y * xNATextureData.mMaxTotalV));
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(theVertices[m, 1].x, theVertices[m, 1].y, z), (theVertices[m, 1].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[m, 1].color) : GetXNAColor(theColor), new Vector2(uv1m.X * xNATextureData.mMaxTotalU, uv1m.Y * xNATextureData.mMaxTotalV));
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(theVertices[m, 2].x, theVertices[m, 2].y, z), (theVertices[m, 2].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[m, 2].color) : GetXNAColor(theColor), new Vector2(uv2m.X * xNATextureData.mMaxTotalU, uv2m.Y * xNATextureData.mMaxTotalV));
                    AdjustVertsForAtlas(0, ref mBatchedTriangleBuffer, mBatchedTriangleIndex - 3, 3, 0u, 32, 0);
                    if (!SUPPORT_HW_CLIP && flag2)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            mScratchTri[n] = mBatchedTriangleBuffer[mBatchedTriangleIndex - (3 - n)];
                        }
                        mBatchedTriangleIndex -= 3;
                        DrawPolyClipped(theClipRect, mScratchTri);
                    }
                }
                return;
            }
            int num3 = 0;
            while (num3 < theNumTriangles)
            {
                if (mBatchedTriangleIndex >= mBatchedTriangleSize)
                {
                    DoCommitAllRenderState();
                    FlushBufferedTriangles();
                }
                int inStartIndex = mBatchedTriangleIndex;
                int num4 = 0;
                int num5 = Math.Min(mBatchedTriangleSize - mBatchedTriangleIndex, theNumTriangles - num3);
                while (num4 < num5)
                {
                    Vector2 uv0n = image.mVectorBase + image.mVectorU * theVertices[num3, 0].u + image.mVectorV * theVertices[num3, 0].v;
                    Vector2 uv1n = image.mVectorBase + image.mVectorU * theVertices[num3, 1].u + image.mVectorV * theVertices[num3, 1].v;
                    Vector2 uv2n = image.mVectorBase + image.mVectorU * theVertices[num3, 2].u + image.mVectorV * theVertices[num3, 2].v;
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(theVertices[num3, 0].x, theVertices[num3, 0].y, z), (theVertices[num3, 0].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[num3, 0].color) : GetXNAColor(theColor), new Vector2(uv0n.X * xNATextureData.mMaxTotalU, uv0n.Y * xNATextureData.mMaxTotalV));
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(theVertices[num3, 1].x, theVertices[num3, 1].y, z), (theVertices[num3, 1].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[num3, 1].color) : GetXNAColor(theColor), new Vector2(uv1n.X * xNATextureData.mMaxTotalU, uv1n.Y * xNATextureData.mMaxTotalV));
                    mBatchedTriangleBuffer[mBatchedTriangleIndex++] = new VertexPositionColorTexture(new Vector3(theVertices[num3, 2].x, theVertices[num3, 2].y, z), (theVertices[num3, 2].color != SexyFramework.Graphics.Color.Zero) ? GetXNAColor(theVertices[num3, 2].color) : GetXNAColor(theColor), new Vector2(uv2n.X * xNATextureData.mMaxTotalU, uv2n.Y * xNATextureData.mMaxTotalV));
                    num4 += 3;
                    num3++;
                }
                AdjustVertsForAtlas(0, ref mBatchedTriangleBuffer, inStartIndex, num4, 0u, 32, 0);
            }
        }

		private void CheckBatchAndCommit()
		{
			mStateMgr.mStateDirty = false;
			mStateMgr.mTextureStateDirty = false;
			mStateMgr.mProjectMatrixDirty = false;
		}

		public void FlushBatchBeforeStateChange()
		{
			if (mSceneBegun && mBatchedTriangleIndex > 0)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}
		}

		private void FlushBufferedTriangles()
		{
			if (mSceneBegun && mBatchedTriangleIndex > 0)
			{
				int inPrimCount = mBatchedTriangleIndex / 3;
				DrawPrimitiveInternal(4, inPrimCount, mBatchedTriangleBuffer, 32uL, mDefaultVertexFVF, false, Matrix.Identity);
				mBatchedTriangleIndex = 0;
				mBatchedIndexIndex = 0;
			}
		}

		public override void BltMirror(Image theImage, int theX, int theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
		{
			mTransform.Reset();
			mTransform.Translate(0f - (float)theSrcRect.mWidth, 0f);
			mTransform.Scale(-1f, 1f);
			mTransform.Translate(theX, theY);
			BltTransformed(theImage, Rect.INVALIDATE_RECT, theColor, theDrawMode, theSrcRect, mTransform.GetMatrix(), false, 0f, 0f, false);
		}

		public override void BltStretched(Image theImage, Rect theDestRect, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, bool fastStretch, bool mirror)
		{
			float num = (float)theDestRect.mWidth / (float)theSrcRect.mWidth;
			float sy = (float)theDestRect.mHeight / (float)theSrcRect.mHeight;
			mTransform.Reset();
			if (mirror)
			{
				mTransform.Translate(0f - (float)theSrcRect.mWidth, 0f);
				mTransform.Scale(0f - num, sy);
			}
			else
			{
				mTransform.Scale(num, sy);
			}
			mTransform.Translate(theDestRect.mX, theDestRect.mY);
			BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, mTransform.GetMatrix(), !fastStretch, 0f, 0f, false);
		}

		public override void SetGlobalAmbient(SexyFramework.Graphics.Color inColor)
		{
		}

		public void Init(int width, int height)
		{
			if (PlatformInfo.MonoGamePlatform == MonoGamePlatform.iOS ||
			    PlatformInfo.MonoGamePlatform == MonoGamePlatform.Android)
			{
				mScreenWidth = width;
				mScreenHeight = height;
			}
			else
			{
				mScreenWidth = 640;
				mScreenHeight = 1066;
				mDevice.PreferredBackBufferWidth = 480;
				mDevice.PreferredBackBufferHeight = 800;
			}
			mWidth = width;
			mHeight = height;
			mDevice.PreferMultiSampling = false;
			mDevice.SupportedOrientations = DisplayOrientation.Portrait;
			mDevice.ApplyChanges();
			mTmpVPCTBuffer = new VertexPositionColorTexture[4];
			mTmpVPCBuffer = new VertexPositionColor[4];
			try
			{
				mSexy2DEffect = WP7AppDriver.sWP7AppDriverInstance.mContentManager.Load<Effect>("effects/Sexy2D");
				mSexy2DWVP        = mSexy2DEffect.Parameters["WorldViewProj"];
				mSexy2DTex0       = mSexy2DEffect.Parameters["Tex0Texture"];
				mSexy2DTextured   = mSexy2DEffect.Techniques["Textured"];
				mSexy2DUntextured = mSexy2DEffect.Techniques["Untextured"];
			}
			catch
			{
				mSexy2DEffect = null;
			}
			mBasicEffect = new BasicEffect(mDevice.GraphicsDevice);
			mSpriteBatch = new SpriteBatch(mDevice.GraphicsDevice);
			mAdditiveState = StateCache.GetBlend(Blend.SourceAlpha, Blend.One, Blend.SourceAlpha, Blend.One);
			mNormalState   = StateCache.GetBlend(Blend.SourceAlpha, Blend.InverseSourceAlpha, Blend.SourceAlpha, Blend.InverseSourceAlpha);
			SetSamplerState(0, 0);
			SetBlend(Graphics3D.EBlendMode.BLEND_DEFAULT, Graphics3D.EBlendMode.BLEND_DEFAULT);
			SetDepthState(Graphics3D.ECompareFunc.COMPARE_NEVER, false);
			SetRasterizerState(0, 0);
			SetDefaultState(null, false);
			mStateMgr.mStateDirty = false;
			mCurDrawMode = 0;
			mScreenTarget = new RenderTarget2D(mDevice.GraphicsDevice, mScreenWidth, mScreenHeight, false, 0, DepthFormat.Depth24, 0,  RenderTargetUsage.PreserveContents);
		}

		public void SetDefaultState(Image theImage, bool isInScene)
		{
			int num = mWidth;
			int num2 = mHeight;
			if (theImage != null)
			{
				num = theImage.mWidth;
				num2 = theImage.mHeight;
			}

			SetViewport(0, 0, 480, 800, 0f, 1f);
			mStateMgr.SetProjectionTransform(Matrix.CreateOrthographicOffCenter(0f, num, num2, 0f, -1000f, 1000f));
			mStateMgr.SetViewTransform(Matrix.CreateLookAt(new Vector3(0f, 0f, 300f), Vector3.Zero, Vector3.Up));
			mStateMgr.SetWorldTransform(Matrix.Identity);
		}

		public override void SetViewport(int theX, int theY, int theWidth, int theHeight, float theMinZ, float theMaxZ)
		{
			if (PlatformInfo.MonoGamePlatform != MonoGamePlatform.iOS &&
			    PlatformInfo.MonoGamePlatform != MonoGamePlatform.Android)
			{
				mStateMgr.SetViewport(theX, theY, theWidth, theHeight, theMinZ, theMaxZ);
				mDevice.GraphicsDevice.Viewport = mStateMgr.mXNAViewPort;
			}
		}

		public void SetTextureDirect(int theStage, Texture2D theTexture)
		{
			mStateMgr.SetTexture(theStage, theTexture);
		}

		public void SetRenderState(SEXY3DRSS theRenderState, uint theValue)
		{
			GraphicsDevice gd = mDevice.GraphicsDevice;
			gd.DepthStencilState = StateCache.GetDepth(false, false, CompareFunction.Always);
			gd.RasterizerState   = StateCache.GetRaster(CullMode.None);
			gd.BlendState        = BlendState.AlphaBlend;
		}

		public void SetSamplerState(int theSampler, int theValue)
		{
			mStateMgr.SetSamplerState(theSampler, StateCache.GetSampler(TextureAddressMode.Clamp, TextureAddressMode.Clamp, TextureFilter.Linear));
		}

		public void SetRasterizerState(int fillMode, int cullMode)
		{
			mStateMgr.SetRasterizerState(StateCache.GetRaster((CullMode)cullMode, (FillMode)fillMode));
		}

		public override bool SetTexture(int inTextureIndex, Image inImage)
		{
			if (inImage == null)
			{
				mImage = null;
				SetTextureDirect(inTextureIndex, null);
				return true;
			}
			mImage = inImage;
			inImage = SetupAtlasState(inTextureIndex, inImage);

			XNATextureData xNATextureData = inImage.GetRenderData() as XNATextureData;
			MemoryImage inImage2 = inImage.AsMemoryImage();
			if (xNATextureData == null)
			{
				if (inImage2 == null)
				{
					return false;
				}
				if (!CreateImageRenderData(ref inImage2))
				{
					return false;
				}
				xNATextureData = inImage2?.GetRenderData() as XNATextureData;
			}

			if (xNATextureData == null || xNATextureData.mTextures[0] == null)
			{
				return false;
			}
			SetTextureDirect(inTextureIndex, xNATextureData.mTextures[0].mTexture);
			return true;
		}

		private void BltClipF(Image theImage, float theX, float theY, Rect theSrcRect, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode)
		{
			SexyTransform2D theTransform = new SexyTransform2D(false);
			theTransform.Translate(theX, theY);
			BltTransformed(theImage, theClipRect, theColor, theDrawMode, theSrcRect, theTransform, true, 0f, 0f, false);
		}

		public void BltNoClipF(Image theImage, float theX, float theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode, bool linearFilter)
		{
			if (mTransformStack.Count != 0)
			{
				BltClipF(theImage, theX, theY, theSrcRect, Rect.INVALIDATE_RECT, theColor, theDrawMode);
			}
			else if (PreDraw())
			{
				BltHelper(theImage, theX, theY, theSrcRect, theColor, theDrawMode, linearFilter);
			}
		}

		public void BltTransformHelper(Image theImage, Rect theClipRect, SexyFramework.Graphics.Color theColor, int theDrawMode, Rect theSrcRect, SexyTransform2D theTransform, bool linearFilter, float theX, float theY, bool center)
		{
			Image image = theImage;
			image.InitAtalasState();
			theImage = SetupAtlasState(0, theImage);
			int mX = theSrcRect.mX;
			int mY = theSrcRect.mY;
			int num = mX + theSrcRect.mWidth;
			int num2 = mY + theSrcRect.mHeight;
			int num3 = 0;
			int num4 = 0;
			float u = 0f;
			float v = 0f;
			float u2 = 0f;
			float v2 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			MemoryImage inImage = theImage as MemoryImage;
			if (!CreateImageRenderData(ref inImage))
			{
				return;
			}
			SetupDrawMode(theDrawMode);
			bool flag = false;
			if (theDrawMode == 0 && !inImage.mHasAlpha && theColor.mAlpha >= 255 && image.mWidth * image.mHeight > 40000)
			{
				SetBlend(Graphics3D.EBlendMode.BLEND_ONE, Graphics3D.EBlendMode.BLEND_ZERO);
				flag = true;
			}
			XNATextureData xNATextureData = inImage.GetRenderData() as XNATextureData;
			if (xNATextureData == null)
			{
				return;
			}
			if (center)
			{
				num5 = (float)(-theSrcRect.mWidth) / 2f;
				num6 = (float)(-theSrcRect.mHeight) / 2f;
			}
			int num7 = mY;
			float num8 = num6;
			if (mX >= num || mY >= num2)
			{
				return;
			}
			theTransform.Translate(theX, theY);
			float z = 0f;
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
			int num9 = mX;
			float num10 = num5;
			num3 = num - num9;
			num4 = num2 - num7;
			Texture2D texture = xNATextureData.GetTexture(image as MemoryImage, num9, num7, ref num3, ref num4, ref u, ref v, ref u2, ref v2);
			SetTextureDirect(0, texture);
			float num11 = num10;
			float num12 = num8;
			mTmpVPCTBuffer[0].Position.X = num11;
			mTmpVPCTBuffer[0].Position.Y = num12;
			mTmpVPCTBuffer[0].Position.Z = z;
			mTmpVPCTBuffer[0].Color = color;
			mTmpVPCTBuffer[0].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v;
			mTmpVPCTBuffer[1].Position.X = num11;
			mTmpVPCTBuffer[1].Position.Y = num12 + (float)num4;
			mTmpVPCTBuffer[1].Position.Z = z;
			mTmpVPCTBuffer[1].Color = color;
			mTmpVPCTBuffer[1].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v2;
			mTmpVPCTBuffer[2].Position.X = num11 + (float)num3;
			mTmpVPCTBuffer[2].Position.Y = num12;
			mTmpVPCTBuffer[2].Position.Z = z;
			mTmpVPCTBuffer[2].Color = color;
			mTmpVPCTBuffer[2].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v;
			mTmpVPCTBuffer[3].Position.X = num11 + (float)num3;
			mTmpVPCTBuffer[3].Position.Y = num12 + (float)num4;
			mTmpVPCTBuffer[3].Position.Z = z;
			mTmpVPCTBuffer[3].Color = color;
			mTmpVPCTBuffer[3].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v2;
			Matrix matrix = theTransform.mMatrix;
			for (int i = 0; i < 4; i++)
			{
				Vector3.Transform(ref mTmpVPCTBuffer[i].Position, ref matrix, out mTmpVPCTBuffer[i].Position);
			}
			Rect rect = theClipRect;
			bool flag2 = false;
			if (rect != Rect.INVALIDATE_RECT && (rect.mX != 0 || rect.mY != 0 || rect.mWidth != mWidth || rect.mHeight != mHeight))
			{
				SexyVector2 sexyVector = new SexyVector2(rect.mX, rect.mY);
				SexyVector2 sexyVector2 = new SexyVector2(rect.mX + rect.mWidth, rect.mY + rect.mHeight);
				for (int j = 0; j < 4; j++)
				{
					if (mTmpVPCTBuffer[j].Position.X < sexyVector.x || mTmpVPCTBuffer[j].Position.X >= sexyVector2.x || mTmpVPCTBuffer[j].Position.Y < sexyVector.y || mTmpVPCTBuffer[j].Position.Y >= sexyVector2.y)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag2)
			{
				VertexPositionColorTexture vertexPositionColorTexture = mTmpVPCTBuffer[2];
				mTmpVPCTBuffer[2] = mTmpVPCTBuffer[3];
				mTmpVPCTBuffer[3] = vertexPositionColorTexture;
				DrawPolyClipped(rect, mTmpVPCTBuffer);
			}
			else
			{
				BufferedDrawPrimitive(5, 2, mTmpVPCTBuffer, 32, mDefaultVertexFVF, Matrix.Identity);
			}
			if (flag)
			{
				SetBlend(Graphics3D.EBlendMode.BLEND_DEFAULT, Graphics3D.EBlendMode.BLEND_DEFAULT);
			}
		}

		public void BltHelper(Image theImage, float theX, float theY, Rect theSrcRect, SexyFramework.Graphics.Color theColor, int theDrawMode, bool linearFilter)
		{
			Image image = theImage;
			image.InitAtalasState();
			theImage = SetupAtlasState(0, theImage);
			MemoryImage inImage = theImage as MemoryImage;
			if (CreateImageRenderData(ref inImage))
			{
				SetupDrawMode(theDrawMode);
				XNATextureData xNATextureData = inImage.GetRenderData() as XNATextureData;
				if (xNATextureData == null)
				{
					return;
				}
				int mX = theSrcRect.mX;
				int mY = theSrcRect.mY;
				int num = mX + theSrcRect.mWidth;
				int num2 = mY + theSrcRect.mHeight;
				int num3 = 0;
				int num4 = 0;
				float u = 0f;
				float v = 0f;
				float u2 = 0f;
				float v2 = 0f;
				int num5 = mY;
				if (mX < num && mY < num2)
				{
					float z = 0f;
					Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(theColor.mRed, theColor.mGreen, theColor.mBlue, theColor.mAlpha);
					int num6 = mX;
					num3 = num - num6;
					num4 = num2 - num5;
					Texture2D texture = xNATextureData.GetTexture((MemoryImage)image, num6, num5, ref num3, ref num4, ref u, ref v, ref u2, ref v2);
					float num7 = theY;
					mTmpVPCTBuffer[0].Position.X = theX;
					mTmpVPCTBuffer[0].Position.Y = num7;
					mTmpVPCTBuffer[0].Position.Z = z;
					mTmpVPCTBuffer[0].Color = color;
					mTmpVPCTBuffer[0].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v;
					mTmpVPCTBuffer[1].Position.X = theX;
					mTmpVPCTBuffer[1].Position.Y = num7 + (float)num4;
					mTmpVPCTBuffer[1].Position.Z = z;
					mTmpVPCTBuffer[1].Color = color;
					mTmpVPCTBuffer[1].TextureCoordinate = image.mVectorBase + image.mVectorU * u + image.mVectorV * v2;
					mTmpVPCTBuffer[2].Position.X = theX + (float)num3;
					mTmpVPCTBuffer[2].Position.Y = num7;
					mTmpVPCTBuffer[2].Position.Z = z;
					mTmpVPCTBuffer[2].Color = color;
					mTmpVPCTBuffer[2].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v;
					mTmpVPCTBuffer[3].Position.X = theX + (float)num3;
					mTmpVPCTBuffer[3].Position.Y = num7 + (float)num4;
					mTmpVPCTBuffer[3].Position.Z = z;
					mTmpVPCTBuffer[3].Color = color;
					mTmpVPCTBuffer[3].TextureCoordinate = image.mVectorBase + image.mVectorU * u2 + image.mVectorV * v2;
					SetTextureDirect(0, texture);
					BufferedDrawPrimitive(5, 2, mTmpVPCTBuffer, 32, mDefaultVertexFVF, Matrix.Identity);
				}
			}
		}

		public bool PreDraw()
		{
			if (!mSceneBegun)
			{
				mSceneBegun = true;
				RenderStateManager.Context context = mStateMgr.GetContext();
				mStateMgr.SetContext(null);
				mStateMgr.RevertState();
				mStateMgr.ApplyContextDefaults();
				mStateMgr.PushState();
				if (!mStateMgr.CommitState())
				{
					mStateMgr.SetContext(context);
					return false;
				}
			}
			return true;
		}

		public void SetupDrawMode(int theDrawMode)
		{
			if (mStateMgr.mSrcBlendMode != Graphics3D.EBlendMode.BLEND_DEFAULT
			 || mStateMgr.mDestBlendMode != Graphics3D.EBlendMode.BLEND_DEFAULT)
			{
				return;
			}
			if (theDrawMode == 0)
			{
				mStateMgr.SetBlendStateState(mNormalState);
			}
			else
			{
				mStateMgr.SetBlendStateState(mAdditiveState);
			}
		}

		public Texture2D CreateTexture2D(int theWidth, int theHeight, PixelFormat theFormat, bool renderTarget, XNATextureData theTexData, XNATextureDataPiece[] theTexDataPiece)
		{
			GlobalMembers.gTotalGraphicsMemory += theWidth * theHeight * 4;
			SurfaceFormat xnaFormat = GetXnaFormat(theFormat);
			if (renderTarget)
			{
				return new RenderTarget2D(mDevice.GraphicsDevice, theWidth, theHeight, false, xnaFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
			}
			return new Texture2D(mDevice.GraphicsDevice, theWidth, theHeight, false, xnaFormat);
		}

		public Texture2D CreateTexture2DFromData(byte[] data)
		{
			MemoryStream stream = new MemoryStream(data);
			Texture2D texture2D = Texture2D.FromStream(mDevice.GraphicsDevice, stream);
			GlobalMembers.gTotalGraphicsMemory += texture2D.Width * texture2D.Height * 4;
			return texture2D;
		}

		public DeviceImage GetOptimizedImage(Texture2D texture, bool commitBits, bool allowTriReps)
		{
			GlobalMembers.gTotalGraphicsMemory += texture.Width * texture.Height * 4;
			DeviceImage deviceImage = new DeviceImage();
			deviceImage.mApp = GlobalMembers.gSexyAppBase;
			deviceImage.mFileName = texture.Name;
			deviceImage.mWidth = texture.Width;
			deviceImage.mHeight = texture.Height;
			deviceImage.mHasAlpha = true;
			XNATextureData xNATextureData = new XNATextureData(this);
			deviceImage.SetRenderData(xNATextureData);
			xNATextureData.mWidth = texture.Width;
			xNATextureData.mHeight = texture.Height;
			xNATextureData.mTexPieceWidth = texture.Width;
			xNATextureData.mTexPieceHeight = texture.Height;
			xNATextureData.mTexVecWidth = 1;
			xNATextureData.mTexVecHeight = 1;
			xNATextureData.mPixelFormat = GetSexyFormat(texture.Format);
			xNATextureData.mMaxTotalU = 1f;
			xNATextureData.mMaxTotalV = 1f;
			xNATextureData.mImageFlags = deviceImage.GetImageFlags();
			xNATextureData.mOptimizedLoad = true;
			xNATextureData.mTextures[0].mWidth = texture.Width;
			xNATextureData.mTextures[0].mHeight = texture.Height;
			xNATextureData.mTextures[0].mTexture = texture;
			return deviceImage;
		}

		public SurfaceFormat GetXnaFormat(PixelFormat theFormat)
		{
			switch (theFormat)
			{
			case PixelFormat.PixelFormat_X8R8G8B8:
				return SurfaceFormat.Color;
			case PixelFormat.PixelFormat_A8R8G8B8:
				return SurfaceFormat.Color;
			case PixelFormat.PixelFormat_R5G6B5:
				return SurfaceFormat.Bgr565;
			case PixelFormat.PixelFormat_A4R4G4B4:
				return SurfaceFormat.Bgra4444;
			default:
				return SurfaceFormat.Color;
			}
		}

		public PixelFormat GetSexyFormat(SurfaceFormat theFormat)
		{
			switch (theFormat)
			{
			case SurfaceFormat.Color:
				return PixelFormat.PixelFormat_A8R8G8B8;
			case SurfaceFormat.Bgr565:
				return PixelFormat.PixelFormat_R5G6B5;
			case SurfaceFormat.Bgra4444:
				return PixelFormat.PixelFormat_A4R4G4B4;
			default:
				return PixelFormat.PixelFormat_A8R8G8B8;
			}
		}

		public CompareFunction GetXNACompareFunc(Graphics3D.ECompareFunc func)
		{
			switch (func)
			{
			case Graphics3D.ECompareFunc.COMPARE_NEVER:
				return CompareFunction.Never;
			case Graphics3D.ECompareFunc.COMPARE_LESS:
				return CompareFunction.Less;
			case Graphics3D.ECompareFunc.COMPARE_EQUAL:
				return CompareFunction.Equal;
			case Graphics3D.ECompareFunc.COMPARE_LESSEQUAL:
				return CompareFunction.LessEqual;
			case Graphics3D.ECompareFunc.COMPARE_GREATER:
				return CompareFunction.Greater;
			case Graphics3D.ECompareFunc.COMPARE_NOTEQUAL:
				return CompareFunction.NotEqual;
			case Graphics3D.ECompareFunc.COMPARE_GREATEREQUAL:
				return CompareFunction.GreaterEqual;
			case Graphics3D.ECompareFunc.COMPARE_ALWAYS:
				return CompareFunction.Always;
			default:
				return CompareFunction.Never;
			}
		}

		public Blend GetXNABlendMode(Graphics3D.EBlendMode mode)
		{
			switch (mode)
			{
			case Graphics3D.EBlendMode.BLEND_DEFAULT:
				return Blend.SourceAlpha;
			case Graphics3D.EBlendMode.BLEND_ZERO:
				return Blend.Zero;
			case Graphics3D.EBlendMode.BLEND_ONE:
				return Blend.One;
			case Graphics3D.EBlendMode.BLEND_SRCCOLOR:
				return Blend.SourceColor;
			case Graphics3D.EBlendMode.BLEND_INVSRCCOLOR:
				return Blend.InverseSourceColor;
			case Graphics3D.EBlendMode.BLEND_SRCALPHA:
				return Blend.SourceAlpha;
			case Graphics3D.EBlendMode.BLEND_INVSRCALPHA:
				return Blend.InverseSourceAlpha;
			case Graphics3D.EBlendMode.BLEND_DESTCOLOR:
				return Blend.DestinationColor;
			case Graphics3D.EBlendMode.BLEND_INVDESTCOLOR:
				return Blend.InverseDestinationColor;
			case Graphics3D.EBlendMode.BLEND_SRCALPHASAT:
				return Blend.SourceAlphaSaturation;
			default:
				return Blend.One;
			}
		}

		public Matrix GetXNAMatrix(SexyMatrix4 mat)
		{
			return new Matrix(mat.m00, mat.m01, mat.m02, mat.m03, mat.m10, mat.m11, mat.m12, mat.m13, mat.m20, mat.m21, mat.m22, mat.m23, mat.m30, mat.m31, mat.m32, mat.m33);
		}

		public Matrix GetXNAMatrix(SexyMatrix3 mat)
		{
			return new Matrix(mat.m00, mat.m10, mat.m20, 0f, mat.m01, mat.m11, mat.m21, 0f, 0f, 0f, mat.m22, 0f, mat.m02, mat.m12, 0f, 1f);
		}

		public Microsoft.Xna.Framework.Color GetXNAColor(SexyFramework.Graphics.Color color)
		{
			return new Microsoft.Xna.Framework.Color(color.mRed, color.mGreen, color.mBlue, color.mAlpha);
		}

		public void CopyImageToTexture(ref Texture2D theTexture, int theTextureFormat, MemoryImage theImage, int offx, int offy, int texWidth, int texHeight, PixelFormat theFormat)
		{
			if (theTexture != null)
			{
				theTexture.SetData(theImage.GetBits());
			}
		}

		public void BufferedDrawIndexedPrimitive(int thePrimType, int thePrimCount, VertexPositionColorTexture[] theVertices, int theVertexSize, ulong theVertexFormat, Matrix transform)
		{
			CheckBatchAndCommit();
			int num = 0;
			switch (thePrimType)
			{
			case 4:
				while (thePrimCount > 0)
				{
					if (mBatchedTriangleIndex > mBatchedTriangleSize - 3)
					{
						DoCommitAllRenderState();
						FlushBufferedTriangles();
					}
					mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
					mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
					mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
					thePrimCount--;
				}
				break;
			case 5:
			{
				if (thePrimCount * 3 > mBatchedTriangleSize - mBatchedTriangleIndex)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				for (int k = 0; k < theVertices.Length; k++)
				{
					mBatchedTriangleBuffer[mBatchedTriangleIndex + k] = theVertices[k];
				}
				for (int l = 0; l < thePrimCount / 2; l++)
				{
					mBatchedIndexBuffer[mBatchedIndexIndex + l * 6] = (short)(mBatchedTriangleIndex + num * 4);
					mBatchedIndexBuffer[mBatchedIndexIndex + l * 6 + 1] = (short)(mBatchedTriangleIndex + num * 4 + 1);
					mBatchedIndexBuffer[mBatchedIndexIndex + l * 6 + 2] = (short)(mBatchedTriangleIndex + num * 4 + 2);
					mBatchedIndexBuffer[mBatchedIndexIndex + l * 6 + 3] = (short)(mBatchedTriangleIndex + num * 4 + 3);
					mBatchedIndexBuffer[mBatchedIndexIndex + l * 6 + 4] = (short)(mBatchedTriangleIndex + num * 4 + 2);
					mBatchedIndexBuffer[mBatchedIndexIndex + l * 6 + 5] = (short)(mBatchedTriangleIndex + num * 4 + 1);
				}
				mBatchedIndexIndex += thePrimCount * 3;
				mBatchedTriangleIndex += theVertices.Length;
				break;
			}
			case 6:
			{
				if (thePrimCount * 3 > mBatchedTriangleSize - mBatchedTriangleIndex)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				for (int i = 0; i < theVertices.Length; i++)
				{
					mBatchedTriangleBuffer[mBatchedTriangleIndex + i] = theVertices[i];
				}
				int num2 = mBatchedTriangleIndex;
				int num3 = num2;
				mBatchedTriangleIndex += theVertices.Length;
				mBatchedIndexBuffer[mBatchedIndexIndex++] = (short)num2;
				mBatchedIndexBuffer[mBatchedIndexIndex++] = (short)(++num3);
				mBatchedIndexBuffer[mBatchedIndexIndex++] = (short)(++num3);
				for (int j = 0; j < thePrimCount - 1; j++)
				{
					mBatchedIndexBuffer[mBatchedIndexIndex + j * 3] = (short)num2;
					mBatchedIndexBuffer[mBatchedIndexIndex + j * 3 + 1] = (short)(num3 - 1);
					mBatchedIndexBuffer[mBatchedIndexIndex + j * 3 + 2] = (short)num3;
					num3++;
				}
				mBatchedIndexIndex += (thePrimCount - 1) * 3;
				break;
			}
			}
			if (mBatchedTriangleIndex + 3 > mBatchedTriangleSize)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}
		}

		public void BufferedDrawPrimitive(int thePrimType, int thePrimCount, VertexPositionColorTexture[] theVertices, int theVertexSize, ulong theVertexFormat, Matrix transform)
		{
			CheckBatchAndCommit();
			int num = 0;
			switch (thePrimType)
			{
			case 4:
				while (thePrimCount > 0)
				{
					if (mBatchedTriangleIndex > mBatchedTriangleSize - 3)
					{
						DoCommitAllRenderState();
						FlushBufferedTriangles();
					}
					mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
					mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
					mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
					thePrimCount--;
				}
				break;
			case 5:
				if (thePrimCount * 3 > mBatchedTriangleSize - mBatchedTriangleIndex)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
				mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
				mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
				for (thePrimCount--; thePrimCount > 0; thePrimCount--)
				{
					mBatchedTriangleBuffer[mBatchedTriangleIndex] = mBatchedTriangleBuffer[mBatchedTriangleIndex - 2];
					mBatchedTriangleBuffer[mBatchedTriangleIndex + 1] = mBatchedTriangleBuffer[mBatchedTriangleIndex - 1];
					mBatchedTriangleBuffer[mBatchedTriangleIndex + 2] = theVertices[num++];
					mBatchedTriangleIndex += 3;
				}
				break;
			case 6:
			{
				if (thePrimCount * 3 > mBatchedTriangleSize - mBatchedTriangleIndex)
				{
					DoCommitAllRenderState();
					FlushBufferedTriangles();
				}
				int num2 = mBatchedTriangleIndex;
				mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
				mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
				mBatchedTriangleBuffer[mBatchedTriangleIndex++] = theVertices[num++];
				for (thePrimCount--; thePrimCount > 0; thePrimCount--)
				{
					mBatchedTriangleBuffer[mBatchedTriangleIndex] = mBatchedTriangleBuffer[num2];
					mBatchedTriangleBuffer[mBatchedTriangleIndex + 1] = mBatchedTriangleBuffer[mBatchedTriangleIndex - 1];
					mBatchedTriangleBuffer[mBatchedTriangleIndex + 2] = theVertices[num++];
					mBatchedTriangleIndex += 3;
				}
				break;
			}
			}
		}

		public void DrawPrimitiveInternal<T>(int inPrimType, int inPrimCount, T[] inVertData, ulong inVertStride, ulong inVertFormat, bool inDoCommit, Matrix transform) where T : struct, IVertexType
		{
			int num = 0;
			switch (inPrimType)
			{
			case 4:
				num = inPrimCount * 3;
				break;
			case 5:
			case 6:
				num = inPrimCount + 2;
				break;
			case 3:
				num = inPrimCount + 1;
				break;
			}
			if (num == 0)
			{
				return;
			}
			if (inDoCommit)
			{
				CheckBatchAndCommit();
				DoCommitAllRenderState();
			}
			PrimitiveType primitiveType = PrimitiveType.TriangleList;
			switch (inPrimType)
			{
			case 4:
				primitiveType = PrimitiveType.TriangleList;
				break;
			case 5:
				primitiveType = PrimitiveType.TriangleStrip;
				break;
			case 6:
				return;
			case 3:
				primitiveType = PrimitiveType.LineStrip;
				break;
			}

			if (mStateMgr.mActiveEffects.Count == 0)
			{
				Effect baseFx = mSexy2DEffect ?? mBasicEffect;
				foreach (EffectPass pass in baseFx.CurrentTechnique.Passes)
				{
					pass.Apply();
					mDevice.GraphicsDevice.DrawUserPrimitives(primitiveType, inVertData, 0, inPrimCount);
				}
			}
			else
			{
				foreach (XNARenderEffect aEffect in mStateMgr.mActiveEffects)
				{
                    DoCommitEffectRenderState(aEffect);
                    aEffect.MG_ApplyPass();
                    mDevice.GraphicsDevice.DrawUserPrimitives(primitiveType, inVertData, 0, inPrimCount);
                }
				//XNARenderEffect aEffect = mStateMgr.mActiveEffects[mStateMgr.mActiveEffects.Count - 1];
				//DoCommitEffectRenderState(aEffect);
				//aEffect.MG_ApplyPass();
				//mDevice.GraphicsDevice.DrawUserPrimitives(primitiveType, inVertData, 0, inPrimCount);
			}
		}

		public void DrawIndexPrimitiveInternal<T>(int inPrimType, int inPrimCount, T[] inVertData, ulong inVertStride, ulong inVertFormat, bool inDoCommit, Matrix transform) where T : struct, IVertexType
		{
			if (inDoCommit)
			{
				CheckBatchAndCommit();
				DoCommitAllRenderState();
			}
			PrimitiveType primitiveType = PrimitiveType.TriangleList;
			switch (inPrimType)
			{
			case 4:
				primitiveType = PrimitiveType.TriangleList;
				break;
			case 5:
				primitiveType = PrimitiveType.TriangleStrip;
				break;
			case 6:
				return;
			case 3:
				primitiveType = PrimitiveType.LineStrip;
				break;
			}

			Effect baseFx2 = (Effect)mSexy2DEffect ?? mBasicEffect;
			foreach (EffectPass pass in baseFx2.CurrentTechnique.Passes)
			{
				pass.Apply();
			}
			if (mStateMgr.mActiveEffects.Count == 0)
			{
				foreach (EffectPass pass in baseFx2.CurrentTechnique.Passes)
				{
					pass.Apply();
					mDevice.GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, inVertData, 0,
						mBatchedTriangleIndex, mBatchedIndexBuffer, 0, inPrimCount);
				}
			}
			else
			{
                foreach (XNARenderEffect aEffect in mStateMgr.mActiveEffects)
                {
                    DoCommitEffectRenderState(aEffect);
                    aEffect.MG_ApplyPass();
                    mDevice.GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, inVertData, 0, mBatchedTriangleIndex, mBatchedIndexBuffer, 0, inPrimCount);
                }
                //XNARenderEffect aEffect = mStateMgr.mActiveEffects[mStateMgr.mActiveEffects.Count - 1];
                //DoCommitEffectRenderState(aEffect);
                //aEffect.MG_ApplyPass();
                //mDevice.GraphicsDevice.DrawUserIndexedPrimitives(primitiveType, inVertData, 0, mBatchedTriangleIndex, mBatchedIndexBuffer, 0, inPrimCount);
            }
		}

		public VertexBuffer InternalCreateVertexBuffer(int inCount, VertexDeclaration vDec, BufferUsage usage)
		{
			return new VertexBuffer(mDevice.GraphicsDevice, vDec, inCount, usage);
		}

		public IndexBuffer InternalCreateIndexBuffer(int indexCount, IndexElementSize size, BufferUsage usage)
		{
			return new IndexBuffer(mDevice.GraphicsDevice, size, indexCount, usage);
		}

		public override Image SwapScreenImage(ref DeviceImage ioSrcImage, ref RenderSurface ioSrcSurface, uint flags)
		{
			if (mBatchedTriangleIndex > 0)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}

			RenderTarget2D oldTarget = mScreenTarget;
			RenderTarget2D newTarget = new RenderTarget2D(mDevice.GraphicsDevice, oldTarget.Width, oldTarget.Height,
				false, 0, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

			XNATextureData srcData = ioSrcImage.GetRenderData() as XNATextureData;
			Texture2D prevSrcTex = (srcData != null && srcData.mTextures != null && srcData.mTextures[0] != null)
				? srcData.mTextures[0].mTexture : null;
			if (srcData != null) srcData.mTextures[0].mTexture = oldTarget;
			if (prevSrcTex != null && prevSrcTex != oldTarget)
			{
				prevSrcTex.Dispose();
			}

			mDevice.GraphicsDevice.SetRenderTarget(newTarget);
			mDevice.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

			mScreenTarget = newTarget;
			return ioSrcImage;
		}

		public override void CopyScreenImage(DeviceImage ioDstImage, uint flags)
		{
			throw new NotImplementedException();
		}

		public override RenderEffect GetEffect(RenderEffectDefinition inDefinition)
		{
			if (inDefinition == null || inDefinition.mEffect == null)
			{
				return null;
			}
			return new XNARenderEffect(inDefinition, this);
		}

		public void SwitchToScreenImage()
		{
			if (mCurrentContex == null || mCurrentContex.GetPointer() == null)
			{
				mDevice.GraphicsDevice.SetRenderTarget(mScreenTarget);
				mDevice.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Microsoft.Xna.Framework.Color.Black, 1f, 0);
			}
		}

		public void PresentScreenImage()
		{
			if (mBatchedTriangleIndex > 0)
			{
				DoCommitAllRenderState();
				FlushBufferedTriangles();
			}

			if (mCurrentContex == null || mCurrentContex.GetPointer() == null)
			{
				mDevice.GraphicsDevice.SetRenderTarget(null);

				Rectangle aRenderRect = new Rectangle(0, 0, mDevice.GraphicsDevice.Viewport.Width, mDevice.GraphicsDevice.Viewport.Height);
				// We render with the predetermined ratio and center it for mobile
				if (PlatformInfo.MonoGamePlatform == MonoGamePlatform.iOS ||
				    PlatformInfo.MonoGamePlatform == MonoGamePlatform.Android)
				{
					float targetAspect = (float)mDevice.GraphicsDevice.Viewport.Width / (float)mDevice.GraphicsDevice.Viewport.Height;
					float baseAspect = (float)mScreenTarget.Width / (float)mScreenTarget.Height;

					int newWidth, newHeight;
					int offsetX = 0, offsetY = 0;

					if (targetAspect > baseAspect)
					{
						newHeight = mDevice.GraphicsDevice.Viewport.Height;
						newWidth = (int)(newHeight * baseAspect);
						offsetX = (mDevice.GraphicsDevice.Viewport.Width - newWidth) / 2;
						offsetY = 0;
					}
					else
					{
						newWidth = mDevice.GraphicsDevice.Viewport.Width;
						newHeight = (int)(newWidth / baseAspect);
						offsetX = 0;
						offsetY = (mDevice.GraphicsDevice.Viewport.Height - newHeight) / 2;
					}

					aRenderRect = new Rectangle(offsetX, offsetY, newWidth, newHeight);
				}
				
				mSpriteBatch.Begin();
				mSpriteBatch.Draw(mScreenTarget, aRenderRect, Microsoft.Xna.Framework.Color.White);
				mSpriteBatch.End();
			}
		}
	}
}
