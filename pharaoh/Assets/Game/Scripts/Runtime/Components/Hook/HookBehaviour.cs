using Pharaoh.GameEvents;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(HookReleaseGameEventListener))]
    public abstract class HookBehaviour<T0> : MonoBehaviour
        where T0 : HookCapacity
    {
        [SerializeField, Tooltip("FX hookIndicator for the best target selected")] 
        protected GameObject hookIndicator;

        public virtual void FoundBestTarget(TargetFinder finder, GameObject go)
        {
            hookIndicator?.SetActive(go == gameObject);
        }

        protected virtual void Awake()
        {
            hookIndicator?.SetActive(false);
        }

        public abstract void Release(HookCapacity capacity);
        public abstract void Begin(T0 capacity, HookBehaviour<T0> behaviour);
    }
}