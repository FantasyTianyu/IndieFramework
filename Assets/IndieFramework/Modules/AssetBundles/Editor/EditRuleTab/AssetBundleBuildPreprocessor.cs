using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IndieFramework {
    public class AssetBundleBuildPreprocessor {
        private string assetsFolderPath = "Assets/"; // 资产的根文件夹路径
        public AssetBundleBuild[] ProcessRules() {
            var rules = RuleSaveUtil.DeserializeFromJson<List<AssetBundleBuildRule>>();

            if (rules == null || rules.Count == 0) {
                Debug.LogError("No build rules found or failed to load them.");
                return null;
            }

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

            foreach (var rule in rules) {
                switch (rule.packMode) {
                    case PackMode.PackByFile:
                        string[] allFiles = Directory.GetFiles(assetsFolderPath + rule.destinationPath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in allFiles) {
                            if (!file.EndsWith(".meta")) {
                                string normalizedPath = file.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                                builds.Add(new AssetBundleBuild {
                                    assetBundleName = Path.GetFileNameWithoutExtension(normalizedPath),
                                    assetNames = new[] { normalizedPath }
                                });
                            }
                        }
                        break;

                    case PackMode.PackByDirectory:
                        var subDirectories = Directory.GetDirectories(assetsFolderPath + rule.destinationPath, "*", SearchOption.AllDirectories);
                        foreach (var subDir in subDirectories) {
                            string relativeDirPath = subDir.Replace("\\", "/").Replace(Application.dataPath, "Assets/");
                            string bundleName = Path.GetFileName(relativeDirPath);
                            var files = Directory.GetFiles(subDir, "*.*", SearchOption.AllDirectories)
                                .Where(f => !f.EndsWith(".meta"))
                                .Select(f => f.Replace("\\", "/").Replace(Application.dataPath, "Assets"))
                                .ToArray();

                            if (files.Length > 0) {
                                builds.Add(new AssetBundleBuild {
                                    assetBundleName = bundleName,
                                    assetNames = files
                                });
                            }
                        }
                        break;

                    case PackMode.PackTogether:
                        var allAssets = Directory.GetFiles(assetsFolderPath + rule.destinationPath, "*.*", SearchOption.AllDirectories)
                            .Where(f => !f.EndsWith(".meta"))
                            .Select(f => f.Replace("\\", "/").Replace(Application.dataPath, "Assets"))
                            .ToArray();

                        string folderName = new DirectoryInfo(rule.destinationPath).Name;
                        builds.Add(new AssetBundleBuild {
                            assetBundleName = folderName,
                            assetNames = allAssets
                        });
                        break;

                        //TODO 其他打包模式...
                }
            }

            return builds.ToArray();
        }
    }
}