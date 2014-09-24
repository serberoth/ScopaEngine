using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    class UIScopaPlayer : AbstractScopaPlayer {
        private IPlaySelection selection;

        public UIScopaPlayer(string name, ScopaGame game, IPlaySelection selection) : base(name, game) {
            this.selection = selection;
        }

        public override Card SelectCard() {
            game.PopulateActions();
            return (Card) selection.SelectedCard;
        }

        public override List<Card> SelectTrick(Card card) {
            game.PopulateActions();
            CardActions actions = game.PossibleActions[card];
            if (selection.SelectedTrick != null && selection.SelectedTrick.Count > 0) {
                return selection.SelectedTrick;
            }
            if (!actions.IsThrowable) {
                if (actions.PossibleTricks != null) {
                    if (actions.PossibleTricks.Count == 1) {
                        return actions.PossibleTricks[0];
                    }
                }
            }
            return new List<Card>();
        }

    }

}
