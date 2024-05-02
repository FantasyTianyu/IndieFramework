using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����ʽö��
public enum PackMode {
    PackByFile,
    PackByDirectory,
    PackTogether,
}

// ������ù�����
[System.Serializable]
public class AssetBundleBuildRule {
    public bool foldout;
    public string assetBundleVariant; // AssetBundle�ĺ�׺
    public string destinationPath;          // ���õ�Ŀ¼
    public PackMode packMode;         // �����ʽ

    // ����Ӷ��������ֶ�
}
