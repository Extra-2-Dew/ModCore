# ModCore

An essential dependency for many mods for Ittle Dew 2. Has an event API and some utility methods.

## Installation

ModCore and all mods that depend on it are **incompatible with Extra 2 Dew**. If you have Extra 2 Dew installed, you'll need to uninstall it by validating your files with Steam.

1. Install the latest release of [Bepinex](https://github.com/BepInEx/BepInEx/releases) via the [installation instructions](https://docs.bepinex.dev/articles/user_guide/installation/index.html). Ittle Dew 2 is a 64 bit game.
2. Download the latest of ModCore from the [releases page](https://github.com/Extra-2-Dew/ModCore/releases).
3. Unzip the release and drop it in your plugins folder.
4. If installed correctly, the Bepinex console should open when you launch Ittle Dew 2, and pressing ` while in-game should open the E2D2 console.

## Features

ModCore does not do much on its own, but it creates a debug console that can be used by other mods, as well as plenty of Utility API methods.

## Console

Press ` to open the console. To close, press it again. Alternatively, you can use Escape, or the X button on the top right.

To use a command, start typing with the input field selected. Press Enter to run the command.

Each page of the console can contain up to 16K characters. If the console history exceeds the limit, the console will put older history entries in another page. You can switch between pages using the arrows on the top left.

The following commands are included with ModCore:

* help: Lists all currently registered commands
* clear: Completely clears the console history
* lorum [quantity]: logs Lorum Ipsum to the console the specified number of times and states the console length in the Bepinex console.

## API

ModCore implements two classes for modders to use, Utility and Events.

### Utilities

* `string ColorText(string text, Color color)`: returns a string that has the proper color tags around it for displaying in UI.
* `string ColorText(string text, string color)`: returns a string that has the proper color tags around it for displaying in UI.
* `Transform FindNestedChild(Transform parent, string childName)`: Finds the requested child of the provided parent.
* `Transform FindNestedChild(string parentName, string childName)`: Finds the requested child of the provided parent.
* `Entity GetPlayer()`: Returns the player's Entity component.
* `LoadScene(int sceneBuildIndex, bool additively = false)`: Loads the scene with the given index.
* `LoadScene(string sceneName, bool additively = false)`: Loads the scene with the given name.
* `Texture2D GetTextureFromFile(string path)`: Loads a texture into memory from a file. The path is appended to the plugin path automatically.
* `bool TryParseJson<T>(string localPath, out Nullable<T> deserializedObj) where T : struct`: Tries to parse the provided JSON file into the requested type. Path starts at the Plugins folder.
* `TryParseJson<T>(string pluginName, string jsonFolder, string fileName, out T rootObj) where T : JsonBase`: Tries to parse the provided JSON file into the request JSONBase type.
* `bool TryParseVector3(string vectorAsString, out Vector3 result)`: Converts a vector string into a Vector3.

### Events

* `OnEntitySpawn(Entity entity)`: Runs whenever any entity spawns.
* `OnEntityDied(Entity entity, Killable.DetailedDeathData data)`: Runs whenever any entity dies, including the player.
* `OnFileStart(bool newFile)`: Runs when a file is loaded. Returns true if the file is a new file, false if loading an existing file.
* `OnGameQuit`: Runs when the game is closed entirely.
* `OnItemGet(Entity entity, Item item)`: Runs when the player picks up any item.
* `OnPauseChange(bool paused)`: Runs when the player pauses or unpauses. Returns true if pausing, false if unpausing.
* `OnPlayerSpawn(Entity player, GameObject camera, PlayerController controller)`: Runs when the player spawns in any given scene.
* `OnRoomChanged(Entity entity, LevelRoom toRoom, LevelRoom fromRoom, EntityEventsOwner.RoomEventData data)`: Runs when the player changes rooms.
* `OnSceneLoaded(Scene scene, LoadSceneMode mode)`: Runs whenever a new scene is loaded
