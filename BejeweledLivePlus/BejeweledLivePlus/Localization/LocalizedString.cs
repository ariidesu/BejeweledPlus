using System.Collections.Generic;
using SexyFramework;

namespace BejeweledLivePlus.Localization
{
	public class LocalizedString : ILocalizedStringProvider
	{
		private Dictionary<int, string> idstrMap_;

		public LocalizedString()
		{
			GlobalMembers.gApp.mPopLoc.LocalizedString = this;
			idstrMap_ = new Dictionary<int, string>();
		}

		public string fromID(int id)
		{
			if (!idstrMap_.ContainsKey(id))
			{
				idstrMap_[id] = $"IDS_{id}";
			}
			return Strings.ResourceManager.GetString(idstrMap_[id], Strings.Culture);
		}
	}
}
