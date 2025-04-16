using System;

namespace SexyFramework.Graphics
{
	public abstract class RenderEffect : IDisposable
	{
		public virtual void Dispose()
		{
		}

		public abstract RenderDevice3D GetDevice();

		public abstract RenderEffectDefinition GetDefinition();

		public abstract void SetParameter(string inParamName, float[] inFloatData, uint inFloatCount);

		public abstract void SetParameter(string inParamName, float inFloatData);

		public void SetFloat(string inParamName, float inValue)
		{
			SetParameter(inParamName, inValue);
		}

		public void SetVector4(string inParamName, float[] inValue)
		{
			SetParameter(inParamName, inValue, 4u);
		}

		public void SetVector3(string inParamName, float[] inValue)
		{
			SetVector4(inParamName, new float[4]
			{
				inValue[0],
				inValue[1],
				inValue[2],
				1f
			});
		}

		public virtual void SetMatrix(string inParamName, float[] inValue)
		{
			SetParameter(inParamName, inValue, 16u);
		}

		public abstract void GetParameterBySemantic(uint inSemantic, float[] outFloatData, uint inMaxFloatCount);

		public void SetCurrentTechnique(string inName)
		{
			SetCurrentTechnique(inName, true);
		}

		public abstract void SetCurrentTechnique(string inName, bool inCheckValid);

		public abstract string GetCurrentTechniqueName();

		public int Begin(out object outRunHandle)
		{
			return Begin(out outRunHandle, new HRenderContext());
		}

		public abstract int Begin(out object outRunHandle, HRenderContext inRenderContext);

		public abstract void BeginPass(object inRunHandle, int inPass);

		public abstract void EndPass(object inRunHandle, int inPass);

		public abstract void End(object inRunHandle);

		public abstract bool PassUsesVertexShader(int inPass);

		public abstract bool PassUsesPixelShader(int inPass);
	}
}
