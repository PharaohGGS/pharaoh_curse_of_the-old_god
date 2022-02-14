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
        [SerializeField] private float fovRange = 6;
        [SerializeField] private LayerMask enemyLayerMask = 1<<6;

        private EnemyAgent _enemyAgent;

        private void Awake()
        {
            colliders = new Collider[8];
        }

        protected override void OnStart()
        {
            _enemyAgent = agent as EnemyAgent;
        }

        protected override NodeState OnUpdate()
        {
            var t = blackboard.GetData("target") as Transform;

            if (t != null)
            {
                state = NodeState.Success;
                return state;
            }

            var size = Physics.OverlapSphereNonAlloc(
                agent.transform.position, fovRange,
                colliders, enemyLayerMask);

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