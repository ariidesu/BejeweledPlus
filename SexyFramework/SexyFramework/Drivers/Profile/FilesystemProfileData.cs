using SexyFramework.File;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.Profile
{
	public class FilesystemProfileData : IProfileData
	{
		private UserProfile mPlayer;

		private int mId = -1;

		private int mUseSeq = -1;

		private string mName = "";

		private uint mGamepad;

		private bool mLoaded;

		private bool mSaved;

		public FilesystemProfileData(UserProfile player)
		{
			mPlayer = player;
		}

		public override int GetId()
		{
			return mId;
		}

		public override string GetName()
		{
			return mName;
		}

		public override uint GetGamepadIndex()
		{
			return mGamepad;
		}

		public override void SetGamepadIndex(uint gamepad)
		{
			mGamepad = gamepad;
		}

		public override bool SignedIn()
		{
			return true;
		}

		public override bool IsSigningIn()
		{
			return false;
		}

		public override bool IsOnline()
		{
			return false;
		}

		public override void DeleteUserFiles()
		{
			string theFileName = $"userdata/user{GetId()}.dat";
			StorageFile.DeleteFile(theFileName);
		}

		public override EProfileIOState LoadDetails()
		{
			Buffer buffer = new Buffer();
			string theFileName = $"userdata/user{GetId()}.dat";
			if (!StorageFile.ReadBufferFromFile(theFileName, buffer))
			{
				if (!StorageFile.FileExists(theFileName))
				{
					mPlayer.Reset();
					mLoaded = true;
					return EProfileIOState.PROFILE_IO_SUCCESS;
				}
				mLoaded = false;
				return EProfileIOState.PROFILE_IO_ERROR;
			}
			if (!mPlayer.ReadProfileSettings(buffer))
			{
				mLoaded = false;
				return EProfileIOState.PROFILE_IO_ERROR;
			}
			mLoaded = true;
			return EProfileIOState.PROFILE_IO_SUCCESS;
		}

		public override bool IsLoading()
		{
			return false;
		}

		public override bool IsLoaded()
		{
			return mLoaded;
		}

		public override EProfileIOState SaveDetails()
		{
			Buffer buffer = new Buffer();
			if (!mPlayer.WriteProfileSettings(buffer))
			{
				mSaved = false;
				return EProfileIOState.PROFILE_IO_ERROR;
			}
			StorageFile.MakeDir("userdata");
			string theFileName = $"userdata/user{GetId()}.dat";
			if (!StorageFile.WriteBufferToFile(theFileName, buffer))
			{
				mSaved = false;
				return EProfileIOState.PROFILE_IO_ERROR;
			}
			mSaved = true;
			return EProfileIOState.PROFILE_IO_SUCCESS;
		}

		public override bool IsSaving()
		{
			return false;
		}

		public override bool IsSaved()
		{
			return mSaved;
		}

		public override bool HasError()
		{
			return false;
		}

		public bool ReadSummary(Buffer data)
		{
			mName = data.ReadString();
			mId = (int)data.ReadLong();
			mUseSeq = (int)data.ReadLong();
			return true;
		}

		public bool WriteSummary(Buffer data)
		{
			data.WriteString(mName);
			data.WriteLong(mId);
			data.WriteLong(mUseSeq);
			return true;
		}

		public override Image GetPlayerIcon()
		{
			return null;
		}

		public override bool IsAchievementUnlocked(uint id)
		{
			return false;
		}

		public override IAchievementContext StartUnlockAchievement(uint id)
		{
			return null;
		}

		public void SetId(int id)
		{
			mId = id;
		}

		public void SetUseSeq(int useSeq)
		{
			mUseSeq = useSeq;
		}

		public void SetName(string name)
		{
			mName = name;
		}

		public int getId()
		{
			return mId;
		}

		public int getUseSeq()
		{
			return mUseSeq;
		}

		public string getName()
		{
			return mName;
		}
	}
}
