using UnityEngine;

namespace BehaviourTree.Tools
{
    public abstract class DecoratorNode : BNode
    {
        [HideInInspector] public BNode child;

        public override BNode Clone()
        {
            DecoratorNode decorator = Instantiate(this);
            decorator.child = child.Clone();
            return decorator;
        }
    }
}