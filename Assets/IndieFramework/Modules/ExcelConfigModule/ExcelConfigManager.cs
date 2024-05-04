using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IndieFramework {
    public class ExcelConfigManager {
        public static string EXCEL_BINARY_PATH => Path.Combine(Application.dataPath, "BundleAssets/ExcelConfigs");
        public static string EXCEL_SCRIPTS_PATH => Path.Combine(Application.dataPath, "Project/Scripts/ExcelConfigs");
    }
}