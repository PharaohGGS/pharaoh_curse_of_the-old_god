using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
    {
        private DetectionComponent _detection;

        protected override void OnStart()
        {
            if (_detection || agent.TryGetComponent(out _detection)) return;
            LogHandler.SendMessage($"Can't attack ennemies !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            if (!_detection) return NodeState.Failure;
            var target = blackboard.GetData<Transform>("target").position;
            return _detection.OverlapPoint(target) ? NodeState.Success : NodeState.Failure;
        }
    }
}