using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class GameObjectPool<T> where T : Component {
        private GameObject prefab;
        private readonly Stack<T> availableObjects = new Stack<T>();
        private int maxSize;

        public GameObjectPool(GameObject prefab, int initialSize = 10, int maxSize = 100) {
            this.prefab = prefab;
            this.maxSize = maxSize;
            for (int i = 0; i < initialSize; i++) {
                CreateObject();
            }
        }

        public T Get() {
            if (availableObjects.Count == 0) {
                CreateObject();
            }
            T obj = availableObjects.Pop();
            obj.gameObject.SetActive(true); // ����GameObject
            return obj;
        }

        public void Release(T obj) {
            if (availableObjects.Count < maxSize) {
                obj.gameObject.SetActive(false); // ����GameObject
                availableObjects.Push(obj);
            } else {
                GameObject.Destroy(obj.gameObject); // ���ٳ��������������Ϸ����
            }
        }

        private void CreateObject() {
            var newObj = GameObject.Instantiate(prefab).GetComponent<T>();
            newObj.gameObject.SetActive(false); // Ĭ��Ϊ����״̬
            availableObjects.Push(newObj);
        }

        public void Clear() {
            while (availableObjects.Count > 0) {
                GameObject.Destroy(availableObjects.Pop().gameObject);
            }
        }
    }
}