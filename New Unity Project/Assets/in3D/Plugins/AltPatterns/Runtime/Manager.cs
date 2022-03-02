using UnityEngine;

namespace Alteracia.Patterns
{
    public abstract class Manager<T> : MonoBehaviour where T : Manager<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Init();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance) Destroy(this.gameObject);
            
            _instance = this as T;
        }

        private static void Init()
        {
            GameObject manager = new GameObject(typeof(T) + "_Manager");
            manager.AddComponent<T>();
        }
    }
}
