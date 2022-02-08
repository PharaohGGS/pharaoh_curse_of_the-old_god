using UnityEngine;

namespace BehaviourTree.Tools
{
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] public Node child;

        public override Node Clone()
        {
            DecoratorNode decorator = Instantiate(this);
            decorator.child = child.Clone();
            return decorator;
        }
    }
}