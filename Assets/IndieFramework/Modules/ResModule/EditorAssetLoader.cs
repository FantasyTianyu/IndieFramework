using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace IndieFramework {
    public class EditorAssetLoader {
#if UNITY_EDITOR
        public static async Task<T> LoadAssetAsync<T>(string assetPath, int delay = 100) where T : UnityEngine.Object {
            await Task.Delay(delay); // 模拟异步加载耗时
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
#endif
    }
}