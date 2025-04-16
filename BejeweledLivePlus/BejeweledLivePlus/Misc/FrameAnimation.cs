using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Widget;

namespace BejeweledLivePlus.Misc
{
	public class FrameAnimation : SexyFramework.Widget.Widget
	{
		protected Image mImage;

		protected List<Rect> mRectInAtlas;

		protected Rect mRectDest;

		protected int mCurFrame;

		public FrameAnimation()
		{
		}

		public override void Update()
		{
			base.Update();
			if (mRectInAtlas != null && mRectInAtlas.Count > 0 && mUpdateCnt % 15 == 0)
			{
				mCurFrame++;
				mCurFrame %= mRectInAtlas.Count;
			}
		}

		public override void Draw(Graphics g)
		{
			if (mImage != null)
			{
				g.DrawImage(mImage, mRectDest, mRectInAtlas[mCurFrame]);
			}
		}

		public override void Resize(int theX, int theY, int theWidth, int theHeight)
		{
			base.Resize(theX, theY, theX + theWidth, theY + theHeight);
			mRectDest = new Rect(theX, theY, theWidth, theHeight);
		}

		public FrameAnimation(Image atlasImage, string atlasPlist)
		{
			XMLParser xMLParser = new XMLParser();
			Stream stream = null;
			try
			{
				stream = TitleContainer.OpenStream("Content\\" + atlasPlist);
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, (int)stream.Length);
				stream.Close();
				xMLParser.checkEncodingType(array);
				xMLParser.SetBytes(array);
			}
			catch (Exception)
			{
				return;
			}
			mImage = atlasImage;
			mRectDest = new Rect(mX, mY, mWidth, mHeight);
			mRectInAtlas = new List<Rect>();
			string text = null;
			bool flag = false;
			XMLElement xMLElement = new XMLElement();
			while (!xMLParser.HasFailed() && xMLParser.NextElement(xMLElement))
			{
				if (xMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
				{
					text = xMLElement.mValue.ToString();
					if (text == "frame")
					{
						flag = true;
					}
					else if (flag)
					{
						Rect item = RectFromString(text);
						mRectInAtlas.Add(item);
						flag = false;
					}
				}
			}
		}

		protected void Parse(string filename)
		{
		}

		protected static Rect RectFromString(string pszContent)
		{
			Rect result = Rect.ZERO_RECT;
			if (pszContent != null)
			{
				string text = pszContent;
				int num = text.IndexOf('{');
				int num2 = text.IndexOf('}');
				for (int i = 1; i < 3; i++)
				{
					if (num2 == -1)
					{
						break;
					}
					num2 = text.IndexOf('}', num2 + 1);
				}
				if (num != -1 && num2 != -1)
				{
					text = text.Substring(num + 1, num2 - num - 1);
					int num3 = text.IndexOf('}');
					if (num3 != -1)
					{
						num3 = text.IndexOf(',', num3);
						if (num3 != -1)
						{
							string pStr = text.Substring(0, num3);
							string pStr2 = text.Substring(num3 + 1);
							List<string> strs = new List<string>();
							if (splitWithForm(pStr, ref strs))
							{
								List<string> strs2 = new List<string>();
								if (splitWithForm(pStr2, ref strs2))
								{
									float num4 = (float)Convert.ToDouble(strs[0]);
									float num5 = (float)Convert.ToDouble(strs[1]);
									float num6 = (float)Convert.ToDouble(strs2[0]);
									float num7 = (float)Convert.ToDouble(strs2[1]);
									result = new Rect((int)num4, (int)num5, (int)num6, (int)num7);
								}
							}
						}
					}
				}
			}
			return result;
		}

		protected static bool splitWithForm(string pStr, ref List<string> strs)
		{
			bool result = false;
			if (pStr != null)
			{
				if (pStr.Length != 0)
				{
					int num = pStr.IndexOf('{');
					int num2 = pStr.IndexOf('}');
					if (num != -1 && num2 != -1 && num <= num2)
					{
						string text = pStr.Substring(num + 1, num2 - num - 1);
						if (text.Length != 0)
						{
							int num3 = text.IndexOf('{');
							int num4 = text.IndexOf('}');
							if (num3 == -1 && num4 == -1)
							{
								split(text, ",", ref strs);
								if (strs.Count != 2 || strs[0].Length == 0 || strs[1].Length == 0)
								{
									strs.Clear();
								}
								else
								{
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		protected static void split(string src, string token, ref List<string> vect)
		{
			string[] array = src.Split(token.ToCharArray());
			string[] array2 = array;
			foreach (string item in array2)
			{
				vect.Add(item);
			}
		}
	}
}
