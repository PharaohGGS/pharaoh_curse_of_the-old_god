using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public abstract class Detection<T> : MonoBehaviour
        where T : Collider2D
    {
        [Header("Settings")] [SerializeField] protected T detectionCollider;
        [SerializeField] protected LayerMask whatIsTarget;
        [SerializeField] protected LayerMask whatIsObstacle;
        [SerializeField, Range(1, 20)] protected int overlapCount = 3;
        protected Collider2D[] _colliders;

        public bool canDetect { get; protected set; } = false;
        public int overlappedCount { get; protected set; } = 0;

        protected virtual void OnEnable()
        {
            if (canDetect) return;

            canDetect = false;

            if (detectionCollider)
            {
                _colliders = new Collider2D[overlapCount];
                canDetect = true;
                return;
            }

            LogHandler.SendMessage($"No detection collider on this agent.", MessageType.Warning);
        }

        protected virtual void FixedUpdate()
        {
            if (!canDetect) return;

            // do the overlap operation each fixedUpdate
            for (int i = 0; i < _colliders.Length; i++) _colliders[i] = null;
            overlappedCount = detectionCollider.OverlapNonAlloc(ref _colliders, whatIsTarget);
        }

    }
}