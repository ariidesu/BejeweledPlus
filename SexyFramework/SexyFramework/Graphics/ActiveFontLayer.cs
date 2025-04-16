using System;
using System.Collections.Generic;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class ActiveFontLayer
	{
		public FontLayer mBaseFontLayer;

		public SharedImageRef[] mScaledImages = new SharedImageRef[8];

		public bool mUseAlphaCorrection;

		public bool mOwnsImage;

		public Dictionary<char, Rect> mScaledCharImageRects = new Dictionary<char, Rect>();

		public List<Color> mColorStack = new List<Color>();

		public ActiveFontLayer()
		{
		}

		public ActiveFontLayer(ActiveFontLayer theActiveFontLayer)
		{
			mBaseFontLayer = theActiveFontLayer.mBaseFontLayer;
			mUseAlphaCorrection = theActiveFontLayer.mUseAlphaCorrection;
			mScaledCharImageRects = theActiveFontLayer.mScaledCharImageRects;
			mColorStack = theActiveFontLayer.mColorStack;
			for (int i = 0; i < 8; i++)
			{
				mScaledImages[i] = theActiveFontLayer.mScaledImages[i];
			}
		}

		public virtual void Dispose()
		{
		}

		public SharedImageRef GenerateAlphaCorrectedImage(int thePalette)
		{
			bool isNew = false;
			mScaledImages[thePalette] = GlobalMembers.gSexyAppBase.GetSharedImage("!" + mScaledImages[7].GetMemoryImage().mFilePath, $"AltFontImage{thePalette}", ref isNew, true, false);
			mScaledImages[thePalette].GetMemoryImage().Create(mScaledImages[7].mWidth, mScaledImages[7].mHeight);
			int num = mScaledImages[7].mWidth * mScaledImages[7].mHeight;
			mScaledImages[thePalette].GetMemoryImage().mColorTable = new uint[256];
			mScaledImages[thePalette].GetMemoryImage().mColorIndices = new byte[num];
			if (mScaledImages[7].GetMemoryImage().mColorTable != null)
			{
				Array.Copy(mScaledImages[thePalette].GetMemoryImage().mColorIndices, mScaledImages[7].GetMemoryImage().mColorIndices, num);
			}
			else
			{
				uint[] bits = mScaledImages[7].GetMemoryImage().GetBits();
				for (int i = 0; i < num; i++)
				{
					mScaledImages[thePalette].GetMemoryImage().mColorIndices[i] = (byte)(bits[i] >> 24);
				}
			}
			Array.Copy(mScaledImages[thePalette].GetMemoryImage().mColorTable, GlobalMembersImageFont.FONT_PALETTES[thePalette], 1024);
			return mScaledImages[thePalette];
		}

		public void PushColor(Color theColor)
		{
			if (mColorStack.Count == 0)
			{
				mColorStack.Add(theColor);
				return;
			}
			Color color = mColorStack[mColorStack.Count - 1];
			Color item = new Color(theColor.mRed * color.mRed / 255, theColor.mGreen * color.mGreen / 255, theColor.mBlue * color.mBlue / 255, theColor.mAlpha * color.mAlpha / 255);
			mColorStack.Add(item);
		}

		public void PopColor()
		{
			if (mColorStack.Count != 0)
			{
				mColorStack.RemoveAt(mColorStack.Count - 1);
			}
		}
	}
}
