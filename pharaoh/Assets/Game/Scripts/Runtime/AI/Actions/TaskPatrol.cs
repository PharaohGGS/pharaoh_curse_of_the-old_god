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
        
        private EnemyPawn _pawn;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;
            var isWaiting = blackboard.GetData("isWaiting");
            
            if (isWaiting is true) return state;
            
            if (!_pawn || !_pawn.movement || _pawn.movement.waypoints.Length <= 0)
            {
                state = NodeState.Failure;
                return state;
            }

            var wp = _pawn.movement.waypoints[currentWaypointIndex];
            if (Vector3.Distance(agent.transform.position, wp.position) < 0.01f)
            {
                agent.transform.position = wp.position;
                blackboard.SetData("isWaiting", true);
                currentWaypointIndex = (currentWaypointIndex + 1) % _pawn.movement.waypoints.Length;
            }
            else
            {
                agent.transform.position = Vector3.MoveTowards(
                    agent.transform.position, wp.position,
                    _pawn.movement.moveSpeed * Time.deltaTime);
                agent.transform.LookAt(wp.position);
            }

            return state;
        }
    }
}