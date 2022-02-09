using UnityEngine;

namespace BehaviourTree.Tools
{
    public class RootNode : BNode
    {
        [HideInInspector] public BNode child;

        protected override NodeState OnUpdate()
        {
            return child.Evaluate();
        }

        public override BNode Clone()
        {
            RootNode root = Instantiate(this);
            root.child = child.Clone();
            return root;
        }
    }
}