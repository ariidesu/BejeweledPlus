using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class DigBackground : Background
	{
		public DigGoal mGoal;

		public float mLastGridDepth;

		public DigBackground(DigGoal theGoal)
			: base(string.Empty, true, false)
		{
			mGoal = theGoal;
			mLastGridDepth = 0f;
		}

		public override void Draw(Graphics g)
		{
			DrawFull(g);
		}

		public virtual void DrawBack(Graphics g)
		{
			double num = GlobalMembers.M(0.0);
			g.SetColor(Utils.ColorLerp(new Color(GlobalMembers.M(190), GlobalMembers.M(150), GlobalMembers.M(95)), new Color(GlobalMembers.M(174), GlobalMembers.M(0), GlobalMembers.M(0)), (int)(float)num));
			g.FillRect(GlobalMembers.MS(620), GlobalMembers.MS(0), GlobalMembers.MS(1200), GlobalMembers.MS(1200));
			g.SetColor(new Color(-1));
		}

		public virtual void DrawFull(Graphics g)
		{
			DrawBack(g);
		}

		public override SharedImageRef GetBackgroundImage(bool wait)
		{
			return GetBackgroundImage(wait, true);
		}

		public override SharedImageRef GetBackgroundImage()
		{
			return GetBackgroundImage(true, true);
		}

		public override SharedImageRef GetBackgroundImage(bool wait, bool removeAnim)
		{
			mImage.mUnsharedImage = mSharedRenderTarget.Lock(GlobalMembers.gApp.mScreenBounds.mWidth, GlobalMembers.gApp.mScreenBounds.mHeight);
			Graphics graphics = new Graphics(mImage.GetImage());
			graphics.Translate(GlobalMembers.MS(-160) - GlobalMembers.gApp.mScreenBounds.mX, GlobalMembers.gApp.mScreenBounds.mY);
			DrawFull(graphics);
			mSharedRenderTarget.Unlock();
			return mImage;
		}

		public override void LoadImageProc()
		{
		}
	}
}
