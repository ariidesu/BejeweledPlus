using System;
using System.Collections.Generic;
using System.Text;
using SexyFramework.Drivers.Graphics;
using SexyFramework.Misc;

namespace SexyFramework.Graphics
{
	public class Graphics : GraphicsState
	{
		public enum DrawMode
		{
			Normal,
			Additive
		}

		protected RenderDevice mRenderDevice;

		protected HRenderContext mRenderContext = new HRenderContext();

		protected Graphics3D mGraphics3D;

		public Edge[] mPFActiveEdgeList;

		public int mPFNumActiveEdges;

		public new Point[] mPFPoints;

		public int mPFNumVertices;

		public Stack<GraphicsState> mStateStack = new Stack<GraphicsState>();

		protected StringBuilder mStringBuilder = new StringBuilder("");

		private static SexyVertex2D[,] DTTS_aList = new SexyVertex2D[100, 3];

		protected static bool PFCompareInd(IntPtr u, IntPtr v)
		{
			return false;
		}

		protected static bool PFCompareActive(IntPtr u, IntPtr v)
		{
			return false;
		}

		protected void PFDelete(int i)
		{
		}

		protected void PFInsert(int i, int y)
		{
		}

		protected void DrawImageTransformHelper(Image theImage, Transform theTransform, Rect theSrcRect, float x, float y, bool useFloat)
		{
			if (theTransform.mComplex || (Get3D() != null && useFloat))
			{
				DrawImageMatrix(theImage, theTransform.GetMatrix(), theSrcRect, x, y);
				return;
			}
			float num = (float)theSrcRect.mWidth / 2f;
			float num2 = (float)theSrcRect.mHeight / 2f;
			if (theTransform.mHaveRot)
			{
				float num3 = num - theTransform.mTransX1;
				float num4 = num2 - theTransform.mTransY1;
				x = x + theTransform.mTransX2 - num3 + 0.5f;
				y = y + theTransform.mTransY2 - num4 + 0.5f;
				if (useFloat)
				{
					DrawImageRotatedF(theImage, x, y, theTransform.mRot, num3, num4, theSrcRect);
				}
				else
				{
					DrawImageRotated(theImage, (int)x, (int)y, theTransform.mRot, (int)num3, (int)num4, theSrcRect);
				}
			}
			else if (theTransform.mHaveScale)
			{
				bool mirror = false;
				if (theTransform.mScaleX == -1f)
				{
					if (theTransform.mScaleY == 1f)
					{
						x = x + theTransform.mTransX1 + theTransform.mTransX2 - num + 0.5f;
						y = y + theTransform.mTransY1 + theTransform.mTransY2 - num2 + 0.5f;
						DrawImageMirror(theImage, (int)x, (int)y, theSrcRect);
						return;
					}
					mirror = true;
				}
				float num5 = num * theTransform.mScaleX;
				float num6 = num2 * theTransform.mScaleY;
				x = x + theTransform.mTransX2 - num5;
				y = y + theTransform.mTransY2 - num6;
				mDestRect.mX = (int)x;
				mDestRect.mY = (int)y;
				mDestRect.mWidth = (int)(num5 * 2f);
				mDestRect.mHeight = (int)(num6 * 2f);
				DrawImageMirror(theImage, mDestRect, theSrcRect, mirror);
			}
			else
			{
				x = x + theTransform.mTransX1 + theTransform.mTransX2 - num + 0.5f;
				y = y + theTransform.mTransY1 + theTransform.mTransY2 - num2 + 0.5f;
				if (useFloat)
				{
					DrawImageF(theImage, x, y, theSrcRect);
				}
				else
				{
					DrawImage(theImage, (int)x, (int)y, theSrcRect);
				}
			}
		}

		protected void InitRenderInfo(Graphics theSourceGraphics)
		{
			mGraphics3D = null;
			mIs3D = false;
			RenderDevice3D renderDevice3D = GlobalMembers.gSexyAppBase.mGraphicsDriver.GetRenderDevice3D();
			if (renderDevice3D != null)
			{
				HRenderContext hRenderContext = new HRenderContext();
				hRenderContext = ((theSourceGraphics == null) ? renderDevice3D.CreateContext(mDestImage) : renderDevice3D.CreateContext(mDestImage, theSourceGraphics.mRenderContext));
				mRenderDevice = renderDevice3D;
				mRenderContext = hRenderContext;
				mGraphics3D = new Graphics3D(this, renderDevice3D, mRenderContext);
				mIs3D = true;
			}
			else
			{
				if (mRenderContext.IsValid())
				{
					return;
				}
				RenderDevice renderDevice = GlobalMembers.gSexyAppBase.mGraphicsDriver.GetRenderDevice();
				if (renderDevice != null)
				{
					HRenderContext hRenderContext2 = new HRenderContext();
					hRenderContext2 = ((theSourceGraphics == null) ? renderDevice.CreateContext(mDestImage) : renderDevice.CreateContext(mDestImage, theSourceGraphics.mRenderContext));
					if (hRenderContext2.IsValid())
					{
						mRenderDevice = renderDevice;
						mRenderContext = hRenderContext2;
						mGraphics3D = null;
						mIs3D = false;
					}
				}
			}
		}

		protected void SetAsCurrentContext()
		{
			if (mRenderDevice != null)
			{
				mRenderDevice.SetCurrentContext(mRenderContext);
			}
		}

		protected void CalcFinalColor()
		{
			if (mPushedColorVector.Count > 0)
			{
				Color color = mPushedColorVector[mPushedColorVector.Count - 1];
				mFinalColor = new Color((int)Math.Min(255f, (float)(color.mRed * mColor.mRed) / 255f), (int)Math.Min(255f, (float)(color.mGreen * mColor.mGreen) / 255f), (int)Math.Min(255f, (float)(color.mBlue * mColor.mBlue) / 255f), (int)Math.Min(255f, (float)(color.mAlpha * mColor.mAlpha) / 255f));
			}
			else
			{
				mFinalColor = mColor;
			}
		}

		protected Color GetImageColor()
		{
			if (mPushedColorVector.Count > 0)
			{
				if (mColorizeImages)
				{
					return mFinalColor;
				}
				return mPushedColorVector[mPushedColorVector.Count - 1];
			}
			if (mColorizeImages)
			{
				return mColor;
			}
			return new Color(Color.White);
		}

		protected bool DrawLineClipHelper(ref double theStartX, ref double theStartY, ref double theEndX, ref double theEndY)
		{
			double a = theStartX;
			double a2 = theStartY;
			double b = theEndX;
			double b2 = theEndY;
			if (a > b)
			{
				Swap(ref a, ref b);
				Swap(ref a2, ref b2);
			}
			if (a < (double)mClipRect.mX)
			{
				if (b < (double)mClipRect.mX)
				{
					return false;
				}
				double num = (b2 - a2) / (b - a);
				a2 += ((double)mClipRect.mX - a) * num;
				a = mClipRect.mX;
			}
			if (b >= (double)(mClipRect.mX + mClipRect.mWidth))
			{
				if (a >= (double)(mClipRect.mX + mClipRect.mWidth))
				{
					return false;
				}
				double num2 = (b2 - a2) / (b - a);
				b2 += ((double)(mClipRect.mX + mClipRect.mWidth - 1) - b) * num2;
				b = mClipRect.mX + mClipRect.mWidth - 1;
			}
			if (a2 > b2)
			{
				Swap(ref a, ref b);
				Swap(ref a2, ref b2);
			}
			if (a2 < (double)mClipRect.mY)
			{
				if (b2 < (double)mClipRect.mY)
				{
					return false;
				}
				double num3 = (b - a) / (b2 - a2);
				a += ((double)mClipRect.mY - a2) * num3;
				a2 = mClipRect.mY;
			}
			if (b2 >= (double)(mClipRect.mY + mClipRect.mHeight))
			{
				if (a2 >= (double)(mClipRect.mY + mClipRect.mHeight))
				{
					return false;
				}
				double num4 = (b - a) / (b2 - a2);
				b += ((double)(mClipRect.mY + mClipRect.mHeight - 1) - b2) * num4;
				b2 = mClipRect.mY + mClipRect.mHeight - 1;
			}
			theStartX = a;
			theStartY = a2;
			theEndX = b;
			theEndY = b2;
			return true;
		}

		protected void Swap<T>(ref T a, ref T b)
		{
			T val = a;
			a = b;
			b = val;
		}

		public Graphics()
		{
			mTransX = 0f;
			mTransY = 0f;
			mScaleX = 1f;
			mScaleY = 1f;
			mScaleOrigX = 0f;
			mScaleOrigY = 0f;
			mDestImage = null;
			mDrawMode = 0;
			mColorizeImages = false;
			mFastStretch = false;
			mWriteColoredString = true;
			mLinearBlend = false;
			mClipRect = new Rect(0, 0, GlobalMembers.gSexyApp.mGraphicsDriver.GetScreenWidth(), GlobalMembers.gSexyApp.mGraphicsDriver.GetScreenHeight());
			InitRenderInfo(null);
		}

		public Graphics(Image theDestImage)
		{
			mTransX = 0f;
			mTransY = 0f;
			mScaleX = 1f;
			mScaleY = 1f;
			mScaleOrigX = 0f;
			mScaleOrigY = 0f;
			mDestImage = theDestImage;
			mDrawMode = 0;
			mColorizeImages = false;
			mFastStretch = false;
			mWriteColoredString = true;
			mLinearBlend = false;
			if (mDestImage == null)
			{
				mClipRect = new Rect(0, 0, GlobalMembers.gSexyApp.mGraphicsDriver.GetScreenWidth(), GlobalMembers.gSexyApp.mGraphicsDriver.GetScreenHeight());
			}
			else
			{
				mClipRect = new Rect(0, 0, mDestImage.GetWidth(), mDestImage.GetHeight());
			}
			InitRenderInfo(null);
		}

		public Graphics(Graphics theGraphics)
		{
			CopyStateFrom(theGraphics);
			InitRenderInfo(theGraphics);
		}

		public virtual void Dispose()
		{
			mRenderDevice.DeleteContext(mRenderContext);
		}

		public void ClearRenderContext()
		{
			XNAGraphicsDriver xNAGraphicsDriver = (XNAGraphicsDriver)GlobalMembers.gSexyAppBase.mGraphicsDriver;
			xNAGraphicsDriver.mXNARenderDevice.SetCurrentContext(null);
		}

		public Graphics3D Get3D()
		{
			return mGraphics3D;
		}

		public RenderDevice GetRenderDevice()
		{
			return mRenderDevice;
		}

		public HRenderContext GetRenderContext()
		{
			return mRenderContext;
		}

		public void PushState()
		{
			GraphicsState graphicsState = GraphicsStatePool.CreateState();
			graphicsState.CopyStateFrom(this);
			mStateStack.Push(graphicsState);
			if (mRenderDevice != null)
			{
				SetAsCurrentContext();
				mRenderDevice.PushState();
			}
		}

		public void PopState()
		{
			if (mStateStack.Count > 0)
			{
				CopyStateFrom(mStateStack.Peek());
				GraphicsStatePool.ReleaseState(mStateStack.Pop());
			}
			if (mRenderDevice != null)
			{
				SetAsCurrentContext();
				mRenderDevice.PopState();
			}
		}

		public void SetFont(Font theFont)
		{
			mFont = theFont;
		}

		public Font GetFont()
		{
			return mFont;
		}

		public void SetColor(Color theColor)
		{
			mColor.mRed = theColor.mRed;
			mColor.mGreen = theColor.mGreen;
			mColor.mBlue = theColor.mBlue;
			mColor.mAlpha = theColor.mAlpha;
			CalcFinalColor();
		}

		public void SetColor(int red, int green, int blue, int alpha)
		{
			mColor.mRed = red;
			mColor.mGreen = green;
			mColor.mBlue = blue;
			mColor.mAlpha = alpha;
			CalcFinalColor();
		}

		public void SetColor(int red, int green, int blue)
		{
			mColor.mRed = red;
			mColor.mGreen = green;
			mColor.mBlue = blue;
			mColor.mAlpha = 255;
			CalcFinalColor();
		}

		public Color GetColor()
		{
			return mColor;
		}

		public void PushColorMult()
		{
			mPushedColorVector.Add(mFinalColor);
			CalcFinalColor();
		}

		public void PopColorMult()
		{
			mPushedColorVector.RemoveAt(mPushedColorVector.Count - 1);
			CalcFinalColor();
		}

		public Color GetFinalColor()
		{
			if (mPushedColorVector.Count > 0)
			{
				return mFinalColor;
			}
			return mColor;
		}

		public void SetDrawMode(int theDrawMode)
		{
			mDrawMode = theDrawMode;
		}

		public void SetDrawMode(DrawMode theDrawMode)
		{
			SetDrawMode((int)theDrawMode);
		}

		public int GetDrawMode()
		{
			return mDrawMode;
		}

		public void SetColorizeImages(bool colorizeImages)
		{
			mColorizeImages = colorizeImages;
		}

		public bool GetColorizeImages()
		{
			return mColorizeImages;
		}

		public void SetFastStretch(bool fastStretch)
		{
			mFastStretch = fastStretch;
		}

		public bool GetFastStretch()
		{
			return mFastStretch;
		}

		public void SetLinearBlend(bool linear)
		{
			mLinearBlend = linear;
		}

		public bool GetLinearBlend()
		{
			return mLinearBlend;
		}

		public void FillRect(int theX, int theY, int theWidth, int theHeight)
		{
			Color finalColor = GetFinalColor();
			if (finalColor.mAlpha != 0)
			{
				SetAsCurrentContext();
				if (mRenderDevice != null)
				{
					mDestRect.mX = theX + (int)mTransX;
					mDestRect.mY = theY + (int)mTransY;
					mDestRect.mWidth = theWidth;
					mDestRect.mHeight = theHeight;
					Rect theRect = mDestRect.Intersection(mClipRect);
					mRenderDevice.FillRect(theRect, finalColor, mDrawMode);
				}
			}
		}

		public void FillRect(Rect theRect)
		{
			FillRect(theRect.mX, theRect.mY, theRect.mWidth, theRect.mHeight);
		}

		public void DrawRect(int theX, int theY, int theWidth, int theHeight)
		{
			Color finalColor = GetFinalColor();
			if (finalColor.mAlpha != 0)
			{
				Rect theRect = new Rect(theX + (int)mTransX, theY + (int)mTransY, theWidth, theHeight);
				Rect rect = new Rect(theX + (int)mTransX, theY + (int)mTransY, theWidth + 1, theHeight + 1);
				Rect rect2 = rect.Intersection(mClipRect);
				if (rect.Equals(rect2))
				{
					SetAsCurrentContext();
					mRenderDevice.DrawRect(theRect, finalColor, mDrawMode);
					return;
				}
				FillRect(theX, theY, theWidth + 1, 1);
				FillRect(theX, theY + theHeight, theWidth + 1, 1);
				FillRect(theX, theY + 1, 1, theHeight - 1);
				FillRect(theX + theWidth, theY + 1, 1, theHeight - 1);
			}
		}

		public void DrawRect(Rect theRect)
		{
			DrawRect(theRect.mX, theRect.mY, theRect.mWidth, theRect.mHeight);
		}

		public void ClearRect(int theX, int theY, int theWidth, int theHeight)
		{
			SetAsCurrentContext();
			mDestRect.mX = theX + (int)mTransX;
			mDestRect.mY = theY + (int)mTransY;
			mDestRect.mWidth = theWidth;
			mDestRect.mHeight = theHeight;
			Rect theRect = mDestRect.Intersection(mClipRect);
			mRenderDevice.ClearRect(theRect);
		}

		public void ClearRect(Rect theRect)
		{
			ClearRect(theRect.mX, theRect.mY, theRect.mWidth, theRect.mHeight);
		}

		public void DrawString(string theString, int theX, int theY)
		{
			if (mFont != null)
			{
				mFont.DrawString(this, theX, theY, theString, GetFinalColor(), mClipRect);
			}
		}

		public void DrawLine(int theStartX, int theStartY, int theEndX, int theEndY)
		{
			double theStartX2 = (float)theStartX + mTransX;
			double theStartY2 = (float)theStartY + mTransY;
			double theEndX2 = (float)theEndX + mTransX;
			double theEndY2 = (float)theEndY + mTransY;
			if (DrawLineClipHelper(ref theStartX2, ref theStartY2, ref theEndX2, ref theEndY2))
			{
				SetAsCurrentContext();
				mRenderDevice.DrawLine(theStartX2, theStartY2, theEndX2, theEndY2, GetFinalColor(), mDrawMode);
			}
		}

		public void DrawLineAA(int theStartX, int theStartY, int theEndX, int theEndY)
		{
			double theStartX2 = (float)theStartX + mTransX;
			double theStartY2 = (float)theStartY + mTransY;
			double theEndX2 = (float)theEndX + mTransX;
			double theEndY2 = (float)theEndY + mTransY;
			if (DrawLineClipHelper(ref theStartX2, ref theStartY2, ref theEndX2, ref theEndY2))
			{
				SetAsCurrentContext();
				mRenderDevice.DrawLine(theStartX2, theStartY2, theEndX2, theEndY2, GetFinalColor(), mDrawMode, true);
			}
		}

		public void PolyFill(Point[] theVertexList, int theNumVertices, bool convex)
		{
			SetAsCurrentContext();
			if (convex && mRenderDevice.CanFillPoly())
			{
				mRenderDevice.FillPoly(theVertexList, theNumVertices, mClipRect, GetFinalColor(), mDrawMode, (int)mTransX, (int)mTransY);
				return;
			}
			throw new NotSupportedException();
		}

		public void PolyFillAA(Point[] theVertexList, int theNumVertices)
		{
			PolyFillAA(theVertexList, theNumVertices, false);
		}

		public void PolyFillAA(Point[] theVertexList, int theNumVertices, bool convex)
		{
			SetAsCurrentContext();
			if (convex && mRenderDevice.CanFillPoly())
			{
				mRenderDevice.FillPoly(theVertexList, theNumVertices, mClipRect, GetFinalColor(), mDrawMode, (int)mTransX, (int)mTransY);
				return;
			}
			throw new NotSupportedException();
		}

		public void DrawImage(Image theImage, int theX, int theY)
		{
			if (mScaleX != 1f || mScaleY != 1f)
			{
				DrawImage(theImage, theX, theY, theImage.GetRect());
				return;
			}
			theX += (int)mTransX;
			theY += (int)mTransY;
			mDestRect.mX = theX;
			mDestRect.mY = theY;
			mDestRect.mWidth = theImage.GetWidth();
			mDestRect.mHeight = theImage.GetHeight();
			Rect rect = mDestRect.Intersection(mClipRect);
			mSrcRect.mX = rect.mX - theX;
			mSrcRect.mY = rect.mY - theY;
			mSrcRect.mWidth = rect.mWidth;
			mSrcRect.mHeight = rect.mHeight;
			if (mSrcRect.mWidth > 0 && mSrcRect.mHeight > 0)
			{
				SetAsCurrentContext();
				mRenderDevice.Blt(theImage, rect.mX, rect.mY, mSrcRect, GetImageColor(), mDrawMode);
			}
		}

		public void DrawImage(Image theImage, int theX, int theY, Rect theSrcRect)
		{
			if (theSrcRect.mX + theSrcRect.mWidth > theImage.GetWidth() || theSrcRect.mY + theSrcRect.mHeight > theImage.GetHeight())
			{
				return;
			}
			theX += (int)mTransX;
			theY += (int)mTransY;
			if (mScaleX != 1f || mScaleY != 1f)
			{
				Rect theDestRect = new Rect((int)((double)mScaleOrigX + Math.Floor(((float)theX - mScaleOrigX) * mScaleX)), (int)((double)mScaleOrigY + Math.Floor(((float)theY - mScaleOrigY) * mScaleY)), (int)Math.Ceiling((float)theSrcRect.mWidth * mScaleX), (int)Math.Ceiling((float)theSrcRect.mHeight * mScaleY));
				SetAsCurrentContext();
				mRenderDevice.BltStretched(theImage, theDestRect, theSrcRect, mClipRect, GetImageColor(), mDrawMode, mFastStretch);
				return;
			}
			mDestRect.mX = theX;
			mDestRect.mY = theY;
			mDestRect.mWidth = theSrcRect.mWidth;
			mDestRect.mHeight = theSrcRect.mHeight;
			Rect rect = mDestRect.Intersection(mClipRect);
			mSrcRect.mX = theSrcRect.mX + rect.mX - theX;
			mSrcRect.mY = theSrcRect.mY + rect.mY - theY;
			mSrcRect.mWidth = rect.mWidth;
			mSrcRect.mHeight = rect.mHeight;
			if (mSrcRect.mWidth > 0 && mSrcRect.mHeight > 0)
			{
				SetAsCurrentContext();
				mRenderDevice.Blt(theImage, rect.mX, rect.mY, mSrcRect, GetImageColor(), mDrawMode);
			}
		}

		public void DrawImage(Image theImage, Rect theDestRect, Rect theSrcRect)
		{
			mDestRect.mX = theDestRect.mX + (int)mTransX;
			mDestRect.mY = theDestRect.mY + (int)mTransY;
			mDestRect.mWidth = theDestRect.mWidth;
			mDestRect.mHeight = theDestRect.mHeight;
			if (mScaleX != 1f || mScaleY != 1f)
			{
				mDestRect = new Rect((int)((double)mScaleOrigX + Math.Floor(((float)mDestRect.mX - mScaleOrigX) * mScaleX)), (int)((double)mScaleOrigY + Math.Floor(((float)mDestRect.mY - mScaleOrigY) * mScaleY)), (int)Math.Ceiling((float)mDestRect.mWidth * mScaleX), (int)Math.Ceiling((float)mDestRect.mHeight * mScaleY));
			}
			SetAsCurrentContext();
			mRenderDevice.BltStretched(theImage, mDestRect, theSrcRect, mClipRect, GetImageColor(), mDrawMode, mFastStretch);
		}

		public void DrawImage(Image theImage, int theX, int theY, int theStretchedWidth, int theStretchedHeight)
		{
			mDestRect.mX = theX + (int)mTransX;
			mDestRect.mY = theY + (int)mTransY;
			mDestRect.mWidth = theStretchedWidth;
			mDestRect.mHeight = theStretchedHeight;
			mSrcRect.mX = 0;
			mSrcRect.mY = 0;
			mSrcRect.mWidth = theImage.mWidth;
			mSrcRect.mHeight = theImage.mHeight;
			SetAsCurrentContext();
			mRenderDevice.BltStretched(theImage, mDestRect, theImage.GetRect(), mClipRect, GetImageColor(), mDrawMode, mFastStretch);
		}

		public void DrawImageF(Image theImage, float theX, float theY)
		{
			theX += mTransX;
			theY += mTransY;
			SetAsCurrentContext();
			mRenderDevice.BltF(theImage, theX, theY, theImage.GetRect(), mClipRect, GetImageColor(), mDrawMode);
		}

		public void DrawImageF(Image theImage, float theX, float theY, Rect theSrcRect)
		{
			theX += mTransX;
			theY += mTransY;
			SetAsCurrentContext();
			mRenderDevice.BltF(theImage, theX, theY, theSrcRect, mClipRect, GetImageColor(), mDrawMode);
		}

		public void DrawImageMirror(Image theImage, int theX, int theY)
		{
			DrawImageMirror(theImage, theX, theY, true);
		}

		public void DrawImageMirror(Image theImage, int theX, int theY, int theStretchedWidth, int theStretchedHeight)
		{
			mDestRect.setValue(theX, theY, theStretchedWidth, theStretchedHeight);
			DrawImageMirror(theImage, mDestRect, theImage.GetRect(), true);
		}

		public void DrawImageMirror(Image theImage, int theX, int theY, bool mirror)
		{
			DrawImageMirror(theImage, theX, theY, theImage.GetRect(), mirror);
		}

		public void DrawImageMirror(Image theImage, int theX, int theY, Rect theSrcRect)
		{
			DrawImageMirror(theImage, theX, theY, theSrcRect, true);
		}

		public void DrawImageMirror(Image theImage, int theX, int theY, Rect theSrcRect, bool mirror)
		{
			if (!mirror)
			{
				DrawImage(theImage, theX, theY, theSrcRect);
				return;
			}
			theX += (int)mTransX;
			theY += (int)mTransY;
			if (theSrcRect.mX + theSrcRect.mWidth <= theImage.GetWidth() && theSrcRect.mY + theSrcRect.mHeight <= theImage.GetHeight())
			{
				mDestRect.mX = theX;
				mDestRect.mY = theY;
				mDestRect.mWidth = theSrcRect.mWidth;
				mDestRect.mHeight = theSrcRect.mHeight;
				Rect rect = mDestRect.Intersection(mClipRect);
				int num = theSrcRect.mWidth - rect.mWidth;
				int num2 = rect.mX - theX;
				int num3 = num - num2;
				mSrcRect.mX = theSrcRect.mX + num3;
				mSrcRect.mY = theSrcRect.mY + rect.mY - theY;
				mSrcRect.mWidth = rect.mWidth;
				mSrcRect.mHeight = rect.mHeight;
				if (mSrcRect.mWidth > 0 && mSrcRect.mHeight > 0)
				{
					SetAsCurrentContext();
					mRenderDevice.BltMirror(theImage, rect.mX, rect.mY, mSrcRect, GetImageColor(), mDrawMode);
				}
			}
		}

		public void DrawImageMirror(Image theImage, Rect theDestRect, Rect theSrcRect)
		{
			DrawImageMirror(theImage, theDestRect, theSrcRect, true);
		}

		public void DrawImageMirror(Image theImage, Rect theDestRect, Rect theSrcRect, bool mirror)
		{
			if (!mirror)
			{
				DrawImage(theImage, theDestRect, theSrcRect);
				return;
			}
			mDestRect.mX = theDestRect.mX + (int)mTransX;
			mDestRect.mY = theDestRect.mY + (int)mTransY;
			mDestRect.mWidth = theDestRect.mWidth;
			mDestRect.mHeight = theDestRect.mHeight;
			SetAsCurrentContext();
			mRenderDevice.BltStretched(theImage, mDestRect, theSrcRect, mClipRect, GetImageColor(), mDrawMode, mFastStretch, true);
		}

		public void DrawImageRotated(Image theImage, int theX, int theY, double theRot)
		{
			DrawImageRotated(theImage, theX, theY, theRot, Rect.INVALIDATE_RECT);
		}

		public void DrawImageRotated(Image theImage, int theX, int theY, double theRot, Rect theSrcRect)
		{
			if (theSrcRect == Rect.INVALIDATE_RECT)
			{
				int num = theImage.GetWidth() / 2;
				int num2 = theImage.GetHeight() / 2;
				DrawImageRotatedF(theImage, theX, theY, theRot, num, num2, theSrcRect);
			}
			else
			{
				int num3 = theSrcRect.mWidth / 2;
				int num4 = theSrcRect.mHeight / 2;
				DrawImageRotatedF(theImage, theX, theY, theRot, num3, num4, theSrcRect);
			}
		}

		public void DrawImageRotated(Image theImage, int theX, int theY, double theRot, int theRotCenterX, int theRotCenterY)
		{
			DrawImageRotated(theImage, theX, theY, theRot, theRotCenterX, theRotCenterY, Rect.INVALIDATE_RECT);
		}

		public void DrawImageRotated(Image theImage, int theX, int theY, double theRot, int theRotCenterX, int theRotCenterY, Rect theSrcRect)
		{
			DrawImageRotatedF(theImage, theX, theY, theRot, theRotCenterX, theRotCenterY, theSrcRect);
		}

		public void DrawImageRotatedF(Image theImage, float theX, float theY, double theRot)
		{
			DrawImageRotatedF(theImage, theX, theY, theRot, Rect.INVALIDATE_RECT);
		}

		public void DrawImageRotatedF(Image theImage, float theX, float theY, double theRot, Rect theSrcRect)
		{
			if (theSrcRect == Rect.INVALIDATE_RECT)
			{
				float theRotCenterX = (float)theImage.GetWidth() / 2f;
				float theRotCenterY = (float)theImage.GetHeight() / 2f;
				DrawImageRotatedF(theImage, theX, theY, theRot, theRotCenterX, theRotCenterY, theSrcRect);
			}
			else
			{
				float theRotCenterX2 = (float)theSrcRect.mWidth / 2f;
				float theRotCenterY2 = (float)theSrcRect.mHeight / 2f;
				DrawImageRotatedF(theImage, theX, theY, theRot, theRotCenterX2, theRotCenterY2, theSrcRect);
			}
		}

		public void DrawImageRotatedF(Image theImage, float theX, float theY, double theRot, float theRotCenterX, float theRotCenterY)
		{
			DrawImageRotatedF(theImage, theX, theY, theRot, theRotCenterX, theRotCenterY, Rect.INVALIDATE_RECT);
		}

		public void DrawImageRotatedF(Image theImage, float theX, float theY, double theRot, float theRotCenterX, float theRotCenterY, Rect theSrcRect)
		{
			theX += mTransX;
			theY += mTransY;
			SetAsCurrentContext();
			if (theSrcRect == Rect.INVALIDATE_RECT)
			{
				mRenderDevice.BltRotated(theImage, theX, theY, theImage.GetRect(), mClipRect, GetImageColor(), mDrawMode, theRot, theRotCenterX, theRotCenterY);
			}
			else
			{
				mRenderDevice.BltRotated(theImage, theX, theY, theSrcRect, mClipRect, GetImageColor(), mDrawMode, theRot, theRotCenterX, theRotCenterY);
			}
		}

		public void DrawImageMatrix(Image theImage, SexyTransform2D theMatrix, float x)
		{
			DrawImageMatrix(theImage, theMatrix, x, 0f);
		}

		public void DrawImageMatrix(Image theImage, SexyTransform2D theMatrix)
		{
			DrawImageMatrix(theImage, theMatrix, 0f, 0f);
		}

		public void DrawImageMatrix(Image theImage, SexyTransform2D theMatrix, float x, float y)
		{
			SetAsCurrentContext();
			mRenderDevice.BltMatrix(theImage, x + mTransX, y + mTransY, theMatrix, mClipRect, GetImageColor(), mDrawMode, theImage.GetRect(), mLinearBlend);
		}

		public void DrawImageMatrix(Image theImage, SexyTransform2D theMatrix, Rect theSrcRect, float x)
		{
			DrawImageMatrix(theImage, theMatrix, theSrcRect, x, 0f);
		}

		public void DrawImageMatrix(Image theImage, SexyTransform2D theMatrix, Rect theSrcRect)
		{
			DrawImageMatrix(theImage, theMatrix, theSrcRect, 0f, 0f);
		}

		public void DrawImageMatrix(Image theImage, SexyTransform2D theMatrix, Rect theSrcRect, float x, float y)
		{
			SetAsCurrentContext();
			mRenderDevice.BltMatrix(theImage, x + mTransX, y + mTransY, theMatrix, mClipRect, GetImageColor(), mDrawMode, theSrcRect, mLinearBlend);
		}

		public void DrawImageTransform(Image theImage, Transform theTransform, float x, float y)
		{
			DrawImageTransformHelper(theImage, theTransform, theImage.GetRect(), x, y, false);
		}

		public void DrawImageTransform(Image theImage, Transform theTransform, Rect theSrcRect, float x, float y)
		{
			DrawImageTransformHelper(theImage, theTransform, theSrcRect, x, y, false);
		}

		public void DrawImageTransformF(Image theImage, Transform theTransform, float x, float y)
		{
			DrawImageTransformHelper(theImage, theTransform, theImage.GetRect(), x, y, true);
		}

		public void DrawImageTransformF(Image theImage, Transform theTransform, Rect theSrcRect, float x, float y)
		{
			DrawImageTransformHelper(theImage, theTransform, theSrcRect, x, y, true);
		}

		public void DrawTriangleTex(Image theTexture, SexyVertex2D v1, SexyVertex2D v2, SexyVertex2D v3)
		{
			SexyVertex2D[,] theVertices = new SexyVertex2D[1, 3] { { v1, v2, v3 } };
			SetAsCurrentContext();
			mRenderDevice.BltTriangles(theTexture, theVertices, 1, GetImageColor(), mDrawMode, mTransX, mTransY, mLinearBlend, mClipRect);
		}

		public void DrawTrianglesTex(Image theTexture, SexyVertex2D[,] theVertices, int theNumTriangles)
		{
			SetAsCurrentContext();
			mRenderDevice.BltTriangles(theTexture, theVertices, theNumTriangles, GetImageColor(), mDrawMode, mTransX, mTransY, mLinearBlend, mClipRect);
		}

		public void DrawTrianglesTex(Image theTexture, SexyVertex2D[,] theVertices, int theNumTriangles, Color theColor, int theDrawMode, float tx, float ty, bool blend, Rect theClipRect)
		{
			SetAsCurrentContext();
			mRenderDevice.BltTriangles(theTexture, theVertices, theNumTriangles, theColor, theDrawMode, tx, ty, blend, theClipRect);
		}

		public void DrawTrianglesTexStrip(Image theTexture, SexyVertex2D[] theVertices, int theNumTriangles)
		{
			DrawTrianglesTexStrip(theTexture, theVertices, theNumTriangles, GetImageColor(), mDrawMode, mTransX, mTransY, mLinearBlend);
		}

		public void DrawTrianglesTexStrip(Image theTexture, SexyVertex2D[] theVertices, int theNumTriangles, Color theColor, int theDrawMode, float tx, float ty, bool blend)
		{
			SetAsCurrentContext();
			int num = 0;
			while (num < theNumTriangles)
			{
				int num2 = Math.Min(100, theNumTriangles - num);
				for (int i = 0; i < num2; i++)
				{
					DTTS_aList[i, 0] = theVertices[num];
					DTTS_aList[i, 1] = theVertices[num + 1];
					DTTS_aList[i, 2] = theVertices[num + 2];
					num++;
				}
				mRenderDevice.BltTriangles(theTexture, DTTS_aList, num2, theColor, theDrawMode, tx, ty, blend);
			}
		}

		public void DrawImageCel(Image theImageStrip, int theX, int theY, int theCel)
		{
			DrawImageCel(theImageStrip, theX, theY, theCel % theImageStrip.mNumCols, theCel / theImageStrip.mNumCols);
		}

		public void DrawImageCel(Image theImageStrip, Rect theDestRect, int theCel)
		{
			DrawImageCel(theImageStrip, theDestRect, theCel % theImageStrip.mNumCols, theCel / theImageStrip.mNumCols);
		}

		public void DrawImageCel(Image theImageStrip, int theX, int theY, int theCelCol, int theCelRow)
		{
			if (theCelRow >= 0 && theCelCol >= 0 && theCelRow < theImageStrip.mNumRows && theCelCol < theImageStrip.mNumCols)
			{
				int num = theImageStrip.mWidth / theImageStrip.mNumCols;
				int num2 = theImageStrip.mHeight / theImageStrip.mNumRows;
				Rect theSrcRect = new Rect(num * theCelCol, num2 * theCelRow, num, num2);
				DrawImage(theImageStrip, theX, theY, theSrcRect);
			}
		}

		public void DrawImageCel(Image theImageStrip, Rect theDestRect, int theCelCol, int theCelRow)
		{
			if (theCelRow >= 0 && theCelCol >= 0 && theCelRow < theImageStrip.mNumRows && theCelCol < theImageStrip.mNumCols)
			{
				int num = theImageStrip.mWidth / theImageStrip.mNumCols;
				int num2 = theImageStrip.mHeight / theImageStrip.mNumRows;
				Rect theSrcRect = new Rect(num * theCelCol, num2 * theCelRow, num, num2);
				DrawImage(theImageStrip, theDestRect, theSrcRect);
			}
		}

		public void DrawImageAnim(Image theImageAnim, int theX, int theY, int theTime)
		{
			DrawImageCel(theImageAnim, theX, theY, theImageAnim.GetAnimCel(theTime));
		}

		public void ClearClipRect()
		{
			if (mDestImage != null)
			{
				mClipRect.mX = 0;
				mClipRect.mY = 0;
				mClipRect.mWidth = mDestImage.GetWidth();
				mClipRect.mHeight = mDestImage.GetHeight();
			}
			else
			{
				mClipRect.mX = 0;
				mClipRect.mY = 0;
				mClipRect.mWidth = GlobalMembers.gSexyAppBase.mWidth;
				mClipRect.mHeight = GlobalMembers.gSexyAppBase.mHeight;
			}
		}

		public void SetClipRect(int theX, int theY, int theWidth, int theHeight)
		{
			if (mDestImage != null)
			{
				mClipRect.mX = 0;
				mClipRect.mY = 0;
				mClipRect.mWidth = mDestImage.GetWidth();
				mClipRect.mHeight = mDestImage.GetHeight();
				mDestRect.mX = theX + (int)mTransX;
				mDestRect.mY = theY + (int)mTransY;
				mDestRect.mWidth = theWidth;
				mDestRect.mHeight = theHeight;
				mClipRect = mClipRect.Intersection(mDestRect);
			}
			else
			{
				mClipRect.mX = 0;
				mClipRect.mY = 0;
				mClipRect.mWidth = GlobalMembers.gSexyAppBase.mWidth;
				mClipRect.mHeight = GlobalMembers.gSexyAppBase.mHeight;
				mDestRect.mX = theX + (int)mTransX;
				mDestRect.mY = theY + (int)mTransY;
				mDestRect.mWidth = theWidth;
				mDestRect.mHeight = theHeight;
				mClipRect = mClipRect.Intersection(mDestRect);
			}
		}

		public void SetClipRect(Rect theRect)
		{
			SetClipRect(theRect.mX, theRect.mY, theRect.mWidth, theRect.mHeight);
		}

		public void ClipRect(int theX, int theY, int theWidth, int theHeight)
		{
			mDestRect.mX = theX + (int)mTransX;
			mDestRect.mY = theY + (int)mTransY;
			mDestRect.mWidth = theWidth;
			mDestRect.mHeight = theHeight;
			mClipRect = mClipRect.Intersection(mDestRect);
		}

		public void ClipRect(Rect theRect)
		{
			ClipRect(theRect.mX, theRect.mY, theRect.mWidth, theRect.mHeight);
		}

		public void Translate(int theTransX, int theTransY)
		{
			mTransX += theTransX;
			mTransY += theTransY;
		}

		public void TranslateF(float theTransX, float theTransY)
		{
			mTransX += theTransX;
			mTransY += theTransY;
		}

		public void SetScale(float theScaleX, float theScaleY, float theOrigX, float theOrigY)
		{
			mScaleX = theScaleX;
			mScaleY = theScaleY;
			mScaleOrigX = theOrigX + mTransX;
			mScaleOrigY = theOrigY + mTransY;
		}

		public int StringWidth(string theString)
		{
			return mFont.StringWidth(theString);
		}

		public void DrawImageBox(Rect theDest, Image theComponentImage)
		{
			DrawImageBox(theComponentImage.GetRect(), theDest, theComponentImage);
		}

		public void DrawImageBox(Rect theSrc, Rect theDest, Image theComponentImage)
		{
			if (theSrc.mWidth <= 0 || theSrc.mHeight <= 0)
			{
				return;
			}
			int width = theSrc.mWidth / 3;
			int height = theSrc.mHeight / 3;
			int x = theSrc.mX;
			int y = theSrc.mY;
			int num = theSrc.mWidth - width * 2;
			int num2 = theSrc.mHeight - height * 2;
			int width2 = width;
			int height2 = height;
			bool flag = false;
			if (theDest.mWidth < width * 2)
			{
				width2 = theDest.mWidth / 2;
				if ((theDest.mWidth & 1) == 1)
				{
					width2++;
				}
				flag = true;
			}
			if (theDest.mHeight < height * 2)
			{
				height2 = theDest.mHeight / 2;
				if ((theDest.mHeight & 1) == 1)
				{
					height2++;
				}
				flag = true;
			}
			Rect rect = mClipRect;
			if (flag)
			{
				mDestRect.setValue(ref theDest.mX, ref theDest.mY, ref width2, ref height2);
				mSrcRect.setValue(ref x, ref y, ref width, ref height);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
				mDestRect.setValue(theDest.mX + theDest.mWidth - width2, theDest.mY, width2, height2);
				mSrcRect.setValue(x + width + num, y, width, height);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
				mDestRect.setValue(theDest.mX, theDest.mY + theDest.mHeight - height2, width2, height2);
				mSrcRect.setValue(x, y + height + num2, width, height);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
				mDestRect.setValue(theDest.mX + theDest.mWidth - width2, theDest.mY + theDest.mHeight - height2, width2, height2);
				mSrcRect.setValue(x + width + num, y + height + num2, width, height);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
				ClipRect(theDest.mX + width2, theDest.mY, theDest.mWidth - width2 * 2, theDest.mHeight);
				int i;
				for (i = 0; i < (theDest.mWidth - width * 2 + num - 1) / num; i++)
				{
					mDestRect.setValue(theDest.mX + width2 + i * num, theDest.mY, num, height2);
					mSrcRect.setValue(x + width, y, num, height);
					DrawImage(theComponentImage, mDestRect, mSrcRect);
					mDestRect.setValue(theDest.mX + width2 + i * num, theDest.mY + theDest.mHeight - height2, num, height2);
					mSrcRect.setValue(x + width, y + height + num2, num, height);
					DrawImage(theComponentImage, mDestRect, mSrcRect);
				}
				mClipRect = rect;
				ClipRect(theDest.mX, theDest.mY + height2, theDest.mWidth, theDest.mHeight - height2 * 2);
				for (int j = 0; j < (theDest.mHeight - height * 2 + num2 - 1) / num2; j++)
				{
					mDestRect.setValue(theDest.mX + width2 + i * num, theDest.mY, num, height2);
					mSrcRect.setValue(x, y + height, width, num2);
					DrawImage(theComponentImage, mDestRect, mSrcRect);
					mDestRect.setValue(theDest.mX + theDest.mWidth - width2, theDest.mY + height2 + j * num2, width2, num2);
					mSrcRect.setValue(x + width + num, y + height, width, num2);
					DrawImage(theComponentImage, mDestRect, mSrcRect);
				}
				mClipRect = rect;
				ClipRect(theDest.mX + width2, theDest.mY + height2, theDest.mWidth - width2 * 2, theDest.mHeight - height2 * 2);
				for (i = 0; i < (theDest.mWidth - width2 * 2 + num - 1) / num; i++)
				{
					for (int j = 0; j < (theDest.mHeight - height2 * 2 + num2 - 1) / num2; j++)
					{
						mSrcRect.setValue(x + width2, y + height2, num, num2);
						DrawImage(theComponentImage, theDest.mX + width2 + i * num, theDest.mY + height2 + j * num2, mSrcRect);
					}
				}
				mClipRect = rect;
				return;
			}
			mSrcRect.setValue(x, y, width, height);
			DrawImage(theComponentImage, theDest.mX, theDest.mY, mSrcRect);
			mSrcRect.setValue(x + width + num, y, width, height);
			DrawImage(theComponentImage, theDest.mX + theDest.mWidth - width, theDest.mY, mSrcRect);
			mSrcRect.setValue(x, y + height + num2, width, height);
			DrawImage(theComponentImage, theDest.mX, theDest.mY + theDest.mHeight - height, mSrcRect);
			mSrcRect.setValue(x + width + num, y + height + num2, width, height);
			DrawImage(theComponentImage, theDest.mX + theDest.mWidth - width, theDest.mY + theDest.mHeight - height, mSrcRect);
			ClipRect(theDest.mX + width, theDest.mY, theDest.mWidth - width * 2, theDest.mHeight);
			for (int k = 0; k < (theDest.mWidth - width * 2 + num - 1) / num; k++)
			{
				mSrcRect.setValue(x + width, y, num, height);
				DrawImage(theComponentImage, theDest.mX + width + k * num, theDest.mY, mSrcRect);
				mSrcRect.setValue(x + width, y + height + num2, num, height);
				DrawImage(theComponentImage, theDest.mX + width + k * num, theDest.mY + theDest.mHeight - height, mSrcRect);
			}
			mClipRect = rect;
			ClipRect(theDest.mX, theDest.mY + height, theDest.mWidth, theDest.mHeight - height * 2);
			for (int l = 0; l < (theDest.mHeight - height * 2 + num2 - 1) / num2; l++)
			{
				mSrcRect.setValue(x, y + height, width, num2);
				DrawImage(theComponentImage, theDest.mX, theDest.mY + height + l * num2, mSrcRect);
				mSrcRect.setValue(x + width + num, y + height, width, num2);
				DrawImage(theComponentImage, theDest.mX + theDest.mWidth - width, theDest.mY + height + l * num2, mSrcRect);
			}
			mClipRect = rect;
			ClipRect(theDest.mX + width, theDest.mY + height, theDest.mWidth - width * 2, theDest.mHeight - height * 2);
			for (int k = 0; k < (theDest.mWidth - width * 2 + num - 1) / num; k++)
			{
				for (int l = 0; l < (theDest.mHeight - height * 2 + num2 - 1) / num2; l++)
				{
					mSrcRect.setValue(x + width, y + height, num, num2);
					DrawImage(theComponentImage, theDest.mX + width + k * num, theDest.mY + height + l * num2, mSrcRect);
				}
			}
			mClipRect = rect;
		}

		public void DrawImageBoxStretch(Rect theDest, Image theComponentImage)
		{
			DrawImageBoxStretch(theComponentImage.GetRect(), theDest, theComponentImage);
		}

		public void DrawImageBoxStretch(Rect theSrc, Rect theDest, Image theComponentImage)
		{
			if (theSrc.mWidth <= 0 || theSrc.mHeight <= 0)
			{
				return;
			}
			int num = theSrc.mWidth / 3;
			int num2 = theSrc.mHeight / 3;
			int mX = theSrc.mX;
			int mY = theSrc.mY;
			int num3 = theSrc.mWidth - num * 2;
			int num4 = theSrc.mHeight - num2 * 2;
			int num5 = num;
			int num6 = num2;
			if (theDest.mWidth < num * 2)
			{
				num5 = theDest.mWidth / 2;
				if ((theDest.mWidth & 1) == 1)
				{
					num5++;
				}
			}
			if (theDest.mHeight < num2 * 2)
			{
				num6 = theDest.mHeight / 2;
				if ((theDest.mHeight & 1) == 1)
				{
					num6++;
				}
			}
			mDestRect.setValue(theDest.mX, theDest.mY, num5, num6);
			mSrcRect.setValue(mX, mY, num, num2);
			DrawImage(theComponentImage, mDestRect, mSrcRect);
			mDestRect.setValue(theDest.mX + theDest.mWidth - num5, theDest.mY, num5, num6);
			mSrcRect.setValue(mX + num + num3, mY, num, num2);
			DrawImage(theComponentImage, mDestRect, mSrcRect);
			mDestRect.setValue(theDest.mX, theDest.mY + theDest.mHeight - num6, num5, num6);
			mSrcRect.setValue(mX, mY + num2 + num4, num, num2);
			DrawImage(theComponentImage, mDestRect, mSrcRect);
			mDestRect.setValue(theDest.mX + theDest.mWidth - num5, theDest.mY + theDest.mHeight - num6, num5, num6);
			mSrcRect.setValue(mX + num + num3, mY + num2 + num4, num, num2);
			DrawImage(theComponentImage, mDestRect, mSrcRect);
			if (theDest.mWidth - num5 * 2 > 0)
			{
				mDestRect.setValue(theDest.mX + num5, theDest.mY, theDest.mWidth - num5 * 2, num6);
				mSrcRect.setValue(mX + num, mY, num3, num2);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
				mDestRect.setValue(theDest.mX + num5, theDest.mY + theDest.mHeight - num6, theDest.mWidth - num5 * 2, num6);
				mSrcRect.setValue(mX + num, mY + num2 + num4, num3, num2);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
			}
			if (theDest.mHeight - num6 * 2 > 0)
			{
				mDestRect.setValue(theDest.mX, theDest.mY + num6, num5, theDest.mHeight - num6 * 2);
				mSrcRect.setValue(mX, mY + num2, num, num4);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
				mDestRect.setValue(theDest.mX + theDest.mWidth - num5, theDest.mY + num6, num5, theDest.mHeight - num6 * 2);
				mSrcRect.setValue(mX + num + num3, mY + num2, num, num4);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
			}
			if (theDest.mWidth - num5 * 2 > 0 && theDest.mHeight - num6 * 2 > 0)
			{
				mDestRect.setValue(theDest.mX + num5, theDest.mY + num6, theDest.mWidth - num5 * 2, theDest.mHeight - num6 * 2);
				mSrcRect.setValue(mX + num, mY + num2, num3, num4);
				DrawImage(theComponentImage, mDestRect, mSrcRect);
			}
		}

		public int WriteString(string theString, int theX, int theY, int theWidth, int theJustification, bool drawString, int theOffset, int theLength)
		{
			return WriteString(theString, theX, theY, theWidth, theJustification, drawString, theOffset, theLength, -1);
		}

		public int WriteString(string theString, int theX, int theY, int theWidth, int theJustification, bool drawString, int theOffset)
		{
			return WriteString(theString, theX, theY, theWidth, theJustification, drawString, theOffset, -1, -1);
		}

		public int WriteString(string theString, int theX, int theY, int theWidth, int theJustification, bool drawString)
		{
			return WriteString(theString, theX, theY, theWidth, theJustification, drawString, 0, -1, -1);
		}

		public int WriteString(string theString, int theX, int theY, int theWidth, int theJustification)
		{
			return WriteString(theString, theX, theY, theWidth, theJustification, true, 0, -1, -1);
		}

		public int WriteString(string theString, int theX, int theY, int theWidth)
		{
			return WriteString(theString, theX, theY, theWidth, 0, true, 0, -1, -1);
		}

		public int WriteString(string theString, int theX, int theY)
		{
			return WriteString(theString, theX, theY, -1, 0, true, 0, -1, -1);
		}

		public int WriteString(string theString, int theX, int theY, int theWidth, int theJustification, bool drawString, int theOffset, int theLength, int theOldColor)
		{
			if (theOldColor == -1)
			{
				theOldColor = GetFinalColor().ToInt();
			}
			if (drawString)
			{
				switch (theJustification)
				{
				case 0:
					theX += (theWidth - WriteString(theString, theX, theY, theWidth, -1, false, theOffset, theLength, theOldColor)) / 2;
					break;
				case 1:
					theX += theWidth - WriteString(theString, theX, theY, theWidth, -1, false, theOffset, theLength, theOldColor);
					break;
				}
			}
			theLength = ((theLength >= 0 && theOffset + theLength <= theString.Length) ? (theOffset + theLength) : theString.Length);
			mStringBuilder.Clear();
			int num = 0;
			for (int i = theOffset; i < theLength; i++)
			{
				if (theString[i] == '^' && mWriteColoredString)
				{
					if (i + 1 < theLength && theString[i + 1] == '^')
					{
						mStringBuilder.Append("^");
						i++;
						continue;
					}
					if (i > theLength - 8)
					{
						break;
					}
					int num2 = 0;
					if (theString[i + 1] == 'o')
					{
						if (theString.Substring(i + 1).StartsWith("oldclr"))
						{
							num2 = theOldColor;
						}
					}
					else
					{
						for (int j = 0; j < 6; j++)
						{
							char c = theString[i + j + 1];
							int num3 = 0;
							if (c >= '0' && c <= '9')
							{
								num3 = c - 48;
							}
							else if (c >= 'A' && c <= 'F')
							{
								num3 = c - 65 + 10;
							}
							else if (c >= 'a' && c <= 'f')
							{
								num3 = c - 97 + 10;
							}
							num2 += num3 << (5 - j) * 4;
						}
					}
					string theString2 = mStringBuilder.ToString();
					if (drawString)
					{
						DrawString(theString2, theX + num, theY);
						SetColor((num2 >> 16) & 0xFF, (num2 >> 8) & 0xFF, num2 & 0xFF, GetColor().mAlpha);
					}
					i += 7;
					num += mFont.StringWidth(theString2);
					mStringBuilder.Clear();
				}
				else
				{
					mStringBuilder.Append(theString[i]);
				}
			}
			string theString3 = mStringBuilder.ToString();
			if (drawString)
			{
				DrawString(theString3, theX + num, theY);
			}
			return num + mFont.StringWidth(theString3);
		}

		public int WriteWordWrapped(Rect theRect, string theLine, int theLineSpacing, int theJustification, ref int theMaxWidth, int theMaxChars, ref int theLastWidth, ref int theLineCount, bool drawString)
		{
			Color color = GetColor();
			int num = color.ToInt();
			if ((num & 0xFF000000u) == 4278190080u)
			{
				num &= 0xFFFFFF;
			}
			if (theMaxChars < 0)
			{
				theMaxChars = theLine.Length;
			}
			Font font = GetFont();
			int num2 = font.GetAscent() - font.GetAscentPadding();
			if (theLineSpacing == -1)
			{
				theLineSpacing = font.GetLineSpacing();
			}
			int i = 0;
			int num3 = 0;
			int num4 = 0;
			char c = '\0';
			char thePrevChar = '\0';
			int num5 = -1;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			num7 = theLastWidth;
			num4 = num7;
			while (i < theLine.Length)
			{
				c = theLine[i];
				if (c == '^' && mWriteColoredString)
				{
					if (i + 1 < theLine.Length)
					{
						if (theLine.ToCharArray()[i + 1] != '^')
						{
							i += 8;
							continue;
						}
						i++;
					}
				}
				else
				{
					switch (c)
					{
					case ' ':
						num5 = i;
						break;
					case '\n':
						num4 = theRect.mWidth + 1;
						num5 = i;
						i++;
						break;
					}
				}
				num4 += font.CharWidthKern(c, thePrevChar);
				thePrevChar = c;
				if (num4 > theRect.mWidth)
				{
					num8++;
					int num10;
					if (num5 != -1)
					{
						int num9 = theRect.mY + num2 + (int)mTransY;
						if (num9 >= mClipRect.mY && num9 < mClipRect.mY + mClipRect.mHeight + theLineSpacing)
						{
							GlobalMembersGraphics.WriteWordWrappedHelper(this, theLine, theRect.mX + num7, theRect.mY + num2, theRect.mWidth, theJustification, drawString, num3, num5 - num3, num, theMaxChars);
						}
						num10 = num4 + num7;
						if (num10 < 0)
						{
							break;
						}
						i = num5 + 1;
						if (c != '\n')
						{
							for (; i < theLine.Length && theLine[i] == ' '; i++)
							{
							}
						}
						num3 = i;
					}
					else
					{
						if (i < num3 + 1)
						{
							i++;
						}
						num10 = GlobalMembersGraphics.WriteWordWrappedHelper(this, theLine, theRect.mX + num7, theRect.mY + num2, theRect.mWidth, theJustification, drawString, num3, i - num3, num, theMaxChars);
						if (num10 < 0)
						{
							break;
						}
						if (num10 > theMaxWidth)
						{
							theMaxWidth = num10;
						}
						theLastWidth = num10;
					}
					if (num10 > num6)
					{
						num6 = num10;
					}
					num3 = i;
					num5 = -1;
					num4 = 0;
					thePrevChar = '\0';
					num7 = 0;
					num2 += theLineSpacing;
				}
				else
				{
					i++;
				}
			}
			if (num3 < theLine.Length)
			{
				int num11 = GlobalMembersGraphics.WriteWordWrappedHelper(this, theLine, theRect.mX + num7, theRect.mY + num2, theRect.mWidth, theJustification, drawString, num3, theLine.Length - num3, num, theMaxChars);
				if (num11 >= 0)
				{
					if (num11 > num6)
					{
						num6 = num11;
					}
					if (num11 > theMaxWidth)
					{
						theMaxWidth = num11;
					}
					theLastWidth = num11;
					num2 += theLineSpacing;
				}
			}
			else if (c == '\n')
			{
				num2 += theLineSpacing;
				theLastWidth = 0;
			}
			SetColor(color);
			theMaxWidth = num6;
			theLineCount = num8;
			return num2 + font.GetDescent() - theLineSpacing;
		}

		public int WriteWordWrapped(Rect theRect, string theLine, int theLineSpacing, int theJustification, ref int theMaxWidth, int theMaxChars, ref int theLastWidth, ref int theLineCount)
		{
			return WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, true);
		}

		public int WriteWordWrapped(Rect theRect, string theLine, int theLineSpacing, int theJustification, ref int theMaxWidth, int theMaxChars, ref int theLastWidth)
		{
			int theLineCount = 0;
			return WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, true);
		}

		public int WriteWordWrapped(Rect theRect, string theLine, int theLineSpacing, int theJustification, ref int theMaxWidth, int theMaxChars)
		{
			int theLineCount = 0;
			int theLastWidth = 0;
			return WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, true);
		}

		public int WriteWordWrapped(Rect theRect, string theLine, int theLineSpacing, int theJustification, ref int theMaxWidth)
		{
			int theLineCount = 0;
			int theLastWidth = 0;
			int theMaxChars = -1;
			return WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, true);
		}

		public int WriteWordWrapped(Rect theRect, string theLine, int theLineSpacing, int theJustification)
		{
			int theLineCount = 0;
			int theLastWidth = 0;
			int theMaxChars = -1;
			int theMaxWidth = 0;
			return WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, true);
		}

		public int WriteWordWrapped(Rect theRect, string theLine, int theLineSpacing)
		{
			int theLineCount = 0;
			int theLastWidth = 0;
			int theMaxChars = -1;
			int theMaxWidth = 0;
			int theJustification = -1;
			return WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, true);
		}

		public int WriteWordWrapped(Rect theRect, string theLine)
		{
			int theLineCount = 0;
			int theLastWidth = 0;
			int theMaxChars = -1;
			int theMaxWidth = 0;
			int theJustification = -1;
			int theLineSpacing = -1;
			return WriteWordWrapped(theRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, true);
		}

		public int DrawStringColor(string theLine, int theX, int theY)
		{
			return DrawStringColor(theLine, theX, theY, -1);
		}

		public int DrawStringColor(string theLine, int theX, int theY, int theOldColor)
		{
			return WriteString(theLine, theX, theY, -1, -1, true, 0, -1, theOldColor);
		}

		public int DrawStringWordWrapped(string theLine, int theX, int theY, int theWrapWidth, int theLineSpacing, int theJustification, ref int theMaxWidth)
		{
			int num = mFont.GetAscent() - mFont.GetAscentPadding();
			mDestRect.setValue(theX, theY - num, theWrapWidth, 0);
			return WriteWordWrapped(mDestRect, theLine, theLineSpacing, theJustification, ref theMaxWidth);
		}

		public int GetWordWrappedHeight(int theWidth, string theLine, int theLineSpacing, ref int theMaxWidth, ref int theLineCount)
		{
			Graphics graphics = new Graphics();
			graphics.SetFont(mFont);
			theLineCount = 0;
			int theLastWidth = 0;
			int theMaxChars = -1;
			theMaxWidth = 0;
			int theJustification = -1;
			mDestRect.setValue(0, 0, theWidth, 0);
			return graphics.WriteWordWrapped(mDestRect, theLine, theLineSpacing, theJustification, ref theMaxWidth, theMaxChars, ref theLastWidth, ref theLineCount, false);
		}

		public bool Is3D()
		{
			return mIs3D;
		}

		public void flush()
		{
			SetAsCurrentContext();
			if (mRenderDevice is RenderDevice3D)
			{
				((RenderDevice3D)mRenderDevice).Flush();
			}
		}
	}
}
