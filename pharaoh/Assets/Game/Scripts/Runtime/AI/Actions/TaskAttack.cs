using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskAttack : ActionNode
    {
        [SerializeField] private float attackRate = 0.5f;
        private float _timeSinceLastAttack = 0f;

        private Transform _lastTarget;
        private HealthComponent _healthComponent;
        private DamageComponent _damageComponent;

        protected override void OnStart()
        {
            if (_damageComponent != null || !agent.TryGetComponent(out _damageComponent))
            {
                LogHandler.SendMessage($"Can't attack enemies", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            var t = blackboard.GetData("target") as Transform;

            if (t != _lastTarget && t != null && t.TryGetComponent(out HealthComponent healthComponent))
            {
                _lastTarget = t;

                if (_healthComponent != null)
                {
                    _healthComponent.OnDeath.RemoveListener(OnTargetDeath);
                    _healthComponent = healthComponent;
                    _healthComponent.OnDeath.AddListener(OnTargetDeath);
                }
            }

            _timeSinceLastAttack += Time.deltaTime;
            if (_timeSinceLastAttack >= attackRate)
            {
                _healthComponent?.ApplyChange(_damageComponent.Damage, FloatOperation.Decrease);
                _timeSinceLastAttack = 0f;
            }

            state = NodeState.Running;
            return state;
        }

        private void OnTargetDeath()
        {
            blackboard.ClearData("target");
        }
    }
}