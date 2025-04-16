using System.Collections.Generic;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.AELib
{
	public class AECommon
	{
		public delegate void PostLoadCompImageFunc(SharedImageRef img, Layer l);

		public delegate SharedImageRef LoadCompImageFunc(string file_dir, string file_name);

		public delegate void PreLayerDrawFunc(SexyFramework.Graphics.Graphics g, Layer l, object data);

		public const int PAX_VERSION = 5;

		public const int COMPOSITION_HEADER = 1;

		public const int LAYER_HEADER = 2;

		public const int ANCHOR_POINT_HEADER = 3;

		public const int POSITION_HEADER = 4;

		public const int SCALE_HEADER = 5;

		public const int ROTATION_HEADER = 6;

		public const int OPACITY_HEADER = 7;

		public const int KEYFRAME_MARKER = 8;

		public const int END_OF_LAYER = 9;

		public const int END_OF_POPFX = 9999999;

		public const int LOOP_REPEAT = 10;

		public const int LOOP_PINGPONG = 11;

		public static bool LoadPAX(string file_name, List<Composition> compositions, LoadCompImageFunc load_img_func, PostLoadCompImageFunc post_load_img_func)
		{
			Buffer theBuffer = new Buffer();
			GlobalMembers.gSexyAppBase.ReadBufferFromStream(file_name + ".pax", ref theBuffer);
			ParseByteArray parseByteArray = new ParseByteArray(theBuffer.GetDataPtr());
			if (parseByteArray.isEnd())
			{
				return false;
			}
			int value = 0;
			parseByteArray.readInt32(ref value);
			int value2 = 0;
			parseByteArray.readInt32(ref value2);
			List<FootageDescriptor> list = new List<FootageDescriptor>();
			for (int i = 0; i < value2; i++)
			{
				int value3 = 0;
				parseByteArray.readInt32(ref value3);
				string value4 = "";
				parseByteArray.readString(ref value4, value3);
				long value5 = 0L;
				parseByteArray.readLong(ref value5);
				parseByteArray.readInt32(ref value3);
				string value6 = "";
				parseByteArray.readString(ref value6, value3);
				long value7 = 0L;
				long value8 = 0L;
				parseByteArray.readLong(ref value7);
				parseByteArray.readLong(ref value8);
				string[] array = new string[6] { ".psd", ".png", ".jp2", ".gif", ".jpg", ".j2k" };
				string text = value6.ToLower();
				bool flag = false;
				for (int j = 0; j < array.Length; j++)
				{
					if (text.Contains(array[j]))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					list.Add(new FootageDescriptor(value4, value5, value6, value7, value8));
				}
			}
			while (!parseByteArray.isEnd())
			{
				Composition composition = new Composition();
				int value9 = 0;
				parseByteArray.readInt32(ref value9);
				if (value9 != 1)
				{
					break;
				}
				compositions.Add(composition);
				int value10 = 0;
				parseByteArray.readInt32(ref value10);
				parseByteArray.readString(ref composition.mLayerName, value10);
				long value11 = 0L;
				long value12 = 0L;
				parseByteArray.readLong(ref value11);
				parseByteArray.readLong(ref value12);
				composition.mWidth = (int)value11;
				composition.mHeight = (int)value12;
				int value13 = 0;
				parseByteArray.readInt32(ref value13);
				int value14 = 0;
				parseByteArray.readInt32(ref value14);
				composition.SetMaxDuration(value14);
				for (int k = 0; k < value13; k++)
				{
					parseByteArray.readInt32(ref value9);
					bool value15 = false;
					parseByteArray.readBoolean(ref value15);
					if (!value15)
					{
						continue;
					}
					bool value16 = false;
					long value17 = 0L;
					parseByteArray.readBoolean(ref value16);
					parseByteArray.readLong(ref value17);
					int value18 = 0;
					parseByteArray.readInt32(ref value18);
					string value19 = "";
					parseByteArray.readString(ref value19, value18);
					value18 = 0;
					string value20 = "";
					parseByteArray.readInt32(ref value18);
					parseByteArray.readString(ref value20, value18);
					int value21 = 0;
					parseByteArray.readInt32(ref value21);
					int value22 = 0;
					parseByteArray.readInt32(ref value22);
					int value23 = 0;
					parseByteArray.readInt32(ref value23);
					int value24 = 0;
					parseByteArray.readInt32(ref value24);
					bool value25 = false;
					if (value >= 5)
					{
						parseByteArray.readBoolean(ref value25);
					}
					Layer layer = null;
					layer = ((!value16) ? new Composition() : new Layer());
					layer.mLayerName = value20;
					layer.mAdditive = value25;
					string text2 = value20;
					if (value16)
					{
						for (int l = 0; l < list.Count; l++)
						{
							if (list[l].mId == value17)
							{
								text2 = list[l].mFullName;
								int num = list[l].mShortName.IndexOf('/');
								value19 = ((num == -1) ? "" : list[l].mShortName.Substring(0, num));
								layer.mWidth = (int)list[l].mWidth;
								layer.mHeight = (int)list[l].mHeight;
								break;
							}
						}
						int num2 = text2.LastIndexOf('\\');
						if (num2 == -1)
						{
							num2 = text2.LastIndexOf('/');
						}
						if (num2 != -1)
						{
							text2 = text2.Substring(num2 + 1);
						}
						text2 = Common.Trim(text2);
						int num3 = text2.LastIndexOf('.');
						if (num3 != -1)
						{
							text2 = text2.Substring(0, num3);
						}
						if (value19.Length > 0)
						{
							text2 = text2 + "\\" + value19;
						}
						SharedImageRef sharedImageRef = load_img_func("images\\", text2);
						if (sharedImageRef == null || sharedImageRef.mWidth == 0 || sharedImageRef.mHeight == 0)
						{
							throw new ParseFileException("Image is null!");
						}
						layer.SetImage(sharedImageRef);
						post_load_img_func(sharedImageRef, layer);
					}
					parseByteArray.readInt32(ref value9);
					while (value9 != 9)
					{
						double value26 = 0.0;
						double value27 = 0.0;
						parseByteArray.readDouble(ref value26);
						parseByteArray.readDouble(ref value27);
						int value28 = 0;
						parseByteArray.readInt32(ref value28);
						long value29 = 0L;
						parseByteArray.readLong(ref value29);
						bool value30 = false;
						int value31 = 0;
						int value32 = 0;
						parseByteArray.readBoolean(ref value30);
						parseByteArray.readInt32(ref value31);
						parseByteArray.readInt32(ref value32);
						if (value30)
						{
							switch (value9)
							{
							case 3:
								layer.mAnchorPoint.mLoopFrame = value32;
								layer.mAnchorPoint.mLoopType = value31;
								break;
							case 4:
								layer.mPosition.mLoopFrame = value32;
								layer.mPosition.mLoopType = value31;
								break;
							case 5:
								layer.mScale.mLoopFrame = value32;
								layer.mScale.mLoopType = value31;
								break;
							case 6:
								layer.mRotation.mLoopFrame = value32;
								layer.mRotation.mLoopType = value31;
								break;
							case 7:
								layer.mOpacity.mLoopFrame = value32;
								layer.mOpacity.mLoopType = value31;
								break;
							}
						}
						int value33 = 0;
						if (value28 > 0)
						{
							parseByteArray.readInt32(ref value33);
							value29 = 0L;
						}
						for (long num4 = -1L; num4 < value29; num4++)
						{
							int value34 = 0;
							if (num4 >= 0)
							{
								parseByteArray.readInt32(ref value34);
							}
							int value35 = 0;
							double value36 = 0.0;
							double value37 = 0.0;
							if (num4 >= 0)
							{
								parseByteArray.readInt32(ref value35);
								parseByteArray.readDouble(ref value36);
								parseByteArray.readDouble(ref value37);
								value35 *= (int)((float)value23 / (float)value22);
							}
							else
							{
								value35 = 0;
								value36 = value26;
								value37 = value27;
							}
							switch (value9)
							{
							case 3:
								layer.AddAnchorPoint(value35, (float)value36, (float)value37);
								break;
							case 4:
								layer.AddPosition(value35, (float)value36, (float)value37);
								break;
							case 5:
								layer.AddScale(value35, (float)value36 / 100f, (float)value37 / 100f);
								break;
							case 6:
								layer.AddRotation(value35, (float)value36 * 3.14159f / 180f);
								break;
							case 7:
								layer.AddOpacity(value35, (float)value36 / 100f);
								break;
							}
						}
						for (int m = 0; m < value28; m++)
						{
							double value38 = 0.0;
							double value39 = 0.0;
							parseByteArray.readDouble(ref value38);
							parseByteArray.readDouble(ref value39);
							switch (value9)
							{
							case 3:
								layer.AddAnchorPoint(value33 + k, (float)value38, (float)value39);
								break;
							case 4:
								layer.AddPosition(value33 + k, (float)value38, (float)value39);
								break;
							case 5:
								layer.AddScale(value33 + k, (float)value38 / 100f, (float)value39 / 100f);
								break;
							case 6:
								layer.AddRotation(value33 + k, (float)value38 * 3.14159f / 180f);
								break;
							case 7:
								layer.AddOpacity(value33 + k, (float)value38 / 100f);
								break;
							}
						}
						parseByteArray.readInt32(ref value9);
					}
					layer.EnsureTimelineDefaults(value11, value12);
					composition.AddLayer(layer, value21, (value23 > value14) ? value14 : value23, value24);
				}
				composition.EnsureTimelineDefaults(value11, value12);
			}
			for (int n = 0; n < compositions.Count; n++)
			{
				Composition composition2 = compositions[n];
				for (int num5 = 0; num5 < composition2.GetNumLayers(); num5++)
				{
					Layer layerAtIdx = composition2.GetLayerAtIdx(num5);
					for (int num6 = 0; num6 < compositions.Count; num6++)
					{
						Composition composition3 = compositions[num6];
						if (composition3.mLayerName.ToLower().Equals(layerAtIdx.mLayerName.ToLower()))
						{
							Composition composition4 = layerAtIdx as Composition;
							if (composition4 != null)
							{
								composition4.CopyLayersFrom(composition3);
								composition4.mWidth = composition3.mWidth;
								composition4.mHeight = composition3.mHeight;
							}
							break;
						}
					}
				}
			}
			return true;
		}
	}
}
