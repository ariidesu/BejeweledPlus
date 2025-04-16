using System.Collections.Generic;
using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3HyperlinkWidget : HyperlinkWidget
	{
		private List<Color> mLayerColours = new List<Color>();

		private int mUnderlineHeight;

		public Bej3HyperlinkWidget(int theId, ButtonListener theButtonListener)
			: base(theId, theButtonListener)
		{
			mUnderlineHeight = 1;
		}

		public override void Draw(Graphics g)
		{
			g.SetColorizeImages(true);
			for (int i = 0; i < mLayerColours.Count; i++)
			{
				Utils.SetFontLayerColor((ImageFont)mFont, i, mLayerColours[i]);
			}
			int theX = (mWidth - mFont.StringWidth(mLabel)) / 2;
			int num = (mHeight + mFont.GetAscent()) / 2 - 1;
			g.SetColor(mColor);
			g.SetFont(mFont);
			g.DrawString(mLabel, theX, num);
			for (int j = 0; j < mUnderlineSize; j++)
			{
				g.FillRect(theX, num + mUnderlineOffset + j, mFont.StringWidth(mLabel), mUnderlineHeight);
			}
		}

		public override void SetFont(Font theFont)
		{
			base.SetFont(theFont);
			mLayerColours.Clear();
			int layerCount = ((ImageFont)mFont).GetLayerCount();
			for (int i = 0; i < layerCount; i++)
			{
				mLayerColours.Add(Color.White);
			}
			mUnderlineHeight = 1;
			if (theFont == GlobalMembersResources.FONT_HUGE)
			{
				mLayerColours[0] = Bej3Widget.COLOR_HEADING_GLOW_1;
			}
			else if (theFont == GlobalMembersResources.FONT_SUBHEADER)
			{
				mLayerColours[1] = Bej3Widget.COLOR_SUBHEADING_1_FILL;
				mLayerColours[0] = Bej3Widget.COLOR_SUBHEADING_1_STROKE;
			}
			else if (theFont == GlobalMembersResources.FONT_DIALOG)
			{
				mLayerColours[0] = Bej3Widget.COLOR_DIALOG_1_FILL;
			}
		}

		public void SetLayerColor(int layer, Color colour)
		{
			mLayerColours[layer] = colour;
		}
	}
}
