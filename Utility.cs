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
	}
}