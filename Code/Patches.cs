using HarmonyLib;
using System;

namespace ModCore
{
	[HarmonyPatch]
	public class Patches
	{
		[HarmonyPostfix, HarmonyPatch(typeof(PlayerRespawner), nameof(PlayerRespawner.DoRespawn))]
		public static void PlayerRespawner_DoRespawn_Patch()
		{
			Events.PlayerRespawn();
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ObjectUpdater.UpdateLayer), nameof(ObjectUpdater.UpdateLayer.SetPause))]
		public static void ObjectUpdaterUpdateLayer_SetPause_Patch(bool pause)
		{
			Events.PauseChange(pause);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntitySpawner), nameof(EntitySpawner.DoSpawn))]
		public static void EntitySpawner_DoSpawn_Patch(ref Entity __result)
		{
			Events.EntitySpawn(__result);
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(Killable), nameof(Killable.ForceDeath))]
		// Prevents pause warps from sending death event for player
		public static void Killable_ForceDeath(Killable.DeathData deathData)
		{
			if (deathData.silentDeath)
				deathData.deathTag = "warp";
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntityEventsOwner), nameof(EntityEventsOwner.SendDetailedDeath))]
		public static void EntityEventsOwner_SendDetailedDeath_Patch(Entity ent, Killable.DetailedDeathData data)
		{
			Events.EntityDied(ent, data);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntityEventsOwner), nameof(EntityEventsOwner.SendRoomChangeDone))]
		public static void EntityEventsOwner_SendRoomChangeDone_Patch(Entity ent, LevelRoom to, LevelRoom from, EntityEventsOwner.RoomEventData data)
		{
			Events.RoomChange(ent, from, to, data);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntityEventsOwner), nameof(EntityEventsOwner.SendItemGet))]
		public static void EntityEventsOwner_SendItemGet_Patch(Entity ent, Item item)
		{
			Events.ItemGet(ent, item);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SaverOwner), nameof(SaverOwner.LoadLocalFromFile))]
		// Stores reference to MainSaver
		public static void SaverOwner_LoadLocalFromFile_Patch(SaverOwner __instance)
		{
			Plugin.MainSaver = __instance;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(MainMenu.MainScreen), nameof(MainMenu.MainScreen.DoShow))]
		// Prevent hitting back button on MainMenu from quitting game, and auto-skip the menu animation
		public static void MainMenu_MainScreen_DoShow_Patch(MainMenu.MainScreen __instance)
		{
			// Prevent back from quitting
			UnityEngine.Object.Destroy(Utility.FindNestedChild("Main", "Quit").GetComponent<GuiCancelObjectTag>());

			// Skip first time animation
			__instance.Root.firstTime = false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.StartGame))]
		public static void MainMenu_StartGame_Patch(MainMenu __instance)
		{
			Events.FileStart(__instance._saver.GetSaver("/local/start", true) == null);
		}

		#region SwitchToScreen
		public static void MenuImplPatches(Harmony harmonyInstance)
		{
			var targetMainMenuMethod = AccessTools.Method(typeof(MenuImpl<MainMenu>), nameof(MenuImpl<MainMenu>.SwitchToScreen), new Type[] { typeof(MenuScreen<MainMenu>), typeof(object) });
			var switchScreenPrefix = new HarmonyMethod(typeof(Patches).GetMethod(nameof(Patches.SendChangeScreenEvent)));
			harmonyInstance.Patch(targetMainMenuMethod, prefix: switchScreenPrefix);
		}

		public static void SendChangeScreenEvent(MenuScreen<MainMenu> screen, object args)
		{
			Plugin.Log.LogInfo($"Switching to screen {screen.Name}");
			Events.ChangeScreen(screen.Name, args);
			// For some reason I don't need to patch the other Menu types, this works for all of them
			// However, I can't actually get the type used in the generic, so all I can do is return the name of the screen
		}
		#endregion
	}
}