using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Tools.BehaviourTree.ScriptableObjects;
using UnityEngine;

namespace Pharaoh.AI.Composites
{
    public class SequencerNode : CompositeNode
    {
        private int _current;

        protected override void OnStart()
        {
            _current = 0;
        }

        protected override NodeState OnUpdate()
        {
            var child = children[_current];

            switch (child.Evaluate())
            {
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Failure:
                    return NodeState.Failure;
                case NodeState.Success:
                    _current++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _current == children.Count ? NodeState.Success : NodeState.Running;
        }
    }
}
