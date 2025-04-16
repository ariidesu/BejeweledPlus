namespace SexyFramework.Graphics
{
	public enum ImageFlags
	{
		ImageFlag_NONE = 0,
		ImageFlag_MinimizeNumSubdivisions = 1,
		ImageFlag_Use64By64Subdivisions = 2,
		ImageFlag_UseA4R4G4B4 = 4,
		ImageFlag_UseA8R8G8B8 = 8,
		ImageFlag_RenderTarget = 16,
		ImageFlag_CubeMap = 32,
		ImageFlag_VolumeMap = 64,
		ImageFlag_NoTriRep = 128,
		ImageFlag_NoQuadRep = 128,
		ImageFlag_RTUseDefaultRenderMode = 256,
		ImageFlag_Atlas = 512,
		REFLECT_ATTR_ENUM_FLAGS = 513
	}
}
