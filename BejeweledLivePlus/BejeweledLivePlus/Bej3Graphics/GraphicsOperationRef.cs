using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class GraphicsOperationRef
	{
		private GraphicsOperation[] opArray_;

		private int opIndex_;

		public int mTimestamp
		{
			get
			{
				return opArray_[opIndex_].mTimestamp;
			}
			set
			{
				opArray_[opIndex_].mTimestamp = value;
			}
		}

		public GraphicsOperation.IMAGE_TYPE mType
		{
			get
			{
				return opArray_[opIndex_].mType;
			}
			set
			{
				opArray_[opIndex_].mType = value;
			}
		}

		public Image mImage
		{
			get
			{
				return opArray_[opIndex_].mImage;
			}
			set
			{
				opArray_[opIndex_].mImage = value;
			}
		}

		public FRect mFRect
		{
			get
			{
				return opArray_[opIndex_].mFRect;
			}
			set
			{
				opArray_[opIndex_].mFRect = value;
			}
		}

		public Color mColor
		{
			get
			{
				return opArray_[opIndex_].mColor;
			}
			set
			{
				opArray_[opIndex_].mColor = value;
			}
		}

		public FRect mDestRect
		{
			get
			{
				return opArray_[opIndex_].mDestRect;
			}
			set
			{
				opArray_[opIndex_].mDestRect = value;
			}
		}

		public Rect mSrcRect
		{
			get
			{
				return opArray_[opIndex_].mSrcRect;
			}
			set
			{
				opArray_[opIndex_].mSrcRect = value;
			}
		}

		public int mDrawMode
		{
			get
			{
				return opArray_[opIndex_].mDrawMode;
			}
			set
			{
				opArray_[opIndex_].mDrawMode = value;
			}
		}

		public float mFloat
		{
			get
			{
				return opArray_[opIndex_].mFloat;
			}
			set
			{
				opArray_[opIndex_].mFloat = value;
			}
		}

		public GraphicsOperationRef()
		{
			opArray_ = null;
			opIndex_ = -1;
		}

		public void prepare(GraphicsOperation[] arr, int idx)
		{
			opArray_ = arr;
			opIndex_ = idx;
		}
	}
}
