using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    enum GameAction : int {
        Nessuna = -1,
        Concede = 0,
        Tira = 1,
        Prendi = 2,
    }

    struct PlayerActions {

        public IScopaPlayer Player { get; set; }
        public bool CanThrow { get; set; }
        public bool CanTrick { get; set; }
        public List<CardActions> CardActions { get; set; }

        public CardActions this[Card card] {
            get { return CardActions.Find(a => a.Card.Equals(card)); }
        }

        public int ThrowableCount {
            // get { return CardActions.Count<CardActions>(a => a.IsThrowable); }
            get { return CardActions.Count(a => a.IsThrowable); }
        }
        public int TrickableCount {
            // get { return CardActions.Count<CardActions>(a => !a.IsThrowable); }
            get { return CardActions.Count(a => !a.IsThrowable); }
        }

        public List<CardActions> ThrowableList {
            get {
                return CardActions.FindAll(delegate(CardActions actions) {
                    return actions.IsThrowable;
                });
            }
        }
        public List<CardActions> TrickableList {
            get {
                return CardActions.FindAll(delegate(CardActions actions) {
                    return !actions.IsThrowable;
                });
            }
        }

    }

    struct CardActions {

        public Card Card { get; set; }
        public bool IsThrowable { get; set; }
        public List<List<Card>> PossibleTricks { get; set; }

    }

}
