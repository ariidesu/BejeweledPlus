using System.Collections.Generic;
using System.Linq;
using SexyFramework.File;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.Profile
{
	public class FilesystemProfileDriver : IProfileDriver
	{
		private Dictionary<string, UserProfile> mProfileMap = new Dictionary<string, UserProfile>();

		private uint mNextProfileId;

		private uint mNextProfileUseSeq;

		public FilesystemProfileDriver()
		{
			ClearProfiles();
		}

		public override bool Init()
		{
			if (GlobalMembers.gSexyAppBase.mProfileManager == null)
			{
				return false;
			}
			Load();
			return true;
		}

		public override void Update()
		{
		}

		public bool HasError()
		{
			return false;
		}

		public override uint GetNumProfiles()
		{
			return (uint)mProfileMap.Count;
		}

		public override bool HasProfile(string theName)
		{
			if (mProfileMap.ContainsKey(theName))
			{
				return mProfileMap[theName] != null;
			}
			return false;
		}

		public override UserProfile GetProfile(int index)
		{
			if (index >= mProfileMap.Count)
			{
				return null;
			}
			UserProfile userProfile = mProfileMap.Values.ElementAt(index);
			if (userProfile == null)
			{
				return null;
			}
			FilesystemProfileData filesystemProfileData = (FilesystemProfileData)userProfile.GetPlatformData();
			userProfile.LoadDetails();
			filesystemProfileData.SetUseSeq((int)mNextProfileUseSeq++);
			return userProfile;
		}

		public override UserProfile GetProfile(string theName)
		{
			if (mProfileMap.ContainsKey(theName))
			{
				UserProfile userProfile = mProfileMap[theName];
				if (userProfile == null)
				{
					return null;
				}
				FilesystemProfileData filesystemProfileData = (FilesystemProfileData)userProfile.GetPlatformData();
				userProfile.LoadDetails();
				filesystemProfileData.SetUseSeq((int)mNextProfileUseSeq++);
				return userProfile;
			}
			return null;
		}

		public override UserProfile GetAnyProfile()
		{
			return GetProfile(0);
		}

		public override UserProfile AddProfile(string theName)
		{
			if (mProfileMap.ContainsKey(theName))
			{
				return null;
			}
			UserProfile userProfile = GlobalMembers.gSexyAppBase.mProfileManager.CreateUserProfile();
			FilesystemProfileData filesystemProfileData = (FilesystemProfileData)userProfile.GetPlatformData();
			filesystemProfileData.SetName(theName);
			filesystemProfileData.SetId((int)mNextProfileId++);
			filesystemProfileData.SetUseSeq((int)mNextProfileUseSeq++);
			mProfileMap.Add(theName, userProfile);
			DeleteOldProfiles();
			Save();
			return userProfile;
		}

		public override bool DeleteProfile(string theName)
		{
			if (mProfileMap.ContainsKey(theName))
			{
				mProfileMap[theName].DeleteUserFiles();
				mProfileMap.Remove(theName);
				Save();
				return true;
			}
			return false;
		}

		public override bool RenameProfile(string theOldName, string theNewName)
		{
			if (!mProfileMap.ContainsKey(theOldName))
			{
				return false;
			}
			if (string.Compare(theOldName.ToLower(), theNewName.ToLower()) == 0)
			{
				FilesystemProfileData filesystemProfileData = (FilesystemProfileData)mProfileMap[theOldName].GetPlatformData();
				filesystemProfileData.SetName(theNewName);
				return true;
			}
			if (mProfileMap.ContainsKey(theNewName))
			{
				return false;
			}
			mProfileMap.Add(theNewName, mProfileMap[theOldName]);
			mProfileMap.Remove(theOldName);
			FilesystemProfileData filesystemProfileData2 = (FilesystemProfileData)mProfileMap[theNewName].GetPlatformData();
			filesystemProfileData2.SetName(theNewName);
			Save();
			return true;
		}

		public override void ClearProfiles()
		{
			mProfileMap.Clear();
			mNextProfileId = 1u;
			mNextProfileUseSeq = 1u;
		}

		protected void Load()
		{
			Buffer buffer = new Buffer();
			string theFileName = "userdata/users.dat";
			if (StorageFile.ReadBufferFromFile(theFileName, buffer) && !ReadState(buffer))
			{
				ClearProfiles();
			}
		}

		protected void Save()
		{
			Buffer buffer = new Buffer();
			if (WriteState(buffer))
			{
				StorageFile.MakeDir("userdata");
				string theFileName = "userdata/users.dat";
				StorageFile.WriteBufferToFile(theFileName, buffer);
			}
		}

		protected void DeleteOldestProfile()
		{
			if (mProfileMap.Count == 0)
			{
				return;
			}
			string text = null;
			FilesystemProfileData filesystemProfileData = null;
			foreach (KeyValuePair<string, UserProfile> item in mProfileMap)
			{
				if (text == null)
				{
					text = item.Key;
					filesystemProfileData = (FilesystemProfileData)item.Value.GetPlatformData();
					continue;
				}
				FilesystemProfileData filesystemProfileData2 = (FilesystemProfileData)item.Value.GetPlatformData();
				if (filesystemProfileData2.getUseSeq() < filesystemProfileData.getUseSeq())
				{
					text = item.Key;
					filesystemProfileData = filesystemProfileData2;
				}
			}
			mProfileMap[text].DeleteUserFiles();
			mProfileMap.Remove(text);
		}

		protected void DeleteOldProfiles()
		{
			while (mProfileMap.Count > 200)
			{
				DeleteOldestProfile();
			}
			Save();
		}

		protected bool ReadState(Buffer data)
		{
			int num = (int)data.ReadLong();
			if (num != GlobalMembers.gSexyAppBase.mProfileManager.GetProfileVersion())
			{
				return false;
			}
			mProfileMap.Clear();
			uint num2 = 0u;
			uint num3 = 0u;
			int num4 = data.ReadShort();
			for (int i = 0; i < num4; i++)
			{
				UserProfile userProfile = GlobalMembers.gSexyAppBase.mProfileManager.CreateUserProfile();
				FilesystemProfileData filesystemProfileData = (FilesystemProfileData)userProfile.GetPlatformData();
				if (!filesystemProfileData.ReadSummary(data))
				{
					return false;
				}
				if (filesystemProfileData.getUseSeq() > num2)
				{
					num2 = (uint)filesystemProfileData.getUseSeq();
				}
				if (userProfile.GetId() > num3)
				{
					num3 = (uint)userProfile.GetId();
				}
				mProfileMap.Add(userProfile.GetName(), userProfile);
			}
			mNextProfileId = num3 + 1;
			mNextProfileUseSeq = num2 + 1;
			return true;
		}

		protected bool WriteState(Buffer data)
		{
			data.WriteLong(GlobalMembers.gSexyAppBase.mProfileManager.GetProfileVersion());
			data.WriteShort((short)mProfileMap.Count);
			foreach (UserProfile value in mProfileMap.Values)
			{
				FilesystemProfileData filesystemProfileData = (FilesystemProfileData)value.GetPlatformData();
				if (!filesystemProfileData.WriteSummary(data))
				{
					return false;
				}
			}
			return true;
		}
	}
}
