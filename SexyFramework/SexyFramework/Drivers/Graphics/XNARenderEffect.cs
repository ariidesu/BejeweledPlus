using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Graphics;
using SexyFramework.Drivers.Graphics;
using System;
using System.Collections.Generic;

namespace SexyFramework.Drivers.Graphics
{
	public class XNARenderEffect : RenderEffect
	{
		public RenderEffectDefinition mDefinition;
		public BaseXNARenderDevice mRenderDevice;
		
		public int mCurrentPass = -1;
		public Dictionary<string, EffectParameter> mParams = new Dictionary<string, EffectParameter>();
		public Dictionary<string, EffectTechnique> mTechniques = new Dictionary<string, EffectTechnique>();

		public XNARenderEffect(RenderEffectDefinition theDefinition, BaseXNARenderDevice theRenderDevice)
		{
			mDefinition = theDefinition;
			mRenderDevice = theRenderDevice;

			if (mDefinition.mEffect != null)
			{
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
		}

		public override RenderDevice3D GetDevice()
		{
			return mRenderDevice;
		}

		public override RenderEffectDefinition GetDefinition()
		{
			return mDefinition;
		}

		public override void SetParameter(string inParamName, float[] inFloatData, uint inFloatCount)
		{
			if (mDefinition.mEffect == null)
			{
				System.Diagnostics.Debug.WriteLine("Attempting to set parameter on null effect");
				return;
			}

			if (!mParams.TryGetValue(inParamName, out var param))
			{
				param = mDefinition.mEffect.Parameters[inParamName];
				if (param != null)
				{
					mParams[inParamName] = param;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Parameter '" + inParamName + "' not found in effect");
					return;
				}
			}

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
			if (mDefinition.mEffect == null)
			{
				System.Diagnostics.Debug.WriteLine("Attempting to set parameter on null effect");
				return;
			}

			if (mParams.TryGetValue(inParamName, out var param))
			{
				param.SetValue(inFloatData);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Parameter '" + inParamName + "' not found in effect");
			}
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
			mRenderDevice.mStateMgr.PushActiveEffect(this);
			return mDefinition.mEffect.CurrentTechnique?.Passes.Count ?? 0;
		}

		public override void BeginPass(object inRunHandle, int inPass)
		{
			mCurrentPass = inPass;
		}

		public bool MG_ApplyPass()
		{
			if (mCurrentPass == -1) return false;
			mDefinition.mEffect.CurrentTechnique?.Passes[mCurrentPass].Apply();
			return true;
		}

		public override void EndPass(object inRunHandle, int inPass)
		{
			mCurrentPass = -1;
		}

		public override void End(object inRunHandle)
		{
			mRenderDevice.mStateMgr.RemoveActiveEffect(this);
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
