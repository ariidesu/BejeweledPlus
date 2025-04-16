namespace SexyFramework.Graphics
{
	public class PIParticleDefInstance
	{
		public float mNumberAcc;

		public float mCurNumberVariation;

		public int mParticlesEmitted;

		public int mTicks;

		public PIParticleDefInstance()
		{
			Reset();
		}

		public void Reset()
		{
			mNumberAcc = 0f;
			mCurNumberVariation = 0f;
			mParticlesEmitted = 0;
			mTicks = 0;
		}
	}
}
