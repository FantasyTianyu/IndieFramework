using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace IndieFramework {
    public class JsonUtil {
        // 序列化一个对象到JSON，并保存到指定路径
        public static void SerializeToJson<T>(T objectToSerialize, string filePath) {
            // 检查目录是否存在，如果不存在则创建
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // 从指定路径加载JSON，并反序列化为对象
        public static T DeserializeFromJson<T>(string filePath) {
            if (!File.Exists(filePath)) {
                // 返回类型的默认值，比如 null for classes
                return default(T);
            }

            string json = File.ReadAllText(filePath);
            T objectToDeserialize = JsonConvert.DeserializeObject<T>(json);
            return objectToDeserialize;
        }
    }
}