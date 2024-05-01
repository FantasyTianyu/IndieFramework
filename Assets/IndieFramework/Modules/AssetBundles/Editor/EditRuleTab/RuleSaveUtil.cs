using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IndieFramework {
    public class RuleSaveUtil {
        public static readonly string AssetBundleHashesPath = "Assets/AssetBundleCache/AssetBundleHashes.json";
        // 配置文件所在的路径
        private static readonly string configFilePath = "Assets/AssetBundleCache/AssetBundleBuildRules.json";
#if UNITY_EDITOR
        // 序列化一个对象到JSON，并保存到指定路径
        public static void SerializeToJson<T>(T objectToSerialize) {
            // 检查目录是否存在，如果不存在则创建
            string directoryPath = Path.GetDirectoryName(configFilePath);

            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
            File.WriteAllText(configFilePath, json);
        }

        // 从指定路径加载JSON，并反序列化为对象
        public static T DeserializeFromJson<T>() {
            if (!File.Exists(configFilePath)) {
                // 返回类型的默认值，比如 null for classes
                return default(T);
            }

            string json = File.ReadAllText(configFilePath);
            T objectToDeserialize = JsonConvert.DeserializeObject<T>(json);
            return objectToDeserialize;
        }
    }
#endif
}