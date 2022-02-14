using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskPatrol : ActionNode
    {
        [SerializeField] private Transform transform;
        [SerializeField] private Transform[] waypoints;

        ///* Patrol attributes *///
        [SerializeField] private int currentWaypointIndex = 0;

        [SerializeField] private float waitTime = 1f;
        
        private float _waitCounter = 0f;
        
        private bool _waiting = false;

        private EnemyAgent _enemyAgent;

        protected override void OnStart()
        {
            _enemyAgent = agent as EnemyAgent;
            transform = agent.transform;
            waypoints = _enemyAgent.waypoints;
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;

            if (_waiting)
            {
                _waitCounter += Time.deltaTime;
                if (_waitCounter >= waitTime)
                {
                    _waiting = false;
                }

                return state;
            }

            if (waypoints.Length <= 0) return state;

            Transform wp = waypoints[currentWaypointIndex];
            if (Vector3.Distance(transform.position, wp.position) < 0.01f)
            {
                transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, wp.position,
                    _enemyAgent.moveSpeed * Time.deltaTime);
                transform.LookAt(wp.position);
            }

            return state;
        }
    }
}