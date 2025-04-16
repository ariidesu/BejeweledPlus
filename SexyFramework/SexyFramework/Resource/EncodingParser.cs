using System;
using System.Collections.Generic;
using System.Text;

namespace SexyFramework.Resource
{
	public class EncodingParser : IDisposable
	{
		public enum EncodingType
		{
			ASCII,
			UTF_8,
			UTF_16,
			UTF_16_LE,
			UTF_16_BE
		}

		public enum GetCharReturnType
		{
			SUCCESSFUL,
			INVALID_CHARACTER,
			END_OF_FILE,
			FAILURE
		}

		private delegate bool GetCharFunc(char theChar, ref bool error);

		protected PFILE mFile;

		private Stack<char> mBufferedText = new Stack<char>();

		private bool mForcedEncodingType;

		private EncodingType mEncodingType;

		public EncodingParser()
		{
			mFile = null;
			mForcedEncodingType = false;
			mEncodingType = EncodingType.UTF_8;
		}

		public virtual void Dispose()
		{
			mBufferedText.Clear();
			mBufferedText = null;
			if (mFile != null)
			{
				mFile.Close();
			}
		}

		public virtual void SetEncodingType(EncodingType theEncoding)
		{
			mEncodingType = theEncoding;
		}

		public virtual void checkEncodingType(byte[] data)
		{
			mEncodingType = EncodingType.ASCII;
			if (data.Length >= 2)
			{
				int num = data[0];
				int num2 = data[1];
				if ((num == 255 && num2 == 254) || (num == 254 && num2 == 255))
				{
					mEncodingType = EncodingType.UTF_16;
				}
			}
			if (mEncodingType == EncodingType.ASCII && data.Length >= 3)
			{
				int num3 = data[0];
				int num4 = data[1];
				int num5 = data[2];
				if (num3 == 239 && num4 == 187 && num5 == 191)
				{
					mEncodingType = EncodingType.UTF_8;
				}
			}
		}

		public virtual bool OpenFile(string theFilename)
		{
			mFile = new PFILE(theFilename, "rb");
			if (mFile.Open())
			{
				byte[] data = mFile.GetData();
				if (data == null)
				{
					return false;
				}
				if (!mForcedEncodingType)
				{
					checkEncodingType(data);
				}
				SetBytes(data);
				return true;
			}
			mFile = null;
			return false;
		}

		public virtual bool CloseFile()
		{
			if (mFile != null)
			{
				mFile.Close();
			}
			return true;
		}

		public virtual bool EndOfFile()
		{
			if (mBufferedText.Count > 0)
			{
				return false;
			}
			if (mFile != null)
			{
				return mFile.IsEndOfFile();
			}
			return true;
		}

		public virtual void SetStringSource(string theString)
		{
			int length = theString.Length;
			mBufferedText.Clear();
			for (int i = 0; i < length; i++)
			{
				mBufferedText.Push(theString[length - i - 1]);
			}
		}

		public virtual void SetBytes(byte[] data)
		{
			switch (mEncodingType)
			{
			case EncodingType.ASCII:
			{
				char[] array3 = new char[data.Length];
				for (int k = 0; k < data.Length; k++)
				{
					array3[k] = (char)data[k];
				}
				SetStringSource(new string(array3));
				break;
			}
			case EncodingType.UTF_8:
			{
				byte[] array2 = new byte[data.Length - 3];
				for (int j = 3; j < data.Length; j++)
				{
					array2[j - 3] = data[j];
				}
				char[] chars2 = Encoding.UTF8.GetChars(array2);
				SetStringSource(new string(chars2));
				break;
			}
			case EncodingType.UTF_16:
			{
				byte[] array = new byte[data.Length - 2];
				for (int i = 2; i < data.Length; i++)
				{
					array[i - 2] = data[i];
				}
				char[] chars = Encoding.Unicode.GetChars(array);
				SetStringSource(new string(chars));
				break;
			}
			case EncodingType.UTF_16_LE:
				throw new NotImplementedException();
			case EncodingType.UTF_16_BE:
				throw new NotImplementedException();
			}
		}

		public virtual GetCharReturnType GetChar(ref char theChar)
		{
			if (mBufferedText.Count != 0)
			{
				theChar = mBufferedText.Peek();
				mBufferedText.Pop();
				return GetCharReturnType.SUCCESSFUL;
			}
			if (mFile == null || (mFile != null && !mFile.Open()))
			{
				return GetCharReturnType.END_OF_FILE;
			}
			if (false)
			{
				return GetCharReturnType.INVALID_CHARACTER;
			}
			return GetCharReturnType.END_OF_FILE;
		}

		public virtual bool PutChar(char theChar)
		{
			mBufferedText.Push(theChar);
			return true;
		}

		public virtual bool PutString(string theString)
		{
			int length = theString.Length;
			for (int i = 0; i < length; i++)
			{
				mBufferedText.Push(theString[length - i - 1]);
			}
			return true;
		}
	}
}
