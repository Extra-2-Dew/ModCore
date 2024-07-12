using BepInEx;
using Newtonsoft.Json;
using SmallJson;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModCore
{
	public static class Utility
	{
		/// <summary>
		/// Loads the scene by build index
		/// </summary>
		/// <param name="sceneBuildIndex">The build index</param>
		/// <param name="additively">Should the scene be loaded additively?</param>
		public static void LoadScene(int sceneBuildIndex, bool additively = false)
		{
			SceneManager.LoadScene(sceneBuildIndex, additively ? LoadSceneMode.Additive : LoadSceneMode.Single);
		}

		/// <summary>
		/// Loads the scene by name
		/// </summary>
		/// <param name="sceneName">The scene name</param>
		/// <param name="additively">Should the scene be loaded additively?</param>
		public static void LoadScene(string sceneName, bool additively = false)
		{
			SceneManager.LoadScene(sceneName, additively ? LoadSceneMode.Additive : LoadSceneMode.Single);
		}

		/// <summary>
		/// Colorizes the given text with the given color
		/// </summary>
		/// <param name="text">The text to colorize</param>
		/// <param name="color">The color to color the text with</param>
		/// <returns>The formatted string with the color tag added</returns>
		public static string ColorText(string text, Color color)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
		}

		/// <summary>
		/// Colorizes the given text with the given color
		/// </summary>
		/// <param name="text">The text to colorize</param>
		/// <param name="color">The HTML color to color the text with</param>
		/// <returns>The formatted string with the color tag added</returns>
		public static string ColorText(string text, string color)
		{
			if (color == "#000000") return text;
			return $"<color={color}>{text}</color>";
		}

		/// <summary>
		/// Tries to parse a string to Vector3
		/// </summary>
		/// <param name="vectorAsString">The string to parse</param>
		/// <param name="result">The parsed Vector3</param>
		/// <returns>True if parse was successful, false otherwise</returns>
		public static bool TryParseVector3(string vectorAsString, out Vector3 result)
		{
			result = Vector3.zero;
			vectorAsString = Regex.Replace(vectorAsString, @"[()]", "");
			string[] components = vectorAsString.Trim().Split(',');

			if (components.Length == 3)
			{
				if (!float.TryParse(components[0], out float x) || !float.TryParse(components[1], out float y) || !float.TryParse(components[2], out float z))
					return false;

				result = new Vector3(x, y, z);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Tries to parse JSON into the given type
		/// </summary>
		/// <typeparam name="T">The type to parse the data into</typeparam>
		/// <param name="localPath">The local path (starting at Plugins folder) to the JSON file</param>
		/// <param name="deserializedObj">The parsed object</param>
		/// <returns>True if parse was successful, false otherwise</returns>
		public static bool TryParseJson<T>(string localPath, out Nullable<T> deserializedObj) where T : struct
		{
			string jsonPath = BepInEx.Utility.CombinePaths(BepInEx.Paths.PluginPath, localPath);
			deserializedObj = null;

			try
			{
				if (!File.Exists(jsonPath))
				{
					Plugin.Log.LogWarning($"JSON file at path '{jsonPath}' was not found!");
					return false;
				}

				string json = File.ReadAllText(jsonPath);
				deserializedObj = JsonConvert.DeserializeObject<T>(json);
				return true;
			}
			catch (JsonException ex)
			{
				Plugin.Log.LogError($"JSON deserialization error: {ex.Message}");
				return false;
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError($"An error occurred: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Tries to parse JSON into the given JsonBase type
		/// </summary>
		/// <typeparam name="T">The JsonBase type to parse the data into</typeparam>
		/// <param name="pluginName">The name of the Plugin that has the JSON file to be parsed</param>
		/// <param name="jsonFolder">The name of the parent folder containing the JSON file to be parsed</param>
		/// <param name="fileName">The filename for the JSON file to be parsed</param>
		/// <param name="rootObj">The parsed JsonBase object</param>
		/// <returns>True if parse was successful, false otherwise</returns>
		public static bool TryParseJson<T>(string pluginName, string jsonFolder, string fileName, out T rootObj) where T : JsonBase
		{
			string jsonPath = BepInEx.Utility.CombinePaths(Paths.PluginPath, pluginName, jsonFolder, fileName);
			rootObj = null;

			try
			{
				// If file doesn't exist, do nothing
				if (!File.Exists(jsonPath))
				{
					Plugin.Log.LogWarning($"JSON file at path '{jsonPath} wasn't found!");
					return false;
				}

				rootObj = JsonBase.Decode<JsonObject>(File.ReadAllText(jsonPath)) as T;
				return rootObj != null;
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError($"Error when parsing JSON at path '{jsonPath}'\n{ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Searches for the child by name nested under the given parent
		/// </summary>
		/// <param name="parent">The parent Transform that contains the child</param>
		/// <param name="childName">The name of the child to look for</param>
		/// <returns>The found Transform of the child, null if not found</returns>
		public static Transform FindNestedChild(Transform parent, string childName)
		{
			foreach (Transform child in parent)
			{
				if (child.name == childName)
					return child;

				Transform result = FindNestedChild(child, childName);

				if (result != null)
					return result;
			}

			return null;
		}

		/// <summary>
		/// Searches for the child by name nested under the given parent
		/// </summary>
		/// <param name="parentName">The name of the parent that contains the child</param>
		/// <param name="childName">The name of the child to look for</param>
		/// <returns>The found Transform of the child, null if not found</returns>
		public static Transform FindNestedChild(string parentName, string childName)
		{
			foreach (Transform child in GameObject.Find(parentName).transform)
			{
				if (child.name == childName)
					return child;

				Transform result = FindNestedChild(child, childName);

				if (result != null)
					return result;
			}

			return null;
		}

		/// <summary>
		/// Creates a new Texture2D with the image found at the given path
		/// </summary>
		/// <param name="path">The relative path to the image (starting from mod directory)</param>
		/// <returns>The created Texture2D, or null if image was not found</returns>
		public static Texture2D GetTextureFromFile(string path)
		{
			try
			{
				string fullPath = BepInEx.Utility.CombinePaths(Paths.PluginPath, path);
				byte[] data = File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null;

				if (data == null)
				{
					Plugin.Log.LogError($"Error in GetTextureFromFile(): No file was found at path '{fullPath}'");
					return null;
				}

				Texture2D texture = new(512, 512, TextureFormat.RGBA32, false);
				texture.LoadImage(data);
				return texture;
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Error in GetTextureFromFile(): " + ex.Message);
				return null;
			}
		}
	}
}