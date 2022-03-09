using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInFovRange : ActionNode
    {
        [HideInInspector] public bool is2D;
        [HideInInspector] public Collider[] colliders3D;
        [HideInInspector] public Collider2D[] colliders2D;
        
        private DetectionComponent _detection = null;
        private Collider _collider3D;
        private Collider2D _collider2D;

        protected override void OnStart()
        {
            if (_detection) return;

            if (!agent.TryGetComponent(out _detection))
            {
                LogHandler.SendMessage($"[{agent.name}] Can't detect enemies", MessageType.Warning);
                return;
            }

            if (agent.TryGetComponent(out _collider3D))
            {
                colliders3D = new Collider[8];
                return;
            }

            if (agent.TryGetComponent(out _collider2D))
            {
                colliders2D = new Collider2D[8];
                is2D = true;
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_detection) return NodeState.Failure;

            if (blackboard.TryGetData("target", out Transform t)) return NodeState.Success;

            int index = 0;
            int size = 0;

            if (is2D)
            {
                size = Physics2D.OverlapCircleNonAlloc(agent.transform.position, 
                    _detection.fovRange, colliders2D, _detection.detectionLayer);

                if (colliders2D[0] && colliders2D[0].transform == agent.transform) index++;
            }
            else
            {
                size = Physics.OverlapSphereNonAlloc(agent.transform.position, 
                    _detection.fovRange, colliders3D, _detection.detectionLayer);

                if (colliders2D[0] && colliders2D[0].transform == agent.transform) index++;
            }
            
            if (size <= index) return NodeState.Failure;

            blackboard.SetData("target", is2D ? colliders2D[index].transform : colliders3D[index].transform);
            return NodeState.Success;
        }
    }
}