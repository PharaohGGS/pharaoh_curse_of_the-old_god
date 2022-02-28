using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public abstract class TaskAttack<T> : ActionNode where T : DamagerData
    {
        private HealthComponent _healthComponent;

        private AttackComponent _attack = null;
        
        protected override void OnStart()
        {
            if (_attack) return;

            if (!agent.TryGetComponent(out _attack))
            {
                LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Failure;
            if (!_attack || !_attack.TryGetHolder<T>(out var holder) || 
                !blackboard.TryGetData("target", out Transform t))
            {
                return state;
            }

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
            
            _attack.Attack(holder);
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", holder.data.rate);
            return state;
        }

        private void OnTargetDeath()
        {
            blackboard.ClearData("target");
            _attack.target = null;
        }
    }
}