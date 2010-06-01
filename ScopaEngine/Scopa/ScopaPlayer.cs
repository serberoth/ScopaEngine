using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    interface IScopaPlayer {

        string Name { get; set; }
        int Points { get; set;  }
        int RoundScore { get; set;  }
        int CardCount { get; }
        int PrimieraValue { get; }
        int DenariCount { get; }
        bool SetteBello { get; }
        int ScopaCount { get; }
        List<Card> LastTrick { get; }

#if DEBUG
        List<Card> CardsTaken { get; }
#endif

        bool IsPly { get; }

        bool IsHolding(Card card);

        List<Card> Hand { get; }

        Card SelectCard();
        List<Card> SelectTrick(Card card);

        void TakeTrick(List<Card> cards, bool scopa);
        TrickTracker GetPossibleScores(List<Card> cards, bool scopa);

        void NewRound();

        int IndexOf(Card card);

    }

    abstract class AbstractScopaPlayer : IScopaPlayer {
        protected ScopaGame game;
        protected string name;
        private List<Card> hand;
        private TrickTracker trickTracker;
        private int points;
        private int roundScore;

        public AbstractScopaPlayer(string name, ScopaGame game) {
            this.game = game;
            this.name = name;
            hand = new List<Card>();
            trickTracker = new TrickTracker();
            points = 0;
            roundScore = 0;
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }
        public int Points {
            get { return points; }
            set { points = value; }
        }
        public int RoundScore {
            get { return roundScore; }
            set { roundScore = value; }
        }
        public int CardCount {
            get { return trickTracker.CardCount; }
        }
        public int PrimieraValue {
            get { return trickTracker.PrimieraValue; }
        }
        public int DenariCount {
            get { return trickTracker.DenariCount; }
        }
        public bool SetteBello {
            get { return trickTracker.SetteBello; }
        }
        public int ScopaCount {
            get { return trickTracker.ScopaCount; }
        }
        public List<Card> LastTrick {
            get {
                if (trickTracker.TrickStack.Count > 0) {
                    return new List<Card>(trickTracker.TrickStack[trickTracker.TrickStack.Count - 1]);
                }
                return new List<Card>();
            }
        }

#if DEBUG
        public List<Card> CardsTaken {
            get { return trickTracker.CardsTaken; }
        }
#endif

        public bool IsPly {
            get { return game.Current.Equals(this) && !game.IsRoundOver; }
        }

        public bool IsHolding(Card card) {
            return hand.Contains<Card>(card);
        }

        public List<Card> Hand {
            get { return hand; }
        }

        public abstract Card SelectCard();
        public abstract List<Card> SelectTrick(Card card);

        public void TakeTrick(List<Card> cards, bool scopa) {
            trickTracker.TakeTrick(cards, scopa);
        }

        public TrickTracker GetPossibleScores(List<Card> cards, bool scopa) {
            return trickTracker.GetPossibleScores(cards, scopa);
        }

        public void NewRound() {
            hand.Clear ();
            trickTracker = new TrickTracker();
        }

        public int IndexOf(Card card) {
            for (int i = 0; i < hand.Count; ++i) {
                if (hand[i].Equals(card)) {
                    return i;
                }
            }
            return -1;
        }

    }

}
