using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// This is useful for certain methods that need to be called lots of times in multiple scripts.
// Potentially more could utilise the event bus later on if the script doesn't require information from another script anyway
// E.g. DisplayTutorial needs only values for its parameters + doesn't need to reference the TutorialManager for those values,
// but PlaySFXAtPlayer needs a reference to the SoundManager to get the clip info anyway, so might as well use the script reference
// if we have it
public static class WVDEventBus 
{
    private static Dictionary<Type, Delegate> _assignedActions = new Dictionary<Type, Delegate>();

    public static void Raise(WVDEventData data)
    {
        Type type = data.GetType();
        if (_assignedActions.TryGetValue(type, out Delegate exisitingAction))
        {
            Debug.Log($"Raised event {type} and found {exisitingAction.GetInvocationList().Length} actions to execute");
            exisitingAction?.DynamicInvoke(data);
        }
    }

    public static void Subscribe<T>(Action<T> action) where T : WVDEventData
    {
        Type type = typeof(T);
        if (_assignedActions.ContainsKey(type))
        {
            _assignedActions[type] = Delegate.Combine(_assignedActions[type], action);
        }
        else
        {
            _assignedActions[type] = action;
        }
    }

    public static void Unsubscribe<T>(Action<T> action) where T : WVDEventData
    {
        Type type = typeof(T);
        if (_assignedActions.ContainsKey(type))
        {
            _assignedActions[type] = Delegate.Remove(_assignedActions[type], action);
        }
    }

    public enum EventType
    {
        LevelComplete
    }
}
