using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Drivers.Graphics
{
	public class BaseXNAStateManager : RenderStateManager
	{
		public enum XNA_TRANSFORM
		{
			OGL_TRANSFORM_WORLD,
			OGL_TRANSFORM_VIEW,
			OGL_TRANSFORM_PROJECTION,
			OGL_TRANSFORM_TEXTURE0,
			OGL_TRANSFORM_TEXTURE1,
			OGL_TRANSFORM_TEXTURE2,
			OGL_TRANSFORM_TEXTURE3,
			OGL_TRANSFORM_TEXTURE4,
			OGL_TRANSFORM_TEXTURE5,
			OGL_TRANSFORM_TEXTURE6,
			OGL_TRANSFORM_TEXTURE7,
			OGL_TRANSFORM_ORTHOPROJ,
			OGL_TRANSFORM_COUNT
		}

		public enum EStateGroup
		{
			SG_RS,
			SG_TSS,
			SG_SS,
			SG_LIGHT,
			SG_MATERIAL,
			SG_STREAM,
			SG_TRANSFORM,
			SG_VIEWPORT,
			SG_MISC,
			SG_SCISSOR,
			SG_COUNT
		}

		public enum EXNAStateGroup
		{
			SG_BLEND,
			SG_Raster,
			SG_Depth,
			SG_Sampler,
			SG_Project,
			SG_View,
			SG_World,
			SG_ViewPort,
			SG_Num
		}

		public enum ERenderStateConst
		{
			ST_COUNT_RS = 256,
			ST_COUNT_TSS = 48,
			ST_COUNT_SS = 16,
			ST_COUNT_TRANSFORM = 512
		}

		public enum ELightState
		{
			ST_LIGHT_ENABLED,
			ST_LIGHT_TYPE,
			ST_LIGHT_DIFFUSE,
			ST_LIGHT_SPECULAR,
			ST_LIGHT_AMBIENT,
			ST_LIGHT_POSITION,
			ST_LIGHT_DIRECTION,
			ST_LIGHT_RANGE,
			ST_LIGHT_FALLOFF,
			ST_LIGHT_ATTENUATION,
			ST_LIGHT_ANGLES,
			ST_COUNT_LIGHT
		}

		public enum EMaterialState
		{
			ST_MAT_DIFFUSE,
			ST_MAT_AMBIENT,
			ST_MAT_SPECULAR,
			ST_MAT_EMISSIVE,
			ST_MAT_POWER,
			ST_COUNT_MAT
		}

		public enum EStreamState
		{
			ST_STREAM_DATA,
			ST_STREAM_OFFSET,
			ST_STREAM_STRIDE,
			ST_STREAM_FREQ,
			ST_COUNT_STREAM
		}

		public enum EViewportState
		{
			ST_VIEWPORT_X,
			ST_VIEWPORT_Y,
			ST_VIEWPORT_WIDTH,
			ST_VIEWPORT_HEIGHT,
			ST_VIEWPORT_MINZ,
			ST_VIEWPORT_MAXZ,
			ST_COUNT_VIEWPORT
		}

		public enum EScissorState
		{
			ST_SCISSOR_ENABLE,
			ST_SCISSOR_X,
			ST_SCISSOR_Y,
			ST_SCISSOR_WIDTH,
			ST_SCISSOR_HEIGHT,
			ST_COUNT_SCISSOR
		}

		public enum EMiscState
		{
			ST_MISC_VERTEXFORMAT = 0,
			ST_MISC_VERTEXSIZE = 1,
			ST_MISC_INDICES = 2,
			ST_MISC_SHADERPROGRAM_ORTHO = 3,
			ST_MISC_SHADERPROGRAM_3D = 4,
			ST_MISC_TEXTUREPALETTE = 5,
			ST_MISC_SCISSORRECT = 6,
			ST_MISC_NPATCHMODE = 7,
			ST_MISC_SRCBLENDOVERRIDE = 8,
			ST_MISC_DESTBLENDOVERRIDE = 9,
			ST_MISC_BLTDEPTH = 10,
			ST_MISC_3DMODE = 11,
			ST_MISC_CULLMODE = 12,
			ST_MISC_USE_TEXSCALE = 13,
			ST_MISC_TEXTURE = 14,
			ST_MISC_PIXELSHADERCONST = 15,
			ST_MISC_VERTEXSHADERCONST = 16,
			ST_MISC_CLIPPLANE = 17,
			ST_MISC_TEXTUREREMAP = 18,
			ST_MISC_ATLASENABLEDANDBASE = 19,
			ST_MISC_ATLASUV = 20,
			ST_COUNT_MISC = 21,
			ST_COUNT_MISC_SINGLE = 14
		}

		public BlendState mXNABlendState;

		public RasterizerState mXNARasterizerState;

		public DepthStencilState mXNADepthStencilState;

		public BlendState mXNALastBlendState;

		public Texture2D mXNATextureSlots;

		public Texture2D mLastXNATextureSlots;

		public SamplerState mXNASamplerStateSlots;

		public SamplerState mXNALastSamplerStateSlots;

		public Matrix mXNAProjectionMatrix = Matrix.Identity;

		public Matrix mXNALastProjectionMatrix;

		public Matrix mXNAViewMatrix = Matrix.Identity;

		public Matrix mXNAWorldMatrix = Matrix.Identity;

		public Matrix mXNALastWorldMatrix;

		public Viewport mXNAViewPort;

		public List<State> mRenderStates;

		public List<List<State>> mTextureStageStates;

		public List<List<State>> mSamplerStates;

		public List<List<State>> mLightStates;

		public List<State> mMaterialStates;

		public List<List<State>> mStreamStates;

		public List<List<State>> mTransformStates;

		public List<State> mViewportStates;

		public List<List<State>> mMiscStates;

		public GraphicsDeviceManager mDevice;

		public Stack<Graphics3D.EBlendMode> mStatckSrcBlendState;

		public Stack<Graphics3D.EBlendMode> mStatckDestBlendState;

		public Stack<RasterizerState> mStatckRasterizerState;

		public Stack<DepthStencilState> mStatckDepthStencilState;

		public Stack<SamplerState> mStatckSamplerState;

		public Stack<Matrix> mStatckProjectionMatrix;

		public Stack<Matrix> mStatckViewMatrix;

		public Stack<Matrix> mStatckWorldMatrix;

		public Stack<Viewport> mStatckViewPort;

		public Stack<int> mStatckDrawMode;

		public bool mAtalasEnabled;

		public bool mStateDirty;

		public bool mTextureStateDirty;

		public bool mProjectMatrixDirty;

		public int mDrawMode;

		public SexyVector2 mAtalasBase;

		public SexyVector2 mAtalasU;

		public SexyVector2 mAtalasV;

		public Graphics3D.EBlendMode mSrcBlendMode = Graphics3D.EBlendMode.BLEND_DEFAULT;

		public Graphics3D.EBlendMode mDestBlendMode = Graphics3D.EBlendMode.BLEND_DEFAULT;

		public BaseXNAStateManager(ref GraphicsDeviceManager theDevice)
		{
			mDevice = theDevice;
			mXNABlendState = BlendState.AlphaBlend;
			mXNARasterizerState = RasterizerState.CullNone;
			mXNADepthStencilState = DepthStencilState.Default;
			mXNATextureSlots = null;
			mXNASamplerStateSlots = SamplerState.LinearClamp;
			mXNAProjectionMatrix = Matrix.CreateOrthographicOffCenter(0f, GlobalMembers.gSexyAppBase.mWidth, GlobalMembers.gSexyAppBase.mHeight, 0f, -1000f, 1000f);
			mXNAViewMatrix = Matrix.CreateLookAt(new Vector3(0f, 0f, 300f), Vector3.Zero, Vector3.Up);
			mXNAWorldMatrix = Matrix.Identity;
			new Viewport(0, 0, GlobalMembers.gSexyAppBase.mWidth, GlobalMembers.gSexyAppBase.mHeight);
			mStatckSrcBlendState = new Stack<Graphics3D.EBlendMode>();
			mStatckDestBlendState = new Stack<Graphics3D.EBlendMode>();
			mStatckRasterizerState = new Stack<RasterizerState>();
			mStatckDepthStencilState = new Stack<DepthStencilState>();
			mStatckSamplerState = new Stack<SamplerState>();
			mStatckProjectionMatrix = new Stack<Matrix>();
			mStatckViewMatrix = new Stack<Matrix>();
			mStatckWorldMatrix = new Stack<Matrix>();
			mStatckViewPort = new Stack<Viewport>();
		}

		public override void Init()
		{
		}

		public override void Reset()
		{
		}

		protected void InitRenderState(ulong inIndex, ref string inStateName, ulong inHardwareDefaultValue, bool inHasContextDefault, ulong inContextDefaultValue, string inValueEnumName)
		{
			string inName = string.Format("RS:0", inStateName);
			if (inHasContextDefault)
			{
				mRenderStates[(int)inIndex].Init(new StateValue(inHardwareDefaultValue), new StateValue(inContextDefaultValue), inName, inValueEnumName);
			}
			else
			{
				mRenderStates[(int)inIndex].Init(new StateValue(inHardwareDefaultValue), inName, inValueEnumName);
			}
		}

		protected void InitRenderStateFloat(ulong inIndex, ref string inStateName, float inDefaultValue)
		{
			InitRenderState(inIndex, ref inStateName, (ulong)inDefaultValue, false, 0uL, null);
		}

		protected void InitTextureStageState(ulong inFirstStage, ulong inLastStage, ulong inIndex, string inStateName, ulong inDefaultValue, bool inHasContextDefault, ulong inContextDefaultValue, string inValueEnumName)
		{
			for (ulong num = inFirstStage; num <= inLastStage; num++)
			{
				string inName = string.Format("TSS:0[1]", inStateName, num);
				if (inHasContextDefault)
				{
					mTextureStageStates[(int)inIndex][(int)num].Init(new StateValue(inDefaultValue), new StateValue(inContextDefaultValue), inName, inValueEnumName);
				}
				else
				{
					mTextureStageStates[(int)inIndex][(int)num].Init(new StateValue(inDefaultValue), inName, inValueEnumName);
				}
			}
		}

		protected void InitTextureStageStateFloat(ulong inFirstStage, ulong inLastStage, ulong inIndex, string inStateName, float inDefaultValue)
		{
			InitTextureStageState(inFirstStage, inLastStage, inIndex, inStateName, (ulong)inDefaultValue, false, 0uL, null);
		}

		protected void InitSamplerState(ulong inFirstStage, ulong inLastStage, ulong inIndex, string inStateName, ulong inDefaultValue, bool inHasContextDefault, ulong inContextDefaultValue, string inValueEnumName)
		{
			for (ulong num = inFirstStage; num <= inLastStage; num++)
			{
				string inName = string.Format("SS:1[2]", inStateName, num);
				if (inHasContextDefault)
				{
					mSamplerStates[(int)inIndex][(int)num].Init(new StateValue(inDefaultValue), new StateValue(inContextDefaultValue), inName, inValueEnumName);
				}
				else
				{
					mSamplerStates[(int)inIndex][(int)num].Init(new StateValue(inDefaultValue), inName, inValueEnumName);
				}
			}
		}

		protected void InitSamplerStateFloat(ulong inFirstStage, ulong inLastStage, ulong inIndex, string inStateName, float inDefaultValue)
		{
			InitSamplerState(inFirstStage, inLastStage, inIndex, inStateName, (ulong)inDefaultValue, false, 0uL, null);
		}

		protected void InitStates()
		{
			for (uint num = 0u; num < 256; num++)
			{
				mRenderStates.Add(new State(this, 0u, num));
			}
			mRenderStates[7].Init(1uL, 0uL, "ZENABLE", "");
			mRenderStates[14].Init(1uL, 0uL, "ZWRITEENABLE", "");
			mRenderStates[15].Init(0uL, 0uL, "ALPHATESTENABLE", "");
			mRenderStates[23].Init(7uL, "ZFUNC", "");
			mRenderStates[24].Init(0uL, 0uL, "ALPHAREF", "");
			mRenderStates[25].Init(8uL, 8uL, "ALPHAFUNC", "");
			mRenderStates[19].Init(1uL, "SRCBLEND", "Sexy::Graphics3D::EBlendMode");
			mRenderStates[20].Init(1uL, "DESTBLEND", "Sexy::Graphics3D::EBlendMode");
			mRenderStates[168].Init(15uL, "COLORWRITE", "");
			for (uint num2 = 0u; num2 < 11; num2++)
			{
				mLightStates.Add(new List<State>());
			}
			for (uint num3 = 0u; num3 < 11; num3++)
			{
				for (uint num4 = 0u; num4 < 8; num4++)
				{
					mLightStates[(int)num3].Add(new State(this, 3u, num3, num4));
				}
			}
			for (ulong num5 = 0uL; num5 < 8; num5++)
			{
				mLightStates[0][(int)num5].Init(0uL, string.Format("LIGHT:ENABLED[0]", num5));
				mLightStates[1][(int)num5].Init(0uL, string.Format("LIGHT:TYPE[0]", num5), "D3DLIGHTTYPE");
				mLightStates[2][(int)num5].Init(new StateValue(1f, 1f, 10f, 0f), string.Format("LIGHT:DIFFUSE[0]", num5));
				mLightStates[3][(int)num5].Init(new StateValue(0f, 0f, 0f, 0f), string.Format("LIGHT:SPECULAR[0]", num5));
				mLightStates[4][(int)num5].Init(new StateValue(0f, 0f, 0f, 0f), string.Format("LIGHT:AMBIENT[0]", num5));
				mLightStates[5][(int)num5].Init(new StateValue(0f, 0f, 0f, 0f), string.Format("LIGHT:POSITION[0]", num5));
				mLightStates[6][(int)num5].Init(new StateValue(0f, 0f, 1f, 0f), string.Format("LIGHT:DIRECTION[0]", num5));
				mLightStates[7][(int)num5].Init(0uL, string.Format("LIGHT:RANGE[%d]", num5));
				mLightStates[8][(int)num5].Init(0uL, string.Format("LIGHT:FALLOFF[%d]", num5));
				mLightStates[9][(int)num5].Init(new StateValue(0f, 0f, 0f, 0f), string.Format("LIGHT:ATTENUATION[0]", num5));
				mLightStates[10][(int)num5].Init(new StateValue(0f, 0f, 0f, 0f), string.Format("LIGHT:ANGLES[0]", num5));
			}
			for (uint num6 = 0u; num6 < 512; num6++)
			{
				mTransformStates.Add(new List<State>());
			}
			for (uint num7 = 0u; num7 < 512; num7++)
			{
				for (uint num8 = 0u; num8 < 4; num8++)
				{
					mTransformStates[(int)num7].Add(new State(this, 6u, num7, num8));
				}
			}
			for (uint num9 = 0u; num9 < 512; num9++)
			{
				string arg;
				switch (num9)
				{
				case 0u:
					arg = "WORLD";
					break;
				case 1u:
					arg = "VIEW";
					break;
				case 2u:
					arg = "PROJECTION";
					break;
				case 11u:
					arg = "ORTHOPROJECTION";
					break;
				default:
					arg = ((num9 < 3 || num9 > 10) ? string.Format("0", num9) : string.Format("TEXTURE0", num9 - 3));
					break;
				}
				for (uint num10 = 0u; num10 < 4; num10++)
				{
					mTransformStates[(int)num9][(int)num10].Init(new StateValue(0f, 0f, 0f, 0f), string.Format("TRANSFORM:0[1]", arg, num10));
				}
			}
			for (uint num11 = 0u; num11 < 6; num11++)
			{
				mViewportStates.Add(new State(this, 7u, num11));
			}
			mViewportStates[0].Init(0uL, "VIEWPORT:X");
			mViewportStates[1].Init(0uL, "VIEWPORT:Y");
			mViewportStates[2].Init((ulong)GlobalMembers.gSexyAppBase.mWidth, "VIEWPORT:WIDTH");
			mViewportStates[3].Init((ulong)GlobalMembers.gSexyAppBase.mHeight, "VIEWPORT:HEIGHT");
			mViewportStates[4].Init(0uL, "VIEWPORT_MINZ");
			mViewportStates[5].Init(1uL, "VIEWPORT_MAXZ");
			for (uint num12 = 0u; num12 < 21; num12++)
			{
				mMiscStates.Add(new List<State>());
			}
			for (uint num13 = 0u; num13 < 14; num13++)
			{
				mMiscStates[(int)num13].Add(new State(this, 8u, num13));
			}
			for (uint num14 = 0u; num14 < 8; num14++)
			{
				mMiscStates[14].Add(new State(this, 8u, 14u, num14));
			}
			for (uint num15 = 0u; num15 < 32; num15++)
			{
				mMiscStates[15].Add(new State(this, 8u, 15u, num15));
			}
			for (uint num16 = 0u; num16 < 256; num16++)
			{
				mMiscStates[16].Add(new State(this, 8u, 16u, num16));
			}
			for (uint num17 = 0u; num17 < 4; num17++)
			{
				mMiscStates[17].Add(new State(this, 8u, 17u, num17));
			}
			for (uint num18 = 0u; num18 < 8; num18++)
			{
				mMiscStates[18].Add(new State(this, 8u, 18u, num18));
			}
			for (uint num19 = 0u; num19 < 8; num19++)
			{
				mMiscStates[19].Add(new State(this, 8u, 19u, num19));
				mMiscStates[20].Add(new State(this, 8u, 20u, num19));
			}
			mMiscStates[0][0].Init(0uL, "MISC:VERTEXFORMAT");
			mMiscStates[1][0].Init(0uL, "MISC:VERTEXSIZE");
			mMiscStates[3][0].Init(0uL, "MISC:SHADERPROGRAM_ORTHO");
			mMiscStates[4][0].Init(0uL, "MISC:SHADERPROGRAM_3D");
			mMiscStates[10][0].Init(0uL, 0uL, "MISC:BLTDEPTH");
			mMiscStates[11][0].Init(0uL, "MISC:3DMODE");
			mMiscStates[12][0].Init(0uL, "MISC:CULLMODE");
			mMiscStates[8][0].Init(65535uL, "MISC:SRCBLENDOVERRIDE", "Sexy::Graphics3D::EBlendMode");
			mMiscStates[9][0].Init(65535uL, "MISC:DESTBLENDOVERRIDE", "Sexy::Graphics3D::EBlendMode");
			mMiscStates[10][0].Init(0uL, "MISC:BLTDEPTH");
			mMiscStates[13][0].Init(0uL, "MISC:USE_TEXSCALE");
			for (uint num20 = 0u; num20 < 8; num20++)
			{
				mMiscStates[14].Add(new State(this, 8u, 14u, num20));
			}
			for (uint num21 = 0u; num21 < 8; num21++)
			{
				mMiscStates[14][(int)num21].Init(0uL, string.Format("MISC:TEXTURE[0]", num21));
			}
			for (uint num22 = 0u; num22 < 8; num22++)
			{
				mMiscStates[19].Add(new State(this, 8u, 19u, num22));
				mMiscStates[20].Add(new State(this, 8u, 20u, num22));
			}
			for (uint num23 = 0u; num23 < 8; num23++)
			{
				mMiscStates[19][(int)num23].Init(new StateValue(0f, 0f, 0f, 0f), string.Format("MISC:ATLASENABLEDANDBASE[0]", num23));
				mMiscStates[20][(int)num23].Init(new StateValue(0f, 0f, 1f, 1f), string.Format("MISC:ATLASUV[0]", num23));
			}
		}

		protected void ResetStates(List<State> list)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				list[i].Reset();
			}
		}

		protected void ResetStatesList(List<List<State>> list)
		{
			foreach (List<State> item in list)
			{
				ResetStates(item);
			}
		}

		protected void ResetStates()
		{
			ResetStates(mRenderStates);
			ResetStatesList(mTextureStageStates);
			ResetStatesList(mSamplerStates);
			ResetStatesList(mLightStates);
			ResetStates(mMaterialStates);
			ResetStatesList(mStreamStates);
			ResetStatesList(mTransformStates);
			ResetStates(mViewportStates);
			ResetStatesList(mMiscStates);
		}

		public void SetRenderState(ulong inRS, ulong inValue)
		{
			mRenderStates[(int)inRS].SetValue(inValue);
		}

		private void SetTextureStageState(ulong inStage, ulong inTSS, ulong inValue)
		{
			mTextureStageStates[(int)inTSS][(int)inStage].SetValue(inValue);
		}

		public void SetSamplerState(ulong inSampler, ulong inSS, ulong inValue)
		{
			mSamplerStates[(int)inSS][(int)inSampler].SetValue(inValue);
		}

		public void SetSamplerState(SamplerState state)
		{
			if (mXNASamplerStateSlots != state)
			{
				mStateDirty = true;
			}
			mXNALastSamplerStateSlots = mXNASamplerStateSlots;
			mXNASamplerStateSlots = state;
		}

		public void SetRasterizerState(RasterizerState state)
		{
			if (mXNARasterizerState != state)
			{
				mStateDirty = true;
			}
			mXNARasterizerState = state;
		}

		public void SetBlendStateState(BlendState state)
		{
			if (mXNABlendState.AlphaSourceBlend != state.AlphaSourceBlend || mXNABlendState.AlphaDestinationBlend != state.AlphaDestinationBlend || mXNABlendState.ColorDestinationBlend != state.ColorDestinationBlend || mXNABlendState.ColorSourceBlend != state.ColorSourceBlend)
			{
				mStateDirty = true;
			}
			mXNALastBlendState = mXNABlendState;
			mXNABlendState = state;
		}

		public void SetBlendOverride(Graphics3D.EBlendMode src, Graphics3D.EBlendMode dest)
		{
			if (mSrcBlendMode != src || mDestBlendMode != dest)
			{
				mStateDirty = true;
			}
			mSrcBlendMode = src;
			mDestBlendMode = dest;
		}

		public void SetDepthStencilState(DepthStencilState state)
		{
			if (mXNADepthStencilState != state)
			{
				mStateDirty = true;
			}
			mXNADepthStencilState = state;
		}

		public void SetProjectionTransform(Matrix mat)
		{
			if (mXNAProjectionMatrix != mat)
			{
				mProjectMatrixDirty = true;
			}
			mXNALastProjectionMatrix = mXNAProjectionMatrix;
			mXNAProjectionMatrix = mat;
		}

		public void SetViewTransform(Matrix mat)
		{
			mXNAViewMatrix = mat;
		}

		public void SetWorldTransform(Matrix mat)
		{
			mXNALastWorldMatrix = mXNAWorldMatrix;
			mXNAWorldMatrix = mat;
		}

		public void SetTexture(Texture2D texture)
		{
			mTextureStateDirty = true;
			mLastXNATextureSlots = mXNATextureSlots;
			mXNATextureSlots = texture;
		}

		private void SetLightEnabled(ulong inLightIndex, bool inEnabled)
		{
		}

		private void SetMaterialAmbient(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
		{
		}

		private void SetMaterialDiffuse(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
		{
		}

		private void SetMaterialSpecular(SexyFramework.Graphics.Color inColor, int inVertexColorComponent, float inPower)
		{
		}

		private void SetMaterialEmissive(SexyFramework.Graphics.Color inColor, int inVertexColorComponent)
		{
		}

		public void SetViewport(int inX, int inY, int inWidth, int inHeight, float inMinZ, float inMaxZ)
		{
			mXNAViewPort = new Viewport(inX, inY, inWidth, inHeight);
		}

		private void SetFVF(ulong inFVF)
		{
		}

		private void SetCurrentTexturePalette(ulong inPaletteIndex)
		{
		}

		private void SetScissorRect(Rect inRect)
		{
		}

		private void SetNPatchMode(float inSegments)
		{
		}

		private void SetTextureRemap(ulong inLogicalSampler, ulong inPhysicalSampler)
		{
		}

		private void SetPixelShaderConstantF(ulong inStartRegister, float[] inConstantData, ulong inVector4fCount)
		{
		}

		private void SetVertexShaderConstantF(ulong inStartRegister, float[] inConstantData, ulong inVector4fCount)
		{
		}

		private void SetClipPlane(ulong inIndex, float[] inPlane)
		{
		}

		private void SetBltDepth(float inDepth)
		{
		}

		public new void PushState()
		{
			mStatckSrcBlendState.Push(mSrcBlendMode);
			mStatckDestBlendState.Push(mDestBlendMode);
			mStatckRasterizerState.Push(mXNARasterizerState);
			mStatckDepthStencilState.Push(mXNADepthStencilState);
			mStatckSamplerState.Push(mXNASamplerStateSlots);
			mStatckProjectionMatrix.Push(mXNAProjectionMatrix);
			mStatckViewMatrix.Push(mXNAViewMatrix);
			mStatckWorldMatrix.Push(mXNAWorldMatrix);
			mStatckViewPort.Push(mXNAViewPort);
		}

		public new void PopState()
		{
			mSrcBlendMode = mStatckSrcBlendState.Pop();
			mDestBlendMode = mStatckDestBlendState.Pop();
			mXNARasterizerState = mStatckRasterizerState.Pop();
			mXNADepthStencilState = mStatckDepthStencilState.Pop();
			if (mXNASamplerStateSlots != mStatckSamplerState.Peek())
			{
				mStateDirty = true;
			}
			mXNALastSamplerStateSlots = mXNASamplerStateSlots;
			mXNASamplerStateSlots = mStatckSamplerState.Pop();
			mXNAProjectionMatrix = mStatckProjectionMatrix.Pop();
			mXNAViewMatrix = mStatckViewMatrix.Pop();
			mXNAWorldMatrix = mStatckWorldMatrix.Pop();
			mXNAViewPort = mStatckViewPort.Pop();
		}

		public void SetAtlasState(ulong inSampler, bool inEnabled, SexyVector2 inBase, SexyVector2 inU, SexyVector2 inV)
		{
			mAtalasEnabled = inEnabled;
			if (mAtalasEnabled)
			{
				mAtalasBase = inBase;
				mAtalasU = inU;
				mAtalasV = inV;
			}
		}

		public bool GetAtlasState(ulong inSampler, ref SexyVector2 outBase, ref SexyVector2 outU, ref SexyVector2 outV)
		{
			if (!mAtalasEnabled)
			{
				return false;
			}
			outBase = mAtalasBase;
			outU = mAtalasU;
			outV = mAtalasV;
			return true;
		}
	}
}
