using Pharaoh.Tools.BehaviourTree.ScriptableObjects;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class WaitNode : ActionNode
    {
        public float duration = 1;
        private float _startTime = 0f;

        protected override void OnStart()
        {
            _startTime = Time.time;
        }

        protected override NodeState OnUpdate()
        {
            if (Time.time - _startTime > duration)
            {
                return NodeState.Success;
            }

            return NodeState.Running;
        }
    }
}