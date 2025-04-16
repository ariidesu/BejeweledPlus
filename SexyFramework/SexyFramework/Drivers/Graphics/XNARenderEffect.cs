using System;
using System.Collections.Generic;
using System.Numerics;
using SexyFramework.Graphics;
using SexyFramework.SexyFramework.Drivers.Graphics;

namespace SexyFramework.Drivers.Graphics
{
    public class XNARenderEffect : RenderEffect
    {
        public RenderDevice3D mRenderDevice;
        public RenderEffectDefinition mDefinition;
        public XNARenderEffectParamCollection mParams;
        public List<XNARenderEffectTechnique> mTechniques = new List<XNARenderEffectTechnique>();
        public Dictionary<string, XNARenderEffectTechnique> mTechniqueNameMap = new Dictionary<string, XNARenderEffectTechnique>();
        public XNARenderEffectTechnique mCurrentTechnique = null;
        public int mBeginPassRefCount;
        
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
            var paramNamed = mParams.GetParamNamed(inParamName, createIfMissing: true);
            if (paramNamed != null)
            {
                paramNamed.SetValue(inFloatData, inFloatCount);
            }
            if (mBeginPassRefCount > 0 && mCurrentTechnique != null)
            {
                mCurrentTechnique.ParametersChanged();
            }
        }

        public override void SetParameter(string inParamName, float inFloatData)
        {
            throw new NotImplementedException();
        }

        public override void GetParameterBySemantic(uint inSemantic, float[] outFloatData, uint inMaxFloatCount)
        {
            XNARenderEffectParamData tempParam = XNARenderEffectPass.MakeTempParamForSemantic((int)(inMaxFloatCount / 4), mRenderDevice, inSemantic);

            if (tempParam.mFloatData != null && tempParam.mFloatData.Count > 0)
            {
                int countToCopy = Math.Min(tempParam.mFloatData.Count, (int)inMaxFloatCount);
                for (int i = 0; i < countToCopy; i++)
                {
                    outFloatData[i] = tempParam.mFloatData[i];
                }
            }
        }

        public override void SetCurrentTechnique(string inName, bool inCheckValid)
        {
            if (mTechniqueNameMap.TryGetValue(inName, out var foundTechnique))
            {
                mCurrentTechnique = foundTechnique;

                if (mCurrentTechnique != null && mCurrentTechnique.mCompatFallback != null)
                {
                    SetCurrentTechnique(mCurrentTechnique.mCompatFallback, inCheckValid);
                }

                if (inCheckValid && mCurrentTechnique != null)
                {
                    mCurrentTechnique = mCurrentTechnique.GetValidTechnique();
                }
            }
        }

        public override string GetCurrentTechniqueName()
        {
            if (mCurrentTechnique != null)
            {
                return mCurrentTechnique.GetName();
            }

            return "";
        }

        public override int Begin(out object outRunHandle, HRenderContext inRenderContext)
        {
            if (!inRenderContext.IsValid())
            {
                inRenderContext = mRenderDevice.GetCurrentContext();
            }
            else
            {
                mRenderDevice.SetCurrentContext(inRenderContext);
            }

            outRunHandle = inRenderContext.mHandlePtr;

            if (mCurrentTechnique == null)
                return 1;

            return mCurrentTechnique.mPasses?.Count ?? 0;
        }

        public override void BeginPass(object inRunHandle, int inPass)
        {
            mRenderDevice.SetCurrentContext(new HRenderContext(inRunHandle));

            mBeginPassRefCount++;
            mRenderDevice.PushState();

            mCurrentTechnique?.BeginPass(inPass);
        }

        public override void End(object inRunHandle)
        {
            mRenderDevice.SetCurrentContext(new HRenderContext(inRunHandle));
        }

        public override void EndPass(object inRunHandle, int inPass)
        {
            End(inRunHandle);

            if (mCurrentTechnique != null)
            {
                var passes = mCurrentTechnique.mPasses;
                if (inPass < passes.Count)
                {
                    passes[inPass].mInProgress = false;
                }
            }

            mRenderDevice.PopState();
            mBeginPassRefCount--;
        }
        
        public override bool PassUsesPixelShader(int inPass)
        {
            return mCurrentTechnique != null && mCurrentTechnique.PassUsesPixelShader(inPass, mCurrentTechnique);
        }


        public override bool PassUsesVertexShader(int inPass)
        {
            return mCurrentTechnique != null && mCurrentTechnique.PassUsesVertexShader(inPass, mCurrentTechnique);
        }
    }
}
