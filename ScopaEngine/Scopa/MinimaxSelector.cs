using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    class WeightedTreeNode : IEnumerable<WeightedTreeNode> {
        private WeightedTreeNode parent;
        private List<WeightedTreeNode> children;
        private WeightedSelection selection;

        public WeightedTreeNode(WeightedTreeNode parent) {
            this.parent = parent;
            this.children = new List<WeightedTreeNode>();
            this.selection = new WeightedSelection();
            if (parent != null) {
                parent.children.Add(this);
            }
        }

        public WeightedTreeNode Parent {
            get { return parent; }
        }
        public List<WeightedTreeNode> Children {
            get { return children; }
        }
        public WeightedSelection Selection {
            get { return selection; }
            set { selection = value; }
        }

        public int Count {
            get { return children.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public IEnumerator<WeightedTreeNode> GetEnumerator() {
            return children.GetEnumerator();
        }

        public WeightedTreeNode FindFirstBreadth(Func<WeightedTreeNode, bool> predicate, int depth) {
            if (predicate(this)) {
                return this;
            }
            foreach (WeightedTreeNode child in children) {
                WeightedTreeNode result;
                if ((result = child.FindFirstBreadth (predicate, depth + 1)) != null) {
                    return result;
                }
            }
            return null;
        }
        public IEnumerable<WeightedTreeNode> FindBreadth() {
            return FindBreadth(a => true, -1);
        }
        public IEnumerable<WeightedTreeNode> FindBreadth(Func<WeightedTreeNode, bool> predicate) {
            return FindBreadth(predicate, -1);
        }
        public IEnumerable<WeightedTreeNode> FindBreatch(int maxDepth) {
            return FindBreadth(a => true, maxDepth);
        }
        public IEnumerable<WeightedTreeNode> FindBreadth(Func<WeightedTreeNode, bool> predicate, int maxDepth) {
            List<object[]> queue = new List<object[]>() { new object[] { this, 0, }, };
            while (queue.Count > 0) {
                WeightedTreeNode curr = queue[0][0] as WeightedTreeNode;
                int depth = (int) (queue[0][1] as int?);
                queue.RemoveAt(0);
                if (predicate(curr)) {
                    yield return curr;
                }
                if (depth > 0 && depth < maxDepth) {
                    foreach (WeightedTreeNode child in curr) {
                        queue.Add(new object[] { child, depth + 1, });
                    }
                }
            }
            // yield break;
        }

    }

    class GameState {
        public List<Card> Table { get; set; }
        public PlayerActions PossibleActions { get; set; }
        public TrickTracker Tracker { get; set; }
        public List<Card> Hand { get; set; }
    }

    class MinimaxSelectorAI : ISelectorAI {
        private WeightedSelection selection;

        public WeightedTreeNode BuildTree(ScopaGame game, IScopaPlayer player, int maxDepth) {
            WeightedTreeNode root = new WeightedTreeNode(null);
            GameState state = new GameState();
            state.Table = game.Table;
            state.PossibleActions = game.PossibleActions;
            state.Tracker = player.GetPossibleScores(new List<Card>(), false);
            state.Hand = new List<Card>(player.Hand);
            List<object[]> queue = new List<object[]>() { new object[] { root, state, 0, }, };
            while (queue.Count > 0) {
                WeightedTreeNode currNode = queue[0][0] as WeightedTreeNode;
                GameState currState = queue[0][1] as GameState;
                int depth = (int)(queue[0][1] as int?);
                queue.RemoveAt(0);
                if (depth > 0 && depth < maxDepth) {
                    WeightedSelection selection = DoLevelSelection(currState);
                    List<Card> trick = new List<Card>(selection.SelectedTrick);
                    trick.Add((Card)selection.SelectedCard);
                    GameState childState = new GameState();
                    childState.Table = new List<Card>(game.Table);
                    childState.Table.RemoveAll(a => trick.Contains(a));
                    childState.PossibleActions = new PlayerActions(); // AbstractScopaGame.EnumerateAll(childState.Table, selection.SelectedCard);
                    childState.Tracker = currState.Tracker.GetPossibleScores(trick, currState.Table.Count == selection.SelectedTrick.Count);
                    childState.Hand = new List<Card>(currState.Hand);
                    childState.Hand.Remove((Card)selection.SelectedCard);
                    queue.Add(new object[] { new WeightedTreeNode(currNode), childState, depth + 1, });
                    currNode.Selection = selection;
                }
            }
            return root;
        }

        private WeightedSelection DoLevelSelection(GameState state) {
            WeightedSelection selection = new WeightedSelection ();
            if (state.Hand.Count == 1) {
                selection = DoLevelTrickSelection(state, state.Hand[0]);
                return selection;
            }
            List<Card> playable = state.Hand.FindAll(a => !state.PossibleActions[a].IsThrowable);
            if (playable.Count > 0) {
                // Always take the Sette Bello
                if (playable.Contains<Card>(Card.SetteBello)) {
                    selection = DoLevelTrickSelection(state, Card.SetteBello);
                    return selection;
                }

                List<WeightedSelection> possibles = new List<WeightedSelection>();
                foreach (Card card in playable) {
                    possibles.Add(DoLevelTrickSelection(state, card));
                }
                List<WeightedSelection> actuals = Utilities.MaximumElements<WeightedSelection>(possibles, a => a.Weight);
                selection = actuals[0];
                return selection;
            }

            // All cards in hand are throwable
            Card? bestThrow = null;
            int maxWeight = Int32.MinValue;
            foreach (Card card in state.Hand) {
                // TODO: This does not take into account that throwing a card can enable a trick,
                // which should be given more weight than throwing a card that does not do so.
                int weight = -CalculateWeight(state.Tracker, card, new List<Card>(), false);
                if (weight > maxWeight) {
                    bestThrow = card;
                    maxWeight = weight;
                }
            }

            selection.SelectedCard = bestThrow;
            selection.SelectedTrick = new List<Card>();
            selection.Weight = maxWeight;
            return selection;
        }

        private WeightedSelection DoLevelTrickSelection(GameState state, Card selectedCard) {
            List<List<Card>> possibleTricks = state.PossibleActions[selectedCard].PossibleTricks;
            List<Card> bestTrick = null;
            int maxWeight = Int32.MinValue;
            foreach (List<Card> trick in possibleTricks) {
                int weight = CalculateWeight(state.Tracker, selectedCard, trick, state.Table.Count == trick.Count);
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

        private int CalculateWeight(TrickTracker tracker, Card selectedCard, List<Card> selectedTrick, bool scopa) {
            // Make a decision here based on the number of denari as well as the primiera value
            // of the in the trick (also base this on player need. i.e. if the primiera value is above
            // the mid-range value (78) then we can prioritize denari and vise-versa for denari if the
            // player has more than 5 denari we can prioritize pirmiera.  As a last criteria we pick up
            // as many cards as possible in each trick.
            List<Card> trick = new List<Card>(selectedTrick);
            trick.Add(selectedCard);
            TrickTracker newTracker = tracker.GetPossibleScores(trick, scopa);
            int weight = ((!tracker.SetteBello && newTracker.SetteBello) ? 84 : 0)
                + (scopa ? 84 : 0)
                + ((tracker.PrimieraValue > 78) ? 1 : 2) * (newTracker.PrimieraValue - tracker.PrimieraValue)
                + ((tracker.DenariCount > 5) ? 1 : 2) * (newTracker.DenariCount - tracker.DenariCount)
                + (newTracker.CardCount - tracker.CardCount);
            Console.Out.WriteLine("SetteBello " + newTracker.SetteBello + ", Scopa " + scopa);
            Console.Out.WriteLine("Primiera " + newTracker.PrimieraValue + " <- " + tracker.PrimieraValue);
            Console.Out.WriteLine("Denari   " + newTracker.DenariCount + " <- " + tracker.DenariCount);
            Console.Out.WriteLine("Cards    " + newTracker.CardCount + " <- " + tracker.CardCount);
            Console.Out.WriteLine("Cards Taken " + Card.ToString(newTracker.CardsTaken));
            Console.Out.WriteLine("Trick " + Card.ToString(selectedTrick) + " with " + selectedCard + " := " + weight);
            return weight;
        }

        private Card DoSelection(ScopaGame game, AIScopaPlayer player) {
            if (player.Hand.Count == 1) {
                selection = DoTrickSelection(game, player, (Card)player.Hand[0]);
                return (Card)selection.SelectedCard;
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
                    possibles.Add(DoTrickSelection(game, player, card));
                }
                List<WeightedSelection> actuals = Utilities.MaximumElements<WeightedSelection>(possibles, a => a.Weight);
                selection = actuals[0];
                return (Card)actuals[0].SelectedCard;
            }

            // All cards in hand are throwable
            Card? bestThrow = null;
            int maxWeight = Int32.MinValue;
            foreach (Card card in player.Hand) {
                // TODO: This does not take into account that throwing a card can enable a trick,
                // which should be given more weight than throwing a card that does not do so.
                int weight = -CalculateWeight(player, card, new List<Card>(), false);
                if (weight > maxWeight) {
                    bestThrow = card;
                    maxWeight = weight;
                }
            }

            selection = new WeightedSelection();
            selection.SelectedCard = bestThrow;
            selection.SelectedTrick = new List<Card>();
            selection.Weight = maxWeight;
            return (Card)bestThrow;
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
                return (Card)selection.SelectedCard;
            }
            DoSelection(game, player);
            return (Card)selection.SelectedCard;
        }

        public List<Card> SelectTrick(ScopaGame game, AIScopaPlayer player, Card selectedCard) {
            if (selection.SelectedCard != null && player.IsHolding((Card)selection.SelectedCard)) {
                return selection.SelectedTrick;
            }
            selection = DoTrickSelection(game, player, selectedCard);
            return selection.SelectedTrick;
        }

    }

}
