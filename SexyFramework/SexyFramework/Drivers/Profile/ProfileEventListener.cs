using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.Profile
{
	public interface ProfileEventListener
	{
		uint GetProfileVersion();

		void NotifyProfileChanged(UserProfile player);

		UserProfile CreateUserProfile();

		void OnProfileLoad(UserProfile player, Buffer buffer);

		void OnProfileSave(UserProfile player, Buffer buffer);
	}
}
