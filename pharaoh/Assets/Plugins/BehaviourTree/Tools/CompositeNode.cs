using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.Tools
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> children;

        public override Node Clone()
        {
            CompositeNode composite = Instantiate(this);
            composite.children = children.ConvertAll(c => c.Clone());
            return composite;
        }
    }
}