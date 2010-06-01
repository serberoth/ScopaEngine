// #define DEBUG_AI
// #define DEBUG_TRICK
// #define DEBUG_SCORE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NIESoftware.Forms {

	using Scopa;

	partial class GameForm : Form {

        private const int COMPUTER_INITIAL_INTERVAL = 2000;
        // private const int COMPUTER_GOPLY_INTERVAL = 1000;
        private const int COMPUTER_STEP_INTERVAL = 1500;

		private const string TrickTrackerLabelBase = @"Carte:
Premiera:
Denari:
Scope:";

		private const string PLAYER_A_NAME = "Computer";
		private const string PLAYER_B_NAME = "Player";

        private ScopaEventHandler eventHandler;
        private ScopaGame game;
        private PlaySelection selection;
        private AIScopaPlayer computer;
        private UIScopaPlayer humanPlayer;

		public GameForm() {
			InitializeComponent();
            InitializeEventHandler();
			NewGame();
		}

        private void InitializeEventHandler() {
            eventHandler = new ScopaEventHandler();
            eventHandler.MustThrowCard += delegate(Card card) {
                MessageBox.Show("The " + card + " must be thrown", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            eventHandler.MustTakeTrick += delegate(Card card) {
                MessageBox.Show("The " + card + " must take a trick", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            eventHandler.UnableToThrow += delegate(Card card) {
                MessageBox.Show("The " + card + " can not be thrown", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            eventHandler.UnableToTakeTrick += delegate(Card card, List<Card> list) {
                MessageBox.Show("The " + card + " can not take " + Card.ToString(list), "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            eventHandler.CardThrown += delegate(Card card) { };
            eventHandler.TrickTaken += delegate(Card card, List<Card> list) { };
            eventHandler.Scopa += delegate(Card card, List<Card> list) {
                MessageBox.Show("Scopa!!!", "Scopa", MessageBoxButtons.OK);
            };
        }

		private void NewGame() {
			game = new ScopaGame();
            selection = new PlaySelection();
            computer = new AIScopaPlayer(PLAYER_A_NAME, game);
            humanPlayer = new UIScopaPlayer(PLAYER_B_NAME, game, selection);

            game.AddPlayers(new List<IScopaPlayer> { computer, humanPlayer, });
			game.BeginGame();
			game.DealStartingHand();

			playerAName.Text = computer.Name;
			playerBName.Text = humanPlayer.Name;

			ShowDealerImage();
			ShowTurnImage();
			ShowCards();
			UpdateScores();

            if (computer.IsPly) {
                CreateDelayedEvent(delegate(object sender, EventArgs e) {
                    ComputerStep();
                }, COMPUTER_INITIAL_INTERVAL);
            }
		}

		private void CreateDelayedEvent(EventHandler handler, int interval) {
			Timer timer = new Timer();
			timer.Interval = interval;
			timer.Tick += delegate(object sender, EventArgs e) {
				timer.Enabled = false;
				handler(sender, e);
			};
			timer.Enabled = true;
		}

		#region Show Image and Text Methods

		private void ShowDealerImage() {
			switch (game.Dealer.Name) {
				case PLAYER_A_NAME:
					if (game.DeckCount > 0) {
						playerADeck.Image = global::NIESoftware.Properties.Resources.Cards;
						playerADeck.Image.Tag = "Dealer - " + game.DeckCount + " Cards left";
					} else {
						playerADeck.Image = null;
						toolTip.SetToolTip(playerADeck, "");
					}
					playerBDeck.Image = null;
					toolTip.SetToolTip(playerBDeck, "");
					break;
				case PLAYER_B_NAME:
					playerADeck.Image = null;
					toolTip.SetToolTip(playerADeck, ""); 
					if (game.DeckCount > 0) {
						playerBDeck.Image = global::NIESoftware.Properties.Resources.Cards;
						playerBDeck.Image.Tag = "Dealer - " + game.DeckCount + " Cards left";
					} else {
						playerBDeck.Image = null;
						toolTip.SetToolTip(playerBDeck, "");
					}
					break;
			}
		}

		private void ShowTurnImage() {
			switch (game.Current.Name) {
				case PLAYER_A_NAME:
					playerATurn.Image = global::NIESoftware.Properties.Resources.Turn;
					playerATurn.Image.Tag = PLAYER_A_NAME + "'s Turn";
					playerBTurn.Image = null;
					toolTip.SetToolTip(playerBTurn, "");
					break;
				case PLAYER_B_NAME:
					playerATurn.Image = null;
					toolTip.SetToolTip(playerATurn, "");
					playerBTurn.Image = global::NIESoftware.Properties.Resources.Turn;
					playerBTurn.Image.Tag = PLAYER_B_NAME + "'s Turn";
					break;
			}
		}

		private void ShowCards() {
#if DEBUG_AI
			ShowCardList(new PictureBox[] { playerAHand0, playerAHand1, playerAHand2, }, computer.Hand);
#else
            ShowCardBackList (new PictureBox[] { playerAHand0, playerAHand1, playerAHand2, }, computer.Hand);
#endif
			ShowCardList(new PictureBox[] { playerBHand0, playerBHand1, playerBHand2, }, humanPlayer.Hand);
            ShowCardList(new PictureBox[] { table0, table1, table2, table3, table4, table5, table6, table7, }, game.Table);
		}

		private void ShowCardList(PictureBox[] pbs, List<Card> cardList) {
			int i;
			for (i = 0; i < cardList.Count && i < pbs.Length; ++i) {
				SetCardImage(pbs[i], cardList[i]);
			}
			if (i < pbs.Length) {
				for (; i < pbs.Length; ++i) {
					pbs[i].Image = null;
					toolTip.SetToolTip(pbs[i], "");
				}
			}
		}

        private void ShowCardBackList(PictureBox[] pbs, List<Card> cardList) {
            int i;
            for (i = 0; i < cardList.Count && i < pbs.Length; ++i) {
                if (pbs[i].Image == null || !(pbs[i].Image.Tag is Card) || (!cardList[i].Equals ((Card)pbs[i].Image.Tag))) {
                    pbs[i].Image = global::NIESoftware.Properties.Resources.CardBack;
                    toolTip.SetToolTip(pbs[i], "");
                }
            }
            if (i < pbs.Length) {
                for (; i < pbs.Length; ++i) {
                    pbs[i].Image = null;
                    toolTip.SetToolTip(pbs[i], "");
                }
            }
        }

		private void SetCardImage(PictureBox pb, Card card) {
			pb.Image = CardToResource(card);
			pb.Image.Tag = card;
            toolTip.SetToolTip(pb, "");
        }

        private void SetCardHighlightedImage(PictureBox pb, Card card) {
            Image resource = CardToResource(card);
            Bitmap highlighted = new Bitmap(resource.Width, resource.Height);
            using (Graphics g = Graphics.FromImage(highlighted)) {
                g.DrawImage(resource, 0, 0, resource.Width, resource.Height);
                Bitmap highlight = global::NIESoftware.Properties.Resources.Highlight;
                g.DrawImage(highlight, 0, 0, highlighted.Width, highlighted.Height);
            }
            pb.Image = highlighted;
            pb.Image.Tag = card;
            toolTip.SetToolTip(pb, "");
        }

        private void RevealCard(Card card) {
            PictureBox[] pbs = new PictureBox[] { playerAHand0, playerAHand1, playerAHand2, };
            PictureBox pb = pbs[computer.IndexOf(card)];
            if (pb != null) {
#if DEBUG_AI
                SetCardHighlightedImage (pb, card);
#else
                SetCardImage(pb, card);
#endif
            }
        }

        private void UpdateScores() {
            UpdateScores(true);
        }
		private void UpdateScores(bool updateTrickTracker) {
			playerAScore.Text = computer.Points + " (+" + computer.RoundScore + ")";
            playerBScore.Text = humanPlayer.Points + " (+" + humanPlayer.RoundScore + ")";
            if (updateTrickTracker) {
                UpdateTrickTracker(playerATrickTracker, playerATrickTrackerCount, computer);
                UpdateTrickTracker(playerBTrickTracker, playerBTrickTrackerCount, humanPlayer);
            }
		}

		private void UpdateTrickTracker(Label trickTracker, Label trickTrackerCount, IScopaPlayer player) {
			trickTracker.Text = TrickTrackerLabelBase + Environment.NewLine
				+ (player.SetteBello ? "Sette Bello" : "");
			trickTrackerCount.Text = player.CardCount + Environment.NewLine
				+ player.PrimieraValue + Environment.NewLine
				+ player.DenariCount + Environment.NewLine
				+ player.ScopaCount;
		}

        private void HighlightTrick(List<Card> cardList) {
            foreach (PictureBox pb in new PictureBox[] { table0, table1, table2, table3, table4, table5, table6, table7, }) {
                if (pb.Image != null && pb.Image.Tag != null && pb.Image.Tag is Card) {
                    Card card = (Card)pb.Image.Tag;
                    if (cardList.Contains<Card>(card)) {
                        SetCardHighlightedImage(pb, card);
                    }
                }
            }
        }

		#endregion

		#region Input Handler Methods

		private void image_MouseHover(object sender, EventArgs e) {
			PictureBox pb = sender as PictureBox;
			MouseEventArgs eventArgs = (e as MouseEventArgs);
			if (pb.Image != null && pb.Image.Tag != null) {
				string value = "";
				if (pb.Image.Tag is string) {
					value = pb.Image.Tag as string;
				} else if (pb.Image.Tag is Card) {
					value = pb.Image.Tag.ToString ();
				}
				toolTip.SetToolTip(pb, value);
			}
		}

		private void table_Click(object sender, EventArgs e) {
			PictureBox pb = sender as PictureBox;
			MouseEventArgs eventArgs = (e as MouseEventArgs);
			if (eventArgs.Button == MouseButtons.Left && !game.IsGameOver) {
				if (pb.Image != null) {
					if (pb.Image.Tag is Card) {
						Card card = (Card)pb.Image.Tag;
						Image resource = CardToResource(card);
						if (!selection.IsInTrick (card)) {
                            SetCardHighlightedImage(pb, card);
                            selection.AddToTrick(card);
						} else {
                            SetCardImage(pb, card);
                            selection.RemoveFromTrick(card);
						}
					}
				}
			}
		}

		private void playerHand_DoubleClick(object sender, EventArgs e) {
			PictureBox pb = sender as PictureBox;
			MouseEventArgs eventArgs = (e as MouseEventArgs);
			if (eventArgs.Button == MouseButtons.Left && !game.IsGameOver) {
                if (pb.Image != null) {
                    if (humanPlayer.IsPly) {
                        selection.SelectedCard = (Card?)pb.Image.Tag;
                        StepGame(humanPlayer);
                        if (!humanPlayer.IsPly) {
                            selection.Clear();
                        }

                        if (computer.IsPly) {
                            ComputerStep();
                            /*
                            CreateDelayedEvent(delegate(object _sender, EventArgs _e) {
                                ComputerStep();
                            }, COMPUTER_GOPLY_INTERVAL);
                             */
                        }
                    }
                }
			}
		}

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e) {
            NewGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        #endregion // Input Handler Methods

		#region Game Methods

        private void ComputerStep() {
            Card selectedCard = computer.SelectCard();
            List<Card> selectedTrick = computer.SelectTrick(selectedCard);
            HighlightTrick(selectedTrick);
            RevealCard(selectedCard);
            CreateDelayedEvent(delegate(object sender, EventArgs e) {
                StepGame(computer);
            }, COMPUTER_STEP_INTERVAL);
        }

		private void StepGame(IScopaPlayer player) {
            TakeAction(player);
            CompleteAction();
		}

        private void TakeAction(IScopaPlayer player) {
            game.TakeAction(player, eventHandler);
        }

        private void CompleteAction() {
            if (game.IsRoundOver) {
                game.TakeLastTrick();
                UpdateScores();
                ShowCards();
                CompleteRound();
            } else {
                if (game.IsHandOver && !game.IsGameOver) {
                    game.DealHand();
                }

                ShowDealerImage();
                ShowTurnImage();
                ShowCards();
                UpdateScores();
            }
        }

		private void CompleteRound() {
#if DEBUG_SCORE
            ShowCompleteRound_DebugMessage ();
#endif
            List<IScopaPlayer> ranking = game.ScoreRound();
			game.NewRound();

            if (!game.IsGameOver) {
                CreateDelayedEvent(delegate(object sender, EventArgs e) {
                    BeginNextRound();
                }, 4000);
            } else {
                ShowDealerImage();
                ShowTurnImage();
                ShowCards();
                UpdateScores(false);

                MessageBox.Show(ranking[0].Name + " Wins the game!", "Game Over", MessageBoxButtons.OK);
            }
        }

        private void BeginNextRound() {
            if (!game.IsGameOver) {
                game.DealStartingHand();
            }
            if (game.IsHandOver && !game.IsGameOver) {
                game.DealHand();
            }

            ShowDealerImage();
            ShowTurnImage();
            ShowCards();
            UpdateScores();

            if (computer.IsPly) {
                ComputerStep();
                /*
                CreateDelayedEvent(delegate(object _sender, EventArgs _e) {
                    ComputerStep();
                }, COMPUTER_GOPLY_INTERVAL);
                 */
            }
        }

#if DEBUG
        private void ShowCompleteRound_DebugMessage() {
			StringBuilder builder = new StringBuilder();
			foreach (IScopaPlayer player in game.Players) {
				builder.Append(player.Name).Append(" ha presa(").Append(player.CardCount).Append("): ");
				List<Card> cardsTaken = player.CardsTaken;
				cardsTaken.Sort();
				builder.Append(Card.ToString(cardsTaken)).Append(Environment.NewLine);
				builder.Append("Primiera: " + player.PrimieraValue).Append(Environment.NewLine);
				builder.Append("Denari: " + player.DenariCount).Append(Environment.NewLine);
				builder.Append("Settebello: " + player.SetteBello).Append(Environment.NewLine);
				builder.Append("Scopa: " + player.ScopaCount).Append(Environment.NewLine);
				builder.Append(Environment.NewLine);
			}
			MessageBox.Show(builder.ToString());
        }
#endif

		#endregion // Game Methods

		#region CardToResource

		private Image CardToResource(Card card) {
			switch (card.Suit) {
				case Suit.Denari:
					return DenariCardToResource(card);
				case Suit.Coppe:
					return CoppeCardToResource(card);
				case Suit.Bastone:
					return BastoneCardToResource(card);
				case Suit.Spade:
					return SpadeCardToResource(card);
			}
			return null;
		}
		private Image DenariCardToResource(Card card) {
			switch (card.Value) {
				case Value.Asso:
					return global::NIESoftware.Properties.Resources.AssoDenari;
				case Value.Due:
					return global::NIESoftware.Properties.Resources.DueDenari;
				case Value.Tre:
					return global::NIESoftware.Properties.Resources.TreDenari;
				case Value.Quattro:
					return global::NIESoftware.Properties.Resources.QuattroDenari;
				case Value.Cinque:
					return global::NIESoftware.Properties.Resources.CinqueDenari;
				case Value.Sei:
					return global::NIESoftware.Properties.Resources.SeiDenari;
				case Value.Sette:
					return global::NIESoftware.Properties.Resources.SetteDenari;
				case Value.Fante:
					return global::NIESoftware.Properties.Resources.FanteDenari;
				case Value.Cavallo:
					return global::NIESoftware.Properties.Resources.CavalloDenari;
				case Value.Re:
					return global::NIESoftware.Properties.Resources.ReDenari;
			}
			return null;
		}
		private Image CoppeCardToResource(Card card) {
			switch (card.Value) {
				case Value.Asso:
					return global::NIESoftware.Properties.Resources.AssoCoppe;
				case Value.Due:
					return global::NIESoftware.Properties.Resources.DueCoppe;
				case Value.Tre:
					return global::NIESoftware.Properties.Resources.TreCoppe;
				case Value.Quattro:
					return global::NIESoftware.Properties.Resources.QuattroCoppe;
				case Value.Cinque:
					return global::NIESoftware.Properties.Resources.CinqueCoppe;
				case Value.Sei:
					return global::NIESoftware.Properties.Resources.SeiCoppe;
				case Value.Sette:
					return global::NIESoftware.Properties.Resources.SetteCoppe;
				case Value.Fante:
					return global::NIESoftware.Properties.Resources.FanteCoppe;
				case Value.Cavallo:
					return global::NIESoftware.Properties.Resources.CavalloCoppe;
				case Value.Re:
					return global::NIESoftware.Properties.Resources.ReCoppe;
			}
			return null;
		}
		private Image BastoneCardToResource(Card card) {
			switch (card.Value) {
				case Value.Asso:
					return global::NIESoftware.Properties.Resources.AssoBastone;
				case Value.Due:
					return global::NIESoftware.Properties.Resources.DueBastone;
				case Value.Tre:
					return global::NIESoftware.Properties.Resources.TreBastone;
				case Value.Quattro:
					return global::NIESoftware.Properties.Resources.QuattroBastone;
				case Value.Cinque:
					return global::NIESoftware.Properties.Resources.CinqueBastone;
				case Value.Sei:
					return global::NIESoftware.Properties.Resources.SeiBastone;
				case Value.Sette:
					return global::NIESoftware.Properties.Resources.SetteBastone;
				case Value.Fante:
					return global::NIESoftware.Properties.Resources.FanteBastone;
				case Value.Cavallo:
					return global::NIESoftware.Properties.Resources.CavalloBastone;
				case Value.Re:
					return global::NIESoftware.Properties.Resources.ReBastone;
			}
			return null;
		}
		private Image SpadeCardToResource(Card card) {
			switch (card.Value) {
				case Value.Asso:
					return global::NIESoftware.Properties.Resources.AssoSpade;
				case Value.Due:
					return global::NIESoftware.Properties.Resources.DueSpade;
				case Value.Tre:
					return global::NIESoftware.Properties.Resources.TreSpade;
				case Value.Quattro:
					return global::NIESoftware.Properties.Resources.QuattroSpade;
				case Value.Cinque:
					return global::NIESoftware.Properties.Resources.CinqueSpade;
				case Value.Sei:
					return global::NIESoftware.Properties.Resources.SeiSpade;
				case Value.Sette:
					return global::NIESoftware.Properties.Resources.SetteSpade;
				case Value.Fante:
					return global::NIESoftware.Properties.Resources.FanteSpade;
				case Value.Cavallo:
					return global::NIESoftware.Properties.Resources.CavalloSpade;
				case Value.Re:
					return global::NIESoftware.Properties.Resources.ReSpade;
			}
			return null;
		}

		#endregion // CardToResource

	}

}
