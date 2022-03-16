
namespace Pharaoh.GameEvents
{
    public interface IGameEventListener
    {
        public void OnEnable();
        public void OnDisable();

        public void OnEventRaised();
    }

    public interface IGameEventListener<in T>
    {
        public void OnEnable();
        public void OnDisable();

        public void OnEventRaised(T item);
    }

    public interface IGameEventListener<in T, in U>
    {
        public void OnEnable();
        public void OnDisable();

        public void OnEventRaised(T item1, U item2);
    }

    public interface IGameEventListener<in T, in U, in V>
    {
        public void OnEnable();
        public void OnDisable();

        public void OnEventRaised(T item1, U item2, V item3);
    }
}