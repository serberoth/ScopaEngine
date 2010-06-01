using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NIESoftware.Scopa {

	class PlayerTracker : IEnumerable<Card> {
		private string name;
		private List<Card> hand;
		private TrickTracker trickTracker;
		private int roundScore;
		private int points;

		public PlayerTracker(string playerName) {
			name = playerName;
			hand = new List<Card>();
			trickTracker = new TrickTracker();
		}

		public string Name {
			get { return name; }
		}
		public List<Card> Hand {
			get { return hand; }
		}
		public TrickTracker TrickTracker {
			get { return trickTracker; }
		}
		public int RoundScore {
			get { return roundScore; }
			set { roundScore = value; }
		}
		public int Points {
			get { return points; }
			set { points = value; }
		}

		public Card this[int index] {
			get { return hand[index]; }
		}

		public int Count {
			get { return hand.Count; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public IEnumerator<Card> GetEnumerator() {
			return hand.GetEnumerator();
		}

		public void TakeTrick(List<Card> cards, bool scopa) {
			trickTracker.TakeTrick(cards, scopa);
		}

		public void NewRound () {
			trickTracker = new TrickTracker();
			hand = new List<Card>();
		}

	}

}
