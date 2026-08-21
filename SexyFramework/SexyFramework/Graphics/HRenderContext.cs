namespace SexyFramework.Graphics
{
	public class HRenderContext
	{
		public object mHandlePtr;

		public HRenderContext mParentContext;

		public RenderStateManager.Context mStateContext;

		public HRenderContext()
		{
		}

		public HRenderContext(object inHandlePtr)
		{
			mHandlePtr = inHandlePtr;
		}

		public bool IsValid()
		{
			return mHandlePtr != null;
		}

		public object GetPointer()
		{
			return mHandlePtr;
		}
	}
}
