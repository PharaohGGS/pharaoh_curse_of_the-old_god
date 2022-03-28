using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskAttack : ActionNode
    {
        [SerializeField] protected GearType gearType;

        protected AttackComponent _attack = null;
        
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
            if (!_attack || gearType == GearType.Null) return NodeState.Failure;
            
            if (!_attack.TryGetHolder(gearType, out var holder))
            {
                LogHandler.SendMessage($"{agent.name} don't have a weapon of this type.", MessageType.Error);
                return NodeState.Failure;
            }

            var gearData = holder.gear.TryGetData(out MeleeGearData meleeGearData) ? meleeGearData : holder.gear.GetBaseData();

            if (!gearData.canAttack)
            {
                LogHandler.SendMessage($"{agent.name} can't attack with his weapon", MessageType.Warning);
                return NodeState.Failure;
            }

            if (!blackboard.TryGetData("target", out Transform t) || t == null || !t.gameObject.activeInHierarchy)
            {
                LogHandler.SendMessage($"{agent.name} doesn't have a target to attack.", MessageType.Error);
                return NodeState.Failure;
            }

            state = NodeState.Running;
            
            _attack.Attack(holder, t.gameObject);
            var rate = holder.gear.isThrown && meleeGearData ? meleeGearData.throwablePickingTime : gearData.rate;

            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", rate);

            return state;
        }

        protected override void OnStop()
        {
            if (!_attack.currentTargetHealth) blackboard.ClearData("target");
        }
    }
}