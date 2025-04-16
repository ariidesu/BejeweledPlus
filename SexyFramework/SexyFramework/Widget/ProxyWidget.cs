using SexyFramework.Graphics;

namespace SexyFramework.Widget
{
	public class ProxyWidget : Widget
	{
		public ProxyWidgetListener mListener;

		public ProxyWidget(ProxyWidgetListener listener)
		{
			mListener = listener;
		}

		public override void Draw(SexyFramework.Graphics.Graphics g)
		{
			if (mListener != null)
			{
				mListener.DrawProxyWidget(g, this);
			}
		}
	}
}
