namespace SexyFramework.Graphics
{
	public class PIParticleGroup
	{
		public PIParticleInstance mHead;

		public PIParticleInstance mTail;

		public int mCount;

		public bool mIsSuperEmitter;

		public bool mWasEmitted;

		public PIParticleGroup()
		{
			mIsSuperEmitter = false;
			mWasEmitted = false;
			mHead = null;
			mTail = null;
			mCount = 0;
		}
	}
}
