using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Resource
{
	public class ResourceXmlReader : ContentTypeReader<ResourceManager>
	{
		protected ResourceManager manager;

		protected override ResourceManager Read(ContentReader input, ResourceManager existingInstance)
		{
			manager = new ResourceManager(null);
			int num = input.ReadInt32();
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				input.ReadString();
				CompositeResGroup compositeResGroup = new CompositeResGroup();
				string key = input.ReadString();
				num2 = input.ReadInt32();
				for (int j = 0; j < num2; j++)
				{
					SubGroup subGroup = new SubGroup();
					subGroup.mGroupName = input.ReadString();
					subGroup.mArtRes = input.ReadInt32();
					subGroup.mLocSet = input.ReadUInt32();
					compositeResGroup.mSubGroups.Add(subGroup);
				}
				manager.mCompositeResGroupMap.Add(key, compositeResGroup);
			}
			int num3 = input.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				ResGroup resGroup = new ResGroup();
				input.ReadString();
				string key2 = input.ReadString();
				int num4 = input.ReadInt32();
				for (int l = 0; l < num4; l++)
				{
					input.ReadString();
					readRes(input, resGroup);
				}
				manager.mResGroupMap.Add(key2, resGroup);
			}
			return manager;
		}

		protected void readRes(ContentReader input, ResGroup group)
		{
			BaseRes baseRes = null;
			ResType resType = (ResType)input.ReadInt32();
			switch (resType)
			{
			case ResType.ResType_Image:
			{
				ImageRes imageRes = new ImageRes();
				baseRes = imageRes;
				readBaseRes(input, baseRes, manager.mResMaps[(int)resType]);
				imageRes.mPalletize = input.ReadBoolean();
				imageRes.mA4R4G4B4 = input.ReadBoolean();
				imageRes.mDDSurface = input.ReadBoolean();
				imageRes.mPurgeBits = input.ReadBoolean();
				imageRes.mA8R8G8B8 = input.ReadBoolean();
				imageRes.mDither16 = input.ReadBoolean();
				imageRes.mMinimizeSubdivisions = input.ReadBoolean();
				imageRes.mAutoFindAlpha = input.ReadBoolean();
				imageRes.mCubeMap = input.ReadBoolean();
				imageRes.mVolumeMap = input.ReadBoolean();
				imageRes.mNoTriRep = input.ReadBoolean();
				imageRes.m2DBig = input.ReadBoolean();
				imageRes.mIsAtlas = input.ReadBoolean();
				imageRes.mAlphaImage = input.ReadString();
				imageRes.mAlphaColor = input.ReadInt32();
				imageRes.mOffset = default(Point);
				imageRes.mOffset.mX = input.ReadInt32();
				imageRes.mOffset.mY = input.ReadInt32();
				imageRes.mVariant = input.ReadString();
				imageRes.mAlphaGridImage = input.ReadString();
				imageRes.mRows = input.ReadInt32();
				imageRes.mCols = input.ReadInt32();
				string text3 = input.ReadString();
				if (text3.Length != 0)
				{
					imageRes.mAtlasName = text3;
				}
				imageRes.mAtlasX = input.ReadInt32();
				imageRes.mAtlasY = input.ReadInt32();
				imageRes.mAtlasW = input.ReadInt32();
				imageRes.mAtlasH = input.ReadInt32();
				imageRes.mAnimInfo.mAnimType = (AnimType)input.ReadInt32();
				imageRes.mAnimInfo.mFrameDelay = input.ReadInt32();
				imageRes.mAnimInfo.mBeginDelay = input.ReadInt32();
				imageRes.mAnimInfo.mEndDelay = input.ReadInt32();
				int num = input.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					imageRes.mAnimInfo.mPerFrameDelay.Add(input.ReadInt32());
				}
				num = input.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					imageRes.mAnimInfo.mFrameMap.Add(input.ReadInt32());
				}
				if (imageRes.mAnimInfo.mAnimType != 0)
				{
					int theNumCels = Math.Max(imageRes.mRows, imageRes.mCols);
					imageRes.mAnimInfo.Compute(theNumCels, imageRes.mAnimInfo.mBeginDelay, imageRes.mAnimInfo.mEndDelay);
				}
				break;
			}
			case ResType.ResType_Sound:
			{
				SoundRes soundRes = new SoundRes();
				baseRes = soundRes;
				readBaseRes(input, baseRes, manager.mResMaps[(int)resType]);
				input.ReadString();
				soundRes.mVolume = input.ReadDouble();
				soundRes.mPanning = input.ReadInt32();
				break;
			}
			case ResType.ResType_Font:
			{
				FontRes fontRes = new FontRes();
				baseRes = fontRes;
				readBaseRes(input, baseRes, manager.mResMaps[(int)resType]);
				string text2 = input.ReadString();
				if (!text2.Equals(""))
				{
					fontRes.mImagePath = text2;
				}
				fontRes.mTags = input.ReadString();
				fontRes.mSize = input.ReadInt32();
				fontRes.mBold = input.ReadBoolean();
				fontRes.mItalic = input.ReadBoolean();
				fontRes.mShadow = input.ReadBoolean();
				fontRes.mUnderline = input.ReadBoolean();
				fontRes.mSysFont = input.ReadBoolean();
				break;
			}
			case ResType.ResType_PopAnim:
				baseRes = new PopAnimRes();
				readBaseRes(input, baseRes, manager.mResMaps[(int)resType]);
				break;
			case ResType.ResType_PIEffect:
				baseRes = new PIEffectRes();
				readBaseRes(input, baseRes, manager.mResMaps[(int)resType]);
				break;
			case ResType.ResType_RenderEffect:
			{
				RenderEffectRes renderEffectRes = new RenderEffectRes();
				baseRes = renderEffectRes;
				readBaseRes(input, baseRes, manager.mResMaps[(int)resType]);
				string text = input.ReadString();
				if (!text.Equals(""))
				{
					renderEffectRes.mSrcFilePath = text;
				}
				break;
			}
			case ResType.ResType_GenericResFile:
				baseRes = new GenericResFileRes();
				readBaseRes(input, baseRes, manager.mResMaps[(int)resType]);
				break;
			}
			baseRes.ApplyConfig();
			group.mResList.Add(baseRes);
		}

		protected void readBaseRes(ContentReader input, BaseRes res, Dictionary<string, BaseRes> theMap)
		{
			res.mId = input.ReadString();
			res.mResGroup = input.ReadString();
			res.mCompositeResGroup = input.ReadString();
			res.mArtRes = input.ReadInt32();
			res.mLocSet = input.ReadUInt32();
			res.mPath = input.ReadString();
			res.mFromProgram = input.ReadBoolean();
			res.mXMLAttributes = new Dictionary<string, string>();
			int num = input.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string key = input.ReadString();
				string value = input.ReadString();
				res.mXMLAttributes.Add(key, value);
			}
			theMap.Add(res.mId, res);
		}
	}
}
