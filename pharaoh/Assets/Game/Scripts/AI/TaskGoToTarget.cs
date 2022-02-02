using Pharaoh.Tools.BehaviourTree;
using UnityEngine;

namespace Pharaoh.Gameplay.AI
{
    public class TaskGoToTarget : Node<GuardBT>
    {
        private Transform _transform;

        public TaskGoToTarget(GuardBT tree, Transform transform) : base(tree)
        {
            _transform = transform;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform) GetData("target");

            if (target == null) return NodeState.FAILURE;

            if (Vector3.Distance(_transform.position, target.position) > 0.01f)
            {
                _transform.position = Vector3.MoveTowards(
                    _transform.position, target.position, 
                    tree.moveSpeed * Time.deltaTime);
                _transform.LookAt(target.position);
            }

            state = NodeState.RUNNING;
            return state;
        }
    }
}