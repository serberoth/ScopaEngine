using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    interface ISelectorAI {
        Card SelectCard(ScopaGame game, AIScopaPlayer player);
        List<Card> SelectTrick(Card card, List<List<Card>> possibleTricks);
    }

    struct SequentialSelectorAI : ISelectorAI {
        public Card SelectCard(ScopaGame game, AIScopaPlayer player) {
            return player.Hand[0];
        }
        public List<Card> SelectTrick(Card card, List<List<Card>> possibleTricks) {
            return possibleTricks[0];
        }
    }

    struct RandomSelectorAI : ISelectorAI {
        public Card SelectCard(ScopaGame game, AIScopaPlayer player) {
            return player.Hand[(int)Random.Default.Ranged(player.Hand.Count)];
        }
        public List<Card> SelectTrick(Card card, List<List<Card>> possibleTricks) {
            return possibleTricks[(int)Random.Default.Ranged(possibleTricks.Count)];
        }
    }

    class AIScopaPlayer : AbstractScopaPlayer {
        private ISelectorAI selector;
        private PlaySelection selection;

        public AIScopaPlayer(ScopaGame game) : this("Computer", game) { }
		public AIScopaPlayer(string name, ScopaGame game) : base (name, game) {
            selector = new RandomSelectorAI();
            selection = new PlaySelection ();
        }

        public override Card SelectCard() {
            game.PopulateActions();
            if (selection.SelectedCard != null && IsHolding((Card) selection.SelectedCard)) {
                return (Card) selection.SelectedCard;
            }
            return (Card)(selection.SelectedCard = selector.SelectCard(game, this));
		}

        public override List<Card> SelectTrick(Card card) {
            game.PopulateActions();
            if (selection.SelectedCard != null && IsHolding((Card) selection.SelectedCard)) {
                CardActions actions = game.Actions[card];
                if (!actions.IsThrowable) {
                    if (actions.PossibleTricks != null) {
                        if (actions.PossibleTricks.Count == 1) {
                            return selection.SelectedTrick = actions.PossibleTricks[0];
                        }
                        return selection.SelectedTrick = selector.SelectTrick(card, actions.PossibleTricks);
                    }
                }
                return selection.SelectedTrick = new List<Card>();
            }
            return selection.SelectedTrick;
        }

	}

}
