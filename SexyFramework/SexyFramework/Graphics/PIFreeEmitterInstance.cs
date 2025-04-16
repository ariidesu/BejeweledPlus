namespace SexyFramework.Graphics
{
	public class PIFreeEmitterInstance : PIParticleInstance
	{
		public PIEmitterBase mEmitter = new PIEmitterBase();

		public PIFreeEmitterInstance()
		{
			mEmitter.mParticleGroup.mWasEmitted = true;
		}

		public override void Dispose()
		{
			base.Dispose();
			mEmitter.Dispose();
		}
	}
}
