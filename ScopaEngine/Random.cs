using System;

namespace NIESoftware {

	/// <summary>
	/// This class is an implementation of the Mastumoto & Nishimura Mersenne Twister.
	/// </summary>
	sealed class Random {
		private const ulong LOWER_MASK			= 0x7fffffff;
		private const ulong TEMPERING_MASK_B	= 0x9d2c5680;
		private const ulong TEMPERING_MASK_C	= 0xefc60000;
		private const ulong MATRIX_A			= 0x9908b0df;
		private const ulong UPPER_MASK			= 0x80000000;
		private const int M						= 397;
		private const int N						= 624;

		private static readonly ulong[] mag01		= new ulong[2] { 0x0, MATRIX_A, };
		private static /*volatile*/ ulong unique	= 8682522807148012L;

		private static Random rand = new Random();

		private int k = 1;
		private ulong[] ptgfsr = new ulong[N];

		public Random() : this(++unique + (ulong)DateTime.Now.Ticks) { }
		public Random(ulong seed) {
			Seed = seed;
		}

		public ulong Seed {
			set {
				ptgfsr[0] = value;
				for (k = 1; k < N; ++k) {
					ptgfsr[k] = 69069 * ptgfsr[k - 1];
				}
				k = 1;
			}
		}

		public ulong Int {
			get {
				int kk;
				ulong y;

				if (k == N) {
					for (kk = 0; kk < N - M; ++kk) {
						y = (ptgfsr[kk] & UPPER_MASK) | (ptgfsr[kk + 1] & LOWER_MASK);
						ptgfsr[kk] = ptgfsr[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
					}
					for (; kk < N - 1; ++kk) {
						y = (ptgfsr[kk] & UPPER_MASK) | (ptgfsr[kk + 1] & LOWER_MASK);
						ptgfsr[kk] = ptgfsr[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
					}
					y = (ptgfsr[kk] & UPPER_MASK) | (ptgfsr[0] & LOWER_MASK);
					ptgfsr[N - 1] = ptgfsr[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];
					k = 0;
				}
				y = ptgfsr[k++];
				y ^= TemperingShiftU(y);
				y ^= TemperingShiftS(y) & TEMPERING_MASK_B;
				y ^= TemperingShiftT(y) & TEMPERING_MASK_C;
				return y ^= TemperingShiftL(y);
			}
		}

		public double Real {
			get {
				ulong a = Int >> 5, b = Int >> 6;
				return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
			}
		}

		public ulong Ranged(int range) {
			return range != 0 ? (Int % (ulong)range) : 0UL;
		}

		private static ulong TemperingShiftL(ulong val) { return val >> 18; }
		private static ulong TemperingShiftS(ulong val) { return val >>  7; }
		private static ulong TemperingShiftT(ulong val) { return val << 15; }
		private static ulong TemperingShiftU(ulong val) { return val >> 11; }

		public static Random Default {
			get { return rand; }
		}

	}

}
