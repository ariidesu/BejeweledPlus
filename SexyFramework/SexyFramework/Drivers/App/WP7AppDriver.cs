using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Framework.Utilities;
using SexyFramework.Drivers.Graphics;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Color = Microsoft.Xna.Framework.Color;

namespace SexyFramework.Drivers.App
{
	public class WP7AppDriver : IAppDriver
	{
		public static WP7AppDriver sWP7AppDriverInstance;

		private SexyAppBase mApp;

		private Game mWP7Game;

		// private GameTime mGameTime;

		private int mGameStartTime;

		private int mGameLastUpdateTime;
		
		public ContentManager mContentManager;

		private XNAGraphicsDriver mXNAGraphicsDriver;

		private ConfigItemKey mConfigRoot;

		private MemoryImage gFPSImage;

		public static WP7AppDriver CreateAppDriver(SexyAppBase App)
		{
			if (sWP7AppDriverInstance == null)
			{
				sWP7AppDriverInstance = new WP7AppDriver(App);
			}
			return sWP7AppDriverInstance;
		}

		public WP7AppDriver(SexyAppBase appBase)
		{
			mApp = appBase;
			mConfigRoot = new ConfigItemKey();
		}

		public override void Dispose()
		{
			SaveConfig();
			Shutdown();
		}

		public override bool InitAppDriver()
		{
			mApp.mNotifyGameMessage = 0u;
			mApp.mOnlyAllowOneCopyToRun = true;
			mApp.mProductVersion = GetProductVersion("");
			mApp.mNoDefer = false;
			mApp.mFullScreenPageFlip = true;
			mApp.mTimeLoaded = GetTickCount();
			mApp.mSEHOccured = false;
			mApp.mProdName = "Product";
			mApp.mShutdown = false;
			mApp.mExitToTop = false;
			mApp.mWidth = 640;
			mApp.mHeight = 1066;
			mApp.mFullscreenBits = 16;
			mApp.mIsWindowed = true;
			mApp.mIsPhysWindowed = true;
			mApp.mFullScreenWindow = false;
			mApp.mPreferredX = -1;
			mApp.mPreferredY = -1;
			mApp.mPreferredWidth = -1;
			mApp.mPreferredHeight = -1;
			mApp.mIsScreenSaver = false;
			mApp.mAllowMonitorPowersave = true;
			mApp.mWantsDialogCompatibility = false;
			mApp.mFrameTime = 10f;
			mApp.mNonDrawCount = 0;
			mApp.mDrawCount = 0;
			mApp.mSleepCount = 0;
			mApp.mUpdateCount.value = 0;
			mApp.mMaxUpdateBacklog = 200;
			mApp.mUpdateAppState = 0;
			mApp.mUpdateAppDepth = 0;
			mApp.mPendingUpdatesAcc = 0.0;
			mApp.mUpdateFTimeAcc = 0.0;
			mApp.mHasPendingDraw = true;
			mApp.mIsDrawing = false;
			mApp.mLastDrawWasEmpty = false;
			mApp.mLastTimeCheck = 0;
			mApp.mUpdateMultiplier = 1;
			mApp.mMaxNonDrawCount = 50;
			mApp.mPaused = false;
			mApp.mFastForwardToUpdateNum = 0;
			mApp.mFastForwardToMarker = false;
			mApp.mFastForwardStep = false;
			mApp.mCursorNum = 13;
			mApp.mMouseIn = false;
			mApp.mRunning = false;
			mApp.mActive = true;
			mApp.mProcessInTimer = false;
			mApp.mMinimized = false;
			mApp.mPhysMinimized = false;
			mApp.mIsDisabled = false;
			mApp.mLoaded = false;
			mApp.mReloadingResources = false;
			mApp.mReloadPct = 0f;
			mApp.mYieldMainThread = false;
			mApp.mLoadingFailed = false;
			mApp.mLoadingThreadStarted = false;
			mApp.mAutoStartLoadingThread = true;
			mApp.mLoadingThreadCompleted = false;
			mApp.mCursorThreadRunning = false;
			mApp.mNumLoadingThreadTasks = 0;
			mApp.mCompletedLoadingThreadTasks = 0;
			mApp.mLastDrawTick = timeGetTime();
			mApp.mNextDrawTick = timeGetTime();
			mApp.mSysCursor = true;
			mApp.mForceFullscreen = false;
			mApp.mForceWindowed = false;
			mApp.mHasFocus = true;
			mApp.mIsOpeningURL = false;
			mApp.mInitialized = false;
			mApp.mLastShutdownWasGraceful = true;
			mApp.mReadFromRegistry = false;
			mApp.mCmdLineParsed = false;
			mApp.mSkipSignatureChecks = false;
			mApp.mCtrlDown = false;
			mApp.mAltDown = false;
			mApp.mAllowAltEnter = true;
			mApp.mStepMode = 0;
			mApp.mCleanupSharedImages = false;
			mApp.mStandardWordWrap = true;
			mApp.mbAllowExtendedChars = true;
			mApp.mEnableMaximizeButton = false;
			mApp.mWriteToSexyCache = true;
			mApp.mSexyCacheBuffers = false;
			mApp.mWriteFontCacheDir = true;
			mApp.mMusicVolume = 0.85;
			mApp.mSfxVolume = 0.85;
			mApp.mMuteCount = 0;
			mApp.mAutoMuteCount = 0;
			mApp.mDemoMute = false;
			mApp.mMuteOnLostFocus = true;
			mApp.mFPSTime = 0;
			mApp.mFPSStartTick = GetTickCount();
			mApp.mFPSFlipCount = 0;
			mApp.mFPSCount = 0;
			mApp.mFPSDirtyCount = 0;
			mApp.mShowFPS = false;
			mApp.mShowFPSMode = 0;
			mApp.mVFPSUpdateTimes = 0.0;
			mApp.mVFPSUpdateCount = 0;
			mApp.mVFPSDrawTimes = 0.0;
			mApp.mVFPSDrawCount = 0;
			mApp.mCurVFPS = 0f;
			mApp.mDrawTime = 0;
			mApp.mScreenBltTime = 0;
			mApp.mDebugKeysEnabled = false;
			mApp.mNoSoundNeeded = false;
			mApp.mWantFMod = false;
			mApp.mSyncRefreshRate = 100;
			mApp.mVSyncUpdates = false;
			mApp.mNoVSync = false;
			mApp.mVSyncBroken = false;
			mApp.mVSyncBrokenCount = 0;
			mApp.mVSyncBrokenTestStartTick = 0L;
			mApp.mVSyncBrokenTestUpdates = 0L;
			mApp.mWaitForVSync = false;
			mApp.mSoftVSyncWait = true;
			mApp.mAutoEnable3D = false;
			mApp.mTest3D = false;
			mApp.mNoD3D9 = false;
			mApp.mMinVidMemory3D = 6u;
			mApp.mRecommendedVidMemory3D = 14u;
			mApp.mRelaxUpdateBacklogCount = 0;
			mApp.mWidescreenAware = false;
			mApp.mWidescreenTranslate = true;
			mApp.mEnableWindowAspect = false;
			mApp.mIsWideWindow = false;
			mApp.mOrigScreenWidth = 640;
			mApp.mOrigScreenHeight = 1066;
			mApp.mIsSizeCursor = false;
			for (int i = 0; i < 13; i++)
			{
				mApp.mCursorImages[i] = null;
			}
			for (int i = 0; i < 256; i++)
			{
				mApp.mAdd8BitMaxTable[i] = (byte)i;
			}
			for (int i = 256; i < 512; i++)
			{
				mApp.mAdd8BitMaxTable[i] = byte.MaxValue;
			}
			mApp.mPrimaryThreadId = 0u;
			mApp.mShowWidgetInspector = false;
			mApp.mWidgetInspectorCurWidget = null;
			mApp.mWidgetInspectorScrollOffset = 0;
			mApp.mWidgetInspectorPickWidget = null;
			mApp.mWidgetInspectorPickMode = false;
			mApp.mWidgetInspectorLeftAnchor = false;
			GlobalMembers.gIs3D = true;
			return true;
		}

		public override void Start()
		{
			if (!mApp.mShutdown && mApp.mAutoStartLoadingThread)
			{
				StartLoadingThread();
			}
		}

		public override void Init()
		{
			if (!mApp.mShutdown)
			{
				mApp.mFileDriver.InitFileDriver(mApp);
				LoadConfig();
				mApp.mFileDriver.InitSaveDataFolder();
				mApp.mRandSeed = (uint)GetTickCount();
				mXNAGraphicsDriver.Init();
				mApp.mSoundManager = mApp.mAudioDriver.CreateSoundManager();
				mApp.mMusicInterface = new CustomBassMusicInterface();
				mApp.SetMusicVolume(mApp.mMusicVolume);
				IsScreenSaver();
				mApp.mScreenBounds.mWidth = mApp.mWidth;
				mApp.mScreenBounds.mHeight = mApp.mHeight;
				mApp.mWidgetManager.Resize(mApp.mScreenBounds, mApp.mScreenBounds);
				mApp.mWidgetManager.mImage = mXNAGraphicsDriver.GetScreenImage();
				mApp.mWidgetManager.MarkAllDirty();
				mApp.mInitialized = true;
			}
		}

		public override bool UpdateAppStep(ref bool updated)
		{
			updated = false;
			if (mApp.mExitToTop)
			{
				return false;
			}

			if (mApp.mUpdateAppState == 0 || mApp.mUpdateAppState == 3)
			{
				mApp.mUpdateAppState = 1;
			}
			mApp.mUpdateAppDepth++;
			if (mApp.mStepMode != 0)
			{
				if (mApp.mStepMode == 2)
				{
					Thread.Sleep((int)mApp.mFrameTime);
					mApp.mUpdateAppState = 3;
				}
				else
				{
					mApp.mStepMode = 2;
					DoUpdateFrames();
					DoUpdateFramesF(1f);
				}
			}
			int mUpdateCount = mApp.mUpdateCount;
			Process();
			mApp.ProcessSafeDeleteList();
			updated = mApp.mUpdateCount != mUpdateCount;
			mApp.mUpdateAppDepth--;
			mGameLastUpdateTime = timeGetTime();
			return true;
		}

		public override void ClearUpdateBacklog(bool relaxForASecond)
		{
			mApp.mLastTimeCheck = timeGetTime();
			mApp.mUpdateFTimeAcc = 0.0;
			if (relaxForASecond)
			{
				mApp.mRelaxUpdateBacklogCount = 1000;
			}
		}

		public override void Shutdown()
		{
			mWP7Game.Exit();
		}

		public override void DoExit(int theCode)
		{
		}

		public override void Remove3DData(MemoryImage theMemoryImage)
		{
		}

		public override void BeginPopup()
		{
		}

		public override void EndPopup()
		{
		}

		public override int MsgBox(string theText, string theTitle, int theFlags)
		{
			return 0;
		}

		public override void Popup(string theString)
		{
		}

		public override bool OpenURL(string theURL, bool shutdownOnOpen)
		{
			return true;
		}

		public override string GetGameSEHInfo()
		{
			return "";
		}

		public override void SEHOccured()
		{
		}

		public override void GetSEHWebParams(DefinesMap theDefinesMap)
		{
		}

		public override void DoParseCmdLine()
		{
		}

		public override void ParseCmdLine(string theCmdLine)
		{
		}

		public override void HandleCmdLineParam(string theParamName, string theParamValue)
		{
		}

		public override void StartLoadingThread()
		{
			GlobalMembers.gSexyApp.mLoadingThreadStarted = true;
			GlobalMembers.gSexyApp.LoadingThreadProc();
			GlobalMembers.gSexyApp.mLoadingThreadCompleted = true;
		}

		public override double GetLoadingThreadProgress()
		{
			return 1.0;
		}

		public override void CopyToClipboard(string theString)
		{
		}

		public override string GetClipboard()
		{
			return "";
		}

		public override void SetCursor(int theCursorNum)
		{
		}

		public override int GetCursor()
		{
			return 0;
		}

		public override void EnableCustomCursors(bool enabled)
		{
		}

		public override void SetCursorImage(int theCursorNum, Image theImage)
		{
		}

		public override void SwitchScreenMode()
		{
		}

		public override void SwitchScreenMode(bool wantWindowed)
		{
		}

		public override void SwitchScreenMode(bool wantWindowed, bool is3d, bool force)
		{
		}

		public override bool KeyDown(int theKey)
		{
			return false;
		}

		public override bool DebugKeyDown(int theKey)
		{
			return false;
		}

		public override bool DebugKeyDownAsync(int theKey, bool ctrlDown, bool altDown)
		{
			return false;
		}

		public override bool Is3DAccelerated()
		{
			return true;
		}

		public override bool Is3DAccelerationSupported()
		{
			return true;
		}

		public override bool Is3DAccelerationRecommended()
		{
			return true;
		}

		public override void Set3DAcclerated(bool is3D, bool reinit)
		{
		}

		public override bool IsUIOrientationAllowed(UI_ORIENTATION theOrientation)
		{
			return false;
		}

		public override UI_ORIENTATION GetUIOrientation()
		{
			return UI_ORIENTATION.UI_ORIENTATION_LANDSCAPE_RIGHT;
		}

		public override bool IsSystemUIShowing()
		{
			return false;
		}

		public override void ShowKeyboard()
		{
		}

		public override void HideKeyboard()
		{
		}

		public override bool CheckSignature(SexyFramework.Misc.Buffer theBuffer, string theFileName)
		{
			return true;
		}

		public override bool ReloadAllResources()
		{
			return true;
		}

		public override bool ConfigGetSubKeys(string theKeyName, List<string> theSubKeys)
		{
			throw new NotImplementedException();
		}

		public override bool ConfigReadString(string theValueName, ref string theString)
		{
			bool result = false;
			ConfigItem configItem = mConfigRoot[theValueName];
			if (configItem != null && configItem.type == ConfigType.String)
			{
				theString = ((ConfigItemString)configItem).value;
				result = true;
			}
			return result;
		}

		public override bool ConfigReadInteger(string theValueName, ref int theValue)
		{
			bool result = false;
			ConfigItem configItem = mConfigRoot[theValueName];
			if (configItem != null && configItem.type == ConfigType.Int)
			{
				theValue = ((ConfigItemInt)configItem).value;
				result = true;
			}
			return result;
		}

		public override bool ConfigReadBoolean(string theValueName, ref bool theValue)
		{
			bool result = false;
			ConfigItem configItem = mConfigRoot[theValueName];
			if (configItem != null && configItem.type == ConfigType.Boolean)
			{
				theValue = ((ConfigItemBoolean)configItem).value;
				result = true;
			}
			return result;
		}

		public override bool ConfigReadData(string theValueName, ref byte[] theValue, ref ulong theLength)
		{
			bool result = false;
			ConfigItem configItem = mConfigRoot[theValueName];
			if (configItem != null && configItem.type == ConfigType.Data)
			{
				ConfigItemData configItemData = (ConfigItemData)configItem;
				theValue = new byte[configItemData.value.Length];
				configItemData.value.CopyTo(theValue, 0);
				theLength = (ulong)configItemData.value.Length;
				result = true;
			}
			return result;
		}

		public override bool ConfigWriteString(string theValueName, string theString)
		{
			bool result = false;
			if (mConfigRoot.create(theValueName, ConfigType.String))
			{
				ConfigItemString configItemString = (ConfigItemString)mConfigRoot[theValueName];
				configItemString.value = theString;
				result = true;
			}
			return result;
		}

		public override bool ConfigWriteInteger(string theValueName, int theValue)
		{
			bool result = false;
			if (mConfigRoot.create(theValueName, ConfigType.Int))
			{
				ConfigItemInt configItemInt = (ConfigItemInt)mConfigRoot[theValueName];
				configItemInt.value = theValue;
				result = true;
			}
			return result;
		}

		public override bool ConfigWriteBoolean(string theValueName, bool theValue)
		{
			bool result = false;
			if (mConfigRoot.create(theValueName, ConfigType.Boolean))
			{
				ConfigItemBoolean configItemBoolean = (ConfigItemBoolean)mConfigRoot[theValueName];
				configItemBoolean.value = theValue;
				result = true;
			}
			return result;
		}

		public override bool ConfigWriteData(string theValueName, byte[] theValue, ulong theLength)
		{
			bool result = false;
			if (mConfigRoot.create(theValueName, ConfigType.Data))
			{
				ConfigItemData configItemData = (ConfigItemData)mConfigRoot[theValueName];
				configItemData.value = new byte[theValue.Length];
				theValue.CopyTo(configItemData.value, 0);
				result = true;
			}
			return result;
		}

		public override bool ConfigEraseKey(string theKeyName)
		{
			throw new NotImplementedException();
		}

		public override void ConfigEraseValue(string theValueName)
		{
			throw new NotImplementedException();
		}

		public override void ReadFromConfig()
		{
		}

		public override void WriteToConfig()
		{
		}

		public override void SaveConfig()
		{
			bool flag = false;
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			if (mConfigRoot != null)
			{
				mConfigRoot.save(buffer);
				if (WriteBufferToFile(configFileName(), buffer))
				{
					flag = true;
				}
			}
		}

		public void LoadConfig()
		{
			bool flag = false;
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			if (ReadBufferFromFile(configFileName(), buffer, false) && buffer.ReadInt32() == 0)
			{
				mConfigRoot = new ConfigItemKey();
				mConfigRoot.load(buffer);
				flag = true;
			}
		}

		private string configFileName()
		{
			return GlobalMembers.gFileDriver.GetSaveDataPath() + "config.dat";
		}

		public override bool WriteBufferToFile(string theFileName, SexyFramework.Misc.Buffer theBuffer)
		{
			return WriteBytesToFile(theFileName, theBuffer.GetDataPtr(), (ulong)theBuffer.GetDataLen());
		}

		public override bool ReadBufferFromFile(string theFileName, SexyFramework.Misc.Buffer theBuffer, bool dontWriteToDemo)
		{
			bool result = false;
			try
			{
				// IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
				// using (IsolatedStorageFileStream isolatedStorageFileStream = userStoreForApplication.OpenFile(theFileName, FileMode.Open, FileAccess.Read))
				// {
				// 	using (BinaryReader binaryReader = new BinaryReader(isolatedStorageFileStream))
				// 	{
				// 		byte[] array = binaryReader.ReadBytes((int)isolatedStorageFileStream.Length);
				// 		theBuffer.SetData(array, array.Length);
				// 	}
				// }
				string storagePath = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					theFileName
				);

				using (var fs = System.IO.File.Open(storagePath, FileMode.Open, FileAccess.Read))
				using (var br = new BinaryReader(fs))
				{
					byte[] array = br.ReadBytes((int)fs.Length);
					theBuffer.SetData(array, array.Length);
				}
				result = true;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public override bool WriteBytesToFile(string theFileName, byte[] theData, ulong theDataLen)
		{
			bool result = false;
			string fileDir = Common.GetFileDir(theFileName, false);
			GlobalMembers.gFileDriver.MakeFolders(fileDir);
			try
			{
				// IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
				// using (IsolatedStorageFileStream output = userStoreForApplication.OpenFile(theFileName, FileMode.Create, FileAccess.Write))
				// {
				// 	using (BinaryWriter binaryWriter = new BinaryWriter(output))
				// 	{
				// 		binaryWriter.Write(theData);
				// 	}
				// }
				string storagePath = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					theFileName
				);

				using (var fs = System.IO.File.Open(storagePath, FileMode.Create, FileAccess.Write))
				using (var br = new BinaryWriter(fs))
				{
					br.Write(theData);
				}
				result = true;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public override DeviceImage GetOptimizedImage(string theFileName, bool commitBits, bool allowTriReps)
		{
			return ((XNAGraphicsDriver)mApp.mGraphicsDriver).GetOptimizedImage(theFileName, commitBits, allowTriReps);
		}

		public override DeviceImage GetOptimizedImage(Stream stream, bool commitBits, bool allowTriReps)
		{
			return ((XNAGraphicsDriver)mApp.mGraphicsDriver).GetOptimizedImage(stream, commitBits, allowTriReps);
		}

		public override object GetOptimizedRenderData(string theFileName)
		{
			return ((XNAGraphicsDriver)mApp.mGraphicsDriver).GetOptimizedRenderData(theFileName);
		}

		public override bool ShouldPauseUpdates()
		{
			return false;
		}

		public override void Draw()
		{
			mXNAGraphicsDriver.ClearColorBuffer(SexyFramework.Graphics.Color.Black);
			DrawDirtyStuff();
		}

		public override int GetAppTime()
		{
			return (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
		}

		public void InitXNADriver(Game game)
		{
			mWP7Game = game;
			mXNAGraphicsDriver = new XNAGraphicsDriver(mWP7Game, mApp);
			mContentManager = mWP7Game.Content;
			mGameStartTime = GetAppTime();
			mApp.mGraphicsDriver = mXNAGraphicsDriver;
		}

		public int timeGetTime()
		{
			return GetAppTime() - mGameStartTime;
		}

		public int GetTickCount()
		{
			return 0;
		}

		public bool IsScreenSaver()
		{
			return mApp.mIsScreenSaver;
		}

		public bool AppCanRestore()
		{
			return !mApp.mIsDisabled;
		}

		public void ReloadAllResources_DrawStateUpdate(string theHeader, string theSubText, float thePct)
		{
			MemoryImage mImage = mApp.mWidgetManager.mImage;
		}

		public void ReloadAllResourcesProc()
		{
			mApp.mReloadingResources = false;
		}

		public void ReloadAllResourcesProcStub(IntPtr theArg)
		{
		}

		public string GetProductVersion(string thePath)
		{
			return "v0.1";
		}

		private bool Process(bool allowSleep = false)
		{
			if (mApp.mLoadingFailed)
			{
				mApp.Shutdown();
			}
			bool isVSynched = mApp.mVSyncUpdates && !mApp.mLastDrawWasEmpty && !mApp.mVSyncBroken && (!mApp.mIsPhysWindowed || (mApp.mIsPhysWindowed && mApp.mWaitForVSync && !mApp.mSoftVSyncWait));
			double aFrameFTime;
			double anUpdatesPerUpdateF;
			int aXNAFrameDelta = timeGetTime() - mGameLastUpdateTime;
			if (mApp.mVSyncUpdates)
			{
				aFrameFTime = (1000.0 / mApp.mSyncRefreshRate) / mApp.mUpdateMultiplier;
				anUpdatesPerUpdateF = (float)(1000.0 / (mApp.mFrameTime * mApp.mSyncRefreshRate));
			}
			else
			{
				aFrameFTime = (double)mApp.mFrameTime / mApp.mUpdateMultiplier;
				anUpdatesPerUpdateF = 1.0;
			}
			if (!mApp.mPaused && mApp.mUpdateMultiplier > 0.0)
			{
				int aStartTime = timeGetTime();
				int aCumSleepTime = 0;
				if (!isVSynched)
				{
					UpdateFTimeAcc();
				}
				bool didUpdate = false;
				if (mApp.mUpdateAppState == 1)
				{
					if (++mApp.mNonDrawCount < (int)Math.Ceiling(mApp.mMaxNonDrawCount * mApp.mUpdateMultiplier) || !mApp.mLoaded)
					{
						bool doUpdate = false;
						if (isVSynched)
						{
							doUpdate = (!mApp.mHasPendingDraw) || (mApp.mUpdateFTimeAcc >= (int)(aFrameFTime * 0.75));
						}
						else if (mApp.mUpdateFTimeAcc >= aFrameFTime)
						{
							doUpdate = true;
						}

						if (doUpdate) {
							if (mApp.mUpdateMultiplier == 1.0)
							{
								mApp.mVSyncBrokenTestUpdates += 1L;
								if (mApp.mVSyncBrokenTestUpdates >= (1000f + mApp.mFrameTime - 1f) / mApp.mFrameTime)
								{
									if (aStartTime - mApp.mVSyncBrokenTestStartTick <= 800L)
									{
										mApp.mVSyncBrokenCount++;
										if (mApp.mVSyncBrokenCount >= 3)
										{
											mApp.mVSyncBroken = true;
										}
									}
									else
									{
										mApp.mVSyncBrokenCount = 0;
									}
									mApp.mVSyncBrokenTestStartTick = aStartTime;
									mApp.mVSyncBrokenTestUpdates = 0L;
								}
							}
							bool hadRealUpdate = DoUpdateFrames();
							if (hadRealUpdate)
							{
								mApp.mUpdateAppState = 2;
							}
							mApp.mHasPendingDraw = true;
							didUpdate = true;
						}
					}
				}
				else if (mApp.mUpdateAppState == 2)
				{
					// For mobile, it's capped at 60FPS, unlike PC which can go unlimited
					// So this is just some workaround
					double aDecrement = 1.0;
					if (PlatformInfo.MonoGamePlatform == MonoGamePlatform.iOS ||
					    PlatformInfo.MonoGamePlatform == MonoGamePlatform.Android)
					{
						aDecrement = Math.Min(4.9 / aXNAFrameDelta, 1.0);
					}
					
					mApp.mUpdateAppState = 3;
					mApp.mPendingUpdatesAcc += anUpdatesPerUpdateF;
					mApp.mPendingUpdatesAcc -= aDecrement;
					
					while (mApp.mPendingUpdatesAcc >= aDecrement)
					{
						mApp.mNonDrawCount++;
						bool hasRealUpdate = DoUpdateFrames();
						if (!hasRealUpdate)
						{
							break;
						}
						mApp.mPendingUpdatesAcc -= aDecrement;
					}
					DoUpdateFramesF((float) anUpdatesPerUpdateF);
					
					if (isVSynched)
					{
						mApp.mUpdateFTimeAcc = Math.Max(mApp.mUpdateFTimeAcc - aFrameFTime - 0.2f, 0.0);
					}
					else
					{
						mApp.mUpdateFTimeAcc -= aFrameFTime;
					}
					if (mApp.mRelaxUpdateBacklogCount > 0)
					{
						mApp.mUpdateFTimeAcc = 0.0;
					}
					didUpdate = true;
				}
				if (!didUpdate)
				{
					mApp.mUpdateAppState = 3;
					mApp.mNonDrawCount = 0;
					if (mApp.mHasPendingDraw)
					{
						DrawDirtyStuff();
					}
					else
					{
						int aTimeToNextFrame = (int)(aFrameFTime - mApp.mUpdateFTimeAcc);
						if (aTimeToNextFrame > 0)
						{
							if (!allowSleep)
							{
								return false;
							}
							mApp.mSleepCount++;
							Thread.Sleep(aTimeToNextFrame);
							aCumSleepTime += aTimeToNextFrame;
						}
					}
				}
				if (mApp.mYieldMainThread)
				{
					int anEndTime = timeGetTime();
					int anElapsedTime = (anEndTime - aStartTime) - aCumSleepTime;
					int aLoadingYieldSleepTime = Math.Min(250, anElapsedTime * 2 - aCumSleepTime);
					if (aLoadingYieldSleepTime >= 0)
					{
						if (!allowSleep)
						{
							return false;
						}
						Thread.Sleep(aLoadingYieldSleepTime);
					}
				}
			}
			
			return true;
		}

		private void UpdateFTimeAcc()
		{
			int num = timeGetTime();
			if (mApp.mLastTimeCheck != 0)
			{
				int num2 = num - mApp.mLastTimeCheck;
				mApp.mUpdateFTimeAcc = Math.Min(mApp.mUpdateFTimeAcc + (double)num2, (float)mApp.mMaxUpdateBacklog);
				if (mApp.mRelaxUpdateBacklogCount > 0)
				{
					mApp.mRelaxUpdateBacklogCount = Math.Max(mApp.mRelaxUpdateBacklogCount - num2, 0);
				}
			}
			mApp.mLastTimeCheck = num;
		}

		private void ReDraw()
		{
			mXNAGraphicsDriver.Redraw(Rect.ZERO_RECT);
		}
		
		private static int gFrameCount = 0;
		private static int gFPSDisplay = 0;
		private static bool gForceDisplay = false;
		private static readonly Stopwatch gFPSTimer = new();
		private void CalculateFPS()
		{
			if (gFPSImage == null)
			{
				gFPSImage = new MemoryImage();
				gFPSImage.Create(50,20);
				gFPSImage.SetImageMode(false,false);
				gFPSImage.mPurgeBits = false;
				gFPSImage.PurgeBits();
			}

			if (gFPSTimer.ElapsedMilliseconds >= 1000 || gForceDisplay)
			{
				gFPSTimer.Stop();
				if (!gForceDisplay)
					gFPSDisplay = (int)(gFrameCount*1000/gFPSTimer.ElapsedMilliseconds + 0.5f);
				else
				{
					gForceDisplay = false;
					gFPSDisplay = 0;
				}

				gFPSTimer.Start();
				gFrameCount = 0;

				SexyFramework.Graphics.Graphics aDrawG = new SexyFramework.Graphics.Graphics(gFPSImage);
				string aFPS = $"FPS: {gFPSDisplay}";
				aDrawG.SetColor(SexyFramework.Graphics.Color.Black);
				aDrawG.FillRect(0,0,gFPSImage.GetWidth(),gFPSImage.GetHeight());
				aDrawG.SetColor(SexyFramework.Graphics.Color.White);
				aDrawG.DrawString(aFPS,2,20);
				gFPSImage.mBitsChangedCount++;
			}
		}

		private void FPSDrawCoords(int theX, int theY)
		{
			if (gFPSImage == null)
			{
				gFPSImage = new MemoryImage();
				gFPSImage.Create(50,20);
				gFPSImage.SetImageMode(false,false);
				gFPSImage.mPurgeBits = false;
				gFPSImage.PurgeBits();
			}

			SexyFramework.Graphics.Graphics aDrawG = new SexyFramework.Graphics.Graphics(gFPSImage);
			string aFPS = $"{theX},{theY}";
			aDrawG.SetColor(SexyFramework.Graphics.Color.Black);
			aDrawG.FillRect(0,0,gFPSImage.GetWidth(),gFPSImage.GetHeight());
			aDrawG.SetColor(SexyFramework.Graphics.Color.White);
			aDrawG.DrawString(aFPS,2,20);
			gFPSImage.mBitsChangedCount++;
		}
		
		private bool DrawDirtyStuff()
		{
			if (mApp.mShowFPS)
			{
				switch (mApp.mShowFPSMode)
				{
					case 0: CalculateFPS(); break;
					case 1:
						if (mApp.mWidgetManager != null)
						{
							FPSDrawCoords(mApp.mWidgetManager.mLastMouseX, mApp.mWidgetManager.mLastMouseY);
						}
						break;
				}
			}
			
			int aStartTime = timeGetTime();
			mApp.mIsDrawing = true;
			mXNAGraphicsDriver.mXNARenderDevice.SwitchToScreenImage();
			bool drewScreen = mApp.mWidgetManager.DrawScreen();
			mApp.mIsDrawing = false;
			if ((drewScreen || aStartTime - mApp.mLastDrawTick >= 1000 || mApp.mCustomCursorDirty) && aStartTime - mApp.mNextDrawTick >= 0)
			{
				mApp.mLastDrawWasEmpty = false;
				mApp.mDrawCount++;
				int aMidTime = timeGetTime();
				mApp.mFPSCount++;
				mApp.mFPSTime += aMidTime - aStartTime;
				mApp.mDrawTime += aMidTime - aStartTime;
				if (mApp.mShowFPS)
				{
					SexyFramework.Graphics.Graphics g = new SexyFramework.Graphics.Graphics();
					g.DrawImage(gFPSImage, mApp.mWidth - gFPSImage.GetWidth() - 10, mApp.mHeight - gFPSImage.GetHeight() - 10);
				}
				
				int aPreScreenBltTime = timeGetTime();
				mApp.mLastDrawTick = aPreScreenBltTime ;
				ReDraw();
				
				UpdateFTimeAcc();
				
				int aEndTime = timeGetTime();
				mApp.mScreenBltTime = aEndTime - aPreScreenBltTime;
				if (mApp.mLoadingThreadStarted && !mApp.mLoadingThreadCompleted)
				{
					int aTotalTime = aEndTime - aStartTime;
					mApp.mNextDrawTick += 35 + Math.Max(aTotalTime, 15);
					if (aEndTime - mApp.mNextDrawTick >= 0)
					{
						mApp.mNextDrawTick = aEndTime;
					}
				}
				else
				{
					mApp.mNextDrawTick = aEndTime;
				}
				mApp.mHasPendingDraw = false;
				mApp.mCustomCursorDirty = false;
				return true;
			}
			else
			{
				mApp.mHasPendingDraw = false;
				mApp.mLastDrawWasEmpty = true;
			}
			mXNAGraphicsDriver.mXNARenderDevice.PresentScreenImage();
			return false;
		}

		private void DoUpdateFramesF(float theFrac)
		{
			if (mApp.mVSyncUpdates && !mApp.mMinimized)
			{
				mApp.mWidgetManager.UpdateFrameF(theFrac);
			}
		}

		private bool DoUpdateFrames()
		{
			if (mApp.mLoadingThreadCompleted && !mApp.mLoaded)
			{
				mApp.mLoaded = true;
				mApp.mYieldMainThread = false;
				mApp.LoadingThreadCompleted();
			}
			mApp.UpdateFrames();
			return true;
		}
	}
}