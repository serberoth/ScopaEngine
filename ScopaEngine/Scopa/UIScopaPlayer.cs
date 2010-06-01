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
            CardActions actions = game.Actions[card];
            if (!actions.IsThrowable) {
                if (actions.PossibleTricks != null) {
                    if (actions.PossibleTricks.Count == 1) {
                        return actions.PossibleTricks[0];
                    }
                    List<Card> selectedTrick = selection.SelectedTrick;
                    if (selectedTrick.Count == 0) {
                        if (actions.PossibleTricks.Count == 1) {
                            return actions.PossibleTricks[0];
                        }
                    }
                    return selectedTrick;
                }
            }
            return new List<Card>();
        }

    }

}
