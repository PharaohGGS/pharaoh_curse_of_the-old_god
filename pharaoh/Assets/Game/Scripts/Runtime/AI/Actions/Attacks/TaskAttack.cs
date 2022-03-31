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
            if (!_fight) return NodeState.Failure;

            if (!blackboard.TryGetData("target", out Transform t) || t == null || !t.gameObject.activeInHierarchy)
            {
                LogHandler.SendMessage($"{agent.name} doesn't have a target to attack.", MessageType.Error);
                return NodeState.Failure;
            }

            var weapon = _fight.activeWeapon;
            if (!weapon || !weapon.isActiveAndEnabled)
            {
                LogHandler.SendMessage($"{agent.name} don't have a active weapon to attack.", MessageType.Error);
                return NodeState.Failure;
            }

            var distance = Vector2.Distance(agent.transform.position, t.position);
            var data = weapon.GetBaseData();
            var range = data.range;

            if (distance > range)
            {
                LogHandler.SendMessage($"{agent.name} can't attack at this distance ({distance} > {range})", MessageType.Warning);
                return NodeState.Failure;
            }

            if (weapon.isThrown)
            {
                LogHandler.SendMessage($"{agent.name} have already throw his gear.", MessageType.Warning);
                return NodeState.Failure;
            }
            
            _fight.Attack(t.gameObject);
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", data is MeleeGearData { throwable: true } meleeGearData
                ? meleeGearData.throwablePickingTime : data.rate);

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