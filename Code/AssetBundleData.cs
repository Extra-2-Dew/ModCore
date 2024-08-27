using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModCore
{
    internal static class AssetBundleData
    {
        public static Dictionary<string, BundleData> bundlePaths;

        public class BundleData()
        {
            public AssetBundle bundle;
            public Dictionary<string, GameObject> loadedObjects;
        }

        public static GameObject GetObjectFromBundle(string bundlePath, string objectPath)
        {
            bundlePaths ??= new();

            if (!bundlePaths.ContainsKey(bundlePath))
            {
                bundlePaths.Add(bundlePath, new());
                bundlePaths[bundlePath].bundle = AssetBundle.LoadFromFile(bundlePath);
                bundlePaths[bundlePath].loadedObjects = new();
            }

            BundleData data = bundlePaths[bundlePath];

            if (!data.loadedObjects.ContainsKey(objectPath))
            {
                data.loadedObjects.Add(objectPath, data.bundle.LoadAsset<GameObject>(objectPath));
                ShaderFix(data.loadedObjects[objectPath]);
            }

            return data.loadedObjects[objectPath];
        }

        private static void ShaderFix(GameObject prefab)
        {
            if (prefab.GetComponent<MeshRenderer>() != null)
            {
                ReplaceShaders(prefab.GetComponent<MeshRenderer>());
            }
            foreach (MeshRenderer rend in prefab.GetComponentsInChildren<MeshRenderer>())
            {
                ReplaceShaders(rend);
            }
        }

        private static void ReplaceShaders(MeshRenderer rend)
        {
            foreach (Material mat in rend.sharedMaterials)
            {
                string shaderName = mat.shader.name;
                if (shaderName.Contains("Dummy"))
                {
                    Shader shader = Shader.Find(shaderName.Replace("Dummy", "Custom"));
                    Object.DontDestroyOnLoad(shader);
                    mat.shader = shader;
                }
            }
        }
    }
}
