namespace SexyFramework.Misc
{
	public class Ratio
	{
		public int mNumerator;

		public int mDenominator;

		public Ratio()
		{
			mNumerator = 0;
			mDenominator = 0;
		}

		public Ratio(int theNumerator, int theDenominator)
		{
			Set(theNumerator, theDenominator);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Ratio)
			{
				Ratio ratio = (Ratio)obj;
				if (mNumerator == ratio.mNumerator)
				{
					return mDenominator == ratio.mDenominator;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Set(int theNumerator, int theDenominator)
		{
			int num = theNumerator;
			int num2 = theDenominator;
			while (num2 != 0)
			{
				int num3 = num2;
				num2 = num % num2;
				num = num3;
			}
			mNumerator = theNumerator / num;
			mDenominator = theDenominator / num;
		}

		public static bool operator ==(Ratio ImpliedObject, Ratio theRatio)
		{
			return ImpliedObject?.Equals(theRatio) ?? ((object)theRatio == null);
		}

		public static bool operator !=(Ratio ImpliedObject, Ratio theRatio)
		{
			return !(ImpliedObject == theRatio);
		}

		public static bool operator <(Ratio ImpliedObject, Ratio theRatio)
		{
			if (ImpliedObject.mNumerator * theRatio.mDenominator / ImpliedObject.mDenominator >= theRatio.mNumerator)
			{
				return ImpliedObject.mNumerator < theRatio.mNumerator * ImpliedObject.mDenominator / theRatio.mDenominator;
			}
			return true;
		}

		public static bool operator >(Ratio ImpliedObject, Ratio theRatio)
		{
			if (ImpliedObject.mNumerator * theRatio.mDenominator / ImpliedObject.mDenominator <= theRatio.mNumerator)
			{
				return ImpliedObject.mNumerator > theRatio.mNumerator * ImpliedObject.mDenominator / theRatio.mDenominator;
			}
			return true;
		}

		public static int operator *(Ratio ImpliedObject, int t)
		{
			return t * ImpliedObject.mNumerator / ImpliedObject.mDenominator;
		}

		public static int operator *(Ratio ImpliedObject, float t)
		{
			return (int)(t * (float)ImpliedObject.mNumerator / (float)ImpliedObject.mDenominator);
		}

		public static int operator *(Ratio ImpliedObject, double t)
		{
			return (int)(t * (double)ImpliedObject.mNumerator / (double)ImpliedObject.mDenominator);
		}

		public static int operator /(Ratio ImpliedObject, int t)
		{
			return t * ImpliedObject.mDenominator / ImpliedObject.mNumerator;
		}

		public static int operator /(Ratio ImpliedObject, float t)
		{
			return (int)(t * (float)ImpliedObject.mDenominator / (float)ImpliedObject.mNumerator);
		}

		public static int operator /(Ratio ImpliedObject, double t)
		{
			return (int)(t * (double)ImpliedObject.mDenominator / (double)ImpliedObject.mNumerator);
		}
	}
}
