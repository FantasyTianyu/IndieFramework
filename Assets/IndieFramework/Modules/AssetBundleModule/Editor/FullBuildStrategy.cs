using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace IndieFramework {
    public class FullBuildStrategy : IBuildStrategy {
        private Dictionary<string, string> currentBuildHashes = new Dictionary<string, string>();
        private AssetBundleMapping assetBundleMapping;
        public AssetBundleBuild[] GetBundlesToBuild(IEnumerable<AssetBundleBuildRule> rules) {
            var bundlesToBuild = new List<AssetBundleBuild>();
            assetBundleMapping = new AssetBundleMapping();
            foreach (var rule in rules) {
                switch (rule.packMode) {
                    case PackMode.PackByFile:
                        var individualFiles = Directory.GetFiles(rule.destinationPath, "*.*", SearchOption.AllDirectories)
                            .Where(file => !file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                            .Select(file => file.Replace("\\", "/").Replace(Application.dataPath, "Assets"));

                        foreach (var file in individualFiles) {
                            var hash = CalculateSHA256(file);
                            currentBuildHashes[file] = hash;
                            bundlesToBuild.Add(new AssetBundleBuild {
                                assetBundleName = Path.GetFileNameWithoutExtension(file),
                                assetBundleVariant = rule.assetBundleVariant,
                                assetNames = new[] { file }
                            });
                            AssetImporter assetImporterPackByFile = AssetImporter.GetAtPath(file);
                            if (assetImporterPackByFile != null) {
                                // 设置AssetBundle的名字和变体
                                assetImporterPackByFile.assetBundleName = Path.GetFileNameWithoutExtension(file);
                                assetImporterPackByFile.assetBundleVariant = rule.assetBundleVariant;

                                // 保存修改
                                AssetDatabase.ImportAsset(file);
                            }
                            assetBundleMapping.AddEntry(file, $"{Path.GetFileNameWithoutExtension(file)}.{ rule.assetBundleVariant}".ToLowerInvariant());
                        }
                        break;

                    case PackMode.PackByDirectory:
                        var directories = Directory.GetDirectories(rule.destinationPath, "*", SearchOption.TopDirectoryOnly);

                        foreach (var dir in directories) {
                            var relatedFiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                                .Where(file => !file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                                .Select(file => file.Replace("\\", "/").Replace(Application.dataPath, "Assets"))
                                .ToArray();
                            var dirHash = string.Join("", relatedFiles.Select(CalculateSHA256).OrderBy(h => h));
                            currentBuildHashes[dir.Replace("\\", "/").Replace(Application.dataPath, "Assets")] = dirHash;
                            string abNamePackByDirectory = new DirectoryInfo(dir).Name;
                            bundlesToBuild.Add(new AssetBundleBuild {
                                assetBundleName = abNamePackByDirectory,
                                assetBundleVariant = rule.assetBundleVariant,
                                assetNames = relatedFiles
                            });
                            foreach (var file in relatedFiles) {
                                AssetImporter assetImporterPackByFile = AssetImporter.GetAtPath(file);
                                if (assetImporterPackByFile != null) {
                                    // 设置AssetBundle的名字和变体
                                    assetImporterPackByFile.assetBundleName = abNamePackByDirectory;
                                    assetImporterPackByFile.assetBundleVariant = rule.assetBundleVariant;

                                    // 保存修改
                                    AssetDatabase.ImportAsset(file);
                                }
                                assetBundleMapping.AddEntry(file, $"{abNamePackByDirectory}.{ rule.assetBundleVariant}".ToLowerInvariant());
                            }
                        }
                        break;

                    case PackMode.PackTogether:
                        var allFiles = Directory.GetFiles(rule.destinationPath, "*.*", SearchOption.AllDirectories)
                            .Where(file => !file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                            .Select(file => file.Replace("\\", "/").Replace(Application.dataPath, "Assets"))
                            .ToArray();
                        var allFilesHash = string.Join("", allFiles.Select(CalculateSHA256).OrderBy(h => h));
                        currentBuildHashes[rule.destinationPath.Replace("\\", "/").Replace(Application.dataPath, "Assets")] = allFilesHash;
                        string abNamePackTogether = new DirectoryInfo(rule.destinationPath).Name;
                        bundlesToBuild.Add(new AssetBundleBuild {
                            assetBundleName = new DirectoryInfo(rule.destinationPath).Name,
                            assetBundleVariant = rule.assetBundleVariant,
                            assetNames = allFiles
                        });
                        foreach (var file in allFiles) {
                            AssetImporter assetImporterPackByFile = AssetImporter.GetAtPath(file);
                            if (assetImporterPackByFile != null) {
                                // 设置AssetBundle的名字和变体
                                assetImporterPackByFile.assetBundleName = abNamePackTogether;
                                assetImporterPackByFile.assetBundleVariant = rule.assetBundleVariant;

                                // 保存修改
                                AssetDatabase.ImportAsset(file);
                            }
                            assetBundleMapping.AddEntry(file, $"{ new DirectoryInfo(rule.destinationPath).Name }.{rule.assetBundleVariant}".ToLowerInvariant());
                        }
                        break;
                }
            }
            SaveCurrentBuildHashes();
            assetBundleMapping.SaveToPath();
            AssetDatabase.Refresh();
            return bundlesToBuild.ToArray();
        }
        private void SaveCurrentBuildHashes() {
            // 使用Newtonsoft.Json来序列化currentBuildHashes字典
            string json = JsonConvert.SerializeObject(currentBuildHashes, Formatting.Indented);
            // 保证缓存文件夹存在
            var fileInfo = new FileInfo(RuleSaveUtil.AssetBundleHashesPath);
            fileInfo.Directory.Create(); // If the directory already exists, this method does nothing
            // 将哈希值JSON字符串写入到缓存文件
            File.WriteAllText(RuleSaveUtil.AssetBundleHashesPath, json);
        }
        private string CalculateSHA256(string filePath) {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            return BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }

    }
}