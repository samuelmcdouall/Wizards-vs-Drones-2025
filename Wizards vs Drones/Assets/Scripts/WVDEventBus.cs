using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class WVDEventBus
{
    private static Dictionary<EventType, Action> _assignedActions = new Dictionary<EventType, Action>();

    public static void Raise(EventType eventType)
    {
        if (_assignedActions.TryGetValue(eventType, out Action exisitingAction))
        {
            Debug.Log($"Raised event {eventType} and found {exisitingAction.GetInvocationList().Length} actions to execute");
            exisitingAction?.Invoke();
        }
    }

    public static void Subscribe(EventType eventType, Action action)
    {
        if (_assignedActions.ContainsKey(eventType))
        {
            _assignedActions[eventType] += action;
        }
        else
        {
            _assignedActions[eventType] = action;
        }
    }

    public static void Unsubscribe(EventType eventType, Action action)
    {
        if (_assignedActions.ContainsKey(eventType))
        {
            _assignedActions[eventType] -= action;
        }
    }

    public enum EventType
    {
        LevelComplete
    }
}
