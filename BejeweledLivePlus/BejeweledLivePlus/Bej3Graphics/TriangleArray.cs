using SexyFramework.Graphics;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class TriangleArray
	{
		private SexyVertex2D[,] vertices_;

		private int capacity_;

		private int next_;

		public TriangleArray()
		{
			vertices_ = new SexyVertex2D[256, 3];
			capacity_ = 256;
			next_ = 0;
		}

		public void add(SexyVertex2D v1, SexyVertex2D v2, SexyVertex2D v3)
		{
			if (next_ >= capacity_)
			{
				expand();
			}
			vertices_[next_, 0] = v1;
			vertices_[next_, 1] = v2;
			vertices_[next_, 2] = v3;
			next_++;
		}

		public SexyVertex2D[,] toArray()
		{
			return vertices_;
		}

		public int count()
		{
			return next_;
		}

		public void clear()
		{
			next_ = 0;
		}

		private void expand()
		{
			SexyVertex2D[,] array = new SexyVertex2D[capacity_ + 256, 3];
			for (int i = 0; i < next_; i++)
			{
				array[i, 0] = vertices_[i, 0];
				array[i, 1] = vertices_[i, 1];
				array[i, 2] = vertices_[i, 2];
			}
			vertices_ = array;
			capacity_ += 256;
		}
	}
}
