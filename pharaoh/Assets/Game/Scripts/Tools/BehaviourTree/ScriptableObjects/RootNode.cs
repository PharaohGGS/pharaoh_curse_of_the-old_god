using UnityEngine;

namespace Pharaoh.Tools.BehaviourTree.ScriptableObjects
{
    public class RootNode : Node
    {
        [HideInInspector] public Node child;

        protected override NodeState OnUpdate()
        {
            return child.Evaluate();
        }
    }
}