using System;
using System.Collections.Generic;
using System.Linq;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class DetectionComponent : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] private Collider2D detectionCollider;
        [SerializeField] private LayerMask whatIsTarget;
        [SerializeField, Range(1, 20)] private int overlapCount = 8;
        
        private Collider2D[] _colliders;

        public bool canDetect { get; private set; } = false;
        public int overlappedCount { get; private set; } = 0;
        
        private void OnEnable()
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

        private void FixedUpdate()
        {
            if (!canDetect) return;

            // do the overlap operation each fixedUpdate
            for (int i = 0; i < _colliders.Length; i++) _colliders[i] = null;
            overlappedCount = detectionCollider.OverlapNonAlloc(ref _colliders, whatIsTarget);
        }

        public bool TryGetByLayerMask(LayerMask layer, out GameObject obj)
        {
            obj = GetByLayerMask(layer);
            return obj != null;
        }

        public GameObject GetByLayerMask(LayerMask layer)
        {
            if (overlappedCount <= 0) return null;

            // check if the mask as multiple layers
            var layerIndexes = layer.GetLayerIndexes();
            if (layerIndexes.Length > 1)
            {
                LogHandler.SendMessage($"Too much layers for just one gameObject", MessageType.Error);
                return null;
            }

            foreach (var coll in _colliders)
            {
                if (!coll || !coll.gameObject.activeInHierarchy || !coll.gameObject.HasLayer(layer)) continue;
                return coll.gameObject;
            }

            return null;
        }

        public bool TryGetByIndex(int index, out GameObject obj)
        {
            obj = GetByIndex(index);
            return obj != null;
        }

        public GameObject GetByIndex(int index) => overlappedCount <= 0 ? null : _colliders[index]?.gameObject;

        public bool OverlapPoint(Vector2 point) => detectionCollider && detectionCollider.OverlapPoint(point);
    }
}