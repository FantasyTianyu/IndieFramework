using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        public bool dontDestroyOnLoad = false;
        private static T instance;

        private static readonly object instanceLock = new object();

        public static T Instance {
            get {
                lock (instanceLock) {
                    if (instance == null) {
                        instance = FindObjectOfType<T>();

                        if (instance == null) {
                            GameObject monoGameObject = new GameObject();
                            monoGameObject.name = typeof(T).ToString();
                            instance = monoGameObject.AddComponent<T>();
                        }
                    }
                    return instance;
                }
            }
        }
        protected virtual void Awake() {
            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}