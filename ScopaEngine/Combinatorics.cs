using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NIESoftware {

	sealed class ChoiceEnumerator<T> : IEnumerator<T[]> {
		private ChoiceEnumerable<T> enumerable;
		private int current = 0;
		private int[] chosen;

		public ChoiceEnumerator (ChoiceEnumerable<T> enumerable) {
			this.enumerable = enumerable;
			chosen = new int[enumerable.SampleSize];
			for (int i = 0; i < enumerable.SampleSize; ++i) {
				chosen[i] = i;
			}
		}
		~ChoiceEnumerator() {
			Dispose (false);
		}

		public void Dispose() {
			Dispose (true);
		}
		void Dispose (bool disposing) {
			if (!disposing) {
				enumerable = null;
			}
		}

		public bool MoveNext () {
			return current < enumerable.NumElements;
		}

		object IEnumerator.Current { get { return Current; } }
		public T[] Current {
			get {
				T[] result = new T[enumerable.SampleSize];
				for (int i = 0; i < enumerable.SampleSize; ++i) {
					result[i] = enumerable.Input[chosen[i]];
				}
				current++;
				if (current < enumerable.NumElements) {
					Increase (enumerable.SampleSize - 1, enumerable.Input.Length - 1);
				}
				return result;
			}
		}

		private void Increase (int n, int max) {
			if (chosen[n] < max) {
				chosen[n]++;
				for (int i = n + 1; i < enumerable.SampleSize; ++i) {
					chosen[i] = chosen[i - 1] + 1;
				}
			} else {
				Increase (n -1, max - 1);
			}
		}

		public void Reset () {
			chosen = new int[enumerable.SampleSize];
			for (int i = 0; i < enumerable.SampleSize; ++i) {
				chosen[i] = i;
			}
		}

	}

	sealed class ChoiceEnumerable<T> : IEnumerable<T[]>  {
		private T[] input;
		private int sampleSize;
		private int numElements;

		public ChoiceEnumerable (int sampleSize, T[] input) {
			this.input = input.Clone () as T[];
			this.sampleSize = sampleSize;
			numElements = ChoiceEnumerable<T>.Factorial(input.Length) / (ChoiceEnumerable<T>.Factorial(sampleSize) * ChoiceEnumerable<T>.Factorial(input.Length - sampleSize));
		}

		public T[] Input {
			get { return input; }
		}
		public int SampleSize {
			get { return sampleSize; }
		}
		public int NumElements {
			get { return numElements; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public IEnumerator<T[]> GetEnumerator () {
			return new ChoiceEnumerator<T> (this);
		}

		private static int Factorial(int n) {
			int result = 1;
			for (int i = 2; i <= n; ++i) {
				result *= i;
			}
			return result;
		}

	}

}
