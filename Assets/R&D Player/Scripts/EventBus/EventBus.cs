using System;
using System.Collections.Generic;
using UnityEngine;


public struct GameEvent
{
    static GameEvent e;

    public string EventName;
    public int IntParameter;
    public Vector2 Vector2Parameter;
    public Vector3 Vector3Parameter;
    public bool BoolParameter;
    public string StringParameter;

    public static void Trigger(string eventName, int intParameter = 0, Vector2 vector2Parameter = default(Vector2), Vector3 vector3Parameter = default(Vector3), bool boolParameter = false, string stringParameter = "")
    {
        e.EventName = eventName;
        e.IntParameter = intParameter;
        e.Vector2Parameter = vector2Parameter;
        e.Vector3Parameter = vector3Parameter;
        e.BoolParameter = boolParameter;
        e.StringParameter = stringParameter;
        EventBus.TriggerEvent(e);
    }
}

[ExecuteAlways]
public static class EventBus
{
    private static Dictionary<Type, List<EventListenerBase>> _subscribersList;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitializeStatics()
    {
        _subscribersList = new Dictionary<Type, List<EventListenerBase>>();
    }

    static EventBus()
    {
        _subscribersList = new Dictionary<Type, List<EventListenerBase>>();
    }

    /// <summary>
    /// Adds a new subscriber to a certain event.
    /// </summary>
    /// <param name="listener">listener.</param>
    /// <typeparam name="Event">The event type.</typeparam>
    public static void AddListener<Event>(EventListener<Event> listener) where Event : struct
    {
        Type eventType = typeof(Event);

        if (!_subscribersList.ContainsKey(eventType))
        {
            _subscribersList[eventType] = new List<EventListenerBase>();
        }

        if (!SubscriptionExists(eventType, listener))
        {
            _subscribersList[eventType].Add(listener);
        }
    }

    /// <summary>
    /// Removes a subscriber from a certain event.
    /// </summary>
    /// <param name="listener">listener.</param>
    /// <typeparam name="Event">The event type.</typeparam>
    public static void RemoveListener<Event>(EventListener<Event> listener) where Event : struct
    {
        Type eventType = typeof(Event);

        if (!_subscribersList.ContainsKey(eventType))
        {
            #if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
            #else
            return;
            #endif
        }

        List<EventListenerBase> subscriberList = _subscribersList[eventType];

        #if EVENTROUTER_THROWEXCEPTIONS
	            bool listenerFound = false;
        #endif

        for (int i = subscriberList.Count - 1; i >= 0; i--)
        {
            if (subscriberList[i] == listener)
            {
                subscriberList.Remove(subscriberList[i]);
            #if EVENTROUTER_THROWEXCEPTIONS
					    listenerFound = true;
            #endif

                if (subscriberList.Count == 0)
                {
                    _subscribersList.Remove(eventType);
                }

                return;
            }
        }

            #if EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
            #endif
    }

    /// <summary>
    /// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
    /// </summary>
    /// <param name="newEvent">The event to trigger.</param>
    /// <typeparam name="Event">The 1st type parameter.</typeparam>
    public static void TriggerEvent<Event>(Event newEvent) where Event : struct
    {
        List<EventListenerBase> list;
        if (!_subscribersList.TryGetValue(typeof(Event), out list))
            #if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( Event ).ToString() ) );
            #else
            return;
            #endif

        for (int i = list.Count - 1; i >= 0; i--)
        {
            (list[i] as EventListener<Event>).OnEvent(newEvent);
        }
    }

    /// <summary>
    /// Checks if there are subscribers for a certain type of events
    /// </summary>
    /// <returns><c>true</c>, if exists was subscriptioned, <c>false</c> otherwise.</returns>
    /// <param name="type">Type.</param>
    /// <param name="receiver">Receiver.</param>
    private static bool SubscriptionExists(Type type, EventListenerBase receiver)
    {
        List<EventListenerBase> receivers;

        if (!_subscribersList.TryGetValue(type, out receivers)) return false;

        bool exists = false;

        for (int i = receivers.Count - 1; i >= 0; i--)
        {
            if (receivers[i] == receiver)
            {
                exists = true;
                break;
            }
        }

        return exists;
    }
}

public static class EventRegister
{
    public delegate void Delegate<T>(T eventType);

    public static void EventStartListening<EventType>(this EventListener<EventType> caller) where EventType : struct
    {
        EventBus.AddListener<EventType>(caller);
    }

    public static void EventStopListening<EventType>(this EventListener<EventType> caller) where EventType : struct
    {
        EventBus.RemoveListener<EventType>(caller);
    }
}

/// <summary>
/// Event listener basic interface
/// </summary>
public interface EventListenerBase { };

/// <summary>
/// A public interface you'll need to implement for each type of event you want to listen to.
/// </summary>
public interface EventListener<T> : EventListenerBase
{
    void OnEvent(T eventType);
}