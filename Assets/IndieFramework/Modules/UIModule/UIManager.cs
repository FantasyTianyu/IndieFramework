using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IndieFramework {
    public class UIManager : MonoSingleton<UIManager> {
        public Camera UICamera;
        public Transform UIRoot;

        private Dictionary<EUIWindowLayer, int> topSortingLayers = new Dictionary<EUIWindowLayer, int>();

        private Dictionary<System.Type, UIWindow> cachedWindow = new Dictionary<System.Type, UIWindow>();
        protected override void Awake() {
            base.Awake();
            for (int i = 0; i < 5; i++) {
                topSortingLayers.Add((EUIWindowLayer)i, 0);
            }
        }

        public async Task<T> LoadWindowAsync<T>() where T : UIWindow {
            if (cachedWindow.ContainsKey(typeof(T))) {
                //T win = (T)cachedWindow[typeof(T)];
                //win.gameObject.SetActive(true);
                return (T)cachedWindow[typeof(T)];
            } else {
                GameObject prefab = await ResLoader.LoadWindowAsync(typeof(T).Name);
                GameObject winObject = Instantiate(prefab, UIRoot);
                T win = winObject.GetComponent<T>();
                cachedWindow.Add(typeof(T), win);
                return win;
            }

        }

        public T LoadWindow<T>() where T : UIWindow {
            if (cachedWindow.ContainsKey(typeof(T))) {
                //T win = (T)cachedWindow[typeof(T)];
                //win.gameObject.SetActive(true);
                return (T)cachedWindow[typeof(T)];
            } else {
                GameObject prefab = ResLoader.LoadWindow(typeof(T).Name);
                GameObject winObject = Instantiate(prefab, UIRoot);
                T win = winObject.GetComponent<T>();
                return win;
            }

        }

        public async Task<T> OpenWindowAsync<T>() where T : UIWindow {
            T win = await LoadWindowAsync<T>();
            if (win != null) {
                win.Show();
            }
            return win;
        }

        public T OpenWindow<T>() where T : UIWindow {
            T win = LoadWindow<T>();
            if (win != null) {
                win.Show();
            }
            return win;
        }

        // 关闭指定类型的窗口
        public void CloseWindow<T>() where T : UIWindow {
            if (cachedWindow.TryGetValue(typeof(T), out UIWindow win)) {
                win.Hide();
            }
        }

        public void SortWindow(UIWindow win) {
            int curTopOrder;
            topSortingLayers.TryGetValue(win.windowLayer, out curTopOrder);
            int winMaxSortingOrder = win.SetSortingOrder(curTopOrder + 500);
            if (winMaxSortingOrder > curTopOrder) {
                topSortingLayers[win.windowLayer] = winMaxSortingOrder;
            }
        }

        public void SetWindowLayer(UIWindow win) {
            topSortingLayers[win.windowLayer] = topSortingLayers[win.windowLayer] - win.GetMaxSortingOrder() - 500;
        }

        public void ClearAllWindows() {
            foreach (var item in cachedWindow) {
                if (item.Value.gameObject != null) {
                    Destroy(item.Value.gameObject);
                }

            }
            cachedWindow.Clear();
            for (int i = 0; i < 5; i++) {
                topSortingLayers[(EUIWindowLayer)i] = 0;
            }
        }
    }
}