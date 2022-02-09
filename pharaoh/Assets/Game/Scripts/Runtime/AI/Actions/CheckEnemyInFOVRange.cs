using System;
using BehaviourTree.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckEnemyInFOVRange : ActionNode
    {
        [SerializeField] private Collider[] _colliders;

        private void Awake()
        {
            _colliders = new Collider[8];
        }

        protected override NodeState OnUpdate()
        {
            var t = Blackboard.GetData("target") as Transform;

            if (t != null)
            {
                state = NodeState.Success;
                return state;
            }

            var size = Physics.OverlapSphereNonAlloc(
                Agent.transform.position, Agent.fovRange,
                _colliders, Agent.enemyLayerMask);

            if (size <= 0)
            {
                state = NodeState.Failure;
                return state;
            }

            
            Blackboard.SetData("target", _colliders[0].transform);
            LogHandler.SendMessage($"OverlapSphere found {size} colliders, {Blackboard.GetData("target")}", MessageType.Log);

            state = NodeState.Success;
            return state;
        }
    }
}