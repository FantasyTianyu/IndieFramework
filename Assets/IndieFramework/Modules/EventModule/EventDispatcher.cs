using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IndieFramework {
    public class EventDispatcher {
        private readonly ObjectPool<EventData> eventPool;
        private readonly Dictionary<string, Action<EventData>> subscribers = new Dictionary<string, Action<EventData>>();

        public EventDispatcher() {
            eventPool = new ObjectPool<EventData>(2, 10);
        }

        public void Subscribe(string eventType, Action<EventData> subscriber) {
            if (!subscribers.ContainsKey(eventType)) {
                subscribers[eventType] = delegate { };
            }
            subscribers[eventType] += subscriber;
        }

        public void Unsubscribe(string eventType, Action<EventData> subscriber) {
            if (subscribers.ContainsKey(eventType)) {
                subscribers[eventType] -= subscriber;
            }
        }

        public void Dispatch(string eventType, Dictionary<string, object> parameters = null) {
            if (subscribers.TryGetValue(eventType, out var handler)) {
                var eventData = eventPool.Get() ?? new EventData(eventType);
                eventData.Reset();
                eventData.EventType = eventType;

                if (parameters != null) {
                    foreach (var param in parameters) {
                        eventData.AddParameter(param.Key, param.Value);
                    }
                }

                handler(eventData);
                eventPool.Release(eventData);
            }
        }
    }
}
