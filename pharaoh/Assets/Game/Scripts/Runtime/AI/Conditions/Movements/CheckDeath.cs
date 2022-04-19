using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class CheckDeath : ActionNode
    {
        private HealthComponent _health = null;

        protected override void OnStart()
        {
            if (_health || agent.TryGetComponent(out _health)) return;
            LogHandler.SendMessage($"No health !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            return _health && _health.isDead ? NodeState.Success : NodeState.Failure;
        }

        protected override void OnStop()
        {
            if (state == NodeState.Success) blackboard.ClearAllData();
        }
    }
}