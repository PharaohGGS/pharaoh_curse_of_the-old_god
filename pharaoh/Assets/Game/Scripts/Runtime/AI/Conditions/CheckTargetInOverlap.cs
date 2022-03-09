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

        protected override void OnStart()
        {
            if (_detection) return;

            if (agent.TryGetComponent(out _detection)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't detect enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_detection || !_detection.hasDetectionCollider) return NodeState.Failure;
            
            int index = 0;
            GameObject potentialTarget = _detection.GetGameObjectAtIndex(0);
            
            // index up if agent is the first collider
            if (potentialTarget != null && potentialTarget == agent.gameObject) index++;

            // if the count is equal to the index, it's possibly the agent, then clear
            if (_detection.overlappedCount <= index)
            {
                blackboard.ClearData("target");
                return NodeState.Failure;
            }
            
            blackboard.SetData("target", _detection.GetGameObjectAtIndex(index).transform);
            return NodeState.Success;
        }
    }
}