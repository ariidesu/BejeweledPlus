using System.Text;
using SexyFramework;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public static class Common
	{
		public static MTRand CommonMTRand = new MTRand();

		public static int Rand()
		{
			return (int)CommonMTRand.Next();
		}

		public static int Rand(int range)
		{
			return (int)CommonMTRand.Next((uint)range);
		}

		public static float Rand(float range)
		{
			return CommonMTRand.Next(range);
		}

		public static void SRand(uint theSeed)
		{
			CommonMTRand.SRand(theSeed);
		}

		public static string GetAppDataFolder()
		{
			return SexyFramework.GlobalMembers.gFileDriver.GetSaveDataPath();
		}

		public static string URLEncode(string theString)
		{
			char[] array = new char[16]
			{
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
				'A', 'B', 'C', 'D', 'E', 'F'
			};
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < theString.Length; i++)
			{
				switch (theString[i])
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
				case '%':
				case '&':
				case '+':
				case '?':
					stringBuilder.Append('%');
					stringBuilder.Append(array[((int)theString[i] >> 4) & 0xF]);
					stringBuilder.Append(array[theString[i] & 0xF]);
					break;
				default:
					stringBuilder.Append(theString[i]);
					break;
				}
			}
			return stringBuilder.ToString();
		}

		public static bool Deltree(string thePath)
		{
			return SexyFramework.GlobalMembers.gFileDriver.DeleteTree(thePath);
		}

		public static bool FileExists(string theFileName)
		{
			bool isFolder = false;
			return FileExists(theFileName, ref isFolder);
		}

		public static bool FileExists(string theFileName, ref bool isFolder)
		{
			return SexyFramework.GlobalMembers.gFileDriver.FileExists(theFileName, ref isFolder);
		}

		public static string GetPathFrom(string theRelPath, string theDir)
		{
			return SexyFramework.Common.GetPathFrom(theRelPath, theDir);
		}

		public static string GetAppFullPath(string theAppRelPath)
		{
			return GetPathFrom(theAppRelPath, SexyFramework.GlobalMembers.gFileDriver.GetLoadDataPath());
		}

		public static void MkDir(string theDir)
		{
			SexyFramework.GlobalMembers.gFileDriver.MakeFolders(theDir);
		}

		public static string GetFileName(string thePath)
		{
			return GetFileName(thePath, false);
		}

		public static string GetFileName(string thePath, bool noExtension)
		{
			return SexyFramework.Common.GetFileName(thePath, noExtension);
		}

		public static string GetFileDir(string thePath)
		{
			return GetFileDir(thePath, false);
		}

		public static string GetFileDir(string thePath, bool withSlash)
		{
			return SexyFramework.Common.GetFileDir(thePath, withSlash);
		}

		public static long GetFileDate(string filename)
		{
			return SexyFramework.GlobalMembers.gFileDriver.GetFileTime(filename);
		}

		public static string GetCurDir()
		{
			return SexyFramework.GlobalMembers.gFileDriver.GetCurPath();
		}

		public static string GetFullPath(string theRelPath)
		{
			return GetPathFrom(theRelPath, GetCurDir());
		}
	}
}
