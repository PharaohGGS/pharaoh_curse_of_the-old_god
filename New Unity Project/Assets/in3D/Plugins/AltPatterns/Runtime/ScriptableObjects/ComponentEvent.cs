using System;
using UnityEngine;

namespace Alteracia.Patterns.ScriptableObjects
{
    public interface IComponentEvents<T> where T : Component
    {
        T Last { get; }
        Action<T> OnEvent { get; set; }
        void GetComponent(GameObject go);
        void AddComponent(GameObject go);
    }

    public abstract class ComponentEvent<T> : NestedScriptableObject, ISubscribableEvent, IComponentEvents<T> where T : Component
    {
        [NonSerialized] private Action<T> _onEvent;

        [NonSerialized] private T _last;
        public T Last => _last;
        
        private object _temporalLast;
        public string AssetName => this.name;

        public object TemporalLast
        {
            get => _temporalLast;
            set => _temporalLast = value;
        }
        
        public Action<T> OnEvent
        {
            get => _onEvent;
            set => _onEvent = value;
        }

        protected ComponentEvent()
        {
            _onEvent += component => _last = component;
        }
        
        public void GetComponent(GameObject go)
        {
            var comp = go.GetComponent<T>();
            _onEvent?.Invoke(comp);
        }

        public void AddComponent(GameObject go)
        {
            var comp = go.AddComponent<T>();
            _onEvent?.Invoke(comp);
        }

        public bool Equals(ISubscribableEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            ComponentEvent<T> otherEvent = other as ComponentEvent<T>;
            return otherEvent && String.Equals(this.name, otherEvent.name, StringComparison.CurrentCultureIgnoreCase);
        }

        public void SubscribeTo(ISubscribableEvent other)
        {
            this.OnEvent += passed =>
            {
                if (passed == null || passed.Equals(other.TemporalLast))
                {
                    _temporalLast = null;
                    other.TemporalLast = null;
                    return;
                }

                _temporalLast = passed;
                ComponentEvent<T> otherObjectEvent = (ComponentEvent<T>)other;
                otherObjectEvent.OnEvent?.Invoke(passed);
            };
        }
    }
}