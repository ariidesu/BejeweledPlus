using SexyFramework.Graphics;

namespace SexyFramework.Drivers.Graphics
{
    public class XNARenderEffectTechnique
    {
        public XNARenderEffect mEffect;
        public RenderDevice3D mRenderDevice;
        public RenderEffectDefinition mDefinition;
        public XNARenderEffectParamCollection mParams;
        public List<XNARenderEffectPass> mPasses;
        public XNARenderEffectTechnique mValidTechnique;
        public bool mValidated = false;
        public string mCompatFallback = "";
        
        // TODO: Finish this
        public void BeginPass(int passIndex)
        {
            var pass = mPasses[passIndex];
        }

        public string GetName()
        {
            return mDefinition.mTechniqueName;
        }
        
        // TODO: Finish this
        public XNARenderEffectTechnique GetValidTechnique(int param)
        {
            if (mValidated)
                return mValidTechnique;
        }
        
        // TODO: Finish this
        public void ParametersChanged(XNARenderEffectTechnique technique)
        {
            if (technique.mPasses.Count == 0)
                return;

            for (int i = 0; i < technique.mPasses.Count; i++)
            {
                var pass = technique.mPasses[i];
                if (pass.mInProgress)
                {
                    /*
                      Sexy::D3DRenderEffect::Pass::ApplyToDevice(
                        (Sexy::D3DRenderEffect::Pass *)a2->mParams,
                        (Sexy::D3DStateManager *)v5,
                        (int)a2->mParams,
                        1);
                     */
                }
            }
        }
        
        public bool PassUsesPixelShader(int index, XNARenderEffectTechnique technique)
        {
            if (technique.mPasses == null || index >= technique.mPasses.Count || index < 0)
            {
                return false;
            }
    
            var pass = technique.mPasses[index];
            return pass.mPixelShader != null; 
        }

        
        public bool PassUsesVertexShader(int index, XNARenderEffectTechnique technique)
        {
            if (technique.mPasses == null || index >= technique.mPasses.Count || index < 0)
            {
                return false;
            }
    
            var pass = technique.mPasses[index];
            return pass.mVertexShader != null;
        }

    }
}