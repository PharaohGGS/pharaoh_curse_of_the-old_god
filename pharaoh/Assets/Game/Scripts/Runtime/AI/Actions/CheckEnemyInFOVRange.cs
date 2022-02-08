using System;
using BehaviourTree.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckEnemyInFOVRange : ActionNode
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Collider[] _colliders;

        private void Awake()
        {
            _colliders = new Collider[8];
        }

        protected override void OnStart()
        {
            _transform = agent.transform;
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
                _transform.position, agent.fovRange,
                _colliders, agent.enemyLayerMask);

            LogHandler.SendMessage($"OverlapSphere found {size} colliders", MessageType.Log);

            if (size > 0)
            {
                blackboard.SetData("target", _colliders[0].transform);
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}