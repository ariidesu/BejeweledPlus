using System.Text;

namespace SexyFramework.Resource
{
	public class SingleDataElement : DataElement
	{
		public StringBuilder mString = new StringBuilder();

		public DataElement mValue;

		public SingleDataElement()
		{
			mIsList = false;
			mValue = null;
		}

		public SingleDataElement(string theString)
		{
			mString = new StringBuilder(theString);
			mIsList = false;
			mValue = null;
		}

		public override void Dispose()
		{
			if (mValue != null && mValue != null)
			{
				mValue.Dispose();
			}
			base.Dispose();
		}

		public override DataElement Duplicate()
		{
			SingleDataElement singleDataElement = new SingleDataElement();
			singleDataElement.mString = mString;
			if (mValue != null)
			{
				singleDataElement.mValue = mValue.Duplicate();
			}
			return singleDataElement;
		}
	}
}
