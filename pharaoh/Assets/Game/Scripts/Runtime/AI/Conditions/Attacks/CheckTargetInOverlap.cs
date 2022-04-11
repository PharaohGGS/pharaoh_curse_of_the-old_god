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
        private DetectionComponent _detection = null;
        [SerializeField] private LayerMask mask;

        protected override void OnStart()
        {
            if (_detection) return;

            if (agent.TryGetComponent(out _detection)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't detect enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_detection || !_detection.canDetect ||
                !_detection.TryGetByLayerMask(mask, out GameObject target))
            {
                return NodeState.Failure;
            }
            
            blackboard.SetData("target", target.transform);
            return NodeState.Success;
        }
    }
}