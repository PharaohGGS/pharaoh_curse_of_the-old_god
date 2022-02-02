using Pharaoh.Tools.BehaviourTree;
using UnityEngine;

namespace Pharaoh.Gameplay.AI
{
    public class CheckEnemyInFOVRange : Node<GuardBT>
    {
        private Transform _transform;

        public CheckEnemyInFOVRange(GuardBT tree, Transform transform) : base(tree)
        {
            _transform = transform;
        }

        public override NodeState Evaluate()
        {
            var t = GetData("target");

            if (t == null)
            {
                var colliders = Physics.OverlapSphere(
                    _transform.position, tree.fovRange, tree.enemyLayerMask);

                if (colliders.Length > 0)
                {
                    var root = this.GetRootNode();
                    root.SetData("target", colliders[0].transform);
                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}