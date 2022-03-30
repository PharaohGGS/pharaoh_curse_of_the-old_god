using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskDelay : ActionNode
    {
        [SerializeField] private float delay;

        protected override NodeState OnUpdate()
        {
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", delay);
            return NodeState.Running;
        }
    }
}