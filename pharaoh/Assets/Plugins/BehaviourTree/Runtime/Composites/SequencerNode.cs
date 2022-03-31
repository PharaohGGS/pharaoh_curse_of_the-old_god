using System;
using BehaviourTree.Tools;
using UnityEngine;

namespace BehaviourTree.Runtime.Composites
{
    /// <summary>
    /// Fail when one of the child is failing
    /// Success when all of the child success
    /// Running when one child is running
    /// </summary>
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
                        return NodeState.Failure;
                    default:
                        return NodeState.Success;
                }
            }
            
            return anyChildRunning ? NodeState.Running : NodeState.Success;
        }
    }
}
