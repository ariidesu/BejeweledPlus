using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.Profile
{
	public class UserProfile
	{
		private IProfileData mPlatformData;

		public UserProfile()
		{
			mPlatformData = IProfileData.CreateProfileData(this);
		}

		public virtual int GetId()
		{
			return mPlatformData.GetId();
		}

		public virtual string GetName()
		{
			return mPlatformData.GetName();
		}

		public virtual uint GetGamepadIndex()
		{
			return mPlatformData.GetGamepadIndex();
		}

		public virtual void SetGamepadIndex(uint gamepad)
		{
			mPlatformData.SetGamepadIndex(gamepad);
		}

		public virtual void Reset()
		{
		}

		public virtual void DeleteUserFiles()
		{
			mPlatformData.DeleteUserFiles();
		}

		public virtual EProfileIOState LoadDetails()
		{
			return mPlatformData.LoadDetails();
		}

		public virtual bool IsLoading()
		{
			return mPlatformData.IsLoading();
		}

		public virtual bool IsLoaded()
		{
			return mPlatformData.IsLoaded();
		}

		public virtual EProfileIOState SaveDetails()
		{
			return mPlatformData.SaveDetails();
		}

		public virtual bool IsSaving()
		{
			return mPlatformData.IsSaving();
		}

		public virtual bool IsSaved()
		{
			return mPlatformData.IsSaved();
		}

		public virtual bool HasError()
		{
			return mPlatformData.HasError();
		}

		public virtual bool SignedIn()
		{
			return mPlatformData.SignedIn();
		}

		public virtual bool IsSigningIn()
		{
			return mPlatformData.IsSigningIn();
		}

		public virtual bool IsOnline()
		{
			return mPlatformData.IsOnline();
		}

		public virtual bool ReadProfileSettings(Buffer theData)
		{
			theData.ReadInt32();
			return true;
		}

		public virtual bool WriteProfileSettings(Buffer theData)
		{
			theData.WriteInt32(1234);
			return true;
		}

		public bool IsAchievementUnlocked(uint id)
		{
			return mPlatformData.IsAchievementUnlocked(id);
		}

		public IAchievementContext StartUnlockAchievement(uint id)
		{
			return mPlatformData.StartUnlockAchievement(id);
		}

		public IProfileData GetPlatformData()
		{
			return mPlatformData;
		}
	}
}
