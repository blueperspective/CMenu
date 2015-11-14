# CMenu
A simple console menu manager for C#

CMenu aims to simplify writing console menus. Instead of manually prompting the user for input and parsing it, you define commands in a short, structured and comprehensive way. You do not have to worry about all the annoying details and can do much more with less code.


## Advantages

* Simple, short syntax
  * Simple commands are created in a single line
  * Complex commands can be created or modified in steps or in their own, short class
* Powerful and extensible
  * Inner commands effortlessly parse parameters to commands
  * Nested menus allow fully or partially independent submenus
  * Own commands as well as subclassing existing commands are fully supported
* Low maintenance
  * Adaptive, automatically configures itself for optimal usage. For instance, command abbreviations are created and updated automatically when new commands (with potentially similar name) are added, or removed.
  * Very tolerant about its input, allowing partial matches and being (by default) case insensitive.
* Integrated tutorial and examples
  * Contains a library of example commands, including a conditional operator, a macro recorder and a procedural call structure
  * Comprehensive, documented code in case you want to dig into the details
* Result: An easy to use menu structure
  * Effortless command detection, including automatic abbreviations and partial matching
  * Intelligent corrections for mistyped commands
  * Integrated useful help which also works in command tree branches



## Creating and using basic commands

A single command is comprised of a keyword (selector), an optional help text describing it, and, most importantly, its behavior. The behavior can be defined as a simple lambda, and it is not unusual for a whole command to be defined in a single line.

	// Create menu
	var menu = new CMenu ();

	// Add simple Hello World command
	menu.Add ("hello", s => Console.WriteLine ("Hello world!"));

	// Run menu. The menu will run until quit by the user.
	menu.Run ();

While running, CMenu will continuously prompt the user for input, then feeds it to the respective command. Let's see how to use the "hello" command defined above:

	$ hello
	Hello world!

If the command happens to be more complex, you can just put it in a separate method.
	
	menu.Add ("len", s => PrintLen (s));

	static void PrintLen (string s)
	{
		Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
	}

	$ len 54321
	String "54321" has length 5

It is also possible to return an exit code to signal that processing should be stopped.
By default, the command "quit" exists for this purpose. Let's add an alternative way to stop processing input.

	menu.Add ("exit", s => MenuResult.Quit);

To create a command with help text, simply add it during definition.

	menu.Add ("time",
		s => Console.WriteLine (DateTime.UtcNow),
		"Help for \"time\": Writes the current time");

	$ time
	2015.10.01 17:54:38
	$ help time
	Help for "time": Writes the current time
	
You can also access individual commands to edit them later, though this is rarely required.

	menu["time"].HelpText += " (UTC).";
	
	$ help time
	Help for "time": Writes the current time (UTC).



## Command abbreviations and integrated help 

CMenu keeps an index of all available commands and lists them upon user request via typing "help". Moreover, it also automatically assigns abbreviations to all commands (if useful) and keeps them up-to-date when you later add new commands with similar keywords.

	$ help
	Available commands:
	e   | exit
	      hello
	      help
	l   | len
	q   | quit
	t   | time
	Type "help <command>" for individual command help.

The builtin command "help" also displays usage information of individual commands:

	$ help q
	quit
	Quits menu processing.
	$ help help
	help [command]
	Displays a help text for the specified command, or
	Displays a list of all available commands.

Commands can by entered abbreviated, as long as it is clear which one was intended. If it is not clear, then the possible options will be displayed.

	$ hel
	Command <hel> not unique. Candidates: hello, help
	$ hell
	Hello world!

Commands are case *in*sensitive by default. This can be changed using the `StringComparison` property.
When in case sensitive mode, CMenu will helpfully point out similar commands with different caseing.

	menu.StringComparison = StringComparison.InvariantCulture;
	menu.Add ("Hello", s => Console.WriteLine ("Hi!"));

	$ help
	Available commands:
	e   | exit
	H   | Hello
	      hello
	      help
	l   | len
	q   | quit
	r   | repeat
	t   | time
	Type "help <command>" for individual command help.
	$ H
	Hi!
	$ h
	Command <h> not unique. Candidates: hello, help
	$ hE
	Unknown command: hE
	Did you mean "hello", "Hello" or "help"?


	
## Advanced usage

There is documentation about [more complex use cases](doc/advanced.md).



## Example project

The source code contains an [example project](doc/example_project.md). It offers commands, which illustrate several (more or less advanced) use cases. It may be useful to reference them in your own projects.


	
## Project history and developer information

Another (much larger) project of mine uses a somewhat complex console menu, and I finally got annoyed enough by repeated, virtually equal code fragments to refactor and extract those pieces of code into a separate project. As I figured it was complete enough to be useful to others, I called it CMenu and put it on github.

In the future, there are certainly a lot of things to improve about CMenu, and I would definitely not call it complete or particularly well made. The changes and issues found on this project are mostly caused by improvements and requirement changes in said larger project. I update CMenu with the relevant changes in parallel when I find the time and will continue to do so for the foreseeable future.

I'm happy to hear if you found this useful, and open to suggestions to improve it.

