using System.Collections.Generic;

namespace SexyFramework.Resource
{
	public class ListDataElement : DataElement
	{
		public List<DataElement> mElementVector = new List<DataElement>();

		public ListDataElement()
		{
			mIsList = true;
		}

		public ListDataElement(ListDataElement theListDataElement)
		{
			mIsList = true;
			for (int i = 0; i < theListDataElement.mElementVector.Count; i++)
			{
				mElementVector.Add(theListDataElement.mElementVector[i].Duplicate());
			}
		}

		public override void Dispose()
		{
			for (int i = 0; i < mElementVector.Count; i++)
			{
				if (mElementVector[i] != null)
				{
					mElementVector[i].Dispose();
				}
			}
			base.Dispose();
		}

		public ListDataElement CopyFrom(ListDataElement theListDataElement)
		{
			for (int i = 0; i < mElementVector.Count; i++)
			{
				if (mElementVector[i] != null)
				{
					mElementVector[i].Dispose();
				}
			}
			mElementVector.Clear();
			for (int j = 0; j < theListDataElement.mElementVector.Count; j++)
			{
				mElementVector.Add(theListDataElement.mElementVector[j].Duplicate());
			}
			return this;
		}

		public override DataElement Duplicate()
		{
			return new ListDataElement(this);
		}
	}
}
