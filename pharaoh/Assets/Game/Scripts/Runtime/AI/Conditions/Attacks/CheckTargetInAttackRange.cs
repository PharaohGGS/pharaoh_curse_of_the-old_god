using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
    {
        private FightComponent _fight;

        protected override void OnStart()
        {
            if (_fight || agent.TryGetComponent(out _fight)) return;
            LogHandler.SendMessage($"Can't attack ennemies !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            if (!_fight) return NodeState.Failure;

            var target = blackboard.GetData<Transform>("target").position;
            var position = agent.transform.position;
            var range = _fight.range;

            return Vector2.Distance(position, target) > range 
                ? NodeState.Failure : NodeState.Success;
        }
    }
}