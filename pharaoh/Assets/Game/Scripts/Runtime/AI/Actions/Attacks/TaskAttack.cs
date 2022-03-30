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
        [SerializeField] protected GearData gearData;

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
            if (!_attack || !gearData) return NodeState.Failure;

            if (!gearData.canAttack)
            {
                LogHandler.SendMessage($"{agent.name} can't attack with this data", MessageType.Warning);
                return NodeState.Failure;
            }

            if (!blackboard.TryGetData("target", out Transform t) || t == null || !t.gameObject.activeInHierarchy)
            {
                LogHandler.SendMessage($"{agent.name} doesn't have a target to attack.", MessageType.Error);
                return NodeState.Failure;
            }

            if (!_attack.dataGears.TryGetValue(gearData, out Gear gear))
            {
                LogHandler.SendMessage($"{agent.name} don't have a gear with this kind of data.", MessageType.Error);
                return NodeState.Failure;
            }

            if (gear.isThrown)
            {
                LogHandler.SendMessage($"{agent.name} have already throw his gear.", MessageType.Warning);
                return NodeState.Failure;
            }

            var rate = gearData.rate;
            if (gearData is MeleeGearData {throwable: true} meleeGearData)
            {
                rate = meleeGearData.throwablePickingTime;
            }

            _attack.Attack(gearData, t.gameObject);
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", rate);

            return NodeState.Success;
        }

        protected override void OnStop()
        {
            if (!_attack.currentTargetHealth) blackboard.ClearData("target");
        }
    }
}