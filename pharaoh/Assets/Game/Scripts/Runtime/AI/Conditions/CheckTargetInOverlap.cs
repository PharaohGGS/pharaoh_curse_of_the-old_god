using System;
using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using MessageType = Pharaoh.Tools.Debug.MessageType;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInOverlap : ActionNode
    {
        [Tooltip("layer of the overlaping collider to be found on agent child")]
        public LayerMask colliderLayer;
        
        [Tooltip("layer of detection for the targets")]
        public LayerMask detectionLayer;

        public bool is2D { get; private set; }
        [HideInInspector] public Collider[] colliders3D;
        [HideInInspector] public Collider2D[] colliders2D;

        private Collider _collider3D;
        private Collider2D _collider2D;
        
        protected override void OnStart()
        {
            if (_collider3D || _collider2D) return;

            foreach (Transform child in agent.transform)
            {
                if (!child.gameObject.IsInLayerMask(colliderLayer)) continue;

                if (child.TryGetComponent(out _collider3D))
                {
                    colliders3D = new Collider[8];
                    return;
                }

                if (child.TryGetComponent(out _collider2D))
                {
                    colliders2D = new Collider2D[8];
                    is2D = true;
                    return;
                }
            }

            LogHandler.SendMessage($"No collider on this agent.", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Failure;
            if (!_collider3D && !_collider2D) return state;

            int size;
            int index = 0;

            if (is2D)
            {
                size = _collider2D.OverlapNonAlloc(ref colliders2D, detectionLayer);
                if (colliders2D[0] && colliders2D[0].transform == agent.transform) index++;
            }
            else
            {
                size = _collider3D.OverlapNonAlloc(ref colliders3D, detectionLayer);
                if (colliders3D[0] && colliders3D[0].transform == agent.transform) index++;
            }

            if (size <= index)
            {
                blackboard.ClearData("target");
                state = NodeState.Running;
                return state;
            }
            
            blackboard.SetData("target", colliders3D[index].transform);
            state = NodeState.Success;
            return state;
        }
    }
}