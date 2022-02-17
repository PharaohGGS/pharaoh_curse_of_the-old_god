using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskAttack : ActionNode
    {
        private float _timeSinceLastAttack = 0f;

        private Transform _lastTarget;
        private HealthComponent _healthComponent;

        private EnemyPawn _pawn = null;
        private Weapon _weapon = null;
        
        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
                return;
            }

            if (_weapon != null) return;

            if (_pawn.weapon == null)
            {
                LogHandler.SendMessage($"[{_pawn.name}] Can't attack enemies", MessageType.Warning);
                return;
            }

            _weapon = _pawn.weapon;
        }

        protected override NodeState OnUpdate()
        {
            var t = blackboard.GetData("target") as Transform;
            state = NodeState.Running;

            if (t != null && t != _lastTarget)
            {
                _lastTarget = t;

                if (t.TryGetComponent(out HealthComponent healthComponent) && _healthComponent != healthComponent)
                {
                    _healthComponent?.OnDeath?.RemoveListener(OnTargetDeath);
                    _healthComponent = healthComponent;
                    _healthComponent?.OnDeath?.AddListener(OnTargetDeath);
                }
            }

            if (!t || !_weapon || !_weapon.data) return state;

            _timeSinceLastAttack += Time.deltaTime;
            if (_timeSinceLastAttack < _weapon.data.attackRate) return state;

            if (!_weapon.data.canThrow)
            {
                _pawn.Attack();
                _timeSinceLastAttack = 0f;
                return state;
            }

            if (_weapon.isThrown) return state;

            _weapon.Throw(t.position);
            _timeSinceLastAttack = 0f;
            return state;
        }

        private void OnTargetDeath()
        {
            blackboard.ClearData("target");
        }
    }
}