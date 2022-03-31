using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskDelay : ActionNode
    {
        [SerializeField] private bool useGearDelay;
        [HideInInspector, SerializeField] private float delay;
        [HideInInspector, SerializeField] private GearData gearData;

        protected override NodeState OnUpdate()
        {
            var time = useGearDelay && gearData ? gearData.delay : delay;
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", time);
            return NodeState.Running;
        }
    }
}