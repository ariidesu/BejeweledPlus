using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Resource;

namespace SexyFramework.Widget
{
	public class PopAnimModParser : DescParser
	{
		public int mPassNum;

		public PopAnim mPopAnim;

		public string mErrorHeader;

		public override bool Error(string theError)
		{
			if (GlobalMembers.gSexyApp != null)
			{
				string text = mErrorHeader + theError;
				if (mCurrentLine.Length > 0)
				{
					object obj = text;
					text = string.Concat(obj, " on Line ", mCurrentLineNum, ":\r\n\r\n", mCurrentLine);
				}
				GlobalMembers.gSexyAppBase.Popup(text);
			}
			return false;
		}

		public void SetParamHelper(PASpriteDef theSpriteDef, string theCmdName, int theCmdNum, string theNewParam)
		{
			if (theCmdNum == 0)
			{
				return;
			}
			for (int i = 0; i < theSpriteDef.mFrames.Length; i++)
			{
				PAFrame pAFrame = theSpriteDef.mFrames[i];
				for (int j = 0; j < pAFrame.mCommandVector.Length; j++)
				{
					PACommand pACommand = pAFrame.mCommandVector[j];
					if (!PopAnim.WildcardMatches(pACommand.mCommand, theCmdName))
					{
						continue;
					}
					if (theCmdNum <= 1)
					{
						pACommand.mParam = theNewParam;
					}
					if (theCmdNum >= 0)
					{
						theCmdNum--;
						if (theCmdNum == 0)
						{
							return;
						}
					}
				}
			}
		}

		public PopAnimModParser()
		{
			mCmdSep = 2;
		}

		public override bool HandleCommand(ListDataElement theParams)
		{
			string text = ((SingleDataElement)theParams.mElementVector[0]).mString.ToString();
			int num = theParams.mElementVector.Count - 1;
			switch (text)
			{
			case "SetPamFile":
				if (mPassNum != 1)
				{
					return true;
				}
				if (num != 1)
				{
					return Error("Invalid Number of Parameters");
				}
				if (mPopAnim.mModPamFile.Length == 0)
				{
					string theString3 = "";
					if (DataToString(theParams.mElementVector[1], ref theString3))
					{
						Error("Invalid Paramater Type");
					}
					mPopAnim.mModPamFile = theString3;
				}
				break;
			case "Remap":
			{
				if (mPassNum != 1)
				{
					return true;
				}
				if (num != 2)
				{
					return Error("Invalid Number of Parameters");
				}
				string theString4 = "";
				if (DataToString(theParams.mElementVector[1], ref theString4))
				{
					Error("Invalid Paramater Type");
				}
				string theString5 = "";
				if (DataToString(theParams.mElementVector[2], ref theString5))
				{
					Error("Invalid Paramater Type");
				}
				mPopAnim.Load_AddRemap(theString4, theString5);
				break;
			}
			case "Colorize":
			case "HueShift":
			{
				if (mPassNum != 2)
				{
					return true;
				}
				string theString6 = "";
				if (DataToString(theParams.mElementVector[1], ref theString6))
				{
					Error("Invalid Paramater Type");
				}
				bool flag = false;
				for (int j = 0; j < mPopAnim.mImageVector.Count; j++)
				{
					PAImage pAImage = mPopAnim.mImageVector[j];
					for (int k = 0; k < pAImage.mImages.Count; k++)
					{
						if (!PopAnim.WildcardMatches(pAImage.mImageName, theString6))
						{
							continue;
						}
						if (text == "Colorize")
						{
							List<int> theIntVector = new List<int>();
							if (DataToIntVector(theParams.mElementVector[2], ref theIntVector) || (theIntVector.Count != 3 && theIntVector.Count != 4))
							{
								Error("Invalid Paramater Type");
							}
							if (theIntVector.Count == 3)
							{
								new Color(theIntVector[0], theIntVector[1], theIntVector[2]);
							}
							else
							{
								new Color(theIntVector[0], theIntVector[1], theIntVector[2], theIntVector[3]);
							}
						}
						else
						{
							int theInt = 0;
							if (DataToInt(theParams.mElementVector[2], ref theInt))
							{
								return false;
							}
						}
						flag = true;
					}
				}
				if (flag)
				{
					return Error("Unable to locate specified element");
				}
				break;
			}
			case "SetParam":
			{
				if (mPassNum != 2)
				{
					return true;
				}
				if (num != 2)
				{
					return Error("Invalid Number of Parameters");
				}
				string theString = "";
				if (DataToString(theParams.mElementVector[1], ref theString))
				{
					Error("Invalid Paramater Type");
				}
				string theString2 = "";
				if (DataToString(theParams.mElementVector[2], ref theString2))
				{
					Error("Invalid Paramater Type");
				}
				int theCmdNum = -1;
				int num2 = theString.IndexOf('[');
				if (num2 != -1)
				{
					theCmdNum = int.Parse(theString) + num2 + 1;
					theString = theString.Substring(0, num2);
				}
				SetParamHelper(mPopAnim.mMainAnimDef.mMainSpriteDef, theString, theCmdNum, theString2);
				for (int i = 0; i < mPopAnim.mMainAnimDef.mSpriteDefVector.Count; i++)
				{
					SetParamHelper(mPopAnim.mMainAnimDef.mSpriteDefVector[i], theString, theCmdNum, theString2);
				}
				break;
			}
			default:
				Error("Unknown Command");
				return false;
			}
			return true;
		}
	}
}
