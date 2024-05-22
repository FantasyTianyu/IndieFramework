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
            Log.LogInfo("cube create success");
        }

        UIManager.Instance.LoadWindow<UITestWindow>();

        Coroutine timerCoroutine = TimerService.StartTimer(1f, PrintCurrentTime, true, true);
    }

    private void PrintCurrentTime() {
        // 获取当前系统时间并打印
        System.DateTime now = System.DateTime.Now;
        Debug.Log(now.ToString("HH:mm:ss"));
    }

    // Update is called once per frame
    void Update() {

    }
}
