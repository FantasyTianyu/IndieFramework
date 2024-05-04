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
            baseAssetBundlePath = Application.streamingAssetsPath;

            // ����ƽ̨��̬����Ŀ¼��
            platformFolderName = GetPlatformFolderForAssetBundles();

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

            string bundlePath = Path.Combine(baseAssetBundlePath, bundleName);
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

            string bundlePath = Path.Combine(baseAssetBundlePath, bundleName);
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

        public static string GetPlatformFolderForAssetBundles() {
#if UNITY_EDITOR
            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget) {
                case UnityEditor.BuildTarget.StandaloneWindows:
                    return "StandaloneWindows";
                case UnityEditor.BuildTarget.Android:
                    return "Android";
                case UnityEditor.BuildTarget.iOS:
                    return "iOS";
                default:
                    return null;
            }
#else
            switch (Application.platform) {
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
#endif

        }
    }
}