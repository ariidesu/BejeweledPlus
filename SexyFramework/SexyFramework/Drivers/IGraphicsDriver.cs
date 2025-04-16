using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Drivers
{
	public abstract class IGraphicsDriver
	{
		public enum EResult
		{
			RESULT_OK,
			RESULT_FAIL,
			RESULT_DD_CREATE_FAIL,
			RESULT_SURFACE_FAIL,
			RESULT_EXCLUSIVE_FAIL,
			RESULT_DISPCHANGE_FAIL,
			RESULT_INVALID_COLORDEPTH,
			RESULT_3D_FAIL,
			RESULT_3D_NOTREADY
		}

		public enum ERenderMode
		{
			RENDERMODE_Default = 0,
			RENDERMODE_Overdraw = 1,
			RENDERMODE_PseudoOverdraw = 2,
			RENDERMODE_BatchSize = 3,
			RENDERMODE_Wireframe = 4,
			RENDERMODE_WastedOverdraw = 5,
			RENDERMODE_TextureHash = 6,
			RENDERMODE_OverdrawExact = 7,
			RENDERMODE_COUNT = 8,
			RENDERMODE_CYCLE_END = 7
		}

		public enum ERenderModeFlags
		{
			RENDERMODEF_NoBatching = 1,
			RENDERMODEF_HalfTris = 2,
			RENDERMODEF_NoDynVB = 4,
			RENDERMODEF_PreventLag = 8,
			RENDERMODEF_NoTriRep = 16,
			RENDERMODEF_NoStretchRectFromTextures = 32,
			RENDERMODEF_HalfPresent = 64,
			RENDERMODEF_USEDBITS = 7
		}

		public static string ResultToString(int theResult)
		{
			switch ((EResult)theResult)
			{
			case EResult.RESULT_OK:
				return "RESULT_OK";
			case EResult.RESULT_FAIL:
				return "RESULT_FAIL";
			case EResult.RESULT_DD_CREATE_FAIL:
				return "RESULT_DD_CREATE_FAIL";
			case EResult.RESULT_SURFACE_FAIL:
				return "RESULT_SURFACE_FAIL";
			case EResult.RESULT_EXCLUSIVE_FAIL:
				return "RESULT_EXCLUSIVE_FAIL";
			case EResult.RESULT_DISPCHANGE_FAIL:
				return "RESULT_DISPCHANGE_FAIL";
			case EResult.RESULT_INVALID_COLORDEPTH:
				return "RESULT_INVALID_COLORDEPTH";
			default:
				return "RESULT_UNKNOWN";
			}
		}

		public virtual void Dispose()
		{
		}

		public abstract bool Is3D();

		public abstract int GetVersion();

		public abstract ulong GetRenderModeFlags();

		public abstract void SetRenderModeFlags(ulong flag);

		public abstract ERenderMode GetRenderMode();

		public abstract void SetRenderMode(ERenderMode inRenderMode);

		public abstract string GetRenderModeString(ERenderMode inRenderMode, ulong inRenderModeFlags, bool inIgnoreMode, bool inIgnoreFlags);

		public abstract void AddDeviceImage(DeviceImage theDDImage);

		public abstract void RemoveDeviceImage(DeviceImage theDDImage);

		public abstract void Remove3DData(MemoryImage theImage);

		public abstract DeviceImage GetScreenImage();

		public abstract int GetScreenWidth();

		public abstract int GetScreenHeight();

		public abstract void WindowResize(int theWidth, int theHeight);

		public abstract bool Redraw(Rect theClipRect);

		public abstract void RemapMouse(ref int theX, ref int theY);

		public abstract bool SetCursorImage(Image theImage);

		public abstract void SetCursorPos(int theCursorX, int theCursorY);

		public abstract void RemoveShader(object theShader);

		public abstract DeviceSurface CreateDeviceSurface();

		public abstract NativeDisplay GetNativeDisplayInfo();

		public abstract RenderDevice GetRenderDevice();

		public abstract RenderDevice3D GetRenderDevice3D();

		public abstract Ratio GetAspectRatio();

		public abstract int GetDisplayWidth();

		public abstract int GetDisplayHeight();

		public abstract CritSect GetCritSect();

		public abstract void SetRenderRect(int theX, int theY, int theWidth, int theHeight);

		public abstract Mesh LoadMesh(string thePath, MeshListener theListener);

		public abstract void AddMesh(Mesh theMesh);
	}
}
