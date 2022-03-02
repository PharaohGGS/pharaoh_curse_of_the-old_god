using System;

namespace Alteracia.Patterns.ScriptableObjects
{
    public interface ISubscribableEvent
    {
        string AssetName { get; }
        object TemporalLast { get; set; }
        bool Equals(ISubscribableEvent other);
        void SubscribeTo(ISubscribableEvent other);
    }
    
    public interface IObjectEvent<T>
    {
        T Last { get; }
        Action<T> OnEvent { get; set; }
    }

    public abstract class ObjectEvent<T> : NestedScriptableObject, IObjectEvent<T>, ISubscribableEvent
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
                ObjectEvent<T> otherObjectEvent = (ObjectEvent<T>)other;
                otherObjectEvent.OnEvent?.Invoke(passed);
            };
        }

        public Action<T> OnEvent
        {
            get => _onEvent;
            set => _onEvent = value;
        }

        protected ObjectEvent()
        {
            _onEvent += obj => _last = obj;
        }
        
        public bool Equals(ISubscribableEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            ObjectEvent<T> otherEvent = other as ObjectEvent<T>;
            return otherEvent != null && String.Equals(this.name, otherEvent.name, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}