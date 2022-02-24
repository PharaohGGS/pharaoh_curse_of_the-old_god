using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public abstract class TaskAttack<T> : ActionNode where T : DamagerData
    {
        public T data;
        private HealthComponent _healthComponent;

        private AttackComponent _attack = null;
        
        protected override void OnStart()
        {
            if (_attack) return;

            if (agent.TryGetComponent(out _attack)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Failure;
            if (!_attack || !_attack.dataHolders.ContainsKey(data))
            {
                return state;
            }

            var hasTarget = blackboard.TryGetData("target", out Transform t);
            if (!hasTarget) return state;

            state = NodeState.Running;

            if (t != _attack.target && t.TryGetComponent(out HealthComponent healthComponent))
            {
                _attack.target = t;

                if (_healthComponent != healthComponent)
                {
                    _healthComponent?.OnDeath?.RemoveListener(OnTargetDeath);
                    _healthComponent = healthComponent;
                    _healthComponent?.OnDeath?.AddListener(OnTargetDeath);
                }
            }
            
            _attack.Attack(data);
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", data.attackRate);
            return state;
        }

        private void OnTargetDeath()
        {
            blackboard.ClearData("target");
            _attack.target = null;
        }
    }
}