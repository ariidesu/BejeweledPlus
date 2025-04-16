using System.Collections.Generic;

namespace SexyFramework.Resource
{
	public class PropertiesParser
	{
		public SexyAppBase mApp;

		public XMLParser mXMLParser;

		public string mError = string.Empty;

		public bool mHasFailed;

		protected void Fail(string theErrorText)
		{
			if (!mHasFailed)
			{
				mHasFailed = true;
				int currentLineNum = mXMLParser.GetCurrentLineNum();
				mError = theErrorText;
				if (currentLineNum > 0)
				{
					mError = mError + " on Line " + currentLineNum;
				}
				if (mXMLParser.GetFileName().Length <= 0)
				{
					mError = mError + " in File " + mXMLParser.GetFileName();
				}
			}
		}

		protected bool ParseSingleElement(out string aString)
		{
			aString = string.Empty;
			while (true)
			{
				XMLElement xMLElement = new XMLElement();
				if (!mXMLParser.NextElement(xMLElement))
				{
					return false;
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
				{
					Fail(string.Concat("Unexpected Section: '", xMLElement.mValue, "'"));
					return false;
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
				{
					aString = xMLElement.mValue.ToString();
				}
				else if (xMLElement.mType == XMLElement.XMLElementType.TYPE_END)
				{
					break;
				}
			}
			return true;
		}

		protected bool ParseStringArray(List<string> theStringVector)
		{
			theStringVector.Clear();
			while (true)
			{
				XMLElement xMLElement = new XMLElement();
				if (!mXMLParser.NextElement(xMLElement))
				{
					return false;
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
				{
					if (!(xMLElement.mValue.ToString() == "String"))
					{
						Fail(string.Concat("Invalid Section '", xMLElement.mValue, "'"));
						return false;
					}
					string aString = "";
					if (!ParseSingleElement(out aString))
					{
						return false;
					}
					theStringVector.Add(aString);
				}
				else
				{
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
					{
						Fail(string.Concat("Element Not Expected '", xMLElement.mValue, "'"));
						return false;
					}
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_END)
					{
						break;
					}
				}
			}
			return true;
		}

		protected bool ParseProperties()
		{
			while (true)
			{
				XMLElement xMLElement = new XMLElement();
				if (!mXMLParser.NextElement(xMLElement))
				{
					return false;
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
				{
					if (xMLElement.mValue.ToString() == "String")
					{
						string aString = "";
						if (!ParseSingleElement(out aString))
						{
							return false;
						}
						string attribute = xMLElement.GetAttribute("id");
						mApp.SetString(attribute, aString);
					}
					else if (xMLElement.mValue.ToString() == "StringArray")
					{
						List<string> list = new List<string>();
						if (!ParseStringArray(list))
						{
							return false;
						}
						string attribute2 = xMLElement.GetAttribute("id");
						mApp.mStringVectorProperties[attribute2] = list;
					}
					else if (xMLElement.mValue.ToString() == "Boolean")
					{
						string aString2 = "";
						if (!ParseSingleElement(out aString2))
						{
							return false;
						}
						aString2 = aString2.ToUpper();
						bool flag = false;
						switch (aString2)
						{
						case "1":
						case "YES":
						case "ON":
						case "TRUE":
							flag = true;
							break;
						case "0":
						case "NO":
						case "OFF":
						case "FALSE":
							flag = false;
							break;
						default:
							Fail("Invalid Boolean Value: '" + aString2 + "'");
							return false;
						}
						string attribute3 = xMLElement.GetAttribute("id");
						mApp.SetBoolean(attribute3, flag);
					}
					else if (xMLElement.mValue.ToString() == "Integer")
					{
						string aString3 = "";
						if (!ParseSingleElement(out aString3))
						{
							return false;
						}
						int theIntVal = 0;
						if (!Common.StringToInt(aString3, ref theIntVal))
						{
							Fail("Invalid Integer Value: '" + aString3 + "'");
							return false;
						}
						string attribute4 = xMLElement.GetAttribute("id");
						mApp.SetInteger(attribute4, theIntVal);
					}
					else
					{
						if (!(xMLElement.mValue.ToString() == "Double"))
						{
							Fail(string.Concat("Invalid Section '", xMLElement.mValue, "'"));
							return false;
						}
						string aString4 = "";
						if (!ParseSingleElement(out aString4))
						{
							return false;
						}
						double theDouble = 0.0;
						if (!Common.StringToDouble(aString4, ref theDouble))
						{
							Fail("Invalid Double Value: '" + aString4 + "'");
							return false;
						}
						string attribute5 = xMLElement.GetAttribute("id");
						mApp.SetDouble(attribute5, theDouble);
					}
				}
				else
				{
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
					{
						Fail(string.Concat("Element Not Expected '", xMLElement.mValue, "'"));
						return false;
					}
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_END)
					{
						break;
					}
				}
			}
			return true;
		}

		protected bool DoParseProperties()
		{
			if (!mXMLParser.HasFailed())
			{
				while (true)
				{
					XMLElement xMLElement = new XMLElement();
					if (!mXMLParser.NextElement(xMLElement))
					{
						break;
					}
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
					{
						if (!(xMLElement.mValue.ToString() == "Properties"))
						{
							Fail(string.Concat("Invalid Section '", xMLElement.mValue, "'"));
							break;
						}
						if (!ParseProperties())
						{
							break;
						}
					}
					else if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
					{
						Fail(string.Concat("Element Not Expected '", xMLElement.mValue, "'"));
						break;
					}
				}
			}
			if (mXMLParser.HasFailed())
			{
				Fail(mXMLParser.GetErrorText());
			}
			mXMLParser = null;
			return !mHasFailed;
		}

		public PropertiesParser(SexyAppBase theApp)
		{
			mApp = theApp;
			mHasFailed = false;
		}

		public virtual void Dispose()
		{
		}

		public bool ParsePropertiesFile(string theFilename)
		{
			mXMLParser = new XMLParser();
			mXMLParser.OpenFile(theFilename);
			return DoParseProperties();
		}

		public bool ParsePropertiesBuffer(byte[] theBuffer)
		{
			mXMLParser = new XMLParser();
			mXMLParser.checkEncodingType(theBuffer);
			mXMLParser.SetBytes(theBuffer);
			return DoParseProperties();
		}

		public string GetErrorText()
		{
			return mError;
		}
	}
}
