namespace SexyFramework.Drivers.Graphics
{
    public class XNARenderEffectParamData
    {
        public List<float> mFloatData;
        
        public void SetValue(float[] src, uint count)
        {
            if (mFloatData == null)
                mFloatData = new List<float>((int)count);
            else
                mFloatData.Clear();

            for (int i = 0; i < count && i < src.Length; i++)
            {
                mFloatData.Add(src[i]);
            }

            while (mFloatData.Count % 4 != 0)
            {
                mFloatData.Add(0.0f);
            }
        }
    }
}