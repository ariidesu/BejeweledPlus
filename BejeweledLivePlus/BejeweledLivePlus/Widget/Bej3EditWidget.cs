using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Widget
{
	public class Bej3EditWidget : EditWidget
	{
		public Bej3EditWidget(int theId, Bej3EditListener theEditListener)
			: base(theId, theEditListener)
		{
			SetFont(GlobalMembersResources.FONT_DIALOG);
			SetColor(0, new Color(0, 0, 0, 0));
			SetColor(1, new Color(0, 0, 0, 0));
			SetColor(2, Color.White);
			SetColor(3, Color.White);
			SetColor(4, new Color(100, 100, 100));
			mCursorOffset = (int)ModVal.M(-5f);
			SetText("");
			mTextInset = ConstantsWP.EDITWIDGET_BOX_PADDING_X;
			mClipInset = ConstantsWP.EDITWIDGET_CURSOR_OFFSET;
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theWidth, ConstantsWP.EDITWIDGET_HEIGHT);
		}

		public override void Draw(Graphics g)
		{
			g.mClipRect.mX -= ConstantsWP.EDITWIDGET_BACKGROUND_OFFSET;
			g.mClipRect.mWidth += ConstantsWP.EDITWIDGET_BACKGROUND_OFFSET * 2;
			g.DrawImageBox(new Rect(-ConstantsWP.EDITWIDGET_BACKGROUND_OFFSET, 0, mWidth + ConstantsWP.EDITWIDGET_BACKGROUND_OFFSET * 2, mHeight), GlobalMembersResourcesWP.IMAGE_DIALOG_TEXTBOX);
			Utils.SetFontLayerColor((ImageFont)mFont, 0, Bej3Widget.COLOR_DIALOG_WHITE);
			base.Draw(g);
		}

		public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
		{
			SetFocus(this);
			GotFocus();
			base.MouseDown(x, y, theBtnNum, theClickCount);
		}

		public override void KeyDown(KeyCode theKey)
		{
			base.KeyDown(theKey);
		}

		public override void GotFocus()
		{
			base.GotFocus();
			Bej3EditListener bej3EditListener = (Bej3EditListener)mEditListener;
			bej3EditListener.EditWidgetGotFocus(mId);
		}

		public override void LostFocus()
		{
			base.LostFocus();
			Bej3EditListener bej3EditListener = (Bej3EditListener)mEditListener;
			bej3EditListener.EditWidgetLostFocus(mId);
		}
	}
}
