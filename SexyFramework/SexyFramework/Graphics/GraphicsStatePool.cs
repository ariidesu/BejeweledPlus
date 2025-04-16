using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public static class GraphicsStatePool
	{
		private static Stack<GraphicsState> mFreeStates = new Stack<GraphicsState>();

		public static GraphicsState CreateState()
		{
			GraphicsState graphicsState = null;
			if (mFreeStates.Count > 0)
			{
				return mFreeStates.Pop();
			}
			return new GraphicsState();
		}

		public static void ReleaseState(GraphicsState state)
		{
			mFreeStates.Push(state);
		}
	}
}
