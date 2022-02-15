using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGoToTarget : ActionNode
    {
        private EnemyAgent _agent = null;

        protected override void OnStart()
        {
            _agent = agent as EnemyAgent;
        }

        protected override NodeState OnUpdate()
        {
            var target = blackboard.GetData("target") as Transform;
            
            if (target != null && _agent)
            {
                var tr = agent.transform;
                if (Vector3.Distance(tr.position, target.position) > 0.01f)
                {
                    tr.position = Vector3.MoveTowards(
                        tr.position, target.position,
                        _agent.movement.moveSpeed * Time.deltaTime);
                    tr.LookAt(target.position);
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}