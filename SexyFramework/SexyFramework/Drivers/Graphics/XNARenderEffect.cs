using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Graphics;

namespace SexyFramework.Drivers.Graphics
{
	public class XNARenderEffect : RenderEffect
	{
		public RenderEffectDefinition mDefinition;
		
		public Dictionary<string, EffectParameter> mParams = new Dictionary<string, EffectParameter>();
		public Dictionary<string, EffectTechnique> mTechniques = new Dictionary<string, EffectTechnique>();

		public XNARenderEffect(RenderEffectDefinition theDefinition, GraphicsDevice theGraphicsDevice)
		{
			mDefinition = theDefinition;
			mDefinition.Initialize(theGraphicsDevice);
			
			foreach (var param in mDefinition.mEffect.Parameters)
				mParams[param.Name] = param;

			foreach (var tech in mDefinition.mEffect.Techniques)
				mTechniques[tech.Name] = tech;
			
			if (mTechniques.TryGetValue("Default", out var defaultTech))
			{
				mDefinition.mEffect.CurrentTechnique = defaultTech;
			}
			else if (mTechniques.Count > 0)
			{
				mDefinition.mEffect.CurrentTechnique = mDefinition.mEffect.Techniques[0];
			}
		}

		public override RenderDevice3D GetDevice()
		{
			return null;
		}

		public override RenderEffectDefinition GetDefinition()
		{
			return mDefinition;
		}

		public override void SetParameter(string inParamName, float[] inFloatData, uint inFloatCount)
		{
			if (!mParams.TryGetValue(inParamName, out var param))
			{
				param = mDefinition.mEffect.Parameters[inParamName];
				if (param != null)
				{
					mParams[inParamName] = param;
				}
				else
				{
					return;
				}
			}
			Console.WriteLine(param.ParameterClass + " " + param.ParameterType);

			switch (inFloatCount)
			{
				case 4:
					param.SetValue(new Vector4(
						inFloatData[0],
						inFloatData[1],
						inFloatData[2],
						inFloatData[3]
					));
					break;

				case 16:
					param.SetValue(new Matrix(
						inFloatData[0], inFloatData[1], inFloatData[2], inFloatData[3],
						inFloatData[4], inFloatData[5], inFloatData[6], inFloatData[7],
						inFloatData[8], inFloatData[9], inFloatData[10], inFloatData[11],
						inFloatData[12], inFloatData[13], inFloatData[14], inFloatData[15]
					));
					break;

				default:
					param.SetValue(inFloatData);
					break;
			}
		}

		public override void SetParameter(string inParamName, float inFloatData)
		{
			if (mParams.TryGetValue(inParamName, out var param))
				param.SetValue(inFloatData);
		}

		public override void GetParameterBySemantic(uint inSemantic, float[] outFloatData, uint inMaxFloatCount)
		{
			for (int i = 0; i < inMaxFloatCount; i++)
				outFloatData[i] = 0f;
		}

		public override void SetCurrentTechnique(string inName, bool inCheckValid)
		{
			if (mTechniques.TryGetValue(inName, out var tech))
			{
				mDefinition.mEffect.CurrentTechnique = tech;
			}
		}

		public override string GetCurrentTechniqueName()
		{
			return mDefinition.mEffect.CurrentTechnique?.Name ?? "";
		}

		public override int Begin(out object outRunHandle, HRenderContext inRenderContext)
		{
			outRunHandle = 0;
			return mDefinition.mEffect.CurrentTechnique?.Passes.Count ?? 0;
		}

		public override void BeginPass(object inRunHandle, int inPass)
		{
			Console.WriteLine(mDefinition.mEffect.CurrentTechnique);
			mDefinition.mEffect.CurrentTechnique?.Passes[inPass].Apply();
			Console.WriteLine("Called BeginPass " + inPass + mDefinition.mEffect.GraphicsDevice.Adapter);
		}

		public override void EndPass(object inRunHandle, int inPass)
		{
		}

		public override void End(object inRunHandle)
		{
		}

		public override bool PassUsesVertexShader(int inPass)
		{
			return true;
		}

		public override bool PassUsesPixelShader(int inPass)
		{
			return true;
		}
	}
}
