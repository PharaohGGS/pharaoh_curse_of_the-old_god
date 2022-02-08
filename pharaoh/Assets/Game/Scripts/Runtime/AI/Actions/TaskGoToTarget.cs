using BehaviourTree.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGoToTarget : ActionNode
    {
        [SerializeField] private Transform _transform;
        protected override void OnStart()
        {
            _transform = agent.transform;
        }

        protected override NodeState OnUpdate()
        {
            var target = blackboard.GetData("target") as Transform;

            LogHandler.SendMessage($"[{this.GetType().Name}] target : {(target == null ? "NULL" : target)}", 
                target == null ? MessageType.Error : MessageType.Log);

            if (target != null)
            {
                if (Vector3.Distance(_transform.position, target.position) > 0.01f)
                {
                    _transform.position = Vector3.MoveTowards(
                        _transform.position, target.position,
                        agent.moveSpeed * Time.deltaTime);
                    _transform.LookAt(target.position);
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}