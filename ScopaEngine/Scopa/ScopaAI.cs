using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

    interface IScopaPlayer {

        string Name { get; set; }
        int Points { get; }
        int RoundScore { get; }
        int CardCount { get; }
        int PrimieraValue { get; }
        int DenariCount { get; }
        bool SetteBello { get; }
        int ScopaCount { get; }

        bool IsPly { get; }

        bool IsHolding(Card card);

        List<Card> Hand { get; }

        Card SelectCard();
        List<Card> SelectTrick(Card card);

        int IndexOf(Card card);

    }

    abstract class AbstractScopaPlayer : IScopaPlayer {
        protected ScopaGame game;
        protected Player player;
        protected string name;

        public AbstractScopaPlayer(string name, ScopaGame game, Player player) {
            this.game = game;
            this.player = player;
            this.name = name;
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }
        public int Points {
            get { return player.Points; }
        }
        public int RoundScore {
            get { return player.RoundScore; }
        }
        public int CardCount {
            get { return player.TrickTracker.CardCount; }
        }
        public int PrimieraValue {
            get { return player.TrickTracker.PrimieraValue; }
        }
        public int DenariCount {
            get { return player.TrickTracker.DenariCount; }
        }
        public bool SetteBello {
            get { return player.TrickTracker.SetteBello; }
        }
        public int ScopaCount {
            get { return player.TrickTracker.ScopaCount; }
        }

        public bool IsPly {
            get { return game.Current.Equals(player) && !game.IsRoundOver; }
        }

        public bool IsHolding(Card card) {
            return player.Contains<Card>(card);
        }

        public List<Card> Hand {
            get { return player.Hand; }
        }

        public abstract Card SelectCard();
        public abstract List<Card> SelectTrick(Card card);

        public int IndexOf(Card card) {
            for (int i = 0; i < player.Count; ++i) {
                if (player[i].Equals(card)) {
                    return i;
                }
            }
            return -1;
        }

    }

    interface ISelectorAI {
        Card SelectCard(Player player);
        List<Card> SelectTrick(Card card, List<List<Card>> possibleTricks);
    }

    struct SequentialSelectorAI : ISelectorAI {
        public Card SelectCard(Player player) {
            return player[0];
        }
        public List<Card> SelectTrick(Card card, List<List<Card>> possibleTricks) {
            return possibleTricks[0];
        }
    }

    struct RandomSelectorAI : ISelectorAI {
        public Card SelectCard(Player player) {
            return player[(int)Random.Default.Ranged(player.Count)];
        }
        public List<Card> SelectTrick(Card card, List<List<Card>> possibleTricks) {
            return possibleTricks[(int)Random.Default.Ranged(possibleTricks.Count)];
        }
    }

    class AIScopaPlayer : AbstractScopaPlayer {
        private ISelectorAI selector;

        public AIScopaPlayer(ScopaGame game, Player player) : this("Computer", game, player) { }
		public AIScopaPlayer(string name, ScopaGame game, Player player) : base (name, game, player) {
            selector = new RandomSelectorAI();
        }

        public override Card SelectCard() {
            game.PopulateActions();
            return selector.SelectCard(player);
		}

        public override List<Card> SelectTrick(Card card) {
            game.PopulateActions();
            CardActions actions = game.Actions[card];
            if (!actions.IsThrowable) {
                if (actions.PossibleTricks != null) {
                    if (actions.PossibleTricks.Count == 1) {
                        return actions.PossibleTricks[0];
                    }
                    selector.SelectTrick(card, actions.PossibleTricks);
                }
            }
            return new List<Card>();
        }

	}

    interface IScopaUIForm {
        Card SelectedCard { get; }
        List<Card> SelectedTrick { get; }
    }

    class UIScopaPlayer : AbstractScopaPlayer {
        private IScopaUIForm form;

		public UIScopaPlayer(string name, ScopaGame game, Player player, IScopaUIForm form) : base (name, game, player) {
            this.form = form;
        }

        public override Card SelectCard() {
            game.PopulateActions();
            return form.SelectedCard;
		}

        public override List<Card> SelectTrick(Card card) {
            game.PopulateActions();
            CardActions actions = game.Actions[card];
            if (!actions.IsThrowable) {
                if (actions.PossibleTricks != null) {
                    if (actions.PossibleTricks.Count == 1) {
                        return actions.PossibleTricks[0];
                    }
                    List<Card> selectedTrick = form.SelectedTrick;
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
