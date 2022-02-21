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
        
        private WeaponHolder _holder = null;
        
        protected override void OnStart()
        {
            if (_holder) return;

            var holder = agent.TryGetComponent(out WeaponHolder h) 
                ? h : agent.GetComponentInChildren<WeaponHolder>();

            if (!holder?.weapon)
            {
                LogHandler.SendMessage($"[{agent.name}] Can't attack enemies", MessageType.Warning);
                return;
            }

            _holder = holder;
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

            if (!hasTarget || !_holder || !_holder.data) return state;
            
            if (blackboard.TryGetData("isWaiting", out bool isWaiting) && isWaiting) return state;

            if (!_holder.data.canThrow)
            {
                _holder.Attack();
                WaitUntilNextAttack(_holder.data.attackRate);
                return state;
            }

            if (_holder.weapon.isThrown || _holder.transform.parent == null) return state;

            _holder.weapon.Throw(t.position + Vector3.up);
            WaitUntilNextAttack(_holder.data.attackRate);
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