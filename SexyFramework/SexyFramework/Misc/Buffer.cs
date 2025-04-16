using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SexyFramework.Misc
{
	public class Buffer
	{
		public enum SeekMode
		{
			eReadStart,
			eReadForward,
			eReadBack,
			eReadEnd,
			eWriteStart,
			eWriteForward,
			eWriteBack,
			eWriteEnd
		}

		public int mDataBitSize;

		public int mReadBitPos;

		public int mWriteBitPos;

		public List<byte> mData;

		private static StringBuilder strReader_ = new StringBuilder();

		private static ushort[] aMaskData = new ushort[5] { 192, 224, 240, 248, 252 };

		public Buffer()
		{
			mData = new List<byte>();
		}

		public void SeekFront()
		{
			mReadBitPos = 0;
		}

		public void Clear()
		{
			mReadBitPos = 0;
			mWriteBitPos = 0;
			mDataBitSize = 0;
		}

		public int GetCurrReadBytePos()
		{
			if (mReadBitPos != 0)
			{
				return (mReadBitPos + 7) / 8;
			}
			return 0;
		}

		public int GetCurrWriteBytePos()
		{
			if (mWriteBitPos != 0)
			{
				return (mWriteBitPos + 7) / 8;
			}
			return 0;
		}

		public void SeekReadBit(int pos)
		{
			mReadBitPos = pos;
		}

		public void SeekReadByte(int pos)
		{
			SeekReadBit(pos * 8);
		}

		public void SeekWriteBit(int pos)
		{
			mWriteBitPos = pos;
		}

		public void SeekWriteByte(int pos)
		{
			SeekWriteBit(pos * 8);
		}

		public void Seek(SeekMode mode)
		{
			Seek(mode, 0);
		}

		public void Seek(SeekMode mode, int bytePos)
		{
			switch (mode)
			{
			case SeekMode.eReadStart:
				SeekReadByte(0);
				break;
			case SeekMode.eReadForward:
				SeekReadByte(GetCurrReadBytePos() + bytePos);
				break;
			case SeekMode.eReadBack:
				SeekReadByte(GetCurrReadBytePos() - bytePos);
				break;
			case SeekMode.eReadEnd:
				SeekReadByte(GetDataLen());
				break;
			case SeekMode.eWriteStart:
				SeekWriteByte(0);
				break;
			case SeekMode.eWriteForward:
				SeekWriteByte(GetCurrWriteBytePos() + bytePos);
				break;
			case SeekMode.eWriteBack:
				SeekWriteByte(GetCurrWriteBytePos() - bytePos);
				break;
			case SeekMode.eWriteEnd:
				SeekWriteByte(GetDataLen());
				break;
			}
		}

		public void Resize(int bytes)
		{
			Clear();
			mDataBitSize = bytes * 8;
			Common.Resize(mData, bytes);
		}

		public void FromWebString(string theString)
		{
			throw new NotImplementedException();
		}

		public string ToWebString()
		{
			throw new NotImplementedException();
		}

		public string UTF8ToWideString()
		{
			byte[] dataPtr = GetDataPtr();
			int num = GetDataLen();
			bool flag = true;
			string text = "";
			int num2 = 0;
			while (num > 0)
			{
				string theChar = "";
				int nextUTF8CharFromStream = GetNextUTF8CharFromStream(dataPtr, num2, num, ref theChar);
				if (nextUTF8CharFromStream == 0)
				{
					break;
				}
				num -= nextUTF8CharFromStream;
				num2 += nextUTF8CharFromStream;
				if (flag)
				{
					flag = false;
					if (theChar[0] == '\ufeff')
					{
						continue;
					}
				}
				text += theChar;
			}
			return text;
		}

		public void WriteByte(byte theByte)
		{
			if (mWriteBitPos >= mDataBitSize)
			{
				if (mWriteBitPos % 8 == 0)
				{
					mData.Add(theByte);
				}
				else
				{
					int num = mWriteBitPos % 8;
					mData[mWriteBitPos / 8] |= (byte)(theByte << num);
					mData.Add((byte)(theByte >> 8 - num));
				}
			}
			else if (mData.Count != 0 && GetCurrWriteBytePos() < mData.Count - 1)
			{
				int currWriteBytePos = GetCurrWriteBytePos();
				if (mWriteBitPos % 8 == 0)
				{
					mData[currWriteBytePos] = theByte;
				}
				else
				{
					int num2 = mWriteBitPos % 8;
					mData[currWriteBytePos] |= (byte)(theByte << num2);
					int num3 = currWriteBytePos + 1;
					if (num3 >= mData.Count)
					{
						mData.Add((byte)(theByte >> 8 - num2));
					}
					else
					{
						mData[num3] = (byte)(theByte >> 8 - num2);
					}
				}
			}
			mWriteBitPos += 8;
			if (mWriteBitPos > mDataBitSize)
			{
				mDataBitSize = mWriteBitPos;
			}
		}

		public void WriteNumBits(int theNum, int theBits)
		{
			for (int i = 0; i < theBits; i++)
			{
				if (mWriteBitPos % 8 == 0)
				{
					mData.Add(0);
				}
				if ((theNum & (1 << i)) != 0)
				{
					mData[mWriteBitPos / 8] |= (byte)(1 << mWriteBitPos % 8);
				}
				mWriteBitPos++;
			}
			if (mWriteBitPos > mDataBitSize)
			{
				mDataBitSize = mWriteBitPos;
			}
		}

		public static int GetBitsRequired(int theNum, bool isSigned)
		{
			if (theNum < 0)
			{
				theNum = -theNum - 1;
			}
			int i;
			for (i = 0; theNum >= 1 << i; i++)
			{
			}
			if (isSigned)
			{
				i++;
			}
			return i;
		}

		public void WriteBoolean(bool theBool)
		{
			WriteByte((byte)(theBool ? 1 : 0));
		}

		public void WriteShort(short theShort)
		{
			WriteByte((byte)theShort);
			WriteByte((byte)(theShort >> 8));
		}

		public void WriteLong(long theLong)
		{
			WriteByte((byte)theLong);
			WriteByte((byte)(theLong >> 8));
			WriteByte((byte)(theLong >> 16));
			WriteByte((byte)(theLong >> 24));
		}

		public void WriteFloat(float theFloat)
		{
			byte[] bytes = BitConverter.GetBytes(theFloat);
			WriteBytes(bytes, 4);
		}

		public void WriteDouble(double theDouble)
		{
			byte[] bytes = BitConverter.GetBytes(theDouble);
			WriteBytes(bytes, 8);
		}

		public void WriteInt8(int theInt8)
		{
			WriteByte((byte)theInt8);
		}

		public void WriteInt16(short theInt16)
		{
			WriteByte((byte)theInt16);
			WriteByte((byte)(theInt16 >> 8));
		}

		public void WriteInt32(int theInt32)
		{
			WriteByte((byte)theInt32);
			WriteByte((byte)(theInt32 >> 8));
			WriteByte((byte)(theInt32 >> 16));
			WriteByte((byte)(theInt32 >> 24));
		}

		public void WriteInt64(long theInt64)
		{
			WriteByte((byte)theInt64);
			WriteByte((byte)(theInt64 >> 8));
			WriteByte((byte)(theInt64 >> 16));
			WriteByte((byte)(theInt64 >> 24));
			WriteByte((byte)(theInt64 >> 32));
			WriteByte((byte)(theInt64 >> 40));
			WriteByte((byte)(theInt64 >> 48));
			WriteByte((byte)(theInt64 >> 56));
		}

		public void WriteTransform2D(SexyTransform2D theTrans)
		{
			WriteFloat(theTrans.m00);
			WriteFloat(theTrans.m01);
			WriteFloat(theTrans.m02);
			WriteFloat(theTrans.m10);
			WriteFloat(theTrans.m11);
			WriteFloat(theTrans.m12);
			WriteFloat(theTrans.m20);
			WriteFloat(theTrans.m21);
			WriteFloat(theTrans.m22);
		}

		public void WriteFPoint(Vector2 thePoint)
		{
			WriteDouble(thePoint.X);
			WriteDouble(thePoint.Y);
		}

		public void WriteString(string theString)
		{
			WriteShort((short)theString.Length);
			for (int i = 0; i < theString.Length; i++)
			{
				WriteByte((byte)theString[i]);
			}
		}

		public void WriteUTF8String(string theString)
		{
			throw new NotSupportedException();
		}

		public void WriteSexyString(string theString)
		{
			throw new NotSupportedException();
		}

		public void WriteLine(string theString)
		{
			throw new NotSupportedException();
		}

		public void WriteBuffer(List<byte> theBuffer)
		{
			WriteLong(theBuffer.Count);
			for (int i = 0; i < theBuffer.Count; i++)
			{
				WriteByte(theBuffer[i]);
			}
		}

		public void WriteBuffer(Buffer theBuffer)
		{
			WriteBuffer(theBuffer.mData);
		}

		public void WriteBytes(byte[] theByte, int theCount)
		{
			for (int i = 0; i < theCount; i++)
			{
				WriteByte(theByte[i]);
			}
		}

		public void SetData(List<byte> theBuffer)
		{
			mData.Clear();
			mData = null;
			mData = new List<byte>(theBuffer);
			mDataBitSize = mData.Count * 8;
		}

		public void SetData(byte[] thePtr, int theCount)
		{
			mData.Clear();
			for (int i = 0; i < theCount; i++)
			{
				WriteByte(thePtr[i]);
			}
		}

		public byte ReadByte()
		{
			if ((mReadBitPos + 7) / 8 >= mData.Count)
			{
				return 0;
			}
			if (mReadBitPos % 8 == 0)
			{
				byte result = mData[mReadBitPos / 8];
				mReadBitPos += 8;
				return result;
			}
			int num = mReadBitPos % 8;
			byte b = 0;
			b = (byte)(mData[mReadBitPos / 8] >> num);
			b |= (byte)(mData[mReadBitPos / 8 + 1] << 8 - num);
			mReadBitPos += 8;
			return b;
		}

		public int ReadNumBits(int theBits, bool isSigned)
		{
			int count = mData.Count;
			int num = 0;
			bool flag = false;
			for (int i = 0; i < theBits; i++)
			{
				int num2 = mReadBitPos / 8;
				if (num2 >= count)
				{
					break;
				}
				if (flag = (mData[num2] & (1 << mReadBitPos % 8)) != 0)
				{
					num |= 1 << i;
				}
				mReadBitPos++;
			}
			if (isSigned && flag)
			{
				for (int j = theBits; j < 32; j++)
				{
					num |= 1 << j;
				}
			}
			return num;
		}

		public bool ReadBoolean()
		{
			return ReadByte() != 0;
		}

		public short ReadShort()
		{
			byte[] theData = new byte[2];
			ReadBytes(ref theData, 2);
			return BitConverter.ToInt16(theData, 0);
		}

		public long ReadLong()
		{
			return ReadInt32();
		}

		public float ReadFloat()
		{
			byte[] theData = new byte[4];
			ReadBytes(ref theData, 4);
			return BitConverter.ToSingle(theData, 0);
		}

		public double ReadDouble()
		{
			byte[] theData = new byte[8];
			ReadBytes(ref theData, 8);
			return BitConverter.ToDouble(theData, 0);
		}

		public byte ReadInt8()
		{
			return ReadByte();
		}

		public short ReadInt16()
		{
			return ReadShort();
		}

		public int ReadInt32()
		{
			byte[] theData = new byte[4];
			ReadBytes(ref theData, 4);
			return BitConverter.ToInt32(theData, 0);
		}

		public long ReadInt64()
		{
			byte[] theData = new byte[8];
			ReadBytes(ref theData, 8);
			return BitConverter.ToInt64(theData, 0);
		}

		public SexyTransform2D ReadTransform2D()
		{
			SexyTransform2D result = new SexyTransform2D(false);
			result.m00 = ReadFloat();
			result.m01 = ReadFloat();
			result.m02 = ReadFloat();
			result.m10 = ReadFloat();
			result.m11 = ReadFloat();
			result.m12 = ReadFloat();
			result.m20 = ReadFloat();
			result.m21 = ReadFloat();
			result.m22 = ReadFloat();
			return result;
		}

		public FPoint ReadFPoint()
		{
			FPoint result = default(FPoint);
			result.mX = (float)ReadDouble();
			result.mY = (float)ReadDouble();
			return result;
		}

		public Vector2 ReadVector2()
		{
			Vector2 result = default(Vector2);
			result.X = (float)ReadDouble();
			result.Y = (float)ReadDouble();
			return result;
		}

		public string ReadString()
		{
			strReader_.Clear();
			int num = ReadShort();
			for (int i = 0; i < num; i++)
			{
				strReader_.Append((char)ReadByte());
			}
			return strReader_.ToString();
		}

		public string ReadUTF8String()
		{
			if ((mReadBitPos & 7) != 0)
			{
				mReadBitPos = (mReadBitPos + 8) & -8;
			}
			string text = "";
			int num = ReadShort();
			if (num == 0)
			{
				return "";
			}
			int num2 = mReadBitPos / 8;
			byte[] theBuffer = mData.ToArray();
			int num3 = (mDataBitSize - mReadBitPos) / 8;
			int num4 = 0;
			num4 = 0;
			while (num3 > 0 && num4 < num)
			{
				string theChar = "";
				int nextUTF8CharFromStream = GetNextUTF8CharFromStream(theBuffer, num2, num3, ref theChar);
				if (nextUTF8CharFromStream == 0)
				{
					break;
				}
				num2 += nextUTF8CharFromStream;
				num3 -= nextUTF8CharFromStream;
				mReadBitPos += 8 * nextUTF8CharFromStream;
				text += theChar;
				num4++;
			}
			return text;
		}

		public string ReadSexyString()
		{
			throw new NotSupportedException();
		}

		public string ReadLine()
		{
			string text = "";
			while (true)
			{
				byte b = ReadByte();
				switch (b)
				{
				case 13:
					break;
				default:
					goto IL_001a;
				case 0:
				case 10:
					return text;
				}
				continue;
				IL_001a:
				text += (char)b;
			}
		}

		public void ReadBytes(ref byte[] theData, int theLen)
		{
			for (int i = 0; i < theLen; i++)
			{
				theData[i] = ReadByte();
			}
		}

		public void ReadBuffer(List<byte> theByteVector)
		{
			theByteVector.Clear();
			long num = ReadLong();
			if (num > 0)
			{
				theByteVector.AddRange(mData);
			}
		}

		public void ReadBuffer(Buffer theBuffer)
		{
			ReadBuffer(theBuffer.mData);
			theBuffer.mDataBitSize = theBuffer.mData.Count * 8;
		}

		public byte[] GetDataPtr()
		{
			if (mData.Count == 0)
			{
				return null;
			}
			return mData.ToArray();
		}

		public int GetDataLen()
		{
			return (mDataBitSize + 7) / 8;
		}

		public int GetDataLenBits()
		{
			return mDataBitSize;
		}

		public ulong GetCRC32(ulong theSeed)
		{
			throw new NotImplementedException();
		}

		public bool AtEnd()
		{
			return mReadBitPos >= mDataBitSize;
		}

		public bool PastEnd()
		{
			return mReadBitPos > mDataBitSize;
		}

		public int GetBitsAvailable()
		{
			if (!AtEnd())
			{
				return mDataBitSize - mReadBitPos;
			}
			return 0;
		}

		public int GetBytesAvailable()
		{
			return GetBitsAvailable() / 8;
		}

		private static int GetNextUTF8CharFromStream(byte[] theBuffer, int theLen, ref string theChar)
		{
			return GetNextUTF8CharFromStream(theBuffer, 0, theLen, ref theChar);
		}

		private static int GetNextUTF8CharFromStream(byte[] theBuffer, int start, int theLen, ref string theChar)
		{
			if (theLen == 0)
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			int num3 = theBuffer[start + num++];
			if ((num3 & 0x80) != 0)
			{
				if ((num3 & 0xC0) != 192)
				{
					return 0;
				}
				int[] array = new int[6];
				int num4 = 0;
				array[num4++] = num3;
				int i;
				for (i = 0; i < aMaskData.Length && (num3 & aMaskData[i]) != ((aMaskData[i] << 1) & aMaskData[i]); i++)
				{
				}
				if (i >= aMaskData.Length)
				{
					return 0;
				}
				num3 &= ~aMaskData[i];
				int num5 = i + 1;
				if (num5 < 2 || num5 > 6)
				{
					return 0;
				}
				int num6 = 0;
				while (i > 0 && num2 < theLen)
				{
					num6 = theBuffer[start + num++];
					if ((num6 & 0xC0) != 128)
					{
						return 0;
					}
					array[num4++] = num6;
					num3 = (num3 << 6) | (num6 & 0x3F);
					i--;
					num2++;
				}
				if (i > 0)
				{
					return 0;
				}
				bool flag = true;
				switch (num5)
				{
				case 2:
					flag = (array[0] & 0x3E) != 0;
					break;
				case 3:
					flag = (array[0] & 0x1F) != 0 || (array[1] & 0x20) != 0;
					break;
				case 4:
					flag = (array[0] & 0xF) != 0 || (array[1] & 0x30) != 0;
					break;
				case 5:
					flag = (array[0] & 7) != 0 || (array[1] & 0x38) != 0;
					break;
				case 6:
					flag = (array[0] & 3) != 0 || (array[1] & 0x3C) != 0;
					break;
				}
				if (!flag)
				{
					return 0;
				}
			}
			int result = num2;
			if ((num3 >= 55296 && num3 <= 57343) || (num3 >= 65534 && num3 <= 65535))
			{
				return 0;
			}
			theChar += num3;
			return result;
		}
	}
}
