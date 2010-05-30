using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NIESoftware.Forms {
	partial class GameForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.playerAName = new System.Windows.Forms.Label();
            this.playerBName = new System.Windows.Forms.Label();
            this.playerATrickTracker = new System.Windows.Forms.Label();
            this.playerBTrickTracker = new System.Windows.Forms.Label();
            this.playerATrickTrackerCount = new System.Windows.Forms.Label();
            this.playerBTrickTrackerCount = new System.Windows.Forms.Label();
            this.playerBTurn = new System.Windows.Forms.PictureBox();
            this.playerATurn = new System.Windows.Forms.PictureBox();
            this.playerBDeck = new System.Windows.Forms.PictureBox();
            this.playerADeck = new System.Windows.Forms.PictureBox();
            this.playerAHand0 = new System.Windows.Forms.PictureBox();
            this.playerAHand1 = new System.Windows.Forms.PictureBox();
            this.playerAHand2 = new System.Windows.Forms.PictureBox();
            this.playerBHand0 = new System.Windows.Forms.PictureBox();
            this.playerBHand1 = new System.Windows.Forms.PictureBox();
            this.playerBHand2 = new System.Windows.Forms.PictureBox();
            this.table0 = new System.Windows.Forms.PictureBox();
            this.table1 = new System.Windows.Forms.PictureBox();
            this.table2 = new System.Windows.Forms.PictureBox();
            this.table3 = new System.Windows.Forms.PictureBox();
            this.table4 = new System.Windows.Forms.PictureBox();
            this.table5 = new System.Windows.Forms.PictureBox();
            this.table6 = new System.Windows.Forms.PictureBox();
            this.table7 = new System.Windows.Forms.PictureBox();
            this.playerAScore = new System.Windows.Forms.Label();
            this.playerBScore = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.playerBTurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerATurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBDeck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerADeck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerAHand0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerAHand1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerAHand2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBHand0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBHand1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBHand2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table7)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            // 
            // playerAName
            // 
            this.playerAName.AutoSize = true;
            this.playerAName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerAName.Location = new System.Drawing.Point(624, 39);
            this.playerAName.Name = "playerAName";
            this.playerAName.Size = new System.Drawing.Size(179, 37);
            this.playerAName.TabIndex = 16;
            this.playerAName.Text = "mmmmmm";
            // 
            // playerBName
            // 
            this.playerBName.AutoSize = true;
            this.playerBName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerBName.Location = new System.Drawing.Point(624, 400);
            this.playerBName.Name = "playerBName";
            this.playerBName.Size = new System.Drawing.Size(179, 37);
            this.playerBName.TabIndex = 17;
            this.playerBName.Text = "mmmmmm";
            // 
            // playerATrickTracker
            // 
            this.playerATrickTracker.AutoSize = true;
            this.playerATrickTracker.Location = new System.Drawing.Point(681, 79);
            this.playerATrickTracker.Name = "playerATrickTracker";
            this.playerATrickTracker.Size = new System.Drawing.Size(58, 65);
            this.playerATrickTracker.TabIndex = 20;
            this.playerATrickTracker.Text = "Carte:\r\nPremiera:\r\nDenari:\r\nScope:\r\nSette Bello";
            this.playerATrickTracker.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // playerBTrickTracker
            // 
            this.playerBTrickTracker.AutoSize = true;
            this.playerBTrickTracker.Location = new System.Drawing.Point(681, 440);
            this.playerBTrickTracker.Name = "playerBTrickTracker";
            this.playerBTrickTracker.Size = new System.Drawing.Size(58, 65);
            this.playerBTrickTracker.TabIndex = 21;
            this.playerBTrickTracker.Text = "Carte:\r\nPremiera:\r\nDenari:\r\nScope:\r\nSette Bello";
            this.playerBTrickTracker.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // playerATrickTrackerCount
            // 
            this.playerATrickTrackerCount.AutoSize = true;
            this.playerATrickTrackerCount.Location = new System.Drawing.Point(735, 79);
            this.playerATrickTrackerCount.Name = "playerATrickTrackerCount";
            this.playerATrickTrackerCount.Size = new System.Drawing.Size(19, 52);
            this.playerATrickTrackerCount.TabIndex = 22;
            this.playerATrickTrackerCount.Text = "40\r\n84\r\n10\r\n9";
            // 
            // playerBTrickTrackerCount
            // 
            this.playerBTrickTrackerCount.AutoSize = true;
            this.playerBTrickTrackerCount.Location = new System.Drawing.Point(735, 440);
            this.playerBTrickTrackerCount.Name = "playerBTrickTrackerCount";
            this.playerBTrickTrackerCount.Size = new System.Drawing.Size(19, 52);
            this.playerBTrickTrackerCount.TabIndex = 23;
            this.playerBTrickTrackerCount.Text = "40\r\n84\r\n10\r\n9";
            // 
            // playerBTurn
            // 
            this.playerBTurn.Location = new System.Drawing.Point(631, 440);
            this.playerBTurn.Name = "playerBTurn";
            this.playerBTurn.Size = new System.Drawing.Size(40, 40);
            this.playerBTurn.TabIndex = 19;
            this.playerBTurn.TabStop = false;
            this.playerBTurn.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerATurn
            // 
            this.playerATurn.Location = new System.Drawing.Point(629, 79);
            this.playerATurn.Name = "playerATurn";
            this.playerATurn.Size = new System.Drawing.Size(40, 40);
            this.playerATurn.TabIndex = 18;
            this.playerATurn.TabStop = false;
            this.playerATurn.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerBDeck
            // 
            this.playerBDeck.Location = new System.Drawing.Point(24, 369);
            this.playerBDeck.Name = "playerBDeck";
            this.playerBDeck.Size = new System.Drawing.Size(128, 177);
            this.playerBDeck.TabIndex = 15;
            this.playerBDeck.TabStop = false;
            this.playerBDeck.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerADeck
            // 
            this.playerADeck.Location = new System.Drawing.Point(24, 36);
            this.playerADeck.Name = "playerADeck";
            this.playerADeck.Size = new System.Drawing.Size(128, 177);
            this.playerADeck.TabIndex = 14;
            this.playerADeck.TabStop = false;
            this.playerADeck.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerAHand0
            // 
            this.playerAHand0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerAHand0.Image = global::NIESoftware.Properties.Resources.CardBack;
            this.playerAHand0.Location = new System.Drawing.Point(298, 39);
            this.playerAHand0.Name = "playerAHand0";
            this.playerAHand0.Size = new System.Drawing.Size(95, 144);
            this.playerAHand0.TabIndex = 0;
            this.playerAHand0.TabStop = false;
            this.playerAHand0.DoubleClick += new System.EventHandler(this.playerHand_DoubleClick);
            this.playerAHand0.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerAHand1
            // 
            this.playerAHand1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerAHand1.Image = global::NIESoftware.Properties.Resources.CardBack;
            this.playerAHand1.Location = new System.Drawing.Point(409, 39);
            this.playerAHand1.Name = "playerAHand1";
            this.playerAHand1.Size = new System.Drawing.Size(95, 144);
            this.playerAHand1.TabIndex = 1;
            this.playerAHand1.TabStop = false;
            this.playerAHand1.DoubleClick += new System.EventHandler(this.playerHand_DoubleClick);
            this.playerAHand1.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerAHand2
            // 
            this.playerAHand2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerAHand2.Image = global::NIESoftware.Properties.Resources.CardBack;
            this.playerAHand2.Location = new System.Drawing.Point(519, 39);
            this.playerAHand2.Name = "playerAHand2";
            this.playerAHand2.Size = new System.Drawing.Size(95, 144);
            this.playerAHand2.TabIndex = 2;
            this.playerAHand2.TabStop = false;
            this.playerAHand2.DoubleClick += new System.EventHandler(this.playerHand_DoubleClick);
            this.playerAHand2.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerBHand0
            // 
            this.playerBHand0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerBHand0.Image = global::NIESoftware.Properties.Resources.CardBack;
            this.playerBHand0.Location = new System.Drawing.Point(298, 400);
            this.playerBHand0.Name = "playerBHand0";
            this.playerBHand0.Size = new System.Drawing.Size(95, 144);
            this.playerBHand0.TabIndex = 3;
            this.playerBHand0.TabStop = false;
            this.playerBHand0.DoubleClick += new System.EventHandler(this.playerHand_DoubleClick);
            this.playerBHand0.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerBHand1
            // 
            this.playerBHand1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerBHand1.Image = global::NIESoftware.Properties.Resources.CardBack;
            this.playerBHand1.Location = new System.Drawing.Point(409, 400);
            this.playerBHand1.Name = "playerBHand1";
            this.playerBHand1.Size = new System.Drawing.Size(95, 144);
            this.playerBHand1.TabIndex = 4;
            this.playerBHand1.TabStop = false;
            this.playerBHand1.DoubleClick += new System.EventHandler(this.playerHand_DoubleClick);
            this.playerBHand1.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerBHand2
            // 
            this.playerBHand2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playerBHand2.Image = global::NIESoftware.Properties.Resources.CardBack;
            this.playerBHand2.Location = new System.Drawing.Point(519, 400);
            this.playerBHand2.Name = "playerBHand2";
            this.playerBHand2.Size = new System.Drawing.Size(95, 144);
            this.playerBHand2.TabIndex = 5;
            this.playerBHand2.TabStop = false;
            this.playerBHand2.DoubleClick += new System.EventHandler(this.playerHand_DoubleClick);
            this.playerBHand2.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table0
            // 
            this.table0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table0.Location = new System.Drawing.Point(24, 219);
            this.table0.Name = "table0";
            this.table0.Size = new System.Drawing.Size(95, 144);
            this.table0.TabIndex = 6;
            this.table0.TabStop = false;
            this.table0.Click += new System.EventHandler(this.table_Click);
            this.table0.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table1
            // 
            this.table1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table1.Location = new System.Drawing.Point(134, 219);
            this.table1.Name = "table1";
            this.table1.Size = new System.Drawing.Size(95, 144);
            this.table1.TabIndex = 7;
            this.table1.TabStop = false;
            this.table1.Click += new System.EventHandler(this.table_Click);
            this.table1.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table2
            // 
            this.table2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table2.Location = new System.Drawing.Point(244, 219);
            this.table2.Name = "table2";
            this.table2.Size = new System.Drawing.Size(95, 144);
            this.table2.TabIndex = 8;
            this.table2.TabStop = false;
            this.table2.Click += new System.EventHandler(this.table_Click);
            this.table2.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table3
            // 
            this.table3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table3.Location = new System.Drawing.Point(354, 219);
            this.table3.Name = "table3";
            this.table3.Size = new System.Drawing.Size(95, 144);
            this.table3.TabIndex = 9;
            this.table3.TabStop = false;
            this.table3.Click += new System.EventHandler(this.table_Click);
            this.table3.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table4
            // 
            this.table4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table4.Location = new System.Drawing.Point(464, 219);
            this.table4.Name = "table4";
            this.table4.Size = new System.Drawing.Size(95, 144);
            this.table4.TabIndex = 10;
            this.table4.TabStop = false;
            this.table4.Click += new System.EventHandler(this.table_Click);
            this.table4.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table5
            // 
            this.table5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table5.Location = new System.Drawing.Point(574, 219);
            this.table5.Name = "table5";
            this.table5.Size = new System.Drawing.Size(95, 144);
            this.table5.TabIndex = 11;
            this.table5.TabStop = false;
            this.table5.Click += new System.EventHandler(this.table_Click);
            this.table5.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table6
            // 
            this.table6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table6.Location = new System.Drawing.Point(684, 219);
            this.table6.Name = "table6";
            this.table6.Size = new System.Drawing.Size(95, 144);
            this.table6.TabIndex = 12;
            this.table6.TabStop = false;
            this.table6.Click += new System.EventHandler(this.table_Click);
            this.table6.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // table7
            // 
            this.table7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.table7.Location = new System.Drawing.Point(794, 219);
            this.table7.Name = "table7";
            this.table7.Size = new System.Drawing.Size(95, 144);
            this.table7.TabIndex = 13;
            this.table7.TabStop = false;
            this.table7.Click += new System.EventHandler(this.table_Click);
            this.table7.MouseHover += new System.EventHandler(this.image_MouseHover);
            // 
            // playerAScore
            // 
            this.playerAScore.AutoSize = true;
            this.playerAScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerAScore.Location = new System.Drawing.Point(795, 39);
            this.playerAScore.Name = "playerAScore";
            this.playerAScore.Size = new System.Drawing.Size(117, 37);
            this.playerAScore.TabIndex = 24;
            this.playerAScore.Text = "11 (+4)";
            // 
            // playerBScore
            // 
            this.playerBScore.AutoSize = true;
            this.playerBScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerBScore.Location = new System.Drawing.Point(795, 400);
            this.playerBScore.Name = "playerBScore";
            this.playerBScore.Size = new System.Drawing.Size(117, 37);
            this.playerBScore.TabIndex = 25;
            this.playerBScore.Text = "11 (+4)";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(936, 24);
            this.menuStrip.TabIndex = 26;
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.newGameToolStripMenuItem.Text = "&New Game";
            this.newGameToolStripMenuItem.ToolTipText = "Start a new game (the current game will be reset)";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.ToolTipText = "Quit this application and return to Windows";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.ClientSize = new System.Drawing.Size(936, 567);
            this.Controls.Add(this.playerBScore);
            this.Controls.Add(this.playerAScore);
            this.Controls.Add(this.playerBTrickTrackerCount);
            this.Controls.Add(this.playerATrickTrackerCount);
            this.Controls.Add(this.playerBTrickTracker);
            this.Controls.Add(this.playerATrickTracker);
            this.Controls.Add(this.playerBTurn);
            this.Controls.Add(this.playerATurn);
            this.Controls.Add(this.playerBName);
            this.Controls.Add(this.playerAName);
            this.Controls.Add(this.playerBDeck);
            this.Controls.Add(this.playerADeck);
            this.Controls.Add(this.playerAHand0);
            this.Controls.Add(this.playerAHand1);
            this.Controls.Add(this.playerAHand2);
            this.Controls.Add(this.playerBHand0);
            this.Controls.Add(this.playerBHand1);
            this.Controls.Add(this.playerBHand2);
            this.Controls.Add(this.table0);
            this.Controls.Add(this.table1);
            this.Controls.Add(this.table2);
            this.Controls.Add(this.table3);
            this.Controls.Add(this.table4);
            this.Controls.Add(this.table5);
            this.Controls.Add(this.table6);
            this.Controls.Add(this.table7);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WinScopa";
            ((System.ComponentModel.ISupportInitialize)(this.playerBTurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerATurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBDeck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerADeck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerAHand0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerAHand1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerAHand2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBHand0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBHand1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBHand2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table7)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private PictureBox playerAHand0;
		private PictureBox playerAHand1;
		private PictureBox playerAHand2;
		private PictureBox playerBHand0;
		private PictureBox playerBHand1;
		private PictureBox playerBHand2;
		private PictureBox table0;
		private PictureBox table1;
		private PictureBox table2;
		private PictureBox table3;
		private PictureBox table4;
		private PictureBox table5;
		private PictureBox table6;
		private PictureBox table7;
		private ToolTip toolTip;
		private PictureBox playerADeck;
		private PictureBox playerBDeck;
		private Label playerAName;
		private Label playerBName;
		private PictureBox playerATurn;
		private PictureBox playerBTurn;
		private Label playerATrickTracker;
		private Label playerBTrickTracker;
		private Label playerATrickTrackerCount;
		private Label playerBTrickTrackerCount;
		private Label playerAScore;
		private Label playerBScore;
        private MenuStrip menuStrip;
        private ToolStripMenuItem gameToolStripMenuItem;
        private ToolStripMenuItem newGameToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;

	}
}