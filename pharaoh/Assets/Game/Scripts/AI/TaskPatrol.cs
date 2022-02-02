using Pharaoh.Tools.BehaviourTree;
using UnityEngine;

namespace Pharaoh.Gameplay.Component.AI
{
    public class TaskPatrol : Node
    {
        private Transform _transform;
        private Transform[] _waypoints;

        public TaskPatrol(Transform transform, Transform[] waypoints)
        {
            _transform = transform;
            _waypoints = waypoints;
        }

        public override NodeState Evaluate()
        {
            state = NodeState.RUNNING;
            return state;
        }
    }
}