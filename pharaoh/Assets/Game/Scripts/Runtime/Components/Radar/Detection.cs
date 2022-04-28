using System;
using System.Collections.Generic;
using System.Linq;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public abstract class Detection<T> : MonoBehaviour
        where T : Collider2D
    {
        [Header("Settings")] [SerializeField] protected T detectionCollider;
        [SerializeField] protected LayerMask whatIsTarget;
        [SerializeField] protected LayerMask whatIsObstacle;
        [SerializeField, Range(1, 20)] protected int overlapCount = 3;
        protected Collider2D[] _overlapColliders;
        protected readonly List<Collider2D> _colliders = new List<Collider2D>();
        protected readonly List<Collider2D> _removables = new List<Collider2D>();

        public UnityAction<Collider2D> onOverlapEnter;
        public UnityAction<Collider2D> onOverlapExit;

        public bool canDetect { get; protected set; } = false;
        public int overlappedCount => _colliders.Count;

        protected virtual void OnEnable()
        {
            onOverlapEnter += OnOverlapEnter;
            onOverlapExit += OnOverlapExit;
            
            if (canDetect) return;

            canDetect = false;

            if (detectionCollider)
            {
                _overlapColliders = new Collider2D[overlapCount];
                canDetect = true;
                return;
            }

            LogHandler.SendMessage($"No detection collider on this agent.", MessageType.Warning);
        }

        private void OnDisable()
        {
            onOverlapEnter -= OnOverlapEnter;
            onOverlapExit -= OnOverlapExit;
        }

        protected virtual void FixedUpdate()
        {
            if (!canDetect) return;

            // do the overlap operation each fixedUpdate
            for (int i = 0; i < _overlapColliders.Length; i++) _overlapColliders[i] = null;
            var size = detectionCollider.OverlapNonAlloc(ref _overlapColliders, whatIsTarget);

            // search the collider not in the overlap anymore
            foreach (var stockedCollider in _colliders)
            {
                if (_overlapColliders.Contains(stockedCollider)) continue;
                _removables.Add(stockedCollider);
            }
            // delete those not in the overlap anymore, invoke the overlapExit event
            foreach (var removableCollider in _removables)
            {
                _colliders.Remove(removableCollider);
                onOverlapExit?.Invoke(removableCollider);
            }
            _removables.Clear();
            // add the new one in the list, invoke the overlapEnter event
            foreach (var overlapCollider in _overlapColliders)
            {
                if (!overlapCollider || _colliders.Contains(overlapCollider)) continue;
                _colliders.Add(overlapCollider);
                onOverlapEnter?.Invoke(overlapCollider);
            }
        }

        public virtual void Reset()
        {
            for (int i = 0; i < _overlapColliders.Length; i++) _overlapColliders[i] = null;
            _colliders.Clear();
            _removables.Clear();
        }

        protected virtual void OnOverlapEnter(Collider2D other) {}

        protected virtual void OnOverlapExit(Collider2D other) {}
    }
}