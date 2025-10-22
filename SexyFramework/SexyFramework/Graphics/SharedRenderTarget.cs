using System;
using System.Collections.Generic;
using SexyFramework.Drivers;

namespace SexyFramework.Graphics
{
	public class SharedRenderTarget : IDisposable
	{
		public class Pool : IDisposable
		{
			protected class Entry
			{
				public DeviceImage mImage;

				public RenderSurface mScreenSurface;

				public SharedRenderTarget mLockOwner;

				public string mLockDebugTag;
			}

			protected List<Entry> mEntries = new List<Entry>();

			public void Dispose()
			{
				int count = mEntries.Count;
				for (int i = 0; i < count; i++)
				{
					Entry entry = mEntries[i];
					SharedRenderTarget mLockOwner = entry.mLockOwner;
					if (entry.mImage != null && entry.mImage != null)
					{
						entry.mImage.Dispose();
					}
					if (entry.mScreenSurface != null)
					{
						entry.mScreenSurface.Release();
					}
				}
				mEntries.Clear();
			}

			public void Acquire(SharedRenderTarget outTarget, int theWidth, int theHeight, uint theD3DFlags, string debugTag)
			{
				Entry entry = null;
				int num = 0;
				int count = mEntries.Count;
				for (num = 0; num < count; num++)
				{
					entry = mEntries[num];
					if (entry.mLockOwner == null && entry.mImage.mWidth == theWidth && entry.mImage.mHeight == theHeight && entry.mImage.GetImageFlags() == theD3DFlags)
					{
						outTarget.mImage = entry.mImage;
						outTarget.mScreenSurface = entry.mScreenSurface;
						outTarget.mLockHandle = (uint)(num + 1);
						entry.mLockOwner = outTarget;
						entry.mLockDebugTag = ((debugTag != string.Empty) ? debugTag : "NULL");
						return;
					}
				}
				mEntries.Add(new Entry());
				entry = mEntries[mEntries.Count - 1];
				num = count++;
				entry.mImage = new DeviceImage();
				entry.mImage.AddImageFlags(theD3DFlags);
				entry.mImage.Create(theWidth, theHeight);
				entry.mImage.SetImageMode(false, false);
				entry.mImage.CreateRenderData();
				entry.mScreenSurface = null;
				Graphics graphics = new Graphics(entry.mImage);
				graphics.Get3D()?.ClearColorBuffer(new Color(0, 0, 0, 0));
				outTarget.mImage = entry.mImage;
				outTarget.mScreenSurface = entry.mScreenSurface;
				outTarget.mLockHandle = (uint)(num + 1);
				entry.mLockOwner = outTarget;
				entry.mLockDebugTag = ((debugTag != string.Empty) ? debugTag : "NULL");
			}

			public void UpdateEntry(SharedRenderTarget inTarget)
			{
				if (inTarget.mLockHandle != 0)
				{
					int index = (int)(inTarget.mLockHandle - 1);
					int count = mEntries.Count;
					Entry entry = mEntries[index];
					SharedRenderTarget mLockOwner = entry.mLockOwner;
					DeviceImage mImage = inTarget.mImage;
					DeviceImage mImage2 = entry.mImage;
					entry.mScreenSurface = inTarget.mScreenSurface;
				}
			}

			public void Unacquire(SharedRenderTarget ioTarget)
			{
				if (ioTarget.mLockHandle != 0)
				{
					int index = (int)(ioTarget.mLockHandle - 1);
					int count = mEntries.Count;
					Entry entry = mEntries[index];
					SharedRenderTarget mLockOwner = entry.mLockOwner;
					DeviceImage mImage = ioTarget.mImage;
					DeviceImage mImage2 = entry.mImage;
					ioTarget.mImage = null;
					ioTarget.mScreenSurface = null;
					ioTarget.mLockHandle = 0u;
					entry.mLockOwner = null;
					entry.mLockDebugTag = "";
				}
			}

			public void InvalidateSurfaces()
			{
				int count = mEntries.Count;
				for (int i = 0; i < count; i++)
				{
					Entry entry = mEntries[i];
					if (entry.mScreenSurface != null)
					{
						entry.mScreenSurface.Release();
						entry.mScreenSurface = null;
					}
					if (entry.mLockOwner != null)
					{
						entry.mLockOwner.mScreenSurface = null;
					}
				}
			}

			public void InvalidateDevice()
			{
				List<SharedRenderTarget> list = new List<SharedRenderTarget>();
				int count = mEntries.Count;
				for (int i = 0; i < count; i++)
				{
					Entry entry = mEntries[i];
					if (entry.mImage != null)
					{
						GlobalMembers.gSexyAppBase.Remove3DData(entry.mImage);
					}
					if (entry.mLockOwner != null)
					{
						list.Add(entry.mLockOwner);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Unlock();
				}
				InvalidateSurfaces();
			}

			public string GetInfoString()
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int count = mEntries.Count;
				for (int i = 0; i < count; i++)
				{
					Entry entry = mEntries[i];
					if (entry.mLockOwner != null)
					{
						num++;
					}
					int mWidth = entry.mImage.mWidth;
					int mHeight = entry.mImage.mHeight;
					int mWidth2 = GlobalMembers.gSexyAppBase.mWidth;
					int mHeight2 = GlobalMembers.gSexyAppBase.mHeight;
					if (mWidth == mWidth2 && mHeight == mHeight2)
					{
						num2++;
					}
					else if (mWidth == mWidth2 / 2 && mHeight == mHeight2 / 2)
					{
						num3++;
					}
					else if (mWidth == mWidth2 / 4 && mHeight == mHeight2 / 4)
					{
						num4++;
					}
					else
					{
						num5++;
					}
				}
				return $"Total:{count} ({num2} Full, {num3} Half, {num4} Quarter, {num5} Other); Locked:{num}";
			}
		}

		public enum FLAGS
		{
			FLAGS_HINT_LAST_LOCK_SCREEN_IMAGE = 1
		}

		protected DeviceImage mImage;

		protected RenderSurface mScreenSurface;

		protected uint mLockHandle;

		public SharedRenderTarget()
		{
			mImage = null;
			mScreenSurface = null;
			mLockHandle = 0u;
		}

		public void Dispose()
		{
			DeviceImage mImage2 = mImage;
		}

		public DeviceImage Lock(int theWidth, int theHeight, uint additionalD3DFlags)
		{
			return Lock(theWidth, theHeight, additionalD3DFlags, string.Empty);
		}

		public DeviceImage Lock(int theWidth, int theHeight)
		{
			return Lock(theWidth, theHeight, 0u, string.Empty);
		}

		public DeviceImage Lock(int theWidth, int theHeight, uint additionalD3DFlags, string debugTag)
		{
			Unlock();
			uint num = 16u;
			GlobalMembers.gSexyAppBase.GetSharedRenderTargetPool().Acquire(this, theWidth, theHeight, num | additionalD3DFlags, debugTag);
			return mImage;
		}

		public DeviceImage LockScreenImage(string debugTag)
		{
			return LockScreenImage(debugTag, 0u);
		}

		public DeviceImage LockScreenImage()
		{
			return LockScreenImage(string.Empty, 0u);
		}

		public DeviceImage LockScreenImage(string debugTag, uint flags)
		{
			IGraphicsDriver mGraphicsDriver = GlobalMembers.gSexyAppBase.mGraphicsDriver;
			if (Lock(mGraphicsDriver.GetDisplayWidth(), mGraphicsDriver.GetDisplayHeight(), flags, debugTag) == null)
			{
				return null;
			}
			if (mGraphicsDriver.GetRenderDevice3D() != null)
			{
				mGraphicsDriver.GetRenderDevice3D().SwapScreenImage(ref mImage, ref mScreenSurface, flags);
			}
			
			GlobalMembers.gSexyAppBase.GetSharedRenderTargetPool().UpdateEntry(this);
			return mImage;
		}

		public bool Unlock()
		{
			if (mLockHandle == 0)
			{
				return false;
			}
			GlobalMembers.gSexyAppBase.GetSharedRenderTargetPool().Unacquire(this);
			return true;
		}

		public DeviceImage GetCurrentLockImage()
		{
			if (mLockHandle == 0)
			{
				return null;
			}
			return mImage;
		}
	}
}