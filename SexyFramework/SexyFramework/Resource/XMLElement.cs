using System;
using System.Collections.Generic;
using System.Text;

namespace SexyFramework.Resource
{
	public class XMLElement
	{
		public enum XMLElementType
		{
			TYPE_NONE,
			TYPE_START,
			TYPE_END,
			TYPE_ELEMENT,
			TYPE_INSTRUCTION,
			TYPE_COMMENT
		}

		public XMLElementType mType;

		public StringBuilder mSection;

		public StringBuilder mValue;

		public StringBuilder mValueEncoded;

		public StringBuilder mInstruction;

		private Dictionary<string, string> mAttributes = new Dictionary<string, string>();

		public Dictionary<string, string> mAttributesEncoded = new Dictionary<string, string>();

		public bool GetAttributeBool(string theKey, bool theDefaultValue)
		{
			if (!HasAttribute(theKey))
			{
				return theDefaultValue;
			}
			string text = mAttributes[theKey];
			if (text.Length == 0)
			{
				return true;
			}
			if (string.Compare(text, "true", StringComparison.OrdinalIgnoreCase) == 0 || text == "1")
			{
				return true;
			}
			if (string.Compare(text, "false", StringComparison.OrdinalIgnoreCase) == 0 || text == "0")
			{
				return false;
			}
			return theDefaultValue;
		}

		public string GetAttribute(string key)
		{
			if (!HasAttribute(key))
			{
				return "";
			}
			return mAttributes[key];
		}

		public bool HasAttribute(string key)
		{
			return mAttributes.ContainsKey(key);
		}

		public Dictionary<string, string> GetAttributeMap()
		{
			return mAttributes;
		}

		public void ClearAttributes()
		{
			mAttributes.Clear();
		}

		public bool AddAttribute(string key, string value)
		{
			bool result = !HasAttribute(key);
			mAttributes[key] = value;
			return result;
		}
	}
}
