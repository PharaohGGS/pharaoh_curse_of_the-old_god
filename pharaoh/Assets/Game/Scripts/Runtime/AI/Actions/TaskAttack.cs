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

        private EnemyAgent _agent = null;
        
        protected override void OnStart()
        {
            _agent = agent as EnemyAgent;
            if (_agent.damage == null)
            {
                LogHandler.SendMessage($"[{_agent.name}] Can't attack enemies", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            var t = blackboard.GetData("target") as Transform;

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

            _timeSinceLastAttack += Time.deltaTime;
            if (_agent && _timeSinceLastAttack >= _agent.damage.attackRate)
            {
                _healthComponent?.ApplyChange(_agent.damage.value, FloatOperation.Decrease);
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