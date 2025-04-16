using System.Collections.Generic;
using System.Text;

namespace SexyFramework.Resource
{
	public abstract class DescParser : EncodingParser
	{
		public enum ECMDSEP
		{
			CMDSEP_SEMICOLON = 1,
			CMDSEP_NO_INDENT
		}

		public int mCmdSep;

		public string mError = "";

		public int mCurrentLineNum;

		public StringBuilder mCurrentLine = new StringBuilder();

		public Dictionary<string, DataElement> mDefineMap = new Dictionary<string, DataElement>();

		public virtual bool Error(string theError)
		{
			mError = mError + "\n" + theError;
			return false;
		}

		public virtual DataElement Dereference(string theString)
		{
			string key = theString.ToUpper();
			if (mDefineMap.ContainsKey(key))
			{
				return mDefineMap[key];
			}
			return null;
		}

		public bool IsImmediate(string theString)
		{
			if ((theString[0] < '0' || theString[0] > '9') && theString[0] != '-' && theString[0] != '+' && theString[0] != '\'')
			{
				return theString[0] == '"';
			}
			return true;
		}

		public string Unquote(string theQuotedString)
		{
			if (theQuotedString[0] == '\'' || theQuotedString[0] == '"')
			{
				char c = theQuotedString[0];
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				bool flag2 = false;
				for (int i = 1; i < theQuotedString.Length - 1; i++)
				{
					if (flag2)
					{
						char c2 = theQuotedString[i];
						switch (c2)
						{
						case 'n':
							c2 = '\n';
							break;
						case 't':
							c2 = '\t';
							break;
						}
						stringBuilder.Append(c2);
						flag2 = false;
					}
					else if (theQuotedString[i] == c)
					{
						if (flag)
						{
							stringBuilder.Append(c);
						}
						flag = true;
					}
					else if (theQuotedString[i] == '\\')
					{
						flag2 = true;
						flag = false;
					}
					else
					{
						stringBuilder.Append(theQuotedString[i]);
						flag = false;
					}
				}
				return stringBuilder.ToString();
			}
			return theQuotedString;
		}

		public bool GetValues(ListDataElement theSource, ListDataElement theValues)
		{
			theValues.mElementVector.Clear();
			for (int i = 0; i < theSource.mElementVector.Count; i++)
			{
				if (theSource.mElementVector[i].mIsList)
				{
					ListDataElement listDataElement = new ListDataElement();
					theValues.mElementVector.Add(listDataElement);
					if (!GetValues((ListDataElement)theSource.mElementVector[i], listDataElement))
					{
						return false;
					}
					continue;
				}
				string text = ((SingleDataElement)theSource.mElementVector[i]).mString.ToString();
				if (text.Length <= 0)
				{
					continue;
				}
				if (text[0] == '\'' || text[0] == '"')
				{
					SingleDataElement item = new SingleDataElement(Unquote(text));
					theValues.mElementVector.Add(item);
					continue;
				}
				if (IsImmediate(text))
				{
					theValues.mElementVector.Add(new SingleDataElement(text));
					continue;
				}
				string key = text.ToUpper();
				if (mDefineMap.ContainsKey(key))
				{
					theValues.mElementVector.Add(mDefineMap[key].Duplicate());
					continue;
				}
				Error("Unable to Dereference \"" + text + "\"");
				return false;
			}
			return true;
		}

		public string DataElementToString(DataElement theDataElement, bool enclose)
		{
			if (theDataElement.mIsList)
			{
				ListDataElement listDataElement = (ListDataElement)theDataElement;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(enclose ? "(" : "");
				for (int i = 0; i < listDataElement.mElementVector.Count; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(enclose ? ", " : " ");
					}
					stringBuilder.Append(DataElementToString(listDataElement.mElementVector[i], true));
				}
				stringBuilder.Append(enclose ? ")" : "");
				return stringBuilder.ToString();
			}
			SingleDataElement singleDataElement = (SingleDataElement)theDataElement;
			if (singleDataElement.mValue != null)
			{
				return string.Concat(singleDataElement.mString, "=", DataElementToString(singleDataElement.mValue, true));
			}
			return singleDataElement.mString.ToString();
		}

		public bool DataToString(DataElement theSource, ref string theString)
		{
			theString = "";
			if (theSource.mIsList)
			{
				return false;
			}
			if (((SingleDataElement)theSource).mValue != null)
			{
				return false;
			}
			string text = ((SingleDataElement)theSource).mString.ToString();
			DataElement dataElement = Dereference(text);
			if (dataElement != null)
			{
				if (dataElement.mIsList)
				{
					return false;
				}
				theString = Unquote(((SingleDataElement)dataElement).mString.ToString());
			}
			else
			{
				theString = Unquote(text);
			}
			return true;
		}

		public bool DataToKeyAndValue(DataElement theSource, ref string theKey, ref DataElement theValue)
		{
			theKey = "";
			if (theSource.mIsList)
			{
				return false;
			}
			if (((SingleDataElement)theSource).mValue == null)
			{
				return false;
			}
			theValue = ((SingleDataElement)theSource).mValue;
			string text = ((SingleDataElement)theSource).mString.ToString();
			DataElement dataElement = Dereference(text);
			if (dataElement != null)
			{
				if (dataElement.mIsList)
				{
					return false;
				}
				theKey = Unquote(((SingleDataElement)dataElement).mString.ToString());
			}
			else
			{
				theKey = Unquote(text);
			}
			return true;
		}

		public bool DataToInt(DataElement theSource, ref int theInt)
		{
			theInt = 0;
			string theString = "";
			if (!DataToString(theSource, ref theString))
			{
				return false;
			}
			if (!Common.StringToInt(theString, ref theInt))
			{
				return false;
			}
			return true;
		}

		public bool DataToDouble(DataElement theSource, ref double theDouble)
		{
			theDouble = 0.0;
			string theString = "";
			if (!DataToString(theSource, ref theString))
			{
				return false;
			}
			if (!Common.StringToDouble(theString, ref theDouble))
			{
				return false;
			}
			return true;
		}

		public bool DataToBoolean(DataElement theSource, ref bool theBool)
		{
			theBool = false;
			string theString = "";
			if (!DataToString(theSource, ref theString))
			{
				return false;
			}
			switch (theString)
			{
			case "false":
			case "no":
			case "0":
				theBool = false;
				return true;
			case "true":
			case "yes":
			case "1":
				theBool = true;
				return true;
			default:
				return false;
			}
		}

		public bool DataToStringVector(DataElement theSource, ref List<string> theStringVector)
		{
			theStringVector.Clear();
			ListDataElement listDataElement = new ListDataElement();
			ListDataElement listDataElement2 = null;
			if (theSource.mIsList)
			{
				if (!GetValues((ListDataElement)theSource, listDataElement))
				{
					return false;
				}
				listDataElement2 = listDataElement;
			}
			else
			{
				string text = ((SingleDataElement)theSource).mString.ToString();
				DataElement dataElement = Dereference(text);
				if (dataElement == null)
				{
					Error("Unable to Dereference \"" + text + "\"");
					return false;
				}
				if (!dataElement.mIsList)
				{
					return false;
				}
				listDataElement2 = (ListDataElement)dataElement;
			}
			for (int i = 0; i < listDataElement2.mElementVector.Count; i++)
			{
				if (listDataElement2.mElementVector[i].mIsList)
				{
					theStringVector.Clear();
					return false;
				}
				SingleDataElement singleDataElement = (SingleDataElement)listDataElement2.mElementVector[i];
				theStringVector.Add(singleDataElement.mString.ToString());
			}
			return true;
		}

		public bool DataToList(DataElement theSource, ref ListDataElement theValues)
		{
			if (theSource.mIsList)
			{
				return GetValues((ListDataElement)theSource, theValues);
			}
			DataElement dataElement = Dereference(((SingleDataElement)theSource).mString.ToString());
			if (dataElement == null || !dataElement.mIsList)
			{
				return false;
			}
			ListDataElement listDataElement = (ListDataElement)dataElement;
			theValues = listDataElement;
			return true;
		}

		public bool DataToIntVector(DataElement theSource, ref List<int> theIntVector)
		{
			theIntVector.Clear();
			List<string> theStringVector = new List<string>();
			if (!DataToStringVector(theSource, ref theStringVector))
			{
				return false;
			}
			for (int i = 0; i < theStringVector.Count; i++)
			{
				int theIntVal = 0;
				if (!Common.StringToInt(theStringVector[i], ref theIntVal))
				{
					return false;
				}
				theIntVector.Add(theIntVal);
			}
			return true;
		}

		public bool DataToDoubleVector(DataElement theSource, ref List<double> theDoubleVector)
		{
			theDoubleVector.Clear();
			List<string> theStringVector = new List<string>();
			if (!DataToStringVector(theSource, ref theStringVector))
			{
				return false;
			}
			for (int i = 0; i < theStringVector.Count; i++)
			{
				double theDouble = 0.0;
				if (!Common.StringToDouble(theStringVector[i], ref theDouble))
				{
					return false;
				}
				theDoubleVector.Add(theDouble);
			}
			return true;
		}

		public bool ParseToList(string theString, ref ListDataElement theList, bool expectListEnd, ref int theStringPos)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			SingleDataElement singleDataElement = null;
			SingleDataElement singleDataElement2 = null;
			int num = 0;
			if (theStringPos == 0)
			{
				theStringPos = num;
			}
			while (theStringPos < theString.Length)
			{
				bool flag5 = false;
				char c = theString[theStringPos++];
				bool flag6 = c == ' ' || c == '\t' || c == '\n' || c == ',';
				if (flag3)
				{
					flag5 = true;
				}
				else
				{
					if (c == '\'' && !flag2)
					{
						flag = !flag;
					}
					else if (c == '"' && !flag)
					{
						flag2 = !flag2;
					}
					if (c == '\\')
					{
						flag3 = true;
					}
					else if (!flag && !flag2)
					{
						switch (c)
						{
						case ')':
							if (expectListEnd)
							{
								return true;
							}
							Error("Unexpected List End");
							return false;
						case '(':
						{
							if (flag4)
							{
								singleDataElement2 = null;
								flag4 = false;
							}
							if (singleDataElement2 != null)
							{
								Error("Unexpected List Start");
								return false;
							}
							ListDataElement theList2 = new ListDataElement();
							if (!ParseToList(theString, ref theList2, true, ref theStringPos))
							{
								return false;
							}
							if (singleDataElement != null)
							{
								singleDataElement.mValue = theList2;
								singleDataElement = null;
							}
							else
							{
								theList.mElementVector.Add(theList2);
							}
							break;
						}
						case '=':
							singleDataElement = singleDataElement2;
							flag4 = true;
							break;
						default:
							if (flag6)
							{
								if (singleDataElement2 != null && singleDataElement2.mString.Length > 0)
								{
									flag4 = true;
								}
								break;
							}
							if (flag4)
							{
								singleDataElement2 = null;
								flag4 = false;
							}
							flag5 = true;
							break;
						}
					}
					else
					{
						if (flag4)
						{
							singleDataElement2 = null;
							flag4 = false;
						}
						flag5 = true;
					}
				}
				if (!flag5)
				{
					continue;
				}
				if (singleDataElement2 == null)
				{
					singleDataElement2 = new SingleDataElement();
					if (singleDataElement != null)
					{
						singleDataElement.mValue = singleDataElement2;
						singleDataElement = null;
					}
					else
					{
						theList.mElementVector.Add(singleDataElement2);
					}
				}
				if (flag3)
				{
					singleDataElement2.mString.Append("\\");
					flag3 = false;
				}
				singleDataElement2.mString.Append(c);
			}
			if (flag)
			{
				Error("Unterminated Single Quotes");
				return false;
			}
			if (flag2)
			{
				Error("Unterminated Double Quotes");
				return false;
			}
			if (expectListEnd)
			{
				Error("Unterminated List");
				return false;
			}
			return true;
		}

		public bool ParseDescriptorLine(string theDescriptorLine)
		{
			ListDataElement theList = new ListDataElement();
			int theStringPos = 0;
			if (!ParseToList(theDescriptorLine, ref theList, false, ref theStringPos))
			{
				return false;
			}
			if (theList.mElementVector.Count > 0)
			{
				if (theList.mElementVector[0].mIsList)
				{
					Error("Missing Command");
					return false;
				}
				if (!HandleCommand(theList))
				{
					return false;
				}
			}
			return true;
		}

		public abstract bool HandleCommand(ListDataElement theParams);

		public DescParser()
		{
			mCmdSep = 1;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public virtual bool LoadDescriptor(string theFileName)
		{
			mCurrentLineNum = 0;
			int num = 0;
			bool flag = false;
			mError = "";
			mCurrentLine.Clear();
			if (!base.OpenFile(theFileName))
			{
				return Error("Unable to open file: " + theFileName);
			}
			while (!EndOfFile())
			{
				char theChar = '0';
				bool flag2 = false;
				bool flag3 = true;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				while (true)
				{
					switch (GetChar(ref theChar))
					{
					case GetCharReturnType.INVALID_CHARACTER:
						return Error("Invalid Character");
					default:
						return Error("Internal Error");
					case GetCharReturnType.SUCCESSFUL:
						switch (theChar)
						{
						case '\r':
							continue;
						case '\n':
							num++;
							break;
						}
						if ((theChar == ' ' || theChar == '\t') && flag3)
						{
							flag7 = true;
						}
						if (flag3 && (theChar == ' ' || theChar == '\t' || theChar == '\n'))
						{
							continue;
						}
						if (flag3)
						{
							if ((mCmdSep & 2) != 0 && !flag7 && mCurrentLine.Length > 0)
							{
								PutChar(theChar);
								break;
							}
							if (theChar == '#')
							{
								flag2 = true;
							}
							flag3 = false;
						}
						if (theChar == '\n')
						{
							flag7 = false;
							flag3 = true;
						}
						if (theChar == '\n' && flag2)
						{
							flag2 = false;
							continue;
						}
						if (flag2)
						{
							continue;
						}
						if (theChar == '\\' && (flag4 || flag5) && !flag6)
						{
							flag6 = true;
							continue;
						}
						if (theChar == '\'' && !flag5 && !flag6)
						{
							flag4 = !flag4;
						}
						if (theChar == '"' && !flag4 && !flag6)
						{
							flag5 = !flag5;
						}
						if (theChar != ';' || (mCmdSep & 1) == 0 || flag4 || flag5)
						{
							if (flag6)
							{
								mCurrentLine.Append('\\');
								flag6 = false;
							}
							if (mCurrentLine.Length == 0)
							{
								mCurrentLineNum = num + 1;
							}
							mCurrentLine.Append(theChar);
							continue;
						}
						break;
					case GetCharReturnType.END_OF_FILE:
						break;
					}
					break;
				}
				if (mCurrentLine.Length > 0)
				{
					if (!ParseDescriptorLine(mCurrentLine.ToString()))
					{
						flag = true;
						break;
					}
					mCurrentLine.Clear();
				}
			}
			mCurrentLine.Clear();
			mCurrentLineNum = 0;
			CloseFile();
			return !flag;
		}

		public virtual bool LoadDescriptor(byte[] buffer)
		{
			mCurrentLineNum = 0;
			int num = 0;
			bool flag = false;
			mError = "";
			mCurrentLine.Clear();
			if (buffer == null)
			{
				return Error("Unable to open file: ");
			}
			SetBytes(buffer);
			char theChar = '0';
			bool flag2 = false;
			bool flag3 = true;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			while (true)
			{
				switch (GetChar(ref theChar))
				{
				case GetCharReturnType.INVALID_CHARACTER:
					return Error("Invalid Character");
				default:
					return Error("Internal Error");
				case GetCharReturnType.SUCCESSFUL:
					switch (theChar)
					{
					case '\r':
						continue;
					case '\n':
						num++;
						break;
					}
					if ((theChar == ' ' || theChar == '\t') && flag3)
					{
						flag7 = true;
					}
					if (flag3 && (theChar == ' ' || theChar == '\t' || theChar == '\n'))
					{
						continue;
					}
					if (flag3)
					{
						if ((mCmdSep & 2) != 0 && !flag7 && mCurrentLine.Length > 0)
						{
							PutChar(theChar);
							break;
						}
						if (theChar == '#')
						{
							flag2 = true;
						}
						flag3 = false;
					}
					if (theChar == '\n')
					{
						flag7 = false;
						flag3 = true;
					}
					if (theChar == '\n' && flag2)
					{
						flag2 = false;
						continue;
					}
					if (flag2)
					{
						continue;
					}
					if (theChar == '\\' && (flag4 || flag5) && !flag6)
					{
						flag6 = true;
						continue;
					}
					if (theChar == '\'' && !flag5 && !flag6)
					{
						flag4 = !flag4;
					}
					if (theChar == '"' && !flag4 && !flag6)
					{
						flag5 = !flag5;
					}
					if (theChar != ';' || (mCmdSep & 1) == 0 || flag4 || flag5)
					{
						if (flag6)
						{
							mCurrentLine.Append('\\');
							flag6 = false;
						}
						if (mCurrentLine.Length == 0)
						{
							mCurrentLineNum = num + 1;
						}
						mCurrentLine.Append(theChar);
						continue;
					}
					break;
				case GetCharReturnType.END_OF_FILE:
					break;
				}
				break;
			}
			if (mCurrentLine.Length > 0)
			{
				if (!ParseDescriptorLine(mCurrentLine.ToString()))
				{
					flag = true;
				}
				mCurrentLine.Clear();
			}
			mCurrentLine.Clear();
			mCurrentLineNum = 0;
			return !flag;
		}

		public virtual bool LoadDescriptorBuffered(string theFileName)
		{
			mCurrentLineNum = 0;
			int num = 0;
			bool flag = false;
			mCurrentLine.Clear();
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			if (!base.OpenFile(theFileName))
			{
				return false;
			}
			while (!EndOfFile())
			{
				char theChar = '0';
				bool flag2 = false;
				bool flag3 = true;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				while (true)
				{
					switch (GetChar(ref theChar))
					{
					case GetCharReturnType.INVALID_CHARACTER:
						return false;
					default:
						return false;
					case GetCharReturnType.SUCCESSFUL:
						switch (theChar)
						{
						case '\r':
							continue;
						case '\n':
							num++;
							break;
						}
						if ((theChar == ' ' || theChar == '\t') && flag3)
						{
							flag7 = true;
						}
						if (flag3 && (theChar == ' ' || theChar == '\t' || theChar == '\n'))
						{
							continue;
						}
						if (flag3)
						{
							if ((mCmdSep & 2) != 0 && !flag7 && mCurrentLine.Length > 0)
							{
								PutChar(theChar);
								break;
							}
							if (theChar == '#')
							{
								flag2 = true;
							}
							flag3 = false;
						}
						if (theChar == '\n')
						{
							flag7 = false;
							flag3 = true;
						}
						if (theChar == '\n' && flag2)
						{
							flag2 = false;
							continue;
						}
						if (flag2)
						{
							continue;
						}
						if (theChar == '\\' && (flag4 || flag5) && !flag6)
						{
							flag6 = true;
							continue;
						}
						if (theChar == '\'' && !flag5 && !flag6)
						{
							flag4 = !flag4;
						}
						if (theChar == '"' && !flag4 && !flag6)
						{
							flag5 = !flag5;
						}
						if (theChar != ';' || (mCmdSep & 1) == 0 || flag4 || flag5)
						{
							if (flag6)
							{
								mCurrentLine.Append('\\');
								flag6 = false;
							}
							if (mCurrentLine.Length == 0)
							{
								mCurrentLineNum = num + 1;
							}
							mCurrentLine.Append(theChar);
							continue;
						}
						break;
					case GetCharReturnType.END_OF_FILE:
						break;
					}
					break;
				}
				if (mCurrentLine.Length > 0)
				{
					list.Add(mCurrentLine.ToString());
					list2.Add(mCurrentLineNum);
					mCurrentLine.Clear();
				}
			}
			mCurrentLine.Clear();
			mCurrentLineNum = 0;
			CloseFile();
			num = list.Count;
			for (int i = 0; i < num; i++)
			{
				mCurrentLineNum = list2[i];
				mCurrentLine.Clear();
				mCurrentLine.AppendLine(list[i]);
				if (!ParseDescriptorLine(mCurrentLine.ToString()))
				{
					flag = true;
					break;
				}
			}
			mCurrentLine.Clear();
			mCurrentLineNum = 0;
			return !flag;
		}
	}
}
