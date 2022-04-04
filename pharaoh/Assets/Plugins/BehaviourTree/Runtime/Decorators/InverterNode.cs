using System;
using BehaviourTree.Tools;

namespace BehaviourTree.Runtime.Decorators
{
    public class InverterNode : DecoratorNode
    {
        protected override NodeState OnUpdate()
        {
            state = child.Evaluate() switch
            {
                NodeState.Running => NodeState.Running,
                NodeState.Success => NodeState.Failure,
                NodeState.Failure => NodeState.Success,
                _ => throw new ArgumentOutOfRangeException()
            };

            return state;
        }
    }
}