using System;
using System.Collections.Generic;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [ExecuteAlways]
    public class DetectionComponent : MonoBehaviour
    {
        [SerializeField] private bool _is2D;
        [SerializeField] private LayerMask detectionLayer = 0;
        [SerializeField] private int maxOverlappedColliders = 8;
        public bool hasDetectionCollider { get; private set; } = false;
        public int overlappedCount { get; private set; } = 0;

        #region 2D Detection

        [HideInInspector, SerializeField] private new Collider2D collider2D;
        [HideInInspector, SerializeField] private Collider2D[] colliders2D;
        [HideInInspector, SerializeField] private GenericDictionary<int, List<Collider2D>> layeredColliders2D;

        #endregion

        #region 3D Detection

        [HideInInspector, SerializeField] private Collider collider3D;
        [HideInInspector, SerializeField] private Collider[] colliders3D;
        [HideInInspector, SerializeField] private GenericDictionary<int, List<Collider>> layeredColliders3D;

        #endregion

        #region Debug
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(transform.position, fovRange);
        }

#endif

        #endregion

        private void OnEnable()
        {
            if (hasDetectionCollider) return;

            hasDetectionCollider = false;
            
            switch (_is2D)
            {
                case false when collider3D:
                {
                    colliders3D = new Collider[maxOverlappedColliders];

                    // get all the layer in detection layer and setup dictionary
                    var hasLayers = detectionLayer.HasLayers();
                    layeredColliders3D = new GenericDictionary<int, List<Collider>>();
                    for (var index = 0; index < hasLayers.Length; index++)
                    {
                        if (!hasLayers[index]) continue;
                        layeredColliders3D.TryAdd(index, new List<Collider>());
                    }

                    hasDetectionCollider = true;
                    _is2D = false;
                    return;
                }
                case true when collider2D:
                {
                    colliders2D = new Collider2D[maxOverlappedColliders];

                    // get all the layer in detection layer and setup dictionary
                    var hasLayers = detectionLayer.HasLayers();
                    layeredColliders2D = new GenericDictionary<int, List<Collider2D>>();
                    for (var index = 0; index < hasLayers.Length; index++)
                    {
                        if (!hasLayers[index]) continue;
                        layeredColliders2D.TryAdd(index, new List<Collider2D>());
                    }

                    hasDetectionCollider = true;
                    _is2D = true;
                    return;
                }
                default:
                    LogHandler.SendMessage($"No detection collider on this agent.", MessageType.Warning);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!hasDetectionCollider) return;

            // do the overlap operation each fixedUpdate
            if (_is2D)
            {
                overlappedCount = collider2D.OverlapNonAlloc(ref colliders2D, detectionLayer);
                if (overlappedCount <= 0) return;

                // clear registered colliders
                foreach (var kvp in layeredColliders2D) kvp.Value.Clear();
                for (var i = 0; i < colliders2D.Length; i++)
                {
                    var coll = colliders2D[i];
                    if (coll == null || coll.gameObject == gameObject) continue;
                    var layerIndex = coll.gameObject.layer;
                    var list = layeredColliders2D[layerIndex];
                    if (list != null && !list.Contains(coll)) list.Add(coll);
                }
            }
            else
            {
                overlappedCount = collider3D.OverlapNonAlloc(ref colliders3D, detectionLayer);
                if (overlappedCount <= 0) return;

                // clear registered colliders
                foreach (var kvp in layeredColliders3D) kvp.Value.Clear();
                for (var i = 0; i < colliders3D.Length; i++)
                {
                    var coll = colliders3D[i];
                    var layerIndex = coll.gameObject.layer;
                    var list = layeredColliders3D[layerIndex];
                    if (list != null && !list.Contains(coll)) list.Add(coll);
                }
            }
        }
        
        public GameObject GetGameObjectAtIndex(int index) =>
            overlappedCount <= 0 ? null : (_is2D ? colliders2D[index].gameObject : colliders3D[index].gameObject);
    }
}