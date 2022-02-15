using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskPatrol : ActionNode
    {
        ///* Patrol attributes *///
        [SerializeField] private int currentWaypointIndex = 0;
        
        private EnemyAgent _agent;

        protected override void OnStart()
        {
            _agent = agent as EnemyAgent;
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;

            if (!_agent || !_agent.movement || _agent.movement.waypoints.Length <= 0)
            {
                return state;
            }

            Transform wp = _agent.movement.waypoints[currentWaypointIndex];
            if (Vector3.Distance(agent.transform.position, wp.position) < 0.01f)
            {
                agent.transform.position = wp.position;
                currentWaypointIndex = (currentWaypointIndex + 1) % _agent.movement.waypoints.Length;
                state = NodeState.Success;
            }
            else
            {
                agent.transform.position = Vector3.MoveTowards(
                    agent.transform.position, wp.position,
                    _agent.movement.moveSpeed * Time.deltaTime);
                agent.transform.LookAt(wp.position);
            }

            return state;
        }
    }
}