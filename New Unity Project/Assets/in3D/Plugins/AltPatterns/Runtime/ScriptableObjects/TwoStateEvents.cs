using System;

namespace Alteracia.Patterns.ScriptableObjects
{
    public interface ITwoStateEvent<T>
    {
        bool? IsSecondary { get; }
        T LastPrimary { get; }
        T LastSecondary { get; }
        Action<T> OnPrimaryEvent { get; set; }
        Action<T> OnSecondaryEvent { get; set; }
    }

    /// <summary>
    /// Events of two state
    /// </summary>
    public abstract class TwoStateEvents<T> : NestedScriptableObject, ITwoStateEvent<T>, ISubscribableEvent
    {
        [NonSerialized] private bool? _state = null;
        public bool? IsSecondary => _state;

        [NonSerialized] private T _lastPrimary;
        public T LastPrimary => _lastPrimary;
        
        [NonSerialized] private T _lastSecondary;
        public T LastSecondary => _lastSecondary;

        [NonSerialized] private Action<T> _onPrimaryEvent;
        public Action<T> OnPrimaryEvent
        {
            get => _onPrimaryEvent;
            set => _onPrimaryEvent = value;
        }
        
        [NonSerialized] private Action<T> _onSecondaryEvent;
        public Action<T> OnSecondaryEvent
        {
            get => _onSecondaryEvent;
            set => _onSecondaryEvent = value;
        }
        
        private object _temporalLast;
        public string AssetName => this.name;

        public object TemporalLast
        {
            get => _temporalLast;
            set => _temporalLast = value;
        }
        
        private object _temporalSecondaryLast;
        public object TemporalSecondaryLast
        {
            get => _temporalSecondaryLast;
            set => _temporalSecondaryLast = value;
        }
        
        protected TwoStateEvents()
        {
            _onPrimaryEvent += obj =>
            {
                _lastPrimary = obj;
                _state = true;
            };
            _onSecondaryEvent += obj =>
            {
                _lastSecondary = obj;
                _state = false;
            };
        }

        public bool Equals(ISubscribableEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true; // not true!!
            TwoStateEvents<T> otherEvent = other as TwoStateEvents<T>;
            return otherEvent && 
                   String.Equals(this.name, otherEvent.name, StringComparison.CurrentCultureIgnoreCase);
        }

        public void SubscribeTo(ISubscribableEvent other)
        {
            TwoStateEvents<T> otherObjectEvent = (TwoStateEvents<T>)other;
            this.OnPrimaryEvent += passed =>
            {
                if (passed == null || passed.Equals(other.TemporalLast))
                {
                    _temporalLast = null;
                    other.TemporalLast = null;
                    return;
                }

                _temporalLast = passed;
                otherObjectEvent.OnPrimaryEvent?.Invoke(passed);
            };
            this.OnSecondaryEvent += passed =>
            {
                if (passed == null || passed.Equals(otherObjectEvent.TemporalSecondaryLast))
                {
                    _temporalSecondaryLast = null;
                    otherObjectEvent.TemporalSecondaryLast = null;
                    return;
                }

                _temporalSecondaryLast = passed;
                otherObjectEvent.OnSecondaryEvent?.Invoke(passed);
            };
        }
    }
}