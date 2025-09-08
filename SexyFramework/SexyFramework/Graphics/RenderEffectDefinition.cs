using System;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Drivers.App;
using SexyFramework.Drivers.Graphics;
using SexyFramework.Resource;

namespace SexyFramework.Graphics
{
    public class RenderEffectDefinition : IDisposable
    {
        public Effect mEffect;
        
        public byte[] mData;

        public string mSrcFileName;

        public string mDataFormat;

        public bool LoadFromMem(int inDataLen, byte[] inData, string inSrcFileName, string inDataFormat)
        {
            mData = inData;
            mSrcFileName = inSrcFileName;
            mDataFormat = inDataFormat;
            return inData != null;
        }

        public bool LoadFromFile(string inFileName, string inSrcFileName)
        {
        	string text = Common.GetFileDir(inFileName, true) + Common.GetFileName(inFileName, true);
        	mSrcFileName = inSrcFileName;
        	try
        	{
        		mEffect = WP7AppDriver.sWP7AppDriverInstance.mContentManager.Load<Effect>(text);
        		return mEffect != null;
        	}
        	catch
        	{
        		mEffect = null;
        		return false;
        	}
        }

        public void Initialize(GraphicsDevice theGraphicsDevice)
        {
            
        }

        public virtual void Dispose()
        {
            mData = null;
            if (mEffect != null)
            {
                mEffect.Dispose();
                mEffect = null;
            }
        }
    }
}