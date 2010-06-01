// #define USE_STACKED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NIESoftware.Scopa {

	class ScopaGame {
		private const int DEFAULT_WINNING_SCORE = 11;

		private int winningScore;
        private List<IScopaPlayer> players;
		private List<Card> table;
		private Deck deck;
		private int currentDealer;
		private int currentPlayer;
		private IScopaPlayer lastTrick;
		private PlayerActions actions;

		/// <summary>
		/// Create a new ScopaGame instance with the default winning score of 11 points.
		/// </summary>
		public ScopaGame() : this(DEFAULT_WINNING_SCORE) { }
		/// <summary>
		/// Create a new ScopaGame with the specified number of points required to win the game.
		/// </summary>
		/// <param name="points">The number of points required to win the game.</param>
		public ScopaGame(int points) {
			winningScore = points;
            players = new List<IScopaPlayer>();
			table = new List<Card>();
			deck = NewDeck;
			currentDealer = 0;
			currentPlayer = currentDealer + 1;
			lastTrick = null;
			actions = new PlayerActions();
		}

        #region Field Accessors

        /// <summary>
		/// Get the required winning score that will complete this game.
		/// </summary>
		public int WinningScore {
			get { return winningScore; }
		}

		/// <summary>
		/// Get the list of players that are currently in this game.
		/// </summary>
		public List<IScopaPlayer> Players {
			get { return players; }
		}

		/// <summary>
		/// Get the list of cards that are currently on the table.
		/// </summary>
		public List<Card> Table {
			get { return table; }
		}

		/// <summary>
		/// Get the current Player who is the dealer.
		/// </summary>
		public IScopaPlayer Dealer {
			get { return players[currentDealer]; }
		}
		
		/// <summary>
		/// Get the current Player that must make a play.
		/// </summary>
		public IScopaPlayer Current {
			get { return players[currentPlayer]; }
		}

        /// <summary>
		/// Get the last Player to successfully take a trick.
		/// </summary>
		public IScopaPlayer LastTrick {
			get { return lastTrick; }
			set { lastTrick = value; }
		}

		public PlayerActions Actions {
			get { return actions; }
		}

		public int DeckCount {
			get { return deck.Count; }
		}

        #endregion // FieldAccessors

        #region Status Accessors

        /// <summary>
		/// Determine if the deck is exhausted
		/// </summary>
		public bool IsDeckExhausted {
			get { return deck.IsExhausted; }
		}

        /// <summary>
        /// Determine if the game is in progress.  This means that hands have been dealt from the
        /// deck and or points have been scored.
        /// </summary>
        public bool IsInProgress {
            get { return players.Any<IScopaPlayer>(a => a.Points > 0) && deck.Count < Deck.TOTAL_SIZE; }
        }

        /// <summary>
        /// Determine if the current hand is over.  This means that no players hold cards in their
        /// hands.
        /// </summary>
        public bool IsHandOver {
            get { return !players.Any<IScopaPlayer> (a => a.Hand.Count > 0); }
        }

        /// <summary>
        /// Determine if the current round is completed. This means the deck is exhausted and all
        /// players hold no cards in their hands.
        /// </summary>
        public bool IsRoundOver {
            get { return deck.IsExhausted && IsHandOver; }
        }

        /// <summary>
        /// Determine if the current game is over (i.e. a player has obtained a winning score and
        /// has more points than every other player).
        /// </summary>
        public bool IsGameOver {
            get {
                IScopaPlayer leader = Utilities.MaximumElement(players, a => a.Points);
                return leader != null && leader.Points >= winningScore;
            }
        }

        #endregion // Status Accessors

        #region Utility Methods

        /// <summary>
        /// Add the provided player instance to the game if it is not currently in progress.
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(IScopaPlayer player) {
            Debug.Assert(player != null && !IsInProgress);
            players.Add(player);
        }
        /// <summary>
        /// Add all of the players in the list to the current game if it is not in progress.
        /// </summary>
        /// <param name="names">The list of player names to add to the game</param>
        public void AddPlayers(List<IScopaPlayer> players) {
            Debug.Assert(players != null && !IsInProgress);
            this.players.AddRange(players);
        }


		/// <summary>
		/// Begin a new game.  This shuffles the deck and determines a random player to
		/// deal the cards.
		/// </summary>
		public void BeginGame () {
			deck = NewDeck;
			for (int i = 0; i < 7; ++i) {
				deck.Shuffle();
			}
            currentDealer = (int) Random.Default.Ranged(players.Count);
			currentPlayer = (currentDealer + 1) % players.Count;
			actions = new PlayerActions();
		}

		/// <summary>
		/// Begin a new round.  This resets the round for every player, shuffles the deck
		/// and changes the dealer to the next player in line for dealing.
		/// </summary>
		public void NewRound() {
            foreach (IScopaPlayer player in players) {
                player.NewRound();
            }
			deck = NewDeck;
			for (int i = 0; i < 7; ++i) {
				deck.Shuffle();
			}
			currentDealer = ++currentDealer % players.Count;
			currentPlayer = (currentDealer + 1) % players.Count;
			actions = new PlayerActions();
		}

		/// <summary>
		/// Deal the starting hand of the round.  This puts 3 cards in every players hand
		/// and 4 cards on the table.
		/// </summary>
		public void DealStartingHand() {
			table.Add(deck.Deal);
			for (int i = 0; i < 3; ++i) {
				for (int j = 0; j < players.Count; ++j) {
					players[(currentPlayer + j) % players.Count].Hand.Add(deck.Deal);
				}
				table.Add(deck.Deal);
			}
		}

		/// <summary>
		/// Deal the next hand in the round.  This puts 3 cards in every players hand.
		/// </summary>
		public void DealHand() {
			for (int i = 0; i < 3; ++i) {
				for (int j = 0; j < players.Count; ++j) {
					players[(currentPlayer + j) % players.Count].Hand.Add(deck.Deal);
				}
			}
		}

        #endregion // Utility Methods

        #region Action Methods

        public void PopulateActions() {
            if (!Current.Equals(actions.Player)) {
                actions.Player = Current;
                actions.CardActions = new List<CardActions>();
                foreach (Card c in Current.Hand) {
                    CardActions cardActions = new CardActions();
                    cardActions.Card = c;
                    cardActions.IsThrowable = ValidateThrow(c);
                    cardActions.PossibleTricks = new List<List<Card>>();
                    if (!cardActions.IsThrowable) {
                        List<List<Card>> possibles = ScopaGame.EnumerateAll(table, c);
                        foreach (List<Card> possible in possibles) {
                            if (ValidateTrick(c, possible)) {
                                cardActions.PossibleTricks.Add(possible);
                            }
                        }
                    }
                    actions.CardActions.Add(cardActions);
                }
                actions.CanThrow = actions.CardActions.Any<CardActions>(a => a.IsThrowable);
                actions.CanTrick = actions.CardActions.Any<CardActions>(a => !a.IsThrowable);
            }
        }

        public void TakeAction(IScopaPlayer player, ScopaEventHandler eventHandler) {
            if (player.IsPly) {
                PopulateActions();
                Card selectedCard = player.SelectCard();
                List<Card> selectedTrick = player.SelectTrick(selectedCard);
                CardActions actions = this.actions[selectedCard];
                if (actions.IsThrowable) {
                    if (selectedTrick.Count == 0) {
                        if (ThrowCard(selectedCard)) {
                            eventHandler.CardThrown(selectedCard);
                        } else {
                            eventHandler.UnableToThrow(selectedCard);
                        }
                    } else {
                        eventHandler.MustThrowCard(selectedCard);
                    }
                } else {
                    if (selectedTrick.Count > 0) {
                        bool scopa;
                        if (TakeTrick(selectedCard, selectedTrick, out scopa)) {
                            eventHandler.TrickTaken(selectedCard, selectedTrick);
                        } else {
                            eventHandler.UnableToTakeTrick(selectedCard, selectedTrick);
                        }
                        if (scopa) {
                            eventHandler.Scopa(selectedCard, selectedTrick);
                        }
                    } else {
                        eventHandler.MustTakeTrick(selectedCard);
                    }
                }
            }
        }

		/// <summary>
		/// Determine if the current player can take only one action with the cards they are
		/// currently holding if so return that action so that it can be taken.
		/// </summary>
		public GameAction AutoAction {
			get {
				PopulateActions();

				if (table.Count == 0) {
					return GameAction.Tira;
				}
				// bool possibleThrow = Current.Any<Card>(a => ValidateThrow(a));
				// bool possibleTake = Current.Any<Card>(a => !ValidateThrow(a));
				// if (possibleThrow && !possibleTake) {
				if (actions.CanThrow && !actions.CanTrick) {
					return GameAction.Tira;
				}
				// if (possibleTake && !possibleThrow) {
				if (actions.CanTrick && !actions.CanThrow) {
					return GameAction.Prendi;
				}
				return GameAction.Nessuna;
			}
		}

		/// <summary>
		/// Throw the given Card from the current Player's hand onto the table.
		/// This action can only be taken if a card of equal value is not on the
		/// table nor is there a set of cards on the table such that the sum of
		/// their values are equal to the value of the thrown card.
		/// If the action is successful play moves to the next player.
		/// </summary>
		/// <param name="fromHand">Card to be thrown onto the table</param>
		/// <returns>Flag indicating success or failure of the action</returns>
		public bool ThrowCard(Card fromHand) {
			if (ValidateThrow(fromHand)) {
				Current.Hand.Remove(fromHand);
				table.Add(fromHand);

				currentPlayer = ++currentPlayer % players.Count;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Take a trick of the given set of cards from the table using the given
		/// card from the current players hand.  This action can only be taken if
		/// the sum of the value of the cards from the table is equal to the value
		/// of the card being used to take the trick.  Also it is not valid to take
		/// a trick consisting of multiple cards when there is a card of equal value
		/// to the card from the hand on the table.
		/// If the action is successful play moves to the next player.
		/// </summary>
		/// <param name="fromHand">The card from the players hand played to take the trick</param>
		/// <param name="fromTable">The cards from the table that compose the trick</param>
		/// <param name="scopa">Flag indicating if the trick cleared the table (caled a Scopa)</param>
		/// <returns>Flag indicating success or failure of the action</returns>
		public bool TakeTrick(Card fromHand, List<Card> fromTable, out bool scopa) {
			if (ValidateTrick(fromHand, fromTable)) {
				Current.Hand.Remove(fromHand);
				foreach (Card c in fromTable) {
					table.Remove(c);
				}
				scopa = table.Count == 0 && !IsRoundOver;
				List<Card> trick = new List<Card>(fromTable);
				trick.Add(fromHand);
				Current.TakeTrick(trick, scopa);
				lastTrick = Current;

				currentPlayer = ++currentPlayer % players.Count;
				return true;
			}
			scopa = false;
			return false;
		}

		/// <summary>
		/// Take the last trick from the table at the end of the round.  This means that
		/// the player who last successfully took a trick by playing a card will recieve
		/// all of the remaining cards on the table.
		/// </summary>
		public void TakeLastTrick() {
			if (table.Count > 0) {
				lastTrick.TakeTrick(table, false);
				table.Clear();
			}
		}

		#endregion // Action Methods

		#region Validation Methods

		/// <summary>
		/// Validate the card thrown against the cards on the table to assure that it can not
		/// take a trick from the cards that are on the table.
		/// </summary>
		/// <param name="thrownCard">The card being thrown from the players hand</param>
		/// <returns>Flag indicating the validity of the throw (i.e. if it is possible or not)</returns>
		public bool ValidateThrow(Card thrownCard) {
			return ScopaGame.ValidateThrow(table, thrownCard);
		}
		/// <summary>
		/// Validate the card thrown against the cards on the table to assure that it can not
		/// take a trick from the cards that are on the table.
		/// </summary>
		/// <param name="set">The set of cards to validate the possible throw against</param>
		/// <param name="thrownCard">The card being thrown from the players hand</param>
		/// <returns>Flag indicating the validity of the throw (i.e. if it is possible or not)</returns>
		public static bool ValidateThrow(List<Card> set, Card thrownCard) {
			Debug.Assert(set != null);
			// If there is a card of equal value on the table it is not a valid throw
			List<Card> table = new List<Card>(set);
			if (!table.Any<Card>(a => (int)a == (int)thrownCard)) {
				// If there is any combination of cards of equal value to the thrown card is it not a valid throw
				return !ScopaGame.Enumerate(table, thrownCard);
			}
			return false;
		}

		/// <summary>
		/// Validate the card being played from the players hand can take the cards in the trick.
		/// It is not valid to take a trick in the following cases:
		/// A trick whose sum of the value of the cards does not equal the value of the card used to play the trick.
		/// A trick that consists of multiple cards when the is a card of equals value to the card being played on the table.
		/// A trick that does not contain any cards from the table.
		/// </summary>
		/// <param name="playedCard">The card being played from the players hand</param>
		/// <param name="trickSet">The set of cards from the table involved in the trick</param>
		/// <returns>Flag indicating the validity of the trick (i.e. is it is possible or not)</returns>
		public bool ValidateTrick(Card playedCard, List<Card> trickSet) {
			return ScopaGame.ValidateTrick(table, playedCard, trickSet);
		}
		/// <summary>
		/// Validate the card being played from the players hand can take the cards in the trick.
		/// It is not valid to take a trick in the following cases:
		/// A trick whose sum of the value of the cards does not equal the value of the card used to play the trick.
		/// A trick that consists of multiple cards when the is a card of equals value to the card being played on the table.
		/// A trick that does not contain any cards from the table.
		/// </summary>
		/// <param name="set">The set of cards to validate the possible trick against</param>
		/// <param name="playedCard">The card being played from the players hand</param>
		/// <param name="trickSet">The set of cards from the table involved in the trick</param>
		/// <returns>Flag indicating the validity of the trick (i.e. is it is possible or not)</returns>
		public static bool ValidateTrick(List<Card> set, Card playedCard, List<Card> trickSet) {
			Debug.Assert(set != null && trickSet != null);
			// If the table is empty you can not pick up
			if (set.Count == 0) {
				return false;
			}
			// You must pick up at least one card
			if (trickSet.Count == 0) {
				return false;
			}
			if (trickSet.Count == 1) {
				// The card values must match
				return (int)playedCard == (int)trickSet[0];
			}

			// The value of the cards taken must sum to the value of the card used and
			// there can not be a card of the equal value to the card used on the table
			if (!set.Any<Card>(a => (int)a == (int)playedCard)) {
				int sum = trickSet.Sum<Card>(a => (int)a);
				int value = (int)playedCard;
				if (value == sum) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Enumerate all of the possible choice combinations of card sets on the table for use in determining
		/// if a throw is legal or not.  A throw is not legal when a trick can be taken using the card.  This
		/// means that if there is a card of equal value on the table the throw is not legal.  If there is a
		/// set of cards on the table that sum to the value of the thrown card then the throw is not legal.
		/// To this end we can remove all cards from the table that are higher value than that of the thrown
		/// card as well as eliminate partial sets that sum to a value greater than that of the thrown card.
		/// Also this method fails fast once a set is identified to meet the criteria then the method returns
		/// without checking for other possible sets.
		/// </summary>
		/// <param name="set">The set of cards to select choices from</param>
		/// <param name="target">The target card to check value against</param>
		/// <returns>
		/// Flag indicating if the given set of cards contains a subset of cards whose values sum to the value
		/// of the target card.
		/// </returns>
		private static bool Enumerate(List<Card> set, Card target) {
			Debug.Assert(set != null);
			set = new List<Card>(set);
			// Remove all Cards of value greater than the target, combinations with those cards are impossible
			set.RemoveAll(delegate(Card c) {
				return (int)c > (int)target;
			});
			if (set.Count > 1) {
				for (int k = 2; k <= set.Count; ++k) {
					ChoiceEnumerable<Card> enumerable = new ChoiceEnumerable<Card>(k, set.ToArray<Card>());
					foreach (Card[] cards in enumerable) {
						if ((int)target == cards.Sum<Card>(a => (int)a)) {
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Enumerate all of the possible choice combinations of card sets on the table for use in determining
		/// possible plays.
		/// To this end we can remove all cards from the table that are higher value than that of the thrown
		/// card as well as eliminate partial sets that sum to a value greater than that of the thrown card.
		/// </summary>
		/// <param name="set">The set of cards to select choices from</param>
		/// <param name="target">The target card to check value against</param>
		/// <returns>
		/// Flag indicating if the given set of cards contains a subset of cards whose values sum to the value
		/// of the target card.
		/// </returns>
		public static List<List<Card>> EnumerateAll(List<Card> set, Card target) {
			Debug.Assert(set != null);
			set = new List<Card>(set);
			set.RemoveAll(delegate(Card c) {
				return (int)c > (int)target;
			});
			List<List<Card>> list = new List<List<Card>>();
			if (set.Count > 0) {
				for (int k = 1; k <= set.Count; ++k) {
					ChoiceEnumerable<Card> enumerable = new ChoiceEnumerable<Card>(k, set.ToArray<Card>());
					foreach (Card[] cards in enumerable) {
						if ((int)target == cards.Sum<Card>(a => (int)a)) {
							list.Add(new List<Card>(cards));
						}
					}
				}
			}
			return list;
		}

		#endregion // Validation Methods

        #region Scoring Methods

		public List<IScopaPlayer> ScoreRound() {
            List<IScopaPlayer> players = new List<IScopaPlayer>(this.players);
            foreach (IScopaPlayer player in players) {
				player.RoundScore = 0;
			}

			AddPoint(a => a.CardCount);
			AddPoint(a => a.DenariCount);
			AddPoint(a => a.PrimieraValue);

			// Add Sette Bello and Scopas to the score
			foreach (IScopaPlayer player in players) {
				player.RoundScore += player.SetteBello ? 1 : 0;
				player.RoundScore += player.ScopaCount;
				player.Points += player.RoundScore;
			}

			// Sort the players by total number of points (descending)
            players.Sort(delegate(IScopaPlayer a, IScopaPlayer b) {
				return -a.Points.CompareTo(b.Points);
			});
			return players;
		}

        private void AddPoint(Func<IScopaPlayer, decimal> predicate) {
            IScopaPlayer player = Utilities.MaximumElement(players, predicate);
            if (player != null) {
                player.RoundScore += 1;
            }
        }

        #endregion // Scoring Methods

        #region NewDeck

        static Deck NewDeck {
#if USE_STACKED
            get { return new StackedDeck(); }
#else
            get { return new Deck(); }
#endif
        }

        #endregion

    }

}
