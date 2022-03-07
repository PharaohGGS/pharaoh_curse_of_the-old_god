using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskPatrol : ActionNode
    {
        ///* Patrol attributes *///
        [SerializeField] private int currentWaypointIndex = 0;
        [SerializeField] private bool ignoreHeight;

        private MovementComponent _movement;

        protected override void OnStart()
        {
            if (!agent.TryGetComponent(out _movement))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }

            if (!_movement) return;

            blackboard.SetData("waitTime", _movement.timeBetweenWaypoints);
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;
            
            if (!_movement || _movement.waypoints.Count <= 0)
            {
                state = NodeState.Failure;
                return state;
            }

            var target = _movement.waypoints[currentWaypointIndex].position;
            if (ignoreHeight) target.y = agent.transform.position.y;

            if (Vector2.Distance(agent.transform.position, target) < 0.01f)
            {
                blackboard.SetData("isWaiting", true);
                blackboard.SetData("waitTime", _movement.timeBetweenWaypoints);
                agent.transform.position = target;
                currentWaypointIndex = (currentWaypointIndex + 1) % _movement.waypoints.Count;
            }
            else
            {
                agent.transform.position = Vector2.MoveTowards(
                    agent.transform.position, target,
                    _movement.moveSpeed * Time.deltaTime);
                agent.transform.LookAt2D(target);
            }

            return state;
        }
    }
}