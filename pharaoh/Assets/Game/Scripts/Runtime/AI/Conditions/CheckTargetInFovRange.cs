using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInFovRange : ActionNode
    {
        [SerializeField] private Collider[] colliders;
        
        private DetectionComponent _detection = null;
        
        protected override void OnStart()
        {
            colliders = new Collider[8];

            if (_detection) return;

            if (agent.TryGetComponent(out _detection)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't detect enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_detection)
            {
                state = NodeState.Failure;
                return state;
            }

            if (blackboard.TryGetData("target", out Transform t))
            {
                state = NodeState.Success;
                return state;
            }

            var size = Physics.OverlapSphereNonAlloc(
                agent.transform.position, _detection.fovRange,
                colliders, _detection.detectionLayer);

            int index = 0;
            if (colliders[0]?.transform == agent.transform) index++;

            if (size <= index)
            {
                state = NodeState.Failure;
                return state;
            }

            blackboard.SetData("target", colliders[index].transform);

            state = NodeState.Success;
            return state;
        }
    }
}