﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;
using ExampleMenu.Procedures;
using ExampleMenu.Recording;

namespace ExampleMenu
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

			var mainmenu = new CMenu ();
			mainmenu.PromptCharacter = "main>";
			mainmenu.Add ("tutorial", s => Tutorial ());
			mainmenu.Add ("tree-init", s => TreeInitialization ());
			mainmenu.Add ("examples", s => Examples ());

			IO.ImmediateInput ("help");
			mainmenu.Run ();
		}

		static void Tutorial ()
		{
			Basics ();
			CaseSensitivity ();
			InputModification ();
			InnerCommands ();
			NestedCommands ();
			InnerWithShared ();
			IO.ImmediateInput ("help");
		}

		static void Basics ()
		{
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
			menu.Add ("exit", s => menu.Quit ());

			/*
			 * To create a command with help text, simply add it during definition.
			 */
			menu.Add ("time",
				s => Console.WriteLine (DateTime.UtcNow),
				"Help for \"time\": Writes the current time");

			/*
			 * You can also access individual commands to edit them later, though this is rarely required.
			 */
			menu["time"].HelpText += " (UTC).";

			// Run menu. The menu will run until quit by the user.
			Console.WriteLine ("Enter \"help\" for help.");
			Console.WriteLine ("Enter \"quit\" to quit (in this case, the next step of this demo will be started).");
			menu.Run ();

			Console.WriteLine ("(First menu example completed, starting the next one...)");
		}

		static void PrintLen (string s)
		{
			Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
		}

		static void CaseSensitivity ()
		{
			/*
			 * Commands are case *in*sensitive by default. This can be changed using the `StringComparison` property.
			 */
			menu.StringComparison = StringComparison.InvariantCulture;
			menu.Add ("Hello", s => Console.WriteLine ("Hi!"));

			Console.WriteLine ("The menu is now case sensitive.");
			menu.Run ();
		}

		static void InputModification ()
		{
			/*
			 * It is also possible to modify the input queue.
			 * Check out how the "repeat" command adds its argument to the input queue two times.
			 */
			menu.Add ("repeat",
				s => {
					IO.ImmediateInput (s);
					IO.ImmediateInput (s);
				},
				"Repeats a command two times.");

			Console.WriteLine ("New command available: repeat");
			menu.Run ();
		}

		static void InnerCommands ()
		{
			var mi = menu.Add ("convert", "convert upper|lower [text]\nConverts the text to upper or lower case");
			mi.Add ("upper", s => Console.WriteLine (s.ToUpperInvariant ()), "Converts to upper case");
			mi.Add ("lower", s => Console.WriteLine (s.ToLowerInvariant ()), "Converts to lower case");

			Console.WriteLine ("New command <convert> available. It features the inner commands \"upper\" and \"lower\".");
			menu.Run ();
		}

		static void NestedCommands ()
		{
			menu.Add (new MI_Add ());

			Console.WriteLine ("New command <add> available.");
			IO.ImmediateInput ("help add");
			menu.Run ();
		}

		class SharedViaOverride : CMenuItem
		{
			public SharedViaOverride ()
				: base ("shared-override")
			{
				Add ("1", s => Console.WriteLine ("First child"));
				Add ("2", s => Console.WriteLine ("Second child"));
			}

			public override void Execute (string arg)
			{
				Console.WriteLine ("This code is shared between all children of this menu item.");
				if (DateTime.UtcNow.Millisecond < 500) {
					base.Execute (arg);
				}
			}
		}

		static void InnerWithShared ()
		{
			/*
			 * If your inner menu items should share code (e.g. common basic validation), there are two ways to
			 * implement this.
			 *
			 * First option: Override Execute in their parent menu item so it first executes the shared code,
			 * then resumes normal processing.
			 */
			menu.Add (new SharedViaOverride ());

			/*
			 * Second option: Use the return values of Execute to indicate if processing should continue with
			 * the children, or return immediately. Returning is the default.
			 */
			var msr = menu.Add ("shared-result", s => {
				Console.WriteLine ("This code is shared between all children of this menu item.");
				if (DateTime.UtcNow.Millisecond < 500) {
					return MenuResult.Proceed;
				}
				else {
					return MenuResult.Return;
				}
			});
			msr.Add ("1", s => Console.WriteLine ("First child"));
			msr.Add ("2", s => Console.WriteLine ("Second child"));

			/*
			 * Which option you chose is up to you. MenuResults have the advantage of compactness and do not
			 * require a deriving from CMenuItem. For larger commands, it may be preferable to use a separate
			 * class. Note that you are still free to use MenuResult values within an overridden Execute.
			 */

			Console.WriteLine ("New commands <shared-override> and <shared-result> available.");
			menu.Run ();
		}

		static void TreeInitialization ()
		{
			/*
			 * It may be useful to create complex menu trees using collection initializers
			 */
			var m = new CMenu () {
				new CMenuItem ("1") {
					new CMenuItem ("1", s => Console.WriteLine ("1-1")),
					new CMenuItem ("2", s => Console.WriteLine ("1-2")),
				},
				new CMenuItem ("2") {
					new CMenuItem ("1", s => Console.WriteLine ("2-1")),
					new CMenuItem ("2", s => Console.WriteLine ("2-2")),
				},
			};
			m.PromptCharacter = "tree>";
			m.Run ();

			/*
			 * You can also combine object and collection initializers
			 */
			m = new CMenu () {
				PromptCharacter = "combined>",
				MenuItem = {
					new CMenuItem ("1", s => Console.WriteLine ("1")),
					new CMenuItem ("2", s => Console.WriteLine ("2")),
				}
			};
			m.Run ();
		}

		static void Examples ()
		{
			var m = new CMenu ();

			m.Add (new MI_Add ());

			m.Add (new MI_Echo ());
			m.Add (new MI_If ());
			m.Add (new MI_Pause ());

			var frs = new FileRecordStore ();
			m.Add (new MI_Record (frs));
			m.Add (new MI_Replay (m, frs));

			var procmgr = new ProcManager ();
			m.Add (new MI_Proc (procmgr));
			m.Add (new MI_Call (m, procmgr));
			m.Add (new MI_Return (m, procmgr));
			m.Add (new MI_Goto (procmgr));

			IO.ImmediateInput ("help");
			m.Run ();
		}
	}
}
