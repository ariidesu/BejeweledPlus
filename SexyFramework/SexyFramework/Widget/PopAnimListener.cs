using SexyFramework.Graphics;

namespace SexyFramework.Widget
{
	public interface PopAnimListener
	{
		void PopAnimPlaySample(string theSampleName, int thePan, double theVolume, double theNumSteps);

		PIEffect PopAnimLoadParticleEffect(string theEffectName);

		bool PopAnimObjectPredraw(int theId, SexyFramework.Graphics.Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor);

		bool PopAnimObjectPostdraw(int theId, SexyFramework.Graphics.Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor);

		ImagePredrawResult PopAnimImagePredraw(int theId, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Image theImage, SexyFramework.Graphics.Graphics g, int theDrawCount);

		void PopAnimStopped(int theId);

		void PopAnimCommand(int theId, string theCommand, string theParam);

		bool PopAnimCommand(int theId, PASpriteInst theSpriteInst, string theCommand, string theParam);
	}
}
