using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    interface ISelectorAI {
        Card SelectCard(ScopaGame game, AIScopaPlayer player);
        List<Card> SelectTrick(ScopaGame game, AIScopaPlayer player, Card selectedCard);
    }

    struct SequentialSelectorAI : ISelectorAI {
        public Card SelectCard(ScopaGame game, AIScopaPlayer player) {
            return player.Hand[0];
        }
        public List<Card> SelectTrick(ScopaGame game, AIScopaPlayer player, Card selectedCard) {
            List<List<Card>> possibleTricks = game.PossibleActions[selectedCard].PossibleTricks;
            return possibleTricks[0];
        }
    }

    class RandomSelectorAI : ISelectorAI {
        private PlaySelection selection = new PlaySelection();

        public Card SelectCard(ScopaGame game, AIScopaPlayer player) {
            if (selection.SelectedCard != null && player.IsHolding((Card)selection.SelectedCard)) {
                return (Card)selection.SelectedCard;
            }
            return (Card)(selection.SelectedCard = player.Hand[(int)Random.Default.Ranged(player.Hand.Count)]);
        }
        public List<Card> SelectTrick(ScopaGame game, AIScopaPlayer player, Card selectedCard) {
            if (selection.SelectedCard != null && player.IsHolding((Card) selection.SelectedCard)) {
                return selection.SelectedTrick;
            }
            List<List<Card>> possibleTricks = game.PossibleActions[selectedCard].PossibleTricks;
            return selection.SelectedTrick = possibleTricks[(int)Random.Default.Ranged(possibleTricks.Count)];
        }
    }

    class AIScopaPlayer : AbstractScopaPlayer {
        private ISelectorAI selector;

        public AIScopaPlayer(ScopaGame game) : this("Computer", game) { }
		public AIScopaPlayer(string name, ScopaGame game) : base (name, game) {
            // selector = new RandomSelectorAI();
            selector = new WeightedSelectorAI();
        }

        public override Card SelectCard() {
            game.PopulateActions();
            return selector.SelectCard(game, this);
		}

        public override List<Card> SelectTrick(Card card) {
            game.PopulateActions();
            CardActions actions = game.PossibleActions[card];
            if (!actions.IsThrowable) {
                if (actions.PossibleTricks != null) {
                    if (actions.PossibleTricks.Count == 1) {
                        return actions.PossibleTricks[0];
                    }
                    return selector.SelectTrick(game, this, card);
                }
            }
            return new List<Card>();
        }

	}

}
