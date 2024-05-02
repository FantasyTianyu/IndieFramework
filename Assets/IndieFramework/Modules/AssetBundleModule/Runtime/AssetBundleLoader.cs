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
            // ��������Ŀʵ����;���ã�����Application.streamingAssetsPath��Application.persistentDataPath
            baseAssetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles");

            // ����ƽ̨��̬����Ŀ¼��
            platformFolderName = GetPlatformFolderForAssetBundles(Application.platform);

            // ��Ϊ�漰�ļ�IO��������Ҫ�ڹ��캯���м���manifest������ʹ��һ����ʼ������
        }

        public void Initialize() {
            // ����ʹ��ͬ��������Ϊ��ʼ�����̵�һ���֣���Ϊ�����ʼ����������һ����������
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