using System;
using System.Collections.Generic;

namespace SexyFramework.Misc
{
	internal class EquationSystem
	{
		public List<float> eqs = new List<float>();

		public List<float> sol = new List<float>();

		public int mRowSize;

		public int mCurRow;

		public EquationSystem(int theNumVariables)
		{
			mRowSize = theNumVariables + 1;
			mCurRow = 0;
			Common.Resize(eqs, mRowSize * theNumVariables);
			Common.Resize(sol, theNumVariables);
		}

		public void SetCoefficient(int theRow, int theCol, float theValue)
		{
			int index = mRowSize * theRow + theCol;
			eqs[index] = theValue;
		}

		public void SetConstantTerm(int theRow, float theValue)
		{
			int index = mRowSize * theRow + mRowSize - 1;
			eqs[index] = theValue;
		}

		public void SetCoefficient(int theCol, float theValue)
		{
			SetCoefficient(mCurRow, theCol, theValue);
		}

		public void SetConstantTerm(float theValue)
		{
			SetConstantTerm(mCurRow, theValue);
		}

		public void NextEquation()
		{
			mCurRow++;
		}

		public void Solve()
		{
			int num = mRowSize;
			int num2 = mRowSize - 1;
			for (int i = 0; i < num2; i++)
			{
				int num3 = i;
				for (int j = i + 1; j < num2; j++)
				{
					if (Math.Abs(eqs[j * num + i]) > Math.Abs(eqs[num3 * num + i]))
					{
						num3 = j;
					}
				}
				for (int k = 0; k < num2 + 1; k++)
				{
					float value = eqs[i * num + k];
					eqs[i * num + k] = eqs[num3 * num + k];
					eqs[num3 * num + k] = value;
				}
				for (int j = i + 1; j < num2; j++)
				{
					float num4 = eqs[j * num + i] / eqs[i * num + i];
					if (num4 != 0f)
					{
						for (int k = num2; k >= i; k--)
						{
							eqs[j * num + k] -= eqs[i * num + k] * num4;
						}
					}
				}
			}
			for (int j = num2 - 1; j >= 0; j--)
			{
				float num5 = 0f;
				for (int k = j + 1; k < num2; k++)
				{
					num5 += eqs[j * num + k] * sol[k];
				}
				sol[j] = (eqs[j * num + num2] - num5) / eqs[j * num + j];
			}
		}
	}
}
