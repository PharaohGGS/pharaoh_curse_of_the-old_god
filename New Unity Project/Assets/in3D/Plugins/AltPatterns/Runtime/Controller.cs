using UnityEngine;

namespace Alteracia.Patterns
{
    public abstract class Controller<T> : MonoBehaviour where T : Controller<T>
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogError(typeof(T) + " instance not initiated");
                return _instance;
            }
        }

        public static bool Initialized => _instance != null;

        private void Awake()
        {
            if (_instance)
            {
                Debug.LogWarning("Instantiate second Controller of type " + typeof(T));
            }
            _instance = this as T;
            
            this.Init();
        }

        /// <summary>
        /// Calls on Awake
        /// Do NOT subscribe here use Start instead
        /// </summary>
        protected virtual void Init(){}
        
    }
}
