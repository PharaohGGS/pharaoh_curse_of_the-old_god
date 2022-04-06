using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInWeaponRange : ActionNode
    {
        private FightComponent _fight = null;

        protected override void OnStart()
        {
            if (_fight || agent.TryGetComponent(out _fight)) return;
            LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            if (!_fight) return NodeState.Failure;
            
            var target = blackboard.GetData<Transform>("target").position;
            var position = _fight.activeWeapon.transform.position;
            var range = _fight.activeWeapon.GetRange();
            var distance = Vector2.Distance(position, target);

            return distance <= range ? NodeState.Success : NodeState.Failure;
        }
    }
}