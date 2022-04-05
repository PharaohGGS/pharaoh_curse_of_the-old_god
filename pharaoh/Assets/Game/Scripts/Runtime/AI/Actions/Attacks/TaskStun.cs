using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskStun : ActionNode
    {
        [SerializeField] private bool stun;
        private AiMovement _aiMovement;

        protected override void OnStart()
        {
            if (_aiMovement || agent.TryGetComponent(out _aiMovement)) return;
            LogHandler.SendMessage($"[{agent.name}] Can't be stunned", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement || !_aiMovement.lastStunData) return NodeState.Failure;

            if (stun)
            {
                blackboard.SetData("isWaiting", true);
                blackboard.SetData("waitTime", _aiMovement.lastStunData.time);
            }

            return NodeState.Success;
        }

        protected override void OnStop()
        {
            if (!stun) _aiMovement.EndStun();
        }
    }
}