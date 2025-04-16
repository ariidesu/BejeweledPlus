using System.Text;

namespace SexyFramework.Misc
{
	public class MTRand
	{
		public static int MTRAND_N = 624;

		public static int MTRAND_M = 397;

		public static uint MATRIX_A = 2567483615u;

		public static uint UPPER_MASK = 2147483648u;

		public static uint LOWER_MASK = 2147483647u;

		public static uint TEMPERING_MASK_B = 2636928640u;

		public static uint TEMPERING_MASK_C = 4022730752u;

		public static uint[] mag01 = new uint[2] { 0u, MATRIX_A };

		private uint[] mt = new uint[MTRAND_N];

		private int mti;

		public MTRand(string theSerialData)
		{
			SRand(theSerialData);
			mti = MTRAND_N + 1;
		}

		public MTRand(uint seed)
		{
			SRand(seed);
		}

		public MTRand()
		{
			SRand(4357u);
		}

		public void SRand(string theSerialData)
		{
			string[] array = theSerialData.Split('|');
			if (array.Length == MTRAND_N + 1)
			{
				mti = int.Parse(array[0]);
				for (int i = 0; i < MTRAND_N; i++)
				{
					mt[i] = uint.Parse(array[i + 1]);
				}
			}
			else
			{
				SRand(4357u);
			}
		}

		public void SRand(uint seed)
		{
			if (seed == 0)
			{
				seed = 4357u;
			}
			mt[0] = seed & 0xFFFFFFFFu;
			for (mti = 1; mti < MTRAND_N; mti++)
			{
				mt[mti] = 1812433253 * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + (uint)mti;
				mt[mti] &= uint.MaxValue;
			}
		}

		private uint NextNoAssert()
		{
			uint num;
			if (mti >= MTRAND_N)
			{
				int i;
				for (i = 0; i < MTRAND_N - MTRAND_M; i++)
				{
					num = (mt[i] & UPPER_MASK) | (mt[i + 1] & LOWER_MASK);
					mt[i] = mt[i + MTRAND_M] ^ (num >> 1) ^ mag01[(ulong)num & 1uL];
				}
				for (; i < MTRAND_N - 1; i++)
				{
					num = (mt[i] & UPPER_MASK) | (mt[i + 1] & LOWER_MASK);
					mt[i] = mt[i + (MTRAND_M - MTRAND_N)] ^ (num >> 1) ^ mag01[(ulong)num & 1uL];
				}
				num = (mt[MTRAND_N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
				mt[MTRAND_N - 1] = mt[MTRAND_M - 1] ^ (num >> 1) ^ mag01[(ulong)num & 1uL];
				mti = 0;
			}
			num = mt[mti++];
			num ^= num >> 11;
			num ^= (num << 7) & TEMPERING_MASK_B;
			num ^= (num << 15) & TEMPERING_MASK_C;
			num ^= num >> 18;
			return num & 0x7FFFFFFF;
		}

		public uint Next()
		{
			return NextNoAssert();
		}

		public uint NextNoAssert(uint range)
		{
			return NextNoAssert() % range;
		}

		public uint Next(uint range)
		{
			return NextNoAssert(range);
		}

		public float NextNoAssert(float range)
		{
			return (float)((double)NextNoAssert() / 2147483647.0 * (double)range);
		}

		public float Next(float range)
		{
			return NextNoAssert(range);
		}

		public string Serialize()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}", new object[1] { mti });
			for (int i = 0; i < MTRAND_N; i++)
			{
				stringBuilder.AppendFormat("|{0}", new object[1] { mt[i] });
			}
			return stringBuilder.ToString();
		}

		public static void SetRandAllowed(bool allowed)
		{
		}
	}
}
