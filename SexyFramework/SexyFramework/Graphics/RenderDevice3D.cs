using System;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public abstract class RenderDevice3D : RenderDevice
	{
		public enum EFlushFlags
		{
			FLUSHF_BufferedTris = 1,
			FLUSHF_CurrentScene = 2,
			FLUSHF_ManagedResources_Immediate = 4,
			FLUSHF_ManagedResources_OnPresent = 8,
			FLUSHF_BufferedState = 0x10
		}

		public enum ECapsFlags
		{
			CAPF_SingleImageTexture = 1,
			CAPF_PixelShaders = 2,
			CAPF_VertexShaders = 4,
			CAPF_ImageRenderTargets = 8,
			CAPF_AutoWindowedVSync = 0x10,
			CAPF_CubeMaps = 0x20,
			CAPF_VolumeMaps = 0x40,
			CAPF_CopyScreenImage = 0x80,
			CAPF_LastLockScreenImage = 0x100
		}

		public enum EInfoString
		{
			INFOSTRING_Adapter,
			INFOSTRING_DrvProductVersion,
			INFOSTRING_DisplayMode,
			INFOSTRING_BackBuffer,
			INFOSTRING_TextureMemory,
			INFOSTRING_DrvResourceManager,
			INFOSTRING_DrvProductFeatures
		}

		public delegate int FBltFilter(IntPtr theContext, int thePrimType, uint thePrimCount, SexyVertex2D theVertices, int theVertexSize, Rect[] theClipRect);

		public delegate int FDrawPrimFilter(IntPtr theContext, int thePrimType, uint thePrimCount, SexyVertex2D theVertices, int theVertexSize);

		public int Flush()
		{
			return Flush(2u);
		}

		public abstract int Flush(uint inFlushFlags);

		public abstract int Present(Rect theSrcRect, Rect theDestRect);

		public abstract uint GetCapsFlags();

		public bool SupportsPixelShaders()
		{
			return (GetCapsFlags() & 2) != 0;
		}

		public bool SupportsVertexShaders()
		{
			return (GetCapsFlags() & 4) != 0;
		}

		public bool SupportsCubeMaps()
		{
			return (GetCapsFlags() & 0x20) != 0;
		}

		public bool SupportsVolumeMaps()
		{
			return (GetCapsFlags() & 0x40) != 0;
		}

		public bool SupportsImageRenderTargets()
		{
			return (GetCapsFlags() & 8) != 0;
		}

		public abstract uint GetMaxTextureStages();

		public abstract string GetInfoString(EInfoString inInfoStr);

		public abstract void GetBackBufferDimensions(ref uint outWidth, ref uint outHeight);

		public abstract int SceneBegun();

		public abstract bool CreateImageRenderData(ref MemoryImage inImage);

		public abstract void RemoveImageRenderData(MemoryImage inImage);

		public abstract int RecoverImageBitsFromRenderData(MemoryImage inImage);

		public abstract int GetTextureMemorySize(MemoryImage theImage);

		public abstract PixelFormat GetTextureFormat(MemoryImage theImage);

		public abstract Image SwapScreenImage(ref DeviceImage ioSrcImage, ref RenderSurface ioSrcSurface, uint flags);

		public abstract void CopyScreenImage(DeviceImage ioDstImage, uint flags);

		public abstract void AdjustVertexUVsEx(uint theVertexFormat, SexyVertex[] theVertices, int theVertexCount, int theVertexSize);

		public void DrawPrimitiveEx(uint theVertexFormat, Graphics3D.EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, Color theColor, int theDrawMode, float tx, float ty, bool blend)
		{
			DrawPrimitiveEx(theVertexFormat, thePrimitiveType, theVertices, thePrimitiveCount, theColor, theDrawMode, tx, ty, blend, 0u);
		}

		public void DrawPrimitiveEx(uint theVertexFormat, Graphics3D.EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, Color theColor, int theDrawMode, float tx, float ty)
		{
			DrawPrimitiveEx(theVertexFormat, thePrimitiveType, theVertices, thePrimitiveCount, theColor, theDrawMode, tx, ty, true, 0u);
		}

		public void DrawPrimitiveEx(uint theVertexFormat, Graphics3D.EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, Color theColor, int theDrawMode, float tx)
		{
			DrawPrimitiveEx(theVertexFormat, thePrimitiveType, theVertices, thePrimitiveCount, theColor, theDrawMode, tx, 0f, true, 0u);
		}

		public void DrawPrimitiveEx(uint theVertexFormat, Graphics3D.EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, Color theColor, int theDrawMode)
		{
			DrawPrimitiveEx(theVertexFormat, thePrimitiveType, theVertices, thePrimitiveCount, theColor, theDrawMode, 0f, 0f, true, 0u);
		}

		public abstract void DrawPrimitiveEx(uint theVertexFormat, Graphics3D.EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, Color theColor, int theDrawMode, float tx, float ty, bool blend, uint theFlags);

		public abstract void SetBltDepth(float inDepth);

		public void PushTransform(SexyTransform2D theTransform)
		{
			PushTransform(theTransform, true);
		}

		public abstract void PushTransform(SexyTransform2D theTransform, bool concatenate);

		public abstract void PopTransform();

		public abstract void PopTransform(ref SexyTransform2D theTransform);

		public abstract void ClearColorBuffer(Color inColor);

		public abstract void ClearDepthBuffer();

		public virtual void Clear(Color theColor)
		{
		}

		public abstract void SetDepthState(Graphics3D.ECompareFunc inDepthTestFunc, bool inDepthWriteEnabled);

		public abstract void SetAlphaTest(Graphics3D.ECompareFunc inAlphaTestFunc, int inRefAlpha);

		public abstract void SetColorWriteState(int inWriteRedEnabled, int inWriteGreenEnabled, int inWriteBlueEnabled, int inWriteAlphaEnabled);

		public abstract void SetWireframe(int inWireframe);

		public abstract void SetBlend(Graphics3D.EBlendMode inSrcBlend, Graphics3D.EBlendMode inDestBlend);

		public abstract void SetBackfaceCulling(int inCullClockwise, int inCullCounterClockwise);

		public abstract void SetLightingEnabled(int inLightingEnabled);

		public abstract void SetLightEnabled(int inLightIndex, int inEnabled);

		public abstract void SetGlobalAmbient(Color inColor);

		public void SetMaterialAmbient(Color inColor)
		{
			SetMaterialAmbient(inColor, -1);
		}

		public abstract void SetMaterialAmbient(Color inColor, int inVertexColorComponent);

		public void SetMaterialDiffuse(Color inColor)
		{
			SetMaterialDiffuse(inColor, -1);
		}

		public abstract void SetMaterialDiffuse(Color inColor, int inVertexColorComponent);

		public void SetMaterialSpecular(Color inColor, int inVertexColorComponent)
		{
			SetMaterialSpecular(inColor, inVertexColorComponent, 0f);
		}

		public void SetMaterialSpecular(Color inColor)
		{
			SetMaterialSpecular(inColor, -1, 0f);
		}

		public abstract void SetMaterialSpecular(Color inColor, int inVertexColorComponent, float inPower);

		public void SetMaterialEmissive(Color inColor)
		{
			SetMaterialEmissive(inColor, -1);
		}

		public abstract void SetMaterialEmissive(Color inColor, int inVertexColorComponent);

		public abstract void SetWorldTransform(SexyMatrix4 inMatrix);

		public abstract void SetViewTransform(SexyMatrix4 inMatrix);

		public abstract void SetProjectionTransform(SexyMatrix4 inMatrix);

		public void SetTextureTransform(int inTextureIndex, SexyMatrix4 inMatrix)
		{
			SetTextureTransform(inTextureIndex, inMatrix, 2);
		}

		public abstract void SetTextureTransform(int inTextureIndex, SexyMatrix4 inMatrix, int inNumDimensions);

		public abstract void SetViewport(int theX, int theY, int theWidth, int theHeight, float theMinZ, float theMaxZ);

		public abstract void SetRenderRect(int theX, int theY, int theWidth, int theHeight);

		public abstract bool SetTexture(int inTextureIndex, Image inImage);

		public abstract void SetTextureWrap(int inTextureIndex, bool inWrapU, bool inWrapV);

		public void SetTextureLinearFilter(int inTextureIndex)
		{
			SetTextureLinearFilter(inTextureIndex, true);
		}

		public abstract void SetTextureLinearFilter(int inTextureIndex, bool inLinear);

		public void SetTextureCoordSource(int inTextureIndex, int inUVComponent)
		{
			SetTextureCoordSource(inTextureIndex, inUVComponent, Graphics3D.ETexCoordGen.TEXCOORDGEN_NONE);
		}

		public abstract void SetTextureCoordSource(int inTextureIndex, int inUVComponent, Graphics3D.ETexCoordGen inTexGen);

		public abstract void SetTextureFactor(int inTextureFactor);

		public abstract RenderEffect GetEffect(RenderEffectDefinition inDefinition);

		public virtual bool ReloadEffects()
		{
			return false;
		}

		public virtual bool ReloadEffects(int inDebug)
		{
			return ReloadEffects();
		}

		public virtual void SetBltFilter(FBltFilter inFilter, IntPtr inContext)
		{
		}

		public virtual void SetDrawPrimFilter(FDrawPrimFilter inFilter, IntPtr inContext)
		{
		}

		public virtual bool LoadMesh(Mesh theMesh)
		{
			throw new NotImplementedException();
		}

		public virtual void RenderMesh(Mesh theMesh, SexyMatrix4 theMatrix, Color theColor)
		{
			RenderMesh(theMesh, theMatrix, theColor, true);
		}

		public virtual void RenderMesh(Mesh theMesh, SexyMatrix4 theMatrix)
		{
			RenderMesh(theMesh, theMatrix, Color.White, true);
		}

		public virtual void RenderMesh(Mesh theMesh, SexyMatrix4 theMatrix, Color theColor, bool doSetup)
		{
			throw new NotImplementedException();
		}
	}
}
