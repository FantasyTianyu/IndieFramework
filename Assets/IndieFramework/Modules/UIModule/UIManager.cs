using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IndieFramework {
    public class UIManager : MonoSingleton<UIManager> {
        public Camera UICamera;
        public Transform UIRoot;

        private Dictionary<EUIWindowLayer, int> topSortingLayers = new Dictionary<EUIWindowLayer, int>();

        private List<UIWindow> windowList = new List<UIWindow>();

        private Dictionary<System.Type, UIWindow> cachedWindow = new Dictionary<System.Type, UIWindow>();
        protected override void Awake() {
            base.Awake();
            for (int i = 0; i < 5; i++) {
                topSortingLayers.Add((EUIWindowLayer)i, 0);
            }
        }

        public async Task<T> LoadWindowAsync<T>() where T : UIWindow {
            if (cachedWindow.ContainsKey(typeof(T))) {
                T win = (T)cachedWindow[typeof(T)];
                win.gameObject.SetActive(true);
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
                T win = (T)cachedWindow[typeof(T)];
                win.gameObject.SetActive(true);
                return (T)cachedWindow[typeof(T)];
            } else {
                GameObject prefab = ResLoader.LoadWindow(typeof(T).Name);
                GameObject winObject = Instantiate(prefab, UIRoot);
                T win = winObject.GetComponent<T>();
                return win;
            }

        }

        public void AddWindow(UIWindow win) {
            int curTopOrder;
            topSortingLayers.TryGetValue(win.windowLayer, out curTopOrder);
            int winMaxSortingOrder = win.SetSortingOrder(curTopOrder + 500);
            if (winMaxSortingOrder > curTopOrder) {
                topSortingLayers[win.windowLayer] = winMaxSortingOrder;
            }
            windowList.Add(win);
        }

        public void RemoveWindow(UIWindow win) {
            windowList.Remove(win);
            topSortingLayers[win.windowLayer] = topSortingLayers[win.windowLayer] - win.GetMaxSortingOrder() - 500;
        }

        public void ClearAllWindows() {
            for (int i = 0; i < windowList.Count; i++) {
                if (windowList[i].gameObject != null) {
                    Destroy(windowList[i].gameObject);
                }
            }
            windowList.Clear();
            cachedWindow.Clear();
            for (int i = 0; i < 5; i++) {
                topSortingLayers[(EUIWindowLayer)i] = 0;
            }
        }
    }
}