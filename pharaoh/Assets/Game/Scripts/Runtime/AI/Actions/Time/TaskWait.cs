using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskWait : ActionNode
    {
        [SerializeField] private float _startTime = 0f;

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
            if (state == NodeState.Failure || !blackboard.TryGetData("waitTime", out float waitTime))
            {
                return state;
            }

            float timeSince = Time.time - _startTime;
            bool isWaiting = timeSince < waitTime;
            blackboard.SetData("timeSince", timeSince);
            blackboard.SetData("isWaiting", isWaiting);

            return isWaiting ? NodeState.Running : NodeState.Failure;
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