using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using AssetBundleBrowser;
using UnityEditor;
using static AssetBundleBrowser.AssetBundleBuildTab;

namespace IndieFramework {
    /// <summary>
    /// AssetBundle的版本信息生成器
    /// </summary>
    public class AssetBundleVersionInfoGenerator {
        public static void GenerateVersionInfo(BuildTabData buildData) {
            string outputPath = buildData.m_UseDefaultPath ?
                Path.Combine("AssetBundles", buildData.m_BuildTarget.ToString()) :
                buildData.m_OutputPath;
            var fullOutputPath = Path.GetFullPath(outputPath);
            var allAssetBundles = AssetDatabase.GetAllAssetBundleNames();
            AssetBundleVersionInfo versionInfo = new AssetBundleVersionInfo {
                Version = buildData.m_BuildVersion,
                assetBundleList = new List<AssetBundleVersionInfo.AssetBundleVersionEntry>()
            };

            foreach (var bundleName in allAssetBundles) {
                var bundlePath = Path.Combine(fullOutputPath, bundleName);
                if (File.Exists(bundlePath)) {
                    string hash = GetAssetBundleHash(bundlePath);
                    versionInfo.assetBundleList.Add(new AssetBundleVersionInfo.AssetBundleVersionEntry {
                        Name = bundleName,
                        Hash = hash,
                        Url = GenerateAssetBundleDownloadUrl(bundleName, buildData.m_BuildTarget.ToString())
                    });
                }
            }

            string versionInfoJson = JsonConvert.SerializeObject(versionInfo, Formatting.Indented);
            File.WriteAllText(Path.Combine(fullOutputPath, "AssetBundleVersionInfo.json"), versionInfoJson);
        }

        private static string GetAssetBundleHash(string bundlePath) {
            using (var sha256 = SHA256.Create()) {
                using (var stream = File.OpenRead(bundlePath)) {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private static string GenerateAssetBundleDownloadUrl(string bundleName, string platform) {
            // 根据CDN或服务的实际情况调整URL生成逻辑
            return $"https://yourcdn.com/AssetBundles/{platform}/{bundleName}";
        }
    }
}