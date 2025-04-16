using System;
using SexyFramework.Resource;

namespace SexyFramework.Graphics
{
    public class RenderEffectDefinition : IDisposable
    {
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
            bool result = false;
            string text = Common.GetFileDir(inFileName, true) + Common.GetFileName(inFileName, true);
            PFILE pFILE = new PFILE(text, "rb");
            if (pFILE.Open())
            {
                byte[] data = pFILE.GetData();
                if (data != null)
                {
                    string text2 = string.Empty;
                    int num = text.LastIndexOf('.');
                    if (num >= 0)
                    {
                        text2 = text.Substring(num);
                    }
                    if (text2.Length > 1)
                    {
                        text2 = text2.Substring(1);
                    }
                    result = LoadFromMem(data.Length, data, inSrcFileName, text2);
                }
                pFILE.Close();
            }
            return result;
        }

        public virtual void Dispose()
        {
            mData = null;
        }
    }
}