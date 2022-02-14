using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGoToTarget : ActionNode
    {
        private EnemyAgent _enemyAgent;

        protected override void OnStart()
        {
            _enemyAgent = agent as EnemyAgent;
        }

        protected override NodeState OnUpdate()
        {
            var target = blackboard.GetData("target") as Transform;

            //LogHandler.SendMessage(
            //    $"[{GetType().Name}] target : {(target == null ? "NULL" : target)}", 
            //    target == null ? MessageType.Error : MessageType.Log);

            if (target != null)
            {
                var tr = agent.transform;
                if (Vector3.Distance(tr.position, target.position) > 0.01f)
                {
                    tr.position = Vector3.MoveTowards(
                        tr.position, target.position,
                        _enemyAgent.moveSpeed * Time.deltaTime);
                    tr.LookAt(target.position);
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}