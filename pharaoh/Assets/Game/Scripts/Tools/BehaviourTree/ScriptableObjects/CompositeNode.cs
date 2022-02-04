using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Tools.BehaviourTree.ScriptableObjects
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> children;
    }
}