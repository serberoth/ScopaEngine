using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NIESoftware.Scopa {

	enum GameAction : int {
		Nessuna = -1,
		Concede = 0,
		Tira = 1,
		Prendi = 2,
	}

	struct PlayerActions {

		public Player Player { get; set; }
		public bool CanThrow { get; set; }
		public bool CanTrick { get; set; }
		public List<CardActions> CardActions { get; set; }

		public CardActions this[Card card] {
			get { return CardActions.Find(a => a.Card.Equals(card)); }
		}

		public int ThrowableCount {
			get { return CardActions.Count<CardActions>(a => a.IsThrowable); }
		}
		public int TrickableCount {
			get { return CardActions.Count<CardActions>(a => !a.IsThrowable); }
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

		public static explicit operator Card (CardActions actions) {
			return actions.Card;
		}

	}

	class ScopaGame {
		private const int DEFAULT_WINNING_SCORE = 11;

		private int winningScore;
		private List<Player> players;
		private List<Card> table;
		private Deck deck;
		private int currentDealer;
		private int currentPlayer;
		private Player lastTrick;
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
			players = new List<Player>();
			table = new List<Card>();
			deck = new Deck();
			currentDealer = 0;
			currentPlayer = currentDealer + 1;
			lastTrick = null;
			actions = new PlayerActions();
		}

		/// <summary>
		/// Get the required winning score that will complete this game.
		/// </summary>
		public int WinningScore {
			get { return winningScore; }
		}

		/// <summary>
		/// Get the list of players that are currently in this game.
		/// </summary>
		public List<Player> Players {
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
		public Player Dealer {
			get { return players[currentDealer]; }
		}
		
		/// <summary>
		/// Get the current Player that must make a play.
		/// </summary>
		public Player Current {
			get { return players[currentPlayer]; }
		}

		/// <summary>
		/// Get the last Player to successfully take a trick.
		/// </summary>
		public Player LastTrick {
			get { return lastTrick; }
			set { lastTrick = value; }
		}

		public PlayerActions Actions {
			get { return actions; }
		}

		public int DeckCount {
			get { return deck.Count; }
		}

		private void GetActions () {
			if (!Current.Equals (actions.Player)) {
				actions.Player = Current;
				actions.CardActions = new List<CardActions>();
				foreach (Card c in Current) {
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

		/// <summary>
		/// Determine if the deck is exhausted
		/// </summary>
		public bool IsDeckExhausted {
			get { return deck.IsExhausted; }
		}

		/// <summary>
		/// Add all of the players in the list to the current game if it is not in progress.
		/// </summary>
		/// <param name="names">The list of player names to add to the game</param>
		public void AddPlayers(List<string> names) {
			Debug.Assert(!IsInProgress);
			foreach (string name in names) {
				players.Add(new Player(name));
			}
		}

		/// <summary>
		/// Begin a new game.  This shuffles the deck and determines a random player to
		/// deal the cards.
		/// </summary>
		public void BeginGame () {
			deck = new Deck();
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
			foreach (Player player in players) {
				player.NewRound();
			}
			deck = new Deck();
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

		/// <summary>
		/// Determine if the game is in progress.  This means that hands have been dealt from the
		/// deck and or points have been scored.
		/// </summary>
		public bool IsInProgress {
			get { return players.Any<Player>(a => a.Points > 0) && deck.Count < Deck.TOTAL_SIZE; }
		}
		
		/// <summary>
		/// Determine if the current hand is over.  This means that no players hold cards in their
		/// hands.
		/// </summary>
		public bool IsHandOver {
			get { return !players.Any<Player>(a => a.Count > 0); }
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
			// TODO: This condition is not correct it is possible for a player to win a game
			// say 13 - 11 (where both scores are greater than the required score).
			get { return players.FindAll(a => a.Points >= winningScore).Count == 1; }
		}

		#region Action Methods
		/// <summary>
		/// Determine if the current player can take only one action with the cards they are
		/// currently holding if so return that action so that it can be taken.
		/// </summary>
		public GameAction AutoAction {
			get {
				GetActions();

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

		/*
		 * nick ha presa(22): [ Quattro di Denari, Sei di Denari, Re di Denari, Asso di Cop
		 * pe, Due di Coppe, Tre di Coppe, Quattro di Coppe, Cinque di Coppe, Cavallo di Co
		 * ppe, Re di Coppe, Asso di Bastone, Due di Bastone, Quattro di Bastone, Cinque di
		 * Bastone, Sette di Bastone, Re di Bastone, Asso di Spade, Quattro di Spade, Sett
		 * e di Spade, Fante di Spade, Cavallo di Spade, Re di Spade, ]
		 * Premiere: 76
		 * Scopa: 1
		 * 
		 * yuki ha presa(18): [ Asso di Denari, Due di Denari, Tre di Denari, Cinque di Den
		 * ari, Sette di Denari, Fante di Denari, Cavallo di Denari, Sei di Coppe, Sette di
		 * Coppe, Fante di Coppe, Tre di Bastone, Sei di Bastone, Fante di Bastone, Cavall
		 * o di Bastone, Due di Spade, Tre di Spade, Cinque di Spade, Sei di Spade, ]
		 * Premiere: 78
		 * Scopa: 1
		 * 
		 * Computer: Nick 2 (+2)
		 *			 Yuki 2 (+2)
		 * Actual Score:
		 * Nick: Carte, 1 Scopa (+2 punti)
		 * Yuki: Settebello, Primiera, Denari, 1 Scopa (+4 punti)
		 * 
		 */
		public List<Player> ScoreRound() {
			// NOTA: I would use Linq for the following segments, but there does not seem to be 
			// a way to return the referential element only the value when using Max, Min, etc.
			// which does not help when I need to add a point to the Player with that score not
			// report the score.
			List<Player> players = new List<Player>(this.players);
			foreach (Player player in players) {
				player.RoundScore = 0;
			}

			AddPoint(a => a.TrickTracker.CardCount);
			AddPoint(a => a.TrickTracker.DenariCount);
			AddPoint(a => a.TrickTracker.PrimieraValue);

			// Add Sette Bello and Scopas to the score
			foreach (Player player in players) {
				player.RoundScore += player.TrickTracker.SetteBello ? 1 : 0;
				player.RoundScore += player.TrickTracker.ScopaCount;
				player.Points += player.RoundScore;
			}

			// Sort the players by total number of points (descending)
			players.Sort(delegate(Player a, Player b) {
				return -a.Points.CompareTo(b.Points);
			});
			return players;
		}

		private void AddPoint(Func<Player, int> predicate) {
			int max = players.Max<Player>(a => predicate (a));
			List<Player> maxList = players.FindAll(delegate(Player p) {
				return predicate (p) == max;
			});
			if (maxList.Count == 1) {
				maxList[0].RoundScore += 1;
			}
		}

	}

}
