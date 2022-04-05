using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckHasTarget : ActionNode
    {
        protected override NodeState OnUpdate()
        {
            var hasTarget = !blackboard.TryGetData("target", out Transform target);
            return hasTarget && target ? NodeState.Success : NodeState.Failure;
        }
    }
}