using System;
using System.Collections.Generic;

public class EventBus
{
    private static EventBus _instance;
    public static EventBus Instance => _instance ??= new EventBus();

    private Dictionary<Type, List<Action<object>>> _eventListeners = new();

    public void Subscribe<T>(Action<T> listener) where T : class
    {
        Type eventType = typeof(T);
        if (!_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType] = new List<Action<object>>();
        }

        _eventListeners[eventType].Add((obj) => listener(obj as T));
    }

    public void Unsubscribe<T>(Action<T> listener) where T : class
    {
        Type eventType = typeof(T);
        if (_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType].Remove((obj) => listener(obj as T));
        }
    }

    public void Publish<T>(T eventData) where T : class
    {
        Type eventType = typeof(T);
        if (_eventListeners.ContainsKey(eventType))
        {
            // Cria uma cópia da lista de ouvintes para evitar erros de modificação durante a iteração
            var listeners = new List<Action<object>>(_eventListeners[eventType]);

            foreach (var listener in listeners)
            {
                listener(eventData);
            }
        }
    }
}