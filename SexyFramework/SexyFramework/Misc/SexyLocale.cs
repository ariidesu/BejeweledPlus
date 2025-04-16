using System.Globalization;
using System.Text;

namespace SexyFramework.Misc
{
	public static class SexyLocale
	{
		public static string gGrouping = "\\3";

		public static string gThousandSep = ", .";

		private static char CHAR_MAX = '\u007f';

		private static StringBuilder RS_builder = new StringBuilder();

		public static void SetSeperators(string theGrouping, string theSeperator)
		{
			gGrouping = theGrouping;
			gThousandSep = theSeperator;
		}

		public static void SetLocale(string theLocale)
		{
		}

		public static string StringToUpper(string theString)
		{
			return theString.ToUpper();
		}

		public static string StringToLower(string theString)
		{
			return theString.ToLower();
		}

		public static bool isalnum(char theChar)
		{
			return char.IsNumber(theChar);
		}

		public static string ReverseString(string str)
		{
			RS_builder.Clear();
			RS_builder.Append(str);
			int num = RS_builder.Length / 2;
			for (int i = 0; i < num; i++)
			{
				int index = RS_builder.Length - (i + 1);
				char value = RS_builder[i];
				RS_builder[i] = RS_builder[index];
				RS_builder[index] = value;
			}
			return RS_builder.ToString();
		}

		public static string CommaSeparate(int theValue)
		{
			if (theValue < 0)
			{
				return "-" + UCommaSeparate((uint)(-theValue));
			}
			return UCommaSeparate((uint)theValue);
		}

		public static string UCommaSeparate(uint theValue)
		{
			string text = theValue.ToString();
			char[] array = new char[text.Length + (text.Length - 1) / 3];
			char c = ',';
			switch (CultureInfo.CurrentCulture.Name)
			{
			case "en-US":
				c = gThousandSep[0];
				break;
			case "de-DE":
			case "es-ES":
			case "it-IT":
				c = gThousandSep[2];
				break;
			case "fr-FR":
				c = gThousandSep[1];
				break;
			default:
				c = gThousandSep[0];
				break;
			}
			int num = 0;
			int num2 = array.Length - 1;
			for (int num3 = text.Length - 1; num3 >= 0; num3--)
			{
				array[num2--] = text[num3];
				num++;
				if (num % 3 == 0 && num2 >= 0)
				{
					array[num2--] = c;
				}
			}
			return new string(array);
		}

		public static string CommaSeparate64(long theValue)
		{
			if (theValue < 0)
			{
				return "-" + UCommaSeparate64((ulong)(-theValue));
			}
			return UCommaSeparate64((ulong)theValue);
		}

		public static string UCommaSeparate64(ulong theValue)
		{
			string text = theValue.ToString();
			char[] array = new char[text.Length + (text.Length - 1) / 3];
			char c = ',';
			switch (CultureInfo.CurrentCulture.Name)
			{
			case "en-US":
				c = gThousandSep[0];
				break;
			case "de-DE":
			case "es-ES":
			case "it-IT":
				c = gThousandSep[2];
				break;
			case "fr-FR":
				c = gThousandSep[1];
				break;
			default:
				c = gThousandSep[0];
				break;
			}
			int num = 0;
			int num2 = array.Length - 1;
			for (int num3 = text.Length - 1; num3 >= 0; num3--)
			{
				array[num2--] = text[num3];
				num++;
				if (num % 3 == 0 && num2 >= 0)
				{
					array[num2--] = c;
				}
			}
			return new string(array);
		}
	}
}
