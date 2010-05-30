using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NIESoftware.Scopa {

	enum Suit : uint {
		Denari	= 0,
		Coppe	= 1,
		Bastone	= 2,
		Spade	= 3,
	}

	enum Value : uint {
		Asso	= 1,
		Due		= 2,
		Tre		= 3,
		Quattro	= 4,
		Cinque	= 5,
		Sei		= 6,
		Sette	= 7,
		Fante	= 8,
		Cavallo	= 9,
		Re		= 10,
	}

	struct Card : IComparable<Card> {
		public static readonly Card SetteBello = new Card(Suit.Denari, Value.Sette);

		private Suit suit;
		private Value value;

		public Card(Suit s, Value v) {
			suit = s;
			value = v;
		}

		public Suit Suit {
			get { return suit; }
		}
		public Value Value {
			get { return value; }
		}

		public override bool Equals(Object obj) {
			if (obj != null && obj is Card) {
				Card c = (Card)obj;
				return suit.Equals(c.suit) && value.Equals(c.value);
			}
			return false;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public int CompareTo (Card c) {
			int result = suit.CompareTo (c.suit);
			if (result == 0) {
				return value.CompareTo (c.value);
			}
			return result;
		}

		public override string ToString() {
			return value + " di " + suit;
		}

		public static explicit operator int(Card c) {
			return (int)c.value;
		}

		public static string ToString(List<Card> cardList) {
			return Card.ToString(cardList, false);
		}
		public static string ToString (List<Card> cardList, bool appendIndex) {
			Debug.Assert(cardList != null);
			StringBuilder builder = new StringBuilder ("[ ");
			int index = 0;
			foreach (Card c in cardList) {
				if (index++ > 0) {
					builder.Append(", ");
				}
				if (appendIndex) {
					builder.Append(index + ") ");
				}
				builder.Append (c);
			}
			return builder.Append (" ]").ToString();
		}

	}

	class Deck {
		public const int TOTAL_SIZE = 40; // Enum.GetValues (typeof (Suit)).Length * Enum.GetValues (typeof (Value)).Length;

#if DEBUG
		private static readonly Random RANDOM = new Random (1);
#endif

		private List<Card> cards;

		public Deck() {
			cards = new List<Card>();
			foreach (Suit s in Enum.GetValues(typeof(Suit))) {
				foreach (Value v in Enum.GetValues(typeof(Value))) {
					cards.Add(new Card(s, v));
				}
			}
		}

		public void Shuffle() {
#if DEBUG
			Utilities.Shuffle(cards, RANDOM);
#else
			Utilities.Shuffle(cards);
#endif
		}

		public Card Deal {
			get {
				Card next = cards[0];
				cards.RemoveAt(0);
				return next;
			}
		}

		public int Count {
			get { return cards.Count; }
		}

		public bool IsExhausted {
			get { return cards.Count == 0; }
		}

	}

}
