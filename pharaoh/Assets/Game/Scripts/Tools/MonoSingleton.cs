using System;
using UnityEngine;

namespace Pharaoh.Tools
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public bool dontDestroyOnLoad = false;
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new SystemException("An instance of this MonoSingleton already exist.");
            }
            else
            {
                Instance = (T)this;
            }

            if (Instance != null && dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}