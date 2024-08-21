using UnityEngine;
using UnityEngine.SceneManagement;

public static class Events
{
	// Player
	public static event PlayerSpawnFunc OnPlayerSpawn;

	// Entity
	public static event EntityFunc OnEntitySpawn;
	public static event EntityDeathFunc OnEntityDied;
	public static event RoomChangeFunc OnRoomChanged;
	public static event ItemGetFunc OnItemGet;

	// Game
	public static event PauseFunc OnPauseChange;
	public static event SceneFunc OnSceneLoaded;
	public static event Func OnGameQuit;
	public static event FileFunc OnFileStart;
	public static event ChangeScreenFunc OnChangeScreen;

	// Delegates
	public delegate void Func();
	public delegate void PlayerSpawnFunc(Entity player, GameObject camera, PlayerController controller);
	public delegate void FileFunc(bool newFile);
	public delegate void PauseFunc(bool paused);
	public delegate void EntityFunc(Entity entity);
	public delegate void EntityDeathFunc(Entity entity, Killable.DetailedDeathData data);
	public delegate void RoomChangeFunc(Entity entity, LevelRoom toRoom, LevelRoom fromRoom, EntityEventsOwner.RoomEventData data);
	public delegate void ItemGetFunc(Entity entity, Item item);
	public delegate void SceneFunc(Scene scene, LoadSceneMode mode);
	public delegate void ChangeScreenFunc(string toScreen, object args = null);

	public static void PlayerSpawn(Entity player, GameObject camera, PlayerController controller)
	{
		OnPlayerSpawn?.Invoke(player, camera, controller);
	}

	public static void EntitySpawn(Entity entity)
	{
		OnEntitySpawn?.Invoke(entity);
	}

	public static void EntityDied(Entity entity, Killable.DetailedDeathData data)
	{
		if (data.hp <= 0)
			OnEntityDied?.Invoke(entity, data);
	}

	public static void RoomChange(Entity entity, LevelRoom toRoom, LevelRoom fromRoom, EntityEventsOwner.RoomEventData data)
	{
		if (toRoom == null)
			toRoom = LevelRoom.GetRoomForPosition(ModCore.Utility.GetPlayer().transform.position);

		OnRoomChanged?.Invoke(entity, toRoom, fromRoom, data);
	}

	public static void ItemGet(Entity entity, Item item)
	{
		OnItemGet?.Invoke(entity, item);
	}

	public static void PauseChange(bool paused)
	{
		OnPauseChange?.Invoke(paused);
	}

	public static void SceneLoad(Scene scene, LoadSceneMode mode)
	{
		PlayerSpawner.RegisterSpawnListener(PlayerSpawn);
		OnSceneLoaded?.Invoke(scene, mode);
	}

	public static void GameQuit()
	{
		OnGameQuit?.Invoke();
	}

	public static void ChangeScreen(string toScreen, object args = null)
	{
		OnChangeScreen?.Invoke(toScreen, args);
	}

	internal static void FileStart(bool newFile)
	{
		OnFileStart?.Invoke(newFile);
	}
}