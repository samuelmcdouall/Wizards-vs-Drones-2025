using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WVDEventBus
{
    private static Dictionary<EventNames, Delegate> events = new Dictionary<EventNames, Delegate>();

    public static void Subscribe<T>(EventNames eventName, Action<T> listener)
    {
        if (!events.ContainsKey(eventName))
        {
            events[eventName] = null;
        }

        events[eventName] = (Action<T>)events[eventName] + listener;
    }

    public static void Unsubscribe<T>(EventNames eventName, Action<T> listener)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] = (Action<T>)events[eventName] - listener;
        }

    }

    public static void Publish<T>(EventNames eventName, T data)
    {
        if (events.ContainsKey(eventName))
        {
            ((Action<T>)events[eventName])?.Invoke(data);
        }
    }

    public enum EventNames
    {
        LevelComplete
    }
}
