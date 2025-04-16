using System.Text;

namespace BejeweledLivePlus.Misc
{
	public static class GlobalMembersUtils
	{
		public static float GetRandFloat()
		{
			return (float)(Common.Rand() % 1000000 - 500000) / 500000f;
		}

		public static float GetRandFloatU()
		{
			return (float)(Common.Rand() % 1000000) / 1000000f;
		}

		public static string StripChar(string theString, char theChar)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in theString)
			{
				if (c != theChar)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static string FindToken(string theString)
		{
			bool flag = false;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			for (; i < theString.Length && char.IsWhiteSpace(theString[i]); i++)
			{
			}
			char c = '"';
			while (i < theString.Length)
			{
				if (flag)
				{
					if (theString[i] == c)
					{
						flag = false;
					}
				}
				else if (theString[i] == c)
				{
					flag = true;
				}
				else if (theString[i] == ',')
				{
					break;
				}
				stringBuilder.Append(theString[i++]);
			}
			return stringBuilder.ToString();
		}
	}
}
