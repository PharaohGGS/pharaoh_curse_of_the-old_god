using UnityEngine;

namespace Pharaoh.Tools.BehaviourTree.ScriptableObjects
{
    public enum NodeState
    {
        Running,
        Success,
        Failure,
    }

    public abstract class Node : ScriptableObject
    {
        public NodeState state;
        public bool hasStart = false;
        public string guid = null;

        protected virtual void OnStart() {}
        protected virtual void OnStop() {}
        protected abstract NodeState OnUpdate();

        public NodeState Evaluate()
        {
            if (!hasStart)
            {
                OnStart();
                hasStart = true;
            }

            state = OnUpdate();

            if (state != NodeState.Running)
            {
                OnStop();
                hasStart = false;
            }

            return state;
        }
    }

}
