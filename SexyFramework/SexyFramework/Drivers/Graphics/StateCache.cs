using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace SexyFramework.Drivers.Graphics
{
	internal static class StateCache
	{
		private struct BlendKey
		{
			public Blend SrcColor, DestColor, SrcAlpha, DestAlpha;
			public BlendFunction ColorOp, AlphaOp;
			public ColorWriteChannels Mask;
		}

		private struct DepthKey
		{
			public bool Enable, WriteEnable;
			public CompareFunction Func;
		}

		private struct RasterKey
		{
			public CullMode Cull;
			public FillMode Fill;
			public bool ScissorEnable;
		}

		private struct SamplerKey
		{
			public TextureAddressMode AddressU, AddressV;
			public TextureFilter Filter;
		}

		private static readonly Dictionary<BlendKey, BlendState>          mBlend   = new();
		private static readonly Dictionary<DepthKey, DepthStencilState>   mDepth   = new();
		private static readonly Dictionary<RasterKey, RasterizerState>    mRaster  = new();
		private static readonly Dictionary<SamplerKey, SamplerState>      mSampler = new();

		public static BlendState GetBlend(Blend srcColor, Blend destColor, Blend srcAlpha, Blend destAlpha,
			BlendFunction colorOp = BlendFunction.Add, BlendFunction alphaOp = BlendFunction.Add,
			ColorWriteChannels mask = ColorWriteChannels.All)
		{
			var key = new BlendKey { SrcColor = srcColor, DestColor = destColor, SrcAlpha = srcAlpha, DestAlpha = destAlpha,
				ColorOp = colorOp, AlphaOp = alphaOp, Mask = mask };
			if (!mBlend.TryGetValue(key, out var s))
			{
				s = new BlendState
				{
					ColorSourceBlend      = srcColor,
					ColorDestinationBlend = destColor,
					AlphaSourceBlend      = srcAlpha,
					AlphaDestinationBlend = destAlpha,
					ColorBlendFunction    = colorOp,
					AlphaBlendFunction    = alphaOp,
					ColorWriteChannels    = mask,
				};
				mBlend[key] = s;
			}
			return s;
		}

		public static DepthStencilState GetDepth(bool enable, bool writeEnable, CompareFunction func)
		{
			var key = new DepthKey { Enable = enable, WriteEnable = writeEnable, Func = func };
			if (!mDepth.TryGetValue(key, out var s))
			{
				s = new DepthStencilState
				{
					DepthBufferEnable      = enable,
					DepthBufferWriteEnable = writeEnable,
					DepthBufferFunction    = func,
				};
				mDepth[key] = s;
			}
			return s;
		}

		public static RasterizerState GetRaster(CullMode cull, FillMode fill = FillMode.Solid, bool scissor = false)
		{
			var key = new RasterKey { Cull = cull, Fill = fill, ScissorEnable = scissor };
			if (!mRaster.TryGetValue(key, out var s))
			{
				s = new RasterizerState
				{
					CullMode             = cull,
					FillMode             = fill,
					ScissorTestEnable    = scissor,
				};
				mRaster[key] = s;
			}
			return s;
		}

		public static SamplerState GetSampler(TextureAddressMode addressU, TextureAddressMode addressV, TextureFilter filter)
		{
			var key = new SamplerKey { AddressU = addressU, AddressV = addressV, Filter = filter };
			if (!mSampler.TryGetValue(key, out var s))
			{
				s = new SamplerState
				{
					AddressU = addressU,
					AddressV = addressV,
					Filter   = filter,
				};
				mSampler[key] = s;
			}
			return s;
		}
	}
}
