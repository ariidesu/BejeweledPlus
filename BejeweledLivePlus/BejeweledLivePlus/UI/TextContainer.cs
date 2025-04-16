using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace BejeweledLivePlus.UI
{
	internal class TextContainer : SexyFramework.Widget.Widget, ScrollWidgetListener
	{
		public Label mText;

		public TextContainer(string theText, int textWidth)
		{
			mText = new Label(GlobalMembersResources.FONT_DIALOG);
			mText.SetText(theText);
			mText.mLineSpacingOffset = ConstantsWP.HINTDIALOG_TEXT_LINE_SPACING_ADJUST;
			Graphics graphics = new Graphics();
			graphics.SetFont(mText.GetFont());
			int theMaxWidth = 0;
			int theLineCount = 0;
			int wordWrappedHeight = graphics.GetWordWrappedHeight(textWidth, mText.GetText(), mText.GetFont().GetLineSpacing() - ConstantsWP.HINTDIALOG_TEXT_LINE_SPACING_ADJUST, ref theMaxWidth, ref theLineCount);
			mText.SetTextBlock(new Rect(0, 0, textWidth, wordWrappedHeight), true);
			mText.SetTextBlockAlignment(0);
			mText.SetTextBlockEnabled(true);
			AddWidget(mText);
			Resize(mText.GetRect());
		}

		public virtual void ScrollTargetReached(ScrollWidget scrollWidget)
		{
		}

		public virtual void ScrollTargetInterrupted(ScrollWidget scrollWidget)
		{
		}
	}
}
