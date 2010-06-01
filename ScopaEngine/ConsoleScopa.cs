using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware {

	using Scopa;

    class ConsoleScopaPlayer : AbstractScopaPlayer {

        public ConsoleScopaPlayer(string name, ScopaGame game) : base(name, game) { }

        public override Card SelectCard() {
            game.PopulateActions();
            if (Hand.Count > 1) {
                Console.Out.WriteLine("Dal mano");
                return Hand[SelectWhich(Hand.Count)];
            }
            return Hand[0];
        }

        private List<Card> SelectCardsFromTable(ScopaGame game, GameAction action, Card fromHand) {
            List<Card> fromTable = new List<Card>();
            if (game.Table.Count > 0) {
                CardActions actions = game.Actions[fromHand];
                Console.Out.WriteLine(actions.PossibleTricks.Count + " trick(s) are possible with " + fromHand);
                foreach (List<Card> possibleTrick in actions.PossibleTricks) {
                    Console.Out.WriteLine("Possible Trick: " + Card.ToString(possibleTrick));
                }
                if (actions.PossibleTricks.Count == 1) {
                    fromTable.AddRange(actions.PossibleTricks[0]);
                } else {
                    int index;
                    Console.Out.WriteLine("Dal tavolo");
                    while (SelectWhichWithEscape(game.Table.Count, out index)) {
                        fromTable.Add(game.Table[index]);
                        // If the trick is valid do not wait for input from the user
                        if (game.ValidateTrick(fromHand, fromTable)) {
                            break;
                        }
                    }
                }
            }
            return fromTable;
        }

        public override List<Card> SelectTrick(Card card) {
            game.PopulateActions();
            CardActions actions = game.Actions[card];
            if (!actions.IsThrowable) {
                if (actions.PossibleTricks.Count == 1) {
                    return actions.PossibleTricks[0];
                }
                List<Card> selectedTrick = new List<Card>();
                int index;
                Console.Out.WriteLine("Dal tavolo");
                while (SelectWhichWithEscape(game.Table.Count, out index)) {
                    selectedTrick.Add(game.Table[index]);
                    // If the trick is valid do not wait for input from the user
                    if (game.ValidateTrick(card, selectedTrick)) {
                        break;
                    }
                }
                return selectedTrick;
            }
            return new List<Card>();
        }

        private static int SelectWhich(int cardCount) {
            int which = -1;
            do {
                Console.Out.Write("Quale #? ");
                try {
                    which = int.Parse(Console.In.ReadLine());
                } catch (FormatException) {
                    which = -1;
                }
            } while (!(which > 0 && which <= cardCount));
            return which - 1;
        }

        private static bool SelectWhichWithEscape(int cardCount, out int which) {
            which = -1;
            do {
                Console.Out.Write("Quale (# o (F)inito)? ");
                string input = Console.In.ReadLine();
                if ("F".Equals(input.ToUpper())) {
                    return false;
                }
                try {
                    which = int.Parse(input);
                } catch (FormatException) {
                    which = -1;
                }
            } while (!(which > 0 && which <= cardCount));
            which -= 1;
            return true;
        }

    }

	class ConsoleScopa {

        private ScopaEventHandler eventHandler;

        public ConsoleScopa() {
            eventHandler = new ScopaEventHandler();
            eventHandler.MustThrowCard = delegate(Card card) {
                Console.Out.WriteLine("Deve tirare " + card);
            };
            eventHandler.MustTakeTrick = delegate(Card card) {
                Console.Out.WriteLine("Deve prendere con " + card);
            };
            eventHandler.UnableToThrow = delegate(Card card) {
                Console.Out.WriteLine("Non é possibile tirare " + card);
            };
            eventHandler.UnableToTakeTrick = delegate(Card card, List<Card> trick) {
                Console.Out.WriteLine("Non é possibile prendere " + Card.ToString(trick) + " con " + card);
            };
            eventHandler.UnableToThrow = delegate(Card card) {
                Console.Out.WriteLine("Non é possibile tirare " + card);
            };
            eventHandler.CardThrown = delegate(Card card) {
					Console.Out.WriteLine("Tira: " + card);
            };
            eventHandler.TrickTaken = delegate(Card card, List<Card> trick) {
                Console.Out.WriteLine("Ha preso: " + Card.ToString(trick) + " usa " + card);
            };
            eventHandler.Scopa = delegate(Card card, List<Card> trick) {
                Console.Out.WriteLine("Scopa!!!");
            };
        }

		public void Run() {
			bool running = true;

			Console.Out.WriteLine("Scopa - Versione di Console");
			Console.Out.WriteLine("==================================================");
			Console.Out.WriteLine("Scritto Di: Nicola DiPasquale");
			Console.Out.WriteLine("Copyright (c) 2010 N.I.E. Software");
			while (running) {
				string input = TakeAction("Si prego di scegliere un azione: (G)ioco Nuovo, (E)sci");
				if ("G".Equals(input.ToUpper())) {
					DoGame();
				} else if ("E".Equals(input.ToUpper())) {
					running = false;
					continue;
				} else {
					Console.Out.WriteLine("Non capisco: '" + input + "'");
				}
			}
			Console.Out.WriteLine("Grazie per avete giocato.");
		}

		#region Input Methods
		private static string TakeAction(string message) {
			Console.Out.WriteLine(message);
			Console.Out.Write("> ");
			return Console.In.ReadLine();
		}

		private static int SelectWhich(int cardCount) {
			int which = -1;
			do {
				Console.Out.Write("Quale #? ");
				try {
					which = int.Parse(Console.In.ReadLine());
				} catch (FormatException) {
					which = -1;
				}
			} while (!(which > 0 && which <= cardCount));
			return which - 1;
		}

		private static bool SelectWhichWithEscape(int cardCount, out int which) {
			which = -1;
			do {
				Console.Out.Write("Quale (# o (F)inito)? ");
				string input = Console.In.ReadLine();
				if ("F".Equals(input.ToUpper())) {
					return false;
				}
				try {
					which = int.Parse(input);
				} catch (FormatException) {
					which = -1;
				}
			} while (!(which > 0 && which <= cardCount));
			which -= 1;
			return true;
		}
		#endregion // Input Methods

		#region Output Methods

		private static void PrintTable(ScopaGame game) {
			Console.Out.WriteLine("Sul Tavolo");
			int index = 0;
			foreach (Card card in game.Table) {
				Console.Out.WriteLine(" " + (++index) + ") " + card);
			}
			Console.Out.WriteLine();
		}

		private static void PrintHand(ScopaGame game) {
			Console.Out.WriteLine("La mano di " + game.Current.Name + (game.Current.Equals(game.Dealer) ? "*" : ""));
			int index = 0;
			Console.Out.Write(" ");
			foreach (Card card in game.Current.Hand) {
				if (index++ > 0) {
					Console.Out.Write(", ");
				}
				Console.Out.Write(index + ") " + (game.ValidateThrow(card) ? "-" : "+") + card);
			}
			Console.Out.WriteLine();
		}

		private static void PrintRanking(List<IScopaPlayer> players) {
			int index = 0;
			foreach (IScopaPlayer player in players) {
				Console.Out.WriteLine(++index + ") " + player.Name + ": " + player.Points + " (+" + player.RoundScore + ")");
			}
		}

		#endregion // Output Methods

		#region Game Methods

		private void DoGame() {
			ScopaGame game = new ScopaGame();
			AddPlayers(game);
			game.BeginGame();
			do {
				game.DealStartingHand();
				if (!PlayRound(game)) {
					Console.Out.WriteLine(game.Current.Name + " ha concesso.");
					return;
				}

				foreach (IScopaPlayer player in game.Players) {
					Console.Out.Write(player.Name + " ha presa(" + player.CardCount + ")");
#if DEBUG
                    List<Card> cardsTaken = player.CardsTaken;
                    cardsTaken.Sort();
                    Console.Out.Write(": " + Card.ToString(cardsTaken));
#endif
                    Console.Out.WriteLine();
					Console.Out.WriteLine("Primiera: " + player.PrimieraValue);
					Console.Out.WriteLine("Denari: " + player.DenariCount);
					Console.Out.WriteLine("Settebello: " + player.SetteBello);
					Console.Out.WriteLine("Scopa: " + player.ScopaCount);
					Console.Out.WriteLine();
				}

				PrintRanking(game.ScoreRound());
				game.NewRound();
			} while (!game.IsGameOver);
		}

		#region Game Setup Methods

		private static void AddPlayers(ScopaGame game) {
			string name = null;
			int count = 0;
			do {
				do {
					Console.Out.Write("Add A Player: ");
					name = Console.In.ReadLine();
					if (name != null && !"".Equals(name)) {
                        if (count == 0) {
                            game.AddPlayer(new ConsoleScopaPlayer(name, game));
                        } else {
                            game.AddPlayer(new AIScopaPlayer(name, game));
                        }
						++count;
					}
				} while (name != null && !"".Equals(name));
			} while (count < 2);
            /*
			Console.Out.Write("Giocatore A: ");
			string a = Console.In.ReadLine();
			a = a != null && !"".Equals(a) ? a : "[Giocatore A]";
			Console.Out.Write("Giocatore B: ");
			string b = Console.In.ReadLine();
			b = b != null && !"".Equals(b) ? b : "[Giocatore B]";
            Console.Out.Write("Giocatore C: ");
            string c = Console.In.ReadLine();
            c = c != null && !"".Equals(c) ? c : "[Giocatore C]";
			game.AddPlayers(new List<IScopaPlayer> {
                new AIScopaPlayer (a, game),
                new AIScopaPlayer (b, game),
                new AIScopaPlayer (c, game),
            });
             */
		}

		#endregion // Game Setup Methods

		#region Game Play Methods

		private bool PlayRound(ScopaGame game) {
			do {
				// If everyone has played out their hand deal a new one
				if (game.IsHandOver) {
					game.DealHand();
				}
				PrintTable(game);
				PrintHand(game);
                game.TakeAction(game.Current, eventHandler);
			} while (!game.IsRoundOver);
			if (game.Table.Count > 0) {
				Console.Out.WriteLine(game.LastTrick.Name + " prende il resto: " + Card.ToString(game.Table));
			}
			game.TakeLastTrick();
			return true;
		}

		/*
        private GameAction GetUserAction() {
			do {
				string action = TakeAction("Azione? (T)ira, (P)rendi, (C)oncede");
				action = action != null ? action.Trim().ToUpper() : "";
				if ("C".Equals(action)) {
					return GameAction.Concede;
				} else if ("T".Equals(action)) {
					return GameAction.Tira;
				} else if ("P".Equals(action)) {
					return GameAction.Prendi;
				} else {
					Console.Out.WriteLine("Non capisco: '" + action + "'");
				}
			} while (true);
		}

		private int GetPossibleCount(ScopaGame game, GameAction action) {
			if (GameAction.Tira.Equals(action)) {
				return game.Actions.ThrowableCount;
			} else if (GameAction.Prendi.Equals(action)) {
				return game.Actions.TrickableCount;
			}
			return game.Current.Hand.Count;
		}
		private Card GetSoloPossible(ScopaGame game, GameAction action) {
			if (GameAction.Tira.Equals(action)) {
				return game.Actions.ThrowableList[0].Card;
			} else if (GameAction.Prendi.Equals(action)) {
                return game.Actions.TrickableList[0].Card;
			}
			return default(Card);
		}

		private Card SelectCardFromHand(ScopaGame game, GameAction action) {
			if (game.Current.Hand.Count > 1) {
				if (GetPossibleCount(game, action) == 1) {
					return GetSoloPossible(game, action);
				}

				Console.Out.WriteLine("Dal mano");
				int which = SelectWhich(game.Current.Hand.Count);
				return game.Current.Hand[which];
			}
			return game.Current.Hand[0];
		}

		private List<Card> SelectCardsFromTable(ScopaGame game, GameAction action, Card fromHand) {
			List<Card> fromTable = new List<Card>();
			if (game.Table.Count > 0) {
				CardActions actions = game.Actions[fromHand];
				Console.Out.WriteLine(actions.PossibleTricks.Count + " trick(s) are possible with " + fromHand);
				foreach (List<Card> possibleTrick in actions.PossibleTricks) {
					Console.Out.WriteLine("Possible Trick: " + Card.ToString(possibleTrick));
				}
				if (actions.PossibleTricks.Count == 1) {
					fromTable.AddRange(actions.PossibleTricks[0]);
				} else {
					int index;
					Console.Out.WriteLine("Dal tavolo");
					while (SelectWhichWithEscape(game.Table.Count, out index)) {
						fromTable.Add(game.Table[index]);
						// If the trick is valid do not wait for input from the user
						if (game.ValidateTrick(fromHand, fromTable)) {
							break;
						}
					}
				}
			}
			return fromTable;
		}

		private void PerformAction(ScopaGame game, GameAction action) {
			Card fromHand = SelectCardFromHand(game, action);

			if (GameAction.Tira.Equals(action)) {
				if (!game.ThrowCard(fromHand)) {
					Console.Out.WriteLine("Non é possibile tirare " + fromHand);
				} else {
					Console.Out.WriteLine("Tira: " + fromHand);
				}
				Console.Out.WriteLine();
			} else if (GameAction.Prendi.Equals(action)) {
				List<Card> fromTable = SelectCardsFromTable(game, action, fromHand);

				bool scopa;
				if (!game.TakeTrick(fromHand, fromTable, out scopa)) {
					Console.Out.WriteLine("Non é possibile prendere " + Card.ToString(fromTable) + " con " + fromHand);
				} else {
					Console.Out.WriteLine("Ha preso: " + Card.ToString(fromTable) + " usa " + fromHand);
					if (scopa) {
						Console.Out.WriteLine("Scopa!!!");
					}
				}
				Console.Out.WriteLine();
			}

		}
         */

		#endregion // Game Play Methods

		#endregion // Game Methods
	}

}
