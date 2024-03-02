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
			return $"<color={color}>{text}</color>";
		}

		/// <summary>
		/// Tries to parsea string to Vector3
		/// </summary>
		/// <param name="vectorAsString">The string to parse</param>
		/// <param name="result">The parsed Vector3</param>
		/// <returns>True if parse was successfull, false otherwise</returns>
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
	}
}