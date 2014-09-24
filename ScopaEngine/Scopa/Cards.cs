using System;
using System.Collections;
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

		private readonly Suit suit;
		private readonly Value value;

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
				Card card = (Card)obj;
                return suit.Equals(card.suit) && value.Equals(card.value);
			}
			return false;
		}

		public override int GetHashCode() {
            const int prime = 31;
            int result = 1;
            result = prime * result + suit.GetHashCode ();
            result = prime * result + value.GetHashCode ();
			return result;
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

	class Deck : IEnumerable<Card>, ICloneable {
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
        protected Deck(List<Card> cards) {
            Debug.Assert(cards.Count == TOTAL_SIZE);
            this.cards = new List<Card> (cards);
        }

        public object Clone () {
            Deck deck = new Deck();
            deck.cards = new List<Card>(cards);
            return deck;
        }

		public virtual void Shuffle() {
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

        public Card this[int index] {
            get { return cards[index]; }
        }

		public int Count {
			get { return cards.Count; }
		}

		public bool IsExhausted {
			get { return cards.Count == 0; }
		}

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public IEnumerator<Card> GetEnumerator() {
            return cards.GetEnumerator();
        }

    }

    class StackedDeck : Deck {

        static readonly List<Card> STACK = new List<Card> {
            new Card (Suit.Coppe, Value.Fante),     // Table 1
            new Card (Suit.Coppe, Value.Fante),     // Player A
            new Card (Suit.Coppe, Value.Fante),     // Player B
            new Card (Suit.Coppe, Value.Fante),     // Table 2
            new Card (Suit.Coppe, Value.Fante),     // Player A
            new Card (Suit.Coppe, Value.Fante),     // Player B
            new Card (Suit.Coppe, Value.Fante),     // Table 3
            new Card (Suit.Coppe, Value.Fante),     // Player A
            new Card (Suit.Coppe, Value.Fante),     // Player B
            new Card (Suit.Coppe, Value.Fante),     // Table 4

            new Card (Suit.Coppe, Value.Fante),     // Player A
            new Card (Suit.Coppe, Value.Fante),     // Player B
            new Card (Suit.Coppe, Value.Fante),     // Player A
            new Card (Suit.Coppe, Value.Fante),     // Player B
            new Card (Suit.Coppe, Value.Fante),     // Player A
            new Card (Suit.Coppe, Value.Fante),     // Player B

            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),

            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),

            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),

            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
            new Card (Suit.Coppe, Value.Fante),
        };

        public StackedDeck() : base(STACK) {
        }

        public override void Shuffle () {
        }

    }

}
