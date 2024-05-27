using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndieFramework {
    public enum EUIWindowLayer {
        VeryLow,
        Low,
        Normal,
        High,
        VeryHigh
    }
    public class UIWindow : UIComponent {
        public EUIWindowLayer windowLayer = EUIWindowLayer.Normal;
        public bool showMask = false;

        protected Canvas canvas;
        private int maxOrder = 0;
        private void Awake() {
            canvas = GetComponent<Canvas>();
            if (canvas == null) {
                Debug.LogError($"window {gameObject.name} has no canvas!");
            }
            canvas.worldCamera = UIManager.Instance.UICamera;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingLayerName = windowLayer.ToString();

            if (showMask) {
                GameObject mask = new GameObject("WindowMask");
                mask.transform.parent = transform;
                mask.transform.localPosition = Vector3.zero;
                Image maskImage = mask.AddComponent<Image>();
                maskImage.color = new Color(0, 0, 0, 0.5f);
                maskImage.transform.SetSiblingIndex(0);
                mask.layer = LayerMask.NameToLayer("UI");

            }
            Initialize();

        }

        protected virtual void OnEnable() {
            UIManager.Instance.SortWindow(this);
        }

        protected virtual void OnDisable() {
            UIManager.Instance.SetWindowLayer(this);
        }

        public int SetSortingOrder(int startOrder) {
            Canvas[] childCanvases = GetComponentsInChildren<Canvas>();
            maxOrder = 0;
            for (int i = 0; i < childCanvases.Length; i++) {
                childCanvases[i].sortingLayerID = canvas.sortingLayerID;
                childCanvases[i].sortingOrder = startOrder + childCanvases[i].sortingOrder;
                if (childCanvases[i].sortingOrder > maxOrder) {
                    maxOrder = childCanvases[i].sortingOrder;
                }
            }
            return maxOrder;
        }

        public int GetMaxSortingOrder() {
            return maxOrder;
        }

        public void Show() {
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide() {
            gameObject.SetActive(false);
            OnHide();
        }
        protected virtual void Initialize() {
            // 窗口初始化代码。
        }

        protected virtual void OnShow() {

        }

        protected virtual void OnHide() {

        }
    }
}