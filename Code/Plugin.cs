﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModCore
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[BepInProcess("ID2.exe")]
	public class Plugin : BaseUnityPlugin
	{
		internal static ManualLogSource Log { get; private set; }


		private void Awake()
		{
			Log = Logger;
			Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			// Applies all patches
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

			// Adds event listeners
			AddEventListeners();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Equals))
			{
				DebugMenuCommands.Instance.debugMenuManager.SetConsoleVisibility(true);
			}
		}

		private void OnApplicationQuit()
		{
			Events.GameQuit();
		}

		private void AddEventListeners()
		{
			SceneManager.sceneLoaded += Events.SceneLoad;
		}
	}
}