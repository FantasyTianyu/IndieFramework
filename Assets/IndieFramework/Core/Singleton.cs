using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class Singleton<T> where T : class, new() {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => new T(),
                                                                   System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        public static T Instance => lazyInstance.Value;

        protected Singleton() { }
    }
}