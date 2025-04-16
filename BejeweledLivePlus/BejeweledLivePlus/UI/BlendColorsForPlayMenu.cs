namespace BejeweledLivePlus.UI
{
	public class BlendColorsForPlayMenu
	{
		public static int BlendColors(int theFromColor, int theToColor, float theLerp, bool theBlendAlpha)
		{
			float num = 1f - theLerp;
			return (int)((!theBlendAlpha) ? (theFromColor & 0xFF000000u) : ((int)((float)(theFromColor >> 24) * num + (float)((theToColor >> 24) & 0xFF) * theLerp) << 24)) | ((int)((float)((theFromColor >> 16) & 0xFF) * num + (float)((theToColor >> 16) & 0xFF) * theLerp) << 16) | ((int)((float)((theFromColor >> 8) & 0xFF) * num + (float)((theToColor >> 8) & 0xFF) * theLerp) << 8) | (int)((float)(theFromColor & 0xFF) * num + (float)(theToColor & 0xFF) * theLerp);
		}
	}
}
