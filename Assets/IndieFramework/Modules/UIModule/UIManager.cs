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
        /// <summary>
        /// 异步加载窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> LoadWindowAsync<T>() where T : UIWindow {
            if (!cachedWindow.TryGetValue(typeof(T), out UIWindow win)) {
                GameObject prefab = await ResLoader.LoadWindowAsync(typeof(T).Name);
                prefab.SetActive(false);
                GameObject winObject = Instantiate(prefab, UIRoot);

                T component = winObject.GetComponent<T>();
                cachedWindow.Add(typeof(T), component);
                return component;
            }
            return (T)win;
        }

        /// <summary>
        /// 加载窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadWindow<T>() where T : UIWindow {
            if (!cachedWindow.TryGetValue(typeof(T), out UIWindow win)) {
                GameObject prefab = ResLoader.LoadWindow(typeof(T).Name);
                prefab.SetActive(false);
                GameObject winObject = Instantiate(prefab, UIRoot);
                T component = winObject.GetComponent<T>();
                cachedWindow.Add(typeof(T), component);
                return component;
            }
            return (T)win;
        }

        /// <summary>
        /// 异步加载窗口并显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> OpenWindowAsync<T>() where T : UIWindow {
            T win = await LoadWindowAsync<T>();
            if (win != null) {
                win.Show();
            }
            return win;
        }

        /// <summary>
        /// 加载窗口并显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenWindow<T>() where T : UIWindow {
            T win = LoadWindow<T>();
            if (win != null) {
                win.Show();
            }
            return win;
        }

        /// <summary>
        /// 关闭指定类型的窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseWindow<T>() where T : UIWindow {
            if (cachedWindow.TryGetValue(typeof(T), out UIWindow win)) {
                win.Hide();
            }
        }

        /// <summary>
        /// 移除类型为T的窗口实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ClearWindow<T>() where T : UIWindow {
            if (cachedWindow.TryGetValue(typeof(T), out UIWindow win)) {
                if (win != null) {
                    if (win.gameObject.activeInHierarchy) {
                        // 调整排序只有当窗口是可见的
                        SetWindowLayer(win);
                    }
                    Destroy(win.gameObject);
                }
                cachedWindow.Remove(typeof(T));
            }
        }

        public void SortWindow(UIWindow win) {
            if (!topSortingLayers.TryGetValue(win.windowLayer, out int currentMaxOrder)) {
                currentMaxOrder = 0;  // 如果没有，从0开始
            }

            int newOrder = currentMaxOrder + 500;  // 使用更小的值以保持排序紧凑
            win.SetSortingOrder(newOrder);  // 设置新窗口的排序顺序
            topSortingLayers[win.windowLayer] = newOrder;  // 更新最大排序值
        }

        public void SetWindowLayer(UIWindow win) {
            // 窗口被隐藏了，我们需要重新计算并设置当前层的最高排序顺序
            int maxOrderInLayer = 0;
            foreach (var existingWin in cachedWindow.Values) {
                // 过滤掉非激活状态的窗口和不在同一层的窗口
                if (existingWin != win && existingWin.gameObject.activeInHierarchy && existingWin.windowLayer == win.windowLayer) {
                    int windowMaxOrder = existingWin.GetMaxSortingOrder();
                    if (windowMaxOrder > maxOrderInLayer) {
                        maxOrderInLayer = windowMaxOrder;
                    }
                }
            }
            // 更新当前窗口层的最高排序顺序
            topSortingLayers[win.windowLayer] = maxOrderInLayer;
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