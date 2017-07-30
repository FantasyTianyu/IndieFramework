using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

struct EventCall
{
    public string eventName;
    public Delegate call;

    public EventCall(string eventName, Delegate call)
    {
        this.eventName = eventName;
        this.call = call;
    }
}

/// <summary>
/// 事件分发器
/// </summary>
public class EventDispatcher
{
    private static List<EventCall> eventList = new List<EventCall>();

    public static void AddEventListener(string eventName, Action callBack)
    {
        EventCall observer = new EventCall(eventName, callBack);
        eventList.Add(observer);
    }

    public static void AddEventListener<T1>(string eventName, Action<T1> callBack)
    {
        EventCall observer = new EventCall(eventName, callBack);
        eventList.Add(observer);
    }

    public static void AddEventListener<T1,T2>(string eventName, Action<T1,T2> callBack)
    {
        EventCall observer = new EventCall(eventName, callBack);
        eventList.Add(observer);
    }

    public static void AddEventListener<T1,T2,T3>(string eventName, Action<T1,T2,T3> callBack)
    {
        EventCall observer = new EventCall(eventName, callBack);
        eventList.Add(observer);
    }

    public static void AddEventListener<T1,T2,T3,T4>(string eventName, Action<T1,T2,T3,T4> callBack)
    {
        EventCall observer = new EventCall(eventName, callBack);
        eventList.Add(observer);
    }

    public static void DispatchEvent(string eventName)
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventName == eventName)
            {
                Action action = eventList[i].call as Action;
                action();
            }
        }
    }

    public static void DispatchEvent<T1>(string eventName, T1 t1)
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventName == eventName)
            {
                Action<T1> action = eventList[i].call as Action<T1>;
                action(t1);
            }
        }
    }

    public static void DispatchEvent<T1,T2>(string eventName, T1 t1, T2 t2)
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventName == eventName)
            {
                Action<T1,T2> action = eventList[i].call as Action<T1,T2>;
                action(t1, t2);
            }
        }
    }

    public static void DispatchEvent<T1,T2,T3>(string eventName, T1 t1, T2 t2, T3 t3)
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventName == eventName)
            {
                Action<T1,T2,T3> action = eventList[i].call as Action<T1,T2,T3>;
                action(t1, t2, t3);
            }
        }
    }

    public static void DispatchEvent<T1,T2,T3,T4>(string eventName, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventName == eventName)
            {
                Action<T1,T2,T3,T4> action = eventList[i].call as Action<T1,T2,T3,T4>;
                action(t1, t2, t3, t4);
            }
        }
    }

    public static void RemoveEventListener(string eventName)
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventName == eventName)
            {
                eventList.Remove(eventList[i]);
            }
        }
    }

}
