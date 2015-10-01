﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// Test program for CMenu.
	/// </summary>
	class Program
	{
		static CMenu menu;

		static void Main (string[] args)
		{
			Console.WriteLine ("Simple CMenu demonstration");
			Console.WriteLine ("Enter \"help\" for help.");

			// Create menu
			menu = new CMenu ();

			// Add simple Hello World command
			menu.Add ("hello", s => Console.WriteLine ("Hello world!"));

			/*
			 * If the command happens to be more complex, you can just put it in a separate method.
			 */
			menu.Add ("len", s => PrintLen (s));

			/*
			 * It is also possible to return an exit code to signal that processing should be stopped.
			 * By default, the command "quit" exists for this purpose. Let's add an alternative way to stop processing input.
			 */
			menu.Add ("exit", s => MenuResult.Quit);

			/*
			 * To create a command with help text, simply add it.
			 */
			menu.Add ("time",
				s => Console.WriteLine (DateTime.UtcNow),
				"Writes the current time (UTC).");

			/*
			 * It is also possible to modify the input queue.
			 * Check out how the "repeat" command adds its argument to the input queue three times.
			 */
			menu.Add ("repeat",
				s => Repeat (s),
				"Repeats a command 3 times.");

			// Run menu. The menu will run until quit by the user.
			menu.Run ();

			Console.WriteLine ("Finished!");
		}

		static void PrintLen (string s)
		{
			Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
		}

		static void Repeat (string s)
		{
			menu.Buffer (s);
			menu.Buffer (s);
			menu.Buffer (s);
		}
	}
}
