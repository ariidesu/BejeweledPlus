using System;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Widget;

namespace BejeweledLivePlus
{
	public static class Res
	{
		private static ResGlobalPtr[] mGlobalRes = new ResGlobalPtr[1811];

		private static Point[] mGlobalResOffset = new Point[1811];

		private static BejeweledLivePlusApp mApp = null;

		private static ResourceManager mResMgr = null;

		public static string STR_NEW_BEST_TIME = "New best time!";

		public static string STR_NEW_HIGH_SCORE = "New high score!";

		public static void InitResources(BejeweledLivePlusApp app)
		{
			mApp = app;
			mResMgr = mApp.mResourceManager;
		}

		public static Image GetImageByID(ResourceId id)
		{
			if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
			{
				return mGlobalRes[(int)id].mResObject as Image;
			}
			string text = GlobalMembersResourcesWP.GetStringIdById((int)id);
			if (string.IsNullOrEmpty(text))
			{
				text = id.ToString();
				text = text.Substring(0, text.IndexOf("_ID"));
			}
			;
			mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
			if (mGlobalRes[(int)id] != null)
			{
				if (mGlobalRes[(int)id].mResObject != null)
				{
					return mGlobalRes[(int)id].mResObject as Image;
				}
				mResMgr.LoadImage(text);
			}

			return mGlobalRes[(int)id].mResObject as Image;
		}

		public static int GetIDByImage(Image img)
		{
			for (int i = 0; i < mGlobalRes.Length; i++)
			{
				if (mGlobalRes[i] != null && mGlobalRes[i].mResObject == img)
				{
					return i;
				}
			}
			return -1;
		}

		public static Font GetFontByID(ResourceId id)
		{
			if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
			{
				return mGlobalRes[(int)id].mResObject as Font;
			}
			string text = id.ToString();
			int num = text.IndexOf("_ID");
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
			if (mGlobalRes[(int)id] != null)
			{
				if (mGlobalRes[(int)id].mResObject != null)
				{
					return mGlobalRes[(int)id].mResObject as Font;
				}
				mResMgr.LoadFont(text);
			}
			return mGlobalRes[(int)id].mResObject as Font;
		}

		public static int GetSoundByID(ResourceId id)
		{
			if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
			{
				return (int)mGlobalRes[(int)id].mResObject;
			}
			string text = id.ToString();
			int num = text.IndexOf("_ID");
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
			if (mGlobalRes[(int)id] != null)
			{
				if (mGlobalRes[(int)id].mResObject != null)
				{
					return (int)mGlobalRes[(int)id].mResObject;
				}
				mResMgr.LoadSound(text);
			}
			return (int)mGlobalRes[(int)id].mResObject;
		}

		public static PIEffect GetPIEffectByID(ResourceId id)
		{
			if (mGlobalRes[(int)id] != null && mGlobalRes[(int)id].mResObject != null)
			{
				return mGlobalRes[(int)id].mResObject as PIEffect;
			}
			string text = id.ToString();
			int num = text.IndexOf("_ID");
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
			if (mGlobalRes[(int)id] != null)
			{
				if (mGlobalRes[(int)id].mResObject != null)
				{
					return mGlobalRes[(int)id].mResObject as PIEffect;
				}
				mResMgr.LoadPIEffect(text);
			}
			return mGlobalRes[(int)id].mResObject as PIEffect;
		}

		public static PopAnim GetPopAnimByID(ResourceId id)
		{
			if (mGlobalRes[(int)id] != null)
			{
				return mGlobalRes[(int)id].mResObject as PopAnim;
			}
			string text = id.ToString();
			int num = text.IndexOf("_ID");
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
			if (mGlobalRes[(int)id] != null)
			{
				if (mGlobalRes[(int)id].mResObject != null)
				{
					return mGlobalRes[(int)id].mResObject as PopAnim;
				}
				mResMgr.LoadPopAnim(text);
			}
			return mGlobalRes[(int)id].mResObject as PopAnim;
		}

		public static RenderEffectDefinition GetRenderEffectByID(ResourceId id)
		{
			if (mGlobalRes[(int)id] != null)
			{
				return mGlobalRes[(int)id].mResObject as RenderEffectDefinition;
			}
			string text = id.ToString();
			int num = text.IndexOf("_ID");
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			mGlobalRes[(int)id] = mResMgr.RegisterGlobalPtr(text);
			if (mGlobalRes[(int)id] != null)
			{
				if (mGlobalRes[(int)id].mResObject != null)
				{
					return mGlobalRes[(int)id].mResObject as RenderEffectDefinition;
				}
				mResMgr.LoadRenderEffect(text);
			}
			return mGlobalRes[(int)id].mResObject as RenderEffectDefinition;
		}

		public static int GetOffsetXByID(ResourceId id)
		{
			Point point = mGlobalResOffset[(int)id];
			return mGlobalResOffset[(int)id].mX;
		}

		public static int GetOffsetYByID(ResourceId id)
		{
			Point point = mGlobalResOffset[(int)id];
			return mGlobalResOffset[(int)id].mY;
		}
	}
}
