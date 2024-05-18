using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IndieFramework {
    public class WaitForTask : CustomYieldInstruction {
        private readonly Task _task;

        public override bool keepWaiting {
            get { return !_task.IsCompleted; }
        }

        public WaitForTask(Func<Task> taskFunc) {
            _task = taskFunc.Invoke();
        }

        public WaitForTask(Task task) {
            _task = task;
        }
    }
}
