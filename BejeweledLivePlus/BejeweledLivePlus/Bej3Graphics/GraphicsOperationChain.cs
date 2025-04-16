using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class GraphicsOperationChain
	{
		private const int OPERATION_MAX = 120000;

		private GraphicsOperation[] content_ = new GraphicsOperation[120000];

		private int head_;

		private int next_;

		private int count_;

		private GraphicsOperationRef operationRef_;

		public GraphicsOperationChain()
		{
			operationRef_ = new GraphicsOperationRef();
		}

		public GraphicsOperationRef alloc(Graphics g, GraphicsOperation.IMAGE_TYPE type, int timestamp)
		{
			int num = 0;
			if (count_ < 120000)
			{
				count_++;
				num = next_++;
				if (next_ >= 120000)
				{
					next_ -= 120000;
				}
			}
			else
			{
				num = head_++;
				if (head_ >= 120000)
				{
					head_ -= 120000;
				}
				next_ = head_;
			}
			content_[num].mType = type;
			content_[num].mDrawMode = g.mDrawMode;
			content_[num].mColor = (g.mColorizeImages ? g.mColor : Color.White);
			content_[num].mTimestamp = timestamp;
			operationRef_.prepare(content_, num);
			return operationRef_;
		}

		public int count()
		{
			return count_;
		}

		public void executeFrom(int timestamp, Graphics g)
		{
			bool flag = false;
			for (int i = 0; i < count_; i++)
			{
				int num = head_ + i;
				if (num >= 120000)
				{
					num -= 120000;
				}
				if (!flag)
				{
					if (content_[num].mTimestamp >= timestamp)
					{
						flag = true;
						i--;
					}
				}
				else
				{
					content_[num].Execute(g);
				}
			}
		}

		public void clearFrom(int timestamp)
		{
			int num = 0;
			for (int num2 = count_ - 1; num2 >= 0; num2--)
			{
				int num3 = head_ + num2;
				if (num3 >= 120000)
				{
					num3 -= 120000;
				}
				if (content_[num3].mTimestamp < timestamp)
				{
					break;
				}
				num++;
			}
			count_ -= num;
			next_ -= num;
			if (next_ < 0)
			{
				next_ += 120000;
			}
		}

		public void clearTo(int timestamp)
		{
			int num = 0;
			for (int i = 0; i < count_; i++)
			{
				int num2 = i + head_;
				if (num2 >= 120000)
				{
					num2 -= 120000;
				}
				GraphicsOperation graphicsOperation = content_[num2];
				if (graphicsOperation.mTimestamp > timestamp)
				{
					break;
				}
				num++;
			}
			count_ -= num;
			head_ += num;
			if (head_ >= 120000)
			{
				head_ -= 120000;
			}
		}

		public int lastTimestamp()
		{
			int result = -1;
			if (count_ > 0)
			{
				int num = next_ - 1;
				if (num < 0)
				{
					num += 120000;
				}
				result = content_[num].mTimestamp;
			}
			return result;
		}

		public int firstTimestamp()
		{
			int result = -1;
			if (count_ > 0)
			{
				result = content_[head_].mTimestamp;
			}
			return result;
		}
	}
}
