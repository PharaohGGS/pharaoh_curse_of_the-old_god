using System;
using BehaviourTree.Tools;
using UnityEngine;

namespace BehaviourTree.Runtime.Actions
{
    public class WaitNode : ActionNode
    {
        public float duration = 1;
        private float _startTime = 0f;

        private object _isWaiting = null;

        protected override void OnStart()
        {
            _startTime = Time.time;

            _isWaiting = blackboard.GetData("isWaiting");
            if (_isWaiting == null)
            {
                blackboard.SetData("isWaiting", false);
            }
        }

        protected override NodeState OnUpdate()
        {
            state = Time.time - _startTime > duration ? NodeState.Success : NodeState.Running;
            blackboard.SetData("isWaiting", state == NodeState.Running);
            return state;
        }
    }
}