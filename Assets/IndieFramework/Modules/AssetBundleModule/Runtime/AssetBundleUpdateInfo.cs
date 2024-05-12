using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    [Serializable]
    public class AssetBundleVersionInfo {
        public string Version;
        public List<AssetBundleVersionEntry> assetBundleList;

        [Serializable]
        public class AssetBundleVersionEntry {
            public string Name;
            public string Hash;
            public string Url;
        }
    }
}
