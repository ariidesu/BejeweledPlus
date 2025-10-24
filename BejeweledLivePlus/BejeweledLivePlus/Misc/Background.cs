using System;
using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Sound;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Misc
{
	public class Background : SexyFramework.Widget.Widget, PopAnimListener
	{
		public bool mNoParticles;

		public string mPath = string.Empty;

		public SharedRenderTarget mSharedRenderTarget = new SharedRenderTarget();

		public ResourceRef mResourceImageRef = new ResourceRef();

		public SharedImageRef mImage = new SharedImageRef();

		public BkgPopAnim mAnim;

		public bool mAnimActive;

		public bool mWantAnim;

		public bool mKeepFlatImage;

		public bool mHasRenderTargetFlatImage;

		public bool mRenderTargetFlatImageDirty = true;

		public double mExtraImgScale;

		public double mExtraDrawScale;

		public bool mAllowRealign;

		public bool mAllowRescale;

		public int mStage;

		public List<string> mLoadedImages = new List<string>();

		public List<string> mLoadedSounds = new List<string>();

		public List<int> mDirectLoadedSounds = new List<int>();

		public int mScoreWaitLevel;

		public int mScoreWaitsPassed;

		public float mUpdateAcc;

		public CurvedVal mUpdateSpeed = new CurvedVal();

		public CurvedVal mImageOverlayAlpha = new CurvedVal();

		public CurvedVal mFlash = new CurvedVal();

		public Color mColor = default(Color);

		public string mResourceGroup = string.Empty;

		public virtual void LoadImageProc()
		{
			string text = mPath;
			string idByPath = GlobalMembers.gApp.mResourceManager.GetIdByPath(text);
			string fileDir = Common.GetFileDir(text);
			if (mPath.ToUpper().Contains(".PAM"))
			{
				idByPath = GlobalMembers.gApp.mResourceManager.GetIdByPath(fileDir + "\\flattenedpam");
				mResourceGroup = GlobalMembers.gApp.mResStreamsManager.GetGroupName((uint)GlobalMembers.gApp.mResStreamsManager.GetGroupForFile(fileDir + "\\flattenedpam"));
			}
			BejeweledLivePlusApp.LoadContent(mResourceGroup);
			mResourceImageRef = GlobalMembers.gApp.mResourceManager.GetImageRef(idByPath);
			mImage = mResourceImageRef.GetSharedImageRef();
			// Due to the modified BaseXNARenderDevice, background ended up being wiped.... somehow??
			// We force load it again.
			//if (mImage.GetMemoryImage() == null)
			{
				mImage = GlobalMembers.gApp.mResourceManager.GetImageRef(idByPath).GetSharedImageRef();
				if (mImage.GetMemoryImage() != null)
				{
					mLoadedImages.Add(idByPath);
				}
				else
				{
					mImage = GlobalMembers.gApp.GetSharedImage(text);
				}
			}
			mStage++;
		}

		public static void LoadImageProcStatic(object theThis)
		{
			((Background)theThis).LoadImageProc();
		}

		public void LoadAnimProc()
		{
		}

		public static void LoadAnimProcStatic(object theThis)
		{
			((Background)theThis).LoadAnimProc();
		}

		public Background(string thePath, bool wantFlatImage)
			: this(thePath, wantFlatImage, true)
		{
		}

		public Background(string thePath)
			: this(thePath, true, true)
		{
		}

		public Background(string thePath, bool wantFlatImage, bool wantAnim)
		{
			mResourceGroup = "";
			mPath = thePath;
			mScoreWaitLevel = 0;
			mScoreWaitsPassed = 0;
			mAnim = null;
			mUpdateAcc = 0f;
			mAnimActive = false;
			mWantAnim = false;
			mStage = 0;
			mKeepFlatImage = false;
			mExtraImgScale = 1.0;
			mExtraDrawScale = 1.0;
			mHasRenderTargetFlatImage = false;
			mRenderTargetFlatImageDirty = false;
			mAllowRealign = false;
			mAllowRescale = false;
			mImageOverlayAlpha.SetConstant(1.0);
			mColor = Color.White;
			mNoParticles = false;
			if (wantFlatImage)
			{
				LoadImageProc();
			}
			else
			{
				mStage++;
			}
		}

		public override void Dispose()
		{
			if (mAnim != null)
			{
				RemoveWidget(mAnim);
				if (mAnim != null)
				{
					mAnim.Dispose();
					mAnim = null;
				}
			}
			RemoveAllWidgets(true, false);
			for (int i = 0; i < mLoadedImages.Count; i++)
			{
				GlobalMembers.gApp.mResourceManager.DeleteImage(mLoadedImages[i]);
			}
			for (int j = 0; j < mLoadedSounds.Count; j++)
			{
				GlobalMembers.gApp.mResourceManager.DeleteSound(mLoadedSounds[j]);
			}
			for (int k = 0; k < mDirectLoadedSounds.Count; k++)
			{
				GlobalMembers.gApp.mSoundManager.ReleaseSound(mDirectLoadedSounds[k]);
			}
			mImage.Release();
			mResourceImageRef.Release();
			BejeweledLivePlusApp.UnloadContent(mResourceGroup);
			GlobalMembers.gApp.CleanSharedImages();
			base.Dispose();
		}

		public override void Draw(Graphics g)
		{
			bool flag = false;
			bool flag2 = mColor != Color.White;
			if (flag2)
			{
				g.SetColor(mColor);
				g.PushColorMult();
				g.SetColor(Color.White);
			}
			Graphics3D graphics3D = g.Get3D();
			if (graphics3D != null && mAllowRescale)
			{
				SexyTransform2D theTransform = new SexyTransform2D(true);
				theTransform.Scale(1.12f, 1.12f);
				graphics3D.PushTransform(theTransform);
			}
			if (mAnim != null && mAnim.mLoaded)
			{
				mAnim.Draw(g);
				flag = true;
			}
			if (mImage.GetMemoryImage() != null && (double)mImageOverlayAlpha > 0.0)
			{
				g.SetColorizeImages(true);
				g.SetColor(mImageOverlayAlpha);
				if (mImage.mHeight == mHeight)
				{
					if (mHasRenderTargetFlatImage && mImage.GetImage().GetRenderData() == null)
					{
						GetBackgroundImage();
					}
					if (mAllowRealign)
					{
						g.DrawImage(mImage.GetImage(), (GlobalMembers.S(1920) - mImage.mWidth) / 2, 0);
					}
					else
					{
						g.DrawImage(mImage.GetImage(), 0, 0);
					}
				}
				else
				{
					int num = mHeight / mImage.mHeight;
					g.DrawImage(mImage.GetImage(), 0, 0, mWidth, mHeight);
				}
				flag = true;
			}
			if (!flag)
			{
				g.SetColor(new Color(64, 0, 0));
				g.FillRect(0, 0, mWidth * 2, mHeight);
			}
			if (flag2)
			{
				g.PopColorMult();
			}
			if ((double)mFlash > 0.0)
			{
				g.PushState();
				g.SetDrawMode(Graphics.DrawMode.Additive);
				g.SetColor(mFlash);
				g.FillRect(0, 0, mWidth, mHeight);
				g.PopState();
			}
			if (graphics3D != null && mAllowRescale)
			{
				graphics3D.PopTransform();
			}
		}

		public override void Update()
		{
			base.Update();
			if (SexyFramework.GlobalMembers.gIs3D && GlobalMembers.gApp.mAnimateBackground && mStage == 1 && (mVisible || mParent == null))
			{
				mStage++;
				LoadAnimProc();
			}
			bool flag = false;
			if (mAnim != null && !mAnimActive && mVisible && GlobalMembers.gApp.mLoaded && mWantAnim)
			{
				flag = true;
				mAnim.Play();
				mAnim.Resize(0, 0, mWidth, mHeight);
				if (mImage.GetMemoryImage() != null)
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBACKGROUND_UPDATE_SPEED, mUpdateSpeed);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBACKGROUND_IMAGE_OVERLAY_ALPHA, mImageOverlayAlpha);
				}
				else
				{
					mUpdateSpeed.SetConstant(1.0);
				}
				mAnimActive = true;
			}
			if (!mHasRenderTargetFlatImage && mAnim != null && !mKeepFlatImage && (double)mImageOverlayAlpha == 0.0)
			{
				mImage.Release();
				mResourceImageRef.Release();
			}
			if (!mAnimActive)
			{
				return;
			}
			mRenderTargetFlatImageDirty = true;
			bool mVSyncUpdates = SexyFramework.GlobalMembers.gSexyApp.mVSyncUpdates;
			SexyFramework.GlobalMembers.gSexyApp.mVSyncUpdates = true;
			mAnim.UpdateF((float)(double)mUpdateSpeed);
			mUpdateAcc += (float)(double)mUpdateSpeed;
			if (mUpdateAcc < 1f && flag)
			{
				mUpdateAcc = 1f;
			}
			while (mUpdateAcc >= 1f)
			{
				mAnim.Update();
				mUpdateAcc -= 1f;
			}
			SexyFramework.GlobalMembers.gSexyApp.mVSyncUpdates = mVSyncUpdates;
			if ((double)mUpdateSpeed > 0.0)
			{
				MarkDirty();
			}
			if (!GlobalMembers.gApp.mAnimateBackground)
			{
				mStage = 0;
				mAnimActive = false;
				RemoveWidget(mAnim);
				if (mAnim != null)
				{
					mAnim.Dispose();
					mAnim = null;
				}
				mAnim = null;
				LoadImageProc();
				mImageOverlayAlpha.SetConstant(1.0);
			}
		}

		public virtual SharedImageRef GetBackgroundImage(bool wait)
		{
			return GetBackgroundImage(wait, true);
		}

		public virtual SharedImageRef GetBackgroundImage()
		{
			return GetBackgroundImage(true, true);
		}

		public virtual SharedImageRef GetBackgroundImage(bool wait, bool removeAnim)
		{
			if (mImage.GetMemoryImage() == null || mHasRenderTargetFlatImage)
			{
				if (mHasRenderTargetFlatImage && mImage.GetImage().GetRenderData() == null)
				{
					mRenderTargetFlatImageDirty = true;
				}
				if (mImage.GetMemoryImage() == null || mRenderTargetFlatImageDirty)
				{
					if (mAnim != null)
					{
						if (mImage.mUnsharedImage != null)
						{
							mImage.mUnsharedImage.Dispose();
						}
						mImage.mUnsharedImage = null;
						mImage.mUnsharedImage = new DeviceImage();
						mImage.GetMemoryImage().AddImageFlags(16u);
						mImage.GetMemoryImage().mIsVolatile = true;
						mImage.GetMemoryImage().SetImageMode(true, true);
						mImage.GetMemoryImage().Create(mWidth, mHeight);
						mImage.GetMemoryImage().CreateRenderData();
						mHasRenderTargetFlatImage = true;
						mRenderTargetFlatImageDirty = false;
						Graphics g = new Graphics(mImage.GetImage());
						new Color(0, 0, 0, 255);
						mAnim.Draw(g);
						if (removeAnim)
						{
							if (mAnim != null)
							{
								mAnim.Dispose();
								mAnim = null;
							}
							mAnim = null;
							mAnimActive = false;
							mImageOverlayAlpha.SetConstant(1.0);
						}
					}
					else
					{
						if (mHasRenderTargetFlatImage)
						{
							mHasRenderTargetFlatImage = false;
						}
						LoadImageProc();
					}
				}
			}
			return new SharedImageRef(mImage);
		}

		public void PrepBackgroundImage()
		{
			SharedImageRef backgroundImage = GetBackgroundImage(true, false);
			if (backgroundImage.GetMemoryImage() != null)
			{
				backgroundImage.GetMemoryImage().CreateRenderData();
			}
		}

		public PopAnim GetPopAnim()
		{
			return GetPopAnim(true);
		}

		public PopAnim GetPopAnim(bool wait)
		{
			if (mAnim == null)
			{
				if (!wait)
				{
					return null;
				}
				if (mStage == 1)
				{
					mStage++;
					LoadAnimProc();
				}
			}
			return mAnim;
		}

		public int GetSoundId(string theSampleName)
		{
			string idByPath = GlobalMembers.gApp.mResourceManager.GetIdByPath("sounds\\backgrounds\\" + theSampleName);
			if (idByPath.Length == 0)
			{
				idByPath = GlobalMembers.gApp.mResourceManager.GetIdByPath("sounds\\" + theSampleName);
			}
			int num = GlobalMembers.gApp.mResourceManager.GetSound(idByPath);
			if (num == -1)
			{
				num = GlobalMembers.gApp.mResourceManager.LoadSound(idByPath);
				if (num != -1)
				{
					mLoadedSounds.Add(idByPath);
				}
				else
				{
					num = GlobalMembers.gApp.mSoundManager.LoadSound("sounds\\backgrounds\\" + theSampleName);
					if (num != -1)
					{
						mDirectLoadedSounds.Add(num);
					}
					else
					{
						num = GlobalMembers.gApp.mSoundManager.LoadSound("sounds\\" + theSampleName);
						if (num != -1)
						{
							mDirectLoadedSounds.Add(num);
						}
					}
				}
			}
			return num;
		}

		public void PrecacheResources(PASpriteDef theSpriteDef)
		{
			for (int i = 0; i < theSpriteDef.mFrames.Length; i++)
			{
				PAFrame pAFrame = theSpriteDef.mFrames[i];
				for (int j = 0; j < pAFrame.mCommandVector.Length; j++)
				{
					PACommand pACommand = pAFrame.mCommandVector[j];
					if (pACommand.mCommand.ToUpper() == "PLAYSAMPLE")
					{
						int num = pACommand.mParam.IndexOf(',');
						if (num == -1)
						{
							num = pACommand.mParam.Length;
						}
						string theSampleName = pACommand.mParam.Substring(0, num).Trim();
						GetSoundId(theSampleName);
					}
				}
			}
		}

		public virtual void PopAnimPlaySample(string theSampleName, int thePan, double theVolume, double theNumSteps)
		{
			int num = GetSoundId(theSampleName);
			if (num == -1)
			{
				num = GlobalMembersResourcesWP.SOUND_START_ROTATE;
			}
			SoundInstance soundInstance = GlobalMembers.gApp.mSoundManager.GetSoundInstance(num);
			if (soundInstance != null)
			{
				soundInstance.SetVolume(theVolume);
				soundInstance.SetPan(thePan);
				soundInstance.AdjustPitch(theNumSteps);
				soundInstance.Play(false, true);
			}
		}

		public virtual bool PopAnimCommand(int theId, PASpriteInst thePASpriteInst, string theCommand, string theParam)
		{
			if (mNoParticles && string.Compare(theCommand, "addparticleeffect", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(theCommand, "waitForScore", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (mScoreWaitLevel > mScoreWaitsPassed)
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BACKGROUND_CHANGE);
					mScoreWaitsPassed++;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eBACKGROUND_FLASH, mFlash);
				}
				else
				{
					thePASpriteInst.mFrameNum -= 1f;
				}
			}
			else if (string.Compare(theCommand, "waitForever", StringComparison.OrdinalIgnoreCase) == 0)
			{
				thePASpriteInst.mFrameNum -= 1f;
			}
			return false;
		}

		public PIEffect PopAnimLoadParticleEffect(string theEffectName)
		{
			throw new NotImplementedException();
		}

		public bool PopAnimObjectPredraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor)
		{
			throw new NotImplementedException();
		}

		public bool PopAnimObjectPostdraw(int theId, Graphics g, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Color theColor)
		{
			throw new NotImplementedException();
		}

		public ImagePredrawResult PopAnimImagePredraw(int theId, PASpriteInst theSpriteInst, PAObjectInst theObjectInst, PATransform theTransform, Image theImage, Graphics g, int theDrawCount)
		{
			throw new NotImplementedException();
		}

		public void PopAnimStopped(int theId)
		{
			throw new NotImplementedException();
		}

		public void PopAnimCommand(int theId, string theCommand, string theParam)
		{
			throw new NotImplementedException();
		}
	}
}
