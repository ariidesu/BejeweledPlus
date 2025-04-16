namespace SexyFramework.AELib
{
	public class CompLayer
	{
		public Layer mSource;

		public int mStartFrameOnComp;

		public int mDuration;

		public int mLayerOffsetStart;

		public CompLayer()
		{
		}

		public CompLayer(Layer l, int start_frame, int dur, int offs)
		{
			mSource = l;
			mStartFrameOnComp = start_frame;
			mDuration = dur;
			mLayerOffsetStart = offs;
		}

		public CompLayer(CompLayer other)
		{
			CopyFrom(other);
		}

		public void CopyFrom(CompLayer other)
		{
			mSource = other.mSource.Duplicate();
			mStartFrameOnComp = other.mStartFrameOnComp;
			mDuration = other.mDuration;
			mLayerOffsetStart = other.mLayerOffsetStart;
		}
	}
}
