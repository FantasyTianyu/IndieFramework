using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace IndieFramework {
    public class JsonUtil {
        // ���л�һ������JSON�������浽ָ��·��
        public static void SerializeToJson<T>(T objectToSerialize, string filePath) {
            // ���Ŀ¼�Ƿ���ڣ�����������򴴽�
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // ��ָ��·������JSON���������л�Ϊ����
        public static T DeserializeFromJson<T>(string filePath) {
            if (!File.Exists(filePath)) {
                // �������͵�Ĭ��ֵ������ null for classes
                return default(T);
            }

            string json = File.ReadAllText(filePath);
            T objectToDeserialize = JsonConvert.DeserializeObject<T>(json);
            return objectToDeserialize;
        }
    }
}