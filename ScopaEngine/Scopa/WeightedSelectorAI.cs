using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    struct WeightedSelection : IPlaySelection {

        public Card? SelectedCard { get; set; }
        public List<Card> SelectedTrick { get; set; }
        public int Weight { get; set; }

    }

    class WeightedSelectorAI : ISelectorAI {
        private WeightedSelection selection;

        private Card DoSelection(ScopaGame game, AIScopaPlayer player) {
            if (player.Hand.Count == 1) {
                selection = DoTrickSelection(game, player, (Card) player.Hand[0]);
                return (Card) selection.SelectedCard;
            }
            List<Card> playable = player.Hand.FindAll(a => !game.PossibleActions[a].IsThrowable);
            if (playable.Count > 0) {
                // Always take the Sette Bello
                if (playable.Contains<Card>(Card.SetteBello)) {
                    selection = DoTrickSelection(game, player, Card.SetteBello);
                    return Card.SetteBello;
                }

                List<WeightedSelection> possibles = new List<WeightedSelection>();
                foreach (Card card in playable) {
                    possibles.Add (DoTrickSelection (game, player, card));
                }
                List<WeightedSelection> actuals = Utilities.MaximumElements<WeightedSelection>(possibles, a => a.Weight);
                selection = actuals[0];
                return (Card) actuals[0].SelectedCard;
            }

            // All cards in hand are throwable
            Card? bestThrow = null;
            int maxWeight = Int32.MinValue;
            foreach (Card card in player.Hand) {
                // TODO: This does not take into account that throwing a card can enable a trick,
                // which should be given more weight than throwing a card that does not do so.
                int weight = -CalculateWeight(player, card, new List<Card> (), false);
                if (weight > maxWeight) {
                    bestThrow = card;
                    maxWeight = weight;
                }
            }

            selection = new WeightedSelection();
            selection.SelectedCard = bestThrow;
            selection.SelectedTrick = new List<Card> ();
            selection.Weight = maxWeight;
            return (Card) bestThrow;
        }

        private int CalculateWeight(AIScopaPlayer player, Card selectedCard, List<Card> selectedTrick, bool scopa) {
            // Make a decision here based on the number of denari as well as the primiera value
            // of the in the trick (also base this on player need. i.e. if the primiera value is above
            // the mid-range value (78) then we can prioritize denari and vise-versa for denari if the
            // player has more than 5 denari we can prioritize pirmiera.  As a last criteria we pick up
            // as many cards as possible in each trick.
            List<Card> trick = new List<Card>(selectedTrick);
            trick.Add(selectedCard);
            TrickTracker tracker = player.GetPossibleScores(trick, scopa);
            int weight = ((!player.SetteBello && tracker.SetteBello) ? 84 : 0)
                + (scopa ? 84 : 0)
                + ((player.PrimieraValue > 78) ? 1 : 2) * (tracker.PrimieraValue - player.PrimieraValue)
                + ((player.DenariCount > 5) ? 1 : 2) * (tracker.DenariCount - player.DenariCount)
                + (tracker.CardCount - player.CardCount);
            Console.Out.WriteLine("SetteBello " + tracker.SetteBello + ", Scopa " + scopa);
            Console.Out.WriteLine("Primiera " + tracker.PrimieraValue + " <- " + player.PrimieraValue);
            Console.Out.WriteLine("Denari   " + tracker.DenariCount + " <- " + player.DenariCount);
            Console.Out.WriteLine("Cards    " + tracker.CardCount + " <- " + player.CardCount);
            Console.Out.WriteLine("Cards Taken " + Card.ToString(tracker.CardsTaken));
            Console.Out.WriteLine("Trick " + Card.ToString(selectedTrick) + " with " + selectedCard + " := " + weight);
            return weight;
        }

        private WeightedSelection DoTrickSelection(ScopaGame game, AIScopaPlayer player, Card selectedCard) {
            List<List<Card>> possibleTricks = game.PossibleActions[selectedCard].PossibleTricks;
            List<Card> bestTrick = null;
            int maxWeight = Int32.MinValue;
            foreach (List<Card> trick in possibleTricks) {
                int weight = CalculateWeight(player, selectedCard, trick, game.Table.Count == trick.Count);
                if (weight > maxWeight) {
                    bestTrick = trick;
                    maxWeight = weight;
                }
            }

            WeightedSelection selection = new WeightedSelection();
            selection.SelectedCard = selectedCard;
            selection.SelectedTrick = bestTrick;
            selection.Weight = maxWeight;
            return selection;
        }

        public Card SelectCard(ScopaGame game, AIScopaPlayer player) {
            if (selection.SelectedCard != null && player.IsHolding((Card)selection.SelectedCard)) {
                return (Card) selection.SelectedCard;
            }
            DoSelection(game, player);
            return (Card) selection.SelectedCard;
        }

        public List<Card> SelectTrick(ScopaGame game, AIScopaPlayer player, Card selectedCard) {
            if (selection.SelectedCard != null && player.IsHolding ((Card) selection.SelectedCard)) {
                return selection.SelectedTrick;
            }
            selection = DoTrickSelection(game, player, selectedCard);
            return selection.SelectedTrick;
        }

    }

}
