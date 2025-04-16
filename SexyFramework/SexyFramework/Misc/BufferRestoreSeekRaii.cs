using System;

namespace SexyFramework.Misc
{
	public class BufferRestoreSeekRaii : IDisposable
	{
		private Buffer mBuffer;

		private int mOrigReadPos;

		private int mOrigWritePos;

		public BufferRestoreSeekRaii(Buffer buffer)
		{
			mBuffer = buffer;
			mOrigReadPos = buffer.GetCurrReadBytePos();
			mOrigWritePos = buffer.GetCurrWriteBytePos();
		}

		public void Dispose()
		{
			mBuffer.SeekReadByte(mOrigReadPos);
			mBuffer.SeekWriteByte(mOrigWritePos);
		}
	}
}
