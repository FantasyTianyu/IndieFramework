using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class TimerService {
        public static Coroutine StartTimer(float delay, Action callback, bool isLoop = false, bool realtime = false) {
            return CoroutineManager.Instance.Execute(realtime ? RealtimeDelay(delay, callback, isLoop) : GameTimeDelay(delay, callback, isLoop));
        }

        public static void StopTimer(Coroutine coroutine) {
            CoroutineManager.Instance.Stop(coroutine);
        }

        private static IEnumerator GameTimeDelay(float delay, Action callback, bool loop) {
            do {
                yield return new WaitForSeconds(delay);
                callback?.Invoke();
            } while (loop);
        }

        private static IEnumerator RealtimeDelay(float delay, Action callback, bool loop) {
            do {
                yield return new WaitForSecondsRealtime(delay);
                callback?.Invoke();
            } while (loop);
        }
    }
}