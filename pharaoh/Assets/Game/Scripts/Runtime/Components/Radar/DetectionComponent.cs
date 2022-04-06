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
        [SerializeField] private DetectionData[] datas;
        [SerializeField] private new Collider2D collider;

        [SerializeField] private int maxOverlappedColliders = 8;
        [SerializeField] private Collider2D[] colliders;
        private Dictionary<int, List<Collider2D>> _layeredColliders;
        private LayerMask detectionLayer;

        public bool hasDetectionCollider { get; private set; } = false;
        public int overlappedCount { get; private set; } = 0;


        private void OnEnable()
        {
            if (hasDetectionCollider) return;

            hasDetectionCollider = false;
            detectionLayer = datas.CombineLayers();

            if (collider)
            {
                colliders = new Collider2D[maxOverlappedColliders];

                // get all the layer in detection layer and setup dictionary
                _layeredColliders = new Dictionary<int, List<Collider2D>>();
                foreach (var index in datas.GetLayerIndexes())
                {
                    _layeredColliders.TryAdd(index, new List<Collider2D>());
                }

                hasDetectionCollider = true;
                return;
            }

            LogHandler.SendMessage($"No detection collider on this agent.", MessageType.Warning);
        }

        private void FixedUpdate()
        {
            if (!hasDetectionCollider) return;

            // do the overlap operation each fixedUpdate
            overlappedCount = collider.OverlapNonAlloc(ref colliders, detectionLayer);
            if (overlappedCount <= 0)
            {
                // clear registered colliders
                foreach (var kvp in _layeredColliders) kvp.Value.Clear();
                return;
            }
            
            foreach (var kvp in _layeredColliders)
            {
                kvp.Value?.RemoveAll(coll => !colliders.Contains(coll));
            }

            foreach (var coll in colliders)
            {
                // skip detection component object
                if (coll == null || coll.gameObject == gameObject) continue;
                // add object in the proper list layered
                if (!_layeredColliders.TryGetValue(coll.gameObject.layer, out var list) || list.Contains(coll)) continue;
                // add collider to the list if it doesn't contains it already
                list.Add(coll);
            }
        }

        public GameObject GetGameObjectWithLayer(DetectionData data)
        {
            if (overlappedCount <= 0) return null;

            // check if the mask as colliders
            var index = data.GetLayerIndex();
            if (!_layeredColliders.ContainsKey(index)) return null;
            
            // return the corresponding collider in index
            foreach (var coll in _layeredColliders[index])
            {
                // only get the active object different from detector
                var gameObjectWithLayer = coll.gameObject;
                if (!gameObjectWithLayer.activeInHierarchy || gameObjectWithLayer == gameObject) continue;
                
                if (colliders.Any(coll2D => coll2D.gameObject == gameObjectWithLayer))
                {
                    return gameObjectWithLayer;
                }
            }

            return null;
        }

        public GameObject GetGameObjectAtIndex(int index) => overlappedCount <= 0 ? null : colliders[index].gameObject;
    }
}