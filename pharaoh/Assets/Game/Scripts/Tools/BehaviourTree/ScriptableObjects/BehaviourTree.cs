using UnityEngine;

namespace Pharaoh.Tools.BehaviourTree.ScriptableObjects
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node root;

        public NodeState treeState = NodeState.Running;

        public NodeState Evaluate()
        {
            if (root.state == NodeState.Running)
            {
                treeState = root.Evaluate();
            }

            return treeState;
        }
    }
}