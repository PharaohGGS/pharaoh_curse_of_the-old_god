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
        
        private EnemyPawn _pawn = null;

        protected override void OnStart()
        {
            colliders = new Collider[8];
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            var t = blackboard.GetData("target") as Transform;

            if (t != null)
            {
                state = NodeState.Success;
                return state;
            }

            if (_pawn == null || _pawn.holder.weapon.transform.parent == null)
            {
                state = NodeState.Failure;
                return state;
            }

            var size = Physics.OverlapSphereNonAlloc(
                agent.transform.position, _pawn.detection.fovRange,
                colliders, _pawn.detection.detectionLayer);

            if (size <= 0)
            {
                state = NodeState.Failure;
                return state;
            }

            blackboard.SetData("target", colliders[0].transform);

            state = NodeState.Success;
            return state;
        }
    }
}