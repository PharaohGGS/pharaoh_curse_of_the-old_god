using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    public abstract class AbstractGameEvent : ScriptableObject
    {
        private readonly List<IGameEventListener> _listeners = new List<IGameEventListener>();
        
        public void Raise() 
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) 
            {
                _listeners[i]?.OnEventRaised(); 
            }
        }

        public void RegisterListener(IGameEventListener listener) 
        {
            if (!_listeners.Contains(listener))
            {
                _listeners?.Add(listener);
            }
        }

        public void UnregisterListener(IGameEventListener listener) 
        {
            if (_listeners.Contains(listener))
            {
                _listeners?.Remove(listener);
            }
        }
    }

    public abstract class AbstractGameEvent<T> : ScriptableObject 
    {
        private readonly List<IGameEventListener<T>> _listeners = new List<IGameEventListener<T>>();
        
        public void Raise(T item) 
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) 
            {
                _listeners[i]?.OnEventRaised(item); 
            }
        }

        public void RegisterListener(IGameEventListener<T> listener) 
        {
            if (!_listeners.Contains(listener))
            {
                _listeners?.Add(listener);
            }
        }

        public void UnregisterListener(IGameEventListener<T> listener) 
        {
            if (_listeners.Contains(listener))
            {
                _listeners?.Remove(listener);
            }
        }
    }
    
    public abstract class AbstractGameEvent<T, U> : ScriptableObject 
    {
        private readonly List<IGameEventListener<T, U>> _listeners = new List<IGameEventListener<T, U>>();
        
        public void Raise(T item1, U item2) 
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) 
            {
                _listeners[i]?.OnEventRaised(item1, item2); 
            }
        }

        public void RegisterListener(IGameEventListener<T, U> listener) 
        {
            if (!_listeners.Contains(listener))
            {
                _listeners?.Add(listener);
            }
        }

        public void UnregisterListener(IGameEventListener<T, U> listener) 
        {
            if (_listeners.Contains(listener))
            {
                _listeners?.Remove(listener);
            }
        }
    }
    
    public abstract class AbstractGameEvent<T, U, V> : ScriptableObject 
    {
        private readonly List<IGameEventListener<T, U, V>> _listeners = new List<IGameEventListener<T, U, V>>();
        
        public void Raise(T item1, U item2, V item3) 
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) 
            {
                _listeners[i]?.OnEventRaised(item1, item2, item3); 
            }
        }

        public void RegisterListener(IGameEventListener<T, U, V> listener) 
        {
            if (!_listeners.Contains(listener))
            {
                _listeners?.Add(listener);
            }
        }

        public void UnregisterListener(IGameEventListener<T, U, V> listener) 
        {
            if (_listeners.Contains(listener))
            {
                _listeners?.Remove(listener);
            }
        }
    }
}