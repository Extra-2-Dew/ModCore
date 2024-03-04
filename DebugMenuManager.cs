using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using BepInEx;

namespace ModCore
{
    public class DebugMenuManager : MonoBehaviour
    {
        public AssetBundle debugMenuBundle;

        public static GameObject debugMenu;

        private InputField commandInput;
        private Text commandOutput;
        private GameObject commandListBG;
        private Text commandList;

        private void Awake()
        {
            // Load assets and assign values
            debugMenuBundle = AssetBundle.LoadFromFile(BepInEx.Utility.CombinePaths(Paths.PluginPath, PluginInfo.PLUGIN_NAME, "Assets", "debugmenu"));

            debugMenu = GameObject.Instantiate(debugMenuBundle.LoadAsset<GameObject>("DebugCanvas"));
            commandInput = debugMenu.GetComponentInChildren<InputField>();
            commandOutput = debugMenu.transform.Find("HistoryBG/Scroll View/Viewport/Content").GetComponent<Text>();
            commandListBG = debugMenu.transform.Find("CommandInput/CommandHelper").gameObject;
            commandList = debugMenu.transform.Find("CommandInput/CommandHelper/Commands").GetComponent<Text>();
            debugMenu.SetActive(false);

            commandInput.onEndEdit.AddListener(SubmitCommand);
            commandInput.onValueChanged.AddListener(SuggestCommand);

            DontDestroyOnLoad(debugMenu);
            debugMenu.SetActive(false);
            commandOutput.text = "";
            commandListBG.SetActive(false);
        }

        /// <summary>
        /// Shows/hides the console
        /// </summary>
        /// <param name="visible"></param>
        public void SetConsoleVisibility(bool visible)
        {
            debugMenu.SetActive(visible);
            if (visible)
            {
                commandInput.text = "";
                commandInput.Select();
            }
        }

        /// <summary>
        /// Submits text to the console output
        /// </summary>
        public void CommandOutput(string outputText)
        {
            commandOutput.text = outputText + "\n" + commandOutput.text;
        }

        private void SubmitCommand(string text)
        {
            if (text == "") return;
            DebugMenuCommands.Instance.ParseInput(text);
            commandInput.text = "";
            commandListBG.SetActive(false);
        }

        private void SuggestCommand(string text)
        {
            if (text == "") return;
            commandList.text = "";
            if (text.Split(' ').Length == 1)
            {
                List<string> suggestedCommands = new List<string>();
                foreach (var command in DebugMenuCommands.Instance.commands)
                {
                    if (command.Name.StartsWith(text)) suggestedCommands.Add(command.Name);
                    if (command.Aliases == null) continue;
                    foreach (string alias in command.Aliases)
                    {
                        if (alias.StartsWith(text)) suggestedCommands.Add(alias);
                    }
                }
                if (suggestedCommands.Count > 0)
                {
                    foreach (string command in suggestedCommands)
                    {
                        commandList.text += "\n" + command;
                    }
                    commandList.text = commandList.text.Trim('\n');
                    commandListBG.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 18 + (22 * suggestedCommands.Count));
                }
            }
            if (commandList.text.Length > 0) commandListBG.SetActive(true);
            else commandListBG.SetActive(false);
        }
    }
}
