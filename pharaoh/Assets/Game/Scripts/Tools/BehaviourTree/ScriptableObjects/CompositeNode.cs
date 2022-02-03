using System.Collections.Generic;

namespace Pharaoh.Tools.BehaviourTree.ScriptableObjects
{
    public abstract class CompositeNode : Node
    {
        public List<Node> children;
    }
}