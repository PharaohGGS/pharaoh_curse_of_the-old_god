namespace Alteracia.Editor
{
    public abstract class AltInspector<T> : UnityEditor.Editor where T : class
    {
        protected T instance;

        private void OnEnable()
        {
            instance = target as T;
            this.Initiate();
        }

        protected virtual void Initiate() { }
    }
}
