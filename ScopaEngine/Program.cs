using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NIESoftware {

	using Forms;

	class Program {

		[STAThread]
		static void Main(string[] args) {
#if CONSOLE
			// new ConsoleScopa().Run();
#else
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			GameForm form = new GameForm();
			Application.Run(form);
#endif
		}

	}

}
