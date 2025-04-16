namespace SexyFramework.Resource
{
	public class SoundRes : BaseRes
	{
		public int mSoundId;

		public double mVolume;

		public int mPanning;

		public SoundRes()
		{
			mType = ResType.ResType_Sound;
			mSoundId = -1;
			mVolume = -1.0;
			mPanning = 0;
		}

		public override void DeleteResource()
		{
			if (mResourceRef != null && mResourceRef.HasResource())
			{
				mResourceRef.Release();
			}
			else if (mSoundId >= 0)
			{
				GlobalMembers.gSexyAppBase.mSoundManager.ReleaseSound(mSoundId);
			}
			mSoundId = -1;
			if (mGlobalPtr != null)
			{
				mGlobalPtr.mResObject = -1;
			}
		}

		public override void ApplyConfig()
		{
			if (mSoundId != -1 && (mResourceRef == null || !mResourceRef.HasResource()))
			{
				if (mVolume >= 0.0)
				{
					GlobalMembers.gSexyAppBase.mSoundManager.SetBaseVolume((uint)mSoundId, mVolume);
				}
				if (mPanning != 0)
				{
					GlobalMembers.gSexyAppBase.mSoundManager.SetBasePan((uint)mSoundId, mPanning);
				}
			}
		}
	}
}
