using System;
using System.Collections.Generic;

namespace NIESoftware {

	sealed class Utilities {

		private Utilities() { }

		public static int BinarySearch<T>(List<T> list, T key) where T : IComparable {
			int low = 0;
			int high = list.Count - 1;
			
			while (low <= high) {
				int mid = (low + high) >> 1;
				T midValue = list[mid];
				int result = midValue.CompareTo (key);

				if (result < 0) {
					low = mid + 1;
				} else if (result > 0) {
					high = mid - 1;
				} else {
					return mid;
				}
			}
			return -(low + 1);
		}
		public static int BinarySearch<T>(List<T> list, T key, Comparison<T> compare) {
			int low = 0;
			int high = list.Count - 1;

			while (low <= high) {
				int mid = (low + high) >> 1;
				int result = compare (list[mid], key);

				if (result < 0) {
					low = mid + 1;
				} else if (result > 0) {
					high = mid - 1;
				} else {
					return mid;
				}
			}
			return -(low + 1);
		}

		public static void Shuffle<T>(List<T> list) {
			Shuffle(list, Random.Default);
		}
		public static void Shuffle<T>(List<T> list, Random random) {
			for (int i = list.Count; i > 1; --i) {
				Swap(list, i - 1, (int)random.Ranged(i));
			}
		}

		public static void Swap<T>(List<T> list, int index0, int index1) {
			T temp = list[index0];
			list[index0] = list[index1];
			list[index1] = temp;
		}

	}

}
