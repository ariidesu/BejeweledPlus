using System;
using System.Collections.Generic;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class Graphics3D
	{
		public enum EBlendMode
		{
			BLEND_ZERO = 1,
			BLEND_ONE = 2,
			BLEND_SRCCOLOR = 3,
			BLEND_INVSRCCOLOR = 4,
			BLEND_SRCALPHA = 5,
			BLEND_INVSRCALPHA = 6,
			BLEND_DESTCOLOR = 9,
			BLEND_INVDESTCOLOR = 10,
			BLEND_SRCALPHASAT = 11,
			BLEND_DEFAULT = 65535
		}

		public enum ECompareFunc
		{
			COMPARE_NEVER = 1,
			COMPARE_LESS,
			COMPARE_EQUAL,
			COMPARE_LESSEQUAL,
			COMPARE_GREATER,
			COMPARE_NOTEQUAL,
			COMPARE_GREATEREQUAL,
			COMPARE_ALWAYS
		}

		public enum ETexCoordGen
		{
			TEXCOORDGEN_NONE,
			TEXCOORDGEN_CAMERASPACENORMAL,
			TEXCOORDGEN_CAMERASPACEPOSITION,
			TEXCOORDGEN_CAMERASPACEREFLECTIONVECTOR
		}

		public enum EPrimitiveType
		{
			PT_PointList = 1,
			PT_LineList,
			PT_LineStrip,
			PT_TriangleList,
			PT_TriangleStrip,
			PT_TriangleFan
		}

		public enum EDrawPrimitiveFlags
		{
			DPF_NoAdjustUVs = 1,
			DPF_NoHalfPixelOffset = 2,
			DPF_DiscardVerts = 4
		}

		public enum EMaskMode
		{
			MASKMODE_NONE,
			MASKMODE_WRITE_MASKONLY,
			MASKMODE_WRITE_MASKANDCOLOR,
			MASKMODE_TEST_INSIDE,
			MASKMODE_TEST_OUTSIDE
		}

		public class LightColors
		{
			public Color mDiffuse = default(Color);

			public Color mSpecular = default(Color);

			public Color mAmbient = default(Color);

			public float mAutoScale;

			public LightColors()
			{
				mDiffuse = new Color(Color.White);
				mSpecular = new Color(Color.Black);
				mAmbient = new Color(Color.Black);
				mAutoScale = 1f;
			}
		}

		public abstract class Camera
		{
			protected SexyCoords3 mCoords = new SexyCoords3();

			protected float mZNear;

			protected float mZFar;

			public Camera()
			{
				mZNear = 1f;
				mZFar = 10000f;
			}

			public SexyCoords3 GetCoords()
			{
				return mCoords;
			}

			public void SetCoords(SexyCoords3 inCoords)
			{
				mCoords = inCoords;
			}

			public float GetZNear()
			{
				return mZNear;
			}

			public float GetZFar()
			{
				return mZFar;
			}

			public void GetViewMatrix(SexyMatrix4 outM)
			{
			}

			public abstract void GetProjectionMatrix(SexyMatrix4 outM);

			public abstract bool IsOrtho();

			public abstract bool IsPerspective();
		}

		public class PerspectiveCamera : Camera
		{
			protected SexyVector3 mProjS = default(SexyVector3);

			protected float mProjT;

			public PerspectiveCamera()
			{
				mProjS = new SexyVector3(0f, 0f, 0f);
				mProjT = 0f;
			}

			public PerspectiveCamera(float inFovDegrees, float inAspectRatio, float inZNear)
				: this(inFovDegrees, inAspectRatio, inZNear, 10000f)
			{
			}

			public PerspectiveCamera(float inFovDegrees, float inAspectRatio)
				: this(inFovDegrees, inAspectRatio, 1f, 10000f)
			{
			}

			public PerspectiveCamera(float inFovDegrees, float inAspectRatio, float inZNear, float inZFar)
			{
				Init(inFovDegrees, inAspectRatio, inZNear, inZFar);
			}

			public void Init(float inFovDegrees, float inAspectRatio, float inZNear)
			{
				Init(inFovDegrees, inAspectRatio, inZNear, 10000f);
			}

			public void Init(float inFovDegrees, float inAspectRatio)
			{
				Init(inFovDegrees, inAspectRatio, 1f, 10000f);
			}

			public void Init(float inFovDegrees, float inAspectRatio, float inZNear, float inZFar)
			{
				float num = SexyMath.DegToRad(inFovDegrees * 0.5f);
				float num2 = num / inAspectRatio;
				mProjS.y = (float)(Math.Cos(num2) / Math.Sin(num2));
				mProjS.x = mProjS.y / inAspectRatio;
				mProjS.z = inZFar / (inZFar - inZNear);
				mProjT = (0f - mProjS.z) * inZNear;
				mZNear = inZNear;
				mZFar = inZFar;
			}

			public override void GetProjectionMatrix(SexyMatrix4 outM)
			{
			}

			public override bool IsOrtho()
			{
				return false;
			}

			public override bool IsPerspective()
			{
				return true;
			}

			public SexyVector3 EyeToScreen(SexyVector3 inEyePos)
			{
				SexyVector3 result = default(SexyVector3);
				float num = 0f - inEyePos.z;
				result.x = inEyePos.x * mProjS.x / num;
				result.y = inEyePos.y * mProjS.y / num;
				result.z = (num * mProjS.z + mProjT) / mZFar;
				result.x = result.x * 0.5f + 0.5f;
				result.y = result.y * -0.5f + 0.5f;
				return result;
			}

			public SexyVector3 ScreenToEye(SexyVector3 inScreenPos)
			{
				float num = (inScreenPos.x - 0.5f) * 2f;
				float num2 = (inScreenPos.y - 0.5f) * -2f;
				SexyVector3 sexyVector = new SexyVector3(num * mZNear / mProjS.x, num2 * mZNear / mProjS.y, 0f - mZNear);
				SexyVector3 sexyVector2 = new SexyVector3(num * mZFar / mProjS.x, num2 * mZFar / mProjS.y, 0f - mZFar);
				return sexyVector + (sexyVector2 - sexyVector) * inScreenPos.z;
			}
		}

		public class OffCenterPerspectiveCamera : Camera
		{
			protected SexyVector3 mProjS = default(SexyVector3);

			protected float mProjT;

			protected float mLeft;

			protected float mRight;

			protected float mTop;

			protected float mBottom;

			public OffCenterPerspectiveCamera()
			{
				mProjS = new SexyVector3(0f, 0f, 0f);
				mProjT = 0f;
			}

			public OffCenterPerspectiveCamera(float inFovDegrees, float inAspectRatio, float inOffsetX, float inOffsetY, float inZNear)
				: this(inFovDegrees, inAspectRatio, inOffsetX, inOffsetY, inZNear, 10000f)
			{
			}

			public OffCenterPerspectiveCamera(float inFovDegrees, float inAspectRatio, float inOffsetX, float inOffsetY)
				: this(inFovDegrees, inAspectRatio, inOffsetX, inOffsetY, 1f, 10000f)
			{
			}

			public OffCenterPerspectiveCamera(float inFovDegrees, float inAspectRatio, float inOffsetX, float inOffsetY, float inZNear, float inZFar)
			{
				Init(inFovDegrees, inAspectRatio, inOffsetX, inOffsetY, inZNear, inZFar);
			}

			public void Init(float inFovDegrees, float inAspectRatio, float inOffsetX, float inOffsetY, float inZNear)
			{
				Init(inFovDegrees, inAspectRatio, inOffsetX, inOffsetY, inZNear, 10000f);
			}

			public void Init(float inFovDegrees, float inAspectRatio, float inOffsetX, float inOffsetY)
			{
				Init(inFovDegrees, inAspectRatio, inOffsetX, inOffsetY, 1f, 10000f);
			}

			public void Init(float inFovDegrees, float inAspectRatio, float inOffsetX, float inOffsetY, float inZNear, float inZFar)
			{
				float num = SexyMath.DegToRad(inFovDegrees * 0.5f);
				float num2 = num / inAspectRatio;
				float num3 = (float)(Math.Cos(num2) / Math.Sin(num2));
				float num4 = num3 / inAspectRatio;
				float num5 = inZNear / num4;
				float num6 = inZNear / num3;
				mLeft = inOffsetX - num5;
				mRight = inOffsetX + num5;
				mTop = inOffsetY + num6;
				mBottom = inOffsetY - num6;
				mProjS.y = num3;
				mProjS.x = num4;
				mProjS.z = inZFar / (inZFar - inZNear);
				mProjT = (0f - mProjS.z) * inZNear;
				mZNear = inZNear;
				mZFar = inZFar;
			}

			public override void GetProjectionMatrix(SexyMatrix4 outM)
			{
			}

			public override bool IsOrtho()
			{
				return false;
			}

			public override bool IsPerspective()
			{
				return true;
			}

			public SexyVector3 EyeToScreen(SexyVector3 inEyePos)
			{
				SexyVector3 result = default(SexyVector3);
				float num = 0f - inEyePos.z;
				result.x = inEyePos.x * mProjS.x / num;
				result.y = inEyePos.y * mProjS.y / num;
				result.z = (num * mProjS.z + mProjT) / mZFar;
				result.x = result.x * 0.5f + 0.5f;
				result.y = result.y * -0.5f + 0.5f;
				return result;
			}

			public SexyVector3 ScreenToEye(SexyVector3 inScreenPos)
			{
				float num = (inScreenPos.x - 0.5f) * 2f;
				float num2 = (inScreenPos.y - 0.5f) * -2f;
				SexyVector3 sexyVector = new SexyVector3(num * mZNear / mProjS.x, num2 * mZNear / mProjS.y, 0f - mZNear);
				SexyVector3 sexyVector2 = new SexyVector3(num * mZFar / mProjS.x, num2 * mZFar / mProjS.y, 0f - mZFar);
				return sexyVector + (sexyVector2 - sexyVector) * inScreenPos.z;
			}
		}

		public class OrthoCamera : Camera
		{
			protected SexyVector3 mProjS = default(SexyVector3);

			protected float mProjT;

			protected float mWidth;

			protected float mHeight;

			public OrthoCamera()
			{
				mProjS = new SexyVector3(0f, 0f, 0f);
				mProjT = 0f;
				mWidth = 0f;
				mHeight = 0f;
			}

			public OrthoCamera(float inWidth, float inHeight, float inZNear)
				: this(inWidth, inHeight, inZNear, 10000f)
			{
			}

			public OrthoCamera(float inWidth, float inHeight)
				: this(inWidth, inHeight, 1f, 10000f)
			{
			}

			public OrthoCamera(float inWidth, float inHeight, float inZNear, float inZFar)
			{
				Init(inWidth, inHeight, inZNear, inZFar);
			}

			public void Init(float inWidth, float inHeight, float inZNear)
			{
				Init(inWidth, inHeight, inZNear, 10000f);
			}

			public void Init(float inWidth, float inHeight)
			{
				Init(inWidth, inHeight, 1f, 10000f);
			}

			public void Init(float inWidth, float inHeight, float inZNear, float inZFar)
			{
				mWidth = inWidth;
				mHeight = inHeight;
				mProjS.y = 2f / mHeight;
				mProjS.x = 2f / mWidth;
				mProjS.z = 1f / (inZFar - inZNear);
				mProjT = (0f - mProjS.z) * inZNear;
				mZNear = inZNear;
				mZFar = inZFar;
			}

			public override void GetProjectionMatrix(SexyMatrix4 outM)
			{
			}

			public override bool IsOrtho()
			{
				return true;
			}

			public override bool IsPerspective()
			{
				return false;
			}

			public SexyVector3 EyeToScreen(SexyVector3 inEyePos)
			{
				SexyVector3 result = default(SexyVector3);
				result.x = inEyePos.x * mProjS.x;
				result.y = inEyePos.y * mProjS.y;
				result.z = (mProjS.z + mProjT) / mZFar;
				result.x = result.x * 0.5f + 0.5f;
				result.y = result.y * -0.5f + 0.5f;
				return result;
			}

			public SexyVector3 ScreenToEye(SexyVector3 inScreenPos)
			{
				float num = (inScreenPos.x - 0.5f) * 2f;
				float num2 = (inScreenPos.y - 0.5f) * -2f;
				SexyVector3 sexyVector = new SexyVector3(num / mProjS.x, num2 / mProjS.y, 0f - mZNear);
				SexyVector3 sexyVector2 = new SexyVector3(sexyVector.x, sexyVector.y, 0f - mZFar);
				return sexyVector + (sexyVector2 - sexyVector) * inScreenPos.z;
			}
		}

		public interface Spline
		{
			SexyVector3 Evaluate(float inTime);
		}

		public class CatmullRomSpline : Spline
		{
			public List<SexyVector3> mPoints = new List<SexyVector3>();

			public CatmullRomSpline()
			{
			}

			public CatmullRomSpline(CatmullRomSpline inSpline)
			{
				mPoints = inSpline.mPoints;
			}

			public CatmullRomSpline(List<SexyVector3> inPoints)
			{
				mPoints = inPoints;
			}

			public SexyVector3 Evaluate(float inTime)
			{
				return default(SexyVector3);
			}
		}

		protected Graphics mGraphics;

		protected RenderDevice3D mRenderDevice;

		protected HRenderContext mRenderContext;

		public Graphics3D(Graphics inGraphics, RenderDevice3D inRenderDevice, HRenderContext inRenderContext)
		{
			mGraphics = inGraphics;
			mRenderDevice = inRenderDevice;
			mRenderContext = inRenderContext;
		}

		protected void SetAsCurrentContext()
		{
			mRenderDevice.SetCurrentContext(mRenderContext);
		}

		public Graphics Get2D()
		{
			return mGraphics;
		}

		public RenderDevice3D GetRenderDevice()
		{
			return mRenderDevice;
		}

		public bool SupportsPixelShaders()
		{
			return mRenderDevice.SupportsPixelShaders();
		}

		public bool SupportsVertexShaders()
		{
			return mRenderDevice.SupportsVertexShaders();
		}

		public bool SupportsCubeMaps()
		{
			return mRenderDevice.SupportsCubeMaps();
		}

		public bool SupportsVolumeMaps()
		{
			return mRenderDevice.SupportsVolumeMaps();
		}

		public bool SupportsImageRenderTargets()
		{
			return mRenderDevice.SupportsImageRenderTargets();
		}

		public uint GetMaxTextureStages()
		{
			return mRenderDevice.GetMaxTextureStages();
		}

		public void AdjustVertexUVsEx(uint theVertexFormat, SexyVertex[] theVertices, int theVertexCount, int theVertexSize)
		{
			SetAsCurrentContext();
			mRenderDevice.AdjustVertexUVsEx(theVertexFormat, theVertices, theVertexCount, theVertexSize);
		}

		public void DrawPrimitiveEx(uint theVertexFormat, EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, Color theColor, int theDrawMode, float tx, float ty, bool blend, uint theFlags)
		{
			SetAsCurrentContext();
			mRenderDevice.DrawPrimitiveEx(theVertexFormat, thePrimitiveType, theVertices, thePrimitiveCount, theColor, theDrawMode, tx, ty, blend, theFlags);
		}

		public void DrawPrimitive(uint theVertexFormat, EPrimitiveType thePrimitiveType, SexyVertex2D[] theVertices, int thePrimitiveCount, Color theColor, int theDrawMode, float tx, float ty, bool blend, uint theFlags)
		{
			SetAsCurrentContext();
			mRenderDevice.DrawPrimitiveEx(theVertexFormat, thePrimitiveType, theVertices, thePrimitiveCount, theColor, theDrawMode, tx, ty, blend, theFlags);
		}

		public void SetBltDepth(float inDepth)
		{
			SetAsCurrentContext();
			mRenderDevice.SetBltDepth(inDepth);
		}

		public void PushTransform(SexyTransform2D theTransform)
		{
			PushTransform(theTransform, false);
		}

		public void PushTransform(SexyTransform2D theTransform, bool concatenate)
		{
			SetAsCurrentContext();
			mRenderDevice.PushTransform(theTransform, concatenate);
		}

		public void PopTransform()
		{
			SetAsCurrentContext();
			mRenderDevice.PopTransform();
		}

		public void PopTransform(ref SexyTransform2D theTransform)
		{
			SetAsCurrentContext();
			mRenderDevice.PopTransform(ref theTransform);
		}

		public void ClearColorBuffer()
		{
			ClearColorBuffer(Color.Black);
		}

		public void ClearColorBuffer(Color inColor)
		{
			SetAsCurrentContext();
			mRenderDevice.ClearColorBuffer(inColor);
		}

		public void ClearDepthBuffer()
		{
			SetAsCurrentContext();
			mRenderDevice.ClearDepthBuffer();
		}

		public void SetDepthState(ECompareFunc inDepthTestFunc, bool inDepthWriteEnabled)
		{
			SetAsCurrentContext();
			mRenderDevice.SetDepthState(inDepthTestFunc, inDepthWriteEnabled);
		}

		public void SetAlphaTest(ECompareFunc inAlphaTestFunc, int inRefAlpha)
		{
			SetAsCurrentContext();
			mRenderDevice.SetAlphaTest(inAlphaTestFunc, inRefAlpha);
		}

		public void SetColorWriteState(int inWriteRedEnabled, int inWriteGreenEnabled, int inWriteBlueEnabled, int inWriteAlphaEnabled)
		{
			SetAsCurrentContext();
			mRenderDevice.SetColorWriteState(inWriteRedEnabled, inWriteGreenEnabled, inWriteBlueEnabled, inWriteAlphaEnabled);
		}

		public void SetWireframe(int inWireframe)
		{
			SetAsCurrentContext();
			mRenderDevice.SetWireframe(inWireframe);
		}

		public void SetBlend(EBlendMode inSrcBlend, EBlendMode inDestBlend)
		{
			SetAsCurrentContext();
			mRenderDevice.SetBlend(inSrcBlend, inDestBlend);
		}

		public void SetBackfaceCulling(int inCullClockwise, int inCullCounterClockwise)
		{
			SetAsCurrentContext();
			mRenderDevice.SetBackfaceCulling(inCullClockwise, inCullCounterClockwise);
		}

		public void SetLightingEnabled(int inLightingEnabled, bool inSetDefaultMaterialState)
		{
			SetAsCurrentContext();
			mRenderDevice.SetLightingEnabled(inLightingEnabled);
			if (inLightingEnabled != 0 && inSetDefaultMaterialState)
			{
				SetMaterialAmbient(Color.White);
				SetMaterialDiffuse(Color.White, 0);
				SetMaterialSpecular(Color.White);
				SetMaterialEmissive(Color.Black);
			}
		}

		public void SetLightEnabled(int inLightIndex, int inEnabled)
		{
			SetAsCurrentContext();
			mRenderDevice.SetLightEnabled(inLightIndex, inEnabled);
		}

		public void SetPointLight(int inLightIndex, SexyVector3 inPos, LightColors inColors, float inRange, SexyVector3 inAttenuation)
		{
			SetAsCurrentContext();
			throw new NotSupportedException();
		}

		public void SetDirectionalLight(int inLightIndex, SexyVector3 inDir, LightColors inColors)
		{
			SetAsCurrentContext();
			throw new NotSupportedException();
		}

		public void SetGlobalAmbient(Color inColor)
		{
			SetAsCurrentContext();
			mRenderDevice.SetGlobalAmbient(inColor);
		}

		public void SetMaterialAmbient(Color inColor)
		{
			SetMaterialAmbient(inColor, -1);
		}

		public void SetMaterialAmbient(Color inColor, int inVertexColorComponent)
		{
			SetAsCurrentContext();
			mRenderDevice.SetMaterialAmbient(inColor, inVertexColorComponent);
		}

		public void SetMaterialDiffuse(Color inColor)
		{
			SetMaterialDiffuse(inColor, -1);
		}

		public void SetMaterialDiffuse(Color inColor, int inVertexColorComponent)
		{
			SetAsCurrentContext();
			mRenderDevice.SetMaterialDiffuse(inColor, inVertexColorComponent);
		}

		public void SetMaterialSpecular(Color inColor, int inVertexColorComponent)
		{
			SetMaterialSpecular(inColor, inVertexColorComponent, 0f);
		}

		public void SetMaterialSpecular(Color inColor)
		{
			SetMaterialSpecular(inColor, -1, 0f);
		}

		public void SetMaterialSpecular(Color inColor, int inVertexColorComponent, float inPower)
		{
			SetAsCurrentContext();
			mRenderDevice.SetMaterialSpecular(inColor, inVertexColorComponent, inPower);
		}

		public void SetMaterialEmissive(Color inColor)
		{
			SetMaterialEmissive(inColor, -1);
		}

		public void SetMaterialEmissive(Color inColor, int inVertexColorComponent)
		{
			SetAsCurrentContext();
			mRenderDevice.SetMaterialEmissive(inColor, inVertexColorComponent);
		}

		public void SetWorldTransform(SexyMatrix4 inMatrix)
		{
			SetAsCurrentContext();
			mRenderDevice.SetWorldTransform(inMatrix);
		}

		public void SetViewTransform(SexyMatrix4 inMatrix)
		{
			SetAsCurrentContext();
			mRenderDevice.SetViewTransform(inMatrix);
		}

		public void SetProjectionTransform(SexyMatrix4 inMatrix)
		{
			SetAsCurrentContext();
			mRenderDevice.SetProjectionTransform(inMatrix);
		}

		public void SetTextureTransform(int inTextureIndex, SexyMatrix4 inMatrix)
		{
			SetTextureTransform(inTextureIndex, inMatrix, 2);
		}

		public void SetTextureTransform(int inTextureIndex, SexyMatrix4 inMatrix, int inNumDimensions)
		{
			SetAsCurrentContext();
			mRenderDevice.SetTextureTransform(inTextureIndex, inMatrix, inNumDimensions);
		}

		public bool SetTexture(int inTextureIndex, Image inImage)
		{
			SetAsCurrentContext();
			return mRenderDevice.SetTexture(inTextureIndex, inImage);
		}

		public void SetTextureWrap(int inTextureIndex, bool inWrap)
		{
			SetAsCurrentContext();
			mRenderDevice.SetTextureWrap(inTextureIndex, inWrap, inWrap);
		}

		public void SetTextureWrap(int inTextureIndex, bool inWrapU, bool inWrapV)
		{
			SetAsCurrentContext();
			mRenderDevice.SetTextureWrap(inTextureIndex, inWrapU, inWrapV);
		}

		public void SetTextureLinearFilter(int inTextureIndex, bool inLinear)
		{
			SetAsCurrentContext();
			mRenderDevice.SetTextureLinearFilter(inTextureIndex, inLinear);
		}

		public void SetTextureCoordSource(int inTextureIndex, int inUVComponent)
		{
			SetTextureCoordSource(inTextureIndex, inUVComponent, ETexCoordGen.TEXCOORDGEN_NONE);
		}

		public void SetTextureCoordSource(int inTextureIndex, int inUVComponent, ETexCoordGen inTexGen)
		{
			SetAsCurrentContext();
			mRenderDevice.SetTextureCoordSource(inTextureIndex, inUVComponent, inTexGen);
		}

		public void SetTextureFactor(int inTextureFactor)
		{
			SetAsCurrentContext();
			mRenderDevice.SetTextureFactor(inTextureFactor);
		}

		public void SetViewport(int theX, int theY, int theWidth, int theHeight, float theMinZ)
		{
			SetViewport(theX, theY, theWidth, theHeight, theMinZ, 1f);
		}

		public void SetViewport(int theX, int theY, int theWidth, int theHeight)
		{
			SetViewport(theX, theY, theWidth, theHeight, 0f, 1f);
		}

		public void SetViewport(int theX, int theY, int theWidth, int theHeight, float theMinZ, float theMaxZ)
		{
			SetAsCurrentContext();
			mRenderDevice.SetViewport((int)mGraphics.mTransX + theX, (int)mGraphics.mTransY + theY, theWidth, theHeight, theMinZ, theMaxZ);
		}

		public RenderEffect GetEffect(RenderEffectDefinition inDefinition)
		{
			return mRenderDevice.GetEffect(inDefinition);
		}

		public void Set3DTransformState(SexyCoords3 inWorldCoords, Camera inCamera)
		{
			SexyMatrix4 sexyMatrix = new SexyMatrix4();
			inWorldCoords.GetOutboundMatrix(sexyMatrix);
			SetWorldTransform(sexyMatrix);
			inCamera.GetViewMatrix(sexyMatrix);
			SetViewTransform(sexyMatrix);
			inCamera.GetProjectionMatrix(sexyMatrix);
			SetProjectionTransform(sexyMatrix);
		}

		public void SetMasking(EMaskMode inMaskMode, int inAlphaRef, float inFrontDepth)
		{
			SetMasking(inMaskMode, inAlphaRef, inFrontDepth, 0.5f);
		}

		public void SetMasking(EMaskMode inMaskMode, int inAlphaRef)
		{
			SetMasking(inMaskMode, inAlphaRef, 0.25f, 0.5f);
		}

		public void SetMasking(EMaskMode inMaskMode)
		{
			SetMasking(inMaskMode, 0, 0.25f, 0.5f);
		}

		public void SetMasking(EMaskMode inMaskMode, int inAlphaRef, float inFrontDepth, float inBackDepth)
		{
			SetAsCurrentContext();
			switch (inMaskMode)
			{
			case EMaskMode.MASKMODE_NONE:
				mRenderDevice.SetAlphaTest(ECompareFunc.COMPARE_ALWAYS, 0);
				mRenderDevice.SetDepthState(ECompareFunc.COMPARE_ALWAYS, false);
				mRenderDevice.SetBltDepth(inBackDepth);
				mRenderDevice.SetBlend(EBlendMode.BLEND_DEFAULT, EBlendMode.BLEND_DEFAULT);
				break;
			case EMaskMode.MASKMODE_WRITE_MASKONLY:
			case EMaskMode.MASKMODE_WRITE_MASKANDCOLOR:
				mRenderDevice.SetAlphaTest(ECompareFunc.COMPARE_GREATER, inAlphaRef);
				mRenderDevice.SetDepthState(ECompareFunc.COMPARE_ALWAYS, true);
				mRenderDevice.SetBltDepth(inFrontDepth);
				if (inMaskMode == EMaskMode.MASKMODE_WRITE_MASKONLY)
				{
					mRenderDevice.SetBlend(EBlendMode.BLEND_ZERO, EBlendMode.BLEND_ONE);
				}
				else
				{
					mRenderDevice.SetBlend(EBlendMode.BLEND_DEFAULT, EBlendMode.BLEND_DEFAULT);
				}
				break;
			case EMaskMode.MASKMODE_TEST_INSIDE:
			case EMaskMode.MASKMODE_TEST_OUTSIDE:
				mRenderDevice.SetAlphaTest(ECompareFunc.COMPARE_GREATER, 0);
				mRenderDevice.SetDepthState((inMaskMode == EMaskMode.MASKMODE_TEST_OUTSIDE) ? ECompareFunc.COMPARE_LESSEQUAL : ECompareFunc.COMPARE_GREATEREQUAL, false);
				mRenderDevice.SetBltDepth(inBackDepth);
				mRenderDevice.SetBlend(EBlendMode.BLEND_DEFAULT, EBlendMode.BLEND_DEFAULT);
				break;
			}
		}

		public void ClearMask()
		{
			SetAsCurrentContext();
			mRenderDevice.ClearDepthBuffer();
		}
	}
}
