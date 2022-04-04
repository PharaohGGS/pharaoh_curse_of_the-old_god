using System;
using BehaviourTree.Tools;
using UnityEngine;

namespace BehaviourTree.Runtime.Actions
{
    public class WaitNode : ActionNode
    {
        public float duration = 1;
        private float _startTime = 0f;

        public float timeSince;
        
        protected override void OnStart()
        {
            _startTime = Time.time;
            if (!blackboard.ContainsData("isWaiting"))
            {
                blackboard.SetData("isWaiting", false);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (state == NodeState.Success) return state;

            timeSince = Time.time - _startTime;
            state = timeSince <= duration 
                ? NodeState.Running : NodeState.Success;

            blackboard.SetData("isWaiting", state == NodeState.Running);
            
            return state;
        }

        protected override void OnStop()
        {
            if (blackboard.GetData<bool>("isWaiting"))
            {
                state = NodeState.Running;
            }
        }
    }
}