using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NIESoftware.Scopa {

	class TrickTracker {
		static readonly Dictionary<Value, int> PrimieraScore = new Dictionary<Value, int> {
			{ Value.Sette,		21 },
			{ Value.Sei,		18 },
			{ Value.Asso,		16 },
			{ Value.Cinque,		15 },
			{ Value.Quattro,	14 },
			{ Value.Tre,		13 },
			{ Value.Due,		12 },
			{ Value.Re,			10 },
			{ Value.Cavallo,	10 },
			{ Value.Fante,		10 },
		};

		private Dictionary<Suit, List<Card>> cardsTaken;
		private int cardCount;
		private int primieraValue;
		private bool setteBello;
		private int scopaCount;
		private List<List<Card>> trickStack;

		public TrickTracker() {
			cardsTaken = new Dictionary<Suit, List<Card>>();
			foreach (Suit suit in Enum.GetValues(typeof(Suit))) {
				cardsTaken.Add(suit, new List<Card>());
			}
			cardCount = 0;
			primieraValue = 0;
			setteBello = false;
			scopaCount = 0;
			trickStack = new List<List<Card>>();
		}
        private TrickTracker(TrickTracker parent) {
            cardsTaken = new Dictionary<Suit, List<Card>>();
            foreach (Suit suit in Enum.GetValues (typeof (Suit))) {
                cardsTaken.Add (suit, new List<Card> (parent.cardsTaken[suit]));
            }
            cardCount = parent.CardCount;
            primieraValue = parent.primieraValue;
            setteBello = parent.setteBello;
            scopaCount = parent.scopaCount;
            trickStack = new List<List<Card>>();
            foreach (List<Card> trick in parent.trickStack) {
                trickStack.Add(new List<Card>(trick));
            }
        }

		public List<Card> CardsTaken {
			get {
				List<Card> cards = new List<Card>();
				foreach (List<Card> list in cardsTaken.Values) {
					cards.AddRange(list);
				}
				cards.Sort();
				return cards;
			}
		}

		public int CardCount {
			get { return cardCount; }
		}
		public int PrimieraValue {
			get { return primieraValue; }
		}
		public int DenariCount {
			get { return cardsTaken[Suit.Denari].Count; }
		}
		public bool SetteBello {
			get { return setteBello; }
		}
		public int ScopaCount {
			get { return scopaCount; }
		}

		public List<List<Card>> TrickStack {
			get { return trickStack; }
		}

		public void TakeTrick(List<Card> cards, bool scopa) {
			Debug.Assert(cards != null);
			foreach (Card card in cards) {
				cardsTaken[card.Suit].Add(card);
				if (Card.SetteBello.Equals(card)) {
					setteBello = true;
				}
			}
			primieraValue = 0;
			foreach (Suit s in cardsTaken.Keys) {
				List<Card> list = cardsTaken[s];
				if (list.Count > 0) {
					list.Sort(CompareToPrimieraScore);
					primieraValue += PrimieraScore[list[list.Count - 1].Value];
				}
			}
			scopaCount += scopa ? 1 : 0;
			cardCount += cards.Count;
			trickStack.Add(new List<Card>(cards));
		}

        public TrickTracker GetPossibleScores(List<Card> cards, bool scopa) {
            TrickTracker tracker = new TrickTracker(this);
            tracker.TakeTrick(cards, scopa);
            return tracker;
        }

		int CompareToPrimieraScore(Card a, Card b) {
			return PrimieraScore[a.Value].CompareTo(PrimieraScore[b.Value]);
		}

	}

}
