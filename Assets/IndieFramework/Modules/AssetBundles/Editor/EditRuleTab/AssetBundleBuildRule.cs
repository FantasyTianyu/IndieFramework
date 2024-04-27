using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 打包方式枚举
public enum PackMode {
    PackByFile,
    PackByDirectory,
    PackTogether,
}

// 打包配置规则类
[System.Serializable]
public class AssetBundleBuildRule {
    public bool foldout;
    public string assetBundleVariant; // AssetBundle的后缀
    public string destinationPath;          // 配置的目录
    public PackMode packMode;         // 打包方式

    // 再添加额外所需字段
}
