using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskChangeWaypoint : ActionNode
    {
        private AiMovement _aiMovement;
        
        protected override void OnStart()
        {
            if (!_aiMovement && !agent.TryGetComponent(out _aiMovement))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
                return;
            }
            
            if (!blackboard.ContainsData("currentWaypointIndex"))
            {
                blackboard.SetData("currentWaypointIndex", 0);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement || !_aiMovement.waypointHolder || _aiMovement.waypointHolder.childCount <= 0)
            {
                return NodeState.Failure;
            }

            var index = blackboard.GetData<int>("currentWaypointIndex");
            ((EnemyAgent) agent).StartWait(WaitType.Movement, _aiMovement.timeBetweenWaypoints);
            
            var nextIndex = (index + 1) % _aiMovement.waypointHolder.childCount;
            blackboard.SetData("currentWaypointIndex", nextIndex);

            var nextTarget = _aiMovement.waypointHolder.GetChild(nextIndex);
            blackboard.SetData("target", nextTarget);

            return NodeState.Success;
        }
    }
}