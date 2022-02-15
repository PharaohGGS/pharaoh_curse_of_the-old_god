using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInFovRange : ActionNode
    {
        [SerializeField] private Collider[] colliders;
        
        private EnemyAgent _agent = null;

        protected override void OnStart()
        {
            colliders = new Collider[8];
            _agent = agent as EnemyAgent;
        }

        protected override NodeState OnUpdate()
        {
            var t = blackboard.GetData("target") as Transform;

            if (t != null)
            {
                state = NodeState.Success;
                return state;
            }

            if (_agent == null)
            {
                state = NodeState.Failure;
                return state;
            }

            var size = Physics.OverlapSphereNonAlloc(
                agent.transform.position, _agent.detection.fovRange,
                colliders, _agent.detection.detectionLayer);

            if (size <= 0)
            {
                state = NodeState.Failure;
                return state;
            }

            
            blackboard.SetData("target", colliders[0].transform);
            LogHandler.SendMessage($"OverlapSphere found {size} colliders, {blackboard.GetData("target")}", MessageType.Log);

            state = NodeState.Success;
            return state;
        }
    }
}