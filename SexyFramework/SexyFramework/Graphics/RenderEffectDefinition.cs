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

        public string mFileName;

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
            bool result = false;
            string text = Common.GetFileDir(inFileName, true) + Common.GetFileName(inFileName, true);
            mFileName = inFileName;
            mSrcFileName = inSrcFileName;

            string[] paths = { inFileName, text };
            for (int i = 0; i < paths.Length && !result; i++)
            {
                PFILE pFILE = new PFILE(paths[i], "rb");
                if (pFILE.Open())
                {
                    byte[] data = pFILE.GetData();
                    if (data != null)
                    {
                        string text2 = string.Empty;
                        int num = paths[i].LastIndexOf('.');
                        if (num >= 0)
                        {
                            text2 = paths[i].Substring(num);
                        }
                        if (text2.Length > 1)
                        {
                            text2 = text2.Substring(1);
                        }
                        result = LoadFromMem(data.Length, data, inSrcFileName, text2);
                    }
                    pFILE.Close();
                }
            }

            try
            {
                mEffect = WP7AppDriver.sWP7AppDriverInstance.mContentManager.Load<Effect>(text);
            }
            catch
            {
                mEffect = null;
            }
            return result || mEffect != null;
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
