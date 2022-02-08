using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskPatrol : ActionNode
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform[] _waypoints;

        ///* Patrol attributes *///
        [SerializeField] private int _currentWaypointIndex = 0;

        [SerializeField] private float _waitTime = 1f;
        [SerializeField] private float _waitCounter = 0f;
        [SerializeField] private bool _waiting = false;

        protected override void OnStart()
        {
            _transform = agent.transform;
            _waypoints = agent.waypoints;
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;

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
                    agent.moveSpeed * Time.deltaTime);
                _transform.LookAt(wp.position);
            }

            return state;
        }
    }
}