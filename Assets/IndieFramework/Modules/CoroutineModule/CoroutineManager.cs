using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IndieFramework {
    public class CoroutineManager : MonoSingleton<CoroutineManager> {
        public Coroutine Execute(IEnumerator coroutine) {
            return Instance.StartCoroutine(coroutine);
        }

        public void Stop(Coroutine coroutine) {
            Instance.StopCoroutine(coroutine);
        }

        public void ExecuteSequentially(List<CoroutineTask> tasks, Action onAllComplete = null) {
            StartCoroutine(SequentialCoroutine(tasks, onAllComplete));
        }

        private IEnumerator SequentialCoroutine(List<CoroutineTask> tasks, Action onAllComplete) {
            foreach (var task in tasks) {
                yield return StartCoroutine(task.Coroutine);
            }
            onAllComplete?.Invoke();
        }

        public void ExecuteInParallel(List<CoroutineTask> tasks, Action onAllComplete = null) {
            int count = tasks.Count;

            foreach (var task in tasks) {
                StartCoroutine(ParallelCoroutine(task, () => {
                    count--;
                    if (count == 0) {
                        onAllComplete?.Invoke();
                    }
                }));
            }
        }

        private IEnumerator ParallelCoroutine(CoroutineTask task, Action onTaskComplete) {
            yield return StartCoroutine(task.Coroutine);
            onTaskComplete?.Invoke();
        }

        public Task ExecuteAsync(IEnumerator coroutine) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            StartCoroutine(WaitForCoroutine(coroutine, result => tcs.SetResult(result)));
            return tcs.Task;
        }

        private IEnumerator WaitForCoroutine(IEnumerator coroutine, Action<bool> onComplete) {
            yield return StartCoroutine(coroutine);
            onComplete?.Invoke(true);
        }

    }
}
