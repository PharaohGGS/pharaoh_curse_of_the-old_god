using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskPatrol : ActionNode
    {
        ///* Patrol attributes *///
        [SerializeField] private int currentWaypointIndex = 0;
        
        private Pawn _pawn;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }

            if (!_pawn.movement) return;

            blackboard.SetData("waitTime", _pawn.movement.timeBetweenWaypoints);
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;
            
            if (!_pawn || !_pawn.movement || _pawn.movement.waypoints.Length <= 0)
            {
                state = NodeState.Failure;
                return state;
            }

            var target = _pawn.movement.waypoints[currentWaypointIndex].position;
            if (Vector3.Distance(agent.transform.position, target) < 0.01f)
            {
                blackboard.SetData("isWaiting", true);
                blackboard.SetData("waitTime", _pawn.movement.timeBetweenWaypoints);
                agent.transform.position = target;
                currentWaypointIndex = (currentWaypointIndex + 1) % _pawn.movement.waypoints.Length;
            }
            else
            {
                agent.transform.position = Vector3.MoveTowards(
                    agent.transform.position, target,
                    _pawn.movement.moveSpeed * Time.deltaTime);
                target.y = agent.transform.position.y;
                agent.transform.LookAt(target);
            }

            return state;
        }
    }
}