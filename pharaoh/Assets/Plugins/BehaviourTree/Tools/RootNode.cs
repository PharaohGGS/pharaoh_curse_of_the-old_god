using UnityEngine;

namespace BehaviourTree.Tools
{
    public class RootNode : Node
    {
        [HideInInspector] public Node child;

        protected override NodeState OnUpdate()
        {
            return child.Evaluate();
        }

        public override Node Clone()
        {
            RootNode root = Instantiate(this);
            root.child = child.Clone();
            return root;
        }
    }
}