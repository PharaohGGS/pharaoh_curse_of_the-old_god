using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
    {
        [SerializeField] private float distance;

        private FightComponent _fight;

        protected override void OnStart()
        {
            if (_fight) return;

            if (agent.TryGetComponent(out _fight)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_fight || !blackboard.TryGetData("target", out Transform t)) return NodeState.Failure;
            distance = Vector2.Distance(agent.transform.position, t.position);
            return distance > _fight.attackRange ? NodeState.Failure : NodeState.Success;
        }
    }
}