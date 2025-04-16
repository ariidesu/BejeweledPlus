using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class PointsEffect : Effect
	{
		private static SimpleObjectPool thePool_;

		public bool mShowNuggetText;

		public CurvedVal mCvY = new CurvedVal();

		public int mCount;

		public new int mPieceId;

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(PointsEffect));
		}

		public static PointsEffect alloc(int thePointCount, int thePieceId, bool theShowNuggetText)
		{
			PointsEffect pointsEffect = (PointsEffect)thePool_.alloc();
			pointsEffect.init(thePointCount, thePieceId, theShowNuggetText);
			return pointsEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}

		public PointsEffect()
			: base(Type.TYPE_CUSTOMCLASS)
		{
		}

		public void init(int thePointCount, int thePieceId, bool theShowNuggetText)
		{
			init(Type.TYPE_CUSTOMCLASS);
			mShowNuggetText = theShowNuggetText;
			mCount = thePointCount;
			mPieceId = thePieceId;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_ALPHA_DIG_GOAL_POINTS, mCurvedAlpha);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CV_Y_DIG_GOAL_POINTS, mCvY);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CURVED_SCALE_DIG_GOAL_POINTS, mCurvedScale);
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			g.SetScale((float)((double)mCurvedScale * (double)ConstantsWP.DIG_BOARD_FLOATING_SCORE_SCALE), (float)((double)mCurvedScale * (double)ConstantsWP.DIG_BOARD_FLOATING_SCORE_SCALE), GlobalMembers.S(mX), GlobalMembers.S(mY));
			g.SetFont(GlobalMembersResources.FONT_HUGE);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 0, Bej3Widget.COLOR_DIGBOARD_SCORE_GLOW);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_HUGE, 1, Bej3Widget.COLOR_DIGBOARD_SCORE_STROKE);
			g.SetColorizeImages(true);
			g.SetColor(Color.FAlpha((float)(double)mCurvedAlpha));
			string empty = string.Empty;
			empty = ((!mShowNuggetText) ? $"+{SexyFramework.Common.CommaSeperate(mCount)}" : $"+{SexyFramework.Common.CommaSeperate(mCount)} GOLD");
			int num = GlobalMembersResources.FONT_HUGE.StringWidth(empty) / 2;
			int num2 = (int)GlobalMembers.S(mX);
			int theX = ((num2 - num <= 0) ? num : ((num2 + num >= GlobalMembers.gApp.mWidth) ? (GlobalMembers.gApp.mWidth - num) : num2));
			int theY = (int)GlobalMembers.S((double)mY + (double)mCvY);
			g.WriteString(empty, theX, theY);
			g.PopState();
		}
	}
}
