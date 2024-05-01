using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IndieFramework {
    public interface IBuildStrategy {
        AssetBundleBuild[] GetBundlesToBuild(IEnumerable<AssetBundleBuildRule> rules);
        //void SetBundlesToBuild(IEnumerable<AssetBundleBuildRule> rules);
    }
}