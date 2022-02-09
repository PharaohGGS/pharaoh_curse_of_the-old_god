using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.Tools
{
    public abstract class CompositeNode : BNode
    {
        [HideInInspector] public List<BNode> children;

        public override BNode Clone()
        {
            CompositeNode composite = Instantiate(this);
            composite.children = children.ConvertAll(c => c.Clone());
            return composite;
        }
    }
}