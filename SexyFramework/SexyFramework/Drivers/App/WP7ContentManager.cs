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
			try
			{
				return ReadAsset<T>(name, mCustom);
			}
			catch (ContentLoadException)
			{
				using (var stream = OpenStream(name))
				{
					using (var ms = new MemoryStream())
					{
						stream.CopyTo(ms);
						return (T)(object)ms.ToArray();
					}
				}
			}
		}

		public void CustomDispose<IDisposable>(IDisposable obj)
		{
		}
	}
}
