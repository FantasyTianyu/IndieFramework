using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace IndieFramework {
    public class AssetBundleMapping {
        private Dictionary<string, string> pathToBundleMap = new Dictionary<string, string>();

        // ����ʱ·�������ڼ���AssetBundleӳ���JSON�ļ�����AssetBundleLoader���baseAssetBundlePathӦ����ͬһ��Ŀ¼
        // (����Ǵ�������Ϸ��Դ���ŵ�StreammingAssets�£���ô���mappingҲӦ�÷ŵ�StreammingAssets�£�����ǰ�����Դab�����µģ���ô���mappingӦ�÷ŵ�cdn�������ص�persistentDataPath��)
#if UNITY_EDITOR
        private static string RuntimeBundleMappingFilePath => Path.Combine(Application.dataPath.Replace("Assets", "AssetBundles"), AssetBundleLoader.GetPlatformFolderForAssetBundles(), "AssetBundleMapping.json").Replace("\\", "/");
#else
        private static string RuntimeBundleMappingFilePath => Path.Combine(Application.streamingAssetsPath, "AssetBundleMapping.json").Replace("\\", "/");
#endif


        // ��ʼ��ӳ�����ݵķ���
        public static async Task<AssetBundleMapping> InitializeAsync() {
            string filePath = RuntimeBundleMappingFilePath;

            if (File.Exists(filePath)) {
                string json = await ReadFileAsync(filePath);
                var mappingDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return new AssetBundleMapping { pathToBundleMap = mappingDict };
            }

            Debug.LogError("Failed to load AssetBundle mapping file at: " + filePath);
            return null;
        }

        public void AddEntry(string assetPath, string bundleName) {
            pathToBundleMap[assetPath] = bundleName;
        }

        public string GetBundleName(string assetPath) {
            pathToBundleMap.TryGetValue(assetPath, out string bundleName);
            return bundleName;
        }

        // ���浽����ʱ·���ķ���
        public void SaveToPath() {
            string json = JsonConvert.SerializeObject(pathToBundleMap, Formatting.Indented);
            var fileInfo = new FileInfo(RuntimeBundleMappingFilePath);
            fileInfo.Directory.Create(); // If the directory already exists, this method does nothing
            File.WriteAllText(RuntimeBundleMappingFilePath, json);
        }

        private static async Task<string> ReadFileAsync(string filePath) {
            using (var reader = new StreamReader(filePath)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
}