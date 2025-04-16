using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class TextNotifyEffect : Effect
	{
		private static SimpleObjectPool thePool_;

		public string mText = string.Empty;

		public int mUpdateCnt;

		public int mDuration;

		public Font mFont;

		public DeviceImage mTexture;

		public bool mIgnoreTexture;

		private CurvedVal Draw_cvScaleIn = new CurvedVal(GlobalMembers.MP("b+0,1.3,0,0.2,#6g<     8~###    ii###"));

		private CurvedVal Draw_cvScaleOut = new CurvedVal(GlobalMembers.MP("b+0,1,0,0.2,~###         ~#>Hu"));

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(TextNotifyEffect));
		}

		public new static TextNotifyEffect alloc()
		{
			TextNotifyEffect textNotifyEffect = (TextNotifyEffect)thePool_.alloc();
			textNotifyEffect.init();
			return textNotifyEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}

		public TextNotifyEffect()
			: base(Type.TYPE_CUSTOMCLASS)
		{
		}

		public void init()
		{
			init(Type.TYPE_CUSTOMCLASS);
			mUpdateCnt = 0;
			mDuration = GlobalMembers.M(200);
			mFont = GlobalMembersResources.FONT_HUGE;
			mTexture = null;
			mDAlpha = 0f;
			mIgnoreTexture = false;
		}

		public override void Dispose()
		{
			if (mTexture != null)
			{
				mTexture.Dispose();
				mTexture = null;
			}
			base.Dispose();
		}

		public override void Draw(Graphics g)
		{
			if (g.mPushedColorVector.Count > 0)
			{
				g.PopColorMult();
			}
			Color color = g.GetColor();
			g.SetScale(mScale, mScale, 0f, 0f);
			g.SetColor(mColor);
			g.DrawString(mText, (int)(GlobalMembers.S(mX) - (float)g.StringWidth(mText) * mScale / 2f), (int)((GlobalMembers.S(mY) + (float)(g.GetFont().GetAscent() / 2)) / mScale));
			g.SetColor(color);
			g.SetScale(1f, 1f, 0f, 0f);
		}

		public override void Update()
		{
			if (mDelay > 0f)
			{
				mDelay -= 1f;
				return;
			}
			mUpdateCnt++;
			if (mUpdateCnt >= 0)
			{
				if (!mIgnoreTexture && mTexture == null)
				{
					mTexture = new DeviceImage();
					mTexture.AddImageFlags(16u);
					mTexture.Create(mFont.StringWidth(mText), mFont.GetLineSpacing());
					mTexture.mHasAlpha = true;
					mTexture.mHasTrans = true;
					Graphics graphics = new Graphics(mTexture);
					graphics.Get3D().ClearColorBuffer(new Color(0, 0));
					graphics.SetColor(new Color(-1));
					graphics.SetFont(mFont);
					Utils.SetFontLayerColor((ImageFont)mFont, 0, Bej3Widget.COLOR_INGAME_ANNOUNCEMENT);
					Utils.SetFontLayerColor((ImageFont)mFont, 1, Color.White);
					graphics.WriteString(mText, mTexture.GetWidth() / 2, mFont.GetAscent());
				}
				else if (mUpdateCnt >= mDuration)
				{
					mDeleteMe = true;
				}
			}
		}
	}
}
