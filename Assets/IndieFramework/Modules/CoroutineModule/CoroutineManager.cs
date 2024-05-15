using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class CoroutineManager : MonoSingleton<CoroutineManager> {
        public Coroutine Start_Coroutine(IEnumerator coroutine) {
            return Instance.StartCoroutine(coroutine);
        }

        public void Stop_Coroutine(Coroutine coroutine) {
            Instance.StopCoroutine(coroutine);
        }
    }
}
