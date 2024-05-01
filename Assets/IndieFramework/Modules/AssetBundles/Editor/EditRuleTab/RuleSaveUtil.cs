using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IndieFramework {
    public class RuleSaveUtil {
        public static readonly string AssetBundleHashesPath = "Assets/AssetBundleCache/AssetBundleHashes.json";
        // �����ļ����ڵ�·��
        private static readonly string configFilePath = "Assets/AssetBundleCache/AssetBundleBuildRules.json";
#if UNITY_EDITOR
        // ���л�һ������JSON�������浽ָ��·��
        public static void SerializeToJson<T>(T objectToSerialize) {
            // ���Ŀ¼�Ƿ���ڣ�����������򴴽�
            string directoryPath = Path.GetDirectoryName(configFilePath);

            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
            File.WriteAllText(configFilePath, json);
        }

        // ��ָ��·������JSON���������л�Ϊ����
        public static T DeserializeFromJson<T>() {
            if (!File.Exists(configFilePath)) {
                // �������͵�Ĭ��ֵ������ null for classes
                return default(T);
            }

            string json = File.ReadAllText(configFilePath);
            T objectToDeserialize = JsonConvert.DeserializeObject<T>(json);
            return objectToDeserialize;
        }
    }
#endif
}