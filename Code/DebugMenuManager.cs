using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ModCore
{
	public class DebugMenuManager : MonoBehaviour
	{
		public static KeyCode DebugMenuKey { get; } = KeyCode.BackQuote;
		public Action OnDebugMenuInitialized;
		public bool IsVisible { get; private set; }

		private static DebugMenuManager instance;
		private CommandHandler commandHandler;
		private InputField commandInput;
		private Text commandOutput;
		private GameObject commandListBG;
		private Text commandList;
		private Button closeButton;
		private Animator animator;
		private ObjectUpdater.PauseTag pauseTag;
		private RectTransform commandListBGRect;
		private bool hasInited;

		public static DebugMenuManager Instance
		{
			get
			{
				if (instance == null)
					instance = Init();

				return instance;
			}
		}
		public CommandHandler CommHandler { get { return commandHandler; } }

		// Creates a static GameObject and adds self to it as a component
		private static DebugMenuManager Init()
		{
			GameObject debugMenuObj = new("Debug Menu Manager");
			DebugMenuManager debugMenuManager = debugMenuObj.AddComponent<DebugMenuManager>();
			DontDestroyOnLoad(debugMenuObj);
			return debugMenuManager;
		}

		private void Awake()
		{
			// Create CommandHandler
			commandHandler = new();

			// Load DebugMenu from AssetBundle
			AssetBundle bundle = AssetBundle.LoadFromFile(BepInEx.Utility.CombinePaths(Paths.PluginPath, PluginInfo.PLUGIN_NAME, "Assets", "menus"));

			// Instantiate DebugMenu object
			GameObject menuObj = Instantiate(bundle.LoadAsset<GameObject>("DebugScreen"));

			// Get references
			commandInput = menuObj.GetComponentInChildren<InputField>();
			commandOutput = menuObj.transform.Find("DebugMenu/HistoryBG/Scroll View/Viewport/Content").GetComponent<Text>();
			commandListBG = menuObj.transform.Find("DebugMenu/CommandInput/CommandHelper").gameObject;
			commandListBGRect = commandListBG.GetComponent<RectTransform>();
			commandList = menuObj.transform.Find("DebugMenu/CommandInput/CommandHelper/Commands").GetComponent<Text>();
			closeButton = menuObj.transform.Find("DebugMenu/CloseButton").GetComponent<Button>();
			animator = menuObj.transform.GetChild(0).GetComponent<Animator>();

			// Add listeners
			closeButton.onClick.AddListener(() => { ToggleMenuVisibility(); });
			commandInput.onEndEdit.AddListener(ParseInput);
			commandInput.onValueChanged.AddListener(SuggestCommand);

			// Set some default values
			commandInput.interactable = false;
			closeButton.interactable = false;
			commandOutput.text = "";
			commandListBG.SetActive(false);

			// Make DebugMenu persistent
			DontDestroyOnLoad(menuObj);
		}

		private void Update()
		{
			// Close menu if open
			if (IsVisible && Input.GetKeyDown(KeyCode.Escape))
				ToggleMenuVisibility();
		}

		/// <summary>
		/// Shows/hides the Debug Menu
		/// </summary>
		public void ToggleMenuVisibility()
		{
			if (!hasInited)
			{
				OnDebugMenuInitialized?.Invoke();
				hasInited = true;
			}

			animator.SetInteger("State", IsVisible ? 0 : 1);
			commandInput.interactable = !IsVisible;
			closeButton.interactable = !IsVisible;
			commandInput.text = "";
			MainMenu mainMenu = SceneManager.GetActiveScene().name == "MainMenu" ? GameObject.Find("GuiFuncs").GetComponent<MainMenu>() : null;
			GuiInteractionLayer mainMenuGuiLayer = mainMenu?.menuImpl.currScreen.Root._interactionLayer;

			// Unpause
			if (IsVisible)
			{
				pauseTag.Release();
				pauseTag = null;

				// Allow interaction with MainMenu UI
				if (mainMenuGuiLayer != null)
					mainMenuGuiLayer.IsActive = true;
			}
			// Pause
			else
			{
				pauseTag = ObjectUpdater.Instance.RequestPause();
				commandInput.ActivateInputField();

				// Disallow interaction with MainMenu UI
				if (mainMenuGuiLayer != null)
					mainMenuGuiLayer.IsActive = false;
			}

			IsVisible = !IsVisible;
		}

		/// <summary>
		/// Updates the output of the debug menu
		/// </summary>
		/// <param name="text">The text to update it to</param>
		public void UpdateOutput(string text)
		{
			commandOutput.text = text + "\n" + commandOutput.text;
		}

		/// <summary>
		/// Parses the user input to get command & args from it
		/// </summary>
		/// <param name="input">The user input</param>
		private void ParseInput(string input)
		{
			if (string.IsNullOrEmpty(input))
				return;

			commandInput.text = "";
			commandListBG.SetActive(false);
			string[] splitInput = input.ToLower().Split(' ');
			string commandName = splitInput[0];
			CommandHandler.CommandInfo command = commandHandler.Commands.Find(x => x.Name == commandName || (x.Aliases != null && x.Aliases.Contains(commandName)));

			// If command is valid
			if (command != null)
			{
				string[] args = splitInput.Skip(1).ToArray();
				command.Callback.Invoke(args);
				Plugin.Log.LogInfo($"Ran command {commandName} with {args.Length} args!");
			}
		}

		private void SuggestCommand(string text)
		{
			if (string.IsNullOrEmpty(text))
				return;

			commandList.text = "";

			if (text.Split(' ').Length == 1)
			{
				List<string> suggestedCommands = new();

				foreach (CommandHandler.CommandInfo command in commandHandler.Commands)
				{
					if (command.Name.StartsWith(text))
						suggestedCommands.Add(command.Name);
					else if (command.Aliases != null)
					{
						foreach (string alias in command.Aliases)
						{
							if (alias.StartsWith(text))
								suggestedCommands.Add(alias);
						}
					}
				}

				if (suggestedCommands.Count > 0)
				{
					foreach (string command in suggestedCommands)
						commandList.text += "\n" + command;

					commandList.text = commandList.text.Trim('\n');
					commandListBGRect.sizeDelta = new Vector2(400, 18 + (22 * suggestedCommands.Count));
				}
			}

			commandListBG.SetActive(!string.IsNullOrEmpty(commandList.text));
		}

		public class CommandHandler
		{
			private List<CommandInfo> commands;

			public List<CommandInfo> Commands { get { return commands; } }

			public delegate void CommandFunc(string[] args);

			public CommandHandler()
			{
				commands = new()
				{
					new("help", HelpCommand)
				};
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


			// Methods for default commands go below here

			private void HelpCommand(string[] args)
			{
				string output = "";

				foreach (CommandInfo command in commands)
					output += command.Name + "\n";

				Instance.UpdateOutput(output);
			}

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
}