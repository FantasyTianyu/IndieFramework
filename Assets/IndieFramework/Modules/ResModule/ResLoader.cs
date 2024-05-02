using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace IndieFramework {
    public class ResLoader {
        private static AssetBundleMapping assetBundleMapping;
        private static AssetBundleLoader assetBundleLoader;
        public static async Task InitializeAsync() {
            assetBundleLoader = new AssetBundleLoader();
            assetBundleLoader.Initialize();
            assetBundleMapping = await AssetBundleMapping.InitializeAsync();
            if (assetBundleMapping == null) {
                throw new System.Exception("AssetBundleMapping initialization failed.");
            }
        }

        public static async Task<T> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object {
#if UNITY_EDITOR
            await Task.Delay(100); // ƒ£ƒ‚“Ï≤Ωº”‘ÿ—”≥Ÿ
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            if (assetBundleMapping == null) {
                Debug.LogError("AssetBundleMapping is not initialized. Call InitializeAsync() first.");
                return null;
            }
            string bundleName = assetBundleMapping.GetBundleName(assetPath);
            if (string.IsNullOrEmpty(bundleName)) {
                Debug.LogError($"AssetBundle name for asset '{assetPath}' not found.");
                return null;
            }

            AssetBundle assetBundle = await assetBundleLoader.LoadBundleAsync(bundleName);
            if (!assetBundle) {
                Debug.LogError($"Failed to load AssetBundle: {bundleName}");
                return null;
            }
            T asset = assetBundle.LoadAsset<T>(assetPath);
            // Optionally, you may want to unload the AssetBundle here if it's no longer needed
            return asset;
#endif
        }

        public static T LoadAsset<T>(string assetPath) where T : UnityEngine.Object {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            if (assetBundleMapping == null) {
                Debug.LogError("AssetBundleMapping is not initialized. Call InitializeAsync() first.");
                return null;
            }
            string bundleName = assetBundleMapping.GetBundleName(assetPath);
            if (string.IsNullOrEmpty(bundleName)) {
                Debug.LogError($"AssetBundle name for asset '{assetPath}' not found.");
                return null;
            }
            AssetBundle assetBundle = assetBundleLoader.LoadBundle(bundleName);
            if (!assetBundle) {
                Debug.LogError($"Failed to load AssetBundle: {bundleName}");
                return null;
            }
            T asset = assetBundle.LoadAsset<T>(assetPath);
            // Optionally, you may want to unload the AssetBundle here if it's no longer needed
            return asset;
#endif
        }
    }
}