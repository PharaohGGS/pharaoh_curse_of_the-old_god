using System;
using BehaviourTree.Tools;
using UnityEngine;

namespace BehaviourTree.Runtime.Composites
{
    public class SequencerNode : CompositeNode
    {
       protected override NodeState OnUpdate()
       {
            bool anyChildRunning = false;
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success: 
                        continue;
                    case NodeState.Running:
                        anyChildRunning = true;
                        continue;
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    default:
                        state = NodeState.Success;
                        return state;
                }
            }
            
            state = anyChildRunning ? NodeState.Running : NodeState.Success;
            return state;
        }
    }
}
