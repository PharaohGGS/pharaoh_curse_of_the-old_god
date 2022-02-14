using UnityEngine;

namespace BehaviourTree.Tools
{
    public enum NodeState
    {
        Running,
        Success,
        Failure,
    }

    public abstract class BNode : ScriptableObject
    {
        [HideInInspector] public NodeState state;
        [HideInInspector] public bool hasStart = false;
        [HideInInspector] public string guid = null;
        [HideInInspector] public Vector2 position;
        [TextArea] public string description;

        [HideInInspector] public Blackboard blackboard;
        [HideInInspector] public AiAgent agent;

        public virtual BNode Clone() => Instantiate(this);
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
