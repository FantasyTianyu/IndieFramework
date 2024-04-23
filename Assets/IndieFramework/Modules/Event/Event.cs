using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public class EventData : IPoolable {
        public string EventType;
        private Dictionary<string, object> _parameters;

        public EventData() {

        }

        public EventData(string eventType) {
            EventType = eventType;
            _parameters = new Dictionary<string, object>();
        }

        public void AddParameter(string key, object value) {
            _parameters[key] = value;
        }

        public T GetParameter<T>(string key) {
            if (_parameters.TryGetValue(key, out object value)) {
                return (T)value;
            }

            throw new ArgumentException("Parameter with key not found", key);
        }

        public void OnCreate() {

        }

        public void OnDestroy() {

        }

        public void OnGet() {

        }

        public void OnRelease() {

        }

        public void Reset() {
            EventType = string.Empty;
            _parameters.Clear();
        }
    }
}