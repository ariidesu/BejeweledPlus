using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using SexyFramework.Misc;

namespace SexyFramework.File
{
	public class StorageFile
	{
		private FileMode m_nMode;

		private IsolatedStorageFile m_userStore = IsolatedStorageFile.GetUserStoreForApplication();

		private IsolatedStorageFileStream fp;

		public static void DeleteFile(string theFileName)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			userStoreForApplication.DeleteFile(theFileName);
		}

		public static bool FileExists(string theFileName)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			return userStoreForApplication.FileExists(theFileName);
		}

		public static void MakeDir(string theFolderName)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			if (!userStoreForApplication.DirectoryExists(theFolderName))
			{
				userStoreForApplication.CreateDirectory(theFolderName);
			}
		}

		public static bool ReadBufferFromFile(string theFileName, SexyFramework.Misc.Buffer theBuffer)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			if (!userStoreForApplication.FileExists(theFileName))
			{
				return false;
			}
			IsolatedStorageFileStream isolatedStorageFileStream = userStoreForApplication.OpenFile(theFileName, System.IO.FileMode.Open);
			if (isolatedStorageFileStream == null)
			{
				return false;
			}
			int num = (int)isolatedStorageFileStream.Length;
			byte[] array = new byte[num];
			isolatedStorageFileStream.Read(array, 0, num);
			theBuffer.Clear();
			theBuffer.SetData(array, num);
			isolatedStorageFileStream.Close();
			return true;
		}

		public static bool WriteBufferToFile(string theFileName, SexyFramework.Misc.Buffer theBuffer)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			IsolatedStorageFileStream isolatedStorageFileStream = userStoreForApplication.OpenFile(theFileName, System.IO.FileMode.Create);
			if (isolatedStorageFileStream == null)
			{
				return false;
			}
			byte[] dataPtr = theBuffer.GetDataPtr();
			isolatedStorageFileStream.Write(dataPtr, 0, dataPtr.Length);
			isolatedStorageFileStream.Close();
			return true;
		}

		public void clear()
		{
			if (m_nMode != 0)
			{
				close();
			}
			fp = null;
		}

		public void close()
		{
			if (fp != null)
			{
				fp.Close();
				fp = null;
				m_nMode = FileMode.MODE_NONE;
			}
		}

		public bool openRead(string fName)
		{
			return openRead(fName, false, true);
		}

		public bool openRead(string fName, bool bSilent, bool bFromDocs)
		{
			clear();
			if (!m_userStore.FileExists(fName))
			{
				return false;
			}
			fp = m_userStore.OpenFile(fName, System.IO.FileMode.Open);
			if (fp == null)
			{
				return false;
			}
			m_nMode = FileMode.MODE_READ;
			return true;
		}

		public bool getBool()
		{
			return getChar() == '\u0001';
		}

		public char getChar()
		{
			byte[] pData = null;
			read(ref pData, 1u);
			return (char)pData[0];
		}

		public short getShort()
		{
			byte[] pData = null;
			read(ref pData, 2u);
			if (BitConverter.IsLittleEndian)
			{
				pData = ReverseBytes(pData);
			}
			return BitConverter.ToInt16(pData, 0);
		}

		public int getInt()
		{
			byte[] pData = null;
			read(ref pData, 4u);
			if (BitConverter.IsLittleEndian)
			{
				pData = ReverseBytes(pData);
			}
			return BitConverter.ToInt32(pData, 0);
		}

		public ulong getLong()
		{
			byte[] pData = null;
			read(ref pData, 8u);
			if (BitConverter.IsLittleEndian)
			{
				pData = ReverseBytes(pData);
			}
			return BitConverter.ToUInt64(pData, 0);
		}

		public float getFloat()
		{
			byte[] pData = null;
			read(ref pData, 4u);
			if (BitConverter.IsLittleEndian)
			{
				pData = ReverseBytes(pData);
			}
			return BitConverter.ToSingle(pData, 0);
		}

		public bool getEof()
		{
			return fp.Position == fp.Length;
		}

		public int getStr(ref string s, int nBufferSize)
		{
			int num = 0;
			byte[] array = new byte[nBufferSize + 1];
			byte b = 0;
			while ((b = (byte)getChar()) != 0 && !getEof())
			{
				if (num < nBufferSize - 1)
				{
					array[num] = b;
					num++;
				}
				else
				{
					int num2 = nBufferSize - 1;
				}
			}
			array[num] = 0;
			s = Encoding.UTF8.GetString(array, 0, num);
			return num;
		}

		public bool openWrite(string name)
		{
			return openWrite(name, true);
		}

		public bool openWrite(string fName, bool bFromDocs)
		{
			clear();
			fp = m_userStore.OpenFile(fName, System.IO.FileMode.Create);
			if (fp == null)
			{
				return false;
			}
			m_nMode = FileMode.MODE_WRITE;
			return true;
		}

		public void setBool(bool b)
		{
			if (b)
			{
				setChar(1);
			}
			else
			{
				setChar(0);
			}
		}

		public void setChar(byte c)
		{
			write(new byte[1] { c }, 1u);
		}

		public void setShort(short s)
		{
			byte[] array = BitConverter.GetBytes(s);
			if (BitConverter.IsLittleEndian)
			{
				array = ReverseBytes(array);
			}
			write(array, 2u);
		}

		public void setInt(int i)
		{
			byte[] array = BitConverter.GetBytes(i);
			if (BitConverter.IsLittleEndian)
			{
				array = ReverseBytes(array);
			}
			write(array, 4u);
		}

		public void setLong(ulong l)
		{
			byte[] array = BitConverter.GetBytes(l);
			if (BitConverter.IsLittleEndian)
			{
				array = ReverseBytes(array);
			}
			write(array, 8u);
		}

		public void setFloat(float f)
		{
			byte[] array = BitConverter.GetBytes(f);
			if (BitConverter.IsLittleEndian)
			{
				array = ReverseBytes(array);
			}
			write(array, 4u);
		}

		public void setStr(string s)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			bool isLittleEndian = BitConverter.IsLittleEndian;
			int byteCount = Encoding.UTF8.GetByteCount(s);
			byte[] array = new byte[byteCount + 1];
			Array.Copy(bytes, array, byteCount);
			write(array, (uint)(byteCount + 1));
		}

		public void useBool(ref bool b)
		{
			if (m_nMode == FileMode.MODE_READ)
			{
				b = getBool();
			}
			else
			{
				setBool(b);
			}
		}

		public void useShort(ref short s)
		{
			if (m_nMode == FileMode.MODE_READ)
			{
				s = getShort();
			}
			else
			{
				setShort(s);
			}
		}

		public void useInt(ref int i)
		{
			if (m_nMode == FileMode.MODE_READ)
			{
				i = getInt();
			}
			else
			{
				setInt(i);
			}
		}

		public void useLong(ref ulong l)
		{
			if (m_nMode == FileMode.MODE_READ)
			{
				l = getLong();
			}
			else
			{
				setLong(l);
			}
		}

		public void useFloat(ref float f)
		{
			if (m_nMode == FileMode.MODE_READ)
			{
				f = getFloat();
			}
			else
			{
				setFloat(f);
			}
		}

		public void useStr(ref byte[] str, int nBufferSize)
		{
			throw new InvalidOperationException("TOTO_WP7 useStr() not implement!");
		}

		public void useBuffer(byte[] p, int nSize)
		{
			if (m_nMode == FileMode.MODE_READ)
			{
				read(ref p, (uint)nSize);
			}
			else
			{
				write(p, (uint)nSize);
			}
		}

		public int getFileSize()
		{
			return (int)(fp.Seek(0L, (SeekOrigin)fp.Length) - 12);
		}

		private void read(ref byte[] pData, uint nDataSize)
		{
			byte[] array = new byte[nDataSize];
			fp.Read(array, 0, (int)nDataSize);
			pData = array;
		}

		private void write(byte[] pData, uint nDataSize)
		{
			int num = 0;
			byte[] array = new byte[64];
			for (uint num2 = 0u; num2 < nDataSize; num2 += 64)
			{
				num = (int)(nDataSize - num2);
				num = ((num > 64) ? 64 : num);
				Array.Copy(pData, array, num);
				fp.Write(array, 0, num);
			}
		}

		private byte[] ReverseBytes(byte[] inArray)
		{
			int num = inArray.Length - 1;
			for (int i = 0; i < inArray.Length / 2; i++)
			{
				byte b = inArray[i];
				inArray[i] = inArray[num];
				inArray[num] = b;
				num--;
			}
			return inArray;
		}
	}
}
