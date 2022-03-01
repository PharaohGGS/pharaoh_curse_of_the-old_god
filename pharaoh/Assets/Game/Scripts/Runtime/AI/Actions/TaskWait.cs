using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskWait : ActionNode
    {
        private float _startTime = 0f;
        
        [SerializeField] private float timeSince;

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
            if (state == NodeState.Success || 
                !blackboard.TryGetData("waitTime", out float waitTime))
            {
                return state;
            }

            timeSince = Time.time - _startTime;
            state = timeSince <= waitTime
                ? NodeState.Running : NodeState.Success;

            blackboard.SetData("isWaiting", state == NodeState.Running);

            return state;
        }

        protected override void OnStop()
        {
            if (blackboard.TryGetData("isWaiting", out bool isWaiting) && isWaiting)
            {
                state = NodeState.Running;
            }
        }
    }
}