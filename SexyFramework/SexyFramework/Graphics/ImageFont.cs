using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace SexyFramework.Graphics
{
	public class ImageFont : Font
	{
		public static bool mAlphaCorrectionEnabled;

		public static bool mOrderedHash;

		public FontData mFontData;

		public int mPointSize;

		public List<string> mTagVector = new List<string>();

		public bool mActivateAllLayers;

		public bool mActiveListValid;

		public ActiveFontLayer[] mActiveLayerList = new ActiveFontLayer[0];

		public double mScale;

		public bool mForceScaledImagesWhite;

		public bool mWantAlphaCorrection;

		public MemoryImage mFontImage;

		public virtual void GenerateActiveFontLayers()
		{
			if (!mFontData.mInitialized)
			{
				return;
			}
			List<ActiveFontLayer> list = new List<ActiveFontLayer>();
			mAscent = 0;
			mAscentPadding = 0;
			mHeight = 0;
			mLineSpacingOffset = 0;
			LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext())
			{
				FontLayer current = enumerator.Current;
				if (mPointSize < current.mMinPointSize || (mPointSize > current.mMaxPointSize && current.mMaxPointSize != -1))
				{
					continue;
				}
				bool flag2 = true;
				for (int i = 0; i < current.mRequiredTags.Count; i++)
				{
					if (mTagVector.IndexOf(current.mRequiredTags[i]) == -1)
					{
						flag2 = false;
					}
				}
				for (int i = 0; i < mTagVector.Count; i++)
				{
					if (current.mExcludedTags.IndexOf(mTagVector[i]) != -1)
					{
						flag2 = false;
					}
				}
				if (flag2 | mActivateAllLayers)
				{
					list.Add(new ActiveFontLayer());
					ActiveFontLayer activeFontLayer = list.Last();
					activeFontLayer.mBaseFontLayer = current;
					activeFontLayer.mUseAlphaCorrection = mWantAlphaCorrection & current.mImageIsWhite;
					double num = 1.0;
					double num2 = mScale;
					if (mScale == 1.0 && (current.mPointSize == 0 || mPointSize == current.mPointSize))
					{
						activeFontLayer.mScaledImages[7] = current.mImage;
						if (mFontImage != null)
						{
							activeFontLayer.mScaledImages[7].mUnsharedImage = mFontImage;
						}
						int count = current.mCharDataHashTable.mCharData.Count;
						CharData[] array = current.mCharDataHashTable.mCharData.ToArray();
						for (int j = 0; j < count; j++)
						{
							CharDataHashEntry charDataHashEntry = current.mCharDataHashTable.mHashEntries[array[j].mHashEntryIndex];
							activeFontLayer.mScaledCharImageRects.Add((char)charDataHashEntry.mChar, array[j].mImageRect);
						}
					}
					else
					{
						if (current.mPointSize != 0)
						{
							num = current.mPointSize;
							num2 = (double)mPointSize * mScale;
						}
						MemoryImage memoryImage = new MemoryImage();
						int num3 = 0;
						bool flag3 = true;
						int num4 = 0;
						int num5 = 0;
						int count2 = current.mCharDataHashTable.mCharData.Count;
						CharData[] array2 = current.mCharDataHashTable.mCharData.ToArray();
						for (int k = 0; k < count2; k++)
						{
							Rect mImageRect = array2[k].mImageRect;
							int mY = array2[k].mOffset.mY;
							int num6 = mY + mImageRect.mHeight;
							num4 = Math.Min(mY, num4);
							num5 = Math.Max(num6, num5);
							if (num4 != mY || num5 != num6)
							{
								flag3 = false;
							}
							num3 += mImageRect.mWidth + 2;
						}
						if (!flag3)
						{
							MemoryImage memoryImage2 = new MemoryImage();
							memoryImage2.Create(num3, num5 - num4);
							Graphics graphics = new Graphics(memoryImage2);
							num3 = 0;
							count2 = current.mCharDataHashTable.mCharData.Count;
							array2 = current.mCharDataHashTable.mCharData.ToArray();
							for (int l = 0; l < count2; l++)
							{
								Rect mImageRect2 = array2[l].mImageRect;
								if (current.mImage.GetImage() != null)
								{
									graphics.DrawImage(current.mImage.GetImage(), num3, array2[l].mOffset.mY - num4, mImageRect2);
								}
								array2[l].mOffset.mY = num4;
								array2[l].mOffset.mX--;
								num3 += new Rect(num3, 0, mImageRect2.mWidth + 2, num5 - num4).mWidth;
							}
							current.mImage.mUnsharedImage = memoryImage2;
							current.mImage.mOwnsUnshared = true;
							graphics.ClearRenderContext();
						}
						num3 = 0;
						int num7 = 0;
						count2 = current.mCharDataHashTable.mCharData.Count;
						array2 = current.mCharDataHashTable.mCharData.ToArray();
						for (int m = 0; m < count2; m++)
						{
							Rect mImageRect3 = array2[m].mImageRect;
							int num8 = (int)Math.Floor((double)array2[m].mOffset.mX * num2 / (double)(float)num);
							int num9 = (int)Math.Ceiling((double)(array2[m].mOffset.mX + mImageRect3.mWidth) * num2 / (double)(float)num);
							int theWidth = Math.Max(0, num9 - num8 - 1);
							int num10 = (int)Math.Floor((double)array2[m].mOffset.mY * num2 / (double)(float)num);
							int num11 = (int)Math.Ceiling((double)(array2[m].mOffset.mY + mImageRect3.mHeight) * num2 / (double)(float)num);
							int theHeight = Math.Max(0, num11 - num10 - 1);
							Rect value = new Rect(num3, 0, theWidth, theHeight);
							if (value.mHeight > num7)
							{
								num7 = value.mHeight;
							}
							CharDataHashEntry charDataHashEntry2 = current.mCharDataHashTable.mHashEntries[array2[m].mHashEntryIndex];
							activeFontLayer.mScaledCharImageRects.Add((char)charDataHashEntry2.mChar, value);
							num3 += value.mWidth;
						}
						activeFontLayer.mScaledImages[7].mUnsharedImage = memoryImage;
						activeFontLayer.mScaledImages[7].mOwnsUnshared = true;
						memoryImage.Create(num3, num7);
						Graphics graphics2 = new Graphics(memoryImage);
						count2 = current.mCharDataHashTable.mCharData.Count;
						array2 = current.mCharDataHashTable.mCharData.ToArray();
						for (int n = 0; n < count2; n++)
						{
							if (current.mImage.GetImage() != null)
							{
								CharDataHashEntry charDataHashEntry3 = current.mCharDataHashTable.mHashEntries[array2[n].mHashEntryIndex];
								graphics2.DrawImage(current.mImage.GetImage(), activeFontLayer.mScaledCharImageRects[(char)charDataHashEntry3.mChar], array2[n].mImageRect);
							}
						}
						if (mForceScaledImagesWhite)
						{
							int num12 = memoryImage.mWidth * memoryImage.mHeight;
							uint[] bits = memoryImage.GetBits();
							for (int num13 = 0; num13 < num12; num13++)
							{
								bits[num13] |= 0xFFFFFF;
							}
						}
						memoryImage.AddImageFlags(128u);
						memoryImage.Palletize();
						graphics2.ClearRenderContext();
					}
					int num14 = (((double)current.mAscent * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mAscent * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mAscent * num2 / (double)(float)num - 0.501)));
					if (num14 > mAscent)
					{
						mAscent = num14;
					}
					if (current.mHeight != 0)
					{
						int num15 = (((double)current.mHeight * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mHeight * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mHeight * num2 / (double)(float)num - 0.501)));
						if (num15 > mHeight)
						{
							mHeight = num15;
						}
					}
					else
					{
						int num16 = (((double)current.mDefaultHeight * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mDefaultHeight * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mDefaultHeight * num2 / (double)(float)num - 0.501)));
						if (num16 > mHeight)
						{
							mHeight = num16;
						}
					}
					int num17 = (((double)current.mAscentPadding * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mAscentPadding * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mAscentPadding * num2 / (double)(float)num - 0.501)));
					if (flag || num17 < mAscentPadding)
					{
						mAscentPadding = num17;
					}
					int num18 = (((double)current.mLineSpacingOffset * num2 / (double)(float)num >= 0.0) ? ((int)((double)current.mLineSpacingOffset * num2 / (double)(float)num + 0.501)) : ((int)((double)current.mLineSpacingOffset * num2 / (double)(float)num - 0.501)));
					if (flag || num18 > mLineSpacingOffset)
					{
						mLineSpacingOffset = num18;
					}
				}
				flag = false;
			}
			mActiveLayerList = list.ToArray();
		}

		public virtual void DrawStringEx(Graphics g, int theX, int theY, string theString, Color theColor, Rect theClipRect, LinkedList<Rect> theDrawnAreas, ref int theWidth)
		{
			lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
			{
				for (int i = 0; i < 256; i++)
				{
					GlobalMembersImageFont.gRenderHead[i] = null;
					GlobalMembersImageFont.gRenderTail[i] = null;
				}
				theDrawnAreas?.Clear();
				if (!mFontData.mInitialized)
				{
					theWidth = 0;
					return;
				}
				Prepare();
				bool colorizeImages = g.GetColorizeImages();
				g.SetColorizeImages(true);
				int num = theX;
				int num2 = 0;
				for (int j = 0; j < theString.Length; j++)
				{
					char mappedChar = GetMappedChar(theString[j]);
					char c = '\0';
					if (j < theString.Length - 1)
					{
						c = GetMappedChar(theString[j + 1]);
					}
					int num3 = num;
					for (int k = 0; k < mActiveLayerList.Length; k++)
					{
						ActiveFontLayer activeFontLayer = mActiveLayerList[k];
						CharData charData = activeFontLayer.mBaseFontLayer.GetCharData(mappedChar);
						int num4 = num;
						int num5 = activeFontLayer.mBaseFontLayer.mPointSize;
						double num6 = mScale;
						if (num5 != 0)
						{
							num6 *= (double)mPointSize / (double)num5;
						}
						int num7;
						int num8;
						int num9;
						int num10;
						if (num6 == 1.0)
						{
							num7 = num4 + activeFontLayer.mBaseFontLayer.mOffset.mX + charData.mOffset.mX;
							num8 = theY - (activeFontLayer.mBaseFontLayer.mAscent - activeFontLayer.mBaseFontLayer.mOffset.mY - charData.mOffset.mY);
							num9 = charData.mWidth;
							if (c != 0)
							{
								num10 = ((c != ' ') ? activeFontLayer.mBaseFontLayer.mSpacing : 0);
								if (charData.mKerningCount != 0)
								{
									int mKerningCount = charData.mKerningCount;
									for (int l = 0; l < mKerningCount; l++)
									{
										if (activeFontLayer.mBaseFontLayer.mKerningData[l + charData.mKerningFirst].mChar == c)
										{
											num10 += activeFontLayer.mBaseFontLayer.mKerningData[l + charData.mKerningFirst].mOffset;
										}
									}
								}
							}
							else
							{
								num10 = 0;
							}
						}
						else
						{
							num7 = num4 + (int)Math.Floor((double)(activeFontLayer.mBaseFontLayer.mOffset.mX + charData.mOffset.mX) * num6);
							num8 = theY - (int)Math.Floor((double)(activeFontLayer.mBaseFontLayer.mAscent - activeFontLayer.mBaseFontLayer.mOffset.mY - charData.mOffset.mY) * num6);
							num9 = (int)((double)charData.mWidth * num6);
							if (c != 0)
							{
								num10 = activeFontLayer.mBaseFontLayer.mSpacing;
								if (charData.mKerningCount != 0)
								{
									int mKerningCount2 = charData.mKerningCount;
									for (int m = 0; m < mKerningCount2; m++)
									{
										if (activeFontLayer.mBaseFontLayer.mKerningData[m + charData.mKerningFirst].mChar == c)
										{
											num10 += (int)((double)activeFontLayer.mBaseFontLayer.mKerningData[m + charData.mKerningFirst].mOffset * num6);
										}
									}
								}
							}
							else
							{
								num10 = 0;
							}
						}
						Color mColor = default(Color);
						if (activeFontLayer.mColorStack.Count == 0)
						{
							mColor.mRed = Math.Min(theColor.mRed * activeFontLayer.mBaseFontLayer.mColorMult.mRed / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mRed, 255);
							mColor.mGreen = Math.Min(theColor.mGreen * activeFontLayer.mBaseFontLayer.mColorMult.mGreen / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mGreen, 255);
							mColor.mBlue = Math.Min(theColor.mBlue * activeFontLayer.mBaseFontLayer.mColorMult.mBlue / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mBlue, 255);
							mColor.mAlpha = Math.Min(theColor.mAlpha * activeFontLayer.mBaseFontLayer.mColorMult.mAlpha / 255 + activeFontLayer.mBaseFontLayer.mColorAdd.mAlpha, 255);
						}
						else
						{
							Color color = activeFontLayer.mColorStack[activeFontLayer.mColorStack.Count - 1];
							mColor.mRed = Math.Min(theColor.mRed * activeFontLayer.mBaseFontLayer.mColorMult.mRed * color.mRed / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mRed * color.mRed / 255, 255);
							mColor.mGreen = Math.Min(theColor.mGreen * activeFontLayer.mBaseFontLayer.mColorMult.mGreen * color.mGreen / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mGreen * color.mGreen / 255, 255);
							mColor.mBlue = Math.Min(theColor.mBlue * activeFontLayer.mBaseFontLayer.mColorMult.mBlue * color.mBlue / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mBlue * color.mBlue / 255, 255);
							mColor.mAlpha = Math.Min(theColor.mAlpha * activeFontLayer.mBaseFontLayer.mColorMult.mAlpha * color.mAlpha / 65025 + activeFontLayer.mBaseFontLayer.mColorAdd.mAlpha * color.mAlpha / 255, 255);
						}
						int num11 = activeFontLayer.mBaseFontLayer.mBaseOrder + charData.mOrder;
						if (num2 >= 4096)
						{
							break;
						}
						RenderCommand renderCommand = GlobalMembersImageFont.GetRenderCommandPool()[num2++];
						renderCommand.mFontLayer = activeFontLayer;
						renderCommand.mColor = mColor;
						renderCommand.mDest[0] = num7;
						renderCommand.mDest[1] = num8;
						renderCommand.mSrc[0] = GlobalMembers.sexyatof(activeFontLayer.mScaledCharImageRects, mappedChar).mX;
						renderCommand.mSrc[1] = GlobalMembers.sexyatof(activeFontLayer.mScaledCharImageRects, mappedChar).mY;
						renderCommand.mSrc[2] = GlobalMembers.sexyatof(activeFontLayer.mScaledCharImageRects, mappedChar).mWidth;
						renderCommand.mSrc[3] = GlobalMembers.sexyatof(activeFontLayer.mScaledCharImageRects, mappedChar).mHeight;
						renderCommand.mMode = activeFontLayer.mBaseFontLayer.mDrawMode;
						renderCommand.mNext = null;
						int num12 = Math.Min(Math.Max(num11 + 128, 0), 255);
						if (GlobalMembersImageFont.gRenderTail[num12] == null)
						{
							GlobalMembersImageFont.gRenderTail[num12] = renderCommand;
							GlobalMembersImageFont.gRenderHead[num12] = renderCommand;
						}
						else
						{
							GlobalMembersImageFont.gRenderTail[num12].mNext = renderCommand;
							GlobalMembersImageFont.gRenderTail[num12] = renderCommand;
						}
						if (theDrawnAreas != null)
						{
							Rect value = new Rect(num7, num8, activeFontLayer.mScaledCharImageRects[mappedChar].mWidth, activeFontLayer.mScaledCharImageRects[mappedChar].mHeight);
							theDrawnAreas.AddLast(value);
						}
						int num13 = num9 + num10;
						if (num13 <= 0 && mappedChar == ' ')
						{
							num13 = num9;
						}
						num4 += num13;
						if (num4 > num3)
						{
							num3 = num4;
						}
					}
					num = num3;
				}
				theWidth = num - theX;
				Color color2 = g.GetColor();
				for (int i = 0; i < 256; i++)
				{
					for (RenderCommand renderCommand2 = GlobalMembersImageFont.gRenderHead[i]; renderCommand2 != null; renderCommand2 = renderCommand2.mNext)
					{
						if (renderCommand2.mFontLayer != null)
						{
							int drawMode = g.GetDrawMode();
							if (renderCommand2.mMode != -1)
							{
								g.SetDrawMode(renderCommand2.mMode);
							}
							g.SetColor(renderCommand2.mColor);
							int num14 = renderCommand2.mColor.mRed * 19660 + renderCommand2.mColor.mGreen * 38666 + renderCommand2.mColor.mBlue * 7208 >> 21;
							if (renderCommand2.mFontLayer.mUseAlphaCorrection && renderCommand2.mFontLayer.mBaseFontLayer.mUseAlphaCorrection && mAlphaCorrectionEnabled && num14 != 7)
							{
								MemoryImage memoryImage = renderCommand2.mFontLayer.mScaledImages[0].GetMemoryImage();
								if (g.Is3D())
								{
									if (false)
									{
										if (memoryImage == null || memoryImage.mColorTable == null || memoryImage.mColorTable[254] != GlobalMembersImageFont.FONT_PALETTES[0][254])
										{
											memoryImage = renderCommand2.mFontLayer.GenerateAlphaCorrectedImage(0).GetMemoryImage();
										}
									}
									else
									{
										MemoryImage memoryImage2 = renderCommand2.mFontLayer.mScaledImages[num14].GetMemoryImage();
										if (memoryImage2 == null || memoryImage2.mColorTable == null || memoryImage2.mColorTable[254] != GlobalMembersImageFont.FONT_PALETTES[num14][254])
										{
											memoryImage2 = renderCommand2.mFontLayer.GenerateAlphaCorrectedImage(num14).GetMemoryImage();
										}
										g.DrawImage(memoryImage2, renderCommand2.mDest[0], renderCommand2.mDest[1], new Rect(renderCommand2.mSrc[0], renderCommand2.mSrc[1], renderCommand2.mSrc[2], renderCommand2.mSrc[3]));
									}
								}
								else
								{
									if (memoryImage == null || memoryImage.mColorTable == null)
									{
										memoryImage = renderCommand2.mFontLayer.GenerateAlphaCorrectedImage(0).GetMemoryImage();
									}
									if (memoryImage.mColorTable[254] != GlobalMembersImageFont.FONT_PALETTES[num14][254])
									{
										Array.Copy(memoryImage.mColorTable, GlobalMembersImageFont.FONT_PALETTES[num14], 1024);
										if (memoryImage.mNativeAlphaData != null)
										{
											for (int n = 0; n < 256; n++)
											{
												uint num15 = GlobalMembersImageFont.FONT_PALETTES[num14][n] >> 24;
												memoryImage.mNativeAlphaData[n] = (num15 << 24) | (num15 << 16) | (num15 << 8) | num15;
											}
										}
									}
									g.DrawImage(memoryImage, renderCommand2.mDest[0], renderCommand2.mDest[1], new Rect(renderCommand2.mSrc[0], renderCommand2.mSrc[1], renderCommand2.mSrc[2], renderCommand2.mSrc[3]));
								}
							}
							else
							{
								g.DrawImage(renderCommand2.mFontLayer.mScaledImages[7].GetImage(), renderCommand2.mDest[0], renderCommand2.mDest[1], new Rect(renderCommand2.mSrc[0], renderCommand2.mSrc[1], renderCommand2.mSrc[2], renderCommand2.mSrc[3]));
							}
							g.SetDrawMode(drawMode);
						}
					}
				}
				g.SetColor(color2);
				g.SetColorizeImages(colorizeImages);
			}
		}

		public static void EnableAlphaCorrection()
		{
			EnableAlphaCorrection(true);
		}

		public static void EnableAlphaCorrection(bool alphaCorrect)
		{
			mAlphaCorrectionEnabled = alphaCorrect;
		}

		public static void SetOrderedHashing()
		{
			SetOrderedHashing(true);
		}

		public static void SetOrderedHashing(bool orderedHash)
		{
			mOrderedHash = orderedHash;
		}

		public char GetMappedChar(char theChar)
		{
			if (mFontData.mCharMap.ContainsKey(theChar))
			{
				return mFontData.mCharMap[theChar];
			}
			if (theChar == '\u00a0')
			{
				theChar = ' ';
			}
			return theChar;
		}

		private ImageFont()
		{
			lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
			{
				GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
			}
			mFontImage = null;
			mScale = 1.0;
			mWantAlphaCorrection = false;
			mFontData = new FontData();
			mFontData.Ref();
		}

		public ImageFont(byte[] buffer)
		{
			mFontImage = null;
			mScale = 1.0;
			mWantAlphaCorrection = false;
			mFontData = new FontData();
			mFontData.Ref();
			if (0 == 0)
			{
				mFontData.Load(buffer);
				mPointSize = mFontData.mDefaultPointSize;
				mActivateAllLayers = false;
				mActiveListValid = true;
				mForceScaledImagesWhite = false;
			}
		}

		public ImageFont(SexyAppBase theSexyApp, string theFontDescFileName, string theImagePathPrefix)
		{
			lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
			{
				GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
			}
			mFontImage = null;
			mScale = 1.0;
			mWantAlphaCorrection = false;
			mFontData = new FontData();
			mFontData.Ref();
			mFontData.mImagePathPrefix = theImagePathPrefix;
			string text = theFontDescFileName + ".cfw2";
			string text2 = "cached\\" + text;
			string theFileName = text2;
			SexyFramework.Misc.Buffer theBuffer = new SexyFramework.Misc.Buffer();
			bool flag = false;
			if (theSexyApp.ReadBufferFromStream(text, ref theBuffer) && theBuffer.GetDataLen() >= 16)
			{
				flag = true;
			}
			else if (theSexyApp.ReadBufferFromStream(text2, ref theBuffer) && theBuffer.GetDataLen() >= 16)
			{
				flag = true;
			}
			else if (theSexyApp.ReadBufferFromStream(theFileName, ref theBuffer) && theBuffer.GetDataLen() >= 16)
			{
				flag = true;
			}
			bool flag2 = false;
			if (flag)
			{
				if (theSexyApp.mResStreamsManager != null && theSexyApp.mResStreamsManager.IsInitialized())
				{
					flag2 = true;
				}
				else
				{
					SerializeRead(theBuffer.GetDataPtr(), theBuffer.GetDataLen() - 16, 16);
					flag2 = true;
				}
			}
			if (!flag2)
			{
				mFontData.Load(theSexyApp, theFontDescFileName);
				mPointSize = mFontData.mDefaultPointSize;
				mActivateAllLayers = false;
				GenerateActiveFontLayers();
				mActiveListValid = true;
				mForceScaledImagesWhite = false;
				bool mWriteFontCacheDir = theSexyApp.mWriteFontCacheDir;
			}
		}

		public ImageFont(Image theFontImage)
		{
			lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
			{
				GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
			}
			mScale = 1.0;
			mWantAlphaCorrection = false;
			mFontData = new FontData();
			mFontData.Ref();
			mFontData.mInitialized = true;
			mPointSize = mFontData.mDefaultPointSize;
			mActiveListValid = false;
			mForceScaledImagesWhite = false;
			mActivateAllLayers = false;
			mFontData.mFontLayerList.AddLast(new FontLayer(mFontData));
			FontLayer value = mFontData.mFontLayerList.Last.Value;
			mFontData.mFontLayerMap.Add("", value);
			mFontImage = (MemoryImage)theFontImage;
			value.mImage.mUnsharedImage = mFontImage;
			value.mDefaultHeight = value.mImage.GetImage().GetHeight();
			value.mAscent = value.mImage.GetImage().GetHeight();
		}

		public ImageFont(ImageFont theImageFont)
			: base(theImageFont)
		{
			mScale = theImageFont.mScale;
			mFontData = theImageFont.mFontData;
			mPointSize = theImageFont.mPointSize;
			mTagVector = theImageFont.mTagVector;
			mActiveListValid = theImageFont.mActiveListValid;
			mForceScaledImagesWhite = theImageFont.mForceScaledImagesWhite;
			mWantAlphaCorrection = theImageFont.mWantAlphaCorrection;
			mActivateAllLayers = theImageFont.mActivateAllLayers;
			mFontImage = theImageFont.mFontImage;
			lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
			{
				GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
			}
			mFontData.Ref();
			if (mActiveListValid)
			{
				mActiveLayerList = theImageFont.mActiveLayerList;
			}
		}

		public override void Dispose()
		{
			lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
			{
				GlobalMembers.gSexyAppBase.mImageFontSet.Remove(this);
			}
			mFontData.DeRef();
			base.Dispose();
		}

		public ImageFont(Image theFontImage, string theFontDescFileName)
		{
			lock (GlobalMembers.gSexyAppBase.mImageSetCritSect)
			{
				GlobalMembers.gSexyAppBase.mImageFontSet.Add(this);
			}
			mScale = 1.0;
			mFontImage = null;
			mFontData = new FontData();
			mFontData.Ref();
			mFontData.LoadLegacy(theFontImage, theFontDescFileName);
			mPointSize = mFontData.mDefaultPointSize;
			mActivateAllLayers = false;
			GenerateActiveFontLayers();
			mActiveListValid = true;
		}

		public override ImageFont AsImageFont()
		{
			return this;
		}

		public override int CharWidth(char theChar)
		{
			return CharWidthKern(theChar, '\0');
		}

		public override int CharWidthKern(char theChar, char thePrevChar)
		{
			Prepare();
			int num = 0;
			double num2 = (double)mPointSize * mScale;
			theChar = GetMappedChar(theChar);
			if (thePrevChar != 0)
			{
				thePrevChar = GetMappedChar(thePrevChar);
			}
			for (int i = 0; i < mActiveLayerList.Length; i++)
			{
				ActiveFontLayer activeFontLayer = mActiveLayerList[i];
				int num3 = 0;
				int num4 = activeFontLayer.mBaseFontLayer.mPointSize;
				int num5;
				int num6;
				if (num4 == 0)
				{
					num5 = (((double)activeFontLayer.mBaseFontLayer.GetCharData(theChar).mWidth * mScale >= 0.0) ? ((int)((double)activeFontLayer.mBaseFontLayer.GetCharData(theChar).mWidth * mScale + 0.501)) : ((int)((double)activeFontLayer.mBaseFontLayer.GetCharData(theChar).mWidth * mScale - 0.501)));
					if (thePrevChar != 0)
					{
						num6 = activeFontLayer.mBaseFontLayer.mSpacing;
						CharData charData = activeFontLayer.mBaseFontLayer.GetCharData(thePrevChar);
						if (charData.mKerningCount != 0)
						{
							int mKerningCount = charData.mKerningCount;
							for (int j = 0; j < mKerningCount; j++)
							{
								if (activeFontLayer.mBaseFontLayer.mKerningData[j + charData.mKerningFirst].mChar == theChar)
								{
									num6 += activeFontLayer.mBaseFontLayer.mKerningData[j + charData.mKerningFirst].mOffset * (int)mScale;
								}
							}
						}
					}
					else
					{
						num6 = 0;
					}
				}
				else
				{
					num5 = (((double)activeFontLayer.mBaseFontLayer.GetCharData(theChar).mWidth * num2 / (double)(float)num4 >= 0.0) ? ((int)((double)activeFontLayer.mBaseFontLayer.GetCharData(theChar).mWidth * num2 / (double)(float)num4 + 0.501)) : ((int)((double)activeFontLayer.mBaseFontLayer.GetCharData(theChar).mWidth * num2 / (double)(float)num4 - 0.501)));
					if (thePrevChar != 0)
					{
						num6 = ((thePrevChar != ' ') ? activeFontLayer.mBaseFontLayer.mSpacing : 0);
						CharData charData2 = activeFontLayer.mBaseFontLayer.GetCharData(thePrevChar);
						if (charData2.mKerningCount != 0)
						{
							int mKerningCount2 = charData2.mKerningCount;
							for (int k = 0; k < mKerningCount2; k++)
							{
								if (activeFontLayer.mBaseFontLayer.mKerningData[k + charData2.mKerningFirst].mChar == theChar)
								{
									num6 += activeFontLayer.mBaseFontLayer.mKerningData[k + charData2.mKerningFirst].mOffset * (int)num2 / num4;
								}
							}
						}
					}
					else
					{
						num6 = 0;
					}
				}
				int num7 = num5 + num6;
				if (num7 <= 0 && theChar == ' ')
				{
					num7 = num5;
				}
				num3 += num7;
				if (num3 > num)
				{
					num = num3;
				}
			}
			return num;
		}

		public override int StringWidth(string theString)
		{
			int num = 0;
			char thePrevChar = '\0';
			foreach (char c in theString)
			{
				num += CharWidthKern(c, thePrevChar);
				thePrevChar = c;
			}
			return num;
		}

		public override void DrawString(Graphics g, int theX, int theY, string theString, Color theColor, Rect theClipRect)
		{
			int theWidth = 0;
			DrawStringEx(g, theX, theY, theString, theColor, theClipRect, null, ref theWidth);
		}

		public override Font Duplicate()
		{
			return new ImageFont(this);
		}

		public virtual void SetPointSize(int thePointSize)
		{
			mPointSize = thePointSize;
			mActiveListValid = false;
		}

		public virtual int GetPointSize()
		{
			return mPointSize;
		}

		public virtual void SetScale(double theScale)
		{
			mScale = theScale;
			mActiveListValid = false;
		}

		public virtual int GetDefaultPointSize()
		{
			return mFontData.mDefaultPointSize;
		}

		public virtual bool AddTag(string theTagName)
		{
			if (HasTag(theTagName))
			{
				return false;
			}
			string item = theTagName.ToUpper();
			mTagVector.Add(item);
			mActiveListValid = false;
			return true;
		}

		public virtual bool RemoveTag(string theTagName)
		{
			string item = theTagName.ToUpper();
			if (mTagVector.Remove(item))
			{
				mActiveListValid = false;
				return true;
			}
			return false;
		}

		public virtual bool HasTag(string theTagName)
		{
			return mTagVector.Contains(theTagName);
		}

		public virtual string GetDefine(string theName)
		{
			DataElement dataElement = mFontData.Dereference(theName);
			if (dataElement == null)
			{
				return "";
			}
			return mFontData.DataElementToString(dataElement, true);
		}

		public virtual void Prepare()
		{
			if (!mActiveListValid)
			{
				GenerateActiveFontLayers();
				mActiveListValid = true;
			}
		}

		public static bool CheckCache(string theSrcFile, string theAltData)
		{
			return false;
		}

		public static bool SetCacheUpToDate(string theSrcFile, string theAltData)
		{
			return false;
		}

		public static ImageFont ReadFromCache(string theSrcFile, string theAltData)
		{
			return null;
		}

		public virtual void WriteToCache(string theSrcFile, string theAltData)
		{
		}

		public string SerializeReadStr(byte[] thePtr, int theStartIndex, int size)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < size; i++)
			{
				stringBuilder.Append((char)thePtr[theStartIndex + i]);
			}
			return stringBuilder.ToString();
		}

		public bool SerializeRead(byte[] thePtr, int theSize, int theStartIndex)
		{
			if (thePtr == null)
			{
				return false;
			}
			bool result = false;
			int num = theStartIndex;
			mAscent = BitConverter.ToInt32(thePtr, num);
			num += 4;
			mAscentPadding = BitConverter.ToInt32(thePtr, num);
			num += 4;
			mHeight = BitConverter.ToInt32(thePtr, num);
			num += 4;
			mLineSpacingOffset = BitConverter.ToInt32(thePtr, num);
			num += 4;
			mFontData.mApp = GlobalMembers.gSexyAppBase;
			mFontData.mInitialized = BitConverter.ToBoolean(thePtr, num);
			num++;
			mFontData.mDefaultPointSize = BitConverter.ToInt32(thePtr, num);
			num += 4;
			int num2 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			for (int i = 0; i < num2; i++)
			{
				ushort key = BitConverter.ToUInt16(thePtr, num);
				num += 2;
				ushort value = BitConverter.ToUInt16(thePtr, num);
				num += 2;
				mFontData.mCharMap.Add((char)key, (char)value);
			}
			int num3 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			for (int j = 0; j < num3; j++)
			{
				mFontData.mFontLayerList.AddLast(new FontLayer(mFontData));
				FontLayer value2 = mFontData.mFontLayerList.Last.Value;
				int num4 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mLayerName = SerializeReadStr(thePtr, num, num4);
				num += num4;
				mFontData.mFontLayerMap.Add(value2.mLayerName, value2);
				int num5 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				for (int k = 0; k < num5; k++)
				{
					int num6 = BitConverter.ToInt32(thePtr, num);
					num += 4;
					string item = SerializeReadStr(thePtr, num, num6);
					num += num6;
					value2.mRequiredTags.Add(item);
				}
				num5 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				for (int l = 0; l < num5; l++)
				{
					int num7 = BitConverter.ToInt32(thePtr, num);
					num += 4;
					string item2 = SerializeReadStr(thePtr, num, num7);
					num += num7;
					value2.mExcludedTags.Add(item2);
				}
				int num8 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				if (num8 != 0)
				{
					List<FontLayer.KerningValue> list = new List<FontLayer.KerningValue>();
					for (int m = 0; m < num8; m++)
					{
						FontLayer.KerningValue item3 = default(FontLayer.KerningValue);
						item3.mInt = BitConverter.ToInt32(thePtr, num);
						num += 4;
						item3.mChar = (ushort)((item3.mInt >> 16) & 0xFFFF);
						item3.mOffset = (short)(item3.mInt & 0xFFFF);
						list.Add(item3);
					}
					value2.mKerningData = list.ToArray();
				}
				int num9 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				for (int n = 0; n < num9; n++)
				{
					ushort inChar = BitConverter.ToUInt16(thePtr, num);
					num += 2;
					CharData charData = value2.mCharDataHashTable.GetCharData((char)inChar, true);
					charData.mImageRect.mX = BitConverter.ToInt32(thePtr, num);
					num += 4;
					charData.mImageRect.mY = BitConverter.ToInt32(thePtr, num);
					num += 4;
					charData.mImageRect.mWidth = BitConverter.ToInt32(thePtr, num);
					num += 4;
					charData.mImageRect.mHeight = BitConverter.ToInt32(thePtr, num);
					num += 4;
					charData.mOffset.mX = BitConverter.ToInt32(thePtr, num);
					num += 4;
					charData.mOffset.mY = BitConverter.ToInt32(thePtr, num);
					num += 4;
					charData.mKerningFirst = BitConverter.ToUInt16(thePtr, num);
					num += 2;
					charData.mKerningCount = BitConverter.ToUInt16(thePtr, num);
					num += 2;
					charData.mWidth = BitConverter.ToInt32(thePtr, num);
					num += 4;
					charData.mOrder = BitConverter.ToInt32(thePtr, num);
					num += 4;
				}
				value2.mColorMult.mRed = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mColorMult.mGreen = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mColorMult.mBlue = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mColorMult.mAlpha = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mColorAdd.mRed = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mColorAdd.mGreen = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mColorAdd.mBlue = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mColorAdd.mAlpha = BitConverter.ToInt32(thePtr, num);
				num += 4;
				int num10 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mImageFileName = SerializeReadStr(thePtr, num, num10);
				num += num10;
				bool flag = false;
				SharedImageRef sharedImageRef = new SharedImageRef();
				if (GlobalMembers.gSexyAppBase.mResourceManager != null && string.IsNullOrEmpty(mFontData.mImagePathPrefix))
				{
					string idByPath = GlobalMembers.gSexyAppBase.mResourceManager.GetIdByPath(value2.mImageFileName);
					if (!string.IsNullOrEmpty(idByPath))
					{
						sharedImageRef = GlobalMembers.gSexyAppBase.mResourceManager.GetImage(idByPath);
						if (sharedImageRef.GetDeviceImage() == null)
						{
							sharedImageRef = GlobalMembers.gSexyAppBase.mResourceManager.LoadImage(idByPath);
						}
						if (sharedImageRef.GetDeviceImage() != null)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					sharedImageRef = GlobalMembers.gSexyAppBase.GetSharedImage(mFontData.mImagePathPrefix + value2.mImageFileName);
				}
				value2.mImage = new SharedImageRef(sharedImageRef);
				if (value2.mImage.GetDeviceImage() == null)
				{
					result = true;
				}
				value2.mDrawMode = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mOffset.mX = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mOffset.mY = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mSpacing = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mMinPointSize = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mMaxPointSize = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mPointSize = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mAscent = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mAscentPadding = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mHeight = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mDefaultHeight = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mLineSpacingOffset = BitConverter.ToInt32(thePtr, num);
				num += 4;
				value2.mBaseOrder = BitConverter.ToInt32(thePtr, num);
				num += 4;
			}
			int num11 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			mFontData.mSourceFile = SerializeReadStr(thePtr, num, num11);
			num += num11;
			int num12 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			mFontData.mFontErrorHeader = SerializeReadStr(thePtr, num, num12);
			num += num12;
			mPointSize = BitConverter.ToInt32(thePtr, num);
			num += 4;
			int num13 = BitConverter.ToInt32(thePtr, num);
			num += 4;
			for (int num14 = 0; num14 < num13; num14++)
			{
				int num15 = BitConverter.ToInt32(thePtr, num);
				num += 4;
				string item4 = SerializeReadStr(thePtr, num, num15);
				num += num15;
				mTagVector.Add(item4);
			}
			mScale = BitConverter.ToDouble(thePtr, num);
			num += 8;
			mForceScaledImagesWhite = BitConverter.ToBoolean(thePtr, num);
			num++;
			mActivateAllLayers = BitConverter.ToBoolean(thePtr, num);
			num++;
			mActiveListValid = false;
			return result;
		}

		public bool SerializeReadEndian(IntPtr thePtr, int theSize)
		{
			return false;
		}

		public bool SerializeWrite(IntPtr thePtr)
		{
			return SerializeWrite(thePtr, 0);
		}

		public bool SerializeWrite(IntPtr thePtr, int theSizeIfKnown)
		{
			return false;
		}

		public int GetLayerCount()
		{
			LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				FontLayer current = enumerator.Current;
				if (current.mLayerName.Length < 6 || !current.mLayerName.EndsWith("__MOD"))
				{
					num++;
				}
			}
			return num;
		}

		public void PushLayerColor(string theLayerName, Color theColor)
		{
			Prepare();
			for (int i = 0; i < mActiveLayerList.Length; i++)
			{
				ActiveFontLayer activeFontLayer = mActiveLayerList[i];
				string mLayerName = activeFontLayer.mBaseFontLayer.mLayerName;
				if (string.Compare(activeFontLayer.mBaseFontLayer.mLayerName, theLayerName, StringComparison.OrdinalIgnoreCase) == 0 || (mLayerName.StartsWith(theLayerName, StringComparison.OrdinalIgnoreCase) && mLayerName.EndsWith("__MOD") && mLayerName.Length == theLayerName.Length + 5))
				{
					activeFontLayer.PushColor(theColor);
				}
			}
		}

		public void PushLayerColor(int theLayer, Color theColor)
		{
			Prepare();
			LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				FontLayer current = enumerator.Current;
				if (current.mLayerName.Length < 6 || !current.mLayerName.EndsWith("__MOD"))
				{
					if (num == theLayer)
					{
						PushLayerColor(current.mLayerName, theColor);
						break;
					}
					num++;
				}
			}
		}

		public void PopLayerColor(string theLayerName)
		{
			for (int i = 0; i < mActiveLayerList.Length; i++)
			{
				ActiveFontLayer activeFontLayer = mActiveLayerList[i];
				string mLayerName = activeFontLayer.mBaseFontLayer.mLayerName;
				if (string.Compare(mLayerName, theLayerName, StringComparison.OrdinalIgnoreCase) == 0 || (mLayerName.StartsWith(theLayerName, StringComparison.OrdinalIgnoreCase) && mLayerName.EndsWith("__MOD") && mLayerName.Length == theLayerName.Length + 5))
				{
					activeFontLayer.PopColor();
				}
			}
		}

		public void PopLayerColor(int theLayer)
		{
			LinkedList<FontLayer>.Enumerator enumerator = mFontData.mFontLayerList.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				FontLayer current = enumerator.Current;
				if (current.mLayerName.Length < 6 || !current.mLayerName.EndsWith("__MOD"))
				{
					if (num == theLayer)
					{
						PopLayerColor(current.mLayerName);
						break;
					}
					num++;
				}
			}
		}
	}
}
