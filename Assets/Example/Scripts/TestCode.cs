using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieFramework;

public class TestCode : MonoBehaviour {
    // Start is called before the first frame update
    async void Start() {
        await ResLoader.InitializeAsync();
        var cube = ResLoader.LoadAsset<GameObject>("Assets/BundleAssets/TestPackByFile/Cube.prefab");
        if (cube != null) {
            Instantiate(cube);
            Log.LogInfo("cube crate success");
        }

    }

    // Update is called once per frame
    void Update() {

    }
}
