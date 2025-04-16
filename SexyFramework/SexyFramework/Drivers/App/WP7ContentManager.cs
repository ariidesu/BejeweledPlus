using System;
using Microsoft.Xna.Framework.Content;

namespace SexyFramework.Drivers.App
{
	public class WP7ContentManager : ContentManager
	{
		private Action<IDisposable> mCustom;

		public WP7ContentManager(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			mCustom = CustomDispose;
		}

		public T LoadResDirectly<T>(string name)
		{
			return ReadAsset<T>(name, mCustom);
		}

		public void CustomDispose<IDisposable>(IDisposable obj)
		{
		}
	}
}
