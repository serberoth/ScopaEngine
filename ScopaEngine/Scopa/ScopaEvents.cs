using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    delegate void CardEvent(Card card);
    delegate void TrickEvent(Card card, List<Card> list);

    struct ScopaEventHandler {
        public CardEvent MustThrowCard;
        public CardEvent MustTakeTrick;
        public CardEvent UnableToThrow;
        public TrickEvent UnableToTakeTrick;
        public CardEvent CardThrown;
        public TrickEvent TrickTaken;
        public TrickEvent Scopa;
    }

    interface IPlaySelection {
        Card? SelectedCard { get; }
        List<Card> SelectedTrick { get; }
    }

    class PlaySelection : IPlaySelection {
        private Card? selectedCard = null;
        private List<Card> selectedTrick = new List<Card>();

        public Card? SelectedCard {
            get { return selectedCard; }
            set { selectedCard = value; }
        }
        public List<Card> SelectedTrick {
            get { return selectedTrick; }
            set { selectedTrick = value != null ? value : new List<Card>(); }
        }

        public bool IsInTrick(Card card) {
            return selectedTrick.Contains<Card>(card);
        }

        public void AddToTrick(Card card) {
            selectedTrick.Add(card);
        }

        public void RemoveFromTrick(Card card) {
            selectedTrick.Remove(card);
        }

        public void Clear() {
            selectedCard = null;
            selectedTrick = new List<Card>();
        }

    }

}
