using System.Collections.Generic;

namespace SexyFramework.Widget
{
	public class PASpriteDef
	{
		public string mName;

		public PAFrame[] mFrames;

		public int mWorkAreaStart;

		public int mWorkAreaDuration;

		public Dictionary<string, int> mLabels = new Dictionary<string, int>();

		public List<PAObjectDef> mObjectDefVector = new List<PAObjectDef>();

		public float mAnimRate;

		public virtual void Dispose()
		{
		}

		public int GetLabelFrame(string theLabel)
		{
			string key = theLabel.ToUpper();
			if (!mLabels.ContainsKey(key))
			{
				return -1;
			}
			return mLabels[key];
		}

		public void GetLabelFrameRange(string theLabel, ref int theStart, ref int theEnd)
		{
			theStart = GetLabelFrame(theLabel);
			theEnd = -1;
			if (theStart == -1)
			{
				return;
			}
			string text = theLabel.ToUpper();
			Dictionary<string, int>.Enumerator enumerator = mLabels.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (text != enumerator.Current.Key && enumerator.Current.Value > theStart && (theEnd < 0 || enumerator.Current.Value < theEnd))
				{
					theEnd = enumerator.Current.Value - 1;
				}
			}
			if (theEnd < 0)
			{
				theEnd = mFrames.Length - 1;
			}
		}
	}
}
