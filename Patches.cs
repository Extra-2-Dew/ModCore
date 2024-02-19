using HarmonyLib;
using UnityEngine;

namespace ModCore
{
	[HarmonyPatch]
	public class Patches
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ObjectUpdater.UpdateLayer), "SetPause")]
		// Invokes an event hook
		public static void ObjectUpdaterUpdateLayer_SetPause_Patch(bool pause)
		{
			Events.PauseChange(pause);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntitySpawner), "DoSpawn")]
		// Invokes an event hook
		public static void EntitySpawner_DoSpawn_Patch(ref Entity __result)
		{
			Events.EntitySpawn(__result);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntityEventsOwner), "SendDetailedDeath")]
		// Invokes an event hook
		public static void EntityEventsOwner_SendDetailedDeath_Patch(Entity ent, Killable.DetailedDeathData data)
		{
			Events.EntityDied(ent, data);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntityEventsOwner), "SendRoomChangeDone")]
		// Invokes an event hook
		public static void EntityEventsOwner_SendRoomChangeDone_Patch(Entity ent, LevelRoom to, LevelRoom from, EntityEventsOwner.RoomEventData data)
		{
			Events.RoomChange(ent, to, from, data);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(EntityEventsOwner), "SendItemGet")]
		// Invokes an event hook
		public static void EntityEventsOwner_SendItemGet_Patch(Entity ent, Item item)
		{
			Events.ItemGet(ent, item);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PauseMenu), "Update")]
		// Allows pressing a key to open debug menu
		public static void PausewMenu_Update_Patch(ref PauseMenu __instance)
		{
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				__instance.menuImpl.SwitchToScreen("debugRoot", null);

				if (DebugMenuCommands.Instance == null)
					new DebugMenuCommands().Initialize(__instance._debugMenu);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(DebugMenu), "ParseResultString")]
		// Extends DebugMenu.ParseResultString() to check for custom commands
		public static void DebugMenu_ParseResultString_Patch(string str)
		{
			DebugMenuCommands.Instance.ParseInput(str);
		}
	}
}