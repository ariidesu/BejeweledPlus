using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Resource;

namespace BejeweledLivePlus.Misc
{
	public class QuestDataParser : DescParser
	{
		public List<QuestData> mQuestDataVector = new List<QuestData>();

		public override bool Error(string theError)
		{
			if (mError.Length == 0)
			{
				mError = theError;
				if (mCurrentLineNum > 0)
				{
					mError = mError + " on line " + mCurrentLineNum;
				}
			}
			return false;
		}

		public override bool HandleCommand(ListDataElement theParams)
		{
			string text = ((SingleDataElement)theParams.mElementVector[0]).mString.ToString();
			int count = theParams.mElementVector.Count;
			if (string.Compare(text, "Quest", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (count < 3)
				{
					return Error("Not enough params");
				}
				string text2 = Unquote(((SingleDataElement)theParams.mElementVector[1]).mString.ToString());
				QuestData questData = new QuestData();
				questData.mQuestName = text2;
				questData.mParams["Title"] = text2;
				if (count >= 3)
				{
					for (int i = 2; i < count; i++)
					{
						string key = Unquote(((SingleDataElement)theParams.mElementVector[i]).mString.ToString());
						string theQuotedString = ((SingleDataElement)((SingleDataElement)theParams.mElementVector[i]).mValue).mString.ToString();
						string value = Unquote(theQuotedString);
						questData.mParams[key] = value;
					}
				}
				if (questData.mParams.ContainsKey("TitleText"))
				{
					questData.mQuestName = questData.mParams["TitleText"];
				}
				else
				{
					questData.mParams["TitleText"] = text2;
				}
				mQuestDataVector.Add(questData);
				return true;
			}
			if (string.Compare(text, "QuickJumpQuestIdx", StringComparison.OrdinalIgnoreCase) == 0)
			{
				int mJumpToQuest = SexyFramework.GlobalMembers.sexyatoi(Unquote(((SingleDataElement)theParams.mElementVector[1]).mString.ToString()));
				GlobalMembers.gApp.mJumpToQuest = mJumpToQuest;
				return true;
			}
			return Error("Unknown command: " + text);
		}
	}
}
