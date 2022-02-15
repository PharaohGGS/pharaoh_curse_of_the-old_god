using System;
using BehaviourTree.Tools;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
    {
        private EnemyAgent _agent = null;

        protected override void OnStart()
        {
            _agent = agent as EnemyAgent;
        }

        protected override NodeState OnUpdate()
        {
            var t = blackboard.GetData("target") as Transform;

            if (t == null || !_agent)
            {
                state = NodeState.Failure;
                return state;
            }

            if (Vector3.Distance(agent.transform.position, t.position) <= _agent.detection.attackRange)
            {
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}