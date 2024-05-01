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
    public class IncrementalBuildStrategy : IBuildStrategy {
        private Dictionary<string, string> lastBuildHashes;
        private Dictionary<string, string> currentBuildHashes = new Dictionary<string, string>();
        public IncrementalBuildStrategy() {
            this.lastBuildHashes = LoadOrInitializeLastBuildHashes();
        }
        private Dictionary<string, string> LoadOrInitializeLastBuildHashes() {
            if (File.Exists(RuleSaveUtil.AssetBundleHashesPath)) {
                // ��������ļ����ڣ�����ļ����ع�ϣֵ
                string json = File.ReadAllText(RuleSaveUtil.AssetBundleHashesPath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            } else {
                // ��������ļ������ڣ����ʼ��һ���µĹ�ϣֵ�ֵ�
                return new Dictionary<string, string>();
            }
        }
        public AssetBundleBuild[] GetBundlesToBuild(IEnumerable<AssetBundleBuildRule> rules) {
            var bundlesToBuild = new List<AssetBundleBuild>();
            foreach (var rule in rules) {
                switch (rule.packMode) {
                    case PackMode.PackByFile:
                        var individualFiles = Directory.GetFiles(rule.destinationPath, ".", SearchOption.AllDirectories)
                            .Where(file => !file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                            .Select(file => file.Replace("\\", "/").Replace(Application.dataPath, "Assets"));
                        foreach (var file in individualFiles) {
                            var hash = CalculateSHA256(file);
                            currentBuildHashes[file] = hash;
                            // �ļ�hash�����ڻ��ߺ�֮ǰ��hash��һ�������·����������У�ʵ���������
                            if (!lastBuildHashes.TryGetValue(file, out var lastHash) || lastHash != hash) {
                                bundlesToBuild.Add(new AssetBundleBuild {
                                    assetBundleName = Path.GetFileNameWithoutExtension(file),
                                    assetNames = new[] { file }
                                });
                                AssetImporter assetImporter = AssetImporter.GetAtPath(file);
                                if (assetImporter != null) {
                                    // ����AssetBundle�����ֺͱ���
                                    assetImporter.assetBundleName = Path.GetFileNameWithoutExtension(file);
                                    assetImporter.assetBundleVariant = rule.assetBundleVariant;

                                    // �����޸�
                                    AssetDatabase.ImportAsset(file);
                                }
                            }
                        }
                        break;
                    case PackMode.PackByDirectory:
                        var directories = Directory.GetDirectories(rule.destinationPath, "*", SearchOption.TopDirectoryOnly);
                        foreach (var dir in directories) {
                            var relatedFiles = Directory.GetFiles(dir, ".", SearchOption.AllDirectories)
                                .Where(file => !file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                                .Select(file => file.Replace("\\", "/").Replace(Application.dataPath, "Assets"))
                                .ToArray();
                            // �ļ���hash�����ڻ��ߺ�֮ǰ��hash��һ�������·����������У�ʵ���������
                            var dirHash = string.Join("", relatedFiles.Select(CalculateSHA256).OrderBy(h => h));
                            string assetBundleKeyPackByDirectory = dir.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                            currentBuildHashes[assetBundleKeyPackByDirectory] = dirHash;
                            if (!lastBuildHashes.TryGetValue(assetBundleKeyPackByDirectory, out var lastDirHash) || lastDirHash != dirHash) {
                                string abName = new DirectoryInfo(dir).Name;
                                bundlesToBuild.Add(new AssetBundleBuild {
                                    assetBundleName = abName,
                                    assetNames = relatedFiles
                                });
                                foreach (var file in relatedFiles) {
                                    AssetImporter assetImporterPackByFile = AssetImporter.GetAtPath(file);
                                    if (assetImporterPackByFile != null) {
                                        // ����AssetBundle�����ֺͱ���
                                        assetImporterPackByFile.assetBundleName = abName;
                                        assetImporterPackByFile.assetBundleVariant = rule.assetBundleVariant;

                                        // �����޸�
                                        AssetDatabase.ImportAsset(file);
                                    }
                                }
                            }
                        }
                        break;
                    case PackMode.PackTogether:
                        var allFiles = Directory.GetFiles(rule.destinationPath, ".", SearchOption.AllDirectories)
                            .Where(file => !file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                            .Select(file => file.Replace("\\", "/").Replace(Application.dataPath, "Assets"))
                            .ToArray();
                        // �ļ���hash�����ڻ��ߺ�֮ǰ��hash��һ�������·����������У�ʵ���������
                        var allFilesHash = string.Join("", allFiles.Select(CalculateSHA256).OrderBy(h => h));
                        string assetBundleKeyPackTogether = rule.destinationPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                        currentBuildHashes[assetBundleKeyPackTogether] = allFilesHash;
                        if (!lastBuildHashes.TryGetValue(assetBundleKeyPackTogether, out var lastAllFilesHash) || lastAllFilesHash != allFilesHash) {
                            string abName = new DirectoryInfo(rule.destinationPath).Name;
                            bundlesToBuild.Add(new AssetBundleBuild {
                                assetBundleName = abName,
                                assetNames = allFiles
                            });
                            foreach (var file in allFiles) {
                                AssetImporter assetImporterPackByFile = AssetImporter.GetAtPath(file);
                                if (assetImporterPackByFile != null) {
                                    // ����AssetBundle�����ֺͱ���
                                    assetImporterPackByFile.assetBundleName = abName;
                                    assetImporterPackByFile.assetBundleVariant = rule.assetBundleVariant;

                                    // �����޸�
                                    AssetDatabase.ImportAsset(file);
                                }
                            }
                        }
                        break;
                }
            }
            SaveCurrentBuildHashes();
            AssetDatabase.Refresh();
            return bundlesToBuild.ToArray();

        }

        private void SaveCurrentBuildHashes() {
            // ʹ��Newtonsoft.Json�����л�currentBuildHashes�ֵ�
            string json = JsonConvert.SerializeObject(currentBuildHashes, Formatting.Indented);
            // ��֤�����ļ��д���
            var fileInfo = new FileInfo(RuleSaveUtil.AssetBundleHashesPath);
            fileInfo.Directory.Create(); // If the directory already exists, this method does nothing
            // ����ϣֵJSON�ַ���д�뵽�����ļ�
            File.WriteAllText(RuleSaveUtil.AssetBundleHashesPath, json);
        }
        private string CalculateSHA256(string filePath) {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            return BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }


    }
}