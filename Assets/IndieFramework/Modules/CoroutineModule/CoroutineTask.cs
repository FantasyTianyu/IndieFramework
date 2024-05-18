using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class CoroutineTask {
        public IEnumerator Coroutine { get; private set; }

        public CoroutineTask(IEnumerator coroutine) {
            Coroutine = coroutine;
        }
    }
}