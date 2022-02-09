using BehaviourTree.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGoToTarget : ActionNode
    {
        protected override NodeState OnUpdate()
        {
            var target = Blackboard.GetData("target") as Transform;

            LogHandler.SendMessage(
                $"[{GetType().Name}] target : {(target == null ? "NULL" : target)}", 
                target == null ? MessageType.Error : MessageType.Log);

            if (target != null)
            {
                var tr = Agent.transform;
                if (Vector3.Distance(tr.position, target.position) > 0.01f)
                {
                    tr.position = Vector3.MoveTowards(
                        tr.position, target.position,
                        Agent.moveSpeed * Time.deltaTime);
                    tr.LookAt(target.position);
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}