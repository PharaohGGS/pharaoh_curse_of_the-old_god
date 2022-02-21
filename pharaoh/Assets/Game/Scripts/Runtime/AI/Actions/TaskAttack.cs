using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskAttack : ActionNode
    {
        private Transform _lastTarget;
        private HealthComponent _healthComponent;

        private EnemyPawn _pawn = null;
        private Weapon _weapon = null;
        
        protected override void OnStart()
        {
            if (!_pawn && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
                return;
            }

            if (_weapon) return;

            if (!_pawn.holder?.weapon)
            {
                LogHandler.SendMessage($"[{_pawn.name}] Can't attack enemies", MessageType.Warning);
                return;
            }

            _weapon = _pawn.holder.weapon;
        }

        protected override NodeState OnUpdate()
        {
            var hasTarget = blackboard.TryGetData("target", out Transform t);
            state = NodeState.Running;

            if (hasTarget && t != _lastTarget)
            {
                _lastTarget = t;

                if (t.TryGetComponent(out HealthComponent healthComponent) && 
                    _healthComponent != healthComponent)
                {
                    _healthComponent?.OnDeath?.RemoveListener(OnTargetDeath);
                    _healthComponent = healthComponent;
                    _healthComponent?.OnDeath?.AddListener(OnTargetDeath);
                }
            }

            if (!hasTarget || !_weapon || !_weapon.data) return state;
            
            if (blackboard.TryGetData("isWaiting", out bool isWaiting) && isWaiting) return state;

            if (!_weapon.data.canThrow)
            {
                _pawn.Attack();
                WaitUntilNextAttack(_weapon.data.attackRate);
                return state;
            }

            if (_weapon.isThrown || _weapon.transform.parent == null) return state;

            _weapon.Throw(t.position + Vector3.up);
            WaitUntilNextAttack(_weapon.data.attackRate);
            return state;
        }

        private void OnTargetDeath()
        {
            blackboard.ClearData("target");
        }

        private void WaitUntilNextAttack(float time)
        {
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", time);
        }
    }
}