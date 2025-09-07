using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SexyFramework.Drivers;
using SexyFramework.Drivers.App;
using SexyFramework.Drivers.Audio;
using SexyFramework.Drivers.File;
using SexyFramework.Drivers.Leaderboard;
using SexyFramework.Drivers.Profile;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Sound;
using SexyFramework.Widget;

namespace SexyFramework
{
	public class SexyAppBase : ButtonListener, DialogListener
	{
		public class Touch
		{
			public IntPtr ident;

			public IntPtr _event;

			public SexyFramework.Misc.Point location = default(SexyFramework.Misc.Point);

			public SexyFramework.Misc.Point previousLocation = default(SexyFramework.Misc.Point);

			public int tapCount;

			public double timestamp;

			public _TouchPhase phase;

			public void SetTouchInfo(SexyFramework.Misc.Point loc, _TouchPhase _phase, double _timestamp)
			{
				previousLocation = location;
				location = loc;
				phase = _phase;
				timestamp = _timestamp / 1000.0;
			}
		}

		public class WidgetSafeDeleteInfo
		{
			public int mUpdateAppDepth;

			public SexyFramework.Widget.Widget mWidget;
		}

		public const int DEMO_FILE_ID = 1119809400;

		public const int DEMO_VERSION = 2;

		public IAppDriver mAppDriver;

		public IAudioDriver mAudioDriver;

		public IGraphicsDriver mGraphicsDriver;

		public IFileDriver mFileDriver;

		public IGamepadDriver mGamepadDriver;

		public IResStreamsDriver mResStreamsDriver;

		public IProfileDriver mProfileDriver;

		public ISaveGameDriver mSaveGameDriver;

		public IHttpDriver mHttpDriver;

		public ILeaderboardDriver mLeaderboardDriver;

		public IAchievementDriver mAchievementDriver;

		public uint mRandSeed;

		public string mCompanyName;

		public string mFullCompanyName;

		public string mProdName;

		public string mRegKey;

		public string mChangeDirTo;

		public int mRelaxUpdateBacklogCount;

		public int mMaxUpdateBacklog;

		public bool mPauseWhenMoving;

		public int mPreferredX;

		public int mPreferredY;

		public int mPreferredWidth;

		public int mPreferredHeight;

		public int mWidth;

		public int mHeight;

		public int mFullscreenBits;

		public double mMusicVolume;

		public double mSfxVolume;

		public double mDemoMusicVolume;

		public double mDemoSfxVolume;

		public bool mNoSoundNeeded;

		public bool mWantFMod;

		public bool mCmdLineParsed;

		public bool mSkipSignatureChecks;

		public bool mStandardWordWrap;

		public bool mbAllowExtendedChars;

		public bool mOnlyAllowOneCopyToRun;

		public uint mNotifyGameMessage;

		public object mCritSect = new object();

		public object mGetImageCritSect = new object();

		public int mBetaValidate;

		public byte[] mAdd8BitMaxTable = new byte[512];

		public WidgetManager mWidgetManager;

		public Dictionary<int, Dialog> mDialogMap = new Dictionary<int, Dialog>();

		public LinkedList<Dialog> mDialogList = new LinkedList<Dialog>();

		public uint mPrimaryThreadId;

		public bool mSEHOccured;

		public bool mShutdown;

		public bool mExitToTop;

		public bool mIsWindowed;

		public bool mIsPhysWindowed;

		public bool mFullScreenWindow;

		public bool mForceFullscreen;

		public bool mForceWindowed;

		public bool mInitialized;

		public bool mProcessInTimer;

		public int mTimeLoaded;

		public bool mIsScreenSaver;

		public bool mAllowMonitorPowersave;

		public bool mWantsDialogCompatibility;

		public bool mNoDefer;

		public bool mFullScreenPageFlip;

		public bool mTabletPC;

		public MusicInterface mMusicInterface;

		public bool mReadFromRegistry;

		public string mRegisterLink;

		public string mProductVersion;

		public Image[] mCursorImages = new Image[13];

		public bool mIsOpeningURL;

		public bool mShutdownOnURLOpen;

		public string mOpeningURL;

		public uint mOpeningURLTime;

		public uint mLastTimerTime;

		public uint mLastBigDelayTime;

		public double mUnmutedMusicVolume;

		public double mUnmutedSfxVolume;

		public int mMuteCount;

		public int mOldMuteCount;

		public int mAutoMuteCount;

		public bool mDemoMute;

		public bool mMuteOnLostFocus;

		public List<MemoryImage> mMemoryImageSet = new List<MemoryImage>();

		public List<ImageFont> mImageFontSet = new List<ImageFont>();

		public List<PIEffect> mPIEffectSet = new List<PIEffect>();

		public List<PopAnim> mPopAnimSet = new List<PopAnim>();

		public Dictionary<KeyValuePair<string, string>, SharedImage> mSharedImageMap = new Dictionary<KeyValuePair<string, string>, SharedImage>();

		public object mImageSetCritSect = new object();

		public bool mCleanupSharedImages;

		public int mNonDrawCount;

		public float mFrameTime;

		public bool mIsDrawing;

		public bool mLastDrawWasEmpty;

		public bool mHasPendingDraw;

		public double mPendingUpdatesAcc;

		public double mUpdateFTimeAcc;

		public int mLastTimeCheck;

		public int mLastTime;

		public int mLastUserInputTick;

		public int mSleepCount;

		public int mDrawCount;

		public Ref<int> mUpdateCount = new Ref<int>(0);

		public int mUpdateAppState;

		public int mUpdateAppDepth;

		public int mMaxNonDrawCount;

		public double mUpdateMultiplier;

		public bool mPaused;

		public int mFastForwardToUpdateNum;

		public bool mFastForwardToMarker;

		public bool mFastForwardStep;

		public int mLastDrawTick;

		public int mNextDrawTick;

		public int mStepMode;

		public int mCursorNum;

		public SoundManager mSoundManager;

		public LinkedList<WidgetSafeDeleteInfo> mSafeDeleteList = new LinkedList<WidgetSafeDeleteInfo>();

		public bool mMouseIn;

		public bool mRunning;

		public bool mActive;

		public bool mMinimized;

		public bool mPhysMinimized;

		public bool mIsDisabled;

		public int mDrawTime;

		public int mFPSStartTick;

		public int mFPSFlipCount;

		public int mFPSDirtyCount;

		public int mFPSTime;

		public int mFPSCount;

		public bool mShowFPS;

		public int mShowFPSMode;

		public double mVFPSUpdateTimes;

		public int mVFPSUpdateCount;

		public double mVFPSDrawTimes;

		public int mVFPSDrawCount;

		public float mCurVFPS;

		public int mScreenBltTime;

		public bool mAutoStartLoadingThread;

		public bool mLoadingThreadStarted;

		public bool mLoadingThreadCompleted;

		public bool mLoaded;

		public bool mReloadingResources;

		public float mReloadPct;

		public string mReloadText;

		public string mReloadSubText;

		public bool mYieldMainThread;

		public bool mLoadingFailed;

		public bool mCursorThreadRunning;

		public bool mSysCursor;

		public bool mCustomCursorsEnabled;

		public bool mCustomCursorDirty;

		public bool mLastShutdownWasGraceful;

		public bool mIsWideWindow;

		public bool mWriteToSexyCache;

		public bool mSexyCacheBuffers;

		public bool mWriteFontCacheDir;

		public int mNumLoadingThreadTasks;

		public int mCompletedLoadingThreadTasks;

		public bool mDebugKeysEnabled;

		public bool mEnableMaximizeButton;

		public bool mCtrlDown;

		public bool mAltDown;

		public bool mAllowAltEnter;

		public int mSyncRefreshRate;

		public bool mVSyncUpdates;

		public bool mNoVSync;

		public bool mVSyncBroken;

		public int mVSyncBrokenCount;

		public long mVSyncBrokenTestStartTick;

		public long mVSyncBrokenTestUpdates;

		public bool mWaitForVSync;

		public bool mSoftVSyncWait;

		public bool mAutoEnable3D;

		public bool mTest3D;

		public bool mNoD3D9;

		public uint mMinVidMemory3D;

		public uint mRecommendedVidMemory3D;

		public bool mWidescreenAware;

		public bool mWidescreenTranslate;

		public Rect mScreenBounds = default(Rect);

		public bool mEnableWindowAspect;

		public bool mAllowWindowResize;

		public int mOrigScreenWidth;

		public int mOrigScreenHeight;

		public bool mIsSizeCursor;

		public bool mFirstLaunch;

		public bool mAppUpdated;

		public Dictionary<string, string> mStringProperties = new Dictionary<string, string>();

		public Dictionary<string, bool> mBoolProperties = new Dictionary<string, bool>();

		public Dictionary<string, int> mIntProperties = new Dictionary<string, int>();

		public Dictionary<string, double> mDoubleProperties = new Dictionary<string, double>();

		public Dictionary<string, List<string>> mStringVectorProperties = new Dictionary<string, List<string>>();

		public ResourceManager mResourceManager;

		public PopLoc mPopLoc = new PopLoc();

		public SharedRenderTarget.Pool mSharedRTPool;

		public EShowCompatInfoMode mShowCompatInfoMode;

		public bool mShowWidgetInspector;

		public bool mWidgetInspectorPickMode;

		public bool mWidgetInspectorLeftAnchor;

		public WidgetContainer mWidgetInspectorPickWidget;

		public WidgetContainer mWidgetInspectorCurWidget;

		public int mWidgetInspectorScrollOffset;

		public FPoint mWidgetInspectorClickPos = default(FPoint);

		public ResStreamsManager mResStreamsManager;

		public ProfileManager mProfileManager;

		public LeaderboardManager mLeaderboardManager;

		public bool mAllowSwapScreenImage;

		public static bool sAttemptingNonRecommended3D;

		public int mGamepadLocked;

		public bool mHasFocus;

		private List<KeyValuePair<string, string>> mRemoveList = new List<KeyValuePair<string, string>>();

		public SexyAppBase()
		{
			mFirstLaunch = false;
			mAppUpdated = false;
			mResStreamsManager = null;
			mGamepadLocked = -1;
			mAllowSwapScreenImage = true;
			mMaxUpdateBacklog = 200;
			mPauseWhenMoving = true;
			mGraphicsDriver = null;
			mProfileManager = null;
			mLeaderboardManager = null;
			InitFileDriver();
			mAppDriver = WP7AppDriver.CreateAppDriver(this);
			mGamepadDriver = XNAGamepadDriver.CreateGamepadDriver();
			mAudioDriver = WP7AudioDriver.CreateAudioDriver(this);
			mProfileDriver = IProfileDriver.CreateProfileDriver();
			GlobalMembers.gSexyAppBase = this;
			mAppDriver.InitAppDriver();
			mWidgetManager = new WidgetManager(this);
		}

		public virtual void Dispose()
		{
			Dictionary<int, Dialog>.Enumerator enumerator = mDialogMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SexyFramework.Widget.Widget value = enumerator.Current.Value;
				if (value.mParent != null)
				{
					value.mParent.RemoveWidget(value);
				}
			}
			mDialogMap.Clear();
			mDialogList.Clear();
			mWidgetManager = null;
			mResourceManager = null;
			Dictionary<KeyValuePair<string, string>, SharedImage>.Enumerator enumerator2 = mSharedImageMap.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				SharedImage value2 = enumerator2.Current.Value;
				if (value2.mImage != null)
				{
					value2.mImage.Dispose();
					value2.mImage = null;
				}
			}
			mSharedImageMap.Clear();
			if (mSharedRTPool != null)
			{
				mSharedRTPool.Dispose();
				mSharedRTPool = null;
			}
			mAppDriver.Shutdown();
			mProfileManager = null;
			mLeaderboardManager = null;
			mAudioDriver = null;
			mGamepadDriver = null;
			mSaveGameDriver = null;
			mProfileDriver = null;
			mHttpDriver = null;
			mLeaderboardDriver = null;
			mAchievementDriver = null;
			mAppDriver = null;
			mResStreamsManager = null;
			mFileDriver = null;
			if (GlobalMembers.gFileDriver != null)
			{
				GlobalMembers.gFileDriver.Dispose();
			}
			GlobalMembers.gFileDriver = null;
			GlobalMembers.gSexyAppBase = null;
		}

		public virtual void ClearUpdateBacklog(bool relaxForASecond)
		{
			mAppDriver.ClearUpdateBacklog(relaxForASecond);
		}

		public virtual bool IsScreenSaver()
		{
			return mIsScreenSaver;
		}

		public virtual bool AppCanRestore()
		{
			return !mIsDisabled;
		}

		public virtual Dialog NewDialog(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
		{
			return new Dialog(null, null, theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode);
		}

		public virtual Dialog DoDialog(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
		{
			KillDialog(theDialogId);
			Dialog dialog = NewDialog(theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode);
			AddDialog(theDialogId, dialog);
			return dialog;
		}

		public Dialog GetDialog(int theDialogId)
		{
			if (mDialogMap.ContainsKey(theDialogId))
			{
				return mDialogMap[theDialogId];
			}
			return null;
		}

		public virtual bool KillDialog(int theDialogId, bool removeWidget, bool deleteWidget)
		{
			if (mDialogMap.ContainsKey(theDialogId))
			{
				Dialog dialog = mDialogMap[theDialogId];
				if (dialog.mResult == -1)
				{
					dialog.mResult = 0;
				}
				if (mDialogList.Contains(dialog))
				{
					mDialogList.Remove(dialog);
				}
				mDialogMap.Remove(theDialogId);
				if ((removeWidget || deleteWidget) && dialog.mParent != null)
				{
					dialog.mParent.RemoveWidget(dialog);
				}
				if (dialog.IsModal())
				{
					ModalClose();
					mWidgetManager.RemoveBaseModal(dialog);
				}
				if (deleteWidget)
				{
					SafeDeleteWidget(dialog);
				}
				return true;
			}
			return false;
		}

		public virtual bool KillDialog(int theDialogId)
		{
			return KillDialog(theDialogId, true, true);
		}

		public virtual bool KillDialog(Dialog theDialog)
		{
			return KillDialog(theDialog.mId);
		}

		public virtual int GetDialogCount()
		{
			return mDialogMap.Count;
		}

		public virtual void AddDialog(int theDialogId, Dialog theDialog, FlagsMod belowModalFlagsMod)
		{
			KillDialog(theDialogId);
			if (theDialog.mWidth == 0)
			{
				int num = mWidth / 2;
				theDialog.Resize((mWidth - num) / 2, mHeight / 5, num, theDialog.GetPreferredHeight(num));
			}
			mDialogMap[theDialogId] = theDialog;
			mDialogList.AddLast(theDialog);
			mWidgetManager.AddWidget(theDialog);
			if (theDialog.IsModal())
			{
				mWidgetManager.AddBaseModal(theDialog, belowModalFlagsMod);
				ModalOpen();
			}
		}

		public virtual void AddDialog(int theDialogId, Dialog theDialog)
		{
			AddDialog(theDialogId, theDialog, mWidgetManager.mDefaultBelowModalFlagsMod);
		}

		public virtual void AddDialog(Dialog theDialog)
		{
			AddDialog(theDialog.mId, theDialog);
		}

		public virtual void ModalOpen()
		{
		}

		public virtual void ModalClose()
		{
		}

		public virtual void DialogButtonPress(int theDialogId, int theButtonId)
		{
		}

		public virtual void DialogButtonDepress(int theDialogId, int theButtonId)
		{
		}

		public virtual void GotFocus()
		{
		}

		public virtual void LostFocus()
		{
		}

		public virtual void URLOpenFailed(string theURL)
		{
			mIsOpeningURL = false;
		}

		public void URLOpenSucceeded(string theURL)
		{
			mIsOpeningURL = false;
			if (mShutdownOnURLOpen)
			{
				Shutdown();
			}
		}

		public bool OpenURL(string theURL, bool shutdownOnOpen)
		{
			return mAppDriver.OpenURL(theURL, shutdownOnOpen);
		}

		public virtual void SetCursorImage(int theCursorNum, Image theImage)
		{
			mAppDriver.SetCursorImage(theCursorNum, theImage);
		}

		public virtual void SetCursor(ECURSOR eCURSOR)
		{
			mAppDriver.SetCursor((int)eCURSOR);
		}

		public virtual int GetCursor()
		{
			return mAppDriver.GetCursor();
		}

		public virtual void EnableCustomCursors(bool enabled)
		{
			mAppDriver.EnableCustomCursors(enabled);
		}

		public virtual double GetLoadingThreadProgress()
		{
			return mAppDriver.GetLoadingThreadProgress();
		}

		public void CopyToClipboard(string theString)
		{
			mAppDriver.CopyToClipboard(theString);
		}

		public string GetClipboard()
		{
			return mAppDriver.GetClipboard();
		}

		public virtual bool RegistryWriteString(string theValueName, string theString)
		{
			return mAppDriver.ConfigWriteString(theValueName, theString);
		}

		public virtual bool RegistryWriteInteger(string theValueName, int theValue)
		{
			return mAppDriver.ConfigWriteInteger(theValueName, theValue);
		}

		public virtual bool RegistryWriteBoolean(string theValueName, bool theValue)
		{
			return mAppDriver.ConfigWriteBoolean(theValueName, theValue);
		}

		public virtual bool RegistryWriteData(string theValueName, byte[] theValue, ulong theLength)
		{
			return mAppDriver.ConfigWriteData(theValueName, theValue, theLength);
		}

		public virtual void WriteToRegistry()
		{
			RegistryWriteInteger("MusicVolume", (int)(mMusicVolume * 100.0));
			RegistryWriteInteger("SfxVolume", (int)(mSfxVolume * 100.0));
			RegistryWriteInteger("Muted", (mMuteCount - mAutoMuteCount > 0) ? 1 : 0);
			RegistryWriteInteger("ScreenMode", (!mIsWindowed) ? 1 : 0);
			RegistryWriteInteger("PreferredX", mPreferredX);
			RegistryWriteInteger("PreferredY", mPreferredY);
			RegistryWriteInteger("PreferredWidth", mPreferredWidth);
			RegistryWriteInteger("PreferredHeight", mPreferredHeight);
			RegistryWriteInteger("CustomCursors", mCustomCursorsEnabled ? 1 : 0);
			RegistryWriteInteger("InProgress", 0);
			RegistryWriteBoolean("WaitForVSync", mWaitForVSync);
			mAppDriver.WriteToConfig();
		}

		public virtual bool RegistryEraseKey(string _theKeyName)
		{
			return mAppDriver.ConfigEraseKey(_theKeyName);
		}

		public virtual void RegistryEraseValue(string _theValueName)
		{
			mAppDriver.ConfigEraseValue(_theValueName);
		}

		public virtual bool RegistryGetSubKeys(string theKeyName, List<string> theSubKeys)
		{
			return mAppDriver.ConfigGetSubKeys(theKeyName, theSubKeys);
		}

		public virtual bool RegistryReadString(string theKey, ref string theString)
		{
			return mAppDriver.ConfigReadString(theKey, ref theString);
		}

		public virtual bool RegistryReadInteger(string theKey, ref int theValue)
		{
			return mAppDriver.ConfigReadInteger(theKey, ref theValue);
		}

		public virtual bool RegistryReadBoolean(string theKey, ref bool theValue)
		{
			return mAppDriver.ConfigReadBoolean(theKey, ref theValue);
		}

		public virtual bool RegistryReadData(string theKey, byte[] theValue, ref ulong theLength)
		{
			return mAppDriver.ConfigReadData(theKey, ref theValue, ref theLength);
		}

		public virtual void ReadFromRegistry()
		{
			mReadFromRegistry = true;
			mRegKey = GetString("RegistryKey", string.Empty);
			int theValue = 0;
			if (RegistryReadInteger("MusicVolume", ref theValue))
			{
				mMusicVolume = (double)theValue / 100.0;
			}
			if (RegistryReadInteger("SfxVolume", ref theValue))
			{
				mSfxVolume = (double)theValue / 100.0;
			}
			if (RegistryReadInteger("Muted", ref theValue))
			{
				mMuteCount = theValue;
			}
			if (RegistryReadInteger("ScreenMode", ref theValue))
			{
				mIsWindowed = theValue == 0 && !mForceFullscreen;
			}
			RegistryReadInteger("PreferredX", ref mPreferredX);
			RegistryReadInteger("PreferredY", ref mPreferredY);
			RegistryReadInteger("PreferredWidth", ref mPreferredWidth);
			RegistryReadInteger("PreferredHeight", ref mPreferredHeight);
			if (RegistryReadInteger("CustomCursors", ref theValue))
			{
				EnableCustomCursors(theValue != 0);
			}
			RegistryReadBoolean("WaitForVSync", ref mWaitForVSync);
			if (RegistryReadInteger("InProgress", ref theValue))
			{
				mLastShutdownWasGraceful = theValue == 0;
			}
			if (!IsScreenSaver())
			{
				RegistryWriteInteger("InProgress", 1);
			}
			mAppDriver.ReadFromConfig();
		}

		public void RegistrySave()
		{
			mAppDriver.SaveConfig();
		}

		public virtual bool WriteBytesToFile(string theFileName, byte[] theData, ulong theDataLen)
		{
			return mAppDriver.WriteBytesToFile(theFileName, theData, theDataLen);
		}

		public virtual bool WriteBufferToFile(string theFileName, SexyFramework.Misc.Buffer theBuffer)
		{
			return WriteBytesToFile(theFileName, theBuffer.GetDataPtr(), (ulong)theBuffer.GetDataLen());
		}

		public bool ReadBufferFromStream(string theFileName, ref SexyFramework.Misc.Buffer theBuffer)
		{
			theBuffer.Clear();
			try
			{
				Stream stream = TitleContainer.OpenStream("Content\\" + theFileName);
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, (int)stream.Length);
				stream.Close();
				theBuffer.SetData(array, array.Length);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public bool ReadBufferFromStorage(string theFileName, SexyFramework.Misc.Buffer theBuffer)
		{
			return mAppDriver.ReadBufferFromFile(theFileName, theBuffer, false);
		}

		public bool ReadBufferFromFile(string theFileName, SexyFramework.Misc.Buffer theBuffer)
		{
			PFILE pFILE = new PFILE(theFileName, "rb");
			if (!pFILE.Open())
			{
				return false;
			}
			byte[] data = pFILE.GetData();
			int theCount = data.Length;
			theBuffer.Clear();
			theBuffer.SetData(data, theCount);
			data = null;
			pFILE.Close();
			return true;
		}

		public virtual bool FileExists(string theFileName)
		{
			bool isFolder = false;
			return mFileDriver.FileExists(theFileName, ref isFolder);
		}

		public virtual bool EraseFile(string theFileName)
		{
			return mFileDriver.DeleteFile(theFileName);
		}

		public virtual void ShutdownHook()
		{
		}

		public virtual void Shutdown()
		{
			mAppDriver.Shutdown();
		}

		public virtual void DoExit(int theCode)
		{
			mAppDriver.DoExit(theCode);
		}

		protected virtual void ReorderSystemButtonHandler(SystemButtons button, List<SexyFramework.Widget.Widget> handlers)
		{
		}

		public virtual void UpdateFrames()
		{
			mUpdateCount.value++;
			mGamepadDriver.Update();
			if (!mMinimized)
			{
				if (mWidgetManager.UpdateFrame())
				{
					mFPSDirtyCount++;
				}
				if (GlobalMembers.IsBackButtonPressed)
				{
					SystemButtonPressedArgs systemButtonPressedArgs = new SystemButtonPressedArgs();
					systemButtonPressedArgs.processed = false;
					systemButtonPressedArgs.button = SystemButtons.Back;
					List<SexyFramework.Widget.Widget> list = new List<SexyFramework.Widget.Widget>();
					foreach (SexyFramework.Widget.Widget mWidget in mWidgetManager.mWidgets)
					{
						if (mWidget.mVisible && !mWidget.mDisabled)
						{
							list.Add(mWidget);
						}
					}
					ReorderSystemButtonHandler(SystemButtons.Back, list);
					foreach (SexyFramework.Widget.Widget item in list)
					{
						item.SignalSystemButtonPressed(systemButtonPressedArgs);
						if (systemButtonPressedArgs.processed)
						{
							break;
						}
					}
					GlobalMembers.IsBackButtonPressed = false;
				}
			}
			if (mResStreamsManager != null)
			{
				mResStreamsManager.Update();
			}
			if (mSoundManager != null)
			{
				mSoundManager.Update();
			}
			if (mMusicInterface != null)
			{
				mMusicInterface.Update();
			}
			if (mSaveGameDriver != null)
			{
				mSaveGameDriver.Update();
			}
			if (mProfileManager != null)
			{
				mProfileManager.Update();
			}
			if (mHttpDriver != null)
			{
				mHttpDriver.Update();
			}
			if (mLeaderboardManager != null)
			{
				mLeaderboardManager.Update();
			}
			if (mAchievementDriver != null)
			{
				mAchievementDriver.Update();
			}
			CleanSharedImages();
		}

		public virtual void BeginPopup()
		{
			mAppDriver.BeginPopup();
		}

		public virtual void EndPopup()
		{
			mAppDriver.EndPopup();
		}

		public virtual int MsgBox(string theText, string theTitle, int theFlags)
		{
			return 0;
		}

		public virtual void Popup(string theString)
		{
			mAppDriver.Popup(theString);
		}

		public void SafeDeleteWidget(SexyFramework.Widget.Widget theWidget)
		{
			WidgetSafeDeleteInfo widgetSafeDeleteInfo = new WidgetSafeDeleteInfo();
			widgetSafeDeleteInfo.mUpdateAppDepth = mUpdateAppDepth;
			widgetSafeDeleteInfo.mWidget = theWidget;
			mSafeDeleteList.AddLast(widgetSafeDeleteInfo);
		}

		public virtual bool KeyDown(int theKey)
		{
			return mAppDriver.KeyDown(theKey);
		}

		public virtual bool DebugKeyDown(int theKey)
		{
			return mAppDriver.DebugKeyDown(theKey);
		}

		public virtual bool DebugKeyDownAsync(int theKey, bool ctrlDown, bool altDown)
		{
			return false;
		}

		public virtual void ShowKeyboard()
		{
			mAppDriver.ShowKeyboard();
		}

		public virtual void HideKeyboard()
		{
			mAppDriver.HideKeyboard();
		}

		public virtual void TouchBegan(Touch theTouch)
		{
			mWidgetManager.TouchBegan(theTouch);
		}

		public virtual void TouchEnded(Touch theTouch)
		{
			mWidgetManager.TouchEnded(theTouch);
		}

		public virtual void TouchMoved(Touch theTouch)
		{
			mWidgetManager.TouchMoved(theTouch);
		}

		public virtual void TouchesCanceled()
		{
			mWidgetManager.TouchesCanceled();
		}

		public virtual void GamepadButtonDown(GamepadButton theButton, int thePlayer, uint theFlags)
		{
			mWidgetManager.GamepadButtonDown(theButton, thePlayer, theFlags);
		}

		public virtual void GamepadButtonUp(GamepadButton theButton, int thePlayer, uint theFlags)
		{
			mWidgetManager.GamepadButtonUp(theButton, thePlayer, theFlags);
		}

		public virtual void GamepadAxisMove(GamepadAxis theAxis, int thePlayer, float theAxisValue)
		{
			mWidgetManager.GamepadAxisMove(theAxis, thePlayer, theAxisValue);
		}

		public virtual bool IsUIOrientationAllowed(UI_ORIENTATION theOrientation)
		{
			return mAppDriver.IsUIOrientationAllowed(theOrientation);
		}

		public virtual void UIOrientationChanged(UI_ORIENTATION theOrientation)
		{
		}

		public virtual UI_ORIENTATION GetUIOrientation()
		{
			return mAppDriver.GetUIOrientation();
		}

		public virtual void CloseRequestAsync()
		{
		}

		public virtual void Done3dTesting()
		{
		}

		public virtual string NotifyCrashHook()
		{
			return "";
		}

		public virtual void DeleteNativeImageData()
		{
		}

		public virtual void DeleteExtraImageData()
		{
		}

		public virtual void ReInitImages()
		{
		}

		public virtual void LoadingThreadProc()
		{
		}

		public virtual void LoadingThreadCompleted()
		{
		}

		public virtual void StartLoadingThread()
		{
			mAppDriver.StartLoadingThread();
		}

		public virtual void SwitchScreenMode(bool wantWindowed, bool is3d, bool force)
		{
		}

		public virtual void SwitchScreenMode(bool wantWindowed)
		{
		}

		public virtual void SwitchScreenMode()
		{
		}

		public void ProcessSafeDeleteList()
		{
			LinkedList<WidgetSafeDeleteInfo>.Enumerator enumerator = mSafeDeleteList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WidgetSafeDeleteInfo current = enumerator.Current;
				if (current.mWidget != null)
				{
					current.mWidget.Dispose();
					current.mWidget = null;
				}
			}
			mSafeDeleteList.Clear();
		}

		public virtual bool UpdateAppStep(ref bool updated)
		{
			return mAppDriver.UpdateAppStep(ref updated);
		}

		public virtual bool Update(int gameTime)
		{
			bool updated = false;
			do
			{
				if (!UpdateAppStep(ref updated))
				{
					return false;
				}
			}
			while (!updated);
			return true;
		}

		public virtual void Draw(int time)
		{
			if (mAppDriver != null)
			{
				mAppDriver.Draw();
			}
		}

		public virtual string GetGameSEHInfo()
		{
			return mAppDriver.GetGameSEHInfo();
		}

		public virtual void PreTerminate()
		{
		}

		public virtual void Start()
		{
			mAppDriver.Start();
		}

		public virtual bool CheckSignature(SexyFramework.Misc.Buffer theBuffer, string theFileName)
		{
			return mAppDriver.CheckSignature(theBuffer, theFileName);
		}

		public virtual bool LoadProperties(string theFileName, bool required, bool checkSig, bool needsLocaleCorrection)
		{
			SexyFramework.Misc.Buffer buffer = new SexyFramework.Misc.Buffer();
			if (!ReadBufferFromFile(theFileName, buffer))
			{
				bool flag = false;
				if (needsLocaleCorrection && mResourceManager != null)
				{
					buffer.Clear();
					flag = ReadBufferFromFile(mResourceManager.GetLocaleFolder(true) + theFileName, buffer);
				}
				if (!flag)
				{
					if (!required)
					{
						return true;
					}
					Popup(GetString("UNABLE_OPEN_PROPERTIES", "Unable to open properties file ") + theFileName);
					return false;
				}
			}
			if (checkSig && !CheckSignature(buffer, theFileName))
			{
				Popup(GetString("PROPERTIES_SIG_FAILED", "Signature check failed on ") + theFileName + "'");
				return false;
			}
			PropertiesParser propertiesParser = new PropertiesParser(this);
			if (!propertiesParser.ParsePropertiesBuffer(buffer.GetDataPtr()))
			{
				Popup(propertiesParser.GetErrorText());
				return false;
			}
			return true;
		}

		public virtual bool LoadProperties()
		{
			return LoadProperties("properties/default.xml", true, false, true);
		}

		public virtual void LoadResourceManifest()
		{
			if (!mResourceManager.ParseResourcesFile("properties/resources.xml"))
			{
				ShowResourceError(true);
			}
		}

		public virtual void ShowResourceError(bool doExit)
		{
			Popup(mResourceManager.GetErrorText());
			if (doExit)
			{
				DoExit(0);
			}
		}

		public virtual bool ReloadAllResources()
		{
			return mAppDriver.ReloadAllResources();
		}

		public virtual bool GetBoolean(string theId, bool theDefault)
		{
			if (mBoolProperties.ContainsKey(theId))
			{
				return mBoolProperties[theId];
			}
			return theDefault;
		}

		public virtual bool GetBoolean(string theId)
		{
			return GetBoolean(theId, false);
		}

		public virtual int GetInteger(string theId)
		{
			return GetInteger(theId, 0);
		}

		public virtual int GetInteger(string theId, int theDefault)
		{
			if (mIntProperties.ContainsKey(theId))
			{
				return mIntProperties[theId];
			}
			return theDefault;
		}

		public virtual double GetDouble(string theId)
		{
			return GetDouble(theId, 0.0);
		}

		public virtual double GetDouble(string theId, double theDefault)
		{
			if (mDoubleProperties.ContainsKey(theId))
			{
				return mDoubleProperties[theId];
			}
			return theDefault;
		}

		public virtual string GetString(string theId, string theDefault)
		{
			if (mStringProperties.ContainsKey(theId))
			{
				return mStringProperties[theId];
			}
			return theDefault;
		}

		public virtual string GetString(string theId)
		{
			return GetString(theId, "");
		}

		public virtual List<string> GetStringVector(string theId)
		{
			if (mStringVectorProperties.ContainsKey(theId))
			{
				return mStringVectorProperties[theId];
			}
			return new List<string>();
		}

		public virtual void SetString(string anID, string value)
		{
			if (!mStringProperties.ContainsKey(anID))
			{
				mStringProperties[anID] = value;
			}
			int result = 0;
			if (int.TryParse(anID, out result))
			{
				mPopLoc.SetString(result, value, false);
			}
		}

		public virtual void SetBoolean(string anID, bool boolValue)
		{
			if (!mBoolProperties.ContainsKey(anID))
			{
				mBoolProperties[anID] = boolValue;
			}
		}

		public virtual void SetInteger(string anID, int anInt)
		{
			if (!mIntProperties.ContainsKey(anID))
			{
				mIntProperties[anID] = anInt;
			}
		}

		public virtual void SetDouble(string anID, double aDouble)
		{
			if (!mDoubleProperties.ContainsKey(anID))
			{
				mDoubleProperties[anID] = aDouble;
			}
		}

		public virtual void DoParseCmdLine()
		{
		}

		public virtual void ParseCmdLine(string theCmdLine)
		{
		}

		public virtual void HandleCmdLineParam(string theParamName, string theParamValue)
		{
		}

		public virtual void PreDisplayHook()
		{
		}

		public virtual void PreDDInterfaceInitHook()
		{
		}

		public virtual void PostDDInterfaceInitHook()
		{
		}

		public virtual bool ChangeDirHook(string theIntendedPath)
		{
			return false;
		}

		public virtual void InitPropertiesHook()
		{
		}

		public virtual void InitHook()
		{
		}

		public virtual MusicInterface CreateMusicInterface()
		{
			if (mNoSoundNeeded || mAudioDriver == null)
			{
				return new MusicInterface();
			}
			return mAudioDriver.CreateMusicInterface();
		}

		public virtual void Init()
		{
			if (mAudioDriver != null)
			{
				mAudioDriver.InitAudioDriver();
			}
			if (mAppDriver != null)
			{
				mAppDriver.Init();
			}
			if (mProfileDriver != null)
			{
				mProfileDriver.Init();
			}
			if (mSaveGameDriver != null)
			{
				mSaveGameDriver.Init();
			}
			if (mLeaderboardDriver != null)
			{
				mLeaderboardDriver.Init();
			}
			if (mAchievementDriver != null)
			{
				mAchievementDriver.Init();
			}
			if (mGamepadDriver != null)
			{
				mGamepadDriver.InitGamepadDriver(this);
			}
			long tick = DateTime.Now.Ticks;

			mResourceManager = new ResourceManager(this);
			if (!mResourceManager.ParseResourcesFile("properties/resources.xml"))
			{
				mResourceManager = null;
				mResourceManager = ((XNAFileDriver)mFileDriver).GetContentManager()
					.Load<ResourceManager>("properties/resources.xml");
				mResourceManager.mApp = this;
			}
			long tick2 = DateTime.Now.Ticks;
		}

		public virtual void HandleGameAlreadyRunning()
		{
		}

		public virtual DeviceImage GetImage(string theFileName, bool commitBits, bool allowTriReps, bool isInAtlas)
		{
			if (isInAtlas)
			{
				allowTriReps = false;
			}
			if (!isInAtlas && mResStreamsManager != null && mResStreamsManager.IsInitialized())
			{
				string theFileName2 = theFileName + ".ptx";
				int groupForFile = mResStreamsManager.GetGroupForFile(theFileName2);
				if (groupForFile != -1 && (mResStreamsManager.IsGroupLoaded(groupForFile) || mResStreamsManager.ForceLoadGroup(groupForFile, theFileName)))
				{
					Image img = new Image();
					if (mResStreamsManager.GetImage(groupForFile, theFileName2, ref img))
					{
						return (DeviceImage)img;
					}
				}
			}
			if (!isInAtlas)
			{
				DeviceImage deviceImage = null;
				deviceImage = mAppDriver.GetOptimizedImage(theFileName, commitBits, allowTriReps);
				if (deviceImage != null)
				{
					return deviceImage;
				}
			}
			DeviceImage deviceImage2 = null;
			if (isInAtlas)
			{
				deviceImage2 = new DeviceImage();
				if (!allowTriReps)
				{
					deviceImage2.AddImageFlags(ImageFlags.ImageFlag_NoTriRep);
				}
				deviceImage2.mWidth = (deviceImage2.mHeight = 0);
				deviceImage2.mFilePath = theFileName;
				return deviceImage2;
			}
			throw new NotSupportedException();
		}

		protected virtual MemoryImage GetImageByName(string name)
		{
			return mResourceManager.LoadImage(name)?.GetMemoryImage();
		}

		public virtual void ColorizeImage(SharedImageRef theImage, SexyFramework.Graphics.Color theColor)
		{
		}

		public virtual DeviceImage CreateColorizedImage(Image theImage, SexyFramework.Graphics.Color theColor)
		{
			return null;
		}

		public virtual DeviceImage CopyImage(Image theImage, Rect theRect)
		{
			return null;
		}

		public virtual DeviceImage CopyImage(Image theImage)
		{
			return null;
		}

		public virtual void MirrorImage(Image theImage)
		{
		}

		public virtual void FlipImage(Image theImage)
		{
		}

		public virtual void RotateImageHue(MemoryImage theImage, int theDelta)
		{
		}

		public virtual void AddMemoryImage(MemoryImage memoryImage)
		{
			if (mGraphicsDriver == null)
			{
				return;
			}
			lock (mImageSetCritSect)
			{
				mMemoryImageSet.Add(memoryImage);
			}
		}

		public virtual void RemoveMemoryImage(MemoryImage theMemoryImage)
		{
			if (mGraphicsDriver == null)
			{
				return;
			}
			lock (mImageSetCritSect)
			{
				if (mMemoryImageSet.Contains(theMemoryImage))
				{
					mMemoryImageSet.Remove(theMemoryImage);
				}
				Remove3DData(theMemoryImage);
			}
		}

		public virtual void Remove3DData(MemoryImage theMemoryImage)
		{
			mAppDriver.Remove3DData(theMemoryImage);
		}

		public virtual SharedImageRef SetSharedImage(string theFileName, string theVariant, DeviceImage anImage)
		{
			bool isNew = false;
			return SetSharedImage(theFileName, theVariant, anImage, ref isNew);
		}

		public virtual SharedImageRef SetSharedImage(string theFileName, string theVariant, DeviceImage theImage, ref bool isNew)
		{
			string key = theFileName.ToUpper();
			string value = theVariant.ToUpper();
			KeyValuePair<string, string> key2 = new KeyValuePair<string, string>(key, value);
			SharedImageRef sharedImageRef = null;
			sharedImageRef = new SharedImageRef();
			if (mSharedImageMap.ContainsKey(key2))
			{
				SharedImage sharedImage = new SharedImage();
				lock (mCritSect)
				{
					mSharedImageMap[key2] = sharedImage;
					sharedImageRef.mSharedImage = sharedImage;
					isNew = true;
				}
			}
			else
			{
				sharedImageRef.mSharedImage = mSharedImageMap[key2];
			}
			if (sharedImageRef.mSharedImage.mImage != theImage)
			{
				sharedImageRef.mSharedImage.mImage.Dispose();
				sharedImageRef.mSharedImage.mImage = theImage;
			}
			return sharedImageRef;
		}

		public virtual SharedImageRef CheckSharedImage(string theFileName, string theVariant)
		{
			string text = "";
			int num = theFileName.IndexOf('|');
			if (num != -1)
			{
				ResourceRef imageRef = mResourceManager.GetImageRef(theFileName.Substring(num + 1));
				if (imageRef.HasResource())
				{
					return imageRef.GetSharedImageRef();
				}
				text = theFileName.Substring(0, num);
			}
			else
			{
				text = theFileName;
			}
			string key = text.ToUpper();
			string value = theVariant.ToUpper();
			SharedImageRef result = new SharedImageRef();
			lock (mCritSect)
			{
				KeyValuePair<string, string> key2 = new KeyValuePair<string, string>(key, value);
				if (mSharedImageMap.ContainsKey(key2))
				{
					result = new SharedImageRef(mSharedImageMap[key2]);
				}
			}
			return result;
		}

		public virtual SharedImageRef GetSharedImage(string aPath)
		{
			bool isNew = false;
			return GetSharedImage(aPath, "", ref isNew, true, false);
		}

		public virtual SharedImageRef GetSharedImage(string theFileName, string theVariant, ref bool isNew, bool allowTriReps, bool isInAtlas)
		{
			string text = "";
			int num = theFileName.IndexOf('|');
			if (num != -1)
			{
				ResourceRef imageRef = mResourceManager.GetImageRef(theFileName.Substring(num + 1));
				if (imageRef.HasResource())
				{
					return imageRef.GetSharedImageRef();
				}
				text = theFileName.Substring(0, num);
			}
			else
			{
				text = theFileName;
			}
			string key = text.ToUpper();
			string value = theVariant.ToUpper();
			SharedImageRef sharedImageRef = null;
			lock (mCritSect)
			{
				KeyValuePair<string, string> key2 = new KeyValuePair<string, string>(key, value);
				if (mSharedImageMap.ContainsKey(key2))
				{
					sharedImageRef = new SharedImageRef(mSharedImageMap[key2]);
					sharedImageRef.mSharedImage.mLoading = true;
				}
				else
				{
					mSharedImageMap[key2] = new SharedImage();
					sharedImageRef = new SharedImageRef(mSharedImageMap[key2]);
					sharedImageRef.mSharedImage.mLoading = true;
					isNew = true;
				}
			}
			if (sharedImageRef != null)
			{
				if (isInAtlas)
				{
					allowTriReps = false;
				}
				if (text.Length > 0 && text[0] == '!')
				{
					sharedImageRef.mSharedImage.mImage = new DeviceImage();
					if (!allowTriReps)
					{
						sharedImageRef.mSharedImage.mImage.AddImageFlags(ImageFlags.ImageFlag_NoTriRep);
					}
				}
				else
				{
					sharedImageRef.mSharedImage.mImage = GetImage(text, false, allowTriReps, isInAtlas);
				}
				sharedImageRef.mSharedImage.mLoading = false;
			}
			return sharedImageRef;
		}

		public SharedRenderTarget.Pool GetSharedRenderTargetPool()
		{
			if (mSharedRTPool == null)
			{
				mSharedRTPool = new SharedRenderTarget.Pool();
			}
			return mSharedRTPool;
		}

		public virtual void CleanSharedImages()
		{
			lock (mCritSect)
			{
				if (!mCleanupSharedImages)
				{
					return;
				}
				Dictionary<KeyValuePair<string, string>, SharedImage>.Enumerator enumerator = mSharedImageMap.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SharedImage value = enumerator.Current.Value;
					if (value.mImage != null && value.mRefCount == 0)
					{
						value.mImage.Dispose();
						value.mImage = null;
						mRemoveList.Add(enumerator.Current.Key);
					}
				}
				for (int i = 0; i < mRemoveList.Count; i++)
				{
					mSharedImageMap.Remove(mRemoveList[i]);
				}
				mRemoveList.Clear();
				mCleanupSharedImages = false;
			}
		}

		public ulong HSLToRGB(int h, int s, int l)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			double num4 = ((l < 128) ? (l * (255 + s) / 255) : (l + s - l * s / 255));
			int num5 = (int)((double)(2 * l) - num4);
			int num6 = 6 * h / 256;
			int num7 = (int)((double)num5 + (num4 - (double)num5) * (double)((h - num6 * 256 / 6) * 6) / 255.0);
			if (num7 > 255)
			{
				num7 = 255;
			}
			int num8 = (int)(num4 - (num4 - (double)num5) * (double)((h - num6 * 256 / 6) * 6) / 255.0);
			if (num8 < 0)
			{
				num8 = 0;
			}
			switch (num6)
			{
			case 0:
				num = (int)num4;
				num2 = num7;
				num3 = num5;
				break;
			case 1:
				num = num8;
				num2 = (int)num4;
				num3 = num5;
				break;
			case 2:
				num = num5;
				num2 = (int)num4;
				num3 = num7;
				break;
			case 3:
				num = num5;
				num2 = num8;
				num3 = (int)num4;
				break;
			case 4:
				num = num7;
				num2 = num5;
				num3 = (int)num4;
				break;
			case 5:
				num = (int)num4;
				num2 = num5;
				num3 = num8;
				break;
			default:
				num = (int)num4;
				num2 = num7;
				num3 = num5;
				break;
			}
			return (ulong)(0xFF000000u | ((long)num << 16) | ((long)num2 << 8) | num3);
		}

		public ulong RGBToHSL(int r, int g, int b)
		{
			int num = Math.Max(r, Math.Max(g, b));
			int num2 = Math.Min(r, Math.Min(g, b));
			int num3 = 0;
			int num4 = 0;
			int num5 = (num2 + num) / 2;
			int num6 = num - num2;
			if (num6 != 0)
			{
				num4 = num6 * 256 / ((num5 <= 128) ? (num2 + num) : (512 - num - num2));
				num3 = ((r == num) ? ((g == num2) ? (1280 + (num - b) * 256 / num6) : (256 - (num - g) * 256 / num6)) : ((g != num) ? ((r == num2) ? (768 + (num - g) * 256 / num6) : (1280 - (num - r) * 256 / num6)) : ((b == num2) ? (256 + (num - r) * 256 / num6) : (768 - (num - b) * 256 / num6))));
				num3 /= 6;
			}
			return (ulong)(0xFF000000u | num3 | (num4 << 8) | (num5 << 16));
		}

		public void HSLToRGB(ulong[] theSource, ulong[] theDest, int theSize)
		{
			for (int i = 0; i < theSize; i++)
			{
				ulong num = theSource[i];
				theDest[i] = (num & 0xFF000000u) | (HSLToRGB((int)(num & 0xFF), (int)(num >> 8) & 0xFF, (int)(num >> 16) & 0xFF) & 0xFFFFFF);
			}
		}

		public void RGBToHSL(ulong[] theSource, ulong[] theDest, int theSize)
		{
			for (int i = 0; i < theSize; i++)
			{
				ulong num = theSource[i];
				theDest[i] = (num & 0xFF000000u) | (RGBToHSL((int)((num >> 16) & 0xFF), (int)(num >> 8) & 0xFF, (int)(num & 0xFF)) & 0xFFFFFF);
			}
		}

		public virtual void PlaySample(int theSoundNum)
		{
			if (mSoundManager != null)
			{
				mSoundManager.GetSoundInstance(theSoundNum)?.Play(false, true);
			}
		}

		public virtual void PlaySample(int theSoundNum, int thePan)
		{
			if (mSoundManager != null)
			{
				SoundInstance soundInstance = mSoundManager.GetSoundInstance(theSoundNum);
				if (soundInstance != null)
				{
					soundInstance.SetPan(thePan);
					soundInstance.Play(false, true);
				}
			}
		}

		public virtual bool IsMuted()
		{
			return mMuteCount > 0;
		}

		public virtual void Mute(bool autoMute)
		{
			mMuteCount++;
			if (autoMute)
			{
				mAutoMuteCount++;
			}
			SetMusicVolume(mMusicVolume);
			SetSfxVolume(mSfxVolume);
		}

		public virtual void Unmute(bool autoMute)
		{
			if (mMuteCount > 0)
			{
				mMuteCount--;
				if (autoMute)
				{
					mAutoMuteCount--;
				}
			}
			SetMusicVolume(mMusicVolume);
			SetSfxVolume(mSfxVolume);
		}

		public virtual double GetMusicVolume()
		{
			if (mMusicInterface.isPlayingUserMusic())
			{
				return 0.0;
			}
			return mMusicVolume;
		}

		public virtual void SetMusicVolume(double theVolume)
		{
			mMusicVolume = theVolume;
			if (mMusicInterface != null)
			{
				mMusicInterface.SetVolume((mMuteCount > 0) ? 0.0 : mMusicVolume);
			}
		}

		public virtual double GetSfxVolume()
		{
			return mSfxVolume;
		}

		public virtual void SetSfxVolume(double theVolume)
		{
			if (mSoundManager != null)
			{
				mSoundManager.SetVolume((mMuteCount > 0) ? 0.0 : theVolume);
			}
			mSfxVolume = theVolume;
		}

		public virtual double GetMasterVolume()
		{
			if (mSoundManager != null)
			{
				return mSoundManager.GetMasterVolume();
			}
			return 0.0;
		}

		public virtual void SetMasterVolume(double theMasterVolume)
		{
			if (mSoundManager != null)
			{
				mSoundManager.SetMasterVolume(theMasterVolume);
			}
		}

		public virtual bool Is3DAccelerated()
		{
			return mAppDriver.Is3DAccelerated();
		}

		public virtual bool Is3DAccelerationSupported()
		{
			return mAppDriver.Is3DAccelerationSupported();
		}

		public virtual bool Is3DAccelerationRecommended()
		{
			return mAppDriver.Is3DAccelerationRecommended();
		}

		public virtual void Set3DAcclerated(bool is3D, bool reinit)
		{
			mAppDriver.Set3DAcclerated(is3D, reinit);
		}

		public virtual void LowMemoryWarning()
		{
		}

		public virtual bool InitFileDriver()
		{
			if (GlobalMembers.gFileDriver == null)
			{
				GlobalMembers.gFileDriver = new XNAFileDriver();
			}
			mFileDriver = GlobalMembers.gFileDriver;
			return true;
		}

		public virtual void ButtonPress(int theId)
		{
		}

		public virtual void ButtonPress(int theId, int theClickCount)
		{
			ButtonPress(theId);
		}

		public virtual void ButtonDepress(int theId)
		{
		}

		public virtual void ButtonDownTick(int theId)
		{
		}

		public virtual void ButtonMouseEnter(int theId)
		{
		}

		public virtual void ButtonMouseLeave(int theId)
		{
		}

		public virtual void ButtonMouseMove(int theId, int theX, int theY)
		{
		}

		public void SetFirstLaunchStatus(bool theStatus)
		{
			mFirstLaunch = theStatus;
		}

		public bool GetFirstLaunchStatus()
		{
			return mFirstLaunch;
		}

		public void SetAppUpdateStatus(bool theStatus)
		{
			mAppUpdated = theStatus;
		}

		public bool GetAppUpdateStatus()
		{
			return mAppUpdated;
		}

		protected virtual PIEffect GetPIEffectByName(string name)
		{
			return mResourceManager.LoadPIEffect(name);
		}

		protected virtual Font GetFontByName(string name)
		{
			return mResourceManager.LoadFont(name);
		}

		protected virtual int GetSoundIDByName(string name)
		{
			return mResourceManager.LoadSound(name);
		}

		protected virtual PopAnim GetPopAnimByName(string name)
		{
			return mResourceManager.LoadPopAnim(name);
		}
	}
}
