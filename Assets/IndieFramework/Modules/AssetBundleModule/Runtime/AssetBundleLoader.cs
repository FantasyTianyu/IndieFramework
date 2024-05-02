using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace IndieFramework {
    public class AssetBundleLoader {
        private Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();
        private AssetBundleManifest mainManifest;
        private string baseAssetBundlePath;
        private string platformFolderName;

        public AssetBundleLoader() {
            // 根据你项目实际用途设置，比如Application.streamingAssetsPath或Application.persistentDataPath
            baseAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles");

            // 根据平台动态设置目录名
            platformFolderName = GetPlatformFolderForAssetBundles(Application.platform);

            // 因为涉及文件IO操作，不要在构造函数中加载manifest，而是使用一个初始化方法
        }

        public void Initialize() {
            // 这里使用同步加载作为初始化过程的一部分，因为处理初始化可以允许一个阻塞调用
            string manifestBundlePath = Path.Combine(baseAssetBundlePath, platformFolderName);

            AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestBundlePath);
            if (manifestBundle != null) {
                mainManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                manifestBundle.Unload(false);
            } else {
                Debug.LogError("Failed to load the main AssetBundle manifest.");
            }
        }

        public async Task<AssetBundle> LoadBundleAsync(string bundleName) {
            if (loadedAssetBundles.TryGetValue(bundleName, out var loadedBundle)) {
                return loadedBundle;
            }

            string[] bundleDependencies = mainManifest.GetAllDependencies(bundleName);
            foreach (var dependency in bundleDependencies) {
                await LoadBundleAsync(dependency);
            }

            string bundlePath = Path.Combine(baseAssetBundlePath, platformFolderName, bundleName);
            loadedBundle = await Task.Run(() => AssetBundle.LoadFromFile(bundlePath));

            if (loadedBundle != null) {
                loadedAssetBundles.Add(bundleName, loadedBundle);
            } else {
                Debug.LogError("Failed to load AssetBundle: " + bundleName);
            }

            return loadedBundle;
        }

        public AssetBundle LoadBundle(string bundleName) {
            if (loadedAssetBundles.TryGetValue(bundleName, out var loadedBundle)) {
                return loadedBundle;
            }

            string[] bundleDependencies = mainManifest.GetAllDependencies(bundleName);
            foreach (var dependency in bundleDependencies) {
                LoadBundle(dependency);
            }

            string bundlePath = Path.Combine(baseAssetBundlePath, platformFolderName, bundleName);
            loadedBundle = AssetBundle.LoadFromFile(bundlePath);

            if (loadedBundle != null) {
                loadedAssetBundles.Add(bundleName, loadedBundle);
            } else {
                Debug.LogError("Failed to load AssetBundle: " + bundleName);
            }

            return loadedBundle;
        }

        public void UnloadBundle(string bundleName) {
            if (loadedAssetBundles.TryGetValue(bundleName, out var bundle)) {
                bundle.Unload(true);
                loadedAssetBundles.Remove(bundleName);
            }
        }

        public static string GetPlatformFolderForAssetBundles(RuntimePlatform platform) {
            switch (platform) {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "StandaloneWindows";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                default:
                    return null;
            }
        }
    }
}