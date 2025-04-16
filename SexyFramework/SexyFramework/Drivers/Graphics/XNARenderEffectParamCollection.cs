namespace SexyFramework.Drivers.Graphics
{
    public class XNARenderEffectParamCollection
    {
        public Dictionary<string, XNARenderEffectParamData> mParamMap;
        
        public XNARenderEffectParamData? GetParamNamed(string name, bool createIfMissing)
        {
            if (mParamMap.TryGetValue(name, out var paramData))
            {
                return paramData;
            }

            if (createIfMissing)
            {
                mParamMap[name] = new XNARenderEffectParamData { mFloatData = new List<float>() };
                return GetParamNamed(name, false);
            }

            return null;
        }
    }
}