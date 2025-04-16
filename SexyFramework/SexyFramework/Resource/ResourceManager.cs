using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace SexyFramework.Resource
{
	public class ResourceManager
	{
		private class GroupLoadInfo
		{
			public string mName;

			public int mTotalFile;

			public int mCurrentFile;

			public GroupLoadInfo(string name, int totalFiles)
			{
				mName = name;
				mTotalFile = totalFiles;
				mCurrentFile = 0;
			}
		}

		public List<string> mLoadedGroups = new List<string>();

		public Dictionary<string, BaseRes>[] mResMaps = new Dictionary<string, BaseRes>[7];

		public Dictionary<string, BaseRes> mResFromPathMap = new Dictionary<string, BaseRes>();

		public Dictionary<string, ResGroup> mResGroupMap = new Dictionary<string, ResGroup>();

		public Dictionary<string, CompositeResGroup> mCompositeResGroupMap = new Dictionary<string, CompositeResGroup>();

		public List<int> mSupportedLocSets = new List<int>();

		public ResGroup mCurResGroupList;

		public List<BaseRes>.Enumerator mCurResGroupListItr;

		public object mLoadCrit = new object();

		public SexyAppBase mApp;

		public string mLastXMLFileName;

		public string mResGenExePath;

		public string mResPropsUsed;

		public string mResWatchFileUsed;

		public string mResGenTargetName;

		public string mResGenRelSrcRootFromDist;

		public string mError;

		public string mCurCompositeResGroup;

		public string mCurResGroup;

		public string mDefaultPath;

		public string mDefaultIdPrefix;

		public int mResGenMajorVersion;

		public int mResGenMinorVersion;

		public int mCurResGroupArtRes;

		public int mReloadIdx;

		public int mCurCompositeSubGroupIndex;

		public int mBaseArtRes;

		public int mCurArtRes;

		public int mLeadArtRes;

		public uint mCurLocSet;

		public uint mCurResGroupLocSet;

		public bool mHasFailed;

		public bool mAllowMissingProgramResources;

		public bool mAllowAlreadyDefinedResources;

		public bool mHadAlreadyDefinedError;

		public XMLParser mXMLParser;

		private string mCurArtResKey = "";

		private string mCurLocSetKey = "";

		private string mCurArtResAndLocSetKey = "";

		private static Point aEmptyPoint = default(Point);

		public bool mIsLoading;

		private Thread mLoadingThread;

		private ThreadStart mLoadingProc;

		private bool mLoadSuccess;

		private List<GroupLoadInfo> mGroupToLoad = new List<GroupLoadInfo>();

		private bool mLoadFinished;

		public bool Fail(string theErrorText)
		{
			if (!mHasFailed)
			{
				mHasFailed = true;
				if (mXMLParser == null)
				{
					mError = theErrorText;
					return false;
				}
				int currentLineNum = mXMLParser.GetCurrentLineNum();
				mError = theErrorText;
				if (currentLineNum > 0)
				{
					mError = mError + " on Line " + currentLineNum;
				}
				if (mXMLParser.GetFileName().Length > 0)
				{
					mError = mError + " in File '" + mXMLParser.GetFileName() + "'";
				}
			}
			return false;
		}

		protected virtual bool ParseCommonResource(XMLElement theElement, BaseRes theRes, Dictionary<string, BaseRes> theMap)
		{
			mHadAlreadyDefinedError = false;
			theRes.mParent = this;
			theRes.mGlobalPtr = null;
			string attribute = theElement.GetAttribute("path");
			if (attribute.Length <= 0)
			{
				return Fail("No path specified.");
			}
			theRes.mXMLAttributes = theElement.GetAttributeMap();
			theRes.mFromProgram = false;
			if (attribute[0] == '!')
			{
				theRes.mPath = attribute;
				if (attribute == "!program")
				{
					theRes.mFromProgram = true;
				}
			}
			else
			{
				theRes.mPath = mDefaultPath + attribute;
				mResFromPathMap[theRes.mPath.ToUpper()] = theRes;
			}
			string text = "";
			text = ((theElement.GetAttribute("id").Length <= 0) ? (mDefaultIdPrefix + Common.GetFileName(theRes.mPath, true)) : (mDefaultIdPrefix + theElement.GetAttribute("id")));
			if (mCurResGroupArtRes != 0)
			{
				text = text + "|" + mCurResGroupArtRes;
			}
			if (mCurResGroupLocSet != 0)
			{
				text = text + "||" + $"{mCurResGroupLocSet:x}";
			}
			theRes.mResGroup = mCurResGroup;
			theRes.mCompositeResGroup = mCurCompositeResGroup;
			theRes.mId = text;
			theRes.mArtRes = mCurResGroupArtRes;
			theRes.mLocSet = mCurResGroupLocSet;
			if (theMap.ContainsKey(text))
			{
				mHadAlreadyDefinedError = true;
				return Fail("Resource already defined.");
			}
			theMap[theRes.mId] = theRes;
			mCurResGroupList.mResList.Add(theRes);
			return true;
		}

		protected virtual bool ParseSoundResource(XMLElement theElement)
		{
			SoundRes soundRes = new SoundRes();
			soundRes.mSoundId = -1;
			soundRes.mVolume = -1.0;
			soundRes.mPanning = 0;
			if (!ParseCommonResource(theElement, soundRes, mResMaps[1]))
			{
				if (!mHadAlreadyDefinedError || !mAllowAlreadyDefinedResources)
				{
					soundRes = null;
					return false;
				}
				mError = "";
				mHasFailed = false;
				SoundRes soundRes2 = soundRes;
				soundRes = (SoundRes)mResMaps[1][soundRes2.mId];
				soundRes.mPath = soundRes2.mPath;
				soundRes.mXMLAttributes = soundRes2.mXMLAttributes;
				soundRes2 = null;
			}
			if (theElement.HasAttribute("volume"))
			{
				double.TryParse(theElement.GetAttribute("volume"), NumberStyles.Float, CultureInfo.InvariantCulture, out soundRes.mVolume);
			}
			if (theElement.HasAttribute("pan"))
			{
				int.TryParse(theElement.GetAttribute("pan"), out soundRes.mPanning);
			}
			soundRes.ApplyConfig();
			soundRes.mReloadIdx = mReloadIdx;
			return true;
		}

		protected virtual bool ParseImageResource(XMLElement theElement)
		{
			string attribute = theElement.GetAttribute("id");
			if (attribute.Length <= 0)
			{
				return true;
			}
			string attribute2 = theElement.GetAttribute("path");
			if (attribute2.Length <= 0)
			{
				return true;
			}
			ImageRes imageRes = new ImageRes();
			if (!ParseCommonResource(theElement, imageRes, mResMaps[0]))
			{
				if (!mHadAlreadyDefinedError || !mAllowAlreadyDefinedResources)
				{
					imageRes = null;
					return false;
				}
				mError = "";
				mHasFailed = false;
				ImageRes imageRes2 = imageRes;
				imageRes = (ImageRes)mResMaps[0][imageRes2.mId];
				imageRes.mPath = imageRes2.mPath;
				imageRes.mXMLAttributes = imageRes2.mXMLAttributes;
				imageRes2 = null;
			}
			imageRes.mPalletize = !theElement.GetAttributeBool("nopal", false);
			imageRes.mA4R4G4B4 = theElement.GetAttributeBool("a4r4g4b4", false);
			imageRes.mDDSurface = theElement.GetAttributeBool("ddsurface", false);
			bool flag = true;
			imageRes.mPurgeBits = theElement.GetAttributeBool("nobits", false) || (flag && theElement.GetAttributeBool("nobits3d", false)) || (!flag && theElement.GetAttributeBool("nobits2d", false));
			imageRes.mA8R8G8B8 = theElement.GetAttributeBool("a8r8g8b8", false);
			imageRes.mDither16 = theElement.GetAttributeBool("dither16", false);
			imageRes.mMinimizeSubdivisions = theElement.GetAttributeBool("minsubdivide", false);
			imageRes.mAutoFindAlpha = !theElement.GetAttributeBool("noalpha", false);
			imageRes.mCubeMap = theElement.GetAttributeBool("cubemap", false);
			imageRes.mVolumeMap = theElement.GetAttributeBool("volumemap", false);
			imageRes.mNoTriRep = theElement.GetAttributeBool("notrirep", false) || theElement.GetAttributeBool("noquadrep", false);
			imageRes.m2DBig = theElement.GetAttributeBool("2dbig", false);
			imageRes.mIsAtlas = theElement.GetAttributeBool("atlas", false);
			if (theElement.HasAttribute("alphaimage"))
			{
				imageRes.mAlphaImage = mDefaultPath + theElement.GetAttribute("alphaimage");
			}
			imageRes.mAlphaColor = 16777215;
			if (theElement.HasAttribute("alphacolor"))
			{
				imageRes.mAlphaColor = int.Parse(string.Format("x", theElement.GetAttribute("alphacolor")));
			}
			imageRes.mOffset = new Point(0, 0);
			if (theElement.HasAttribute("x"))
			{
				imageRes.mOffset.mX = int.Parse(theElement.GetAttribute("x"));
			}
			if (theElement.HasAttribute("y"))
			{
				imageRes.mOffset.mY = int.Parse(theElement.GetAttribute("y"));
			}
			if (theElement.HasAttribute("variant"))
			{
				imageRes.mVariant = theElement.GetAttribute("variant");
			}
			if (theElement.HasAttribute("alphagrid"))
			{
				imageRes.mAlphaGridImage = mDefaultPath + theElement.GetAttribute("alphagrid");
			}
			if (theElement.HasAttribute("rows"))
			{
				imageRes.mRows = int.Parse(theElement.GetAttribute("rows"));
			}
			if (theElement.HasAttribute("cols"))
			{
				imageRes.mCols = int.Parse(theElement.GetAttribute("cols"));
			}
			if (theElement.HasAttribute("parent"))
			{
				imageRes.mAtlasName = theElement.GetAttribute("parent");
				imageRes.mAtlasX = int.Parse(theElement.GetAttribute("ax"));
				imageRes.mAtlasY = int.Parse(theElement.GetAttribute("ay"));
				imageRes.mAtlasW = int.Parse(theElement.GetAttribute("aw"));
				imageRes.mAtlasH = int.Parse(theElement.GetAttribute("ah"));
			}
			if (imageRes.mCubeMap)
			{
				if (imageRes.mRows * imageRes.mCols != 6)
				{
					Fail("Invalid CubeMap definition; must have 6 cells (check rows & cols values).");
					return false;
				}
			}
			else if (imageRes.mVolumeMap)
			{
				int num = imageRes.mRows * imageRes.mCols;
				if (num == 0 || (num & (num - 1)) != 0)
				{
					Fail("Invalid VolumeMap definition; must have a pow2 cell count (check rows & cols values).");
					return false;
				}
			}
			AnimType animType = AnimType.AnimType_None;
			if (theElement.HasAttribute("anim"))
			{
				string attribute3 = theElement.GetAttribute("anim");
				if (string.Compare(attribute3, "none", StringComparison.OrdinalIgnoreCase) == 0)
				{
					animType = AnimType.AnimType_None;
				}
				else if (string.Compare(attribute3, "once", StringComparison.OrdinalIgnoreCase) == 0)
				{
					animType = AnimType.AnimType_Once;
				}
				else if (string.Compare(attribute3, "loop", StringComparison.OrdinalIgnoreCase) == 0)
				{
					animType = AnimType.AnimType_Loop;
				}
				else
				{
					if (string.Compare(attribute3, "pingpong", StringComparison.OrdinalIgnoreCase) != 0)
					{
						Fail("Invalid animation type.");
						return false;
					}
					animType = AnimType.AnimType_PingPong;
				}
			}
			imageRes.mAnimInfo.mAnimType = animType;
			if (animType != 0)
			{
				int theNumCels = Math.Max(imageRes.mRows, imageRes.mCols);
				int theBeginFrameTime = 0;
				int theEndFrameTime = 0;
				if (theElement.HasAttribute("framedelay"))
				{
					imageRes.mAnimInfo.mFrameDelay = int.Parse(theElement.GetAttribute("framedelay"));
				}
				if (theElement.HasAttribute("begindelay"))
				{
					theBeginFrameTime = (imageRes.mAnimInfo.mBeginDelay = int.Parse(theElement.GetAttribute("begindelay")));
				}
				if (theElement.HasAttribute("enddelay"))
				{
					theEndFrameTime = (imageRes.mAnimInfo.mEndDelay = int.Parse(theElement.GetAttribute("enddelay")));
				}
				if (theElement.HasAttribute("perframedelay"))
				{
					ReadIntVector(theElement.GetAttribute("perframedelay"), imageRes.mAnimInfo.mPerFrameDelay);
				}
				if (theElement.HasAttribute("framemap"))
				{
					ReadIntVector(theElement.GetAttribute("framemap"), imageRes.mAnimInfo.mFrameMap);
				}
				imageRes.mAnimInfo.Compute(theNumCels, theBeginFrameTime, theEndFrameTime);
			}
			imageRes.ApplyConfig();
			imageRes.mReloadIdx = mReloadIdx;
			return true;
		}

		protected virtual bool ParseFontResource(XMLElement theElement)
		{
			FontRes fontRes = new FontRes();
			fontRes.mFont = null;
			fontRes.mImage = null;
			if (!ParseCommonResource(theElement, fontRes, mResMaps[2]))
			{
				if (!mHadAlreadyDefinedError || !mAllowAlreadyDefinedResources)
				{
					fontRes = null;
					return false;
				}
				mError = "";
				mHasFailed = false;
				FontRes fontRes2 = fontRes;
				fontRes = (FontRes)mResMaps[2][fontRes2.mId];
				fontRes.mPath = fontRes2.mPath;
				fontRes.mXMLAttributes = fontRes2.mXMLAttributes;
				fontRes2 = null;
			}
			fontRes.mImagePath = "";
			if (theElement.HasAttribute("image"))
			{
				fontRes.mImagePath = theElement.GetAttribute("image");
			}
			if (theElement.HasAttribute("tags"))
			{
				fontRes.mTags = theElement.GetAttribute("tags");
			}
			if (fontRes.mImagePath.StartsWith("!sys:"))
			{
				fontRes.mSysFont = true;
				string mPath = fontRes.mPath;
				fontRes.mPath = mPath.Substring(5);
				if (!theElement.HasAttribute("size"))
				{
					return Fail("SysFont needs point size");
				}
				fontRes.mSize = int.Parse(theElement.GetAttribute("size"));
				if (fontRes.mSize <= 0)
				{
					return Fail("SysFont needs point size");
				}
				fontRes.mBold = theElement.GetAttributeBool("bold", false);
				fontRes.mItalic = theElement.GetAttributeBool("italic", false);
				fontRes.mShadow = theElement.GetAttributeBool("shadow", false);
				fontRes.mUnderline = theElement.GetAttributeBool("underline", false);
			}
			else
			{
				fontRes.mSysFont = false;
			}
			fontRes.ApplyConfig();
			fontRes.mReloadIdx = mReloadIdx;
			return true;
		}

		protected virtual bool ParsePopAnimResource(XMLElement theElement)
		{
			PopAnimRes popAnimRes = new PopAnimRes();
			if (!ParseCommonResource(theElement, popAnimRes, mResMaps[3]))
			{
				if (!mHadAlreadyDefinedError || !mAllowAlreadyDefinedResources)
				{
					popAnimRes = null;
					return false;
				}
				mError = "";
				mHasFailed = false;
				PopAnimRes popAnimRes2 = popAnimRes;
				popAnimRes = (PopAnimRes)mResMaps[3][popAnimRes2.mId];
				popAnimRes.mPath = popAnimRes2.mPath;
				popAnimRes.mXMLAttributes = popAnimRes2.mXMLAttributes;
				popAnimRes2 = null;
			}
			popAnimRes.ApplyConfig();
			popAnimRes.mReloadIdx = mReloadIdx;
			return true;
		}

		protected virtual bool ParsePIEffectResource(XMLElement theElement)
		{
			PIEffectRes pIEffectRes = new PIEffectRes();
			if (!ParseCommonResource(theElement, pIEffectRes, mResMaps[4]))
			{
				if (!mHadAlreadyDefinedError || !mAllowAlreadyDefinedResources)
				{
					pIEffectRes = null;
					return false;
				}
				mError = "";
				mHasFailed = false;
				PIEffectRes pIEffectRes2 = pIEffectRes;
				pIEffectRes = (PIEffectRes)mResMaps[4][pIEffectRes2.mId];
				pIEffectRes.mPath = pIEffectRes2.mPath;
				pIEffectRes.mXMLAttributes = pIEffectRes2.mXMLAttributes;
				pIEffectRes2 = null;
			}
			pIEffectRes.ApplyConfig();
			pIEffectRes.mReloadIdx = mReloadIdx;
			return true;
		}

		protected virtual bool ParseRenderEffectResource(XMLElement theElement)
		{
			RenderEffectRes renderEffectRes = new RenderEffectRes();
			if (!ParseCommonResource(theElement, renderEffectRes, mResMaps[5]))
			{
				if (!mHadAlreadyDefinedError || !mAllowAlreadyDefinedResources)
				{
					renderEffectRes = null;
					return false;
				}
				mError = "";
				mHasFailed = false;
				RenderEffectRes renderEffectRes2 = renderEffectRes;
				renderEffectRes = (RenderEffectRes)mResMaps[5][renderEffectRes2.mId];
				renderEffectRes.mPath = renderEffectRes2.mPath;
				renderEffectRes.mXMLAttributes = renderEffectRes2.mXMLAttributes;
				renderEffectRes2 = null;
			}
			renderEffectRes.mSrcFilePath = "";
			if (theElement.HasAttribute("srcpath") && theElement.GetAttribute("srcpath").Length > 0)
			{
				renderEffectRes.mSrcFilePath = mDefaultPath + theElement.GetAttribute("srcpath");
			}
			renderEffectRes.ApplyConfig();
			renderEffectRes.mReloadIdx = mReloadIdx;
			return true;
		}

		protected virtual bool ParseGenericResFileResource(XMLElement theElement)
		{
			GenericResFileRes genericResFileRes = new GenericResFileRes();
			if (!ParseCommonResource(theElement, genericResFileRes, mResMaps[6]))
			{
				if (!mHadAlreadyDefinedError || !mAllowAlreadyDefinedResources)
				{
					genericResFileRes = null;
					return false;
				}
				mError = "";
				mHasFailed = false;
				GenericResFileRes genericResFileRes2 = genericResFileRes;
				genericResFileRes = (GenericResFileRes)mResMaps[6][genericResFileRes2.mId];
				genericResFileRes.mPath = genericResFileRes2.mPath;
				genericResFileRes.mXMLAttributes = genericResFileRes2.mXMLAttributes;
				genericResFileRes2 = null;
			}
			genericResFileRes.ApplyConfig();
			genericResFileRes.mReloadIdx = mReloadIdx;
			return true;
		}

		protected virtual bool ParseSetDefaults(XMLElement theElement)
		{
			if (theElement.HasAttribute("path"))
			{
				mDefaultPath = Common.RemoveTrailingSlash(theElement.GetAttribute("path")) + "/";
			}
			if (theElement.HasAttribute("idprefix"))
			{
				mDefaultIdPrefix = Common.RemoveTrailingSlash(theElement.GetAttribute("idprefix"));
			}
			return true;
		}

		public virtual bool ParseResources()
		{
			while (true)
			{
				XMLElement xMLElement = new XMLElement();
				if (!mXMLParser.NextElement(xMLElement))
				{
					return false;
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
				{
					if (xMLElement.mValue.ToString() == "Image")
					{
						if (!ParseImageResource(xMLElement))
						{
							return false;
						}
						if (!mXMLParser.NextElement(xMLElement))
						{
							return false;
						}
						if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
						{
							return Fail("Unexpected element found.");
						}
						continue;
					}
					if (xMLElement.mValue.ToString() == "Sound")
					{
						if (!ParseSoundResource(xMLElement))
						{
							return false;
						}
						if (!mXMLParser.NextElement(xMLElement))
						{
							return false;
						}
						if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
						{
							return Fail("Unexpected element found.");
						}
						continue;
					}
					if (xMLElement.mValue.ToString() == "Font")
					{
						if (!ParseFontResource(xMLElement))
						{
							return false;
						}
						if (!mXMLParser.NextElement(xMLElement))
						{
							return false;
						}
						if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
						{
							return Fail("Unexpected element found.");
						}
						continue;
					}
					if (xMLElement.mValue.ToString() == "PopAnim")
					{
						if (!ParsePopAnimResource(xMLElement))
						{
							return false;
						}
						if (!mXMLParser.NextElement(xMLElement))
						{
							return false;
						}
						if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
						{
							return Fail("Unexpected element found.");
						}
						continue;
					}
					if (xMLElement.mValue.ToString() == "PIEffect")
					{
						if (!ParsePIEffectResource(xMLElement))
						{
							return false;
						}
						if (!mXMLParser.NextElement(xMLElement))
						{
							return false;
						}
						if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
						{
							return Fail("Unexpected element found.");
						}
						continue;
					}
					if (xMLElement.mValue.ToString() == "RenderEffect")
					{
						if (!ParseRenderEffectResource(xMLElement))
						{
							return false;
						}
						if (!mXMLParser.NextElement(xMLElement))
						{
							return false;
						}
						if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
						{
							return Fail("Unexpected element found.");
						}
						continue;
					}
					if (xMLElement.mValue.ToString() == "GenericResFile")
					{
						if (!ParseGenericResFileResource(xMLElement))
						{
							return false;
						}
						if (!mXMLParser.NextElement(xMLElement))
						{
							return false;
						}
						if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
						{
							return Fail("Unexpected element found.");
						}
						continue;
					}
					if (!(xMLElement.mValue.ToString() == "SetDefaults"))
					{
						Fail(string.Concat("Invalid Section '", xMLElement.mValue, "'"));
						return false;
					}
					if (!ParseSetDefaults(xMLElement))
					{
						return false;
					}
					if (!mXMLParser.NextElement(xMLElement))
					{
						return false;
					}
					if (xMLElement.mType != XMLElement.XMLElementType.TYPE_END)
					{
						return Fail("Unexpected element found.");
					}
				}
				else
				{
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
					{
						Fail(string.Concat("Element Not Expected '", xMLElement.mValue, "'"));
						return false;
					}
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_END)
					{
						break;
					}
				}
			}
			return true;
		}

		public bool DoParseResources()
		{
			if (!mXMLParser.HasFailed())
			{
				while (true)
				{
					XMLElement xMLElement = new XMLElement();
					if (!mXMLParser.NextElement(xMLElement))
					{
						break;
					}
					if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
					{
						if (xMLElement.mValue.ToString() == "Resources")
						{
							mCurResGroup = xMLElement.GetAttribute("id");
							if (mCurResGroup.Length <= 0)
							{
								Fail("No id specified.");
								break;
							}
							if (mResGroupMap.ContainsKey(mCurResGroup))
							{
								mCurResGroupList = mResGroupMap[mCurResGroup];
							}
							else
							{
								mCurResGroupList = new ResGroup();
								mResGroupMap[mCurResGroup] = mCurResGroupList;
							}
							mCurCompositeResGroup = xMLElement.GetAttribute("parent");
							string attribute = xMLElement.GetAttribute("res");
							mCurResGroupArtRes = ((attribute.Length > 0) ? int.Parse(attribute) : 0);
							string attribute2 = xMLElement.GetAttribute("loc");
							mCurResGroupLocSet = ((attribute2.Length >= 4) ? (((uint)attribute2[0] << 24) | ((uint)attribute2[1] << 16) | ((uint)attribute2[2] << 8) | attribute2[3]) : 0u);
							if (!ParseResources())
							{
								break;
							}
							continue;
						}
						if (!(xMLElement.mValue.ToString() == "CompositeResources"))
						{
							Fail(string.Concat("Invalid Section '", xMLElement.mValue, "'"));
							break;
						}
						string attribute3 = xMLElement.GetAttribute("id");
						if (attribute3.Length <= 0)
						{
							Fail("No id specified on CompositeGroup.");
							break;
						}
						CompositeResGroup compositeResGroup = null;
						if (mCompositeResGroupMap.ContainsKey(attribute3))
						{
							compositeResGroup = mCompositeResGroupMap[attribute3];
						}
						else
						{
							compositeResGroup = new CompositeResGroup();
							mCompositeResGroupMap[attribute3] = compositeResGroup;
						}
						while (true)
						{
							XMLElement xMLElement2 = new XMLElement();
							if (!mXMLParser.NextElement(xMLElement2))
							{
								return false;
							}
							if (xMLElement2.mType == XMLElement.XMLElementType.TYPE_START)
							{
								if (!(xMLElement2.mValue.ToString() == "Group"))
								{
									Fail(string.Concat("Invalid Section '", xMLElement2.mValue, "' within CompositeGroup"));
									break;
								}
								string attribute4 = xMLElement2.GetAttribute("id");
								int mArtRes = 0;
								string attribute5 = xMLElement2.GetAttribute("res");
								if (attribute5.Length > 0)
								{
									mArtRes = int.Parse(attribute5);
								}
								uint mLocSet = 0u;
								string attribute6 = xMLElement2.GetAttribute("loc");
								if (attribute6.Length >= 4)
								{
									mLocSet = ((uint)attribute6[0] << 24) | ((uint)attribute6[1] << 16) | ((uint)attribute6[2] << 8) | attribute6[3];
								}
								SubGroup subGroup = new SubGroup();
								subGroup.mGroupName = attribute4;
								subGroup.mArtRes = mArtRes;
								subGroup.mLocSet = mLocSet;
								compositeResGroup.mSubGroups.Add(subGroup);
								if (!mXMLParser.NextElement(xMLElement2))
								{
									Fail("Group end expected");
									break;
								}
								if (xMLElement2.mType != XMLElement.XMLElementType.TYPE_END)
								{
									Fail("Unexpected element found. (mType " + xMLElement2.mType + ' ' + xMLElement2.mValue.ToString() + ")");
									break;
								}
							}
							else
							{
								if (xMLElement2.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
								{
									Fail(string.Concat("Element Not Expected '", xMLElement2.mValue, "'"));
									return false;
								}
								if (xMLElement2.mType == XMLElement.XMLElementType.TYPE_END)
								{
									break;
								}
							}
						}
						if (mHasFailed)
						{
							break;
						}
					}
					else if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
					{
						Fail(string.Concat("Element Not Expected '", xMLElement.mValue, "'"));
						break;
					}
				}
			}
			if (mXMLParser.HasFailed())
			{
				Fail(mXMLParser.GetErrorText());
			}
			mXMLParser.Dispose();
			mXMLParser = null;
			return !mHasFailed;
		}

		public void DeleteMap(Dictionary<string, BaseRes> theMap)
		{
			Dictionary<string, BaseRes>.Enumerator enumerator = theMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.DeleteResource();
			}
			theMap.Clear();
		}

		public virtual void DeleteResources(Dictionary<string, BaseRes> theMap, string theGroup)
		{
			if (theGroup.Length <= 0)
			{
				Dictionary<string, BaseRes>.Enumerator enumerator = theMap.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Value.mDirectLoaded)
					{
						enumerator.Current.Value.mDirectLoaded = false;
						Deref(enumerator.Current.Value);
					}
				}
				return;
			}
			if (mCompositeResGroupMap.ContainsKey(theGroup))
			{
				CompositeResGroup compositeResGroup = mCompositeResGroupMap[theGroup];
				int count = compositeResGroup.mSubGroups.Count;
				{
					foreach (SubGroup mSubGroup in compositeResGroup.mSubGroups)
					{
						if (mSubGroup.mGroupName.Length <= 0)
						{
							continue;
						}
						Dictionary<string, BaseRes>.Enumerator enumerator3 = theMap.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.Value.mDirectLoaded && enumerator3.Current.Value.mResGroup == mSubGroup.mGroupName)
							{
								enumerator3.Current.Value.mDirectLoaded = false;
								Deref(enumerator3.Current.Value);
							}
						}
					}
					return;
				}
			}
			Dictionary<string, BaseRes>.Enumerator enumerator4 = theMap.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				if (enumerator4.Current.Value.mDirectLoaded && enumerator4.Current.Value.mResGroup == theGroup)
				{
					enumerator4.Current.Value.mDirectLoaded = false;
					Deref(enumerator4.Current.Value);
				}
			}
		}

		public BaseRes GetBaseRes(int theType, string theId)
		{
			if (mCurArtResKey.Length <= 0)
			{
				mCurArtResKey = "|" + mCurArtRes;
				mCurLocSetKey = "||" + $"{mCurLocSet:x}";
				mCurArtResAndLocSetKey = mCurArtRes + "|||" + $"{mCurLocSet:x}";
			}
			if (mResMaps[theType].ContainsKey(theId + mCurArtResAndLocSetKey))
			{
				return mResMaps[theType][theId + mCurArtResAndLocSetKey];
			}
			if (mResMaps[theType].ContainsKey(theId + mCurArtResKey))
			{
				return mResMaps[theType][theId + mCurArtResKey];
			}
			if (mResMaps[theType].ContainsKey(theId + mCurLocSetKey))
			{
				return mResMaps[theType][theId + mCurLocSetKey];
			}
			if (mResMaps[theType].ContainsKey(theId))
			{
				return mResMaps[theType][theId];
			}
			return null;
		}

		public void Deref(BaseRes theRes)
		{
			theRes.mRefCount--;
			if (theRes.mRefCount == 0)
			{
				theRes.DeleteResource();
			}
		}

		public bool LoadAlphaGridImage(ImageRes theRes, DeviceImage theImage)
		{
			throw new NotImplementedException();
		}

		public bool LoadAlphaImage(ImageRes theRes, DeviceImage theImage)
		{
			throw new NotImplementedException();
		}

		public virtual bool DoLoadImage(ImageRes theRes)
		{
			lock (mLoadCrit)
			{
				string mPath = theRes.mPath;
				if (mPath.StartsWith("!ref:"))
				{
					string text = mPath.Substring(5);
					theRes.mResourceRef = GetImageRef(text);
					SharedImageRef sharedImageRef = theRes.mResourceRef.GetSharedImageRef();
					if (sharedImageRef.GetImage() == null)
					{
						sharedImageRef = LoadImage(text);
					}
					if (sharedImageRef.GetImage() == null)
					{
						return Fail("Ref Image not found: " + text);
					}
					theRes.mImage = sharedImageRef;
					theRes.mGlobalPtr = RegisterGlobalPtr(text);
					return true;
				}
				bool flag = theRes.mAtlasName != null;
				bool flag2 = false;
				SharedImageRef sharedImageRef2 = mApp.CheckSharedImage(mPath, theRes.mVariant);
				if (sharedImageRef2.GetDeviceImage() != null)
				{
					flag2 = true;
				}
				else if (!flag)
				{
					DeviceImage deviceImage = DeviceImage.ReadFromCache(Common.GetFullPath(mPath), "ResMan");
					if (deviceImage != null)
					{
						sharedImageRef2 = (theRes.mImage = mApp.SetSharedImage(mPath, theRes.mVariant, deviceImage));
						flag2 = true;
					}
				}
				bool isNew = false;
				bool mWriteToSexyCache = mApp.mWriteToSexyCache;
				mApp.mWriteToSexyCache = false;
				if (!flag2)
				{
					sharedImageRef2 = mApp.GetSharedImage(mPath, theRes.mVariant, ref isNew, !theRes.mNoTriRep, flag);
				}
				mApp.mWriteToSexyCache = mWriteToSexyCache;
				DeviceImage deviceImage2 = sharedImageRef2.GetDeviceImage();
				if (deviceImage2 == null)
				{
					return Fail("Failed to load image: " + mPath);
				}
				if (isNew)
				{
					if (flag)
					{
						deviceImage2.mWidth = theRes.mAtlasW;
						deviceImage2.mHeight = theRes.mAtlasH;
					}
					if (theRes.mAlphaImage.Length > 0 && !LoadAlphaImage(theRes, deviceImage2))
					{
						return false;
					}
					if (theRes.mAlphaGridImage.Length > 0 && !LoadAlphaGridImage(theRes, deviceImage2))
					{
						return false;
					}
				}
				if (theRes.mPalletize && !flag2)
				{
					if (deviceImage2.mSurface == null)
					{
						deviceImage2.Palletize();
					}
					else
					{
						deviceImage2.mWantPal = true;
					}
				}
				theRes.mImage = sharedImageRef2;
				theRes.ApplyConfig();
				theRes.mImage.GetImage().mNameForRes = theRes.mId;
				if (theRes.mGlobalPtr != null)
				{
					theRes.mGlobalPtr.mResObject = deviceImage2;
				}
				if (!flag2 && !flag)
				{
					deviceImage2.WriteToCache(Common.GetFullPath(mPath), "ResMan");
				}
				ResourceLoadedHook(theRes);
			}
			return true;
		}

		public virtual bool DoLoadFont(FontRes theRes)
		{
			lock (mLoadCrit)
			{
				Font font = null;
				string text = theRes.mPath;
				string key = $"path{mCurArtRes}";
				if (theRes.mXMLAttributes.ContainsKey(key))
				{
					text = theRes.mXMLAttributes[key];
				}
				if (!theRes.mSysFont)
				{
					if (string.IsNullOrEmpty(theRes.mImagePath))
					{
						if (string.Compare(text, 0, "!ref:", 0, 5) == 0)
						{
							string text2 = text.Substring(5);
							theRes.mResourceRef = GetFontRef(text2);
							Font font2 = theRes.mResourceRef.GetFont();
							if (font2 == null)
							{
								return Fail("Ref Font not found: " + text2);
							}
							font = (theRes.mFont = font2.Duplicate());
						}
						else
						{
							ImageFont imageFont = ImageFont.ReadFromCache(Common.GetFullPath(text), "ResMan");
							if (imageFont != null)
							{
								font = imageFont;
							}
							else
							{
								imageFont = new ImageFont(mApp, text, "");
								font = imageFont;
							}
						}
					}
					else
					{
						Image image = mApp.GetImage(theRes.mImagePath, false, false, false);
						if (image == null)
						{
							return Fail($"Failed to load image: {theRes.mImagePath}");
						}
						theRes.mImage = image;
					}
				}
				ImageFont imageFont2 = font.AsImageFont();
				if (imageFont2 != null)
				{
					if (imageFont2.mFontData == null || !imageFont2.mFontData.mInitialized)
					{
						font?.Dispose();
						return Fail($"Failed to load font: {text}");
					}
					imageFont2.mTagVector.Clear();
					imageFont2.mActiveListValid = false;
					if (!string.IsNullOrEmpty(theRes.mTags))
					{
						string[] array = theRes.mTags.Split(", \r\n\t".ToCharArray());
						for (int i = 0; i < array.Length; i++)
						{
							imageFont2.AddTag(array[i]);
						}
						imageFont2.Prepare();
					}
				}
				theRes.mFont = imageFont2;
				if (theRes.mGlobalPtr != null)
				{
					theRes.mGlobalPtr.mResObject = font;
				}
				theRes.ApplyConfig();
				ResourceLoadedHook(theRes);
			}
			return true;
		}

		public virtual bool DoLoadSound(SoundRes theRes)
		{
			lock (mLoadCrit)
			{
				string mPath = theRes.mPath;
				if (theRes.mPath.StartsWith("!ref:"))
				{
					string text = mPath.Substring(5);
					theRes.mResourceRef = GetSoundRef(text);
					int soundID = theRes.mResourceRef.GetSoundID();
					if (soundID == -1)
					{
						return Fail("Ref sound not found: " + text);
					}
					theRes.mSoundId = soundID;
					return true;
				}
				int num = -1;
				num = mApp.mSoundManager.GetFreeSoundId();
				if (num < 0)
				{
					return Fail("Out of free sound ids");
				}
				if (!mApp.mSoundManager.LoadSound((uint)num, theRes.mPath))
				{
					return Fail("Failed to load sound: " + theRes.mPath);
				}
				theRes.mSoundId = num;
				if (theRes.mGlobalPtr != null)
				{
					theRes.mGlobalPtr.mResObject = num;
				}
				theRes.ApplyConfig();
				ResourceLoadedHook(theRes);
			}
			return true;
		}

		public virtual bool DoLoadPopAnim(PopAnimRes theRes)
		{
			lock (mLoadCrit)
			{
				PopAnim popAnim = new PopAnim(0, null);
				popAnim.mImgScale = 1f;
				popAnim.mDrawScale = 1f;
				if (string.Compare(theRes.mPath, "images\\768\\anims\\drill\\drill.pam") == 0)
				{
					popAnim.mInterpolate = false;
				}
				if (string.Compare(theRes.mPath, "images\\960\\anims\\spider\\spider.pam") == 0)
				{
					popAnim.mImgScale = 0.8f;
					popAnim.mDrawScale = 0.8f;
				}
				if (string.Compare(theRes.mPath, "images\\768\\anims\\spider\\spider.pam") == 0)
				{
					popAnim.mImgScale = 0.65f;
					popAnim.mDrawScale = 0.65f;
				}
				if (string.Compare(theRes.mPath, "images\\960\\FlameGemCreation\\FlameGemCreation.pam") == 0 || string.Compare(theRes.mPath, "images\\960\\FlameGemExplode\\FlameGemExplode.pam") == 0)
				{
					popAnim.mImgScale = 0.625f;
					popAnim.mDrawScale = 0.625f;
				}
				if (string.Compare(theRes.mPath, "images\\960\\anims\\column1\\column1.pam") == 0 || string.Compare(theRes.mPath, "images\\960\\anims\\column2\\column2.pam") == 0)
				{
					popAnim.mImgScale = 0.65f;
					popAnim.mDrawScale = 0.65f;
				}
				if (string.Compare(theRes.mPath, "images\\768\\FlameGemCreation\\FlameGemCreation.pam") == 0 || string.Compare(theRes.mPath, "images\\768\\FlameGemExplode\\FlameGemExplode.pam") == 0)
				{
					popAnim.mImgScale = (float)mCurArtRes / (float)mBaseArtRes;
					popAnim.mDrawScale = (float)mCurArtRes / (float)mBaseArtRes;
				}
				string text = "images\\960\\help";
				string text2 = theRes.mPath.Substring(0, text.Length);
				if (text2 == text)
				{
					popAnim.mImgScale = 0.625f;
					popAnim.mDrawScale = 0.625f;
				}
				text = "images\\768\\help";
				text2 = theRes.mPath.Substring(0, text.Length);
				bool flag = false;
				if (text2 == text)
				{
					popAnim.mImgScale = 0.35f;
					popAnim.mDrawScale = 0.35f;
					flag = true;
				}
				if (mCurArtRes == 480)
				{
					popAnim.mImgScale *= 0.5f;
					popAnim.mDrawScale *= 0.5f;
				}
				popAnim.LoadFile(theRes.mPath);
				if (popAnim.mError.Length > 0)
				{
					Fail("PopAnim loading error: " + popAnim.mError + " on file " + theRes.mPath);
					popAnim.Dispose();
					return false;
				}
				if (flag)
				{
					FRect fRect = new FRect(popAnim.mAnimRect.mX, popAnim.mAnimRect.mY, popAnim.mAnimRect.mWidth, popAnim.mAnimRect.mHeight);
					fRect.Scale(0.3499999940395355, 0.3499999940395355);
					popAnim.mAnimRect.mX = (int)fRect.mX;
					popAnim.mAnimRect.mY = (int)fRect.mY;
					popAnim.mAnimRect.mWidth = (int)fRect.mWidth;
					popAnim.mAnimRect.mHeight = (int)fRect.mHeight;
				}
				if (theRes.mGlobalPtr != null)
				{
					theRes.mGlobalPtr.mResObject = popAnim;
				}
				theRes.mPopAnim = popAnim;
				return true;
			}
		}

		public virtual bool DoLoadPIEffect(PIEffectRes theRes)
		{
			lock (mLoadCrit)
			{
				PIEffect pIEffect = new PIEffect();
				pIEffect.LoadEffect(theRes.mPath);
				if (pIEffect.mError.Length > 0)
				{
					Fail("PIEffect loading error: " + pIEffect.mError + " on file " + theRes.mPath);
					pIEffect = null;
					return false;
				}
				if (theRes.mGlobalPtr != null)
				{
					theRes.mGlobalPtr.mResObject = pIEffect;
				}
				theRes.mPIEffect = pIEffect;
				return true;
			}
		}

		public virtual bool DoLoadRenderEffect(RenderEffectRes theRes)
		{
			lock (mLoadCrit)
			{
				RenderEffectDefinition renderEffectDefinition = new RenderEffectDefinition();
				renderEffectDefinition.LoadFromFile(theRes.mPath, theRes.mSrcFilePath);
				if (theRes.mGlobalPtr != null)
				{
					theRes.mGlobalPtr.mResObject = renderEffectDefinition;
				}
				theRes.mRenderEffectDefinition = renderEffectDefinition;
				return true;
			}
		}

		public virtual bool DoLoadGenericResFile(GenericResFileRes theRes)
		{
			return true;
		}

		public int GetNumResources(string theGroup, Dictionary<string, BaseRes> theMap, bool curArtResOnly, bool curLocSetOnly)
		{
			int num = 0;
			if (theGroup.Length <= 0)
			{
				if (!curArtResOnly && !curLocSetOnly)
				{
					return theMap.Count;
				}
				Dictionary<string, BaseRes>.Enumerator enumerator = theMap.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BaseRes value = enumerator.Current.Value;
					if ((!curArtResOnly || value.mArtRes == 0 || value.mArtRes == mCurArtRes) && (!curLocSetOnly || value.mLocSet == 0 || value.mLocSet == mCurLocSet) && !value.mFromProgram)
					{
						num++;
					}
				}
			}
			else
			{
				Dictionary<string, BaseRes>.Enumerator enumerator2 = theMap.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					BaseRes value2 = enumerator2.Current.Value;
					if ((!curArtResOnly || value2.mArtRes == 0 || value2.mArtRes == mCurArtRes) && (!curLocSetOnly || value2.mLocSet == 0 || value2.mLocSet == mCurLocSet) && (value2.mResGroup == theGroup || value2.mCompositeResGroup == theGroup) && !value2.mFromProgram)
					{
						num++;
					}
				}
			}
			return num;
		}

		public ResourceManager(SexyAppBase theApp)
		{
			mApp = theApp;
			for (int i = 0; i < 7; i++)
			{
				mResMaps[i] = new Dictionary<string, BaseRes>();
			}
			mBaseArtRes = 0;
			mLeadArtRes = 0;
			mCurArtRes = 0;
			mCurLocSet = 1162761555u;
			mHasFailed = false;
			mXMLParser = null;
			mResGenMajorVersion = 0;
			mResGenMinorVersion = 0;
			mAllowMissingProgramResources = false;
			mAllowAlreadyDefinedResources = false;
			mCurResGroupList = null;
			mReloadIdx = 0;
		}

		public virtual void Dispose()
		{
			for (int i = 0; i < 7; i++)
			{
				DeleteMap(mResMaps[i]);
			}
		}

		public bool ParseResourcesFileBinary(byte[] data)
		{
			mXMLParser = new XMLParser();
			mXMLParser.checkEncodingType(data);
			mXMLParser.SetBytes(data);
			XMLElement xMLElement = new XMLElement();
			while (!mXMLParser.HasFailed())
			{
				if (!mXMLParser.NextElement(xMLElement))
				{
					Fail(mXMLParser.GetErrorText());
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
				{
					if (xMLElement.mValue.ToString() != "ResourceManifest")
					{
						break;
					}
					if (xMLElement.GetAttribute("version").Length > 0)
					{
						int.Parse(xMLElement.GetAttribute("version"));
					}
					return DoParseResources();
				}
			}
			Fail("Expecting ResourceManifest tag");
			return DoParseResources();
		}

		public bool ParseResourcesFile(string theFilename)
		{
			mLastXMLFileName = theFilename;
			if (mApp.mResStreamsManager != null && mApp.mResStreamsManager.IsInitialized())
			{
				return mApp.mResStreamsManager.LoadResourcesManifest(this);
			}
			mXMLParser = new XMLParser();
			if (!mXMLParser.OpenFile(theFilename))
			{
				Fail("Resource file not found: " + theFilename);
			}
			XMLElement xMLElement = new XMLElement();
			while (!mXMLParser.HasFailed())
			{
				if (!mXMLParser.NextElement(xMLElement))
				{
					Fail(mXMLParser.GetErrorText());
				}
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_START)
				{
					if (xMLElement.mValue.ToString() != "ResourceManifest")
					{
						break;
					}
					if (xMLElement.GetAttribute("version").Length > 0)
					{
						int.Parse(xMLElement.GetAttribute("version"));
					}
					return DoParseResources();
				}
			}
			Fail("Expecting ResourceManifest tag");
			return DoParseResources();
		}

		public bool ReparseResourcesFile(string theFilename)
		{
			bool flag = mAllowAlreadyDefinedResources;
			mAllowAlreadyDefinedResources = true;
			mReloadIdx++;
			bool result = ParseResourcesFile(theFilename);
			for (int i = 0; i < 7; i++)
			{
				Dictionary<string, BaseRes>.Enumerator enumerator = mResMaps[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					BaseRes value = enumerator.Current.Value;
					if (value.mReloadIdx != mReloadIdx)
					{
						value.DeleteResource();
					}
				}
			}
			mAllowAlreadyDefinedResources = flag;
			return result;
		}

		public ResGlobalPtr RegisterGlobalPtr(string theId)
		{
			for (int i = 0; i < 7; i++)
			{
				BaseRes baseRes = GetBaseRes(i, theId);
				if (baseRes != null)
				{
					return baseRes.mGlobalPtr;
				}
			}
			return null;
		}

		public void ReapplyConfigs()
		{
			for (int i = 0; i < 7; i++)
			{
				Dictionary<string, BaseRes>.Enumerator enumerator = mResMaps[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value.ApplyConfig();
				}
			}
		}

		public string GetErrorText()
		{
			return mError;
		}

		public bool HadError()
		{
			return mHasFailed;
		}

		public bool IsGroupLoaded(string theGroup)
		{
			return mLoadedGroups.Contains(theGroup);
		}

		public bool IsResourceLoaded(string theId)
		{
			if (GetImage(theId) != null && GetImage(theId).GetDeviceImage() != null)
			{
				return true;
			}
			if (GetFont(theId) != null)
			{
				return true;
			}
			if (GetSound(theId) != -1)
			{
				return true;
			}
			return false;
		}

		public int GetNumImages(string theGroup, bool curArtResOnly, bool curLocSetOnly)
		{
			return GetNumResources(theGroup, mResMaps[0], curArtResOnly, curLocSetOnly);
		}

		public int GetNumSounds(string theGroup, bool curArtResOnly, bool curLocSetOnly)
		{
			return GetNumResources(theGroup, mResMaps[1], curArtResOnly, curLocSetOnly);
		}

		public int GetNumFonts(string theGroup, bool curArtResOnly, bool curLocSetOnly)
		{
			return GetNumResources(theGroup, mResMaps[2], curArtResOnly, curLocSetOnly);
		}

		public int GetNumResources(string theGroup, bool curArtResOnly, bool curLocSetOnly)
		{
			int num = 0;
			for (int i = 0; i < 7; i++)
			{
				num += GetNumResources(theGroup, mResMaps[i], curArtResOnly, curLocSetOnly);
			}
			return num;
		}

		public virtual bool DoLoadResource(BaseRes theRes, out bool skipped)
		{
			skipped = false;
			if (theRes.mFromProgram)
			{
				skipped = true;
				return true;
			}
			switch (theRes.mType)
			{
			case ResType.ResType_Image:
			{
				ImageRes theRes8 = (ImageRes)theRes;
				return DoLoadImage(theRes8);
			}
			case ResType.ResType_Sound:
			{
				SoundRes theRes7 = (SoundRes)theRes;
				return DoLoadSound(theRes7);
			}
			case ResType.ResType_Font:
			{
				FontRes theRes6 = (FontRes)theRes;
				return DoLoadFont(theRes6);
			}
			case ResType.ResType_PopAnim:
			{
				PopAnimRes theRes5 = (PopAnimRes)theRes;
				return DoLoadPopAnim(theRes5);
			}
			case ResType.ResType_PIEffect:
			{
				PIEffectRes theRes4 = (PIEffectRes)theRes;
				return DoLoadPIEffect(theRes4);
			}
			case ResType.ResType_RenderEffect:
			{
				RenderEffectRes theRes3 = (RenderEffectRes)theRes;
				return DoLoadRenderEffect(theRes3);
			}
			case ResType.ResType_GenericResFile:
			{
				GenericResFileRes theRes2 = (GenericResFileRes)theRes;
				return DoLoadGenericResFile(theRes2);
			}
			default:
				return false;
			}
		}

		public virtual bool LoadNextResource()
		{
			if (HadError())
			{
				return false;
			}
			if (mCurResGroupList == null)
			{
				return false;
			}
			while (mCurResGroupListItr.MoveNext())
			{
				bool result = true;
				bool skipped = true;
				BaseRes current = mCurResGroupListItr.Current;
				if (current.mRefCount == 0)
				{
					result = DoLoadResource(current, out skipped);
				}
				current.mDirectLoaded = true;
				current.mRefCount++;
				if (!skipped)
				{
					return result;
				}
			}
			if (mCurCompositeResGroup.Length > 0 && mCompositeResGroupMap.ContainsKey(mCurCompositeResGroup))
			{
				CompositeResGroup compositeResGroup = mCompositeResGroupMap[mCurCompositeResGroup];
				int count = compositeResGroup.mSubGroups.Count;
				for (int i = mCurCompositeSubGroupIndex + 1; i < count; i++)
				{
					SubGroup subGroup = compositeResGroup.mSubGroups[i];
					if (subGroup.mGroupName.Length > 0 && (subGroup.mArtRes == 0 || subGroup.mArtRes == mCurArtRes) && (subGroup.mLocSet == 0 || subGroup.mLocSet == mCurLocSet))
					{
						mCurCompositeSubGroupIndex = i;
						StartLoadResources(subGroup.mGroupName, true);
						return LoadNextResource();
					}
				}
			}
			return false;
		}

		public virtual void ResourceLoadedHook(BaseRes theRes)
		{
		}

		public virtual void PrepareLoadResources(string theGroup)
		{
			if (mApp.mResStreamsManager != null && mApp.mResStreamsManager.IsInitialized())
			{
				mApp.mResStreamsManager.LoadGroup(theGroup);
			}
		}

		public virtual void StartLoadResources(string theGroup, bool fromComposite)
		{
			if (!fromComposite)
			{
				mError = "";
				mHasFailed = false;
				mCurCompositeResGroup = "";
				mCurCompositeSubGroupIndex = 0;
				if (mCompositeResGroupMap.ContainsKey(theGroup))
				{
					mCurResGroup = "";
					mCurResGroupList = null;
					mCurCompositeResGroup = theGroup;
					CompositeResGroup compositeResGroup = mCompositeResGroupMap[theGroup];
					int count = compositeResGroup.mSubGroups.Count;
					for (int i = 0; i < count; i++)
					{
						SubGroup subGroup = compositeResGroup.mSubGroups[i];
						if (subGroup.mGroupName.Length > 0 && (subGroup.mArtRes == 0 || subGroup.mArtRes == mCurArtRes) && (subGroup.mLocSet == 0 || subGroup.mLocSet == mCurLocSet))
						{
							mCurCompositeSubGroupIndex = i;
							StartLoadResources(subGroup.mGroupName, true);
							break;
						}
					}
					return;
				}
			}
			if (mResGroupMap.ContainsKey(theGroup))
			{
				mCurResGroup = theGroup;
				mCurResGroupList = mResGroupMap[theGroup];
				mCurResGroupListItr = mCurResGroupList.mResList.GetEnumerator();
			}
		}

		public virtual bool LoadResources(string theGroup)
		{
			if (mApp.mResStreamsManager != null && mApp.mResStreamsManager.IsInitialized())
			{
				mApp.mResStreamsManager.ForceLoadGroup(theGroup);
			}
			mError = "";
			mHasFailed = false;
			StartLoadResources(theGroup, false);
			while (LoadNextResource())
			{
			}
			if (!HadError())
			{
				mLoadedGroups.Add(theGroup);
				return true;
			}
			return false;
		}

		public bool ReplaceImage(string theId, Image theImage)
		{
			throw new NotImplementedException();
		}

		public bool ReplaceSound(string theId, int theSound)
		{
			throw new NotImplementedException();
		}

		public bool ReplaceFont(string theId, Font theFont)
		{
			throw new NotImplementedException();
		}

		public bool ReplacePopAnim(string theId, PopAnim theFont)
		{
			throw new NotImplementedException();
		}

		public bool ReplacePIEffect(string theId, PIEffect theFont)
		{
			throw new NotImplementedException();
		}

		public bool ReplaceRenderEffect(string theId, RenderEffectDefinition theDefinition)
		{
			throw new NotImplementedException();
		}

		public bool ReplaceGenericResFile(string theId, GenericResFile theFile)
		{
			throw new NotImplementedException();
		}

		public void DeleteImage(string theName)
		{
			BaseRes baseRes = GetBaseRes(0, theName);
			if (baseRes != null && baseRes.mDirectLoaded)
			{
				baseRes.mDirectLoaded = false;
				Deref(baseRes);
			}
		}

		public SharedImageRef LoadImage(string theName)
		{
			bool flag = theName == "IMAGE_UI_MAINMENU_TIKI";
			lock (mLoadCrit)
			{
				ImageRes imageRes = (ImageRes)GetBaseRes(0, theName);
				if (imageRes == null)
				{
					return null;
				}
				if (!imageRes.mDirectLoaded)
				{
					imageRes.mRefCount++;
					imageRes.mDirectLoaded = true;
				}
				if (imageRes.mImage.GetDeviceImage() != null)
				{
					return imageRes.mImage;
				}
				if (imageRes.mFromProgram)
				{
					return null;
				}
				if (!DoLoadImage(imageRes))
				{
					return null;
				}
				return imageRes.mImage;
			}
		}

		public Point GetImageOffset(string theName)
		{
			return ((ImageRes)GetBaseRes(0, theName))?.mOffset ?? aEmptyPoint;
		}

		public void DeleteFont(string theName)
		{
			throw new NotImplementedException();
		}

		public Font LoadFont(string theName)
		{
			lock (mLoadCrit)
			{
				FontRes fontRes = (FontRes)GetBaseRes(2, theName);
				if (fontRes == null)
				{
					return null;
				}
				if (!fontRes.mDirectLoaded)
				{
					fontRes.mRefCount++;
					fontRes.mDirectLoaded = true;
				}
				if (fontRes.mFont != null)
				{
					return fontRes.mFont;
				}
				if (fontRes.mFromProgram)
				{
					return null;
				}
				if (!DoLoadFont(fontRes))
				{
					return null;
				}
				return fontRes.mFont;
			}
		}

		public int LoadSound(string theName)
		{
			lock (mLoadCrit)
			{
				SoundRes soundRes = (SoundRes)GetBaseRes(1, theName);
				if (soundRes == null)
				{
					return -1;
				}
				if (!soundRes.mDirectLoaded)
				{
					soundRes.mRefCount++;
					soundRes.mDirectLoaded = true;
				}
				if (soundRes.mSoundId != -1)
				{
					return soundRes.mSoundId;
				}
				if (soundRes.mFromProgram)
				{
					return -1;
				}
				if (!DoLoadSound(soundRes))
				{
					return -1;
				}
				return soundRes.mSoundId;
			}
		}

		public void DeleteSound(string theName)
		{
			throw new NotImplementedException();
		}

		public void DeletePopAnim(string theName)
		{
			throw new NotImplementedException();
		}

		public PopAnim LoadPopAnim(string theName)
		{
			lock (mLoadCrit)
			{
				PopAnimRes popAnimRes = (PopAnimRes)GetBaseRes(3, theName);
				if (popAnimRes == null)
				{
					return null;
				}
				if (!popAnimRes.mDirectLoaded)
				{
					popAnimRes.mRefCount++;
					popAnimRes.mDirectLoaded = true;
				}
				if (popAnimRes.mPopAnim != null)
				{
					return popAnimRes.mPopAnim;
				}
				if (popAnimRes.mFromProgram)
				{
					return null;
				}
				if (!DoLoadPopAnim(popAnimRes))
				{
					return null;
				}
				return popAnimRes.mPopAnim;
			}
		}

		public void DeletePIEffect(string theName)
		{
			throw new NotImplementedException();
		}

		public PIEffect LoadPIEffect(string theName)
		{
			PIEffectRes pIEffectRes = (PIEffectRes)GetBaseRes(4, theName);
			if (pIEffectRes == null)
			{
				return null;
			}
			if (!pIEffectRes.mDirectLoaded)
			{
				pIEffectRes.mRefCount++;
				pIEffectRes.mDirectLoaded = true;
			}
			if (pIEffectRes.mPIEffect != null)
			{
				return pIEffectRes.mPIEffect;
			}
			if (pIEffectRes.mFromProgram)
			{
				return null;
			}
			if (!DoLoadPIEffect(pIEffectRes))
			{
				return null;
			}
			return pIEffectRes.mPIEffect;
		}

		public void DeleteRenderEffect(string theName)
		{
			throw new NotImplementedException();
		}

		public RenderEffectDefinition LoadRenderEffect(string theName)
		{
			RenderEffectRes renderEffectRes = (RenderEffectRes)GetBaseRes(5, theName);
			if (renderEffectRes == null)
			{
				return null;
			}
			if (!renderEffectRes.mDirectLoaded)
			{
				renderEffectRes.mRefCount++;
				renderEffectRes.mDirectLoaded = true;
			}
			if (renderEffectRes.mRenderEffectDefinition != null)
			{
				return renderEffectRes.mRenderEffectDefinition;
			}
			if (renderEffectRes.mFromProgram)
			{
				return null;
			}
			if (!DoLoadRenderEffect(renderEffectRes))
			{
				return null;
			}
			return renderEffectRes.mRenderEffectDefinition;
		}

		public void DeleteGenericResFile(string theName)
		{
			throw new NotImplementedException();
		}

		public GenericResFile LoadGenericResFile(string theName)
		{
			throw new NotImplementedException();
		}

		public SharedImageRef GetImage(string theId)
		{
			return ((ImageRes)GetBaseRes(0, theId))?.mImage;
		}

		public int GetSound(string theId)
		{
			return ((SoundRes)GetBaseRes(1, theId))?.mSoundId ?? 0;
		}

		public Font GetFont(string theId)
		{
			return ((FontRes)GetBaseRes(2, theId))?.mFont;
		}

		public PopAnim GetPopAnim(string theId)
		{
			return ((PopAnimRes)GetBaseRes(3, theId))?.mPopAnim;
		}

		public PIEffect GetPIEffect(string theId)
		{
			return ((PIEffectRes)GetBaseRes(4, theId))?.mPIEffect;
		}

		public RenderEffectDefinition GetRenderEffect(string theId)
		{
			return ((RenderEffectRes)GetBaseRes(5, theId))?.mRenderEffectDefinition;
		}

		public GenericResFile GetGenericResFile(string theId)
		{
			return ((GenericResFileRes)GetBaseRes(6, theId))?.mGenericResFile;
		}

		public string GetIdByPath(string thePath)
		{
			string text = thePath.Replace('/', '\\');
			for (int i = 0; i < 7; i++)
			{
				Dictionary<string, BaseRes>.Enumerator enumerator = mResMaps[i].GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Value.mPath == text)
					{
						return enumerator.Current.Value.mId;
					}
				}
			}
			text = text.ToUpper();
			for (int j = 0; j < 7; j++)
			{
				Dictionary<string, BaseRes>.Enumerator enumerator2 = mResMaps[j].GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.Value.mPath.ToUpper() == text)
					{
						return enumerator2.Current.Value.mId;
					}
				}
			}
			return "";
		}

		public Dictionary<string, string> GetImageAttributes(string theId)
		{
			return ((ImageRes)GetBaseRes(0, theId))?.mXMLAttributes;
		}

		public virtual SharedImageRef GetImageThrow(string theId, int artRes, bool optional)
		{
			optional = true;
			if (mApp.mShutdown)
			{
				return null;
			}
			if (artRes != 0 && artRes != mCurArtRes)
			{
				Fail("Attempted to load image of incorrect art resolution " + artRes + " (expected " + mCurArtRes + "):" + theId);
				throw new ResourceManagerException(GetErrorText());
			}
			ImageRes imageRes = (ImageRes)GetBaseRes(0, theId);
			if (imageRes != null)
			{
				if (imageRes.mImage.GetMemoryImage() != null)
				{
					return imageRes.mImage;
				}
				if (mAllowMissingProgramResources && imageRes.mFromProgram)
				{
					return null;
				}
			}
			else if (optional)
			{
				return null;
			}
			Fail("Image resource not found:" + theId);
			imageRes = (ImageRes)GetBaseRes(0, theId);
			throw new ResourceManagerException(GetErrorText());
		}

		public virtual int GetSoundThrow(string theId)
		{
			if (mApp.mShutdown)
			{
				return 0;
			}
			SoundRes soundRes = (SoundRes)GetBaseRes(1, theId);
			soundRes.mSoundId = GetSound(theId);
			if (soundRes != null)
			{
				if (soundRes.mSoundId != -1)
				{
					return soundRes.mSoundId;
				}
				if (mAllowMissingProgramResources && soundRes.mFromProgram)
				{
					return -1;
				}
			}
			return -1;
		}

		public virtual Font GetFontThrow(string theId, int artRes)
		{
			if (mApp.mShutdown)
			{
				return null;
			}
			if (artRes != 0 && artRes != mCurArtRes)
			{
				Fail("Attempted to load font of incorrect art resolution " + artRes + " (expected " + mCurArtRes + "):" + theId);
				throw new ResourceManagerException(GetErrorText());
			}
			FontRes fontRes = (FontRes)GetBaseRes(2, theId);
			if (fontRes != null)
			{
				if (fontRes.mFont != null)
				{
					return fontRes.mFont;
				}
				if (mAllowMissingProgramResources && fontRes.mFromProgram)
				{
					return null;
				}
			}
			return null;
		}

		public virtual PopAnim GetPopAnimThrow(string theId)
		{
			return null;
		}

		public virtual PIEffect GetPIEffectThrow(string theId)
		{
			if (mApp.mShutdown)
			{
				return null;
			}
			PIEffectRes pIEffectRes = (PIEffectRes)GetBaseRes(4, theId);
			if (pIEffectRes != null)
			{
				if (pIEffectRes.mPIEffect != null)
				{
					return pIEffectRes.mPIEffect;
				}
				if (mAllowMissingProgramResources && pIEffectRes.mFromProgram)
				{
					return null;
				}
			}
			Fail($"PIEffect resource not found: {theId}");
			return null;
		}

		public virtual RenderEffectDefinition GetRenderEffectThrow(string theId)
		{
			return null;
		}

		public virtual GenericResFile GetGenericResFileThrow(string theId)
		{
			return null;
		}

		public ResourceRef GetResourceRef(BaseRes theBaseRes)
		{
			ResourceRef resourceRef = new ResourceRef();
			bool skipped = false;
			resourceRef.mBaseResP = theBaseRes;
			if (theBaseRes.mRefCount == 0)
			{
				DoLoadResource(theBaseRes, out skipped);
			}
			theBaseRes.mRefCount++;
			return resourceRef;
		}

		public ResourceRef GetResourceRef(int theType, string theId)
		{
			BaseRes baseRes = GetBaseRes(theType, theId);
			if (baseRes != null)
			{
				return GetResourceRef(baseRes);
			}
			return null;
		}

		public ResourceRef GetResourceRefFromPath(string theFileName)
		{
			string text = theFileName.ToUpper();
			if (text.IndexOf(".") != -1)
			{
				text = text.Substring(0, text.IndexOf("."));
			}
			if (mResFromPathMap.ContainsKey(text))
			{
				return GetResourceRef(mResFromPathMap[text]);
			}
			return null;
		}

		public Point? GetOffsetOfImage(string theId)
		{
			return ((ImageRes)GetBaseRes(0, theId))?.mOffset;
		}

		public ResourceRef GetImageRef(string theId)
		{
			return GetResourceRef(0, theId);
		}

		public ResourceRef GetImageRef(Image theGlobalPtrRef)
		{
			string mNameForRes = theGlobalPtrRef.mNameForRes;
			return GetResourceRef(0, mNameForRes);
		}

		public ResourceRef GetSoundRef(string theId)
		{
			return GetResourceRef(1, theId);
		}

		public ResourceRef GetSoundRef(int theGlobalPtrRef)
		{
			throw new NotImplementedException();
		}

		public ResourceRef GetFontRef(string theId)
		{
			return GetResourceRef(2, theId);
		}

		public ResourceRef GetPopAnimRef(string theId)
		{
			return GetResourceRef(3, theId);
		}

		public ResourceRef GetPopAnimRef(PopAnim theGlobalPtrRef)
		{
			throw new NotImplementedException();
		}

		public ResourceRef GetPIEffectRef(string theId)
		{
			return GetResourceRef(4, theId);
		}

		public ResourceRef GetPIEffectRef(PIEffect theGlobalPtrRef)
		{
			throw new NotImplementedException();
		}

		public ResourceRef GetRenderEffectRef(string theId)
		{
			return GetResourceRef(5, theId);
		}

		public ResourceRef GetRenderEffectRef(RenderEffectDefinition theGlobalPtrRef)
		{
			throw new NotImplementedException();
		}

		public ResourceRef GetGenericResFileRef(string theId)
		{
			return GetResourceRef(6, theId);
		}

		public ResourceRef GetGenericResFileRef(GenericResFile theGlobalPtrRef)
		{
			throw new NotImplementedException();
		}

		public void SetAllowMissingProgramImages(bool allow)
		{
			mAllowMissingProgramResources = allow;
		}

		public virtual void DeleteResources(string theGroup)
		{
			for (int i = 0; i < 7; i++)
			{
				DeleteResources(mResMaps[i], theGroup);
			}
			mLoadedGroups.Remove(theGroup);
			if (mApp.mResStreamsManager != null && mApp.mResStreamsManager.IsInitialized())
			{
				mApp.mResStreamsManager.DeleteGroup(theGroup);
			}
		}

		public void DeleteExtraImageBuffers(string theGroup)
		{
		}

		public ResGroup GetCurResGroupList()
		{
			return mCurResGroupList;
		}

		public string GetCurResGroup()
		{
			return mCurResGroup;
		}

		public void DumpCurResGroup(ref string theDestStr)
		{
			ResGroup resGroup = mResGroupMap[mCurResGroup];
			List<BaseRes>.Enumerator enumerator = resGroup.mResList.GetEnumerator();
			object obj = theDestStr;
			theDestStr = string.Concat(obj, "\n About to dump ", resGroup.mResList.Count, " elements from current res group name \r\n");
			while (enumerator.MoveNext())
			{
				BaseRes current = enumerator.Current;
				string text = current.mId + ":" + current.mPath + "\r\n";
				theDestStr += text;
				if (current.mFromProgram)
				{
					theDestStr += "     res is from program\r\n";
				}
				else if (current.mType == ResType.ResType_Image)
				{
					theDestStr += "     res is an image\r\n";
				}
				else if (current.mType == ResType.ResType_Sound)
				{
					theDestStr += "     res is a sound\r\n";
				}
				else if (current.mType == ResType.ResType_Font)
				{
					theDestStr += "     res is a font\r\n";
				}
				else if (current.mType == ResType.ResType_PopAnim)
				{
					theDestStr += "     res is a popanim\r\n";
				}
				else if (current.mType == ResType.ResType_PIEffect)
				{
					theDestStr += "     res is a pieffect\r\n";
				}
				else if (current.mType == ResType.ResType_RenderEffect)
				{
					theDestStr += "     res is a rendereffectdefinition\r\n";
				}
				else if (current.mType == ResType.ResType_GenericResFile)
				{
					theDestStr += "     res is a genericresfile\r\n";
				}
				if (enumerator.Current == mCurResGroupListItr.Current)
				{
					theDestStr += "iterator has reached mCurResGroupItr\r\n";
				}
			}
			theDestStr += "Done dumping resources\r\n";
		}

		public void DumpAllGroup(ref string theDestStr)
		{
			string text = mCurResGroup;
			Dictionary<string, ResGroup>.Enumerator enumerator = mResGroupMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				mCurResGroup = enumerator.Current.Key;
				DumpCurResGroup(ref theDestStr);
			}
			mCurResGroup = text;
		}

		public string GetLocaleFolder(bool addTrailingSlash)
		{
			uint num = mCurLocSet;
			if (num == 0)
			{
				return "";
			}
			string empty = string.Empty;
			switch (num)
			{
			case 1145390149u:
				empty = "de-DE";
				break;
			case 1163085139u:
				empty = "es-ES";
				break;
			case 1179797074u:
				empty = "fr-FR";
				break;
			case 1230260564u:
				empty = "it-IT";
				break;
			default:
				empty = "en-US";
				break;
			}
			string text = "locales\\" + empty;
			if (addTrailingSlash)
			{
				text += '\\';
			}
			return text;
		}

		public uint GetLocSetForLocaleName(string theLocaleName)
		{
			throw new NotImplementedException();
		}

		public void PrepareLoadResourcesList(string[] theGroups)
		{
			if (mApp.mResStreamsManager != null && mApp.mResStreamsManager.IsInitialized())
			{
				for (int i = 0; theGroups[i] != null; i++)
				{
					int num = mApp.mResStreamsManager.LookupGroup(theGroups[i]);
					if (num != -1)
					{
						mApp.mResStreamsManager.LoadGroup(num);
					}
				}
			}
			LoadGroupAsyn(theGroups);
		}

		public float GetLoadResourcesListProgress(string[] theGroups)
		{
			int num = 0;
			int num2 = 0;
			foreach (GroupLoadInfo item in mGroupToLoad)
			{
				num += item.mTotalFile;
				num2 += item.mCurrentFile;
			}
			if (num == 0)
			{
				return 0f;
			}
			if (mLoadFinished || num2 == num)
			{
				if (!mLoadFinished)
				{
					return 0.99f;
				}
				return 1f;
			}
			return (float)num2 / (float)num;
		}

		public bool IsLoadSuccess()
		{
			return mLoadSuccess;
		}

		public void LoadGroupAsyn(string[] group)
		{
			if (mIsLoading)
			{
				throw new ResourceManagerException("Can't loading two group at one time");
			}
			mError = "";
			mHasFailed = false;
			mLoadFinished = false;
			mGroupToLoad.Clear();
			GetLoadingGroup(group);
			mIsLoading = true;
			mLoadSuccess = false;
			LoadingProc();
			// mLoadingProc = LoadingProc;
			// mLoadingThread = new Thread(mLoadingProc);
			// mLoadingThread.Name = "ResLoadingThread";
			// mLoadingThread.Start();
		}

		private void GetLoadingGroup(string[] group)
		{
			mGroupToLoad.Clear();
			mError = "";
			mHasFailed = false;
			foreach (string text in group)
			{
				if (text == null)
				{
					continue;
				}
				if (mCompositeResGroupMap.ContainsKey(text))
				{
					CompositeResGroup compositeResGroup = mCompositeResGroupMap[text];
					int count = compositeResGroup.mSubGroups.Count;
					for (int j = 0; j < count; j++)
					{
						SubGroup subGroup = compositeResGroup.mSubGroups[j];
						if (subGroup.mGroupName.Length > 0 && (subGroup.mArtRes == 0 || subGroup.mArtRes == mCurArtRes) && (subGroup.mLocSet == 0 || subGroup.mLocSet == mCurLocSet))
						{
							mCurCompositeSubGroupIndex = j;
							if (mResGroupMap.ContainsKey(subGroup.mGroupName))
							{
								ResGroup resGroup = mResGroupMap[subGroup.mGroupName];
								mGroupToLoad.Add(new GroupLoadInfo(subGroup.mGroupName, resGroup.mResList.Count));
							}
						}
					}
				}
				else if (mResGroupMap.ContainsKey(text))
				{
					ResGroup resGroup2 = mResGroupMap[text];
					mGroupToLoad.Add(new GroupLoadInfo(text, resGroup2.mResList.Count));
				}
			}
		}

		private void LoadingProc()
		{
			foreach (GroupLoadInfo item in mGroupToLoad)
			{
				StartLoadResources(item.mName, false);
				while (LoadNextResource())
				{
					item.mCurrentFile++;
				}
				if (!HadError())
				{
					mLoadedGroups.Add(mCurResGroup);
					mLoadSuccess = true;
					continue;
				}
				mLoadSuccess = false;
				break;
			}
			mIsLoading = false;
			mLoadFinished = true;
		}

		public static void ReadIntVector(string theVal, List<int> theVector)
		{
			theVector.Clear();
			string[] array = theVal.Split(',');
			string[] array2 = array;
			foreach (string s in array2)
			{
				theVector.Add(int.Parse(s));
			}
		}
	}
}
