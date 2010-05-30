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

	partial class GameForm : Form, IScopaUIForm {

		private const string TrickTrackerLabelBase = @"Carte:
Premiera:
Denari:
Scope:";

		private const string PLAYER_A_NAME = "Computer";
		private const string PLAYER_B_NAME = "Player  ";

		private ScopaGame game;
        private AIScopaPlayer computer;
        private UIScopaPlayer humanPlayer;
        private Card selectedCard;
		private List<Card> selectedTrick;

		public GameForm() {
			InitializeComponent();
			NewGame();
		}

        public Card SelectedCard {
            get { return selectedCard; }
        }
        public List<Card> SelectedTrick {
            get { return selectedTrick; }
        }

		private void NewGame() {
			game = new ScopaGame();
			selectedTrick = new List<Card>();
			game.AddPlayers(new List<String> { PLAYER_A_NAME, PLAYER_B_NAME, });
			game.BeginGame();
			game.DealStartingHand();
            computer = new AIScopaPlayer (game, game.Players[0]);
            humanPlayer = new UIScopaPlayer("Player", game, game.Players[1], this);

			playerAName.Text = computer.Name;
			playerBName.Text = humanPlayer.Name;

			ShowDealerImage();
			ShowTurnImage();
			ShowCards();
			UpdateScores();

            if (computer.IsPly) {
                CreateDelayedEvent(delegate(object sender, EventArgs e) {
                    ComputerStep();
                }, 5000);
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
			selectedTrick = new List<Card>();
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
			playerAScore.Text = computer.Points + " (+" + computer.RoundScore + ")";
			UpdateTrickTracker(playerATrickTracker, playerATrickTrackerCount, computer);
			playerBScore.Text = humanPlayer.Points + " (+" + humanPlayer.RoundScore + ")";
			UpdateTrickTracker(playerBTrickTracker, playerBTrickTrackerCount, humanPlayer);
		}

		private void UpdateTrickTracker(Label trickTracker, Label trickTrackerCount, IScopaPlayer player) {
			trickTracker.Text = TrickTrackerLabelBase + Environment.NewLine
				+ (player.SetteBello ? "Sette Bello" : "");
			trickTrackerCount.Text = player.CardCount + Environment.NewLine
				+ player.PrimieraValue + Environment.NewLine
				+ player.DenariCount + Environment.NewLine
				+ player.ScopaCount;
		}

        private void HighlightTrick() {
            foreach (PictureBox pb in new PictureBox[] { table0, table1, table2, table3, table4, table5, table6, table7, }) {
                if (pb.Image != null && pb.Image.Tag != null && pb.Image.Tag is Card) {
                    Card card = (Card)pb.Image.Tag;
                    if (selectedTrick.Contains<Card>(card)) {
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
						if (!selectedTrick.Contains<Card>(card)) {
                            SetCardHighlightedImage(pb, card);
							selectedTrick.Add(card);
						} else {
                            SetCardImage(pb, card);
							selectedTrick.Remove(card);
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
                    selectedCard = (Card)pb.Image.Tag;
                    StepGame();

                    if (computer.IsPly) {
                        CreateDelayedEvent(delegate(object _sender, EventArgs _e) {
                            ComputerStep();
                        }, 3000);
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
            selectedCard = computer.SelectCard();
            selectedTrick = computer.SelectTrick(selectedCard);
            HighlightTrick();
            RevealCard(selectedCard);
            CreateDelayedEvent(delegate(object sender, EventArgs e) {
                StepGame();
            }, 3000);
        }

		private void StepGame() {
			if (game.Current.Contains<Card>(selectedCard)) {
                game.PopulateActions();
                TakeAction();
                CompleteAction();
			}
		}

        private void TakeAction() {
            CardActions actions = game.Actions[selectedCard];
            if (actions.IsThrowable) {
                if (selectedTrick.Count == 0) {
                    if (!game.ThrowCard(selectedCard)) {
                        MessageBox.Show("The " + selectedCard + " can not be thrown", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                } else {
                    MessageBox.Show("The " + selectedCard + " must be thrown", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else {
                if (selectedTrick.Count == 0) {
                    if (actions.PossibleTricks.Count == 1) {
                        selectedTrick.AddRange(actions.PossibleTricks[0]);
                        HighlightTrick();
                    }
                }

                if (selectedTrick.Count > 0) {
                    bool scopa;
                    if (!game.TakeTrick(selectedCard, selectedTrick, out scopa)) {
                        MessageBox.Show("The " + selectedCard + " can not take " + Card.ToString(selectedTrick), "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#if DEBUG_TRICK
						} else {
							MessageBox.Show("Took trick " + Card.ToString(selectedList) + " with " + card);
#endif
                    }
                    if (scopa) {
                        MessageBox.Show("Scopa!!!", "Scopa", MessageBoxButtons.OK);
                    }
                } else {
                    MessageBox.Show("The " + selectedCard + " must take a trick", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void CompleteAction() {
            if (game.IsRoundOver) {
                game.TakeLastTrick();
                UpdateScores();
                ShowCards();

                if (!game.IsGameOver) {
                    CreateDelayedEvent(delegate(object sender, EventArgs e) {
                        CompleteRound();
                    }, 5000);
                } else {
                    List<Player> ranking = game.ScoreRound();
                    game.NewRound();
                    MessageBox.Show(ranking[0].Name + " Wins the game!", "Game Over", MessageBoxButtons.OK);
                }
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

			List<Player> ranking = game.ScoreRound();
			game.NewRound();
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
                CreateDelayedEvent(delegate(object sender, EventArgs e) {
                    ComputerStep();
                }, 3000);
            }
        }

        private void ShowCompleteRound_DebugMessage() {
			StringBuilder builder = new StringBuilder();
			foreach (Player player in game.Players) {
				builder.Append(player.Name).Append(" ha presa(").Append(player.TrickTracker.CardCount).Append("): ");
				List<Card> cardsTaken = player.TrickTracker.CardsTaken;
				cardsTaken.Sort();
				builder.Append(Card.ToString(cardsTaken)).Append(Environment.NewLine);
				builder.Append("Primiera: " + player.TrickTracker.PrimieraValue).Append(Environment.NewLine);
				builder.Append("Denari: " + player.TrickTracker.DenariCount).Append(Environment.NewLine);
				builder.Append("Settebello: " + player.TrickTracker.SetteBello).Append(Environment.NewLine);
				builder.Append("Scopa: " + player.TrickTracker.ScopaCount).Append(Environment.NewLine);
				builder.Append(Environment.NewLine);
			}
			MessageBox.Show(builder.ToString());
        }

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
