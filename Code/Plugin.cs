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
		public static SaverOwner MainSaver { get; internal set; }
		internal static ManualLogSource Log { get; private set; }

		private void Awake()
		{
			Log = Logger;
			Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			// Applies all patches
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
			Patches.MenuImplPatches(new("MenuImplPatcher"));

			// Adds event listeners
			AddEventListeners();

			// Mute splash sound effect and the start of MainMenu song
			MuteAudio();
		}

		private void Update()
		{
			if (Input.GetKeyDown(DebugMenuManager.DebugMenuKey))
			{
				if (!ObjectUpdater.Instance.IsPaused() || (DebugMenuManager.Instance.IsVisible))
					DebugMenuManager.Instance.ToggleMenuVisibility();
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

		private void MuteAudio()
		{
			SoundPlayer.Instance.SoundVolume = 0;
			MusicPlayer.Instance.MusicVolume = 0;
		}
	}
}