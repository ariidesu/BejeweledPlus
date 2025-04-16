// TODO: too stupid to figure this out, anyone help
using SexyFramework.Graphics;

namespace SexyFramework.Drivers.Graphics
{
    public class XNARenderEffectPass
    {
            public XNARenderEffect mEffect;
            public RenderDevice3D mRenderDevice;
            public RenderEffectDefinition mDefinition;
            public object mVertexShader;
            public object mPixelShader;
            public string mTextureRemapStr = "";
            public bool mInProgress = false;

            public void ApplyToDevice()
            {
                
            }

            public XNARenderEffectParamData MakeTempParamForSemantic()
            {
                
            }
    }
}