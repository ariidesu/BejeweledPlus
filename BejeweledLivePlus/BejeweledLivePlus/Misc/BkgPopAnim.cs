using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Misc
{
	public class BkgPopAnim : PopAnim
	{
		public Background mBackground;

		public BkgPopAnim(Background theBackground, int theId, PopAnimListener theListener)
			: base(theId, theListener)
		{
			mBackground = theBackground;
		}

		public override void Dispose()
		{
		}

		public override SharedImageRef Load_GetImageHook(string theFileDir, string theOrigName, string theRemappedName)
		{
			return base.Load_GetImageHook(theFileDir, theOrigName, theRemappedName);
		}
	}
}
