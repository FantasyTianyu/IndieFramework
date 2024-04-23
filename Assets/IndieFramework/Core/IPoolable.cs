using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public interface IPoolable {
        void OnCreate();
        void OnGet();
        void OnRelease();
        void OnDestroy();
    }
}