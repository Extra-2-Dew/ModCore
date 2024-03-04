using System;
using System.Collections.Generic;
using System.Linq;

namespace ModCore
{
	public class DebugMenuCommands
	{
		public static Action OnDebugMenuInitialized;
		private static DebugMenuCommands instance;
		private DebugMenu menu;
		private List<CommandInfo> commands;

		public static DebugMenuCommands Instance { get { return instance; } }
		public DebugMenu Menu { get { return menu; } }
		public SaverOwner Saver { get { return menu._saver; } }
		public bool HasInitialized { get; private set; }

		/// <summary>
		/// Initializes core debug menu commands
		/// </summary>
		/// <param name="menu">The DebugMenu reference</param>
		public void Initialize(DebugMenu menu)
		{
			instance = this;
			this.menu = menu;
			commands = new List<CommandInfo>
			{
				new("help", HelpCommand)
			};

			OnDebugMenuInitialized?.Invoke();
			HasInitialized = true;
		}

		/// <summary>
		/// Parses the user input to get command & args from it
		/// </summary>
		/// <param name="input">The user input</param>
		public void ParseInput(string input)
		{
			string[] splitInput = input.ToLower().Split(' ');
			string commandName = splitInput[0];
			CommandInfo command = commands.Find(x => x.Name == commandName || (x.Aliases != null && x.Aliases.Contains(commandName)));

			// If command is valid
			if (command != null)
			{
				string[] args = splitInput.Skip(1).ToArray();
				command.Callback.Invoke(args);
				Plugin.Log.LogInfo($"Ran command {commandName} with {args.Length} args!");
			}
		}

		/// <summary>
		/// Adds the command to list of commands
		/// </summary>
		/// <param name="commandName">The name of the command</param>
		/// <param name="commandFunc">The CommandFunc function to invoke when command is executed</param>
		public void AddCommand(string commandName, CommandFunc commandFunc, string[] aliases = null)
		{
			CommandInfo newCommand = new(commandName, commandFunc, aliases);
			commands.Add(newCommand);
			Plugin.Log.LogInfo("Added command " + commandName + "!");
		}

		/// <summary>
		/// Updates the output of the debug menu
		/// </summary>
		/// <param name="text">The text to update it to</param>
		public void UpdateOutput(string text)
		{
			menu.UpdateInfo(text);
		}


		// Outputs the list of available commands
		private void HelpCommand(string[] args)
		{
			string output = "";

			foreach (CommandInfo command in commands)
			{
				output += command.Name + "\n";
			}

			UpdateOutput(output);
		}
		public delegate void CommandFunc(string[] args);

		public class CommandInfo
		{
			public string Name { get; }
			public string[] Aliases { get; }
			public CommandFunc Callback { get; }

			public CommandInfo(string name, CommandFunc callback, string[] aliases = null)
			{
				Name = name;
				Aliases = aliases;
				Callback = callback;
			}
		}
	}
}