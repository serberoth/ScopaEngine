// #define USE_STACKED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NIESoftware.Scopa {

	class ScopaGame : AbstractScopaGame {
		private const int DEFAULT_WINNING_SCORE = 11;

		/// <summary>
		/// Create a new ScopaGame instance with the default winning score of 11 points.
		/// </summary>
		public ScopaGame() : this(DEFAULT_WINNING_SCORE) { }
		/// <summary>
		/// Create a new ScopaGame with the specified number of points required to win the game.
		/// </summary>
		/// <param name="points">The number of points required to win the game.</param>
		public ScopaGame(int requiredPoints) : base (requiredPoints) {
		}

		/// <summary>
		/// Deal the starting hand of the round.  This puts 3 cards in every players hand
		/// and 4 cards on the table.
		/// </summary>
		public override void DealStartingHand() {
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
		public override void DealHand() {
			for (int i = 0; i < 3; ++i) {
				for (int j = 0; j < players.Count; ++j) {
					players[(currentPlayer + j) % players.Count].Hand.Add(deck.Deal);
				}
			}
		}

    }

}
