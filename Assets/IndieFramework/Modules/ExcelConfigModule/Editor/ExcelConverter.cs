using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Text;

namespace IndieFramework {
    public class ExcelConverter {
        public static string EXCEL_PATH => Path.Combine(Application.dataPath, "Res/Design/Excel");
        public class ExcelFileEntry {
            public string className;//excel文件的名字，作为类名
            public string[] propertyNames;//excel文件字段名字
            public string[] propertyTypes;//excel文件字段的类型
            public string[] propertyDescriptions;//excel文件字段的描述
            public List<string[]> datas;//数据
        }

        [MenuItem("Tools/Excel/Convert All Excel")]
        public static void ConvertAllExcel() {
            string[] files = Directory.GetFiles(EXCEL_PATH, "*.xlsx");

            List<ExcelFileEntry> fileEntries = new List<ExcelFileEntry>();

            for (int i = 0; i < files.Length; i++) {
                string filePath = files[i];
                ExcelFileEntry fileEntry = ConvertExcel(filePath);
                if (fileEntry == null) {
                    continue;
                }
                fileEntries.Add(fileEntry);
            }
            GenerateBinaryFiles(fileEntries);
            AssetDatabase.Refresh();
        }
        [MenuItem("Assets/Excel/Convert Selection Excel")]
        public static void ConvertSelectionExcel() {
            Object[] selectedObjects = Selection.objects;
            if (selectedObjects.Length > 0) {
                List<ExcelFileEntry> fileEntries = new List<ExcelFileEntry>();
                foreach (var selectedObject in selectedObjects) {
                    if (selectedObject != null) {
                        string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                        string fileName = Path.GetFileName(assetPath);
                        if (!fileName.EndsWith(".xlsx")) {
                            Debug.LogError("选择的不是excel文件 (Selection file is not a excel file.)");
                            return;
                        }
                        ExcelFileEntry fileEntry = ConvertExcel($"{EXCEL_PATH}/{fileName}");
                        if (fileEntry == null) {
                            continue;
                        }
                        fileEntries.Add(fileEntry);
                    }
                }
                GenerateBinaryFiles(fileEntries);
                AssetDatabase.Refresh();
            }

        }

        private static ExcelFileEntry ConvertExcel(string filePath) {
            FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
            if (!excelReader.IsValid) {
                Debug.Log($"Excel {filePath} read failed");
                return null;
            }

            ExcelFileEntry fileEntry = new ExcelFileEntry();
            fileEntry.className = excelReader.Name;
            fileEntry.datas = new List<string[]>();


            int line = 1;
            while (excelReader.Read()) {
                string[] lineDatas = new string[excelReader.FieldCount];
                for (int j = 0; j < lineDatas.Length; j++) {
                    lineDatas[j] = excelReader.GetString(j);
                }
                if (line == 1) {
                    fileEntry.propertyNames = lineDatas;
                }
                if (line == 2) {
                    fileEntry.propertyTypes = lineDatas;
                }
                if (line == 3) {
                    fileEntry.propertyDescriptions = lineDatas;
                }
                if (line >= 4) {
                    fileEntry.datas.Add(lineDatas);
                }
                line++;
            }
            return fileEntry;
        }

        private static void GenerateBinaryFiles(List<ExcelFileEntry> fileEntries) {
            for (int i = 0; i < fileEntries.Count; i++) {
                ExcelFileEntry fileEntry = fileEntries[i];
                List<byte> byteList = new List<byte>();

                int dataCount = fileEntry.datas.Count;
                byteList.AddRange(ExcelData.WriteInt(dataCount));

                int tempIndex = 0;
                while (tempIndex < dataCount) {
                    string[] data = fileEntry.datas[tempIndex];
                    for (int j = 0; j < fileEntry.propertyTypes.Length; j++) {
                        byte[] typeBytes = GetBytes(fileEntry.propertyTypes[j], data[j]);
                        byteList.AddRange(typeBytes);
                    }
                    tempIndex++;
                }
                string savePath = $"{ExcelConfigManager.EXCEL_BINARY_PATH}/{fileEntry.className}.bytes";
                if (!Directory.Exists(ExcelConfigManager.EXCEL_BINARY_PATH)) {
                    Directory.CreateDirectory(ExcelConfigManager.EXCEL_BINARY_PATH);
                }
                File.WriteAllBytes(savePath, byteList.ToArray());

                if (!Directory.Exists(ExcelConfigManager.EXCEL_SCRIPTS_PATH)) {
                    Directory.CreateDirectory(ExcelConfigManager.EXCEL_SCRIPTS_PATH);
                }
                string code = GenerateScripts(fileEntry.className, fileEntry.propertyNames, fileEntry.propertyTypes, fileEntry.propertyDescriptions);
                File.WriteAllText($"{ExcelConfigManager.EXCEL_SCRIPTS_PATH}/Config{fileEntry.className}.cs", code);
            }
        }

        private static string GenerateScripts(string className, string[] propertyNames, string[] propertyTypes, string[] propertyDescriptions) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("using UnityEngine;\n");
            stringBuilder.Append("using System;\n");
            stringBuilder.Append("\n");

            stringBuilder.Append("namespace IndieFramework {\n");
            stringBuilder.Append($"\tpublic class Config{className} : ExcelData\n");
            stringBuilder.Append("{\n");

            for (int i = 0; i < propertyNames.Length; i++) {
                stringBuilder.Append(CreateNotes(propertyDescriptions[i], 2));
                stringBuilder.Append($"\t\tpublic {propertyTypes[i]} {propertyNames[i]};\n");
            }

            stringBuilder.Append("\n");
            stringBuilder.Append("\t\tpublic override void ReadData(byte[] datas, ref int index){\n");

            for (int i = 0; i < propertyNames.Length; i++) {
                string readInfo = "";
                switch (propertyTypes[i]) {
                    case "int":
                        readInfo = "ExcelData.ReadInt";
                        break;
                    case "float":
                        readInfo = "ExcelData.ReadFloat";
                        break;
                    case "string":
                        readInfo = "ExcelData.ReadString";
                        break;
                    case "string[]":
                        readInfo = "ExcelData.ReadStringArray";
                        break;
                    default:
                        break;
                }
                stringBuilder.Append($"\t\t\t{propertyNames[i]} = {readInfo}(datas, ref index);\n");
            }
            stringBuilder.Append("\t\t}\n");
            stringBuilder.Append("\t}\n");
            stringBuilder.Append("}\n");

            return stringBuilder.ToString();
        }

        private static string CreateNotes(string note, int t = 0) {
            StringBuilder stringBuilder = new StringBuilder();
            string str = "";
            for (int i = 0; i < t; i++) {
                str += "\t";
            }
            stringBuilder.Append($"{str}//{note}\n");
            return stringBuilder.ToString();
        }

        private static object GetValue(string typeName, string data) {
            string[] splitStr = data.Split('|');
            switch (typeName) {
                case "int":
                    return int.Parse(data);
                case "float":
                    return float.Parse(data);
                case "string":
                    return data;
                case "string[]":
                    return splitStr;
            }
            return null;
        }

        private static byte[] GetBytes(string typeName, string data) {
            List<byte> bytes = new List<byte>();
            object obj = GetValue(typeName, data);
            switch (typeName) {
                case "int":
                    bytes.AddRange(ExcelData.WriteInt((int)obj));
                    break;
                case "float":
                    bytes.AddRange(ExcelData.WriteFloat((float)obj));
                    break;
                case "string":
                    bytes.AddRange(ExcelData.WriteString((string)obj));
                    break;
                case "string[]":
                    bytes.AddRange(ExcelData.WriteStringArray((string[])obj));
                    break;
                default:
                    break;
            }
            return bytes.ToArray();
        }

    }
}