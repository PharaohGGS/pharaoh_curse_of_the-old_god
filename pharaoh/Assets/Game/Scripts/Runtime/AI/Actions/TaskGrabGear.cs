using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGrabGear : ActionNode
    {
        private AttackComponent _attack = null;
        
        protected override void OnStart()
        {
            if (_attack) return;

            if (!agent.TryGetComponent(out _attack))
            {
                LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_attack || !blackboard.TryGetData("target", out Transform t)) return NodeState.Running;
            if (!t.TryGetComponent(out Gear gear) || !gear.isThrown || !gear.isGrounded) return NodeState.Running;

            gear.SocketAttach(true);
            blackboard.ClearData("target");
            return NodeState.Success;
        }
    }
}