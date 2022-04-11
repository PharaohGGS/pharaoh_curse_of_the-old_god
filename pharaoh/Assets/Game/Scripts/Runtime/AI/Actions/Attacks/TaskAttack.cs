using System;
using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskAttack : ActionNode
    {
        private FightComponent _fight = null;
        
        protected override void OnStart()
        {
            if (_fight) return;

            if (!agent.TryGetComponent(out _fight))
            {
                LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_fight || !_fight.activeWeapon) return NodeState.Failure;
            
            var target = blackboard.GetData<Transform>("target");
            if (!_fight.Attack(target)) return NodeState.Failure;
            
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", _fight.activeWeapon.GetRate());

            return NodeState.Success;
        }

        protected override void OnStop()
        {
            if (!_fight.hasTarget && state == NodeState.Success)
            {
                blackboard.ClearData("target");
            }
        }
    }
}