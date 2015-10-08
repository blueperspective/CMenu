﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu.DefaultItems
{
	public class MI_Quit : CMenuItem
	{
		public MI_Quit ()
			: base ("quit")
		{
			HelpText = ""
				+ "quit\n"
				+ "Quits menu processing.";
		}

		public override MenuResult Execute (string arg)
		{
			return MenuResult.Quit;
		}
	}
}
