using Pharaoh.Tools.BehaviourTree.CSharp;
using UnityEngine;
using Tree = Pharaoh.Tools.BehaviourTree.CSharp.Tree;

namespace Pharaoh.Gameplay.AI
{
    public class TaskPatrol : Node<GuardBT>
    {
        private Transform _transform;
        private Transform[] _waypoints;

        ///* Patrol attributes *///
        private int _currentWaypointIndex = 0;

        private float _waitTime = 1f;
        private float _waitCounter = 0f;
        private bool _waiting = false;

        public TaskPatrol(GuardBT tree, Transform transform, Transform[] waypoints) : base(tree)
        {
            _transform = transform;
            _waypoints = waypoints;
        }

        public override NodeState Evaluate()
        {
            state = NodeState.RUNNING;

            if (_waiting)
            {
                _waitCounter += Time.deltaTime;
                if (_waitCounter >= _waitTime)
                {
                    _waiting = false;
                }

                return state;
            }

            if (_waypoints.Length <= 0) return state;

            Transform wp = _waypoints[_currentWaypointIndex];
            if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            }
            else
            {
                _transform.position = Vector3.MoveTowards(
                    _transform.position, wp.position, 
                    tree.moveSpeed * Time.deltaTime);
                _transform.LookAt(wp.position);
            }
            
            return state;
        }
    }
}