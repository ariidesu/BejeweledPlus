using System.IO;

namespace SexyFramework.AELib
{
	internal class ParseByteArray
	{
		private MemoryStream stream;

		private BinaryReader reader;

		public ParseByteArray(byte[] d)
		{
			if (d != null)
			{
				stream = new MemoryStream(d);
				reader = new BinaryReader(stream);
			}
		}

		public bool isEnd()
		{
			return stream.Position >= stream.Length;
		}

		public bool readBoolean(ref bool value)
		{
			value = reader.ReadBoolean();
			return true;
		}

		public bool readInt32(ref int value)
		{
			value = reader.ReadInt32();
			return true;
		}

		public bool readLong(ref long value)
		{
			value = reader.ReadInt32();
			return true;
		}

		public bool readDouble(ref double value)
		{
			value = reader.ReadDouble();
			return true;
		}

		public bool readString(ref string value, int len)
		{
			char[] array = reader.ReadChars(len);
			string text = "";
			for (int i = 0; i < array.Length && array[i] != 0; i++)
			{
				text += array[i];
			}
			value = text;
			return true;
		}
	}
}
