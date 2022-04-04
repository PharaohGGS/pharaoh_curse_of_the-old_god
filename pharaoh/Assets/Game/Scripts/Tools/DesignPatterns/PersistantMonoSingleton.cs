using UnityEngine;

namespace DesignPatterns
{
    public class PersistantMonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject
                    {
                        name = nameof(T),
                        hideFlags = HideFlags.HideAndDontSave
                    };

                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}