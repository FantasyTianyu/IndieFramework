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
        private AssetBundleMapping assetBundleMapping;
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
            assetBundleMapping = new AssetBundleMapping();
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
                                    assetBundleVariant = rule.assetBundleVariant,
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
                            assetBundleMapping.AddEntry(file, $"{Path.GetFileNameWithoutExtension(file)}.{ rule.assetBundleVariant}".ToLowerInvariant());
                        }
                        break;
                    case PackMode.PackByDirectory:
                        var directories = Directory.GetDirectories(rule.destinationPath, "*", SearchOption.TopDirectoryOnly);
                        foreach (var dir in directories) {
                            string dirRelativePath = dir.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                            var relatedFiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                                .Where(file => !file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                                .Select(file => file.Replace("\\", "/").Replace(Application.dataPath, "Assets"))
                                .ToArray();

                            string dirHash = CalculateDirectoryHash(relatedFiles);
                            currentBuildHashes[dirRelativePath] = dirHash;

                            bool directoryHasChanged = !lastBuildHashes.TryGetValue(dirRelativePath, out var lastDirHash) || lastDirHash != dirHash;

                            if (directoryHasChanged) {
                                string bundleName = Path.GetFileName(dir);
                                bundlesToBuild.Add(new AssetBundleBuild {
                                    assetBundleName = bundleName,
                                    assetBundleVariant = rule.assetBundleVariant,
                                    assetNames = relatedFiles
                                });
                            }

                            string abNamePackByDirectory = Path.GetFileName(dir);
                            foreach (var file in relatedFiles) {
                                assetBundleMapping.AddEntry(file, $"{abNamePackByDirectory}.{ rule.assetBundleVariant}".ToLowerInvariant());
                                if (directoryHasChanged) {
                                    // �ļ���hash�б䶯�������AssetImporter��Ϣ
                                    var importer = AssetImporter.GetAtPath(file);
                                    if (importer != null) {
                                        importer.assetBundleName = abNamePackByDirectory;
                                        importer.assetBundleVariant = rule.assetBundleVariant;
                                        AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate);
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
                        string abNamePackTogether = new DirectoryInfo(rule.destinationPath).Name;
                        if (!lastBuildHashes.TryGetValue(assetBundleKeyPackTogether, out var lastAllFilesHash) || lastAllFilesHash != allFilesHash) {
                            bundlesToBuild.Add(new AssetBundleBuild {
                                assetBundleName = abNamePackTogether,
                                assetBundleVariant = rule.assetBundleVariant,
                                assetNames = allFiles
                            });
                            foreach (var file in allFiles) {

                                AssetImporter assetImporterPackByFile = AssetImporter.GetAtPath(file);
                                if (assetImporterPackByFile != null) {
                                    // ����AssetBundle�����ֺͱ���
                                    assetImporterPackByFile.assetBundleName = abNamePackTogether;
                                    assetImporterPackByFile.assetBundleVariant = rule.assetBundleVariant;

                                    // �����޸�
                                    AssetDatabase.ImportAsset(file);
                                }
                            }
                        }
                        foreach (var file in allFiles) {
                            assetBundleMapping.AddEntry(file, $"{abNamePackTogether}.{rule.assetBundleVariant}".ToLowerInvariant());
                        }
                        break;
                }
            }
            assetBundleMapping.SaveToPath();
            SaveCurrentBuildHashes();
            AssetDatabase.Refresh();
            return bundlesToBuild.ToArray();

        }

        private string CalculateDirectoryHash(IEnumerable<string> filePaths) {
            using (SHA256 sha256 = SHA256.Create()) {
                List<byte> hashList = new List<byte>();

                foreach (var filePath in filePaths.OrderBy(p => p).Select(f => Path.Combine(Application.dataPath, f.Substring(7)))) // ��Assets/ȥ��
                {
                    byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(filePath);
                    hashList.AddRange(sha256.ComputeHash(pathBytes));

                    if (File.Exists(filePath)) {
                        byte[] contentBytes = File.ReadAllBytes(filePath);
                        hashList.AddRange(sha256.ComputeHash(contentBytes));
                    }
                }

                var hashBytes = sha256.ComputeHash(hashList.ToArray());
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
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