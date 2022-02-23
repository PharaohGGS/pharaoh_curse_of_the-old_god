using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskLookAtTarget : ActionNode
    {
        private Pawn _pawn = null;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (_pawn && blackboard.TryGetData("target", out Transform t))
            {
                agent.transform.LookAt(t.position);
            }

            state = NodeState.Running;
            return state;
        }
    }
}