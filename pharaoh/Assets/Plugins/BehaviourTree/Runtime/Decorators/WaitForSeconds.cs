using BehaviourTree.Tools;
using UnityEngine;

namespace BehaviourTree.Runtime.Decorators
{
    public class WaitForSeconds : DecoratorNode
    {
        private float _startTime = 0f;
        [SerializeField] private float timeSince;
        
        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;

            if (blackboard.TryGetData("isWaiting", out bool isWaiting) && !isWaiting)
            {
                _startTime = Time.time;
                return state;
            }

            if (!blackboard.TryGetData("waitTime", out float waitTime))
            {
                return state;
            }
            
            timeSince = Time.time - _startTime;
            blackboard.SetData("isWaiting", timeSince <= waitTime);


            timeSince = Time.time - _startTime;
            blackboard.SetData("isWaiting", timeSince <= waitTime);
            
            return state;
        }

        protected override void OnStop()
        {
            child.Evaluate();
        }
    }
}