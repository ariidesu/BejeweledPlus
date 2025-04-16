using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Misc
{
	public class Messager
	{
		public enum EJustification
		{
			eJustification_Left,
			eJustification_Right
		}

		public class Msg
		{
			public string Text = string.Empty;

			public double LifeLeft;

			public int TextColor;
		}

		private double Draw_fadeAt = ModVal.M(0.5f);

		public Font mFont;

		public List<Msg> mMessages = new List<Msg>();

		protected EJustification mJustification;

		protected double mDefaultLife;

		protected int mDefaultColor;

		public Messager()
		{
			mFont = null;
			mDefaultLife = 1.0;
			mJustification = EJustification.eJustification_Left;
		}

		public void Init(Font i_font, int iOpt_defaultColor)
		{
			Init(i_font, iOpt_defaultColor, 2.5);
		}

		public void Init(Font i_font)
		{
			Init(i_font, -1, 2.5);
		}

		public void Init(Font i_font, int iOpt_defaultColor, double iOpt_defaultLife)
		{
			mFont = i_font;
			mDefaultColor = iOpt_defaultColor;
			mDefaultLife = iOpt_defaultLife;
		}

		public void AddMessage(string i_msg, int iOpt_color)
		{
			AddMessage(i_msg, iOpt_color, -1.0);
		}

		public void AddMessage(string i_msg)
		{
			AddMessage(i_msg, -1, -1.0);
		}

		public void AddMessage(string i_msg, int iOpt_color, double iOpt_life)
		{
			Msg msg = new Msg();
			msg.LifeLeft = ((iOpt_life < 0.0) ? mDefaultLife : iOpt_life);
			msg.TextColor = ((iOpt_color < 0) ? mDefaultColor : iOpt_color);
			msg.Text = i_msg;
			mMessages.Add(msg);
		}

		public void Update()
		{
			Update(0.01);
		}

		public void Update(double i_deltaT)
		{
			for (int num = mMessages.Count - 1; num >= 0; num--)
			{
				mMessages[num].LifeLeft -= i_deltaT;
				if (mMessages[num].LifeLeft <= 0.0)
				{
					mMessages.RemoveAt(num);
				}
			}
		}

		public void Draw(Graphics g, int iOpt_x)
		{
			Draw(g, iOpt_x, 0);
		}

		public void Draw(Graphics g)
		{
			Draw(g, 0, 0);
		}

		public void Draw(Graphics g, int iOpt_x, int iOpt_y)
		{
			int num = iOpt_y;
			int num2 = iOpt_x;
			g.SetFont(mFont);
			for (int num3 = mMessages.Count - 1; num3 >= 0; num3--)
			{
				Msg msg = mMessages[num3];
				num -= g.GetFont().GetHeight();
				if (mJustification == EJustification.eJustification_Right)
				{
					num2 = -g.GetFont().StringWidth(msg.Text);
				}
				Color color = default(Color);
				Color color2 = default(Color);
				if (msg.LifeLeft < Draw_fadeAt)
				{
					g.SetColorizeImages(true);
					color = new Color(0, (int)(msg.LifeLeft / Draw_fadeAt * 255.0));
					color2 = new Color(msg.TextColor, (int)(msg.LifeLeft / Draw_fadeAt * 255.0));
				}
				else
				{
					color = new Color(0);
					color2 = new Color(msg.TextColor);
				}
				g.SetColor(color);
				g.DrawString(msg.Text, num2 + 1, num + 1);
				g.SetColor(color2);
				g.DrawString(msg.Text, num2, num);
				g.SetColorizeImages(false);
			}
			g.SetColor(new Color(-1));
		}

		public EJustification GetJustification()
		{
			return mJustification;
		}

		public void SetJustification(EJustification i_val)
		{
			mJustification = i_val;
		}
	}
}
