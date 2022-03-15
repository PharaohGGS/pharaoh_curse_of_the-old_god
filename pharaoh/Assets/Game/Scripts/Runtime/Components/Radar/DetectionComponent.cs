using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GenericDictionary<int, List<Collider2D>> layeredColliders2D;

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
                if (overlappedCount <= 0)
                {
                    // clear registered colliders
                    foreach (var kvp in layeredColliders2D) kvp.Value.Clear();
                    return;
                }
                
                foreach (var kvp in layeredColliders2D)
                {
                    kvp.Value?.RemoveAll(coll => !colliders2D.Contains(coll));
                }

                foreach (var coll in colliders2D)
                {
                    // skip detection component object
                    if (coll == null || coll.gameObject == gameObject) continue;
                    // add object in the proper list layered
                    var list = layeredColliders2D[coll.gameObject.layer];
                    if (list == null || list.Contains(coll)) continue;
                    // add collider to the list if it doesn't contains it already
                    list.Add(coll);
                }
            }
            else
            {
                overlappedCount = collider3D.OverlapNonAlloc(ref colliders3D, detectionLayer);
                if (overlappedCount <= 0)
                {
                    // clear registered colliders
                    foreach (var kvp in layeredColliders3D) kvp.Value.Clear();
                    return;
                }

                foreach (var kvp in layeredColliders3D)
                {
                    kvp.Value?.RemoveAll(coll => !colliders3D.Contains(coll));
                }

                foreach (var coll in colliders3D)
                {
                    // skip detection component object
                    if (coll == null || coll.gameObject == gameObject) continue;
                    // add object in the proper list layered
                    var list = layeredColliders3D[coll.gameObject.layer];
                    if (list == null || list.Contains(coll)) continue;
                    // add collider to the list if it doesn't contains it already
                    list.Add(coll);
                }
            }
        }

        public GameObject GetGameObjectWithLayer(LayerMask mask)
        {
            if (overlappedCount <= 0) return null;

            // check if the mask as colliders
            foreach (int layerIndex in mask.HasLayerIndexes())
            {
                if (_is2D)
                {
                    if (!layeredColliders2D.ContainsKey(layerIndex)) return null;
                
                    // return the corresponding collider in index
                    foreach (var coll in layeredColliders2D[layerIndex])
                    {
                        // only get the active object different from detector
                        var gameObjectWithLayer = coll.gameObject;
                        if (!gameObjectWithLayer.activeInHierarchy || gameObjectWithLayer == gameObject) continue;
                        foreach (var coll2D in colliders2D)
                        {
                            if (coll2D.gameObject != gameObjectWithLayer) continue;
                            return gameObjectWithLayer;
                        }
                    }
                }
                else
                {
                    // check if the mask as colliders
                    if (!layeredColliders3D.ContainsKey(layerIndex)) return null;
                
                    // return the corresponding collider in index
                    foreach (var coll in layeredColliders3D[layerIndex])
                    {
                        // only get the active object different from detector
                        var gameObjectWithLayer = coll.gameObject;
                        if (!gameObjectWithLayer.activeInHierarchy || gameObjectWithLayer == gameObject) continue;
                        foreach (var coll3D in colliders3D)
                        {
                            if (coll3D.gameObject != gameObjectWithLayer) continue;
                            return gameObjectWithLayer;
                        }
                    }
                }
            }

            return null;
        }

        public GameObject GetGameObjectAtIndex(int index) =>
            overlappedCount <= 0 ? null : (_is2D ? colliders2D[index].gameObject : colliders3D[index].gameObject);
    }
}