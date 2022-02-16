using System;

namespace Pharaoh.Gameplay.Components.Events
{
    public interface IGameEventListener
    {
        public void OnEnable();
        public void OnDisable();
    }

    public interface IGameEventListener<in T> : IGameEventListener
    {
        public void OnEventRaised(T item);
    }

    public interface IGameEventListener<in T, in U> : IGameEventListener
    {
        public void OnEventRaised(T item1, U item2);
    }

    public interface IGameEventListener<in T, in U, in V> : IGameEventListener
    {
        public void OnEventRaised(T item1, U item2, V item3);
    }
}