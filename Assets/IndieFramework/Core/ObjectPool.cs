using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class ObjectPool<T> where T : IPoolable, new() {
        private readonly ConcurrentStack<T> availableObjects = new ConcurrentStack<T>();
        private int maxSize;

        public ObjectPool(int initialSize = 10, int maxSize = 100) {
            this.maxSize = maxSize;
            for (int i = 0; i < initialSize; i++) {
                CreateObject();
            }
        }

        public T Get() {
            T obj;
            if (!availableObjects.TryPop(out obj)) {
                obj = new T();
                obj.OnCreate();
            }
            obj.OnGet();
            return obj;
        }

        public void Release(T obj) {
            if (availableObjects.Count < maxSize) {
                obj.OnRelease();
                availableObjects.Push(obj);
            } else {
                obj.OnDestroy();
            }
        }

        private void CreateObject() {
            T obj = new T();
            obj.OnCreate();
            availableObjects.Push(obj);
        }

        public void Clear() {
            T obj;
            while (availableObjects.TryPop(out obj)) {
                obj.OnDestroy();
            }
        }
    }
}