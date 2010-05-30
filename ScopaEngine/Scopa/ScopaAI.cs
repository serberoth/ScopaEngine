using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIESoftware.Scopa {

	class ScopaAI {
		private ScopaGame game;
		private Player player;

		ScopaAI(ScopaGame game, Player player) {
			this.game = game;
			this.player = player;
		}

		public Card SelectCard () {
			return player.Hand[0];
		}

	}

}
