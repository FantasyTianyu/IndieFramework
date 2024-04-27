using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IndieFramework {
    public class AssetBundleEditRuleTab {

        private Vector2 scrollPosition;
        private string newRuleDirectory;
        private List<AssetBundleBuildRule> buildRules;
        internal void OnEnable() {
            Debug.Log("���ص�ǰ�Ĵ������");
            LoadBuildRules();
        }

        internal void RefreshData() {
            LoadBuildRules();
        }




        private void LoadBuildRules() {
            buildRules = RuleSaveUtil.DeserializeFromJson<List<AssetBundleBuildRule>>();
            if (buildRules == null) {
                Debug.LogError("Failed to load build rules from config file. Creating a new list.");
                buildRules = new List<AssetBundleBuildRule>();
            }
        }
        private void SaveBuildRules() {
            RuleSaveUtil.SerializeToJson<List<AssetBundleBuildRule>>(buildRules);
            AssetDatabase.Refresh();
            Debug.Log("AssetBundle Build Rules saved.");
        }

        private void DrawRuleDetail(AssetBundleBuildRule rule) {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            rule.destinationPath = EditorGUILayout.TextField("Destination Path:", rule.destinationPath);
            if (GUILayout.Button("...", GUILayout.MaxWidth(30))) {
                var selectedPath = EditorUtility.OpenFolderPanel("Select Directory", rule.destinationPath, "");
                if (!string.IsNullOrEmpty(selectedPath)) {
                    rule.destinationPath = selectedPath;
                }
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            //rule.assetBundleName = EditorGUILayout.TextField("AssetBundle Name:", rule.assetBundleName);
            rule.assetBundleVariant = EditorGUILayout.TextField("AssetBundle Variant:", rule.assetBundleVariant);
            rule.packMode = (PackMode)EditorGUILayout.EnumPopup("Pack Mode:", rule.packMode);
            EditorGUI.indentLevel--;
        }

        private void CreateNewRule(string path) {
            var newRule = new AssetBundleBuildRule {
                destinationPath = path,
                foldout = false,
                //assetBundleName = "",
                assetBundleVariant = "",
                packMode = PackMode.PackByDirectory,
            };
            buildRules.Add(newRule);
        }

        internal void OnGUI() {
            GUILayout.Label("AssetBundle Build Rules", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            newRuleDirectory = EditorGUILayout.TextField("New Rule Directory:", newRuleDirectory);
            if (GUILayout.Button("...", GUILayout.MaxWidth(30))) {
                newRuleDirectory = EditorUtility.OpenFolderPanel("Select Directory for New Rule", newRuleDirectory, "");
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Add", GUILayout.MaxWidth(60))) {
                CreateNewRule(newRuleDirectory);
            }
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (buildRules != null) {
                for (int i = 0; i < buildRules.Count; i++) {
                    // ʹ�� VerticalScope ��Χÿ�� Foldout ����������
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                        EditorGUILayout.BeginHorizontal();
                        buildRules[i].foldout = EditorGUILayout.Foldout(buildRules[i].foldout, buildRules[i].destinationPath, true);
                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(80))) {
                            buildRules.RemoveAt(i);
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                        EditorGUILayout.EndHorizontal();

                        if (buildRules[i].foldout) {
                            DrawRuleDetail(buildRules[i]);
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(); // �ڱ��水ť֮ǰ���һЩ�ռ�

            // ����ͨ��GUILayout�ռ�ϵ�к�����ȷ����ť�ڵײ���ʾ
            GUILayout.FlexibleSpace();

            // ����һ����ť����������չ���������ڵĿ��
            if (GUILayout.Button("Save Rules", GUILayout.ExpandWidth(true), GUILayout.Height(40))) {
                SaveBuildRules();
            }

            // Ϊ���ڵײ�Ԥ���㹻�ռ�
            GUILayout.Space(10);
        }
    }
}