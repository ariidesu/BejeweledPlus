using System.Text;

namespace SexyFramework.Resource
{
	public class XMLParser : EncodingParser
	{
		protected string mFileName = string.Empty;

		protected string mErrorText = string.Empty;

		protected int mLineNum;

		protected bool mHasFailed;

		protected bool mAllowComments;

		protected string mSection = string.Empty;

		protected void Fail(string theErrorText)
		{
			mHasFailed = true;
			mErrorText = theErrorText;
		}

		public void Init()
		{
			mSection = string.Empty;
			mLineNum = 1;
			mHasFailed = false;
			mErrorText = string.Empty;
		}

		protected bool AddAttribute(XMLElement theElement, string theAttributeKey, string theAttributeValue)
		{
			return theElement.AddAttribute(theAttributeKey, theAttributeValue);
		}

		protected bool AddAttributeEncoded(XMLElement theElement, string theAttributeKey, string theAttributeValue)
		{
			bool result = !theElement.mAttributesEncoded.ContainsKey(theAttributeKey);
			theElement.mAttributesEncoded[theAttributeKey] = theAttributeValue;
			return result;
		}

		public XMLParser()
		{
			mFile = null;
			mLineNum = 0;
			mAllowComments = false;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override bool OpenFile(string theFilename)
		{
			if (!base.OpenFile(theFilename))
			{
				mLineNum = 0;
				Fail("Unable to open file " + theFilename);
				return false;
			}
			mFileName = theFilename;
			Init();
			return true;
		}

		public virtual bool NextElement(XMLElement theElement)
		{
			do
			{
				theElement.mType = XMLElement.XMLElementType.TYPE_NONE;
				theElement.mSection = new StringBuilder(mSection);
				theElement.mInstruction = new StringBuilder("");
				theElement.mValue = new StringBuilder("");
				theElement.mValueEncoded = new StringBuilder("");
				theElement.ClearAttributes();
				theElement.mAttributesEncoded.Clear();
				theElement.mInstruction.Clear();
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				string text4 = string.Empty;
				while (true)
				{
					char theChar = '\0';
					int num = 0;
					switch (GetChar(ref theChar))
					{
					case GetCharReturnType.SUCCESSFUL:
						num = 1;
						break;
					case GetCharReturnType.INVALID_CHARACTER:
						Fail("Illegal Character");
						return false;
					case GetCharReturnType.FAILURE:
						Fail("Internal Error");
						return false;
					default:
						num = 0;
						break;
					}
					if (num == 1)
					{
						bool flag6 = false;
						if (theChar == '\n')
						{
							mLineNum++;
						}
						if (theElement.mType == XMLElement.XMLElementType.TYPE_COMMENT)
						{
							theElement.mInstruction.Append(theChar);
							int length = theElement.mInstruction.Length;
							if (theChar == '>' && length >= 3 && theElement.mInstruction[length - 2] == '-' && theElement.mInstruction[length - 3] == '-')
							{
								theElement.mInstruction.Remove(length - 3, 3);
								break;
							}
							continue;
						}
						if (theElement.mType == XMLElement.XMLElementType.TYPE_INSTRUCTION)
						{
							StringBuilder stringBuilder = theElement.mValue;
							if (theElement.mInstruction.Length != 0 || char.IsWhiteSpace(theChar))
							{
								stringBuilder = theElement.mInstruction;
							}
							stringBuilder.Append(theChar);
							int length2 = stringBuilder.Length;
							if (theChar == '>' && length2 >= 2 && stringBuilder[length2 - 2] == '?')
							{
								stringBuilder.Remove(length2 - 2, 2);
								break;
							}
							continue;
						}
						if (theChar == '"')
						{
							flag2 = !flag2;
							if (theElement.mType == XMLElement.XMLElementType.TYPE_NONE || theElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
							{
								flag6 = true;
							}
							if (!flag2)
							{
								flag3 = true;
							}
						}
						else if (!flag2)
						{
							if (theChar == '<')
							{
								if (theElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
								{
									PutChar(theChar);
									break;
								}
								if (theElement.mType != 0)
								{
									Fail("Unexpected '<'");
									return false;
								}
								theElement.mType = XMLElement.XMLElementType.TYPE_START;
							}
							else
							{
								if (theChar == '>')
								{
									if (theElement.mType == XMLElement.XMLElementType.TYPE_START)
									{
										bool flag7 = false;
										if (text == "/")
										{
											flag7 = true;
										}
										else
										{
											if (text.Length > 0)
											{
												text3 = Common.XMLDecodeString(text);
												text4 = text;
												AddAttribute(theElement, Common.XMLDecodeString(text), Common.XMLDecodeString(text2));
												AddAttributeEncoded(theElement, text, text2);
												text = string.Empty;
												text2 = string.Empty;
											}
											if (text3.Length > 0)
											{
												string attribute = theElement.GetAttribute(text3);
												int length3 = attribute.Length;
												if (length3 > 0 && attribute[length3 - 1] == '/')
												{
													AddAttribute(theElement, text3, Common.XMLDecodeString(attribute.Substring(0, length3 - 1)));
													flag7 = true;
												}
												attribute = theElement.mAttributesEncoded[text4];
												length3 = attribute.Length;
												if (length3 > 0 && attribute[length3 - 1] == '/')
												{
													AddAttributeEncoded(theElement, text4, attribute.Substring(0, length3 - 1));
													flag7 = true;
												}
											}
											else
											{
												int length4 = theElement.mValue.Length;
												if (length4 > 0 && theElement.mValue[length4 - 1] == '/')
												{
													theElement.mValue.Remove(length4 - 1, 1);
													flag7 = true;
												}
											}
										}
										if (flag7)
										{
											string theString = string.Concat("</", theElement.mValue, ">");
											PutString(theString);
											text = string.Empty;
										}
										if (mSection.Length != 0)
										{
											mSection += "/";
										}
										mSection += theElement.mValue.ToString();
										break;
									}
									if (theElement.mType == XMLElement.XMLElementType.TYPE_END)
									{
										int num2 = mSection.LastIndexOf('/');
										if (num2 == -1 && mSection.Length == 0)
										{
											Fail("Unexpected End");
											return false;
										}
										string text5 = mSection.Substring(num2 + 1);
										if (text5 != theElement.mValue.ToString())
										{
											Fail(string.Concat("End '", theElement.mValue, "' Doesn't Match Start '", text5, "'"));
											return false;
										}
										if (num2 == -1)
										{
											mSection = string.Empty;
										}
										else
										{
											mSection = mSection.Remove(num2);
										}
										break;
									}
									Fail("Unexpected '>'");
									return false;
								}
								if (theChar == '/' && theElement.mType == XMLElement.XMLElementType.TYPE_START && theElement.mValue.Length == 0)
								{
									theElement.mType = XMLElement.XMLElementType.TYPE_END;
								}
								else if (theChar == '?' && theElement.mType == XMLElement.XMLElementType.TYPE_START && theElement.mValue.Length == 0)
								{
									theElement.mType = XMLElement.XMLElementType.TYPE_INSTRUCTION;
								}
								else if (char.IsWhiteSpace(theChar))
								{
									if (theElement.mValue.Length != 0)
									{
										flag = true;
									}
									if (theElement.mType == XMLElement.XMLElementType.TYPE_START && theElement.mValue.ToString() == "!--")
									{
										theElement.mType = XMLElement.XMLElementType.TYPE_COMMENT;
									}
								}
								else
								{
									if (theChar <= ' ')
									{
										Fail("Illegal Character");
										return false;
									}
									flag6 = true;
								}
							}
						}
						else
						{
							flag6 = true;
						}
						if (!flag6)
						{
							continue;
						}
						if (theElement.mType == XMLElement.XMLElementType.TYPE_NONE)
						{
							theElement.mType = XMLElement.XMLElementType.TYPE_ELEMENT;
						}
						if (theElement.mType == XMLElement.XMLElementType.TYPE_START)
						{
							if (flag)
							{
								if (!flag4 || (!flag5 && theChar != '=') || (flag5 && (text2.Length > 0 || flag3)))
								{
									if (flag4)
									{
										AddAttribute(theElement, Common.XMLDecodeString(text), Common.XMLDecodeString(text2));
										AddAttributeEncoded(theElement, text, text2);
										text = string.Empty;
										text2 = string.Empty;
										text3 = string.Empty;
										text4 = string.Empty;
									}
									else
									{
										flag4 = true;
									}
									flag5 = false;
								}
								flag = false;
							}
							if (!flag4)
							{
								theElement.mValue.Append(theChar);
							}
							else if (theChar == '=')
							{
								flag5 = true;
								flag3 = false;
							}
							else if (!flag5)
							{
								text += theChar;
							}
							else
							{
								text2 += theChar;
							}
						}
						else
						{
							if (flag)
							{
								theElement.mValue.Append(" ");
								flag = false;
							}
							theElement.mValue.Append(theChar);
						}
						continue;
					}
					if (theElement.mType != 0)
					{
						Fail("Unexpected End of File");
					}
					return false;
				}
				if (text.Length > 0)
				{
					AddAttribute(theElement, Common.XMLDecodeString(text), Common.XMLDecodeString(text2));
					AddAttribute(theElement, text, text2);
				}
				theElement.mValueEncoded = new StringBuilder(theElement.mValue.ToString());
				string theString2 = theElement.mValue.ToString();
				theElement.mValue.Clear();
				theElement.mValue.Append(Common.XMLDecodeString(theString2));
			}
			while (theElement.mType == XMLElement.XMLElementType.TYPE_COMMENT && !mAllowComments);
			return true;
		}

		public string GetErrorText()
		{
			return mErrorText;
		}

		public int GetCurrentLineNum()
		{
			return mLineNum;
		}

		public string GetFileName()
		{
			return mFileName;
		}

		public override void SetStringSource(string theString)
		{
			Init();
			base.SetStringSource(theString);
		}

		public void AllowComments(bool doAllow)
		{
			mAllowComments = doAllow;
		}

		public bool HasFailed()
		{
			return mHasFailed;
		}
	}
}
