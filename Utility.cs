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
	}
}