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
        /// �첽���ش���
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
        /// ���ش���
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
        /// �첽���ش��ڲ���ʾ
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
        /// ���ش��ڲ���ʾ
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
        /// �ر�ָ�����͵Ĵ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseWindow<T>() where T : UIWindow {
            if (cachedWindow.TryGetValue(typeof(T), out UIWindow win)) {
                win.Hide();
            }
        }

        /// <summary>
        /// �Ƴ�����ΪT�Ĵ���ʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ClearWindow<T>() where T : UIWindow {
            if (cachedWindow.TryGetValue(typeof(T), out UIWindow win)) {
                if (win != null) {
                    if (win.gameObject.activeInHierarchy) {
                        // ��������ֻ�е������ǿɼ���
                        SetWindowLayer(win);
                    }
                    Destroy(win.gameObject);
                }
                cachedWindow.Remove(typeof(T));
            }
        }

        public void SortWindow(UIWindow win) {
            if (!topSortingLayers.TryGetValue(win.windowLayer, out int currentMaxOrder)) {
                currentMaxOrder = 0;  // ���û�У���0��ʼ
            }

            int newOrder = currentMaxOrder + 500;  // ʹ�ø�С��ֵ�Ա����������
            win.SetSortingOrder(newOrder);  // �����´��ڵ�����˳��
            topSortingLayers[win.windowLayer] = newOrder;  // �����������ֵ
        }

        public void SetWindowLayer(UIWindow win) {
            // ���ڱ������ˣ�������Ҫ���¼��㲢���õ�ǰ����������˳��
            int maxOrderInLayer = 0;
            foreach (var existingWin in cachedWindow.Values) {
                // ���˵��Ǽ���״̬�Ĵ��ںͲ���ͬһ��Ĵ���
                if (existingWin != win && existingWin.gameObject.activeInHierarchy && existingWin.windowLayer == win.windowLayer) {
                    int windowMaxOrder = existingWin.GetMaxSortingOrder();
                    if (windowMaxOrder > maxOrderInLayer) {
                        maxOrderInLayer = windowMaxOrder;
                    }
                }
            }
            // ���µ�ǰ���ڲ���������˳��
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